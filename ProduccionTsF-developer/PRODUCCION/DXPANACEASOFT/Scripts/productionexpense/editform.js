
//COMBOS

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvProductionExpense.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvProductionExpense !== null && gvProductionExpense !== undefined) {
        gvProductionExpense.CancelEdit();
    }
}