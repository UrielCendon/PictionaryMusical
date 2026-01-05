using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Salas
{
    [TestClass]
    public class SalaInternaManejadorPruebas
    {
        private const string CodigoSalaPrueba = "123456";
        private const string NombreCreadorPrueba = "Creador";
        private const string NombreJugadorUno = "Jugador1";
        private const string NombreJugadorDos = "Jugador2";
        private const string NombreJugadorTres = "Jugador3";
        private const string NombreJugadorCuatro = "Jugador4";
        private const string IdiomaEspanol = "es";
        private const string DificultadFacil = "facil";
        private const int NumeroRondasValido = 3;
        private const int TiempoRondaValido = 60;
        private const int CantidadMaximaJugadores = 4;
        private const int CantidadJugadoresVacia = 0;
        private const int CantidadUnJugador = 1;

        private Mock<IGestorNotificacionesSalaInterna> _gestorNotificacionesMock;
        private Mock<ISalasManejadorCallback> _callbackMock;
        private ConfiguracionPartidaDTO _configuracionValida;
        private SalaInternaManejador _sala;

        [TestInitialize]
        public void Inicializar()
        {
            _gestorNotificacionesMock = new Mock<IGestorNotificacionesSalaInterna>();
            _callbackMock = new Mock<ISalasManejadorCallback>();
            _configuracionValida = CrearConfiguracionValida();
            _sala = new SalaInternaManejador(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                _configuracionValida,
                _gestorNotificacionesMock.Object);
        }

        private ConfiguracionPartidaDTO CrearConfiguracionValida()
        {
            return new ConfiguracionPartidaDTO
            {
                NumeroRondas = NumeroRondasValido,
                TiempoPorRondaSegundos = TiempoRondaValido,
                IdiomaCanciones = IdiomaEspanol,
                Dificultad = DificultadFacil
            };
        }

        [TestMethod]
        public void Prueba_Constructor_GestorNotificacionesNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<System.ArgumentNullException>(() =>
                new SalaInternaManejador(
                    CodigoSalaPrueba,
                    NombreCreadorPrueba,
                    _configuracionValida,
                    null));
        }

        [TestMethod]
        public void Prueba_Constructor_AsignaCodigoCorrectamente()
        {
            Assert.AreEqual(CodigoSalaPrueba, _sala.Codigo);
        }

        [TestMethod]
        public void Prueba_Constructor_AsignaCreadorCorrectamente()
        {
            Assert.AreEqual(NombreCreadorPrueba, _sala.Creador);
        }

        [TestMethod]
        public void Prueba_Constructor_AsignaConfiguracionCorrectamente()
        {
            Assert.AreEqual(_configuracionValida, _sala.Configuracion);
        }

        [TestMethod]
        public void Prueba_Constructor_InicializaPartidaNoIniciada()
        {
            Assert.IsFalse(_sala.PartidaIniciada);
        }

        [TestMethod]
        public void Prueba_Constructor_InicializaPartidaNoFinalizada()
        {
            Assert.IsFalse(_sala.PartidaFinalizada);
        }

        [TestMethod]
        public void Prueba_Constructor_InicializaDebeEliminarseEnFalso()
        {
            Assert.IsFalse(_sala.DebeEliminarse);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaCodigoCorrecto()
        {
            SalaDTO resultado = _sala.ConvertirADto();

            Assert.AreEqual(CodigoSalaPrueba, resultado.Codigo);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaCreadorCorrecto()
        {
            SalaDTO resultado = _sala.ConvertirADto();

            Assert.AreEqual(NombreCreadorPrueba, resultado.Creador);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaConfiguracionCorrecta()
        {
            SalaDTO resultado = _sala.ConvertirADto();

            Assert.AreEqual(_configuracionValida, resultado.Configuracion);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaListaJugadoresVacia()
        {
            SalaDTO resultado = _sala.ConvertirADto();

            Assert.AreEqual(CantidadJugadoresVacia, resultado.Jugadores.Count);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_JugadorSeAgregaCorrectamente()
        {
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            SalaDTO resultado = _sala.ConvertirADto();

            Assert.IsTrue(resultado.Jugadores.Contains(NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_AgregarJugador_RegistraCallbackEnGestor()
        {
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.Registrar(NombreJugadorUno, _callbackMock.Object),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_ConNotificacionEnviaNotificacion()
        {
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, true);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.NotificarIngreso(
                    CodigoSalaPrueba,
                    NombreJugadorUno,
                    It.IsAny<SalaDTO>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_SinNotificacionNoEnviaNotificacion()
        {
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.NotificarIngreso(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SalaDTO>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_JugadorExistenteActualizaCallback()
        {
            var nuevoCallbackMock = new Mock<ISalasManejadorCallback>();
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.AgregarJugador(NombreJugadorUno, nuevoCallbackMock.Object, false);

            SalaDTO resultado = _sala.ConvertirADto();
            int cantidadOcurrencias = resultado.Jugadores
                .Count(jugador => jugador == NombreJugadorUno);

            Assert.AreEqual(CantidadUnJugador, cantidadOcurrencias);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_SalaLlena_LanzaFaultException()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorDos, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorTres, _callbackMock.Object, false);

            Assert.ThrowsException<FaultException>(() =>
                _sala.AgregarJugador(NombreJugadorCuatro, _callbackMock.Object, false));
        }

        [TestMethod]
        public void Prueba_AgregarJugador_RetornaDtoConJugadorAgregado()
        {
            SalaDTO resultado = _sala.AgregarJugador(
                NombreJugadorUno, 
                _callbackMock.Object, 
                false);

            Assert.IsTrue(resultado.Jugadores.Contains(NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_RemoverJugador_JugadorSeRemueveCorrectamente()
        {
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.RemoverJugador(NombreJugadorUno);

            SalaDTO resultado = _sala.ConvertirADto();
            Assert.IsFalse(resultado.Jugadores.Contains(NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_RemoverJugador_LlamaRemoverEnGestor()
        {
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.RemoverJugador(NombreJugadorUno);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.Remover(NombreJugadorUno),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_JugadorInexistenteNoLanzaExcepcion()
        {
            _sala.RemoverJugador(NombreJugadorUno);

            SalaDTO resultado = _sala.ConvertirADto();
            Assert.AreEqual(CantidadJugadoresVacia, resultado.Jugadores.Count);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_NotificaSalidaAOtrosJugadores()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.RemoverJugador(NombreJugadorUno);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.NotificarSalida(
                    CodigoSalaPrueba,
                    NombreJugadorUno,
                    It.IsAny<SalaDTO>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_AnfitrionSaleCancelaSala()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.RemoverJugador(NombreCreadorPrueba);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.NotificarCancelacion(CodigoSalaPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_AnfitrionSaleMarcaDebeEliminarse()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);

            _sala.RemoverJugador(NombreCreadorPrueba);

            Assert.IsTrue(_sala.DebeEliminarse);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_SalaVaciaMarcaDebeEliminarse()
        {
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.RemoverJugador(NombreJugadorUno);

            Assert.IsTrue(_sala.DebeEliminarse);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_PartidaFinalizadaAnfitrionSaleNoNotifica()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.PartidaFinalizada = true;

            _sala.RemoverJugador(NombreCreadorPrueba);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.NotificarSalida(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SalaDTO>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_JugadorSeRemueve()
        {
            _gestorNotificacionesMock
                .Setup(gestor => gestor.ObtenerCallback(NombreJugadorUno))
                .Returns(_callbackMock.Object);
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.ExpulsarJugador(NombreCreadorPrueba, NombreJugadorUno);

            SalaDTO resultado = _sala.ConvertirADto();
            Assert.IsFalse(resultado.Jugadores.Contains(NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_NotificaExpulsion()
        {
            _gestorNotificacionesMock
                .Setup(gestor => gestor.ObtenerCallback(NombreJugadorUno))
                .Returns(_callbackMock.Object);
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.ExpulsarJugador(NombreCreadorPrueba, NombreJugadorUno);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.NotificarExpulsion(
                    It.Is<ExpulsionNotificacionParametros>(
                        parametros => parametros.NombreExpulsado == NombreJugadorUno)),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_NoCreadorIntentaExpulsar_LanzaFaultException()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorDos, _callbackMock.Object, false);

            Assert.ThrowsException<FaultException>(() =>
                _sala.ExpulsarJugador(NombreJugadorUno, NombreJugadorDos));
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_CreadorIntentaAutoexpulsarse_LanzaFaultException()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);

            Assert.ThrowsException<FaultException>(() =>
                _sala.ExpulsarJugador(NombreCreadorPrueba, NombreCreadorPrueba));
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_JugadorNoExiste_LanzaFaultException()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);

            Assert.ThrowsException<FaultException>(() =>
                _sala.ExpulsarJugador(NombreCreadorPrueba, NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_BanearJugador_JugadorSeRemueve()
        {
            _gestorNotificacionesMock
                .Setup(gestor => gestor.ObtenerCallback(NombreJugadorUno))
                .Returns(_callbackMock.Object);
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.BanearJugador(NombreJugadorUno);

            SalaDTO resultado = _sala.ConvertirADto();
            Assert.IsFalse(resultado.Jugadores.Contains(NombreJugadorUno));
        }

        [TestMethod]
        public void Prueba_BanearJugador_NotificaBaneo()
        {
            _gestorNotificacionesMock
                .Setup(gestor => gestor.ObtenerCallback(NombreJugadorUno))
                .Returns(_callbackMock.Object);
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.BanearJugador(NombreJugadorUno);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.NotificarBaneo(
                    It.Is<BaneoNotificacionParametros>(
                        parametros => parametros.NombreBaneado == NombreJugadorUno)),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_BanearJugador_JugadorNoExisteNoLanzaExcepcion()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);

            _sala.BanearJugador(NombreJugadorUno);

            SalaDTO resultado = _sala.ConvertirADto();
            Assert.AreEqual(CantidadUnJugador, resultado.Jugadores.Count);
        }

        [TestMethod]
        public void Prueba_PartidaIniciada_PuedeEstablecerseEnTrue()
        {
            _sala.PartidaIniciada = true;

            Assert.IsTrue(_sala.PartidaIniciada);
        }

        [TestMethod]
        public void Prueba_PartidaFinalizada_PuedeEstablecerseEnTrue()
        {
            _sala.PartidaFinalizada = true;

            Assert.IsTrue(_sala.PartidaFinalizada);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_MultiplesJugadoresHastaMaximo()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorDos, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorTres, _callbackMock.Object, false);

            SalaDTO resultado = _sala.ConvertirADto();

            Assert.AreEqual(CantidadMaximaJugadores, resultado.Jugadores.Count);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaCopiaDeLista()
        {
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            SalaDTO resultado = _sala.ConvertirADto();
            resultado.Jugadores.Add(NombreJugadorDos);

            SalaDTO resultadoActual = _sala.ConvertirADto();
            Assert.IsFalse(resultadoActual.Jugadores.Contains(NombreJugadorDos));
        }

        [TestMethod]
        public void Prueba_RemoverJugador_LimpiaGestorCuandoAnfitrionSale()
        {
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.RemoverJugador(NombreCreadorPrueba);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.Limpiar(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_RemoveCallbackDelGestor()
        {
            _gestorNotificacionesMock
                .Setup(gestor => gestor.ObtenerCallback(NombreJugadorUno))
                .Returns(_callbackMock.Object);
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.ExpulsarJugador(NombreCreadorPrueba, NombreJugadorUno);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.Remover(NombreJugadorUno),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_BanearJugador_RemoveCallbackDelGestor()
        {
            _gestorNotificacionesMock
                .Setup(gestor => gestor.ObtenerCallback(NombreJugadorUno))
                .Returns(_callbackMock.Object);
            _sala.AgregarJugador(NombreCreadorPrueba, _callbackMock.Object, false);
            _sala.AgregarJugador(NombreJugadorUno, _callbackMock.Object, false);

            _sala.BanearJugador(NombreJugadorUno);

            _gestorNotificacionesMock.Verify(
                gestor => gestor.Remover(NombreJugadorUno),
                Times.Once);
        }
    }
}
