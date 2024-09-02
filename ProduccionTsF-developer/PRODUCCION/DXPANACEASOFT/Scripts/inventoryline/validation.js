
function OnInventoryLinesNameValidation(s, e) {
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

function OnInventoryLinesCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length >= 20) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "InventoryLine/ValidateCode",
                type: "post",
                async: false,
                cache: false,
                data: {
                    id_inventoryLine: gvInventoryLines.cpEditingRowKey,
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
                }});
        }
    }
}

function OnItemTypesSequentialValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

