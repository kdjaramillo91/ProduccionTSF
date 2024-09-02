using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using DevExpress.Web;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ItemSizeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ItemSize GRIDVIEW
        [ValidateInput(false)]
        public ActionResult ItemSizesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ItemSize.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ItemSize.Where(it => it.id_company == this.ActiveCompanyId);
            return PartialView("_ItemSizesPartial", model.ToList());
            
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemSizesPartialAddNew(ItemSize item)
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

                        db.ItemSize.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Talla de Producto: " + item.name + " guardada exitosamente");
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

            var model = db.ItemSize.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemSizesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemSizesPartialUpdate(ItemSize item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemSize.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.ItemSize.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Talla de Producto: " + item.name + " guardada exitosamente");
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

            var model = db.ItemSize.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemSizesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemSizesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ItemSize.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemSize.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Talla de Prodcuto: " + (item?.name ?? "") + " desactivada exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.ItemSize.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemSizesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedItemSizes(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemSize.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemSize.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tallas de Productos desactivadas exitosamente");
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

            var model = db.ItemSize.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemSizesPartial", model.ToList());
        }
        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "ItemSizeArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItems()
        {
            return PartialView("_FormEditImportItemSize");
        }

      

        #endregion

        #region REPORTS ItemSizeReport


        [HttpPost]
        public ActionResult ItemSizeReport()
        {
            ItemSizeReport report = new ItemSizeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ItemSizeReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemSizeDetailReport(int id)
        {
            ItemSizeDetailReport report = new ItemSizeDetailReport();
            report.Parameters["id_ItemSize"].Value = id;
            return PartialView("_ItemSizeReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS
        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItems()
        {
            var empresa = this.ActiveCompanyId;
            return ItemSizeExcelFileParser.GetTemplateFileContentResult(empresa);
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
                var itemTypeParser = new ItemSizeExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemTypeParser.ParseItemSize(ActiveCompanyId, ActiveUser);

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

                message = "Error al importar las tallas de producto. " + ex.InnerException.Message;
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
            this.ViewBag.ReportTitle = "Errores en Importación de Tallas de Producto";
            this.ViewBag.ExcelFileName = $"ErroresImportacionItemSize_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
            this.ViewBag.ReportTitle = "Resultados de Importación de Tallas de Producto";
            this.ViewBag.ExcelFileName = $"ResultadosImportaciónTallaDeProducto_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
                                        ItemSize itemSize = db.ItemSize.FirstOrDefault(l => l.code.Equals(code));

                                        if (itemSize == null)
                                        {
                                            itemSize = new ItemSize
                                            {
                                                code = code,
                                                name = name,
                                                //id_inventoryLine = id_inventoryLine,
                                                description = description,
                                                isActive = true,

                                                id_company = this.ActiveCompanyId,
                                                id_userCreate = ActiveUser.id,
                                                dateCreate = DateTime.Now,
                                                id_userUpdate = ActiveUser.id,
                                                dateUpdate = DateTime.Now
                                            };

                                            db.ItemSize.Add(itemSize);
                                        }
                                        else
                                        {
                                            itemSize.code = code;
                                            itemSize.name = name;
                                           // itemSize.id_inventoryLine = id_inventoryLine;
                                            itemSize.description = description;

                                            itemSize.id_userUpdate = ActiveUser.id;
                                            itemSize.dateUpdate = DateTime.Now;

                                            db.ItemSize.Attach(itemSize);
                                            db.Entry(itemSize).State = EntityState.Modified;
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
        public JsonResult ImportFileItemSize()
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

                                    ItemSize itemSize = db.ItemSize.FirstOrDefault(l => l.code.Equals(code));

                                    if (itemSize == null)
                                    {
                                        itemSize = new ItemSize
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

                                        db.ItemSize.Add(itemSize);
                                    }
                                    else
                                    {
                                        itemSize.code = code;
                                        itemSize.name = name;
                                        itemSize.description = description;

                                        itemSize.id_userUpdate = ActiveUser.id;
                                        itemSize.dateUpdate = DateTime.Now;

                                        db.ItemSize.Attach(itemSize);
                                        db.Entry(itemSize).State = EntityState.Modified;
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
        public JsonResult ValidateCodeItemSize(int id_itemSize, string code)
        {
            ItemSize itemSize = db.ItemSize.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_itemSize);

            if (itemSize == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Talla de Producto" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region MyRegion

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

