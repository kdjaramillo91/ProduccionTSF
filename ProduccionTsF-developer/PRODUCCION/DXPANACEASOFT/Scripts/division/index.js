// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvDivisions.AddNewRow();
}

function RemoveItems(s, e) {
  
    gvDivisions.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            $.ajax({
                url: "Division/DeleteSelectedDivisions",
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
                    gvDivisions.PerformCallback();
                    gvDivisions.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function RefreshGrid(s, e) {
    gvDivisions.PerformCallback();
}

//btnPrint
function Print(s, e) {
    var _url = "Division/DivisionReport";

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
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvDivisions.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

// SELECTION

function UpdateTitlePanel() {
	btnNew.SetEnabled(false);
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvDivisions.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvDivisions.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvDivisions.GetSelectedRowCount() > 0 && gvDivisions.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvDivisions.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvDivisions.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvDivisions.cpFilteredRowCountWithoutPage + gvDivisions.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvDivisions.SelectRows();
}

function UnselectAllRows() {
    gvDivisions.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});