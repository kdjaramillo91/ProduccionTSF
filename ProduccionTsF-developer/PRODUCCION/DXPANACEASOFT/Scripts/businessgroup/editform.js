
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvBusinessGroups.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvBusinessGroups !== null && gvBusinessGroups !== undefined) {
        gvBusinessGroups.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
