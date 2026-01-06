using System;
using System.Collections.Generic;
using System.ServiceModel;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class ClasificacionManejadorPruebas
    {
        private const int IdUsuarioUno = 1;
        private const int IdUsuarioDos = 2;
        private const int IdJugadorUno = 10;
        private const int IdJugadorDos = 20;
        private const int IdClasificacionUno = 100;
        private const int IdClasificacionDos = 200;
        private const int PuntosJugadorUno = 1000;
        private const int PuntosJugadorDos = 800;
        private const int RondasGanadasUno = 50;
        private const int RondasGanadasDos = 40;
        private const int LimiteTopJugadores = 10;
        private const string NombreUsuarioUno = "JugadorUno";
        private const string NombreUsuarioDos = "JugadorDos";

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private Mock<IClasificacionRepositorio> _mockClasificacionRepositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();
            _mockClasificacionRepositorio = new Mock<IClasificacionRepositorio>();

            _mockContextoFactoria.Setup(factoria => factoria.CrearContexto())
                .Returns(_mockContexto.Object);
            _mockRepositorioFactoria.Setup(factoria => 
                factoria.CrearClasificacionRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_mockClasificacionRepositorio.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ClasificacionManejador(null, _mockRepositorioFactoria.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ClasificacionManejador(_mockContextoFactoria.Object, null));
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_RetornaListaVaciaSinJugadores()
        {
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(new List<Usuario>());
            var manejador = CrearManejador();

            var resultado = manejador.ObtenerTopJugadores();

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_RetornaListaConJugadores()
        {
            var usuarios = CrearListaUsuariosConClasificacion();
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(usuarios);
            var manejador = CrearManejador();

            var resultado = manejador.ObtenerTopJugadores();

            Assert.AreEqual(usuarios.Count, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_RetornaNombreUsuarioCorrecto()
        {
            var usuarios = CrearListaUsuariosConClasificacion();
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(usuarios);
            var manejador = CrearManejador();

            var resultado = manejador.ObtenerTopJugadores();

            Assert.AreEqual(NombreUsuarioUno, resultado[0].Usuario);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_RetornaPuntosCorrecto()
        {
            var usuarios = CrearListaUsuariosConClasificacion();
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(usuarios);
            var manejador = CrearManejador();

            var resultado = manejador.ObtenerTopJugadores();

            Assert.AreEqual(PuntosJugadorUno, resultado[0].Puntos);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_RetornaRondasGanadasCorrecto()
        {
            var usuarios = CrearListaUsuariosConClasificacion();
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(usuarios);
            var manejador = CrearManejador();

            var resultado = manejador.ObtenerTopJugadores();

            Assert.AreEqual(RondasGanadasUno, resultado[0].RondasGanadas);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_LanzaFaultExceptionErrorBaseDatos()
        {
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Throws(new System.Data.Entity.Core.EntityException());
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerTopJugadores());
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_LanzaFaultExceptionErrorDatos()
        {
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Throws(new System.Data.DataException());
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerTopJugadores());
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_LanzaFaultExceptionOperacionInvalida()
        {
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Throws(new InvalidOperationException());
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerTopJugadores());
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_LanzaFaultExceptionErrorInesperado()
        {
            _mockClasificacionRepositorio.Setup(repositorio => 
                repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Throws(new Exception());
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerTopJugadores());
        }

        private ClasificacionManejador CrearManejador()
        {
            return new ClasificacionManejador(
                _mockContextoFactoria.Object, 
                _mockRepositorioFactoria.Object);
        }

        private IList<Usuario> CrearListaUsuariosConClasificacion()
        {
            return new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = IdUsuarioUno,
                    Nombre_Usuario = NombreUsuarioUno,
                    Jugador = new Jugador
                    {
                        idJugador = IdJugadorUno,
                        Clasificacion = new Clasificacion
                        {
                            idClasificacion = IdClasificacionUno,
                            Puntos_Ganados = PuntosJugadorUno,
                            Rondas_Ganadas = RondasGanadasUno
                        }
                    }
                },
                new Usuario
                {
                    idUsuario = IdUsuarioDos,
                    Nombre_Usuario = NombreUsuarioDos,
                    Jugador = new Jugador
                    {
                        idJugador = IdJugadorDos,
                        Clasificacion = new Clasificacion
                        {
                            idClasificacion = IdClasificacionDos,
                            Puntos_Ganados = PuntosJugadorDos,
                            Rondas_Ganadas = RondasGanadasDos
                        }
                    }
                }
            };
        }
    }
}
