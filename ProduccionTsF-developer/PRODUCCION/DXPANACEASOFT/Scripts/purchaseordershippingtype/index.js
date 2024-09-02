// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvPurchaseOrderShippingType.AddNewRow();
}

function RemoveItems(s, e) {
    gvPurchaseOrderShippingType.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvPurchaseOrderShippingType.PerformCallback({ ids: selectedRows });
            gvPurchaseOrderShippingType.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvPurchaseOrderShippingType.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvPurchaseOrderShippingType.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("PurchaseOrderShippingType/ImportFilePurchaseOrderShippingType", data, function (result) {
        gvPurchaseOrderShippingType.Refresh();
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
    gvPurchaseOrderShippingType.Refresh();
}

function Print(s, e) {
    gvPurchaseOrderShippingType.GetSelectedFieldValues("id", function (values) {

        var _url = "PurchaseOrderShippingType/PurchaseOrderShippingTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "PurchaseOrderShippingType/PurchaseOrderShippingTypeDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrderShippingType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrderShippingType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrderShippingType.GetSelectedRowCount() > 0 && gvPurchaseOrderShippingType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrderShippingType.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvPurchaseOrderShippingType.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvPurchaseOrderShippingType.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvPurchaseOrderShippingType.cpFilteredRowCountWithoutPage + gvPurchaseOrderShippingType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvPurchaseOrderShippingType.SelectRows();
}

function UnselectAllRows() {
    gvPurchaseOrderShippingType.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

