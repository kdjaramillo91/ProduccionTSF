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
using System.Data.OleDb;
using System.IO;
using System.Data;

namespace DXPANACEASOFT.Auxiliares.ExcelFileParsers
{
    internal class ItemTypeCategoryExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/ItemTypeCategoryTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores

        static ItemTypeCategoryExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }

        internal ItemTypeCategoryExcelFileParser(string contentFilePath, string contentFileMime)
            : base(contentFilePath, contentFileMime, true)
        {
        }

        private ItemTypeCategoryExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
            : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        #endregion

        #region Métodos Importación de Plantilla

        internal ImportResult ParseItemTypeCategory(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTypeColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarItemTypeCategory(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }

        private static string GetTablaItemTypeColumnNames()
        {
            string columnTypes = $"CODIGO,NOMBRE,SUBGRUPO,DESCRIPCION";
            return columnTypes;
        }

        private ImportResult ProcesarItemTypeCategory(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el Excel

            var itemsCrear = new List<ItemTypeCategory>();
            int numFilasVacias = 0;
            int numFila = 0;

            foreach (DataRow drCabecera in dtCabecera.Rows)
            {
                numFila++;

                if (numFilasVacias > 5) break;

                try
                {
                    if (!string.IsNullOrEmpty(drCabecera.GetString("CODIGO")))
                    {
                        var itemTypeCategoriaSubir = new ItemTypeCategory()
                        {
                            code = drCabecera.GetString("CODIGO"),
                            name = drCabecera.GetString("NOMBRE"),
                            description = drCabecera.GetString("DESCRIPCION"),
                            isActive = true,
                            id_company = activeCompanyId,
                            id_userCreate = activeUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = activeUser.id,
                            dateUpdate = DateTime.Now
                        };

                        // Tipos de producto (Mejorar recorrido)
                        var itemTypeItemTypeCategories = new ItemTypeItemTypeCategory()
                        {
                            id_itemType = drCabecera.GetInteger("ID_TIPO_PRODUCTO"),
                            id_itemTypeCategory = itemTypeCategoriaSubir.id,
                        };

                        itemTypeCategoriaSubir.ItemTypeItemTypeCategory.Add(itemTypeItemTypeCategories);

                        itemsCrear.Add(itemTypeCategoriaSubir);
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
                        Descripcion = $"Error agregando fila a tabla de datos: Categoria de Ítem. Fila: {numFila}. {ex.Message}",
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

            #region Validar datos

            for (int i = 0; i < itemsCrear.Count; i++)
            {
                var itemTypeCategoryCrear = itemsCrear.ElementAt(i);

                try
                {
                    // Validamos integridad de campos obligatorios
                    if (String.IsNullOrEmpty(itemTypeCategoryCrear.code))
                    {
                        throw new Exception("El código es obligatorio para crear categoria de ítem.");
                    }

                    if (String.IsNullOrEmpty(itemTypeCategoryCrear.name))
                    {
                        throw new Exception("El nombre es obligatorio para crear categoria de ítem.");
                    }

                    // Validamos longitud de código
                    if (itemTypeCategoryCrear.code.Length > 20)
                    {
                        throw new Exception("El código no debe ser de longitud mayor a 20 caracteres.");
                    }

                    if (itemTypeCategoryCrear.ItemTypeItemTypeCategory.Count <= 0)
                    {
                        throw new Exception("El tipo de producto es obligatorio para crear categoria de ítem.");
                    }

                    // Validamos longitud de código
                    if (itemTypeCategoryCrear.name.Length > 50)
                    {
                        throw new Exception("El nombre no debe ser de longitud mayor a 50 caracteres.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    if (itemsCrear.Where(e => e.code == itemTypeCategoryCrear.code).Count() > 1)
                    {
                        throw new Exception($"El código: {itemTypeCategoryCrear.code} está repetido dentro del archivo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    var itemBase = db.ItemTypeCategory.FirstOrDefault(e => e.code == itemTypeCategoryCrear.code);
                    if (itemBase != null)
                    {
                        throw new Exception($"El código: {itemTypeCategoryCrear.code} ya existe o está inactivo dentro de la base de datos.");
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
                    db.ItemTypeCategory.AddRange(itemsCrear);

                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = itemsCrear.Count.ToString(),
                        NumDocumento = itemsCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {itemsCrear.Count} elementos."
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

        #region Métodos descarga de plantillas

        internal static FileContentResult GetTemplateFileContentResult(int empresa)
        {
            // Generamos una copia del template original
            var contentId = CopyContentFile(m_TemplateAbsolutePath, ExcelXlsmMime);
            var contentPath = FileExcelUploadHelper.GetFileContentPath(contentId);
            var excelParser = new ItemTypeCategoryExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();
                    CopyTipoProductoData(empresa, cnn);

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
                    $"CategoriaProducto_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
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

        private static void CopyTipoProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los tipos de productos
            var tipoProductos = db.ItemType
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.id_inventoryLine);

            // Procesamos los datos recuperamos
            if (tipoProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_TIPO_PRODUCTO$] ([CODIGO_LINEA], [NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?, ?)";

                    var codigoLinea = cmd.Parameters.Add("CODIGO_LINEA", OleDbType.VarChar);
                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var tipoProducto in tipoProductos)
                    {
                        var lineaInventario = db.InventoryLine.FirstOrDefault(t => t.id == tipoProducto.id_inventoryLine);
                        // Insertamos los valores en la tabla de Excel
                        codigoLinea.SetStringValue(lineaInventario.code);
                        nombreParam.SetStringValue(tipoProducto.name);
                        idParam.SetIntegerValue(tipoProducto.id);
                        codigoParam.SetStringValue(tipoProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion
    }
}