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
    /// Contiene las pruebas unitarias para la clase VerificacionCodigoServicio.
    /// Verifica el comportamiento del servicio de verificacion de codigos de registro.
    /// </summary>
    [TestClass]
    public class VerificacionCodigoServicioPruebas
    {
        private Mock<IWcfClienteEjecutor> _ejecutorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<PictionaryServidorServicioCodigoVerificacion.ICodigoVerificacionManejador>
            _clienteVerificacionMock;
        private Mock<PictionaryServidorServicioCuenta.ICuentaManejador> _clienteCuentaMock;
        private VerificacionCodigoServicio _servicio;

        /// <summary>
        /// Inicializa los mocks y el servicio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _ejecutorMock = new Mock<IWcfClienteEjecutor>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _clienteVerificacionMock =
                new Mock<PictionaryServidorServicioCodigoVerificacion.ICodigoVerificacionManejador>();
            _clienteCuentaMock =
                new Mock<PictionaryServidorServicioCuenta.ICuentaManejador>();

            _fabricaClientesMock
                .Setup(f => f.CrearClienteVerificacion())
                .Returns(_clienteVerificacionMock.Object);

            _fabricaClientesMock
                .Setup(f => f.CrearClienteCuenta())
                .Returns(_clienteCuentaMock.Object);

            _servicio = new VerificacionCodigoServicio(
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
                var servicio = new VerificacionCodigoServicio(
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
                var servicio = new VerificacionCodigoServicio(
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
                var servicio = new VerificacionCodigoServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    null);
            });
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_SolicitudValida_RetornaExito()
        {
            var solicitud = CrearNuevaCuentaValida();
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true,
                TokenCodigo = "token123",
                Mensaje = "Codigo enviado"
            };

            ConfigurarEjecutorSolicitarCodigoExitoso(resultadoEsperado);

            var resultado = await _servicio.SolicitarCodigoRegistroAsync(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
            Assert.AreEqual("token123", resultado.TokenCodigo);
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_EnvioFallido_RetornaFallo()
        {
            var solicitud = CrearNuevaCuentaValida();
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                Mensaje = "Error al enviar codigo"
            };

            ConfigurarEjecutorSolicitarCodigoExitoso(resultadoEsperado);

            var resultado = await _servicio.SolicitarCodigoRegistroAsync(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_AsignaIdiomaDefault()
        {
            var solicitud = new DTOs.NuevaCuentaDTO
            {
                Correo = "usuario@ejemplo.com",
                Usuario = "Usuario",
                Contrasena = "Contrasena123!",
                Idioma = null
            };
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true
            };

            ConfigurarEjecutorSolicitarCodigoExitoso(resultadoEsperado);

            await _servicio.SolicitarCodigoRegistroAsync(solicitud);

            Assert.IsFalse(string.IsNullOrEmpty(solicitud.Idioma));
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_SolicitudNula_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.SolicitarCodigoRegistroAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_FaultException_LanzaExcepcion()
        {
            var solicitud = CrearNuevaCuentaValida();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorSolicitarCodigoConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.SolicitarCodigoRegistroAsync(solicitud);
            });
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_FaultException_TipoFallaServicio()
        {
            var solicitud = CrearNuevaCuentaValida();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorSolicitarCodigoConExcepcion(faultException);

            try
            {
                await _servicio.SolicitarCodigoRegistroAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_CommunicationException_LanzaExcepcion()
        {
            var solicitud = CrearNuevaCuentaValida();
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorSolicitarCodigoConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.SolicitarCodigoRegistroAsync(solicitud);
            });
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_TimeoutException_LanzaExcepcion()
        {
            var solicitud = CrearNuevaCuentaValida();
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorSolicitarCodigoConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.SolicitarCodigoRegistroAsync(solicitud);
            });
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRegistroAsync_CodigoValido_RetornaExito()
        {
            string tokenCodigo = "token123";
            string codigoIngresado = "123456";
            var resultadoEsperado = new DTOs.ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = true,
                Mensaje = "Cuenta creada"
            };

            ConfigurarEjecutorConfirmarCodigoExitoso(resultadoEsperado);

            var resultado = await _servicio.ConfirmarCodigoRegistroAsync(
                tokenCodigo,
                codigoIngresado);

            Assert.IsTrue(resultado.RegistroExitoso);
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRegistroAsync_CodigoInvalido_RetornaFallo()
        {
            string tokenCodigo = "token123";
            string codigoIngresado = "000000";
            var resultadoEsperado = new DTOs.ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = false,
                Mensaje = "Codigo incorrecto"
            };

            ConfigurarEjecutorConfirmarCodigoExitoso(resultadoEsperado);

            var resultado = await _servicio.ConfirmarCodigoRegistroAsync(
                tokenCodigo,
                codigoIngresado);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRegistroAsync_TokenNulo_LanzaExcepcion()
        {
            string codigoIngresado = "123456";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ConfirmarCodigoRegistroAsync(null, codigoIngresado);
            });
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRegistroAsync_TokenVacio_LanzaExcepcion()
        {
            string codigoIngresado = "123456";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ConfirmarCodigoRegistroAsync("", codigoIngresado);
            });
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRegistroAsync_TokenEspaciosBlanco_LanzaExcepcion()
        {
            string codigoIngresado = "123456";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ConfirmarCodigoRegistroAsync("   ", codigoIngresado);
            });
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRegistroAsync_FaultException_LanzaExcepcion()
        {
            string tokenCodigo = "token123";
            string codigoIngresado = "123456";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConfirmarCodigoConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ConfirmarCodigoRegistroAsync(tokenCodigo, codigoIngresado);
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_TokenValido_RetornaExito()
        {
            string tokenCodigo = "token123";
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true,
                TokenCodigo = "nuevoToken456",
                Mensaje = "Codigo reenviado"
            };

            ConfigurarEjecutorReenviarCodigoExitoso(resultadoEsperado);

            var resultado = await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_ReenvioFallido_RetornaFallo()
        {
            string tokenCodigo = "tokenExpirado";
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                Mensaje = "Token expirado"
            };

            ConfigurarEjecutorReenviarCodigoExitoso(resultadoEsperado);

            var resultado = await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_TokenNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ReenviarCodigoRegistroAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_TokenVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ReenviarCodigoRegistroAsync("");
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_TokenEspaciosBlanco_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ReenviarCodigoRegistroAsync("   ");
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_FaultException_LanzaExcepcion()
        {
            string tokenCodigo = "token123";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorReenviarCodigoConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_FaultException_TipoFallaServicio()
        {
            string tokenCodigo = "token123";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorReenviarCodigoConExcepcion(faultException);

            try
            {
                await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_CommunicationException_LanzaExcepcion()
        {
            string tokenCodigo = "token123";
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorReenviarCodigoConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_CommunicationException_TipoComunicacion()
        {
            string tokenCodigo = "token123";
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorReenviarCodigoConExcepcion(communicationException);

            try
            {
                await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.Comunicacion, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_TimeoutException_LanzaExcepcion()
        {
            string tokenCodigo = "token123";
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorReenviarCodigoConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_TimeoutException_TipoTiempoAgotado()
        {
            string tokenCodigo = "token123";
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorReenviarCodigoConExcepcion(timeoutException);

            try
            {
                await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.TiempoAgotado, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRegistroAsync_InvocaFabricaClientes()
        {
            var solicitud = CrearNuevaCuentaValida();
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true
            };

            ConfigurarEjecutorSolicitarCodigoExitoso(resultadoEsperado);

            await _servicio.SolicitarCodigoRegistroAsync(solicitud);

            _fabricaClientesMock.Verify(f => f.CrearClienteVerificacion(), Times.Once);
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRegistroAsync_InvocaFabricaClientes()
        {
            string tokenCodigo = "token123";
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true
            };

            ConfigurarEjecutorReenviarCodigoExitoso(resultadoEsperado);

            await _servicio.ReenviarCodigoRegistroAsync(tokenCodigo);

            _fabricaClientesMock.Verify(f => f.CrearClienteCuenta(), Times.Once);
        }

        private static DTOs.NuevaCuentaDTO CrearNuevaCuentaValida()
        {
            return new DTOs.NuevaCuentaDTO
            {
                Correo = "usuario@ejemplo.com",
                Usuario = "UsuarioPrueba",
                Contrasena = "Contrasena123!",
                Idioma = "es-MX"
            };
        }

        private void ConfigurarEjecutorSolicitarCodigoExitoso(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador,
                        Task<DTOs.ResultadoSolicitudCodigoDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorSolicitarCodigoConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador,
                        Task<DTOs.ResultadoSolicitudCodigoDTO>>>()))
                .ThrowsAsync(excepcion);
        }

        private void ConfigurarEjecutorConfirmarCodigoExitoso(
            DTOs.ResultadoRegistroCuentaDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador,
                        Task<DTOs.ResultadoRegistroCuentaDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorConfirmarCodigoConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador,
                        Task<DTOs.ResultadoRegistroCuentaDTO>>>()))
                .ThrowsAsync(excepcion);
        }

        private void ConfigurarEjecutorReenviarCodigoExitoso(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCuenta.ICuentaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCuenta.ICuentaManejador,
                        Task<DTOs.ResultadoSolicitudCodigoDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorReenviarCodigoConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCuenta.ICuentaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCuenta.ICuentaManejador,
                        Task<DTOs.ResultadoSolicitudCodigoDTO>>>()))
                .ThrowsAsync(excepcion);
        }
    }
}
