using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class AmigosManejadorPruebas
    {
        private const string NombreUsuarioEmisor = "EmisorPrueba";
        private const string NombreUsuarioReceptor = "ReceptorPrueba";
        private const string NombreUsuarioVacio = "";
        private const string NombreUsuarioSoloEspacios = "   ";
        private const int IdUsuarioEmisor = 1;
        private const int IdUsuarioReceptor = 2;

        private Mock<IOperacionAmistadServicio> _mockOperacionAmistad;
        private Mock<INotificadorListaAmigos> _mockNotificadorLista;
        private Mock<INotificadorAmigos> _mockNotificadorAmigos;
        private Mock<IManejadorCallback<IAmigosManejadorCallback>> _mockManejadorCallback;
        private Mock<IProveedorCallback<IAmigosManejadorCallback>> _mockProveedorCallback;
        private AmigosManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockOperacionAmistad = new Mock<IOperacionAmistadServicio>();
            _mockNotificadorLista = new Mock<INotificadorListaAmigos>();
            _mockNotificadorAmigos = new Mock<INotificadorAmigos>();
            _mockManejadorCallback = new Mock<IManejadorCallback<IAmigosManejadorCallback>>();
            _mockProveedorCallback = new Mock<IProveedorCallback<IAmigosManejadorCallback>>();

            _manejador = new AmigosManejador(
                _mockOperacionAmistad.Object,
                _mockNotificadorLista.Object,
                _mockNotificadorAmigos.Object,
                _mockManejadorCallback.Object,
                _mockProveedorCallback.Object);
        }
        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionOperacionAmistadNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AmigosManejador(
                    null,
                    _mockNotificadorLista.Object,
                    _mockNotificadorAmigos.Object,
                    _mockManejadorCallback.Object,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionNotificadorListaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AmigosManejador(
                    _mockOperacionAmistad.Object,
                    null,
                    _mockNotificadorAmigos.Object,
                    _mockManejadorCallback.Object,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionNotificadorAmigosNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AmigosManejador(
                    _mockOperacionAmistad.Object,
                    _mockNotificadorLista.Object,
                    null,
                    _mockManejadorCallback.Object,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionManejadorCallbackNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AmigosManejador(
                    _mockOperacionAmistad.Object,
                    _mockNotificadorLista.Object,
                    _mockNotificadorAmigos.Object,
                    null,
                    _mockProveedorCallback.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionProveedorCallbackNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AmigosManejador(
                    _mockOperacionAmistad.Object,
                    _mockNotificadorLista.Object,
                    _mockNotificadorAmigos.Object,
                    _mockManejadorCallback.Object,
                    null));
        }        [TestMethod]
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
        public void Prueba_Suscribir_LlamaObtenerDatosUsuario()
        {
            ConfigurarMocksSuscripcion();

            _manejador.Suscribir(NombreUsuarioEmisor);

            _mockOperacionAmistad.Verify(
                o => o.ObtenerDatosUsuarioSuscripcion(NombreUsuarioEmisor),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_Suscribir_LlamaNotificarSolicitudesPendientes()
        {
            ConfigurarMocksSuscripcion();

            _manejador.Suscribir(NombreUsuarioEmisor);

            _mockNotificadorAmigos.Verify(
                n => n.NotificarSolicitudesPendientesAlSuscribir(
                    NombreUsuarioEmisor, 
                    IdUsuarioEmisor),
                Times.Once);
        }        [TestMethod]
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
            _manejador.CancelarSuscripcion(NombreUsuarioEmisor);

            _mockManejadorCallback.Verify(
                m => m.Desuscribir(NombreUsuarioEmisor),
                Times.Once);
        }        [TestMethod]
        public void Prueba_EnviarSolicitudAmistad_LanzaExcepcionEmisorNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.EnviarSolicitudAmistad(null, NombreUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_EnviarSolicitudAmistad_LanzaExcepcionReceptorNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.EnviarSolicitudAmistad(NombreUsuarioEmisor, null));
        }

        [TestMethod]
        public void Prueba_EnviarSolicitudAmistad_LanzaExcepcionMismoUsuario()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.EnviarSolicitudAmistad(
                    NombreUsuarioEmisor, 
                    NombreUsuarioEmisor));
        }

        [TestMethod]
        public void Prueba_EnviarSolicitudAmistad_LlamaCrearSolicitud()
        {
            ConfigurarMocksEnviarSolicitud();

            _manejador.EnviarSolicitudAmistad(NombreUsuarioEmisor, NombreUsuarioReceptor);

            _mockOperacionAmistad.Verify(
                o => o.EjecutarCreacionSolicitud(
                    NombreUsuarioEmisor, 
                    NombreUsuarioReceptor),
                Times.Once);
        }        [TestMethod]
        public void Prueba_ResponderSolicitudAmistad_LanzaExcepcionEmisorNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.ResponderSolicitudAmistad(null, NombreUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_ResponderSolicitudAmistad_LanzaExcepcionReceptorNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.ResponderSolicitudAmistad(NombreUsuarioEmisor, null));
        }

        [TestMethod]
        public void Prueba_ResponderSolicitudAmistad_LlamaAceptarSolicitud()
        {
            ConfigurarMocksResponderSolicitud();

            _manejador.ResponderSolicitudAmistad(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            _mockOperacionAmistad.Verify(
                o => o.EjecutarAceptacionSolicitud(
                    NombreUsuarioEmisor, 
                    NombreUsuarioReceptor),
                Times.Once);
        }        [TestMethod]
        public void Prueba_EliminarAmigo_LanzaExcepcionUsuarioANulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.EliminarAmigo(null, NombreUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_EliminarAmigo_LanzaExcepcionUsuarioBNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => _manejador.EliminarAmigo(NombreUsuarioEmisor, null));
        }

        [TestMethod]
        public void Prueba_EliminarAmigo_LlamaEliminarAmistad()
        {
            ConfigurarMocksEliminarAmigo();

            _manejador.EliminarAmigo(NombreUsuarioEmisor, NombreUsuarioReceptor);

            _mockOperacionAmistad.Verify(
                o => o.EjecutarEliminacion(NombreUsuarioEmisor, NombreUsuarioReceptor),
                Times.Once);
        }        private void ConfigurarMocksSuscripcion()
        {
            var datosUsuario = new DatosSuscripcionUsuario
            {
                IdUsuario = IdUsuarioEmisor,
                NombreNormalizado = NombreUsuarioEmisor
            };
            _mockOperacionAmistad
                .Setup(o => o.ObtenerDatosUsuarioSuscripcion(NombreUsuarioEmisor))
                .Returns(datosUsuario);
            _mockProveedorCallback
                .Setup(p => p.ObtenerCallbackActual())
                .Returns(Mock.Of<IAmigosManejadorCallback>());
        }

        private void ConfigurarMocksEnviarSolicitud()
        {
            var resultado = new ResultadoCreacionSolicitud
            {
                Emisor = new global::Datos.Modelo.Usuario 
                { 
                    idUsuario = IdUsuarioEmisor, 
                    Nombre_Usuario = NombreUsuarioEmisor 
                },
                Receptor = new global::Datos.Modelo.Usuario 
                { 
                    idUsuario = IdUsuarioReceptor, 
                    Nombre_Usuario = NombreUsuarioReceptor 
                }
            };
            _mockOperacionAmistad
                .Setup(o => o.EjecutarCreacionSolicitud(
                    NombreUsuarioEmisor, 
                    NombreUsuarioReceptor))
                .Returns(resultado);
        }

        private void ConfigurarMocksResponderSolicitud()
        {
            var resultado = new ResultadoOperacionAmistad
            {
                NombrePrimerUsuario = NombreUsuarioEmisor,
                NombreSegundoUsuario = NombreUsuarioReceptor
            };
            _mockOperacionAmistad
                .Setup(o => o.EjecutarAceptacionSolicitud(
                    NombreUsuarioEmisor, 
                    NombreUsuarioReceptor))
                .Returns(resultado);
        }

        private void ConfigurarMocksEliminarAmigo()
        {
            var resultado = new ResultadoEliminacionAmistad
            {
                Relacion = new global::Datos.Modelo.Amigo 
                { 
                    UsuarioEmisor = IdUsuarioEmisor, 
                    UsuarioReceptor = IdUsuarioReceptor 
                },
                NombrePrimerUsuario = NombreUsuarioEmisor,
                NombreSegundoUsuario = NombreUsuarioReceptor
            };
            _mockOperacionAmistad
                .Setup(o => o.EjecutarEliminacion(
                    NombreUsuarioEmisor, 
                    NombreUsuarioReceptor))
                .Returns(resultado);
        }    }
}
