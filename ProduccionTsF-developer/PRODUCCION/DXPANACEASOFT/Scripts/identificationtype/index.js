function AddNewItem(s, e) {
    gvIdentificationType.AddNewRow();
}

function RemoveItems(s, e) {
   // var c = confirm("¿Desea eliminar los tipos de identificación seleccionadas?");
  //  if (c === true) {
    showConfirmationDialog(function() {
        $.ajax({
            url: "IdentificationType/DeleteSelectedIdentificationTypes",
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
                gvIdentificationType.PerformCallback();
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
    gvIdentificationType.PerformCallback();
}

function Print(s, e) {

}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


//SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    s.ApplyFilter('[is_Active] = true');
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

    var text = "Total de elementos seleccionados: <b>" + gvIdentificationType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvIdentificationType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvIdentificationType.GetSelectedRowCount() > 0 && gvIdentificationType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvIdentificationType.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvIdentificationType.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvIdentificationType.cpFilteredRowCountWithoutPage + gvIdentificationType.GetSelectedKeysOnPage().length;
}
function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {
    $("form").on("click", "#lnkSelectAllRows", function () {
        gvIdentificationType.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
        gvIdentificationType.UnselectRows();
    });
});