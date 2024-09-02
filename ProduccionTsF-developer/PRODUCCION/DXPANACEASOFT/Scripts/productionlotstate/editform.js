function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvProductionLotState.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvProductionLotState !== null && gvProductionLotState !== undefined) {
        gvProductionLotState.CancelEdit();
    }
}