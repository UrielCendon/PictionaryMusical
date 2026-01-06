using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.Entidades;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class ControladorPartidaPruebas
    {
        private const int TiempoRondaPrueba = 60;
        private const int TotalRondasPrueba = 3;
        private const string DificultadPrueba = "Normal";
        private const string IdJugadorHost = "host-id";
        private const string IdJugadorDos = "jugador-dos-id";
        private const string NombreJugadorHost = "HostPlayer";
        private const int TiempoRondaInvalido = 0;
        private const int TotalRondasInvalido = 0;

        private Mock<ICatalogoCanciones> _mockCatalogo;
        private Mock<IGestorJugadoresPartida> _mockGestorJugadores;
        private Mock<IGestorTiemposPartida> _mockGestorTiempos;
        private Mock<IValidadorAdivinanza> _mockValidador;
        private Mock<IProveedorTiempo> _mockProveedorTiempo;
        private ConfiguracionPartida _configuracion;
        private DependenciasPartida _dependencias;

        [TestInitialize]
        public void Inicializar()
        {
            _mockCatalogo = new Mock<ICatalogoCanciones>();
            _mockGestorJugadores = new Mock<IGestorJugadoresPartida>();
            _mockGestorTiempos = new Mock<IGestorTiemposPartida>();
            _mockValidador = new Mock<IValidadorAdivinanza>();
            _mockProveedorTiempo = new Mock<IProveedorTiempo>();

            _configuracion = new ConfiguracionPartida
            {
                TiempoRonda = TiempoRondaPrueba,
                Dificultad = DificultadPrueba,
                TotalRondas = TotalRondasPrueba
            };

            _dependencias = new DependenciasPartida
            {
                Catalogo = _mockCatalogo.Object,
                GestorJugadores = _mockGestorJugadores.Object,
                GestorTiempos = _mockGestorTiempos.Object,
                ValidadorAdivinanza = _mockValidador.Object,
                ProveedorTiempo = _mockProveedorTiempo.Object
            };
        }
        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionConfiguracionNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(null, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionDependenciasNulas()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(_configuracion, null));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionTiempoRondaInvalido()
        {
            _configuracion.TiempoRonda = TiempoRondaInvalido;

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionTotalRondasInvalido()
        {
            _configuracion.TotalRondas = TotalRondasInvalido;

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionDificultadNula()
        {
            _configuracion.Dificultad = null;

            Assert.ThrowsException<ArgumentException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionDificultadVacia()
        {
            _configuracion.Dificultad = string.Empty;

            Assert.ThrowsException<ArgumentException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionCatalogoNulo()
        {
            _dependencias.Catalogo = null;

            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionGestorJugadoresNulo()
        {
            _dependencias.GestorJugadores = null;

            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionGestorTiemposNulo()
        {
            _dependencias.GestorTiempos = null;

            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionValidadorNulo()
        {
            _dependencias.ValidadorAdivinanza = null;

            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionProveedorTiempoNulo()
        {
            _dependencias.ProveedorTiempo = null;

            Assert.ThrowsException<ArgumentNullException>(
                () => new ControladorPartida(_configuracion, _dependencias));
        }

        [TestMethod]
        public void Prueba_Constructor_CreaInstanciaCorrectamente()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);

            Assert.IsInstanceOfType(controlador, typeof(ControladorPartida));
        }        
        
        [TestMethod]
        public void Prueba_EstaFinalizada_RetornaFalsoAlCrear()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);

            Assert.IsFalse(controlador.EstaFinalizada);
        }        
        
        [TestMethod]
        public void Prueba_ConfigurarIdiomaCanciones_NoLanzaExcepcion()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);

            controlador.ConfigurarIdiomaCanciones("Ingles");

            Assert.IsFalse(controlador.EstaFinalizada);
        }        
        
        [TestMethod]
        public void Prueba_AgregarJugador_LlamaGestorCorrectamente()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);

            controlador.AgregarJugador(IdJugadorHost, NombreJugadorHost, true);

            _mockGestorJugadores.Verify(
                gestor => gestor.Agregar(IdJugadorHost, NombreJugadorHost, true), 
                Times.Once);
        }        
        
        [TestMethod]
        public void Prueba_RemoverJugador_LlamaGestorCorrectamente()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            bool eraDibujante;
            string nombreUsuario;
            _mockGestorJugadores
                .Setup(gestor => gestor.Remover(IdJugadorHost, out eraDibujante, out nombreUsuario))
                .Returns(true);

            controlador.RemoverJugador(IdJugadorHost);

            _mockGestorJugadores.Verify(
                gestor => gestor.Remover(IdJugadorHost, out eraDibujante, out nombreUsuario), 
                Times.Once);
        }        
        
        [TestMethod]
        public void Prueba_IniciarPartida_LanzaExcepcionNoEsHost()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            _mockGestorJugadores
                .Setup(gestor => gestor.HaySuficientesJugadores)
                .Returns(true);
            _mockGestorJugadores
                .Setup(gestor => gestor.EsHost(IdJugadorDos))
                .Returns(false);

            Assert.ThrowsException<SecurityException>(
                () => controlador.IniciarPartida(IdJugadorDos));
        }

        [TestMethod]
        public void Prueba_IniciarPartida_LanzaExcepcionFaltanJugadores()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            _mockGestorJugadores
                .Setup(gestor => gestor.HaySuficientesJugadores)
                .Returns(false);

            Assert.ThrowsException<InvalidOperationException>(
                () => controlador.IniciarPartida(IdJugadorHost));
        }

        [TestMethod]
        public void Prueba_IniciarPartida_DisparaEventoPartidaIniciada()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            ConfigurarMocksParaInicioPartida();
            bool eventoDisparado = false;
            controlador.PartidaIniciada += () => eventoDisparado = true;

            controlador.IniciarPartida(IdJugadorHost);

            Assert.IsTrue(eventoDisparado);
        }        
        
        [TestMethod]
        public void Prueba_ObtenerJugadores_LlamaGestorCorrectamente()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            var listaJugadores = new List<JugadorPartida>();
            _mockGestorJugadores
                .Setup(gestor => gestor.ObtenerCopiaLista())
                .Returns(listaJugadores);

            var resultado = controlador.ObtenerJugadores();

            _mockGestorJugadores.Verify(gestor => gestor.ObtenerCopiaLista(), Times.Once);
        }        
        
        [TestMethod]
        public void Prueba_ObtenerNombreUsuarioPorId_RetornaNombreCorrecto()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            var jugador = new JugadorPartida { NombreUsuario = NombreJugadorHost };
            _mockGestorJugadores
                .Setup(gestor => gestor.Obtener(IdJugadorHost))
                .Returns(jugador);

            string resultado = controlador.ObtenerNombreUsuarioPorId(IdJugadorHost);

            Assert.AreEqual(NombreJugadorHost, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerNombreUsuarioPorId_RetornaNuloSiNoExiste()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            _mockGestorJugadores
                .Setup(gestor => gestor.Obtener(IdJugadorHost))
                .Returns((JugadorPartida)null);

            string resultado = controlador.ObtenerNombreUsuarioPorId(IdJugadorHost);

            Assert.IsNull(resultado);
        }        
        
        [TestMethod]
        public void Prueba_ProcesarMensaje_DisparaEventoMensajeEnSalaEspera()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            var jugador = new JugadorPartida { NombreUsuario = NombreJugadorHost };
            _mockGestorJugadores
                .Setup(gestor => gestor.Obtener(IdJugadorHost))
                .Returns(jugador);
            string mensajeRecibido = null;
            controlador.MensajeChatRecibido += (nombre, mensaje) => mensajeRecibido = mensaje;

            controlador.ProcesarMensaje(IdJugadorHost, "Hola");

            Assert.AreEqual("Hola", mensajeRecibido);
        }

        [TestMethod]
        public void Prueba_ProcesarMensaje_NoDisparaEventoMensajeNulo()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            bool eventoDisparado = false;
            controlador.MensajeChatRecibido += (nombre, mensaje) => eventoDisparado = true;

            controlador.ProcesarMensaje(IdJugadorHost, null);

            Assert.IsFalse(eventoDisparado);
        }

        [TestMethod]
        public void Prueba_ProcesarMensaje_NoDisparaEventoMensajeVacio()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            bool eventoDisparado = false;
            controlador.MensajeChatRecibido += (nombre, mensaje) => eventoDisparado = true;

            controlador.ProcesarMensaje(IdJugadorHost, string.Empty);

            Assert.IsFalse(eventoDisparado);
        }        
        
        [TestMethod]
        public void Prueba_ProcesarTrazo_NoDisparaEventoEnSalaEspera()
        {
            var controlador = new ControladorPartida(_configuracion, _dependencias);
            var jugador = new JugadorPartida { EsDibujante = true };
            _mockGestorJugadores
                .Setup(gestor => gestor.Obtener(IdJugadorHost))
                .Returns(jugador);
            bool eventoDisparado = false;
            controlador.TrazoRecibido += (trazo) => eventoDisparado = true;

            controlador.ProcesarTrazo(IdJugadorHost, new TrazoDTO());

            Assert.IsFalse(eventoDisparado);
        }        
        
        private void ConfigurarMocksParaInicioPartida()
        {
            _mockGestorJugadores
                .Setup(gestor => gestor.HaySuficientesJugadores)
                .Returns(true);
            _mockGestorJugadores
                .Setup(gestor => gestor.EsHost(IdJugadorHost))
                .Returns(true);
            _mockGestorJugadores
                .Setup(gestor => gestor.QuedanDibujantesPendientes())
                .Returns(true);
            _mockGestorJugadores
                .Setup(gestor => gestor.SeleccionarSiguienteDibujante())
                .Returns(true);
            _mockCatalogo
                .Setup(catalogo => catalogo.ObtenerCancionAleatoria(
                    It.IsAny<string>(), 
                    It.IsAny<HashSet<int>>()))
                .Returns(new Cancion 
                { 
                    Id = 1, 
                    Nombre = "Cancion", 
                    Artista = "Artista" 
                });
        }    
    }
}
