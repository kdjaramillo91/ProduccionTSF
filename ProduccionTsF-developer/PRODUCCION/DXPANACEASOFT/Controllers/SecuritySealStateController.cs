using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using Excel = Microsoft.Office.Interop.Excel;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    public class SecuritySealStateController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }
        #region SecuritySealState GRIDVIEW

        [ValidateInput(false)]
        public ActionResult SecuritySealStatePartial(int? keyToCopy)
        {if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.SecuritySealState.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.SecuritySealState.Where(t => t.id_company == this.ActiveCompanyId);
            return PartialView("_SecuritySealStatePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SecuritySealStatePartialAddNew(DXPANACEASOFT.Models.SecuritySealState item)
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
                        db.SecuritySealState.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Estado del Sello de Seguridad: " + item.name + " guardado exitosamente");
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

            var model = db.SecuritySealState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_SecuritySealStatePartial", model.ToList());

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult SecuritySealStatePartialUpdate(DXPANACEASOFT.Models.SecuritySealState item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.SecuritySealState.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.SecuritySealState.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Estado del Sello de Seguridad: " + item.name + " guardado exitosamente");
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

            var model = db.SecuritySealState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_SecuritySealStatePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SecuritySealStatePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.SecuritySealState.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.SecuritySealState.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Estado del Sello de Seguridad: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.SecuritySealState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_SecuritySealStatePartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedSecuritySealState(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var security_seal_state = db.SecuritySealState.Where(i => ids.Contains(i.id));
                        foreach (var securitysealstate in security_seal_state)
                        {
                            securitysealstate.isActive = false;

                            securitysealstate.id_userUpdate = ActiveUser.id;
                            securitysealstate.dateUpdate = DateTime.Now;

                            db.SecuritySealState.Attach(securitysealstate);
                            db.Entry(securitysealstate).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Estados del Sello de Seguridad desactivados exitosamente");
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

            var model = db.SecuritySealState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_SecuritySealStatePartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileSecuritySealStatey()
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

                                    SecuritySealState securitySealStateImport = db.SecuritySealState.FirstOrDefault(l => l.code.Equals(code));

                                    if (securitySealStateImport == null)
                                    {
                                        securitySealStateImport = new SecuritySealState
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

                                        db.SecuritySealState.Add(securitySealStateImport);
                                    }
                                    else
                                    {
                                        securitySealStateImport.code = code;
                                        securitySealStateImport.name = name;
                                        securitySealStateImport.description = description;

                                        securitySealStateImport.id_userUpdate = ActiveUser.id;
                                        securitySealStateImport.dateUpdate = DateTime.Now;

                                        db.SecuritySealState.Attach(securitySealStateImport);
                                        db.Entry(securitySealStateImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeSecuritySealState(int id_securitySealState, string code)
        {
            SecuritySealState securitySealState = db.SecuritySealState.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_securitySealState);

            if (securitySealState == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Estado del Sello de Seguridad" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult SecuritySealStateReport()
        {
            SecuritySealStateReport report = new SecuritySealStateReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_SecuritySealStateReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SecuritySealStateDetailReport(int id)
        {
            SecuritySealStateDetailReport report = new SecuritySealStateDetailReport();
            report.Parameters["id_securitySealState"].Value = id;
            return PartialView("_SecuritySealStateReport", report);
        }

        #endregion
    }
}
