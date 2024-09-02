
function NotifyError(Message) {

    DevExpress.ui.notify(Message, "error");
}

function NotifySuccess(Message) {

    DevExpress.ui.notify(Message, "success");
}

function NotifyWarning(Message) {

    DevExpress.ui.notify(Message, "warning");
}

function NotifyDialog(Message) {
    DevExpress.ui.dialog.alert(Message, "Alerta");
}
