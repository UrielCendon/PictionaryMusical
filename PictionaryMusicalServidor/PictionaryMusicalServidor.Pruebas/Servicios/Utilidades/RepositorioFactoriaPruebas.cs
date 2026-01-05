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
        private Mock<BaseDatosPruebaEntities> _contextoMock;

        [TestInitialize]
        public void Inicializar()
        {
            _factoria = new RepositorioFactoria();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
        }

        [TestMethod]
        public void Prueba_CrearUsuarioRepositorio_RetornaInstanciaValida()
        {
            IUsuarioRepositorio repositorio = _factoria.CrearUsuarioRepositorio(
                _contextoMock.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IUsuarioRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearUsuarioRepositorio_LlamadasConsecutivas_RetornaDiferentes()
        {
            IUsuarioRepositorio repositorioPrimero = _factoria.CrearUsuarioRepositorio(
                _contextoMock.Object);
            IUsuarioRepositorio repositorioSegundo = _factoria.CrearUsuarioRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorioPrimero, repositorioSegundo);
        }

        [TestMethod]
        public void Prueba_CrearAmigoRepositorio_RetornaInstanciaValida()
        {
            IAmigoRepositorio repositorio = _factoria.CrearAmigoRepositorio(
                _contextoMock.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IAmigoRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearAmigoRepositorio_LlamadasConsecutivas_RetornaDiferentes()
        {
            IAmigoRepositorio repositorioPrimero = _factoria.CrearAmigoRepositorio(
                _contextoMock.Object);
            IAmigoRepositorio repositorioSegundo = _factoria.CrearAmigoRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorioPrimero, repositorioSegundo);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionRepositorio_RetornaInstanciaValida()
        {
            IClasificacionRepositorio repositorio = _factoria.CrearClasificacionRepositorio(
                _contextoMock.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IClasificacionRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearClasificacionRepositorio_LlamadasConsecutivas_RetornaDiferentes()
        {
            IClasificacionRepositorio repositorioPrimero = _factoria
                .CrearClasificacionRepositorio(_contextoMock.Object);
            IClasificacionRepositorio repositorioSegundo = _factoria
                .CrearClasificacionRepositorio(_contextoMock.Object);

            Assert.AreNotSame(repositorioPrimero, repositorioSegundo);
        }

        [TestMethod]
        public void Prueba_CrearReporteRepositorio_RetornaInstanciaValida()
        {
            IReporteRepositorio repositorio = _factoria.CrearReporteRepositorio(
                _contextoMock.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IReporteRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearReporteRepositorio_LlamadasConsecutivas_RetornaDiferentes()
        {
            IReporteRepositorio repositorioPrimero = _factoria.CrearReporteRepositorio(
                _contextoMock.Object);
            IReporteRepositorio repositorioSegundo = _factoria.CrearReporteRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorioPrimero, repositorioSegundo);
        }

        [TestMethod]
        public void Prueba_CrearJugadorRepositorio_RetornaInstanciaValida()
        {
            IJugadorRepositorio repositorio = _factoria.CrearJugadorRepositorio(
                _contextoMock.Object);

            Assert.IsInstanceOfType(repositorio, typeof(IJugadorRepositorio));
        }

        [TestMethod]
        public void Prueba_CrearJugadorRepositorio_LlamadasConsecutivas_RetornaDiferentes()
        {
            IJugadorRepositorio repositorioPrimero = _factoria.CrearJugadorRepositorio(
                _contextoMock.Object);
            IJugadorRepositorio repositorioSegundo = _factoria.CrearJugadorRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorioPrimero, repositorioSegundo);
        }

        [TestMethod]
        public void Prueba_RepositorioFactoria_ImplementaIRepositorioFactoria()
        {
            Assert.IsInstanceOfType(_factoria, typeof(IRepositorioFactoria));
        }
    }
}
