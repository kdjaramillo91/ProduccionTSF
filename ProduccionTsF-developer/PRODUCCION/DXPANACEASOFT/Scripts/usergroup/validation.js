
function OnNameValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnControllerValidation(s, e) {
    e.isValid = true;
}

function OnActionValidation(s, e) {
    e.isValid = true;
}

function OnAssignedValidation(s, e) {
    e.isValid = true;
}