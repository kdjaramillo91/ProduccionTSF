
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvPurchaseReason.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvPurchaseReason !== null && gvPurchaseReason !== undefined) {
        gvPurchaseReason.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}