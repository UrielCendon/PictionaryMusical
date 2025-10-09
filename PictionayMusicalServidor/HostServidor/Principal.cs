using Datos.Modelo;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HostServidor
{
    class Principal
    {
        private static readonly ILog Bitacora = LogManager.GetLogger(typeof(Principal));

        static void Main()
        {
            Directory.CreateDirectory("Logs");

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                Bitacora.Fatal("Excepción no controlada.", (Exception)e.ExceptionObject);

            using (var hostCuenta = new ServiceHost(typeof(Servicios.Servicios.CuentaManejador)))
            using (var hostAvatares = new ServiceHost(typeof(Servicios.Servicios.CatalogoAvatares)))
            {
                try
                {
                    hostCuenta.Open();
                    Bitacora.Info("Servicio Cuenta iniciado.");
                    foreach (var ep in hostCuenta.Description.Endpoints)
                        Bitacora.Info($"Cuenta -> {ep.Address} ({ep.Binding.Name})");

                    hostAvatares.Open();
                    Bitacora.Info("Servicio Avatares iniciado.");
                    foreach (var ep in hostAvatares.Description.Endpoints)
                        Bitacora.Info($"Avatares -> {ep.Address} ({ep.Binding.Name})");

                    Console.WriteLine("Servicios arriba. ENTER para salir.");
                    Console.ReadLine();
                }
                catch (AddressAccessDeniedException ex) { Bitacora.Error("Permisos insuficientes para abrir los puertos.", ex); }
                catch (AddressAlreadyInUseException ex) { Bitacora.Error("Puerto en uso.", ex); }
                catch (TimeoutException ex) { Bitacora.Error("Timeout al iniciar el host.", ex); }
                catch (CommunicationException ex) { Bitacora.Error("Error de comunicación al iniciar el host.", ex); }
                finally
                {
                    CerrarFormaSegura(hostAvatares);
                    CerrarFormaSegura(hostCuenta);
                    Bitacora.Info("Host detenido.");
                }
            }
        }

        private static void CerrarFormaSegura(ServiceHost host)
        {
            if (host == null) return;
            try
            {
                if (host.State != CommunicationState.Closed)
                    host.Close();
            }
            catch (Exception ex)
            {
                Bitacora.Warn("Cierre no limpio; abortando.", ex);
                host.Abort();
            }
        }
    }
}
