
function OnCompaniesBusinessGroupValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCompaniesRucValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCompaniesBusinessNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCompaniesTrademarkValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCompaniesAddressValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCompaniesEmailValidation(s, e) {
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

function OnCompaniesPhoneNumberValidation(s, e) {
    if(e.value===null ) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// ELECTRONIC FACTURATION

function OnEnvironmentTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEmissionTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnResolutionNumberValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnRiseValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCertificateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPasswordValidation(s, e) {

    var id_company = parseInt($("#id_company").val());
    
    if (id_company === 0 && e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}