
function OnEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "salesOrder");
}

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

function OnQuantityDeliveredValidation(s, e) {
    e.isValid = true;
}

function ItemValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

function OnQuantityOrderedValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        if (e.value <= 0) {
            e.isValid = false;
            e.errorText = "Valor Incorrecto";
        }
    }


}

function PriceValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }

    if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
    }
}

function OnCustomerValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPriceListValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEmployeeSellerValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

//function OnSendToValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnDeliveryToValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnDeliveryDateValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnPaymentTermValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnPaymentMethodValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnShippingTypeValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnReasonValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

function OnLogisticValidation(s, e) {
    e.isValid = true;
}

//function OnImportationValidation(s, e) {
//    e.isValid = true;
//}

// IMPORTATION INFORMATION VALIDATION

//function OnCustomsDocumentNumberValidation(s, e) {
//    if (!isImportation.GetChecked()) {
//        e.isValid = true;
//    } else if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnReferendumNumberValidation(s, e) {
//    if (!isImportation.GetChecked()) {
//        e.isValid = true;
//    } else if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnShipmentDateValidation(s, e) {
//    if (!isImportation.GetChecked()) {
//        e.isValid = true;
//    } else if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnDepartureCustomsDateValidation(s, e) {
//    if (!isImportation.GetChecked()) {
//        e.isValid = true;
//    } else if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnArrivalDateValidation(s, e) {
//    if (!isImportation.GetChecked()) {
//        e.isValid = true;
//    } else if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

