//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductionApiApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbsysControlContainerFields
    {
        public int id { get; set; }
        public Nullable<int> id_TController { get; set; }
        public int id_tbsysTableFieldsData { get; set; }
        public string label_tbsysTableFieldsData { get; set; }
    
        public virtual tbsysTableFieldsData tbsysTableFieldsData { get; set; }
        public virtual TController TController { get; set; }
    }
}
