using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class WarehouseController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Warehouse GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehousesPartial(int? keyToCopy, bool? isEditing, bool? isNewRowEditing)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Warehouse.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.Warehouse.Where(whl => whl.id_company == this.ActiveCompanyId);

            //if (isNewRowEditing == true || isEditing == true)
            {
                if (TempData["WarehouseExpenseAccounting"] != null)
                    TempData.Remove("WarehouseExpenseAccounting");
            }

            return PartialView("_WarehousesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehousesPartialAddNew(Warehouse item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Roles de la bodega
                        string[] tokens = TokenBoxExtension.GetSelectedValues<string>("ids_Roles");
                        item.ids_Roles = (tokens != null) ? string.Join("|", tokens) : null;

                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        HandleExpenseAccountingDetail(ref item);

                        db.Warehouse.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Bodega: " + item.name + " guardada exitosamente");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                        ViewData["EditError"] = ex.InnerException?.Message ?? ex.Message;
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.Warehouse.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_WarehousesPartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult WarehousesPartialUpdate(Warehouse item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Warehouse.FirstOrDefault(it => it.id == item.id);
                        string[] tokens = TokenBoxExtension.GetSelectedValues<string>("ids_Roles");
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.id_warehouseType = item.id_warehouseType;
                            modelItem.id_inventoryLine = item.id_inventoryLine;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.id_inventoryValuaionPeriod = item.id_inventoryValuaionPeriod;
                            modelItem.yearPeriod = item.yearPeriod;
                            modelItem.numberPeriod = item.numberPeriod;
                            modelItem.dateInitPeriod = item.dateInitPeriod;
                            modelItem.dateEndPeriod = item.dateEndPeriod;
                            modelItem.allowsNegativeBalances = item.allowsNegativeBalances;
                            modelItem.applyinCost = item.applyinCost;
                            modelItem.requirePerson = item.requirePerson;
                            modelItem.ids_Roles = (tokens != null) ? string.Join("|", tokens) : null;
                            modelItem.isActive = item.isActive;
                            modelItem.enableProductionCost = item.enableProductionCost;
                            modelItem.id_productionCostPoundType = item.id_productionCostPoundType;

                            HandleExpenseAccountingDetail(ref modelItem);

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.Warehouse.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Bodega: " + item.name + " guardada exitosamente");
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(ex.Message);
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.Warehouse.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_WarehousesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehousesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Warehouse.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.Warehouse.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();


                            ViewData["EditMessage"] = SuccessMessage("Bodega: " + (item?.name ?? "") + " desactivada exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.Warehouse.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_WarehousesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedWarehouses(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Warehouse.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.Warehouse.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Bodegas desactivadas exitosamente");
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

            var model = db.Warehouse.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_WarehousesPartial", model.ToList());
        }
        #endregion

        #region REPORTS


        [HttpPost]
        public ActionResult WarehouseReport()
        {
            WarehouseReport report = new WarehouseReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_WarehouseReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseDetailReport(int id)
        {
            WarehouseDetailReport report = new WarehouseDetailReport();
            report.Parameters["id_warehouse"].Value = id;
            return PartialView("_WarehouseReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        public JsonResult GetProcessPlantbyIdAccountingtemplate(int id_accountingTemplate)
        {
            if (TempData["warehouse"] != null)
            {
                TempData.Keep("warehouse");
            }
            var accountinTemplate = db.AccountingTemplate.FirstOrDefault(e => e.id == id_accountingTemplate);

            var nombreProceso = db.Person.FirstOrDefault(e => e.id == accountinTemplate.id_processPlant);

            var result = new
            {
                namePlantaProceso = nombreProceso.processPlant
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadNameProcessPlantChangeData(int id_accountingTemplate)
        {
            if (TempData["warehouse"] != null)
            {
                TempData.Keep("warehouse");
            }

            var accountinTemplate = db.AccountingTemplate.FirstOrDefault(e => e.id == id_accountingTemplate);

            var nombreProceso = db.Person.FirstOrDefault(e => e.id == accountinTemplate.id_processPlant);

            var result = new
            {
                namePlantaProceso = nombreProceso.processPlant
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AccountingTemplateChangeData(int id_costPoduction, int id_productionExpense)
        {
            if (TempData["warehouse"] != null)
            {
                TempData.Keep("warehouse");
            }

            var accountinTemplate = db.AccountingTemplate.Where(e => e.id_costProduction == id_costPoduction && e.id_expenseProduction == id_productionExpense)
                                        .Select(s => new
                                        {
                                            id = s.id,
                                            name = s.description
                                        }).ToList();

            return Json(accountinTemplate, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CostProductionChangeData(int id_costPoduction)
        {
            if (TempData["warehouse"] != null)
            {
                TempData.Keep("warehouse");
            }
            var expenseProduction = db.ProductionExpense.Where(w => w.id_productionCostType == id_costPoduction && w.isActive)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            return Json(expenseProduction, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NumberPeriodChangeData(int yearPeriod)
        {
            if (TempData["warehouse"] != null)
            {
                TempData.Keep("warehouse");
            }
            var lista1 = db.InventoryValuationPeriod.Where(w => w.id == yearPeriod).ToList();

            var result = (from a in lista1
                          join b in db.InventoryValuationPeriodDetail on a.id equals b.id_InventoryValuationPeriod
                          select new
                          {
                              id = a.id,
                              period = b.periodNumber
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DateInitChangeData(int id, int yearPeriod)
        {
            if (id <= 12)
            {
                var model = db.InventoryValuationPeriodDetail.FirstOrDefault(d => d.id_InventoryValuationPeriod == yearPeriod && d.periodNumber == id);

                if (model == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var result = new
                {
                    dateInitYear = model.dateInit.Year,
                    dateInitMonth = model.dateInit.Month,
                    dateInitDate = model.dateInit.Day,
                    dateEndYear = model.dateEnd.Year,
                    dateEndMonth = model.dateEnd.Month,
                    dateEndDay = model.dateEnd.Day,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = db.InventoryValuationPeriodDetail.FirstOrDefault(d => d.id_InventoryValuationPeriod == yearPeriod && d.id == id);

                if (model == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var result = new
                {
                    dateInitYear = model.dateInit.Year,
                    dateInitMonth = model.dateInit.Month,
                    dateInitDate = model.dateInit.Day,
                    dateEndYear = model.dateEnd.Year,
                    dateEndMonth = model.dateEnd.Month,
                    dateEndDay = model.dateEnd.Day,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ImportFileWarehouses()
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                List<string> errorMessages = new List<string>();

                if (file != null)
                {
                    string filename = Server.MapPath("~/App_Data/Temp/" + file.FileName);

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    file.SaveAs(filename);

                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(filename);

                    if (workbook.Sheets.Count > 0)
                    {
                        Excel.Worksheet worksheet = workbook.ActiveSheet;
                        Excel.Range table = worksheet.UsedRange;

                        string code = string.Empty;
                        string name = string.Empty;
                        int id_warehouseType = 0;
                        int id_inventoryLine = 0;
                        bool allowsNegativeBalances = true;

                        string description = string.Empty;


                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 2; i < table.Rows.Count; i++)
                                {
                                    Excel.Range row = table.Rows[i]; // FILA i
                                    try
                                    {
                                        code = row.Cells[1].Text;        // COLUMNA 1
                                        name = row.Cells[2].Text;
                                        description = row.Cells[3].Text;
                                        id_warehouseType = int.Parse(row.Cells[4].Text);
                                        id_inventoryLine = int.Parse(row.Cells[5].Text);
                                        allowsNegativeBalances = int.Parse(row.Cells[6].Text);

                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    Warehouse warehouseImport = db.Warehouse.FirstOrDefault(l => l.code.Equals(code));

                                    if (warehouseImport == null)
                                    {
                                        warehouseImport = new Warehouse
                                        {
                                            code = code,
                                            name = name,
                                            id_warehouseType = id_warehouseType,
                                            id_inventoryLine = id_inventoryLine,
                                            description = description,
                                            allowsNegativeBalances = allowsNegativeBalances,


                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.Warehouse.Add(warehouseImport);
                                    }
                                    else
                                    {
                                        warehouseImport.code = code;
                                        warehouseImport.name = name;
                                        warehouseImport.id_warehouseType = id_warehouseType;
                                        warehouseImport.id_inventoryLine = id_inventoryLine;
                                        warehouseImport.description = description;
                                        warehouseImport.allowsNegativeBalances = allowsNegativeBalances;

                                        warehouseImport.id_userUpdate = ActiveUser.id;
                                        warehouseImport.dateUpdate = DateTime.Now;

                                        db.Warehouse.Attach(warehouseImport);
                                        db.Entry(warehouseImport).State = EntityState.Modified;
                                    }
                                }

                                db.SaveChanges();
                                trans.Commit();
                            }
                            catch (Exception)
                            {
                                trans.Rollback();
                            }
                        }
                    }

                    application.Workbooks.Close();

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }

                    return Json(file?.FileName, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateCodeWarehouses(int id_warehouse, string code)
        {
            Warehouse warehouse = db.Warehouse.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_warehouse);

            if (warehouse == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Bodega" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region ProductionExpense and AccountingTemplate.
        private void HandleExpenseAccountingDetail(ref Warehouse item)
        {
            List<WarehouseExpenseAccountingTemplate> lsW = TempData["WarehouseExpenseAccounting"] as List<WarehouseExpenseAccountingTemplate>;

            if (lsW == null)
                return;

            int id = item.id;
            if (id > 0)
            {
                var lsTmp = db.WarehouseExpenseAccountingTemplate.Where(w => w.id_warehouse == id);

                if (lsTmp != null && lsTmp.Count() > 0)
                {
                    foreach (var det in lsTmp)
                    {
                        db.WarehouseExpenseAccountingTemplate.Remove(det);
                        db.WarehouseExpenseAccountingTemplate.Attach(det);
                        db.Entry(det).State = EntityState.Deleted;
                    }
                }
            }
            item.WarehouseExpenseAccountingTemplate = item.WarehouseExpenseAccountingTemplate ?? new List<WarehouseExpenseAccountingTemplate>();
            foreach (var det in lsW)
            {
                WarehouseExpenseAccountingTemplate tmp = new WarehouseExpenseAccountingTemplate();
                tmp.id_accountingTemplate = det.id_accountingTemplate;
                tmp.id_expenseProduction = det.id_expenseProduction;
                tmp.id = det.id;
                item.WarehouseExpenseAccountingTemplate.Add(tmp);
            }
        }

        [ValidateInput(false)]
        public ActionResult WarehouseExpenseAccountingDetail(int id_warehouse)
        {
            List<WarehouseExpenseAccountingTemplate> lsW = TempData["WarehouseExpenseAccounting"] as List<WarehouseExpenseAccountingTemplate>;
            if (lsW == null)
            {
                lsW = db.WarehouseExpenseAccountingTemplate
                    .Where(w => w.id_warehouse == id_warehouse)
                    .ToList();
                TempData["WarehouseExpenseAccounting"] = lsW;
            }
            lsW = lsW ?? new List<WarehouseExpenseAccountingTemplate>();
            TempData["WarehouseExpenseAccounting"] = lsW;
            TempData.Keep("WarehouseExpenseAccounting");

            return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
        }
        public ActionResult WarehouseExpenseAccountingDetailAddNew(int id_warehouse
            , WarehouseExpenseAccountingTemplate item)
        {
            List<WarehouseExpenseAccountingTemplate> lsW = TempData["WarehouseExpenseAccounting"] as List<WarehouseExpenseAccountingTemplate>;

            if (lsW == null)
            {
                lsW = db.WarehouseExpenseAccountingTemplate
                    .Where(w => w.id_warehouse == id_warehouse)
                    .ToList();
                TempData["WarehouseExpenseAccounting"] = lsW;
                TempData.Keep("WarehouseExpenseAccounting");
            }
            lsW = lsW ?? new List<WarehouseExpenseAccountingTemplate>();

            if (item == null)
            {
                ViewData["EditMessage"] = ErrorMessage("Se está agregando un modelo incorrecto.");
                return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
            }

            var idMax = lsW.Any()
                ? lsW.Max(s => s.id) + 1
                : 1;

            item.id = idMax;

            lsW.Add(item);

            TempData["WarehouseExpenseAccounting"] = lsW;
            TempData.Keep("WarehouseExpenseAccounting");

            return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
        }
        public ActionResult WarehouseExpenseAccountingDetailUpdate(int id_warehouse
            , WarehouseExpenseAccountingTemplate item)
        {
            List<WarehouseExpenseAccountingTemplate> lsW = TempData["WarehouseExpenseAccounting"] as List<WarehouseExpenseAccountingTemplate>;

            if (lsW == null)
            {
                lsW = db.WarehouseExpenseAccountingTemplate
                    .Where(w => w.id_warehouse == id_warehouse)
                    .ToList();
                TempData["WarehouseExpenseAccounting"] = lsW;
                TempData.Keep("WarehouseExpenseAccounting");
            }
            lsW = lsW ?? new List<WarehouseExpenseAccountingTemplate>();

            if (item == null)
            {
                ViewData["EditMessage"] = ErrorMessage("Se está actualizando un modelo incorrecto.");
                return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
            }
            var itemToUpdate = lsW.FirstOrDefault(fod => fod.id == item.id);

            if (itemToUpdate == null)
            {
                ViewData["EditMessage"] = ErrorMessage("Se está actualizando un modelo incorrecto [1].");
                return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
            }
            itemToUpdate.id_accountingTemplate = item.id_accountingTemplate;
            itemToUpdate.id_expenseProduction = item.id_expenseProduction;
            itemToUpdate.AccountingTemplate = db.AccountingTemplate.FirstOrDefault(fod => fod.id == item.id_accountingTemplate);

            TempData["WarehouseExpenseAccounting"] = lsW;
            TempData.Keep("WarehouseExpenseAccounting");

            return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
        }
        public ActionResult WarehouseExpenseAccountingDetailDelete(int id_warehouse
            , WarehouseExpenseAccountingTemplate item)
        {
            List<WarehouseExpenseAccountingTemplate> lsW = TempData["WarehouseExpenseAccounting"] as List<WarehouseExpenseAccountingTemplate>;

            if (lsW == null)
            {
                lsW = db.WarehouseExpenseAccountingTemplate
                    .Where(w => w.id_warehouse == id_warehouse)
                    .ToList();
                TempData["WarehouseExpenseAccounting"] = lsW;
                TempData.Keep("WarehouseExpenseAccounting");
            }
            lsW = lsW ?? new List<WarehouseExpenseAccountingTemplate>();

            if (item == null)
            {
                ViewData["EditMessage"] = ErrorMessage("Se está actualizando un modelo incorrecto.");
                return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
            }
            var itemToUpdate = lsW.FirstOrDefault(fod => fod.id == item.id);

            if (itemToUpdate == null)
            {
                ViewData["EditMessage"] = ErrorMessage("Se está actualizando un modelo incorrecto [1].");
                return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
            }
            lsW.Remove(itemToUpdate);

            TempData["WarehouseExpenseAccounting"] = lsW;
            TempData.Keep("WarehouseExpenseAccounting");

            return PartialView("_WarehousesExpensesTempAccDetailPartial", lsW);
        }
        [HttpPost]
        public JsonResult ValidateRepeatedExpenses(int id_warehouse, int id_productionExpense, int id_accountingTemplate, bool isNewRow, int editRowKey)
        {

            /*
		  id_productionExpense: productionExpense,
			id_accountingTemplate: accountTemplate,
			: isNewRow*/
            List<WarehouseExpenseAccountingTemplate> lsW = TempData["WarehouseExpenseAccounting"] as List<WarehouseExpenseAccountingTemplate>;
            if (lsW == null)
            {
                lsW = db.WarehouseExpenseAccountingTemplate
                    .Where(w => w.id_warehouse == id_warehouse)
                    .ToList();
                TempData["WarehouseExpenseAccounting"] = lsW;
            }
            lsW = lsW ?? new List<WarehouseExpenseAccountingTemplate>();
            TempData["WarehouseExpenseAccounting"] = lsW;
            TempData.Keep("WarehouseExpenseAccounting");

            if (isNewRow)
            {
                var accountTemplate = lsW.FirstOrDefault(fod =>
                fod.id_accountingTemplate == id_accountingTemplate
                && fod.id_expenseProduction == id_productionExpense);
                if (accountTemplate != null)
                    return Json(new { isValid = false, errorText = "El gasto de produccion ya esta registrado con la plantilla seleccionada." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var accountTemplate = lsW.FirstOrDefault(fod => fod.id != editRowKey &&
                fod.id_accountingTemplate == id_accountingTemplate
                && fod.id_expenseProduction == id_productionExpense);
                if (accountTemplate != null)
                    return Json(new { isValid = false, errorText = "El gasto de produccion ya esta registrado con la plantilla seleccionada." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetExpenseProduction(int? id_productionCost, int? id_productionExpense)
        {
            //WarehouseExpenseAccountingTemplate warehouseExpenseAccountingTemplate = new WarehouseExpenseAccountingTemplate();
            //warehouseExpenseAccountingTemplate.id_expenseProduction = id_productionExpense??0;
            //return PartialView("_ComboBoxProductionExpense", warehouseExpenseAccountingTemplate);
            List<WarehouseExpenseAccountingTemplate> item = (TempData["WarehouseExpenseAccounting"] as List<WarehouseExpenseAccountingTemplate>);
            item = item ?? new List<WarehouseExpenseAccountingTemplate>();
            TempData.Keep("WarehouseExpenseAccounting");

            ViewData["id_productionExpense"] = id_productionExpense;

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_expenseProduction";

                p.ValueField = "id";
                p.ValueType = typeof(int);
                p.TextFormatString = "{0} - {1}";
                p.CallbackPageSize = 15;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.ClientSideEvents.Init = "ExpenseProductionCombo_OnInit";

                p.Columns.Add("code", "Código", Unit.Percentage(20));
                p.Columns.Add("name", "Nombre", Unit.Percentage(70));
                p.CallbackRouteValues = new { Controller = "Warehouse", Action = "GetExpenseProduction" };
                p.ClientSideEvents.BeginCallback = "ExpenseProductionCombo_BeginCallback";
                p.ClientSideEvents.SelectedIndexChanged = "ExpenseProductionCombo_SelectedIndexChanged";
                p.ClientSideEvents.EndCallback = "ExpenseProductionCombo_EndCallback";

                p.BindList(DataProviderProductionExpense.ProductionExpenseByIdProductionCost(id_productionCost));

            });

        }
        [HttpPost]
        public ActionResult GetAccountingTemplate(int? id_productionCost, int? id_productionExpense, int? id_accountingTemplate)
        {
            //WarehouseExpenseAccountingTemplate warehouseExpenseAccountingTemplate = new WarehouseExpenseAccountingTemplate();
            //warehouseExpenseAccountingTemplate.id_accountingTemplate = id_accountingTemplate ?? 0;
            //return PartialView("_ComboBoxAccountingTemplate", warehouseExpenseAccountingTemplate);
            List<WarehouseExpenseAccountingTemplate> item = (TempData["WarehouseExpenseAccounting"] as List<WarehouseExpenseAccountingTemplate>);
            item = item ?? new List<WarehouseExpenseAccountingTemplate>();
            TempData.Keep("WarehouseExpenseAccounting");

            ViewData["id_accountingTemplate"] = id_accountingTemplate;

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_accountingTemplate";
                //p.DataSource = DataProviderAccountingTemplate.AccountingTemplateByCompany((int)ViewData["id_company"]);

                p.ValueField = "id";
                p.ValueType = typeof(int);
                p.DisplayFormatString = "{0} - {1}";
                p.CallbackPageSize = 15;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.ClientSideEvents.Init = "AccountingTemplateCombo_OnInit";

                p.Columns.Add("code", "Código", Unit.Percentage(30));
                p.Columns.Add("description", "Descripción", Unit.Percentage(70));
                p.CallbackRouteValues = new { Controller = "Warehouse", Action = "GetAccountingTemplate" };
                p.ClientSideEvents.BeginCallback = "AccountingTemplateCombo_BeginCallback";
                p.ClientSideEvents.EndCallback = "AccountingTemplateCombo_EndCallback";

                p.BindList(DataProviderProductionExpense.AccountTemplateByIdProductionCostIdProductionExpense(id_productionCost, id_productionExpense));//.Bind(id_person);

            });

        }
        #endregion

        #region Métodos para la importación de archivos

        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "WarehouseArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditWarehouses()
        {
            return PartialView("_FormEditImportWarehouse");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportWarehouses()
        {
            var empresa = this.ActiveCompanyId;
            return WarehouseExcelFileParser.GetTemplateFileContentResult(empresa);
        }

        [HttpPost]
        public JsonResult ImportDatosCargaMasiva(string guidArchivoDatos)
        {
            bool isValid;
            string message;
            ImportResult resultadoImportacion;

            try
            {
                var contenidoArchivo = FileUploadHelper.ReadFileUpload(guidArchivoDatos,
                    out var nombreArchivo, out var mimeArchivo, out var rutaArchivoContenido);

                var itemTypeCategoryParser = new WarehouseExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemTypeCategoryParser.ParseWarehouse(ActiveCompanyId, ActiveUser);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron las bodegas exitosamente";
            }
            catch (Exception ex)
            {
                isValid = false;

                resultadoImportacion = new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = new ImportResult.DocumentoFallido[] { },
                };

                message = ex.InnerException != null
                    ? "Error al importar las bodegas. " + ex.InnerException.Message
                    : "Error al importar las bodegas. " + ex.Message;
            }

            // Preservamos los resultados de la importación de datos
            var guidResultado = Guid.NewGuid().ToString("n");
            this.TempData[$"documentos-importados-{guidResultado}"] = resultadoImportacion.Importados;
            this.TempData[$"documentos-fallidos-{guidResultado}"] = resultadoImportacion.Fallidos;

            ViewData["EditMessage"] = resultadoImportacion.Fallidos.Any() || !isValid
                ? ErrorMessage(message) : SuccessMessage(message);

            // Retornar el resultado...
            var result = new
            {
                isValid,
                message,
                guidResultado,
                HayErrores = resultadoImportacion.Fallidos.Any(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResult DownloadDocumentosFallidosImportacion(string guidResultado, string mensajeAlerta)
        {
            this.ViewBag.ReportCommand = "export";
            this.ViewBag.ReportTitle = "Errores en Importación de Bodegas";
            this.ViewBag.ExcelFileName = $"ErroresImportacionWarehouse_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-fallidos-{guidResultado}";
            var documentosFallidos = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoFallido[]
                : new ImportResult.DocumentoFallido[] { };

            ViewData["EditMessage"] = ErrorMessage(mensajeAlerta);


            return this.View("_DocumentosFallidosImportacionReportPartial", documentosFallidos);
        }

        [HttpGet]
        public ViewResult DownloadDocumentosImportadosImportacion(string guidResultado, string mensajeAlerta)
        {
            this.ViewBag.ReportCommand = "export";
            this.ViewBag.ReportTitle = "Resultados de Importación de Bodegas";
            this.ViewBag.ExcelFileName = $"ResultadosImportacionWarehouse_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-importados-{guidResultado}";
            var documentosImportados = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoImportado[]
                : new ImportResult.DocumentoImportado[] { };

            ViewData["EditMessage"] = SuccessMessage(mensajeAlerta);


            return this.View("_DocumentosImportadosImportacionReportPartial", documentosImportados);
        }

        #endregion

        #region Configuración común para la carga de archivo de Excel con transacciones

        public class UploadControlSettings
        {
            public readonly static UploadControlValidationSettings ImageUploadValidationSettings;
            public readonly static UploadControlValidationSettings ExcelUploadValidationSettings;
            public readonly static UploadControlValidationSettings AnyDocumentUploadValidationSettings;

            static UploadControlSettings()
            {
                ImageUploadValidationSettings = new UploadControlValidationSettings()
                {
                    AllowedFileExtensions = new[] { ".jpe", ".jpeg", ".jpg", ".gif", ".png" },
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };

                ExcelUploadValidationSettings = new UploadControlValidationSettings()
                {
                    AllowedFileExtensions = new[] { ".xls", ".xlsx", ".xlsm" },
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };

                AnyDocumentUploadValidationSettings = new UploadControlValidationSettings()
                {
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };
            }

            public static void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
            {
                if (e.UploadedFile.IsValid)
                {
                    var fileId = FileUploadHelper.FileUploadProcess(e);

                    if (!String.IsNullOrEmpty(fileId))
                    {
                        var result = new
                        {
                            id = fileId,
                            filename = e.UploadedFile.FileName,
                        };

                        e.CallbackData = JsonConvert.SerializeObject(result);
                    }
                }
            }
        }

        #endregion
    }
}



