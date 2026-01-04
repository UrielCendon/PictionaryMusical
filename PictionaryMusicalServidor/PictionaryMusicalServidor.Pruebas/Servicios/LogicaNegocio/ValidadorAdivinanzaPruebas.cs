using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;

namespace PictionaryMusicalServidor.Pruebas.Servicios
{
    /// <summary>
    /// Contiene pruebas unitarias para la clase <see cref="ValidadorAdivinanza"/>.
    /// Valida la logica de verificacion de aciertos y permisos de adivinanza.
    /// </summary>
    [TestClass]
    public class ValidadorAdivinanzaPruebas
    {
        private const int IdCancion = 1;
        private const string MensajeCorrecto = "Respuesta Correcta";
        private const string MensajeIncorrecto = "Respuesta Incorrecta";
        private const string MensajeProtocolo = "ACIERTO:1:50";
        private const int PuntosTiempo = 100;
        private const int PuntosProtocolo = 50;

        private Mock<ICatalogoCanciones> _catalogoMock;
        private Mock<IGestorTiemposPartida> _gestorTiemposMock;
        private ValidadorAdivinanza _validador;

        [TestInitialize]
        public void Inicializar()
        {
            _catalogoMock = new Mock<ICatalogoCanciones>();
            _gestorTiemposMock = new Mock<IGestorTiemposPartida>();

            _validador = new ValidadorAdivinanza(_catalogoMock.Object, _gestorTiemposMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_DependenciasNulasLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ValidadorAdivinanza(null, _gestorTiemposMock.Object));
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_CondicionesIdealesRetornaTrue()
        {
            var jugador = new JugadorPartida { EsDibujante = false, YaAdivino = false };

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_EsDibujanteRetornaFalse()
        {
            var jugador = new JugadorPartida { EsDibujante = true, YaAdivino = false };

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_YaAdivinoRetornaFalse()
        {
            var jugador = new JugadorPartida { EsDibujante = false, YaAdivino = true };

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RespuestaCorrectaCalculaPuntosDelTiempo()
        {
            _catalogoMock.Setup(c => c.ValidarRespuesta(IdCancion, MensajeCorrecto)).Returns(true);
            _gestorTiemposMock.Setup(g => g.CalcularPuntosPorTiempo()).Returns(PuntosTiempo);

            int puntos;
            bool resultado = _validador.VerificarAcierto(IdCancion, MensajeCorrecto, out puntos);

            Assert.IsTrue(resultado);
            Assert.AreEqual(PuntosTiempo, puntos);
            _gestorTiemposMock.Verify(g => g.CalcularPuntosPorTiempo(), Times.Once);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RespuestaIncorrectaRetornaFalse()
        {
            _catalogoMock.Setup(c => c.ValidarRespuesta(IdCancion, MensajeIncorrecto)).Returns(false);

            int puntos;
            bool resultado = _validador.VerificarAcierto(IdCancion, MensajeIncorrecto, out puntos);

            Assert.IsFalse(resultado);
            Assert.AreEqual(0, puntos);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_MensajeProtocoloValidoRetornaPuntosProtocolo()
        {
            _catalogoMock.Setup(c => c.ValidarRespuesta(IdCancion, MensajeProtocolo)).Returns(false);

            int puntos;
            bool resultado = _validador.VerificarAcierto(IdCancion, MensajeProtocolo, out puntos);

            Assert.IsTrue(resultado);
            Assert.AreEqual(PuntosProtocolo, puntos);
        }

        [TestMethod]
        public void Prueba_RegistrarAcierto_ActualizaEstadoYPuntosJugador()
        {
            var jugador = new JugadorPartida { YaAdivino = false, PuntajeTotal = 0 };

            _validador.RegistrarAcierto(jugador, PuntosTiempo);

            Assert.IsTrue(jugador.YaAdivino);
            Assert.AreEqual(PuntosTiempo, jugador.PuntajeTotal);
        }
    }
}