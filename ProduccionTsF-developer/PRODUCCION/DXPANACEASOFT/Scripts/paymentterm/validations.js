
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


            var id = $("#id_PaymentTerm").val();

            $.ajax({
                url: "PaymentTerm/ReptCodigo",
                type: "post",
                data: {
                    id_PaymentTerm: $("#id_PaymentTerm").val(),
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
                        e.errorText = "Ya existe una Forma de Pago con el mismo Codigo ";
                        e.isValid = false;
                    } else {

                    }


                },
                complete: function () {
                }
            });



            //     var data = "id=" + id + "&" +  $("#FormEditPaymentTerm").serialize();
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
function OndescriptionEnglishValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnfirstPaymentDays(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


function OnnumberPeriods(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPeriodType(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnPercentAnticipation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPercentBalance(s, e) {
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












