using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Data.Entity.Core;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class AmigosManejadorPrueba
    {
        private Mock<IAmistadServicio> _amistadServicioMock;
        private Mock<IOperacionAmistadServicio> _operacionAmistadServicioMock;
        private Mock<INotificadorListaAmigos> _notificadorListaAmigosMock;
        private Mock<INotificadorAmigos> _notificadorAmigosMock;
        private Mock<IManejadorCallback<IAmigosManejadorCallback>> _manejadorCallbackMock;
        private Mock<IProveedorCallback<IAmigosManejadorCallback>> _proveedorCallbackMock;
        private Mock<IAmigosManejadorCallback> _callbackMock;
        private AmigosManejador _amigosManejador;

        private const string NombreUsuarioPrueba = "UsuarioPrueba";
        private const string NombreAmigoPrueba = "AmigoPrueba";
        private const string NombreUsuarioNormalizado = "USUARIOPRUEBA";
        private const string NombreAmigoNormalizado = "AMIGOPRUEBA";
        private const int IdUsuarioPrueba = 10;

        [TestInitialize]
        public void Inicializar()
        {
            _amistadServicioMock = new Mock<IAmistadServicio>();
            _operacionAmistadServicioMock = new Mock<IOperacionAmistadServicio>();
            _notificadorListaAmigosMock = new Mock<INotificadorListaAmigos>();
            _notificadorAmigosMock = new Mock<INotificadorAmigos>();
            _manejadorCallbackMock = new Mock<IManejadorCallback<IAmigosManejadorCallback>>();
            _proveedorCallbackMock = new Mock<IProveedorCallback<IAmigosManejadorCallback>>();
            _callbackMock = new Mock<IAmigosManejadorCallback>();

            _proveedorCallbackMock
                .Setup(proveedor => proveedor.ObtenerCallbackActual())
                .Returns(_callbackMock.Object);

            _amigosManejador = new AmigosManejador(
                _amistadServicioMock.Object,
                _operacionAmistadServicioMock.Object,
                _notificadorListaAmigosMock.Object,
                _notificadorAmigosMock.Object,
                _manejadorCallbackMock.Object,
                _proveedorCallbackMock.Object);
        }

        [TestMethod]
        public void Prueba_Suscribir_FlujoExitoso()
        {
            var datosSuscripcion = new DatosSuscripcionUsuario
            {
                IdUsuario = IdUsuarioPrueba,
                NombreNormalizado = NombreUsuarioNormalizado
            };

            _operacionAmistadServicioMock
                .Setup(operacion => operacion.ObtenerDatosUsuarioSuscripcion(NombreUsuarioPrueba))
                .Returns(datosSuscripcion);

            _amigosManejador.Suscribir(NombreUsuarioPrueba);

            _manejadorCallbackMock.Verify(
                manejador => manejador.Suscribir(NombreUsuarioNormalizado, _callbackMock.Object),
                Times.Once);
            _notificadorAmigosMock.Verify(
                notificador => notificador.NotificarSolicitudesPendientesAlSuscribir(
                    NombreUsuarioNormalizado,
                    IdUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_Suscribir_ErrorBaseDatos_LanzaFaultException()
        {
            _operacionAmistadServicioMock
                .Setup(operacion => operacion.ObtenerDatosUsuarioSuscripcion(NombreUsuarioPrueba))
                .Throws(new EntityException());

            Assert.ThrowsException<FaultException>(
                () => _amigosManejador.Suscribir(NombreUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_CancelarSuscripcion_FlujoExitoso()
        {
            _amigosManejador.CancelarSuscripcion(NombreUsuarioPrueba);

            _manejadorCallbackMock.Verify(
                manejador => manejador.Desuscribir(NombreUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CancelarSuscripcion_NombreVacio_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(
                () => _amigosManejador.CancelarSuscripcion(string.Empty));
        }

        [TestMethod]
        public void Prueba_EnviarSolicitudAmistad_FlujoExitoso()
        {
            var resultadoCreacion = new ResultadoCreacionSolicitud
            {
                Emisor = new Usuario { Nombre_Usuario = NombreUsuarioPrueba },
                Receptor = new Usuario { Nombre_Usuario = NombreAmigoPrueba }
            };

            _operacionAmistadServicioMock
                .Setup(operacion => operacion.EjecutarCreacionSolicitud(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba))
                .Returns(resultadoCreacion);

            _amigosManejador.EnviarSolicitudAmistad(NombreUsuarioPrueba, NombreAmigoPrueba);

            _notificadorAmigosMock.Verify(
                notificador => notificador.NotificarSolicitudActualizada(
                    It.IsAny<string>(),
                    It.IsAny<SolicitudAmistadDTO>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EnviarSolicitudAmistad_UsuarioNoEncontrado_LanzaFaultException()
        {
            _operacionAmistadServicioMock
                .Setup(operacion => operacion.EjecutarCreacionSolicitud(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba))
                .Throws(new FaultException("Usuario no encontrado"));

            Assert.ThrowsException<FaultException>(
                () => _amigosManejador.EnviarSolicitudAmistad(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba));
        }

        [TestMethod]
        public void Prueba_EnviarSolicitudAmistad_RelacionExistente_LanzaFaultException()
        {
            _operacionAmistadServicioMock
                .Setup(operacion => operacion.EjecutarCreacionSolicitud(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba))
                .Throws(new InvalidOperationException("La relacion ya existe"));

            Assert.ThrowsException<FaultException>(
                () => _amigosManejador.EnviarSolicitudAmistad(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba));
        }

        [TestMethod]
        public void Prueba_ResponderSolicitudAmistad_FlujoExitoso()
        {
            var resultadoAceptacion = new ResultadoAceptacionSolicitud
            {
                NombreNormalizadoEmisor = NombreUsuarioNormalizado,
                NombreNormalizadoReceptor = NombreAmigoNormalizado
            };

            _operacionAmistadServicioMock
                .Setup(operacion => operacion.EjecutarAceptacionSolicitud(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba))
                .Returns(resultadoAceptacion);

            _amigosManejador.ResponderSolicitudAmistad(NombreUsuarioPrueba, NombreAmigoPrueba);

            _notificadorAmigosMock.Verify(
                notificador => notificador.NotificarSolicitudActualizada(
                    NombreUsuarioNormalizado,
                    It.IsAny<SolicitudAmistadDTO>()),
                Times.Once);
            _notificadorListaAmigosMock.Verify(
                notificador => notificador.NotificarCambioAmistad(NombreUsuarioNormalizado),
                Times.Once);
            _notificadorListaAmigosMock.Verify(
                notificador => notificador.NotificarCambioAmistad(NombreAmigoNormalizado),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ResponderSolicitudAmistad_SolicitudNoExiste_LanzaFaultException()
        {
            _operacionAmistadServicioMock
                .Setup(operacion => operacion.EjecutarAceptacionSolicitud(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba))
                .Throws(new InvalidOperationException("Solicitud no existe"));

            Assert.ThrowsException<FaultException>(
                () => _amigosManejador.ResponderSolicitudAmistad(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarAmigo_FlujoExitoso()
        {
            var resultadoEliminacion = new ResultadoEliminacionAmistad
            {
                Relacion = new Amigo(),
                NombrePrimerUsuarioNormalizado = NombreUsuarioNormalizado,
                NombreSegundoUsuarioNormalizado = NombreAmigoNormalizado
            };

            _operacionAmistadServicioMock
                .Setup(operacion => operacion.EjecutarEliminacion(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba))
                .Returns(resultadoEliminacion);

            _amigosManejador.EliminarAmigo(NombreUsuarioPrueba, NombreAmigoPrueba);

            _notificadorAmigosMock.Verify(
                notificador => notificador.NotificarAmistadEliminada(
                    NombreUsuarioNormalizado,
                    It.IsAny<SolicitudAmistadDTO>()),
                Times.Once);
            _notificadorListaAmigosMock.Verify(
                notificador => notificador.NotificarCambioAmistad(NombreUsuarioNormalizado),
                Times.Once);
            _notificadorListaAmigosMock.Verify(
                notificador => notificador.NotificarCambioAmistad(NombreAmigoNormalizado),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EliminarAmigo_RelacionNoExiste_LanzaFaultException()
        {
            _operacionAmistadServicioMock
                .Setup(operacion => operacion.EjecutarEliminacion(
                    NombreUsuarioPrueba,
                    NombreAmigoPrueba))
                .Throws(new InvalidOperationException("Relacion no existe"));

            Assert.ThrowsException<FaultException>(
                () => _amigosManejador.EliminarAmigo(NombreUsuarioPrueba, NombreAmigoPrueba));
        }
    }
}