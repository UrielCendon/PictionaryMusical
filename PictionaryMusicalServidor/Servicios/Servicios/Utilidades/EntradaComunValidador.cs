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
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$",
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

        /// <summary>
        /// Valida la configuracion de una partida.
        /// Verifica que los campos de configuracion cumplan con los requisitos.
        /// </summary>
        /// <param name="configuracion">Configuracion de partida a validar.</param>
        /// <exception cref="FaultException">Se lanza si algun campo no es valido.</exception>
        public static void ValidarConfiguracionPartida(ConfiguracionPartidaDTO configuracion)
        {
            if (configuracion == null)
            {
                throw new FaultException(MensajesError.Cliente.ConfiguracionObligatoria);
            }

            if (configuracion.NumeroRondas <= 0 || 
                configuracion.NumeroRondas > NumeroRondasMaximo)
            {
                throw new FaultException(MensajesError.Cliente.NumeroRondasInvalido);
            }

            if (configuracion.TiempoPorRondaSegundos <= 0 ||
                configuracion.TiempoPorRondaSegundos > TiempoRondaMaximoSegundos)
            {
                throw new FaultException(MensajesError.Cliente.TiempoRondaInvalido);
            }

            if (!EsIdiomaValido(configuracion.IdiomaCanciones))
            {
                throw new FaultException(MensajesError.Cliente.IdiomaObligatorio);
            }

            if (!EsDificultadValida(configuracion.Dificultad))
            {
                throw new FaultException(MensajesError.Cliente.DificultadObligatoria);
            }
        }

        /// <summary>
        /// Valida los datos de entrada para unirse a una sala o chat.
        /// Verifica que el codigo de sala y nombre de jugador sean validos.
        /// </summary>
        /// <param name="codigoSala">Codigo de la sala a validar.</param>
        /// <param name="nombreJugador">Nombre del jugador a validar.</param>
        /// <exception cref="FaultException">Se lanza si algun campo no es valido.</exception>
        public static void ValidarEntradaSalaChat(string codigoSala, string nombreJugador)
        {
            ValidarNombreUsuario(nombreJugador, nameof(nombreJugador));

            if (!EsCodigoSalaValido(codigoSala))
            {
                throw new FaultException(MensajesError.Cliente.CodigoSalaObligatorio);
            }
        }

        /// <summary>
        /// Valida que el codigo de sala sea valido y lanza FaultException si no lo es.
        /// </summary>
        /// <param name="codigoSala">Codigo de sala a validar.</param>
        /// <exception cref="FaultException">Se lanza si el codigo no es valido.</exception>
        public static void ValidarCodigoSala(string codigoSala)
        {
            if (!EsCodigoSalaValido(codigoSala))
            {
                throw new FaultException(MensajesError.Cliente.CodigoSalaObligatorio);
            }
        }

        /// <summary>
        /// Valida que el identificador de sala no este vacio.
        /// </summary>
        /// <param name="idSala">Identificador de sala a validar.</param>
        /// <exception cref="FaultException">Se lanza si el identificador esta vacio.</exception>
        public static void ValidarIdSala(string idSala)
        {
            if (string.IsNullOrWhiteSpace(idSala))
            {
                throw new FaultException(MensajesError.Cliente.IdSalaObligatorio);
            }
        }

        /// <summary>
        /// Valida que el identificador de jugador no este vacio.
        /// </summary>
        /// <param name="idJugador">Identificador de jugador a validar.</param>
        /// <exception cref="FaultException">Se lanza si el identificador esta vacio.</exception>
        public static void ValidarIdJugador(string idJugador)
        {
            if (string.IsNullOrWhiteSpace(idJugador))
            {
                throw new FaultException(MensajesError.Cliente.IdJugadorObligatorio);
            }
        }

        /// <summary>
        /// Valida los datos de suscripcion de un jugador a una partida.
        /// Verifica que la suscripcion, el id de sala y el id de jugador sean validos.
        /// </summary>
        /// <param name="suscripcion">Datos de suscripcion a validar.</param>
        /// <exception cref="FaultException">Se lanza si algun campo no es valido.</exception>
        public static void ValidarSuscripcionJugador(SuscripcionJugadorDTO suscripcion)
        {
            if (suscripcion == null)
            {
                throw new FaultException(MensajesError.Cliente.DatosSuscripcionObligatorios);
            }

            ValidarIdSala(suscripcion.IdSala);
            ValidarIdJugador(suscripcion.IdJugador);
        }

        /// <summary>
        /// Valida que un mensaje no supere el limite de caracteres permitido.
        /// </summary>
        /// <param name="mensaje">Mensaje a validar.</param>
        /// <returns>True si el mensaje supera el limite de caracteres.</returns>
        public static bool SuperaLimiteCaracteresMensaje(string mensaje)
        {
            return !string.IsNullOrWhiteSpace(mensaje) && mensaje.Length > LongitudMaximaMensajeChat;
        }

        /// <summary>
        /// Valida que un mensaje de juego sea valido.
        /// </summary>
        /// <param name="mensaje">Mensaje a validar.</param>
        /// <param name="idSala">Identificador de sala.</param>
        /// <exception cref="FaultException">Se lanza si el mensaje o sala no son validos.</exception>
        public static void ValidarMensajeJuego(string mensaje, string idSala)
        {
            ValidarIdSala(idSala);

            if (SuperaLimiteCaracteresMensaje(mensaje))
            {
                throw new FaultException(MensajesError.Cliente.MensajeSuperaLimiteCaracteres);
            }
        }

        /// <summary>
        /// Valida los datos de un reporte de jugador.
        /// Verifica que el reporte no sea nulo, los usuarios sean validos y el motivo cumpla 
        /// requisitos.
        /// </summary>
        /// <param name="reporte">Reporte a validar.</param>
        /// <exception cref="FaultException">Se lanza si algun campo no es valido.</exception>
        public static void ValidarReporteJugador(ReporteJugadorDTO reporte)
        {
            if (reporte == null)
            {
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }

            ValidarNombreUsuario(reporte.NombreUsuarioReportante, "usuario reportante");
            ValidarNombreUsuario(reporte.NombreUsuarioReportado, "usuario reportado");

            string motivo = NormalizarTexto(reporte.Motivo);
            if (motivo == null)
            {
                throw new FaultException(MensajesError.Cliente.ReporteMotivoObligatorio);
            }

            if (!EsLongitudValidaReporte(motivo))
            {
                throw new FaultException(MensajesError.Cliente.ReporteMotivoLongitud);
            }
        }

        /// <summary>
        /// Valida los datos de una invitacion a sala.
        /// Verifica que la invitacion no sea nula, el codigo de sala y correo sean validos.
        /// </summary>
        /// <param name="invitacion">Invitacion a validar.</param>
        /// <exception cref="ArgumentException">Se lanza si algun campo no es valido.</exception>
        public static void ValidarInvitacionSala(InvitacionSalaDTO invitacion)
        {
            if (invitacion == null)
            {
                throw new ArgumentException(MensajesError.Cliente.SolicitudInvitacionInvalida);
            }

            if (!EsCodigoSalaValido(invitacion.CodigoSala) ||
                string.IsNullOrWhiteSpace(invitacion.Correo))
            {
                throw new ArgumentException(MensajesError.Cliente.DatosInvitacionInvalidos);
            }

            if (!EsCorreoValido(invitacion.Correo.Trim()))
            {
                throw new ArgumentException(MensajesError.Cliente.CorreoInvalido);
            }
        }

        /// <summary>
        /// Valida que el identificador de usuario sea un valor positivo.
        /// </summary>
        /// <param name="idUsuario">Identificador de usuario a validar.</param>
        /// <exception cref="ArgumentException">Se lanza si el identificador no es valido.</exception>
        public static void ValidarIdUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException(MensajesError.Cliente.DatosInvalidos);
            }
        }

        /// <summary>
        /// Valida que el nombre de usuario para suscripcion no este vacio.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a validar.</param>
        /// <exception cref="FaultException">Se lanza si el nombre esta vacio.</exception>
        public static void ValidarNombreUsuarioSuscripcion(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new FaultException(MensajesError.Cliente.NombreUsuarioObligatorioSuscripcion);
            }
        }

        /// <summary>
        /// Valida que dos nombres de usuario para interaccion sean validos.
        /// </summary>
        /// <param name="usuarioA">Primer nombre de usuario.</param>
        /// <param name="usuarioB">Segundo nombre de usuario.</param>
        /// <exception cref="FaultException">Se lanza si alguno no es valido.</exception>
        public static void ValidarUsuariosInteraccion(string usuarioA, string usuarioB)
        {
            ValidarNombreUsuario(usuarioA, nameof(usuarioA));
            ValidarNombreUsuario(usuarioB, nameof(usuarioB));
        }
    }
}