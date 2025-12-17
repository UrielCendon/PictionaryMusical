using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Vista;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PictionaryMusicalCliente.Utilidades
{
    /// <summary>
    /// Comportamiento adjunto para validar el limite de caracteres en controles de 
    /// entrada y mostrar un aviso cuando se alcanza el maximo permitido.
    /// </summary>
    public static class LimiteCaracteresEntrada
    {
        /// <summary>
        /// Identifica la propiedad adjunta HabilitarAvisoLimite.
        /// </summary>
        public static readonly DependencyProperty HabilitarAvisoLimiteProperty =
            DependencyProperty.RegisterAttached(
                "HabilitarAvisoLimite",
                typeof(bool),
                typeof(LimiteCaracteresEntrada),
                new PropertyMetadata(false, AlCambiarHabilitarAvisoLimite));

        /// <summary>
        /// Obtiene el valor de la propiedad HabilitarAvisoLimite del objeto especificado.
        /// </summary>
        /// <param name="objeto">
        /// El objeto de dependencia del cual se obtiene el valor.
        /// </param>
        /// <returns>
        /// True si el aviso de limite esta habilitado, False en caso contrario.
        /// </returns>
        public static bool GetHabilitarAvisoLimite(DependencyObject objeto)
        {
            return objeto is not null && (bool)objeto.GetValue(HabilitarAvisoLimiteProperty);
        }

        /// <summary>
        /// Establece el valor de la propiedad HabilitarAvisoLimite en el objeto 
        /// especificado.
        /// </summary>
        /// <param name="objeto">
        /// El objeto de dependencia al cual se le asigna el valor.
        /// </param>
        /// <param name="valor">True para habilitar el aviso de limite de caracteres.</param>
        public static void SetHabilitarAvisoLimite(DependencyObject objeto, bool valor)
        {
            objeto?.SetValue(HabilitarAvisoLimiteProperty, valor);
        }

        private static void AlCambiarHabilitarAvisoLimite(
            DependencyObject dependencia,
            DependencyPropertyChangedEventArgs argumentosEvento)
        {
            bool habilitar = argumentosEvento.NewValue is bool valor && valor;

            if (dependencia is TextBox cuadroTexto)
            {
                if (habilitar)
                {
                    cuadroTexto.PreviewTextInput += ValidarEntradaTexto;
                    DataObject.AddPastingHandler(cuadroTexto, ManejarPegadoTexto);
                }
                else
                {
                    cuadroTexto.PreviewTextInput -= ValidarEntradaTexto;
                    DataObject.RemovePastingHandler(cuadroTexto, ManejarPegadoTexto);
                }
            }
            else if (dependencia is PasswordBox campoContrasena)
            {
                if (habilitar)
                {
                    campoContrasena.PreviewTextInput += ValidarEntradaContrasena;
                    DataObject.AddPastingHandler(campoContrasena, ManejarPegadoContrasena);
                }
                else
                {
                    campoContrasena.PreviewTextInput -= ValidarEntradaContrasena;
                    DataObject.RemovePastingHandler(campoContrasena, ManejarPegadoContrasena);
                }
            }
        }

        private static void ValidarEntradaTexto(
            object remitente,
            TextCompositionEventArgs argumentosEvento)
        {
            if (remitente is not TextBox cuadroTexto)
            {
                return;
            }

            int longitudActual = cuadroTexto.Text.Length - cuadroTexto.SelectionLength;
            int longitudNueva = longitudActual + argumentosEvento.Text.Length;

            if (cuadroTexto.MaxLength > 0 && longitudNueva > cuadroTexto.MaxLength)
            {
                argumentosEvento.Handled = true;
                MostrarAvisoLimite();
            }
        }

        private static void ValidarEntradaContrasena(
            object remitente,
            TextCompositionEventArgs argumentosEvento)
        {
            if (remitente is not PasswordBox campoContrasena)
            {
                return;
            }

            int longitudActual = campoContrasena.Password.Length;
            int longitudNueva = longitudActual + argumentosEvento.Text.Length;

            if (campoContrasena.MaxLength > 0 && longitudNueva > campoContrasena.MaxLength)
            {
                argumentosEvento.Handled = true;
                MostrarAvisoLimite();
            }
        }

        private static void ManejarPegadoTexto(
            object remitente,
            DataObjectPastingEventArgs argumentosEvento)
        {
            if (remitente is not TextBox cuadroTexto)
            {
                argumentosEvento.CancelCommand();
                return;
            }

            if (!argumentosEvento.DataObject.GetDataPresent(DataFormats.Text))
            {
                argumentosEvento.CancelCommand();
                return;
            }

            string textoPegado = argumentosEvento.DataObject.GetData(DataFormats.Text) 
                as string ?? string.Empty;

            int longitudActual = cuadroTexto.Text.Length - cuadroTexto.SelectionLength;
            int longitudNueva = longitudActual + textoPegado.Length;

            if (cuadroTexto.MaxLength > 0 && longitudNueva > cuadroTexto.MaxLength)
            {
                argumentosEvento.CancelCommand();
                MostrarAvisoLimite();
            }
        }

        private static void ManejarPegadoContrasena(
            object remitente,
            DataObjectPastingEventArgs argumentosEvento)
        {
            if (remitente is not PasswordBox campoContrasena)
            {
                argumentosEvento.CancelCommand();
                return;
            }

            if (!argumentosEvento.DataObject.GetDataPresent(DataFormats.Text))
            {
                argumentosEvento.CancelCommand();
                return;
            }

            string textoPegado = argumentosEvento.DataObject.GetData(DataFormats.Text) 
                as string ?? string.Empty;

            int longitudActual = campoContrasena.Password.Length;
            int longitudNueva = longitudActual + textoPegado.Length;

            if (campoContrasena.MaxLength > 0 && longitudNueva > campoContrasena.MaxLength)
            {
                argumentosEvento.CancelCommand();
                MostrarAvisoLimite();
            }
        }

        private static void MostrarAvisoLimite()
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new System.Action(() =>
                {
                    Avisos ventanaAviso = new Avisos(Lang.avisoTextoLimiteCaracteres);
                    ventanaAviso.ShowDialog();
                }));
        }
    }
}
