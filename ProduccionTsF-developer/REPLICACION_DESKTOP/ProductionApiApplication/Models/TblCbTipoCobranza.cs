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
    
    public partial class TblCbTipoCobranza
    {
        public short NCiTipoCobranza { get; set; }
        public string CNoTipoCobranza { get; set; }
        public string CCtClaseCobranza { get; set; }
        public string CCtUserSistema { get; set; }
        public string CCtTablaValida { get; set; }
        public short NQnPrioridad { get; set; }
        public string CCeTipoCobranza { get; set; }
        public string CCiSiglaCobranza { get; set; }
        public string CCiBanco { get; set; }
        public decimal NNuPorcRete { get; set; }
        public string CSNModificaPorcRete { get; set; }
        public string CSNExigeAsientoContable { get; set; }
        public string CSNAnticipo { get; set; }
        public string CSNExcedente { get; set; }
        public string CSNAplicaCobro { get; set; }
        public string CSNMuestraAutomaticaCobro { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public System.DateTime DFxIngreso { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public Nullable<System.DateTime> DFxModifica { get; set; }
        public string CCiTipoComprobante { get; set; }
        public string CSNPagoAnticipado { get; set; }
        public string CSNAplicaProcesoFact { get; set; }
        public Nullable<int> NNuDiasAntes { get; set; }
        public string CCiGrupo { get; set; }
        public string CCiTipoRetencion { get; set; }
        public string CSnNoTransferirReteAt { get; set; }
        public string CSnAplicaNCFiscal { get; set; }
        public string CSnAplicaComision { get; set; }
        public string CSnAplicaCanje { get; set; }
        public string CSnAplicaUAF { get; set; }
    }
}
