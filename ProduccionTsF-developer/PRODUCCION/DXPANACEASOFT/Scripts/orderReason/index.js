// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvOrderReason.AddNewRow();
}

function RemoveItems(s, e) {
    gvOrderReason.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvOrderReason.PerformCallback({ ids: selectedRows });
            gvOrderReason.UnselectRows();
        });
    });
}
function ImportFile(data) {
    uploadFile("OrderReason/ImportFileOrderReason", data, function (result) {
        gvOrderReason.Refresh();
    });
}
function RefreshGrid(s, e) {
    gvOrderReason.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvOrderReason.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvOrderReason.AddNewRow();
        }
    });
}

function Print(s, e) {
    gvOrderReason.GetSelectedFieldValues("id", function (values) {

        var _url = "OrderReason/OrderReasonReport";

        var data = null;
        if (values.length === 1) {
            _url = "OrderReason/OrderReasonDetailReport";
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

function OnGridViewInit() {
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

    var text = "Total de elementos seleccionados: <b>" + gvOrderReason.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvOrderReason.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvOrderReason.GetSelectedRowCount() > 0 && gvOrderReason.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvOrderReason.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvOrderReason.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvOrderReason.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvOrderReason.cpFilteredRowCountWithoutPage + gvOrderReason.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvOrderReason.SelectRows();
}

function UnselectAllRows() {
    gvOrderReason.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

