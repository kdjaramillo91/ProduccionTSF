var id_originLotCurrent = null;
var id_itemCurrent = null;
//var id_itemCurrentPosition = -1;
var id_warehouseCurrent = null;
var id_warehouseLocationCurrent = null;
var errorMessage = "";
var runningValidation = false;
var minimumPresentation = 1;

var id_itemInit = null;
var id_warehouseInit = null;
var id_warehouseLocationInit = null;
//var metricPresentacion = "";


function ComboProductionLot_Init(s, e) {

    id_originLotCurrent = s.GetValue();
    id_originLotInit = s.GetValue();
    id_itemCurrent = id_item.GetValue();
    id_itemInit = id_item.GetValue();
    s.PerformCallback();
}

function ComboProductionLot_InitM(s, e) {

    id_originLotCurrent = s.GetValue();
    id_originLotInit = s.GetValue();
    id_itemCurrent = id_item.GetValue();
    id_itemInit = id_item.GetValue();
    var fecha = receptionDate.GetDate().toISOString();

    var data = {
        id_productionLot: s.GetValue(),
        fechaProceso: fecha,
        id_item: id_itemInit,
    };
    $.ajax({
        url: "ProductionLotProcess/GetOriginLotM",
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
            if (result !== null && result !== undefined) {
                if (result.originLots != null) {
                    for (var i = 0; i < result.originLots.length; i++) {
                        if (result.originLots[i].internalNumber != "") {
                            id_originLot.AddItem([result.originLots[i].internalNumber], result.originLots[i].id);
                        }
                    }
                }
            }
            else {
                id_originLot.ClearItems();
            }
        },
        complete: function () {

        }
    });


}

function ProductionLot_BeginCallback(s, e) {
    var fecha = receptionDate.GetDate().toISOString();
    e.customArgs["id_productionLot"] = s.GetValue();
    e.customArgs["fechaProceso"] = fecha;
}

function ProductionLot_EndCallback(s, e)
{
    if (id_originLotCurrent !== null && id_originLotCurrent !== 0) {
        id_originLot.SetValue(id_originLotCurrent);
    }
    id_item.PerformCallback();
}

function ComboProductionLot_SelectedIndexChanged(s, e) {
    
    metricUnit.SetText("");
    metricUnitPresentation.SetText("");
    currentStock.SetValue(0);
    minimumPresentation = 1;
    QuantityRecived_NumberChangeItem(quantityRecived, quantityRecived);
    currentStockAux = 0;

    id_item.SetValue(null);
    id_itemInit = null;
    masterCode.SetText("");
    
    var texto = s.GetText();
    var _lotMarked = null;
    var splits = texto.split("/");
    if (splits.length > 2) {
        _lotMarked = splits[2].trim();
    }

    if (gvProductionLotProcessEditFormItemsDetail.cpIsParLotMarked) {
        lotMarked.SetText(_lotMarked);
    }
        
    id_item.PerformCallback();
}


function ComboProductionLotM_SelectedIndexChanged(s, e) {

    metricUnit.SetText("");
    metricUnitPresentation.SetText("");
    currentStock.SetValue(0);
    //debugger;

    var texto = s.GetText();
    var _lotMarked = null;
    var splits = texto.split("/");
    if (splits.length > 2) {
        _lotMarked = splits[2].trim();
    }

    if (gvProductionLotProcessEditFormItemsDetail.cpIsParLotMarked) {
        lotMarked.SetText(_lotMarked);
    }


    var id_productionLotSelected = s.GetValue();
    var id_itemSelected = id_item.GetValue();
    var id_lotSelected = s.GetValue();
    //debugger;
    if (id_lotSelected == null) {
        metricUnit.SetText("");
        metricUnitPresentation.SetText("");
        currentStock.SetValue(0);
        currentStockAux = 0;
        quantityRecived.SetValue(0);
        minimumPresentation = 1;
        masterCode.SetText("");
        QuantityRecived_NumberChangeItem(quantityRecived, quantityRecived);
        return;
    }

    var fecha = receptionDate.GetDate().toISOString();
    var data = {
        id_originLot: id_originLotCurrent,
        id_item: id_itemCurrent,
        id_warehouse: 0,
        id_warehouseLocation: 0,
        id_productionLotSelected: id_productionLotSelected,
        id_itemSelected: id_itemSelected,
        id_warehouseSelected: 0,
        id_warehouseLocationSelected: 0,
        fechaProceso: fecha
    };

    $.ajax({
        url: "ProductionLotProcess/ItemLotDetailData",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
               
                metricUnit.SetText(result.metricUnit);
                metricUnitPresentation.SetText(result.metricUnitPresentation);
                currentStock.SetValue(result.currentStock);
                currentStockAux = result.currentStock;
                minimumPresentation = result.minimumPresentation;
            }
            else {
                metricUnit.SetText("");
                metricUnitPresentation.SetText("");
                currentStock.SetValue(0);
                currentStockAux = 0;
                minimumPresentation = 1;
            }
            //debugger;
            QuantityRecived_NumberChangeItem(quantityRecived, quantityRecived);

        },
        complete: function () {
            hideLoading();
        }
    });

}

