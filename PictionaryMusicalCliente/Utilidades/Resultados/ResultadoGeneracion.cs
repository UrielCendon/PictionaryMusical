namespace PictionaryMusicalCliente.Utilidades.Resultados
{
    /// <summary>
    /// Representa el resultado de una operacion de generacion de nombre de invitado.
    /// </summary>
    public sealed class ResultadoGeneracion
    {
        /// <summary>
        /// Obtiene un valor que indica si la generacion fue exitosa.
        /// </summary>
        public bool Exitoso { get; }

        /// <summary>
        /// Obtiene el nombre generado cuando la operacion es exitosa.
        /// </summary>
        public string NombreGenerado { get; }

        /// <summary>
        /// Obtiene el motivo del fallo cuando la operacion no es exitosa.
        /// </summary>
        public MotivoFalloGeneracion Motivo { get; }

        private ResultadoGeneracion(
            bool exitoso,
            string nombreGenerado,
            MotivoFalloGeneracion motivo)
        {
            Exitoso = exitoso;
            NombreGenerado = nombreGenerado;
            Motivo = motivo;
        }

        /// <summary>
        /// Crea un resultado exitoso con el nombre generado.
        /// </summary>
        /// <param name="nombre">El nombre generado.</param>
        /// <returns>Un resultado exitoso.</returns>
        public static ResultadoGeneracion Exito(string nombre)
        {
            return new ResultadoGeneracion(true, nombre, MotivoFalloGeneracion.Ninguno);
        }

        /// <summary>
        /// Crea un resultado fallido indicando el motivo.
        /// </summary>
        /// <param name="motivo">El motivo del fallo.</param>
        /// <returns>Un resultado fallido.</returns>
        public static ResultadoGeneracion Fallo(MotivoFalloGeneracion motivo)
        {
            return new ResultadoGeneracion(false, string.Empty, motivo);
        }
    }

    /// <summary>
    /// Enumera las posibles razones por las que falla la generacion de nombres.
    /// </summary>
    public enum MotivoFalloGeneracion
    {
        /// <summary>
        /// No hubo fallo.
        /// </summary>
        Ninguno,

        /// <summary>
        /// No se encontraron recursos de nombres para la cultura.
        /// </summary>
        RecursoNoEncontrado,

        /// <summary>
        /// La lista de nombres parseada esta vacia.
        /// </summary>
        ListaVacia,

        /// <summary>
        /// Todos los nombres disponibles ya fueron utilizados.
        /// </summary>
        NombresAgotados
    }
}
