var errorMessage = "";

function OnstartDateValidation(s, e) {
    //gridMessageErrorPurchaseOrderDetail.SetText(result.Message);
    errorMessage = "";
    $("#gridMessageError").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";

        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- La Fecha de Inicio: Es obligatoria.";
        } else {
            errorMessage += "</br>- La Fecha de Inicio: Es obligatoria.";
        }

        if (errorMessage != null && errorMessage != "") {
            var msgErrorAux = ErrorMessage(errorMessage);
            gridMessageError.SetText(msgErrorAux);
            $("#gridMessageError").show();

        }
      
    } 
}

function OnendDateValidation(s, e) {
    //gridMessageErrorPurchaseOrderDetail.SetText(result.Message);
    errorMessage = "";
    $("#gridMessageError").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";

        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- La Fecha de Fin: Es obligatoria.";
        } else {
            errorMessage += "</br>- La Fecha de Fin: Es obligatoria.";
        }

        if (errorMessage != null && errorMessage != "") {
            var msgErrorAux = ErrorMessage(errorMessage);
            gridMessageError.SetText(msgErrorAux);
            $("#gridMessageError").show();

        }

    }
}

function OnPeriodStateValidation(s, e) {
    //gridMessageErrorPurchaseOrderDetail.SetText(result.Message);
    errorMessage = "";
    $("#gridMessageError").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";

        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- El estado : Es obligatoria.";
        } else {
            errorMessage += "</br>- El estado: Es obligatoria.";
        }

        if (errorMessage != null && errorMessage != "") {
            var msgErrorAux = ErrorMessage(errorMessage);
            gridMessageError.SetText(msgErrorAux);
            $("#gridMessageError").show();

        }

    }
}

