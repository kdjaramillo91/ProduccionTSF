

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvVehicle.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvVehicle !== null && gvVehicle !== undefined) {
        gvVehicle.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}

