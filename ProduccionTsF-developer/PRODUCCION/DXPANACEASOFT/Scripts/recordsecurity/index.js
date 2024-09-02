// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvRecordSecurity.AddNewRow();
}

function RemoveItems(s, e) {
    gvRecordSecurity.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvRecordSecurity.PerformCallback({ ids: selectedRows });
            gvRecordSecurity.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvRecordSecurity.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvRecordSecurity.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvRecordSecurity.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {
    //gvRecordSecurity.GetSelectedFieldValues("id", function (values) {

    //    var _url = "Warehouse/WarehouseReport";

    //    var data = null;
    //    if (values.length === 1) {
    //        _url = "Warehouse/WarehouseDetailReport";
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

function ImportFile(data) {
    //uploadFile("Warehouses/ImportFileWarehouses", data, function (result) {
    //    gvRecordSecurity.Refresh();
    //});
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit() {
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
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

function isActiveInit() {
	//isActive.SetValue(true);
}
// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRecordSecurity.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRecordSecurity.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvRecordSecurity.GetSelectedRowCount() > 0 && gvRecordSecurity.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRecordSecurity.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvRecordSecurity.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvRecordSecurity.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvRecordSecurity.cpFilteredRowCountWithoutPage + gvRecordSecurity.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvRecordSecurity.SelectRows();
}

function UnselectAllRows() {
    gvRecordSecurity.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});