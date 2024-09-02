// BUTTONS

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvCostsPoundManualFactor.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvCostsPoundManualFactor !== null && gvCostsPoundManualFactor !== undefined) {
        gvCostsPoundManualFactor.CancelEdit();
    }
}
