using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class GestorJugadoresPartidaPruebas
    {
        private const string IdJugadorUno = "ConexionA";
        private const string NombreJugadorUno = "JugadorA";
        private const string IdJugadorDos = "ConexionB";
        private const string NombreJugadorDos = "JugadorB";
        private const string IdInexistente = "ConexionX";
        private const string NuevoNombre = "NombreCambiado";
        private const int PuntajeAlto = 100;
        private const int PuntajeBajo = 50;
        private const int CantidadEsperadaUno = 1;
        private const int IndicePrimero = 0;
        private const int IndiceSegundo = 1;

        private Mock<IGeneradorAleatorio> _generadorMock;
        private GestorJugadoresPartida _gestor;

        [TestInitialize]
        public void Inicializar()
        {
            _generadorMock = new Mock<IGeneradorAleatorio>();

            _generadorMock.Setup(generador => generador.MezclarLista(It.IsAny<IList<string>>()))
                .Callback<IList<string>>((lista) => { });

            _gestor = new GestorJugadoresPartida(_generadorMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_GeneradorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new GestorJugadoresPartida(null));
        }

        // fix el flujo deberia validar solo que el jugador exista y sin usar el not null
        [TestMethod]
        public void Prueba_Agregar_JugadorNuevo()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            var jugador = _gestor.Obtener(IdJugadorUno);

            Assert.IsNotNull(jugador);
            Assert.AreEqual(NombreJugadorUno, jugador.NombreUsuario);
            Assert.IsTrue(jugador.EsHost);
        }

        // fix el flujo debería validar solo que se actualizo el nombre
        [TestMethod]
        public void Prueba_Agregar_JugadorExistente()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            _gestor.Agregar(IdJugadorUno, NuevoNombre, false);
            var jugador = _gestor.Obtener(IdJugadorUno);

            Assert.AreEqual(NuevoNombre, jugador.NombreUsuario);
            Assert.IsFalse(jugador.EsHost);
        }

        [TestMethod]
        public void Prueba_HaySuficientesJugadores_MenosDeDos()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            Assert.IsFalse(_gestor.HaySuficientesJugadores);
        }

        [TestMethod]
        public void Prueba_HaySuficientesJugadores_DosOMas()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);

            Assert.IsTrue(_gestor.HaySuficientesJugadores);
        }

        // fix el flujo debería validar solo el resultado true al remover
        [TestMethod]
        public void Prueba_Remover_JugadorExistente()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            
            bool resultado = _gestor.Remover(IdJugadorUno, out _, out string nombreSalida);

            Assert.IsTrue(resultado);
            Assert.AreEqual(NombreJugadorUno, nombreSalida);
            Assert.IsNull(_gestor.Obtener(IdJugadorUno));
        }

        [TestMethod]
        public void Prueba_Remover_JugadorInexistente()
        {
            bool resultado = _gestor.Remover(IdInexistente, out _, out string nombreSalida);

            Assert.IsFalse(resultado);
            Assert.IsNull(nombreSalida);
        }

        [TestMethod]
        public void Prueba_EsHost_JugadorHost()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            Assert.IsTrue(_gestor.EsHost(IdJugadorUno));
        }

        [TestMethod]
        public void Prueba_PrepararColaDibujantes_InvocaMezclador()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);

            _gestor.PrepararColaDibujantes();

            _generadorMock.Verify(generador => generador.MezclarLista(It.IsAny<IList<string>>()), Times.Once);
            Assert.IsTrue(_gestor.QuedanDibujantesPendientes());
        }

        // fix el flujo debería validar solo que el siguiente tenga el rol no mas cosas
        [TestMethod]
        public void Prueba_SeleccionarSiguienteDibujante_AsignaRol()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.PrepararColaDibujantes();

            bool resultado = _gestor.SeleccionarSiguienteDibujante();
            var jugador = _gestor.Obtener(IdJugadorUno);

            Assert.IsTrue(resultado);
            Assert.IsTrue(jugador.EsDibujante);
            Assert.IsTrue(jugador.YaAdivino);
        }

        [TestMethod]
        public void Prueba_TodosAdivinaron_FaltaAlguien()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);
            _gestor.PrepararColaDibujantes();

            _gestor.SeleccionarSiguienteDibujante();

            Assert.IsFalse(_gestor.TodosAdivinaron());
        }

        [TestMethod]
        public void Prueba_TodosAdivinaron_TodosListos()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);
            _gestor.PrepararColaDibujantes();

            _gestor.SeleccionarSiguienteDibujante();

            var jugadorDos = _gestor.Obtener(IdJugadorDos);
            jugadorDos.YaAdivino = true;

            Assert.IsTrue(_gestor.TodosAdivinaron());
        }

        // fix el flujo debería validar solo el primero de la clasificacion para no tener muchas veces lo mismo
        [TestMethod]
        public void Prueba_GenerarClasificacion_OrdenaDescendente()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);

            var jugadorUno = _gestor.Obtener(IdJugadorUno);
            jugadorUno.PuntajeTotal = PuntajeBajo;

            var jugadorDos = _gestor.Obtener(IdJugadorDos);
            jugadorDos.PuntajeTotal = PuntajeAlto;

            var clasificacion = _gestor.GenerarClasificacion();

            Assert.AreEqual(NombreJugadorDos, clasificacion[IndicePrimero].Usuario);
            Assert.AreEqual(NombreJugadorUno, clasificacion[IndiceSegundo].Usuario);
        }

        [TestMethod]
        public void Prueba_ObtenerCopiaLista_RetornaCopia()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            var copia = _gestor.ObtenerCopiaLista();

            Assert.AreEqual(CantidadEsperadaUno, copia.Count);
            Assert.AreNotSame(_gestor.Obtener(IdJugadorUno), copia);
        }
    }
}
