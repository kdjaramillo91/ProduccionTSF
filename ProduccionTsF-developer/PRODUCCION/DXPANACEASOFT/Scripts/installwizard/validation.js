// WELCOME

function OnAcceptTermsAndConditionsValidation(s, e) {
    if (e.value === null || e.value === false) {
        e.isValid = false;
        e.errorText = "Debe aceptar los terminos y condiciones";
    }
}

//Company

function OnBusinessNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnTradeMarkValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnRucValidation(s, e) {
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

function OnEmailValidation(s, e) {
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


function OnAddressValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPhoneNumberValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


//Structure

function OnStructureValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnBranchOfficeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCodeBranchOfficeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEmissionPointValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCodeEmissionPointValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

//Security

function OnUserAdminValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


function OnPasswordAdminValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


// Funciones RUC y EMAIL

function validarRUC(ruc) {
 
    var regExp = new RegExp("[^0-9]", "i");

    if (ruc.length != 13) {
        return { isValid: false, errorText: "El ruc debe tener 13 dígitos" };
    }

    if (regExp.test(ruc)) {
        return { isValid: false, errorText: "Solo se admiten dígitos" };
    }

    var tipoPersona = parseInt(ruc[2]);
    if (tipoPersona > 6 && tipoPersona != 9) {
        return { isValid: false, errorText: "El 3er dígito del ruc debe ser menor que 6 ó 9" };
    }

    var provCode = parseInt(ruc.substr(0, 2));
    if (provCode < 1 || provCode > 24) {
        return { isValid: false, errorText: "Código de provincia emisora errado" };
    }

    if (ruc.substr(10, 13) === "000") {
        return { isValid: false, errorText: "Los tres últimos dígitos no pueden ser 000" };
    }

    var digitoAutoverificador = 0;
    var coeficientes = [];

    if (tipoPersona < 6) {
        digitoAutoverificador = ruc[9];
        coeficientes = [2, 1, 2, 1, 2, 1, 2, 1, 2];

        var sum = 0;
        for (var i = 0; i < coeficientes.length; i++) {
            var value = parseInt(coeficientes[i]) * parseInt(ruc[i]);
            if (value > 9) {
                sum += value % 10 + 1;
            } else {
                sum += value;
            }
        }

        var residuo = sum % 10;
        if (residuo == 0 && digitoAutoverificador == 0) {
            return { isValid: true, errorText: null };
        }

        if (10 - sum % 10 != digitoAutoverificador) {
            return { isValid: false, errorText: "Número de RUC incorrecto" };
        }

    } else if (tipoPersona == 6) {
        var error = false;
        digitoAutoverificador = ruc[8];
        coeficientes = [3, 2, 7, 6, 5, 4, 3, 2];

        var sum = 0;
        for (var i = 0; i < coeficientes.length; i++) {
            var value = parseInt(coeficientes[i]) * parseInt(ruc[i]);
            sum += value;
        }

        var residuo = sum % 11;

        if (residuo == 0 && digitoAutoverificador == 0) {
            return { isValid: true, errorText: null };
        }

        if (11 - sum % 11 != digitoAutoverificador) {
            return { isValid: false, errorText: "Número de RUC incorrecto" };
        }
    }
    else {
        digitoAutoverificador = ruc[9];
        coeficientes = [4, 3, 2, 7, 6, 5, 4, 3, 2];

        var sum = 0;
        for (var i = 0; i < coeficientes.length; i++) {
            var value = parseInt(coeficientes[i]) * parseInt(ruc[i]);
            sum += value;
        }

        var residuo = sum % 11;

        if (residuo == 0 && digitoAutoverificador == 0) {
            return { isValid: true, errorText: null };
        }

        if (11 - sum % 11 != digitoAutoverificador) {
            error = true;
            //return { isValid: false, errorText: "Número de RUC incorrecto" };
        }

        if (error) {
            digitoAutoverificador = ruc[9];
            coeficientes = [2, 1, 2, 1, 2, 1, 2, 1, 2];

            var sum = 0;
            for (var i = 0; i < coeficientes.length; i++) {
                var value = parseInt(coeficientes[i]) * parseInt(ruc[i]);
                if (value > 9) {
                    sum += value % 10 + 1;
                } else {
                    sum += value;
                }
            }

            var residuo = sum % 10;
            if (residuo == 0 && digitoAutoverificador == 0) {
                return { isValid: true, errorText: null };
            }

            if (10 - sum % 10 != digitoAutoverificador) {
                return { isValid: false, errorText: "Número de RUC incorrecto" };
            }

        }

    }

    return { isValid: true, errorText: null };
}


function validarEMAIL(email) {
    var regexp = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

    if (!regexp.test(email)) {
        return { isValid: false, errorText: "Correo electrónico incorrecto" };
    }

    return { isValid: true, errorText: null };
}