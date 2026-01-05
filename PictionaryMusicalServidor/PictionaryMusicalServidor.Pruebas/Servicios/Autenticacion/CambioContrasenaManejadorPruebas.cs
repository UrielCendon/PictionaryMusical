using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using System;
using System.Data;
using System.Data.Entity.Core;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class CambioContrasenaManejadorPruebas
    {
        private const string IdentificadorValido = "usuario@test.com";
        private const string TokenValido = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6";
        private const string CodigoValido = "123456";
        private const string ContrasenaValida = "Password1!";
        private const string MensajeExito = "Operacion exitosa";
        private const string MensajeError = "Error en la operacion";
        private const string CorreoDestino = "usuario@correo.com";
        private const string IdiomaEspanol = "es";

        private Mock<IRecuperacionCuentaServicio> _recuperacionServicioMock;
        private CambioContrasenaManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _recuperacionServicioMock = new Mock<IRecuperacionCuentaServicio>();
            _manejador = new CambioContrasenaManejador(_recuperacionServicioMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_ServicioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new CambioContrasenaManejador(null));
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_FlujoExitoso_RetornaCodigoEnviado()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido,
                Idioma = IdiomaEspanol
            };

            var resultadoEsperado = new ResultadoSolicitudRecuperacionDTO
            {
                CuentaEncontrada = true,
                CodigoEnviado = true,
                CorreoDestino = CorreoDestino,
                TokenCodigo = TokenValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Returns(resultadoEsperado);

            ResultadoSolicitudRecuperacionDTO resultado = 
                _manejador.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
            Assert.IsTrue(resultado.CuentaEncontrada);
            Assert.AreEqual(CorreoDestino, resultado.CorreoDestino);
            Assert.AreEqual(TokenValido, resultado.TokenCodigo);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_CuentaNoEncontrada_RetornaCodigoNoEnviado()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido,
                Idioma = IdiomaEspanol
            };

            var resultadoEsperado = new ResultadoSolicitudRecuperacionDTO
            {
                CuentaEncontrada = false,
                CodigoEnviado = false,
                Mensaje = MensajeError
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Returns(resultadoEsperado);

            ResultadoSolicitudRecuperacionDTO resultado = 
                _manejador.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
            Assert.IsFalse(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_ArgumentoNulo_RetornaFallo()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Throws(new ArgumentNullException(
                    "El argumento solicitud no puede ser nulo",
                    innerException: null));

            ResultadoSolicitudRecuperacionDTO resultado = 
                _manejador.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_ErrorBaseDatos_RetornaFallo()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Throws(new EntityException());

            ResultadoSolicitudRecuperacionDTO resultado = 
                _manejador.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_ErrorDatos_RetornaFallo()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Throws(new DataException());

            ResultadoSolicitudRecuperacionDTO resultado = 
                _manejador.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_FaultException_RelanzaExcepcion()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Throws(new FaultException());

            Assert.ThrowsException<FaultException>(() =>
                _manejador.SolicitarCodigoRecuperacion(solicitud));
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoRecuperacion_FlujoExitoso_RetornaCodigoEnviado()
        {
            var solicitud = new ReenvioCodigoDTO
            {
                TokenCodigo = TokenValido
            };

            var resultadoEsperado = new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ReenviarCodigoRecuperacion(solicitud))
                .Returns(resultadoEsperado);

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.ReenviarCodigoRecuperacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoRecuperacion_TokenInvalido_RetornaFallo()
        {
            var solicitud = new ReenvioCodigoDTO
            {
                TokenCodigo = TokenValido
            };

            var resultadoEsperado = new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                Mensaje = MensajeError
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ReenviarCodigoRecuperacion(solicitud))
                .Returns(resultadoEsperado);

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.ReenviarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoRecuperacion_ErrorBaseDatos_RetornaFallo()
        {
            var solicitud = new ReenvioCodigoDTO
            {
                TokenCodigo = TokenValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ReenviarCodigoRecuperacion(solicitud))
                .Throws(new EntityException());

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.ReenviarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_FlujoExitoso_RetornaOperacionExitosa()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            var resultadoEsperado = new ResultadoOperacionDTO
            {
                OperacionExitosa = true,
                Mensaje = MensajeExito
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ConfirmarCodigoRecuperacion(confirmacion))
                .Returns(resultadoEsperado);

            ResultadoOperacionDTO resultado = 
                _manejador.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_CodigoIncorrecto_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            var resultadoEsperado = new ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = MensajeError
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ConfirmarCodigoRecuperacion(confirmacion))
                .Returns(resultadoEsperado);

            ResultadoOperacionDTO resultado = 
                _manejador.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_ErrorBaseDatos_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ConfirmarCodigoRecuperacion(confirmacion))
                .Throws(new EntityException());

            ResultadoOperacionDTO resultado = 
                _manejador.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_FlujoExitoso_RetornaOperacionExitosa()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenValido,
                NuevaContrasena = ContrasenaValida
            };

            var resultadoEsperado = new ResultadoOperacionDTO
            {
                OperacionExitosa = true,
                Mensaje = MensajeExito
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ActualizarContrasena(solicitud))
                .Returns(resultadoEsperado);

            ResultadoOperacionDTO resultado = _manejador.ActualizarContrasena(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_TokenExpirado_RetornaFallo()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenValido,
                NuevaContrasena = ContrasenaValida
            };

            var resultadoEsperado = new ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = MensajeError
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ActualizarContrasena(solicitud))
                .Returns(resultadoEsperado);

            ResultadoOperacionDTO resultado = _manejador.ActualizarContrasena(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_ArgumentoNulo_RetornaFallo()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenValido,
                NuevaContrasena = ContrasenaValida
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ActualizarContrasena(solicitud))
                .Throws(new ArgumentNullException(
                    "El argumento solicitud no puede ser nulo",
                    innerException: null));

            ResultadoOperacionDTO resultado = _manejador.ActualizarContrasena(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_ErrorBaseDatos_RetornaFallo()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenValido,
                NuevaContrasena = ContrasenaValida
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ActualizarContrasena(solicitud))
                .Throws(new EntityException());

            ResultadoOperacionDTO resultado = _manejador.ActualizarContrasena(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_ErrorDatos_RetornaFallo()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenValido,
                NuevaContrasena = ContrasenaValida
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ActualizarContrasena(solicitud))
                .Throws(new DataException());

            ResultadoOperacionDTO resultado = _manejador.ActualizarContrasena(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_ExcepcionGenerica_RetornaFallo()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenValido,
                NuevaContrasena = ContrasenaValida
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ActualizarContrasena(solicitud))
                .Throws(new Exception());

            ResultadoOperacionDTO resultado = _manejador.ActualizarContrasena(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }
    }
}
