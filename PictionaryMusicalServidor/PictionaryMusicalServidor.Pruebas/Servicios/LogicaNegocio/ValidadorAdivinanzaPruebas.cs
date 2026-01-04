using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using System;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class ValidadorAdivinanzaPruebas
    {
        private const int DuracionRondaSegundos = 60;
        private const int DuracionTransicionSegundos = 5;
        private const int IdCancionPrueba = 1;
        private const int PuntajeBase = 50;
        private const int PuntajeInicial = 100;
        private const int PuntajeEsperadoConBonus = 150;
        private const int PuntajeMensajeProtocolo = 30;

        private Mock<ICatalogoCanciones> _catalogoMock;
        private GestorTiemposPartida _gestorTiempos;
        private ValidadorAdivinanza _validador;

        [TestInitialize]
        public void Inicializar()
        {
            _catalogoMock = new Mock<ICatalogoCanciones>();
            _gestorTiempos = new GestorTiemposPartida(
                DuracionRondaSegundos, 
                DuracionTransicionSegundos);
            _validador = new ValidadorAdivinanza(_catalogoMock.Object, _gestorTiempos);
        }

        [TestCleanup]
        public void Limpiar()
        {
            _gestorTiempos?.Dispose();
        }

        [TestMethod]
        public void Prueba_ConstructorCatalogoNulo_LanzaExcepcion()
        {

            Assert.ThrowsException<ArgumentNullException>(
                () => new ValidadorAdivinanza(null, _gestorTiempos));
        }

        [TestMethod]
        public void Prueba_ConstructorGestorTiemposNulo_LanzaExcepcion()
        {

            Assert.ThrowsException<ArgumentNullException>(
                () => new ValidadorAdivinanza(_catalogoMock.Object, null));
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinarValido_RetornaTrue()
        {

            var jugador = new JugadorPartida
            {
                EsDibujante = false,
                YaAdivino = false
            };


            var resultado = ValidadorAdivinanza.JugadorPuedeAdivinar(
                jugador, 
                EstadoPartida.Jugando);


            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinarEsDibujante_RetornaFalse()
        {

            var jugador = new JugadorPartida
            {
                EsDibujante = true,
                YaAdivino = false
            };


            var resultado = ValidadorAdivinanza.JugadorPuedeAdivinar(
                jugador, 
                EstadoPartida.Jugando);


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinarYaAdivino_RetornaFalse()
        {

            var jugador = new JugadorPartida
            {
                EsDibujante = false,
                YaAdivino = true
            };


            var resultado = ValidadorAdivinanza.JugadorPuedeAdivinar(
                jugador, 
                EstadoPartida.Jugando);


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_JugadorPuedeAdivinarPartidaNoEnJuego_RetornaFalse()
        {

            var jugador = new JugadorPartida
            {
                EsDibujante = false,
                YaAdivino = false
            };


            var resultado = ValidadorAdivinanza.JugadorPuedeAdivinar(
                jugador, 
                EstadoPartida.EnSalaEspera);


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAciertoRespuestaCorrecta_RetornaTrue()
        {

            _catalogoMock.Setup(c => c.ValidarRespuesta(
                IdCancionPrueba, 
                "Cancion Correcta"))
                .Returns(true);
            _gestorTiempos.IniciarRonda();
            int puntos;


            var resultado = _validador.VerificarAcierto(
                IdCancionPrueba, 
                "Cancion Correcta", 
                out puntos);


            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAciertoRespuestaIncorrecta_RetornaFalse()
        {

            _catalogoMock.Setup(c => c.ValidarRespuesta(
                IdCancionPrueba, 
                "Respuesta Erronea"))
                .Returns(false);
            int puntos;


            var resultado = _validador.VerificarAcierto(
                IdCancionPrueba, 
                "Respuesta Erronea", 
                out puntos);


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_VerificarAciertoMensajeProtocolo_RetornaTrue()
        {

            _catalogoMock.Setup(c => c.ValidarRespuesta(
                It.IsAny<int>(), 
                It.IsAny<string>()))
                .Returns(false);
            int puntos;


            var resultado = _validador.VerificarAcierto(
                IdCancionPrueba, 
                $"ACIERTO:cancion:{PuntajeBase}", 
                out puntos);


            Assert.IsTrue(resultado);
            Assert.AreEqual(PuntajeBase, puntos);
        }

        [TestMethod]
        public void Prueba_RegistrarAcierto_ActualizaEstadoPuntaje()
        {

            var jugador = new JugadorPartida
            {
                YaAdivino = false,
                PuntajeTotal = PuntajeInicial
            };


            ValidadorAdivinanza.RegistrarAcierto(jugador, PuntajeBase);


            Assert.IsTrue(jugador.YaAdivino);
            Assert.AreEqual(PuntajeEsperadoConBonus, jugador.PuntajeTotal);
        }

        [TestMethod]
        public void Prueba_EsMensajeAciertoProtocoloValido_RetornaTrue()
        {

            int puntos;


            var resultado = ValidadorAdivinanza.EsMensajeAciertoProtocolo(
                $"ACIERTO:nombreCancion:{PuntajeMensajeProtocolo}", 
                out puntos);


            Assert.IsTrue(resultado);
            Assert.AreEqual(PuntajeMensajeProtocolo, puntos);
        }

        [TestMethod]
        public void Prueba_EsMensajeAciertoProtocoloInvalido_RetornaFalse()
        {

            int puntos;


            var resultado = ValidadorAdivinanza.EsMensajeAciertoProtocolo(
                "mensaje normal", 
                out puntos);


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeAciertoProtocoloPuntosCero_RetornaFalse()
        {

            int puntos;


            var resultado = ValidadorAdivinanza.EsMensajeAciertoProtocolo(
                "ACIERTO:cancion:0", 
                out puntos);


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeAciertoProtocoloPartesInsuficientes_RetornaFalse()
        {

            int puntos;


            var resultado = ValidadorAdivinanza.EsMensajeAciertoProtocolo(
                "ACIERTO:cancion", 
                out puntos);


            Assert.IsFalse(resultado);
        }
    }
}
