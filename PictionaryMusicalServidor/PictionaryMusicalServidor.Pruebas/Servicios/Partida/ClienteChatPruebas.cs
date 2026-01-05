using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class ClienteChatPruebas
    {
        private const string NombreJugadorPrueba = "JugadorTest";
        private const string NombreJugadorAlternativo = "OtroJugador";
        private const string NombreJugadorVacio = "";

        private Mock<IChatManejadorCallback> _callbackMock;

        [TestInitialize]
        public void Inicializar()
        {
            _callbackMock = new Mock<IChatManejadorCallback>();
        }

        [TestMethod]
        public void Prueba_Constructor_ValoresValidos_AsignaCorrectamente()
        {
            var cliente = new ClienteChat(NombreJugadorPrueba, _callbackMock.Object);

            Assert.AreEqual(NombreJugadorPrueba, cliente.NombreJugador);
            Assert.AreEqual(_callbackMock.Object, cliente.Callback);
        }

        [TestMethod]
        public void Prueba_Constructor_NombreVacio_AsignaCorrectamente()
        {
            var cliente = new ClienteChat(NombreJugadorVacio, _callbackMock.Object);

            Assert.AreEqual(NombreJugadorVacio, cliente.NombreJugador);
        }

        [TestMethod]
        public void Prueba_Constructor_NombreNulo_AsignaNulo()
        {
            var cliente = new ClienteChat(null, _callbackMock.Object);

            Assert.IsNull(cliente.NombreJugador);
        }

        [TestMethod]
        public void Prueba_Constructor_CallbackNulo_AsignaNulo()
        {
            var cliente = new ClienteChat(NombreJugadorPrueba, null);

            Assert.IsNull(cliente.Callback);
        }

        [TestMethod]
        public void Prueba_NombreJugador_PropiedadSoloLectura_MantieneSuValor()
        {
            var cliente = new ClienteChat(NombreJugadorPrueba, _callbackMock.Object);

            string nombreObtenido = cliente.NombreJugador;

            Assert.AreEqual(NombreJugadorPrueba, nombreObtenido);
        }

        [TestMethod]
        public void Prueba_Callback_ModificarValor_ActualizaCorrectamente()
        {
            var cliente = new ClienteChat(NombreJugadorPrueba, _callbackMock.Object);
            var nuevoCallbackMock = new Mock<IChatManejadorCallback>();

            cliente.Callback = nuevoCallbackMock.Object;

            Assert.AreEqual(nuevoCallbackMock.Object, cliente.Callback);
        }

        [TestMethod]
        public void Prueba_Callback_AsignarNulo_PermiteNulo()
        {
            var cliente = new ClienteChat(NombreJugadorPrueba, _callbackMock.Object);

            cliente.Callback = null;

            Assert.IsNull(cliente.Callback);
        }

        [TestMethod]
        public void Prueba_Constructor_DosInstancias_SonIndependientes()
        {
            var callbackMockDos = new Mock<IChatManejadorCallback>();
            var clienteUno = new ClienteChat(NombreJugadorPrueba, _callbackMock.Object);
            var clienteDos = new ClienteChat(NombreJugadorAlternativo, callbackMockDos.Object);

            Assert.AreNotEqual(clienteUno.NombreJugador, clienteDos.NombreJugador);
            Assert.AreNotEqual(clienteUno.Callback, clienteDos.Callback);
        }

        [TestMethod]
        public void Prueba_Callback_ReasignarMultiplesVeces_ConservaUltimoValor()
        {
            var cliente = new ClienteChat(NombreJugadorPrueba, _callbackMock.Object);
            var callbackDos = new Mock<IChatManejadorCallback>();
            var callbackTres = new Mock<IChatManejadorCallback>();

            cliente.Callback = callbackDos.Object;
            cliente.Callback = callbackTres.Object;

            Assert.AreEqual(callbackTres.Object, cliente.Callback);
        }
    }
}
