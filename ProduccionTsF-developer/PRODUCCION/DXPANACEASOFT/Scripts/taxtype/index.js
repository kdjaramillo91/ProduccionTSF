// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvTaxesTypes.AddNewRow();
}


function RemoveItems(s, e) {
    gvTaxesTypes.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvTaxesTypes.PerformCallback({ ids: selectedRows });
            gvTaxesTypes.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvTaxesTypes.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvTaxesTypes.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function RefreshGrid(s, e) {
    gvTaxesTypes.Refresh();
}

function ImportFile(data) {
    uploadFile("TaxType/ImportFileTaxType", data, function (result) {
        gvTaxesTypes.Refresh();
    });
}

function Print(s, e) {
    gvTaxesTypes.GetSelectedFieldValues("id", function (values) {

        var _url = "TaxType/TaxTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "TaxType/TaxTypeDetailReport";
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

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
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


// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvTaxesTypes.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTaxesTypes.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvTaxesTypes.GetSelectedRowCount() > 0 && gvTaxesTypes.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTaxesTypes.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvTaxesTypes.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvTaxesTypes.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvTaxesTypes.cpFilteredRowCountWithoutPage + gvTaxesTypes.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvTaxesTypes.SelectRows();
}

function UnselectAllRows() {
    gvTaxesTypes.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});