using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAREPLICATEPRODUCTION.GruposToProd.Modelos
{
    public class GrupoPersonaRol
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int idCompania { get; set; }
        public int idRol { get; set; }
        public string nombreRol { get; set; }
        public bool activo { get; set; }
    }
    public class GrupoPersonaRolDetalle
    {
        public int id { get; set; }
        public int idPersona { get; set; }
        public string identificacionPersona { get; set; }
    }

    public class GrupoPersonaDTO
    {
        public GrupoPersonaRol grupoPersona { get; set; }

        public List<GrupoPersonaRolDetalle> grupoPersonaDetalle { get; set; }
    }
}
