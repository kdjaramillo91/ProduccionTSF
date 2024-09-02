// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvRemissionGuideTravelType.AddNewRow();
}

function RemoveItems(s, e) {
    gvRemissionGuideTravelType.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            gvRemissionGuideTravelType.PerformCallback({ ids: selectedRows });
            gvRemissionGuideTravelType.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvRemissionGuideTravelType.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvRemissionGuideTravelType.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvRemissionGuideTravelType.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function ImportFile(data) {
    /* uploadFile("ItemType/ImportFileItemType", data, function (result) {
         gvItemTypes.Refresh();
     });*/
}

function Print(s, e) {
    /*
    gvItemTypes.GetSelectedFieldValues("id", function (values) {

    var _url = "ItemType/ItemTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemType/ItemTypeDetailReport";
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
    */
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

function GridViewRemissionGuideTravelTypeCustomCommandButton_Click(s, e) {

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

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideTravelType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideTravelType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideTravelType.GetSelectedRowCount() > 0 && gvRemissionGuideTravelType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideTravelType.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvRemissionGuideTravelType.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvRemissionGuideTravelType.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvRemissionGuideTravelType.cpFilteredRowCountWithoutPage + gvRemissionGuideTravelType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvRemissionGuideTravelType.SelectRows();
}

function UnselectAllRows() {
    gvRemissionGuideTravelType.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});