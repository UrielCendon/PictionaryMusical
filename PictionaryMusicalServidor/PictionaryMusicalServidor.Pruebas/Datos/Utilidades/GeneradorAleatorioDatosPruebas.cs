using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;
using System;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Pruebas.Datos.Utilidades
{
    [TestClass]
    public class GeneradorAleatorioDatosPruebas
    {
        private const int TamanoColeccionValido = 10;
        private const int TamanoColeccionCero = 0;
        private const int TamanoColeccionNegativo = -1;
        private const int TamanoColeccionUno = 1;
        private const int IndiceCero = 0;
        private const string ElementoPruebaUno = "ElementoUno";
        private const string ElementoPruebaDos = "ElementoDos";
        private const string ElementoPruebaTres = "ElementoTres";

        private GeneradorAleatorioDatos _generador;

        [TestInitialize]
        public void Inicializar()
        {
            _generador = new GeneradorAleatorioDatos();
        }

        #region ObtenerIndiceAleatorio

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_RetornaIndiceDentroDeRango()
        {
            int indice = _generador.ObtenerIndiceAleatorio(TamanoColeccionValido);

            Assert.IsTrue(indice >= IndiceCero && indice < TamanoColeccionValido);
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_RetornaCeroParaColeccionUnitaria()
        {
            int indice = _generador.ObtenerIndiceAleatorio(TamanoColeccionUno);

            Assert.AreEqual(IndiceCero, indice);
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_LanzaExcepcionTamanoCero()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _generador.ObtenerIndiceAleatorio(TamanoColeccionCero));
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_LanzaExcepcionTamanoNegativo()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _generador.ObtenerIndiceAleatorio(TamanoColeccionNegativo));
        }

        #endregion

        #region SeleccionarAleatorio

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_LanzaExcepcionListaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _generador.SeleccionarAleatorio<string>(null));
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_LanzaExcepcionListaVacia()
        {
            var listaVacia = new List<string>();

            Assert.ThrowsException<ArgumentException>(
                () => _generador.SeleccionarAleatorio(listaVacia));
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_RetornaElementoUnicoEnListaUnitaria()
        {
            var lista = new List<string> { ElementoPruebaUno };

            string resultado = _generador.SeleccionarAleatorio(lista);

            Assert.AreEqual(ElementoPruebaUno, resultado);
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_RetornaElementoDeLista()
        {
            var lista = new List<string> 
            { 
                ElementoPruebaUno, 
                ElementoPruebaDos, 
                ElementoPruebaTres 
            };

            string resultado = _generador.SeleccionarAleatorio(lista);

            Assert.IsTrue(lista.Contains(resultado));
        }

        #endregion

        #region MezclarLista

        [TestMethod]
        public void Prueba_MezclarLista_LanzaExcepcionListaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _generador.MezclarLista<string>(null));
        }

        [TestMethod]
        public void Prueba_MezclarLista_NoModificaListaVacia()
        {
            var listaVacia = new List<string>();
            int cantidadOriginal = listaVacia.Count;

            _generador.MezclarLista(listaVacia);

            Assert.AreEqual(cantidadOriginal, listaVacia.Count);
        }

        [TestMethod]
        public void Prueba_MezclarLista_ConservaElementosOriginales()
        {
            var lista = new List<string> 
            { 
                ElementoPruebaUno, 
                ElementoPruebaDos, 
                ElementoPruebaTres 
            };
            var elementosOriginales = new HashSet<string>(lista);

            _generador.MezclarLista(lista);

            Assert.IsTrue(elementosOriginales.SetEquals(lista));
        }

        [TestMethod]
        public void Prueba_MezclarLista_ConservaCantidadElementos()
        {
            var lista = new List<string> 
            { 
                ElementoPruebaUno, 
                ElementoPruebaDos, 
                ElementoPruebaTres 
            };
            int cantidadOriginal = lista.Count;

            _generador.MezclarLista(lista);

            Assert.AreEqual(cantidadOriginal, lista.Count);
        }

        [TestMethod]
        public void Prueba_MezclarLista_NoModificaListaUnitaria()
        {
            var lista = new List<string> { ElementoPruebaUno };

            _generador.MezclarLista(lista);

            Assert.AreEqual(ElementoPruebaUno, lista[IndiceCero]);
        }

        #endregion
    }
}
