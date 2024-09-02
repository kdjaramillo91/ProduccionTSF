using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;

namespace DXPANACEASOFT.Controllers
{
    public class PriceListController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region PRICELIST FORM EDITOR

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListEditForm(System.Int32 id)
        {
            PriceList priceList = db.PriceList.FirstOrDefault(p => p.id == id);

            if (priceList == null)
            {
                //DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("17"));
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");

                //Employee employee = ActiveUser.Employee;

                priceList = new PriceList
                {
                    Document = new Document
                    {
                        id = 0,
                        //id_documentType = documentType?.id ?? 0,
                        //DocumentType = documentType,
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        emissionDate = DateTime.Now
                    },

                    //startDate = null,
                    //endDate = null,

                    //isForPurchase = true,
                    //isForSold = true,
                    //isActive = true,
                    list_idInventaryLineFilter = JsonConvert.SerializeObject(new List<int>()),
                    list_idItemTypeFilter = JsonConvert.SerializeObject(new List<int>()),
                    list_idItemGroupFilter = JsonConvert.SerializeObject(new List<int>()),
                    list_filterShow = JsonConvert.SerializeObject(new List<int>()),
                    PriceListDetail = new List<PriceListDetail>(),
                    PriceListDetailFilterShow = new List<PriceListDetailFilterShow>()
                };
                UpdatePriceListDetailFilter(priceList);
                UpdatePriceListDetailFilterShow(priceList);
                
            }

            UpdatePriceListDetailCategoryAdjustment(priceList);

            TempData["priceList"] = priceList;
            TempData.Keep("priceList");

            return PartialView("_PriceListEditForm", priceList);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListCopy(System.Int32 id)
        {
            var priceList = db.PriceList.FirstOrDefault(p => p.id == id);

            PriceList priceListCopy = null;
            if (priceList != null)
            {
                DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == priceList.Document.id_documentType);
                DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");
                priceListCopy = new PriceList
                {
                    Document = new Document
                    {
                        id = 0,
                        id_documentType = documentType?.id ?? 0,
                        DocumentType = documentType,
                        id_documentState = documentState?.id ?? 0,
                        DocumentState = documentState,
                        emissionDate = DateTime.Now
                    },
                    //id_priceListType = priceList.id_priceListType,
                    name = priceList.name + "-Copia",
                    startDate = priceList.startDate,
                    endDate = priceList.endDate,
                    isForPurchase = priceList.isForPurchase,
                    isForSold = priceList.isForSold,
                    isQuotation = priceList.isQuotation,
                    list_idInventaryLineFilter = priceList.list_idInventaryLineFilter,
                    list_idItemTypeFilter = priceList.list_idItemTypeFilter,
                    list_idItemGroupFilter = priceList.list_idItemGroupFilter,
                    list_filterShow = priceList.list_filterShow,
                    PriceListDetail = new List<PriceListDetail>(),
                    PriceListDetailFilterShow = new List<PriceListDetailFilterShow>()
                };

                foreach (var detail in priceList.PriceListDetail)
                {
                    priceListCopy.PriceListDetail.Add(new PriceListDetail
                    {
                        id_item = detail.id_item,
                        Item = detail.Item,
                        id_metricUnit = detail.id_metricUnit,
                        MetricUnit = detail.MetricUnit,
                        purchasePrice = detail.purchasePrice,
                        salePrice = detail.salePrice,
                        specialPrice = detail.specialPrice
                    });
                }

                foreach (var detail in priceList.PriceListDetailFilterShow)
                {
                    priceListCopy.PriceListDetailFilterShow.Add(new PriceListDetailFilterShow
                    {
                        id_item = detail.id_item,
                        Item = detail.Item,
                        id_metricUnit = detail.id_metricUnit,
                        MetricUnit = detail.MetricUnit,
                        purchasePrice = detail.purchasePrice,
                        salePrice = detail.salePrice,
                        specialPrice = detail.specialPrice
                    });
                }
            }

            TempData["priceList"] = priceListCopy;
            TempData.Keep("priceList");

            return PartialView("_PriceListEditForm", priceListCopy);
        }


        #endregion

        #region PRICELIST HEADER

