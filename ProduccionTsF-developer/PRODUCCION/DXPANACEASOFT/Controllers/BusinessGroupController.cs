using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.DataProviders;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class BusinessGroupController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region BusinessGroup GridView

        [ValidateInput(false)]
        public ActionResult BusinessGroupsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Country.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.BusinessGroup/*.Where(o => o.id_company == this.ActiveCompanyId)*/;
            return PartialView("_BusinessGroupsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessGroupsPartialAddNew(DXPANACEASOFT.Models.BusinessGroup item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        //item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.BusinessGroup.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Grupo de Negocio: " + item.name + " guardado exitosamente");
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

            var model = db.BusinessGroup/*.Where(o => o.id_company == this.ActiveCompanyId)*/;
            return PartialView("_BusinessGroupsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessGroupsPartialUpdate(DXPANACEASOFT.Models.BusinessGroup item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.BusinessGroup.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.logo = item.logo;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.BusinessGroup.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Grupo de Negocio: " + item.name + " guardado exitosamente");
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

            var model = db.BusinessGroup/*.Where(o => o.id_company == this.ActiveCompanyId)*/;
            return PartialView("_BusinessGroupsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessGroupsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                { 
                    try
                    {
                        var item = db.BusinessGroup.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }

                        db.BusinessGroup.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Grupo de Negocio: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.BusinessGroup/*.Where(o => o.id_company == this.ActiveCompanyId)*/;
            return PartialView("_BusinessGroupsPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedBusinessGroups(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var car = db.BusinessGroup.Where(i => ids.Contains(i.id));
                        foreach (var businessGroup in car)
                        {
                            businessGroup.isActive = false;

                            businessGroup.id_userUpdate = ActiveUser.id;
                            businessGroup.dateUpdate = DateTime.Now;

                            db.BusinessGroup.Attach(businessGroup);
                            db.Entry(businessGroup).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Grupo de Negocio exitosamente");
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

            var model = db.BusinessGroup/*.Where(o => o.id_company == this.ActiveCompanyId)*/;
            return PartialView("_BusinessGroupsPartial", model.ToList());
        }

        #endregion


        #region Auxiliar Functions

        public ActionResult BinaryImageColumnPhotoUpdate()
        {
            return BinaryImageEditExtension.GetCallbackResult();
        }

        public ActionResult BusinessGroupsDetailPartial()
        {
            int id_businessGroup = (Request.Params["id_businessGroup"] != null && Request.Params["id_businessGroup"] != "") ? int.Parse(Request.Params["id_businessGroup"]) : -1;
            ViewData["id_businessGroup"] = id_businessGroup;
            return PartialView("_BusinessGroupDetailsPartial", DataProviderCompany.CompaniesByBusinessGroup(id_businessGroup));
        }

        [HttpPost]
        public JsonResult ImportFileBusinessGroup()
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

                                    BusinessGroup businessGroupImport = db.BusinessGroup.FirstOrDefault(l => l.code.Equals(code));

                                    if (businessGroupImport == null)
                                    {
                                        businessGroupImport = new BusinessGroup
                                        {
                                            code = code,
                                            name = name,
                                            description = description,
                                            isActive = true,

                                            //id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.BusinessGroup.Add(businessGroupImport);
                                    }
                                    else
                                    {
                                        businessGroupImport.code = code;
                                        businessGroupImport.name = name;
                                        businessGroupImport.description = description;

                                        businessGroupImport.id_userUpdate = ActiveUser.id;
                                        businessGroupImport.dateUpdate = DateTime.Now;

                                        db.BusinessGroup.Attach(businessGroupImport);
                                        db.Entry(businessGroupImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeBusinessGroup(int id_businessGroup, string code)
        {
            BusinessGroup businessGroup = db.BusinessGroup.FirstOrDefault(b => /*b.id_company == this.ActiveCompanyId*/
                                                                            /*&&*/ b.code == code
                                                                            && b.id != id_businessGroup);

            if (businessGroup == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Grupo de Negocio" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult BusinessGroupReport()
        {
            BusinessGroupReport report = new BusinessGroupReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_BusinessGroupReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BusinessGroupDetailReport(int id)
        {
            BusinessGroupDetailReport report = new BusinessGroupDetailReport();
            report.Parameters["id_country"].Value = id;
            return PartialView("_BusinessGroupReport", report);
        }

        #endregion
    }
}



