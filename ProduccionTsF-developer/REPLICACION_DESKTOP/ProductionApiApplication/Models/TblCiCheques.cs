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
    
    public partial class TblCiCheques
    {
        public string CCiBanco { get; set; }
        public string CCiCtaCte { get; set; }
        public string NnuCheque { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CCiTipoMvto { get; set; }
        public decimal NNuSecuencia { get; set; }
        public decimal NNuAnio { get; set; }
        public System.DateTime DFmFecDocum { get; set; }
        public string CDsBeneficiario { get; set; }
        public decimal NNuValor { get; set; }
        public string CCePreavisa { get; set; }
        public string CCtChqEntrega { get; set; }
        public Nullable<System.DateTime> DFxEntregaChq { get; set; }
        public string CTxImpreso { get; set; }
        public string CCeImprCheque { get; set; }
        public string CCtOrigenCheque { get; set; }
        public string CCeFirma { get; set; }
        public Nullable<System.DateTime> DFxFirma { get; set; }
        public string CCiUsuarioEntrega { get; set; }
        public string CDsEstacionEntrega { get; set; }
        public Nullable<System.DateTime> DFxModificaEntrega { get; set; }
    }
}
