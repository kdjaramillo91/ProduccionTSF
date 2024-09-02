// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvWarehousesTypes.AddNewRow();
}

function RemoveItems(s, e) {
    gvWarehousesTypes.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvWarehousesTypes.PerformCallback({ ids: selectedRows });
            gvWarehousesTypes.UnselectRows();
        });
    });
}
function ImportFile(data) {
    uploadFile("WarehouseType/ImportFileWarehouseType", data, function (result) {
        gvWarehousesTypes.Refresh();
    });
}
function RefreshGrid(s, e) {
    gvWarehousesTypes.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvWarehousesTypes.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvWarehousesTypes.AddNewRow();
        }
    });
}

function Print(s, e) {
    gvWarehousesTypes.GetSelectedFieldValues("id", function (values) {

        var _url = "WarehouseType/WarehouseTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "WarehouseType/WarehouseTypeDetailReport";
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

function OnGridViewInit() {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
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
    let _poundsType = ASPxClientControl.GetControlCollection().GetByName("poundsType");
    if (!(_poundsType == null || _poundsType == undefined)) {
        e.customArgs["poundsType"] = _poundsType.GetValue();
    }
    let _reasonCosts = ASPxClientControl.GetControlCollection().GetByName("reasonCosts");
    if (!(_reasonCosts == null || _reasonCosts == undefined)) {
        e.customArgs["reasonCosts"] = _reasonCosts.GetValue();
    }
    let _idProcessedPoundsSimpleFormula = ASPxClientControl.GetControlCollection().GetByName("idProcessedPoundsSimpleFormula");
    if (!(_idProcessedPoundsSimpleFormula == null || _idProcessedPoundsSimpleFormula == undefined)) {
        e.customArgs["idProcessedPoundsSimpleFormula"] = _idProcessedPoundsSimpleFormula.GetValue();
    }
    let _idFinishedPoundsSimpleFormula = ASPxClientControl.GetControlCollection().GetByName("idFinishedPoundsSimpleFormula");
    if (!(_idFinishedPoundsSimpleFormula == null || _idFinishedPoundsSimpleFormula == undefined)) {
        e.customArgs["idFinishedPoundsSimpleFormula"] = _idFinishedPoundsSimpleFormula.GetValue();
    }
    e.customArgs["keyToCopy"] = keyToCopy;
}

// SELECTION
function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvWarehousesTypes.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvWarehousesTypes.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvWarehousesTypes.GetSelectedRowCount() > 0 && gvWarehousesTypes.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvWarehousesTypes.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvWarehousesTypes.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvWarehousesTypes.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvWarehousesTypes.cpFilteredRowCountWithoutPage + gvWarehousesTypes.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvWarehousesTypes.SelectRows();
}

function UnselectAllRows() {
    gvWarehousesTypes.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

