using System.Data.Entity;
using Datos.Modelo;
using Moq;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL.Utilidades
{
    /// <summary>
    /// Fabrica para crear mocks del contexto de base de datos en pruebas unitarias.
    /// </summary>
    internal static class ContextoMockFabrica
    {
        /// <summary>
        /// Crea un mock basico del contexto de base de datos.
        /// </summary>
        /// <returns>Mock configurado del contexto.</returns>
        public static Mock<BaseDatosPruebaEntities> CrearContextoMock()
        {
            return new Mock<BaseDatosPruebaEntities>();
        }

        /// <summary>
        /// Crea un mock del contexto con DbSets vacios configurados.
        /// </summary>
        /// <returns>Mock del contexto con DbSets inicializados.</returns>
        public static Mock<BaseDatosPruebaEntities> CrearContextoConDbSetsVacios()
        {
            var contextoMock = new Mock<BaseDatosPruebaEntities>();
            
            var usuarioDbSet = new Mock<DbSet<Usuario>>();
            var jugadorDbSet = new Mock<DbSet<Jugador>>();
            var amigoDbSet = new Mock<DbSet<Amigo>>();
            var clasificacionDbSet = new Mock<DbSet<Clasificacion>>();
            var reporteDbSet = new Mock<DbSet<Reporte>>();
            
            contextoMock.Setup(c => c.Usuario).Returns(usuarioDbSet.Object);
            contextoMock.Setup(c => c.Jugador).Returns(jugadorDbSet.Object);
            contextoMock.Setup(c => c.Amigo).Returns(amigoDbSet.Object);
            contextoMock.Setup(c => c.Clasificacion).Returns(clasificacionDbSet.Object);
            contextoMock.Setup(c => c.Reporte).Returns(reporteDbSet.Object);
            
            return contextoMock;
        }
    }
}
