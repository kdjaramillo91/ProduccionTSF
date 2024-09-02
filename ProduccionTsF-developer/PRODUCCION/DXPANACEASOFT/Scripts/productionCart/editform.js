

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
		gvProductionCart.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
	if (gvProductionCart !== null && gvProductionCart !== undefined) {
		gvProductionCart.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}