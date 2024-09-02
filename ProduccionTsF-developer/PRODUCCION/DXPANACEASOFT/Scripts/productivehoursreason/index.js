// BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvProductiveHoursReason.AddNewRow();
}

function RemoveItems(s, e) {
    gvProductiveHoursReason.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvProductiveHoursReason.PerformCallback({ ids: selectedRows });
            gvProductiveHoursReason.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvProductiveHoursReason.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvProductiveHoursReason.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("ProductiveHoursReason/ImportFileProductiveHoursReason", data, function (result) {
        gvProductiveHoursReason.Refresh();
    });
}

function Print(s, e) {
    
    gvProductiveHoursReason.GetSelectedFieldValues("id", function (values) {

        var _url = "ProductiveHoursReason/ProductiveHoursReasonReport";

        var data = null;
        if (values.length === 1) {
            _url = "ProductiveHoursReason/ProductiveHoursReasonDetailReport";
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
                var w = window.open();
                $(w.document.body).html(result);
                //window.open(_url)
                //$("#maincontent").html(result);
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

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {   
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();    
        });
    }
}

function RefreshGrid(s, e) {
    gvProductiveHoursReason.Refresh();
}


// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    s.ApplyFilter('[isActive] = true');
}


function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function GetSelectedFieldValuesCallback(values) {
    selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
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

    var text = "Total de elementos seleccionados: <b>" + gvProductiveHoursReason.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductiveHoursReason.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    
    SetElementVisibility("lnkSelectAllRows", gvProductiveHoursReason.GetSelectedRowCount() > 0 && gvProductiveHoursReason.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductiveHoursReason.GetSelectedRowCount() > 0);
 

    btnRemove.SetEnabled(gvProductiveHoursReason.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvProductiveHoursReason.GetSelectedRowCount() === 1);
}
function GetSelectedFilteredRowCount() {
    return gvProductiveHoursReason.cpFilteredRowCountWithoutPage + gvProductiveHoursReason.GetSelectedKeysOnPage().length;
}
function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvProductiveHoursReason.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvProductiveHoursReason.UnselectRows();
    });
});
function SelectAllRows() {
    gvProductiveHoursReason.SelectRows();
}

function UnselectAllRows() {
    gvProductiveHoursReason.UnselectRows();
}

// MAIN FUNCTIONS

function init() {
   
}

$(function () {
    init();
});

