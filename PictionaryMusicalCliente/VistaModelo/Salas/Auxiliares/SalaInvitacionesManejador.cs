using System.Collections.Generic;

namespace PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares
{
    /// <summary>
    /// Gestiona la logica de permisos e invitaciones en una sala de juego.
    /// </summary>
    public sealed class SalaInvitacionesManejador
    {
        private readonly bool _esInvitado;
        private readonly HashSet<int> _amigosInvitados;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="SalaInvitacionesManejador"/>.
        /// </summary>
        /// <param name="esInvitado">Indica si el usuario es invitado.</param>
        public SalaInvitacionesManejador(bool esInvitado)
        {
            _esInvitado = esInvitado;
            _amigosInvitados = new HashSet<int>();
        }

        /// <summary>
        /// Obtiene o establece si se puede invitar por correo.
        /// </summary>
        public bool PuedeInvitarPorCorreo { get; set; }

        /// <summary>
        /// Obtiene o establece si se puede invitar amigos.
        /// </summary>
        public bool PuedeInvitarAmigos { get; set; }

        /// <summary>
        /// Obtiene el conjunto de amigos invitados.
        /// </summary>
        public ISet<int> AmigosInvitados => _amigosInvitados;

        /// <summary>
        /// Configura los permisos iniciales de invitacion.
        /// </summary>
        public void ConfigurarPermisos()
        {
            PuedeInvitarPorCorreo = !_esInvitado;
            PuedeInvitarAmigos = !_esInvitado;
        }

        /// <summary>
        /// Verifica si un amigo ya fue invitado.
        /// </summary>
        /// <param name="amigoId">ID del amigo.</param>
        /// <returns>True si ya fue invitado.</returns>
        public bool AmigoYaInvitado(int amigoId)
        {
            return _amigosInvitados.Contains(amigoId);
        }

        /// <summary>
        /// Marca a un amigo como invitado.
        /// </summary>
        /// <param name="amigoId">ID del amigo.</param>
        public void MarcarAmigoInvitado(int amigoId)
        {
            _amigosInvitados.Add(amigoId);
        }

        /// <summary>
        /// Limpia la lista de amigos invitados.
        /// </summary>
        public void LimpiarAmigosInvitados()
        {
            _amigosInvitados.Clear();
        }
    }
}
