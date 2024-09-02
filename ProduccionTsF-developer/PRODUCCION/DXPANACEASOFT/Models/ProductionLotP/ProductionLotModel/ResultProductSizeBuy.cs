using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultProductSizeBuy
    {
        public string Compania { get; set; }
        public string Ciudad { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string Comisionista { get; set; }
        public string Proveedores { get; set; }
        public decimal? idestado { get; set; }
        public int AgruparTipoProducto { get; set; }
        public string TipoProducto { get; set; }
        public int AgruparCategoriaProducto { get; set; }
        public string CategoriaProducto { get; set; }
        public int AgruparTallas { get; set; }
        public string Talla { get; set; }
        public decimal? Rendimiento { get; set; }
        public decimal? Valor { get; set; }
        public decimal? PrecioRendimiento { get; set; }
        public decimal? PrecioRef { get; set; }

    }
}