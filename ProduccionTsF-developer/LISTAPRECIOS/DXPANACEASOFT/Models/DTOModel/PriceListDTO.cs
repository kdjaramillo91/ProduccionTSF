using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DXPANACEASOFT.Models.DTOModel
{
    public class PriceListDTO
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string name { get; set; }
        public int id_state { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string state { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public DateTime startDate { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public DateTime endDate { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int? id_periodoCalendario { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string code_processtype { get; set; }
        public int? id_processtype { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int? id_comprador { get; set; }

        public bool isQuotation { get; set; }
        public int? id_priceListBase { get; set; }
        public string basePriceList { get; set; }
        
        public bool? paraProveedor { get; set; }
        public int? id_proveedor { get; set; }
        public bool? paraGrupo { get; set; }
        public int? id_grupo { get; set; }

        public int? id_certification { get; set; }
        public string certification { get; set; }

        public List<PriceListDetailDTO> ListPriceListDetailDTO { get; set; }
        public List<PriceListPenaltyDTO> ListPriceListPenaltyDTO { get; set; }
        //public bool listPriceListDetailsUpdated { get; set; }
        //public bool listPriceListPenalityUpdated { get; set; }

        public bool isUsed { get; set; }
        public bool canClose { get; set; }
        public bool canAnnul { get; set; }
        public bool canRefreshReplicate { get; set; }
        public string state_code { get; set; }
        public string userAproval { get; set; }
    }

    public class PriceListDetailDTO
    {
        public int id { get; set; }
        public int order { get; set; }
        public int id_priceList { get; set; }
        public int? idItemSize { get; set; }
        public string name { get; set; }
        public string codeType { get; set; }
        public string type { get; set; }
        public decimal price { get; set; }
        public decimal commission { get; set; }
        public decimal pricePurchase { get; set; }
        public decimal basePrice { get; set; }
        public decimal distint { get; set; }
        public int id_Class { get; set; }
        public string codeClass { get; set; }
        public string nameClass { get; set; }
        public PriceListClassShrimpDTO D0 { get; set; }
        public PriceListClassShrimpDTO D1 { get; set; }
        public PriceListClassShrimpDTO D2 { get; set; }
        public PriceListClassShrimpDTO D3 { get; set; }
        public PriceListClassShrimpDTO D4 { get; set; }
        public PriceListClassShrimpDTO D5 { get; set; }
        public PriceListClassShrimpDTO D6 { get; set; }
        public PriceListClassShrimpDTO D7 { get; set; }
        public PriceListClassShrimpDTO D8 { get; set; }
    }
    public class PriceListClassShrimpDTO
    {
        public int id_classShrimp { get; set; }
        public string classShrimp { get; set; }
        public decimal price { get; set; }
        public decimal difference { get; set; }
        public decimal commission { get; set; }
        public decimal price_RF { get; set; }
        public decimal price_PC { get; set; }
        public decimal difference_F_RF { get; set; }
    }
    public class PriceListPenaltyDTO
    {
        public int id_classShrimp { get; set; }
        public string classShrimp { get; set; }
        public int order { get; set; }
        public decimal value { get; set; }
    }

    public class PriceListConsultDTO
    {
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public int? id_estado { get; set; }
        public int? id_tipoLista { get; set; }
        public int? id_proveedor { get; set; }
        public int? id_grupo { get; set; }
        public int? id_tipoListaCamaron { get; set; }
        public int? id_responsable { get; set; }
        public int? id_certification { get; set; }
    }
}