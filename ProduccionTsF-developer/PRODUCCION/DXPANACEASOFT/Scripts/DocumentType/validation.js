function OnDocumentTypeCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }else if (e.value.length > 10) {
        e.isValid = false;
        e.errorText = "Ingrese solo 10 Digitos";
    }
}
function OnDocumentTypeNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnDocumentTypeDayofExpirationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}