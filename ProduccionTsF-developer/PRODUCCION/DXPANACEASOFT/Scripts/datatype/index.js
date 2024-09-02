
// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvDataTypes.AddNewRow();
}


function RemoveItems(s, e) {
    gvDataTypes.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvDataTypes.PerformCallback({ ids: selectedRows });
            gvDataTypes.UnselectRows();
        });
    });
}
function RefreshGrid(s, e) {
    gvDataTypes.Refresh();
}

var keyToCopy = null;

function CopyItems(s, e) {
    gvDataTypes.GetSelectedFieldValues("id",
        function(values) {
            if (values.length === 1) {
                keyToCopy = values[0];
                gvDataTypes.AddNewRow();
                keyToCopy = 0;

            }
        });
}

function Print(s, e) {
    gvDataTypes.GetSelectedFieldValues("id", function (values) {

        var _url = "DataType/DataTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "DataType/DataTypeDetailReport";
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

function ImportFile(data) {
    uploadFile("DataType/ImportFileDataType", data, function (result) {
        gvDataTypes.Refresh();
    });
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}
// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvDataTypes.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvDataTypes.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvDataTypes.GetSelectedRowCount() > 0 && gvDataTypes.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvDataTypes.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvDataTypes.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvDataTypes.GetSelectedRowCount() === 1);}

function GetSelectedFilteredRowCount() {
    return gvDataTypes.cpFilteredRowCountWithoutPage + gvDataTypes.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvDataTypes.SelectRows();
}

function UnselectAllRows() {
    gvDataTypes.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});
