using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using DXPANACEASOFT.Models;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class DocumentTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region DOCUMENT TYPE GRIDVIEW

        [ValidateInput(false)]
        public ActionResult DocumentTypePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.DocumentType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.DocumentType.OrderByDescending(t => t.id);
            return PartialView("_DocumentTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentTypePartialAddNew(DXPANACEASOFT.Models.DocumentType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var documentStates = item.DocumentState.Select(t => t.id).ToList();
                        item.DocumentState = db.DocumentState.Where(t => documentStates.Contains(t.id)).ToList();

                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.DocumentType.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipo de Documento: " + item.name + " guardado exitosamente");
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

            var model = db.DocumentType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DocumentTypePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentTypePartialUpdate(DXPANACEASOFT.Models.DocumentType item)
        {
            if (ModelState.IsValid)
            {
               using (DbContextTransaction trans = db.Database.BeginTransaction())
               {
                   try
                   {
                       var modelItem = db.DocumentType.FirstOrDefault(it => it.id == item.id);
                       if (modelItem != null)
                       {
                           modelItem.name = item.name;
                           modelItem.code = item.code;
                           modelItem.daysToExpiration = item.daysToExpiration;
                           modelItem.description = item.description;
                           modelItem.isActive = item.isActive;

                           var documentStates = item.DocumentState.Select(t => t.id).ToList();

                           if (documentStates.Count > 0)
                           {
                               // REMOVE OLD DOCUMENTSTATES
                               for (int i = modelItem.DocumentState.Count - 1; i >= 0; i--)
                               {
                                   DocumentState documentState = modelItem.DocumentState.ElementAt(i);
                                   if (!documentStates.Contains(documentState.id))
                                   {
                                       modelItem.DocumentState.Remove(documentState);
                                   }
                               }

                               // UPDATE DOCUMENTSTATES
                               foreach (var id in documentStates)
                               {
                                   DocumentState docState = modelItem.DocumentState.FirstOrDefault(d => d.id == id);
                                   if (docState == null)
                                   {
                                       docState = db.DocumentState.FirstOrDefault(d => d.id == id);
                                       modelItem.DocumentState.Add(docState);
                                   }
                               }
                           }

                           modelItem.id_userUpdate = ActiveUser.id;
                           modelItem.dateUpdate = DateTime.Now;

                           db.DocumentType.Attach(modelItem);
                           db.Entry(modelItem).State = EntityState.Modified;

                           db.SaveChanges();
                           trans.Commit();

                           ViewData["EditMessage"] =
                               SuccessMessage("Tipo de Documento: " + item.name + " guardado exitosamente");
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

            var model = db.DocumentType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DocumentTypePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.DocumentType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.DocumentType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Tipo de Documento: " + (item?.name ?? "") + " desactivado exitosamente");
                                }
                            }
                            catch (Exception)
                            {
                                ViewData["EditMessage"] = ErrorMessage();
                                trans.Rollback();
                            }
                        }

                    }

                    var model = db.DocumentType.Where(o => o.id_company == this.ActiveCompanyId);
                    return PartialView("_DocumentTypePartial", model.ToList());
                }

        [HttpPost]
        public ActionResult DeleteSelectedDocumentType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var doc = db.DocumentType.Where(i => ids.Contains(i.id));
                        foreach (var Documen in doc)
                        {
                            Documen.isActive = false;
                            db.Entry(Documen).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Tipo de Documentos desactivados exitosamente");
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

            var model = db.DocumentType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DocumentTypePartial", model.ToList());
        }

        #endregion

        public ActionResult DocumentTypeEditForm(int? id)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.id == id);
            documentType = documentType ?? new DocumentType { isActive = true };

            ViewData["dialogAddDocumentType"] = true;

            return PartialView("_DocumentTypeEditForm", documentType);
        }

        public ActionResult DocumentTypeDetailPartial(DocumentType metricuni)
        {
            var model = db.DocumentType.FirstOrDefault(i => i.id==metricuni.id);
            return PartialView("_DocumentTypePartial", model);
        }

        #region AUXILIAR FUNCTIONS


        [HttpPost]
        public JsonResult ValidateCodeDocumentType(int id_documentType, string code)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_documentType);

            if (documentType == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tipo de Documento" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult DocumentTypeReport()
        {
            DocumentTypeReport report = new DocumentTypeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_DocumentTypeReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentTypeDetailReport(int id)
        {
            DocumentTypeDetailReport report = new DocumentTypeDetailReport();
            report.Parameters["id_documentType"].Value = id;
            return PartialView("_DocumentTypeReport", report);
        }

        #endregion

    }
}


