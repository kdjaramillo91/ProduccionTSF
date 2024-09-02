// GRIDVIEW ACTIONS 

function AddNewItem(s, e) {
    gvDocumentStates.AddNewRow();
}

function RemoveItems(s, e) {
   // var c = confirm("¿Desea eliminar las categorías de grupos de productos seleccionados?");
   // if (c === true) {
    showConfirmationDialog(function() {
        $.ajax({
            url: "DocumentStates/DeleteSelectedDocumentStates",
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function(error) {
                console.log(error);
            },
            beforeSend: function() {
                showLoading();
            },
            success: function(result) {
                //$("#maincontent").html(result);
            },
            complete: function() {
                gvDocumentStates.performCallBack();
                // gvDepartament.UnselectRows();
                hideLoading();
            }
        });
    });
}
function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
        });
    }
}

function RefreshGrid(s, e) {
    gvDocumentStates.PerformCallback();
}

function Print(s, e) {
    var _url = "DocumentStates/DocumentStateReport";

    $.ajax({
        url: _url,
        type: "post",
        data: { ids: selectedRows },
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
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

// GRIDVIEW SELECTIONS

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    s.ApplyFilter('[isActive] = true');
}

var selectedRows = [];

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();

    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvDocumentStates.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvDocumentStates.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvDocumentStates.GetSelectedRowCount() > 0 && gvDocumentStates.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvDocumentStates.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvDocumentStates.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvDocumentStates.cpFilteredRowCountWithoutPage + gvDocumentStates.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

// MAIN FUNCTIONS

function init() {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvDocumentStates.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvDocumentStates.UnselectRows();
    });

    $.getScript("Scripts/documentstates/editform.js", function () { });
}

$(function () {
    init();
});