function ComboItem_BeginCallback(s, e) {
    //debugger;
    var _lotMarked = null;
    if (gvProductionLotProcessEditFormItemsDetail.cpIsParLotMarked) {
        _lotMarked = lotMarked.GetText();
    }
        
    var fecha = receptionDate.GetDate().toISOString();
    e.customArgs["id_productionLot"] = id_originLot.GetValue();
    e.customArgs["id_originLot"] = id_originLotCurrent;
    e.customArgs["id_item"] = id_itemCurrent;
    e.customArgs["lotMarked"] = _lotMarked;
    e.customArgs["fechaProceso"] = fecha;
    // [x]
    //e.customArgs["id_warehouse"] = id_warehouse.GetValue();  //id_warehouseCurrent;
    //e.customArgs["id_warehouseLocation"] = id_warehouseLocation.GetValue();  //id_warehouseLocationCurrent;
}
function ComboItemM_BeginCallback(s, e) {
    e.customArgs["id_item"] = id_itemCurrent;
}
function Dev_masterCode_Init(s, e) {
    var masterCode = s;
    masterCode.inputElement.tabIndex = -1;
}

function ComboItem_EndCallback(s, e) {
    // 
    if (s.initializeInputValue !== null && s.initializeInputValue !== 0) {

        id_item.SetValue(s.initializeInputValue);
        ComboItem_SelectedIndexChanged(s, e);
    }

}


function ComboItem_Init(s, e) {

    var id_productionLotSelected = id_originLot.GetValue();
    var id_itemSelected = s.GetValue();
    // [x]
    //var id_warehouseSelected = id_warehouse.GetValue();
    //var id_warehouseLocationSelected = id_warehouseLocation.GetValue();
    var id_warehouseSelected = 0;
    var id_warehouseLocationSelected = 0;
    var data = {
        id_productionLot: id_productionLotSelected,
        id_item: id_itemSelected,
        id_warehouse: id_warehouseSelected,
        id_warehouseLocation: id_warehouseLocationSelected
    };
    
    $.ajax({
        url: "ProductionLotProcess/ProductionLotDetailData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                //metricUnit.SetText(result.metricUnit);

                UpdateProductionLotDetailItems(result.items);
                //s.SetValue(id_itemSelected);
                //ComboItem_SelectedIndexChanged(s, e);
                        //ComboWarehouse_Init(id_warehouse, e);
            }
            else {
                id_item.ClearItems();
                metricUnit.SetText("");
                currentStock.SetValue(0);
                currentStockAux = 0;
                    //id_warehouse.ClearItems();
                    //id_warehouseLocation.ClearItems();
            }
        },
        complete: function () {
            //hideLoading();
        }
    });


}

function ComboItem_Init(s, e) {

    id_item.PerformCallback();
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

function ComboItem_SelectedIndexChanged(s, e) {

    var id_productionLotSelected = id_originLot.GetValue();
    var id_itemSelected = s.GetValue();


 
    if (id_itemSelected == null)
    {
        metricUnit.SetText("");
        metricUnitPresentation.SetText("");
        currentStock.SetValue(0);
        currentStockAux = 0;
        quantityRecived.SetValue(0);
        minimumPresentation = 1;
        masterCode.SetText("");
        QuantityRecived_NumberChangeItem(quantityRecived, quantityRecived);
        return;
    }

    var fecha = receptionDate.GetDate().toISOString();
    var data = {
        id_originLot: id_originLotCurrent,
        id_item: id_itemCurrent,
        id_warehouse: 0,
        id_warehouseLocation: 0,
        id_productionLotSelected: id_productionLotSelected,
        id_itemSelected: id_itemSelected,
        id_warehouseSelected: 0,
        id_warehouseLocationSelected: 0,
        fechaProceso: fecha
    };

    $.ajax({
        url: "ProductionLotProcess/ItemDetailData",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                metricUnit.SetText(result.metricUnit);
                metricUnitPresentation.SetText(result.metricUnitPresentation);
                currentStock.SetValue(result.currentStock);
                currentStockAux = result.currentStock;
                minimumPresentation = result.minimumPresentation;
                masterCode.SetText(result.masterCode);
            }
            else {
                metricUnit.SetText("");
                metricUnitPresentation.SetText("");
                currentStock.SetValue(0);
                currentStockAux = 0;
                minimumPresentation = 1;
                masterCode.SetText("");
            }
            QuantityRecived_NumberChangeItem(quantityRecived, quantityRecived);

        },
        complete: function () {
            hideLoading();
        }
    });
}


