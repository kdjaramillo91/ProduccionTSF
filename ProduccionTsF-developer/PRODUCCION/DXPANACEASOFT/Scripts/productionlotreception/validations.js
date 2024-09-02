
// TABIMAGE

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function OnInternalNumberValidation(s, e) {
    var incorrect = false
    var messageErrorText = "";
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        $.ajax({
            url: "ProductionLotReception/ValidateInternalNumberLot",
            type: "post",
            async: false,
            cache: false,
            data: {
                idPl: $("#id_productionLot").val(),
                internalNum: internalNumber.GetText(),
                julianoNum: julianoNumber.GetText(),
                id_provider: id_provider.GetValue(),
                id_productionUnitProvider: id_productionUnitProvider.GetValue(),
                id_productionUnitProviderPool: id_productionUnitProviderPool.GetValue()
            },
            error: function (error) {
            },
            beforeSend: function () {
            },
            success: function (result) {
                //// 
                if (result !== null) {
                    if (result.itsAssigned === "YES") {
                        incorrect = true;
                        messageErrorText = result.Error1;
                    }
                }
            },
            complete: function () {
            }
        });
    }
    if (incorrect == true) {
        e.isValid = false;
        e.errorText = messageErrorText;
    }
}

function OnDrawersNumberValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnPiscinaValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnInternalNumberTextChanged(s, e) {
    var jngt = julianoNumber.GetText();
    var ingt = internalNumber.GetText();
    internalNumberConcatenated.SetText((id_certification.GetValue() !== null ? IdLote : "") + String(jngt) + String(ingt));
}

function OnProductionUnitValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


function OnTotalQuantityRecivedValidation(s, e) {
    var totalQuantityRecivedAux = e.value === null ? 0 : e.value;
    var totalQuantityLiquidationAux = totalQuantityLiquidation.GetValue();
    totalQuantityLiquidationAux = totalQuantityLiquidationAux === null ? 0 : totalQuantityLiquidationAux;
    var totalQuantityTrashAux = totalQuantityTrash.GetValue();
    totalQuantityTrashAux = totalQuantityTrashAux === null ? 0 : totalQuantityTrashAux;
    //console.log("totalQuantityRecivedAux" + totalQuantityRecivedAux);
    //console.log("totalQuantityLiquidationAux" + totalQuantityLiquidationAux);
    //console.log("totalQuantityTrashAux" + totalQuantityTrashAux);
    //console.log("(totalQuantityLiquidationAux + totalQuantityTrashAux)" + (totalQuantityLiquidationAux + totalQuantityTrashAux));
    if (totalQuantityRecivedAux < (totalQuantityLiquidationAux + totalQuantityTrashAux)) {
        e.isValid = false;
        e.errorText = "Libras Recibidas debe ser mayor o igual a la suma de las Libras Liquidadas mas las Libras de Desperdicio";
    }
}

function OnProductionProcessValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}