using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderBusinessOportunity
    {
        private static DBContext db = null;

        public static List<BusinessOportunity> BusinessOportunityInList(string listInt)
        {
            List<int> listIntAux = JsonConvert.DeserializeObject<List<int>>(listInt);
            db = new DBContext();
            var model = db.BusinessOportunity.Where(t => listIntAux.Contains(t.id)).ToList();

            return model;
        }

        public static BusinessOportunity BusinessOportunityById(int? id)
        {
            db = new DBContext(); ;
            return db.BusinessOportunity.FirstOrDefault(v => v.id == id);
        }

        public static BusinessOportunity BusinessOportunityById_businessOportunityPlanningDetail(int? id_businessOportunityPlanningDetail)
        {
            db = new DBContext(); ;
            var businessOportunityPlanningDetailAux = db.BusinessOportunityPlanningDetail.FirstOrDefault(v => v.id == id_businessOportunityPlanningDetail);
            return businessOportunityPlanningDetailAux?.BusinessOportunityPlaninng?.BusinessOportunity;
        }

        public static IEnumerable BusinessOportunityPlanningDetailPurchase(int? id_company)
        {
            db = new DBContext(); ;
            var businessOportunityPlanningDetailAux = db.BusinessOportunityPlanningDetail.Where(v => v.BusinessOportunityPlaninng.BusinessOportunity.id_company == id_company && v.BusinessOportunityPlaninng.BusinessOportunity.Document.DocumentType.code.Equals("16"));//Oportunidad de Compra
            return businessOportunityPlanningDetailAux.Select(s => new { id = s.id, name = s.BusinessOportunityPlaninng.BusinessOportunity.name}).ToList();
        }

        public static IEnumerable BusinessOportunityPurchase(int? id_company)
        {
            db = new DBContext(); ;
            var businessOportunityAux = db.BusinessOportunity.Where(v => v.id_company == id_company && v.Document.DocumentType.code.Equals("16"));//Oportunidad de Compra
            return businessOportunityAux.Select(s => new { id = s.id, name = s.name }).ToList();
        }

        public static IEnumerable AllBusinessOportunityActivitiesByCompany(int? id_company)
        {
            db = new DBContext();

            return db.BusinessOportunityActivity.Where(s => (s.id_company == id_company)).ToList();

        }

        public static BusinessOportunityActivity BusinessOportunityActivity(int? id_businessOportunityActivity)
        {
            db = new DBContext(); ;
            return db.BusinessOportunityActivity.FirstOrDefault(v => v.id == id_businessOportunityActivity);
        }

        public static IEnumerable AllBusinessOportunityActivityStatesByCompany(int? id_company)
        {
            db = new DBContext();

            return db.BusinessOportunityActivityState.Where(s => (s.id_company == id_company)).ToList();

        }

        public static BusinessOportunityActivityState BusinessOportunityActivityState(int? id_state)
        {
            db = new DBContext();
            return db.BusinessOportunityActivityState.FirstOrDefault(v => v.id == id_state);
        }

        //public static BusinessOportunity BusinessOportunityPhase(int? id_businessOportunityPhase)
        //{
        //    db = new DBContext();

        //    //ViewData["codeBusinessOportunityDocumentType"] = businessOportunity?.Document?.DocumentType?.code ?? "";
        //    var businessOportunityPlanningDetailAux = db.BusinessOportunityPlanningDetail.FirstOrDefault(v => v.id == id_businessOportunityPlanningDetail);
        //    return businessOportunityPlanningDetailAux?.BusinessOportunityPlaninng?.BusinessOportunity;
        //}
    }
}