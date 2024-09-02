var errorMessage = "";
var runningValidation = false;
var codeDocumentType = $("#codeDocumentType").val();

var unitPriceMoveAux = 0;
var id_metrictUnitMoveAux = 0;

var lotAux = "";
var lotCliAux = "";

var id_subCostCenterDetailInit = null;

function OnItemDetailBeginCallback(s, e) {
    // 
    if (s != undefined) {
        if (s.name.startsWith('ItemDetail')) {
            var index = parseInt(s.name.substr("ItemDetail".length));
            e.customArgs["indice"] = index;
            indexDetail = index;
        }
    }
}

function OnItemDetailEndCallback(s, e) {
}

// VALIDATION
function OnItemValidation(s, e) {
    errorMessage = "";
    $("#GridMessageErrorMaterialsDetail").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    }
}

function OnWarehouseDetailValidation(s, e) {
    var caption = (codeDocumentType == "34") ? "Bodega Ingreso" : ((codeDocumentType = "32") ? "Bodega Egreso" : ("Bodega"));
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- " + caption + ": Es obligatoria.";
        } else {
            errorMessage += "</br>- " + caption + ": Es obligatoria.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnWarehouseLocationDetailValidation(s, e) {

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Ubicación: Es obligatoria.";
        } else {
            errorMessage += "</br>- Ubicación: Es obligatoria.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnWarehouseEntryValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Bodega Ingreso: Es obligatoria.";
        } else {
            errorMessage += "</br>- Bodega Ingreso: Es obligatoria.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnAmountValidation(s, e) {
    //codeDocumentType = $("#codeDocumentType").val();
    var id_inventoryMove = $("#id_inventoryMove").val();
    var caption = (codeDocumentType == "03" || codeDocumentType == "04" || codeDocumentType == "34") ? (id_inventoryMove == 0) ? "Cantidad a Ingresar" : "Cantidad a Ingresada" :
                  ((codeDocumentType == "05" || codeDocumentType == "32") ? ((id_inventoryMove == 0) ? "Cantidad a Egresar" : "Cantidad a Egresada") : "");
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- " + caption + ": Es obligatoria.";
        } else {
            errorMessage += "</br>- " + caption + ": Es obligatoria.";
        }
    } else if (s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- " + caption + ": Valor Incorrecto.";
        } else {
            errorMessage += "</br>- " + caption + ": Valor Incorrecto.";
        }
    } else {
        try {
            var entryAmount = parseFloat(s.GetValue());
            //console.log(entryAmount);
            if (entryAmount < 0) {
                e.isValid = false;
                e.errorText = "La cantidad no puede ser menor a 0";
                if (errorMessage == null || errorMessage == "") {
                    errorMessage = "- " + caption + ": La cantidad no puede ser menor a 0.";
                } else {
                    errorMessage += "</br>- " + caption + ": La cantidad no puede ser menor a 0.";
                }
            }

        } catch (e) {
            e.isValid = false;
            e.errorText = "Valor Incorrecto";
            if (errorMessage == null || errorMessage == "") {
                errorMessage = "- " + caption + ": Valor Incorrecto.";
            } else {
                errorMessage += "</br>- " + caption + ": Valor Incorrecto.";
            }
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnMetricUnitMoveValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- UM Mov.: Es obligatoria.";
        } else {
            errorMessage += "</br>- UM Mov.: Es obligatoria.";
        }
    }

    var isEntry = codeDocumentType == "03" || codeDocumentType == "04" || codeDocumentType == "34";
    if (isEntry) {
        lotAux = lotNumber.GetText();
        lotCliAux = lotInternalNumber.GetText();
    }

    //if (errorMessage != null && errorMessage != "") {
    //    var msgErrorAux = ErrorMessage(errorMessage);
    //    gridMessageErrorMaterialsDetail.SetText(msgErrorAux);
    //    $("#GridMessageErrorMaterialsDetail").show();

    //}

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnLotNumberValidation(s, e) {
    var valueAux = s.GetValue();
    var withLotSystemAux = $("#withLotSystem").val();
    if ((withLotSystemAux == true || withLotSystemAux == "True" || withLotSystemAux == "true") && (valueAux === null || valueAux.trim() == "")) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Lote Sistema: Es obligatorio.";
        } else {
            errorMessage += "</br>- Lote Sistema: Es obligatorio.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnLotInternalNumberValidation(s, e) {
    var valueAux = s.GetValue();
    var withLotCustomerAux = $("#withLotCustomer").val();
    if ((withLotCustomerAux == true || withLotCustomerAux == "True" || withLotCustomerAux == "true") && (valueAux === null || valueAux.trim() == "")) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Lote Cliente: Es obligatorio.";
        } else {
            errorMessage += "</br>- Lote Cliente: Es obligatorio.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnSubCostCenterDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- SubCentro Costo: Es obligatorio.";
        } else {
            errorMessage += "</br>- SubCentro Costo: Es obligatorio.";
        }
    }

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorMaterialsDetail.SetText(msgErrorAux);
        $("#GridMessageErrorMaterialsDetail").show();

    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnCostCenterDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Centro Costo: Es obligatorio.";
        } else {
            errorMessage += "</br>- Centro Costo: Es obligatorio.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function ValidateDetail() {
    var _index = gridViewMoveDetails.cpRowIndex;
    var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
    var objItem = window["ItemDetail" + key];

    runningValidation = true;
    if (objItem != undefined && objItem != null) {
        OnItemValidation(objItem, objItem);
    }
    
    OnWarehouseDetailValidation(id_warehouseDetail, id_warehouseDetail);
    OnWarehouseLocationDetailValidation(id_warehouseLocationDetail, id_warehouseLocationDetail);
    codeDocumentType = $("#codeDocumentType").val();
    //console.log("codeDocumentType: " + codeDocumentType);
    if (codeDocumentType == "32")//Egreso Por Transferencia
    {
        OnWarehouseEntryValidation(id_warehouseEntry, id_warehouseEntry);
    }
    OnAmountValidation(amountMove, amountMove);
    OnMetricUnitMoveValidation(id_metricUnitMove, id_metricUnitMove);
    if (codeDocumentType == "03" || codeDocumentType == "04" || codeDocumentType == "34")//Egreso Por Transferencia
    {
        OnLotNumberValidation(lotNumber, lotNumber);
        OnLotInternalNumberValidation(lotInternalNumber, lotInternalNumber);
    }
    OnCostCenterDetailValidation(id_costCenterDetail, id_costCenterDetail);
    OnSubCostCenterDetailValidation(id_subCostCenterDetail, id_subCostCenterDetail);

    runningValidation = false;

}

