using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Salas
{
    [TestClass]
    public class SalaInternaFactoriaPruebas
    {
        private const string CodigoSalaPrueba = "123456";
        private const string CreadorPrueba = "Creador1";

        private SalaInternaFactoria _factoria;

        [TestInitialize]
        public void Inicializar()
        {
            _factoria = new SalaInternaFactoria();
        }

        [TestMethod]
        public void Prueba_Crear_RetornaSalaConCodigoCorrecto()
        {
            var configuracion = new ConfiguracionPartidaDTO();

            var sala = _factoria.Crear(CodigoSalaPrueba, CreadorPrueba, configuracion);

            Assert.AreEqual(CodigoSalaPrueba, sala.Codigo);
        }

        [TestMethod]
        public void Prueba_Crear_RetornaSalaConCreadorCorrecto()
        {
            var configuracion = new ConfiguracionPartidaDTO();

            var sala = _factoria.Crear(CodigoSalaPrueba, CreadorPrueba, configuracion);

            Assert.AreEqual(CreadorPrueba, sala.Creador);
        }

        [TestMethod]
        public void Prueba_Crear_RetornaSalaConConfiguracionCorrecta()
        {
            var configuracion = new ConfiguracionPartidaDTO { NumeroRondas = 5 };

            var sala = _factoria.Crear(CodigoSalaPrueba, CreadorPrueba, configuracion);

            Assert.IsNotNull(sala.Configuracion);
        }

        [TestMethod]
        public void Prueba_Crear_RetornaSalaConPartidaNoIniciada()
        {
            var configuracion = new ConfiguracionPartidaDTO();

            var sala = _factoria.Crear(CodigoSalaPrueba, CreadorPrueba, configuracion);

            Assert.IsFalse(sala.PartidaIniciada);
        }

        [TestMethod]
        public void Prueba_Crear_RetornaSalaConPartidaNoFinalizada()
        {
            var configuracion = new ConfiguracionPartidaDTO();

            var sala = _factoria.Crear(CodigoSalaPrueba, CreadorPrueba, configuracion);

            Assert.IsFalse(sala.PartidaFinalizada);
        }

        [TestMethod]
        public void Prueba_Crear_RetornaSalaQueNoDebeEliminarse()
        {
            var configuracion = new ConfiguracionPartidaDTO();

            var sala = _factoria.Crear(CodigoSalaPrueba, CreadorPrueba, configuracion);

            Assert.IsFalse(sala.DebeEliminarse);
        }
    }
}
