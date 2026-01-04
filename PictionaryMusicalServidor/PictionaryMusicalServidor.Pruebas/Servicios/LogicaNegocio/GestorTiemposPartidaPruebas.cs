using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using System.Threading;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class GestorTiemposPartidaPruebas
    {
        private const int DuracionRondaSegundos = 60;
        private const int DuracionTransicionSegundos = 5;
        private const int DuracionRondaRapidaSegundos = 1;
        private const int TiempoEsperaEventoMilisegundos = 1500;
        private const int PuntosMinimosEsperados = 58;

        private GestorTiemposPartida _gestor;

        [TestInitialize]
        public void Inicializar()
        {
            _gestor = new GestorTiemposPartida(DuracionRondaSegundos, DuracionTransicionSegundos);
        }

        [TestCleanup]
        public void Limpiar()
        {
            _gestor?.Dispose();
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempoRondaNoIniciada_RetornaCero()
        {

            var puntos = _gestor.CalcularPuntosPorTiempo();


            Assert.AreEqual(0, puntos);
        }

        [TestMethod]
        public void Prueba_CalcularPuntosPorTiempoRondaIniciada_RetornaPuntosAltos()
        {

            _gestor.IniciarRonda();


            var puntos = _gestor.CalcularPuntosPorTiempo();


            Assert.IsTrue(puntos >= PuntosMinimosEsperados);
        }

        [TestMethod]
        public void Prueba_IniciarRondaTiempoAgotado_DisparaEvento()
        {
            var eventoRecibido = new ManualResetEvent(false);
            var gestorRapido = new GestorTiemposPartida(
                DuracionRondaRapidaSegundos, 
                DuracionRondaRapidaSegundos);
            bool eventoDisparado = false;
            gestorRapido.TiempoRondaAgotado += () => 
            {
                eventoDisparado = true;
                eventoRecibido.Set();
            };


            gestorRapido.IniciarRonda();
            eventoRecibido.WaitOne(TiempoEsperaEventoMilisegundos);


            Assert.IsTrue(eventoDisparado);

            gestorRapido.Dispose();
        }

        [TestMethod]
        public void Prueba_IniciarTransicionAgotada_DisparaEvento()
        {
            var eventoRecibido = new ManualResetEvent(false);
            var gestorRapido = new GestorTiemposPartida(
                DuracionTransicionSegundos, 
                DuracionRondaRapidaSegundos);
            bool eventoDisparado = false;
            gestorRapido.TiempoTransicionAgotado += () => 
            {
                eventoDisparado = true;
                eventoRecibido.Set();
            };


            gestorRapido.IniciarTransicion();
            eventoRecibido.WaitOne(TiempoEsperaEventoMilisegundos);


            Assert.IsTrue(eventoDisparado);

            gestorRapido.Dispose();
        }

        [TestMethod]
        public void Prueba_DetenerTodo_DetieneTemporalizadores()
        {

            _gestor.IniciarRonda();


            _gestor.DetenerTodo();
            var puntosDetenido = _gestor.CalcularPuntosPorTiempo();


            Assert.AreEqual(0, puntosDetenido);
        }

        [TestMethod]
        public void Prueba_DisposeMultiple_NoLanzaExcepcion()
        {

            var gestorDispose = new GestorTiemposPartida(
                DuracionRondaSegundos, 
                DuracionTransicionSegundos);
            gestorDispose.IniciarRonda();


            gestorDispose.Dispose();
            gestorDispose.Dispose();

            int puntosDespuesDispose = gestorDispose.CalcularPuntosPorTiempo();
            Assert.AreEqual(0, puntosDespuesDispose);
        }
    }
}
