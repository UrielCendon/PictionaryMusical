using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.Utilidades;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class GestorTiemposPartidaPruebas
    {
        private const int DuracionRondaSegundos = 60;
        private const int DuracionTransicionSegundos = 5;
        private const int PuntosInicialCero = 0;

        private Mock<IProveedorFecha> _mockProveedorFecha;
        private GestorTiemposPartida _gestor;

        [TestInitialize]
        public void Inicializar()
        {
            _mockProveedorFecha = new Mock<IProveedorFecha>();
            _mockProveedorFecha
                .Setup(p => p.ObtenerFechaActualUtc())
                .Returns(DateTime.UtcNow);
            _gestor = new GestorTiemposPartida(
                DuracionRondaSegundos,
                DuracionTransicionSegundos,
                _mockProveedorFecha.Object);
        }

        [TestCleanup]
        public void Limpiar()
        {
            _gestor?.Dispose();
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionProveedorFechaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new GestorTiemposPartida(
                    DuracionRondaSegundos, 
                    DuracionTransicionSegundos, 
                    null));
        }

        [TestMethod]
        public void Prueba_Constructor_CreaInstanciaCorrectamente()
        {
            var gestor = new GestorTiemposPartida(
                DuracionRondaSegundos,
                DuracionTransicionSegundos,
                _mockProveedorFecha.Object);

            Assert.IsInstanceOfType(gestor, typeof(GestorTiemposPartida));
            gestor.Dispose();
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_RetornaCeroSinRonda()
        {
            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(PuntosInicialCero, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_RetornaValorDuranteRonda()
        {
            var fechaInicio = DateTime.UtcNow;
            _mockProveedorFecha
                .Setup(p => p.ObtenerFechaActualUtc())
                .Returns(fechaInicio);
            _gestor.IniciarRonda();
            _mockProveedorFecha
                .Setup(p => p.ObtenerFechaActualUtc())
                .Returns(fechaInicio.AddSeconds(10));

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(DuracionRondaSegundos - 10, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempo_RetornaCeroDespuesDeDetener()
        {
            _gestor.IniciarRonda();
            _gestor.DetenerTodo();

            int puntos = _gestor.CalcularPuntosPorTiempo();

            Assert.AreEqual(PuntosInicialCero, puntos);
        }
    }
}