// TRASNFER MOVE

function OnAmountValueChanged(s, e) {
    OnAmountOrPriceMoveValueChanged();
    //try {
    //    var amountMoveAux = parseFloat(s.GetValue());
    //    var balanceCostAux = amountMoveAux * unitPriceMoveAux;
    //    balanceCostAux = isNaN(balanceCostAux) ? 0 : balanceCostAux;
    //    balanceCost.SetValue(balanceCostAux);
    //} catch (e) {
    //    balanceCost.SetValue(0);
    //}
    //gridViewMoveDetails.SetEditValue('entryAmount', s.GetValue());
    //gridViewMoveDetails.SetEditValue('exitAmount', s.GetValue());
}

function GetValueUnitPriceMove() {
    try {
        return unitPriceMove.GetValue();
    } catch (e) {
        return gridViewMoveDetails.cpEditingRowUnitPriceMove;

    }
}

function SetValueUnitPriceMove(value) {
    try {
        unitPriceMove.SetValue(value);
    } catch (e) {
        gridViewMoveDetails.cpEditingRowUnitPriceMove = value;

    }
}

function OnUnitPriceMoveValueChanged(s, e) {
    unitPriceMoveAux = GetValueUnitPriceMove();// unitPriceMove.GetValue();
    id_metrictUnitMoveAux = id_metricUnitMove.GetValue();
    OnAmountOrPriceMoveValueChanged();
}

