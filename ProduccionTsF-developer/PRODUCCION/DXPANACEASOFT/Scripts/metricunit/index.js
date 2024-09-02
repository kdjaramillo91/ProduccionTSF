
// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvMetricUnits.AddNewRow();
}


function RefreshGrid(s, e) {
    gvMetricUnits.Refresh();
}

function RemoveItems(s, e) {
    gvMetricUnits.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvMetricUnits.PerformCallback({ ids: selectedRows });
            gvMetricUnits.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvMetricUnits.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvMetricUnits.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("MetricUnit/ImportFileMetricUnit", data, function (result) {
        gvMetricUnits.Refresh();
    });
}

function Print(s, e) {
    gvMetricUnits.GetSelectedFieldValues("id", function (values) {

        var _url = "MetricUnit/CountryMetricUnit";

        var data = null;
        if (values.length === 1) {
            _url = "MetricUnit/MetricUnitDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvMetricUnits.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvMetricUnits.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvMetricUnits.GetSelectedRowCount() > 0 && gvMetricUnits.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvMetricUnits.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvMetricUnits.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvMetricUnits.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvMetricUnits.cpFilteredRowCountWithoutPage + gvMetricUnits.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvMetricUnits.SelectRows();
}

function UnselectAllRows() {
    gvMetricUnits.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});


