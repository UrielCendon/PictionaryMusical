using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PictionaryMusicalCliente.VistaModelo.Perfil
{
    /// <summary>
    /// Vista Modelo para el dialogo de seleccion de avatares predefinidos.
    /// </summary>
    public class SeleccionAvatarVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAvisoServicio _avisoServicio;
        private readonly SonidoManejador _sonidoManejador;
        private ObjetoAvatar _avatarSeleccionado;

        public SeleccionAvatarVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            IEnumerable<ObjetoAvatar> avatares,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
            : base(ventana, localizador)
        {
            _avisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            if (avatares == null)
            {
                throw new ArgumentNullException(nameof(avatares));
            }

            Avatares = new ObservableCollection<ObjetoAvatar>(avatares);
            ConfirmarSeleccionComando = new ComandoDelegado(_ =>
            {
                _sonidoManejador.ReproducirClick();
                ConfirmarSeleccion();
            });
        }

        /// <summary>
        /// Coleccion de avatares disponibles para seleccionar.
        /// </summary>
        public ObservableCollection<ObjetoAvatar> Avatares { get; }

        /// <summary>
        /// Avatar actualmente seleccionado por el usuario.
        /// </summary>
        public ObjetoAvatar AvatarSeleccionado
        {
            get => _avatarSeleccionado;
            set => EstablecerPropiedad(ref _avatarSeleccionado, value);
        }

        /// <summary>
        /// Comando para confirmar la seleccion del avatar.
        /// </summary>
        public ICommand ConfirmarSeleccionComando { get; }

        /// <summary>
        /// Delegado invocado cuando el usuario confirma su seleccion de avatar.
        /// </summary>
        public Action<ObjetoAvatar> SeleccionConfirmada { get; set; }

        private void ConfirmarSeleccion()
        {
            if (AvatarSeleccionado == null)
            {
				_logger.Warn("Intento de confirmar seleccion sin avatar elegido.");
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(Lang.errorTextoSeleccionAvatarValido);
                return;
            }

            SeleccionConfirmada?.Invoke(AvatarSeleccionado);
        }
    }
}