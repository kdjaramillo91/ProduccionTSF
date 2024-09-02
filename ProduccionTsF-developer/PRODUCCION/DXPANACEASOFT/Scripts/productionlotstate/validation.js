function OnProductionLotStateNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnProductionLotStateCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    else {
        $.ajax({
            url: "ProductionLotState/ValidateCode",
            type: "post",
            async: false,
            cache: false,
            data: {
                id_productionLotState: gvProductionLotState.cpEditingRowKey,
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
