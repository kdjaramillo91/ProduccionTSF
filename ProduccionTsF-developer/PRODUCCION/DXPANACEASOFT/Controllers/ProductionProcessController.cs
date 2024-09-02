using DevExpress.Web.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Data.Common;
using System.Configuration;
using EntidadesAuxiliares.General;


namespace DXPANACEASOFT.Controllers
{
    public class ProductionProcessController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region ProductionProcess GRIDVIEW

        [ValidateInput(false)]
        public ActionResult ProductionProcessPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = ProductionProcess.GetOneById((keyToCopy??0));
            }
            var model = ProductionProcess.GetAllByCompany(this.ActiveCompanyId);
            return PartialView("_ProductionProcessPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionProcessPartialAddNew(DXPANACEASOFT.Models.ProductionProcess item)
        {
            if (ModelState.IsValid)
            {
                //DbConnection conexion = db.Database.Connection;
                //conexion.Open();
                string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                using (var conexion = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                {
                    try
                    {
                        conexion.Open();
                        using (var trans = conexion.BeginTransaction())
                        {
                            try
                            {
                                item.id_company = this.ActiveCompanyId;
                                item.id_userCreate = ActiveUser.id;
                                item.dateCreate = DateTime.Now;
                                item.id_userUpdate = ActiveUser.id;
                                item.dateUpdate = DateTime.Now;

                                ProductionProcess.InsertProductionProcess(conexion, trans, item);
                                //db.ProductionProcess.Add(item);
                                //db.SaveChanges();

                                trans.Commit();
                                ViewData["EditMessage"] = SuccessMessage("Proceso de producción: " + item.name + " guardado exitosamente");
                            }
                            catch (Exception)
                            {
                                trans.Rollback();
                                ViewData["EditMessage"] = ErrorMessage();
                            }
                        }
                    }
                    finally
                    {
                        conexion.Close();
                    }
                        
                    
                }
                //using (DbContextTransaction trans = db.Database.BeginTransaction())
                

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            //var model = db.ProductionProcess.Where(o => o.id_company == this.ActiveCompanyId);
            var model = ProductionProcess.GetAllByCompany(this.ActiveCompanyId);
            return PartialView("_ProductionProcessPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionProcessPartialUpdate(DXPANACEASOFT.Models.ProductionProcess item)
        {
            if (ModelState.IsValid)
            {
                string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                using (var conexion = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                {
                    try
                    {
                        conexion.Open();
                        using (var trans = conexion.BeginTransaction())
                        {
                            try
                            {
                                //var modelItem = db.ProductionProcess.FirstOrDefault(it => it.id == item.id);
                                var modelItem = ProductionProcess.GetOneById(item.id);
                                if (modelItem != null)
                                {

                                    modelItem.id_parentProcess = item.id_parentProcess;
                                    modelItem.name = item.name;
                                    modelItem.code = item.code;
                                    modelItem.sequential = item.sequential;
                                    modelItem.id_ProductionUnit = item.id_ProductionUnit;
                                    modelItem.id_warehouse = item.id_warehouse;
                                    modelItem.id_WarehouseLocation = item.id_WarehouseLocation;
                                    modelItem.id_CostCenter = item.id_CostCenter;
                                    modelItem.id_SubCostCenter = item.id_SubCostCenter;
                                    modelItem.description = item.description;
                                    modelItem.isActive = item.isActive;
                                    modelItem.generateTransfer = item.generateTransfer;
                                    modelItem.requestliquidationmachine = item.requestliquidationmachine;
                                    modelItem.generatesAbsorption = item.generatesAbsorption;

                                    modelItem.requestCarMachine = item.requestCarMachine;

                                    modelItem.id_userUpdate = ActiveUser.id;
                                    modelItem.dateUpdate = DateTime.Now;

                                    //db.ProductionProcess.Attach(modelItem);
                                    //db.Entry(modelItem).State = EntityState.Modified;
                                    //db.SaveChanges();
                                    ProductionProcess.UpdateProductionProcess(conexion, trans, modelItem);

                                    trans.Commit();
                                    ViewData["EditMessage"] = SuccessMessage("Proceso de producción: " + item.name + " guardado exitosamente");
                                }
                            }
                            catch (Exception)
                            {
                                trans.Rollback();
                                ViewData["EditMessage"] = ErrorMessage();
                            }
                        }
                    }
                    finally
                    {
                        conexion.Close();
                    }
                }
                

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            //var model = db.ProductionProcess.Where(o => o.id_company == this.ActiveCompanyId);
            var model = ProductionProcess.GetAllByCompany(this.ActiveCompanyId);
            return PartialView("_ProductionProcessPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionProcessPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                using (var conexion = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                {
                    try
                    {
                        conexion.Open();
                        using (var trans = conexion.BeginTransaction())
                        {
                            try
                            {
                                var item = ProductionProcess.GetOneById(id);
                                if (item != null)
                                {
                                    item.isActive = false;
                                    item.id_userUpdate = ActiveUser.id;
                                    item.dateUpdate = DateTime.Now;

                                }
                                //db.ProductionProcess.Remove(item);
                                //db.Entry(item).State = EntityState.Modified;
                                //db.SaveChanges();
                                ProductionProcess.UpdateProductionProcess(conexion, trans, item);
                                trans.Commit();
                                ViewData["EditMessage"] = SuccessMessage("Proceso de producción: " + (item?.name ?? "") + " desactivado exitosamente");
                            }
                            catch (Exception)
                            {
                                ViewData["EditMessage"] = ErrorMessage();
                                trans.Rollback();
                            }
                        }
                    }
                    finally
                    {
                        conexion.Close();
                    }
                }
                

            }

            var model = ProductionProcess.GetAllByCompany(this.ActiveCompanyId);
            return PartialView("_ProductionProcessPartial", model.ToList());
        }

        public ActionResult DeleteSelectedProductionProcess(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                using (var conexion = new System.Data.SqlClient.SqlConnection(dapperDBContext))
                {
                    try
                    {
                        conexion.Open();
                        using (var trans = conexion.BeginTransaction())
                        {
                            try
                            {
                                var production_process = ProductionProcess.GetAllByIds(ids);
                                foreach (var productionprocess in production_process)
                                {
                                    productionprocess.isActive = false;

                                    productionprocess.id_userUpdate = ActiveUser.id;
                                    productionprocess.dateUpdate = DateTime.Now;

                                    //db.ProductionProcess.Attach(productionprocess);
                                    //db.Entry(productionprocess).State = EntityState.Modified;

                                    ProductionProcess.UpdateProductionProcess(conexion, trans, productionprocess);
                                }

                                //db.SaveChanges();
                                trans.Commit();
                                ViewData["EditMessage"] = SuccessMessage("Procesos de producción desactivados exitosamente");
                            }
                            catch (Exception)
                            {
                                trans.Rollback();
                                ViewData["EditMessage"] = ErrorMessage();
                            }
                        }
                    }
                    finally
                    {
                        conexion.Close();
                    }
                }

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = ProductionProcess.GetAllByCompany(this.ActiveCompanyId);
            return PartialView("_ProductionProcessPartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS
        [HttpPost]
        public JsonResult WarehouseChangeData(int id_warehouse)
        {
            if (TempData["productionProcess"] != null)
            {
                TempData.Keep("productionProcess");
            }
            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse && w.isActive)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            var result = new
            {
                warehouseLocations = warehouseLocations
                                    .Select(s => new
                                    {
                                        id = s.id,
                                        name = s.name
                                    })
            };

            TempData.Keep("productionProcess");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CostCenterChangeData(int id_costCenter)
        {
            if (TempData["productionProcess"] != null)
            {
                TempData.Keep("productionProcess");
            }
            var subcenterCost = db.CostCenter.Where(w => w.id_higherCostCenter == id_costCenter && w.isActive && w.id_higherCostCenter != null)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            return Json(subcenterCost, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileProductionProcess()
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
                        int sequential = 0;
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
                                        sequential = int.Parse(row.Cells[3].Text);
                                        description = row.Cells[4].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ProductionProcess productionProcessImport = db.ProductionProcess.FirstOrDefault(l => l.code.Equals(code));

                                    if (productionProcessImport == null)
                                    {
                                        productionProcessImport = new ProductionProcess
                                        {
                                            code = code,
                                            name = name,
                                            sequential = sequential,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.ProductionProcess.Add(productionProcessImport);
                                    }
                                    else
                                    {
                                        productionProcessImport.code = code;
                                        productionProcessImport.name = name;
                                        productionProcessImport.sequential = sequential;
                                        productionProcessImport.description = description;

                                        productionProcessImport.id_userUpdate = ActiveUser.id;
                                        productionProcessImport.dateUpdate = DateTime.Now;

                                        db.ProductionProcess.Attach(productionProcessImport);
                                        db.Entry(productionProcessImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeProductionProcess(int id_productionProcess, string code)
        {
            ProductionProcess productionProcess = db.ProductionProcess.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_productionProcess);

            if (productionProcess == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Proceso de producción" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult ProductionProcessReport()
        {
            ProductionProcessReport report = new ProductionProcessReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ProductionProcessReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionProcessDetailReport(int id)
        {
            ProductionProcessDetailReport report = new ProductionProcessDetailReport();
            report.Parameters["id_productionProcess"].Value = id;
            return PartialView("_ProductionProcessReport", report);
        }

        #endregion
    }
}