using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL.Utilidades
{
    /// <summary>
    /// Extensiones para configurar mocks de DbSet en pruebas unitarias.
    /// </summary>
    internal static class DbSetMockExtensiones
    {
        /// <summary>
        /// Configura un Mock de DbSet para soportar operaciones IQueryable.
        /// </summary>
        /// <typeparam name="T">Tipo de entidad del DbSet.</typeparam>
        /// <param name="dbSetMock">Mock del DbSet a configurar.</param>
        /// <param name="datos">Datos que contendra el mock.</param>
        public static void ConfigurarComoQueryable<T>(
            this Mock<DbSet<T>> dbSetMock, 
            IQueryable<T> datos) where T : class
        {
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(datos.Provider);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Expression)
                .Returns(datos.Expression);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.ElementType)
                .Returns(datos.ElementType);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(datos.GetEnumerator());
        }

        /// <summary>
        /// Configura un Mock de DbSet con una lista de datos.
        /// </summary>
        /// <typeparam name="T">Tipo de entidad del DbSet.</typeparam>
        /// <param name="dbSetMock">Mock del DbSet a configurar.</param>
        /// <param name="datos">Lista de datos que contendra el mock.</param>
        public static void ConfigurarConDatos<T>(
            this Mock<DbSet<T>> dbSetMock, 
            List<T> datos) where T : class
        {
            var queryable = datos.AsQueryable();
            dbSetMock.ConfigurarComoQueryable(queryable);
        }
    }
}
