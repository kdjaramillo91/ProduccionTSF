
//COMBOS
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvOrderReason.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvOrderReason !== null && gvOrderReason !== undefined) {
        gvOrderReason.CancelEdit();
    } 
}