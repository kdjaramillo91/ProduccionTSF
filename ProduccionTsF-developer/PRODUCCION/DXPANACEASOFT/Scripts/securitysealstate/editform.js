

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvSecuritySealState.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvSecuritySealState !== null && gvSecuritySealState !== undefined) {
        gvSecuritySealState.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}