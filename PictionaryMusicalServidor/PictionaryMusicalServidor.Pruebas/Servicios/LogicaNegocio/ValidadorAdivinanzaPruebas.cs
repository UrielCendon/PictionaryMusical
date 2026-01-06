using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class ValidadorAdivinanzaPruebas
    {
        private const int IdCancionPrueba = 1;
        private const int PuntosPorTiempoPrueba = 50;
        private const string MensajeCorrecto = "gasolina";
        private const string MensajeIncorrecto = "mensaje incorrecto";
        private const string MensajeVacio = "";
        private const string MensajeSoloEspacios = "   ";
        private const string MensajeAciertoProtocolo = "ACIERTO:jugador:75";
        private const int PuntosProtocolo = 75;
        private const string NombreJugadorPrueba = "JugadorPrueba";
        private const string IdConexionPrueba = "conexion-prueba";
        private const int PuntajeInicialCero = 0;
        private const int PuntosObtenidos = 100;

        private Mock<ICatalogoCanciones> _mockCatalogo;
        private Mock<IGestorTiemposPartida> _mockGestorTiempos;
        private ValidadorAdivinanza _validador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockCatalogo = new Mock<ICatalogoCanciones>();
            _mockGestorTiempos = new Mock<IGestorTiemposPartida>();
            _validador = new ValidadorAdivinanza(
                _mockCatalogo.Object, 
                _mockGestorTiempos.Object);
        }
        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionCatalogoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ValidadorAdivinanza(null, _mockGestorTiempos.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionGestorTiemposNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ValidadorAdivinanza(_mockCatalogo.Object, null));
        }        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_RetornaFalsoJugadorNulo()
        {
            bool resultado = _validador.JugadorPuedeAdivinar(null, EstadoPartida.Jugando);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_RetornaFalsoEstadoEnSalaEspera()
        {
            var jugador = CrearJugadorPrueba();

            bool resultado = _validador.JugadorPuedeAdivinar(
                jugador, 
                EstadoPartida.EnSalaEspera);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_RetornaFalsoEstadoFinalizada()
        {
            var jugador = CrearJugadorPrueba();

            bool resultado = _validador.JugadorPuedeAdivinar(
                jugador, 
                EstadoPartida.Finalizada);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_RetornaFalsoSiEsDibujante()
        {
            var jugador = CrearJugadorPrueba();
            jugador.EsDibujante = true;

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_RetornaFalsoSiYaAdivino()
        {
            var jugador = CrearJugadorPrueba();
            jugador.YaAdivino = true;

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_RetornaVerdaderoCondicionesCorrectas()
        {
            var jugador = CrearJugadorPrueba();

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsTrue(resultado);
        }        [TestMethod]
        public void Prueba_VerificarAcierto_RetornaFalsoMensajeNulo()
        {
            int puntos;

            bool resultado = _validador.VerificarAcierto(IdCancionPrueba, null, out puntos);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RetornaFalsoMensajeVacio()
        {
            int puntos;

            bool resultado = _validador.VerificarAcierto(
                IdCancionPrueba, 
                MensajeVacio, 
                out puntos);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RetornaFalsoMensajeSoloEspacios()
        {
            int puntos;

            bool resultado = _validador.VerificarAcierto(
                IdCancionPrueba, 
                MensajeSoloEspacios, 
                out puntos);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RetornaVerdaderoRespuestaCorrecta()
        {
            _mockCatalogo
                .Setup(catalogo => catalogo.ValidarRespuesta(IdCancionPrueba, MensajeCorrecto))
                .Returns(true);
            _mockGestorTiempos
                .Setup(gestor => gestor.CalcularPuntosPorTiempo())
                .Returns(PuntosPorTiempoPrueba);
            int puntos;

            bool resultado = _validador.VerificarAcierto(
                IdCancionPrueba, 
                MensajeCorrecto, 
                out puntos);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_AsignaPuntosRespuestaCorrecta()
        {
            _mockCatalogo
                .Setup(catalogo => catalogo.ValidarRespuesta(IdCancionPrueba, MensajeCorrecto))
                .Returns(true);
            _mockGestorTiempos
                .Setup(gestor => gestor.CalcularPuntosPorTiempo())
                .Returns(PuntosPorTiempoPrueba);
            int puntos;

            _validador.VerificarAcierto(IdCancionPrueba, MensajeCorrecto, out puntos);

            Assert.AreEqual(PuntosPorTiempoPrueba, puntos);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RetornaFalsoRespuestaIncorrecta()
        {
            _mockCatalogo
                .Setup(catalogo => catalogo.ValidarRespuesta(IdCancionPrueba, MensajeIncorrecto))
                .Returns(false);
            int puntos;

            bool resultado = _validador.VerificarAcierto(
                IdCancionPrueba, 
                MensajeIncorrecto, 
                out puntos);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RetornaVerdaderoMensajeProtocolo()
        {
            _mockCatalogo
                .Setup(catalogo => catalogo.ValidarRespuesta(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);
            int puntos;

            bool resultado = _validador.VerificarAcierto(
                IdCancionPrueba, 
                MensajeAciertoProtocolo, 
                out puntos);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_AsignaPuntosProtocolo()
        {
            _mockCatalogo
                .Setup(catalogo => catalogo.ValidarRespuesta(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);
            int puntos;

            _validador.VerificarAcierto(IdCancionPrueba, MensajeAciertoProtocolo, out puntos);

            Assert.AreEqual(PuntosProtocolo, puntos);
        }        [TestMethod]
        public void Prueba_RegistrarAcierto_LanzaExcepcionJugadorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _validador.RegistrarAcierto(null, PuntosObtenidos));
        }

        [TestMethod]
        public void Prueba_RegistrarAcierto_EstableceYaAdivinoVerdadero()
        {
            var jugador = CrearJugadorPrueba();

            _validador.RegistrarAcierto(jugador, PuntosObtenidos);

            Assert.IsTrue(jugador.YaAdivino);
        }

        [TestMethod]
        public void Prueba_RegistrarAcierto_SumaPuntosAlTotal()
        {
            var jugador = CrearJugadorPrueba();
            jugador.PuntajeTotal = PuntajeInicialCero;

            _validador.RegistrarAcierto(jugador, PuntosObtenidos);

            Assert.AreEqual(PuntosObtenidos, jugador.PuntajeTotal);
        }        private JugadorPartida CrearJugadorPrueba()
        {
            return new JugadorPartida
            {
                IdConexion = IdConexionPrueba,
                NombreUsuario = NombreJugadorPrueba,
                EsHost = false,
                EsDibujante = false,
                YaAdivino = false,
                PuntajeTotal = PuntajeInicialCero
            };
        }    }
}
