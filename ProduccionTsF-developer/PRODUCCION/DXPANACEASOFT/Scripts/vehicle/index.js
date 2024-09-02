// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvVehicle.AddNewRow();
}

function RemoveItems(s, e) {
    gvVehicle.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvVehicle.PerformCallback({ ids: selectedRows });
            gvVehicle.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvVehicle.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvVehicle.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvVehicle.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("Vehicle/ImportFileVehicle", data, function (result) {
        gvVehicle.Refresh();
    });
}

function Print(s, e) {
    gvVehicle.GetSelectedFieldValues("id", function (values) {

        var _url = "Vehicle/VehicleReport";

        var data = null;
        if (values.length === 1) {
            _url = "Vehicle/VehicleDetailReport";
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

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

  // DEFUALT FILTER
    s.ApplyFilter('[isActive] = true');
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

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

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvVehicle.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvVehicle.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvVehicle.GetSelectedRowCount() > 0 && gvVehicle.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvVehicle.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvVehicle.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvVehicle.GetSelectedRowCount() === 1);
}
function GetSelectedFilteredRowCount() {
    return gvVehicle.cpFilteredRowCountWithoutPage + gvVehicle.GetSelectedKeysOnPage().length;
}
function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvVehicle.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvVehicle.UnselectRows();
    });
});
function SelectAllRows() {
    gvVehicle.SelectRows();
}

function UnselectAllRows() {
    gvVehicle.UnselectRows();
}

