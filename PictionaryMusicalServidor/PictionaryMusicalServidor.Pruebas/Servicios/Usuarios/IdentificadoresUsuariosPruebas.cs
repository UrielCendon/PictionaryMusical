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
        public void Prueba_IdReportante_AsignarYRecuperar_ValorCorrecto()
        {
            var identificadores = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteValido
            };

            Assert.AreEqual(IdReportanteValido, identificadores.IdReportante);
        }

        [TestMethod]
        public void Prueba_IdReportado_AsignarYRecuperar_ValorCorrecto()
        {
            var identificadores = new IdentificadoresUsuarios
            {
                IdReportado = IdReportadoValido
            };

            Assert.AreEqual(IdReportadoValido, identificadores.IdReportado);
        }

        [TestMethod]
        public void Prueba_AmbosIdentificadores_AsignarYRecuperar_ValoresCrrectos()
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
        public void Prueba_IdReportante_ModificarValor_NuevoValorCorrecto()
        {
            var identificadores = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteValido
            };

            identificadores.IdReportante = IdReportanteDiferente;

            Assert.AreEqual(IdReportanteDiferente, identificadores.IdReportante);
        }

        [TestMethod]
        public void Prueba_IdReportado_ModificarValor_NuevoValorCorrecto()
        {
            var identificadores = new IdentificadoresUsuarios
            {
                IdReportado = IdReportadoValido
            };

            identificadores.IdReportado = IdReportadoDiferente;

            Assert.AreEqual(IdReportadoDiferente, identificadores.IdReportado);
        }

        [TestMethod]
        public void Prueba_Constructor_SinAsignacion_IdReportantePorDefecto()
        {
            var identificadores = new IdentificadoresUsuarios();

            Assert.AreEqual(ValorPorDefecto, identificadores.IdReportante);
        }

        [TestMethod]
        public void Prueba_Constructor_SinAsignacion_IdReportadoPorDefecto()
        {
            var identificadores = new IdentificadoresUsuarios();

            Assert.AreEqual(ValorPorDefecto, identificadores.IdReportado);
        }

        [TestMethod]
        public void Prueba_Identificadores_DiferentesInstancias_NoCompartenEstado()
        {
            var identificadores1 = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteValido,
                IdReportado = IdReportadoValido
            };
            var identificadores2 = new IdentificadoresUsuarios
            {
                IdReportante = IdReportanteDiferente,
                IdReportado = IdReportadoDiferente
            };

            Assert.AreEqual(IdReportanteValido, identificadores1.IdReportante);
            Assert.AreEqual(IdReportanteDiferente, identificadores2.IdReportante);
        }

        [TestMethod]
        public void Prueba_Identificadores_MismoValorEnAmbos_PermiteAsignacion()
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
