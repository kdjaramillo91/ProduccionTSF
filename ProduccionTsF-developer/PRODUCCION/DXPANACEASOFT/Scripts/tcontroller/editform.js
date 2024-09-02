

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {

        var tcontroller = {
            id: $("#id_tcontroller").val(),
            name: controladorName.GetText(),
            description: description.GetText(),
            isActive: isActive.GetChecked()
        };

        //var tcontroller = "id=" + $("#id_tcontroller").val() + "&" + $("#formEditTController").serialize();

        var url = (tcontroller.id === "0") ? "TController/TControllerPartialAddNew"
                                       : "TController/TControllerPartialUpdate";

        $.ajax({
            url: url,
            type: "post",
            data: tcontroller,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.code < 0) {
                    $("#tcontrollerErrorMessage").html(result.message);
                    $("#tcontrollerAlertRow").css("display", "");
                    return;
                }

                //Todo: Set all Empty
                gvTController.CancelEdit();
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function ButtonCancel_Click(s, e)
{
    if (gvTController !== null && gvTController !== undefined) {
        gvTController.CancelEdit();
    } 
}