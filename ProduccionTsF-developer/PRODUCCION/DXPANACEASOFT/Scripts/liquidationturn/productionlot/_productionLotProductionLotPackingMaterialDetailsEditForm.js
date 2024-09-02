//var id_salesOrderIniAux = 0;
//var id_salesOrderDetailIniAux = 0;
//var id_itemIniAux = 0;
//var id_warehouseIniAux = 0;
//var id_warehouseLocationIniAux = 0;
var errorMessage = "";
var runningValidation = false;
//Validations

//function OnItemDispatchMaterialValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

function OnItemPackingMaterialValidation(s, e) {
    //gridMessageErrorPackingMaterialDetail.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorPackingMaterialDetail").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    }
}

function OnQuantityPackingMaterialValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cantidad: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() <= 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad: Debe ser mayor a cero.";
        } else {
            errorMessage += "</br>- Cantidad: Debe ser mayor a cero.";
        }
    } else {
        var quantityRequiredForProductionLotAux = gvProductionLotEditFormProductionLotPackingMaterialsDetail.cpEditingRowQuantityRequiredForProductionLot;
        if (s.GetValue() < quantityRequiredForProductionLotAux) {
            e.isValid = false;
            e.errorText = "Valor Incorrecto";
            if (errorMessage == null || errorMessage == "") {
                errorMessage = "- Cantidad: Debe ser mayor e igual a " + quantityRequiredForProductionLotAux + " que es la cantidad mínima que requiere la producción.";
            } else {
                errorMessage += "</br>- Cantidad: Debe ser mayor e igual a " + quantityRequiredForProductionLotAux + " que es la cantidad mínima que requiere la producción.";
            }
        }
    }
    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorPackingMaterialDetail.SetText(msgErrorAux);
        $("#GridMessageErrorPackingMaterialDetail").show();

    }

    if (!runningValidation) {
        ValidateDetailPackingMaterial();
    }

}

function ValidateDetailPackingMaterial() {
    runningValidation = true;
    OnItemPackingMaterialValidation(id_itemPackingMaterial, id_itemPackingMaterial);
    OnQuantityPackingMaterialValidation(quantityPackingMaterial, quantityPackingMaterial);
    runningValidation = false;

}

// EDITOR'S EVENTS

function ItemPackingMaterialCombo_OnInit(s, e) {

    var data = {
        id_itemCurrent: s.GetValue()
    };

    if (data.id_itemCurrent != null) s.SetEnabled(false);

    $.ajax({
        url: "ProductionLot/PackingMaterialDetails",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //id_company
            var arrayFieldStr = [];
            arrayFieldStr.push("masterCode");
            arrayFieldStr.push("name");
            arrayFieldStr.push("ItemInventoryMetricUnitCode");
            UpdateDetailObjects(id_itemPackingMaterial, result.items, arrayFieldStr);

        },
        complete: function () {
            //hideLoading();
        }
    });
}

function DetailsItemPackingMaterialsCombo_SelectedIndexChanged(s, e) {
    DetailsUpdateItemPackingMaterialsInfo({
        id_item: s.GetValue()
    });
}

function DetailsUpdateItemPackingMaterialsInfo(data) {
    if (data.id_item === null) {
        ValidateDetailPackingMaterial();
        return;
    }

    masterCodePackingMaterial.SetText("");
    metricUnitPackingMaterial.SetText("");

    if (id_itemPackingMaterial.GetValue() != null) {

        $.ajax({
            url: "ProductionLot/ItemPackingMaterialDetailsData",
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null) {
                    masterCodePackingMaterial.SetText(result.ItemDetailData.masterCode);
                    metricUnitPackingMaterial.SetText(result.ItemDetailData.metricUnit);
                }
            },
            complete: function () {
                //hideLoading();
                ValidateDetailPackingMaterial();
            }
        });
    } else {
        ValidateDetailPackingMaterial();
    }
}



// EDITOR'S EVENTS
function OnGridViewPackingMaterialDetailsInit(s, e) {
    UpdateTitlePanelPackingMaterialDetails();
}

function UpdateTitlePanelPackingMaterialDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountPackingMaterialDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotEditFormProductionLotPackingMaterialsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotEditFormProductionLotPackingMaterialsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountPackingMaterialDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoPackingMaterials").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsPackingMaterials", gvProductionLotEditFormProductionLotPackingMaterialsDetail.GetSelectedRowCount() > 0 && gvProductionLotEditFormProductionLotPackingMaterialsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionPackingMaterials", gvProductionLotEditFormProductionLotPackingMaterialsDetail.GetSelectedRowCount() > 0);
    }

    btnRemovePackingMaterial.SetEnabled(gvProductionLotEditFormProductionLotPackingMaterialsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountPackingMaterialDetails() {
    return gvProductionLotEditFormProductionLotPackingMaterialsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotEditFormProductionLotPackingMaterialsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewPackingMaterialDetailsSelectionChanged(s, e) {
    UpdateTitlePanelPackingMaterialDetails();
    s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallbackPackingMaterialsDetail);

}

function GetSelectedFieldValuesCallbackPackingMaterialsDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewPackingMaterialDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewPackingMaterialDetailsEndCallback(s, e) {
    UpdateTitlePanelPackingMaterialDetails();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //}
    //if (gv !== gvProductionLotEditFormProductionLotQualityAnalysissDetail) {
    //    if (gv !== gvProductionLotEditFormItemsDetail) {
    //        if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //            s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //        }
    //    } else {
    //        if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
    //            s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
    //        }
    //    }

    //} else {
    //    if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
    //        s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
    //    }
    //}

    //UpdateProductionLotTotals();

}

function gvEditPackingMaterialDetailsClearSelection() {
    gvProductionLotEditFormProductionLotPackingMaterialsDetail.UnselectRows();
}

function gvEditPackingMaterialDetailsSelectAllRows() {
    gvProductionLotEditFormProductionLotPackingMaterialsDetail.SelectRows();
}


