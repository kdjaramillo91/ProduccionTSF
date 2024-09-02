// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvProductionSchedulePeriods.AddNewRow();
}

function RemoveItems(s, e) {
    gvProductionSchedulePeriods.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvProductionSchedulePeriods.PerformCallback({ ids: selectedRows });
            gvProductionSchedulePeriods.UnselectRows();
        });
    });
}

function ImportFile(data) {
    //uploadFile("ProductionSchedulePeriod/ImportFileProductionSchedulePeriod", data, function (result) {
    //    gvProductionSchedulePeriods.Refresh();
    //});
}

function RefreshGrid(s, e) {
    gvProductionSchedulePeriods.Refresh();
}

var keyToCopy = null;

function CopyItems(s, e) {
    //gvProductionSchedulePeriods.GetSelectedFieldValues("id", function (values) {
    //    if (values.length === 1) {
    //        keyToCopy = values[0];
    //        gvProductionSchedulePeriods.AddNewRow();
    //    }
    //});
}

function Print(s, e) {
    //gvProductionSchedulePeriods.GetSelectedFieldValues("id", function (values) {
        
    //    var _url = "ProductionSchedulePeriod/ProductionSchedulePeriodReport";

    //    var data = null;
    //    if (values.length === 1) {
    //        _url = "ProductionSchedulePeriod/ProductionSchedulePeriodDetailReport";
    //        data = { id: values[0] };
    //    }

    //    $.ajax({
    //        url: _url,
    //        type: "post",
    //        data: data,
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            showLoading();
    //        },
    //        success: function (result) {
    //            $("#maincontent").html(result);
    //        },
    //        complete: function () {
    //            hideLoading();
    //        }
    //    });


    //});
    
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
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}


function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}


// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionSchedulePeriods.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionSchedulePeriods.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionSchedulePeriods.GetSelectedRowCount() > 0 && gvProductionSchedulePeriods.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionSchedulePeriods.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvProductionSchedulePeriods.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvProductionSchedulePeriods.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvProductionSchedulePeriods.cpFilteredRowCountWithoutPage + gvProductionSchedulePeriods.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvProductionSchedulePeriods.SelectRows();
}

function UnselectAllRows() {
    gvProductionSchedulePeriods.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});