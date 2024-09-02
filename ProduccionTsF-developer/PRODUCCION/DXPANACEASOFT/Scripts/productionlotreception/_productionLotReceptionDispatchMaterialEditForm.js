
var errorMessage = "";
var runningValidation = false;

// VALIDATIONS

function OnWarehouseMaterialsDetailValidation(s, e) {
    //gridMessageErrorMaterialsDetail.SetText(result.Message);
    errorMessage = "";

    $("#GridMessageErrorMaterialsDetail").hide();
    
    var arrivalDestinationQuantityAux =  parseFloat(arrivalDestinationQuantity.GetValue())

    if (arrivalDestinationQuantityAux > 0) {
        if (s.GetValue() === null) {
            e.isValid = false;
            e.errorText = "Campo Obligatorio";
            errorMessage = "- Bodega: Es obligatoria, por tener una cantidad recibida.";
        }
    }
    
}

function OnWarehouseLocationMaterialsDetailValidation(s, e) {

    var arrivalDestinationQuantityAux =  parseFloat(arrivalDestinationQuantity.GetValue())

    if (arrivalDestinationQuantityAux > 0) {
        if (s.GetValue() === null) {
            e.isValid = false;
            e.errorText = "Campo Obligatorio";
            if (errorMessage == null || errorMessage == "") {
                errorMessage = "- Ubicación: Es obligatoria, por tener una cantidad recibida.";
            } else {
                errorMessage += "</br>- Ubicación: Es obligatoria, por tener una cantidad recibida.";
            }
        }
    }

    if (!runningValidation) {
        ValidateMaterialsDetail();
    }
}

function OnSendedDestinationQuantityValidation(s, e) {

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Despachada Destino: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Despachada Destino: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Despachada Destino: Es incorrecta.";
        } else {
            errorMessage += "</br>- Cantidad Despachada Destino: Es incorrecta.";
        }
    }

    if (!runningValidation) {
        ValidateMaterialsDetail();
    }
}

function OnArrivalDestinationQuantityValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Recibida: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Recibida: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Recibida: Es incorrecta.";
        } else {
            errorMessage += "</br>- Cantidad Recibida: Es incorrecta.";
        }
    } else if (parseFloat(s.GetValue()) > parseFloat(sourceExitQuantity.GetValue())) {
        e.isValid = false;
        e.errorText = "Cantidad Recibida no puede ser mayor que Cantida Salida Origen";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Recibida: No puede ser mayor que Cantida Salida Origen.";
        } else {
            errorMessage += "</br>- Cantidad Recibida: No puede ser mayor que Cantida Salida Origen.";
        }
    } else if (parseFloat(s.GetValue()) != (parseFloat(arrivalGoodCondition.GetValue()) + parseFloat(arrivalBadCondition.GetValue()))) {
        e.isValid = false;
        e.errorText = "Cantidad Recibida debe ser igual a Cantida en Buen Estado más Cantidad en Mal Estado";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Recibida: Debe ser igual a Cantida en Buen Estado más Cantidad en Mal Estado.";
        } else {
            errorMessage += "</br>- Cantidad Recibida: Debe ser igual a Cantida en Buen Estado más Cantidad en Mal Estado.";
        }
    }

    if (!runningValidation) {
        ValidateMaterialsDetail();
    }

}

function OnArrivalGoodConditionValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Buen Estado: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Buen Estado: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Buen Estado: Es incorrecta.";
        } else {
            errorMessage += "</br>- Cantidad Buen Estado: Es incorrecta.";
        }
    }

    if (!runningValidation) {
        ValidateMaterialsDetail();
    }
}

function OnArrivalBadConditionValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Mal Estado: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Mal Estado: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Mal Estado: Es incorrecta.";
        } else {
            errorMessage += "</br>- Cantidad Mal Estado: Es incorrecta.";
        }
    }

    if (!runningValidation) {
        ValidateMaterialsDetail();
    }

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorMaterialsDetail.SetText(msgErrorAux);
        $("#GridMessageErrorMaterialsDetail").show();

    }
}

function ValidateMaterialsDetail() {
    runningValidation = true;
    OnWarehouseMaterialsDetailValidation(id_warehouseMaterial, id_warehouseMaterial);
    OnWarehouseLocationMaterialsDetailValidation(id_warehouseLocationMaterial, id_warehouseLocationMaterial);
    OnSendedDestinationQuantityValidation(sendedDestinationQuantity, sendedDestinationQuantity);
    OnArrivalDestinationQuantityValidation(arrivalDestinationQuantity, arrivalDestinationQuantity);
    OnArrivalGoodConditionValidation(arrivalGoodCondition, arrivalGoodCondition);
    OnArrivalBadConditionValidation(arrivalBadCondition, arrivalBadCondition);
    runningValidation = false;

}

function ArrivalDestinationQuantity_ValueChanged(s, e) {
    var notReceivedQuantityAux = parseFloat(sourceExitQuantity.GetValue()) - parseFloat(arrivalDestinationQuantity.GetValue());
    notReceivedQuantity.SetValue(notReceivedQuantityAux);
    //ValidateMaterialsDetail();
}

// EDITOR'S EVENTS

function UpdateMaterialsDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    metricUnitMaterial.SetText("");
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnitMaterial").SetText("");
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(null);// 
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(null);// SetValue("");

    if (id_itemMaterial != null) {

        $.ajax({
            url: "ProductionLotReception/ItemDetailData",
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
                    metricUnitMaterial.SetText(result.metricUnit);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemMaterialsDetailCombo_SelectedIndexChanged(s, e) {
    UpdateMaterialsDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

function ItemMaterialsDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionMaterialDetails",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            for (var i = 0; i < id_itemMaterial.GetItemCount() ; i++) {
                var item = id_itemMaterial.GetItem(i);
                if (result.indexOf(item.value) >= 0) {
                    id_itemMaterial.RemoveItem(i);
                    i = -1;
                }
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ComboItem_Init(s, e) {
    var data = {
        id_item: s.GetValue()
    };

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionItemData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            metricUnitMaterial.SetText("");
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                metricUnitMaterial.SetText(result.metricUnit);
            }
            else {
                metricUnitMaterial.SetText("");
            }
        },
        complete: function () {
            //hideLoading();
        }
    });

    ItemCombo_OnInit(s, e);
}

function ItemCombo_OnInit(s, e) {
    //store actual filtering method and override
    var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
    s.filterStrategy.FilteringOnClient = function () {
        //create a new format string for all list box columns. you could skip this bit and just set
        //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
        //columns 1 and 3
        var lb = this.GetListBoxControl();
        var filterTextFormatStringItems = [];
        for (var i = 0; i < lb.columnFieldNames.length; i++) {
            filterTextFormatStringItems.push('{' + i + '}');
        }
        var filterTextFormatString = filterTextFormatStringItems.join(' ');

        //store actual format string and override with one for all columns
        var actualTextFormatString = lb.textFormatString;
        lb.textFormatString = filterTextFormatString;

        //call actual filtering method which will now work on our temporary format string
        actualFilteringOnClient.apply(this);

        //restore original format string
        lb.textFormatString = actualTextFormatString;
    };
}

function UpdateWarehouseLocations(warehouseLocations) {

    for (var i = 0; i < id_warehouseLocationMaterial.GetItemCount() ; i++) {
        var warehouseLocation = id_warehouseLocationMaterial.GetItem(i);
        var into = false;
        for (var j = 0; j < warehouseLocations.length; j++) {
            if (warehouseLocation.value == warehouseLocations[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_warehouseLocationMaterial.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < warehouseLocations.length; i++) {
        var warehouseLocation = id_warehouseLocationMaterial.FindItemByValue(warehouseLocations[i].id);
        if (warehouseLocation == null) id_warehouseLocationMaterial.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
    }

}

function ComboWarehouseMaterials_SelectedIndexChanged(s, e) {

    id_warehouseLocationMaterial.SetValue(null);
    id_warehouseLocationMaterial.ClearItems();
    var data = {
        id_warehouse: s.GetValue()//,
    };

    $.ajax({
        url: "ProductionLotReception/UpdateProductionLotReceptionWarehouseLocation",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_priceList.ClearItems();
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                var arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_warehouseLocationMaterial, result.warehouseLocations, arrayFieldStr);
                //UpdateWarehouseLocations(result.warehouseLocations);
            }
            //else {
            //    id_priceList.ClearItems();
            //}
        },
        complete: function () {
            //hideLoading();
            ValidateMaterialsDetail();
        }
    });
}

function ComboWarehouseLocationMaterials_Init(s, e) {
    var data = {
        id_warehouse: id_warehouseMaterial.GetValue(),
        id_warehouseLocation: s.GetValue()//,
    };
    $.ajax({
        url: "ProductionLotReception/GetProductionLotReceptionWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_warehouseLocationMaterial, [], arrayFieldStr);
            //UpdateWarehouseLocations([]);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_warehouseLocationMaterial, result.warehouseLocations, arrayFieldStr);
            //UpdateWarehouseLocations(result.warehouseLocations);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// EDITOR'S EVENTS

function OnGridViewMaterialDetailsInit(s, e) {
    UpdateTitlePanelMaterialDetails();
}

function UpdateTitlePanelMaterialDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountMaterialDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotReceptionEditFormMaterialsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotReceptionEditFormMaterialsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountMaterialDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoMaterials").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsMaterials", gvProductionLotReceptionEditFormMaterialsDetail.GetSelectedRowCount() > 0 && gvProductionLotReceptionEditFormMaterialsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionMaterials", gvProductionLotReceptionEditFormMaterialsDetail.GetSelectedRowCount() > 0);
    }

    //btnRemoveDispatchMaterials.SetEnabled(gvProductionLotReceptionEditFormMaterialsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountMaterialDetails() {
    return gvProductionLotReceptionEditFormMaterialsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotReceptionEditFormMaterialsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewMaterialsDetailSelectionChanged(s, e) {
    UpdateTitlePanelMaterialDetails();
    s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallbackMaterialsDetail);

}

function GetSelectedFieldValuesCallbackMaterialsDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewMaterialDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewMaterialDetailsEndCallback(s, e) {
    UpdateTitlePanelMaterialDetails();
    if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
        s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    }
    //if (gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
    //    if (gv !== gvProductionLotReceptionEditFormMaterialsDetail) {
    //        if (s.GetEditor("id_Material") !== null && s.GetEditor("id_Material") !== undefined) {
    //            s.GetEditor("id_Material").SetEnabled(customCommand === "ADDNEWROW");
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

function gvEditMaterialDetailsClearSelection() {
    gvProductionLotReceptionEditFormMaterialsDetail.UnselectRows();
}

function gvEditMaterialDetailsSelectAllRows() {
    gvProductionLotReceptionEditFormMaterialsDetail.SelectRows();
}