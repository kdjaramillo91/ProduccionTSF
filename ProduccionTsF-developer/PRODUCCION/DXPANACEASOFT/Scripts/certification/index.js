// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvCertifications.AddNewRow();
}

function RemoveItems(s, e) {
    gvCertifications.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvCertifications.PerformCallback({ ids: selectedRows });
            gvCertifications.UnselectRows();
        });
    });
}
function ImportFile(data) {
    uploadFile("Certification/ImportFileCertification", data, function (result) {
        gvCertifications.Refresh();
    });
}
function RefreshGrid(s, e) {
    gvCertifications.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvCertifications.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvCertifications.AddNewRow();
        }
    });
}

function Print(s, e) {
    gvCertifications.GetSelectedFieldValues("id", function (values) {

        var _url = "Certification/CertificationReport";

        var data = null;
        if (values.length === 1) {
            _url = "Certification/CertificationDetailReport";
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

function OnGridViewInit() {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvCertifications.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCertifications.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvCertifications.GetSelectedRowCount() > 0 && gvCertifications.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCertifications.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvCertifications.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvCertifications.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvCertifications.cpFilteredRowCountWithoutPage + gvCertifications.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvCertifications.SelectRows();
}

function UnselectAllRows() {
    gvCertifications.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

