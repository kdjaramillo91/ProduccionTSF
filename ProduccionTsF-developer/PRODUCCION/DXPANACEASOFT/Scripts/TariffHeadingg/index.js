// BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvTariffHeadingg.AddNewRow();
}

function RemoveItems(s, e) {
    gvTariffHeadingg.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvTariffHeadingg.PerformCallback({ ids: selectedRows });
            gvTariffHeadingg.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvTariffHeadingg.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvTariffHeadingg.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("TariffHeadingg/ImportFileTariffHeadingg", data, function (result) {
        gvTariffHeadingg.Refresh();
    });
}

function Print(s, e) {
    
    gvTariffHeadingg.GetSelectedFieldValues("id", function (values) {

        var _url = "TariffHeadingg/TariffHeadinggReport";

        var data = null;
        if (values.length === 1) {
            _url = "TariffHeadingg/TariffHeadinggDetailReport";
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
    gvTariffHeadingg.Refresh();
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

    var text = "Total de elementos seleccionados: <b>" + gvTariffHeadingg.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTariffHeadingg.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    
    SetElementVisibility("lnkSelectAllRows", gvTariffHeadingg.GetSelectedRowCount() > 0 && gvTariffHeadingg.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTariffHeadingg.GetSelectedRowCount() > 0);
 

    btnRemove.SetEnabled(gvTariffHeadingg.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvTariffHeadingg.GetSelectedRowCount() === 1);
}
function GetSelectedFilteredRowCount() {
    return gvTariffHeadingg.cpFilteredRowCountWithoutPage + gvTariffHeadingg.GetSelectedKeysOnPage().length;
}
function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvTariffHeadingg.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvTariffHeadingg.UnselectRows();
    });
});
function SelectAllRows() {
    gvTariffHeadingg.SelectRows();
}

function UnselectAllRows() {
    gvTariffHeadingg.UnselectRows();
}

// MAIN FUNCTIONS

function init() {
   
}

$(function () {
    init();
});

