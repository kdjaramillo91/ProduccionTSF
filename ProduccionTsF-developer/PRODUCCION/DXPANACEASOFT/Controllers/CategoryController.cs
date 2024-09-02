using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Auxiliares;
using DevExpress.Web;
using Newtonsoft.Json;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class CategoryController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ClientCategory GRIDVIEW

        [HttpPost, ValidateInput(false)]

        public ActionResult CategoryPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ClientCategory.FirstOrDefault(b => b.id == keyToCopy);
            }

            var model = db.ClientCategory.Where(wht => wht.id_company == this.ActiveCompanyId);
            return PartialView("_CategoryPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryPartialAddNew(ClientCategory item)
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

                        db.ClientCategory.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Categoría: " + item.name + " guardada exitosamente");
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

            var model = db.ClientCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CategoryPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult CategoryPartialUpdate(ClientCategory item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ClientCategory.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                            modelItem.byDefault = item.byDefault;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.ClientCategory.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Categoría: " + item.name + " guardada exitosamente");
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

            var model = db.ClientCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CategoryPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ClientCategory.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.ClientCategory.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Categoría: " + (item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.ClientCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CategoryPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedCategory(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ClientCategory.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ClientCategory.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Categorías desactivadas exitosamente");
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

            var model = db.ClientCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_CategoryPartial", model.ToList());
        }

        #endregion

        #region REPORTS 


        //[HttpPost]
        //public ActionResult CategoryReport(int[] ids)
        //{
        //    WarehouseTypeReport report = new WarehouseTypeReport();
        //    report.Parameters["id_company"].Value = this.ActiveCompanyId;
        //    return PartialView("_CategoryReport", report);
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult WarehouseTypeDetailReport(int id)
        //{
        //    WarehouseTypeDetailReport report = new WarehouseTypeDetailReport();
        //    report.Parameters["id__warehouseType"].Value = id;
        //    return PartialView("_WarehouseTypeReport", report);}

        #endregion

        #region AUXILIAR FUNCTIONS

        //public JsonResult ImportFileWarehouseType()
        //{
        //    if (Request.Files.Count > 0)
        //    {
        //        HttpPostedFileBase file = Request.Files[0];

        //        List<string> errorMessages = new List<string>();

        //        if (file != null)
        //        {
        //            string filename = Server.MapPath("~/App_Data/Temp/" + file.FileName);

        //            if (System.IO.File.Exists(filename))
        //            {
        //                System.IO.File.Delete(filename);
        //            }
        //            file.SaveAs(filename);

        //            Excel.Application application = new Excel.Application();
        //            Excel.Workbook workbook = application.Workbooks.Open(filename);

        //            if (workbook.Sheets.Count > 0)
        //            {
        //                Excel.Worksheet worksheet = workbook.ActiveSheet;
        //                Excel.Range table = worksheet.UsedRange;

        //                string code = string.Empty;
        //                string name = string.Empty;
        //                string description = string.Empty;

        //                using (DbContextTransaction trans = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        for (int i = 2; i < table.Rows.Count; i++)
        //                        {
        //                            Excel.Range row = table.Rows[i];
        //                            try
        //                            {
        //                                code = row.Cells[1].Text;
        //                                name = row.Cells[2].Text;
        //                                description = row.Cells[4].Text;
        //                            }
        //                            catch (Exception)
        //                            {
        //                                errorMessages.Add($"Error en formato de datos fila: {i}.");
        //                            }

        //                            WarehouseType wareHouseType = db.WarehouseType.FirstOrDefault(l => l.code.Equals(code));

        //                            if (wareHouseType == null)
        //                            {
        //                                wareHouseType = new WarehouseType
        //                                {
        //                                    code = code,
        //                                    name = name,
        //                                    description = description,
        //                                    isActive = true,

        //                                    id_company = this.ActiveCompanyId,
        //                                    id_userCreate = ActiveUser.id,
        //                                    dateCreate = DateTime.Now,
        //                                    id_userUpdate = ActiveUser.id,
        //                                    dateUpdate = DateTime.Now
        //                                };

        //                                db.WarehouseType.Add(wareHouseType);
        //                            }
        //                            else
        //                            {
        //                                wareHouseType.code = code;
        //                                wareHouseType.name = name;
        //                                wareHouseType.description = description;

        //                                wareHouseType.id_userUpdate = ActiveUser.id;
        //                                wareHouseType.dateUpdate = DateTime.Now;

        //                                db.WarehouseType.Attach(wareHouseType);
        //                                db.Entry(wareHouseType).State = EntityState.Modified;
        //                            }
        //                        }

        //                        db.SaveChanges();
        //                        trans.Commit();
        //                    }
        //                    catch (Exception)
        //                    {
        //                        trans.Rollback();
        //                    }
        //                }
        //            }

        //            application.Workbooks.Close();

        //            if (System.IO.File.Exists(filename))
        //            {
        //                System.IO.File.Delete(filename);
        //            }

        //            return Json(file?.FileName, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult ValidateCodeCategory(int id_category, string code)
        {
            ClientCategory category = db.ClientCategory.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_category);

            if (category == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Categoría" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Métodos para la importación de archivos

        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "CategoryArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditCategory()
        {
            return PartialView("_FormEditImportCategory");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportCategory()
        {
            var empresa = db.Company.FirstOrDefault(fod => fod.code == "01").id;
            return ItemTypeCategoryExcelFileParser.GetTemplateFileContentResult(empresa);
        }

        #endregion

        #region Configuración común para la carga de archivo de Excel con transacciones

        public class UploadControlSettings
        {
            public readonly static UploadControlValidationSettings ImageUploadValidationSettings;
            public readonly static UploadControlValidationSettings ExcelUploadValidationSettings;
            public readonly static UploadControlValidationSettings AnyDocumentUploadValidationSettings;

            static UploadControlSettings()
            {
                ImageUploadValidationSettings = new UploadControlValidationSettings()
                {
                    AllowedFileExtensions = new[] { ".jpe", ".jpeg", ".jpg", ".gif", ".png" },
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };

                ExcelUploadValidationSettings = new UploadControlValidationSettings()
                {
                    AllowedFileExtensions = new[] { ".xls", ".xlsx", ".xlsm" },
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };

                AnyDocumentUploadValidationSettings = new UploadControlValidationSettings()
                {
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };
            }

            public static void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
            {
                if (e.UploadedFile.IsValid)
                {
                    var fileId = FileUploadHelper.FileUploadProcess(e);

                    if (!String.IsNullOrEmpty(fileId))
                    {
                        var result = new
                        {
                            id = fileId,
                            filename = e.UploadedFile.FileName,
                        };

                        e.CallbackData = JsonConvert.SerializeObject(result);
                    }
                }
            }
        }

        #endregion

    }
}




