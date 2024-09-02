
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
function OnPaymentMethode(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {



        $.ajax({
            url: "PaymentMethodPaymentTerm/ReptCodigo",
            type: "post",
            data: {
                id: $("#id_PaymentMethodPaymentTerm").val(),
                id_paymentMethod: e.value,
                id_paymentTerm: id_paymentTerm.GetValue()
               
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
                    e.errorText = "Ya existe una Metodo y una Forma de Pago ";
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

function OnpaymentTerm(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {



        $.ajax({
            url: "PaymentMethodPaymentTerm/ReptCodigo",
            type: "post",
            data: {
                id: $("#id_PaymentMethodPaymentTerm").val(),
                id_paymentMethod: id_paymentMethod.GetValue(),
                id_paymentTerm: e.value

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
                    e.errorText = "Ya existe un Metodo y una Forma de Pago ";
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











