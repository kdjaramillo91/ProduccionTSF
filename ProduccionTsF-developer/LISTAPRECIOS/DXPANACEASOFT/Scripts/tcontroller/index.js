// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvTController.AddNewRow();
}

function RemoveItems(s, e) {
    gvTController.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            $.ajax({
                url: "TController/DeleteSelectedTController",
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
                    gvTController.PerformCallback();
                    gvTController.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvTController.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

function RefreshGrid(s, e) {
    gvTController.PerformCallback();
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

    var text = "Total de elementos seleccionados: <b>" + gvTController.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTController.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvTController.GetSelectedRowCount() > 0 && gvTController.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTController.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvTController.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvTController.cpFilteredRowCountWithoutPage + gvTController.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvTController.SelectRows();
}

function UnselectAllRows() {
    gvTController.UnselectRows();
}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {
    init();
});


