// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvEmissionPoints.AddNewRow();
}

function RemoveItems(s, e) {
    gvEmissionPoints.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvEmissionPoints.PerformCallback({ ids: selectedRows });
            gvEmissionPoints.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvEmissionPoints.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvEmissionPoints.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function RefreshGrid(s, e) {
    gvEmissionPoints.Refresh();
}

function ImportFile(data) {
    uploadFile("EmissionPoint/ImportFileEmissionPoint", data, function (result) {
        gvEmissionPoints.Refresh();
    });
}

function Print(s, e) {
    gvEmissionPoints.GetSelectedFieldValues("id", function (values) {

        var _url = "EmissionPoint/EmissionPointReport";

        var data = null;
        if (values.length === 1) {
            _url = "EmissionPoint/EmissionPointDetailReport";
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

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}
function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvEmissionPoints.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvEmissionPoints.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvEmissionPoints.GetSelectedRowCount() > 0 && gvEmissionPoints.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvEmissionPoints.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvEmissionPoints.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvEmissionPoints.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvEmissionPoints.cpFilteredRowCountWithoutPage + gvEmissionPoints.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvEmissionPoints.SelectRows();
}

function UnselectAllRows() {
    gvEmissionPoints.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});