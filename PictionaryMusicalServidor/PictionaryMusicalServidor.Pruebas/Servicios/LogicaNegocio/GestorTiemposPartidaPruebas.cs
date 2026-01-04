using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;

namespace PictionaryMusicalServidor.Pruebas.Servicios
{
    /// <summary>
    /// Contiene pruebas unitarias para la clase <see cref="GestorTiemposPartida"/>.
    /// Valida el calculo de puntos segun tiempo restante y el control de rondas.
    /// </summary>
    [TestClass]
    public class GestorTiemposPartidaPruebas
    {
        private const int DuracionRonda = 60;
        private const int DuracionTransicion = 5;
        private readonly DateTime FechaInicio = new DateTime(2026, 1, 4, 12, 0, 0, DateTimeKind.Utc);

        private Mock<IProveedorFecha> _proveedorFechaMock;
        private GestorTiemposPartida _gestor;

        [TestInitialize]
        public void Inicializar()
        {
            _proveedorFechaMock = new Mock<IProveedorFecha>();
            _proveedorFechaMock.Setup(p => p.ObtenerFechaActualUtc()).Returns(FechaInicio);

            _gestor = new GestorTiemposPartida(DuracionRonda, DuracionTransicion, _proveedorFechaMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_ProveedorFechaNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new GestorTiemposPartida(DuracionRonda, DuracionTransicion, null));
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_RondaNoIniciadaRetornaCero()
        {
            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(0, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_InicioRondaRetornaTotal()
        {
            _gestor.IniciarRonda();

            _proveedorFechaMock.Setup(p => p.ObtenerFechaActualUtc()).Returns(FechaInicio);

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(DuracionRonda, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_MitadDeTiempoRetornaMitadPuntos()
        {
            _gestor.IniciarRonda();

            var fechaFutura = FechaInicio.AddSeconds(30);
            _proveedorFechaMock.Setup(p => p.ObtenerFechaActualUtc()).Returns(fechaFutura);

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(30, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_TiempoExcedidoNoRetornaNegativos()
        {
            _gestor.IniciarRonda();

            var fechaFutura = FechaInicio.AddSeconds(DuracionRonda + 10);
            _proveedorFechaMock.Setup(p => p.ObtenerFechaActualUtc()).Returns(fechaFutura);

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(0, puntos);
        }

        [TestMethod]
        public void Prueba_DetenerTodo_DetieneCalculoPuntos()
        {
            _gestor.IniciarRonda();
            _gestor.DetenerTodo();

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(0, puntos);
        }
    }
}