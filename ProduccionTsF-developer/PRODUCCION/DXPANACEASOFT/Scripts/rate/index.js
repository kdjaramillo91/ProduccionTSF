// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvRates.AddNewRow();
}

function RemoveItems(s, e) {
    gvRates.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvRates.PerformCallback({ ids: selectedRows });
            gvRates.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvRates.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvRates.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function RefreshGrid(s, e) {
    gvRates.Refresh();
}

function ImportFile(data) {
    uploadFile("Rate/ImportFileRate", data, function (result) {
        gvRates.Refresh();
    });
}

function Print(s, e) {
    gvRates.GetSelectedFieldValues("id", function (values) {

        var _url = "Rate/RateReport";

        var data = null;
        if (values.length === 1) {
            _url = "Rate/RateDetailReport";
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

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
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

    var text = "Total de elementos seleccionados: <b>" + gvRates.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRates.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvRates.GetSelectedRowCount() > 0 && gvRates.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRates.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvRates.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvRates.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvRates.cpFilteredRowCountWithoutPage + gvRates.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvRates.SelectRows();
}

function UnselectAllRows() {
    gvRates.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});