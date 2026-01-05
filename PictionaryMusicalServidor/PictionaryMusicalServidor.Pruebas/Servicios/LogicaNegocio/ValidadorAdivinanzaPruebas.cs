using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class ValidadorAdivinanzaPruebas
    {
        private const int IdCancion = 1;
        private const string MensajeCorrecto = "Respuesta Correcta";
        private const string MensajeIncorrecto = "Respuesta Incorrecta";
        private const string MensajeProtocolo = "ACIERTO:1:50";
        private const string MensajeVacio = "";
        private const int PuntosTiempo = 100;
        private const int PuntosProtocolo = 50;
        private const int PuntosCero = 0;

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
        public void Prueba_Constructor_CatalogoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ValidadorAdivinanza(null, _gestorTiemposMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_GestorTiemposNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ValidadorAdivinanza(_catalogoMock.Object, null));
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_JugadorNulo()
        {
            bool resultado = _validador.JugadorPuedeAdivinar(null, EstadoPartida.Jugando);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_EstadoNoJugando()
        {
            var jugador = new JugadorPartida { EsDibujante = false, YaAdivino = false };

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.EnSalaEspera);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_CondicionesIdeales()
        {
            var jugador = new JugadorPartida { EsDibujante = false, YaAdivino = false };

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_EsDibujante()
        {
            var jugador = new JugadorPartida { EsDibujante = true, YaAdivino = false };

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinar_YaAdivino()
        {
            var jugador = new JugadorPartida { EsDibujante = false, YaAdivino = true };

            bool resultado = _validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_MensajeVacio()
        {
            bool resultado = _validador.VerificarAcierto(IdCancion, MensajeVacio, out int puntos);

            Assert.IsFalse(resultado);
            Assert.AreEqual(PuntosCero, puntos);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RespuestaCorrecta()
        {
            _catalogoMock
                .Setup(catalogo => catalogo.ValidarRespuesta(IdCancion, MensajeCorrecto))
                .Returns(true);
            _gestorTiemposMock
                .Setup(gestor => gestor.CalcularPuntosPorTiempo())
                .Returns(PuntosTiempo);

            bool resultado = _validador.VerificarAcierto(IdCancion, MensajeCorrecto, out int puntos);

            Assert.IsTrue(resultado);
            Assert.AreEqual(PuntosTiempo, puntos);
            _gestorTiemposMock.Verify(
                gestor => gestor.CalcularPuntosPorTiempo(), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_RespuestaIncorrecta()
        {
            _catalogoMock
                .Setup(catalogo => catalogo.ValidarRespuesta(IdCancion, MensajeIncorrecto))
                .Returns(false);

            bool resultado = _validador.VerificarAcierto(IdCancion, MensajeIncorrecto, out int puntos);

            Assert.IsFalse(resultado);
            Assert.AreEqual(PuntosCero, puntos);
        }

        [TestMethod]
        public void Prueba_VerificarAcierto_MensajeProtocoloValido()
        {
            _catalogoMock
                .Setup(catalogo => catalogo.ValidarRespuesta(IdCancion, MensajeProtocolo))
                .Returns(false);

            bool resultado = _validador.VerificarAcierto(IdCancion, MensajeProtocolo, out int puntos);

            Assert.IsTrue(resultado);
            Assert.AreEqual(PuntosProtocolo, puntos);
        }

        [TestMethod]
        public void Prueba_RegistrarAcierto_JugadorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _validador.RegistrarAcierto(null, PuntosTiempo));
        }

        [TestMethod]
        public void Prueba_RegistrarAcierto_JugadorValido()
        {
            var jugador = new JugadorPartida { YaAdivino = false, PuntajeTotal = 0 };

            _validador.RegistrarAcierto(jugador, PuntosTiempo);

            Assert.IsTrue(jugador.YaAdivino);
            Assert.AreEqual(PuntosTiempo, jugador.PuntajeTotal);
        }
    }
}
