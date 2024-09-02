
//COMBOS
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvCustomerType.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvCustomerType !== null && gvCustomerType !== undefined) {
        gvCustomerType.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}