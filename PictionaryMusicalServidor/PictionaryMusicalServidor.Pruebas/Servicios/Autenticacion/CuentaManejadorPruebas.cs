using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class CuentaManejadorPruebas
    {
        private const string NombreUsuarioValido = "UsuarioTest";
        private const string CorreoValido = "test@correo.com";
        private const string ContrasenaValida = "Password1!";
        private const string NombreValido = "NombreTest";
        private const string ApellidoValido = "ApellidoTest";
        private const string TokenValido = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6";
        private const string CodigoValido = "123456";
        private const string MensajeExito = "Operacion exitosa";
        private const string MensajeError = "Error en operacion";
        private const string IdiomaEspanol = "es";
        private const int AvatarIdValido = 1;
        private const int AvatarIdInvalido = 0;
        private const int IdClasificacionCreada = 1;
        private const int IdJugadorCreado = 10;
        private const int IdUsuarioCreado = 100;

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IVerificacionRegistroServicio> _verificacionServicioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<IClasificacionRepositorio> _clasificacionRepositorioMock;
        private Mock<IJugadorRepositorio> _jugadorRepositorioMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private CuentaManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _verificacionServicioMock = new Mock<IVerificacionRegistroServicio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _clasificacionRepositorioMock = new Mock<IClasificacionRepositorio>();
            _jugadorRepositorioMock = new Mock<IJugadorRepositorio>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();

            _contextoFactoriaMock
                .Setup(fabrica => fabrica.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearClasificacionRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_clasificacionRepositorioMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearJugadorRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_jugadorRepositorioMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearUsuarioRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _manejador = new CuentaManejador(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object,
                _verificacionServicioMock.Object);
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
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new CuentaManejador(
                    null, 
                    _repositorioFactoriaMock.Object, 
                    _verificacionServicioMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new CuentaManejador(
                    _contextoFactoriaMock.Object, 
                    null, 
                    _verificacionServicioMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_VerificacionServicioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new CuentaManejador(
                    _contextoFactoriaMock.Object, 
                    _repositorioFactoriaMock.Object, 
                    null));
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_CuentaNula_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _manejador.RegistrarCuenta(null));
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_UsuarioVacio_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.Usuario = string.Empty;

            ResultadoRegistroCuentaDTO resultado = _manejador.RegistrarCuenta(nuevaCuenta);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_CorreoInvalido_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.Correo = "correo_invalido";

            ResultadoRegistroCuentaDTO resultado = _manejador.RegistrarCuenta(nuevaCuenta);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_ContrasenaDebil_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.Contrasena = "123";

            ResultadoRegistroCuentaDTO resultado = _manejador.RegistrarCuenta(nuevaCuenta);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_CuentaNoVerificada_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _verificacionServicioMock
                .Setup(servicio => servicio.EstaVerificacionConfirmada(nuevaCuenta))
                .Returns(false);

            ConfigurarContextoSinDuplicados();

            ResultadoRegistroCuentaDTO resultado = _manejador.RegistrarCuenta(nuevaCuenta);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_AvatarInvalido_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            nuevaCuenta.AvatarId = AvatarIdInvalido;

            _verificacionServicioMock
                .Setup(servicio => servicio.EstaVerificacionConfirmada(nuevaCuenta))
                .Returns(true);

            ConfigurarContextoSinDuplicados();

            ResultadoRegistroCuentaDTO resultado = _manejador.RegistrarCuenta(nuevaCuenta);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_ErrorBaseDatos_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _verificacionServicioMock
                .Setup(servicio => servicio.EstaVerificacionConfirmada(nuevaCuenta))
                .Returns(true);

            ConfigurarContextoSinDuplicados();
            ConfigurarTransaccionMock();

            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.CrearClasificacionInicial())
                .Throws(new EntityException());

            ResultadoRegistroCuentaDTO resultado = _manejador.RegistrarCuenta(nuevaCuenta);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

        [TestMethod]
        public void Prueba_RegistrarCuenta_ErrorDatos_RetornaFallo()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();

            _verificacionServicioMock
                .Setup(servicio => servicio.EstaVerificacionConfirmada(nuevaCuenta))
                .Returns(true);

            ConfigurarContextoSinDuplicados();
            ConfigurarTransaccionMock();

            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.CrearClasificacionInicial())
                .Throws(new DataException());

            ResultadoRegistroCuentaDTO resultado = _manejador.RegistrarCuenta(nuevaCuenta);

            Assert.IsFalse(resultado.RegistroExitoso);
        }

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
        public void Prueba_SolicitarCodigoVerificacion_UsuarioExistente_RetornaConflicto()
        {
            var nuevaCuenta = CrearNuevaCuentaValida();
            var resultadoEsperado = new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                UsuarioRegistrado = true
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

        private void ConfigurarContextoSinDuplicados()
        {
            var usuariosVacios = new Usuario[] { }.AsQueryable();
            var jugadoresVacios = new Jugador[] { }.AsQueryable();

            var usuarioDbSetMock = CrearDbSetMock(usuariosVacios);
            var jugadorDbSetMock = CrearDbSetMock(jugadoresVacios);

            _contextoMock.Setup(contexto => contexto.Usuario).Returns(usuarioDbSetMock.Object);
            _contextoMock.Setup(contexto => contexto.Jugador).Returns(jugadorDbSetMock.Object);
        }

        private void ConfigurarTransaccionMock()
        {
            var transaccionMock = new Mock<System.Data.Entity.DbContextTransaction>();
            var databaseMock = new Mock<System.Data.Entity.Database>();

            _contextoMock
                .Setup(contexto => contexto.Database)
                .Returns(databaseMock.Object);
        }

        private static Mock<System.Data.Entity.DbSet<T>> CrearDbSetMock<T>(IQueryable<T> datos) 
            where T : class
        {
            var mockSet = new Mock<System.Data.Entity.DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(datos.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(datos.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(datos.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator())
                .Returns(datos.GetEnumerator());
            return mockSet;
        }
    }
}
