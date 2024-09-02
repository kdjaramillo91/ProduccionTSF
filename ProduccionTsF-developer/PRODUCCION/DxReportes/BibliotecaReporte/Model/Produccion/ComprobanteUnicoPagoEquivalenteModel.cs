using System;

namespace BibliotecaReporte.Model.Produccion
{
    internal class ComprobanteUnicoPagoEquivalenteModel
    {
        public string Compania { get; set; }
        public int Nliquidacion { get; set; }
        public string Titulo { get; set; }
        public string Proveedor { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public string IdentificacionProveedor { get; set; }
        public string Piscina { get; set; }
        public string Aguaje { get; set; }
        public string Sector { get; set; }
        public string Lote { get; set; }
        public string INP { get; set; }
        public string NoASC { get; set; }
        public string SiASC { get; set; }
        public decimal LibrasRecibidas { get; set; }
        public decimal LibrasRemitidas { get; set; }
        public decimal Sobrante { get; set; }
        public decimal BasuraEntero { get; set; }
        public decimal BasuraDescabezado { get; set; }
        public decimal LbsProcesadas { get; set; }
        public decimal RendimientoEntero { get; set; }
        public decimal RendimientoCOla { get; set; }
        public int AGR1EnteroCola { get; set; }
        public string TipoProducto { get; set; }
        public int AGR2Clase { get; set; }
        public string Categoria { get; set; }
        public int AGR3Producto { get; set; }
        public int Distribuido { get; set; }
        public int Normal { get; set; }
        public string CodigoProducto { get; set; }
        public string Producto { get; set; }
        public int AGR4Talla { get; set; }
        public string Talla { get; set; }
        public decimal CantidadLibrasEntero { get; set; }
        public decimal CantidadLibrasEntero2 { get; set; }
        public bool Distributedd { get; set; }
        public decimal TotalCabCol { get; set; }
        public decimal Totaltopay { get; set; }

        public decimal PrecioLibras { get; set; }

        public decimal PrecioKilos { get; set; }

    }
}