using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class ClasificacionManejadorPruebas
    {
        private const int LimiteTopJugadores = 10;
        private const int IdUsuarioPrimero = 1;
        private const int IdUsuarioSegundo = 2;
        private const int IdUsuarioTercero = 3;
        private const int IdJugadorPrimero = 10;
        private const int IdJugadorSegundo = 20;
        private const int IdJugadorTercero = 30;
        private const int IdClasificacionPrimero = 100;
        private const int IdClasificacionSegundo = 200;
        private const int IdClasificacionTercero = 300;
        private const int PuntosPrimero = 1000;
        private const int PuntosSegundo = 800;
        private const int PuntosTercero = 600;
        private const int RondasPrimero = 50;
        private const int RondasSegundo = 40;
        private const int RondasTercero = 30;
        private const string NombreUsuarioPrimero = "JugadorTop1";
        private const string NombreUsuarioSegundo = "JugadorTop2";
        private const string NombreUsuarioTercero = "JugadorTop3";
        private const int CeroPuntos = 0;
        private const int CeroRondas = 0;

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IClasificacionRepositorio> _clasificacionRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private ClasificacionManejador _manejador;

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

            _manejador = new ClasificacionManejador(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object);
        }

        private Usuario CrearUsuarioConClasificacion(
            int idUsuario,
            string nombreUsuario,
            int idJugador,
            int idClasificacion,
            int puntos,
            int rondas)
        {
            return new Usuario
            {
                idUsuario = idUsuario,
                Nombre_Usuario = nombreUsuario,
                Jugador = new Jugador
                {
                    idJugador = idJugador,
                    Clasificacion = new Clasificacion
                    {
                        idClasificacion = idClasificacion,
                        Puntos_Ganados = puntos,
                        Rondas_Ganadas = rondas
                    }
                }
            };
        }

        private IList<Usuario> CrearListaUsuariosTop()
        {
            return new List<Usuario>
            {
                CrearUsuarioConClasificacion(
                    IdUsuarioPrimero, NombreUsuarioPrimero,
                    IdJugadorPrimero, IdClasificacionPrimero,
                    PuntosPrimero, RondasPrimero),
                CrearUsuarioConClasificacion(
                    IdUsuarioSegundo, NombreUsuarioSegundo,
                    IdJugadorSegundo, IdClasificacionSegundo,
                    PuntosSegundo, RondasSegundo),
                CrearUsuarioConClasificacion(
                    IdUsuarioTercero, NombreUsuarioTercero,
                    IdJugadorTercero, IdClasificacionTercero,
                    PuntosTercero, RondasTercero)
            };
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ClasificacionManejador(null, _repositorioFactoriaMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ClasificacionManejador(_contextoFactoriaMock.Object, null));
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_ListaValida_RetornaClasificacionesCorrectas()
        {
            IList<Usuario> usuariosTop = CrearListaUsuariosTop();
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(usuariosTop);

            IList<ClasificacionUsuarioDTO> resultado = _manejador.ObtenerTopJugadores();

            Assert.AreEqual(usuariosTop.Count, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_ListaValida_RetornaPrimerJugadorCorrecto()
        {
            IList<Usuario> usuariosTop = CrearListaUsuariosTop();
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(usuariosTop);

            IList<ClasificacionUsuarioDTO> resultado = _manejador.ObtenerTopJugadores();

            Assert.AreEqual(NombreUsuarioPrimero, resultado[0].Usuario);
            Assert.AreEqual(PuntosPrimero, resultado[0].Puntos);
            Assert.AreEqual(RondasPrimero, resultado[0].RondasGanadas);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_ListaValida_RetornaSegundoJugadorCorrecto()
        {
            IList<Usuario> usuariosTop = CrearListaUsuariosTop();
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(usuariosTop);

            IList<ClasificacionUsuarioDTO> resultado = _manejador.ObtenerTopJugadores();

            Assert.AreEqual(NombreUsuarioSegundo, resultado[1].Usuario);
            Assert.AreEqual(PuntosSegundo, resultado[1].Puntos);
            Assert.AreEqual(RondasSegundo, resultado[1].RondasGanadas);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_ListaVacia_RetornaListaVacia()
        {
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(new List<Usuario>());

            IList<ClasificacionUsuarioDTO> resultado = _manejador.ObtenerTopJugadores();

            Assert.AreEqual(CeroPuntos, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_ClasificacionConPuntosNulos_RetornaCeroPuntos()
        {
            var usuario = new Usuario
            {
                idUsuario = IdUsuarioPrimero,
                Nombre_Usuario = NombreUsuarioPrimero,
                Jugador = new Jugador
                {
                    idJugador = IdJugadorPrimero,
                    Clasificacion = new Clasificacion
                    {
                        idClasificacion = IdClasificacionPrimero,
                        Puntos_Ganados = null,
                        Rondas_Ganadas = null
                    }
                }
            };
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(new List<Usuario> { usuario });

            IList<ClasificacionUsuarioDTO> resultado = _manejador.ObtenerTopJugadores();

            Assert.AreEqual(CeroPuntos, resultado[0].Puntos);
            Assert.AreEqual(CeroRondas, resultado[0].RondasGanadas);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_EntityException_LanzaFaultException()
        {
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Throws(new EntityException());

            Assert.ThrowsException<FaultException>(() => _manejador.ObtenerTopJugadores());
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_DataException_LanzaFaultException()
        {
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Throws(new DataException());

            Assert.ThrowsException<FaultException>(() => _manejador.ObtenerTopJugadores());
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_InvalidOperationException_LanzaFaultException()
        {
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Throws(new InvalidOperationException());

            Assert.ThrowsException<FaultException>(() => _manejador.ObtenerTopJugadores());
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_ExcepcionGenerica_LanzaFaultException()
        {
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Throws(new Exception());

            Assert.ThrowsException<FaultException>(() => _manejador.ObtenerTopJugadores());
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_MultiplesJugadores_OrdenPreservado()
        {
            IList<Usuario> usuariosTop = CrearListaUsuariosTop();
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(usuariosTop);

            IList<ClasificacionUsuarioDTO> resultado = _manejador.ObtenerTopJugadores();

            Assert.IsTrue(resultado[0].Puntos >= resultado[1].Puntos);
            Assert.IsTrue(resultado[1].Puntos >= resultado[2].Puntos);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_RepositorioLlamadoConLimiteCorrecto()
        {
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(new List<Usuario>());

            _manejador.ObtenerTopJugadores();

            _clasificacionRepositorioMock.Verify(
                repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ObtenerTopJugadores_ContextoDispuesto_LlamaCrearContexto()
        {
            _clasificacionRepositorioMock
                .Setup(repositorio => repositorio.ObtenerMejoresJugadores(LimiteTopJugadores))
                .Returns(new List<Usuario>());

            _manejador.ObtenerTopJugadores();

            _contextoFactoriaMock.Verify(
                fabrica => fabrica.CrearContexto(),
                Times.Once);
        }
    }
}
