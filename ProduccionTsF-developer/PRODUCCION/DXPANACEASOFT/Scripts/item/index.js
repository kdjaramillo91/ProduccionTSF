
function InventoryLinesCombo_SelectedIndexChanged(s, e) {
    masterCode.SetText("");
       
    var data = {
        id_inventoryLine: id_inventoryLine.GetValue()
    };

    $.ajax({
        url: "Item/BuildMasterCode",
        type: "post",
        data: data, 
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            
            UpdateMessageErrorInventoryLine(result.Message);
              
            if (result.Message != "Ok") {
                id_inventoryLine.SetValue(null);
            }
            else {
                masterCode.SetText(result.itemMasterCode);
                masterCode.readOnly = result.readOnly;
                $("#code_inventoryLine").val(result.code_inventoryLine);
                UpdateItemPresentations(result.code_inventoryLine);
                UpdateGeneralByInventoryLine(result);
                UpdateTabFormulation();
                UpdateTabWeight();
                UpdateTabItemEquivalence();
                UpdateTabTechnicalSpecifications();
                var vsTmp = $("#valSet").val();
                document.getElementById("code_inventoryLine").value = result.code_inventoryLine;
                var vsTmp = $("#valSet").val();
                  
                if (vsTmp == "YES")
                {
                    if (result.code_inventoryLine == "PT" || result.code_inventoryLine == "PP"  ) {
                        $('input[name=auxCode]').attr('readonly', true);
                        id_PersonLabel.SetVisible(true);
                        id_Person.SetVisible(true);
                    } else {
                        id_PersonLabel.SetVisible(false);
                        id_Person.SetVisible(false);
                        $('input[name=auxCode]').attr('readonly', false);
                    }
                    if (result.code_inventoryLine === "MI") {
                        $("#trIsConsumed").show();
                    } else {
                        $("#trIsConsumed").hide(1500);
                    }
                }
            }
        },
        complete: function () {
        }
    });

    ItemsTypesCombo_Update();

}

function UpdateGeneralByInventoryLine(result) {
    if (result.code_inventoryLine == "PT") {
        id_metricType.SetValue(result.id_metricType);
        MetricTypesCombo_SelectedIndexChanged(id_metricType, null);
        amount.SetValue(1);
        id_metricUnit.SetValue(result.id_metricUni);
    } else {
        id_metricType.SetEnabled(true);
        amount.SetEnabled(true);
        id_metricUnit.SetEnabled(true);
    }
}

function UpdateTabFormulation() {
    var idItem = $("#itemId").val();
    var code_inventoryLine = $("#code_inventoryLine").val();
    if (idItem !== null && idItem !== undefined) {
        tabControl.GetTabByName("tabFormulation").SetVisible(code_inventoryLine != "MI");
        if (code_inventoryLine == "MI") {
            amount.SetValue(0);
            id_metricUnit.SetValue(null);
            hasFormulation.SetChecked(false);
        } else if (code_inventoryLine == "PT") {
            hasFormulation.SetChecked(true);
        } else {
            hasFormulation.SetChecked(true);
        }
        
    }
}

function UpdateTabItemEquivalence() {
    var idItem = $("#itemId").val();
    var code_inventoryLine = $("#code_inventoryLine").val();
    tabControl.GetTabByName("tabEquivalence").SetVisible(code_inventoryLine === "PP" || code_inventoryLine === "PT");
}

function UpdateTabWeight() {
    var idItem = $("#itemId").val();
    var code_inventoryLine = $("#code_inventoryLine").val();
    if (idItem !== null && idItem !== undefined) {
        tabControl.GetTabByName("tabWeight").SetVisible(code_inventoryLine === "PT");

    }
}


function UpdateTabTechnicalSpecifications() {
    var idItem = $("#itemId").val();
    var code_inventoryLine = $("#code_inventoryLine").val();
    if (idItem !== null && idItem !== undefined) {
        tabControl.GetTabByName("tabTechnicalSpecifications").SetVisible(code_inventoryLine === "PT");

    }
}

function UpdateItemPresentations(code_inventoryLine)
{
    id_presentationLabel.SetClientVisible(code_inventoryLine == "PT"|| code_inventoryLine == "PP");
    id_presentation.SetClientVisible(code_inventoryLine == "PT" || code_inventoryLine == "PP");
    if (code_inventoryLine != "PT") {
        id_presentation.SetValue(null);
        ComboPresentation_SelectedIndexChanged(id_presentation, null)
    }
}

function ItemsTypesCombo_Update() {
    id_itemType.ClearItems();
    id_itemTypeCategory.ClearItems();

    $.ajax({
        url: "Item/ItemTypesByInventoryLine",
        type: "post",
        data: { id_inventoryLine: id_inventoryLine.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                var itemType = result[i];
                id_itemType.AddItem(itemType.name, itemType.id);
            }
        },
        complete: function () {
        }
    });
}

function ItemsTypesCombo_SelectedIndexChanged(s, e) {
    ItemTypeCategory_Update();
}

function ItemTypeCategory_Update() {
    id_itemTypeCategory.ClearItems();

    $.ajax({
        url: "Item/ItemTypesCategoriesByItemType",
        type: "post",
        data: { id_itemType: id_itemType.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                var itemTypeCategory = result[i];
                id_itemTypeCategory.AddItem(itemTypeCategory.name, itemTypeCategory.id);
            }
        },
        complete: function () {
        }
    });
}

function PurchaseControl_CheckedChange(s, e) {
    UpdateTabs();
}

function SoldControl_CheckedChange(s, e) {
    UpdateTabs();
}

function InventoryControl_CheckedChange(s, e) {
    UpdateTabs();
}

function FormulationControl_CheckedChange(s, e) {
    UpdateTabs();
}

function MetricTypesCombo_SelectedIndexChanged(s, e) {
    id_metricUnitSale.SetValue(null);
    id_metricUnitPurchase.SetValue(null);
    id_metricUnitInventory.SetValue(null); 
    id_metricUnit.SetValue(null);

    var data = {
        id_metricType: s.GetValue()
    };

    $.ajax({
        url: "Item/GetMetricUnit",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            UpdateMetricUnits(result.metricUnits);
        },
        complete: function () {
        }
    });

}

