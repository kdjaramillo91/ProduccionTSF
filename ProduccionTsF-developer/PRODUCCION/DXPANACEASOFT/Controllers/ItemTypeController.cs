using DevExpress.Web.Mvc;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using DXPANACEASOFT.Auxiliares;
using DevExpress.Web;
using Newtonsoft.Json;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ItemTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ItemType GRIDVIEW

        public ActionResult ItemTypesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ItemType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ItemType.Where(it => it.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTypesPartialAddNew(ItemType item)
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

                        db.ItemType.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipo de Producto: " + item.name + " guardado exitosamente");
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

            var model = db.ItemType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTypesPartialUpdate(ItemType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.id_inventoryLine = item.id_inventoryLine;

                            modelItem.name = item.name;
                            modelItem.code = item.code;

                            modelItem.description = item.description;

                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.ItemType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Tipo de Producto: " + item.name + " guardado exitosamente");
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

            var model = db.ItemType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTypesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ItemType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.ItemType.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipos de Productos: " + (item?.name ?? "") + " desactivado exitosamente");
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

            var model = db.ItemType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypesPartial", model.ToList());
        }


        [HttpPost]
        public ActionResult DeleteSelectedItemTypes(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemType.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipos de Productos desactivados exitosamente");
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

            var model = db.ItemType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypesPartial", model.ToList());
        }

        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "ItemTypeArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItems()
        {
            return PartialView("_FormEditImportItemType");
        }

        #endregion

        #region MASTER DETAILS VIEW

        public ActionResult ItemTypeDetailItemTypeCategoriesPartial(int? id_itemType)
        {
            ItemType itemType = db.ItemType.FirstOrDefault(o => o.id == id_itemType);
            //var model = itemType?.ItemTypeCategory?.ToList() ?? new List<ItemTypeCategory>();
            var model = db.ItemTypeCategory.Where(w => w.ItemTypeItemTypeCategory.Any(a => a.id_itemType == id_itemType)).ToList() ?? new List<ItemTypeCategory>();

            return PartialView("_ItemTypeDetailItemTypeCategoriesPartial", model);//.ToList());
        }

        #endregion

        #region REPORTS

        public ActionResult ItemTypeReport()
        {
            ItemTypeReport report = new ItemTypeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ItemReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTypeDetailReport(int id)
        {
            ItemTypeDetailReport report = new ItemTypeDetailReport();
            report.Parameters["id_itemType"].Value = id;
            return PartialView("_ItemReport", report);
        }
        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItems()
        {
            var empresa = this.ActiveCompanyId;
            return ProductTypeExcelFileParser.GetTemplateFileContentResult(empresa);
        }

        [HttpPost]
        public JsonResult ImportDatosCargaMasiva(string guidArchivoDatos)
        {
            bool isValid;
            string message;
            ImportResult resultadoImportacion;

            try
            {
                var contenidoArchivo = FileUploadHelper.ReadFileUpload(guidArchivoDatos,
                    out var nombreArchivo, out var mimeArchivo, out var rutaArchivoContenido);
                var itemTypeParser = new ProductTypeExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemTypeParser.ParseItemType(ActiveCompanyId, ActiveUser);

                FileUploadHelper.CleanUpUploadedFiles(guidArchivoDatos);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron los tipos de producto exitosamente";
            }
            catch (Exception ex)
            {
                isValid = false;

                resultadoImportacion = new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = new ImportResult.DocumentoFallido[] { },
                };

                message = "Error al importar los tipos de producto. " + ex.InnerException.Message;
            }

            // Preservamos los resultados de la importación de datos
            var guidResultado = Guid.NewGuid().ToString("n");
            this.TempData[$"documentos-importados-{guidResultado}"] = resultadoImportacion.Importados;
            this.TempData[$"documentos-fallidos-{guidResultado}"] = resultadoImportacion.Fallidos;

            ViewData["EditMessage"] = resultadoImportacion.Fallidos.Any() || !isValid
                ? ErrorMessage(message) : SuccessMessage(message);

            // Retornar el resultado...
            var result = new
            {
                isValid,
                message,
                guidResultado,
                HayErrores = resultadoImportacion.Fallidos.Any(),
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResult DownloadDocumentosFallidosImportacion(string guidResultado, string mensajeAlerta)
        {
            this.ViewBag.ReportCommand = "export";
            this.ViewBag.ReportTitle = "Errores en Importación de Tipos de Producto";
            this.ViewBag.ExcelFileName = $"ErroresImportacionItemTypes_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-fallidos-{guidResultado}";
            var documentosFallidos = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoFallido[]
                : new ImportResult.DocumentoFallido[] { };

            ViewData["EditMessage"] = ErrorMessage(mensajeAlerta);


            return this.View("_DocumentosFallidosImportacionReportPartial", documentosFallidos);
        }

        [HttpGet]
        public ViewResult DownloadDocumentosImportadosImportacion(string guidResultado, string mensajeAlerta)
        {
            this.ViewBag.ReportCommand = "export";
            this.ViewBag.ReportTitle = "Resultados de Importación de Tipos de Producto";
            this.ViewBag.ExcelFileName = $"ResultadosImportacionItemTypes_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-importados-{guidResultado}";
            var documentosImportados = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoImportado[]
                : new ImportResult.DocumentoImportado[] { };

            ViewData["EditMessage"] = SuccessMessage(mensajeAlerta);


            return this.View("_DocumentosImportadosImportacionReportPartial", documentosImportados);
        }

        [HttpPost]
        public JsonResult ImportFileItemType()
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
                        int id_inventoryLine = 0;
                        string description = string.Empty;

                        int numMaxVacios = 0;
                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 2; i < table.Rows.Count; i++)
                                {
                                    Excel.Range row = table.Rows[i]; // FILA i
                                    try
                                    {
                                        code = row.Cells[1].Text;
                                        name = row.Cells[2].Text;
                                        id_inventoryLine = int.Parse(row.Cells[5].Text);
                                        description = row.Cells[6].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    if (numMaxVacios >= 10) break;

                                    if (!String.IsNullOrEmpty(code))
                                    {
                                        ItemType itemType = db.ItemType.FirstOrDefault(l => l.code.Equals(code));

                                        if (itemType == null)
                                        {
                                            itemType = new ItemType
                                            {
                                                code = code,
                                                name = name,
                                                id_inventoryLine = id_inventoryLine,
                                                description = description,
                                                isActive = true,

                                                id_company = this.ActiveCompanyId,
                                                id_userCreate = ActiveUser.id,
                                                dateCreate = DateTime.Now,
                                                id_userUpdate = ActiveUser.id,
                                                dateUpdate = DateTime.Now
                                            };

                                            db.ItemType.Add(itemType);
                                        }
                                        else
                                        {
                                            itemType.code = code;
                                            itemType.name = name;
                                            itemType.id_inventoryLine = id_inventoryLine;
                                            itemType.description = description;

                                            itemType.id_userUpdate = ActiveUser.id;
                                            itemType.dateUpdate = DateTime.Now;

                                            db.ItemType.Attach(itemType);
                                            db.Entry(itemType).State = EntityState.Modified;
                                        }
                                    }
                                    else
                                    {
                                        numMaxVacios++;
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
        public JsonResult ValidateCodeItemType(int id_itemType, string code)
        {
            ItemType itemType = db.ItemType.FirstOrDefault(il => il.code == code);

            if (itemType == null || itemType.id == id_itemType){
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro tipo de producto" }, JsonRequestBehavior.AllowGet);
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



