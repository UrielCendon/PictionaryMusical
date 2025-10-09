using Datos.DAL.Implementaciones;
using Datos.DAL.Interfaces;
using Servicios.Contratos;
using Servicios.Contratos.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Servicios
{
    public class CuentaManejador : ICuentaManejador
    {
        private readonly ICuentaRepositorio _repo;

        public CuentaManejador() : this(new CuentaRepositorio()) { }
        public CuentaManejador(ICuentaRepositorio repo) => _repo = repo;

        public bool RegistrarCuenta(NuevaCuentaDTO nuevaCuenta)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(nuevaCuenta.Contrasena);
            return _repo.CreateAccount(
                email: nuevaCuenta.Correo,
                passwordHash: hash,
                usuario: nuevaCuenta.Usuario,
                nombre: nuevaCuenta.Nombre,
                apellido: nuevaCuenta.Apellido,
                avatarId: nuevaCuenta.AvatarId
            );
        }
    }
}
