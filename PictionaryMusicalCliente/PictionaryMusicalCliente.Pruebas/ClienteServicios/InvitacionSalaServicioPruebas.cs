using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using PictionaryMusicalCliente.Utilidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase InvitacionSalaServicio.
    /// Verifica el comportamiento del servicio de invitaciones a salas.
    /// </summary>
    [TestClass]
    public class InvitacionSalaServicioPruebas
    {
        private Mock<IInvitacionesServicio> _invitacionesServicioMock;
        private Mock<IListaAmigosServicio> _listaAmigosServicioMock;
        private Mock<IPerfilServicio> _perfilServicioMock;
        private Mock<SonidoManejador> _sonidoManejadorMock;
        private Mock<IAvisoServicio> _avisoServicioMock;
        private InvitacionSalaServicio _servicio;

        /// <summary>
        /// Inicializa los mocks y el servicio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _invitacionesServicioMock = new Mock<IInvitacionesServicio>();
            _listaAmigosServicioMock = new Mock<IListaAmigosServicio>();
            _perfilServicioMock = new Mock<IPerfilServicio>();
            _sonidoManejadorMock = new Mock<SonidoManejador>();
            _avisoServicioMock = new Mock<IAvisoServicio>();

            _servicio = new InvitacionSalaServicio(
                _invitacionesServicioMock.Object,
                _listaAmigosServicioMock.Object,
                _perfilServicioMock.Object,
                _sonidoManejadorMock.Object,
                _avisoServicioMock.Object);
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
        public void Prueba_Constructor_InvitacionesServicioNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InvitacionSalaServicio(
                    null,
                    _listaAmigosServicioMock.Object,
                    _perfilServicioMock.Object,
                    _sonidoManejadorMock.Object,
                    _avisoServicioMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ListaAmigosServicioNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InvitacionSalaServicio(
                    _invitacionesServicioMock.Object,
                    null,
                    _perfilServicioMock.Object,
                    _sonidoManejadorMock.Object,
                    _avisoServicioMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_PerfilServicioNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InvitacionSalaServicio(
                    _invitacionesServicioMock.Object,
                    _listaAmigosServicioMock.Object,
                    null,
                    _sonidoManejadorMock.Object,
                    _avisoServicioMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_SonidoManejadorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InvitacionSalaServicio(
                    _invitacionesServicioMock.Object,
                    _listaAmigosServicioMock.Object,
                    _perfilServicioMock.Object,
                    null,
                    _avisoServicioMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_AvisoServicioNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var servicio = new InvitacionSalaServicio(
                    _invitacionesServicioMock.Object,
                    _listaAmigosServicioMock.Object,
                    _perfilServicioMock.Object,
                    _sonidoManejadorMock.Object,
                    null);
            });
        }

        [TestMethod]
        public async Task Prueba_InvitarPorCorreoAsync_CodigoSalaNulo_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.InvitarPorCorreoAsync(null, "correo@ejemplo.com");
            });
        }

        [TestMethod]
        public async Task Prueba_InvitarPorCorreoAsync_CodigoSalaVacio_LanzaExcepcion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _servicio.InvitarPorCorreoAsync("", "correo@ejemplo.com");
            });
        }

        [TestMethod]
        public async Task Prueba_InvitarPorCorreoAsync_CorreoInvalido_RetornaFallo()
        {
            var resultado = await _servicio.InvitarPorCorreoAsync("SALA123", "correo-invalido");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.Exitoso);
            Assert.IsNotNull(resultado.Mensaje);
        }

        [TestMethod]
        public async Task Prueba_InvitarPorCorreoAsync_CorreoNulo_RetornaFallo()
        {
            var resultado = await _servicio.InvitarPorCorreoAsync("SALA123", null);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.Exitoso);
        }

        [TestMethod]
        public async Task Prueba_InvitarPorCorreoAsync_CorreoVacio_RetornaFallo()
        {
            var resultado = await _servicio.InvitarPorCorreoAsync("SALA123", "");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.Exitoso);
        }

        [TestMethod]
        public async Task Prueba_InvitarPorCorreoAsync_CorreoValido_InvocaServicio()
        {
            string codigoSala = "SALA123";
            string correo = "usuario@ejemplo.com";

            _invitacionesServicioMock
                .Setup(s => s.EnviarInvitacionAsync(codigoSala, correo))
                .ReturnsAsync(new DTOs.ResultadoOperacionDTO
                {
                    OperacionExitosa = true,
                    Mensaje = "Invitacion enviada"
                });

            var resultado = await _servicio.InvitarPorCorreoAsync(codigoSala, correo);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Exitoso);
            _invitacionesServicioMock.Verify(
                s => s.EnviarInvitacionAsync(codigoSala, correo),
                Times.Once);
        }

        [TestMethod]
        public async Task Prueba_InvitarPorCorreoAsync_ServicioRetornaFallo_RetornaFallo()
        {
            string codigoSala = "SALA123";
            string correo = "usuario@ejemplo.com";

            _invitacionesServicioMock
                .Setup(s => s.EnviarInvitacionAsync(codigoSala, correo))
                .ReturnsAsync(new DTOs.ResultadoOperacionDTO
                {
                    OperacionExitosa = false,
                    Mensaje = "Error al enviar"
                });

            var resultado = await _servicio.InvitarPorCorreoAsync(codigoSala, correo);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.Exitoso);
        }

        [TestMethod]
        public async Task Prueba_ObtenerInvitacionAmigosAsync_UsuarioNulo_RetornaFallo()
        {
            var amigosInvitados = new HashSet<int>();
            Action<string> mostrarMensaje = m => { };

            var resultado = await _servicio.ObtenerInvitacionAmigosAsync(
                "SALA123",
                null,
                amigosInvitados,
                mostrarMensaje);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.Exitoso);
        }

        [TestMethod]
        public async Task Prueba_ObtenerInvitacionAmigosAsync_UsuarioVacio_RetornaFallo()
        {
            var amigosInvitados = new HashSet<int>();
            Action<string> mostrarMensaje = m => { };

            var resultado = await _servicio.ObtenerInvitacionAmigosAsync(
                "SALA123",
                "",
                amigosInvitados,
                mostrarMensaje);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.Exitoso);
        }

        [TestMethod]
        public async Task Prueba_ObtenerInvitacionAmigosAsync_SinAmigos_RetornaFallo()
        {
            var amigosInvitados = new HashSet<int>();
            Action<string> mostrarMensaje = m => { };

            _listaAmigosServicioMock
                .Setup(s => s.ObtenerAmigosAsync("usuario"))
                .ReturnsAsync(new List<DTOs.AmigoDTO>());

            var resultado = await _servicio.ObtenerInvitacionAmigosAsync(
                "SALA123",
                "usuario",
                amigosInvitados,
                mostrarMensaje);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.Exitoso);
        }

        [TestMethod]
        public void Prueba_Dispose_LlamadoMultiple_NoLanzaExcepcion()
        {
            _servicio.Dispose();
            _servicio.Dispose();

            Assert.IsNotNull(_servicio, "El servicio no debe ser nulo después de Dispose");
        }
    }
}
