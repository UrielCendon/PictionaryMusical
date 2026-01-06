using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class IdentificadoresUsuariosPruebas
    {
        private const int IdReportanteEsperado = 1;
        private const int IdReportadoEsperado = 2;
        private const int ValorPorDefecto = 0;

        [TestMethod]
        public void Prueba_IdReportante_PuedeAsignarseYObtenerse()
        {
            var identificadores = new IdentificadoresUsuarios();

            identificadores.IdReportante = IdReportanteEsperado;

            Assert.AreEqual(IdReportanteEsperado, identificadores.IdReportante);
        }

        [TestMethod]
        public void Prueba_IdReportado_PuedeAsignarseYObtenerse()
        {
            var identificadores = new IdentificadoresUsuarios();

            identificadores.IdReportado = IdReportadoEsperado;

            Assert.AreEqual(IdReportadoEsperado, identificadores.IdReportado);
        }

        [TestMethod]
        public void Prueba_IdReportante_ValorPorDefectoCero()
        {
            var identificadores = new IdentificadoresUsuarios();

            Assert.AreEqual(ValorPorDefecto, identificadores.IdReportante);
        }

        [TestMethod]
        public void Prueba_IdReportado_ValorPorDefectoCero()
        {
            var identificadores = new IdentificadoresUsuarios();

            Assert.AreEqual(ValorPorDefecto, identificadores.IdReportado);
        }
    }
}
