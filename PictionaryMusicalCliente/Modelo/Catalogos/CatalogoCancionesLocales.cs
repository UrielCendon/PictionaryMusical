using log4net;
using System.Collections.Generic;

namespace PictionaryMusicalCliente.Modelo.Catalogos
{
    /// <summary>
    /// Gestiona el catalogo de canciones disponibles para las partidas.
    /// </summary>
    public class CatalogoCancionesLocales : ICatalogoCanciones
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string IdiomaIngles = "Ingles";
        private const string IdiomaEspanol = "Espanol";

        private static readonly IReadOnlyList<Cancion> _listaCanciones =
            InicializarCanciones();
        private static readonly Dictionary<int, Cancion> _diccionarioCanciones =
            CrearDiccionario(_listaCanciones);

        /// <summary>
        /// Obtiene una cancion por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la cancion.</param>
        /// <returns>La cancion si existe, o null si no se encuentra.</returns>
        public Cancion ObtenerPorId(int id)
        {
            if (_diccionarioCanciones.TryGetValue(id, out var cancion))
            {
                return cancion;
            }

            RegistrarCancionNoEncontrada(id);
            return null;
        }

        /// <summary>
        /// Obtiene todas las canciones del catalogo.
        /// </summary>
        /// <returns>Coleccion de todas las canciones disponibles.</returns>
        public IReadOnlyList<Cancion> ObtenerTodas()
        {
            return _listaCanciones;
        }

        private static void RegistrarCancionNoEncontrada(int idCancion)
        {
            _logger.WarnFormat(
                "No se encontro la cancion con id {0} en el catalogo local.",
                idCancion);
        }

        private static Dictionary<int, Cancion> CrearDiccionario(
            IReadOnlyList<Cancion> canciones)
        {
            var diccionario = new Dictionary<int, Cancion>();
            foreach (var cancion in canciones)
            {
                diccionario[cancion.Id] = cancion;
            }
            return diccionario;
        }

        private static List<Cancion> InicializarCanciones()
        {
            return new List<Cancion>
            {
                new Cancion(1, "Gasolina", "Gasolina_Daddy_Yankee.mp3", IdiomaEspanol),
                new Cancion(2, "Bocanada", "Bocanada_Gustavo_Cerati.mp3", IdiomaEspanol),
                new Cancion(
                    3,
                    "La Nave Del Olvido",
                    "La_Nave_Del_Olvido_Jose_Jose.mp3",
                    IdiomaEspanol),
                new Cancion(4, "Tiburón", "Tiburon_Proyecto_Uno.mp3", IdiomaEspanol),
                new Cancion(
                    5,
                    "Pupilas De Gato",
                    "Pupilas_De_Gato_Luis_Miguel.mp3",
                    IdiomaEspanol),
                new Cancion(6, "El Triste", "El_Triste_Jose_Jose.mp3", IdiomaEspanol),
                new Cancion(7, "El Reloj", "El_Reloj_Luis_Miguel.mp3", IdiomaEspanol),
                new Cancion(8, "La Camisa Negra", "La_Camisa_Negra_Juanes.mp3", IdiomaEspanol),
                new Cancion(
                    9,
                    "Rosas",
                    "Rosas_La_Oreja_de_Van_Gogh.mp3",
                    IdiomaEspanol),
                new Cancion(10, "La Bicicleta", "La_Bicicleta_Shakira.mp3", IdiomaEspanol),
                new Cancion(11, "El Taxi", "El_Taxi_Pitbull.mp3", IdiomaEspanol),
                new Cancion(
                    12,
                    "La Puerta Negra",
                    "La_Puerta_Negra_Los_Tigres_del_Norte.mp3",
                    IdiomaEspanol),
                new Cancion(
                    13,
                    "Baraja de Oro",
                    "Baraja_de_Oro_Chalino_Sanchez.mp3",
                    IdiomaEspanol),
                new Cancion(
                    14,
                    "Los Luchadores",
                    "Los_Luchadores_La_Sonora_Santanera.mp3",
                    IdiomaEspanol),
                new Cancion(15, "El Oso Polar", "El_Oso_Polar_Nelson_Kanzela.mp3", IdiomaEspanol),
                new Cancion(
                    16,
                    "El Teléfono",
                    "El_Telefono_Wisin_&_Yandel.mp3",
                    IdiomaEspanol),
                new Cancion(17, "La Planta", "La_Planta_Caos.mp3", IdiomaEspanol),
                new Cancion(18, "Lluvia", "Lluvia_Eddie_Santiago.mp3", IdiomaEspanol),
                new Cancion(19, "Pose", "Pose_Daddy_Yankee.mp3", IdiomaEspanol),
                new Cancion(20, "Cama y Mesa", "Cama_y_Mesa_Roberto_Carlos.mp3", IdiomaEspanol),

                new Cancion(
                    21,
                    "Black Or White",
                    "Black_Or_White_Michael_Jackson.mp3",
                    IdiomaIngles),
                new Cancion(
                    22,
                    "Don't Stop The Music",
                    "Dont_Stop_The_Music_Rihanna.mp3",
                    IdiomaIngles),
                new Cancion(
                    23,
                    "Man In The Mirror",
                    "Man_In_The_Mirror_Michael_Jackson.mp3",
                    IdiomaIngles),
                new Cancion(24, "Earth Song", "Earth_Song_Michael_Jackson.mp3", IdiomaIngles),
                new Cancion(25, "Redbone", "Redbone_Childish_Gambino.mp3", IdiomaIngles),
                new Cancion(26, "The Chain", "The_Chain_Fleetwood_Mac.mp3", IdiomaIngles),
                new Cancion(27, "Umbrella", "Umbrella_Rihanna.mp3", IdiomaIngles),
                new Cancion(
                    28,
                    "Yellow Submarine",
                    "Yellow_Submarine_The_Beatles.mp3",
                    IdiomaIngles),
                new Cancion(29, "Money", "Money_Pink_Floyd.mp3", IdiomaIngles),
                new Cancion(30, "Diamonds", "Diamonds_Rihanna.mp3", IdiomaIngles),
                new Cancion(31, "Grenade", "Grenade_Bruno_Mars.mp3", IdiomaIngles),
                new Cancion(32, "Scarface", "Scarface_Paul_Engemann.mp3", IdiomaIngles),
                new Cancion(33, "Animals", "Animals_Martin_Garrix.mp3", IdiomaIngles),
                new Cancion(34, "Hotel California", "Hotel_California_Eagles.mp3", IdiomaIngles),
                new Cancion(35, "67", "67_Skrilla.mp3", IdiomaIngles),
                new Cancion(36, "Blackbird", "Blackbird_The_Beatles.mp3", IdiomaIngles),
                new Cancion(37, "Pony", "Pony_Ginuwine.mp3", IdiomaIngles),
                new Cancion(38, "Rocket Man", "Rocket_Man_Elton_John.mp3", IdiomaIngles),
                new Cancion(39, "Starman", "Starman_David_Bowie.mp3", IdiomaIngles),
                new Cancion(
                    40,
                    "Time In A Bottle",
                    "Time_In_A_Bottle_Jim_Croce.mp3",
                    IdiomaIngles)
            };
        }
    }
}
