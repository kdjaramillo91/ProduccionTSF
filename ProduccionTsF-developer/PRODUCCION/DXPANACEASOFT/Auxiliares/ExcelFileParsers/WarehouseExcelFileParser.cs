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
    internal class WarehouseExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/WarehouseTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores

        static WarehouseExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }

        internal WarehouseExcelFileParser(string contentFilePath, string contentFileMime)
            : base(contentFilePath, contentFileMime, true)
        {
        }

        private WarehouseExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
            : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        #endregion

        #region Métodos para la importación de plantilla

        internal ImportResult ParseWarehouse(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTypeColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarWarehouses(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }

        private static string GetTablaItemTypeColumnNames()
        {
            string columnTypes = $"CODIGO,NOMBRE,ID_TIPO_BODEGA,ID_LINEA_INVENTARIO,DESCRIPCION";
            return columnTypes;
        }

        private ImportResult ProcesarWarehouses(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el Excel

            var warehousesCrear = new List<Warehouse>();
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
                        var warehouseSubir = new Warehouse()
                        {
                            code = drCabecera.GetString("CODIGO"),
                            name = drCabecera.GetString("NOMBRE"),
                            description = drCabecera.GetString("DESCRIPCION"),
                            id_warehouseType = drCabecera.GetInteger("ID_TIPO_BODEGA"),
                            id_inventoryLine = drCabecera.GetInteger("ID_LINEA_INVENTARIO"),
                            isActive = true,
                            id_company = activeCompanyId,
                            id_userCreate = activeUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = activeUser.id,
                            dateUpdate = DateTime.Now
                        };

                        // Se agrega a la lista auxiliar
                        warehousesCrear.Add(warehouseSubir);
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
                        Descripcion = $"Error agregando fila a tabla de datos: Bodega. Fila: {numFila}. {ex.Message}",
                    });
                }
            }

            if (!warehousesCrear.Any())
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

            for (int i = 0; i < warehousesCrear.Count; i++)
            {
                var warehouseCrear = warehousesCrear.ElementAt(i);

                try
                {
                    // Validamos integridad de campos obligatorios
                    if (String.IsNullOrEmpty(warehouseCrear.code))
                    {
                        throw new Exception("El código es obligatorio para crear bodega.");
                    }

                    // Validamos longitud de código
                    if (warehouseCrear.code.Length > 20)
                    {
                        throw new Exception("El código no debe tener longitud mayor a 20 caracteres.");
                    }

                    // Nombre de la bodega
                    if (String.IsNullOrEmpty(warehouseCrear.name))
                    {
                        throw new Exception("El nombre es obligatorio para crear bodega.");
                    }

                    // Validamos longitud de nombre
                    if (warehouseCrear.name.Length > 50)
                    {
                        throw new Exception("El nombre no debe tener longitud mayor a 50 caracteres.");
                    }

                    // Tipo de Bodega
                    if (warehouseCrear.id_warehouseType <= 0)
                    {
                        throw new Exception("El tipo de bodega es obligatorio para crear bodega.");
                    }

                    // Linea de inventario
                    if (warehouseCrear.id_inventoryLine <= 0)
                    {
                        throw new Exception("La línea de inventario es obligatorio para crear bodega.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    if (warehousesCrear.Where(e => e.code == warehouseCrear.code).Count() > 1)
                    {
                        throw new Exception($"El código: {warehouseCrear.code} está repetido dentro del archivo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    var warehouseBase = db.Warehouse.FirstOrDefault(e => e.code == warehouseCrear.code);
                    if (warehouseBase != null)
                    {
                        throw new Exception($"El código: {warehouseCrear.code} ya existe o está inactivo dentro de la base de datos.");
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
                    db.Warehouse.AddRange(warehousesCrear);

                    // Ingresa a la BD
                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = warehousesCrear.Count.ToString(),
                        NumDocumento = warehousesCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {warehousesCrear.Count} elementos."
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
            var excelParser = new WarehouseExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();

                    CopyWarehouseType(empresa, cnn);
                    CopyLineaInventarioData(empresa, cnn);

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
                    $"Bodega_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
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

        private static void CopyWarehouseType(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();

            // Recuperampos los datos de tipos de bodegas
            var tipoBodegas = db.WarehouseType
                .Where(e => e.isActive && e.id_company == empresa)
                .OrderBy(e => e.name);

            if (tipoBodegas.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_TIPO_BODEGA$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    foreach (var tipoBodega in tipoBodegas)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(tipoBodega.name);
                        idParam.SetIntegerValue(tipoBodega.id);
                        codigoParam.SetStringValue(tipoBodega.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyLineaInventarioData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las lineas de Inventario
            var inventoryLines = db.InventoryLine
                .Where(t => t.isActive && t.id_company == empresa);

            // Procesamos los datos recuperamos
            if (inventoryLines.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_LINEA_INVENTARIO$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var inventoryLine in inventoryLines)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(inventoryLine.name);
                        idParam.SetIntegerValue(inventoryLine.id);
                        codigoParam.SetStringValue(inventoryLine.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion
    }
}