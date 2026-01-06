using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Datos.Modelo;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class CuentaManejadorPruebas
    {
        private const string UsuarioPrueba = "UsuarioPrueba";
        private const string CorreoPrueba = "usuario@test.com";
        private const string ContrasenaPrueba = "Contrasena123!";
        private const string TokenPrueba = "token-prueba-123";
        private const string CodigoPrueba = "123456";
        private const int AvatarIdPrueba = 1;

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<IVerificacionRegistroServicio> _mockVerificacionServicio;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private CuentaManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockVerificacionServicio = new Mock<IVerificacionRegistroServicio>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();

            _mockContextoFactoria
                .Setup(factoria => factoria.CrearContexto())
                .Returns(_mockContexto.Object);

            _manejador = new CuentaManejador(
                _mockContextoFactoria.Object,
                _mockRepositorioFactoria.Object,
                _mockVerificacionServicio.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CuentaManejador(
                    null, 
                    _mockRepositorioFactoria.Object, 
                    _mockVerificacionServicio.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CuentaManejador(
                    _mockContextoFactoria.Object, 
                    null, 
                    _mockVerificacionServicio.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionVerificacionServicioNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CuentaManejador(
                    _mockContextoFactoria.Object, 
                    _mockRepositorioFactoria.Object, 
                    null));
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_LanzaExcepcionCuentaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _manejador.RegistrarCuenta(null));
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_DelegaAlServicio()
        {
            var nuevaCuenta = CrearNuevaCuentaPrueba();
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO { CodigoEnviado = true };
            _mockVerificacionServicio
                .Setup(servicio => servicio.SolicitarCodigo(nuevaCuenta))
                .Returns(resultadoEsperado);

            var resultado = _manejador.SolicitarCodigoVerificacion(nuevaCuenta);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_LlamaMetodoServicio()
        {
            var nuevaCuenta = CrearNuevaCuentaPrueba();
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO { CodigoEnviado = true };
            _mockVerificacionServicio
                .Setup(servicio => servicio.SolicitarCodigo(nuevaCuenta))
                .Returns(resultadoEsperado);

            _manejador.SolicitarCodigoVerificacion(nuevaCuenta);

            _mockVerificacionServicio.Verify(
                servicio => servicio.SolicitarCodigo(nuevaCuenta), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoVerificacion_DelegaAlServicio()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO { TokenCodigo = TokenPrueba };
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO { CodigoEnviado = true };
            _mockVerificacionServicio
                .Setup(servicio => servicio.ReenviarCodigo(solicitud))
                .Returns(resultadoEsperado);

            var resultado = _manejador.ReenviarCodigoVerificacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoVerificacion_LlamaMetodoServicio()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO { TokenCodigo = TokenPrueba };
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO { CodigoEnviado = true };
            _mockVerificacionServicio
                .Setup(servicio => servicio.ReenviarCodigo(solicitud))
                .Returns(resultadoEsperado);

            _manejador.ReenviarCodigoVerificacion(solicitud);

            _mockVerificacionServicio.Verify(
                servicio => servicio.ReenviarCodigo(solicitud), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoVerificacion_DelegaAlServicio()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenPrueba,
                CodigoIngresado = CodigoPrueba
            };
            var resultadoEsperado = new ResultadoRegistroCuentaDTO { RegistroExitoso = true };
            _mockVerificacionServicio
                .Setup(servicio => servicio.ConfirmarCodigo(confirmacion))
                .Returns(resultadoEsperado);

            var resultado = _manejador.ConfirmarCodigoVerificacion(confirmacion);

            Assert.IsTrue(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoVerificacion_LlamaMetodoServicio()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenPrueba,
                CodigoIngresado = CodigoPrueba
            };
            var resultadoEsperado = new ResultadoRegistroCuentaDTO { RegistroExitoso = true };
            _mockVerificacionServicio
                .Setup(servicio => servicio.ConfirmarCodigo(confirmacion))
                .Returns(resultadoEsperado);

            _manejador.ConfirmarCodigoVerificacion(confirmacion);

            _mockVerificacionServicio.Verify(
                servicio => servicio.ConfirmarCodigo(confirmacion), 
                Times.Once);
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
