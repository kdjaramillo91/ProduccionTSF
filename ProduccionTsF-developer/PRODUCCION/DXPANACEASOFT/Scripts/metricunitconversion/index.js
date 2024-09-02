

// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvMetricUnitConversions.AddNewRow();
}


function RemoveItems(s, e) {
    gvMetricUnitConversions.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvMetricUnitConversions.PerformCallback({ ids: selectedRows });
            gvMetricUnitConversions.UnselectRows();
        });
    });
}
function RefreshGrid(s, e) {
    gvMetricUnitConversions.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvMetricUnitConversions.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvMetricUnitConversions.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("MetricUnitConversion/ImportFileMetricUnitConversion", data, function (result) {
        gvMetricUnitConversions.Refresh();
    });
}

function Print(s, e) {
    gvMetricUnitConversions.GetSelectedFieldValues("id", function (values) {

        var _url = "MetricUnitConversion/MetricUnitConversionReport";

        var data = null;
        if (values.length === 1) {
            _url = "MetricUnitConversion/MetricUnitConversionDetailReport";
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


// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
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

    var text = "Total de elementos seleccionados: <b>" + gvMetricUnitConversions.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvMetricUnitConversions.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvMetricUnitConversions.GetSelectedRowCount() > 0 && gvMetricUnitConversions.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvMetricUnitConversions.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvMetricUnitConversions.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvMetricUnitConversions.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvMetricUnitConversions.cpFilteredRowCountWithoutPage + gvMetricUnitConversions.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvMetricUnitConversions.SelectRows();
}

function UnselectAllRows() {
    gvMetricUnitConversions.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

