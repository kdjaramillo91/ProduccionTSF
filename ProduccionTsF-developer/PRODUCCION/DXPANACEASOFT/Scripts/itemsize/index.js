// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvItemSizes.AddNewRow();
}

function RemoveItems(s, e) {
    gvItemSizes.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvItemSizes.PerformCallback({ ids: selectedRows });
            gvItemSizes.UnselectRows();
        });
    });
}

function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemSizeUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItemSize").val());
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
                    var url = "ItemSize/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "ItemSize/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemSizesButton.SetEnabled(false);
                ItemSizeArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "ItemSize/ImportDatosCargaMasiva",
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

function onItemSizeImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemSize").val(userData.id);
        ItemSizeArchivoEditText.SetText(userData.filename);
    }
    ItemSizeArchivoEditText.Validate();
}

function onItemSizeImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItemSize").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemSizesButton.SetEnabled(false);
    } else {
        ImportarItemSizesButton.SetEnabled(true);
    }
}

function ImportFile(data) {

    showPage('ItemSize/FormEditItems');
   // uploadFile("ItemSize/ImportFileItemSize", data, function (result) {
     //   gvItemSizes.Refresh();
   // });
}

function DownloadTemplateImportItems(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='ItemSize/DownloadTemplateImportItems'></iframe>");
}

function RefreshGrid(s, e) {
    gvItemSizes.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvItemSizes.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvItemSizes.AddNewRow();
        }
    });
}
function Print(s, e) {
    gvItemSizes.GetSelectedFieldValues("id", function (values) {

        var _url = "ItemSize/ItemSizeReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemSize/ItemSizeDetailReport";
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
    showPage('ItemSize/FormEditItems');
}



// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    s.ApplyFilter('[isActive] = true');
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);

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

    var text = "Total de elementos seleccionados: <b>" + gvItemSizes.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItemSizes.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvItemSizes.GetSelectedRowCount() > 0 && gvItemSizes.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvItemSizes.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvItemSizes.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvItemSizes.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvItemSizes.cpFilteredRowCountWithoutPage + gvItemSizes.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItemSizes.SelectRows();
}

function UnselectAllRows() {
    gvItemSizes.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});