//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DestinoListaProduccion
{
    using System;
    using System.Collections.Generic;
    
    public partial class CompanyElectronicFacturation
    {
        public int id_company { get; set; }
        public int id_enviromentType { get; set; }
        public int id_emissionType { get; set; }
        public string resolutionNumber { get; set; }
        public bool requireAccounting { get; set; }
        public string rise { get; set; }
        public string certificate { get; set; }
        public string password { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual EmissionType EmissionType { get; set; }
        public virtual EnvironmentType EnvironmentType { get; set; }
    }
}
