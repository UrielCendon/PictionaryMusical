using Logica.DAL;
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

    public class CatalogoAvatares : ICatalogoAvatares
    {
        private readonly IAvatarRepositorio _repoAvatar;
        public CatalogoAvatares(IAvatarRepositorio repo) => _repoAvatar = repo;

        public List<AvatarDTO> ObtenerAvataresDisponibles() =>
            _repoAvatar.ObtenerTodos()
                       .Select(avatar => new AvatarDTO
                       {
                           Id = avatar.id,
                           Nombre = avatar.nombre,
                           RutaRelativa = avatar.rutaRelativa
                       })
                       .ToList();
    }
}
