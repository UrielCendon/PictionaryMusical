using System;
using System.ServiceModel;
using Datos.Modelo;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Servicio para operaciones de amistad en base de datos.
    /// Encapsula las transacciones de base de datos relacionadas con amistades.
    /// </summary>
    public class OperacionAmistadServicio : IOperacionAmistadServicio
    {
        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;
        private readonly IAmistadServicio _amistadServicio;

        /// <summary>
        /// Constructor por defecto para uso en WCF.
        /// </summary>
        public OperacionAmistadServicio() 
            : this(new ContextoFactoria(), new RepositorioFactoria(), new AmistadServicio())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        /// <param name="amistadServicio">Servicio de amistad.</param>
        public OperacionAmistadServicio(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria,
            IAmistadServicio amistadServicio)
        {
            _contextoFactoria = contextoFactoria ??
                throw new ArgumentNullException(nameof(contextoFactoria));
            _repositorioFactoria = repositorioFactoria ??
                throw new ArgumentNullException(nameof(repositorioFactoria));
            _amistadServicio = amistadServicio ??
                throw new ArgumentNullException(nameof(amistadServicio));
        }

        /// <summary>
        /// Obtiene los datos de un usuario para la suscripcion a notificaciones.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        /// <returns>Objeto con los datos del usuario para suscripcion.</returns>
        public DatosSuscripcionUsuario ObtenerDatosUsuarioSuscripcion(string nombreUsuario)
        {
            using (var contexto = _contextoFactoria.CrearContexto())
            {
                var usuarioRepositorio =
                    _repositorioFactoria.CrearUsuarioRepositorio(contexto);
                var usuario = usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuario);

                if (usuario == null)
                {
                    throw new FaultException(MensajesError.Cliente.UsuarioNoEncontrado);
                }

                string nombreNormalizado = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                    usuario.Nombre_Usuario,
                    nombreUsuario);

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                {
                    throw new FaultException(MensajesError.Cliente.UsuarioNoEncontrado);
                }

                return new DatosSuscripcionUsuario
                {
                    IdUsuario = usuario.idUsuario,
                    NombreNormalizado = nombreNormalizado
                };
            }
        }

        /// <summary>
        /// Ejecuta la creacion de una solicitud de amistad en la base de datos.
        /// </summary>
        /// <param name="nombreEmisor">Nombre del usuario emisor.</param>
        /// <param name="nombreReceptor">Nombre del usuario receptor.</param>
        /// <returns>Objeto con los usuarios emisor y receptor.</returns>
        public ResultadoCreacionSolicitud EjecutarCreacionSolicitud(
            string nombreEmisor,
            string nombreReceptor)
        {
            using (var contexto = _contextoFactoria.CrearContexto())
            {
                var (emisor, receptor) = ObtenerUsuariosParaInteraccion(
                    contexto,
                    nombreEmisor,
                    nombreReceptor);

                _amistadServicio.CrearSolicitud(emisor.idUsuario, receptor.idUsuario);

                return new ResultadoCreacionSolicitud
                {
                    Emisor = emisor,
                    Receptor = receptor
                };
            }
        }

        /// <summary>
        /// Ejecuta la aceptacion de una solicitud de amistad en la base de datos.
        /// </summary>
        /// <param name="nombreEmisor">Nombre del usuario emisor.</param>
        /// <param name="nombreReceptor">Nombre del usuario receptor.</param>
        /// <returns>Objeto con los nombres normalizados de ambos usuarios.</returns>
        public ResultadoAceptacionSolicitud EjecutarAceptacionSolicitud(
            string nombreEmisor,
            string nombreReceptor)
        {
            using (var contexto = _contextoFactoria.CrearContexto())
            {
                var (usuarioEmisor, usuarioReceptor) = ObtenerUsuariosParaInteraccion(
                    contexto,
                    nombreEmisor,
                    nombreReceptor);

                _amistadServicio.AceptarSolicitud(
                    usuarioEmisor.idUsuario,
                    usuarioReceptor.idUsuario);

                string nombreNormalizadoEmisor = 
                    EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                        usuarioEmisor.Nombre_Usuario,
                        nombreEmisor);

                string nombreNormalizadoReceptor = 
                    EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                        usuarioReceptor.Nombre_Usuario,
                        nombreReceptor);

                return new ResultadoAceptacionSolicitud
                {
                    NombreNormalizadoEmisor = nombreNormalizadoEmisor,
                    NombreNormalizadoReceptor = nombreNormalizadoReceptor
                };
            }
        }

        /// <summary>
        /// Ejecuta la eliminacion de una amistad en la base de datos.
        /// </summary>
        /// <param name="nombrePrimerUsuario">Nombre del primer usuario.</param>
        /// <param name="nombreSegundoUsuario">Nombre del segundo usuario.</param>
        /// <returns>Objeto con el resultado de la eliminacion.</returns>
        public ResultadoEliminacionAmistad EjecutarEliminacion(
            string nombrePrimerUsuario, 
            string nombreSegundoUsuario)
        {
            using (var contexto = _contextoFactoria.CrearContexto())
            {
                var (primerUsuario, segundoUsuario) = ObtenerUsuariosParaInteraccion(
                    contexto,
                    nombrePrimerUsuario,
                    nombreSegundoUsuario);

                var relacion = _amistadServicio.EliminarAmistad(
                    primerUsuario.idUsuario,
                    segundoUsuario.idUsuario);

                string nombrePrimerUsuarioNormalizado = 
                    EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                        primerUsuario.Nombre_Usuario,
                        nombrePrimerUsuario);

                string nombreSegundoUsuarioNormalizado = 
                    EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                        segundoUsuario.Nombre_Usuario,
                        nombreSegundoUsuario);

                return new ResultadoEliminacionAmistad
                {
                    Relacion = relacion,
                    NombrePrimerUsuarioNormalizado = nombrePrimerUsuarioNormalizado,
                    NombreSegundoUsuarioNormalizado = nombreSegundoUsuarioNormalizado
                };
            }
        }

        private (Usuario Emisor, Usuario Receptor) ObtenerUsuariosParaInteraccion(
            BaseDatosPruebaEntities contexto,
            string nombreEmisor,
            string nombreReceptor)
        {
            var usuarioRepositorio =
                _repositorioFactoria.CrearUsuarioRepositorio(contexto);
            var usuarioEmisor = usuarioRepositorio.ObtenerPorNombreUsuario(nombreEmisor);
            var usuarioReceptor = usuarioRepositorio.ObtenerPorNombreUsuario(nombreReceptor);

            ValidarUsuariosExistentes(usuarioEmisor, usuarioReceptor);

            return (usuarioEmisor, usuarioReceptor);
        }

        private static void ValidarUsuariosExistentes(Usuario emisor, Usuario receptor)
        {
            if (emisor == null)
            {
                throw new FaultException(
                    MensajesError.Cliente.UsuariosEspecificadosNoExisten);
            }
            if (receptor == null)
            {
                throw new FaultException(
                    MensajesError.Cliente.UsuariosEspecificadosNoExisten);
            }
        }
    }
}
