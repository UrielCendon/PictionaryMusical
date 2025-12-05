using PictionaryMusicalCliente.VistaModelo.Amigos;

namespace PictionaryMusicalCliente.ClienteServicios.Abstracciones
{
    /// <summary>
    /// Resultado estandarizado de las invitaciones por correo.
    /// </summary>
    public class InvitacionCorreoResultado
    {
        public bool Exitoso { get; private set; }
        public string Mensaje { get; private set; }

        private InvitacionCorreoResultado(bool exitoso, string mensaje)
        {
            Exitoso = exitoso;
            Mensaje = mensaje;
        }

        public static InvitacionCorreoResultado Exito(string mensaje) =>
            new InvitacionCorreoResultado(true, mensaje);

        public static InvitacionCorreoResultado Fallo(string mensaje) =>
            new InvitacionCorreoResultado(false, mensaje);
    }

    /// <summary>
    /// Resultado estandarizado al preparar invitaciones de amigos.
    /// </summary>
    public class InvitacionAmigosResultado
    {
        public bool Exitoso { get; private set; }
        public string Mensaje { get; private set; }
        public InvitarAmigosVistaModelo VistaModelo { get; private set; }

        private InvitacionAmigosResultado(
            bool exitoso,
            string mensaje,
            InvitarAmigosVistaModelo vistaModelo)
        {
            Exitoso = exitoso;
            Mensaje = mensaje;
            VistaModelo = vistaModelo;
        }

        public static InvitacionAmigosResultado Exito(InvitarAmigosVistaModelo vistaModelo) =>
            new InvitacionAmigosResultado(true, string.Empty, vistaModelo);

        public static InvitacionAmigosResultado Fallo(string mensaje) =>
            new InvitacionAmigosResultado(false, mensaje, null);
    }
}