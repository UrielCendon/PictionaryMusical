using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Datos.Utilidades
{
    /// <summary>
    /// Pruebas unitarias para la clase GeneradorAleatorioDatos.
    /// Verifica la generacion de indices aleatorios y seleccion de elementos.
    /// </summary>
    [TestClass]
    public class GeneradorAleatorioDatosPruebas
    {
        private const int TamanoCero = 0;
        private const int TamanoNegativo = -5;
        private const int TamanoValido = 10;
        private const int TamanoUno = 1;
        private const int IndiceCero = 0;
        private const string ElementoUnico = "unico";
        private const int ElementoUno = 1;
        private const int ElementoDos = 2;
        private const int ElementoTres = 3;
        private const int ElementoCuatro = 4;
        private const int ElementoCinco = 5;

        #region ObtenerIndiceAleatorio

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorioTamanoCero_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                GeneradorAleatorioDatos.ObtenerIndiceAleatorio(TamanoCero);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorioTamanoNegativo_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                GeneradorAleatorioDatos.ObtenerIndiceAleatorio(TamanoNegativo);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorioTamanoValido_RetornaIndiceDentroRango()
        {
            int indice = GeneradorAleatorioDatos.ObtenerIndiceAleatorio(TamanoValido);

            Assert.IsTrue(indice >= IndiceCero && indice < TamanoValido);
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorioTamanoUno_RetornaCero()
        {
            int indice = GeneradorAleatorioDatos.ObtenerIndiceAleatorio(TamanoUno);

            Assert.AreEqual(IndiceCero, indice);
        }

        #endregion

        #region SeleccionarAleatorio

        [TestMethod]
        public void Prueba_SeleccionarAleatorioListaNula_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GeneradorAleatorioDatos.SeleccionarAleatorio<string>(null);
            });
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorioListaVacia_LanzaArgumentException()
        {
            var listaVacia = new List<string>();

            Assert.ThrowsException<ArgumentException>(() =>
            {
                GeneradorAleatorioDatos.SeleccionarAleatorio(listaVacia);
            });
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorioListaUnElemento_RetornaElemento()
        {
            var lista = new List<string> { ElementoUnico };

            string resultado = GeneradorAleatorioDatos.SeleccionarAleatorio(lista);

            Assert.AreEqual(ElementoUnico, resultado);
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorioListaMultiplesElementos_RetornaElementoDeLista()
        {
            var lista = new List<int> { ElementoUno, ElementoDos, ElementoTres, ElementoCuatro, ElementoCinco };

            int resultado = GeneradorAleatorioDatos.SeleccionarAleatorio(lista);

            CollectionAssert.Contains(lista, resultado);
        }

        #endregion
    }
}
