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
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using Newtonsoft.Json;
using DevExpress.Web;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ItemGroupCategoryController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ItemGroupCategory GRIDVIEW
        [ValidateInput(false)]
        public ActionResult ItemGroupCategoriesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ItemGroupCategory.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ItemGroupCategory.Where(igc => igc.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupCategoriesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemGroupCategoriesPartialAddNew(ItemGroupCategory item)
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

                        #region ItemGroupItemGroupCategory

                        List<int> itemGroups = (TempData["itemGroups"] as List<int>);
                        itemGroups = itemGroups ?? new List<int>();
                        
                        foreach (var i in itemGroups)
                        {
                            item.ItemGroupItemGroupCategory.Add(new ItemGroupItemGroupCategory
                            {
                                id_itemGroup = i,
                                ItemGroup = db.ItemGroup.FirstOrDefault(e => e.id == i),
                                id_itemGroupCategory = item.id,
                                ItemGroupCategory = item
                            });
                        }
                        #endregion


                        db.ItemGroupCategory.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Categoría de Grupos de Productos: " + item.name + " guardado exitosamente");
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

            var model = db.ItemGroupCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupCategoriesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemGroupCategoriesPartialUpdate(ItemGroupCategory item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemGroupCategory.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            //modelItem.id_itemGroup = item.id_itemGroup;
                            modelItem.name = item.name;
                            modelItem.description = item.description;modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            #region ItemGroupItemGroupCategory

                            for (int i = modelItem.ItemGroupItemGroupCategory.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.ItemGroupItemGroupCategory.ElementAt(i);

                                modelItem.ItemGroupItemGroupCategory.Remove(detail);
                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            List<int> itemGroups = (TempData["itemGroups"] as List<int>);
                            itemGroups = itemGroups ?? new List<int>();

                            foreach (var i in itemGroups)
                            {
                                modelItem.ItemGroupItemGroupCategory.Add(new ItemGroupItemGroupCategory
                                {
                                    id_itemGroup = i,
                                    ItemGroup = db.ItemGroup.FirstOrDefault(e => e.id == i),
                                    id_itemGroupCategory = modelItem.id,
                                    ItemGroupCategory = modelItem
                                });
                            }
                            #endregion

                            db.ItemGroupCategory.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Categoría de Grupos de Productos: " + item.name + " guardado exitosamente");
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

            var model = db.ItemGroupCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupCategoriesPartial", model.ToList());

        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ItemGroupCategoriesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ItemGroupCategory.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemGroupCategory.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Categoría de Grupos de Productos: " + item.name + " desactivada exitosamente");
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

            var model = db.ItemGroupCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupCategoriesPartial", model.ToList());

        }

        [HttpPost]
        public ActionResult DeleteSelectedItemGroupCategories(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemGroupCategory.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemGroupCategory.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Categorías de Grupos de Productos desactivada exitosamente");
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

            var model = db.ItemGroupCategory.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupCategoriesPartial", model.ToList());
        }

        #endregion


        #region REPORT

        [HttpPost]
        public ActionResult ItemGroupCategoryReport()
        {
            ItemGroupCategoryReport report = new ItemGroupCategoryReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ItemGroupCategoryReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemGroupCategoryDetailReport(int id)
        {
            ItemGroupCategoryDetailReport report = new ItemGroupCategoryDetailReport();
            report.Parameters["id_itemGroupCategory"].Value = id;
            return PartialView("_ItemGroupCategoryReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public ActionResult ComboBoxItemSubGroupsPartial()
        {
            int id_parentGroup = (Request.Params["id_parentGroup"] != null && Request.Params["id_parentGroup"] != "") ? int.Parse(Request.Params["id_parentGroup"]) : -1;
            return PartialView("_ComboBoxItemSubGroupsPartial", DataProviderItemGroup.ItemSubGroupsOfGroup(id_parentGroup));
        }

        [HttpPost]
        public JsonResult ItemGroupByCompany(int id_company)
        {
            var model = db.ItemGroup.Where(d => d.id_company == id_company && d.isActive).ToList();

            var result = model.Select(d => new { d.id, d.name });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateCodeItemGroupCategory(int id_itemGroupCategory, string code)
        {
            ItemGroupCategory itemGroupCategory = db.ItemGroupCategory.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_itemGroupCategory);

            if (itemGroupCategory == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Categoría de Grupos de Productos " }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileItemGroupCategory()
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
                        int id_itemGroup = 0;
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
                                        id_itemGroup = int.Parse(row.Cells[3].Text);
                                        description = row.Cells[4].Text;


                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ItemGroupCategory itemGroupCategory = db.ItemGroupCategory.FirstOrDefault(l => l.code.Equals(code));

                                    if (itemGroupCategory == null)
                                    {
                                        itemGroupCategory = new ItemGroupCategory
                                        {
                                            code = code,
                                            name = name,
                                            //id_itemGroup = id_itemGroup,
                                            description = description,

                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.ItemGroupCategory.Add(itemGroupCategory);
                                    }
                                    else
                                    {
                                        itemGroupCategory.code = code;
                                        itemGroupCategory.name = name;
                                        //itemGroupCategory.id_itemGroup = id_itemGroup;
                                        itemGroupCategory.description = description;

                                        itemGroupCategory.id_userUpdate = ActiveUser.id;
                                        itemGroupCategory.dateUpdate = DateTime.Now;

                                        db.ItemGroupCategory.Attach(itemGroupCategory);
                                        db.Entry(itemGroupCategory).State = EntityState.Modified;
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
        public JsonResult GetItemGroupCategoriesItemGroups(int? id_itemGroupCategory)
        {
            var itemGroupItemGroupCategoryAux = db.ItemGroupItemGroupCategory.Where(w=> w.id_itemGroupCategory == id_itemGroupCategory);
            List<int> itemGroups = new List<int>();
            string list_itemGroupsStr = "";
            foreach (var igigc in itemGroupItemGroupCategoryAux)
            {
                
                if (list_itemGroupsStr == "") list_itemGroupsStr = igigc.id_itemGroup.ToString();
                else list_itemGroupsStr += "," + igigc.id_itemGroup.ToString();
                itemGroups.Add(igigc.id_itemGroup);
            }
            var result = new
            {
                itemGroups = list_itemGroupsStr

            };

            TempData["itemGroups"] = itemGroups;
            TempData.Keep("itemGroups");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateItemGroups(int[] itemGroupsCurrent)
        {
            List<int> itemGroups = new List<int>();
            //itemGroups = itemGroups ?? int[];
            foreach (var i in itemGroupsCurrent)
            {
                if (i != 0)
                {
                    itemGroups.Add(i);
                    //if (list_idInventaryLineFilterStr == "") list_idInventaryLineFilterStr = i.ToString();
                    //else list_idInventaryLineFilterStr += "," + i.ToString();
                }
            }

            var result = new
            {
                Message = "Ok"

            };

            TempData["itemGroups"] = itemGroups;

            TempData.Keep("itemGroups");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Importación
        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "ItemGroupCategoryArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItemGroupCategory()
        {
            return PartialView("_FormEditImportItemGroupCategory");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItemGroupCategoria()
        {
            var empresa = this.ActiveCompanyId;
            return ItemGroupCategoryExcelFileParser.GetTemplateFileContentResult(empresa);
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

                var itemTypeCategoryParser = new ItemGroupCategoryExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemTypeCategoryParser.ParseItemGroupCategory(ActiveCompanyId, ActiveUser);

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
            this.ViewBag.ReportTitle = "Errores en Importación de Categoría de Producto";
            this.ViewBag.ExcelFileName = $"ErroresImportacionItemGroupCategory_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
            this.ViewBag.ExcelFileName = $"ResultadosImportaciónGrupoDeCategoríaDeProducto_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-importados-{guidResultado}";
            var documentosImportados = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoImportado[]
                : new ImportResult.DocumentoImportado[] { };

            ViewData["EditMessage"] = SuccessMessage(mensajeAlerta);


            return this.View("_DocumentosImportadosImportacionReportPartial", documentosImportados);
        }




        #endregion

        #region configuración común para la carga de excel

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

