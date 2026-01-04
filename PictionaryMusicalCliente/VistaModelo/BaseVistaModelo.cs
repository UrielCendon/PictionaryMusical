using log4net;
using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Auxiliares;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace PictionaryMusicalCliente.VistaModelo
{
    /// <summary>
    /// Clase base para todos los ViewModels. Implementa notificacion de cambios
    /// y manejo centralizado de excepciones asincronas.
    /// </summary>
    public abstract class BaseVistaModelo : INotifyPropertyChanged
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IVentanaServicio _ventana;
        protected readonly ILocalizadorServicio _localizador;

        /// <summary>
        /// Evento para notificar cambios en las propiedades a la interfaz de usuario.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Evento disparado cuando se detecta una desconexion critica del servidor.
        /// </summary>
        public event Action<string> DesconexionDetectada;

        /// <summary>
        /// Constructor para inyeccion de dependencias.
        /// </summary>
        protected BaseVistaModelo(IVentanaServicio ventana, ILocalizadorServicio localizador)
        {
            _ventana = ventana ?? throw new ArgumentNullException(nameof(ventana));
            _localizador = localizador ?? throw new ArgumentNullException(nameof(localizador));
        }

        /// <summary>
        /// Ejecuta una operacion asincrona capturando excepciones comunes de WCF.
        /// </summary>
        /// <param name="operacion">La tarea asincrona a ejecutar.</param>
        /// <param name="accionError">Accion opcional a ejecutar si ocurre un error.</param>
        protected async Task EjecutarOperacionAsync(Func<Task> operacion, Action<Exception> 
            accionError = null)
        {
            try
            {
                await operacion();
            }
            catch (TimeoutException excepcion)
            {
                ManejarErrorTiempoAgotado(excepcion, accionError);
            }
            catch (EndpointNotFoundException excepcion)
            {
                ManejarErrorServidorNoEncontrado(excepcion, accionError);
            }
            catch (CommunicationObjectFaultedException excepcion)
            {
                ManejarErrorCanalFallido(excepcion, accionError);
            }
            catch (CommunicationObjectAbortedException excepcion)
            {
                ManejarErrorCanalAbortado(excepcion, accionError);
            }
            catch (FaultException excepcion)
            {
                ManejarErrorFallaServicio(excepcion, accionError);
            }
            catch (CommunicationException excepcion)
            {
                ManejarErrorComunicacion(excepcion, accionError);
            }
            catch (ServicioExcepcion excepcion)
            {
                ManejarErrorServicio(excepcion, accionError);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejarErrorOperacionInvalida(excepcion, accionError);
            }
            catch (Exception excepcion)
            {
                ManejarErrorDesconocido(excepcion, accionError);
            }
        }

        /// <summary>
        /// Ejecuta una operacion asincrona y redirige al inicio de sesion en caso de desconexion.
        /// </summary>
        /// <param name="operacion">La tarea asincrona a ejecutar.</param>
        /// <param name="redirigirEnDesconexion">Indica si se debe redirigir al inicio de sesion en
        /// caso de desconexion.</param>
        protected async Task EjecutarOperacionConDesconexionAsync(
            Func<Task> operacion,
            bool redirigirEnDesconexion = true)
        {
            try
            {
                await operacion();
            }
            catch (TimeoutException excepcion)
            {
                var parametros = new ErrorConexionParametros(
                    excepcion,
                    "Tiempo agotado en operacion.",
                    Lang.errorTextoTiempoAgotadoConexion,
                    Lang.errorTextoServidorNoDisponible,
                    redirigirEnDesconexion);
                ManejarErrorDeConexion(parametros);
            }
            catch (EndpointNotFoundException excepcion)
            {
                var parametros = new ErrorConexionParametros(
                    excepcion,
                    "Servidor no encontrado.",
                    Lang.errorTextoServidorDesconectado,
                    Lang.errorTextoServidorNoDisponible,
                    redirigirEnDesconexion);
                ManejarErrorDeConexion(parametros);
            }
            catch (CommunicationObjectFaultedException excepcion)
            {
                var parametros = new ErrorConexionParametros(
                    excepcion,
                    "Canal de comunicacion en estado fallido.",
                    Lang.errorTextoConexionInterrumpida,
                    Lang.errorTextoDesconexionServidor,
                    redirigirEnDesconexion);
                ManejarErrorDeConexion(parametros);
            }
            catch (CommunicationObjectAbortedException excepcion)
            {
                var parametros = new ErrorConexionParametros(
                    excepcion,
                    "Canal de comunicacion abortado.",
                    Lang.errorTextoConexionInterrumpida,
                    Lang.errorTextoDesconexionServidor,
                    redirigirEnDesconexion);
                ManejarErrorDeConexion(parametros);
            }
            catch (FaultException excepcion)
            {
                _logger.Warn("Falla controlada del servicio.", excepcion);
                string mensajeLocalizado = _localizador.Localizar(
                    excepcion.Message, 
                    Lang.errorTextoErrorProcesarSolicitud);
                MostrarErrorEnUI(mensajeLocalizado);
            }
            catch (CommunicationException excepcion)
            {
                var parametros = new ErrorConexionParametros(
                    excepcion,
                    "Error de comunicacion con el servicio.",
                    Lang.errorTextoDesconexionServidor,
                    Lang.errorTextoServidorNoDisponible,
                    redirigirEnDesconexion);
                ManejarErrorDeConexion(parametros);
            }
            catch (ServicioExcepcion excepcion)
            {
                ManejarErrorServicioConDesconexion(excepcion, redirigirEnDesconexion);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Info("Operacion cancelada por objeto dispuesto.", excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida.", excepcion);
                MostrarErrorEnUI(Lang.errorTextoErrorProcesarSolicitud);
            }
            catch (Exception excepcion)
            {
                _logger.Error("Error inesperado en operacion.", excepcion);
                MostrarErrorEnUI(Lang.errorTextoErrorProcesarSolicitud);
            }
        }

        private void ManejarErrorDeConexion(ErrorConexionParametros parametros)
        {
            _logger.Error(parametros.MensajeLog, parametros.Excepcion);
            if (parametros.RedirigirEnDesconexion)
            {
                ManejarDesconexionCritica(parametros.MensajeDesconexion);
            }
            else
            {
                MostrarErrorEnUI(parametros.MensajeError);
            }
        }

        private void ManejarErrorServicioConDesconexion(
            ServicioExcepcion excepcion,
            bool redirigirEnDesconexion)
        {
            _logger.Warn("Excepcion de servicio controlada.", excepcion);
            string mensajeExcepcion = excepcion.Message ?? Lang.errorTextoErrorProcesarSolicitud;

            if (EsExcepcionDeDesconexion(excepcion) && redirigirEnDesconexion)
            {
                ManejarDesconexionCritica(mensajeExcepcion);
            }
            else
            {
                MostrarErrorEnUI(mensajeExcepcion);
            }
        }

        private void ManejarErrorTiempoAgotado(TimeoutException excepcion, 
            Action<Exception> accionError)
        {
            _logger.Error("Tiempo agotado en la solicitud al servidor.", excepcion);
            ManejarError(excepcion, Lang.errorTextoServidorNoDisponible, accionError);
        }

        private void ManejarErrorServidorNoEncontrado(EndpointNotFoundException excepcion, 
            Action<Exception> accionError)
        {
            _logger.Error("No se encontro el servidor.", excepcion);
            ManejarError(excepcion, Lang.errorTextoServidorNoDisponible, accionError);
        }

        private void ManejarErrorCanalFallido(CommunicationObjectFaultedException excepcion, 
            Action<Exception> accionError)
        {
            _logger.Error("El canal de comunicacion esta en estado fallido.", excepcion);
            ManejarError(excepcion, Lang.errorTextoDesconexionServidor, accionError);
        }

        private void ManejarErrorCanalAbortado(CommunicationObjectAbortedException excepcion, 
            Action<Exception> accionError)
        {
            _logger.Error("El canal de comunicacion fue abortado.", excepcion);
            ManejarError(excepcion, Lang.errorTextoConexionInterrumpida, accionError);
        }

        private void ManejarErrorFallaServicio(FaultException excepcion, 
            Action<Exception> accionError)
        {
            _logger.Warn("El servicio reporto una falla controlada.", excepcion);
            string mensaje = _localizador.Localizar(
                excepcion.Message, 
                Lang.errorTextoErrorProcesarSolicitud);
            ManejarError(excepcion, mensaje, accionError);
        }

        private void ManejarErrorComunicacion(CommunicationException excepcion, 
            Action<Exception> accionError)
        {
            _logger.Error("Error de comunicacion con el servicio.", excepcion);
            ManejarError(excepcion, Lang.errorTextoServidorNoDisponible, accionError);
        }

        private void ManejarErrorServicio(ServicioExcepcion excepcion, 
            Action<Exception> accionError)
        {
            _logger.Warn("Excepcion del servicio personalizada.", excepcion);
            string mensaje = excepcion.Message ?? Lang.errorTextoErrorProcesarSolicitud;
            ManejarError(excepcion, mensaje, accionError);
        }

        private void ManejarErrorOperacionInvalida(InvalidOperationException excepcion, 
            Action<Exception> accionError)
        {
            _logger.Error("Operacion invalida ejecutada.", excepcion);
            ManejarError(excepcion, Lang.errorTextoErrorProcesarSolicitud, accionError);
        }

        private void ManejarErrorDesconocido(Exception excepcion, Action<Exception> accionError)
        {
            _logger.Error("Ocurrio un error inesperado.", excepcion);
            ManejarError(excepcion, Lang.errorTextoErrorProcesarSolicitud, accionError);
        }

        private void ManejarError(Exception excepcion, string mensajePorDefecto, Action<Exception> 
            accionError)
        {
            if (accionError != null)
            {
                accionError(excepcion);
            }
            else
            {
                MostrarErrorEnUI(mensajePorDefecto);
            }
        }

        /// <summary>
        /// Muestra un mensaje de error en la interfaz de usuario.
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar.</param>
        protected void MostrarErrorEnUI(string mensaje)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                _ventana?.MostrarError(mensaje);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() => _ventana?.MostrarError(mensaje)));
            }
        }

        /// <summary>
        /// Maneja una desconexion critica mostrando el mensaje y disparando el evento.
        /// </summary>
        /// <param name="mensaje">Mensaje explicativo de la desconexion.</param>
        protected void ManejarDesconexionCritica(string mensaje)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                EjecutarDesconexionCritica(mensaje);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() => EjecutarDesconexionCritica(mensaje)));
            }
        }

        private void EjecutarDesconexionCritica(string mensaje)
        {
            DesconexionDetectada?.Invoke(mensaje);
        }

        /// <summary>
        /// Determina si una ServicioExcepcion es causada por desconexion.
        /// </summary>
        private static bool EsExcepcionDeDesconexion(ServicioExcepcion excepcion)
        {
            return excepcion.Tipo == TipoErrorServicio.Comunicacion ||
                   excepcion.Tipo == TipoErrorServicio.TiempoAgotado;
        }

        /// <summary>
        /// Ejecuta una accion en el dispatcher de la aplicacion.
        /// </summary>
        /// <param name="accion">Accion a ejecutar.</param>
        protected static void EjecutarEnDispatcher(Action accion)
        {
            if (accion == null)
            {
                return;
            }

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                accion();
            }
            else
            {
                dispatcher.BeginInvoke(accion);
            }
        }

        /// <summary>
        /// Asigna un nuevo valor a una propiedad y notifica el cambio si es diferente.
        /// </summary>
        protected bool EstablecerPropiedad<T>(ref T campo, T valor, [CallerMemberName] 
            string nombrePropiedad = null)
        {
            if (Equals(campo, valor))
            {
                return false;
            }

            campo = valor;
            NotificarCambio(nombrePropiedad);
            return true;
        }

        /// <summary>
        /// Dispara el evento PropertyChanged.
        /// </summary>
        protected void NotificarCambio([CallerMemberName] string nombrePropiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }
    }
}
