using Logica.DAL;
using Logica.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.EF
{
    public class CuentaRepositorio : ICuentaRepositorio
    {
        private readonly BaseDatosPruebaEntities1 _contexto;
        public CuentaRepositorio(BaseDatosPruebaEntities1 contexto) => _contexto = contexto;

        public bool Registrar(CuentaUsuario cuenta)
        {
            var clasificacion = new Clasificacion { Puntos_Ganados = 0, Rondas_Ganadas = 0 };
            _contexto.Clasificacion.Add(clasificacion); _contexto.SaveChanges();

            var jugador = new Jugador
            {
                Nombre = cuenta.Nombre,
                Apellido = cuenta.Apellido,
                Correo = cuenta.Correo,
                Avatar_idAvatar = cuenta.AvatarId,
                Clasificacion_idClasificacion = clasificacion.idClasificacion
            };
            _contexto.Jugador.Add(jugador); _contexto.SaveChanges();

            var usuario = new Usuario
            {
                Nombre_Usuario = cuenta.Usuario,
                Contrasena = cuenta.ContrasenaHash,
                Jugador_idJugador = jugador.idJugador
            };
            _contexto.Usuario.Add(usuario); _contexto.SaveChanges();
            return true;
        }
    }
}
