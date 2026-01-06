using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class CodigoVerificacionManejadorPruebas
    {
        private const string UsuarioPrueba = "UsuarioPrueba";
        private const string CorreoPrueba = "usuario@test.com";
        private const string ContrasenaPrueba = "Contrasena123!";
        private const string TokenPrueba = "token-prueba-123";
        private const string CodigoPrueba = "123456";
        private const int AvatarIdPrueba = 1;

        private Mock<IVerificacionRegistroServicio> _mockVerificacionRegistro;
        private Mock<IRecuperacionCuentaServicio> _mockRecuperacionCuenta;
        private CodigoVerificacionManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockVerificacionRegistro = new Mock<IVerificacionRegistroServicio>();
            _mockRecuperacionCuenta = new Mock<IRecuperacionCuentaServicio>();
            _manejador = new CodigoVerificacionManejador(
                _mockVerificacionRegistro.Object,
                _mockRecuperacionCuenta.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionVerificacionRegistroNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CodigoVerificacionManejador(
                    null, 
                    _mockRecuperacionCuenta.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRecuperacionCuentaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CodigoVerificacionManejador(
                    _mockVerificacionRegistro.Object, 
                    null));
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_DelegaAlServicioVerificacion()
        {
            var nuevaCuenta = CrearNuevaCuentaPrueba();
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO { CodigoEnviado = true };
            _mockVerificacionRegistro
                .Setup(servicio => servicio.SolicitarCodigo(nuevaCuenta))
                .Returns(resultadoEsperado);

            var resultado = _manejador.SolicitarCodigoVerificacion(nuevaCuenta);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_RetornaFalloArgumentoNulo()
        {
            _mockVerificacionRegistro
                .Setup(servicio => servicio.SolicitarCodigo(It.IsAny<NuevaCuentaDTO>()))
                .Throws(new ArgumentNullException());

            var resultado = _manejador.SolicitarCodigoVerificacion(null);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoVerificacion_DelegaAlServicioVerificacion()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO { TokenCodigo = TokenPrueba };
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO { CodigoEnviado = true };
            _mockVerificacionRegistro
                .Setup(servicio => servicio.ReenviarCodigo(solicitud))
                .Returns(resultadoEsperado);

            var resultado = _manejador.ReenviarCodigoVerificacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoVerificacion_RetornaFalloArgumentoNulo()
        {
            _mockVerificacionRegistro
                .Setup(servicio => servicio.ReenviarCodigo(
                    It.IsAny<ReenvioCodigoVerificacionDTO>()))
                .Throws(new ArgumentNullException());

            var resultado = _manejador.ReenviarCodigoVerificacion(null);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoVerificacion_DelegaAlServicioVerificacion()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenPrueba,
                CodigoIngresado = CodigoPrueba
            };
            var resultadoEsperado = new ResultadoRegistroCuentaDTO { RegistroExitoso = true };
            _mockVerificacionRegistro
                .Setup(servicio => servicio.ConfirmarCodigo(confirmacion))
                .Returns(resultadoEsperado);

            var resultado = _manejador.ConfirmarCodigoVerificacion(confirmacion);

            Assert.IsTrue(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_DelegaAlServicioRecuperacion()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = CorreoPrueba
            };
            var resultadoEsperado = new ResultadoSolicitudRecuperacionDTO
            {
                CodigoEnviado = true,
                CuentaEncontrada = true
            };
            _mockRecuperacionCuenta
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Returns(resultadoEsperado);

            var resultado = _manejador.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_RetornaFalloArgumentoNulo()
        {
            _mockRecuperacionCuenta
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(
                    It.IsAny<SolicitudRecuperarCuentaDTO>()))
                .Throws(new ArgumentNullException());

            var resultado = _manejador.SolicitarCodigoRecuperacion(null);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_DelegaAlServicioRecuperacion()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenPrueba,
                CodigoIngresado = CodigoPrueba
            };
            var resultadoEsperado = new ResultadoOperacionDTO { OperacionExitosa = true };
            _mockRecuperacionCuenta
                .Setup(servicio => servicio.ConfirmarCodigoRecuperacion(confirmacion))
                .Returns(resultadoEsperado);

            var resultado = _manejador.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        private static NuevaCuentaDTO CrearNuevaCuentaPrueba()
        {
            return new NuevaCuentaDTO
            {
                Usuario = UsuarioPrueba,
                Correo = CorreoPrueba,
                Contrasena = ContrasenaPrueba,
                AvatarId = AvatarIdPrueba
            };
        }
    }
}
