
// GRIDVIEW ACTIONS

function AddNewItem(s, e) {
    gvDocumentType.AddNewRow();
}

function RemoveItems(s, e) {
   // var c = confirm("¿Desea eliminar las categorías de grupos de productos seleccionados?");
   // if (c === true) {
    showConfirmationDialog(function() {
        $.ajax({
            url: "DocumentType/DeleteSelectedDocumentType",
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
                gvDocumentType.performCallBack();
                // gvDepartament.UnselectRows();
                hideLoading();
            }
        });
    });
}

function CopyItems(s, e) {
    
}

function RefreshGrid(s, e) {
    gvDocumentType.PerformCallback();
}

function Print(s, e) {
    var _url = "DocumentType/DocumentTypeReport";

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

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
        });
    }
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

    var text = "Total de elementos seleccionados: <b>" + gvDocumentType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvDocumentType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvDocumentType.GetSelectedRowCount() > 0 && gvDocumentType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvDocumentType.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvDocumentType.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvDocumentType.cpFilteredRowCountWithoutPage + gvDocumentType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

// MAIN FUNCTIONS

function init() {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvDocumentType.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvDocumentType.UnselectRows();
    });

    $.getScript("Scripts/documenttype/editform.js", function () { });
}

$(function () {
    init();
});