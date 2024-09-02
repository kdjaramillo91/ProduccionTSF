using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    public class ProductionLotStateController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region PRODUCTION LOTE GRIDVIEW

        [ValidateInput(false)]
        public ActionResult ProductionLotStatePartial()
        {
            var model = db.ProductionLotState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionLotStatePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotStatePartialAddNew(DXPANACEASOFT.Models.ProductionLotState item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                        db.ProductionLotState.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Estado de Lote de Producción: " + item.name + " guardado exitosamente");
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

            var model = db.ProductionLotState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionLotStatePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotStatePartialUpdate(DXPANACEASOFT.Models.ProductionLotState item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionLotState.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            
                            modelItem.name = item.name;
                            modelItem.code = item.code;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.ProductionLotState.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Estado de Lote de Producción: " + modelItem.name + " guardado exitosamente");
                        }
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

            var model = db.ProductionLotState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionLotStatePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotStatePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ProductionLotState.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.ProductionLotState.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Estado de Lote de Producción: " + item?.name ?? "" + " desactivados exitosamente");
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

            var model = db.ProductionLotState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionLotStatePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedProductionLotState(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var production_lot_state = db.ProductionLotState.Where(i => ids.Contains(i.id));
                        foreach (var productionlotstate in production_lot_state)
                        {
                            productionlotstate.isActive = false;

                            productionlotstate.id_userUpdate = ActiveUser.id;
                            productionlotstate.dateUpdate = DateTime.Now;

                            db.ProductionLotState.Attach(productionlotstate);
                            db.Entry(productionlotstate).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Estados de Lote de Producción desactivados exitosamente");
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

            var model = db.ProductionLotState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ProductionLotStatePartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateCode(int id_productionLotState, string code)
        {
            ProductionLotState productionLot = db.ProductionLotState.FirstOrDefault(b => b.id != id_productionLotState && b.id_company == this.ActiveCompanyId && b.code.Equals(code) && b.isActive);

            if (productionLot == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro estado" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileProductionLotState()
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
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ProductionLotState productionLotState = db.ProductionLotState.FirstOrDefault(l => l.code.Equals(code));

                                    if (productionLotState == null)
                                    {
                                        productionLotState = new ProductionLotState
                                        {
                                            code = code,
                                            name = name,
                                            description = description,
                                            
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.ProductionLotState.Add(productionLotState);
                                    }
                                    else
                                    {
                                        productionLotState.code = code;
                                        productionLotState.name = name;
                                        productionLotState.description = description;

                                        productionLotState.id_userUpdate = ActiveUser.id;
                                        productionLotState.dateUpdate = DateTime.Now;

                                        db.ProductionLotState.Attach(productionLotState);
                                        db.Entry(productionLotState).State = EntityState.Modified;
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

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult ProductionLotStateReport()
        {
            ProductionLotStateReport report = new ProductionLotStateReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ProductionLotStateReport", report);
        }

        #endregion
    }
}