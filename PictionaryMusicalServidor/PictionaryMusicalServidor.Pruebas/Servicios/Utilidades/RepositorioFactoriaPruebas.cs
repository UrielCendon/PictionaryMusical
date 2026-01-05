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

        #region Pruebas CrearUsuarioRepositorio

        [TestMethod]
        public void Prueba_CrearUsuarioRepositorio_RetornaInstanciaValida()
        {
            IUsuarioRepositorio repositorio = _factoria.CrearUsuarioRepositorio(
                _contextoMock.Object);

            Assert.IsTrue(repositorio is IUsuarioRepositorio);
        }

        [TestMethod]
        public void Prueba_CrearUsuarioRepositorio_LlamadasConsecutivas_RetornanInstanciasDiferentes()
        {
            IUsuarioRepositorio repositorio1 = _factoria.CrearUsuarioRepositorio(
                _contextoMock.Object);
            IUsuarioRepositorio repositorio2 = _factoria.CrearUsuarioRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorio1, repositorio2);
        }

        #endregion

        #region Pruebas CrearAmigoRepositorio

        [TestMethod]
        public void Prueba_CrearAmigoRepositorio_RetornaInstanciaValida()
        {
            IAmigoRepositorio repositorio = _factoria.CrearAmigoRepositorio(
                _contextoMock.Object);

            Assert.IsTrue(repositorio is IAmigoRepositorio);
        }

        [TestMethod]
        public void Prueba_CrearAmigoRepositorio_LlamadasConsecutivas_RetornanInstanciasDiferentes()
        {
            IAmigoRepositorio repositorio1 = _factoria.CrearAmigoRepositorio(
                _contextoMock.Object);
            IAmigoRepositorio repositorio2 = _factoria.CrearAmigoRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorio1, repositorio2);
        }

        #endregion

        #region Pruebas CrearClasificacionRepositorio

        [TestMethod]
        public void Prueba_CrearClasificacionRepositorio_RetornaInstanciaValida()
        {
            IClasificacionRepositorio repositorio = _factoria.CrearClasificacionRepositorio(
                _contextoMock.Object);

            Assert.IsTrue(repositorio is IClasificacionRepositorio);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionRepositorio_LlamadasConsecutivas_RetornanDiferentes()
        {
            IClasificacionRepositorio repositorio1 = _factoria.CrearClasificacionRepositorio(
                _contextoMock.Object);
            IClasificacionRepositorio repositorio2 = _factoria.CrearClasificacionRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorio1, repositorio2);
        }

        #endregion

        #region Pruebas CrearReporteRepositorio

        [TestMethod]
        public void Prueba_CrearReporteRepositorio_RetornaInstanciaValida()
        {
            IReporteRepositorio repositorio = _factoria.CrearReporteRepositorio(
                _contextoMock.Object);

            Assert.IsTrue(repositorio is IReporteRepositorio);
        }

        [TestMethod]
        public void Prueba_CrearReporteRepositorio_LlamadasConsecutivas_RetornanInstanciasDiferentes()
        {
            IReporteRepositorio repositorio1 = _factoria.CrearReporteRepositorio(
                _contextoMock.Object);
            IReporteRepositorio repositorio2 = _factoria.CrearReporteRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorio1, repositorio2);
        }

        #endregion

        #region Pruebas CrearJugadorRepositorio

        [TestMethod]
        public void Prueba_CrearJugadorRepositorio_RetornaInstanciaValida()
        {
            IJugadorRepositorio repositorio = _factoria.CrearJugadorRepositorio(
                _contextoMock.Object);

            Assert.IsTrue(repositorio is IJugadorRepositorio);
        }

        [TestMethod]
        public void Prueba_CrearJugadorRepositorio_LlamadasConsecutivas_RetornanInstanciasDiferentes()
        {
            IJugadorRepositorio repositorio1 = _factoria.CrearJugadorRepositorio(
                _contextoMock.Object);
            IJugadorRepositorio repositorio2 = _factoria.CrearJugadorRepositorio(
                _contextoMock.Object);

            Assert.AreNotSame(repositorio1, repositorio2);
        }

        #endregion

        #region Pruebas Implementacion Interfaz

        [TestMethod]
        public void Prueba_RepositorioFactoria_ImplementaIRepositorioFactoria()
        {
            Assert.IsTrue(_factoria is IRepositorioFactoria);
        }

        #endregion
    }
}
