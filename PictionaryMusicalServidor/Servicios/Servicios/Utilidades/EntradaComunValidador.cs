using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Clase utilitaria para validar y normalizar entradas comunes del sistema.
    /// Proporciona validaciones de formato y longitud para texto, correo, contrasena, token y 
    /// datos de cuenta.
    /// Verifica que las contrasenas cumplan con requisitos de seguridad (mayuscula, numero, 
    /// caracter especial).
    /// </summary>
    internal static class EntradaComunValidador
    {
        internal const int LongitudMaximaTexto = 50;
        internal const int LongitudMaximaReporte = 100;
        internal const int LongitudMaximaContrasena = 15;
        internal const int LongitudCodigoVerificacion = 6;
        internal const int LongitudMaximaMensajeChat = 150;
        internal const int LongitudCodigoSala = 6;
        internal const int NumeroRondasMaximo = 4;
        internal const int TiempoRondaMaximoSegundos = 120;

        private static readonly Regex CorreoRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));

        private static readonly Regex ContrasenaRegex = new Regex(
            @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-\[\]{};:'"",.<>/?]).{8,15}$",
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(1));

        private static readonly Regex TokenRegex = new Regex(
            @"^[a-fA-F0-9]{32}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));

        /// <summary>
        /// Normaliza un texto eliminando espacios al inicio y final.
        /// Retorna null si el valor es nulo o solo espacios en blanco.
        /// </summary>
        /// <param name="valor">Texto a normalizar.</param>
        /// <returns>Texto normalizado o null.</returns>
        public static string NormalizarTexto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }

        /// <summary>
        /// Verifica si un texto tiene una longitud valida segun el limite maximo.
        /// </summary>
        /// <param name="valor">Texto a validar.</param>
        /// <returns>True si el texto no es vacio y no excede la longitud maxima.</returns>
        public static bool EsLongitudValida(string valor)
        {
            return !string.IsNullOrWhiteSpace(valor) && valor.Length <= LongitudMaximaTexto;
        }

        /// <summary>
        /// Verifica si un texto tiene una longitud valida segun el limite maximo.
        /// </summary>
        /// <param name="valor">Texto a validar.</param>
        /// <returns>True si el texto no es vacio y no excede la longitud maxima.</returns>
        public static bool EsLongitudValidaReporte(string valor)
        {
            return !string.IsNullOrWhiteSpace(valor) && valor.Length <= LongitudMaximaReporte;
        }

        /// <summary>
        /// Verifica si un correo electronico tiene formato valido.
        /// Valida longitud y patron basico de correo electronico.
        /// </summary>
        /// <param name="valor">Correo electronico a validar.</param>
        /// <returns>True si el correo tiene formato valido.</returns>
        public static bool EsCorreoValido(string valor)
        {
            return EsLongitudValida(valor) && CorreoRegex.IsMatch(valor);
        }

        /// <summary>
        /// Verifica si una contrasena cumple con los requisitos de seguridad.
        /// Valida que contenga al menos una mayuscula, un numero, un caracter especial y longitud
        /// entre 8 y 15.
        /// </summary>
        /// <param name="valor">Contrasena a validar.</param>
        /// <returns>True si la contrasena cumple todos los requisitos.</returns>
        public static bool EsContrasenaValida(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            string normalizado = valor.Trim();
            return normalizado.Length <= LongitudMaximaContrasena && 
                ContrasenaRegex.IsMatch(normalizado);
        }

        /// <summary>
        /// Verifica si un token tiene formato valido (32 caracteres hexadecimales).
        /// </summary>
        /// <param name="token">Token a validar.</param>
        /// <returns>True si el token tiene formato valido.</returns>
        public static bool EsTokenValido(string token)
        {
            string normalizado = NormalizarTexto(token);
            return normalizado != null && TokenRegex.IsMatch(normalizado);
        }

        /// <summary>
        /// Verifica si un codigo de verificacion tiene formato valido (6 digitos).
        /// </summary>
        /// <param name="codigo">Codigo de verificacion a validar.</param>
        /// <returns>True si el codigo tiene formato valido.</returns>
        public static bool EsCodigoVerificacionValido(string codigo)
        {
            string normalizado = NormalizarTexto(codigo);
            return normalizado != null && 
                   normalizado.Length == LongitudCodigoVerificacion &&
                   normalizado.All(char.IsDigit);
        }

        /// <summary>
        /// Verifica si un codigo de sala tiene formato valido.
        /// Valida que no sea nulo, tenga longitud correcta y contenga solo digitos.
        /// </summary>
        /// <param name="codigoSala">Codigo de sala a validar.</param>
        /// <returns>True si el codigo de sala tiene formato valido.</returns>
        public static bool EsCodigoSalaValido(string codigoSala)
        {
            string normalizado = NormalizarTexto(codigoSala);
            return normalizado != null &&
                   normalizado.Length == LongitudCodigoSala &&
                   normalizado.All(char.IsDigit);
        }

        /// <summary>
        /// Verifica si un mensaje de chat tiene contenido valido.
        /// Valida que no este vacio y no exceda la longitud maxima permitida.
        /// </summary>
        /// <param name="mensaje">Mensaje a validar.</param>
        /// <returns>True si el mensaje tiene contenido valido.</returns>
        public static bool EsMensajeValido(string mensaje)
        {
            return !string.IsNullOrWhiteSpace(mensaje) && 
                   mensaje.Trim().Length <= LongitudMaximaMensajeChat;
        }

        /// <summary>
        /// Verifica si una configuracion de partida tiene idioma valido.
        /// Valida que no este vacio y no exceda la longitud maxima.
        /// </summary>
        /// <param name="idioma">Idioma a validar.</param>
        /// <returns>True si el idioma tiene valor valido.</returns>
        public static bool EsIdiomaValido(string idioma)
        {
            return !string.IsNullOrWhiteSpace(idioma) && 
                   idioma.Trim().Length <= LongitudMaximaTexto;
        }

        /// <summary>
        /// Verifica si una configuracion de partida tiene dificultad valida.
        /// Valida que no este vacia y no exceda la longitud maxima.
        /// </summary>
        /// <param name="dificultad">Dificultad a validar.</param>
        /// <returns>True si la dificultad tiene valor valido.</returns>
        public static bool EsDificultadValida(string dificultad)
        {
            return !string.IsNullOrWhiteSpace(dificultad) && 
                   dificultad.Trim().Length <= LongitudMaximaTexto;
        }

        /// <summary>
        /// Valida que el nombre de usuario cumpla con los requisitos de longitud.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a validar.</param>
        /// <returns>True si el nombre es valido.</returns>
        public static bool EsNombreUsuarioValido(string nombreUsuario)
        {
            return EsLongitudValida(NormalizarTexto(nombreUsuario));
        }

        /// <summary>
        /// Valida que el nombre de usuario sea valido y lanza FaultException si no lo es.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a validar.</param>
        /// <param name="nombreParametro">Nombre del parametro para el mensaje de error.</param>
        /// <exception cref="FaultException">Se lanza si el nombre no es valido.</exception>
        public static void ValidarNombreUsuario(string nombreUsuario, string nombreParametro)
        {
            if (!EsNombreUsuarioValido(nombreUsuario))
            {
                string mensaje = string.Format(
                    CultureInfo.CurrentCulture,
                    MensajesError.Cliente.ParametroObligatorio, 
                    nombreParametro);
                throw new FaultException(mensaje);
            }
        }

        /// <summary>
        /// Obtiene el nombre normalizado de usuario, priorizando el primer valor no vacio.
        /// </summary>
        /// <param name="nombrePrincipal">Nombre principal (ej: de base de datos).</param>
        /// <param name="nombreAlterno">Nombre alternativo si el principal no es valido.</param>
        /// <returns>Nombre normalizado de usuario.</returns>
        public static string ObtenerNombreUsuarioNormalizado(
            string nombrePrincipal, 
            string nombreAlterno)
        {
            string nombre = NormalizarTexto(nombrePrincipal);
            return nombre ?? NormalizarTexto(nombreAlterno);
        }
        

        /// <summary>
        /// Valida todos los campos de una nueva cuenta.
        /// Verifica usuario, nombre, apellido, correo, contrasena y avatar cumplan con los 
        /// requisitos.
        /// </summary>
        /// <param name="nuevaCuenta">Datos de la nueva cuenta a validar.</param>
        /// <returns>Resultado de la validacion indicando exito o errores.</returns>
        public static ResultadoOperacionDTO ValidarNuevaCuenta(NuevaCuentaDTO nuevaCuenta)
        {
            if (nuevaCuenta == null)
            {
                return CrearResultadoOperacion(false, MensajesError.Cliente.DatosInvalidos);
            }

            ResultadoValidacionCampo resultadoUsuario = ValidarCampoObligatorio(
                nuevaCuenta.Usuario, 
                EsLongitudValida, 
                MensajesError.Cliente.UsuarioRegistroInvalido);

            if (!resultadoUsuario.Resultado.OperacionExitosa)
            {
                return resultadoUsuario.Resultado;
            }
            nuevaCuenta.Usuario = resultadoUsuario.ValorNormalizado;

            ResultadoValidacionCampo resultadoNombre = ValidarCampoObligatorio(
                nuevaCuenta.Nombre, 
                EsLongitudValida, 
                MensajesError.Cliente.NombreRegistroInvalido);

            if (!resultadoNombre.Resultado.OperacionExitosa)
            {
                return resultadoNombre.Resultado;
            }
            nuevaCuenta.Nombre = resultadoNombre.ValorNormalizado;

            ResultadoValidacionCampo resultadoApellido = ValidarCampoObligatorio(
                nuevaCuenta.Apellido, 
                EsLongitudValida, 
                MensajesError.Cliente.ApellidoRegistroInvalido);

            if (!resultadoApellido.Resultado.OperacionExitosa)
            {
                return resultadoApellido.Resultado;
            }
            nuevaCuenta.Apellido = resultadoApellido.ValorNormalizado;

            ResultadoValidacionCampo resultadoCorreo = ValidarCampoObligatorio(
                nuevaCuenta.Correo, 
                EsCorreoValido, 
                MensajesError.Cliente.CorreoRegistroInvalido);

            if (!resultadoCorreo.Resultado.OperacionExitosa)
            {
                return resultadoCorreo.Resultado;
            }
            nuevaCuenta.Correo = resultadoCorreo.ValorNormalizado;

            ResultadoValidacionCampo resultadoContrasena = ValidarCampoObligatorio(
                nuevaCuenta.Contrasena, 
                EsContrasenaValida, 
                MensajesError.Cliente.ContrasenaRegistroInvalida);

            if (!resultadoContrasena.Resultado.OperacionExitosa)
            {
                return resultadoContrasena.Resultado;
            }
            nuevaCuenta.Contrasena = resultadoContrasena.ValorNormalizado;

            return CrearResultadoOperacion(true);
        }

        public static ResultadoOperacionDTO ValidarActualizacionPerfil(
            ActualizacionPerfilDTO solicitud)
        {
            if (solicitud == null || solicitud.UsuarioId <= 0)
            {
                return CrearResultadoOperacion(false, MensajesError.Cliente.DatosInvalidos);
            }

            if (solicitud.AvatarId <= 0)
            {
                return CrearResultadoOperacion(false, MensajesError.Cliente.AvatarInvalido);
            }

            var resultadoDatos = ValidarDatosPersonalesPerfil(solicitud);
            if (!resultadoDatos.OperacionExitosa)
            {
                return resultadoDatos;
            }

            return ValidarRedesSociales(solicitud);
        }

        private static ResultadoOperacionDTO ValidarDatosPersonalesPerfil(
            ActualizacionPerfilDTO solicitud)
        {
            ResultadoValidacionCampo resultadoNombre = ValidarCampoObligatorio(
                solicitud.Nombre,
                EsLongitudValida,
                MensajesError.Cliente.NombreRegistroInvalido);

            if (!resultadoNombre.Resultado.OperacionExitosa)
            {
                return resultadoNombre.Resultado;
            }
            solicitud.Nombre = resultadoNombre.ValorNormalizado;

            ResultadoValidacionCampo resultadoApellido = ValidarCampoObligatorio(
                solicitud.Apellido,
                EsLongitudValida,
                MensajesError.Cliente.ApellidoRegistroInvalido);

            if (!resultadoApellido.Resultado.OperacionExitosa)
            {
                return resultadoApellido.Resultado;
            }
            solicitud.Apellido = resultadoApellido.ValorNormalizado;

            return CrearResultadoOperacion(true);
        }

        private static ResultadoOperacionDTO ValidarRedesSociales(ActualizacionPerfilDTO solicitud)
        {
            ResultadoValidacionRedSocial resultadoInstagram = ValidarRedSocial(
                "Instagram", 
                solicitud.Instagram);

            if (!resultadoInstagram.Resultado.OperacionExitosa)
            {
                return resultadoInstagram.Resultado;
            }
            solicitud.Instagram = resultadoInstagram.ValorNormalizado;

            ResultadoValidacionRedSocial resultadoFacebook = ValidarRedSocial(
                "Facebook", 
                solicitud.Facebook);

            if (!resultadoFacebook.Resultado.OperacionExitosa)
            {
                return resultadoFacebook.Resultado;
            }
            solicitud.Facebook = resultadoFacebook.ValorNormalizado;

            ResultadoValidacionRedSocial resultadoX = ValidarRedSocial("X", solicitud.X);
            if (!resultadoX.Resultado.OperacionExitosa)
            {
                return resultadoX.Resultado;
            }
            solicitud.X = resultadoX.ValorNormalizado;

            ResultadoValidacionRedSocial resultadoDiscord = ValidarRedSocial(
                "Discord", 
                solicitud.Discord);

            if (!resultadoDiscord.Resultado.OperacionExitosa)
            {
                return resultadoDiscord.Resultado;
            }
            solicitud.Discord = resultadoDiscord.ValorNormalizado;

            return CrearResultadoOperacion(true);
        }

        private static ResultadoValidacionCampo ValidarCampoObligatorio(
            string campo,
            Func<string, bool> regla,
            string mensajeError)
        {
            string campoNormalizado = NormalizarTexto(campo);
            if (!regla(campoNormalizado))
            {
                return new ResultadoValidacionCampo
                {
                    Resultado = CrearResultadoOperacion(false, mensajeError),
                    ValorNormalizado = null
                };
            }

            return new ResultadoValidacionCampo
            {
                Resultado = CrearResultadoOperacion(true),
                ValorNormalizado = campoNormalizado
            };
        }

        private static ResultadoValidacionRedSocial ValidarRedSocial(string nombre, string valor)
        {
            string valorNormalizado = NormalizarTexto(valor);
            if (valorNormalizado == null)
            {
                return new ResultadoValidacionRedSocial
                {
                    Resultado = CrearResultadoOperacion(true),
                    ValorNormalizado = null
                };
            }

            if (valorNormalizado.Length > LongitudMaximaTexto)
            {
                return new ResultadoValidacionRedSocial
                {
                    Resultado = CrearResultadoOperacion(
                        false,
                        string.Format("El identificador de {0} no debe exceder {1} caracteres.",
                            nombre, LongitudMaximaTexto)),
                    ValorNormalizado = null
                };
            }

            return new ResultadoValidacionRedSocial
            {
                Resultado = CrearResultadoOperacion(true),
                ValorNormalizado = valorNormalizado
            };
        }

        private sealed class ResultadoValidacionCampo
        {
            public ResultadoOperacionDTO Resultado { get; set; }
            public string ValorNormalizado { get; set; }
        }

        private sealed class ResultadoValidacionRedSocial
        {
            public ResultadoOperacionDTO Resultado { get; set; }
            public string ValorNormalizado { get; set; }
        }

        private static ResultadoOperacionDTO CrearResultadoOperacion(bool exitoso,
            string mensaje = null)
        {
            return new ResultadoOperacionDTO
            {
                OperacionExitosa = exitoso,
                Mensaje = mensaje
            };
        }
    }
}