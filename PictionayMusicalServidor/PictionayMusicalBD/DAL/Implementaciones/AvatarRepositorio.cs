using Datos.DAL.Interfaces;
using Datos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL.Implementaciones
{
    public class AvatarRepositorio : IAvatarRepositorio
    {
        public IEnumerable<Avatar> ObtenerAvatares()
        {
            using (var contexto = new BaseDatosPruebaEntities1())
            {
                return contexto.Avatar.AsNoTracking().ToList();
            }
        }
    }
}
