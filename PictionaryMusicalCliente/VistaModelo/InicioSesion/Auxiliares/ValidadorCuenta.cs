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
        /// <param name="datos">Datos de creacion de cuenta a validar.</param>
        /// <returns>
        /// Resultado con campos invalidos y primer mensaje de error.
        /// </returns>
        public ResultadoValidacionCampos ValidarCamposCreacion(DatosCreacionCuenta datos)
        {
            var contexto = new ValidacionContexto();

            ValidarCampo(
                ValidadorEntrada.ValidarUsuario(datos.Usuario),
                "Usuario",
                contexto);

            ValidarCampo(
                ValidadorEntrada.ValidarNombre(datos.Nombre),
                "Nombre",
                contexto);

            ValidarCampo(
                ValidadorEntrada.ValidarApellido(datos.Apellido),
                "Apellido",
                contexto);

            ValidarCampo(
                ValidadorEntrada.ValidarCorreo(datos.Correo),
                "Correo",
                contexto);

            ValidarCampo(
                ValidadorEntrada.ValidarContrasena(datos.Contrasena),
                "Contrasena",
                contexto);

            if (datos.AvatarId <= 0)
            {
                contexto.AgregarCampoInvalido("Avatar", Lang.errorTextoSeleccionAvatarValido);
            }

            return new ResultadoValidacionCampos(
                contexto.CamposInvalidos, 
                contexto.PrimerMensajeError);
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
            ValidacionContexto contexto)
        {
            if (resultado?.OperacionExitosa != true)
            {
                contexto.AgregarCampoInvalido(nombreCampo, resultado?.Mensaje);
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
