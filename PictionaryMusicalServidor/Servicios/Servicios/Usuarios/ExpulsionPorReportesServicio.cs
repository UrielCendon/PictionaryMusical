using System;
using System.Linq;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Servicio encargado de expulsar jugadores de salas activas cuando alcanzan 
    /// el limite de reportes.
    /// </summary>
    public class ExpulsionPorReportesServicio : IExpulsionPorReportesServicio
    {
        private const int LimiteReportesParaExpulsion = 3;
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(ExpulsionPorReportesServicio));

        /// <summary>
        /// Expulsa a un jugador de todas las salas activas si alcanza el limite de reportes.
        /// </summary>
        /// <param name="idReportado">Identificador del usuario reportado.</param>
        /// <param name="nombreUsuarioReportado">Nombre del usuario reportado.</param>
        /// <param name="totalReportes">Total de reportes recibidos.</param>
        public void ExpulsarSiAlcanzaLimite(
            int idReportado, 
            string nombreUsuarioReportado, 
            int totalReportes)
        {
            if (idReportado <= 0 || totalReportes < LimiteReportesParaExpulsion)
            {
                return;
            }

            string nombreNormalizado = EntradaComunValidador.NormalizarTexto(
                nombreUsuarioReportado);
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                return;
            }

            _logger.WarnFormat(
                "Usuario {0} alcanzo el limite de reportes ({1}). Iniciando expulsion de salas activas.",
                nombreNormalizado,
                LimiteReportesParaExpulsion);

            try
            {
                ExpulsarDeSalasActivas(nombreNormalizado);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    "Error al intentar expulsar jugador con limite de reportes.",
                    excepcion);
            }
        }

        private static void ExpulsarDeSalasActivas(string nombreUsuario)
        {
            var salas = SalasManejador.ObtenerListaSalas();
            if (salas == null || salas.Count == 0)
            {
                return;
            }

            foreach (var sala in salas)
            {
                bool jugadorEnSala = sala?.Jugadores?.Any(jugador =>
                {
                    string jugadorNormalizado = EntradaComunValidador.NormalizarTexto(jugador);
                    return string.Equals(
                        jugadorNormalizado,
                        EntradaComunValidador.NormalizarTexto(nombreUsuario),
                        StringComparison.OrdinalIgnoreCase);
                }) == true;

                if (!jugadorEnSala)
                {
                    continue;
                }

                ExpulsarDeSalaIndividual(sala, nombreUsuario);
            }
        }

        private static void ExpulsarDeSalaIndividual(
            SalaDTO sala, 
            string nombreUsuario)
        {
            try
            {
                var salasManejador = new SalasManejador();

                string creadorNormalizado = EntradaComunValidador.NormalizarTexto(sala?.Creador);
                string usuarioNormalizado = EntradaComunValidador.NormalizarTexto(nombreUsuario);

                bool esCreador = !string.IsNullOrWhiteSpace(creadorNormalizado) &&
                    string.Equals(
                        creadorNormalizado,
                        usuarioNormalizado,
                        StringComparison.OrdinalIgnoreCase);

                if (esCreador)
                {
                    salasManejador.AbandonarSala(sala.Codigo, nombreUsuario);
                }
                else
                {
                    salasManejador.ExpulsarJugador(
                        sala.Codigo,
                        sala.Creador,
                        nombreUsuario);
                }
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(string.Format(
                    "No se pudo expulsar a {0} de la sala {1}.",
                    nombreUsuario,
                    sala?.Codigo), excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(string.Format(
                    "Error inesperado al expulsar a {0} de la sala {1}.",
                    nombreUsuario,
                    sala?.Codigo), excepcion);
            }
        }
    }
}
