
//COMBOS

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvRecordSecurity.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvRecordSecurity !== null && gvRecordSecurity !== undefined) {
        gvRecordSecurity.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}