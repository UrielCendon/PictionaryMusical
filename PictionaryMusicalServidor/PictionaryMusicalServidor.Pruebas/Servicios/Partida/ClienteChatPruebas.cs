using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class ClienteChatPruebas
    {
        private const string NombreJugadorPrueba = "Jugador1";

        [TestMethod]
        public void Prueba_Constructor_AsignaNombreJugadorCorrectamente()
        {
            var mockCallback = new Mock<IChatManejadorCallback>();

            var cliente = new ClienteChat(NombreJugadorPrueba, mockCallback.Object);

            Assert.AreEqual(NombreJugadorPrueba, cliente.NombreJugador);
        }

        [TestMethod]
        public void Prueba_Constructor_AsignaCallbackCorrectamente()
        {
            var mockCallback = new Mock<IChatManejadorCallback>();

            var cliente = new ClienteChat(NombreJugadorPrueba, mockCallback.Object);

            Assert.AreEqual(mockCallback.Object, cliente.Callback);
        }

        [TestMethod]
        public void Prueba_Callback_PermiteCambiarValor()
        {
            var mockCallbackOriginal = new Mock<IChatManejadorCallback>();
            var mockCallbackNuevo = new Mock<IChatManejadorCallback>();
            var cliente = new ClienteChat(NombreJugadorPrueba, mockCallbackOriginal.Object);

            cliente.Callback = mockCallbackNuevo.Object;

            Assert.AreEqual(mockCallbackNuevo.Object, cliente.Callback);
        }

        [TestMethod]
        public void Prueba_Constructor_AceptaCallbackNulo()
        {
            var cliente = new ClienteChat(NombreJugadorPrueba, null);

            Assert.IsNull(cliente.Callback);
        }

        [TestMethod]
        public void Prueba_Constructor_AceptaNombreVacio()
        {
            var mockCallback = new Mock<IChatManejadorCallback>();

            var cliente = new ClienteChat(string.Empty, mockCallback.Object);

            Assert.AreEqual(string.Empty, cliente.NombreJugador);
        }
    }
}
