using System;
using System.Windows;
using System.Windows.Input;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Vista;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Provee mecanismos para mostrar mensajes al usuario de manera segura en hilos de UI.
    /// Permite la inyeccion de dependencias para pruebas unitarias.
    /// </summary>
    public class AvisoServicio : IAvisoServicio
    {
        private const int SegundosMinimoEntreMensajes = 2;

        private static readonly object _sincronizacion = new object();
        private static string _ultimoMensaje;
        private static DateTime _tiempoUltimoMensaje = DateTime.MinValue;
        private static readonly TimeSpan TiempoMinimoEntreMensajes = 
            TimeSpan.FromSeconds(SegundosMinimoEntreMensajes);

        /// <summary>
        /// Muestra un mensaje al usuario asegurando que se ejecute en el hilo principal.
        /// Evita mostrar mensajes duplicados consecutivos dentro de un intervalo corto.
        /// </summary>
        /// <param name="mensaje">El contenido del aviso a mostrar.</param>
        public void Mostrar(string mensaje)
        {
            if (Application.Current?.Dispatcher == null)
            {
                return;
            }

            if (EsMensajeDuplicadoReciente(mensaje))
            {
                return;
            }

            if (Application.Current.Dispatcher.CheckAccess())
            {
                EjecutarMostrarReal(mensaje);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => EjecutarMostrarReal(mensaje));
            }
        }

        private static bool EsMensajeDuplicadoReciente(string mensaje)
        {
            lock (_sincronizacion)
            {
                DateTime ahora = DateTime.UtcNow;
                bool esDuplicado = string.Equals(_ultimoMensaje, mensaje, StringComparison.Ordinal)
                    && (ahora - _tiempoUltimoMensaje) < TiempoMinimoEntreMensajes;

                if (!esDuplicado)
                {
                    _ultimoMensaje = mensaje;
                    _tiempoUltimoMensaje = ahora;
                }

                return esDuplicado;
            }
        }

        private static void EjecutarMostrarReal(string mensaje)
        {
            Cursor cursorAnterior = Mouse.OverrideCursor;
            Mouse.OverrideCursor = null;

            try
            {
                new Avisos(mensaje).ShowDialog();
            }
            finally
            {
                Mouse.OverrideCursor = cursorAnterior;
            }
        }
    }
}