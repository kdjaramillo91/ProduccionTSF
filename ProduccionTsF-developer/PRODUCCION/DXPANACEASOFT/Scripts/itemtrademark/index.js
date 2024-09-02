// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvItemTrademarks.AddNewRow();
}

function RemoveItems(s, e) {
    gvItemTrademarks.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvItemTrademarks.PerformCallback({ ids: selectedRows });
            gvItemTrademarks.UnselectRows();
        });
    });
}

function createNewFromFileUploaded() {

    if (ASPxClientEdit.ValidateEditorsInContainerById("itemTrademarkUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItemTrademark").val());
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
                // ItemTradeMarkArchivoEditText
                var downloadArgs = {
                    guidResultado: result.guidResultado,
                    mensajeAlerta: alertMessage,
                };
                var url;
                if (result.HayErrores) {
                    var url = "ItemTrademark/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "ItemTrademark/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemTrademarksButton.SetEnabled(false);  
                ItemTrademarkArchivoEditText.SetText(null);
              
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");




            }




        }
        $.ajax({
            url: "ItemTrademark/ImportDatosCargaMasiva",
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


function onItemTrademarkImportFileUploadComplete(s, e) {
    
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemTrademark").val(userData.id);
        ItemTrademarkArchivoEditText.SetText(userData.filename);
    }
    ItemTrademarkArchivoEditText.Validate();
}

function onItemTrademarkImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItemTrademark").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemTrademarksButton.SetEnabled(false);
    } else {
        ImportarItemTrademarksButton.SetEnabled(true);
    }
}

function DownloadTemplateImportItems(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='ItemTrademark/DownloadTemplateImportItems'></iframe>");
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvItemTrademarks.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvItemTrademarks.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function RefreshGrid(s, e) {
    gvItemTrademarks.Refresh();
}



function Print(s, e) {
    gvItemTrademarks.GetSelectedFieldValues("id", function (values) {

        var _url = "ItemTrademark/ItemTradeMarkReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemTrademark/ItemTradeMarkDetailReport";
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
   
    //console.log('Funcionalidad no implementada');
    showPage('ItemTrademark/FormEditItems');
}



/*function ImportFile(data) {
    uploadFile("ItemTrademark/ImportFileItemTrademark", data, function (result) {
        gvItemTrademarks.Refresh();
    });
}*/



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

    var text = "Total de elementos seleccionados: <b>" + gvItemTrademarks.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItemTrademarks.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvItemTrademarks.GetSelectedRowCount() > 0 && gvItemTrademarks.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvItemTrademarks.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvItemTrademarks.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvItemTrademarks.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvItemTrademarks.cpFilteredRowCountWithoutPage + gvItemTrademarks.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItemTrademarks.SelectRows();
}

function UnselectAllRows() {
    gvItemTrademarks.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});