function OnProductionUnitNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnProductionUnitCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var itemBranchOffice = id_branchOffice.GetSelectedItem();
        if (itemBranchOffice !== null && itemBranchOffice !== undefined) {
            $.ajax({
                url: "ProductionUnit/ValidateCode",
                type: "post",
                async: false,
                cache: false,
                data: {
                    id_branchOffice: itemBranchOffice.value,
                    id_productionUnit: gvProductionUnit.cpEditingRowKey,
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
        } else {
            e.isValid = false;
            e.errorText = "Debe seleccionar sucursal antes de definir el código";
        }
    }
}

function OnProductionUnitValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
