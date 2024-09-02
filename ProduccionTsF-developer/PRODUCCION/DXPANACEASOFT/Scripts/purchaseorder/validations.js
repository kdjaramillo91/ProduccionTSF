
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

function OnQuantityRequestedValidation(s, e) {
    e.isValid = true;
}

function OnQuantityReceivedValidation(s, e) {
    e.isValid = true;
}

function OnEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "purchaseOrder");
}

//function ItemValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo obligatorio";
//    }
//}



//function QuantityApprovedValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo obligatorio";
//    } else if (e.value !== null && e.value.toString().length > 20) {
//        e.isValid = false;
//        e.errorText = "Máximo 20 caracteres";
//    } else if (e.value <= 0) {
//        e.isValid = false;
//        e.errorText = "Valor Incorrecto";
//    }
//}

//function PriceValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo obligatorio";
//    } else if (e.value !== null && e.value.toString().length > 20) {
//        e.isValid = false;
//        e.errorText = "Máximo 20 caracteres";
//    } else if (e.value <= 0) {
//        e.isValid = false;
//        e.errorText = "Valor Incorrecto";
//    }
//}

function OnProviderValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


function OnProductionUnitProviderValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPriceListValidation(s, e) {

    var code_purchaseReason = $("#code_purchaseReason").val();
    if (code_purchaseReason == "MP") {
        if (e.value === null) {
            e.isValid = false;
            e.errorText = "Campo Obligatorio";
        }
    }
}

function OnBuyerValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


function OnDeliveryDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDeliveryhourValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPaymentTermValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPaymentMethodValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnShippingTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnReasonValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


function OnFishingSiteValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnLogisticValidation(s, e) {
    e.isValid = true;
}

function OnImportationValidation(s, e) {
    e.isValid = true;
}

// IMPORTATION INFORMATION VALIDATION

function OnCustomsDocumentNumberValidation(s, e) {
    if (typeof isImportation === 'undefined' || !isImportation.GetChecked()) {
        e.isValid = true;
    } else if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnReferendumNumberValidation(s, e) {
    if (typeof isImportation === 'undefined' || !isImportation.GetChecked()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnShipmentDateValidation(s, e) {
    if (typeof isImportation === 'undefined' || !isImportation.GetChecked()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDepartureCustomsDateValidation(s, e) {
    if (typeof isImportation === 'undefined' || !isImportation.GetChecked()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnArrivalDateValidation(s, e) {
    if (typeof isImportation === 'undefined' || !isImportation.GetChecked()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

