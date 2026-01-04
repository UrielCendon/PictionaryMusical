using System;
using System.Collections.Generic;
using Datos.Modelo;

namespace PictionaryMusicalServidor.Pruebas.DAL.Utilidades
{
    /// <summary>
    /// Fabrica de entidades de prueba para los tests unitarios.
    /// Proporciona metodos para crear entidades con datos predefinidos o personalizados.
    /// </summary>
    public static class EntidadesPruebaFabrica
    {
        /// <summary>
        /// Crea un usuario de prueba con datos validos por defecto.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="nombreUsuario">Nombre de usuario.</param>
        /// <returns>Usuario con datos de prueba.</returns>
        public static Usuario CrearUsuario(
            int id = 1,
            string nombreUsuario = "UsuarioPrueba")
        {
            var clasificacion = CrearClasificacion(id);
            var jugador = CrearJugador(id, clasificacion);

            return new Usuario
            {
                idUsuario = id,
                Nombre_Usuario = nombreUsuario,
                Contrasena = "contrasenaHasheada123",
                Jugador_idJugador = id,
                Jugador = jugador
            };
        }

        /// <summary>
        /// Crea un usuario sin jugador asociado para pruebas de casos limite.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="nombreUsuario">Nombre de usuario.</param>
        /// <returns>Usuario sin relaciones.</returns>
        public static Usuario CrearUsuarioSinJugador(
            int id = 1,
            string nombreUsuario = "UsuarioSinJugador")
        {
            return new Usuario
            {
                idUsuario = id,
                Nombre_Usuario = nombreUsuario,
                Contrasena = "contrasenaHasheada456",
                Jugador_idJugador = id
            };
        }

        /// <summary>
        /// Crea un jugador de prueba con datos validos.
        /// </summary>
        /// <param name="id">Identificador del jugador.</param>
        /// <param name="clasificacion">Clasificacion asociada al jugador.</param>
        /// <returns>Jugador con datos de prueba.</returns>
        public static Jugador CrearJugador(
            int id = 1,
            Clasificacion clasificacion = null)
        {
            return new Jugador
            {
                idJugador = id,
                Nombre = "NombrePrueba",
                Apellido = "ApellidoPrueba",
                Correo = $"usuario{id}@ejemplo.com",
                Id_Avatar = 1,
                Clasificacion_idClasificacion = clasificacion?.idClasificacion ?? id,
                Clasificacion = clasificacion
            };
        }

        /// <summary>
        /// Crea una clasificacion de prueba con valores iniciales.
        /// </summary>
        /// <param name="id">Identificador de la clasificacion.</param>
        /// <param name="puntos">Puntos ganados iniciales.</param>
        /// <param name="rondas">Rondas ganadas iniciales.</param>
        /// <returns>Clasificacion con datos de prueba.</returns>
        public static Clasificacion CrearClasificacion(
            int id = 1,
            int puntos = 0,
            int rondas = 0)
        {
            return new Clasificacion
            {
                idClasificacion = id,
                Puntos_Ganados = puntos,
                Rondas_Ganadas = rondas
            };
        }

        /// <summary>
        /// Crea una relacion de amistad de prueba.
        /// </summary>
        /// <param name="emisorId">ID del usuario emisor.</param>
        /// <param name="receptorId">ID del usuario receptor.</param>
        /// <param name="estado">Estado de la relacion (true = aceptada).</param>
        /// <returns>Relacion de amistad con datos de prueba.</returns>
        public static Amigo CrearRelacionAmistad(
            int emisorId = 1,
            int receptorId = 2,
            bool estado = false)
        {
            return new Amigo
            {
                UsuarioEmisor = emisorId,
                UsuarioReceptor = receptorId,
                Estado = estado
            };
        }

        /// <summary>
        /// Crea un reporte de prueba.
        /// </summary>
        /// <param name="id">Identificador del reporte.</param>
        /// <param name="reportanteId">ID del usuario que reporta.</param>
        /// <param name="reportadoId">ID del usuario reportado.</param>
        /// <param name="motivo">Motivo del reporte.</param>
        /// <returns>Reporte con datos de prueba.</returns>
        public static Reporte CrearReporte(
            int id = 1,
            int reportanteId = 1,
            int reportadoId = 2,
            string motivo = "Comportamiento inapropiado")
        {
            return new Reporte
            {
                idReporte = id,
                idReportante = reportanteId,
                idReportado = reportadoId,
                Motivo = motivo,
                Fecha_Reporte = DateTime.Now
            };
        }

        /// <summary>
        /// Crea una lista de usuarios de prueba.
        /// </summary>
        /// <param name="cantidad">Cantidad de usuarios a crear.</param>
        /// <returns>Lista de usuarios de prueba.</returns>
        public static List<Usuario> CrearListaUsuarios(int cantidad)
        {
            var usuarios = new List<Usuario>();

            for (int i = 1; i <= cantidad; i++)
            {
                usuarios.Add(CrearUsuario(i, $"Usuario{i}"));
            }

            return usuarios;
        }

        /// <summary>
        /// Crea una lista de jugadores de prueba.
        /// </summary>
        /// <param name="cantidad">Cantidad de jugadores a crear.</param>
        /// <returns>Lista de jugadores de prueba.</returns>
        public static List<Jugador> CrearListaJugadores(int cantidad)
        {
            var jugadores = new List<Jugador>();

            for (int i = 1; i <= cantidad; i++)
            {
                var clasificacion = CrearClasificacion(i);
                jugadores.Add(CrearJugador(i, clasificacion));
            }

            return jugadores;
        }
    }
}
