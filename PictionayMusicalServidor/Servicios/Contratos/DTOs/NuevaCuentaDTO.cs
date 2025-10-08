using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Contratos.DTOs
{
    [DataContract]
    public class NuevaCuentaDTO
    {
        [DataMember] 
        public string correo { get; set; }
        [DataMember] 
        public string contraseña { get; set; }
        [DataMember] 
        public string usuario { get; set; }
        [DataMember] 
        public string nombre { get; set; }
        [DataMember] 
        public string apellido { get; set; }
        [DataMember] 
        public int avatarId { get; set; }
    }
}
