using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Datos.Excepciones;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class ActualizadorClasificacionPartidaPruebas
    {
        private const string IdConexionJugadorUno = "1";
        private const string IdConexionJugadorDos = "2";
        private const string IdConexionJugadorTres = "3";
        private const string IdConexionNoNumerico = "abc";
        private const string NombreJugadorUno = "JugadorUno";
        private const string NombreJugadorDos = "JugadorDos";
        private const string NombreJugadorTres = "JugadorTres";
        private const int PuntajeAlto = 100;
        private const int PuntajeMedio = 50;
        private const int PuntajeBajo = 25;
        private const int IdJugadorUno = 1;
        private const int IdJugadorDos = 2;
        private const string MensajePartidaAbortada = "Partida abortada";

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IClasificacionRepositorio> _clasificacionRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private ActualizadorClasificacionPartida _actualizador;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _clasificacionRepositorioMock = new Mock<IClasificacionRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock
                .Setup(fabrica => fabrica.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearClasificacionRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_clasificacionRepositorioMock.Object);

            _actualizador = new ActualizadorClasificacionPartida(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object);
        }

        private JugadorPartida CrearJugador(
            string idConexion,
            string nombreUsuario,
            int puntajeTotal)
        {
            return new JugadorPartida
            {
                IdConexion = idConexion,
                NombreUsuario = nombreUsuario,
                PuntajeTotal = puntajeTotal
            };
        }

        private ResultadoPartidaDTO CrearResultadoValido()
        {
            return new ResultadoPartidaDTO
            {
                Clasificacion = new List<ClasificacionUsuarioDTO>
                {
                    new ClasificacionUsuarioDTO
                    {
                        Usuario = NombreJugadorUno,
                        Puntos = PuntajeAlto
                    }
                },
                Mensaje = null
            };
        }

        private List<JugadorPartida> CrearListaUnJugador(string idConexion, int puntaje)
        {
            return new List<JugadorPartida>
            {
                CrearJugador(idConexion, NombreJugadorUno, puntaje)
            };
        }

        private void ConfigurarExcepcionEnRepositorio(Exception excepcion)
        {
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ActualizarEstadisticas(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()))
                .Throws(excepcion);
        }

        private void VerificarNingunaActualizacionRealizada()
        {
            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstadisticas(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ActualizadorClasificacionPartida(null, _repositorioFactoriaMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ActualizadorClasificacionPartida(_contextoFactoriaMock.Object, null));
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_JugadoresNuloOVacio_NoPersiste()
        {
            var resultado = CrearResultadoValido();

            _actualizador.ActualizarClasificaciones(null, resultado);
            _actualizador.ActualizarClasificaciones(new List<JugadorPartida>(), resultado);

            VerificarNingunaActualizacionRealizada();
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_ResultadoNulo_NoPersiste()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);

            _actualizador.ActualizarClasificaciones(jugadores, null);

            VerificarNingunaActualizacionRealizada();
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_ClasificacionNulaOVacia_NoPersiste()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);
            var resultadoClasificacionNula = new ResultadoPartidaDTO
            {
                Clasificacion = null,
                Mensaje = null
            };
            var resultadoClasificacionVacia = new ResultadoPartidaDTO
            {
                Clasificacion = new List<ClasificacionUsuarioDTO>(),
                Mensaje = null
            };

            _actualizador.ActualizarClasificaciones(jugadores, resultadoClasificacionNula);
            _actualizador.ActualizarClasificaciones(jugadores, resultadoClasificacionVacia);

            VerificarNingunaActualizacionRealizada();
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_ConMensajeError_NoPersiste()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);
            var resultado = new ResultadoPartidaDTO
            {
                Clasificacion = new List<ClasificacionUsuarioDTO>
                {
                    new ClasificacionUsuarioDTO
                    {
                        Usuario = NombreJugadorUno,
                        Puntos = PuntajeAlto
                    }
                },
                Mensaje = MensajePartidaAbortada
            };

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            VerificarNingunaActualizacionRealizada();
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_JugadorValido_ActualizaComoGanador()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);
            var resultado = CrearResultadoValido();

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstadisticas(
                    IdJugadorUno,
                    PuntajeAlto,
                    true),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_DosJugadores_ActualizaGanadorYPerdedor()
        {
            var jugadores = new List<JugadorPartida>
            {
                CrearJugador(IdConexionJugadorUno, NombreJugadorUno, PuntajeAlto),
                CrearJugador(IdConexionJugadorDos, NombreJugadorDos, PuntajeBajo)
            };
            var resultado = CrearResultadoValido();

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstadisticas(
                    IdJugadorUno,
                    PuntajeAlto,
                    true),
                Times.Once);

            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstadisticas(
                    IdJugadorDos,
                    PuntajeBajo,
                    false),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_EmpateEnPuntaje_AmbosGanadores()
        {
            var jugadores = new List<JugadorPartida>
            {
                CrearJugador(IdConexionJugadorUno, NombreJugadorUno, PuntajeAlto),
                CrearJugador(IdConexionJugadorDos, NombreJugadorDos, PuntajeAlto)
            };
            var resultado = CrearResultadoValido();

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstadisticas(
                    It.IsAny<int>(),
                    PuntajeAlto,
                    true),
                Times.Exactly(2));
        }

        [TestMethod]
        [DataRow("abc", DisplayName = "IdConexion no numerico")]
        [DataRow("0", DisplayName = "IdConexion cero")]
        [DataRow("-1", DisplayName = "IdConexion negativo")]
        public void Prueba_ActualizarClasificaciones_IdConexionInvalido_IgnoraJugador(
            string idConexionInvalido)
        {
            var jugadores = new List<JugadorPartida>
            {
                CrearJugador(idConexionInvalido, NombreJugadorUno, PuntajeAlto)
            };
            var resultado = CrearResultadoValido();

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            VerificarNingunaActualizacionRealizada();
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_ExcepcionBaseDatos_AsignaMensajeError()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);
            var resultado = CrearResultadoValido();
            ConfigurarExcepcionEnRepositorio(new BaseDatosExcepcion("Error de base de datos"));

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            Assert.IsFalse(string.IsNullOrEmpty(resultado.Mensaje));
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_DbUpdateException_AsignaMensajeError()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);
            var resultado = CrearResultadoValido();
            ConfigurarExcepcionEnRepositorio(new DbUpdateException("Error de actualizacion"));

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            Assert.IsFalse(string.IsNullOrEmpty(resultado.Mensaje));
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_EntityException_AsignaMensajeError()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);
            var resultado = CrearResultadoValido();
            ConfigurarExcepcionEnRepositorio(new EntityException("Error de entidad"));

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            Assert.IsFalse(string.IsNullOrEmpty(resultado.Mensaje));
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_DataException_AsignaMensajeError()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);
            var resultado = CrearResultadoValido();
            ConfigurarExcepcionEnRepositorio(new DataException("Error de datos"));

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            Assert.IsFalse(string.IsNullOrEmpty(resultado.Mensaje));
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_ExcepcionGeneral_AsignaMensajeError()
        {
            var jugadores = CrearListaUnJugador(IdConexionJugadorUno, PuntajeAlto);
            var resultado = CrearResultadoValido();
            ConfigurarExcepcionEnRepositorio(new Exception("Error inesperado"));

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            Assert.IsFalse(string.IsNullOrEmpty(resultado.Mensaje));
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_TresJugadores_ActualizaTodos()
        {
            var jugadores = new List<JugadorPartida>
            {
                CrearJugador(IdConexionJugadorUno, NombreJugadorUno, PuntajeAlto),
                CrearJugador(IdConexionJugadorDos, NombreJugadorDos, PuntajeMedio),
                CrearJugador(IdConexionJugadorTres, NombreJugadorTres, PuntajeBajo)
            };
            var resultado = CrearResultadoValido();

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstadisticas(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()),
                Times.Exactly(3));
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_JugadorValidoEInvalido_SoloActualizaValido()
        {
            var jugadores = new List<JugadorPartida>
            {
                CrearJugador(IdConexionJugadorUno, NombreJugadorUno, PuntajeAlto),
                CrearJugador(IdConexionNoNumerico, NombreJugadorDos, PuntajeBajo)
            };
            var resultado = CrearResultadoValido();

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstadisticas(
                    IdJugadorUno,
                    PuntajeAlto,
                    true),
                Times.Once);

            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstadisticas(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()),
                Times.Once);
        }
    }
}
