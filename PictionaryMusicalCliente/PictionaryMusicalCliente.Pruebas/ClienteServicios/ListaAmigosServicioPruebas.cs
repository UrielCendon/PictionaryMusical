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
    public class ListaAmigosServicioPruebas
    {
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private ListaAmigosServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();

            _servicio = new ListaAmigosServicio(
                _manejadorErrorMock.Object,
                _fabricaClientesMock.Object);
        }

        [TestCleanup]
        public void Limpiar()
        {
            _servicio?.Dispose();
            _servicio = null;
        }

        [TestMethod]
        public void Prueba_Constructor_ManejadorErrorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new ListaAmigosServicio(
                    null,
                    _fabricaClientesMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_FabricaNula_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new ListaAmigosServicio(
                    _manejadorErrorMock.Object,
                    null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ParametrosValidos_CreaInstancia()
        {
            var servicio = new ListaAmigosServicio(
                _manejadorErrorMock.Object,
                _fabricaClientesMock.Object);

            Assert.IsInstanceOfType(servicio, typeof(ListaAmigosServicio));
            servicio.Dispose();
        }

        [TestMethod]
        public async Task Prueba_SuscribirAsync_NombreUsuarioNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.SuscribirAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_SuscribirAsync_NombreUsuarioVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.SuscribirAsync("");
            });
        }

        [TestMethod]
        public async Task Prueba_SuscribirAsync_NombreUsuarioEspacios_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.SuscribirAsync("   ");
            });
        }

        [TestMethod]
        public async Task Prueba_CancelarSuscripcionAsync_NombreUsuarioNulo_RetornaSinAccion()
        {
            await _servicio.CancelarSuscripcionAsync(null);

            Assert.IsInstanceOfType(_servicio, typeof(ListaAmigosServicio));
        }

        [TestMethod]
        public async Task Prueba_CancelarSuscripcionAsync_NombreUsuarioVacio_RetornaSinAccion()
        {
            await _servicio.CancelarSuscripcionAsync("");

            Assert.IsInstanceOfType(_servicio, typeof(ListaAmigosServicio));
        }

        [TestMethod]
        public async Task Prueba_ObtenerAmigosAsync_NombreUsuarioNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ObtenerAmigosAsync(null);
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerAmigosAsync_NombreUsuarioVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ObtenerAmigosAsync("");
            });
        }

        [TestMethod]
        public async Task Prueba_ObtenerAmigosAsync_NombreUsuarioEspacios_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _servicio.ObtenerAmigosAsync("   ");
            });
        }

        [TestMethod]
        public void Prueba_ListaActual_InicialmenteVacia_RetornaListaVacia()
        {
            var resultado = _servicio.ListaActual;

            resultado.Should().BeEmpty();
        }

        [TestMethod]
        public void Prueba_Dispose_LlamadoMultiple_NoLanzaExcepcion()
        {
            _servicio.Dispose();
            _servicio.Dispose();

            Assert.IsInstanceOfType(_servicio, typeof(ListaAmigosServicio), 
                "El servicio debe mantener su tipo después de Dispose");
        }

        [TestMethod]
        public void Prueba_ListaActual_EsDesoloLectura()
        {
            var resultado = _servicio.ListaActual;

            Assert.IsInstanceOfType(resultado, typeof(IReadOnlyList<DTOs.AmigoDTO>));
        }
    }
}
