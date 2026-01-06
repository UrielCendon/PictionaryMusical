using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Datos.Modelo;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class AmistadServicioPruebas
    {
        private const int IdUsuarioEmisor = 1;
        private const int IdUsuarioReceptor = 2;
        private const int IdUsuarioInexistente = 999;
        private const string NombreUsuarioEmisor = "Emisor";
        private const string NombreUsuarioReceptor = "Receptor";
        private const string NombreUsuarioPrueba = "UsuarioPrueba";

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private Mock<IAmigoRepositorio> _mockAmigoRepositorio;
        private AmistadServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();
            _mockAmigoRepositorio = new Mock<IAmigoRepositorio>();

            _mockContextoFactoria
                .Setup(f => f.CrearContexto())
                .Returns(_mockContexto.Object);
            _mockRepositorioFactoria
                .Setup(f => f.CrearAmigoRepositorio(_mockContexto.Object))
                .Returns(_mockAmigoRepositorio.Object);

            _servicio = new AmistadServicio(
                _mockContextoFactoria.Object,
                _mockRepositorioFactoria.Object);
        }

        #region Constructor

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AmistadServicio(null, _mockRepositorioFactoria.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AmistadServicio(_mockContextoFactoria.Object, null));
        }

        #endregion

        #region ObtenerSolicitudesPendientesDTO

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesDTO_RetornaListaVaciaSinSolicitudes()
        {
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerSolicitudesPendientes(IdUsuarioEmisor))
                .Returns(new List<Amigo>());

            var resultado = _servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioEmisor);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesDTO_RetornaListaVaciaSiNulo()
        {
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerSolicitudesPendientes(IdUsuarioEmisor))
                .Returns((IList<Amigo>)null);

            var resultado = _servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioEmisor);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesDTO_RetornaSolicitudesCorrectas()
        {
            var solicitudes = CrearListaSolicitudesPrueba();
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerSolicitudesPendientes(IdUsuarioReceptor))
                .Returns(solicitudes);

            var resultado = _servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioReceptor);

            Assert.AreEqual(1, resultado.Count);
        }

        #endregion

        #region CrearSolicitud

        [TestMethod]
        public void Prueba_CrearSolicitud_LanzaExcepcionMismoUsuario()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => _servicio.CrearSolicitud(IdUsuarioEmisor, IdUsuarioEmisor));
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_LanzaExcepcionRelacionExistente()
        {
            _mockAmigoRepositorio
                .Setup(r => r.ExisteRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(true);

            Assert.ThrowsException<InvalidOperationException>(
                () => _servicio.CrearSolicitud(IdUsuarioEmisor, IdUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_CreaCorrectamenteSolicitud()
        {
            _mockAmigoRepositorio
                .Setup(r => r.ExisteRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(false);

            _servicio.CrearSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);

            _mockAmigoRepositorio.Verify(
                r => r.CrearSolicitud(IdUsuarioEmisor, IdUsuarioReceptor),
                Times.Once);
        }

        #endregion

        #region AceptarSolicitud

        [TestMethod]
        public void Prueba_AceptarSolicitud_LanzaExcepcionRelacionNoExiste()
        {
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns((Amigo)null);

            Assert.ThrowsException<InvalidOperationException>(
                () => _servicio.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_AceptarSolicitud_LanzaExcepcionReceptorIncorrecto()
        {
            var relacion = CrearRelacionPrueba();
            relacion.UsuarioReceptor = IdUsuarioInexistente;
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            Assert.ThrowsException<InvalidOperationException>(
                () => _servicio.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_AceptarSolicitud_LanzaExcepcionSolicitudYaAceptada()
        {
            var relacion = CrearRelacionPrueba();
            relacion.Estado = true;
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            Assert.ThrowsException<InvalidOperationException>(
                () => _servicio.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_AceptarSolicitud_ActualizaEstadoCorrectamente()
        {
            var relacion = CrearRelacionPrueba();
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            _servicio.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);

            _mockAmigoRepositorio.Verify(
                r => r.ActualizarEstado(relacion, true),
                Times.Once);
        }

        #endregion

        #region EliminarAmistad

        [TestMethod]
        public void Prueba_EliminarAmistad_LanzaExcepcionMismoUsuario()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => _servicio.EliminarAmistad(IdUsuarioEmisor, IdUsuarioEmisor));
        }

        [TestMethod]
        public void Prueba_EliminarAmistad_LanzaExcepcionRelacionNoExiste()
        {
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns((Amigo)null);

            Assert.ThrowsException<InvalidOperationException>(
                () => _servicio.EliminarAmistad(IdUsuarioEmisor, IdUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_EliminarAmistad_EliminaRelacionCorrectamente()
        {
            var relacion = CrearRelacionPrueba();
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            _servicio.EliminarAmistad(IdUsuarioEmisor, IdUsuarioReceptor);

            _mockAmigoRepositorio.Verify(
                r => r.EliminarRelacion(relacion),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EliminarAmistad_RetornaRelacionEliminada()
        {
            var relacion = CrearRelacionPrueba();
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            var resultado = _servicio.EliminarAmistad(IdUsuarioEmisor, IdUsuarioReceptor);

            Assert.AreEqual(relacion, resultado);
        }

        #endregion

        #region ObtenerAmigosDTO

        [TestMethod]
        public void Prueba_ObtenerAmigosDTO_RetornaListaVaciaSiNulo()
        {
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerAmigos(IdUsuarioEmisor))
                .Returns((List<Usuario>)null);

            var resultado = _servicio.ObtenerAmigosDTO(IdUsuarioEmisor);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigosDTO_RetornaListaAmigosCorrecta()
        {
            var amigos = CrearListaAmigosPrueba();
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerAmigos(IdUsuarioEmisor))
                .Returns(amigos);

            var resultado = _servicio.ObtenerAmigosDTO(IdUsuarioEmisor);

            Assert.AreEqual(1, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigosDTO_RetornaNombreUsuarioCorrecto()
        {
            var amigos = CrearListaAmigosPrueba();
            _mockAmigoRepositorio
                .Setup(r => r.ObtenerAmigos(IdUsuarioEmisor))
                .Returns(amigos);

            var resultado = _servicio.ObtenerAmigosDTO(IdUsuarioEmisor);

            Assert.AreEqual(NombreUsuarioReceptor, resultado[0].NombreUsuario);
        }

        #endregion

        #region Metodos auxiliares

        private List<Amigo> CrearListaSolicitudesPrueba()
        {
            return new List<Amigo>
            {
                new Amigo
                {
                    UsuarioEmisor = IdUsuarioEmisor,
                    UsuarioReceptor = IdUsuarioReceptor,
                    Estado = false,
                    Usuario = new Usuario 
                    { 
                        idUsuario = IdUsuarioEmisor, 
                        Nombre_Usuario = NombreUsuarioEmisor 
                    },
                    Usuario1 = new Usuario 
                    { 
                        idUsuario = IdUsuarioReceptor, 
                        Nombre_Usuario = NombreUsuarioReceptor 
                    }
                }
            };
        }

        private List<Usuario> CrearListaAmigosPrueba()
        {
            return new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = IdUsuarioReceptor,
                    Nombre_Usuario = NombreUsuarioReceptor
                }
            };
        }

        private Amigo CrearRelacionPrueba()
        {
            return new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor,
                Estado = false
            };
        }

        #endregion
    }
}
