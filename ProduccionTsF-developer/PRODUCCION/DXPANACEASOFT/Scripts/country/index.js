// BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvCountry.AddNewRow();
}

function RemoveItems(s, e) {
    gvCountry.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvCountry.PerformCallback({ ids: selectedRows });
            gvCountry.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvCountry.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvCountry.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("Country/ImportFileCountry", data, function (result) {
        gvCountry.Refresh();
    });
}

function Print(s, e) {
    
    gvCountry.GetSelectedFieldValues("id", function (values) {

        var _url = "Country/CountryReport";

        var data = null;
        if (values.length === 1) {
            _url = "Country/CountryDetailReport";
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
    gvCountry.Refresh();
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

    var text = "Total de elementos seleccionados: <b>" + gvCountry.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCountry.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    
    SetElementVisibility("lnkSelectAllRows", gvCountry.GetSelectedRowCount() > 0 && gvCountry.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCountry.GetSelectedRowCount() > 0);
 

    btnRemove.SetEnabled(gvCountry.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvCountry.GetSelectedRowCount() === 1);
}
function GetSelectedFilteredRowCount() {
    return gvCountry.cpFilteredRowCountWithoutPage + gvCountry.GetSelectedKeysOnPage().length;
}
function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvCountry.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvCountry.UnselectRows();
    });
});
function SelectAllRows() {
    gvCountry.SelectRows();
}

function UnselectAllRows() {
    gvCountry.UnselectRows();
}

// MAIN FUNCTIONS

function init() {
   
}

$(function () {
    init();
});

