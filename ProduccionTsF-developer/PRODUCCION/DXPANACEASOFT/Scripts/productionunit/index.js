// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvProductionUnit.AddNewRow();
}

function RemoveItems(s, e) {
    gvProductionUnit.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            $.ajax({
                url: "ProductionUnit/DeleteSelectedProductionUnit",
                type: "post",
                data: { ids: selectedRows },
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                },
                complete: function () {
                    gvProductionUnit.PerformCallback();
                    gvProductionUnit.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function ImportFile(data) {
    uploadFile("ProductionUnit/ImportFileProductionUnit", data, function (result) {
        gvProductionUnit.Refresh();
    });
}

function RefreshGrid(s, e) {
    gvProductionUnit.Refresh();
}

function Print(s, e) {
    var _url = "ProductionUnit/ProductionUnitsReport";

    $.ajax({
        url: _url,
        type: "post",
        data: null,
        async: true,
        cache: false,
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
            gvProductionUnit.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
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

    var text = "Total de elementos seleccionados: <b>" + gvProductionUnit.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionUnit.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvProductionUnit.GetSelectedRowCount() > 0 && gvProductionUnit.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionUnit.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvProductionUnit.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvProductionUnit.cpFilteredRowCountWithoutPage + gvProductionUnit.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvProductionUnit.SelectRows();
}

function UnselectAllRows() {
    gvProductionUnit.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

