
// VALIDATIONS
var Message = "Ok";

function OnFreezerWarehouseValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabOpeningClosingPlateLyingDetails");

}

function UpdateOpeningClosingPlateLyingFreezerWarehouseLocations(warehouseLocations, ids_freezerWarehouseLocationParam) {

    for (var i = 0; i < ids_freezerWarehouseLocation.GetItemCount() ; i++) {
        var warehouseLocation = ids_freezerWarehouseLocation.GetItem(i);
        var into = false;
        for (var j = 0; j < warehouseLocations.length; j++) {
            if (warehouseLocation.value == warehouseLocations[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            ids_freezerWarehouseLocation.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < warehouseLocations.length; i++) {
        var warehouseLocation = ids_freezerWarehouseLocation.FindItemByValue(warehouseLocations[i].id);
        if (warehouseLocation == null) ids_freezerWarehouseLocation.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
    }

    ids_freezerWarehouseLocation.SetValue(ids_freezerWarehouseLocationParam);

}

function ComboFreezerWarehouse_SelectedIndexChanged(s, e) {
    ids_freezerWarehouseLocation.ClearTokenCollection();
    ids_freezerWarehouseLocation.ClearItems();
    var data = {
        id_freezerWarehouse: s.GetValue()//,
    };

    $.ajax({
        url: "OpeningClosingPlateLying/UpdateOpeningClosingPlateLyingFreezerWarehouseLocation",
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
                UpdateOpeningClosingPlateLyingFreezerWarehouseLocations(result.warehouseLocations, null);
                UpdategvOpeningClosingPlateLyingEditFormDetail();
            }
        },
        complete: function () {
        }
    });

}

function UpdategvOpeningClosingPlateLyingEditFormDetail() {
    gvOpeningClosingPlateLyingEditFormDetail.UnselectRows();
    gvOpeningClosingPlateLyingEditFormDetail.PerformCallback();
}

function TokenfreezerWarehouseLocation_Init(s, e) {
    
    $.ajax({
        url: "OpeningClosingPlateLying/GetOpeningClosingPlateLyingIds_freezerWarehouseLocation",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateOpeningClosingPlateLyingFreezerWarehouseLocations([], null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdateOpeningClosingPlateLyingFreezerWarehouseLocations(result.warehouseLocations,
                                                             result.ids_freezerWarehouseLocationParam);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function TokenfreezerWarehouseLocation_ValueChanged(s, e) {
    ids_freezerWarehouseLocationAux = s.GetValue();
    var data = {
        id_freezerWarehouse: id_freezerWarehouse.GetValue(),
        ids_freezerWarehouseLocation: ids_freezerWarehouseLocationAux.split(",")
    };
   
    $.ajax({
        url: "OpeningClosingPlateLying/UpdateOpeningClosingPlateLyingDetail",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //UpdatePriceListInventoryLines(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //UpdatePriceListInventoryLines(result.inventoryLines);
            UpdategvOpeningClosingPlateLyingEditFormDetail();
        },
        complete: function () {
            //hideLoading();
        }
    });


}

function UpdateOpeningClosingPlateLyingMaintenanceWarehouseLocations(warehouseLocations) {

    for (var i = 0; i < id_maintenanceWarehouseLocation.GetItemCount() ; i++) {
        var warehouseLocation = id_maintenanceWarehouseLocation.GetItem(i);
        var into = false;
        for (var j = 0; j < warehouseLocations.length; j++) {
            if (warehouseLocation.value == warehouseLocations[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_maintenanceWarehouseLocation.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < warehouseLocations.length; i++) {
        var warehouseLocation = id_maintenanceWarehouseLocation.FindItemByValue(warehouseLocations[i].id);
        if (warehouseLocation == null) id_maintenanceWarehouseLocation.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
    }

}

function OnMaintenanceWarehouseValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabOpeningClosingPlateLyingDetails");

}

function ComboMaintenanceWarehouse_SelectedIndexChanged(s, e) {

    id_maintenanceWarehouseLocation.SetValue(null);
    id_maintenanceWarehouseLocation.ClearItems();
    var data = {
        id_maintenanceWarehouse: s.GetValue()//,
    };

    $.ajax({
        url: "OpeningClosingPlateLying/UpdateOpeningClosingPlateLyingMaintenanceWarehouseLocation",
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
                UpdateOpeningClosingPlateLyingMaintenanceWarehouseLocations(result.warehouseLocations);
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

function OnMaintenanceWarehouseLocationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabOpeningClosingPlateLyingDetails");

}

function ComboMaintenanceWarehouseLocation_Init(s, e) {

    $.ajax({
        url: "OpeningClosingPlateLying/GetOpeningClosingPlateLyingMaintenanceWarehouseLocation",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateOpeningClosingPlateLyingMaintenanceWarehouseLocations([]);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdateOpeningClosingPlateLyingMaintenanceWarehouseLocations(result.warehouseLocations);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function init() {
    var id_priceListAux = $("#id_priceListAux").val();
    var codeState = $("#codeState").val();
    if (codeState == "07")//PENDIENTE DE PAGO
    {
        UpdateProductionLotPaymentDetails(id_priceListAux);
    }
    
}

$(function () {

    init();
});