function ComboItemM_SelectedIndexChanged(s, e) {

    var id_productionLotSelected = id_originLot.GetValue();
    var id_itemSelected = s.GetValue();

    if (id_itemSelected == null) {
        masterCode.SetText("");
        id_originLot.ClearItems();
        QuantityRecived_NumberChangeItem(quantityRecived, quantityRecived);
        return;
    }

    var fecha = receptionDate.GetDate().toISOString();
    var data = {
        id_originLot: id_originLotCurrent,
        id_item: id_itemCurrent,
        id_warehouse: 0,
        id_warehouseLocation: 0,
        id_productionLotSelected: id_productionLotSelected,
        id_itemSelected: id_itemSelected,
        id_warehouseSelected: 0,
        id_warehouseLocationSelected: 0,
        fechaProceso: fecha
    };

    $.ajax({
        url: "ProductionLotProcess/ItemMDetailData",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                if (result.originLots != null) {
                    for (var i = 0; i < result.originLots.length; i++) {
                        if (result.originLots[i].internalNumber != "") {
                            id_originLot.AddItem([result.originLots[i].internalNumber], result.originLots[i].id);
                        }
                    }
                }
               
                masterCode.SetText(result.masterCode);
            }
            else {
               
                masterCode.SetText("");
                id_originLot.ClearItems();
            }
            //debugger;
            QuantityRecived_NumberChangeItem(quantityRecived, quantityRecived);

        },
        complete: function () {
            hideLoading();
        }
    });
}
function ComboWarehouse_Init(s, e) {

    
    s.PerformCallback();
    s.SetSelectedIndex(1);
}

function ComboWarehouse_BeginCallback(s, e) {
    
    e.customArgs["id_productionLotSelected"] = id_originLot.GetValue();
    e.customArgs["id_itemSelected"] = id_item.GetValue();
    e.customArgs["id_originLot"] = id_originLotCurrent;
    e.customArgs["id_item"] = id_itemCurrent;
    e.customArgs["idProductionProcess"] = id_productionProcess.GetValue();
}

function ComboWarehouse_EndCallback(s, e) {

     
    if (s.initializeInputValue !== null && s.initializeInputValue !== 0) {

    }
}

function ComboWarehouse_SelectedIndexChanged(s, e) {
}

function ComboWarehouseLocation_BeginCallback(s, e) {
    e.customArgs["id_productionLotSelected"] = id_originLot.GetValue();
    e.customArgs["id_itemSelected"] = id_item.GetValue();
    //e.customArgs["id_warehouseSelected"] = id_warehouse.GetValue();
    e.customArgs["id_originLot"] = id_originLotCurrent;
    e.customArgs["id_item"] = id_itemCurrent;
}

function ComboWarehouseLocation_EndCallback(s, e) {

    
    if (s.initializeInputValue !== null && s.initializeInputValue !== 0) {
        // [x]
        //id_warehouseLocation.SetValue(s.initializeInputValue);
    }
   
    id_originLot.PerformCallback();
}

function ComboWarehouseLocation_SelectedIndexChanged(s, e) {

    id_originLot.PerformCallback();
}

function QuantityRecived_NumberChangeItem(s, e) {
      
    // 
    var quantityRecivedAux = s.GetValue();
    quantityRecivedAux = quantityRecivedAux == null ? 0 : quantityRecivedAux;
    totalQuantityItem.SetValue(minimumPresentation * quantityRecivedAux);

}

//Validations

function ValidateDetail() {
    runningValidation = true;
    //OnOriginLotDetailValidation(id_originLot, id_originLot);
    OnItemDetailValidation(id_item, id_item);
    //OnWarehouseDetailValidation(id_warehouse, id_warehouse);
    //OnWarehouseLocationDetailValidation(id_warehouseLocation, id_warehouseLocation);
    //OnQuantityRecivedValidation(quantityRecived, quantityRecived);
    runningValidation = false;
}

