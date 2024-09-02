
// USER VALIDATIONS

function OnUserNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPasswordValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnGroupValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEmissionPointValidation(s, e) {
    var emissionPoints = EmissionPoint.GetValue().split(",");
    if (emissionPoints === null || emissionPoints == "") {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnEmployeeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// USER MENU VALIDATIONS

function OnControllerValidation(s, e) {
    e.isValid = true;
}

function OnActionValidation(s, e) {
    e.isValid = true;
}

function OnAssignedValidation(s, e) {
    e.isValid = true;
}