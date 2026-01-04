using System;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;
using PictionaryMusicalServidor.Datos.Excepciones;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL.Implementaciones
{
    [TestClass]
    public class ClasificacionRepositorioPruebas
    {
        private const int IdJugadorInexistente = 999;
        private const int PuntosPrueba = 100;

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private ClasificacionRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
        }

        [TestCleanup]
        public void Limpiar()
        {
            _repositorio = null;
            _contextoMock = null;
        }

        #region Constructor

        [TestMethod]
        public void Prueba_ConstructorContextoNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ClasificacionRepositorio(null);
            });
        }

        #endregion

        #region ActualizarEstadisticas

        [TestMethod]
        public void Prueba_ActualizarEstadisticasJugadorInexistente_LanzaBaseDatosExcepcion()
        {
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ActualizarEstadisticas(IdJugadorInexistente, PuntosPrueba, false);
            });
        }

        #endregion
    }
}
