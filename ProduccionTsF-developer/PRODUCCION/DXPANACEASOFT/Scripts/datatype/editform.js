function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvDataTypes.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvDataTypes !== null && gvDataTypes !== undefined) {
        gvDataTypes.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
