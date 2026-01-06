using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Datos.Modelo;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class ListaAmigosManejadorPruebas
    {
        private const string NombreUsuarioPrueba = "UsuarioPrueba";
        private const string NombreUsuarioVacio = "";
        private const string NombreUsuarioSoloEspacios = "   ";
        private const int IdUsuarioPrueba = 1;
        private const int IdAmigoPrueba = 2;
        private const string NombreAmigoPrueba = "AmigoPrueba";

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<IAmistadServicio> _mockAmistadServicio;
        private Mock<INotificadorListaAmigos> _mockNotificador;
        private Mock<IManejadorCallback<IListaAmigosManejadorCallback>> _mockManejadorCallback;
        private Mock<IProveedorCallback<IListaAmigosManejadorCallback>> _mockProveedorCallback;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private ListaAmigosManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockAmistadServicio = new Mock<IAmistadServicio>();
            _mockNotificador = new Mock<INotificadorListaAmigos>();
            _mockManejadorCallback = 
                new Mock<IManejadorCallback<IListaAmigosManejadorCallback>>();
            _mockProveedorCallback = 
                new Mock<IProveedorCallback<IListaAmigosManejadorCallback>>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();

            _mockContextoFactoria
                .Setup(factoria => factoria.CrearContexto())
                .Returns(_mockContexto.Object);

            _manejador = new ListaAmigosManejador(
                _mockContextoFactoria.Object,
                _mockRepositorioFactoria.Object,
                _mockAmistadServicio.Object,
                _mockNotificador.Object,
                _mockManejadorCallback.Object,
                _mockProveedorCallback.Object);
        }
        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ListaAmigosManejador(
                    null,
                    _mockRepositorioFactoria.Object,
                    _mockAmistadServicio.Object,
                    _mockNotificador.Object,
                    _mockManejadorCallback.Object,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ListaAmigosManejador(
                    _mockContextoFactoria.Object,
                    null,
                    _mockAmistadServicio.Object,
                    _mockNotificador.Object,
                    _mockManejadorCallback.Object,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionAmistadServicioNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ListaAmigosManejador(
                    _mockContextoFactoria.Object,
                    _mockRepositorioFactoria.Object,
                    null,
                    _mockNotificador.Object,
                    _mockManejadorCallback.Object,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionNotificadorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ListaAmigosManejador(
                    _mockContextoFactoria.Object,
                    _mockRepositorioFactoria.Object,
                    _mockAmistadServicio.Object,
                    null,
                    _mockManejadorCallback.Object,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionManejadorCallbackNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ListaAmigosManejador(
                    _mockContextoFactoria.Object,
                    _mockRepositorioFactoria.Object,
                    _mockAmistadServicio.Object,
                    _mockNotificador.Object,
                    null,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionProveedorCallbackNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ListaAmigosManejador(
                    _mockContextoFactoria.Object,
                    _mockRepositorioFactoria.Object,
                    _mockAmistadServicio.Object,
                    _mockNotificador.Object,
                    _mockManejadorCallback.Object,
                    null));
        }        
        
        [TestMethod]
        public void Prueba_Suscribir_LanzaExcepcionNombreNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.Suscribir(null));
        }

        [TestMethod]
        public void Prueba_Suscribir_LanzaExcepcionNombreVacio()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.Suscribir(NombreUsuarioVacio));
        }

        [TestMethod]
        public void Prueba_Suscribir_LanzaExcepcionNombreSoloEspacios()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.Suscribir(NombreUsuarioSoloEspacios));
        }

        [TestMethod]
        public void Prueba_Suscribir_LlamaSuscribirCallback()
        {
            ConfigurarMocksSuscripcion();

            _manejador.Suscribir(NombreUsuarioPrueba);

            _mockManejadorCallback.Verify(
                m => m.Suscribir(
                    NombreUsuarioPrueba, 
                    It.IsAny<IListaAmigosManejadorCallback>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_Suscribir_LlamaConfigurarEventosCanal()
        {
            ConfigurarMocksSuscripcion();

            _manejador.Suscribir(NombreUsuarioPrueba);

            _mockManejadorCallback.Verify(
                m => m.ConfigurarEventosCanal(NombreUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_Suscribir_LlamaNotificarLista()
        {
            ConfigurarMocksSuscripcion();

            _manejador.Suscribir(NombreUsuarioPrueba);

            _mockNotificador.Verify(
                n => n.NotificarLista(
                    NombreUsuarioPrueba, 
                    It.IsAny<List<AmigoDTO>>()),
                Times.Once);
        }        
        
        [TestMethod]
        public void Prueba_CancelarSuscripcion_LanzaExcepcionNombreNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.CancelarSuscripcion(null));
        }

        [TestMethod]
        public void Prueba_CancelarSuscripcion_LanzaExcepcionNombreVacio()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.CancelarSuscripcion(NombreUsuarioVacio));
        }

        [TestMethod]
        public void Prueba_CancelarSuscripcion_LlamaDesuscribir()
        {
            _manejador.CancelarSuscripcion(NombreUsuarioPrueba);

            _mockManejadorCallback.Verify(
                m => m.Desuscribir(NombreUsuarioPrueba),
                Times.Once);
        }        
        
        [TestMethod]
        public void Prueba_ObtenerAmigos_LanzaExcepcionNombreNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.ObtenerAmigos(null));
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_LanzaExcepcionNombreVacio()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.ObtenerAmigos(NombreUsuarioVacio));
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_RetornaListaAmigos()
        {
            ConfigurarMocksObtenerAmigos();

            var resultado = _manejador.ObtenerAmigos(NombreUsuarioPrueba);

            Assert.AreEqual(1, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_RetornaNombreAmigoCorrecto()
        {
            ConfigurarMocksObtenerAmigos();

            var resultado = _manejador.ObtenerAmigos(NombreUsuarioPrueba);

            Assert.AreEqual(NombreAmigoPrueba, resultado[0].NombreUsuario);
        }        
        private void ConfigurarMocksSuscripcion()
        {
            var listaAmigos = new List<AmigoDTO>
            {
                new AmigoDTO 
                { 
                    UsuarioId = IdAmigoPrueba, 
                    NombreUsuario = NombreAmigoPrueba 
                }
            };

            ConfigurarMocksObtenerAmigosInterno(listaAmigos);

            _mockProveedorCallback
                .Setup(proveedor => proveedor.ObtenerCallbackActual())
                .Returns(Mock.Of<IListaAmigosManejadorCallback>());
        }

        private void ConfigurarMocksObtenerAmigos()
        {
            var listaAmigos = new List<AmigoDTO>
            {
                new AmigoDTO 
                { 
                    UsuarioId = IdAmigoPrueba, 
                    NombreUsuario = NombreAmigoPrueba 
                }
            };

            ConfigurarMocksObtenerAmigosInterno(listaAmigos);
        }

        private void ConfigurarMocksObtenerAmigosInterno(List<AmigoDTO> listaAmigos)
        {
            var mockUsuarioRepositorio = 
                new Mock<PictionaryMusicalServidor.Datos.DAL.Interfaces.IUsuarioRepositorio>();
            var usuario = new Usuario 
            { 
                idUsuario = IdUsuarioPrueba, 
                Nombre_Usuario = NombreUsuarioPrueba 
            };

            mockUsuarioRepositorio
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioPrueba))
                .Returns(usuario);

            _mockRepositorioFactoria
                .Setup(factoria => factoria.CrearUsuarioRepositorio(_mockContexto.Object))
                .Returns(mockUsuarioRepositorio.Object);

            _mockAmistadServicio
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Returns(listaAmigos);
        }    
    }
}
