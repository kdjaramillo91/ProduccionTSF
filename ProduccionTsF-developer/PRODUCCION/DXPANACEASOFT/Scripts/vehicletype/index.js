// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvVehicleType.AddNewRow();
}

function RemoveItems(s, e) {
    gvVehicleType.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvVehicleType.PerformCallback({ ids: selectedRows });
            gvVehicleType.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvVehicleType.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvVehicleType.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvVehicleType.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {
 
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

function ImportFile() {
    
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit() {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvVehicleType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvVehicleType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvVehicleType.GetSelectedRowCount() > 0 && gvVehicleType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvVehicleType.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvVehicleType.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvVehicleType.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvVehicleType.cpFilteredRowCountWithoutPage + gvVehicleType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvVehicleType.SelectRows();
}

function UnselectAllRows() {
    gvVehicleType.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});