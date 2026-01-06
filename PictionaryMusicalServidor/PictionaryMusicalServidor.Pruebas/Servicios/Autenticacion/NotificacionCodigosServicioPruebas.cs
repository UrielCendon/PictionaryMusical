using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class NotificacionCodigosServicioPruebas
    {
        private const string CorreoDestinoPrueba = "test@correo.com";
        private const string CodigoPrueba = "123456";
        private const string UsuarioDestinoPrueba = "UsuarioPrueba";
        private const string IdiomaPrueba = "es";

        private Mock<ICodigoVerificacionNotificador> _mockNotificador;
        private NotificacionCodigosServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _mockNotificador = new Mock<ICodigoVerificacionNotificador>();
            _servicio = new NotificacionCodigosServicio(_mockNotificador.Object);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_RetornaFalsoParametrosNulos()
        {
            bool resultado = _servicio.EnviarNotificacion(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_RetornaFalsoCorreoNulo()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = null,
                Codigo = CodigoPrueba
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_RetornaFalsoCorreoVacio()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = string.Empty,
                Codigo = CodigoPrueba
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_RetornaFalsoCorreoSoloEspacios()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = "   ",
                Codigo = CodigoPrueba
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_RetornaFalsoCodigoNulo()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = CorreoDestinoPrueba,
                Codigo = null
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_RetornaFalsoCodigoVacio()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = CorreoDestinoPrueba,
                Codigo = string.Empty
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_RetornaFalsoCodigoSoloEspacios()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = CorreoDestinoPrueba,
                Codigo = "   "
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }
    }
}
