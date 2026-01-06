using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class ChatManejadorPruebas
    {
        private const string IdSalaPrueba = "123456";
        private const string NombreJugadorPrueba = "Jugador1";
        private const string MensajePrueba = "Mensaje de prueba";

        private Mock<IAlmacenClientesChat> _mockAlmacenClientes;
        private Mock<IProveedorContextoOperacion> _mockProveedorContexto;
        private Mock<ISesionUsuarioManejador> _mockSesionManejador;
        private Mock<IChatManejadorCallback> _mockCallback;
        private Mock<IContextChannel> _mockCanalActual;
        private ChatManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockAlmacenClientes = new Mock<IAlmacenClientesChat>();
            _mockProveedorContexto = new Mock<IProveedorContextoOperacion>();
            _mockSesionManejador = new Mock<ISesionUsuarioManejador>();
            _mockCallback = new Mock<IChatManejadorCallback>();
            _mockCanalActual = new Mock<IContextChannel>();
            _mockProveedorContexto
                .Setup(proveedor => proveedor.ExisteContexto)
                .Returns(true);
            _mockProveedorContexto
                .Setup(proveedor => proveedor.ObtenerCallbackChannel<IChatManejadorCallback>())
                .Returns(_mockCallback.Object);
            _mockProveedorContexto
                .Setup(proveedor => proveedor.ObtenerCanalActual())
                .Returns(_mockCanalActual.Object);
            _mockAlmacenClientes
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<ClienteChat>());
            _manejador = new ChatManejador(
                _mockAlmacenClientes.Object,
                _mockProveedorContexto.Object,
                _mockSesionManejador.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionAlmacenClientesNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ChatManejador(
                    null,
                    _mockProveedorContexto.Object,
                    _mockSesionManejador.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionProveedorContextoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ChatManejador(
                    _mockAlmacenClientes.Object,
                    null,
                    _mockSesionManejador.Object));
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_RegistraClienteEnAlmacen()
        {
            _manejador.UnirseChatSala(IdSalaPrueba, NombreJugadorPrueba);

            _mockAlmacenClientes.Verify(
                almacen => almacen.RegistrarOActualizarCliente(
                    IdSalaPrueba,
                    NombreJugadorPrueba,
                    It.IsAny<IChatManejadorCallback>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_SalirChatSala_RemueveClienteDelAlmacen()
        {
            _manejador.SalirChatSala(IdSalaPrueba, NombreJugadorPrueba);

            _mockAlmacenClientes.Verify(
                almacen => almacen.RemoverCliente(IdSalaPrueba, NombreJugadorPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_NotificaATodosLosClientes()
        {
            var clientes = new System.Collections.Generic.List<ClienteChat>
            {
                new ClienteChat("OtroJugador", _mockCallback.Object)
            };
            _mockAlmacenClientes
                .Setup(almacen => almacen.ObtenerClientesSala(It.IsAny<string>()))
                .Returns(clientes);

            _manejador.EnviarMensaje(IdSalaPrueba, MensajePrueba, NombreJugadorPrueba);

            _mockCallback.Verify(
                callback => callback.RecibirMensaje(
                    NombreJugadorPrueba,
                    MensajePrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_IgnoraMensajeVacio()
        {
            _manejador.EnviarMensaje(IdSalaPrueba, string.Empty, NombreJugadorPrueba);

            _mockAlmacenClientes.Verify(
                almacen => almacen.ObtenerClientesSala(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_IgnoraMensajeNulo()
        {
            _manejador.EnviarMensaje(IdSalaPrueba, null, NombreJugadorPrueba);

            _mockAlmacenClientes.Verify(
                almacen => almacen.ObtenerClientesSala(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_IgnoraMensajeSoloEspacios()
        {
            _manejador.EnviarMensaje(IdSalaPrueba, "   ", NombreJugadorPrueba);

            _mockAlmacenClientes.Verify(
                almacen => almacen.ObtenerClientesSala(It.IsAny<string>()),
                Times.Never);
        }
    }
}
