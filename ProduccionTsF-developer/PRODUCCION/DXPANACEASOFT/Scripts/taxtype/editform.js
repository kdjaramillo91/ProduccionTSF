

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvTaxesTypes.UpdateEdit();
    }
}


function ButtonCancel_Click(s, e) {
    if (gvTaxesTypes !== null && gvTaxesTypes !== undefined) {
        gvTaxesTypes.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
