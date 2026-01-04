using System;
using Datos.Modelo;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL.Utilidades
{
    /// <summary>
    /// Fabrica para crear entidades de prueba con datos predeterminados.
    /// </summary>
    internal static class EntidadesPruebaFabrica
    {
        /// <summary>
        /// Crea un usuario de prueba con valores predeterminados.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="nombreUsuario">Nombre de usuario.</param>
        /// <returns>Usuario configurado para pruebas.</returns>
        public static Usuario CrearUsuario(int id = 1, string nombreUsuario = "UsuarioPrueba")
        {
            return new Usuario
            {
                idUsuario = id,
                Nombre_Usuario = nombreUsuario,
                Contrasena = "hashPrueba123"
            };
        }

        /// <summary>
        /// Crea un jugador de prueba con valores predeterminados.
        /// </summary>
        /// <param name="id">Identificador del jugador.</param>
        /// <param name="correo">Correo electronico del jugador.</param>
        /// <returns>Jugador configurado para pruebas.</returns>
        public static Jugador CrearJugador(int id = 1, string correo = "prueba@ejemplo.com")
        {
            return new Jugador
            {
                idJugador = id,
                Nombre = "JugadorPrueba",
                Apellido = "ApellidoPrueba",
                Correo = correo
            };
        }

        /// <summary>
        /// Crea una clasificacion de prueba con valores predeterminados.
        /// </summary>
        /// <param name="id">Identificador de la clasificacion.</param>
        /// <returns>Clasificacion configurada para pruebas.</returns>
        public static Clasificacion CrearClasificacion(int id = 1)
        {
            return new Clasificacion
            {
                idClasificacion = id,
                Puntos_Ganados = 0,
                Rondas_Ganadas = 0
            };
        }

        /// <summary>
        /// Crea una relacion de amistad de prueba.
        /// </summary>
        /// <param name="emisorId">Id del usuario emisor.</param>
        /// <param name="receptorId">Id del usuario receptor.</param>
        /// <param name="aceptada">Estado de la solicitud.</param>
        /// <returns>Amigo configurado para pruebas.</returns>
        public static Amigo CrearAmigo(int emisorId = 1, int receptorId = 2, bool aceptada = false)
        {
            return new Amigo
            {
                UsuarioEmisor = emisorId,
                UsuarioReceptor = receptorId,
                Estado = aceptada
            };
        }

        /// <summary>
        /// Crea un reporte de prueba.
        /// </summary>
        /// <param name="id">Identificador del reporte.</param>
        /// <param name="reportanteId">Id del usuario que reporta.</param>
        /// <param name="reportadoId">Id del usuario reportado.</param>
        /// <returns>Reporte configurado para pruebas.</returns>
        public static Reporte CrearReporte(
            int id = 1, 
            int reportanteId = 1, 
            int reportadoId = 2)
        {
            return new Reporte
            {
                idReporte = id,
                idReportante = reportanteId,
                idReportado = reportadoId,
                Motivo = "Motivo de prueba",
                Fecha_Reporte = DateTime.Now
            };
        }
    }
}
