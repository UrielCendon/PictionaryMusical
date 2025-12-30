using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.VistaModelo.Amigos;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using System;
using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Encapsula la logica de invitaciones para reducir la carga del ViewModel principal.
    /// </summary>
    public class InvitacionSalaServicio : IInvitacionSalaServicio
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(InvitacionSalaServicio));

        private readonly IInvitacionesServicio _invitacionesServicio;
        private readonly IListaAmigosServicio _listaAmigosServicio;
        private readonly IPerfilServicio _perfilServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _aviso;
        private bool _desechado;

        /// <summary>
        /// Inicializa el servicio facade de invitaciones de sala.
        /// </summary>
        /// <param name="invitacionesServicio">Servicio para enviar invitaciones.</param>
        /// <param name="listaAmigosServicio">Servicio para obtener lista de amigos.</param>
        /// <param name="perfilServicio">Servicio de perfiles de usuario.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <param name="aviso">Servicio para mostrar avisos al usuario.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia es nula.
        /// </exception>
        public InvitacionSalaServicio(
            IInvitacionesServicio invitacionesServicio,
            IListaAmigosServicio listaAmigosServicio,
            IPerfilServicio perfilServicio,
            SonidoManejador sonidoManejador,
            IAvisoServicio aviso)
        {
            _invitacionesServicio = invitacionesServicio ??
                throw new ArgumentNullException(nameof(invitacionesServicio));
            _listaAmigosServicio = listaAmigosServicio ??
                throw new ArgumentNullException(nameof(listaAmigosServicio));
            _perfilServicio = perfilServicio ??
                throw new ArgumentNullException(nameof(perfilServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            _aviso = aviso ??
                throw new ArgumentNullException(nameof(aviso));
        }

        /// <summary>
        /// Envia una invitacion por correo validando el formato previamente.
        /// </summary>
        public async Task<InvitacionCorreoResultado> InvitarPorCorreoAsync(
            string codigoSala,
            string correo)
        {
            if (string.IsNullOrWhiteSpace(codigoSala))
            {
                throw new ArgumentNullException(nameof(codigoSala));
            }

            string correoLimpio = correo?.Trim();

            var validacion = ValidadorEntrada.ValidarCorreo(correoLimpio);

            if (!validacion.OperacionExitosa)
            {
                return InvitacionCorreoResultado.Fallo(
                    validacion.Mensaje ?? Lang.errorTextoCorreoInvalido);
            }

            return await ProcesarEnvioCorreoAsync(codigoSala, correoLimpio).ConfigureAwait(false);
        }

        /// <summary>
        /// Prepara el ViewModel para invitar amigos conectados.
        /// </summary>
        /// <param name="parametros">Parametros que contienen el codigo de sala, usuario y
        /// amigos invitados.</param>
        /// <returns>Resultado con el ViewModel preparado o mensaje de error.</returns>
        public async Task<InvitacionAmigosResultado> ObtenerInvitacionAmigosAsync(
            InvitacionAmigosParametros parametros)
        {
            if (parametros == null)
            {
                throw new ArgumentNullException(nameof(parametros));
            }

            if (string.IsNullOrWhiteSpace(parametros.NombreUsuarioSesion))
            {
                _logger.Warn("Intento de invitar amigos sin usuario de sesion.");
                return InvitacionAmigosResultado.Fallo(Lang.errorTextoErrorProcesarSolicitud);
            }

            try
            {
                var amigos = await _listaAmigosServicio
                    .ObtenerAmigosAsync(parametros.NombreUsuarioSesion)
                    .ConfigureAwait(false);

                if (amigos == null || amigos.Count == 0)
                {
                    return InvitacionAmigosResultado.Fallo(Lang.invitarAmigosTextoSinAmigos);
                }

                var dependenciasBase = new VistaModeloBaseDependencias(
                    App.VentanaServicio,
                    App.Localizador,
                    _sonidoManejador,
                    _aviso);

                var dependenciasInvitar = new InvitarAmigosDependencias(
                    _invitacionesServicio,
                    _perfilServicio,
                    amigos,
                    parametros.CodigoSala,
                    id => parametros.AmigosInvitados?.Contains(id) ?? false,
                    id => parametros.AmigosInvitados?.Add(id));

                var vistaModelo = new InvitarAmigosVistaModelo(
                    dependenciasBase,
                    dependenciasInvitar);

                return InvitacionAmigosResultado.Exito(vistaModelo);
            }
            catch (ServicioExcepcion excepcion)
            {
                _logger.Error("Error de servicio al obtener amigos.", excepcion);
                return InvitacionAmigosResultado.Fallo(
                    excepcion.Message ?? Lang.errorTextoErrorProcesarSolicitud);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida al obtener amigos.", excepcion);
                return InvitacionAmigosResultado.Fallo(Lang.errorTextoErrorProcesarSolicitud);
            }
        }

        /// <summary>
        /// Libera los recursos utilizados por el servicio.
        /// </summary>
        /// <param name="desechando">
        /// True si se llama desde Dispose, false desde el finalizador.
        /// </param>
        protected virtual void Dispose(bool desechando)
        {
            if (_desechado)
            {
                return;
            }

            _desechado = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(desechando: true);
            GC.SuppressFinalize(this);
        }

        private async Task<InvitacionCorreoResultado> ProcesarEnvioCorreoAsync(
            string codigoSala,
            string correo)
        {
            try
            {
                _logger.InfoFormat("Enviando invitacion a: {0}", correo);
                var resultado = await _invitacionesServicio
                    .EnviarInvitacionAsync(codigoSala, correo)
                    .ConfigureAwait(false);

                if (resultado != null && resultado.OperacionExitosa)
                {
                    return InvitacionCorreoResultado.Exito(Lang.invitarCorreoTextoEnviado);
                }

                return InvitacionCorreoResultado.Fallo(
                    resultado?.Mensaje ?? Lang.errorTextoEnviarCorreo);
            }
            catch (ServicioExcepcion excepcion)
            {
                _logger.Error("Error de servicio al enviar correo.", excepcion);
                return InvitacionCorreoResultado.Fallo(Lang.errorTextoErrorProcesarSolicitud);
            }
        }
    }
}