// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvItemColors.AddNewRow();
}

function RemoveItems(s, e) {
    gvItemColors.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvItemColors.PerformCallback({ ids: selectedRows });
            gvItemColors.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvItemColors.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvItemColors.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvItemColors.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function Print(s, e) {
    gvItemColors.GetSelectedFieldValues("id", function (values) {

        var _url = "ItemColor/ItemColorReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemColor/ItemColorDetailReport";
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


function ImportFile(data) {
    uploadFile("ItemColor/ImportFileItemColor", data, function (result) {
        gvCountry.Refresh();
    });
}


// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    s.ApplyFilter('[isActive] = true');
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();

}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
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

    var text = "Total de elementos seleccionados: <b>" + gvItemColors.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItemColors.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvItemColors.GetSelectedRowCount() > 0 && gvItemColors.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvItemColors.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvItemColors.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvItemColors.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvItemColors.cpFilteredRowCountWithoutPage + gvItemColors.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItemColors.SelectRows();
}

function UnselectAllRows() {
    gvItemColors.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});


