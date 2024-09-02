
//COMBOS
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvCategory.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvCategory !== null && gvCategory !== undefined) {
        gvCategory.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}