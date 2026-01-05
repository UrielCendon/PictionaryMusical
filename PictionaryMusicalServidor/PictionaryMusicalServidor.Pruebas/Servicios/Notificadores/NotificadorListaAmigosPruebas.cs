using System;
using System.Collections.Generic;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class NotificadorListaAmigosPruebas
    {
        private const string NombreUsuarioPrueba = "UsuarioTest";
        private const string NombreAmigoPrueba = "AmigoTest";
        private const int IdAmigoPrueba = 2;

        private Mock<IManejadorCallback<IListaAmigosManejadorCallback>> manejadorCallbackMock;
        private Mock<IAmistadServicio> amistadServicioMock;
        private Mock<IRepositorioFactoria> repositorioFactoriaMock;
        private Mock<IContextoFactoria> contextoFactoriaMock;
        private Mock<IListaAmigosManejadorCallback> callbackMock;
        private Mock<BaseDatosPruebaEntities> contextoMock;
        private Mock<IUsuarioRepositorio> usuarioRepositorioMock;
        private NotificadorListaAmigos notificador;

        [TestInitialize]
        public void Inicializar()
        {
            manejadorCallbackMock = new Mock<IManejadorCallback<IListaAmigosManejadorCallback>>();
            amistadServicioMock = new Mock<IAmistadServicio>();
            repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            contextoFactoriaMock = new Mock<IContextoFactoria>();
            callbackMock = new Mock<IListaAmigosManejadorCallback>();
            contextoMock = new Mock<BaseDatosPruebaEntities>();
            usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();

            contextoFactoriaMock
                .Setup(factory => factory.CrearContexto())
                .Returns(contextoMock.Object);

            repositorioFactoriaMock
                .Setup(factory => factory.CrearUsuarioRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(usuarioRepositorioMock.Object);

            notificador = new NotificadorListaAmigos(
                manejadorCallbackMock.Object,
                amistadServicioMock.Object,
                contextoFactoriaMock.Object,
                repositorioFactoriaMock.Object);
        }

        [TestMethod]
        public void Prueba_NotificarLista_ConCallbackValidoInvocaNotificacion()
        {
            var listaAmigos = new List<AmigoDTO>
            {
                new AmigoDTO
                {
                    NombreUsuario = NombreAmigoPrueba,
                    UsuarioId = IdAmigoPrueba
                }
            };

            manejadorCallbackMock
                .Setup(handler => handler.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(callbackMock.Object);

            notificador.NotificarLista(NombreUsuarioPrueba, listaAmigos);

            callbackMock.Verify(
                callback => callback.NotificarListaAmigosActualizada(listaAmigos),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLista_CallbackNuloNoLanzaExcepcion()
        {
            var listaAmigos = new List<AmigoDTO>();

            manejadorCallbackMock
                .Setup(handler => handler.ObtenerCallback(NombreUsuarioPrueba))
                .Returns((IListaAmigosManejadorCallback)null);

            notificador.NotificarLista(NombreUsuarioPrueba, listaAmigos);

            callbackMock.Verify(
                callback => callback.NotificarListaAmigosActualizada(It.IsAny<List<AmigoDTO>>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarLista_ListaVaciaInvocaCallback()
        {
            var listaVacia = new List<AmigoDTO>();

            manejadorCallbackMock
                .Setup(handler => handler.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(callbackMock.Object);

            notificador.NotificarLista(NombreUsuarioPrueba, listaVacia);

            callbackMock.Verify(
                callback => callback.NotificarListaAmigosActualizada(listaVacia),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLista_CallbackFallaNoLanzaExcepcion()
        {
            var listaAmigos = new List<AmigoDTO>();

            manejadorCallbackMock
                .Setup(handler => handler.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(callbackMock.Object);

            callbackMock
                .Setup(callback => callback.NotificarListaAmigosActualizada(
                    It.IsAny<List<AmigoDTO>>()))
                .Throws(new Exception("Error de callback"));

            notificador.NotificarLista(NombreUsuarioPrueba, listaAmigos);

            manejadorCallbackMock.Verify(
                handler => handler.ObtenerCallback(NombreUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_NombreNuloNoNotifica()
        {
            notificador.NotificarCambioAmistad(null);

            manejadorCallbackMock.Verify(
                handler => handler.ObtenerCallback(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_NombreVacioNoNotifica()
        {
            notificador.NotificarCambioAmistad(string.Empty);

            manejadorCallbackMock.Verify(
                handler => handler.ObtenerCallback(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Prueba_Constructor_RepositorioFactoriaNuloLanzaExcepcion()
        {
            new NotificadorListaAmigos(
                manejadorCallbackMock.Object,
                amistadServicioMock.Object,
                null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Prueba_ConstructorCompleto_ContextoFactoriaNuloLanzaExcepcion()
        {
            new NotificadorListaAmigos(
                manejadorCallbackMock.Object,
                amistadServicioMock.Object,
                null,
                repositorioFactoriaMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Prueba_ConstructorCompleto_RepositorioFactoriaNuloLanzaExcepcion()
        {
            new NotificadorListaAmigos(
                manejadorCallbackMock.Object,
                amistadServicioMock.Object,
                contextoFactoriaMock.Object,
                null);
        }
    }
}
