using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class ExpulsionPorReportesServicioPruebas
    {
        private const int IdUsuarioValido = 1;
        private const int IdUsuarioInvalido = 0;
        private const int TotalReportesParaExpulsion = 3;
        private const int TotalReportesInsuficientes = 2;
        private const string NombreUsuarioValido = "UsuarioPrueba";
        private const string NombreUsuarioCreador = "Creador";
        private const string CodigoSala = "123456";

        private Mock<ISalasProveedor> _mockSalasProveedor;
        private Mock<ISalaExpulsor> _mockSalaExpulsor;

        [TestInitialize]
        public void Inicializar()
        {
            _mockSalasProveedor = new Mock<ISalasProveedor>();
            _mockSalaExpulsor = new Mock<ISalaExpulsor>();
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionSalasProveedorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ExpulsionPorReportesServicio(null, _mockSalaExpulsor.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionSalaExpulsorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ExpulsionPorReportesServicio(_mockSalasProveedor.Object, null));
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NoExpulsaConIdInvalido()
        {
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioInvalido, 
                NombreUsuarioValido, 
                TotalReportesParaExpulsion);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NoExpulsaConReportesInsuficientes()
        {
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesInsuficientes);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NoExpulsaConNombreUsuarioVacio()
        {
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                string.Empty, 
                TotalReportesParaExpulsion);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NoExpulsaConNombreUsuarioNulo()
        {
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                null, 
                TotalReportesParaExpulsion);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NoExpulsaSinSalasActivas()
        {
            _mockSalasProveedor.Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO>());
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesParaExpulsion);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NoExpulsaJugadorNoEnSala()
        {
            var salas = CrearListaSalasConOtroJugador();
            _mockSalasProveedor.Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(salas);
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesParaExpulsion);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_BaneaJugadorNoCreador()
        {
            var salas = CrearListaSalasConJugadorComoMiembro();
            _mockSalasProveedor.Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(salas);
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesParaExpulsion);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.BanearJugador(CodigoSala, NombreUsuarioValido), Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_AbandonaSalaComoCreador()
        {
            var salas = CrearListaSalasConJugadorComoCreador();
            _mockSalasProveedor.Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(salas);
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesParaExpulsion);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.AbandonarSala(CodigoSala, NombreUsuarioValido), Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ManejaExcepcionDuranteExpulsion()
        {
            var salas = CrearListaSalasConJugadorComoMiembro();
            _mockSalasProveedor.Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(salas);
            _mockSalaExpulsor.Setup(expulsor => 
                expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesParaExpulsion);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NoExpulsaCuandoListaSalasEsNula()
        {
            _mockSalasProveedor.Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns((IList<SalaDTO>)null);
            var servicio = CrearServicio();

            servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesParaExpulsion);

            _mockSalaExpulsor.Verify(expulsor => 
                expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        private ExpulsionPorReportesServicio CrearServicio()
        {
            return new ExpulsionPorReportesServicio(
                _mockSalasProveedor.Object, 
                _mockSalaExpulsor.Object);
        }

        private IList<SalaDTO> CrearListaSalasConOtroJugador()
        {
            return new List<SalaDTO>
            {
                new SalaDTO
                {
                    Codigo = CodigoSala,
                    Creador = NombreUsuarioCreador,
                    Jugadores = new List<string> { "OtroJugador" }
                }
            };
        }

        private IList<SalaDTO> CrearListaSalasConJugadorComoMiembro()
        {
            return new List<SalaDTO>
            {
                new SalaDTO
                {
                    Codigo = CodigoSala,
                    Creador = NombreUsuarioCreador,
                    Jugadores = new List<string> { NombreUsuarioValido, NombreUsuarioCreador }
                }
            };
        }

        private IList<SalaDTO> CrearListaSalasConJugadorComoCreador()
        {
            return new List<SalaDTO>
            {
                new SalaDTO
                {
                    Codigo = CodigoSala,
                    Creador = NombreUsuarioValido,
                    Jugadores = new List<string> { NombreUsuarioValido }
                }
            };
        }
    }
}
