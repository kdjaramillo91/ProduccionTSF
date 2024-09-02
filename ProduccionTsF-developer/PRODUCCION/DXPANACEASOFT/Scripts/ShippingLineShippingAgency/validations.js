
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




function OnstartDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnShippingLine(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {



        $.ajax({
            url: "ShippingLineShippingAgency/ReptCodigo",
            type: "post",
            data: {
                id: $("#id_ShippingLineShippingAgency").val(),
                id_ShippingLine: e.value,
                id_ShippingAgency: id_ShippingAgency.GetValue()
               
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
                    e.errorText = "Ya existe una Agencia y una Linea Naviera ";
                    e.isValid = false;
                } else {

                }


            },
            complete: function () {
            }
        });



        //     var data = "id=" + id + "&" +  $("#FormEditShippingAgency").serialize();
    }


   
}

function OnShippingAgency(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {



        $.ajax({
            url: "ShippingLineShippingAgency/ReptCodigo",
            type: "post",
            data: {
                id: $("#id_ShippingLineShippingAgency").val(),
                id_ShippingLine: id_ShippingLine.GetValue(),
                id_ShippingAgency: e.value

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
                    e.errorText = "Ya existe una Agencia y una Linea Naviera ";
                    e.isValid = false;
                } else {

                }


            },
            complete: function () {
            }
        });



        //     var data = "id=" + id + "&" +  $("#FormEditShippingAgency").serialize();
    }
}

function OnendDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}











