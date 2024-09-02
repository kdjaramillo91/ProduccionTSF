
function OnTitleValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPositionValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnControllerValditation(s, e) {
    e.isValid = true;
}

function OnActionValidation(s, e) {

    var item = id_controller.GetSelectedItem();

    if (item !== null && e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}