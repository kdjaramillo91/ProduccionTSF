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
    internal class ItemPresentationExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/ItemPresentationTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores

        static ItemPresentationExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }

        internal ItemPresentationExcelFileParser(string contentFilePath, string contentFileMime)
            : base(contentFilePath, contentFileMime, true)
        {
        }

        private ItemPresentationExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
            : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        #endregion

        #region Métodos para la importación de plantilla

        internal ImportResult ParseItemPresentation(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTypeColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarPresentaciones(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }

        private static string GetTablaItemTypeColumnNames()
        {
            string columnTypes = $"CODIGO,NOMBRE,ID_PRODUCTO_MINIMO,ID_PRODUCTO_FINAL,ID_UNIDAD_MEDIDA,MINIMO,MAXIMO,DESCRIPCION";
            return columnTypes;
        }

        private ImportResult ProcesarPresentaciones(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el Excel

            var itemPresentationCrear = new List<Presentation>();
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
                        var itemPresentationSubir = new Presentation()
                        {
                            code = drCabecera.GetString("CODIGO"),
                            name = drCabecera.GetString("NOMBRE"),
                            description = drCabecera.GetString("DESCRIPCION"),
                            id_itemPackingMinimum = drCabecera.GetInteger("ID_PRODUCTO_MINIMO"),
                            id_itemPackingMaximum = drCabecera.GetInteger("ID_PRODUCTO_FINAL"),
                            id_metricUnit = drCabecera.GetInteger("ID_UNIDAD_MEDIDA"),
                            minimum = drCabecera.GetDecimal("MINIMO"),
                            maximum = drCabecera.GetInteger("MAXIMO"),
                            isActive = true,
                            id_company = activeCompanyId,
                            id_userCreate = activeUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = activeUser.id,
                            dateUpdate = DateTime.Now
                        };
                        // Se agrega a la lista auxiliar
                        itemPresentationCrear.Add(itemPresentationSubir);
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
                        Descripcion = $"Error agregando fila a tabla de datos: Presentación. Fila: {numFila}. {ex.Message}",
                    });
                }
            }
            // Si la lista está vacía, retornamos como error.
            if (!itemPresentationCrear.Any())
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

            if (itemPresentationCrear.Count > 0)
            {
                for (int i = 0; i < itemPresentationCrear.Count; i++)
                {
                    var presentationCrear = itemPresentationCrear.ElementAt(i);

                    try
                    {
                        // Validamos integridad de campos obligatorios
                        if (String.IsNullOrEmpty(presentationCrear.code))
                        {
                            throw new Exception("El código es obligatorio para crear presentación.");
                        }

                        // Validamos longitud de código
                        if (presentationCrear.code.Length > 20)
                        {
                            throw new Exception("El código no debe tener longitud mayor a 20 caracteres.");
                        }

                        // Nombre de la bodega
                        if (String.IsNullOrEmpty(presentationCrear.name))
                        {
                            throw new Exception("El nombre es obligatorio para crear presentación.");
                        }

                        // Validamos longitud de nombre
                        if (presentationCrear.name.Length > 50)
                        {
                            throw new Exception("El nombre no debe tener longitud mayor a 50 caracteres.");
                        }

                        // Empaque Minimo
                        if (presentationCrear.id_itemPackingMinimum <= 0)
                        {
                            throw new Exception("El empaque mínimo es obligatorio para crear presentación.");
                        }

                        // Empaque Minimo
                        if (presentationCrear.id_itemPackingMaximum <= 0)
                        {
                            throw new Exception("El empaque máximo es obligatorio para crear presentación.");
                        }

                        // Unidad de medida
                        if (presentationCrear.id_metricUnit <= 0)
                        {
                            throw new Exception("La unidad de medida es obligatoria para crear presentación.");
                        }

                        // Unidad de medida
                        if (String.IsNullOrEmpty(presentationCrear.description))
                        {
                            throw new Exception("La descripción es obligatoria para crear presentación.");
                        }

                        // Validamos que el código no esté repetido dentro del mismo archivo
                        if (itemPresentationCrear.Where(e => e.code == presentationCrear.code).Count() > 1)
                        {
                            throw new Exception($"El código: {presentationCrear.code} está repetido dentro del archivo.");
                        }

                        // Validamos que el código no esté repetido dentro de la base de datos
                        var itemPresentationBase = db.Presentation.FirstOrDefault(e => e.code == presentationCrear.code);
                        if (itemPresentationBase != null)
                        {
                            throw new Exception($"El código: {presentationCrear.code} ya existe o está inactivo dentro de la base de datos.");
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
            }
            else
            {
                throw new Exception("Sin información para crear presentaciones.");
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
                    db.Presentation.AddRange(itemPresentationCrear);

                    // Ingresa a la BD
                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = itemPresentationCrear.Count.ToString(),
                        NumDocumento = itemPresentationCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {itemPresentationCrear.Count} elementos."
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
            var excelParser = new ItemPresentationExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();

                    CopyProduct(empresa, cnn);
                    CopyMetricUnit(empresa, cnn);

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
                    $"Presentacion_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
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

        private static void CopyProduct(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();

            // Recuperampos los datos de tipos de bodegas
            // Id de Linea de inventario SOLO para MATERIALES E INSUMOS (5)
            var productos = db.Item
                .Where(e => e.isActive && e.id_company == empresa && e.id_inventoryLine == 5)
                .OrderBy(e => e.name);

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
                        nombreParam.SetStringValue(product.name);
                        idParam.SetIntegerValue(product.id);
                        codigoParam.SetStringValue(product.masterCode);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyMetricUnit(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las lineas de Inventario
            var inventoryLines = db.MetricUnit
                .Where(t => t.isActive && t.id_company == empresa);

            // Procesamos los datos recuperamos
            if (inventoryLines.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_UNIDAD_MEDIDA$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

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