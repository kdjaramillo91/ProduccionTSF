

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvRemissionGuideAssignedStaffRol.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvRemissionGuideAssignedStaffRol !== null && gvRemissionGuideAssignedStaffRol !== undefined) {
        gvRemissionGuideAssignedStaffRol.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
