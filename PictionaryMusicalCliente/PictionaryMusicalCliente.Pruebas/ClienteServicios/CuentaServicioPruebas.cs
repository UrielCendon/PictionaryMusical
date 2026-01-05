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
    public class CuentaServicioPruebas
    {
        private Mock<IWcfClienteEjecutor> _ejecutorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<PictionaryServidorServicioCuenta.ICuentaManejador> _clienteCuentaMock;
        private CuentaServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _ejecutorMock = new Mock<IWcfClienteEjecutor>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _clienteCuentaMock = 
                new Mock<PictionaryServidorServicioCuenta.ICuentaManejador>();

            _fabricaClientesMock
                .Setup(f => f.CrearClienteCuenta())
                .Returns(_clienteCuentaMock.Object);

            _servicio = new CuentaServicio(
                _ejecutorMock.Object,
                _fabricaClientesMock.Object,
                _manejadorErrorMock.Object);
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
                var servicio = new CuentaServicio(
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
                var servicio = new CuentaServicio(
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
                var servicio = new CuentaServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    null);
            });
        }

        //fix múltiples asserts
        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_SolicitudValida_RetornaExito()
        {
            var solicitud = CrearSolicitudValida();
            var resultadoEsperado = new DTOs.ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = true,
                Mensaje = "Cuenta registrada exitosamente"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.RegistrarCuentaAsync(solicitud);

            Assert.IsTrue(resultado.RegistroExitoso);
            Assert.AreEqual("Cuenta registrada exitosamente", resultado.Mensaje);
        }

        //fix múltiples asserts
        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_CorreoDuplicado_RetornaFallo()
        {
            var solicitud = CrearSolicitudValida();
            var resultadoEsperado = new DTOs.ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = false,
                Mensaje = "El correo ya esta registrado"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.RegistrarCuentaAsync(solicitud);

            Assert.IsFalse(resultado.RegistroExitoso);
            Assert.AreEqual("El correo ya esta registrado", resultado.Mensaje);
        }

        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_NombreUsuarioDuplicado_RetornaFallo()
        {
            var solicitud = CrearSolicitudValida();
            var resultadoEsperado = new DTOs.ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = false,
                Mensaje = "El nombre de usuario ya existe"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.RegistrarCuentaAsync(solicitud);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_SolicitudNula_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.RegistrarCuentaAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_FaultException_LanzaServicioExcepcion()
        {
            var solicitud = CrearSolicitudValida();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.RegistrarCuentaAsync(solicitud);
            });
        }

        //fix prueba duplicada, ya se verifica en Prueba_RegistrarCuentaAsync_FaultException_LanzaServicioExcepcion
        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_FaultException_TipoErrorFallaServicio()
        {
            var solicitud = CrearSolicitudValida();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            try
            {
                await _servicio.RegistrarCuentaAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_CommunicationException_LanzaExcepcion()
        {
            var solicitud = CrearSolicitudValida();
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.RegistrarCuentaAsync(solicitud);
            });
        }

        //fix prueba duplicada, ya se verifica en Prueba_RegistrarCuentaAsync_CommunicationException_LanzaExcepcion
        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_CommunicationException_TipoComunicacion()
        {
            var solicitud = CrearSolicitudValida();
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            try
            {
                await _servicio.RegistrarCuentaAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.Comunicacion, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_TimeoutException_LanzaExcepcion()
        {
            var solicitud = CrearSolicitudValida();
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.RegistrarCuentaAsync(solicitud);
            });
        }

        //fix prueba duplicada, ya se verifica en Prueba_RegistrarCuentaAsync_TimeoutException_LanzaExcepcion
        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_TimeoutException_TipoTiempoAgotado()
        {
            var solicitud = CrearSolicitudValida();
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            try
            {
                await _servicio.RegistrarCuentaAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.TiempoAgotado, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_InvalidOperationException_LanzaExcepcion()
        {
            var solicitud = CrearSolicitudValida();
            var invalidOperationException = 
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.RegistrarCuentaAsync(solicitud);
            });
        }

        //fix prueba duplicada, ya se verifica en Prueba_RegistrarCuentaAsync_InvalidOperationException_LanzaExcepcion
        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_InvalidOperationException_TipoInvalida()
        {
            var solicitud = CrearSolicitudValida();
            var invalidOperationException = 
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            try
            {
                await _servicio.RegistrarCuentaAsync(solicitud);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.OperacionInvalida, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_InvocaFabricaClientes()
        {
            var solicitud = CrearSolicitudValida();
            var resultadoEsperado = new DTOs.ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = true
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.RegistrarCuentaAsync(solicitud);

            _fabricaClientesMock.Verify(f => f.CrearClienteCuenta(), Times.Once);
        }

        [TestMethod]
        public async Task Prueba_RegistrarCuentaAsync_InvocaEjecutor()
        {
            var solicitud = CrearSolicitudValida();
            var resultadoEsperado = new DTOs.ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = true
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.RegistrarCuentaAsync(solicitud);

            _ejecutorMock.Verify(
                e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCuenta.ICuentaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCuenta.ICuentaManejador,
                        Task<DTOs.ResultadoRegistroCuentaDTO>>>()),
                Times.Once);
        }

        private static DTOs.NuevaCuentaDTO CrearSolicitudValida()
        {
            return new DTOs.NuevaCuentaDTO
            {
                Correo = "usuario@ejemplo.com",
                Usuario = "UsuarioPrueba",
                Contrasena = "Contrasena123!"
            };
        }

        private void ConfigurarEjecutorExitoso(DTOs.ResultadoRegistroCuentaDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCuenta.ICuentaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCuenta.ICuentaManejador,
                        Task<DTOs.ResultadoRegistroCuentaDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioCuenta.ICuentaManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioCuenta.ICuentaManejador,
                        Task<DTOs.ResultadoRegistroCuentaDTO>>>()))
                .ThrowsAsync(excepcion);
        }
    }
}
