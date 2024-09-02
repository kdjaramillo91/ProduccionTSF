function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
        if (valid) {
            gvProductionUnit.UpdateEdit();
        }
    }
}

function ButtonCancel_Click(s, e) {
    if (gvProductionUnit !== null && gvProductionUnit !== undefined) {
        gvProductionUnit.CancelEdit();
    }
}