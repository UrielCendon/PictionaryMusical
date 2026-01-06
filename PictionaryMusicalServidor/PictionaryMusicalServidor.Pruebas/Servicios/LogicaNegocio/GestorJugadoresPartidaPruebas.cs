using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.Utilidades;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class GestorJugadoresPartidaPruebas
    {
        private const string IdConexionJugadorUno = "conexion-1";
        private const string IdConexionJugadorDos = "conexion-2";
        private const string IdConexionInexistente = "conexion-inexistente";
        private const string NombreJugadorUno = "JugadorUno";
        private const string NombreJugadorDos = "JugadorDos";
        private const int MinimoJugadoresRequeridos = 2;

        private Mock<IGeneradorAleatorio> _mockGenerador;
        private GestorJugadoresPartida _gestor;

        [TestInitialize]
        public void Inicializar()
        {
            _mockGenerador = new Mock<IGeneradorAleatorio>();
            _gestor = new GestorJugadoresPartida(_mockGenerador.Object);
        }

        #region Constructor

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionGeneradorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new GestorJugadoresPartida(null));
        }

        #endregion

        #region HaySuficientesJugadores

        [TestMethod]
        public void Prueba_HaySuficientesJugadores_RetornaFalsoSinJugadores()
        {
            bool resultado = _gestor.HaySuficientesJugadores;

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_HaySuficientesJugadores_RetornaFalsoConUnJugador()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);

            bool resultado = _gestor.HaySuficientesJugadores;

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_HaySuficientesJugadores_RetornaVerdaderoConDosJugadores()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdConexionJugadorDos, NombreJugadorDos, false);

            bool resultado = _gestor.HaySuficientesJugadores;

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Agregar

        [TestMethod]
        public void Prueba_Agregar_AgregaJugadorNuevo()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);

            JugadorPartida jugador = _gestor.Obtener(IdConexionJugadorUno);

            Assert.AreEqual(NombreJugadorUno, jugador.NombreUsuario);
        }

        [TestMethod]
        public void Prueba_Agregar_EstablecePropiedadEsHost()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);

            JugadorPartida jugador = _gestor.Obtener(IdConexionJugadorUno);

            Assert.IsTrue(jugador.EsHost);
        }

        [TestMethod]
        public void Prueba_Agregar_ActualizaJugadorExistente()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, false);
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorDos, true);

            JugadorPartida jugador = _gestor.Obtener(IdConexionJugadorUno);

            Assert.AreEqual(NombreJugadorDos, jugador.NombreUsuario);
        }

        [TestMethod]
        public void Prueba_Agregar_InicializaPuntajeTotalEnCero()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, false);

            JugadorPartida jugador = _gestor.Obtener(IdConexionJugadorUno);

            Assert.AreEqual(0, jugador.PuntajeTotal);
        }

        #endregion

        #region Obtener

        [TestMethod]
        public void Prueba_Obtener_RetornaJugadorExistente()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);

            JugadorPartida resultado = _gestor.Obtener(IdConexionJugadorUno);

            Assert.AreEqual(IdConexionJugadorUno, resultado.IdConexion);
        }

        [TestMethod]
        public void Prueba_Obtener_RetornaNuloJugadorInexistente()
        {
            JugadorPartida resultado = _gestor.Obtener(IdConexionInexistente);

            Assert.IsNull(resultado);
        }

        #endregion

        #region Remover

        [TestMethod]
        public void Prueba_Remover_RetornaVerdaderoJugadorExistente()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            bool eraDibujante;
            string nombreUsuario;

            bool resultado = _gestor.Remover(
                IdConexionJugadorUno, 
                out eraDibujante, 
                out nombreUsuario);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_Remover_RetornaFalsoJugadorInexistente()
        {
            bool eraDibujante;
            string nombreUsuario;

            bool resultado = _gestor.Remover(
                IdConexionInexistente, 
                out eraDibujante, 
                out nombreUsuario);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_Remover_DevuelveNombreUsuario()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            bool eraDibujante;
            string nombreUsuario;

            _gestor.Remover(IdConexionJugadorUno, out eraDibujante, out nombreUsuario);

            Assert.AreEqual(NombreJugadorUno, nombreUsuario);
        }

        [TestMethod]
        public void Prueba_Remover_EliminaJugadorDelGestor()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            bool eraDibujante;
            string nombreUsuario;

            _gestor.Remover(IdConexionJugadorUno, out eraDibujante, out nombreUsuario);

            Assert.IsNull(_gestor.Obtener(IdConexionJugadorUno));
        }

        #endregion

        #region EsHost

        [TestMethod]
        public void Prueba_EsHost_RetornaVerdaderoParaHost()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);

            bool resultado = _gestor.EsHost(IdConexionJugadorUno);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsHost_RetornaFalsoParaNoHost()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, false);

            bool resultado = _gestor.EsHost(IdConexionJugadorUno);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsHost_RetornaFalsoJugadorInexistente()
        {
            bool resultado = _gestor.EsHost(IdConexionInexistente);

            Assert.IsFalse(resultado);
        }

        #endregion

        #region PrepararColaDibujantes

        [TestMethod]
        public void Prueba_PrepararColaDibujantes_MezclaListaJugadores()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdConexionJugadorDos, NombreJugadorDos, false);

            _gestor.PrepararColaDibujantes();

            _mockGenerador.Verify(
                g => g.MezclarLista(It.IsAny<IList<string>>()),
                Times.Once);
        }

        #endregion

        #region SeleccionarSiguienteDibujante

        [TestMethod]
        public void Prueba_SeleccionarSiguienteDibujante_RetornaFalsoColaVacia()
        {
            bool resultado = _gestor.SeleccionarSiguienteDibujante();

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_SeleccionarSiguienteDibujante_RetornaVerdaderoConJugadores()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdConexionJugadorDos, NombreJugadorDos, false);
            _gestor.PrepararColaDibujantes();

            bool resultado = _gestor.SeleccionarSiguienteDibujante();

            Assert.IsTrue(resultado);
        }

        #endregion

        #region TodosAdivinaron

        [TestMethod]
        public void Prueba_TodosAdivinaron_RetornaFalsoSinJugadores()
        {
            bool resultado = _gestor.TodosAdivinaron();

            Assert.IsFalse(resultado);
        }

        #endregion

        #region GenerarClasificacion

        [TestMethod]
        public void Prueba_GenerarClasificacion_RetornaListaOrdenadaPorPuntos()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdConexionJugadorDos, NombreJugadorDos, false);
            var jugadorUno = _gestor.Obtener(IdConexionJugadorUno);
            var jugadorDos = _gestor.Obtener(IdConexionJugadorDos);
            jugadorUno.PuntajeTotal = 50;
            jugadorDos.PuntajeTotal = 100;

            var clasificacion = _gestor.GenerarClasificacion();

            Assert.AreEqual(NombreJugadorDos, clasificacion.First().Usuario);
        }

        [TestMethod]
        public void Prueba_GenerarClasificacion_RetornaListaConTodosLosJugadores()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdConexionJugadorDos, NombreJugadorDos, false);

            var clasificacion = _gestor.GenerarClasificacion();

            Assert.AreEqual(MinimoJugadoresRequeridos, clasificacion.Count);
        }

        #endregion

        #region ObtenerCopiaLista

        [TestMethod]
        public void Prueba_ObtenerCopiaLista_RetornaCantidadCorrecta()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdConexionJugadorDos, NombreJugadorDos, false);

            var copia = _gestor.ObtenerCopiaLista();

            Assert.AreEqual(MinimoJugadoresRequeridos, copia.Count);
        }

        #endregion

        #region QuedanDibujantesPendientes

        [TestMethod]
        public void Prueba_QuedanDibujantesPendientes_RetornaFalsoSinPreparar()
        {
            bool resultado = _gestor.QuedanDibujantesPendientes();

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_QuedanDibujantesPendientes_RetornaVerdaderoConCola()
        {
            _gestor.Agregar(IdConexionJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdConexionJugadorDos, NombreJugadorDos, false);
            _gestor.PrepararColaDibujantes();

            bool resultado = _gestor.QuedanDibujantesPendientes();

            Assert.IsTrue(resultado);
        }

        #endregion
    }
}
