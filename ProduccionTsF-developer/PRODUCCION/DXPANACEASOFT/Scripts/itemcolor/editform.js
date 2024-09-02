//Button

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvItemColors.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvItemColors !== null && gvItemColors !== undefined) {
        gvItemColors.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
