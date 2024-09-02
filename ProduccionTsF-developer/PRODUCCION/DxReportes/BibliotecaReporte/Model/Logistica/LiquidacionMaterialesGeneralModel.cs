using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaReporte.Model.Logistica
{
   internal  class LiquidacionMaterialesGeneralModel
    {
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public DateTime FechaLiquidacion { get; set; }
        public int NumeroLiquidacion { get; set; }
        public int SecuenciaGuia { get; set; }
        public DateTime FechaEmisionGuia { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoProducto { get; set; }
        public decimal CantidadDetail { get; set; }
        public decimal CantidadFacturada { get; set; }
        public decimal PrecioUnitarioDetail { get; set; }
        public decimal SubTotalDetail { get; set; }
        public decimal SubtotalTaxDetail { get; set; }
        public decimal TotalDetail { get; set; }
        public string Telefono { get; set; }
        public string NombreCompania { get; set; }
        public string Unidad_Medida { get; set; }
        public string Ruc_Compania { get; set; }
        public string Estado { get; set; }
        public string Proceso { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string DateInit { get; set; }
        public string DateEnd { get; set; }
    }
}
