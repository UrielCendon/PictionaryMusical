using Datos;
using Datos.EF;
using Logica.Funciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HostServidor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var contexto = new BaseDatosPruebaEntities1();

            var cuentaRepo = new CuentaRepositorio(contexto);
            var cuentaSvc = new CuentaManejador(cuentaRepo);

            var avataresRepo = new AvatarRepositorio(contexto);
            var avataresSvc = new CatalogoAvatares(avataresRepo);

            using (var hostCuenta = new ServiceHost(cuentaSvc))
            using (var hostAvatares = new ServiceHost(avataresSvc))
            {
                hostCuenta.Open();
                hostAvatares.Open();
                Console.WriteLine("Host arriba. Enter para salir.");
                Console.ReadLine();
            }
        }
    }
}
