

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvRates.UpdateEdit();
    }
}


function ButtonCancel_Click(s, e) {
    if (gvRates !== null && gvRates !== undefined) {
        gvRates.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
