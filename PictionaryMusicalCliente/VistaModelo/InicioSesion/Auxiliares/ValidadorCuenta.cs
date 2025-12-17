using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using System.Collections.Generic;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.InicioSesion.Auxiliares
{
    /// <summary>
    /// Realiza la validacion de datos para la creacion de cuenta.
    /// </summary>
    public sealed class ValidadorCuenta
    {
        /// <summary>
        /// Valida los campos para la creacion de una nueva cuenta.
        /// </summary>
        /// <param name="usuario">Nombre de usuario.</param>
        /// <param name="nombre">Nombre del usuario.</param>
        /// <param name="apellido">Apellido del usuario.</param>
        /// <param name="correo">Correo electronico.</param>
        /// <param name="contrasena">Contrasena.</param>
        /// <param name="avatarId">ID del avatar seleccionado.</param>
        /// <returns>
        /// Resultado con campos invalidos y primer mensaje de error.
        /// </returns>
        public ResultadoValidacionCampos ValidarCamposCreacion(
            string usuario,
            string nombre,
            string apellido,
            string correo,
            string contrasena,
            int avatarId)
        {
            var camposInvalidos = new List<string>();
            string primerMensajeError = null;

            ValidarCampo(
                ValidadorEntrada.ValidarUsuario(usuario),
                "Usuario",
                camposInvalidos,
                ref primerMensajeError);

            ValidarCampo(
                ValidadorEntrada.ValidarNombre(nombre),
                "Nombre",
                camposInvalidos,
                ref primerMensajeError);

            ValidarCampo(
                ValidadorEntrada.ValidarApellido(apellido),
                "Apellido",
                camposInvalidos,
                ref primerMensajeError);

            ValidarCampo(
                ValidadorEntrada.ValidarCorreo(correo),
                "Correo",
                camposInvalidos,
                ref primerMensajeError);

            ValidarCampo(
                ValidadorEntrada.ValidarContrasena(contrasena),
                "Contrasena",
                camposInvalidos,
                ref primerMensajeError);

            if (avatarId <= 0)
            {
                camposInvalidos.Add("Avatar");
                primerMensajeError ??= Lang.errorTextoSeleccionAvatarValido;
            }

            return new ResultadoValidacionCampos(camposInvalidos, primerMensajeError);
        }

        /// <summary>
        /// Valida los campos de inicio de sesion.
        /// </summary>
        /// <param name="identificador">Identificador de usuario.</param>
        /// <param name="contrasena">Contrasena.</param>
        /// <returns>Lista de campos invalidos.</returns>
        public List<string> ValidarCamposInicioSesion(
            string identificador,
            string contrasena)
        {
            var camposInvalidos = new List<string>();
            bool identificadorIngresado = !string.IsNullOrWhiteSpace(identificador);
            bool contrasenaIngresada = !string.IsNullOrWhiteSpace(contrasena);

            if (!identificadorIngresado)
            {
                camposInvalidos.Add("Identificador");
            }

            if (!contrasenaIngresada)
            {
                camposInvalidos.Add("Contrasena");
            }

            return camposInvalidos;
        }

        private static void ValidarCampo(
            DTOs.ResultadoOperacionDTO resultado,
            string nombreCampo,
            List<string> invalidos,
            ref string primerError)
        {
            if (resultado?.OperacionExitosa != true)
            {
                invalidos.Add(nombreCampo);
                primerError ??= resultado?.Mensaje;
            }
        }
    }

    /// <summary>
    /// Encapsula el resultado de una validacion de campos.
    /// </summary>
    public sealed class ResultadoValidacionCampos
    {
        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="ResultadoValidacionCampos"/>.
        /// </summary>
        /// <param name="camposInvalidos">Lista de campos invalidos.</param>
        /// <param name="primerMensajeError">Primer mensaje de error.</param>
        public ResultadoValidacionCampos(
            List<string> camposInvalidos,
            string primerMensajeError)
        {
            CamposInvalidos = camposInvalidos ?? new List<string>();
            PrimerMensajeError = primerMensajeError;
        }

        /// <summary>
        /// Obtiene la lista de campos invalidos.
        /// </summary>
        public List<string> CamposInvalidos { get; }

        /// <summary>
        /// Obtiene el primer mensaje de error encontrado.
        /// </summary>
        public string PrimerMensajeError { get; }

        /// <summary>
        /// Indica si todos los campos son validos.
        /// </summary>
        public bool EsValido => CamposInvalidos.Count == 0;
    }
}
