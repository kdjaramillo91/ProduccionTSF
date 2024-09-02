function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvEconomicGroups.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvEconomicGroups !== null && gvEconomicGroups !== undefined) {
        gvEconomicGroups.CancelEdit();
    }
}