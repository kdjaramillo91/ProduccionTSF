

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvCountry.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvCountry !== null && gvCountry !== undefined) {
        gvCountry.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}