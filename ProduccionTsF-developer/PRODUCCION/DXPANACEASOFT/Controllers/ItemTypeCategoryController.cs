using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using DevExpress.Web;
using Newtonsoft.Json;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ItemTypeCategoryController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region InventoryLine GRIDVIEW

        public ActionResult ItemTypeCategoriesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ItemTypeCategory.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ItemTypeCategory.Where(itc => itc.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypeCategoriesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTypeCategoriesPartialAddNew(ItemTypeCategory item)
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

                        #region ItemTypeItemTypeCategory

                        List<int> itemTypes = (TempData["itemTypes"] as List<int>);
                        itemTypes = itemTypes ?? new List<int>();

                        foreach (var i in itemTypes)
                        {
                            item.ItemTypeItemTypeCategory.Add(new ItemTypeItemTypeCategory
                            {
                                id_itemType = i,
                                ItemType = db.ItemType.FirstOrDefault(e => e.id == i),
                                id_itemTypeCategory = item.id,
                                ItemTypeCategory = item
                            });
                        }
                        #endregion

                        db.ItemTypeCategory.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] =
                            SuccessMessage("Categoría de Producto: " + item.name + " guardada exitosamente");
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

            var model = db.ItemTypeCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypeCategoriesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTypeCategoriesPartialUpdate(ItemTypeCategory item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemTypeCategory.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            //modelItem.id_inventoryLine = item.id_inventoryLine;
                            //modelItem.id_itemType = item.id_itemType;

                            modelItem.name = item.name;
                            modelItem.description = item.description;

                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            #region ItemTypeItemTypeCategory

                            for (int i = modelItem.ItemTypeItemTypeCategory.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.ItemTypeItemTypeCategory.ElementAt(i);

                                modelItem.ItemTypeItemTypeCategory.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            List<int> itemTypes = (TempData["itemTypes"] as List<int>);
                            itemTypes = itemTypes ?? new List<int>();

                            foreach (var i in itemTypes)
                            {
                                modelItem.ItemTypeItemTypeCategory.Add(new ItemTypeItemTypeCategory
                                {
                                    id_itemType = i,
                                    ItemType = db.ItemType.FirstOrDefault(e => e.id == i),
                                    id_itemTypeCategory = modelItem.id,
                                    ItemTypeCategory = modelItem
                                });
                            }
                            #endregion


                            db.ItemTypeCategory.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] =
                                SuccessMessage("Categoría de Producto: " + item.name + " guardada exitosamente");
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

            var model = db.ItemTypeCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypeCategoriesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTypeCategoriesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ItemTypeCategory.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.ItemTypeCategory.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Categorìa de Producto: " + (item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.ItemTypeCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypeCategoriesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSelectedItemTypeCategories(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemTypeCategory.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemTypeCategory.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Categorías de Productos desactivadas exitosamente");
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

            var model = db.ItemTypeCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemTypeCategoriesPartial", model.ToList());
        }

        #endregion


        #region REPORTS

        public ActionResult ItemTypeCategoryReport()
        {
            ItemTypeCategoryReport report = new ItemTypeCategoryReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ItemTypeCategoryReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTypeCategoryDetailReport(int id)
        {
            ItemTypeCategoryDetailReport report = new ItemTypeCategoryDetailReport();
            report.Parameters["id_itemTypeCategory"].Value = id;
            return PartialView("_ItemTypeCategoryReport", report);
        }


        #endregion

        #region AUXILIAR FUNCTIONS


        [HttpPost]
        public JsonResult ValidateCodeItemTypeCategory(int id_itemTypeCategory, string code)
        {
            ItemTypeCategory itemTypeCategory = db.ItemTypeCategory.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                                        && b.code == code
                                                                                        && b.id != id_itemTypeCategory);

            if (itemTypeCategory == null)
            {
                return Json(new {isValid = true, errorText = ""}, JsonRequestBehavior.AllowGet);
            }

            return Json(new {isValid = false, errorText = "Código en uso por otra Categoría de Tipos de Productos" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ImportFileItemTypeCategory()
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
                        int id_itemType = 0;
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
                                        code = row.Cells[1].Text; // COLUMNA 1
                                        name = row.Cells[2].Text;
                                        id_inventoryLine = int.Parse(row.Cells[3].Text);
                                        id_itemType = int.Parse(row.Cells[4].Text);
                                        description = row.Cells[5].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ItemTypeCategory itemTypeCategory =
                                        db.ItemTypeCategory.FirstOrDefault(l => l.code.Equals(code));

                                    if (itemTypeCategory == null)
                                    {
                                        itemTypeCategory = new ItemTypeCategory
                                        {
                                          
                                            code = code,
                                            name = name,
                                            //id_inventoryLine = id_inventoryLine,
                                            //id_itemType = id_itemType,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.ItemTypeCategory.Add(itemTypeCategory);
                                    }
                                    else
                                    {                                        
                                        itemTypeCategory.code = code;
                                        itemTypeCategory.name = name;
                                        //itemTypeCategory.id_inventoryLine = id_inventoryLine;
                                        //itemTypeCategory.id_itemType = id_itemType;
                                        itemTypeCategory.description = description;
                                        itemTypeCategory.id_userUpdate = ActiveUser.id;
                                        itemTypeCategory.dateUpdate = DateTime.Now;

                                        db.ItemTypeCategory.Attach(itemTypeCategory);
                                        db.Entry(itemTypeCategory).State = EntityState.Modified;
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

        [HttpPost, ValidateInput(false)]
        public JsonResult GetItemTypeCategoriesItemTypes(int? id_itemTypeCategory)
        {
            var itemTypeItemTypeCategoryAux = db.ItemTypeItemTypeCategory.Where(w => w.id_itemTypeCategory == id_itemTypeCategory);
            List<int> itemTypes = new List<int>();
            string list_itemTypesStr = "";
            foreach (var ititc in itemTypeItemTypeCategoryAux)
            {

                if (list_itemTypesStr == "") list_itemTypesStr = ititc.id_itemType.ToString();
                else list_itemTypesStr += "," + ititc.id_itemType.ToString();
                itemTypes.Add(ititc.id_itemType);
            }
            var result = new
            {
                itemTypes = list_itemTypesStr

            };

            TempData["itemTypes"] = itemTypes;
            TempData.Keep("itemTypes");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateItemTypes(int[] itemTypesCurrent)
        {
            List<int> itemTypes = new List<int>();
            foreach (var i in itemTypesCurrent)
            {
                if (i != 0)
                {
                    itemTypes.Add(i);
                }
            }

            var result = new
            {
                Message = "Ok"

            };

            TempData["itemTypes"] = itemTypes;

            TempData.Keep("itemTypes");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Métodos para la importación de archivos

        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "ItemTypeCategoryArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItemTypeCategory()
        {
            return PartialView("_FormEditImportItemTypeCategory");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItemTypeCategoria()
        {
            var empresa = this.ActiveCompanyId;
            return ItemTypeCategoryExcelFileParser.GetTemplateFileContentResult(empresa);
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

                var itemTypeCategoryParser = new ItemTypeCategoryExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemTypeCategoryParser.ParseItemTypeCategory(ActiveCompanyId, ActiveUser);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron las categorias exitosamente";
            }
            catch (Exception ex)
            {
                isValid = false;

                resultadoImportacion = new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = new ImportResult.DocumentoFallido[] { },
                };

                message = ex.InnerException != null 
                    ? "Error al importar las categorias de item. " + ex.InnerException.Message
                    : "Error al importar las categorias de item. " + ex.Message;
            }

            // Preservamos los resultados de la importación de datos
            var guidResultado = Guid.NewGuid().ToString("n");
            this.TempData[$"documentos-importados-{guidResultado}"] = resultadoImportacion.Importados;
            this.TempData[$"documentos-fallidos-{guidResultado}"] = resultadoImportacion.Fallidos;

            ViewData["EditMessage"] = resultadoImportacion.Fallidos.Any() || ! isValid
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
            this.ViewBag.ReportTitle = "Errores en Importación de Categoría de Producto";
            this.ViewBag.ExcelFileName = $"ErroresImportacionItemTypeCategory_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
            this.ViewBag.ReportTitle = "Resultados de Importación de Categoría de Producto";
            this.ViewBag.ExcelFileName = $"ResultadosImportacionItemTypeCategory_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-importados-{guidResultado}";
            var documentosImportados = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoImportado[]
                : new ImportResult.DocumentoImportado[] { };

            ViewData["EditMessage"] = SuccessMessage(mensajeAlerta);


            return this.View("_DocumentosImportadosImportacionReportPartial", documentosImportados);
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





