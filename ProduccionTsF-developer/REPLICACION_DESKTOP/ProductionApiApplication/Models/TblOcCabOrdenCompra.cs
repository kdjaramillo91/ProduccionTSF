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
    
    public partial class TblOcCabOrdenCompra
    {
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public decimal NNuOrdenCompra { get; set; }
        public Nullable<int> NNuAnio { get; set; }
        public string CCiProveedor { get; set; }
        public string CCiBodega { get; set; }
        public string CCiMoneda { get; set; }
        public System.DateTime DFxOrden { get; set; }
        public Nullable<System.DateTime> DFxEntrega { get; set; }
        public string CCiPlazoPago { get; set; }
        public string CCiFormaPago { get; set; }
        public string CCtOrden { get; set; }
        public decimal NVtBrutoBaseIva { get; set; }
        public Nullable<decimal> NVtBrutoBaseCero { get; set; }
        public Nullable<decimal> NVtPorcDscto1 { get; set; }
        public Nullable<decimal> NVtMontoDscto1 { get; set; }
        public Nullable<decimal> NVtPorcDscto2 { get; set; }
        public Nullable<decimal> NVtMontoDscto2 { get; set; }
        public Nullable<decimal> NVtPorcDscto3 { get; set; }
        public Nullable<decimal> NVtMontoDscto3 { get; set; }
        public decimal NVtBaseIva { get; set; }
        public Nullable<decimal> NVtBaseCero { get; set; }
        public decimal NCiPorcIva { get; set; }
        public decimal NVtPorcIva { get; set; }
        public decimal NVtMontoIva { get; set; }
        public decimal NVtMontoFlete { get; set; }
        public Nullable<decimal> NVtMontoAdicional { get; set; }
        public decimal NVtMontoTotal { get; set; }
        public string CCeOrden { get; set; }
        public string CCeFactura { get; set; }
        public string CCePago { get; set; }
        public string CCiDepartamento { get; set; }
        public string CCiEmpleadoSolicita { get; set; }
        public string CDsEnviarA { get; set; }
        public string CDsReferencia { get; set; }
        public string CDsObservacion { get; set; }
        public string CCiUsuarioAprueba { get; set; }
        public Nullable<System.DateTime> DFxAprueba { get; set; }
        public string CCiUsuarioNiega { get; set; }
        public Nullable<System.DateTime> DFxNiega { get; set; }
        public string CCiUsuarioCierra { get; set; }
        public Nullable<System.DateTime> DFxCierra { get; set; }
        public string CDsMotivo { get; set; }
        public string CSNImpresa { get; set; }
        public string CCiUsuarioIngreso { get; set; }
        public string CDsEstacionIngreso { get; set; }
        public System.DateTime DFiIngreso { get; set; }
        public string CCiUsuarioModifica { get; set; }
        public string CDsEstacionModifica { get; set; }
        public Nullable<System.DateTime> DFmModifica { get; set; }
        public string CDsEntregarA { get; set; }
        public string CDsReferencia01 { get; set; }
        public string CDsViaEmbarque { get; set; }
        public string CSNPresupuesto { get; set; }
        public string CDsDirectorioImagen { get; set; }
        public string CDsNombreImagen1 { get; set; }
        public string CDsExtensionImagen1 { get; set; }
        public string CDsNombreImagen2 { get; set; }
        public string CDsExtensionImagen2 { get; set; }
        public string CDsNombreImagen3 { get; set; }
        public string CDsExtensionImagen3 { get; set; }
        public Nullable<int> NNuSecAlterna { get; set; }
        public string CCtTipoCompra { get; set; }
        public Nullable<decimal> NVtPorcDsctoItem1 { get; set; }
        public Nullable<decimal> NVtMontoDsctoItem1 { get; set; }
        public string CCiIdentificacion { get; set; }
        public string CCiVendedor { get; set; }
        public Nullable<int> NIdGeMoneda { get; set; }
        public Nullable<decimal> NQnFactorConvMoneda { get; set; }
        public string CCtPrograma { get; set; }
        public string CCtNegociacionImportacion { get; set; }
        public Nullable<decimal> NVtMontoSeguro { get; set; }
        public Nullable<decimal> NVtMontoOtrosGastos { get; set; }
        public string CSnCosteo { get; set; }
        public Nullable<int> NIdIvMvtoImportacion { get; set; }
        public string CSnIvMvtoImportacion { get; set; }
        public string CCiTipoSustento { get; set; }
        public string CCiTipoExpoImpo { get; set; }
        public string CNuRefrendoDistrito { get; set; }
        public string CNuRefrendoAnio { get; set; }
        public string CNuRefrendoRegimen { get; set; }
        public string CNuRefrendoCorrelativo { get; set; }
        public string CNuRefrendoVerificador { get; set; }
        public Nullable<int> NIdSecRefeLT { get; set; }
        public Nullable<int> NNuEntrega { get; set; }
        public string CCiMotivoOC { get; set; }
        public Nullable<int> NNuOrdenTrabajo { get; set; }
        public string CNuAPI { get; set; }
        public string CNombreTransportista { get; set; }
        public string CNoPtoEmbarque { get; set; }
        public string CNoTipoCarga { get; set; }
        public Nullable<System.DateTime> DfxVencimiento { get; set; }
        public Nullable<System.DateTime> DfxEmbarque { get; set; }
        public Nullable<System.DateTime> DfxSalidaAduana { get; set; }
        public Nullable<System.DateTime> DfxEntregaDocu { get; set; }
        public string CtxtDau { get; set; }
        public Nullable<decimal> NvtCosteo { get; set; }
        public string CSnDocumento { get; set; }
        public Nullable<System.DateTime> DfxArribo { get; set; }
        public string CCiDptoImportacion { get; set; }
        public Nullable<int> NIdIvProductoVenta { get; set; }
        public string CsnAutorizado { get; set; }
        public string CCiUsuarioAutoriza { get; set; }
        public string CDsEstacionAutoriza { get; set; }
        public Nullable<System.DateTime> DFxAutoriza { get; set; }
        public string CSnLeySolidaria { get; set; }
        public Nullable<decimal> NNuPorIVACompensacion { get; set; }
        public Nullable<decimal> NNuValIVACompensacion { get; set; }
        public string CSnEnviarCorreo { get; set; }
    }
}
