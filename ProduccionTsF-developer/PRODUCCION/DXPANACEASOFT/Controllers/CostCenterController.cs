using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class CostCenterController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Cost Center GridView

        [ValidateInput(false)]
        public ActionResult CostCentersPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.CostCenter.FirstOrDefault(b => b.id == keyToCopy);
            }
           
            var model = db.CostCenter.Where(e => e.id_higherCostCenter == null);
            return PartialView("_CostCentersPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostCenterPartialAddNew(CostCenter costCenter)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        costCenter.dateCreate = DateTime.Now;
                        costCenter.id_userUpdate = ActiveUser.id;
                        costCenter.dateUpdate = DateTime.Now;

                        db.CostCenter.Add(costCenter);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Centro de costo : " + costCenter.name + " guardado exitosamente");
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

            var model = db.CostCenter.Where(e => e.id_higherCostCenter == null);
            return PartialView("_CostCentersPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostCenterPartialUpdate(CostCenter costCenter)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.CostCenter.FirstOrDefault(it => it.id == costCenter.id);
                        if (modelItem != null)
                        {

                            modelItem.code = costCenter.code;
                            modelItem.name = costCenter.name;
                            modelItem.description = costCenter.description;
                            modelItem.isActive = costCenter.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.CostCenter.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Centro de Costo: " + costCenter.name + " guardado exitosamente");
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

            var model = db.CostCenter.Where(e => e.id_higherCostCenter == null);
            return PartialView("_CostCentersPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostCenterPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.CostCenter.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.CostCenter.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] =
                                SuccessMessage("Centro de Costo: " + (item?.name ?? "") + " desactivado exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.CostCenter.Where(e => e.id_higherCostCenter == null);
            return PartialView("_CataloguesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedCostCenters(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.CostCenter.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.CostCenter.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Centros de costo desactivados exitosamente");
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

            var model = db.CostCenter.Where(e => e.id_higherCostCenter == null);
            return PartialView("_CostCenterPartial", model.ToList());
        }

        #endregion

        #region Métodos Auxiliares
        public JsonResult ValidateCodeCostCenter(int id_costCenter, string code)
        {
            CostCenter costCenter = db.CostCenter.FirstOrDefault(b => b.code == code && b.id != id_costCenter);

            // Verificación de información existente
            if (costCenter == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Modelo " }, JsonRequestBehavior.AllowGet);
        } 
        #endregion

        #region Métodos para la importación de archivos

        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "CostCenterArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditCostCenters()
        {
            return PartialView("_FormEditImportCostCenter");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateCostCenters()
        {
            var empresa = this.ActiveCompanyId;
            return CostCenterExcelFileParser.GetTemplateFileContentResult(empresa);
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

                var costCenterParser = new CostCenterExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = costCenterParser.ParseCostCenter(ActiveUser);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron los centros de costo exitosamente";
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
                    ? "Error al importar los centros de costo. " + ex.InnerException.Message
                    : "Error al importar los centros de costo. " + ex.Message;
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
            this.ViewBag.ReportTitle = "Errores en Importación de Bodegas";
            this.ViewBag.ExcelFileName = $"ErroresImportacionCCosto_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
            this.ViewBag.ReportTitle = "Resultados de Importación de Centros de Costo";
            this.ViewBag.ExcelFileName = $"ResultadosImportacionCCosto_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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

