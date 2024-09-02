
// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvMetricTypes.AddNewRow();
}

function RemoveItems(s, e) {
    gvMetricTypes.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvMetricTypes.PerformCallback({ ids: selectedRows });
            gvMetricTypes.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvMetricTypes.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvMetricTypes.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function RefreshGrid(s, e) {
    gvMetricTypes.Refresh();
}
function Print(s, e) {
    gvMetricTypes.GetSelectedFieldValues("id", function (values) {

        var _url = "MetricType/MetricTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "MetricType/MetricTypeDetailReport";
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


function ImportFile(data) {
    uploadFile("MetricType/ImportFileMetricType", data, function (result) {
        gvMetricTypes.Refresh();
    });
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
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
            gvMetricTypes.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvMetricTypes.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvMetricTypes.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvMetricTypes.GetSelectedRowCount() > 0 && gvMetricTypes.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvMetricTypes.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvMetricTypes.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvMetricTypes.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvMetricTypes.cpFilteredRowCountWithoutPage + gvMetricTypes.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvMetricTypes.SelectRows();
}

function UnselectAllRows() {
    gvMetricTypes.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