        [ValidateInput(false)]
        public ActionResult PriceListPartial()
        {
            var model = db.PriceList;
            return PartialView("_PriceListPartial", model.OrderByDescending(o => o.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListPartialAddNew(bool approve, PriceList item, Document itemDoc,
                                                   int? id_priceListBasePurchase, int? id_priceListBaseSale,
                                                   int? id_groupPersonByRolProvider, int? id_groupPersonByRolCustomer,
                                                   bool? byGroupProvider, bool? byGroupCustomer)
        {
            PriceList tempPriceList = (TempData["priceList"] as PriceList);
            tempPriceList = tempPriceList ?? new PriceList();


            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Document

                        item.Document = item.Document ?? new Document();
                        item.Document.id_userCreate = ActiveUser.id;
                        item.Document.dateCreate = DateTime.Now;
                        item.Document.id_userUpdate = ActiveUser.id;
                        item.Document.dateUpdate = DateTime.Now;

                        DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.id == itemDoc.id_documentType);
                        if (documentType == null)
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la lista de Precio porque no existe el Tipo de Documento, configúrelo e inténtelo de nuevo");
                            return PartialView("_PriceListEditForm", item);

                        }
                        item.Document.id_documentType = documentType.id;
                        item.Document.DocumentType = documentType;
                        item.Document.sequential = GetDocumentSequential(item.Document.id_documentType);
                        item.Document.number = GetDocumentNumber(item.Document.id_documentType);


                        DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");
                        if (documentState == null)
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar la planificación porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                            return PartialView("_PriceListEditForm", item);

                        }
                        item.Document.id_documentState = documentState.id;
                        item.Document.DocumentState = documentState;

                        item.Document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
                        item.Document.id_emissionPoint = ActiveEmissionPoint.id;

                        item.Document.emissionDate = itemDoc.emissionDate;
                        item.Document.description = itemDoc.description;

                        //Actualiza Secuencial
                        if (documentType != null)
                        {
                            documentType.currentNumber = documentType.currentNumber + 1;
                            db.DocumentType.Attach(documentType);
                            db.Entry(documentType).State = EntityState.Modified;
                        }

                        #endregion

                        #region PriceList

                        item.id_company = this.ActiveCompanyId;
                        item.isForPurchase = documentType.code == "18" || documentType.code == "19";
                        item.isForSold = documentType.code == "20" || documentType.code == "21";
                        item.isQuotation = documentType.code == "19" || documentType.code == "21";

                        
                        if (item.isForPurchase && item.isQuotation)
                        {
                            //item.id_provider = item.id_provider;
                            item.byGroup = byGroupProvider;
                            item.id_groupPersonByRol = id_groupPersonByRolProvider;
                            item.id_customer = null;
                            item.Provider = db.Provider.FirstOrDefault(fod => fod.id == item.id_provider);
                            item.GroupPersonByRol = db.GroupPersonByRol.FirstOrDefault(fod => fod.id == item.id_groupPersonByRol);
                            item.Customer = null;

                            item.id_calendarPriceList = tempPriceList.id_calendarPriceList;
                            item.CalendarPriceList = db.CalendarPriceList.FirstOrDefault(fod => fod.id == tempPriceList.id_calendarPriceList);

                            item.id_priceListBase = id_priceListBasePurchase.Value;
                            item.PriceList2 = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListBasePurchase);
                        }
                        else
                        {
                            if (item.isForSold && item.isQuotation)
                            {
                                item.id_provider = null;
                                item.byGroup = byGroupCustomer;
                                item.id_groupPersonByRol = id_groupPersonByRolCustomer;
                                //item.id_customer = item.id_customer;
                                item.Provider = null;
                                item.Customer = db.Customer.FirstOrDefault(fod => fod.id == item.id_customer);
                                item.GroupPersonByRol = db.GroupPersonByRol.FirstOrDefault(fod => fod.id == item.id_groupPersonByRol);

                                item.id_calendarPriceList = tempPriceList.id_calendarPriceList;
                                item.CalendarPriceList = db.CalendarPriceList.FirstOrDefault(fod => fod.id == tempPriceList.id_calendarPriceList);

                                item.id_priceListBase = id_priceListBaseSale.Value;
                                item.PriceList2 = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListBaseSale);
                            }
                            else
                            {
                                item.CalendarPriceList = db.CalendarPriceList.FirstOrDefault(fod=> fod.id == item.id_calendarPriceList) ?? db.CalendarPriceList.FirstOrDefault(fod => fod.id == tempPriceList.id_calendarPriceList);
                                item.id_provider = null;
                                item.id_customer = null;
                                item.Provider = null;
                                item.Customer = null;
                                item.id_priceListBase = null;
                                item.PriceList2 = null;
                            }
                        }

                        item.startDate = item.CalendarPriceList.startDate;
                        item.endDate = item.CalendarPriceList.endDate;

                        item.list_idInventaryLineFilter = tempPriceList.list_idInventaryLineFilter;
                        item.list_idItemTypeFilter = tempPriceList.list_idItemTypeFilter;
                        item.list_idItemGroupFilter = tempPriceList.list_idItemGroupFilter;

                        bool priceListGeneralRepeated = RepeatedPriceListGeneral(item);
                        if (priceListGeneralRepeated)
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Lista: " + item.Document.DocumentType.name + " con los mismos detalle, con el mismo tipo de Documento y mismo Calendario que otra ya existente, verifique el caso e intente de nuevo");
                            return PartialView("_PriceListEditForm", item);
                        }

                        item.list_filterShow = tempPriceList.list_filterShow;

                        #endregion

                        #region Details
                        decimal purchasePriceAux = 0;
                        decimal salePriceAux = 0;

                        if (tempPriceList.PriceListDetail != null)
                        {
                            foreach (var detail in tempPriceList.PriceListDetail)
                            {
                                item.PriceListDetail.Add(new PriceListDetail
                                {
                                    id_item = detail.id_item,
                                    Item = db.Item.FirstOrDefault(e => e.id == detail.id_item),
                                    id_metricUnit = detail.id_metricUnit,
                                    MetricUnit = db.MetricUnit.FirstOrDefault(e => e.id == detail.id_metricUnit),
                                    purchasePrice = detail.purchasePrice,
                                    salePrice = detail.salePrice,
                                    specialPrice = detail.specialPrice
                                });
                                purchasePriceAux += detail.purchasePrice;
                                salePriceAux += detail.salePrice;
                            }
                        }

                        if (item.PriceListDetail.Count == 0)
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Lista de Precio sin detalles");
                            return PartialView("_PriceListEditForm", item);
                        }

                        if ((item.isForPurchase && purchasePriceAux == 0))
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Lista de Precio sin precio de Compra en alguno de sus detalles");
                            return PartialView("_PriceListEditForm", item);
                        }
                        if ((item.isForSold && salePriceAux == 0))
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Lista de Precio sin precio de Venta en alguno de sus detalles");
                            return PartialView("_PriceListEditForm", item);
                        }

                        if (tempPriceList.PriceListDetailFilterShow != null)
                        {
                            foreach (var detail in tempPriceList.PriceListDetailFilterShow)
                            {
                                item.PriceListDetailFilterShow.Add(new PriceListDetailFilterShow
                                {
                                    id_item = detail.id_item,
                                    Item = db.Item.FirstOrDefault(e => e.id == detail.id_item),
                                    id_metricUnit = detail.id_metricUnit,
                                    MetricUnit = db.MetricUnit.FirstOrDefault(e => e.id == detail.id_metricUnit),
                                    purchasePrice = detail.purchasePrice,
                                    salePrice = detail.salePrice,
                                    specialPrice = detail.specialPrice
                                });
                            }

                        }

                        if (tempPriceList.PriceListDetailCategoryAdjustment != null)
                        {
                            foreach (var detail in tempPriceList.PriceListDetailCategoryAdjustment)
                            {
                                item.PriceListDetailCategoryAdjustment.Add(new PriceListDetailCategoryAdjustment
                                {
                                    id_itemGroupCategory = detail.id_itemGroupCategory,
                                    ItemGroupCategory = db.ItemGroupCategory.FirstOrDefault(e => e.id == detail.id_itemGroupCategory),
                                    adjustment = detail.adjustment
                                });
                            }

                        }

                        #endregion

                        if (approve)
                        {
                            item.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        }

                        db.PriceList.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        TempData["priceList"] = item;
                        TempData.Keep("priceList");

                        ViewData["EditMessage"] = SuccessMessage("Lista de Precios: " + item.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("priceList");
                        ViewData["EditMessage"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            return PartialView("_PriceListEditForm", item);
            //return PartialView("Index");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListPartialUpdate(bool approve, PriceList item, Document itemDoc, 
                                                   int? id_priceListBasePurchase, int? id_priceListBaseSale,
                                                   int? id_groupPersonByRolProvider, int? id_groupPersonByRolCustomer,
                                                   bool? byGroupProvider, bool? byGroupCustomer)
        {
            PriceList tempPriceList = (TempData["priceList"] as PriceList);
            tempPriceList = tempPriceList ?? new PriceList();

            var modelItem = db.PriceList.FirstOrDefault(p => p.id == item.id);

            

            if (ModelState.IsValid && modelItem != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region DOCUMENT

                        modelItem.Document.emissionDate = itemDoc.emissionDate;
                        modelItem.Document.description = itemDoc.description;
                        modelItem.Document.id_userUpdate = ActiveUser.id;
                        modelItem.Document.dateUpdate = DateTime.Now;

                        #endregion

                        #region PriceList

                        modelItem.name = item.name;
                        modelItem.id_calendarPriceList = tempPriceList.id_calendarPriceList;
                        modelItem.CalendarPriceList = db.CalendarPriceList.FirstOrDefault(fod => fod.id == item.id_calendarPriceList);
                        if (modelItem.isForPurchase && modelItem.isQuotation)
                        {
                            modelItem.byGroup = byGroupProvider;
                            modelItem.id_groupPersonByRol = id_groupPersonByRolProvider;
                            modelItem.id_provider = item.id_provider;
                            modelItem.id_customer = null;
                            modelItem.Provider = db.Provider.FirstOrDefault(fod => fod.id == item.id_provider);
                            modelItem.GroupPersonByRol = db.GroupPersonByRol.FirstOrDefault(fod => fod.id == item.id_groupPersonByRol);
                            modelItem.Customer = null;
                            
                            modelItem.id_priceListBase = id_priceListBasePurchase.Value;
                            item.PriceList2 = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListBasePurchase);
                        }
                        else
                        {
                            if (modelItem.isForSold && modelItem.isQuotation)
                            {
                                modelItem.byGroup = byGroupCustomer;
                                modelItem.id_groupPersonByRol = id_groupPersonByRolCustomer;

                                modelItem.id_provider = null;
                                modelItem.id_customer = item.id_customer;

                                modelItem.Provider = null;
                                modelItem.Customer = db.Customer.FirstOrDefault(fod => fod.id == item.id_customer);
                                modelItem.GroupPersonByRol = db.GroupPersonByRol.FirstOrDefault(fod => fod.id == item.id_groupPersonByRol);

                                modelItem.id_priceListBase = id_priceListBaseSale.Value;
                                item.PriceList2 = db.PriceList.FirstOrDefault(fod => fod.id == id_priceListBaseSale);
                            }
                            else
                            {
                                modelItem.id_provider = null;
                                modelItem.id_customer = null;
                                modelItem.Provider = null;
                                modelItem.Customer = null;
                                modelItem.id_priceListBase = null;
                                item.PriceList2 = null;
                            }
                        }

                        modelItem.startDate = modelItem.CalendarPriceList.startDate;
                        modelItem.endDate = modelItem.CalendarPriceList.endDate;

                        modelItem.list_idInventaryLineFilter = tempPriceList.list_idInventaryLineFilter;
                        modelItem.list_idItemTypeFilter = tempPriceList.list_idItemTypeFilter;
                        modelItem.list_idItemGroupFilter = tempPriceList.list_idItemGroupFilter;

                        bool priceListGeneralRepeated = RepeatedPriceListGeneral(modelItem);
                        if (priceListGeneralRepeated)
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Lista: " + modelItem.Document.DocumentType.name + " con los mismos detalle, con el mismo tipo de Documento y mismo Calendario que otra ya existente, verifique el caso e intente de nuevo");
                            return PartialView("_PriceListEditForm", item);
                        }

                        modelItem.list_filterShow = tempPriceList.list_filterShow;


                        #endregion


                        #region Detail

                        for (int i = modelItem.PriceListDetail.Count - 1; i >= 0; i--)
                        {
                            var detail = modelItem.PriceListDetail.ElementAt(i);

                            modelItem.PriceListDetail.Remove(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        decimal purchasePriceAux = 0;
                        decimal salePriceAux = 0;

                        if (tempPriceList.PriceListDetail != null)
                        {
                            foreach (var detail in tempPriceList.PriceListDetail)
                            {
                                modelItem.PriceListDetail.Add(new PriceListDetail
                                {
                                    id_priceList = modelItem.id,
                                    PriceList = modelItem,
                                    id_item = detail.id_item,
                                    Item = db.Item.FirstOrDefault(p => p.id == detail.id_item),
                                    id_metricUnit = detail.id_metricUnit,
                                    MetricUnit = db.MetricUnit.FirstOrDefault(p => p.id == detail.id_metricUnit),
                                    purchasePrice = detail.purchasePrice,
                                    salePrice = detail.salePrice,
                                    specialPrice = detail.specialPrice
                                });
                                purchasePriceAux += detail.purchasePrice;
                                salePriceAux += detail.salePrice;
                            }
                        }

                        if (modelItem.PriceListDetail.Count == 0)
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Lista de Precio sin detalles");
                            return PartialView("_PriceListEditForm", item);
                        }

                        if ((modelItem.isForPurchase && purchasePriceAux == 0))
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Lista de Precio sin precio de Compra en alguno de sus detalles");
                            return PartialView("_PriceListEditForm", item);
                        }

                        if ((modelItem.isForSold && salePriceAux == 0))
                        {
                            TempData.Keep("priceList");
                            ViewData["EditMessage"] = ErrorMessage("No se puede guardar una Lista de Precio sin precio de Venta en alguno de sus detalles");
                            return PartialView("_PriceListEditForm", item);
                        }

                        for (int i = modelItem.PriceListDetailFilterShow.Count - 1; i >= 0; i--)
                        {
                            var detail = modelItem.PriceListDetailFilterShow.ElementAt(i);

                            modelItem.PriceListDetailFilterShow.Remove(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        if (tempPriceList.PriceListDetailFilterShow != null)
                        {
                            foreach (var detail in tempPriceList.PriceListDetailFilterShow)
                            {
                                modelItem.PriceListDetailFilterShow.Add(new PriceListDetailFilterShow
                                {
                                    id_priceList = modelItem.id,
                                    PriceList = modelItem,
                                    id_item = detail.id_item,
                                    Item = db.Item.FirstOrDefault(p => p.id == detail.id_item),
                                    id_metricUnit = detail.id_metricUnit,
                                    MetricUnit = db.MetricUnit.FirstOrDefault(p => p.id == detail.id_metricUnit),
                                    purchasePrice = detail.purchasePrice,
                                    salePrice = detail.salePrice,
                                    specialPrice = detail.specialPrice
                                });
                            }

                        }

                        for (int i = modelItem.PriceListDetailCategoryAdjustment.Count - 1; i >= 0; i--)
                        {
                            var detail = modelItem.PriceListDetailCategoryAdjustment.ElementAt(i);

                            modelItem.PriceListDetailCategoryAdjustment.Remove(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        if (tempPriceList.PriceListDetailCategoryAdjustment != null)
                        {
                            foreach (var detail in tempPriceList.PriceListDetailCategoryAdjustment)
                            {
                                modelItem.PriceListDetailCategoryAdjustment.Add(new PriceListDetailCategoryAdjustment
                                {
                                    id_priceList = modelItem.id,
                                    PriceList = modelItem,
                                    id_itemGroupCategory = detail.id_itemGroupCategory,
                                    ItemGroupCategory = db.ItemGroupCategory.FirstOrDefault(e => e.id == detail.id_itemGroupCategory),
                                    adjustment = detail.adjustment
                                });
                            }

                        }

                        #endregion

                        if (approve)
                        {
                            modelItem.Document.DocumentState = db.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
                        }

                        db.PriceList.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        TempData["priceList"] = modelItem;
                        TempData.Keep("priceList");

                        ViewData["EditMessage"] = SuccessMessage("Lista de Precios: " + modelItem.Document.number + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("priceList");
                        ViewData["EditMessage"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor , corrija todos los errores.";

            return PartialView("_PriceListEditForm", modelItem);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListPartialDelete(System.Int32 id)
        {
            var model = db.PriceList;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)
                    {
                        //item.isActive = false;

                        //item.id_userUpdate = ActiveUser.id;
                        //item.dateUpdate = DateTime.Now;
                        
                        db.PriceList.Attach(item);
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    ViewData["EditMessage"] = SuccessMessage("Lista de Precios: " + item.name + " desactivada");
                }
                catch (Exception)
                {
                    ViewData["EditMessage"] = ErrorMessage();
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }
            return PartialView("_PriceListPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedPriceLists(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var priceLists = db.PriceList.Where(i => ids.Contains(i.id));
                        foreach (var list in priceLists)
                        {
                            //list.isActive = false;
                            
                            //list.id_userUpdate = ActiveUser.id;
                            //list.dateUpdate = DateTime.Now;

                            db.Entry(list).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Listas de Precios desactivadas");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.PriceList;
            return PartialView("_PriceListPartial", model.ToList());
        }

        #endregion

        #region DETAILS

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListDetailsPartial()
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            TempData.Keep("priceList");

            var model = priceList.PriceListDetailFilterShow?.ToList() ?? new List<PriceListDetailFilterShow>();
            UpdateViewDataShow();
            return PartialView("_PriceListDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListDetailsPartialAddNew(DXPANACEASOFT.Models.PriceListDetail item)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            priceList.PriceListDetail = priceList.PriceListDetail ?? new List<PriceListDetail>();

            if (ModelState.IsValid)
            {
                try
                {
                    priceList.PriceListDetail.Add(new PriceListDetail
                    {
                        id_item = item.id_item,
                        purchasePrice = item.purchasePrice,
                        salePrice = item.salePrice,
                        //isActive = item.isActive,

                        //id_userCreate = ActiveUser.id,
                        //dateCreate = DateTime.Now,
                        //id_userUpdate = ActiveUser.id,
                        //dateUpdate = DateTime.Now
                    });

                    TempData["priceList"] = priceList;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("priceList");

            var model = priceList?.PriceListDetail.ToList() ?? new List<PriceListDetail>();
            return PartialView("_PriceListDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListDetailsPartialUpdate(DXPANACEASOFT.Models.PriceListDetailFilterShow item)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            priceList.PriceListDetailFilterShow = priceList.PriceListDetailFilterShow ?? new List<PriceListDetailFilterShow>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = priceList.PriceListDetailFilterShow.FirstOrDefault(it => it.id_item == item.id_item);
                    if (modelItem != null)
                    {
                        //modelItem.id_userUpdate = ActiveUser.id;
                        //modelItem.dateCreate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        

                        UpdateModelPriceListDetail(priceList, modelItem);
                        TempData["priceList"] = priceList;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("priceList");

            var model = priceList?.PriceListDetailFilterShow.ToList() ?? new List<PriceListDetailFilterShow>();
            UpdateViewDataShow();
            return PartialView("_PriceListDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListDetailsPartialDelete(System.Int32 id_item)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            priceList.PriceListDetail = priceList.PriceListDetail ?? new List<PriceListDetail>();

            if (id_item >= 0)
            {
                try
                {
                    var item = priceList.PriceListDetail.FirstOrDefault(it => it.id_item == id_item);
                    if (item != null)
                    {
                        //item.isActive = false;
                        //item.id_userUpdate = ActiveUser.id;
                        //item.dateCreate = DateTime.Now;
                    }
                    TempData["priceList"] = priceList;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("priceList");

            var model = priceList?.PriceListDetail.ToList() ?? new List<PriceListDetail>();
            return PartialView("_PriceListDetailsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void PriceListDetailsDeleteSeleted(int[] ids)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            priceList.PriceListDetail = priceList.PriceListDetail ?? new List<PriceListDetail>();

            if (ids != null)
            {
                try
                {
                    var priceListDetail = priceList.PriceListDetail.Where(i => ids.Contains(i.id_item));

                    foreach (var detail in priceListDetail)
                    {
                        //detail.isActive = false;
                        //detail.id_userUpdate = ActiveUser.id;
                        //detail.dateUpdate = DateTime.Now;
                    }

                    TempData["priceList"] = priceList;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("priceList");
        }

        public ActionResult PriceListDetailFilterShowPartial(int? id_priceList)
        {
            ViewData["id_priceList"] =  id_priceList ; 
            var priceList = db.PriceList.FirstOrDefault(p => p.id == id_priceList);
            var model = priceList?.PriceListDetailFilterShow.ToList() ?? new List<PriceListDetailFilterShow>();

            TempData["priceList"] = priceList ?? new PriceList();
            TempData.Keep("priceList");
            UpdateViewDataShow();
            return PartialView("_PriceListDetailViewPartialPriceListDetails", model);
        }

        #endregion

        #region DETAILS_PriceListDetailCategoryAdjustment

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListDetailCategoryAdjustmentsPartial()
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            TempData.Keep("priceList");

            var model = priceList.PriceListDetailCategoryAdjustment?.ToList() ?? new List<PriceListDetailCategoryAdjustment>();
            return PartialView("_PriceListDetailCategoryAdjustmentDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListDetailCategoryAdjustmentsPartialUpdate(DXPANACEASOFT.Models.PriceListDetailCategoryAdjustment item)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            priceList.PriceListDetailCategoryAdjustment = priceList.PriceListDetailCategoryAdjustment ?? new List<PriceListDetailCategoryAdjustment>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = priceList.PriceListDetailCategoryAdjustment.FirstOrDefault(it => it.id == item.id);
                    if (modelItem != null)
                    {
                        //modelItem.id_userUpdate = ActiveUser.id;
                        //modelItem.dateCreate = DateTime.Now;

                        this.UpdateModel(modelItem);


                        TempData["priceList"] = priceList;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija los errores.";

            TempData.Keep("priceList");

            var model = priceList?.PriceListDetailCategoryAdjustment.ToList() ?? new List<PriceListDetailCategoryAdjustment>();
            return PartialView("_PriceListDetailCategoryAdjustmentDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListDetailViewCategoryAdjustmentsPartial(int? id_priceList)
        {
            ViewData["id_priceList"] = id_priceList;
            var priceList = db.PriceList.FirstOrDefault(p => p.id == id_priceList);
            var model = priceList?.PriceListDetailFilterShow.ToList() ?? new List<PriceListDetailFilterShow>();

            TempData["priceList"] = priceList ?? new PriceList();
            TempData.Keep("priceList");

            return PartialView("_PriceListDetailCategoryAdjustmentViewPartialPriceListDetails", model);
        }

        #endregion

        #region SINGLE CHANGE PRICE LIST STATE

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            PriceList priceList = db.PriceList.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var existDetail = priceList.PriceList1.Any() || priceList.ProductionLot.Any() || priceList.PurchaseOrder.Any() ||
                                      priceList.SalesOrder.Any() || priceList.SalesQuotation.Any() || priceList.SalesRequest.Any();

                    if (existDetail)
                    {
                        TempData.Keep("priceList");
                        ViewData["EditMessage"] = ErrorMessage("No se puede anular la lista de Precio pues esta siendo usada en el sistema");
                        return PartialView("_PriceListMainForm", priceList);
                        //return PartialView("_PriceListEditForm", priceList);
                    }
                    
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");//ANULADA

                    if (priceList != null && documentState != null)
                    {
                        priceList.Document.id_documentState = documentState.id;
                        priceList.Document.DocumentState = documentState;

                        db.PriceList.Attach(priceList);
                        db.Entry(priceList).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["priceList"] = priceList;
            TempData.Keep("priceList");
            ViewData["EditMessage"] = SuccessMessage("Lista de Precio: " + priceList.Document.number + " anulado exitosamente");

            //return PartialView("_PriceListEditForm", priceList);
            return PartialView("_PriceListMainForm", priceList);
        }

        [HttpPost]
        public ActionResult Revert(int id)
        {
            PriceList priceList = db.PriceList.FirstOrDefault(r => r.id == id);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE DE RECEPCION

                    if (priceList != null && documentState != null)
                    {
                        priceList.Document.id_documentState = documentState.id;
                        priceList.Document.DocumentState = documentState;

                        db.PriceList.Attach(priceList);
                        db.Entry(priceList).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            TempData["priceList"] = priceList;
            TempData.Keep("priceList");
            ViewData["EditMessage"] = SuccessMessage("Lista de Precio: " + priceList.Document.number + " reversado exitosamente");

            //return PartialView("_PriceListEditForm", priceList);
            return PartialView("_PriceListMainForm", priceList);
        }

        #endregion

        #region REPORTS

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListReport()
        {
            PriceListReport report = new PriceListReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_PriceListReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PriceListDetailReport(int id)
        {
            PriceListDetailReport report = new PriceListDetailReport();
            report.Parameters["id_priceList"].Value = id;
            return PartialView("_PriceListDetailReport", report);
        }

        #endregion

        #region ACTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult Actions(int id)
        {
            var actions = new
            {
                btnApprove = true,
                btnAutorize = false,
                btnProtect = false,
                btnCancel = false,
                btnRevert = false,
            };

            if (id == 0)
            {
                return Json(actions, JsonRequestBehavior.AllowGet);
            }

            PriceList priceList = db.PriceList.FirstOrDefault(r => r.id == id);
            string code_state = priceList.Document.DocumentState.code;

            if (code_state == "01") // PENDIENTE
            {
                actions = new
                {
                    btnApprove = true,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = false,
                };
            }
            else if (code_state == "03") // APROBADA
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = true,
                    btnRevert = true,
                };
            }
            else if (code_state == "05") // ANULADA 
            {
                actions = new
                {
                    btnApprove = false,
                    btnAutorize = false,
                    btnProtect = false,
                    btnCancel = false,
                    btnRevert = false,
                };
            }

            return Json(actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_priceList)
        {
            TempData.Keep("priceList");

            int index = db.PriceList.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_priceList);

            var result = new
            {
                maximunPages = db.PriceList.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            PriceList priceList = db.PriceList.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (priceList != null)
            {
                TempData["priceList"] = priceList;
                TempData.Keep("priceList");
                return PartialView("_PriceListMainForm", priceList);
                //return PartialView("_PriceListEditForm", priceList);

            }

            TempData.Keep("priceList");

            //return PartialView("_PriceListMainForm", new PriceList());
            return PartialView("_PriceListEditForm", new PriceList());

        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemDetailData(int id_item)
        {
            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                masterCode = item.masterCode,
                purchasePrice = item.ItemPurchaseInformation?.purchasePrice ?? 0.0M,
                salePrice = item.ItemPurchaseInformation?.purchasePrice ?? 0.0M
            };

            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PriceListDetails()
        {
            PriceList priceList = (TempData["priceList"] as PriceList);

            priceList = priceList ?? new PriceList();
            priceList.PriceListDetail = priceList.PriceListDetail ?? new List<PriceListDetail>();

            TempData.Keep("priceList");

            return Json(priceList.PriceListDetail.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetCodeDocumentType(int? id_documentType)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(i => i.id == id_documentType);

            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            priceList.Document.id_documentType = id_documentType ?? 0;
            priceList.Document.DocumentType = documentType;
            var codeDocumentType = documentType?.code ?? "";

            var codeAux = codeDocumentType == "19" ? "18" : (codeDocumentType == "21" ? "20" : "");
            var priceListAux = db.PriceList.Where(t => (t.Document.DocumentType.code.Equals(codeAux)) &&
                                                        t.id_company == this.ActiveCompanyId && t.Document.DocumentState.code.Equals("03")).ToList();//Code "03" es estado APROBADA

            var nowAux = DateTime.Now;
            priceListAux = priceListAux.AsEnumerable().Where(w => DateTime.Compare(w.CalendarPriceList.startDate.Date, nowAux.Date) <= 0 && DateTime.Compare(nowAux.Date, w.CalendarPriceList.endDate.Date) <= 0).ToList();

            var result = new
            {
                codeDocumentType = codeDocumentType,
                priceListBase = priceListAux.Select(s => new {
                    id = s.id,
                    name = s.name + " (" + s.Document.DocumentType.name + ") " + s.CalendarPriceList.CalendarPriceListType.name + " [" + s.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " +
                                                                           s.CalendarPriceList.endDate.ToString("dd/MM/yyyy") + "]"
                }).OrderBy(t => t.id).ToList()
            };

            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetDatesOfCalendar(int? id_calendarPriceList)
        {
            CalendarPriceList calendarPriceList = db.CalendarPriceList.FirstOrDefault(i => i.id == id_calendarPriceList);

            var result = new
            {
                startDate = calendarPriceList?.startDate.ToString("dd/MM/yyyy"),
                endDate = calendarPriceList?.endDate.ToString("dd/MM/yyyy")

            };

            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetPriceListInventoryLinesFilter()
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            //TempData.Keep("priceList");
            //CalendarPriceList calendarPriceList = db.CalendarPriceList.FirstOrDefault(i => i.id == id_calendarPriceList);
            List<int> inventoryLinesFilters = priceList.list_idInventaryLineFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idInventaryLineFilter);
            string list_idInventaryLineFilterStr = "";
            foreach (var i in inventoryLinesFilters)
            {
                if (i != 0)
                {
                    if (list_idInventaryLineFilterStr == "") list_idInventaryLineFilterStr = i.ToString();
                    else list_idInventaryLineFilterStr += "," + i.ToString();
                }
            }
            var result = new
            {
                inventoryLines = list_idInventaryLineFilterStr

            };

            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePriceListDetail(int[] inventoryLines, int[] itemTypes, int[] itemGroups)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();

            if (inventoryLines != null && inventoryLines.Length > 0)
            {
                var tempInventoryLines = new List<int>();
                foreach (var i in inventoryLines)
                {
                    tempInventoryLines.Add(i);
                }

                priceList.list_idInventaryLineFilter = JsonConvert.SerializeObject(tempInventoryLines);
            }

            if (itemTypes != null && itemTypes.Length > 0)
            {
                var tempItemTypes = new List<int>();
                foreach (var i in itemTypes)
                {
                    tempItemTypes.Add(i);
                }

                priceList.list_idItemTypeFilter = JsonConvert.SerializeObject(tempItemTypes);
            }

            if (itemGroups != null && itemGroups.Length > 0)
            {
                var tempItemGroups = new List<int>();
                foreach (var i in itemGroups)
                {
                    tempItemGroups.Add(i);
                }

                priceList.list_idItemGroupFilter = JsonConvert.SerializeObject(tempItemGroups);
            }

            var result = new
            {
                Message = "Ok"

            };

            TempData["priceList"] = priceList;

            UpdatePriceListDetailFilter(priceList);
            UpdatePriceListDetailFilterShow(priceList);

            TempData["priceList"] = priceList;
            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void UpdatePriceListDetailFilter(PriceList priceList)
        {
            var itemsAux = db.Item.ToList();

            List<int> inventoryLinesFilters = priceList.list_idInventaryLineFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idInventaryLineFilter);
            if (inventoryLinesFilters != null && inventoryLinesFilters.Count() > 0)
            {
                itemsAux = itemsAux.Where(w=> inventoryLinesFilters.Contains(w.id_inventoryLine) || inventoryLinesFilters.Contains(0)).ToList();
            }

            List<int> itemTypesFilters = priceList.list_idItemTypeFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idItemTypeFilter);
            if (itemTypesFilters != null && itemTypesFilters.Count() > 0)
            {
                itemsAux = itemsAux.Where(w => itemTypesFilters.Contains(w.id_itemType) || itemTypesFilters.Contains(0)).ToList();
            }

            List<int> itemGroupsFilters = priceList.list_idItemGroupFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idItemGroupFilter);
            if (itemGroupsFilters != null && itemGroupsFilters.Count() > 0)
            {
                itemsAux = itemsAux.Where(w => itemGroupsFilters.Contains(w.ItemGeneral?.id_group ?? 0) || 
                                               itemGroupsFilters.Contains(w.ItemGeneral?.id_subgroup ?? 0) ||
                                               itemGroupsFilters.Contains(0)).ToList();
            }

            if (priceList.PriceListDetail == null)
            {
                priceList.PriceListDetail = new List<PriceListDetail>();
            }
            for (int i = priceList.PriceListDetail.Count - 1; i >= 0; i--)
            {
                var detail = priceList.PriceListDetail.ElementAt(i);
                priceList.PriceListDetail.Remove(detail);
                //db.Entry(detail).State = EntityState.Deleted;
            }

            
            foreach (var i in itemsAux)
            {
                var newDetail = new PriceListDetail
                {
                    id_item = i.id,
                    Item = i,
                    
                    purchasePrice = 0,
                    salePrice = 0,
                    specialPrice = 0
                };
                var metricUnitLbs = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs");
                metricUnitLbs = metricUnitLbs ?? db.MetricUnit.FirstOrDefault();
                if (priceList.Document.DocumentType != null)
                {
                    newDetail.id_metricUnit = (priceList.Document.DocumentType.code == "18" ||
                                               priceList.Document.DocumentType.code == "19") ?
                                               i.ItemPurchaseInformation?.id_metricUnitPurchase ?? metricUnitLbs.id :
                                               i.ItemSaleInformation?.id_metricUnitSale ?? metricUnitLbs.id;
                    newDetail.MetricUnit = (priceList.Document.DocumentType.code == "18" ||
                                         priceList.Document.DocumentType.code == "19") ?
                                         i.ItemPurchaseInformation?.MetricUnit ?? metricUnitLbs :
                                         i.ItemSaleInformation?.MetricUnit ?? metricUnitLbs;
                }
                else
                {
                    newDetail.id_metricUnit = metricUnitLbs.id;
                    newDetail.MetricUnit = metricUnitLbs;
                }

                priceList.PriceListDetail.Add(newDetail);
            }
        }

        private void UpdatePriceListDetailFilterShow(PriceList priceList)
        {
            //var itemsAux = db.Item.ToList();

            List<int> filterShow = priceList.list_filterShow == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_filterShow);
            if (priceList.PriceListDetailFilterShow == null)
            {
                priceList.PriceListDetailFilterShow = new List<PriceListDetailFilterShow>();
            }
            for (int i = priceList.PriceListDetailFilterShow.Count - 1; i >= 0; i--)
            {
                var detail = priceList.PriceListDetailFilterShow.ElementAt(i);
                priceList.PriceListDetailFilterShow.Remove(detail);
                //db.Entry(detail).State = EntityState.Deleted;
            }


            foreach (var d in priceList.PriceListDetail)
            {
                var newDetail = new PriceListDetailFilterShow
                {
                    id_item = d.id_item,
                    Item = d.Item,
                    id_metricUnit = d.id_metricUnit,
                    MetricUnit = d.MetricUnit,
                    purchasePrice = d.purchasePrice,
                    salePrice = d.salePrice,
                    specialPrice = d.specialPrice
                };
                if (filterShow != null && filterShow.Count() > 0)
                {
                    if (filterShow.Contains(1) || filterShow.Contains(0))
                    {
                        priceList.PriceListDetailFilterShow.Add(newDetail);
                    }else
                    {
                        if (filterShow.Contains(2) && filterShow.Contains(3))
                        {
                            if(priceList.PriceListDetailFilterShow.FirstOrDefault(fod=> fod.Item.id_itemTypeCategory == newDetail.Item.id_itemTypeCategory &&
                                                                                        fod.Item.ItemGeneral?.id_size == newDetail.Item.ItemGeneral?.id_size) == null)
                            {
                                priceList.PriceListDetailFilterShow.Add(newDetail);
                            }
                        }else
                        {
                            if (filterShow.Contains(2))
                            {
                                if (priceList.PriceListDetailFilterShow.FirstOrDefault(fod => fod.Item.id_itemTypeCategory == newDetail.Item.id_itemTypeCategory) == null)
                                {
                                    priceList.PriceListDetailFilterShow.Add(newDetail);
                                }
                            }else
                            {
                                if (filterShow.Contains(3))
                                {
                                    if (priceList.PriceListDetailFilterShow.FirstOrDefault(fod => fod.Item.ItemGeneral?.id_size == newDetail.Item.ItemGeneral?.id_size) == null)
                                    {
                                        priceList.PriceListDetailFilterShow.Add(newDetail);
                                    }
                                }
                            }
                        }
                    }
                        //itemsAux = itemsAux.Where(w => filterShow.Contains(1)).ToList();
                }
                else
                {
                    priceList.PriceListDetailFilterShow.Add(newDetail);
                }
                
            }
        }

        private void UpdatePriceListDetailCategoryAdjustment(PriceList priceList)
        {
            var ItemGroupCategories = db.ItemGroupCategory.Where(w=> w.isActive && w.id_company == this.ActiveCompanyId);

            foreach (var i in ItemGroupCategories)
            {
                if(priceList.PriceListDetailCategoryAdjustment.FirstOrDefault(fod=> fod.id_itemGroupCategory == i.id) == null)
                {
                    priceList.PriceListDetailCategoryAdjustment.Add(new PriceListDetailCategoryAdjustment
                    {
                        id = priceList.PriceListDetailCategoryAdjustment.Count() > 0 ? priceList.PriceListDetailCategoryAdjustment.Max(pld => pld.id) + 1 : 1,
                        id_itemGroupCategory = i.id,
                        ItemGroupCategory = i,
                        id_priceList = priceList.id,
                        PriceList = priceList,
                        adjustment = 0
                    });
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetPriceListItemTypesFilter()
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            //TempData.Keep("priceList");
            List<int> itemTypesFilters = priceList.list_idItemTypeFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idItemTypeFilter);
            string list_idItemTypeFilterStr = "";
            foreach (var i in itemTypesFilters)
            {
                if (i != 0)
                {
                    if (list_idItemTypeFilterStr == "") list_idItemTypeFilterStr = i.ToString();
                    else list_idItemTypeFilterStr += "," + i.ToString();
                }
            }
            var result = new
            {
                itemTypes = list_idItemTypeFilterStr

            };

            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetPriceListItemGroupsFilter()
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            //TempData.Keep("priceList");
            List<int> itemGroupsFilters = priceList.list_idItemGroupFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idItemGroupFilter);
            string list_idItemGroupFilterStr = "";
            foreach (var i in itemGroupsFilters)
            {
                if (i != 0)
                {
                    if (list_idItemGroupFilterStr == "") list_idItemGroupFilterStr = i.ToString();
                    else list_idItemGroupFilterStr += "," + i.ToString();
                }
            }
            var result = new
            {
                itemGroups = list_idItemGroupFilterStr

            };

            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetPriceListFilterShow()
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            //TempData.Keep("priceList");
            List<int> filterShowsFilters = priceList.list_filterShow == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_filterShow);
            string list_filterShowStr = "";
            foreach (var i in filterShowsFilters)
            {
                if (i != 0)
                {
                    if (list_filterShowStr == "") list_filterShowStr = i.ToString();
                    else list_filterShowStr += "," + i.ToString();
                }
            }
            var result = new
            {
                filterShows = list_filterShowStr

            };

            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePriceListDetailFilterShow(int[] filterShows)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();

            if (filterShows != null && filterShows.Length > 0)
            {
                var tempFilterShows = new List<int>();
                foreach (var i in filterShows)
                {
                    tempFilterShows.Add(i);
                }

                priceList.list_filterShow = JsonConvert.SerializeObject(tempFilterShows);
            }

            var result = new
            {
                Message = "Ok"

            };

            TempData["priceList"] = priceList;

            //UpdatePriceListDetailFilter(priceList);
            UpdatePriceListDetailFilterShow(priceList);

            TempData["priceList"] = priceList;
            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void UpdatePriceListDetailFilterShowToPriceListDetail(PriceListDetailFilterShow priceListDetailFilterShow, PriceListDetail priceListDetail)
        {
            priceListDetail.id_metricUnit = priceListDetailFilterShow.id_metricUnit;
            priceListDetail.MetricUnit = priceListDetailFilterShow.MetricUnit;
            priceListDetail.purchasePrice = priceListDetailFilterShow.purchasePrice;
            priceListDetail.salePrice = priceListDetailFilterShow.salePrice;
            priceListDetail.specialPrice = priceListDetailFilterShow.specialPrice;
            //this.UpdateModel(priceListDetail);
        }

        private void UpdateModelPriceListDetail(PriceList priceList, PriceListDetailFilterShow modelItem)
        {
            var model = priceList?.PriceListDetail.ToList() ?? new List<PriceListDetail>();

            List<int> filterShow = priceList.list_filterShow == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_filterShow);

            foreach (var d in model)
            {
                if (filterShow != null && filterShow.Count() > 0)
                {
                    if (filterShow.Contains(1) || filterShow.Contains(0))//Mostrar Item
                    {
                        UpdatePriceListDetailFilterShowToPriceListDetail(modelItem, d);
                    }
                    else
                    {
                        if (filterShow.Contains(2) && filterShow.Contains(3))//Mostrar Item Tipo Categoria y Talla
                        {
                            if (d.Item.id_itemTypeCategory == modelItem.Item.id_itemTypeCategory && d.Item.ItemGeneral.id_size == modelItem.Item.ItemGeneral.id_size)
                            {
                                UpdatePriceListDetailFilterShowToPriceListDetail(modelItem, d);
                            }
                        }
                        else
                        {
                            if (filterShow.Contains(2))//Mostrar Item Tipo Categoria
                            {
                                if (d.Item.id_itemTypeCategory == modelItem.Item.id_itemTypeCategory)
                                {
                                    UpdatePriceListDetailFilterShowToPriceListDetail(modelItem, d);
                                }
                            }
                            else
                            {
                                if (filterShow.Contains(3))//Mostrar  Talla
                                {
                                    if (d.Item.ItemGeneral.id_size == modelItem.Item.ItemGeneral.id_size)
                                    {
                                        UpdatePriceListDetailFilterShowToPriceListDetail(modelItem, d);
                                    }
                                }
                            }
                        }
                    }
                    //itemsAux = itemsAux.Where(w => filterShow.Contains(1)).ToList();
                }
                else
                {
                    UpdatePriceListDetailFilterShowToPriceListDetail(modelItem, d);//Mostrar Todos
                }

            }
        }

        private void UpdateViewDataShow()
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();
            TempData.Keep("priceList");

            List<int> filterShow = priceList.list_filterShow == null ? new List<int>(): JsonConvert.DeserializeObject<List<int>>(priceList.list_filterShow);
            ViewData["ShowItem"] = (filterShow != null && filterShow.Count() > 0) ? filterShow.Contains(1) || filterShow.Contains(0) : true;
            ViewData["ShowItemTypeCategory"] = (filterShow != null && filterShow.Count() > 0) ? filterShow.Contains(2) || filterShow.Contains(0) : true;
            ViewData["Showsize"] = (filterShow != null && filterShow.Count() > 0) ? filterShow.Contains(3) || filterShow.Contains(0) : true;
            var codeDocumentType = priceList.Document?.DocumentType?.code ?? "";
            ViewData["ShowPurchasePrice"] = (codeDocumentType == "18" || codeDocumentType == "19");
            ViewData["ShowSalePrice"] = (codeDocumentType == "20" || codeDocumentType == "21");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePriceListDetails(int? id_priceListBase)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();

            PriceList priceListBase = db.PriceList.FirstOrDefault(fod=> fod.id == id_priceListBase);
            if (priceListBase != null)
            {

                priceList.id_calendarPriceList = priceListBase.id_calendarPriceList;
                priceList.CalendarPriceList = priceListBase.CalendarPriceList;
                priceList.startDate = priceListBase.startDate;
                priceList.endDate = priceListBase.endDate;
                priceList.isForPurchase = priceListBase.isForPurchase;
                priceList.isForSold = priceListBase.isForSold;
                //priceList.isQuotation = priceListBase.isQuotation;

                priceList.list_idInventaryLineFilter = priceListBase.list_idInventaryLineFilter;
                priceList.list_idItemTypeFilter = priceListBase.list_idItemTypeFilter;
                priceList.list_idItemGroupFilter = priceListBase.list_idItemGroupFilter;
                priceList.list_filterShow = priceListBase.list_filterShow;
                priceList.PriceListDetail = new List<PriceListDetail>();
                priceList.PriceListDetailFilterShow = new List<PriceListDetailFilterShow>();

                foreach (var detail in priceListBase.PriceListDetail)
                {
                    priceList.PriceListDetail.Add(new PriceListDetail
                    {
                        id_item = detail.id_item,
                        Item = detail.Item,
                        id_metricUnit = detail.id_metricUnit,
                        MetricUnit = detail.MetricUnit,
                        purchasePrice = detail.purchasePrice,
                        salePrice = detail.salePrice,
                        specialPrice = detail.specialPrice
                    });
                }

                foreach (var detail in priceListBase.PriceListDetailFilterShow)
                {
                    priceList.PriceListDetailFilterShow.Add(new PriceListDetailFilterShow
                    {
                        id_item = detail.id_item,
                        Item = detail.Item,
                        id_metricUnit = detail.id_metricUnit,
                        MetricUnit = detail.MetricUnit,
                        purchasePrice = detail.purchasePrice,
                        salePrice = detail.salePrice,
                        specialPrice = detail.specialPrice
                    });
                }
            }
            else
            {
                priceList.id_calendarPriceList = 0;
                priceList.CalendarPriceList = null;

                priceList.list_idInventaryLineFilter = JsonConvert.SerializeObject(new List<int>());
                priceList.list_idItemTypeFilter = JsonConvert.SerializeObject(new List<int>());
                priceList.list_idItemGroupFilter = JsonConvert.SerializeObject(new List<int>());
                priceList.list_filterShow = JsonConvert.SerializeObject(new List<int>());
                priceList.PriceListDetail = new List<PriceListDetail>();
                priceList.PriceListDetailFilterShow = new List<PriceListDetailFilterShow>();

                UpdatePriceListDetailFilter(priceList);
                UpdatePriceListDetailFilterShow(priceList);
            }

            List<int> list_idInventaryLineFilter = priceList.list_idInventaryLineFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idInventaryLineFilter);
            string list_idInventaryLineFilterStr = "";
            foreach (var i in list_idInventaryLineFilter)
            {
                if (i != 0)
                {
                    if (list_idInventaryLineFilterStr == "") list_idInventaryLineFilterStr = i.ToString();
                    else list_idInventaryLineFilterStr += "," + i.ToString();
                }
                
            }

            List<int> list_idItemTypeFilter = priceList.list_idItemTypeFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idItemTypeFilter);
            string list_idItemTypeFilterStr = "";
            foreach (var i in list_idItemTypeFilter)
            {
                if (i != 0)
                {
                    if (list_idItemTypeFilterStr == "") list_idItemTypeFilterStr = i.ToString();
                    else list_idItemTypeFilterStr += "," + i.ToString();
                }
                
            }

            List<int> list_idItemGroupFilter = priceList.list_idItemGroupFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_idItemGroupFilter);
            string list_idItemGroupFilterStr = "";
            foreach (var i in list_idItemGroupFilter)
            {
                if (i != 0)
                {
                    if (list_idItemGroupFilterStr == "") list_idItemGroupFilterStr = i.ToString();
                    else list_idItemGroupFilterStr += "," + i.ToString();
                }
                
            }

            List<int> list_filterShow = priceList.list_filterShow == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(priceList.list_filterShow);
            string list_filterShowStr = "";
            foreach (var i in list_filterShow)
            {
                if(i != 0)
                {
                    if (list_filterShowStr == "") list_filterShowStr = i.ToString();
                    else list_filterShowStr += "," + i.ToString();
                }
                
            }

            var result = new
            {
                id_calendarPriceList = priceList.id_calendarPriceList,
                list_idInventaryLineFilter = list_idInventaryLineFilterStr,
                list_idItemTypeFilter = list_idItemTypeFilterStr,
                list_idItemGroupFilter = list_idItemGroupFilterStr,
                list_filterShow = list_filterShowStr
            };


            TempData["priceList"] = priceList;
            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private bool RepeatedPriceListGeneral(PriceList item)
        {
            var priceListGeneralAux = db.PriceList.Where(fod => fod.id_calendarPriceList == item.id_calendarPriceList &&
                                                                            !fod.isQuotation &&
                                                                            fod.Document.id_documentType == item.Document.id_documentType &&
                                                                            fod.id != item.id);

            if (priceListGeneralAux == null || priceListGeneralAux.Count() == 0) return false;

            bool priceListGeneralRepeated = true;
            foreach (var detailPriceListGeneralAux in priceListGeneralAux)
            {
                //list_idInventaryLineFilter
                List<int> list_idInventaryLineFilterDetail = detailPriceListGeneralAux.list_idInventaryLineFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(detailPriceListGeneralAux.list_idInventaryLineFilter);
                List<int> list_idInventaryLineFilterCurrent = item.list_idInventaryLineFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(item.list_idInventaryLineFilter);
                if (list_idInventaryLineFilterDetail.Count() == list_idInventaryLineFilterCurrent.Count())
                {
                    foreach (var i in list_idInventaryLineFilterCurrent)
                    {
                        if (!list_idInventaryLineFilterDetail.Any(a => a == i))
                        {
                            return false;
                        }
                    }

                }
                else
                {
                    return false;
                }
                //list_idItemTypeFilter
                List<int> list_idItemTypeFilterDetail = detailPriceListGeneralAux.list_idItemTypeFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(detailPriceListGeneralAux.list_idItemTypeFilter);
                List<int> list_idItemTypeFilterCurrent = item.list_idItemTypeFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(item.list_idItemTypeFilter);
                if (list_idItemTypeFilterDetail.Count() == list_idItemTypeFilterCurrent.Count())
                {
                    foreach (var i in list_idItemTypeFilterCurrent)
                    {
                        if (!list_idItemTypeFilterDetail.Any(a => a == i))
                        {
                            return false;
                        }
                    }

                }
                else
                {
                    return false;
                }
                //list_idItemGroupFilter
                List<int> list_idItemGroupFilterDetail = detailPriceListGeneralAux.list_idItemGroupFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(detailPriceListGeneralAux.list_idItemGroupFilter);
                List<int> list_idItemGroupFilterCurrent = item.list_idItemGroupFilter == null ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(item.list_idItemGroupFilter);
                if (list_idItemGroupFilterDetail.Count() == list_idItemGroupFilterCurrent.Count())
                {
                    foreach (var i in list_idItemGroupFilterCurrent)
                    {
                        if (!list_idItemGroupFilterDetail.Any(a => a == i))
                        {
                            return false;
                        }
                    }

                }
                else
                {
                    return false;
                }
            }
            return priceListGeneralRepeated;
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitAdjustment()
        {
            decimal maxValueAdjustment = 0;

            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();

            DocumentType documentType = db.DocumentType.FirstOrDefault(i => i.id == priceList.Document.id_documentType);
            var codeDocumentType = documentType?.code ?? "";

            var minPurchasePrice = priceList.PriceListDetailFilterShow.Min(m => m.purchasePrice);
            var minSalePrice = priceList.PriceListDetailFilterShow.Min(m => m.salePrice);

            if (codeDocumentType != "")
            {
                if (codeDocumentType == "19" || codeDocumentType == "18")
                {
                    maxValueAdjustment = minPurchasePrice;
                }
                if (codeDocumentType == "21" || codeDocumentType == "20")
                {
                    maxValueAdjustment = minSalePrice;
                }
            }else
            {
                if(minPurchasePrice < minSalePrice)
                {
                    maxValueAdjustment = minPurchasePrice;

                }
                else
                {
                    maxValueAdjustment = minSalePrice;

                }
            }

            var result = new
            {
                maxValueAdjustment = maxValueAdjustment

            };

            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateAdjustmentItemGroupCategory(string price)
        {
            PriceList priceList = (TempData["priceList"] as PriceList);
            priceList = priceList ?? new PriceList();

            decimal _price = Convert.ToDecimal(price);

            if (priceList.PriceListDetailCategoryAdjustment != null)
            {
                foreach (var detail in priceList.PriceListDetailCategoryAdjustment)
                {
                    var difference = _price + detail.adjustment;
                    if (difference < 0)
                    {
                        detail.adjustment = (-_price);
                        this.UpdateModel(detail);
                    }
                }
            }

            var result = new
            {
                Message = "OK"

            };

            TempData["priceList"] = priceList;
            TempData.Keep("priceList");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}