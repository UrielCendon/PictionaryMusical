namespace PictionaryMusicalServidor.Datos.Constantes
{
    /// <summary>
    /// Clase estatica que contiene los mensajes de error utilizados en la capa de datos.
    /// Los mensajes de log solo deben incluir IDs, nunca datos personales como nombres o correos.
    /// </summary>
    public static class MensajesErrorDatos
    {
        /// <summary>
        /// Mensajes de error para operaciones con amigos.
        /// </summary>
        public static class Amigo
        {
            public const string ErrorVerificarRelacion = 
                "Error al verificar existencia de relacion de amistad.";
            public const string ErrorObtenerRelacion = 
                "Error al obtener la relacion de amistad.";
            public const string ErrorObtenerSolicitudesPendientes = 
                "Error al obtener solicitudes pendientes para el usuario con id {0}.";
            public const string ErrorActualizarEstado = 
                "Error al actualizar el estado de la relacion entre usuarios con ids {0} y {1}.";
            public const string ErrorEliminarRelacion = 
                "Error al eliminar la relacion de amistad entre usuarios con ids {0} y {1}.";
            public const string ErrorObtenerAmigos = 
                "Error al obtener amigos para el usuario con id {0}.";
            public const string IntentarActualizarRelacionNula = 
                "Se intento actualizar una relacion nula.";
            public const string IntentarEliminarRelacionNula = 
                "Se intento eliminar una relacion nula.";
            public const string IdUsuarioInvalido = 
                "El identificador del usuario debe ser positivo.";
            public const string IntentarObtenerSolicitudesIdInvalido = 
                "Intento de obtener solicitudes con ID invalido.";
            public const string IdUsuarioInvalidoObtenerAmigos = 
                "ID de usuario invalido al obtener lista de amigos.";
        }

        /// <summary>
        /// Mensajes de error para operaciones con usuarios.
        /// </summary>
        public static class Usuario
        {
            public const string ErrorVerificarExistencia = 
                "Error al verificar existencia del usuario.";
            public const string ErrorGuardarUsuario = 
                "Error al guardar el nuevo usuario.";
            public const string ErrorObtenerUsuario = 
                "Error al obtener el usuario.";
            public const string ErrorObtenerPorCorreo = 
                "Error al obtener usuario por correo.";
            public const string ErrorAsincronoObtenerPorCorreo = 
                "Error asincrono al obtener usuario por correo.";
            public const string ErrorObtenerConRedesSociales = 
                "Error al obtener usuario con redes sociales con id {0}.";
            public const string ErrorActualizarContrasena = 
                "Error al actualizar contrasena del usuario con id {0}.";
            public const string ErrorObtenerConJugadorPorNombre = 
                "Error al obtener usuario con jugador.";
            public const string UsuarioNoEncontrado = 
                "El usuario no fue encontrado.";
            public const string UsuarioNoExiste = 
                "El usuario no existe.";
            public const string IntentarCrearUsuarioNulo = 
                "Intento de crear un usuario nulo.";
            public const string IntentoBusquedaNombreVacio = 
                "Intento de busqueda de usuario con nombre vacio o nulo.";
            public const string IntentoBusquedaCorreoVacio = 
                "Intento de busqueda de usuario con correo vacio.";
            public const string NombreUsuarioObligatorio = 
                "El nombre de usuario es obligatorio.";
            public const string CorreoObligatorio = 
                "El correo es obligatorio.";
            public const string CorreoObligatorioBusquedaAsincrona = 
                "El correo electronico es obligatorio para la busqueda asincrona.";
            public const string IdUsuarioMayorCero = 
                "El identificador del usuario debe ser mayor que cero.";
            public const string NombreUsuarioObligatorioBusquedaJugador = 
                "El nombre de usuario es obligatorio para la busqueda con jugador.";
        }

        /// <summary>
        /// Mensajes de error para operaciones con reportes.
        /// </summary>
        public static class Reporte
        {
            public const string ErrorVerificarExistencia = 
                "Error al verificar existencia del reporte entre usuarios con ids {0} y {1}.";
            public const string ErrorGuardarReporte = 
                "Error al guardar el reporte.";
            public const string ErrorContarReportes = 
                "Error al contar reportes del usuario con id {0}.";
            public const string IntentarCrearReporteNulo = 
                "Se intento crear un reporte nulo.";
            public const string IdReportadoMayorCero = 
                "El identificador del usuario reportado debe ser mayor a cero.";
        }

        /// <summary>
        /// Mensajes de error para operaciones con jugadores.
        /// </summary>
        public static class Jugador
        {
            public const string ErrorVerificarExistenciaCorreo = 
                "Error al verificar existencia del correo.";
            public const string ErrorGuardarJugador = 
                "Error al guardar el jugador.";
            public const string IntentarCrearJugadorNulo = 
                "Intento de crear un jugador nulo.";
        }

        /// <summary>
        /// Mensajes de error para operaciones con clasificaciones.
        /// </summary>
        public static class Clasificacion
        {
            public const string ErrorCrearInicial = 
                "Error al crear la clasificacion inicial.";
            public const string ErrorActualizarClasificacion = 
                "Error al actualizar la clasificacion.";
            public const string ErrorConsultarMejores = 
                "Error al consultar los mejores jugadores.";
            public const string ClasificacionNoEncontrada = 
                "No se encontro clasificacion para el jugador con id {0}.";
        }

        /// <summary>
        /// Mensajes de error para operaciones con canciones.
        /// </summary>
        public static class Cancion
        {
            public const string ErrorInesperadoObtener = 
                "Error inesperado al obtener una cancion aleatoria.";
            public const string CancionesNoDisponibles = 
                "No hay canciones disponibles para los criterios solicitados.";
            public const string CancionesNoCumplenCriterios = 
                "No hay canciones disponibles que cumplan con los criterios solicitados.";
            public const string IdiomaInvalidoSolicitar = 
                "Se recibio un idioma invalido al solicitar cancion.";
            public const string IdiomaNoNuloVacio = 
                "El idioma no puede ser nulo o vacio.";
            public const string CancionNoEncontradaCatalogo = 
                "No se encontro la cancion con id {0} en el catalogo.";
            public const string CancionNoEnCatalogo = 
                "La cancion solicitada no se encuentra en el catalogo interno.";
        }
    }
}
