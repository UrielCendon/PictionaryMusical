using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Pruebas.Servicios
{
    /// <summary>
    /// Contiene pruebas unitarias para la clase <see cref="GestorJugadoresPartida"/>.
    /// Valida la gestion de jugadores, cola de dibujantes, adivinanzas y clasificacion.
    /// </summary>
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
        public void Prueba_Constructor_GeneradorNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new GestorJugadoresPartida(null));
        }

        [TestMethod]
        public void Prueba_Agregar_JugadorNuevoSeRegistraCorrectamente()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            var jugador = _gestor.Obtener(IdJugadorUno);

            Assert.IsNotNull(jugador);
            Assert.AreEqual(NombreJugadorUno, jugador.NombreUsuario);
            Assert.IsTrue(jugador.EsHost);
        }

        [TestMethod]
        public void Prueba_Agregar_JugadorExistenteActualizaDatos()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            _gestor.Agregar(IdJugadorUno, NuevoNombre, false);
            var jugador = _gestor.Obtener(IdJugadorUno);

            Assert.AreEqual(NuevoNombre, jugador.NombreUsuario);
            Assert.IsFalse(jugador.EsHost);
        }

        [TestMethod]
        public void Prueba_HaySuficientesJugadores_MenosDeDosRetornaFalse()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            Assert.IsFalse(_gestor.HaySuficientesJugadores);
        }

        [TestMethod]
        public void Prueba_HaySuficientesJugadores_DosOMasRetornaTrue()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);

            Assert.IsTrue(_gestor.HaySuficientesJugadores);
        }

        [TestMethod]
        public void Prueba_Remover_JugadorExistenteRetornaTrueYDatosSalida()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            bool eraDibujante;
            string nombreSalida;

            bool resultado = _gestor.Remover(IdJugadorUno, out eraDibujante, out nombreSalida);

            Assert.IsTrue(resultado);
            Assert.AreEqual(NombreJugadorUno, nombreSalida);
            Assert.IsNull(_gestor.Obtener(IdJugadorUno));
        }

        [TestMethod]
        public void Prueba_Remover_JugadorInexistenteRetornaFalse()
        {
            bool eraDibujante;
            string nombreSalida;

            bool resultado = _gestor.Remover(IdInexistente, out eraDibujante, out nombreSalida);

            Assert.IsFalse(resultado);
            Assert.IsNull(nombreSalida);
        }

        [TestMethod]
        public void Prueba_EsHost_JugadorHostRetornaTrue()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            Assert.IsTrue(_gestor.EsHost(IdJugadorUno));
        }

        [TestMethod]
        public void Prueba_PrepararColaDibujantes_InvocaMezcladorYLlenaCola()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);

            _gestor.PrepararColaDibujantes();

            _generadorMock.Verify(generador => generador.MezclarLista(It.IsAny<IList<string>>()), Times.Once);
            Assert.IsTrue(_gestor.QuedanDibujantesPendientes());
        }

        [TestMethod]
        public void Prueba_SeleccionarSiguienteDibujante_AsignaRolDibujante()
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
        public void Prueba_TodosAdivinaron_FaltaAlguienRetornaFalse()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);
            _gestor.PrepararColaDibujantes();

            _gestor.SeleccionarSiguienteDibujante();

            Assert.IsFalse(_gestor.TodosAdivinaron());
        }

        [TestMethod]
        public void Prueba_TodosAdivinaron_TodosListosRetornaTrue()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);
            _gestor.PrepararColaDibujantes();

            _gestor.SeleccionarSiguienteDibujante();

            var jugadorDos = _gestor.Obtener(IdJugadorDos);
            jugadorDos.YaAdivino = true;

            Assert.IsTrue(_gestor.TodosAdivinaron());
        }

        [TestMethod]
        public void Prueba_GenerarClasificacion_OrdenaDescendentePorPuntos()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);
            _gestor.Agregar(IdJugadorDos, NombreJugadorDos, false);

            var jugadorUno = _gestor.Obtener(IdJugadorUno);
            jugadorUno.PuntajeTotal = PuntajeBajo;

            var jugadorDos = _gestor.Obtener(IdJugadorDos);
            jugadorDos.PuntajeTotal = PuntajeAlto;

            var clasificacion = _gestor.GenerarClasificacion();

            Assert.AreEqual(NombreJugadorDos, clasificacion[0].Usuario);
            Assert.AreEqual(NombreJugadorUno, clasificacion[1].Usuario);
        }

        [TestMethod]
        public void Prueba_ObtenerCopiaLista_RetornaNuevaColeccion()
        {
            _gestor.Agregar(IdJugadorUno, NombreJugadorUno, true);

            var copia = _gestor.ObtenerCopiaLista();

            Assert.AreEqual(1, copia.Count);
            Assert.AreNotSame(_gestor.Obtener(IdJugadorUno), copia);
        }
    }
}