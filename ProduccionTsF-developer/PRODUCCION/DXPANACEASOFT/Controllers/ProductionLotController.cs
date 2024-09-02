using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Filter;
using System.Configuration;
using DXPANACEASOFT.Services;
using DXPANACEASOFT.Reports.ProductionLot;
using System.Threading;
using Newtonsoft.Json;
using DXPANACEASOFT.DataProviders;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;
using DevExpress.Web;
using System.Collections;
using System.Data.Linq;
using DevExpress.Utils;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using System.Security.Cryptography.X509Certificates;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ProductionLotController : DefaultController
    {
        #region ADVANCED FILTER

        [ValidateInput(false)]
        public ActionResult PopupAdvancedFilter(string codeAdvancedFiltersConfiguration)
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            //List<FilterType> listFilterType = (TempData["listFilterType"] as List<FilterType>);
            var advancedFiltersConfigurations = db.AdvancedFiltersConfiguration.Where(w => w.code == codeAdvancedFiltersConfiguration).ToList();
            TempData["advancedFiltersConfigurations"] = advancedFiltersConfigurations;

            ViewBag.Attributes = advancedFiltersConfigurations.Select(s => new { s.id, s.alias });
            ViewBag.Logicos = DataProviderAdvancedFilter.Logical();

            var model = new AdvancedFilter();//listFilterType ?? new List<FilterType>();

            //TempData["listFilterType"] = TempData["listFilterType"] ?? listFilterType;
            TempData.Keep("advancedFiltersConfigurations");

            return PartialView("FilterBoxTemplates/_PopupAdvancedFilter", model);
        }

        #endregion

        #region FILTER

        [ValidateInput(false)]
        public ActionResult FilterBoxGridViewLeftPartial(string type)
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            List<FilterType> listFilterType = (TempData["listFilterType"] as List<FilterType>);

            var model = listFilterType ?? new List<FilterType>();

            TempData["listFilterType"] = TempData["listFilterType"] ?? listFilterType;
            TempData.Keep("listFilterType");
            TempData.Keep("listFilterTypeWithCondition");

            return PartialView("FilterBoxTemplates/_FilterBox" + type + "GridViewLeft", model.Where(w => w.type == type).ToList());
        }

        [ValidateInput(false)]
        public ActionResult FilterBoxDateGridViewLeftPartial()
        {
            return FilterBoxGridViewLeftPartial("Date");
        }
        [ValidateInput(false)]
        public ActionResult FilterBoxTextGridViewLeftPartial()
        {
            return FilterBoxGridViewLeftPartial("Text");
        }
        [ValidateInput(false)]
        public ActionResult FilterBoxNumberGridViewLeftPartial()
        {
            return FilterBoxGridViewLeftPartial("Number");
        }
        [ValidateInput(false)]
        public ActionResult FilterBoxSelectGridViewLeftPartial()
        {
            return FilterBoxGridViewLeftPartial("Select");
        }
        [ValidateInput(false)]
        public ActionResult FilterBoxCheckGridViewLeftPartial()
        {
            return FilterBoxGridViewLeftPartial("Check");
        }

        [ValidateInput(false)]
        public ActionResult FilterBoxGridViewRightPartial(string type)
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            List<FilterTypeWithCondition> listFilterTypeWithCondition = (TempData["listFilterTypeWithCondition"] as List<FilterTypeWithCondition>);

            var model = listFilterTypeWithCondition ?? new List<FilterTypeWithCondition>();

            TempData["listFilterTypeWithCondition"] = TempData["listFilterTypeWithCondition"] ?? listFilterTypeWithCondition;
            TempData.Keep("listFilterTypeWithCondition");
            TempData.Keep("listFilterType");

            return PartialView("FilterBoxTemplates/_FilterBox" + type + "GridViewRight", model.Where(w => w.filterType.type == type).ToList());
        }

        [ValidateInput(false)]
        public ActionResult FilterBoxDateGridViewRightPartial()
        {
            return FilterBoxGridViewRightPartial("Date");
        }
        [ValidateInput(false)]
        public ActionResult FilterBoxTextGridViewRightPartial()
        {
            return FilterBoxGridViewRightPartial("Text");
        }
        [ValidateInput(false)]
        public ActionResult FilterBoxNumberGridViewRightPartial()
        {
            return FilterBoxGridViewRightPartial("Number");
        }
        [ValidateInput(false)]
        public ActionResult FilterBoxSelectGridViewRightPartial()
        {
            return FilterBoxGridViewRightPartial("Select");
        }
        [ValidateInput(false)]
        public ActionResult FilterBoxCheckGridViewRightPartial()
        {
            return FilterBoxGridViewRightPartial("Check");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FilterBoxGridViewRightPartialUpdate(FilterTypeWithCondition filterTypeWithCondition, string type/*, string valueConditionSelectValueText = null*/)
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            List<FilterTypeWithCondition> listFilterTypeWithCondition = (TempData["listFilterTypeWithCondition"] as List<FilterTypeWithCondition>);

            listFilterTypeWithCondition = listFilterTypeWithCondition ?? new List<FilterTypeWithCondition>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = listFilterTypeWithCondition.FirstOrDefault(it => it.id == filterTypeWithCondition.id);//var modelItem = productionLot.ProductionLotLiquidation.FirstOrDefault(pll => pll.id_item == productionLotLiquidation.id_item);
                    if (modelItem != null)
                    {
                        modelItem.id_logicalOperator = filterTypeWithCondition.id_logicalOperator;
                        modelItem.logicalOperator = (type == "Date" || type == "Number") ? DataProviderLogicalOperator.LogicalOperatorDateNumbersById(filterTypeWithCondition.id_logicalOperator):
                                                    (type == "Text" ? DataProviderLogicalOperator.LogicalOperatorTextsById(filterTypeWithCondition.id_logicalOperator):
                                                    (type == "Select" ? DataProviderLogicalOperator.LogicalOperatorSelectsById(filterTypeWithCondition.id_logicalOperator) : null));
                        //Date
                        modelItem.valueConditionFromDateTime = filterTypeWithCondition.valueConditionFromDateTime;
                        modelItem.valueConditionToDateTime = filterTypeWithCondition.valueConditionToDateTime;
                        //Decimal
                        modelItem.valueConditionFromDecimal = filterTypeWithCondition.valueConditionFromDecimal;
                        modelItem.valueConditionToDecimal = filterTypeWithCondition.valueConditionToDecimal;
                        //String o Select
                        modelItem.valueConditionTextOrSelect = filterTypeWithCondition.valueConditionTextOrSelect;
                        //Bool Check
                        modelItem.valueConditionCheck = filterTypeWithCondition.valueConditionCheck;
                        //Select
                        //modelItem.valueConditionSelectValue = filterTypeWithCondition.valueConditionSelectValue;

                        //modelItem.valueConditionSelectValueText = valueConditionSelectValueText;
                        this.UpdateModel(modelItem);
                        //UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                        //modelItem.Item = db.Item.FirstOrDefault(i => i.id == productionLotLiquidation.id_item);

                        //UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, modelItem);

                        TempData["listFilterTypeWithCondition"] = listFilterTypeWithCondition;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            //TempData["listFilterTypeWithCondition"] = TempData["listFilterTypeWithCondition"] ?? listFilterTypeWithCondition;
            TempData.Keep("listFilterTypeWithCondition");
            TempData.Keep("listFilterType");

            //var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("FilterBoxTemplates/_FilterBox" + type + "GridViewRight", listFilterTypeWithCondition.Where(w => w.filterType.type == type).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FilterBoxDateGridViewRightPartialUpdate(FilterTypeWithCondition filterTypeWithCondition)
        {
            return FilterBoxGridViewRightPartialUpdate(filterTypeWithCondition, "Date");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FilterBoxTextGridViewRightPartialUpdate(FilterTypeWithCondition filterTypeWithCondition)
        {
            return FilterBoxGridViewRightPartialUpdate(filterTypeWithCondition, "Text");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FilterBoxNumberGridViewRightPartialUpdate(FilterTypeWithCondition filterTypeWithCondition)
        {
            return FilterBoxGridViewRightPartialUpdate(filterTypeWithCondition, "Number");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FilterBoxSelectGridViewRightPartialUpdate(FilterTypeWithCondition filterTypeWithCondition/*, string valueConditionSelectValueText = null*/)
        {
            return FilterBoxGridViewRightPartialUpdate(filterTypeWithCondition, "Select"/*, valueConditionSelectValueText*/);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FilterBoxCheckGridViewRightPartialUpdate(FilterTypeWithCondition filterTypeWithCondition)
        {
            return FilterBoxGridViewRightPartialUpdate(filterTypeWithCondition, "Check");
        }

        #endregion

        #region PRODUCTION LOT LIQUIDATION

        [ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLiquidationsDetailPartial(string txtItemsFilter,
            int? id_itemType, int? id_size, int? id_trademark, int? id_presentation,
            string codigoProducto, int? categoriaProducto,int? modeloProducto)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            bool isRequestCarMachine = false;
            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            if(productionLot == null)
            {
            productionLot = (TempData["productionLotProcess"] as ProductionLot);
            }

            if (productionLot == null)
            {
                productionLot = (TempData["productionLotReception"] as ProductionLot);
            }   
            
            if (productionLot == null)
            {
                productionLot = (TempData["productionLotQuality"] as ProductionLot);
            }

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            #region Liquiacion Valorizada - 6115
            string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r => r.code == "LIQNOVAL")?.value ?? "NO");
            if (setting_liquidacionValorizada == "SI")
            {
                isRequestCarMachine = (ProductionProcess.GetOneById(productionLot.id_productionProcess)?.requestCarMachine ?? false);
            }
            ViewBag.isRequestCarMachine = isRequestCarMachine;
            #endregion

            TempData["productionLot"] = TempData["productionLot"] ?? productionLot;
            TempData.Keep("productionLotProcess");
            TempData.Keep("productionLotReception");
            TempData.Keep("productionLotQuality");
            TempData.Keep("productionLot");
            var codeAux = productionLot?.ProcessType?.code ?? "ENT";

            #region << Ajuste Product Process , Enviar Tipo >>
            var productionProcess = db.ProductionProcess.FirstOrDefault(r => r.id == productionLot.id_productionProcess);
            ViewBag.idWarehouseDefault = (productionProcess?.id_warehouse??0);
            
            #endregion

            RefresshDataForEditForm(txtItemsFilter, codeAux, id_itemType, id_size,id_trademark, id_presentation,
                codigoProducto,categoriaProducto, modeloProducto);
            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLiquidationsDetailPartial", model);
        }


        [ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLiquidationsTransferDetailPartial(string txtItemsFilter, 
            bool Act, int? id_wareHouse, int? id_wareHouseLocation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();
            if (Act)
            {
                foreach (var item in model)
                {
                    item.id_wareHouseDetailTransfer = id_wareHouse;
                    item.id_wareHouseLocationDetailTransfer = id_wareHouseLocation;
                }
            }

            TempData["productionLot"] = TempData["productionLot"] ?? productionLot;
            TempData.Keep("productionLot");
            var codeAux = productionLot?.ProcessType?.code ?? "ENT";

            #region << Ajuste Product Process , Enviar Tipo >>
            var productionProcess = db.ProductionProcess.FirstOrDefault(r => r.id == productionLot.id_productionProcess);
            ViewBag.idWarehouseDefault = (productionProcess?.id_warehouse ?? 0);

            #endregion

            RefresshDataForEditForm(txtItemsFilter, codeAux,null,null,null,null,null,null,null);
            this.ViewBag.Id_wareHouse = id_wareHouse;
            this.ViewBag.Id_wareHouseLocation = id_wareHouseLocation;
            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLiquidationsTransferDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLiquidationsDetailAddNew(int? id_salesOrder, 
            ProductionLotLiquidation productionLotLiquidation, int id_item2)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            bool isRequestCarMachine = false;


            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            ProductionLot productionLotProcess = (TempData["productionLotProcess"] as ProductionLot);
            ProductionLot productionLotReception = (TempData["productionLotReception"] as ProductionLot);
            ProductionLot productionLotQuality = (TempData["productionLotQuality"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();
            ViewBag.isRequestCarMachine = false;
            if (ModelState.IsValid)
            {
                try
                {
                    var productionLotLiquidationAux = productionLot.ProductionLotLiquidation.FirstOrDefault(fod => fod.id_item == id_item2 &&
                                                                                fod.id_warehouse == productionLotLiquidation.id_warehouse &&
                                                                                fod.id_warehouseLocation == productionLotLiquidation.id_warehouseLocation);

                    #region Liquiacion Valorizada - 6115
                    string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r => r.code == "LIQNOVAL")?.value ?? "NO");
                    if (setting_liquidacionValorizada == "SI")
                    {
                        isRequestCarMachine = (ProductionProcess.GetOneById(productionLot.id_productionProcess)?.requestCarMachine ?? false);
                        ViewBag.isRequestCarMachine = isRequestCarMachine;
                        if (isRequestCarMachine)
                        {
                            if (productionLotLiquidation.id_productionCart == 0 || productionLotLiquidation.id_productionCart == null)
                            {
                                GetItems(null, null, null, null, null, null, null, null, null);
                                throw new Exception("El campo de carro no puede estar vacio");
                            }
                        }                        
                    }
                    
                    #endregion

                    if (productionLotLiquidationAux != null)
                    {
                        var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_item2);
                        var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_warehouse);
                        var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_warehouseLocation);

                        if (itemAux != null)
                        {
                            GetItems(null, null, null, null, null, null, null, null, null);
                            throw new Exception("No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de esta Liquidación.");
                        }
                    }

                    productionLotLiquidation.id = productionLot.ProductionLotLiquidation.Count() > 0 ? productionLot.ProductionLotLiquidation.Max(pld => pld.id) + 1 : 1;
                    productionLotLiquidation.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod=> fod.id_salesOrder == id_salesOrder && fod.id_item == productionLotLiquidation.id_item);
                    productionLotLiquidation.id_salesOrderDetail = productionLotLiquidation.SalesOrderDetail?.id;

                    productionLotLiquidation.Item = db.Item.FirstOrDefault(i => i.id == id_item2);
                    productionLotLiquidation.id_item = id_item2;
                    productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>();

                    productionLot.ProductionLotLiquidation.Add(productionLotLiquidation);

                    CalculateDistributionPercentage(productionLot);

                    UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                    //UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, productionLotLiquidation);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";


            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");


            TempData.Keep("productionLotProcess");
            TempData.Keep("productionLotReception");
            TempData.Keep("productionLotQuality");
            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLiquidationsDetailPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLiquidationsDetailTransferAddNew(int? id_salesOrder,
        ProductionLotLiquidation productionLotLiquidation, int id_item2)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();

            if (ModelState.IsValid)
            {
                productionLotLiquidation.id = productionLot.ProductionLotLiquidation.Count() > 0 ? productionLot.ProductionLotLiquidation.Max(pld => pld.id) + 1 : 1;
                productionLotLiquidation.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id_salesOrder == id_salesOrder && fod.id_item == productionLotLiquidation.id_item);
                productionLotLiquidation.id_salesOrderDetail = productionLotLiquidation.SalesOrderDetail?.id;

                productionLotLiquidation.Item = db.Item.FirstOrDefault(i => i.id == id_item2);
                productionLotLiquidation.id_item = id_item2;
                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>();

                productionLot.ProductionLotLiquidation.Add(productionLotLiquidation);

                UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

            }

            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLiquidationsTransferDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLiquidationsDetailUpdate(int? id_salesOrder, 
            ProductionLotLiquidation productionLotLiquidation, int id_item2)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            bool isRequestCarMachine = false;

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            ProductionLot productionLotProcess = (TempData["productionLotProcess"] as ProductionLot);
            ProductionLot productionLotReception = (TempData["productionLotReception"] as ProductionLot);
            ProductionLot productionLotQuality = (TempData["productionLotQuality"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);

            if (productionLotProcess != null)
            {
                productionLot.totalQuantityRecived = productionLotProcess.totalQuantityRecived;
                productionLot.totalQuantityLiquidation = productionLotProcess.totalQuantityLiquidation;
                productionLot.totalQuantityTrash = productionLotProcess.totalQuantityTrash;
                productionLot.totalQuantityLoss = productionLotProcess.totalQuantityLoss;

            }

            if (productionLotReception != null)
            {
                productionLot.totalQuantityRecived = productionLotReception.totalQuantityRecived;
                productionLot.totalQuantityLiquidation = productionLotReception.totalQuantityLiquidation;
                productionLot.totalQuantityTrash = productionLotReception.totalQuantityTrash;
                productionLot.totalQuantityLoss = productionLotReception.totalQuantityLoss;
            }

            if (productionLotQuality != null)
            {
                productionLot.totalQuantityRecived = productionLotQuality.totalQuantityRecived;
                productionLot.totalQuantityLiquidation = productionLotQuality.totalQuantityLiquidation;
                productionLot.totalQuantityTrash = productionLotQuality.totalQuantityTrash;
                productionLot.totalQuantityLoss = productionLotQuality.totalQuantityLoss;
            }

            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    #region Liquiacion Valorizada - 6115
                    string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r => r.code == "LIQNOVAL")?.value ?? "NO");
                    if (setting_liquidacionValorizada == "SI")
                    {
                        isRequestCarMachine = (ProductionProcess.GetOneById(productionLot.id_productionProcess)?.requestCarMachine ?? false);
                    }
                    ViewBag.isRequestCarMachine = isRequestCarMachine;
                    #endregion

                    var modelItem = productionLot.ProductionLotLiquidation.FirstOrDefault(it => it.id == productionLotLiquidation.id);//var modelItem = productionLot.ProductionLotLiquidation.FirstOrDefault(pll => pll.id_item == productionLotLiquidation.id_item);
                    if (modelItem != null)
                    {
                        modelItem.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id_salesOrder == id_salesOrder && fod.id_item == productionLotLiquidation.id_item);
                        modelItem.id_salesOrderDetail = productionLotLiquidation.SalesOrderDetail?.id;
                        modelItem.Item = db.Item.FirstOrDefault(i => i.id == id_item2);
                        modelItem.id_item = id_item2;
                        modelItem.quantity = productionLotLiquidation.quantity;
                        modelItem.quantityPoundsLiquidation = productionLotLiquidation.quantityPoundsLiquidation;
                        modelItem.quantityTotal = productionLotLiquidation.quantityTotal;
                        modelItem.id_warehouse = productionLotLiquidation.id_warehouse;
                        modelItem.id_warehouseLocation = productionLotLiquidation.id_warehouseLocation;
                        modelItem.id_metricUnit = productionLotLiquidation.id_metricUnit;
                        modelItem.id_metricUnitPresentation = productionLotLiquidation.id_metricUnitPresentation;
                        this.UpdateModel(modelItem);
                        CalculateDistributionPercentage(productionLot);
                        UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                        

                        //UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, modelItem);

                        TempData["productionLot"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("productionLot");
            TempData.Keep("productionLotProcess");
            TempData.Keep("productionLotReception");
            TempData.Keep("productionLotQuality");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLiquidationsDetailPartial", model);
        }
        public ActionResult ProductionLotEditFormProductionLotLiquidationsDetailTransferUpdate(
            ProductionLotLiquidation productionLotLiquidation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotLiquidation.FirstOrDefault(it => it.id == productionLotLiquidation.id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        //UpdateProductionLotProductionLotLossDetailTotals(productionLot);
                        TempData["productionLot"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLiquidationsTransferDetailPartial", model);

        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLiquidationsDetailDelete(System.Int32 id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLiquidation = productionLot.ProductionLotLiquidation ?? new List<ProductionLotLiquidation>();
            ViewBag.isRequestCarMachine = false;
            bool isRequestCarMachine = false;
            //if (id_item >= 0)
            //{
            try
            {
                #region Liquiacion Valorizada - 6115
                string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r => r.code == "LIQNOVAL")?.value ?? "NO");
                if (setting_liquidacionValorizada == "SI")
                {
                    isRequestCarMachine = (ProductionProcess.GetOneById(productionLot.id_productionProcess)?.requestCarMachine ?? false);
                    ViewBag.isRequestCarMachine = isRequestCarMachine;
                }

                #endregion

                var productionLotLiquidation = productionLot.ProductionLotLiquidation.FirstOrDefault(p => p.id == id);//var productionLotLiquidation = productionLot.ProductionLotLiquidation.FirstOrDefault(p => p.id_item == id_item);
                if (productionLotLiquidation != null)
                        productionLot.ProductionLotLiquidation.Remove(productionLotLiquidation);

                    CalculateDistributionPercentage(productionLot);
                    UpdateProductionLotProductionLotLiquidationsDetailTotals(productionLot);

                //UpdateProductionLotLiquidationPackingMaterialDetail(productionLot, productionLotLiquidation);

                TempData["productionLot"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            //}

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();
            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLiquidationsDetailPartial", model);
        }

        private void CalculateDistributionPercentage(ProductionLot productionLot)
        {

            var aQuantityPoundLiquidationTotal = productionLot.ProductionLotLiquidation.Sum(s => s.quantityPoundsLiquidation);

            foreach (var detail in productionLot.ProductionLotLiquidation)
            {

                detail.distributionPercentage = Math.Round((detail.quantityPoundsLiquidation.Value / aQuantityPoundLiquidationTotal.Value) * 100, 2);
            }


        }

        private void UpdateProductionLotProductionLotLiquidationsDetailTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityLiquidation = 0.0M;
            productionLot.wholeSubtotal = 0.0M;
            productionLot.subtotalTail = 0.0M;

            //----------------------------------------------------
            // Obtener Peso Configurado para Presentar en Formularios
            //----------------------------------------------------
            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);
            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0; // db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;


            //-------------------------------
            // Obtener Peso en Libras 
            //-------------------------------
            var metricUnitFixedPund = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs");
            var id_metricUnitFixedPund = metricUnitFixedPund?.id ?? 0;


            //--------------------------------------------------------------
            // Conversion de Libras de Liquidacion Total a Peso Configurado para Presentar en Formularios
            //--------------------------------------------------------------
            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitFixedPund &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
            var factor = id_metricUnitLbsAux == id_metricUnitFixedPund && id_metricUnitFixedPund != 0 ? 1 : (metricUnitConversion?.factor ?? 0);

            foreach (var productionLotLiquidation in productionLot.ProductionLotLiquidation)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_item);
                //var id_metricUnitAux = productionLotLiquidation.id_metricUnitPresentation;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                //var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                //                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                //                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                //
                //var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);

                //productionLot.totalQuantityLiquidation += productionLotLiquidation.quantityTotal.Value * factor;
                //ojo
                var valueAux = decimal.Round(productionLotLiquidation.quantityPoundsLiquidation.Value * factor, 2);
                // Siempre Unidad de Medida COnfigurado para Presentacion
                //var valueAux = decimal.Round(productionLotLiquidation.quantityPoundsLiquidation.Value, 2);

                productionLot.totalQuantityLiquidation += valueAux;
                //if (productionLotLiquidation.Item.ItemTypeCategory.code == "ENT")
                //{
                var codeAux = productionLotLiquidation.Item?.ItemType?.ProcessType?.code ?? "";
                if (codeAux == "ENT")
                {
                    productionLot.wholeSubtotal += valueAux;
                }
                else
                {
                    productionLot.subtotalTail += valueAux;
                }
            }

//productionLot.totalQuantityLiquidation = decimal.Round(productionLot.totalQuantityLiquidation, 2);

        }

        #endregion

        #region PRODUCTION LOT PACKING MATERIAL

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotPackingMaterialsPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotPackingMaterial = productionLot.ProductionLotPackingMaterial ?? new List<ProductionLotPackingMaterial>();

            var model = productionLot.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0);

            TempData.Keep("productionLot");

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotPackingMaterialsPartialAddNew(ProductionLotPackingMaterial productionLotPackingMaterial)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotPackingMaterial.FirstOrDefault(it => it.id_item == productionLotPackingMaterial.id_item);
                    if (modelItem != null)
                    {
                        modelItem.isActive = true;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);

                    }
                    else
                    {
                        productionLotPackingMaterial.Item = db.Item.FirstOrDefault(i => i.id == productionLotPackingMaterial.id_item);

                        productionLotPackingMaterial.isActive = true;
                        productionLotPackingMaterial.manual = true;
                        productionLotPackingMaterial.id_userCreate = ActiveUser.id;
                        productionLotPackingMaterial.dateCreate = DateTime.Now;
                        productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                        productionLotPackingMaterial.dateUpdate = DateTime.Now;

                        productionLot.ProductionLotPackingMaterial.Add(productionLotPackingMaterial);
                    }



                    TempData["productionLot"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();
            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotPackingMaterialsPartialUpdate(ProductionLotPackingMaterial productionLotPackingMaterial)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotPackingMaterial.FirstOrDefault(it => it.id_item == productionLotPackingMaterial.id_item);
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;

                        this.UpdateModel(modelItem);
                        TempData["productionLot"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotPackingMaterialsPartialDelete(System.Int32 id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (id_item >= 0)
            {
                try
                {
                    var productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.FirstOrDefault(p => p.id_item == id_item);
                    if (productionLotPackingMaterial != null)
                    {
                        if (!productionLotPackingMaterial.manual)
                        {
                            throw (new Exception("Este Ítem de Materiales de Empaque no se puede eliminar debido a que fue cargado desde la formulación de algún producto de la liquidación"));
                        }

                        productionLotPackingMaterial.isActive = false;
                        productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
                        productionLotPackingMaterial.dateUpdate = DateTime.Now;
                    }

                    TempData["productionLot"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotPackingMaterial.Where(d => d.isActive && d.quantity > 0).ToList() ?? new List<ProductionLotPackingMaterial>();
            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotPackingMaterialsDetailPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedProductionLotPackingMaterials(int[] ids)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ids != null)
            {
                try
                {
                    var productionLotPackingMaterial = productionLot.ProductionLotPackingMaterial.Where(i => ids.Contains(i.id_item));

                    foreach (var detail in productionLotPackingMaterial)
                    {
                        if (detail.manual)
                        {
                            detail.isActive = false;
                            detail.id_userUpdate = ActiveUser.id;
                            detail.dateUpdate = DateTime.Now;
                        }
                        
                    }

                    TempData["productionLot"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("productionLot");
        }

        #endregion

        #region PRODUCTION LOT TRASH

        [ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotTrashsDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            if (productionLot == null)
            {
                productionLot = (TempData["productionLotProcess"] as ProductionLot);
            }

            if (productionLot == null)
            {
                productionLot = (TempData["productionLotReception"] as ProductionLot);
            }

            if (productionLot == null)
            {
                productionLot = (TempData["productionLotQuality"] as ProductionLot);
            }
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            TempData["productionLot"] = TempData["productionLot"] ?? productionLot;
            TempData.Keep("productionLot");

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotTrashsDetailAddNew(ProductionLotTrash productionLotTrash)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotTrash = productionLot.ProductionLotTrash ?? new List<ProductionLotTrash>();

            if (ModelState.IsValid)
            {
                productionLotTrash.id = productionLot.ProductionLotTrash.Count() > 0 ? productionLot.ProductionLotTrash.Max(pld => pld.id) + 1 : 1;
                productionLot.ProductionLotTrash.Add(productionLotTrash);
                UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
            }

            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotTrashsDetailUpdate(ProductionLotTrash productionLotTrash)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotTrash.FirstOrDefault(it => it.id == productionLotTrash.id);//var modelItem = productionLot.ProductionLotTrash.FirstOrDefault(pll => pll.id_item == productionLotTrash.id_item);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
                        TempData["productionLot"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotTrashsDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotTrashsDetailDelete(System.Int32 id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            //if (id_item >= 0)
            //{
                try
                {
                var productionLotTrash = productionLot.ProductionLotTrash.FirstOrDefault(p => p.id == id);//var productionLotTrash = productionLot.ProductionLotTrash.FirstOrDefault(p => p.id_item == id_item);
                if (productionLotTrash != null)
                        productionLot.ProductionLotTrash.Remove(productionLotTrash);

                    UpdateProductionLotProductionLotTrashsDetailTotals(productionLot);
                    TempData["productionLot"] = productionLot;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            //}

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();
            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotTrashsDetailPartial", model);
        }

        private void UpdateProductionLotProductionLotTrashsDetailTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityTrash = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;// db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotTrash in productionLot.ProductionLotTrash)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotTrash.id_item);
                var id_metricUnitAux = productionLotTrash.id_metricUnit;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityTrash += productionLotTrash.quantity * factor;
            }

            productionLot.totalQuantityTrash = decimal.Round(productionLot.totalQuantityTrash, 2);

        }

        #endregion

        #region PRODUCTION LOT LOSS

        [ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLossDetailPartial()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            if (productionLot == null)
            {
                productionLot = (TempData["productionLotProcess"] as ProductionLot);
            }

            if (productionLot == null)
            {
                productionLot = (TempData["productionLotReception"] as ProductionLot);
            }

            if (productionLot == null)
            {
                productionLot = (TempData["productionLotQuality"] as ProductionLot);
            }

            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();

            TempData["productionLot"] = TempData["productionLot"] ?? productionLot;
            TempData.Keep("productionLot");

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLossDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLossDetailAddNew(ProductionLotLoss productionLotLoss)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotLoss = productionLot.ProductionLotLoss ?? new List<ProductionLotLoss>();

            if (ModelState.IsValid)
            {
                productionLotLoss.id = productionLot.ProductionLotLoss.Count() > 0 ? productionLot.ProductionLotLoss.Max(pld => pld.id) + 1 : 1;
                productionLot.ProductionLotLoss.Add(productionLotLoss);
                UpdateProductionLotProductionLotLossDetailTotals(productionLot);
            }

            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLossDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLossDetailUpdate(ProductionLotLoss productionLotLoss)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = productionLot.ProductionLotLoss.FirstOrDefault(it => it.id == productionLotLoss.id);//var modelItem = productionLot.ProductionLotLoss.FirstOrDefault(pll => pll.id_item == productionLotLoss.id_item);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        UpdateProductionLotProductionLotLossDetailTotals(productionLot);
                        TempData["productionLot"] = productionLot;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();

            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLossDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionLotEditFormProductionLotLossDetailDelete(System.Int32 id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            productionLot = productionLot ?? db.ProductionLot.FirstOrDefault(i => i.id == productionLot.id);
            productionLot = productionLot ?? new ProductionLot();

            //if (id_item >= 0)
            //{
            try
            {
                var productionLotLoss = productionLot.ProductionLotLoss.FirstOrDefault(p => p.id == id);//var productionLotLoss = productionLot.ProductionLotLoss.FirstOrDefault(p => p.id_item == id_item);
                if (productionLotLoss != null)
                    productionLot.ProductionLotLoss.Remove(productionLotLoss);

                UpdateProductionLotProductionLotLossDetailTotals(productionLot);
                TempData["productionLot"] = productionLot;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("productionLot");

            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();
            return PartialView("ProductionLot/_ProductionLotEditFormProductionLotLossDetailPartial", model);
        }

        private void UpdateProductionLotProductionLotLossDetailTotals(ProductionLot productionLot)
        {
            productionLot.totalQuantityLoss = 0.0M;

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;// db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

            foreach (var productionLotLoss in productionLot.ProductionLotLoss)
            {
                var ItemAux = db.Item.FirstOrDefault(fod => fod.id == productionLotLoss.id_item);
                var id_metricUnitAux = productionLotLoss.id_metricUnit;//ItemAux?.ItemPurchaseInformation.MetricUnit.id ?? 0;
                var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
                var factor = id_metricUnitLbsAux == id_metricUnitAux && id_metricUnitAux != 0 ? 1 : (metricUnitConversion?.factor ?? 0);
                productionLot.totalQuantityLoss += productionLotLoss.quantity * factor;
            }

            productionLot.totalQuantityLoss = decimal.Round(productionLot.totalQuantityLoss.Value, 2);

        }

        #endregion

        #region DETAILS VIEW

        public ActionResult ProductionLotDetailProductionLotLiquidationTotalsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotLiquidationTotal.ToList() ?? new List<ProductionLotLiquidationTotal>();
            return PartialView("ProductionLot/_ProductionLotDetailProductionLotLiquidationTotalsPartial", model.ToList());
        }
        public ActionResult ProductionLotDetailProductionLotLiquidationsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotLiquidation.ToList() ?? new List<ProductionLotLiquidation>();
            if (id_productionLot == null)
            {
                ProductionLot pl = (TempData["productionLot"] as ProductionLot);
                model = pl?.ProductionLotLiquidation.ToList();
            }

            #region Liquiacion Valorizada - 6115
            string setting_liquidacionValorizada = (db.Setting.FirstOrDefault(r => r.code == "LIQNOVAL")?.value ?? "NO");
            bool isRequestCarMachine = false;
            if (setting_liquidacionValorizada == "SI")
            {
                isRequestCarMachine = (ProductionProcess.GetOneById(productionLot.id_productionProcess)?.requestCarMachine ?? false);
            }
            ViewBag.isRequestCarMachine = isRequestCarMachine;
            #endregion

            return PartialView("ProductionLot/_ProductionLotDetailProductionLotLiquidationsPartial", model.ToList());
        }

        public ActionResult ProductionLotDetailProductionLotPackingMaterialsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotPackingMaterial.ToList() ?? new List<ProductionLotPackingMaterial>();

            model = model.Where(d => d.isActive && d.quantity > 0).ToList();

            return PartialView("ProductionLot/_ProductionLotDetailProductionLotPackingMaterialsPartial", model.ToList());
        }

        public ActionResult ProductionLotDetailProductionLotTrashsPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotTrash.ToList() ?? new List<ProductionLotTrash>();
            return PartialView("ProductionLot/_ProductionLotDetailProductionLotTrashsPartial", model.ToList());
        }
        public ActionResult ProductionLotDetailProductionLotLossPartial(int? id_productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ViewData["id_productionLot"] = id_productionLot;
            var productionLot = db.ProductionLot.FirstOrDefault(p => p.id == id_productionLot);
            var model = productionLot?.ProductionLotLoss.ToList() ?? new List<ProductionLotLoss>();
            return PartialView("ProductionLot/_ProductionLotDetailProductionLotLossPartial", model.ToList());
        }

        #endregion

        #region AXILIAR FUNCTIONS

        #region General

        [HttpPost, ValidateInput(false)]
        public JsonResult ExistsConversionWithLbsProductionLotLiquidation(int? id_item, int? id_metricUnit)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = db.Item.FirstOrDefault(p => p.id == id_item);

            var metricUnitUMTPAux = db.Setting.FirstOrDefault(fod => fod.code.Equals("UMTP"));
            var id_metricUnitUMTPValueAux = int.Parse(metricUnitUMTPAux?.value ?? "0");
            var metricUnitUMTP = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitUMTPValueAux);

            var id_metricUnitLbsAux = metricUnitUMTP?.id ?? 0;// db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;
            var id_metricUnitAux = id_metricUnit ?? item?.Presentation?.MetricUnit?.id ?? item?.ItemInventory?.MetricUnit?.id;

            var metricUnitConversion = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                    muc.id_metricOrigin == id_metricUnitAux &&
                                                                                    muc.id_metricDestiny == id_metricUnitLbsAux);
            return Json(new
            {
                metricUnitConversionValue = id_metricUnitAux == id_metricUnitLbsAux ? 1 : metricUnitConversion?.factor ?? 0,
                metricUnitName = metricUnitUMTP?.name ?? "",
                metricUnitCode = metricUnitUMTP?.code ?? ""
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitMetricUnitsItem(int? id_item, int? id_metricUnit)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_item);

            var item = new
            {
                id = itemAux?.id,
                name = itemAux?.name
            };

            var id_metricTypeAux = itemAux?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => ((w.id_metricType == id_metricTypeAux) && w.isActive) || w.id == id_metricUnit).Select(s => new
            {
                id = s.id,
                code = s.code
            }); ;

            var result = new
            {
                item = item,
                metricUnits = metricUnits
            };

            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionLotWarehouseLocation(int? id_warehouse)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var result = new
            {
                warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse && w.isActive)
                                       .Select(s => new {
                                           id = s.id,
                                           name = s.name
                                       })

            };

            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetProductionLotWarehouseLocation(int? id_warehouse, int? id_warehouseLocation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var warehouseLocationAux = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouse).ToList();
            var warehouseLocationCurrentAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocation);
            if (warehouseLocationCurrentAux != null && !warehouseLocationAux.Contains(warehouseLocationCurrentAux)) warehouseLocationAux.Add(warehouseLocationCurrentAux);

            var result = new
            {
                warehouseLocations = warehouseLocationAux
                                       .Select(s => new {
                                           id = s.id,
                                           name = s.name
                                       })

            };

            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateDepartament(int? id_employee, string tempDataKeep)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var employee = db.Employee.FirstOrDefault(fod => fod.id == id_employee);

            var result = new
            {
                employeeDepartament = employee?.Department.name
            };

            TempData.Keep(tempDataKeep);

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult OnEmissionDateDocumentValidation(string emissionDate, string tempDataKeep)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var result = new
            {
                itsValided = 1,
                Error = ""

            };

            DateTime _emissionDate = JsonConvert.DeserializeObject<DateTime>(emissionDate);

            var DFEAFA = db.Setting.FirstOrDefault(fod => fod.code == "DFEAFA" && fod.id_company == this.ActiveCompanyId)?.value ?? "0";
            var int_DFEAFA = int.Parse(DFEAFA);

            DateTime nowDate = DateTime.Now;
            DateTime emissionMinDate = nowDate.AddDays(-int_DFEAFA);
			DateTime emissionMaxDate = nowDate;//.AddDays(int_DFEAFA);

            if (DateTime.Compare(_emissionDate.Date, emissionMaxDate) > 0 || DateTime.Compare(emissionMinDate.Date, _emissionDate.Date) > 0)
            {
                result = new
                {
                    itsValided = 0,
                    Error = "La Fecha de Emisión debe estar en el siguiente intervalo de fecha ("+ emissionMinDate.ToString("dd/MM/yyyy") + " - " + nowDate.ToString("dd/MM/yyyy") + ")."
                };
            }


            TempData.Keep(tempDataKeep);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProductionLot(int? idPlQc, ProductionLot productionLot)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var productionLotAux = (TempData["productionLot"] as ProductionLot);

            bool noProdQuality = true;

            if (productionLotAux.id == 0)
            {
                noProdQuality = false;
            }
            else
            {
                var plTmp = db.ProductionLot.FirstOrDefault(fod => fod.id == productionLotAux.id);
                if (plTmp != null && plTmp.ProductionLotDetail != null)
                {
                    var pldTmp = plTmp.ProductionLotDetail.FirstOrDefault(fod => fod.id == idPlQc);

                    if (pldTmp != null)
                    {
                        var qcTmp = db.ProductionLotDetailQualityControl
                                        .FirstOrDefault(fod => fod.id_productionLotDetail == idPlQc);

                        if (qcTmp != null)
                        {
                            noProdQuality = true;
                        }
                        else
                        {
                            noProdQuality = false;
                        }
                    }
                    else
                    {
                        noProdQuality = false;
                    }
                }
                else
                {
                    noProdQuality = false;
                }
            }

            productionLotAux.receptionDate = productionLot.receptionDate;
            productionLotAux.id_productionUnit = productionLot.id_productionUnit;
            productionLotAux.description = productionLot.description;
            productionLotAux.reference = productionLot.reference;
            productionLotAux.expirationDate = productionLot.expirationDate;
            productionLotAux.totalQuantityOrdered = productionLot.totalQuantityOrdered;
            productionLotAux.totalQuantityRemitted = productionLot.totalQuantityRemitted;
            productionLotAux.totalQuantityRecived = productionLot.totalQuantityRecived;
            productionLotAux.totalQuantityLiquidation = productionLot.totalQuantityLiquidation;
            productionLotAux.wholeSubtotal = productionLot.wholeSubtotal;
            productionLotAux.subtotalTail = productionLot.subtotalTail;
            productionLotAux.totalQuantityTrash = productionLot.totalQuantityTrash;
            productionLotAux.wholeGarbagePounds = productionLot.wholeGarbagePounds;
            productionLotAux.poundsGarbageTail = productionLot.poundsGarbageTail;
            productionLotAux.totalQuantityLiquidationAdjust = productionLot.totalQuantityLiquidationAdjust;
            productionLotAux.wholeLeftover = productionLot.wholeLeftover;

            productionLotAux.totalAdjustmentPounds = productionLot.totalAdjustmentPounds;
            productionLotAux.totalAdjustmentWholePounds = productionLot.totalAdjustmentWholePounds;
            productionLotAux.totalAdjustmentTailPounds = productionLot.totalAdjustmentTailPounds;
            productionLotAux.wholeSubtotalAdjust = productionLot.wholeSubtotalAdjust;
            productionLotAux.subtotalTailAdjust = productionLot.subtotalTailAdjust;
            productionLotAux.wholeSubtotalAdjustProcess = productionLot.wholeSubtotalAdjustProcess;
            productionLotAux.wholeTotalToPay = productionLot.wholeTotalToPay;
            productionLotAux.tailTotalToPay = productionLot.tailTotalToPay;
            productionLotAux.totalToPay = productionLot.totalToPay;



            productionLotAux.liquidationDate = productionLot.liquidationDate;
            productionLotAux.closeDate = productionLot.closeDate;
            productionLotAux.liquidationPaymentDate = productionLot.liquidationPaymentDate;

            if(productionLot.id_buyer != 0 && productionLot.id_buyer != null)
            {
                productionLotAux.id_buyer = productionLot.id_buyer;
            }

            if (productionLot.id_provider != 0 && productionLot.id_provider != null)
            {
                productionLotAux.id_provider = productionLot.id_provider;
            }

            if (productionLot.id_productionUnitProviderPool != 0 && productionLot.id_productionUnitProviderPool != null)
            {
                productionLotAux.id_productionUnitProviderPool = productionLot.id_productionUnitProviderPool;
            }

            if (productionLot.id_productionProcess != 0)
            {
                productionLotAux.id_productionProcess = productionLot.id_productionProcess;
            }

            var result = new
            {
                Message = "OK",
                hasProdQuality = (noProdQuality) ? "YES":"NO"
            };

            TempData["productionLot"] = productionLotAux;
            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Trash

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedTrash(int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var productionLotTrashAux = productionLot.ProductionLotTrash.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew);
            if (productionLotTrashAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de Desperdicio"

                };

            }


            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult TrashItemDetailData(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);


            var id_metricTypeAux = item?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.id_metricType == id_metricTypeAux) && w.isActive).Select(s => new
            {
                id = s.id,
                code = s.code
            }); ;

            var id_metricUnitAux = item?.ItemHeadIngredient?.id_metricUnit ?? item?.ItemInventory?.id_metricUnitInventory;

            var id_warehouseAux = item?.ItemInventory?.Warehouse.id;
            var id_warehouseLocationAux = item?.ItemInventory?.WarehouseLocation.id;
            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            var result = new
            {
                id_metricUnit = id_metricUnitAux,
                metricUnits = metricUnits,
                id_warehouse = id_warehouseAux,
                id_warehouseLocation = id_warehouseLocationAux,
                warehouseLocations = warehouseLocations
            };


            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost, ValidateInput(false)]
        //public JsonResult ProductionLotTrashDetails()
        //{
        //    ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
        //    productionLot = productionLot ?? new ProductionLot();
        //    productionLot.ProductionLotTrash = productionLot.ProductionLotTrash ?? new List<ProductionLotTrash>();
        //    TempData.Keep("productionLot");

        //    return Json(productionLot.ProductionLotTrash.Select(d => d.id_item).ToList(), JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region Loss

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedLoss(int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var productionLotLossAux = productionLot.ProductionLotLoss.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew);
            if (productionLotLossAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de Merma"

                };

            }


            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult LossItemDetailData(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);


            var id_metricTypeAux = item?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.id_metricType == id_metricTypeAux) && w.isActive).Select(s => new
            {
                id = s.id,
                code = s.code
            }); ;

            var id_metricUnitAux = item?.ItemHeadIngredient?.id_metricUnit ?? item?.ItemInventory?.id_metricUnitInventory;

            var id_warehouseAux = item?.ItemInventory?.Warehouse.id;
            var id_warehouseLocationAux = item?.ItemInventory?.WarehouseLocation.id;
            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            var result = new
            {
                id_metricUnit = id_metricUnitAux,
                metricUnits = metricUnits,
                id_warehouse = id_warehouseAux,
                id_warehouseLocation = id_warehouseLocationAux,
                warehouseLocations = warehouseLocations
            };


            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        #endregion


        #region Packing Material

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemPackingMaterialDetailsData(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            Item item = db.Item.FirstOrDefault(i => i.id == id_item);

            if (item == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                ItemDetailData = new
                {
                    masterCode = item.masterCode,
                    metricUnit = item.ItemInventory?.MetricUnit?.code,
                }
            };

            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PackingMaterialDetails(int? id_itemCurrent)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            productionLot.ProductionLotPackingMaterial = productionLot.ProductionLotPackingMaterial ?? new List<ProductionLotPackingMaterial>();


            var items = db.Item.Where(w => (w.isActive && w.id_company == this.ActiveCompanyId && w.InventoryLine.code.Equals("MI") && w.ItemType.code.Equals("INS") && w.ItemTypeCategory.code.Equals("MDE")) || w.id == id_itemCurrent).ToList();
            var tempItems = new List<Item>();
            foreach (var i in items)
            {
                if (!(productionLot.ProductionLotPackingMaterial.Any(a => a.id_item == i.id && a.quantityRequiredForProductionLot != 0)) || i.id == id_itemCurrent)
                {
                    tempItems.Add(i);
                }

            }
            items = tempItems;
            var result = new
            {
                items = items.Select(s => new
                {
                    id = s.id,
                    masterCode = s.masterCode,
                    ItemInventoryMetricUnitCode = (s.ItemInventory != null) ? s.ItemInventory.MetricUnit.code : "",
                    name = s.name
                }).ToList(),
                Message = "Ok"
            };

            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void UpdateProductionLotLiquidationPackingMaterialDetail(ProductionLot productionLot, ProductionLotLiquidation productionLotLiquidation)
        {
            for (int i = productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Count - 1; i >= 0; i--)
            {


                var detail = productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.ElementAt(i);

                detail.ProductionLotPackingMaterial.quantityRequiredForProductionLot -= detail.quantity;
                detail.ProductionLotPackingMaterial.manual = detail.ProductionLotPackingMaterial.quantityRequiredForProductionLot == 0;
                detail.ProductionLotPackingMaterial.quantity -= detail.quantity;

                //for (int j = detail.ProductionLotDetailPurchaseDetail.Count - 1; j >= 0; j--)
                //{
                //    var detailProductionLotDetailPurchaseDetail = detail.ProductionLotDetailPurchaseDetail.ElementAt(j);
                //    detail.ProductionLotDetailPurchaseDetail.Remove(detailProductionLotDetailPurchaseDetail);
                //    db.Entry(detailProductionLotDetailPurchaseDetail).State = EntityState.Deleted;
                //}

                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Remove(detail);
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

            //foreach (var remissionGuideDetailDispatchMaterialDetail in remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail)
            //{
            //    remissionGuideDetailDispatchMaterialDetail.RemissionGuideDispatchMaterial.quantityRequiredForPurchase -= remissionGuideDetailDispatchMaterialDetail.quantity;
            //    remissionGuideDetailDispatchMaterialDetail.RemissionGuideDispatchMaterial.sourceExitQuantity -= remissionGuideDetailDispatchMaterialDetail.quantity;

            //    remissionGuideDetail.RemissionGuideDetailDispatchMaterialDetail.Remove(remissionGuideDetailDispatchMaterialDetail);
            //    //db.Entry(remissionGuideDetailDispatchMaterialDetail).State = EntityState.Deleted;

            //}

            //if (!productionLotLiquidation.isActive) return;
            if (!productionLot.ProductionLotLiquidation.Any(a => a.id == productionLotLiquidation.id)) return;

            if (productionLotLiquidation.Item == null)
            {
                productionLotLiquidation.Item = db.Item.FirstOrDefault(fod => fod.id == productionLotLiquidation.id_item);
            }
            var itemIngredientMDE = productionLotLiquidation.Item.ItemIngredient.Where(w => w.Item1.InventoryLine.code.Equals("MI") && w.Item1.ItemType.code.Equals("INS") && w.Item1.ItemTypeCategory.code.Equals("MDE"));//"MI": Linea de Inventario Materiales e Insumos, "INS": Tipo de Producto Insumos y  "MDE": Categoría de Tipo de Producto Meteriales de Empaque
            if (itemIngredientMDE.Count() == 0) return;
            var id_metricUnitLiquidation = productionLotLiquidation.id_metricUnit;//ItemPurchaseInformation?.id_metricUnitPurchase;
            var id_metricUnitItemHeadIngredient = productionLotLiquidation.Item.ItemHeadIngredient?.id_metricUnit;
            var factorConversionLiquidationFormulation = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                             muc.id_metricOrigin == id_metricUnitLiquidation &&
                                                                                             muc.id_metricDestiny == id_metricUnitItemHeadIngredient);
            if (id_metricUnitLiquidation != null && id_metricUnitLiquidation == id_metricUnitItemHeadIngredient)
            {
                factorConversionLiquidationFormulation = new MetricUnitConversion() { factor = 1 };
            }
            if (factorConversionLiquidationFormulation == null)
            {
                var metricUnitLiquidation = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitLiquidation);
                throw new Exception("Falta el Factor de Conversión entre : " + (metricUnitLiquidation?.code ?? "(UM No Existe)") + ", del Ítem: " + productionLotLiquidation.Item.name + " y " + (productionLotLiquidation.Item.ItemHeadIngredient?.MetricUnit?.code ?? "(UM No Existe)") + " configurado en la cabecera de la formulación del este Ítem. Necesario para cargar los Materiales de Empaque Configúrelo, e intente de nuevo");
            }

            foreach (var iimdd in itemIngredientMDE)
            {
                var quantityMetricUnitItemHeadIngredient = productionLotLiquidation.quantity * factorConversionLiquidationFormulation.factor;
                var amountItemHeadIngredient = (productionLotLiquidation.Item.ItemHeadIngredient?.amount ?? 0);
                if (amountItemHeadIngredient == 0)
                {
                    throw new Exception("La cantidad en la cabecera de la formulación del Ítem: " + productionLotLiquidation.Item.name + " no está configurada o es cero, debe configurar un valor mayor a cero. Configúrelo, e intente de nuevo");
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
                var factorConversionFormulationInventory = db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId &&
                                                                                                  muc.id_metricOrigin == id_metricUnitFormulation &&
                                                                                                  muc.id_metricDestiny == id_metricUnitInventory);
                if (id_metricUnitFormulation != null && id_metricUnitFormulation == id_metricUnitInventory)
                {
                    factorConversionFormulationInventory = new MetricUnitConversion() { factor = 1 };
                }
                if (factorConversionFormulationInventory == null)
                {
                    throw new Exception("Falta el Factor de Conversión entre : " + iimdd.MetricUnit?.code ?? "(UM No Existe)" + ", del Ítem: " + iimdd.Item1.name + " y " + iimdd.Item1.ItemInventory?.MetricUnit?.code ?? "(UM No Existe)" + " configurado en el detalle de la formulación del Ítem: " + productionLotLiquidation.Item.name + ". Necesario para cargar los Materiales de Empaque Configúrelo, e intente de nuevo");
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
                    productionLotPackingMaterial.id_userUpdate = ActiveUser.id;
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
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now,
                        id_userUpdate = ActiveUser.id,
                        dateUpdate = DateTime.Now,
                        ProductionLotLiquidationPackingMaterialDetail = new List<ProductionLotLiquidationPackingMaterialDetail>()
                    };
                    productionLot.ProductionLotPackingMaterial.Add(productionLotPackingMaterial);
                }

                var productionLotLiquidationPackingMaterialDetail = new ProductionLotLiquidationPackingMaterialDetail
                {
                    ProductionLotLiquidation = productionLotLiquidation,
                    id_productionLotLiquidation = productionLotLiquidation.id,
                    ProductionLotPackingMaterial = productionLotPackingMaterial,
                    id_productionLotPackingMaterial = productionLotPackingMaterial.id,
                    quantity = quantityUMInventory
                };
                productionLotLiquidation.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
                productionLotPackingMaterial.ProductionLotLiquidationPackingMaterialDetail.Add(productionLotLiquidationPackingMaterialDetail);
            }

        }

        #endregion

        #region Liquidation

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedLiquidation(int? id_salesOrderNew, int? id_itemNew, int? id_warehouseNew, int? id_warehouseLocationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var productionLotLiquidationAux = productionLot.ProductionLotLiquidation.FirstOrDefault(fod => fod.id_item == id_itemNew &&
                                                                                fod.id_warehouse == id_warehouseNew &&
                                                                                fod.id_warehouseLocation == id_warehouseLocationNew &&
                                                                                (db.SalesOrderDetail.FirstOrDefault(fod2 => fod2.id == fod.id_salesOrderDetail)?.id_salesOrder == id_salesOrderNew ||
                                                                                fod.id_salesOrder == id_salesOrderNew));
            if (productionLotLiquidationAux != null)
            {
                var itemAux = db.Item.FirstOrDefault(fod => fod.id == id_itemNew);
                var warehouseAux = db.Warehouse.FirstOrDefault(fod => fod.id == id_warehouseNew);
                var warehouseLocationAux = db.WarehouseLocation.FirstOrDefault(fod => fod.id == id_warehouseLocationNew);
                ViewData["EditError"] = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de esta Liquidación";
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Item: " + itemAux.name +
                            ",  en la bodega: " + warehouseAux.name +
                            ", en la ubicación: " + warehouseLocationAux.name + ",  en los detalles de esta Liquidación"

                };

            }


            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ItemDetailData(int? id_item, string quantity)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);

            decimal _quantity = Convert.ToDecimal(quantity);
            var presentation = item?.Presentation;

            decimal maximum = 1;
            if (presentation != null)
            {
                if (presentation.code != null)
                {
                    if (presentation.code.Substring(0, 1) == "M") maximum = presentation.maximum;

                }
            }

            decimal _minimunValue = (presentation?.minimum ?? 0) * maximum; 

            decimal _quantityPounds = QuantityTotalsPoundsCalculation(_minimunValue, presentation, _quantity);

            decimal quantityTotal = QuantityTotalByPresentation(_minimunValue,presentation, _quantity);


            var id_metricTypeAux = item?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => (w.id_metricType == id_metricTypeAux) && w.isActive).Select(s => new
            {
                id = s.id,
                code = s.code
            });

            var id_metricUnitAux = item?.ItemHeadIngredient?.id_metricUnit ?? item?.ItemInventory?.id_metricUnitInventory;
            var id_metricUnitPresentation = presentation?.id_metricUnit ?? id_metricUnitAux;

            var id_warehouseAux = item?.ItemInventory?.Warehouse.id;
            var id_warehouseLocationAux = item?.ItemInventory?.WarehouseLocation.id;
            var warehouseLocations = db.WarehouseLocation.Where(w => w.id_warehouse == id_warehouseAux)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            var result = new
            {
                id_metricUnit = id_metricUnitAux,
                metricUnits = metricUnits,
                id_warehouse = id_warehouseAux,
                id_warehouseLocation = id_warehouseLocationAux,
                warehouseLocations = warehouseLocations,
                quantityTotal = quantityTotal,
                quantityLiquidationPounds = _quantityPounds,
                id_metricUnitPresentation = id_metricUnitPresentation
            };


            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SalesOrderDetailData(int? id_salesOrder, int? id_salesOrderDetailIni, int? id_itemIni)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var salesOrder = db.SalesOrder.FirstOrDefault(fod => fod.id == id_salesOrder);


            var items = salesOrder != null ? salesOrder.SalesOrderDetail.Where(w => (w.quantityDelivered < w.quantityApproved && w.Item.isActive) || (w.id == id_salesOrderDetailIni && w.id_item == id_itemIni))
                                                                                                           .Select(s => new
                                                                                                           {
                                                                                                               id = s.Item.id,
                                                                                                               name = s.Item.name,
                                                                                                               clase = s.Item.ItemGeneral.ItemGroupCategory?.name ?? "",
                                                                                                               size = s.Item.ItemGeneral.ItemSize?.name
                                                                                                           }) :
                                                               db.Item.Where(w => w.InventoryLine.code.Equals("PP") && w.isActive)
                                                                      .Select(s => new
                                                                      {
                                                                          id = s.id,
                                                                          name = s.name,
                                                                          clase = s.ItemGeneral.ItemGroupCategory != null ? s.ItemGeneral.ItemGroupCategory.name : "",
                                                                          size = s.ItemGeneral.ItemSize != null ? s.ItemGeneral.ItemSize.name : ""
                                                                      });

            var result = new
            {
                items = items
            };


            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLiquidationDetailItem(string txtItemsFilter, 
            int? id_salesOrder, int? id_salesOrderDetailIni, int? id_itemIni, int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            if (txtItemsFilter == null)
            {
                txtItemsFilter = "";
            }
            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var codeAux = productionLot.ProcessType?.code ?? "ENT";

            var salesOrder = db.SalesOrder.FirstOrDefault(fod => fod.id == id_salesOrder);


            var items = salesOrder != null ? salesOrder
                                            .SalesOrderDetail
                                            .Where(w => (w.quantityDelivered < w.quantityApproved && w.Item.isActive) 
                                                            || (w.id == id_salesOrderDetailIni && w.id_item == id_itemIni) || w.id_item == id_item)
                                                                .Select(s => s.Item
                                                                ).ToList() :
                                                        db.Item.AsEnumerable().Where(w => (w.InventoryLine.code.Equals("PP") || w.InventoryLine.code.Equals("PT")) 
                                                                            && w.isActive && (txtItemsFilter !="" ? w.auxCode.Contains(txtItemsFilter) : w.auxCode == w.auxCode) &&
                                                                            (codeAux == "ENT" || 
                                                                            (codeAux != "ENT" 
                                                                            && w.ItemType != null && w.ItemType.ProcessType != null 
                                                                            && !w.ItemType.ProcessType.code.Equals("ENT")))).ToList();
            
            TempData.Keep("productionLot");

            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_item";
                p.ClientInstanceName = "id_item";
                //p.DataSource = null;// DataProviderItem.AllItemsByCompany((int?)ViewData["id_company"]);
                p.ValueField = "id";
                //p.TextField = "name";
                p.TextFormatString = "{0},{1}";
                p.ValueType = typeof(int);
                //settings.Properties.CallbackPageSize = 5;
                p.Width = Unit.Percentage(113);
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                p.EnableSynchronization = DefaultBoolean.False;
                p.Columns.Add("auxCode", "Código Auxiliar", 100);
                p.Columns.Add("name", "Nombre del Producto", 200);//, Unit.Percentage(70));
                p.Columns.Add("ItemTypeCategory.name", "Clase", 70);//, Unit.Percentage(50));
                p.Columns.Add("ItemGeneral.ItemSize.name", "Talla", 70);//, Unit.Percentage(50));
                                                                        //settings.Properties.ClientSideEvents.Init = "ItemCombo_OnInit";
                                                                        //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
                p.ClientSideEvents.SelectedIndexChanged = "ItemProductionLotLiquidationDetailCombo_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnItemProductionLotLiquidationDetailValidation";


                //settings.Properties.ClientInstanceName = "id_person";
                //settings.Width = Unit.Percentage(100);

                //settings.Properties.DropDownStyle = DropDownStyle.DropDownList;
                //settings.Properties.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;

                //settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                //settings.Properties.CallbackPageSize = 5;
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.CallbackRouteValues = new { Controller = "ProductionLot", Action = "GetLiquidationDetailItem"/*, TextField = "CityName"*/ };
                p.ClientSideEvents.BeginCallback = "ProductionLotLiquidationDetailItem_BeginCallback";
                p.ClientSideEvents.EndCallback = "ProductionLotLiquidationDetailItem_EndCallback";

                //settings.Properties.ValueField = "id";
                //settings.Properties.TextField = "fullname_businessName";
                //settings.Properties.ValueType = typeof(int);
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                p.BindList(items);//.Bind(id_person);

            });
        }

        [HttpPost]
        public ActionResult GetLiquidationMetricUnit(int? id_item, int? id_metricUnit)
        {

            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);
            var id_metricTypeAux = item?.id_metricType;
            var metricUnits = db.MetricUnit.Where(w => ((w.id_metricType == id_metricTypeAux) && w.isActive) || w.id == id_metricUnit).Select(s => new
            {
                id = s.id,
                code = s.code
            }).ToList();


            TempData.Keep("productionLot");

            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_item";
                p.ClientInstanceName = "id_metricUnit";
                //p.DataSource = null;// DataProviderItem.AllItemsByCompany((int?)ViewData["id_company"]);
                p.ValueField = "id";
                p.TextField = "code";
                //p.TextFormatString = "{0}";
                p.ValueType = typeof(int);
                //settings.Properties.CallbackPageSize = 5;
                p.Width = Unit.Percentage(150);
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                p.EnableSynchronization = DefaultBoolean.False;
                p.ClientSideEvents.SelectedIndexChanged = "MetricUnitProductionLotLiquidationDetailCombo_SelectedIndexChanged";
                p.ClientSideEvents.Validation = "OnMetricUnitProductionLotLiquidationDetailValidation";

                p.CallbackRouteValues = new { Controller = "ProductionLot", Action = "GetLiquidationMetricUnit"/*, TextField = "CityName"*/ };
                p.ClientSideEvents.BeginCallback = "ProductionLotLiquidationMetricUnit_BeginCallback";
                p.ClientSideEvents.EndCallback = "ProductionLotLiquidationMetricUnit_EndCallback";

                p.BindList(metricUnits);//.Bind(id_person);

            });
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InitSalesOrderItemAndMetricUnit(int? id_salesOrder, int? id_item, int? id_metricUnit)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);

            productionLot = productionLot ?? new ProductionLot();

            var salesOrderAux = db.SalesOrder.FirstOrDefault(fod => fod.id == id_salesOrder);

            var salesOrder = new
            {
                id = salesOrderAux?.id,
                name = salesOrderAux?.Document.number
            };

            //var items = salesOrderAux != null ? salesOrderAux.SalesOrderDetail.Where(w => (w.quantityDelivered < w.quantityApproved && w.Item.isActive) || w.id_item == id_item)
            //                                                                                               .Select(s => new
            //                                                                                               {
            //                                                                                                   id = s.Item.id,
            //                                                                                                   name = s.Item.name,
            //                                                                                                   clase = s.Item.ItemGeneral.ItemGroupCategory?.name ?? "",
            //                                                                                                   size = s.Item.ItemGeneral.ItemSize?.name
            //                                                                                               }) :
            //                                                   db.Item.Where(w => w.InventoryLine.code.Equals("PP") && w.isActive)
            //                                                          .Select(s => new
            //                                                          {
            //                                                              id = s.id,
            //                                                              name = s.name,
            //                                                              clase = s.ItemGeneral.ItemGroupCategory != null ? s.ItemGeneral.ItemGroupCategory.name : "",
            //                                                              size = s.ItemGeneral.ItemSize != null ? s.ItemGeneral.ItemSize.name : ""
            //                                                          });
            //var item = db.Item.FirstOrDefault(fod => fod.id == id_item);
            //var id_metricTypeAux = item?.id_metricType;
            //var metricUnits = db.MetricUnit.Where(w => ((w.id_metricType == id_metricTypeAux) && w.isActive) || w.id == id_metricUnit).Select(s => new
            //{
            //    id = s.id,
            //    code = s.code
            //});

            //var result = new
            //{
            //    salesOrder = salesOrder,
            //    items = items,
            //    metricUnits = metricUnits
            //};
            var result = new
            {
                salesOrder = salesOrder
                //items = items,
                //metricUnits = metricUnits
            };
            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult UpdateQuantityTotal(int? id_item, string quantity, int? id_metricUnit)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var item = db.Item.FirstOrDefault(fod => fod.id == id_item);

            decimal _quantity = Convert.ToDecimal(quantity);
            var presentation = item?.Presentation;

            decimal maximum = 1;
            if (presentation != null  )
            {
                if (presentation.code != null)
                {
                    if (presentation.code.Substring(0, 1) == "M") maximum = presentation.maximum;

                }
            }

            decimal _minimunValue = (presentation?.minimum ?? 0) * maximum;

            decimal _quantityPounds = QuantityTotalsPoundsCalculation(_minimunValue, presentation, _quantity);

            decimal quantityTotal   = QuantityTotalByPresentation(_minimunValue,presentation, _quantity);

            var id_metricUnitPresentation = presentation?.id_metricUnit ?? id_metricUnit;

            var result = new
            {
                quantityTotal = quantityTotal,
                id_metricUnitPresentation = id_metricUnitPresentation,
                quantityPoundsLiquidation = (decimal.Truncate(_quantityPounds*100)/100)
            };


            TempData.Keep("productionLot");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private decimal QuantityTotalsPoundsCalculation(decimal _mValue, Presentation presentation, decimal _quantity)
        {
            decimal _factor = presentation?.MetricUnit?.code == "Kg" ? Convert.ToDecimal(2.2046) : 1;
            decimal _factorlb = Math.Truncate((_mValue * _factor) * 100000m) / 100000m;

			if (presentation == null)
            {
                return _mValue;
            }
            else
            {
                return Math.Round((_factorlb * _quantity), 2);
            }
        }

        private decimal QuantityTotalByPresentation(decimal _mValue, Presentation presentation, decimal quantity)
        {

            decimal _factor = presentation?.MetricUnit?.code == "Kg" ? 1 :Convert.ToDecimal(2.2046);
            decimal _factorGk = Math.Truncate((_mValue / _factor) * 100000m) / 100000m;

            if (presentation == null)
            {
                return quantity;
            }
            else
            {
                return Math.Round((_factorGk * quantity), 2);
                //  return presentation.minimum * quantity;
            }

        }

        #endregion

        #region FILTER

        [HttpPost]
        public JsonResult MoveIdsLeftToRight(int[] ids)
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            List<FilterType> listFilterType = (TempData["listFilterType"] as List<FilterType>);

            List<FilterTypeWithCondition> listFilterTypeWithCondition = (TempData["listFilterTypeWithCondition"] as List<FilterTypeWithCondition>);

            foreach (var id in ids)
            {
                int id_logicalOperator = 1;
                LogicalOperator logicalOperator = null;
                DateTime? valueConditionFromDateTime = null;
                DateTime? valueConditionToDateTime = null;
                decimal? valueConditionFromDecimal = null;
                decimal? valueConditionToDecimal = null;
                string valueConditionTextOrSelect = null;
                bool? valueConditionCheck = null;
                int[] valueConditionSelectValue = null;
                string valueConditionSelectValueText = null;

                var filterType = listFilterType.FirstOrDefault(fod=> fod.id == id);
                if(filterType.type == "Date")
                {
                    id_logicalOperator = 5;
                    logicalOperator = DataProviderLogicalOperator.LogicalOperatorDateNumbersById(5);
                    valueConditionFromDateTime = DateTime.Now;
                }
                else
                if (filterType.type == "Text")
                {
                    id_logicalOperator = 3;
                    logicalOperator = DataProviderLogicalOperator.LogicalOperatorTextsById(3);
                }
                else
                if (filterType.type == "Number")
                {
                    id_logicalOperator = 5;
                    logicalOperator = DataProviderLogicalOperator.LogicalOperatorDateNumbersById(5);
                    valueConditionFromDecimal = 0;
                }
                else
                if (filterType.type == "Select")
                {
                    id_logicalOperator = 1;
                    logicalOperator = DataProviderLogicalOperator.LogicalOperatorSelectsById(1);
                    //valueConditionSelectValue = null;
                    //valueConditionSelectValueText = null;
                }
                else
                if (filterType.type == "Check")
                {
                    valueConditionCheck = true;
                }
                
                listFilterTypeWithCondition.Add(new FilterTypeWithCondition {
                    id = id,
                    filterType = filterType,
                    id_logicalOperator = id_logicalOperator,
                    logicalOperator = logicalOperator,
                    valueConditionFromDateTime = valueConditionFromDateTime,
                    valueConditionToDateTime = valueConditionToDateTime,
                    valueConditionFromDecimal = valueConditionFromDecimal,
                    valueConditionToDecimal = valueConditionToDecimal,
                    valueConditionTextOrSelect = valueConditionTextOrSelect,
                    valueConditionCheck = valueConditionCheck,
                    valueConditionSelectValue = valueConditionSelectValue,
                    valueConditionSelectValueText = valueConditionSelectValueText
                });
                listFilterType.Remove(filterType);
            }

            //var model = listFilterTypeWithCondition ?? new List<FilterTypeWithCondition>();

            //TempData["listFilterTypeWithCondition"] = TempData["listFilterTypeWithCondition"] ?? listFilterTypeWithCondition;


            var result = new
            {
                message = "OK"
            };


            TempData.Keep("listFilterTypeWithCondition");
            TempData.Keep("listFilterType");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult MoveIdsRightToLeft(int[] ids)
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            List<FilterType> listFilterType = (TempData["listFilterType"] as List<FilterType>);

            List<FilterTypeWithCondition> listFilterTypeWithCondition = (TempData["listFilterTypeWithCondition"] as List<FilterTypeWithCondition>);

            foreach (var id in ids)
            {
                var filterTypeWithCondition = listFilterTypeWithCondition.FirstOrDefault(fod => fod.id == id);
                listFilterType.Add(filterTypeWithCondition.filterType);
                listFilterTypeWithCondition.Remove(filterTypeWithCondition);
            }

            //var model = listFilterTypeWithCondition ?? new List<FilterTypeWithCondition>();

            //TempData["listFilterTypeWithCondition"] = TempData["listFilterTypeWithCondition"] ?? listFilterTypeWithCondition;


            var result = new
            {
                message = "OK"
            };


            TempData.Keep("listFilterTypeWithCondition");
            TempData.Keep("listFilterType");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult FilterClearGlobal()
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            List<FilterType> listFilterType = (TempData["listFilterType"] as List<FilterType>);

            List<FilterTypeWithCondition> listFilterTypeWithCondition = (TempData["listFilterTypeWithCondition"] as List<FilterTypeWithCondition>);
            for (int i = listFilterTypeWithCondition.Count - 1; i >= 0; i--)
            {
                var detail = listFilterTypeWithCondition.ElementAt(i);

                listFilterType.Add(detail.filterType);

                listFilterTypeWithCondition.Remove(detail);
            }

            var result = new
            {
                message = "OK"
            };


            TempData.Keep("listFilterTypeWithCondition");
            TempData.Keep("listFilterType");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult UpdateFilterSelect(int? key, int[] valueConditionSelectValue, string valueConditionSelectValueText)
        {
            //UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            //List<FilterType> listFilterType = (TempData["listFilterType"] as List<FilterType>);

            List<FilterTypeWithCondition> listFilterTypeWithCondition = (TempData["listFilterTypeWithCondition"] as List<FilterTypeWithCondition>);

            var listFilterTypeWithConditionAux = listFilterTypeWithCondition.FirstOrDefault(fod=> fod.id == key);
            if(listFilterTypeWithConditionAux != null)
            {
                listFilterTypeWithConditionAux.valueConditionSelectValue = valueConditionSelectValue;
                listFilterTypeWithConditionAux.valueConditionSelectValueText = valueConditionSelectValueText;
            }
            //for (int i = listFilterTypeWithCondition.Count - 1; i >= 0; i--)
            //{
            //    var detail = listFilterTypeWithCondition.ElementAt(i);

            //    listFilterType.Add(detail.filterType);

            //    listFilterTypeWithCondition.Remove(detail);
            //}

            var result = new
            {
                message = "OK"
            };


            TempData.Keep("listFilterTypeWithCondition");
            TempData.Keep("listFilterType");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetSelectInit(int? key)
        {
            List<FilterTypeWithCondition> listFilterTypeWithCondition = (TempData["listFilterTypeWithCondition"] as List<FilterTypeWithCondition>);

            string list_idItemsStr = "";
            var listFilterTypeWithConditionAux = listFilterTypeWithCondition.FirstOrDefault(fod => fod.id == key);
            if (listFilterTypeWithConditionAux != null)
            {
                if(listFilterTypeWithConditionAux.valueConditionSelectValue != null && listFilterTypeWithConditionAux.valueConditionSelectValue.Count() > 0)
                {
                    foreach (var i in listFilterTypeWithConditionAux.valueConditionSelectValue)
                    {
                        if (i != 0)
                        {
                            if (list_idItemsStr == "") list_idItemsStr = i.ToString();
                            else list_idItemsStr += "," + i.ToString();
                        }
                    }
                }
            }

            var result = new
            {
                message = "OK",
                items = list_idItemsStr
            };


            TempData.Keep("listFilterTypeWithCondition");
            TempData.Keep("listFilterType");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSelect(string dataSource)
        {
            TempData.Keep("listFilterTypeWithCondition");
            TempData.Keep("listFilterType");
            //ViewData["id_person"] = id_person;
            var dataProvider = GetDataProvider(dataSource);
            return GridViewExtension.GetTokenBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "valueConditionSelect";
                p.Width = Unit.Percentage(100);

                //p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                //p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.NullDisplayText = "Todo";
                p.NullText = "Todo";
                p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.CallbackRouteValues = new { Controller = "ProductionLot", Action = "GetSelect"/*, TextField = "CityName"*/ };
                p.ClientSideEvents.BeginCallback = "ValueConditionSelect_BeginCallback";
                p.CallbackPageSize = 5;
                p.ClientSideEvents.Init = "ValueConditionSelect_Init";
                p.ClientSideEvents.ValueChanged = "ValueConditionSelect_ValueChanged";
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                p.ClientSideEvents.EndCallback = "ValueConditionSelect_EndCallback";

                p.ValueField = "id";
                p.TextField = "name";
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                //p.ClientSideEvents.Validation = "OnPersonValidation";

                //p.TextField = textField;
                p.BindList(dataProvider);//.Bind(id_person);

            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }
        //public static void Invoke<T>(string methodName) where T : new()
        //{
        //    T instance = new T();
        //    MethodInfo method = typeof(T).GetMethod(methodName);
        //    method.Invoke(instance, new[] { this.ActiveCompanyId.ToString() });
        //}
        private IEnumerable GetDataProvider(string dataSource)
        {

            try
            {
                //return (IEnumerable)Invoker.CreateAndInvoke("IEnumerable", null, dataSource, new[] { this.ActiveCompanyId.ToString() });
                if (dataSource == "DataProviderDocumentState.AllDocumentStatesByCompany")
                {
                    return DataProviderDocumentState.AllDocumentStatesByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderPaymentTerm.AllPaymentTermsByCompany")
                {
                    return DataProviderPaymentTerm.AllPaymentTermsByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderPaymentMethod.AllPaymentMethodsByCompany")
                {
                    return DataProviderPaymentMethod.AllPaymentMethodsByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderPerson.AllProvidersByCompany")
                {
                    return DataProviderPerson.AllProvidersByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderPerson.AllBuyersByCompany")
                {
                    return DataProviderPerson.AllBuyersByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderItem.AllPurchaseItemsByCompany")
                {
                    return DataProviderItem.AllPurchaseItemsByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderPriceList.AllPriceListsForPurchaseByCompany")
                {
                    return DataProviderPriceList.AllPriceListsForPurchaseByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderProductionLotState.AllProductionLotStatesByCompany")
                {
                    return DataProviderProductionLotState.AllProductionLotStatesByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderProductionUnit.AllProductionUnitRECsByCompany")
                {
                    return DataProviderProductionUnit.AllProductionUnitRECsByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderPerson.AllRequestingPersonsByCompany")
                {
                    return DataProviderPerson.AllRequestingPersonsByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderPerson.AllReceivingPersonsByCompany")
                {
                    return DataProviderPerson.AllReceivingPersonsByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderProductionUnitProvider.AllProductionUnitProviderPoolsByCompany")
                {
                    return DataProviderProductionUnitProvider.AllProductionUnitProviderPoolsByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderItem.AllPurchaseItemsMPByCompany")
                {
                    return DataProviderItem.AllPurchaseItemsMPByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderWarehouse.AllWarehousesMPByCompany")
                {
                    return DataProviderWarehouse.AllWarehousesMPByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderWarehouseLocation.AllWarehouseLocationsMPByCompany")
                {
                    return DataProviderWarehouseLocation.AllWarehouseLocationsMPByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderItem.AllPurchaseItemsMDDByCompany")
                {
                    return DataProviderItem.AllPurchaseItemsMDDByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderWarehouse.AllWarehousesMIByCompany")
                {
                    return DataProviderWarehouse.AllWarehousesMIByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderWarehouseLocation.AllWarehouseLocationsMIByCompany")
                {
                    return DataProviderWarehouseLocation.AllWarehouseLocationsMIByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderItem.AllPurchaseItemsPPPTByCompany")
                {
                    return DataProviderItem.AllPurchaseItemsPPPTByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderSalesOrder.AllSalesOrderByCompany")
                {
                    return DataProviderSalesOrder.AllSalesOrderByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderItem.AllPurchaseItemsMDEByCompany")
                {
                    return DataProviderItem.AllPurchaseItemsMDEByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderItem.AllPurchaseItemsDEByCompany")
                {
                    return DataProviderItem.AllPurchaseItemsDEByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderWarehouse.AllWarehousesDEByCompany")
                {
                    return DataProviderWarehouse.AllWarehousesDEByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderWarehouseLocation.AllWarehouseLocationsDEByCompany")
                {
                    return DataProviderWarehouseLocation.AllWarehouseLocationsDEByCompany(this.ActiveCompanyId);
                }
                else
                if (dataSource == "DataProviderAdvancedFilter.CheckDataSource")
                {
                    return DataProviderAdvancedFilter.CheckDataSource();
                }
                else return null;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }

        #endregion

        #region ADVANCED FILTER

        [HttpPost, ValidateInput(false)]
        public JsonResult GetAdvancedFiltersConfiguration(int? id_advancedFiltersConfiguration)
        {
            List<AdvancedFiltersConfiguration> advancedFiltersConfigurations = (TempData["advancedFiltersConfigurations"] as List<AdvancedFiltersConfiguration>);
            var advancedFiltersConfiguration = new AdvancedFiltersConfiguration();
            if (advancedFiltersConfigurations != null)
            {
                advancedFiltersConfiguration = advancedFiltersConfigurations.FirstOrDefault(fod => fod.id == id_advancedFiltersConfiguration);
            }

            var result = new
            {
                message = "OK",
                name_typeFiltersConfiguration = advancedFiltersConfiguration.TypeFiltersConfiguration?.name,
                id_typeFiltersConfiguration = advancedFiltersConfiguration.TypeFiltersConfiguration?.id,
                code_typeFiltersConfiguration = advancedFiltersConfiguration.TypeFiltersConfiguration?.code,
                datasource_advancedFiltersConfiguration = advancedFiltersConfiguration.dataSource
            };


            TempData.Keep("advancedFiltersConfigurations");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComparisonOperator(int? id_typeFiltersConfiguration)
        {
            //List<AdvancedFiltersConfiguration> advancedFiltersConfigurations = (TempData["advancedFiltersConfigurations"] as List<AdvancedFiltersConfiguration>);
            //var advancedFiltersConfiguration = new AdvancedFiltersConfiguration();
            //if (advancedFiltersConfigurations != null)
            //{
            //    advancedFiltersConfiguration = advancedFiltersConfigurations.FirstOrDefault(fod=> fod.id == id_advancedFiltersConfiguration);
            //}
            var comparisonOperators = db.TypeFiltersConfigurationComparisonOperator.Where(w => w.id_typeFiltersConfiguration == id_typeFiltersConfiguration)
                                                                                                             .Select(s=> new { s.ComparisonOperator.id, s.ComparisonOperator.name}).ToList();
            TempData.Keep("advancedFiltersConfigurations");
            //ViewData["id_person"] = id_person;
            //var dataProvider = GetDataProvider(dataSource);
            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "comparisonOperator";
                p.Width = Unit.Percentage(100);

                //p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                //p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                //p.NullDisplayText = "Todo";
                //p.NullText = "Todo";
                //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.CallbackRouteValues = new { Controller = "ProductionLot", Action = "GetComparisonOperator"/*, TextField = "CityName"*/ };
                p.ClientSideEvents.BeginCallback = "ComparisonOperatorComboBox_BeginCallback";
                p.CallbackPageSize = 5;
                //p.ClientSideEvents.Init = "ValueConditionSelect_Init";
                //p.ClientSideEvents.ValueChanged = "ValueConditionSelect_ValueChanged";
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                //p.ClientSideEvents.EndCallback = "ValueConditionSelect_EndCallback";
                p.DropDownStyle = DropDownStyle.DropDown;

                p.ValueField = "id";
                p.TextField = "name";
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                //p.ClientSideEvents.Validation = "OnPersonValidation";

                //p.TextField = textField;
                p.BindList(comparisonOperators);//.Bind(id_person);

            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        [HttpPost]
        public ActionResult GetConditionID(string datasource_advancedFiltersConfiguration)
        {
            //List<AdvancedFiltersConfiguration> advancedFiltersConfigurations = (TempData["advancedFiltersConfigurations"] as List<AdvancedFiltersConfiguration>);
            //var advancedFiltersConfiguration = new AdvancedFiltersConfiguration();
            //if (advancedFiltersConfigurations != null)
            //{
            //    advancedFiltersConfiguration = advancedFiltersConfigurations.FirstOrDefault(fod=> fod.id == id_advancedFiltersConfiguration);
            //}
            //var comparisonOperators = db.TypeFiltersConfigurationComparisonOperator.Where(w => w.id_typeFiltersConfiguration == id_typeFiltersConfiguration)
            //                                                                                 .Select(s => new { s.ComparisonOperator.id, s.ComparisonOperator.name }).ToList();
            TempData.Keep("advancedFiltersConfigurations");
            //ViewData["id_person"] = id_person;
            var dataProvider = GetDataProvider(datasource_advancedFiltersConfiguration);
            return GridViewExtension.GetComboBoxCallbackResult(p => {
                //settings.Name = "id_person";
                p.ClientInstanceName = "conditionID";
                p.Width = Unit.Percentage(100);

                //p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                //p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                //p.NullDisplayText = "Todo";
                //p.NullText = "Todo";
                //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;

                p.CallbackRouteValues = new { Controller = "ProductionLot", Action = "GetConditionID"/*, TextField = "CityName"*/ };
                p.ClientSideEvents.BeginCallback = "ConditionIDComboBox_BeginCallback";
                p.CallbackPageSize = 5;
                //p.ClientSideEvents.Init = "ValueConditionSelect_Init";
                //p.ClientSideEvents.ValueChanged = "ValueConditionSelect_ValueChanged";
                //settings.Properties.EnableCallbackMode = true;
                //settings.Properties.TextField = "CityName";
                //p.ClientSideEvents.EndCallback = "ValueConditionSelect_EndCallback";
                p.DropDownStyle = DropDownStyle.DropDown;

                p.ValueField = "id";
                p.TextField = "name";
                //settings.ReadOnly = codeState != "01";//Pendiente
                //p.ShowModelErrors = true;
                //settings.Properties.ClientSideEvents.SelectedIndexChanged = "BusinessOportunityBusinessPartner_SelectedIndexChanged";
                //p.ClientSideEvents.Validation = "OnPersonValidation";

                //p.TextField = textField;
                p.BindList(dataProvider);//.Bind(id_person);

            });

            //return PartialView("Component/_ComboBoxBusinessPlanningDetailPerson");
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ValidQuery(string query)
        {
            var isSintaxiError = true;
            try
            {
                var isValidQuery = true;

                var strQuery = query.Replace("'Verdadero'", "1");
                strQuery = strQuery.Replace("'Falso'", "0");
                strQuery = strQuery.Replace(" O ", " Or ");
                strQuery = strQuery.Replace(" Y ", " And ");
                var comparisonOperators = db.ComparisonOperator.ToList();
                foreach (var item in comparisonOperators)
                {
                    if (item.code != "Like")
                    {
                        strQuery = strQuery.Replace(" " + item.name + " ", " " + item.code + " ");
                    }
                }
                strQuery = strQuery.Replace(" Contiene ", " Like ");

                //Not Like
                string[] separators2 = { " Not Like " };
                //string value = "The handsome, energetic, young dog was playing with his smaller, more lethargic litter mate.";
                string[] words = strQuery.Split(separators2, StringSplitOptions.RemoveEmptyEntries);
                var nameStr = "";
                //var wordCurrent = "";
                foreach (var word in words)
                {
                    if (nameStr == "")
                    {
                        //wordCurrent = word;
                        nameStr = (word);
                    }
                    else
                    {
                        //wordCurrent += "." + word;
                        nameStr += (" Not Like" + GetCompleteConditionLike(" " + word));
                    }
                }
                strQuery = nameStr;

                //Like
                string[] separators = { " Like " };
                //string value = "The handsome, energetic, young dog was playing with his smaller, more lethargic litter mate.";
                words = strQuery.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                nameStr = "";
                //var wordCurrent = "";
                foreach (var word in words)
                {
                    if (nameStr == "")
                    {
                        //wordCurrent = word;
                        nameStr = (word);
                    }
                    else
                    {
                        //wordCurrent += "." + word;
                        nameStr += (" Like" + GetCompleteConditionLike(" " + word));
                    }
                }

                strQuery = nameStr;

                List<AdvancedFiltersConfiguration> advancedFiltersConfigurations = (TempData["advancedFiltersConfigurations"] as List<AdvancedFiltersConfiguration>);
                var nameTableMain = advancedFiltersConfigurations.FirstOrDefault().nameTableMain;
                var select = "Select T1.id From " + nameTableMain + " T1 ";
                List<LogicalOperator> tableOfSelect = new List<LogicalOperator>();
                tableOfSelect.Add(new LogicalOperator { code = "T1", name = nameTableMain });

                var countAux = 1;
                foreach (var item in advancedFiltersConfigurations)
                {
                    if (strQuery.IndexOf("[" + item.alias + "]") >= 0)
                    {
                        //N N N
                        if (string.IsNullOrEmpty(item.nameTableJoinDetail) && string.IsNullOrEmpty(item.nameTableJoinMove) && string.IsNullOrEmpty(item.nameTableJoinShow))
                        {
                            var nameAux = item.TypeFiltersConfiguration.code == "Date" ? "CONVERT(date, " + "T1." + item.name + ")" : "T1." + item.name;
                            strQuery = strQuery.Replace("[" + item.alias + "]", nameAux);
                        }
                        else //S N N
                        if (!string.IsNullOrEmpty(item.nameTableJoinDetail) && string.IsNullOrEmpty(item.nameTableJoinMove) && string.IsNullOrEmpty(item.nameTableJoinShow))
                        {
                            var firstOrDefaultAux = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinDetail);
                            if(firstOrDefaultAux == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinDetail)){
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinDetail debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinDetail2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinDetail2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinDetail };
                                tableOfSelect.Add(firstOrDefaultAux);
                                select += "Inner Join " + item.nameTableJoinDetail + " " + firstOrDefaultAux.code + " On T1." + item.nameJoinDetail2 + " = " + firstOrDefaultAux.code + "." + item.nameJoinDetail + " ";
                            }
                            var nameAux = item.TypeFiltersConfiguration.code == "Date" ? "CONVERT(date, " + firstOrDefaultAux.code + "." + item.name + ")" : firstOrDefaultAux.code + "." + item.name;
                            strQuery = strQuery.Replace("[" + item.alias + "]", nameAux);
                        }
                        else // S N S
                        if (!string.IsNullOrEmpty(item.nameTableJoinDetail) && string.IsNullOrEmpty(item.nameTableJoinMove) && !string.IsNullOrEmpty(item.nameTableJoinShow))
                        {
                            var firstOrDefaultAux = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinDetail);
                            if (firstOrDefaultAux == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinDetail))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinDetail debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinDetail2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinDetail2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinDetail };
                                tableOfSelect.Add(firstOrDefaultAux);
                                select += "Inner Join " + item.nameTableJoinDetail + " " + firstOrDefaultAux.code + " On T1." + item.nameJoinDetail2 + " = " + firstOrDefaultAux.code + "." + item.nameJoinDetail + " ";
                            }
                            var firstOrDefaultAux2 = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinShow);
                            if (firstOrDefaultAux2 == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinShow))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinShow debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinShow2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinShow2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux2 = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinShow };
                                tableOfSelect.Add(firstOrDefaultAux2);
                                select += "Inner Join " + item.nameTableJoinShow + " " + firstOrDefaultAux2.code + " On " + firstOrDefaultAux2.code + "." + item.nameJoinShow2 + " = " + firstOrDefaultAux.code + "." + item.nameJoinShow + " ";
                            }
                            var nameAux = item.TypeFiltersConfiguration.code == "Date" ? "CONVERT(date, " + firstOrDefaultAux2.code + "." + item.name + ")" : firstOrDefaultAux2.code + "." + item.name;
                            strQuery = strQuery.Replace("[" + item.alias + "]", nameAux);
                        }
                        else // S S S
                        if (!string.IsNullOrEmpty(item.nameTableJoinDetail) && !string.IsNullOrEmpty(item.nameTableJoinMove) && !string.IsNullOrEmpty(item.nameTableJoinShow))
                        {
                            var firstOrDefaultAux = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinDetail);
                            if (firstOrDefaultAux == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinDetail))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinDetail debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinDetail2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinDetail2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinDetail };
                                tableOfSelect.Add(firstOrDefaultAux);
                                select += "Inner Join " + item.nameTableJoinDetail + " " + firstOrDefaultAux.code + " On T1." + item.nameJoinDetail2 + " = " + firstOrDefaultAux.code + "." + item.nameJoinDetail + " ";
                            }
                            var firstOrDefaultAux1 = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinMove);
                            if (firstOrDefaultAux1 == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinMove))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinMove debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinMove2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinMove2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux1 = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinMove };
                                tableOfSelect.Add(firstOrDefaultAux1);
                                select += "Inner Join " + item.nameTableJoinMove + " " + firstOrDefaultAux1.code + " On " + firstOrDefaultAux1.code + "." + item.nameJoinMove2 + " = " + firstOrDefaultAux.code + "." + item.nameJoinMove + " ";
                            }
                            var firstOrDefaultAux2 = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinShow);
                            if (firstOrDefaultAux2 == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinShow))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinShow debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinShow2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinShow2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux2 = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinShow };
                                tableOfSelect.Add(firstOrDefaultAux2);
                                select += "Inner Join " + item.nameTableJoinShow + " " + firstOrDefaultAux2.code + " On " + firstOrDefaultAux2.code + "." + item.nameJoinShow2 + " = " + firstOrDefaultAux1.code + "." + item.nameJoinShow + " ";
                            }
                            var nameAux = item.TypeFiltersConfiguration.code == "Date" ? "CONVERT(date, " + firstOrDefaultAux2.code + "." + item.name + ")" : firstOrDefaultAux2.code + "." + item.name;
                            strQuery = strQuery.Replace("[" + item.alias + "]", nameAux);
                        }
                        else // N S N
                        if (string.IsNullOrEmpty(item.nameTableJoinDetail) && !string.IsNullOrEmpty(item.nameTableJoinMove) && string.IsNullOrEmpty(item.nameTableJoinShow))
                        {
                            var firstOrDefaultAux = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinMove);
                            if (firstOrDefaultAux == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinMove))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinMove debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinMove2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinMove2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinMove };
                                tableOfSelect.Add(firstOrDefaultAux);
                                select += "Inner Join " + item.nameTableJoinMove + " " + firstOrDefaultAux.code + " On T1." + item.nameJoinMove + " = " + firstOrDefaultAux.code + "." + item.nameJoinMove2 + " ";
                            }
                            var nameAux = item.TypeFiltersConfiguration.code == "Date" ? "CONVERT(date, " + firstOrDefaultAux.code + "." + item.name + ")" : firstOrDefaultAux.code + "." + item.name;
                            strQuery = strQuery.Replace("[" + item.alias + "]", nameAux);
                        }
                        else // N S S
                        if (string.IsNullOrEmpty(item.nameTableJoinDetail) && !string.IsNullOrEmpty(item.nameTableJoinMove) && !string.IsNullOrEmpty(item.nameTableJoinShow))
                        {
                            var firstOrDefaultAux = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinMove);
                            if (firstOrDefaultAux == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinMove))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinMove debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinMove2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinMove2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinMove };
                                tableOfSelect.Add(firstOrDefaultAux);
                                select += "Inner Join " + item.nameTableJoinMove + " " + firstOrDefaultAux.code + " On T1." + item.nameJoinMove + " = " + firstOrDefaultAux.code + "." + item.nameJoinMove2 + " ";
                            }
                            var firstOrDefaultAux2 = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinShow);
                            if (firstOrDefaultAux2 == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinShow))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinShow debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinShow2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinShow2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux2 = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinShow };
                                tableOfSelect.Add(firstOrDefaultAux2);
                                select += "Inner Join " + item.nameTableJoinShow + " " + firstOrDefaultAux2.code + " On " + firstOrDefaultAux2.code + "." + item.nameJoinShow2 + " = " + firstOrDefaultAux.code + "." + item.nameJoinShow + " ";
                            }
                            var nameAux = item.TypeFiltersConfiguration.code == "Date" ? "CONVERT(date, " + firstOrDefaultAux2.code + "." + item.name + ")" : firstOrDefaultAux2.code + "." + item.name;
                            strQuery = strQuery.Replace("[" + item.alias + "]", nameAux);
                        }
                        else // N N S
                        if (string.IsNullOrEmpty(item.nameTableJoinDetail) && string.IsNullOrEmpty(item.nameTableJoinMove) && !string.IsNullOrEmpty(item.nameTableJoinShow))
                        {
                            var firstOrDefaultAux2 = tableOfSelect.FirstOrDefault(fod => fod.name == item.nameTableJoinShow);
                            if (firstOrDefaultAux2 == null)
                            {
                                if (string.IsNullOrEmpty(item.nameJoinShow))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinShow debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                if (string.IsNullOrEmpty(item.nameJoinShow2))
                                {
                                    isSintaxiError = false;
                                    throw new Exception("Error de Configuración: En la tabla AdvancedFiltersConfiguration id: " + item.id.ToString() + " campo nameJoinShow2 debe estar configurado para proceder a ejecutar el Filtro Dinámico");
                                }
                                countAux++;
                                firstOrDefaultAux2 = new LogicalOperator { code = "T" + countAux.ToString(), name = item.nameTableJoinShow };
                                tableOfSelect.Add(firstOrDefaultAux2);
                                select += "Inner Join " + item.nameTableJoinShow + " " + firstOrDefaultAux2.code + " On " + firstOrDefaultAux2.code + "." + item.nameJoinShow2 + " = " + "T1." + item.nameJoinShow + " ";
                            }
                            var nameAux = item.TypeFiltersConfiguration.code == "Date" ? "CONVERT(date, " + firstOrDefaultAux2.code + "." + item.name + ")" : firstOrDefaultAux2.code + "." + item.name;
                            strQuery = strQuery.Replace("[" + item.alias + "]", nameAux);
                        }
                    }

                }
                strQuery = select + " Where " + strQuery;

                var listIds = db.Database.SqlQuery<int>(strQuery).ToList();
                TempData["listIds"] = listIds;
                //   <List<int>>
                //("Select JobID,JobName From jobs");
                //strQuery.IndexOf()
                //var advancedFiltersConfiguration = new AdvancedFiltersConfiguration();
                //if (advancedFiltersConfigurations != null)
                //{
                //    advancedFiltersConfiguration = advancedFiltersConfigurations.FirstOrDefault(fod => fod.id == id_advancedFiltersConfiguration);
                //}

                var result = new
                {
                    message = "OK",
                    isValidQuery = isValidQuery
                };


                TempData.Keep("advancedFiltersConfigurations");
                TempData.Keep("listIds");

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                TempData.Keep("advancedFiltersConfigurations");
                TempData.Keep("listIds");

                return Json(new
                {
                    message = isSintaxiError ? "Sintaxis Error: " + e.Message : e.Message,//"Error de Configuración",
                    isValidQuery = false
                }, JsonRequestBehavior.AllowGet);
            }

        }

        private string GetCompleteConditionLike(string v)
        {
            string[] separators = { "'" };
            //string value = "The handsome, energetic, young dog was playing with his smaller, more lethargic litter mate.";
            string[] words = v.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var nameStr = "";
            int countAux = 0;
            foreach (var word in words)
            {
                countAux++;
                if (nameStr == "")
                {
                    //wordCurrent = word;
                    nameStr = (word);
                }
                else if (countAux == 2)
                {
                    //wordCurrent += "." + word;
                    nameStr += ("'%" + word);
                }
                else if (countAux == 3)
                {
                    //wordCurrent += "." + word;
                    nameStr += ("%'" + word);
                }else
                {
                    nameStr += ("'" + word);
                }
            }
            return nameStr;
        }

        [HttpPost]
        public ActionResult GetResultsAdvancedFilter(string codeAdvancedFiltersConfiguration)
        {
            //List<int> listIds = (TempData["listIds"] as List<int>);
            //listIds = listIds ?? new List<int>();
            //if (codeAdvancedFiltersConfiguration == "OC")
            //{
            //    List<PurchaseOrder> model = db.PurchaseOrder.AsEnumerable().Where(w => listIds.Contains(w.id)).ToList();
            //    TempData["model"] = model;
            //    TempData.Keep("model");

            //    return View("PurchaseOrder/_PurchaseOrderResultsPartial", model.OrderByDescending(o => o.id).ToList());
            //}else
            //if (codeAdvancedFiltersConfiguration == "RP")
            //{
            //    List<ProductionLot> model = db.ProductionLot.AsEnumerable().Where(w => w.ProductionProcess.code == "REC" && listIds.Contains(w.id)).ToList();
            //    TempData["model"] = model;
            //    TempData.Keep("model");

            //    return PartialView("ProductionLotReception/_ProductionLotReceptionResultsPartial", model.OrderByDescending(o => o.id).ToList());
            //}


            return null;

        }

        #endregion

        #endregion

        #region New Version
        [HttpPost]
        public ActionResult GetItems(int? id_item, int? indice, int? id_itemType, int? id_size, int? id_trademark, int? id_presentation,
            string codigoProducto, int? categoriaProducto, int? modeloProducto)
        {
            ProductionLot productionLot = (TempData["productionLot"] as ProductionLot);
            TempData["productionLot"] = productionLot;
            TempData.Keep("productionLot");

            var codeAux = productionLot?.ProcessType?.code ?? "ENT";
            var itemsSeleccionados = db.Item.Where( w => w.id == id_item 
                                                         || 
                                                         (
                                                            ( w.InventoryLine.code.Equals("PP") || w.InventoryLine.code.Equals("PT"))
                                                            && w.isActive 
                                                            && ( codeAux == "ENT" ||
                                                                 ( codeAux != "ENT"
                                                                   && w.ItemType != null && w.ItemType.ProcessType != null
                                                                   && !w.ItemType.ProcessType.code.Equals("ENT")
                                                                  ))
                                                             )).ToList();

            if (id_size != null && id_size > 0)
            {
                itemsSeleccionados = itemsSeleccionados
                    .Where(w => w.ItemGeneral.id_size == id_size).ToList();
            }
            if (id_itemType != null && id_itemType > 0)
            {
                itemsSeleccionados = itemsSeleccionados
                    .Where(w => w.id_itemType == id_itemType).ToList();
            }

            if (id_trademark != null && id_trademark > 0)
            {
                itemsSeleccionados = itemsSeleccionados
                    .Where(w => w.ItemGeneral.id_trademark == id_trademark).ToList();
            }

            if (modeloProducto != null && modeloProducto > 0)
            {
                itemsSeleccionados = itemsSeleccionados
                    .Where(w => w.ItemGeneral.id_trademarkModel == modeloProducto).ToList();
            }

            if (categoriaProducto != null && categoriaProducto > 0)
            {
                itemsSeleccionados = itemsSeleccionados
                    .Where(w => w.ItemGeneral.id_groupCategory == categoriaProducto).ToList();
            }

            if (id_presentation != null && id_presentation > 0)
            {
                itemsSeleccionados = itemsSeleccionados
                    .Where(w => w.id_presentation == id_presentation).ToList();
            }

            if (!String.IsNullOrEmpty(codigoProducto))
            {
                itemsSeleccionados = itemsSeleccionados
                    .Where(w => w.masterCode.Contains(codigoProducto)).ToList();
            }




            ViewData["_ItemsDetailEditPLL"] = itemsSeleccionados;

            ProductionLotLiquidation pll = productionLot?.ProductionLotLiquidation.FirstOrDefault(fod => fod.id == indice);
            return this.PartialView("ProductionLot/ComponentsDetail/_ComboBoxItems", pll ?? new ProductionLotLiquidation());
        }

        [HttpPost]
        public void RefresshDataForEditForm(string txtItemsFilter, string codeProccessType,
            int? id_itemType,
            int? id_size,
            int? id_trademark,
            int? id_presentation,
            string codigoProducto,
            int? categoriaProducto,
            int? modeloProducto)
        {

            
            var items = db.Item.AsEnumerable().Where(w => (w.InventoryLine.code.Equals("PP") || w.InventoryLine.code.Equals("PT"))
                                                                            && w.isActive && (!string.IsNullOrEmpty(txtItemsFilter) ? w.auxCode.Contains(txtItemsFilter) : w.auxCode == w.auxCode) &&
                                                                            (codeProccessType == "ENT" ||
                                                                            (codeProccessType != "ENT"
                                                                            && w.ItemType != null && w.ItemType.ProcessType != null
                                                                            && !w.ItemType.ProcessType.code.Equals("ENT")))).ToList();

            if (id_size != null && id_size > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_size == id_size).ToList();
            }
            if (id_itemType != null && id_itemType > 0)
            {
                items = items
                    .Where(w => w.id_itemType == id_itemType).ToList();
            }

            if (id_trademark != null && id_trademark > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_trademark == id_trademark).ToList();
            }

            if (modeloProducto != null && modeloProducto > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_trademarkModel == modeloProducto).ToList();
            }

            if (categoriaProducto != null && categoriaProducto > 0)
            {
                items = items
                    .Where(w => w.ItemGeneral.id_groupCategory == categoriaProducto).ToList();
            }

            if (id_presentation != null && id_presentation > 0)
            {
                items = items
                    .Where(w => w.id_presentation == id_presentation).ToList();
            }

            if (!String.IsNullOrEmpty(codigoProducto))
            {
                items = items
                    .Where(w => w.masterCode.Contains(codigoProducto)).ToList();
            }





            ViewData["_ItemsDetailEditPLL"] = items;


        }

        #endregion

        #region << Build ComboBoxes Edit >>
        [HttpPost]
        public ActionResult BuildViewDataEdit()
        {
            BuildComboBoxItemType();
            BuildComboBoxTrademark();
            BuildComboBoxPresentation();
            BuildComboBoxSize();
            BuildComboBoxItemTrademarkModel();
            BuildComboBoxItemGroupCategory();
            return Json(new { success = true });

        }

        private void BuildComboBoxItemType()
        {
            ViewData["ItemType"] = db.ItemType.Where(t => (t.InventoryLine.code == "PP" || t.InventoryLine.code == "PT") &&
                                                          t.id_company == ActiveCompany.id && t.isActive).ToList()
                .Select(s => new SelectListItem
                {
                    Text = s.InventoryLine.name + " - " + s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemType()
        {
            BuildComboBoxItemType();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxItemType");
        }

        private void BuildComboBoxTrademark()
        {
            ViewData["Trademark"] = (DataProviderItemTrademark.ItemTrademarks(ActiveCompany.id) as List<ItemTrademark>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxTrademark()
        {
            BuildComboBoxTrademark();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxTrademark");
        }

        private void BuildComboBoxPresentation()
        {
            ViewData["Presentation"] = (db.Presentation.Where(w => w.isActive && w.Item.FirstOrDefault(fod => fod.isActive && (fod.InventoryLine.code == "PP" || fod.InventoryLine.code == "PT")) != null))
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxPresentation()
        {
            BuildComboBoxPresentation();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxPresentation");
        }

        private void BuildComboBoxSize()
        {
            ViewData["Size"] = (DataProviderItemSize.ItemSizes() as List<ItemSize>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxSize()
        {
            BuildComboBoxSize();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxSize");
        }

        private void BuildComboBoxItemTrademarkModel()
        {
            ViewData["ItemTrademarkModel"] = (DataProviderItemTrademarkModel.ItemTrademarkModels() as List<ItemTrademarkModel>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemTrademarkModel()
        {
            BuildComboBoxItemTrademarkModel();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxItemTrademarkModel");
        }

        private void BuildComboBoxItemGroupCategory()
        {
            ViewData["ItemGroupCategory"] = (DataProviderItemGroupCategory.ItemGroupCategories(this.ActiveCompanyId) as List<ItemGroupCategory>)
                .Select(s => new SelectListItem
                {
                    Text = s.name,
                    Value = s.id.ToString()
                }).ToList();
        }

        public ActionResult ComboBoxItemGroupCategory()
        {
            BuildComboBoxItemGroupCategory();
            ViewBag.enabled = true;
            return PartialView("ProductionLot/ComponentsDetail/_ComboBoxItemGroupCategory");
        }

        #endregion
    }
}