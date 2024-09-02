var rowKeySelected;

function ButtonRemove_Click (s, e) {

    if (confirm("Seguro que desea eliminar los elementos seleccionados?")) {

        $.ajax({
            url: "Notification/DeleteNotification",
            type: "post",
            data: { id:  $("#id").val()},
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                gvNotification.CancelEdit();
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ButtonUpdate_Click(s, e) {

    var notification = {
        id: idNotification.GetText(),
        reading: reading.GetChecked()
    };

    $.ajax({
        url: "Notification/UpdateNotification",
        type: "post",
        data: notification,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        success: function (result) {
            gvNotification.CancelEdit();
        },
    });
}

function ButtonCancel_Click(s, e) {
    gvNotification.CancelEdit();
}

function EditItem(s, e) {
    gvNotification.StartEditRow(rowKeySelected);
}

function On_GridView_DobleClick(s, e) {
    gvNotification.StartEditRow(e.visibleIndex);
}

function DeleteItems() {
    
    if (confirm("Seguro que desea eliminar los elementos seleccionados?")) {

        var keys = gvNotification.GetSelectedKeysOnPage();

        $.ajax({
            url: "Notification/DeleteNotifications",
            type: "post",
            data: { ids: keys},
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                gvNotification.CancelEdit();
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function On_GridView_SelectionChanged(s, e) {
    var keys = gvNotification.GetSelectedKeysOnPage();
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
