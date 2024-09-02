
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if(valid) {
        tvMenu.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    tvMenu.CancelEdit();
}

// COMBOBOX ACTIONS EVENTES

function ComboBoxController_SelectedIndexChanged(s, e) {

    id_action.ClearItems();
    var item = id_controller.GetSelectedItem();

    if(item !== null) {
        $.ajax({
            url: "Menu/GetActionsByController",
            type: "post",
            data: { id_controller: item.value },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                for(var i = 0; i < result.length; i++) {
                    id_action.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }

}