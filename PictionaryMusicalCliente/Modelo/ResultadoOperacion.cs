namespace PictionaryMusicalCliente.Modelo
{
    /// <summary>
    /// Representa el resultado de una operacion que puede tener exito o fallar,
    /// conteniendo un valor en caso de exito.
    /// </summary>
    /// <typeparam name="T">Tipo del valor contenido en caso de exito.</typeparam>
    public sealed class ResultadoOperacion<T>
    {
        private readonly T _valor;
        private readonly bool _exitoso;

        private ResultadoOperacion(T valor, bool exitoso)
        {
            _valor = valor;
            _exitoso = exitoso;
        }

        /// <summary>
        /// Indica si la operacion fue exitosa.
        /// </summary>
        public bool Exitoso
        {
            get { return _exitoso; }
        }

        /// <summary>
        /// Indica si la operacion fallo.
        /// </summary>
        public bool Fallido
        {
            get { return !_exitoso; }
        }

        /// <summary>
        /// Obtiene el valor contenido. Solo debe accederse si Exitoso es true.
        /// </summary>
        public T Valor
        {
            get { return _valor; }
        }

        /// <summary>
        /// Crea un resultado exitoso con el valor especificado.
        /// </summary>
        /// <param name="valor">Valor a contener.</param>
        /// <returns>Resultado exitoso.</returns>
        public static ResultadoOperacion<T> Exito(T valor)
        {
            return new ResultadoOperacion<T>(valor, true);
        }

        /// <summary>
        /// Crea un resultado fallido sin valor.
        /// </summary>
        /// <returns>Resultado fallido.</returns>
        public static ResultadoOperacion<T> Fallo()
        {
            return new ResultadoOperacion<T>(default(T), false);
        }
    }
}
