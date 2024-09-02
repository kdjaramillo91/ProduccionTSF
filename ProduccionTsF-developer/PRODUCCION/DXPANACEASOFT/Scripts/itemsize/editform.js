

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvItemSizes.UpdateEdit();
    }
}


function ButtonCancel_Click(s, e) {
    if (gvItemSizes !== null && gvItemSizes !== undefined) {
        gvItemSizes.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
