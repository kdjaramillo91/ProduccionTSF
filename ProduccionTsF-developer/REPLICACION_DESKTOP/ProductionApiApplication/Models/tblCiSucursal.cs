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
    
    public partial class tblCiSucursal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblCiSucursal()
        {
            this.TblCiSaldos = new HashSet<TblCiSaldos>();
            this.TblCiSaldosPres = new HashSet<TblCiSaldosPres>();
            this.TblCpCabOrdenPagoBatch = new HashSet<TblCpCabOrdenPagoBatch>();
            this.TblFaEstadisticaProducto = new HashSet<TblFaEstadisticaProducto>();
            this.TblFaHistoricoVtaProducto = new HashSet<TblFaHistoricoVtaProducto>();
        }
    
        public string CCiCia { get; set; }
        public string CCiDivision { get; set; }
        public string CCiSucursal { get; set; }
        public string CDsSucursal { get; set; }
        public string CTxDireccion { get; set; }
        public string CTxReferencia { get; set; }
        public string CCeSucursal { get; set; }
        public string CNuSerieDcto { get; set; }
        public string CNuAutorizacionSriDcto { get; set; }
        public string CNuEmpresaBanco { get; set; }
        public string CNuCtaCteBanco { get; set; }
        public string CNuFax { get; set; }
        public string CNuTelefono { get; set; }
        public string CNoEmpresaBanco { get; set; }
        public string CsnSucursalMatriz { get; set; }
        public string CCiUnidadPeso { get; set; }
        public string CSNReplicarCierre { get; set; }
        public string CTxSlogan { get; set; }
        public string CCtGuiaRemision { get; set; }
        public string CCtSecuenciaDcto { get; set; }
        public string CCtPrecioOC { get; set; }
        public string CCiEmpleadoAdministrador { get; set; }
        public string CCiTipoPrecioProducto { get; set; }
        public string CciVendedor { get; set; }
        public string CDsEmail { get; set; }
        public string CCiCodigoAgenciaUAF { get; set; }
        public string CSnDatosMovil { get; set; }
        public string CCiTipoRecibero { get; set; }
        public string CNuSerieRecibero { get; set; }
        public string CCiTipoCanalVta { get; set; }
        public string CCiCanalVenta { get; set; }
    
        public virtual TblCiDivision TblCiDivision { get; set; }
        public virtual TblCiDivision TblCiDivision1 { get; set; }
        public virtual TblCiDivision TblCiDivision2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblCiSaldos> TblCiSaldos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblCiSaldosPres> TblCiSaldosPres { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblCpCabOrdenPagoBatch> TblCpCabOrdenPagoBatch { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblFaEstadisticaProducto> TblFaEstadisticaProducto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblFaHistoricoVtaProducto> TblFaHistoricoVtaProducto { get; set; }
    }
}
