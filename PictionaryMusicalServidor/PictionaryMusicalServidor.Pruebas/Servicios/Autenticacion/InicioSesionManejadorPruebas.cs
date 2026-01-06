using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Datos.Modelo;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class InicioSesionManejadorPruebas
    {
        private const string NombreUsuarioPrueba = "UsuarioPrueba";
        private const string CorreoPrueba = "usuario@test.com";
        private const string ContrasenaPrueba = "Contrasena123!";
        private const int IdUsuarioPrueba = 1;
        private const int IdJugadorPrueba = 1;

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<ISesionUsuarioManejador> _mockSesionManejador;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
        private Mock<IReporteRepositorio> _mockReporteRepositorio;
        private InicioSesionManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockSesionManejador = new Mock<ISesionUsuarioManejador>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();
            _mockReporteRepositorio = new Mock<IReporteRepositorio>();

            _mockContextoFactoria
                .Setup(factoria => factoria.CrearContexto())
                .Returns(_mockContexto.Object);

            _mockRepositorioFactoria
                .Setup(factoria => factoria.CrearUsuarioRepositorio(_mockContexto.Object))
                .Returns(_mockUsuarioRepositorio.Object);

            _mockRepositorioFactoria
                .Setup(factoria => factoria.CrearReporteRepositorio(_mockContexto.Object))
                .Returns(_mockReporteRepositorio.Object);

            _manejador = new InicioSesionManejador(
                _mockContextoFactoria.Object,
                _mockRepositorioFactoria.Object,
                _mockSesionManejador.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new InicioSesionManejador(
                    null, 
                    _mockRepositorioFactoria.Object, 
                    _mockSesionManejador.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new InicioSesionManejador(
                    _mockContextoFactoria.Object, 
                    null, 
                    _mockSesionManejador.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionSesionManejadorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new InicioSesionManejador(
                    _mockContextoFactoria.Object, 
                    _mockRepositorioFactoria.Object, 
                    null));
        }

        [TestMethod]
        public void Prueba_IniciarSesion_LanzaExcepcionCredencialesNulas()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _manejador.IniciarSesion(null));
        }

        [TestMethod]
        public void Prueba_IniciarSesion_RetornaDatosInvalidosIdentificadorVacio()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = string.Empty,
                Contrasena = ContrasenaPrueba
            };

            var resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsNotNull(resultado.Mensaje);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_RetornaDatosInvalidosContrasenaVacia()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioPrueba,
                Contrasena = string.Empty
            };

            var resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsNotNull(resultado.Mensaje);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_RetornaDatosInvalidosContrasenaNula()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioPrueba,
                Contrasena = null
            };

            var resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsNotNull(resultado.Mensaje);
        }

        [TestMethod]
        public void Prueba_IniciarSesion_RetornaDatosInvalidosContrasenaSoloEspacios()
        {
            var credenciales = new CredencialesInicioSesionDTO
            {
                Identificador = NombreUsuarioPrueba,
                Contrasena = "   "
            };

            var resultado = _manejador.IniciarSesion(credenciales);

            Assert.IsNotNull(resultado.Mensaje);
        }

        [TestMethod]
        public void Prueba_CerrarSesion_LlamaEliminarSesionPorNombre()
        {
            _manejador.CerrarSesion(NombreUsuarioPrueba);

            _mockSesionManejador.Verify(
                sesion => sesion.EliminarSesionPorNombre(NombreUsuarioPrueba), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CerrarSesion_NoLlamaMetodoConNombreNulo()
        {
            _manejador.CerrarSesion(null);

            _mockSesionManejador.Verify(
                sesion => sesion.EliminarSesionPorNombre(It.IsAny<string>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_CerrarSesion_NoLlamaMetodoConNombreVacio()
        {
            _manejador.CerrarSesion(string.Empty);

            _mockSesionManejador.Verify(
                sesion => sesion.EliminarSesionPorNombre(It.IsAny<string>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_CerrarSesion_NoLlamaMetodoConNombreSoloEspacios()
        {
            _manejador.CerrarSesion("   ");

            _mockSesionManejador.Verify(
                sesion => sesion.EliminarSesionPorNombre(It.IsAny<string>()), 
                Times.Never);
        }
    }
}