function SetValueBalanceCost(value) {
    try {
        balanceCost.SetValue(value);
    } catch (e) {
        gridViewMoveDetails.cpEditingRowBalanceCost = value;

    }
}

function OnAmountOrPriceMoveValueChanged() {
    try {
        var amountMoveAux = parseFloat(amountMove.GetValue());
        var balanceCostAux = amountMoveAux * unitPriceMoveAux;
        balanceCostAux = isNaN(balanceCostAux) ? 0 : balanceCostAux;
        SetValueBalanceCost(balanceCostAux);//balanceCost.SetValue(balanceCostAux);
    } catch (e) {
        SetValueBalanceCost(0);//balanceCost.SetValue(0);
    }
}

function OnInitCostCenterCombo(s, e) {
    id_subCostCenterDetailInit = id_subCostCenterDetail.GetValue();
    id_subCostCenterDetail.PerformCallback();
}

function OnCostCenterCombo_SelectedIndexChanged(s, e) {
    id_subCostCenterDetailInit = null;
    id_subCostCenterDetail.PerformCallback();
}

function InventoryMoveSubCostCenter_BeginCallback(s, e) {
    e.customArgs["id_costCenterDetail"] = id_costCenterDetail.GetValue();
    e.customArgs["id_subCostCenterDetail"] = id_subCostCenterDetailInit;
}

function InventoryMoveSubCostCenter_EndCallback(s, e) {

    id_subCostCenterDetail.SetValue(id_subCostCenterDetailInit);
}


// SELECTION

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

//Grid View

function OnGridViewInit(s, e) {
    UpdateTitlePanelDetails();
}

function UpdateTitlePanelDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountDetails();

    var text = "Total de elementos seleccionados: <b>" + gridViewMoveDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gridViewMoveDetails.GetSelectedRowCount() - GetSelectedFilteredRowCountDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfo").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRows", gridViewMoveDetails.GetSelectedRowCount() > 0 && gridViewMoveDetails.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelection", gridViewMoveDetails.GetSelectedRowCount() > 0);
    }

    btnRemoveDetail.SetEnabled(gridViewMoveDetails.GetSelectedRowCount() > 0);

    var codeDocumentType = $("#codeDocumentType").val();
    //console.log("codeDocumentType: " + codeDocumentType);

    if (codeDocumentType == "04" || codeDocumentType == "34")//04: Ingreso x Orden de Compra y 34: Ingreso Por Transferencia
    {
        btnRemoveDetail.SetEnabled(false);
        btnNewDetail.SetEnabled(false);
    }
}

function GetSelectedFilteredRowCountDetails() {
    return gridViewMoveDetails.cpFilteredRowCountWithoutPage +
           gridViewMoveDetails.GetSelectedKeysOnPage().length;
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanelDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbacksDetail);

}

function GetSelectedFieldValuesCallbacksDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewBeginCallback(s, e) {
    customCommand = e.command;
    e.customArgs['code'] = $("#codeDocumentType").val();
    e.customArgs['lotNumber'] = lotAux;
    e.customArgs['lotInternalNumber'] = lotCliAux;
    e.customArgs["errorMessage"] = errorMessage;
    e.customArgs["cpEditingRowUnitPriceMove"] = gridViewMoveDetails.cpEditingRowUnitPriceMove;
    e.customArgs["cpEditingRowBalanceCost"] = gridViewMoveDetails.cpEditingRowBalanceCost;
    // 
    if (e.command == "UPDATEROW" || e.command == "UPDATEEDIT") {
        if (indexDetail > 0) {
            if (window["ItemDetail" + indexDetail] != undefined) {
                e.customArgs["id_item2"] = window["ItemDetail" + indexDetail].GetValue();
            }
        } else if (indexDetail == 0) {
            var _index = gridViewMoveDetails.cpRowIndex;
            var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
            e.customArgs["id_item2"] = window["ItemDetail" + key].GetValue();
        }
    }
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanelDetails();
}

