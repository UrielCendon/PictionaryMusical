using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Linq;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class VerificacionRegistroServicioPruebas
    {
        private const string NombreUsuarioValido = "UsuarioTest";
        private const string CorreoValido = "test@correo.com";
        private const string ContrasenaValida = "Password1!";
        private const string NombreValido = "NombreTest";
        private const string ApellidoValido = "ApellidoTest";
        private const string TokenValido = "a1b2c3d4e5f6g7h8a1b2c3d4e5f6g7h8";
        private const string CodigoValido = "123456";
        private const string IdiomaEspanol = "es";
        private const string CadenaVacia = "";
        private const int AvatarIdValido = 1;

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<INotificacionCodigosServicio> _notificacionServicioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private VerificacionRegistroServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _notificacionServicioMock = new Mock<INotificacionCodigosServicio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock
                .Setup(fabrica => fabrica.CrearContexto())
                .Returns(_contextoMock.Object);

            ConfigurarContextoSinDuplicados();

            _servicio = new VerificacionRegistroServicio(
                _contextoFactoriaMock.Object,
                _notificacionServicioMock.Object);
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

        private void ConfigurarContextoSinDuplicados()
        {
            var usuariosVacios = new Usuario[] { }.AsQueryable();
            var jugadoresVacios = new Jugador[] { }.AsQueryable();

            var usuarioDbSetMock = CrearDbSetMock(usuariosVacios);
            var jugadorDbSetMock = CrearDbSetMock(jugadoresVacios);

            _contextoMock.Setup(contexto => contexto.Usuario).Returns(usuarioDbSetMock.Object);
            _contextoMock.Setup(contexto => contexto.Jugador).Returns(jugadorDbSetMock.Object);
        }

        private void ConfigurarContextoConUsuarioDuplicado()
        {
            var usuariosExistentes = new Usuario[]
            {
                new Usuario { Nombre_Usuario = NombreUsuarioValido }
            }.AsQueryable();
            var jugadoresVacios = new Jugador[] { }.AsQueryable();

            var usuarioDbSetMock = CrearDbSetMock(usuariosExistentes);
            var jugadorDbSetMock = CrearDbSetMock(jugadoresVacios);

            _contextoMock.Setup(contexto => contexto.Usuario).Returns(usuarioDbSetMock.Object);
            _contextoMock.Setup(contexto => contexto.Jugador).Returns(jugadorDbSetMock.Object);
        }

        private void ConfigurarContextoConCorreoDuplicado()
        {
            var usuariosVacios = new Usuario[] { }.AsQueryable();
            var jugadoresExistentes = new Jugador[]
            {
                new Jugador { Correo = CorreoValido }
            }.AsQueryable();

            var usuarioDbSetMock = CrearDbSetMock(usuariosVacios);
            var jugadorDbSetMock = CrearDbSetMock(jugadoresExistentes);

            _contextoMock.Setup(contexto => contexto.Usuario).Returns(usuarioDbSetMock.Object);
            _contextoMock.Setup(contexto => contexto.Jugador).Returns(jugadorDbSetMock.Object);
        }

        private static Mock<System.Data.Entity.DbSet<T>> CrearDbSetMock<T>(IQueryable<T> datos)
            where T : class
        {
            var mockSet = new Mock<System.Data.Entity.DbSet<T>>();
            mockSet.As<IQueryable<T>>()
                .Setup(conjunto => conjunto.Provider)
                .Returns(datos.Provider);
            mockSet.As<IQueryable<T>>()
                .Setup(conjunto => conjunto.Expression)
                .Returns(datos.Expression);
            mockSet.As<IQueryable<T>>()
                .Setup(conjunto => conjunto.ElementType)
                .Returns(datos.ElementType);
            mockSet.As<IQueryable<T>>()
                .Setup(conjunto => conjunto.GetEnumerator())
                .Returns(datos.GetEnumerator());
            return mockSet;
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new VerificacionRegistroServicio(null, _notificacionServicioMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_NotificacionServicioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new VerificacionRegistroServicio(_contextoFactoriaMock.Object, null));
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_CuentaNula_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _servicio.SolicitarCodigo(null));
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_UsuarioVacio_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.Usuario = CadenaVacia;

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_CorreoInvalido_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.Correo = "correo_invalido";

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_ContrasenaDebil_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.Contrasena = "123";

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_NombreVacio_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.Nombre = CadenaVacia;

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_ApellidoVacio_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.Apellido = CadenaVacia;

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_UsuarioDuplicado_RetornaConflicto()
        {
            ConfigurarContextoConUsuarioDuplicado();
            var nuevaCuenta = CrearNuevaCuentaValida();

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
            Assert.IsTrue(resultado.UsuarioRegistrado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_CorreoDuplicado_RetornaConflicto()
        {
            ConfigurarContextoConCorreoDuplicado();
            var nuevaCuenta = CrearNuevaCuentaValida();

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
            Assert.IsTrue(resultado.CorreoRegistrado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_NotificacionFalla_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Returns(false);

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigo_FlujoExitoso_RetornaCodigoEnviado()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Returns(true);

            ResultadoSolicitudCodigoDTO resultado = _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsTrue(resultado.CodigoEnviado);
            Assert.IsFalse(string.IsNullOrEmpty(resultado.TokenCodigo));
        }

        [TestMethod]
        public void Prueba_ReenviarCodigo_SolicitudNula_RetornaFallo()
        {
            ResultadoSolicitudCodigoDTO resultado = _servicio.ReenviarCodigo(null);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigo_TokenVacio_RetornaFallo()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = CadenaVacia
            };

            ResultadoSolicitudCodigoDTO resultado = _servicio.ReenviarCodigo(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigo_TokenNoExiste_RetornaFallo()
        {
            var solicitud = new ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = TokenValido
            };

            ResultadoSolicitudCodigoDTO resultado = _servicio.ReenviarCodigo(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigo_ConfirmacionNula_RetornaFallo()
        {
            ResultadoRegistroCuentaDTO resultado = _servicio.ConfirmarCodigo(null);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigo_TokenVacio_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = CadenaVacia,
                CodigoIngresado = CodigoValido
            };

            ResultadoRegistroCuentaDTO resultado = _servicio.ConfirmarCodigo(confirmacion);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigo_CodigoVacio_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CadenaVacia
            };

            ResultadoRegistroCuentaDTO resultado = _servicio.ConfirmarCodigo(confirmacion);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigo_TokenNoExiste_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            ResultadoRegistroCuentaDTO resultado = _servicio.ConfirmarCodigo(confirmacion);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_EstaVerificacionConfirmada_CuentaNula_RetornaFalse()
        {
            bool resultado = _servicio.EstaVerificacionConfirmada(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EstaVerificacionConfirmada_CuentaSinVerificar_RetornaFalse()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            bool resultado = _servicio.EstaVerificacionConfirmada(nuevaCuenta);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_LimpiarVerificacion_CuentaNula_NoLanzaExcepcion()
        {
            _servicio.LimpiarVerificacion(null);

            Assert.IsFalse(_servicio.EstaVerificacionConfirmada(null));
        }

        [TestMethod]
        public void Prueba_FlujoCompleto_SolicitarConfirmarYVerificar_ExitoTotal()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            string tokenCapturado = null;
            string codigoCapturado = null;

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Callback<NotificacionCodigoParametros>(parametros =>
                {
                    codigoCapturado = parametros.Codigo;
                })
                .Returns(true);

            ResultadoSolicitudCodigoDTO resultadoSolicitud = 
                _servicio.SolicitarCodigo(nuevaCuenta);
            tokenCapturado = resultadoSolicitud.TokenCodigo;

            Assert.IsTrue(resultadoSolicitud.CodigoEnviado);

            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = tokenCapturado,
                CodigoIngresado = codigoCapturado
            };

            ResultadoRegistroCuentaDTO resultadoConfirmacion = 
                _servicio.ConfirmarCodigo(confirmacion);

            Assert.IsTrue(resultadoConfirmacion.RegistroExitoso);
            Assert.IsTrue(_servicio.EstaVerificacionConfirmada(nuevaCuenta));

            _servicio.LimpiarVerificacion(nuevaCuenta);

            Assert.IsFalse(_servicio.EstaVerificacionConfirmada(nuevaCuenta));
        }

        // fix el areequal del tokencodigo no es necesario, el flujo debería validar solo codigoenviado
        [TestMethod]
        public void Prueba_FlujoCompleto_SolicitarYReenviar_ExitoTotal()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Returns(true);

            ResultadoSolicitudCodigoDTO resultadoSolicitud = 
                _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsTrue(resultadoSolicitud.CodigoEnviado);

            var solicitudReenvio = new ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = resultadoSolicitud.TokenCodigo
            };

            ResultadoSolicitudCodigoDTO resultadoReenvio = 
                _servicio.ReenviarCodigo(solicitudReenvio);

            Assert.IsTrue(resultadoReenvio.CodigoEnviado);
            Assert.AreEqual(resultadoSolicitud.TokenCodigo, resultadoReenvio.TokenCodigo);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigo_NotificacionFalla_RestauraDatosAnteriores()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            int contadorLlamadas = 0;

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Returns(() =>
                {
                    contadorLlamadas++;
                    return contadorLlamadas == 1;
                });

            ResultadoSolicitudCodigoDTO resultadoSolicitud = 
                _servicio.SolicitarCodigo(nuevaCuenta);

            Assert.IsTrue(resultadoSolicitud.CodigoEnviado);

            var solicitudReenvio = new ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = resultadoSolicitud.TokenCodigo
            };

            ResultadoSolicitudCodigoDTO resultadoReenvio = 
                _servicio.ReenviarCodigo(solicitudReenvio);

            Assert.IsFalse(resultadoReenvio.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigo_CodigoIncorrecto_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            const string codigoIncorrecto = "999999";

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Returns(true);

            ResultadoSolicitudCodigoDTO resultadoSolicitud = 
                _servicio.SolicitarCodigo(nuevaCuenta);

            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = resultadoSolicitud.TokenCodigo,
                CodigoIngresado = codigoIncorrecto
            };

            ResultadoRegistroCuentaDTO resultadoConfirmacion = 
                _servicio.ConfirmarCodigo(confirmacion);

            Assert.IsFalse(resultadoConfirmacion.RegistroExitoso);
        }
    }
}
