using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Services;

namespace DXPANACEASOFT.Controllers
{
    public class LiquidationMaterialController : DefaultController
    {
        // GET: LiquidationMaterial
        public ActionResult Index()
        {
            BuildViewData();
            return View();
        }

        private LiquidationMaterialDTO GetLiquidationMaterialDTO()
        {
            if (!(Session["LiquidationMaterialDTO"] is LiquidationMaterialDTO liquidationMaterialDTO))
                liquidationMaterialDTO = new LiquidationMaterialDTO();
            return liquidationMaterialDTO;
        }

        private List<LiquidationMaterialResultConsultDTO> GetLiquidationMaterialResultConsultDTO()
        {
            if (!(Session["LiquidationMaterialResultConsultDTO"] is List<LiquidationMaterialResultConsultDTO> liquidationMaterialResultConsult))
                liquidationMaterialResultConsult = new List<LiquidationMaterialResultConsultDTO>();
            return liquidationMaterialResultConsult;
        }

        private void SetLiquidationMaterialDTO(LiquidationMaterialDTO liquidationMaterialDTO)
        {
            Session["LiquidationMaterialDTO"] = liquidationMaterialDTO;
        }

        private void SetLiquidationMaterialResultConsultDTO(List<LiquidationMaterialResultConsultDTO> liquidationMaterialResultConsultDTO)
        {
            Session["LiquidationMaterialResultConsultDTO"] = liquidationMaterialResultConsultDTO;
        }

        private void Estados(string From = "")
        {
            ViewData["Estados"] = db.tbsysDocumentTypeDocumentState
                    .Where(d => d.DocumentType.code.Equals("18") ||
                                d.DocumentType.code.Equals("19") ||
                                d.DocumentType.code.Equals("20") ||
                                d.DocumentType.code.Equals("21"))
                    .Select(s => new SelectListItem
                    {
                        Text = s.DocumentState.name,
                        Value = s.id_DocumenteState.ToString()
                    }).Distinct().ToList();
        }

        public ActionResult ComboBoxEstados(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                Estados();

                if (From.Equals("Edit"))
                {
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxEstadosEdit");
                }
                else
                {
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxEstadosIndex");
                }
            }
        }

