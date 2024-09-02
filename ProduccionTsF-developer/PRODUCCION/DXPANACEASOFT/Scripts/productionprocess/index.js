// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvProductionProcess.AddNewRow();
}

function RemoveItems(s, e) {
    gvProductionProcess.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvProductionProcess.PerformCallback({ ids: selectedRows });
            gvProductionProcess.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvProductionProcess.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvProductionProcess.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function RefreshGrid(s, e) {
    gvProductionProcess.Refresh();
}

function ImportFile(data) {
    uploadFile("ProductionProcess/ImportFileProductionProcess", data, function (result) {
        gvProductionProcess.Refresh();
    });
}

function Print(s, e) {
    gvProductionProcess.GetSelectedFieldValues("id", function (values) {

        var _url = "ProductionProcess/ProductionProcessReport";

        var data = null;
        if (values.length === 1) {
            _url = "ProductionProcess/ProductionProcessDetailReport";
            data = { id: values[0] };
        }
        $.ajax({
            url: _url,
            type: "post",
            data: data,
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


    });

}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    s.ApplyFilter('[isActive] = true');
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}
function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionProcess.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionProcess.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvProductionProcess.GetSelectedRowCount() > 0 && gvProductionProcess.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionProcess.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvProductionProcess.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvProductionProcess.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvProductionProcess.cpFilteredRowCountWithoutPage + gvProductionProcess.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvProductionProcess.SelectRows();
}

function UnselectAllRows() {
    gvProductionProcess.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

