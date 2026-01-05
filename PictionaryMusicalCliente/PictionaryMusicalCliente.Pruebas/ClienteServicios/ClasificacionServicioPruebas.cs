using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    [TestClass]
    public class ClasificacionServicioPruebas
    {
        private Mock<IWcfClienteEjecutor> _ejecutorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<PictionaryServidorServicioClasificacion.IClasificacionManejador>
            _clienteClasificacionMock;
        private ClasificacionServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _ejecutorMock = new Mock<IWcfClienteEjecutor>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _clienteClasificacionMock =
                new Mock<PictionaryServidorServicioClasificacion.IClasificacionManejador>();

            _fabricaClientesMock
                .Setup(f => f.CrearClienteClasificacion())
                .Returns(_clienteClasificacionMock.Object);

            _servicio = new ClasificacionServicio(
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
                var servicio = new ClasificacionServicio(
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
                var servicio = new ClasificacionServicio(
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
                var servicio = new ClasificacionServicio(
                    _ejecutorMock.Object,
                    _fabricaClientesMock.Object,
                    null);
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_Exitoso_RetornaLista()
        {
            var clasificacionEsperada = CrearClasificacionValida();

            ConfigurarEjecutorExitoso(clasificacionEsperada);

            var resultado = await _servicio.ObtenerTopJugadoresAsync();

            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_Exitoso_DatosCorrectos()
        {
            var clasificacionEsperada = CrearClasificacionValida();

            ConfigurarEjecutorExitoso(clasificacionEsperada);

            var resultado = await _servicio.ObtenerTopJugadoresAsync();

            Assert.AreEqual("Jugador1", resultado[0].Usuario);
            Assert.AreEqual(1000, resultado[0].Puntos);
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_ListaVacia_RetornaVacia()
        {
            var clasificacionVacia = Array.Empty<DTOs.ClasificacionUsuarioDTO>();

            ConfigurarEjecutorExitoso(clasificacionVacia);

            var resultado = await _servicio.ObtenerTopJugadoresAsync();

            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_ResultadoNulo_RetornaVacia()
        {
            ConfigurarEjecutorExitoso(null);

            var resultado = await _servicio.ObtenerTopJugadoresAsync();

            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_FaultException_LanzaExcepcion()
        {
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ObtenerTopJugadoresAsync();
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_FaultException_TipoFallaServicio()
        {
            var faultException = new FaultException("Error del servidor");

            _manejadorErrorMock
                .Setup(m => m.ObtenerMensaje(
                    It.IsAny<FaultException>(),
                    It.IsAny<string>()))
                .Returns("Error procesando solicitud");

            ConfigurarEjecutorConExcepcion(faultException);

            try
            {
                await _servicio.ObtenerTopJugadoresAsync();
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_CommunicationException_LanzaExcepcion()
        {
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ObtenerTopJugadoresAsync();
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_CommunicationException_TipoComunicacion()
        {
            var communicationException = new CommunicationException("Error de red");

            ConfigurarEjecutorConExcepcion(communicationException);

            try
            {
                await _servicio.ObtenerTopJugadoresAsync();
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.Comunicacion, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_TimeoutException_LanzaExcepcion()
        {
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ObtenerTopJugadoresAsync();
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_TimeoutException_TipoTiempoAgotado()
        {
            var timeoutException = new TimeoutException("Tiempo agotado");

            ConfigurarEjecutorConExcepcion(timeoutException);

            try
            {
                await _servicio.ObtenerTopJugadoresAsync();
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.TiempoAgotado, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_InvalidOperationException_LanzaExcepcion()
        {
            var invalidOperationException =
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            await Assert.ThrowsAsync<ServicioExcepcion>(async () =>
            {
                await _servicio.ObtenerTopJugadoresAsync();
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_InvalidOperationException_TipoInvalida()
        {
            var invalidOperationException =
                new InvalidOperationException("Operacion no valida");

            ConfigurarEjecutorConExcepcion(invalidOperationException);

            try
            {
                await _servicio.ObtenerTopJugadoresAsync();
                Assert.Fail("Se esperaba ServicioExcepcion");
            }
            catch (ServicioExcepcion excepcion)
            {
                Assert.AreEqual(TipoErrorServicio.OperacionInvalida, excepcion.Tipo);
            }
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_InvocaFabricaClientes()
        {
            var clasificacionEsperada = CrearClasificacionValida();

            ConfigurarEjecutorExitoso(clasificacionEsperada);

            await _servicio.ObtenerTopJugadoresAsync();

            _fabricaClientesMock.Verify(f => f.CrearClienteClasificacion(), Times.Once);
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_InvocaEjecutor()
        {
            var clasificacionEsperada = CrearClasificacionValida();

            ConfigurarEjecutorExitoso(clasificacionEsperada);

            await _servicio.ObtenerTopJugadoresAsync();

            _ejecutorMock.Verify(
                e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioClasificacion.IClasificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioClasificacion.IClasificacionManejador,
                        Task<DTOs.ClasificacionUsuarioDTO[]>>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Prueba_ObtenerTopJugadoresAsync_RetornaTipoReadOnlyList()
        {
            var clasificacionEsperada = CrearClasificacionValida();

            ConfigurarEjecutorExitoso(clasificacionEsperada);

            var resultado = await _servicio.ObtenerTopJugadoresAsync();

            Assert.IsInstanceOfType(resultado, typeof(IReadOnlyList<DTOs.ClasificacionUsuarioDTO>));
        }

        private static DTOs.ClasificacionUsuarioDTO[] CrearClasificacionValida()
        {
            return new[]
            {
                new DTOs.ClasificacionUsuarioDTO
                {
                    Usuario = "Jugador1",
                    Puntos = 1000
                },
                new DTOs.ClasificacionUsuarioDTO
                {
                    Usuario = "Jugador2",
                    Puntos = 800
                },
                new DTOs.ClasificacionUsuarioDTO
                {
                    Usuario = "Jugador3",
                    Puntos = 600
                }
            };
        }

        private void ConfigurarEjecutorExitoso(DTOs.ClasificacionUsuarioDTO[] resultado)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioClasificacion.IClasificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioClasificacion.IClasificacionManejador,
                        Task<DTOs.ClasificacionUsuarioDTO[]>>>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarEjecutorConExcepcion(Exception excepcion)
        {
            _ejecutorMock
                .Setup(e => e.EjecutarAsincronoAsync(
                    It.IsAny<PictionaryServidorServicioClasificacion.IClasificacionManejador>(),
                    It.IsAny<Func<PictionaryServidorServicioClasificacion.IClasificacionManejador,
                        Task<DTOs.ClasificacionUsuarioDTO[]>>>()))
                .ThrowsAsync(excepcion);
        }
    }
}
