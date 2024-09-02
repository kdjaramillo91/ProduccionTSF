// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvFishingZone.AddNewRow();
}

function RemoveItems(s, e) {
    gvFishingZone.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            gvFishingZone.PerformCallback({ ids: selectedRows });
            gvFishingZone.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvFishingZone.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvFishingZone.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvFishingZone.AddNewRow();
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
         
function GridViewFishingZoneCustomCommandButton_Click(s, e) {

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

    var text = "Total de elementos seleccionados: <b>" + gvFishingZone.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvFishingZone.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvFishingZone.GetSelectedRowCount() > 0 && gvFishingZone.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvFishingZone.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvFishingZone.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvFishingZone.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvFishingZone.cpFilteredRowCountWithoutPage + gvFishingZone.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvFishingZone.SelectRows();
}

function UnselectAllRows() {
    gvFishingZone.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});