function UpdateMetricUnits(metricUnits) {

    for (var i = 0; i < id_metricUnitSale.GetItemCount() ; i++) {
        var metricUnitSale = id_metricUnitSale.GetItem(i);
        var into = false;
        for (var j = 0; j < metricUnits.length; j++) {
            if (metricUnitSale.value == metricUnits[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_metricUnitSale.RemoveItem(i);
            i -= 1;
        }
    }

    for (var i = 0; i < id_metricUnitPurchase.GetItemCount() ; i++) {
        var metricUnitPurchase = id_metricUnitPurchase.GetItem(i);
        var into = false;
        for (var j = 0; j < metricUnits.length; j++) {
            if (metricUnitPurchase.value == metricUnits[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_metricUnitPurchase.RemoveItem(i);
            i -= 1;
        }
    }

    for (var i = 0; i < id_metricUnitInventory.GetItemCount() ; i++) {
        var metricUnitInventory = id_metricUnitInventory.GetItem(i);
        var into = false;
        for (var j = 0; j < metricUnits.length; j++) {
            if (metricUnitInventory.value == metricUnits[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_metricUnitInventory.RemoveItem(i);
            i -= 1;
        }
    }

    for (var i = 0; i < id_metricUnit.GetItemCount() ; i++) {
        var metricUnit = id_metricUnit.GetItem(i);
        var into = false;
        for (var j = 0; j < metricUnits.length; j++) {
            if (metricUnit.value == metricUnits[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_metricUnit.RemoveItem(i);
            i -= 1;
        }
    }

    for (var i = 0; i < metricUnits.length; i++) {
        var metricUnitSale = id_metricUnitSale.FindItemByValue(metricUnits[i].id);
        if (metricUnitSale == null) id_metricUnitSale.AddItem(metricUnits[i].name, metricUnits[i].id);

        var metricUnitPurchase = id_metricUnitPurchase.FindItemByValue(metricUnits[i].id);
        if (metricUnitPurchase == null) id_metricUnitPurchase.AddItem(metricUnits[i].name, metricUnits[i].id);

        var metricUnitInventory = id_metricUnitInventory.FindItemByValue(metricUnits[i].id);
        if (metricUnitInventory == null) id_metricUnitInventory.AddItem(metricUnits[i].name, metricUnits[i].id);

        var metricUnit = id_metricUnit.FindItemByValue(metricUnits[i].id);
        if (metricUnit == null) id_metricUnit.AddItem(metricUnits[i].name, metricUnits[i].id);
    }

}


var id_presentationAux = null;

function ComboPresentation_SelectedIndexChanged(s, e) {

    var data = {
        id_presentationOld : id_presentationAux,
        id_presentationNew: s.GetValue()
    };

    $.ajax({
        url: "Item/UpdateFormulationWithItemPresentation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            
            UpdateMessageErrorPresentation(result.Message);
            if (result.Message != "Ok") {
                id_presentation.SetValue(id_presentationAux);
            }
            else {
                Formulation.PerformCallback();
                id_presentationAux = s.GetValue();
            }
        },
        complete: function () {
        }
    });

}

function OnPresentationInit(s, e) {
    id_presentationAux = s.GetValue();
}

function UpdateInventoryItemWarehouseLocations(warehouseLocations) {

    for (var i = 0; i < id_warehouseLocation.GetItemCount() ; i++) {
        var warehouseLocation = id_warehouseLocation.GetItem(i);
        var into = false;
        for (var j = 0; j < warehouseLocations.length; j++) {
            if (warehouseLocation.value == warehouseLocations[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_warehouseLocation.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < warehouseLocations.length; i++) {
        var warehouseLocation = id_warehouseLocation.FindItemByValue(warehouseLocations[i].id);
        if (warehouseLocation == null) id_warehouseLocation.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
    }

}

function ComboWarehouse_SelectedIndexChanged(s, e) {

    id_warehouseLocation.SetValue(null);
    id_warehouseLocation.ClearItems();
    var data = {
        id_warehouse: s.GetValue()
    };

    $.ajax({
        url: "Item/UpdateInventoryItemWarehouseLocation",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                UpdateInventoryItemWarehouseLocations(result.warehouseLocations);
            }
        },
        complete: function () {
        }
    });
}

function ComboWarehouseLocation_Init(s, e) {
    var data = {
        id_warehouse: id_warehouse.GetValue(),
        id_warehouseLocation: s.GetValue()
    };
    $.ajax({
        url: "Item/UpdateInventoryItemWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateInventoryItemWarehouseLocations([]);
        },
        beforeSend: function () {
        },
        success: function (result) {
            UpdateInventoryItemWarehouseLocations(result.warehouseLocations);
        },
        complete: function () {
        }
    });
}

function OnAuxCodeInit(s, e) {
    var coInLiTmp = $("#code_inventoryLine").val();
    var valSet = $("#valSet").val();
    if (valSet == "YES") {
        if (coInLiTmp == "PT" || coInLiTmp == "PP") {
            $('input[name=auxCode]').attr('readonly', true);
        } else {
            $('input[name=auxCode]').attr('readonly', false);
        }
    }
}

function ItemGroupCombo_SelectedIndexChanged(s, e) {
    id_subgroup.PerformCallback();
   
}

function ItemSubGroupsCombo_BeginCallback(s, e) {
    e.customArgs['id_group'] = id_group.GetValue();
    e.customArgs['id_subgroup'] = id_subgroup.GetValue();
    e.customArgs['id_groupCategory'] = id_groupCategory.GetValue();
}

function ItemSubGroupsCombo_EndCallback(s, e) {
}

function ItemSubGroupsCombo_SelectedIndexChanged(s, e) {
}

function ItemGroupCategoryCombo_BeginCallback(s, e) {
    e.customArgs['id_group'] = id_group.GetValue();
    e.customArgs['id_subgroup'] = id_subgroup.GetValue();
    e.customArgs['id_groupCategory'] = id_groupCategory.GetValue();
}

function ItemGroupCategoryCombo_SelectedIndexChanged(s, e) {
    $.ajax({
        url: "Item/ComboBoxItemGroupCategory_SelectedIndexChanged",
        type: "post",
        data: {
            id_group: id_group.GetValue(),
            id_subgroup: id_subgroup.GetValue(),
            id_groupCategory: id_groupCategory.GetValue()
        },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
        },
        complete: function () {
        }
    });
}

function Providers_BeginCallback(s, e) {
    e.customArgs['id'] = $("#itemId").val();
}

function Providers_EndCallback(s, e) {
    
}

function GridViewProvidersCustomCommandButton_Click(s, e) {
     
}

function OnItemEquivalenceBeginCallback(s, e) {
    e.customArgs["idItemEquivalence"] = s.GetValue();
}

var id_ingredientItemAux = null;

var id_inventoryLineIngredientItemInit = null;
var id_itemTypeIngredientItemInit = null;
var id_itemTypeCategoryIngredientItemInit = null;
var id_ingredientItemInit = null;
var id_metricUnitIngredientItemInit = null;
var id_metricUnitMaxIngredientItemInit = null;
var id_costumerItemInit = null;

function Formulation_OnBeginCallback(s, e) {
    e.customArgs['itemId'] = $("#itemId").val();
    e.customArgs['id'] = Formulation.cpRowKey == undefined ? 0 : Formulation.cpRowKey;
}

function InventoryLineIngredientItem_BeginCallback(s, e) {
      
    e.customArgs['id_inventoryLineIngredientItem'] = id_inventoryLineIngredientItemInit;
}

function InventoryLineIngredientItem_EndCallback(s, e) {
    s.SetValue(id_inventoryLineIngredientItemInit);
    id_itemTypeIngredientItem.PerformCallback();
}

function ComboInventoryLineIngredientItem_SelectedIndexChanged(s, e) {

    id_itemTypeIngredientItemInit = null;
    id_itemTypeCategoryIngredientItemInit = null;
    id_ingredientItemInit = null;
    id_metricUnitIngredientItemInit = null;
    id_metricUnitMaxIngredientItemInit = null;

    id_itemTypeIngredientItem.ClearItems();
    id_itemTypeCategoryIngredientItem.ClearItems();
    id_ingredientItem.ClearItems();
    id_metricUnitIngredientItem.ClearItems();
    id_metricUnitMaxIngredientItem.ClearItems();

    id_itemTypeIngredientItem.PerformCallback();
     
}

function ItemTypeIngredientItem_BeginCallback(s, e) {
    e.customArgs['id_inventoryLineIngredientItem'] = id_inventoryLineIngredientItem.GetValue();
    e.customArgs['id_itemTypeIngredientItem'] = id_itemTypeIngredientItemInit;
}

function ItemTypeIngredientItem_EndCallback(s, e) {
    s.SetValue(id_itemTypeIngredientItemInit);
    id_itemTypeCategoryIngredientItem.PerformCallback();
}

function ComboItemTypeIngredientItem_SelectedIndexChanged(s, e) {

    id_itemTypeCategoryIngredientItemInit = null;
    id_ingredientItemInit = null;
    id_metricUnitIngredientItemInit = null;
    id_metricUnitMaxIngredientItemInit = null;

    id_itemTypeCategoryIngredientItem.ClearItems();
    id_ingredientItem.ClearItems();
    id_metricUnitIngredientItem.ClearItems();
    id_metricUnitMaxIngredientItem.ClearItems();

    id_itemTypeCategoryIngredientItem.PerformCallback();
     
}

function ItemTypeCategoryIngredientItem_BeginCallback(s, e) {
    e.customArgs['id_itemTypeIngredientItem'] = id_itemTypeIngredientItem.GetValue();
    e.customArgs['id_itemTypeCategoryIngredientItem'] = id_itemTypeCategoryIngredientItemInit;
}

function ItemTypeCategoryIngredientItem_EndCallback(s, e) {
    s.SetValue(id_itemTypeCategoryIngredientItemInit);
    id_ingredientItem.PerformCallback();
}

function ComboItemTypeCategoryIngredientItem_SelectedIndexChanged(s, e) {

    id_ingredientItemInit = null;
    id_metricUnitIngredientItemInit = null;
    id_metricUnitMaxIngredientItemInit = null;

    id_ingredientItem.ClearItems();
    id_metricUnitIngredientItem.ClearItems();
    id_metricUnitMaxIngredientItem.ClearItems();

    id_ingredientItem.PerformCallback();
    

}

function IngredientItem_BeginCallback(s, e) {
    e.customArgs['id_inventoryLineIngredientItem'] = id_inventoryLineIngredientItem.GetValue();
    e.customArgs['id_itemTypeIngredientItem'] = id_itemTypeIngredientItem.GetValue();
    e.customArgs['id_itemTypeCategoryIngredientItem'] = id_itemTypeCategoryIngredientItem.GetValue();
    e.customArgs['id_ingredientItem'] = id_ingredientItemInit;
}

function IngredientItem_EndCallback(s, e) {
    if (id_ingredientItemInit !== null)
        s.SetValue(id_ingredientItemInit);
    var _selectItem = s.GetSelectedItem();
    if (_selectItem == undefined || _selectItem == null) {
        masterCode.SetText(null);
        auxCode.SetText(null);
        return;
    }
    var _masterCode = _selectItem.GetColumnText("masterCode");
    var _auxCode = _selectItem.GetColumnText("auxCode");
    masterCode.SetText(_masterCode);
    auxCode.SetText(_auxCode);
    id_metricUnitIngredientItem.PerformCallback();
}

function ComboItemIngredientItem_SelectedIndexChanged(s, e) {
    id_metricUnitIngredientItemInit = null;
    id_metricUnitMaxIngredientItemInit = null;
    var _selectItem = s.GetSelectedItem();
    if (_selectItem == undefined || _selectItem == null) {
        masterCode.SetText(null);
        auxCode.SetText(null);
        return;
    }
    var _masterCode = _selectItem.GetColumnText("masterCode");
    var _auxCode= _selectItem.GetColumnText("auxCode");
    masterCode.SetText(_masterCode);
    auxCode.SetText(_auxCode);
    id_metricUnitIngredientItem.ClearItems();
    id_metricUnitMaxIngredientItem.ClearItems();

    id_metricUnitIngredientItem.PerformCallback();
     
}

function Item_masterCode_Init(s, e) {
    var masterCode = s;
    masterCode.inputElement.tabIndex = -1;

}

function AmountIngredientItem_ValueChanged(s, e) {
    var valueAmountIngredientItem = s.GetValue();
    var valueMetricUnitIngredientItem = id_metricUnitIngredientItem.GetValue();

    amountMaxIngredientItem.SetValue(valueAmountIngredientItem);
    var valueAmountMaxIngredientItem = valueAmountIngredientItem;
    var valueMetricUnitMaxIngredientItem = id_metricUnitMaxIngredientItem.GetValue();

   
    UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem);
}

function MetricUnitIngredientItem_BeginCallback(s, e) {
    e.customArgs['id_ingredientItem'] = id_ingredientItem.GetValue();
    e.customArgs['id_metricUnitIngredientItem'] = id_metricUnitIngredientItemInit;
}

function MetricUnitIngredientItem_EndCallback(s, e) {
    if (id_metricUnitIngredientItemInit !== null) {
        s.SetValue(id_metricUnitIngredientItemInit);
        id_metricUnitMaxIngredientItem.PerformCallback();
    } else {
        var id_ingredientItemAux = id_ingredientItem.GetValue();
        if (id_ingredientItemAux !== null && id_ingredientItemAux !== undefined) {
            $.ajax({
                url: "Item/GetValueMetricUnitIngredientItem",
                type: "post",
                data: {
                    id_ingredientItem: id_ingredientItemAux
                },
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    if (result !== null) {
                        s.SetValue(result.id_metricUnitIngredientItem);
                        id_metricUnitMaxIngredientItem.PerformCallback();
                    }
                },
                complete: function () {
                    hideLoading();
                }
            });
        }
    }
}

function ComboMetricUnitIngredientItem_SelectedIndexChanged(s, e) {
    var valueAmountIngredientItem = amountIngredientItem.GetValue();
    var valueMetricUnitIngredientItem = s.GetValue();
    var valueAmountMaxIngredientItem = amountMaxIngredientItem.GetValue();

    id_metricUnitMaxIngredientItem.SetValue(valueMetricUnitIngredientItem);
    var valueMetricUnitMaxIngredientItem = valueMetricUnitIngredientItem;


    UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem);
}

function AmountMaxIngredientItem_ValueChanged(s, e) {
    var valueAmountIngredientItem = amountIngredientItem.GetValue();
    var valueMetricUnitIngredientItem = id_metricUnitIngredientItem.GetValue();
    var valueAmountMaxIngredientItem = s.GetValue();
    var valueMetricUnitMaxIngredientItem = id_metricUnitMaxIngredientItem.GetValue();


    UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem);
}

function MetricUnitMaxIngredientItem_BeginCallback(s, e) {
    e.customArgs['id_ingredientItem'] = id_ingredientItem.GetValue();
    e.customArgs['id_metricUnitMaxIngredientItem'] = id_metricUnitMaxIngredientItemInit;
}

function MetricUnitMaxIngredientItem_EndCallback(s, e) {
    if (id_metricUnitMaxIngredientItemInit !== null) {
        s.SetValue(id_metricUnitMaxIngredientItemInit);
    } else {
        var id_metricUnitIngredientItemAux = id_metricUnitIngredientItem.GetValue();
        if (id_metricUnitIngredientItemAux !== null && id_metricUnitIngredientItemAux !== undefined) {
            s.SetValue(id_metricUnitIngredientItemAux);
        } else {
            var id_ingredientItemAux = id_ingredientItem.GetValue();
            if (id_ingredientItemAux !== null && id_ingredientItemAux !== undefined) {
                $.ajax({
                    url: "Item/GetValueMetricUnitIngredientItem",
                    type: "post",
                    data: {
                        id_ingredientItem: id_ingredientItemAux
                    },
                    async: false,
                    cache: false,
                    error: function (error) {
                        console.log(error);
                    },
                    beforeSend: function () {
                        showLoading();
                    },
                    success: function (result) {
                        if (result !== null) {
                            s.SetValue(result.id_metricUnitIngredientItem);
                        }
                    },
                    complete: function () {
                        hideLoading();
                    }
                });
            }
        }
        
    }
}

function ComboMetricUnitMaxIngredientItem_SelectedIndexChanged(s, e) {
    var valueAmountIngredientItem = amountIngredientItem.GetValue();
    var valueMetricUnitIngredientItem = id_metricUnitIngredientItem.GetValue();
    var valueAmountMaxIngredientItem = amountMaxIngredientItem.GetValue();
    var valueMetricUnitMaxIngredientItem = s.GetValue(); 


    UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem);
}

function UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem) {
    if (valueAmountIngredientItem == null && valueMetricUnitIngredientItem == null) {
        amountMaxIngredientItem.SetEnabled(true);
        id_metricUnitMaxIngredientItem.SetEnabled(true);
    } else {
    }

    if (valueAmountMaxIngredientItem == null && valueMetricUnitMaxIngredientItem == null) {
        amountIngredientItem.SetEnabled(true);
        id_metricUnitIngredientItem.SetEnabled(true);
    } else {
    }
}

