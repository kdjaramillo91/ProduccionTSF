function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvDepartments.UpdateEdit();
    }
}


function ButtonCancel_Click(s, e) {
    if (gvDepartments !== null && gvDepartments !== undefined) {
        gvDepartments.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
