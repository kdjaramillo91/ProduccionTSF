

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvItemTrademarkModels.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvItemTrademarkModels !== null && gvItemTrademarkModels !== undefined) {
        gvItemTrademarkModels.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}

//FUNTION COPY

