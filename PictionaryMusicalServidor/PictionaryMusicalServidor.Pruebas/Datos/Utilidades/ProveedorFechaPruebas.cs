using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;
using System;

namespace PictionaryMusicalServidor.Pruebas.Datos.Utilidades
{
    [TestClass]
    public class ProveedorFechaPruebas
    {
        private const int MargenSegundosTolerancia = 2;

        private ProveedorFecha _proveedor;

        [TestInitialize]
        public void Inicializar()
        {
            _proveedor = new ProveedorFecha();
        }

        [TestMethod]
        public void Prueba_ObtenerFechaActualUtc_RetornaFechaEnFormatoUtc()
        {
            DateTime resultado = _proveedor.ObtenerFechaActualUtc();

            Assert.AreEqual(DateTimeKind.Utc, resultado.Kind);
        }

        [TestMethod]
        public void Prueba_ObtenerFechaActualUtc_RetornaFechaCercanaAUtcNow()
        {
            DateTime fechaAntes = DateTime.UtcNow;

            DateTime resultado = _proveedor.ObtenerFechaActualUtc();

            Assert.IsTrue(
                (resultado - fechaAntes).TotalSeconds <= MargenSegundosTolerancia);
        }

        [TestMethod]
        public void Prueba_ObtenerFechaActualUtc_RetornaFechaNoAnteriorAlMomento()
        {
            DateTime fechaAntes = DateTime.UtcNow.AddSeconds(-MargenSegundosTolerancia);

            DateTime resultado = _proveedor.ObtenerFechaActualUtc();

            Assert.IsTrue(resultado >= fechaAntes);
        }
    }
}
