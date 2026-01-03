namespace PictionaryMusicalServidor.Servicios.Servicios.Constantes
{
    /// <summary>
    /// Mensajes de error centralizados para evitar repeticion de cadenas de texto.
    /// Separados en mensajes para el cliente (informativos) y mensajes para el log (detallados).
    /// </summary>
    internal static class MensajesError
    {
        /// <summary>
        /// Mensajes informativos para el cliente.
        /// Deben ser claros y utiles sin exponer detalles tecnicos internos.
        /// </summary>
        public static class Cliente
        {
            public const string ErrorInesperado = 
                "Ocurrio un error inesperado. Por favor, intente mas tarde.";
            public const string OperacionExitosa = 
                "La operacion se completo correctamente.";
            public const string ErrorOperacionInvalida = 
                "No se puede realizar esta accion en este momento. Intente mas tarde.";

            public const string ErrorCrearReporte = 
                "No se pudo enviar el reporte. Intente mas tarde.";
            public const string ReporteRegistrado = 
                "Reporte enviado correctamente.";
            public const string ReporteDuplicado = 
                "Ya reportaste a este jugador anteriormente.";
            public const string ReporteMotivoObligatorio = 
                "Por favor, escribe el motivo del reporte.";
            public const string ReporteMismoUsuario = 
                "No puedes reportarte a ti mismo.";
            public const string ReporteMotivoLongitud = 
                "El motivo del reporte es muy largo. Usa maximo 100 caracteres.";

            public const string DatosInvalidos = 
                "Algunos datos no son correctos. Por favor, revisa la informacion ingresada.";
            public const string CredencialesInvalidas = 
                "Usuario o contrasena incorrectos.";
            public const string CredencialesIncorrectas = 
                "Usuario o contrasena incorrectos.";

            public const string UsuarioNoEncontrado = 
                "No encontramos al usuario. Verifica que el nombre sea correcto.";
            public const string UsuariosNoEncontrados = 
                "No encontramos a uno o mas usuarios especificados.";
            public const string JugadorNoEncontrado = 
                "No encontramos la informacion del jugador.";
            public const string UsuariosEspecificadosNoExisten = 
                "El usuario que buscas no existe. Verifica el nombre e intenta de nuevo.";
            public const string JugadorNoAsociado = 
                "No encontramos un jugador asociado a este usuario.";
            public const string CuentaNoVerificada = 
                "Tu cuenta aun no ha sido verificada. Revisa tu correo electronico.";
            public const string CuentaNoEncontrada = 
                "No encontramos una cuenta con estos datos. Verifica la informacion.";
            public const string AvatarInvalido = 
                "Por favor, selecciona un avatar valido.";

            public const string ErrorRegistrarCuenta = 
                "No pudimos completar tu registro. Por favor, intenta mas tarde.";
            public const string ErrorInicioSesion = 
                "No pudimos iniciar sesion. Por favor, intenta mas tarde.";
            public const string UsuarioRegistroInvalido = 
                "El nombre de usuario es obligatorio y debe tener maximo 50 caracteres.";
            public const string NombreRegistroInvalido = 
                "El nombre es obligatorio y debe tener maximo 50 caracteres.";
            public const string ApellidoRegistroInvalido = 
                "El apellido es obligatorio y debe tener maximo 50 caracteres.";
            public const string CorreoRegistroInvalido = 
                "Ingresa un correo electronico valido (maximo 50 caracteres).";
            public const string ContrasenaRegistroInvalida = 
                "La contrasena debe tener entre 8 y 15 caracteres, incluir una mayuscula, " +
                "un numero y un caracter especial.";

            public const string ErrorObtenerPerfil = 
                "No pudimos cargar tu perfil. Intenta mas tarde.";
            public const string ErrorActualizarPerfil = 
                "No pudimos guardar los cambios de tu perfil. Intenta mas tarde.";
            public const string PerfilActualizadoExito = 
                "Tu perfil se actualizo correctamente.";

            public const string ErrorRecuperarSolicitudes = 
                "No pudimos cargar tus solicitudes de amistad. Intenta mas tarde.";
            public const string ErrorAlmacenarSolicitud = 
                "No pudimos enviar la solicitud de amistad. Intenta mas tarde.";
            public const string ErrorActualizarSolicitud = 
                "No pudimos procesar la solicitud de amistad. Intenta mas tarde.";
            public const string ErrorEliminarAmistad = 
                "No pudimos eliminar a este amigo. Intenta mas tarde.";
            public const string ErrorRecuperarListaAmigos = 
                "No pudimos cargar tu lista de amigos. Intenta mas tarde.";
            public const string ErrorSuscripcionAmigos = 
                "No pudimos cargar tus amigos debido a un problema del servidor.";
            public const string ErrorNotificarSolicitud = 
                "No pudimos enviar la notificacion de amistad. Intenta mas tarde.";
            public const string ErrorNotificarEliminacion = 
                "No pudimos notificar la eliminacion de amistad.";
            public const string SolicitudAmistadMismoUsuario = 
                "No puedes enviarte una solicitud de amistad a ti mismo.";
            public const string RelacionAmistadExistente = 
                "Ya tienes una solicitud o amistad con este usuario.";
            public const string SolicitudAmistadNoExiste = 
                "No existe una solicitud de amistad con este usuario.";
            public const string ErrorAceptarSolicitud = 
                "No pudimos aceptar la solicitud de amistad. Intenta mas tarde.";
            public const string SolicitudAmistadYaAceptada = 
                "Esta solicitud de amistad ya fue aceptada.";
            public const string RelacionAmistadNoExiste = 
                "No tienes una amistad con este usuario.";

            public const string ErrorRecuperarCuenta = 
                "No pudimos procesar tu solicitud de recuperacion. Intenta mas tarde.";
            public const string ErrorBaseDatosRecuperacion = 
                "No pudimos conectar con el servidor. Verifica tu conexion e intenta mas tarde.";
            public const string ErrorReenviarCodigo = 
                "No pudimos reenviar el codigo. Intenta mas tarde.";
            public const string ErrorConfirmarCodigo = 
                "No pudimos confirmar el codigo. Intenta mas tarde.";
            public const string ErrorActualizarContrasena = 
                "No pudimos actualizar tu contrasena. Intenta mas tarde.";
            public const string DatosRecuperacionInvalidos = 
                "Los datos ingresados no son correctos. Por favor, revisalos.";
            public const string DatosReenvioCodigo = 
                "Los datos ingresados no son correctos para reenviar el codigo.";
            public const string DatosConfirmacionInvalidos = 
                "Los datos de confirmacion no son correctos. Revisalos.";
            public const string DatosActualizacionContrasena = 
                "Los datos para actualizar la contrasena no son correctos.";
            public const string ErrorConfirmarCodigoRecuperacion = 
                "No pudimos confirmar el codigo de recuperacion. Intenta mas tarde.";
            public const string DatosSolicitudVerificacionInvalidos = 
                "Los datos ingresados no son correctos para solicitar el codigo.";
            public const string SolicitudRecuperacionIdentificadorObligatorio = 
                "Ingresa tu usuario o correo (maximo 50 caracteres).";
            public const string SolicitudRecuperacionCuentaNoEncontrada = 
                "No encontramos una cuenta con ese usuario o correo.";
            public const string SolicitudRecuperacionNoEncontrada = 
                "No tienes una solicitud de recuperacion activa. Inicia el proceso de nuevo.";
            public const string CodigoRecuperacionExpirado = 
                "El codigo ha expirado. Solicita uno nuevo.";
            public const string ErrorReenviarCodigoRecuperacion = 
                "No pudimos reenviar el codigo de recuperacion. Intenta mas tarde.";
            public const string CodigoRecuperacionIncorrecto = 
                "El codigo ingresado no es correcto. Verifica e intenta de nuevo.";
            public const string SolicitudRecuperacionNoVigente = 
                "No tienes una solicitud de recuperacion activa.";
            public const string SolicitudRecuperacionInvalida = 
                "La solicitud de recuperacion ya no es valida. Inicia el proceso de nuevo.";

            public const string ErrorSolicitudVerificacion = 
                "No pudimos procesar tu solicitud de verificacion. Intenta mas tarde.";
            public const string ErrorReenviarCodigoVerificacion = 
                "No pudimos reenviar el codigo de verificacion. Intenta mas tarde.";
            public const string SolicitudVerificacionNoEncontrada = 
                "No tienes una solicitud de verificacion activa. Registrate de nuevo.";
            public const string CodigoVerificacionExpirado = 
                "El codigo de verificacion ha expirado. Registrate de nuevo.";
            public const string CodigoVerificacionIncorrecto = 
                "El codigo ingresado no es correcto. Verifica e intenta de nuevo.";
            public const string UsuarioOCorreoRegistrado = 
                "El correo o usuario ya esta en uso. Intenta con otro.";

            public const string UsuarioBaneadoPorReportes = 
                "Tu cuenta ha sido suspendida por mala conducta.";

            public const string ErrorCrearSala = 
                "No pudimos crear la sala. Intenta mas tarde.";
            public const string ErrorInesperadoCrearSala = 
                "Ocurrio un problema al crear la sala. Intenta mas tarde.";
            public const string ErrorInesperadoUnirse = 
                "No pudimos unirte a la sala. Intenta mas tarde.";
            public const string ErrorInesperadoAbandonar = 
                "Ocurrio un problema al salir de la sala.";
            public const string ErrorInesperadoExpulsar = 
                "No pudimos expulsar al jugador. Intenta mas tarde.";
            public const string ErrorInesperadoSuscripcion = 
                "No pudimos conectarte a las salas. Intenta mas tarde.";
            public const string SalaNoEncontrada = 
                "La sala ya no existe o el codigo es incorrecto.";
            public const string ErrorGenerarCodigo = 
                "No pudimos generar el codigo de la sala. Intenta mas tarde.";
            public const string SalaLlena = 
                "La sala esta llena. Intenta con otra sala.";
            public const string SalaExpulsionRestringida = 
                "Solo el creador de la sala puede expulsar jugadores.";
            public const string SalaCreadorNoExpulsable = 
                "El creador de la sala no puede ser expulsado.";
            public const string SalaJugadorNoExiste = 
                "El jugador ya no esta en la sala.";

            public const string InvitacionInvalida = 
                "La invitacion no es valida o ha expirado.";
            public const string DatosInvitacionInvalidos = 
                "Los datos de la invitacion no son correctos.";
            public const string CorreoInvalido = 
                "Ingresa un correo electronico valido.";
            public const string ErrorEnviarInvitacion = 
                "No pudimos enviar la invitacion. Intenta mas tarde.";
            public const string ErrorProcesarInvitacion = 
                "Hubo un problema al procesar la invitacion.";
            public const string ErrorInesperadoInvitacion = 
                "No pudimos enviar la invitacion. Intenta mas tarde.";
            public const string JugadorYaEnSala = 
                "Este jugador ya esta en la sala.";
            public const string InvitacionEnviadaExito = 
                "Invitacion enviada correctamente.";
            public const string SolicitudInvitacionInvalida = 
                "La invitacion no es valida.";
            public const string CorreoJugadorEnSala = 
                "El jugador con este correo ya esta en la sala.";
            public const string ErrorEnviarInvitacionCorreo = 
                "No pudimos enviar la invitacion al correo. Intenta mas tarde.";

            public const string ErrorObtenerCallback = 
                "No pudimos conectarte al servidor. Verifica tu conexion e intenta de nuevo.";
            public const string ErrorObtenerCallbackAmigos = 
                "No pudimos conectarte al servicio de amigos. Intenta mas tarde.";
            public const string ErrorContextoOperacion = 
                "Ocurrio un problema de conexion. Intenta de nuevo.";
            public const string ErrorContextoOperacionAmigos = 
                "Ocurrio un problema de conexion con el servicio de amigos.";

            public const string ParametroObligatorio = 
                "Falta informacion requerida: {0}.";
            public const string NombreUsuarioObligatorio = 
                "Ingresa tu nombre de usuario.";
            public const string NombreUsuarioObligatorioCancelar = 
                "Se requiere el nombre de usuario para cancelar.";
            public const string NombreUsuarioObligatorioSuscripcion = 
                "Se requiere el nombre de usuario para conectarte.";
            public const string CodigoSalaObligatorio = 
                "Ingresa el codigo de la sala.";
            public const string IdJugadorObligatorio = 
                "No se pudo identificar al jugador.";
            public const string IdSalaObligatorio = 
                "No se pudo identificar la sala.";
            public const string DatosSuscripcionObligatorios = 
                "Se requieren los datos de suscripcion para unirse a la partida.";

            public const string ConfiguracionObligatoria = 
                "Configura la partida antes de iniciar.";
            public const string NumeroRondasInvalido = 
                "El numero de rondas debe ser mayor a cero.";
            public const string TiempoRondaInvalido = 
                "El tiempo por ronda debe ser mayor a cero.";
            public const string IdiomaObligatorio = 
                "Selecciona el idioma de las canciones.";
            public const string DificultadObligatoria = 
                "Selecciona la dificultad.";

            public const string PartidaCanceladaFaltaJugadores = 
                "La partida se cancelo porque ya no hay suficientes jugadores.";
            public const string PartidaCanceladaHostSalio = 
                "La partida se cancelo porque el anfitrion abandono.";
            public const string PartidaYaIniciada = 
                "La partida ya esta en curso.";
            public const string PartidaComenzo = 
                "La partida ya comenzo, no puedes unirte.";
            public const string FaltanJugadores = 
                "Espera a que se unan mas jugadores para iniciar.";
            public const string SoloHost = 
                "Solo el anfitrion puede iniciar la partida.";

            public const string MensajeSuperaLimiteCaracteres = 
                "Tu mensaje es muy largo. Usa menos de 200 caracteres.";
            public const string ErrorEnviarMensaje = 
                "No pudimos enviar el mensaje. Intenta de nuevo.";
            public const string ErrorUnirseChat = 
                "No pudimos conectarte al chat de la sala. Intenta de nuevo.";
            public const string ErrorSalirChat = 
                "Ocurrio un problema al salir del chat.";
        }

        /// <summary>
        /// Mensajes para registro de errores en bitacora.
        /// Contienen detalles tecnicos para depuracion sin datos personales.
        /// Solo se permiten IDs de usuario, nunca nombres, correos u otros datos personales.
        /// </summary>
        public static class Bitacora
        {
            public const string ErrorNotificandoInicioRonda = 
                "Error notificando inicio de ronda al jugador con id {0}.";
            public const string ErrorActualizarClasificaciones = 
                "Error inesperado al actualizar clasificaciones.";
            public const string ErrorObtenerJugadoresClasificacion = 
                "Error al obtener jugadores para actualizar clasificacion.";
            public const string ErrorActualizarClasificacionJugador = 
                "No se pudo actualizar clasificacion del jugador con id {0}.";
            public const string ErrorObtenerConfiguracionSala = 
                "No se pudo obtener configuracion de sala. Se usara la configuracion por defecto.";
            public const string ErrorDatosObtenerJugadoresClasificacion = 
                "Error de datos al obtener jugadores para clasificacion.";
            public const string ErrorDatosActualizarClasificaciones = 
                "Error de datos al actualizar clasificaciones de partida.";
            public const string PartidaFinalizadaSinClasificacion = 
                "Partida finalizada sin clasificacion por mensaje de error.";
            public const string SalaCanceladaSalidaAnfitrion = 
                "Sala cancelada por salida del anfitrion.";

            public const string ErrorSuscripcionAmigos = 
                "Error al suscribir a notificaciones de amistad.";
            public const string ErrorEnviarSolicitudAmistad = 
                "Error al enviar solicitud de amistad.";
            public const string ErrorResponderSolicitudAmistad = 
                "Error al responder solicitud de amistad.";
            public const string ErrorEliminarAmistad = 
                "Error al eliminar relacion de amistad.";
            public const string IntentarEnviarSolicitudUsuarioInexistente = 
                "Intento de enviar solicitud a usuario inexistente.";
            public const string ValidacionFallidaEnvioSolicitud = 
                "Error de validacion al enviar solicitud de amistad.";
            public const string ReglaNegocioVioladaSolicitud = 
                "Regla de negocio violada al enviar solicitud de amistad.";
            public const string DatosInvalidosSolicitud = 
                "Datos invalidos al enviar solicitud de amistad.";
            public const string ReglaNegocioVioladaAceptar = 
                "Regla de negocio violada al aceptar solicitud de amistad.";
            public const string DatosInvalidosAceptar = 
                "Datos invalidos al aceptar solicitud de amistad.";
            public const string ReglaNegocioVioladaEliminar = 
                "Regla de negocio violada al eliminar amistad.";
            public const string ErrorObtenerListaAmigosNotificacion = 
                "No se pudo obtener la lista de amigos del usuario para notificar.";
            public const string ErrorBaseDatosListaAmigos = 
                "Error de base de datos al obtener la lista de amigos del usuario.";
            public const string DatosInvalidosActualizarListaAmigos = 
                "Datos invalidos al actualizar la lista de amigos del usuario.";
            public const string ErrorInesperadoListaAmigos = 
                "Error inesperado al obtener la lista de amigos del usuario.";
            public const string OperacionInvalidaListaAmigos = 
                "Operacion invalida al obtener la lista de amigos del usuario.";
            public const string ErrorNotificarListaAmigosActualizada = 
                "Error al notificar lista de amigos actualizada.";
            public const string ErrorNotificarSolicitudActualizada = 
                "Error al notificar solicitud actualizada.";

            public const string ErrorConcurrenciaCrearSala = 
                "Error de concurrencia al crear sala con codigo {0}.";
            public const string SalaCreadaExito = 
                "Sala creada exitosamente con codigo {0}.";
            public const string UsuarioUnidoSala = 
                "Usuario unido exitosamente a sala con codigo {0}.";
            public const string ErrorValidacionCrearSala = 
                "Operacion invalida al crear sala.";
            public const string ErrorComunicacionCrearSala = 
                "Error de comunicacion WCF al crear sala.";
            public const string ErrorTimeoutCrearSala = 
                "Timeout al crear sala.";
            public const string ErrorCanalCerradoCrearSala = 
                "Canal WCF cerrado al crear sala.";
            public const string ErrorInesperadoCrearSala = 
                "Error inesperado al crear sala.";
            public const string ErrorValidacionUnirse = 
                "Error de validacion al unirse a sala.";
            public const string ErrorOperacionUnirse = 
                "Operacion invalida al unirse a sala.";
            public const string ErrorComunicacionUnirse = 
                "Error de comunicacion WCF al unirse a sala.";
            public const string ErrorTimeoutUnirse = 
                "Timeout al unirse a la sala.";
            public const string ErrorCanalCerradoUnirse = 
                "Canal WCF cerrado al unirse a la sala.";
            public const string ErrorInesperadoUnirse = 
                "Error inesperado al unirse a la sala.";
            public const string ErrorValidacionAbandonar = 
                "Error de validacion al abandonar sala.";
            public const string ErrorOperacionAbandonar = 
                "Operacion invalida al abandonar sala.";
            public const string ErrorInesperadoAbandonar = 
                "Error inesperado al abandonar sala.";
            public const string ExpulsandoJugador = 
                "Expulsando jugador de sala con codigo {0}.";
            public const string JugadorExpulsadoExito = 
                "Jugador expulsado exitosamente de sala con codigo {0}.";
            public const string ErrorValidacionExpulsar = 
                "Error de validacion al expulsar jugador de sala.";
            public const string ErrorOperacionExpulsar = 
                "Operacion invalida al expulsar jugador.";
            public const string ErrorInesperadoExpulsar = 
                "Error inesperado al expulsar jugador.";
            public const string ErrorSuscripcionListaSalas = 
                "Operacion invalida al suscribirse a lista de salas.";
            public const string ErrorComunicacionSuscripcion = 
                "Error de comunicacion WCF al suscribirse a lista de salas.";
            public const string ErrorTimeoutSuscripcion = 
                "Timeout al suscribirse a la lista de salas.";
            public const string ErrorInesperadoSuscripcion = 
                "Error inesperado al suscribirse a la lista de salas.";
            public const string ErrorCancelarSuscripcion = 
                "Error al cancelar suscripcion.";
            public const string ErrorGenerarCodigoSala = 
                "No se pudo generar codigo unico de sala.";
            public const string ErrorOperacionInvalidaObtenerSalas = 
                "Operacion invalida al obtener lista de salas.";
            public const string ErrorInesperadoObtenerSalas = 
                "Error inesperado al obtener lista de salas.";

            public const string ErrorComunicacionNotificarSalas = 
                "Error de comunicacion al notificar la lista de salas a los suscriptores.";
            public const string ErrorTimeoutNotificarSalas = 
                "Timeout al notificar la lista de salas a los suscriptores.";
            public const string ErrorCanalCerradoNotificarSalas = 
                "Canal cerrado al notificar la lista de salas a los suscriptores.";
            public const string ErrorInesperadoNotificarSalas = 
                "Error inesperado al notificar la lista de salas a los suscriptores.";
            public const string ErrorComunicacionMasivaNotificarSalas = 
                "Error de comunicacion al notificar masivamente. " +
                "Eliminando suscripcion defectuosa.";
            public const string ErrorTimeoutMasivoNotificarSalas = 
                "Timeout al notificar masivamente lista de salas. " +
                "Eliminando suscripcion defectuosa.";
            public const string ErrorCanalCerradoMasivoNotificarSalas = 
                "Canal cerrado al notificar masivamente lista de salas.";
            public const string ErrorInesperadoMasivoNotificarSalas = 
                "Error inesperado al notificar masivamente lista de salas.";
            public const string ErrorComunicacionNotificarSuscriptor = 
                "Error de comunicacion al notificar al suscriptor.";
            public const string ErrorTimeoutNotificarSuscriptor = 
                "Timeout al notificar al suscriptor.";
            public const string ErrorCanalCerradoNotificarSuscriptor = 
                "Canal cerrado al notificar al suscriptor.";
            public const string ErrorInesperadoNotificarSuscriptor = 
                "Error inesperado al notificar al suscriptor.";

            public const string ErrorValidacionInvitacion = 
                "Error de validacion al enviar invitacion.";
            public const string DatosInvalidosInvitacion = 
                "Datos invalidos al enviar invitacion.";
            public const string OperacionInvalidaInvitacion = 
                "Operacion invalida al enviar invitacion.";
            public const string ErrorBaseDatosInvitacion = 
                "Error de base de datos al enviar invitacion.";
            public const string ErrorDatosInvitacion = 
                "Error de datos al enviar invitacion.";
            public const string ErrorTimeoutValidacionCorreo = 
                "Timeout al validar el formato del correo de invitacion.";
            public const string ErrorInesperadoInvitacion = 
                "Error inesperado al enviar invitacion.";

            public const string ErrorValidacionChat = 
                "Error de validacion en operacion de chat.";
            public const string DatosInvalidosChat = 
                "Datos invalidos en operacion de chat.";
            public const string ErrorComunicacionChat = 
                "Error de comunicacion en chat.";
            public const string ErrorTimeoutChat = 
                "Tiempo de espera agotado en chat.";
            public const string ErrorCanalCerradoChat = 
                "Canal WCF cerrado en chat.";
            public const string ErrorInesperadoChat = 
                "Error inesperado en chat.";

            public const string JugadorSuscritoPartida = 
                "Jugador con id {0} suscrito para partida en sala {1}.";
            public const string InicioPartidaSolicitado = 
                "Inicio de partida solicitado para sala {0} por jugador con id {1}.";

            public const string CodigoVerificacionGenerado = 
                "Codigo de verificacion de registro generado correctamente.";
            public const string VerificacionConfirmadaExitosamente = 
                "Verificacion confirmada exitosamente.";
            public const string RegistroDuplicadoIntentado = 
                "Registro duplicado intentado (usuario o correo existente).";
            public const string ErrorEnviarCodigoVerificacion = 
                "Error al enviar codigo de verificacion.";
            public const string TokenNoEncontradoExpirado = 
                "Token no encontrado o expirado en cache.";
            public const string ErrorReenviarCodigoVerificacion = 
                "Error al reenviar codigo de verificacion.";

            public const string ErrorEnviarCorreoRecuperacion = 
                "Fallo critico al enviar correo de recuperacion.";
            public const string ErrorReenviarCorreoRecuperacion = 
                "Fallo critico al reenviar correo de recuperacion.";
            public const string ErrorActualizarContrasena = 
                "Error al actualizar contrasena.";
            public const string ErrorDatosActualizarContrasena = 
                "Error de datos al actualizar contrasena.";
            public const string ErrorInesperadoActualizarContrasena = 
                "Error inesperado al actualizar contrasena.";

            public const string ArgumentoInvalidoObtenerPerfil = 
                "Argumento invalido al obtener perfil.";
            public const string OperacionInvalidaObtenerPerfil = 
                "Operacion invalida al obtener perfil.";
            public const string ErrorActualizacionObtenerPerfil = 
                "Error de actualizacion al obtener perfil.";
            public const string ErrorBaseDatosObtenerPerfil = 
                "Error de base de datos al obtener perfil.";
            public const string ErrorDatosObtenerPerfil = 
                "Error de datos al obtener perfil.";
            public const string ErrorInesperadoObtenerPerfil = 
                "Error inesperado al obtener perfil.";
            public const string PerfilActualizadoExitosamente = 
                "Perfil actualizado exitosamente.";
            public const string ArgumentoInvalidoActualizarPerfil = 
                "Argumento invalido al actualizar perfil.";
            public const string OperacionInvalidaActualizarPerfil = 
                "Operacion invalida al actualizar perfil.";
            public const string ValidacionEntidadActualizarPerfil = 
                "Validacion de entidad fallida al actualizar perfil.";
            public const string ErrorConcurrenciaActualizarPerfil = 
                "Error de concurrencia al actualizar perfil.";
            public const string ErrorActualizacionBDActualizarPerfil = 
                "Error de actualizacion de BD al actualizar perfil.";
            public const string ErrorBaseDatosActualizarPerfil = 
                "Error de base de datos al actualizar perfil.";
            public const string ErrorDatosActualizarPerfil = 
                "Error de datos al actualizar perfil.";
            public const string ErrorInesperadoActualizarPerfil = 
                "Error inesperado al actualizar perfil.";

            public const string ValidacionFallidaReporte = 
                "Validacion fallida al registrar reporte.";
            public const string DatosInvalidosReporte = 
                "Datos invalidos al registrar reporte.";
            public const string OperacionInvalidaReporte = 
                "Operacion invalida al registrar reporte.";
            public const string UsuariosNoEncontradosReporte = 
                "No se encontraron usuarios al registrar reporte.";
            public const string ErrorBaseDatosReporte = 
                "Error de base de datos al registrar reporte.";
            public const string ErrorDatosReporte = 
                "Error de datos al registrar reporte.";
            public const string ErrorInesperadoReporte = 
                "Error inesperado al registrar reporte.";
            public const string ReporteUsuariosNoRegistrados = 
                "Intento de reporte con usuarios no registrados.";
            public const string ErrorObtenerIdUsuariosReporte = 
                "Error inesperado al obtener identificadores de usuarios para reporte.";

            public const string OperacionInvalidaComunicacionWCF = 
                "Operacion invalida en comunicacion WCF.";
            public const string ErrorConstruirContextoBaseDatos = 
                "Error al construir el contexto de base de datos.";
            public const string ErrorInesperadoCrearContexto = 
                "Error inesperado al crear el contexto de base de datos.";
            public const string CadenaConexionVacia = 
                "La cadena de conexion obtenida esta vacia.";
            public const string ErrorInesperadoObtenerJugadoresClasificacion = 
                "Error inesperado al obtener jugadores para clasificacion.";
            public const string ErrorInesperadoActualizarClasificaciones = 
                "Error inesperado al actualizar clasificaciones de partida.";
            public const string IdentificadorInvalidoListaAmigos = 
                "Identificador invalido al actualizar la lista de amigos del usuario.";
            public const string ErrorDatosObtenerAmigos = 
                "Error de datos al obtener lista de amigos. " +
                "Fallo en la consulta de amigos del usuario.";

            public const string ErrorNotificarCallback = 
                "Error al notificar callback de partida.";

            public const string ErrorComunicacionNotificarClienteSala = 
                "Error de comunicacion al notificar cliente en sala.";
            public const string ErrorTimeoutNotificarClienteSala = 
                "Tiempo de espera agotado al notificar cliente en sala.";
            public const string ErrorCanalCerradoNotificarClienteSala = 
                "Canal WCF cerrado al ejecutar notificacion en sala.";
            public const string ErrorInesperadoNotificarClienteSala = 
                "Error inesperado al ejecutar notificacion WCF en sala.";

            public const string ErrorNotificarAmistadEliminada = 
                "Error al notificar amistad eliminada.";

            public const string ErrorBaseDatos = 
                "Error de base de datos.";
            public const string ErrorDatos = 
                "Error de datos.";
            public const string ErrorInesperado = 
                "Error inesperado.";

            public const string ConfiguracionCorreoInvalida = 
                "La configuracion de correo es invalida o esta incompleta.";
            public const string ErrorSmtpEnviarCorreo = 
                "Error SMTP al enviar correo electronico.";
            public const string OperacionInvalidaEnviarCorreo = 
                "Operacion invalida al enviar correo.";
            public const string ArgumentosInvalidosCorreo = 
                "Argumentos invalidos para enviar correo.";
            public const string FormatoCorreoInvalido = 
                "Formato de correo invalido al enviar invitacion.";
            public const string ErrorCriticoEnviarNotificacionCodigo = 
                "Error critico al enviar notificacion de codigo.";

            public const string IdentificadorInvalidoSuscripcionListaAmigos = 
                "Identificador invalido al suscribirse a la lista de amigos.";
            public const string DatosInvalidosSuscripcionListaAmigos = 
                "Datos invalidos al suscribirse a la lista de amigos.";
            public const string DatosInvalidosCancelarSuscripcion = 
                "Datos invalidos al cancelar suscripcion.";
            public const string ErrorInesperadoCancelarSuscripcion = 
                "Error inesperado al cancelar suscripcion.";
            public const string DatosInvalidosObtenerListaAmigos = 
                "Datos invalidos al obtener la lista de amigos.";
            public const string ErrorActualizacionBDObtenerListaAmigos = 
                "Error de actualizacion de BD al obtener lista de amigos.";
            public const string ErrorBaseDatosObtenerListaAmigos = 
                "Error de base de datos al obtener lista de amigos.";
            public const string ErrorDatosObtenerListaAmigos = 
                "Error de datos al obtener lista de amigos.";
            public const string ErrorInesperadoObtenerListaAmigos = 
                "Error inesperado al obtener lista de amigos.";

            public const string IntentoInicioSesionFormatoInvalido = 
                "Intento de inicio de sesion con formato de datos invalido.";
            public const string ErrorBaseDatosInicioSesion = 
                "Error de base de datos durante el inicio de sesion.";
            public const string ErrorDatosInicioSesion = 
                "Error de datos durante el inicio de sesion.";
            public const string OperacionInvalidaInicioSesion = 
                "Operacion invalida durante el inicio de sesion.";
            public const string ErrorInesperadoInicioSesion = 
                "Error inesperado durante el inicio de sesion.";
            public const string InicioSesionUsuarioNoEncontrado = 
                "Inicio de sesion fallido. Usuario no encontrado.";
            public const string InicioSesionContrasenaIncorrecta = 
                "Inicio de sesion fallido. Contrasena incorrecta.";

            public const string ValidacionEntidadFallidaRegistro = 
                "Validacion de entidad fallida durante el registro.";
            public const string ErrorActualizacionBDRegistro = 
                "Error de actualizacion de BD durante el registro.";
            public const string ErrorBaseDatosRegistro = 
                "Error de base de datos durante el registro.";
            public const string ErrorDatosRegistro = 
                "Error de datos durante el registro.";
            public const string OperacionInvalidaRegistro = 
                "Operacion invalida durante el registro.";
            public const string ErrorInesperadoRegistro = 
                "Error inesperado durante el registro.";
            public const string IntentoRegistroSinVerificacion = 
                "Intento de registro sin verificacion confirmada.";
            public const string RegistroDuplicadoDetectado = 
                "Registro duplicado detectado (usuario o correo existente).";

            public const string ArgumentoNuloSolicitarCodigo = 
                "Argumento nulo al solicitar codigo de verificacion.";
            public const string ErrorDatosSolicitarCodigo = 
                "Error de datos al solicitar codigo de verificacion.";
            public const string ArgumentoNuloReenviarCodigo = 
                "Argumento nulo al reenviar codigo de verificacion.";
            public const string ErrorDatosReenviarCodigo = 
                "Error de datos al reenviar codigo de verificacion.";
            public const string ArgumentoNuloConfirmarCodigo = 
                "Argumento nulo al confirmar codigo de verificacion.";
            public const string ValidacionEntidadFallidaConfirmarCodigo = 
                "Validacion de entidad fallida al confirmar codigo.";
            public const string ErrorActualizacionBDConfirmarCodigo = 
                "Error de actualizacion de BD al confirmar codigo.";
            public const string ErrorBaseDatosConfirmarCodigo = 
                "Error de base de datos al confirmar codigo.";
            public const string ErrorDatosConfirmarCodigo = 
                "Error de datos al confirmar codigo.";
            public const string ArgumentoNuloSolicitarCodigoRecuperacion = 
                "Argumento nulo al solicitar codigo de recuperacion.";

            public const string ErrorBaseDatosObtenerClasificacion = 
                "Error de base de datos al obtener la clasificacion.";
            public const string ErrorDatosObtenerClasificacion = 
                "Error de datos al obtener la clasificacion.";
            public const string OperacionInvalidaObtenerClasificacion = 
                "Operacion invalida al obtener la clasificacion.";
            public const string ErrorInesperadoObtenerClasificacion = 
                "Error inesperado al obtener la clasificacion.";

            public const string ErrorBaseDatosSolicitarRecuperacion = 
                "Error de base de datos al solicitar codigo de recuperacion.";
            public const string ErrorBaseDatosBuscarUsuarioRecuperacion = 
                "Error de base de datos al buscar usuario para recuperacion.";
            public const string ErrorDatosSolicitarRecuperacion = 
                "Error de datos al solicitar codigo de recuperacion.";
            public const string ErrorInesperadoSolicitarRecuperacion = 
                "Error inesperado al solicitar codigo de recuperacion.";
            public const string ErrorBaseDatosReenviarRecuperacion = 
                "Error de base de datos al reenviar codigo de recuperacion.";
            public const string ErrorDatosReenviarRecuperacion = 
                "Error de datos al reenviar codigo de recuperacion.";
            public const string ErrorInesperadoReenviarRecuperacion = 
                "Error inesperado al reenviar codigo de recuperacion.";
            public const string ErrorBaseDatosConfirmarRecuperacion = 
                "Error de base de datos al confirmar codigo de recuperacion.";
            public const string ErrorDatosConfirmarRecuperacion = 
                "Error de datos al confirmar codigo de recuperacion.";
            public const string ErrorInesperadoConfirmarRecuperacion = 
                "Error inesperado al confirmar codigo de recuperacion.";
            public const string ValidacionEntidadActualizarContrasena = 
                "Validacion de entidad fallida al actualizar contrasena.";
            public const string ErrorActualizacionBDContrasena = 
                "Error de actualizacion de base de datos al actualizar contrasena.";
            public const string ErrorBaseDatosActualizarContrasena = 
                "Error de base de datos al actualizar contrasena.";
            public const string ErrorDatosActualizarContrasenaManejador = 
                "Error de datos al actualizar contrasena.";
            public const string ErrorInesperadoActualizarContrasenaManejador = 
                "Error inesperado al actualizar contrasena.";
            public const string ArgumentoNuloReenviarRecuperacion = 
                "Argumento nulo al reenviar codigo de recuperacion.";
            public const string ArgumentoNuloConfirmarRecuperacion = 
                "Argumento nulo al confirmar codigo de recuperacion.";
            public const string ArgumentoNuloActualizarContrasena = 
                "Argumento nulo al actualizar contrasena.";

            public const string ErrorDatosSuscripcionListaAmigos = 
                "Error de datos al suscribirse. Fallo recuperar lista de amigos.";
            public const string ErrorInesperadoSuscripcionListaAmigos = 
                "Error inesperado al suscribirse a lista de amigos.";
        }
    }
}
