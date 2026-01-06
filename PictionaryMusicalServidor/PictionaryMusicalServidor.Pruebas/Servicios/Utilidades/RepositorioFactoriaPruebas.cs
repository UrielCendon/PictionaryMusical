using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Utilidades
{
    [TestClass]
    public class RepositorioFactoriaPruebas
    {
        private RepositorioFactoria _factoria;
        private Mock<BaseDatosPruebaEntities> _mockContexto;

        [TestInitialize]
        public void Inicializar()
        {
            _factoria = new RepositorioFactoria();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();
        }

        [TestMethod]
        public void Prueba_CrearUsuarioRepositorio_RetornaInstanciaValida()
        {
            var repositorio = _factoria.CrearUsuarioRepositorio(_mockContexto.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IUsuarioRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearAmigoRepositorio_RetornaInstanciaValida()
        {
            var repositorio = _factoria.CrearAmigoRepositorio(_mockContexto.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IAmigoRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearClasificacionRepositorio_RetornaInstanciaValida()
        {
            var repositorio = _factoria.CrearClasificacionRepositorio(_mockContexto.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IClasificacionRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearReporteRepositorio_RetornaInstanciaValida()
        {
            var repositorio = _factoria.CrearReporteRepositorio(_mockContexto.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IReporteRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearJugadorRepositorio_RetornaInstanciaValida()
        {
            var repositorio = _factoria.CrearJugadorRepositorio(_mockContexto.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IJugadorRepositorio));
        }
    }
}
