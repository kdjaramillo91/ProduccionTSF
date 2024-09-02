// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvBusinessTurn.AddNewRow();
}

function RemoveItems(s, e) {
    gvBusinessTurn.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvBusinessTurn.PerformCallback({ ids: selectedRows });
            gvBusinessTurn.UnselectRows();
        });
    });
}
function ImportFile(data) {
    uploadFile("BusinessTurn/ImportFileBusinessTurn", data, function (result) {
        gvBusinessTurn.Refresh();
    });
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

function RefreshGrid(s, e) {
    gvBusinessTurn.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvBusinessTurn.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvBusinessTurn.AddNewRow();
        }
    });
}

function Print(s, e) {
    gvBusinessTurn.GetSelectedFieldValues("id", function (values) {

        var _url = "BusinessTurn/BusinessTurnReport";

        var data = null;
        if (values.length === 1) {
            _url = "BusinessTurn/BusinessTurnDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvBusinessTurn.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvBusinessTurn.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvBusinessTurn.GetSelectedRowCount() > 0 && gvBusinessTurn.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvBusinessTurn.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvBusinessTurn.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvBusinessTurn.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvBusinessTurn.cpFilteredRowCountWithoutPage + gvBusinessTurn.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvBusinessTurn.SelectRows();
}

function UnselectAllRows() {
    gvBusinessTurn.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

