using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Moq;

namespace PictionaryMusicalServidor.Pruebas.DAL.Utilidades
{
    /// <summary>
    /// Extensiones para crear mocks de DbSet utilizables en pruebas unitarias.
    /// Permite simular operaciones de Entity Framework sin conexion a base de datos.
    /// </summary>
    public static class DbSetMockExtensiones
    {
        /// <summary>
        /// Crea un mock de DbSet a partir de una lista de datos.
        /// Configura operaciones LINQ y manipulacion de entidades.
        /// Soporta Include() mediante el uso de IncludeQueryProvider.
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad del DbSet.</typeparam>
        /// <param name="listaDatos">Lista de datos iniciales para el DbSet.</param>
        /// <returns>Mock configurado del DbSet.</returns>
        public static Mock<DbSet<T>> CrearDbSetMock<T>(List<T> listaDatos) where T : class
        {
            var queryable = listaDatos.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            var proveedorInclude = new IncludeQueryProvider<T>(queryable);

            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(proveedorInclude);

            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Expression)
                .Returns(queryable.Expression);

            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.ElementType)
                .Returns(queryable.ElementType);

            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => listaDatos.GetEnumerator());

            dbSetMock.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(listaDatos.GetEnumerator()));

            dbSetMock.Setup(d => d.Add(It.IsAny<T>()))
                .Callback<T>(entidad => listaDatos.Add(entidad))
                .Returns<T>(entidad => entidad);

            dbSetMock.Setup(d => d.Remove(It.IsAny<T>()))
                .Callback<T>(entidad => listaDatos.Remove(entidad))
                .Returns<T>(entidad => entidad);

            dbSetMock.Setup(d => d.Include(It.IsAny<string>()))
                .Returns(dbSetMock.Object);

            return dbSetMock;
        }

        /// <summary>
        /// Crea un mock de DbSet vacio.
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad del DbSet.</typeparam>
        /// <returns>Mock configurado del DbSet vacio.</returns>
        public static Mock<DbSet<T>> CrearDbSetMockVacio<T>() where T : class
        {
            return CrearDbSetMock(new List<T>());
        }
    }

    /// <summary>
    /// Proveedor de consultas personalizado que soporta Include().
    /// Ignora las llamadas a Include() y simplemente devuelve la consulta original.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad.</typeparam>
    internal class IncludeQueryProvider<T> : IQueryProvider where T : class
    {
        private readonly IQueryable<T> _fuenteDatos;

        public IncludeQueryProvider(IQueryable<T> fuenteDatos)
        {
            _fuenteDatos = fuenteDatos;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new IncludeQueryable<T>(_fuenteDatos.Provider.CreateQuery<T>(expression), this);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var queryResultante = _fuenteDatos.Provider.CreateQuery<TElement>(expression);
            return new IncludeQueryable<TElement>(queryResultante, 
                new IncludeQueryProviderGenerico<TElement>(queryResultante));
        }

        public object Execute(Expression expression)
        {
            return _fuenteDatos.Provider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _fuenteDatos.Provider.Execute<TResult>(expression);
        }
    }

    /// <summary>
    /// Proveedor generico para consultas derivadas (Where, Select, etc.).
    /// </summary>
    internal class IncludeQueryProviderGenerico<T> : IQueryProvider
    {
        private readonly IQueryable<T> _fuenteDatos;

        public IncludeQueryProviderGenerico(IQueryable<T> fuenteDatos)
        {
            _fuenteDatos = fuenteDatos;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return _fuenteDatos.Provider.CreateQuery(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var queryResultante = _fuenteDatos.Provider.CreateQuery<TElement>(expression);
            
            if (queryResultante is IOrderedQueryable<TElement>)
            {
                return new IncludeOrderedQueryable<TElement>(
                    (IOrderedQueryable<TElement>)queryResultante, 
                    new IncludeQueryProviderGenerico<TElement>(queryResultante));
            }
            
            return new IncludeQueryable<TElement>(queryResultante, 
                new IncludeQueryProviderGenerico<TElement>(queryResultante));
        }

        public object Execute(Expression expression)
        {
            return _fuenteDatos.Provider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _fuenteDatos.Provider.Execute<TResult>(expression);
        }
    }

    /// <summary>
    /// Queryable personalizado que soporta Include() mediante el proveedor personalizado.
    /// </summary>
    internal class IncludeQueryable<T> : IQueryable<T>, IDbAsyncEnumerable<T>
    {
        protected readonly IQueryable<T> _fuenteDatos;
        protected readonly IQueryProvider _proveedor;

        public IncludeQueryable(IQueryable<T> fuenteDatos, IQueryProvider proveedor)
        {
            _fuenteDatos = fuenteDatos;
            _proveedor = proveedor;
        }

        public Expression Expression => _fuenteDatos.Expression;
        public System.Type ElementType => _fuenteDatos.ElementType;
        public IQueryProvider Provider => _proveedor;

        public IEnumerator<T> GetEnumerator() => _fuenteDatos.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(_fuenteDatos.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }
    }

    /// <summary>
    /// Queryable ordenado personalizado que soporta Include() y operaciones de ordenamiento.
    /// </summary>
    internal class IncludeOrderedQueryable<T> : IncludeQueryable<T>, IOrderedQueryable<T>
    {
        public IncludeOrderedQueryable(IOrderedQueryable<T> fuenteDatos, IQueryProvider proveedor)
            : base(fuenteDatos, proveedor)
        {
        }
    }

    /// <summary>
    /// Enumerador asincrono de prueba para EF.
    /// </summary>
    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumeradorInterno;

        public TestDbAsyncEnumerator(IEnumerator<T> enumerador)
        {
            _enumeradorInterno = enumerador;
        }

        public void Dispose()
        {
            _enumeradorInterno.Dispose();
        }

        public System.Threading.Tasks.Task<bool> MoveNextAsync(
            System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult(_enumeradorInterno.MoveNext());
        }

        public T Current => _enumeradorInterno.Current;
        object IDbAsyncEnumerator.Current => Current;
    }
}
