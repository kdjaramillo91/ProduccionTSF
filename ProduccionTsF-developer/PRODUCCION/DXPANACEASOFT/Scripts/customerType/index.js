// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvCustomerType.AddNewRow();
}

function RemoveItems(s, e) {
    gvCustomerType.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvCustomerType.PerformCallback({ ids: selectedRows });
            gvCustomerType.UnselectRows();
        });
    });
}
function ImportFile(data) {
    uploadFile("CustomerType/ImportFileCustomerType", data, function (result) {
        gvCustomerType.Refresh();
    });
}
function RefreshGrid(s, e) {
    gvCustomerType.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvCustomerType.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvCustomerType.AddNewRow();
        }
    });
}

function Print(s, e) {
    gvCustomerType.GetSelectedFieldValues("id", function (values) {

        var _url = "CustomerType/CustomerTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "CustomerType/CustomerTypeDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvCustomerType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCustomerType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvCustomerType.GetSelectedRowCount() > 0 && gvCustomerType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCustomerType.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvCustomerType.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvCustomerType.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvCustomerType.cpFilteredRowCountWithoutPage + gvCustomerType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvCustomerType.SelectRows();
}

function UnselectAllRows() {
    gvCustomerType.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

