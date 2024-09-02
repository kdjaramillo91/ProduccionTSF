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
    internal class SubcostCenterExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/SubcostCenterTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores

        static SubcostCenterExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }

        internal SubcostCenterExcelFileParser(string contentFilePath, string contentFileMime)
            : base(contentFilePath, contentFileMime, true)
        {
        }

        private SubcostCenterExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
            : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        #endregion

        #region Métodos para la importación de plantilla

        internal ImportResult ParseSubcostCenter(User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTypeColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarSubcostCenter(dataTables[0], activeUser);

            return importResult;
        }

        private static string GetTablaItemTypeColumnNames()
        {
            string columnTypes = $"CODIGO,NOMBRE,ID_CENTRO_COSTO,DESCRIPCION";
            return columnTypes;
        }

        private ImportResult ProcesarSubcostCenter(DataTable dtCabecera, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el Excel

            var subcostCentersCrear = new List<CostCenter>();
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
                        var subcostCenterSubir = new CostCenter()
                        {
                            code = drCabecera.GetString("CODIGO"),
                            name = drCabecera.GetString("NOMBRE"),
                            id_higherCostCenter = drCabecera.GetIntegerNullable("ID_CENTRO_COSTO"),
                            description = drCabecera.GetString("DESCRIPCION"),
                            isActive = true,
                            id_userCreate = activeUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = activeUser.id,
                            dateUpdate = DateTime.Now,                            
                        };

                        // Se agrega a la lista auxiliar
                        subcostCentersCrear.Add(subcostCenterSubir);
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
                        Descripcion = $"Error agregando fila a tabla de datos: Subcentro de Costo. Fila: {numFila}. {ex.Message}",
                    });
                }
            }

            if (!subcostCentersCrear.Any())
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

            for (int i = 0; i < subcostCentersCrear.Count; i++)
            {
                var subcostCenterCrear = subcostCentersCrear.ElementAt(i);

                try
                {
                    // Validamos integridad de campos obligatorios
                    if (String.IsNullOrEmpty(subcostCenterCrear.code))
                    {
                        throw new Exception("El código es obligatorio para crear subcentro de Costo.");
                    }

                    // Validamos longitud de código
                    if (subcostCenterCrear.code.Length > 50)
                    {
                        throw new Exception("El código no debe tener longitud mayor a 50 caracteres.");
                    }

                    // Nombre 
                    if (String.IsNullOrEmpty(subcostCenterCrear.name))
                    {
                        throw new Exception("El nombre es obligatorio para crear subcentro de costo.");
                    }

                    // Validamos longitud de nombre
                    if (subcostCenterCrear.name.Length > 250)
                    {
                        throw new Exception("El nombre no debe tener longitud mayor a 250 caracteres.");
                    }

                    if (!subcostCenterCrear.id_higherCostCenter.HasValue)
                    {
                        throw new Exception("El centro de costo es obligatorio para crear subcentro de costo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    if (subcostCentersCrear.Where(e => e.code == subcostCenterCrear.code).Count() > 1)
                    {
                        throw new Exception($"El código: {subcostCenterCrear.code} está repetido dentro del archivo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    var subcostCenterBase = db.CostCenter.FirstOrDefault(e => e.code == subcostCenterCrear.code);
                    if (subcostCenterBase != null)
                    {
                        throw new Exception($"El código: {subcostCenterCrear.code} ya existe o está inactivo dentro de la base de datos.");
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
                    db.CostCenter.AddRange(subcostCentersCrear);

                    // Ingresa a la BD
                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = subcostCentersCrear.Count.ToString(),
                        NumDocumento = subcostCentersCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {subcostCentersCrear.Count} elementos."
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
            var excelParser = new SubcostCenterExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();

                    CopyCostCenter(cnn);

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
                    $"SubCCosto_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
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

        private static void CopyCostCenter(OleDbConnection cnn)
        {
            var db = new DBContext();

            // Recuperampos los datos de tipos de bodegas
            var bodegas = db.CostCenter
                .Where(e => e.isActive && e.id_higherCostCenter == null)
                .OrderBy(e => e.name);

            if (bodegas.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_CENTRO_COSTO$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    foreach (var bodega in bodegas)
                    {
                        // Preparación de concatenación
                        var bodegaString = $"{bodega.code}|{bodega.name}";
                        // Insertamos los valores en la tabla de Excel

                        nombreParam.SetStringValue(bodegaString);
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