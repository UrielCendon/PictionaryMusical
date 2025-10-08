using Logica.DAL;
using Logica.Entidades;
using Servicios.Contratos;
using Servicios.Contratos.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Logica.Funciones
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

    public class CuentaManejador : ICuentaManejador
    {
        private readonly ICuentaRepositorio _repoCuenta;
        public CuentaManejador(ICuentaRepositorio repo) => _repoCuenta = repo;

        public bool RegistrarCuenta(NuevaCuentaDTO nuevaCuenta)
        {
            var cuenta = new CuentaUsuario
            {
                correo = nuevaCuenta.correo,
                usuario = nuevaCuenta.usuario,
                nombre = nuevaCuenta.nombre,
                apellido = nuevaCuenta.apellido,
                avatarId = nuevaCuenta.avatarId,
                contraseñaHash = BCrypt.Net.BCrypt.HashPassword(nuevaCuenta.contraseña)
            };
            return _repoCuenta.Registrar(cuenta);
        }
    }
}
