//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DXPANACEASOFT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RemissionGuideRiverSecuritySeal
    {
        public int id { get; set; }
        public int id_remissionGuideRiver { get; set; }
        public string number { get; set; }
        public int id_travelType { get; set; }
        public Nullable<int> id_exitState { get; set; }
        public Nullable<int> id_arrivalState { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
    
        public virtual RemissionGuideRiver RemissionGuideRiver { get; set; }
        public virtual RemissionGuideTravelType RemissionGuideTravelType { get; set; }
        public virtual SecuritySealState SecuritySealState { get; set; }
        public virtual SecuritySealState SecuritySealState1 { get; set; }
    }
}
