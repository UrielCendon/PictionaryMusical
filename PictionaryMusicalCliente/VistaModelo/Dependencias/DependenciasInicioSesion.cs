using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias de autenticacion para el inicio de sesion.
    /// </summary>
    /// <remarks>
    /// Incluye servicios de login, cambio/recuperacion de contrasena
    /// y gestion de usuario.
    /// </remarks>
    public class DependenciasInicioSesion
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias de autenticacion.
        /// </summary>
        /// <param name="inicioSesionServicio">
        /// Servicio para autenticar usuarios.
        /// </param>
        /// <param name="cambioContrasenaServicio">
        /// Servicio para cambiar contrasena.
        /// </param>
        /// <param name="recuperacionCuentaServicio">
        /// Servicio de recuperacion de cuenta.
        /// </param>
        /// <param name="localizacionServicio">Servicio de idiomas.</param>
        /// <param name="generadorNombres">Generador de nombres de invitado.</param>
        /// <param name="usuarioSesion">Datos del usuario autenticado.</param>
        /// <param name="salasServicioFactory">Fabrica de servicios de salas.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        public DependenciasInicioSesion(
            IInicioSesionServicio inicioSesionServicio,
            ICambioContrasenaServicio cambioContrasenaServicio,
            IRecuperacionCuentaServicio recuperacionCuentaServicio,
            ILocalizacionServicio localizacionServicio,
            INombreInvitadoGenerador generadorNombres,
            IUsuarioAutenticado usuarioSesion,
            Func<ISalasServicio> salasServicioFactory)
        {
            InicioSesionServicio = inicioSesionServicio ?? 
                throw new ArgumentNullException(nameof(inicioSesionServicio));
            CambioContrasenaServicio = cambioContrasenaServicio ?? 
                throw new ArgumentNullException(nameof(cambioContrasenaServicio));
            RecuperacionCuentaServicio = recuperacionCuentaServicio ?? 
                throw new ArgumentNullException(nameof(recuperacionCuentaServicio));
            LocalizacionServicio = localizacionServicio ?? 
                throw new ArgumentNullException(nameof(localizacionServicio));
            GeneradorNombres = generadorNombres ?? 
                throw new ArgumentNullException(nameof(generadorNombres));
            UsuarioSesion = usuarioSesion ?? 
                throw new ArgumentNullException(nameof(usuarioSesion));
            SalasServicioFactory = salasServicioFactory ?? 
                throw new ArgumentNullException(nameof(salasServicioFactory));
        }

        /// <summary>
        /// Servicio para autenticar usuarios en el sistema.
        /// </summary>
        public IInicioSesionServicio InicioSesionServicio { get; }

        /// <summary>
        /// Servicio para gestionar cambios de contrasena.
        /// </summary>
        public ICambioContrasenaServicio CambioContrasenaServicio { get; }

        /// <summary>
        /// Servicio de dialogo para recuperacion de cuenta.
        /// </summary>
        public IRecuperacionCuentaServicio RecuperacionCuentaServicio { get; }

        /// <summary>
        /// Servicio para gestionar idiomas y localizacion.
        /// </summary>
        public ILocalizacionServicio LocalizacionServicio { get; }

        /// <summary>
        /// Generador de nombres aleatorios para usuarios invitados.
        /// </summary>
        public INombreInvitadoGenerador GeneradorNombres { get; }

        /// <summary>
        /// Datos del usuario autenticado actualmente.
        /// </summary>
        public IUsuarioAutenticado UsuarioSesion { get; }

        /// <summary>
        /// Fabrica para crear instancias del servicio de salas.
        /// </summary>
        public Func<ISalasServicio> SalasServicioFactory { get; }
    }
}
