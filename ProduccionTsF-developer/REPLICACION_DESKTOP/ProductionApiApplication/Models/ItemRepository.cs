using ProductionApiApplication.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MigracionProduccionCIWebApi.Models
{
    public class ItemRepository
    {
        private DBContext db = new DBContext();
        private  DBContextCI dbCI = new DBContextCI();
        public ItemRepository()
        {
            //ItemRepository
        }

        public void UpdateMigrationItem(MigrationItem migrationItem, string message)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var userCreateAux = db.User.FirstOrDefault(fod => fod.username == "admin" /*|| fod.id == 1*/ || fod.username.Contains("admin"));// ?? db.User.FirstOrDefault();
                    HistoryMigrationItem historyMigrationItemNew = new HistoryMigrationItem
                    {
                        id_migrationItem = migrationItem.id,
                        id_item = migrationItem.id_item,
                        message = message,
                        id_userCreateMigrationItem = (int) userCreateAux?.id,
                        dateCreateMigrationItem = DateTime.Now,
                        dateCreate = DateTime.Now,
                        id_userCreate = (int)userCreateAux?.id
                    };
                    db.HistoryMigrationItem.Add(historyMigrationItemNew);
                    db.Entry(historyMigrationItemNew).State = EntityState.Added;
                    
                    db.SaveChanges();

                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
        }

        public void DeleteMigrationInsertHistoryItem(MigrationItem migrationItem, string message)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    // CAMPOS DE AUDITORIA 
                    var userCreateAux = db.User.FirstOrDefault(fod => fod.username == "admin" /*|| fod.id == 1*/ || fod.username.Contains("admin"));// ?? db.User.FirstOrDefault();
                    HistoryMigrationItem historyMigrationItemTmp = new HistoryMigrationItem();
                    historyMigrationItemTmp.id_migrationItem = migrationItem.id;
                    historyMigrationItemTmp.id_item = migrationItem.id_item;
                    historyMigrationItemTmp.id_userCreateMigrationItem = migrationItem.id_userCreate;
                    historyMigrationItemTmp.dateCreateMigrationItem = migrationItem.dateCreate;
                    historyMigrationItemTmp.message = message;
                    historyMigrationItemTmp.id_userCreate = migrationItem.id_userCreate;
                    historyMigrationItemTmp.dateCreate = DateTime.Now;

                    db.HistoryMigrationItem.Add(historyMigrationItemTmp);
                    //Delete
                    db.MigrationItem.Remove(migrationItem);
                    db.Entry(migrationItem).State = EntityState.Deleted;
                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    Console.Write(e.Message);
                }
            }
               
        }

        public AnswerMigration AddItem(MigrationItem migrationItem)
        {
			List<ItemHomologation> itemHomologation = db.Database.SqlQuery<ItemHomologation>("SELECT * FROM ItemHomologation").ToList<ItemHomologation>();

			var messageCommon = "Item: " + migrationItem.Item.name ;

            bool newValue = false;
            var answerMigration = new AnswerMigration();
            var item = db.Item.FirstOrDefault(fod => fod.id == migrationItem.id_item);
            var settingInvCons = db.Setting.FirstOrDefault(fod => fod.code == "PRCIINV")?.value ?? "NO"; 

            if (item == null)
            {
                answerMigration.message = messageCommon + ". No existe actualmente el Item, se descarto su migración";
                DeleteMigrationInsertHistoryItem(migrationItem, answerMigration.message);
                return answerMigration;
            }

            using (DbContextTransaction trans = dbCI.Database.BeginTransaction())
            {
                try
                {
                    var itemInventoryDetail = item.InventoryMoveDetail?.Where(w => w.id_inventoryMoveDetailNext == null)?.OrderByDescending(w => w.dateCreate)?.FirstOrDefault() ?? null;

                    var itemTaxation = item.ItemTaxation.Join(db.TaxType,
                                                it => it.id_taxType,
                                                tt => tt.id,
                                                (it, tt) => new { IT = it, TT = tt }).Where(ittt => ittt.TT.code == "2" && ittt.IT.Rate.code != "0");




                    var itemGeneral = item.ItemGeneral;

                    var productoCI = dbCI.TblIvProducto.FirstOrDefault(fod => fod.CCiProducto == item.masterCode.ToString());
                   

                    if(productoCI == null)
                    {
                        productoCI = new TblIvProducto();
                        newValue = true;
                        productoCI.CCiProducto = item.masterCode.ToString();
                    }



                    var itemGroup = item.ItemGeneral?.ItemGroup;
                    var itemSubGroup = item.ItemGeneral?.ItemGroup;


					var clas1 = (from di in itemHomologation
								 where di.code.Equals("CLAS1") && di.homologationType.Equals("PR") && di.value.Equals(item.InventoryLine.code.ToString())
								 select new { valuehomologation = di.valuehomologation, descripcion = di.descriptionhomologation }).FirstOrDefault();

				
					string valuehomologationClas1 = null;
					string descripcionClas1 = null;

					if (clas1 == null)
					{
						//	throw new Exception("El código '" + item.InventoryLine.code + "'. No existe en la tabla de Homologación para la clasificación 1.");
						answerMigration.message = "El código '" + item.InventoryLine.code + "'. No existe en la tabla de Homologación para la clasificación 1.";
						return answerMigration;
					}
					else
					{
						valuehomologationClas1 = clas1.valuehomologation;
						descripcionClas1 = clas1.descripcion;
					}

					if (valuehomologationClas1 != null)
                    {
                        var clasificacion1ProductoCI = dbCI.TblIvClasifProducto1.FirstOrDefault(fod => fod.CCiClasificacion1 == valuehomologationClas1);					
						if (clasificacion1ProductoCI == null)
                        {
                            clasificacion1ProductoCI = new TblIvClasifProducto1();
							clasificacion1ProductoCI.CCiClasificacion1 = valuehomologationClas1;
                            clasificacion1ProductoCI.CDsClasificacion1 = descripcionClas1;
							clasificacion1ProductoCI.CCeClasificacion = "A";
                            clasificacion1ProductoCI.CSNObligaIngresoCategoria = "N";
                            clasificacion1ProductoCI.CCtObligaIngresoCategoria = string.Empty;
                            dbCI.TblIvClasifProducto1.Add(clasificacion1ProductoCI);
                            dbCI.Entry(clasificacion1ProductoCI).State = EntityState.Added;
                        }
                    }

                    productoCI.CCiClasificacion1 = valuehomologationClas1;

					var clas2 = (from di in itemHomologation
								 where di.code.Equals("CLAS2") && di.homologationType.Equals("PR") && di.value.Equals(item.ItemType.code.ToString())
								 select new { valuehomologation = di.valuehomologation, descripcion = di.descriptionhomologation }).FirstOrDefault();

					string valuehomologationClas2 = null;
					string descripcionClas2 = null;

					if (clas2 == null)
					{
						//	throw new Exception("El código '" + item.InventoryLine.code + "'. No existe en la tabla de Homologación para la clasificación 2.");
						answerMigration.message = "El código '" + item.ItemType.code + "'. No existe en la tabla de Homologación para la clasificación 2.";
						return answerMigration;
					}
					else
					{
						valuehomologationClas2 = clas2.valuehomologation;
						descripcionClas2 = clas2.descripcion;
					}

					if (valuehomologationClas2 != null)
                    {
                        var clasificacion2ProductoCI = dbCI.TblIvClasifProducto2.FirstOrDefault(fod => fod.CCiClasificacion2 == valuehomologationClas2 && fod.CCiClasificacion1 == valuehomologationClas1);
                        if (clasificacion2ProductoCI == null)
                        {
                            clasificacion2ProductoCI = new TblIvClasifProducto2();
                            clasificacion2ProductoCI.CCiClasificacion1 = valuehomologationClas1;
                            clasificacion2ProductoCI.CCiClasificacion2 = valuehomologationClas2;
                            clasificacion2ProductoCI.CDsClasificacion2 = descripcionClas2;
                            clasificacion2ProductoCI.CCeClasificacion = "A";
                            dbCI.TblIvClasifProducto2.Add(clasificacion2ProductoCI);
                            dbCI.Entry(clasificacion2ProductoCI).State = EntityState.Added;
                        }
                    }

					productoCI.CCiClasificacion2 = valuehomologationClas2;

					var clas3 = (from di in itemHomologation
								 where di.code.Equals("CLAS3") && di.homologationType.Equals("PR") && di.value.Equals(item.ItemTypeCategory.code.ToString())
								 select new { valuehomologation = di.valuehomologation, descripcion = di.descriptionhomologation }).FirstOrDefault();

					string valuehomologationClas3 = null;
					string descripcionClas3 = null;

					if (clas3 == null)
					{
						//	throw new Exception("El código '" + item.InventoryLine.code + "'. No existe en la tabla de Homologación para la clasificación 3.");
						answerMigration.message = "El código '" + item.ItemTypeCategory.code.ToString() + "'. No existe en la tabla de Homologación para la clasificación 3.";
						return answerMigration;
					}
					else
					{
						valuehomologationClas3 = clas3.valuehomologation;
						descripcionClas3 = clas3.descripcion;
					}


					if (valuehomologationClas3 != null)
                    {
                        var clasificacion3ProductoCI = dbCI.TblIvClasifProducto3.FirstOrDefault(fod => fod.CCiClasificacion3 == valuehomologationClas3 && 
                                                                                fod.CCiClasificacion2 == valuehomologationClas2
																				&& fod.CCiClasificacion1 == valuehomologationClas1);
                        if (clasificacion3ProductoCI == null)
                        {
                            clasificacion3ProductoCI = new TblIvClasifProducto3();
                            clasificacion3ProductoCI.CCiClasificacion1 = valuehomologationClas1;
                            clasificacion3ProductoCI.CCiClasificacion2 = valuehomologationClas2;
                            clasificacion3ProductoCI.CCiClasificacion3 = valuehomologationClas3;
                            clasificacion3ProductoCI.CDsClasificacion3 = descripcionClas3;
                            clasificacion3ProductoCI.CCeClasificacion = "A";
                            dbCI.TblIvClasifProducto3.Add(clasificacion3ProductoCI);
                            dbCI.Entry(clasificacion3ProductoCI).State = EntityState.Added;
                        }
                    }
					productoCI.CCiClasificacion3 = valuehomologationClas3;

					//if (itemSubGroup != null)
					//{
					//    var clasificacion2ProductoCI = dbCI.TblIvClasifProducto2.FirstOrDefault(fod => fod.CCiClasificacion2 == itemSubGroup.code && fod.CCiClasificacion1 == itemGroup.code);
					//    if (clasificacion2ProductoCI == null)
					//    {
					//        clasificacion2ProductoCI = new TblIvClasifProducto2();
					//        clasificacion2ProductoCI.CCiClasificacion1 = itemGroup.code;
					//        clasificacion2ProductoCI.CCiClasificacion2 = itemSubGroup.code;
					//        clasificacion2ProductoCI.CDsClasificacion2 = itemSubGroup.name;
					//        clasificacion2ProductoCI.CCeClasificacion = "A";
					//        dbCI.TblIvClasifProducto2.Add(clasificacion2ProductoCI);
					//        dbCI.Entry(clasificacion2ProductoCI).State = EntityState.Added;
					//    }
					//}


					ItemGroupCategory itgcTemp = item.ItemGeneral.ItemGroupCategory;

                    TblGeCategoriaProducto categoriaProducto = null;

                    if (itgcTemp != null)
                    {
                        categoriaProducto = dbCI.TblGeCategoriaProducto.FirstOrDefault(fod => fod.CCiCategoriaProducto == (itgcTemp.code.Substring(0, 2) ?? null));
                    }
                    
                    int? idSequential = dbCI.TblGeCategoriaProducto.AsQueryable().Max(m => (int?)m.NIdGeCategoriaProducto) ?? 0;
                    if (itgcTemp != null)
                    {
                        
                        
                        if (categoriaProducto == null)
                        {
                            categoriaProducto = new TblGeCategoriaProducto();
                            categoriaProducto.NIdGeCategoriaProducto = (short)(idSequential + 1);
                            categoriaProducto.CCiCategoriaProducto = item.ItemGeneral.ItemGroupCategory?.code.Substring(0, 2);
                            categoriaProducto.CNoCategoriaProducto = item.ItemGeneral.ItemGroupCategory?.name;
                            categoriaProducto.CCeCategoriaProducto = "A";
                            categoriaProducto.CSNAfectaRatio = "S";
                            categoriaProducto.CSNIngresoManualFactura = "N";
                            categoriaProducto.DFxIngreso = item.dateCreate;
                            categoriaProducto.CCiUsuarioIngreso = db.User.FirstOrDefault(fod => fod.id == item.id_userUpdate).username ?? string.Empty;
                            categoriaProducto.CDsEstacionIngreso = "PC-PRODUCCION";
                            categoriaProducto.DFxModifica = null;
                            categoriaProducto.CCiUsuarioModifica = null;
                            categoriaProducto.CDsEstacionModifica = null;

                            dbCI.TblGeCategoriaProducto.Add(categoriaProducto);
                            dbCI.Entry(categoriaProducto).State = EntityState.Added;
                        }
                    }
                    

                    productoCI.CNoProducto = item.name;
                    productoCI.CNoIngles = string.Empty;
                    var maxLengthCCiCorto = (item.masterCode.Length > 10) ? 10 : (item.masterCode.Length);
                    productoCI.CCiCorto = item.masterCode.Substring(0, maxLengthCCiCorto) ?? string.Empty;
                    productoCI.CCiAlterno = string.Empty;
                    productoCI.CCiCodigoBarra = item.barCode ?? string.Empty;
                    var maxLength = (item.auxCode.Length > 20) ? 20 : (item.auxCode.Length);
                    productoCI.CCiAnterior = item.auxCode.Substring(0, maxLength) ?? string.Empty;
					productoCI.CCiPartida = string.Empty;
                    productoCI.CSNSubItem = string.Empty;
                    productoCI.CCiProductoPadre = string.Empty;
                    productoCI.CNoPropiedad1 = itemGeneral?.manufacturer ?? string.Empty;
                    productoCI.CNoPropiedad2 = itemGeneral?.ItemTrademark?.name ?? string.Empty;
                    productoCI.CNoPropiedad3 = itemGeneral?.ItemTrademarkModel?.name ?? string.Empty;
                    productoCI.CNoPropiedad4 = string.Empty;
                    productoCI.CNoPropiedad5 = string.Empty;
                    productoCI.CNoPropiedad6 = string.Empty;

                    
                    productoCI.CCiTipoInventario = item.InventoryLine.id.ToString();

					var tipoProducto = (from di in itemHomologation
								 where di.code.Equals("TPROD") && di.homologationType.Equals("PR") && di.value.Equals(item.InventoryLine.code.ToString())
								 select new { valuehomologation = di.valuehomologation, descripcion = di.descriptionhomologation }).FirstOrDefault();

					string valuehomologationTproduc = null;
					string descripcionTproduc = null;

					if (tipoProducto == null)
					{
						//	throw new Exception("El código '" + item.InventoryLine.code + "'. No existe en la tabla de Homologación para la clasificación 3.");
						answerMigration.message = "El código '" + item.InventoryLine.code.ToString() + "'. No existe en la tabla de Homologación para el Tipo de Producto.";
						return answerMigration;
					}
					else
					{
						valuehomologationTproduc = tipoProducto.valuehomologation;
						descripcionTproduc = tipoProducto.descripcion;
					}

					{
						TblIvTipoProducto tivpTemporal = dbCI.TblIvTipoProducto.FirstOrDefault(fod => fod.CCiTipoProducto == valuehomologationTproduc);
                        if (tivpTemporal == null)
                        {
                            tivpTemporal = new TblIvTipoProducto();
                            tivpTemporal.CCiTipoProducto = valuehomologationTproduc;
                            tivpTemporal.CDsTipoProducto = descripcionTproduc;
                            tivpTemporal.CCeTipoProducto = item.ItemType.isActive ? "S":"N";

                            dbCI.TblIvTipoProducto.Add(tivpTemporal);
                            dbCI.Entry(tivpTemporal).State = EntityState.Added;
                        }
                    }
                    productoCI.CCiTipoProducto = valuehomologationTproduc;

					var tipoUnidad = (from di in itemHomologation
										where di.code.Equals("TUNID") && di.homologationType.Equals("PR") && di.value.Equals(item.MetricType.code.ToString())
										select new { valuehomologation = di.valuehomologation, descripcion = di.descriptionhomologation }).FirstOrDefault();

					string valuehomologationTunidad = null;
					string descripcionTunidad = null;

					if (tipoUnidad == null)
					{
						//	throw new Exception("El código '" + item.InventoryLine.code + "'. No existe en la tabla de Homologación para la clasificación 3.");
						answerMigration.message = "El código '" + item.MetricType.code.ToString() + "'. No existe en la tabla de Homologación para el Tipo de Unidad.";
						return answerMigration;
					}
					else
					{
						valuehomologationTunidad = tipoUnidad.valuehomologation;
						descripcionTunidad = tipoUnidad.descripcion;
					}
					{
                        tblcitipounidadmedida tctumTemportal = dbCI.tblcitipounidadmedida.FirstOrDefault(fod => fod.CCiTipoUnidad == valuehomologationTunidad);
                        if (tctumTemportal == null)
                        {
                            tctumTemportal = new tblcitipounidadmedida();
                            tctumTemportal.CCiTipoUnidad = valuehomologationTunidad;
                            tctumTemportal.CDsTipoUnidad = descripcionTunidad;
                            tctumTemportal.CCeTipoUnidad = item.MetricType.isActive ? "A" : "I";
                            tctumTemportal.CSNPeso = item.MetricType.code == "PES01" ? "S" : "N";
                            
                            dbCI.tblcitipounidadmedida.Add(tctumTemportal);
                            dbCI.Entry(tctumTemportal).State = EntityState.Added;
                        }
                    }

					var unidadMedida = (from di in itemHomologation
									  where di.code.Equals("UMEDI") && di.homologationType.Equals("PR") && di.value.Equals(item.ItemInventory.MetricUnit.code.ToString())
									  select new { valuehomologation = di.valuehomologation, descripcion = di.descriptionhomologation }).FirstOrDefault();

					string valuehomologationunidad = null;
					string descripcionunidad = null;

					if (tipoUnidad == null)
					{
						//	throw new Exception("El código '" + item.InventoryLine.code + "'. No existe en la tabla de Homologación para la clasificación 3.");
						answerMigration.message = "El código '" + item.ItemInventory.MetricUnit.code.ToString() + "'. No existe en la tabla de Homologación para la Unidad de Medida.";
						return answerMigration;
					}
					else
					{
						valuehomologationunidad = unidadMedida.valuehomologation;
						descripcionunidad = unidadMedida.descripcion;
					}

					if (item.ItemInventory != null)
                    {
                        TblCiUnidadMedida tcumTemporal = dbCI.TblCiUnidadMedida.FirstOrDefault(fod => fod.CCiUnidadMedida == valuehomologationunidad);
                        if (tcumTemporal == null)
                        {
                            tcumTemporal = new TblCiUnidadMedida();
                            tcumTemporal.CCiTipoUnidad = valuehomologationTunidad;
                            tcumTemporal.CCiUnidadMedida = valuehomologationunidad;
                            tcumTemporal.CDsUnidadMedida = descripcionunidad;
                            tcumTemporal.CCeUnidadMedida = item.ItemInventory.MetricUnit.isActive ? "S" : "N";

                            dbCI.TblCiUnidadMedida.Add(tcumTemporal);
                            dbCI.Entry(tcumTemporal).State = EntityState.Added;
                        }
                    }

                    {
                        TblCiRelTipoUnidadMedida tcrtumTemporal = dbCI.TblCiRelTipoUnidadMedida.FirstOrDefault(fod => fod.CCiUnidadMedida == valuehomologationunidad && fod.CCiTipoUnidad == valuehomologationTunidad);
                        if (tcrtumTemporal == null)
                        {
                            tcrtumTemporal = new TblCiRelTipoUnidadMedida();
                            tcrtumTemporal.CCiTipoUnidad = valuehomologationTunidad;
                            tcrtumTemporal.CCiUnidadMedida = valuehomologationunidad;

                            dbCI.TblCiRelTipoUnidadMedida.Add(tcrtumTemporal);
                            dbCI.Entry(tcrtumTemporal).State = EntityState.Added;
                        }
                    }
                    
                    productoCI.CCiTipoUnidad = valuehomologationTunidad;
                    productoCI.CCiUnidadStock = valuehomologationunidad != null ? valuehomologationunidad : string.Empty;

                    productoCI.CCiUnidadCompra = item.ItemSaleInformation?.MetricUnit?.code.ToString() ?? valuehomologationunidad;
                    productoCI.CCiUnidadVenta = item.ItemPurchaseInformation?.MetricUnit?.code.ToString() ?? valuehomologationunidad;

                    productoCI.CCiUnidadConsulta = string.Empty;
                    productoCI.CCiUnidadPeso = string.Empty;

                    productoCI.NNuPeso = 0.0M;
                    productoCI.CSNInventariable = (settingInvCons == "SI") ? "S" : "N";
                    productoCI.CSNLotizado = "N";
                    productoCI.CSNGrabaIva = "b";


                    productoCI.CSNValorizaIva = "N";
                    productoCI.CSNCompuesto = "N";
                    productoCI.CSNSustitutos = string.Empty;
					productoCI.NQnAncho = 0.0M;
                    productoCI.NQnLargo = 0.0M;
                    productoCI.NQnProfundidad = 0.0M;
                    productoCI.NQnVolumen = 0.0M;
                      
                    productoCI.CCtOrigen = string.Empty;
					productoCI.DFxRegistro = item.dateCreate;
                    productoCI.CCeProducto = item.isActive ? "A" : "I";
                    productoCI.CCiUsuarioIngreso = db.User.FirstOrDefault(fod => fod.id == item.id_userCreate).username ?? string.Empty;

                    productoCI.CDsEstacionIngreso = "PC-Produc";
                    productoCI.DFiIngreso = item.dateCreate;
                    productoCI.CCiUsuarioModifica = db.User.FirstOrDefault(fod => fod.id == item.id_userUpdate).username ?? string.Empty;
                    productoCI.CDsEstacionModifica = "PC-Produc";
                    productoCI.DFmModifica = item.dateUpdate;
                    //Puede ser de Inventario
                    productoCI.NVtUltimoPrecio = itemInventoryDetail?.unitPrice ?? (item.isPurchased ? item.ItemPurchaseInformation?.purchasePrice : 0) ;
                    productoCI.DFxUltimoPrecio = itemInventoryDetail?.dateUpdate;

                    productoCI.CCiUnidadAncho = string.Empty;
                    productoCI.CCiUnidadLargo = string.Empty;
                    productoCI.CCiUnidadProf = string.Empty;
                    productoCI.CCiUnidadVolumen = string.Empty;
                    productoCI.CSnCompra = item.isPurchased ? "S" : "N";
                    productoCI.CSnVenta = item.isSold ? "S" : "N";
					productoCI.CSNConsumo = (settingInvCons == "SI") ? "S" : "N";
                    productoCI.NnuCajasPallets = 0; 
                    productoCI.CiModoExplosion = string.Empty; 
                    productoCI.CctDestinoFactura = string.Empty; 

                    productoCI.CsnImpCorpei = "N"; 
                    productoCI.CsnImpRenta = "N";

                    productoCI.NvtUltimoCosto = itemInventoryDetail?.unitPrice ?? 0;
                    productoCI.dfxUltimoCosto = itemInventoryDetail?.dateUpdate;
                    productoCI.NnuPesoBruto = 0;
                    productoCI.NnuPalletContenedor = 0;
                    productoCI.NVtDescCredito = 0;
                    productoCI.NVtDescContado = 0;
                    productoCI.NvtPorcUtilidad = 0;
                    productoCI.CsnCarniceria = string.Empty;
					productoCI.CsnUnidadyPeso = string.Empty;
					productoCI.NNuInspCampo = 0;
                    productoCI.CSnCalculoAutomaticoPrecio = "N";
                    productoCI.CCtPrecio = string.Empty;
                    productoCI.NNuPisosPallet = 0;
					productoCI.CSnImportacion = "N";
                    productoCI.CSnPresupuesto = "N";
                    productoCI.CSnDistribuyeFlete = string.Empty;
					productoCI.CSnRecarga = "N";
                    productoCI.CCiOperadora = string.Empty;
                    productoCI.DFxVigenciaDistFlete = null;
                    productoCI.CSnProductoCompraNoInventariable = "N";

                    if (itgcTemp != null)
                    {
                        productoCI.NIdGeCategoriaProducto = categoriaProducto?.NIdGeCategoriaProducto ?? (short)(idSequential + 1);
                    }
                    else
                    {
                        productoCI.NIdGeCategoriaProducto = 0;
                    }
                        
                    productoCI.CCtGrabaIva = "C";
                    productoCI.CCiTipoComprobante = string.Empty;
                    productoCI.CSnImpuestoVerde = "N";

                    productoCI.NVtImpuestoVerde = 0;
                    productoCI.CSnCreditoTributarioVta = "N";
                    productoCI.CSnActivoFijoVta = "N";
                    productoCI.DFxVigenciaValorizaIva = null;
                    productoCI.CSnProductoGenerico = "N";
                    productoCI.NVtPrecioProveedor = 0;

                    if (item.ItemSaleInformation?.salePrice != null)
                    {
                        productoCI.NVtPrecioVta = (decimal)item.ItemSaleInformation?.salePrice;
                    }
                    else
                    {
                        productoCI.NVtPrecioVta = (decimal)0.01;
                    }

                    TblIvTipoPrecioProducto ttppTemporal = dbCI.TblIvTipoPrecioProducto.FirstOrDefault(fod => fod.CCiProducto == item.masterCode.ToString() && fod.CSnDefecto =="S");
                    if (ttppTemporal == null)
                    {
                        ttppTemporal = new TblIvTipoPrecioProducto();
                        ttppTemporal.CCiProducto = item.masterCode.ToString();
                        ttppTemporal.CSnDefecto = "S";
                        ttppTemporal.NNuPrecio = (decimal)0.01;
                        ttppTemporal.NQnPorcDscto = 0;
                        ttppTemporal.NQnPorcPeso = 0;
                        ttppTemporal.CCiTipoPrecioProducto = "18";

                        dbCI.TblIvTipoPrecioProducto.Add(ttppTemporal);
                        dbCI.Entry(ttppTemporal).State = EntityState.Added;
                    }

                    productoCI.CSnModificaPrecioProveedor = "N";
                    productoCI.NVtDescuentoRef = 0;
                    productoCI.NVtPrecioEstandarProduccion = 0;
                    productoCI.CNoProductoRef = string.Empty;
                    productoCI.NNuPorcArancel = 0;
                    productoCI.NNuPorcICE = 0;
                    productoCI.NVtValorFDI = 0;
                    productoCI.CCtGenero = string.Empty;
					productoCI.NNuPosicionTeclado = 0;
                    productoCI.CCiTipoFact = string.Empty;
                    productoCI.CSNPagaComision = string.Empty;
					productoCI.CSNGeneraControlPaquete = string.Empty;
					productoCI.CCtControlSaldo = string.Empty;
                    productoCI.DFxVencimiento = null;
                    productoCI.NNuDiaVigenciaPostFactura = 0;
                    productoCI.CSnCortesia = "N";
                    productoCI.CSnCtrlCaducidad = string.Empty;

					productoCI.NNuDiasenCaducar = 0;
                    productoCI.NNuDiaAvisoCaducidad = 0;
                    productoCI.CCiIntervalo = string.Empty;
                    productoCI.CSnNoExigeCostoenCierreInv = string.Empty;
                    productoCI.NnuPiezas = 0;
                    productoCI.NNuPorcImpCombustible = 0;
                    productoCI.CCtPorcImpCombustible = string.Empty;
                    productoCI.CSnPorcImpCombustible = "N";
                    productoCI.CCtClaseRetImpComb = string.Empty;
                    productoCI.CCiGrupoRetImpComb = string.Empty;
                    productoCI.CCtTipoRetImpComb = "";
                    productoCI.NNuPorcRetImpComb = 0;
                    productoCI.CSnDescontinuado = string.Empty;
					productoCI.NQnFactorCostoImprt = 0;
                    productoCI.NNuPorcFDI = 0;
                    productoCI.NQnAltura = 0;
                    productoCI.CCiUnidadAltura = string.Empty;
                    productoCI.NQnDiametroExterior = 0;

                    //CCiUnidadDiametroExt
                    productoCI.CCiUnidadDiametroExt = string.Empty;
                    productoCI.NQnDiametroInterior = 0;
                    productoCI.CCiUnidadDiametroInt = string.Empty;
                    productoCI.NQnRosca = string.Empty;
                    productoCI.CCiUnidadRosca = string.Empty;
                    productoCI.NVtFactorImportacion = 0;
                    productoCI.CSnProdSerie = "N";
                    productoCI.CSnPesoInventario = "N";
                    productoCI.CSnGrabaSubsidio = "N";
					productoCI.CCiClasificacion4 = string.Empty;
					productoCI.CCiClasificacion5 = string.Empty;
					productoCI.CCiClasificacion6 = string.Empty;
					productoCI.CCiClasificacion7 = string.Empty;
					productoCI.NnuPesoGlaseo = 0;

                    try
                    {
                        if (newValue)
                        {
                            dbCI.TblIvProducto.Add(productoCI);
                            dbCI.Entry(productoCI).State = EntityState.Added;
                        }
                        else
                        {
                            dbCI.Entry(productoCI).State = EntityState.Modified;
                        }

                        dbCI.SaveChanges();
                        trans.Commit();
                        answerMigration.message = messageCommon + ". Migrado satisfactoriamente.";
                    }
                    catch(Exception ex) {
                        answerMigration.message = messageCommon + ". Error al guardar" + ex.InnerException.Message;
                        answerMigration.resultado = false;
                    }
					
                    
                }
                catch (Exception e)
                {

					System.Diagnostics.Debug.WriteLine($"ERROR en respuesta ADD: {e}");

					trans.Rollback();

					answerMigration.message = messageCommon + ". Error no esperado: Test" + e.Message;
                    answerMigration.resultado = false;

					//answerMigration.message = messageCommon + ". Error no esperado: " + e.Message + e.InnerException != null? "Execpcion Interna : " + e.InnerException.Message :"" ;

					UpdateMigrationItem(migrationItem, answerMigration.message);
					return answerMigration;


				}
            }

            answerMigration.message = messageCommon + ". Migrado satisfactoriamente.";
            DeleteMigrationInsertHistoryItem(migrationItem, answerMigration.message);
            return answerMigration;
        }

        public List<AnswerMigration> AddItems()
        {
            ConnectionStringSettings connString = null;
            try
            {
                Configuration rootWebConfig =
                    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/web");
                
                if (0 < rootWebConfig.ConnectionStrings.ConnectionStrings.Count)
                {
                    connString =
                        rootWebConfig.ConnectionStrings.ConnectionStrings["DBContextCI"];
                }
            }
            catch(Exception ex)
            {
                return new List<AnswerMigration>
                {
                new AnswerMigration
                {
                    resultado = false,
                    message = "AddItems" + ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message,
					// Puedes agregar más propiedades según sea necesario para proporcionar información detallada
				} 
                };
            }
            int i = 0;
            string sTemporal = string.Empty;
            string sTemporal2 = string.Empty;

            try
            {
                if (connString != null)
                {
                    sTemporal = connString.ConnectionString;
                    i = sTemporal.IndexOf("connection string");
                    if (i > 0)
                    {
                        sTemporal2 = sTemporal.Substring((i +19), (sTemporal.Length - (i+20)));
                    }

                }
            }
            catch (Exception ex)
            {
                return new List<AnswerMigration>
                {
                new AnswerMigration
                {
                    resultado = false,
                    message = "AddItems 2" + ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message,
					// Puedes agregar más propiedades según sea necesario para proporcionar información detallada
				}
                };
            }
            
            if(connString != null)
            {
                if (!string.IsNullOrEmpty(sTemporal2))
                {
                    HabilitarDeshabilitarTrigger(sTemporal2, "D");
                }
                
            }

            List<MigrationItem> listItemUpdate = new List<MigrationItem>();
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    listItemUpdate = db.MigrationItem.ToList();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new List<AnswerMigration>
                {
                new AnswerMigration
                {
                    resultado = false,
                    message = "AddItems 3 " + ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message,
					// Puedes agregar más propiedades según sea necesario para proporcionar información detallada
				}
                };
                }
            }
                
            

            var listAnswerMigration = new List<AnswerMigration>();
            

            foreach (var item in listItemUpdate)
            {
                listAnswerMigration.Add(AddItem(item));
            }
            if (connString != null)
            {
                if (!string.IsNullOrEmpty(sTemporal2))
                {
                    HabilitarDeshabilitarTrigger(sTemporal2, "H");

                }
            }
            return listAnswerMigration;
        }

		public string AddModifyItem(int id)
		{
			string resultado;
			string sMensaje = string.Empty;
			MigrationItem mpTmp = null;
			AnswerMigration amTmp = null;
			using (DbContextTransaction tran = db.Database.BeginTransaction())
			{
				try
				{
					mpTmp = db.MigrationItem.FirstOrDefault(fod => fod.id_item == id);
					tran.Commit();
				}
				catch (Exception ex)
				{
					if (ex.InnerException.Message != null)
					{
						sMensaje = string.Concat("Excepcion: ", ex.Message, "; Excepción Interna: ", ex.InnerException.Message);
					}
					tran.Rollback();

				}
			}
			amTmp = (mpTmp != null ? AddItem(mpTmp) : null);

			resultado = amTmp != null ? (amTmp.resultado == true ? "OK" : amTmp.message) : "NO EXISTE PRODUCTO DISPONIBLE PARA MIGRACIÓN";

			return resultado;
		}

		private static void HabilitarDeshabilitarTrigger(string connectionString, string sTipo)
        {
            string queryString = string.Empty;

            if (sTipo == "H")
            {
                queryString = "ENABLE TRIGGER TIU02_TblIvProducto ON TBLIVPRODUCTO";
            }
            else if(sTipo == "D")
            {
                queryString = "DISABLE TRIGGER TIU02_TblIvProducto ON TBLIVPRODUCTO";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(
                           connectionString))
                {

                    try
                    {
                        SqlCommand command = new SqlCommand(
                        queryString, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}