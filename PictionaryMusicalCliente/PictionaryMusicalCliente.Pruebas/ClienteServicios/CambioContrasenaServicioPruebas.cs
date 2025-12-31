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
    /// Contiene las pruebas unitarias para la clase CambioContrasenaServicio.
    /// Verifica el comportamiento del servicio de cambio y recuperacion de contrasena.
    /// </summary>
    [TestClass]
    public class CambioContrasenaServicioPruebas
    {
        private Mock<IWcfClienteEjecutor> _ejecutorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<ILocalizadorServicio> _localizadorMock;
        private Mock<PictionaryServidorServicioCodigoVerificacion.ICodigoVerificacionManejador>
            _clienteVerificacionMock;
        private Mock<PictionaryServidorServicioCambioContrasena.ICambioContrasenaManejador>
            _clienteCambioContrasenaMock;
        private CambioContrasenaServicio _servicio;

        /// <summary>
        /// Inicializa los mocks y el servicio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _ejecutorMock = new Mock<IWcfClienteEjecutor>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _localizadorMock = new Mock<ILocalizadorServicio>();
            _clienteVerificacionMock =
                new Mock<PictionaryServidorServicioCodigoVerificacion.ICodigoVerificacionManejador>();
            _clienteCambioContrasenaMock =
                new Mock<PictionaryServidorServicioCambioContrasena.ICambioContrasenaManejador>();

            _fabricaClientesMock
                .Setup(f => f.CrearClienteVerificacion())
                .Returns(_clienteVerificacionMock.Object);

            _fabricaClientesMock
                .Setup(f => f.CrearClienteCambioContrasena())
                .Returns(_clienteCambioContrasenaMock.Object);

            _localizadorMock
                .Setup(l => l.Localizar(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string mensaje, string predeterminado) => 
                    string.IsNullOrWhiteSpace(mensaje) ? predeterminado : mensaje);

            _servicio = new CambioContrasenaServicio(
                _ejecutorMock.Object,
                _fabricaClientesMock.Object,
                _manejadorErrorMock.Object,
                _localizadorMock.Object);
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
                var servicio = new CambioContrasenaServicio(
                    null,
                    _fabricaClientesMock.Object,
                    _manejadorErrorMock.Object,
                    _localizadorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_FabricaNula_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new CambioContrasenaServicio(
                    _ejecutorMock.Object,
                    null,
                    _manejadorErrorMock.Object,
                    _localizadorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ManejadorErrorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new CambioContrasenaServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    null,
                    _localizadorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_LocalizadorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new CambioContrasenaServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    _manejadorErrorMock.Object,
                    null);
            });
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRecuperacionAsync_Exitoso_RetornaResultado()
        {
            string identificador = "usuario@ejemplo.com";
            var resultadoEsperado = new DTOs.ResultadoSolicitudRecuperacionDTO
            {
                CuentaEncontrada = true,
                CodigoEnviado = true,
                TokenCodigo = "token123",
                CorreoDestino = "usuario@ejemplo.com"
            };

            ConfigurarEjecutorSolicitarRecuperacionExitoso(resultadoEsperado);

            var resultado = await _servicio.SolicitarCodigoRecuperacionAsync(identificador);

            Assert.IsTrue(resultado.CuentaEncontrada);
            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRecuperacionAsync_CuentaNoEncontrada_RetornaFallo()
        {
            string identificador = "noexiste@ejemplo.com";
            var resultadoEsperado = new DTOs.ResultadoSolicitudRecuperacionDTO
            {
                CuentaEncontrada = false,
                CodigoEnviado = false,
                Mensaje = "Cuenta no encontrada"
            };

            ConfigurarEjecutorSolicitarRecuperacionExitoso(resultadoEsperado);

            var resultado = await _servicio.SolicitarCodigoRecuperacionAsync(identificador);

            Assert.IsFalse(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRecuperacionAsync_ResultadoNulo_RetornaVacio()
        {
            string identificador = "usuario@ejemplo.com";

            ConfigurarEjecutorSolicitarRecuperacionExitoso(null);

            var resultado = await _servicio.SolicitarCodigoRecuperacionAsync(identificador);

            Assert.IsFalse(resultado.CuentaEncontrada);
            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRecuperacionAsync_IdentificadorNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.SolicitarCodigoRecuperacionAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRecuperacionAsync_IdentificadorVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.SolicitarCodigoRecuperacionAsync("");
            });
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRecuperacionAsync_FaultException_LanzaExcepcion()
        {
            string identificador = "usuario@ejemplo.com";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorSolicitarRecuperacionConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.SolicitarCodigoRecuperacionAsync(identificador);
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRecuperacionAsync_Exitoso_RetornaResultado()
        {
            string tokenCodigo = "token123";
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true,
                TokenCodigo = "nuevoToken456"
            };

            ConfigurarEjecutorReenviarRecuperacionExitoso(resultadoEsperado);

            var resultado = await _servicio.ReenviarCodigoRecuperacionAsync(tokenCodigo);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRecuperacionAsync_ReenvioFallido_RetornaFallo()
        {
            string tokenCodigo = "tokenExpirado";
            var resultadoEsperado = new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                Mensaje = "Token expirado"
            };

            ConfigurarEjecutorReenviarRecuperacionExitoso(resultadoEsperado);

            var resultado = await _servicio.ReenviarCodigoRecuperacionAsync(tokenCodigo);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRecuperacionAsync_ResultadoNulo_RetornaVacio()
        {
            string tokenCodigo = "token123";

            ConfigurarEjecutorReenviarRecuperacionExitoso(null);

            var resultado = await _servicio.ReenviarCodigoRecuperacionAsync(tokenCodigo);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRecuperacionAsync_TokenNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ReenviarCodigoRecuperacionAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRecuperacionAsync_TokenVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ReenviarCodigoRecuperacionAsync("");
            });
        }

        [TestMethod]
        public async Task Prueba_ReenviarCodigoRecuperacionAsync_FaultException_LanzaExcepcion()
        {
            string tokenCodigo = "token123";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorReenviarRecuperacionConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ReenviarCodigoRecuperacionAsync(tokenCodigo);
            });
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRecuperacionAsync_Exitoso_RetornaOperacionExitosa()
        {
            string tokenCodigo = "token123";
            string codigoIngresado = "123456";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true,
                Mensaje = "Codigo verificado"
            };

            ConfigurarEjecutorConfirmarRecuperacionExitoso(resultadoEsperado);

            var resultado = await _servicio.ConfirmarCodigoRecuperacionAsync(
                tokenCodigo,
                codigoIngresado);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRecuperacionAsync_CodigoIncorrecto_RetornaFallo()
        {
            string tokenCodigo = "token123";
            string codigoIngresado = "000000";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = "Codigo incorrecto"
            };

            ConfigurarEjecutorConfirmarRecuperacionExitoso(resultadoEsperado);

            var resultado = await _servicio.ConfirmarCodigoRecuperacionAsync(
                tokenCodigo,
                codigoIngresado);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRecuperacionAsync_ResultadoNulo_RetornaVacio()
        {
            string tokenCodigo = "token123";
            string codigoIngresado = "123456";

            ConfigurarEjecutorConfirmarRecuperacionExitoso(null);

            var resultado = await _servicio.ConfirmarCodigoRecuperacionAsync(
                tokenCodigo,
                codigoIngresado);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRecuperacionAsync_TokenNulo_LanzaExcepcion()
        {
            string codigoIngresado = "123456";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ConfirmarCodigoRecuperacionAsync(null, codigoIngresado);
            });
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRecuperacionAsync_CodigoNulo_LanzaExcepcion()
        {
            string tokenCodigo = "token123";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ConfirmarCodigoRecuperacionAsync(tokenCodigo, null);
            });
        }

        [TestMethod]
        public async Task Prueba_ConfirmarCodigoRecuperacionAsync_FaultException_LanzaExcepcion()
        {
            string tokenCodigo = "token123";
            string codigoIngresado = "123456";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConfirmarRecuperacionConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ConfirmarCodigoRecuperacionAsync(tokenCodigo, codigoIngresado);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_Exitoso_RetornaOperacionExitosa()
        {
            string tokenCodigo = "tokenValidado123";
            string nuevaContrasena = "NuevaContrasena123!";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true,
                Mensaje = "Contrasena actualizada"
            };

            ConfigurarEjecutorActualizarContrasenaExitoso(resultadoEsperado);

            var resultado = await _servicio.ActualizarContrasenaAsync(
                tokenCodigo,
                nuevaContrasena);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_ActualizacionFallida_RetornaFallo()
        {
            string tokenCodigo = "tokenExpirado";
            string nuevaContrasena = "NuevaContrasena123!";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = "Token expirado"
            };

            ConfigurarEjecutorActualizarContrasenaExitoso(resultadoEsperado);

            var resultado = await _servicio.ActualizarContrasenaAsync(
                tokenCodigo,
                nuevaContrasena);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_ResultadoNulo_RetornaVacio()
        {
            string tokenCodigo = "tokenValidado123";
            string nuevaContrasena = "NuevaContrasena123!";

            ConfigurarEjecutorActualizarContrasenaExitoso(null);

            var resultado = await _servicio.ActualizarContrasenaAsync(
                tokenCodigo,
                nuevaContrasena);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_TokenNulo_LanzaExcepcion()
        {
            string nuevaContrasena = "NuevaContrasena123!";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ActualizarContrasenaAsync(null, nuevaContrasena);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_ContrasenaNula_LanzaExcepcion()
        {
            string tokenCodigo = "tokenValidado123";

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.ActualizarContrasenaAsync(tokenCodigo, null);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_FaultException_LanzaExcepcion()
        {
            string tokenCodigo = "tokenValidado123";
            string nuevaContrasena = "NuevaContrasena123!";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorActualizarContrasenaConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ActualizarContrasenaAsync(tokenCodigo, nuevaContrasena);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_FaultException_TipoFallaServicio()
        {
            string tokenCodigo = "tokenValidado123";
            string nuevaContrasena = "NuevaContrasena123!";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorActualizarContrasenaConExcepcion(faultException);

            try
            {
                await _servicio.ActualizarContrasenaAsync(tokenCodigo, nuevaContrasena);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_CommunicationException_LanzaExcepcion()
        {
            string tokenCodigo = "tokenValidado123";
            string nuevaContrasena = "NuevaContrasena123!";
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorActualizarContrasenaConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ActualizarContrasenaAsync(tokenCodigo, nuevaContrasena);
            });
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_TimeoutException_LanzaExcepcion()
        {
            string tokenCodigo = "tokenValidado123";
            string nuevaContrasena = "NuevaContrasena123!";
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorActualizarContrasenaConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ActualizarContrasenaAsync(tokenCodigo, nuevaContrasena);
            });
        }

        [TestMethod]
        public async Task Prueba_SolicitarCodigoRecuperacionAsync_InvocaFabricaClientes()
        {
            string identificador = "usuario@ejemplo.com";
            var resultadoEsperado = new DTOs.ResultadoSolicitudRecuperacionDTO
            {
                CuentaEncontrada = true,
                CodigoEnviado = true
            };

            ConfigurarEjecutorSolicitarRecuperacionExitoso(resultadoEsperado);

            await _servicio.SolicitarCodigoRecuperacionAsync(identificador);

            _fabricaClientesMock.Verify(f => f.CrearClienteVerificacion(), Times.Once);
        }

        [TestMethod]
        public async Task Prueba_ActualizarContrasenaAsync_InvocaFabricaClientes()
        {
            string tokenCodigo = "tokenValidado123";
            string nuevaContrasena = "NuevaContrasena123!";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };

            ConfigurarEjecutorActualizarContrasenaExitoso(resultadoEsperado);

            await _servicio.ActualizarContrasenaAsync(tokenCodigo, nuevaContrasena);

            _fabricaClientesMock.Verify(f => f.CrearClienteCambioContrasena(), Times.Once);
        }

        private void ConfigurarEjecutorSolicitarRecuperacionExitoso(
            DTOs.ResultadoSolicitudRecuperacionDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador,
                        Task<DTOs.ResultadoSolicitudRecuperacionDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorSolicitarRecuperacionConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador,
                        Task<DTOs.ResultadoSolicitudRecuperacionDTO>>>()))
                .ThrowsAsync(excepcion);
        }

        private void ConfigurarEjecutorReenviarRecuperacionExitoso(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCambioContrasena
                        .ICambioContrasenaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCambioContrasena
                        .ICambioContrasenaManejador,
                        Task<DTOs.ResultadoSolicitudCodigoDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorReenviarRecuperacionConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCambioContrasena
                        .ICambioContrasenaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCambioContrasena
                        .ICambioContrasenaManejador,
                        Task<DTOs.ResultadoSolicitudCodigoDTO>>>()))
                .ThrowsAsync(excepcion);
        }

        private void ConfigurarEjecutorConfirmarRecuperacionExitoso(
            DTOs.ResultadoOperacionDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorConfirmarRecuperacionConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCodigoVerificacion
                        .ICodigoVerificacionManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ThrowsAsync(excepcion);
        }

        private void ConfigurarEjecutorActualizarContrasenaExitoso(
            DTOs.ResultadoOperacionDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCambioContrasena
                        .ICambioContrasenaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCambioContrasena
                        .ICambioContrasenaManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorActualizarContrasenaConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCambioContrasena
                        .ICambioContrasenaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCambioContrasena
                        .ICambioContrasenaManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ThrowsAsync(excepcion);
        }
    }
}
