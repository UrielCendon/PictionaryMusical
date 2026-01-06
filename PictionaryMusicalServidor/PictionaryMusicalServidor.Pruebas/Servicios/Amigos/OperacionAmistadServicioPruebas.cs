using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Datos.Modelo;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class OperacionAmistadServicioPruebas
    {
        private const int IdUsuarioEmisor = 1;
        private const int IdUsuarioReceptor = 2;
        private const string NombreUsuarioEmisor = "Emisor";
        private const string NombreUsuarioReceptor = "Receptor";
        private const string NombreUsuarioInexistente = "UsuarioNoExiste";

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<IAmistadServicio> _mockAmistadServicio;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
        private OperacionAmistadServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockAmistadServicio = new Mock<IAmistadServicio>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();

            _mockContextoFactoria
                .Setup(f => f.CrearContexto())
                .Returns(_mockContexto.Object);
            _mockRepositorioFactoria
                .Setup(f => f.CrearUsuarioRepositorio(_mockContexto.Object))
                .Returns(_mockUsuarioRepositorio.Object);

            _servicio = new OperacionAmistadServicio(
                _mockContextoFactoria.Object,
                _mockRepositorioFactoria.Object,
                _mockAmistadServicio.Object);
        }

        #region Constructor

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new OperacionAmistadServicio(
                    null, 
                    _mockRepositorioFactoria.Object, 
                    _mockAmistadServicio.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new OperacionAmistadServicio(
                    _mockContextoFactoria.Object, 
                    null, 
                    _mockAmistadServicio.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionAmistadServicioNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new OperacionAmistadServicio(
                    _mockContextoFactoria.Object, 
                    _mockRepositorioFactoria.Object, 
                    null));
        }

        #endregion

        #region ObtenerDatosUsuarioSuscripcion

        [TestMethod]
        public void Prueba_ObtenerDatosUsuarioSuscripcion_LanzaExcepcionUsuarioNoExiste()
        {
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioInexistente))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _servicio.ObtenerDatosUsuarioSuscripcion(NombreUsuarioInexistente));
        }

        [TestMethod]
        public void Prueba_ObtenerDatosUsuarioSuscripcion_RetornaIdUsuarioCorrecto()
        {
            var usuario = CrearUsuarioPrueba(IdUsuarioEmisor, NombreUsuarioEmisor);
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(usuario);

            var resultado = _servicio.ObtenerDatosUsuarioSuscripcion(NombreUsuarioEmisor);

            Assert.AreEqual(IdUsuarioEmisor, resultado.IdUsuario);
        }

        [TestMethod]
        public void Prueba_ObtenerDatosUsuarioSuscripcion_RetornaNombreNormalizado()
        {
            var usuario = CrearUsuarioPrueba(IdUsuarioEmisor, NombreUsuarioEmisor);
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(usuario);

            var resultado = _servicio.ObtenerDatosUsuarioSuscripcion(NombreUsuarioEmisor);

            Assert.AreEqual(NombreUsuarioEmisor, resultado.NombreNormalizado);
        }

        #endregion

        #region EjecutarCreacionSolicitud

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitud_LanzaExcepcionEmisorNoExiste()
        {
            ConfigurarUsuariosParaCreacion(null, NombreUsuarioReceptor);

            Assert.ThrowsException<FaultException>(
                () => _servicio.EjecutarCreacionSolicitud(
                    NombreUsuarioInexistente, 
                    NombreUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitud_LanzaExcepcionReceptorNoExiste()
        {
            var emisor = CrearUsuarioPrueba(IdUsuarioEmisor, NombreUsuarioEmisor);
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(emisor);
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioInexistente))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _servicio.EjecutarCreacionSolicitud(
                    NombreUsuarioEmisor, 
                    NombreUsuarioInexistente));
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitud_RetornaEmisorCorrecto()
        {
            ConfigurarUsuariosParaOperacion();

            var resultado = _servicio.EjecutarCreacionSolicitud(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            Assert.AreEqual(IdUsuarioEmisor, resultado.Emisor.idUsuario);
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitud_RetornaReceptorCorrecto()
        {
            ConfigurarUsuariosParaOperacion();

            var resultado = _servicio.EjecutarCreacionSolicitud(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            Assert.AreEqual(IdUsuarioReceptor, resultado.Receptor.idUsuario);
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitud_LlamaServicioCrearSolicitud()
        {
            ConfigurarUsuariosParaOperacion();

            _servicio.EjecutarCreacionSolicitud(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            _mockAmistadServicio.Verify(
                s => s.CrearSolicitud(IdUsuarioEmisor, IdUsuarioReceptor),
                Times.Once);
        }

        #endregion

        #region EjecutarAceptacionSolicitud

        [TestMethod]
        public void Prueba_EjecutarAceptacionSolicitud_LanzaExcepcionEmisorNoExiste()
        {
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioInexistente))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _servicio.EjecutarAceptacionSolicitud(
                    NombreUsuarioInexistente, 
                    NombreUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_EjecutarAceptacionSolicitud_RetornaNombreEmisorNormalizado()
        {
            ConfigurarUsuariosParaOperacion();

            var resultado = _servicio.EjecutarAceptacionSolicitud(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            Assert.AreEqual(NombreUsuarioEmisor, resultado.NombrePrimerUsuario);
        }

        [TestMethod]
        public void Prueba_EjecutarAceptacionSolicitud_RetornaNombreReceptorNormalizado()
        {
            ConfigurarUsuariosParaOperacion();

            var resultado = _servicio.EjecutarAceptacionSolicitud(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            Assert.AreEqual(NombreUsuarioReceptor, resultado.NombreSegundoUsuario);
        }

        [TestMethod]
        public void Prueba_EjecutarAceptacionSolicitud_LlamaServicioAceptarSolicitud()
        {
            ConfigurarUsuariosParaOperacion();

            _servicio.EjecutarAceptacionSolicitud(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            _mockAmistadServicio.Verify(
                s => s.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor),
                Times.Once);
        }

        #endregion

        #region EjecutarEliminacion

        [TestMethod]
        public void Prueba_EjecutarEliminacion_LanzaExcepcionPrimerUsuarioNoExiste()
        {
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioInexistente))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _servicio.EjecutarEliminacion(
                    NombreUsuarioInexistente, 
                    NombreUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_EjecutarEliminacion_RetornaNombrePrimerUsuarioNormalizado()
        {
            ConfigurarUsuariosParaOperacion();
            ConfigurarRelacionEliminacion();

            var resultado = _servicio.EjecutarEliminacion(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            Assert.AreEqual(NombreUsuarioEmisor, resultado.NombrePrimerUsuario);
        }

        [TestMethod]
        public void Prueba_EjecutarEliminacion_RetornaNombreSegundoUsuarioNormalizado()
        {
            ConfigurarUsuariosParaOperacion();
            ConfigurarRelacionEliminacion();

            var resultado = _servicio.EjecutarEliminacion(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            Assert.AreEqual(NombreUsuarioReceptor, resultado.NombreSegundoUsuario);
        }

        [TestMethod]
        public void Prueba_EjecutarEliminacion_RetornaRelacionEliminada()
        {
            ConfigurarUsuariosParaOperacion();
            var relacion = ConfigurarRelacionEliminacion();

            var resultado = _servicio.EjecutarEliminacion(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            Assert.AreEqual(relacion, resultado.Relacion);
        }

        [TestMethod]
        public void Prueba_EjecutarEliminacion_LlamaServicioEliminarAmistad()
        {
            ConfigurarUsuariosParaOperacion();
            ConfigurarRelacionEliminacion();

            _servicio.EjecutarEliminacion(
                NombreUsuarioEmisor, 
                NombreUsuarioReceptor);

            _mockAmistadServicio.Verify(
                s => s.EliminarAmistad(IdUsuarioEmisor, IdUsuarioReceptor),
                Times.Once);
        }

        #endregion

        #region Metodos auxiliares

        private Usuario CrearUsuarioPrueba(int id, string nombre)
        {
            return new Usuario
            {
                idUsuario = id,
                Nombre_Usuario = nombre
            };
        }

        private void ConfigurarUsuariosParaOperacion()
        {
            var emisor = CrearUsuarioPrueba(IdUsuarioEmisor, NombreUsuarioEmisor);
            var receptor = CrearUsuarioPrueba(IdUsuarioReceptor, NombreUsuarioReceptor);

            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(emisor);
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioReceptor))
                .Returns(receptor);
        }

        private void ConfigurarUsuariosParaCreacion(Usuario emisor, string nombreReceptor)
        {
            _mockUsuarioRepositorio
                .Setup(r => r.ObtenerPorNombreUsuario(It.IsAny<string>()))
                .Returns(emisor);
        }

        private Amigo ConfigurarRelacionEliminacion()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor,
                Estado = true
            };

            _mockAmistadServicio
                .Setup(s => s.EliminarAmistad(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            return relacion;
        }

        #endregion
    }
}
