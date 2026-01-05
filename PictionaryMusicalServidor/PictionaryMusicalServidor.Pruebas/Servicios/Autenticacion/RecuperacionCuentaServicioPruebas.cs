using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Datos.Excepciones;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class RecuperacionCuentaServicioPruebas
    {
        private const string IdentificadorValido = "usuariotest";
        private const string CorreoValido = "test@correo.com";
        private const string TokenValido = "a1b2c3d4e5f6g7h8a1b2c3d4e5f6g7h8";
        private const string CodigoValido = "123456";
        private const string ContrasenaValida = "Password1!";
        private const string ContrasenaInvalida = "123";
        private const string IdiomaEspanol = "es";
        private const string IdentificadorVacio = "";
        private const string NombreUsuarioValido = "UsuarioTest";
        private const int IdUsuarioValido = 1;
        private const int IdJugadorValido = 10;

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<INotificacionCodigosServicio> _notificacionServicioMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private RecuperacionCuentaServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _notificacionServicioMock = new Mock<INotificacionCodigosServicio>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock
                .Setup(fabrica => fabrica.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearUsuarioRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _servicio = new RecuperacionCuentaServicio(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object,
                _notificacionServicioMock.Object);
        }

        private Usuario CrearUsuarioValido()
        {
            return new Usuario
            {
                idUsuario = IdUsuarioValido,
                Nombre_Usuario = NombreUsuarioValido,
                Jugador = new Jugador
                {
                    idJugador = IdJugadorValido,
                    Correo = CorreoValido
                }
            };
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new RecuperacionCuentaServicio(
                    null,
                    _repositorioFactoriaMock.Object,
                    _notificacionServicioMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new RecuperacionCuentaServicio(
                    _contextoFactoriaMock.Object,
                    null,
                    _notificacionServicioMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_NotificacionServicioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new RecuperacionCuentaServicio(
                    _contextoFactoriaMock.Object,
                    _repositorioFactoriaMock.Object,
                    null));
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_SolicitudNula_RetornaFallo()
        {
            ResultadoSolicitudRecuperacionDTO resultado = 
                _servicio.SolicitarCodigoRecuperacion(null);

            Assert.IsFalse(resultado.CodigoEnviado);
            Assert.IsFalse(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_IdentificadorVacio_RetornaFallo()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorVacio,
                Idioma = IdiomaEspanol
            };

            ResultadoSolicitudRecuperacionDTO resultado = 
                _servicio.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_UsuarioNoExiste_RetornaFallo()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido,
                Idioma = IdiomaEspanol
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreConJugador(IdentificadorValido))
                .Throws(new BaseDatosExcepcion(
                    "No encontrado",
                    new KeyNotFoundException()));

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreo(IdentificadorValido))
                .Throws(new BaseDatosExcepcion(
                    "No encontrado",
                    new KeyNotFoundException()));

            ResultadoSolicitudRecuperacionDTO resultado = 
                _servicio.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
            Assert.IsFalse(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_ErrorBaseDatos_RetornaFallo()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido,
                Idioma = IdiomaEspanol
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreConJugador(IdentificadorValido))
                .Throws(new BaseDatosExcepcion("Error de conexion"));

            ResultadoSolicitudRecuperacionDTO resultado = 
                _servicio.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_NotificacionFalla_RetornaFallo()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido,
                Idioma = IdiomaEspanol
            };
            var usuario = CrearUsuarioValido();

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreConJugador(IdentificadorValido))
                .Returns(usuario);

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Returns(false);

            ResultadoSolicitudRecuperacionDTO resultado = 
                _servicio.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        // fix el areequal del correodestino no es necesario el flujo debería validar solo codigoenviado como resultado principal
        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_FlujoExitoso_RetornaCodigoEnviado()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = IdentificadorValido,
                Idioma = IdiomaEspanol
            };
            var usuario = CrearUsuarioValido();

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreConJugador(IdentificadorValido))
                .Returns(usuario);

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Returns(true);

            ResultadoSolicitudRecuperacionDTO resultado = 
                _servicio.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
            Assert.IsTrue(resultado.CuentaEncontrada);
            Assert.AreEqual(CorreoValido, resultado.CorreoDestino);
        }

        // fix el flujo debería validar solo CodigoEnviado
        [TestMethod]
        public void Prueba_SolicitarCodigoRecuperacion_BusquedaPorCorreo_RetornaCodigoEnviado()
        {
            var solicitud = new SolicitudRecuperarCuentaDTO
            {
                Identificador = CorreoValido,
                Idioma = IdiomaEspanol
            };
            var usuario = CrearUsuarioValido();

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreConJugador(CorreoValido))
                .Throws(new BaseDatosExcepcion(
                    "No encontrado",
                    new KeyNotFoundException()));

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreo(CorreoValido))
                .Returns(usuario);

            _notificacionServicioMock
                .Setup(servicio => servicio.EnviarNotificacion(
                    It.IsAny<NotificacionCodigoParametros>()))
                .Returns(true);

            ResultadoSolicitudRecuperacionDTO resultado = 
                _servicio.SolicitarCodigoRecuperacion(solicitud);

            Assert.IsTrue(resultado.CodigoEnviado);
            Assert.IsTrue(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoRecuperacion_SolicitudNula_RetornaFallo()
        {
            ResultadoSolicitudCodigoDTO resultado = 
                _servicio.ReenviarCodigoRecuperacion(null);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoRecuperacion_TokenVacio_RetornaFallo()
        {
            var solicitud = new ReenvioCodigoDTO
            {
                TokenCodigo = IdentificadorVacio
            };

            ResultadoSolicitudCodigoDTO resultado = 
                _servicio.ReenviarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ReenviarCodigoRecuperacion_TokenNoExiste_RetornaFallo()
        {
            var solicitud = new ReenvioCodigoDTO
            {
                TokenCodigo = TokenValido
            };

            ResultadoSolicitudCodigoDTO resultado = 
                _servicio.ReenviarCodigoRecuperacion(solicitud);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_ConfirmacionNula_RetornaFallo()
        {
            ResultadoOperacionDTO resultado = 
                _servicio.ConfirmarCodigoRecuperacion(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_TokenVacio_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = IdentificadorVacio,
                CodigoIngresado = CodigoValido
            };

            ResultadoOperacionDTO resultado = 
                _servicio.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_CodigoVacio_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = IdentificadorVacio
            };

            ResultadoOperacionDTO resultado = 
                _servicio.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ConfirmarCodigoRecuperacion_TokenNoExiste_RetornaFallo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValido,
                CodigoIngresado = CodigoValido
            };

            ResultadoOperacionDTO resultado = 
                _servicio.ConfirmarCodigoRecuperacion(confirmacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_SolicitudNula_RetornaFallo()
        {
            ResultadoOperacionDTO resultado = _servicio.ActualizarContrasena(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_TokenVacio_RetornaFallo()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = IdentificadorVacio,
                NuevaContrasena = ContrasenaValida
            };

            ResultadoOperacionDTO resultado = _servicio.ActualizarContrasena(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_ContrasenaInvalida_RetornaFallo()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenValido,
                NuevaContrasena = ContrasenaInvalida
            };

            ResultadoOperacionDTO resultado = _servicio.ActualizarContrasena(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_TokenNoExiste_RetornaFallo()
        {
            var solicitud = new ActualizacionContrasenaDTO
            {
                TokenCodigo = TokenValido,
                NuevaContrasena = ContrasenaValida
            };

            ResultadoOperacionDTO resultado = _servicio.ActualizarContrasena(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }
    }
}
