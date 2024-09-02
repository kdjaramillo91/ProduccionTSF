
//COMBOS
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvCertifications.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvCertifications !== null && gvCertifications !== undefined) {
        gvCertifications.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}