using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL.Interfaces
{
    public interface ICuentaRepositorio
    {
        bool CreateAccount(string email, string passwordHash,
                           string usuario, string nombre, string apellido,
                           int avatarId);
    }
}
