using PictionaryMusicalCliente.Properties.Langs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.Utilidades
{
    /// <summary>
    /// Provee metodos estaticos para validar la entrada de datos del usuario.
    /// </summary>
    public static class ValidadorEntrada
    {
        private const int LongitudCodigoSala = 6;
        private const int SegundosTimeoutRegex = 1;
        private const string RecursoNombresInvitados = "invitadoNombres";

        private static readonly Regex CorreoRegex = new Regex(
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9]" +
            @"(?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?" +
            @"(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(SegundosTimeoutRegex));

        private static readonly Regex ContrasenaRegex = new Regex(
            @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-\[\]{};:'"",.\<>/?]).{8,15}$",
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(SegundosTimeoutRegex));

        /// <summary>
        /// Valida que el nombre de usuario no este vacio.
        /// </summary>
        public static DTOs.ResultadoOperacionDTO ValidarUsuario(string usuario)
        {
            return ValidarCampoObligatorio(usuario, Lang.errorTextoCampoObligatorio);
        }

        /// <summary>
        /// Valida que el nombre real no este vacio.
        /// </summary>
        public static DTOs.ResultadoOperacionDTO ValidarNombre(string nombre)
        {
            return ValidarCampoObligatorio(
                nombre,
                Lang.errorTextoNombreObligatorioLongitud);
        }

        /// <summary>
        /// Valida que el apellido no este vacio.
        /// </summary>
        public static DTOs.ResultadoOperacionDTO ValidarApellido(string apellido)
        {
            return ValidarCampoObligatorio(
                apellido,
                Lang.errorTextoApellidoObligatorioLongitud);
        }

        /// <summary>
        /// Valida el formato y presencia del correo electronico.
        /// </summary>
        public static DTOs.ResultadoOperacionDTO ValidarCorreo(string correo)
        {
            DTOs.ResultadoOperacionDTO resultado = ValidarCampoObligatorio(
                correo,
                Lang.errorTextoCorreoInvalido);

            if (!EsOperacionExitosa(resultado))
            {
                return resultado;
            }

            string correoNormalizado = correo.Trim();

            if (!CorreoRegex.IsMatch(correoNormalizado))
            {
                return CrearResultadoFallido(Lang.errorTextoCorreoInvalido);
            }

            return CrearResultadoExitoso();
        }

        /// <summary>
        /// Valida que la contrasena cumpla con los requisitos de seguridad.
        /// </summary>
        public static DTOs.ResultadoOperacionDTO ValidarContrasena(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena))
            {
                return CrearResultadoFallido(Lang.errorTextoCampoObligatorio);
            }

            string contrasenaNormalizada = contrasena.Trim();

            if (!ContrasenaRegex.IsMatch(contrasenaNormalizada))
            {
                return CrearResultadoFallido(Lang.errorTextoContrasenaFormato);
            }

            return CrearResultadoExitoso();
        }

        /// <summary>
        /// Valida que el codigo de sala tenga el formato correcto.
        /// Debe tener exactamente 6 caracteres numericos.
        /// </summary>
        public static DTOs.ResultadoOperacionDTO ValidarCodigoSala(string codigoSala)
        {
            if (string.IsNullOrWhiteSpace(codigoSala))
            {
                return CrearResultadoFallido(Lang.errorTextoCodigoSalaRequerido);
            }

            string codigoNormalizado = codigoSala.Trim();

            if (codigoNormalizado.Length != LongitudCodigoSala ||
                !codigoNormalizado.All(char.IsDigit))
            {
                return CrearResultadoFallido(Lang.errorTextoCodigoSalaRequerido);
            }

            return CrearResultadoExitoso();
        }

        /// <summary>
        /// Verifica si el nombre de usuario proporcionado corresponde a un 
        /// nombre reservado para invitados.
        /// </summary>
        /// <param name="nombreUsuario">El nombre de usuario a verificar.</param>
        /// <returns>True si es un nombre de invitado reservado, false de lo contrario.</returns>
        public static bool EsNombreInvitado(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return false;
            }

            HashSet<string> nombresInvitados = ObtenerNombresInvitados();
            return nombresInvitados.Contains(nombreUsuario.Trim());
        }

        /// <summary>
        /// Obtiene el conjunto de nombres reservados para invitados 
        /// de todos los idiomas configurados.
        /// </summary>
        /// <returns>
        /// HashSet con los nombres de invitados (comparacion insensible a mayusculas).
        /// </returns>
        public static HashSet<string> ObtenerNombresInvitados()
        {
            var nombres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            AgregarNombresDeCultura(nombres, CultureInfo.InvariantCulture);
            AgregarNombresDeCultura(nombres, new CultureInfo("es-MX"));
            AgregarNombresDeCultura(nombres, new CultureInfo("en-US"));

            return nombres;
        }

        private static void AgregarNombresDeCultura(
            HashSet<string> nombres,
            CultureInfo cultura)
        {
            string recurso = Lang.ResourceManager.GetString(
                RecursoNombresInvitados,
                cultura);

            if (string.IsNullOrWhiteSpace(recurso))
            {
                return;
            }

            string[] nombresCultura = recurso
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(nombre => nombre.Trim())
                .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                .ToArray();

            foreach (string nombre in nombresCultura)
            {
                nombres.Add(nombre);
            }
        }

        private static DTOs.ResultadoOperacionDTO ValidarCampoObligatorio(
            string valor,
            string mensajeCampoVacio)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return CrearResultadoFallido(mensajeCampoVacio);
            }

            string valorNormalizado = valor.Trim();

            if (valorNormalizado.Length == 0)
            {
                return CrearResultadoFallido(mensajeCampoVacio);
            }

            return CrearResultadoExitoso();
        }

        private static DTOs.ResultadoOperacionDTO CrearResultadoExitoso()
        {
            return new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = true
            };
        }

        private static DTOs.ResultadoOperacionDTO CrearResultadoFallido(string mensaje)
        {
            return new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = mensaje
            };
        }

        private static bool EsOperacionExitosa(DTOs.ResultadoOperacionDTO resultado)
        {
            return resultado != null && resultado.OperacionExitosa;
        }
    }
}