
function OnDivisionsCompanyValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDivisionsRucValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var validation = validarRUC(e.value);
        if (!validation.isValid) {
            e.isValid = validation.isValid;
            e.errorText = validation.errorText;
        }
    }
}

function OnDivisionsNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDivisionsAddressValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDivisionsPhoneNumberValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDivisionsEmailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var validation = validarEMAIL(e.value);
        if (!validation.isValid) {
            e.isValid = validation.isValid;
            e.errorText = validation.errorText;
        }
    }
}