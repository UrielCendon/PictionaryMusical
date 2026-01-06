using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class AlmacenClientesChatPruebas
    {
        private const string IdSalaPrueba = "123456";
        private const string NombreJugadorPrueba = "Jugador1";

        private AlmacenClientesChat _almacen;
        private Mock<IChatManejadorCallback> _mockCallback;

        [TestInitialize]
        public void Inicializar()
        {
            _almacen = new AlmacenClientesChat();
            _mockCallback = new Mock<IChatManejadorCallback>();
        }

        [TestMethod]
        public void Prueba_ObtenerClientesSala_RetornaListaVaciaSiSalaNoExiste()
        {
            var clientes = _almacen.ObtenerClientesSala("SalaInexistente");

            Assert.AreEqual(0, clientes.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerOCrearListaClientes_CreaListaSiNoExiste()
        {
            var clientes = _almacen.ObtenerOCrearListaClientes(IdSalaPrueba);

            Assert.AreEqual(0, clientes.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerOCrearListaClientes_RetornaListaExistente()
        {
            var primeraLista = _almacen.ObtenerOCrearListaClientes(IdSalaPrueba);
            var segundaLista = _almacen.ObtenerOCrearListaClientes(IdSalaPrueba);

            Assert.AreSame(primeraLista, segundaLista);
        }

        [TestMethod]
        public void Prueba_RegistrarOActualizarCliente_AgregaNuevoCliente()
        {
            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, NombreJugadorPrueba, _mockCallback.Object);

            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(1, clientes.Count);
        }

        [TestMethod]
        public void Prueba_RegistrarOActualizarCliente_ActualizaCallbackExistente()
        {
            var mockCallbackNuevo = new Mock<IChatManejadorCallback>();
            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, NombreJugadorPrueba, _mockCallback.Object);

            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, NombreJugadorPrueba, mockCallbackNuevo.Object);

            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(1, clientes.Count);
        }

        [TestMethod]
        public void Prueba_RemoverCliente_RetornaVerdaderoSiClienteExiste()
        {
            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, NombreJugadorPrueba, _mockCallback.Object);

            var resultado = _almacen.RemoverCliente(IdSalaPrueba, NombreJugadorPrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_RemoverCliente_RetornaFalsoSiSalaNoExiste()
        {
            var resultado = _almacen.RemoverCliente("SalaInexistente", NombreJugadorPrueba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_RemoverCliente_RetornaFalsoSiClienteNoExiste()
        {
            _almacen.ObtenerOCrearListaClientes(IdSalaPrueba);

            var resultado = _almacen.RemoverCliente(IdSalaPrueba, "JugadorInexistente");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_RemoverCliente_EliminaClienteDeLaLista()
        {
            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, NombreJugadorPrueba, _mockCallback.Object);

            _almacen.RemoverCliente(IdSalaPrueba, NombreJugadorPrueba);

            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(0, clientes.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerClientesExcluyendo_ExcluyeJugadorEspecificado()
        {
            var mockCallback2 = new Mock<IChatManejadorCallback>();
            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, NombreJugadorPrueba, _mockCallback.Object);
            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, "Jugador2", mockCallback2.Object);

            var clientes = _almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorPrueba);

            Assert.AreEqual(1, clientes.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerClientesExcluyendo_RetornaListaVaciaSiSalaNoExiste()
        {
            var clientes = _almacen.ObtenerClientesExcluyendo("SalaInexistente", NombreJugadorPrueba);

            Assert.AreEqual(0, clientes.Count);
        }

        [TestMethod]
        public void Prueba_ExisteSala_RetornaVerdaderoSiSalaExiste()
        {
            _almacen.ObtenerOCrearListaClientes(IdSalaPrueba);

            var existe = _almacen.ExisteSala(IdSalaPrueba);

            Assert.IsTrue(existe);
        }

        [TestMethod]
        public void Prueba_ExisteSala_RetornaFalsoSiSalaNoExiste()
        {
            var existe = _almacen.ExisteSala("SalaInexistente");

            Assert.IsFalse(existe);
        }

        [TestMethod]
        public void Prueba_LimpiarSalaVacia_EliminaSalaSinClientes()
        {
            _almacen.ObtenerOCrearListaClientes(IdSalaPrueba);

            _almacen.LimpiarSalaVacia(IdSalaPrueba);

            Assert.IsFalse(_almacen.ExisteSala(IdSalaPrueba));
        }

        [TestMethod]
        public void Prueba_LimpiarSalaVacia_NoEliminaSalaConClientes()
        {
            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, NombreJugadorPrueba, _mockCallback.Object);

            _almacen.LimpiarSalaVacia(IdSalaPrueba);

            Assert.IsTrue(_almacen.ExisteSala(IdSalaPrueba));
        }

        [TestMethod]
        public void Prueba_Constructor_AceptaDiccionarioNulo()
        {
            var almacen = new AlmacenClientesChat(null);

            Assert.IsNotNull(almacen);
        }

        [TestMethod]
        public void Prueba_RegistrarOActualizarCliente_IgnoraMayusculasMinusculas()
        {
            _almacen.RegistrarOActualizarCliente(IdSalaPrueba, "jugador1", _mockCallback.Object);

            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(1, clientes.Count);
        }
    }
}
