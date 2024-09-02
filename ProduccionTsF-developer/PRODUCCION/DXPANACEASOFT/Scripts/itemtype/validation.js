
function OnItemTypesInventoryLineValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnItemTypesNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length >= 50) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        }
    }
}


    function OnItemTypesCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length >= 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "ItemType/ValidateCodeItemType",
                type: "post",
                async: false,
                cache: false, data: {
                    id_itemType: gvItemTypes.cpEditingRowKey,
                    code: e.value
                },
                error: function (error) {
                    console.log(error);
                },
                success: function (result) {
                    e.isValid = result.isValid;
                    e.errorText = result.errorText;
                }    
            });
        }
    }
}

