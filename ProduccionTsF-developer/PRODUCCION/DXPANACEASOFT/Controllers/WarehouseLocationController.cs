using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using DevExpress.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.DataProviders;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class WarehouseLocationController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region WarehouseLocation GRIDVIEW
        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseLocationsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.WarehouseLocation.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.WarehouseLocation.Where(whl => whl.id_company == this.ActiveCompanyId);
            return PartialView("_WarehouseLocationPartial", model.ToList().OrderByDescending(ob=> ob.id));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseLocationsPartialAddNew(WarehouseLocation item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var warehouseType = db.Warehouse.FirstOrDefault(fod => fod.id == item.id_warehouse).WarehouseType;
                        var person = db.Person.FirstOrDefault(fod => fod.id == item.id_person);

                        this.GeneratePersonWarehouseValues(item.id_warehouse, item.id_person,
                            item.code, item.name, item.description, out var code, out var warehouseLocationName,
                            out var description);

                        item.code = code;
                        item.name = warehouseLocationName;
                        item.description = description;

                        item.isRolling = person != null ? false : item.isRolling;
                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.WarehouseLocation.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Ubicación: " + item.name + " guardada exitosamente");
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

            var model = db.WarehouseLocation.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_WarehouseLocationPartial", model.ToList().OrderByDescending(ob => ob.id));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseLocationsPartialUpdate(WarehouseLocation item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.WarehouseLocation.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {                            

                            this.GeneratePersonWarehouseValues(item.id_warehouse, item.id_person,
                                item.code, item.name, item.description, out var code, out var warehouseLocationName,
                                out var description);

                            // Verifica que la bodega tenga movimientos de inventario
                            bool tieneMovimientosBodega = db.InventoryMove.Any(e => e.idWarehouse == modelItem.id_warehouse);
                            // Verifica que la ubicación tenga movimientos de inventario
                            bool iteneMovimientosUbicacion = db.InventoryMoveDetail.Any(e => e.id_warehouse == modelItem.id_warehouse && e.id_warehouseLocation == modelItem.id);
                            // Verifica que nueva bodega sea diferente a la inical
                            bool bodegaDiferenteInicial = modelItem.id_warehouse != item.id_warehouse;

                            var bodega = db.Warehouse.FirstOrDefault(e => e.id == modelItem.id_warehouse);

                            if (tieneMovimientosBodega && iteneMovimientosUbicacion && bodegaDiferenteInicial)
                            {
                                throw new Exception($"No es posible cambiar la bodega {bodega.code} de la ubicación {item.code}, " +
                                    $"dado que presenta movimientos de inventarios relacionados.");
                            }

                            //if(modelItem.id_warehouse != item.id_warehouse && db.InventoryMove.Any(e => e.idWarehouse == modelItem.id_warehouse))
                            //{
                            //    throw new Exception($"No es posible cambiar la bodega {bodega.code} de la ubicación {item.code}, " +
                            //        $"dado que presenta movimientos de inventarios relacionados.");
                            //}

                            modelItem.code = code;
                            modelItem.name = warehouseLocationName;
                            modelItem.description = description;

                            modelItem.id_warehouse = item.id_warehouse;
                            modelItem.id_person = item.id_person;

                            var warehouseType = db.Warehouse.FirstOrDefault(fod => fod.id == modelItem.id_warehouse).WarehouseType;
                            var person = db.Person.FirstOrDefault(fod => fod.id == modelItem.id_person);
                            modelItem.isRolling = person != null ? false : item.isRolling;

                            //modelItem.isRolling = item.isRolling;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.WarehouseLocation.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Ubicación: " + item.name + " guardada exitosamente");
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                        ViewData["EditError"] = ex.InnerException?.Message ?? ex.Message;
                        //ViewData["EditError"] = !string.IsNullOrEmpty(e.InnerException?.Message) ? e.InnerException?.Message : e.Message;
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.WarehouseLocation.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_WarehouseLocationPartial", model.ToList().OrderByDescending(ob => ob.id));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseLocationsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.WarehouseLocation.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }

                        db.WarehouseLocation.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Ubicación: " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception ex)
                    {
                        ViewData["EditError"] = ex.InnerException?.Message ?? ex.Message;
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.WarehouseLocation.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_WarehouseLocationPartial", model.ToList().OrderByDescending(ob => ob.id));
        }

        [HttpPost]
        public ActionResult DeleteSelectedWarehouseLocations(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.WarehouseLocation.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.WarehouseLocation.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Ubicaciones desactivadas exitosamente");
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

            var model = db.WarehouseLocation.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_WarehouseLocationPartial", model.ToList().OrderByDescending(ob => ob.id));
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult WarehouseLocationReport()
        {
            WarehouseLocationReport report = new WarehouseLocationReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_WarehouseLocationReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseLocationDetailReport(int id)
        {
            WarehouseLocationDetailReport report = new WarehouseLocationDetailReport();
            report.Parameters["id_warehouseLocation"].Value = id;
            return PartialView("_WarehouseLocationReport", report);
        }

        #endregion

        #region AUXILIAR FUNCTIONS


        [HttpPost]
        public JsonResult ValidateCodeWarehouseLocation(int id_warehouseLocation, string code)
        {
            WarehouseLocation warehouseLocation = db.WarehouseLocation.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_warehouseLocation);

            if (warehouseLocation == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Ubicación" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileWarehouseLocation()
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
                        int id_warehouse = 0;
                        bool isRolling = false;

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
                                        id_warehouse = int.Parse(row.Cells[4].Text);
                                        isRolling = row.Cells[5].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    WarehouseLocation warehouseLocationImport = db.WarehouseLocation.FirstOrDefault(l => l.code.Equals(code));

                                    if (warehouseLocationImport == null)
                                    {
                                        warehouseLocationImport = new WarehouseLocation
                                        {
                                            code = code,
                                            name = name,
                                            id_warehouse = id_warehouse,
                                            isRolling = isRolling,
                                            description = description,

                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.WarehouseLocation.Add(warehouseLocationImport);
                                    }
                                    else
                                    {
                                        warehouseLocationImport.code = code;
                                        warehouseLocationImport.name = name;
                                        warehouseLocationImport.id_warehouse = id_warehouse;
                                        warehouseLocationImport.isRolling = isRolling;
                                        warehouseLocationImport.description = description;

                                        warehouseLocationImport.id_userUpdate = ActiveUser.id;
                                        warehouseLocationImport.dateUpdate = DateTime.Now;

                                        db.WarehouseLocation.Attach(warehouseLocationImport);
                                        db.Entry(warehouseLocationImport).State = EntityState.Modified;
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
        public ActionResult GetPersonWarehouseLocation(int? id_warehouse)
        {
            Warehouse warehoseAux = db.Warehouse.FirstOrDefault(i => i.id == id_warehouse);
            warehoseAux = warehoseAux ?? new Warehouse();
            var idsRoles = !String.IsNullOrEmpty(warehoseAux.ids_Roles)
                ? warehoseAux.ids_Roles.Split('|').Select(e => Convert.ToInt32(e)).ToArray()
                : new int[] { };

            var warehouseType = warehoseAux.WarehouseType ?? new WarehouseType();

            Person[] persons;
            if (warehoseAux?.requirePerson ?? false)
            {
                persons = DataProviderPerson.PersonsByCompanyRolsForWarehouseLocations(this.ActiveCompanyId, idsRoles, null);
            }
            else
            {
                persons = new Person[] {};
            }

            return GridViewExtension.GetComboBoxCallbackResult(p => {
                p.ClientInstanceName = "id_person";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.NullText = "Seleccione la Persona";
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "WarehouseLocation", Action = "GetPersonWarehouseLocation" };
                p.ClientSideEvents.BeginCallback = "OnWarehouseLocationsWarehouse_BeginCallback";

                p.ClientSideEvents.SelectedIndexChanged = "OnWarehouseLocationsWarehouse_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnWarehouseLocationsPersonValidation";
                p.ClientSideEvents.Init = "OnWarehouseLocationsPerson_Init";
                p.CallbackPageSize = 5;

                p.ValueField = "id";
                p.TextField = "fullname_businessName";
                p.ValueType = typeof(int);

                p.BindList(persons);

            });

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetCodeWarehouseType(int? id_warehouse)
        {
            var warehouse = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouse);

            var result = new
            {
                Message = "Ok",
                codeWarehouseType = warehouse?.WarehouseType?.code ?? "",
                requirePerson = warehouse?.requirePerson ?? false,
            };
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetDataPerson(int? id_person, int? id_warehouse)
        {

            this.GeneratePersonWarehouseValues(id_warehouse, id_person,
                null, null, null, out var code, out var warehouseLocationName,
                out var description);

            var result = new
            {
                Message = "Ok",
                code,
                warehouseLocationName,
                description,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        private void GeneratePersonWarehouseValues(int? id_warehouse, int? id_person,
            string codigo, string nombre, string descripcion, out string codigoRetorno,
            out string nombreRetorno, out string descripcionRetorno)
        {
            if(!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(nombre))
            {
                codigoRetorno = codigo;
                nombreRetorno = nombre;
                descripcionRetorno = descripcion;

                return;
            }


           

            var person = db.Person.FirstOrDefault(fod => fod.id == id_person);
            var idsRolesString = db.Warehouse
                .FirstOrDefault(e => e.id == id_warehouse)?
                .ids_Roles ?? String.Empty;
            var idsRoles = idsRolesString.Split('|').Select(e => Convert.ToInt32(e)).ToArray();
            var roles = db.Rol.Where(e => idsRoles.Contains(e.id));
            var nombres = roles.Select(e => e.name).Distinct().ToArray();
            string caption = string.Join(" - ", nombres);

            var codigos = nombres
                .Select(e => e.Length >= 3 ? e.Substring(0, 3).ToUpper() : e.ToUpper())
                .ToArray();

            var nameAux = $"{caption}: {person?.fullname_businessName ?? "Sin nombre"}";

            codigoRetorno = person != null
                               ? $"{string.Join("-", codigos)}{person.id}"
                               : "SINCODIGO";
            nombreRetorno = nameAux;
            descripcionRetorno = nameAux;
        } 
        #endregion

        #region Métodos para la importación de archivos

        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "WarehouseLocationArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditWarehouseLocations()
        {
            return PartialView("_FormEditImportWarehouseLocation");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportWarehouseLocations()
        {
            var empresa = this.ActiveCompanyId;
            return WarehouseLocationExcelFileParser.GetTemplateFileContentResult(empresa);
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

                var warehouseLocationParser = new WarehouseLocationExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = warehouseLocationParser.ParseWarehouseLocation(ActiveCompanyId, ActiveUser);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron las ubicaciones exitosamente";
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
                    ? "Error al importar las ubicaciones. " + ex.InnerException.Message
                    : "Error al importar las ubicaciones. " + ex.Message;
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
            this.ViewBag.ReportTitle = "Errores en Importación de ubicaciones";
            this.ViewBag.ExcelFileName = $"ErroresImportacionWarehouseLocation_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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
            this.ViewBag.ReportTitle = "Resultados de Importación de ubicaciones";
            this.ViewBag.ExcelFileName = $"ResultadosImportacionWarehouseLocation_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

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



