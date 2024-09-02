// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvCatalogues.AddNewRow();
}

function RemoveItems(s, e) {
    gvCatalogues.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvCatalogues.PerformCallback({ ids: selectedRows });
            gvCatalogues.UnselectRows();
        });
    });
}

var ShowEditMessage = function (message) {
    if (message !== null && message.length > 0) {
        $("#messageAlert").html(message);

        $(".close").click(function () {
            $(".alert").alert('close');
            $("#messageAlert").empty();
        });
    }
}

function RefreshGrid(s, e) {
    gvCatalogues.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvCatalogues.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvCatalogues.AddNewRow();
            keyToCopy = 0;
        }
    });
}

function Print(s, e) {
    gvCatalogues.GetSelectedFieldValues("id", function (values) {

        var _url = "Catalogue/CatalogueReport";

        var data = null;
        if (values.length === 1) {
            _url = "Catalogue/CatalogueDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvCatalogues.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCatalogues.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvCatalogues.GetSelectedRowCount() > 0 && gvCatalogues.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCatalogues.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvCatalogues.GetSelectedRowCount() > 0);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
    btnCopy.SetEnabled(false);
}

function GetSelectedFilteredRowCount() {
    return gvCatalogues.cpFilteredRowCountWithoutPage + gvCatalogues.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvCatalogues.SelectRows();
}

function UnselectAllRows() {
    gvCatalogues.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});