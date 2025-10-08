using Logica.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.DAL
{
    public interface IAvatarRepositorio
    {
        IEnumerable<Avatar> ObtenerTodos();
    }
}
