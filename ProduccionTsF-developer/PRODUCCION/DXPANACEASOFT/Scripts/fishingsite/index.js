// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvFishingSite.AddNewRow();
}

function RemoveItems(s, e) {
    gvFishingSite.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            gvFishingSite.PerformCallback({ ids: selectedRows });
            gvFishingSite.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvFishingSite.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvFishingSite.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvFishingSite.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function ImportFile(data) {
   /* uploadFile("ItemType/ImportFileItemType", data, function (result) {
        gvItemTypes.Refresh();
    });*/
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
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
         
function GridViewFishingSiteCustomCommandButton_Click(s, e) {

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

    var text = "Total de elementos seleccionados: <b>" + gvFishingSite.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvFishingSite.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvFishingSite.GetSelectedRowCount() > 0 && gvFishingSite.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvFishingSite.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvFishingSite.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvFishingSite.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvFishingSite.cpFilteredRowCountWithoutPage + gvFishingSite.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvFishingSite.SelectRows();
}

function UnselectAllRows() {
    gvFishingSite.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});