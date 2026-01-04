using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using System.Linq;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class GestorJugadoresPartidaPruebas
    {
        private const int PuntajeBajo = 50;
        private const int PuntajeMedio = 100;
        private const int PuntajeAlto = 150;
        private const int CantidadJugadoresEsperada = 2;

        private GestorJugadoresPartida _gestor;

        [TestInitialize]
        public void Inicializar()
        {
            _gestor = new GestorJugadoresPartida();
        }

        [TestMethod]
        public void Prueba_HaySuficientesJugadoresMenosDeDos_RetornaFalse()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);


            var resultado = _gestor.HaySuficientesJugadores;


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_HaySuficientesJugadoresDosOMas_RetornaTrue()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);
            _gestor.Agregar("conexion-2", "Jugador2", false);


            var resultado = _gestor.HaySuficientesJugadores;


            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_AgregarJugadorNuevo_AgregaCorrectamente()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);


            var jugador = _gestor.Obtener("conexion-1");
            Assert.AreEqual("Jugador1", jugador.NombreUsuario);
            Assert.IsTrue(jugador.EsHost);
        }

        [TestMethod]
        public void Prueba_AgregarJugadorExistente_ActualizaDatos()
        {

            _gestor.Agregar("conexion-1", "JugadorOriginal", false);


            _gestor.Agregar("conexion-1", "JugadorActualizado", true);


            var jugador = _gestor.Obtener("conexion-1");
            Assert.AreEqual("JugadorActualizado", jugador.NombreUsuario);
            Assert.IsTrue(jugador.EsHost);
        }

        [TestMethod]
        public void Prueba_ObtenerIdInexistente_RetornaNull()
        {

            var jugador = _gestor.Obtener("id-inexistente");


            Assert.IsNull(jugador);
        }

        [TestMethod]
        public void Prueba_RemoverJugadorExistente_RemueveCorrectamente()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);


            bool eraDibujante;
            var resultado = _gestor.Remover("conexion-1", out eraDibujante);


            Assert.IsTrue(resultado);
            Assert.IsNull(_gestor.Obtener("conexion-1"));
        }

        [TestMethod]
        public void Prueba_RemoverJugadorInexistente_RetornaFalse()
        {

            bool eraDibujante;
            var resultado = _gestor.Remover("id-inexistente", out eraDibujante);


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsHostJugadorHost_RetornaTrue()
        {

            _gestor.Agregar("conexion-host", "HostJugador", true);


            var resultado = _gestor.EsHost("conexion-host");


            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsHostJugadorNoHost_RetornaFalse()
        {

            _gestor.Agregar("conexion-normal", "JugadorNormal", false);


            var resultado = _gestor.EsHost("conexion-normal");


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_PrepararColaDibujantes_PreparaCola()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);
            _gestor.Agregar("conexion-2", "Jugador2", false);


            _gestor.PrepararColaDibujantes();


            Assert.IsTrue(_gestor.QuedanDibujantesPendientes());
        }

        [TestMethod]
        public void Prueba_SeleccionarSiguienteDibujanteConCola_RetornaTrue()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);
            _gestor.Agregar("conexion-2", "Jugador2", false);
            _gestor.PrepararColaDibujantes();


            var resultado = _gestor.SeleccionarSiguienteDibujante();


            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_SeleccionarSiguienteDibujanteSinCola_RetornaFalse()
        {



            var resultado = _gestor.SeleccionarSiguienteDibujante();


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_SeleccionarSiguienteDibujante_MarcaJugador()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);
            _gestor.PrepararColaDibujantes();


            _gestor.SeleccionarSiguienteDibujante();


            var jugador = _gestor.Obtener("conexion-1");
            Assert.IsTrue(jugador.EsDibujante);
            Assert.IsTrue(jugador.YaAdivino);
        }

        [TestMethod]
        public void Prueba_TodosAdivinaron_RetornaTrue()
        {
            _gestor.Agregar("conexion-1", "Jugador1", true);
            _gestor.Agregar("conexion-2", "Jugador2", false);
            _gestor.PrepararColaDibujantes();
            _gestor.SeleccionarSiguienteDibujante();

            var jugador1 = _gestor.Obtener("conexion-1");
            var jugador2 = _gestor.Obtener("conexion-2");
            if (!jugador1.EsDibujante)
            {
                jugador1.YaAdivino = true;
            }
            if (!jugador2.EsDibujante)
            {
                jugador2.YaAdivino = true;
            }

            var resultado = _gestor.TodosAdivinaron();

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_TodosAdivinaronParcial_RetornaFalse()
        {
            _gestor.Agregar("conexion-1", "Jugador1", true);
            _gestor.Agregar("conexion-2", "Jugador2", false);
            _gestor.Agregar("conexion-3", "Jugador3", false);
            _gestor.PrepararColaDibujantes();
            _gestor.SeleccionarSiguienteDibujante();

            var jugador1 = _gestor.Obtener("conexion-1");
            var jugador2 = _gestor.Obtener("conexion-2");
            var jugador3 = _gestor.Obtener("conexion-3");
            
            int adivinadoresCount = 0;
            if (!jugador1.EsDibujante) adivinadoresCount++;
            if (!jugador2.EsDibujante) adivinadoresCount++;
            if (!jugador3.EsDibujante) adivinadoresCount++;
            
            if (!jugador1.EsDibujante && adivinadoresCount > 1)
            {
                jugador1.YaAdivino = true;
            }
            else if (!jugador2.EsDibujante)
            {
                jugador2.YaAdivino = true;
            }

            var resultado = _gestor.TodosAdivinaron();

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_GenerarClasificacion_OrdenaPorPuntaje()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);
            _gestor.Agregar("conexion-2", "Jugador2", false);
            _gestor.Agregar("conexion-3", "Jugador3", false);

            _gestor.Obtener("conexion-1").PuntajeTotal = PuntajeBajo;
            _gestor.Obtener("conexion-2").PuntajeTotal = PuntajeAlto;
            _gestor.Obtener("conexion-3").PuntajeTotal = PuntajeMedio;


            var clasificacion = _gestor.GenerarClasificacion();


            Assert.AreEqual("Jugador2", clasificacion[0].Usuario);
            Assert.AreEqual("Jugador3", clasificacion[1].Usuario);
            Assert.AreEqual("Jugador1", clasificacion[2].Usuario);
        }

        [TestMethod]
        public void Prueba_ObtenerCopiaLista_RetornaCopia()
        {

            _gestor.Agregar("conexion-1", "Jugador1", true);
            _gestor.Agregar("conexion-2", "Jugador2", false);


            var copia = _gestor.ObtenerCopiaLista();


            Assert.AreEqual(CantidadJugadoresEsperada, copia.Count);
        }

        [TestMethod]
        public void Prueba_QuedanDibujantesPendientesColaVacia_RetornaFalse()
        {

            var resultado = _gestor.QuedanDibujantesPendientes();


            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_RemoverJugadorDibujante_IndicaEraDibujante()
        {

            _gestor.Agregar("conexion-1", "Dibujante", true);
            _gestor.PrepararColaDibujantes();
            _gestor.SeleccionarSiguienteDibujante();


            bool eraDibujante;
            _gestor.Remover("conexion-1", out eraDibujante);


            Assert.IsTrue(eraDibujante);
        }

        [TestMethod]
        public void Prueba_RemoverConTresParametros_RetornaNombre()
        {

            _gestor.Agregar("conexion-1", "JugadorRemover", true);


            bool eraDibujante;
            string nombreUsuario;
            var resultado = _gestor.Remover("conexion-1", out eraDibujante, out nombreUsuario);


            Assert.IsTrue(resultado);
            Assert.AreEqual("JugadorRemover", nombreUsuario);
        }
    }
}
