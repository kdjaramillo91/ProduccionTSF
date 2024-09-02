// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvTransportSize.AddNewRow();
}

function RemoveItems(s, e) {
    gvTransportSize.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvTransportSize.PerformCallback({ ids: selectedRows });
            gvTransportSize.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvTransportSize.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvTransportSize.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvTransportSize.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {

}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


function ImportFile(data) {
    uploadFile("TransportSize/ImportFileTransportSize", data, function (result) {
        gvTransportSize.Refresh();
    });
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

    var text = "Total de elementos seleccionados: <b>" + gvTransportSize.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTransportSize.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvTransportSize.GetSelectedRowCount() > 0 && gvTransportSize.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTransportSize.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvTransportSize.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvTransportSize.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvTransportSize.cpFilteredRowCountWithoutPage + gvTransportSize.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvTransportSize.SelectRows();
}

function UnselectAllRows() {
    gvTransportSize.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});