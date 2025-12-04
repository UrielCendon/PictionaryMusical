namespace PictionaryMusicalCliente.Modelo
{
    /// <summary>
    /// Resultado capturado al solicitar los datos para reportar a un jugador.
    /// </summary>
    /// <remarks>
    /// Se utiliza un objeto dedicado porque el cuadro de diálogo de reportes devuelve
    /// dos piezas de información en una sola respuesta: si el usuario confirmó el envío
    /// y el motivo textual capturado en el formulario. En otras interacciones de la
    /// aplicación (por ejemplo, confirmaciones simples) basta con un valor booleano,
    /// mientras que aquí es necesario empaquetar ambos datos para que la vista y el
    /// <c>ViewModel</c> permanezcan desacoplados.
    /// </remarks>
    public class ResultadoReporteJugador
    {
        /// <summary>
        /// Indica si el usuario confirmó el envío del reporte.
        /// </summary>
        public bool Confirmado { get; set; }

        /// <summary>
        /// Motivo detallado proporcionado por el usuario.
        /// </summary>
        public string Motivo { get; set; }
    }
}
