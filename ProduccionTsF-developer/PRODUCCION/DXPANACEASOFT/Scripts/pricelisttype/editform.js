

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvPriceListType.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvPriceListType !== null && gvPriceListType !== undefined) {
        gvPriceListType.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}