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

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class ControladorPartidaPruebas
    {
        private const int TiempoRonda = 30;
        private const int TotalRondas = 3;
        private const int PuntosAdivinanza = 10;
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

            _proveedorTiempoMock
                .Setup(proveedor => proveedor.Retrasar(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var configuracion = new ConfiguracionPartida
            {
                TiempoRonda = TiempoRonda,
                Dificultad = DificultadMedia,
                TotalRondas = TotalRondas
            };

            var dependencias = new DependenciasPartida
            {
                Catalogo = _catalogoMock.Object,
                GestorJugadores = _gestorJugadoresMock.Object,
                GestorTiempos = _gestorTiemposMock.Object,
                ValidadorAdivinanza = _validadorMock.Object,
                ProveedorTiempo = _proveedorTiempoMock.Object
            };

            _controlador = new ControladorPartida(configuracion, dependencias);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_PartidaYaIniciada()
        {
            _gestorJugadoresMock.Setup(gestor => gestor.HaySuficientesJugadores).Returns(true);
            _gestorJugadoresMock.Setup(gestor => gestor.EsHost(IdHost)).Returns(true);
            _catalogoMock
                .Setup(catalogo => catalogo.ObtenerCancionAleatoria(
                    It.IsAny<string>(), 
                    It.IsAny<HashSet<int>>()))
                .Returns(new Cancion { Id = CancionId });

            _controlador.IniciarPartida(IdHost);

            Assert.ThrowsException<InvalidOperationException>(() =>
                _controlador.AgregarJugador(IdJugador, NombreJugador, false));
        }

        [TestMethod]
        public void Prueba_IniciarPartida_NoHost()
        {
            _gestorJugadoresMock
                .Setup(gestor => gestor.HaySuficientesJugadores)
                .Returns(true);
            _gestorJugadoresMock
                .Setup(gestor => gestor.EsHost(IdJugador))
                .Returns(false);

            Assert.ThrowsException<SecurityException>(() =>
                _controlador.IniciarPartida(IdJugador));
        }

        // fix el flujo debería validar solo que el evento se disparo
        [TestMethod]
        public void Prueba_IniciarPartida_FlujoCorrecto()
        {
            bool eventoDisparado = false;
            _controlador.PartidaIniciada += () => eventoDisparado = true;

            _gestorJugadoresMock
                .Setup(gestor => gestor.HaySuficientesJugadores)
                .Returns(true);
            _gestorJugadoresMock
                .Setup(gestor => gestor.EsHost(IdHost))
                .Returns(true);
            _catalogoMock
                .Setup(catalogo => catalogo.ObtenerCancionAleatoria(
                    It.IsAny<string>(), 
                    It.IsAny<HashSet<int>>()))
                .Returns(new Cancion { Id = CancionId });

            _controlador.IniciarPartida(IdHost);

            Assert.IsTrue(eventoDisparado);
            _gestorJugadoresMock.Verify(
                gestor => gestor.SeleccionarSiguienteDibujante(), 
                Times.Once);
            _proveedorTiempoMock.Verify(
                proveedor => proveedor.Retrasar(It.IsAny<int>()), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ProcesarMensaje_AdivinanzaCorrecta()
        {
            _gestorJugadoresMock
                .Setup(gestor => gestor.HaySuficientesJugadores)
                .Returns(true);
            _gestorJugadoresMock
                .Setup(gestor => gestor.EsHost(IdHost))
                .Returns(true);
            _catalogoMock
                .Setup(catalogo => catalogo.ObtenerCancionAleatoria(
                    It.IsAny<string>(), 
                    It.IsAny<HashSet<int>>()))
                .Returns(new Cancion { Id = CancionId });
            _controlador.IniciarPartida(IdHost);

            var jugador = new JugadorPartida { NombreUsuario = NombreJugador };
            _gestorJugadoresMock
                .Setup(gestor => gestor.Obtener(IdJugador))
                .Returns(jugador);

            _validadorMock
                .Setup(validador => validador.JugadorPuedeAdivinar(jugador, EstadoPartida.Jugando))
                .Returns(true);
            int puntos = PuntosAdivinanza;
            _validadorMock
                .Setup(validador => validador.VerificarAcierto(
                    It.IsAny<int>(), 
                    MensajeChat, 
                    out puntos))
                .Returns(true);

            bool acerto = false;
            _controlador.JugadorAdivino += (nombreUsuario, puntosObtenidos) => acerto = true;

            _controlador.ProcesarMensaje(IdJugador, MensajeChat);

            Assert.IsTrue(acerto);
            _validadorMock.Verify(
                validador => validador.RegistrarAcierto(jugador, puntos), 
                Times.Once);
        }

        // fix el flujo debería validar solo que la partida finalizo
        [TestMethod]
        public void Prueba_RemoverJugador_HostAbandona()
        {
            _gestorJugadoresMock
                .Setup(gestor => gestor.HaySuficientesJugadores)
                .Returns(true);
            _gestorJugadoresMock
                .Setup(gestor => gestor.EsHost(IdHost))
                .Returns(true);
            _catalogoMock
                .Setup(catalogo => catalogo.ObtenerCancionAleatoria(
                    It.IsAny<string>(), 
                    It.IsAny<HashSet<int>>()))
                .Returns(new Cancion { Id = CancionId });
            _controlador.IniciarPartida(IdHost);

            bool eraDibujante;
            string nombre = NombreHost;
            _gestorJugadoresMock
                .Setup(gestor => gestor.Remover(IdHost, out eraDibujante, out nombre))
                .Returns(true);

            _gestorJugadoresMock
                .Setup(gestor => gestor.HaySuficientesJugadores)
                .Returns(false);
            _gestorJugadoresMock
                .Setup(gestor => gestor.EsHost(IdHost))
                .Returns(true);

            bool partidaFinalizada = false;
            _controlador.FinPartida += (resultado) => partidaFinalizada = true;

            _controlador.RemoverJugador(IdHost);

            Assert.IsTrue(partidaFinalizada);
            Assert.IsTrue(_controlador.EstaFinalizada);
        }
    }
}
