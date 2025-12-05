using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.VistaModelo.Amigos;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Encapsula la logica de invitaciones para reducir la carga del ViewModel principal
    /// y permitir inyectar dependencias en pruebas.
    /// </summary>
    public class InvitacionSalaServicio : IInvitacionSalaServicio
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InvitacionSalaServicio));

        private readonly IInvitacionesServicio _invitacionesServicio;
        private readonly IListaAmigosServicio _listaAmigosServicio;
        private readonly IPerfilServicio _perfilServicio;
        private bool _disposed;

        public InvitacionSalaServicio(
            IInvitacionesServicio invitacionesServicio,
            IListaAmigosServicio listaAmigosServicio,
            IPerfilServicio perfilServicio)
        {
            _invitacionesServicio = invitacionesServicio ??
                throw new ArgumentNullException(nameof(invitacionesServicio));
            _listaAmigosServicio = listaAmigosServicio ??
                throw new ArgumentNullException(nameof(listaAmigosServicio));
            _perfilServicio = perfilServicio ??
                throw new ArgumentNullException(nameof(perfilServicio));
        }

        public async Task<InvitacionCorreoResultado> InvitarPorCorreoAsync(
            string codigoSala,
            string correo)
        {
            if (string.IsNullOrWhiteSpace(codigoSala))
            {
                throw new ArgumentNullException(nameof(codigoSala));
            }

            string correoNormalizado = correo?.Trim();
            if (string.IsNullOrWhiteSpace(correoNormalizado))
            {
                return InvitacionCorreoResultado.Fallo(Lang.errorTextoCorreoInvalido);
            }

            var resultadoValidacion = ValidacionEntrada.ValidarCorreo(correoNormalizado);
            if (!resultadoValidacion.OperacionExitosa)
            {
                return InvitacionCorreoResultado.Fallo(
                    resultadoValidacion.Mensaje ?? Lang.errorTextoCorreoInvalido);
            }

            try
            {
                Logger.InfoFormat("Enviando invitación por correo a: {0}", correoNormalizado);

                var resultado = await _invitacionesServicio
                    .EnviarInvitacionAsync(codigoSala, correoNormalizado)
                    .ConfigureAwait(false);

                if (resultado != null && resultado.OperacionExitosa)
                {
                    return InvitacionCorreoResultado.Exito(Lang.invitarCorreoTextoEnviado);
                }

                Logger.WarnFormat(
                    "Fallo al enviar invitación: {0}",
                    resultado?.Mensaje);

                return InvitacionCorreoResultado.Fallo(
                    resultado?.Mensaje ?? Lang.errorTextoEnviarCorreo);
            }
            catch (ServicioExcepcion ex)
            {
                Logger.Error("Excepción de servicio al enviar invitación.", ex);
                return InvitacionCorreoResultado.Fallo(
                    ex.Message ?? Lang.errorTextoEnviarCorreo);
            }
            catch (ArgumentException ex)
            {
                Logger.Error("Error de argumento al enviar invitación.", ex);
                return InvitacionCorreoResultado.Fallo(
                    ex.Message ?? Lang.errorTextoErrorProcesarSolicitud);
            }
            catch (Exception ex)
            {
                Logger.Error("Error inesperado al invitar.", ex);
                return InvitacionCorreoResultado.Fallo(
                    Lang.errorTextoErrorProcesarSolicitud);
            }
        }

        public async Task<InvitacionAmigosResultado> ObtenerInvitacionAmigosAsync(
            string codigoSala,
            string nombreUsuarioSesion,
            ISet<int> amigosInvitados,
            Action<string> mostrarMensaje)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuarioSesion))
            {
                Logger.Warn("Intento de invitar amigos sin usuario de sesión.");
                return InvitacionAmigosResultado.Fallo(Lang.errorTextoErrorProcesarSolicitud);
            }

            IReadOnlyList<DTOs.AmigoDTO> amigos;

            try
            {
                amigos = await _listaAmigosServicio
                    .ObtenerAmigosAsync(nombreUsuarioSesion)
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is ServicioExcepcion || ex is ArgumentException)
            {
                Logger.Error("Error al obtener lista de amigos para invitar.", ex);
                return InvitacionAmigosResultado.Fallo(
                    ex.Message ?? Lang.errorTextoErrorProcesarSolicitud);
            }

            if (amigos == null || amigos.Count == 0)
            {
                return InvitacionAmigosResultado.Fallo(Lang.invitarAmigosTextoSinAmigos);
            }

            var vistaModelo = new InvitarAmigosVistaModelo(
                amigos,
                _invitacionesServicio,
                _perfilServicio,
                codigoSala,
                id => amigosInvitados?.Contains(id) ?? false,
                id => amigosInvitados?.Add(id),
                mostrarMensaje ?? (_ => { }));

            return InvitacionAmigosResultado.Exito(vistaModelo);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            (_invitacionesServicio as IDisposable)?.Dispose();
            (_listaAmigosServicio as IDisposable)?.Dispose();
            (_perfilServicio as IDisposable)?.Dispose();

            _disposed = true;
        }
    }
}