function OnBranchOfficesCompanyValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCodeBranchOfficesValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var itemDivision = id_division.GetSelectedItem();
        if (itemDivision !== null && itemDivision !== undefined) {
            if(e.value.length !== 3) {
                e.isValid = false;
                e.errorText = "Longitud de 3 digitos requerida";
            } else if(e.value === 0 || e.value === "000") {
                e.isValid = false;
                e.errorText = "Valor incorrecto";
            } else {
                $.ajax({
                    url: "BranchOffice/ValidateCode",
                    type: "post",
                    async: false,
                    cache: false,
                    data: {
                        id_division: itemDivision.value,
                        id_branchOffice: gvBranchOffices.cpEditingRowKey,
                        code: e.value
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
        } else {
            e.isValid = false;
            e.errorText = "Debe seleccionar división antes de definir el código";
        }
    }
}

function OnBranchOfficesDivisionValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnBranchOfficesRucValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }  else {
        if (e.value.length > 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        }
    }
}


function OnBranchOfficesNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 51) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        }
    }
}

function OnBranchOfficesAddressValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnBranchOfficesEmailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 51) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        }
    }
}

function OnBranchOfficesPhoneValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        }
    }
}