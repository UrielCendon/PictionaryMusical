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
    /// Contiene las pruebas unitarias para la clase InvitacionesServicio.
    /// Verifica el comportamiento del servicio de envio de invitaciones por correo.
    /// </summary>
    [TestClass]
    public class InvitacionesServicioPruebas
    {
        private Mock<IWcfClienteEjecutor> _ejecutorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<PictionaryServidorServicioInvitaciones.IInvitacionesManejador>
            _clienteInvitacionesMock;
        private InvitacionesServicio _servicio;

        /// <summary>
        /// Inicializa los mocks y el servicio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _ejecutorMock = new Mock<IWcfClienteEjecutor>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _clienteInvitacionesMock =
                new Mock<PictionaryServidorServicioInvitaciones.IInvitacionesManejador>();

            _fabricaClientesMock
                .Setup(f => f.CrearClienteInvitaciones())
                .Returns(_clienteInvitacionesMock.Object);

            _servicio = new InvitacionesServicio(
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
                var servicio = new InvitacionesServicio(
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
                var servicio = new InvitacionesServicio(
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
                var servicio = new InvitacionesServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    null);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_DatosValidos_RetornaExito()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true,
                Mensaje = "Invitacion enviada"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_EnvioFallido_RetornaFallo()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = "Error al enviar invitacion"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_CodigoSalaNulo_LanzaExcepcion()
        {
            string correoDestino = "invitado@ejemplo.com";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.EnviarInvitacionAsync(null, correoDestino);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_CodigoSalaVacio_LanzaExcepcion()
        {
            string correoDestino = "invitado@ejemplo.com";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.EnviarInvitacionAsync("", correoDestino);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_CodigoSalaEspaciosBlanco_LanzaExcepcion()
        {
            string correoDestino = "invitado@ejemplo.com";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.EnviarInvitacionAsync("   ", correoDestino);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_CorreoNulo_LanzaExcepcion()
        {
            string codigoSala = "ABC123";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, null);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_CorreoVacio_LanzaExcepcion()
        {
            string codigoSala = "ABC123";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, "");
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_CorreoEspaciosBlanco_LanzaExcepcion()
        {
            string codigoSala = "ABC123";

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, "   ");
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_FaultException_LanzaExcepcion()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_FaultException_TipoFallaServicio()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            try
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_CommunicationException_LanzaExcepcion()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_CommunicationException_TipoComunicacion()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            try
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.Comunicacion, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_TimeoutException_LanzaExcepcion()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_TimeoutException_TipoTiempoAgotado()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            try
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.TiempoAgotado, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_InvalidOperationException_LanzaExcepcion()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var invalidOperationException =
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);
            });
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_InvalidOperationException_TipoInvalida()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var invalidOperationException =
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            try
            {
                await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.OperacionInvalida, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_InvocaFabricaClientes()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);

            _fabricaClientesMock.Verify(f => f.CrearClienteInvitaciones(), Times.Once);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_InvocaEjecutor()
        {
            string codigoSala = "ABC123";
            string correoDestino = "invitado@ejemplo.com";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.EnviarInvitacionAsync(codigoSala, correoDestino);

            _ejecutorMock.Verify(
                e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioInvitaciones.IInvitacionesManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioInvitaciones.IInvitacionesManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_TrimEspaciosCorreo()
        {
            string codigoSala = "ABC123";
            string correoConEspacios = "  invitado@ejemplo.com  ";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.EnviarInvitacionAsync(
                codigoSala, 
                correoConEspacios);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_TrimEspaciosCodigoSala()
        {
            string codigoSalaConEspacios = "  ABC123  ";
            string correoDestino = "invitado@ejemplo.com";
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.EnviarInvitacionAsync(
                codigoSalaConEspacios,
                correoDestino);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        private void ConfigurarEjecutorExitoso(DTOs.ResultadoOperacionDTO resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioInvitaciones.IInvitacionesManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioInvitaciones.IInvitacionesManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioInvitaciones.IInvitacionesManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioInvitaciones.IInvitacionesManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ThrowsAsync(excepcion);
        }
    }
}
