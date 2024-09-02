// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvCostCenters.AddNewRow();
}

function RemoveItems(s, e) {
    gvCostCenters.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvCostCenters.PerformCallback({ ids: selectedRows });
            gvCostCenters.UnselectRows();
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
    gvCostCenters.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvCostCenters.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvCostCenters.AddNewRow();
            keyToCopy = 0;
        }
    });
}

function Print(s, e) {
    gvCostCenters.GetSelectedFieldValues("id", function (values) {

        var _url = "CostCenter/CostCenterReport";

        var data = null;
        if (values.length === 1) {
            _url = "CostCenter/CostCenterDetailReport";
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

    var text = "Total de elementos seleccionados: <b>" + gvCostCenters.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCostCenters.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvCostCenters.GetSelectedRowCount() > 0 && gvCostCenters.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCostCenters.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvCostCenters.GetSelectedRowCount() > 0);
    btnPrint.SetEnabled(false);
    btnCopy.SetEnabled(false);
}

function GetSelectedFilteredRowCount() {
    return gvCostCenters.cpFilteredRowCountWithoutPage + gvCostCenters.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvCostCenters.SelectRows();
}

function UnselectAllRows() {
    gvCostCenters.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

// Funciones de validación
function OnCostCenterNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 250) {
            e.isValid = false;
            e.errorText = "Máximo 250 caracteres";
        }
    }
}

function OnCodeCostCenterValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 50) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        } else {
            $.ajax({
                url: "CostCenter/ValidateCodeCostCenter",
                type: "post",
                async: false,
                cache: false, data: {
                    id_costCenter: gvCostCenters.cpEditingRowKey,
                    code: e.value
                },
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    e.isValid = result.isValid;
                    e.errorText = result.errorText;
                },
                complete: function () {
                    hideLoading();
                }
            });
        }
    }
}

// Métodos guardar y cancelar registro
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvCostCenters.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvCostCenters !== null && gvCostCenters !== undefined) {
        gvCostCenters.CancelEdit();
    }
}


// Métodos de importación masiva
function importFile(s, e) {
    showPage('CostCenter/FormEditCostCenters');
}

function DownloadTemplateImportCostCenters(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='CostCenter/DownloadTemplateCostCenters'></iframe>");
}

function onCostCenterImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoCostCenter").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarCostCenterButton.SetEnabled(false);
    } else {
        ImportarCostCenterButton.SetEnabled(true);
    }
}

function onCostCenterImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoCostCenter").val(userData.id);
        CostCenterArchivoEditText.SetText(userData.filename);
    }
    CostCenterArchivoEditText.Validate();
}

function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("costCenterUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoCostCenter").val());
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
                    var url = "CostCenter/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "CostCenter/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarCostCenterButton.SetEnabled(false);
                CostCenterArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "CostCenter/ImportDatosCargaMasiva",
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

var ShowEditMessage = function (message) {
    if (message !== null && message.length > 0) {
        $("#messageAlert").html(message);

        $(".close").click(function () {
            $(".alert").alert('close');
            $("#messageAlert").empty();
        });
    }
}