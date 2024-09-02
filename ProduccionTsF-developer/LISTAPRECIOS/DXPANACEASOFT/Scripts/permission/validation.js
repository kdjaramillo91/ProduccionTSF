
function OnPermissionNameValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}