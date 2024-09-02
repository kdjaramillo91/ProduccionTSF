// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvWarehouses.AddNewRow();
}

function RemoveItems(s, e) {
    gvWarehouses.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvWarehouses.PerformCallback({ ids: selectedRows });
            gvWarehouses.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvWarehouses.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvWarehouses.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvWarehouses.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {
    gvWarehouses.GetSelectedFieldValues("id", function (values) {

        var _url = "Warehouse/WarehouseReport";

        var data = null;
        if (values.length === 1) {
            _url = "Warehouse/WarehouseDetailReport";
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

function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("warehouseUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoWarehouse").val());
        let alertMessage = "";
        var userData = {
            guidArchivoDatos: obj.guid,
        }

        _successCallback = function (result) {
            if (result.isValid) {
                $('#download-area').empty();

                if (result.HayErrores) {
                    alertMessage = "Existen errores en los datos a importar.";
                }
                else {
                    alertMessage = "Importación realizada exitosamente";
                }

                var downloadArgs = {
                    guidResultado: result.guidResultado,
                    mensajeAlerta: alertMessage,
                };
                var url;
                if (result.HayErrores) {
                    var url = "Warehouse/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "Warehouse/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarWarehouseButton.SetEnabled(false);
                WarehouseArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "Warehouse/ImportDatosCargaMasiva",
            type: "post",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(userData),
            async: true,
            cache: false,
            error: function (error) {
                console.error(error);
                showErrorMessage(error.responseText);
                hideLoading();
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.isValid) {
                    if (result.message) {
                        if (result.HayErrores) {
                            showWarningMessage(result.message);
                        } else {
                            showSuccessMessage(result.message);
                        }
                    }
                    if (_successCallback) {
                        _successCallback(result);
                    }
                }

                if (typeof result.keepLoading === "undefined" || !result.keepLoading) {
                    hideLoading();
                }
            },
            complete: function () {
            }
        });
	}
}

function onWarehouseImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoWarehouse").val(userData.id);
        WarehouseArchivoEditText.SetText(userData.filename);
    }
    WarehouseArchivoEditText.Validate();
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

function onWarehouseImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoWarehouse").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarWarehouseButton.SetEnabled(false);
    } else {
        ImportarWarehouseButton.SetEnabled(true);
    }
}

function importFile(s, e) {
    showPage('Warehouse/FormEditWarehouses');
}

function DownloadTemplateImportWarehouses(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='Warehouse/DownloadTemplateImportWarehouses'></iframe>");
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit() {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    // 
    e.customArgs["keyToCopy"] = keyToCopy;
    
    let isNewRowEditing = s.IsNewRowEditing();
    let isEditing = s.IsEditing();

    e.customArgs["isNewRowEditing"] = isNewRowEditing;
    e.customArgs["isEditing"] = isEditing;
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

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvWarehouses.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvWarehouses.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvWarehouses.GetSelectedRowCount() > 0 && gvWarehouses.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvWarehouses.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvWarehouses.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvWarehouses.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvWarehouses.cpFilteredRowCountWithoutPage + gvWarehouses.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvWarehouses.SelectRows();
}

function UnselectAllRows() {
    gvWarehouses.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});