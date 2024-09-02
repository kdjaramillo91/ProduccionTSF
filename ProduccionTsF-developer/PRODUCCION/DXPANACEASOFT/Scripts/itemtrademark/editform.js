

function GridViewItemTrademarkItemTrademarkModelsDetails_BeginCallback(s, e) {
    e.customArgs["id_itemTrademark"] = $("#id_itemTrademark").val();
}

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvItemTrademarks.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvItemTrademarks !== null && gvItemTrademarks !== undefined) {
        gvItemTrademarks.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}