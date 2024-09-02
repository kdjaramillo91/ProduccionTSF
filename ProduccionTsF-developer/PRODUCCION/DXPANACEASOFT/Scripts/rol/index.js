function AddNewItem(s, e) {
    gvRol.AddNewRow();
}

function RemoveItems(s, e) {
  //  var c = confirm("¿Desea eliminar los Roles seleccionados?");
   // if (c === true) {
    showConfirmationDialog(function() {
        $.ajax({
            url: "Rol/DeleteSelectedRoles",
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function(error) {
                console.log(error);
            },
            beforeSend: function() {
                showLoading();
            },
            success: function(result) {
                //$("#maincontent").html(result);
            },
            complete: function() {
                gvRol.PerformCallback();
                hideLoading();
            }
        });
    });


}
function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
        });
    }
}

function RefreshGrid(s, e) {
    gvRol.PerformCallback();
}

function Print(s, e) {

}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

//SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

var selectedRows = [];

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

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRol.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRol.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvRol.GetSelectedRowCount() > 0 && gvRol.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRol.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvRol.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvRol.cpFilteredRowCountWithoutPage + gvRol.GetSelectedKeysOnPage().length;
}
function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvRol.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvRol.UnselectRows();
    });
});