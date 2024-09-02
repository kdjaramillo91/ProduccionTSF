
//Validations

function OnItemDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnQuantityRemittedValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if(parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
    }
}

function OnQuantityRecivedValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
    }
}

// EDITOR'S EVENTS

function UpdateDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText("");
    gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(null);// 
    gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(null);// SetValue("");

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
                    gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemDetailCombo_SelectedIndexChanged(s, e) {
    UpdateDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

function ItemDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionItemDetails",
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



function QuantityRemitted_NumberChange(s, e) {
    //UpdateProductionLotTotals();
}

function QuantityRecived_NumberChange(s, e) {
    //UpdateProductionLotTotals();
}
