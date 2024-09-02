using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{

    public class ItemPresentationController : DefaultController
    {

        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult PresentationResults(Presentation presentation,
                                                  int? id_metricUnit,
                                                  int? id_itemPackingMinimum,
                                                  int? id_itemPackingMaximum
                                                  )
        {

            var model = db.Presentation.ToList();

            #region  FILTERS

          

            if (id_metricUnit != null && id_metricUnit > 0)
            {
                model = model.Where(o => o.id_metricUnit == id_metricUnit).ToList();
            }



            if (id_itemPackingMinimum != null && id_itemPackingMinimum > 0)
            {
                model = model.Where(o => o.id_itemPackingMinimum == id_itemPackingMinimum).ToList();
            }


            if (id_itemPackingMaximum != null && id_itemPackingMaximum > 0)
            {
                model = model.Where(o => o.id_itemPackingMaximum == id_itemPackingMaximum).ToList();
            }


            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_PresentationResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }


        #endregion

        #region Calendar HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult PresentationPartial()
        {
            var model = (TempData["model"] as List<Presentation>);
            model = model ?? new List<Presentation>();
            TempData.Keep("model");
            return PartialView("_PresentationPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion

        #region Edit Presentation
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditPresentation(int id, int[] orderDetails)
        {
            Presentation Presentation = db.Presentation.Where(o => o.id == id).FirstOrDefault();

            if (Presentation == null)
            {

                Presentation = new Presentation
                {
                    id_company = ActiveUser.id_company,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                  
                    isActive = true,
                    minimum=0,
                    maximum=0,
                    Item2= new Item(),
                    Item1 = new Item(),
                    MetricUnit = new MetricUnit(),
                };
            }
            TempData["Presentation"] = Presentation;
            TempData.Keep("Presentation");

            return PartialView("_FormEditPresentation", Presentation);
        }
        #endregion

        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_Presentation)
        {
            TempData.Keep("Presentation");
            int index = db.Presentation.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_Presentation);
            var result = new
            {
                maximunPages = db.Presentation.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            Presentation Presentation = db.Presentation.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (Presentation != null)
            {
                TempData["Presentation"] = Presentation;
                TempData.Keep("Presentation");
                return PartialView("_PresentationMainFormPartial", Presentation);
            }

            TempData.Keep("Presentation");

            return PartialView("_PresentationMainFormPartial", new Presentation());
        }
        #endregion

        #region Validacion

        public Boolean validate(Presentation item)
        {
            Boolean wsreturn = true;
            try
            {
                Presentation conPresentation = (TempData["Presentation"] as Presentation);
            

                var cantre = db.Presentation.Where(x => x.id != item.id && x.code == item.code ).ToList().Count();
                if (cantre > 0)
                {
                    TempData.Keep("Presentation");
                    ViewData["EditMessage"] = ErrorMessage("Y existe un codigo");
                    wsreturn = false;
                }

                Item itemPackingMaximum = db.Item.Where(x => x.id == item.id_itemPackingMaximum).FirstOrDefault();

                Item itemPackingMinimum = db.Item.Where(x => x.id == item.id_itemPackingMinimum).FirstOrDefault();


               if( itemPackingMaximum.id_inventoryLine != itemPackingMinimum.id_inventoryLine)
                {
                    TempData.Keep("Presentation");
                    ViewData["EditMessage"] = ErrorMessage("Las Líneas de Inventario son diferentes");
                    wsreturn = false;
                }
               

                if (itemPackingMaximum.id_itemType != itemPackingMinimum.id_itemType)
                {
                    TempData.Keep("Presentation");
                    ViewData["EditMessage"] = ErrorMessage("Los Tipos de Productos son diferentes");
                    wsreturn = false;
                }

                if (itemPackingMaximum.id_itemTypeCategory != itemPackingMinimum.id_itemTypeCategory)
                {
                    TempData.Keep("Presentation");
                    ViewData["EditMessage"] = ErrorMessage("Los Tipos de Categoria del Producto son diferentes");
                    wsreturn = false;
                }


                if (itemPackingMaximum.id_itemWeight != itemPackingMinimum.id_itemWeight)
                {
                    TempData.Keep("Presentation");
                    ViewData["EditMessage"] = ErrorMessage("Los Pesos de los Productos son diferentes");
                    wsreturn = false;
                }


            }
            catch (Exception)
            {


            }

            return wsreturn;

        }
        #endregion

        #region Save and Update
        [HttpPost, ValidateInput(false)]
        public ActionResult PresentationPartialAddNew(bool approve, Presentation item)
        {
            Presentation conPresentation = (TempData["Presentation"] as Presentation);
    
            if (!validate(item))
            {
                return PartialView("_PresentationMainFormPartial", item);
            }
            DBContext dbemp = new DBContext();
          
            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region Presentation
                    item.Item1 = null;
                    item.Item2 = null;
                    #endregion

                    item.id_company = this.ActiveCompanyId;
                    item.id_userUpdate = ActiveUser.id;
                    item.id_userCreate = ActiveUser.id;
                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

                    if (approve)
                    { item.isActive = true; }

                    dbemp.Presentation.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    item.Item1 = dbemp.Item.Where(x => x.id == item.id_itemPackingMinimum).FirstOrDefault();
                    item.Item2 = dbemp.Item.Where(x => x.id == item.id_itemPackingMaximum).FirstOrDefault();
                    TempData["Presentation"] = item;
                    TempData.Keep("Presentation");
                    ViewData["EditMessage"] = SuccessMessage("Presentacion: " + item.code + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("Presentation");
                    item = (TempData["Presentation"] as Presentation);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_PresentationMainFormPartial", item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PresentationPartialUpdate(bool approve, Presentation item)
        {
            Presentation modelItem = db.Presentation.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                Presentation conPresentation = (TempData["Presentation"] as Presentation);

                if (!validate(item))
                {
                    return PartialView("_PresentationMainFormPartial", item);
                }

           

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Presentation
                        modelItem.id_company = this.ActiveCompanyId;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.code = item.code;
                        modelItem.name = item.name;
                        modelItem.isActive = item.isActive;
                        modelItem.id_itemPackingMaximum = item.id_itemPackingMaximum;
                        modelItem.id_itemPackingMinimum = item.id_itemPackingMinimum;
                        modelItem.id_metricUnit = item.id_metricUnit;
                        modelItem.description = item.description;
                        modelItem.minimum = item.minimum;
                        modelItem.maximum = item.maximum;
                        modelItem.Item1 = db.Item.Where(x => x.id == modelItem.id_itemPackingMinimum).FirstOrDefault();
                        modelItem.Item2 = db.Item.Where(x => x.id == modelItem.id_itemPackingMaximum).FirstOrDefault();
                        #endregion


                        if (approve)
                        {
                            modelItem.isActive = true;
                        }

                        db.Presentation.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["Presentation"] = modelItem;
                        TempData.Keep("Presentation");
                        ViewData["EditMessage"] = SuccessMessage("Presentacion: " + modelItem.code + " guardada exitosamente");

                    }
                    catch (Exception e)
                    {
                        TempData.Keep("Presentation");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("Presentation");
            return PartialView("_PresentationMainFormPartial", modelItem);
        }
        #endregion

        #region Presentation Gridview

        [ValidateInput(false)]
        public ActionResult PresentationPartial(int? id)
        {
            if (id != null)
            {
                ViewData["PresentationToCopy"] = db.Presentation.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.Presentation.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PresentationPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PresentationPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Presentation.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }

                        db.Presentation.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Presentacion : " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.Presentation.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PresentationPartial", model.ToList());
        }

        public ActionResult DeleteSelectedPresentation(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Presentations = db.Presentation.Where(i => ids.Contains(i.id));
                        foreach (var vPresentation in Presentations)
                        {
                            vPresentation.isActive = false;

                            vPresentation.id_userUpdate = ActiveUser.id;
                            vPresentation.dateUpdate = DateTime.Now;

                            db.Presentation.Attach(vPresentation);
                            db.Entry(vPresentation).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Presentacion desactivadas exitosamente");
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

            var model = db.Presentation.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_PresentationPartial", model.ToList());
        }

        #endregion

        #region Métodos para la importación de archivos

        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "PresentationArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItemPresentations()
        {
            return PartialView("_FormEditImportItemPresentation");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItemPresentations()
        {
            var empresa = this.ActiveCompanyId;
            return ItemPresentationExcelFileParser.GetTemplateFileContentResult(empresa);
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

                var itemTypeCategoryParser = new ItemPresentationExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemTypeCategoryParser.ParseItemPresentation(ActiveCompanyId, ActiveUser);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron las presentaciones de producto exitosamente";
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
                    ? "Error al importar las presentaciones. " + ex.InnerException.Message
                    : "Error al importar las presentaciones. " + ex.Message;
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
            this.ViewBag.ReportTitle = "Errores en Importación de Presentación";
            this.ViewBag.ExcelFileName = $"ErroresImportacion_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
            this.ViewBag.ReportTitle = "Resultados de Importación de Presentación";
            this.ViewBag.ExcelFileName = $"ResultadosImportacion_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
