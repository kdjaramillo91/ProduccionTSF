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
    
    public partial class TblFaTdmCard
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblFaTdmCard()
        {
            this.TblFaTdmPuerto = new HashSet<TblFaTdmPuerto>();
        }
    
        public short NNuIdCard { get; set; }
        public string CCiCodigoCard { get; set; }
        public string CDsDescripcion { get; set; }
        public Nullable<short> NNuIdFrame { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblFaTdmPuerto> TblFaTdmPuerto { get; set; }
        public virtual TblFaTdmFrame TblFaTdmFrame { get; set; }
    }
}
