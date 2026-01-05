using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class ListaAmigosManejadorPrueba
    {
        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IAmistadServicio> _amistadServicioMock;
        private Mock<INotificadorListaAmigos> _notificadorListaMock;
        private Mock<IManejadorCallback<IListaAmigosManejadorCallback>> _manejadorCallbackMock;
        private Mock<IProveedorCallback<IListaAmigosManejadorCallback>> _proveedorCallbackMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private ListaAmigosManejador _listaAmigosManejador;

        private const string NombreUsuarioPrueba = "UsuarioLista";
        private const string NombreAmigoPrueba = "AmigoPrueba";
        private const int IdUsuarioPrueba = 50;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _amistadServicioMock = new Mock<IAmistadServicio>();
            _notificadorListaMock = new Mock<INotificadorListaAmigos>();
            _manejadorCallbackMock = 
                new Mock<IManejadorCallback<IListaAmigosManejadorCallback>>();
            _proveedorCallbackMock = 
                new Mock<IProveedorCallback<IListaAmigosManejadorCallback>>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock
                .Setup(contextoFactoria => contextoFactoria.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(repositorioFactoria => repositorioFactoria
                    .CrearUsuarioRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _listaAmigosManejador = new ListaAmigosManejador(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object,
                _amistadServicioMock.Object,
                _notificadorListaMock.Object,
                _manejadorCallbackMock.Object,
                _proveedorCallbackMock.Object);
        }

        [TestMethod]
        public void Prueba_Suscribir_FlujoExitoso()
        {
            var usuario = new Usuario
            {
                idUsuario = IdUsuarioPrueba,
                Nombre_Usuario = NombreUsuarioPrueba
            };
            var listaAmigos = new List<AmigoDTO>();

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioPrueba))
                .Returns(usuario);
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Returns(listaAmigos);
            _proveedorCallbackMock
                .Setup(proveedor => proveedor.ObtenerCallbackActual())
                .Returns(new Mock<IListaAmigosManejadorCallback>().Object);

            _listaAmigosManejador.Suscribir(NombreUsuarioPrueba);

            _manejadorCallbackMock.Verify(
                manejador => manejador.Suscribir(
                    NombreUsuarioPrueba,
                    It.IsAny<IListaAmigosManejadorCallback>()),
                Times.Once);
            _notificadorListaMock.Verify(
                notificador => notificador.NotificarLista(NombreUsuarioPrueba, listaAmigos),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_Suscribir_ErrorBaseDatos_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioPrueba))
                .Throws(new EntityException());

            Assert.ThrowsException<FaultException>(
                () => _listaAmigosManejador.Suscribir(NombreUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_UsuarioNoExiste_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioPrueba))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _listaAmigosManejador.ObtenerAmigos(NombreUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_FlujoExitoso()
        {
            var usuario = new Usuario
            {
                idUsuario = IdUsuarioPrueba,
                Nombre_Usuario = NombreUsuarioPrueba
            };
            var listaEsperada = new List<AmigoDTO>
            {
                new AmigoDTO { NombreUsuario = NombreAmigoPrueba }
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioPrueba))
                .Returns(usuario);
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Returns(listaEsperada);

            var resultado = _listaAmigosManejador.ObtenerAmigos(NombreUsuarioPrueba);

            CollectionAssert.AreEqual(listaEsperada, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_ErrorBaseDatos_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioPrueba))
                .Throws(new EntityException());

            Assert.ThrowsException<FaultException>(
                () => _listaAmigosManejador.ObtenerAmigos(NombreUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_CancelarSuscripcion_FlujoExitoso()
        {
            _listaAmigosManejador.CancelarSuscripcion(NombreUsuarioPrueba);

            _manejadorCallbackMock.Verify(
                manejador => manejador.Desuscribir(NombreUsuarioPrueba),
                Times.Once);
        }
    }
}