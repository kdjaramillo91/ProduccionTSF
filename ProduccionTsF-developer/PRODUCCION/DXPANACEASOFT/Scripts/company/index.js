// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvCompanies.AddNewRow();
}

function RemoveItems(s, e) {
    gvCompanies.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            $.ajax({
                url: "Company/DeleteSelectedCompanies",
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
                    gvCompanies.PerformCallback();
                    gvCompanies.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function RefreshGrid(s, e) {
    gvCompanies.PerformCallback();
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


// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    //s.ApplyFilter('[isActive] = true');
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();

    //s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvCompanies.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

// SELECTION

function UpdateTitlePanel() {
	btnNew.SetEnabled(false);
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvCompanies.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCompanies.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvCompanies.GetSelectedRowCount() > 0 && gvCompanies.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCompanies.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvCompanies.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvCompanies.cpFilteredRowCountWithoutPage + gvCompanies.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvCompanies.SelectRows();
}

function UnselectAllRows() {
    gvCompanies.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});


