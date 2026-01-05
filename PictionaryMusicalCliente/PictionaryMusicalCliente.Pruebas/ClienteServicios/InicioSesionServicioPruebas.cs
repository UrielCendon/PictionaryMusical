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
    [TestClass]
    public class InicioSesionServicioPruebas
    {
        private Mock<IWcfClienteEjecutor> _ejecutorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<IUsuarioMapeador> _usuarioMapeadorMock;
        private Mock<ILocalizadorServicio> _localizadorMock;
        private Mock<PictionaryServidorServicioInicioSesion.IInicioSesionManejador> 
            _clienteInicioSesionMock;
        private InicioSesionServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _ejecutorMock = new Mock<IWcfClienteEjecutor>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _usuarioMapeadorMock = new Mock<IUsuarioMapeador>();
            _localizadorMock = new Mock<ILocalizadorServicio>();
            _clienteInicioSesionMock = 
                new Mock<PictionaryServidorServicioInicioSesion.IInicioSesionManejador>();

            _fabricaClientesMock
                .Setup(f => f.CrearClienteInicioSesion())
                .Returns(_clienteInicioSesionMock.Object);

            _localizadorMock
                .Setup(l => l.Localizar(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string mensaje, string def) => mensaje ?? def);

            _servicio = new InicioSesionServicio(
                _ejecutorMock.Object,
                _fabricaClientesMock.Object,
                _manejadorErrorMock.Object,
                _usuarioMapeadorMock.Object,
                _localizadorMock.Object);
        }

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
                var servicio = new InicioSesionServicio(
                    null,
                    _fabricaClientesMock.Object,
                    _manejadorErrorMock.Object,
                    _usuarioMapeadorMock.Object,
                    _localizadorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_FabricaNula_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InicioSesionServicio(
                    _ejecutorMock.Object,
                    null,
                    _manejadorErrorMock.Object,
                    _usuarioMapeadorMock.Object,
                    _localizadorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ManejadorErrorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InicioSesionServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    null,
                    _usuarioMapeadorMock.Object,
                    _localizadorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_UsuarioMapeadorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InicioSesionServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    _manejadorErrorMock.Object,
                    null,
                    _localizadorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_LocalizadorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InicioSesionServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    _manejadorErrorMock.Object,
                    _usuarioMapeadorMock.Object,
                    null);
            });
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_CredencialesValidas_RetornaUsuario()
        {
            var solicitud = CrearCredencialesValidas();
            var usuarioEsperado = CrearUsuarioAutenticado();
            var resultadoEsperado = new DTOs.ResultadoInicioSesionDTO
            {
                Usuario = usuarioEsperado,
                Mensaje = "Inicio de sesion exitoso"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.IniciarSesionAsync(solicitud);

            Assert.AreEqual(usuarioEsperado.UsuarioId, resultado.Usuario.UsuarioId);
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_CredencialesValidas_ActualizaSesion()
        {
            var solicitud = CrearCredencialesValidas();
            var usuarioEsperado = CrearUsuarioAutenticado();
            var resultadoEsperado = new DTOs.ResultadoInicioSesionDTO
            {
                Usuario = usuarioEsperado,
                Mensaje = "Exito"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.IniciarSesionAsync(solicitud);

            _usuarioMapeadorMock.Verify(
                u => u.ActualizarSesion(usuarioEsperado), 
                Times.Once);
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_CredencialesValidas_LocalizaMensaje()
        {
            var solicitud = CrearCredencialesValidas();
            var resultadoEsperado = new DTOs.ResultadoInicioSesionDTO
            {
                Usuario = CrearUsuarioAutenticado(),
                Mensaje = "Mensaje del servidor"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.IniciarSesionAsync(solicitud);

            _localizadorMock.Verify(
                l => l.Localizar("Mensaje del servidor", "Mensaje del servidor"),
                Times.Once);
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_CredencialesInvalidas_RetornaSinUsuario()
        {
            var solicitud = CrearCredencialesValidas();
            var resultadoEsperado = new DTOs.ResultadoInicioSesionDTO
            {
                Usuario = null,
                Mensaje = "Credenciales incorrectas"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.IniciarSesionAsync(solicitud);

            Assert.IsNull(resultado.Usuario);
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_ResultadoNulo_ManejaCorrectamente()
        {
            var solicitud = CrearCredencialesValidas();

            ConfigurarEjecutorExitoso(null);

            var resultado = await _servicio.IniciarSesionAsync(solicitud);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_SolicitudNula_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.IniciarSesionAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_FaultException_LanzaServicioExcepcion()
        {
            var solicitud = CrearCredencialesValidas();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.IniciarSesionAsync(solicitud);
            });
        }

        //fix prueba duplicada, ya se verifica en Prueba_IniciarSesionAsync_FaultException_LanzaServicioExcepcion
        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_FaultException_TipoErrorFallaServicio()
        {
            var solicitud = CrearCredencialesValidas();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            try
            {
                await _servicio.IniciarSesionAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_CommunicationException_LanzaExcepcion()
        {
            var solicitud = CrearCredencialesValidas();
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.IniciarSesionAsync(solicitud);
            });
        }

        //fix prueba duplicada, ya se verifica en Prueba_IniciarSesionAsync_CommunicationException_LanzaExcepcion
        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_CommunicationException_TipoComunicacion()
        {
            var solicitud = CrearCredencialesValidas();
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            try
            {
                await _servicio.IniciarSesionAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.Comunicacion, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_TimeoutException_LanzaExcepcion()
        {
            var solicitud = CrearCredencialesValidas();
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.IniciarSesionAsync(solicitud);
            });
        }

        //fix prueba duplicada, ya se verifica en Prueba_IniciarSesionAsync_TimeoutException_LanzaExcepcion
        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_TimeoutException_TipoTiempoAgotado()
        {
            var solicitud = CrearCredencialesValidas();
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            try
            {
                await _servicio.IniciarSesionAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.TiempoAgotado, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_InvalidOperationException_LanzaExcepcion()
        {
            var solicitud = CrearCredencialesValidas();
            var invalidOperationException = 
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.IniciarSesionAsync(solicitud);
            });
        }

        //fix prueba duplicada, ya se verifica en Prueba_IniciarSesionAsync_InvalidOperationException_LanzaExcepcion
        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_InvalidOperationException_TipoInvalida()
        {
            var solicitud = CrearCredencialesValidas();
            var invalidOperationException = 
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            try
            {
                await _servicio.IniciarSesionAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.OperacionInvalida, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_IniciarSesionAsync_InvocaFabricaClientes()
        {
            var solicitud = CrearCredencialesValidas();
            var resultadoEsperado = new DTOs.ResultadoInicioSesionDTO
            {
                Usuario = CrearUsuarioAutenticado()
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.IniciarSesionAsync(solicitud);

            _fabricaClientesMock.Verify(f => f.CrearClienteInicioSesion(), Times.Once);
        }

        private static DTOs.CredencialesInicioSesionDTO CrearCredencialesValidas()
        {
            return new DTOs.CredencialesInicioSesionDTO
            {
                Identificador = "usuario@ejemplo.com",
                Contrasena = "contrasenaHasheada123"
            };
        }

        private static DTOs.UsuarioDTO CrearUsuarioAutenticado()
        {
            return new DTOs.UsuarioDTO
            {
                UsuarioId = 1,
                NombreUsuario = "UsuarioPrueba",
                Correo = "usuario@ejemplo.com"
            };
        }

        private void ConfigurarEjecutorExitoso(DTOs.ResultadoInicioSesionDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioInicioSesion.IInicioSesionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioInicioSesion.IInicioSesionManejador,
                        Task<DTOs.ResultadoInicioSesionDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioInicioSesion.IInicioSesionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioInicioSesion.IInicioSesionManejador,
                        Task<DTOs.ResultadoInicioSesionDTO>>>()))
                .ThrowsAsync(excepcion);
        }
    }
}
