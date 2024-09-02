
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvIceBagRange.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvIceBagRange !== null && gvIceBagRange !== undefined) {
        gvIceBagRange.CancelEdit();
    } 
}