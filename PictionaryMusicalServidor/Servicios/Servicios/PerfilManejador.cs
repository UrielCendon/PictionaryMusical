using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.ServiceModel;
using Datos.Modelo;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using log4net;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios
{
    /// <summary>
    /// Implementacion del servicio de gestion de perfiles de usuario.
    /// Maneja consulta y actualizacion de datos de perfil incluyendo informacion personal y 
    /// redes sociales.
    /// Verifica que el usuario exista y tenga jugador asociado antes de operar.
    /// </summary>
    public class PerfilManejador : IPerfilManejador
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(PerfilManejador));
        private readonly IContextoFactory _contextoFactory;

        public PerfilManejador() : this(new ContextoFactory())
        {
        }

        public PerfilManejador(IContextoFactory contextoFactory)
        {
            _contextoFactory = contextoFactory ??
                throw new ArgumentNullException(nameof(contextoFactory));
        }

        /// <summary>
        /// Obtiene el perfil completo de un usuario incluyendo datos de jugador y redes sociales.
        /// Valida que el usuario exista y tenga un jugador asociado.
        /// </summary>
        /// <param name="idUsuario">Identificador unico del usuario.</param>
        /// <returns>Datos completos del perfil del usuario.</returns>
        public UsuarioDTO ObtenerPerfil(int idUsuario)
        {
            try
            {
                if (idUsuario <= 0)
                {
                    throw new ArgumentException(MensajesError.Cliente.DatosInvalidos);
                }

                using (var contexto = _contextoFactory.CrearContexto())
                {
                    var usuario = contexto.Usuario
                        .Include(u => u.Jugador.RedSocial)
                        .FirstOrDefault(u => u.idUsuario == idUsuario);

                    if (usuario == null)
                    {
                        throw new InvalidOperationException(
                            MensajesError.Cliente.UsuarioNoEncontrado);
                    }

                    var jugador = usuario.Jugador;

                    if (jugador == null)
                    {
                        throw new InvalidOperationException(
                            MensajesError.Cliente.JugadorNoAsociado);
                    }

                    var redSocial = jugador.RedSocial.FirstOrDefault();

                    return new UsuarioDTO
                    {
                        UsuarioId = usuario.idUsuario,
                        JugadorId = jugador.idJugador,
                        NombreUsuario = usuario.Nombre_Usuario,
                        Nombre = jugador.Nombre,
                        Apellido = jugador.Apellido,
                        Correo = jugador.Correo,
                        AvatarId = jugador.Id_Avatar ?? 0,
                        Instagram = redSocial?.Instagram,
                        Facebook = redSocial?.facebook,
                        X = redSocial?.x,
                        Discord = redSocial?.discord
                    };
                }
            }
            catch (ArgumentException ex)
            {
                _logger.Warn("Operacion invalida al obtener perfil.", ex);
                throw new FaultException(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn("Operacion invalida al obtener perfil.", ex);
                throw new FaultException(ex.Message);
            }
            catch (EntityException ex)
            {
                _logger.Error("Error de base de datos al obtener perfil.", ex);
                throw new FaultException(MensajesError.Cliente.ErrorObtenerPerfil);
            }
            catch (DataException ex)
            {
                _logger.Error("Error de datos al obtener perfil.", ex);
                throw new FaultException(MensajesError.Cliente.ErrorObtenerPerfil);
            }
            catch (Exception ex)
            {
                _logger.Error("Operacion invalida al obtener perfil.", ex);
                throw new FaultException(MensajesError.Cliente.ErrorObtenerPerfil);
            }
        }

        /// <summary>
        /// Actualiza el perfil de un usuario con nuevos datos personales y de redes sociales.
        /// Valida los datos de entrada, verifica que el usuario exista y actualiza jugador.
        /// </summary>
        /// <param name="solicitud">Datos actualizados del perfil.</param>
        /// <returns>Resultado de la actualizacion del perfil.</returns>
        public ResultadoOperacionDTO ActualizarPerfil(ActualizacionPerfilDTO solicitud)
        {
            try
            {
                var validacion = EntradaComunValidador.ValidarActualizacionPerfil(solicitud);
                if (!validacion.OperacionExitosa)
                {
                    return validacion;
                }

                using (var contexto = _contextoFactory.CrearContexto())
                {
                    var usuario = contexto.Usuario
                        .Include(u => u.Jugador.RedSocial)
                        .FirstOrDefault(u => u.idUsuario == solicitud.UsuarioId);

                    if (usuario == null)
                    {
                        throw new InvalidOperationException(
                            MensajesError.Cliente.UsuarioNoEncontrado);
                    }

                    var jugador = usuario.Jugador;

                    if (jugador == null)
                    {
                        throw new InvalidOperationException(
                            MensajesError.Cliente.JugadorNoAsociado);
                    }

                    ActualizarDatosJugador(jugador, solicitud);
                    ActualizarRedesSociales(contexto, jugador, solicitud);

                    contexto.SaveChanges();

                    _logger.Info("Perfil actualizado exitosamente.");

                    return new ResultadoOperacionDTO
                    {
                        OperacionExitosa = true,
                        Mensaje = MensajesError.Cliente.PerfilActualizadoExito
                    };
                }
            }
            catch (ArgumentException ex)
            {
                _logger.Warn("Operacion invalida al actualizar perfil.", ex);
                return CrearResultadoFallo(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn("Operacion invalida al actualizar perfil.", ex);
                return CrearResultadoFallo(ex.Message);
            }
            catch (DbEntityValidationException ex)
            {
                _logger.Error("Validacion de entidad fallida al actualizar perfil.", ex);
                return CrearResultadoFallo(MensajesError.Cliente.ErrorActualizarPerfil);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error("Error de actualizacion de BD al actualizar perfil.", ex);
                return CrearResultadoFallo(MensajesError.Cliente.ErrorActualizarPerfil);
            }
            catch (EntityException ex)
            {
                _logger.Error("Error de base de datos al actualizar perfil.", ex);
                return CrearResultadoFallo(MensajesError.Cliente.ErrorActualizarPerfil);
            }
            catch (DataException ex)
            {
                _logger.Error("Error de datos al actualizar perfil.", ex);
                return CrearResultadoFallo(MensajesError.Cliente.ErrorActualizarPerfil);
            }
            catch (Exception ex)
            {
                _logger.Error("Operacion invalida al actualizar perfil.", ex);
                return CrearResultadoFallo(MensajesError.Cliente.ErrorActualizarPerfil);
            }
        }

        private void ActualizarDatosJugador(Jugador jugador, ActualizacionPerfilDTO solicitud)
        {
            jugador.Nombre = solicitud.Nombre;
            jugador.Apellido = solicitud.Apellido;
            jugador.Id_Avatar = solicitud.AvatarId;
        }

        private void ActualizarRedesSociales(
            BaseDatosPruebaEntities contexto,
            Jugador jugador,
            ActualizacionPerfilDTO solicitud)
        {
            var redSocial = jugador.RedSocial.FirstOrDefault();
            if (redSocial == null)
            {
                redSocial = new RedSocial
                {
                    Jugador_idJugador = jugador.idJugador
                };
                contexto.RedSocial.Add(redSocial);
                jugador.RedSocial.Add(redSocial);
            }

            redSocial.Instagram = solicitud.Instagram;
            redSocial.facebook = solicitud.Facebook;
            redSocial.x = solicitud.X;
            redSocial.discord = solicitud.Discord;
        }

        private static ResultadoOperacionDTO CrearResultadoFallo(string mensaje)
        {
            return new ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = mensaje
            };
        }
    }
}