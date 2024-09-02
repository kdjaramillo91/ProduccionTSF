var rowKeySelected;

function ButtonUpdate_Click(s, e)
{
    var settingNotification = {
        id: idSettingNotification.GetText(),
        title: title.GetText(),
        description: description.GetText(),
        id_documentType: id_documentType.GetValue(),
        id_documentState: id_documentState.GetValue(),
        sendByMail: sendByMail.GetChecked(),
        addressesMails: addressesMails.GetText()
    };

    $.ajax({
        url: "SettingNotification/UpdateSettingNotification",
        type: "post",
        data: settingNotification,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        success: function (result) {
            gvSettingNotification.CancelEdit();
        },
    });
}

function ButtonCancel_Click(s, e) {
    gvSettingNotification.CancelEdit();
}

function AddItem() {
    gvSettingNotification.AddNewRow();
}

function EditItem(s, e) {
    if (rowKeySelected != undefined)
        gvSettingNotification.StartEditRow(rowKeySelected);
}

function DeleteItems() {

    if (confirm("Seguro que desea eliminar los elementos seleccionados?")) {

        var keys = gvSettingNotification.GetSelectedKeysOnPage();

        $.ajax({
            url: "SettingNotification/DeleteNotifications",
            type: "post",
            data: { ids: keys },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                gvSettingNotification.CancelEdit();
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function On_GridView_DobleClick(s, e) {
    gvSettingNotification.StartEditRow(e.visibleIndex);
}

function On_GridView_SelectionChanged(s, e) {
    var keys = gvSettingNotification.GetSelectedKeysOnPage();
    if (keys.length == 0) {
        btnEdit.SetEnabled(false);
        btnDelete.SetEnabled(false);
    } else {
        btnEdit.SetEnabled(true);
        btnDelete.SetEnabled(true);
    }

    rowKeySelected = e.visibleIndex;
}

$(function () {
    rowKeySelected = undefined;
});
