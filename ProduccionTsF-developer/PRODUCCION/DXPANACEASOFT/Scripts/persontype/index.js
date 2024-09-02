function AddNewItem(s, e) {
    gvPersonTypes.AddNewRow();
}

function RemoveItems(s, e) {
   // var c = confirm("¿Desea eliminar los Tipos de Personas seleccionados?");
 //   if (c === true) {
    showConfirmationDialog(function() {


        $.ajax({
            url: "Person/DeleteSelectedPersonTypes",
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
                gvPersonTypes.PerformCallback();
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
    gvPersonTypes.PerformCallback();
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

    var text = "Total de elementos seleccionados: <b>" + gvPersonTypes.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPersonTypes.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPersonTypes.GetSelectedRowCount() > 0 && gvPersonTypes.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPersonTypes.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvPersonTypes.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvPersonTypes.cpFilteredRowCountWithoutPage + gvPersonTypes.GetSelectedKeysOnPage().length;
}
function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvPersonTypes.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvPersonTypes.UnselectRows();
    });
});