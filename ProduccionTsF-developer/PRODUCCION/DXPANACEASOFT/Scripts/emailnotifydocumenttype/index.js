//#region Accion Botones
function AddNewItem(s, e) {
    gvEmailNotifyDocumentType.AddNewRow();
}

function RemoveItems(s, e) {
    gvEmailNotifyDocumentType.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvEmailNotifyDocumentType.PerformCallback({ ids: selectedRows });
            gvEmailNotifyDocumentType.UnselectRows();
        });
    });
}
var keyToCopy = null;
function CopyItems(s, e) {
    gvEmailNotifyDocumentType.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvEmailNotifyDocumentType.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function RefreshGrid(s, e) {
    gvEmailNotifyDocumentType.Refresh();
}


function Print(s, e) {
    gvEmailNotifyDocumentType.GetSelectedFieldValues("id", function (values) {

        var _url = "EmailNotify/EmailNotifyDocumentTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "EmailNotify/EmailNotifyDocumentTypeDetailReport";
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
    showPage('ItemGroupCategory/FormEditItemGroupCategory');
}
//#endregion

//#region Acciones Grid
function OnGridViewInit(s, e) {
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
//#endregion