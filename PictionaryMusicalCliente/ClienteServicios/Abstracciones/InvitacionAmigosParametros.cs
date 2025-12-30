using System;
using System.Collections.Generic;

namespace PictionaryMusicalCliente.ClienteServicios.Abstracciones
{
    /// <summary>
    /// Agrupa los parámetros necesarios para obtener la información de invitación de amigos.
    /// </summary>
    public class InvitacionAmigosParametros
    {
        /// <summary>
        /// Inicializa una nueva instancia de los parámetros de invitación de amigos.
        /// </summary>
        /// <param name="codigoSala">Código de la sala a la que se invitará.</param>
        /// <param name="nombreUsuarioSesion">Nombre del usuario de la sesión actual.</param>
        /// <param name="amigosInvitados">Conjunto de IDs de amigos ya invitados.</param>
        /// <exception cref="ArgumentException">
        /// Si el código de sala está vacío o es nulo.
        /// </exception>
        public InvitacionAmigosParametros(
            string codigoSala,
            string nombreUsuarioSesion,
            ISet<int> amigosInvitados)
        {
            if (string.IsNullOrWhiteSpace(codigoSala))
            {
                throw new ArgumentException(
                    "El código de sala es obligatorio.",
                    nameof(codigoSala));
            }

            CodigoSala = codigoSala;
            NombreUsuarioSesion = nombreUsuarioSesion;
            AmigosInvitados = amigosInvitados;
        }

        /// <summary>
        /// Código de la sala a la que se invitará a los amigos.
        /// </summary>
        public string CodigoSala { get; }

        /// <summary>
        /// Nombre del usuario de la sesión actual.
        /// </summary>
        public string NombreUsuarioSesion { get; }

        /// <summary>
        /// Conjunto de IDs de amigos que ya han sido invitados.
        /// </summary>
        public ISet<int> AmigosInvitados { get; }
    }
}
