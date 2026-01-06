using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.Entidades;
using PictionaryMusicalServidor.Datos.Excepciones;
using PictionaryMusicalServidor.Datos.Utilidades;
using System;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Pruebas.Datos
{
    [TestClass]
    public class CatalogoCancionesPruebas
    {
        private const string IdiomaEspanol = "Espanol";
        private const string IdiomaIngles = "Ingles";
        private const string IdiomaEs = "es";
        private const string IdiomaEn = "en";
        private const string IdiomaInvalido = "Aleman";
        private const int IdCancionGasolina = 1;
        private const int IdCancionBlackOrWhite = 21;
        private const int IdCancionInexistente = 999;
        private const int IdCancionNegativo = -1;
        private const int IndiceCero = 0;
        private const string NombreGasolina = "Gasolina";
        private const string IntentoCorrectoGasolina = "gasolina";
        private const string IntentoIncorrecto = "cancion incorrecta";
        private const string IntentoConMayusculas = "GASOLINA";
        private const string IntentoConEspacios = "  gasolina  ";
        private const string IntentoVacio = "";
        private const string IntentoSoloEspacios = "   ";

        private Mock<IGeneradorAleatorio> _mockGenerador;
        private CatalogoCanciones _catalogo;

        [TestInitialize]
        public void Inicializar()
        {
            _mockGenerador = new Mock<IGeneradorAleatorio>();
            _catalogo = new CatalogoCanciones(_mockGenerador.Object);
        }

        #region Constructor

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionGeneradorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CatalogoCanciones(null));
        }

        #endregion

        #region ObtenerCancionAleatoria

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_LanzaExcepcionIdiomaNull()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _catalogo.ObtenerCancionAleatoria(null, new HashSet<int>()));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_LanzaExcepcionIdiomaVacio()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _catalogo.ObtenerCancionAleatoria(IntentoVacio, new HashSet<int>()));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_LanzaExcepcionIdiomaSoloEspacios()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _catalogo.ObtenerCancionAleatoria(IntentoSoloEspacios, new HashSet<int>()));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_RetornaCancionIdiomaEspanol()
        {
            ConfigurarMockSeleccionarPrimerElemento();

            Cancion resultado = _catalogo.ObtenerCancionAleatoria(
                IdiomaEspanol, 
                new HashSet<int>());

            Assert.AreEqual(IdiomaEspanol, resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_RetornaCancionIdiomaIngles()
        {
            ConfigurarMockSeleccionarPrimerElemento();

            Cancion resultado = _catalogo.ObtenerCancionAleatoria(
                IdiomaIngles, 
                new HashSet<int>());

            Assert.AreEqual(IdiomaIngles, resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_MapeoIdiomaEsAEspanol()
        {
            ConfigurarMockSeleccionarPrimerElemento();

            Cancion resultado = _catalogo.ObtenerCancionAleatoria(IdiomaEs, new HashSet<int>());

            Assert.AreEqual(IdiomaEspanol, resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_MapeoIdiomaEnAIngles()
        {
            ConfigurarMockSeleccionarPrimerElemento();

            Cancion resultado = _catalogo.ObtenerCancionAleatoria(IdiomaEn, new HashSet<int>());

            Assert.AreEqual(IdiomaIngles, resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_ExcluyeIdsProporcionados()
        {
            var idsExcluidos = new HashSet<int> { IdCancionGasolina };
            ConfigurarMockSeleccionarPrimerElemento();

            Cancion resultado = _catalogo.ObtenerCancionAleatoria(IdiomaEspanol, idsExcluidos);

            Assert.AreNotEqual(IdCancionGasolina, resultado.Id);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_LanzaExcepcionTodasExcluidas()
        {
            var idsExcluidos = ObtenerTodosLosIdsEspanol();

            Assert.ThrowsException<CancionNoDisponibleExcepcion>(
                () => _catalogo.ObtenerCancionAleatoria(IdiomaEspanol, idsExcluidos));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_LanzaExcepcionIdiomaNoExistente()
        {
            Assert.ThrowsException<CancionNoDisponibleExcepcion>(
                () => _catalogo.ObtenerCancionAleatoria(IdiomaInvalido, new HashSet<int>()));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_AceptaIdsExcluidosNulo()
        {
            ConfigurarMockSeleccionarPrimerElemento();

            Cancion resultado = _catalogo.ObtenerCancionAleatoria(IdiomaEspanol, null);

            Assert.IsTrue(resultado.Id > IndiceCero);
        }

        #endregion

        #region ObtenerCancionPorId

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_RetornaCancionExistente()
        {
            Cancion resultado = _catalogo.ObtenerCancionPorId(IdCancionGasolina);

            Assert.AreEqual(NombreGasolina, resultado.Nombre);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_LanzaExcepcionIdInexistente()
        {
            Assert.ThrowsException<KeyNotFoundException>(
                () => _catalogo.ObtenerCancionPorId(IdCancionInexistente));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_LanzaExcepcionIdNegativo()
        {
            Assert.ThrowsException<KeyNotFoundException>(
                () => _catalogo.ObtenerCancionPorId(IdCancionNegativo));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_RetornaCancionConIdCorrecto()
        {
            Cancion resultado = _catalogo.ObtenerCancionPorId(IdCancionBlackOrWhite);

            Assert.AreEqual(IdCancionBlackOrWhite, resultado.Id);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_RetornaCancionIngles()
        {
            Cancion resultado = _catalogo.ObtenerCancionPorId(IdCancionBlackOrWhite);

            Assert.AreEqual(IdiomaIngles, resultado.Idioma);
        }

        #endregion

        #region ValidarRespuesta

        [TestMethod]
        public void Prueba_ValidarRespuesta_RetornaVerdaderoIntentoCorreecto()
        {
            bool resultado = _catalogo.ValidarRespuesta(
                IdCancionGasolina, 
                IntentoCorrectoGasolina);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RetornaFalsoIntentoIncorrecto()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionGasolina, IntentoIncorrecto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RetornaVerdaderoIgnoraMayusculas()
        {
            bool resultado = _catalogo.ValidarRespuesta(
                IdCancionGasolina, 
                IntentoConMayusculas);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RetornaVerdaderoIgnoraEspaciosExtremos()
        {
            bool resultado = _catalogo.ValidarRespuesta(
                IdCancionGasolina, 
                IntentoConEspacios);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RetornaFalsoIdInexistente()
        {
            bool resultado = _catalogo.ValidarRespuesta(
                IdCancionInexistente, 
                IntentoCorrectoGasolina);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RetornaFalsoIntentoVacio()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionGasolina, IntentoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RetornaFalsoIntentoNull()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionGasolina, null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RetornaFalsoIntentoSoloEspacios()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionGasolina, IntentoSoloEspacios);

            Assert.IsFalse(resultado);
        }

        #endregion

        #region Metodos auxiliares

        private void ConfigurarMockSeleccionarPrimerElemento()
        {
            _mockGenerador
                .Setup(g => g.SeleccionarAleatorio(It.IsAny<IList<Cancion>>()))
                .Returns((IList<Cancion> lista) => lista[IndiceCero]);
        }

        private static HashSet<int> ObtenerTodosLosIdsEspanol()
        {
            var ids = new HashSet<int>();
            for (int i = 1; i <= 20; i++)
            {
                ids.Add(i);
            }
            return ids;
        }

        #endregion
    }
}
