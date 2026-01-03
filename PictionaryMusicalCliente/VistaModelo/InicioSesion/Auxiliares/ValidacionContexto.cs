using System.Collections.Generic;

namespace PictionaryMusicalCliente.VistaModelo.InicioSesion.Auxiliares
{
    /// <summary>
    /// Contexto acumulador para validación de campos.
    /// </summary>
    public sealed class ValidacionContexto
    {
        private readonly List<string> _camposInvalidos;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ValidacionContexto"/>.
        /// </summary>
        public ValidacionContexto()
        {
            _camposInvalidos = new List<string>();
            PrimerMensajeError = null;
        }

        /// <summary>
        /// Obtiene la lista de campos inválidos.
        /// </summary>
        public List<string> CamposInvalidos => _camposInvalidos;

        /// <summary>
        /// Obtiene o establece el primer mensaje de error encontrado.
        /// </summary>
        public string PrimerMensajeError { get; set; }

        /// <summary>
        /// Agrega un campo inválido al contexto.
        /// </summary>
        /// <param name="nombreCampo">Nombre del campo inválido.</param>
        /// <param name="mensajeError">Mensaje de error asociado.</param>
        public void AgregarCampoInvalido(string nombreCampo, string mensajeError)
        {
            _camposInvalidos.Add(nombreCampo);
            PrimerMensajeError ??= mensajeError;
        }
    }
}
