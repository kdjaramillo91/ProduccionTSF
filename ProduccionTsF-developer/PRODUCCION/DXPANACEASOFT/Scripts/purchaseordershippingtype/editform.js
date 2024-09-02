
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvPurchaseOrderShippingType.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvPurchaseOrderShippingType !== null && gvPurchaseOrderShippingType !== undefined) {
        gvPurchaseOrderShippingType.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
