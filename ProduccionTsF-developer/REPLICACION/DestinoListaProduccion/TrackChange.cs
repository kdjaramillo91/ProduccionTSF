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
    
    public partial class TrackChange
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrackChange()
        {
            this.TrackChange1 = new HashSet<TrackChange>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_previousChange { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public int id_user { get; set; }
        public System.DateTime date { get; set; }
    
        public virtual DocumentTrackChange DocumentTrackChange { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrackChange> TrackChange1 { get; set; }
        public virtual TrackChange TrackChange2 { get; set; }
    }
}
