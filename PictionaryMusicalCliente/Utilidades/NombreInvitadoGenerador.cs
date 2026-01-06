using log4net;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.Utilidades.Resultados;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PictionaryMusicalCliente.Utilidades
{
    /// <summary>
    /// Genera nombres aleatorios para los invitados respetando la cultura solicitada.
    /// </summary>
    public class NombreInvitadoGenerador : INombreInvitadoGenerador
    {
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(NombreInvitadoGenerador));
        private readonly object _sincronizacion = new();
        private readonly Random _aleatorio = new();

        /// <inheritdoc />
        public ResultadoGeneracion Generar(
            CultureInfo cultura,
            IEnumerable<string> nombresExcluidos = null)
        {
            CultureInfo culturaEfectiva = cultura ?? CultureInfo.CurrentUICulture;
            string recursoNombres = ObtenerRecursoNombres(culturaEfectiva);

            if (string.IsNullOrWhiteSpace(recursoNombres))
            {
                RegistrarRecursoNoEncontrado(culturaEfectiva);
                return ResultadoGeneracion.Fallo(MotivoFalloGeneracion.RecursoNoEncontrado);
            }

            string[] todosLosNombres = ParsearNombres(recursoNombres);
            if (todosLosNombres.Length == 0)
            {
                RegistrarListaVacia();
                return ResultadoGeneracion.Fallo(MotivoFalloGeneracion.ListaVacia);
            }

            string[] nombresDisponibles = FiltrarNombresDisponibles(
                todosLosNombres,
                nombresExcluidos);

            if (nombresDisponibles.Length == 0)
            {
                RegistrarNombresAgotados();
                return ResultadoGeneracion.Fallo(MotivoFalloGeneracion.NombresAgotados);
            }

            string nombreSeleccionado = SeleccionarAleatorio(nombresDisponibles);
            return ResultadoGeneracion.Exito(nombreSeleccionado);
        }

        private static void RegistrarRecursoNoEncontrado(CultureInfo cultura)
        {
            _logger.ErrorFormat(
                "No se encontraron nombres para cultura: {0}",
                cultura);
        }

        private static void RegistrarListaVacia()
        {
            _logger.Warn("La lista de nombres parseada esta vacia.");
        }

        private static void RegistrarNombresAgotados()
        {
            _logger.Info("Todos los nombres disponibles ya han sido utilizados.");
        }

        private static string ObtenerRecursoNombres(CultureInfo cultura)
        {
            string opciones = Lang.ResourceManager.GetString("invitadoNombres", cultura);

            if (string.IsNullOrWhiteSpace(opciones) && 
                !cultura.Equals(CultureInfo.InvariantCulture))
            {
                RegistrarRecursoFaltanteEnCultura(cultura);
                return Lang.ResourceManager.GetString(
                    "invitadoNombres",
                    CultureInfo.InvariantCulture);
            }

            return opciones;
        }

        private static void RegistrarRecursoFaltanteEnCultura(CultureInfo cultura)
        {
            _logger.WarnFormat(
                "Falta recurso 'invitadoNombres' en {0}, usando Invariant.",
                cultura);
        }

        private static string[] ParsearNombres(string cadenaOpciones)
        {
            return cadenaOpciones
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(nombre => nombre.Trim())
                .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                .ToArray();
        }

        private static string[] FiltrarNombresDisponibles(
            string[] todos,
            IEnumerable<string> excluidos)
        {
            if (excluidos == null)
            {
                return todos;
            }

            var conjuntoExcluidos = new HashSet<string>(
                excluidos.Where(nombre => !string.IsNullOrWhiteSpace(nombre)),
                StringComparer.OrdinalIgnoreCase);

            return todos.Where(nombre => !conjuntoExcluidos.Contains(nombre)).ToArray();
        }

        private string SeleccionarAleatorio(string[] opciones)
        {
            lock (_sincronizacion)
            {
                int indice = _aleatorio.Next(opciones.Length);
                return opciones[indice];
            }
        }
    }
}