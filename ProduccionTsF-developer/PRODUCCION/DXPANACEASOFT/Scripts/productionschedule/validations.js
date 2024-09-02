
function OnProductionScheduleEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "productionSchedule");
}

function OnProductionSchedulePeriodValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var url = "ProductionSchedule/ValidateProductionSchedulePeriod";
        $.ajax({
            url: url,
            type: "post",
            data: { id_productionSchedulePeriod: e.value },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
                e.isValid = false;
                e.errorText = "Campo no Válido";
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log("result: ");
                //console.log(result);
                if (result !== null && result != "") {
                    if (result.code != 0) {
                        e.isValid = false;
                        e.errorText = result.message;
                    }
                } else {
                    e.isValid = false;
                    e.errorText = "Campo no Válido";
                }
            },
            complete: function () {
                //hideLoading();
            }
        });

    }
}

function OnPersonScheduleValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// TABIMAGE

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function OnInternalNumberValidation(s, e) {
    if (e.value !== null && e.value.toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
    }
}

function OnProductionUnitValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnProductionProcessValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}