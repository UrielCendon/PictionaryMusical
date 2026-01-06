using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Salas
{
    [TestClass]
    public class InvitacionesManejadorPruebas
    {
        private const string CodigoSalaPrueba = "123456";
        private const string CorreoPrueba = "correo@ejemplo.com";

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<ISalasManejador> _mockSalasManejador;
        private Mock<ICorreoInvitacionNotificador> _mockCorreoNotificador;
        private InvitacionesManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockSalasManejador = new Mock<ISalasManejador>();
            _mockCorreoNotificador = new Mock<ICorreoInvitacionNotificador>();
            _manejador = new InvitacionesManejador(
                _mockContextoFactoria.Object,
                _mockRepositorioFactoria.Object,
                _mockSalasManejador.Object,
                _mockCorreoNotificador.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new InvitacionesManejador(
                    null,
                    _mockRepositorioFactoria.Object,
                    _mockSalasManejador.Object,
                    _mockCorreoNotificador.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new InvitacionesManejador(
                    _mockContextoFactoria.Object,
                    null,
                    _mockSalasManejador.Object,
                    _mockCorreoNotificador.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionSalasManejadorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new InvitacionesManejador(
                    _mockContextoFactoria.Object,
                    _mockRepositorioFactoria.Object,
                    null,
                    _mockCorreoNotificador.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionCorreoNotificadorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new InvitacionesManejador(
                    _mockContextoFactoria.Object,
                    _mockRepositorioFactoria.Object,
                    _mockSalasManejador.Object,
                    null));
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_RetornaFalloCorreoVacio()
        {
            var invitacion = new InvitacionSalaDTO
            {
                CodigoSala = CodigoSalaPrueba,
                Correo = string.Empty
            };

            var resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_RetornaFalloCodigoSalaVacio()
        {
            var invitacion = new InvitacionSalaDTO
            {
                CodigoSala = string.Empty,
                Correo = CorreoPrueba
            };

            var resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacionAsync_RetornaFalloInvitacionNula()
        {
            var resultado = await _manejador.EnviarInvitacionAsync(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }
    }
}
