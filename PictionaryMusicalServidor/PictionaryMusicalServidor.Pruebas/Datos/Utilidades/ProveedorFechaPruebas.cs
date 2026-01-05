using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Datos.Utilidades
{
    /// <summary>
    /// Pruebas unitarias para la clase <see cref="ProveedorFecha"/>.
    /// </summary>
    [TestClass]
    public class ProveedorFechaPruebas
    {
        private ProveedorFecha _proveedor;

        [TestInitialize]
        public void Inicializar()
        {
            _proveedor = new ProveedorFecha();
        }

        [TestMethod]
        public void Prueba_ObtenerFechaActualUtc_RetornaFechaUtcNoNula()
        {
            DateTime fecha = _proveedor.ObtenerFechaActualUtc();

            Assert.AreNotEqual(DateTime.MinValue, fecha);
            Assert.AreEqual(DateTimeKind.Utc, fecha.Kind);
        }

        [TestMethod]
        public void Prueba_ObtenerFechaActualUtc_RetornaFechaActual()
        {
            DateTime antes = DateTime.UtcNow;
            DateTime fecha = _proveedor.ObtenerFechaActualUtc();
            DateTime despues = DateTime.UtcNow;

            Assert.IsTrue(fecha >= antes);
            Assert.IsTrue(fecha <= despues);
        }
    }
}