function OnOriginLotDetailValidation(s, e) {
    //gridMessageErrorItemDetail.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorItemDetail").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Lote Origen: Es obligatorio.";
    }

    //if (!runningValidation) {
    //    ValidateDetail();
    //}

    //if (e.value === null) {
    //    e.isValid = false;
    //    e.errorText = "Campo Obligatorio";
    //}
}

function OnItemDetailValidation(s, e) {
    // 
    errorMessage = "";
    if (s.GetValue() === null)
    {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Producto: Es obligatorio.";
        } else {
            errorMessage += "</br>- Producto: Es obligatorio.";
        }
    }
    

    if (!runningValidation) {
        ValidateDetail();
    }

    //if(e.value === null) {
    //    e.isValid = false;
    //    e.errorText = "Campo Obligatorio";
    //}
}

function OnWarehouseDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Bodega: Es obligatoria.";
        } else {
            errorMessage += "</br>- Bodega: Es obligatoria.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }

}

function OnWarehouseLocationDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
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

var currentStockAux = 0;

function OnQuantityRecivedValidation(s, e)
{
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad: Es obligatoria.";
        }
    }
    else if (parseFloat(s.GetValue()) <= 0)
    {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad: Incorrecta.";
        } else {
            errorMessage += "</br>- Cantidad: Incorrecta.";
        }
    } else
    {
        if (parseFloat(s.GetValue()) > parseFloat(currentStockAux)) {
            e.isValid = false;
            e.errorText = "Cantidad Incorrecta, no puede ser mayor a la existente: " + currentStockAux;
            if (errorMessage == null || errorMessage == "") {
                errorMessage = "- Cantidad: Incorrecta, no puede ser mayor a la existente: " + currentStockAux;
            } else {
                errorMessage += "</br>- Cantidad: Incorrecta, no puede ser mayor a la existente: " + currentStockAux;
            }
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorItemDetail.SetText(msgErrorAux);
        $("#GridMessageErrorItemDetail").show();
        //id_item.isValid = false;

    }

}

// EDITOR'S EVENTS

function OnGridViewItemDetailsInit(s, e) {
    UpdateTitlePanelItemDetails();
}

function UpdateTitlePanelItemDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountItemDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotProcessEditFormItemsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotProcessEditFormItemsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountItemDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoItems").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsItems", gvProductionLotProcessEditFormItemsDetail.GetSelectedRowCount() > 0 && gvProductionLotProcessEditFormItemsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionItems", gvProductionLotProcessEditFormItemsDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveDetail.SetEnabled(gvProductionLotProcessEditFormItemsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountItemDetails() {
    return gvProductionLotProcessEditFormItemsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotProcessEditFormItemsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewItemsDetailSelectionChanged(s, e) {
    UpdateTitlePanelItemDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackItemsDetail);

}

function GetSelectedFieldValuesCallbackItemsDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewItemDetailsBeginCallback(s, e) {
    customCommand = e.command;

    var fecha = receptionDate.GetDate().toISOString();
    e.customArgs["fechaProceso"] = fecha;
}

function OnGridViewItemDetailsEndCallback(s, e) {
      
    UpdateTitlePanelItemDetails();
    // 
    if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
        s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
    }

    UpdateProductionLotTotals();
    receptionDate.SetEnabled(!gvProductionLotProcessEditFormItemsDetail.cpExistenRegistros);
}

function gvEditItemDetailsClearSelection() {
    gvProductionLotProcessEditFormItemsDetail.UnselectRows();
}

function gvEditItemDetailsSelectAllRows() {
    gvProductionLotProcessEditFormItemsDetail.SelectRows();
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnQualityControlRow") {
        var data = $("#formEditProductionLotProcess").serialize();
        $.ajax({
            url: "ProductionLot/UpdateProductionLot",
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
                    var data2 = {
                        id: 0,
                        id_productionLotDetail: gvProductionLotProcessEditFormItemsDetail.GetRowKey(e.visibleIndex),
                        urlToReturn: "ProductionLotProcess/ProductionLotProcessFormEditPartial",
                        tabSelected: 2,//Seleccionado el tab 3 de Matreria Prima
                        arrayTempDataKeep: ["productionLot"]
                    };
                    //console.log("data2.id: " + data2.id);
                    //console.log("data2: " + data2);
                    showPagefromLink("QualityControl/FormEditQualityControl", data2);
                    //Mostrar Vista de Gestion de Calidad pasandole el id_QualityControl que estaria en una propiedad del grid
                }
            },
            complete: function () {
                //hideLoading();
            }
        });

    }
}