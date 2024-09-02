
//COMBOS
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvTypeMachineForProd.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvTypeMachineForProd !== null && gvTypeMachineForProd !== undefined) {
        gvTypeMachineForProd.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}