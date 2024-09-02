function OnWarehousesTypesNameValidation(s, e) {
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


function OnWarehousesTypesCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length >= 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "WarehouseType/ValidateCodeWarehouseType",
                type: "post",
                async: false,
                cache: false, data: {
                    id_warehouseType: gvWarehousesTypes.cpEditingRowKey,
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

function OnProductionCostValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } 
}

function OnPoundTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } 
}

function OnFormulaPoundsProcessedValidation(s, e) {
    let _prodCostingSel = productionCosting.GetValue();
    let _poundsType = poundsType.GetValue();

    if (_prodCostingSel == "SI") {
        if (_poundsType == "AMBAS" || _poundsType == "LIBPRO") {
            if (e.value === null) {
                e.isValid = false;
                e.errorText = "Campo Obligatorio";
            }         
        }
    }
}

function OnFormulaPoundsFinishedValidation(s, e) {
    let _prodCostingSel = productionCosting.GetValue();
    let _poundsType = poundsType.GetValue();

    if (_prodCostingSel == "SI") {
        if (_poundsType == "AMBAS" || _poundsType == "LIBTERM") {
            if (e.value === null) {
                e.isValid = false;
                e.errorText = "Campo Obligatorio";
            }
        }
    }
}

function OnReasonCostsValidation(s, e) {
    let _prodCostingSel = productionCosting.GetValue();
    if (_prodCostingSel == "SI") {
        if (e.value === null) {
            e.isValid = false;
            e.errorText = "Campo Obligatorio";
        }        
    }
}