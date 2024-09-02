using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXPANACEASOFT.Auxiliares.ExcelFileParsers
{
    internal class WarehouseLocationExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/WarehouseLocationTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores

        static WarehouseLocationExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }

        internal WarehouseLocationExcelFileParser(string contentFilePath, string contentFileMime)
            : base(contentFilePath, contentFileMime, true)
        {
        }

        private WarehouseLocationExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
            : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        #endregion

        #region Métodos para la importación de plantilla

        internal ImportResult ParseWarehouseLocation(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTypeColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarWarehouseLocations(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }

        private static string GetTablaItemTypeColumnNames()
        {
            string columnTypes = $"CODIGO,NOMBRE,ID_BODEGA,DESCRIPCION";
            return columnTypes;
        }

        private ImportResult ProcesarWarehouseLocations(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el Excel

            var warehouseLocationsCrear = new List<WarehouseLocation>();
            int numFilasVacias = 0;
            int numFila = 0;

            foreach (DataRow drCabecera in dtCabecera.Rows)
            {
                numFila++;

                if (numFilasVacias > 5) break;

                try
                {
                    // Verifico la primera información
                    if (!string.IsNullOrEmpty(drCabecera.GetString("CODIGO")))
                    {
                        var warehouseLocationSubir = new WarehouseLocation()
                        {
                            code = drCabecera.GetString("CODIGO"),
                            name = drCabecera.GetString("NOMBRE"),
                            description = drCabecera.GetString("DESCRIPCION"),
                            id_warehouse = drCabecera.GetInteger("ID_BODEGA"),
                            isActive = true,
                            id_company = activeCompanyId,
                            id_userCreate = activeUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = activeUser.id,
                            dateUpdate = DateTime.Now,                            
                        };

                        // Se agrega a la lista auxiliar
                        warehouseLocationsCrear.Add(warehouseLocationSubir);
                    }
                    else
                    {
                        numFilasVacias++;
                    }
                }
                catch (Exception ex)
                {
                    documentosFallidos.Add(new ImportResult.DocumentoFallido()
                    {
                        Filas = numFila.ToString(),
                        Descripcion = $"Error agregando fila a tabla de datos: Ubicaciones. Fila: {numFila}. {ex.Message}",
                    });
                }
            }

            if (!warehouseLocationsCrear.Any())
            {
                documentosFallidos.Add(new ImportResult.DocumentoFallido()
                {
                    //Filas = string.Empty, numFila.ToString(),
                    Filas = string.Empty,
                    Descripcion = $"El documento está vacío.",
                });
            }

            # endregion

            // Si hay documentosFallidos al poblar la lista, retornamos
            if (documentosFallidos.Any())
            {
                return new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = documentosFallidos.ToArray(),
                };
            }

            #region Validar datos

            for (int i = 0; i < warehouseLocationsCrear.Count; i++)
            {
                var warehouseLocationCrear = warehouseLocationsCrear.ElementAt(i);

                try
                {
                    // Validamos integridad de campos obligatorios
                    if (String.IsNullOrEmpty(warehouseLocationCrear.code))
                    {
                        throw new Exception("El código es obligatorio para crear ubicación.");
                    }

                    // Validamos longitud de código
                    if (warehouseLocationCrear.code.Length > 20)
                    {
                        throw new Exception("El código no debe tener longitud mayor a 20 caracteres.");
                    }

                    // Nombre 
                    if (String.IsNullOrEmpty(warehouseLocationCrear.name))
                    {
                        throw new Exception("El nombre es obligatorio para crear ubicación.");
                    }

                    // Validamos longitud de código
                    if (warehouseLocationCrear.name.Length > 250)
                    {
                        throw new Exception("El nombre no debe tener longitud mayor a 250 caracteres.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    if (warehouseLocationsCrear.Where(e => e.code == warehouseLocationCrear.code).Count() > 1)
                    {
                        throw new Exception($"El código: {warehouseLocationCrear.code} está repetido dentro del archivo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    var warehouseBase = db.WarehouseLocation.FirstOrDefault(e => e.code == warehouseLocationCrear.code);
                    if (warehouseBase != null)
                    {
                        throw new Exception($"El código: {warehouseLocationCrear.code} ya existe o está inactivo dentro de la base de datos.");
                    }
                }
                catch (Exception ex)
                {
                    documentosFallidos.Add(new ImportResult.DocumentoFallido()
                    {
                        Filas = (i + 1).ToString(),
                        Descripcion = $"Se debe corregir el error en la Fila: {i + 1}. Error: {ex.Message}",
                    });
                }
            }

            #endregion

            // Si hay documentosFallidos al poblar la lista, retornamos
            if (documentosFallidos.Any())
            {
                return new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = documentosFallidos.ToArray(),
                };
            }

            #region Guardar información en la base

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    db.WarehouseLocation.AddRange(warehouseLocationsCrear);

                    // Ingresa a la BD
                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = warehouseLocationsCrear.Count.ToString(),
                        NumDocumento = warehouseLocationsCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {warehouseLocationsCrear.Count} elementos."
                    });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    documentosFallidos.Add(new ImportResult.DocumentoFallido()
                    {
                        Descripcion = $"Error al guardar los registros en la base de datos. Error: {ex.Message}"
                    });
                }
            }

            #endregion

            return new ImportResult()
            {
                Importados = documentosImportados.ToArray(),
                Fallidos = documentosFallidos.ToArray(),
            };
        }

        #endregion

        #region Métodos para la descarga de plantillas

        internal static FileContentResult GetTemplateFileContentResult(int empresa)
        {
            // Generamos una copia del template original
            var contentId = CopyContentFile(m_TemplateAbsolutePath, ExcelXlsmMime);
            var contentPath = FileExcelUploadHelper.GetFileContentPath(contentId);
            var excelParser = new WarehouseLocationExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();

                    CopyWarehouse(empresa, cnn);

                    cnn.Close();
                }
            }
            catch (Exception exception)
            {
                Logging.LogException(exception);
            }

            // Recuperar los datos de la plantilla
            var content = FileExcelUploadHelper.ReadFileUpload(contentId,
                out var fileName, out string contentType, out _);

            try
            {
                fileName = Path.ChangeExtension(
                    $"Ubicacion_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
                    Path.GetExtension(fileName));

                return new FileContentResult(content, contentType)
                {
                    FileDownloadName = fileName,
                };
            }

            finally
            {
                FileExcelUploadHelper.CleanUpUploadedFile(contentId);
            }
        }

        private static void CopyWarehouse(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();

            // Recuperampos los datos de tipos de bodegas
            var bodegas = db.Warehouse
                .Where(e => e.isActive && e.id_company == empresa)
                .OrderBy(e => e.name);

            if (bodegas.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_BODEGA$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    foreach (var bodega in bodegas)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(bodega.name);
                        idParam.SetIntegerValue(bodega.id);
                        codigoParam.SetStringValue(bodega.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion
    }
}