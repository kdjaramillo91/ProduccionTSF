function OnWarehouseLocationsNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length >= 101) {
            e.isValid = false;
            e.errorText = "Máximo 100 caracteres";
        }
    }
}

function OnWarehouseLocationsWarehouseValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCodeWarehouseLocationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "WarehouseLocation/ValidateCodeWarehouseLocation",
                type: "post",
                async: false,
                cache: false, data: {
                    id_warehouseLocation: gvWarehouseLocations.cpEditingRowKey,
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

function OnWarehouseLocationsPersonValidation(s, e) {
    var codeWarehouseType = $("#codeWarehouseType").val();
    if (codeWarehouseType == "VIR01" || codeWarehouseType == "RES01") {
        if (e.value === null) {
            e.isValid = false;
            e.errorText = "Campo Obligatorio";
        }
    }

}