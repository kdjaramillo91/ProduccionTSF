using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Web;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class DocumentStatesController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region DOCUMENT STATE GRIDVIEW

        [ValidateInput(false)]
        public ActionResult DocumentStatesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.DocumentState.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.DocumentState.OrderByDescending(s => s.id);
            return PartialView("_DocumentStatesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentStatesPartialAddNew(DXPANACEASOFT.Models.DocumentState item)
        {
            if (ModelState.IsValid)
            {
               using (DbContextTransaction trans = db.Database.BeginTransaction())
               {
                    try
                    {
                        var documentTypes = item.DocumentType.Select(t => t.id).ToList();
                        item.DocumentType = db.DocumentType.Where(t => documentTypes.Contains(t.id)).ToList();

                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.DocumentState.Add(item);

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Estado de Documento: " + item.name + " guardado exitosamente");
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

            var model = db.DocumentState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DocumentStatesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentStatesPartialUpdate(DXPANACEASOFT.Models.DocumentState item)
        {
            
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.DocumentState.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            var documentTypes = item.DocumentType.Select(t => t.id).ToList();

                            if (documentTypes.Count > 0)
                            {
                                // REMOVE OLD DOCUMENTTYPES
                                for (int i = modelItem.DocumentType.Count - 1; i >= 0; i--)
                                {
                                    DocumentType documentType = modelItem.DocumentType.ElementAt(i);
                                    if (!documentTypes.Contains(documentType.id))
                                    {
                                        modelItem.DocumentType.Remove(documentType);
                                    }
                                }

                                // UPDATE DOCUMENTOTYPES
                                foreach (var id in documentTypes)
                                {
                                    DocumentType docType = modelItem.DocumentType.FirstOrDefault(d => d.id == id);
                                    if (docType == null)
                                    {
                                        docType = db.DocumentType.FirstOrDefault(d => d.id == id);
                                        modelItem.DocumentType.Add(docType);
                                    }
                                }
                            }

                            modelItem.id_company = this.ActiveCompanyId;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.DocumentState.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Estado de Documento: " + item.name + " guardado exitosamente");
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

            var model = db.DocumentState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DocumentStatesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentStatesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.DocumentState.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.DocumentState.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] =  SuccessMessage("Estado de Documento: " + (item?.name ?? "") + " desactivado exitosamente");
                            }
                        }
                        catch (Exception)
                        {
                            ViewData["EditMessage"] = ErrorMessage();
                            trans.Rollback();
                        }
                    }

                }

            var model = db.DocumentState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DocumentStatesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedDocumentStates(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var car = db.DocumentState.Where(i => ids.Contains(i.id));
                        foreach (var documentStates in car)
                        {
                            documentStates.isActive = false;

                            documentStates.id_userUpdate = ActiveUser.id;
                            documentStates.dateUpdate = DateTime.Now;

                            db.DocumentState.Attach(documentStates);
                            db.Entry(documentStates).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Estados de Documentos desactivados exitosamente");
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

            var model = db.DocumentState.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DocumentStatesPartial", model.ToList());
        }

        #endregion

        public ActionResult DocumentStateEditForm(int? id)
        {
            DocumentState documentState = db.DocumentState.FirstOrDefault(d => d.id == id);
            documentState = documentState ?? new DocumentState { isActive = true };

            ViewData["dialogAddDocumentState"] = true;

            return PartialView("_DocumentStateEditForm", documentState);
        }

        //[HttpPost]
        //public void DeleteSelectedDocumentStates(int[] ids)
        //{
        //    if (ids != null && ids.Length > 0)
        //    {
        //        using (DbContextTransaction trans = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var doc = db.DocumentState.Where(i => ids.Contains(i.id));
        //                foreach (var Documen in doc)
        //                {
        //                    //Documen.isActive = false;
        //                    db.Entry(Documen).State = EntityState.Modified;
        //                }
        //                db.SaveChanges();
        //                trans.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                trans.Rollback();
        //            }
        //        }
        //    }
        //}

        //public ActionResult DocumentStatesDetailPartial(DocumentState metricunit)
        //{
        //    var model = db.DocumentState.FirstOrDefault(i => i.id == metricunit.id);
        //    return PartialView("_DocumentStatesDetailPartial", model);
        //}

        #region AUXILIAR FUNCTIONS

        [HttpPost]public JsonResult ImportFileDocumentState()
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

                                    Country countryImport = db.Country.FirstOrDefault(l => l.code.Equals(code));

                                    if (countryImport == null)
                                    {
                                        countryImport = new Country
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

                                        db.Country.Add(countryImport);
                                    }
                                    else
                                    {
                                        countryImport.code = code;
                                        countryImport.name = name;
                                        countryImport.description = description;

                                        countryImport.id_userUpdate = ActiveUser.id;
                                        countryImport.dateUpdate = DateTime.Now;

                                        db.Country.Attach(countryImport);
                                        db.Entry(countryImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeDocumentState(int id_documentState, string code)
        {
            DocumentState documentState = db.DocumentState.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_documentState);

            if (documentState == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Estado de Documento" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region REPORT

        [HttpPost]
        public ActionResult DocumentStateReport()
        {
            DocumentStateReport report = new DocumentStateReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_DocumentStateReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentStateDetailReport(int id)
        {
            DocumentStateDetailReport report = new DocumentStateDetailReport();
            report.Parameters["id_documentState"].Value = id;
            return PartialView("_DocumentStateReport", report);
        }


        #endregion
    }
}


