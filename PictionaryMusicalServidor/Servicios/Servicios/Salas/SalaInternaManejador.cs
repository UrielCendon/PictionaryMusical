using System;
using System.Collections.Generic;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Representa una sala de juego interna con su estado y jugadores.
    /// Gestiona el estado de los jugadores y delega las notificaciones al gestor inyectado.
    /// </summary>
    internal sealed class SalaInternaManejador
    {
        private const int MaximoJugadores = 4;

        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(SalaInternaManejador));

        private readonly object _sincrono = new object();
        private readonly IGestorNotificacionesSalaInterna _gestorNotificaciones;
        private readonly List<string> _jugadores;

        /// <summary>
        /// Inicializa una nueva instancia de la sala interna.
        /// </summary>
        /// <param name="codigo">Codigo unico de la sala.</param>
        /// <param name="creador">Nombre de usuario del creador.</param>
        /// <param name="configuracion">Configuracion de la partida.</param>
        /// <param name="gestorNotificaciones">Dependencia para el manejo de notificaciones.
        /// </param>
        public SalaInternaManejador(
            string codigo,
            string creador,
            ConfiguracionPartidaDTO configuracion,
            IGestorNotificacionesSalaInterna gestorNotificaciones)
        {
            Codigo = codigo;
            Creador = creador;
            Configuracion = configuracion;

            _gestorNotificaciones = gestorNotificaciones ??
                throw new ArgumentNullException(nameof(gestorNotificaciones));

            _jugadores = new List<string>();
            PartidaIniciada = false;
            PartidaFinalizada = false;
            DebeEliminarse = false;
        }

        public string Codigo { get; }
        public string Creador { get; }
        public ConfiguracionPartidaDTO Configuracion { get; }

        public bool DebeEliminarse { get; private set; }
        public bool PartidaIniciada { get; set; }
        public bool PartidaFinalizada { get; set; }

        /// <summary>
        /// Genera un objeto de transferencia de datos (DTO) con el estado actual de la sala.
        /// <returns>Instancia de SalaDTO.</returns>
        /// </summary>
        public SalaDTO ConvertirADto()
        {
            lock (_sincrono)
            {
                return new SalaDTO
                {
                    Codigo = Codigo,
                    Creador = Creador,
                    Configuracion = Configuracion,
                    Jugadores = new List<string>(_jugadores)
                };
            }
        }

        /// <summary>
        /// Intenta agregar un jugador a la sala y gestiona las notificaciones.
        /// Las notificaciones se ejecutan fuera del lock para evitar deadlocks.
        /// </summary>
        /// <param name="nombreUsuario">Usuario a agregar.</param>
        /// <param name="callback">Canal de comunicacion del usuario.</param>
        /// <param name="notificar">Indica si se debe notificar el ingreso a los demas.</param>
        /// <returns>DTO actualizado de la sala.</returns>
        public SalaDTO AgregarJugador(
            string nombreUsuario,
            ISalasManejadorCallback callback,
            bool notificar)
        {
            SalaDTO resultado;
            bool debeNotificar = false;

            lock (_sincrono)
            {
                if (_jugadores.Contains(nombreUsuario))
                {
                    _gestorNotificaciones.Registrar(nombreUsuario, callback);
                    return ConvertirADtoInterno();
                }

                ValidarCapacidad();

                _jugadores.Add(nombreUsuario);
                _gestorNotificaciones.Registrar(nombreUsuario, callback);
                resultado = ConvertirADtoInterno();
                debeNotificar = notificar;
            }

            if (debeNotificar)
            {
                _gestorNotificaciones.NotificarIngreso(Codigo, nombreUsuario, resultado);
            }

            return resultado;
        }

        /// <summary>
        /// Remueve a un jugador de la sala y gestiona la logica de abandono.
        /// Las notificaciones se ejecutan fuera del lock para evitar deadlocks.
        /// </summary>
        /// <param name="nombreUsuario">Usuario a remover.</param>
        public void RemoverJugador(string nombreUsuario)
        {
            AccionPostRemocion accionPendiente;

            lock (_sincrono)
            {
                if (!_jugadores.Contains(nombreUsuario))
                {
                    return;
                }

                _jugadores.Remove(nombreUsuario);
                accionPendiente = PrepararAccionPostRemocion(nombreUsuario);
                _gestorNotificaciones.Remover(nombreUsuario);
            }

            EjecutarAccionPostRemocion(accionPendiente);
        }

        private AccionPostRemocion PrepararAccionPostRemocion(string nombreUsuario)
        {
            bool esAnfitrion = string.Equals(
                nombreUsuario,
                Creador,
                StringComparison.OrdinalIgnoreCase);

            if (PartidaFinalizada && esAnfitrion)
            {
                _gestorNotificaciones.Limpiar();
                DebeEliminarse = true;
                return new AccionPostRemocion { Tipo = TipoAccionRemocion.Ninguna };
            }

            var salaActualizada = ConvertirADtoInterno();

            if (esAnfitrion)
            {
                _jugadores.Clear();
                _gestorNotificaciones.Limpiar();
                DebeEliminarse = true;
                return new AccionPostRemocion
                {
                    Tipo = TipoAccionRemocion.CancelarSala,
                    NombreUsuario = nombreUsuario,
                    SalaActualizada = salaActualizada
                };
            }

            if (_jugadores.Count == 0)
            {
                DebeEliminarse = true;
            }

            return new AccionPostRemocion
            {
                Tipo = TipoAccionRemocion.NotificarSalida,
                NombreUsuario = nombreUsuario,
                SalaActualizada = salaActualizada
            };
        }

        private void EjecutarAccionPostRemocion(AccionPostRemocion accion)
        {
            switch (accion.Tipo)
            {
                case TipoAccionRemocion.NotificarSalida:
                    _gestorNotificaciones.NotificarSalida(
                        Codigo, 
                        accion.NombreUsuario, 
                        accion.SalaActualizada);
                    break;
                case TipoAccionRemocion.CancelarSala:
                    _gestorNotificaciones.NotificarSalida(
                        Codigo, 
                        accion.NombreUsuario, 
                        accion.SalaActualizada);
                    _gestorNotificaciones.NotificarCancelacion(Codigo);
                    _logger.Info(MensajesError.Bitacora.SalaCanceladaSalidaAnfitrion);
                    break;
            }
        }

        private SalaDTO ConvertirADtoInterno()
        {
            return new SalaDTO
            {
                Codigo = Codigo,
                Creador = Creador,
                Configuracion = Configuracion,
                Jugadores = new List<string>(_jugadores)
            };
        }

        private enum TipoAccionRemocion
        {
            Ninguna,
            NotificarSalida,
            CancelarSala
        }

        private struct AccionPostRemocion
        {
            public TipoAccionRemocion Tipo;
            public string NombreUsuario;
            public SalaDTO SalaActualizada;
        }

        /// <summary>
        /// Expulsa forzosamente a un jugador de la sala si el solicitante es el creador.
        /// Las notificaciones se ejecutan fuera del lock para evitar deadlocks.
        /// </summary>
        /// <param name="nombreAnfitrion">Nombre de quien solicita la expulsion.</param>
        /// <param name="nombreJugadorAExpulsar">Nombre del jugador a expulsar.</param>
        public void ExpulsarJugador(string nombreAnfitrion, string nombreJugadorAExpulsar)
        {
            ExpulsionNotificacionParametros parametrosExpulsion;

            lock (_sincrono)
            {
                ValidarPermisosExpulsion(nombreAnfitrion, nombreJugadorAExpulsar);

                var callbackExpulsado = _gestorNotificaciones.ObtenerCallback(
                    nombreJugadorAExpulsar);

                _jugadores.Remove(nombreJugadorAExpulsar);
                _gestorNotificaciones.Remover(nombreJugadorAExpulsar);

                parametrosExpulsion = new ExpulsionNotificacionParametros
                {
                    CodigoSala = Codigo,
                    NombreExpulsado = nombreJugadorAExpulsar,
                    CallbackExpulsado = callbackExpulsado,
                    SalaActualizada = ConvertirADtoInterno()
                };
            }

            _gestorNotificaciones.NotificarExpulsion(parametrosExpulsion);

            _logger.InfoFormat(
                "Sala '{0}': Jugador expulsado y notificado exitosamente.",
                Codigo);
        }

        /// <summary>
        /// Banea a un jugador de la sala por exceso de reportes.
        /// No requiere validacion de permisos ya que es una accion del sistema.
        /// Las notificaciones se ejecutan fuera del lock para evitar deadlocks.
        /// </summary>
        /// <param name="nombreJugadorABanear">Nombre del jugador a banear.</param>
        public void BanearJugador(string nombreJugadorABanear)
        {
            BaneoNotificacionParametros parametrosBaneo;
            bool debeBanear;

            lock (_sincrono)
            {
                if (!_jugadores.Contains(nombreJugadorABanear))
                {
                    return;
                }

                var callbackBaneado = _gestorNotificaciones.ObtenerCallback(
                    nombreJugadorABanear);

                _jugadores.Remove(nombreJugadorABanear);
                _gestorNotificaciones.Remover(nombreJugadorABanear);

                parametrosBaneo = new BaneoNotificacionParametros
                {
                    CodigoSala = Codigo,
                    NombreBaneado = nombreJugadorABanear,
                    CallbackBaneado = callbackBaneado,
                    SalaActualizada = ConvertirADtoInterno()
                };
                debeBanear = true;
            }

            if (debeBanear)
            {
                _gestorNotificaciones.NotificarBaneo(parametrosBaneo);

                _logger.InfoFormat(
                    "Sala '{0}': Jugador baneado por reportes y notificado.",
                    Codigo);
            }
        }

        private void ValidarCapacidad()
        {
            if (_jugadores.Count >= MaximoJugadores)
            {
                throw new FaultException(MensajesError.Cliente.SalaLlena);
            }
        }

        private void ValidarPermisosExpulsion(string nombreAnfitrion, string objetivo)
        {
            if (!string.Equals(nombreAnfitrion, Creador, StringComparison.OrdinalIgnoreCase))
            {
                throw new FaultException(MensajesError.Cliente.SalaExpulsionRestringida);
            }

            if (string.Equals(objetivo, Creador, StringComparison.OrdinalIgnoreCase))
            {
                throw new FaultException(MensajesError.Cliente.SalaCreadorNoExpulsable);
            }

            if (!_jugadores.Contains(objetivo))
            {
                throw new FaultException(MensajesError.Cliente.SalaJugadorNoExiste);
            }
        }
    }
}
