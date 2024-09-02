// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvInventoryLines.AddNewRow();
}

function RemoveItems(s, e) {
    gvInventoryLines.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvInventoryLines.PerformCallback({ ids: selectedRows });
            gvInventoryLines.UnselectRows();
        });
    });
}

function ImportFile(data) {
    uploadFile("InventoryLine/ImportFileInventoryLine", data, function (result) {
        gvInventoryLines.Refresh();
    });
}

function RefreshGrid(s, e) {
    gvInventoryLines.Refresh();
}

var keyToCopy = null;

function CopyItems(s, e) {
    gvInventoryLines.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvInventoryLines.AddNewRow();
        }
    });
}

function Print(s, e) {
    gvInventoryLines.GetSelectedFieldValues("id", function (values) {
        
        var _url = "InventoryLine/InventoryLineReport";

        var data = null;
        if (values.length === 1) {
            _url = "InventoryLine/InventoryLineDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvInventoryLines.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryLines.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvInventoryLines.GetSelectedRowCount() > 0 && gvInventoryLines.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryLines.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvInventoryLines.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvInventoryLines.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvInventoryLines.cpFilteredRowCountWithoutPage + gvInventoryLines.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvInventoryLines.SelectRows();
}

function UnselectAllRows() {
    gvInventoryLines.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});