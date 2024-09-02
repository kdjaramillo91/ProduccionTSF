using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Web.Mvc;

namespace DXPANACEASOFT.Auxiliares
{
    internal class ItemTrademarkFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/ItemTradeMarkTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        static ItemTrademarkFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }
        internal ItemTrademarkFileParser(string contentFilePath, string contentFileMime)
           : base(contentFilePath, contentFileMime, true)
        {
        }

        

        internal ImportResult ParseItemTrademark(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemtrademarkColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarItemTrademark(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }

        private static string GetTablaItemtrademarkColumnNames()
        {
            string columnTypes = $"CODIGO,NOMBRE,DESCRIPCION";
            return columnTypes;
        }

        internal ImportResult ProcesarItemTrademark(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

           

            var itemsCrear = new List<ItemTrademark>();
            int numFilasVacias = 0;
            int numFila = 0;

            #region Leer el Excel

            foreach (DataRow drCabecera in dtCabecera.Rows)
            {
                numFila++;

                if (numFilasVacias > 5) break;

                try
                {
                    if (!string.IsNullOrEmpty(drCabecera.GetString("CODIGO")))
                    {
                        var itemTypeCategoriaSubir = new ItemTrademark()
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
                        Descripcion = $"Error agregando fila a tabla de datos: Marca. Fila: {numFila}. {ex.Message}",
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

            #region Validar los datos

            for (int i = 0; i < itemsCrear.Count; i++)
            {
                var itemTrademarkCrear = itemsCrear.ElementAt(i);

                try
                {
                    // Validamos integridad de campos obligatorios
                    if (String.IsNullOrEmpty(itemTrademarkCrear.code))
                    {
                        throw new Exception("El código es obligatorio para crear Marca de ítem.");
                    }

                    // Validamos longitud de código
                    if (itemTrademarkCrear.code.Length > 20)
                    {
                        throw new Exception("El código no debe tener longitud mayor a 20 caracteres.");
                    }

                    if (String.IsNullOrEmpty(itemTrademarkCrear.name))
                    {
                        throw new Exception("El nombre es obligatorio para crear Marca de ítem.");
                    }

                    // Validamos longitud de código
                    if (itemTrademarkCrear.name.Length > 50)
                    {
                        throw new Exception("El código no debe tener longitud mayor a 50 caracteres.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    if (itemsCrear.Where(e => e.code == itemTrademarkCrear.code).Count() > 1)
                    {
                        throw new Exception($"El código: {itemTrademarkCrear.code} está repetido dentro del archivo.");
                    }

                    // Validamos que el código no esté repetido dentro del mismo archivo
                    var itemBase = db.ItemTrademark.FirstOrDefault(e => e.code == itemTrademarkCrear.code);
                    if (itemBase != null)
                    {
                        throw new Exception($"El código: {itemTrademarkCrear.code} ya existe o está inactivo dentro de la base de datos.");
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
                    db.ItemTrademark.AddRange(itemsCrear);

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
        internal static FileContentResult GetTemplateFileContentResult(int empresa)
        {
            // Generamos una copia del template original
            var contentId = CopyContentFile(m_TemplateAbsolutePath, ExcelXlsmMime);
            var contentPath = FileExcelUploadHelper.GetFileContentPath(contentId);
            var excelParser = new ItemTrademarkFileParser(contentPath, ExcelXlsmMime);

           

            // Recuperar los datos de la plantilla
            var content = FileExcelUploadHelper.ReadFileUpload(contentId,
                out var fileName, out string contentType, out _);

            try
            {
                fileName = Path.ChangeExtension(
                    $"MarcaProducto_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
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
    }
}