        private void Proveedores(string From = "")
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    ViewData["Proveedores"] = db.Provider.Select(s => new SelectListItem
                    {
                        Text = s.Person.fullname_businessName,
                        Value = s.id.ToString()
                    }).ToList();
                }
                else
                {
                    ViewData["Proveedores"] = db.Provider.Select(s => new SelectListItem
                    {
                        Text = s.Person.fullname_businessName,
                        Value = s.id.ToString()
                    }).ToList();
                }
            }
        }

        public ActionResult ComboBoxProveedores(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    Proveedores(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxProveedoresEdit");
                }
                else
                {
                    Proveedores(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxProveedoresIndex");
                }
            }
        }

        private void Productos(string From = "")
        {
            //return db.Item.Where(s => s.id_company == id_company && s.InventoryLine.code == "MI" && s.ItemType.code == "MDD").Select(s => new { id = s.id, name = s.name }).ToList();
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    ViewData["Productos"] = db.Item.Select(s => new SelectListItem
                    {
                        Text = s.name,
                        Value = s.id.ToString()
                    }).ToList();
                }
                else
                {
                    ViewData["Productos"] = db.Item.Select(s => new SelectListItem
                    {
                        Text = s.name,
                        Value = s.id.ToString()
                    }).ToList();
                }
            }
        }

        public ActionResult ComboBoxProductos(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    Productos(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxProductosEdit");
                }
                else
                {
                    Productos(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxProductosIndex");
                }
            }
        }

        private void BuildViewData(string From = "", PriceList priceList = null, bool enabled = true)
        {
            Session["consulta"] = null;
            Estados(From);
            Proveedores(From);
            Productos(From);

            if (!From.Equals("Edit"))
                return;

        }

        private List<LiquidationMaterialResultConsultDTO> GetLiquidationMaterialDTO(LiquidationMaterialConsultDTO consulta)
        {
            using (var db = new DBContext())
            {
                var consultaAux = Session["consulta"] as LiquidationMaterialConsultDTO;
                if (consultaAux != null) {
                    consulta = consultaAux;
                }
               

                var consultResult = db.LiquidationMaterialSupplies.Where(LiquidationMaterialQueryExtensions.GetRequestByFilter(consulta));
                var query = consultResult.Select(t => new LiquidationMaterialResultConsultDTO
                {
                    id = t.id,
                    numberDocument = t.Document.number,
                    emissionDateDocument = t.Document.emissionDate,
                    id_provider = t.idProvider,
                    provider = t.Provider.Person.fullname_businessName,
                    id_documentState = t.Document.id_documentState,
                    documentState = t.Document.DocumentState.name,

                    canEdit = t.Document.DocumentState.code.Equals("01"),
                    canAproved = t.Document.DocumentState.code.Equals("01"),
                    canAuthorize = t.Document.DocumentState.code.Equals("03"),
                    canAnnul = t.Document.DocumentState.code.Equals("01"),
                    canReverse = t.Document.DocumentState.code.Equals("03") || t.Document.DocumentState.code.Equals("06")

                }).OrderBy(ob=> ob.numberDocument).ToList();

                Session["consulta"] = consulta;

                return query;
            }
        }

        [HttpPost]
        public ActionResult SearchResult(LiquidationMaterialConsultDTO consulta)
        {
            var result = GetLiquidationMaterialDTO(consulta);

            SetLiquidationMaterialResultConsultDTO(result);

            return PartialView("ConsultResult", result);
        }

        [ValidateInput(false)]
        public ActionResult GridViewLiquidationMaterial(LiquidationMaterialConsultDTO consulta)
        {
            return PartialView("_GridViewLiquidationMaterial", GetLiquidationMaterialDTO(consulta));
        }

        private List<ResultReceptionDispatchMaterial> GetReceptionMaterialDTO()
        {
            using (var db = new DBContext())
            {
                var query = db.ResultReceptionDispatchMaterial.Where(d => d.idLiquidationMaterialSupplies == null).ToList();
                return query;
            }
        }

        [ValidateInput(false)]
        public ActionResult ReceptionMaterialApproved()
        {
            return PartialView("ReceptionMaterialApproved", GetReceptionMaterialDTO());
        }

        [ValidateInput(false)]
        public ActionResult GridViewReceptionMaterialApproved()
        {
            return PartialView("_GridViewReceptionMaterialApproved", GetReceptionMaterialDTO());
        }

        [HttpPost]
        public ActionResult Edit(int[] ids, int id, bool enabled = true)
        {
            var liquidationMaterialDTO = GetLiquidationMaterialDTO(ids, id);
            BuilViewBag(enabled);
            return PartialView(liquidationMaterialDTO);
        }

        private LiquidationMaterialDTO GetLiquidationMaterialDTO(int[] ids, int id)
        {
            LiquidationMaterialSupplies liquidationMaterial = db.LiquidationMaterialSupplies.FirstOrDefault(o => o.id == id);
            LiquidationMaterialDTO liquidationMaterialDTO = null;

            if (liquidationMaterial == null)
            {
                liquidationMaterialDTO = new LiquidationMaterialDTO();
                liquidationMaterialDTO.id = 0;
               
                liquidationMaterialDTO.emissionDateDocument = DateTime.Now;
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
                liquidationMaterialDTO.documentState = documentState.name;
                liquidationMaterialDTO.id_documentState = documentState.id;
                liquidationMaterialDTO.subTotal = 0M;
                liquidationMaterialDTO.iva = 0M;
                liquidationMaterialDTO.total = 0M;
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("95"));//"95- Liquidación Material"
                liquidationMaterialDTO.numberDocument = GetDocumentNumber(documentType?.id ?? 0);
                liquidationMaterialDTO.documentType = documentType.name;
                liquidationMaterialDTO.id_documentType = documentType.id;
                liquidationMaterialDTO.LiquidationMaterialDetailDTO = new List<LiquidationMaterialDetailDTO>();
                liquidationMaterialDTO.SummaryLiquidationMaterialDetailDTO = new List<SummaryLiquidationMaterialDetailDTO>();

                var id_providerAux = 0;

                if (ids != null)
                {
                    foreach (var id_receptionMaterial in ids)
                    {
                        ResultReceptionDispatchMaterial receptionMaterial = db.ResultReceptionDispatchMaterial.FirstOrDefault(d => d.idReceptionDispatchMaterial == id_receptionMaterial);

                        if (id_providerAux == 0)
                        {
                            id_providerAux = receptionMaterial.idProvider;
                            liquidationMaterialDTO.id_provider = id_providerAux;
                            liquidationMaterialDTO.provider = receptionMaterial.nameProvider;
                        }

                        foreach (var receptionMaterialDetail in receptionMaterial.ResultReceptionDispatchMaterialDetail)
                        {
                            decimal price = ItemDetailPrice(receptionMaterialDetail.idItem);
                            decimal subTotal = receptionMaterialDetail.quantity * price;
                            decimal subTotaliva = PriceDetailIVA(receptionMaterialDetail.idItem, receptionMaterialDetail.quantity, price);
                            decimal total = subTotal + subTotaliva;
                            Item item = db.Item.FirstOrDefault(i => i.id == receptionMaterialDetail.idItem);

                            var ivaAux = item?.ItemTaxation.FirstOrDefault()?.percentage ?? 0.00M;

                            LiquidationMaterialDetailDTO newLiquidationMaterialDetailDTO = new LiquidationMaterialDetailDTO
                            {
                                id = liquidationMaterialDTO.LiquidationMaterialDetailDTO.Any() ? (liquidationMaterialDTO.LiquidationMaterialDetailDTO.Max(m => m.id) + 1) : 1,
                                id_guia = id_receptionMaterial,
                                numberGuia = receptionMaterial.numberRemissionGuide,
                                emisionGuia = receptionMaterial.dateRemissionGuide,
                                id_guiaDetail = receptionMaterialDetail.id,
                                id_item = receptionMaterialDetail.idItem,
                                codigo = receptionMaterialDetail.Item.masterCode,
                                name = receptionMaterialDetail.Item.name,
                                id_metricUnit = receptionMaterialDetail.idMetricUnit,
                                metricUnit = receptionMaterialDetail.MetricUnit.code,
                                quantity = receptionMaterialDetail.quantity,
                                quantityOrigin = receptionMaterialDetail.quantity,
                                unitCostOrigin = price,
                                unitCost = price,
                                subTotalOrigin = subTotal,
                                subTotal = subTotal,
                                iva = ivaAux,
                                subTotalIvaOrigin = subTotaliva,
                                subTotalIva = subTotaliva,
                                totalOrigin = total,
                                total = total,
                                aprovedLogist = true,
                                aprovedComertial = true,
                                descriptionLogist = "",
                                descriptionComertial = ""
                            };
                            liquidationMaterialDTO.LiquidationMaterialDetailDTO.Add(newLiquidationMaterialDetailDTO);
                            liquidationMaterialDTO.subTotal += newLiquidationMaterialDetailDTO.subTotal;
                            liquidationMaterialDTO.iva += newLiquidationMaterialDetailDTO.iva;
                            liquidationMaterialDTO.total += newLiquidationMaterialDetailDTO.total;
                        }
                        liquidationMaterialDTO.LiquidationMaterialDetailDTO = liquidationMaterialDTO.LiquidationMaterialDetailDTO.OrderBy(ob => ob.codigo).ThenBy(tb => tb.numberGuia).ToList();
                        liquidationMaterialDTO.SummaryLiquidationMaterialDetailDTO = GetSummary(liquidationMaterialDTO.LiquidationMaterialDetailDTO);
                    }
                }
            }
            else
            {
                liquidationMaterialDTO = ConvertToDto(liquidationMaterial);
            }

            SetLiquidationMaterialDTO(liquidationMaterialDTO);
            return liquidationMaterialDTO;
        }
        private List<SummaryLiquidationMaterialDetailDTO> GetSummary(List<LiquidationMaterialDetailDTO> liquidationMaterialDetailDTO)
        {
            var summaryLiquidationMaterialDetailDTO = liquidationMaterialDetailDTO.GroupBy(gb => new
            {
                gb.id_item,
                gb.codigo,
                gb.name,
                gb.id_metricUnit,
                gb.metricUnit,
                gb.unitCost
            }).Select(s => new SummaryLiquidationMaterialDetailDTO
            {
                id_item = s.Key.id_item,
                codigo = s.Key.codigo,
                name = s.Key.name,
                id_metricUnit = s.Key.id_metricUnit,
                metricUnit = s.Key.metricUnit,
                quantity = s.Sum(ss => ss.quantity),
                unitCost = s.Key.unitCost,
                subTotal = s.Sum(ss => ss.subTotal),
                subTotalIva = s.Sum(ss => ss.subTotalIva),
                total = s.Sum(ss => ss.total)
            }).ToList();

            return summaryLiquidationMaterialDetailDTO;
        }

        private LiquidationMaterialDTO ConvertToDto(LiquidationMaterialSupplies liquidationMaterialSupplies)
        {
            if (liquidationMaterialSupplies == null)
                return null;

            var liquidationMaterialDTO = new LiquidationMaterialDTO();
            liquidationMaterialDTO.id = liquidationMaterialSupplies.id;
            liquidationMaterialDTO.numberDocument = liquidationMaterialSupplies.Document.number;
            liquidationMaterialDTO.emissionDateDocument = liquidationMaterialSupplies.Document.emissionDate;
            liquidationMaterialDTO.documentDescription = liquidationMaterialSupplies.Document.description;
            liquidationMaterialDTO.documentState = liquidationMaterialSupplies.Document.DocumentState.name;
            liquidationMaterialDTO.id_documentState = liquidationMaterialSupplies.Document.id_documentState;
            liquidationMaterialDTO.subTotal = liquidationMaterialSupplies.subTotal;
            liquidationMaterialDTO.iva = liquidationMaterialSupplies.subTotalTax;
            liquidationMaterialDTO.total = liquidationMaterialSupplies.Total;
            liquidationMaterialDTO.documentType = liquidationMaterialSupplies.Document.DocumentType.name;
            liquidationMaterialDTO.id_documentType = liquidationMaterialSupplies.Document.id_documentType;
            liquidationMaterialDTO.LiquidationMaterialDetailDTO = new List<LiquidationMaterialDetailDTO>();
            liquidationMaterialDTO.SummaryLiquidationMaterialDetailDTO = new List<SummaryLiquidationMaterialDetailDTO>();

            liquidationMaterialDTO.id_provider = liquidationMaterialSupplies.idProvider;
            liquidationMaterialDTO.provider = liquidationMaterialSupplies.Provider.Person.fullname_businessName;

            var canAproved = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTO.id_documentState)
                                   ?.code.Equals("01") ?? false) && (liquidationMaterialDTO.id != 0);
            //var canAuthorize = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTO.id_documentState)
            //                      ?.code.Equals("03") ?? false);

            foreach (var resultReceptionDispatchMaterial in liquidationMaterialSupplies.ResultReceptionDispatchMaterial)
            {

                foreach (var receptionMaterialDetail in resultReceptionDispatchMaterial.ResultReceptionDispatchMaterialDetail)
                {
                    LiquidationMaterialSuppliesDetail liquidationMaterialSuppliesDetail = liquidationMaterialSupplies.LiquidationMaterialSuppliesDetail.FirstOrDefault(o => o.id == receptionMaterialDetail.idLiquidationMaterialSuppliesDetail);

                    decimal price = liquidationMaterialSuppliesDetail.priceUnit;
                    decimal subTotal = liquidationMaterialSuppliesDetail.subTotal;
                    decimal subTotaliva = liquidationMaterialSuppliesDetail.subTotalTax;
                    decimal total = subTotal + subTotaliva;
                    decimal quantityAux = liquidationMaterialSuppliesDetail.quantity ?? 0.00M;

                    LiquidationMaterialDetailDTO newLiquidationMaterialDetailDTO = new LiquidationMaterialDetailDTO
                    {
                        id = liquidationMaterialSuppliesDetail.id,
                        id_guia = resultReceptionDispatchMaterial.idReceptionDispatchMaterial,
                        numberGuia = resultReceptionDispatchMaterial.numberRemissionGuide,
                        emisionGuia = resultReceptionDispatchMaterial.dateRemissionGuide,
                        id_guiaDetail = receptionMaterialDetail.id,
                        id_item = receptionMaterialDetail.idItem,
                        codigo = receptionMaterialDetail.Item.masterCode,
                        name = receptionMaterialDetail.Item.name,
                        id_metricUnit = receptionMaterialDetail.idMetricUnit,
                        metricUnit = receptionMaterialDetail.MetricUnit.code,
                        //quantity = receptionMaterialDetail.quantity,
                        quantityOrigin = quantityAux,
                        quantity = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? quantityAux : 0.00M) :
                                                (liquidationMaterialSuppliesDetail.aprovedComertial ? quantityAux : 0.00M),
                        unitCostOrigin = price,
                        unitCost = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? price : 0.00M) :
                                                (liquidationMaterialSuppliesDetail.aprovedComertial ? price : 0.00M),
                        subTotalOrigin = subTotal,
                        subTotal = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? subTotal : 0.00M) :
                                                (liquidationMaterialSuppliesDetail.aprovedComertial ? subTotal : 0.00M),
                        iva = liquidationMaterialSuppliesDetail.tax,
                        subTotalIvaOrigin = subTotaliva,
                        subTotalIva = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? subTotaliva : 0.00M) :
                                                   (liquidationMaterialSuppliesDetail.aprovedComertial ? subTotaliva : 0.00M),
                        totalOrigin = total,
                        total = canAproved ? (liquidationMaterialSuppliesDetail.aprovedLogist ? total : 0.00M) :
                                             (liquidationMaterialSuppliesDetail.aprovedComertial ? total : 0.00M),
                        descriptionLogist = liquidationMaterialSuppliesDetail.descriptionLogist,
                        descriptionComertial = liquidationMaterialSuppliesDetail.descriptionComertial,
                        aprovedLogist = liquidationMaterialSuppliesDetail.aprovedLogist,
                        aprovedComertial = liquidationMaterialSuppliesDetail.aprovedComertial
                    };
                    liquidationMaterialDTO.LiquidationMaterialDetailDTO.Add(newLiquidationMaterialDetailDTO);
                    //liquidationMaterialDTO.subTotal += newLiquidationMaterialDetailDTO.subTotal;
                    //liquidationMaterialDTO.iva += newLiquidationMaterialDetailDTO.iva;
                    //liquidationMaterialDTO.total += newLiquidationMaterialDetailDTO.total;
                }

                liquidationMaterialDTO.SummaryLiquidationMaterialDetailDTO = GetSummary(liquidationMaterialDTO.LiquidationMaterialDetailDTO);
            }

            return liquidationMaterialDTO;
        }

        private decimal ItemDetailPrice(int id_item)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return 0.0M;
            }

            decimal price = item?.ItemSaleInformation?.salePrice ?? 0.000M;

            return price;
        }

        private decimal PriceDetailIVA(int id_item, decimal quantity, decimal price)
        {
            decimal iva = 0.0M;

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return 0.0M;
            }

            var percentageAux = item?.ItemTaxation?.FirstOrDefault()?.percentage ?? 0.00M;

            iva = Math.Round(((quantity * price) * (percentageAux/100)), 2);

            return iva;
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewSummary()
        {
            var model = GetLiquidationMaterialDTO().SummaryLiquidationMaterialDetailDTO;
            return PartialView("_GridViewSummary", model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewDetail(bool canAproved, bool canAuthorize, bool canReverse, bool? visibleCantidadCero = false)
        {
            var model = GetLiquidationMaterialDTO().LiquidationMaterialDetailDTO.Where(w => (!visibleCantidadCero.Value ? w.quantityOrigin != 0 : true)).OrderBy(ob => ob.codigo).ThenBy(tb => tb.numberGuia).ToList();
            ViewBag.canAproved = canAproved;
            ViewBag.canAuthorize = canAuthorize;
            ViewBag.canReverse = canReverse;
            ViewBag.visibleCantidadCero = visibleCantidadCero.Value;
            return PartialView("_GridViewDetail", model);
        }

        private void BuilViewBag(bool enabled)
        {
            ViewBag.enabled = enabled;
            var liquidationMaterialDTOAux = GetLiquidationMaterialDTO();
            ViewBag.canEdit = !enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canCreate = (liquidationMaterialDTOAux.id != 0);
            ViewBag.canSave = enabled && (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAproved = enabled &&
                              (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
                                   ?.code.Equals("01") ?? false);
            ViewBag.canAuthorize = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
                                  ?.code.Equals("03") ?? false);
            ViewBag.canAnnul = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
                                  ?.code.Equals("01") ?? false) && (liquidationMaterialDTOAux.id != 0);
            ViewBag.canReverse = (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
                                  ?.code.Equals("03") ?? false) ||
                                  (db.DocumentState.AsEnumerable().FirstOrDefault(s => s.id == liquidationMaterialDTOAux.id_documentState)
                                  ?.code.Equals("06") ?? false);
            ViewBag.canPrint = (liquidationMaterialDTOAux.id != 0);
            ViewBag.visibleCantidadCero = false;
        }

        [HttpPost]
        public JsonResult Save(LiquidationMaterialDTO liquidationMaterialDTO, bool? approved = false)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        var liquidationMaterialDTOAux = GetLiquidationMaterialDTO();
                        var newObject = false;
                        var id = liquidationMaterialDTO.id;

                        var documentType = db.DocumentType.FirstOrDefault(d => d.code.Equals("95"));
                        var documentState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));

                        var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(d => d.id == id);
                        if (liquidationMaterialSupplies == null)
                        {
                            newObject = true;

                            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
                            if (id_emissionPoint == 0)
                                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                            liquidationMaterialSupplies = new LiquidationMaterialSupplies
                            {
                                Document = new Document
                                {
                                    number = GetDocumentNumber(documentType?.id ?? 0),
                                    sequential = GetDocumentSequential(documentType?.id ?? 0),
                                    authorizationDate = DateTime.Now,
                                    id_emissionPoint = id_emissionPoint,
                                    id_documentType = documentType.id,
                                    id_userCreate = ActiveUser.id,
                                    dateCreate = DateTime.Now,
                                    id_documentState = documentState.id,
                                    reference = ""
                                },
                                LiquidationMaterialSuppliesDetail = new List<LiquidationMaterialSuppliesDetail>()
                            };

                            documentType.currentNumber++;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;

                        }

                        liquidationMaterialSupplies.Document.emissionDate = liquidationMaterialDTO.emissionDateDocument;
                        liquidationMaterialSupplies.Document.id_userUpdate = ActiveUser.id;
                        liquidationMaterialSupplies.Document.dateUpdate = DateTime.Now;
                        liquidationMaterialSupplies.Document.description = liquidationMaterialDTO.documentDescription;

                        liquidationMaterialSupplies.idProvider = liquidationMaterialDTOAux.id_provider;
                        liquidationMaterialSupplies.subTotal = 0.00M;
                        liquidationMaterialSupplies.subTotalTax = 0.00M;
                        liquidationMaterialSupplies.Total = 0.00M;

                        if (newObject)
                        {
                            db.LiquidationMaterialSupplies.Add(liquidationMaterialSupplies);
                            db.Entry(liquidationMaterialSupplies).State = EntityState.Added;
                        }
                        else
                        {
                            db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                            db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;

                        }
                       
                        db.SaveChanges();

                        if (approved.Value)
                        {
                            var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                            if (aprovedState == null)
                                throw new Exception("No existe el estado de aprobación en el sistema configurelo e intentelo de nuevo.");
                            liquidationMaterialSupplies.Document.id_documentState = aprovedState.id;
                            db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                            db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;
                        }
                       

                        //Details
                        foreach (var detail in liquidationMaterialDTO.LiquidationMaterialDetailDTO.ToList())
                        {
                            var detailAux = db.LiquidationMaterialSuppliesDetail.FirstOrDefault(d => d.id == detail.id);
                            if (detailAux != null && !newObject)
                            {
                                detailAux.aprovedLogist = detail.aprovedLogist;
                                detailAux.aprovedComertial = detail.aprovedLogist;
                                detailAux.descriptionLogist = detail.descriptionLogist ?? "";
                                detailAux.descriptionComertial = "";

                                db.LiquidationMaterialSuppliesDetail.Attach(detailAux);
                                db.Entry(detailAux).State = EntityState.Modified;
                                //newObject = false;
                            }
                            else
                            {
                                detailAux = new LiquidationMaterialSuppliesDetail
                                {
                                    aprovedLogist = detail.aprovedLogist,
                                    aprovedComertial = detail.aprovedLogist,
                                    idItem = detail.id_item,
                                    idMetricUnit = detail.id_metricUnit,
                                    quantity = detail.quantityOrigin,
                                    priceUnit = detail.unitCostOrigin,
                                    subTotal = detail.subTotalOrigin,
                                    tax = detail.iva,
                                    subTotalTax = detail.subTotalIvaOrigin,
                                    total = detail.totalOrigin,
                                    descriptionLogist = detail.descriptionLogist ?? "",
                                    descriptionComertial = ""
                                };
                                liquidationMaterialSupplies.LiquidationMaterialSuppliesDetail.Add(detailAux);
                                
                                //newObject = true;
                            }
                            db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                            db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;
                            db.SaveChanges();

                            if (newObject)
                            {
                                var resultReceptionDispatchMaterialAux = db.ResultReceptionDispatchMaterial.FirstOrDefault(d => d.idReceptionDispatchMaterial == detail.id_guia);
                                if (resultReceptionDispatchMaterialAux != null)
                                {
                                    resultReceptionDispatchMaterialAux.idLiquidationMaterialSupplies = liquidationMaterialSupplies.id;
                                    db.ResultReceptionDispatchMaterial.Attach(resultReceptionDispatchMaterialAux);
                                    db.Entry(resultReceptionDispatchMaterialAux).State = EntityState.Modified;
                                }

                                var resultReceptionDispatchMaterialDetailAux = db.ResultReceptionDispatchMaterialDetail.FirstOrDefault(d => d.id == detail.id_guiaDetail);
                                if (resultReceptionDispatchMaterialDetailAux != null)
                                {
                                    resultReceptionDispatchMaterialDetailAux.idLiquidationMaterialSuppliesDetail = detailAux.id;
                                    db.ResultReceptionDispatchMaterialDetail.Attach(resultReceptionDispatchMaterialDetailAux);
                                    db.Entry(resultReceptionDispatchMaterialDetailAux).State = EntityState.Modified;
                                }
                            }

                            liquidationMaterialSupplies.subTotal += detailAux.aprovedComertial ? detailAux.subTotal : 0.00M;
                            liquidationMaterialSupplies.subTotalTax += detailAux.aprovedComertial ? detailAux.subTotalTax : 0.00M;
                            liquidationMaterialSupplies.Total += detailAux.aprovedComertial ? detailAux.total : 0.00M;

                            db.SaveChanges();
                        }

                        foreach (var LiquidationMaterialDetailDTOAux in liquidationMaterialDTOAux.LiquidationMaterialDetailDTO.Where(w => w.quantity == 0).ToList())
                        {
                            var detailAux = db.LiquidationMaterialSuppliesDetail.FirstOrDefault(d => d.id == LiquidationMaterialDetailDTOAux.id);
                            if (detailAux == null) {
                                detailAux = new LiquidationMaterialSuppliesDetail
                                {
                                    aprovedLogist = LiquidationMaterialDetailDTOAux.aprovedLogist,
                                    aprovedComertial = LiquidationMaterialDetailDTOAux.aprovedLogist,
                                    idItem = LiquidationMaterialDetailDTOAux.id_item,
                                    idMetricUnit = LiquidationMaterialDetailDTOAux.id_metricUnit,
                                    quantity = LiquidationMaterialDetailDTOAux.quantityOrigin,
                                    priceUnit = LiquidationMaterialDetailDTOAux.unitCostOrigin,
                                    subTotal = LiquidationMaterialDetailDTOAux.subTotalOrigin,
                                    tax = LiquidationMaterialDetailDTOAux.iva,
                                    subTotalTax = LiquidationMaterialDetailDTOAux.subTotalIvaOrigin,
                                    total = LiquidationMaterialDetailDTOAux.totalOrigin,
                                    descriptionLogist = LiquidationMaterialDetailDTOAux.descriptionLogist,
                                    descriptionComertial = ""
                                };
                                liquidationMaterialSupplies.LiquidationMaterialSuppliesDetail.Add(detailAux);

                                db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                                db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;
                                db.SaveChanges();

                                var resultReceptionDispatchMaterialAux = db.ResultReceptionDispatchMaterial.FirstOrDefault(d => d.idReceptionDispatchMaterial == LiquidationMaterialDetailDTOAux.id_guia);
                                if (resultReceptionDispatchMaterialAux != null)
                                {
                                    resultReceptionDispatchMaterialAux.idLiquidationMaterialSupplies = liquidationMaterialSupplies.id;
                                    db.ResultReceptionDispatchMaterial.Attach(resultReceptionDispatchMaterialAux);
                                    db.Entry(resultReceptionDispatchMaterialAux).State = EntityState.Modified;
                                }

                                var resultReceptionDispatchMaterialDetailAux = db.ResultReceptionDispatchMaterialDetail.FirstOrDefault(d => d.id == LiquidationMaterialDetailDTOAux.id_guiaDetail);
                                if (resultReceptionDispatchMaterialDetailAux != null)
                                {
                                    resultReceptionDispatchMaterialDetailAux.idLiquidationMaterialSuppliesDetail = detailAux.id;
                                    db.ResultReceptionDispatchMaterialDetail.Attach(resultReceptionDispatchMaterialDetailAux);
                                    db.Entry(resultReceptionDispatchMaterialDetailAux).State = EntityState.Modified;
                                }

                                db.SaveChanges();

                            }

                        }

                        trans.Commit();

                        result.Data = liquidationMaterialSupplies.id.ToString();

                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult Approve(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Data = ApproveLiquidationMaterialSupplies(id).name;
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

        }

        private DocumentState ApproveLiquidationMaterialSupplies(int id_liquidationMaterialSupplies)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(p => p.id == id_liquidationMaterialSupplies);
                    if (liquidationMaterialSupplies != null)
                    {
                        var aprovedState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                        if (aprovedState == null)
                            return liquidationMaterialSupplies.Document.DocumentState;
                        liquidationMaterialSupplies.Document.id_documentState = aprovedState.id;

                        db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                        db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return liquidationMaterialSupplies.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Authorize(int id, List<LiquidationMaterialDetailDTO> listAuthorize = null)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Data = AuthorizeLiquidationMaterialSupplies(id, listAuthorize).name;
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private DocumentState AuthorizeLiquidationMaterialSupplies(int id_liquidationMaterialSupplies, List<LiquidationMaterialDetailDTO> listAuthorize = null)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(p => p.id == id_liquidationMaterialSupplies);
                    if (liquidationMaterialSupplies != null)
                    {
                        if (listAuthorize != null)
                        {
                            liquidationMaterialSupplies.subTotal = 0.00M;
                            liquidationMaterialSupplies.subTotalTax = 0.00M;
                            liquidationMaterialSupplies.Total = 0.00M;

                            //Details
                            foreach (var detail in listAuthorize.ToList())
                            {
                                var detailAux = db.LiquidationMaterialSuppliesDetail.FirstOrDefault(d => d.id == detail.id);
                                if (detailAux != null)
                                {
                                    detailAux.aprovedComertial = detail.aprovedComertial;
                                    detailAux.descriptionComertial = detail.descriptionComertial ?? "";

                                    db.LiquidationMaterialSuppliesDetail.Attach(detailAux);
                                    db.Entry(detailAux).State = EntityState.Modified;
                                }

                                db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                                db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;
                                //db.SaveChanges();

                                liquidationMaterialSupplies.subTotal += detailAux.aprovedComertial ? detailAux.subTotal : 0.00M;
                                liquidationMaterialSupplies.subTotalTax += detailAux.aprovedComertial ? detailAux.subTotalTax : 0.00M;
                                liquidationMaterialSupplies.Total += detailAux.aprovedComertial ? detailAux.total : 0.00M;

                                db.SaveChanges();
                            }
                        }
                        var authorizeState = db.DocumentState.FirstOrDefault(d => d.code.Equals("06"));
                        if (authorizeState == null)
                            return

                        liquidationMaterialSupplies.Document.DocumentState;
                        liquidationMaterialSupplies.Document.id_documentState = authorizeState.id;
                        liquidationMaterialSupplies.Document.authorizationDate = DateTime.Now;

                        db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                        db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return liquidationMaterialSupplies.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Reverse(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Data = ReverseLiquidationMaterialSupplies(id).name;
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private DocumentState ReverseLiquidationMaterialSupplies(int id_liquidationMaterialSupplies)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(p => p.id == id_liquidationMaterialSupplies);
                    if (liquidationMaterialSupplies != null)
                    {
                        DocumentState reverseState = null;
                        if (liquidationMaterialSupplies.Document.DocumentState.code.Equals("03"))
                        {
                            reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("01"));
                        }
                        else
                        {
                            if (liquidationMaterialSupplies.Document.DocumentState.code.Equals("06"))
                            {
                                reverseState = db.DocumentState.FirstOrDefault(d => d.code.Equals("03"));
                            }
                        }
                        if (reverseState == null)
                            return

                        liquidationMaterialSupplies.Document.DocumentState;
                        liquidationMaterialSupplies.Document.id_documentState = reverseState.id;
                        liquidationMaterialSupplies.Document.authorizationDate = DateTime.Now;

                        db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                        db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return liquidationMaterialSupplies.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Annul(int id)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = new ApiResult();

                    try
                    {
                        result.Data = AnnulLiquidationMaterialSupplies(id).name;
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private DocumentState AnnulLiquidationMaterialSupplies(int id_liquidationMaterialSupplies)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(p => p.id == id_liquidationMaterialSupplies);
                    if (liquidationMaterialSupplies != null)
                    {
                        var annulState = db.DocumentState.FirstOrDefault(d => d.code.Equals("05"));
                        if (annulState == null)
                            return

                        liquidationMaterialSupplies.Document.DocumentState;
                        liquidationMaterialSupplies.Document.id_documentState = annulState.id;
                        liquidationMaterialSupplies.Document.authorizationDate = DateTime.Now;

                        db.LiquidationMaterialSupplies.Attach(liquidationMaterialSupplies);
                        db.Entry(liquidationMaterialSupplies).State = EntityState.Modified;


                        var resultReceptionDispatchMaterialAux = db.ResultReceptionDispatchMaterial.Where(d => d.idLiquidationMaterialSupplies == liquidationMaterialSupplies.id);
                        if (resultReceptionDispatchMaterialAux != null && resultReceptionDispatchMaterialAux.Any())
                        {
                            foreach (var item in resultReceptionDispatchMaterialAux)
                            {
                                item.idLiquidationMaterialSupplies = null;
                                db.ResultReceptionDispatchMaterial.Attach(item);
                                db.Entry(item).State = EntityState.Modified;
                                foreach (var detail in item.ResultReceptionDispatchMaterialDetail)
                                {
                                    detail.idLiquidationMaterialSuppliesDetail = null;
                                    db.ResultReceptionDispatchMaterialDetail.Attach(detail);
                                    db.Entry(detail).State = EntityState.Modified;

                                }
                            }

                        }

                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto seleccionado");
                    }

                    return liquidationMaterialSupplies.Document.DocumentState;
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id)
        {
            var index = GetLiquidationMaterialResultConsultDTO().OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id);

            var result = new
            {
                maximunPages = GetLiquidationMaterialResultConsultDTO().Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            var element = GetLiquidationMaterialResultConsultDTO().OrderByDescending(p => p.id).Take(page).Last();
            var liquidationMaterialSupplies = db.LiquidationMaterialSupplies.FirstOrDefault(d => d.id == element.id);
            if (liquidationMaterialSupplies == null)
                return PartialView("Edit", new LiquidationMaterialDTO());

            var model = ConvertToDto(liquidationMaterialSupplies);
            SetLiquidationMaterialDTO(model);
            BuilViewBag(false);
            return PartialView("Edit", model);
        }

        [HttpPost]
        public JsonResult UpdateAndRefreshGridSummary(int id, bool aproved, bool canAproved, string description)
        {
            var result = new ApiResult();

            try
            {
                var liquidationMaterialDTOAux = GetLiquidationMaterialDTO();
                var detailAux = liquidationMaterialDTOAux.LiquidationMaterialDetailDTO.FirstOrDefault(d => d.id == id);
                if (detailAux != null)
                {
                    detailAux.quantity = aproved ? detailAux.quantityOrigin : 0.00M;
                    detailAux.unitCost = aproved ? detailAux.unitCostOrigin : 0.00M;
                    detailAux.subTotal = aproved ? detailAux.subTotalOrigin : 0.00M;
                    detailAux.subTotalIva = aproved ? detailAux.subTotalIvaOrigin : 0.00M;
                    detailAux.total = aproved ? detailAux.totalOrigin : 0.00M;
                    detailAux.aprovedLogist = canAproved ? aproved : detailAux.aprovedLogist;
                    detailAux.aprovedComertial = !canAproved ? aproved : detailAux.aprovedComertial;

                    if (canAproved) detailAux.descriptionLogist = description;
                    else detailAux.descriptionComertial = description;

                    liquidationMaterialDTOAux.SummaryLiquidationMaterialDetailDTO = GetSummary(liquidationMaterialDTOAux.LiquidationMaterialDetailDTO);

                    SetLiquidationMaterialDTO(liquidationMaterialDTOAux);
                }
            }
            catch (Exception e)
            {
                result.Code = e.HResult;
                result.Message = e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
