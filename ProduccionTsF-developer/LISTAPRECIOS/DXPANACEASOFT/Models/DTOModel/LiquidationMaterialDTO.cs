using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DXPANACEASOFT.Models.DTOModel
{
    public class LiquidationMaterialDTO
    {
        public LiquidationMaterialDTO()
        {
            SummaryLiquidationMaterialDetailDTO = new List<SummaryLiquidationMaterialDetailDTO>();
            LiquidationMaterialDetailDTO = new List<LiquidationMaterialDetailDTO>();
        }

        public int id { get; set; }
        public string numberDocument { get; set; }
        public DateTime emissionDateDocument { get; set; }
        public int id_documentType { get; set; }
        public string documentType { get; set; }
        public int id_documentState { get; set; }
        public string documentState { get; set; }
        public string documentDescription { get; set; }
        public int id_provider { get; set; }
        public string provider { get; set; }
        public decimal subTotal { get; set; }
        public decimal iva { get; set; }
        public decimal total { get; set; }

        public List<SummaryLiquidationMaterialDetailDTO> SummaryLiquidationMaterialDetailDTO { get; set; }
        public List<LiquidationMaterialDetailDTO> LiquidationMaterialDetailDTO { get; set; }
    }

    public class SummaryLiquidationMaterialDetailDTO
    {
        public int id_item { get; set; }
        public string codigo { get; set; }
        public string name { get; set; }
        public int id_metricUnit { get; set; }
        public string metricUnit { get; set; }
        public decimal quantity { get; set; }
        public decimal unitCost { get; set; }
        public decimal subTotal { get; set; }
        public decimal subTotalIva { get; set; }
        public decimal total { get; set; }
    }

    public class LiquidationMaterialDetailDTO
    {
        public int id { get; set; }
        public int id_guia { get; set; }
        public string numberGuia { get; set; }
        public DateTime emisionGuia { get; set; }
        public int id_guiaDetail { get; set; }
        public int id_item { get; set; }
        public string codigo { get; set; }
        public string name { get; set; }
        public int id_metricUnit { get; set; }
        public string metricUnit { get; set; }
        public decimal quantityOrigin { get; set; }
        public decimal quantity { get; set; }
        public decimal unitCostOrigin { get; set; }
        public decimal unitCost { get; set; }
        public decimal subTotalOrigin { get; set; }
        public decimal subTotal { get; set; }
        public decimal iva { get; set; }
        public decimal subTotalIvaOrigin { get; set; }
        public decimal subTotalIva { get; set; }
        public decimal totalOrigin { get; set; }
        public decimal total { get; set; }
        public bool aprovedLogist { get; set; }
        public bool aprovedComertial { get; set; }
        public string descriptionLogist { get; set; }
        public string descriptionComertial { get; set; }
    }

    public class LiquidationMaterialConsultDTO
    {
        public int? id_estado { get; set; }
        public string numeroLiquidacion { get; set; }
        public string fechaInicioEmision { get; set; }
        public string fechaFinEmision { get; set; }
        public int id_proveedor { get; set; }
        public int id_producto { get; set; }
        public string fechaInicioGuia { get; set; }
        public string fechaFinGuia { get; set; }
        public string numeroGuia { get; set; }
    }

    public class LiquidationMaterialResultConsultDTO
    {
        public int id { get; set; }
        public string numberDocument { get; set; }
        public DateTime emissionDateDocument { get; set; }
        public int id_provider { get; set; }
        public string provider { get; set; }
        public int id_documentState { get; set; }
        public string documentState { get; set; }
        public bool canEdit { get; set; }
        public bool canAproved { get; set; }
        public bool canAuthorize { get; set; }
        public bool canAnnul { get; set; }
        public bool canReverse { get; set; }

    }

    public class ReceptionMaterialDTO
    {
        public int id_receptionMaterial { get; set; }
        public string numeroGuia { get; set; }
        public DateTime fechaEmisionGuia { get; set; }
        public int id_estado { get; set; }
        public string estado { get; set; }
        public int id_proveedor { get; set; }
        public string proveedor { get; set; }
        public string camaronera { get; set; }
        public string numeroReceptionMaterial { get; set; }
        public string fechaReceptionMaterial { get; set; }
    }
}