function OnItemIngredientItemInit(s, e) {

    id_ingredientItemAux = s.GetValue();

    var valueAmountIngredientItem = amountIngredientItem.GetValue();
    var valueMetricUnitIngredientItem = id_metricUnitIngredientItem.GetValue();
    var valueAmountMaxIngredientItem = amountMaxIngredientItem.GetValue();
    var valueMetricUnitMaxIngredientItem = id_metricUnitMaxIngredientItem.GetValue();


    UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem);
}

function OnInventoryLineIngredientItemInit(s, e) {
      

    id_inventoryLineIngredientItemInit = s.GetValue();
    id_itemTypeIngredientItemInit = id_itemTypeIngredientItem.GetValue();
    id_itemTypeCategoryIngredientItemInit = id_itemTypeCategoryIngredientItem.GetValue();
    id_ingredientItemInit = id_ingredientItem.GetValue();
    id_metricUnitIngredientItemInit = id_metricUnitIngredientItem.GetValue();
    id_metricUnitMaxIngredientItemInit = id_metricUnitMaxIngredientItem.GetValue();
    id_costumerItemInit = id_costumerItem.GetValue();

    id_inventoryLineIngredientItem.PerformCallback();
     
}

function initIngredientItem(datosIniciales) {

    for (var i = 0; i < id_inventoryLineIngredientItem.GetItemCount() ; i++) {
        var inventoryLineIngredientItem = id_inventoryLineIngredientItem.GetItem(i);
        var into = false;
        for (var j = 0; j < datosIniciales.inventoryLines.length; j++) {
            if (inventoryLineIngredientItem.value == datosIniciales.inventoryLines[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_inventoryLineIngredientItem.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < datosIniciales.inventoryLines.length; i++) {
        var inventoryLineIngredientItem = id_inventoryLineIngredientItem.FindItemByValue(datosIniciales.inventoryLines[i].id);
        if (inventoryLineIngredientItem == null) id_inventoryLineIngredientItem.AddItem(datosIniciales.inventoryLines[i].name, datosIniciales.inventoryLines[i].id);
    }

    for (var i = 0; i < id_itemTypeIngredientItem.GetItemCount() ; i++) {
        var itemTypeIngredientItem = id_itemTypeIngredientItem.GetItem(i);
        var into = false;
        for (var j = 0; j < datosIniciales.itemTypes.length; j++) {
            if (itemTypeIngredientItem.value == datosIniciales.itemTypes[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_itemTypeIngredientItem.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < datosIniciales.itemTypes.length; i++) {
        var itemTypeIngredientItem = id_itemTypeIngredientItem.FindItemByValue(datosIniciales.itemTypes[i].id);
        if (itemTypeIngredientItem == null) id_itemTypeIngredientItem.AddItem(datosIniciales.itemTypes[i].name, datosIniciales.itemTypes[i].id);
    }


    for (var i = 0; i < id_itemTypeCategoryIngredientItem.GetItemCount() ; i++) {
        var itemTypeCategoryIngredientItem = id_itemTypeCategoryIngredientItem.GetItem(i);
        var into = false;
        for (var j = 0; j < datosIniciales.itemTypeCategories.length; j++) {
            if (itemTypeCategoryIngredientItem.value == datosIniciales.itemTypeCategories[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_itemTypeCategoryIngredientItem.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < datosIniciales.itemTypeCategories.length; i++) {
        var itemTypeCategoryIngredientItem = id_itemTypeCategoryIngredientItem.FindItemByValue(datosIniciales.itemTypeCategories[i].id);
        if (itemTypeCategoryIngredientItem == null) id_itemTypeCategoryIngredientItem.AddItem(datosIniciales.itemTypeCategories[i].name, datosIniciales.itemTypeCategories[i].id);
    }
    
    for (var i = 0; i < id_ingredientItem.GetItemCount() ; i++) {
        var ingredientItem = id_ingredientItem.GetItem(i);
        var into = false;
        for (var j = 0; j < datosIniciales.ingredientItems.length; j++) {
            if (ingredientItem.value == datosIniciales.ingredientItems[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_ingredientItem.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < datosIniciales.ingredientItems.length; i++) {
        var ingredientItem = id_ingredientItem.FindItemByValue(datosIniciales.ingredientItems[i].id);
        if (ingredientItem == null) id_ingredientItem.AddItem(datosIniciales.ingredientItems[i].name, datosIniciales.ingredientItems[i].id);
    }

    for (var i = 0; i < id_metricUnitIngredientItem.GetItemCount() ; i++) {
        var metricUnitIngredientItem = id_metricUnitIngredientItem.GetItem(i);
        var into = false;
        for (var j = 0; j < datosIniciales.metricUnits.length; j++) {
            if (metricUnitIngredientItem.value == datosIniciales.metricUnits[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_metricUnitIngredientItem.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < datosIniciales.metricUnits.length; i++) {
        var metricUnitIngredientItem = id_metricUnitIngredientItem.FindItemByValue(datosIniciales.metricUnits[i].id);
        if (metricUnitIngredientItem == null) id_metricUnitIngredientItem.AddItem(datosIniciales.metricUnits[i].code, datosIniciales.metricUnits[i].id);
    }

    for (var i = 0; i < id_metricUnitMaxIngredientItem.GetItemCount() ; i++) {
        var metricUnitMaxIngredientItem = id_metricUnitMaxIngredientItem.GetItem(i);
        var into = false;
        for (var j = 0; j < datosIniciales.metricUnitsMax.length; j++) {
            if (metricUnitMaxIngredientItem.value == datosIniciales.metricUnitsMax[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_metricUnitMaxIngredientItem.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < datosIniciales.metricUnitsMax.length; i++) {
        var metricUnitMaxIngredientItem = id_metricUnitMaxIngredientItem.FindItemByValue(datosIniciales.metricUnitsMax[i].id);
        if (metricUnitMaxIngredientItem == null) id_metricUnitMaxIngredientItem.AddItem(datosIniciales.metricUnitsMax[i].code, datosIniciales.metricUnitsMax[i].id);
    }

}


function ItemTaxationTaxTypeCombo_Init(s, e) {

    $.ajax({
        url: "Item/TaxTypesWithCurrent",
        type: "post",
        data: { id_taxTypeCurrent: s.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            UpdateItemTaxationTaxTypes(result)
        },
        complete: function () {
        }
    });
}

function UpdateItemTaxationTaxTypes(taxTypes) {

    for (var i = 0; i < id_taxType.GetItemCount() ; i++) {
        var taxType = id_taxType.GetItem(i);
        var into = false;
        for (var j = 0; j < taxTypes.length; j++) {
            if (taxType.value == taxTypes[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_taxType.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < taxTypes.length; i++) {
        var taxType = id_taxType.FindItemByValue(taxTypes[i].id);
        if (taxType == null) id_taxType.AddItem(taxTypes[i].name, taxTypes[i].id);
    }

}

function Taxation_OnBeginCallback(s, e) {
    e.customArgs['id_item'] = $("#itemId").val();
}

function ItemTaxationTaxCombo_SelectedIndexChanged(s, e) {
    id_rate.ClearItems();
    ItemTaxationRateCombo_Init(id_rate, e);
}

function ItemTaxationRateCombo_BeginCallback(s, e) {
    e.customArgs['id_taxType'] = id_taxType.GetValue();
}

function ItemTaxationRateCombo_Init(s, e) {
    $.ajax({
        url: "Item/GetRatesByTaxType",
        type: "post",
        data: { id_taxType: id_taxType.GetValue(), id_rate: s.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            UpdateRates(result.rates);
        },
        complete: function () {
        }
    });
}

function UpdateRates(rates) {

    for (var i = 0; i < id_rate.GetItemCount() ; i++) {
        var rate = id_rate.GetItem(i);
        var into = false;
        for (var j = 0; j < rates.length; j++) {
            if (rate.value == rates[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_rate.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < rates.length; i++) {
        var rate = id_rate.FindItemByValue(rates[i].id);
        if (rate == null) id_rate.AddItem(rates[i].name, rates[i].id); 
    }

}

function ItemTaxationRateCombo_SelectedIndexChanged(s, e) {
    percentage.SetValue(null);

    var data = {
        id_rate: s.GetValue()
    };

    $.ajax({
        url: "Item/GetPercentage",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            percentage.SetValue(result.percentage);

        },
        complete: function () {
        }
    });

}

function WeightTypeCombo_Initi(s, e) {
}

function WeightTypeCombo_SelectedIndexChanged(s, e) {
    var data = {
        id_metricUnit: weightType.GetValue()
    };
    $.ajax({
        url: "Item/GetMetricUnitCode",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            UpdateConversionKilosLibrasWeight(result.metricUnitCode);
        },
        complete: function () {
        }
    });
}

function UpdateConversionKilosLibrasWeight(code_MetricUnit) {
    
    if (code_MetricUnit == "Kg") {
        conversionToKilos.SetEnabled(false);
        conversionToKilos.SetText("1");
        conversionToPounds.SetEnabled(true);
    }
    else if (code_MetricUnit == "Lbs") {
        conversionToKilos.SetEnabled(true);
        conversionToPounds.SetEnabled(false);
        conversionToPounds.SetText("1");
    }
    else {
        conversionToKilos.SetEnabled(true);
        conversionToPounds.SetEnabled(true);
    }
}

function AditionalField_OnBeginCallback(s, e) {
    e.customArgs['id'] = $("#itemId").val();
}

function TechnicalSpecifications_OnBeginCallback(s, e) {
    e.customArgs['id_Item'] = $("#itemId").val();
}


function AddNewAttachedDocumentDetail(s, e) {
    gvItemTechnicalSpecificationsAttachedDocuments.AddNewRow();
}

function RemoveAttachedDocumentDetail(s, e) {
}

function RefreshAttachedDocumentDetail(s, e) {
    gvItemTechnicalSpecificationsAttachedDocuments.UnselectRows();
    gvItemTechnicalSpecificationsAttachedDocuments.PerformCallback();
}

function AttachedUploadComplete(s, e) {
    var userData = JSON.parse(e.callbackData);
    $("#guid").val(userData.guid);
    $("#url").val(userData.url);
    TSattachmentName.SetText(userData.filename);
}

var attachmentNameIniAux = null;

function AttachedName_OnInit(s, e) {

    attachmentNameIniAux = s.GetText();

}

function AttachedNameValidate(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Archivo Obligatorio";
    } else {
        var guid = $("#guidAttachment").val();
        if (guid === null || guid.length === 0) {
            e.isValid = false;
            e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
        } else {
            var url = $("#urlAttachment").val();
            if (guid === null || guid.length === 0) {
                e.isValid = false;
                e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
            } else {
                var data = {
                    attachmentNameNew: e.value
                };
                if (data.attachmentNameNew != attachmentNameIniAux) {
                    $.ajax({
                        url: "Item/ItsRepeatedAttachmentDetail",
                        type: "post",
                        data: data,
                        async: false,
                        cache: false,
                        error: function (error) {
                            console.log(error);
                        },
                        beforeSend: function () {
                        },
                        success: function (result) {
                            if (result !== null) {
                                if (result.itsRepeated == 1) {
                                    e.isValid = false;
                                    e.errorText = result.Error;
                                }
                            }
                        },
                        complete: function () {
                        }
                    });
                }
            }
        }
    }

}

function gvItemTechnicalSpecificationsAttachedDocumentsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnUpdate") {
        var valid = true;
        var validAttachmentFormUpLoad = ASPxClientEdit.ValidateEditorsInContainerById("attachment-form-upLoad", null, true);
        if (validAttachmentFormUpLoad) {
            UpdateTabImage({ isValid: true }, "tabAttachedDocument");
        } else {
            UpdateTabImage({ isValid: false }, "tabAttachedDocument");
            valid = false;
        }

        if (valid) {
            gvItemTechnicalSpecificationsAttachedDocuments.UpdateEdit();
        }
    }
}

function AddNewItem(s, e) {
    showPage("Item/FormEditItem", null)
}

function RemoveItems(s, e) {
    gvItems.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            $.ajax({
                url: "Item/DeleteSelectedItems",
                type: "post",
                data: { ids: selectedRows },
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                },
                complete: function () {
                    gvItems.PerformCallback();
                    gvItems.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function CopyItems(s, e) {
    gvItems.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showCopyConfirmationDialog(function () {
            $.ajax({
                url: "Item/CopySelectedItem",
                type: "post",
                data: { idsItem: selectedRows },
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {

                },
                complete: function () {
                    gvItems.PerformCallback();
                    gvItems.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function RefreshGrid(s, e) {
    gvItems.PerformCallback();
}

function Print(s, e) {

}

function importItemsFile(s, e) {
    VerImportarPopupMenu.popupHorizontalAlign = s.name === 'btnImport' ? "RightSides" : "LeftSides";
    VerImportarPopupMenu.ShowAtElement(s.GetMainElement());
    
}

function importProductFile(s, e) {
    var itemName = e.item.name;
    if (itemName === "ImportProductButton") {
        showPage("Item/FormEditItems");
    }
    else if (itemName === "ImportFormulationButton") {
        showPage("Item/FormEditItemFormulations");
    }
    else if (itemName === "ImportEquivalenceButton") {
        showPage("Item/FormEditItemEquivalences");
	}
}

function DownloadTemplateImportItemFormulations(s, e) {
    showLoading();
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='Item/DownloadTemplateImportItemFormulations'></iframe>");
    hideLoading();
}

function onItemFormulationImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItemFormulation").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemFormulationButton.SetEnabled(false);
    } else {
        ImportarItemFormulationButton.SetEnabled(true);
    }
}

function onItemFormulationImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemFormulation").val(userData.id);
        ItemFormulationArchivoEditText.SetText(userData.filename);
    }
    ItemFormulationArchivoEditText.Validate();
}

function createNewFromFileUploadedFormulation() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemFormulationUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItemFormulation").val());
        let alertMessage = "";
        var userData = {
            guidArchivoDatos: obj.guid,
        }
        _successCallback = function (result) {
            if (result.isValid) {
                $('#download-area').empty();

                if (result.HayErrores) {
                    alertMessage = "Existen errores en los datos a importar.";
                }
                else {
                    alertMessage = "Importación realizada exitosamente";
                }

                var downloadArgs = {
                    guidResultado: result.guidResultado,
                    mensajeAlerta: alertMessage,
                };
                var url;
                if (result.HayErrores) {
                    var url = "Item/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "Item/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemFormulationButton.SetEnabled(false);
                ItemFormulationArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "Item/ImportDatosCargaMasivaFormulacion",
            type: "post",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(userData),
            async: true,
            cache: false,
            error: function (error) {
                console.error(error);
                showErrorMessage(error.responseText);
                hideLoading();
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.isValid) {
                    if (result.message) {
                        if (result.HayErrores) {
                            showWarningMessage(result.message);
                        } else {
                            showSuccessMessage(result.message);
                        }
                    }
                    if (_successCallback) {
                        _successCallback(result);
                    }
                }
                if (typeof result.keepLoading === "undefined" || !result.keepLoading) {
                    hideLoading();
                }
            },
            complete: function () {
            }
        });
    }
}

// #region Importación de productos Equivalentes

function DownloadTemplateImportItemEquivalences(s, e) {
    showLoading();
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='Item/DownloadTemplateImportItemEquivalences'></iframe>");
    hideLoading();
}

function onItemEquivalenceImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItemEquivalence").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemEquivalenceButton.SetEnabled(false);
    } else {
        ImportarItemEquivalenceButton.SetEnabled(true);
    }
}

function onItemEquivalenceImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemEquivalence").val(userData.id);
        ItemEquivalenceArchivoEditText.SetText(userData.filename);
    }
    ItemEquivalenceArchivoEditText.Validate();
}

function createNewFromFileUploadedEquivalence() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemEquivalenceUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItemEquivalence").val());
        let alertMessage = "";
        var userData = {
            guidArchivoDatos: obj.guid,
        }
        _successCallback = function (result) {
            if (result.isValid) {
                $('#download-area').empty();

                if (result.HayErrores) {
                    alertMessage = "Existen errores en los datos a importar.";
                }
                else {
                    alertMessage = "Importación realizada exitosamente";
                }

                var downloadArgs = {
                    guidResultado: result.guidResultado,
                    mensajeAlerta: alertMessage,
                };
                var url;
                if (result.HayErrores) {
                    var url = "Item/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "Item/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemEquivalenceButton.SetEnabled(false);
                ItemEquivalenceArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "Item/ImportDatosCargaMasivaEquivalencia",
            type: "post",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(userData),
            async: true,
            cache: false,
            error: function (error) {
                console.error(error);
                showErrorMessage(error.responseText);
                hideLoading();
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.isValid) {
                    if (result.message) {
                        if (result.HayErrores) {
                            showWarningMessage(result.message);
                        } else {
                            showSuccessMessage(result.message);
                        }
                    }
                    if (_successCallback) {
                        _successCallback(result);
                    }
                }

                if (typeof result.keepLoading === "undefined" || !result.keepLoading) {
                    hideLoading();
                }
            },
            complete: function () {
            }
        });
    }
}

// #endregion 



function DownloadTemplateImportItems(s, e) {
    showLoading();
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='Item/DownloadTemplateImportItems'></iframe>");
    hideLoading();
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
        });
    }
    else if (e.buttonID == "btnEditRow") {
        var data = {
            id: s.GetRowKey(e.visibleIndex),
        };
        showPage("Item/FormEditItem", data); 
	}
}

function onItemImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItem").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemButton.SetEnabled(false);
    } else {
        ImportarItemButton.SetEnabled(true);
    }
}

function onItemImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItem").val(userData.id);
        ItemArchivoEditText.SetText(userData.filename);
    }
    ItemArchivoEditText.Validate();
}

function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItem").val());
        let alertMessage = "";
        var userData = {
            guidArchivoDatos: obj.guid,
        }
        _successCallback = function (result) {
            if (result.isValid) {
                $('#download-area').empty();

                if (result.HayErrores) {
                    alertMessage = "Existen errores en los datos a importar.";
                }
                else {
                    alertMessage = "Importación realizada exitosamente";
                }

                var downloadArgs = {
                    guidResultado: result.guidResultado,
                    mensajeAlerta: alertMessage,
                };
                var url;
                if (result.HayErrores) {
                    var url = "Item/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "Item/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemButton.SetEnabled(false);
                ItemArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "Item/ImportDatosCargaMasiva",
            type: "post",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(userData),
            async: true,
            cache: false,
            error: function (error) {
                console.error(error);
                showErrorMessage(error.responseText);
                hideLoading();
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.isValid) {
                    if (result.message) {
                        if (result.HayErrores) {
                            showWarningMessage(result.message);
                        } else {
                            showSuccessMessage(result.message);
                        }
                    }
                    if (_successCallback) {
                        _successCallback(result);
                    }
                }
                if (typeof result.keepLoading === "undefined" || !result.keepLoading) {
                    hideLoading();
                }
            },
            complete: function () {
            }
        });
    }
}

var ShowEditMessage = function (message) {
    if (message !== null && message.length > 0) {
        $("#messageAlert").html(message);

        $(".close").click(function () {
            $(".alert").alert('close');
            $("#messageAlert").empty();
        });
    }
}

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    UpdateTabs();
}

function AddValidation(grid, name) {
    if (grid.editorIDList.indexOf(name) < 0)
        grid.editorIDList.push(name);
}

function RemoveValidation(grid, name) {
    var index = grid.editorIDList.indexOf(name);
    if (index > 0) {
        grid.editorIDList.splice(index, 1);
    }
}

function UpdateTabs() {
    var idItem = $("#itemId").val();
    if (idItem !== null && idItem !== undefined) {
        tabControl.GetTabByName("tabPurchase").SetVisible(isPurchased.GetChecked());
        tabControl.GetTabByName("tabSale").SetVisible(isSold.GetChecked());
        tabControl.GetTabByName("tabInventory").SetVisible(inventoryControl.GetChecked());
        tabControl.GetTabByName("tabFormulation").SetVisible(hasFormulation.GetChecked());
    }
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvItems.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItems.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

        SetElementVisibility("lnkSelectAllRows", gvItems.GetSelectedRowCount() > 0 && gvItems.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelection", gvItems.GetSelectedRowCount() > 0);
    btnRemove.SetEnabled(gvItems.GetSelectedRowCount() > 0 && gvItems.cpPuedeEliminar);
    btnCopy.SetEnabled(gvItems.GetSelectedRowCount() == 1 && gvItems.cpPuedeCopiar);
}

function GetSelectedFilteredRowCount() {
    return gvItems.cpFilteredRowCountWithoutPage + gvItems.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItems.SelectRows();
}

function UnselectAllRows() {
    gvItems.UnselectRows();
}

function ButtonUpdate_Click(s, e) {     
      
    var isBtnToReturn = $("#isBtnToReturn").val();
    var codTariffItem = codeTariffItem.GetText();
    var namTariffItem = nameTariffItem.GetText();
    var idMetUnWeightType = weightType.GetValue();
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);
    if (valid) {

        var itEquivalence = 0;
        if (idItemEquivalence !== undefined) {
            itEquivalence = idItemEquivalence.GetValue();
        }

        var idMetricUnitConversionTmp = 0;

        var item = "id=" + $("#id_item").val() +  "&" + "idMetUnWeightType="
            + idMetUnWeightType + "&" + "codTariffItem=" + codTariffItem + "&"
            + "idItemEquivalence=" + itEquivalence + "&"
            + "namTariffItem=" + namTariffItem + "&"
            + "toReturn=" + $("#isBtnToReturn").val() + "&" + $("#formEditItem").serialize();

        let url = null;
        var esNuevo = false;
        if ($("#id_item").val() === "0") {
            url = "Item/ItemsPartialAddNew";
            esNuevo = true;
        }
        else
        {
            url = "Item/ItemsPartialUpdate";
        }
        

        $.ajax({
            url: url,
            type: "post",
            data: item,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {

                if (esNuevo)
                {
                    let oItemId = $(result).find("#id_item");
                    if (oItemId != null && oItemId.length > 0)
                    {                        
                        $("#id_item").val(oItemId[0].value);                        
                        id_inventoryLine.readOnly = true;
                        masterCode.readOnly = true;
                        id_inventoryLine.SetEnabled(false); 
                        masterCode.SetEnabled(false);
                    }
                }

                if (result.isValid) {
                    showSuccessMessage(result.message);

                    var data = {
                        id: result.idItem,
                    };
                    showPage("Item/FormEditItem", data);
                }
                else {
                    showErrorMessage(result.message);
				}

            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function BtnCancel_Click(s, e) {
    var isBtnToReturn = $("#isBtnToReturn").val();
    if (isBtnToReturn == "True" || isBtnToReturn == "true" || isBtnToReturn == true) {
        btnToReturn_click(s, e);
    } else {
        $.ajax({
            url: "Item/ClearStructureItem",
            type: "post",
            data: null,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                //gvItems.CancelEdit();
                showPage("Item/Index", null);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
    
}

function OnChangeText_masterCode(s, e)
{
    let valorMod = s.GetValue();
    let valorLast = s.prevInputValue
    if (!valorMod.includes(valorLast) && valorMod.length < 3)
    {
        s.SetValue(valorLast);
    }

}
function OnValidate_masterCode(s, e) {
    var _masterCode = s.GetText();
    if ((_masterCode == null) || (_masterCode.length == 0)) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
    else
    {
        var data = {
            masterCode: _masterCode
        };

        $.ajax({
            url: "Item/ValidaRepeatMasterCodeItem",
            type: "post",
            data: data,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {

                if (result.itsRepeated == "1") {
                    e.isValid = false;
                    e.errorText = result.Error;
                }
                
            },
            complete: function () {
            }
        });
    }

}

function onCopyData() {
    var data = {
        id: $("#id_item").val(),
    };
    showPage("Item/CopyEditItem", data);
}

function OnTabPageControlInit(s, e) {
}

function init() {

}

$(function () {
    init();
});