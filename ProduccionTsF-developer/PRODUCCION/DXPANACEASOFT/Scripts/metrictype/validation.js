function OnMetricTypeNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        if (e.value.length > 50) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        }
    }
 }

function OnMetricTypeDataTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

function OnCodeMetricTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "MetricType/ValidateCodeMetricType",
                type: "post",
                async: false,
                cache: false, data: {
                    id_metricType: gvMetricTypes.cpEditingRowKey,
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