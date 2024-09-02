// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvPriceListType.AddNewRow();
}

function RemoveItems(s, e) {
    gvPriceListType.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvPriceListType.PerformCallback({ ids: selectedRows });
            gvPriceListType.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvPriceListType.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvPriceListType.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

function RefreshGrid(s, e) {
    gvPriceListType.Refresh();
}

function ImportFile(data) {
    uploadFile("PriceListType/ImportFilePriceListType", data, function (result) {
        gvPriceListType.Refresh();
    });
}

function Print(s, e) {
    gvPriceListType.GetSelectedFieldValues("id", function (values) {

        var _url = "PriceListType/PriceListTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "PriceListType/PriceListTypeDetailReport";
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


function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPriceListType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPriceListType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvPriceListType.GetSelectedRowCount() > 0 && gvPriceListType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPriceListType.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvPriceListType.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvPriceListType.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvPriceListType.cpFilteredRowCountWithoutPage + gvPriceListType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvPriceListType.SelectRows();
}

function UnselectAllRows() {
    gvPriceListType.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

