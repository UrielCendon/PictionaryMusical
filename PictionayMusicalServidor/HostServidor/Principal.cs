using Datos;
using Datos.EF;
using log4net;
using Logica.Funciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HostServidor
{
    internal class Principal
    {
        private static readonly ILog Bitacora = LogManager.GetLogger(typeof(Principal));
        static void Main(string[] args)
        {
            ServiceHost hostCuenta = null;
            ServiceHost hostAvatares = null;

            try
            {
                var contexto = new BaseDatosPruebaEntities1();

                var cuentaRepo = new CuentaRepositorio(contexto);
                var cuentaSvc = new CuentaManejador(cuentaRepo);

                var avataresRepo = new AvatarRepositorio(contexto);
                var avataresSvc = new CatalogoAvatares(avataresRepo);

                hostCuenta = new ServiceHost(cuentaSvc);
                hostAvatares = new ServiceHost(avataresSvc);

                hostCuenta.Open();
                hostAvatares.Open();
                Bitacora.Info("Host arriba. Enter para salir.");
                Console.ReadLine();
            }
            catch (AddressAccessDeniedException ex)
            {
                Bitacora.Error("Permisos insuficientes para abrir los puertos configurados.", ex);
            }
            catch (AddressAlreadyInUseException ex)
            {
                Bitacora.Error("El puerto ya está en uso.", ex);
            }
            catch (TimeoutException ex)
            {
                Bitacora.Error("Timeout al iniciar el host WCF.", ex);
            }
            catch (CommunicationException ex)
            {
                Bitacora.Error("Error de comunicación al iniciar el host WCF.", ex);
            }
            finally
            {
                CerrarFormaSegura(hostAvatares);
                CerrarFormaSegura(hostCuenta);
                Bitacora.Info("Host detenido.");
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
            catch (TimeoutException ex)
            {
                Bitacora.Warn("Timeout al cerrar ServiceHost; abortando.", ex);
                host.Abort();
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Bitacora.Warn("ServiceHost en estado Faulted; abortando.", ex);
                host.Abort();
            }
            catch (ObjectDisposedException ex)
            {
                Bitacora.Warn("ServiceHost ya estaba dispuesto. Nada que cerrar.", ex);
            }
            catch (InvalidOperationException ex)
            {
                Bitacora.Warn("Operación inválida al cerrar ServiceHost; abortando.", ex);
                host.Abort();
            }
        }
    }
}
