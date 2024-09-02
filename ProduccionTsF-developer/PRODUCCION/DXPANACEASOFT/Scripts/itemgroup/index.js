// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvItemGroups.AddNewRow();
}

function RemoveItems(s, e) {
    gvItemGroups.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvItemGroups.PerformCallback({ ids: selectedRows });
            gvItemGroups.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvItemGroups.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvItemGroups.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {
    gvItemGroups.GetSelectedFieldValues("id", function (values) {

        var _url = "ItemGroup/ItemGroupReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemGroup/ItemGroupDetailReport";
            data = { id: values[0] };
        }
        $.ajax({url: _url,
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



function ImportFile(data) {
    uploadFile("ItemGroup/ImportFileItemGroup", data, function (result) {
        gvItemGroups.Refresh();
    });
}


// GRIDVIEW CLIENT SIDE EVENTES
function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function RefreshGrid(s, e) {
    gvItemGroups.Refresh();
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
}



// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvItemGroups.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItemGroups.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvItemGroups.GetSelectedRowCount() > 0 && gvItemGroups.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvItemGroups.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvItemGroups.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvItemGroups.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvItemGroups.cpFilteredRowCountWithoutPage + gvItemGroups.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItemGroups.SelectRows();
}

function UnselectAllRows() {
    gvItemGroups.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});
