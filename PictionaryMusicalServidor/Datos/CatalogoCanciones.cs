using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using log4net;
using PictionaryMusicalServidor.Datos.Constantes;
using PictionaryMusicalServidor.Datos.Entidades;
using PictionaryMusicalServidor.Datos.Excepciones;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Datos
{
    /// <summary>
    /// 
    /// Proporciona el acceso al catalogo interno de canciones disponibles para las partidas.
    /// </summary>
    public class CatalogoCanciones : ICatalogoCanciones
    {
        private const string IdiomaEspanol = "Espanol";
        private const string IdiomaIngles = "Ingles";
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CatalogoCanciones));
        private static readonly Dictionary<int, Cancion> _canciones = new Dictionary<int, Cancion>
        {
            { 1, CrearCancion(new CancionCreacionParametros { 
                Id = 1, 
                Nombre = "Gasolina", 
                Artista = "Daddy Yankee", 
                Genero = "Reggaeton", 
                Idioma = IdiomaEspanol }) },
            { 2, CrearCancion(new CancionCreacionParametros { 
                Id = 2, 
                Nombre = "Bocanada", 
                Artista = "Gustavo Cerati", 
                Genero = "Rock Alternativo", 
                Idioma = IdiomaEspanol }) },
            { 3, CrearCancion(new CancionCreacionParametros { 
                Id = 3, 
                Nombre = "La Nave Del Olvido", 
                Artista = "José José", 
                Genero = "Balada", 
                Idioma = IdiomaEspanol }) },
            { 4, CrearCancion(new CancionCreacionParametros { 
                Id = 4, 
                Nombre = "Tiburón", 
                Artista = "Proyecto Uno", 
                Genero = "Merengue House", 
                Idioma = IdiomaEspanol }) },
            { 5, CrearCancion(new CancionCreacionParametros { 
                Id = 5, 
                Nombre = "Pupilas De Gato", 
                Artista = "Luis Miguel", 
                Genero = "Pop Latino", 
                Idioma = IdiomaEspanol }) },
            { 6, CrearCancion(new CancionCreacionParametros { 
                Id = 6, 
                Nombre = "El Triste", 
                Artista = "Jose Jose", 
                Genero = "Balada", 
                Idioma = IdiomaEspanol }) },
            { 7, CrearCancion(new CancionCreacionParametros { 
                Id = 7, 
                Nombre = "El Reloj", 
                Artista = "Luis Miguel", 
                Genero = "Bolero", 
                Idioma = IdiomaEspanol }) },
            { 8, CrearCancion(new CancionCreacionParametros { 
                Id = 8, 
                Nombre = "La Camisa Negra", 
                Artista = "Juanes", 
                Genero = "Pop Rock", 
                Idioma = IdiomaEspanol }) },
            { 9, CrearCancion(new CancionCreacionParametros { 
                Id = 9, 
                Nombre = "Rosas", 
                Artista = "La Oreja de Van Gogh", 
                Genero = "Pop", 
                Idioma = IdiomaEspanol }) },
            { 10, CrearCancion(new CancionCreacionParametros { 
                Id = 10, 
                Nombre = "La Bicicleta", 
                Artista = "Shakira", 
                Genero = "Vallenato Pop", 
                Idioma = IdiomaEspanol }) },
            { 11, CrearCancion(new CancionCreacionParametros { 
                Id = 11, 
                Nombre = "El Taxi", 
                Artista = "Pitbull", 
                Genero = "Urbano", 
                Idioma = IdiomaEspanol }) },
            { 12, CrearCancion(new CancionCreacionParametros { 
                Id = 12, 
                Nombre = "La Puerta Negra", 
                Artista = "Los Tigres del Norte", 
                Genero = "Norteño", 
                Idioma = IdiomaEspanol }) },
            { 13, CrearCancion(new CancionCreacionParametros { 
                Id = 13, 
                Nombre = "Baraja de Oro", 
                Artista = "Chalino Sánchez", 
                Genero = "Corrido", 
                Idioma = IdiomaEspanol }) },
            { 14, CrearCancion(new CancionCreacionParametros { 
                Id = 14, 
                Nombre = "Los Luchadores", 
                Artista = "La Sonora Santanera", 
                Genero = "Cumbia", 
                Idioma = IdiomaEspanol }) },
            { 15, CrearCancion(new CancionCreacionParametros { 
                Id = 15, 
                Nombre = "El Oso Polar", 
                Artista = "Nelson Kanzela", 
                Genero = "Cumbia", 
                Idioma = IdiomaEspanol }) },
            { 16, CrearCancion(new CancionCreacionParametros { 
                Id = 16, 
                Nombre = "El Teléfono", 
                Artista = "Wisin & Yandel", 
                Genero = "Reggaeton", 
                Idioma = IdiomaEspanol }) },
            { 17, CrearCancion(new CancionCreacionParametros { 
                Id = 17, 
                Nombre = "La Planta", 
                Artista = "Caos", 
                Genero = "Pop Rock", 
                Idioma = IdiomaEspanol }) },
            { 18, CrearCancion(new CancionCreacionParametros { 
                Id = 18, 
                Nombre = "Lluvia", 
                Artista = "Eddie Santiago", 
                Genero = "Salsa", 
                Idioma = IdiomaEspanol }) },
            { 19, CrearCancion(new CancionCreacionParametros { 
                Id = 19, 
                Nombre = "Pose", 
                Artista = "Daddy Yankee", 
                Genero = "Reggaeton", 
                Idioma = IdiomaEspanol }) },
            { 20, CrearCancion(new CancionCreacionParametros { 
                Id = 20, 
                Nombre = "Cama y Mesa", 
                Artista = "Roberto Carlos", 
                Genero = "Balada", 
                Idioma = IdiomaEspanol }) },

            { 21, CrearCancion(new CancionCreacionParametros { 
                Id = 21, 
                Nombre = "Black Or White", 
                Artista = "Michael Jackson", 
                Genero = "Pop", 
                Idioma = IdiomaIngles }) },
            { 22, CrearCancion(new CancionCreacionParametros { 
                Id = 22, 
                Nombre = "Don't Stop The Music", 
                Artista = "Rihanna", 
                Genero = "Dance Pop", 
                Idioma = IdiomaIngles }) },
            { 23, CrearCancion(new CancionCreacionParametros { 
                Id = 23, 
                Nombre = "Man In The Mirror", 
                Artista = "Michael Jackson", 
                Genero = "Pop/R&B", 
                Idioma = IdiomaIngles }) },
            { 24, CrearCancion(new CancionCreacionParametros { 
                Id = 24, 
                Nombre = "Earth Song", 
                Artista = "Michael Jackson", 
                Genero = "Pop", 
                Idioma = IdiomaIngles }) },
            { 25, CrearCancion(new CancionCreacionParametros { 
                Id = 25, 
                Nombre = "Redbone", 
                Artista = "Childish Gambino", 
                Genero = "Funk", 
                Idioma = IdiomaIngles }) },
            { 26, CrearCancion(new CancionCreacionParametros { 
                Id = 26, 
                Nombre = "The Chain", 
                Artista = "Fleetwood Mac", 
                Genero = "Rock", 
                Idioma = IdiomaIngles }) },
            { 27, CrearCancion(new CancionCreacionParametros { 
                Id = 27, 
                Nombre = "Umbrella", 
                Artista = "Rihanna", 
                Genero = "R&B", 
                Idioma = IdiomaIngles }) },
            { 28, CrearCancion(new CancionCreacionParametros { 
                Id = 28, 
                Nombre = "Yellow Submarine", 
                Artista = "The Beatles", 
                Genero = "Pop Rock", 
                Idioma = IdiomaIngles }) },
            { 29, CrearCancion(new CancionCreacionParametros { 
                Id = 29, 
                Nombre = "Money", 
                Artista = "Pink Floyd", 
                Genero = "Rock", 
                Idioma = IdiomaIngles }) },
            { 30, CrearCancion(new CancionCreacionParametros { 
                Id = 30, 
                Nombre = "Diamonds", 
                Artista = "Rihanna", 
                Genero = "Pop", 
                Idioma = IdiomaIngles }) },
            { 31, CrearCancion(new CancionCreacionParametros { 
                Id = 31, 
                Nombre = "Grenade", 
                Artista = "Bruno Mars", 
                Genero = "Pop", 
                Idioma = IdiomaIngles }) },
            { 32, CrearCancion(new CancionCreacionParametros { 
                Id = 32, 
                Nombre = "Scarface", 
                Artista = "Paul Engemann", 
                Genero = "Synthpop", 
                Idioma = IdiomaIngles }) },
            { 33, CrearCancion(new CancionCreacionParametros { 
                Id = 33, 
                Nombre = "Animals", 
                Artista = "Martin Garrix", 
                Genero = "EDM", 
                Idioma = IdiomaIngles }) },
            { 34, CrearCancion(new CancionCreacionParametros { 
                Id = 34, 
                Nombre = "Hotel California", 
                Artista = "Eagles", 
                Genero = "Rock", 
                Idioma = IdiomaIngles }) },
            { 35, CrearCancion(new CancionCreacionParametros { 
                Id = 35, 
                Nombre = "67", 
                Artista = "Skrilla", 
                Genero = "Hip Hop", 
                Idioma = IdiomaIngles }) },
            { 36, CrearCancion(new CancionCreacionParametros { 
                Id = 36, 
                Nombre = "Blackbird", 
                Artista = "The Beatles", 
                Genero = "Folk", 
                Idioma = IdiomaIngles }) },
            { 37, CrearCancion(new CancionCreacionParametros { 
                Id = 37, 
                Nombre = "Pony", 
                Artista = "Ginuwine", 
                Genero = "R&B", 
                Idioma = IdiomaIngles }) },
            { 38, CrearCancion(new CancionCreacionParametros { 
                Id = 38, 
                Nombre = "Rocket Man", 
                Artista = "Elton John", 
                Genero = "Soft Rock", 
                Idioma = IdiomaIngles }) },
            { 39, CrearCancion(new CancionCreacionParametros { 
                Id = 39, 
                Nombre = "Starman", 
                Artista = "David Bowie", 
                Genero = "Glam Rock", 
                Idioma = IdiomaIngles }) },
            { 40, CrearCancion(new CancionCreacionParametros { 
                Id = 40, 
                Nombre = "Time In A Bottle", 
                Artista = "Jim Croce", 
                Genero = "Folk", 
                Idioma = IdiomaIngles }) }
        };

        /// <summary>
        /// Obtiene una cancion aleatoria segun el idioma solicitado y excluyendo los 
        /// identificadores proporcionados.
        /// </summary>
        /// <param name="idioma">Idioma de la cancion ("Espanol" o "Ingles").</param>
        /// <param name="idsExcluidos">Coleccion de identificadores que no deben considerarse.
        /// </param>
        /// <returns>Una instancia de <see cref="Cancion"/> que cumple los criterios.</returns>
        /// <exception cref="ArgumentException">Se produce cuando el idioma es nulo o vacio.
        /// </exception>
        /// <exception cref="CancionNoDisponibleExcepcion">Se produce cuando no hay canciones 
        /// disponibles.</exception>

        public Cancion ObtenerCancionAleatoria(string idioma, HashSet<int> idsExcluidos)
        {
            if (string.IsNullOrWhiteSpace(idioma))
            {
                var excepcion = new ArgumentException(
                    MensajesErrorDatos.Cancion.IdiomaNoNuloVacio, 
                    nameof(idioma));
                _logger.Error(MensajesErrorDatos.Cancion.IdiomaInvalidoSolicitar, excepcion);
                throw excepcion;
            }

            try
            {
                string idiomaInterno = MapearIdiomaInterno(idioma);
                string idiomaBusqueda = NormalizarTexto(idiomaInterno);
                var idsRechazados = idsExcluidos ?? new HashSet<int>();

                var candidatos = ObtenerCandidatos(idiomaBusqueda, idsRechazados);

                if (!candidatos.Any())
                {
                    RegistrarErrorFaltaCanciones(idioma, idiomaInterno, idiomaBusqueda);
                    throw new CancionNoDisponibleExcepcion(
                        MensajesErrorDatos.Cancion.CancionesNoDisponibles);
                }

                return SeleccionarAleatorio(candidatos);
            }
            catch (CancionNoDisponibleExcepcion excepcion)
            {
                _logger.Warn(
                    MensajesErrorDatos.Cancion.CancionesNoCumplenCriterios,
                    excepcion);
                throw new CancionNoDisponibleExcepcion(
                    MensajesErrorDatos.Cancion.CancionesNoCumplenCriterios,
                    excepcion);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Cancion.ErrorInesperadoObtener, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Cancion.ErrorInesperadoObtener, 
                    excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Cancion.ErrorInesperadoObtener, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Cancion.ErrorInesperadoObtener, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesErrorDatos.Cancion.ErrorInesperadoObtener, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Cancion.ErrorInesperadoObtener, 
                    excepcion);
            }
        }

        /// <summary>
        /// Obtiene una cancion por su identificador.
        /// </summary>
        /// <param name="idCancion">Identificador de la cancion.</param>
        /// <returns>La cancion correspondiente.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si el identificador no existe en el 
        /// catalogo.</exception>
        public Cancion ObtenerCancionPorId(int idCancion)
        {
            Cancion cancion;
            if (_canciones.TryGetValue(idCancion, out cancion))
            {
                return cancion;
            }

            _logger.WarnFormat(
                MensajesErrorDatos.Cancion.CancionNoEncontradaCatalogo, 
                idCancion);
            
            throw new KeyNotFoundException(
                MensajesErrorDatos.Cancion.CancionNoEnCatalogo);
        }

        /// <summary>
        /// Valida si el intento del usuario coincide con el nombre de la cancion indicada.
        /// </summary>
        /// <param name="idCancion">Identificador de la cancion a evaluar.</param>
        /// <param name="intentoUsuario">Texto proporcionado por el usuario.</param>
        /// <returns>True si el intento coincide con el nombre normalizado de la cancion, false en
        /// caso contrario.</returns>
        public bool ValidarRespuesta(int idCancion, string intentoUsuario)
        {
            if (!_canciones.ContainsKey(idCancion))
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Cancion.CancionNoEncontradaCatalogo, 
                    idCancion);
                return false;
            }

            var intentoNormalizado = NormalizarTexto(intentoUsuario);

            if (string.IsNullOrWhiteSpace(intentoNormalizado))
            {
                return false;
            }

            return string.Equals(intentoNormalizado, _canciones[idCancion].NombreNormalizado, 
                StringComparison.Ordinal);
        }

        private static Cancion CrearCancion(CancionCreacionParametros parametros)
        {
            var nombreNormalizado = NormalizarTexto(parametros.Nombre);

            return new Cancion
            {
                Id = parametros.Id,
                Nombre = parametros.Nombre,
                NombreNormalizado = nombreNormalizado,
                Artista = parametros.Artista,
                Genero = parametros.Genero,
                Idioma = parametros.Idioma
            };
        }

        private static string MapearIdiomaInterno(string idiomaEntrada)
        {
            if (idiomaEntrada.StartsWith("es", StringComparison.OrdinalIgnoreCase))
            {
                return IdiomaEspanol;
            }

            if (idiomaEntrada.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                return IdiomaIngles;
            }

            return idiomaEntrada;
        }

        private static List<Cancion> ObtenerCandidatos(string idiomaNormalizado, 
            HashSet<int> idsRechazados)
        {
            var candidatos = new List<Cancion>();
            foreach (var cancion in _canciones.Values)
            {
                if (idsRechazados.Contains(cancion.Id))
                {
                    continue;
                }

                if (string.Equals(idiomaNormalizado, "mixto", StringComparison.OrdinalIgnoreCase))
                {
                    candidatos.Add(cancion);
                }
                else if (string.Equals(NormalizarTexto(cancion.Idioma), idiomaNormalizado, 
                    StringComparison.OrdinalIgnoreCase))
                {
                    candidatos.Add(cancion);
                }
            }

            return candidatos;
        }

        private static void RegistrarErrorFaltaCanciones(string idiomaOriginal, 
            string idiomaMapeado, string idiomaNorm)
        {
            _logger.WarnFormat(
                "Fallo al buscar cancion. Idioma Entrante: {0}, Mapeado: {1}, Normalizado: {2}.",
                idiomaOriginal, idiomaMapeado, idiomaNorm);
        }

        private static Cancion SeleccionarAleatorio(List<Cancion> candidatos)
        {
            return GeneradorAleatorioDatos.SeleccionarAleatorio(candidatos);
        }

        private static string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            var textoFormateado = texto.Trim().ToLowerInvariant();
            var textoDescompuesto = textoFormateado.Normalize(NormalizationForm.FormD);
            var constructor = new StringBuilder();

            foreach (var caracter in textoDescompuesto)
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(caracter);
                if (categoria == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(caracter) || char.IsWhiteSpace(caracter))
                {
                    constructor.Append(caracter);
                }
            }

            var textoSinAcentos = constructor.ToString().Normalize(NormalizationForm.FormC);
            var partes = textoSinAcentos.Split(new[] { ' ' }, 
                StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", partes);
        }
    }
}
