
function OnItemGroupCategoriesItemGroupValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnItemGroupCategoriesNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        if (e.value.length > 50) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        }
    }
}

function OnCodeItemGroupCategoryValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "ItemGroupCategory/ValidateCodeItemGroupCategory",
                type: "post",
                async: false,
                cache: false, data: {
                    id_itemGroupCategory: gvItemGroupCategories.cpEditingRowKey,
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
