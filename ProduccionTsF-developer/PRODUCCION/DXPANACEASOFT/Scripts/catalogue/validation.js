function OnCatalogueNameValidation(s, e) {
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



function OnCatalogueTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCodeCatalogueValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 50) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        } else {
            $.ajax({
                url: "Catalogue/ValidateCodeCatalogue",
                type: "post",
                async: false,
                cache: false, data: {
                    id_catalogue: gvCatalogues.cpEditingRowKey,
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

function OnInitDateDesde() {
    var d = new Date();
    dateStart.SetDate(new Date(d.getFullYear(), d.getMonth(), 1));
}

function OnInitDateHasta() {
    dateEnd.SetDate(new Date());
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, dateStart.GetValue(), dateEnd.GetValue(), "Rango de Fecha no válido");
}

