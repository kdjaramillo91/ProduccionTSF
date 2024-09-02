
function GenerateInventoryMoveTransferEntry(s, e) {
    
    errorMessage = "";
    $("#GridMessageErrorMaterialsDetail").hide();
    btnGenerateMoveTransferEntry.SetEnabled(false);
    btnGenerateMoveTransferEntry.SetText("Procesando...");
    
    gvInventoryMoveDetailTransferExits.GetSelectedFieldValues("id;id_warehouseEntry", function (values)
    {
        btnGenerateMoveTransferEntry.SetEnabled(false);
        btnGenerateMoveTransferEntry.SetText("Procesando...");
        var isExit = false;

        selectedInventoryMoveDetailTransferExitsRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedInventoryMoveDetailTransferExitsRows.push(values[i]);
        }

        var data = {
            id: 0,
            natureMoveType: "I",
            code: "34",//Ingreso Por Transferencia
            inventoryMoveDetailTransferExitsDetails: []
        };
        var id_warehouseEntryAux = null;
        for (var i = 0; i < selectedInventoryMoveDetailTransferExitsRows.length; i++) {
            if (id_warehouseEntryAux == null) {
                id_warehouseEntryAux = selectedInventoryMoveDetailTransferExitsRows[i][1];
            } else {
                if (id_warehouseEntryAux != selectedInventoryMoveDetailTransferExitsRows[i][1]) {
                    //Error Deben seleccionarse Detalles de movimientos de Egresos por transferencia que la bodega de Ingreso sean iguales.
                    var msgErrorAux = ErrorMessage("Deben seleccionarse Detalles de movimientos de Egresos por transferencia que la bodega de Ingreso sean iguales.");
                    gridMessageErrorMaterialsDetail.SetText(msgErrorAux);
                    $("#GridMessageErrorMaterialsDetail").show();
                    isExit = true;
                    break;
                }
            }
            data.inventoryMoveDetailTransferExitsDetails.push(selectedInventoryMoveDetailTransferExitsRows[i][0]);
        }
        if (isExit)
        {
            btnGenerateMoveTransferEntry.SetText("Generar Ingreso");
            btnGenerateMoveTransferEntry.SetEnabled(true);

            return;
        }
            

        showPageCallBack("InventoryMove/InventoryMoveEditFormPartial", data, function ()
        {
            btnGenerateMoveTransferEntry.SetText("Procesar...");
            btnGenerateMoveTransferEntry.SetEnabled(false);
        }
        , function ()
        {
            btnGenerateMoveTransferEntry.SetText("Generar Ingreso");
            btnGenerateMoveTransferEntry.SetEnabled(true);
                
        });

    });
    
}

// SELECTIONS

function OnGridViewInventoryMoveDetailTransferExitsInit(s, e) {
    UpdateTitlePanelInventoryMoveDetailTransferExits();
}

function OnGridViewInventoryMoveDetailTransferExitsSelectionChanged(s, e) {

    //let index = 0;
    //
    //let id = gvInventoryMoveDetailTransferExits.cpIds[e.visibleIndex - gvInventoryMoveDetailTransferExits.GetTopVisibleIndex()];
    //
    //selectedInventoryMoveDetailTransferExitsRows.push(id);
    //s.GetSelectedFieldValues("id;id_warehouseEntry", GetSelectedFieldDetailValuesCallback);
    
    try
    {
        UpdateTitlePanelInventoryMoveDetailTransferExits();
    }
    catch (err)
    {
        console.log(err);
    }
    
}

var selectedInventoryMoveDetailTransferExitsRows = [];

function GetSelectedFieldDetailValuesCallback(values) {
    //// 
    selectedInventoryMoveDetailTransferExitsRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedInventoryMoveDetailTransferExitsRows.push(values[i]);
    }
}

function OnGridViewInventoryMoveDetailTransferExitsEndCallback(s, e) {
    UpdateTitlePanelInventoryMoveDetailTransferExits();
}

function UpdateTitlePanelInventoryMoveDetailTransferExits() {
    
    var selectedFilteredRowCount = GetSelectedFilteredInventoryMoveDetailTransferExitsRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvInventoryMoveDetailTransferExits.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryMoveDetailTransferExits.GetSelectedRowCount() - GetSelectedFilteredInventoryMoveDetailTransferExitsRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    console.log(gvInventoryMoveDetailTransferExits.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvInventoryMoveDetailTransferExits.GetSelectedRowCount() > 0 && gvInventoryMoveDetailTransferExits.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryMoveDetailTransferExits.GetSelectedRowCount() > 0);
    //}

    btnGenerateMoveTransferEntry.SetEnabled(gvInventoryMoveDetailTransferExits.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredInventoryMoveDetailTransferExitsRowCount() {
    return gvInventoryMoveDetailTransferExits.cpFilteredRowCountWithoutPage + gvInventoryMoveDetailTransferExits.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function GridViewInventoryMoveDetailTransferExitsClearSelection() {
    gvInventoryMoveDetailTransferExits.UnselectRows();
}

function GridViewPurchaseInventoryMoveDetailTransferExitsSelectAllRow() {
    gvInventoryMoveDetailTransferExits.SelectRows();
}

function inti() {

}

$(function () {

});