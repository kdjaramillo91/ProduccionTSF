// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvTransportTariffType.AddNewRow();
}

function RemoveItems(s, e) {
    gvTransportTariffType.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvTransportTariffType.PerformCallback({ ids: selectedRows });
            gvTransportTariffType.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvTransportTariffType.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvTransportTariffType.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvTransportTariffType.AddNewRow();
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
    uploadFile("TransportTariffType/ImportFileTransportTariffType", data, function (result) {
        gvTransportTariffType.Refresh();
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

    var text = "Total de elementos seleccionados: <b>" + gvTransportTariffType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTransportTariffType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvTransportTariffType.GetSelectedRowCount() > 0 && gvTransportTariffType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTransportTariffType.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvTransportTariffType.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvTransportTariffType.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvTransportTariffType.cpFilteredRowCountWithoutPage + gvTransportTariffType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvTransportTariffType.SelectRows();
}

function UnselectAllRows() {
    gvTransportTariffType.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});