
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvFishingCustodian.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvFishingCustodian !== null && gvFishingCustodian !== undefined) {
        gvFishingCustodian.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}