function gvEditDetailsClearSelection() {
    gridViewMoveDetails.UnselectRows();
}

function gvEditDetailsSelectAllRows() {
    gridViewMoveDetails.SelectRows();
}

// EDITOR'S EVENTS

function ItemCombo_OnInit(s, e) {
    unitPriceMoveAux = GetValueUnitPriceMove();// unitPriceMove.GetValue();
    id_metrictUnitMoveAux = id_metricUnitMove.GetValue();
    id_itemIniAux = s.GetValue();
    id_warehouseIniAux = id_warehouseDetail.GetValue();
    id_warehouseLocationIniAux = id_warehouseLocationDetail.GetValue();
    codeDocumentType = $("#codeDocumentType").val();

    var isExit = codeDocumentType == "05" || codeDocumentType == "32";

    var data = {
        id_itemCurrent: s.GetValue(),
        codeDocumentType: codeDocumentType,
        id_metricUnitMove: id_metricUnitMove.GetValue(),
        id_warehouse: id_warehouseDetail.GetValue(),
        id_warehouseLocation: id_warehouseLocationDetail.GetValue(),
        id_lot: isExit ? id_lot.GetValue() : 0
    };
    var id_purchaseOrderDetail = gridViewMoveDetails.cpEditingRowPurchaseOrderDetail;
    var id_inventoryMoveExit = gridViewMoveDetails.cpEditingRowInventoryMoveExit;

    if (data.id_itemCurrent != null && (id_purchaseOrderDetail != 0 || id_inventoryMoveExit != 0)) s.SetEnabled(false);

    $.ajax({
        url: "InventoryMoveTransfer/InventoryMoveItemDetails",
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
            masterCode.SetText(result.masterCode);

            //id_company
            var arrayFieldStr = [];
            arrayFieldStr.push("masterCode");
            arrayFieldStr.push("name");
            //arrayFieldStr.push("ItemInventoryMetricUnitCode");

            var _index = gridViewMoveDetails.cpRowIndex;
            var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
            var objItem = window["ItemDetail" + key]

            //if (objItem != null && objItem != undefined) {
            //    UpdateDetailObjects(objItem, result.items, arrayFieldStr);
            //}

            arrayFieldStr = [];
            arrayFieldStr.push("code");
            UpdateDetailObjects(id_metricUnitMove, result.metricUnits, arrayFieldStr);
            //id_metricUnitMove.SetValue(result.id_metricUnitMove);

            arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_warehouseLocationDetail, result.warehouseLocations, arrayFieldStr);

            

            if (isExit) {
                arrayFieldStr = [];
                arrayFieldStr.push("number");
                UpdateDetailObjects(id_lot, result.lots, arrayFieldStr);

                remainingBalance.SetValue(result.remainingBalance);
            }
            

        },
        complete: function () {
            //hideLoading();
            MetricUnitMoveCombo_SelectedIndexChanged(id_metricUnitMove, e);
        }
    });
}

function DetailsItemsCombo_SelectedIndexChanged(s, e) {
    DetailsUpdateItemInfo({
        id_item: s.GetValue()
    });
}

