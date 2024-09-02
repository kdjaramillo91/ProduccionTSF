using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.ItemP.ItemModel;
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
    internal class ItemEquivalenceExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/ItemEquivalenceTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores

        static ItemEquivalenceExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }

        internal ItemEquivalenceExcelFileParser(string contentFilePath, string contentFileMime)
            : base(contentFilePath, contentFileMime, true)
        {
        }

        private ItemEquivalenceExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
            : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        #endregion

        #region Métodos para la importación de plantilla

        internal ImportResult ParseItemEquivalence()
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTypeColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarCostCenters(dataTables[0]);

            return importResult;
        }

        private static string GetTablaItemTypeColumnNames()
        {
            string columnTypes = $"ID_PRODUCTO,ID_EQUIVALENTE";
            return columnTypes;
        }

        private ImportResult ProcesarCostCenters(DataTable dtCabecera)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el Excel

            var itemEquivalenceCrear = new List<ItemEquivalence>();
            int numFilasVacias = 0;
            int numFila = 0;

            foreach (DataRow drCabecera in dtCabecera.Rows)
            {
                numFila++;

                if (numFilasVacias > 5) break;

                try
                {
                    // Verifico la primera información
                    if (!string.IsNullOrEmpty(drCabecera.GetString("ID_PRODUCTO")))
                    {
                        var itemEquivalenceSubir = new ItemEquivalence()
                        {
                            id_item = drCabecera.GetInteger("ID_PRODUCTO"),
                            id_itemEquivalence = drCabecera.GetInteger("ID_EQUIVALENTE"),
                        };

                        // Se agrega a la lista auxiliar
                        itemEquivalenceCrear.Add(itemEquivalenceSubir);
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
                        Descripcion = $"Error agregando fila a tabla de datos: Centro de Costo. Fila: {numFila}. {ex.Message}",
                    });
                }
            }

            if (!itemEquivalenceCrear.Any())
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

            List<int> auxiliarIdItem = new List<int>();

            for (int i = 0; i < itemEquivalenceCrear.Count; i++)
            {
                var itemEquivalenceValidar = itemEquivalenceCrear.ElementAt(i);                

                try
                {
                    var existeDuplicado = auxiliarIdItem.Contains(itemEquivalenceValidar.id_item);

                    // Verificar valor duplicado
                    if(existeDuplicado)
                    {
                        throw new Exception("Existe producto duplicado dentro de lista.");
                    }

                    // Validamos integridad de campos obligatorios
                    if (itemEquivalenceValidar.id_item <= 0)
                    {
                        throw new Exception("El producto principal no existe para crear equivalente.");
                    }

                    if (itemEquivalenceValidar.id_itemEquivalence <= 0)
                    {
                        throw new Exception("El producto equivalente no existe para crear su equivalente.");
                    }

                    auxiliarIdItem.Add(itemEquivalenceValidar.id_item);
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

            auxiliarIdItem.Clear();

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
                    db.ItemEquivalence.AddRange(itemEquivalenceCrear);

                    // Ingresa a la BD
                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = itemEquivalenceCrear.Count.ToString(),
                        NumDocumento = itemEquivalenceCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {itemEquivalenceCrear.Count} elementos."
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
            var excelParser = new ItemEquivalenceExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();
                    CopyProduct(cnn);
                    CopyEquivalence(cnn);
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
                    $"EquivalenciaProducto_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
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

        private static void CopyEquivalence(OleDbConnection cnn)
        {
            var db = new DBContext();

            // Recuperamos los equivalentes ya existentes
            var equivalentesExistentes = db.ItemEquivalence.ToList();

            // Recuperamos los items por 'Producto Terminado' y 'Producto en Proceso'
            var productos = db.Item
                        .Where(r => (r.InventoryLine.code == "PT" || r.InventoryLine.code == "PP") && r.isActive)
                        .OrderBy(o => o.masterCode)
                        .ToList();

            if (productos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_EQUIVALENTE$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    foreach (var product in productos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue($"{product.masterCode} - {product.name}");
                        idParam.SetIntegerValue(product.id);
                        codigoParam.SetStringValue(product.masterCode);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyProduct(OleDbConnection cnn)
        {
            var db = new DBContext();

            // Recuperamos los equivalentes ya existentes
            var equivalentesExistentes = db.ItemEquivalence.ToList();

            // Recuperamos los items por 'Producto Terminado' y 'Producto en Proceso'
            var productos = db.Item
                        .Where(r => (r.InventoryLine.code == "PT" || r.InventoryLine.code == "PP") && r.isActive)
                        .OrderBy(o => o.masterCode)
                        .ToList();

            // Filtramos los productos que no estén en equivalentes
            productos = productos.Where(e => !equivalentesExistentes.Contains(e.ItemEquivalence)).ToList();

            if (productos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_PRODUCTO$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    foreach (var product in productos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue($"{product.masterCode} - {product.name}");
                        idParam.SetIntegerValue(product.id);
                        codigoParam.SetStringValue(product.masterCode);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion
    }
}