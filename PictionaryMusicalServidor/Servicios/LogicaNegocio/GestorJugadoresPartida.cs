using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio
{
    /// <summary>
    /// Gestiona los jugadores de una partida, incluyendo la cola de dibujantes,
    /// puntajes y estados de cada jugador.
    /// </summary>
    public class GestorJugadoresPartida : IGestorJugadoresPartida
    {
        private const int MinimoJugadoresRequeridos = 2;

        private readonly Dictionary<string, JugadorPartida> _jugadores;
        private readonly Queue<string> _colaDibujantes;
        private readonly IGeneradorAleatorio _generadorAleatorio;
        private List<string> _ordenDibujantesBase;

        /// <summary>
        /// Inicializa una nueva instancia del gestor de jugadores.
        /// </summary>
        /// <param name="generadorAleatorio">Generador para mezclar el orden de dibujantes.
        /// </param>
        /// <exception cref="ArgumentNullException">Se lanza si generadorAleatorio es nulo.
        /// </exception>
        public GestorJugadoresPartida(IGeneradorAleatorio generadorAleatorio)
        {
            _generadorAleatorio = generadorAleatorio ??
                throw new ArgumentNullException(nameof(generadorAleatorio));
            _jugadores = new Dictionary<string, JugadorPartida>(StringComparer.Ordinal);
            _colaDibujantes = new Queue<string>();
            _ordenDibujantesBase = null;
        }

        /// <summary>
        /// Indica si hay suficientes jugadores para iniciar una partida.
        /// </summary>
        public bool HaySuficientesJugadores
        {
            get { return _jugadores.Count >= MinimoJugadoresRequeridos; }
        }

        /// <summary>
        /// Agrega un jugador o actualiza sus datos si ya existe.
        /// </summary>
        /// <param name="id">Identificador de conexion del jugador.</param>
        /// <param name="nombre">Nombre de usuario del jugador.</param>
        /// <param name="esHost">Indica si el jugador es el anfitrion de la sala.</param>
        public void Agregar(string id, string nombre, bool esHost)
        {
            if (_jugadores.ContainsKey(id))
            {
                var existente = _jugadores[id];
                existente.NombreUsuario = nombre;
                existente.EsHost = esHost;
                return;
            }

            _jugadores.Add(id, new JugadorPartida
            {
                IdConexion = id,
                NombreUsuario = nombre,
                EsHost = esHost,
                PuntajeTotal = 0
            });
        }

        /// <summary>
        /// Obtiene un jugador por su identificador de conexion.
        /// </summary>
        /// <param name="id">Identificador de conexion del jugador.</param>
        /// <returns>El jugador encontrado o null si no existe.</returns>
        public JugadorPartida Obtener(string id)
        {
            JugadorPartida jugador;
            _jugadores.TryGetValue(id, out jugador);
            return jugador;
        }

        /// <summary>
        /// Remueve un jugador de la partida.
        /// </summary>
        /// <param name="idConexion">Identificador de conexion del jugador.</param>
        /// <param name="eraDibujante">Salida que indica si el jugador era el dibujante actual.
        /// </param>
        /// <returns>True si el jugador fue removido, false si no existia.</returns>
        public bool Remover(string idConexion, out bool eraDibujante)
        {
            string nombreUsuario;
            return Remover(idConexion, out eraDibujante, out nombreUsuario);
        }

        /// <summary>
        /// Remueve un jugador de la partida y obtiene su informacion.
        /// </summary>
        /// <param name="id">Identificador de conexion del jugador.</param>
        /// <param name="eraDibujante">Salida que indica si el jugador era el dibujante actual.
        /// </param>
        /// <param name="nombreUsuario">Salida con el nombre del usuario removido.</param>
        /// <returns>True si el jugador fue removido, false si no existia.</returns>
        public bool Remover(string id, out bool eraDibujante, out string nombreUsuario)
        {
            eraDibujante = false;
            nombreUsuario = null;
            JugadorPartida jugador;
            if (_jugadores.TryGetValue(id, out jugador))
            {
                eraDibujante = jugador.EsDibujante;
                nombreUsuario = jugador.NombreUsuario;
                _jugadores.Remove(id);
                ReconstruirColaDibujantes(id);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifica si un jugador es el anfitrion de la sala.
        /// </summary>
        /// <param name="id">Identificador de conexion del jugador.</param>
        /// <returns>True si es anfitrion, false en caso contrario.</returns>
        public bool EsHost(string id)
        {
            JugadorPartida jugador;
            if (_jugadores.TryGetValue(id, out jugador))
            {
                return jugador.EsHost;
            }
            return false;
        }

        /// <summary>
        /// Prepara la cola de dibujantes mezclando el orden de los jugadores.
        /// </summary>
        public void PrepararColaDibujantes()
        {
            _colaDibujantes.Clear();

            if (_ordenDibujantesBase == null)
            {
                _ordenDibujantesBase = new List<string>(_jugadores.Keys);
                _generadorAleatorio.MezclarLista(_ordenDibujantesBase);
            }

            foreach (var identificador in _ordenDibujantesBase.Where(id => _jugadores.ContainsKey(id)))
            {
                _colaDibujantes.Enqueue(identificador);
            }
        }

        /// <summary>
        /// Selecciona al siguiente dibujante de la cola.
        /// </summary>
        /// <returns>True si se asigno un dibujante, false si la cola esta vacia.</returns>
        public bool SeleccionarSiguienteDibujante()
        {
            ReiniciarEstadoRondaJugadores();

            while (_colaDibujantes.Count > 0)
            {
                var id = _colaDibujantes.Dequeue();
                JugadorPartida jugador;
                if (_jugadores.TryGetValue(id, out jugador))
                {
                    jugador.EsDibujante = true;
                    jugador.YaAdivino = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Verifica si todos los jugadores (excepto el dibujante) han adivinado.
        /// </summary>
        /// <returns>True si todos adivinaron, false en caso contrario.</returns>
        public bool TodosAdivinaron()
        {
            var adivinadores = new List<JugadorPartida>();
            foreach (var jugador in _jugadores.Values)
            {
                if (!jugador.EsDibujante)
                {
                    adivinadores.Add(jugador);
                }
            }

            if (adivinadores.Count == 0)
            {
                return false;
            }

            foreach (var jugador in adivinadores)
            {
                if (!jugador.YaAdivino)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Genera la clasificacion ordenada por puntaje descendente.
        /// </summary>
        /// <returns>Lista de clasificaciones ordenadas por puntos.</returns>
        public List<ClasificacionUsuarioDTO> GenerarClasificacion()
        {
            var clasificacion = new List<ClasificacionUsuarioDTO>();
            foreach (var jugador in _jugadores.Values)
            {
                clasificacion.Add(new ClasificacionUsuarioDTO
                {
                    Usuario = jugador.NombreUsuario,
                    Puntos = jugador.PuntajeTotal
                });
            }

            clasificacion.Sort(delegate (ClasificacionUsuarioDTO a, ClasificacionUsuarioDTO b)
            {
                return b.Puntos.CompareTo(a.Puntos);
            });

            return clasificacion;
        }

        /// <summary>
        /// Obtiene una copia de la lista de jugadores.
        /// </summary>
        /// <returns>Coleccion de solo lectura con copias de los jugadores.</returns>
        public IReadOnlyCollection<JugadorPartida> ObtenerCopiaLista()
        {
            var copia = new List<JugadorPartida>();
            foreach (var jugador in _jugadores.Values)
            {
                copia.Add(jugador.CopiarDatosBasicos());
            }
            return copia;
        }

        /// <summary>
        /// Indica si quedan dibujantes pendientes en la cola.
        /// </summary>
        /// <returns>True si hay dibujantes pendientes, false en caso contrario.</returns>
        public bool QuedanDibujantesPendientes()
        {
            return _colaDibujantes.Count > 0;
        }

        private void ReiniciarEstadoRondaJugadores()
        {
            foreach (var jugador in _jugadores.Values)
            {
                jugador.EsDibujante = false;
                jugador.YaAdivino = false;
            }
        }

        private void ReconstruirColaDibujantes(string idExcluido)
        {
            if (_colaDibujantes.Count == 0)
            {
                return;
            }

            var listaFiltrada = _colaDibujantes
                .Where(id => id != idExcluido && _jugadores.ContainsKey(id))
                .ToList();

            _colaDibujantes.Clear();

            foreach (var id in listaFiltrada)
            {
                _colaDibujantes.Enqueue(id);
            }
        }
    }
}