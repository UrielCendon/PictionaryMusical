using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class PerfilManejadorPruebas
    {
        private const int IdUsuarioValido = 1;
        private const int IdUsuarioInvalido = 0;
        private const int IdUsuarioNegativo = -1;
        private const int IdJugadorValido = 10;
        private const int IdAvatarValido = 1;
        private const int IdAvatarInvalido = 0;
        private const string NombreUsuarioValido = "UsuarioPrueba";
        private const string NombreValido = "NombreTest";
        private const string ApellidoValido = "ApellidoTest";
        private const string CorreoValido = "usuario@correo.com";
        private const string InstagramValido = "usuario_instagram";
        private const string FacebookValido = "usuario_facebook";
        private const string XValido = "usuario_x";
        private const string DiscordValido = "usuario_discord";

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();

            _mockContextoFactoria.Setup(factoria => factoria.CrearContexto())
                .Returns(_mockContexto.Object);
            _mockRepositorioFactoria.Setup(factoria => 
                factoria.CrearUsuarioRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_mockUsuarioRepositorio.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new PerfilManejador(null, _mockRepositorioFactoria.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new PerfilManejador(_mockContextoFactoria.Object, null));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_LanzaFaultExceptionIdUsuarioInvalido()
        {
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerPerfil(IdUsuarioInvalido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_LanzaFaultExceptionIdUsuarioNegativo()
        {
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerPerfil(IdUsuarioNegativo));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_LanzaFaultExceptionUsuarioNoEncontrado()
        {
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns((Usuario)null);
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_LanzaFaultExceptionJugadorNoAsociado()
        {
            var usuario = new Usuario
            {
                idUsuario = IdUsuarioValido,
                Nombre_Usuario = NombreUsuarioValido,
                Jugador = null
            };
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_RetornaNombreUsuarioCorrecto()
        {
            var usuario = CrearUsuarioConJugadorYRedesSociales();
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);
            var manejador = CrearManejador();

            var resultado = manejador.ObtenerPerfil(IdUsuarioValido);

            Assert.AreEqual(NombreUsuarioValido, resultado.NombreUsuario);
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_RetornaNombreCorrecto()
        {
            var usuario = CrearUsuarioConJugadorYRedesSociales();
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);
            var manejador = CrearManejador();

            var resultado = manejador.ObtenerPerfil(IdUsuarioValido);

            Assert.AreEqual(NombreValido, resultado.Nombre);
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_RetornaApellidoCorrecto()
        {
            var usuario = CrearUsuarioConJugadorYRedesSociales();
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);
            var manejador = CrearManejador();

            var resultado = manejador.ObtenerPerfil(IdUsuarioValido);

            Assert.AreEqual(ApellidoValido, resultado.Apellido);
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_LanzaFaultExceptionErrorBaseDatos()
        {
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new System.Data.Entity.Core.EntityException());
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_LanzaFaultExceptionErrorDatos()
        {
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new DataException());
            var manejador = CrearManejador();

            Assert.ThrowsException<FaultException>(
                () => manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_RetornaFalsoParaSolicitudNula()
        {
            var manejador = CrearManejador();

            var resultado = manejador.ActualizarPerfil(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_RetornaFalsoParaIdUsuarioInvalido()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.UsuarioId = IdUsuarioInvalido;
            var manejador = CrearManejador();

            var resultado = manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_RetornaFalsoParaAvatarIdInvalido()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.AvatarId = IdAvatarInvalido;
            var manejador = CrearManejador();

            var resultado = manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_RetornaVerdaderoParaSolicitudValida()
        {
            var usuario = CrearUsuarioConJugadorYRedesSociales();
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);
            var solicitud = CrearActualizacionPerfilValida();
            var manejador = CrearManejador();

            var resultado = manejador.ActualizarPerfil(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_RetornaFalsoParaUsuarioNoEncontrado()
        {
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns((Usuario)null);
            var solicitud = CrearActualizacionPerfilValida();
            var manejador = CrearManejador();

            var resultado = manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_RetornaFalsoParaErrorBaseDatos()
        {
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new System.Data.Entity.Core.EntityException());
            var solicitud = CrearActualizacionPerfilValida();
            var manejador = CrearManejador();

            var resultado = manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_GuardaCambiosEnContexto()
        {
            var usuario = CrearUsuarioConJugadorYRedesSociales();
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);
            var solicitud = CrearActualizacionPerfilValida();
            var manejador = CrearManejador();

            manejador.ActualizarPerfil(solicitud);

            _mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        private PerfilManejador CrearManejador()
        {
            return new PerfilManejador(
                _mockContextoFactoria.Object, 
                _mockRepositorioFactoria.Object);
        }

        private Usuario CrearUsuarioConJugadorYRedesSociales()
        {
            var redSocial = new RedSocial
            {
                Instagram = InstagramValido,
                facebook = FacebookValido,
                x = XValido,
                discord = DiscordValido
            };

            var jugador = new Jugador
            {
                idJugador = IdJugadorValido,
                Nombre = NombreValido,
                Apellido = ApellidoValido,
                Correo = CorreoValido,
                Id_Avatar = IdAvatarValido,
                RedSocial = new List<RedSocial> { redSocial }
            };

            return new Usuario
            {
                idUsuario = IdUsuarioValido,
                Nombre_Usuario = NombreUsuarioValido,
                Jugador = jugador
            };
        }

        private ActualizacionPerfilDTO CrearActualizacionPerfilValida()
        {
            return new ActualizacionPerfilDTO
            {
                UsuarioId = IdUsuarioValido,
                Nombre = NombreValido,
                Apellido = ApellidoValido,
                AvatarId = IdAvatarValido,
                Instagram = InstagramValido,
                Facebook = FacebookValido,
                X = XValido,
                Discord = DiscordValido
            };
        }
    }
}
