
//COMBOS
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvBusinessTurn.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvBusinessTurn !== null && gvBusinessTurn !== undefined) {
        gvBusinessTurn.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}