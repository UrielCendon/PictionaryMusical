using Datos.DAL.Interfaces;
using Datos.Modelo;
using Datos.Utilidades;
using System.Linq;

namespace Datos.DAL.Implementaciones
{
    public class CuentaRepositorio : ICuentaRepositorio
    {
        public bool CreateAccount(string correo, string contrasenaHash, string usuario, string nombre, string apellido, int avatarId)
        {
            using (var contexto = new BaseDatosPruebaEntities1(Conexion.ObtenerConexion()))
            {
                var clasificacion = new Clasificacion { Puntos_Ganados = 0, Rondas_Ganadas = 0 };
                contexto.Clasificacion.Add(clasificacion);
                contexto.SaveChanges();

                var jugador = new Jugador
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Correo = correo,
                    Avatar_idAvatar = avatarId,
                    Clasificacion_idClasificacion = clasificacion.idClasificacion
                };
                contexto.Jugador.Add(jugador);
                contexto.SaveChanges();

                var usuarioEntidad = new Usuario
                {
                    Nombre_Usuario = usuario,
                    Contrasena = contrasenaHash,
                    Jugador_idJugador = jugador.idJugador
                };
                contexto.Usuario.Add(usuarioEntidad);
                contexto.SaveChanges();

                return true;
            }
        }

        public bool ExisteCorreo(string correo)
        {
            using (var contexto = new BaseDatosPruebaEntities1(Conexion.ObtenerConexion()))
            {
                return contexto.Jugador.Any(j => j.Correo == correo);
            }
        }

        public bool ExisteUsuario(string usuario)
        {
            using (var contexto = new BaseDatosPruebaEntities1(Conexion.ObtenerConexion()))
            {
                return contexto.Usuario.Any(u => u.Nombre_Usuario == usuario);
            }
        }
    }
}
