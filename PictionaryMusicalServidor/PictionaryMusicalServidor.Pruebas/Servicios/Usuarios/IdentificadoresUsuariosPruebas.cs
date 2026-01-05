using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class IdentificadoresUsuariosPruebas
    {
        private const int IdReportanteValido = 1;
        private const int IdReportadoValido = 2;
        private const int IdReportanteDiferente = 10;
        private const int IdReportadoDiferente = 20;
        private const int ValorPorDefecto = 0;

        [TestMethod]
        public void Prueba_AmbosIdentificadores_AsignarYRecuperar_ValoresCorrectos()
        {
            var identificadores = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteValido,
                IdReportado = IdReportadoValido
            };

            Assert.AreEqual(IdReportanteValido, identificadores.IdReportante);
            Assert.AreEqual(IdReportadoValido, identificadores.IdReportado);
        }

        [TestMethod]
        public void Prueba_ModificarIdentificadores_NuevosValores_ValoresActualizados()
        {
            var identificadores = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteValido,
                IdReportado = IdReportadoValido
            };

            identificadores.IdReportante = IdReportanteDiferente;
            identificadores.IdReportado = IdReportadoDiferente;

            Assert.AreEqual(IdReportanteDiferente, identificadores.IdReportante);
            Assert.AreEqual(IdReportadoDiferente, identificadores.IdReportado);
        }

        [TestMethod]
        public void Prueba_Constructor_SinAsignacion_ValoresPorDefecto()
        {
            var identificadores = new IdentificadoresUsuarios();

            Assert.AreEqual(ValorPorDefecto, identificadores.IdReportante);
            Assert.AreEqual(ValorPorDefecto, identificadores.IdReportado);
        }

        [TestMethod]
        public void Prueba_DiferentesInstancias_AsignarValores_NoCompartenEstado()
        {
            var primeraInstancia = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteValido,
                IdReportado = IdReportadoValido
            };
            var segundaInstancia = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteDiferente,
                IdReportado = IdReportadoDiferente
            };

            Assert.AreEqual(IdReportanteValido, primeraInstancia.IdReportante);
            Assert.AreEqual(IdReportanteDiferente, segundaInstancia.IdReportante);
            Assert.AreEqual(IdReportadoValido, primeraInstancia.IdReportado);
            Assert.AreEqual(IdReportadoDiferente, segundaInstancia.IdReportado);
        }

        [TestMethod]
        public void Prueba_MismoValorEnAmbosIds_AsignarIgual_PermiteAsignacion()
        {
            var identificadores = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteValido,
                IdReportado = IdReportanteValido
            };

            Assert.AreEqual(identificadores.IdReportante, identificadores.IdReportado);
        }
    }
}
