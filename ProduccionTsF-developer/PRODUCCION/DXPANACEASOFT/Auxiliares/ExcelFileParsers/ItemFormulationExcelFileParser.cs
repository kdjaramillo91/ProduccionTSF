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
    internal class ItemFormulationExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/ItemFormulationTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores
        static ItemFormulationExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }
        internal ItemFormulationExcelFileParser(string contentFilePath, string contentFileMime)
           : base(contentFilePath, contentFileMime, true)
        {
        }

        private ItemFormulationExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
           : base(contentFilePath, contentFileMime, readOnly)
        {
        }
        #endregion

        #region Métodos para la importación de plantilla
        internal ImportResult ParseItemFormulation(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemFormulationColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarItemFormulation(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }
        private static string GetTablaItemFormulationColumnNames()
        {
            string columnTypes = $"ID,CODIGO_PRODUCTO,ID_PRODUCTO,ID_INGREDIENTE,CANTIDAD," +
                $"CANTIDAD_MAXIMA,ID_UM,ID_UM_MAXIMA,ID_CLIENTE";
            return columnTypes;
        }

        #region Subclase para la importación de formulas a agrupar

        public class ItemFormulation
        {
            // Cabecera
            public int IdItem { get; set; }
            public decimal Amount { get; set; }
            public int IdMetricUnit { get; set; }

            // Detalles
            public int IdRow { get; set; }
            public int IdIngredientItem { get; set; }
            public int IdMetricUnitDetail { get; set; }
            public int IdMetricUnitMaxDetail { get; set; }
            public decimal AmountDetail { get; set; }
            public decimal AmountMaxDetail { get; set; }
            public int? IdCustomerDetail { get; set; }
        }

        #endregion

        private ImportResult ProcesarItemFormulation(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el excel
            var formulacionesCrear = new List<ItemFormulation>();
            var formulacionesCabeceraCrear = new List<ItemHeadIngredient>();
            var formulacionesDetalleCrear = new List<ItemIngredient>();
            int numFilasVacias = 0;
            int numFila = 0;
            foreach (DataRow drCabecera in dtCabecera.Rows)
            {
                numFila++;

                if (numFilasVacias > 5) break;

                try
                {
                    if (!string.IsNullOrEmpty(drCabecera.GetString("CODIGO_PRODUCTO")))
                    {
                        formulacionesCrear.Add(new ItemFormulation()
                        {
                            IdItem = drCabecera.GetInteger("ID_PRODUCTO"),
                            IdRow = drCabecera.GetInteger("ID"),
                            IdIngredientItem = drCabecera.GetInteger("ID_INGREDIENTE"),
                            AmountDetail = drCabecera.GetDecimal("CANTIDAD"),
                            AmountMaxDetail = drCabecera.GetDecimal("CANTIDAD_MAXIMA"),
                            IdMetricUnitDetail = drCabecera.GetInteger("ID_UM"),
                            IdMetricUnitMaxDetail = drCabecera.GetInteger("ID_UM_MAXIMA"),
                            IdCustomerDetail = drCabecera.GetIntegerNullable("ID_CLIENTE"),
                        });
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

            if (!formulacionesCrear.Any())
            {
                documentosFallidos.Add(new ImportResult.DocumentoFallido()
                {
                    //Filas = string.Empty, numFila.ToString(),
                    Filas = string.Empty,
                    Descripcion = $"El documento está vacío o contiene errores.",
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

            if (formulacionesCrear.Count() > 0)
            {
                var agrupaciones = formulacionesCrear
                .GroupBy(e => new
                {
                    e.IdItem,
                    e.Amount,
                    e.IdMetricUnit
                })
                .Select(e => new
                {
                    e.Key.IdItem,
                    e.Key.Amount,
                    e.Key.IdMetricUnit,
                    Detalles = e.Select(x => x)
                });

                foreach (var agrupacion in agrupaciones)
                {
                    // Rango de Error (Puesto que la cabecera se agrupa)
                    var agrupacionErrorRangoInicio = formulacionesCrear.FirstOrDefault(e => e.IdItem == agrupacion.IdItem &&
                                                            e.Amount == agrupacion.Amount &&
                                                            e.IdMetricUnit == agrupacion.IdMetricUnit).IdRow;

                    var agrupacionErrorRangoFin = formulacionesCrear.LastOrDefault(e => e.IdItem == agrupacion.IdItem &&
                                                            e.Amount == agrupacion.Amount &&
                                                            e.IdMetricUnit == agrupacion.IdMetricUnit).IdRow;
                    var filas = string.Join(",", agrupacion.Detalles.Select(e => e.IdRow).Distinct());

                    try
                    {
                        // Validación de productos
                        var item = db.Item.FirstOrDefault(e => e.id == agrupacion.IdItem);

                        if (item == null)
                        {
                            throw new Exception("El producto no existe en la base de datos.");
                        }

                        var numRepetidos = agrupaciones
                            .Where(e => e.IdItem == agrupacion.IdItem)
                            .Count();

                        if (numRepetidos > 1)
                        {
                            throw new Exception("Existe un producto duplicado a ingresar dentro del documento.");
                        }

                        //// Validación de Unidad de Medida
                        //var metricUnit = db.MetricUnit.FirstOrDefault(e => e.id == agrupacion.IdMetricUnit);

                        //if (metricUnit == null)
                        //{
                        //    throw new Exception("La unidad de medida no existe en la base de datos.");
                        //}

                        // Si está Ok, creamos el objeto
                        var formulacion = new ItemHeadIngredient()
                        {
                            id_Item = agrupacion.IdItem,
                            amount = agrupacion.Amount,
                            id_metricUnit = agrupacion.IdMetricUnit,
                        };

                        formulacionesCabeceraCrear.Add(formulacion);

                        // Proceso de Detalles
                        foreach (var detalle in agrupacion.Detalles)
                        {
                            var filaErrorDetalle = detalle.IdRow;

                            try
                            {
                                // Validación de productos
                                var itemIngredient = db.Item.FirstOrDefault(e => e.id == detalle.IdIngredientItem);

                                if (itemIngredient == null)
                                {
                                    throw new Exception("El ingrediente de producto no existe en la base de datos.");
                                }

                                // Validación de Unidades de Medida                                
                                var metricUnitDetail = db.MetricUnit.FirstOrDefault(e => e.id == detalle.IdMetricUnitDetail);

                                if (metricUnitDetail == null)
                                {
                                    throw new Exception("La unidad de medida no existe en la base de datos.");
                                }

                                var metricUnitMaxDetail = db.MetricUnit.FirstOrDefault(e => e.id == detalle.IdMetricUnitMaxDetail);

                                if (metricUnitMaxDetail == null)
                                {
                                    throw new Exception("La unidad de medida máxima no existe en la base de datos.");
                                }

                                // Validación de Monto
                                if (detalle.AmountDetail < 0)
                                {
                                    throw new Exception("La CANTIDAD no puede ser menor a cero.");
                                }

                                if (detalle.AmountMaxDetail < 0)
                                {
                                    throw new Exception("La CANTIDAD MÁXIMA no puede ser menor a cero.");
                                }

                                //Validación de Cliente
                                if (detalle.IdCustomerDetail.HasValue)
                                {
                                    var personDetail = db.Person.FirstOrDefault(e => e.id == detalle.IdCustomerDetail);

                                    if (personDetail == null)
                                    {
                                        throw new Exception("El cliente no existe en la base de datos.");
                                    }
                                }

                                // Si está Ok, creamos el objeto Detalles
                                var formulacionDetalle = new ItemIngredient()
                                {
                                    id_compoundItem = agrupacion.IdItem,
                                    id_ingredientItem = detalle.IdIngredientItem,
                                    id_metricUnit = detalle.IdMetricUnitDetail,
                                    id_metricUnitMax = detalle.IdMetricUnitMaxDetail,
                                    amount = detalle.AmountDetail,
                                    amountMax = detalle.AmountMaxDetail,
                                    id_costumerItem = detalle.IdCustomerDetail,
                                };

                                formulacionesDetalleCrear.Add(formulacionDetalle);
                            }
                            catch (Exception ex)
                            {
                                documentosFallidos.Add(new ImportResult.DocumentoFallido()
                                {
                                    Filas = filaErrorDetalle.ToString(),
                                    Descripcion = $"Se debe corregir el error en la Fila: {filaErrorDetalle}. Error: {ex.Message}",
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        documentosFallidos.Add(new ImportResult.DocumentoFallido()
                        {
                            Filas = filas,
                            Descripcion = $"Se debe corregir el error en fila(s): {filas}. Error: {ex.Message}",
                        });
                    }
                }
            }
            else
            {
                throw new Exception("Sin información para crear formulaciones.");
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
                    // Recorro la lista para ingresar a la base
                    foreach (var formulacionCrear in formulacionesCabeceraCrear)
                    {
                        // Verifico si se modifica o crea la cabecera
                        var itemHeadBuscar = db.ItemHeadIngredient.FirstOrDefault(e => e.id_Item == formulacionCrear.id_Item);

                        if (itemHeadBuscar != null)
                        {
                            // Agregamos los detalles
                            var ingredientDetailList = formulacionesDetalleCrear
                                                    .Where(e => e.id_compoundItem == formulacionCrear.id_Item)
                                                    .ToList();

                            db.ItemIngredient.AddRange(ingredientDetailList);
                        }
                    }

                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = formulacionesDetalleCrear.Count.ToString(),
                        NumDocumento = formulacionesDetalleCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se ha(n) creado {formulacionesDetalleCrear.Count} modelo(s)."
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

        #region Métodos para la descarga de datos
        internal static FileContentResult GetTemplateFileContentResult(int Marca)
        {
            // Generamos una copia del template original
            var contentId = CopyContentFile(m_TemplateAbsolutePath, ExcelXlsmMime);
            var contentPath = FileExcelUploadHelper.GetFileContentPath(contentId);
            var excelParser = new ItemFormulationExcelFileParser(contentPath, ExcelXlsmMime, false);

            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();
                    CopyProductData(Marca, cnn);
                    CopyLineaInventarioData(Marca, cnn);
                    CopyTipoProductoData(Marca, cnn);
                    CopyCategoriaProductoData(Marca, cnn);
                    CopyUnidadMedidaProductoData(Marca, cnn);
                    CopyProductIngredientData(Marca, cnn);
                    CopyClienteExteriorData(Marca, cnn);

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
                    $"FormulaProducto_{Marca}_{DateTime.Now:yyyyMMdd HHmm}",
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

        private static void CopyProductData(int id_Company, OleDbConnection cnn)
        {
            var db = new DBContext();

            // Recuperamos los datos de los productos con formulación
            var items = db.Item
                .Where(t => t.isActive &&
                       t.id_company == id_Company &&
                       t.ItemHeadIngredient.Item.hasFormulation == true && // Item Tiene formulación
                       (t.id_inventoryLine == 2 || t.id_inventoryLine == 12)) // Lineas de inventario: "Producto en proceso" y "Producto Terminado"
                .Include(t => t.MetricType)
                .OrderByDescending(e => e.dateCreate)
                .ThenBy(e => e.name);

            var a = items.Count();
            // Procesamos los datos recuperamos
            if (items.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_PRODUCTO$] ([NOMBRE], [ID], [CODIGO], [COD_TIPO_UMEDIDA]) VALUES (?, ?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);
                    var codTipoUMedidaParam = cmd.Parameters.Add("COD_TIPO_UMEDIDA", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var item in items)
                    {
                        //var codigoTipoUMedida = db.MetricType.Where(e => e.id == item.id_metricType).FirstOrDefault();

                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(item.name);
                        idParam.SetIntegerValue(item.id);
                        codigoParam.SetStringValue(item.masterCode);
                        codTipoUMedidaParam.SetStringValue(item.MetricType.code);

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

        private static void CopyCategoriaProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las categorias de productos
            var categoriaProductos = db.ItemTypeCategory
                .Where(t => t.isActive && t.id_company == empresa);

            // Procesamos los datos recuperamos
            if (categoriaProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_CATEGORIA_PRODUCTO$] ([CODIGO_TIPO], [NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?, ?)";

                    var tipoProdParam = cmd.Parameters.Add("CODIGO_TIPO", OleDbType.VarChar);
                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var categoriaProducto in categoriaProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        var tipoProducto = db.ItemTypeItemTypeCategory.FirstOrDefault(e => e.id_itemTypeCategory == categoriaProducto.id);

                        // Verificación por configuración de la categoria de producto
                        if (tipoProducto == null)
                        {
                            continue;
                        }
                        else
                        {
                            tipoProdParam.SetStringValue(tipoProducto.ItemType.code);
                        }

                        nombreParam.SetStringValue(categoriaProducto.name);
                        idParam.SetIntegerValue(categoriaProducto.id);
                        codigoParam.SetStringValue(categoriaProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyUnidadMedidaProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las unidades de medida de productos
            var unidadMedidaProductos = db.MetricUnit
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.MetricType.code);

            // Procesamos los datos recuperamos
            if (unidadMedidaProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_UNIDAD_MEDIDA$] ([CODIGO_TIPO], [NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?, ?)";

                    var codigoTipo = cmd.Parameters.Add("CODIGO_TIPO", OleDbType.VarChar);
                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var unidadMedidaProducto in unidadMedidaProductos)
                    {
                        var tipoUnidadMedida = db.MetricType.FirstOrDefault(e => e.id == unidadMedidaProducto.id_metricType);

                        // Insertamos los valores en la tabla de Excel
                        codigoTipo.SetStringValue(tipoUnidadMedida.code);
                        nombreParam.SetStringValue(unidadMedidaProducto.name);
                        idParam.SetIntegerValue(unidadMedidaProducto.id);
                        codigoParam.SetStringValue(unidadMedidaProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyProductIngredientData(int id_Company, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las lineas de Inventario
            var items = db.Item
                .Where(t => t.isActive &&
                       t.id_company == id_Company)
                .OrderBy(e => e.id_itemTypeCategory)
                .ThenBy(e => e.id_metricType)
                .ThenBy(e => e.masterCode)
             .Include(t => t.MetricType)
            .Include(t => t.ItemTypeCategory);

            // Procesamos los datos recuperamos
            if (items.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_INGREDIENTE$] ([COD_CATEGORIA_PRODUCTO], [NOMBRE], [ID], [CODIGO], [COD_TIPO_UMEDIDA]) VALUES (?, ?, ?, ?, ?)";

                    var codTipoCategoria = cmd.Parameters.Add("COD_CATEGORIA_PRODUCTO", OleDbType.VarChar);
                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);
                    var codTipoUMedida = cmd.Parameters.Add("COD_TIPO_UMEDIDA", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var item in items)
                    {
                        // Obtenemos el código de categoria
                        //var codigoTipoCategoria = db.ItemTypeCategory.Where(e => e.id == item.id_itemTypeCategory).FirstOrDefault();
                        //Obtenemos el código de tipo de unidad de medida
                        //var codigoTipoUnidadMedida = db.MetricType.Where(e => e.id == item.id_metricType).FirstOrDefault();

                        // Insertamos los valores en la tabla de Excel
                        codTipoCategoria.SetStringValue(item.ItemTypeCategory.code);
                        nombreParam.SetStringValue(item.name);
                        idParam.SetIntegerValue(item.id);
                        codigoParam.SetStringValue(item.masterCode);
                        codTipoUMedida.SetStringValue(item.MetricType.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyClienteExteriorData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los clientes del exterior
            var clientes = db.Person
                .Where(t => t.isActive && t.id_company == empresa
                && t.Rol.FirstOrDefault(r => r.name.Equals("Cliente Exterior")) != null)
                .OrderBy(t => t.fullname_businessName);

            // Procesamos los datos recuperamos
            if (clientes.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_CLIENTE_EXTERIOR$] ([NOMBRE], [ID], [IDENTIFICACION]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var identificacionParam = cmd.Parameters.Add("IDENTIFICACION", OleDbType.VarChar);

                    // Procesamos cada uno de los clientes del exterior
                    foreach (var cliente in clientes)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(cliente.fullname_businessName);
                        idParam.SetIntegerValue(cliente.id);
                        identificacionParam.SetStringValue(cliente.identification_number);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }

        #endregion
    }
}