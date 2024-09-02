 
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvTransportSize.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvTransportSize !== null && gvTransportSize !== undefined) {
        gvTransportSize.CancelEdit();
    } 
}