
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
function OnCategory(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {



        $.ajax({
            url: "CategoryCustomerType/ReptCodigo",
            type: "post",
            data: {
                id: $("#id_CategoryCustomerType").val(),
                id_Category: e.value,
                id_CustomerType: id_CustomerType.GetValue()
               
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
                    e.errorText = "Ya existe una Categoría y un tipo de Cliente";
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

function OnCustomerType(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {



        $.ajax({
            url: "CategoryCustomerType/ReptCodigo",
            type: "post",
            data: {
                id: $("#id_CategoryCustomerType").val(),
                id_Category: id_Category.GetValue(),
                id_CustomerType: e.value

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
                    e.errorText = "Ya existe una categoría y un tipo de cliente ";
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











