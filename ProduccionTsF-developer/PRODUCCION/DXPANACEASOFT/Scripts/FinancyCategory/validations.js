
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


            var id = $("#id_Bank").val();

            $.ajax({
                url: "FinancyCategory/ReptCodigo",
                type: "post",
                data: {
                    id_Bank: $("#id_Bank").val(),
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
                        e.errorText = "Ya existe una Categoría con el mismo Codigo ";
                        e.isValid = false;
                    } else {

                    }


                },
                complete: function () {
                }
            });



            //     var data = "id=" + id + "&" +  $("#FormEditBank").serialize();
        }

    }
}
function OnNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


















