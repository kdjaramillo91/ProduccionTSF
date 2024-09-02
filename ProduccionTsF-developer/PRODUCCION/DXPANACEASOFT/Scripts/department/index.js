// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvDepartments.AddNewRow();
}


function RemoveItems(s, e) {
    gvDepartments.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvDepartments.PerformCallback({ ids: selectedRows });
            gvDepartments.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvDepartments.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvDepartments.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function RefreshGrid(s, e) {
    gvDepartments.Refresh();
}

function ImportFile(data) {
    uploadFile("Department/ImportFileDepartment", data, function (result) {
        gvDepartments.Refresh();
    });
}

function Print(s, e) {
    gvDepartments.GetSelectedFieldValues("id", function (values) {

        var _url = "Department/DepartmentReport";

        var data = null;
        if (values.length === 1) {
            _url = "Department/DepartmentDetailReport";
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

    s.ApplyFilter('[isActive] = true');
}


function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();

}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}
function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
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

    var text = "Total de elementos seleccionados: <b>" + gvDepartments.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvDepartments.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvDepartments.GetSelectedRowCount() > 0 && gvDepartments.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvDepartments.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvDepartments.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvDepartments.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvDepartments.cpFilteredRowCountWithoutPage + gvDepartments.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvDepartments.SelectRows();
}

function UnselectAllRows() {
    gvDepartments.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});
