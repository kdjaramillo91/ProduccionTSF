using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.DataProviders;
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

namespace DXPANACEASOFT.Auxiliares
{
    internal class ProductTypeExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/ProductTypeTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores

        static ProductTypeExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }

        internal ProductTypeExcelFileParser(string contentFilePath, string contentFileMime)
            : base(contentFilePath, contentFileMime, true)
        {
        }

        private ProductTypeExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
            : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        #endregion

        #region Métodos
        internal ImportResult ParseItemType(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTypeColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarItemType(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }

        private static string GetTablaItemTypeColumnNames()
        {
            string columnTypes = $"CODIGO,NOMBRE,LINEA_INVENTARIO,CODIGO_LINEA_INVENTARIO,ID_LINEA_INVENTARIO,DESCRIPCION";
            return columnTypes;
        }

        private ImportResult ProcesarItemType(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el excel
            var itemsCrear = new List<ItemType>();
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
                        var itemSubir = new ItemType()
                        {
                            code = drCabecera.GetString("CODIGO"),
                            name = drCabecera.GetString("NOMBRE"),
                            id_inventoryLine = drCabecera.GetInteger("ID_LINEA_INVENTARIO"),
                            description = drCabecera.GetString("DESCRIPCION"),
                            isActive = true,
                            id_company = activeCompanyId,
                            id_userCreate = activeUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = activeUser.id,
                            dateUpdate = DateTime.Now
                        };

                        itemsCrear.Add(itemSubir);
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
                        Descripcion = $"Error agregando fila a tabla de datos: Tipo de ítem. Fila: {numFila}. {ex.Message}",
                    });
                }
            }
            if (!itemsCrear.Any())
            {
                documentosFallidos.Add(new ImportResult.DocumentoFallido()
                {
                    //Filas = string.Empty, numFila.ToString(),
                    Filas = string.Empty,
                    Descripcion = $"El documento está vacío.",
                });
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

            #region Validar Datos
            for (int i = 0; i < itemsCrear.Count; i++)
            {
                var itemCrear = itemsCrear.ElementAt(i);

                try
                {
                    // Validamos integridad de campos obligatorios
                    if (String.IsNullOrEmpty(itemCrear.code))
                    {
                        throw new Exception("El código es obligatorio para crear tipo de ítem.");
                    }

                    // Validamos longitud de código
                    if (itemCrear.code.Length > 20)
                    {
                        throw new Exception("El código no debe tener longitud mayor a 20 caracteres.");
                    }

                    if (String.IsNullOrEmpty(itemCrear.name))
                    {
                        throw new Exception("El nombre es obligatorio para crear tipo de ítem.");
                    }

                    // Validamos longitud de nombre
                    if (itemCrear.code.Length > 50)
                    {
                        throw new Exception("El nombre no debe tener longitud mayor a 50 caracteres.");
                    }

                    if (itemCrear.id_inventoryLine <= 0)
                    {
                        throw new Exception("La línea de inventario es obligatorio para crear tipo de ítem.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    if(itemsCrear.Where(e => e.code == itemCrear.code).Count() > 1)
                    {
                        throw new Exception($"El código: {itemCrear.code} está repetido dentro del archivo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    var itemBase = db.ItemType.FirstOrDefault(e => e.code == itemCrear.code);
                    if (itemBase != null)
                    {
                        throw new Exception($"El código: {itemCrear.code} ya existe o está inactivo dentro de la base de datos.");
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
                    db.ItemType.AddRange(itemsCrear);

                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado() 
                    { 
                        Filas = itemsCrear.Count.ToString(),
                        NumDocumento = itemsCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {itemsCrear.Count} tipos de ítem."
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

        internal static FileContentResult GetTemplateFileContentResult(int empresa)
        {
            // Generamos una copia del template original
            var contentId = CopyContentFile(m_TemplateAbsolutePath, ExcelXlsmMime);
            var contentPath = FileExcelUploadHelper.GetFileContentPath(contentId);
            var excelParser = new ProductTypeExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();
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
                    $"TipoProducto_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
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