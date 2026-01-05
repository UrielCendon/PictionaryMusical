using System;
using System.Collections.Generic;
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
    public class SalasManejadorPruebas
    {
        private const string CodigoSalaValido = "123456";
        private const string CodigoSalaInvalido = "12345";
        private const string NombreCreadorValido = "CreadorPrueba";
        private const string NombreJugadorValido = "JugadorPrueba";
        private const string NombreUsuarioVacio = "";
        private const string IdiomaEspanol = "es";
        private const string DificultadFacil = "facil";
        private const int NumeroRondasValido = 3;
        private const int TiempoRondaValido = 60;
        private const int CantidadSalasVacia = 0;
        private const int CantidadUnaSala = 1;
        private const int IndicePrimeraSala = 0;

        private Mock<INotificadorSalas> _notificadorMock;
        private Mock<IAlmacenSalas> _almacenSalasMock;
        private Mock<IProveedorContextoOperacion> _proveedorContextoMock;
        private Mock<ISalaInternaFactoria> _salaFactoriaMock;
        private Mock<IGeneradorCodigoSala> _generadorCodigoMock;
        private Mock<ISalasManejadorCallback> _callbackMock;
        private Mock<IContextChannel> _canalMock;
        private SalasManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _notificadorMock = new Mock<INotificadorSalas>();
            _almacenSalasMock = new Mock<IAlmacenSalas>();
            _proveedorContextoMock = new Mock<IProveedorContextoOperacion>();
            _salaFactoriaMock = new Mock<ISalaInternaFactoria>();
            _generadorCodigoMock = new Mock<IGeneradorCodigoSala>();
            _callbackMock = new Mock<ISalasManejadorCallback>();
            _canalMock = new Mock<IContextChannel>();

            _proveedorContextoMock
                .Setup(proveedor => proveedor.ObtenerCallback())
                .Returns(_callbackMock.Object);

            _proveedorContextoMock
                .Setup(proveedor => proveedor.ObtenerCanal())
                .Returns(_canalMock.Object);

            _generadorCodigoMock
                .Setup(generador => generador.GenerarCodigo())
                .Returns(CodigoSalaValido);

            _manejador = new SalasManejador(
                _notificadorMock.Object,
                _almacenSalasMock.Object,
                _proveedorContextoMock.Object,
                _salaFactoriaMock.Object,
                _generadorCodigoMock.Object);
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

        private SalaInternaManejador CrearSalaInternaMock()
        {
            var gestorMock = new Mock<IGestorNotificacionesSalaInterna>();
            return new SalaInternaManejador(
                CodigoSalaValido,
                NombreCreadorValido,
                CrearConfiguracionValida(),
                gestorMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_NotificadorNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new SalasManejador(
                    null,
                    _almacenSalasMock.Object,
                    _proveedorContextoMock.Object,
                    _salaFactoriaMock.Object,
                    _generadorCodigoMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_AlmacenSalasNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new SalasManejador(
                    _notificadorMock.Object,
                    null,
                    _proveedorContextoMock.Object,
                    _salaFactoriaMock.Object,
                    _generadorCodigoMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_ProveedorContextoNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new SalasManejador(
                    _notificadorMock.Object,
                    _almacenSalasMock.Object,
                    null,
                    _salaFactoriaMock.Object,
                    _generadorCodigoMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_SalaFactoriaNula_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new SalasManejador(
                    _notificadorMock.Object,
                    _almacenSalasMock.Object,
                    _proveedorContextoMock.Object,
                    null,
                    _generadorCodigoMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_GeneradorCodigoNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new SalasManejador(
                    _notificadorMock.Object,
                    _almacenSalasMock.Object,
                    _proveedorContextoMock.Object,
                    _salaFactoriaMock.Object,
                    null));
        }

        [TestMethod]
        public void Prueba_ObtenerSalas_SinSalas_RetornaListaVacia()
        {
            _almacenSalasMock
                .Setup(almacen => almacen.Values)
                .Returns(new List<SalaInternaManejador>());

            IList<SalaDTO> resultado = _manejador.ObtenerSalas();

            Assert.AreEqual(CantidadSalasVacia, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerSalas_ConSalas_RetornaListaConvertida()
        {
            var salaInterna = CrearSalaInternaMock();
            _almacenSalasMock
                .Setup(almacen => almacen.Values)
                .Returns(new List<SalaInternaManejador> { salaInterna });

            IList<SalaDTO> resultado = _manejador.ObtenerSalas();

            Assert.AreEqual(CantidadUnaSala, resultado.Count);
            Assert.AreEqual(CodigoSalaValido, resultado[IndicePrimeraSala].Codigo);
        }

        [TestMethod]
        public void Prueba_CrearSala_NombreCreadorVacio_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionValida();

            Assert.ThrowsException<FaultException>(() =>
                _manejador.CrearSala(NombreUsuarioVacio, configuracion));
        }

        [TestMethod]
        public void Prueba_CrearSala_ConfiguracionNula_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _manejador.CrearSala(NombreCreadorValido, null));
        }

        [TestMethod]
        public void Prueba_CrearSala_DatosValidos_RetornaSalaCreada()
        {
            var configuracion = CrearConfiguracionValida();
            var salaInterna = CrearSalaInternaMock();

            _salaFactoriaMock
                .Setup(factoria => factoria.Crear(
                    CodigoSalaValido,
                    NombreCreadorValido,
                    configuracion))
                .Returns(salaInterna);

            _almacenSalasMock
                .Setup(almacen => almacen.TryAdd(CodigoSalaValido, salaInterna))
                .Returns(true);

            SalaDTO resultado = _manejador.CrearSala(NombreCreadorValido, configuracion);

            Assert.AreEqual(CodigoSalaValido, resultado.Codigo);
            Assert.AreEqual(NombreCreadorValido, resultado.Creador);
        }

        [TestMethod]
        public void Prueba_CrearSala_DatosValidos_NotificaATodos()
        {
            var configuracion = CrearConfiguracionValida();
            var salaInterna = CrearSalaInternaMock();

            _salaFactoriaMock
                .Setup(factoria => factoria.Crear(
                    CodigoSalaValido,
                    NombreCreadorValido,
                    configuracion))
                .Returns(salaInterna);

            _almacenSalasMock
                .Setup(almacen => almacen.TryAdd(CodigoSalaValido, salaInterna))
                .Returns(true);

            _manejador.CrearSala(NombreCreadorValido, configuracion);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearSala_ErrorConcurrencia_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionValida();
            var salaInterna = CrearSalaInternaMock();

            _salaFactoriaMock
                .Setup(factoria => factoria.Crear(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ConfiguracionPartidaDTO>()))
                .Returns(salaInterna);

            _almacenSalasMock
                .Setup(almacen => almacen.TryAdd(It.IsAny<string>(), It.IsAny<SalaInternaManejador>()))
                .Returns(false);

            Assert.ThrowsException<FaultException>(() =>
                _manejador.CrearSala(NombreCreadorValido, configuracion));
        }

        [TestMethod]
        public void Prueba_UnirseSala_CodigoInvalido_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _manejador.UnirseSala(CodigoSalaInvalido, NombreJugadorValido));
        }

        [TestMethod]
        public void Prueba_UnirseSala_NombreUsuarioVacio_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _manejador.UnirseSala(CodigoSalaValido, NombreUsuarioVacio));
        }

        [TestMethod]
        public void Prueba_UnirseSala_SalaNoExiste_LanzaFaultException()
        {
            SalaInternaManejador salaOut;
            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaOut))
                .Returns(false);

            Assert.ThrowsException<FaultException>(() =>
                _manejador.UnirseSala(CodigoSalaValido, NombreJugadorValido));
        }

        [TestMethod]
        public void Prueba_UnirseSala_PartidaIniciada_LanzaFaultException()
        {
            var salaInterna = CrearSalaInternaMock();
            salaInterna.PartidaIniciada = true;

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            Assert.ThrowsException<FaultException>(() =>
                _manejador.UnirseSala(CodigoSalaValido, NombreJugadorValido));
        }

        [TestMethod]
        public void Prueba_UnirseSala_DatosValidos_NotificaATodos()
        {
            var salaInterna = CrearSalaInternaMock();

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.UnirseSala(CodigoSalaValido, NombreJugadorValido);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_AbandonarSala_CodigoInvalido_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _manejador.AbandonarSala(CodigoSalaInvalido, NombreJugadorValido));
        }

        [TestMethod]
        public void Prueba_AbandonarSala_SalaNoExiste_LanzaFaultException()
        {
            SalaInternaManejador salaOut;
            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaOut))
                .Returns(false);

            Assert.ThrowsException<FaultException>(() =>
                _manejador.AbandonarSala(CodigoSalaValido, NombreJugadorValido));
        }

        [TestMethod]
        public void Prueba_AbandonarSala_SalaDebeEliminarse_RemueveSala()
        {
            var gestorMock = new Mock<IGestorNotificacionesSalaInterna>();
            var salaInterna = new SalaInternaManejador(
                CodigoSalaValido,
                NombreCreadorValido,
                CrearConfiguracionValida(),
                gestorMock.Object);
            salaInterna.AgregarJugador(NombreCreadorValido, _callbackMock.Object, false);

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.AbandonarSala(CodigoSalaValido, NombreCreadorValido);

            SalaInternaManejador salaRemovida;
            _almacenSalasMock.Verify(
                almacen => almacen.TryRemove(CodigoSalaValido, out salaRemovida),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_AbandonarSala_DatosValidos_NotificaATodos()
        {
            var salaInterna = CrearSalaInternaMock();

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.AbandonarSala(CodigoSalaValido, NombreJugadorValido);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_CodigoInvalido_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                _manejador.ExpulsarJugador(
                    CodigoSalaInvalido, 
                    NombreCreadorValido, 
                    NombreJugadorValido));
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_SalaNoExiste_LanzaFaultException()
        {
            SalaInternaManejador salaOut;
            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaOut))
                .Returns(false);

            Assert.ThrowsException<FaultException>(() =>
                _manejador.ExpulsarJugador(
                    CodigoSalaValido, 
                    NombreCreadorValido, 
                    NombreJugadorValido));
        }

        [TestMethod]
        public void Prueba_ExpulsarJugador_DatosValidos_NotificaATodos()
        {
            var gestorMock = new Mock<IGestorNotificacionesSalaInterna>();
            gestorMock
                .Setup(gestor => gestor.ObtenerCallback(NombreJugadorValido))
                .Returns(_callbackMock.Object);

            var salaInterna = new SalaInternaManejador(
                CodigoSalaValido,
                NombreCreadorValido,
                CrearConfiguracionValida(),
                gestorMock.Object);
            salaInterna.AgregarJugador(NombreCreadorValido, _callbackMock.Object, false);
            salaInterna.AgregarJugador(NombreJugadorValido, _callbackMock.Object, false);

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.ExpulsarJugador(CodigoSalaValido, NombreCreadorValido, NombreJugadorValido);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_BanearJugador_CodigoInvalido_NoLanzaExcepcion()
        {
            _manejador.BanearJugador(CodigoSalaInvalido, NombreJugadorValido);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_BanearJugador_SalaNoExiste_NoLanzaExcepcion()
        {
            SalaInternaManejador salaOut;
            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaOut))
                .Returns(false);

            _manejador.BanearJugador(CodigoSalaValido, NombreJugadorValido);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_BanearJugador_DatosValidos_NotificaATodos()
        {
            var gestorMock = new Mock<IGestorNotificacionesSalaInterna>();
            gestorMock
                .Setup(gestor => gestor.ObtenerCallback(NombreJugadorValido))
                .Returns(_callbackMock.Object);

            var salaInterna = new SalaInternaManejador(
                CodigoSalaValido,
                NombreCreadorValido,
                CrearConfiguracionValida(),
                gestorMock.Object);
            salaInterna.AgregarJugador(NombreCreadorValido, _callbackMock.Object, false);
            salaInterna.AgregarJugador(NombreJugadorValido, _callbackMock.Object, false);

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.BanearJugador(CodigoSalaValido, NombreJugadorValido);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ObtenerSalaPorCodigo_CodigoInvalido_LanzaInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _manejador.ObtenerSalaPorCodigo(CodigoSalaInvalido));
        }

        [TestMethod]
        public void Prueba_ObtenerSalaPorCodigo_SalaNoExiste_LanzaInvalidOperationException()
        {
            SalaInternaManejador salaOut;
            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaOut))
                .Returns(false);

            Assert.ThrowsException<InvalidOperationException>(() =>
                _manejador.ObtenerSalaPorCodigo(CodigoSalaValido));
        }

        [TestMethod]
        public void Prueba_ObtenerSalaPorCodigo_SalaExiste_RetornaSalaDto()
        {
            var salaInterna = CrearSalaInternaMock();

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            SalaDTO resultado = _manejador.ObtenerSalaPorCodigo(CodigoSalaValido);

            Assert.AreEqual(CodigoSalaValido, resultado.Codigo);
        }

        [TestMethod]
        public void Prueba_MarcarPartidaComoIniciada_SalaExiste_MarcaPartidaIniciada()
        {
            var salaInterna = CrearSalaInternaMock();

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.MarcarPartidaComoIniciada(CodigoSalaValido);

            Assert.IsTrue(salaInterna.PartidaIniciada);
        }

        [TestMethod]
        public void Prueba_MarcarPartidaComoIniciada_SalaNoExiste_NoLanzaExcepcion()
        {
            SalaInternaManejador salaOut;
            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaOut))
                .Returns(false);

            _manejador.MarcarPartidaComoIniciada(CodigoSalaValido);

            _almacenSalasMock.Verify(
                almacen => almacen.TryGetValue(CodigoSalaValido, out salaOut),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_MarcarPartidaComoFinalizada_SalaExiste_MarcaPartidaFinalizada()
        {
            var salaInterna = CrearSalaInternaMock();

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.MarcarPartidaComoFinalizada(CodigoSalaValido);

            Assert.IsTrue(salaInterna.PartidaFinalizada);
        }

        [TestMethod]
        public void Prueba_MarcarPartidaComoFinalizada_RemueveSalaYNotifica()
        {
            var salaInterna = CrearSalaInternaMock();

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.MarcarPartidaComoFinalizada(CodigoSalaValido);

            SalaInternaManejador salaRemovida;
            _almacenSalasMock.Verify(
                almacen => almacen.TryRemove(CodigoSalaValido, out salaRemovida),
                Times.Once);
            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_SuscribirListaSalas_ObtieneSesionIdYNotifica()
        {
            var sesionId = Guid.NewGuid();
            _notificadorMock
                .Setup(notificador => notificador.Suscribir(_callbackMock.Object))
                .Returns(sesionId);

            _manejador.SuscribirListaSalas();

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalas(_callbackMock.Object),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CancelarSuscripcionListaSalas_DesuscribePorCallback()
        {
            _manejador.CancelarSuscripcionListaSalas();

            _notificadorMock.Verify(
                notificador => notificador.DesuscribirPorCallback(_callbackMock.Object),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarDesconexionJugador_SalaExiste_NotificaATodos()
        {
            var salaInterna = CrearSalaInternaMock();

            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaInterna))
                .Returns(true);

            _manejador.NotificarDesconexionJugador(CodigoSalaValido, NombreJugadorValido);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarDesconexionJugador_SalaNoExiste_NoLanzaExcepcion()
        {
            SalaInternaManejador salaOut;
            _almacenSalasMock
                .Setup(almacen => almacen.TryGetValue(CodigoSalaValido, out salaOut))
                .Returns(false);

            _manejador.NotificarDesconexionJugador(CodigoSalaValido, NombreJugadorValido);

            _notificadorMock.Verify(
                notificador => notificador.NotificarListaSalasATodos(),
                Times.Never);
        }
    }
}
