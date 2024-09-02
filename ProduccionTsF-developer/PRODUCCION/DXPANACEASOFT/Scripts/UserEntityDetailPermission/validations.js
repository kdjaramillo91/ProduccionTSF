
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
function OnWarehouse(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {


        //$.ajax({
        //    url: "UserEntityDetailPermission/ReptCodigo",
        //    type: "post",
        //    data: {
        //        id: $("#gvUserEntityDetail").val(),
        //        id_user: id_user.GetValue(),
        //        id_warehouse: id_entityValue.GetValue()
               
        //    },
        //    async: false,
        //    cache: false,
        //    error: function (error) {
        //        console.log(error);
        //    },
        //    beforeSend: function () {
        //    },
        //    success: function (result) {
        //        if (result != null && result.rept) {
        //            e.errorText = "Ya existe una Bodega ";
        //            e.isValid = false;
        //        } else {

        //        }


        //    },
        //    complete: function () {
        //    }
        //});



        //     var data = "id=" + id + "&" +  $("#FormEditShippingAgency").serialize();
    }


   
}

function OnUser(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        // 
        $.ajax({
            url: "UserEntityDetailPermission/ReptCodigo",
            type: "post",
            data: {
                id: $("#gvUserEntityDetail").val(),
                id_user: id_user.GetValue(),
                //id_warehouse: id_entityValue.GetValue()

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











