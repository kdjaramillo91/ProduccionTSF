using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Web.Mvc;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using DXPANACEASOFT.Auxiliares;
using DevExpress.Web;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Data.Entity;

namespace DXPANACEASOFT.Auxiliares.ExcelFileParsers
{
   internal class ItemTrademarkModelExcelFileParser: ExcelFileParserBase
    {
        private const string m_TemplateRelativePath = "~/App_Data/Templates/ItemTradeMarkModelTemplate.xlsx";

        private static readonly string m_TemplateAbsolutePath;

        static ItemTrademarkModelExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }
        internal ItemTrademarkModelExcelFileParser(string contentFilePath, string contentFileMime)
           : base(contentFilePath, contentFileMime, true)
        {
        }

        private ItemTrademarkModelExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
           : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        internal ImportResult ParseItemType(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTrademarkModelColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarItemTrademarkModel(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }
        private static string GetTablaItemTrademarkModelColumnNames()
        {
            string columnTypes = $"CODIGO,NOMBRE,ID_MARCA,DESCRIPCION";
            return columnTypes;
        }

        private ImportResult ProcesarItemTrademarkModel(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el excel
            var itemsCrear = new List<ItemTrademarkModel>();
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
                        var itemSubir = new ItemTrademarkModel()
                        {
                            code = drCabecera.GetString("CODIGO"),
                            name = drCabecera.GetString("NOMBRE"),
                            id_trademark = drCabecera.GetInteger("ID_Marca"),
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
                        Descripcion = $"Error agregando fila a tabla de datos: Modelo. Fila: {numFila}. {ex.Message}",
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
                        throw new Exception("El código es obligatorio para crear Modelo.");
                    }

                    // Validamos longitud de código
                    if (itemCrear.code.Length > 20)
                    {
                        throw new Exception("El código no debe tener longitud mayor a 20 caracteres.");
                    }

                    if (String.IsNullOrEmpty(itemCrear.name))
                    {
                        throw new Exception("El nombre es obligatorio para crear Modelo.");
                    }

                    // Validamos longitud de código
                    if (itemCrear.name.Length > 50)
                    {
                        throw new Exception("El nombre no debe tener longitud mayor a 50 caracteres.");
                    }

                    if (itemCrear.id_trademark <= 0)
                    {
                        throw new Exception("La Marca es obligatorio para crear Modelo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    if (itemsCrear.Where(e => e.code == itemCrear.code).Count() > 1)
                    {
                        throw new Exception($"El código: {itemCrear.code} está repetido dentro del archivo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    var itemBase = db.ItemTrademarkModel.FirstOrDefault(e => e.code == itemCrear.code);
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
                    db.ItemTrademarkModel.AddRange(itemsCrear);

                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = itemsCrear.Count.ToString(),
                        NumDocumento = itemsCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {itemsCrear.Count} los Modelos."
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

        internal static FileContentResult GetTemplateFileContentResult(int Marca)
        {
            // Generamos una copia del template original
            var contentId = CopyContentFile(m_TemplateAbsolutePath, ExcelXlsmMime);
            var contentPath = FileExcelUploadHelper.GetFileContentPath(contentId);
            var excelParser = new ItemTrademarkModelExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();
                    CopyMarcaData(Marca, cnn);

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
                    $"ModeloProducto_{Marca}_{DateTime.Now:yyyyMMdd HHmm}",
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

        private static void CopyMarcaData(int Marca, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las lineas de Inventario
            var inventoryLines = db.ItemTrademark
                .Where(t => t.isActive && t.id_company == Marca);

            // Procesamos los datos recuperamos
            if (inventoryLines.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_MODELO$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

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


    }
}