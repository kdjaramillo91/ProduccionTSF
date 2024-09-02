// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvCostsPoundManualFactor.AddNewRow();
}

function RemoveItems(s, e) {
    gvCostsPoundManualFactor.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            $.ajax({
                url: "CostPoundManualFactor/DeleteSelectedCostsPoundManualFactor",
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
                    //$("#maincontent").html(result);
                },
                complete: function () {
                    gvCostsPoundManualFactor.PerformCallback();
                    gvCostsPoundManualFactor.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function RefreshGrid(s, e) {
    gvCostsPoundManualFactor.PerformCallback();
}

function Print(s, e) {
    console.log('Funcionalidad no implementada');
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
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
            gvCostsPoundManualFactor.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

// SELECTION

function UpdateTitlePanel() {
    btnCopy.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvCostsPoundManualFactor.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCostsPoundManualFactor.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvCostsPoundManualFactor.GetSelectedRowCount() > 0 && gvCostsPoundManualFactor.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCostsPoundManualFactor.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvCostsPoundManualFactor.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvCostsPoundManualFactor.cpFilteredRowCountWithoutPage + gvCostsPoundManualFactor.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvCostsPoundManualFactor.SelectRows();
}

function UnselectAllRows() {
    gvCostsPoundManualFactor.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});


