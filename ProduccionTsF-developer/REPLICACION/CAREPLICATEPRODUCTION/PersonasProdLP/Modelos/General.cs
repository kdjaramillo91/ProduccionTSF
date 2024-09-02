using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAREPLICATEPRODUCTION.PersonasProdLP.Modelos
{
    public class LPPersona
    {
        public int id { get; set; }
        public int idTipoPersona { get; set; }
        public int idTipoIdentificacion { get; set; }
        public string numeroIdentificacion { get; set; }
        public string nombreCompleto { get; set; }
        public byte[] foto { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public int idCompania { get; set; }
        public bool activo { get; set; }
        public int idUsuarioCreacion { get; set; }
        public DateTime fechaCreacion { get; set; }
        public int idUsuarioModificacion { get; set; }
        public DateTime fechaModificacion { get; set; }
        public string codigoCI { get; set; }
        public string bCC { get; set; }
        public string nombreComercial { get; set; }
        public string numeroCelular { get; set; }
    }
    public class ProducionPersona
    {
        public int id { get; set; }
        public int idTipoPersona { get; set; }
        public int idTipoIdentificacion { get; set; }
        public string numeroIdentificacion { get; set; }
        public string nombreCompleto { get; set; }
        public byte[] foto { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public int? idCompania { get; set; }
        public bool activo { get; set; }
        public int idUsuarioCreacion { get; set; }
        public DateTime fechaCreacion { get; set; }
        public int idUsuarioModificacion { get; set; }
        public DateTime fechaModificacion { get; set; }
        public string codigoCI { get; set; }
        public string bCC { get; set; }
        public string nombreComercial { get; set; }
        public string numeroCelular { get; set; }
        public string emailCC { get; set; }
    }
    public class RolP
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int idCompania { get; set; }
        public bool activo { get; set; }
    }
    public class Empleado
    {
        public int idPersona { get; set; }

        public int idDepartamento { get; set; }
    }
    public class TipoIdentificacion
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string codigoSRI { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public int idCompania { get; set; }
    }
    public class TipoPersona
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public int idCompania { get; set; }
    }
    public class Departamento
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public int idCompania { get; set; }
    }

    public class PersonSourceDTO
    {
        public ProducionPersona persona { get; set; }
        public TipoIdentificacion tipoIdenficacion { get; set; }
        public TipoPersona tipoPersona { get; set; }
        public Departamento departamento { get; set; }
        public List<RolP> lstRol { get; set; }

        public Empleado empleado { get; set; }
    }
}
