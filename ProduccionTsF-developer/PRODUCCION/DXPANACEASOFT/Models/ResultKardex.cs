using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class ResultKardexExcel2
    {
        //public int id { get; set; }//Este es el id del detalle del Inventario
        [Description("Fecha de Proceso")]
        public string emissionDate { get; set; }
        [Description("Proceso")]
        public string name_productionProcess { get; set; }
        [Description("Sec. Transaccional")]
        public string number { get; set; }
        [Description("Lote")]
        public string internalNumber { get; set; }
        [Description("Proveedor")]
        public string Provider_name { get; set; }

        [Description("Motivo de Inventario")]
        public string inventoryReason { get; set; }
        [Description("Bodega")]
        public string warehouse { get; set; }
        [Description("Ubicación")]
        public string warehouseLocation { get; set; }

        [Description("Ref")]
        public string natureSequential { get; set; }

        //[Description("Cód Producto")]
        //public string code_item { get; set; }
        [Description("Cód Producto")]
        public string codigo_producto { get; set; }
        [Description("Desc Producto")]
        public string descripcion_producto { get; set; }
        [Description("Talla")]
        public string itemSize { get; set; }
        [Description("Und Prest")]
        public string ItemMetricUnit { get; set; }
        [Description("Ingreso")]
        public decimal? entry { get; set; }
        [Description("Egreso")]
        public decimal? exit { get; set; }
        [Description("LB")]
        public decimal? LB { get; set; }
        [Description("KG")]
        public decimal? KG { get; set; }
        [Description("Usuario")]
        public string usuario { get; set; }
		public string costo_promedio { get; set; }

        [Description("Estado")]
        public string nameDocumentState { get; set; }
        

    }

    public class ResultKardex
	{
		public int id { get; set; }//Este es el id del detalle del Inventario

		public int id_document { get; set; }
		public string document { get; set; }

		public int? id_documentType { get; set; }
		public string documentType { get; set; }

		public string nameDocumentState { get; set; }

		public int? id_inventoryReason { get; set; }
		public string inventoryReason { get; set; }

        public DateTime? emissionDate { get; set; }
		public DateTime? dateCreate { get; set; }

		public int? id_item { get; set; }
		public string code_item { get; set; }

		public int? id_metricUnit { get; set; }
		public string metricUnit { get; set; }

		public int? id_lot { get; set; }
		public string number { get; set; }
		public string internalNumber { get; set; }
        public string lotMarked { get; set; }

        public int? id_warehouse { get; set; }
		public string warehouse { get; set; }

		public int? id_warehouseLocation { get; set; }
		public string warehouseLocation { get; set; }

		public int? id_warehouseExit { get; set; }
		public string warehouseExit { get; set; }

		public int? id_warehouseLocationExit { get; set; }
		public string warehouseLocationExit { get; set; }

		public int? id_warehouseEntry { get; set; }
		public string warehouseEntry { get; set; }

		public int? id_warehouseLocationEntry { get; set; }
		public string warehouseLocationEntry { get; set; }

		public decimal? priceCost { get; set; }

		public decimal? previousBalance { get; set; }
		public decimal? previousBalanceCost { get; set; }

		public decimal? entry { get; set; }
		public decimal? entryCost { get; set; }

		public decimal? exit { get; set; }
		public decimal? exitCost { get; set; }

		public decimal? balance { get; set; }
		public decimal? balanceCost { get; set; }

		public decimal? balanceCutting { get; set; }
		public decimal? balanceCuttingCost { get; set; }

		public string numberRemissionGuide { get; set; }

		public int? idCompany { get; set; }

		public string nameCompany { get; set; }
		public string nameDivision { get; set; }
		public string nameBranchOffice { get; set; }

		public string Provider_name { get; set; }
		public bool isCopacking { get; set; }
		public string nameProviderShrimp { get; set; }
		public string productionUnitProviderPool { get; set; }
		public string itemSize { get; set; }
		public string itemType { get; set; }
		public string ItemMetricUnit { get; set; }
		public decimal? ItemPresentationValue { get; set; }
		public string name_productionProcess { get; set; }
		public string natureSequential { get; set; }
		public decimal? LB { get; set; }
		public decimal? KG { get; set; }
        public string usuario { get; set; }
		public decimal? ValorSaldo { get; set; }	
		public string codigo_producto { get; set; }
		public string descripcion_producto { get; set; }
		public decimal? presentacionMinima { get; set; }	
        public string costo_promedio { get; set; }
    }
}