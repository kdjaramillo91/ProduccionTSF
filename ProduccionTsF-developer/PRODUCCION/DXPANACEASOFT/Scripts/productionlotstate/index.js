// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvProductionLotState.AddNewRow();
}

function RemoveItems(s, e) {
    gvProductionLotState.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            gvProductionLotState.PerformCallback({ ids: selectedRows });
            gvProductionLotState.UnselectRows();
        });
    });
}

function ImportFile(data) {
    uploadFile("ProductionLotState/ImportFileProductionLotState", data, function (result) {
        gvProductionLotState.Refresh();
    });
}

function RefreshGrid(s, e) {
    gvProductionLotState.Refresh();
}

function Print(s, e) {
    var _url = "ProductionLotState/ProductionLotStateReport";

    $.ajax({
        url: _url,
        type: "post",
        data: null,
        async: true, cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#maincontent").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvProductionLotState.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    s.ApplyFilter('[isActive] = true');
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotState.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotState.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvProductionLotState.GetSelectedRowCount() > 0 && gvProductionLotState.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionLotState.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvProductionLotState.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvProductionLotState.cpFilteredRowCountWithoutPage + gvProductionLotState.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvProductionLotState.SelectRows();
}

function UnselectAllRows() {
    gvProductionLotState.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

