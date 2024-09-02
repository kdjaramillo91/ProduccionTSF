using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace DXPANACEASOFT.Controllers{
    public class TariffHeadinggController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region TariffHeadingg GRIDVIEW

        [ValidateInput(false)]
        public ActionResult TariffHeadinggPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.TariffHeading.FirstOrDefault(b => b.id == keyToCopy);
            }
            //var model = db.Country.Where(o => o.id_company == this.ActiveCompanyId);
            var model = db.TariffHeading.ToList();
            return PartialView("_TariffHeadinggPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TariffHeadinggPartialAddNew(DXPANACEASOFT.Models.TariffHeading item)
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
                        db.TariffHeading.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Partida Arancelaria : " + item.nombre + " guardado exitosamente");
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

            //var model = db.TariffHeadingg.Where(o => o.id_company == this.ActiveCompanyId);
            var model = db.TariffHeading.ToList();
            return PartialView("_TariffHeadinggPartial", model.ToList());

        }

        public ActionResult TariffHeadinggPartialUpdate(DXPANACEASOFT.Models.TariffHeading item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.TariffHeading.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                        
                            modelItem.code = item.code;
                            modelItem.nombre = item.nombre;
                            modelItem.nombre = item.nombre;

                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;


                            db.TariffHeading.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Partida Arancelaria: " + item.nombre + " guardado exitosamente");
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

            //var model = db.TariffHeadingg.Where(o => o.id_company == this.ActiveCompanyId);
            var model = db.TariffHeading.ToList();
            return PartialView("_TariffHeadinggPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TariffHeadinggPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.TariffHeading.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.TariffHeading.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Partida Arancelaria: " + (item?.nombre ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            //var model = db.Country.Where(o => o.id_company == this.ActiveCompanyId);
            var model = db.TariffHeading.ToList();
            return PartialView("_TariffHeadinggPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedTariffHeadingg(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var car = db.TariffHeading.Where(i => ids.Contains(i.id));
                        foreach (var TariffHeadingg in car)
                        {
                            TariffHeadingg.isActive = false;

                            TariffHeadingg.id_userUpdate = ActiveUser.id;
                            TariffHeadingg.dateUpdate = DateTime.Now;

                            db.TariffHeading.Attach(TariffHeadingg);
                            db.Entry(TariffHeadingg).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Partida Arancelaria desactivados exitosamente");
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

            //var model = db.Country.Where(o => o.id_company == this.ActiveCompanyId);
            var model = db.TariffHeading.ToList();
            return PartialView("_TariffHeadinggPartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportTariffHeadingg()
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

                                    TariffHeading TariffHeadinggImport = db.TariffHeading.FirstOrDefault(l => l.code.Equals(code));

                                    if (TariffHeadinggImport == null)
                                    {
                                        TariffHeadinggImport = new TariffHeading
                                        {
                                            code = code,
                                            nombre = name,
                                            //description = description,
                                            isActive = true,

                                            //id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.TariffHeading.Add(TariffHeadinggImport);
                                    }
                                    else
                                    {
                                        TariffHeadinggImport.code = code;
                                        TariffHeadinggImport.nombre = name;
                                        //TariffHeadinggImport.description = description;

                                        TariffHeadinggImport.id_userUpdate = ActiveUser.id;
                                        TariffHeadinggImport.dateUpdate = DateTime.Now;

                                        db.TariffHeading.Attach(TariffHeadinggImport);
                                        db.Entry(TariffHeadinggImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeTariffHeadingg(int id_TariffHeadingg, string code)
        {
            TariffHeading TariffHeadingg = db.TariffHeading.FirstOrDefault(b =>  b.code == code && b.id != id_TariffHeadingg);

            if (TariffHeadingg == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Término de Negociación" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult TariffHeadinggReport()
        {
            //TariffHeadinggReport report = new TariffHeadinggReport();
            //report.Parameters["id_company"].Value = this.ActiveCompanyId;
            //return PartialView("_TariffHeadinggReport", report);
            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TariffHeadinggDetailReport(int id)
        {
            //TariffHeadinggDetailReport report = new TariffHeadinggDetailReport();
            //report.Parameters["id_TariffHeadingg"].Value = id;
            //return PartialView("_TariffHeadinggReport", report);
            return null;
        }

        #endregion
    }
}

