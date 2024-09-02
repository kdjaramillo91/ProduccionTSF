
// COMMON TABS FUNCTIOS VALIDATIONS

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }

    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}


function OnCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var noValido = / /;
        if (noValido.test(e.value)) { // se chequea el regex de que el string no tenga espacio
            e.errorText = " no puede contener espacios en blanco";
            e.isValid = false;
        }
        else {
            
            var id = $("#id_Turn").val();

            $.ajax({
                url: "Turn/ReptCodigo",
                type: "post",
                data: {
                    id_Turn: $("#id_Turn").val(),
                    codigo: e.value
                },
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                },
                success: function (result) {
                    if (result !== null && result.rept) {
                        e.errorText = "Ya existe un Turno con el mismo Codigo ";
                        e.isValid = false;
                    }                 },
                complete: function () {
                }
            });
        }
    }
}
function OnNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnTimeInitValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Ingrese valor";
    }
}

function OnTimeEndValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

















