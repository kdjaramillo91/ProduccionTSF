// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvCategory.AddNewRow();
}

function RemoveItems(s, e) {
    gvCategory.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvCategory.PerformCallback({ ids: selectedRows });
            gvCategory.UnselectRows();
        });
    });
}
function ImportFile(data) {
    uploadFile("Category/ImportFileCategory", data, function (result) {
        gvCategory.Refresh();
    });
}
function RefreshGrid(s, e) {
    gvCategory.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvCategory.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvCategory.AddNewRow();
        }
    });
}

function Print(s, e) {
    gvCategory.GetSelectedFieldValues("id", function (values) {

        var _url = "Category/CategoryReport";

        var data = null;
        if (values.length === 1) {
            _url = "Category/CategoryDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvCategory.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCategory.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvCategory.GetSelectedRowCount() > 0 && gvCategory.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCategory.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvCategory.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvCategory.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvCategory.cpFilteredRowCountWithoutPage + gvCategory.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvCategory.SelectRows();
}

function UnselectAllRows() {
    gvCategory.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

