using Dapper;
using DevExpress.Data.ODataLinq.Helpers;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPivotGrid;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.GenericProcess;
using DXPANACEASOFT.Models.ModelExtension;
using DXPANACEASOFT.Models.PLDTO;
using DXPANACEASOFT.Models.PO;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using DXPANACEASOFT.Reports.ProductionLot;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.Logs;
using Utilitarios.ProdException;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ProductionLotReceptionController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IndexReportLote(string ID)
        {

            try
            {
                Session["URLOTE"] = ConfigurationManager.AppSettings["URLOTE"];
            }
            catch (Exception ex)
            {

                ViewBag.IframeUrl = ex.Message;

            }

            ViewBag.IframeUrl = Session["URLOTE"] + "?id=" + ID;



            //return Redirect("../Views/AditionalReport/WReportGuia.aspx"); //  View("WReportGuia"); //Aspx file Views/Products/WebForm1.aspx
            return PartialView();
        }

        #region Remision Guide ResultsPartial

        [HttpPost]
        public ActionResult RemissionGuideResults()
        {
            #region New Version
            var queryRelationPLRG = db.ProductionLotDetailPurchaseDetail
                                        .Where(w => !w.ProductionLotDetail.ProductionLot.ProductionLotState.code.Equals("09"))
                                        .Select(s => new
                                        {
                                            s.ProductionLotDetail.quantityRecived,
                                            s.RemissionGuideDetail.id_remisionGuide
                                        });
            var queryRelationPLRGgroupResult = from x in queryRelationPLRG
                                               group x by x.id_remisionGuide into x
                                               select new
                                               {
                                                   IdRemissionGuide = x.Key,
                                                   TotalQuantityReceived = x.Sum(s => s.quantityRecived),
                                               };

            var queryRemissionGuide = db.RemissionGuideDetail
                                        .Where(w => (w.RemissionGuide.Document.DocumentState.code.Equals("06")
                                        || w.RemissionGuide.Document.DocumentState.code.Equals("03")
                                        || w.RemissionGuide.Document.DocumentState.code.Equals("09"))
                                        && ((w.RemissionGuide.RemissionGuideTransportation.isOwn == false &&
                                            w.RemissionGuide.hasExitPlanctProduction == true &&
                                            w.RemissionGuide.hasEntrancePlanctProduction == true) ||
                                            (w.RemissionGuide.RemissionGuideTransportation.isOwn == true &&
                                            w.RemissionGuide.hasEntrancePlanctProduction == true))
                                        && (w.RemissionGuide.Document.isOpen != true))
                                            .Select(s => new
                                            {
                                                s.id_remisionGuide,
                                                s.quantityProgrammed
                                            });
            var queryRemissionGuidegroupResult = from x in queryRemissionGuide
                                                 group x by x.id_remisionGuide into x
                                                 select new
                                                 {
                                                     IdRemissionGuide = x.Key,
                                                     TotalQuantityProgrammed = x.Sum(s => s.quantityProgrammed),
                                                 };

            var queryRGsResult = from x in queryRemissionGuidegroupResult
                                 join y in queryRelationPLRGgroupResult on x.IdRemissionGuide equals y.IdRemissionGuide into xy
                                 from xyrg in xy.DefaultIfEmpty()
                                 select new
                                 {
                                     x.IdRemissionGuide,
                                     TotalQuantityPending = xyrg == null ? x.TotalQuantityProgrammed : x.TotalQuantityProgrammed - xyrg.TotalQuantityReceived
                                 };

            var lstRgsResultList = queryRGsResult
                .Where(w => w.TotalQuantityPending > 0)
                .Select(s => s.IdRemissionGuide)
                .ToList();

            #endregion

            var model = db.RemissionGuide
                .Where(w => lstRgsResultList.Contains(w.id))
                .Include(w => w.Provider1.Person)
                .Include(w => w.Document)
                .OrderByDescending(r => r.id)
                .ToList();
            //var model = db.RemissionGuide.ToList();
            //#region RemissionGuide FILTERS
            ////Autorizadas
            //model = model.Where(o => (o.Document.DocumentState.code == "06" || o.Document.DocumentState.code == "03") &&
            //                            ((o.RemissionGuideTransportation.isOwn == false &&
            //                            o.hasExitPlanctProduction == true &&
            //                            o.hasEntrancePlanctProduction == true) ||
            //                            (o.RemissionGuideTransportation.isOwn == true &&
            //                            o.hasEntrancePlanctProduction == true))).ToList();
            ////Cantidades Pendientes   
            ////var lstRGPOD = db.ProductionLotDetailPurchaseDetail.Select(s => s.id_remissionGuideDetail).Distinct().ToList();

            //model = model.Where(w => w.RemissionGuideDetail.Any(a =>
            //                            (a.quantityProgrammed - a.ProductionLotDetailPurchaseDetail
            //                                                    .Where(wh => wh.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05")
            //                                                    .Sum(s => s.ProductionLotDetail.quantityRecived) > 0))).ToList();
            //#endregion



            TempData["listProductionLotRemissionGuides"] = model;
            TempData.Keep("listProductionLotRemissionGuides");

            return PartialView("_RemissionGuideResultsPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuidePartial()
        {
            var model = (TempData["listProductionLotRemissionGuides"] as List<RemissionGuide>);
            model = model ?? new List<RemissionGuide>();

            TempData.Keep("listProductionLotRemissionGuides");

            return PartialView("_RemissionGuidePartial", model);
        }

        #endregion

        #region PRODUCTION LOT RECEPTION FILTERS RESULTS

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsResults()
        {
            var model = db.PurchaseOrderDetail.Where(d => d.PurchaseOrder.Document.EmissionPoint.id_company == this.ActiveCompanyId &&
                                                          d.PurchaseOrder.Document.DocumentState.code == "06" &&
                                                          d.Item.InventoryLine.code == "MP" && //MP: Materia Prima(Para la producción)
                                                          ((!d.PurchaseOrder.requiredLogistic && d.quantityReceived < d.quantityApproved) || d.PurchaseOrder.requiredLogistic)).ToList();// "06" AUTORIZADA
            var listPendingPurchaseOrdersAndRemissionGuides = new List<PendingPurchaseOrdersAndRemissionGuides>();
            PendingPurchaseOrdersAndRemissionGuides newPendingPurchaseOrdersAndRemissionGuides;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            foreach (var m in model)
            {
                var quantityPendingOrderAux = m.quantityApproved - m.quantityReceived;
                var metricUnitAux = m.Item.ItemPurchaseInformation?.MetricUnit?.code ?? (metricUnitUMTP?.code ?? "");//"Lbs";
                if (!m.PurchaseOrder.requiredLogistic)
                {
                    newPendingPurchaseOrdersAndRemissionGuides = new PendingPurchaseOrdersAndRemissionGuides()
                    {
                        id = listPendingPurchaseOrdersAndRemissionGuides.Count() > 0 ? listPendingPurchaseOrdersAndRemissionGuides.Max(lppoarg => lppoarg.id) + 1 : 1,
                        id_purchaseOrderDetail = m.id,
                        purchaseOrderDetail = m,
                        id_remissionGuideDetail = null,
                        remissionGuideDetail = null,
                        metricUnit = metricUnitAux,
                        quantityPendingOrder = quantityPendingOrderAux > 0 ? quantityPendingOrderAux : 0,
                        quantityPendingGuide = 0,
                        id_provider = m.PurchaseOrder.id_provider,
                        provider = m.PurchaseOrder.Provider,
                        id_buyer = m.PurchaseOrder.id_buyer,
                        buyer = m.PurchaseOrder.Person,
                        withPrice = !m.PurchaseOrder.pricePerList
                    };
                    listPendingPurchaseOrdersAndRemissionGuides.Add(newPendingPurchaseOrdersAndRemissionGuides);
                }
                else
                {
                    if (m.RemissionGuideDetailPurchaseOrderDetail != null)
                    {
                        foreach (var mrgdpod in m.RemissionGuideDetailPurchaseOrderDetail)
                        {
                            if (mrgdpod.RemissionGuideDetail.quantityReceived < mrgdpod.RemissionGuideDetail.quantityProgrammed &&
                                mrgdpod.RemissionGuideDetail.RemissionGuide.Document.EmissionPoint.id_company == this.ActiveCompanyId &&
                                mrgdpod.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code == "06")// "06" AUTORIZADA
                            {
                                newPendingPurchaseOrdersAndRemissionGuides = new PendingPurchaseOrdersAndRemissionGuides()
                                {
                                    id = listPendingPurchaseOrdersAndRemissionGuides.Count() > 0 ? listPendingPurchaseOrdersAndRemissionGuides.Max(lppoarg => lppoarg.id) + 1 : 1,
                                    id_purchaseOrderDetail = m.id,
                                    purchaseOrderDetail = m,
                                    id_remissionGuideDetail = mrgdpod.id_remissionGuideDetail,
                                    remissionGuideDetail = mrgdpod.RemissionGuideDetail,
                                    metricUnit = metricUnitAux,
                                    quantityPendingOrder = quantityPendingOrderAux > 0 ? quantityPendingOrderAux : 0,
                                    quantityPendingGuide = mrgdpod.RemissionGuideDetail.quantityProgrammed - mrgdpod.RemissionGuideDetail.quantityReceived,
                                    id_provider = m.PurchaseOrder.id_provider,
                                    provider = m.PurchaseOrder.Provider,
                                    id_buyer = m.PurchaseOrder.id_buyer,
                                    buyer = m.PurchaseOrder.Person,
                                    withPrice = !m.PurchaseOrder.pricePerList
                                };
                                listPendingPurchaseOrdersAndRemissionGuides.Add(newPendingPurchaseOrdersAndRemissionGuides);
                            }
                        }
                    }
                }
            }
            TempData["listPendingPurchaseOrdersAndRemissionGuides"] = listPendingPurchaseOrdersAndRemissionGuides;
            TempData.Keep("listPendingPurchaseOrdersAndRemissionGuides");
            return PartialView("_PurchaseOrderDetailsResultsPartial", listPendingPurchaseOrdersAndRemissionGuides.OrderBy(d => d.purchaseOrderDetail.Item.name).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PurchaseOrderDetailsPartial()
        {
            var model = db.PurchaseOrderDetail.Where(d => d.PurchaseOrder.Document.EmissionPoint.id_company == this.ActiveCompanyId &&
                                                          d.PurchaseOrder.Document.DocumentState.code == "06" &&
                                                          d.Item.InventoryLine.code == "MP" && //MP: Materia Prima(Para la producción)
                                                          ((!d.PurchaseOrder.requiredLogistic && d.quantityReceived < d.quantityApproved) || d.PurchaseOrder.requiredLogistic)).ToList();// "06" AUTORIZADA
            var listPendingPurchaseOrdersAndRemissionGuides = new List<PendingPurchaseOrdersAndRemissionGuides>();
            PendingPurchaseOrdersAndRemissionGuides newPendingPurchaseOrdersAndRemissionGuides;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            foreach (var m in model)
            {
                var quantityPendingOrderAux = m.quantityApproved - m.quantityReceived;
                var metricUnitAux = m.Item.ItemPurchaseInformation?.MetricUnit?.code ?? (metricUnitUMTP?.code ?? "");//"Lbs";
                if (!m.PurchaseOrder.requiredLogistic)
                {
                    newPendingPurchaseOrdersAndRemissionGuides = new PendingPurchaseOrdersAndRemissionGuides()
                    {
                        id = listPendingPurchaseOrdersAndRemissionGuides.Count() > 0 ? listPendingPurchaseOrdersAndRemissionGuides.Max(lppoarg => lppoarg.id) + 1 : 1,
                        id_purchaseOrderDetail = m.id,
                        purchaseOrderDetail = m,
                        id_remissionGuideDetail = null,
                        remissionGuideDetail = null,
                        metricUnit = metricUnitAux,
                        quantityPendingOrder = quantityPendingOrderAux > 0 ? quantityPendingOrderAux : 0,
                        quantityPendingGuide = 0,
                        id_provider = m.PurchaseOrder.id_provider,
                        provider = m.PurchaseOrder.Provider,
                        id_buyer = m.PurchaseOrder.id_buyer,
                        buyer = m.PurchaseOrder.Person,
                        withPrice = !m.PurchaseOrder.pricePerList
                    };
                    listPendingPurchaseOrdersAndRemissionGuides.Add(newPendingPurchaseOrdersAndRemissionGuides);
                }
                else
                {
                    if (m.RemissionGuideDetailPurchaseOrderDetail != null)
                    {
                        foreach (var mrgdpod in m.RemissionGuideDetailPurchaseOrderDetail)
                        {
                            if (mrgdpod.RemissionGuideDetail.quantityReceived < mrgdpod.RemissionGuideDetail.quantityProgrammed &&
                                mrgdpod.RemissionGuideDetail.RemissionGuide.Document.EmissionPoint.id_company == this.ActiveCompanyId &&
                                mrgdpod.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code == "06")// "06" AUTORIZADA
                            {
                                newPendingPurchaseOrdersAndRemissionGuides = new PendingPurchaseOrdersAndRemissionGuides()
                                {
                                    id = listPendingPurchaseOrdersAndRemissionGuides.Count() > 0 ? listPendingPurchaseOrdersAndRemissionGuides.Max(lppoarg => lppoarg.id) + 1 : 1,
                                    id_purchaseOrderDetail = m.id,
                                    purchaseOrderDetail = m,
                                    id_remissionGuideDetail = mrgdpod.id_remissionGuideDetail,
                                    remissionGuideDetail = mrgdpod.RemissionGuideDetail,
                                    metricUnit = metricUnitAux,
                                    quantityPendingOrder = quantityPendingOrderAux > 0 ? quantityPendingOrderAux : 0,
                                    quantityPendingGuide = mrgdpod.RemissionGuideDetail.quantityProgrammed - mrgdpod.RemissionGuideDetail.quantityReceived,
                                    id_provider = m.PurchaseOrder.id_provider,
                                    provider = m.PurchaseOrder.Provider,
                                    id_buyer = m.PurchaseOrder.id_buyer,
                                    buyer = m.PurchaseOrder.Person,
                                    withPrice = !m.PurchaseOrder.pricePerList
                                };
                                listPendingPurchaseOrdersAndRemissionGuides.Add(newPendingPurchaseOrdersAndRemissionGuides);
                            }
                        }
                    }
                }
            }
            TempData["listPendingPurchaseOrdersAndRemissionGuides"] = listPendingPurchaseOrdersAndRemissionGuides;
            TempData.Keep("listPendingPurchaseOrdersAndRemissionGuides");
            return PartialView("_PurchaseOrderDetailsPartial", listPendingPurchaseOrdersAndRemissionGuides.OrderBy(d => d.purchaseOrderDetail.Item.name).ToList());
        }

        #endregion

        #region PRODUCTION LOT RECEPTION EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionFormEditPartial(int id, bool? loteManual, int[] ids, string[] arrayTempDataKeep, int? tabSelected, bool? toReturn = null)
        {
            UpdateArrayTempDataKeep(arrayTempDataKeep);

            ProductionLot productionLot = new ProductionLot();

            ViewData["tabSelected"] = tabSelected;

            if (toReturn == true)
            {
                productionLot = (TempData["productionLotReception"] as ProductionLot);
                if (productionLot != null && productionLot.id > 0)
                {
                    productionLot.internalNumber = db.ProductionLot.FirstOrDefault(fod => fod.id == productionLot.id).internalNumber;
                }
            }
            else
            {
                productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            }

            bool loteManualEdit = false;
            if (productionLot != null)
            {
                loteManualEdit = productionLot.ProductionProcess.code == "RMM";
            }

            this.ViewBag.loteManualEdit = loteManualEdit;
            string loteManualParm = db.Setting.FirstOrDefault(fod => fod.code == "PLOM")?.value ?? "NO";
            if (productionLot == null)
            {
                ProductionLotState state = db.ProductionLotState.FirstOrDefault(s => s.code.Equals("01"));
                ProductionProcess process = new ProductionProcess();
                if (loteManual.Value == true)
                {
                    process = db.ProductionProcess.FirstOrDefault(p => p.code.Equals("RMM"));
                }
                else
                {
                    process = db.ProductionProcess.FirstOrDefault(p => p.code.Equals("REC"));
                }
                ProductionUnit productionUnit = process?.ProductionUnit/*.First()*/ ?? db.ProductionUnit.First();

                DateTime receptionDate = DateTime.Now;

                var DCLP = db.Setting.FirstOrDefault(fod => fod.code == "DCLP" && fod.id_company == this.ActiveCompanyId)?.value ?? "0";
                var int_DCLP = int.Parse(DCLP);
                DateTime expirationDate = DateTime.Now.AddDays(int_DCLP);

                Employee employee = ActiveUser.Employee;

                productionLot = new ProductionLot
                {
                    id_ProductionLotState = state?.id ?? 0,
                    ProductionLotState = state,

                    id_productionProcess = process?.id ?? 0,
                    ProductionProcess = process,
                    id_productionUnit = productionUnit?.id ?? 0,
                    ProductionUnit = productionUnit,

                    receptionDate = receptionDate,
                    expirationDate = expirationDate,
                    id_personReceiving = employee?.id ?? 0,
                    Employee = employee,
                    withPrice = false,
                    pricePerLbs = 0
                };

                productionLot.julianoNumber = DataProviderJulianoNumber.GetJulianoNumber(receptionDate);
                productionLot.internalNumberConcatenated = productionLot.julianoNumber;

                if (loteManual.Value == true)
                {
                    string rucCompany = db.Company.FirstOrDefault(r => r.id == process.id_company).ruc;
                    var PersonProcess = db.Person.FirstOrDefault(p => p.identification_number == rucCompany);
                    productionLot.id_personProcessPlant = PersonProcess.id;
                    productionLot.Person1 = PersonProcess;

                }

                if (ids != null && !loteManual.Value == true)
                {
                    List<PendingPurchaseOrdersAndRemissionGuides> listPendingPurchaseOrdersAndRemissionGuides = (TempData["listPendingPurchaseOrdersAndRemissionGuides"] as List<PendingPurchaseOrdersAndRemissionGuides>);

                    listPendingPurchaseOrdersAndRemissionGuides = listPendingPurchaseOrdersAndRemissionGuides ?? new List<PendingPurchaseOrdersAndRemissionGuides>();

                    List<RemissionGuideDispatchMaterial> remissionGuideMaterials = new List<RemissionGuideDispatchMaterial>();


                    foreach (var i in ids)
                    {
                        PendingPurchaseOrdersAndRemissionGuides pendingPurchaseOrdersAndRemissionGuides =
                        listPendingPurchaseOrdersAndRemissionGuides.FirstOrDefault(
                            d =>
                                d.id == i);

                        if (productionLot.id_provider == null)
                        {
                            productionLot.id_provider = pendingPurchaseOrdersAndRemissionGuides.id_provider;
                            productionLot.id_buyer = pendingPurchaseOrdersAndRemissionGuides.id_buyer;
                            productionLot.withPrice = pendingPurchaseOrdersAndRemissionGuides.withPrice;

                            //modificado para estraer los datos de proveedor amparante, unidad de produccion y picina
                            productionLot.id_providerapparent = pendingPurchaseOrdersAndRemissionGuides.purchaseOrderDetail?.PurchaseOrder?.id_providerapparent;
                            productionLot.id_productionUnitProvider = pendingPurchaseOrdersAndRemissionGuides.purchaseOrderDetail?.PurchaseOrder?.id_productionUnitProvider;

                            RefreshProductionUnitProvider(productionLot.id_provider, productionLot.id_productionUnitProvider);

                        }

                        ProductionLotDetail productionLotDetail = new ProductionLotDetail
                        {
                            id = pendingPurchaseOrdersAndRemissionGuides.id,
                            id_item = pendingPurchaseOrdersAndRemissionGuides.purchaseOrderDetail.id_item,
                            Item = pendingPurchaseOrdersAndRemissionGuides.purchaseOrderDetail.Item,

                            id_warehouse = pendingPurchaseOrdersAndRemissionGuides.purchaseOrderDetail.Item.ItemInventory?.Warehouse?.id ?? 0,

                            id_warehouseLocation = pendingPurchaseOrdersAndRemissionGuides.purchaseOrderDetail.Item.ItemInventory?.WarehouseLocation?.id ?? 0,

                            ProductionLotDetailPurchaseDetail = new List<ProductionLotDetailPurchaseDetail>()
                        };

                        if (pendingPurchaseOrdersAndRemissionGuides != null)
                        {
                            var quantityPendingAux = pendingPurchaseOrdersAndRemissionGuides.id_remissionGuideDetail != null ? pendingPurchaseOrdersAndRemissionGuides.quantityPendingGuide : pendingPurchaseOrdersAndRemissionGuides.quantityPendingOrder;
                            productionLotDetail.quantityOrdered = quantityPendingAux;
                            productionLotDetail.quantityRemitted = quantityPendingAux;
                            productionLotDetail.quantityRecived = quantityPendingAux;

                            productionLotDetail.ProductionLotDetailPurchaseDetail.Add(new ProductionLotDetailPurchaseDetail
                            {
                                id_purchaseOrderDetail = pendingPurchaseOrdersAndRemissionGuides.id_purchaseOrderDetail,
                                id_remissionGuideDetail = pendingPurchaseOrdersAndRemissionGuides.id_remissionGuideDetail,
                                quanty = quantityPendingAux
                            });
                            if (pendingPurchaseOrdersAndRemissionGuides.id_remissionGuideDetail != null)
                            {
                                foreach (var remissionGuideDispatchMaterial in pendingPurchaseOrdersAndRemissionGuides.remissionGuideDetail.RemissionGuide.RemissionGuideDispatchMaterial)
                                {
                                    var exists = productionLot.ProductionLotDispatchMaterial.Where(w => w.ProductionLotDispatchMaterialRemissionDetail.FirstOrDefault(fod => fod.id_remissionGuideDispatchMaterial == remissionGuideDispatchMaterial.id) != null).ToList();
                                    if (exists.Count() == 0)
                                    {
                                        Setting settingUDLI = db.Setting.FirstOrDefault(t => t.code == "UDLI");

                                        WarehouseLocation warehouseLocationAux = null;

                                        var id_inventoryLineAux = db.Item.FirstOrDefault(it => it.id == remissionGuideDispatchMaterial.id_item).id_inventoryLine.ToString();
                                        var id_warehouseLocationAux = settingUDLI?.SettingDetail.FirstOrDefault(fod => fod.value == id_inventoryLineAux)?.valueAux;

                                        var id_warehouseLocationAuxInt = (id_warehouseLocationAux != null) ? int.Parse(id_warehouseLocationAux) : (int?)null;
                                        warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationAuxInt);

                                        ProductionLotDispatchMaterial productionLotMaterial = new ProductionLotDispatchMaterial
                                        {
                                            id = productionLot.ProductionLotDispatchMaterial.Count() > 0 ? productionLot.ProductionLotDispatchMaterial.Max(lppoarg => lppoarg.id) + 1 : 1,
                                            id_item = remissionGuideDispatchMaterial.id_item,
                                            Item = db.Item.FirstOrDefault(it => it.id == remissionGuideDispatchMaterial.id_item),
                                            id_warehouse = warehouseLocationAux?.Warehouse.id ?? 0,
                                            Warehouse = warehouseLocationAux?.Warehouse,
                                            id_warehouseLocation = warehouseLocationAux?.id ?? 0,
                                            WarehouseLocation = warehouseLocationAux,
                                            ProductionLotDispatchMaterialRemissionDetail = new List<ProductionLotDispatchMaterialRemissionDetail>(),
                                            sourceExitQuantity = remissionGuideDispatchMaterial.sourceExitQuantity
                                        };

                                        productionLotMaterial.ProductionLotDispatchMaterialRemissionDetail.Add(new ProductionLotDispatchMaterialRemissionDetail
                                        {
                                            id_remissionGuideDispatchMaterial = remissionGuideDispatchMaterial.id,

                                        });
                                        productionLot.ProductionLotDispatchMaterial.Add(productionLotMaterial);
                                    }
                                }
                            }
                        }

                        productionLot.ProductionLotDetail.Add(productionLotDetail);
                    }
                }


            }
            else
            {
                int indexInit = 0;
                var aCertification = db.Certification.FirstOrDefault(fod => fod.id == productionLot.id_certification);
                if (aCertification != null) indexInit = aCertification.idLote?.Length ?? 0;
                productionLot.internalNumberConcatenated = productionLot.internalNumber;//(aCertification?.idLote ?? "") + productionLot.julianoNumber + productionLot.internalNumber;

                if (productionLot.internalNumber.Length > (5 + indexInit))
                {
                    productionLot.julianoNumber = productionLot.internalNumber.Substring((indexInit + 0), 5);
                    productionLot.internalNumber = productionLot.internalNumber.Substring((indexInit + 5), (productionLot.internalNumber.Length - (indexInit + 5)));
                }
                else
                {
                    productionLot.julianoNumber = productionLot.internalNumber;
                    //productionLot.internalNumber = "";
                }


                //productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;
                RefreshProductionUnitProvider(productionLot.id_provider, productionLot.id_productionUnitProvider);

                if (productionLot.id_priceList == null)
                {
                    string settingTLA = DataProviderSetting.ValueSetting("TLA");
                    if (string.IsNullOrEmpty(settingTLA) || !settingTLA.Equals("REF"))
                    {
                        AdvanceProvider apTmp = db.AdvanceProvider.FirstOrDefault(fod => fod.id_Lot == productionLot.id);
                        if (apTmp != null && apTmp.id_priceList != null && apTmp.id_priceList > 0)
                        {
                            productionLot.id_priceList = apTmp.id_priceList;
                        }
                    }

                }
                if (productionLot.liquidationPaymentDate == null)
                {
                    productionLot.liquidationPaymentDate = DateTime.Now;
                }
            }

            if (productionLot.isCopackingLot == null)
                productionLot.isCopackingLot = false;
            
            this.ViewBag.isCopacking = productionLot.isCopackingLot;
            this.ViewBag.loteManual = loteManual;

            UpdateProductionLotTotals(productionLot);

            string settCxC = db.Setting.FirstOrDefault(fod => fod.code == "HLCXC")?.value ?? "0";

            if (settCxC != "0" || productionLot.ProductionLotLiquidationTotal.Any())
            {
                if (productionLot.ProductionLotState.code == "02" || productionLot.ProductionLotState.code == "03") UpdateProductionLotProductionLotLiquidationTotals(productionLot);
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            if (arrayTempDataKeep != null)
            {
                ViewData["ModelLink"] = productionLot;
                return PartialView("LinkBoxTemplates/_LinkBox", "_FormEditProductionLotReception");
            }
            else
            {
                return PartialView("_FormEditProductionLotReception", productionLot);
            }


        }

        private void RefreshProductionUnitProvider(int? id_provider, int? id_productionUnitProvider)
        {
            TempData["ProductionUnitProviderByProvider"] = DataProviders.DataProviderProductionUnitProvider.ProductionUnitProviderByProvider(null, id_provider);
            TempData.Keep("ProductionUnitProviderByProvider");

            TempData["ProductionUnitProviderPoolByUnitProvider"] = DataProviders.DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolByUnitProvider(null, id_productionUnitProvider);
            TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionCopy(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

            ProductionLot productionLotCopy = null;
            if (productionLot != null)
            {
                string intNumber = productionLot.internalNumber;
                productionLot.internalNumber = intNumber.Substring(1, 5);
                productionLot.internalNumber = intNumber.Substring(5, (intNumber.Length - 1));

                ProductionLotState state = db.ProductionLotState.FirstOrDefault(s => s.code.Equals("01"));

                ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.code.Equals("REC"));
                ProductionUnit productionUnit = process?.ProductionUnit/*.First()*/ ?? db.ProductionUnit.First();

                DateTime receptionDate = DateTime.Now;//.ToString("dd/MM/yyyy");

                Employee employee = ActiveUser.Employee;
                Provider provider = db.Provider.FirstOrDefault(p => p.id == productionLot.id_provider);

                productionLotCopy = new ProductionLot
                {
                    id_ProductionLotState = state?.id ?? 0,
                    ProductionLotState = state,

                    id_productionProcess = process?.id ?? 0,
                    ProductionProcess = process,
                    id_productionUnit = productionUnit?.id ?? 0,
                    ProductionUnit = productionUnit,
                    //id_itemResulting = itemResulting?.id ?? 0,
                    //Item = itemResulting,

                    id_provider = productionLot.id_provider,
                    Provider = provider,

                    receptionDate = receptionDate,
                    id_personReceiving = employee?.id ?? 0,
                    Employee = employee
                };

                if (productionLot.ProductionLotDetail != null)
                {
                    productionLotCopy.ProductionLotDetail = new List<ProductionLotDetail>();
                    foreach (var detail in productionLot.ProductionLotDetail.AsEnumerable())
                    {
                        productionLotCopy.ProductionLotDetail.Add(new ProductionLotDetail
                        {
                            id_item = detail.id_item,
                            id_originLot = detail.id_originLot,
                            id_warehouse = detail.id_warehouse,
                            id_warehouseLocation = detail.id_warehouseLocation,
                            quantityOrdered = detail.quantityOrdered,
                            quantityRecived = detail.quantityRecived,
                            quantityRemitted = detail.quantityRemitted,
                            quantityProcessed = detail.quantityProcessed
                        });
                    }
                }

                if (productionLot.ProductionLotDispatchMaterial != null)
                {
                    productionLotCopy.ProductionLotDispatchMaterial = new List<ProductionLotDispatchMaterial>();
                    foreach (var detail in productionLot.ProductionLotDispatchMaterial.AsEnumerable())
                    {
                        productionLotCopy.ProductionLotDispatchMaterial.Add(new ProductionLotDispatchMaterial
                        {
                            id_item = detail.id_item,
                            sendedDestinationQuantity = detail.sendedDestinationQuantity,
                            arrivalDestinationQuantity = detail.arrivalDestinationQuantity,
                            sourceExitQuantity = detail.sourceExitQuantity,
                            arrivalGoodCondition = detail.arrivalGoodCondition,
                            arrivalBadCondition = detail.arrivalBadCondition
                        });
                    }
                }

                if (productionLot.ProductionLotLiquidation != null)
                {
                    productionLotCopy.ProductionLotLiquidation = new List<ProductionLotLiquidation>();
                    foreach (var detail in productionLot.ProductionLotLiquidation)
                    {
                        productionLotCopy.ProductionLotLiquidation.Add(new ProductionLotLiquidation
                        {
                            id_item = detail.id_item,
                            id_warehouse = detail.id_warehouse,
                            id_warehouseLocation = detail.id_warehouseLocation,
                            quantity = detail.quantity
                        });
                    }
                }

                if (productionLot.ProductionLotTrash != null)
                {
                    productionLotCopy.ProductionLotTrash = new List<ProductionLotTrash>();
                    foreach (var detail in productionLot.ProductionLotTrash)
                    {
                        productionLotCopy.ProductionLotTrash.Add(new ProductionLotTrash
                        {
                            id_item = detail.id_item,
                            id_warehouse = detail.id_warehouse,
                            id_warehouseLocation = detail.id_warehouseLocation,
                            quantity = detail.quantity
                        });
                    }
                }

                UpdateProductionLotTotals(productionLotCopy);
            }



            TempData["productionLotReception"] = productionLotCopy;
            TempData.Keep("productionLotReception");

            return PartialView("_FormEditProductionLotReception", productionLotCopy);
        }

        #endregion

        #region ProductionUnitProvider

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderPoolByUnitProvider(int? id_productionUnitProviderPoolCurrent, int? id_productionUnitProvider)
        {
            var productionUnitProviderPoolAux = db.ProductionUnitProviderPool.Where(t => t.isActive && t.id_productionUnitProvider == id_productionUnitProvider).ToList();

            var productionUnitProviderPoolCurrentAux = db.ProductionUnitProviderPool.FirstOrDefault(fod => fod.id == id_productionUnitProviderPoolCurrent);
            if (productionUnitProviderPoolCurrentAux != null && !productionUnitProviderPoolAux.Contains(productionUnitProviderPoolCurrentAux)) productionUnitProviderPoolAux.Add(productionUnitProviderPoolCurrentAux);



            TempData["ProductionUnitProviderPoolByUnitProvider"] = productionUnitProviderPoolAux.Select(s => new
            {
                s.id,
                s.code
            }).OrderBy(t => t.id).ToList();
            TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            TempData.Keep("productionLotReception");
            productionLot.id_productionUnitProvider = id_productionUnitProvider;
            return PartialView("comboboxcascading/_cmbProviderProductionUnitProviderPoolPartial", productionLot);


        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionUnitProviderByProvider(int? id_productionUnitProviderCurrent, int? id_provider)
        {

            if (id_provider == null || id_provider < 0)
            {
                if (Request.Params["id_provider"] != null && Request.Params["id_provider"] != "") id_provider = int.Parse(Request.Params["id_provider"]);
                else id_provider = -1;
            }
            var productionUnitProviderAux = db.ProductionUnitProvider.Where(t => t.isActive && t.id_provider == id_provider).ToList();

            var productionUnitProviderCurrentAux = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionUnitProviderCurrent);
            if (productionUnitProviderCurrentAux != null && !productionUnitProviderAux.Contains(productionUnitProviderCurrentAux)) productionUnitProviderAux.Add(productionUnitProviderCurrentAux);


            TempData["ProductionUnitProviderByProvider"] = productionUnitProviderAux.Select(s => new
            {
                s.id,
                s.name
            }).OrderBy(t => t.id).ToList();

            TempData.Keep("ProductionUnitProviderByProvider");
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot.id_provider = id_provider;
            TempData.Keep("productionLotReception");
            return PartialView("comboboxcascading/_cmbProviderProductionUnitPartial", productionLot);
        }

        #endregion

        #region PRODUCTION LOT RECEPTION EDITFORM GUIDREMISION

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionFormEditPartialGuiadRemision(int id, int[] ids, string[] arrayTempDataKeep, bool? toReturn = null, int? tabSelected = null)
        {
            //UpdateArrayTempDataKeep(arrayTempDataKeep);

            ProductionLot productionLot = new ProductionLot();

            ProductionLotState state = db.ProductionLotState.FirstOrDefault(s => s.code.Equals("01"));

            ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.code.Equals("REC"));
            ProductionUnit productionUnit = process?.ProductionUnit/*.First()*/ ?? db.ProductionUnit.First();

            DateTime receptionDate = DateTime.Now;//.ToString("dd/MM/yyyy");

            string settLE = DataProviderSetting.ValueSetting("ULERMP");

            var DCLP = db.Setting.FirstOrDefault(fod => fod.code == "DCLP" && fod.id_company == this.ActiveCompanyId)?.value ?? "0";
            var int_DCLP = int.Parse(DCLP);
            DateTime expirationDate = DateTime.Now.AddDays(int_DCLP);

            Employee employee = ActiveUser.Employee;

            productionLot = new ProductionLot
            {
                id_ProductionLotState = state?.id ?? 0,
                ProductionLotState = state,
                id_productionProcess = process?.id ?? 0,
                ProductionProcess = process,
                id_productionUnit = productionUnit?.id ?? 0,
                ProductionUnit = productionUnit,
                receptionDate = receptionDate,
                expirationDate = expirationDate,
                id_personReceiving = employee?.id ?? 0,
                Employee = employee,
                withPrice = false,
                pricePerLbs = 0
            };
            productionLot.julianoNumber = DataProviderJulianoNumber.GetJulianoNumber(receptionDate);
            if (ids != null)
            {
                TempData.Keep("listProductionLotRemissionGuides");

                List<RemissionGuide> listPendingPurchaseOrdersAndRemissionGuides = (TempData["listProductionLotRemissionGuides"] as List<RemissionGuide>);

                listPendingPurchaseOrdersAndRemissionGuides = listPendingPurchaseOrdersAndRemissionGuides ?? new List<RemissionGuide>();

                List<RemissionGuideDispatchMaterial> remissionGuideMaterials = new List<RemissionGuideDispatchMaterial>();

                if (!settLE.Equals("Y"))
                {
                    ViewBag.WarningDrainedQuantity = WarningMessage("Debe indicar la cantidad de libras escurrido por cada guía, de lo contrario se utilizará la cantidad remitida.");
                }



                foreach (var i in ids)
                {
                    RemissionGuide pendingPurchaseOrdersAndRemissionGuides = listPendingPurchaseOrdersAndRemissionGuides.FirstOrDefault(d => d.id == i);

                    if (productionLot.id_personProcessPlant == null)
                    {
                        productionLot.id_personProcessPlant =
                            pendingPurchaseOrdersAndRemissionGuides.id_personProcessPlant;
                        productionLot.Person1 =
                            db.Person.FirstOrDefault(p => p.id == pendingPurchaseOrdersAndRemissionGuides.id_personProcessPlant);
                    }
                    if (productionLot.id_provider == null)
                    {
                        productionLot.id_provider = pendingPurchaseOrdersAndRemissionGuides.id_providerRemisionGuide;
                        productionLot.id_buyer = pendingPurchaseOrdersAndRemissionGuides.id_buyer;

                        string settingTLOC = DataProviderSetting.ValueSetting("TLOC");
                        if (!string.IsNullOrEmpty(settingTLOC) && settingTLOC.Equals("REF"))
                        {
                            productionLot.id_priceList = null;
                        }
                        else
                        {
                            productionLot.id_priceList = pendingPurchaseOrdersAndRemissionGuides.id_priceList;
                        }

                        productionLot.id_providerapparent = pendingPurchaseOrdersAndRemissionGuides.id_protectiveProvider;
                        productionLot.id_productionUnitProvider = pendingPurchaseOrdersAndRemissionGuides.id_productionUnitProvider;
                        productionLot.id_productionUnitProviderPool = pendingPurchaseOrdersAndRemissionGuides.id_productionUnitProviderPool;

                        RefreshProductionUnitProvider(productionLot.id_provider, productionLot.id_productionUnitProvider);
                    }
                    if (productionLot.id_certification == null)
                    {
                        productionLot.id_certification = pendingPurchaseOrdersAndRemissionGuides.id_certification;
                    }

                    #region RemissionGuideDetail
                    if (pendingPurchaseOrdersAndRemissionGuides != null)
                    {
                        var rgd = pendingPurchaseOrdersAndRemissionGuides.RemissionGuideDetail.ToList();
                        var rgdFirst = rgd.FirstOrDefault();

                        ProductionLotDetail productionLotDetail = new ProductionLotDetail
                        {
                            id = pendingPurchaseOrdersAndRemissionGuides.id,
                            id_item = rgdFirst.id_item,
                            Item = rgdFirst.Item,
                            id_warehouse = rgdFirst.Item.ItemInventory?.Warehouse?.id ?? 0,
                            id_warehouseLocation = rgdFirst.Item.ItemInventory?.WarehouseLocation?.id ?? 0,
                            ProductionLotDetailPurchaseDetail = new List<ProductionLotDetailPurchaseDetail>()
                        };

                        //var quantityPendingAux = (remissionGuideDetail.quantityProgrammed - remissionGuideDetail.ProductionLotDetailPurchaseDetail.Where(w => w.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05").Sum(s => s.quanty));

                        decimal valReceived = rgd.Select(a => (a.ProductionLotDetailPurchaseDetail
                                                                 .Where(wh => wh.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05")
                                                                 .Sum(s => s.ProductionLotDetail.quantityRecived))).FirstOrDefault();

                        decimal quantityPendingAux = (rgd.Select(s => s.quantityProgrammed).DefaultIfEmpty(0).Sum() - valReceived);



                        productionLotDetail.quantityOrdered = quantityPendingAux;
                        productionLotDetail.quantityRemitted = quantityPendingAux;
                        productionLotDetail.quantityRecived = quantityPendingAux;

                        foreach (var remissionGuideDetail in pendingPurchaseOrdersAndRemissionGuides.RemissionGuideDetail)
                        {
                            productionLotDetail.ProductionLotDetailPurchaseDetail.Add(new ProductionLotDetailPurchaseDetail
                            {
                                id_purchaseOrderDetail = remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail != null ? remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault() != null ? remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail.FirstOrDefault().id_purchaseOrderDetail : 0 : (int)0,

                                id_remissionGuideDetail = remissionGuideDetail.id,

                                quanty = quantityPendingAux
                            });
                        }
                        productionLot.ProductionLotDetail.Add(productionLotDetail);
                    }
                    //if (pendingPurchaseOrdersAndRemissionGuides.RemissionGuideDetail != null)
                    //{

                    //    foreach (var remissionGuideDetail in pendingPurchaseOrdersAndRemissionGuides.RemissionGuideDetail)
                    //    {
                    //        ProductionLotDetail productionLotDetail = new ProductionLotDetail
                    //        {
                    //            id = remissionGuideDetail.id,
                    //            id_item = remissionGuideDetail.id_item,
                    //            id_warehouse = remissionGuideDetail.Item.ItemInventory?.Warehouse?.id ?? 0,
                    //            id_warehouseLocation = remissionGuideDetail.Item.ItemInventory?.WarehouseLocation?.id ?? 0,
                    //            ProductionLotDetailPurchaseDetail = new List<ProductionLotDetailPurchaseDetail>()
                    //        };
                    //    }
                    //}
                    #endregion

                    #region  RemissionGuideDispatchMaterial
                    if (pendingPurchaseOrdersAndRemissionGuides.RemissionGuideDetail != null)
                    {

                        foreach (var remissionGuideDispatchMaterial in pendingPurchaseOrdersAndRemissionGuides.RemissionGuideDispatchMaterial)
                        {
                            var exists = productionLot.ProductionLotDispatchMaterial.Where(w => w.ProductionLotDispatchMaterialRemissionDetail.FirstOrDefault(fod => fod.id_remissionGuideDispatchMaterial == remissionGuideDispatchMaterial.id) != null).ToList();
                            if (exists.Count() == 0)
                            {
                                Setting settingUDLI = db.Setting.FirstOrDefault(t => t.code == "UDLI");
                                WarehouseLocation warehouseLocationAux = null;

                                var id_inventoryLineAux = db.Item.FirstOrDefault(it => it.id == remissionGuideDispatchMaterial.id_item).id_inventoryLine.ToString();
                                var id_warehouseLocationAux = settingUDLI?.SettingDetail.FirstOrDefault(fod => fod.value == id_inventoryLineAux)?.valueAux;

                                var id_warehouseLocationAuxInt = (id_warehouseLocationAux != null) ? int.Parse(id_warehouseLocationAux) : (int?)null;
                                warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationAuxInt);

                                ProductionLotDispatchMaterial productionLotMaterial = new ProductionLotDispatchMaterial
                                {
                                    id = productionLot.ProductionLotDispatchMaterial.Count() > 0 ? productionLot.ProductionLotDispatchMaterial.Max(lppoarg => lppoarg.id) + 1 : 1,
                                    id_item = remissionGuideDispatchMaterial.id_item,
                                    Item = db.Item.FirstOrDefault(it => it.id == remissionGuideDispatchMaterial.id_item),
                                    id_warehouse = warehouseLocationAux?.Warehouse.id ?? 0,
                                    Warehouse = warehouseLocationAux?.Warehouse,
                                    id_warehouseLocation = warehouseLocationAux?.id ?? 0,
                                    WarehouseLocation = warehouseLocationAux,
                                    ProductionLotDispatchMaterialRemissionDetail = new List<ProductionLotDispatchMaterialRemissionDetail>(),
                                    sourceExitQuantity = remissionGuideDispatchMaterial.sourceExitQuantity
                                };

                                productionLotMaterial.ProductionLotDispatchMaterialRemissionDetail.Add(new ProductionLotDispatchMaterialRemissionDetail
                                {
                                    id_remissionGuideDispatchMaterial = remissionGuideDispatchMaterial.id,

                                });
                                productionLot.ProductionLotDispatchMaterial.Add(productionLotMaterial);
                            }
                        }
                    }
                    #endregion

                    if (pendingPurchaseOrdersAndRemissionGuides.isCopackingRG == null)
                        this.ViewBag.isCopacking = false;
                    else
                        this.ViewBag.isCopacking = pendingPurchaseOrdersAndRemissionGuides.isCopackingRG;

                }

            }

            productionLot.isCopackingLot = this.ViewBag.isCopacking;
            UpdateProductionLotTotals(productionLot);

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            if (arrayTempDataKeep != null)
            {
                ViewData["ModelLink"] = productionLot;
                return PartialView("LinkBoxTemplates/_LinkBox", "_FormEditProductionLotReception");
            }
            else
            {
                return PartialView("_FormEditProductionLotReception", productionLot);
            }


        }

        #endregion

        #region ResultGridView

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionResultsPartial(ProductionLot productionLot,
                                                                 int? filterWarehouse, int? filterWarehouseLocation,
                                                                 DateTime? startReceptionDate, DateTime? endReceptionDate,
                                                                 string liqNumber,
                                                                 int[] items)
        {
            List<ProductionLot> model = db.ProductionLot.Where(p => p.ProductionProcess.code == "REC" || p.ProductionProcess.code == "RMM").AsEnumerable().ToList();

            if (productionLot.id_ProductionLotState != 0)
            {
                model = model.Where(o => o.id_ProductionLotState == productionLot.id_ProductionLotState).ToList();
            }

            if (!String.IsNullOrEmpty(liqNumber))
            {
                model = model.Where(w => w.sequentialLiquidation != null).ToList();
                model = model.Where(w => w.sequentialLiquidation.ToString().Contains(liqNumber)).ToList();
            }

            if (!string.IsNullOrEmpty(productionLot.number))
            {
                model = model.Where(o => o.number.Contains(productionLot.number)).ToList();
            }

            if (!string.IsNullOrEmpty(productionLot.internalNumber))
            {
                model = model.Where(o => (o.internalNumber != null) && o.internalNumber.Contains(productionLot.internalNumber)).ToList();
            }


            if (!string.IsNullOrEmpty(productionLot.reference))
            {
                model = model.Where(o => (o.reference != null) && o.reference.Contains(productionLot.reference)).ToList();
            }

            if (productionLot.id_productionUnit != 0)
            {
                model = model.Where(o => o.id_productionUnit == productionLot.id_productionUnit).ToList();
            }

            if (filterWarehouse != null && filterWarehouse != 0)
            {
                var tempModel = new List<ProductionLot>();
                foreach (var production in model)
                {
                    var details = production.ProductionLotDetail.Where(d => d.id_warehouse == filterWarehouse.Value);
                    if (details.Any())
                    {
                        tempModel.Add(production);
                    }
                }

                model = tempModel;
            }

            if (filterWarehouseLocation != null && filterWarehouseLocation != 0)
            {
                var tempModel = new List<ProductionLot>();
                foreach (var production in model)
                {
                    var details = production.ProductionLotDetail.Where(d => d.id_warehouseLocation == filterWarehouseLocation.Value);
                    if (details.Any())
                    {
                        tempModel.Add(production);
                    }
                }

                model = tempModel;
            }

            if (startReceptionDate != null && endReceptionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startReceptionDate.Value.Date, o.receptionDate.Date) <= 0 && DateTime.Compare(o.receptionDate.Date, endReceptionDate.Value.Date) <= 0).ToList();
            }

            if (productionLot.id_personReceiving != 0)
            {
                model = model.Where(o => o.id_personReceiving == productionLot.id_personReceiving).ToList();
            }

            if (productionLot.id_provider != 0 && productionLot.id_provider != null)
            {
                model = model.Where(o => o.id_provider == productionLot.id_provider).ToList();
            }

            if (productionLot.id_buyer != 0 && productionLot.id_buyer != null)
            {
                model = model.Where(o => o.id_buyer == productionLot.id_buyer).ToList();
            }
            if (productionLot.id_processtype !=0 && productionLot.id_processtype != null)
            {
                model = model.Where(o => o.id_processtype == productionLot.id_processtype).ToList();
            }

            if (items != null && items.Length > 0)
            {
                var tempModel = new List<ProductionLot>();
                foreach (var production in model)
                {
                    var details = production.ProductionLotDetail.Where(d => items.Contains(d.id_item));
                    if (details.Any())
                    {
                        tempModel.Add(production);
                    }
                }

                model = tempModel;
            }

            int[] idsProductionLots = model.Select(s => s.id).ToArray();

            int[] idsProductionUnit = model.GroupBy(r => r.id_productionUnit).Select(s => s.Key).ToArray();
            int[] idsProductionUnitProvider = model.Where(r => r.id_productionUnitProvider != null)
                                                        .GroupBy(r => r.id_productionUnitProvider).Select(s => (int)s.Key).ToArray();
           

            int[] idsProvider = model.Where(r => r.id_provider != null)
                                        .GroupBy(r => r.id_provider).Select(s => (int)s.Key).ToArray();
            int[] idsTypeProcess = model.Where(r => r.id_processtype != null)
                                        .GroupBy(r => r.id_processtype).Select(s => (int)s.Key).ToArray();

            List<BasicAux> _productionUnitList = db.ProductionUnit
                                                    .Where(r => idsProductionUnit.Contains(r.id))
                                                    .Select(s => new BasicAux
                                                    {
                                                        id = s.id,
                                                        description = s.name
                                                    })
                                                    .ToList();

            List<BasicAux> _productionUnitProviderList = db.ProductionUnitProvider
                                                               .Where(r => idsProductionUnitProvider.Contains(r.id))
                                                               .Select(s => new BasicAux
                                                               {
                                                                   id = s.id,
                                                                   description = s.name
                                                               })
                                                               .ToList();

           
            List<BasicAux> _providerList = db.Person
                                                .Where(r => idsProvider.Contains(r.id))
                                                .Select(s => new BasicAux
                                                {
                                                    id = s.id,
                                                    description = s.fullname_businessName
                                                })
                                                .ToList();

            List<BasicAux> _idsTypeProcess = db.ProcessType
                                            .Where(r => idsTypeProcess.Contains(r.id))
                                            .Select(s => new BasicAux
                                            {

                                                id = s.id,
                                                description = s.code
                                            }).ToList();
                                            
            var model2 = model.Select(s => new ProductionLotReceptionFilter
            {


                idProductionLot = s.id,
                numberProductionLot = s.number,
                internalNumberProductionLot = s.internalNumber,
                //internalNumberProductionLot = (s.Certification != null ? s.Certification.idLote : "") + 
                //                              (s.julianoNumber != null ? s.julianoNumber : "") +
                //                              (s.internalNumber != null ? s.internalNumber : ""),//s.internalNumber,
                receptionDateProductionLot = s.receptionDate,
                idProductionUnit = s.id_productionUnit,
                idProductionUnitProvider = (int)s.id_productionUnitProvider,
                idProvider = (int)s.id_provider,
                //idProcessType=(int)s.id_processtype,
                nameProductionLotState = s.ProductionLotState.name,
                nameCertification = s.Certification != null ? s.Certification.name : "",
                liquidationNumber = (s.sequentialLiquidation == null) ? "" : s.sequentialLiquidation.ToString(),
                id_personProcessPlant = s.id_personProcessPlant,
                personProcessPlant = s.Person1?.processPlant,
                Process = (s.ProductionProcess != null ? s.ProductionProcess.code : "")
            }).ToList();



            // JOIN LOT DETAIL
            var idsProdLotSet = new HashSet<int>(idsProductionLots);
            var dataProductDetail = db.ProductionLotDetail.Where(r => idsProdLotSet.Contains((int)r.id_productionLot)).ToList();

            var _productionLotDetailPartialList = dataProductDetail
                                                            .Select(s => new
                                                            {

                                                                idProductionLot = s.id_productionLot,
                                                                idProductionLotDetail = s.id
                                                            })
                                                            .ToList();

            int[] idsProductionLotDetails = _productionLotDetailPartialList.Select(r => r.idProductionLotDetail).ToArray();

            // JOIN Detail Lot QC
            // 2021 08 19
            // Filtrado Aplicacion Columna IsDelete
            var idSet = new HashSet<int>(idsProductionLotDetails);
            var qualityControlPartialList = db.ProductionLotDetailQualityControl
                                                .Where(r => idSet.Contains((int)r.id_productionLotDetail)
                                                            && r.IsDelete == false // -2021 08 19 Filtrado Analisis Anulado 
                                                )
                                                .ToList();

            int[] idsQualityControl = qualityControlPartialList.Select(r => r.id_qualityControl).ToArray();


            // JOIN QC CONFORM
            var idsQualitySet = new HashSet<int>(idsQualityControl);

            var dataQuality = db.QualityControl.Where(r => idsQualitySet.Contains(r.id)).ToList();

            var qualityControlisConformPartialList = dataQuality
                                                    .Select(s => new BasicAuxValida
                                                    {
                                                        id = s.id,
                                                        valida = s.isConforms
                                                    })
                                                    .ToList();


            // JOIN QC CONFORM
            // JOIN QC CONFORM
            var data = db.Document.Where(s => idsQualitySet.Contains(s.id)).ToList();


            var qualityControlConformPartialFinalList = data.Select(d => new { idQualityControl = d.id, codeDocumentState = d.DocumentState.code }).ToList();

            // 2021 08 19
            // Adicionar Consulta de Guias remision para comparar los Analisis Aprobados VS las Guias de Remision


            List<ProductionLotQualityControlExt> qualityControlStateList =
                                                                            (
                                                                                from productionLotDetail in _productionLotDetailPartialList
                                                                                join qualityControl in qualityControlPartialList on
                                                                                productionLotDetail.idProductionLotDetail equals qualityControl.id_productionLotDetail
                                                                                into ps
                                                                                from _qualityControl in ps.DefaultIfEmpty(new ProductionLotDetailQualityControl())
                                                                                join qualityControlConform in qualityControlisConformPartialList on
                                                                                //_qualityControl.id_qualityControl equals qualityControlConform.idQualityControl
                                                                                _qualityControl.id_qualityControl equals qualityControlConform.id
                                                                                into ps2
                                                                                from _qualityControlConform in ps2.DefaultIfEmpty(new BasicAuxValida())
                                                                                join qualityControlFinal in qualityControlConformPartialFinalList on
                                                                                _qualityControl.id_qualityControl equals qualityControlFinal.idQualityControl
                                                                                into ps3
                                                                                from _qualityControlFinal in ps3.DefaultIfEmpty()
                                                                                select new ProductionLotQualityControlExt
                                                                                {
                                                                                    idProductionLot = (int)productionLotDetail.idProductionLot,
                                                                                    idProductionLotDetail = productionLotDetail.idProductionLotDetail,
                                                                                    id_qualityControl = _qualityControl.id_qualityControl,
                                                                                    //isConform = _qualityControlConform.isConformsQC,
                                                                                    isConform = _qualityControlConform.valida,
                                                                                    statusDocument = (_qualityControlFinal == null) ? "00" : _qualityControlFinal.codeDocumentState,
                                                                                    isNotCompleteQuality = (_qualityControlFinal == null) ? true : false

                                                                                }).ToList();




            var dataFilter = qualityControlStateList
                                                            .Where(w => w.statusDocument != "05")
                                                            .GroupBy(s => s.idProductionLot)
                                                            .Select(r => new
                                                            {
                                                                idProductionLot = r.Key,
                                                                countDetail = r.Count(),
                                                                countDetailOk = r.Where(s => s.isConform && s.statusDocument == "03").Count(),
                                                                countIsNotComplete = r.Where(s => s.isNotCompleteQuality).Count()
                                                            }).ToList();

            var _productionLotQualityControlResumeList = dataFilter.Select(s => new
                                                            {
                                                                idPrLot = s.idProductionLot,
                                                                status = (s.countDetail != s.countDetailOk || (s.countIsNotComplete > 0)) ? "Aprobado Parcial" : "Aprobado Total"
                                                            })
                                                            .ToList();

            // Calcular qualityControl



            //List<QualityControl> _QualityControl = db.QualityControl.Where

            List<ProductionLotReceptionFilter> model3 =
                                                        (from _productionReception in model2
                                                         join _productionUnit in _productionUnitList on
                                                         _productionReception.idProductionUnit equals _productionUnit.id
                                                         join _productionUnitProvider in _productionUnitProviderList on
                                                         _productionReception.idProductionUnitProvider equals _productionUnitProvider.id
                                                         join _provider in _providerList on
                                                         _productionReception.idProvider equals _provider.id
                                                         join productionLotQualityControl in _productionLotQualityControlResumeList on
                                                         _productionReception.idProductionLot equals productionLotQualityControl.idPrLot into ps
                                                         from _productionLotQualityControl in ps.DefaultIfEmpty()
                                                         select
                                                        new ProductionLotReceptionFilter
                                                        {
                                                            idProductionLot = _productionReception.idProductionLot,
                                                            numberProductionLot = _productionReception.numberProductionLot,
                                                            internalNumberProductionLot = _productionReception.internalNumberProductionLot,
                                                            receptionDateProductionLot = _productionReception.receptionDateProductionLot,
                                                            nameProductionUnit = _productionUnit.description,

                                                            nameProductionUnitProvider = _productionUnitProvider.description,
                                                            fullnameBusinessNameProvider = _provider.description,
                                                            nameProductionLotState = _productionReception.nameProductionLotState,

                                                            idProductionUnit = _productionReception.idProductionUnit,
                                                            idProductionUnitProvider = _productionReception.idProductionUnitProvider,
                                                            stateQuality = (_productionReception.Process == "RMM") ? "" : ((_productionLotQualityControl == null) ? "No tiene Calidad" : _productionLotQualityControl.status),
                                                            //stateQuality = (_productionLotQualityControl == null) ? "No tiene Calidad" : _productionLotQualityControl.status,

                                                            liquidationNumber = _productionReception.liquidationNumber,
                                                            // ProductName = p == null ? "(No products)" : p.ProductName
                                                            nameCertification = _productionReception.nameCertification,

                                                            id_personProcessPlant = _productionReception.id_personProcessPlant,
                                                            personProcessPlant = _productionReception.personProcessPlant

                                                        }).ToList();



            TempData["idsProductionLots"] = model3.Select(r => r.idProductionLot).ToArray();
            //TempData.Keep("model");
            TempData.Keep("idsProductionLots");
            return PartialView("_ProductionLotReceptionResultsPartial", model3.OrderByDescending(o => o.idProductionLot).ToList());
        }

        [HttpPost]
        public ActionResult GetResultsAdvancedFilter()
        {
            List<int> listIds = (TempData["listIds"] as List<int>);
            listIds = listIds ?? new List<int>();

            List<ProductionLot> model = db.ProductionLot.AsEnumerable().Where(w => w.ProductionProcess.code == "REC" && listIds.Contains(w.id)).ToList();
            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_ProductionLotReceptionResultsPartial", model.OrderByDescending(o => o.id).ToList());
            //}


            //return null;

        }

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionResultsOrdersPartial()
        {
            var model = db.PurchaseOrder.OrderByDescending(o => o.id); ;

            return PartialView("_ProductionLotReceptionDetailPartial");
        }

        #endregion

        #region PRODCUTION LOTS

        [HttpPost]
        public ActionResult ProductionLotReceptionsPartial()
        {


            int[] idsProductionLots = (int[])TempData["idsProductionLots"];

            List<ProductionLot> model = db.ProductionLot
                                            .Where(p => idsProductionLots.Contains(p.id)).ToList();



            int[] idsProductionUnit = model.GroupBy(r => r.id_productionUnit).Select(s => s.Key).ToArray();
            int[] idsProductionUnitProvider = model.Where(r => r.id_productionUnitProvider != null)
                                                        .GroupBy(r => r.id_productionUnitProvider).Select(s => (int)s.Key).ToArray();

            int[] idsProvider = model.Where(r => r.id_provider != null)
                                        .GroupBy(r => r.id_provider).Select(s => (int)s.Key).ToArray();


            List<BasicAux> _productionUnitList = db.ProductionUnit
                                                    .Where(r => idsProductionUnit.Contains(r.id))
                                                    .Select(s => new BasicAux
                                                    {
                                                        id = s.id,
                                                        description = s.name
                                                    })
                                                    .ToList();

            List<BasicAux> _productionUnitProviderList = db.ProductionUnitProvider
                                                               .Where(r => idsProductionUnitProvider.Contains(r.id))
                                                               .Select(s => new BasicAux
                                                               {
                                                                   id = s.id,
                                                                   description = s.name
                                                               })
                                                               .ToList();


            List<BasicAux> _providerList = db.Person
                                                .Where(r => idsProvider.Contains(r.id))
                                                .Select(s => new BasicAux
                                                {
                                                    id = s.id,
                                                    description = s.fullname_businessName
                                                })
                                                .ToList();


            var model2 = model.Select(s => new ProductionLotReceptionFilter
            {


                idProductionLot = s.id,
                numberProductionLot = s.number,
                internalNumberProductionLot = s.internalNumber,
                //internalNumberProductionLot = (s.Certification != null ? s.Certification.idLote : "") + 
                //                              (s.julianoNumber != null ? s.julianoNumber : "") +
                //                              (s.internalNumber != null ? s.internalNumber : ""),//s.internalNumber,
                receptionDateProductionLot = s.receptionDate,
                idProductionUnit = s.id_productionUnit,
                idProductionUnitProvider = (int)s.id_productionUnitProvider,
                idProvider = (int)s.id_provider,
                nameProductionLotState = s.ProductionLotState.name,
                nameCertification = (s.Certification != null) ? s.Certification.name : "",
                id_personProcessPlant = s.id_personProcessPlant,
                personProcessPlant = s.Person1.processPlant,
                liquidationNumber = (s.sequentialLiquidation == null) ? "" : s.sequentialLiquidation.ToString(),
                Process = (s.ProductionProcess != null ? s.ProductionProcess.code : "")

            }).ToList();



            // JOIN LOT DETAIL
            var _productionLotDetailPartialList = db.ProductionLotDetail
                                                            .Where(r => idsProductionLots.Contains((int)r.id_productionLot))
                                                            .Select(s => new
                                                            {

                                                                idProductionLot = s.id_productionLot,
                                                                idProductionLotDetail = s.id
                                                            })
                                                            .ToList();

            int[] idsProductionLotDetails = _productionLotDetailPartialList.Select(r => r.idProductionLotDetail).ToArray();

            // JOIN Detail Lot QC
            var qualityControlPartialList = db.ProductionLotDetailQualityControl
                                                .Where(r => idsProductionLotDetails.Contains((int)r.id_productionLotDetail)
                                                            && r.IsDelete == false
                                                )
                                                .ToList();

            int[] idsQualityControl = qualityControlPartialList.Select(r => r.id_qualityControl).ToArray();


            // JOIN QC CONFORM
            var qualityControlisConformPartialList = db.QualityControl
                                                    .Where(r => idsQualityControl.Contains(r.id))
                                                    .Select(s => new BasicAuxValida
                                                    {
                                                        id = s.id,
                                                        valida = s.isConforms
                                                        //idQualityControl = s.id,
                                                        //isConformsQC = s.isConforms
                                                    })
                                                    .ToList();


            // JOIN QC CONFORM
            var dataQuality = db.Document.Where(r => idsQualityControl.Contains(r.id)).ToList();
            var qualityControlConformPartialFinalList = dataQuality.Select(s => new
                                                        {
                                                            idQualityControl = s.id,
                                                            codeDocumentState = s.DocumentState.code

                                                        })
                                                        .ToList();

            List<ProductionLotQualityControlExt> qualityControlStateList =
                                                                            (
                                                                                from productionLotDetail in _productionLotDetailPartialList
                                                                                join qualityControl in qualityControlPartialList on
                                                                                productionLotDetail.idProductionLotDetail equals qualityControl.id_productionLotDetail
                                                                                into ps
                                                                                from _qualityControl in ps.DefaultIfEmpty(new ProductionLotDetailQualityControl())

                                                                                join qualityControlConform in qualityControlisConformPartialList on
                                                                                _qualityControl.id_qualityControl equals qualityControlConform.id
                                                                                into ps2
                                                                                from _qualityControlConform in ps2.DefaultIfEmpty(new BasicAuxValida())

                                                                                join qualityControlFinal in qualityControlConformPartialFinalList on
                                                                                _qualityControl.id_qualityControl equals qualityControlFinal.idQualityControl
                                                                                into ps3
                                                                                from _qualityControlFinal in ps3.DefaultIfEmpty()
                                                                                select new ProductionLotQualityControlExt
                                                                                {
                                                                                    idProductionLot = (int)productionLotDetail.idProductionLot,
                                                                                    idProductionLotDetail = productionLotDetail.idProductionLotDetail,
                                                                                    id_qualityControl = _qualityControl.id_qualityControl,
                                                                                    isConform = _qualityControlConform.valida,
                                                                                    statusDocument = (_qualityControlFinal == null) ? "00" : _qualityControlFinal.codeDocumentState,
                                                                                    isNotCompleteQuality = (_qualityControlFinal == null) ? true : false
                                                                                }
                                                                            ).ToList();



            var _productionLotQualityControlResumeList = qualityControlStateList
                                                                        .Where(w => w.statusDocument != "05")
                                                                        .GroupBy(s => s.idProductionLot)
                                                                        .Select(r => new
                                                                        {
                                                                            idProductionLot = r.Key,
                                                                            countDetail = r.Count(),
                                                                            countDetailOk = r.Where(s => s.isConform && s.statusDocument == "03").Count(),
                                                                            countIsNotComplete = r.Where(s => s.isNotCompleteQuality).Count()
                                                                        })
                                                                        .Select(s => new
                                                                        {
                                                                            idPrLot = s.idProductionLot,
                                                                            status = (s.countDetail != s.countDetailOk || (s.countIsNotComplete > 0)) ? "Aprobado Parcial" : "Aprobado Total"
                                                                        })
                                                                        .ToList();

            // Calcular qualityControl

            //List<QualityControl> _QualityControl = db.QualityControl.Where

            List<ProductionLotReceptionFilter> model3 =
                                                        (from _productionReception in model2
                                                         join _productionUnit in _productionUnitList on
                                                         _productionReception.idProductionUnit equals _productionUnit.id
                                                         join _productionUnitProvider in _productionUnitProviderList on
                                                         _productionReception.idProductionUnitProvider equals _productionUnitProvider.id
                                                         join _provider in _providerList on
                                                         _productionReception.idProvider equals _provider.id
                                                         join _productionLotQualityControl in _productionLotQualityControlResumeList on
                                                         _productionReception.idProductionLot equals _productionLotQualityControl.idPrLot into ps
                                                         from _productionLotQualityControl in ps.DefaultIfEmpty()
                                                         select
                                                        new ProductionLotReceptionFilter
                                                        {
                                                            idProductionLot = _productionReception.idProductionLot,
                                                            numberProductionLot = _productionReception.numberProductionLot,
                                                            internalNumberProductionLot = _productionReception.internalNumberProductionLot,
                                                            receptionDateProductionLot = _productionReception.receptionDateProductionLot,
                                                            nameProductionUnit = _productionUnit.description,

                                                            nameProductionUnitProvider = _productionUnitProvider.description,
                                                            fullnameBusinessNameProvider = _provider.description,
                                                            nameProductionLotState = _productionReception.nameProductionLotState,
                                                            nameCertification = _productionReception.nameCertification,

                                                            idProductionUnit = _productionReception.idProductionUnit,
                                                            idProductionUnitProvider = _productionReception.idProductionUnitProvider,
                                                            stateQuality = (_productionReception.Process == "RMM") ? "" : ((_productionLotQualityControl == null) ? "No tiene Calidad" : _productionLotQualityControl.status),
                                                            //stateQuality = (_productionLotQualityControl == null) ? "No tiene Calidad" : _productionLotQualityControl.status,
                                                            id_personProcessPlant = _productionReception.id_personProcessPlant,
                                                            personProcessPlant = _productionReception.personProcessPlant,
                                                            liquidationNumber = _productionReception.liquidationNumber
                                                            // ProductName = p == null ? "(No products)" : p.ProductName

                                                        }).ToList();


            TempData.Keep("idsProductionLots");
            return PartialView("_ProductionLotReceptionsPartial", model3.OrderByDescending(o => o.idProductionLot).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionsAddNew(bool approve, bool? loteManual, ProductionLot item)
        {
             bool isSaved = false;

            bool? copack = null;

            if (item.isCopackingLot == null)
                copack = false;
            else
                copack = item.isCopackingLot;

            this.ViewBag.isCopacking = copack;

            if (TempData["ProductionUnitProviderByProvider"] != null)
            {
                TempData.Keep("ProductionUnitProviderByProvider");
            }
            string settLE = DataProviderSetting.ValueSetting("ULERMP");

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.receptionDate = item.receptionDate;
            productionLot.internalNumber = productionLot.internalNumber ?? item.internalNumber;
            productionLot.id_productionUnit = item.id_productionUnit;
            productionLot.description = item.description;
            productionLot.reference = item.reference;
            productionLot.id_productionUnitProviderPool = item.id_productionUnitProviderPool;
            productionLot.expirationDate = item.expirationDate;
            productionLot.totalQuantityOrdered = item.totalQuantityOrdered;
            productionLot.totalQuantityRemitted = item.totalQuantityRemitted;
            productionLot.totalQuantityRecived = item.totalQuantityRecived;
            productionLot.totalQuantityLiquidation = item.totalQuantityLiquidation;
            productionLot.wholeSubtotal = item.wholeSubtotal;
            productionLot.subtotalTail = item.subtotalTail;
            productionLot.totalQuantityTrash = item.totalQuantityTrash;
            productionLot.wholeGarbagePounds = item.wholeGarbagePounds;
            productionLot.poundsGarbageTail = item.poundsGarbageTail;
            productionLot.totalQuantityLiquidationAdjust = item.totalQuantityLiquidationAdjust;
            productionLot.wholeLeftover = item.wholeLeftover;

            productionLot.totalAdjustmentPounds = item.totalAdjustmentPounds;
            productionLot.totalAdjustmentWholePounds = item.totalAdjustmentWholePounds;
            productionLot.totalAdjustmentTailPounds = item.totalAdjustmentTailPounds;
            productionLot.wholeSubtotalAdjust = item.wholeSubtotalAdjust;
            productionLot.subtotalTailAdjust = item.subtotalTailAdjust;
            productionLot.wholeSubtotalAdjustProcess = item.wholeSubtotalAdjustProcess;
            productionLot.wholeTotalToPay = item.wholeTotalToPay;
            productionLot.tailTotalToPay = item.tailTotalToPay;
            productionLot.totalToPay = item.totalToPay;
            productionLot.totalToPayEq = item.totalToPayEq;
            productionLot.liquidationDate = item.liquidationDate;
            productionLot.closeDate = item.closeDate;
            productionLot.liquidationPaymentDate = item.liquidationPaymentDate;
            productionLot.id_providerapparent = item.id_providerapparent;
            productionLot.isCopackingLot = item.isCopackingLot;
            productionLot.id_CopackingTariff = item.id_CopackingTariff;
            ProductionUnitProvider pupTmp = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == item.id_productionUnitProvider);

            pupTmp = pupTmp ?? new ProductionUnitProvider();

            productionLot.tramitNumberPL = pupTmp.tramitNumber ?? "";
            productionLot.INPnumberPL = pupTmp.INPnumber ?? "";
            productionLot.ministerialAgreementPL = pupTmp.ministerialAgreement ?? "";

            Lot lot = new Lot();
            var strJul = productionLot.julianoNumber ?? item.julianoNumber;
            var strInt = productionLot.internalNumber ?? item.internalNumber;

            productionLot.id_certification = item.id_certification;
            var aCertification = db.Certification.FirstOrDefault(fod => fod.id == item.id_certification);
            productionLot.internalNumberConcatenated = (aCertification?.idLote ?? "") + productionLot.julianoNumber + productionLot.internalNumber;

            //if (ModelState.IsValid)
            //{

            ////No está validando el No. de Lote Interno. Que debe ser único.
            //if (!String.IsNullOrEmpty(strJul + strInt))
            //{
            //    var cont = (from e in db.ProductionLot
            //                where productionLot.id != e.id && e.internalNumber == (strJul + strInt) 
            //                && e.ProductionLotState.code != "09"
            //                select e.internalNumber).Count();

            //    if (cont > 0)
            //    {
            //        TempData.Keep("productionLotReception");
            //        ViewData["EditMessage"] = ErrorMessage("El Numero de Lote Interno ya esta en Uso...");

            //        return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
            //    }
            //}

            // No se debe permitir realizar recepción en un mismo lote de productos diferentes.

            if (productionLot.ProductionLotDetail != null)
            {
                var contdis = (from e in productionLot.ProductionLotDetail

                               select e.id_item).Distinct().Count();

                if (contdis > 1)
                {
                    TempData.Keep("productionLotReception");
                    ViewData["EditMessage"] = ErrorMessage("No puede haber dos productos diferentes...");

                    return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                }
            }

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    #region Lot

                    lot = ServiceLot.CreateLot(db, ActiveUser, ActiveCompany, ActiveEmissionPoint);
                    item.id = lot.id;
                    item.Lot = lot;
                    item.Lot.Document.emissionDate = item.receptionDate;

                    #endregion

                    #region ProductionLot

                    item.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.id == productionLot.id_ProductionLotState);
                    productionLot.ProductionLotState = item.ProductionLotState;
                    //item.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.id == productionLot.id_ProductionLotState);

                    //ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.id == item.id_productionProcess);
                    ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.id == productionLot.id_productionProcess);

                    //item.ProductionProcess = db.ProductionProcess.FirstOrDefault(p => p.id == item.id_productionProcess);
                    item.ProductionProcess = process;
                    productionLot.ProductionProcess = process;

                    item.ProductionUnit = db.ProductionUnit.FirstOrDefault(u => u.id == item.id_productionUnit);
                    productionLot.ProductionUnit = item.ProductionUnit;
                    //item.Item = db.Item.FirstOrDefault(i => i.id == item.id_itemResulting);
                    //item.id_metricUnitResulting = item.Item?.ItemPurchaseInformation?.MetricUnit?.id ?? 0;
                    //item.MetricUnit = db.MetricUnit.FirstOrDefault(m => m.id == item.id_metricUnitResulting);
                    item.withPrice = productionLot.withPrice;
                    item.id_buyer = (item.id_buyer ?? productionLot.id_buyer);
                    productionLot.id_buyer = item.id_buyer;
                    item.Person = db.Person.FirstOrDefault(p => p.id == item.id_buyer);
                    productionLot.Person = item.Person;
                    item.id_provider = (item.id_provider ?? productionLot.id_provider);
                    productionLot.id_provider = item.id_provider;
                    item.Provider = db.Provider.FirstOrDefault(p => p.id == item.id_provider);
                    productionLot.Provider = item.Provider;
                    item.Employee = db.Employee.FirstOrDefault(p => p.id == item.id_personReceiving);
                    item.Employee1 = item.Employee;
                    item.Person1 = db.Person.FirstOrDefault(p => p.id == productionLot.id_personProcessPlant);
                    productionLot.Person1 = item.Person1;


                    item.id_company = this.ActiveCompanyId;
                    item.id_userCreate = ActiveUser.id;
                    item.dateCreate = DateTime.Now;
                    item.id_userUpdate = ActiveUser.id;
                    item.dateUpdate = DateTime.Now;

                    item.tramitNumberPL = productionLot.tramitNumberPL;
                    item.INPnumberPL = productionLot.INPnumberPL;
                    item.ministerialAgreementPL = productionLot.ministerialAgreementPL;

                    string code = process?.code ?? "";
                    string sequential = process?.sequential.ToString("D9") ?? "";

                    item.number = code + sequential;
                    item.internalNumber = (!string.IsNullOrEmpty(item.internalNumber)) ? item.internalNumber : item.number;

                    //Actualizar el number del Lote Base
                    item.Lot.number = item.number;

                    if (process != null)
                    {
                        process.sequential++;
                        db.ProductionProcess.Attach(process);
                        db.Entry(process).State = EntityState.Modified;
                    }


                    //decimal costMP = 0;

                    #endregion

                    #region ADD ITEMS DETAILS

                    RemissionGuideDetail remissionGuideDetail = null;
                    if (loteManual == null)
                    {
                        if (productionLot.ProductionLotDetail != null)
                        {
                            item.ProductionLotDetail = new List<ProductionLotDetail>();
                            foreach (var detail in productionLot.ProductionLotDetail)
                            {
                                var productionLotDetail = new ProductionLotDetail
                                {
                                    ProductionLotDetailPurchaseDetail = new List<ProductionLotDetailPurchaseDetail>()
                                };
                                foreach (var productionLotDetailPurchaseDetail in detail.ProductionLotDetailPurchaseDetail)
                                {
                                    var pldpd = new ProductionLotDetailPurchaseDetail();

                                    pldpd.id_productionLotDetail = productionLotDetailPurchaseDetail.id_productionLotDetail;
                                    pldpd.ProductionLotDetail = db.ProductionLotDetail.FirstOrDefault(i => i.id == productionLotDetailPurchaseDetail.id_productionLotDetail);
                                    pldpd.id_purchaseOrderDetail = productionLotDetailPurchaseDetail.id_purchaseOrderDetail != null && productionLotDetailPurchaseDetail.id_purchaseOrderDetail > 0 ? productionLotDetailPurchaseDetail.id_purchaseOrderDetail : null;
                                    pldpd.PurchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(i => i.id == productionLotDetailPurchaseDetail.id_purchaseOrderDetail);
                                    pldpd.id_remissionGuideDetail = productionLotDetailPurchaseDetail.id_remissionGuideDetail != null && productionLotDetailPurchaseDetail.id_remissionGuideDetail > 0 ? productionLotDetailPurchaseDetail.id_remissionGuideDetail : null;
                                    pldpd.quanty = detail.quantityRecived;//productionLotDetailPurchaseDetail.quanty

                                    remissionGuideDetail = db.RemissionGuideDetail.FirstOrDefault(i => i.id == productionLotDetailPurchaseDetail.id_remissionGuideDetail);
                                    pldpd.RemissionGuideDetail = remissionGuideDetail;

                                    productionLotDetail.ProductionLotDetailPurchaseDetail.Add(pldpd);
                                }
                                //Quality

                                //Temporal Not Used OJO

                                productionLotDetail.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();
                                foreach (var productionLotDetailQualityControl in detail.ProductionLotDetailQualityControl)
                                {
                                    var newProductionLotDetailQualityControl = new ProductionLotDetailQualityControl
                                    {
                                        id_productionLotDetail = productionLotDetailQualityControl.id_productionLotDetail,
                                        ProductionLotDetail = productionLotDetail,
                                        id_qualityControl = productionLotDetailQualityControl.id_qualityControl,
                                        QualityControl = db.QualityControl.FirstOrDefault(i => i.id == productionLotDetailQualityControl.id_qualityControl),
                                    };
                                    productionLotDetail.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                    newProductionLotDetailQualityControl.QualityControl.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                    newProductionLotDetailQualityControl.QualityControl.id_lot = item.id;
                                    newProductionLotDetailQualityControl.QualityControl.Lot = item.Lot;
                                }

                                productionLotDetail.id_item = detail.id_item;
                                productionLotDetail.Item = db.Item.FirstOrDefault(i => i.id == productionLotDetail.id_item);
                                //productionLotDetail.id_originLot = detail.id_originLot;
                                if (detail.id_warehouse == 0)
                                {
                                    throw new Exception("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                            productionLotDetail.Item.name + ", no tiene definida una Bodega, Configúrela e intente de nuevo.");
                                    //TempData.Keep("productionLotReception");
                                    //ViewData["EditMessage"] = ErrorMessage("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                    //                                        productionLotDetail.Item.name + ", no tiene definida una Bodega, Configúrela e intente de nuevo."
                                    //    );
                                    //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                                }
                                productionLotDetail.id_warehouse = detail.id_warehouse;
                                productionLotDetail.Warehouse = db.Warehouse.FirstOrDefault(i => i.id == productionLotDetail.id_warehouse);
                                if (detail.id_warehouseLocation == 0)
                                {
                                    throw new Exception("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                            productionLotDetail.Item.name + ", no tiene definida una Ubicación, Configúrela e intente de nuevo.");
                                    //TempData.Keep("productionLotReception");
                                    //ViewData["EditMessage"] = ErrorMessage("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                    //                                        productionLotDetail.Item.name + ", no tiene definida una Ubicación, Configúrela e intente de nuevo."
                                    //    );
                                    //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                                }
                                productionLotDetail.id_warehouseLocation = detail.id_warehouseLocation;
                                productionLotDetail.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(i => i.id == productionLotDetail.id_warehouseLocation);
                                productionLotDetail.quantityOrdered = detail.quantityOrdered;
                                productionLotDetail.quantitydrained = detail.quantitydrained;

                                if (detail.quantityRemitted <= 0)
                                {
                                    throw new Exception("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                            productionLotDetail.Item.name + ", en Bodega: " +
                                                                            productionLotDetail.Warehouse.name + ", Ubicación: " +
                                                                            productionLotDetail.WarehouseLocation.name + " y Cantidad Ordenada: " +
                                                                            productionLotDetail.quantityOrdered.ToString("#,##0.00") + ". La Cantida Remitida debe ser mayor que cero.");
                                    //TempData.Keep("productionLotReception");
                                    //ViewData["EditMessage"] = ErrorMessage("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                    //                                        productionLotDetail.Item.name + ", en Bodega: " +
                                    //                                        productionLotDetail.Warehouse.name + ", Ubicación: " +
                                    //                                        productionLotDetail.WarehouseLocation.name + " y Cantidad Ordenada: " +
                                    //                                        productionLotDetail.quantityOrdered.ToString("#,##0.00") + ". La Cantida Remitida debe ser mayor que cero."
                                    //    );
                                    //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                                }
                                productionLotDetail.quantityRemitted = detail.quantityRemitted;

                                if (detail.quantityRecived <= 0)
                                {
                                    throw new Exception("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                            productionLotDetail.Item.name + ", en Bodega: " +
                                                                            productionLotDetail.Warehouse.name + ", Ubicación: " +
                                                                            productionLotDetail.WarehouseLocation.name + " y Cantidad Ordenada: " +
                                                                            productionLotDetail.quantityOrdered.ToString("#,##0.00") + ". La Cantida Recibida debe ser mayor que cero.");
                                    //TempData.Keep("productionLotReception");
                                    //ViewData["EditMessage"] = ErrorMessage("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                    //                                        productionLotDetail.Item.name + ", en Bodega: " +
                                    //                                        productionLotDetail.Warehouse.name + ", Ubicación: " +
                                    //                                        productionLotDetail.WarehouseLocation.name + " y Cantidad Ordenada: " +
                                    //                                        productionLotDetail.quantityOrdered.ToString("#,##0.00") + ". La Cantida Recibida debe ser mayor que cero."
                                    //    );
                                    //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                                }

                                productionLotDetail.quantityRecived = detail.quantityRecived;

                                if (settLE.Equals("Y"))
                                {

                                    productionLotDetail.quantitydrained = detail.quantitydrained;
                                }
                                else
                                {
                                    productionLotDetail.quantitydrained = (detail.quantitydrained != null && detail.quantitydrained > 0) ? detail.quantitydrained : detail.quantityRecived;
                                }
                                //

                                productionLotDetail.drawersNumber = detail.drawersNumber ?? 0;
                                if (productionLotDetail.drawersNumber <= 0)
                                {
                                    throw new Exception("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                            productionLotDetail.Item.name + ", en Bodega: " +
                                                                            productionLotDetail.Warehouse.name + ", Ubicación: " +
                                                                            productionLotDetail.WarehouseLocation.name + " y Cantidad Ordenada: " +
                                                                            productionLotDetail.quantityOrdered.ToString("#,##0.00") + ". El Número Kavetas debe ser mayor que cero.");
                                }
                                productionLotDetail.quantityProcessed = detail.quantityProcessed;
                                //if (item.withPrice)
                                //{
                                //    costMP += (detail.quantityRecived * (detail.ProductionLotDetailPurchaseDetail?.FirstOrDefault()?.PurchaseOrderDetail.price ?? 0));
                                //}
                                item.ProductionLotDetail.Add(productionLotDetail);
                            }
                        }

                        if (item.ProductionLotDetail.Count == 0)
                        {
                            throw new Exception("No se puede guardar un lote sin detalles de Materia Prima");
                            //TempData.Keep("productionLotReception");
                            //ViewData["EditMessage"] = ErrorMessage("No se puede guardar un lote sin detalles de Materia Prima");
                            //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                        }
                    }
                    #endregion

                    //Por ahora Esta región queda deshabilitada hasta ver que ocurre con la parametrización 
                    // de la recepción de Materiales de Despacho
                    #region ADD ITEMS DISPATCH MATERIAL DETAILS

                    //if (productionLot.ProductionLotDispatchMaterial != null)
                    //{
                    //    item.ProductionLotDispatchMaterial = new List<ProductionLotDispatchMaterial>();
                    //    foreach (var detail in productionLot.ProductionLotDispatchMaterial)
                    //    {
                    //        var productionLotDispatchMaterial = new ProductionLotDispatchMaterial();
                    //        productionLotDispatchMaterial.ProductionLotDispatchMaterialRemissionDetail = new List<ProductionLotDispatchMaterialRemissionDetail>();
                    //        foreach (var productionLotDispatchMaterialRemissionDetail in detail.ProductionLotDispatchMaterialRemissionDetail)
                    //        {
                    //            productionLotDispatchMaterial.ProductionLotDispatchMaterialRemissionDetail.Add(new ProductionLotDispatchMaterialRemissionDetail
                    //            {
                    //                id_remissionGuideDispatchMaterial = productionLotDispatchMaterialRemissionDetail.id_remissionGuideDispatchMaterial,
                    //                RemissionGuideDispatchMaterial = db.RemissionGuideDispatchMaterial.FirstOrDefault(i => i.id == productionLotDispatchMaterialRemissionDetail.id_remissionGuideDispatchMaterial)
                    //            });
                    //        }

                    //        productionLotDispatchMaterial.id_item = detail.id_item;
                    //        productionLotDispatchMaterial.Item = db.Item.FirstOrDefault(i => i.id == productionLotDispatchMaterial.id_item);

                    //        if ((detail.id_warehouse == 0 || detail.id_warehouse == null) && detail.arrivalDestinationQuantity > 0)
                    //        {
                    //            throw new Exception("No se puede guardar el lote. Debido a que el detalle de Materiales de Despacho, el Ítem: " +
                    //                                                    productionLotDispatchMaterial.Item.name + ", no tiene definida una Bodega en donde ingresar el Inventario, Configúrela e intente de nuevo.");

                    //        }
                    //        productionLotDispatchMaterial.id_warehouse = detail.id_warehouse;
                    //        productionLotDispatchMaterial.Warehouse = db.Warehouse.FirstOrDefault(w => w.id == productionLotDispatchMaterial.id_warehouse);

                    //        if ((detail.id_warehouseLocation == 0 || detail.id_warehouseLocation == null) && detail.arrivalDestinationQuantity > 0)
                    //        {
                    //            throw new Exception("No se puede guardar el lote. Debido a que el detalle de Materiales de Despacho, el Ítem: " +
                    //                                                    productionLotDispatchMaterial.Item.name + ", no tiene definida la Ubicación en donde ingresar el Inventario, Configúrela e intente de nuevo.");

                    //        }
                    //        productionLotDispatchMaterial.id_warehouseLocation = detail.id_warehouseLocation;
                    //        productionLotDispatchMaterial.WarehouseLocation = db.WarehouseLocation.FirstOrDefault(w => w.id == productionLotDispatchMaterial.id_warehouseLocation);

                    //        productionLotDispatchMaterial.sourceExitQuantity = detail.sourceExitQuantity;
                    //        productionLotDispatchMaterial.sendedDestinationQuantity = detail.sendedDestinationQuantity;
                    //        productionLotDispatchMaterial.arrivalDestinationQuantity = detail.arrivalDestinationQuantity;
                    //        productionLotDispatchMaterial.arrivalGoodCondition = detail.arrivalGoodCondition;
                    //        productionLotDispatchMaterial.arrivalBadCondition = detail.arrivalBadCondition;

                    //        item.ProductionLotDispatchMaterial.Add(productionLotDispatchMaterial);
                    //    }
                    //}

                    #endregion

                    #region ADD QUALITY ANALYSIS DETAILS

                    //if (productionLot.ProductionLotQualityAnalysis != null)
                    //{
                    //    item.ProductionLotQualityAnalysis = new List<ProductionLotQualityAnalysis>();

                    //    foreach (var detail in productionLot.ProductionLotQualityAnalysis)
                    //    {
                    //        var productionLotQualityAnalysis = new ProductionLotQualityAnalysis();
                    //        productionLotQualityAnalysis.id_qualityAnalysis = detail.id_qualityAnalysis;
                    //        productionLotQualityAnalysis.QualityAnalysis = db.QualityAnalysis.FirstOrDefault(i => i.id == productionLotQualityAnalysis.id_qualityAnalysis);
                    //        productionLotQualityAnalysis.result = detail.result;

                    //        item.ProductionLotQualityAnalysis.Add(productionLotQualityAnalysis);
                    //    }
                    //}

                    #endregion


                    UpdateProductionLotTotals(item);

                    item.internalNumber = item.internalNumberConcatenated;// strJul + strInt;
                    if (loteManual == null)
                    {
                        item.id_personProcessPlant = remissionGuideDetail.RemissionGuide.id_personProcessPlant;
                        this.ActualizarRegistroRomaneo(item);
                    }


                    if (item.id == 0)
                    {
                        db.ProductionLot.Add(item);
                    }
                    else
                    {
                        db.ProductionLot.Attach(item);
                        db.Entry(item).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    if (approve)
                    {
                        int? aId_processType = 0;
                        foreach (var detail in item.ProductionLotDetail)
                        {
                            var aQualityControl = detail.ProductionLotDetailQualityControl
                                                                    .FirstOrDefault(fod => fod.IsDelete == false
                                                                                           && fod.QualityControl.Document.DocumentState.code.Equals("03"))?.QualityControl;//"03": Aprobado
                            if (aQualityControl == null)
                            {
                                throw new Exception("No se puede aprobar el lote por tener detalles de Materia Prima sin Calidad APROBADA(Conforme)");
                            }

                            if (!(aQualityControl?.isConforms ?? false))
                            {
                                throw new Exception("No se puede aprobar el lote por tener detalles de Materia Prima con Calidad no APROBADA(Conforme)");
                                //TempData.Keep("productionLotReception");
                                //ViewData["EditMessage"] = ErrorMessage("No se puede guardar un lote sin detalles de Materia Prima");
                                //return PartialView("_ProductionLotReceptionEditFormPartial", modelItem);
                            }

                            if (aId_processType == 0)
                            {
                                aId_processType = aQualityControl?.id_processType;
                            }
                            if (aQualityControl?.id_processType != aId_processType)
                            {
                                throw new Exception("No se puede aprobar el lote por tener detalles de Materia Prima con Calidad APROBADA(Conforme) y Tipo de Proceso diferente");
                            }

                            foreach (var productionLotDetailPurchaseDetail in detail.ProductionLotDetailPurchaseDetail)
                            {
                                ServicePurchaseRemission.UpdateQuantityRecived(db, productionLotDetailPurchaseDetail.id_purchaseOrderDetail,
                                                                               productionLotDetailPurchaseDetail.id_remissionGuideDetail,
                                                                               productionLotDetailPurchaseDetail.quanty);
                            }
                        }

                        //AQUI SE COMENTA 11102018
                        //ServiceInventoryMove.UpdateInventaryMoveEntryMateriaPrimaReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);

                        //Enviar notificacion a bodegueros para que conozcan el Ingreso de Materiales de Despacho
                        //if (item.ProductionLotDispatchMaterial.Count > 0 && item.ProductionLotDispatchMaterial.Sum(s => s.arrivalDestinationQuantity) > 0)
                        //{
                        //    //heeey
                        //    ServiceInventoryMove.UpdateInventaryMoveTransferDispatchMaterialsReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                        //    //ServiceInventoryMove.UpdateInventaryMoveEntryDispatchMaterial(ActiveUser, ActiveCompany, ActiveEmissionPoint, item, db, false);
                        //}

                        item.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "02"); //RECEPCIONADO



                        //Actualizando Estado del Documento Lote Base
                        var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        item.Lot.Document.id_documentState = documentState.id;
                        item.Lot.Document.DocumentState = documentState;

                        db.SaveChanges();
                    }


                    trans.Commit();
                    isSaved = true;
                    TempData["productionLotReception"] = item;
                    TempData.Keep("productionLotReception");

                    ViewData["EditMessage"] = SuccessMessage("Lote: " + item.number + " guardado exitosamente");
                }
                catch (Exception e)
                {
                    TempData["productionLotReception"] = productionLot;
                    TempData.Keep("productionLotReception");
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    productionLot.internalNumber = strInt;
                    productionLot.julianoNumber = strJul;
                    bool _loteManualEdit = false;
                    if (productionLot != null)
                    {
                        _loteManualEdit = productionLot.ProductionProcess.code == "RMM";
                    }
                    if (!_loteManualEdit)
                    {
                        return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                    }
                    else
                    {
                        return PartialView("_ProductionLotManualReceptionEditFormPartial", productionLot);
                    }

                }
            }
            //}
            //else
            //{
            //	ViewData["EditError"] = "Por Favor, corrija todos los errores.";
            //}

            item.julianoNumber = strJul;
            item.internalNumber = strInt;
            if (isSaved)
            {
                if (item.ProductionLotState.code == "01")
                {
                    GenerateResultForDrainingTest(item.id);
                }
            }
            bool loteManualEdit = false;
            if (item != null)
            {
                loteManualEdit = item.ProductionProcess.code == "RMM";
            }
            if (!loteManualEdit)
            {
                return PartialView("_ProductionLotReceptionEditFormPartial", item);
            }
            else
            {
                return PartialView("_ProductionLotManualReceptionEditFormPartial", item);
            }

        }

        


        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionsUpdate(bool approve, ProductionLot item,
                                                          decimal? wholeGarbagePoundsLiq, decimal? poundsGarbageTailLiq,
                                                          decimal? wholeLeftoverLiq, decimal? tailLeftOverLiq)
        {
            bool isSaved = false;
            bool? copack = null;            
            string seccion = null;
            string data = null;
            string settLE = null;
            string settCxC = null;
            bool isValidate = false;
            ProductionLot productionLot = null;
            ProductionLot modelItem = null;
            decimal costMP = 0;
            bool loteManualEdit = false;
            DocumentState[] documentStates = Array.Empty<DocumentState>();
            
            Item[] itemDetails =Array.Empty<Item>();
            Warehouse[] warehouseDetails = Array.Empty<Warehouse>();
            WarehouseLocation[] warehouseLocationDetails = Array.Empty<WarehouseLocation>();
            MetricUnitConversion[] metricUnitConversions = Array.Empty<MetricUnitConversion>();
            ProductionLotState[] productionLotStates = Array.Empty<ProductionLotState>();
            Setting aSettingANL = null;
            Item[] itemDetailsLiq = Array.Empty<Item>();
            Warehouse[] warehouseDetailsLiq = Array.Empty<Warehouse>();
            WarehouseLocation[] warehouseLocationDetailsLiq = Array.Empty<WarehouseLocation>();
            SalesOrder[] salesOrderSalesOrdersLiq = Array.Empty<SalesOrder>();
            SalesOrderDetail[] salesOrderDetailsLiq = Array.Empty<SalesOrderDetail>();
            MetricUnit[] metricUnitsLiq = Array.Empty<MetricUnit>();

            Item[] itemDetailsPaq = Array.Empty<Item>();
            MetricUnit[] metricUnitsTrash = Array.Empty<MetricUnit>();

            Item[] itemDetailsTrash = Array.Empty<Item>();
            var messageWarning = "";
            string settPDESCABEZADO = null;

            AdvanceProvider _apTmp = null;

            string strJul = null;
            string strInt = null;
            Certification aCertification = null;

            Item[] itemProductionLotPackingMaterials = Array.Empty<Item>();
            PriceList pricelist = null;
            ProcessType[] processTypes = Array.Empty<ProcessType>();
            string statePriceList = null;
            string nameView = "";
            int[] idReceptionDetailIds = Array.Empty<int>();
            ReceptionDetailDrainingTest[] receptionDetailDrainingTestList = Array.Empty<ReceptionDetailDrainingTest>();
            int[] qualityControlIds = Array.Empty<int>();
            QualityControl[] QualityControls = Array.Empty<QualityControl>();
            int id_productionLotDetailAuxTemp = 0;
            ProductionLotDetailQualityControl productionLotDetailQualityControl = null;
            QualityControl qualityControlDetailParam = null;
            Setting[] settings = Array.Empty<Setting>();
            MetricUnit[] metricUnits = Array.Empty<MetricUnit>();

            Item[] itemDetailPackingMaterialDetails = Array.Empty<Item>();
            Item[] itemProductionLotPayments = Array.Empty<Item>();
            Item[] itemEquivalences = Array.Empty<Item>();

            RemissionGuideDetail[] remissionGuideDetails = Array.Empty<RemissionGuideDetail>();
            PurchaseOrderDetail[] purchaseOrderDetails = Array.Empty<PurchaseOrderDetail>();
            string productionUnitProviderName = null;
            string productionUnitProviderPoolName = null;

            int[] inventoryMoveDetailIdsForDelete = Array.Empty<int>();

            EntityObjectPermissions entityObjectPermissions = null;

            try
            {
                #region Preparacion Data Preliminar | 0
                if (item.isCopackingLot == null)
                    copack = false;
                else
                    copack = item.isCopackingLot;

                this.ViewBag.isCopacking = copack;
                settCxC = db.Setting.FirstOrDefault(fod => fod.code == "HLCXC")?.value ?? "0";
                if (TempData["ProductionUnitProviderByProvider"] != null)
                {
                    TempData.Keep("ProductionUnitProviderByProvider");
                }
                settLE = DataProviderSetting.ValueSetting("ULERMP");
                documentStates = db.DocumentState.ToArray();
                aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");                
                settPDESCABEZADO = db.Setting.FirstOrDefault(fod => fod.code == "PDESCABEZADO")?.value ?? "";
                settings = db.Setting.ToArray();
                metricUnits = db.MetricUnit.ToArray();
                processTypes = db.ProcessType.ToArray();
                #endregion

                #region Get ProductionLot Cache
                seccion = "GetProductionLotCache";
                LogInfo(seccion, DateTime.Now);
                productionLot = (TempData["productionLotReception"] as ProductionLot);
                productionLot = productionLot ?? new ProductionLot();
                productionLot.internalNumber = item.internalNumber;
                productionLot.receptionDate = item.receptionDate;
                productionLot.id_productionUnit = item.id_productionUnit;
                productionLot.description = item.description;
                productionLot.reference = item.reference;
                productionLot.expirationDate = item.expirationDate;
                productionLot.totalQuantityOrdered = item.totalQuantityOrdered;
                productionLot.totalQuantityRemitted = item.totalQuantityRemitted;
                productionLot.totalQuantityRecived = item.totalQuantityRecived;

                productionLot.totalQuantityLiquidation = item.totalQuantityLiquidation;
                productionLot.wholeSubtotal = item.wholeSubtotal;
                productionLot.subtotalTail = item.subtotalTail;
                productionLot.totalQuantityTrash = item.totalQuantityTrash;
                productionLot.wholeGarbagePounds = item.wholeGarbagePounds;
                productionLot.poundsGarbageTail = item.poundsGarbageTail;
                productionLot.totalQuantityLiquidationAdjust = item.totalQuantityLiquidationAdjust;
                productionLot.wholeLeftover = item.wholeLeftover;
                productionLot.totalAdjustmentPounds = item.totalAdjustmentPounds;
                productionLot.totalAdjustmentWholePounds = item.totalAdjustmentWholePounds;
                productionLot.totalAdjustmentTailPounds = item.totalAdjustmentTailPounds;
                productionLot.wholeSubtotalAdjust = item.wholeSubtotalAdjust;
                productionLot.subtotalTailAdjust = item.subtotalTailAdjust;
                productionLot.wholeSubtotalAdjustProcess = item.wholeSubtotalAdjustProcess;
                productionLot.wholeTotalToPay = item.wholeTotalToPay;
                productionLot.tailTotalToPay = item.tailTotalToPay;
                productionLot.totalToPay = item.totalToPay;
                productionLot.totalToPayEq = item.totalToPayEq;
                productionLot.tailLeftOver = item.tailLeftOver;

                productionLot.liquidationDate = item.liquidationDate;
                productionLot.closeDate = item.closeDate;
                productionLot.liquidationPaymentDate = item.liquidationPaymentDate;
                productionLot.id_providerapparent = item.id_providerapparent;
                productionLot.isCopackingLot = item.isCopackingLot;
                productionLot.id_CopackingTariff = item.id_CopackingTariff;

                productionLot.ProductionProcess = db.ProductionProcess.FirstOrDefault(r => r.id == productionLot.id_productionProcess);
                #endregion

                #region Get Production Unit Provider
                seccion = "GetProductionUnitProvider";
                LogInfo(seccion, DateTime.Now);
                ProductionUnitProvider pupTmp = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == item.id_productionUnitProvider);
                pupTmp = pupTmp ?? new ProductionUnitProvider();

                productionLot.tramitNumberPL = pupTmp.tramitNumber ?? "";
                productionLot.INPnumberPL = pupTmp.INPnumber ?? "";
                productionLot.ministerialAgreementPL = pupTmp.ministerialAgreement ?? "";
                #endregion

                #region Get Production Lot DataBase
                seccion = "GetProductionLotDataBase";
                LogInfo(seccion, DateTime.Now);

                
                modelItem = db.ProductionLot.FirstOrDefault(p => p.id == item.id);
                modelItem.id_CopackingTariff = item.id_CopackingTariff;
                modelItem.isCopackingLot = item.isCopackingLot;
                modelItem.id_providerapparent = item.id_providerapparent;

                strJul = productionLot.julianoNumber ?? item.julianoNumber;
                strInt = productionLot.internalNumber ?? item.internalNumber;

                productionLot.id_certification = item.id_certification;
                aCertification = db.Certification.FirstOrDefault(fod => fod.id == item.id_certification);

                productionLot.internalNumberConcatenated = (aCertification?.idLote ?? "") + strJul + strInt;
                #endregion

                #region Validaciones Generales | 0
                if (modelItem == null)
                {
                    throw new ProdHandlerException("Revise los errores, no existe modelo de datos de recepción");
                }
                #endregion

                #region Validacion Recepcion lote igual Producto
                seccion = "ValidacionRecepcionloteigualProducto";
                LogInfo(seccion, DateTime.Now);

                if (productionLot.ProductionLotDetail != null)
                {
                    var contdis = (from e in productionLot.ProductionLotDetail

                                   select e.id_item).Distinct().Count();

                    if (contdis > 1)
                    {
                        TempData.Keep("productionLotReception");
                        ViewData["EditMessage"] = ErrorMessage("No puede haber dos productos diferentes...");
                        productionLot.julianoNumber = strJul;
                        productionLot.internalNumber = strInt;
                        productionLot.internalNumberConcatenated = (aCertification?.idLote ?? "") + strJul + strInt;
                        return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                    }
                }
                #endregion

                #region Get AdvanceProvider Database
                seccion = "GetAdvanceProviderDatabase";
                LogInfo(seccion, DateTime.Now);

                _apTmp = db.AdvanceProvider.FirstOrDefault(fod => fod.id_Lot == modelItem.id);
                modelItem.id_userUpdate = ActiveUser.id;
                modelItem.dateUpdate = DateTime.Now;
                modelItem.id_productionUnitProviderPool = item.id_productionUnitProviderPool;
                #endregion

                #region Get ProductionUnitProvider Datebase
                seccion = "GetProductionUnitProviderDatebase";
                LogInfo(seccion, DateTime.Now);

                ProductionUnitProvider pupTmp2 = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == item.id_productionUnitProvider);
                pupTmp2 = pupTmp2 ?? new ProductionUnitProvider();
                modelItem.tramitNumberPL = pupTmp2.tramitNumber ?? "";
                modelItem.INPnumberPL = pupTmp2.INPnumber ?? "";
                modelItem.ministerialAgreementPL = pupTmp2.ministerialAgreement ?? "";
                #endregion

                #region Obtener items | bodegas | ubicaciones |  Unit metric convertions  | ProductionLotState            

                int[] itemDetailsIds = productionLot.ProductionLotDetail.Select(r => r.id_item).ToArray();
                int[] warehouseDetailsIds= productionLot.ProductionLotDetail.Select(r => r.id_warehouse).ToArray();
                int[] warehouseLocationDetailsIds = productionLot.ProductionLotDetail.Select(r => r.id_warehouseLocation).ToArray();
                itemDetails = db.Item.Where(i => itemDetailsIds.Contains(i.id)).ToArray();
                warehouseDetails = db.Warehouse.Where(i => warehouseDetailsIds.Contains(i.id)).ToArray();
                warehouseLocationDetails = db.WarehouseLocation.Where(i => warehouseLocationDetailsIds.Contains(i.id)).ToArray();
                metricUnitConversions = db.MetricUnitConversion.ToList().ToArray();
                productionLotStates = db.ProductionLotState.ToList().ToArray();

                int[] itemDetailsLiqIds = productionLot.ProductionLotLiquidation.Select(r => r.id_item).ToArray();
                int[] warehouseDetailsLiqIds = productionLot.ProductionLotLiquidation.Select(r => r.id_warehouse).ToArray();
                int[] warehouseLocationDetailsLiqIds = productionLot.ProductionLotLiquidation.Select(r => r.id_warehouseLocation).ToArray();
                int[] salesOrderSalesOrdersLiqIds = productionLot.ProductionLotLiquidation.Where(r => r.id_salesOrder != null ).Select(r => r.id_salesOrder.Value).ToArray();
                int[] salesOrderDetailsLiqIds = productionLot.ProductionLotLiquidation.Where(r => r.id_salesOrderDetail != null).Select(r => r.id_salesOrderDetail.Value).ToArray();
                int[] metricUnitsLiqIdsmetricUnit = productionLot.ProductionLotLiquidation.Where(r => r.id_metricUnit != null).Select(r => r.id_metricUnit.Value).ToArray();
                int[] metricUnitsLiqIdsmetricUnitPresentation = productionLot.ProductionLotLiquidation.Where(r => r.id_metricUnitPresentation != null).Select(r => r.id_metricUnitPresentation.Value).ToArray();
                itemDetailsLiq = db.Item.Where(i => itemDetailsLiqIds.Contains(i.id)).ToArray();
                warehouseDetailsLiq = db.Warehouse.Where(i => warehouseDetailsLiqIds.Contains(i.id)).ToArray();
                warehouseLocationDetailsLiq = db.WarehouseLocation.Where(i => warehouseLocationDetailsLiqIds.Contains(i.id)).ToArray();
                salesOrderSalesOrdersLiq = db.SalesOrder.Where(i => salesOrderSalesOrdersLiqIds.Contains(i.id)).ToArray();
                salesOrderDetailsLiq = db.SalesOrderDetail.Where(i => salesOrderDetailsLiqIds.Contains(i.id)).ToArray(); ;
                metricUnitsLiq = metricUnits.Where(i => metricUnitsLiqIdsmetricUnit.Contains(i.id) 
                                                          || metricUnitsLiqIdsmetricUnitPresentation.Contains(i.id)).ToArray();

                int[] itemDetailsPaqIds = productionLot.ProductionLotPackingMaterial.Select(r => r.id_item).ToArray();
                itemDetailsPaq = db.Item.Where(i => itemDetailsPaqIds.Contains(i.id)).ToArray();

                int[] metricUnitsTrashIds = productionLot.ProductionLotTrash.Where(r => r.id_metricUnit != null).Select(r => r.id_metricUnit.Value).ToArray();
                metricUnitsTrash = metricUnits.Where(r => metricUnitsTrashIds.Contains(r.id)).ToArray();

                int[] itemDetailsTrashIds = productionLot.ProductionLotTrash.Select(r => r.id_item).ToArray();
                itemDetailsTrash = db.Item.Where(i => itemDetailsTrashIds.Contains(i.id)).ToArray();

                int[] itemProductionLotPaymentsIds = productionLot.ProductionLotPayment.Select(r => r.id_item).ToArray();
                itemProductionLotPayments = db.Item
                                                .Include("Presentation")
                                                .Include("ItemEquivalence")
                                                .Include("ItemType.ProcessType")                                                
                                                .Where(i => itemProductionLotPaymentsIds.Contains(i.id))
                                                .ToArray();

                int[] itemEquivalencesIds = itemProductionLotPayments                                
                                                    .Where(r=> r.ItemEquivalence?.id_itemEquivalence != null)
                                                    .Select(r => r.ItemEquivalence.id_itemEquivalence.Value)
                                                    .ToArray();

                itemEquivalences = db.Item
                                    .Include("Presentation")
                                    .Include("ItemEquivalence")
                                    .Include("ItemType.ProcessType")
                                    .Where(r => itemEquivalencesIds.Contains(r.id))
                                    .ToArray();

                if (modelItem.ProductionLotState.code == "05" || modelItem.ProductionLotState.code == "07")
                {
                    if (modelItem.id_priceList == null)
                    {
                        if (_apTmp != null && _apTmp.id_priceList != null)
                        {
                            pricelist = db.PriceList.FirstOrDefault(p => p.id == _apTmp.id_priceList);
                        }
                            
                    }
                }
                if (modelItem.ProductionLotState.code == "06")
                {
                    if (item.id_priceList == null)
                    {
                        if (_apTmp != null && _apTmp.id_priceList != null)
                        {
                            pricelist = db.PriceList.FirstOrDefault(p => p.id == _apTmp.id_priceList);
                        }
                    }
                }
                if (approve)
                {
                    if (modelItem.ProductionLotState.code == "07")
                    {
                        statePriceList = db.PriceList.FirstOrDefault(fod => fod.id == modelItem.id_priceList)?.Document?.DocumentState?.code ?? "";
                    }
                    
                }

                productionUnitProviderName = db.ProductionUnitProvider.FirstOrDefault(p => p.id == productionLot.id_productionUnitProvider)?.name; ;
                productionUnitProviderPoolName = db.ProductionUnitProviderPool.FirstOrDefault(p => p.id == productionLot.id_productionUnitProviderPool)?.name;
                
                #endregion

                #region Estados - 01
                seccion = "Estados-01-validate";
                LogInfo(seccion, DateTime.Now);
                if (modelItem.ProductionLotState.code == "01")//PENDIENTE DE RECEPCION
                {
                    modelItem.ProductionUnit = db.ProductionUnit.FirstOrDefault(u => u.id == item.id_productionUnit);
                    productionLot.ProductionUnit = modelItem.ProductionUnit;

                    var id_buyerAux = item.id_buyer ?? productionLot.id_buyer;
                    var id_providerAux = item.id_provider ?? productionLot.id_provider;
                    modelItem.id_buyer = id_buyerAux;
                    productionLot.id_buyer = id_buyerAux;
                    modelItem.Person = db.Person.FirstOrDefault(p => p.id == id_buyerAux);
                    productionLot.Person = modelItem.Person;
                    modelItem.id_provider = id_providerAux;
                    productionLot.id_provider = id_providerAux;
                    modelItem.Provider = db.Provider.FirstOrDefault(p => p.id == id_providerAux);
                    productionLot.Provider = modelItem.Provider;
                    modelItem.Employee = db.Employee.FirstOrDefault(p => p.id == item.id_personReceiving);
                    modelItem.Employee1 = modelItem.Employee;

                    modelItem.id_certification = item.id_certification;

                    modelItem.julianoNumber = strJul; //strJul + strInt;
                    modelItem.internalNumber = strInt; //strJul + strInt;
                    modelItem.internalNumberConcatenated = (aCertification?.idLote ?? "") + strJul + strInt;

                    modelItem.receptionDate = item.receptionDate;
                    modelItem.expirationDate = item.expirationDate;

                    if (productionLot != null)
                    {
                        loteManualEdit = (productionLot.ProductionProcess.code == "RMM");
                    }

                    #region UPDATE ITEMS DETAILS
                    if (!loteManualEdit)
                    {
                        if (productionLot.ProductionLotDetail != null)
                        {
                            idReceptionDetailIds = modelItem
                                                                    .ProductionLotDetail
                                                                    .Where(r => (!r.quantitydrained.HasValue
                                                                               || (r.quantitydrained.HasValue && r.quantitydrained.Value == 0))
                                                                               && r.ResultProdLotReceptionDetail != null)
                                                                    .Select(r => r.ResultProdLotReceptionDetail.idProductionLotReceptionDetail)
                                                                    .ToArray();

                            receptionDetailDrainingTestList = db.ReceptionDetailDrainingTest
                                                                         .Where(r => idReceptionDetailIds.Contains(r.idReceptionDetail))
                                                                         .ToArray();


                            #region Obtener qualityControlDetails
                            qualityControlIds = productionLot
                                                            .ProductionLotDetail?
                                                            .SelectMany(r => r?
                                                                          .ProductionLotDetailQualityControl?
                                                                          .Select(t => t.id_qualityControl)?
                                                                          .ToArray()
                                                                          )?.ToArray();


                            QualityControls = db.QualityControl
                                                           .Where(r => qualityControlIds.Contains(r.id))
                                                           .ToArray();
                            #endregion
                        }
                    }
                    #endregion

                    #region Obtener QualityControl Desde ProductionlotDetail
                    if (aSettingANL != null && aSettingANL.value == "SI")
                    {
                        id_productionLotDetailAuxTemp = db.ProductionLotDetail.Where(r => r.id_productionLot == productionLot.id).FirstOrDefault().id;
                        productionLotDetailQualityControl = db.ProductionLotDetailQualityControl.Where(r => r.id_productionLotDetail == id_productionLotDetailAuxTemp && !r.IsDelete).FirstOrDefault();
                        qualityControlDetailParam = db.QualityControl.FirstOrDefault(i => i.id == productionLotDetailQualityControl.id_qualityControl);
                    }
                    #endregion

                }
                else
                {
                    costMP = modelItem.ProductionLotDetail.Sum(s => s.quantityRecived * (s.ProductionLotDetailPurchaseDetail?.FirstOrDefault()?.PurchaseOrderDetail?.price ?? 0));
                }


                #endregion

                #region Estados - 03 
                if (modelItem.ProductionLotState.code == "03")
                {
                    if (settCxC == "0" && modelItem.ProductionLotLiquidationTotal.Count() == 0)
                    {
                        if (productionLot.ProductionLotLiquidation != null)
                        {
                            int[] itemIds = productionLot
                                                .ProductionLotLiquidation
                                                .SelectMany(r=> r.ProductionLotLiquidationPackingMaterialDetail
                                                                    .Select(t=> t.ProductionLotPackingMaterial.id_item).ToArray())
                                                .ToArray();

                            itemDetailPackingMaterialDetails = db.Item.Where(r => itemIds.Contains(r.id)).ToArray();
                        }
                    }
                }
                #endregion

                #region Aprovee 01
                entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];
                seccion = "AP-001";
                LogInfo(seccion, DateTime.Now);
                if (approve)
                {
                    seccion = "AP-002";
                    LogInfo(seccion, DateTime.Now);
                    if (modelItem.ProductionLotState.code == "01")
                    {
                        seccion = "AP-003";
                        LogInfo(seccion, DateTime.Now);
                        int[] purchaseOrderDetailsIds = modelItem
                                                            .ProductionLotDetail?
                                                            .SelectMany(r => r.ProductionLotDetailPurchaseDetail?
                                                                                .Where(t=> t.id_purchaseOrderDetail.HasValue)?                                                                                
                                                                                .Select(t => t.id_purchaseOrderDetail.Value)?
                                                                                .ToArray())
                                                            .ToArray();

                        seccion = "AP-004";
                        LogInfo(seccion, DateTime.Now);
                        purchaseOrderDetails = db.PurchaseOrderDetail
                                                                .Where(r => purchaseOrderDetailsIds.Contains(r.id))
                                                                .ToArray();
                        //RemissionGuideDetail[] remissionGuideDetails 
                        seccion = "AP-005";
                        LogInfo(seccion, DateTime.Now);
                        int[] remissionGuideDetailsIds = modelItem
                                                            .ProductionLotDetail?
                                                            .SelectMany(r => r.ProductionLotDetailPurchaseDetail?
                                                                                .Where(t => t.id_remissionGuideDetail.HasValue)?
                                                                                .Select(t => t.id_remissionGuideDetail.Value)?
                                                                                .ToArray())
                                                            .ToArray();

                        seccion = "AP-006";
                        LogInfo(seccion, DateTime.Now);
                        remissionGuideDetails = db.RemissionGuideDetail
                                                            .Where(r => remissionGuideDetailsIds.Contains(r.id))
                                                            .ToArray();


                        seccion = "AP-007";
                        LogInfo(seccion, DateTime.Now);
                    }
                }
                #endregion

                

                isValidate = true;
            }
            catch (ProdHandlerException e)
            {
                seccion = "AP-008-ERR";
                LogInfo(seccion, DateTime.Now);
                ViewData["EditError"] = ErrorMessage(e.Message);
            }
            catch (Exception e)
            {
                seccion = "AP-009-ERR";
                LogInfo(seccion, DateTime.Now);
                ViewData["EditError"] = GenericError.ErrorGeneral;
                FullLog(e, seccion: seccion);
            }
            finally
            {
                seccion = "AP-0010-END";
                LogInfo(seccion, DateTime.Now);
                TempData["productionLotReception"] = item;
                TempData.Keep("productionLotReception");
                TempData.Keep("ProductionUnitProviderByProvider");
                TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
            }

            seccion = "AP-0011";
            LogInfo(seccion, DateTime.Now);
            if (isValidate)
            {
                bool isValidateOP = false;
                try 
                {
                    #region Optimizacion Transac CTL 
                    seccion = "AP-0012";
                    LogInfo(seccion, DateTime.Now);
                    if (modelItem.ProductionLotState.code == "05")//PENDIENTE DE CIERRE
                    {
                        seccion = "AP-0013";
                        LogInfo(seccion, DateTime.Now);
                        if (settPDESCABEZADO == "SI" && modelItem.Headless.FirstOrDefault(fod => fod.Document.DocumentState.code == "03") == null)//"03": APROBADA
                        {
                            seccion = "AP-0014-ERR";
                            LogInfo(seccion, DateTime.Now);
                            throw new ProdHandlerException("No se puede Guardar, el cierre porque el Lote no tiene Proceso de descabezado Realizado o no esta aprobado. Por favor verifique el caso e inténtelo de nuevo.");
                        }
                    }

                    seccion = "AP-0015";
                    LogInfo(seccion, DateTime.Now);
                    if (modelItem.ProductionLotState.code == "05" || modelItem.ProductionLotState.code == "07")//PENDIENTE DE CIERRE Ó PENDIENTE DE PAGO
                    {
                        seccion = "AP-0016";
                        LogInfo(seccion, DateTime.Now);
                        if (modelItem.id_priceList == null)
                        {
                            if (_apTmp != null && _apTmp.id_priceList != null)
                            {
                                seccion = "AP-0017";
                                LogInfo(seccion, DateTime.Now);
                                string settingTLA = settings.FirstOrDefault(r => r.code == "TLA")?.value;   //DataProviderSetting.ValueSetting("TLA");
                                if (!string.IsNullOrEmpty(settingTLA) && settingTLA.Equals("REF"))
                                {
                                    modelItem.id_priceList = null;
                                    modelItem.PriceList = null;
                                }
                                else
                                {
                                    modelItem.id_priceList = _apTmp.id_priceList;
                                    modelItem.PriceList = pricelist;
                                }
                                seccion = "AP-0018";
                                LogInfo(seccion, DateTime.Now);
                                modelItem.liquidationDate = _apTmp.Document.emissionDate;
                            }
                        }

                        if (modelItem.ProductionLotState.code == "05")//PENDIENTE DE CIERRE
                        {
                            modelItem.closeDate = DateTime.Now;
                            modelItem.wholeGarbagePounds = item.wholeGarbagePounds;
                            modelItem.poundsGarbageTail = item.poundsGarbageTail;
                            modelItem.wholeLeftover = item.wholeLeftover;
                            modelItem.tailLeftOver = item.tailLeftOver;

                            var quantityDrainedAux = GetTotalQuantityDrainedOP(modelItem, itemDetails, metricUnitConversions, settings, metricUnits);
                            var quantityRecibedAux = modelItem.wholeSubtotalAdjust > 0 ? modelItem.wholeLeftover : quantityDrainedAux;

                            if (quantityRecibedAux != (modelItem.subtotalTailAdjust + modelItem.totalQuantityTrash + modelItem.poundsGarbageTail))
                            {
                                messageWarning = WarningMessage("Lote: " + modelItem.number + " guardado exitosamente. Pero la cantidad total Recibida para Cola es diferente a la cantidad liquidada y ajustada de Cola más desperdicio de Cola y más Basura de Cola en el Cierre.");
                            }

                            if (modelItem.id_processtype == null)
                            {
                                int? aId_processType = 0;
                                foreach (var detail in modelItem.ProductionLotDetail)
                                {
                                    var aQualityControl = detail.ProductionLotDetailQualityControl
                                                                            .FirstOrDefault(fod => fod.IsDelete == false
                                                                                                   && fod.QualityControl.Document.DocumentState.code.Equals("03"))?.QualityControl;//"03": Aprobado

                                    if (aQualityControl == null)
                                    {
                                        continue;
                                    }

                                    if (aId_processType == 0)
                                    {
                                        aId_processType = aQualityControl?.id_processType;
                                        break;
                                    }
                                }

                                var processTypeENT = processTypes.FirstOrDefault(fod => fod.code == "ENT");
                                var processTypeCOL = processTypes.FirstOrDefault(fod => fod.code == "COL");
                                if (aId_processType == null || aId_processType == 0)
                                {
                                    modelItem.ProcessType = (modelItem.wholeSubtotalAdjust > 0) ? processTypeENT : processTypeCOL;
                                    modelItem.id_processtype = modelItem.wholeSubtotalAdjust > 0 ? processTypeENT.id : processTypeCOL.id;
                                }
                                else
                                {
                                    modelItem.ProcessType = (processTypeENT.id == aId_processType) ? processTypeENT : processTypeCOL;
                                    modelItem.id_processtype = (processTypeENT.id == aId_processType) ? processTypeENT.id : processTypeCOL.id;
                                }
                            }
                        }

                        seccion = "AP-0019";
                        LogInfo(seccion, DateTime.Now);
                        if (modelItem.ProductionLotState.code == "07")//PENDIENTE DE PAGO
                        {
                            seccion = "AP-0020";
                            LogInfo(seccion, DateTime.Now);
                            if (item.id_priceList == null)
                            {
                                seccion = "AP-0021";
                                LogInfo(seccion, DateTime.Now);
                                if (_apTmp != null && _apTmp.id_priceList != null)
                                {
                                    seccion = "AP-0022";
                                    LogInfo(seccion, DateTime.Now);
                                    string settingTLA = settings.FirstOrDefault(r => r.code == "TLA")?.value;
                                    if (!string.IsNullOrEmpty(settingTLA) && settingTLA.Equals("REF"))
                                    {
                                        modelItem.id_priceList = null;
                                        modelItem.PriceList = null;
                                    }
                                    else
                                    {
                                        modelItem.id_priceList = _apTmp.id_priceList;
                                        modelItem.PriceList = pricelist;
                                    }
                                    modelItem.liquidationPaymentDate = _apTmp.Document.emissionDate;
                                }
                            }
                            else
                            {
                                modelItem.id_priceList = item.id_priceList;
                                modelItem.liquidationPaymentDate = item.liquidationDate;
                            }
                            seccion = "AP-0023";
                            LogInfo(seccion, DateTime.Now);
                            modelItem.liquidationPaymentDate = item.liquidationPaymentDate;
                        }

                        #region UPDATE PRODUCTION LOT CLOSES AND PAYMENT DETAILS

                        seccion = "AP-0024";
                        LogInfo(seccion, DateTime.Now);
                        if (productionLot.ProductionLotPayment != null)
                        {
                            seccion = "AP-0025";
                            LogInfo(seccion, DateTime.Now);
                            for (int i = modelItem.ProductionLotPayment.Count - 1; i >= 0; i--)
                            {
                                seccion = "AP-0025i-{i}";
                                LogInfo(seccion, DateTime.Now);
                                var detail = modelItem.ProductionLotPayment.ElementAt(i);

                                seccion = "AP-0026i-{i}";
                                LogInfo(seccion, DateTime.Now);
                                var listDistributed = detail.ProductionLotPaymentDistributed;

                                seccion = "AP-0027i-{i}";
                                if (listDistributed != null && listDistributed.Count > 0)
                                {
                                    seccion = "AP-0028i-{i}";
                                    for (int j = listDistributed.Count - 1; j >= 0; j--)
                                    {
                                        var detailDistributed = listDistributed.ElementAt(j);
                                        listDistributed.Remove(detailDistributed);
                                        db.Entry(detailDistributed).State = EntityState.Deleted;
                                    }
                                }
                                seccion = "AP-0026";
                                LogInfo(seccion, DateTime.Now);
                                modelItem.ProductionLotPayment.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }
                            seccion = "AP-0027";
                            LogInfo(seccion, DateTime.Now);
                            foreach (var detail in productionLot.ProductionLotPayment)
                            {
                                seccion = $"AP-0028-{detail.id_item}";
                                LogInfo(seccion, DateTime.Now);
                                var newDetail = new ProductionLotPayment
                                {
                                    id_productionLot = modelItem.id,
                                    id_item = detail.id_item,
                                    Item = db.Item
                                                .Include(r => r.ItemGeneral)
                                                .Include(r => r.ItemGeneral.ItemSize)
                                                .FirstOrDefault(r=> r.id == detail.id_item),
                                    id_metricUnit = detail.id_metricUnit,
                                    quantity = detail.quantity,
                                    adjustMore = detail.adjustMore,
                                    adjustLess = detail.adjustLess,
                                    totalMU = detail.totalMU,
                                    price = detail.price,
                                    priceEdition = detail.priceEdition,
                                    totalToPay = detail.totalToPay,
                                    totalToPayEq = detail.totalToPayEq,
                                    totalPounds = detail.totalPounds,
                                    fitPounds = detail.fitPounds,
                                    totalProcessMetricUnit = detail.totalProcessMetricUnit,
                                    totalProcessMetricUnitEq = detail.totalProcessMetricUnitEq,
                                    id_metricUnitProcess = detail.id_metricUnitProcess,
                                    quantityEntered = detail.quantityEntered,
                                    quantityPoundsClose = detail.quantityPoundsClose,

                                    //Campos nuevos agregados a la tabla
                                    differencia = detail.differencia,
                                    distributedd = detail.distributedd,
                                    precioDisF = detail.precioDisF,
                                    priceDis = detail.priceDis,
                                    rendimientoF = detail.rendimientoF,
                                    totalKgs = detail.totalKgs,
                                    totalLbs = detail.totalLbs,
                                    totalPriceDis = detail.totalPriceDis
                                };


                                foreach (var distributed in detail.ProductionLotPaymentDistributed)
                                {
                                    var newDistributed = new ProductionLotPaymentDistributed
                                    {
                                        id_productionLotPayment = newDetail.id,
                                        id_item = distributed.id_item,
                                        kilogram = distributed.kilogram,
                                        number_box = distributed.number_box,
                                        pound = distributed.pound,
                                        performance = distributed.performance,
                                        priceLP = distributed.priceLP,
                                        totalPayLP = distributed.totalPayLP,
                                        isActive = distributed.isActive
                                    };

                                    newDetail.ProductionLotPaymentDistributed.Add(newDistributed);
                                }

                                modelItem.ProductionLotPayment.Add(newDetail);
                            }
                        }

                        #endregion
                        seccion = $"AP-0029";
                        LogInfo(seccion, DateTime.Now);
                        UpdateProductionLotPerformanceOP(modelItem,
                                                            settings,
                                                            metricUnits,
                                                            itemProductionLotPayments,
                                                            metricUnitConversions,
                                                            itemEquivalences);

                        seccion = $"AP-0030";
                        LogInfo(seccion, DateTime.Now);
                        if (approve)
                        {
                            seccion = $"AP-0031";
                            LogInfo(seccion, DateTime.Now);
                            if (modelItem.ProductionLotState.code == "07")//PENDIENTE DE PAGO
                            {
                                seccion = $"AP-0032";
                                LogInfo(seccion, DateTime.Now);
                                if (entityObjectPermissions != null)
                                {
                                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                    if (entityPermissions != null)
                                    {
                                        foreach (var detailt in modelItem.ProductionLotDetail)
                                        {
                                            var entityValuePermissions = entityPermissions
                                                                                    .listValue
                                                                                    .FirstOrDefault(fod2 => fod2.id_entityValue == detailt.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);

                                            if (entityValuePermissions == null)
                                            {
                                                seccion = $"AP-0034-ERR";
                                                LogInfo(seccion, DateTime.Now);
                                                throw new ProdHandlerException("No tiene Permiso para Aprobar la Recepción de Materia Prima.");
                                            }
                                        }

                                    }
                                }
                                seccion = $"AP-0035";
                                LogInfo(seccion, DateTime.Now);
                                modelItem.id_userAuthorizedBy = ActiveUser.id;
                                modelItem.liquidationPaymentDate = DateTime.Now;
                                string state = statePriceList;

                                seccion = $"AP-0036";
                                LogInfo(seccion, DateTime.Now);
                                if (!state.Equals("15"))
                                {
                                    seccion = $"AP-0037-ERR";
                                    LogInfo(seccion, DateTime.Now);
                                    throw new ProdHandlerException("La Lista de Precios debe estar aprobado por Gerencia General");
                                }
                                //if (settCxC != "0")
                                //{
                                seccion = $"AP-0038";
                                LogInfo(seccion, DateTime.Now);
                                string settPCONGELA = settings.FirstOrDefault(fod => fod.code == "PCONGELA")?.value ?? "";
                                string modPrecio = settings.FirstOrDefault(fod => fod.code == "MODPREC")?.value ?? "NO";
                                if (settPCONGELA == "")
                                {
                                    seccion = $"AP-0039-ERR";
                                    LogInfo(seccion, DateTime.Now);
                                    throw new ProdHandlerException("No se puede Aprobar, por no estar definido el Parámetro con código: PCONGELA con valor SI o NO");
                                }
                                else
                                {
                                    if (settPCONGELA != "SI" && settPCONGELA != "NO")
                                    {
                                        throw new ProdHandlerException("No se puede Aprobar, porque el valor: " + settPCONGELA + " definido en el Parámetro con código: PCONGELA, es erróneo los valores deben ser SI o NO");
                                    }
                                    else
                                    {
                                        if (settPCONGELA == "SI" || modPrecio == "SI")
                                        {
                                            seccion = $"AP-0040-PRE";
                                            LogInfo(seccion, DateTime.Now);
                                            var result = ServiceInventoryMove.UpdateInventaryMoveAutomaticRawMaterialEntry(approve, ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem, db, false, null, modelItem.pricePerLbs);
                                            if (!string.IsNullOrEmpty(result?.message)) throw new ProdHandlerException(result.message);
                                            inventoryMoveDetailIdsForDelete = result.inventoryMoveDetailIdsForDelete;
                                            seccion = $"AP-0040-POST";
                                            LogInfo(seccion, DateTime.Now);

                                        }
                                    }
                                }

                                //}
                                //AQUI SE COMENTA 11102018
                                //ReCostReception(modelItem);
                                seccion = $"AP-0041";
                                LogInfo(seccion, DateTime.Now);
                                modelItem.ProductionLotState = productionLotStates.FirstOrDefault(s => s.code == "08"); //PAGADO
                            }
                        }
                    }

                    
                    #endregion

                    isValidateOP = true;
                }
                catch (ProdHandlerException e)
                {
                    seccion = $"AP-0042-ERR";
                    LogInfo(seccion, DateTime.Now);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                }
                catch (Exception e)
                {
                    seccion = $"AP-0043-ERR";
                    LogInfo(seccion, DateTime.Now);
                    ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
                    FullLog(e, seccion: seccion);
                }
                finally
                {
                    seccion = $"AP-0044-ERR";
                    LogInfo(seccion, DateTime.Now);
                    TempData["productionLotReception"] = productionLot;
                    TempData.Keep("productionLotReception");


                     
                }

                seccion = $"AP-0045";
                LogInfo(seccion, DateTime.Now);
                if (isValidateOP)
                {
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //modelItem.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.id == item.id_ProductionLotState);

                            if (modelItem.ProductionLotState.code == "01")//PENDIENTE DE RECEPCION
                            {

                                #region UPDATE ITEMS DETAILS

                                if (!loteManualEdit)
                                {

                                    if (productionLot.ProductionLotDetail != null)
                                    {

                                        for (int i = modelItem.ProductionLotDetail.Count - 1; i >= 0; i--)
                                        {
                                            //ProductionLot Result
                                            var detail = modelItem.ProductionLotDetail.ElementAt(i);

                                            if (settLE.Equals("Y") || detail.ResultProdLotReceptionDetail != null)
                                            {
                                                if (detail.ResultProdLotReceptionDetail != null)
                                                {
                                                    if ((detail.quantitydrained ?? 0) > 0)
                                                    {
                                                        continue;
                                                    }
                                                    else
                                                    {

                                                        var receptionDetailDrainingTest = receptionDetailDrainingTestList
                                                                                                .FirstOrDefault(e => e.idReceptionDetail == detail.ResultProdLotReceptionDetail.idProductionLotReceptionDetail);

                                                        if (receptionDetailDrainingTest != null)
                                                        {
                                                            db.Entry(receptionDetailDrainingTest).State = EntityState.Deleted;
                                                        }

                                                        var resultProdLotReceptionDetail = detail.ResultProdLotReceptionDetail;
                                                        //detail.ResultProdLotReceptionDetail.Remove(resultProdLotReceptionDetail);
                                                        db.Entry(resultProdLotReceptionDetail).State = EntityState.Deleted;
                                                    }
                                                }
                                            }

                                            for (int j = detail.ProductionLotDetailPurchaseDetail.Count - 1; j >= 0; j--)
                                            {
                                                var detailProductionLotDetailPurchaseDetail = detail.ProductionLotDetailPurchaseDetail.ElementAt(j);
                                                detail.ProductionLotDetailPurchaseDetail.Remove(detailProductionLotDetailPurchaseDetail);
                                                db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                                            }

                                            //Temporal Not Used

                                            for (int j = detail.ProductionLotDetailQualityControl.Count - 1; j >= 0; j--)
                                            {
                                                var detailProductionLotDetailQualityControl = detail.ProductionLotDetailQualityControl.ElementAt(j);

                                                if (productionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id == detail.id) == null)
                                                {

                                                    var qualityControlAux = detailProductionLotDetailQualityControl.QualityControl;
                                                    if (qualityControlAux != null)
                                                    {
                                                        var documentStateAnulada = documentStates.FirstOrDefault(s => s.code == "05");//ANULADA
                                                        qualityControlAux.Document.id_documentState = documentStateAnulada.id;
                                                        qualityControlAux.Document.DocumentState = documentStateAnulada;

                                                        //db.QualityControl.Attach(qualityControlAux);
                                                        db.Entry(qualityControlAux).State = EntityState.Modified;
                                                    }
                                                }
                                                detailProductionLotDetailQualityControl.IsDelete = true;
                                                detail.ProductionLotDetailQualityControl.Remove(detailProductionLotDetailQualityControl);
                                                db.Entry(detailProductionLotDetailQualityControl).State = EntityState.Deleted;
                                            }

                                            modelItem.ProductionLotDetail.Remove(detail);
                                            db.Entry(detail).State = EntityState.Deleted;
                                        }

                                        foreach (var detail in productionLot.ProductionLotDetail)
                                        {
                                            var tmp = modelItem.ProductionLotDetail.FirstOrDefault(fod => fod.id == detail.id);
                                            if (tmp != null)
                                            {
                                                tmp.quantityRecived = detail.quantityRecived;
                                                tmp.quantitydrained = detail.quantitydrained;
                                                tmp.drawersNumber = detail.drawersNumber;
                                                //db.ProductionLotDetail.Attach(tmp);
                                                db.Entry(tmp).State = EntityState.Modified;
                                                continue;
                                            }

                                            var newDetail = new ProductionLotDetail
                                            {
                                                id_productionLot = modelItem.id,
                                                id_item = detail.id_item,
                                                Item = itemDetails.FirstOrDefault(i => i.id == detail.id_item),
                                                id_warehouse = detail.id_warehouse,
                                                Warehouse = warehouseDetails.FirstOrDefault(i => i.id == detail.id_warehouse),
                                                id_warehouseLocation = detail.id_warehouseLocation,
                                                WarehouseLocation = warehouseLocationDetails.FirstOrDefault(i => i.id == detail.id_warehouseLocation),

                                                quantityOrdered = detail.quantityOrdered,
                                                quantityRemitted = detail.quantityRemitted,
                                                quantityRecived = detail.quantityRecived,

                                                //quantitydrained = (detail.quantitydrained != null && detail.quantitydrained > 0) ? detail.quantitydrained : detail.quantityRecived,
                                                drawersNumber = detail.drawersNumber
                                            };

                                            if (newDetail.drawersNumber <= 0)
                                            {
                                                throw new ProdHandlerException("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                                        newDetail.Item.name + ", en Bodega: " +
                                                                                        newDetail.Warehouse.name + ", Ubicación: " +
                                                                                        newDetail.WarehouseLocation.name + " y Cantidad Ordenada: " +
                                                                                        newDetail.quantityOrdered.ToString("#,##0.00") + ". El Número Kavetas debe ser mayor que cero.");
                                            }

                                            if (settLE.Equals("Y"))
                                            {
                                                newDetail.quantitydrained = detail.quantitydrained;
                                            }
                                            else
                                            {
                                                newDetail.quantitydrained = (detail.quantitydrained != null && detail.quantitydrained > 0) ? detail.quantitydrained : detail.quantityRecived;
                                            }

                                            if (detail.id_warehouse == 0)
                                            {
                                                throw new ProdHandlerException("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                                        newDetail.Item.name + ", no tiene definida una Bodega, Configúrela e intente de nuevo.");

                                            }
                                            if (detail.id_warehouseLocation == 0)
                                            {
                                                throw new ProdHandlerException("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                                        newDetail.Item.name + ", no tiene definida una Ubicación, Configúrela e intente de nuevo.");
                                            }

                                            if (detail.quantityRemitted <= 0)
                                            {
                                                throw new ProdHandlerException("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                                        newDetail.Item.name + ", en Bodega: " +
                                                                                        newDetail.Warehouse.name + ", Ubicación: " +
                                                                                        newDetail.WarehouseLocation.name + " y Cantidad Ordenada: " +
                                                                                        newDetail.quantityOrdered.ToString("#,##0.00") + ". La Cantida Remitida debe ser mayor que cero.");
                                            }

                                            if (detail.quantityRecived <= 0)
                                            {
                                                throw new ProdHandlerException("No se puede guardar el lote. Debido a que el detalle de Materia Prima, el Ítem: " +
                                                                                        newDetail.Item.name + ", en Bodega: " +
                                                                                        newDetail.Warehouse.name + ", Ubicación: " +
                                                                                        newDetail.WarehouseLocation.name + " y Cantidad Ordenada: " +
                                                                                        newDetail.quantityOrdered.ToString("#,##0.00") + ". La Cantida Recibida debe ser mayor que cero.");
                                            }

                                            foreach (var detailPurchaseDetail in detail.ProductionLotDetailPurchaseDetail)
                                            {
                                                var newProductionLotDetailPurchaseDetail = new ProductionLotDetailPurchaseDetail
                                                {
                                                    //id_productionLotDetail = detailPurchaseDetail.id_productionLotDetail,
                                                    id_purchaseOrderDetail = detailPurchaseDetail.id_purchaseOrderDetail != null && detailPurchaseDetail.id_purchaseOrderDetail > 0 ? detailPurchaseDetail.id_purchaseOrderDetail : null,
                                                    id_remissionGuideDetail = detailPurchaseDetail.id_remissionGuideDetail != null && detailPurchaseDetail.id_remissionGuideDetail > 0 ? detailPurchaseDetail.id_remissionGuideDetail : null,
                                                    quanty = detailPurchaseDetail.quanty
                                                };
                                                newDetail.ProductionLotDetailPurchaseDetail.Add(newProductionLotDetailPurchaseDetail);
                                            }

                                            //Quality
                                            //Temporal Not Use
                                            newDetail.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();
                                            foreach (var productionLotDetailQualityControlDet in detail.ProductionLotDetailQualityControl)
                                            {
                                                var newProductionLotDetailQualityControl = new ProductionLotDetailQualityControl
                                                {
                                                    id_productionLotDetail = productionLotDetailQualityControlDet.id_productionLotDetail,
                                                    ProductionLotDetail = newDetail,
                                                    id_qualityControl = productionLotDetailQualityControlDet.id_qualityControl,
                                                    QualityControl = QualityControls.FirstOrDefault(i => i.id == productionLotDetailQualityControlDet.id_qualityControl),
                                                };
                                                newDetail.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                                newProductionLotDetailQualityControl.QualityControl.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                                newProductionLotDetailQualityControl.QualityControl.id_lot = modelItem.id;
                                                newProductionLotDetailQualityControl.QualityControl.Lot = modelItem.Lot;
                                            }

                                            if (aSettingANL != null && aSettingANL.value == "SI")
                                            {
                                                if (productionLotDetailQualityControl != null)
                                                {
                                                    newDetail.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();

                                                    var newProductionLotDetailQualityControl = new ProductionLotDetailQualityControl
                                                    {
                                                        id_productionLotDetail = detail.id,
                                                        ProductionLotDetail = newDetail,
                                                        id_qualityControl = productionLotDetailQualityControl.id_qualityControl,
                                                        QualityControl = qualityControlDetailParam,
                                                    };
                                                    newDetail.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);
                                                    newProductionLotDetailQualityControl.QualityControl.ProductionLotDetailQualityControl.Add(newProductionLotDetailQualityControl);

                                                }
                                            }

                                            if (modelItem.withPrice)
                                            {
                                                costMP += (detail.quantityRecived * (detail.ProductionLotDetailPurchaseDetail?.FirstOrDefault()?.PurchaseOrderDetail.price ?? 0));
                                            }

                                            modelItem.ProductionLotDetail.Add(newDetail);
                                        }
                                    }

                                    if (modelItem.ProductionLotDetail.Count == 0)
                                    {
                                        throw new ProdHandlerException("No se puede guardar un lote sin detalles de Materia Prima");
                                    }

                                    #endregion

                                    UpdateProductionLotTotalsOP(modelItem, itemDetails, metricUnitConversions, settings, metricUnits);
                                    //Por ahora esto queda desahibilitado hasta que se parametrice 
                                    //la opción de recepción de Materiales.

                                }
                            }

                            if (modelItem.ProductionLotState.code == "02")//RECEPCIONADO
                            {
                                modelItem.ProductionLotState = productionLotStates.FirstOrDefault(s => s.code == "03");
                                if (settCxC != "0")
                                {
                                    int seqLiquidation = modelItem.sequentialLiquidation ?? 0;
                                    if (seqLiquidation == 0)
                                    {
                                        var resultDocumentType = ServiceInventoryMove
                                                                .GetDocumentTypeSequentialAndNumber("94", db, this.ActiveCompany);

                                        DocumentType documentTypeLiq = resultDocumentType.Item1;
                                        modelItem.sequentialLiquidation = resultDocumentType.Item2;
                                        if (documentTypeLiq != null)
                                        {
                                            documentTypeLiq.currentNumber = resultDocumentType.Item4;
                                            //db.DocumentType.Attach(documentTypeLiq);
                                            db.Entry(documentTypeLiq).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }

                            if (modelItem.ProductionLotState.code == "03")//PENDIENTE DE PROCESAMIENTO
                            {
                                //modelItem.liquidationDate = modelItem.liquidationDate ?? DateTime.Now;
                                modelItem.liquidationDate = DateTime.Now;
                                modelItem.wholeGarbagePounds = wholeGarbagePoundsLiq ?? 0;
                                modelItem.poundsGarbageTail = poundsGarbageTailLiq ?? 0;
                                modelItem.wholeLeftover = wholeLeftoverLiq ?? 0;
                                modelItem.tailLeftOver = tailLeftOverLiq ?? 0;

                                #region UPDATE PRODUCTION LOT LIQUIDATIONS DETAILS
                                if (settCxC == "0" && modelItem.ProductionLotLiquidationTotal.Count() == 0)
                                {
                                    if (productionLot.ProductionLotLiquidation != null)
                                    {
                                        for (int i = modelItem.ProductionLotLiquidation.Count - 1; i >= 0; i--)
                                        {
                                            var detail = modelItem.ProductionLotLiquidation.ElementAt(i);

                                            for (int j = detail.ProductionLotLiquidationPackingMaterialDetail.Count - 1; j >= 0; j--)
                                            {
                                                var detailProductionLotLiquidationPackingMaterialDetail = detail.ProductionLotLiquidationPackingMaterialDetail.ElementAt(j);
                                                detail.ProductionLotLiquidationPackingMaterialDetail.Remove(detailProductionLotLiquidationPackingMaterialDetail);
                                                db.Entry(detailProductionLotLiquidationPackingMaterialDetail).State = EntityState.Deleted;
                                            }

                                            modelItem.ProductionLotLiquidation.Remove(detail);
                                            db.Entry(detail).State = EntityState.Deleted;
                                        }

                                        for (int i = modelItem.ProductionLotPackingMaterial.Count - 1; i >= 0; i--)
                                        {
                                            var detail = modelItem.ProductionLotPackingMaterial.ElementAt(i);

                                            for (int j = detail.ProductionLotLiquidationPackingMaterialDetail.Count - 1; j >= 0; j--)
                                            {
                                                var detailProductionLotLiquidationPackingMaterialDetail = detail.ProductionLotLiquidationPackingMaterialDetail.ElementAt(j);
                                                detail.ProductionLotLiquidationPackingMaterialDetail.Remove(detailProductionLotLiquidationPackingMaterialDetail);
                                                db.Entry(detailProductionLotLiquidationPackingMaterialDetail).State = EntityState.Deleted;
                                            }

                                            modelItem.ProductionLotPackingMaterial.Remove(detail);
                                            db.Entry(detail).State = EntityState.Deleted;
                                        }

                                        foreach (var detail in productionLot.ProductionLotLiquidation)
                                        {
                                            var newDetail = new ProductionLotLiquidation
                                            {
                                                id_productionLot = modelItem.id,
                                                id_item = detail.id_item,
                                                Item = itemDetailsLiq.FirstOrDefault(fod => fod.id == detail.id_item),
                                                id_warehouse = detail.id_warehouse,
                                                Warehouse = warehouseDetailsLiq.FirstOrDefault(fod => fod.id == detail.id_warehouse),
                                                id_warehouseLocation = detail.id_warehouseLocation,
                                                WarehouseLocation = warehouseLocationDetailsLiq.FirstOrDefault(fod => fod.id == detail.id_warehouseLocation),
                                                id_salesOrder = detail.id_salesOrder,
                                                SalesOrder = salesOrderSalesOrdersLiq.FirstOrDefault(fod => fod.id == detail.id_salesOrder),
                                                id_salesOrderDetail = detail.id_salesOrderDetail,
                                                SalesOrderDetail = salesOrderDetailsLiq.FirstOrDefault(fod => fod.id == detail.id_salesOrderDetail),
                                                quantity = detail.quantity,
                                                id_metricUnit = detail.id_metricUnit,
                                                MetricUnit = metricUnitsLiq.FirstOrDefault(fod => fod.id == detail.id_metricUnit),
                                                quantityTotal = detail.quantityTotal,
                                                id_metricUnitPresentation = detail.id_metricUnitPresentation,
                                                quantityPoundsLiquidation = detail.quantityPoundsLiquidation,
                                                MetricUnit1 = metricUnitsLiq.FirstOrDefault(fod => fod.id == detail.id_metricUnitPresentation)
                                            };

                                            foreach (var detailPackingMaterialDetail in detail.ProductionLotLiquidationPackingMaterialDetail)
                                            {
                                                var newDetailPackingMaterial = modelItem.ProductionLotPackingMaterial.FirstOrDefault(d => d.id == detailPackingMaterialDetail.id_productionLotPackingMaterial);
                                                if (newDetailPackingMaterial == null)
                                                {
                                                    newDetailPackingMaterial = new ProductionLotPackingMaterial
                                                    {
                                                        id = detailPackingMaterialDetail.id_productionLotPackingMaterial,
                                                        id_productionLot = modelItem.id,
                                                        id_item = detailPackingMaterialDetail.ProductionLotPackingMaterial.id_item,
                                                        Item = itemDetailPackingMaterialDetails.FirstOrDefault(fod => fod.id == detailPackingMaterialDetail.ProductionLotPackingMaterial.id_item),
                                                        quantity = detailPackingMaterialDetail.ProductionLotPackingMaterial.quantity,
                                                        quantityRequiredForProductionLot = detailPackingMaterialDetail.ProductionLotPackingMaterial.quantityRequiredForProductionLot,
                                                        isActive = true,
                                                        manual = false,
                                                        id_userCreate = ActiveUser.id,
                                                        dateCreate = DateTime.Now,
                                                        id_userUpdate = ActiveUser.id
                                                    };
                                                    newDetailPackingMaterial.dateUpdate = newDetailPackingMaterial.dateCreate;
                                                    modelItem.ProductionLotPackingMaterial.Add(newDetailPackingMaterial);
                                                }

                                                var newProductionLotLiquidationPackingMaterialDetail = new ProductionLotLiquidationPackingMaterialDetail
                                                {
                                                    id_productionLotLiquidation = newDetail.id,
                                                    ProductionLotLiquidation = newDetail,
                                                    id_productionLotPackingMaterial = newDetailPackingMaterial.id,
                                                    ProductionLotPackingMaterial = newDetailPackingMaterial,
                                                    quantity = detailPackingMaterialDetail.quantity
                                                };
                                                newDetail.ProductionLotLiquidationPackingMaterialDetail.Add(newProductionLotLiquidationPackingMaterialDetail);
                                                newDetailPackingMaterial.ProductionLotLiquidationPackingMaterialDetail.Add(newProductionLotLiquidationPackingMaterialDetail);

                                            }

                                            modelItem.ProductionLotLiquidation.Add(newDetail);
                                        }
                                    }

                                    if (modelItem.ProductionLotLiquidation.Count == 0)
                                    {
                                        throw new ProdHandlerException("No se puede guardar un lote sin detalles de Liquidación en este Estado");
                                    }
                                }
                                else
                                {
                                    if (modelItem.ProductionLotLiquidationTotal.Count == 0)
                                    {
                                        throw new ProdHandlerException("No se puede guardar un lote sin detalles de Liquidación en este Estado");
                                    }

                                }
                                #endregion

                                #region UPDATE PRODUCTION LOT PACKING MATERIAL
                                if (productionLot.ProductionLotPackingMaterial != null)
                                {

                                    foreach (var detail in productionLot.ProductionLotPackingMaterial)
                                    {
                                        if ((detail.ProductionLotLiquidationPackingMaterialDetail?.Count() ?? 0) > 0)
                                        {
                                            continue;
                                        }

                                        var newDetailPackingMaterial = new ProductionLotPackingMaterial
                                        {
                                            id = detail.id,
                                            id_productionLot = modelItem.id,
                                            id_item = detail.id_item,
                                            Item = itemDetailsPaq.FirstOrDefault(fod => fod.id == detail.id_item),
                                            quantity = detail.quantity,
                                            quantityRequiredForProductionLot = 0,
                                            isActive = true,
                                            manual = true,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id
                                        };
                                        newDetailPackingMaterial.dateUpdate = newDetailPackingMaterial.dateCreate;

                                        modelItem.ProductionLotPackingMaterial.Add(newDetailPackingMaterial);
                                    }
                                }

                                #endregion

                                #region UPDATE PRODUCTION LOT TRASHS DETAILS

                                if (productionLot.ProductionLotTrash != null)
                                {
                                    for (int i = modelItem.ProductionLotTrash.Count - 1; i >= 0; i--)
                                    {
                                        var detail = modelItem.ProductionLotTrash.ElementAt(i);
                                        modelItem.ProductionLotTrash.Remove(detail);
                                        db.Entry(detail).State = EntityState.Deleted;
                                    }

                                    foreach (var detail in productionLot.ProductionLotTrash)
                                    {
                                        var newDetail = new ProductionLotTrash
                                        {
                                            id_productionLot = modelItem.id,
                                            id_item = detail.id_item,
                                            id_metricUnit = detail.id_metricUnit,
                                            MetricUnit = metricUnitsTrash.FirstOrDefault(fod => fod.id == detail.id_metricUnit),
                                            id_warehouse = detail.id_warehouse,
                                            id_warehouseLocation = detail.id_warehouseLocation,

                                            quantity = detail.quantity
                                        };

                                        modelItem.ProductionLotTrash.Add(newDetail);
                                    }
                                }

                                #endregion

                                if (settCxC == "0" && modelItem.ProductionLotLiquidationTotal.Count() == 0)
                                {
                                    UpdateProductionLotProductionLotLiquidationsDetailTotalsOP(modelItem, itemDetailsLiq, metricUnitConversions, settings, metricUnits);
                                }
                                else
                                {
                                    UpdateProductionLotProductionLotLiquidationTotalsOP(modelItem, settings, metricUnits);
                                }

                                if (modelItem.withPrice)
                                {
                                    modelItem.pricePerLbs = (modelItem.totalQuantityLiquidation != 0 ? costMP / modelItem.totalQuantityLiquidation : 0);
                                }
                                UpdateProductionLotProductionLotTrashsDetailTotalsOP(modelItem, itemDetailsTrash, metricUnitConversions, settings, metricUnits);
                                var quantityDrainedAux = GetTotalQuantityDrainedOP(modelItem, itemDetails, metricUnitConversions, settings, metricUnits);
                                if (quantityDrainedAux < (modelItem.totalQuantityLiquidation + modelItem.totalQuantityTrash))
                                {
                                    messageWarning = WarningMessage("Lote: " + modelItem.number + " guardado exitosamente. Pero la cantidad total de Prueba de Escurrido es menor que la cantidad liquidada más desperdicio.");
                                }
                            }

                            if (modelItem.ProductionLotState.code == "04")//EN PROCESAMIENTO
                            {
                                modelItem.ProductionLotState = productionLotStates.FirstOrDefault(s => s.code == "05"); ;
                            }

                            if (modelItem.ProductionLotState.code == "06")//CERRADO
                            {
                                modelItem.ProductionLotState = productionLotStates.FirstOrDefault(s => s.code == "07");
                                if (settCxC == "0" || modelItem.ProductionLotLiquidationTotal.Count() == 0)
                                {
                                    int seqLiquidation = modelItem.sequentialLiquidation ?? 0;
                                    if (seqLiquidation == 0)
                                    {
                                        var resultDocumentType = ServiceInventoryMove
                                                                        .GetDocumentTypeSequentialAndNumber("94", db, this.ActiveCompany);
                                        DocumentType documentTypeLiq = resultDocumentType.Item1;
                                        modelItem.sequentialLiquidation = resultDocumentType.Item2;
                                        if (documentTypeLiq != null)
                                        {
                                            documentTypeLiq.currentNumber = resultDocumentType.Item4;
                                            //db.DocumentType.Attach(documentTypeLiq);
                                            db.Entry(documentTypeLiq).State = EntityState.Modified;
                                        }

                                    }
                                }

                                if (item.id_priceList == null)
                                {
                                    if (_apTmp != null && _apTmp.id_priceList != null)
                                    {
                                        string settingTLA = settings.FirstOrDefault(r => r.code == "TLA")?.value;   // DataProviderSetting.ValueSetting("TLA");
                                        if (!string.IsNullOrEmpty(settingTLA) && settingTLA.Equals("REF"))
                                        {
                                            modelItem.id_priceList = null;
                                            modelItem.PriceList = null;
                                        }
                                        else
                                        {
                                            modelItem.id_priceList = _apTmp.id_priceList;
                                            modelItem.PriceList = pricelist;
                                        }

                                        modelItem.liquidationPaymentDate = _apTmp.Document.emissionDate;
                                    }
                                }
                                else
                                {
                                    modelItem.id_priceList = item.id_priceList;
                                    modelItem.liquidationPaymentDate = item.liquidationDate;
                                }

                                var command = db.Database.Connection.CreateCommand();

                                var m_idProductionLot = "@idProductionLot";
                                var m_userPendingApproval = "@userPendingApproval";

                                command.CommandText = $"INSERT INTO ProductionLotStateChange " +
                                    $"(IdProductionLot,Id_UserPendingApproval) VALUES" +
                                    $"(@idProductionLot,@userPendingApproval)";

                                command.Parameters.Add(new SqlParameter(m_idProductionLot, modelItem.id));
                                command.Parameters.Add(new SqlParameter(m_userPendingApproval, ActiveUser.id));
                                command.CommandType = CommandType.Text;
                                command.Transaction = trans.UnderlyingTransaction;
                                command.ExecuteNonQuery();

                            }

                            if (approve & modelItem.ProductionLotState.code != "07")
                            {
                                seccion = $"AP-0046-NODEBIOINGRESAR";
                                LogInfo(seccion, DateTime.Now);
                                if (entityObjectPermissions != null)
                                {
                                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                                    if (entityPermissions != null)
                                    {
                                        foreach (var detailt in modelItem.ProductionLotDetail)
                                        {
                                            var entityValuePermissions = entityPermissions
                                                                                    .listValue
                                                                                    .FirstOrDefault(fod2 => fod2.id_entityValue == detailt.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);

                                            if (entityValuePermissions == null)
                                            {
                                                throw new ProdHandlerException("No tiene Permiso para Aprobar la Recepción de Materia Prima.");
                                            }
                                        }

                                    }
                                }

                                if (modelItem.ProductionLotState.code == "01")//PENDIENTE DE RECEPCION
                                {
                                    bool loteManual1 = false;
                                    if (productionLot != null)
                                    {
                                        loteManual1 = productionLot.ProductionProcess.code == "RMM";
                                    }
                                    if (!loteManual1)
                                    {
                                        var quantityDrainedAux = GetTotalQuantityDrainedOP(modelItem, itemDetails, metricUnitConversions, settings, metricUnits);
                                        if (quantityDrainedAux <= 0)
                                        {
                                            throw new ProdHandlerException("No se puede aprobar el lote por no tener prueba de escurrido realizada");
                                        }

                                        int? aId_processType = 0;
                                        string aCode_processType = "";
                                        foreach (var detail in modelItem.ProductionLotDetail)
                                        {
                                            var aQualityControl = detail.ProductionLotDetailQualityControl
                                                                                    .FirstOrDefault(fod => fod.IsDelete == false
                                                                                                           && fod.QualityControl.Document.DocumentState.code.Equals("03"))?.QualityControl;//"03": Aprobado

                                            if (aQualityControl == null)
                                            {
                                                throw new ProdHandlerException("No se puede aprobar el lote por tener detalles de Materia Prima sin Calidad APROBADA(Conforme)");
                                            }

                                            if (!(aQualityControl?.isConforms ?? false))//"03": Aprobado
                                            {
                                                throw new ProdHandlerException("No se puede aprobar el lote por tener detalles de Materia Prima con Calidad no APROBADA(Conforme)");
                                            }

                                            if (detail.quantitydrained <= 0)
                                            {
                                                throw new ProdHandlerException("No se puede aprobar el lote por no tener prueba de escurrido realizada");
                                            }

                                            if (aId_processType == 0)
                                            {
                                                aId_processType = aQualityControl?.id_processType;
                                                aCode_processType = aQualityControl?.ProcessType?.code;
                                            }
                                            if (aQualityControl?.id_processType != aId_processType)
                                            {
                                                throw new ProdHandlerException("No se puede aprobar el lote por tener detalles de Materia Prima con Calidad APROBADA(Conforme) y Tipo de Proceso diferente");
                                            }

                                            foreach (var productionLotDetailPurchaseDetail in detail.ProductionLotDetailPurchaseDetail)
                                            {
                                                ServicePurchaseRemission.UpdateQuantityRecivedOP(db,
                                                                                                    productionLotDetailPurchaseDetail.id_purchaseOrderDetail,
                                                                                                    productionLotDetailPurchaseDetail.id_remissionGuideDetail,
                                                                                                    productionLotDetailPurchaseDetail.quanty,
                                                                                                    remissionGuideDetails,
                                                                                                    purchaseOrderDetails);
                                            }
                                        }

                                        var processTypeENT = processTypes.FirstOrDefault(fod => fod.code == "ENT");
                                        var processTypeCOL = processTypes.FirstOrDefault(fod => fod.code == "COL");

                                        if (aCode_processType != "ENT" && modelItem.LiquidationCartOnCart != null && modelItem.LiquidationCartOnCart.Count() > 0)
                                        {
                                            var aLiquidationCartOnCart = modelItem.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code == "03" && //03:APROBADA
                                                                                                                               fod.idProccesType == processTypeENT.id);
                                            if (aLiquidationCartOnCart != null)
                                            {
                                                throw new ProdHandlerException("No se puede aprobar el Lote, debe revisar los Tipo de Proceso de las liquidaciones Carro X Carro de este Lote");
                                            }

                                        }

                                        modelItem.ProductionLotState = productionLotStates.FirstOrDefault(s => s.code == "02"); //RECEPCIONADO

                                        modelItem.ProcessType = (processTypeENT.id == aId_processType) ? processTypeENT : processTypeCOL;
                                        modelItem.id_processtype = (processTypeENT.id == aId_processType) ? processTypeENT.id : processTypeCOL.id;

                                        //Actualizando Estado del Documento Lote Base
                                        var documentState = documentStates.FirstOrDefault(s => s.code == "03"); //APROBADA
                                        modelItem.Lot.Document.id_documentState = documentState.id;
                                        modelItem.Lot.Document.DocumentState = documentState;
                                    }
                                    else
                                    {
                                        modelItem.ProductionLotState = productionLotStates.FirstOrDefault(s => s.code == "08");
                                        //Actualizando Estado del Documento Lote Base
                                        var documentState = documentStates.FirstOrDefault(s => s.code == "03"); //APROBADA
                                        modelItem.Lot.Document.id_documentState = documentState.id;
                                        modelItem.Lot.Document.DocumentState = documentState;
                                    }

                                }
                                if (modelItem.ProductionLotState.code == "03")//PENDIENTE DE PROCESAMIENTO
                                {
                                    int seqLiquidation = modelItem.sequentialLiquidation ?? 0;
                                    if (seqLiquidation == 0)
                                    {
                                        var resultDocumentType = ServiceInventoryMove
                                                                        .GetDocumentTypeSequentialAndNumber("94", db, this.ActiveCompany);

                                        DocumentType documentTypeLiq = resultDocumentType.Item1;
                                        modelItem.sequentialLiquidation = resultDocumentType.Item2;

                                        if (documentTypeLiq != null)
                                        {
                                            documentTypeLiq.currentNumber = resultDocumentType.Item4;
                                            //db.DocumentType.Attach(documentTypeLiq);
                                            db.Entry(documentTypeLiq).State = EntityState.Modified;
                                        }
                                    }

                                    if (settCxC == "0" && modelItem.ProductionLotLiquidationTotal.Count() == 0)
                                    {
                                        //if (modelItem.totalQuantityRecived < (modelItem.totalQuantityLiquidation + modelItem.totalQuantityTrash))
                                        //{
                                        //    throw new Exception("No se puede aprobar el lote por tener la cantidad liquidada(procesada) mayor que la recibida");
                                        //}
                                    }
                                    else
                                    {
                                        if (modelItem.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code == "01") != null)//PENDIENTE
                                        {
                                            throw new ProdHandlerException("No se puede aprobar el lote por tener Liquidación de Carror x Carro pendiente. Apruébelo e intente de nuevo");
                                        }
                                    }
                                    //General Detalle de Cierre y Pago                                                                                          
                                    #region CREATE PRODUCTION LOT CLOSES AND PAYMENT DETAILS

                                    for (int i = modelItem.ProductionLotPayment.Count - 1; i >= 0; i--)
                                    {
                                        var detail = modelItem.ProductionLotPayment.ElementAt(i);
                                        modelItem.ProductionLotPayment.Remove(detail);
                                        db.Entry(detail).State = EntityState.Deleted;
                                    }

                                    if (productionLot.ProductionLotPayment != null)
                                    {
                                        for (int i = modelItem.ProductionLotPayment.Count - 1; i >= 0; i--)
                                        {
                                            var detail = modelItem.ProductionLotPayment.ElementAt(i);
                                            modelItem.ProductionLotPayment.Remove(detail);
                                            db.Entry(detail).State = EntityState.Deleted;
                                        }

                                        List<PLliqTotalDTO> groupByItem = new List<PLliqTotalDTO>();
                                        if (settCxC == "0" && modelItem.ProductionLotLiquidationTotal.Count() == 0)
                                        {
                                            groupByItem = productionLot.ProductionLotLiquidation.GroupBy(gb => new
                                            {
                                                gb.id_item,
                                                gb.id_metricUnitPresentation
                                            }).Select(s => new PLliqTotalDTO
                                            {
                                                id_item = s.Key.id_item,
                                                id_metricUnit = s.Key.id_metricUnitPresentation,
                                                quantity = s.Sum(ss => ss.quantityTotal.Value),
                                                quantityEntered = s.Sum(ss => ss.quantity),
                                                quantityPounds = s.Sum(ss => ss.quantityPoundsLiquidation)
                                            }).ToList();
                                        }
                                        else
                                        {
                                            groupByItem = productionLot.ProductionLotLiquidationTotal.GroupBy(gb => new
                                            {
                                                gb.id_ItemLiquidation,
                                                gb.id_metricUnitPresentation
                                            }).Select(s => new PLliqTotalDTO
                                            {
                                                id_item = s.Key.id_ItemLiquidation,
                                                id_metricUnit = s.Key.id_metricUnitPresentation,
                                                quantity = s.Sum(ss => ss.quantityTotal.Value),
                                                quantityEntered = s.Sum(ss => ss.quatityBoxesIL),
                                                quantityPounds = s.Sum(ss => ss.quantityPoundsIL),
                                            }).ToList();
                                        }

                                        foreach (var detail in groupByItem)
                                        {
                                            var newDetail = new ProductionLotPayment
                                            {
                                                id_productionLot = modelItem.id,
                                                id_item = detail.id_item,                                                
                                                id_metricUnit = detail.id_metricUnit,
                                                quantity = detail.quantity,
                                                quantityEntered = detail.quantityEntered,
                                                quantityPoundsClose = detail.quantityPounds,
                                                adjustMore = 0,
                                                adjustLess = 0,
                                                totalMU = detail.quantity,
                                                price = 0,
                                                priceEdition = 0,
                                                totalToPay = 0,
                                                totalToPayEq = 0,
                                                totalPounds = 0,
                                                fitPounds = 0,
                                                totalProcessMetricUnit = 0,
                                                totalProcessMetricUnitEq = 0,
                                                id_metricUnitProcess = detail.id_metricUnit
                                            };

                                            modelItem.ProductionLotPayment.Add(newDetail);
                                        }
                                    }

                                    #endregion
                                    UpdateProductionLotPerformance(modelItem);

                                    if (modelItem.ProductionLotDetail.FirstOrDefault(w => w.quantitydrained == 0 || w.quantitydrained == null) != null)
                                    {
                                        throw new ProdHandlerException("Hay detalles sin libras de escurrido");
                                    }

                                    modelItem.ProductionLotState = productionLotStates.FirstOrDefault(s => s.code == "04"); //EN PROCESAMIENTO
                                }
                                if (modelItem.ProductionLotState.code == "05")//PENDIENTE DE CIERRE
                                {
                                    modelItem.closeDate = DateTime.Now;
                                    modelItem.ProductionLotState = productionLotStates.FirstOrDefault(s => s.code == "06"); //CERRADO
                                }

                            }

                            seccion = $"AP-0047";
                            LogInfo(seccion, DateTime.Now);
                            bool _loteManual = false;
                            seccion = $"AP-0048";
                            LogInfo(seccion, DateTime.Now);
                            if (productionLot != null)
                            {
                                seccion = $"AP-0049";
                                LogInfo(seccion, DateTime.Now);
                                _loteManual = productionLot.ProductionProcess.code == "RMM";
                            }
                            seccion = $"AP-0050";
                            LogInfo(seccion, DateTime.Now);
                            if (!_loteManual)
                            {
                                seccion = $"AP-0051";
                                LogInfo(seccion, DateTime.Now);
                                if (modelItem.liquidationPaymentDate == null)
                                {
                                    seccion = $"AP-0052";
                                    LogInfo(seccion, DateTime.Now);
                                    modelItem.liquidationPaymentDate = _apTmp?.Document?.emissionDate;
                                }

                                seccion = $"AP-0053";
                                LogInfo(seccion, DateTime.Now);
                                this.ActualizarRegistroRomaneoOP(modelItem, productionUnitProviderName, productionUnitProviderPoolName);
                            }
                            seccion = $"AP-0054";
                            LogInfo(seccion, DateTime.Now);
                            modelItem.internalNumber = item.internalNumberConcatenated;
                            
                            seccion = $"AP-0055";
                            LogInfo(seccion, DateTime.Now);
                            db.ProductionLot.Attach(modelItem);

                            seccion = $"AP-0056";
                            LogInfo(seccion, DateTime.Now);
                            db.Entry(modelItem).State = EntityState.Modified;

                            seccion = $"AP-0057";
                            LogInfo(seccion, DateTime.Now);
                            db.SaveChanges();

                            seccion = $"AP-0058";
                            LogInfo(seccion, DateTime.Now);
                            trans.Commit();

                            seccion = $"AP-0059";
                            LogInfo(seccion, DateTime.Now);
                            isSaved = true;
                            if (!string.IsNullOrEmpty(messageWarning))
                            {
                                ViewData["EditMessage"] = messageWarning;
                            }
                            else
                            {
                                ViewData["EditMessage"] = SuccessMessage($"Lote: {modelItem.number} guardado exitosamente");
                            }

                        }
                        catch (ProdHandlerException e)
                        {
                            seccion = $"AP-0060-ERR";
                            LogInfo(seccion, DateTime.Now);
                            trans.Rollback();
                            ViewData["EditMessage"] = ErrorMessage(e.Message);
                        }
                        catch (Exception e)
                        {
                            seccion = $"AP-0061-ERR";
                            LogInfo(seccion, DateTime.Now);
                            trans.Rollback();
                            ViewData["EditMessage"] = GenericError.ErrorGeneral;
                            FullLog(e, seccion);
                        }
                        finally
                        {
                            TempData["productionLotReception"] = productionLot;
                            TempData.Keep("productionLotReception");
                        }

                        if (isSaved)
                        {
                            seccion = $"AP-0062";
                            LogInfo(seccion, DateTime.Now);
                            if ((inventoryMoveDetailIdsForDelete?.Length ?? 0) > 0)
                            {
                                seccion = $"AP-0063";
                                LogInfo(seccion, DateTime.Now);
                                using (DbContextTransaction transDel = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        seccion = $"AP-0064";
                                        LogInfo(seccion, DateTime.Now);
                                        string ids = inventoryMoveDetailIdsForDelete
                                                        .Select(r => r.ToString())
                                                        .Aggregate((i, j) => $"{i}|{j}");

                                        seccion = $"AP-0065";
                                        LogInfo(seccion, DateTime.Now);
                                        System.Data.Common.DbTransaction transaction = transDel.UnderlyingTransaction;
                                        SqlConnection connectionDel = transaction.Connection as SqlConnection;

                                        seccion = $"AP-0066";
                                        LogInfo(seccion, DateTime.Now);
                                        connectionDel.Execute("dbo.TransCtlDeleteInventoryMoveDetail", new
                                        {
                                            inventoryMoveDetailIds = ids
                                        }, transaction, 1200, CommandType.StoredProcedure);

                                        seccion = $"AP-0066";
                                        LogInfo(seccion, DateTime.Now);
                                        transDel.Commit();

                                        seccion = $"AP-0067";
                                        LogInfo(seccion, DateTime.Now);

                                    }
                                    catch (Exception e)
                                    {
                                        seccion = $"AP-0068-ERR";
                                        LogInfo(seccion, DateTime.Now);
                                        transDel.Rollback();
                                        FullLog(e);
                                        throw;
                                    }


                                }

                            }

                        }

                    }

                    if (!isSaved)
                    {
                        seccion = $"AP-0069";
                        LogInfo(seccion, DateTime.Now);
                        productionLot.julianoNumber = strJul;
                        productionLot.internalNumber = strInt;
                        productionLot.internalNumberConcatenated = (aCertification?.idLote ?? "") + strJul + strInt;
                        bool _loteManual = false;
                        if (productionLot != null)
                        {
                            seccion = $"AP-0070";
                            LogInfo(seccion, DateTime.Now);
                            _loteManual = productionLot.ProductionProcess.code == "RMM";
                        }
                        if (!_loteManual)
                        {
                            seccion = $"AP-0071";
                            LogInfo(seccion, DateTime.Now);
                            return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                        }
                        else
                        {
                            seccion = $"AP-0072";
                            LogInfo(seccion, DateTime.Now);
                            return PartialView("_ProductionLotManualReceptionEditFormPartial", productionLot);
                        }
                    }

                }


                try
                {
                    string settECUP = db.Setting.FirstOrDefault(fod => fod.code == "ECUP")?.value ?? "0";
                    if (settECUP == "1")
                    {
                        try
                        {
                            if (approve && (modelItem.ProductionLotState.code == "08"))
                            {
                                bool respuestaEnvioCorreo = EnviarCorreoComprobanteUnicoPago(modelItem);
                                if (!respuestaEnvioCorreo)
                                    throw new ProdHandlerException("Error, de configuración de envio de correo. Verifique, e intente de nuevo");
                            }
                        }
                        catch (DbEntityValidationException dbEx) //Exception err
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    LogInfo(validationError.ErrorMessage, DateTime.Now);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            FullLog(e);
                        }
                    }
                    seccion = $"AP-0073";
                    LogInfo(seccion, DateTime.Now);
                    modelItem.julianoNumber = strJul;
                    modelItem.internalNumber = strInt;

                    seccion = $"AP-0074";
                    LogInfo(seccion, DateTime.Now);
                    modelItem.internalNumberConcatenated = (aCertification?.idLote ?? "") + strJul + strInt;

                    bool loteManual = false;
                    seccion = $"AP-0075";
                    LogInfo(seccion, DateTime.Now);
                    if (modelItem != null)
                    {
                        loteManual = modelItem.ProductionProcess.code == "RMM";
                    }
                    seccion = $"AP-0076";
                    LogInfo(seccion, DateTime.Now);
                    if (!loteManual)
                    {
                        seccion = $"AP-0077";
                        LogInfo(seccion, DateTime.Now);
                        if (isSaved)
                        {
                            seccion = $"AP-0078";
                            LogInfo(seccion, DateTime.Now);
                            if (modelItem.ProductionLotState.code == "01")
                            {
                                seccion = $"AP-0079";
                                LogInfo(seccion, DateTime.Now);
                                GenerateResultForDrainingTest(modelItem.id);
                                this.ViewBag.MostrarDistribuir = isSaved;
                            }
                        }
                        seccion = $"AP-0080";
                        LogInfo(seccion, DateTime.Now);
                        nameView = "_ProductionLotReceptionEditFormPartial";
                        //return PartialView("_ProductionLotReceptionEditFormPartial", modelItem);
                    }
                    else
                    {
                        seccion = $"AP-0081";
                        LogInfo(seccion, DateTime.Now);
                        nameView = "_ProductionLotManualReceptionEditFormPartial";
                        //return PartialView("_ProductionLotManualReceptionEditFormPartial", modelItem);
                    }
                }
                catch (ProdHandlerException e)
                {
                    seccion = $"AP-0082-err";
                    LogInfo(seccion, DateTime.Now);
                    ViewData["EditError"] = ErrorMessage(e.Message);
                }
                catch (Exception e)
                {
                    seccion = $"AP-0083-err";
                    ViewData["EditError"] = GenericError.ErrorGeneral;
                    FullLog(e, seccion: seccion);
                }
                finally
                {
                    TempData["productionLotReception"] = modelItem;
                    TempData.Keep("productionLotReception");
                    TempData.Keep("ProductionUnitProviderByProvider");
                    TempData.Keep("ProductionUnitProviderPoolByUnitProvider");
                }

            }
            
            return PartialView(nameView, modelItem);

            
        }

        private bool EnviarCorreoComprobanteUnicoPago(ProductionLot modelItem)
        {
            //PrintReportsForDocumentGeneral(modelItem.id, "RPCUP");
            ProductionLot ptTmp = (TempData["productionLotReception"] as ProductionLot);
            ptTmp = ptTmp ?? new ProductionLot();
            TempData["productionLotReception"] = ptTmp;
            TempData.Keep("productionLotReception");

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = modelItem.id
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel model = new ReportProdModel
            {
                codeReport = "RPCUP",
                conex = objConex,
                paramCRList = paramLst
            };
            #endregion

            //rep = GetTmpDataName(20);

            //TempData[rep.nameQS] = model;
            //TempData.Keep(rep.nameQS);

            //ReportProdModel model = (TempData[repProd] as ReportProdModel);

            model = model ?? new ReportProdModel();

            Stream str = ServicePrintCrystalReport.PrintCRParameters(db, model.codeReport, model.paramCRList, model.conex, true);

            //TempData.Remove(repProd);

            return str != null;
        }

        private void ReCostReception(ProductionLot productionLot)
        {
            //productionLot.totalQuantityLiquidation = 0.0M;

            //var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;
            decimal priceAdjustAux = 0;
            foreach (var detail in productionLot.ProductionLotPayment)
            {

                priceAdjustAux = detail.totalToPay / detail.quantity;

                var inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("ILP")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                InventoryMove lastInventoryMoveILP = (inventoryMoveDetailAux.Count > 0)
                                                                    ? inventoryMoveDetailAux.First().InventoryMove
                                                                    : null;
                if (lastInventoryMoveILP != null)
                {
                    foreach (var inventoryMoveDetail in lastInventoryMoveILP.InventoryMoveDetail.Where(w => w.id_item == detail.id_item &&
                                                                                                            detail.id_metricUnit == (w.Item.Presentation?.id_metricUnit ?? w.id_metricUnitMove)))
                    {
                        decimal priceAux = 0;

                        priceAux = priceAdjustAux * (inventoryMoveDetail.Item.Presentation?.minimum ?? 1);

                        var id_metricUnitFormulationAux = inventoryMoveDetail.Item.ItemHeadIngredient?.MetricUnit?.id ?? inventoryMoveDetail.id_metricUnitMove;
                        var metricUnitFormulationAux = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitFormulationAux);

                        var factorConversionFormulation = (inventoryMoveDetail.id_metricUnitMove != metricUnitFormulationAux.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == inventoryMoveDetail.id_metricUnitMove &&
                                                                                                                                      fod.id_metricDestiny == metricUnitFormulationAux.id)?.factor ?? 0 : 1;
                        if (factorConversionFormulation == 0)
                        {
                            throw new Exception("Falta el Factor de Conversión entre : " + inventoryMoveDetail.MetricUnit1.code + " y " + metricUnitFormulationAux.code + ".Necesario para el precio Configúrelo, e intente de nuevo");
                        }
                        else
                        {
                            priceAux = priceAux * factorConversionFormulation;
                        }

                        if (inventoryMoveDetail.unitPrice != priceAux)
                        {
                            inventoryMoveDetail.unitPrice = priceAux;
                            db.InventoryMoveDetail.Attach(inventoryMoveDetail);
                            db.Entry(inventoryMoveDetail).State = EntityState.Modified;
                            ServiceInventoryMove.ReCosting(inventoryMoveDetail, db, ActiveUser);
                        }

                    }
                }
            }
            var inventoryMoves = db.InventoryMove.Where(w => w.InventoryMoveDetail.Any(a => a.id_lot == productionLot.id) &&
                                       w.InventoryReason.code.Equals("EMPP")).ToList();
            List<InventoryMove> inventoryMovesAux = new List<InventoryMove>();
            foreach (var inventoryMove in inventoryMoves)
            {
                var inventoryMovesAuxOrden = inventoryMoves.Where(w => w.id_productionLot == inventoryMove.id_productionLot).OrderByDescending(d => d.Document.dateCreate).ToList();
                InventoryMove lastInventoryMoveEMPP = (inventoryMovesAuxOrden.Count > 0)
                                                       ? inventoryMovesAuxOrden.First()
                                                       : null;
                if (lastInventoryMoveEMPP != null && !inventoryMovesAux.Contains(lastInventoryMoveEMPP))
                {

                    ReCostProcess(lastInventoryMoveEMPP);
                    inventoryMovesAux.Add(lastInventoryMoveEMPP);
                }
            }

        }

        private void ReCostProcess(InventoryMove inventoryMove)
        {

            decimal priceXLbsAux = inventoryMove.InventoryMoveDetail.Sum(s => s.exitAmountCost) / inventoryMove.ProductionLot.totalQuantityLiquidation;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;  
            var inventoryMoveDetailAux = inventoryMove.ProductionLot?.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("ILP")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
            InventoryMove lastInventoryMoveILP = (inventoryMoveDetailAux.Count > 0)
                                                                ? inventoryMoveDetailAux.First().InventoryMove
                                                                : null;
            if (lastInventoryMoveILP != null)
            {
                foreach (var inventoryMoveDetail in lastInventoryMoveILP.InventoryMoveDetail)
                {
                    var metricUnitAux = inventoryMoveDetail.Item.Presentation?.MetricUnit ?? inventoryMoveDetail.MetricUnit1;//inventoryMoveDetail.Item.ItemPurchaseInformation?.MetricUnit ?? db.MetricUnit.FirstOrDefault(fod => fod.code.Equals("Lbs"));
                    var factorConversion = (metricUnitAux.id != id_metricUnitLbsAux) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricUnitAux.id &&
                                                                                                                              fod.id_metricDestiny == id_metricUnitLbsAux)?.factor ?? 0 : 1;
                    decimal priceAux = 0;
                    if (factorConversion == 0)
                    {
                        throw new Exception("Falta de Factor de Conversión entre : " + metricUnitAux.code + " y " + (metricUnitUMTP?.code ?? "")/*"Lbs"*/ + ".Necesario para el recoste del precio Configúrelo, e intente de nuevo");

                    }
                    else
                    {
                        priceAux = priceXLbsAux * factorConversion * inventoryMoveDetail.Item.Presentation?.minimum ?? 1;
                    }

                    var id_metricUnitFormulationAux = inventoryMoveDetail.Item.ItemHeadIngredient?.MetricUnit?.id ?? inventoryMoveDetail.id_metricUnitMove;
                    var metricUnitFormulationAux = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitFormulationAux);

                    var factorConversionFormulation = (inventoryMoveDetail.id_metricUnitMove != metricUnitFormulationAux.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == inventoryMoveDetail.id_metricUnitMove &&
                                                                                                                                  fod.id_metricDestiny == metricUnitFormulationAux.id)?.factor ?? 0 : 1;
                    if (factorConversionFormulation == 0)
                    {
                        throw new Exception("Falta el Factor de Conversión entre : " + inventoryMoveDetail.MetricUnit1.code + " y " + metricUnitFormulationAux.code + ".Necesario para el precio Configúrelo, e intente de nuevo");
                    }
                    else
                    {
                        priceAux = priceAux * factorConversionFormulation;
                    }

                    if (inventoryMoveDetail.unitPrice != priceAux)
                    {
                        inventoryMoveDetail.unitPrice = priceAux;
                        db.InventoryMoveDetail.Attach(inventoryMoveDetail);
                        db.Entry(inventoryMoveDetail).State = EntityState.Modified;
                        ServiceInventoryMove.ReCosting(inventoryMoveDetail, db, ActiveUser);
                    }
                }

            }
            var id_productionLotAux = inventoryMove.ProductionLot?.id;
            var inventoryMoves = db.InventoryMove.Where(w => w.InventoryMoveDetail.Any(a => a.id_lot == id_productionLotAux) &&
                                       w.InventoryReason.code.Equals("EMPP")).ToList();
            List<InventoryMove> inventoryMovesAux = new List<InventoryMove>();
            foreach (var im in inventoryMoves)
            {
                var inventoryMovesAuxOrden = inventoryMoves.Where(w => w.id_productionLot == im.id_productionLot).OrderByDescending(d => d.Document.dateCreate).ToList();
                InventoryMove lastInventoryMoveEMPP = (inventoryMovesAuxOrden.Count > 0)
                                                       ? inventoryMovesAuxOrden.First()
                                                       : null;
                if (lastInventoryMoveEMPP != null && !inventoryMovesAux.Contains(lastInventoryMoveEMPP))
                {

                    ReCostProcess(lastInventoryMoveEMPP);
                    inventoryMovesAux.Add(lastInventoryMoveEMPP);
                }
            }

        }

        #endregion

        #region FORM DISTRIBUTED

        [HttpPost, ValidateInput(false)]

        public ActionResult FormShowDistributed(int id, int id_listPrice)
        {
            var productionLot = db.ProductionLotPayment.FirstOrDefault(e => e.id == id);
            List<ProductionLotPayment> productionLotPayment = db.ProductionLotPayment.Where(e => e.id == id).ToList();

            ViewBag.idPoductionLotReceptionPayment = id;
            ViewBag.IdProdutionLot = productionLot.id_productionLot;
            ViewBag.listaPrecio = id_listPrice;

            return PartialView("_ProductionLotReceptionEditFormProductionLotPaymentDetailPartialDistributed", productionLotPayment);
        }

        //Actualizar Registro
        [HttpPost, ValidateInput(false)]

        public ActionResult ProductionLotReceptionPartialUpdate(bool approve, ProductionLotPayment item)
        {
            ProductionLotPayment modelItem = db.ProductionLotPayment.FirstOrDefault(e => e.id == item.id);
            List<ProductionLotPayment> modelTotal = db.ProductionLotPayment.Where(e => e.id_productionLot == item.id_productionLot).ToList();


            if (modelItem != null)
            {
                ProductionLotPayment productionLotPayment = (TempData["productionLotPayment"] as ProductionLotPayment);
                decimal totalKg = 0m;
                decimal totalLb = 0m;
                decimal totalPrecioDis = 0m;
                decimal precioDis = 0m;
                decimal diferencia = 0m;

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (productionLotPayment?.ProductionLotPaymentDistributed != null)
                        {
                            var dbproductionLotDetail = modelItem.ProductionLotPaymentDistributed.ToList();
                            var uptproductionLotDetail = productionLotPayment.ProductionLotPaymentDistributed.ToList();

                            foreach (var productionLotReception in uptproductionLotDetail)
                            {
                                var oriproductionLotReception = dbproductionLotDetail.FirstOrDefault(r => r.id == productionLotReception.id);

                                if (oriproductionLotReception != null)
                                {
                                    oriproductionLotReception.isActive = productionLotReception.isActive;
                                }
                                if (productionLotReception.isActive)
                                {
                                    totalKg = (decimal)(totalKg + productionLotReception.kilogram);
                                    totalLb = totalLb + productionLotReception.pound;
                                    totalPrecioDis = totalPrecioDis + productionLotReception.totalPayLP;
                                    if (modelItem.id_metricUnitProcess == 4)
                                    {
                                        if (totalLb > 0)
                                        {
                                            precioDis = totalPrecioDis / totalLb;
                                        }
                                    }
                                    else
                                    {
                                        if (totalKg > 0)
                                        {
                                            precioDis = totalPrecioDis / totalKg;
                                        }
                                    }

                                    if (modelItem.totalPriceDis == null)
                                    {
                                        modelItem.totalPriceDis = 0;
                                    }
                                    diferencia = (decimal)(totalPrecioDis - modelItem.totalToPay);
                                }

                                if (oriproductionLotReception != null)
                                {
                                    oriproductionLotReception.id_item = productionLotReception.id_item;
                                    oriproductionLotReception.number_box = productionLotReception.number_box;
                                    oriproductionLotReception.kilogram = productionLotReception.kilogram;
                                    oriproductionLotReception.pound = productionLotReception.pound;
                                    oriproductionLotReception.performance = productionLotReception.performance;
                                    oriproductionLotReception.priceLP = productionLotReception.priceLP;
                                    oriproductionLotReception.totalPayLP = productionLotReception.totalPayLP;
                                    oriproductionLotReception.isActive = productionLotReception.isActive;
                                }
                                else
                                {
                                    var _tempProductionLotReception = new ProductionLotPaymentDistributed
                                    {
                                        id_item = productionLotReception.id_item,
                                        number_box = productionLotReception.number_box,
                                        kilogram = productionLotReception.kilogram,
                                        pound = productionLotReception.pound,
                                        performance = productionLotReception.performance,
                                        priceLP = productionLotReception.priceLP,
                                        totalPayLP = productionLotReception.totalPayLP,
                                        isActive = productionLotReception.isActive
                                    };
                                    modelItem.ProductionLotPaymentDistributed.Add(_tempProductionLotReception);
                                }
                            }

                        }

                        if (modelItem.totalKgs == 0)
                        {
                            modelItem.totalKgs = totalKg;
                            modelItem.totalLbs = totalLb;
                            modelItem.totalPriceDis = totalPrecioDis;
                            modelItem.priceDis = precioDis;
                            modelItem.differencia = diferencia;
                            modelItem.distributedd = (precioDis == 0) ? false : true;
                            modelItem.precioDisF = totalPrecioDis / modelItem.totalProcessMetricUnit;
                        }
                        else
                        {
                            modelItem.totalKgs = totalKg;
                            modelItem.totalLbs = totalLb;
                            modelItem.totalPriceDis = totalPrecioDis;
                            modelItem.priceDis = precioDis;
                            modelItem.differencia = diferencia;
                            modelItem.distributedd = true;
                            modelItem.precioDisF = totalPrecioDis / modelItem.totalProcessMetricUnit;
                        }

                        if (modelItem.id_metricUnitProcess == 4 && totalLb > modelItem.totalProcessMetricUnit)
                        {
                            ViewBag.idProductionLotPayment = modelItem.id;
                            ViewBag.idPoductionLotReceptionPayment = modelItem.id;
                            ViewBag.IdProdutionLot = modelItem.id_productionLot;
                            ViewBag.errorSave = true;

                            ViewData["EditMessage"] = ErrorMessage("Error: El total de Lb no puede ser mayor al rendimiento.");
                            throw new Exception("Error: El total de Lb no puede ser mayor al rendimiento.");
                        }
                        if (modelItem.id_metricUnitProcess == 1 && totalKg > modelItem.totalProcessMetricUnit)
                        {
                            ViewBag.idProductionLotPayment = modelItem.id;
                            ViewBag.idPoductionLotReceptionPayment = modelItem.id;
                            ViewBag.IdProdutionLot = modelItem.id_productionLot;
                            ViewBag.errorSave = true;

                            ViewData["EditMessage"] = ErrorMessage("Error: El total de Kg no puede ser mayor al rendimiento.");
                            throw new Exception("Error: El total de Kg no puede ser mayor al rendimiento.");
                        }

                        db.ProductionLotPayment.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Distribución de Precios guardado exitosamente");
                        ViewBag.idPoductionLotReceptionPayment = modelItem.id;
                        ViewBag.IdProdutionLot = modelItem.id_productionLot;
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("productionLotPayment");
                        TempData.Keep("id_productionLotPaymentType");

                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            return PartialView("_ProductionLotReceptionHeadFormProductionDistributedCostPartial", db.ProductionLotPayment.Where(e => e.id == item.id).ToList());
        }

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionPaymentDistributedDetail(int id_productionLotPayment, int id_productionLotPaymentType)
        {
            ProductionLotPayment productionLotPayment = ObtainProductionLotReceptionDistributed(id_productionLotPayment);

            List<ProductionLotPaymentDistributed> model = productionLotPayment.ProductionLotPaymentDistributed?.Where(r => r.id_productionLotPayment == id_productionLotPayment && r.isActive == true).ToList() ?? new List<ProductionLotPaymentDistributed>();

            if (model.Count() == 0)
            {
                model = db.ProductionLotPaymentDistributed.Where(e => e.id_productionLotPayment == id_productionLotPayment && e.isActive == true).ToList();
            }

            TempData["id_productionLotPaymentType"] = id_productionLotPaymentType;
            TempData.Keep("id_productionLotPaymentType");

            TempData["productionLotPayment"] = TempData["productionLotPayment"] ?? productionLotPayment;
            TempData.Keep("productionLotPayment");

            return PartialView("_ProductionLotReceptionLiquidationDistributedTableNewDetailPartial", model);
        }

        ///* ADD */
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotRecpetionDistributedDetailAddNew(int? id_productionLotPayment, ProductionLotPaymentDistributed productionLotPaymentDistributed)
        {
            ProductionLotPayment productionLotPayment = ObtainProductionLotReceptionDistributed(id_productionLotPayment);

            if (ModelState.IsValid)
            {
                try
                {
                    productionLotPaymentDistributed.id = productionLotPayment.ProductionLotPaymentDistributed.Count() > 0 ? productionLotPayment.ProductionLotPaymentDistributed.Max(e => e.id) + 1 : 1;

                    productionLotPaymentDistributed.isActive = true;

                    productionLotPayment.ProductionLotPaymentDistributed.Add(productionLotPaymentDistributed);
                    TempData["productionLotPayment"] = productionLotPayment;
                    TempData.Keep("productionLotPayment");
                    TempData.Keep("id_productionLotPaymentType");
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Por favor, corrija todos los errores.";
            }

            var model = productionLotPayment.ProductionLotPaymentDistributed.Where(v => v.isActive);
            if (model != null)
            {
                model = model.ToList();
            }
            else
            {
                model = new List<ProductionLotPaymentDistributed>();
            }
            return PartialView("_ProductionLotReceptionLiquidationDistributedTableNewDetailPartial", model);
        }

        //UPDATE
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotRecpetionDistributedDetailUpdate(ProductionLotPaymentDistributed productionLotPaymentDistributed)
        {
            ProductionLotPayment productionLotPayment = ObtainProductionLotReceptionDistributed(productionLotPaymentDistributed.id_productionLotPayment);

            List<ProductionLotPaymentDistributed> model = ProductionLotRecpetionDistributedDetailUpdateDelete(productionLotPayment, productionLotPaymentDistributed.id, false);

            TempData.Keep("productionLotPayment");
            TempData.Keep("id_productionLotPaymentType");

            return PartialView("_ProductionLotReceptionLiquidationDistributedTableNewDetailPartial", model);
        }

        private List<ProductionLotPaymentDistributed> ProductionLotRecpetionDistributedDetailUpdateDelete(ProductionLotPayment productionLotPayment, int id_productionLotPaymentDetail, Boolean isDelete)
        {
            ProductionLotPayment modelItem = db.ProductionLotPayment.FirstOrDefault(e => e.id == productionLotPayment.id);
            if (ModelState.IsValid && productionLotPayment != null)
            {
                try
                {
                    var modelProductionLotReceptionDetailDetail = productionLotPayment.ProductionLotPaymentDistributed.FirstOrDefault(i => i.id == id_productionLotPaymentDetail);
                    if (modelProductionLotReceptionDetailDetail != null)
                    {
                        if (isDelete) modelProductionLotReceptionDetailDetail.isActive = false;
                        this.UpdateModel(modelProductionLotReceptionDetailDetail);
                    }

                    TempData["productionLotPayment"] = productionLotPayment;
                    TempData.Keep("id_productionLotPaymentType");

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }

            }
            else
            {
                ViewData["EditError"] = "Por favor, corrija todos los errores.";
            }

            TempData.Keep("productionLotPayment");
            TempData.Keep("id_productionLotPaymentType");


            List<ProductionLotPaymentDistributed> model = productionLotPayment.ProductionLotPaymentDistributed?.Where(x => x.isActive).ToList() ?? new List<ProductionLotPaymentDistributed>();
            model = (model.Count() != 0) ? model : new List<ProductionLotPaymentDistributed>();

            return model;
        }

        //Delete
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotRecpetionDistributedDetailDelete(int id_productionLotPayment, int id)
        {
            ProductionLotPayment productionLotPayment = ObtainProductionLotReceptionDistributed(id_productionLotPayment);

            List<ProductionLotPaymentDistributed> model = ProductionLotRecpetionDistributedDetailUpdateDelete(productionLotPayment, id, true);

            TempData.Keep("productionLotPayment");
            TempData.Keep("id_productionLotPaymentType");

            return PartialView("_ProductionLotReceptionLiquidationDistributedTableNewDetailPartial", model);
        }

        //Validar Productos Repetidos
        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedDetail(int? id_codigoItem, int? id_Lot)
        {
            ProductionLotPayment productionLotPayment = (TempData["productionLotPayment"] as ProductionLotPayment);
            productionLotPayment = productionLotPayment ?? new ProductionLotPayment();

            if (productionLotPayment.id != id_Lot)
            {
                productionLotPayment = db.ProductionLotPayment.FirstOrDefault(e => e.id == id_Lot);
            }

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var codeProductionDistributed = productionLotPayment.ProductionLotPaymentDistributed.FirstOrDefault(e => e.id_item == id_codigoItem && e.isActive == true);
            if (codeProductionDistributed != null)
            {
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el producto en los detalles."
                };
            }

            TempData["productionLotPayment"] = productionLotPayment;
            TempData.Keep("productionLotPayment");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Cargar Datos de los GridViews
        public JsonResult LoadDatosGridViewReception(int? idCode, int id_price)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            string nameUmProcess;

            var db = new DBContext();

            //Obtene valor de la Lista de Precio de los registros Guardados
            ProductionLotPayment productionLotPayment = (TempData["productionLotPayment"] as ProductionLotPayment);
            productionLotPayment = productionLotPayment ?? new ProductionLotPayment();
            var valuePrice = productionLotPayment.ProductionLotPaymentDistributed.FirstOrDefault(e => e.id_item == idCode);

            decimal valorPrice = 0m;

            //Codigo

            var codeProductionLotReception = db.Item.Where(e => e.id == idCode).FirstOrDefault();
            var id_talla = codeProductionLotReception.ItemGeneral.ItemSize.id;

            var priceItem = db.PriceListItemSizeDetail.FirstOrDefault(e => e.Id_PriceList == id_price && e.Id_Itemsize == id_talla);

            if (priceItem != null)
            {
                valorPrice = priceItem.price;
            }
            else
            {
                if (valuePrice != null)
                {
                    valorPrice = valuePrice.priceLP;
                }
                else
                {
                    valorPrice = 0;
                }

            }

            if (codeProductionLotReception == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            if (codeProductionLotReception.ItemType.code == "C")
            {
                nameUmProcess = "KG";
            }
            else
            {
                nameUmProcess = "LB";
            }

            var result = new
            {
                codigoProducto = codeProductionLotReception.masterCode,
                nombreProducto = codeProductionLotReception.name,
                nombreCategoria = codeProductionLotReception.ItemTypeCategory.name,
                nombreTalla = codeProductionLotReception.ItemGeneral.ItemSize.name,
                nombreProceso = codeProductionLotReception.ItemType.name,
                nombreUMProceso = nameUmProcess,
                nombreUMPresentacion = codeProductionLotReception.Presentation.MetricUnit.name,
                preiceListItem = valorPrice
            };

            TempData["productionLotPayment"] = productionLotPayment;
            TempData.Keep("productionLotPayment");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDatosGridViewReceptionKg(int? idCode, int? numberB, string nombreP, string nombrePro)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //Codigo
            var db = new DBContext();
            var codeProductionLotReception = db.Item.Where(e => e.id == idCode).FirstOrDefault();

            if (codeProductionLotReception == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var valorMaximoPresentacion = codeProductionLotReception.Presentation.maximum;
            var valorMinimoPresentacion = codeProductionLotReception.Presentation.minimum;
            var total = numberB * valorMaximoPresentacion * valorMinimoPresentacion;
            decimal? totalKg;
            decimal? rendimiento = 0m;

            if (nombreP == "Kilogramos")
            {
                totalKg = total;
            }
            else
            {
                totalKg = (total) / (decimal)2.2046;
            }

            if (nombrePro == "KG")
            {
                rendimiento = totalKg;
            }

            var result = new
            {
                totalKilo = totalKg,
                rendimientoTotal = rendimiento
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDatosGridViewReceptionLbs(int? idCode, int? numberB, string nombreP, string nombrePro)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //Codigo
            var db = new DBContext();
            var codeProductionLotReception = db.Item.Where(e => e.id == idCode).FirstOrDefault();

            if (codeProductionLotReception == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var valorMaximoPresentacion = codeProductionLotReception.Presentation.maximum;
            var valorMinimoPresentacion = codeProductionLotReception.Presentation.minimum;
            var total = numberB * valorMaximoPresentacion * valorMinimoPresentacion;
            decimal? totalLb;
            decimal? rendimiento = 0m;

            if (nombreP == "Libras")
            {
                totalLb = total;
            }
            else
            {
                totalLb = (total) * (decimal)2.2046;
            }

            if (nombrePro == "LB")
            {
                rendimiento = totalLb;
            }

            var result = new
            {
                totalLibras = totalLb,
                rendimientoTotal = rendimiento
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionDistributedDetailChangePartial(int id_productionLotPaymentType, int id_productionLotPayment)
        {
            ProductionLotPayment productionLotPayment = ObtainProductionLotReceptionDistributed(id_productionLotPayment);
            var model = productionLotPayment.ProductionLotPaymentDistributed?.Where(r => r.isActive).ToList() ?? new List<ProductionLotPaymentDistributed>();

            TempData["productionLotPayment"] = productionLotPayment;
            TempData.Keep("productionLotPayment");
            TempData["id_productionLotPayment"] = id_productionLotPaymentType;
            TempData.Keep("id_productionLotPayment");

            return PartialView("_ProductionLotReceptionLiquidationDistributedTableNewDetailPartial", model);
        }

        private ProductionLotPayment ObtainProductionLotReceptionDistributed(int? id_productionLotPayment)
        {

            ProductionLotPayment productionLotPayment = (TempData["productionLotPayment"] as ProductionLotPayment);

            productionLotPayment = productionLotPayment ?? db.ProductionLotPayment.FirstOrDefault(i => i.id == id_productionLotPayment);
            productionLotPayment = productionLotPayment ?? new ProductionLotPayment();

            return productionLotPayment;

        }

        #endregion

        #region ITEMS

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormItemsDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            var productionLotDetails = db.ProductionLotDetail.Where(r => r.id_productionLot == productionLot.id);
            foreach (var productionLotDetail in productionLot.ProductionLotDetail)
            {
                var _idproductionLotDetail = db.ProductionLotDetail.FirstOrDefault(r => r.id == productionLotDetail.id);
                if (_idproductionLotDetail != null)
                {
                    productionLotDetail.quantitydrained = _idproductionLotDetail.quantitydrained;
                }


            }

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormItemsDetailAddNew(ProductionLotDetail item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();

            if (ModelState.IsValid)
            {
                item.id = productionLot.ProductionLotDetail.Count() > 0 ? productionLot.ProductionLotDetail.Max(pld => pld.id) + 1 : 1;
                item.Item = db.Item.FirstOrDefault(fod => fod.id == item.id_item);
                item.ProductionLotDetailPurchaseDetail = new List<ProductionLotDetailPurchaseDetail>();

                item.ProductionLotDetailPurchaseDetail.Add(new ProductionLotDetailPurchaseDetail
                {
                    id_purchaseOrderDetail = db.PurchaseOrder.FirstOrDefault(fod => fod.id == item.id_purchaseOrder).PurchaseOrderDetail.FirstOrDefault().id,
                    id_remissionGuideDetail = db.RemissionGuide.FirstOrDefault(fod => fod.id == item.id_remissionGuide).RemissionGuideDetail.FirstOrDefault().id,
                    quanty = item.quantityRecived
                });


                //var aSettingANL = db.Setting.FirstOrDefault(fod => fod.code == "ANALXLOT");
                //if (aSettingANL != null && aSettingANL.value == "SI")
                //{
                //    var id_productionLotDetailAuxTemp = db.ProductionLotDetail.Where(r => r.id_productionLot == productionLot.id).FirstOrDefault().id;
                //    var productionLotDetailQualityControl = db.ProductionLotDetailQualityControl.Where(r => r.id_productionLotDetail == id_productionLotDetailAuxTemp && r.IsDelete).FirstOrDefault();
                //    if (productionLotDetailQualityControl != null)
                //    {
                //        //var id_productionLotDetailAux = item.ProductionLotDetailQualityControl.Where(r => !r.IsDelete).FirstOrDefault();

                //        //var tempProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == item.id_productionLot);
                //        var tempProductionLotDetail = db.ProductionLotDetail.Where(r => r.id_productionLot == productionLot.id).FirstOrDefault();
                //        if (tempProductionLotDetail != null)
                //        {
                //            tempProductionLotDetail.ProductionLotDetailQualityControl = new List<ProductionLotDetailQualityControl>();
                //            tempProductionLotDetail.ProductionLotDetailQualityControl.Add(new ProductionLotDetailQualityControl
                //            {
                //                id_qualityControl = productionLotDetailQualityControl.id_qualityControl,
                //                //QualityControl = qualityControl,
                //                id_productionLotDetail = item.id
                //            });
                //        }

                //    }
                //}

                item.ValidateDrainingTest(db);

                productionLot.ProductionLotDetail.Add(item);
                UpdateProductionLotTotals(productionLot);
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            return PartialView("_ProductionLotReceptionEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormItemsDetailUpdate(ProductionLotDetail item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotDetail.FirstOrDefault(it => it.id == item.id);//var modelItem = productionLot.ProductionLotDetail.FirstOrDefault(it => it.id_item == item.id_item);
                    if (modelItem != null)
                    {
                        if (modelItem.ProductionLotDetailPurchaseDetail != null && modelItem.ProductionLotDetailPurchaseDetail.Count > 0)
                        {
                            int? id_purchaseOrderDetail = modelItem.ProductionLotDetailPurchaseDetail.ToList()[0].id_purchaseOrderDetail;
                            int? id_remissionGuideDetail = modelItem.ProductionLotDetailPurchaseDetail.ToList()[0].id_remissionGuideDetail;
                            PurchaseOrderDetail purchaseOrderDetail = DataProviderPurchaseOrder.PurchaseOrderDetail(id_purchaseOrderDetail);
                            RemissionGuideDetail remissionGuideDetail = DataProviderRemissionGuide.RemissionGuideDetail(id_remissionGuideDetail);
                            if (purchaseOrderDetail.id_purchaseOrder == modelItem.id_purchaseOrder &&
                                remissionGuideDetail.id_remisionGuide == modelItem.id_remissionGuide)
                            {
                                modelItem.ProductionLotDetailPurchaseDetail.ToList()[0].quanty = modelItem.quantityRecived;
                            }
                            else
                            {
                                modelItem.ProductionLotDetailPurchaseDetail = new List<ProductionLotDetailPurchaseDetail>();

                                modelItem.ProductionLotDetailPurchaseDetail.Add(new ProductionLotDetailPurchaseDetail
                                {
                                    id_purchaseOrderDetail = db.PurchaseOrder.FirstOrDefault(fod => fod.id == item.id_purchaseOrder).PurchaseOrderDetail.FirstOrDefault().id,
                                    id_remissionGuideDetail = db.RemissionGuide.FirstOrDefault(fod => fod.id == item.id_remissionGuide).RemissionGuideDetail.FirstOrDefault().id,
                                    quanty = modelItem.quantityRecived
                                });
                            }

                        }

                        this.UpdateModel(modelItem);
                        modelItem.ValidateDrainingTest(db);
                        UpdateProductionLotTotals(productionLot);
                        TempData["productionLotReception"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            return PartialView("_ProductionLotReceptionEditFormItemsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormItemsDetailDelete(int id)//System.Int32 id_item)
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            //if (id_item >= 0)
            //{
            try
            {
                var productionLotDetails = productionLot.ProductionLotDetail.FirstOrDefault(p => p.id == id);//var productionLotDetails = productionLot.ProductionLotDetail.FirstOrDefault(p => p.id_item == id_item);
                if (productionLotDetails != null)
                {
                    //if (productionLotDetails.ProductionLotDetailPurchaseDetail.Count() > 0)
                    //{
                    //    TempData.Keep("productionLotReception");
                    //    // throw (new Exception("Este Ítem de Materia Prima no se puede eliminar debido a que fue cargado desde una Orden de Compra"));


                    //    //     ViewData["EditMessage"] = ErrorMessage("Este Ítem de Materia Prima no se puede eliminar debido a que fue cargado desde una Orden de Compra");
                    //    ViewData["EditError"] = ErrorMessage("Este Ítem de Materia Prima no se puede eliminar debido ya que fue cargado desde una Orden de Compra");
                    //    var modelError = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
                    //    return PartialView("_ProductionLotReceptionEditFormItemsDetailPartial", modelError);
                    //}
                    string settLE = DataProviderSetting.ValueSetting("ULERMP");


                    if (settLE.Equals("Y") || productionLotDetails.ResultProdLotReceptionDetail != null)
                    {
                        if (productionLotDetails.ResultProdLotReceptionDetail != null && ((productionLotDetails.quantitydrained ?? 0) > 0))
                        {
                            TempData.Keep("productionLotReception");
                            ViewData["EditError"] = ErrorMessage("Este Ítem de Materia Prima no se puede eliminar debido a tener prueba de Escurrido");
                            var modelError = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
                            return PartialView("_ProductionLotReceptionEditFormItemsDetailPartial", modelError);
                        }
                    }




                    var id_qualityControlAux = productionLotDetails.ProductionLotDetailQualityControl
                                                                                .FirstOrDefault(fod => fod.IsDelete == false
                                                                                                       && fod.QualityControl.Document.DocumentState.code != ("05"))?.id_qualityControl;//"05": Anulado

                    if (id_qualityControlAux != null)
                    {
                        TempData.Keep("productionLotReception");
                        ViewData["EditError"] = ErrorMessage("Este Ítem de Materia Prima no se puede eliminar debido a tener Análisis de calidad");
                        var modelError = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
                        return PartialView("_ProductionLotReceptionEditFormItemsDetailPartial", modelError);
                    }
                    if (id_qualityControlAux != null)
                    {
                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");//ANULADA

                                var qualityControlAux = db.QualityControl.FirstOrDefault(fod => fod.id == id_qualityControlAux);
                                qualityControlAux.Document.id_documentState = documentState.id;
                                qualityControlAux.Document.DocumentState = documentState;

                                db.QualityControl.Attach(qualityControlAux);
                                db.Entry(qualityControlAux).State = EntityState.Modified;

                                db.SaveChanges();
                                trans.Commit();

                            }
                            catch (Exception e)
                            {
                                //ViewData["EditError"] = e.Message;
                                trans.Rollback();
                                throw e;
                            }
                        }
                    }

                    productionLot.ProductionLotDetail.Remove(productionLotDetails);
                }


                UpdateProductionLotTotals(productionLot);
                TempData["productionLotReception"] = productionLot;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            return PartialView("_ProductionLotReceptionEditFormItemsDetailPartial", model);
        }

        #endregion

        #region MATERIALS

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormMaterialsDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotDispatchMaterial.ToList() ?? new List<ProductionLotDispatchMaterial>();

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionEditFormMaterialsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormMaterialsDetailAddNew(ProductionLotDispatchMaterial item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotDispatchMaterial = productionLot.ProductionLotDispatchMaterial ?? new List<ProductionLotDispatchMaterial>();

            if (ModelState.IsValid)
            {
                item.id = productionLot.ProductionLotDispatchMaterial.Count() > 0 ? productionLot.ProductionLotDispatchMaterial.Max(pld => pld.id) + 1 : 1;
                productionLot.ProductionLotDispatchMaterial.Add(item);
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotDispatchMaterial.ToList() ?? new List<ProductionLotDispatchMaterial>();

            return PartialView("_ProductionLotReceptionEditFormMaterialsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormMaterialsDetailUpdate(ProductionLotDispatchMaterial item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotDispatchMaterial.FirstOrDefault(it => it.id == item.id);//var modelItem = productionLot.ProductionLotDispatchMaterial.FirstOrDefault(it => it.id_item == item.id_item);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        TempData["productionLotReception"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotDispatchMaterial.ToList() ?? new List<ProductionLotDispatchMaterial>();

            return PartialView("_ProductionLotReceptionEditFormMaterialsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormMaterialsDetailDelete(System.Int32 id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            //if (id >= 0)
            //{
            try
            {
                var productionLotDispatchMaterial = productionLot.ProductionLotDispatchMaterial.FirstOrDefault(p => p.id == id);//var productionLotDispatchMaterial = productionLot.ProductionLotDispatchMaterial.FirstOrDefault(p => p.id_item == id_item);
                if (productionLotDispatchMaterial != null)
                    productionLot.ProductionLotDispatchMaterial.Remove(productionLotDispatchMaterial);

                TempData["productionLotReception"] = productionLot;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotDispatchMaterial.ToList() ?? new List<ProductionLotDispatchMaterial>();

            return PartialView("_ProductionLotReceptionEditFormMaterialsDetailPartial", model);
        }

        #endregion

        #region PRODUCTION LOT LIQUIDATION

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotLiquidationsDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionEditFormProductionLotLiquidationsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotLiquidationsDetailAddNew(int? id_salesOrder, ProductionLotLiquidation productionLotLiquidation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();

            if (ModelState.IsValid)
            {
                productionLotLiquidation.id = productionLot.ProductionLotLiquidation.Count() > 0 ? productionLot.ProductionLotLiquidation.Max(pld => pld.id) + 1 : 1;
                productionLotLiquidation.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id_salesOrder == id_salesOrder && fod.id_item == productionLotLiquidation.id_item);
                productionLotLiquidation.id_salesOrderDetail = productionLotLiquidation.SalesOrderDetail?.id;

                productionLotLiquidation.Item = db.Item.FirstOrDefault(i => i.id == productionLotLiquidation.id_item);
                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>();

                productionLot.ProductionLotLiquidation.Add(productionLotLiquidation);
                UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, productionLotLiquidation);
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("_ProductionLotReceptionEditFormProductionLotLiquidationsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotLiquidationsDetailUpdate(int? id_salesOrder, ProductionLotLiquidation productionLotLiquidation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotLiquidation.FirstOrDefault(it => it.id == productionLotLiquidation.id);//var modelItem = productionLot.ProductionLotLiquidation.FirstOrDefault(pll => pll.id_item == productionLotLiquidation.id_item);
                    if (modelItem != null)
                    {
                        modelItem.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id_salesOrder == id_salesOrder && fod.id_item == productionLotLiquidation.id_item);
                        modelItem.id_salesOrderDetail = productionLotLiquidation.SalesOrderDetail?.id;
                        this.UpdateModel(modelItem);
                        UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                        modelItem.Item = db.Item.FirstOrDefault(i => i.id == productionLotLiquidation.id_item);

                        UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, modelItem);

                        TempData["productionLotReception"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("_ProductionLotReceptionEditFormProductionLotLiquidationsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotLiquidationsDetailDelete(System.Int32 id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();

            //if (id_item >= 0)
            //{
            try
            {
                var productionLotLiquidation = productionLot.ProductionLotLiquidation.FirstOrDefault(p => p.id == id);//var productionLotLiquidation = productionLot.ProductionLotLiquidation.FirstOrDefault(p => p.id_item == id_item);
                if (productionLotLiquidation != null)
                    productionLot.ProductionLotLiquidation.Remove(productionLotLiquidation);

                UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, productionLotLiquidation);

                TempData["productionLotReception"] = productionLot;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();
            return PartialView("_ProductionLotReceptionEditFormProductionLotLiquidationsDetailPartial", model);
        }

        private void UpdateProductionLotProductionLotLiquidationsDetailTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityLiquidation = 0.0M;
            productionLot.wholeSubtotal = 0.0M;
            productionLot.subtotalTail = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotLiquidation in productionLot.ProductionLotLiquidation)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_item);
                var id_metricUnitAux = productionLotLiquidation.id_metricUnitPresentation;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                var factorlib = decimal.Truncate(((ItemAux.Presentation?.minimum ?? 1) * factor) * 100000m) / 100000m;
                //var valueAux = decimal.Truncate(productionLotLiquidation.quantityPoundsLiquidation.Value*100)/100;
                var valueAux = Math.Round((productionLotLiquidation.quantity * factorlib), 2);
                //var valueAux = decimal.Round(productionLotLiquidation.quantityPoundsLiquidation.Value, 2);
                productionLot.totalQuantityLiquidation += valueAux;
                //if (productionLotLiquidation.Item.ItemTypeCategory.code == "ENT")
                var codeAux = productionLotLiquidation.Item?.ItemType?.ProcessType?.code ?? "";
                if (codeAux == "ENT")
                {
                    productionLot.wholeSubtotal += valueAux;
                }
                else
                {
                    productionLot.subtotalTail += valueAux;
                }

            }
            //    productionLot.totalQuantityLiquidation += productionLotLiquidation.quantityTotal.Value * factor;
            //}
            //productionLot.totalQuantityLiquidation = decimal.Round(productionLot.totalQuantityLiquidation, 2);

        }

        private void UpdateProductionLotProductionLotLiquidationTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityLiquidation = 0.0M;
            productionLot.wholeSubtotal = 0.0M;
            productionLot.subtotalTail = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotLiquidationTotal in productionLot.ProductionLotLiquidationTotal)
            {
                //var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidationTotal.id_ItemLiquidation);
                //var id_metricUnitAux = productionLotLiquidationTotal.id_metricUnitPresentation;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                //var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                //                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                //                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                //var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                //var valueAux = decimal.Truncate(productionLotLiquidationTotal.quantityPoundsIL * 100) / 100;
                //var factorlib = decimal.Truncate(((ItemAux.Presentation?.minimum ?? 1) * factor) * 100000m) / 100000m;
                //var valueAux = Math.Round((productionLotLiquidationTotal.quatityBoxesIL * factorlib),2);
                //var valueAux = decimal.Truncate((productionLotLiquidationTotal.quatityBoxesIL * factorlib) * 100) / 100;
                var valueAux = productionLotLiquidationTotal.quantityPoundsIL;
                //productionLotLiquidationTotal.quantityPoundsIL = valueAux;

                productionLot.totalQuantityLiquidation += valueAux;

                var codeAux = productionLotLiquidationTotal.Item?.ItemType?.ProcessType?.code ?? "";
                if (codeAux == "ENT")
                {
                    productionLot.wholeSubtotal += valueAux;
                }
                else
                {
                    productionLot.subtotalTail += valueAux;
                }

            }

        }

        [HttpPost]
        public ActionResult GetCopackingTariffListForPayment(int? liquidationPayDateYear, int? liquidationPayDateMonth, int? liquidationPayDateDay)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);



            productionLot = productionLot ?? new ProductionLot();


            TempData.Keep("productionLotReception");

            DateTime dtNow = DateTime.Now;
            if (liquidationPayDateYear > 0 && liquidationPayDateMonth >= 0 && liquidationPayDateDay > 0)
            {
                int monthTmp = (int)liquidationPayDateMonth + 1;
                dtNow = new DateTime((int)liquidationPayDateYear, monthTmp, (int)liquidationPayDateDay);
            }

            int id_comp = (int)ViewData["id_company"];


            var priceListAux = db.CopackingTariff
                                    .Where(t => t.id_company == id_comp && (t.id_provider == productionLot.id_provider) && t.isActive
                                          ).ToList();

            //ojo con esto
            priceListAux = priceListAux.AsEnumerable()
                                        .Where(w => DateTime.Compare(w.dateInit.Date, dtNow.Date) <= 0
                                                && DateTime.Compare(dtNow.Date, w.dateEnd.Date) <= 0).ToList();

            var priceListCurrentAux = db.CopackingTariff.FirstOrDefault(fod => fod.id == productionLot.id_CopackingTariff);

            if (priceListCurrentAux != null && !priceListAux.Contains(priceListCurrentAux)) priceListAux.Add(priceListCurrentAux);

            var priceListAuxNew = priceListAux.Select(s => new
            {
                s.id,
                name = s.name
                + " [" + s.dateInit.ToString("dd/MM/yyyy") + " - "
                + s.dateEnd.ToString("dd/MM/yyyy") + "]"
            }).OrderBy(t => t.id).ToList();

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.Width = Unit.Percentage(100);

                p.ValueField = "id";
                p.TextField = "name";
                p.DataSource = DataProviderCopackingTariff.CopackingTariffByCompanyWithCurrentAndProviderForLiquidation((int?)ViewData["id_company"], productionLot.id_provider, dtNow);
                p.ValueType = typeof(string);

                p.ClientInstanceName = "id_priceList";


                p.ValidationSettings.RequiredField.IsRequired = true;
                p.ValidationSettings.RequiredField.ErrorText = "Campo Obligatorio";
                p.ValidationSettings.CausesValidation = true;
                p.ValidationSettings.ErrorDisplayMode = DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
                p.ValidationSettings.ValidateOnLeave = true;
                p.ValidationSettings.SetFocusOnError = true;
                p.ValidationSettings.ErrorText = "Valor Incorrecto";
                p.CallbackRouteValues = new { Controller = "ProductionLotReception", Action = "GetCopackingTariffListForPayment" };

                p.ClientSideEvents.BeginCallback = "ProductionLotPriceListPayment_BeginCallback";

                p.ValidationSettings.EnableCustomValidation = true;
                p.DropDownStyle = DropDownStyle.DropDown;
                p.ClientSideEvents.Validation = "OnPriceListValidation";
                p.ClientSideEvents.SelectedIndexChanged = "ComboCopackingTariff_SelectedIndexChanged";
                //p.BindList(priceListAuxNew);

            });
        }

        [HttpPost]
        public ActionResult GetPriceListForPayment(int? liquidationPayDateYear, int? liquidationPayDateMonth, int? liquidationPayDateDay)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);



            productionLot = productionLot ?? new ProductionLot();


            TempData.Keep("productionLotReception");

            DateTime dtNow = DateTime.Now;
            if (liquidationPayDateYear > 0 && liquidationPayDateMonth >= 0 && liquidationPayDateDay > 0)
            {
                int monthTmp = (int)liquidationPayDateMonth + 1;
                dtNow = new DateTime((int)liquidationPayDateYear, monthTmp, (int)liquidationPayDateDay);
            }

            int id_comp = (int)ViewData["id_company"];


            var priceListAux = db.PriceList
                                    .Where(t => t.id_company == id_comp
                                                && (t.Document.DocumentState.code.Equals("02")
                                                || t.Document.DocumentState.code.Equals("03")
                                                || t.Document.DocumentState.code.Equals("04"))
                                                && t.isForPurchase
                                                && ((t.isQuotation && (((t.byGroup == null || t.byGroup == false)
                                                && t.id_provider == productionLot.id_provider)
                                                || (t.byGroup == true
                                                && t.GroupPersonByRol.GroupPersonByRolDetail.FirstOrDefault(fod => fod.id_person == productionLot.id_provider) != null))))
                                                ).ToList();


            string varParSys1 = DataProviderSetting.ValueSetting("FLPEC");
            if (!string.IsNullOrEmpty(varParSys1) && varParSys1.Equals("0"))
            {
                priceListAux = priceListAux.Where(w => w.Document.DocumentState.code.Equals("03")
                || w.Document.DocumentState.code.Equals("02")).ToList();
            }
            //ojo con esto
            priceListAux = priceListAux.AsEnumerable()
                                        .Where(w => DateTime.Compare(w.startDate.Date, dtNow.Date) <= 0
                                                && DateTime.Compare(dtNow.Date, w.endDate.Date) <= 0).ToList();

            var priceListCurrentAux = db.PriceList.FirstOrDefault(fod => fod.id == productionLot.id_priceList);

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

            var priceListAuxNew = priceListAux.Select(s => new
            {
                s.id,
                name = s.name + " Estado: " + s.Document.DocumentState.name + " (" + s.Document.DocumentType.name + ") "
                + s.CalendarPriceList.CalendarPriceListType.name
                + " [" + s.startDate.ToString("dd/MM/yyyy") + " - "
                + s.endDate.ToString("dd/MM/yyyy") + "]" + " PROCESO: "
                + s.ProcessType.name + (s.Document.authorizationDate.HasValue ? " GERENCIA GENERAL ["
                + s.Document.authorizationDate.Value.ToString("dd/MM/yyyy hh:mm:ss") + "]" : "")
            }).OrderBy(t => t.id).ToList();

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.Width = Unit.Percentage(100);

                p.ValueField = "id";
                p.TextField = "name";
                p.DataSource = DataProviderPriceList.PriceListByCompanyWithCurrentAndProviderForLiquidation((int?)ViewData["id_company"], productionLot.id_priceList, productionLot.id_provider, dtNow);
                p.ValueType = typeof(string);

                p.ClientInstanceName = "id_priceList";


                p.ValidationSettings.RequiredField.IsRequired = true;
                p.ValidationSettings.RequiredField.ErrorText = "Campo Obligatorio";
                p.ValidationSettings.CausesValidation = true;
                p.ValidationSettings.ErrorDisplayMode = DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
                p.ValidationSettings.ValidateOnLeave = true;
                p.ValidationSettings.SetFocusOnError = true;
                p.ValidationSettings.ErrorText = "Valor Incorrecto";
                p.CallbackRouteValues = new { Controller = "ProductionLotReception", Action = "GetPriceListForPayment" };

                p.ClientSideEvents.BeginCallback = "ProductionLotPriceListPayment_BeginCallback";

                p.ValidationSettings.EnableCustomValidation = true;
                p.DropDownStyle = DropDownStyle.DropDown;
                p.ClientSideEvents.Validation = "OnPriceListValidation";
                p.ClientSideEvents.SelectedIndexChanged = "ComboPriceList_SelectedIndexChanged";
                //p.BindList(priceListAuxNew);

            });
        }
        #endregion

        #region PRODUCTION LOT PACKING MATERIAL

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotPackingMaterialsPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotPackingMaterial = productionLot.ProductionLotPackingMaterial ?? new List<ProductionLotPackingMaterial>();

            var model = productionLot.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0);

            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotPackingMaterialsPartialAddNew(ProductionLotPackingMaterial productionLotPackingMaterial)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotPackingMaterial.FirstOrDefault(it => it.id_item == productionLotPackingMaterial.id_item);
                    if (modelItem != null)
                    {
                        modelItem.isActive = true;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);

                    }
                    else
                    {
                        productionLotPackingMaterial.Item = db.Item.FirstOrDefault(i => i.id == productionLotPackingMaterial.id_item);

                        productionLotPackingMaterial.isActive = true;
                        productionLotPackingMaterial.manual = true;
                        productionLotPackingMaterial.id_userCreate = ActiveUser.id;
                        productionLotPackingMaterial.dateCreate = DateTime.Now;
                        productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                        productionLotPackingMaterial.dateUpdate = DateTime.Now;

                        productionLot.ProductionLotPackingMaterial.Add(productionLotPackingMaterial);
                    }



                    TempData["productionLotReception"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();
            return PartialView("_ProductionLotReceptionEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotPackingMaterialsPartialUpdate(ProductionLotPackingMaterial productionLotPackingMaterial)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotPackingMaterial.FirstOrDefault(it => it.id_item == productionLotPackingMaterial.id_item);
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        TempData["productionLotReception"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();

            return PartialView("_ProductionLotReceptionEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotPackingMaterialsPartialDelete(System.Int32 id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (id_item >= 0)
            {
                try
                {
                    var productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.FirstOrDefault(p => p.id_item == id_item);
                    if (productionLotPackingMaterial != null)
                    {
                        if (!productionLotPackingMaterial.manual)
                        {
                            throw (new Exception("Este Ítem de Materiales de Empaque no se puede eliminar debido a que fue cargado desde la formulación de algún producto de la liquidación"));
                        }

                        productionLotPackingMaterial.isActive = false;
                        productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                        productionLotPackingMaterial.dateUpdate = DateTime.Now;
                    }

                    TempData["productionLotReception"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();
            return PartialView("_ProductionLotReceptionEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedProductionLotPackingMaterials(int[] ids)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ids != null)
            {
                try
                {
                    var productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.Where(i => ids.Contains(i.id_item));

                    foreach (var detail in productionLotPackingMaterial)
                    {
                        if (detail.manual)
                        {
                            detail.isActive = false;
                            detail.id_userUpdate = ActiveUser.id;
                            detail.dateUpdate = DateTime.Now;
                        }

                    }

                    TempData["productionLotReception"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("productionLotReception");
        }

        #endregion

        #region PRODUCTION LOT TRASH

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotTrashsDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotTrashsDetailAddNew(ProductionLotTrash productionLotTrash)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotTrash = productionLot.ProductionLotTrash ?? new List<ProductionLotTrash>();

            if (ModelState.IsValid)
            {
                productionLotTrash.id = productionLot.ProductionLotTrash.Count() > 0 ? productionLot.ProductionLotTrash.Max(pld => pld.id) + 1 : 1;
                productionLot.ProductionLotTrash.Add(productionLotTrash);
                UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            return PartialView("_ProductionLotReceptionEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotTrashsDetailUpdate(ProductionLotTrash productionLotTrash)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotTrash.FirstOrDefault(it => it.id == productionLotTrash.id);//var modelItem = productionLot.ProductionLotTrash.FirstOrDefault(pll => pll.id_item == productionLotTrash.id_item);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
                        TempData["productionLotReception"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            return PartialView("_ProductionLotReceptionEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotTrashsDetailDelete(System.Int32 id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            //if (id_item >= 0)
            //{
            try
            {
                var productionLotTrash = productionLot.ProductionLotTrash.FirstOrDefault(p => p.id == id);//var productionLotTrash = productionLot.ProductionLotTrash.FirstOrDefault(p => p.id_item == id_item);
                if (productionLotTrash != null)
                    productionLot.ProductionLotTrash.Remove(productionLotTrash);

                UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
                TempData["productionLotReception"] = productionLot;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();
            return PartialView("_ProductionLotReceptionEditFormProductionLotTrashsDetailPartial", model);
        }

        private void UpdateProductionLotProductionLotTrashsDetailTotals(ProductionLot productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            productionLot.totalQuantityTrash = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotTrash in productionLot.ProductionLotTrash)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotTrash.id_item);
                var id_metricUnitAux = productionLotTrash.id_metricUnit;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityTrash += productionLotTrash.quantity * factor;
            }
            productionLot.totalQuantityTrash = decimal.Round(productionLot.totalQuantityTrash, 2);

        }

        #endregion

        #region PRODUCTION LOT QUALITY ANALYSIS

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotQualityAnalysissDetailPartial()
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotQualityAnalysis.ToList() ?? new List<ProductionLotQualityAnalysis>();

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionEditFormProductionLotQualityAnalysisDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotQualityAnalysissDetailAddNew(ProductionLotQualityAnalysis productionLotQualityAnalysis)
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotQualityAnalysis = productionLot.ProductionLotQualityAnalysis ?? new List<ProductionLotQualityAnalysis>();

            if (ModelState.IsValid)
            {
                productionLot.ProductionLotQualityAnalysis.Add(productionLotQualityAnalysis);
                //UpdateProductionLotProductionLotQualityAnalysissDetailTotals(productionLot);
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotQualityAnalysis.ToList() ?? new List<ProductionLotQualityAnalysis>();

            return PartialView("_ProductionLotReceptionEditFormProductionLotQualityAnalysisDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotQualityAnalysissDetailUpdate(ProductionLotQualityAnalysis productionLotQualityAnalysis)
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelQualityAnalysis = productionLot.ProductionLotQualityAnalysis.FirstOrDefault(pll => pll.id_qualityAnalysis == productionLotQualityAnalysis.id_qualityAnalysis);
                    if (modelQualityAnalysis != null)
                    {
                        this.UpdateModel(modelQualityAnalysis);
                        //UpdateProductionLotProductionLotQualityAnalysissDetailTotals(productionLot);
                        TempData["productionLotReception"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotQualityAnalysis.ToList() ?? new List<ProductionLotQualityAnalysis>();

            return PartialView("_ProductionLotReceptionEditFormProductionLotQualityAnalysisDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormProductionLotQualityAnalysissDetailDelete(System.Int32 id_qualityAnalysis)
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (id_qualityAnalysis >= 0)
            {
                try
                {
                    var productionLotQualityAnalysis = productionLot.ProductionLotQualityAnalysis.FirstOrDefault(p => p.id_qualityAnalysis == id_qualityAnalysis);
                    if (productionLotQualityAnalysis != null)
                        productionLot.ProductionLotQualityAnalysis.Remove(productionLotQualityAnalysis);

                    //UpdateProductionLotProductionLotQualityAnalysissDetailTotals(productionLot);
                    TempData["productionLotReception"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotQualityAnalysis.ToList() ?? new List<ProductionLotQualityAnalysis>();
            return PartialView("_ProductionLotReceptionEditFormProductionLotQualityAnalysisDetailPartial", model);
        }

        //private void UpdateProductionLotProductionLotQualityAnalysissDetailTotals(ProductionLot productionLot)
        //{
        //    productionLot.totalQuantityTrash = 0.0M;

        //    foreach (var productionLotTrash in productionLot.ProductionLotTrash)
        //    {
        //        productionLot.totalQuantityTrash += productionLotTrash.quantity;
        //    }
        //}

        #endregion

        #region CLOSES

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormClosesDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotPayment.ToList() ?? new List<ProductionLotPayment>();

            ViewBag.totalQuantityLiquidationAdjust = productionLot.totalQuantityLiquidationAdjust;

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionEditFormProductionLotCloseDetailPartial", model.OrderBy(ob => ob.Item.ItemGeneral.ItemSize?.orderSize ?? 0).ToList());
            //return PartialView("_ProductionLotReceptionEditFormProductionLotCloseDetailPartial", model);
        }

        //[HttpPost, ValidateInput(false)]
        //public ActionResult ProductionLotProcessEditFormItemsDetailAddNew(ProductionLotDetail item)
        //{
        //    ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
        //    //productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
        //    productionLot = productionLot ?? new ProductionLot();
        //    productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();

        //    if (ModelState.IsValid)
        //    {
        //        item.id = productionLot.ProductionLotDetail.Count() > 0 ? productionLot.ProductionLotDetail.Max(pld => pld.id) + 1 : 1;
        //        ProductionLot productionOriginLot = db.ProductionLot.FirstOrDefault(pl => pl.id == item.id_originLot);
        //        item.id_warehouse =
        //            productionOriginLot.ProductionLotLiquidation.FirstOrDefault(pll => pll.id_item == item.id_item)
        //                .id_warehouse;
        //        item.id_warehouseLocation =
        //            productionOriginLot.ProductionLotLiquidation.FirstOrDefault(pll => pll.id_item == item.id_item)
        //                .id_warehouseLocation;
        //        productionLot.ProductionLotDetail.Add(item);
        //        UpdateProductionLotTotals(productionLot);
        //    }

        //    TempData["productionLotReception"] = productionLot;
        //    TempData.Keep("productionLotReception");

        //    var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

        //    return PartialView("_ProductionLotProcessEditFormItemsDetailPartial", model);
        //}

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormClosesDetailUpdate(ProductionLotPayment item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotPayment = productionLot.ProductionLotPayment ?? new List<ProductionLotPayment>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotPayment.FirstOrDefault(it => it.id == item.id);//it.id_originLot == item.id_originLot && it.id_item == item.id_item);
                    if (modelItem != null)
                    {
                        modelItem.adjustLess = item.adjustLess;
                        modelItem.adjustMore = item.adjustMore;
                        modelItem.totalPounds = item.totalPounds;
                        modelItem.totalMU = item.totalMU;
                        //modelItem.quantity = item.quantity;

                        //this.UpdateModel(modelItem);
                        UpdateProductionLotPerformance(productionLot);
                        TempData["productionLotReception"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLotReception");

            var model = productionLot?.ProductionLotPayment.ToList() ?? new List<ProductionLotPayment>();

            ViewBag.totalQuantityLiquidationAdjust = productionLot.totalQuantityLiquidationAdjust;
            return PartialView("_ProductionLotReceptionEditFormProductionLotCloseDetailPartial", model.OrderBy(ob => ob.Item.ItemGeneral.ItemSize?.orderSize ?? 0).ToList());
        }

        //[HttpPost, ValidateInput(false)]
        //public ActionResult ProductionLotProcessEditFormItemsDetailDelete(int id)//int id_originLot, int id_item)
        //{
        //    ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
        //    //productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
        //    productionLot = productionLot ?? new ProductionLot();
        //    productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();

        //    //if (id_originLot >= 0 && id_item >= 0)
        //    //{
        //    try
        //    {
        //        var productionLotDetails = productionLot.ProductionLotDetail.FirstOrDefault(p => p.id == id);//p.id_originLot == id_originLot && p.id_item == id_item);
        //        if (productionLotDetails != null)
        //        {
        //            productionLot.ProductionLotDetail.Remove(productionLotDetails);
        //            UpdateProductionLotTotals(productionLot);
        //        }

        //        TempData["productionLotReception"] = productionLot;
        //    }
        //    catch (Exception e)
        //    {
        //        ViewData["EditError"] = e.Message;
        //    }
        //    //}

        //    TempData.Keep("productionLotReception");

        //    var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
        //    return PartialView("_ProductionLotProcessEditFormItemsDetailPartial", model);
        //}

        private void UpdateProductionLotPerformance(ProductionLot productionLot)
        {
            productionLot.totalQuantityLiquidationAdjust = 0.0M;
            productionLot.totalAdjustmentPounds = 0.0M;
            productionLot.totalAdjustmentWholePounds = 0.0M;
            productionLot.totalAdjustmentTailPounds = 0.0M;
            productionLot.wholeSubtotalAdjust = 0.0M;
            productionLot.subtotalTailAdjust = 0.0M;
            productionLot.wholeSubtotalAdjustProcess = 0.0M;
            productionLot.wholeTotalToPay = 0.0M;
            productionLot.tailTotalToPay = 0.0M;
            productionLot.totalToPay = 0.0M;
            productionLot.totalToPayEq = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var metricUnitUMEPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMEP"));
            var id_metricUnitUMEPValueAux = int.Parse(metricUnitUMEPAux?.value ?? "0");
            var metricUnitUMEP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMEPValueAux);


            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;
            var id_metricUnitKgAux = metricUnitUMEP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotPayment in productionLot.ProductionLotPayment)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotPayment.id_item);
                var id_metricUnitAux = productionLotPayment.id_metricUnit;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var id_metricUnitProcessAux = productionLotPayment.id_metricUnitProcess;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                var factorlib = decimal.Truncate(((ItemAux.Presentation?.minimum ?? 1) * factor) * 100000m) / 100000m;
                //var factorlib = ((ItemAux.Presentation?.minimum ?? 1) * factor);
                //var factorCantidad = decimal.Truncate((productionLotPayment.quantity / (ItemAux.Presentation?.minimum ?? 1))*100)/100;
                //var factorCantidadAjustada = decimal.Truncate((productionLotPayment.totalMU / (ItemAux.Presentation?.minimum ?? 1))*100)/100;
                var quantityAux = Math.Round((productionLotPayment.quantityEntered.Value * factorlib), 2);
                //var quantityAux = decimal.Truncate((productionLotPayment.quantityEntered.Value * factorlib) * 100) / 100;
                var adjustAux = (productionLotPayment.adjustMore ?? 0) + (productionLotPayment.adjustLess ?? 0);
                var totalMUAux = Math.Round((quantityAux + (adjustAux * factor)), 2);
                //var totalMUAux = decimal.Truncate((quantityAux + (adjustAux * factor)) * 100) / 100;
                //            var quantityAux = decimal.Round((factorCantidad * factorlib), 2);
                //var totalMUAux = decimal.Round((factorCantidadAjustada * factorlib), 2);

                metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                   muc.id_metricOrigin == id_metricUnitAux &&
                                                                                   muc.id_metricDestiny == id_metricUnitKgAux);
                factor = id_metricUnitKgAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                var metricUnitConversionProcess = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitProcessAux);

                productionLotPayment.totalPounds = totalMUAux;
                var fitPoundsAux = totalMUAux - quantityAux;
                productionLotPayment.fitPounds = fitPoundsAux;

                productionLot.totalQuantityLiquidationAdjust += totalMUAux;
                productionLot.totalAdjustmentPounds += fitPoundsAux;

                //var priceAux = decimal.Round((productionLotPayment.price), 2);
                //productionLotPayment.totalToPay = decimal.Round((priceAux * productionLotPayment.totalProcessMetricUnit), 2);
                productionLot.totalToPay += productionLotPayment.totalToPay;
                productionLot.totalToPayEq += productionLotPayment.totalToPayEq;

                var ItemEqAux = db.Item.FirstOrDefault(fod => fod.id == ItemAux.ItemEquivalence.id_itemEquivalence);

                var factorEq = decimal.Truncate(((ItemEqAux.Presentation?.minimum ?? 1)) * 100000m) / 100000m;
                var totalMUEqAux = Math.Round((productionLotPayment.quantityEntered.Value * factorEq * factor), 2);
                var totalMUEqColAux = (id_metricUnitAux == 1) ? Math.Round((productionLotPayment.quantityEntered.Value * factorEq * metricUnitConversionProcess?.factor ?? 0), 2) : Math.Round((productionLotPayment.quantityEntered.Value * factorEq), 2);

                //if (ItemAux.ItemTypeCategory.code == "ENT")
                //{
                var codeAux = ItemAux?.ItemType?.ProcessType?.code ?? "";
                if (codeAux == "ENT")
                {
                    productionLot.totalAdjustmentWholePounds += fitPoundsAux;
                    productionLot.wholeSubtotalAdjust += totalMUAux;
                    var totalMUAux2 = decimal.Round(productionLotPayment.totalMU * factor, 2);
                    productionLot.wholeSubtotalAdjustProcess += totalMUAux2;
                    productionLotPayment.totalProcessMetricUnit = totalMUAux2;
                    productionLotPayment.totalProcessMetricUnitEq = totalMUEqAux;
                    productionLotPayment.id_metricUnitProcess = id_metricUnitKgAux;

                    productionLot.wholeTotalToPay += productionLotPayment.totalToPay;
                }
                else
                {
                    productionLot.totalAdjustmentTailPounds += fitPoundsAux;
                    productionLot.subtotalTailAdjust += totalMUAux;

                    productionLotPayment.totalProcessMetricUnit = totalMUAux;
                    productionLotPayment.totalProcessMetricUnitEq = totalMUEqColAux;
                    productionLotPayment.id_metricUnitProcess = id_metricUnitLbsAux;

                    productionLot.tailTotalToPay += productionLotPayment.totalToPay;
                }
                //productionLot.totalQuantityLiquidationAdjust += productionLotPayment.totalMU * factor;
            }
            //productionLot.totalQuantityLiquidationAdjust = decimal.Round(productionLot.totalQuantityLiquidationAdjust, 2);


        }

        #endregion

        #region PAYMENTS

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionEditFormPaymentsDetailPartial()
        {
            var view = "_ProductionLotReceptionEditFormProductionLotPaymentDetailPartial";
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotPayment.OrderBy(ob => ob.Item.ItemGeneral.ItemSize != null ? ob.Item.ItemGeneral.ItemSize.orderSize : 0).ToList() ?? new List<ProductionLotPayment>();

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");


            ViewBag.totalToPay = productionLot.totalToPay;
            ViewBag.totalToPayEq = productionLot.totalToPayEq;
            ViewBag.totalPriceDistribuido = productionLot.ProductionLotPayment.Sum(e => e.totalPriceDis);
            ViewBag.diferencia = productionLot.ProductionLotPayment.Sum(e => e.differencia);
            var modPrecio = db.Setting.FirstOrDefault(fod => fod.code.Equals("MODPREC"));

            if (productionLot.ProductionLotState.code == "07")
            {
                ViewBag.ShowActionDis = true;
            }
            else
            {
                ViewBag.ShowActionDis = false;
            }
            if (modPrecio.value == "SI" && (productionLot.ProductionLotState.code == "07" || productionLot.ProductionLotState.code == "08"))
            {
                ViewBag.ShowModifPrecio = true;
                ViewBag.ShowModifDetalle = (productionLot.ProductionLotState.code == "07") ? true : false;
            }
            else
            {
                ViewBag.ShowModifPrecio = false;
                ViewBag.ShowModifDetalle = false;
            }


            if (productionLot.isCopackingLot ?? false)
                view = "_ProductionLotReceptionEditFormProductionLotPaymentDetailPartialCopacking";

            return PartialView(view, model);
        }

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionPaymentsDetailDistributedPartial(int id_productionLotPayment)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLotPayment productionLotPayment = (TempData["productionLotPayment"] as ProductionLotPayment);
            productionLotPayment = productionLotPayment ?? new ProductionLotPayment();

            if (productionLotPayment.id != id_productionLotPayment)
            {
                productionLotPayment = db.ProductionLotPayment.FirstOrDefault(e => e.id == id_productionLotPayment);
            }

            List<ProductionLotPayment> model = GetValoresTotalesDeatil(productionLotPayment);

            model = model ?? new List<ProductionLotPayment>();

            TempData.Keep("productionLotPayment");

            return PartialView("_ProductionLotReceptionDistributedEditPartialDetail", model);
        }

        [ValidateInput(false)]
        public ActionResult ProductionLotReceptionSummaryPaymentsDetailPartial()
        {
            var view = "_ProductionLotReceptionSummaryProductionLotPaymentDetailPartial";
            bool? copack = null;

            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            List<SummaryProductionLotPaymentDetail> model = GetListSummaryProductionLotPaymentDetail(productionLot);

            model = model ?? new List<SummaryProductionLotPaymentDetail>();
            //TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");

            if (productionLot.isCopackingLot == null)
                copack = false;
            else
                copack = productionLot.isCopackingLot;

            if ((bool)copack)
                view = "_ProductionLotReceptionSummaryProductionLotPaymentDetailPartialCopacking";

            if (productionLot.ProductionLotState.code == "07")
            {
                ViewBag.ShowActionDis = true;
            }
            else
            {
                ViewBag.ShowActionDis = false;
            }
            var modPrecio = db.Setting.FirstOrDefault(fod => fod.code.Equals("MODPREC"));
            if (modPrecio.value == "SI" && (productionLot.ProductionLotState.code == "07" || productionLot.ProductionLotState.code == "08"))
            {
                ViewBag.ShowModifPrecio = true;
                ViewBag.ShowModifDetalle = (productionLot.ProductionLotState.code == "07") ? true : false;
            }
            else
            {
                ViewBag.ShowModifPrecio = false;
                ViewBag.ShowModifDetalle = false;
            }

            return PartialView(view, model);
        }

        //[HttpPost, ValidateInput(false)]
        //public ActionResult ProductionLotProcessEditFormItemsDetailAddNew(ProductionLotDetail item)
        //{
        //    ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
        //    //productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
        //    productionLot = productionLot ?? new ProductionLot();
        //    productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();

        //    if (ModelState.IsValid)
        //    {
        //        item.id = productionLot.ProductionLotDetail.Count() > 0 ? productionLot.ProductionLotDetail.Max(pld => pld.id) + 1 : 1;
        //        ProductionLot productionOriginLot = db.ProductionLot.FirstOrDefault(pl => pl.id == item.id_originLot);
        //        item.id_warehouse =
        //            productionOriginLot.ProductionLotLiquidation.FirstOrDefault(pll => pll.id_item == item.id_item)
        //                .id_warehouse;
        //        item.id_warehouseLocation =
        //            productionOriginLot.ProductionLotLiquidation.FirstOrDefault(pll => pll.id_item == item.id_item)
        //                .id_warehouseLocation;
        //        productionLot.ProductionLotDetail.Add(item);
        //        UpdateProductionLotTotals(productionLot);
        //    }

        //    TempData["productionLotReception"] = productionLot;
        //    TempData.Keep("productionLotReception");

        //    var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

        //    return PartialView("_ProductionLotProcessEditFormItemsDetailPartial", model);
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult ProductionLotReceptionEditFormClosesDetailUpdate(ProductionLotPayment item)
        //{
        //    ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
        //    productionLot = productionLot ?? new ProductionLot();
        //    productionLot.ProductionLotPayment = productionLot.ProductionLotPayment ?? new List<ProductionLotPayment>();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var modelItem = productionLot.ProductionLotPayment.FirstOrDefault(it => it.id == item.id);//it.id_originLot == item.id_originLot && it.id_item == item.id_item);
        //            if (modelItem != null)
        //            {
        //                this.UpdateModel(modelItem);
        //                UpdateProductionLotPerformance(productionLot);
        //                TempData["productionLotReception"] = productionLot;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "Please, correct all errors.";

        //    TempData.Keep("productionLotReception");

        //    var model = productionLot?.ProductionLotPayment.ToList() ?? new List<ProductionLotPayment>();

        //    return PartialView("_ProductionLotReceptionEditFormProductionLotCloseDetailPartial", model);
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult ProductionLotProcessEditFormItemsDetailDelete(int id)//int id_originLot, int id_item)
        //{
        //    ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
        //    //productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
        //    productionLot = productionLot ?? new ProductionLot();
        //    productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();

        //    //if (id_originLot >= 0 && id_item >= 0)
        //    //{
        //    try
        //    {
        //        var productionLotDetails = productionLot.ProductionLotDetail.FirstOrDefault(p => p.id == id);//p.id_originLot == id_originLot && p.id_item == id_item);
        //        if (productionLotDetails != null)
        //        {
        //            productionLot.ProductionLotDetail.Remove(productionLotDetails);
        //            UpdateProductionLotTotals(productionLot);
        //        }

        //        TempData["productionLotReception"] = productionLot;
        //    }
        //    catch (Exception e)
        //    {
        //        ViewData["EditError"] = e.Message;
        //    }
        //    //}

        //    TempData.Keep("productionLotReception");

        //    var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
        //    return PartialView("_ProductionLotProcessEditFormItemsDetailPartial", model);
        //}

        //private void UpdateProductionLotPerformance(ProductionLot productionLot)
        //{
        //    productionLot.totalQuantityLiquidationAdjust = 0.0M;

        //    foreach (var productionLotPayment in productionLot.ProductionLotPayment)
        //    {
        //        productionLot.totalQuantityLiquidationAdjust += productionLotPayment.totalMU;
        //    }
        //}

        #endregion

        #region DETAILS VIEW

        public ActionResult ProductionLotReceptionDetailItemsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();
            return PartialView("_ProductionLotReceptionDetailItemsPartial", model.ToList());
        }

        public ActionResult ProductionLotReceptionDetailDispatchMaterialsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotDispatchMaterial.ToList() ?? new List<ProductionLotDispatchMaterial>();
            return PartialView("_ProductionLotReceptionDetailDispatchMaterialsPartial", model.ToList());
        }

        public ActionResult ProductionLotReceptionDetailProductionLotLiquidationsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();
            return PartialView("_ProductionLotReceptionDetailProductionLotLiquidationsPartial", model.ToList());
        }

        public ActionResult ProductionLotReceptionDetailProductionLotPackingMaterialsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotPackingMaterial.ToList() ?? new List<ProductionLotPackingMaterial>();

            model = model.Where(d => d.isActive && d.quantity > 0).ToList();

            return PartialView("_ProductionLotReceptionDetailProductionLotPackingMaterialsPartial", model.ToList());
        }

        public ActionResult ProductionLotReceptionDetailProductionLotTrashsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();
            return PartialView("_ProductionLotReceptionDetailProductionLotTrashsPartial", model.ToList());
        }

        public ActionResult ProductionLotReceptionDetailProductionLotQualityAnalysisPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotQualityAnalysis.ToList() ?? new List<ProductionLotQualityAnalysis>();
            return PartialView("_ProductionLotReceptionDetailProductionLotQualityAnalysisPartial", model.ToList());
        }

        public ActionResult ProductionLotReceptionDetailProductionLotClosesPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            //ViewData["id_productionLot"] = id_productionLot;
            //var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            var model = productionLot?.ProductionLotPayment.ToList() ?? new List<ProductionLotPayment>();

            ViewData["withPrice"] = productionLot.withPrice;
            ViewBag.totalQuantityLiquidationAdjust = productionLot.totalQuantityLiquidationAdjust;

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionDetailProductionLotClosesPartial", model.ToList());
        }

        public ActionResult ProductionLotReceptionCustomCallbackAction()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            var model = productionLot?.ProductionLotPayment.ToList() ?? new List<ProductionLotPayment>();
            ViewData["withPrice"] = productionLot.withPrice;
            return PartialView("_ProductionLotReceptionDetailProductionLotClosesPartial", model.ToList());
        }
        #endregion

        #region SINGLE CHANGE PRODUCTION LOT STATE

        [HttpPost]
        public ActionResult Approve(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            //string plin = "";
            string plinInt = "";
            string plinJul = "";
            if (productionLot != null)
            {
                //plin = productionLot.internalNumber;
                //if (productionLot.internalNumber.Length > 5)
                //{
                //	plinJul = productionLot.internalNumber.Substring(0, 5);
                //	plinInt = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                //	productionLot.julianoNumber = productionLot.internalNumber.Substring(0, 5);
                //	productionLot.internalNumber = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                //}
                //else
                //{
                //	plinInt = productionLot.internalNumber;
                //	productionLot.julianoNumber = productionLot.internalNumber;
                //	productionLot.internalNumber = "";
                //}
                //productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;

                int indexInit = 0;
                var aCertification = db.Certification.FirstOrDefault(fod => fod.id == productionLot.id_certification);
                if (aCertification != null) indexInit = aCertification.idLote?.Length ?? 0;
                productionLot.internalNumberConcatenated = productionLot.internalNumber;//(aCertification?.idLote ?? "") + productionLot.julianoNumber + productionLot.internalNumber;

                if (productionLot.internalNumber.Length > (5 + indexInit))
                {
                    productionLot.julianoNumber = productionLot.internalNumber.Substring((indexInit + 0), 5);
                    plinJul = productionLot.julianoNumber;
                    productionLot.internalNumber = productionLot.internalNumber.Substring((indexInit + 5), (productionLot.internalNumber.Length - (indexInit + 5)));
                    plinInt = productionLot.internalNumber;
                }
                else
                {
                    plinInt = productionLot.internalNumber;
                    productionLot.julianoNumber = productionLot.internalNumber;
                    productionLot.internalNumber = "";
                }

            }
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "02");

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        productionLot.internalNumber = productionLot.internalNumberConcatenated;// plinJul + plinInt;

                        this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        productionLot.julianoNumber = plinJul;
                        productionLot.internalNumberConcatenated = productionLot.internalNumber;//plinJul + plinInt;
                        productionLot.internalNumber = plinInt;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }


            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " aprobado exitosamente");

            return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Autorize(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            //ProductionProcess productionProcess = productionLot.ProductionProcess;
            string plin = "";
            string plinInt = "";
            string plinJul = "";
            if (productionLot != null)
            {
                plin = productionLot.internalNumber;
                if (productionLot.internalNumber.Length > 5)
                {
                    plinJul = productionLot.internalNumber.Substring(0, 5);
                    plinInt = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                    productionLot.julianoNumber = productionLot.internalNumber.Substring(0, 5);
                    productionLot.internalNumber = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                }
                else
                {
                    plinInt = productionLot.internalNumber;
                    productionLot.julianoNumber = productionLot.internalNumber;
                    productionLot.internalNumber = "";
                }
                productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;
            }
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "04");

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                        //{
                        //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                        //    db.PurchaseOrderDetail.Attach(details);
                        //    db.Entry(details).State = EntityState.Modified;
                        //}
                        productionLot.internalNumber = plinJul + plinInt;

                        this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        productionLot.julianoNumber = plinJul;
                        productionLot.internalNumber = plinInt;
                        productionLot.internalNumberConcatenated = plinJul + plinInt;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " autorizado exitosamente");

            return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Protect(int id)
        {
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            //ProductionProcess productionProcess = productionLot.ProductionProcess;
            string plin = "";
            string plinInt = "";
            string plinJul = "";
            if (productionLot != null)
            {
                plin = productionLot.internalNumber;
                if (productionLot.internalNumber.Length > 5)
                {
                    plinJul = productionLot.internalNumber.Substring(0, 5);
                    plinInt = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                    productionLot.julianoNumber = productionLot.internalNumber.Substring(0, 5);
                    productionLot.internalNumber = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                }
                else
                {
                    plinInt = productionLot.internalNumber;
                    productionLot.julianoNumber = productionLot.internalNumber;
                    productionLot.internalNumber = "";
                }
                productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;
            }
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "05");

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                        //{
                        //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                        //    db.PurchaseOrderDetail.Attach(details);
                        //    db.Entry(details).State = EntityState.Modified;
                        //}
                        productionLot.internalNumber = plinJul + plinInt;

                        this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        productionLot.julianoNumber = plinJul;
                        productionLot.internalNumber = plinInt;
                        productionLot.internalNumberConcatenated = plinJul + plinInt;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " cerrado exitosamente");

            return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            //string settCxC = db.Setting.FirstOrDefault(fod => fod.code == "HLCXC")?.value ?? "";
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //string plin = "";
            string plinInt = "";
            string plinJul = "";
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

            if (productionLot.isCopackingLot == null)
                this.ViewBag.isCopacking = false;
            else
                this.ViewBag.isCopacking = productionLot.isCopackingLot;
            //ProductionProcess productionProcess = productionLot.ProductionProcess;
            if (productionLot != null)
            {

                int indexInit = 0;
                var aCertification = db.Certification.FirstOrDefault(fod => fod.id == productionLot.id_certification);
                if (aCertification != null) indexInit = aCertification.idLote?.Length ?? 0;
                productionLot.internalNumberConcatenated = productionLot.internalNumber;//(aCertification?.idLote ?? "") + productionLot.julianoNumber + productionLot.internalNumber;

                if (productionLot.internalNumber.Length > (5 + indexInit))
                {
                    productionLot.julianoNumber = productionLot.internalNumber.Substring((indexInit + 0), 5);
                    plinJul = productionLot.julianoNumber;
                    productionLot.internalNumber = productionLot.internalNumber.Substring((indexInit + 5), (productionLot.internalNumber.Length - (indexInit + 5)));
                    plinInt = productionLot.internalNumber;
                }
                else
                {
                    plinInt = productionLot.internalNumber;
                    productionLot.julianoNumber = productionLot.internalNumber;
                    productionLot.internalNumber = "";
                }

            }
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {

                    var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

                    if (entityObjectPermissions != null)
                    {
                        var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                        if (entityPermissions != null)
                        {
                            foreach (var detailt in productionLot.ProductionLotDetail)
                            {
                                var entityValuePermissions = entityPermissions.listValue
                               .FirstOrDefault(fod2 => fod2.id_entityValue == detailt.id_warehouse && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);

                                if (entityValuePermissions == null)
                                {
                                    throw new Exception("No tiene Permiso para Anular la Recepción de Materia Prima.");
                                }
                            }

                        }
                    }

                    var existInProductionLotDetail = db.ProductionLotDetail.FirstOrDefault(fod => fod.id_originLot == id && fod.ProductionLot.ProductionLotState.code != "09");

                    if (existInProductionLotDetail != null)
                    {
                        //TempData.Keep("productionLotReception");
                        //ViewData["EditMessage"] = ErrorMessage("No se puede anular el lote debido a tener Lotes de Procesos que dependen de él.");
                        //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                        throw new Exception("Tener Lotes de Procesos que dependen de él.");
                    }

                    var existInProductionLotDailyCloseDetail = db.ProductionLotDailyCloseDetail.FirstOrDefault(fod => fod.id_productionLot == id && fod.ProductionLotDailyClose.Document.DocumentState.code != "05");

                    if (existInProductionLotDailyCloseDetail != null)
                    {
                        //TempData.Keep("productionLotReception");
                        //ViewData["EditMessage"] = ErrorMessage("No se puede anular el lote debido a pertenecer a un cierre diario/turno.");
                        //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                        throw new Exception("Pertenecer a un cierre diario/turno.");

                    }

                    //Se anula el Ingreso Materia Prima Automático si existe
                    var id_inventaryMoveAutomaticRawMaterialEntry = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == productionLot.id &&
                                                                fod.Document.DocumentType.code.Equals("137"))?.id_document;//137: Ingreso Materia Prima Automático
                    InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveAutomaticRawMaterialEntry);
                    if (inventoryMove != null)
                    {
                        //ServiceInventoryMove.UpdateInventaryMoveAutomaticRawMaterialEntry(false, ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, inventoryMove);

                        DocumentState documentStateAnulado = db.DocumentState.FirstOrDefault(s => s.code == "05"); //Anulado

                        inventoryMove.Document.id_documentState = documentStateAnulado.id;
                        inventoryMove.Document.DocumentState = documentStateAnulado;

                        db.InventoryMove.Attach(inventoryMove);
                        db.Entry(inventoryMove).State = EntityState.Modified;
                    }

                    if (productionLot.ProductionLotState.code != "01" && productionLot.ProductionLotState.code != "02" &&
                        productionLot.ProductionLotState.code != "03")//PENDIENTE DE RECEPCION, //RECEPCIONADO, //PENDIENTE DE PROCESAMIENTO
                    {
                        var inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("ILP")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                        InventoryMove lastInventoryMoveILP = (inventoryMoveDetailAux.Count > 0)
                                                                            ? inventoryMoveDetailAux.First().InventoryMove
                                                                            : null;
                        //AQUI SE COMENTA 11102018
                        //if (settCxC == "0")
                        //{
                        //    ServiceInventoryMove.UpdateInventaryMoveEntryLiquidationPlant(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveILP);
                        //}


                        var idProductionLotDetailAux = productionLot.ProductionLotDetail.FirstOrDefault().id;
                        InventoryMove lastInventoryMoveEMPRA = db.InventoryMoveDetailExitProductionLotDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idProductionLotDetailAux)?.InventoryMoveDetail.InventoryMove;

                        //AQUI SE COMENTA 11102018
                        //if (lastInventoryMoveEMPRA != null)
                        //    ServiceInventoryMove.UpdateInventaryMoveExitMateriaPrimaReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveEMPRA);

                        //AQUI SE COMENTA 11102018
                        //if (productionLot.ProductionLotTrash.Count > 0)
                        //{
                        //    inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("IDE")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                        //    InventoryMove lastInventoryMoveIDE = (inventoryMoveDetailAux.Count > 0)
                        //                                        ? inventoryMoveDetailAux.First().InventoryMove
                        //                                        : null;
                        //    ServiceInventoryMove.UpdateInventaryMoveEntryTrash(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveIDE);
                        //}

                        var existInInventoryMoveDetailExitPackingMaterial = productionLot.ProductionLotPackingMaterial.FirstOrDefault(fod => fod.InventoryMoveDetailExitPackingMaterial.Count() > 0);//Tiene egreso de Inventario de MDE

                        if (existInInventoryMoveDetailExitPackingMaterial != null)
                        {
                            var existInInventoryMoveEME = existInInventoryMoveDetailExitPackingMaterial.InventoryMoveDetailExitPackingMaterial.FirstOrDefault().InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EME");
                            if (existInInventoryMoveEME)
                            {
                                //TempData.Keep("productionLotReception");
                                //ViewData["EditMessage"] = ErrorMessage("No se puede anular el lote debido a tener egresos de materiales de empaque en inventario, manual.");
                                //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                                throw new Exception("Tener egresos de materiales de empaque en inventario, manual.");

                            }

                        }

                        //AQUI SE COMENTA 11102018
                        //var inventoryMoveEMEA = productionLot.ProductionLotPackingMaterial.FirstOrDefault()?.InventoryMoveDetailExitPackingMaterial?.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EMEA"))?.InventoryMoveDetail.InventoryMove;//EMEA: Egreso Materiales de Empaque Automático
                        //if (inventoryMoveEMEA != null)
                        //{
                        //    ServiceInventoryMove.UpdateInventaryMoveExitPackingMaterial(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, inventoryMoveEMEA);
                        //}
                    }

                    if (productionLot.ProductionLotState.code != "01")//PENDIENTE DE RECEPCION
                    {
                        //var inventoryMoveIMDA = productionLot.ProductionLotDispatchMaterial.FirstOrDefault()?.InventoryMoveDetailEntryDispatchMaterialsInProductionLot?.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("IMDA"))?.InventoryMoveDetail.InventoryMove;//EMDA: Egreso Materiales de Despacho Automatica
                        //if (inventoryMoveIMDA != null)
                        //{
                        //    ServiceInventoryMove.UpdateInventaryMoveEntryDispatchMaterial(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, inventoryMoveIMDA);
                        //}
                        //var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == productionLot.id &&
                        //                                                         (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDR") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                        //InventoryMove lastInventoryMoveEPTAMDR = (inventoryMoveDetailAux.Count > 0)
                        //                                    ? inventoryMoveDetailAux.First().Document.InventoryMove
                        //                                    : null;
                        //inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == productionLot.id &&
                        //                                                     (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDR") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                        //InventoryMove lastInventoryMoveIPTAMDR = (inventoryMoveDetailAux.Count > 0)
                        //                                    ? inventoryMoveDetailAux.First().Document.InventoryMove
                        //                                    : null;
                        //hey
                        //ServiceInventoryMove.UpdateInventaryMoveTransferDispatchMaterialsReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveEPTAMDR, lastInventoryMoveIPTAMDR);
                        //ServiceInventoryMove.UpdateInventaryMoveTransferDispatchMaterialsReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveEPTAMDR, lastInventoryMoveIPTAMDR);

                        //var inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("IMPRA")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                        //InventoryMove lastInventoryMoveIMPRA = (inventoryMoveDetailAux.Count > 0)
                        //                                                    ? inventoryMoveDetailAux.First().InventoryMove
                        //                                                    : null;
                        //if (settCxC == "1")
                        //{
                        if (productionLot.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code == "03") != null)//Aprobada
                        {
                            throw new Exception("Existe Liquidación de Carro x Carro Aprobada asociada a este Lote.");
                        }
                        //}
                        var idProductionLotDetailAux = productionLot.ProductionLotDetail.FirstOrDefault().id;
                        //AQUI SE COMENTA 11102018
                        //InventoryMove lastInventoryMoveIMPRA = db.InventoryMoveDetailEntryProductionLotDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idProductionLotDetailAux).InventoryMoveDetail.InventoryMove;
                        //ServiceInventoryMove.UpdateInventaryMoveEntryMateriaPrimaReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveIMPRA);

                    }

                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "09");//ANULADO

                    if (productionLot != null && productionLotState != null)
                    {
                        var documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");//ANULADA

                        foreach (var detail in productionLot.ProductionLotDetail)
                        {
                            var qualityControlAux = detail.ProductionLotDetailQualityControl
                                                                            .FirstOrDefault(fod => fod.IsDelete == false
                                                                                                   && fod.QualityControl.Document.DocumentState.code != ("05"))?.QualityControl;//"05": ANULADA
                            if (qualityControlAux != null)
                            {
                                qualityControlAux.Document.id_documentState = documentState.id;
                                qualityControlAux.Document.DocumentState = documentState;

                                db.QualityControl.Attach(qualityControlAux);
                                db.Entry(qualityControlAux).State = EntityState.Modified;
                            }

                            foreach (var productionLotDetailPurchaseDetail in detail.ProductionLotDetailPurchaseDetail)
                            {
                                ServicePurchaseRemission.UpdateQuantityRecived(db, productionLotDetailPurchaseDetail.id_purchaseOrderDetail,
                                                                               productionLotDetailPurchaseDetail.id_remissionGuideDetail,
                                                                               -productionLotDetailPurchaseDetail.quanty);
                            }
                        }

                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        //Actualizar Estado del Lote Base

                        productionLot.Lot.Document.id_documentState = documentState.id;
                        productionLot.Lot.Document.DocumentState = documentState;
                        //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                        //{
                        //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                        //    db.PurchaseOrderDetail.Attach(details);
                        //    db.Entry(details).State = EntityState.Modified;
                        //}
                        productionLot.internalNumber = productionLot.internalNumberConcatenated;//plinJul + plinInt;

                        this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        productionLot.julianoNumber = plinJul;
                        productionLot.internalNumberConcatenated = productionLot.internalNumber;//plinJul + plinInt;
                        productionLot.internalNumber = plinInt;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("productionLotReception");
                    ViewData["EditMessage"] = ErrorMessage("No se puede anular el lote debido a: " + e.Message);
                    return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                }
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " anulado exitosamente");

            return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //string settCxC = db.Setting.FirstOrDefault(fod => fod.code == "HLCXC")?.value ?? "";
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            //ProductionProcess productionProcess = productionLot.ProductionProcess;
            var loteManualEdit = productionLot.ProductionProcess.code == "RMM";

            if (productionLot.isCopackingLot == null)
                this.ViewBag.isCopacking = false;
            else
                this.ViewBag.isCopacking = productionLot.isCopackingLot;

            //string plin = "";
            string plinInt = "";
            string plinJul = "";
            if (productionLot != null)
            {
                //plin = productionLot.internalNumber;
                //if (productionLot.internalNumber.Length > 5)
                //{
                //	plinJul = productionLot.internalNumber.Substring(0, 5);
                //	plinInt = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                //	productionLot.julianoNumber = productionLot.internalNumber.Substring(0, 5);
                //	productionLot.internalNumber = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                //}
                //else
                //{
                //	plinInt = productionLot.internalNumber;
                //	productionLot.julianoNumber = productionLot.internalNumber;
                //	productionLot.internalNumber = "";
                //}
                //productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;

                int indexInit = 0;
                var aCertification = db.Certification.FirstOrDefault(fod => fod.id == productionLot.id_certification);
                if (aCertification != null) indexInit = aCertification.idLote?.Length ?? 0;
                productionLot.internalNumberConcatenated = productionLot.internalNumber;//(aCertification?.idLote ?? "") + productionLot.julianoNumber + productionLot.internalNumber;

                if (productionLot.internalNumber.Length > (5 + indexInit))
                {
                    productionLot.julianoNumber = productionLot.internalNumber.Substring((indexInit + 0), 5);
                    plinJul = productionLot.julianoNumber;
                    productionLot.internalNumber = productionLot.internalNumber.Substring((indexInit + 5), (productionLot.internalNumber.Length - (indexInit + 5)));
                    plinInt = productionLot.internalNumber;
                }
                else
                {
                    plinInt = productionLot.internalNumber;
                    productionLot.julianoNumber = productionLot.internalNumber;
                    productionLot.internalNumber = "";
                }
            }
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //Verificar si existe lotes relacionados al proceso de cierre
                    var productionLotAux = db.ProductionLot.FirstOrDefault(a => a.id == id && a.isClosed);
                    if (productionLotAux != null)
                    {
                        var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.id_lot == id && a.Document.DocumentState.code != "05"
                                                && a.isActive);

                        if (productionLotClose != null && productionLotAux.receptionDate.Date <= productionLotClose.Document.emissionDate.Date 
                            && productionLotAux.ProductionLotState.code == "11")
                        {
                            throw new Exception("El lote " + productionLotAux.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
                        }
                    }

                    var codeStateCurrent = productionLot.ProductionLotState.code;

                    var existInProductionLotDetail = db.ProductionLotDetail.FirstOrDefault(fod => fod.id_originLot == id && fod.ProductionLot.ProductionLotState.code != "09");

                    var bcxc = db.Setting.FirstOrDefault(fod => fod.code.Equals("HLCXC"));
                    var Liqcxc = db.LiquidationCartOnCart.FirstOrDefault(fod => fod.id_ProductionLot == id);
                    if (bcxc.value == "1" && Liqcxc != null)
                    {
                        var LiqTur = db.LiquidationTurn.FirstOrDefault(fod => DbFunctions.TruncateTime(fod.liquidationDate) == DbFunctions.TruncateTime(Liqcxc.dateInit)
                                                    && (fod.Document.DocumentState.code == "01" || fod.Document.DocumentState.code == "03")
                                                    && fod.id_turn == Liqcxc.MachineProdOpening.id_Turn);


                        if (bcxc != null && bcxc.value == "1")
                        {
                            if (LiqTur != null)
                            {
                                throw new Exception("que se encuentra en una liquidación de turno Nº " + LiqTur.Document.number + " en estado " + LiqTur.Document.DocumentState.name);
                            }
                        }
                    }

                    if (existInProductionLotDetail != null && codeStateCurrent != "11")
                    {
                        //TempData.Keep("productionLotReception");
                        //ViewData["EditMessage"] = ErrorMessage("No se puede reversar el lote debido a tener Lotes de Procesos que dependen de él.");
                        //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                        throw new Exception("Existen Lotes de Procesos " + existInProductionLotDetail.ProductionLot.number + " que dependen de él.");
                    }

                    if (codeStateCurrent == "02" ||
                        codeStateCurrent == "03")//RECEPCIONADO, //PENDIENTE DE PROCESAMIENTO
                    {
                        //var inventoryMoveIMDA = productionLot.ProductionLotDispatchMaterial.FirstOrDefault(fod1=> fod1.arrivalDestinationQuantity > 0)?.InventoryMoveDetailEntryDispatchMaterialsInProductionLot?.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("IMDA"))?.InventoryMoveDetail.InventoryMove;//EMDA: Egreso Materiales de Despacho Automatica
                        //if (inventoryMoveIMDA != null)
                        //{
                        //    ServiceInventoryMove.UpdateInventaryMoveEntryDispatchMaterial(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, inventoryMoveIMDA);
                        //}
                        //var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == productionLot.id &&
                        //                                                         (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDR") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                        //InventoryMove lastInventoryMoveEPTAMDR = (inventoryMoveDetailAux.Count > 0)
                        //                                    ? inventoryMoveDetailAux.First().Document.InventoryMove
                        //                                    : null;
                        //inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == productionLot.id &&
                        //                                                     (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDR") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                        //InventoryMove lastInventoryMoveIPTAMDR = (inventoryMoveDetailAux.Count > 0)
                        //                                    ? inventoryMoveDetailAux.First().Document.InventoryMove
                        //                                    : null;
                        //hey
                        //ServiceInventoryMove.UpdateInventaryMoveTransferDispatchMaterialsReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveEPTAMDR, lastInventoryMoveIPTAMDR);
                        //ServiceInventoryMove.UpdateInventaryMoveTransferDispatchMaterialsReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveEPTAMDR, lastInventoryMoveIPTAMDR);
                        //if (settCxC == "1")
                        //{
                        if (productionLot.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code == "03") != null)//Aprobada
                        {
                            throw new Exception("Debido a que existe Liquidación de Carro x Carro Aprobada asociada a este Lote.");
                        }
                        //}
                        var idProductionLotDetailAux = productionLot.ProductionLotDetail.FirstOrDefault().id;
                        //AQUI SE COMENTA 11102018
                        //InventoryMove lastInventoryMoveIMPRA = db.InventoryMoveDetailEntryProductionLotDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idProductionLotDetailAux).InventoryMoveDetail.InventoryMove;
                        //ServiceInventoryMove.UpdateInventaryMoveEntryMateriaPrimaReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveIMPRA);

                    }

                    if (codeStateCurrent == "04" || codeStateCurrent == "05")
                    {
                        var inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("ILP")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                        //AQUI SE COMENTA 11102018
                        //InventoryMove lastInventoryMoveILP = (inventoryMoveDetailAux.Count > 0)
                        //                                                    ? inventoryMoveDetailAux.First().InventoryMove
                        //                                                    : null;
                        //ServiceInventoryMove.UpdateInventaryMoveEntryLiquidationPlant(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveILP);

                        var idAux = productionLot.ProductionLotDetail.FirstOrDefault().id;
                        //AQUI SE COMENTA 11102018
                        //InventoryMove lastInventoryMoveEMPRA = db.InventoryMoveDetailExitProductionLotDetail.FirstOrDefault(fod => fod.id_productionLotDetail == idAux)?.InventoryMoveDetail.InventoryMove;
                        //if (lastInventoryMoveEMPRA != null)
                        //    ServiceInventoryMove.UpdateInventaryMoveExitMateriaPrimaReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveEMPRA);

                        if (productionLot.ProductionLotTrash.Count > 0)
                        {
                            inventoryMoveDetailAux = productionLot.Lot.InventoryMoveDetail.Where(w => w.InventoryMove.InventoryReason.code.Equals("IDE")).OrderByDescending(d => d.InventoryMove.Document.dateCreate).ToList();
                            //AQUI SE COMENTA 11102018
                            //InventoryMove lastInventoryMoveIDE = (inventoryMoveDetailAux.Count > 0)
                            //                                    ? inventoryMoveDetailAux.First().InventoryMove
                            //                                    : null;
                            //ServiceInventoryMove.UpdateInventaryMoveEntryTrash(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, lastInventoryMoveIDE);
                        }

                        var existInInventoryMoveDetailExitPackingMaterial = productionLot.ProductionLotPackingMaterial.FirstOrDefault(fod => fod.InventoryMoveDetailExitPackingMaterial.Count() > 0);//Tiene egreso de Inventario de MDE

                        if (existInInventoryMoveDetailExitPackingMaterial != null)
                        {
                            var existInInventoryMoveEME = existInInventoryMoveDetailExitPackingMaterial.InventoryMoveDetailExitPackingMaterial.FirstOrDefault().InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EME");
                            if (existInInventoryMoveEME)
                            {
                                //TempData.Keep("productionLotReception");
                                //ViewData["EditMessage"] = ErrorMessage("No se puede reversar el lote debido a tener egresos de materiales de empaque en inventario, manual.");
                                //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                                throw new Exception("Tener egresos de materiales de empaque en inventario, manual.");

                            }

                        }
                        //AQUI SE COMENTA 11102018
                        //var inventoryMoveEMEA = productionLot.ProductionLotPackingMaterial.FirstOrDefault()?.InventoryMoveDetailExitPackingMaterial?.FirstOrDefault(fod => fod.InventoryMoveDetail.InventoryMove.InventoryReason.code.Equals("EMEA"))?.InventoryMoveDetail.InventoryMove;//EMEA: Egreso Materiales de Empaque Automático
                        //if (inventoryMoveEMEA != null)
                        //{
                        //    ServiceInventoryMove.UpdateInventaryMoveExitPackingMaterial(ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, inventoryMoveEMEA);
                        //}

                    }

                    if (codeStateCurrent == "06" || codeStateCurrent == "07")
                    {
                        var existInProductionLotDailyCloseDetail = db.ProductionLotDailyCloseDetail.FirstOrDefault(fod => fod.id_productionLot == id && fod.ProductionLotDailyClose.Document.DocumentState.code != "05");

                        if (existInProductionLotDailyCloseDetail != null)
                        {
                            //TempData.Keep("productionLotReception");
                            //ViewData["EditMessage"] = ErrorMessage("No se puede reversar el lote debido a pertenecer a un cierre diario/turno.");
                            //return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                            throw new Exception("Pertenecer a un cierre diario/turno.");

                        }
                    }

                    if (codeStateCurrent == "08" && loteManualEdit == false)//PAGADO
                    {
                        var existInIntegrationProcessTrans = db.IntegrationProcessDetail.FirstOrDefault(fod => fod.id_Document == id && fod.IntegrationProcess.IntegrationState.code == "03");//Transmitido

                        if (existInIntegrationProcessTrans != null)
                        {
                            throw new Exception("Ya ha sido Transmitido en el Proceso de Integración.");

                        }

                        var id_inventaryMoveAutomaticRawMaterialEntry = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == productionLot.id &&
                                                                   fod.Document.DocumentType.code.Equals("137"))?.id_document;//137: Ingreso Materia Prima Automático
                        InventoryMove inventoryMove = db.InventoryMove.FirstOrDefault(fod => fod.id == id_inventaryMoveAutomaticRawMaterialEntry);
                        if (inventoryMove != null)
                        {
                            ServiceInventoryMove.UpdateInventaryMoveAutomaticRawMaterialEntry(false, ActiveUser, ActiveCompany, ActiveEmissionPoint, productionLot, db, true, inventoryMove);

                            DocumentState documentStatePendiente = db.DocumentState.FirstOrDefault(s => s.code == "01"); //Pendiente

                            inventoryMove.Document.id_documentState = documentStatePendiente.id;
                            inventoryMove.Document.DocumentState = documentStatePendiente;

                            db.InventoryMove.Attach(inventoryMove);
                            db.Entry(inventoryMove).State = EntityState.Modified;
                        }
                    }
                    ProductionLotState productionLotState = null;
                    if (codeStateCurrent == "08" && loteManualEdit == false)//PAGADO
                    {
                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "07");//PENDIENTE DE PAGO
                    }
                    else
                    {
                        if (productionLot.LiquidationCartOnCartDetail.FirstOrDefault(fod => fod.ProductionLot.Lot.Document.DocumentState.code != "05") != null)//Diferente a anulada
                        {
                            throw new Exception("Debido a que existe Liquidación de Carro x Carro asociada a este Lote.");
                        }
                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "01");//PENDIENTE DE RECEPCION
                    }

                    if (codeStateCurrent == "07" || codeStateCurrent == "06")//PENDIENTE DE PAGO Ó CERRADO
                    {
                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "05");//PENDIENTE DE CIERRE
                    }

                    if (codeStateCurrent == "05" || codeStateCurrent == "04")//PENDIENTE DE CIERRE Ó EN PROCESAMIENTO
                    {
                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "03");//PENDIENTE DE PROCESAMIENTO
                    }
                    if (codeStateCurrent == "11" && loteManualEdit == false)//CONCILIADO
                    {

                        int id_user = (int)ViewData["id_user"];
                        int id_menu = (int)ViewData["id_menu"];

                        User user = DataProviderUser.UserById(id_user);
                        UserMenu userMenu = user.UserMenu.FirstOrDefault(m => m.Menu.id == id_menu);
                        if (userMenu != null)
                        {
                            Permission permission = userMenu.Permission.FirstOrDefault(p => p.name == "Conciliar");
                            if (permission == null)
                            {
                                throw new Exception("No tiene Permiso para Reversar en estado Conciliado");
                            }
                        }

                        // Actualizo los movimientos de inventario
                        #region Actualizo los movimientos de inventario
                        // Busco el estado CONCILIADO para Movimiento de inventario
                        DocumentState documentState = db.DocumentState.FirstOrDefault(fod => fod.code == "03");
                        if (documentState == null)
                            throw new Exception("Estado de documento  no encontrado");

                        DocumentSource documentSource = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == productionLot.id);
                        Document document = db.Document.FirstOrDefault(e => e.id == documentSource.id_document);

                        if (document == null)
                            throw new Exception("Documento de movimiento de inventario no encontrado");

                        document.id_documentState = documentState.id;
                        document.id_userUpdate = this.ActiveUserId;
                        document.dateUpdate = DateTime.Now;

                        db.Document.Attach(document);
                        db.Entry(document).State = EntityState.Modified;
                        #endregion

                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "08");//APROBADO
                    }

                    if (codeStateCurrent == "03" || codeStateCurrent == "02")//PENDIENTE DE PROCESAMIENTO Ó RECEPCIONADO
                    {
                        foreach (var detail in productionLot.ProductionLotDetail)
                        {
                            foreach (var productionLotDetailPurchaseDetail in detail.ProductionLotDetailPurchaseDetail)
                            {
                                ServicePurchaseRemission.UpdateQuantityRecived(db, productionLotDetailPurchaseDetail.id_purchaseOrderDetail,
                                                                               productionLotDetailPurchaseDetail.id_remissionGuideDetail,
                                                                               -productionLotDetailPurchaseDetail.quanty);
                            }
                        }

                        productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "01");//PENDIENTE DE RECEPCION
                                                                                                       //Actualizar Estado del Lote Base
                        var documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE
                        productionLot.Lot.Document.id_documentState = documentState.id;
                        productionLot.Lot.Document.DocumentState = documentState;
                    }

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;

                        //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                        //{
                        //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                        //    db.PurchaseOrderDetail.Attach(details);
                        //    db.Entry(details).State = EntityState.Modified;
                        //}

                        productionLot.internalNumber = productionLot.internalNumberConcatenated;//plinJul + plinInt;

                        if (loteManualEdit == false)
                            this.ActualizarRegistroRomaneo(productionLot);

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        string settCxC = db.Setting.FirstOrDefault(fod => fod.code == "HLCXC")?.value ?? "0";

                        if (settCxC != "0" || productionLot.ProductionLotLiquidationTotal.Any())
                        {
                            if (productionLot.ProductionLotState.code == "02" || productionLot.ProductionLotState.code == "03") UpdateProductionLotProductionLotLiquidationTotals(productionLot);
                        }

                        productionLot.julianoNumber = plinJul;
                        productionLot.internalNumberConcatenated = productionLot.internalNumber;//plinJul + plinInt;
                        productionLot.internalNumber = plinInt;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    TempData.Keep("productionLotReception");
                    ViewData["EditMessage"] = ErrorMessage("No se puede reversar el lote debido a: " + e.Message);
                    return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                }
            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " reversado exitosamente");

            //if (productionLot.isCopackingLot == null)
            //	this.ViewBag.isCopacking = false;
            //else
            //	this.ViewBag.isCopacking = productionLot.isCopackingLot;
            if (loteManualEdit == false)
            {
                return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
            }
            else
            {
                return PartialView("_ProductionLotManualReceptionEditFormPartial", productionLot);
            }
        }

        [HttpPost]
        public ActionResult Conciliation(int id)
        {
            string plin = "";
            string plinInt = "";
            string plinJul = "";
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            var loteManualEdit = productionLot.ProductionProcess.code == "RMM";

            if (productionLot != null)
            {
                plin = productionLot.internalNumber;
                if (productionLot.internalNumber.Length > 5)
                {
                    plinJul = productionLot.internalNumber.Substring(0, 5);
                    plinInt = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                    productionLot.julianoNumber = productionLot.internalNumber.Substring(0, 5);
                    productionLot.internalNumber = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                }
                else
                {
                    plinInt = productionLot.internalNumber;
                    productionLot.julianoNumber = productionLot.internalNumber;
                    productionLot.internalNumber = "";
                }
                productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;
            }

            if (productionLot.isCopackingLot == null)
                this.ViewBag.isCopacking = false;
            else
                this.ViewBag.isCopacking = productionLot.isCopackingLot;


            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "11");

                    if (productionLot != null && productionLotState != null)
                    {
                        productionLot.id_ProductionLotState = productionLotState.id;
                        productionLot.ProductionLotState = productionLotState;
                        productionLot.internalNumber = plinJul + plinInt;
                        productionLot.id_userUpdate = this.ActiveUserId;
                        productionLot.dateUpdate = DateTime.Now;

                        // Actualizo los movimientos de inventario
                        #region Actualizo el movimiento de inventario

                        ConciliarMovimientosInventarioRecepcion(productionLot);

                        #endregion                        

                        db.ProductionLot.Attach(productionLot);
                        db.Entry(productionLot).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        productionLot.julianoNumber = plinJul;
                        productionLot.internalNumber = plinInt;
                        productionLot.internalNumberConcatenated = plinJul + plinInt;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                    if (loteManualEdit == false)
                    {
                        return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                    }
                    else
                    {
                        return PartialView("_ProductionLotManualReceptionEditFormPartial", productionLot);
                    }
                }
            }


            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");
            ViewData["EditMessage"] = SuccessMessage("Lote: " + productionLot.number + " conciliado exitosamente");


            if (loteManualEdit == false)
            {
                return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
            }
            else
            {
                return PartialView("_ProductionLotManualReceptionEditFormPartial", productionLot);
            }
        }

        private void ConciliarMovimientosInventarioRecepcion(ProductionLot productionLot)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction()) 
                {
                    try
                    {
                        // Busco el estado CONCILIADO para Movimiento de inventario
                        DocumentState documentState = db.DocumentState.FirstOrDefault(fod => fod.code == "16");
                        if (documentState == null)
                            throw new Exception("Estado de documento CONCILIADO no encontrado");

                        // Busco el movimiento en base al movimiento de inventario y actualizo estado
                        DocumentSource documentSource = db.DocumentSource.FirstOrDefault(fod => fod.id_documentOrigin == productionLot.id);

                        if (documentSource == null)
                            throw new Exception("Documento de movimiento de inventario no encontrado");

                        Document document = db.Document.FirstOrDefault(e => e.id == documentSource.id_document);

                        if (document == null)
                            throw new Exception("Documento de movimiento de inventario no encontrado");

                        document.id_documentState = documentState.id;
                        document.id_userUpdate = this.ActiveUserId;
                        document.dateUpdate = DateTime.Now;

                        db.Document.Attach(document);
                        db.Entry(document).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var message = ex.InnerException?.Message ?? ex.Message;
                        throw new Exception($"Error al conciliar el movimiento de Inventario: {message}");
                    }
                }
            }
        }

        #endregion

        #region SELECTED PRODUCTION LOT STATE CHANGE

        [HttpPost, ValidateInput(false)]
        public void ApproveLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "02");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;

                                //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                                //{
                                //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                                //    db.PurchaseOrderDetail.Attach(details);
                                //    db.Entry(details).State = EntityState.Modified;
                                //}
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void AutorizeLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "04");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;

                                //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                                //{
                                //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                                //    db.PurchaseOrderDetail.Attach(details);
                                //    db.Entry(details).State = EntityState.Modified;
                                //}
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void ProtectLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "05");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;

                                //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                                //{
                                //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                                //    db.PurchaseOrderDetail.Attach(details);
                                //    db.Entry(details).State = EntityState.Modified;
                                //}
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void CancelLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "03");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;

                                //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                                //{
                                //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                                //    db.PurchaseOrderDetail.Attach(details);
                                //    db.Entry(details).State = EntityState.Modified;
                                //}
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        [HttpPost, ValidateInput(false)]
        public void RevertLots(int[] ids)
        {
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "01");

                        foreach (var id in ids)
                        {
                            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);

                            if (productionLot != null && productionLotState != null)
                            {
                                productionLot.id_ProductionLotState = productionLotState.id;
                                productionLot.ProductionLotState = productionLotState;

                                //foreach (var details in purchaseOrder.PurchaseOrderDetail)
                                //{
                                //    details.quantityApproved = (details.quantityApproved > 0) ? details.quantityApproved : details.quantityRequested;

                                //    db.PurchaseOrderDetail.Attach(details);
                                //    db.Entry(details).State = EntityState.Modified;
                                //}
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = (TempData["model"] as List<ProductionLot>);
            model = model ?? new List<ProductionLot>();
            int[] filters = model.Select(i => i.id).ToArray();
            model = db.ProductionLot.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

            TempData["model"] = model;
            TempData.Keep("model");
        }

        #endregion

        #region PRODUCTION LOT RECEPTION REPORTS

        [HttpPost]
        public ActionResult ProductionLotReceptionReport(int id)
        {
            //RemissionGuideReport remissionGuideReport = new RemissionGuideReport();
            //remissionGuideReport.Parameters["id_company"].Value = this.ActiveCompanyId;
            //remissionGuideReport.Parameters["id_remissionGuide"].Value = id;
            //return PartialView("_RemissionGuideReport", remissionGuideReport);
            try
            {
                Session["URLOTE"] = ConfigurationManager.AppSettings["URLOTE"];
            }
            catch (Exception ex)
            {
                ViewBag.IframeUrl = ex.Message;


            }

            ViewBag.IframeUrl = Session["URLOTE"] + "?id=" + id;



            //return Redirect("../Views/AditionalReport/WReportGuia.aspx"); //  View("WReportGuia"); //Aspx file Views/Products/WebForm1.aspx
            return PartialView("IndexReportLote");
        }

        [HttpPost]
        public ActionResult ProductionLotLiquidationPaymentReport(int id)
        {
            ProductionLotLiquidationPaymentReport productionLotLiquidationPaymentReport = new ProductionLotLiquidationPaymentReport();
            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(pp => pp.id == id);

            var id_companyAux = (productionLot != null ? productionLot.id_company : this.ActiveCompanyId);
            Company company = db.Company.FirstOrDefault(c => c.id == id_companyAux);
            if (productionLot != null)
            {
                if (productionLot.internalNumber.Length > 5)
                {
                    productionLot.julianoNumber = productionLot.internalNumber.Substring(0, 5);
                    productionLot.internalNumber = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                }
                else
                {
                    productionLot.julianoNumber = productionLot.internalNumber;
                    productionLot.internalNumber = "";
                }
                productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;
            }
            try
            {
                var listProductionLotLiquidationPaymentDetailAux = productionLot?.ProductionLotPayment.GroupBy(gb => new
                {
                    gb.Item.id_itemType,
                    gb.Item.id_itemTypeCategory,
                    gb.Item.ItemGeneral?.id_size,
                    gb.id_metricUnit,
                    gb.price
                }).Select(s => new
                {
                    s.Key.id_itemType,
                    itemType = db.ItemType.FirstOrDefault(fod => fod.id == s.Key.id_itemType)?.name ?? "",
                    id_groupCategory = s.Key.id_itemTypeCategory,
                    groupCategory = db.ItemTypeCategory.FirstOrDefault(fod => fod.id == s.Key.id_itemTypeCategory)?.name ?? "Sin Clase",
                    s.Key.id_size,
                    size = db.ItemSize.FirstOrDefault(fod => fod.id == s.Key.id_size)?.name ?? "Sin Talla",
                    orderSize = db.ItemSize.FirstOrDefault(fod => fod.id == s.Key.id_size)?.orderSize ?? 0,
                    s.Key.id_metricUnit,
                    s.Key.price,
                    totalMU = s.Sum(ss => ss.totalMU),
                    totalToPay = s.Sum(ss => ss.totalToPay),
                    totalToPayEq = s.Sum(ss => ss.totalToPayEq)
                }).OrderBy(ob => ob.orderSize).ToList();

                var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
                var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
                var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

                var metricUnitLbsAux = metricUnitUMTP;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs");
                if (metricUnitLbsAux == null)
                {
                    throw new Exception("No Existe o esta mal configurado el parámentro con nombre (Unidad de Medida para Totales en Producción) con Código: UMTP. Necesario para el reporte Configúrelo, e intente de nuevo");
                    //throw new Exception("No Existe la unidad de medida Libras con Código: Lbs. Necesario para el reporte Configúrelo, e intente de nuevo");
                }

                var listProductionLotLiquidationPaymentDetailFinalAux = new List<ProductionLotLiquidationPaymentDetailReport>();
                foreach (var detail in listProductionLotLiquidationPaymentDetailAux)
                {
                    var metricUnitAux = db.MetricUnit.FirstOrDefault(fod => fod.id == detail.id_metricUnit);
                    var metricUnitConversionCurrentToLbs = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                            muc.id_metricOrigin == detail.id_metricUnit &&
                                                                                            muc.id_metricDestiny == metricUnitLbsAux.id);
                    var factorCurrentToLbs = detail.id_metricUnit == metricUnitLbsAux.id ? 1 : metricUnitConversionCurrentToLbs?.factor ?? 0;
                    if (factorCurrentToLbs == 0)
                    {
                        throw new Exception("Falta el Factor de Conversión entre : " + metricUnitAux.code + " y " + (metricUnitUMTP?.code)/*"Lbs"*/ + ".Necesario para el reporte Configúrelo, e intente de nuevo");
                    }
                    var metricUnitConversionLbsToCurrent = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                            muc.id_metricOrigin == metricUnitLbsAux.id &&
                                                                                            muc.id_metricDestiny == detail.id_metricUnit);
                    var factorLbsToCurrent = detail.id_metricUnit == metricUnitLbsAux.id ? 1 : metricUnitConversionLbsToCurrent?.factor ?? 0;
                    if (factorLbsToCurrent == 0)
                    {
                        throw new Exception("Falta el Factor de Conversión entre : " + (metricUnitUMTP?.code)/*"Lbs"*/ + " y " + metricUnitAux.code + ".Necesario para el reporte Configúrelo, e intente de nuevo");
                    }
                    var newDetail = new ProductionLotLiquidationPaymentDetailReport
                    {
                        id_itemType = detail.id_itemType,
                        itemType = detail.itemType,
                        id_groupCategory = detail.id_groupCategory,
                        groupCategory = detail.groupCategory,
                        id_size = detail.id_size,
                        size = detail.size,
                        orderSize = detail.orderSize,
                        price = detail.price * factorLbsToCurrent,
                        totalMU = detail.totalMU * factorCurrentToLbs,
                        totalToPay = detail.totalToPay//(detail.price * factorLbsToCurrent * detail.totalMU * factorCurrentToLbs)
                    };

                    listProductionLotLiquidationPaymentDetailFinalAux.Add(newDetail);
                };

                var logisticTotalToPayAux = 0;
                var liquidationPaymentTotalToPayAux = logisticTotalToPayAux + listProductionLotLiquidationPaymentDetailFinalAux.Sum(s => s.totalToPay);
                var rteFlePorCientoAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("RFPC"));
                var rteFlePorCientoValorAux = (Convert.ToDecimal(rteFlePorCientoAux?.value ?? "0") * liquidationPaymentTotalToPayAux) / 100;
                productionLotLiquidationPaymentReport.DataSource = new ProductionLotLiquidationPaymentsCompany
                {
                    //state = purchasePlanning?.Document.DocumentState.name ?? "",
                    liquidationPaymentDate = productionLot?.liquidationPaymentDate.Value.ToString("dd/MM/yyyy hh:mm tt") ?? "",
                    provider = productionLot?.Provider.Person.fullname_businessName ?? "",
                    number = productionLot?.number ?? "",
                    list_id_productionLotLiquidationPayment = new List<int>(id),
                    listProductionLotLiquidationPaymentDetail = listProductionLotLiquidationPaymentDetailFinalAux,
                    logisticTotalLbs = 0,
                    logisticPrecioLbs = 0,
                    logisticTotalToPay = logisticTotalToPayAux,
                    liquidationPaymentTotalLbs = listProductionLotLiquidationPaymentDetailFinalAux.Sum(s => s.totalMU),
                    liquidationPaymentTotalToPay = liquidationPaymentTotalToPayAux,
                    rteFlePorCientoValor = rteFlePorCientoValorAux,
                    rteFlePorCientoName = rteFlePorCientoAux?.name ?? "(-)Rte Fle 0%",
                    total = liquidationPaymentTotalToPayAux - rteFlePorCientoValorAux,
                    company = company
                };
            }
            catch (Exception e)
            {
                TempData.Keep("productionLotReception");
                ViewData["EditMessage"] = e.Message;
                return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
                //trans.Rollback();
            }
            return PartialView("_ProductionLotLiquidationPaymentReport", productionLotLiquidationPaymentReport);
        }

        #endregion

        #region GENERATE LOT RECEPTION

        [HttpPost]
        public ActionResult ProductionLotReceptionGenerateAccounting(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            try
            {
                Thread.Sleep(5000);//Este ejemplo solo espera 10 segundos
            }
            catch (Exception)
            {


            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProductionLotReceptionGenerateElectronicInvoice(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            try
            {
                Thread.Sleep(5000);//Este ejemplo solo espera 10 segundos
            }
            catch (Exception)
            {


            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            int id_menu = (int)ViewData["id_menu"];
            var tienePermisioConciliar = this.ActiveUser
                    .UserMenu.FirstOrDefault(e => e.id_menu == id_menu)?
                    .Permission?.FirstOrDefault(p => p.name == "Conciliar");

            var actions = new
            {
                btnApprove = false,
                btnAutorize = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
                btnConciliation = false,
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            ProductionLot productionLot = db.ProductionLot.FirstOrDefault(r => r.id == id);
            string code_state = productionLot.ProductionLotState.code;

            if (code_state == "01") // PENDIENTE DE RECEPCION
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false,
                    btnConciliation = false,
                };
            }
            else if (code_state == "02") // RECEPCIONADO
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                    btnConciliation = false,
                };
            }
            else if (code_state == "03") // PENDIENTE DE PROCESAMIENTO
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                    btnConciliation = false,
                };
            }
            else if (code_state == "04") // EN PROCESAMIENTO
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                    btnConciliation = false,
                };
            }
            else if (code_state == "05") // PENDIENTE DE CIERRE
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                    btnConciliation = false,
                };
            }
            else if (code_state == "06") // CERRADO
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                    btnConciliation = false,
                };
            }
            else if (code_state == "07") // PENDIENTE DE PAGO
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                    btnConciliation = false,
                };
            }
            else if (code_state == "08") // PAGADO
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                    btnConciliation = tienePermisioConciliar != null,
                };
            }
            else if (code_state == "09") // ANULADO
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                    btnConciliation = false,
                };
            }
            else if (code_state == "11") // CONCILIADO
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = true,
                    btnConciliation = false,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            TempData.Keep("productionLotReception");

            int index = db.ProductionLot.Where(p => p.ProductionProcess.code == "REC").OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_productionLot);

            var result = new
            {
                maximunPages = db.ProductionLot.Where(p => p.ProductionProcess.code == "REC").Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = db.ProductionLot.Where(p => p.ProductionProcess.code == "REC").OrderByDescending(p => p.id).Take(page).ToList().Last();
            if (productionLot != null)
            {
                if (productionLot.internalNumber.Length > 5)
                {
                    productionLot.julianoNumber = productionLot.internalNumber.Substring(0, 5);
                    productionLot.internalNumber = productionLot.internalNumber.Substring(5, (productionLot.internalNumber.Length - 5));
                }
                else
                {
                    productionLot.julianoNumber = productionLot.internalNumber;
                    productionLot.internalNumber = "";
                }
                productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;
            }
            if (productionLot != null)
            {
                this.ViewBag.isCopacking = productionLot.isCopackingLot ?? false;
                TempData["productionLotReception"] = productionLot;
                TempData.Keep("productionLotReception");
                return PartialView("_ProductionLotReceptionEditFormPartial", productionLot);
            }

            TempData.Keep("productionLotReception");

            return PartialView("_ProductionLotReceptionEditFormPartial", new ProductionLot());
        }

        #endregion

        #region AXILIAR FUNCTIONS
        [HttpPost]
        public JsonResult GetIdLoteCertificacion(int? id_certification)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            TempData.Keep("productionLotReception");

            var aCertification = db.Certification.FirstOrDefault(fod => fod.id == id_certification);

            var result = new { IdLote = aCertification?.idLote ?? "" };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotReceptionItemDataRemissionGuide(int? id, int? id_item, int? id_purchaseOrder, int? id_remissionGuide)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            //Item item = db.Item.FirstOrDefault(p => p.id == id_item);
            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(p => p.id == id_purchaseOrder);
            id_item = purchaseOrder?.PurchaseOrderDetail.FirstOrDefault()?.id_item;
            var id_personProcessPlant = purchaseOrder?.id_personProcessPlant;

            RemissionGuide remissionGuide = db.RemissionGuide.FirstOrDefault(p => p.id == id_remissionGuide);
            var rgd = remissionGuide?.RemissionGuideDetail.ToList();

            //decimal valReceived = rgd?.Select(a => (a.ProductionLotDetailPurchaseDetail
            //										 .Where(wh => wh.ProductionLotDetail.ProductionLot.Lot.Document.DocumentState.code != "05")
            //										 .Sum(s => s.ProductionLotDetail.quantityRecived)))?.FirstOrDefault() ?? 0;

            //decimal quantityPendingAux = ((rgd?.Select(s => s.quantityProgrammed)?.DefaultIfEmpty(0)?.Sum() ?? 0) - valReceived);

            //var quantityGuidePending = quantityPendingAux;
            var quantityGuidePending = (remissionGuide?.RemissionGuideDetail.FirstOrDefault(fod => fod.id_item == id_item && fod.isActive)?.quantityProgrammed ?? 0) -
                                       (remissionGuide?.RemissionGuideDetail.FirstOrDefault(fod => fod.id_item == id_item)?.quantityReceived ?? 0);
            decimal quantityOrdered = remissionGuide?.RemissionGuideDetail.FirstOrDefault(fod => fod.id_item == id_item)?.quantityOrdered ?? 0;
            decimal quantityRemitted = remissionGuide?.RemissionGuideDetail.FirstOrDefault(fod => fod.id_item == id_item)?.quantityProgrammed ?? 0;
            TempData.Keep("productionLotReception");

            return Json(new
            {
                quantityOrdered = quantityGuidePending,
                quantityRemitted = quantityGuidePending,
                remissionGuideGuiaExterna = !string.IsNullOrEmpty(remissionGuide.Guia_Externa) ? remissionGuide.Guia_Externa : string.Empty,
                id_item,
                process = db.Person.FirstOrDefault(fod => fod.id == id_personProcessPlant)?.processPlant
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemComboboxPurcharseOrder(int? id_purchaseOrder)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxPurcharseOrderColumnProperties(id_purchaseOrder);
            TempData.Keep("productionLotReception");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxPurcharseOrderColumnProperties(int? id_purchaseOrder)
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            var id_purchaseOrderDetailAux = productionLot.ProductionLotDetail.FirstOrDefault()?.ProductionLotDetailPurchaseDetail.FirstOrDefault()?.id_purchaseOrderDetail;
            var id_priceListAux = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == id_purchaseOrderDetailAux)?.PurchaseOrder.id_priceList;
            var purcharseOrders = new List<SelectListItem>();

            if (id_purchaseOrder != null)
            {
                purcharseOrders = db.PurchaseOrder.Where(e => e.id == id_purchaseOrder ||
                                                              (e.id_provider == productionLot.id_provider &&
                                                               e.id_productionUnitProvider == productionLot.id_productionUnitProvider &&
                                                               e.id_personProcessPlant == productionLot.id_personProcessPlant &&
                                                               e.id_priceList == id_priceListAux))
                                                                .Select(s => new SelectListItem
                                                                {
                                                                    Text = s.Document.number,
                                                                    Value = s.id.ToString(),
                                                                }).ToList();
            }
            else
            {
                purcharseOrders = db.PurchaseOrder.Where(e => (e.id_provider == productionLot.id_provider &&
                                                                           e.id_productionUnitProvider == productionLot.id_productionUnitProvider &&
                                                                           e.id_personProcessPlant == productionLot.id_personProcessPlant &&
                                                                           e.id_priceList == id_priceListAux))
                                                                            .Select(s => new SelectListItem
                                                                            {
                                                                                Text = s.Document.number,
                                                                                Value = s.id.ToString(),
                                                                            }).ToList();
            }


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.CallbackRouteValues = new { Controller = "ProductionLotReception", Action = "LoadItemComboboxPurcharseOrder" };
            //p.ClientInstanceName = "id_purcharseOrder";
            p.ValueField = "Value";
            p.TextField = "Text";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
            //p.Columns.Add("masterCode", "Código.", 80);//, Unit.Percentage(50));
            //p.Columns.Add("name", "Nombre del Producto", 300);
            //p.Columns.Add("auxCode", "Cod.Aux.", 160);
            //p.Columns.Add("Presentation.MetricUnit.code", "U.M.", 50);
            //p.Columns.Add("description2", "Descripcion", 0);

            p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.ClientSideEvents.SelectedIndexChanged = "PurcharseOrder_SelectedIndexChanged";
            p.ClientSideEvents.BeginCallback = "PurcharseOrder_BeginCallback";
            p.ClientSideEvents.EndCallback = "PurcharseOrder_EndCallback";
            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
            p.ClientSideEvents.Validation = "OnPurcharseOrderValidation";
            p.BindList(purcharseOrders);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemComboboxRemissionGuide(int? id_purchaseOrder, int? id_remissionGuide)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxRemissionGuideColumnProperties(id_purchaseOrder, id_remissionGuide);
            TempData.Keep("productionLotReception");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxRemissionGuideColumnProperties(int? id_purchaseOrder, int? id_remissionGuide)
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();

            var remissionGuides = new List<SelectListItem>();

            var model = db.PurchaseOrderDetail.Where(d => d.PurchaseOrder.Document.EmissionPoint.id_company == this.ActiveCompanyId &&
                                                          (d.PurchaseOrder.Document.DocumentState.code == "06") &&
                                                          d.Item.InventoryLine.code == "MP" && //MP: Materia Prima(Para la producción)
                                                          (d.id_purchaseOrder == id_purchaseOrder)).ToList();// "06" AUTORIZADA
            foreach (var m in model)
            {
                if (m.RemissionGuideDetailPurchaseOrderDetail != null)
                {
                    foreach (var mrgdpod in m.RemissionGuideDetailPurchaseOrderDetail)
                    {
                        if (mrgdpod.RemissionGuideDetail.quantityReceived < mrgdpod.RemissionGuideDetail.quantityProgrammed &&
                            mrgdpod.RemissionGuideDetail.RemissionGuide.Document.EmissionPoint.id_company == this.ActiveCompanyId &&
                            (mrgdpod.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code == "06" ||
                            mrgdpod.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code.Equals("09") ||
                            mrgdpod.RemissionGuideDetail.RemissionGuide.Document.DocumentState.code.Equals("03")) &&
                            ((mrgdpod.RemissionGuideDetail.RemissionGuide.RemissionGuideTransportation.isOwn == false &&
                                mrgdpod.RemissionGuideDetail.RemissionGuide.hasExitPlanctProduction == true &&
                                mrgdpod.RemissionGuideDetail.RemissionGuide.hasEntrancePlanctProduction == true) ||
                                (mrgdpod.RemissionGuideDetail.RemissionGuide.RemissionGuideTransportation.isOwn == true &&
                                mrgdpod.RemissionGuideDetail.RemissionGuide.hasEntrancePlanctProduction == true))
                            && (mrgdpod.RemissionGuideDetail.RemissionGuide.Document.isOpen != true))// "06" AUTORIZADA
                        {
                            if (productionLot.ProductionLotDetail.FirstOrDefault(fod => fod.ProductionLotDetailPurchaseDetail.FirstOrDefault(fod2 => fod2.id_remissionGuideDetail == mrgdpod.id_remissionGuideDetail) != null) != null)
                            {
                                continue;
                            }
                            remissionGuides.Add(new SelectListItem
                            {
                                Text = mrgdpod.RemissionGuideDetail.RemissionGuide.Document.number,
                                Value = mrgdpod.RemissionGuideDetail.id_remisionGuide.ToString(),
                            });
                        }
                    }
                }
            }


            if (id_purchaseOrder != null && id_remissionGuide != null)
            {
                if (remissionGuides.FirstOrDefault(fod => fod.Value == id_remissionGuide.ToString()) == null)
                {
                    remissionGuides.Add(new SelectListItem
                    {
                        Text = db.RemissionGuide.FirstOrDefault(fod => fod.id == id_remissionGuide).Document.number,
                        Value = id_remissionGuide.Value.ToString(),
                    });
                }
            }


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.CallbackRouteValues = new { Controller = "ProductionLotReception", Action = "LoadItemComboboxRemissionGuide" };
            //p.ClientInstanceName = "id_purcharseOrder";
            p.ValueField = "Value";
            p.TextField = "Text";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
            //p.Columns.Add("masterCode", "Código.", 80);//, Unit.Percentage(50));
            //p.Columns.Add("name", "Nombre del Producto", 300);
            //p.Columns.Add("auxCode", "Cod.Aux.", 160);
            //p.Columns.Add("Presentation.MetricUnit.code", "U.M.", 50);
            //p.Columns.Add("description2", "Descripcion", 0);

            p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.ClientSideEvents.SelectedIndexChanged = "RemissionGuide_SelectedIndexChanged";
            p.ClientSideEvents.BeginCallback = "RemissionGuide_BeginCallback";
            p.ClientSideEvents.EndCallback = "RemissionGuide_EndCallback";
            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
            p.ClientSideEvents.Validation = "OnRemissionGuideValidation";
            p.BindList(remissionGuides);
            return p;

        }

        private void GenerateResultForDrainingTest(int idProductionLot)
        {
            using (DBContext db = new DBContext())
            {
                try
                {

                    List<EntityParameters> parameterMaxLb = (List<EntityParameters>)db.AdvanceParameters.FindParameters("PRESC");

                    if (parameterMaxLb == null) throw new Exception("No existe configuración Parametros Prueba de Escurrido");

                    EntityParameters entParameterMaxLb = parameterMaxLb.FirstOrDefault(r => r.code == "ESENT");
                    if (entParameterMaxLb == null) throw new Exception("No existe valor parámetro valor Mínimo prueba de escurrido");

                    string codeMetricUnit = entParameterMaxLb.valueString;
                    int valueParameterMaxLb = (int)entParameterMaxLb.valueInteger;


                    var lstPlDetail = db.ProductionLotDetail
                                            .Where(w => w.id_productionLot == idProductionLot)
                                            .ToList();

                    foreach (var det in lstPlDetail)
                    {

                        if (det.quantityRecived < valueParameterMaxLb)
                        {
                            continue;
                        }

                        ResultProdLotReceptionDetail rplrd;

                        //var _dtTmp = det.ProductionLotDetailPurchaseDetail.Select(s => s.RemissionGuideDetail.RemissionGuide.RemissionGuideControlVehicle.entranceDateProductionBuilding).FirstOrDefault();
                        //var _tiTmp = det.ProductionLotDetailPurchaseDetail.Select(s => s.RemissionGuideDetail.RemissionGuide.RemissionGuideControlVehicle.entranceTimeProductionBuilding).FirstOrDefault();
                        //DateTime _dt = new DateTime(_dtTmp.Value.Year, _dtTmp.Value.Month, _dtTmp.Value.Day,_tiTmp.Value.Hours, _tiTmp.Value.Minutes, _tiTmp.Value.Seconds);

                        if (det.ResultProdLotReceptionDetail == null)
                        {
                            rplrd = new ResultProdLotReceptionDetail
                            {
                                idRemissionGuide = det.ProductionLotDetailPurchaseDetail.Select(s => s.RemissionGuideDetail.RemissionGuide.id).FirstOrDefault(),
                                numberRemissionGuide = det.ProductionLotDetailPurchaseDetail.Select(s => s.RemissionGuideDetail.RemissionGuide.Document.sequential.ToString()).FirstOrDefault(),
                                dateTimeArrived = det.ProductionLot.receptionDate,
                                //det.ProductionLotDetailPurchaseDetail.Select(s => s.RemissionGuideDetail.RemissionGuide.RemissionGuideControlVehicle.entranceDateProductionBuilding).FirstOrDefault(),
                                poundsRemitted = det.quantityRecived,
                                drawersNumber = det.drawersNumber ?? 0,
                                numberLot = det.ProductionLot.internalNumber,
                                numberLotSequential = det.ProductionLot.number,
                                namePool = det.ProductionLot.ProductionUnitProviderPool.name,
                                nameProvider = det.ProductionLot.Provider.Person.fullname_businessName,
                                INPnumber = det.ProductionLot.ProductionUnitProvider.INPnumber,
                                idWarehouse = det.id_warehouse,
                                nameWarehouse = det.Warehouse.name,
                                idWarehouseLocation = det.id_warehouseLocation,
                                nameWarehouseLocation = det.WarehouseLocation.name,
                                nameProviderShrimp = det.ProductionLot.ProductionUnitProvider.name,
                                idItem = det.id_item,
                                nameItem = det.Item.name,
                                idMetricUnit = det.Item.ItemInventory.id_metricUnitInventory
                            };
                            det.ResultProdLotReceptionDetail = rplrd;
                            db.ResultProdLotReceptionDetail.Attach(rplrd);
                            db.Entry(rplrd).State = EntityState.Added;
                        }
                        else
                        {
                            rplrd = det.ResultProdLotReceptionDetail;
                            rplrd.idRemissionGuide = det.ProductionLotDetailPurchaseDetail.Select(s => s.RemissionGuideDetail.RemissionGuide.id).FirstOrDefault();
                            rplrd.numberRemissionGuide = det.ProductionLotDetailPurchaseDetail.Select(s => s.RemissionGuideDetail.RemissionGuide.Document.sequential.ToString()).FirstOrDefault();
                            rplrd.dateTimeArrived = det.ProductionLot.receptionDate;
                            //det.ProductionLotDetailPurchaseDetail.Select(s => s.RemissionGuideDetail.RemissionGuide.RemissionGuideControlVehicle.entranceDateProductionBuilding).FirstOrDefault();
                            rplrd.poundsRemitted = det.quantityRecived;
                            rplrd.drawersNumber = det.drawersNumber ?? 0;
                            rplrd.numberLot = det.ProductionLot.internalNumber;
                            rplrd.numberLotSequential = det.ProductionLot.number;
                            rplrd.namePool = det.ProductionLot.ProductionUnitProviderPool.name;
                            rplrd.nameProvider = det.ProductionLot.Provider.Person.fullname_businessName;
                            rplrd.INPnumber = det.ProductionLot.ProductionUnitProvider.INPnumber;
                            rplrd.idWarehouse = det.id_warehouse;
                            rplrd.nameWarehouse = det.Warehouse.name;
                            rplrd.idWarehouseLocation = det.id_warehouseLocation;
                            rplrd.nameProviderShrimp = det.ProductionLot.ProductionUnitProvider.name;
                            rplrd.nameWarehouseLocation = det.WarehouseLocation.name;
                            rplrd.idItem = det.id_item;
                            rplrd.nameItem = det.Item.name;
                            rplrd.idMetricUnit = det.Item.ItemInventory.id_metricUnitInventory;
                            det.ResultProdLotReceptionDetail = rplrd;
                            db.ResultProdLotReceptionDetail.Attach(rplrd);
                            db.Entry(rplrd).State = EntityState.Modified;
                        }
                        db.SaveChanges();

                    }
                }
                catch //(Exception ex)
                {

                }

            }

        }

        [HttpPost]
        public JsonResult ValidateInternalNumberLot(int idPl, string internalNum, string julianoNum, int? id_provider, int? id_productionUnitProvider, int? id_productionUnitProviderPool)
        {
            string internalNumberTotal = julianoNum + internalNum;

            var result = new
            {
                itsAssigned = "NO",
                Error1 = ""
            };
            ProductionLot plTmp = null;
            var aid_provider = id_provider;
            var aid_productionUnitProvider = id_productionUnitProvider;
            var aid_productionUnitProviderPool = id_productionUnitProviderPool;
            var settingNLR = db.Setting.FirstOrDefault(fod => fod.code == "NLR");
            var error1Aux = "";
            var recepcionCode = db.ProductionProcess.FirstOrDefault(fod => fod.code == "REC");
            if (settingNLR != null && settingNLR.value == "SI")
            {
                plTmp = db.ProductionLot.FirstOrDefault(fod => fod.internalNumber == internalNumberTotal &&
                                                               fod.id != idPl &&
                                                               fod.ProductionLotState.code != "09" &&
                                                               (fod.id_provider != aid_provider ||
                                                                fod.id_productionUnitProvider != aid_productionUnitProvider ||
                                                                fod.id_productionUnitProviderPool != aid_productionUnitProviderPool) &&
                                                                fod.id_productionProcess == recepcionCode.id);
                if (plTmp != null)
                {
                    var diferencia = (plTmp.id_provider != aid_provider ? "el proveedor" : "");
                    var aAux = (plTmp.id_productionUnitProvider != aid_productionUnitProvider ? "la camaronera" : "");
                    diferencia += diferencia != "" && aAux != "" ? ", " + aAux : aAux;
                    aAux = (plTmp.id_productionUnitProviderPool != aid_productionUnitProviderPool ? "la piscina" : "");
                    diferencia += diferencia != "" && aAux != "" ? ", " + aAux : aAux;
                    error1Aux = "El número de Lote Interno ya está siendo utilizado en la recepción " + plTmp.number + " y tiene diferente: (" +
                                 diferencia + ")";
                }
            }
            else
            {
                plTmp = db.ProductionLot.FirstOrDefault(fod => fod.internalNumber == internalNumberTotal && fod.id != idPl && fod.ProductionLotState.code != "09");
                if (plTmp != null)
                {
                    error1Aux = "El número de Lote Interno ya está siendo utilizado en la recepción " + plTmp.number;
                }
            }



            result = new
            {
                itsAssigned = (plTmp != null) ? "YES" : "NO",
                Error1 = error1Aux//(plTmp != null) ? "El número de Lote Interno ya está siendo utilizado en la recepción " + plTmp.number : ""
            };

            TempData.Keep("productionLotReception");
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult UpdateProductionLotJulianoNumber(int? id_pl, string intNumber, int yearR, int monthR, int dayR)
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(fod => fod.id == id_pl);
            productionLot = productionLot ?? new ProductionLot();

            DateTime? dateReceptionNow = new DateTime(yearR, monthR, dayR);

            dateReceptionNow = dateReceptionNow ?? DateTime.Now;

            productionLot.julianoNumber = DataProviderJulianoNumber.GetJulianoNumber(dateReceptionNow.Value);
            productionLot.internalNumber = intNumber;

            productionLot.internalNumberConcatenated = productionLot.julianoNumber + productionLot.internalNumber;

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");


            return Json(new
            {
                julianoNumberTmp = productionLot.julianoNumber,
                internalNumberTmp = productionLot.internalNumber,
                internalNumberConcatenatedTmp = productionLot.internalNumberConcatenated
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSelectedRowsRemissionGuide(int[] ids)
        {
            var result = new
            {
                Message = "OK"
            };

            int? idPupFirst = null;
            int? idProviderFirst = null;
            //int? idPurchaseOrderFirst = null;
            int? id_personProcessPlantFirst = null;
            Boolean? copack = null;
            int? id_priceListFirst = null;

            var etiquetaPup = DataProviderSetting.ValueSetting("EUPPPRIM");
            if (String.IsNullOrEmpty(etiquetaPup))
            {
                etiquetaPup = "Unidad de Producción";
            }

            foreach (var i in ids)
            {
                var rgCurrent = db.RemissionGuide.FirstOrDefault(fod => fod.id == i);

                var pupCurrent = rgCurrent?.id_productionUnitProvider;
                if (pupCurrent.HasValue)
                {
                    var existePupPool = db.ProductionUnitProviderPool.Any(fod => fod.id_productionUnitProvider == pupCurrent);
                    if (!existePupPool)
                    {
                        result = new
                        {
                            Message = ErrorMessage("La " + etiquetaPup + " de la guía " + (rgCurrent?.Document?.number ?? "") + " no tiene piscina")
                        };
                        TempData.Keep("productionLotReception");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                    if (idPupFirst.HasValue)
                    {
                        if (pupCurrent != idPupFirst)
                        {
                            result = new
                            {
                                Message = ErrorMessage("No se pueden mezclar detalles con " + etiquetaPup + " diferentes")
                            };
                            TempData.Keep("productionLotReception");
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        idPupFirst = pupCurrent;
                    }
                }

                var providerCurrent = rgCurrent?.id_providerRemisionGuide;
                if (providerCurrent.HasValue)
                {
                    if (idProviderFirst.HasValue)
                    {
                        if (providerCurrent != idProviderFirst)
                        {
                            result = new
                            {
                                Message = ErrorMessage("No se pueden mezclar detalles con proveedores propuestos diferentes")
                            };
                            TempData.Keep("productionLotReception");
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        idProviderFirst = providerCurrent;
                    }
                }

                var idPersonProcessPlantCurrent = rgCurrent?.id_personProcessPlant;
                if (idPersonProcessPlantCurrent.HasValue)
                {
                    if (id_personProcessPlantFirst.HasValue)
                    {
                        if (idPersonProcessPlantCurrent != id_personProcessPlantFirst)
                        {
                            result = new
                            {
                                Message = ErrorMessage("No se pueden mezclar detalles con procesos diferentes")
                            };
                            TempData.Keep("productionLotReception");
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        id_personProcessPlantFirst = idPersonProcessPlantCurrent;
                    }
                }

                var copackRg = rgCurrent?.isCopackingRG;
                if (copackRg == null)
                    copackRg = false;


                if (copackRg.HasValue)
                {
                    if (copack.HasValue)
                    {
                        if (copack != copackRg)
                        {
                            result = new
                            {
                                Message = ErrorMessage("No se pueden mezclar detalles con procesos copacking diferentes")
                            };
                            TempData.Keep("productionLotReception");
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        copack = copackRg;
                    }
                }

                var aId_priceListCurrent = rgCurrent?.id_priceList;
                if (aId_priceListCurrent.HasValue)
                {
                    if (id_priceListFirst.HasValue)
                    {
                        if (aId_priceListCurrent != id_priceListFirst)
                        {
                            result = new
                            {
                                Message = ErrorMessage("No se pueden mezclar detalles con listas de precio diferentes")
                            };
                            TempData.Keep("productionLotReception");
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        id_priceListFirst = aId_priceListCurrent;
                    }
                }
                else
                {
                    result = new
                    {
                        Message = ErrorMessage($"La Guía de Remisión: {(rgCurrent?.Document?.number ?? string.Empty)} , seleccionada en detalle no tiene listas de precio definida")
                    };
                    TempData.Keep("productionLotReception");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

            TempData.Keep("purchaseOrder");
            TempData.Keep("model");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult GetEnabledBtnGenerateLot(int[] ids)
        {
            var listPendingPurchaseOrdersAndRemissionGuides =
                (TempData["listPendingPurchaseOrdersAndRemissionGuides"] as List<PendingPurchaseOrdersAndRemissionGuides>)
                ?? new List<PendingPurchaseOrdersAndRemissionGuides>();

            var enabledBtnGenerateLotAux = false;
            var id_providerAux = 0;

            foreach (var id in ids)
            {
                if (id_providerAux == 0)
                {
                    var pendingPurchaseOrdersAndRemissionGuidesAux = listPendingPurchaseOrdersAndRemissionGuides.FirstOrDefault(fod => fod.id == id);
                    id_providerAux = pendingPurchaseOrdersAndRemissionGuidesAux?.id_provider ?? 0;
                    enabledBtnGenerateLotAux = true;
                }
                else
                {
                    var pendingPurchaseOrdersAndRemissionGuidesAux = listPendingPurchaseOrdersAndRemissionGuides.FirstOrDefault(fod => fod.id == id);
                    if (id_providerAux != (pendingPurchaseOrdersAndRemissionGuidesAux?.id_provider ?? 0))
                    {
                        enabledBtnGenerateLotAux = false;
                        break;
                    }
                }
            }

            var result = new
            {
                enabledBtnGenerateLot = enabledBtnGenerateLotAux

            };

            TempData.Keep("listPendingPurchaseOrdersAndRemissionGuides");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewDetailsPagoUpdate(ProductionLotPayment item, bool? enabled)
        {
            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var index = productionLot.ProductionLotPayment.ToList().FindIndex(it => it.id == item.id);

                    if (index >= 0)
                    {
                        productionLot.ProductionLotPayment.ElementAt(index).price = item.price;
                        productionLot.ProductionLotPayment.ElementAt(index).totalToPay = item.price * productionLot.ProductionLotPayment.ElementAt(index).totalProcessMetricUnit;
                        productionLot.ProductionLotPayment.ElementAt(index).totalToPayEq = item.price * productionLot.ProductionLotPayment.ElementAt(index).totalProcessMetricUnitEq;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                foreach (var modelState in this.ModelState.Values)
                {
                    if (modelState.Errors.Count > 0)
                    {
                        ViewData["EditError"] = modelState.Errors.Last().ErrorMessage;
                        break;
                    }
                }
            }

            ViewBag.enabled = enabled;
            if (productionLot.ProductionLotState.code == "07")
            {
                ViewBag.ShowActionDis = true;
            }
            else
            {
                ViewBag.ShowActionDis = false;
            }
            var modPrecio = db.Setting.FirstOrDefault(fod => fod.code.Equals("MODPREC"));
            if (modPrecio.value == "SI" && (productionLot.ProductionLotState.code == "07" || productionLot.ProductionLotState.code == "08"))
            {
                ViewBag.ShowModifPrecio = true;
                ViewBag.ShowModifDetalle = (productionLot.ProductionLotState.code == "07") ? true : false;
            }
            else
            {
                ViewBag.ShowModifPrecio = false;
                ViewBag.ShowModifDetalle = false;

            }

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            if (productionLot.isCopackingLot ?? false)
            {
                return PartialView("_ProductionLotReceptionEditFormProductionLotPaymentDetailPartialCopacking", productionLot.ProductionLotPayment.ToList());
            }
            else
            {
                return PartialView("_ProductionLotReceptionEditFormProductionLotPaymentDetailPartial", productionLot.ProductionLotPayment.ToList());
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionLotReceptionWarehouseLocation(int? id_warehouse)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var result = new
            {
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse)
                                       .Select(s => new
                                       {
                                           s.id,
                                           s.name
                                       })

            };

            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetProductionLotReceptionWarehouseLocation(int? id_warehouse, int? id_warehouseLocation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var warehouseLocationAux = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse).ToList();
            var warehouseLocationCurrentAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocation);
            if (warehouseLocationCurrentAux != null && !warehouseLocationAux.Contains(warehouseLocationCurrentAux)) warehouseLocationAux.Add(warehouseLocationCurrentAux);

            var result = new
            {
                warehouseLocations = warehouseLocationAux
                                       .Select(s => new
                                       {
                                           s.id,
                                           s.name
                                       })

            };

            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionLotPaymentDetails(int? id_priceList)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var modPrecio = db.Setting.FirstOrDefault(fod => fod.code.Equals("MODPREC"));

            var model = productionLot?.ProductionLotPayment.ToList() ?? new List<ProductionLotPayment>();
            var Message = "";

            if (productionLot.isCopackingLot == null)
                productionLot.isCopackingLot = false;

            if ((bool)productionLot.isCopackingLot)
            {
                CopackingTariff copackingTariffAux;
                CopackingTariffDetail copackingTariffDetail;

                copackingTariffAux = db.CopackingTariff.FirstOrDefault(fod => fod.id == id_priceList);

                if (copackingTariffAux == null)
                {
                    foreach (var productionLotPayment in model)
                    {
                        var ItemAux = db.Item.FirstOrDefault(fod => fod.masterCode == productionLotPayment.Item.masterCode);
                        var valueDefault = 0;
                        productionLotPayment.price = valueDefault;
                        productionLotPayment.priceEdition = valueDefault;
                        productionLotPayment.totalToPay = decimal.Round((productionLotPayment.price * productionLotPayment.totalProcessMetricUnit), 2, MidpointRounding.AwayFromZero);
                        productionLotPayment.totalToPayEq = decimal.Round((productionLotPayment.price * productionLotPayment.totalProcessMetricUnitEq), 2, MidpointRounding.AwayFromZero);
                        productionLot.totalToPay += productionLotPayment.totalToPay;
                        productionLot.totalToPayEq += productionLotPayment.totalToPayEq;

                        var codeAux = ItemAux?.ItemType?.ProcessType?.code ?? "";
                        if (codeAux == "ENT")
                        {
                            productionLot.wholeTotalToPay += productionLotPayment.totalToPay;
                        }
                        else
                        {
                            productionLot.tailTotalToPay += productionLotPayment.totalToPay;
                        }


                        this.UpdateModel(productionLotPayment);
                    }
                }
                else
                {
                    List<Class> lstClass = db.Class.ToList();

                    int idBrok = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("BROK")).id;
                    int idVetl = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("VETL")).id;

                    productionLot.totalToPay = 0.0M;
                    productionLot.totalToPayEq = 0.0M;
                    productionLot.wholeTotalToPay = 0.0M;
                    productionLot.tailTotalToPay = 0.0M;

                    foreach (var productionLotPayment in model)
                    {
                        var ItemAux = db.Item.FirstOrDefault(fod => fod.masterCode == productionLotPayment.Item.masterCode);

                        copackingTariffDetail = db.CopackingTariffDetail.FirstOrDefault(fod => fod.CopackingTariff.code == copackingTariffAux.code
                                                && (ItemAux.InventoryLine.code == fod.InventoryLine.code && ItemAux.ItemType.code == fod.ItemType.code));

                        if (copackingTariffDetail == null)
                        {
                            Message += Message == "" ? "Prodcuto(s): " + productionLotPayment.Item.name : ", " + productionLotPayment.Item.name;
                        }
                        else
                        {
                            productionLotPayment.price = copackingTariffDetail.tariff;
                            productionLotPayment.priceEdition = copackingTariffDetail.tariff;
                            productionLotPayment.totalToPay = decimal.Round((productionLotPayment.price * productionLotPayment.totalProcessMetricUnit), 2, MidpointRounding.AwayFromZero);
                            productionLotPayment.totalToPayEq = decimal.Round((productionLotPayment.price * productionLotPayment.totalProcessMetricUnitEq), 2, MidpointRounding.AwayFromZero);
                            productionLot.totalToPay += productionLotPayment.totalToPay;
                            productionLot.totalToPayEq += productionLotPayment.totalToPayEq;
                        }


                        var codeAux = ItemAux?.ItemType?.ProcessType?.code ?? "";
                        if (codeAux == "ENT")
                        {
                            productionLot.wholeTotalToPay += productionLotPayment.totalToPay;
                        }
                        else
                        {
                            productionLot.tailTotalToPay += productionLotPayment.totalToPay;
                        }


                        this.UpdateModel(productionLotPayment);
                    }

                }

                if (Message != "")
                {
                    Message += ". Debe definirle precio en la lista de precio escogida, configúrelo e intente de nuevo";
                }

                TempData["productionLotReception"] = productionLot;
                TempData.Keep("productionLotReception");

                Message = Message != "" ? Message : "Ok";
            }
            else
            {
                PriceList priceListAux;

                int id_priceListBase = 0;

                priceListAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
                id_priceListBase = priceListAux?.id_priceListBase ?? 0;

                decimal priceAux;

                //int idGroupPerson = db.GroupPersonByRolDetail
                //				.FirstOrDefault(fod => fod.id_person == productionLot.id_provider)?
                //				.id_groupPersonByRol ?? 0;

                //List<ItemSizePriceClass> lstPreciosClases = null;
                //List<Class> lstClass = db.Class.ToList();

                //int idBrok = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("BROK")).id;
                //int idVetl = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("VETL")).id;

                //if (idGroupPerson > 0)
                //{
                //	lstPreciosClases = db.ItemSizePriceClass
                //							.Where(w => w.idGroupPersonByRol == idGroupPerson)
                //							.ToList();
                //}

                //if (lstPreciosClases == null || lstPreciosClases.Count == 0)
                //{
                //	lstPreciosClases = db.ItemSizePriceClass
                //							.Where(w => w.idGroupPersonByRol == null)
                //							.ToList();
                //}

                productionLot.totalToPay = 0.0M;
                productionLot.totalToPayEq = 0.0M;
                productionLot.wholeTotalToPay = 0.0M;
                productionLot.tailTotalToPay = 0.0M;

                int idClassShrimpB = db.ClassShrimp.FirstOrDefault(fod => fod.code.Trim().Equals("B"))?.id ?? 0;

                decimal valB = db.PriceListClassShrimp
                                    .FirstOrDefault(fod => fod.id_priceList == id_priceList
                                    && fod.id_classShrimp == idClassShrimpB)?.value ?? 0;

                List<PriceListDet> lispre = db.PriceListItemSizeDetail
                                    .Where(w => w.Id_PriceList == id_priceList)
                                    .Select(s => new PriceListDet
                                    {
                                        Id_Itemsize = s.Id_Itemsize,
                                        PriceA = s.price - (s.commission),
                                        PriceB = ((s.price - valB - s.commission) >= 0) ? (s.price - valB - s.commission) : 0
                                    }).ToList();

                //foreach (var det in lispre)
                //{
                //	var PrecioTalla = lstPreciosClases
                //						.FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize);

                //	if (PrecioTalla != null)
                //	{
                //		det.PriceA = lstPreciosClases
                //						.FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize && fod.idClass == idBrok)?.price ?? 0;

                //		det.PriceB = lstPreciosClases
                //						.FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize && fod.idClass == idVetl)?.price ?? 0;

                //	}
                //}
                //var a = 1;
                foreach (var productionLotPayment in model)
                {
                    var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotPayment.id_item);

                    var itemTypeCategoryClassRelationAux = db.ItemTypeCategoryClassRelation.FirstOrDefault(fod => fod.id_ItemTypeCategory == ItemAux.id_itemTypeCategory);
                    var id_classAux = itemTypeCategoryClassRelationAux?.id_Class;
                    var confProcessSeriesSizeAux = db.ConfSizeClass
                                                        .FirstOrDefault(fod => fod.idClass == id_classAux
                                                                                && fod.idItemsize == ItemAux.ItemGeneral.id_size);

                    PriceListDet pldDetail = lispre
                                                .FirstOrDefault(fod => fod.Id_Itemsize == ItemAux.ItemGeneral.id_size);
                    priceAux = 0;
                    if (pldDetail != null)
                    {
                        var codeClassShrimpAux = ItemAux.ItemGeneral.ItemGroupCategory?.code;
                        PriceListItemSizeDetail listDetail = priceListAux.PriceListItemSizeDetail.FirstOrDefault(d => d.Id_Itemsize == ItemAux.ItemGeneral.id_size &&
                                                                                                              d.ClassShrimp.code == codeClassShrimpAux &&
                                                                                                              d.id_class == id_classAux);
                        if (listDetail != null)
                        {
                            priceAux = decimal.Round(listDetail.price - listDetail.commission, 2);
                            var aItemSizePriceClass = db.ItemSizePriceClass.FirstOrDefault(fod => fod.idGroupPersonByRol == priceListAux.id_groupPersonByRol &&
                                                                                                 fod.idItemSize == ItemAux.ItemGeneral.id_size &&
                                                                                                 fod.idClass == id_classAux && fod.ClassShrimp.code == codeClassShrimpAux);
                            if (aItemSizePriceClass != null)
                            {
                                priceAux = decimal.Round(aItemSizePriceClass.price, 2);
                            }
                        }
                        else
                        {
                            listDetail = priceListAux.PriceListItemSizeDetail.FirstOrDefault(d => d.Id_Itemsize == ItemAux.ItemGeneral.id_size &&
                                                                                                              d.ClassShrimp.code == "D0" &&
                                                                                                              d.id_class == id_classAux);
                            if (listDetail != null)
                            {
                                priceAux = decimal.Round(listDetail.price - listDetail.commission, 2);
                            }
                            var aItemSizePriceClass = db.ItemSizePriceClass.FirstOrDefault(fod => fod.idGroupPersonByRol == priceListAux.id_groupPersonByRol &&
                                                                                                 fod.idItemSize == ItemAux.ItemGeneral.id_size &&
                                                                                                 fod.idClass == id_classAux && fod.ClassShrimp.code == "D0");
                            if (aItemSizePriceClass != null)
                            {
                                priceAux = decimal.Round(aItemSizePriceClass.price, 2);
                            }
                        }


                        //           priceAux = confProcessSeriesSizeAux == null || confProcessSeriesSizeAux.isPriceA ?
                        //decimal.Round((pldDetail?.PriceA ?? 0), 2) : decimal.Round((pldDetail?.PriceB ?? 0), 2);
                    }
                    if (modPrecio.value == "SI" && productionLotPayment.price != 0)
                    {
                        productionLotPayment.totalToPay = decimal.Round((productionLotPayment.price * productionLotPayment.totalProcessMetricUnit), 2, MidpointRounding.AwayFromZero);
                        productionLotPayment.totalToPayEq = decimal.Round((productionLotPayment.price * productionLotPayment.totalProcessMetricUnitEq), 2, MidpointRounding.AwayFromZero);
                    }
                    else 
                    {
                        productionLotPayment.price = priceAux;
                        productionLotPayment.totalToPay = decimal.Round((priceAux * productionLotPayment.totalProcessMetricUnit), 2, MidpointRounding.AwayFromZero);
                        productionLotPayment.totalToPayEq = decimal.Round((priceAux * productionLotPayment.totalProcessMetricUnitEq), 2, MidpointRounding.AwayFromZero);
                    }
                    productionLotPayment.priceEdition = priceAux;
                    productionLot.totalToPay += productionLotPayment.totalToPay;
                    productionLot.totalToPayEq += productionLotPayment.totalToPayEq;

                    var codeAux = ItemAux?.ItemType?.ProcessType?.code ?? "";
                    if (codeAux == "ENT")
                    {
                        productionLot.wholeTotalToPay += productionLotPayment.totalToPay;
                    }
                    else
                    {
                        productionLot.tailTotalToPay += productionLotPayment.totalToPay;
                    }
                    if (priceAux == 0)
                    {
                        Message += Message == "" ? "Prodcuto(s): " + productionLotPayment.Item.name : ", " + productionLotPayment.Item.name;
                    }

                    this.UpdateModel(productionLotPayment);

                }

                if (Message != "")
                {
                    Message += ". Debe definirle precio en la lista de precio escogida, configúrelo e intente de nuevo";
                }

                TempData["productionLotReception"] = productionLot;
                TempData.Keep("productionLotReception");

                Message = Message != "" ? Message : "Ok";
            }

            var result = new
            {
                Message,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotReceptionPriceListData(int? id_priceList, int? id_provider)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLotTemp = (TempData["productionLotReception"] as ProductionLot);
            productionLotTemp = productionLotTemp ?? new ProductionLot();

            var model = productionLotTemp?.ProductionLotDetail.ToList() ?? new List<ProductionLotDetail>();

            var priceLists = new List<PriceList>();
            PriceList priceListAux;

            priceListAux = db.PriceList.FirstOrDefault(fod => fod.id == id_priceList);
            if (priceListAux != null) priceLists.Add(priceListAux);
            var nowAux = DateTime.Now;
            var priceListProviderAndGenerals = db.PriceList.AsEnumerable().Where(w => DateTime.Compare(w.CalendarPriceList.startDate.Date, nowAux.Date) <= 0 &&
                                                                                     DateTime.Compare(nowAux.Date, w.CalendarPriceList.endDate.Date) <= 0 &&
                                                                                     w.Document.DocumentState.code.Equals("03")).ToList();//"03" Code de Estado APROBADA de La Lista de Precio

            priceListProviderAndGenerals = priceListProviderAndGenerals.Where(w => (w.id_provider == id_provider || w.id_provider == null) && w.id_customer == null).ToList();

            foreach (var detail in priceListProviderAndGenerals)
            {
                if (!priceLists.Contains(detail)) priceLists.Add(detail);
            }
            foreach (var detailProductionLot in model)
            {
                foreach (var productionLotDetailPurchaseDetail in detailProductionLot.ProductionLotDetailPurchaseDetail)
                {
                    if (productionLotDetailPurchaseDetail.PurchaseOrderDetail != null)
                    {
                        priceListAux = productionLotDetailPurchaseDetail.PurchaseOrderDetail.PurchaseOrder.PriceList;
                        if (!priceLists.Contains(priceListAux)) priceLists.Add(priceListAux);
                    }
                }
            }

            TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLotTemp;
            TempData.Keep("productionLotReception");

            var result = new
            {
                //metricUnit = productionLotLiquidation.Item.ItemPurchaseInformation.MetricUnit.code,
                priceLists = priceLists.Select(d => new
                {
                    d.id,
                    name = d.name + " (" + d.Document.DocumentType.name + ") " + d.CalendarPriceList.CalendarPriceListType.name + " [" + d.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                                 d.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]"
                }).ToList()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private decimal QuantityTotalByPresentation(Presentation presentation, decimal quantity)
        {

            if (presentation == null)
            {
                return quantity;
            }
            else
            {
                return presentation.minimum * quantity;
            }

        }

        [HttpPost]
        public JsonResult ItemDetailData(int? id_item, string quantity)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);

            decimal _quantity = Convert.ToDecimal(quantity);
            var presentation = item?.Presentation;
            decimal quantityTotal = QuantityTotalByPresentation(presentation, _quantity);


            var id_metricTypeAux = item?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.id_metricType == id_metricTypeAux) && w.isActive).Select(s => new
            {
                s.id,
                s.code
            });

            var id_metricUnitAux = item?.ItemHeadIngredient?.id_metricUnit ?? item?.ItemInventory?.id_metricUnitInventory;
            var id_metricUnitPresentation = presentation?.id_metricUnit ?? id_metricUnitAux;

            var id_warehouseAux = item?.ItemInventory?.Warehouse.id;
            var id_warehouseLocationAux = item?.ItemInventory?.WarehouseLocation.id;
            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
                                       .Select(s => new
                                       {
                                           s.id,
                                           s.name
                                       });

            var result = new
            {
                metricUnit = item.ItemPurchaseInformation?.MetricUnit?.code ?? "",
                id_metricUnit = id_metricUnitAux,
                metricUnits,
                id_warehouse = id_warehouseAux,
                id_warehouseLocation = id_warehouseLocationAux,
                warehouseLocations,
                quantityTotal,
                id_metricUnitPresentation
            };


            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DispatchMaterialDetailData(int id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot purchaseOrder = (TempData["productionLotReception"] as ProductionLot);
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                metricUnit = item.ItemPurchaseInformation.MetricUnit.code,
            };

            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductionLotTotals()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();

            TempData.Keep("productionLotReception");

            return Json(new
            {
                productionLot.totalQuantityOrdered,
                productionLot.totalQuantityRemitted,
                productionLot.totalQuantityRecived,
                productionLot.totalQuantityLiquidation,
                productionLot.totalQuantityTrash,
                productionLot.wholeSubtotal,
                productionLot.subtotalTail
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductionLotPerformances(string processType, bool adjust = false)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();

            TempData.Keep("productionLotReception");

            processType = (productionLot.wholeSubtotal > 0) ? "ENT" : "COL";

            if (adjust)
            {
                productionLot.wholeSubtotalAdjust = productionLot.wholeSubtotal;
            }
            var totalQuantityRecivedAux = productionLot.wholeSubtotalAdjust + productionLot.wholeLeftover + productionLot.wholeGarbagePounds;
            //Entero
            var wholeTotalQuantityRecivedSumary = productionLot.wholeSubtotal > 0 ? totalQuantityRecivedAux : 0.0M;

            var wholeNetSumary = Convert.ToDecimal(0);
            if (processType == "ENT")
            {
                wholeNetSumary = wholeTotalQuantityRecivedSumary - productionLot.wholeGarbagePounds;
            }
            else
            {
                wholeNetSumary = 0;
            }

            var percentWholePerformanceSumary = decimal.Round(wholeNetSumary > 0 ? (productionLot.wholeSubtotalAdjust / wholeNetSumary) : 0, 4);// * 100;

            var totalQuantityDrainedAux = GetTotalQuantityDrained(productionLot);

            //Cola
            //var tailTotalQuantityRecivedSumary = productionLot.wholeSubtotal > 0 ? productionLot.wholeLeftover : totalQuantityDrainedAux;
            var tailTotalQuantityRecivedSumary = productionLot.tailLeftOver;
            var tailNetSumary2 = (productionLot.wholeSubtotal > 0 ? productionLot.wholeLeftover : productionLot.tailLeftOver) - productionLot.poundsGarbageTail;
            var tailNetSumary = tailNetSumary2 ?? 0;

            if (processType == "COL")
            {
                tailNetSumary = (productionLot.tailLeftOver ?? 0) - (productionLot.poundsGarbageTail);
            }
            else
            {
                tailTotalQuantityRecivedSumary = productionLot.wholeLeftover;
                tailNetSumary = (tailTotalQuantityRecivedSumary ?? 0) - (productionLot.poundsGarbageTail);
            }

            var percentTailPerformanceSumary = Convert.ToDecimal(0);

            if (adjust)
            {
                productionLot.subtotalTailAdjust = productionLot.subtotalTail;
            }

            if (processType == "ENT")
            {
                percentTailPerformanceSumary = decimal.Round(tailNetSumary > 0 ? (productionLot.subtotalTailAdjust / tailNetSumary) : 0, 4);// * 100;
            }
            else
            {
                percentTailPerformanceSumary = decimal.Round(tailNetSumary > 0 ? (productionLot.subtotalTailAdjust / tailNetSumary) : 0, 4);
            }

            //Total
            var poundsGarbageTotal = productionLot.wholeGarbagePounds + productionLot.poundsGarbageTail;
            var totalTotalQuantityRecivedSumary = wholeTotalQuantityRecivedSumary + tailTotalQuantityRecivedSumary;
            var totalNetSumary = wholeNetSumary + tailNetSumary;

            if (adjust)
            {
                productionLot.totalQuantityLiquidationAdjust = productionLot.totalQuantityLiquidation;
            }
            //Subtotal Entero
            var percentPerformanceWholeSubtotalAdjust = decimal.Round(productionLot.totalQuantityLiquidationAdjust > 0 ? (productionLot.wholeSubtotalAdjust / productionLot.totalQuantityLiquidationAdjust) : 0, 4);

            //Subtotal Cola
            var percentPerformanceSubtotalTailAdjust = 1 - percentPerformanceWholeSubtotalAdjust;

            return Json(new
            {
                //Entero
                wholeTotalQuantityRecivedSumary,
                wholeNetSumary,
                percentWholePerformanceSumary,

                //Cola
                tailTotalQuantityRecivedSumary,
                tailNetSumary,
                percentTailPerformanceSumary,

                //Total
                poundsGarbageTotal,
                totalTotalQuantityRecivedSumary,
                totalNetSumary,

                //Subtotal Entero
                productionLot.totalAdjustmentWholePounds,
                productionLot.wholeSubtotalAdjust,
                percentPerformanceWholeSubtotalAdjust,

                //Subtotal Entero
                productionLot.totalAdjustmentTailPounds,
                productionLot.subtotalTailAdjust,
                percentPerformanceSubtotalTailAdjust,

                //Subtotal Entero
                productionLot.totalAdjustmentPounds,
                productionLot.totalQuantityLiquidationAdjust

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotReceptionItemDetails()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotDetail = productionLot.ProductionLotDetail ?? new List<ProductionLotDetail>();
            TempData.Keep("productionLotReception");

            return Json(productionLot.ProductionLotDetail.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotReceptionMaterialDetails()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotDispatchMaterial = productionLot.ProductionLotDispatchMaterial ?? new List<ProductionLotDispatchMaterial>();
            TempData.Keep("productionLotReception");

            return Json(productionLot.ProductionLotDispatchMaterial.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotReceptionLiquidationDetails()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();
            TempData.Keep("productionLotReception");

            return Json(productionLot.ProductionLotLiquidation.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotReceptionTrashDetails()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotTrash = productionLot.ProductionLotTrash ?? new List<ProductionLotTrash>();
            TempData.Keep("productionLotReception");

            return Json(productionLot.ProductionLotTrash.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotReceptionQualityAnalysisDetails()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotQualityAnalysis = productionLot.ProductionLotQualityAnalysis ?? new List<ProductionLotQualityAnalysis>();
            TempData.Keep("productionLotReception");

            return Json(productionLot.ProductionLotQualityAnalysis.Select(d => d.id_qualityAnalysis).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ProductionLotReceptionItemData(int? id, int? id_item, int? id_purcharseOrder)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var id_purchaseOrderDetailNew = productionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id == id)?.ProductionLotDetailPurchaseDetail?.FirstOrDefault()?.id_purchaseOrderDetail;

            Item item = db.Item.FirstOrDefault(p => p.id == id_item);

            PurchaseOrder purchaseOrder = db.PurchaseOrder.FirstOrDefault(p => p.id == id_purcharseOrder);
            var id_personProcessPlant = purchaseOrder?.id_personProcessPlant;
            TempData.Keep("productionLotReception");

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            return Json(new
            {
                metricUnit = item != null ? item.ItemPurchaseInformation?.MetricUnit.code ?? (metricUnitUMTP?.code ?? "Lbs")/*"Lbs"*/ : "",
                hasOrderDetail = id_purchaseOrderDetailNew != null,
                process = db.Person.FirstOrDefault(fod => fod.id == id_personProcessPlant)?.processPlant
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ExistsConversionWithLbs(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = db.Item.FirstOrDefault(p => p.id == id_item);

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod=> fod.code == "Lbs")?.id ?? 0;
            var id_metricUnitAux = item?.ItemPurchaseInformation?.MetricUnit?.id ?? id_metricUnitLbsAux;

            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
            return Json(new
            {
                metricUnitConversionValue = id_metricUnitAux == id_metricUnitLbsAux && id_metricUnitLbsAux != 0 ? 1 : metricUnitConversion?.factor ?? 0,
                metricUnitName = metricUnitUMTP?.name ?? "",
                metricUnitCode = metricUnitUMTP?.code ?? ""
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedItem(int? id, int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var id_purchaseOrderDetailNew = productionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id == id)?.ProductionLotDetailPurchaseDetail?.FirstOrDefault()?.id_purchaseOrderDetail;
            var id_remissionGuideDetailNew = productionLot.ProductionLotDetail.FirstOrDefault(fod => fod.id == id)?.ProductionLotDetailPurchaseDetail?.FirstOrDefault()?.id_remissionGuideDetail;

            foreach (var productionLotDetail in productionLot.ProductionLotDetail)
            {
                var id_purchaseOrderDetailAux = productionLotDetail.ProductionLotDetailPurchaseDetail?.FirstOrDefault()?.id_purchaseOrderDetail;
                var id_remissionGuideDetailAux = productionLotDetail.ProductionLotDetailPurchaseDetail?.FirstOrDefault()?.id_remissionGuideDetail;
                if (id_purchaseOrderDetailAux == id_purchaseOrderDetailNew && id_remissionGuideDetailAux == id_remissionGuideDetailNew &&
                     productionLotDetail.id_warehouse == id_warehouseNew && productionLotDetail.id_warehouseLocation == id_warehouseLocationNew &&
                     productionLotDetail.id_item == id_itemNew)
                {
                    var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                    var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                    var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se puede repetir el Item: " + itemAux.name +
                                ",  en la bodega: " + warehouseAux.name +
                                ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de Materia Prima"

                    };
                    //TempData["productionLotReception"] = productionLot;
                    TempData.Keep("productionLotReception");

                    return Json(result, JsonRequestBehavior.AllowGet);

                }
            }

            //TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedLiquidation(int? id_salesOrderNew, int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var productionLotLiquidationAux = productionLot.ProductionLotLiquidation.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew &&
                                                                                db.SalesOrderDetail.FirstOrDefault(fod2 => fod2.id == fod.id_salesOrderDetail)?.id_salesOrder == id_salesOrderNew);
            if (productionLotLiquidationAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de esta Liquidación"

                };

            }


            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedTrash(int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var productionLotTrashAux = productionLot.ProductionLotTrash.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew);
            if (productionLotTrashAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de Desperdicio"

                };

            }


            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitSalesOrderItemAndMetricUnit(int? id_salesOrder, int? id_item, int? id_metricUnit)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var salesOrderAux = db.SalesOrder.FirstOrDefault(fod => fod.id == id_salesOrder);

            var salesOrder = new
            {
                salesOrderAux?.id,
                name = salesOrderAux?.Document.number
            };

            var items = salesOrderAux != null ? salesOrderAux.SalesOrderDetail.Where(w => w.quantityDelivered < w.quantityApproved || w.id_item == id_item)
                                                                                                           .Select(s => new
                                                                                                           {
                                                                                                               s.Item.id,
                                                                                                               s.Item.name
                                                                                                           }) :
                                                               db.Item.Where(w => w.InventoryLine.code.Equals("PP"))
                                                                      .Select(s => new
                                                                      {
                                                                          s.id,
                                                                          s.name
                                                                      });
            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);
            var id_metricTypeAux = item?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => ((w.id_metricType == id_metricTypeAux) && w.isActive) || w.id == id_metricUnit).Select(s => new
            {
                s.id,
                s.code
            });

            var result = new
            {
                salesOrder,
                items,
                metricUnits
            };

            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult SalesOrderDetailData(int? id_salesOrder, int? id_salesOrderDetailIni, int? id_itemIni)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var salesOrder = db.SalesOrder.FirstOrDefault(fod => fod.id == id_salesOrder);


            var items = salesOrder != null ? salesOrder.SalesOrderDetail.Where(w => w.quantityDelivered < w.quantityApproved || (w.id == id_salesOrderDetailIni && w.id_item == id_itemIni))
                                                                                                           .Select(s => new
                                                                                                           {
                                                                                                               s.Item.id,
                                                                                                               s.Item.name,
                                                                                                               clase = s.Item.ItemTypeCategory?.name ?? "",
                                                                                                               size = s.Item.ItemGeneral.ItemSize?.name
                                                                                                           }) :
                                                               db.Item.Where(w => w.InventoryLine.code.Equals("PP"))
                                                                      .Select(s => new
                                                                      {
                                                                          s.id,
                                                                          s.name,
                                                                          clase = s.ItemTypeCategory != null ? s.ItemTypeCategory.name : "",
                                                                          size = s.ItemGeneral.ItemSize != null ? s.ItemGeneral.ItemSize.name : ""
                                                                      });

            var result = new
            {
                items
            };


            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateQuantityTotal(int? id_item, string quantity, int? id_metricUnit)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);

            decimal _quantity = Convert.ToDecimal(quantity);
            var presentation = item?.Presentation;
            decimal quantityTotal = QuantityTotalByPresentation(presentation, _quantity);

            var id_metricUnitPresentation = presentation?.id_metricUnit ?? id_metricUnit;

            var result = new
            {
                quantityTotal,
                id_metricUnitPresentation
            };


            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ExistsConversionWithLbsProductionLotLiquidation(int? id_item, int? id_metricUnit)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = db.Item.FirstOrDefault(p => p.id == id_item);

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;// db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;
            var id_metricUnitAux = id_metricUnit ?? item?.Presentation?.MetricUnit?.id ?? item?.ItemInventory?.MetricUnit?.id;

            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
            return Json(new
            {
                metricUnitConversionValue = id_metricUnitAux == id_metricUnitLbsAux ? 1 : metricUnitConversion?.factor ?? 0,
                metricUnitName = metricUnitUMTP?.name ?? "",
                metricUnitCode = metricUnitUMTP?.code ?? ""
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitMetricUnitsItem(int? id_item, int? id_metricUnit)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_item);

            var item = new
            {
                itemAux?.id,
                itemAux?.name
            };

            var id_metricTypeAux = itemAux?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => ((w.id_metricType == id_metricTypeAux) && w.isActive) || w.id == id_metricUnit).Select(s => new
            {
                s.id,
                s.code
            });

            var result = new
            {
                item,
                metricUnits
            };

            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult TrashItemDetailData(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);


            var id_metricTypeAux = item?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.id_metricType == id_metricTypeAux) && w.isActive).Select(s => new
            {
                s.id,
                s.code
            });

            var id_metricUnitAux = item?.ItemHeadIngredient?.id_metricUnit ?? item?.ItemInventory?.id_metricUnitInventory;

            var id_warehouseAux = item?.ItemInventory?.Warehouse.id;
            var id_warehouseLocationAux = item?.ItemInventory?.WarehouseLocation.id;
            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
                                       .Select(s => new
                                       {
                                           s.id,
                                           s.name
                                       });

            var result = new
            {
                id_metricUnit = id_metricUnitAux,
                metricUnits,
                id_warehouse = id_warehouseAux,
                id_warehouseLocation = id_warehouseLocationAux,
                warehouseLocations
            };


            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult GetItemLiquidation(int id_salesOrder, ComboBoxExtension me)//, string textField)
        //{
        //    //me.BindList(DataProviderItem.ItemsByCompany(this.ActiveCompanyId));
        //    return GridViewExtension.GetComboBoxCallbackResult(p =>
        //    {
        //        p.TextField = textField;
        //        p.BindList(WorldCitiesDataProvider.GetCities(countryName));
        //    });//Json("Ok", JsonRequestBehavior.AllowGet);

        //    //    (new MVCxComboBoxProperties
        //    //{
        //    //    DataSource = DataProviderItem.TrashItemsByCompany(this.ActiveCompanyId)
        //    //}
        //    ////    p => {
        //    ////    //p.TextField = textField;
        //    ////    p.BindList(DataProviderItem.ItemsByCompany(this.ActiveCompanyId));
        //    ////}
        //    //    ,null);
        //}

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemPackingMaterialDetailsData(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                ItemDetailData = new
                {
                    item.masterCode,
                    metricUnit = item.ItemInventory?.MetricUnit?.code,
                }
            };

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PackingMaterialDetails(int? id_itemCurrent)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotPackingMaterial = productionLot.ProductionLotPackingMaterial ?? new List<ProductionLotPackingMaterial>();


            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.InventoryLine.code.Equals("MI") && w.ItemType.code.Equals("INS") && w.ItemTypeCategory.code.Equals("MDE")) || w.id == id_itemCurrent).ToList();
            var tempItems = new List<Item>();
            foreach (var i in items)
            {
                if (!(productionLot.ProductionLotPackingMaterial.Any(a => a.id_item == i.id && a.quantityRequiredForProductionLot != 0)) || i.id == id_itemCurrent)
                {
                    tempItems.Add(i);
                }

            }
            items = tempItems;
            var result = new
            {
                items = items.Select(s => new
                {
                    s.id,
                    s.masterCode,
                    ItemInventoryMetricUnitCode = (s.ItemInventory != null) ? s.ItemInventory.MetricUnit.code : "",
                    s.name
                }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void UpdateProductionLotLiquidationPackingMaterialDetail(ProductionLot productionLot, ProductionLotLiquidation productionLotLiquidation)
        {
            for (int i = productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Count - 1; i >= 0; i--)
            {


                var detail = productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.ElementAt(i);

                detail.ProductionLotPackingMaterial.quantityRequiredForProductionLot -= detail.quantity;
                detail.ProductionLotPackingMaterial.manual = detail.ProductionLotPackingMaterial.quantityRequiredForProductionLot == 0;
                detail.ProductionLotPackingMaterial.quantity -= detail.quantity;

                //for (int j = detail.ProductionLotDetailPurchaseDetail.Count - 1; j >= 0; j--)
                //{
                //    var detailProductionLotDetailPurchaseDetail = detail.ProductionLotDetailPurchaseDetail.ElementAt(j);
                //    detail.ProductionLotDetailPurchaseDetail.Remove(detailProductionLotDetailPurchaseDetail);
                //    db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                //}

                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Remove(detail);
                try
                {
                    db.ProductionLotLiquidationPackingMaterialDetail.Attach(detail);
                    db.Entry(detail).State = EntityState.Deleted;
                }
                catch (Exception)
                {
                    //ViewData["EditError"] = e.Message;
                    continue;
                }

            }

            //foreach (var remissionGuideDetailDispatchMaterialDetail in remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail)
            //{
            //    remissionGuideDetailDispatchMaterialDetail.RemissionGuideDispatchMaterial.quantityRequiredForPurchase -= remissionGuideDetailDispatchMaterialDetail.quantity;
            //    remissionGuideDetailDispatchMaterialDetail.RemissionGuideDispatchMaterial.sourceExitQuantity -= remissionGuideDetailDispatchMaterialDetail.quantity;

            //    remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.Remove(remissionGuideDetailDispatchMaterialDetail);
            //    //db.Entry(remissionGuideDetailDispatchMaterialDetail).State = EntityState.Deleted;

            //}

            //if (!productionLotLiquidation.isActive) return;
            if (!productionLot.ProductionLotLiquidation.Any(a => a.id == productionLotLiquidation.id)) return;

            if (productionLotLiquidation.Item == null)
            {
                productionLotLiquidation.Item = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_item);
            }
            var itemIngredientMDE = productionLotLiquidation.Item.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("MI") && w.Item1.ItemType.code.Equals("INS") && w.Item1.ItemTypeCategory.code.Equals("MDE"));//"MI": Linea de Inventario Materiales e Insumos, "INS": Tipo de Producto Insumos y  "MDE": Categoría de Tipo de Producto Meteriales de Empaque
            if (itemIngredientMDE.Count() == 0) return;
            var id_metricUnitLiquidation = productionLotLiquidation.id_metricUnit;//ItemPurchaseInformation?.id_metricUnitPurchase;
            var id_metricUnitItemHeadIngredient = productionLotLiquidation.Item.ItemHeadIngredient?.id_metricUnit;
            var factorConversionLiquidationFormulation = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                             muc.id_metricOrigin == id_metricUnitLiquidation &&
                                                                                             muc.id_metricDestiny == id_metricUnitItemHeadIngredient);
            if (id_metricUnitLiquidation != null && id_metricUnitLiquidation == id_metricUnitItemHeadIngredient)
            {
                factorConversionLiquidationFormulation = new MetricUnitConversion() { factor = 1 };
            }
            if (factorConversionLiquidationFormulation == null)
            {
                var metricUnitLiquidation = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitLiquidation);
                throw new Exception("Falta el Factor de Conversión entre : " + (metricUnitLiquidation?.code ?? "(UM No Existe)") + ", del Ítem: " + productionLotLiquidation.Item.name + " y " + (productionLotLiquidation.Item.ItemHeadIngredient?.MetricUnit?.code ?? "(UM No Existe)") + " configurado en la cabecera de la formulación del este Ítem. Necesario para cargar los Materiales de Empaque Configúrelo, e intente de nuevo");
            }

            foreach (var iimdd in itemIngredientMDE)
            {
                var quantityMetricUnitItemHeadIngredient = productionLotLiquidation.quantity * factorConversionLiquidationFormulation.factor;
                var amountItemHeadIngredient = (productionLotLiquidation.Item.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + productionLotLiquidation.Item.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                }
                var quantityItemIngredientMDE = (quantityMetricUnitItemHeadIngredient * (iimdd.amount ?? 0)) / amountItemHeadIngredient;
                if (quantityItemIngredientMDE == 0) continue;

                //if(iimdd.Item1.MetricType.DataType.code.Equals("ENTE01"))//"ENTE01" Codigo de Entero de Tipo de Datos en la unidad de medida
                //{
                var truncateQuantityItemIngredientMDE = decimal.Truncate(quantityItemIngredientMDE);
                if ((quantityItemIngredientMDE - truncateQuantityItemIngredientMDE) > 0)
                {
                    quantityItemIngredientMDE = truncateQuantityItemIngredientMDE + 1;
                };
                //}
                var id_metricUnitFormulation = iimdd.id_metricUnit;
                var id_metricUnitInventory = iimdd.Item1.ItemInventory?.id_metricUnitInventory;
                var factorConversionFormulationInventory = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                                  muc.id_metricOrigin == id_metricUnitFormulation &&
                                                                                                  muc.id_metricDestiny == id_metricUnitInventory);
                if (id_metricUnitFormulation != null && id_metricUnitFormulation == id_metricUnitInventory)
                {
                    factorConversionFormulationInventory = new MetricUnitConversion() { factor = 1 };
                }
                if (factorConversionFormulationInventory == null)
                {
                    throw new Exception("Falta el Factor de Conversión entre : " + iimdd.MetricUnit?.code ?? "(UM No Existe)" + ", del Ítem: " + iimdd.Item1.name + " y " + iimdd.Item1.ItemInventory?.MetricUnit.code ?? "(UM No Existe)" + " configurado en el detalle de la formulación del Ítem: " + productionLotLiquidation.Item.name + ". Necesario para cargar los Materiales de Empaque Configúrelo, e intente de nuevo");
                }

                var quantityUMInventory = quantityItemIngredientMDE * factorConversionFormulationInventory.factor;

                var truncateQuantityUMInventory = decimal.Truncate(quantityUMInventory);
                if ((quantityUMInventory - truncateQuantityUMInventory) > 0)
                {
                    quantityUMInventory = truncateQuantityUMInventory + 1;
                };

                ProductionLotPackingMaterial productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.Where(w => /*!w.manual &&*/ w.isActive).FirstOrDefault(fod => fod.id_item == iimdd.id_ingredientItem);
                if (productionLotPackingMaterial != null)
                {
                    productionLotPackingMaterial.quantityRequiredForProductionLot += quantityUMInventory;
                    productionLotPackingMaterial.quantity += quantityUMInventory;
                    productionLotPackingMaterial.manual = false;
                    productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                    productionLotPackingMaterial.dateUpdate = DateTime.Now;
                }
                else
                {
                    productionLotPackingMaterial = new ProductionLotPackingMaterial
                    {
                        id = productionLot.ProductionLotPackingMaterial.Count() > 0 ? productionLot.ProductionLotPackingMaterial.Max(pld => pld.id) + 1 : 1,
                        id_item = iimdd.id_ingredientItem,
                        Item = db.Item.FirstOrDefault(i => i.id == iimdd.id_ingredientItem),
                        quantityRequiredForProductionLot = quantityUMInventory,
                        quantity = quantityUMInventory,
                        manual = false,
                        isActive = true,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                        ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>()
                    };
                    productionLot.ProductionLotPackingMaterial.Add(productionLotPackingMaterial);
                }

                var productionLotLiquidationPackingMaterialDetail = new ProductionLotLiquidationPackingMaterialDetail
                {
                    ProductionLotLiquidation = productionLotLiquidation,
                    id_productionLotLiquidation = productionLotLiquidation.id,
                    ProductionLotPackingMaterial = productionLotPackingMaterial,
                    id_productionLotPackingMaterial = productionLotPackingMaterial.id,
                    quantity = quantityUMInventory
                };
                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
                productionLotPackingMaterial.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
            }

        }

        [HttpPost]
        public ActionResult GetProductionUnitProviderPool(int? id_provider)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);
            TempData.Keep("productionLotReception");
            //ViewData["id_person"] = id_person;

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                //settings.Name = "id_person";
                p.ClientInstanceName = "id_productionUnitProviderPool";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "ProductionLotReception", Action = "GetProductionUnitProviderPool" };
                p.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.BeginCallback = "ProductionUnitProviderPool_BeginCallback";

                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnproductionUnitProviderPoolValidation";

                //p.TextField = textField;
                p.BindList(DataProviderProductionUnitProviderPool.ProductionUnitProviderPoolByProvider(null, id_provider));//.Bind(id_person);

            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateTotalsDetailClose(int id, string totalUM)
        {

            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotPayment = productionLot.ProductionLotPayment ?? new List<ProductionLotPayment>();

            decimal _totalUM = Convert.ToDecimal(totalUM);
            var totalPounds = 0.0M;
            var percentPerformancePounds = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0; // db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            var productionLotPayment = productionLot.ProductionLotPayment.FirstOrDefault(fod => fod.id == id);
            if (productionLotPayment != null)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotPayment.id_item);
                var id_metricUnitAux = productionLotPayment.id_metricUnit;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                totalPounds = decimal.Round(_totalUM * factor, 2);
                var valueAux = productionLot.totalQuantityLiquidationAdjust - productionLotPayment.totalPounds + totalPounds;
                percentPerformancePounds = decimal.Round((totalPounds / valueAux), 4);
            }


            var result = new
            {
                totalPounds,
                percentPerformancePounds
            };

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionLotPaymentTotals(/*int? id_priceList, bool updatePrice*/)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLotReception"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            decimal totalPromedioEnterio = 0m, totalPromedioCola = 0m, totalPromedioFinal = 0m;

            if (productionLot.ProductionLotPayment.Count() > 0)
            {
                foreach (var productionLotPayment in productionLot.ProductionLotPayment)
                {
                    var valor1 = productionLotPayment.totalPriceDis != null ? productionLotPayment.totalPriceDis : 0;
                    if (productionLotPayment.Item.ItemType.ProcessType.code == "ENT")
                    {
                        totalPromedioEnterio = (decimal)(totalPromedioEnterio + valor1);
                    }
                    else
                    {
                        totalPromedioCola = (decimal)(totalPromedioCola + valor1);
                    }

                    totalPromedioFinal = (decimal)(totalPromedioFinal + valor1);
                }
            }


            //Resumen General
            var totalToPayIvaRate0PaymentSummary = 0;
            var totalToPayTotalLiquidationPaymentSummary = productionLot.totalToPay + totalToPayIvaRate0PaymentSummary;
            var advanceProviderAux = db.AdvanceProvider.FirstOrDefault(fod => fod.id_provider == productionLot.id_provider &&
                                                                             fod.id_Lot == productionLot.id);
            var totalToPayAdvancePaymentSummary = decimal.Round((advanceProviderAux?.valueAdvanceTotal ?? 0), 2);
            totalToPayAdvancePaymentSummary *= (-1);

            var totalToPayToReceivePaymentSummary = totalToPayTotalLiquidationPaymentSummary + totalToPayAdvancePaymentSummary;

            //Entero
            var wholeTotalToPayPaymentSummary = productionLot.wholeTotalToPay;
            var percentPerformanceWholePaymentSummary = decimal.Round((productionLot.totalToPay > 0 ? (productionLot.wholeTotalToPay / productionLot.totalToPay) : 0), 4);

            //Cola
            var tailTotalToPayPaymentSummary = productionLot.tailTotalToPay;
            var percentPerformanceTailPaymentSummary = decimal.Round((productionLot.totalToPay > 0 ? (productionLot.tailTotalToPay / productionLot.totalToPay) : 0), 4);

            //Total
            var totalToPayPaymentSummary = productionLot.totalToPay;
            var percentPerformanceTotalPaymentSummary = 1;


            var tailTotalQuantityRecivedPaymentSummaryAux = GetTotalQuantityDrained(productionLot);
            var tailTotalQuantityRecivedPaymentSummary = productionLot.wholeSubtotalAdjust > 0 ? productionLot.wholeLeftover : tailTotalQuantityRecivedPaymentSummaryAux;
            var wholeTotalQuantityRecivedPaymentSummaryAux = productionLot.wholeSubtotalAdjust + productionLot.wholeLeftover + productionLot.wholeGarbagePounds;
            var wholeTotalQuantityRecivedPaymentSummary = productionLot.wholeSubtotalAdjust > 0 ? wholeTotalQuantityRecivedPaymentSummaryAux : 0.0M;
            var totalTotalQuantityRecivedPaymentSummary = wholeTotalQuantityRecivedPaymentSummary + tailTotalQuantityRecivedPaymentSummary;

            //Resumen Entero
            var wholeAveragePerformancePricePaymentSummary = decimal.Round((productionLot.wholeSubtotalAdjust > 0 ? (productionLot.wholeTotalToPay / productionLot.wholeSubtotalAdjust) : 0), 2);
            var FullAverageDistributedThroughputPricePaymentSummary = decimal.Round((productionLot.wholeSubtotalAdjust > 0 ? (totalPromedioEnterio / productionLot.wholeSubtotalAdjust) : 0), 2);
            var percentWholePerformancePaymentSummary = decimal.Round((productionLot.totalQuantityLiquidationAdjust > 0 ? (productionLot.wholeSubtotalAdjust / productionLot.totalQuantityLiquidationAdjust) : 0), 4);
            var wholeAverageTotalQuantityRecivedPricePaymentSummary = decimal.Round((wholeTotalQuantityRecivedPaymentSummary > 0 ? (productionLot.wholeTotalToPay / wholeTotalQuantityRecivedPaymentSummary) : 0), 2);
            var wholeAverageTotalQuantityReceivedPrizePaymentDistibutedSummary = decimal.Round((wholeTotalQuantityRecivedPaymentSummary > 0 ? (totalPromedioEnterio / wholeTotalQuantityRecivedPaymentSummary) : 0), 2);
            var percentWholeTotalQuantityRecivedPaymentSummary = decimal.Round((totalTotalQuantityRecivedPaymentSummary > 0 ? (wholeTotalQuantityRecivedPaymentSummary / totalTotalQuantityRecivedPaymentSummary) : 0), 4);
            var wholeLbsProcessedReceivedPaymentSummary = decimal.Round((wholeTotalQuantityRecivedPaymentSummary > 0 ? (productionLot.wholeSubtotalAdjust / wholeTotalQuantityRecivedPaymentSummary) : 0), 4);

            //Resumen Cola
            var tailAveragePerformancePricePaymentSummary = decimal.Round((productionLot.subtotalTailAdjust > 0 ? (productionLot.tailTotalToPay / productionLot.subtotalTailAdjust) : 0), 2);
            var tailAverageYieldDistributedPricePaymentSummary = decimal.Round((productionLot.subtotalTailAdjust > 0 ? (totalPromedioCola / productionLot.subtotalTailAdjust) : 0), 2);
            var percentTailPerformancePaymentSummary = decimal.Round((1 - percentWholePerformancePaymentSummary), 4);
            var tailAverageTotalQuantityRecivedPricePaymentSummary = decimal.Round((tailTotalQuantityRecivedPaymentSummary > 0 ? (productionLot.tailTotalToPay / tailTotalQuantityRecivedPaymentSummary) : 0), 2);
            var tailAverageTotalQuantityRecivedPriceDistributedPaymentSummary = decimal.Round((tailTotalQuantityRecivedPaymentSummary > 0 ? (totalPromedioCola / tailTotalQuantityRecivedPaymentSummary) : 0), 2);
            var percentTailTotalQuantityRecivedPaymentSummary = decimal.Round((1 - percentWholeTotalQuantityRecivedPaymentSummary), 4);
            var tailLbsProcessedReceivedPaymentSummary = decimal.Round((tailTotalQuantityRecivedPaymentSummary > 0 ? (productionLot.subtotalTailAdjust / tailTotalQuantityRecivedPaymentSummary) : 0), 4);

            //Resumen Total
            var totalValoresPromedio = totalPromedioEnterio + totalPromedioCola;
            var totalAveragePerformancePricePaymentSummary = decimal.Round((productionLot.totalQuantityLiquidationAdjust > 0 ? (totalToPayTotalLiquidationPaymentSummary / productionLot.totalQuantityLiquidationAdjust) : 0), 2);
            var totalAveragePerformanceDistributedPricePaymentSummary = decimal.Round((productionLot.totalQuantityLiquidationAdjust > 0 ? (totalValoresPromedio / productionLot.totalQuantityLiquidationAdjust) : 0), 2);
            var totalAverageTotalQuantityRecivedPricePaymentSummary = decimal.Round((totalTotalQuantityRecivedPaymentSummary > 0 ? (productionLot.tailTotalToPay / totalTotalQuantityRecivedPaymentSummary) : 0), 2);
            var totalAverageTotalQuantityRecivedPriceDistributedPaymentSummary = decimal.Round((totalTotalQuantityRecivedPaymentSummary > 0 ? (totalValoresPromedio / totalTotalQuantityRecivedPaymentSummary) : 0), 2);
            //var totalAverageTotalQuantityRecivedPricePaymentSummary = decimal.Round(((wholeAverageTotalQuantityRecivedPricePaymentSummary + tailAverageTotalQuantityRecivedPricePaymentSummary) / 2), 2);
            var totalLbsProcessedReceivedPaymentSummary = decimal.Round((totalTotalQuantityRecivedPaymentSummary > 0 ? (productionLot.totalQuantityLiquidationAdjust / totalTotalQuantityRecivedPaymentSummary) : 0), 4);


            //if (Message != "")
            //{
            //    Message += ". Debe definirle precio en la lista de precio escogida, configúrelo e intente de nuevo";
            //    //ViewData["EditMessage"] = ErrorMessage(Message);
            //}
            //TempData["productionLotReception"] = TempData["productionLotReception"] ?? productionLot;
            //TempData.Keep("productionLotReception");

            TempData["productionLotReception"] = productionLot;
            TempData.Keep("productionLotReception");

            string valueDistribution = DataProviderSetting.ValueSetting("DIST");

            //Message = Message != "" ? Message : "Ok";
            var result = new
            {
                //Message = Message,
                valueDistribution,
                //Resumen Entero
                wholeAveragePerformancePricePaymentSummary,
                FullAverageDistributedThroughputPricePaymentSummary,
                percentWholePerformancePaymentSummary,
                wholeTotalQuantityRecivedPaymentSummary,
                wholeAverageTotalQuantityRecivedPricePaymentSummary,
                wholeAverageTotalQuantityReceivedPrizePaymentDistibutedSummary,
                percentWholeTotalQuantityRecivedPaymentSummary,
                wholeLbsProcessedReceivedPaymentSummary,

                //Resumen Cola
                tailTotalQuantityRecivedPaymentSummary,
                tailAveragePerformancePricePaymentSummary,
                tailAverageYieldDistributedPricePaymentSummary,
                percentTailPerformancePaymentSummary,
                tailAverageTotalQuantityRecivedPricePaymentSummary,
                tailAverageTotalQuantityRecivedPriceDistributedPaymentSummary,
                percentTailTotalQuantityRecivedPaymentSummary,
                tailLbsProcessedReceivedPaymentSummary,

                //Resumen Total
                totalTotalQuantityRecivedPaymentSummary,
                totalAveragePerformancePricePaymentSummary,
                totalAveragePerformanceDistributedPricePaymentSummary,
                totalAverageTotalQuantityRecivedPricePaymentSummary,
                totalAverageTotalQuantityRecivedPriceDistributedPaymentSummary,
                totalLbsProcessedReceivedPaymentSummary,

                //Resumen General
                totalToPayIvaRate0PaymentSummary,
                totalToPayTotalLiquidationPaymentSummary,
                totalToPayAdvancePaymentSummary,

                totalToPayToReceivePaymentSummary,

                //Entero
                wholeTotalToPayPaymentSummary,
                percentPerformanceWholePaymentSummary,

                //Cola
                tailTotalToPayPaymentSummary,
                percentPerformanceTailPaymentSummary,

                //Total
                totalToPayPaymentSummary,
                percentPerformanceTotalPaymentSummary

            };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public List<ProductionLotPayment> GetValoresTotalesDeatil(ProductionLotPayment productionLotPayment)
        {
            var listaAuxProductionLotPayment = new List<ProductionLotPayment>();
            //Cálculos

            var estado = productionLotPayment.ProductionLotPaymentDistributed.Select(e => e.isActive);

            decimal totalKgDetail = 0m;
            decimal totalLbDetail = 0m;
            decimal totalPagarDistribuido = 0m;
            decimal precioDistribuido = 0m;

            foreach (var detalle in productionLotPayment.ProductionLotPaymentDistributed.Where(s => s.isActive))
            {

                totalKgDetail = (decimal)(totalKgDetail + detalle.kilogram);
                totalLbDetail = totalLbDetail + detalle.pound;
                totalPagarDistribuido = totalPagarDistribuido + detalle.totalPayLP;
                
            }

            if (productionLotPayment.id_metricUnitProcess == 4)
            {
                if (totalLbDetail > 0)
                {
                    precioDistribuido = totalPagarDistribuido / totalLbDetail;
                }
            }
            else
            {
                if (totalKgDetail > 0)
                {
                    precioDistribuido = (decimal)(totalPagarDistribuido / totalKgDetail);
                }
            }

            listaAuxProductionLotPayment.Add(new ProductionLotPayment
            {
                id_item = productionLotPayment.id_item,
                totalProcessMetricUnit = productionLotPayment.totalProcessMetricUnit,
                totalProcessMetricUnitEq = productionLotPayment.totalProcessMetricUnitEq,
                id_metricUnitProcess = productionLotPayment.id_metricUnitProcess,
                price = productionLotPayment.price,
                priceEdition = productionLotPayment.priceEdition,
                totalToPay = productionLotPayment.totalToPay,
                totalToPayEq = productionLotPayment.totalToPayEq,
                totalKgs = totalKgDetail,
                totalLbs = totalLbDetail,
                totalPriceDis = totalPagarDistribuido,
                priceDis = precioDistribuido,
                differencia = totalPagarDistribuido - productionLotPayment.totalToPay
            });

            return listaAuxProductionLotPayment;
        }

        public List<SummaryProductionLotPaymentDetail> GetListSummaryProductionLotPaymentDetail(ProductionLot productionLot)
        {
            var summaryProductionLotPaymentDetailAux = new List<SummaryProductionLotPaymentDetail>();
            decimal totalPromedioEnterio = 0m, totalPromedioCola = 0m, totalPromedioFinal = 0m;

            if (productionLot.ProductionLotPayment.Count() > 0)
            {
                foreach (var productionLotPayment in productionLot.ProductionLotPayment)
                {
                    var valor1 = productionLotPayment.totalPriceDis != null ? productionLotPayment.totalPriceDis : 0;
                    if (productionLotPayment.Item.ItemType.ProcessType.code == "ENT")
                    {
                        totalPromedioEnterio = (decimal)(totalPromedioEnterio + valor1);
                    }
                    else
                    {
                        totalPromedioCola = (decimal)(totalPromedioCola + valor1);
                    }

                    totalPromedioFinal = (decimal)(totalPromedioFinal + valor1);
                }
            }

            //Resumen General
            var totalToPayIvaRate0PaymentSummary = 0;
            var totalToPayTotalLiquidationPaymentSummary = productionLot.totalToPay + totalToPayIvaRate0PaymentSummary;
            var advanceProviderAux = db.AdvanceProvider.FirstOrDefault(fod => fod.id_provider == productionLot.id_provider &&
                                                                             fod.id_Lot == productionLot.id);
            var totalToPayAdvancePaymentSummary = advanceProviderAux?.valueAdvanceTotalRounded ?? 0;
            totalToPayAdvancePaymentSummary *= (-1);

            var totalToPayToReceivePaymentSummary = totalToPayTotalLiquidationPaymentSummary + totalToPayAdvancePaymentSummary;


            var percentPerformanceWholePaymentSummary = decimal.Round((productionLot.totalToPay > 0 ? (productionLot.wholeTotalToPay / productionLot.totalToPay) : 0), 4);

            decimal porcentajeEnteroTotal = decimal.Round((totalPromedioFinal > 0 ? (totalPromedioEnterio / totalPromedioFinal) : 0), 4);
            decimal porcentajeColaTotal = decimal.Round((totalPromedioFinal > 0 ? (totalPromedioCola / totalPromedioFinal) : 0), 4);
            decimal porcentajeTotal = porcentajeEnteroTotal + porcentajeColaTotal;

            summaryProductionLotPaymentDetailAux.Add(new SummaryProductionLotPaymentDetail
            {
                id = 1,
                titleTotal = "SubTotal Entero",
                subTotalProceso = productionLot.wholeSubtotalAdjustProcess,
                total = productionLot.wholeTotalToPay,
                percentPerformanceTotal = percentPerformanceWholePaymentSummary,
                metricUnitProcess = "Kg",
                totalProm = totalPromedioEnterio,
                percentajeTotal = porcentajeEnteroTotal,
                diferencia = totalPromedioEnterio - productionLot.wholeTotalToPay
            });

            var percentPerformanceTailPaymentSummary = decimal.Round((productionLot.totalToPay > 0 ? (productionLot.tailTotalToPay / productionLot.totalToPay) : 0), 4);

            summaryProductionLotPaymentDetailAux.Add(new SummaryProductionLotPaymentDetail
            {
                id = 2,
                titleTotal = "SubTotal Cola",
                subTotalProceso = productionLot.subtotalTailAdjust,
                total = productionLot.tailTotalToPay,
                percentPerformanceTotal = percentPerformanceTailPaymentSummary,
                metricUnitProcess = "Lb",
                totalProm = totalPromedioCola,
                percentajeTotal = porcentajeColaTotal,
                diferencia = totalPromedioCola - productionLot.tailTotalToPay
            });

            var percentPerformanceTotalPaymentSummary = 1;
            var totalPromD = totalPromedioEnterio + totalPromedioCola;

            summaryProductionLotPaymentDetailAux.Add(new SummaryProductionLotPaymentDetail
            {
                id = 3,
                titleTotal = "Total",
                subTotalProceso = null,
                total = productionLot.totalToPay,
                percentPerformanceTotal = percentPerformanceTotalPaymentSummary,
                totalProm = totalPromD,
                percentajeTotal = porcentajeTotal,
                diferencia = totalPromD - productionLot.totalToPay
            });

            summaryProductionLotPaymentDetailAux.Add(new SummaryProductionLotPaymentDetail
            {
                id = 4,
                titleTotal = "Iva Tarifa 0%",
                subTotalProceso = null,
                total = totalToPayIvaRate0PaymentSummary,
                percentPerformanceTotal = null,
                totalProm = 0,
                percentajeTotal = null,
                diferencia = null
            });

            summaryProductionLotPaymentDetailAux.Add(new SummaryProductionLotPaymentDetail
            {
                id = 5,
                titleTotal = "Total Liquidación",
                subTotalProceso = null,
                total = totalToPayTotalLiquidationPaymentSummary,
                percentPerformanceTotal = null,
                totalProm = totalPromedioEnterio + totalPromedioCola + 0,
                percentajeTotal = null,
                diferencia = null
            });

            summaryProductionLotPaymentDetailAux.Add(new SummaryProductionLotPaymentDetail
            {
                id = 6,
                titleTotal = "Anticipo",
                subTotalProceso = null,
                total = totalToPayAdvancePaymentSummary,
                percentPerformanceTotal = null,
                totalProm = 0,
                percentajeTotal = null,
                diferencia = null
            });

            summaryProductionLotPaymentDetailAux.Add(new SummaryProductionLotPaymentDetail
            {
                id = 7,
                titleTotal = "A recibir",
                subTotalProceso = null,
                total = totalToPayToReceivePaymentSummary,
                percentPerformanceTotal = null,
                totalProm = totalPromedioEnterio + totalPromedioCola - 0,
                percentajeTotal = null,
                diferencia = null
            });


            return summaryProductionLotPaymentDetailAux;
        }

        private decimal GetTotalQuantityDrained(ProductionLot productionLot)
        {
            var resultAux = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotDetail in productionLot.ProductionLotDetail)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotDetail.id_item);
                var id_metricUnitAux = ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                resultAux += decimal.Round((productionLotDetail.quantitydrained ?? 0) * factor, 2);
            }
            return resultAux;
            //productionLot.totalQuantityOrdered = decimal.Round(productionLot.totalQuantityOrdered, 2);
            //productionLot.totalQuantityRemitted = decimal.Round(productionLot.totalQuantityRemitted, 2);
            //productionLot.totalQuantityRecived = decimal.Round(productionLot.totalQuantityRecived, 2);
        }

        public JsonResult ApproveProductionLotsPendingsMassive(int[] ids)
        {
            GenericResultJson oJsonResult = new GenericResultJson();
            ProductionLot plTmp = new ProductionLot();
            ProductionLotState plsTmp = new ProductionLotState();
            //int iCounDetails = 0;
            //int iCountDetilsApproved = 0;
            //string codeQc = "";
            string strLotsApproved = "";
            //string strLotsNotApproved = "";
            string strMessageApproved = "";
            string strMessageNotApproved = "";
            string strMessageFinal = "";
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    if (ids != null && ids.Length > 0)
                    {
                        foreach (int i in ids)
                        {
                            plTmp = db.ProductionLot.FirstOrDefault(fod => fod.id == i);
                            string settCxC = db.Setting.FirstOrDefault(fod => fod.code == "HLCXC")?.value ?? "0";

                            string loteManualParm = db.Setting.FirstOrDefault(fod => fod.code == "PLOM")?.value ?? "NO";

                            if (plTmp != null && plTmp.ProductionLotState != null && plTmp.ProductionLotState.code == "01" && loteManualParm == "NO")//PENDIENTE DE RECEPCION
                            {
                                var quantityDrainedAux = GetTotalQuantityDrained(plTmp);
                                if (quantityDrainedAux <= 0)
                                {
                                    strMessageFinal = "No se puede aprobar el lote por no tener prueba de escurrido realizada";
                                }

                                int? aId_processType = 0;
                                string aCode_processType = "";
                                foreach (var detail in plTmp.ProductionLotDetail)
                                {
                                    var aQualityControl = detail.ProductionLotDetailQualityControl
                                                                            .FirstOrDefault(fod => fod.IsDelete == false
                                                                                                    && fod.QualityControl.Document.DocumentState.code.Equals("03"))?.QualityControl;//"03": Aprobado

                                    if (aQualityControl == null)
                                    {
                                        strMessageFinal = "No se puede aprobar el lote por tener detalles de Materia Prima sin Calidad APROBADA(Conforme)";
                                    }

                                    if (!(aQualityControl?.isConforms ?? false))//"03": Aprobado
                                    {
                                        strMessageFinal = "No se puede aprobar el lote por tener detalles de Materia Prima con Calidad no APROBADA(Conforme)";
                                    }

                                    if (aId_processType == 0)
                                    {
                                        aId_processType = aQualityControl?.id_processType;
                                        aCode_processType = aQualityControl?.ProcessType?.code;
                                    }
                                    if (aQualityControl?.id_processType != aId_processType)
                                    {
                                        strMessageFinal = "No se puede aprobar el lote por tener detalles de Materia Prima con Calidad APROBADA(Conforme) y Tipo de Proceso diferente";
                                    }
                                    if (detail.quantitydrained <= 0)
                                    {
                                        strMessageFinal = "No se puede aprobar el lote por no tener prueba de escurrido realizada";
                                    }
                                    foreach (var productionLotDetailPurchaseDetail in detail.ProductionLotDetailPurchaseDetail)
                                    {
                                        ServicePurchaseRemission.UpdateQuantityRecived(db, productionLotDetailPurchaseDetail.id_purchaseOrderDetail,
                                                                                        productionLotDetailPurchaseDetail.id_remissionGuideDetail,
                                                                                        productionLotDetailPurchaseDetail.quanty);
                                    }
                                }

                                var processTypeENT = db.ProcessType.FirstOrDefault(fod => fod.code == "ENT");
                                var processTypeCOL = db.ProcessType.FirstOrDefault(fod => fod.code == "COL");

                                if (aCode_processType != "ENT" && plTmp.LiquidationCartOnCart != null && plTmp.LiquidationCartOnCart.Count() > 0)
                                {
                                    var aLiquidationCartOnCart = plTmp.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code == "03" && //03:APROBADA
                                                                                                                        fod.idProccesType == processTypeENT.id);
                                    if (aLiquidationCartOnCart != null)
                                    {
                                        strMessageFinal = "No se puede aprobar el Lote, debe revisar los Tipo de Proceso de las liquidaciones Carro X Carro de este Lote";
                                    }

                                }
                                //AQUI SE COMENTA 11102018
                                //ServiceInventoryMove.UpdateInventaryMoveEntryMateriaPrimaReception(ActiveUser, ActiveCompany, ActiveEmissionPoint, modelItem, db, false);
                                //Enviar notificacion a bodegueros para que conozcan el Ingreso de Materiales de Despacho

                                plTmp.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "02"); //RECEPCIONADO

                                plTmp.ProcessType = (processTypeENT.id == aId_processType) ? processTypeENT : processTypeCOL;
                                plTmp.id_processtype = (processTypeENT.id == aId_processType) ? processTypeENT.id : processTypeCOL.id;

                                //Actualizando Estado del Documento Lote Base
                                var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                                plTmp.Lot.Document.id_documentState = documentState.id;
                                plTmp.Lot.Document.DocumentState = documentState;
                            }
                            if (plTmp != null && plTmp.ProductionLotState != null && plTmp.ProductionLotState.code == "01" && loteManualParm == "SI")//PENDIENTE DE RECEPCION
                            {
                                plTmp.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "08");
                                //Actualizando Estado del Documento Lote Base
                                var documentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                                plTmp.Lot.Document.id_documentState = documentState.id;
                                plTmp.Lot.Document.DocumentState = documentState;
                            }
                            if (plTmp.ProductionLotState.code == "03")//PENDIENTE DE PROCESAMIENTO
                            {
                                int seqLiquidation = plTmp.sequentialLiquidation ?? 0;
                                if (seqLiquidation == 0)
                                {
                                    var documentTypeLiq = db.DocumentType.FirstOrDefault(fod => fod.code.Equals("94"));
                                    plTmp.sequentialLiquidation = GetDocumentSequential(documentTypeLiq.id);
                                    if (documentTypeLiq != null)
                                    {
                                        documentTypeLiq.currentNumber = documentTypeLiq.currentNumber + 1;
                                        db.DocumentType.Attach(documentTypeLiq);
                                        db.Entry(documentTypeLiq).State = EntityState.Modified;
                                    }
                                }

                                if (settCxC == "0" && plTmp.ProductionLotLiquidationTotal.Count() == 0)
                                {
                                    //if (modelItem.totalQuantityRecived < (modelItem.totalQuantityLiquidation + modelItem.totalQuantityTrash))
                                    //{
                                    //    throw new Exception("No se puede aprobar el lote por tener la cantidad liquidada(procesada) mayor que la recibida");
                                    //}
                                }
                                else
                                {
                                    if (plTmp.LiquidationCartOnCart.FirstOrDefault(fod => fod.Document.DocumentState.code == "01") != null)//PENDIENTE
                                    {
                                        strMessageFinal = "No se puede aprobar el lote por tener Liquidación de Carror x Carro pendiente. Apruébelo e intente de nuevo";
                                    }
                                }
                                //General Detalle de Cierre y Pago                                                                                          
                                #region CREATE PRODUCTION LOT CLOSES AND PAYMENT DETAILS

                                for (int j = plTmp.ProductionLotPayment.Count - 1; j >= 0; j--)
                                {
                                    var detail = plTmp.ProductionLotPayment.ElementAt(j);
                                    plTmp.ProductionLotPayment.Remove(detail);
                                    db.Entry(detail).State = EntityState.Deleted;
                                }

                                if (plTmp.ProductionLotPayment != null)
                                {
                                    for (int j = plTmp.ProductionLotPayment.Count - 1; j >= 0; j--)
                                    {
                                        var detail = plTmp.ProductionLotPayment.ElementAt(j);
                                        plTmp.ProductionLotPayment.Remove(detail);
                                        db.Entry(detail).State = EntityState.Deleted;
                                    }

                                    List<PLliqTotalDTO> groupByItem = new List<PLliqTotalDTO>();
                                    if (settCxC == "0" && plTmp.ProductionLotLiquidationTotal.Count() == 0)
                                    {
                                        groupByItem = plTmp.ProductionLotLiquidation.GroupBy(gb => new
                                        {
                                            gb.id_item,
                                            gb.id_metricUnitPresentation
                                        }).Select(s => new PLliqTotalDTO
                                        {
                                            id_item = s.Key.id_item,
                                            id_metricUnit = s.Key.id_metricUnitPresentation,
                                            quantity = s.Sum(ss => ss.quantityTotal.Value),
                                            quantityEntered = s.Sum(ss => ss.quantity),
                                            quantityPounds = s.Sum(ss => ss.quantityPoundsLiquidation)
                                        }).ToList();
                                    }
                                    else
                                    {
                                        groupByItem = plTmp.ProductionLotLiquidationTotal.GroupBy(gb => new
                                        {
                                            gb.id_ItemLiquidation,
                                            gb.id_metricUnitPresentation
                                        }).Select(s => new PLliqTotalDTO
                                        {
                                            id_item = s.Key.id_ItemLiquidation,
                                            id_metricUnit = s.Key.id_metricUnitPresentation,
                                            quantity = s.Sum(ss => ss.quantityTotal.Value),
                                            quantityEntered = s.Sum(ss => ss.quatityBoxesIL),
                                            quantityPounds = s.Sum(ss => ss.quantityPoundsIL),
                                        }).ToList();
                                    }

                                    foreach (var detail in groupByItem)
                                    {
                                        var newDetail = new ProductionLotPayment
                                        {
                                            id_productionLot = plTmp.id,
                                            id_item = detail.id_item,
                                            id_metricUnit = detail.id_metricUnit,
                                            quantity = detail.quantity,
                                            quantityEntered = detail.quantityEntered,
                                            quantityPoundsClose = detail.quantityPounds,
                                            adjustMore = 0,
                                            adjustLess = 0,
                                            totalMU = detail.quantity,
                                            price = 0,
                                            priceEdition = 0,
                                            totalToPay = 0,
                                            totalToPayEq = 0,
                                            totalPounds = 0,
                                            fitPounds = 0,
                                            totalProcessMetricUnit = 0,
                                            totalProcessMetricUnitEq = 0,
                                            id_metricUnitProcess = detail.id_metricUnit
                                        };

                                        plTmp.ProductionLotPayment.Add(newDetail);
                                    }
                                }

                                #endregion
                                UpdateProductionLotPerformance(plTmp);

                                if (plTmp.ProductionLotDetail.FirstOrDefault(w => w.quantitydrained == 0 || w.quantitydrained == null) != null)
                                {
                                    strMessageFinal = "Hay detalles sin libras de escurrido";
                                }

                                plTmp.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "04"); //EN PROCESAMIENTO
                            }
                            if (plTmp.ProductionLotState.code == "05")//PENDIENTE DE CIERRE
                            {
                                plTmp.closeDate = DateTime.Now;
                                plTmp.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "06"); //CERRADO
                            }
                            if (plTmp.ProductionLotState.code == "07")//PENDIENTE DE PAGO
                            {
                                plTmp.id_userAuthorizedBy = ActiveUser.id;
                                plTmp.liquidationPaymentDate = DateTime.Now;
                                string state = db.PriceList.FirstOrDefault(fod => fod.id == plTmp.id_priceList)?.Document?.DocumentState?.code ?? "";

                                if (!state.Equals("15"))
                                {
                                    strMessageFinal = "La Lista de Precios debe estar aprobado por Gerencia General";
                                }
                                //if (settCxC != "0")
                                //{
                                string settPCONGELA = db.Setting.FirstOrDefault(fod => fod.code == "PCONGELA")?.value ?? "";
                                if (settPCONGELA == "")
                                {
                                    strMessageFinal = "No se puede Aprobar, por no estar definido el Parámetro con código: PCONGELA con valor SI o NO";
                                }
                                else
                                {
                                    if (settPCONGELA != "SI" && settPCONGELA != "NO")
                                    {
                                        strMessageFinal = "No se puede Aprobar, porque el valor: " + settPCONGELA + " definido en el Parámetro con código: PCONGELA, es erróneo los valores deben ser SI o NO";
                                    }
                                    else
                                    {
                                        if (settPCONGELA == "SI")
                                        {
                                            ServiceInventoryMove.UpdateInventaryMoveAutomaticRawMaterialEntry(true, ActiveUser, ActiveCompany, ActiveEmissionPoint, plTmp, db, false, null, plTmp.pricePerLbs);
                                        }
                                    }
                                }


                                plTmp.ProductionLotState = db.ProductionLotState.FirstOrDefault(s => s.code == "08"); //PAGADO
                            }


                        }


                        if (strMessageFinal == "")
                        {
                            db.Entry(plTmp).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();
                            strMessageApproved = "Los Lotes pendientes " + strLotsApproved + " fueron Aprobados.";
                        }
                        strMessageFinal = strMessageApproved + strMessageNotApproved + strMessageFinal;

                        //if (strLotsApproved != "" && strLotsApproved.Length > 0)
                        //{
                        //    strLotsApproved = strLotsApproved.Substring(0, strLotsApproved.Length - 2);
                        //    strMessageApproved = "Los Lotes pendientes " + strLotsApproved + " fueron Aprobados.";
                        //}
                        //if (strLotsNotApproved != "" && strLotsNotApproved.Length > 0)
                        //{
                        //    strLotsNotApproved = strLotsNotApproved.Substring(0, strLotsNotApproved.Length - 2);
                        //    strMessageNotApproved = " Los Lotes pendientes " + strLotsNotApproved + " no fueron Aprobados, pues no tienen control de calidad.";
                        //}
                        //var quantityDrainedAux = GetTotalQuantityDrained(plTmp);
                        //if (quantityDrainedAux <= 0 && strLotsNotApproved.Length > 0)
                        //{
                        //    strLotsNotApproved = strLotsNotApproved.Substring(0, strLotsNotApproved.Length - 2);
                        //    strMessageNotApproved = " Los Lotes pendientes " + strLotsNotApproved + " no fueron Aprobados, pues no tienen prueba de escurrido realizada";
                        //}


                        //strMessageFinal = WarningMessage(strMessageFinal);
                        oJsonResult.codeReturn = -1;
                        oJsonResult.message = ErrorMessage(strMessageFinal);
                    }

                }
                catch //(Exception e)
                {
                    trans.Rollback();
                    var result = new { message = strMessageFinal };
                    oJsonResult.codeReturn = -1;
                    oJsonResult.message = ErrorMessage(strMessageFinal);
                }
                return Json(oJsonResult, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        #region"PRINT REPORTS PRELIMINARY FINAL"

        public JsonResult ProductionLotReceptionReportFilter(ReportModel reportModel, int? id_productionLot)
        {
            ProductionLot _productionLot = null;
            if (TempData["productionLotReception"] != null)
            {
                _productionLot = (ProductionLot)TempData["productionLotReception"];
            }

            TempData.Keep("productionLotReception");
            bool isvalid = false;
            string message = "";
            string strnamedata = "reportModel" + DateTime.Now.ToString("yyyyMMddmmssfff");

            TempData.Keep("productionLotReception");

            try
            {

                int id_plTmp = _productionLot.id;



                if (reportModel == null)
                {
                    reportModel = new ReportModel
                    {
                        ReportName = "ProductionLotPaymentReport"
                    };
                }
                else
                {
                    if (reportModel.ListReportParameter == null)
                    {
                        reportModel.ListReportParameter = new List<ReportParameter>();
                    }
                }
                isvalid = true;
            }
            catch //(Exception ex)
            {
            }
            TempData[strnamedata] = reportModel;
            TempData.Keep(strnamedata);

            var result = new
            {
                isvalid,
                message,
                reportModel = strnamedata,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductionLotReceptionPayEndReportFilter(ReportModel reportModel, int? id_productionLot)
        {
            ProductionLot _productionLot = null;
            if (TempData["productionLotReception"] != null)
            {
                _productionLot = (ProductionLot)TempData["productionLotReception"];
            }

            TempData.Keep("productionLotReception");
            bool isvalid = false;
            string message = "";
            string strnamedata = "reportModel" + DateTime.Now.ToString("yyyyMMddmmssfff");


            try
            {

                int id_plTmp = _productionLot.id;



                if (reportModel == null)
                {
                    reportModel = new ReportModel
                    {
                        ReportName = "ProductionLotPaymentEndReport"
                    };

                }
                else
                {
                    if (reportModel.ListReportParameter == null)
                    {

                        reportModel.ListReportParameter = new List<ReportParameter>();

                    }
                    ReportParameter reportParameter = new ReportParameter();
                    //reportParameter.Name = "id_invoiceexterior";
                    //reportParameter.Value = ViewData["id_invoiceexterior"].ToString();
                    //reportModel.ListReportParameter.Add(reportParameter);


                    ////reportParameter = new ReportParameter();
                    //reportParameter.Name = "id_productionLotTmp";
                    //reportParameter.Value = id_plTmp.ToString();
                    //reportModel.ListReportParameter.Add(reportParameter);



                }

                isvalid = true;
            }
            catch //(Exception ex)
            {


            }


            TempData[strnamedata] = reportModel;
            TempData.Keep(strnamedata);





            var result = new
            {
                isvalid,
                message,
                reportModel = strnamedata,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PRINT REPORTS FILTER PAGE
        public JsonResult PrintReportVitacora(string codeReport, ProductionLot productionLot,
                                int? filterWarehouse, int? filterWarehouseLocation,
                                DateTime? startReceptionDate, DateTime? endReceptionDate,
                                int[] items)
        {
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();

            string str_startReceptionDate = "";
            if (startReceptionDate != null) { str_startReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@dt_start",
                Valor = str_startReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@dt_end",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PRReception(string codeReport, ProductionLot productionLot,
                                        int? filterWarehouse, int? filterWarehouseLocation,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate,
                                        int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        //----------------------------------------------------

                [HttpPost, ValidateInput(false)]
        public JsonResult PRMargenporTallas(string codeReport, ProductionLot productionLot,
                                        int? Proveedor, 
                                        DateTime? startReceptionDate, DateTime? endReceptionDate,
                                        int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
                        {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0

            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }

            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@seqliq",
                Valor = productionLot?.sequentialLiquidation ?? 0
            };

            _param = new ParamCR
            {
                Nombre = "@tipoProceso",
                Valor = productionLot?.id_processtype ?? 0
            };

            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@estado",
                Valor = productionLot?.id_ProductionLotState ?? 0
            };

            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@idPersonaProceso",
                Valor = productionLot?.id_personProcessPlant ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            db.Database.CommandTimeout = 2200;
            List<ResultProductSizeBuy> modelAux = new List<ResultProductSizeBuy>();
            modelAux = db.Database.SqlQuery<ResultProductSizeBuy>
                   ("exec SP_ResumenTallaCompras @proveedor,@fi,@ff,@estado,@tipoProceso,@idPersonaProceso",
                   new SqlParameter("proveedor", paramLst[0].Valor),
                   new SqlParameter("fi", paramLst[1].Valor),
                   new SqlParameter("ff", paramLst[2].Valor),
                   new SqlParameter("estado", paramLst[3].Valor),
                   new SqlParameter("tipoProceso", paramLst[4].Valor),
                   new SqlParameter("idPersonaProceso", paramLst[5].Valor)
                   ).ToList();

            TempData["modelProcessSizeBuy"] = modelAux;
            TempData["repMod"] = _repMod;
            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        //----------------------------------------------------

        //----------------------------------------------------

        [HttpPost, ValidateInput(false)]
        public JsonResult PRResumenProveedorCompras(string codeReport, ProductionLot productionLot,
                                int? Proveedor,
                                DateTime? startReceptionDate, DateTime? endReceptionDate,
                                int[] items)
         {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0

            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }

            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@idPersonaProceso",
                Valor = productionLot?.id_personProcessPlant ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            db.Database.CommandTimeout = 2200;
            List<ResultSupplierLiquidationResumen> modelAux = new List<ResultSupplierLiquidationResumen>();
            modelAux = db.Database.SqlQuery<ResultSupplierLiquidationResumen>
                   ("exec SP_ResumenLiquidacionesProveedor @proveedor,@fi,@ff,@idPersonaProceso",
                   new SqlParameter("proveedor", paramLst[0].Valor),
                   new SqlParameter("fi", paramLst[1].Valor),
                   new SqlParameter("ff", paramLst[2].Valor),
                   new SqlParameter("idPersonaProceso", paramLst[3].Valor)
                   ).ToList();

            TempData["modelLiquidationSupplier"] = modelAux;

            modelAux = db.Database.SqlQuery<ResultSupplierLiquidationResumen>
                   ("exec SP_ResumenLiquidacionesProveedor_Resumen @proveedor,@fi,@ff,@idPersonaProceso",
                   new SqlParameter("proveedor", paramLst[0].Valor),
                   new SqlParameter("fi", paramLst[1].Valor),
                   new SqlParameter("ff", paramLst[2].Valor),
                   new SqlParameter("idPersonaProceso", paramLst[3].Valor)
                   ).ToList();

            TempData["modelLiquidationSupplieRresume"] = modelAux;

            TempData["repModLS"] = _repMod;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        //----------------------------------------------------



        //----------------------------------------------------

        [HttpPost, ValidateInput(false)]
        public JsonResult PRResumenCompraPeriodo(string codeReport, ProductionLot productionLot,
                                int? Proveedor,
                                DateTime? startReceptionDate, DateTime? endReceptionDate,
                                int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0

            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }

            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);





            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        //----------------------------------------------------


        //----------------------------------------------------

        [HttpPost, ValidateInput(false)]
        public JsonResult PRResumenCompraPeriodoG(string codeReport, ProductionLot productionLot,
                                int? Proveedor,
                                DateTime? startReceptionDate, DateTime? endReceptionDate,
                                int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0

            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }

            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);


            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        //----------------------------------------------------


        [HttpPost, ValidateInput(false)]
        public JsonResult PRPoundsLiquidation(string codeReport, ProductionLot productionLot,
                                        int? filterWarehouse, int? filterWarehouseLocation,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate,
                                        int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PRShrimpIncome(string codeReport, ProductionLot productionLot,
                                int? filterWarehouse, int? filterWarehouseLocation,
                                DateTime? startReceptionDate, DateTime? endReceptionDate,
                                int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        #endregion

        [HttpPost, ValidateInput(false)]
        public JsonResult PRLiquidacionSequencial(string codeReport, ProductionLot productionLot,
                                        int? filterWarehouse, int? filterWarehouseLocation,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate,
                                        int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PRLiquidacionValorizadaLiquid(string codeReport, ProductionLot productionLot,
                                        int? filterWarehouse, int? filterWarehouseLocation,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate,
                                        int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PRLiquidacionValorizadaLiquidPAproved(string codeReport, ProductionLot productionLot,
                                        int? filterWarehouse, int? filterWarehouseLocation,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate,
                                        int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        public JsonResult PRLiquidacionValorizadaLiquidProv(string codeReport, ProductionLot productionLot,
                                        int? filterWarehouse, int? filterWarehouseLocation,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate,
                                        int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        public JsonResult PRLiquidacionValorizadaLiquidProvPAproved(string codeReport, ProductionLot productionLot,
                                        int? filterWarehouse, int? filterWarehouseLocation,
                                        DateTime? startReceptionDate, DateTime? endReceptionDate,
                                        int[] items)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = productionLot?.internalNumber ?? ""
            };
            paramLst.Add(_param);

            string str_starReceptionDate = "";
            if (startReceptionDate != null) { str_starReceptionDate = startReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@fi",
                Valor = str_starReceptionDate
            };
            paramLst.Add(_param);

            string str_endReceptionDate = "";
            if (endReceptionDate != null) { str_endReceptionDate = endReceptionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR
            {
                Nombre = "@ff",
                Valor = str_endReceptionDate
            };
            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@proveedor",
                Valor = productionLot?.id_provider ?? 0
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }
        //Fin Cambios Miguel

        #region PRINT REPORTS DOCUMENT
        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReportsForDocumentGeneral(int id_pl, string codeReport)
        {
            ProductionLot ptTmp = (TempData["productionLotReception"] as ProductionLot);
            ptTmp = ptTmp ?? new ProductionLot();
            TempData["productionLotReception"] = ptTmp;
            TempData.Keep("productionLotReception");

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id_pl
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReportsForDistribucion(int id_pl, string codeReport)
        {
            ProductionLot ptTmp = (TempData["productionLotReception"] as ProductionLot);
            ptTmp = ptTmp ?? new ProductionLot();
            TempData["productionLotReception"] = ptTmp;
            TempData.Keep("productionLotReception");

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id_pl
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReportsForDocumentGeneralLiquidation(int id_pl, string codeReport)
        {
            ProductionLot ptTmp = (TempData["productionLotReception"] as ProductionLot);
            ptTmp = ptTmp ?? new ProductionLot();
            TempData["productionLotReception"] = ptTmp;
            TempData.Keep("productionLotReception");

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id_pl
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReportClose(int id_productionLot, string codeProcessType, string codeReport)
        {
            ProductionLot ptTmp = (TempData["productionLotReception"] as ProductionLot);
            ptTmp = ptTmp ?? new ProductionLot();
            TempData["productionLotReception"] = ptTmp;
            TempData.Keep("productionLotReception");

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id_productionLot
            };

            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@codeProcessType",
                Valor = codeProcessType
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintReportCloseRend(int id_productionLot, string codeProcessType, string codeReport)
        {
            ProductionLot ptTmp = (TempData["productionLotReception"] as ProductionLot);
            ptTmp = ptTmp ?? new ProductionLot();
            TempData["productionLotReception"] = ptTmp;
            TempData.Keep("productionLotReception");

            #region "Armo Parametros"
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = id_productionLot
            };

            paramLst.Add(_param);

            _param = new ParamCR
            {
                Nombre = "@codeProcessType",
                Valor = codeProcessType
            };

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel
            {
                codeReport = codeReport,
                conex = objConex,
                paramCRList = paramLst
            };

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }
        #endregion

        #region Método auxiliar para crear/actualizar el registro de romaneo

        private void ActualizarRegistroRomaneo(ProductionLot productionLot, string productionUnitProviderName, string productionUnitProviderPoolName)
        {
            var resultProdRomaneo = productionLot.ResultProdLotRomaneo;

            if (resultProdRomaneo == null)
            {
                resultProdRomaneo = new ResultProdLotRomaneo();
                productionLot.ResultProdLotRomaneo = resultProdRomaneo;
            }

            resultProdRomaneo.numberLot = productionLot.internalNumber;
            resultProdRomaneo.numberLotSequential = productionLot.number;
            resultProdRomaneo.nameProvider = productionLot.Provider.Person.fullname_businessName;
            resultProdRomaneo.nameProviderShrimp = productionUnitProviderName;  //db.ProductionUnitProvider.FirstOrDefault(p => p.id == productionLot.id_productionUnitProvider)?.name;
            resultProdRomaneo.namePool = productionUnitProviderPoolName;  //db.ProductionUnitProviderPool.FirstOrDefault(p => p.id == productionLot.id_productionUnitProviderPool)?.name;
            resultProdRomaneo.INPnumber = productionLot.INPnumberPL;
            resultProdRomaneo.dateTimeReception = productionLot.receptionDate;

            var firstProductionLotDetail = productionLot.ProductionLotDetail.First();
            var firstProductionLotDetailItem = firstProductionLotDetail.Item;

            resultProdRomaneo.nameItem = firstProductionLotDetailItem.name;
            resultProdRomaneo.nameWarehouseItem = firstProductionLotDetail.Warehouse.name;
            resultProdRomaneo.nameWarehouseLocationItem = firstProductionLotDetail.WarehouseLocation.name;
            resultProdRomaneo.codeProcessType = firstProductionLotDetailItem.ItemProcessType.FirstOrDefault()?.ProcessType?.code;
            resultProdRomaneo.quantityRemitted = productionLot.totalQuantityRecived; // Aquí se debe enviar la cantidad recibida!
            resultProdRomaneo.idMetricUnit = firstProductionLotDetailItem.ItemPurchaseInformation?.id_metricUnitPurchase ?? 0;
            resultProdRomaneo.gavetaNumber = firstProductionLotDetail.drawersNumber ?? 0;
        }

        #endregion

        private int? GetIdPurchaseOrderFromRemissionGuide(RemissionGuide remissionGuide)
        {
            if ((remissionGuide != null) && (remissionGuide.RemissionGuideDetail != null))
            {
                foreach (var remissionGuideDetail in remissionGuide.RemissionGuideDetail)
                {
                    if ((remissionGuideDetail != null)
                        && (remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail != null))
                    {
                        foreach (var remissionGuideDetailPurchaseDetail in remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail)
                        {
                            if ((remissionGuideDetailPurchaseDetail != null)
                                && (remissionGuideDetailPurchaseDetail.PurchaseOrderDetail != null))
                            {
                                return remissionGuideDetailPurchaseDetail.PurchaseOrderDetail.id_purchaseOrder;
                            }
                        }
                    }
                }
            }

            return null;
        }

        #region Metodos Auxiliaes Optimizados 
        private void UpdateProductionLotTotalsOP(ProductionLot productionLot,
                                                    Item[] items,
                                                    MetricUnitConversion[] metricUnitConversions,
                                                    Setting[] settings,
                                                    MetricUnit[] metricUnits)
        {
            productionLot.totalQuantityOrdered = 0.0M;
            productionLot.totalQuantityRemitted = 0.0M;
            productionLot.totalQuantityRecived = 0.0M;

            var metricUnitUMTPAux = settings.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = metricUnits.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotDetail in productionLot.ProductionLotDetail)
            {
                var ItemAux = items.FirstOrDefault(fod => fod.id == productionLotDetail.id_item);
                var id_metricUnitAux = ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = metricUnitConversions.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityOrdered += decimal.Round(productionLotDetail.quantityOrdered * factor, 2);
                productionLot.totalQuantityRemitted += decimal.Round(productionLotDetail.quantityRemitted * factor, 2);
                productionLot.totalQuantityRecived += decimal.Round(productionLotDetail.quantityRecived * factor, 2);
            }

            //productionLot.totalQuantityOrdered = decimal.Round(productionLot.totalQuantityOrdered, 2);
            //productionLot.totalQuantityRemitted = decimal.Round(productionLot.totalQuantityRemitted, 2);
            //productionLot.totalQuantityRecived = decimal.Round(productionLot.totalQuantityRecived, 2);
        }

        private void UpdateProductionLotProductionLotLiquidationsDetailTotalsOP(    ProductionLot productionLot,
                                                                                    Item[] itemsLiq,
                                                                                    MetricUnitConversion[] metricUnitConversions,
                                                                                    Setting[] settings,
                                                                                    MetricUnit[] metricUnits)
        {
            productionLot.totalQuantityLiquidation = 0.0M;
            productionLot.wholeSubtotal = 0.0M;
            productionLot.subtotalTail = 0.0M;

            var metricUnitUMTPAux = settings.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = metricUnits.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotLiquidation in productionLot.ProductionLotLiquidation)
            {
                var ItemAux = itemsLiq.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_item);
                var id_metricUnitAux = productionLotLiquidation.id_metricUnitPresentation;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = metricUnitConversions.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                var factorlib = decimal.Truncate(((ItemAux.Presentation?.minimum ?? 1) * factor) * 100000m) / 100000m;
                //var valueAux = decimal.Truncate(productionLotLiquidation.quantityPoundsLiquidation.Value*100)/100;
                var valueAux = Math.Round((productionLotLiquidation.quantity * factorlib), 2);
                //var valueAux = decimal.Round(productionLotLiquidation.quantityPoundsLiquidation.Value, 2);
                productionLot.totalQuantityLiquidation += valueAux;
                //if (productionLotLiquidation.Item.ItemTypeCategory.code == "ENT")
                var codeAux = productionLotLiquidation.Item?.ItemType?.ProcessType?.code ?? "";
                if (codeAux == "ENT")
                {
                    productionLot.wholeSubtotal += valueAux;
                }
                else
                {
                    productionLot.subtotalTail += valueAux;
                }

            }
            
        }

        private void UpdateProductionLotProductionLotTrashsDetailTotalsOP(  ProductionLot productionLot,
                                                                            Item[] itemsTrashes,
                                                                            MetricUnitConversion[] metricUnitConversions,
                                                                            Setting[] settings,
                                                                            MetricUnit[] metricUnits)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            productionLot.totalQuantityTrash = 0.0M;

            var metricUnitUMTPAux = settings.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = metricUnits.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotTrash in productionLot.ProductionLotTrash)
            {
                var ItemAux = itemsTrashes.FirstOrDefault(fod => fod.id == productionLotTrash.id_item);
                var id_metricUnitAux = productionLotTrash.id_metricUnit;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = metricUnitConversions.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityTrash += productionLotTrash.quantity * factor;
            }
            productionLot.totalQuantityTrash = decimal.Round(productionLot.totalQuantityTrash, 2);

        }

        private decimal GetTotalQuantityDrainedOP(  ProductionLot productionLot,
                                                    Item[] itemsDetails,
                                                    MetricUnitConversion[] metricUnitConversions,
                                                    Setting[] settings,
                                                    MetricUnit[] metricUnits)
        {
            var resultAux = 0.0M;

            var metricUnitUMTPAux = settings.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = metricUnits.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotDetail in productionLot.ProductionLotDetail)
            {
                var ItemAux = itemsDetails.FirstOrDefault(fod => fod.id == productionLotDetail.id_item);
                var id_metricUnitAux = ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = metricUnitConversions.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                resultAux += decimal.Round((productionLotDetail.quantitydrained ?? 0) * factor, 2);
            }
            return resultAux;
            
        }


        private void UpdateProductionLotProductionLotLiquidationTotalsOP(ProductionLot productionLot, Setting[] settings, MetricUnit[] metricUnits)
        {
            productionLot.totalQuantityLiquidation = 0.0M;
            productionLot.wholeSubtotal = 0.0M;
            productionLot.subtotalTail = 0.0M;

            var metricUnitUMTPAux = settings.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = metricUnits.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotLiquidationTotal in productionLot.ProductionLotLiquidationTotal)
            {
                //var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidationTotal.id_ItemLiquidation);
                //var id_metricUnitAux = productionLotLiquidationTotal.id_metricUnitPresentation;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                //var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                //                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                //                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                //var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                //var valueAux = decimal.Truncate(productionLotLiquidationTotal.quantityPoundsIL * 100) / 100;
                //var factorlib = decimal.Truncate(((ItemAux.Presentation?.minimum ?? 1) * factor) * 100000m) / 100000m;
                //var valueAux = Math.Round((productionLotLiquidationTotal.quatityBoxesIL * factorlib),2);
                //var valueAux = decimal.Truncate((productionLotLiquidationTotal.quatityBoxesIL * factorlib) * 100) / 100;
                var valueAux = productionLotLiquidationTotal.quantityPoundsIL;
                //productionLotLiquidationTotal.quantityPoundsIL = valueAux;

                productionLot.totalQuantityLiquidation += valueAux;

                var codeAux = productionLotLiquidationTotal.Item?.ItemType?.ProcessType?.code ?? "";
                if (codeAux == "ENT")
                {
                    productionLot.wholeSubtotal += valueAux;
                }
                else
                {
                    productionLot.subtotalTail += valueAux;
                }

            }

        }

        private void UpdateProductionLotPerformanceOP(  ProductionLot productionLot,
                                                        Setting[] settings,
                                                        MetricUnit[] metricUnits,
                                                        Item[] items,
                                                        MetricUnitConversion[] metricUnitConversions,
                                                        Item[] itemsEquivalences)
        {
            productionLot.totalQuantityLiquidationAdjust = 0.0M;
            productionLot.totalAdjustmentPounds = 0.0M;
            productionLot.totalAdjustmentWholePounds = 0.0M;
            productionLot.totalAdjustmentTailPounds = 0.0M;
            productionLot.wholeSubtotalAdjust = 0.0M;
            productionLot.subtotalTailAdjust = 0.0M;
            productionLot.wholeSubtotalAdjustProcess = 0.0M;
            productionLot.wholeTotalToPay = 0.0M;
            productionLot.tailTotalToPay = 0.0M;
            productionLot.totalToPay = 0.0M;
            productionLot.totalToPayEq = 0.0M;

            var metricUnitUMTPAux = settings.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = metricUnits.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var metricUnitUMEPAux = settings.FirstOrDefault(fod => fod.code.Equals("UMEP"));
            var id_metricUnitUMEPValueAux = int.Parse(metricUnitUMEPAux?.value ?? "0");
            var metricUnitUMEP = metricUnits.FirstOrDefault(fod => fod.id == id_metricUnitUMEPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;
            var id_metricUnitKgAux = metricUnitUMEP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotPayment in productionLot.ProductionLotPayment)
            {
                var ItemAux = items.FirstOrDefault(fod => fod.id == productionLotPayment.id_item);
                var id_metricUnitAux = productionLotPayment.id_metricUnit;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var id_metricUnitProcessAux = productionLotPayment.id_metricUnitProcess;
                var metricUnitConversion = metricUnitConversions.FirstOrDefault(    muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                var factorlib = decimal.Truncate(((ItemAux.Presentation?.minimum ?? 1) * factor) * 100000m) / 100000m;
                
                var quantityAux = Math.Round((productionLotPayment.quantityEntered.Value * factorlib), 2);
                
                var adjustAux = (productionLotPayment.adjustMore ?? 0) + (productionLotPayment.adjustLess ?? 0);
                var totalMUAux = Math.Round((quantityAux + (adjustAux * factor)), 2);
                

                metricUnitConversion = metricUnitConversions.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                   muc.id_metricOrigin == id_metricUnitAux &&
                                                                                   muc.id_metricDestiny == id_metricUnitKgAux);
                factor = id_metricUnitKgAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                var metricUnitConversionProcess = metricUnitConversions.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitProcessAux);

                productionLotPayment.totalPounds = totalMUAux;
                var fitPoundsAux = totalMUAux - quantityAux;
                productionLotPayment.fitPounds = fitPoundsAux;

                productionLot.totalQuantityLiquidationAdjust += totalMUAux;
                productionLot.totalAdjustmentPounds += fitPoundsAux;
                
                productionLot.totalToPay += productionLotPayment.totalToPay;
                productionLot.totalToPayEq += productionLotPayment.totalToPayEq;

                var ItemEqAux = itemsEquivalences.FirstOrDefault(fod => fod.id == ItemAux.ItemEquivalence.id_itemEquivalence);

                var factorEq = decimal.Truncate(((ItemEqAux.Presentation?.minimum ?? 1)) * 100000m) / 100000m;
                var totalMUEqAux = Math.Round((productionLotPayment.quantityEntered.Value * factorEq * factor), 2);
                var totalMUEqColAux = (id_metricUnitAux == 1) ? Math.Round((productionLotPayment.quantityEntered.Value * factorEq * metricUnitConversionProcess?.factor ?? 0), 2) : Math.Round((productionLotPayment.quantityEntered.Value * factorEq), 2);

                //if (ItemAux.ItemTypeCategory.code == "ENT")
                //{
                var codeAux = ItemAux?.ItemType?.ProcessType?.code ?? "";
                if (codeAux == "ENT")
                {
                    productionLot.totalAdjustmentWholePounds += fitPoundsAux;
                    productionLot.wholeSubtotalAdjust += totalMUAux;
                    var totalMUAux2 = decimal.Round(productionLotPayment.totalMU * factor, 2);
                    productionLot.wholeSubtotalAdjustProcess += totalMUAux2;
                    productionLotPayment.totalProcessMetricUnit = totalMUAux2;
                    productionLotPayment.totalProcessMetricUnitEq = totalMUEqAux;
                    productionLotPayment.id_metricUnitProcess = id_metricUnitKgAux;

                    productionLot.wholeTotalToPay += productionLotPayment.totalToPay;
                }
                else
                {
                    productionLot.totalAdjustmentTailPounds += fitPoundsAux;
                    productionLot.subtotalTailAdjust += totalMUAux;

                    productionLotPayment.totalProcessMetricUnit = totalMUAux;
                    productionLotPayment.totalProcessMetricUnitEq = totalMUEqColAux;
                    productionLotPayment.id_metricUnitProcess = id_metricUnitLbsAux;

                    productionLot.tailTotalToPay += productionLotPayment.totalToPay;
                }
                
            }
            
        }

        private void ActualizarRegistroRomaneoOP(ProductionLot productionLot, string productionUnitProviderName , string productionUnitProviderPoolName)
        {
            var resultProdRomaneo = productionLot.ResultProdLotRomaneo;

            if (resultProdRomaneo == null)
            {
                resultProdRomaneo = new ResultProdLotRomaneo();
                productionLot.ResultProdLotRomaneo = resultProdRomaneo;
            }

            resultProdRomaneo.numberLot = productionLot.internalNumber;
            resultProdRomaneo.numberLotSequential = productionLot.number;
            resultProdRomaneo.nameProvider = productionLot.Provider.Person.fullname_businessName;
            resultProdRomaneo.nameProviderShrimp = productionUnitProviderName; //db.ProductionUnitProvider.FirstOrDefault(p => p.id == productionLot.id_productionUnitProvider)?.name;
            resultProdRomaneo.namePool = productionUnitProviderPoolName;  //db.ProductionUnitProviderPool.FirstOrDefault(p => p.id == productionLot.id_productionUnitProviderPool)?.name;
            resultProdRomaneo.INPnumber = productionLot.INPnumberPL;
            resultProdRomaneo.dateTimeReception = productionLot.receptionDate;

            var firstProductionLotDetail = productionLot.ProductionLotDetail.First();
            var firstProductionLotDetailItem = firstProductionLotDetail.Item;

            resultProdRomaneo.nameItem = firstProductionLotDetailItem.name;
            resultProdRomaneo.nameWarehouseItem = firstProductionLotDetail.Warehouse.name;
            resultProdRomaneo.nameWarehouseLocationItem = firstProductionLotDetail.WarehouseLocation.name;
            resultProdRomaneo.codeProcessType = firstProductionLotDetailItem.ItemProcessType.FirstOrDefault()?.ProcessType?.code;
            resultProdRomaneo.quantityRemitted = productionLot.totalQuantityRecived; // Aquí se debe enviar la cantidad recibida!
            resultProdRomaneo.idMetricUnit = firstProductionLotDetailItem.ItemPurchaseInformation?.id_metricUnitPurchase ?? 0;
            resultProdRomaneo.gavetaNumber = firstProductionLotDetail.drawersNumber ?? 0;
        }
        #endregion


        private void ActualizarRegistroRomaneo(ProductionLot productionLot)
        {
            var resultProdRomaneo = productionLot.ResultProdLotRomaneo;

            if (resultProdRomaneo == null)
            {
                resultProdRomaneo = new ResultProdLotRomaneo();
                productionLot.ResultProdLotRomaneo = resultProdRomaneo;
            }

            resultProdRomaneo.numberLot = productionLot.internalNumber;
            resultProdRomaneo.numberLotSequential = productionLot.number;
            resultProdRomaneo.nameProvider = productionLot.Provider.Person.fullname_businessName;
            resultProdRomaneo.nameProviderShrimp = db.ProductionUnitProvider.FirstOrDefault(p => p.id == productionLot.id_productionUnitProvider)?.name;
            resultProdRomaneo.namePool = db.ProductionUnitProviderPool.FirstOrDefault(p => p.id == productionLot.id_productionUnitProviderPool)?.name;
            resultProdRomaneo.INPnumber = productionLot.INPnumberPL;
            resultProdRomaneo.dateTimeReception = productionLot.receptionDate;

            var firstProductionLotDetail = productionLot.ProductionLotDetail.First();
            var firstProductionLotDetailItem = firstProductionLotDetail.Item;

            resultProdRomaneo.nameItem = firstProductionLotDetailItem.name;
            resultProdRomaneo.nameWarehouseItem = firstProductionLotDetail.Warehouse.name;
            resultProdRomaneo.nameWarehouseLocationItem = firstProductionLotDetail.WarehouseLocation.name;
            resultProdRomaneo.codeProcessType = firstProductionLotDetailItem.ItemProcessType.FirstOrDefault()?.ProcessType?.code;
            resultProdRomaneo.quantityRemitted = productionLot.totalQuantityRecived; // Aquí se debe enviar la cantidad recibida!
            resultProdRomaneo.idMetricUnit = firstProductionLotDetailItem.ItemPurchaseInformation?.id_metricUnitPurchase ?? 0;
            resultProdRomaneo.gavetaNumber = firstProductionLotDetail.drawersNumber ?? 0;
        }

        private void UpdateProductionLotTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityOrdered = 0.0M;
            productionLot.totalQuantityRemitted = 0.0M;
            productionLot.totalQuantityRecived = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;//db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotDetail in productionLot.ProductionLotDetail)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotDetail.id_item);
                var id_metricUnitAux = ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityOrdered += decimal.Round(productionLotDetail.quantityOrdered * factor, 2);
                productionLot.totalQuantityRemitted += decimal.Round(productionLotDetail.quantityRemitted * factor, 2);
                productionLot.totalQuantityRecived += decimal.Round(productionLotDetail.quantityRecived * factor, 2);
            }

        }
    }
}