function DetailsUpdateItemInfo(data) {
    // 
    masterCode.SetText("");
    metricUnitInventoryPurchase.SetText("");

    id_metricUnitMove.SetValue(null);
    SetValueUnitPriceMove(0);

    unitPriceMoveAux = 0;
    id_metrictUnitMoveAux = 0;

    var _index = gridViewMoveDetails.cpRowIndex;
    var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
    var id_itemTmp = window["ItemDetail" + key].GetValue();

    codeDocumentType = $("#codeDocumentType").val();

    var isExit = codeDocumentType == "05" || codeDocumentType == "32";

    if (isExit) {
        id_lot.ClearItems();
        id_lot.SetValue(null);

        remainingBalance.SetValue(0);
    }

    if (data.id_item === null) {
        ValidateDetail();
        return;
    }

    if (id_itemTmp != null) {

        $.ajax({
            url: "InventoryMoveTransfer/ItemDetailsData",
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
                    masterCode.SetText(result.masterCode);
                    metricUnitInventoryPurchase.SetText(result.metricUnitInventoryPurchase);
                    //id_metricUnitInventoryPurchase.SetValue(result.id_metricUnitInventoryPurchase);

                    var arrayFieldStr = [];
                    arrayFieldStr.push("code");
                    UpdateDetailObjects(id_metricUnitMove, result.metricUnits, arrayFieldStr);
                    id_metricUnitMove.SetValue(result.id_metricUnitMove);
                    id_metrictUnitMoveAux = result.id_metricUnitMove;

                    unitPriceMove.SetValue(result.unitPriceMove);
                    unitPriceMoveAux = result.unitPriceMove;
   
                    OnAmountOrPriceMoveValueChanged();

                    //quantityTotal.SetValue(result.quantityTotal);
                    //id_metricUnitPresentation.SetValue(result.id_metricUnitPresentation);

                    id_warehouseDetail.SetValue(result.id_warehouse);

                    arrayFieldStr = [];
                    arrayFieldStr.push("name");
                    UpdateDetailObjects(id_warehouseLocationDetail, result.warehouseLocations, arrayFieldStr);
                    id_warehouseLocationDetail.SetValue(result.id_warehouseLocation);

                    if (isExit) {
                        arrayFieldStr = [];
                        arrayFieldStr.push("number");
                        UpdateDetailObjects(id_lot, result.lots, arrayFieldStr);

                        remainingBalance.SetValue(result.remainingBalance);

                        unitPriceMove.SetValue(result.averagePrice);
                        unitPriceMoveAux = result.averagePrice;

                        //id_metricUnitMove.SetValue(result.id_metricUnitMove);
                        //id_metrictUnitMoveAux = result.id_metricUnitMove;

                        OnAmountOrPriceMoveValueChanged();
                    }
                }
            },
            complete: function () {
                //hideLoading();
                ValidateDetail();
            }
        });
    } else {
        ValidateDetail();
    }
}

function OnInitWarehouseExit(s, e) {
    var data = s.GetValue();
    if (data === null) {
        //return;
        var id_warehouseExit = $("#id_warehouseExit").val();//id_warehouseExit.GetValue();
        console.log(id_warehouseExit);
        s.SetValue(id_warehouseExit);
    }
    
}

function OnInitWarehouseLocationExit(s, e) {
    var data = s.GetValue();
    if (data === null) {
        //return;
        var id_warehouseLocationExit = $("#id_warehouseLocationExit").val();// id_warehouseLocationExit.GetValue();
        console.log(id_warehouseLocationExit);
        s.SetValue(id_warehouseLocationExit);
    }
    
}

function OnInitWarehouseEntry(s, e) {
    var data = s.GetValue();
    if (data === null) {
        //return;
        var id_warehouseEntry = $("#id_warehouseEntry").val();//id_warehouseEntry.GetValue();
        console.log(id_warehouseEntry);
        s.SetValue(id_warehouseEntry);
    }

}

function OnInitWarehouseLocationEntry(s, e) {
    var data = s.GetValue();
    if (data === null) {
        //return;
        var id_warehouseLocationEntry = $("#id_warehouseLocationEntry").val();//id_warehouseLocationEntry.GetValue();
        console.log(id_warehouseLocationEntry);
        s.SetValue(id_warehouseLocationEntry);
    }

}

