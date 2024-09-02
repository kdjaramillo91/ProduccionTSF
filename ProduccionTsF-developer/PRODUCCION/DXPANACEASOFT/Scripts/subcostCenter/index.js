// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvSubcostCenters.AddNewRow();
}

function RemoveItems(s, e) {
    gvSubcostCenters.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvSubcostCenters.PerformCallback({ ids: selectedRows });
            gvSubcostCenters.UnselectRows();
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
    gvSubcostCenters.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvSubcostCenters.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvSubcostCenters.AddNewRow();
            keyToCopy = 0;
        }
    });
}

function Print(s, e) {
    gvSubcostCenters.GetSelectedFieldValues("id", function (values) {

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

    var text = "Total de elementos seleccionados: <b>" + gvSubcostCenters.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSubcostCenters.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSubcostCenters.GetSelectedRowCount() > 0 && gvSubcostCenters.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSubcostCenters.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvSubcostCenters.GetSelectedRowCount() > 0);
    btnPrint.SetEnabled(false);
    btnCopy.SetEnabled(false);
}

function GetSelectedFilteredRowCount() {
    return gvSubcostCenters.cpFilteredRowCountWithoutPage + gvSubcostCenters.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvSubcostCenters.SelectRows();
}

function UnselectAllRows() {
    gvSubcostCenters.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});


// Métodos validaciones
function OnSubcostCenterNameValidation(s, e) {
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



function OnCostCenterValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCodeSubcostCenterValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 50) {
            e.isValid = false;
            e.errorText = "Máximo 50 caracteres";
        } else {
            $.ajax({
                url: "SubcostCenter/ValidateCodeSubcostCenter",
                type: "post",
                async: false,
                cache: false, data: {
                    id_subCostCenter: gvSubcostCenters.cpEditingRowKey,
                    code: e.value
                },
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    e.isValid = result.isValid;
                    e.errorText = result.errorText;
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }
    }
}

// Métodos de edición
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvSubcostCenters.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvSubcostCenters !== null && gvSubcostCenters !== undefined) {
        gvSubcostCenters.CancelEdit();
    }
}

// Métodos de importación masiva
function importFile(s, e) {
    showPage('SubcostCenter/FormEditSubcostCenters');
}

function DownloadTemplateImportSubcostCenters(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='SubcostCenter/DownloadTemplateSubcostCenters'></iframe>");
}

function onSubcostCenterImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoSubcostCenter").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarSubcostCenterButton.SetEnabled(false);
    } else {
        ImportarSubcostCenterButton.SetEnabled(true);
    }
}

function onSubcostCenterImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoSubcostCenter").val(userData.id);
        SubcostCenterArchivoEditText.SetText(userData.filename);
    }
    SubcostCenterArchivoEditText.Validate();
}

function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("subcostCenterUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoSubcostCenter").val());
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
                    var url = "SubcostCenter/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "SubcostCenter/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarSubcostCenterButton.SetEnabled(false);
                SubcostCenterArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "SubcostCenter/ImportDatosCargaMasiva",
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
