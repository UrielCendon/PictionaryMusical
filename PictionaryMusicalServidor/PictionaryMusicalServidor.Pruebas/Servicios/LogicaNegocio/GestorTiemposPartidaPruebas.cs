using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class GestorTiemposPartidaPruebas
    {
        private const int DuracionRonda = 60;
        private const int DuracionTransicion = 5;
        private const int MitadDuracionRonda = 30;
        private const int TiempoExcedenteSegundos = 10;
        private const int PuntosCero = 0;
        private readonly DateTime FechaInicio = 
            new DateTime(2026, 1, 4, 12, 0, 0, DateTimeKind.Utc);

        private Mock<IProveedorFecha> _proveedorFechaMock;
        private GestorTiemposPartida _gestor;

        [TestInitialize]
        public void Inicializar()
        {
            _proveedorFechaMock = new Mock<IProveedorFecha>();
            _proveedorFechaMock
                .Setup(proveedor => proveedor.ObtenerFechaActualUtc())
                .Returns(FechaInicio);

            _gestor = new GestorTiemposPartida(
                DuracionRonda, 
                DuracionTransicion, 
                _proveedorFechaMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_ProveedorFechaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new GestorTiemposPartida(DuracionRonda, DuracionTransicion, null));
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_RondaNoIniciada()
        {
            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(PuntosCero, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_InicioRonda()
        {
            _gestor.IniciarRonda();

            _proveedorFechaMock
                .Setup(proveedor => proveedor.ObtenerFechaActualUtc())
                .Returns(FechaInicio);

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(DuracionRonda, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_MitadDeTiempo()
        {
            _gestor.IniciarRonda();

            var fechaFutura = FechaInicio.AddSeconds(MitadDuracionRonda);
            _proveedorFechaMock
                .Setup(proveedor => proveedor.ObtenerFechaActualUtc())
                .Returns(fechaFutura);

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(MitadDuracionRonda, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_TiempoExcedido()
        {
            _gestor.IniciarRonda();

            var fechaFutura = FechaInicio.AddSeconds(DuracionRonda + TiempoExcedenteSegundos);
            _proveedorFechaMock
                .Setup(proveedor => proveedor.ObtenerFechaActualUtc())
                .Returns(fechaFutura);

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(PuntosCero, puntos);
        }

        [TestMethod]
        public void Prueba_DetenerTodo_DetieneCalculo()
        {
            _gestor.IniciarRonda();
            _gestor.DetenerTodo();

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(PuntosCero, puntos);
        }

    }
}
