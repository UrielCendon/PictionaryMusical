using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.Utilidades;
using PictionaryMusicalServidor.Datos.Entidades;
using PictionaryMusicalServidor.Datos.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PictionaryMusicalServidor.Pruebas.Datos
{
    [TestClass]
    public class CatalogoCancionesPruebas
    {
        private const string IdiomaEspanol = "Espanol";
        private const string IdiomaIngles = "Ingles";
        private const string IdiomaMixto = "Mixto";
        private const string IdiomaInvalido = "";
        private const string CodigoIdiomaEspanol = "es-MX";
        private const int IdCancionEspanol = 1;
        private const int IdCancionIngles = 21;
        private const int IdCancionInexistente = 999;
        private const string NombreCancionEspanol = "Gasolina";
        private const string IntentoCorrecto = "Gasolina";
        private const string IntentoConMayusculas = "GASOLINA";
        private const string IntentoConEspacios = " Gasolina ";
        private const string IntentoIncorrecto = "Despacito";
        private const string IntentoVacio = "";

        private Mock<IGeneradorAleatorio> _generadorMock;
        private CatalogoCanciones _catalogo;

        [TestInitialize]
        public void Inicializar()
        {
            _generadorMock = new Mock<IGeneradorAleatorio>();
            _catalogo = new CatalogoCanciones(_generadorMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_GeneradorNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new CatalogoCanciones(null));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaVacioLanzaExcepcion()
        {
            var idsExcluidos = new HashSet<int>();

            Assert.ThrowsException<ArgumentException>(() =>
                _catalogo.ObtenerCancionAleatoria(IdiomaInvalido, idsExcluidos));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaEspanolInvocaGenerador()
        {
            var idsExcluidos = new HashSet<int>();
            var cancionEsperada = new Cancion
            { 
                Id = IdCancionEspanol, 
                Idioma = IdiomaEspanol 
            };

            _generadorMock
                .Setup(generador => 
                    generador.SeleccionarAleatorio(It.IsAny<IList<Cancion>>()))
                .Returns(cancionEsperada);

            var resultado = _catalogo.ObtenerCancionAleatoria(IdiomaEspanol, idsExcluidos);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(IdiomaEspanol, resultado.Idioma);
            _generadorMock.Verify(g => g.SeleccionarAleatorio(It.IsAny<IList<Cancion>>()), Times.Once);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaInglesInvocaGenerador()
        {
            var idsExcluidos = new HashSet<int>();
            var cancionEsperada = new Cancion
            { 
                Id = IdCancionIngles, 
                Idioma = IdiomaIngles 
            };

            _generadorMock
                .Setup(generador => 
                    generador.SeleccionarAleatorio(It.IsAny<IList<Cancion>>()))
                .Returns(cancionEsperada);

            var resultado = _catalogo.ObtenerCancionAleatoria(IdiomaIngles, idsExcluidos);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(IdiomaIngles, resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_MapeoIdiomaCodigoFunciona()
        {
            var idsExcluidos = new HashSet<int>();
            var cancionEsperada = new Cancion
            { 
                Id = IdCancionEspanol, 
                Idioma = IdiomaEspanol 
            };

            _generadorMock
                .Setup(generador => 
                    generador.SeleccionarAleatorio(It.IsAny<IList<Cancion>>()))
                .Returns(cancionEsperada);

            var resultado = _catalogo.ObtenerCancionAleatoria(CodigoIdiomaEspanol, idsExcluidos);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(IdiomaEspanol, resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_SinCancionesDisponiblesLanzaExcepcion()
        {
            var idsExcluidos = new HashSet<int>(Enumerable.Range(1, 20));

            Assert.ThrowsException<CancionNoDisponibleExcepcion>(() =>
                _catalogo.ObtenerCancionAleatoria(IdiomaEspanol, idsExcluidos));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaMixtoInvocaGenerador()
        {
            var idsExcluidos = new HashSet<int>();
            var cancionEsperada = new Cancion
            { 
                Id = IdCancionEspanol, 
                Idioma = IdiomaEspanol 
            };

            _generadorMock
                .Setup(generador => 
                    generador.SeleccionarAleatorio(It.IsAny<IList<Cancion>>()))
                .Returns(cancionEsperada);

            var resultado = _catalogo.ObtenerCancionAleatoria(IdiomaMixto, idsExcluidos);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_IdExistenteRetornaCancion()
        {
            var resultado = _catalogo.ObtenerCancionPorId(IdCancionEspanol);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(IdCancionEspanol, resultado.Id);
            Assert.AreEqual(NombreCancionEspanol, resultado.Nombre);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_IdInexistenteLanzaExcepcion()
        {
            Assert.ThrowsException<KeyNotFoundException>(() =>
                _catalogo.ObtenerCancionPorId(IdCancionInexistente));
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IdInexistenteRetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionInexistente, IntentoCorrecto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IntentoVacioRetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionEspanol, IntentoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IntentoCorrectoRetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionEspanol, IntentoCorrecto);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IntentoConMayusculasRetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionEspanol, IntentoConMayusculas);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IntentoConEspaciosRetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionEspanol, IntentoConEspacios);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IntentoIncorrectoRetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(IdCancionEspanol, IntentoIncorrecto);

            Assert.IsFalse(resultado);
        }
    }
}