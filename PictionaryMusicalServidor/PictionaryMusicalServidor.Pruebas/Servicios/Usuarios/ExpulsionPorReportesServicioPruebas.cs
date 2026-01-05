using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class ExpulsionPorReportesServicioPruebas
    {
        private const int IdUsuarioValido = 1;
        private const int IdUsuarioInvalido = 0;
        private const int IdUsuarioNegativo = -1;
        private const int TotalReportesBajoLimite = 2;
        private const int TotalReportesLimite = 3;
        private const int TotalReportesSobreLimite = 5;
        private const string NombreUsuarioValido = "UsuarioReportado";
        private const string NombreUsuarioCreador = "UsuarioCreador";
        private const string CodigoSalaValido = "123456";
        private const string CadenaVacia = "";
        private const string CadenaSoloEspacios = "   ";

        private Mock<ISalasProveedor> _salasProveedorMock;
        private Mock<ISalaExpulsor> _salaExpulsorMock;
        private ExpulsionPorReportesServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _salasProveedorMock = new Mock<ISalasProveedor>();
            _salaExpulsorMock = new Mock<ISalaExpulsor>();

            _servicio = new ExpulsionPorReportesServicio(
                _salasProveedorMock.Object,
                _salaExpulsorMock.Object);
        }

        private SalaDTO CrearSalaConJugador(
            string codigoSala,
            string creador,
            params string[] jugadores)
        {
            return new SalaDTO
            {
                Codigo = codigoSala,
                Creador = creador,
                Jugadores = new List<string>(jugadores)
            };
        }

        [TestMethod]
        public void Prueba_Constructor_SalasProveedorNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ExpulsionPorReportesServicio(null, _salaExpulsorMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_SalaExpulsorNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ExpulsionPorReportesServicio(_salasProveedorMock.Object, null));
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_IdCero_NoExpulsa()
        {
            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioInvalido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salasProveedorMock.Verify(
                proveedor => proveedor.ObtenerListaSalas(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_IdNegativo_NoExpulsa()
        {
            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioNegativo, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salasProveedorMock.Verify(
                proveedor => proveedor.ObtenerListaSalas(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ReportesBajoLimite_NoExpulsa()
        {
            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesBajoLimite);

            _salasProveedorMock.Verify(
                proveedor => proveedor.ObtenerListaSalas(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NombreVacio_NoExpulsa()
        {
            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                CadenaVacia, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.AbandonarSala(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NombreSoloEspacios_NoExpulsa()
        {
            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                CadenaSoloEspacios, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.AbandonarSala(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_NombreNulo_NoExpulsa()
        {
            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                null, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.AbandonarSala(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ListaSalasVacia_NoExpulsa()
        {
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO>());

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.AbandonarSala(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ListaSalasNula_NoExpulsa()
        {
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns((IList<SalaDTO>)null);

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.AbandonarSala(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_JugadorNoEnSala_NoExpulsa()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioCreador, 
                "OtroJugador");
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.AbandonarSala(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_JugadorEsCreador_AbandonaSala()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioValido, 
                NombreUsuarioValido);
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.AbandonarSala(CodigoSalaValido, NombreUsuarioValido),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_JugadorNoEsCreador_BaneaJugador()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioCreador, 
                NombreUsuarioValido, NombreUsuarioCreador);
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(CodigoSalaValido, NombreUsuarioValido),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_JugadorEnMultiplesSalas_ExpulsaDeTodas()
        {
            var sala1 = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioCreador, 
                NombreUsuarioValido);
            var sala2 = CrearSalaConJugador(
                "654321", 
                NombreUsuarioCreador, 
                NombreUsuarioValido);
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala1, sala2 });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), NombreUsuarioValido),
                Times.Exactly(2));
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ReportesSobreLimite_Expulsa()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioCreador, 
                NombreUsuarioValido);
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesSobreLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(CodigoSalaValido, NombreUsuarioValido),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ExpulsionFalla_NoPropagaExcepcion()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioCreador, 
                NombreUsuarioValido);
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });
            _salaExpulsorMock
                .Setup(expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new FaultException());

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(CodigoSalaValido, NombreUsuarioValido),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ExcepcionGenerica_NoPropagaExcepcion()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioCreador, 
                NombreUsuarioValido);
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });
            _salaExpulsorMock
                .Setup(expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(CodigoSalaValido, NombreUsuarioValido),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_SalaNula_NoPropagaExcepcion()
        {
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { null });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_JugadoresNulo_NoPropagaExcepcion()
        {
            var sala = new SalaDTO
            {
                Codigo = CodigoSalaValido,
                Creador = NombreUsuarioCreador,
                Jugadores = null
            };
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ComparacionCaseInsensitive_Expulsa()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioCreador, 
                NombreUsuarioValido.ToUpper());
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido.ToLower(), 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.BanearJugador(
                    CodigoSalaValido, 
                    NombreUsuarioValido.ToLower()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_CreadorCaseInsensitive_AbandonaSala()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioValido.ToUpper(), 
                NombreUsuarioValido);
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido.ToLower(), 
                TotalReportesLimite);

            _salaExpulsorMock.Verify(
                expulsor => expulsor.AbandonarSala(
                    CodigoSalaValido, 
                    NombreUsuarioValido.ToLower()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarSiAlcanzaLimite_ReportesExactamenteLimite_Expulsa()
        {
            var sala = CrearSalaConJugador(
                CodigoSalaValido, 
                NombreUsuarioCreador, 
                NombreUsuarioValido);
            _salasProveedorMock
                .Setup(proveedor => proveedor.ObtenerListaSalas())
                .Returns(new List<SalaDTO> { sala });

            _servicio.ExpulsarSiAlcanzaLimite(
                IdUsuarioValido, 
                NombreUsuarioValido, 
                TotalReportesLimite);

            _salasProveedorMock.Verify(
                proveedor => proveedor.ObtenerListaSalas(),
                Times.Once);
        }
    }
}
