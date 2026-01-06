using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class CursoPartidaManejadorPruebas
    {
        private const string IdSalaPrueba = "123456";
        private const string IdJugadorPrueba = "jugador-123";
        private const string NombreUsuarioPrueba = "Jugador1";

        private Mock<ISalasManejador> _mockSalasManejador;
        private Mock<ICatalogoCanciones> _mockCatalogoCanciones;
        private Mock<INotificadorPartida> _mockNotificadorPartida;
        private Mock<IActualizadorClasificacionPartida> _mockActualizadorClasificacion;
        private CursoPartidaManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockSalasManejador = new Mock<ISalasManejador>();
            _mockCatalogoCanciones = new Mock<ICatalogoCanciones>();
            _mockNotificadorPartida = new Mock<INotificadorPartida>();
            _mockActualizadorClasificacion = new Mock<IActualizadorClasificacionPartida>();
            _manejador = new CursoPartidaManejador(
                _mockSalasManejador.Object,
                _mockCatalogoCanciones.Object,
                _mockNotificadorPartida.Object,
                _mockActualizadorClasificacion.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionSalasManejadorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CursoPartidaManejador(
                    null,
                    _mockCatalogoCanciones.Object,
                    _mockNotificadorPartida.Object,
                    _mockActualizadorClasificacion.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionCatalogoCancionesNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CursoPartidaManejador(
                    _mockSalasManejador.Object,
                    null,
                    _mockNotificadorPartida.Object,
                    _mockActualizadorClasificacion.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionNotificadorPartidaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CursoPartidaManejador(
                    _mockSalasManejador.Object,
                    _mockCatalogoCanciones.Object,
                    null,
                    _mockActualizadorClasificacion.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionActualizadorClasificacionNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CursoPartidaManejador(
                    _mockSalasManejador.Object,
                    _mockCatalogoCanciones.Object,
                    _mockNotificadorPartida.Object,
                    null));
        }

        [TestMethod]
        public void Prueba_IniciarPartida_LanzaExcepcionSiNoHayJugadoresSuficientes()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => _manejador.IniciarPartida(IdSalaPrueba, IdJugadorPrueba));
        }
    }
}
