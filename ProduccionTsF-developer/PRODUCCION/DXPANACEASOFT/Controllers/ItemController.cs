using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Auxiliares.ExcelFileParsers;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ItemController : DefaultController
    {
        [HttpPost, ValidateInput(false)]
        public ActionResult Index()
        {
            FileUploadHelper.CleanUpUploadedFilesDirectory();
            // Permisos de usuario
            this.SetCommonViewBagData();
            return PartialView();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemsPartial(int? idEditar = null)
        {
            Item tempItem = (TempData["item"] as Item);
            if (tempItem != null)
            {
                TempData.Remove("item");
            }
            var model = db.Item.Where(i => i.id_company == this.ActiveCompanyId);

            var bloquearPeso = db.Setting.FirstOrDefault(fod => fod.code == "BLOQPESO")?.value ?? string.Empty;
            if (!String.IsNullOrEmpty(bloquearPeso) && bloquearPeso == "SI")
            {
                ViewBag.bloquearPeso = true;


            }
            else
            {
                ViewBag.bloquearPeso = false;
            }
            //Permisos de usuario
            this.SetCommonViewBagData();
            this.ViewBag.IdEditar = idEditar;
            return PartialView("_ItemsPartial", model.OrderByDescending(o => o.id).ToList());       
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemDetailPartial(Item item)
        {
            var model = db.Item.FirstOrDefault(i => i.id == item.id);

            return PartialView("_ItemDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItem(int? id, string[] arrayTempDataKeep)
        {
            UpdateArrayTempDataKeep(arrayTempDataKeep);

            var model = id.HasValue
                ? db.Item.FirstOrDefault(i => i.id == id)
                : new Item();

            TempData["item"] = model;
            TempData.Keep("item");

            return PartialView("_EditForm", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemsPartialAddNew(int id_item, int? idMetUnWeightType
                                                , string codTariffItem, string namTariffItem
                                                , int? idItemEquivalence
                                                , Item item, ItemGeneral itemGeneral
                                                , ItemPurchaseInformation itemPurchase
                                                , ItemSaleInformation itemSale, ItemInventory itemInventory
                                                , TariffItem tariffItem, ItemHeadIngredient itemHeadIngredient
                                                , ItemWeightConversionFreezen itemWeightConversionFreezen
                                                , List<ItemDocument> itemDocument
                                                , ItemEquivalence itemEquivalence)
        {

            ModelState.Clear();

            string valSet = DataProviderSetting.ValueSetting("CDCAP");

            //bool isOk = false;
            var model = db.Item;

            bool isValid = false;
            string message;
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        Item tempItem = (TempData["item"] as Item);

                        #region ADD GENERAL

                        if (itemGeneral != null)
                        {
                            item.ItemGeneral = new ItemGeneral
                            {
                                id_group = itemGeneral.id_group,
                                id_subgroup = itemGeneral.id_subgroup,
                                id_groupCategory = itemGeneral.id_groupCategory,
                                id_countryOrigin = itemGeneral.id_countryOrigin,
                                id_trademark = itemGeneral.id_trademark,
                                id_trademarkModel = itemGeneral.id_trademarkModel,
                                id_color = itemGeneral.id_color,
                                id_size = itemGeneral.id_size,
                                id_Person = itemGeneral.id_Person,
                                id_certification = itemGeneral.id_certification,
                                mesVidaUtil = itemGeneral.mesVidaUtil
                                //isASC = itemGeneral.isASC


                            };
                        }

                        #endregion

                        #region ADD PROVIDERS

                        if (tempItem?.ItemProvider != null)
                        {
                            item.ItemProvider = new List<ItemProvider>();

                            var providers = tempItem.ItemProvider.ToList();

                            foreach (var provider in providers)
                            {
                                var tempProvider = new ItemProvider
                                {
                                    id_provider = provider.id_provider,
                                    isFavorite = provider.isFavorite,
                                };
                                item.ItemProvider.Add(tempProvider);
                            }
                        }

                        #endregion

                        #region ADD PURCHASE INFORMATION

                        if (item.isPurchased && itemPurchase != null)
                        {
                            item.ItemPurchaseInformation = new ItemPurchaseInformation
                            {
                                purchasePrice = itemPurchase.purchasePrice,
                                largestPurchasePrice = itemPurchase.largestPurchasePrice,
                                id_metricUnitPurchase = itemPurchase.id_metricUnitPurchase,
                                shortDescriptionPurchase = itemPurchase.shortDescriptionPurchase,
                                descriptionPurchase = itemPurchase.descriptionPurchase
                            };
                        }

                        #endregion

                        #region ADD SALE INFORMATION

                        if (item.isSold && itemSale != null)
                        {
                            item.ItemSaleInformation = new ItemSaleInformation
                            {
                                salePrice = itemSale.salePrice,
                                wholesalePrice = itemSale.wholesalePrice,
                                id_metricUnitSale = itemSale.id_metricUnitSale,
                                shortDescriptionSale = itemSale.shortDescriptionSale,
                                descriptionSale = itemSale.descriptionSale
                            };
                        }

                        #endregion

                        #region ADD INVENTORY

                        if (item.inventoryControl && itemInventory != null)
                        {
                            item.ItemInventory = new ItemInventory
                            {
                                //id_inventoryControlType = itemInventory.id_inventoryControlType,
                                //id_valueValuationMethod = itemInventory.id_valueValuationMethod,
                                isImported = itemInventory.isImported,
                                requiresLot = itemInventory.requiresLot,
                                id_warehouse = itemInventory.id_warehouse,
                                id_warehouseLocation = itemInventory.id_warehouseLocation,
                                minimumStock = itemInventory.minimumStock,
                                maximumStock = itemInventory.maximumStock,
                                currentStock = itemInventory.currentStock,
                                id_metricUnitInventory = itemInventory.id_metricUnitInventory
                                //expirationDate = itemInventory.expirationDate
                            };
                        }

                        #endregion

                        #region ADD HEADER INGREDIENTS

                        if (itemHeadIngredient != null)
                        {
                            item.ItemHeadIngredient = new ItemHeadIngredient
                            {
                                amount = itemHeadIngredient.amount,
                                id_metricUnit = itemHeadIngredient.id_metricUnit
                            };
                        }

                        #endregion

                        #region ADD INGREDIENTS

                        if (tempItem?.ItemIngredient != null)
                        {
                            item.ItemIngredient = new List<ItemIngredient>();

                            var ingredients = tempItem.ItemIngredient.ToList();

                            foreach (var ingredient in ingredients)
                            {
                                var tempIngredient = new ItemIngredient
                                {
                                    id_ingredientItem = ingredient.id_ingredientItem,
                                    id_metricUnit = ingredient.id_metricUnit,
                                    amount = ingredient.amount,
                                    id_metricUnitMax = ingredient.id_metricUnitMax,
                                    amountMax = ingredient.amountMax
                                };
                                item.ItemIngredient.Add(tempIngredient);
                            }
                        }

                        #endregion

                        #region ADD TAXATION

                        if (tempItem?.ItemTaxation != null)
                        {
                            item.ItemTaxation = new List<ItemTaxation>();

                            var taxations = tempItem.ItemTaxation.ToList();

                            foreach (var taxation in taxations)
                            {
                                var tempTaxation = new ItemTaxation
                                {
                                    id_taxType = taxation.id_taxType,
                                    id_rate = taxation.id_rate,
                                    percentage = taxation.percentage
                                };
                                item.ItemTaxation.Add(tempTaxation);
                            }
                        }

                        #endregion

                        #region TARIFFITEM
                        if (!string.IsNullOrEmpty(codTariffItem) && !string.IsNullOrEmpty(namTariffItem))
                        {
                            item.TariffItem = new TariffItem
                            {
                                code = codTariffItem,
                                name = namTariffItem
                            };
                        }
                        #endregion

                        #region ADD ADITIONAL FIELDS

                        if (tempItem?.ItemAditionalField != null)
                        {
                            item.ItemAditionalField = new List<ItemAditionalField>();

                            var aditionalFields = tempItem.ItemAditionalField.ToList();

                            foreach (var aditionalField in aditionalFields)
                            {
                                var tempAditionalField = new ItemAditionalField
                                {
                                    label = aditionalField.label,
                                    value = aditionalField.value
                                };
                                item.ItemAditionalField.Add(tempAditionalField);
                            }
                        }

                        #endregion

                        #region ADD WEIGHT
                        InventoryLine inventoryLineTmp = db.InventoryLine.FirstOrDefault(fod => fod.id == item.id_inventoryLine);

                        inventoryLineTmp = inventoryLineTmp ?? new InventoryLine();
                        if (itemWeightConversionFreezen != null && inventoryLineTmp.code == "PT")
                        {
                            item.ItemWeightConversionFreezen = new ItemWeightConversionFreezen
                            {
                                //id_MetricUnit = itemWeightConversionFreezen.id_MetricUnit,
                                id_MetricUnit = (int)idMetUnWeightType,
                                itemWeightGrossWeight = itemWeightConversionFreezen.itemWeightGrossWeight,
                                itemWeightNetWeight = itemWeightConversionFreezen.itemWeightNetWeight,
                                conversionToKilos = itemWeightConversionFreezen.conversionToKilos == 0 || itemWeightConversionFreezen.conversionToKilos == null ? 1 : itemWeightConversionFreezen.conversionToKilos,
                                conversionToPounds = itemWeightConversionFreezen.conversionToPounds == 0 || itemWeightConversionFreezen.conversionToPounds == null ? 1 : itemWeightConversionFreezen.conversionToPounds,
                                weightWithGlaze = itemWeightConversionFreezen.weightWithGlaze,
                                glazePercentage = itemWeightConversionFreezen.glazePercentage

                            };
                        }
                        #endregion

                        #region ItemDocument

                        if (tempItem?.ItemDocument != null)
                        {
                            item.ItemDocument = new List<ItemDocument>();
                            var itemItemDocument = tempItem.ItemDocument.ToList();

                            foreach (var detail in itemItemDocument)
                            {
                                var tempItemItemDocument = new ItemDocument
                                {
                                    guid = detail.guid,
                                    url = detail.url,
                                    attachment = detail.attachment,
                                    referenceDocument = detail.referenceDocument,
                                    descriptionDocument = detail.descriptionDocument
                                };

                                item.ItemDocument.Add(tempItemItemDocument);
                            }
                        }

                        #endregion

                        #region Item Equivalence
                        var _il = db.InventoryLine
                                    .FirstOrDefault(fod => fod.id == item.id_inventoryLine);
                        if (_il.code.Equals("PP") || _il.code.Equals("PT"))
                        {
                            if (idItemEquivalence > 0)
                            {
                                item.ItemEquivalence = item.ItemEquivalence ?? new ItemEquivalence();
                                item.ItemEquivalence.id_itemEquivalence = idItemEquivalence;
                            }
                        }
                        #endregion

                        // CAMPOS DE AUDITORIA
                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;


                        item.description = item.description?.Trim() ?? "";
                        item.name = item.name.Trim();

                        var aCertification = db.Certification.FirstOrDefault(fod => fod.id == itemGeneral.id_certification);
                        if (!string.IsNullOrEmpty(aCertification?.idProducto))
                        {
                            item.name = aCertification.idProducto + "-" + item.name;
                        }
                        item.description = item.name;

                        InventoryLine inventoryLine = db.InventoryLine.FirstOrDefault(it => it.id == item.id_inventoryLine);
                        if (inventoryLine != null)
                        {
                            inventoryLine.sequential = inventoryLine.sequential + 1;

                            db.InventoryLine.Attach(inventoryLine);
                            db.Entry(inventoryLine).State = EntityState.Modified;
                        }

                        #region"Calculo Automático de Código Auxiliar"
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

                        model.Add(item);

                        MigrationItem migrationItem = db.MigrationItem.FirstOrDefault(fod => fod.id_item == item.id);
                        if (migrationItem == null && item.InventoryLine.code == "PT")
                        {
                            migrationItem = new MigrationItem
                            {
                                id_item = item.id,
                                id_userCreate = ActiveUser.id,
                                dateCreate = DateTime.Now
                            };
                            db.MigrationItem.Add(migrationItem);
                        }

                        db.SaveChanges();
                        trans.Commit();
                        //isOk = true;
                        ViewData["EditMessage"] = SuccessMessage("Producto: " + item.name + " guardado exitosamente"); 
                        this.ViewBag.EditMessage = "Producto: " + item.name + " guardado exitosamente";
                        message = "Producto: " + item.name + " guardado exitosamente";
                        isValid = true;
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        ViewData["EditMessage"] = ErrorMessage(); 
                        this.ViewBag.EditError = e.GetBaseException().Message;
                        message = e.GetBaseException().Message;
                        trans.Rollback();
                    }
                }

            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                this.ViewBag.EditError = "Please, correct all errors.";
                message = "Please, correct all errors.";
            }


            var result = new
            {
                idItem = item.id,
                isValid,
                message,
            };

            return Json(result, JsonRequestBehavior.AllowGet);

            //TempData["item"] = item;
            //TempData.Keep("item");

            //return PartialView("_EditForm", item);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemsPartialUpdate(int id, int? idMetUnWeightType,
            string codTariffItem, string namTariffItem,
            int? idItemEquivalence,
            Item item, ItemGeneral _itemGeneral,
            ItemPurchaseInformation _itemPurchase,
            ItemSaleInformation _itemSale, ItemInventory _itemInventory,
            TariffItem _tariffItem, ItemHeadIngredient _itemHeadIngredient,
            ItemWeightConversionFreezen _itemWeightConversionFreezen,
            List<ItemDocument> _itemDocument, ItemEquivalence itemEquivalence, bool? toReturn)
        {
            //bool isValid = false;

            string valSet = DataProviderSetting.ValueSetting("CDCAP");

            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var model = db.Item;

            bool isValid = false;
            string message = string.Empty;
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    Item modelItem = model.FirstOrDefault(it => it.id == id);


                    //db.SaveChanges();

                    if (modelItem != null)
                    {
                        Item tempItem = (TempData["item"] as Item);
                        ItemGeneral itemGeneral = db.ItemGeneral.FirstOrDefault(ig => ig.id_item == modelItem.id);

                        #region ITEM

                        modelItem.auxCode = item.auxCode;
                        modelItem.foreignName = item.foreignName;
                        modelItem.barCode = item.barCode;
                        modelItem.description = item.description?.Trim() ?? "";
                        modelItem.description2 = item.description2?.Trim() ?? "";
                        modelItem.hasFormulation = item.id_metricType == null ? modelItem.hasFormulation : item.hasFormulation;
                        modelItem.id_inventoryLine = item.id_inventoryLine;
                        modelItem.isConsumed = item.isConsumed;
                        modelItem.id_itemType = item.id_itemType;
                        modelItem.id_itemTypeCategory = item.id_itemTypeCategory;
                        modelItem.id_metricType = item.id_metricType ?? modelItem.id_metricType;
                        modelItem.id_presentation = item.id_presentation;
                        modelItem.isPurchased = item.isPurchased;
                        modelItem.isSold = item.isSold;
                        modelItem.inventoryControl = item.inventoryControl;
                        modelItem.masterCode = item.masterCode;
                        modelItem.name = item.name.Trim();
                        modelItem.photo = item.photo;

                        if (itemGeneral != null && itemGeneral.id_certification != null)
                        {
                            var aModelItemNameSplit = modelItem.name.Split('-');
                            if (aModelItemNameSplit.Length > 1)
                            {
                                modelItem.name = "";
                                for (int i = 1; i < aModelItemNameSplit.Length; i++)
                                {
                                    if (modelItem.name != "")
                                    {
                                        modelItem.name += "-";
                                    }
                                    modelItem.name += aModelItemNameSplit[i];
                                }
                            }
                        }

                        var aCertification = db.Certification.FirstOrDefault(fod => fod.id == _itemGeneral.id_certification);
                        if (!string.IsNullOrEmpty(aCertification?.idProducto))
                        {
                            modelItem.name = aCertification.idProducto + "-" + modelItem.name;
                        }
                        //modelItem.description = modelItem.name;

                        #endregion

                        #region UPDATE GENERAL

                        if (itemGeneral != null /* && tempItem?.ItemGeneral != null*/)
                        {
                            itemGeneral.id_group = _itemGeneral.id_group;
                            itemGeneral.id_subgroup = _itemGeneral.id_subgroup;
                            itemGeneral.id_groupCategory = _itemGeneral.id_groupCategory;
                            itemGeneral.id_countryOrigin = _itemGeneral.id_countryOrigin;
                            itemGeneral.id_trademark = _itemGeneral.id_trademark;
                            itemGeneral.id_trademarkModel = _itemGeneral.id_trademarkModel;
                            itemGeneral.id_color = _itemGeneral.id_color;
                            itemGeneral.id_size = _itemGeneral.id_size;
                            itemGeneral.id_Person = _itemGeneral.id_Person;
                            itemGeneral.id_certification = _itemGeneral.id_certification;
                            itemGeneral.mesVidaUtil = _itemGeneral.mesVidaUtil;
                            //itemGeneral.isASC = _itemGeneral.isASC;

                            db.ItemGeneral.Attach(itemGeneral);
                            db.Entry(itemGeneral).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                        else
                        {
                            _itemGeneral.id_item = item.id;
                            item.ItemGeneral = _itemGeneral;
                            db.ItemGeneral.Attach(_itemGeneral);
                            db.Entry(_itemGeneral).State = EntityState.Added;
                        }

                        #endregion

                        #region UPDATE PROVIDERS

                        if (tempItem?.ItemProvider != null)
                        {
                            var itemProviders = db.ItemProvider.Where(i => i.id_item == modelItem.id);

                            foreach (var provider in itemProviders)
                            {
                                db.ItemProvider.Remove(provider);
                                db.Entry(provider).State = EntityState.Deleted;
                            }

                            //db.SaveChanges();

                            var providers = tempItem.ItemProvider.ToList();

                            foreach (var provider in providers)
                            {
                                var tempProvider = new ItemProvider
                                {
                                    id_item = modelItem.id,
                                    id_provider = provider.id_provider,
                                    isFavorite = provider.isFavorite
                                };
                                db.ItemProvider.Add(tempProvider);
                                db.Entry(tempProvider).State = EntityState.Added;
                            }
                        }

                        #endregion

                        #region UPDATE PURCHASE INFORMATION

                        ItemPurchaseInformation itemPurchaseInformation = db.ItemPurchaseInformation.FirstOrDefault(ig => ig.id_item == modelItem.id);
                        if (itemPurchaseInformation != null /* && tempItem?.ItemGeneral != null*/)
                        {
                            itemPurchaseInformation.purchasePrice = _itemPurchase.purchasePrice;
                            itemPurchaseInformation.largestPurchasePrice = _itemPurchase.largestPurchasePrice;
                            itemPurchaseInformation.id_metricUnitPurchase = _itemPurchase.id_metricUnitPurchase;
                            itemPurchaseInformation.shortDescriptionPurchase = _itemPurchase.shortDescriptionPurchase;
                            itemPurchaseInformation.descriptionPurchase = _itemPurchase.descriptionPurchase;

                            db.ItemPurchaseInformation.Attach(itemPurchaseInformation);
                            db.Entry(itemPurchaseInformation).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                        else
                        {
                            _itemPurchase.id_item = item.id;
                            item.ItemPurchaseInformation = _itemPurchase;
                            db.ItemPurchaseInformation.Attach(_itemPurchase);
                            db.Entry(_itemPurchase).State = EntityState.Added;
                        }

                        #endregion

                        #region UPDATE PURCHASE INFORMATION

                        ItemSaleInformation itemSaleInformation = db.ItemSaleInformation.FirstOrDefault(ig => ig.id_item == modelItem.id);
                        if (itemSaleInformation != null /* && tempItem?.ItemGeneral != null*/)
                        {
                            itemSaleInformation.salePrice = _itemSale.salePrice;
                            itemSaleInformation.wholesalePrice = _itemSale.wholesalePrice;
                            itemSaleInformation.id_metricUnitSale = _itemSale.id_metricUnitSale;
                            itemSaleInformation.shortDescriptionSale = _itemSale.shortDescriptionSale;
                            itemSaleInformation.descriptionSale = _itemSale.descriptionSale;

                            db.ItemSaleInformation.Attach(itemSaleInformation);
                            db.Entry(itemSaleInformation).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                        else
                        {
                            _itemSale.id_item = item.id;
                            item.ItemSaleInformation = _itemSale;
                            db.ItemSaleInformation.Attach(_itemSale);
                            db.Entry(_itemSale).State = EntityState.Added;
                        }

                        #endregion

                        #region UPDATE INVENTORY

                        ItemInventory itemInventory = db.ItemInventory.FirstOrDefault(ig => ig.id_item == modelItem.id);
                        if (itemInventory != null/* && tempItem?.ItemGeneral != null*/)
                        {
                            if (item.inventoryControl)
                            {
                                //itemInventory.id_inventoryControlType = _itemInventory.id_inventoryControlType;
                                //itemInventory.id_valueValuationMethod = _itemInventory.id_valueValuationMethod;
                                itemInventory.isImported = _itemInventory.isImported;
                                itemInventory.requiresLot = _itemInventory.requiresLot;
                                itemInventory.id_warehouse = _itemInventory.id_warehouse;
                                itemInventory.id_warehouseLocation = _itemInventory.id_warehouseLocation;
                                itemInventory.minimumStock = _itemInventory.minimumStock;
                                itemInventory.maximumStock = _itemInventory.maximumStock;
                                itemInventory.currentStock = _itemInventory.currentStock;
                                itemInventory.id_metricUnitInventory = _itemInventory.id_metricUnitInventory;
                                //itemInventory.expirationDate = _itemInventory.expirationDate;

                                db.ItemInventory.Attach(itemInventory);
                                db.Entry(itemInventory).State = EntityState.Modified;
                                //db.SaveChanges();
                            }
                            else
                            {
                                db.ItemInventory.Attach(itemInventory);
                                db.Entry(itemInventory).State = EntityState.Deleted;
                            }

                        }
                        else
                        {
                            if (item.inventoryControl && _itemInventory != null)
                            {
                                _itemInventory.id_item = item.id;
                                item.ItemInventory = _itemInventory;
                                db.ItemInventory.Attach(_itemInventory);
                                db.Entry(_itemInventory).State = EntityState.Added;
                            }
                        }
                        #endregion

                        #region UPDATE HEADER INGREDIENTS

                        ItemHeadIngredient itemHeadIngredient = db.ItemHeadIngredient.FirstOrDefault(ig => ig.id_Item == modelItem.id);
                        if (itemHeadIngredient != null /* && tempItem?.ItemGeneral != null*/)
                        {
                            itemHeadIngredient.amount = _itemHeadIngredient.id_metricUnit == null ? itemHeadIngredient.amount : _itemHeadIngredient.amount;
                            itemHeadIngredient.id_metricUnit = _itemHeadIngredient.id_metricUnit ?? itemHeadIngredient.id_metricUnit;

                            db.ItemHeadIngredient.Attach(itemHeadIngredient);
                            db.Entry(itemHeadIngredient).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                        else
                        {
                            _itemHeadIngredient.id_Item = item.id;
                            item.ItemHeadIngredient = _itemHeadIngredient;
                            db.ItemHeadIngredient.Attach(_itemHeadIngredient);
                            db.Entry(_itemHeadIngredient).State = EntityState.Added;
                        }

                        #endregion

                        #region UPDATE INGREDIENTS

                        if (tempItem?.ItemIngredient != null)
                        {
                            var itemIngredients = db.ItemIngredient.Where(i => i.id_compoundItem == modelItem.id);
                            foreach (var ingredient in itemIngredients)
                            {
                                db.ItemIngredient.Remove(ingredient);
                                db.Entry(ingredient).State = EntityState.Deleted;
                            }

                            //db.SaveChanges();

                            var ingredients = tempItem.ItemIngredient.ToList();

                            foreach (var ingredient in ingredients)
                            {
                                var tempIngredient = new ItemIngredient
                                {
                                    id_compoundItem = modelItem.id,
                                    id_ingredientItem = ingredient.id_ingredientItem,
                                    id_metricUnit = ingredient.id_metricUnit,
                                    amount = ingredient.amount,
                                    id_metricUnitMax = ingredient.id_metricUnitMax,
                                    amountMax = ingredient.amountMax,
                                    id_costumerItem = ingredient.id_costumerItem
                                };
                                db.ItemIngredient.Add(tempIngredient);
                                db.Entry(tempIngredient).State = EntityState.Added;
                            }
                        }

                        #endregion

                        #region UPDATE TAXATION

                        if (tempItem?.ItemTaxation != null)
                        {
                            var itemTaxations = db.ItemTaxation.Where(i => i.id_item == modelItem.id);
                            foreach (var taxation in itemTaxations)
                            {
                                db.ItemTaxation.Remove(taxation);
                                db.Entry(taxation).State = EntityState.Deleted;
                            }

                            //db.SaveChanges();

                            var taxations = tempItem.ItemTaxation.ToList();

                            foreach (var taxation in taxations)
                            {
                                var tempTaxation = new ItemTaxation
                                {
                                    id_item = modelItem.id,
                                    id_taxType = taxation.id_taxType,
                                    id_rate = taxation.id_rate,
                                    percentage = taxation.percentage
                                };

                                db.ItemTaxation.Add(tempTaxation);
                                ;
                                db.Entry(tempTaxation).State = EntityState.Added;
                            }
                        }

                        #endregion

                        #region UPDATE TARIFF ITEM
                        if (!string.IsNullOrEmpty(codTariffItem) && !string.IsNullOrEmpty(namTariffItem))
                        {
                            TariffItem tariffItem = db.TariffItem.FirstOrDefault(fod => fod.id_Item == modelItem.id);
                            if (tariffItem != null)
                            {
                                //tariffItem.id_Item = modelItem.id;
                                tariffItem.code = codTariffItem;
                                tariffItem.name = namTariffItem;

                                db.TariffItem.Attach(tariffItem);
                                db.Entry(tariffItem).State = EntityState.Modified;
                            }
                            else
                            {
                                TariffItem _tariffItem2 = new TariffItem();
                                _tariffItem2.id_Item = modelItem.id;
                                _tariffItem2.code = codTariffItem;
                                _tariffItem2.name = namTariffItem;

                                db.TariffItem.Attach(_tariffItem2);
                                db.Entry(_tariffItem2).State = EntityState.Added;
                            }
                        }

                        #endregion

                        #region UPDATE ADITIONAL FIELDS

                        if (tempItem?.ItemAditionalField != null)
                        {
                            var itemAditionalFields = db.ItemAditionalField.Where(i => i.id_item == modelItem.id);
                            foreach (var aditionalField in itemAditionalFields)
                            {
                                db.ItemAditionalField.Remove(aditionalField);
                                db.Entry(aditionalField).State = EntityState.Deleted;
                            }

                            //db.SaveChanges();

                            var aditionalFields = tempItem.ItemAditionalField.ToList();

                            foreach (var aditionalField in aditionalFields)
                            {
                                var tempAditionalField = new ItemAditionalField
                                {
                                    id_item = modelItem.id,
                                    label = aditionalField.label,
                                    value = aditionalField.value
                                };
                                db.ItemAditionalField.Add(tempAditionalField);
                                db.Entry(tempAditionalField).State = EntityState.Added;
                            }
                        }

                        #endregion

                        #region UPDATE WEIGHT
                        ItemWeightConversionFreezen itemWeightConversionFreezen = db.ItemWeightConversionFreezen.FirstOrDefault(fod => fod.id_Item == modelItem.id);
                        InventoryLine inventoryLineTmp = db.InventoryLine.FirstOrDefault(fod => fod.id == item.id_inventoryLine);

                        inventoryLineTmp = inventoryLineTmp ?? new InventoryLine();

                        if (inventoryLineTmp.code == "PT")
                        {
                            if (itemWeightConversionFreezen != null)
                            {
                                itemWeightConversionFreezen.id_MetricUnit = (int)idMetUnWeightType;
                                itemWeightConversionFreezen.id_Item = _itemWeightConversionFreezen.id_Item;
                                itemWeightConversionFreezen.itemWeightGrossWeight = _itemWeightConversionFreezen.itemWeightGrossWeight;
                                itemWeightConversionFreezen.itemWeightNetWeight = _itemWeightConversionFreezen.itemWeightNetWeight;
                                itemWeightConversionFreezen.conversionToKilos = _itemWeightConversionFreezen.conversionToKilos == 0 || _itemWeightConversionFreezen.conversionToKilos == null ? 1 : _itemWeightConversionFreezen.conversionToKilos;
                                itemWeightConversionFreezen.conversionToPounds = _itemWeightConversionFreezen.conversionToPounds == 0 || _itemWeightConversionFreezen.conversionToPounds == null ? 1 : _itemWeightConversionFreezen.conversionToPounds;
                                itemWeightConversionFreezen.weightWithGlaze = _itemWeightConversionFreezen.weightWithGlaze;
                                itemWeightConversionFreezen.glazePercentage = _itemWeightConversionFreezen.glazePercentage;

                                db.ItemWeightConversionFreezen.Attach(itemWeightConversionFreezen);
                                db.Entry(itemWeightConversionFreezen).State = EntityState.Modified;
                            }
                            else
                            {
                                if (_itemWeightConversionFreezen != null && _itemWeightConversionFreezen != null)
                                {
                                    _itemWeightConversionFreezen.id_Item = item.id;
                                    _itemWeightConversionFreezen.id_MetricUnit = (int)idMetUnWeightType;

                                    item.ItemWeightConversionFreezen = _itemWeightConversionFreezen;
                                    db.ItemWeightConversionFreezen.Attach(_itemWeightConversionFreezen);
                                    db.Entry(_itemWeightConversionFreezen).State = EntityState.Added;
                                }

                            }
                        }
                        #endregion

                        #region UPDATE TECHNICAL SPECIFICATIONS
                        List<ItemDocument> _itemDocumenLst = tempItem.ItemDocument.ToList();
                        if (_itemDocumenLst != null)
                        {
                            var itemItemDocument = _itemDocumenLst.ToList();

                            for (int i = modelItem.ItemDocument.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.ItemDocument.ElementAt(i);

                                if (itemItemDocument.FirstOrDefault(fod => fod.id == detail.id) == null)
                                {
                                    TechnicalSpecificationsDeleteAttachment(detail);
                                    modelItem.ItemDocument.Remove(detail);
                                    db.Entry(detail).State = EntityState.Deleted;
                                }

                            }

                            foreach (var detail in itemItemDocument)
                            {
                                var tempItemItemDocument = modelItem.ItemDocument.FirstOrDefault(fod => fod.id == detail.id);
                                if (tempItemItemDocument == null)
                                {
                                    tempItemItemDocument = new ItemDocument
                                    {
                                        guid = detail.guid,
                                        url = detail.url,
                                        attachment = detail.attachment,
                                        referenceDocument = detail.referenceDocument,
                                        descriptionDocument = detail.descriptionDocument
                                    };
                                    modelItem.ItemDocument.Add(tempItemItemDocument);
                                }
                                else
                                {
                                    if (tempItemItemDocument.url != detail.url)
                                    {
                                        TechnicalSpecificationsDeleteAttachment(tempItemItemDocument);
                                        tempItemItemDocument.guid = detail.guid;
                                        tempItemItemDocument.url = detail.url;
                                        tempItemItemDocument.attachment = detail.attachment;
                                    }
                                    tempItemItemDocument.referenceDocument = detail.referenceDocument;
                                    tempItemItemDocument.descriptionDocument = detail.descriptionDocument;
                                    db.Entry(tempItemItemDocument).State = EntityState.Modified;
                                }

                            }
                        }
                        #endregion

                        #region Item Equivalence
                        var _il = db.InventoryLine
                                    .FirstOrDefault(fod => fod.id == item.id_inventoryLine);
                        if (_il.code.Equals("PP") || _il.code.Equals("PT"))
                        {
                            if (idItemEquivalence > 0)
                            {
                                modelItem.ItemEquivalence = modelItem.ItemEquivalence ?? new ItemEquivalence();
                                modelItem.ItemEquivalence.id_itemEquivalence = idItemEquivalence;
                            }
                        }
                        #endregion

                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.isActive = item.isActive;

                        #region"Calculo Automático de Código Auxiliar"
                        if (valSet == "YES")
                        {
                            if (inventoryLineTmp.code == "PT" || inventoryLineTmp.code == "PP")
                            {
                                var strCodeAux = "";
                                //Tipo de Producto
                                strCodeAux = GetAuxiliarCodeForProduct(modelItem);

                                if (strCodeAux != "ERROR" && strCodeAux.Length > 8)
                                {
                                    modelItem.auxCode = strCodeAux;
                                }
                            }
                        }
                        #endregion

                        db.Item.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;

                        MigrationItem migrationItem = db.MigrationItem.FirstOrDefault(fod => fod.id_item == modelItem.id);
                        if (migrationItem == null && inventoryLineTmp.code == "PT")
                        {
                            migrationItem = new MigrationItem
                            {
                                id_item = modelItem.id,
                                id_userCreate = ActiveUser.id,
                                dateCreate = DateTime.Now
                            };
                            db.MigrationItem.Add(migrationItem);
                        }

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Producto: " + item.name + " guardado exitosamente");
                        if (toReturn ?? false)
                        {
                            ViewData["ModelLink"] = modelItem;
                        }


                        isValid = true;
                        this.ViewBag.EditMessage = "Producto: " + item.name + " guardado exitosamente";
                        message = "Producto: " + item.name + " guardado exitosamente";
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                    this.ViewBag.EditError = e.GetBaseException().Message;
                    message = e.GetBaseException().Message;
                    ViewData["EditMessage"] = ErrorMessage();
                }
            }

            var result = new
            {
                idItem = item.id,
                isValid,
                message,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CopyEditItem(int id)
        {
            string valueSetting = DataProviderSetting.ValueSetting("PGCMLI");
            var model = db.Item
                .Include("InventoryLine")
                .Include("ItemHeadIngredient")
                .Include("ItemIngredient")
                .Include("ItemIngredient1")
                .Include("ItemAccount")
                .Include("ItemAditionalField")
                .Include("ItemDocument")
                .Include("ItemGeneral")
                .Include("ItemInventory")
                .Include("ItemWeightConversionFreezen")
                .Include("ItemProcessType")
                .Include("ItemProvider")
                .Include("ItemTaxation")
                .Include("ItemTechnicalSpecificationsAttachment")
                .Include("MigrationItem")
                .Include("Presentation1")
                .Include("Presentation2")
                .Include("ConversionItemPurchasePlanning")
                .Include("ConversionItemPurchasePlanning1")
                .Include("ProviderItem")
                .Include("ItemWarehouse")
                .Include("ShrimpSupplierTraceability")
                .Include("ShrimpSupplierTraceability1")
                .Include("ShrimpSupplierTraceability2")
                .Include("ShrimpSupplierTraceability3")
                .Include("ShrimpSupplierTraceability4")
                .FirstOrDefault(i => i.id == id);

            // Instanciamos un nuevo Item
            var masterCode = model.masterCode;
            model.id = 0;
            model.masterCode = masterCode.Substring(0, 2);
            if (valueSetting == "NO")
            {
                string itemMasterCode = BuildCopiedMasterCode(model.id_inventoryLine);
                model.masterCode = itemMasterCode;
            }
            TempData["item"] = model;
            TempData.Keep("item");

            return PartialView("_EditForm", model);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult ClearStructureItem()
        {
            TempData.Remove("item");
            var result = new
            {
                Message = "Ok"
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemsPartialDelete(System.Int32 id)
        {
            var model = db.Item;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)
                    {
                        item.isActive = false;

                        // CAMPOS DE AUDITORIA
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.Entry(item).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_ItemsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedItems(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var items = db.Item.Where(i => ids.Contains(i.id));
                        foreach (var item in items)
                        {
                            item.isActive = false;

                            // CAMPOS DE AUDITORIA
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                    }
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemTypesByInventoryLine(int? id_inventoryLine)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var model = db.ItemType.Where(t => t.id_inventoryLine == id_inventoryLine).Select(t => new { t.id, t.name }).ToList();
            TempData.Keep("item");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemTypesCategoriesByItemType(int? id_itemType)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var model = db.ItemTypeCategory.Where(c => c.ItemTypeItemTypeCategory.Any(a => a.id_itemType == id_itemType)).Select(c => new { c.id, c.name }).ToList();
            TempData.Keep("item");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItemsByItemTypeAndItemTypeCategory(int? id_itemType, int? id_itemTypeCategory, int? id_ingredientItemAux)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            Item item = (TempData["item"] as Item);

            item = item ?? new Item();

            var model = db.Item.Where(t => t.id_itemType == id_itemType && t.id_itemTypeCategory == id_itemTypeCategory && t.isActive == true).ToList();

            var tempModel = new List<Item>();
            foreach (var m in model)
            {
                if (id_ingredientItemAux == m.id || item.ItemIngredient.FirstOrDefault(fod => fod.id_ingredientItem == m.id) == null)
                {
                    tempModel.Add(m);
                }
            }

            model = tempModel;

            var modelAux = model.Select(t => new { t.id, t.name }).ToList();

            TempData.Keep("item");

            return Json(modelAux, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult MetricUnitsByItem(int? id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var id_metricTypeAux = db.Item.FirstOrDefault(fod => fod.id == id_item)?.id_metricType;
            var model = db.MetricUnit.Where(t => t.id_metricType == id_metricTypeAux).Select(t => new { t.id, t.code }).ToList();
            TempData.Keep("item");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult InventoryLineIngredientItemInit(int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory, int? id_ingredientItem, int? id_metricUnit, int? id_metricUnitMax)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //inventoryLines
            var inventoryLines = db.InventoryLine.Where(t => t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_inventoryLine)).Select(t => new { t.id, t.name }).ToList();

            //itemTypes
            var itemTypes = db.ItemType.Where(t => t.id_inventoryLine == id_inventoryLine && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_itemType)).Select(t => new { t.id, t.name }).ToList();

            //itemTypeCategories
            var itemTypeCategories = db.ItemTypeCategory.Where(c => c.ItemTypeItemTypeCategory.Any(a => a.id_itemType == id_itemType)).Select(c => new { c.id, c.name }).ToList();

            //ingredientItems
            var ingredientItems = db.Item.Where(t => t.id_itemType == id_itemType && t.id_itemTypeCategory == id_itemTypeCategory && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_ingredientItem)).Select(t => new { t.id, t.name }).ToList();

            //metricUnits
            var id_metricTypeAux = db.Item.FirstOrDefault(fod => fod.id == id_ingredientItem)?.id_metricType;
            var metricUnits = db.MetricUnit.Where(t => t.id_metricType == id_metricTypeAux && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_metricUnit)).Select(t => new { t.id, t.code }).ToList();

            //metricUnitsMax
            var metricUnitsMax = db.MetricUnit.Where(t => t.id_metricType == id_metricTypeAux && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_metricUnitMax)).Select(t => new { t.id, t.code }).ToList();

            var result = new
            {
                inventoryLines = inventoryLines,
                itemTypes = itemTypes,
                itemTypeCategories = itemTypeCategories,
                ingredientItems = ingredientItems,
                metricUnits = metricUnits,
                metricUnitsMax = metricUnitsMax
            };

            TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuildMasterCode(int? id_inventoryLine)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            bool readOnly = true;
            string itemMasterCode = string.Empty;

            /*
			 *  Cambios 2021 07 20
			 *  Condicionamiento de Generacion de Codigo Master por Linea de Inventario en Configuracion
			 *
			 */
            //PGCMLI
            string[] congLineasInvCods = new string[] { };
            var settLineaInventario = db.Setting.FirstOrDefault(r => r.code == "PGCMLI");
            if (settLineaInventario != null)
            {
                congLineasInvCods = settLineaInventario
                                                .SettingDetail
                                                .Select(r => r.value)
                                                .ToArray();
            }

            InventoryLine inventoryLine = db.InventoryLine.FirstOrDefault(i => i.id == id_inventoryLine);
            if (inventoryLine != null)
            {
                int codeLength = 6;
                if (congLineasInvCods.Contains(inventoryLine.code))
                {
                    itemMasterCode = inventoryLine.code + "-";
                    readOnly = false;
                }
                else
                {
                    itemMasterCode = inventoryLine.code + inventoryLine.sequential.ToString().PadLeft(codeLength, '0');
                }

            }
            var code_inventoryLine = db.InventoryLine.FirstOrDefault(fod => fod.id == id_inventoryLine)?.code ?? "";
            MetricType metricTypeUNI01 = null;
            MetricUnit metricUnitUn = null;
            if (code_inventoryLine == "PT")
            {
                metricTypeUNI01 = db.MetricType.FirstOrDefault(fod => fod.code == "UNI01");
                if (metricTypeUNI01 == null)
                {
                    var error = new
                    {
                        itemMasterCode = itemMasterCode,
                        code_inventoryLine = "",
                        id_metricType = (int?)null,
                        id_metricUni = (int?)null,
                        readOnly = readOnly,
                        Message = "No Existe el Tipo de Unidad de Medida: (Unidades), que el código que se espera sea: (UNI01). Configúrelo e intente de nuevo",
                    };
                    TempData.Keep("item");
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
                metricUnitUn = db.MetricUnit.FirstOrDefault(fod => fod.code == "Un");
                if (metricUnitUn == null)
                {
                    var error = new
                    {
                        itemMasterCode = itemMasterCode,
                        code_inventoryLine = "",
                        id_metricType = (int?)metricTypeUNI01?.id,
                        id_metricUni = (int?)null,
                        readOnly = readOnly,
                        Message = "No Existe la Unidad de Medida: (Unidades), que el código que se espera sea: (Un). Configúrelo e intente de nuevo",
                    };
                    TempData.Keep("item");
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
            }
            var result = new
            {
                itemMasterCode = itemMasterCode,
                code_inventoryLine = code_inventoryLine,
                id_metricType = (int?)metricTypeUNI01?.id,
                id_metricUni = (int?)metricUnitUn?.id,
                readOnly = readOnly,
                Message = "Ok"
            };
            TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMetricUnitCode(int? id_metricUnit)
        {
            string metricUnitCode = string.Empty;

            metricUnitCode = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnit)?.code ?? string.Empty;

            var result = new
            {
                metricUnitCode = metricUnitCode,
                Message = "Ok"
            };
            TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BinaryImageColumnPhotoUpdate()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            return BinaryImageEditExtension.GetCallbackResult();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateInventoryItemWarehouseLocation(int? id_warehouse, int? id_warehouseLocation = null)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            var result = new
            {
                warehouseLocations = db.WarehouseLocation.Where(w => (w.id_warehouse == id_warehouse && w.isActive) || w.id == id_warehouseLocation)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       })

            };

            TempData.Keep("item");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult GetMetricUnit(int? id_metricType)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            MetricType metricType = db.MetricType.FirstOrDefault(t => t.id == id_metricType && t.isActive && t.id_company == this.ActiveCompanyId);

            var metricUnitAux = metricType?.MetricUnit?.ToList() ?? new List<MetricUnit>();
            metricUnitAux = metricUnitAux.Where(g => (g.isActive && g.id_company == this.ActiveCompanyId)).ToList();

            var result = new
            {
                metricUnits = metricUnitAux.Select(s => new
                {
                    id = s.id,
                    name = s.name
                })

            };

            TempData.Keep("item");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateFormulationWithItemPresentation(int? id_presentationOld, int? id_presentationNew)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? new Item();

            item.ItemIngredient = item.ItemIngredient?.ToList() ?? new List<ItemIngredient>();
            //SIN CLIENTE
            var aPerson = db.Person.FirstOrDefault(p => p.isActive && p.fullname_businessName == "SIN CLIENTE");
            var costumerItemDefault = aPerson?.id;

            var presentationOld = db.Presentation.FirstOrDefault(fod => fod.id == id_presentationOld);
            if (presentationOld != null)
            {
                var itemIngredientAux = item.ItemIngredient.FirstOrDefault(fod => fod.id_ingredientItem == presentationOld.id_itemPackingMinimum && (fod.id_costumerItem == null || fod.id_costumerItem == costumerItemDefault));
                if (itemIngredientAux != null)
                {
                    item.ItemIngredient.Remove(itemIngredientAux);
                }
                itemIngredientAux = item.ItemIngredient.FirstOrDefault(fod => fod.id_ingredientItem == presentationOld.id_itemPackingMaximum && (fod.id_costumerItem == null || fod.id_costumerItem == costumerItemDefault));
                if (itemIngredientAux != null)
                {
                    item.ItemIngredient.Remove(itemIngredientAux);
                }
            }

            var presentationNew = db.Presentation.FirstOrDefault(fod => fod.id == id_presentationNew);
            if (presentationNew != null)
            {
                var metricUnitUn = db.MetricUnit.FirstOrDefault(fod => fod.code == "Un");
                if (metricUnitUn == null)
                {
                    var error = new
                    {
                        Message = "No Existe la Unidad de Medida: (Unidades), que el código que se espera sea: (Un). Configúrelo e intente de nuevo",
                    };
                    TempData.Keep("item");
                    return Json(error, JsonRequestBehavior.AllowGet);
                }
                ItemIngredient itemIngredientAux;
                if (presentationNew.id_itemPackingMinimum != null)
                {
                    itemIngredientAux = new ItemIngredient
                    {
                        id = item.ItemIngredient.Count() > 0 ? item.ItemIngredient.Max(pld => pld.id) + 1 : 1,
                        id_ingredientItem = presentationNew.id_itemPackingMinimum.Value,
                        amount = 1,
                        id_metricUnit = metricUnitUn.id,
                        amountMax = presentationNew.maximum,
                        id_metricUnitMax = metricUnitUn.id,
                        manual = false
                    };
                    item.ItemIngredient.Add(itemIngredientAux);
                }


                itemIngredientAux = new ItemIngredient
                {
                    id_ingredientItem = presentationNew.id_itemPackingMaximum,
                    amountMax = 1,
                    id_metricUnitMax = metricUnitUn.id,
                    manual = false
                };
                item.ItemIngredient.Add(itemIngredientAux);
            }

            var result = new
            {
                Message = "Ok",
            };
            TempData["item"] = item;
            TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult TaxTypesWithCurrent(int? id_taxTypeCurrent)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? new Item();

            var model = db.TaxType.Where(t => (t.id_company == this.ActiveCompanyId && t.isActive) || t.id == id_taxTypeCurrent).ToList();

            var tempModel = new List<TaxType>();
            foreach (var m in model)
            {
                if (id_taxTypeCurrent == m.id || item.ItemTaxation.FirstOrDefault(fod => fod.id_taxType == m.id) == null)
                {
                    tempModel.Add(m);
                }
            }

            model = tempModel;

            var modelAux = model.Select(t => new { t.id, t.name }).ToList();

            TempData.Keep("item");

            return Json(modelAux, JsonRequestBehavior.AllowGet);
        }

        #region GENERAL

        public ActionResult ComboBoxItemGroupPartial(/*Item item*/)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //item = (TempData["item"] as Item) ?? item;

            //item.ItemGeneral = item.ItemGeneral ?? new ItemGeneral();

            //TempData["item"] = item;
            TempData.Keep("item");
            return PartialView("_ComboBoxItemGroupPartial", DataProviderItemGroup.ItemGroups((int)ViewData["id_company"]));
        }

        public ActionResult ComboBoxItemSubGroupPartial(/*Item item, */int? id_group, int? id_subgroup, int? id_groupCategory)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            //item = (TempData["item"] as Item) ?? item;

            //item.ItemGeneral = item.ItemGeneral ?? new ItemGeneral();

            ////int? id_group = (Request.Params["id_group"] != null && Request.Params["id_group"] != "") ? int.Parse(Request.Params["id_group"]) : (int?) null;
            //item.ItemGeneral.id_group = id_group;
            //item.ItemGeneral.id_subgroup = id_subgroup;
            //item.ItemGeneral.id_groupCategory = id_groupCategory;

            //TempData["item"] = item;
            TempData.Keep("item");

            return PartialView("_ComboBoxItemSubGroupPartial", DataProviderItemGroup.ItemSubGroupsOfGroup(id_group));
        }

        public ActionResult ComboBoxItemGroupCategoryPartial(/*Item item, */int? id_group, int? id_subgroup, int? id_groupCategory)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            //item = (TempData["item"] as Item) ?? item;

            //item.ItemGeneral = item.ItemGeneral ?? new ItemGeneral();

            ////int? id_subgroup = (Request.Params["id_subgroup"] != null && Request.Params["id_subgroup"] != "") ? int.Parse(Request.Params["id_subgroup"]) : (int?) null;
            //item.ItemGeneral.id_group = id_group;
            //item.ItemGeneral.id_subgroup = id_subgroup;
            //item.ItemGeneral.id_groupCategory = id_groupCategory;
            //TempData["item"] = item;
            TempData.Keep("item");
            int? id_groupAux = id_subgroup ?? id_group;
            return PartialView("_ComboBoxItemGroupCategoryPartial", DataProviderItemGroupCategory.ItemCategories());
        }

        [HttpPost]
        public void ComboBoxItemGroupCategory_SelectedIndexChanged(/*Item item, */int? id_group, int? id_subgroup, int? id_groupCategory)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            //item = (TempData["item"] as Item) ?? item;

            //item.ItemGeneral = item.ItemGeneral ?? new ItemGeneral();

            //item.ItemGeneral.id_group = id_group;
            //item.ItemGeneral.id_subgroup = id_subgroup;
            //item.ItemGeneral.id_groupCategory = id_groupCategory;

            //TempData["item"] = item;
            TempData.Keep("item");
        }

        #endregion

        #region FORMULATION

        [ValidateInput(false)]
        public ActionResult Formulation(int itemId, int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == itemId);
            item = item ?? new Item();

            var model = item?.ItemIngredient.ToList() ?? new List<ItemIngredient>();

            TempData["item"] = TempData["item"] ?? item;
            TempData.Keep("item");

            var idCliente = model.FirstOrDefault(fod => fod.id == id)?.id_costumerItem;
            var aPerson = db.Person.FirstOrDefault(p => p.isActive && p.fullname_businessName == "SIN CLIENTE");
            var itemDefault = aPerson?.id;
            this.ViewBag.IdCliente = idCliente.HasValue
                ? idCliente.Value
                : itemDefault;

            return PartialView("_Formulation", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormulationAddNew(int itemId, ItemIngredient ingredient)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == itemId);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    ingredient.id = item.ItemIngredient.Count() > 0 ? item.ItemIngredient.Max(pld => pld.id) + 1 : 1;
                    //ingredient.id_metricUnit = id_metricUnitIngredientItem;
                    //ingredient.MetricUnit = db.MetricUnit.FirstOrDefault(fod=> fod.id == id_metricUnitIngredientItem);
                    //ingredient.id_metricUnitMax = id_metricUnitMaxIngredientItem;
                    //ingredient.MetricUnit1 = db.MetricUnit.FirstOrDefault(fod => fod.id == id_metricUnitMaxIngredientItem);
                    ingredient.manual = true;
                    item.ItemIngredient.Add(ingredient);
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("item");

            var model = item?.ItemIngredient.ToList() ?? new List<ItemIngredient>();
            return PartialView("_Formulation", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormulationUpdate(int itemId, ItemIngredient ingredient)//int id_ingredientItem, int id_metricUnit, decimal amount)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == itemId);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    //var modelItem = item.ItemIngredient.FirstOrDefault(i => i.id_ingredientItem == ingredient.id_ingredientItem);
                    var modelItem = item.ItemIngredient.FirstOrDefault(i => i.id == ingredient.id);
                    if (modelItem != null)
                    {
                        modelItem.id_ingredientItem = ingredient.id_ingredientItem;
                        modelItem.Item1 = db.Item.FirstOrDefault(i => i.id == ingredient.id_ingredientItem);
                        modelItem.id_metricUnit = ingredient.id_metricUnit;
                        modelItem.MetricUnit = db.MetricUnit.FirstOrDefault(fod => fod.id == ingredient.id_metricUnit);
                        modelItem.id_metricUnitMax = ingredient.id_metricUnitMax;
                        modelItem.MetricUnit1 = db.MetricUnit.FirstOrDefault(fod => fod.id == ingredient.id_metricUnitMax);
                        modelItem.amount = ingredient.amount;
                        modelItem.amountMax = ingredient.amountMax;
                        modelItem.manual = true;
                        modelItem.id_costumerItem = ingredient.id_costumerItem;
                        modelItem.Person = db.Person.FirstOrDefault(fod => fod.id == ingredient.id_costumerItem);
                        //this.UpdateModel(modelItem);
                    }
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("item");

            var model = item?.ItemIngredient.ToList() ?? new List<ItemIngredient>();
            return PartialView("_Formulation", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FormulationDelete(int itemId, int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == itemId);
            item = item ?? new Item();

            if (id >= 0 && item != null)
            {
                try
                {
                    using (DbContextTransaction trans = db.Database.BeginTransaction())
                    {
                        var ingredient = item.ItemIngredient.FirstOrDefault(it => it.id == id);
                        if (ingredient != null)
                        {
                            //if (!ingredient.manual)
                            //{
                            //	throw (new Exception("Este Ítem de La formulación no se puede eliminar debido a que fue cargado desde la presentación del producto"));
                            //}

                            item.ItemIngredient.Remove(ingredient);
                            //db.SaveChanges();
                            //trans.Commit();
                        }
                    }

                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("item");

            var model = item?.ItemIngredient.ToList() ?? new List<ItemIngredient>();
            return PartialView("_Formulation", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadInventoryLineIngredientItem(int? id_inventoryLineIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxInventoryLineIngredientItemProperties(id_inventoryLineIngredientItem);
            TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxInventoryLineIngredientItemProperties(int? id_inventoryLineIngredientItem)
        {
            //inventoryLines
            var inventoryLines = db.InventoryLine.Where(t => t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_inventoryLineIngredientItem)).Select(t => new { t.id, t.name }).ToList();

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_inventoryLineIngredientItem";
            p.ValueField = "id";
            p.TextField = "name";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "Item", Action = "LoadInventoryLineIngredientItem" };
            p.ClientSideEvents.BeginCallback = "InventoryLineIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "InventoryLineIngredientItem_EndCallback";
            p.ClientSideEvents.SelectedIndexChanged = "ComboInventoryLineIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnInventoryLineIngredientItemValidation";
            p.ClientSideEvents.Init = "OnInventoryLineIngredientItemInit";

            p.BindList(inventoryLines);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemTypeIngredientItem(int? id_inventoryLineIngredientItem, int? id_itemTypeIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxItemTypeIngredientItemProperties(id_inventoryLineIngredientItem, id_itemTypeIngredientItem);
            TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxItemTypeIngredientItemProperties(int? id_inventoryLineIngredientItem, int? id_itemTypeIngredientItem)
        {
            //itemTypes
            var itemTypes = db.ItemType.Where(t => t.id_inventoryLine == id_inventoryLineIngredientItem && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_itemTypeIngredientItem)).Select(t => new { t.id, t.name }).ToList();


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_itemTypeIngredientItem";
            p.ValueField = "id";
            p.TextField = "name";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "Item", Action = "LoadItemTypeIngredientItem" };
            p.ClientSideEvents.BeginCallback = "ItemTypeIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "ItemTypeIngredientItem_EndCallback";
            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboItemTypeIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnItemTypeIngredientItemValidation";

            p.BindList(itemTypes);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadItemTypeCategoryIngredientItem(int? id_itemTypeIngredientItem, int? id_itemTypeCategoryIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxItemTypeCategoryIngredientItemProperties(id_itemTypeIngredientItem, id_itemTypeCategoryIngredientItem);
            TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxItemTypeCategoryIngredientItemProperties(int? id_itemTypeIngredientItem, int? id_itemTypeCategoryIngredientItem)
        {
            //itemTypeCategories
            var itemTypeCategories = db.ItemTypeCategory.Where(c => c.ItemTypeItemTypeCategory.Any(a => a.id_itemType == id_itemTypeIngredientItem || c.id == id_itemTypeCategoryIngredientItem)).Select(c => new { c.id, c.name }).ToList();


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_itemTypeCategoryIngredientItem";
            p.ValueField = "id";
            p.TextField = "name";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "Item", Action = "LoadItemTypeCategoryIngredientItem" };
            p.ClientSideEvents.BeginCallback = "ItemTypeCategoryIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "ItemTypeCategoryIngredientItem_EndCallback";
            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboItemTypeCategoryIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnItemTypeCategoryIngredientItemValidation";

            p.BindList(itemTypeCategories);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadIngredientItem(int? id_inventoryLineIngredientItem, int? id_itemTypeIngredientItem, int? id_itemTypeCategoryIngredientItem, int? id_ingredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxIngredientItemProperties(id_inventoryLineIngredientItem, id_itemTypeIngredientItem, id_itemTypeCategoryIngredientItem, id_ingredientItem);
            TempData.Keep("item");
            //return GridViewExtension.GetComboBoxCallbackResult(p => {
            //    //p.ClientInstanceName = "id_ingredientItem";
            //    p.ValueField = "id";
            //    p.TextField = "name";
            //    p.ValueType = typeof(int);
            //    //p.CallbackPageSize = 10;
            //    //p.Width = Unit.Percentage(100);
            //    p.BindList(db.Item.Where(t => t.id_itemType == id_itemTypeIngredientItem && t.id_itemTypeCategory == id_itemTypeCategoryIngredientItem && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_ingredientItem)).Select(t => new { t.id, t.name }).ToList());
            //});
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxIngredientItemProperties(int? id_inventoryLineIngredientItem, int? id_itemTypeIngredientItem, int? id_itemTypeCategoryIngredientItem, int? id_ingredientItem)
        {
            //ingredientItems
            var ingredientItems = db.Item.Where(t => t.id_inventoryLine == id_inventoryLineIngredientItem &&
                                                     t.id_itemType == id_itemTypeIngredientItem &&
                                                     t.id_itemTypeCategory == id_itemTypeCategoryIngredientItem &&
                                                     t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_ingredientItem)).Select(t => new { t.id, t.name, t.auxCode, t.masterCode }).ToList();


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_ingredientItem";
            p.ValueField = "id";
            //p.TextField = "name";
            p.TextFormatString = "{0} | {1} | {2}";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            p.Columns.Add("masterCode", "Código", 25);//, Unit.Percentage(50));
            p.Columns.Add("auxCode", "Código Aux.", 35);
            p.Columns.Add("name", "Ingrediente", 70);//, Unit.Percentage(70));
            

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "Item", Action = "LoadIngredientItem" };
            p.ClientSideEvents.BeginCallback = "IngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "IngredientItem_EndCallback";
            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboItemIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnItemIngredientItemValidation";
            p.ClientSideEvents.Init = "OnItemIngredientItemInit";

            p.BindList(ingredientItems);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadMetricUnitIngredientItem(int? id_ingredientItem, int? id_metricUnitIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxMetricUnitIngredientItemProperties(id_ingredientItem, id_metricUnitIngredientItem);
            TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxMetricUnitIngredientItemProperties(int? id_ingredientItem, int? id_metricUnitIngredientItem)
        {
            //metricUnits
            var id_metricTypeAux = db.Item.FirstOrDefault(fod => fod.id == id_ingredientItem)?.id_metricType;
            var metricUnits = db.MetricUnit.Where(t => t.id_metricType == id_metricTypeAux && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_metricUnitIngredientItem)).Select(t => new { t.id, t.code }).ToList();


            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_metricUnitIngredientItem";
            p.ValueField = "id";
            p.TextField = "code";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "Item", Action = "LoadMetricUnitIngredientItem" };
            p.ClientSideEvents.BeginCallback = "MetricUnitIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "MetricUnitIngredientItem_EndCallback";
            //p.EnableSynchronization = DefaultBoolean.False;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboMetricUnitIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnMetricUnitIngredientItemValidation";

            p.BindList(metricUnits);
            return p;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LoadMetricUnitMaxIngredientItem(int? id_ingredientItem, int? id_metricUnitMaxIngredientItem)
        {

            MVCxColumnComboBoxProperties p = CreateComboBoxMetricUnitMaxIngredientItemProperties(id_ingredientItem, id_metricUnitMaxIngredientItem);
            TempData.Keep("item");
            return GridViewExtension.GetComboBoxCallbackResult(p);

        }

        private MVCxColumnComboBoxProperties CreateComboBoxMetricUnitMaxIngredientItemProperties(int? id_ingredientItem, int? id_metricUnitMaxIngredientItem)
        {
            //metricUnitsMax
            var id_metricTypeAux = db.Item.FirstOrDefault(fod => fod.id == id_ingredientItem)?.id_metricType;
            var metricUnitsMax = db.MetricUnit.Where(t => t.id_metricType == id_metricTypeAux && t.id_company == this.ActiveCompanyId && (t.isActive || t.id == id_metricUnitMaxIngredientItem)).Select(t => new { t.id, t.code }).ToList();

            MVCxColumnComboBoxProperties p = new MVCxColumnComboBoxProperties();
            p.ClientInstanceName = "id_metricUnitMaxIngredientItem";
            p.ValueField = "id";
            p.TextField = "code";
            p.ValueType = typeof(int);
            p.CallbackPageSize = 10;
            p.Width = Unit.Percentage(100);
            p.DropDownStyle = DropDownStyle.DropDownList;
            p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
            p.CallbackRouteValues = new { Controller = "Item", Action = "LoadMetricUnitMaxIngredientItem" };
            p.ClientSideEvents.BeginCallback = "MetricUnitMaxIngredientItem_BeginCallback";
            p.ClientSideEvents.EndCallback = "MetricUnitMaxIngredientItem_EndCallback";
            //p.EnableSynchronization = DefaultBoolean.False;
            p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

            //p.ShowModelErrors = true;
            p.ClientSideEvents.SelectedIndexChanged = "ComboMetricUnitMaxIngredientItem_SelectedIndexChanged";
            p.ClientSideEvents.Validation = "OnMetricUnitMaxIngredientItemValidation";

            p.BindList(metricUnitsMax);
            return p;

        }

        public JsonResult GetValueMetricUnitIngredientItem(int? id_ingredientItem)
        {
            TempData.Keep("item");
            var result = new
            {
                id_metricUnitIngredientItem = (int?)null

            };
            var id_metricAux = db.Item.FirstOrDefault(fod => fod.id == id_ingredientItem)?.ItemInventory?.id_metricUnitInventory;
            result = new
            {
                id_metricUnitIngredientItem = id_metricAux

            };

            //TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region TAXATION

        [ValidateInput(false)]
        public ActionResult Taxation(int id_item)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id_item);
            item = item ?? new Item();

            var model = item?.ItemTaxation.ToList() ?? new List<ItemTaxation>();
            TempData["item"] = TempData["item"] ?? item;
            TempData.Keep("item");
            return PartialView("_Taxation", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaxationAddNew(int id_item, ItemTaxation itemTaxation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id_item);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    itemTaxation.id = item.ItemTaxation.Count() > 0 ? item.ItemTaxation.Max(pld => pld.id) + 1 : 1;
                    item.ItemTaxation.Add(itemTaxation);
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("item");

            var model = item?.ItemTaxation.ToList() ?? new List<ItemTaxation>();
            return PartialView("_Taxation", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaxationUpdate(/*int id_item, int id, int id_taxType, int id_rate, decimal percentage, */ItemTaxation itemTaxation)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == itemTaxation.id_item);
            item = item ?? new Item();

            if (ModelState.IsValid && item != null)
            {
                try
                {
                    var modelItem = item.ItemTaxation.FirstOrDefault(i => i.id == itemTaxation.id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                    }
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("item");

            var model = item?.ItemTaxation.ToList() ?? new List<ItemTaxation>();
            return PartialView("_Taxation", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaxationDelete(int id_item, int id)//, System.Int32 id_rate)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id_item);
            item = item ?? new Item();

            //if (id_rate >= 0)
            //{
            try
            {
                var taxation = item.ItemTaxation.FirstOrDefault(it => it.id == id);
                if (taxation != null)
                    item.ItemTaxation.Remove(taxation);

                TempData["item"] = item;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //}

            TempData.Keep("item");

            var model = item?.ItemTaxation.ToList() ?? new List<ItemTaxation>();
            return PartialView("_Taxation", model);
        }

        public ActionResult ComboBoxTaxationRatePartial(int? id_taxType)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            TempData.Keep("item");

            return PartialView("_ComboBoxItemTaxationRatePartial", DataProviderRate.RatesByTaxType(id_taxType));
        }

        [HttpPost]
        public JsonResult GetRatesByTaxType(int? id_taxType, int? id_rateCurrent)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            //DataProviderRate.RatesByCompanyTaxTypeAndCurrent(this.ActiveCompanyId, id_taxType, id_rateCurrent);
            var ratesAux = db.Rate.Where(t => (t.isActive && t.id_company == this.ActiveCompanyId && t.id_taxType == id_taxType) ||
                                                 t.id == id_rateCurrent).ToList();
            var result = new
            {
                rates = ratesAux.Select(s => new
                {
                    id = s.id,
                    name = s.name
                    //percentage = s.percentage
                })
            };
            TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPercentage(int? id_rate)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            decimal? percentage = db.Rate.FirstOrDefault(i => i.id == id_rate)?.percentage;

            var result = new
            {
                percentage = percentage
            };
            TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ADITIONAL FIELDS

        [ValidateInput(false)]
        public ActionResult AditionalField(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id);
            item = item ?? new Item();

            var model = item?.ItemAditionalField.ToList() ?? new List<ItemAditionalField>();
            TempData["item"] = TempData["item"] ?? item;
            TempData.Keep("item");
            return PartialView("_AditionalField", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AditionalFieldAddNew(int id, string label, string value)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    item.ItemAditionalField.Add(new ItemAditionalField
                    {
                        label = label,
                        value = value
                    });
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("item");

            var model = item?.ItemAditionalField.ToList() ?? new List<ItemAditionalField>();
            return PartialView("_AditionalField", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AditionalFieldUpdate(int id, ItemAditionalField aditionalField)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = item.ItemAditionalField.FirstOrDefault(i => i.label.Equals(aditionalField.label));
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                    }
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("item");

            var model = item?.ItemAditionalField.ToList() ?? new List<ItemAditionalField>();
            return PartialView("_AditionalField", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AditionalFieldDelete(int id, System.Int32 id_aditionalField)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id);
            item = item ?? new Item();

            if (id_aditionalField >= 0)
            {
                try
                {
                    var aditionalField = item.ItemAditionalField.FirstOrDefault(f => f.id_aditionalField == id_aditionalField);
                    if (aditionalField != null)
                        item.ItemAditionalField.Remove(aditionalField);

                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("item");

            var model = item?.ItemAditionalField.ToList() ?? new List<ItemAditionalField>();
            return PartialView("_AditionalField", model);
        }

        #endregion

        #region PROVIDERS

        [ValidateInput(false)]
        public ActionResult Providers(int id)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id);
            item = item ?? new Item();

            item.ItemProvider = item.ItemProvider ?? new List<ItemProvider>();

            var model = item.ItemProvider.ToList();

            TempData["item"] = TempData["item"] ?? item;
            TempData.Keep("item");

            this.UpdateModel(item);
            return PartialView("_Providers", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderAddNew(int id, int id_provider, bool isFavorite)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    item.ItemProvider.Add(new ItemProvider
                    {
                        id_provider = id_provider,
                        isFavorite = isFavorite
                    });

                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("item");

            var model = item?.ItemProvider.ToList() ?? new List<ItemProvider>();
            return PartialView("_Providers", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderUpdate(int id, DXPANACEASOFT.Models.ItemProvider itemProvider)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = item.ItemProvider.FirstOrDefault(i => i.id_provider == itemProvider.id_provider);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                    }
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            TempData.Keep("item");

            var model = item?.ItemProvider.ToList() ?? new List<ItemProvider>();
            return PartialView("_Providers", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProviderDelete(int id, System.Int32 id_provider)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? db.Item.FirstOrDefault(i => i.id == id);
            item = item ?? new Item();

            if (id_provider >= 0)
            {
                try
                {
                    var itemProvider = item.ItemProvider.FirstOrDefault(p => p.id_provider == id_provider);
                    if (itemProvider != null)
                        item.ItemProvider.Remove(itemProvider);

                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            TempData.Keep("item");

            var model = item?.ItemProvider.ToList() ?? new List<ItemProvider>();
            return PartialView("_Providers", model);
        }

        #endregion

        #region "MIGRACION ITEM"
        protected async Task<JsonResult> AsyncAwait_GetSomeDataAsync(int id)
        {
            Item item = (TempData["item"] as Item);
            if (item != null)
            {
                TempData["item"] = item;
                TempData.Keep("item");
            }
            var httpClient = new HttpClient();

            string resultado = string.Empty;
            try
            {
                var data = JsonConvert.SerializeObject(null);

                var actualizar = "/AddModifyItem?id=" + id;

                var uri = ConfigurationManager.AppSettings["URIProduccion"] + actualizar;//"http://localhost:49067/Api/Provider + actualizar";
                var response = await httpClient.PostAsync(uri, new StringContent(data));

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    return null;
                }

                //var result = await httpClient.GetAsync("http://localhost:27631/Api/PersonProvider", HttpCompletionOption.ResponseContentRead);

                string content = await response.Content.ReadAsStringAsync();
                resultado = JsonConvert.DeserializeObject<string>(content);

            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());
            }
            int indiceRespuestaProducto = 0;
            string respuestaProductoTmp = string.Empty;
            if (resultado != string.Empty)
            {
                indiceRespuestaProducto = resultado.IndexOf("respuestaProducto:");
                if (indiceRespuestaProducto > -1)
                {
                    indiceRespuestaProducto = indiceRespuestaProducto > -1 ? indiceRespuestaProducto : 0;
                    if (indiceRespuestaProducto == 0)
                    {
                        respuestaProductoTmp = resultado.Substring(19, (resultado.Length - 19));
                    }
                }
            }
            var result = new
            {
                respuestaProducto = respuestaProductoTmp,

            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> MigrarIndividual(int id)
        {

            var resultado = await AsyncAwait_GetSomeDataAsync(id);
            var result = new
            {
                Data = JsonConvert.SerializeObject(resultado)
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ITEM TECHNICAL SPECIFICATIONS
        [ValidateInput(false)]
        public ActionResult ItemTechnicalSpecificationsAttachedDocumentsPartial()
        {
            Item item = (TempData["item"] as Item);
            item = item ?? new Item();

            var model = item.ItemDocument;
            TempData.Keep("item");
            //SetViewData();

            return PartialView("_ItemTechnicalSpecificationsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTechnicalSpecificationsAttachedDocumentsPartialAddNew(DXPANACEASOFT.Models.ItemDocument itemTechnicalSpecifications)
        {
            Item item = (TempData["item"] as Item);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(itemTechnicalSpecifications.attachment))
                    {
                        throw new Exception("El Documento adjunto no puede ser vacio");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(itemTechnicalSpecifications.guid) || string.IsNullOrEmpty(itemTechnicalSpecifications.url))
                        {
                            throw new Exception("El fichero no se cargo completo, intente de nuevo");
                        }
                        else
                        {
                            var itemTechnicalSpecificationsDocumentDetailAux = item.
                                                    ItemDocument.
                                                    FirstOrDefault(fod => fod.attachment == itemTechnicalSpecifications.attachment);
                            if (itemTechnicalSpecificationsDocumentDetailAux != null)
                            {
                                throw new Exception("No se puede repetir el Documento Adjunto: " + itemTechnicalSpecifications.attachment + ", en el detalle de los Documentos Adjunto.");
                            }

                        }
                    }
                    itemTechnicalSpecifications.id = item.ItemDocument.Count() > 0 ? item.ItemDocument.Max(pld => pld.id) + 1 : 1;
                    item.ItemDocument.Add(itemTechnicalSpecifications);
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("item");
            var model = item.ItemDocument;

            return PartialView("_ItemTechnicalSpecificationsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTechnicalSpecificationsAttachedDocumentsPartialUpdate(DXPANACEASOFT.Models.ItemDocument itemTechnicalSpecifications)
        {
            Item item = (TempData["item"] as Item);
            item = item ?? new Item();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = item.ItemDocument.FirstOrDefault(i => i.id == item.id);
                    if (string.IsNullOrEmpty(itemTechnicalSpecifications.attachment))
                    {
                        throw new Exception("El Documento adjunto no puede ser vacio");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(itemTechnicalSpecifications.guid) || string.IsNullOrEmpty(itemTechnicalSpecifications.url))
                        {
                            throw new Exception("El fichero no se cargo completo, intente de nuevo");
                        }
                        else
                        {
                            if (modelItem.attachment != itemTechnicalSpecifications.attachment)
                            {
                                var itemTechnicalSpecificationsDocumentDetailAux = item.
                                                      ItemDocument.
                                                      FirstOrDefault(fod => fod.attachment == itemTechnicalSpecifications.attachment);
                                if (itemTechnicalSpecificationsDocumentDetailAux != null)
                                {
                                    throw new Exception("No se puede repetir el Documento Adjunto: " + itemTechnicalSpecifications.attachment + ", en el detalle de los Documentos Adjunto.");
                                }
                            }
                        }
                    }
                    if (modelItem != null)
                    {
                        modelItem.guid = itemTechnicalSpecifications.guid;
                        modelItem.url = itemTechnicalSpecifications.url;
                        modelItem.attachment = itemTechnicalSpecifications.attachment;
                        modelItem.referenceDocument = itemTechnicalSpecifications.referenceDocument;
                        modelItem.descriptionDocument = itemTechnicalSpecifications.descriptionDocument;

                        this.UpdateModel(modelItem);
                    }
                    TempData["item"] = item;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            TempData.Keep("item");
            var model = item.ItemDocument;


            return PartialView("_ItemTechnicalSpecificationsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTechnicalSpecificationsAttachedDocumentsPartialDelete(System.Int32 id)
        {
            Item item = (TempData["item"] as Item);
            item = item ?? new Item();

            try
            {
                var itemTechnicalSpecifications = item.ItemDocument.FirstOrDefault(it => it.id == id);
                if (itemTechnicalSpecifications != null)
                    item.ItemDocument.Remove(itemTechnicalSpecifications);

                TempData["item"] = item;
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            TempData.Keep("item");
            var model = item.ItemDocument;

            return PartialView("_ItemTechnicalSpecificationsAttachedDocumentsEditPartial", model.OrderByDescending(od => od.id).ToList());
        }

        #endregion

        #region ITEM TECHNICAL SPECIFICATIONS ATTACHED DOCUMENTS
        private void TechnicalSpecificationsUpdateAttachment(Item item)
        {
            List<ItemDocument> itemDocument = item.ItemDocument.ToList() ?? new List<ItemDocument>();
            foreach (var itemTmp in itemDocument)
            {
                if (itemTmp.url == FileUploadHelper.UploadDirectoryDefaultTemp)
                {
                    try
                    {
                        // Carga el contenido guardado en el temp
                        string nameAttachment;
                        string typeContentAttachment;
                        string guidAux = itemTmp.guid;
                        string urlAux = itemTmp.url;
                        var contentAttachment = FileUploadHelper.ReadFileUpload(
                            ref guidAux, out nameAttachment, out typeContentAttachment);

                        // Guardamos en el directorio final el fichero que este aun en su ruta temporal
                        itemTmp.guid = FileUploadHelper.FileUploadProcessAttachment("/Item/" + item.id.ToString(), nameAttachment, typeContentAttachment, contentAttachment, out urlAux);
                        itemTmp.url = urlAux;

                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error al guardar el adjunto. Error: " + exception.Message);
                    }

                }
            }
        }

        private void TechnicalSpecificationsDeleteAttachment(ItemDocument itemDocument)
        {
            if (itemDocument.url != FileUploadHelper.UploadDirectoryDefaultTemp)
            {
                try
                {
                    // Carga el contenido guardado en el temp
                    FileUploadHelper.CleanUpUploadedFiles(itemDocument.url, itemDocument.guid);

                }
                catch (Exception exception)
                {
                    throw new Exception("Error al borrar el adjunto. Error: " + exception.Message);
                }
            }
        }

        [HttpGet]
        [ActionName("TSdownload-attachment")]
        public ActionResult TechnicalSpecificationsDownloadAttachment(int id)
        {
            //TempData.Keep("item");

            try
            {
                Item item = (TempData["item"] as Item);
                TempData.Keep("item");
                List<ItemDocument> itemDocument = item?.ItemDocument?.ToList() ?? new List<ItemDocument>();


                var itemDocumentAux = itemDocument.FirstOrDefault(fod => fod.id == id);
                if (itemDocumentAux != null)
                {
                    // Carga el contenido guardado en el temp
                    string nameAttachment;
                    string typeContentAttachment;
                    string guidAux = itemDocumentAux.guid;
                    string urlAux = itemDocumentAux.url;
                    var contentAttachment = FileUploadHelper.ReadFileUpload(
                        ref guidAux, ref urlAux, out nameAttachment, out typeContentAttachment);

                    return this.File(contentAttachment, typeContentAttachment, nameAttachment);
                }
                else
                {
                    //return this.File(new byte[] { }, "", "");
                    return null;
                }

            }
            catch (Exception exception)
            {
                throw new Exception("Error al bajar el adjunto. Error: " + exception.Message);
            }
        }

        #endregion

        #region Copiar Items

        [HttpPost, ValidateInput(false)]
        public ActionResult CopySelectedItem(int[] idsItem)
        {
            // Genero i nuevo modelo
            var _copyItem = new Item();
            // Verifico el Id de Item

            // valor para devolver la vista de edición o no
            bool isValid = false;
            string valueSetting = DataProviderSetting.ValueSetting("PGCMLI");
            if (idsItem != null && idsItem.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Busco en la DB
                        var items = db.Item.Where(i => idsItem.Contains(i.id));

                        // Recorro la información mientras la asigno al nuevo modelo
                        foreach (var item in items)
                        {
                            // Formación de nuevo código
                            string itemMasterCode = BuildCopiedMasterCode(item.id_inventoryLine);

                            #region General Data
                            _copyItem.id_inventoryLine = item.id_inventoryLine;
                            _copyItem.id_itemType = item.id_itemType;
                            _copyItem.id_itemTypeCategory = item.id_itemTypeCategory;
                            _copyItem.name = item.name.Trim();
                            _copyItem.description = item.description?.Trim() ?? string.Empty;
                            _copyItem.description2 = item.description2?.Trim() ?? string.Empty;
                            _copyItem.masterCode = !string.IsNullOrEmpty(itemMasterCode) ? itemMasterCode : string.Empty;
                            _copyItem.id_presentation = item.id_presentation;
                            _copyItem.id_metricType = item.id_metricType;
                            _copyItem.auxCode = item.auxCode;
                            _copyItem.barCode = item.barCode;
                            _copyItem.foreignName = item.foreignName;
                            _copyItem.isPurchased = item.isPurchased;
                            _copyItem.isSold = item.isSold;
                            _copyItem.inventoryControl = item.inventoryControl;
                            _copyItem.hasFormulation = item.hasFormulation;
                            _copyItem.id_itemWeight = item.id_itemWeight;
                            _copyItem.isConsumed = item.isConsumed;
                            _copyItem.notShowInReport = item.notShowInReport;
                            _copyItem.isActive = item.isActive;
                            _copyItem.photo = item.photo;
                            #endregion

                            #region ItemGeneral
                            var itemGeneral = item?.ItemGeneral;

                            if (itemGeneral != null)
                            {
                                //_copyItem.ItemGeneral = itemGeneral;
                                _copyItem.ItemGeneral = new ItemGeneral()
                                {
                                    id_group = itemGeneral.id_group,
                                    id_subgroup = itemGeneral.id_subgroup,
                                    id_groupCategory = itemGeneral.id_groupCategory,
                                    id_countryOrigin = itemGeneral.id_countryOrigin,
                                    id_trademark = itemGeneral.id_trademark,
                                    id_trademarkModel = itemGeneral.id_trademarkModel,
                                    id_color = itemGeneral.id_color,
                                    id_size = itemGeneral.id_size,
                                    id_Person = itemGeneral.id_Person,
                                    id_certification = itemGeneral.id_certification,
                                    mesVidaUtil = itemGeneral.mesVidaUtil
                                };
                            }
                            #endregion

                            #region Item Certification

                            if (itemGeneral != null && itemGeneral.id_certification != null)
                            {
                                var aModelItemNameSplit = _copyItem.name.Split('-');
                                if (aModelItemNameSplit.Length > 1)
                                {
                                    _copyItem.name = "";
                                    for (int i = 1; i < aModelItemNameSplit.Length; i++)
                                    {
                                        if (_copyItem.name != "")
                                        {
                                            _copyItem.name += "-";
                                        }
                                        _copyItem.name += aModelItemNameSplit[i];
                                    }
                                }
                            }

                            if (itemGeneral != null && itemGeneral.id_certification != null)
                            {
                                var certification = db.Certification.FirstOrDefault(e => e.id == itemGeneral.id_certification);
                                if(!string.IsNullOrEmpty(certification?.idProducto))
                                {
                                    _copyItem.name = $"{certification.idProducto} - {_copyItem.name}";
                                }
                            }

                            #endregion

                            #region Items Provider

                            if (item?.ItemProvider != null)
                            {
                                _copyItem.ItemProvider = new List<ItemProvider>();

                                var providers = item.ItemProvider.ToList();

                                foreach (var provider in providers)
                                {
                                    var tempProvider = new ItemProvider
                                    {
                                        id_provider = provider.id_provider,
                                        isFavorite = provider.isFavorite,
                                    };
                                    _copyItem.ItemProvider.Add(tempProvider);
                                }
                            }

                            #endregion

                            #region Purchase Information

                            var itemPurchase = item.ItemPurchaseInformation;

                            if (item.isPurchased && itemPurchase != null)
                            {
                                _copyItem.ItemPurchaseInformation = new ItemPurchaseInformation
                                {
                                    purchasePrice = itemPurchase.purchasePrice,
                                    largestPurchasePrice = itemPurchase.largestPurchasePrice,
                                    id_metricUnitPurchase = itemPurchase.id_metricUnitPurchase,
                                    shortDescriptionPurchase = itemPurchase.shortDescriptionPurchase,
                                    descriptionPurchase = itemPurchase.descriptionPurchase
                                };
                            }

                            #endregion

                            #region Sale Information

                            var itemSale = item.ItemSaleInformation;

                            if (item.isSold && itemSale != null)
                            {
                                _copyItem.ItemSaleInformation = new ItemSaleInformation
                                {
                                    salePrice = itemSale.salePrice,
                                    wholesalePrice = itemSale.wholesalePrice,
                                    id_metricUnitSale = itemSale.id_metricUnitSale,
                                    shortDescriptionSale = itemSale.shortDescriptionSale,
                                    descriptionSale = itemSale.descriptionSale
                                };
                            }

                            #endregion

                            #region Inventory

                            var itemInventory = item.ItemInventory;

                            if (item.inventoryControl && itemInventory != null)
                            {
                                _copyItem.ItemInventory = new ItemInventory
                                {
                                    //id_inventoryControlType = itemInventory.id_inventoryControlType,
                                    //id_valueValuationMethod = itemInventory.id_valueValuationMethod,
                                    isImported = itemInventory.isImported,
                                    requiresLot = itemInventory.requiresLot,
                                    id_warehouse = itemInventory.id_warehouse,
                                    id_warehouseLocation = itemInventory.id_warehouseLocation,
                                    minimumStock = itemInventory.minimumStock,
                                    maximumStock = itemInventory.maximumStock,
                                    currentStock = itemInventory.currentStock,
                                    id_metricUnitInventory = itemInventory.id_metricUnitInventory
                                    //expirationDate = itemInventory.expirationDate
                                };
                            }


                            #endregion

                            #region  Header Ingredients

                            var itemHeadIngredient = item.ItemHeadIngredient;

                            if (itemHeadIngredient != null)
                            {
                                _copyItem.ItemHeadIngredient = new ItemHeadIngredient
                                {
                                    amount = itemHeadIngredient.amount,
                                    id_metricUnit = itemHeadIngredient.id_metricUnit
                                };
                            }
                            #endregion

                            #region Ingredients

                            if (item?.ItemIngredient != null  && item?.ItemIngredient.Count() > 0)
                            {
                                _copyItem.ItemIngredient = new List<ItemIngredient>();

                                var ingredients = item.ItemIngredient.ToList();

                                foreach (var ingredient in ingredients)
                                {
                                    var tempIngredient = new ItemIngredient
                                    {
                                        id_ingredientItem = ingredient.id_ingredientItem,
                                        id_metricUnit = ingredient.id_metricUnit,
                                        amount = ingredient.amount,
                                        id_metricUnitMax = ingredient.id_metricUnitMax,
                                        amountMax = ingredient.amountMax
                                    };
                                    _copyItem.ItemIngredient.Add(tempIngredient);
                                }
                            }
                            #endregion

                            #region Taxation

                            if (item?.ItemTaxation != null)
                            {
                                _copyItem.ItemTaxation = new List<ItemTaxation>();

                                var taxations = item.ItemTaxation.ToList();

                                foreach (var taxation in taxations)
                                {
                                    var tempTaxation = new ItemTaxation
                                    {
                                        id_taxType = taxation.id_taxType,
                                        id_rate = taxation.id_rate,
                                        percentage = taxation.percentage
                                    };
                                    _copyItem.ItemTaxation.Add(tempTaxation);
                                }
                            }

                            #endregion

                            #region Tariff Item

                            if (item.TariffItem != null)
                            {
                                if (!string.IsNullOrEmpty(item.TariffItem?.code) && !string.IsNullOrEmpty(item.TariffItem?.name))
                                {
                                    _copyItem.TariffItem = new TariffItem
                                    {
                                        code = item.TariffItem.code,
                                        name = item.TariffItem.name,
                                    };
                                }
                            }

                            #endregion

                            #region Additional Fields

                            if (item?.ItemAditionalField != null)
                            {
                                _copyItem.ItemAditionalField = new List<ItemAditionalField>();

                                var aditionalFields = item.ItemAditionalField.ToList();

                                foreach (var aditionalField in aditionalFields)
                                {
                                    var tempAditionalField = new ItemAditionalField
                                    {
                                        label = aditionalField.label,
                                        value = aditionalField.value,
                                    };
                                    _copyItem.ItemAditionalField.Add(tempAditionalField);
                                }

                            }

                            #endregion

                            #region Weight

                            InventoryLine inventoryLineTmp = db.InventoryLine.FirstOrDefault(fod => fod.id == item.id_inventoryLine);
                            inventoryLineTmp = inventoryLineTmp ?? new InventoryLine();
                            var itemWeightConversionFreezen = item.ItemWeightConversionFreezen;

                            if (itemWeightConversionFreezen != null && inventoryLineTmp.code == "PT")
                            {
                                _copyItem.ItemWeightConversionFreezen = new ItemWeightConversionFreezen
                                {
                                    id_MetricUnit = itemWeightConversionFreezen.id_MetricUnit,
                                    itemWeightGrossWeight = itemWeightConversionFreezen.itemWeightGrossWeight,
                                    itemWeightNetWeight = itemWeightConversionFreezen.itemWeightNetWeight,
                                    conversionToKilos = itemWeightConversionFreezen.conversionToKilos == 0 || itemWeightConversionFreezen.conversionToKilos == null ? 1 : itemWeightConversionFreezen.conversionToKilos,
                                    conversionToPounds = itemWeightConversionFreezen.conversionToPounds == 0 || itemWeightConversionFreezen.conversionToPounds == null ? 1 : itemWeightConversionFreezen.conversionToPounds,
                                    weightWithGlaze = itemWeightConversionFreezen.weightWithGlaze,
                                    glazePercentage = itemWeightConversionFreezen.glazePercentage

                                };
                            }

                            #endregion

                            #region Document

                            if (item?.ItemDocument != null && item.ItemDocument.Any())
                            {
                                _copyItem.ItemDocument = new List<ItemDocument>();
                                var itemItemDocument = item.ItemDocument.ToList();

                                foreach (var detail in itemItemDocument)
                                {
                                    var tempItemItemDocument = new ItemDocument
                                    {
                                        guid = detail.guid,
                                        url = detail.url,
                                        attachment = detail.attachment,
                                        referenceDocument = detail.referenceDocument,
                                        descriptionDocument = detail.descriptionDocument
                                    };

                                    _copyItem.ItemDocument.Add(tempItemItemDocument);
                                }
                            }

                            #endregion

                            #region Equivalence

                            var _il = db.InventoryLine.FirstOrDefault(fod => fod.id == item.id_inventoryLine);
                            if (item.ItemEquivalence != null)
                            {
                                var idItemEquivalence = item.ItemEquivalence?.id_itemEquivalence;

                                if (_il.code.Equals("PP") || _il.code.Equals("PT"))
                                {
                                    if (idItemEquivalence.Value > 0)
                                    {
                                        var equivalence = new ItemEquivalence()
                                        {
                                            id_itemEquivalence = idItemEquivalence,
                                        };

                                        _copyItem.ItemEquivalence = equivalence;
                                    }
                                }
                            }

                            #endregion

                            #region Audit

                            _copyItem.id_company = this.ActiveCompanyId;
                            _copyItem.id_userCreate = ActiveUser.id;
                            _copyItem.dateCreate = DateTime.Now;
                            _copyItem.id_userUpdate = ActiveUser.id;
                            _copyItem.dateUpdate = DateTime.Now;

                            #endregion

                            #region Inventory Line

                            InventoryLine inventoryLine = db.InventoryLine.FirstOrDefault(it => it.id == item.id_inventoryLine);
                            if (valueSetting == "NO")
                            {
                                if (inventoryLine != null)
                                {
                                    inventoryLine.sequential++;

                                    db.InventoryLine.Attach(inventoryLine);
                                    db.Entry(inventoryLine).State = EntityState.Modified;
                                } 
                            }

                            string valSet = DataProviderSetting.ValueSetting("CDCAP");

                            if (valSet == "YES")
                            {
                                if (inventoryLine.code == "PT" || inventoryLine.code == "PP")
                                {
                                    var strCodeAux = "";
                                    //Tipo de Producto
                                    strCodeAux = GetAuxiliarCodeForProduct(item);

                                    if (strCodeAux != "ERROR" && strCodeAux.Length > 8)
                                    {
                                        _copyItem.auxCode = strCodeAux;
                                    }
                                }
                            }

                            #endregion

                            //if (valueSetting == "NO")
                            //{
                                // Asigno a la base para guardar cambios
                                db.Item.Add(_copyItem);
                                db.Entry(_copyItem).State = EntityState.Added;

                                #region Migration Item

                                MigrationItem migrationItem = db.MigrationItem.FirstOrDefault(fod => fod.id_item == item.id);
                                if (migrationItem == null && item.InventoryLine.code == "PT")
                                {
                                    migrationItem = new MigrationItem
                                    {
                                        id_userCreate = ActiveUser.id,
                                        dateCreate = DateTime.Now
                                    };
                                    db.MigrationItem.Add(migrationItem);
                                }

                                #endregion

                                db.SaveChanges();                            
                            //}
                        }
                        //if (valueSetting == "NO")
                        //{
                            isValid = true;
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage($"Producto copiado exitosamente");
                        //}
                    }
                    catch (Exception e)
                    {                  
                        ViewData["EditError"] = e.Message;
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }
            }

            if (valueSetting == "SI")
            {
                var result = new
                {
                    isValid,
                    idNewItem = isValid ? _copyItem?.id : null,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        private string BuildCopiedMasterCode(int? id_InventoryLine)
        {
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

            InventoryLine inventoryLine = db.InventoryLine.FirstOrDefault(i => i.id == id_InventoryLine);
            if (inventoryLine != null)
            {
                int codeLength = 6;
                if (congLineasInvCods.Contains(inventoryLine.code))
                {
                    itemMasterCode = inventoryLine.code + "-";
                }
                else
                {
                    itemMasterCode = inventoryLine.code + inventoryLine.sequential.ToString().PadLeft(codeLength, '0');
                }
            }
            return itemMasterCode;
        }

        public JsonResult ItsRepeatedAttachmentDetail(string attachmentNameNew)
        {
            Item item = (TempData["item"] as Item);

            item = item ?? new Item();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var itemDocumentDetailAux = item.
                                        ItemDocument.
                                        FirstOrDefault(fod => fod.attachment == attachmentNameNew);
            if (itemDocumentDetailAux != null)
            {
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Documento Adjunto: " + attachmentNameNew + ", en el detalle de los Documentos Adjunto."

                };

            }

            TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public string GetAuxiliarCodeForProduct(Item it)
        {
            var strCodeAux = "";
            string codAuxPerson = "-----";
            try
            {

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
            catch (Exception e)
            {

                strCodeAux = "ERROR";
                LogWrite(e, null, "GetAuxiliarCodeForProduct=>" + it.id);
            }


            var stringLength = strCodeAux?.Count() ?? 0;
            var maxLengthGloss = ((stringLength > 25) ? 25 : stringLength);
            return strCodeAux.Substring(0, maxLengthGloss);

        }

        [HttpPost]
        public JsonResult ValidaRepeatMasterCodeItem(string masterCode)
        {
            Item item = (TempData["item"] as Item);
            item = item ?? new Item();

            TempData.Keep("item");


            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            if (!string.IsNullOrWhiteSpace(masterCode))
            {
                var nitems = db.Item
                                .Where(r => r.masterCode.Trim().ToUpper() == masterCode.Trim().ToUpper()
                                            && r.id != item.id)
                                .Count();
                if (nitems > 0)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = $"No se puede repetir el Codigo del Producto: {masterCode}"

                    };
                }


            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult ItemEquivalenceCallBack(int? idItemEquivalence)
        {

            return this.PartialView("ComboBox/_EquivalenceItem", new ItemEquivalence { id_itemEquivalence = idItemEquivalence ?? 0 });
        }

        public JsonResult ItsRepeatedIngredientItemAndCostumerItem(int? id_ingredientItem, int? id_costumerItem)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            Item item = (TempData["item"] as Item);

            item = item ?? new Item();
            var result = new
            {
                itsRepeated = 0,
                Error = ""

            };

            var itemItemIngredientAux = item.
                                        ItemIngredient.
                                        FirstOrDefault(fod => fod.id_ingredientItem == id_ingredientItem && fod.id_costumerItem == id_costumerItem);
            if (itemItemIngredientAux != null)
            {
                var aItem = db.Item.FirstOrDefault(fod => fod.id == id_ingredientItem);
                var aPerson = db.Person.FirstOrDefault(fod => fod.id == id_costumerItem);
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir el Ingrediente: " + (aItem?.name ?? "SIN INGREDIENTE") + ", con el mismo cliente: " + (aPerson?.fullname_businessName ?? "SIN CLIENTE") + " en el detalle de Formulación."

                };

            }

            TempData.Keep("item");
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Métodos para la importación de plantillas (De productos)

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItems()
        {
            return PartialView("_FormEditImportItems");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItems()
        {
            var empresa = this.ActiveCompanyId;
            return ItemExcelFileParser.GetTemplateFileContentResult(empresa);
        }

        [HttpPost]
        [ActionName("upload-file")]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles(
                "ItemArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        [HttpPost]
        public JsonResult ImportDatosCargaMasiva(string guidArchivoDatos)
        {
            bool isValid;
            string message;
            ImportResult resultadoImportacion;

            try
            {
                var contenidoArchivo = FileUploadHelper.ReadFileUpload(guidArchivoDatos,
                    out var nombreArchivo, out var mimeArchivo, out var rutaArchivoContenido);

                var itemTypeCategoryParser = new ItemExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemTypeCategoryParser.ParseItem(ActiveCompanyId, ActiveUser);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron los productos exitosamente";
            }
            catch (Exception ex)
            {
                isValid = false;

                resultadoImportacion = new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = new ImportResult.DocumentoFallido[] { },
                };

                message = ex.InnerException != null
                    ? "Error al importar los productos. " + ex.InnerException.Message
                    : "Error al importar los productos. " + ex.Message;
            }

            // Preservamos los resultados de la importación de datos
            var guidResultado = Guid.NewGuid().ToString("n");
            this.TempData[$"documentos-importados-{guidResultado}"] = resultadoImportacion.Importados;
            this.TempData[$"documentos-fallidos-{guidResultado}"] = resultadoImportacion.Fallidos;

            ViewData["EditMessage"] = resultadoImportacion.Fallidos.Any() || !isValid
                ? ErrorMessage(message) : SuccessMessage(message);

            // Retornar el resultado...
            var result = new
            {
                isValid,
                message,
                guidResultado,
                HayErrores = resultadoImportacion.Fallidos.Any(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResult DownloadDocumentosFallidosImportacion(string guidResultado, string mensajeAlerta)
        {
            this.ViewBag.ReportCommand = "export";
            this.ViewBag.ReportTitle = "Errores en Importación de Productos";
            this.ViewBag.ExcelFileName = $"ErroresImportacion_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-fallidos-{guidResultado}";
            var documentosFallidos = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoFallido[]
                : new ImportResult.DocumentoFallido[] { };

            ViewData["EditMessage"] = ErrorMessage(mensajeAlerta);


            return this.View("_DocumentosFallidosImportacionReportPartial", documentosFallidos);
        }

        [HttpGet]
        public ViewResult DownloadDocumentosImportadosImportacion(string guidResultado, string mensajeAlerta)
        {
            this.ViewBag.ReportCommand = "export";
            this.ViewBag.ReportTitle = "Resultados de Importación de Producto";
            this.ViewBag.ExcelFileName = $"ResultadosImportacion_{ActiveCompanyId}_{DateTime.Now:yyyyMMdd HHmm}.xls";

            var key = $"documentos-importados-{guidResultado}";
            var documentosImportados = this.TempData.ContainsKey(key)
                ? this.TempData[key] as ImportResult.DocumentoImportado[]
                : new ImportResult.DocumentoImportado[] { };

            ViewData["EditMessage"] = SuccessMessage(mensajeAlerta);


            return this.View("_DocumentosImportadosImportacionReportPartial", documentosImportados);
        }

        #endregion

        #region Métodos para la importación de plantillas (De formulaciones)

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItemFormulations()
        {
            return PartialView("_FormEditImportItemFormulations");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItemFormulations()
        {
            var empresa = this.ActiveCompanyId;
            return ItemFormulationExcelFileParser.GetTemplateFileContentResult(empresa);
        }

        [HttpPost]
        [ActionName("upload-file-formulation")]
        public ActionResult UploadControlUploadFormulation()
        {
            UploadControlExtension.GetUploadedFiles(
                "ItemFormulationArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        public JsonResult ImportDatosCargaMasivaFormulacion(string guidArchivoDatos)
        {
            bool isValid;
            string message;
            ImportResult resultadoImportacion;

            try
            {
                var contenidoArchivo = FileUploadHelper.ReadFileUpload(guidArchivoDatos,
                    out var nombreArchivo, out var mimeArchivo, out var rutaArchivoContenido);

                var itemFormulationCategoryParser = new ItemFormulationExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemFormulationCategoryParser.ParseItemFormulation(ActiveCompanyId, ActiveUser);

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron los productos exitosamente";
            }
            catch (Exception ex)
            {
                isValid = false;

                resultadoImportacion = new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = new ImportResult.DocumentoFallido[] { },
                };

                message = ex.InnerException != null
                    ? "Error al importar los productos. " + ex.InnerException.Message
                    : "Error al importar los productos. " + ex.Message;
            }

            // Preservamos los resultados de la importación de datos
            var guidResultado = Guid.NewGuid().ToString("n");
            this.TempData[$"documentos-importados-{guidResultado}"] = resultadoImportacion.Importados;
            this.TempData[$"documentos-fallidos-{guidResultado}"] = resultadoImportacion.Fallidos;

            ViewData["EditMessage"] = resultadoImportacion.Fallidos.Any() || !isValid
                ? ErrorMessage(message) : SuccessMessage(message);

            // Retornar el resultado...
            var result = new
            {
                isValid,
                message,
                guidResultado,
                HayErrores = resultadoImportacion.Fallidos.Any(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Métodos para la importación de plantillas (De Equivalente) 

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditItemEquivalences()
        {
            return PartialView("_FormEditImportItemEquivalence");
        }

        [HttpGet, ValidateInput(false)]
        public FileContentResult DownloadTemplateImportItemEquivalences()
        {
            var empresa = this.ActiveCompanyId;
            return ItemEquivalenceExcelFileParser.GetTemplateFileContentResult(empresa);
        }

        [HttpPost]
        [ActionName("upload-file-equivalence")]
        public ActionResult UploadControlUploadEquivalence()
        {
            UploadControlExtension.GetUploadedFiles(
                "ItemEquivalenceArchivoUploadControl", UploadControlSettings.ExcelUploadValidationSettings, UploadControlSettings.FileUploadComplete);

            return null;
        }

        public JsonResult ImportDatosCargaMasivaEquivalencia(string guidArchivoDatos)
        {
            bool isValid;
            string message;
            ImportResult resultadoImportacion;

            try
            {
                var contenidoArchivo = FileUploadHelper.ReadFileUpload(guidArchivoDatos,
                    out var nombreArchivo, out var mimeArchivo, out var rutaArchivoContenido);

                var itemEquivalenceCategoryParser = new ItemEquivalenceExcelFileParser(rutaArchivoContenido, mimeArchivo);

                resultadoImportacion = itemEquivalenceCategoryParser.ParseItemEquivalence();

                isValid = true;
                message = resultadoImportacion.Fallidos.Any()
                    ? "Se encontraron errores en la plantilla a importar"
                    : "Se importaron los productos exitosamente";
            }
            catch (Exception ex)
            {
                isValid = false;

                resultadoImportacion = new ImportResult()
                {
                    Importados = new ImportResult.DocumentoImportado[] { },
                    Fallidos = new ImportResult.DocumentoFallido[] { },
                };

                message = ex.InnerException != null
                    ? "Error al importar los productos. " + ex.InnerException.Message
                    : "Error al importar los productos. " + ex.Message;
            }

            // Preservamos los resultados de la importación de datos
            var guidResultado = Guid.NewGuid().ToString("n");
            this.TempData[$"documentos-importados-{guidResultado}"] = resultadoImportacion.Importados;
            this.TempData[$"documentos-fallidos-{guidResultado}"] = resultadoImportacion.Fallidos;

            ViewData["EditMessage"] = resultadoImportacion.Fallidos.Any() || !isValid
                ? ErrorMessage(message) : SuccessMessage(message);

            // Retornar el resultado...
            var result = new
            {
                isValid,
                message,
                guidResultado,
                HayErrores = resultadoImportacion.Fallidos.Any(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Configuración común para la carga de archivo de Excel con transacciones

        public class UploadControlSettings
        {
            public readonly static UploadControlValidationSettings ImageUploadValidationSettings;
            public readonly static UploadControlValidationSettings ExcelUploadValidationSettings;
            public readonly static UploadControlValidationSettings AnyDocumentUploadValidationSettings;

            static UploadControlSettings()
            {
                ImageUploadValidationSettings = new UploadControlValidationSettings()
                {
                    AllowedFileExtensions = new[] { ".jpe", ".jpeg", ".jpg", ".gif", ".png" },
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };

                ExcelUploadValidationSettings = new UploadControlValidationSettings()
                {
                    AllowedFileExtensions = new[] { ".xls", ".xlsx", ".xlsm" },
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };

                AnyDocumentUploadValidationSettings = new UploadControlValidationSettings()
                {
                    MaxFileCount = 1,
                    MaxFileSize = FileUploadHelper.MaxFileUploadSize,
                    MaxFileSizeErrorText = FileUploadHelper.MaxFileSizeErrorText,
                };
            }

            public static void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
            {
                if (e.UploadedFile.IsValid)
                {
                    var fileId = FileUploadHelper.FileUploadProcess(e);

                    if (!String.IsNullOrEmpty(fileId))
                    {
                        var result = new
                        {
                            id = fileId,
                            filename = e.UploadedFile.FileName,
                        };

                        e.CallbackData = JsonConvert.SerializeObject(result);
                    }
                }
            }
        }

        #endregion
    }
}