
function NotifyError(Message, Time) {
    if (Time !== undefined || Time != null) {
        DevExpress.ui.notify(Message, "error", Time);
    }
    else {
        DevExpress.ui.notify(Message, "error");
    }

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
