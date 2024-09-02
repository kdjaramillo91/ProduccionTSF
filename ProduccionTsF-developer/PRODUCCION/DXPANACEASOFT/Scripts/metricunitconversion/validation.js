function OnMetricUnitConversionMetricOriginValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnMetricUnitConversionMetricDestinyValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnMetricUnitConversionFactorValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Factor Incorrecto";
    }
}

