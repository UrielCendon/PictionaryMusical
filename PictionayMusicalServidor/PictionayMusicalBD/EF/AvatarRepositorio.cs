using Logica.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.EF
{
    public class AvatarRepositorio : IAvatarRepositorio
    {
        private readonly BaseDatosPruebaEntities1 _contexto;
        public AvatarRepositorio(BaseDatosPruebaEntities1 contexto) => _contexto = contexto;
        IEnumerable<Logica.Entidades.Avatar> IAvatarRepositorio.ObtenerTodos() =>
        _contexto.Avatar.Select(avatar => new Logica.Entidades.Avatar{ id = avatar.idAvatar, nombre = avatar.Nombre_Avatar, rutaRelativa = avatar.Avatar_Ruta }).ToList();
    }
}