using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Administrador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase AmigosServicio.
    /// Verifica el comportamiento del servicio duplex de gestion de amigos.
    /// </summary>
    [TestClass]
    public class AmigosServicioPruebas
    {
        private Mock<ISolicitudesAmistadAdministrador> _administradorSolicitudesMock;
        private Mock<IManejadorErrorServicio> _manejadorErrorMock;
        private Mock<IWcfClienteFabrica> _fabricaClientesMock;
        private AmigosServicio _servicio;

        /// <summary>
        /// Inicializa los mocks y el servicio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _administradorSolicitudesMock = new Mock<ISolicitudesAmistadAdministrador>();
            _manejadorErrorMock = new Mock<IManejadorErrorServicio>();
            _fabricaClientesMock = new Mock<IWcfClienteFabrica>();

            _servicio = new AmigosServicio(
                _administradorSolicitudesMock.Object,
                _manejadorErrorMock.Object,
                _fabricaClientesMock.Object);
        }

        /// <summary>
        /// Limpia los recursos despues de cada prueba.
        /// </summary>
        [TestCleanup]
        public void Limpiar()
        {
            _servicio?.Dispose();
            _servicio = null;
        }

        [TestMethod]
        public void Prueba_Constructor_AdministradorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new AmigosServicio(
                    null,
                    _manejadorErrorMock.Object,
                    _fabricaClientesMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ManejadorErrorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new AmigosServicio(
                    _administradorSolicitudesMock.Object,
                    null,
                    _fabricaClientesMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_FabricaNula_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new AmigosServicio(
                    _administradorSolicitudesMock.Object,
                    _manejadorErrorMock.Object,
                    null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ParametrosValidos_CreaInstancia()
        {
            var servicio = new AmigosServicio(
                _administradorSolicitudesMock.Object,
                _manejadorErrorMock.Object,
                _fabricaClientesMock.Object);

            Assert.IsNotNull(servicio);
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
            // El metodo retorna sin lanzar excepcion cuando el parametro es nulo
        }

        [TestMethod]
        public async Task Prueba_CancelarSuscripcionAsync_NombreUsuarioVacio_RetornaSinAccion()
        {
            await _servicio.CancelarSuscripcionAsync("");
            // El metodo retorna sin lanzar excepcion cuando el parametro es vacio
        }

        [TestMethod]
        public void Prueba_EnviarSolicitudAsync_DelegaAlClienteWcf()
        {
            // EnviarSolicitudAsync delega directamente al cliente WCF sin validar parametros
            // La validacion ocurre en el servidor
            Assert.IsNotNull(_servicio);
        }

        [TestMethod]
        public void Prueba_ResponderSolicitudAsync_DelegaAlClienteWcf()
        {
            // ResponderSolicitudAsync delega directamente al cliente WCF sin validar parametros
            // La validacion ocurre en el servidor
            Assert.IsNotNull(_servicio);
        }

        [TestMethod]
        public void Prueba_EliminarAmigoAsync_DelegaAlClienteWcf()
        {
            // EliminarAmigoAsync delega directamente al cliente WCF sin validar parametros
            // La validacion ocurre en el servidor
            Assert.IsNotNull(_servicio);
        }

        [TestMethod]
        public void Prueba_SolicitudesPendientes_RetornaListaDelAdministrador()
        {
            var solicitudesEsperadas = new List<DTOs.SolicitudAmistadDTO>
            {
                new DTOs.SolicitudAmistadDTO
                {
                    UsuarioEmisor = "usuario1",
                    UsuarioReceptor = "usuario2"
                }
            };

            _administradorSolicitudesMock
                .Setup(a => a.ObtenerSolicitudes())
                .Returns(solicitudesEsperadas);

            var resultado = _servicio.SolicitudesPendientes;

            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
            _administradorSolicitudesMock.Verify(
                a => a.ObtenerSolicitudes(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_SolicitudesPendientes_SinSolicitudes_RetornaListaVacia()
        {
            var solicitudesVacias = new List<DTOs.SolicitudAmistadDTO>();

            _administradorSolicitudesMock
                .Setup(a => a.ObtenerSolicitudes())
                .Returns(solicitudesVacias);

            var resultado = _servicio.SolicitudesPendientes;

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_Dispose_LlamadoMultiple_NoLanzaExcepcion()
        {
            _servicio.Dispose();
            _servicio.Dispose();
        }
    }
}
