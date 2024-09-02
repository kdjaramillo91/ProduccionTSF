// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvTAction.AddNewRow();
}

function RemoveItems(s, e) {
    gvTAction.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            $.ajax({
                url: "TAction/DeleteSelectedTAction",
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
                    //$("#maincontent").html(result);
                },
                complete: function () {
                    gvTAction.PerformCallback();
                    gvTAction.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvTAction.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

function RefreshGrid(s, e) {
    gvTAction.PerformCallback();
}

function Print(s, e) {
    var _url = "Company/CompanyReport";

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

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    s.ApplyFilter('[isActive] = true');
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvTAction.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTAction.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvTAction.GetSelectedRowCount() > 0 && gvTAction.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTAction.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvTAction.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvTAction.cpFilteredRowCountWithoutPage + gvTAction.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvTAction.SelectRows();
}

function UnselectAllRows() {
    gvTAction.UnselectRows();
}

function init() {
    
}

$(function () {
    init();
});

