using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ItemTrademarkModelController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ItemTrademarkModel GRIDVIEW
        [ValidateInput(false)]
        public ActionResult ItemTrademarkModelsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ItemTrademarkModel.FirstOrDefault(b => b.id == keyToCopy);
            }
           
            var model = db.ItemTrademarkModel.Where(itm => itm.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTrademarkModelsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTrademarkModelsPartialAddNew(ItemTrademarkModel item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_company = this.ActiveCompanyId;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.ItemTrademarkModel.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Modelo : " + item.name + " guardado exitosamente");
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

            var model = db.ItemTrademarkModel.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTrademarkModelsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTrademarkModelsPartialUpdate(ItemTrademarkModel item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemTrademarkModel.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.id_trademark = item.id_trademark;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.ItemTrademarkModel.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Modelo: " + item.name + " guardado exitosamente");
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

            var model = db.ItemTrademarkModel.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTrademarkModelsPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTrademarkModelsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ItemTrademarkModel.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemTrademarkModel.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] =
                                SuccessMessage("Modelo: " + (item?.name ?? "") + " desactivado exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.ItemTrademarkModel.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTrademarkModelsPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedItemTrademarkModels(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemTrademarkModel.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemTrademarkModel.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Modelos desactivados exitosamente");
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

            var model = db.ItemTrademarkModel.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTrademarkModelsPartial", model.ToList());
        }

        #endregion
        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "ItemTrademarkModelArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItems()
        {
            return PartialView("_FormEditImportItemTrademarkModel");
        }

        #region REPORTS

        [HttpPost]
        public ActionResult ItemTradeMarkModelReport(int[] ids)
        {
            ItemTradeMarkModelReport report = new ItemTradeMarkModelReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ItemTrademarkModelReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTradeMarkModelDetailReport(int id)
        {
            ItemTradeMarkModelDetailReport report = new ItemTradeMarkModelDetailReport();
            report.Parameters["id_itemTradeMarkModel"].Value = id;
            return PartialView("_ItemTrademarkModelReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItems()
        {
            var empresa = this.ActiveCompanyId;
            //cambiar el parser
            return ItemTrademarkModelExcelFileParser.GetTemplateFileContentResult(empresa);
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
                var itemTypeParser = new ItemTrademarkModelExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemTypeParser.ParseItemType(ActiveCompanyId, ActiveUser);

                FileUploadHelper.CleanUpUploadedFiles(guidArchivoDatos);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron los Modelos exitosamente";
            }
            catch (Exception ex)
            {
                isValid = false;

                resultadoImportacion = new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = new ImportResult.DocumentoFallido[] { },
                };

                message = "Error al importar los Modelos. " + ex.InnerException.Message;
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
            this.ViewBag.ReportTitle = "Errores en Importación de Modelos";
            this.ViewBag.ExcelFileName = $"ErroresImportacionItemTrademarkModels_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
            this.ViewBag.ReportTitle = "Resultados de Importación de Modelos";
            this.ViewBag.ExcelFileName = $"ResultadosImportacionItemTrademarkModels_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-importados-{guidResultado}";
            var documentosImportados = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoImportado[]
                : new ImportResult.DocumentoImportado[] { };

            ViewData["EditMessage"] = SuccessMessage(mensajeAlerta);


            return this.View("_DocumentosImportadosImportacionReportPartial", documentosImportados);
        }

        [HttpPost]
        public JsonResult ValidateCodeItemTrademarkModel(int id_itemTrademarkModel, string code)
        {
            ItemTrademarkModel itemTrademarkModel = db.ItemTrademarkModel.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code  
                                                                            && b.id != id_itemTrademarkModel);

            if (itemTrademarkModel == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Modelo " }, JsonRequestBehavior.AllowGet);
        }
       

        [HttpPost]
        public JsonResult ImportFileItemTradeMarkModel()
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
                        int id_trademark = 0;
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
                                        id_trademark = int.Parse(row.Cells[3].Text);
                                        description = row.Cells[4].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ItemTrademarkModel itemTrademarkModelImport = db.ItemTrademarkModel.FirstOrDefault(l => l.code.Equals(code));

                                    if (itemTrademarkModelImport == null)
                                    {
                                        itemTrademarkModelImport = new ItemTrademarkModel
                                        {
                                            code = code,
                                            name = name,
                                            id_trademark = id_trademark,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.ItemTrademarkModel.Add(itemTrademarkModelImport);
                                    }
                                    else
                                    {
                                        itemTrademarkModelImport.code = code;
                                        itemTrademarkModelImport.name = name;
                                        itemTrademarkModelImport.id_trademark = id_trademark;
                                        itemTrademarkModelImport.description = description;

                                        itemTrademarkModelImport.id_userUpdate = ActiveUser.id;
                                        itemTrademarkModelImport.dateUpdate = DateTime.Now;

                                        db.ItemTrademarkModel.Attach(itemTrademarkModelImport);
                                        db.Entry(itemTrademarkModelImport).State = EntityState.Modified;
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

        #endregion



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
    }
}