function OnWarehouseDetailCombo_SelectedIndexChanged(s, e) {

    id_warehouseLocationDetail.SetValue(null);
    id_warehouseLocationDetail.ClearItems();

    codeDocumentType = $("#codeDocumentType").val();

    var isExit = codeDocumentType == "05" || codeDocumentType == "32";

    if (isExit) {

        id_lot.ClearItems();
        id_lot.SetValue(null);

        remainingBalance.SetValue(0);

        SetValueUnitPriceMove(0);
        //unitPriceMove.SetValue(0);
        unitPriceMoveAux = 0;

        id_metrictUnitMoveAux = 0;
        id_metricUnitMove.SetValue(null);

        OnAmountOrPriceMoveValueChanged();
    }

    var data = {
        id_warehouse: s.GetValue()//,
    };

    $.ajax({
        url: "InventoryMoveTransfer/UpdateWarehouseLocation",
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
                UpdateDetailObjects(id_warehouseLocationDetail, result.warehouseLocations, arrayFieldStr);
                //UpdateProductionLotLiquidationDetailWarehouseLocations(result.warehouseLocations);
            }
            //else {
            //    id_priceList.ClearItems();
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });

}

function OnWarehouseLocationCombo_SelectedIndexChanged(s, e) {
    
    var _index = gridViewMoveDetails.cpRowIndex;
    var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
    var id_itemTmp = window["ItemDetail" + key].GetValue();

    codeDocumentType = $("#codeDocumentType").val();

    var isExit = codeDocumentType == "05" || codeDocumentType == "32";

    if (isExit) {

        id_lot.ClearItems();
        id_lot.SetValue(null);

        remainingBalance.SetValue(0);

        SetValueUnitPriceMove(0);
        //unitPriceMove.SetValue(0);
        unitPriceMoveAux = 0;

        id_metrictUnitMoveAux = 0;
        id_metricUnitMove.SetValue(null);

        OnAmountOrPriceMoveValueChanged();

    }

    var data = {
        id_item: id_itemTmp,
        id_warehouse: id_warehouseDetail.GetValue(),
        id_warehouseLocation: s.GetValue()
    };

    $.ajax({
        url: "InventoryMoveTransfer/UpdateLot",//UpdateWarehouseLocation",
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
                if (isExit) {
                    arrayFieldStr = [];
                    arrayFieldStr.push("number");
                    UpdateDetailObjects(id_lot, result.lots, arrayFieldStr);

                    remainingBalance.SetValue(result.remainingBalance);

                    unitPriceMove.SetValue(result.averagePrice);
                    unitPriceMoveAux = result.averagePrice;

                    id_metricUnitMove.SetValue(result.id_metricUnitMove);
                    id_metrictUnitMoveAux = result.id_metricUnitMove;

                    //unitPriceMove.SetValue(result.unitPriceMove);
                    //unitPriceMoveAux = result.unitPriceMove;

                    OnAmountOrPriceMoveValueChanged();

                }
            }
            //else {
            //    id_priceList.ClearItems();
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });

}

