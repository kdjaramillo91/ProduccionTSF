function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvInventoryValuationMethod.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvInventoryValuationMethod !== null && gvInventoryValuationMethod !== undefined) {
        gvInventoryValuationMethod.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}