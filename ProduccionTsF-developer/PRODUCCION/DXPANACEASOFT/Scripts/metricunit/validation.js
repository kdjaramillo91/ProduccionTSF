function OnMetricUnitNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 50) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        }
    }
}

function OnMetricUnitCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "MetricUnit/ValidateCodeMetricUnit",
                type: "post",
                async: false,
                cache: false, data: {
                    id_metricUnit: gvMetricUnits.cpEditingRowKey,
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
    }
}

function OnMetricUnitMetricTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnMetricUnitCompanyValidation(e, s) {
    if (e.value === null) {
        e.isvalid = false;
        e.errorText = "Campo Obligatorio";
    }

}