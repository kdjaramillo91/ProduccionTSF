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
    
    public partial class liquidacionflete
    {
        public string TipoLiquidacion { get; set; }
        public int SecuenciaLiquidacion { get; set; }
        public System.DateTime FechaLiquidacion { get; set; }
        public string EstadoLiquidacion { get; set; }
        public int SecuenciaGuia { get; set; }
        public System.DateTime FechaGuia { get; set; }
        public string EstadoGuia { get; set; }
        public string Facturador { get; set; }
        public string Placa { get; set; }
        public string Conductor { get; set; }
        public string CiaTransporte { get; set; }
        public Nullable<decimal> PrecioDiasTotal { get; set; }
        public Nullable<decimal> PrecioExtension { get; set; }
        public Nullable<decimal> PrecioAjusteTotal { get; set; }
        public Nullable<decimal> PrecioTotalTotal { get; set; }
        public Nullable<decimal> PrecioSubTotalTotal { get; set; }
        public Nullable<decimal> PrecioAnticipoTotal { get; set; }
        public decimal PrecioFleteTotal { get; set; }
        public decimal PrecioCanceladoTotal { get; set; }
        public string NumeroFactura { get; set; }
        public decimal PrecioDiasDetalle { get; set; }
        public decimal PrecioExtensionDetalle { get; set; }
        public decimal PrecioAjusteDetalle { get; set; }
        public decimal PrecioTotalDetalle { get; set; }
        public decimal PrecioSubTotalDetalle { get; set; }
        public decimal PrecioAnticipoDetalle { get; set; }
        public decimal PrecioFleteDetalle { get; set; }
        public Nullable<decimal> PrecioCanceladoDetalle { get; set; }
        public bool DetalleActivo { get; set; }
    }
}
