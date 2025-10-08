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
                Nombre = cuenta.nombre,
                Apellido = cuenta.apellido,
                Correo = cuenta.correo,
                Avatar_idAvatar = cuenta.avatarId,
                Clasificacion_idClasificacion = clasificacion.idClasificacion
            };
            _contexto.Jugador.Add(jugador); _contexto.SaveChanges();

            var usuario = new Usuario
            {
                Nombre_Usuario = cuenta.usuario,
                Contrasena = cuenta.contraseñaHash,
                Jugador_idJugador = jugador.idJugador
            };
            _contexto.Usuario.Add(usuario); _contexto.SaveChanges();
            return true;
        }
    }
}
