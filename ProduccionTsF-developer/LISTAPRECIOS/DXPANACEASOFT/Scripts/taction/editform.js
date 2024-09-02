
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        
        //var taction = "id=" + $("#id_taction").val() + "&" + $("#formEditTAction").serialize();

        var taction = {
            id: $("#id_taction").val(),
            id_controller: id_controller.GetValue(),
            name: tactionName.GetText(),
            description: description.GetText(),
            isActive: isActive.GetChecked()
        };

        var url = (taction.id === "0") ? "TAction/TActionPartialAddNew"
                                       : "TAction/TActionPartialUpdate";

        $.ajax({
            url: url,
            type: "post",
            data: taction,
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
                    $("#tactionErrorMessage").html(result.message);
                    $("#tactionAlertRow").css("display", "");
                    return;
                }

                //Todo: Set all Empty
                gvTAction.CancelEdit();
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function ButtonCancel_Click(s, e) {
    if (gvTAction !== null && gvTAction !== undefined) {
        gvTAction.CancelEdit();
    }
}

//function ComboBoxController_SelectedIndexChanged(s, e) {
//    //ruc.SetText("");
//    //address.SetText("");
//    //phoneNumber.SetText("");
//    //email.SetText("");
//    //id_division.ClearItems();

//    var item = id_controller.GetSelectedItem();

//    if (item !== null && item !== undefined) {
//        $.ajax({
//            url: "TAction/TControllerByTAction",
//            type: "post",
//            data: { id_controller: item.value },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                //showLoading();
//            },
//            success: function (result) {
//                for (var i = 0; i < result.length; i++) {
//                    id_controller.AddItem(result[i].name, result[i].id);
//                }
//            },
//            complete: function () {
//                //hideLoading();
//            }
//        });
//    }

//}