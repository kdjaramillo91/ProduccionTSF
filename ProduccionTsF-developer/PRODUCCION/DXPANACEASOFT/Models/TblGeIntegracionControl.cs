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
    
    public partial class TblGeIntegracionControl
    {
        public long id { get; set; }
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public short NNuAnio { get; set; }
        public short NNuPeriodo { get; set; }
        public string CodOrigen { get; set; }
        public string TipoComprobante { get; set; }
        public string ControlComprobante { get; set; }
        public string CodTipoComprobante { get; set; }
        public int CodLoteOutput { get; set; }
        public System.DateTime FechaIntegracion { get; set; }
        public Nullable<System.DateTime> FechaRespuesta { get; set; }
        public string CodEstado { get; set; }
        public bool Procesado { get; set; }
        public string MensajeError { get; set; }
    }
}
