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
    [Authorize]
    public class CertificationController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Certification GRIDVIEW

        [HttpPost, ValidateInput(false)]

        public ActionResult CertificationsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Certification.FirstOrDefault(b => b.id == keyToCopy);
            }

            //var model = db.Certification.Where(wht => wht.id_company == this.ActiveCompanyId);
            return PartialView("_CertificationsPartial", db.Certification.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CertificationsPartialAddNew(Certification item)
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

                        db.Certification.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Certificación: " + item.name + " guardada exitosamente");
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

            //var model = db.Certification.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CertificationsPartial", db.Certification.ToList());
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult CertificationsPartialUpdate(Certification item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Certification.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.idLote = item.idLote;
                            modelItem.idProducto = item.idProducto;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.Certification.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Certificación: " + item.name + " guardada exitosamente");
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

            //var model = db.Certification.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CertificationsPartial", db.Certification.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CertificationsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Certification.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.Certification.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Certificación: " + (item?.name ?? "") + " desactivada exitosamente");
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

            //var model = db.Certification.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CertificationsPartial", db.Certification.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedCertifications(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Certification.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.Certification.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Certificaciones desactivadas exitosamente");
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

            //var model = db.Certification.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CertificationsPartial", db.Certification.ToList());
        }

        #endregion

        #region REPORTS 


        //[HttpPost]
        //public ActionResult CertificationReport(int[] ids)
        //{
        //    CertificationReport report = new CertificationReport();
        //    report.Parameters["id_company"].Value = this.ActiveCompanyId;
        //    return PartialView("_CertificationReport", report);
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult CertificationDetailReport(int id)
        //{
        //    CertificationDetailReport report = new CertificationDetailReport();
        //    report.Parameters["id__certification"].Value = id;
        //    return PartialView("_CertificationReport", report);
        //}

        #endregion

        #region AUXILIAR FUNCTIONS

        public JsonResult ImportFileCertification()
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
                        string idLote = string.Empty;
                        string idProducto = string.Empty;

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
                                        idLote = row.Cells[1].Text;
                                        idProducto = row.Cells[1].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    Certification certification = db.Certification.FirstOrDefault(l => l.code.Equals(code));

                                    if (certification == null)
                                    {
                                        certification = new Certification
                                        {
                                            code = code,
                                            name = name,
                                            idLote = idLote,
                                            idProducto = idProducto,
                                            isActive = true,

                                            //id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.Certification.Add(certification);
                                    }
                                    else
                                    {
                                        certification.code = code;
                                        certification.name = name;
                                        certification.idLote = idLote;
                                        certification.idProducto = idProducto;

                                        certification.id_userUpdate = ActiveUser.id;
                                        certification.dateUpdate = DateTime.Now;

                                        db.Certification.Attach(certification);
                                        db.Entry(certification).State = EntityState.Modified;
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
        public JsonResult ValidateCodeCertification(int id_certification, string code)
        {
            Certification certification = db.Certification.FirstOrDefault(b => /*b.id_company == this.ActiveCompanyId &&*/
                                                                            b.code == code
                                                                            && b.id != id_certification);

            if (certification == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Certificación" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}




