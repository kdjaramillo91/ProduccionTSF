using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    public class EconomicGroupController : DefaultController
    {
        //Método POST
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ECONOMIC GROUP GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult EconomicGroupPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.EconomicGroup.FirstOrDefault(b => b.id == keyToCopy);
            }

            var model = db.EconomicGroup.Where(wht => (1==1));
            return PartialView("_EconomicGroupPartial", model.ToList());
            
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EconomicGroupPartialAddNew(EconomicGroup item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.EconomicGroup.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Grupo Económico: " + item.name + " guardado exitosamente");
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

            var model = db.EconomicGroup.Where(o => (1==1));
            return PartialView("_EconomicGroupPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult EconomicGroupPartialUpdate(EconomicGroup item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.EconomicGroup.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.EconomicGroup.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Grupo Económico: " + item.name + " guardado exitosamente");
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

            var model = db.EconomicGroup.Where(o => (1==1));
            return PartialView("_EconomicGroupPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EconomicGroupPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.EconomicGroup.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.EconomicGroup.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Grupo Económico: " + (item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.EconomicGroup.Where(o => (1==1));
            return PartialView("_EconomicGroupPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedEconomicGroup(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.EconomicGroup.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.EconomicGroup.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Grupos Económicos desactivados exitosamente");
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

            var model = db.EconomicGroup.Where(o => (1==1));
            return PartialView("_EconomicGroupPartial", model.ToList());
        }
        #endregion

        #region REPORTES
        //[HttpPost]
        //public ActionResult EconomicGroupReport(int[] ids)
        //{
        //    EconomicGroupReport report = new EconomicGroupReport();
        //    report.Parameters["id_company"].Value = this.ActiveCompanyId;
        //    return PartialView("_EconomicGroupReport", report);
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult WarehouseTypeDetailReport(int id)
        //{
        //    WarehouseTypeDetailReport report = new WarehouseTypeDetailReport();
        //    report.Parameters["id__warehouseType"].Value = id;
        //    return PartialView("_WarehouseTypeReport", report);
        //}
        #endregion

        #region FUNCIONES AUXILIARES

        public JsonResult ImportFileEconomicGroup()
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
                                    Excel.Range row = table.Rows[i];
                                    try
                                    {
                                        code = row.Cells[1].Text;
                                        name = row.Cells[2].Text;
                                        description = row.Cells[4].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    EconomicGroup economicGroup = db.EconomicGroup.FirstOrDefault(l => l.code.Equals(code));
                                    
                                    if (economicGroup == null)
                                    {
                                        economicGroup = new EconomicGroup
                                        {
                                            code = code,
                                            name = name,
                                            description = description,
                                            isActive = true,
                                            
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.EconomicGroup.Add(economicGroup);
                                    }
                                    else
                                    {
                                        economicGroup.code = code;
                                        economicGroup.name = name;
                                        economicGroup.description = description;

                                        economicGroup.id_userUpdate = ActiveUser.id;
                                        economicGroup.dateUpdate = DateTime.Now;

                                        db.EconomicGroup.Attach(economicGroup);
                                        db.Entry(economicGroup).State = EntityState.Modified;
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
        public JsonResult ValidateCodeEconomicGroup(int id_economicGroup, string code)
        {
            EconomicGroup economicGroup = db.EconomicGroup.FirstOrDefault(b => b.code == code
                                                                            && b.id != id_economicGroup);

            if (economicGroup == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Grupo Económico" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}