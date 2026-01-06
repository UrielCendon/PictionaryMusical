using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL
{
    [TestClass]
    public class AmigoRepositorioPruebas
    {
        private const int IdUsuarioPruebaUno = 1;
        private const int IdUsuarioPruebaDos = 2;
        private const int IdUsuarioInvalido = 0;
        private const int IdUsuarioNegativo = -1;

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AmigoRepositorio(null));
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_RetornaVerdaderoSiExisteRelacionComoEmisor()
        {
            var datosAmigos = new List<Amigo>
            {
                new Amigo
                {
                    UsuarioEmisor = IdUsuarioPruebaUno,
                    UsuarioReceptor = IdUsuarioPruebaDos,
                    Estado = true
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosAmigos);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Amigo).Returns(mockDbSet.Object);

            var repositorio = new AmigoRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteRelacion(IdUsuarioPruebaUno, IdUsuarioPruebaDos);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_RetornaVerdaderoSiExisteRelacionComoReceptor()
        {
            var datosAmigos = new List<Amigo>
            {
                new Amigo
                {
                    UsuarioEmisor = IdUsuarioPruebaDos,
                    UsuarioReceptor = IdUsuarioPruebaUno,
                    Estado = true
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosAmigos);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Amigo).Returns(mockDbSet.Object);

            var repositorio = new AmigoRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteRelacion(IdUsuarioPruebaUno, IdUsuarioPruebaDos);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_RetornaFalsoSiNoExisteRelacion()
        {
            var datosAmigos = new List<Amigo>().AsQueryable();

            var mockDbSet = CrearMockDbSet(datosAmigos);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Amigo).Returns(mockDbSet.Object);

            var repositorio = new AmigoRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteRelacion(IdUsuarioPruebaUno, IdUsuarioPruebaDos);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_AgregaSolicitudAlContexto()
        {
            var listaAmigos = new List<Amigo>();
            var mockDbSet = CrearMockDbSetConAdd(listaAmigos);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Amigo).Returns(mockDbSet.Object);

            var repositorio = new AmigoRepositorio(mockContexto.Object);

            repositorio.CrearSolicitud(IdUsuarioPruebaUno, IdUsuarioPruebaDos);

            mockDbSet.Verify(
                dbSet => dbSet.Add(It.IsAny<Amigo>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_GuardaCambiosEnContexto()
        {
            var listaAmigos = new List<Amigo>();
            var mockDbSet = CrearMockDbSetConAdd(listaAmigos);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Amigo).Returns(mockDbSet.Object);

            var repositorio = new AmigoRepositorio(mockContexto.Object);

            repositorio.CrearSolicitud(IdUsuarioPruebaUno, IdUsuarioPruebaDos);

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_RetornaSolicitudConEstadoFalso()
        {
            Amigo solicitudAgregada = null;
            var mockDbSet = new Mock<DbSet<Amigo>>();
            mockDbSet.Setup(dbSet => dbSet.Add(It.IsAny<Amigo>()))
                .Callback<Amigo>(amigo => solicitudAgregada = amigo)
                .Returns<Amigo>(amigo => amigo);

            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Amigo).Returns(mockDbSet.Object);

            var repositorio = new AmigoRepositorio(mockContexto.Object);

            repositorio.CrearSolicitud(IdUsuarioPruebaUno, IdUsuarioPruebaDos);

            Assert.IsFalse(solicitudAgregada.Estado);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_LanzaExcepcionIdInvalido()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new AmigoRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => repositorio.ObtenerSolicitudesPendientes(IdUsuarioInvalido));
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_LanzaExcepcionIdNegativo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new AmigoRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => repositorio.ObtenerSolicitudesPendientes(IdUsuarioNegativo));
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_LanzaExcepcionRelacionNula()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new AmigoRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => repositorio.ActualizarEstado(null, true));
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_CambiaEstadoDeRelacion()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioPruebaUno,
                UsuarioReceptor = IdUsuarioPruebaDos,
                Estado = false
            };

            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new AmigoRepositorio(mockContexto.Object);

            repositorio.ActualizarEstado(relacion, true);

            Assert.IsTrue(relacion.Estado);
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_GuardaCambiosEnContexto()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioPruebaUno,
                UsuarioReceptor = IdUsuarioPruebaDos,
                Estado = false
            };

            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new AmigoRepositorio(mockContexto.Object);

            repositorio.ActualizarEstado(relacion, true);

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_EliminarRelacion_LanzaExcepcionRelacionNula()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new AmigoRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => repositorio.EliminarRelacion(null));
        }

        [TestMethod]
        public void Prueba_EliminarRelacion_EliminaRelacionDelContexto()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioPruebaUno,
                UsuarioReceptor = IdUsuarioPruebaDos,
                Estado = true
            };

            var mockDbSet = new Mock<DbSet<Amigo>>();
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Amigo).Returns(mockDbSet.Object);

            var repositorio = new AmigoRepositorio(mockContexto.Object);

            repositorio.EliminarRelacion(relacion);

            mockDbSet.Verify(dbSet => dbSet.Remove(relacion), Times.Once);
        }

        [TestMethod]
        public void Prueba_EliminarRelacion_GuardaCambiosEnContexto()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioPruebaUno,
                UsuarioReceptor = IdUsuarioPruebaDos,
                Estado = true
            };

            var mockDbSet = new Mock<DbSet<Amigo>>();
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Amigo).Returns(mockDbSet.Object);

            var repositorio = new AmigoRepositorio(mockContexto.Object);

            repositorio.EliminarRelacion(relacion);

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_LanzaExcepcionIdInvalido()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new AmigoRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => repositorio.ObtenerAmigos(IdUsuarioInvalido));
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_LanzaExcepcionIdNegativo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new AmigoRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => repositorio.ObtenerAmigos(IdUsuarioNegativo));
        }

        private Mock<DbSet<Amigo>> CrearMockDbSet(IQueryable<Amigo> datos)
        {
            var mockDbSet = new Mock<DbSet<Amigo>>();
            mockDbSet.As<IQueryable<Amigo>>()
                .Setup(dbSet => dbSet.Provider)
                .Returns(datos.Provider);
            mockDbSet.As<IQueryable<Amigo>>()
                .Setup(dbSet => dbSet.Expression)
                .Returns(datos.Expression);
            mockDbSet.As<IQueryable<Amigo>>()
                .Setup(dbSet => dbSet.ElementType)
                .Returns(datos.ElementType);
            mockDbSet.As<IQueryable<Amigo>>()
                .Setup(dbSet => dbSet.GetEnumerator())
                .Returns(datos.GetEnumerator());
            return mockDbSet;
        }

        private Mock<DbSet<Amigo>> CrearMockDbSetConAdd(List<Amigo> listaAmigos)
        {
            var datos = listaAmigos.AsQueryable();
            var mockDbSet = CrearMockDbSet(datos);
            mockDbSet.Setup(dbSet => dbSet.Add(It.IsAny<Amigo>()))
                .Callback<Amigo>(amigo => listaAmigos.Add(amigo))
                .Returns<Amigo>(amigo => amigo);
            return mockDbSet;
        }
    }
}
