
// VALIDATIONS

function OnSendedDestinationQuantityValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
    }
}

function OnArrivalDestinationQuantityValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
    }
}

function OnArrivalGoodConditionValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
    }
}

function OnArrivalBadConditionValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
    }
}