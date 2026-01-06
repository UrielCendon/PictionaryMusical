using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Utilidades
{
    [TestClass]
    public class GeneradorAleatorioPruebas
    {
        private const int LongitudCodigoPorDefecto = 6;
        private const int LongitudCodigoPersonalizado = 4;
        private const int LongitudInvalida = 0;
        private const int LongitudNegativa = -1;
        private const int LongitudTokenEsperada = 32;
        private const int IndiceInicial = 0;

        [TestMethod]
        public void Prueba_GenerarToken_RetornaCadenaNoVacia()
        {
            string token = GeneradorAleatorio.GenerarToken();

            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public void Prueba_GenerarToken_RetornaCadenaConLongitudCorrecta()
        {
            string token = GeneradorAleatorio.GenerarToken();

            Assert.AreEqual(LongitudTokenEsperada, token.Length);
        }

        [TestMethod]
        public void Prueba_GenerarToken_RetornaTokensUnicos()
        {
            string tokenUno = GeneradorAleatorio.GenerarToken();
            string tokenDos = GeneradorAleatorio.GenerarToken();

            Assert.AreNotEqual(tokenUno, tokenDos);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_RetornaCadenaNoVacia()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();

            Assert.IsFalse(string.IsNullOrWhiteSpace(codigo));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_RetornaCodigoConLongitudPorDefecto()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();

            Assert.AreEqual(LongitudCodigoPorDefecto, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_RetornaCodigoConLongitudPersonalizada()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion(LongitudCodigoPersonalizado);

            Assert.AreEqual(LongitudCodigoPersonalizado, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LanzaExcepcionLongitudCero()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => GeneradorAleatorio.GenerarCodigoVerificacion(LongitudInvalida));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LanzaExcepcionLongitudNegativa()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => GeneradorAleatorio.GenerarCodigoVerificacion(LongitudNegativa));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_RetornaCadenaNoVacia()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoSala();

            Assert.IsFalse(string.IsNullOrWhiteSpace(codigo));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_RetornaCodigoConLongitudPorDefecto()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoSala();

            Assert.AreEqual(LongitudCodigoPorDefecto, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_RetornaCodigoConLongitudPersonalizada()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoSala(LongitudCodigoPersonalizado);

            Assert.AreEqual(LongitudCodigoPersonalizado, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_LanzaExcepcionLongitudCero()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => GeneradorAleatorio.GenerarCodigoSala(LongitudInvalida));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_LanzaExcepcionLongitudNegativa()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => GeneradorAleatorio.GenerarCodigoSala(LongitudNegativa));
        }

        [TestMethod]
        public void Prueba_MezclarLista_LanzaExcepcionListaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => GeneradorAleatorio.MezclarLista<string>(null));
        }

        [TestMethod]
        public void Prueba_MezclarLista_NoModificaTamanoLista()
        {
            var lista = new List<string> { "Uno", "Dos", "Tres" };
            int tamanoOriginal = lista.Count;

            GeneradorAleatorio.MezclarLista(lista);

            Assert.AreEqual(tamanoOriginal, lista.Count);
        }

        [TestMethod]
        public void Prueba_MezclarLista_ConservaElementosOriginales()
        {
            var lista = new List<string> { "Uno", "Dos", "Tres" };
            var elementosOriginales = new List<string>(lista);

            GeneradorAleatorio.MezclarLista(lista);

            CollectionAssert.AreEquivalent(elementosOriginales, lista);
        }

        [TestMethod]
        public void Prueba_MezclarLista_NoLanzaExcepcionListaVacia()
        {
            var listaVacia = new List<string>();

            GeneradorAleatorio.MezclarLista(listaVacia);

            Assert.AreEqual(IndiceInicial, listaVacia.Count);
        }

        [TestMethod]
        public void Prueba_MezclarLista_NoModificaListaConUnElemento()
        {
            var lista = new List<string> { "Unico" };
            string elementoOriginal = lista[IndiceInicial];

            GeneradorAleatorio.MezclarLista(lista);

            Assert.AreEqual(elementoOriginal, lista[IndiceInicial]);
        }
    }
}
