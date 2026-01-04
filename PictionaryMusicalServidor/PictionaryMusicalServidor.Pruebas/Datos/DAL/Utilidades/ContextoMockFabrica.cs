using System.Collections.Generic;
using System.Data.Entity;
using Datos.Modelo;
using Moq;

namespace PictionaryMusicalServidor.Pruebas.DAL.Utilidades
{
    /// <summary>
    /// Fabrica de mocks para el contexto de base de datos.
    /// Centraliza la creacion de mocks para mantener consistencia en las pruebas.
    /// </summary>
    public static class ContextoMockFabrica
    {
        /// <summary>
        /// Crea un mock del contexto de base de datos con DbSets vacios.
        /// </summary>
        /// <returns>Mock configurado del contexto.</returns>
        public static Mock<BaseDatosPruebaEntities> CrearContextoMock()
        {
            var contextoMock = new Mock<BaseDatosPruebaEntities>();

            contextoMock.Setup(c => c.Usuario)
                .Returns(DbSetMockExtensiones.CrearDbSetMockVacio<Usuario>().Object);

            contextoMock.Setup(c => c.Jugador)
                .Returns(DbSetMockExtensiones.CrearDbSetMockVacio<Jugador>().Object);

            contextoMock.Setup(c => c.Amigo)
                .Returns(DbSetMockExtensiones.CrearDbSetMockVacio<Amigo>().Object);

            contextoMock.Setup(c => c.Clasificacion)
                .Returns(DbSetMockExtensiones.CrearDbSetMockVacio<Clasificacion>().Object);

            contextoMock.Setup(c => c.Reporte)
                .Returns(DbSetMockExtensiones.CrearDbSetMockVacio<Reporte>().Object);

            return contextoMock;
        }

        /// <summary>
        /// Crea un mock del contexto con datos de usuarios preconfigurados.
        /// </summary>
        /// <param name="usuarios">Lista de usuarios para el DbSet.</param>
        /// <returns>Mock configurado del contexto.</returns>
        public static Mock<BaseDatosPruebaEntities> CrearContextoConUsuarios(
            List<Usuario> usuarios)
        {
            var contextoMock = CrearContextoMock();
            var usuarioDbSetMock = DbSetMockExtensiones.CrearDbSetMock(usuarios);

            contextoMock.Setup(c => c.Usuario).Returns(usuarioDbSetMock.Object);

            return contextoMock;
        }

        /// <summary>
        /// Crea un mock del contexto con datos de jugadores preconfigurados.
        /// </summary>
        /// <param name="jugadores">Lista de jugadores para el DbSet.</param>
        /// <returns>Mock configurado del contexto.</returns>
        public static Mock<BaseDatosPruebaEntities> CrearContextoConJugadores(
            List<Jugador> jugadores)
        {
            var contextoMock = CrearContextoMock();
            var jugadorDbSetMock = DbSetMockExtensiones.CrearDbSetMock(jugadores);

            contextoMock.Setup(c => c.Jugador).Returns(jugadorDbSetMock.Object);

            return contextoMock;
        }

        /// <summary>
        /// Crea un mock del contexto con datos de amigos preconfigurados.
        /// </summary>
        /// <param name="amigos">Lista de relaciones de amistad para el DbSet.</param>
        /// <returns>Mock configurado del contexto.</returns>
        public static Mock<BaseDatosPruebaEntities> CrearContextoConAmigos(
            List<Amigo> amigos)
        {
            var contextoMock = CrearContextoMock();
            var amigoDbSetMock = DbSetMockExtensiones.CrearDbSetMock(amigos);

            contextoMock.Setup(c => c.Amigo).Returns(amigoDbSetMock.Object);

            return contextoMock;
        }

        /// <summary>
        /// Crea un mock del contexto con datos de clasificaciones preconfigurados.
        /// </summary>
        /// <param name="clasificaciones">Lista de clasificaciones para el DbSet.</param>
        /// <returns>Mock configurado del contexto.</returns>
        public static Mock<BaseDatosPruebaEntities> CrearContextoConClasificaciones(
            List<Clasificacion> clasificaciones)
        {
            var contextoMock = CrearContextoMock();
            var clasificacionDbSetMock = DbSetMockExtensiones.CrearDbSetMock(clasificaciones);

            contextoMock.Setup(c => c.Clasificacion).Returns(clasificacionDbSetMock.Object);

            return contextoMock;
        }

        /// <summary>
        /// Crea un mock del contexto con datos de reportes preconfigurados.
        /// </summary>
        /// <param name="reportes">Lista de reportes para el DbSet.</param>
        /// <returns>Mock configurado del contexto.</returns>
        public static Mock<BaseDatosPruebaEntities> CrearContextoConReportes(
            List<Reporte> reportes)
        {
            var contextoMock = CrearContextoMock();
            var reporteDbSetMock = DbSetMockExtensiones.CrearDbSetMock(reportes);

            contextoMock.Setup(c => c.Reporte).Returns(reporteDbSetMock.Object);

            return contextoMock;
        }
    }
}
