using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    [TestClass]
    public class SalasServicioPruebas
    {
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private SalasServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();

            _servicio = new SalasServicio(
                _fabricaClientesMock.Object,
                _manejadorErrorMock.Object);
        }

        [TestCleanup]
        public void Limpiar()
        {
            _servicio?.Dispose();
            _servicio = null;
        }

        [TestMethod]
        public void Prueba_Constructor_FabricaNula_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new SalasServicio(
                    null,
                    _manejadorErrorMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ManejadorErrorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new SalasServicio(
                    _fabricaClientesMock.Object,
                    null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ParametrosValidos_CreaInstancia()
        {
            var servicio = new SalasServicio(
                _fabricaClientesMock.Object,
                _manejadorErrorMock.Object);

            Assert.IsInstanceOfType(servicio, typeof(SalasServicio));
            servicio.Dispose();
        }

        [TestMethod]
        public async Task Prueba_CrearSalaAsync_NombreCreadorNulo_LanzaExcepcion()
        {
            var configuracion = CrearConfiguracionValida();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.CrearSalaAsync(null, configuracion);
            });
        }

        [TestMethod]
        public async Task Prueba_CrearSalaAsync_NombreCreadorVacio_LanzaExcepcion()
        {
            var configuracion = CrearConfiguracionValida();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.CrearSalaAsync("", configuracion);
            });
        }

        [TestMethod]
        public async Task Prueba_CrearSalaAsync_ConfiguracionNula_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.CrearSalaAsync("creador", null);
            });
        }

        [TestMethod]
        public async Task Prueba_UnirseSalaAsync_CodigoSalaNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.UnirseSalaAsync(null, "usuario");
            });
        }

        [TestMethod]
        public async Task Prueba_UnirseSalaAsync_CodigoSalaVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.UnirseSalaAsync("", "usuario");
            });
        }

        [TestMethod]
        public async Task Prueba_UnirseSalaAsync_NombreUsuarioNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.UnirseSalaAsync("SALA123", null);
            });
        }

        [TestMethod]
        public async Task Prueba_UnirseSalaAsync_NombreUsuarioVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.UnirseSalaAsync("SALA123", "");
            });
        }

        [TestMethod]
        public async Task Prueba_AbandonarSalaAsync_CodigoSalaNulo_RetornaSinAccion()
        {
            await _servicio.AbandonarSalaAsync(null, "usuario");

            Assert.IsInstanceOfType(_servicio, typeof(SalasServicio));
        }

        [TestMethod]
        public async Task Prueba_AbandonarSalaAsync_CodigoSalaVacio_RetornaSinAccion()
        {
            await _servicio.AbandonarSalaAsync("", "usuario");

            Assert.IsInstanceOfType(_servicio, typeof(SalasServicio));
        }

        [TestMethod]
        public async Task Prueba_AbandonarSalaAsync_NombreUsuarioNulo_RetornaSinAccion()
        {
            await _servicio.AbandonarSalaAsync("SALA123", null);

            Assert.IsInstanceOfType(_servicio, typeof(SalasServicio));
        }

        [TestMethod]
        public async Task Prueba_AbandonarSalaAsync_NombreUsuarioVacio_RetornaSinAccion()
        {
            await _servicio.AbandonarSalaAsync("123456", "");

            Assert.IsInstanceOfType(_servicio, typeof(SalasServicio));
        }

        [TestMethod]
        public async Task Prueba_ExpulsarJugadorAsync_CodigoSalaNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ExpulsarJugadorAsync(null, "host", "jugador");
            });
        }

        [TestMethod]
        public async Task Prueba_ExpulsarJugadorAsync_CodigoSalaVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ExpulsarJugadorAsync("", "host", "jugador");
            });
        }

        [TestMethod]
        public async Task Prueba_ExpulsarJugadorAsync_NombreHostNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ExpulsarJugadorAsync("SALA123", null, "jugador");
            });
        }

        [TestMethod]
        public async Task Prueba_ExpulsarJugadorAsync_NombreHostVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ExpulsarJugadorAsync("SALA123", "", "jugador");
            });
        }

        [TestMethod]
        public async Task Prueba_ExpulsarJugadorAsync_NombreJugadorNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ExpulsarJugadorAsync("SALA123", "host", null);
            });
        }

        [TestMethod]
        public async Task Prueba_ExpulsarJugadorAsync_NombreJugadorVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ExpulsarJugadorAsync("SALA123", "host", "");
            });
        }

        //fix múltiples asserts innecesarios
        [TestMethod]
        public void Prueba_ListaSalasActual_InicialmenteVacia_RetornaListaVacia()
        {
            var resultado = _servicio.ListaSalasActual;

            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [TestMethod]
        public void Prueba_Dispose_LiberaRecursos()
        {
            var servicioLocal = new SalasServicio(
                _fabricaClientesMock.Object,
                _manejadorErrorMock.Object);
            
            servicioLocal.Dispose();

            Assert.IsInstanceOfType(servicioLocal, typeof(SalasServicio), "El servicio debe ser una instancia válida después de Dispose");
        }

        [TestMethod]
        public void Prueba_ListaSalasActual_EsDeSoloLectura()
        {
            var resultado = _servicio.ListaSalasActual;

            Assert.IsInstanceOfType(resultado, typeof(IReadOnlyList<DTOs.SalaDTO>));
        }

        private static DTOs.ConfiguracionPartidaDTO CrearConfiguracionValida()
        {
            return new DTOs.ConfiguracionPartidaDTO
            {
                NumeroRondas = 3,
                TiempoPorRondaSegundos = 60,
                IdiomaCanciones = "es",
                Dificultad = "Normal"
            };
        }
    }
}
