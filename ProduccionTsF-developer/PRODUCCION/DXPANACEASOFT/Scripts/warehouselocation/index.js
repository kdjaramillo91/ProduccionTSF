// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvWarehouseLocations.AddNewRow();
}

function RemoveItems(s, e) {
    gvWarehouseLocations.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvWarehouseLocations.PerformCallback({ ids: selectedRows });
            gvWarehouseLocations.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvWarehouseLocations.Refresh();
}

var keyToCopy = null;

function CopyItems(s, e) {
    gvWarehouseLocations.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvWarehouseLocations.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {
    gvWarehouseLocations.GetSelectedFieldValues("id", function (values) {

        var _url = "WarehouseLocation/WarehouseLocationReport";

        var data = null;
        if (values.length === 1) {
            _url = "WarehouseLocation/WarehouseLocationDetailReport";
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
    if (ASPxClientEdit.ValidateEditorsInContainerById("warehouseLocationUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoWarehouseLocation").val());
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
                    var url = "WarehouseLocation/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "WarehouseLocation/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarWarehouseLocationButton.SetEnabled(false);
                WarehouseLocationArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "WarehouseLocation/ImportDatosCargaMasiva",
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

function onWarehouseLocationImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoWarehouseLocation").val(userData.id);
        WarehouseLocationArchivoEditText.SetText(userData.filename);
    }
    WarehouseLocationArchivoEditText.Validate();
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

function onWarehouseLocationImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoWarehouseLocation").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarWarehouseLocationButton.SetEnabled(false);
    } else {
        ImportarWarehouseLocationButton.SetEnabled(true);
    }
}

function importFile(s, e) {
    showPage('WarehouseLocation/FormEditWarehouseLocations');
}

function DownloadTemplateImportWarehouseLocations(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='WarehouseLocation/DownloadTemplateImportWarehouseLocations'></iframe>");
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

    var text = "Total de elementos seleccionados: <b>" + gvWarehouseLocations.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvWarehouseLocations.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvWarehouseLocations.GetSelectedRowCount() > 0 && gvWarehouseLocations.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvWarehouseLocations.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvWarehouseLocations.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvWarehouseLocations.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvWarehouseLocations.cpFilteredRowCountWithoutPage + gvWarehouseLocations.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvWarehouseLocations.SelectRows();
}

function UnselectAllRows() {
    gvWarehouseLocations.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});
