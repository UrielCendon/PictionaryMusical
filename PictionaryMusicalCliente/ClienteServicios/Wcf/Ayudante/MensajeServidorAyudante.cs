using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using LangResources = PictionaryMusicalCliente.Properties.Langs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante
{
    /// <summary>
    /// Centraliza la traduccion de mensajes de error provenientes del servidor a recursos locales.
    /// </summary>
    public static class MensajeServidorAyudante
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
                ["La partida ya comenzo"]
                    = () => LangResources.Lang.errorTextoPartidaYaIniciada,
                ["Partida cancelada por falta de jugadores."]
                    = () => LangResources.Lang.partidaTextoJugadoresInsuficientes,
                ["Reporte enviado correctamente."]
                    = () => LangResources.Lang.reportarJugadorTextoExito,
                ["No fue posible registrar el reporte."]
                    = () => LangResources.Lang.errorTextoReportarJugador,
                ["Ya has reportado a este jugador."]
                    = () => LangResources.Lang.reportarJugadorTextoDuplicado,
                ["El motivo del reporte es obligatorio."]
                    = () => LangResources.Lang.reportarJugadorTextoMotivoRequerido,
                ["El motivo del reporte no debe exceder 100 caracteres."]
                    = () => LangResources.Lang.reportarJugadorTextoMotivoLongitud,
                ["No puedes reportarte a ti mismo."]
                    = () => LangResources.Lang.reportarJugadorTextoAutoReporte,
                ["No fue posible procesar la solicitud de verificacion."]
                    = () => LangResources.Lang.errorTextoProcesarSolicitudVerificacion,
                ["No fue posible reenviar el codigo de verificacion."]
                    = () => LangResources.Lang.errorTextoServidorReenviarCodigo,
                ["No se encontro una solicitud de verificacion activa."]
                    = () => LangResources.Lang.errorTextoSolicitudVerificacionActiva,
                ["El codigo de verificacion ha expirado. Inicie el proceso nuevamente."]
                    = () => LangResources.Lang.avisoTextoCodigoExpirado,
                ["El codigo ingresado no es correcto."]
                    = () => LangResources.Lang.errorTextoCodigoIncorrecto,
                ["El correo o usuario ya esta registrado."]
                    = () => LangResources.Lang.errorTextoCorreoEnUso,
                ["No fue posible procesar la recuperacion de cuenta."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["No fue posible reenviar el codigo de recuperacion."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["No fue posible confirmar el codigo de recuperacion."]
                    = () => LangResources.Lang.errorTextoServidorValidarCodigo,
                ["No fue posible actualizar la contrasena."]
                    = () => LangResources.Lang.errorTextoActualizarContrasena,
                ["Los datos de recuperacion no son validos."]
                    = () => LangResources.Lang.errorTextoServidorSolicitudCambioContrasena,
                ["Los datos para reenviar el codigo no son validos."]
                    = () => LangResources.Lang.errorTextoServidorSolicitudCambioContrasena,
                ["Los datos de confirmacion no son validos."]
                    = () => LangResources.Lang.errorTextoSolicitudVerificacionInvalida,
                ["Los datos de actualizacion no son validos."]
                    = () => LangResources.Lang.errorTextoPrepararSolicitudCambioContrasena,
                ["Los datos proporcionados no son validos para solicitar el codigo."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Debe proporcionar el usuario o correo registrado y no debe exceder 50 caracteres."]
                    = () => LangResources.Lang.errorTextoIdentificadorRecuperacionRequerido,
                ["No se encontro una cuenta con el usuario o correo proporcionado."]
                    = () => LangResources.Lang.errorTextoCuentaNoRegistrada,
                ["No se encontro una solicitud de recuperacion activa."]
                    = () => LangResources.Lang.errorTextoSolicitudRecuperacionActiva,
                ["El codigo de verificacion ha expirado. Solicite uno nuevo."]
                    = () => LangResources.Lang.errorTextoCodigoExpiradoSolicitarNuevo,
                ["No hay una solicitud de recuperacion vigente."]
                    = () => LangResources.Lang.errorTextoSolicitudRecuperacionVigente,
                ["La solicitud de recuperacion no es valida."]
                    = () => LangResources.Lang.errorTextoSolicitudRecuperacionInvalida,
                ["No fue posible completar el registro. Por favor, intente nuevamente."]
                    = () => LangResources.Lang.errorTextoRegistrarCuentaMasTarde,
                ["No fue posible iniciar sesion. Por favor, intente nuevamente."]
                    = () => LangResources.Lang.errorTextoServidorInicioSesion,
                ["Usuario o contrasena incorrectos."]
                    = () => LangResources.Lang.errorTextoCredencialesIncorrectas,
                ["Las credenciales proporcionadas no son validas."]
                    = () => LangResources.Lang.errorTextoCredencialesIncorrectas,
                ["El correo electronico es obligatorio, debe tener un formato valido y no debe exceder 50 caracteres."]
                    = () => LangResources.Lang.errorTextoCorreoInvalido,
                ["El nombre de usuario es obligatorio y no debe exceder 50 caracteres."]
                    = () => LangResources.Lang.errorTextoIdentificadorUsuarioInvalido,
                ["El nombre es obligatorio y no debe exceder 50 caracteres."]
                    = () => LangResources.Lang.errorTextoNombreObligatorioLongitud,
                ["El apellido es obligatorio y no debe exceder 50 caracteres."]
                    = () => LangResources.Lang.errorTextoApellidoObligatorioLongitud,
                ["No se encontro el usuario especificado."]
                    = () => LangResources.Lang.errorTextoUsuarioNoEncontrado,
                ["No se encontro la informacion del jugador."]
                    = () => LangResources.Lang.errorTextoJugadorNoExiste,
                ["No existe un jugador asociado al usuario especificado."]
                    = () => LangResources.Lang.errorTextoJugadorNoExiste,
                ["El avatar seleccionado no es valido."]
                    = () => LangResources.Lang.errorTextoSeleccionAvatarValido,
                ["No fue posible obtener la informacion del perfil."]
                    = () => LangResources.Lang.errorTextoServidorObtenerPerfil,
                ["No fue posible actualizar el perfil. Por favor, intente nuevamente."]
                    = () => LangResources.Lang.errorTextoActualizarPerfil,
                ["Perfil actualizado correctamente."]
                    = () => LangResources.Lang.avisoTextoPerfilActualizado,
                ["No fue posible recuperar las solicitudes de amistad."]
                    = () => LangResources.Lang.amigosErrorRecuperarSolicitudes,
                ["No es posible enviarse una solicitud de amistad a si mismo."]
                    = () => LangResources.Lang.amigosErrorAutoSolicitud,
                ["Alguno de los usuarios especificados no existe."]
                    = () => LangResources.Lang.amigosErrorUsuarioNoExiste,
                ["Ya existe una solicitud o relacion de amistad entre los usuarios."]
                    = () => LangResources.Lang.amigosErrorRelacionExiste,
                ["No fue posible enviar la solicitud de amistad."]
                    = () => LangResources.Lang.amigosErrorCompletarSolicitud,
                ["No fue posible actualizar la solicitud de amistad."]
                    = () => LangResources.Lang.amigosErrorActualizarSolicitud,
                ["No existe una solicitud de amistad entre los usuarios."]
                    = () => LangResources.Lang.amigosErrorSolicitudNoExiste,
                ["No fue posible aceptar la solicitud de amistad."]
                    = () => LangResources.Lang.amigosErrorAceptarSolicitud,
                ["La solicitud de amistad ya fue aceptada con anterioridad."]
                    = () => LangResources.Lang.amigosErrorSolicitudAceptada,
                ["No existe una relacion de amistad entre los usuarios."]
                    = () => LangResources.Lang.amigosErrorRelacionNoExiste,
                ["No fue posible eliminar la relacion de amistad."]
                    = () => LangResources.Lang.amigosErrorEliminarRelacion,
                ["No fue posible recuperar la lista de amigos."]
                    = () => LangResources.Lang.amigosErrorRecuperarSolicitudes,
                ["La invitacion no es valida."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Los datos de la invitacion no son validos."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El correo electronico no es valido."]
                    = () => LangResources.Lang.errorTextoCorreoInvalido,
                ["No fue posible enviar la invitacion."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Ocurrio un problema al procesar la invitacion."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Ocurrio un error al enviar la invitacion."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Invitacion enviada correctamente."]
                    = () => LangResources.Lang.invitarCorreoTextoEnviado,
                ["El jugador con el correo ingresado ya esta en la sala."]
                    = () => LangResources.Lang.invitarCorreoTextoJugadorYaEnSala,
                ["No fue posible enviar la invitacion por correo electronico."]
                    = () => LangResources.Lang.errorTextoEnviarCorreo,
                ["No se encontro la sala especificada."]
                    = () => LangResources.Lang.errorTextoNoEncuentraPartida,
                ["La sala esta llena."]
                    = () => LangResources.Lang.errorTextoSalaLlena,
                ["No fue posible crear la sala."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Ocurrio un error al unirse a la sala."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Ocurrio un error al abandonar la sala."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Ocurrio un error al expulsar al jugador."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Ocurrio un error al suscribirse a las salas."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["No fue posible generar un codigo para la sala."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Solo el creador de la sala puede expulsar jugadores."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El creador de la sala no puede ser expulsado."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El jugador especificado no esta en la sala."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El jugador ya esta en la sala."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Los datos proporcionados no son validos. Por favor, verifique la informacion."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Ocurrio un error inesperado. Por favor, intente nuevamente."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["No se encontro una cuenta con los datos proporcionados."]
                    = () => LangResources.Lang.errorTextoCuentaNoRegistrada,
                ["La cuenta no ha sido verificada. Por favor, verifique su correo."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["La operacion se completo correctamente."]
                    = () => LangResources.Lang.avisoTextoPerfilActualizado,
                ["La solicitud de invitacion no es valida."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["No fue posible confirmar el codigo de verificacion."]
                    = () => LangResources.Lang.errorTextoServidorValidarCodigo,
                ["No fue posible establecer el contexto de la operacion."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["No fue posible establecer el contexto para amigos."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["No fue posible establecer la conexion con el servidor."]
                    = () => LangResources.Lang.errorTextoServidorNoDisponible,
                ["No fue posible establecer la conexion para amigos."]
                    = () => LangResources.Lang.errorTextoServidorNoDisponible,
                ["No fue posible notificar la actualizacion de la solicitud de amistad."]
                    = () => LangResources.Lang.amigosErrorActualizarSolicitud,
                ["No fue posible notificar la eliminacion de la relacion de amistad."]
                    = () => LangResources.Lang.amigosErrorEliminarRelacion,
                ["No fue posible suscribirse a las actualizaciones de amigos."]
                    = () => LangResources.Lang.amigosErrorRecuperarSolicitudes,
                ["No se encontraron todos los usuarios especificados."]
                    = () => LangResources.Lang.amigosErrorUsuarioNoExiste,
                ["Ocurrio un error al crear la sala."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El codigo de sala es obligatorio."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El idioma de las canciones es obligatorio."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El nombre de usuario es obligatorio para cancelar la suscripcion."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El nombre de usuario es obligatorio para suscribirse a las notificaciones."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El nombre de usuario es obligatorio."]
                    = () => LangResources.Lang.errorTextoIdentificadorUsuarioInvalido,
                ["El numero de rondas debe ser mayor a cero."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El parametro {0} es obligatorio."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["El tiempo por ronda debe ser mayor a cero."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["La configuracion de la partida es obligatoria."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["La contrasena debe tener entre 8 y 15 caracteres, incluir una letra mayuscula, un numero y un caracter especial."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["La dificultad es obligatoria."]
                    = () => LangResources.Lang.errorTextoErrorProcesarSolicitud,
                ["Tu cuenta ha sido suspendida por mala conducta."]
                    = () => LangResources.Lang.errorTextoUsuarioBaneado
            };

        /// <summary>
        /// Intenta traducir un mensaje del servidor usando el mapa o expresiones regulares.
        /// </summary>
        public static string Localizar(string mensaje, string mensajePredeterminado)
        {
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                string mensajeNormalizado = mensaje.Trim();

                if (TryLocalizarMensajeDinamico(mensajeNormalizado, out string mensajeTraducido))
                {
                    return mensajeTraducido;
                }

                if (MapaMensajes.TryGetValue(mensajeNormalizado, out Func<string> traductor))
                {
                    return traductor();
                }
            }

            if (!string.IsNullOrWhiteSpace(mensajePredeterminado))
            {
                return mensajePredeterminado;
            }

            return LangResources.Lang.errorTextoErrorProcesarSolicitud;
        }

        private static bool TryLocalizarMensajeDinamico(string mensaje, out string traducido)
        {
            Match espera = EsperaCodigoRegex.Match(mensaje);
            if (espera.Success)
            {
                traducido = string.Format(
                    CultureInfo.CurrentCulture,
                    LangResources.Lang.errorTextoTiempoEsperaCodigo,
                    espera.Groups[1].Value);
                return true;
            }

            Match identificador = IdentificadorRedSocialRegex.Match(mensaje);
            if (identificador.Success)
            {
                traducido = string.Format(
                    CultureInfo.CurrentCulture,
                    LangResources.Lang.errorTextoIdentificadorRedSocialLongitud,
                    identificador.Groups[1].Value,
                    identificador.Groups[2].Value);
                return true;
            }

            traducido = null;
            return false;
        }
    }
}