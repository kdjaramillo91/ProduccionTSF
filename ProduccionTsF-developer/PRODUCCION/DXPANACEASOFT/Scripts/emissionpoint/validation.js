
function OnEmissionPointsDivisionValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEmissionPointsBrachofficeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEmissionPointsCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 6) {
            e.isValid = false;
            e.errorText = "Máximo de caracteres";
        } else {
            $.ajax({
                url: "EmissionPoint/ValidateCodeEmissionPoint",
                type: "post",
                async: false,
                cache: true,
                data: {
                    id_emissionPoint: gvEmissionPoints.cpEditingRowKey,
                    code: e.value,
                    id_documentType: id_documentType.GetValue()

                },
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    e.isValid = result.isValid;
                    e.errorText = result.errorText;
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }
    }
}







function OnEmissionPointsNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (e.value.length > 50) {
        e.isValid = false;
        e.errorText = "Máximo 50 caracteres";
    }
}
function OnEmissionPointsAddressValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (e.value.length > 100) {
        e.isValid = false;
        e.errorText = "Máximo 100 caracteres";
    }
}

function OnEmissionPointsEmailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (e.value.length > 50) {
        e.isValid = false;
        e.errorText = "Máximo 50 caracteres";
    }
}

function OnEmissionPointsPhoneValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (e.value.length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
    }
}