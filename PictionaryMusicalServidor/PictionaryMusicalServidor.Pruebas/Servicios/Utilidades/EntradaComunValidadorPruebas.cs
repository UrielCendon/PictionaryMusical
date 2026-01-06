using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Utilidades
{
    [TestClass]
    public class EntradaComunValidadorPruebas
    {
        private const string TextoValido = "TextoValido";
        private const string TextoConEspacios = "  TextoConEspacios  ";
        private const string TextoVacio = "";
        private const string TextoSoloEspacios = "   ";
        private const string CorreoValido = "usuario@dominio.com";
        private const string CorreoInvalido = "correo_invalido";
        private const string ContrasenaValida = "Password1!";
        private const string ContrasenaSinMayuscula = "password1!";
        private const string ContrasenaSinNumero = "Password!";
        private const string ContrasenaSinEspecial = "Password1";
        private const string ContrasenaCorta = "Pass1!";
        private const string TokenValido = "a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4";
        private const string TokenInvalido = "tokeninvalido";
        private const string CodigoVerificacionValido = "123456";
        private const string CodigoVerificacionInvalido = "12345";
        private const string CodigoSalaValido = "123456";
        private const string CodigoSalaInvalido = "12345";
        private const string MensajeValido = "Mensaje de prueba";
        private const string NombreUsuarioValido = "UsuarioPrueba";
        private const string IdSalaValido = "sala123";
        private const string IdJugadorValido = "jugador123";
        private const int IdUsuarioValido = 1;
        private const int IdUsuarioInvalido = 0;
        private const int IdUsuarioNegativo = -1;
        private const int AvatarIdValido = 1;
        private const int AvatarIdInvalido = 0;
        private const int NumeroRondasValido = 2;
        private const int NumeroRondasInvalido = 0;
        private const int NumeroRondasExcedido = 10;
        private const int TiempoRondaValido = 60;
        private const int TiempoRondaInvalido = 0;
        private const int TiempoRondaExcedido = 200;
        private const int LongitudMaximaTexto = 50;
        private const int LongitudMaximaMensaje = 150;
        private const int LongitudMaximaReporte = 100;

        [TestMethod]
        public void Prueba_NormalizarTexto_RetornaNuloParaValorNulo()
        {
            string resultado = EntradaComunValidador.NormalizarTexto(null);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_NormalizarTexto_RetornaNuloParaEspaciosEnBlanco()
        {
            string resultado = EntradaComunValidador.NormalizarTexto(TextoSoloEspacios);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_NormalizarTexto_EliminaEspaciosAlInicioYFinal()
        {
            string resultado = EntradaComunValidador.NormalizarTexto(TextoConEspacios);

            Assert.AreEqual(TextoConEspacios.Trim(), resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_RetornaVerdaderoParaTextoValido()
        {
            bool resultado = EntradaComunValidador.EsLongitudValida(TextoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_RetornaFalsoParaTextoVacio()
        {
            bool resultado = EntradaComunValidador.EsLongitudValida(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_RetornaFalsoParaTextoNulo()
        {
            bool resultado = EntradaComunValidador.EsLongitudValida(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_RetornaFalsoParaTextoExcedeLongitud()
        {
            string textoLargo = new string('a', LongitudMaximaTexto + 1);

            bool resultado = EntradaComunValidador.EsLongitudValida(textoLargo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValidaReporte_RetornaVerdaderoParaTextoValido()
        {
            bool resultado = EntradaComunValidador.EsLongitudValidaReporte(TextoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValidaReporte_RetornaFalsoParaTextoExcedeLongitud()
        {
            string textoLargo = new string('a', LongitudMaximaReporte + 1);

            bool resultado = EntradaComunValidador.EsLongitudValidaReporte(textoLargo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCorreoValido_RetornaVerdaderoParaCorreoCorrecto()
        {
            bool resultado = EntradaComunValidador.EsCorreoValido(CorreoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsCorreoValido_RetornaFalsoParaCorreoIncorrecto()
        {
            bool resultado = EntradaComunValidador.EsCorreoValido(CorreoInvalido);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCorreoValido_RetornaFalsoParaCorreoNulo()
        {
            bool resultado = EntradaComunValidador.EsCorreoValido(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_RetornaVerdaderoParaContrasenaCorrecta()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaValida);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_RetornaFalsoParaContrasenaSinMayuscula()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaSinMayuscula);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_RetornaFalsoParaContrasenaSinNumero()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaSinNumero);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_RetornaFalsoParaContrasenaSinCaracterEspecial()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaSinEspecial);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_RetornaFalsoParaContrasenaCorta()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaCorta);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_RetornaFalsoParaContrasenaNula()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsTokenValido_RetornaVerdaderoParaTokenCorrecto()
        {
            bool resultado = EntradaComunValidador.EsTokenValido(TokenValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsTokenValido_RetornaFalsoParaTokenIncorrecto()
        {
            bool resultado = EntradaComunValidador.EsTokenValido(TokenInvalido);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsTokenValido_RetornaFalsoParaTokenNulo()
        {
            bool resultado = EntradaComunValidador.EsTokenValido(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_RetornaVerdaderoParaCodigoCorrecto()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(
                CodigoVerificacionValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_RetornaFalsoParaCodigoIncorrecto()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(
                CodigoVerificacionInvalido);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_RetornaFalsoParaCodigoNulo()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_RetornaVerdaderoParaCodigoCorrecto()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(CodigoSalaValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_RetornaFalsoParaCodigoIncorrecto()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(CodigoSalaInvalido);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_RetornaFalsoParaCodigoNulo()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeValido_RetornaVerdaderoParaMensajeCorrecto()
        {
            bool resultado = EntradaComunValidador.EsMensajeValido(MensajeValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeValido_RetornaFalsoParaMensajeVacio()
        {
            bool resultado = EntradaComunValidador.EsMensajeValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeValido_RetornaFalsoParaMensajeExcedeLongitud()
        {
            string mensajeLargo = new string('a', LongitudMaximaMensaje + 1);

            bool resultado = EntradaComunValidador.EsMensajeValido(mensajeLargo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsIdiomaValido_RetornaVerdaderoParaIdiomaValido()
        {
            bool resultado = EntradaComunValidador.EsIdiomaValido(TextoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsIdiomaValido_RetornaFalsoParaIdiomaVacio()
        {
            bool resultado = EntradaComunValidador.EsIdiomaValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsDificultadValida_RetornaVerdaderoParaDificultadValida()
        {
            bool resultado = EntradaComunValidador.EsDificultadValida(TextoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsDificultadValida_RetornaFalsoParaDificultadVacia()
        {
            bool resultado = EntradaComunValidador.EsDificultadValida(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsNombreUsuarioValido_RetornaVerdaderoParaNombreValido()
        {
            bool resultado = EntradaComunValidador.EsNombreUsuarioValido(NombreUsuarioValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsNombreUsuarioValido_RetornaFalsoParaNombreVacio()
        {
            bool resultado = EntradaComunValidador.EsNombreUsuarioValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarNombreUsuario_LanzaFaultExceptionParaNombreInvalido()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarNombreUsuario(TextoVacio, "nombreUsuario"));
        }

        [TestMethod]
        public void Prueba_ValidarNombreUsuario_NoLanzaExcepcionParaNombreValido()
        {
            EntradaComunValidador.ValidarNombreUsuario(NombreUsuarioValido, "nombreUsuario");
        }

        [TestMethod]
        public void Prueba_ObtenerNombreUsuarioNormalizado_RetornaPrincipalSiEsValido()
        {
            string resultado = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                TextoConEspacios, 
                NombreUsuarioValido);

            Assert.AreEqual(TextoConEspacios.Trim(), resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerNombreUsuarioNormalizado_RetornaAlternoSiPrincipalInvalido()
        {
            string resultado = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                null, 
                NombreUsuarioValido);

            Assert.AreEqual(NombreUsuarioValido, resultado);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_RetornaFalsoParaCuentaNula()
        {
            var resultado = EntradaComunValidador.ValidarNuevaCuenta(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_RetornaFalsoParaUsuarioInvalido()
        {
            var cuenta = CrearCuentaValida();
            cuenta.Usuario = TextoVacio;

            var resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_RetornaFalsoParaCorreoInvalido()
        {
            var cuenta = CrearCuentaValida();
            cuenta.Correo = CorreoInvalido;

            var resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_RetornaFalsoParaContrasenaInvalida()
        {
            var cuenta = CrearCuentaValida();
            cuenta.Contrasena = ContrasenaCorta;

            var resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_RetornaVerdaderoParaCuentaValida()
        {
            var cuenta = CrearCuentaValida();

            var resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_RetornaFalsoParaSolicitudNula()
        {
            var resultado = EntradaComunValidador.ValidarActualizacionPerfil(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_RetornaFalsoParaIdUsuarioInvalido()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.UsuarioId = IdUsuarioInvalido;

            var resultado = EntradaComunValidador.ValidarActualizacionPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_RetornaFalsoParaAvatarInvalido()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.AvatarId = AvatarIdInvalido;

            var resultado = EntradaComunValidador.ValidarActualizacionPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_RetornaVerdaderoParaSolicitudValida()
        {
            var solicitud = CrearActualizacionPerfilValida();

            var resultado = EntradaComunValidador.ValidarActualizacionPerfil(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_LanzaExcepcionParaConfiguracionNula()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarConfiguracionPartida(null));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_LanzaExcepcionParaRondasInvalidas()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.NumeroRondas = NumeroRondasInvalido;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_LanzaExcepcionParaRondasExcedidas()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.NumeroRondas = NumeroRondasExcedido;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_LanzaExcepcionParaTiempoInvalido()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.TiempoPorRondaSegundos = TiempoRondaInvalido;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_LanzaExcepcionParaTiempoExcedido()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.TiempoPorRondaSegundos = TiempoRondaExcedido;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_LanzaExcepcionParaIdiomaInvalido()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.IdiomaCanciones = TextoVacio;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_LanzaExcepcionParaDificultadInvalida()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.Dificultad = TextoVacio;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarEntradaSalaChat_LanzaExcepcionParaNombreJugadorInvalido()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarEntradaSalaChat(
                    CodigoSalaValido, 
                    TextoVacio));
        }

        [TestMethod]
        public void Prueba_ValidarEntradaSalaChat_LanzaExcepcionParaCodigoSalaInvalido()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarEntradaSalaChat(
                    CodigoSalaInvalido, 
                    NombreUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ValidarCodigoSala_LanzaExcepcionParaCodigoInvalido()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarCodigoSala(CodigoSalaInvalido));
        }

        [TestMethod]
        public void Prueba_ValidarIdSala_LanzaExcepcionParaIdVacio()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarIdSala(TextoVacio));
        }

        [TestMethod]
        public void Prueba_ValidarIdJugador_LanzaExcepcionParaIdVacio()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarIdJugador(TextoVacio));
        }

        [TestMethod]
        public void Prueba_ValidarSuscripcionJugador_LanzaExcepcionParaSuscripcionNula()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarSuscripcionJugador(null));
        }

        [TestMethod]
        public void Prueba_ValidarSuscripcionJugador_LanzaExcepcionParaIdSalaVacio()
        {
            var suscripcion = new SuscripcionJugadorDTO
            {
                IdSala = TextoVacio,
                IdJugador = IdJugadorValido
            };

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarSuscripcionJugador(suscripcion));
        }

        [TestMethod]
        public void Prueba_SuperaLimiteCaracteresMensaje_RetornaVerdaderoParaMensajeLargo()
        {
            string mensajeLargo = new string('a', LongitudMaximaMensaje + 1);

            bool resultado = EntradaComunValidador.SuperaLimiteCaracteresMensaje(mensajeLargo);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_SuperaLimiteCaracteresMensaje_RetornaFalsoParaMensajeCorto()
        {
            bool resultado = EntradaComunValidador.SuperaLimiteCaracteresMensaje(MensajeValido);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarMensajeJuego_LanzaExcepcionParaIdSalaVacio()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarMensajeJuego(MensajeValido, TextoVacio));
        }

        [TestMethod]
        public void Prueba_ValidarMensajeJuego_LanzaExcepcionParaMensajeLargo()
        {
            string mensajeLargo = new string('a', LongitudMaximaMensaje + 1);

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarMensajeJuego(mensajeLargo, IdSalaValido));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_LanzaExcepcionParaReporteNulo()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarReporteJugador(null));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_LanzaExcepcionParaReportanteInvalido()
        {
            var reporte = CrearReporteJugadorValido();
            reporte.NombreUsuarioReportante = TextoVacio;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarReporteJugador(reporte));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_LanzaExcepcionParaReportadoInvalido()
        {
            var reporte = CrearReporteJugadorValido();
            reporte.NombreUsuarioReportado = TextoVacio;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarReporteJugador(reporte));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_LanzaExcepcionParaMotivoVacio()
        {
            var reporte = CrearReporteJugadorValido();
            reporte.Motivo = TextoVacio;

            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarReporteJugador(reporte));
        }

        [TestMethod]
        public void Prueba_ValidarInvitacionSala_LanzaExcepcionParaInvitacionNula()
        {
            Assert.ThrowsException<ArgumentException>(
                () => EntradaComunValidador.ValidarInvitacionSala(null));
        }

        [TestMethod]
        public void Prueba_ValidarInvitacionSala_LanzaExcepcionParaCodigoSalaInvalido()
        {
            var invitacion = new InvitacionSalaDTO
            {
                CodigoSala = CodigoSalaInvalido,
                Correo = CorreoValido
            };

            Assert.ThrowsException<ArgumentException>(
                () => EntradaComunValidador.ValidarInvitacionSala(invitacion));
        }

        [TestMethod]
        public void Prueba_ValidarInvitacionSala_LanzaExcepcionParaCorreoInvalido()
        {
            var invitacion = new InvitacionSalaDTO
            {
                CodigoSala = CodigoSalaValido,
                Correo = CorreoInvalido
            };

            Assert.ThrowsException<ArgumentException>(
                () => EntradaComunValidador.ValidarInvitacionSala(invitacion));
        }

        [TestMethod]
        public void Prueba_ValidarIdUsuario_LanzaExcepcionParaIdCero()
        {
            Assert.ThrowsException<ArgumentException>(
                () => EntradaComunValidador.ValidarIdUsuario(IdUsuarioInvalido));
        }

        [TestMethod]
        public void Prueba_ValidarIdUsuario_LanzaExcepcionParaIdNegativo()
        {
            Assert.ThrowsException<ArgumentException>(
                () => EntradaComunValidador.ValidarIdUsuario(IdUsuarioNegativo));
        }

        [TestMethod]
        public void Prueba_ValidarNombreUsuarioSuscripcion_LanzaExcepcionParaNombreVacio()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarNombreUsuarioSuscripcion(TextoVacio));
        }

        [TestMethod]
        public void Prueba_ValidarUsuariosInteraccion_LanzaExcepcionParaPrimerUsuarioInvalido()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarUsuariosInteraccion(
                    TextoVacio, 
                    NombreUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ValidarUsuariosInteraccion_LanzaExcepcionParaSegundoUsuarioInvalido()
        {
            Assert.ThrowsException<FaultException>(
                () => EntradaComunValidador.ValidarUsuariosInteraccion(
                    NombreUsuarioValido, 
                    TextoVacio));
        }

        private NuevaCuentaDTO CrearCuentaValida()
        {
            return new NuevaCuentaDTO
            {
                Usuario = NombreUsuarioValido,
                Nombre = TextoValido,
                Apellido = TextoValido,
                Correo = CorreoValido,
                Contrasena = ContrasenaValida,
                AvatarId = AvatarIdValido
            };
        }

        private ActualizacionPerfilDTO CrearActualizacionPerfilValida()
        {
            return new ActualizacionPerfilDTO
            {
                UsuarioId = IdUsuarioValido,
                Nombre = TextoValido,
                Apellido = TextoValido,
                AvatarId = AvatarIdValido
            };
        }

        private ConfiguracionPartidaDTO CrearConfiguracionPartidaValida()
        {
            return new ConfiguracionPartidaDTO
            {
                NumeroRondas = NumeroRondasValido,
                TiempoPorRondaSegundos = TiempoRondaValido,
                IdiomaCanciones = TextoValido,
                Dificultad = TextoValido
            };
        }

        private ReporteJugadorDTO CrearReporteJugadorValido()
        {
            return new ReporteJugadorDTO
            {
                NombreUsuarioReportante = NombreUsuarioValido,
                NombreUsuarioReportado = "UsuarioReportado",
                Motivo = "Motivo de prueba"
            };
        }
    }
}
