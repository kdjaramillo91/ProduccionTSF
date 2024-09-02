using BibliotecaReporte.Dataset.Inventario;
using BibliotecaReporte.Reportes.Inventario;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaReporte.Model.Inventario
{
    internal class KardexCostoModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string NumeroDocumentoInventario { get; set; }
        public string NombreBodega { get; set; }
        public string NombreUbicacion { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public DateTime FechaEmison { get; set; }
        public string NombreMotivoInventario { get; set; }
        public string NombreUnidadMedida { get; set; }
        public decimal MontoEntrada { get; set; }
        public decimal MontoSalida { get; set; }
        public string NameCompania { get; set; }
        public string NameDivision { get; set; }
        public string NameBranchOffice { get; set; }
        public string NumberLot { get; set; }
        public string Provider_name { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountCostUnit { get; set; }
        public decimal AmountCostTotal { get; set; }
        public decimal PreviousPound { get; set; }
        public decimal PreviousCostPound { get; set; }
        public decimal PreviousTotalCostPound { get; set; }
        public decimal EntradaPound { get; set; }
        public decimal EntradaCostPound { get; set; }
        public decimal EntradaTotalCostPound { get; set; }
        public decimal SalidaPound { get; set; }
        public decimal SalidaCostPound { get; set; }
        public decimal SalidaTotalCostPound { get; set; }
        public decimal FinalPound { get; set; }
        public decimal FinalCostPound { get; set; }
        public decimal FinalTotalCostPound { get; set; }
        public string ItemPresentationDescrip { get; set; }
        public decimal OneItemPound { get; set; }

    }
}
