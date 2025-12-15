using PictionaryMusicalCliente.Utilidades.Resultados;
using System.Collections.Generic;
using System.Globalization;

namespace PictionaryMusicalCliente.Utilidades.Abstracciones
{
    /// <summary>
    /// Define el contrato para la generacion de nombres aleatorios de invitados.
    /// </summary>
    public interface INombreInvitadoGenerador
    {
        /// <summary>
        /// Obtiene un nombre de invitado aleatorio acorde a la cultura proporcionada.
        /// </summary>
        /// <param name="cultura">Cultura a utilizar. Si es null se usa la actual.</param>
        /// <param name="nombresExcluidos">Lista de nombres que no deben repetirse.</param>
        /// <returns>Resultado de la generacion con el nombre o motivo de fallo.</returns>
        ResultadoGeneracion Generar(
            CultureInfo cultura,
            IEnumerable<string> nombresExcluidos = null);
    }
}
