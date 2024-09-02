
//Validations

function OnItemProductionLotTrashDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseProductionLotTrashDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationProductionLotTrashDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnQuantityProductionLotTrashDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if(parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
    }
}


// EDITOR'S EVENTS

function UpdateProductionLotTrashDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    gvProductionLotReceptionEditFormProductionLotTrashsDetail.GetEditor("metricUnit").SetText("");
    gvProductionLotReceptionEditFormProductionLotTrashsDetail.GetEditor("id_warehouse").SetValue(null);//ClearSelection();// SetValue("");
    gvProductionLotReceptionEditFormProductionLotTrashsDetail.GetEditor("id_warehouseLocation").SetValue(null);//.ClearSelection();// SetValue("");

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
                    gvProductionLotReceptionEditFormProductionLotTrashsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    gvProductionLotReceptionEditFormProductionLotTrashsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    gvProductionLotReceptionEditFormProductionLotTrashsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemProductionLotTrashDetailCombo_SelectedIndexChanged(s, e) {
    UpdateProductionLotTrashDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

function ItemProductionLotTrashDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionTrashDetails",
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


