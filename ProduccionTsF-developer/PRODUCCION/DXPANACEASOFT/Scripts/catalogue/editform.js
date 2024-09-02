

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvCatalogues.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvCatalogues !== null && gvCatalogues !== undefined) {
        gvCatalogues.CancelEdit();
    }
}

