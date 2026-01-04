using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.Entidades;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;

namespace PictionaryMusicalServidor.Pruebas.Servicios
{
    [TestClass]
    public class ControladorPartidaPruebas
    {
        private const int TiempoRonda = 30;
        private const int TotalRondas = 3;
        private const string DificultadMedia = "media";
        private const string IdHost = "host123";
        private const string NombreHost = "HostPlayer";
        private const string IdJugador = "jugador123";
        private const string NombreJugador = "JugadorNormal";
        private const string MensajeChat = "Hola mundo";
        private const int CancionId = 1;

        private Mock<ICatalogoCanciones> _catalogoMock;
        private Mock<IGestorJugadoresPartida> _gestorJugadoresMock;
        private Mock<IGestorTiemposPartida> _gestorTiemposMock;
        private Mock<IValidadorAdivinanza> _validadorMock;
        private Mock<IProveedorTiempo> _proveedorTiempoMock;
        private ControladorPartida _controlador;

        [TestInitialize]
        public void Inicializar()
        {
            _catalogoMock = new Mock<ICatalogoCanciones>();
            _gestorJugadoresMock = new Mock<IGestorJugadoresPartida>();
            _gestorTiemposMock = new Mock<IGestorTiemposPartida>();
            _validadorMock = new Mock<IValidadorAdivinanza>();
            _proveedorTiempoMock = new Mock<IProveedorTiempo>();

            _proveedorTiempoMock.Setup(p => p.Retrasar(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            _controlador = new ControladorPartida(
                TiempoRonda,
                DificultadMedia,
                TotalRondas,
                _catalogoMock.Object,
                _gestorJugadoresMock.Object,
                _gestorTiemposMock.Object,
                _validadorMock.Object,
                _proveedorTiempoMock.Object
            );
        }

        [TestMethod]
        public void Prueba_AgregarJugador_PartidaYaIniciadaLanzaExcepcion()
        {

            _gestorJugadoresMock.Setup(g => g.HaySuficientesJugadores).Returns(true);
            _gestorJugadoresMock.Setup(g => g.EsHost(IdHost)).Returns(true);
            _catalogoMock.Setup(c => c.ObtenerCancionAleatoria(It.IsAny<string>(), It.IsAny<HashSet<int>>()))
                .Returns(new Cancion { Id = CancionId });

            _controlador.IniciarPartida(IdHost);

            Assert.ThrowsException<InvalidOperationException>(() =>
                _controlador.AgregarJugador(IdJugador, NombreJugador, false));
        }

        [TestMethod]
        public void Prueba_IniciarPartida_NoHostLanzaExcepcion()
        {
            _gestorJugadoresMock.Setup(g => g.HaySuficientesJugadores).Returns(true);
            _gestorJugadoresMock.Setup(g => g.EsHost(IdJugador)).Returns(false);

            Assert.ThrowsException<SecurityException>(() =>
                _controlador.IniciarPartida(IdJugador));
        }

        [TestMethod]
        public void Prueba_IniciarPartida_FlujoCorrectoDisparaEvento()
        {
            bool eventoDisparado = false;
            _controlador.PartidaIniciada += () => eventoDisparado = true;

            _gestorJugadoresMock.Setup(g => g.HaySuficientesJugadores).Returns(true);
            _gestorJugadoresMock.Setup(g => g.EsHost(IdHost)).Returns(true);
            _catalogoMock.Setup(c => c.ObtenerCancionAleatoria(It.IsAny<string>(), It.IsAny<HashSet<int>>()))
                .Returns(new Cancion { Id = CancionId });

            _controlador.IniciarPartida(IdHost);

            Assert.IsTrue(eventoDisparado);
            _gestorJugadoresMock.Verify(g => g.SeleccionarSiguienteDibujante(), Times.Once);
            _proveedorTiempoMock.Verify(p => p.Retrasar(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void Prueba_ProcesarMensaje_AdivinanzaCorrectaDisparaEvento()
        {
            _gestorJugadoresMock.Setup(g => g.HaySuficientesJugadores).Returns(true);
            _gestorJugadoresMock.Setup(g => g.EsHost(IdHost)).Returns(true);
            _catalogoMock.Setup(c => c.ObtenerCancionAleatoria(It.IsAny<string>(), It.IsAny<HashSet<int>>()))
                .Returns(new Cancion { Id = CancionId });
            _controlador.IniciarPartida(IdHost);

            var jugador = new JugadorPartida { NombreUsuario = NombreJugador };
            _gestorJugadoresMock.Setup(g => g.Obtener(IdJugador)).Returns(jugador);

            _validadorMock.Setup(v => v.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando)).Returns(true);
            int puntos = 10;
            _validadorMock.Setup(v => v.VerificarAcierto(It.IsAny<int>(), MensajeChat, out puntos)).Returns(true);

            bool acerto = false;
            _controlador.JugadorAdivino += (u, p) => acerto = true;

            _controlador.ProcesarMensaje(IdJugador, MensajeChat);

            Assert.IsTrue(acerto);
            _validadorMock.Verify(v => v.RegistrarAcierto(jugador, puntos), Times.Once);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_HostAbandonaCancelaPartida()
        {
            _gestorJugadoresMock.Setup(g => g.HaySuficientesJugadores).Returns(true);
            _gestorJugadoresMock.Setup(g => g.EsHost(IdHost)).Returns(true);
            _catalogoMock.Setup(c => c.ObtenerCancionAleatoria(It.IsAny<string>(), It.IsAny<HashSet<int>>()))
                .Returns(new Cancion { Id = CancionId });
            _controlador.IniciarPartida(IdHost);

            bool eraDibujante;
            string nombre = NombreHost;
            _gestorJugadoresMock.Setup(g => g.Remover(IdHost, out eraDibujante, out nombre)).Returns(true);

            _gestorJugadoresMock.Setup(g => g.HaySuficientesJugadores).Returns(false);
            _gestorJugadoresMock.Setup(g => g.EsHost(IdHost)).Returns(true);

            bool partidaFinalizada = false;
            _controlador.FinPartida += (res) => partidaFinalizada = true;

            _controlador.RemoverJugador(IdHost);

            Assert.IsTrue(partidaFinalizada);
            Assert.IsTrue(_controlador.EstaFinalizada);
        }
    }
}