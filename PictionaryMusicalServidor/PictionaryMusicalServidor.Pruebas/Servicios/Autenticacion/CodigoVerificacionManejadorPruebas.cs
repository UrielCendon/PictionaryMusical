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
    public class CodigoVerificacionManejadorPruebas
    {
        private const string NombreUsuarioValido = "UsuarioTest";
        private const string CorreoValido = "test@correo.com";
        private const string ContrasenaValida = "Password1!";
        private const string NombreValido = "NombreTest";
        private const string ApellidoValido = "ApellidoTest";
        private const string TokenValido = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6";
        private const string CodigoValido = "123456";
        private const string IdentificadorValido = "usuario@test.com";
        private const string MensajeExito = "Operacion exitosa";
        private const string MensajeError = "Error en operacion";
        private const string IdiomaEspanol = "es";
        private const int AvatarIdValido = 1;

        private Mock<IVerificacionRegistroServicio> _verificacionServicioMock;
        private Mock<IRecuperacionCuentaServicio> _recuperacionServicioMock;
        private CodigoVerificacionManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _verificacionServicioMock = new Mock<IVerificacionRegistroServicio>();
            _recuperacionServicioMock = new Mock<IRecuperacionCuentaServicio>();
            _manejador = new CodigoVerificacionManejador(
                _verificacionServicioMock.Object,
                _recuperacionServicioMock.Object);
        }

        private NuevaCuentaDTO CrearNuevaCuentaValida()
        {
            return new NuevaCuentaDTO
            {
                Usuario = NombreUsuarioValido,
                Correo = CorreoValido,
                Contrasena = ContrasenaValida,
                Nombre = NombreValido,
                Apellido = ApellidoValido,
                AvatarId = AvatarIdValido,
                Idioma = IdiomaEspanol
            };
        }

        [TestMethod]
        public void Prueba_Constructor_VerificacionServicioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new CodigoVerificacionManejador(null, _recuperacionServicioMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RecuperacionServicioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new CodigoVerificacionManejador(_verificacionServicioMock.Object, null));
        }

        // fix el flujo debería validar solo CodigoEnviado como resultado principal
        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_FlujoExitoso_RetornaCodigoEnviado()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true,
                TokenCodigo = TokenValido
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigo(nuevaCuenta))
                .Returns(resultadoEsperado);

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.SolicitarCodigoVerificacion(nuevaCuenta);

            Assert.IsTrue(resultado.CodigoEnviado);
            Assert.AreEqual(TokenValido, resultado.TokenCodigo);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_UsuarioDuplicado_RetornaConflicto()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                UsuarioRegistrado = true,
                Mensaje = MensajeError
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigo(nuevaCuenta))
                .Returns(resultadoEsperado);

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.SolicitarCodigoVerificacion(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
            Assert.IsTrue(resultado.UsuarioRegistrado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_ArgumentoNulo_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _verificacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigo(nuevaCuenta))
                .Throws(new ArgumentNullException(
                    "El argumento nuevaCuenta no puede ser nulo",
                    innerException: null));

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.SolicitarCodigoVerificacion(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_ErrorBaseDatos_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _verificacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigo(nuevaCuenta))
                .Throws(new EntityException());

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.SolicitarCodigoVerificacion(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoVerificacion_FaultException_RelanzaExcepcion()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _verificacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigo(nuevaCuenta))
                .Throws(new FaultException());

            Assert.ThrowsException<FaultException>(() =>
                _manejador.SolicitarCodigoVerificacion(nuevaCuenta));
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoVerificacion_FlujoExitoso_RetornaCodigoEnviado()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = TokenValido
            };

            var resultadoEsperado = new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.ReenviarCodigo(solicitud))
                .Returns(resultadoEsperado);

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.ReenviarCodigoVerificacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoVerificacion_TokenInvalido_RetornaFallo()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = TokenValido
            };

            var resultadoEsperado = new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                Mensaje = MensajeError
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.ReenviarCodigo(solicitud))
                .Returns(resultadoEsperado);

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.ReenviarCodigoVerificacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoVerificacion_ErrorBaseDatos_RetornaFallo()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = TokenValido
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.ReenviarCodigo(solicitud))
                .Throws(new EntityException());

            ResultadoSolicitudCodigoDTO resultado = 
                _manejador.ReenviarCodigoVerificacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoVerificacion_FaultException_RelanzaExcepcion()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = TokenValido
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.ReenviarCodigo(solicitud))
                .Throws(new FaultException());

            Assert.ThrowsException<FaultException>(() =>
                _manejador.ReenviarCodigoVerificacion(solicitud));
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoVerificacion_FlujoExitoso_RetornaRegistroExitoso()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            var resultadoEsperado = new ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = true
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.ConfirmarCodigo(confirmacion))
                .Returns(resultadoEsperado);

            ResultadoRegistroCuentaDTO resultado = 
                _manejador.ConfirmarCodigoVerificacion(confirmacion);

            Assert.IsTrue(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoVerificacion_CodigoIncorrecto_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            var resultadoEsperado = new ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = false,
                Mensaje = MensajeError
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.ConfirmarCodigo(confirmacion))
                .Returns(resultadoEsperado);

            ResultadoRegistroCuentaDTO resultado = 
                _manejador.ConfirmarCodigoVerificacion(confirmacion);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoVerificacion_ErrorBaseDatos_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            _verificacionServicioMock
                .Setup(servicio => servicio.ConfirmarCodigo(confirmacion))
                .Throws(new EntityException());

            ResultadoRegistroCuentaDTO resultado = 
                _manejador.ConfirmarCodigoVerificacion(confirmacion);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        // fix el flujo debería validar solo codigoenviado
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
                TokenCodigo = TokenValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.SolicitarCodigoRecuperacion(solicitud))
                .Returns(resultadoEsperado);

            ResultadoSolicitudRecuperacionDTO resultado = 
                _manejador.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
            Assert.IsTrue(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_CuentaNoEncontrada_RetornaFallo()
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
        public void Prueba_ConfirmarCodigoRecuperacion_ErrorDatos_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ConfirmarCodigoRecuperacion(confirmacion))
                .Throws(new DataException());

            ResultadoOperacionDTO resultado = 
                _manejador.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_ExcepcionGenerica_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            _recuperacionServicioMock
                .Setup(servicio => servicio.ConfirmarCodigoRecuperacion(confirmacion))
                .Throws(new Exception());

            ResultadoOperacionDTO resultado = 
                _manejador.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }
    }
}
