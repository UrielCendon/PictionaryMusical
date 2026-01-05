using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Utilidades
{
    [TestClass]
    public class GeneradorAleatorioPruebas
    {
        private const int LongitudTokenEsperada = 32;
        private const int LongitudPorDefectoCodigoVerificacion = 6;
        private const int LongitudPorDefectoCodigoSala = 6;
        private const int LongitudCuatroDigitos = 4;
        private const int LongitudOchoDigitos = 8;
        private const int LongitudCero = 0;
        private const int LongitudNegativa = -1;
        private const int NumeroIteracionesPruebaAleatoriedad = 100;
        private const int TamanioListaMezcla = 10;
        private const int IndiceInicial = 0;

        #region Pruebas GenerarToken

        [TestMethod]
        public void Prueba_GenerarToken_RetornaTokenConLongitudCorrecta()
        {
            string token = GeneradorAleatorio.GenerarToken();

            Assert.AreEqual(LongitudTokenEsperada, token.Length);
        }

        [TestMethod]
        public void Prueba_GenerarToken_RetornaTokenHexadecimal()
        {
            string token = GeneradorAleatorio.GenerarToken();

            bool esHexadecimal = System.Text.RegularExpressions.Regex.IsMatch(
                token, "^[a-fA-F0-9]+$");

            Assert.IsTrue(esHexadecimal);
        }

        [TestMethod]
        public void Prueba_GenerarToken_TokensConsecutivosSonDiferentes()
        {
            string token1 = GeneradorAleatorio.GenerarToken();
            string token2 = GeneradorAleatorio.GenerarToken();

            Assert.AreNotEqual(token1, token2);
        }

        [TestMethod]
        public void Prueba_GenerarToken_GeneraTokensUnicos()
        {
            var tokens = new HashSet<string>();

            for (int i = 0; i < NumeroIteracionesPruebaAleatoriedad; i++)
            {
                tokens.Add(GeneradorAleatorio.GenerarToken());
            }

            Assert.AreEqual(NumeroIteracionesPruebaAleatoriedad, tokens.Count);
        }

        #endregion

        #region Pruebas GenerarCodigoVerificacion

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LongitudPorDefecto_RetornaSeisDigitos()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();

            Assert.AreEqual(LongitudPorDefectoCodigoVerificacion, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LongitudPersonalizada_RetornaLongitudCorrecta()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion(LongitudCuatroDigitos);

            Assert.AreEqual(LongitudCuatroDigitos, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_RetornaSoloDigitos()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();

            bool soloDigitos = System.Text.RegularExpressions.Regex.IsMatch(codigo, "^[0-9]+$");

            Assert.IsTrue(soloDigitos);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LongitudCero_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GeneradorAleatorio.GenerarCodigoVerificacion(LongitudCero));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LongitudNegativa_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GeneradorAleatorio.GenerarCodigoVerificacion(LongitudNegativa));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_CodigosConsecutivosSonDiferentes()
        {
            var codigos = new HashSet<string>();

            for (int i = 0; i < NumeroIteracionesPruebaAleatoriedad; i++)
            {
                codigos.Add(GeneradorAleatorio.GenerarCodigoVerificacion(LongitudOchoDigitos));
            }

            Assert.IsTrue(codigos.Count > 1);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_NoPresentaCerosIniciales()
        {
            bool encontroCeroInicial = false;

            for (int i = 0; i < NumeroIteracionesPruebaAleatoriedad; i++)
            {
                string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();
                if (codigo.StartsWith("0"))
                {
                    encontroCeroInicial = true;
                    break;
                }
            }

            Assert.IsFalse(encontroCeroInicial);
        }

        #endregion

        #region Pruebas GenerarCodigoSala

        [TestMethod]
        public void Prueba_GenerarCodigoSala_LongitudPorDefecto_RetornaSeisDigitos()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoSala();

            Assert.AreEqual(LongitudPorDefectoCodigoSala, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_LongitudPersonalizada_RetornaLongitudCorrecta()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoSala(LongitudCuatroDigitos);

            Assert.AreEqual(LongitudCuatroDigitos, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_RetornaSoloDigitos()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoSala();

            bool soloDigitos = System.Text.RegularExpressions.Regex.IsMatch(codigo, "^[0-9]+$");

            Assert.IsTrue(soloDigitos);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_LongitudCero_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GeneradorAleatorio.GenerarCodigoSala(LongitudCero));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_LongitudNegativa_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GeneradorAleatorio.GenerarCodigoSala(LongitudNegativa));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_PuedeTenerCerosIniciales()
        {
            bool encontroCeroInicial = false;

            for (int i = 0; i < NumeroIteracionesPruebaAleatoriedad; i++)
            {
                string codigo = GeneradorAleatorio.GenerarCodigoSala();
                if (codigo.StartsWith("0"))
                {
                    encontroCeroInicial = true;
                    break;
                }
            }

            Assert.IsTrue(encontroCeroInicial);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_CodigosConsecutivosSonDiferentes()
        {
            var codigos = new HashSet<string>();

            for (int i = 0; i < NumeroIteracionesPruebaAleatoriedad; i++)
            {
                codigos.Add(GeneradorAleatorio.GenerarCodigoSala());
            }

            Assert.IsTrue(codigos.Count > 1);
        }

        #endregion

        #region Pruebas MezclarLista

        [TestMethod]
        public void Prueba_MezclarLista_ListaNula_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                GeneradorAleatorio.MezclarLista<int>(null));
        }

        [TestMethod]
        public void Prueba_MezclarLista_ListaVacia_NoLanzaExcepcion()
        {
            var lista = new List<int>();

            GeneradorAleatorio.MezclarLista(lista);

            Assert.AreEqual(IndiceInicial, lista.Count);
        }

        [TestMethod]
        public void Prueba_MezclarLista_ListaUnElemento_ConservaElemento()
        {
            var lista = new List<int> { 1 };

            GeneradorAleatorio.MezclarLista(lista);

            Assert.AreEqual(1, lista[IndiceInicial]);
        }

        [TestMethod]
        public void Prueba_MezclarLista_ConservaElementos()
        {
            var lista = CrearListaOrdenada(TamanioListaMezcla);
            var elementosOriginales = new HashSet<int>(lista);

            GeneradorAleatorio.MezclarLista(lista);

            CollectionAssert.AreEquivalent(
                new List<int>(elementosOriginales), 
                new List<int>(lista));
        }

        [TestMethod]
        public void Prueba_MezclarLista_ConservaCantidadElementos()
        {
            var lista = CrearListaOrdenada(TamanioListaMezcla);

            GeneradorAleatorio.MezclarLista(lista);

            Assert.AreEqual(TamanioListaMezcla, lista.Count);
        }

        [TestMethod]
        public void Prueba_MezclarLista_MezclaEfectivamente()
        {
            int vecesDiferentes = 0;

            for (int i = 0; i < NumeroIteracionesPruebaAleatoriedad; i++)
            {
                var lista = CrearListaOrdenada(TamanioListaMezcla);
                var listaOriginal = new List<int>(lista);

                GeneradorAleatorio.MezclarLista(lista);

                if (!SonListasIguales(lista, listaOriginal))
                {
                    vecesDiferentes++;
                }
            }

            Assert.IsTrue(vecesDiferentes > NumeroIteracionesPruebaAleatoriedad / 2);
        }

        [TestMethod]
        public void Prueba_MezclarLista_ConTipoString_ConservaElementos()
        {
            var lista = new List<string> { "uno", "dos", "tres", "cuatro", "cinco" };
            var elementosOriginales = new HashSet<string>(lista);

            GeneradorAleatorio.MezclarLista(lista);

            CollectionAssert.AreEquivalent(
                new List<string>(elementosOriginales), 
                new List<string>(lista));
        }

        #endregion

        #region Metodos Auxiliares

        private List<int> CrearListaOrdenada(int tamanio)
        {
            var lista = new List<int>();
            for (int i = 0; i < tamanio; i++)
            {
                lista.Add(i);
            }
            return lista;
        }

        private bool SonListasIguales<T>(IList<T> lista1, IList<T> lista2)
        {
            if (lista1.Count != lista2.Count)
            {
                return false;
            }

            for (int i = 0; i < lista1.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(lista1[i], lista2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
