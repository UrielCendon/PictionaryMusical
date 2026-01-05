using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using System;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class NotificacionCodigosServicioPruebas
    {
        private const string CorreoValido = "test@correo.com";
        private const string CodigoValido = "123456";
        private const string CorreoVacio = "";
        private const string CodigoVacio = "";
        private const string IdiomaEspanol = "es";

        private Mock<ICodigoVerificacionNotificador> _notificadorMock;
        private NotificacionCodigosServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _notificadorMock = new Mock<ICodigoVerificacionNotificador>();
            _servicio = new NotificacionCodigosServicio(_notificadorMock.Object);
        }

        private NotificacionCodigoParametros CrearParametrosValidos()
        {
            return new NotificacionCodigoParametros
            {
                CorreoDestino = CorreoValido,
                Codigo = CodigoValido,
                Idioma = IdiomaEspanol
            };
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_FlujoExitoso_RetornaTrue()
        {
            var parametros = CrearParametrosValidos();

            _notificadorMock
                .Setup(notificador => notificador.NotificarAsync(parametros))
                .ReturnsAsync(true);

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_NotificadorFalla_RetornaFalse()
        {
            var parametros = CrearParametrosValidos();

            _notificadorMock
                .Setup(notificador => notificador.NotificarAsync(parametros))
                .ReturnsAsync(false);

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_ParametrosNulos_RetornaFalse()
        {
            bool resultado = _servicio.EnviarNotificacion(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_CorreoVacio_RetornaFalse()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = CorreoVacio,
                Codigo = CodigoValido
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_CodigoVacio_RetornaFalse()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = CorreoValido,
                Codigo = CodigoVacio
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_CorreoNulo_RetornaFalse()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = null,
                Codigo = CodigoValido
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_CodigoNulo_RetornaFalse()
        {
            var parametros = new NotificacionCodigoParametros
            {
                CorreoDestino = CorreoValido,
                Codigo = null
            };

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_FaultException_RelanzaExcepcion()
        {
            var parametros = CrearParametrosValidos();

            _notificadorMock
                .Setup(notificador => notificador.NotificarAsync(parametros))
                .Throws(new FaultException());

            Assert.ThrowsException<FaultException>(() =>
                _servicio.EnviarNotificacion(parametros));
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_ExcepcionInvalidOperation_RetornaFalse()
        {
            var parametros = CrearParametrosValidos();

            _notificadorMock
                .Setup(notificador => notificador.NotificarAsync(parametros))
                .Throws(new InvalidOperationException());

            bool resultado = _servicio.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EnviarNotificacion_NotificadorNulo_RetornaFalse()
        {
            var servicioConNotificadorNulo = new NotificacionCodigosServicio(null);
            var parametros = CrearParametrosValidos();

            bool resultado = servicioConNotificadorNulo.EnviarNotificacion(parametros);

            Assert.IsFalse(resultado);
        }
    }
}
