
function OnAñoValidation(s, e) {
    if (e.value == null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnAñoValueChanged(s, e) {
    ValidateCode(s, e);
}

function OnMesValidation(s, e) {
    if (e.value == null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCodeValidation(s, e) {
    var value = s.GetText();
    if (value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        ValidateCode(s, e);
	}
}

function OnMesValueChanged(s, e) {
    ValidateCode(s, e);
}

function OnCategoryValidation(s, e) {
    var value = s.GetText();
    if (value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (value.length != 3) {
        e.isValid = false;
        e.errorText = "Debe tener 3 carácteres";
	}
}

function OnCodeTextChanged(s, e) {
    ValidateCode(s, e);
}

function OnNameValidation(s, e) {
    var value = s.GetText();
    if (value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (value.length <= 0) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCodTiposItemValidation(s, e) {

}

function OnValorValidation(s, e) {
    var value = s.GetText();
    if (value == null ) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function ValidateCode(s, e) {
    $.ajax({
        url: "CostPoundManualFactor/ValidateCodeCostPoundManualFactor",
        type: "post",
        async: false,
        cache: false, data: {
            id: gvCostsPoundManualFactor.cpEditingRowKey,
            año: año.GetValue(),
            mes: mes.GetValue(),
            code: code.GetText(),
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