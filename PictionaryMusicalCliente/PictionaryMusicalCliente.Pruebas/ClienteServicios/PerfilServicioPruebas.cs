using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase PerfilServicio.
    /// Verifica el comportamiento del servicio de gestion de perfiles de usuario.
    /// </summary>
    [TestClass]
    public class PerfilServicioPruebas
    {
        private Mock<IWcfClienteEjecutor> _ejecutorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<PictionaryServidorServicioPerfil.IPerfilManejador> _clientePerfilMock;
        private PerfilServicio _servicio;

        /// <summary>
        /// Inicializa los mocks y el servicio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _ejecutorMock = new Mock<IWcfClienteEjecutor>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _clientePerfilMock =
                new Mock<PictionaryServidorServicioPerfil.IPerfilManejador>();

            _fabricaClientesMock
                .Setup(f => f.CrearClientePerfil())
                .Returns(_clientePerfilMock.Object);

            _servicio = new PerfilServicio(
                _ejecutorMock.Object,
                _fabricaClientesMock.Object,
                _manejadorErrorMock.Object);
        }

        /// <summary>
        /// Limpia los recursos despues de cada prueba.
        /// </summary>
        [TestCleanup]
        public void Limpiar()
        {
            _servicio = null;
        }

        [TestMethod]
        public void Prueba_Constructor_EjecutorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new PerfilServicio(
                    null,
                    _fabricaClientesMock.Object,
                    _manejadorErrorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_FabricaNula_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new PerfilServicio(
                    _ejecutorMock.Object,
                    null,
                    _manejadorErrorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ManejadorErrorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new PerfilServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    null);
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_UsuarioExistente_RetornaPerfil()
        {
            int usuarioId = 1;
            var perfilEsperado = CrearPerfilValido(usuarioId);

            ConfigurarEjecutorObtenerPerfilExitoso(perfilEsperado);

            var resultado = await _servicio.ObtenerPerfilAsync(usuarioId);

            Assert.AreEqual(usuarioId, resultado.UsuarioId);
            Assert.AreEqual("UsuarioPrueba", resultado.NombreUsuario);
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_UsuarioExistente_DatosCompletos()
        {
            int usuarioId = 1;
            var perfilEsperado = CrearPerfilValido(usuarioId);

            ConfigurarEjecutorObtenerPerfilExitoso(perfilEsperado);

            var resultado = await _servicio.ObtenerPerfilAsync(usuarioId);

            Assert.AreEqual("usuario@ejemplo.com", resultado.Correo);
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_UsuarioNoExiste_RetornaNulo()
        {
            int usuarioId = 999;

            ConfigurarEjecutorObtenerPerfilExitoso(null);

            var resultado = await _servicio.ObtenerPerfilAsync(usuarioId);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_FaultException_LanzaExcepcion()
        {
            int usuarioId = 1;
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorObtenerPerfilConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ObtenerPerfilAsync(usuarioId);
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_FaultException_TipoFallaServicio()
        {
            int usuarioId = 1;
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorObtenerPerfilConExcepcion(faultException);

            try
            {
                await _servicio.ObtenerPerfilAsync(usuarioId);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_CommunicationException_LanzaExcepcion()
        {
            int usuarioId = 1;
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorObtenerPerfilConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ObtenerPerfilAsync(usuarioId);
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_CommunicationException_TipoComunicacion()
        {
            int usuarioId = 1;
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorObtenerPerfilConExcepcion(communicationException);

            try
            {
                await _servicio.ObtenerPerfilAsync(usuarioId);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.Comunicacion, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_TimeoutException_LanzaExcepcion()
        {
            int usuarioId = 1;
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorObtenerPerfilConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ObtenerPerfilAsync(usuarioId);
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_TimeoutException_TipoTiempoAgotado()
        {
            int usuarioId = 1;
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorObtenerPerfilConExcepcion(timeoutException);

            try
            {
                await _servicio.ObtenerPerfilAsync(usuarioId);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.TiempoAgotado, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_InvalidOperationException_LanzaExcepcion()
        {
            int usuarioId = 1;
            var invalidOperationException =
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorObtenerPerfilConExcepcion(invalidOperationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ObtenerPerfilAsync(usuarioId);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_SolicitudValida_RetornaExito()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true,
                Mensaje = "Perfil actualizado"
            };

            ConfigurarEjecutorActualizarPerfilExitoso(resultadoEsperado);

            var resultado = await _servicio.ActualizarPerfilAsync(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_ActualizacionFallida_RetornaFallo()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = "Error al actualizar"
            };

            ConfigurarEjecutorActualizarPerfilExitoso(resultadoEsperado);

            var resultado = await _servicio.ActualizarPerfilAsync(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_ResultadoNulo_RetornaFalso()
        {
            var solicitud = CrearSolicitudActualizacionValida();

            ConfigurarEjecutorActualizarPerfilExitoso(null);

            var resultado = await _servicio.ActualizarPerfilAsync(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_SolicitudNula_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.ActualizarPerfilAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_FaultException_LanzaExcepcion()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorActualizarPerfilConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ActualizarPerfilAsync(solicitud);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_FaultException_TipoFallaServicio()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorActualizarPerfilConExcepcion(faultException);

            try
            {
                await _servicio.ActualizarPerfilAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_CommunicationException_LanzaExcepcion()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorActualizarPerfilConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ActualizarPerfilAsync(solicitud);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_TimeoutException_LanzaExcepcion()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorActualizarPerfilConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ActualizarPerfilAsync(solicitud);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_InvalidOperationException_LanzaExcepcion()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            var invalidOperationException =
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorActualizarPerfilConExcepcion(invalidOperationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ActualizarPerfilAsync(solicitud);
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerPerfilAsync_InvocaFabricaClientes()
        {
            int usuarioId = 1;
            var perfilEsperado = CrearPerfilValido(usuarioId);

            ConfigurarEjecutorObtenerPerfilExitoso(perfilEsperado);

            await _servicio.ObtenerPerfilAsync(usuarioId);

            _fabricaClientesMock.Verify(f => f.CrearClientePerfil(), Times.Once);
        }

        [TestMethod]
        public async Task Prueba_ActualizarPerfilAsync_InvocaFabricaClientes()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };

            ConfigurarEjecutorActualizarPerfilExitoso(resultadoEsperado);

            await _servicio.ActualizarPerfilAsync(solicitud);

            _fabricaClientesMock.Verify(f => f.CrearClientePerfil(), Times.Once);
        }

        private static DTOs.UsuarioDTO CrearPerfilValido(int usuarioId)
        {
            return new DTOs.UsuarioDTO
            {
                UsuarioId = usuarioId,
                NombreUsuario = "UsuarioPrueba",
                Correo = "usuario@ejemplo.com"
            };
        }

        private static DTOs.ActualizacionPerfilDTO CrearSolicitudActualizacionValida()
        {
            return new DTOs.ActualizacionPerfilDTO
            {
                UsuarioId = 1,
                Nombre = "NuevoNombre"
            };
        }

        private void ConfigurarEjecutorObtenerPerfilExitoso(DTOs.UsuarioDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioPerfil.IPerfilManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioPerfil.IPerfilManejador,
                        Task<DTOs.UsuarioDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorObtenerPerfilConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioPerfil.IPerfilManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioPerfil.IPerfilManejador,
                        Task<DTOs.UsuarioDTO>>>()))
                .ThrowsAsync(excepcion);
        }

        private void ConfigurarEjecutorActualizarPerfilExitoso(
            DTOs.ResultadoOperacionDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioPerfil.IPerfilManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioPerfil.IPerfilManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorActualizarPerfilConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioPerfil.IPerfilManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioPerfil.IPerfilManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ThrowsAsync(excepcion);
        }
    }
}
