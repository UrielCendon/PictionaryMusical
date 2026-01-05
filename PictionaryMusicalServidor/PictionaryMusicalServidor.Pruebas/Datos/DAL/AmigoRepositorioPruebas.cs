using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;
using PictionaryMusicalServidor.Datos.Excepciones;
using Datos.Modelo;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL
{
    [TestClass]
    public class AmigoRepositorioPruebas
    {
        private const int IdUsuarioEmisor = 10;
        private const int IdUsuarioReceptor = 20;
        private const int IdUsuarioTercero = 30;
        private const int IdUsuarioInvalido = 0;
        private const int IdUsuarioNegativo = -5;
        private const int IdUsuarioNoExistente = 99;
        private const int IdUsuarioSinAmigos = 50;
        private const string NombreUsuarioAmigo = "Amigo Test";
        private const string NombreUsuarioDesconocido = "Desconocido";
        private const bool EstadoAceptado = true;
        private const bool EstadoPendiente = false;

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Amigo>> _amigoDbSetMock;
        private Mock<DbSet<Usuario>> _usuarioDbSetMock;
        private AmigoRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _amigoDbSetMock = CrearDbSetMock(new List<Amigo>());
            _usuarioDbSetMock = CrearDbSetMock(new List<Usuario>());

            _contextoMock
                .Setup(contexto => contexto.Amigo)
                .Returns(_amigoDbSetMock.Object);
            _contextoMock
                .Setup(contexto => contexto.Usuario)
                .Returns(_usuarioDbSetMock.Object);

            _repositorio = new AmigoRepositorio(_contextoMock.Object);
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_RelacionExistenteRetornaTrue()
        {
            var datos = new List<Amigo>
            {
                new Amigo
                {
                    UsuarioEmisor = IdUsuarioEmisor,
                    UsuarioReceptor = IdUsuarioReceptor,
                    Estado = EstadoAceptado
                }
            }.AsQueryable();

            ConfigurarDbSet(_amigoDbSetMock, datos);

            bool resultado = _repositorio.ExisteRelacion(IdUsuarioEmisor, IdUsuarioReceptor);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_ErrorConexionLanzaExcepcionPersonalizada()
        {
            _amigoDbSetMock.As<IQueryable<Amigo>>()
                .Setup(consulta => consulta.Provider)
                .Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ExisteRelacion(IdUsuarioEmisor, IdUsuarioReceptor));
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_DatosValidosGuardaCambios()
        {
            _amigoDbSetMock.Setup(conjuntoAmigos => conjuntoAmigos.Add(It.IsAny<Amigo>()))
                .Callback<Amigo>((amigoAgregado) => { });

            var resultado = _repositorio.CrearSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);

            _amigoDbSetMock.Verify(
                conjuntoAmigos => conjuntoAmigos.Add(It.IsAny<Amigo>()),
                Times.Once);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
            Assert.AreEqual(IdUsuarioEmisor, resultado.UsuarioEmisor);
            Assert.AreEqual(IdUsuarioReceptor, resultado.UsuarioReceptor);
            Assert.AreEqual(EstadoPendiente, resultado.Estado);
        }

        [TestMethod]
        public void Prueba_ObtenerRelacion_RelacionNoExistenteLanzaExcepcion()
        {
            var datos = new List<Amigo>().AsQueryable();
            ConfigurarDbSet(_amigoDbSetMock, datos);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioNoExistente));
        }

        [TestMethod]
        public void Prueba_ObtenerRelacion_RelacionInversaRetornaEntidad()
        {
            var datos = new List<Amigo>
            {
                new Amigo
                {
                    UsuarioEmisor = IdUsuarioReceptor,
                    UsuarioReceptor = IdUsuarioEmisor,
                    Estado = EstadoAceptado
                }
            }.AsQueryable();

            ConfigurarDbSet(_amigoDbSetMock, datos);

            var resultado = _repositorio.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor);

            Assert.AreEqual(IdUsuarioReceptor, resultado.UsuarioEmisor);
            Assert.AreEqual(IdUsuarioEmisor, resultado.UsuarioReceptor);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_IdInvalidoLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _repositorio.ObtenerSolicitudesPendientes(IdUsuarioInvalido));
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_UsuarioConPendientesRetornaLista()
        {
            var datos = new List<Amigo>
            {
                new Amigo
                {
                    UsuarioEmisor = IdUsuarioReceptor,
                    UsuarioReceptor = IdUsuarioEmisor,
                    Estado = EstadoPendiente
                },
                new Amigo
                {
                    UsuarioEmisor = IdUsuarioTercero,
                    UsuarioReceptor = IdUsuarioEmisor,
                    Estado = EstadoAceptado
                }
            }.AsQueryable();

            ConfigurarDbSet(_amigoDbSetMock, datos);

            var resultados = _repositorio.ObtenerSolicitudesPendientes(IdUsuarioEmisor);

            Assert.AreEqual(1, resultados.Count);
            Assert.AreEqual(IdUsuarioReceptor, resultados[0].UsuarioEmisor);
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_RelacionNulaLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _repositorio.ActualizarEstado(null, EstadoAceptado));
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_EntidadValidaGuardaCambios()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor,
                Estado = EstadoPendiente
            };

            _repositorio.ActualizarEstado(relacion, EstadoAceptado);

            Assert.AreEqual(EstadoAceptado, relacion.Estado);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_ErrorBaseDatosLanzaExcepcionPersonalizada()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor
            };

            _contextoMock.Setup(contexto => contexto.SaveChanges())
                .Throws(new DbUpdateException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ActualizarEstado(relacion, EstadoAceptado));
        }

        [TestMethod]
        public void Prueba_EliminarRelacion_RelacionNulaLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _repositorio.EliminarRelacion(null));
        }

        [TestMethod]
        public void Prueba_EliminarRelacion_EntidadValidaRemueveYGuarda()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor
            };

            _repositorio.EliminarRelacion(relacion);

            _amigoDbSetMock.Verify(conjuntoAmigos => conjuntoAmigos.Remove(relacion), Times.Once);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_EliminarRelacion_ErrorEntityLanzaExcepcionPersonalizada()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor
            };

            _amigoDbSetMock.Setup(conjuntoAmigos => conjuntoAmigos.Remove(It.IsAny<Amigo>()))
                .Callback((Amigo amigoAEliminar) => { });
            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.EliminarRelacion(relacion));
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_IdInvalidoLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _repositorio.ObtenerAmigos(IdUsuarioNegativo));
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_UsuarioSinAmigosRetornaListaVacia()
        {
            var datosAmigos = new List<Amigo>().AsQueryable();
            ConfigurarDbSet(_amigoDbSetMock, datosAmigos);

            var resultado = _repositorio.ObtenerAmigos(IdUsuarioSinAmigos);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_UsuarioTieneAmigosRetornaListaUsuarios()
        {
            var datosAmigos = new List<Amigo>
            {
                new Amigo
                {
                    UsuarioEmisor = IdUsuarioEmisor,
                    UsuarioReceptor = IdUsuarioReceptor,
                    Estado = EstadoAceptado
                }
            }.AsQueryable();
            ConfigurarDbSet(_amigoDbSetMock, datosAmigos);

            var datosUsuarios = new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = IdUsuarioReceptor,
                    Nombre_Usuario = NombreUsuarioAmigo
                },
                new Usuario
                {
                    idUsuario = IdUsuarioNoExistente,
                    Nombre_Usuario = NombreUsuarioDesconocido
                }
            }.AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datosUsuarios);

            var listaAmigos = _repositorio.ObtenerAmigos(IdUsuarioEmisor);

            Assert.AreEqual(1, listaAmigos.Count);
            Assert.AreEqual(IdUsuarioReceptor, listaAmigos[0].idUsuario);
            Assert.AreEqual(NombreUsuarioAmigo, listaAmigos[0].Nombre_Usuario);
        }

        private static Mock<DbSet<T>> CrearDbSetMock<T>(List<T> datos) where T : class
        {
            var queryable = datos.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>()
                .Setup(consulta => consulta.Provider)
                .Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>()
                .Setup(consulta => consulta.Expression)
                .Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>()
                .Setup(consulta => consulta.ElementType)
                .Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>()
                .Setup(consulta => consulta.GetEnumerator())
                .Returns(() => queryable.GetEnumerator());

            mockSet.Setup(dbSet => dbSet.Include(It.IsAny<string>())).Returns(mockSet.Object);
            mockSet.Setup(dbSet => dbSet.Add(It.IsAny<T>())).Returns<T>(entidad => entidad);

            return mockSet;
        }

        private static void ConfigurarDbSet<T>(
            Mock<DbSet<T>> mockSet,
            IQueryable<T> datos) where T : class
        {
            mockSet.As<IQueryable<T>>().Setup(consulta => consulta.Provider)
                .Returns(datos.Provider);
            mockSet.As<IQueryable<T>>().Setup(consulta => consulta.Expression)
                .Returns(datos.Expression);
            mockSet.As<IQueryable<T>>().Setup(consulta => consulta.ElementType)
                .Returns(datos.ElementType);
            mockSet.As<IQueryable<T>>().Setup(consulta => consulta.GetEnumerator())
                .Returns(() => datos.GetEnumerator());

            mockSet.Setup(dbSet => dbSet.Include(It.IsAny<string>())).Returns(mockSet.Object);
        }
    }
}