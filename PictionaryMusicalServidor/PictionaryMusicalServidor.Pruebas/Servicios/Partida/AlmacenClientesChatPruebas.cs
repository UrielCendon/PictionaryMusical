using System;
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
        private const string IdSalaPrueba = "SALA001";
        private const string IdSalaAlternativa = "SALA002";
        private const string NombreJugadorUno = "JugadorUno";
        private const string NombreJugadorDos = "JugadorDos";
        private const string NombreJugadorInexistente = "JugadorInexistente";
        private const int CantidadCero = 0;
        private const int CantidadUno = 1;
        private const int CantidadDos = 2;

        private AlmacenClientesChat _almacen;
        private Mock<IChatManejadorCallback> _callbackMock;

        [TestInitialize]
        public void Inicializar()
        {
            _almacen = new AlmacenClientesChat();
            _callbackMock = new Mock<IChatManejadorCallback>();
        }

        [TestMethod]
        public void Prueba_ObtenerClientesSala_SalaInexistente_RetornaListaVacia()
        {
            var resultado = _almacen.ObtenerClientesSala(IdSalaPrueba);

            Assert.AreEqual(CantidadCero, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerOCrearListaClientes_SalaNueva_CreaListaVacia()
        {
            var resultado = _almacen.ObtenerOCrearListaClientes(IdSalaPrueba);

            Assert.AreEqual(CantidadCero, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerOCrearListaClientes_SalaExistente_RetornaMismaLista()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            var resultado = _almacen.ObtenerOCrearListaClientes(IdSalaPrueba);

            Assert.AreEqual(CantidadUno, resultado.Count);
        }

        [TestMethod]
        public void Prueba_RegistrarOActualizarCliente_ClienteNuevo_AgregaCliente()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(CantidadUno, clientes.Count);
            Assert.AreEqual(NombreJugadorUno, clientes[0].NombreJugador);
        }

        [TestMethod]
        public void Prueba_RegistrarOActualizarCliente_ClienteExistente_ActualizaCallback()
        {
            var callbackOriginal = new Mock<IChatManejadorCallback>();
            var callbackNuevo = new Mock<IChatManejadorCallback>();

            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba, 
                NombreJugadorUno, 
                callbackOriginal.Object);
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba, 
                NombreJugadorUno, 
                callbackNuevo.Object);

            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(CantidadUno, clientes.Count);
            Assert.AreEqual(callbackNuevo.Object, clientes[0].Callback);
        }

        [TestMethod]
        public void Prueba_RegistrarOActualizarCliente_MultiplesClientes_TodosRegistrados()
        {
            var callbackDos = new Mock<IChatManejadorCallback>();

            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorDos,
                callbackDos.Object);

            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(CantidadDos, clientes.Count);
        }

        [TestMethod]
        public void Prueba_RemoverCliente_ClienteExistente_RetornaTrue()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            bool resultado = _almacen.RemoverCliente(IdSalaPrueba, NombreJugadorUno);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_RemoverCliente_ClienteInexistente_RetornaFalse()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            bool resultado = _almacen.RemoverCliente(IdSalaPrueba, NombreJugadorInexistente);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_RemoverCliente_SalaInexistente_RetornaFalse()
        {
            bool resultado = _almacen.RemoverCliente(IdSalaPrueba, NombreJugadorUno);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_RemoverCliente_UltimoCliente_EliminaSala()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            _almacen.RemoverCliente(IdSalaPrueba, NombreJugadorUno);

            Assert.IsFalse(_almacen.ExisteSala(IdSalaPrueba));
        }

        [TestMethod]
        public void Prueba_RemoverCliente_QuedanClientes_SalaPersiste()
        {
            var callbackDos = new Mock<IChatManejadorCallback>();
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorDos,
                callbackDos.Object);

            _almacen.RemoverCliente(IdSalaPrueba, NombreJugadorUno);

            Assert.IsTrue(_almacen.ExisteSala(IdSalaPrueba));
            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(CantidadUno, clientes.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerClientesExcluyendo_ExcluyeJugadorCorrectamente()
        {
            var callbackDos = new Mock<IChatManejadorCallback>();
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorDos,
                callbackDos.Object);

            var resultado = _almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno);

            Assert.AreEqual(CantidadUno, resultado.Count);
            Assert.AreEqual(NombreJugadorDos, resultado[0].NombreJugador);
        }

        [TestMethod]
        public void Prueba_ObtenerClientesExcluyendo_SalaInexistente_RetornaListaVacia()
        {
            var resultado = _almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno);

            Assert.AreEqual(CantidadCero, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerClientesExcluyendo_TodosExcluidos_RetornaListaVacia()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            var resultado = _almacen.ObtenerClientesExcluyendo(IdSalaPrueba, NombreJugadorUno);

            Assert.AreEqual(CantidadCero, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ExisteSala_SalaExistente_RetornaTrue()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            bool resultado = _almacen.ExisteSala(IdSalaPrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteSala_SalaInexistente_RetornaFalse()
        {
            bool resultado = _almacen.ExisteSala(IdSalaPrueba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_LimpiarSalaVacia_SalaConClientes_NoElimina()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            _almacen.LimpiarSalaVacia(IdSalaPrueba);

            Assert.IsTrue(_almacen.ExisteSala(IdSalaPrueba));
        }

        [TestMethod]
        public void Prueba_LimpiarSalaVacia_SalaInexistente_NoLanzaExcepcion()
        {
            _almacen.LimpiarSalaVacia(IdSalaPrueba);

            Assert.IsFalse(_almacen.ExisteSala(IdSalaPrueba));
        }

        [TestMethod]
        public void Prueba_RegistrarCliente_DiferentesSalas_IndependientesPorSala()
        {
            var callbackDos = new Mock<IChatManejadorCallback>();
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);
            _almacen.RegistrarOActualizarCliente(
                IdSalaAlternativa, 
                NombreJugadorDos, 
                callbackDos.Object);

            var clientesSalaUno = _almacen.ObtenerClientesSala(IdSalaPrueba);
            var clientesSalaDos = _almacen.ObtenerClientesSala(IdSalaAlternativa);

            Assert.AreEqual(CantidadUno, clientesSalaUno.Count);
            Assert.AreEqual(CantidadUno, clientesSalaDos.Count);
            Assert.AreEqual(NombreJugadorUno, clientesSalaUno[0].NombreJugador);
            Assert.AreEqual(NombreJugadorDos, clientesSalaDos[0].NombreJugador);
        }

        [TestMethod]
        public void Prueba_ObtenerClientesSala_RetornaCopia_ModificarNoAfectaOriginal()
        {
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                NombreJugadorUno,
                _callbackMock.Object);

            var copia = _almacen.ObtenerClientesSala(IdSalaPrueba);
            copia.Clear();

            var original = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(CantidadUno, original.Count);
        }

        [TestMethod]
        public void Prueba_RegistrarCliente_NombreCaseInsensitive_ActualizaExistente()
        {
            var callbackNuevo = new Mock<IChatManejadorCallback>();

            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                "jugadoruno",
                _callbackMock.Object);
            _almacen.RegistrarOActualizarCliente(
                IdSalaPrueba,
                "JUGADORUNO",
                callbackNuevo.Object);

            var clientes = _almacen.ObtenerClientesSala(IdSalaPrueba);
            Assert.AreEqual(CantidadUno, clientes.Count);
            Assert.AreEqual(callbackNuevo.Object, clientes[0].Callback);
        }
    }
}
