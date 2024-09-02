

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvProductiveHoursReason.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvProductiveHoursReason !== null && gvProductiveHoursReason !== undefined) {
        gvProductiveHoursReason.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}