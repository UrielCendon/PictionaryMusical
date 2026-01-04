using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Utilidades
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase GeneradorAleatorioDatos.
    /// Verifica flujos normales, alternos y de excepcion para la generacion de datos aleatorios.
    /// </summary>
    [TestClass]
    public class GeneradorAleatorioDatosPruebas
    {
        #region ObtenerIndiceAleatorio - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoUno_RetornaCero()
        {
            int resultado = GeneradorAleatorioDatos.ObtenerIndiceAleatorio(1);

            Assert.AreEqual(0, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoValido_RetornaIndiceEnRango()
        {
            int tamano = 10;

            int resultado = GeneradorAleatorioDatos.ObtenerIndiceAleatorio(tamano);

            Assert.IsTrue(resultado >= 0 && resultado < tamano);
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoGrande_RetornaIndiceEnRango()
        {
            int tamano = 1000;

            int resultado = GeneradorAleatorioDatos.ObtenerIndiceAleatorio(tamano);

            Assert.IsTrue(resultado >= 0 && resultado < tamano);
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_MultiplesLlamadas_RetornaIndicesValidos()
        {
            int tamano = 5;
            bool todosValidos = true;

            for (int i = 0; i < 100; i++)
            {
                int resultado = GeneradorAleatorioDatos.ObtenerIndiceAleatorio(tamano);
                if (resultado < 0 || resultado >= tamano)
                {
                    todosValidos = false;
                    break;
                }
            }

            Assert.IsTrue(todosValidos);
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_MultiplesLlamadas_ProduceVariedad()
        {
            int tamano = 10;
            var resultados = new HashSet<int>();

            for (int i = 0; i < 100; i++)
            {
                resultados.Add(GeneradorAleatorioDatos.ObtenerIndiceAleatorio(tamano));
            }

            Assert.IsTrue(resultados.Count > 1, 
                "Se esperaba variedad en los resultados aleatorios");
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoDos_RetornaIndiceValido()
        {
            int tamano = 2;

            int resultado = GeneradorAleatorioDatos.ObtenerIndiceAleatorio(tamano);

            Assert.IsTrue(resultado == 0 || resultado == 1);
        }

        #endregion

        #region ObtenerIndiceAleatorio - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoCero_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                GeneradorAleatorioDatos.ObtenerIndiceAleatorio(0);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoNegativo_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                GeneradorAleatorioDatos.ObtenerIndiceAleatorio(-1);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_TamanoMuyNegativo_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                GeneradorAleatorioDatos.ObtenerIndiceAleatorio(-100);
            });
        }

        #endregion

        #region SeleccionarAleatorio - Flujos Normales

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaUnElemento_RetornaElemento()
        {
            var lista = new List<string> { "unico" };

            string resultado = GeneradorAleatorioDatos.SeleccionarAleatorio(lista);

            Assert.AreEqual("unico", resultado);
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaMultiplesElementos_RetornaElementoDeLista()
        {
            var lista = new List<string> { "a", "b", "c", "d", "e" };

            string resultado = GeneradorAleatorioDatos.SeleccionarAleatorio(lista);

            Assert.IsTrue(lista.Contains(resultado));
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaEnteros_RetornaElementoDeLista()
        {
            var lista = new List<int> { 1, 2, 3, 4, 5 };

            int resultado = GeneradorAleatorioDatos.SeleccionarAleatorio(lista);

            Assert.IsTrue(lista.Contains(resultado));
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_MultiplesLlamadas_RetornaElementosValidos()
        {
            var lista = new List<string> { "uno", "dos", "tres" };
            bool todosValidos = true;

            for (int i = 0; i < 50; i++)
            {
                string resultado = GeneradorAleatorioDatos.SeleccionarAleatorio(lista);
                if (!lista.Contains(resultado))
                {
                    todosValidos = false;
                    break;
                }
            }

            Assert.IsTrue(todosValidos);
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_MultiplesLlamadas_ProduceVariedad()
        {
            var lista = new List<string> { "a", "b", "c", "d", "e" };
            var resultados = new HashSet<string>();

            for (int i = 0; i < 100; i++)
            {
                resultados.Add(GeneradorAleatorioDatos.SeleccionarAleatorio(lista));
            }

            Assert.IsTrue(resultados.Count > 1, 
                "Se esperaba variedad en los resultados aleatorios");
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaObjetos_RetornaObjetoDeLista()
        {
            var objeto1 = new { Id = 1, Nombre = "Objeto1" };
            var objeto2 = new { Id = 2, Nombre = "Objeto2" };
            var lista = new List<object> { objeto1, objeto2 };

            object resultado = GeneradorAleatorioDatos.SeleccionarAleatorio(lista);

            Assert.IsTrue(resultado == objeto1 || resultado == objeto2);
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaDosElementos_RetornaUnoDeLosDos()
        {
            var lista = new List<string> { "primero", "segundo" };

            string resultado = GeneradorAleatorioDatos.SeleccionarAleatorio(lista);

            Assert.IsTrue(resultado == "primero" || resultado == "segundo");
        }

        #endregion

        #region SeleccionarAleatorio - Flujos de Excepcion

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaNula_LanzaArgumentNullException()
        {
            List<string> lista = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                GeneradorAleatorioDatos.SeleccionarAleatorio(lista);
            });
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaVacia_LanzaArgumentException()
        {
            var lista = new List<string>();

            Assert.ThrowsException<ArgumentException>(() =>
            {
                GeneradorAleatorioDatos.SeleccionarAleatorio(lista);
            });
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_ListaEnterosVacia_LanzaArgumentException()
        {
            var lista = new List<int>();

            Assert.ThrowsException<ArgumentException>(() =>
            {
                GeneradorAleatorioDatos.SeleccionarAleatorio(lista);
            });
        }

        #endregion

        #region Thread Safety (Pruebas de Concurrencia Basicas)

        [TestMethod]
        public void Prueba_ObtenerIndiceAleatorio_LlamadasConcurrentes_NoLanzaExcepcion()
        {
            int tamano = 100;
            var tareas = new List<System.Threading.Tasks.Task>();
            bool huboExcepcion = false;

            for (int i = 0; i < 10; i++)
            {
                var tarea = System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            GeneradorAleatorioDatos.ObtenerIndiceAleatorio(tamano);
                        }
                    }
                    catch
                    {
                        huboExcepcion = true;
                    }
                });
                tareas.Add(tarea);
            }

            System.Threading.Tasks.Task.WaitAll(tareas.ToArray());

            Assert.IsFalse(huboExcepcion, "No deberian ocurrir excepciones en llamadas concurrentes");
        }

        [TestMethod]
        public void Prueba_SeleccionarAleatorio_LlamadasConcurrentes_NoLanzaExcepcion()
        {
            var lista = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var tareas = new List<System.Threading.Tasks.Task>();
            bool huboExcepcion = false;

            for (int i = 0; i < 10; i++)
            {
                var tarea = System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            GeneradorAleatorioDatos.SeleccionarAleatorio(lista);
                        }
                    }
                    catch
                    {
                        huboExcepcion = true;
                    }
                });
                tareas.Add(tarea);
            }

            System.Threading.Tasks.Task.WaitAll(tareas.ToArray());

            Assert.IsFalse(huboExcepcion, "No deberian ocurrir excepciones en llamadas concurrentes");
        }

        #endregion
    }
}
