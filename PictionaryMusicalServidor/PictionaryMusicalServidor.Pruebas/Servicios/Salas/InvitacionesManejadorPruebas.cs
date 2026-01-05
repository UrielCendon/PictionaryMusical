using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Datos.Excepciones;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Salas
{
    [TestClass]
    public class InvitacionesManejadorPruebas
    {
        private const string CodigoSalaValido = "123456";
        private const string CodigoSalaConEspacios = "  123456  ";
        private const string CorreoValido = "usuario@ejemplo.com";
        private const string CorreoJugadorEnSala = "jugadorensala@ejemplo.com";
        private const string IdiomaEspanol = "es";
        private const string IdiomaIngles = "en";
        private const string NombreCreadorSala = "CreadorSala";
        private const string NombreJugadorEnSala = "JugadorEnSala";
        private const string NombreJugadorInvitado = "JugadorInvitado";
        private const int NumeroRondasValido = 3;
        private const int TiempoRondaValido = 60;

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<ISalasManejador> _salasManejadorMock;
        private Mock<ICorreoInvitacionNotificador> _correoNotificadorMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private InvitacionesManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _salasManejadorMock = new Mock<ISalasManejador>();
            _correoNotificadorMock = new Mock<ICorreoInvitacionNotificador>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();

            _contextoFactoriaMock
                .Setup(factoria => factoria.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(factoria => factoria.CrearUsuarioRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _manejador = new InvitacionesManejador(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object,
                _salasManejadorMock.Object,
                _correoNotificadorMock.Object);
        }

        private InvitacionSalaDTO CrearInvitacionValida()
        {
            return new InvitacionSalaDTO
            {
                CodigoSala = CodigoSalaValido,
                Correo = CorreoValido,
                Idioma = IdiomaEspanol
            };
        }

        private SalaDTO CrearSalaDtoValida()
        {
            return new SalaDTO
            {
                Codigo = CodigoSalaValido,
                Creador = NombreCreadorSala,
                Configuracion = new ConfiguracionPartidaDTO
                {
                    NumeroRondas = NumeroRondasValido,
                    TiempoPorRondaSegundos = TiempoRondaValido,
                    IdiomaCanciones = IdiomaEspanol,
                    Dificultad = "facil"
                },
                Jugadores = new List<string> { NombreCreadorSala }
            };
        }

        private Usuario CrearUsuarioValido(string nombreUsuario)
        {
            return new Usuario
            {
                Nombre_Usuario = nombreUsuario
            };
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvitacionesManejador(
                    null,
                    _repositorioFactoriaMock.Object,
                    _salasManejadorMock.Object,
                    _correoNotificadorMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvitacionesManejador(
                    _contextoFactoriaMock.Object,
                    null,
                    _salasManejadorMock.Object,
                    _correoNotificadorMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_SalasManejadorNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvitacionesManejador(
                    _contextoFactoriaMock.Object,
                    _repositorioFactoriaMock.Object,
                    null,
                    _correoNotificadorMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_CorreoNotificadorNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InvitacionesManejador(
                    _contextoFactoriaMock.Object,
                    _repositorioFactoriaMock.Object,
                    _salasManejadorMock.Object,
                    null));
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_InvitacionNula_RetornaFallo()
        {
            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_CodigoSalaVacio_RetornaFallo()
        {
            var invitacion = CrearInvitacionValida();
            invitacion.CodigoSala = string.Empty;

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_CorreoInvalido_RetornaFallo()
        {
            var invitacion = CrearInvitacionValida();
            invitacion.Correo = "correo_invalido";

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_SalaNoExiste_RetornaFallo()
        {
            var invitacion = CrearInvitacionValida();
            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns((SalaDTO)null);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_UsuarioYaEnSala_RetornaFallo()
        {
            var invitacion = CrearInvitacionValida();
            invitacion.Correo = CorreoJugadorEnSala;

            var salaConJugador = CrearSalaDtoValida();
            salaConJugador.Jugadores = new List<string> { NombreCreadorSala, NombreJugadorEnSala };

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(salaConJugador);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoJugadorEnSala))
                .ReturnsAsync(CrearUsuarioValido(NombreJugadorEnSala));

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_EnvioExitoso_RetornaExito()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ReturnsAsync(CrearUsuarioValido(NombreJugadorInvitado));

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(true);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_EnvioFalla_RetornaFallo()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ReturnsAsync(CrearUsuarioValido(NombreJugadorInvitado));

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(false);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_UsuarioNoEnBaseDatos_EnviaCorreo()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ThrowsAsync(new KeyNotFoundException());

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(true);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_ErrorBaseDatos_RetornaFallo()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ThrowsAsync(new BaseDatosExcepcion("Error de conexion"));

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(true);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_CorreoEnviado_ConParametrosCorrectos()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();
            InvitacionCorreoParametros parametrosCapturados = null;

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ReturnsAsync((Usuario)null);

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .Callback<InvitacionCorreoParametros>(parametros => 
                    parametrosCapturados = parametros)
                .ReturnsAsync(true);

            await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.AreEqual(CorreoValido, parametrosCapturados.CorreoDestino);
            Assert.AreEqual(CodigoSalaValido, parametrosCapturados.CodigoSala);
            Assert.AreEqual(NombreCreadorSala, parametrosCapturados.Creador);
            Assert.AreEqual(IdiomaEspanol, parametrosCapturados.Idioma);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_SalaJugadoresNulos_EnviaCorreo()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();
            sala.Jugadores = null;

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(true);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_SalaJugadoresVacia_EnviaCorreo()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();
            sala.Jugadores = new List<string>();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(true);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_IdiomaIngles_EnviaConIdiomaIngles()
        {
            var invitacion = CrearInvitacionValida();
            invitacion.Idioma = IdiomaIngles;
            var sala = CrearSalaDtoValida();
            InvitacionCorreoParametros parametrosCapturados = null;

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ReturnsAsync((Usuario)null);

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .Callback<InvitacionCorreoParametros>(parametros => 
                    parametrosCapturados = parametros)
                .ReturnsAsync(true);

            await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.AreEqual(IdiomaIngles, parametrosCapturados.Idioma);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_UsuarioNombreVacio_EnviaCorreo()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ReturnsAsync(new Usuario { Nombre_Usuario = string.Empty });

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(true);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_UsuarioNombreNulo_EnviaCorreo()
        {
            var invitacion = CrearInvitacionValida();
            var sala = CrearSalaDtoValida();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ReturnsAsync(new Usuario { Nombre_Usuario = null });

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(true);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_CorreoConEspacios_NormalizaCorreo()
        {
            var invitacion = CrearInvitacionValida();
            invitacion.Correo = "  usuario@ejemplo.com  ";
            var sala = CrearSalaDtoValida();
            InvitacionCorreoParametros parametrosCapturados = null;

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ReturnsAsync((Usuario)null);

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .Callback<InvitacionCorreoParametros>(parametros => 
                    parametrosCapturados = parametros)
                .ReturnsAsync(true);

            await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.AreEqual(CorreoValido, parametrosCapturados.CorreoDestino);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_CodigoSalaConEspacios_NormalizaCodigo()
        {
            var invitacion = CrearInvitacionValida();
            invitacion.CodigoSala = CodigoSalaConEspacios;
            var sala = CrearSalaDtoValida();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(CodigoSalaValido))
                .Returns(sala);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoValido))
                .ReturnsAsync((Usuario)null);

            _correoNotificadorMock
                .Setup(notificador => notificador.EnviarInvitacionAsync(
                    It.IsAny<InvitacionCorreoParametros>()))
                .ReturnsAsync(true);

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            _salasManejadorMock.Verify(
                manejador => manejador.ObtenerSalaPorCodigo(CodigoSalaValido),
                Times.Once);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_ExcepcionGeneral_RetornaFallo()
        {
            var invitacion = CrearInvitacionValida();

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Throws(new Exception("Error inesperado"));

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public async Task Prueba_EnviarInvitacion_ComparacionNombreIgnoraMayusculas()
        {
            var invitacion = CrearInvitacionValida();
            invitacion.Correo = CorreoJugadorEnSala;

            var salaConJugador = CrearSalaDtoValida();
            salaConJugador.Jugadores = new List<string> 
            { 
                NombreCreadorSala, 
                NombreJugadorEnSala.ToUpper() 
            };

            _salasManejadorMock
                .Setup(manejador => manejador.ObtenerSalaPorCodigo(It.IsAny<string>()))
                .Returns(salaConJugador);

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreoAsync(CorreoJugadorEnSala))
                .ReturnsAsync(CrearUsuarioValido(NombreJugadorEnSala.ToLower()));

            ResultadoOperacionDTO resultado = await _manejador.EnviarInvitacionAsync(invitacion);

            Assert.IsFalse(resultado.OperacionExitosa);
        }
    }
}
