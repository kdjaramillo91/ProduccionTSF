
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.Controllers;
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
    internal class ItemExcelFileParser : ExcelFileParserBase
    {
        #region Constantes

        private const string m_TemplateRelativePath = "~/App_Data/Templates/ItemTemplate.xlsx";

        #endregion

        #region Campos

        private static readonly string m_TemplateAbsolutePath;

        #endregion

        #region Constructores

        static ItemExcelFileParser()
        {
            m_TemplateAbsolutePath = HttpContext.Current.Server.MapPath(m_TemplateRelativePath);
        }

        internal ItemExcelFileParser(string contentFilePath, string contentFileMime)
            : base(contentFilePath, contentFileMime, true)
        {
        }

        private ItemExcelFileParser(string contentFilePath, string contentFileMime, bool readOnly)
            : base(contentFilePath, contentFileMime, readOnly)
        {
        }

        #endregion

        #region Métodos para la importación de plantillas

        internal ImportResult ParseItem(int activeCompanyId, User activeUser)
        {
            // Recuperamos los datos del EXCEL
            var validTableNames = new string[][]
            {
                new[] { "CARGA_DATOS$", GetTablaItemTypeColumnNames() },
            };

            var dataTables = this.GetDataTables(validTableNames);

            var importResult = ProcesarItem(dataTables[0], activeCompanyId, activeUser);

            return importResult;
        }

        private static string GetTablaItemTypeColumnNames()
        {
            string columnTypes = $"ID_LINEA_INVENTARIO,ID_TIPO_PRODUCTO,ID_CATEGORIA_PRODUCTO,ID_PRESENTACION," +
                $"ID_TIPO_UNIDAD_MEDIDA,CODIGO_BARRA,CODIGO_PRINCIPAL,NOMBRE,NOMBRE_EXTRANJERO,DESCRIPCION,DESCRIPCION_2,ES_COMPRA," +
                $"ES_VENTA,ES_CONSUMIBLE,TIENE_FORMULACION,ID_PESO_PRODUCTO," +
                // Datos generales (ItemGeneral)
                $"ID_GRUPO,ID_SUBGRUPO,ID_CATEGORIA,FABRICANTE,ID_PAIS,ID_MARCA,ID_MODELO,ID_COLOR,ID_TALLA,ID_CLIENTE," +
                $"ID_CERTIFICACION,MES_VIDA_UTIL, " +
                // Inventario de Item 
                $"ES_INVENTARIO,ES_IMPORTADO,ID_BODEGA,ID_UBICACION,STOCK_MINIMO,STOCK_MAXIMO,STOCK_ACTUAL,ID_UNIDAD_MEDIDA," +
                $"REQUIERE_LOTE," +
                // Peso de Items
                $"ID_UNIDAD_MEDIDA_PESO,PESO_BRUTO,PESO_NETO,CONVERSION_KILOS,CONVERSION_LIBRAS,PESO_GLASEO,PORCENTAJE_GLASEO";

            return columnTypes;
        }

        private ImportResult ProcesarItem(DataTable dtCabecera, int activeCompanyId, User activeUser)
        {
            DBContext db = new DBContext();
            // Procesamos las distintas agrupaciones
            var documentosFallidos = new List<ImportResult.DocumentoFallido>();
            var documentosImportados = new List<ImportResult.DocumentoImportado>();

            #region Leer el Excel

            var itemCrear = new List<Item>();
            int numFilasVacias = 0;
            int numFila = 0;

            string valSet = DataProviderSetting.ValueSetting("CDCAP");
            string valueSetting = DataProviderSetting.ValueSetting("PGCMLI");

            foreach (DataRow drCabecera in dtCabecera.Rows)
            {
                numFila++;

                if (numFilasVacias > 5) break;

                try
                {
                    // Verifico la primera información
                    if (!string.IsNullOrEmpty(drCabecera.GetString("NOMBRE")))
                    {
                        #region Datos generales
                        var itemSubir = new Item()
                        {
                            id_inventoryLine = drCabecera.GetInteger("ID_LINEA_INVENTARIO"),
                            id_itemType = drCabecera.GetInteger("ID_TIPO_PRODUCTO"),
                            id_itemTypeCategory = drCabecera.GetInteger("ID_CATEGORIA_PRODUCTO"),
                            id_presentation = drCabecera.GetIntegerNullable("ID_PRESENTACION"),
                            id_metricType = drCabecera.GetIntegerNullable("ID_TIPO_UNIDAD_MEDIDA"),
                            barCode = drCabecera.GetString("CODIGO_BARRA"),
                            name = drCabecera.GetString("NOMBRE"),
                            foreignName = drCabecera.GetString("NOMBRE_EXTRANJERO"),
                            description = drCabecera.GetString("DESCRIPCION"),
                            isPurchased = drCabecera.GetBoolean("ES_COMPRA"),
                            isSold = drCabecera.GetBoolean("ES_VENTA"),
                            inventoryControl = drCabecera.GetBoolean("ES_INVENTARIO"),
                            hasFormulation = drCabecera.GetBoolean("TIENE_FORMULACION"),
                            id_itemWeight = drCabecera.GetIntegerNullable("ID_PESO_PRODUCTO"),
                            description2 = drCabecera.GetString("DESCRIPCION_2"),
                            isConsumed = drCabecera.GetBoolean("ES_CONSUMIBLE"),
                            isActive = true,
                            id_company = activeCompanyId,
                            id_userCreate = activeUser.id,
                            dateCreate = DateTime.Now,
                            id_userUpdate = activeUser.id,
                            dateUpdate = DateTime.Now,
                        };
                        #endregion

                        #region Item General

                        var itemGeneralSubir = new ItemGeneral()
                        {
                            id_group = drCabecera.GetIntegerNullable("ID_GRUPO"),
                            id_subgroup = drCabecera.GetIntegerNullable("ID_SUBGRUPO"),
                            id_groupCategory = drCabecera.GetIntegerNullable("ID_CATEGORIA"),
                            manufacturer = drCabecera.GetString("FABRICANTE"),
                            id_countryOrigin = drCabecera.GetIntegerNullable("ID_PAIS"),
                            id_trademark = drCabecera.GetIntegerNullable("ID_MARCA"),
                            id_trademarkModel = drCabecera.GetIntegerNullable("ID_MODELO"),
                            id_color = drCabecera.GetIntegerNullable("ID_COLOR"),
                            id_size = drCabecera.GetIntegerNullable("ID_TALLA"),
                            id_Person = drCabecera.GetIntegerNullable("ID_CLIENTE"),
                            id_certification = drCabecera.GetIntegerNullable("ID_CERTIFICACION"),
                            mesVidaUtil = drCabecera.GetIntegerNullable("MES_VIDA_UTIL")
                        };

                        // Agregamos a objeto principal
                        itemSubir.ItemGeneral = itemGeneralSubir;

                        #endregion

                        #region Inventario de item

                        if (itemSubir.inventoryControl)
                        {
                            var itemInventarioSubir = new ItemInventory()
                            {
                                isImported = drCabecera.GetBoolean("ES_IMPORTADO"),
                                id_warehouse = drCabecera.GetInteger("ID_BODEGA"),
                                id_warehouseLocation = drCabecera.GetInteger("ID_UBICACION"),
                                minimumStock = drCabecera.GetDecimal("STOCK_MINIMO"),
                                maximumStock = drCabecera.GetDecimal("STOCK_MAXIMO"),
                                currentStock = 0,
                                id_metricUnitInventory = drCabecera.GetInteger("ID_UNIDAD_MEDIDA"),
                                requiresLot = drCabecera.GetBoolean("REQUIERE_LOTE"),
                            };

                            // Agregamos a objeto principal
                            itemSubir.ItemInventory = itemInventarioSubir;
                        }

                        #endregion

                        #region ItemWeightConversionFreezen
                        // Buscamos la linea de inventario
                        var inventoryLine = db.InventoryLine.Where(e => e.id == itemSubir.id_inventoryLine).FirstOrDefault();

                        // Verificamos si la LI pertenece a 'Producto terminado' para ingresar a la tabla 'Conversión de congelado'
                        if (inventoryLine != null && inventoryLine.code == "PT")
                        {
                            // Creamos la rama del objeto
                            var itemPesoConversionCongelado = new ItemWeightConversionFreezen()
                            {
                                id_MetricUnit = drCabecera.GetInteger("ID_UNIDAD_MEDIDA_PESO"),
                                itemWeightGrossWeight = drCabecera.GetDecimal("PESO_BRUTO"),
                                itemWeightNetWeight = drCabecera.GetDecimal("PESO_NETO"),
                                conversionToKilos = drCabecera.GetDecimal("CONVERSION_KILOS"),
                                conversionToPounds = drCabecera.GetDecimal("CONVERSION_LIBRAS"),
                                weightWithGlaze = drCabecera.GetDecimal("PESO_GLASEO"),
                                glazePercentage = drCabecera.GetDecimal("PORCENTAJE_GLASEO"),
                            };

                            // Agregamos al objeto principal
                            itemSubir.ItemWeightConversionFreezen = itemPesoConversionCongelado;
                        }

                        #endregion

                        #region Cálculo para 'MasterCode'

                        //// Formación de nuevo código en base a EXCEl
                        string itemMasterCodeExcel = !string.IsNullOrEmpty(drCabecera.GetString("CODIGO_PRINCIPAL"))
                            ? drCabecera.GetString("CODIGO_PRINCIPAL")
                            : string.Empty;

                        itemSubir.masterCode = itemMasterCodeExcel;

                        #endregion

                        // Se agrega a la lista auxiliar
                        itemCrear.Add(itemSubir);
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
                        Descripcion = $"Error agregando fila a tabla de datos: Productos. Fila: {numFila}. {ex.Message}",
                    });
                }
            }

            #endregion

            // Si la lista está vacía, retornamos como error.
            if (!itemCrear.Any())
            {
                documentosFallidos.Add(new ImportResult.DocumentoFallido()
                {
                    //Filas = string.Empty, numFila.ToString(),
                    Filas = string.Empty,
                    Descripcion = $"El documento está vacío.",
                });
            }

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

            if (itemCrear.Count > 0)
            {
                for (int i = 0; i < itemCrear.Count; i++)
                {
                    var productoCrear = itemCrear.ElementAt(i);

                    try
                    {
                        // Validamos los campos obligatorios
                        //Código
                        if ((valueSetting == "SI") && (string.IsNullOrEmpty(productoCrear.masterCode)))
                        {
                            throw new Exception("El código es obligatorio para crear producto. Revisar parametrización de código automático.");
                        }
                        // Linea de inventario
                        if (productoCrear.id_inventoryLine <= 0)
                        {
                            throw new Exception("La linea de inventario es obligatoria para crear producto.");
                        }
                        // tipo de producto
                        if (productoCrear.id_itemType <= 0)
                        {
                            throw new Exception("El tipo de producto es obligatorio para crear producto.");
                        }
                        // Categoria 1
                        if (productoCrear.id_itemTypeCategory <= 0)
                        {
                            throw new Exception("La categoria es obligatoria para crear producto.");
                        }
                        // Longitud de código de barra (50)
                        if ((!string.IsNullOrEmpty(productoCrear.barCode)) && (productoCrear.barCode.Length > 13))
                        {
                            throw new Exception("El código de barra no debe tener longitud mayor a 13 caracteres.");
                        }

                        // Nombre de producto
                        if (string.IsNullOrEmpty(productoCrear.name))
                        {
                            throw new Exception("El nombre de producto es obligatorio para crear producto.");
                        }
                        // Longitud de nombre de producto
                        if (productoCrear.name.Length > 250)
                        {
                            throw new Exception("El nombre no debe tener longitud mayor a 250 caracteres.");
                        }

                        // Longitud de nombre extranjero de producto
                        if ((!string.IsNullOrEmpty(productoCrear.foreignName)) && (productoCrear.foreignName.Length > 250))
                        {
                            throw new Exception("El nombre extranjero no debe tener longitud mayor a 250 caracteres.");
                        }

                        // Longitud de descripción de producto
                        if ((!string.IsNullOrEmpty(productoCrear.description)) && (productoCrear.description.Length > 250))
                        {
                            throw new Exception("El nombre extranjero no debe tener longitud mayor a 250 caracteres.");
                        }

                        // Longitud de descripción 2 de producto
                        if ((!string.IsNullOrEmpty(productoCrear.description2)) && (productoCrear.description2.Length > 250))
                        {
                            throw new Exception("El nombre extranjero no debe tener longitud mayor a 250 caracteres.");
                        }

                        // Validamos que el código no esté repetido dentro de la base de datos
                        var productoBase = db.Item.FirstOrDefault(e => e.masterCode == productoCrear.masterCode);
                        if (productoBase != null)
                        {
                            throw new Exception($"El código: {productoBase.masterCode} ya existe o está inactivo dentro de la base de datos.");
                        }

                        // Validamos que el nombre no esté repetido dentro de la base de datos
                        var productoNombreBase = db.Item.FirstOrDefault(e => e.name == productoCrear.name);
                        if (productoBase != null)
                        {
                            throw new Exception($"El nombre: {productoNombreBase.name} ya existe o está inactivo dentro de la base de datos.");
                        }

                        // Validaciones item General (Longitud de fabricante)
                        if ((!string.IsNullOrEmpty(productoCrear.ItemGeneral.manufacturer)) && (productoCrear.ItemGeneral.manufacturer.Length > 250))
                        {
                            throw new Exception("El fabricante no debe tener longitud mayor a 250 caracteres.");
                        }

                        // Verificación de pesos por Linea de Inventario 'Producto Terminado'
                        var lineaInventario = db.InventoryLine.Where(e => e.id == productoCrear.id_inventoryLine).FirstOrDefault();
                        if (lineaInventario != null && lineaInventario.code == "PT")
                        {
                            // Verificación de peso bruto
                            if (productoCrear.ItemWeightConversionFreezen.itemWeightGrossWeight <= 0)
                            {
                                throw new Exception("El peso bruto no puede ser menor o igual a cero.");
                            }

                            // Verificación de Peso Neto
                            if (productoCrear.ItemWeightConversionFreezen.itemWeightNetWeight <= 0)
                            {
                                throw new Exception("El peso neto no puede ser menor o igual a cero.");
                            }

                            // Verificación de Conversión de Kilos 
                            if (productoCrear.ItemWeightConversionFreezen.id_MetricUnit == 1 &&
                                (productoCrear.ItemWeightConversionFreezen.conversionToKilos < 1 || productoCrear.ItemWeightConversionFreezen.conversionToKilos > 1))
                            {
                                throw new Exception("Si el tipo de peso es Kilogramos, la conversión a kilos no puede ser diferente de uno.");
                            }

                            if (productoCrear.ItemWeightConversionFreezen.conversionToKilos <= 0)
                            {
                                throw new Exception("La conversión a kilos no puede ser menor o igual a cero.");
                            }

                            // Verificación de Conversión de libras 
                            if (productoCrear.ItemWeightConversionFreezen.id_MetricUnit == 4 &&
                                (productoCrear.ItemWeightConversionFreezen.conversionToPounds < 1 || productoCrear.ItemWeightConversionFreezen.conversionToPounds > 1))
                            {
                                throw new Exception("Si el tipo de peso es Libras, la conversión a kilos no puede ser diferente de uno.");
                            }

                            if (productoCrear.ItemWeightConversionFreezen.conversionToPounds <= 0)
                            {
                                throw new Exception("La conversión a libras no puede ser menor o igual a cero.");
                            }

                            // Verificación de Peso con Glaseo
                            if (productoCrear.ItemWeightConversionFreezen.weightWithGlaze < 0)
                            {
                                throw new Exception("El peso con glaseo no puede ser menor a cero.");
                            }

                            // Verificación de Porcentaje Glaseo
                            if (productoCrear.ItemWeightConversionFreezen.glazePercentage < 0)
                            {
                                throw new Exception("El porcentaje de glaseo no puede ser menor a cero.");
                            }
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
                throw new Exception("Sin información para crear productos.");
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

            #region Guardar información PRINCIPAL en la base

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var lista = new List<InventoryLine>();
                    foreach (var item in itemCrear)
                    {
                        #region Cálculo para 'MasterCode'
                        var index = lista.FindIndex(e => e.id == item.id_inventoryLine);
                        InventoryLine inventoryLine;
                        if (index >= 0)
                        {
                            inventoryLine = lista.ElementAt(index);
                        }
                        else
                        {
                            inventoryLine = db.InventoryLine.FirstOrDefault(it => it.id == item.id_inventoryLine);
                            lista.Add(inventoryLine);
                            index = lista.Count - 1;
                        }


                        // Formación de nuevo código en base a EXCEl
                        string itemMasterCodeExcel = !string.IsNullOrEmpty(item.masterCode)
                            ? item.masterCode
                            : string.Empty;

                        string itemMasterCode = valueSetting == "NO"
                            ? BuildCopiedMasterCode(inventoryLine)
                            : itemMasterCodeExcel;

                        item.masterCode = itemMasterCode;
                        lista.ElementAt(index).sequential = inventoryLine.sequential + 1;

                        //if(lista.Any(e ))

                        #endregion

                        #region Actualización de nombre de producto por certificación

                        var aCertification = db.Certification.FirstOrDefault(fod => fod.id == item.ItemGeneral.id_certification);
                        if (!string.IsNullOrEmpty(aCertification?.idProducto))
                        {
                            item.name = aCertification.idProducto + "-" + item.name;
                        }
                        item.description = item.name;

                        #endregion

                        #region Cálculo de aprobación de código auxiliar

                        if (valSet == "YES")
                        {
                            if (inventoryLine.code == "PT" || inventoryLine.code == "PP")
                            {
                                var strCodeAux = "";
                                //Tipo de Producto
                                strCodeAux = GetAuxiliarCodeForProduct(item);

                                if (strCodeAux != "ERROR" && strCodeAux.Length > 8)
                                {
                                    item.auxCode = strCodeAux;
                                }
                            }
                        }

                        #endregion

                        db.Item.Add(item);
                        //db.Entry(itemNew).State = EntityState.Added;
                    }

                    foreach (var item in lista)
                    {
                        db.InventoryLine.Attach(item);
                        db.Entry(item).State = EntityState.Modified;
                    }

                    // Ingresa a la BD
                    db.SaveChanges();
                    trans.Commit();
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

            #region Guardar información de la MIGRACIÓN en la base

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in itemCrear)
                    {
                        var itemMigrate = db.Item.FirstOrDefault(e => e.masterCode == item.masterCode);

                        #region Migración de productos por Linea de Inventario 'PRODUCTO TERMINADO'

                        MigrationItem migrationItem = db.MigrationItem.FirstOrDefault(fod => fod.id_item == itemMigrate.id);
                        if (migrationItem == null && itemMigrate.InventoryLine.code == "PT")
                        {
                            migrationItem = new MigrationItem
                            {
                                id_item = itemMigrate.id,
                                id_userCreate = activeUser.id,
                                dateCreate = DateTime.Now
                            };
                            db.MigrationItem.Add(migrationItem);
                        }

                        #endregion
                    }

                    // Ingresa a la BD
                    db.SaveChanges();
                    trans.Commit();

                    documentosImportados.Add(new ImportResult.DocumentoImportado()
                    {
                        Filas = itemCrear.Count.ToString(),
                        NumDocumento = itemCrear.Count,
                        FechaProceso = DateTime.Today,
                        Descripcion = $"Se han creado {itemCrear.Count} elementos."
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

        private string BuildCopiedMasterCode(InventoryLine inventoryLine)
        {
            DBContext db = new DBContext();
            string itemMasterCode = string.Empty;
            string[] congLineasInvCods = new string[] { };
            var settLineaInventario = db.Setting.FirstOrDefault(r => r.code == "PGCMLI");
            if (settLineaInventario != null)
            {
                congLineasInvCods = settLineaInventario
                                        .SettingDetail
                                        .Select(r => r.value)
                                        .ToArray();
            }

            if (inventoryLine != null)
            {
                int codeLength = 6;
                if (congLineasInvCods.Contains(inventoryLine.code))
                {
                    itemMasterCode = inventoryLine.code + "-";
                }
                else
                {
                    itemMasterCode = $"{inventoryLine.code}{inventoryLine.sequential.ToString().PadLeft(codeLength, '0')}";
                }
            }
            return itemMasterCode;
        }

        public string GetAuxiliarCodeForProduct(Item it)
        {
            var strCodeAux = "";
            string codAuxPerson = "-----";
            try
            {
                DBContext db = new DBContext();
                ItemGeneral _itemGeneral = it.ItemGeneral;
                if (_itemGeneral != null)
                {
                    int? idRolForeingCustomer = db.Rol.FirstOrDefault(r => r.name == "Cliente Exterior")?.id;
                    if (idRolForeingCustomer != null)
                    {

                        int[] idsGroupRolDetail = db.GroupRolDetail
                                                .Where(r => r.id_Rol == idRolForeingCustomer)
                                                .Select(r => r.GroupRol.id)
                                                .ToArray();

                        PersonGroupRol idPersonGroupRol = db.PersonGroupRol
                                                                .FirstOrDefault(
                                                                        r => r.id_Person == _itemGeneral.id_Person
                                                                        && idsGroupRolDetail.Contains(r.id_GroupRol)
                                                                );

                        codAuxPerson = idPersonGroupRol?.PersonSharedInfo?.FirstOrDefault()?.codePerson ?? "-----";
                    }
                }

                strCodeAux += db.ItemType.FirstOrDefault(fod => fod.id == it.id_itemType)?.code ?? "";
                strCodeAux += db.ItemGroup.FirstOrDefault(fod => fod.id == it.ItemGeneral.id_group)?.code ?? "";
                strCodeAux += db.ItemGroup.FirstOrDefault(fod => fod.id == it.ItemGeneral.id_subgroup)?.code ?? "";
                strCodeAux += db.ItemColor.FirstOrDefault(fod => fod.id == it.ItemGeneral.id_color)?.code ?? "";
                strCodeAux += db.ItemSize.FirstOrDefault(fod => fod.id == it.ItemGeneral.id_size)?.code ?? "";
                strCodeAux += db.ItemTypeCategory.FirstOrDefault(fod => fod.id == it.id_itemTypeCategory)?.code ?? "";
                strCodeAux += db.ItemTrademark.FirstOrDefault(fod => fod.id == it.ItemGeneral.id_trademark)?.code ?? "";
                //strCodeAux += codAuxPerson;
                strCodeAux += db.ItemTrademarkModel.FirstOrDefault(fod => fod.id == it.ItemGeneral.id_trademarkModel)?.code ?? "";
                strCodeAux += db.Presentation.FirstOrDefault(fod => fod.id == it.id_presentation)?.code ?? "";

                if (strCodeAux == "" && strCodeAux.Length < 8)
                {
                    strCodeAux = "ERROR";
                }

            }
            catch (Exception)
            {
                strCodeAux = "ERROR";
            }

            var stringLength = strCodeAux?.Count() ?? 0;
            var maxLengthGloss = ((stringLength > 25) ? 25 : stringLength);
            return strCodeAux.Substring(0, maxLengthGloss);
        }

        #endregion

        #region Métodos para la descarga de datos

        internal static FileContentResult GetTemplateFileContentResult(
            int empresa)
        {
            // Generamos una copia del template original
            var contentId = CopyContentFile(m_TemplateAbsolutePath, ExcelXlsmMime);
            var contentPath = FileExcelUploadHelper.GetFileContentPath(contentId);
            var excelParser = new ItemExcelFileParser(contentPath, ExcelXlsmMime, false);

            // Copiamos los datos de referencia en el archivo de Excel
            try
            {
                using (var cnn = new OleDbConnection(excelParser.ConnectionString))
                {
                    cnn.Open();

                    CopyLineaInventarioData(empresa, cnn);
                    CopyTipoProductoData(empresa, cnn);
                    CopyCategoriaProductoData(empresa, cnn);
                    CopyPresentacionProductoData(empresa, cnn);
                    CopyTipoUnidadMedidaProductoData(empresa, cnn);
                    CopyUnidadMedidaProductoData(empresa, cnn);                    
                    CopyPesoProductoData(empresa, cnn);
                    CopyUnidadMedidaPesoData(empresa, cnn);
                    CopyGrupoProductoData(empresa, cnn);
                    CopySubGrupoProductoData(empresa, cnn);
                    CopyGrupoCategoriaData(empresa, cnn);
                    CopyPaisData(empresa, cnn);
                    CopyMarcaData(empresa, cnn);
                    CopyModeloData(empresa, cnn);
                    CopyColorData(empresa, cnn);
                    CopyTallasData(empresa, cnn);
                    CopyClienteExteriorData(empresa, cnn);
                    CopyCertificacionesData(cnn);
                    CopyWarehouseData(empresa, cnn);
                    CopyWarehouseLocationData(empresa, cnn);
                    CopyParametersData(cnn);

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
                    $"Producto_{empresa}_{DateTime.Now:yyyyMMdd HHmm}",
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

        #region Categoria de Item Auxiliar

        public class ItemTypeCategorySub
        {
            public string CodigoTipoProducto { get; set; }
            public string NombreCategoria { get; set; }
            public int IdCategoria { get; set; }
            public string CodigoCategoria { get; set; }

        }

        #endregion


        private static void CopyCategoriaProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            var categoriasDescarga = new List<ItemTypeCategorySub>();

            // Categoria de Items
            var categoriaItems = db.ItemTypeItemTypeCategory.Where(e => e.ItemType.isActive && e.ItemType.id_company == empresa);
            categoriaItems = categoriaItems.Where(e => e.ItemTypeCategory.isActive && e.ItemTypeCategory.id_company == empresa);

            // Recorro los tipo para la categoria auxiliar
            foreach (var item in categoriaItems)
            {
                var categoriaAuxiliar = new ItemTypeCategorySub()
                {
                    CodigoTipoProducto = item.ItemType.code,
                    NombreCategoria = item.ItemTypeCategory.name,
                    IdCategoria = item.id_itemTypeCategory,
                    CodigoCategoria = item.ItemTypeCategory.code,
                };

                categoriasDescarga.Add(categoriaAuxiliar);
            }

            // Orden para enviar a excel
            categoriasDescarga = categoriasDescarga.OrderBy(e => e.CodigoTipoProducto).ToList();

            // Procesamos los datos recuperamos
            if (categoriasDescarga.Count() > 0)
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
                    foreach (var categoriaProducto in categoriasDescarga)
                    {
                        // Insertamos los valores en la tabla de Excel
                        tipoProdParam.SetStringValue(categoriaProducto.CodigoTipoProducto);
                        nombreParam.SetStringValue(categoriaProducto.NombreCategoria);
                        codigoParam.SetStringValue(categoriaProducto.CodigoCategoria);
                        idParam.SetIntegerValue(categoriaProducto.IdCategoria);                        

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private static void CopyPresentacionProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las presentaciones de productos
            var presentacionProductos = db.Presentation
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (presentacionProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_PRESENTACION$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var presentacionProducto in presentacionProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(presentacionProducto.name);
                        idParam.SetIntegerValue(presentacionProducto.id);
                        codigoParam.SetStringValue(presentacionProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private static void CopyTipoUnidadMedidaProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las unidades de medida de productos
            var unidadMedidaProductos = db.MetricType
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (unidadMedidaProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_TIPO_UNIDAD_MEDIDA$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var unidadMedidaProducto in unidadMedidaProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(unidadMedidaProducto.name);
                        idParam.SetIntegerValue(unidadMedidaProducto.id);
                        codigoParam.SetStringValue(unidadMedidaProducto.code);

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

        private static void CopyUnidadMedidaPesoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los unidad de pesos de productos
            int? id_metricType = db.MetricType.FirstOrDefault(fod => fod.code.Equals("PES01")).id;
            MetricUnit metricUnitNoDefinido = new Models.MetricUnit
                            {
                                id = 999,
                                code = "00",
                                name = "No Definido",
                                description = "No Definido"
                            };

            var unidadPesoProductos = db.MetricUnit
                .Where(w => w.id_metricType == id_metricType
                        && w.id_company == empresa & w.isActive)
                .ToList();

            if (unidadPesoProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_UNIDAD_PESO$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";
                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var unidadMedidaProducto in unidadPesoProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(unidadMedidaProducto.name);
                        idParam.SetIntegerValue(unidadMedidaProducto.id);
                        codigoParam.SetStringValue(unidadMedidaProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyPesoProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los pesos de productos
            var pesoProductos = db.ItemWeight
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (pesoProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_PESO$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var pesoProducto in pesoProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(pesoProducto.name);
                        idParam.SetIntegerValue(pesoProducto.id);
                        codigoParam.SetStringValue(pesoProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyGrupoProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los grupo de productos
            var grupoProductos = db.ItemGroup
                .Where(t => t.isActive && t.id_company == empresa && t.id_parentGroup == null)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (grupoProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_GRUPO$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var grupoProducto in grupoProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(grupoProducto.name);
                        idParam.SetIntegerValue(grupoProducto.id);
                        codigoParam.SetStringValue(grupoProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private static void CopySubGrupoProductoData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los subgrupo de productos
            var sunGrupoProductos = db.ItemGroup
                .Where(t => t.isActive && t.id_company == empresa && t.id_parentGroup != null)
                .OrderBy(t => t.id_parentGroup);

            // Procesamos los datos recuperamos
            if (sunGrupoProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_SUBGRUPO$] ([CODIGO_GRUPO], [CODIGO_GRUPO_SUBGRUPO], [NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?, ?, ?)";

                    var codigoGrupo = cmd.Parameters.Add("CODIGO_GRUPO", OleDbType.VarChar);
                    var codigoGrupoSubGrupo = cmd.Parameters.Add("CODIGO_GRUPO_SUBGRUPO", OleDbType.VarChar);
                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var sunGrupoProducto in sunGrupoProductos)
                    {
                        var subGrupo = db.ItemGroup.FirstOrDefault(t => t.id == sunGrupoProducto.id_parentGroup);
                        // Insertamos los valores en la tabla de Excel
                        codigoGrupo.SetStringValue(subGrupo.code);
                        codigoGrupoSubGrupo.SetStringValue($"{subGrupo.code}|{sunGrupoProducto.name}");
                        nombreParam.SetStringValue(sunGrupoProducto.name);
                        idParam.SetIntegerValue(sunGrupoProducto.id);
                        codigoParam.SetStringValue(sunGrupoProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyGrupoCategoriaData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los grupo de categorias de los productos
            var grupoCategoriaProductos = db.ItemGroupCategory
                .Where(t => t.isActive && t.id_company == empresa);

            // Procesamos los datos recuperamos
            if (grupoCategoriaProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_CATEGORIA$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los motivos de consumo
                    foreach (var grupoCategoriaProducto in grupoCategoriaProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(grupoCategoriaProducto.name);
                        idParam.SetIntegerValue(grupoCategoriaProducto.id);
                        codigoParam.SetStringValue(grupoCategoriaProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }
        private static void CopyPaisData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los grupo de categorias de los productos
            var paises = db.Country
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (paises.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_PAIS$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los paises
                    foreach (var pais in paises)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(pais.name);
                        idParam.SetIntegerValue(pais.id);
                        codigoParam.SetStringValue(pais.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }
        private static void CopyMarcaData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las marcas de los productos
            var marcaProductos = db.ItemTrademark
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (marcaProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_MARCA$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de las marca de los productos
                    foreach (var marcaProducto in marcaProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(marcaProducto.name);
                        idParam.SetIntegerValue(marcaProducto.id);
                        codigoParam.SetStringValue(marcaProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }
        private static void CopyModeloData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los modelos de los productos
            var modeloProductos = db.ItemTrademarkModel
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (modeloProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_MODELO$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los modelos de los productos
                    foreach (var modeloProducto in modeloProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(modeloProducto.name);
                        idParam.SetIntegerValue(modeloProducto.id);
                        codigoParam.SetStringValue(modeloProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }
        private static void CopyColorData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de los colores de los productos
            var colorProductos = db.ItemColor
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.name);

            // Procesamos los datos recuperamos
            if (colorProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_COLOR$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de los colores de los productos
                    foreach (var colorProducto in colorProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(colorProducto.name);
                        idParam.SetIntegerValue(colorProducto.id);
                        codigoParam.SetStringValue(colorProducto.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }
        private static void CopyTallasData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las tallas de los productos
            var tallaProductos = db.ItemSize
                .Where(t => t.isActive && t.id_company == empresa)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (tallaProductos.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_TALLA$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de las tallas de los productos
                    foreach (var tallaProducto in tallaProductos)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(tallaProducto.name);
                        idParam.SetIntegerValue(tallaProducto.id);
                        codigoParam.SetStringValue(tallaProducto.code);

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

        private static void CopyCertificacionesData(OleDbConnection cnn)
        {
            var db = new DBContext();
            // Recuperamos los datos de las tallas de los productos
            var certifications = db.Certification
                .Where(t => t.isActive)
                .OrderBy(t => t.code);

            // Procesamos los datos recuperamos
            if (certifications.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_CERTIFICACION$] ([NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?)";

                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    // Procesamos cada uno de las tallas de los productos
                    foreach (var certification in certifications)
                    {
                        // Insertamos los valores en la tabla de Excel
                        nombreParam.SetStringValue(certification.name);
                        idParam.SetIntegerValue(certification.id);
                        codigoParam.SetStringValue(certification.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }

        private static void CopyWarehouseData(int empresa, OleDbConnection cnn)
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

        private static void CopyWarehouseLocationData(int empresa, OleDbConnection cnn)
        {
            var db = new DBContext();

            // Recuperampos los datos de tipos de bodegas
            var bodegas = db.WarehouseLocation
                .Where(e => e.isActive && e.id_company == empresa)
                .OrderBy(e => e.Warehouse.code);

            if (bodegas.Count() > 0)
            {
                using (var cmd = cnn.CreateCommand())
                {
                    // Preparamos el comando para la inserción de datos...
                    cmd.CommandText = "INSERT INTO [DATA_UBICACION$] ([COD_BODEGA], [NOMBRE], [ID], [CODIGO]) VALUES (?, ?, ?, ?)";

                    var codigoBodeParam = cmd.Parameters.Add("COD_BODEGA", OleDbType.VarChar);
                    var nombreParam = cmd.Parameters.Add("NOMBRE", OleDbType.VarChar);
                    var idParam = cmd.Parameters.Add("ID", OleDbType.VarChar);
                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);

                    foreach (var bodega in bodegas)
                    {
                        // Insertamos los valores en la tabla de Excel
                        codigoBodeParam.SetStringValue(bodega.Warehouse.code);
                        nombreParam.SetStringValue(bodega.name);
                        idParam.SetIntegerValue(bodega.id);
                        codigoParam.SetStringValue(bodega.code);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CopyParametersData(OleDbConnection cnn)
        {
            var db = new DBContext();

            var settLineaInventario = db.Setting
                .Where(r => r.code == "PGCMLI")
                .OrderBy(e => e.code);

            if (settLineaInventario != null)
            {// Preparamos el comando para la inserción de datos...
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO [PARAMETROS$] ([CODIGO], [VALOR]) VALUES (?, ?)";

                    var codigoParam = cmd.Parameters.Add("CODIGO", OleDbType.VarChar);
                    var valorParam = cmd.Parameters.Add("VALOR", OleDbType.VarChar);

                    foreach (var setting in settLineaInventario)
                    {
                        // Insertamos los valores en la tabla de Excel
                        codigoParam.SetStringValue(setting.code);
                        valorParam.SetStringValue(setting.value);

                        cmd.ExecuteNonQuery();
                    }
                }

            }
        }
        #endregion
    }
}
