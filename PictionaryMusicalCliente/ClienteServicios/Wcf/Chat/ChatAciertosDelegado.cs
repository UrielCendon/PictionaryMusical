using System;
using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Chat
{
    /// <summary>
    /// Implementacion delegada que conecta la logica de chat con el registro de aciertos.
    /// </summary>
    public class ChatAciertosDelegado : IChatAciertosServicio
    {
        private readonly Func<string> _obtenerNombreJugador;
        private readonly Func<string, int, int, Task> _registrarAcierto;

        /// <summary>
        /// Inicializa el delegado con las funciones necesarias para operar.
        /// </summary>
        /// <param name="obtenerNombreJugador">Funcion para obtener el usuario actual.</param>
        /// <param name="registrarAcierto">Funcion asincrona para registrar puntos.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna de las funciones es nula.
        /// </exception>
        public ChatAciertosDelegado(
            Func<string> obtenerNombreJugador,
            Func<string, int, int, Task> registrarAcierto)
        {
            _obtenerNombreJugador = obtenerNombreJugador
                ?? throw new ArgumentNullException(nameof(obtenerNombreJugador));
            _registrarAcierto = registrarAcierto
                ?? throw new ArgumentNullException(nameof(registrarAcierto));
        }

        /// <inheritdoc />
        public string ObtenerNombreJugadorActual()
        {
            return _obtenerNombreJugador();
        }

        /// <inheritdoc />
        public Task RegistrarAciertoAsync(
            string nombreJugador,
            int puntosAdivinador,
            int puntosDibujante)
        {
            return _registrarAcierto(nombreJugador, puntosAdivinador, puntosDibujante);
        }
    }
}
