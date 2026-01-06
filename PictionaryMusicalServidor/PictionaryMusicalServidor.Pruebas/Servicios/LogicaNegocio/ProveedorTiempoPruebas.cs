using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class ProveedorTiempoPruebas
    {
        private const int TiempoEsperaCorto = 50;
        private const int TiempoEsperaMinimo = 1;
        private const int TiempoEsperaCero = 0;
        private const int ToleranciaMargenMs = 100;

        private ProveedorTiempo _proveedor;

        [TestInitialize]
        public void Inicializar()
        {
            _proveedor = new ProveedorTiempo();
        }
        [TestMethod]
        public void Prueba_Constructor_CreaInstanciaCorrectamente()
        {
            var proveedor = new ProveedorTiempo();

            Assert.IsInstanceOfType(proveedor, typeof(ProveedorTiempo));
        }        [TestMethod]
        public async Task Prueba_Retrasar_CompletaTareaConTiempoCorto()
        {
            bool completado = false;

            await _proveedor.Retrasar(TiempoEsperaCorto);
            completado = true;

            Assert.IsTrue(completado);
        }

        [TestMethod]
        public async Task Prueba_Retrasar_CompletaTareaConTiempoMinimo()
        {
            bool completado = false;

            await _proveedor.Retrasar(TiempoEsperaMinimo);
            completado = true;

            Assert.IsTrue(completado);
        }

        [TestMethod]
        public async Task Prueba_Retrasar_CompletaTareaConTiempoCero()
        {
            bool completado = false;

            await _proveedor.Retrasar(TiempoEsperaCero);
            completado = true;

            Assert.IsTrue(completado);
        }

        [TestMethod]
        public async Task Prueba_Retrasar_EsperaAproximadamenteTiempoIndicado()
        {
            var cronometro = System.Diagnostics.Stopwatch.StartNew();

            await _proveedor.Retrasar(TiempoEsperaCorto);
            cronometro.Stop();

            Assert.IsTrue(
                cronometro.ElapsedMilliseconds >= TiempoEsperaCorto - ToleranciaMargenMs);
        }    }
}
