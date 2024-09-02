function OnEmissionDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnPriceListValidation(s, e) {
    var incorrect = false
    var messageErrorText = "";
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var data = { id_priList: id_priceList.GetValue(), id_procType: $("#id_procType").val() };
        $.ajax({
            url: "AdvanceProvider/ValidateProcessType",
            type: "post",
            data: data,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                if (result !== null) {
                    if (result.areDiferentProc == "YES") {
                        incorrect = true;
                        messageErrorText = "El Tipo de Proceso de la lista de Precio debe ser el mismo que el Proceso del Lote";
                    } else if (result.isListApproved == "NO") {
                        incorrect = true;
                        messageErrorText = "La Lista de Precios debe estar aprobada por Gerencia Comercial";
                    }
                }
            },
            complete: function () {
            }
        });
    }
    if (incorrect == true) {
        e.isValid = false;
        e.errorText = messageErrorText;
    }
    UpdateTabImage({ isValid: !incorrect }, "tabAdvanceProviderPL");
}