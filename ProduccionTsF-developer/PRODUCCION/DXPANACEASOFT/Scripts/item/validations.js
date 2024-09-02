function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }


    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

var MessageErrorInventoryLine = "Ok";
function UpdateMessageErrorInventoryLine(Message) {

    MessageErrorInventoryLine = Message;

}


function OnInventoryLineValidation(s, e) {

    if (MessageErrorInventoryLine != "Ok") {
        e.isValid = false;
        e.errorText = MessageErrorInventoryLine;
    } else {
        if (e.value === null) {
            e.isValid = false;
            e.errorText = "Campo obligatorio";
        }
    }
}

function OnItemTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

function OnItemTypeCategoryValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

function OnNameValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        var name = String(e.value);
        if (name === "") {
            e.isValid = false;
            e.errorText = "Campo obligatorio";
        } else {
            if (e.value.length > 151) {
                e.isValid = false;
                e.errorText = "M�ximo 150 caracteres";
            }
        }
    }
    
}

function OnBarCodeValidation(s, e) {
    console.log("e.value: ");
    console.log(e.value);

    if (e.value != null && e.value.length != 13 && e.value.length != 0) {
        e.isValid = false;
        e.errorText = "El c�digo de barra debe ser de 13 d�gitos";
    }
    //if (e.value === null) {
    //    e.isValid = false;
    //    e.errorText = "Campo obligatorio";
    //} else {
    //    var name = String(e.value);
    //    if (name === "") {
    //        e.isValid = false;
    //        e.errorText = "Campo obligatorio";
    //    } else {
    //        if (e.value.length > 51) {
    //            e.isValid = false;
    //            e.errorText = "M�ximo 50 caracteres";
    //        }
    //    }
    //}

}

function OnItemMetricTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

var MessageErrorPresentation = "Ok";

function UpdateMessageErrorPresentation(Message) {

    MessageErrorPresentation = Message;

}

function OnItemPresentationValidation(s, e) {
    
    var code_inventoryLineTmp = $("#code_inventoryLine").val();
    if (code_inventoryLineTmp == "PT") {
        if (MessageErrorPresentation != "Ok") {
            e.isValid = false;
            e.errorText = MessageErrorPresentation;
        } else {
            if (e.value === null) {
                e.isValid = false;
                e.errorText = "Campo obligatorio";
            }
        }
    }
    
}


//VALIDATIONS
function OnGroupValidation(s, e) {
    var vsTmp = $("#valSet").val();
    var code_inventoryLineTmp = $("#code_inventoryLine").val();
    if (vsTmp == "YES") {
        if (code_inventoryLineTmp == "PT") {
            if (e.value == null) {
                e.isValid = false;
                e.errorText = "Campo Obligatorio";
            }
        }
    }
    UpdateTabImage(e, "tabGeneral");
}
function OnSubGroupValidation(s, e) {
    var vsTmp = $("#valSet").val();
    var code_inventoryLineTmp = $("#code_inventoryLine").val();
    if (vsTmp == "YES") {
        if (code_inventoryLineTmp == "PT") {
            if (e.value == null) {
                e.isValid = false;
                e.errorText = "Campo Obligatorio";
            }
        }
    }
    UpdateTabImage(e, "tabGeneral");
}
 



function OnFinalProductValidation(s, e) {
    var vsTmp = $("#valSet").val();
    var code_inventoryLineTmp = $("#code_inventoryLine").val();
    if (vsTmp == "YES") {
        if (code_inventoryLineTmp == "PT") {
            if (e.value == null) {
                e.isValid = false;
                e.errorText = "Campo Obligatorio";
            }
        }
    }
    UpdateTabImage(e, "tabGeneral");
}

// PURCHASE VALIDATORS

function OnPurchasePriceValidation(s, e) {
    var tab = tabControl.GetTabByName("tabPurchase");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if(e.value < 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabPurchase");
}

function OnLargestPurchasePriceValidation(s, e) {
    var tab = tabControl.GetTabByName("tabPurchase");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value < 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabPurchase");
}

function OnItemMetricUnitPurchaseValidation(s, e) {
    var tab = tabControl.GetTabByName("tabPurchase");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabPurchase");
}

//TAX VALIDATIONS
function OnCodeTariffItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabTax");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabTax");
}

function OnNameTariffItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabTax");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabTax");
}

// SALE VALIDATIONS

function OnSalePriceValidation(s, e) {
    var tab = tabControl.GetTabByName("tabSale");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabSale");
}

function OnWholeSalePriceValidation(s, e) {
    var tab = tabControl.GetTabByName("tabSale");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value < 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabSale");
}

function OnMetricUnitSaleValidation(s, e) {
    var tab = tabControl.GetTabByName("tabSale");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabSale");
}


//ITEM WEIGHT CONVERSION FREEZEN VALIDATION
function OnItemWeightTypeValidation(s, e) {
    var tab = tabControl.GetTabByName("tabWeight");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabWeight");
}

function OnitemWeightGrossWeightValidation(s, e) {
    var tab = tabControl.GetTabByName("tabWeight");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabWeight");
}

function OnitemWeightNetWeightValidation(s, e) {
    var tab = tabControl.GetTabByName("tabWeight");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabWeight");
}

function OnconversionToKilosValidation(s, e) {
    var tab = tabControl.GetTabByName("tabWeight");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabWeight");
}

function OnconversionToPoundsValidation(s, e) {
    var tab = tabControl.GetTabByName("tabWeight");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabWeight");
}

function OnweightWithGlazeValidation(s, e) {
    var tab = tabControl.GetTabByName("tabWeight");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value < 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabWeight");
}

function OnglazePercentageValidation(s, e) {
    var tab = tabControl.GetTabByName("tabWeight");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value < 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabWeight");
}

// INVENTORY VALIDATION

function OnInventoryControlTypeValidation(s, e) {
    e.isValid = true;
}

function OnMinimumStockValidation(s, e) {
    var tab = tabControl.GetTabByName("tabInventory");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value < 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabInventory");
}

function OnValueValuationValidation(s, e) {
    e.isValid = true;
}

function OnMaximumStockValidation(s, e) {
    var tab = tabControl.GetTabByName("tabInventory");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value < 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabInventory");
}

function OnWarehouseValidation(s, e) {
    var tab = tabControl.GetTabByName("tabInventory");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabInventory");
}

function OnCurrentStockValidation(s, e) {
    var tab = tabControl.GetTabByName("tabInventory");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value < 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabInventory");
}

function OnLocationValidation(s, e) {
    var tab = tabControl.GetTabByName("tabInventory");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabInventory");
}

function OnItemMetricUnitInventoryValidation(s, e) {
    var tab = tabControl.GetTabByName("tabInventory");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabInventory");
}

function OnExpirationDateValidation(s, e) {
    e.isValid = true;
}

// HEADER FORMULATION VALIDATORS

function OnAmountValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabFormulation");
}

function OnItemMetricUnitValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabFormulation");
}

// FORMULATION VALIDATORS

function OnInventoryLineIngredientItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } 

    UpdateTabImage(e, "tabFormulation");
}

function OnItemTypeIngredientItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabFormulation");
}

function OnItemTypeCategoryIngredientItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabFormulation");
}

function OnItemIngredientItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value !== id_ingredientItemInit || id_costumerItem.GetValue() !== id_costumerItemInit) {
        $.ajax({
            url: "Item/ItsRepeatedIngredientItemAndCostumerItem",
            type: "post",
            data: {
                id_ingredientItem: e.value,
                id_costumerItem: id_costumerItem.GetValue()
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
                    if (result.itsRepeated == 1) {
                        e.isValid = false;
                        e.errorText = result.Error;
                    }
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }

    UpdateTabImage(e, "tabFormulation");
}

function OnMetricUnitIngredientItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (!id_metricUnitIngredientItem.GetEnabled()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabFormulation");
}

function OnAmountIngredientItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } if (!amountIngredientItem.GetEnabled()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabFormulation");
}

function OnMetricUnitMaxIngredientItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (!id_metricUnitMaxIngredientItem.GetEnabled()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    UpdateTabImage(e, "tabFormulation");
}

function OnAmountMaxIngredientItemValidation(s, e) {
    var tab = tabControl.GetTabByName("tabFormulation");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } if (!amountMaxIngredientItem.GetEnabled()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor incorrecto";
    }

    UpdateTabImage(e, "tabFormulation");
}