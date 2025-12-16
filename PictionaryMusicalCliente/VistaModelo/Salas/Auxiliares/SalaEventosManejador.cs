using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Properties.Langs;
using System;
using System.Linq;
using System.Windows;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares
{
    /// <summary>
    /// Gestiona los eventos del servicio de salas.
    /// </summary>
    public sealed class SalaEventosManejador : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISalasServicio _salasServicio;
        private readonly IAvisoServicio _avisoServicio;
        private readonly string _codigoSala;
        private readonly string _nombreUsuarioSesion;
        private readonly string _creadorSala;

        private bool _salaCancelada;
        private bool _expulsionProcesada;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="SalaEventosManejador"/>.
        /// </summary>
        public SalaEventosManejador(
            ISalasServicio salasServicio,
            IAvisoServicio avisoServicio,
            string codigoSala,
            string nombreUsuarioSesion,
            string creadorSala)
        {
            _salasServicio = salasServicio ?? 
                throw new ArgumentNullException(nameof(salasServicio));
            _avisoServicio = avisoServicio ?? 
                throw new ArgumentNullException(nameof(avisoServicio));
            _codigoSala = codigoSala ?? string.Empty;
            _nombreUsuarioSesion = nombreUsuarioSesion ?? string.Empty;
            _creadorSala = creadorSala ?? string.Empty;

            Suscribir();
        }

        /// <summary>
        /// Evento cuando un jugador se une a la sala.
        /// </summary>
        public event Action<string> JugadorSeUnio;

        /// <summary>
        /// Evento cuando un jugador sale de la sala.
        /// </summary>
        public event Action<string> JugadorSalio;

        /// <summary>
        /// Evento cuando un jugador es expulsado.
        /// </summary>
        public event Action<string> JugadorExpulsado;

        /// <summary>
        /// Evento cuando la sala es actualizada.
        /// </summary>
        public event Action<DTOs.SalaDTO> SalaActualizada;

        /// <summary>
        /// Evento cuando la sala es cancelada por el anfitrion.
        /// </summary>
        public event Action SalaCanceladaPorAnfitrion;

        /// <summary>
        /// Evento cuando el usuario actual es expulsado.
        /// </summary>
        public event Action ExpulsionPropia;

        /// <summary>
        /// Obtiene si la sala fue cancelada.
        /// </summary>
        public bool SalaCancelada => _salaCancelada;

        /// <summary>
        /// Obtiene si la expulsion ya fue procesada.
        /// </summary>
        public bool ExpulsionProcesada => _expulsionProcesada;

        /// <summary>
        /// Marca la sala como cancelada.
        /// </summary>
        public void MarcarSalaCancelada()
        {
            _salaCancelada = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Desuscribir();
        }

        private void Suscribir()
        {
            _salasServicio.JugadorSeUnio += SalasServicio_JugadorSeUnio;
            _salasServicio.JugadorSalio += SalasServicio_JugadorSalio;
            _salasServicio.JugadorExpulsado += SalasServicio_JugadorExpulsado;
            _salasServicio.SalaActualizada += SalasServicio_SalaActualizada;
            _salasServicio.SalaCancelada += SalasServicio_SalaCancelada;
        }

        private void Desuscribir()
        {
            _salasServicio.JugadorSeUnio -= SalasServicio_JugadorSeUnio;
            _salasServicio.JugadorSalio -= SalasServicio_JugadorSalio;
            _salasServicio.JugadorExpulsado -= SalasServicio_JugadorExpulsado;
            _salasServicio.SalaActualizada -= SalasServicio_SalaActualizada;
            _salasServicio.SalaCancelada -= SalasServicio_SalaCancelada;
        }

        private void SalasServicio_JugadorSeUnio(
            object remitente, 
            string nombreJugador)
        {
            EjecutarEnDispatcher(() =>
            {
                if (string.IsNullOrWhiteSpace(nombreJugador))
                {
                    return;
                }

                JugadorSeUnio?.Invoke(nombreJugador);
            });
        }

        private void SalasServicio_JugadorSalio(
            object remitente, 
            string nombreJugador)
        {
            EjecutarEnDispatcher(() =>
            {
                if (string.IsNullOrWhiteSpace(nombreJugador))
                {
                    return;
                }

                if (EsCreador(nombreJugador))
                {
                    CancelarSalaPorAnfitrion();
                    return;
                }

                JugadorSalio?.Invoke(nombreJugador);
            });
        }

        private void SalasServicio_SalaCancelada(
            object remitente, 
            string codigoSala)
        {
            EjecutarEnDispatcher(() =>
            {
                if (!EsMismaSala(codigoSala))
                {
                    return;
                }

                CancelarSalaPorAnfitrion();
            });
        }

        private void SalasServicio_JugadorExpulsado(
            object remitente, 
            string nombreJugador)
        {
            EjecutarEnDispatcher(() =>
            {
                if (EsUsuarioActual(nombreJugador))
                {
                    ManejarExpulsionPropia();
                }
                else
                {
                    JugadorExpulsado?.Invoke(nombreJugador);
                }
            });
        }

        private void SalasServicio_SalaActualizada(
            object remitente, 
            DTOs.SalaDTO sala)
        {
            if (sala == null || !EsMismaSala(sala.Codigo))
            {
                return;
            }

            EjecutarEnDispatcher(() =>
            {
                bool usuarioSiguePresente = sala.Jugadores?.Any(jugador =>
                    string.Equals(
                        jugador,
                        _nombreUsuarioSesion,
                        StringComparison.OrdinalIgnoreCase)) == true;

                if (!_expulsionProcesada && !usuarioSiguePresente)
                {
                    ManejarExpulsionPropia();
                    return;
                }

                bool anfitrionSiguePresente = sala.Jugadores?.Any(jugador =>
                    string.Equals(
                        jugador,
                        _creadorSala,
                        StringComparison.OrdinalIgnoreCase)) == true;

                if (!anfitrionSiguePresente)
                {
                    CancelarSalaPorAnfitrion();
                    return;
                }

                SalaActualizada?.Invoke(sala);
            });
        }

        private void ManejarExpulsionPropia()
        {
            if (_expulsionProcesada)
            {
                return;
            }

            _expulsionProcesada = true;
            _avisoServicio.Mostrar(Lang.expulsarJugadorTextoFuisteExpulsado);
            ExpulsionPropia?.Invoke();
        }

        private void CancelarSalaPorAnfitrion()
        {
            if (_salaCancelada)
            {
                return;
            }

            _salaCancelada = true;
            _logger.Warn(
                "La sala se cancelo porque el anfitrion abandono la partida.");
            _avisoServicio.Mostrar(Lang.partidaTextoHostCanceloSala);
            SalaCanceladaPorAnfitrion?.Invoke();
        }

        private bool EsCreador(string nombreJugador)
        {
            return string.Equals(
                nombreJugador,
                _creadorSala,
                StringComparison.OrdinalIgnoreCase);
        }

        private bool EsMismaSala(string codigo)
        {
            return string.Equals(
                codigo,
                _codigoSala,
                StringComparison.OrdinalIgnoreCase);
        }

        private bool EsUsuarioActual(string nombreJugador)
        {
            return string.Equals(
                nombreJugador,
                _nombreUsuarioSesion,
                StringComparison.OrdinalIgnoreCase);
        }

        private static void EjecutarEnDispatcher(Action accion)
        {
            if (accion == null)
            {
                return;
            }

            var dispatcher = Application.Current?.Dispatcher;

            if (dispatcher == null || dispatcher.CheckAccess())
            {
                accion();
            }
            else
            {
                dispatcher.BeginInvoke(accion);
            }
        }
    }
}
