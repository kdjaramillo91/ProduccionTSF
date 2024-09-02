
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvInventoryControlType.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvInventoryControlType !== null && gvInventoryControlType !== undefined) {
        gvInventoryControlType.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}