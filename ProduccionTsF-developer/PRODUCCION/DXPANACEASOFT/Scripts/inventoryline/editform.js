function GridViewInventoryLineItemTypesDetails_BeginCallback(s, e) {
    e.customArgs["id_inventoryLine"] = $("#id_inventoryLine").val();
}

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvInventoryLines.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvInventoryLines !== null && gvInventoryLines !== undefined) {
        gvInventoryLines.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}

function PrintDocument(s, e) {
    var data = {
        id: $("#id_inventoryLine").val()};

    showPage("InventoryLine/InventoryLineDetailReport", data);
}