
//Validations

function OnItemProductionLotLiquidationDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseProductionLotLiquidationDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationProductionLotLiquidationDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnQuantityProductionLotLiquidationDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if(parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
    }
}


// EDITOR'S EVENTS

function UpdateProductionLotLiquidationDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("metricUnit").SetText("");
    gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouse").SetValue(null);//ClearSelection();// SetValue("");
    gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(null);//.ClearSelection();// SetValue("");

    if (id_item != null) {

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
                    gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemProductionLotLiquidationDetailCombo_SelectedIndexChanged(s, e) {
    UpdateProductionLotLiquidationDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}


function ItemProductionLotLiquidationDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionLiquidationDetails",
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
            for (var i = 0; i < id_item.GetItemCount() ; i++) {
                var item = id_item.GetItem(i);
                if (result.indexOf(item.value) >= 0) {
                    id_item.RemoveItem(i);
                    i = -1;
                }
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// EDITOR'S EVENTS



function Quantity_NumberChange(s, e) {
    //UpdateProductionLotTotals();
}


