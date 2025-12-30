using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Recursos = PictionaryMusicalCliente.Properties.Langs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Centraliza la traduccion de mensajes de error provenientes del servidor a recursos locales.
    /// </summary>
    public class LocalizadorServicio : ILocalizadorServicio
    {
        private static readonly Regex EsperaCodigoRegex = new Regex(
            @"^Debe esperar (\d+) segundos para solicitar un nuevo codigo\.$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

        private static readonly Regex IdentificadorRedSocialRegex = new Regex(
            @"^El identificador de (.+) no debe exceder (\d+) caracteres\.$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

        private static readonly Dictionary<string, Func<string>> MapaMensajes =
            new Dictionary<string, Func<string>>(StringComparer.Ordinal)
            {
                ["La partida ya comenzo, no puedes unirte."]
                    = () => Recursos.Lang.errorTextoPartidaYaIniciada,
                ["La partida ya esta en curso."]
                    = () => Recursos.Lang.errorTextoPartidaYaIniciada,
                ["La partida se cancelo porque ya no hay suficientes jugadores."]
                    = () => Recursos.Lang.partidaTextoJugadoresInsuficientes,
                ["La partida se cancelo porque el anfitrion abandono."]
                    = () => Recursos.Lang.partidaTextoHostCanceloSala,
                ["Espera a que se unan mas jugadores para iniciar."]
                    = () => Recursos.Lang.partidaTextoJugadoresInsuficientes,
                ["Solo el anfitrion puede iniciar la partida."]
                    = () => Recursos.Lang.errorTextoSoloHost,

                ["Reporte enviado correctamente."]
                    = () => Recursos.Lang.reportarJugadorTextoExito,
                ["No se pudo enviar el reporte. Intente mas tarde."]
                    = () => Recursos.Lang.errorTextoReportarJugador,
                ["Ya reportaste a este jugador anteriormente."]
                    = () => Recursos.Lang.reportarJugadorTextoDuplicado,
                ["Por favor, escribe el motivo del reporte."]
                    = () => Recursos.Lang.reportarJugadorTextoMotivoRequerido,
                ["El motivo del reporte es muy largo. Usa maximo 100 caracteres."]
                    = () => Recursos.Lang.reportarJugadorTextoMotivoLongitud,
                ["No puedes reportarte a ti mismo."]
                    = () => Recursos.Lang.reportarJugadorTextoAutoReporte,

                ["Tu mensaje es muy largo. Usa menos de 200 caracteres."]
                    = () => Recursos.Lang.MensajeChatTextoMotivoLongitud,
                ["No pudimos enviar el mensaje. Intenta de nuevo."]
                    = () => Recursos.Lang.errorTextoEnviarMensaje,

                ["No pudimos procesar tu solicitud de verificacion. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoProcesarSolicitudVerificacion,
                ["No pudimos reenviar el codigo de verificacion. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoServidorReenviarCodigo,
                ["No tienes una solicitud de verificacion activa. Registrate de nuevo."]
                    = () => Recursos.Lang.errorTextoSolicitudVerificacionActiva,
                ["El codigo de verificacion ha expirado. Registrate de nuevo."]
                    = () => Recursos.Lang.avisoTextoCodigoExpirado,
                ["El codigo ingresado no es correcto. Verifica e intenta de nuevo."]
                    = () => Recursos.Lang.errorTextoCodigoIncorrecto,
                ["El correo o usuario ya esta en uso. Intenta con otro."]
                    = () => Recursos.Lang.errorTextoCorreoEnUso,

                ["No pudimos procesar tu solicitud de recuperacion. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["No pudimos conectar con el servidor. Verifica tu conexion e intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoBaseDatosRecuperacion,
                ["No pudimos reenviar el codigo. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoServidorReenviarCodigo,
                ["No pudimos confirmar el codigo de recuperacion. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoServidorValidarCodigo,
                ["No pudimos actualizar tu contrasena. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoActualizarContrasena,
                ["Los datos ingresados no son correctos. Por favor, revisalos."]
                    = () => Recursos.Lang.errorTextoServidorSolicitudCambioContrasena,
                ["Los datos ingresados no son correctos para reenviar el codigo."]
                    = () => Recursos.Lang.errorTextoServidorSolicitudCambioContrasena,
                ["Los datos de confirmacion no son correctos. Revisalos."]
                    = () => Recursos.Lang.errorTextoSolicitudVerificacionInvalida,
                ["Los datos para actualizar la contrasena no son correctos."]
                    = () => Recursos.Lang.errorTextoPrepararSolicitudCambioContrasena,
                ["Los datos ingresados no son correctos para solicitar el codigo."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["Ingresa tu usuario o correo (maximo 50 caracteres)."]
                    = () => Recursos.Lang.errorTextoIdentificadorRecuperacionRequerido,
                ["No encontramos una cuenta con ese usuario o correo."]
                    = () => Recursos.Lang.errorTextoCuentaNoRegistrada,
                ["No tienes una solicitud de recuperacion activa. Inicia el proceso de nuevo."]
                    = () => Recursos.Lang.errorTextoSolicitudRecuperacionActiva,
                ["El codigo ha expirado. Solicita uno nuevo."]
                    = () => Recursos.Lang.errorTextoCodigoExpiradoSolicitarNuevo,
                ["El codigo ingresado no es correcto. Verifica e intenta de nuevo."]
                    = () => Recursos.Lang.errorTextoCodigoIncorrecto,
                ["No tienes una solicitud de recuperacion activa."]
                    = () => Recursos.Lang.errorTextoSolicitudRecuperacionVigente,
                ["La solicitud de recuperacion ya no es valida. Inicia el proceso de nuevo."]
                    = () => Recursos.Lang.errorTextoSolicitudRecuperacionInvalida,

                ["No pudimos completar tu registro. Por favor, intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoRegistrarCuentaMasTarde,
                ["Tu cuenta ha sido suspendida por mala conducta."]
                    = () => Recursos.Lang.errorTextoUsuarioBaneado,
                ["Usuario o contrasena incorrectos."]
                    = () => Recursos.Lang.errorTextoCredencialesIncorrectas,
                ["Algunos datos no son correctos. Por favor, revisa la informacion ingresada."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,

                ["Ingresa un correo electronico valido (maximo 50 caracteres)."]
                    = () => Recursos.Lang.errorTextoCorreoInvalido,
                ["El nombre de usuario es obligatorio y debe tener maximo 50 caracteres."]
                    = () => Recursos.Lang.errorTextoIdentificadorUsuarioInvalido,
                ["El nombre es obligatorio y debe tener maximo 50 caracteres."]
                    = () => Recursos.Lang.errorTextoNombreObligatorioLongitud,
                ["El apellido es obligatorio y debe tener maximo 50 caracteres."]
                    = () => Recursos.Lang.errorTextoApellidoObligatorioLongitud,
                ["La contrasena debe tener entre 8 y 15 caracteres, incluir una mayuscula, un numero y un caracter especial."]
                    = () => Recursos.Lang.globalTextoEspecificaContrasena,

                ["No encontramos al usuario. Verifica que el nombre sea correcto."]
                    = () => Recursos.Lang.errorTextoUsuarioNoEncontrado,
                ["No pudimos iniciar sesion. Por favor, intenta mas tarde."]
                    = () => Recursos.Lang.inicioSesionErrorServicio,
                ["Error al obtener el usuario."]
                    = () => Recursos.Lang.amigosErrorBuscarBase,
                ["No encontramos a uno o mas usuarios especificados."]
                    = () => Recursos.Lang.amigosErrorUsuarioNoExiste,
                ["No encontramos la informacion del jugador."]
                    = () => Recursos.Lang.errorTextoJugadorNoExiste,
                ["El usuario no fue encontrado."]
                    = () => Recursos.Lang.amigosErrorUsuarioNoExiste,
                ["El usuario que buscas no existe. Verifica el nombre e intenta de nuevo."]
                    = () => Recursos.Lang.amigosErrorUsuarioNoExiste,
                ["No encontramos un jugador asociado a este usuario."]
                    = () => Recursos.Lang.errorTextoJugadorNoExiste,
                ["Tu cuenta aun no ha sido verificada. Revisa tu correo electronico."]
                    = () => Recursos.Lang.errorTextoCuentaNoVerificada,
                ["No encontramos una cuenta con estos datos. Verifica la informacion."]
                    = () => Recursos.Lang.errorTextoCuentaNoRegistrada,
                ["Por favor, selecciona un avatar valido."]
                    = () => Recursos.Lang.errorTextoSeleccionAvatarValido,

                ["No pudimos cargar tu perfil. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoServidorObtenerPerfil,
                ["No pudimos guardar los cambios de tu perfil. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoActualizarPerfil,
                ["Tu perfil se actualizo correctamente."]
                    = () => Recursos.Lang.avisoTextoPerfilActualizado,

                ["No pudimos cargar tus solicitudes de amistad. Intenta mas tarde."]
                    = () => Recursos.Lang.amigosErrorRecuperarSolicitudes,
                ["No puedes enviarte una solicitud de amistad a ti mismo."]
                    = () => Recursos.Lang.amigosErrorAutoSolicitud,
                ["Ya tienes una solicitud o amistad con este usuario."]
                    = () => Recursos.Lang.amigosErrorRelacionExiste,
                ["No pudimos enviar la solicitud de amistad. Intenta mas tarde."]
                    = () => Recursos.Lang.amigosErrorCompletarSolicitud,
                ["No pudimos procesar la solicitud de amistad. Intenta mas tarde."]
                    = () => Recursos.Lang.amigosErrorActualizarSolicitud,
                ["No existe una solicitud de amistad con este usuario."]
                    = () => Recursos.Lang.amigosErrorSolicitudNoExiste,
                ["No pudimos aceptar la solicitud de amistad. Intenta mas tarde."]
                    = () => Recursos.Lang.amigosErrorAceptarSolicitud,
                ["Esta solicitud de amistad ya fue aceptada."]
                    = () => Recursos.Lang.amigosErrorSolicitudAceptada,
                ["No tienes una amistad con este usuario."]
                    = () => Recursos.Lang.amigosErrorRelacionNoExiste,
                ["No pudimos eliminar a este amigo. Intenta mas tarde."]
                    = () => Recursos.Lang.amigosErrorEliminarRelacion,
                ["No pudimos cargar tu lista de amigos. Intenta mas tarde."]
                    = () => Recursos.Lang.amigosErrorRecuperarSolicitudes,
                ["No pudimos cargar tus amigos debido a un problema del servidor."]
                    = () => Recursos.Lang.amigosErrorRecuperarSolicitudes,
                ["No pudimos enviar la notificacion de amistad. Intenta mas tarde."]
                    = () => Recursos.Lang.amigosErrorActualizarSolicitud,
                ["No pudimos notificar la eliminacion de amistad."]
                    = () => Recursos.Lang.amigosErrorEliminarRelacion,

                ["La invitacion no es valida o ha expirado."]
                    = () => Recursos.Lang.errorTextoInvitacionExpirada,
                ["Los datos de la invitacion no son correctos."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["Ingresa un correo electronico valido."]
                    = () => Recursos.Lang.errorTextoCorreoInvalido,
                ["No pudimos enviar la invitacion. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoEnviarInvitacion,
                ["Hubo un problema al procesar la invitacion."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["Este jugador ya esta en la sala."]
                    = () => Recursos.Lang.invitarCorreoTextoJugadorYaEnSala,
                ["Invitacion enviada correctamente."]
                    = () => Recursos.Lang.invitarCorreoTextoEnviado,
                ["La invitacion no es valida."]
                    = () => Recursos.Lang.errorTextoInvitacionExpirada,
                ["El jugador con este correo ya esta en la sala."]
                    = () => Recursos.Lang.invitarCorreoTextoJugadorYaEnSala,
                ["No pudimos enviar la invitacion al correo. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoEnviarCorreo,

                ["No pudimos crear la sala. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoCrearSala,
                ["Ocurrio un problema al crear la sala. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoCrearSala,
                ["No pudimos unirte a la sala. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoUnirsePartida,
                ["Ocurrio un problema al salir de la sala."]
                    = () => Recursos.Lang.errorTextoAbandonarPartida,
                ["No pudimos expulsar al jugador. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoExpulsarJugador,
                ["No pudimos conectarte a las salas. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoServidorNoDisponible,
                ["La sala ya no existe o el codigo es incorrecto."]
                    = () => Recursos.Lang.errorTextoNoEncuentraPartida,
                ["No pudimos generar el codigo de la sala. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoCrearSala,
                ["La sala esta llena. Intenta con otra sala."]
                    = () => Recursos.Lang.errorTextoSalaLlena,
                ["Solo el creador de la sala puede expulsar jugadores."]
                    = () => Recursos.Lang.errorTextoSoloCreador,
                ["El creador de la sala no puede ser expulsado."]
                    = () => Recursos.Lang.errorTextoCreadorNoExpulsable,
                ["El jugador ya no esta en la sala."]
                    = () => Recursos.Lang.errorTextoJugadorNoEnSala,

                ["No pudimos conectarte al servidor. Verifica tu conexion e intenta de nuevo."]
                    = () => Recursos.Lang.errorTextoServidorNoDisponible,
                ["No pudimos conectarte al servicio de amigos. Intenta mas tarde."]
                    = () => Recursos.Lang.errorTextoServidorNoDisponible,
                ["Ocurrio un problema de conexion. Intenta de nuevo."]
                    = () => Recursos.Lang.errorTextoServidorNoDisponible,
                ["Ocurrio un problema de conexion con el servicio de amigos."]
                    = () => Recursos.Lang.errorTextoServidorNoDisponible,

                ["Ingresa tu nombre de usuario."]
                    = () => Recursos.Lang.errorTextoIdentificadorUsuarioInvalido,
                ["Se requiere el nombre de usuario para cancelar."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["Se requiere el nombre de usuario para conectarte."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["Ingresa el codigo de la sala."]
                    = () => Recursos.Lang.errorTextoCodigoSalaRequerido,
                ["No se pudo identificar al jugador."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["No se pudo identificar la sala."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,

                ["Configura la partida antes de iniciar."]
                    = () => Recursos.Lang.errorTextoConfiguracionRequerida,
                ["El numero de rondas debe ser mayor a cero."]
                    = () => Recursos.Lang.errorTextoRondasInvalidas,
                ["El tiempo por ronda debe ser mayor a cero."]
                    = () => Recursos.Lang.errorTextoTiempoInvalido,
                ["Selecciona el idioma de las canciones."]
                    = () => Recursos.Lang.errorTextoIdiomaRequerido,
                ["Selecciona la dificultad."]
                    = () => Recursos.Lang.errorTextoDificultadRequerida,

                ["Error al actualizar la clasificacion."]
                    = () => Recursos.Lang.clasificacionErrorActualizar,

                ["Error al consultar los mejores jugadores."]
                    = () => Recursos.Lang.clasificacionErrorObtener,

                ["Ocurrio un error inesperado. Por favor, intente mas tarde."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["No se puede realizar esta accion en este momento. Intente mas tarde."]
                    = () => Recursos.Lang.errorTextoErrorProcesarSolicitud,
                ["La operacion se completo correctamente."]
                    = () => Recursos.Lang.avisoTextoOperacionExitosa
            };

        /// <summary>
        /// Intenta traducir un mensaje del servidor usando el mapa o expresiones regulares.
        /// </summary>
        /// <param name="mensaje">Mensaje recibido del servidor.</param>
        /// <param name="mensajePredeterminado">Mensaje alternativo si no hay traduccion.</param>
        /// <returns>Mensaje traducido o el predeterminado.</returns>
        public string Localizar(string mensaje, string mensajePredeterminado)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return ObtenerMensajePredeterminado(mensajePredeterminado);
            }

            string mensajeNormalizado = mensaje.Trim();

            if (TryLocalizarMensajeDinamico(mensajeNormalizado, out string mensajeTraducido))
            {
                return mensajeTraducido;
            }

            if (TryLocalizarMensajeEstatico(mensajeNormalizado, out string mensajeEstatico))
            {
                return mensajeEstatico;
            }

            return ObtenerMensajePredeterminado(mensajePredeterminado);
        }

        private static bool TryLocalizarMensajeDinamico(string mensaje, out string traducido)
        {
            if (IntentarCoincidenciaEsperaCodigo(mensaje, out traducido))
            {
                return true;
            }

            if (IntentarCoincidenciaRedSocial(mensaje, out traducido))
            {
                return true;
            }

            traducido = null;
            return false;
        }

        private static bool IntentarCoincidenciaEsperaCodigo(string mensaje, out string traducido)
        {
            Match espera = EsperaCodigoRegex.Match(mensaje);
            if (espera.Success)
            {
                traducido = string.Format(
                    CultureInfo.CurrentCulture,
                    Recursos.Lang.errorTextoTiempoEsperaCodigo,
                    espera.Groups[1].Value);
                return true;
            }

            traducido = null;
            return false;
        }

        private static bool IntentarCoincidenciaRedSocial(string mensaje, out string traducido)
        {
            Match identificador = IdentificadorRedSocialRegex.Match(mensaje);
            if (identificador.Success)
            {
                traducido = string.Format(
                    CultureInfo.CurrentCulture,
                    Recursos.Lang.errorTextoIdentificadorRedSocialLongitud,
                    identificador.Groups[1].Value,
                    identificador.Groups[2].Value);
                return true;
            }

            traducido = null;
            return false;
        }

        private static bool TryLocalizarMensajeEstatico(string mensaje, out string traducido)
        {
            if (MapaMensajes.TryGetValue(mensaje, out Func<string> traductor))
            {
                traducido = traductor();
                return true;
            }

            traducido = null;
            return false;
        }

        private static string ObtenerMensajePredeterminado(string mensajePredeterminado)
        {
            if (!string.IsNullOrWhiteSpace(mensajePredeterminado))
            {
                return mensajePredeterminado;
            }

            return Recursos.Lang.errorTextoErrorProcesarSolicitud;
        }
    }
}