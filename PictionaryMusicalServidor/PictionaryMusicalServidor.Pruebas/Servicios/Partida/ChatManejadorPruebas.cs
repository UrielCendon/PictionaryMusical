using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class ChatManejadorPruebas
    {
        private const string IdSalaPrueba = "123456";
        private const string IdSalaConEspacios = "  123456  ";
        private const string NombreJugadorUno = "JugadorUno";
        private const string NombreJugadorDos = "JugadorDos";
        private const string NombreJugadorConEspacios = "  JugadorUno  ";
        private const string MensajePrueba = "Hola mundo";
        private const string MensajeConEspacios = "  Hola mundo  ";
        private const string MensajeVacio = "";
        private const string MensajeNulo = null;
        private const string IdSalaVacia = "";
        private const string IdSalaNula = null;
        private const string NombreVacio = "";
        private const string NombreNulo = null;

        private Mock<IAlmacenClientesChat> _almacenMock;
        private Mock<IProveedorContextoOperacion> _proveedorContextoMock;
        private Mock<IChatManejadorCallback> _callbackMock;
        private ChatManejador _chatManejador;

        [TestInitialize]
        public void Inicializar()
        {
            _almacenMock = new Mock<IAlmacenClientesChat>();
            _proveedorContextoMock = new Mock<IProveedorContextoOperacion>();
            _callbackMock = new Mock<IChatManejadorCallback>();

            _proveedorContextoMock
                .Setup(proveedor => proveedor.ExisteContexto)
                .Returns(true);
            _proveedorContextoMock
                .Setup(proveedor => proveedor.ObtenerCallbackChannel<IChatManejadorCallback>())
                .Returns(_callbackMock.Object);

            _chatManejador = new ChatManejador(
                _almacenMock.Object,
                _proveedorContextoMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_AlmacenNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ChatManejador(null, _proveedorContextoMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_ProveedorContextoNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ChatManejador(_almacenMock.Object, null));
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_DatosValidos_RegistraCliente()
        {
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno))
                .Returns(new List<ClienteChat>());

            _chatManejador.UnirseChatSala(IdSalaPrueba, NombreJugadorUno);

            _almacenMock.Verify(
                almacen => almacen.RegistrarOActualizarCliente(
                    IdSalaPrueba,
                    NombreJugadorUno,
                    _callbackMock.Object),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_NormalizaEspacios_RegistraCorrectamente()
        {
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno))
                .Returns(new List<ClienteChat>());

            _chatManejador.UnirseChatSala(IdSalaConEspacios, NombreJugadorConEspacios);

            _almacenMock.Verify(
                almacen => almacen.RegistrarOActualizarCliente(
                    IdSalaPrueba,
                    NombreJugadorUno,
                    _callbackMock.Object),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_SinContexto_LanzaFaultException()
        {
            _proveedorContextoMock
                .Setup(proveedor => proveedor.ExisteContexto)
                .Returns(false);

            Assert.ThrowsException<FaultException>(() =>
                _chatManejador.UnirseChatSala(IdSalaPrueba, NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_CallbackNulo_LanzaFaultException()
        {
            _proveedorContextoMock
                .Setup(proveedor => proveedor.ObtenerCallbackChannel<IChatManejadorCallback>())
                .Returns((IChatManejadorCallback)null);

            Assert.ThrowsException<FaultException>(() =>
                _chatManejador.UnirseChatSala(IdSalaPrueba, NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_IdSalaNula_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _chatManejador.UnirseChatSala(IdSalaNula, NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_NombreNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _chatManejador.UnirseChatSala(IdSalaPrueba, NombreNulo));
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_NotificaClientesExistentes_LlamaNotificarJugadorUnido()
        {
            var clienteExistente = new ClienteChat(NombreJugadorDos, _callbackMock.Object);
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno))
                .Returns(new List<ClienteChat> { clienteExistente });

            _chatManejador.UnirseChatSala(IdSalaPrueba, NombreJugadorUno);

            _callbackMock.Verify(
                callback => callback.NotificarJugadorUnido(NombreJugadorUno),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_DatosValidos_NotificaATodos()
        {
            var cliente = new ClienteChat(NombreJugadorUno, _callbackMock.Object);
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesSala(IdSalaPrueba))
                .Returns(new List<ClienteChat> { cliente });

            _chatManejador.EnviarMensaje(IdSalaPrueba, MensajePrueba, NombreJugadorUno);

            _callbackMock.Verify(
                callback => callback.RecibirMensaje(NombreJugadorUno, MensajePrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_NormalizaEspacios_EnviaMensajeTrimmeado()
        {
            var cliente = new ClienteChat(NombreJugadorUno, _callbackMock.Object);
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesSala(IdSalaPrueba))
                .Returns(new List<ClienteChat> { cliente });

            _chatManejador.EnviarMensaje(
                IdSalaConEspacios,
                MensajeConEspacios,
                NombreJugadorConEspacios);

            _callbackMock.Verify(
                callback => callback.RecibirMensaje(NombreJugadorUno, MensajePrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_MensajeVacio_NoNotifica()
        {
            _chatManejador.EnviarMensaje(IdSalaPrueba, MensajeVacio, NombreJugadorUno);

            _almacenMock.Verify(
                almacen => almacen.ObtenerClientesSala(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_MensajeNulo_NoNotifica()
        {
            _chatManejador.EnviarMensaje(IdSalaPrueba, MensajeNulo, NombreJugadorUno);

            _almacenMock.Verify(
                almacen => almacen.ObtenerClientesSala(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_SalaInexistente_NoLanzaExcepcion()
        {
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesSala(IdSalaPrueba))
                .Returns((List<ClienteChat>)null);

            _chatManejador.EnviarMensaje(IdSalaPrueba, MensajePrueba, NombreJugadorUno);

            _callbackMock.Verify(
                callback => callback.RecibirMensaje(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_IdSalaVacia_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _chatManejador.EnviarMensaje(IdSalaVacia, MensajePrueba, NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_NombreVacio_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _chatManejador.EnviarMensaje(IdSalaPrueba, MensajePrueba, NombreVacio));
        }

        [TestMethod]
        public void Prueba_SalirChatSala_DatosValidos_RemueveCliente()
        {
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno))
                .Returns(new List<ClienteChat>());

            _chatManejador.SalirChatSala(IdSalaPrueba, NombreJugadorUno);

            _almacenMock.Verify(
                almacen => almacen.RemoverCliente(IdSalaPrueba, NombreJugadorUno),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_SalirChatSala_NotificaClientesRestantes_LlamaNotificarJugadorSalio()
        {
            var clienteRestante = new ClienteChat(NombreJugadorDos, _callbackMock.Object);
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno))
                .Returns(new List<ClienteChat> { clienteRestante });

            _chatManejador.SalirChatSala(IdSalaPrueba, NombreJugadorUno);

            _callbackMock.Verify(
                callback => callback.NotificarJugadorSalio(NombreJugadorUno),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_SalirChatSala_IdSalaVacia_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _chatManejador.SalirChatSala(IdSalaVacia, NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_SalirChatSala_NombreVacio_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _chatManejador.SalirChatSala(IdSalaPrueba, NombreVacio));
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_ErrorComunicacion_RemueveClienteInalcanzable()
        {
            var callbackFallido = new Mock<IChatManejadorCallback>();
            callbackFallido
                .Setup(callback => callback.NotificarJugadorUnido(It.IsAny<string>()))
                .Throws(new CommunicationException("Error"));

            var clienteExistente = new ClienteChat(NombreJugadorDos, callbackFallido.Object);
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno))
                .Returns(new List<ClienteChat> { clienteExistente });

            _chatManejador.UnirseChatSala(IdSalaPrueba, NombreJugadorUno);

            _almacenMock.Verify(
                almacen => almacen.RemoverCliente(IdSalaPrueba, NombreJugadorDos),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_CallbackTimeout_RemueveClienteInalcanzable()
        {
            var callbackFallido = new Mock<IChatManejadorCallback>();
            callbackFallido
                .Setup(callback => callback.RecibirMensaje(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new TimeoutException("Timeout"));

            var cliente = new ClienteChat(NombreJugadorUno, callbackFallido.Object);
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesSala(IdSalaPrueba))
                .Returns(new List<ClienteChat> { cliente });

            _chatManejador.EnviarMensaje(IdSalaPrueba, MensajePrueba, NombreJugadorDos);

            _almacenMock.Verify(
                almacen => almacen.RemoverCliente(IdSalaPrueba, NombreJugadorUno),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EnviarMensaje_MultiplesClientes_NotificaATodos()
        {
            var callbackDos = new Mock<IChatManejadorCallback>();
            var clienteUno = new ClienteChat(NombreJugadorUno, _callbackMock.Object);
            var clienteDos = new ClienteChat(NombreJugadorDos, callbackDos.Object);

            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesSala(IdSalaPrueba))
                .Returns(new List<ClienteChat> { clienteUno, clienteDos });

            _chatManejador.EnviarMensaje(IdSalaPrueba, MensajePrueba, NombreJugadorUno);

            _callbackMock.Verify(
                callback => callback.RecibirMensaje(NombreJugadorUno, MensajePrueba),
                Times.Once);
            callbackDos.Verify(
                callback => callback.RecibirMensaje(NombreJugadorUno, MensajePrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_SalirChatSala_SinClientesRestantes_NoNotifica()
        {
            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno))
                .Returns(new List<ClienteChat>());

            _chatManejador.SalirChatSala(IdSalaPrueba, NombreJugadorUno);

            _callbackMock.Verify(
                callback => callback.NotificarJugadorSalio(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_UnirseChatSala_ConfiguraEventosCanal_CuandoExisteCanal()
        {
            var canalMock = new Mock<IContextChannel>();
            _proveedorContextoMock
                .Setup(proveedor => proveedor.ObtenerCanalActual())
                .Returns(canalMock.Object);

            _almacenMock
                .Setup(almacen => almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno))
                .Returns(new List<ClienteChat>());

            _chatManejador.UnirseChatSala(IdSalaPrueba, NombreJugadorUno);

            canalMock.VerifyAdd(
                canal => canal.Closed += It.IsAny<EventHandler>(),
                Times.Once);
            canalMock.VerifyAdd(
                canal => canal.Faulted += It.IsAny<EventHandler>(),
                Times.Once);
        }
    }
}