function OnLotDetailCombo_SelectedIndexChanged(s, e) {
    var _index = gridViewMoveDetails.cpRowIndex;
    var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
    var id_itemTmp = window["ItemDetail" + key].GetValue();

    codeDocumentType = $("#codeDocumentType").val();

    var isExit = codeDocumentType == "05" || codeDocumentType == "32";

    if (isExit) {
        //id_lot.ClearItems();
        //id_lot.SetValue(null);

        id_metricUnitMove.SetValue(null);
        id_metrictUnitMoveAux = 0;

        remainingBalance.SetValue(0);

        SetValueUnitPriceMove(0);
        //unitPriceMove.SetValue(0);
        unitPriceMoveAux = 0;

        OnAmountOrPriceMoveValueChanged();

    }

    var data = {
        id_item: id_itemTmp,
        id_warehouse: id_warehouseDetail.GetValue(),
        id_warehouseLocation: id_warehouseLocationDetail.GetValue(),
        id_lot: s.GetValue()
    };

    $.ajax({
        url: "InventoryMoveTransfer/LotDetail",//UpdateWarehouseLocation",
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
                //if (isExit) {
                    //arrayFieldStr = [];
                    //arrayFieldStr.push("number");
                    //UpdateDetailObjects(id_lot, result.lots, arrayFieldStr);

                    remainingBalance.SetValue(result.remainingBalance);

                    unitPriceMove.SetValue(result.averagePrice);
                    unitPriceMoveAux = result.averagePrice;

                    id_metricUnitMove.SetValue(result.id_metricUnitMove);
                    id_metrictUnitMoveAux = result.id_metricUnitMove;

                    //unitPriceMove.SetValue(result.unitPriceMove);
                    //unitPriceMoveAux = result.unitPriceMove;

                    OnAmountOrPriceMoveValueChanged();

                //}
            }
            //else {
            //    id_priceList.ClearItems();
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function UpdateQuantityTotal(data) {

    if (data.id_metricUnitMove === null) {
        SetValueBalanceCost(0);//balanceCost.SetValue(0);
        //id_metricUnitPresentation.SetValue(null);
        return;
    }

    $.ajax({
        url: "InventoryMoveTransfer/UpdateQuantityTotal",
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
            if (result !== null && result != undefined) {
                if (result.Message == "OK") {
                    SetValueBalanceCost(result.balanceCost);//balanceCost.SetValue(result.balanceCost);
                    //unitPriceMove.SetValue(result.unitPriceMove);
                    id_metricUnitMove.SetValue(result.id_metricUnitMove);
                    id_metrictUnitMoveAux = result.id_metricUnitMove;
                    unitPriceMoveAux = result.unitPriceMove;
                    SetValueUnitPriceMove(result.unitPriceMove);
                    //unitPriceMove.SetValue(result.unitPriceMove);
                } else {
                    id_metricUnitMove.SetValue(result.id_metricUnitMove);
                    var msgAux = WarningMessage(result.Message);
                    gridMessageErrorMaterialsDetail.SetText(msgAux);
                    $("#GridMessageErrorMaterialsDetail").show();

                    //$("#GridMessageErrorMaterialsDetail").show();
                    if ($(".alert-warning") !== undefined && $(".alert-warning") !== null) {
                        $(".alert-warning").fadeTo(3000, 0.45, function () {
                            $(".alert-warning").alert('close');
                        });
                        //setTimeout(function () {
                        //    $(".alert-warning").alert('close');
                        //}, 3000);
                    }
                }

            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function MetricUnitMoveCombo_SelectedIndexChanged(s, e) {
    //var quantityAux = quantity.GetValue();
    var strUnitPriceMove = unitPriceMoveAux == null ? "0" : unitPriceMoveAux.toString();
    var resUnitPriceMove = strUnitPriceMove.replace(".", ",");

    var amountMoveAux = amountMove.GetValue();
    var strAmountMove = amountMoveAux == null ? "0" : amountMoveAux.toString();
    var resAmountMove = strAmountMove.replace(".", ",");

    UpdateQuantityTotal({
        unitPriceMove: resUnitPriceMove,
        id_metricUnitMove: id_metrictUnitMoveAux,
        id_metricUnitMoveCurrent: s.GetValue(),
        amountMove: resAmountMove
    });
}

// DETAILS ACTIONS 

function AddNewDetail(s, e) {
    gridViewMoveDetails.AddNewRow();
}



$(function () {

    //UpdateTitlePanelDetails();
    //var codeDocumentType = $("#codeDocumentType").val();
    //console.log("codeDocumentType: " + codeDocumentType);
    //console.log("codeDocumentType: " + codeDocumentType);
    //if (codeDocumentType == "04")//Ingreso x Orden de Compra
    //{
    //    btnRemoveDetail.SetEnabled(false);
    //    btnNewDetail.SetEnabled(false);
    //}
});