using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class ActualizadorClasificacionPartidaPruebas
    {
        private const string IdConexionPrueba = "conexion-123";
        private const string NombreUsuarioPrueba = "Usuario1";

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private ActualizadorClasificacionPartida _actualizador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _actualizador = new ActualizadorClasificacionPartida(
                _mockContextoFactoria.Object,
                _mockRepositorioFactoria.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ActualizadorClasificacionPartida(
                    null,
                    _mockRepositorioFactoria.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ActualizadorClasificacionPartida(
                    _mockContextoFactoria.Object,
                    null));
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_NoHaceNadaConJugadoresNulo()
        {
            var resultado = new ResultadoPartidaDTO
            {
                Clasificacion = new List<ClasificacionUsuarioDTO>()
            };

            _actualizador.ActualizarClasificaciones(null, resultado);

            _mockContextoFactoria.Verify(
                factoria => factoria.CrearContexto(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_NoHaceNadaConListaVacia()
        {
            var jugadores = new List<JugadorPartida>();
            var resultado = new ResultadoPartidaDTO
            {
                Clasificacion = new List<ClasificacionUsuarioDTO>()
            };

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _mockContextoFactoria.Verify(
                factoria => factoria.CrearContexto(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_NoHaceNadaConResultadoNulo()
        {
            var jugadores = CrearListaJugadoresPrueba();

            _actualizador.ActualizarClasificaciones(jugadores, null);

            _mockContextoFactoria.Verify(
                factoria => factoria.CrearContexto(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_NoHaceNadaConClasificacionNula()
        {
            var jugadores = CrearListaJugadoresPrueba();
            var resultado = new ResultadoPartidaDTO { Clasificacion = null };

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _mockContextoFactoria.Verify(
                factoria => factoria.CrearContexto(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_NoHaceNadaConClasificacionVacia()
        {
            var jugadores = CrearListaJugadoresPrueba();
            var resultado = new ResultadoPartidaDTO
            {
                Clasificacion = new List<ClasificacionUsuarioDTO>()
            };

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _mockContextoFactoria.Verify(
                factoria => factoria.CrearContexto(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ActualizarClasificaciones_NoHaceNadaConMensajeDeError()
        {
            var jugadores = CrearListaJugadoresPrueba();
            var resultado = new ResultadoPartidaDTO
            {
                Clasificacion = new List<ClasificacionUsuarioDTO>
                {
                    new ClasificacionUsuarioDTO()
                },
                Mensaje = "Error previo"
            };

            _actualizador.ActualizarClasificaciones(jugadores, resultado);

            _mockContextoFactoria.Verify(
                factoria => factoria.CrearContexto(),
                Times.Never);
        }

        private static List<JugadorPartida> CrearListaJugadoresPrueba()
        {
            return new List<JugadorPartida>
            {
                new JugadorPartida
                {
                    IdConexion = IdConexionPrueba,
                    NombreUsuario = NombreUsuarioPrueba
                }
            };
        }
    }
}
