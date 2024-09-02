 



function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvPoundsRange.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvPoundsRange !== null && gvPoundsRange !== undefined) {
        gvPoundsRange.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}