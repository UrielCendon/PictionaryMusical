using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Dialogos;
using PictionaryMusicalCliente.ClienteServicios.Idiomas;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Properties;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.Utilidades.Idiomas;
using System;
using System.Globalization;
using System.Windows;

namespace PictionaryMusicalCliente
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// Actúa como la Raíz de Composición (Composition Root) de la aplicación.
    /// </summary>
    public partial class App : Application
    {
        private IUsuarioAutenticado _usuarioGlobal;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            log4net.Config.XmlConfigurator.Configure();

            IWcfClienteFabrica fabricaClientes = new WcfClienteFabrica();
            IWcfClienteEjecutor ejecutorWcf = new WcfClienteEjecutor();

            _usuarioGlobal = new UsuarioAutenticado();
            ICatalogoAvatares catalogoAvatares = new CatalogoAvataresLocales();
            ICatalogoImagenesPerfil catalogoImagenes = new CatalogoImagenesPerfilLocales();

            ILocalizadorServicio servicioTraductor = new LocalizadorServicio();
            ILocalizacionServicio servicioIdioma = new LocalizacionServicio();
            IManejadorErrorServicio manejadorError = new ManejadorErrorServicio(servicioTraductor);
            IUsuarioMapeador usuarioMapeador = new UsuarioMapeador(_usuarioGlobal);
            IAvisoServicio avisoServicio = new AvisoServicio();
            IMusicaManejador musicaManejador = new MusicaManejador();

            string codigoIdioma = Settings.Default.idiomaCodigo;
            try
            {
                servicioIdioma.EstablecerIdioma(codigoIdioma);
            }
            catch (CultureNotFoundException)
            {
                servicioIdioma.EstablecerCultura(CultureInfo.CurrentUICulture);
            }

            var contextoLocalizacion = new LocalizacionContexto(servicioIdioma);
            this.Resources.Add("Localizacion", contextoLocalizacion);

            IInicioSesionServicio inicioSesionServicio = new InicioSesionServicio(
                ejecutorWcf,
                fabricaClientes,
                manejadorError,
                usuarioMapeador,
                servicioTraductor);

            ICambioContrasenaServicio cambioPassServicio = new CambioContrasenaServicio(
                ejecutorWcf,
                fabricaClientes,
                manejadorError,
                servicioTraductor);

            IVerificacionCodigoDialogoServicio verifCodigoDialogo =
                new VerificacionCodigoDialogoServicio();

            IRecuperacionCuentaServicio recupCuentaDialogo =
                new RecuperacionCuentaDialogoServicio(verifCodigoDialogo, avisoServicio);

            Func<ISalasServicio> fabricaSalas = () =>
                new SalasServicio(fabricaClientes, manejadorError);

            var ventanaInicio = new InicioSesion(
                musicaManejador,
                inicioSesionServicio,
                cambioPassServicio,
                recupCuentaDialogo,
                servicioTraductor,
                fabricaSalas,
                catalogoAvatares,
                avisoServicio
            );

            ventanaInicio.Show();
        }
    }
}