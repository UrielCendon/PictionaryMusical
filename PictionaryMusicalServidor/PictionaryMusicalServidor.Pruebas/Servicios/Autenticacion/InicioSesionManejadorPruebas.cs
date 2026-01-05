using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Datos.Excepciones;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using BCryptNet = BCrypt.Net.BCrypt;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class InicioSesionManejadorPruebas
    {
        private const string NombreUsuarioValido = "UsuarioTest";
        private const string CorreoValido = "test@correo.com";
        private const string ContrasenaValida = "Password1!";
        private const string ContrasenaInvalida = "ContrasenaMala";
        private const string IdentificadorVacio = "";
        private const string IdentificadorEspacios = "   ";
        private const int IdUsuarioValido = 1;
        private const int IdJugadorValido = 10;
        private const int IdAvatarValido = 5;
        private const int CeroReportes = 0;
        private const int ReportesLimite = 3;
        private const string NombreJugador = "NombreTest";
        private const string ApellidoJugador = "ApellidoTest";

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private Mock<IReporteRepositorio> _reporteRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private InicioSesionManejador _manejador;
        private string _hashContrasenaValida;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _reporteRepositorioMock = new Mock<IReporteRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _hashContrasenaValida = BCryptNet.HashPassword(ContrasenaValida);

            _contextoFactoriaMock
                .Setup(fabrica => fabrica.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearUsuarioRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearReporteRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_reporteRepositorioMock.Object);

            _manejador = new InicioSesionManejador(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object);

            LimpiarSesionesActivas();
        }

        [TestCleanup]
        public void Limpiar()
        {
            LimpiarSesionesActivas();
        }

        private void LimpiarSesionesActivas()
        {
            SesionUsuarioManejador.Instancia.EliminarSesionPorNombre(NombreUsuarioValido);
        }

        private Usuario CrearUsuarioValido()
        {
            return new Usuario
            {
                idUsuario = IdUsuarioValido,
                Nombre_Usuario = NombreUsuarioValido,
                Contrasena = _hashContrasenaValida,
                Jugador = new Jugador
                {
                    idJugador = IdJugadorValido,
                    Nombre = NombreJugador,
                    Apellido = ApellidoJugador,
                    Correo = CorreoValido,
                    Id_Avatar = IdAvatarValido
                }
            };
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InicioSesionManejador(null, _repositorioFactoriaMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new InicioSesionManejador(_contextoFactoriaMock.Object, null));
        }

        [TestMethod]
        public void Prueba_IniciarSesion_CredencialesNulas_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _manejador.IniciarSesion(null));
        }

        [TestMethod]
        public void Prueba_IniciarSesion_IdentificadorVacio_RetornaCredencialesInvalidas()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = IdentificadorVacio,
                Contrasena = ContrasenaValida
            };

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsTrue(resultado.CuentaEncontrada);
            Assert.IsFalse(resultado.InicioSesionExitoso);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_IdentificadorSoloEspacios_RetornaCredencialesInvalidas()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = IdentificadorEspacios,
                Contrasena = ContrasenaValida
            };

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsTrue(resultado.CuentaEncontrada);
            Assert.IsFalse(resultado.InicioSesionExitoso);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_ContrasenaVacia_RetornaCredencialesInvalidas()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = IdentificadorVacio
            };

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsTrue(resultado.CuentaEncontrada);
            Assert.IsFalse(resultado.InicioSesionExitoso);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_UsuarioNoExiste_RetornaCuentaNoEncontrada()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Throws(new BaseDatosExcepcion(
                    "No encontrado", 
                    new KeyNotFoundException()));

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreo(NombreUsuarioValido))
                .Throws(new BaseDatosExcepcion(
                    "No encontrado", 
                    new KeyNotFoundException()));

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsFalse(resultado.CuentaEncontrada);
            Assert.IsFalse(resultado.InicioSesionExitoso);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_ContrasenaIncorrecta_RetornaContrasenaIncorrecta()
        {
            var usuario = CrearUsuarioValido();
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaInvalida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Returns(usuario);

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsTrue(resultado.ContrasenaIncorrecta);
            Assert.IsFalse(resultado.InicioSesionExitoso);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_UsuarioBaneado_RetornaMensajeBaneo()
        {
            var usuario = CrearUsuarioValido();
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Returns(usuario);

            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ContarReportesRecibidos(IdUsuarioValido))
                .Returns(ReportesLimite);

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsFalse(resultado.InicioSesionExitoso);
            Assert.IsTrue(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_CredencialesCorrectas_RetornaSesionExitosa()
        {
            var usuario = CrearUsuarioValido();
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Returns(usuario);

            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ContarReportesRecibidos(IdUsuarioValido))
                .Returns(CeroReportes);

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsTrue(resultado.InicioSesionExitoso);
            Assert.AreEqual(NombreUsuarioValido, resultado.Usuario.NombreUsuario);
            Assert.AreEqual(IdUsuarioValido, resultado.Usuario.UsuarioId);
            Assert.AreEqual(IdJugadorValido, resultado.Usuario.JugadorId);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_SesionDuplicada_RetornaSesionDuplicada()
        {
            var usuario = CrearUsuarioValido();
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Returns(usuario);

            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ContarReportesRecibidos(IdUsuarioValido))
                .Returns(CeroReportes);

            _manejador.IniciarSesion(credenciales);
            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsFalse(resultado.InicioSesionExitoso);
            Assert.IsTrue(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_BusquedaPorCorreo_RetornaSesionExitosa()
        {
            var usuario = CrearUsuarioValido();
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = CorreoValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(CorreoValido))
                .Throws(new BaseDatosExcepcion(
                    "No encontrado", 
                    new KeyNotFoundException()));

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorCorreo(CorreoValido))
                .Returns(usuario);

            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ContarReportesRecibidos(IdUsuarioValido))
                .Returns(CeroReportes);

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsTrue(resultado.InicioSesionExitoso);
            Assert.AreEqual(NombreUsuarioValido, resultado.Usuario.NombreUsuario);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_ErrorEntidad_RetornaErrorGenerico()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Throws(new EntityException());

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsFalse(resultado.InicioSesionExitoso);
            Assert.IsTrue(resultado.CuentaEncontrada);
        }

        [TestMethod]
        public void Prueba_CerrarSesion_NombreUsuarioValido_EliminaSesion()
        {
            var usuario = CrearUsuarioValido();
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Returns(usuario);

            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ContarReportesRecibidos(IdUsuarioValido))
                .Returns(CeroReportes);

            _manejador.IniciarSesion(credenciales);

            _manejador.CerrarSesion(NombreUsuarioValido);
            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsTrue(resultado.InicioSesionExitoso);
        }

        [TestMethod]
        public void Prueba_CerrarSesion_NombreUsuarioVacio_SesionPermaneceIntacta()
        {
            var usuario = CrearUsuarioValido();
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Returns(usuario);

            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ContarReportesRecibidos(IdUsuarioValido))
                .Returns(CeroReportes);

            _manejador.IniciarSesion(credenciales);
            _manejador.CerrarSesion(IdentificadorVacio);

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsFalse(resultado.InicioSesionExitoso);
        }

        [TestMethod]
        public void Prueba_CerrarSesion_NombreUsuarioNulo_SesionPermaneceIntacta()
        {
            var usuario = CrearUsuarioValido();
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioValido,
                Contrasena = ContrasenaValida
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido))
                .Returns(usuario);

            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ContarReportesRecibidos(IdUsuarioValido))
                .Returns(CeroReportes);

            _manejador.IniciarSesion(credenciales);
            _manejador.CerrarSesion(null);

            ResultadoInicioSesionDTO resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsFalse(resultado.InicioSesionExitoso);
        }
    }
}
