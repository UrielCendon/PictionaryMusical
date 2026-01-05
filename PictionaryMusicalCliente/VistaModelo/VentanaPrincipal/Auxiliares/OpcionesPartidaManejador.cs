using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Properties.Langs;
using System.Collections.ObjectModel;

namespace PictionaryMusicalCliente.VistaModelo.VentanaPrincipal.Auxiliares
{
    /// <summary>
    /// Gestiona las opciones de configuracion de partida.
    /// </summary>
    public sealed class OpcionesPartidaManejador
    {
        private const int MinimoRondas = 2;
        private const int RondasMedia = 3;
        private const int MaximoRondas = 4;
        private const int IndiceRondasPredeterminado = 0;

        private const int TiempoRondaCorto = 30;
        private const int TiempoRondaMedio = 60;
        private const int TiempoRondaLargo = 90;
        private const int TiempoRondaExtendido = 120;
        private const int IndiceTiempoPredeterminado = 1;

        private const int IndiceIdiomaPredeterminado = 0;
        private const int IndiceDificultadPredeterminada = 0;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="OpcionesPartidaManejador"/>.
        /// </summary>
        public OpcionesPartidaManejador()
        {
            CargarOpciones();
        }

        /// <summary>
        /// Opciones disponibles para el numero de rondas.
        /// </summary>
        public ObservableCollection<OpcionEntero> NumeroRondasOpciones { get; private set; }

        /// <summary>
        /// Opciones disponibles para el tiempo de ronda.
        /// </summary>
        public ObservableCollection<OpcionEntero> TiempoRondaOpciones { get; private set; }

        /// <summary>
        /// Idiomas disponibles para las canciones.
        /// </summary>
        public ObservableCollection<IdiomaOpcion> IdiomasDisponibles { get; private set; }

        /// <summary>
        /// Niveles de dificultad disponibles.
        /// </summary>
        public ObservableCollection<OpcionTexto> DificultadesDisponibles { get; private set; }

        /// <summary>
        /// Numero de rondas seleccionado por defecto.
        /// </summary>
        public OpcionEntero NumeroRondasPredeterminado { get; private set; }

        /// <summary>
        /// Tiempo de ronda seleccionado por defecto.
        /// </summary>
        public OpcionEntero TiempoRondaPredeterminado { get; private set; }

        /// <summary>
        /// Idioma seleccionado por defecto.
        /// </summary>
        public IdiomaOpcion IdiomaPredeterminado { get; private set; }

        /// <summary>
        /// Dificultad seleccionada por defecto.
        /// </summary>
        public OpcionTexto DificultadPredeterminada { get; private set; }

        private void CargarOpciones()
        {
            CargarOpcionesRondas();
            CargarOpcionesTiempo();
            CargarOpcionesIdioma();
            CargarOpcionesDificultad();
        }

        private void CargarOpcionesRondas()
        {
            NumeroRondasOpciones = new ObservableCollection<OpcionEntero>
            {
                new OpcionEntero(MinimoRondas),
                new OpcionEntero(RondasMedia),
                new OpcionEntero(MaximoRondas)
            };
            NumeroRondasPredeterminado = NumeroRondasOpciones[IndiceRondasPredeterminado];
        }

        private void CargarOpcionesTiempo()
        {
            TiempoRondaOpciones = new ObservableCollection<OpcionEntero>
            {
                new OpcionEntero(TiempoRondaCorto),
                new OpcionEntero(TiempoRondaMedio),
                new OpcionEntero(TiempoRondaLargo),
                new OpcionEntero(TiempoRondaExtendido)
            };
            TiempoRondaPredeterminado = TiempoRondaOpciones[IndiceTiempoPredeterminado];
        }

        private void CargarOpcionesIdioma()
        {
            IdiomasDisponibles = new ObservableCollection<IdiomaOpcion>
            {
                new IdiomaOpcion("es-MX", Lang.idiomaTextoEspanol),
                new IdiomaOpcion("en-US", Lang.idiomaTextoIngles),
                new IdiomaOpcion("mixto", Lang.principalTextoMixto)
            };
            IdiomaPredeterminado = IdiomasDisponibles[IndiceIdiomaPredeterminado];
        }

        private void CargarOpcionesDificultad()
        {
            DificultadesDisponibles = new ObservableCollection<OpcionTexto>
            {
                new OpcionTexto("facil", Lang.principalTextoFacil),
                new OpcionTexto("media", Lang.principalTextoMedia),
                new OpcionTexto("dificil", Lang.principalTextoDificil)
            };
            DificultadPredeterminada = DificultadesDisponibles[IndiceDificultadPredeterminada];
        }
    }
}
