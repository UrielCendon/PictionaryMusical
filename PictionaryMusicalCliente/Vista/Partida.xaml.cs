using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.VistaModelo.Salas;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace PictionaryMusicalCliente.Vista
{
    /// <summary>
    /// Control de usuario para el area de juego de la partida (lienzo).
    /// </summary>
    public partial class Partida : UserControl
    {
        private readonly List<Point> _puntosBorrador = new();
        private bool _borradoEnProgreso;

        private const int MaximoPuntosTrazo = 2000;

        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public Partida()
        {
            InitializeComponent();
            Loaded += AlCargarPartida;
        }

        private void AlCargarPartida(object remitente, RoutedEventArgs argumentosEvento)
        {
            if (DataContext is PartidaVistaModelo vistaModelo)
            {
                vistaModelo.NotificarCambioHerramienta = EstablecerHerramienta;
                vistaModelo.AplicarEstiloLapiz = AplicarEstiloLapiz;
                vistaModelo.ActualizarFormaGoma = ActualizarFormaGoma;
                vistaModelo.LimpiarTrazos = LimpiarLienzo;

                vistaModelo.TrazoRecibidoServidor += AlRecibirTrazoDelServidor;

                RegistrarEventosLienzo();
            }
        }

        private void RegistrarEventosLienzo()
        {
            if (inkLienzoDibujo == null)
            {
                return;
            }

            inkLienzoDibujo.StrokeCollected += AlRecolectarTrazoEnLienzo;
            inkLienzoDibujo.PreviewMouseLeftButtonDown += AlPresionarBotonIzquierdoEnLienzo;
            inkLienzoDibujo.PreviewMouseMove += AlMoverRatonEnLienzo;
            inkLienzoDibujo.PreviewMouseLeftButtonUp += AlSoltarBotonIzquierdoEnLienzo;
        }

        private void AlRecolectarTrazoEnLienzo(object remitente,
            InkCanvasStrokeCollectedEventArgs argumentosEvento)
        {
            if (argumentosEvento.Stroke == null || !(DataContext is PartidaVistaModelo vistaModelo)
                || !vistaModelo.PuedeDibujar)
            {
                return;
            }

            if (argumentosEvento.Stroke.StylusPoints.Count > MaximoPuntosTrazo)
            {
                inkLienzoDibujo.Strokes.Remove(argumentosEvento.Stroke);
                vistaModelo.NotificarTrazoDemasiadoGrande?.Invoke();
                return;
            }

            ResultadoOperacion<TrazoDTO> resultadoTrazo = 
                ConvertirLineaATrazo(argumentosEvento.Stroke, false);
            if (resultadoTrazo.Exitoso)
            {
                vistaModelo.EnviarTrazoAlServidor?.Invoke(resultadoTrazo.Valor);
            }
        }

        private void AlPresionarBotonIzquierdoEnLienzo(object remitente,
            MouseButtonEventArgs argumentosEvento)
        {
            if (DataContext is PartidaVistaModelo vistaModelo && vistaModelo.PuedeDibujar
                && vistaModelo.EsHerramientaBorrador)
            {
                _borradoEnProgreso = true;
                _puntosBorrador.Clear();
                _puntosBorrador.Add(argumentosEvento.GetPosition(inkLienzoDibujo));
            }
        }

        private void AlMoverRatonEnLienzo(object remitente, MouseEventArgs argumentosEvento)
        {
            if (_borradoEnProgreso)
            {
                _puntosBorrador.Add(argumentosEvento.GetPosition(inkLienzoDibujo));
            }
        }

        private void AlSoltarBotonIzquierdoEnLienzo(object remitente, 
            MouseButtonEventArgs argumentosEvento)
        {
            if (_borradoEnProgreso && DataContext is PartidaVistaModelo vistaModelo)
            {
                _borradoEnProgreso = false;

                if (_puntosBorrador.Count > MaximoPuntosTrazo)
                {
                    vistaModelo.NotificarTrazoDemasiadoGrande?.Invoke();
                    _puntosBorrador.Clear();
                    return;
                }

                ResultadoOperacion<TrazoDTO> resultadoTrazo = ConvertirPuntosATrazoBorrador(
                    _puntosBorrador, 
                    vistaModelo.Grosor);
                
                if (resultadoTrazo.Exitoso)
                {
                    vistaModelo.EnviarTrazoAlServidor?.Invoke(resultadoTrazo.Valor);
                }

                _puntosBorrador.Clear();
            }
        }

        private static ResultadoOperacion<TrazoDTO> ConvertirPuntosATrazoBorrador(
            IEnumerable<Point> puntos, 
            double grosor)
        {
            if (puntos == null)
            {
                return ResultadoOperacion<TrazoDTO>.Fallo();
            }

            var listaPuntos = puntos.ToList();
            if (listaPuntos.Count == 0)
            {
                return ResultadoOperacion<TrazoDTO>.Fallo();
            }

            TrazoDTO trazo = new TrazoDTO
            {
                PuntosX = listaPuntos.Select(punto => punto.X).ToArray(),
                PuntosY = listaPuntos.Select(punto => punto.Y).ToArray(),
                ColorHex = Colors.Transparent.ToString(),
                Grosor = grosor,
                EsBorrado = true
            };

            return ResultadoOperacion<TrazoDTO>.Exito(trazo);
        }

        private static ResultadoOperacion<TrazoDTO> ConvertirLineaATrazo(
            Stroke linea, 
            bool esBorrado)
        {
            if (linea == null)
            {
                return ResultadoOperacion<TrazoDTO>.Fallo();
            }

            var puntos = linea.StylusPoints;

            TrazoDTO trazo = new TrazoDTO
            {
                PuntosX = puntos.Select(punto => punto.X).ToArray(),
                PuntosY = puntos.Select(punto => punto.Y).ToArray(),
                ColorHex = ColorAHex(linea.DrawingAttributes.Color),
                Grosor = linea.DrawingAttributes.Width,
                EsBorrado = esBorrado
            };

            return ResultadoOperacion<TrazoDTO>.Exito(trazo);
        }

        private void AlRecibirTrazoDelServidor(TrazoDTO trazo)
        {
            if (trazo == null || inkLienzoDibujo == null)
            {
                return;
            }

            if (trazo.EsBorrado)
            {
                AplicarBorradoRemoto(trazo);
                return;
            }

            if (trazo.PuntosX == null || trazo.PuntosY == null)
            {
                return;
            }

            var puntos = new StylusPointCollection();
            for (int i = 0; i < Math.Min(trazo.PuntosX.Length, trazo.PuntosY.Length); i++)
            {
                puntos.Add(new StylusPoint(trazo.PuntosX[i], trazo.PuntosY[i]));
            }

            var atributos = new DrawingAttributes
            {
                Color = (Color)ColorConverter.ConvertFromString(trazo.ColorHex ?? 
                    Colors.Black.ToString()),
                Width = trazo.Grosor,
                Height = trazo.Grosor,
                FitToCurve = false,
                IgnorePressure = true
            };

            var linea = new Stroke(puntos)
            {
                DrawingAttributes = atributos
            };

            inkLienzoDibujo.Strokes.Add(linea);
        }

        private void AplicarBorradoRemoto(TrazoDTO trazo)
        {
            if (trazo.PuntosX == null || trazo.PuntosY == null)
            {
                return;
            }

            if (trazo.EsLimpiarTodo)
            {
                inkLienzoDibujo.Strokes.Clear();
                return;
            }

            var puntosTrayectoria = new List<Point>();
            for (int i = 0; i < Math.Min(trazo.PuntosX.Length, trazo.PuntosY.Length); i++)
            {
                puntosTrayectoria.Add(new Point(trazo.PuntosX[i], trazo.PuntosY[i]));
            }

            if (puntosTrayectoria.Count == 0)
            {
                return;
            }

            var tamano = Math.Max(1, trazo.Grosor);
            var formaBorrador = new EllipseStylusShape(tamano, tamano);
            var lineasActuales = inkLienzoDibujo.Strokes.ToList();

            foreach (var linea in lineasActuales)
            {
                var resultado = linea.GetEraseResult(puntosTrayectoria, formaBorrador);
                inkLienzoDibujo.Strokes.Remove(linea);

                if (resultado != null && resultado.Count > 0)
                {
                    inkLienzoDibujo.Strokes.Add(resultado);
                }
            }
        }

        private void EstablecerHerramienta(bool esLapiz)
        {
            if (inkLienzoDibujo == null)
            {
                return;
            }

            inkLienzoDibujo.EditingMode = esLapiz
                ? InkCanvasEditingMode.Ink
                : InkCanvasEditingMode.EraseByPoint;

            if (esLapiz)
            {
                AplicarEstiloLapiz();
            }
            else
            {
                ActualizarFormaGoma();
            }
        }

        private void AplicarEstiloLapiz()
        {
            if (inkLienzoDibujo == null || !(DataContext is PartidaVistaModelo vistaModelo))
            {
                return;
            }

            inkLienzoDibujo.DefaultDrawingAttributes = new DrawingAttributes
            {
                Color = vistaModelo.Color,
                Width = vistaModelo.Grosor,
                Height = vistaModelo.Grosor,
                FitToCurve = false,
                IgnorePressure = true
            };
        }

        private void ActualizarFormaGoma()
        {
            if (inkLienzoDibujo == null || !(DataContext is PartidaVistaModelo vistaModelo))
            {
                return;
            }

            var tamano = Math.Max(1, vistaModelo.Grosor);
            inkLienzoDibujo.EraserShape = new EllipseStylusShape(tamano, tamano);
        }

        private void LimpiarLienzo()
        {
            inkLienzoDibujo?.Strokes.Clear();
        }

        private static string ColorAHex(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
