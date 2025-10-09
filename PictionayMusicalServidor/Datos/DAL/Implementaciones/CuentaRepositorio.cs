using Datos.DAL.Interfaces;
using Datos.Modelo;
using Datos.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL.Implementaciones
{
    public class CuentaRepositorio : ICuentaRepositorio
    {
        public bool CreateAccount(string correo, string contrasenaHash, string usuario, string nombre, string apellido, int avatarId)
        {
            using (var contexto = new BaseDatosPruebaEntities1(Conexion.ObtenerConexion()))
            {
                var clas = new Clasificacion { Puntos_Ganados = 0, Rondas_Ganadas = 0 };
                contexto.Clasificacion.Add(clas);
                contexto.SaveChanges();

                var jugador = new Jugador
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Correo = correo,
                    Avatar_idAvatar = avatarId,
                    Clasificacion_idClasificacion = clas.idClasificacion
                };
                contexto.Jugador.Add(jugador);
                contexto.SaveChanges();

                var user = new Usuario
                {
                    Nombre_Usuario = usuario,
                    Contrasena = contrasenaHash,
                    Jugador_idJugador = jugador.idJugador
                };
                contexto.Usuario.Add(user);
                contexto.SaveChanges();

                return true;
            }
        }
    }
}
