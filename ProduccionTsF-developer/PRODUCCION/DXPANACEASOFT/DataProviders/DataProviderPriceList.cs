using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
//using System.Web.Mvc;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderPriceList
    {
        private static DBContext db = null;

        public static IEnumerable PriceLists()
        {
            db = new DBContext();

            return db.PriceList.ToList();
        }


        public static IEnumerable PriceListsForPurchaseByCompany(int? id_company)
        {
            db = new DBContext();

            var priceListAux = db.PriceList.Where(t => t.id_company == id_company && !t.Document.DocumentState.code.Equals("01") && t.isForPurchase &&
                                             !t.isQuotation && t.isForPurchase).ToList();//03 Codigo de APROBADA

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                           s.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
                                                                            (s.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + s.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "")
            }).OrderByDescending(t => t.id).ToList();
        }

        public static IEnumerable AllPriceListsForPurchaseByCompany(int? id_company)
        {
            db = new DBContext();

            return db.PriceList.Where(s => s.id_company == id_company && s.isForPurchase).Select(s => new { id = s.id, name = s.name }).ToList();

        }

        public static PriceList PriceListById(int? id_priceList)
        {
            db = new DBContext();
            return db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
        }

        public static IEnumerable PriceListsForSoldOpen(int? id_company)
        {

            db = new DBContext();

            return db.PriceList.Where(r => r.isForSold == true && r.isQuotation == false && r.id_customer == null && r.id_company == id_company).ToList();

        }

        public static IEnumerable PriceListBasesByCompanyDocumentTypeWithCurrent(int? id_company, int? id_documentTypeCurrent, int? id_priceListBaseCurrent)
        {
            db = new DBContext();

            var codeCurrentAux = db.DocumentType.FirstOrDefault(fod => fod.id == id_documentTypeCurrent)?.code ?? "";
            var codeAux = codeCurrentAux == "19" ? "18" : (codeCurrentAux == "21" ? "20" : "");
            var priceListAux = db.PriceList.Where(t => (t.Document.DocumentType.code.Equals(codeAux)) &&
                                         t.id_company == id_company && t.Document.DocumentState.code.Equals("03")).ToList();//Code "03" es estado APROBADA

            var nowAux = DateTime.Now;
            priceListAux = priceListAux.AsEnumerable().Where(w => DateTime.Compare(w.CalendarPriceList.startDate.Date, nowAux.Date) <= 0 && DateTime.Compare(nowAux.Date, w.CalendarPriceList.endDate.Date) <= 0).ToList();

            var priceListBaseCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListBaseCurrent);
            if (priceListBaseCurrentAux != null && !priceListAux.Contains(priceListBaseCurrentAux)) priceListAux.Add(priceListBaseCurrentAux);

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                           s.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]"
            }).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable PriceListByCompanyWithCurrent(int? id_company, int? id_priceListCurrent)
        {
            db = new DBContext();

            var priceListAux = db.PriceList.Where(t => t.id_company == id_company).ToList();

            var nowAux = DateTime.Now;
            priceListAux = priceListAux.AsEnumerable().Where(w => DateTime.Compare(w.CalendarPriceList.startDate.Date, nowAux.Date) <= 0 && DateTime.Compare(nowAux.Date, w.CalendarPriceList.endDate.Date) <= 0).ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListCurrent);
            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux)) priceListAux.Add(priceListCurrentAux);

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                           s.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]"
            }).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable PurchaseApprovedPriceLists(int? id_company, int? id_priceListCurrent, int? id_provider)
        {
            db = new DBContext();

            var priceListAux = db.PriceList.Where(t => t.id_company == id_company && t.Document.DocumentState.code.Equals("03") && t.isForPurchase &&
                                                         (!t.isQuotation || (t.isQuotation && (((t.byGroup == null || t.byGroup == false) && t.id_provider == id_provider) || (t.byGroup == true && t.GroupPersonByRol.GroupPersonByRolDetail.FirstOrDefault(fod => fod.id_person == id_provider) != null))))).ToList();//03 Codigo de APROBADA

            var nowAux = DateTime.Now;
            priceListAux = priceListAux.AsEnumerable().Where(w => DateTime.Compare(w.CalendarPriceList.startDate.Date, nowAux.Date) <= 0 && DateTime.Compare(nowAux.Date, w.CalendarPriceList.endDate.Date) <= 0).ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListCurrent);
            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux)) priceListAux.Add(priceListCurrentAux);

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                           s.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
                                                                            (s.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + s.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "")
            }).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable SaleApprovedPriceLists(int? id_company, int? id_priceListCurrent, int? id_customer)
        {
            db = new DBContext();

            var priceListAux = db.PriceList.Where(t => t.id_company == id_company && t.Document.DocumentState.code.Equals("03") && t.isForSold &&
                                                         (!t.isQuotation || (t.isQuotation && (((t.byGroup == null || t.byGroup == false) && t.id_customer == id_customer) || (t.byGroup == true && t.GroupPersonByRol.GroupPersonByRolDetail.FirstOrDefault(fod => fod.id_person == id_customer) != null))))).ToList();//03 Codigo de APROBADA

            var nowAux = DateTime.Now;
            priceListAux = priceListAux.AsEnumerable().Where(w => DateTime.Compare(w.CalendarPriceList.startDate.Date, nowAux.Date) <= 0 && DateTime.Compare(nowAux.Date, w.CalendarPriceList.endDate.Date) <= 0).ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListCurrent);
            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux)) priceListAux.Add(priceListCurrentAux);

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                           s.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]"
            }).OrderBy(t => t.id).ToList();
        }


        public static IEnumerable FiltersShow()
        {

            var model = new List<FilterShowPriceList>();

            model.Add(new FilterShowPriceList
            {
                id = 1,
                code = "Item",
                name = "Ítem"
            });
            model.Add(new FilterShowPriceList
            {
                id = 2,
                code = "TipoCategoriaItem",
                name = "Tipo Categoría de Ítem"
            });
            model.Add(new FilterShowPriceList
            {
                id = 3,
                code = "Talla",
                name = "Talla"
            });
            return model;
        }

        public static string GetColumnVisibles(int? id_priceList)
        {
            PriceList priceList = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
            List<bool> result = new List<bool>();

            if (priceList != null)
            {
                List<int> filterShow = priceList.list_filterShow == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_filterShow);
                var codeDocumentType = priceList.Document?.DocumentType?.code ?? "";
                result.Add((filterShow != null && filterShow.Count() > 0) ? filterShow.Contains(1) || filterShow.Contains(0) : true);
                result.Add((filterShow != null && filterShow.Count() > 0) ? filterShow.Contains(2) || filterShow.Contains(0) : true);
                result.Add((filterShow != null && filterShow.Count() > 0) ? filterShow.Contains(3) || filterShow.Contains(0) : true);
                result.Add((codeDocumentType == "18" || codeDocumentType == "19"));
                result.Add((codeDocumentType == "20" || codeDocumentType == "21"));
            }
            else
            {
                result.Add(true);
                result.Add(true);
                result.Add(true);
                result.Add(true);
                result.Add(true);
            }

            return JsonConvert.SerializeObject(result);
        }

        public static IEnumerable PriceListByCompanyWithCurrentAndProductionLot(int? id_company, int? id_priceListCurrent, int id_productionLot)
        {
            db = new DBContext();

            var productionLotAux = db.ProductionLot.FirstOrDefault(t => t.id == id_productionLot);

            var priceListAux = db.PriceList.Where(t => (t.id_processtype == null || t.id_processtype == productionLotAux.id_processtype) && t.id_company == id_company && t.Document.DocumentState.code.Equals("03") && t.isForPurchase &&
                                                        t.Document.authorizationDate.HasValue
                                                        && ((t.isQuotation && (((t.byGroup == null || t.byGroup == false) && t.id_provider == productionLotAux.id_provider) || (t.byGroup == true && t.GroupPersonByRol.GroupPersonByRolDetail.FirstOrDefault(fod => fod.id_person == productionLotAux.id_provider) != null))))).ToList();//03 Codigo de APROBADA

            //var priceListAux = db.PriceList.Where(t => (t.id_processtype == null || t.id_processtype == productionLotAux.id_processtype) && t.id_company == id_company && t.Document.DocumentState.code.Equals("03") && t.isForPurchase &&
            //                                            (/*!t.isQuotation ||*/ (t.isQuotation && (((t.byGroup == null || t.byGroup == false) && t.id_provider == productionLotAux.id_provider) || (t.byGroup == true && t.GroupPersonByRol.GroupPersonByRolDetail.FirstOrDefault(fod => fod.id_person == productionLotAux.id_provider) != null))))).ToList();//03 Codigo de APROBADA


            var nowAux = DateTime.Now;
            priceListAux = priceListAux
                            .AsEnumerable()
                            .Where(w => DateTime.Compare(w.startDate.Date, nowAux.Date) <= 0
                            && DateTime.Compare(nowAux.Date, w.endDate.Date) <= 0).ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListCurrent);
            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux)) priceListAux.Add(priceListCurrentAux);

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                           s.endDate.ToString("dd/MM/yyyy") + "]" +
                                                                           (s.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + s.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "")
            }).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable PriceListByCompanyWithCurrentAndProvider(int? id_company, int? id_priceListCurrent, int id_provider)
        {
            db = new DBContext();

            //var productionLotAux = db.ProductionLot.FirstOrDefault(t => t.id == id_productionLot);

            var priceListAux = db.PriceList.Where(t => t.id_company == id_company && t.Document.DocumentState.code.Equals("03") && t.isForPurchase &&
                                                        t.Document.authorizationDate.HasValue && (/*!t.isQuotation ||*/ (t.isQuotation && (((t.byGroup == null || t.byGroup == false) && t.id_provider == id_provider) || (t.byGroup == true && t.GroupPersonByRol.GroupPersonByRolDetail.FirstOrDefault(fod => fod.id_person == id_provider) != null))))).ToList();//03 Codigo de APROBADA

            //var priceListAux = db.PriceList.Where(t => (t.id_processtype == null || t.id_processtype == productionLotAux.id_processtype) && t.id_company == id_company && t.Document.DocumentState.code.Equals("03") && t.isForPurchase &&
            //                                            (/*!t.isQuotation ||*/ (t.isQuotation && (((t.byGroup == null || t.byGroup == false) && t.id_provider == productionLotAux.id_provider) || (t.byGroup == true && t.GroupPersonByRol.GroupPersonByRolDetail.FirstOrDefault(fod => fod.id_person == productionLotAux.id_provider) != null))))).ToList();//03 Codigo de APROBADA


            var nowAux = DateTime.Now;
            priceListAux = priceListAux.AsEnumerable().Where(w => DateTime.Compare(w.CalendarPriceList.startDate.Date, nowAux.Date) <= 0 && DateTime.Compare(nowAux.Date, w.CalendarPriceList.endDate.Date) <= 0).ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListCurrent);
            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux)) priceListAux.Add(priceListCurrentAux);

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                           s.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]" +
                                                                           (s.Document.authorizationDate.HasValue ? "  AUTORIZACIÓN [" + s.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "")
            }).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable PriceListByCompanyWithCurrentAndProviderAndCommercialDate(int? id_company, int? id_priceListCurrent, int? id_provider, DateTime? dt)
        {
            db = new DBContext();

            List<PriceList> priceListAux = new List<PriceList>();
           string settingEBLPOC = DataProviderSetting.ValueSetting("EBLPOC");
            int intSettingEBLPOC = 0;
            if (!string.IsNullOrEmpty(settingEBLPOC) && (int.TryParse(settingEBLPOC, out intSettingEBLPOC) == true))
            {
                if (intSettingEBLPOC < 11) intSettingEBLPOC = 11;
            }
                string settingTLOC = DataProviderSetting.ValueSetting("TLOC");
            if (!string.IsNullOrEmpty(settingTLOC) && settingTLOC.Equals("REF"))
            {
                priceListAux = db.PriceList.AsEnumerable()
                                    .Where(t => t.id_company == id_company
                                                && int.Parse(t.Document.DocumentState.code) >= intSettingEBLPOC
                                                && t.isForPurchase
                                                && t.id_priceListBase == null
                                                ).ToList();
            }
            else {
                priceListAux = db.PriceList
                                    .Where(t => t.id_company == id_company
                                                && (
                                                //t.Document.DocumentState.code.Equals("10") 
                                                t.Document.DocumentState.code.Equals("11")
                                                //|| t.Document.DocumentState.code.Equals("12")
                                                || t.Document.DocumentState.code.Equals("13")
                                                //|| t.Document.DocumentState.code.Equals("14")
                                                || t.Document.DocumentState.code.Equals("15")
                                                )
                                                && t.isForPurchase
                                                && t.PriceListPersonPersonRol.FirstOrDefault(fod => fod.id_Person == id_provider) != null

                                                //&& ((t.isQuotation && (((t.byGroup == null || t.byGroup == false) 
                                                //&& t.id_provider == id_provider))))
                                                ).ToList();

            }
            


            string varParSys1 = DataProviderSetting.ValueSetting("FLPEC");
            if (!string.IsNullOrEmpty(varParSys1) && varParSys1.Equals("0"))
            {
                priceListAux = priceListAux.Where(w => w.Document.DocumentState.code.Equals("03")
                || w.Document.DocumentState.code.Equals("02")).ToList();
            }


            var nowAux = DateTime.Now;
            if (dt != null)
            {
                nowAux = dt.Value;
            }


            //ojo con esto
            priceListAux = priceListAux.AsEnumerable()
                                        .Where(w => DateTime.Compare(w.startDate.Date, nowAux.Date) <= 0
                                                && DateTime.Compare(nowAux.Date, w.endDate.Date) <= 0).ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListCurrent);

            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux)) priceListAux.Add(priceListCurrentAux);


            string valParSys = DataProviderSetting.ValueSetting("SSLVTP");

            if (!string.IsNullOrEmpty(valParSys) && valParSys.Equals("1"))
            {

                var plEnt = priceListAux
                            .Where(w => w.ProcessType?.code == "ENT")
                            .Select(s => s)
                            .OrderBy(o => o.commercialDate).FirstOrDefault();


                var plCol = priceListAux
                            .Where(w => w.ProcessType?.code == "COL")
                            .Select(s => s)
                            .OrderBy(o => o.commercialDate).FirstOrDefault();

                priceListAux.Clear();
                if (plEnt != null) { priceListAux.Add(plEnt); }
                if (plCol != null) { priceListAux.Add(plCol); }
            }

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " Estado: " + s.Document.DocumentState.name + " (" + s.Document.DocumentType.name + ") "
                + s.CalendarPriceList.CalendarPriceListType.name
                + " [" + s.startDate.ToString("dd/MM/yyyy") + " - "
                + s.endDate.ToString("dd/MM/yyyy") + "]" + " PROCESO: "
                + s.ProcessType.name + (s.commercialDate.HasValue ? " JEFE COMERCIAL ["
                + s.commercialDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "")
            }).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable PriceListByCompanyWithCurrentAndProviderAndCommercialDate2(int? id_company, int? id_priceListCurrent, int? id_provider, DateTime? dt, int? idPt)
        {
            db = new DBContext();

            List<PriceList> priceListAux = new List<PriceList>();
            string settingEBLPA = DataProviderSetting.ValueSetting("EBLPA");
            int intSettingEBLPA = 0;
            if (!string.IsNullOrEmpty(settingEBLPA) && (int.TryParse(settingEBLPA, out intSettingEBLPA) == true))
            {
                if (intSettingEBLPA < 4) intSettingEBLPA = 4;
            }
            string settingTLA = DataProviderSetting.ValueSetting("TLA");
            if (!string.IsNullOrEmpty(settingTLA) && settingTLA.Equals("REF"))
            {
                priceListAux = db.PriceList.AsEnumerable()
                                    .Where(t => t.id_company == id_company
                                                && int.Parse(t.Document.DocumentState.code) >= intSettingEBLPA
                                                && t.isForPurchase
                                                && t.id_priceListBase == null
                                                ).ToList();
            }
            else
            {
                priceListAux = db.PriceList
                                    .Where(t => t.id_company == id_company
                                                && t.id_processtype == idPt
                                                && (
                                                t.Document.DocumentState.code.Equals("13")
                                                || t.Document.DocumentState.code.Equals("04")
                                                || t.Document.DocumentState.code.Equals("14")
                                                || t.Document.DocumentState.code.Equals("15")
                                                )
                                                && t.isForPurchase
                                                && t.PriceListPersonPersonRol.FirstOrDefault(fod => fod.id_Person == id_provider) != null
                                                ).ToList();

            }


            string varParSys1 = DataProviderSetting.ValueSetting("FLPEC");
            if (!string.IsNullOrEmpty(varParSys1) && varParSys1.Equals("0"))
            {
                priceListAux = priceListAux.Where(w => w.Document.DocumentState.code.Equals("03")
                || w.Document
                    .DocumentState
                    .code
                    .Equals("02"))
                    .ToList();
            }


            var nowAux = DateTime.Now;
            if (dt != null)
            {
                nowAux = dt.Value;
            }


            //ojo con esto
            priceListAux = priceListAux.AsEnumerable()
                                        .Where(w => DateTime.Compare(w.startDate.Date, nowAux.Date) <= 0
                                                && DateTime.Compare(nowAux.Date, w.endDate.Date) <= 0)
                                                .ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListCurrent && fod.id_processtype == idPt);


            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux)) priceListAux.Add(priceListCurrentAux);


            string valParSys = DataProviderSetting.ValueSetting("SSLVTP");

            if (!string.IsNullOrEmpty(valParSys) && valParSys.Equals("1"))
            {

                var plEnt = priceListAux
                            .Where(w => w.ProcessType?.code == "ENT")
                            .Select(s => s)
                            .OrderBy(o => o.commercialDate).FirstOrDefault();


                var plCol = priceListAux
                            .Where(w => w.ProcessType?.code == "COL")
                            .Select(s => s)
                            .OrderBy(o => o.commercialDate).FirstOrDefault();

                priceListAux.Clear();
                if (plEnt != null) { priceListAux.Add(plEnt); }
                if (plCol != null) { priceListAux.Add(plCol); }
            }

            return priceListAux.Select(s => new {
                s.id,
                name = s.name + " Estado: " + s.Document.DocumentState.name + " (" + s.Document.DocumentType.name + ") "
                + s.CalendarPriceList.CalendarPriceListType.name
                + " [" + s.startDate.ToString("dd/MM/yyyy") + " - "
                + s.endDate.ToString("dd/MM/yyyy") + "]" + " PROCESO: "
                + s.ProcessType.name + (s.commercialDate.HasValue ? " JEFE COMERCIAL ["
                + s.commercialDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "")
            }).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable PriceListByCompanyWithCurrentAndProviderForLiquidation(int? id_company, int? id_priceListCurrent, int? id_provider, DateTime? dt)
        {
            db = new DBContext();

            string[] documentStatesCodes = new string[] { "15","04","11", "12","13", "14" };

            int[] priceListByPersonRolIds = db.PriceListPersonPersonRol
                                                        .Where(fod => fod.id_Person == id_provider)? //?.id_PriceList?
                                                        .Select(r => r.id_PriceList)?
                                                        .ToArray();
            int[] documentStateValidIds = db.DocumentState
                                                .Where(r => documentStatesCodes.Contains(r.code))
                                                .Select(r => r.id)
                                                .ToArray();

            var priceListIds = db.PriceList
                                      .Where(t => t.id_company == id_company
                                                 && t.isForPurchase )
                                      .Select(r => r.id).ToArray();

            int[] priceListFilteredIds = (from byRol in priceListByPersonRolIds
                                          join priceList in priceListIds
                                          on byRol equals priceList
                                          select byRol)
                                          .ToArray();

            var documentFilteredIds = db.Document
                                                .Where(r => priceListFilteredIds.Contains(r.id) && documentStateValidIds.Contains(r.id_documentState))
                                                .Select(r => r.id)
                                                .ToArray();

            var priceListAux = db.PriceList
                                    .Include("Document")
                                    .Include("Document.DocumentState")
                                    .Include("ProcessType")
                                    .Include("CalendarPriceList")
                                    .Include("CalendarPriceList.CalendarPriceListType")
                                    .Where(t => documentFilteredIds.Contains(t.id))
                                    .ToArray();

            //var priceListAux = db.PriceList
            //                        .Where(t => t.id_company == id_company
            //                                    && (
            //                                    (t.Document.DocumentState.code.Equals("15") || t.Document.DocumentState.code.Equals("04")
            //                                    || t.Document.DocumentState.code.Equals("11") || t.Document.DocumentState.code.Equals("12")
            //                                    || t.Document.DocumentState.code.Equals("13") || t.Document.DocumentState.code.Equals("14")))
            //                                    && t.isForPurchase
            //                                    && t.PriceListPersonPersonRol.FirstOrDefault(fod => fod.id_Person == id_provider) != null
            //                                    ).ToList();

            string varParSys1 = DataProviderSetting.ValueSetting("FLPEC");
            if (!string.IsNullOrEmpty(varParSys1) && varParSys1.Equals("0"))
            {
                priceListAux = priceListAux
                                    .Where( w => w.Document.DocumentState.code.Equals("03")
                                            || w.Document.DocumentState.code.Equals("02"))
                                    .ToArray();
            }

            var nowAux = DateTime.Now;
            if (dt != null)
            {
                nowAux = dt.Value;
            }

            //ojo con esto
            priceListAux = priceListAux
                                    .Where(w => DateTime.Compare(w.startDate.Date, nowAux.Date) <= 0
                                            && DateTime.Compare(nowAux.Date, w.endDate.Date) <= 0)
                                    .ToArray();

            //priceListAux = priceListAux.AsEnumerable()
            //                            .Where(w => DateTime.Compare(w.startDate.Date, nowAux.Date) <= 0
            //                                    && DateTime.Compare(nowAux.Date, w.endDate.Date) <= 0).ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListCurrent);

            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux))
            {
                var prePriceList = priceListAux.ToList();
                prePriceList.Add(priceListCurrentAux);
                priceListAux = prePriceList.ToArray();

                //   priceListAux.Add(priceListCurrentAux);
            }
             
            string valParSys = DataProviderSetting.ValueSetting("SSLVTP");

            if (!string.IsNullOrEmpty(valParSys) && valParSys.Equals("1"))
            {

                var plEnt = priceListAux
                            .Where(w => w.ProcessType?.code == "ENT")
                            .Select(s => s)
                            .OrderBy(o => o.commercialDate).FirstOrDefault();


                var plCol = priceListAux
                            .Where(w => w.ProcessType?.code == "COL")
                            .Select(s => s)
                            .OrderBy(o => o.commercialDate).FirstOrDefault();

                //priceListAux.Clear();
                priceListAux = Array.Empty<PriceList>();

                if (plEnt != null) 
                {
                    var prePriceList = priceListAux.ToList();
                    prePriceList.Add(plEnt);
                    priceListAux = prePriceList.ToArray();
                
                }
                if (plCol != null) 
                {
                    var prePriceList = priceListAux.ToList();
                    prePriceList.Add(plCol);
                    priceListAux = prePriceList.ToArray();
                    //priceListAux.Add(plCol); 
                }
            }

            return priceListAux.Select(s => new 
            {
                s.id,
                name = s.name + " Estado: " + s.Document.DocumentState.name + " (" + s.Document.DocumentType.name + ") "
                + s.CalendarPriceList.CalendarPriceListType.name
                + " [" + s.startDate.ToString("dd/MM/yyyy") + " - "
                + s.endDate.ToString("dd/MM/yyyy") + "]" + " PROCESO: "
                + s.ProcessType.name + (s.commercialDate.HasValue ? " JEFE COMERCIAL ["
                + s.commercialDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "")
            })
            .OrderBy(t => t.id)
            .ToList();
        }

        public static IEnumerable PriceListByCompanyWithCurrentAndProviderForLiquidationCopacking(int? id_company, int? id_provider, DateTime? dt)
        {
            db = new DBContext();

            var priceListAux = db.CopackingTariff
                                    .Where(t => t.id_company == id_company && (t.id_provider == id_provider)
                                           && (dt >= t.dateInit && dt <= t.dateEnd)).ToList();

            return priceListAux.Select(s => new {
                s.id,
                name = s.name
                + " [" + s.dateInit.ToString("dd/MM/yyyy") + " - "
                + s.dateEnd.ToString("dd/MM/yyyy") + "]"
            }).OrderBy(t => t.id).ToList();
        }
    }
}