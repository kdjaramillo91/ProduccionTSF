function ButtonCancel_Click(s, e) {
    if (gvWarehouses !== null && gvWarehouses !== undefined) {
        gvWarehouses.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}