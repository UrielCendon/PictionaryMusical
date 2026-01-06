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
    public class ReportesServicioPruebas
    {
        private Mock<IWcfClienteEjecutor> _ejecutorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<ILocalizadorServicio> _localizadorMock;
        private Mock<PictionaryServidorServicioReportes.IReportesManejador> _clienteReportesMock;
        private ReportesServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _ejecutorMock = new Mock<IWcfClienteEjecutor>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _localizadorMock = new Mock<ILocalizadorServicio>();
            _clienteReportesMock =
                new Mock<PictionaryServidorServicioReportes.IReportesManejador>();

            _fabricaClientesMock
                .Setup(fabrica => fabrica.CrearClienteReportes())
                .Returns(_clienteReportesMock.Object);

            _localizadorMock
                .Setup(localizador => localizador.Localizar(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string mensaje, string def) => mensaje ?? def);

            _servicio = new ReportesServicio(
                _ejecutorMock.Object,
                _fabricaClientesMock.Object,
                _manejadorErrorMock.Object,
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
                var servicio = new ReportesServicio(
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
                var servicio = new ReportesServicio(
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
                var servicio = new ReportesServicio(
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
                var servicio = new ReportesServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    _manejadorErrorMock.Object,
                    null);
            });
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_ReporteValido_RetornaExito()
        {
            var reporte = CrearReporteValido();
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true,
                Mensaje = "Reporte enviado"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.ReportarJugadorAsync(reporte);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_ReporteRechazado_RetornaFallo()
        {
            var reporte = CrearReporteValido();
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = "No se pudo enviar el reporte"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            var resultado = await _servicio.ReportarJugadorAsync(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_LocalizaMensajeResultado()
        {
            var reporte = CrearReporteValido();
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true,
                Mensaje = "Mensaje servidor"
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.ReportarJugadorAsync(reporte);

            _localizadorMock.Verify(
                l => l.Localizar("Mensaje servidor", It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_ResultadoNulo_LocalizaMensajePredeterminado()
        {
            var reporte = CrearReporteValido();

            ConfigurarEjecutorExitoso(null);

            var resultado = await _servicio.ReportarJugadorAsync(reporte);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_ReporteNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.ReportarJugadorAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_FaultException_LanzaExcepcion()
        {
            var reporte = CrearReporteValido();
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(manejador => manejador.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ReportarJugadorAsync(reporte);
            });
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_CommunicationException_LanzaExcepcion()
        {
            var reporte = CrearReporteValido();
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ReportarJugadorAsync(reporte);
            });
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_TimeoutException_LanzaExcepcion()
        {
            var reporte = CrearReporteValido();
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ReportarJugadorAsync(reporte);
            });
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_InvalidOperationException_LanzaExcepcion()
        {
            var reporte = CrearReporteValido();
            var invalidOperationException =
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ReportarJugadorAsync(reporte);
            });
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_InvocaFabricaClientes()
        {
            var reporte = CrearReporteValido();
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.ReportarJugadorAsync(reporte);

            _fabricaClientesMock.Verify(fabrica => fabrica.CrearClienteReportes(), Times.Once);
        }

        [TestMethod]
        public async Task Prueba_ReportarJugadorAsync_InvocaEjecutor()
        {
            var reporte = CrearReporteValido();
            var resultadoEsperado = new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };

            ConfigurarEjecutorExitoso(resultadoEsperado);

            await _servicio.ReportarJugadorAsync(reporte);

            _ejecutorMock.Verify(
                e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioReportes.IReportesManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioReportes.IReportesManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()),
                Times.Once);
        }

        private static DTOs.ReporteJugadorDTO CrearReporteValido()
        {
            return new DTOs.ReporteJugadorDTO
            {
                NombreUsuarioReportante = "UsuarioReportante",
                NombreUsuarioReportado = "UsuarioReportado",
                Motivo = "Comportamiento inapropiado"
            };
        }

        private void ConfigurarEjecutorExitoso(DTOs.ResultadoOperacionDTO resultado)
        {
            _ejecutorMock
                .Setup(ejecutor => ejecutor.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioReportes.IReportesManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioReportes.IReportesManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(ejecutor => ejecutor.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioReportes.IReportesManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioReportes.IReportesManejador,
                        Task<DTOs.ResultadoOperacionDTO>>>()))
                .ThrowsAsync(excepcion);
        }
    }
}
