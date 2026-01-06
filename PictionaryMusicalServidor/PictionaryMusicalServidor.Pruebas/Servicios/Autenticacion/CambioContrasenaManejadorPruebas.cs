using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class CambioContrasenaManejadorPruebas
    {
        private const string IdentificadorPrueba = "usuario@test.com";
        private const string TokenPrueba = "token-prueba-123";
        private const string CodigoPrueba = "123456";
        private const string NuevaContrasenaPrueba = "NuevaContrasena123!";
        private const string IdiomaPrueba = "es";
        private const string MensajeExitoPrueba = "Operacion exitosa";

        private Mock<IRecuperacionCuentaServicio> _mockRecuperacionServicio;
        private CambioContrasenaManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockRecuperacionServicio = new Mock<IRecuperacionCuentaServicio>();
            _manejador = new CambioContrasenaManejador(_mockRecuperacionServicio.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRecuperacionServicioNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CambioContrasenaManejador(null));
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_DelegaAlServicio()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorPrueba,
                Idioma = IdiomaPrueba
            };
            var resultadoEsperado = new ResultadoSolicitudRecuperacionDTO
            {
                CodigoEnviado = true,
                CuentaEncontrada = true
            };
            _mockRecuperacionServicio
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Returns(resultadoEsperado);

            var resultado = _manejador.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_RetornaFalloArgumentoNulo()
        {
            _mockRecuperacionServicio
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(
                    It.IsAny<SolicitudRecuperarCuentaDTO>()))
                .Throws(new ArgumentNullException());

            var resultado = _manejador.SolicitarCodigoRecuperacion(null);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoRecuperacion_DelegaAlServicio()
        {
            var solicitud = new ReenvioCodigoDTO { TokenCodigo = TokenPrueba };
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true
            };
            _mockRecuperacionServicio
                .Setup(servicio => servicio.ReenviarCodigoRecuperacion(solicitud))
                .Returns(resultadoEsperado);

            var resultado = _manejador.ReenviarCodigoRecuperacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoRecuperacion_RetornaFalloArgumentoNulo()
        {
            _mockRecuperacionServicio
                .Setup(servicio => servicio.ReenviarCodigoRecuperacion(
                    It.IsAny<ReenvioCodigoDTO>()))
                .Throws(new ArgumentNullException());

            var resultado = _manejador.ReenviarCodigoRecuperacion(null);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_DelegaAlServicio()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenPrueba,
                CodigoIngresado = CodigoPrueba
            };
            var resultadoEsperado = new ResultadoOperacionDTO { OperacionExitosa = true };
            _mockRecuperacionServicio
                .Setup(servicio => servicio.ConfirmarCodigoRecuperacion(confirmacion))
                .Returns(resultadoEsperado);

            var resultado = _manejador.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_RetornaFalloArgumentoNulo()
        {
            _mockRecuperacionServicio
                .Setup(servicio => servicio.ConfirmarCodigoRecuperacion(
                    It.IsAny<ConfirmacionCodigoDTO>()))
                .Throws(new ArgumentNullException());

            var resultado = _manejador.ConfirmarCodigoRecuperacion(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_DelegaAlServicio()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenPrueba,
                NuevaContrasena = NuevaContrasenaPrueba
            };
            var resultadoEsperado = new ResultadoOperacionDTO { OperacionExitosa = true };
            _mockRecuperacionServicio
                .Setup(servicio => servicio.ActualizarContrasena(solicitud))
                .Returns(resultadoEsperado);

            var resultado = _manejador.ActualizarContrasena(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_RetornaFalloArgumentoNulo()
        {
            _mockRecuperacionServicio
                .Setup(servicio => servicio.ActualizarContrasena(
                    It.IsAny<ActualizacionContrasenaDTO>()))
                .Throws(new ArgumentNullException());

            var resultado = _manejador.ActualizarContrasena(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }
    }
}
