
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


            var id = $("#id_InventoryReason").val();

            $.ajax({
                url: "InventoryReason/ReptCodigo",
                type: "post",
                data: {
                    id_InventoryReason: $("#id_InventoryReason").val(),
                    codio: e.value
                },
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                },
                success: function (result) {
                    if (result != null && result.rept) {
                        e.errorText = "Ya existe una Linea con el mismo Codigo ";
                        e.isValid = false;
                    } else {

                    }


                },
                complete: function () {
                }
            });



            //     var data = "id=" + id + "&" +  $("#FormEditInventoryReason").serialize();
        }

    }
}
function OnNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnDescriptionValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OndocumentTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnNatureMoveReason(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnValorizationReason(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnTypeOfCalculationReason(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnInventoryReasonRelated(s, e) {
    if (typeOfCalculation.GetText() === "Heredado") {
        if (e.value === null) {
            e.isValid = false;
            e.errorText = "Campo Obligatorio";
        }
    } else {
        e.isValid = true;
    }
   
}











