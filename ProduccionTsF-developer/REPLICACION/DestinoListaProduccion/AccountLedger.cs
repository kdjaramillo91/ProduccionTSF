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
    
    public partial class AccountLedger
    {
        public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string typeCount { get; set; }
        public Nullable<int> id_costCenter { get; set; }
        public Nullable<int> id_costSubCenter { get; set; }
        public int id_company { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public Nullable<int> accountingTemplate { get; set; }
        public string id_auxiliary { get; set; }
        public string nameAuxiliar { get; set; }
        public string nameCenterCost { get; set; }
        public string nameSubCenterCost { get; set; }
        public string typeAuxiliar { get; set; }
    
        public virtual AccountingTemplate AccountingTemplate1 { get; set; }
        public virtual Company Company { get; set; }
    }
}
