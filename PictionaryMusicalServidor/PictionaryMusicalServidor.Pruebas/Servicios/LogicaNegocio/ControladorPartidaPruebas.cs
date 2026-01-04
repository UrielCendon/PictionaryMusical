using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.Entidades;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using System;
using System.Collections.Generic;
using System.Security;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class ControladorPartidaPruebas
    {
        private const int TiempoRondaSegundos = 60;
        private const int TotalRondas = 3;
        private const string DificultadFacil = "facil";
        private const int IdCancionPrueba = 1;
        private const int CantidadJugadoresEsperada = 1;
        private const int CantidadDosJugadores = 2;

        private Mock<ICatalogoCanciones> _catalogoMock;
        private GestorJugadoresPartida _gestorJugadores;
        private ControladorPartida _controlador;

        [TestInitialize]
        public void Inicializar()
        {
            _catalogoMock = new Mock<ICatalogoCanciones>();
            _gestorJugadores = new GestorJugadoresPartida();
            
            ConfigurarCatalogoMockPorDefecto();
            
            _controlador = new ControladorPartida(
                TiempoRondaSegundos, 
                DificultadFacil, 
                TotalRondas, 
                _catalogoMock.Object, 
                _gestorJugadores);
        }

        private void ConfigurarCatalogoMockPorDefecto()
        {
            _catalogoMock
                .Setup(c => c.ObtenerCancionAleatoria(
                    It.IsAny<string>(), 
                    It.IsAny<HashSet<int>>()))
                .Returns(new Cancion 
                { 
                    Id = IdCancionPrueba, 
                    Nombre = "Cancion Test", 
                    Artista = "Artista Test", 
                    Genero = "Pop" 
                });
        }

        [TestMethod]
        public void Prueba_ConstructorTiempoRondaCero_LanzaExcepcion()
        {

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new ControladorPartida(
                    0, 
                    DificultadFacil, 
                    TotalRondas, 
                    _catalogoMock.Object, 
                    _gestorJugadores));
        }

        [TestMethod]
        public void Prueba_ConstructorTotalRondasCero_LanzaExcepcion()
        {

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new ControladorPartida(
                    TiempoRondaSegundos, 
                    DificultadFacil, 
                    0, 
                    _catalogoMock.Object, 
                    _gestorJugadores));
        }

        [TestMethod]
        public void Prueba_ConstructorDificultadNula_LanzaExcepcion()
        {

            Assert.ThrowsException<ArgumentException>(
                () => new ControladorPartida(
                    TiempoRondaSegundos, 
                    null, 
                    TotalRondas, 
                    _catalogoMock.Object, 
                    _gestorJugadores));
        }

        [TestMethod]
        public void Prueba_ConstructorCatalogoNulo_LanzaExcepcion()
        {

            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(
                    TiempoRondaSegundos, 
                    DificultadFacil, 
                    TotalRondas, 
                    null, 
                    _gestorJugadores));
        }

        [TestMethod]
        public void Prueba_ConstructorGestorJugadoresNulo_LanzaExcepcion()
        {

            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(
                    TiempoRondaSegundos, 
                    DificultadFacil, 
                    TotalRondas, 
                    _catalogoMock.Object, 
                    null));
        }

        [TestMethod]
        public void Prueba_AgregarJugadorPartidaEnEspera_AgregaCorrectamente()
        {

            _controlador.AgregarJugador("id-1", "Jugador1", true);


            var jugadores = _controlador.ObtenerJugadores();
            Assert.AreEqual(
                CantidadJugadoresEsperada, 
                ((ICollection<JugadorPartida>)jugadores).Count);
        }

        [TestMethod]
        public void Prueba_AgregarJugadorPartidaYaIniciada_LanzaExcepcion()
        {

            _controlador.AgregarJugador("id-host", "Host", true);
            _controlador.AgregarJugador("id-2", "Jugador2", false);
            _controlador.IniciarPartida("id-host");


            Assert.ThrowsException<InvalidOperationException>(
                () => _controlador.AgregarJugador("id-3", "Jugador3", false));
        }

        [TestMethod]
        public void Prueba_IniciarPartidaSinJugadores_LanzaExcepcion()
        {

            _controlador.AgregarJugador("id-host", "Host", true);


            Assert.ThrowsException<InvalidOperationException>(
                () => _controlador.IniciarPartida("id-host"));
        }

        [TestMethod]
        public void Prueba_IniciarPartidaNoEsHost_LanzaExcepcion()
        {

            _controlador.AgregarJugador("id-host", "Host", true);
            _controlador.AgregarJugador("id-jugador", "Jugador", false);


            Assert.ThrowsException<SecurityException>(
                () => _controlador.IniciarPartida("id-jugador"));
        }

        [TestMethod]
        public void Prueba_IniciarPartidaCondicionesValidas_DisparaEvento()
        {

            _controlador.AgregarJugador("id-host", "Host", true);
            _controlador.AgregarJugador("id-2", "Jugador2", false);
            bool eventoDisparado = false;
            _controlador.PartidaIniciada += () => eventoDisparado = true;


            _controlador.IniciarPartida("id-host");


            Assert.IsTrue(eventoDisparado);
        }

        [TestMethod]
        public void Prueba_IniciarPartidaYaIniciada_LanzaExcepcion()
        {

            _controlador.AgregarJugador("id-host", "Host", true);
            _controlador.AgregarJugador("id-2", "Jugador2", false);
            _controlador.IniciarPartida("id-host");


            Assert.ThrowsException<InvalidOperationException>(
                () => _controlador.IniciarPartida("id-host"));
        }

        [TestMethod]
        public void Prueba_EstaFinalizadaPartidaNueva_RetornaFalse()
        {

            var resultado = _controlador.EstaFinalizada;


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ConfigurarIdiomaCanciones_AplicaConfiguracion()
        {

            _controlador.ConfigurarIdiomaCanciones("Ingles");
            _controlador.AgregarJugador("id-host", "Host", true);
            _controlador.AgregarJugador("id-2", "Jugador2", false);
            _controlador.IniciarPartida("id-host");


            _catalogoMock.Verify(
                c => c.ObtenerCancionAleatoria("Ingles", It.IsAny<HashSet<int>>()), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_RemoverJugadorInexistente_NoLanzaExcepcion()
        {
            var jugadoresAntes = _controlador.ObtenerJugadores();
            int cantidadAntes = ((ICollection<JugadorPartida>)jugadoresAntes).Count;

            _controlador.RemoverJugador("id-inexistente");

            var jugadoresDespues = _controlador.ObtenerJugadores();
            Assert.AreEqual(
                cantidadAntes, 
                ((ICollection<JugadorPartida>)jugadoresDespues).Count);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_DisparaEventoDesconectado()
        {

            _controlador.AgregarJugador("id-1", "JugadorTest", false);
            string nombreDesconectado = null;
            _controlador.JugadorDesconectado += nombre => nombreDesconectado = nombre;


            _controlador.RemoverJugador("id-1");


            Assert.AreEqual("JugadorTest", nombreDesconectado);
        }

        [TestMethod]
        public void Prueba_ProcesarMensajeEnSalaEspera_RetransmiteMensaje()
        {

            _controlador.AgregarJugador("id-1", "Jugador1", true);
            string mensajeRecibido = null;
            _controlador.MensajeChatRecibido += (usuario, mensaje) => mensajeRecibido = mensaje;


            _controlador.ProcesarMensaje("id-1", "Hola a todos");


            Assert.AreEqual("Hola a todos", mensajeRecibido);
        }

        [TestMethod]
        public void Prueba_ProcesarMensajeVacio_NoRetransmite()
        {

            _controlador.AgregarJugador("id-1", "Jugador1", true);
            bool mensajeRecibido = false;
            _controlador.MensajeChatRecibido += (usuario, mensaje) => mensajeRecibido = true;


            _controlador.ProcesarMensaje("id-1", "   ");


            Assert.IsFalse(mensajeRecibido);
        }

        [TestMethod]
        public void Prueba_ProcesarMensajeJugadorInexistente_NoRetransmite()
        {

            bool mensajeRecibido = false;
            _controlador.MensajeChatRecibido += (usuario, mensaje) => mensajeRecibido = true;


            _controlador.ProcesarMensaje("id-inexistente", "Mensaje");


            Assert.IsFalse(mensajeRecibido);
        }

        [TestMethod]
        public void Prueba_ObtenerJugadores_RetornaLista()
        {

            _controlador.AgregarJugador("id-1", "Jugador1", true);
            _controlador.AgregarJugador("id-2", "Jugador2", false);


            var jugadores = _controlador.ObtenerJugadores();


            Assert.AreEqual(
                CantidadDosJugadores, 
                ((ICollection<JugadorPartida>)jugadores).Count);
        }
    }
}
