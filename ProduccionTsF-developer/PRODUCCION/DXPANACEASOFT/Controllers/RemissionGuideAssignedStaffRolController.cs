using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    public class RemissionGuideAssignedStaffRolController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }


        #region COUNTRY RemissionGuideAssignedStaffRol

        [ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffRolPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.RemissionGuideAssignedStaffRol.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.RemissionGuideAssignedStaffRol.Where(p => p.id_company == this.ActiveCompanyId);
            return PartialView("_RemissionGuideAssignedStaffRolPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffRolPartialAddNew(DXPANACEASOFT.Models.RemissionGuideAssignedStaffRol item)
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
                        db.RemissionGuideAssignedStaffRol.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión al Personal: " + item.name + " guardado exitosamente");
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

            var model = db.RemissionGuideAssignedStaffRol.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_RemissionGuideAssignedStaffRolPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffRolPartialUpdate(DXPANACEASOFT.Models.RemissionGuideAssignedStaffRol item)
        {


            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.RemissionGuideAssignedStaffRol.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.RemissionGuideAssignedStaffRol.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Guía de Remisión al Personal: " + item.name + " guardado exitosamente");
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

            var model = db.RemissionGuideAssignedStaffRol.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_RemissionGuideAssignedStaffRolPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffRolPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.RemissionGuideAssignedStaffRol.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.RemissionGuideAssignedStaffRol.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Guía de Remisión al Personal: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.RemissionGuideAssignedStaffRol.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_RemissionGuideAssignedStaffRolPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedRemissionGuideAssignedStaffRol(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var remission_guide_assigned_staff_rol = db.RemissionGuideAssignedStaffRol.Where(i => ids.Contains(i.id));
                        foreach (var remissionguideassignedstaffrol in remission_guide_assigned_staff_rol)
                        {
                            remissionguideassignedstaffrol.isActive = false;

                            remissionguideassignedstaffrol.id_userUpdate = ActiveUser.id;
                            remissionguideassignedstaffrol.dateUpdate = DateTime.Now;

                            db.RemissionGuideAssignedStaffRol.Attach(remissionguideassignedstaffrol);
                            db.Entry(remissionguideassignedstaffrol).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Guías de Remisión al Personal desactivadas exitosamente");
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

            var model = db.RemissionGuideAssignedStaffRol.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_RemissionGuideAssignedStaffRolPartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]public JsonResult ImportFileRemissionGuideAssignedStaffRol()
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

                                    RemissionGuideAssignedStaffRol remissionGuideAssignedStaffRolImport = db.RemissionGuideAssignedStaffRol.FirstOrDefault(l => l.code.Equals(code));

                                    if (remissionGuideAssignedStaffRolImport == null)
                                    {
                                        remissionGuideAssignedStaffRolImport = new RemissionGuideAssignedStaffRol
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

                                        db.RemissionGuideAssignedStaffRol.Add(remissionGuideAssignedStaffRolImport);
                                    }
                                    else
                                    {
                                        remissionGuideAssignedStaffRolImport.code = code;
                                        remissionGuideAssignedStaffRolImport.name = name;
                                        remissionGuideAssignedStaffRolImport.description = description;

                                        remissionGuideAssignedStaffRolImport.id_userUpdate = ActiveUser.id;
                                        remissionGuideAssignedStaffRolImport.dateUpdate = DateTime.Now;

                                        db.RemissionGuideAssignedStaffRol.Attach(remissionGuideAssignedStaffRolImport);
                                        db.Entry(remissionGuideAssignedStaffRolImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeRemissionGuideAssignedStaffRol(int id_remissionGuideAssignedStaffRol, string code)
        {
            RemissionGuideAssignedStaffRol remissionGuideAssignedStaffRol = db.RemissionGuideAssignedStaffRol.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_remissionGuideAssignedStaffRol);

            if (remissionGuideAssignedStaffRol == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Guía de Remisión al Personal" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult RemissionGuideAssignedStaffRolReport()
        {
            RemissionGuideAssignedStaffRolReport report = new RemissionGuideAssignedStaffRolReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_RemissionGuideAssignedStaffRolReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RemissionGuideAssignedStaffRolDetailReport(int id)
        {
            RemissionGuideAssignedStaffRolDetailReport report = new RemissionGuideAssignedStaffRolDetailReport();
            report.Parameters["id_remissionGuideAssignedStaffRol"].Value = id;
            return PartialView("_RemissionGuideAssignedStaffRolReport", report);
        }


        #endregion
    }
}