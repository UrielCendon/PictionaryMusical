using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PictionaryMusicalServidor.Servicios.Servicios
{
    /// <summary>
    /// Implementacion del servicio de gestion de invitaciones a salas de juego.
    /// Maneja el envio de invitaciones por correo electronico a usuarios para unirse a salas.
    /// </summary>
    public class InvitacionesManejador : IInvitacionesManejador
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(InvitacionesManejador));

        private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(500);

        private static readonly Regex CorreoRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant,
            RegexTimeout);

        private readonly IContextoFactoria _contextoFactory;
        private readonly ISalasManejador _salasManejador;
        private readonly ICorreoInvitacionNotificador _correoNotificador;

        public InvitacionesManejador() : this(
            new ContextoFactoria(),
            new SalasManejador(),
            new CorreoInvitacionNotificador())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias.
        /// </summary>
        public InvitacionesManejador(
            IContextoFactoria contextoFactory,
            ISalasManejador salasManejador,
            ICorreoInvitacionNotificador correoNotificador)
        {
            _contextoFactory = contextoFactory
                ?? throw new ArgumentNullException(nameof(contextoFactory));

            _salasManejador = salasManejador
                ?? throw new ArgumentNullException(nameof(salasManejador));

            _correoNotificador = correoNotificador
                ?? throw new ArgumentNullException(nameof(correoNotificador));
        }

        /// <summary>
        /// Envia una invitacion a una sala de juego a un usuario via correo electronico.
        /// Valida el correo, verifica que la sala exista y que el usuario no este ya en la sala.
        /// </summary>
        /// <param name="invitacion">Datos de la invitacion con codigo de sala y correo.</param>
        /// <returns>Resultado del envio indicando exito o fallo con mensaje descriptivo.</returns>
        public async Task<ResultadoOperacionDTO> EnviarInvitacionAsync(
            InvitacionSalaDTO invitacion)
        {
            try
            {
                ValidarSolicitud(invitacion);

                string codigoSala = invitacion.CodigoSala.Trim();
                string correo = invitacion.Correo.Trim();
                string idioma = invitacion.Idioma;

                var sala = _salasManejador.ObtenerSalaPorCodigo(codigoSala);
                ValidarSala(sala);

                if (sala.Jugadores != null && sala.Jugadores.Count > 0 &&
                    await UsuarioYaEnSalaAsync(correo, sala))
                {
                    throw new InvalidOperationException(
                        MensajesError.Cliente.CorreoJugadorEnSala);
                }

                bool enviado = await _correoNotificador.EnviarInvitacionAsync(
                    correo,
                    sala.Codigo,
                    sala.Creador,
                    idioma).ConfigureAwait(false);

                if (!enviado)
                {
                    return CrearFallo(MensajesError.Cliente.ErrorEnviarInvitacionCorreo);
                }

                return new ResultadoOperacionDTO
                {
                    OperacionExitosa = true,
                    Mensaje = MensajesError.Cliente.InvitacionEnviadaExito
                };
            }
            catch (FaultException)
            {
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Warn("Operacion invalida al enviar invitacion.", ex);
                return CrearFallo(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn("Operacion invalida al enviar invitacion.", ex);
                return CrearFallo(ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Error de base de datos al enviar invitacion.", ex);
                return CrearFallo(MensajesError.Cliente.ErrorProcesarInvitacion);
            }
            catch (DataException ex)
            {
                _logger.Error("Error de datos al enviar invitacion.", ex);
                return CrearFallo(MensajesError.Cliente.ErrorProcesarInvitacion);
            }
            catch (Exception ex)
            {
                _logger.Error("Operacion invalida al enviar invitacion.", ex);
                return CrearFallo(MensajesError.Cliente.ErrorInesperadoInvitacion);
            }
        }

        private static void ValidarSolicitud(InvitacionSalaDTO invitacion)
        {
            if (invitacion == null)
            {
                throw new ArgumentException(
                    MensajesError.Cliente.SolicitudInvitacionInvalida);
            }

            string codigoSala = invitacion.CodigoSala?.Trim();
            string correo = invitacion.Correo?.Trim();

            if (string.IsNullOrWhiteSpace(codigoSala) || string.IsNullOrWhiteSpace(correo))
            {
                throw new ArgumentException(
                    MensajesError.Cliente.DatosInvitacionInvalidos);
            }

            if (!CorreoRegex.IsMatch(correo))
            {
                throw new ArgumentException(MensajesError.Cliente.CorreoInvalido);
            }
        }

        private static void ValidarSala(dynamic sala)
        {
            if (sala == null)
            {
                throw new InvalidOperationException(MensajesError.Cliente.SalaNoEncontrada);
            }
        }

        private async Task<bool> UsuarioYaEnSalaAsync(string correo, dynamic sala)
        {
            if (sala.Jugadores == null || sala.Jugadores.Count == 0)
            {
                return false;
            }

            using (var contexto = _contextoFactory.CrearContexto())
            {
                var usuario = await contexto.Usuario
                    .Include(u => u.Jugador)
                    .FirstOrDefaultAsync(u => u.Jugador.Correo == correo);

                if (string.IsNullOrWhiteSpace(usuario?.Nombre_Usuario))
                {
                    return false;
                }

                var listaJugadores = (IEnumerable<string>)sala.Jugadores;
                return listaJugadores.Contains(
                    usuario.Nombre_Usuario,
                    StringComparer.OrdinalIgnoreCase);
            }
        }

        private static ResultadoOperacionDTO CrearFallo(string mensaje)
        {
            return new ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = mensaje
            };
        }
    }
}