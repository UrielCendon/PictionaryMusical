using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Resultados;
using System.Collections.Generic;
using System.Globalization;

namespace PictionaryMusicalCliente.Pruebas.Utilidades
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase NombreInvitadoGenerador.
    /// Verifica el comportamiento del generador de nombres aleatorios para invitados.
    /// </summary>
    [TestClass]
    public class NombreInvitadoGeneradorPruebas
    {
        private NombreInvitadoGenerador _generador;

        /// <summary>
        /// Inicializa el generador antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _generador = new NombreInvitadoGenerador();
        }

        [TestMethod]
        public void Prueba_Generar_SinParametros_RetornaExito()
        {
            var resultado = _generador.Generar(null);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Exitoso);
            Assert.IsNotNull(resultado.NombreGenerado);
        }

        [TestMethod]
        public void Prueba_Generar_ConCulturaActual_RetornaExito()
        {
            var resultado = _generador.Generar(CultureInfo.CurrentUICulture);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Exitoso);
        }

        [TestMethod]
        public void Prueba_Generar_ConCulturaEspanol_RetornaExito()
        {
            var cultura = new CultureInfo("es-MX");

            var resultado = _generador.Generar(cultura);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_Generar_ConCulturaIngles_RetornaExito()
        {
            var cultura = new CultureInfo("en-US");

            var resultado = _generador.Generar(cultura);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_Generar_NombreNoVacio_RetornaNombreValido()
        {
            var resultado = _generador.Generar(null);

            if (resultado.Exitoso)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(resultado.NombreGenerado));
            }
        }

        [TestMethod]
        public void Prueba_Generar_ConNombresExcluidos_NoRetornaNombreExcluido()
        {
            var nombresExcluidos = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                var resultado = _generador.Generar(null, nombresExcluidos);
                if (resultado.Exitoso)
                {
                    Assert.IsFalse(nombresExcluidos.Contains(resultado.NombreGenerado));
                    nombresExcluidos.Add(resultado.NombreGenerado);
                }
            }
        }

        [TestMethod]
        public void Prueba_Generar_ConListaExcluidosNula_RetornaExito()
        {
            var resultado = _generador.Generar(null, null);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_Generar_ConListaExcluidosVacia_RetornaExito()
        {
            var nombresExcluidos = new List<string>();

            var resultado = _generador.Generar(null, nombresExcluidos);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_Generar_ConNombresExcluidosConEspacios_IgnoraVacios()
        {
            var nombresExcluidos = new List<string> { "", "   ", null };

            var resultado = _generador.Generar(null, nombresExcluidos);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_Generar_TodosNombresExcluidos_RetornaFallo()
        {
            var nombresExcluidos = new HashSet<string>();

            for (int i = 0; i < 100; i++)
            {
                var resultado = _generador.Generar(null, nombresExcluidos);
                if (resultado.Exitoso)
                {
                    nombresExcluidos.Add(resultado.NombreGenerado);
                }
                else
                {
                    Assert.AreEqual(MotivoFalloGeneracion.NombresAgotados, resultado.Motivo);
                    return;
                }
            }
        }

        [TestMethod]
        public void Prueba_Generar_MultiplesCalls_RetornaNombresDiferentes()
        {
            var nombresGenerados = new HashSet<string>();
            int intentos = 10;
            int diferentes = 0;

            for (int i = 0; i < intentos; i++)
            {
                var resultado = _generador.Generar(null);
                if (resultado.Exitoso && nombresGenerados.Add(resultado.NombreGenerado))
                {
                    diferentes++;
                }
            }

            Assert.IsTrue(diferentes >= 1);
        }

        [TestMethod]
        public void Prueba_Generar_EsThreadSafe_NoLanzaExcepcion()
        {
            var tareas = new System.Threading.Tasks.Task[10];

            for (int i = 0; i < 10; i++)
            {
                tareas[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        _generador.Generar(null);
                    }
                });
            }

            System.Threading.Tasks.Task.WaitAll(tareas);
        }

        [TestMethod]
        public void Prueba_ResultadoGeneracion_Exito_PropiedadesCorrectas()
        {
            var resultado = ResultadoGeneracion.Exito("NombrePrueba");

            Assert.IsTrue(resultado.Exitoso);
            Assert.AreEqual("NombrePrueba", resultado.NombreGenerado);
            Assert.AreEqual(MotivoFalloGeneracion.Ninguno, resultado.Motivo);
        }

        [TestMethod]
        public void Prueba_ResultadoGeneracion_Fallo_PropiedadesCorrectas()
        {
            var resultado = ResultadoGeneracion.Fallo(MotivoFalloGeneracion.NombresAgotados);

            Assert.IsFalse(resultado.Exitoso);
            Assert.AreEqual(string.Empty, resultado.NombreGenerado);
            Assert.AreEqual(MotivoFalloGeneracion.NombresAgotados, resultado.Motivo);
        }

        [TestMethod]
        public void Prueba_ResultadoGeneracion_FalloRecursoNoEncontrado_MotivoCorreecto()
        {
            var resultado = ResultadoGeneracion.Fallo(MotivoFalloGeneracion.RecursoNoEncontrado);

            Assert.IsFalse(resultado.Exitoso);
            Assert.AreEqual(MotivoFalloGeneracion.RecursoNoEncontrado, resultado.Motivo);
        }

        [TestMethod]
        public void Prueba_ResultadoGeneracion_FalloListaVacia_MotivoCorecto()
        {
            var resultado = ResultadoGeneracion.Fallo(MotivoFalloGeneracion.ListaVacia);

            Assert.IsFalse(resultado.Exitoso);
            Assert.AreEqual(MotivoFalloGeneracion.ListaVacia, resultado.Motivo);
        }
    }
}
