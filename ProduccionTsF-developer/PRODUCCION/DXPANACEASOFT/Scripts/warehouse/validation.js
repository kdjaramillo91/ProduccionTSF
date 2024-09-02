function OnWarehousesNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length >= 51) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        }
    }
}

function OnWarehousesWarehouseTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnWarehousesInventoryLineValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnWarehousesProductionExpenseValidation(s, e) {
    var value = id_costProduction.GetValue();
    if (value !== null && e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnCodeWarehousesValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "Warehouse/ValidateCodeWarehouses",
                type: "post",
                async: false,
                cache: false,data: {
                    id_warehouse: gvWarehouses.cpEditingRowKey,
                    code: e.value
                },
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    e.isValid = result.isValid;
                    e.errorText = result.errorText;
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }
    }
}

