// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvPurchaseReason.AddNewRow();
}

function RemoveItems(s, e) {
    gvPurchaseReason.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvPurchaseReason.PerformCallback({ ids: selectedRows });
            gvPurchaseReason.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvPurchaseReason.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvPurchaseReason.AddNewRow();
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
    gvPurchaseReason.Refresh();
}

function ImportFile(data) {
    uploadFile("PurchaseReason/ImportFilePurchaseReason", data, function (result) {
        gvPurchaseReason.Refresh();
    });
}


function Print(s, e) {
    gvPurchaseReason.GetSelectedFieldValues("id", function (values) {

        var _url = "PurchaseReason/PurchaseReasonReport";

        var data = null;
        if (values.length === 1) {
            _url = "PurchaseReason/PurchaseReasonDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseReason.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseReason.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvPurchaseReason.GetSelectedRowCount() > 0 && gvPurchaseReason.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseReason.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvPurchaseReason.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvPurchaseReason.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvPurchaseReason.cpFilteredRowCountWithoutPage + gvPurchaseReason.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvPurchaseReason.SelectRows();
}

function UnselectAllRows() {
    gvPurchaseReason.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

