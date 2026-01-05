using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        private const int MitadIteraciones = NumeroIteracionesPruebaAleatoriedad / 2;
        private const int ElementoUnico = 1;
        private const string PatronHexadecimal = "^[a-fA-F0-9]+$";
        private const string PatronSoloDigitos = "^[0-9]+$";
        private const string CaracterCeroInicial = "0";

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

            bool esHexadecimal = Regex.IsMatch(token, PatronHexadecimal);

            Assert.IsTrue(esHexadecimal);
        }

        [TestMethod]
        public void Prueba_GenerarToken_TokensConsecutivosSonDiferentes()
        {
            string tokenPrimero = GeneradorAleatorio.GenerarToken();
            string tokenSegundo = GeneradorAleatorio.GenerarToken();

            Assert.AreNotEqual(tokenPrimero, tokenSegundo);
        }

        [TestMethod]
        public void Prueba_GenerarToken_GeneraTokensUnicos()
        {
            var tokens = new HashSet<string>();

            for (int indice = 0; indice < NumeroIteracionesPruebaAleatoriedad; indice++)
            {
                tokens.Add(GeneradorAleatorio.GenerarToken());
            }

            Assert.AreEqual(NumeroIteracionesPruebaAleatoriedad, tokens.Count);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LongitudPorDefecto_RetornaSeisDigitos()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();

            Assert.AreEqual(LongitudPorDefectoCodigoVerificacion, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LongitudPersonalizada_RetornaLongitud()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion(LongitudCuatroDigitos);

            Assert.AreEqual(LongitudCuatroDigitos, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_RetornaSoloDigitos()
        {
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();

            bool soloDigitos = Regex.IsMatch(codigo, PatronSoloDigitos);

            Assert.IsTrue(soloDigitos);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LongitudCero_LanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GeneradorAleatorio.GenerarCodigoVerificacion(LongitudCero));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_LongitudNegativa_LanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GeneradorAleatorio.GenerarCodigoVerificacion(LongitudNegativa));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_CodigosConsecutivosSonDiferentes()
        {
            var codigos = new HashSet<string>();

            for (int indice = 0; indice < NumeroIteracionesPruebaAleatoriedad; indice++)
            {
                codigos.Add(GeneradorAleatorio.GenerarCodigoVerificacion(LongitudOchoDigitos));
            }

            Assert.IsTrue(codigos.Count > ElementoUnico);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoVerificacion_NoPresentaCerosIniciales()
        {
            bool encontroCeroInicial = false;

            for (int indice = 0; indice < NumeroIteracionesPruebaAleatoriedad; indice++)
            {
                string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();
                if (codigo.StartsWith(CaracterCeroInicial))
                {
                    encontroCeroInicial = true;
                    break;
                }
            }

            Assert.IsFalse(encontroCeroInicial);
        }

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

            bool soloDigitos = Regex.IsMatch(codigo, PatronSoloDigitos);

            Assert.IsTrue(soloDigitos);
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_LongitudCero_LanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GeneradorAleatorio.GenerarCodigoSala(LongitudCero));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_LongitudNegativa_LanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GeneradorAleatorio.GenerarCodigoSala(LongitudNegativa));
        }

        [TestMethod]
        public void Prueba_GenerarCodigoSala_PuedeTenerCerosIniciales()
        {
            bool encontroCeroInicial = false;

            for (int indice = 0; indice < NumeroIteracionesPruebaAleatoriedad; indice++)
            {
                string codigo = GeneradorAleatorio.GenerarCodigoSala();
                if (codigo.StartsWith(CaracterCeroInicial))
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

            for (int indice = 0; indice < NumeroIteracionesPruebaAleatoriedad; indice++)
            {
                codigos.Add(GeneradorAleatorio.GenerarCodigoSala());
            }

            Assert.IsTrue(codigos.Count > ElementoUnico);
        }

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
            var lista = new List<int> { ElementoUnico };

            GeneradorAleatorio.MezclarLista(lista);

            Assert.AreEqual(ElementoUnico, lista[IndiceInicial]);
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

            for (int indice = 0; indice < NumeroIteracionesPruebaAleatoriedad; indice++)
            {
                var lista = CrearListaOrdenada(TamanioListaMezcla);
                var listaOriginal = new List<int>(lista);

                GeneradorAleatorio.MezclarLista(lista);

                if (!SonListasIguales(lista, listaOriginal))
                {
                    vecesDiferentes++;
                }
            }

            Assert.IsTrue(vecesDiferentes > MitadIteraciones);
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

        private static List<int> CrearListaOrdenada(int tamanio)
        {
            var lista = new List<int>();
            for (int indice = 0; indice < tamanio; indice++)
            {
                lista.Add(indice);
            }
            return lista;
        }

        private static bool SonListasIguales<T>(IList<T> listaPrimera, IList<T> listaSegunda)
        {
            if (listaPrimera.Count != listaSegunda.Count)
            {
                return false;
            }

            for (int indice = 0; indice < listaPrimera.Count; indice++)
            {
                if (!EqualityComparer<T>.Default.Equals(listaPrimera[indice], listaSegunda[indice]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
