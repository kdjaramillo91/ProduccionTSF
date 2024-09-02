
// VALIDATIONS

function OnProviderValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnBuyerValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function DateValidation(expirationDateAux, receptionDateAux) {
    if (expirationDateAux <= receptionDateAux) {
        return ("La Fecha de Caducidad debe ser mayor que la Fecha de Recepción del Lote");
    } else {
        return ("Ok");
    }
    
}

function OnExpirationDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        // 
        if (e.value <= receptionDate.GetValue()) {
            e.isValid = false;
            e.errorText = "La Fecha de Caducidad debe ser mayor que la Fecha de Recepción del Lote";
        } 
    }
        
}

function OnproductionUnitProviderPoolValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// REFRESH COMBO

function Provider_SelectedIndexChanged(s, e) {
    id_productionUnitProviderPool.PerformCallback();
}

function ProductionUnitProviderPool_BeginCallback(s, e) {
    e.customArgs["id_provider"] = id_provider.GetValue();
}

function ProductionUnitProviderPool_OnBeginCallback(s, e) {
    e.customArgs["id_productionUnitProviderPoolCurrent"] = null;
    e.customArgs["id_productionUnitProvider"] = id_productionUnitProvider.GetValue();
}

//On Reception Date
function OnReceptionDateChanged(s, e) {
    // 

    var dateR = receptionDate.GetDate();
    var year = dateR.getFullYear();
    var month = dateR.getMonth() + 1;
    var day = dateR.getDate();

    var data = {
        id_pl: $("#id_productionLot").val(),
        intNumber: internalNumber.GetText(),
        yearR: year,
        monthR: month,
        dayR: day
    }

    $.ajax({
        url: "ProductionLotReception/UpdateProductionLotJulianoNumber",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            // 
            if (result !== null) {
                julianoNumber.SetText(result.julianoNumberTmp);
                internalNumber.SetText(result.internalNumberTmp);
                internalNumberConcatenated.SetText(result.internalNumberConcatenatedTmp);
            }
        },
        complete: function () {
        }
    });

}