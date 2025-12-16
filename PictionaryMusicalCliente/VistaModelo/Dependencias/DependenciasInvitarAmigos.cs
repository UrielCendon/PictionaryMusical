using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;
using System.Collections.Generic;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias de datos para invitar amigos.
    /// </summary>
    /// <remarks>
    /// Incluye la lista de amigos, codigo de sala y callbacks de invitacion.
    /// </remarks>
    public class DependenciasInvitarAmigos
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias de invitacion.
        /// </summary>
        /// <param name="invitacionesServicio">
        /// Servicio para enviar invitaciones.
        /// </param>
        /// <param name="perfilServicio">Servicio de perfil de usuario.</param>
        /// <param name="amigos">Lista de amigos disponibles.</param>
        /// <param name="codigoSala">Codigo de la sala de juego.</param>
        /// <param name="amigoInvitado">
        /// Funcion para verificar si un amigo ya fue invitado.
        /// </param>
        /// <param name="registrarAmigoInvitado">
        /// Accion para registrar un amigo como invitado.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Si el codigo de sala esta vacio o es solo espacios.
        /// </exception>
        public DependenciasInvitarAmigos(
            IInvitacionesServicio invitacionesServicio,
            IPerfilServicio perfilServicio,
            IEnumerable<DTOs.AmigoDTO> amigos,
            string codigoSala,
            Func<int, bool> amigoInvitado,
            Action<int> registrarAmigoInvitado)
        {
            InvitacionesServicio = invitacionesServicio ?? 
                throw new ArgumentNullException(nameof(invitacionesServicio));
            PerfilServicio = perfilServicio ?? 
                throw new ArgumentNullException(nameof(perfilServicio));
            Amigos = amigos ?? 
                throw new ArgumentNullException(nameof(amigos));
            
            if (string.IsNullOrWhiteSpace(codigoSala))
            {
                throw new ArgumentException(
                    "El codigo de la sala es obligatorio.",
                    nameof(codigoSala));
            }

            CodigoSala = codigoSala;
            AmigoInvitado = amigoInvitado;
            RegistrarAmigoInvitado = registrarAmigoInvitado;
        }

        /// <summary>
        /// Servicio para enviar invitaciones.
        /// </summary>
        public IInvitacionesServicio InvitacionesServicio { get; }

        /// <summary>
        /// Servicio para obtener perfiles de usuario.
        /// </summary>
        public IPerfilServicio PerfilServicio { get; }

        /// <summary>
        /// Lista de amigos disponibles para invitar.
        /// </summary>
        public IEnumerable<DTOs.AmigoDTO> Amigos { get; }

        /// <summary>
        /// Codigo de la sala de juego.
        /// </summary>
        public string CodigoSala { get; }

        /// <summary>
        /// Funcion que verifica si un amigo ya fue invitado.
        /// </summary>
        public Func<int, bool> AmigoInvitado { get; }

        /// <summary>
        /// Accion para registrar un amigo como invitado.
        /// </summary>
        public Action<int> RegistrarAmigoInvitado { get; }
    }
}
