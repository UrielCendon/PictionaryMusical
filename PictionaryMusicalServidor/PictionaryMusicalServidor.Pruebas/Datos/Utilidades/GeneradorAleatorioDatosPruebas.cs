using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Datos
{
    /// <summary>
    /// Contiene pruebas unitarias para la clase <see cref="GeneradorAleatorioDatos"/>.
    /// Valida la generacion de indices aleatorios, seleccion de elementos y mezcla de listas.
    /// </summary>
    [TestClass]
    public class GeneradorAleatorioDatosPruebas
    {
        private const int TamanoColeccionValido = 10;
        private const int TamanoColeccionCero = 0;
        private const int TamanoColeccionNegativo = -5;
        private const int IndiceMinimo = 0;
        private const string ElementoUno = "A";
        private const string ElementoDos = "B";
        private const string ElementoTres = "C";

        private GeneradorAleatorioDatos _generador;

        [TestInitialize]
        public void Inicializar()
        {
            _generador = new GeneradorAleatorioDatos();
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoValidoRetornaIndiceEnRango()
        {
            int resultado = _generador.ObtenerIndiceAleatorio(TamanoColeccionValido);

            Assert.IsTrue(resultado >= IndiceMinimo);
            Assert.IsTrue(resultado < TamanoColeccionValido);
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoCeroLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _generador.ObtenerIndiceAleatorio(TamanoColeccionCero));
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoNegativoLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _generador.ObtenerIndiceAleatorio(TamanoColeccionNegativo));
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaValidaRetornaElementoDeLaLista()
        {
            var listaDatos = new List<string> { ElementoUno, ElementoDos, ElementoTres };

            var resultado = _generador.SeleccionarAleatorio(listaDatos);

            CollectionAssert.Contains(listaDatos, resultado);
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaNulaLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _generador.SeleccionarAleatorio<string>(null));
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaVaciaLanzaExcepcion()
        {
            var listaVacia = new List<int>();

            Assert.ThrowsException<ArgumentException>(() =>
                _generador.SeleccionarAleatorio(listaVacia));
        }

        [TestMethod]
        public void Prueba_MezclarLista_ListaConElementosNoPierdeDatos()
        {
            var listaOriginal = new List<string> { ElementoUno, ElementoDos, ElementoTres };
            var listaMezclada = new List<string>(listaOriginal);

            _generador.MezclarLista(listaMezclada);

            Assert.AreEqual(listaOriginal.Count, listaMezclada.Count);
            CollectionAssert.AreEquivalent(listaOriginal, listaMezclada);
        }

        [TestMethod]
        public void Prueba_MezclarLista_ListaNulaLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _generador.MezclarLista<string>(null));
        }
    }
}