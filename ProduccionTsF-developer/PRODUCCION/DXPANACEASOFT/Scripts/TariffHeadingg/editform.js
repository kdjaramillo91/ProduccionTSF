

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvTariffHeadingg.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvTariffHeadingg !== null && gvTariffHeadingg !== undefined) {
        gvTariffHeadingg.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}