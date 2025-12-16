namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Chat
{
    /// <summary>
    /// Representa los posibles resultados al evaluar un mensaje en el chat.
    /// </summary>
    public enum ChatDecision
    {
        /// <summary>
        /// El mensaje puede enviarse libremente sin restricciones.
        /// </summary>
        CanalLibre,

        /// <summary>
        /// El mensaje no puede enviarse porque el usuario es el dibujante.
        /// </summary>
        MensajeBloqueado,

        /// <summary>
        /// El mensaje fue procesado pero no coincide con la respuesta correcta.
        /// </summary>
        IntentoFallido,

        /// <summary>
        /// El mensaje coincidio con la respuesta correcta y se registro el acierto.
        /// </summary>
        AciertoRegistrado
    }
}
