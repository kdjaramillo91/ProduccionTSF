// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvItemTrademarkModels.AddNewRow();
}

function RemoveItems(s, e) {
    gvItemTrademarkModels.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvItemTrademarkModels.PerformCallback({ ids: selectedRows });
            gvItemTrademarkModels.UnselectRows();
        });
    });
}
function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemTrademarkModelUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItemTrademarkModel").val());
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
                    var url = "ItemTrademarkModel/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "ItemTrademarkModel/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemTrademarkModelsButton.SetEnabled(false);
                ItemTradeMarkModelArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "ItemTrademarkModel/ImportDatosCargaMasiva",
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
function onItemTrademarkModelImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemTrademarkModel").val(userData.id);
        ItemTradeMarkModelArchivoEditText.SetText(userData.filename);
       
    }
    ItemTradeMarkModelArchivoEditText.Validate();
}
function onItemTrademarkModelImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItemTrademarkModel").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemTrademarkModelsButton.SetEnabled(false);
    } else {
        ImportarItemTrademarkModelsButton.SetEnabled(true);
    }
}

function importFile(s, e) {
    
    showPage('ItemTrademarkModel/FormEditItems');
    //showPage("ItemType/FormEditItemsType");
}

function DownloadTemplateImportItems(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='ItemTrademarkModel/DownloadTemplateImportItems'></iframe>");
}

function RefreshGrid(s, e) {
    gvItemTrademarkModels.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvItemTrademarkModels.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvItemTrademarkModels.AddNewRow();
            keyToCopy = 0;
        }
    });
}

function Print(s, e) {
    gvItemTrademarkModels.GetSelectedFieldValues("id", function (values) {

        var _url = "ItemTrademarkModel/ItemTradeMarkModelReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemTrademarkModel/ItemTradeMarkModelDetailReport";
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
    showPage('ItemTrademarkModel/FormEditItems');
}



function ImportFile(data) {
    uploadFile("ItemTrademarkModel/ImportFileItemTradeMarkModel", data, function (result) {
        gvItemTrademarkModels.Refresh();
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

    var text = "Total de elementos seleccionados: <b>" + gvItemTrademarkModels.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItemTrademarkModels.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvItemTrademarkModels.GetSelectedRowCount() > 0 && gvItemTrademarkModels.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvItemTrademarkModels.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvItemTrademarkModels.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvItemTrademarkModels.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvItemTrademarkModels.cpFilteredRowCountWithoutPage + gvItemTrademarkModels.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItemTrademarkModels.SelectRows();
}

function UnselectAllRows() {
    gvItemTrademarkModels.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});