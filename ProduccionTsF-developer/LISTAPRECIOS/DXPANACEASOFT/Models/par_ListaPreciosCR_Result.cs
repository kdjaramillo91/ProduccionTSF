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
    
    public partial class par_ListaPreciosCR_Result
    {
        public string NombreLista { get; set; }
        public string NombreListaBase { get; set; }
        public string NombreListaBaseSimple { get; set; }
        public string NombreListaBaseSingle { get; set; }
        public string NombreProveedor { get; set; }
        public Nullable<System.DateTime> FechaAprobacionComprador { get; set; }
        public Nullable<System.DateTime> FechaAprobacionJefeComercial { get; set; }
        public Nullable<System.DateTime> FechaAprobacionGerente { get; set; }
        public string NombreComprador { get; set; }
        public string NombreJefeComercial { get; set; }
        public string NombreGerente { get; set; }
        public string EstadoDocumento { get; set; }
        public string NombreCompletoPrecioLista { get; set; }
        public bool EsCotizacion { get; set; }
        public string NombreProcesoLista { get; set; }
        public string CodigoTalla { get; set; }
        public string NombreTalla { get; set; }
        public Nullable<decimal> PrecioCotizado { get; set; }
        public Nullable<decimal> PrecioReferencial { get; set; }
        public Nullable<decimal> Diferencia { get; set; }
        public decimal Penalty { get; set; }
        public Nullable<decimal> PrecioConPenalty { get; set; }
        public string NombreTallaDetalle { get; set; }
        public string EtiquetaTalla { get; set; }
        public string FechaValido { get; set; }
        public int IdCompania { get; set; }
        public string CodigoEstadoDocumento { get; set; }
        public decimal Comision { get; set; }
        public decimal PrecioCompra { get; set; }
        public string EsDefinitivo { get; set; }
        public string HaAprobadoComprador { get; set; }
        public string HaAprobadoJefe { get; set; }
        public string HaAprobadoGerente { get; set; }
        public string EsAsistente { get; set; }
    }
}
