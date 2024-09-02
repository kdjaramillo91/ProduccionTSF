
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvMetricUnits.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvMetricUnits !== null && gvMetricUnits !== undefined) {
        gvMetricUnits.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
