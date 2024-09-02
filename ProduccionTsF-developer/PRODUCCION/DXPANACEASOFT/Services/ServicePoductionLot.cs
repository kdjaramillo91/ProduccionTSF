using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DevExpress.Utils.Extensions;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Services
{
    public class ServicePoductionLot
    {
        public void UpdateProductionLotLiquidationTotal(ref DBContext db, ref LiquidationCartOnCart liquidationCartOnCart, Company ActiveCompany, User ActiveUser, bool reverse = false)
        {
            //string result = "";
            // WarehouseLocation warehouseLocation = new WarehouseLocation();
            try
            {
                var _liquidationCartOnCart = liquidationCartOnCart;
                var productionLotAux = db.ProductionLot.FirstOrDefault(fod => fod.id == _liquidationCartOnCart.id_ProductionLot);

                var _liquidationCartOnCartDb = db.LiquidationCartOnCart
                                                .Where(fod => fod.id_ProductionLot == _liquidationCartOnCart.id_ProductionLot
                                                && fod.Document.DocumentState.code == "03"
                                                && fod.id != _liquidationCartOnCart.id).ToList();
                if (!reverse)
                    _liquidationCartOnCartDb.Add(_liquidationCartOnCart);

                var _liquidationCartOnCartDbEliminar = db.LiquidationCartOnCart
                                                .Where(fod => fod.id_ProductionLot == _liquidationCartOnCart.id_ProductionLot
                                                && fod.Document.DocumentState.code == "03").ToList();

                if (_liquidationCartOnCartDbEliminar.Count() > 0)
                {
                    foreach (var detalles in _liquidationCartOnCartDbEliminar)
                    {
                        foreach (var item in detalles.LiquidationCartOnCartDetail)
                        {
                            var _productionLotLiquidationTotalAuxi = db.ProductionLotLiquidationTotal.FirstOrDefault(fod => fod.id_productionLot == _liquidationCartOnCart.id_ProductionLot
                                                                    && fod.id_ItemLiquidation == item.id_ItemLiquidation);

                            if (_productionLotLiquidationTotalAuxi != null)
                            {
                                productionLotAux.ProductionLotLiquidationTotal.Remove(_productionLotLiquidationTotalAuxi);
                                UpdateProductionLotLiquidationPackingMaterialDetail(db, productionLotAux, _productionLotLiquidationTotalAuxi, ActiveCompany, ActiveUser);

                                if (_productionLotLiquidationTotalAuxi.ProductionLotLiquidationPackingMaterialDetail.Count() > 0)
                                    db.ProductionLotLiquidationPackingMaterialDetail.RemoveRange(_productionLotLiquidationTotalAuxi.ProductionLotLiquidationPackingMaterialDetail);
                                db.ProductionLotLiquidationTotal.Remove(_productionLotLiquidationTotalAuxi);
                                db.Entry(_productionLotLiquidationTotalAuxi).State = EntityState.Deleted;
                            }

                            db.SaveChanges();
                        }
                    }
                }

                foreach (var detalles in _liquidationCartOnCartDb)
                {
                    foreach (var item in detalles.LiquidationCartOnCartDetail)
                    {
                        var ItemAux = db.Item.FirstOrDefault(fod => fod.id == item.id_ItemLiquidation);
                        //LBS
                        var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? null;
                        var presentation = ItemAux?.Presentation;
                        var id_metricUnitPresentation = ItemAux?.Presentation.id_metricUnit ?? id_metricUnitLbsAux;
                        decimal quantityTotal = QuantityTotalByPresentation(presentation, item.quatityBoxesIL);

                        var productionLotLiquidationTotalAux = db.ProductionLotLiquidationTotal.FirstOrDefault(fod => fod.id_SalesOrder == item.id_SalesOrder && fod.id_SalesOrderDetail == item.id_SalesOrderDetail &&
                                                                                            fod.id_ItemLiquidation == item.id_ItemLiquidation && fod.id_ItemToWarehouse == item.id_ItemToWarehouse &&
                                                                                            fod.id_productionLot == _liquidationCartOnCart.id_ProductionLot);


                        if (productionLotLiquidationTotalAux == null)
                        {
                            productionLotLiquidationTotalAux = new ProductionLotLiquidationTotal();
                            productionLotLiquidationTotalAux.id_productionLot = liquidationCartOnCart.id_ProductionLot;
                            productionLotLiquidationTotalAux.ProductionLot = productionLotAux;
                            productionLotLiquidationTotalAux.id_SalesOrder = item.id_SalesOrder;
                            productionLotLiquidationTotalAux.id_SalesOrderDetail = item.id_SalesOrderDetail;
                            productionLotLiquidationTotalAux.id_ItemLiquidation = item.id_ItemLiquidation;
                            productionLotLiquidationTotalAux.Item = db.Item.FirstOrDefault(fod => fod.id == item.id_ItemLiquidation);
                            productionLotLiquidationTotalAux.quatityBoxesIL = item.quatityBoxesIL;
                            productionLotLiquidationTotalAux.quantityKgsIL = item.quantityKgsIL;
                            productionLotLiquidationTotalAux.quantityPoundsIL = item.quantityPoundsIL;
                            productionLotLiquidationTotalAux.id_ItemToWarehouse = item.id_ItemToWarehouse;
                            // productionLotLiquidationTotalAux.Item1 = db.Item.FirstOrDefault(fod => fod.id == item.id_ItemToWarehouse);
                            productionLotLiquidationTotalAux.quantityKgsITW = item.quantityKgsITW;
                            productionLotLiquidationTotalAux.quantityPoundsITW = item.quantityPoundsITW;

                            productionLotLiquidationTotalAux.quantityTotal = quantityTotal;
                            productionLotLiquidationTotalAux.id_metricUnitPresentation = id_metricUnitPresentation;

                            db.ProductionLotLiquidationTotal.Add(productionLotLiquidationTotalAux);
                        }
                        else
                        {
                            productionLotLiquidationTotalAux.quatityBoxesIL += (item.quatityBoxesIL);
                            productionLotLiquidationTotalAux.quantityKgsIL += (item.quantityKgsIL);
                            productionLotLiquidationTotalAux.quantityPoundsIL += (item.quantityPoundsIL);
                            productionLotLiquidationTotalAux.quantityKgsITW += (item.quantityKgsITW);
                            productionLotLiquidationTotalAux.quantityPoundsITW += (item.quantityPoundsITW);
                            productionLotLiquidationTotalAux.quantityTotal += (quantityTotal);

                            if (productionLotLiquidationTotalAux.quatityBoxesIL <= 0)
                            {
                                productionLotAux.ProductionLotLiquidationTotal.Remove(productionLotLiquidationTotalAux);
                                UpdateProductionLotLiquidationPackingMaterialDetail(db, productionLotAux, productionLotLiquidationTotalAux, ActiveCompany, ActiveUser);

                                if (productionLotLiquidationTotalAux.ProductionLotLiquidationPackingMaterialDetail.Count() > 0)
                                    db.ProductionLotLiquidationPackingMaterialDetail.RemoveRange(productionLotLiquidationTotalAux.ProductionLotLiquidationPackingMaterialDetail);
                                db.ProductionLotLiquidationTotal.Remove(productionLotLiquidationTotalAux);
                                db.Entry(productionLotLiquidationTotalAux).State = EntityState.Deleted;
                            }
                            else
                            {
                                db.ProductionLotLiquidationTotal.Attach(productionLotLiquidationTotalAux);
                                db.Entry(productionLotLiquidationTotalAux).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                    }

                }

                if (_liquidationCartOnCartDb.Count() == 0)
                {
                    foreach (var item in _liquidationCartOnCart.LiquidationCartOnCartDetail)
                    {
                        var _productionLotLiquidationTotalAuxi = db.ProductionLotLiquidationTotal.FirstOrDefault(fod => fod.id_productionLot == _liquidationCartOnCart.id_ProductionLot
                                                                && fod.id_ItemLiquidation == item.id_ItemLiquidation);

                        if(_productionLotLiquidationTotalAuxi != null)
                        {
                            productionLotAux.ProductionLotLiquidationTotal.Remove(_productionLotLiquidationTotalAuxi);
                            UpdateProductionLotLiquidationPackingMaterialDetail(db, productionLotAux, _productionLotLiquidationTotalAuxi, ActiveCompany, ActiveUser);

                            if (_productionLotLiquidationTotalAuxi.ProductionLotLiquidationPackingMaterialDetail.Count() > 0)
                                db.ProductionLotLiquidationPackingMaterialDetail.RemoveRange(_productionLotLiquidationTotalAuxi.ProductionLotLiquidationPackingMaterialDetail);
                            db.ProductionLotLiquidationTotal.Remove(_productionLotLiquidationTotalAuxi);
                            db.Entry(_productionLotLiquidationTotalAuxi).State = EntityState.Deleted;
                        }
                        

                        db.SaveChanges();
                    } 
                }
             

                foreach (var productionLotLiquidationTotal in productionLotAux.ProductionLotLiquidationTotal)
                {
                    UpdateProductionLotLiquidationPackingMaterialDetail(db, productionLotAux, productionLotLiquidationTotal, ActiveCompany, ActiveUser);
                }
            }
            catch (Exception e)
            {
                //result = e.Message;
                throw e;
            }

            //return warehouseLocation;
        }

        private static void UpdateProductionLotLiquidationTotal(ProductionLot productionLot)
        {
            productionLot.totalQuantityLiquidation = 0.0M;
            productionLot.wholeSubtotal = 0.0M;
            productionLot.subtotalTail = 0.0M;

            foreach (var productionLotLiquidationTotal in productionLot.ProductionLotLiquidationTotal)
            {
                productionLot.totalQuantityLiquidation += productionLotLiquidationTotal.quantityPoundsIL;
                var codeAux = productionLotLiquidationTotal.Item?.ItemType?.ProcessType?.code ?? "";
                if (codeAux == "ENT")
                {
                    productionLot.wholeSubtotal += productionLotLiquidationTotal.quantityPoundsIL;
                }
                else
                {
                    productionLot.subtotalTail += productionLotLiquidationTotal.quantityPoundsIL;
                }

            }
        }

        private static void UpdateProductionLotLiquidationPackingMaterialDetail(DBContext db, ProductionLot productionLot, ProductionLotLiquidationTotal productionLotLiquidationTotal, Company activeCompany, User activeUser)
        {
            for (int i = productionLotLiquidationTotal.ProductionLotLiquidationPackingMaterialDetail.Count - 1; i >= 0; i--)
            {


                var detail = productionLotLiquidationTotal.ProductionLotLiquidationPackingMaterialDetail.ElementAt(i);

                detail.ProductionLotPackingMaterial.quantityRequiredForProductionLot -= detail.quantity;
                detail.ProductionLotPackingMaterial.manual = detail.ProductionLotPackingMaterial.quantityRequiredForProductionLot == 0;
                detail.ProductionLotPackingMaterial.quantity -= detail.quantity;

                productionLotLiquidationTotal.ProductionLotLiquidationPackingMaterialDetail.Remove(detail);
                try
                {
                    db.ProductionLotLiquidationPackingMaterialDetail.Attach(detail);
                    db.Entry(detail).State = EntityState.Deleted;
                }
                catch (Exception)
                {
                    //ViewData["EditError"] = e.Message;
                    continue;
                }

            }

            //if (!productionLotLiquidation.isActive) return;
            if (!productionLot.ProductionLotLiquidationTotal.Any(a => a.id == productionLotLiquidationTotal.id)) return;

            if (productionLotLiquidationTotal.Item == null)
            {
                productionLotLiquidationTotal.Item = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidationTotal.id_ItemLiquidation);
            }
            var itemIngredientMDE = productionLotLiquidationTotal.Item.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("MI") && w.Item1.ItemType.code.Equals("INS") && w.Item1.ItemTypeCategory.code.Equals("MDE"));//"MI": Linea de Inventario Materiales e Insumos, "INS": Tipo de Producto Insumos y  "MDE": Categoría de Tipo de Producto Meteriales de Empaque
            if (itemIngredientMDE.Count() == 0) return;
            //UN
            var id_metricUnitUnAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Un")?.id ?? null;
            var id_metricUnitLiquidation = id_metricUnitUnAux;//productionLotLiquidationTotal.id_metricUnit;//ItemPurchaseInformation?.id_metricUnitPurchase;
            var id_metricUnitItemHeadIngredient = productionLotLiquidationTotal.Item.ItemHeadIngredient?.id_metricUnit;
            var factorConversionLiquidationFormulation = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == activeCompany.id &&
                                                                                             muc.id_metricOrigin == id_metricUnitLiquidation &&
                                                                                             muc.id_metricDestiny == id_metricUnitItemHeadIngredient);
            if (id_metricUnitLiquidation != null && id_metricUnitLiquidation == id_metricUnitItemHeadIngredient)
            {
                factorConversionLiquidationFormulation = new MetricUnitConversion() { factor = 1 };
            }
            if (factorConversionLiquidationFormulation == null)
            {
                var metricUnitLiquidation = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitLiquidation);
                throw new Exception("Falta el Factor de Conversión entre : " + (metricUnitLiquidation?.code ?? "(UM No Existe)") + ", del Ítem: " + productionLotLiquidationTotal.Item.name + " y " + (productionLotLiquidationTotal.Item.ItemHeadIngredient?.MetricUnit?.code ?? "(UM No Existe)") + " configurado en la cabecera de la formulación del este Ítem. Necesario para cargar los Materiales de Empaque Configúrelo, e intente de nuevo");
            }

            foreach (var iimdd in itemIngredientMDE)
            {
                var quantityMetricUnitItemHeadIngredient = productionLotLiquidationTotal.quatityBoxesIL * factorConversionLiquidationFormulation.factor;
                var amountItemHeadIngredient = (productionLotLiquidationTotal.Item.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + productionLotLiquidationTotal.Item.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
                }
                var quantityItemIngredientMDE = (quantityMetricUnitItemHeadIngredient * (iimdd.amount ?? 0)) / amountItemHeadIngredient;
                if (quantityItemIngredientMDE == 0) continue;

                //if(iimdd.Item1.MetricType.DataType.code.Equals("ENTE01"))//"ENTE01" Codigo de Entero de Tipo de Datos en la unidad de medida
                //{
                var truncateQuantityItemIngredientMDE = decimal.Truncate(quantityItemIngredientMDE);
                if ((quantityItemIngredientMDE - truncateQuantityItemIngredientMDE) > 0)
                {
                    quantityItemIngredientMDE = truncateQuantityItemIngredientMDE + 1;
                };
                //}
                var id_metricUnitFormulation = iimdd.id_metricUnit;
                var id_metricUnitInventory = iimdd.Item1.ItemInventory?.id_metricUnitInventory;
                var factorConversionFormulationInventory = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == activeCompany.id &&
                                                                                                  muc.id_metricOrigin == id_metricUnitFormulation &&
                                                                                                  muc.id_metricDestiny == id_metricUnitInventory);
                if (id_metricUnitFormulation != null && id_metricUnitFormulation == id_metricUnitInventory)
                {
                    factorConversionFormulationInventory = new MetricUnitConversion() { factor = 1 };
                }
                if (factorConversionFormulationInventory == null)
                {
                    throw new Exception("Falta el Factor de Conversión entre : " + iimdd.MetricUnit?.code ?? "(UM No Existe)" + ", del Ítem: " + iimdd.Item1.name + " y " + iimdd.Item1.ItemInventory?.MetricUnit.code ?? "(UM No Existe)" + " configurado en el detalle de la formulación del Ítem: " + productionLotLiquidationTotal.Item.name + ". Necesario para cargar los Materiales de Empaque Configúrelo, e intente de nuevo");
                }

                var quantityUMInventory = quantityItemIngredientMDE * factorConversionFormulationInventory.factor;

                var truncateQuantityUMInventory = decimal.Truncate(quantityUMInventory);
                if ((quantityUMInventory - truncateQuantityUMInventory) > 0)
                {
                    quantityUMInventory = truncateQuantityUMInventory + 1;
                };

                ProductionLotPackingMaterial productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.Where(w => /*!w.manual &&*/ w.isActive).FirstOrDefault(fod => fod.id_item == iimdd.id_ingredientItem);
                if (productionLotPackingMaterial != null)
                {
                    productionLotPackingMaterial.quantityRequiredForProductionLot += quantityUMInventory;
                    productionLotPackingMaterial.quantity += quantityUMInventory;
                    productionLotPackingMaterial.manual = false;
                    productionLotPackingMaterial.id_userUpdate = activeUser.id;
                    productionLotPackingMaterial.dateUpdate = DateTime.Now;
                }
                else
                {
                    productionLotPackingMaterial = new ProductionLotPackingMaterial
                    {
                        id = productionLot.ProductionLotPackingMaterial.Count() > 0 ? productionLot.ProductionLotPackingMaterial.Max(pld => pld.id) + 1 : 1,
                        id_item = iimdd.id_ingredientItem,
                        Item = db.Item.FirstOrDefault(i => i.id == iimdd.id_ingredientItem),
                        quantityRequiredForProductionLot = quantityUMInventory,
                        quantity = quantityUMInventory,
                        manual = false,
                        isActive = true,
                        id_userCreate = activeUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = activeUser.id,
                        dateUpdate = DateTime.Now,
                        ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>()
                    };
                    productionLot.ProductionLotPackingMaterial.Add(productionLotPackingMaterial);
                }

                var productionLotLiquidationPackingMaterialDetail = new ProductionLotLiquidationPackingMaterialDetail
                {
                    //ProductionLotLiquidation = productionLotLiquidation,
                    //id_productionLotLiquidation = productionLotLiquidation.id,
                    ProductionLotLiquidationTotal = productionLotLiquidationTotal,
                    id_productionLotLiquidationTotal = productionLotLiquidationTotal.id,
                    ProductionLotPackingMaterial = productionLotPackingMaterial,
                    id_productionLotPackingMaterial = productionLotPackingMaterial.id,
                    quantity = quantityUMInventory
                };
                productionLotLiquidationTotal.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
                productionLotPackingMaterial.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
            }

        }

        private static decimal QuantityTotalByPresentation(Presentation presentation, decimal quantity)
        {

            if (presentation == null)
            {
                return decimal.Round((quantity), 2);
            }
            else
            {
                return decimal.Round((presentation.minimum * quantity), 2);
            }

        }

        public static ProductionLot GetOriginProductionLotForProductProcess
            (DBContext db,
                Company ActiveCompany,
                User ActiveUser,
                int idLot,
                string numberLot,
                int idProductionProcess,
                DateTime receptionDate,
                DateTime expirationDate,
                decimal cantidadIngresaMateriales,
                int id_ItemSequence,
                int idEmployee
                )
        {

            ProductionLot productionLot = db.ProductionLot
                                                        .FirstOrDefault(r => r.id == idLot);
            if (productionLot != null) return productionLot;
            productionLot = new ProductionLot();

            var stateInProdutionProcess = db.ProductionLotState
                                                    .FirstOrDefault(r => r.code == "100");
            if (stateInProdutionProcess == null) throw new Exception("Debe definir Estado de Lote EN PROCESO INTERNO, con código 100");

            var id_productionUnit = db.ProductionUnit.FirstOrDefault(r => r.code == "PR1");

            productionLot.id = idLot;
            productionLot.number = $"I{numberLot}-{id_ItemSequence}";
            productionLot.id_ProductionLotState = stateInProdutionProcess.id;
            productionLot.id_productionUnit = id_productionUnit.id;
            productionLot.id_productionProcess = idProductionProcess;
            productionLot.receptionDate = receptionDate;
            productionLot.id_personRequesting = idEmployee;
            productionLot.id_personReceiving = idEmployee;
            productionLot.expirationDate = expirationDate;
            productionLot.totalQuantityOrdered = cantidadIngresaMateriales;
            productionLot.totalQuantityRemitted = cantidadIngresaMateriales;
            productionLot.totalQuantityRecived = cantidadIngresaMateriales;

            productionLot.totalQuantityLiquidation = 0;
            productionLot.totalQuantityTrash = 0;
            productionLot.totalQuantityLiquidationAdjust = 0;
            productionLot.withPrice = false;
            productionLot.pricePerLbs = 0;

            productionLot.id_company = ActiveCompany.id;
            productionLot.id_userCreate = ActiveUser.id;
            productionLot.dateCreate = DateTime.Now;
            productionLot.id_userUpdate = ActiveUser.id;
            productionLot.dateUpdate = DateTime.Now;

            productionLot.wholeSubtotal = 0;
            productionLot.subtotalTail = 0;
            productionLot.wholeGarbagePounds = 0;
            productionLot.poundsGarbageTail = 0;
            productionLot.wholeLeftover = 0;
            productionLot.totalAdjustmentPounds = 0;
            productionLot.totalAdjustmentWholePounds = 0;
            productionLot.totalAdjustmentTailPounds = 0;
            productionLot.wholeSubtotalAdjust = 0;
            productionLot.subtotalTailAdjust = 0;
            productionLot.totalToPay = 0;
            productionLot.wholeTotalToPay = 0;
            productionLot.tailTotalToPay = 0;


            db.ProductionLot.Add(productionLot);
            //db.SaveChanges();

            //using (DbContextTransaction trans = db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        db.ProductionLot.Add(productionLot);
            //        db.SaveChanges();
            //        trans.Commit();
            //    }
            //    catch (Exception)
            //    {
            //        trans.Rollback();
            //        throw new Exception("Ha ocurrido un error al crear Lote de Proceso Interno");
            //    }
            //}


            return productionLot;

        }


    }
}