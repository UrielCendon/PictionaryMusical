namespace PictionaryMusicalServidor.Datos.Constantes
{
    /// <summary>
    /// Clase estatica que contiene los mensajes de error utilizados en la capa de datos.
    /// </summary>
    public static class MensajesErrorDatos
    {
        /// <summary>
        /// Mensajes de error para operaciones con amigos.
        /// </summary>
        public static class Amigo
        {
            public const string ErrorVerificarRelacion = 
                "Error al verificar existencia de relacion de amistad en la base de datos.";
            public const string ErrorObtenerRelacion = 
                "Error al obtener la relacion de amistad de la base de datos.";
            public const string ErrorObtenerSolicitudesPendientes = 
                "Error al obtener solicitudes pendientes para el usuario ID: {0}.";
            public const string ErrorActualizarEstado = 
                "Error al actualizar el estado de la relacion entre {0} y {1}.";
            public const string ErrorEliminarRelacion = 
                "Error al eliminar la relacion de amistad entre {0} y {1}.";
            public const string ErrorObtenerAmigos = 
                "Error de base de datos al obtener amigos para el usuario ID: {0}.";
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
                "Error al verificar existencia del usuario '{0}'.";
            public const string ErrorGuardarUsuario = 
                "Error al guardar el nuevo usuario '{0}' en la base de datos.";
            public const string ErrorObtenerUsuario = 
                "Error al obtener el usuario '{0}' de la base de datos.";
            public const string ErrorObtenerPorCorreo = 
                "Error al obtener usuario por correo '{0}'.";
            public const string ErrorAsincronoObtenerPorCorreo = 
                "Error asincrono al obtener usuario por correo '{0}'.";
            public const string ErrorObtenerConRedesSociales = 
                "Error al obtener usuario con redes sociales ID: {0}";
            public const string ErrorActualizarContrasena = 
                "Error al actualizar contrasena del usuario ID {0}.";
            public const string ErrorObtenerConJugadorPorNombre = 
                "Error al obtener usuario con jugador por nombre '{0}'.";
            public const string UsuarioNoEncontrado = 
                "El usuario '{0}' no fue encontrado en la base de datos.";
            public const string UsuarioNoExiste = 
                "El usuario '{0}' no existe en la base de datos.";
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
                "Error al verificar existencia del reporte entre {0} y {1}.";
            public const string ErrorGuardarReporte = 
                "Error al guardar el reporte en la base de datos.";
            public const string ErrorContarReportes = 
                "Error al contar reportes del usuario {0}.";
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
                "Error al verificar existencia del correo '{0}'.";
            public const string ErrorGuardarJugador = 
                "Error al guardar el jugador con correo '{0}'.";
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
                "Error al actualizar la clasificacion del jugador con ID {0}.";
            public const string ErrorConsultarMejores = 
                "Error al consultar los mejores jugadores.";
            public const string ClasificacionNoEncontrada = 
                "No se encontro clasificacion para el jugador con ID {0}.";
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
