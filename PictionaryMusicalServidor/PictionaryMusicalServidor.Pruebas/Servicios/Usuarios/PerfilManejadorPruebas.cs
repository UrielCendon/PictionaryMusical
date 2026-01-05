using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class PerfilManejadorPruebas
    {
        private const int IdUsuarioValido = 1;
        private const int IdJugadorValido = 10;
        private const int IdAvatarValido = 5;
        private const int IdUsuarioInvalido = 0;
        private const int IdUsuarioNegativo = -1;
        private const int IdAvatarInvalido = 0;
        private const string NombreUsuarioValido = "UsuarioTest";
        private const string NombreValido = "Juan";
        private const string ApellidoValido = "Perez";
        private const string CorreoValido = "test@correo.com";
        private const string InstagramValido = "instagram_user";
        private const string FacebookValido = "facebook_user";
        private const string XValido = "x_user";
        private const string DiscordValido = "discord_user";
        private const string CadenaVacia = "";
        private const string CadenaSoloEspacios = "   ";
        private const string CadenaMuyLarga = 
            "Esta es una cadena que excede el limite de cincuenta caracteres permitidos";

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private PerfilManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock
                .Setup(fabrica => fabrica.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearUsuarioRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _manejador = new PerfilManejador(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object);
        }

        private Usuario CrearUsuarioValido()
        {
            return new Usuario
            {
                idUsuario = IdUsuarioValido,
                Nombre_Usuario = NombreUsuarioValido,
                Jugador = new Jugador
                {
                    idJugador = IdJugadorValido,
                    Nombre = NombreValido,
                    Apellido = ApellidoValido,
                    Correo = CorreoValido,
                    Id_Avatar = IdAvatarValido,
                    RedSocial = new List<RedSocial>
                    {
                        new RedSocial
                        {
                            Instagram = InstagramValido,
                            facebook = FacebookValido,
                            x = XValido,
                            discord = DiscordValido
                        }
                    }
                }
            };
        }

        private Usuario CrearUsuarioSinRedesSociales()
        {
            return new Usuario
            {
                idUsuario = IdUsuarioValido,
                Nombre_Usuario = NombreUsuarioValido,
                Jugador = new Jugador
                {
                    idJugador = IdJugadorValido,
                    Nombre = NombreValido,
                    Apellido = ApellidoValido,
                    Correo = CorreoValido,
                    Id_Avatar = IdAvatarValido,
                    RedSocial = new List<RedSocial>()
                }
            };
        }

        private ActualizacionPerfilDTO CrearSolicitudActualizacionValida()
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

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new PerfilManejador(null, _repositorioFactoriaMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new PerfilManejador(_contextoFactoriaMock.Object, null));
        }

        // Fix prueba innecesaria
        [TestMethod]
        public void Prueba_ObtenerPerfil_UsuarioCompleto_RetornaTodosLosDatos()
        {
            Usuario usuario = CrearUsuarioValido();
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);

            UsuarioDTO resultado = _manejador.ObtenerPerfil(IdUsuarioValido);

            Assert.AreEqual(IdUsuarioValido, resultado.UsuarioId);
            Assert.AreEqual(NombreUsuarioValido, resultado.NombreUsuario);
            Assert.AreEqual(IdJugadorValido, resultado.JugadorId);
            Assert.AreEqual(NombreValido, resultado.Nombre);
            Assert.AreEqual(ApellidoValido, resultado.Apellido);
            Assert.AreEqual(InstagramValido, resultado.Instagram);
            Assert.AreEqual(FacebookValido, resultado.Facebook);
            Assert.AreEqual(XValido, resultado.X);
            Assert.AreEqual(DiscordValido, resultado.Discord);
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_UsuarioSinRedesSociales_RetornaRedesNulas()
        {
            Usuario usuario = CrearUsuarioSinRedesSociales();
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);

            UsuarioDTO resultado = _manejador.ObtenerPerfil(IdUsuarioValido);

            Assert.AreEqual(null, resultado.Instagram);
            Assert.AreEqual(null, resultado.Facebook);
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_IdInvalido_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() => 
                _manejador.ObtenerPerfil(IdUsuarioInvalido));
            Assert.ThrowsException<FaultException>(() => 
                _manejador.ObtenerPerfil(IdUsuarioNegativo));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_UsuarioNoExiste_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(() => 
                _manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_UsuarioSinJugador_LanzaFaultException()
        {
            Usuario usuarioSinJugador = new Usuario
            {
                idUsuario = IdUsuarioValido,
                Nombre_Usuario = NombreUsuarioValido,
                Jugador = null
            };
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuarioSinJugador);

            Assert.ThrowsException<FaultException>(() => 
                _manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_EntityException_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new EntityException());

            Assert.ThrowsException<FaultException>(() => 
                _manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_DataException_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new DataException());

            Assert.ThrowsException<FaultException>(() => 
                _manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_DbUpdateException_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new DbUpdateException());

            Assert.ThrowsException<FaultException>(() => 
                _manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_ExcepcionGenerica_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new Exception());

            Assert.ThrowsException<FaultException>(() => 
                _manejador.ObtenerPerfil(IdUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_SolicitudNula_RetornaOperacionFallida()
        {
            ResultadoOperacionDTO resultado = _manejador.ActualizarPerfil(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_IdUsuarioCero_RetornaOperacionFallida()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            solicitud.UsuarioId = IdUsuarioInvalido;

            ResultadoOperacionDTO resultado = _manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_AvatarIdCero_RetornaOperacionFallida()
        {
            var solicitud = CrearSolicitudActualizacionValida();
            solicitud.AvatarId = IdAvatarInvalido;

            ResultadoOperacionDTO resultado = _manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_NombreInvalido_RetornaOperacionFallida()
        {
            var solicitudNombreVacio = CrearSolicitudActualizacionValida();
            solicitudNombreVacio.Nombre = CadenaVacia;
            var solicitudNombreEspacios = CrearSolicitudActualizacionValida();
            solicitudNombreEspacios.Nombre = CadenaSoloEspacios;
            var solicitudNombreLargo = CrearSolicitudActualizacionValida();
            solicitudNombreLargo.Nombre = CadenaMuyLarga;

            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudNombreVacio).OperacionExitosa);
            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudNombreEspacios).OperacionExitosa);
            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudNombreLargo).OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_ApellidoInvalido_RetornaOperacionFallida()
        {
            var solicitudApellidoVacio = CrearSolicitudActualizacionValida();
            solicitudApellidoVacio.Apellido = CadenaVacia;
            var solicitudApellidoLargo = CrearSolicitudActualizacionValida();
            solicitudApellidoLargo.Apellido = CadenaMuyLarga;

            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudApellidoVacio).OperacionExitosa);
            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudApellidoLargo).OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_DatosValidos_RetornaOperacionExitosa()
        {
            Usuario usuario = CrearUsuarioValido();
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);

            var solicitud = CrearSolicitudActualizacionValida();

            ResultadoOperacionDTO resultado = _manejador.ActualizarPerfil(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_EntityException_RetornaOperacionFallida()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new EntityException());

            var solicitud = CrearSolicitudActualizacionValida();

            ResultadoOperacionDTO resultado = _manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_DataException_RetornaOperacionFallida()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Throws(new DataException());

            var solicitud = CrearSolicitudActualizacionValida();

            ResultadoOperacionDTO resultado = _manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_UsuarioNoExiste_RetornaOperacionFallida()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns((Usuario)null);

            var solicitud = CrearSolicitudActualizacionValida();

            ResultadoOperacionDTO resultado = _manejador.ActualizarPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_RedesSocialesMuyLargas_RetornaOperacionFallida()
        {
            var solicitudInstagram = CrearSolicitudActualizacionValida();
            solicitudInstagram.Instagram = CadenaMuyLarga;
            var solicitudFacebook = CrearSolicitudActualizacionValida();
            solicitudFacebook.Facebook = CadenaMuyLarga;
            var solicitudX = CrearSolicitudActualizacionValida();
            solicitudX.X = CadenaMuyLarga;
            var solicitudDiscord = CrearSolicitudActualizacionValida();
            solicitudDiscord.Discord = CadenaMuyLarga;

            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudInstagram).OperacionExitosa);
            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudFacebook).OperacionExitosa);
            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudX).OperacionExitosa);
            Assert.IsFalse(_manejador.ActualizarPerfil(solicitudDiscord).OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ActualizarPerfil_RedesSocialesVacias_RetornaOperacionExitosa()
        {
            Usuario usuario = CrearUsuarioValido();
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);

            var solicitud = CrearSolicitudActualizacionValida();
            solicitud.Instagram = null;
            solicitud.Facebook = null;
            solicitud.X = null;
            solicitud.Discord = null;

            ResultadoOperacionDTO resultado = _manejador.ActualizarPerfil(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ObtenerPerfil_AvatarNulo_RetornaCeroComoAvatar()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.Jugador.Id_Avatar = null;
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido))
                .Returns(usuario);

            UsuarioDTO resultado = _manejador.ObtenerPerfil(IdUsuarioValido);

            Assert.AreEqual(IdUsuarioInvalido, resultado.AvatarId);
        }
    }
}
