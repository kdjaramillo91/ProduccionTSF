// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvItemTypes.AddNewRow();
}

function RemoveItems(s, e) {
    gvItemTypes.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            gvItemTypes.PerformCallback({ ids: selectedRows });
            gvItemTypes.UnselectRows();
        });
    });
}

function createNewFromFileUploaded(){
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemTypeUploadFileForm", "", true)) {        
        const obj = JSON.parse($("#GuidArchivoItemType").val());
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
                    var url = "ItemType/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "ItemType/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemTypesButton.SetEnabled(false);
                ItemTypeArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "ItemType/ImportDatosCargaMasiva",
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


function onItemTypeImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemType").val(userData.id);
        ItemTypeArchivoEditText.SetText(userData.filename);
    }
    ItemTypeArchivoEditText.Validate();
}

function onItemTypeImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItemType").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemTypesButton.SetEnabled(false);
    } else {
        ImportarItemTypesButton.SetEnabled(true);
    }
}

function importFile(s, e) {
    showPage('ItemType/FormEditItems');
    //showPage("ItemType/FormEditItemsType");
}

function DownloadTemplateImportItems(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='ItemType/DownloadTemplateImportItems'></iframe>");
}


function RefreshGrid(s, e) {
    gvItemTypes.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvItemTypes.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvItemTypes.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("ItemType/ImportFileItemType", data, function (result) {
        gvItemTypes.Refresh();
    });
}

function Print(s, e) {
    gvItemTypes.GetSelectedFieldValues("id", function (values) {

        var _url = "ItemType/ItemTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemType/ItemTypeDetailReport";
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
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
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

    var text = "Total de elementos seleccionados: <b>" + gvItemTypes.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItemTypes.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvItemTypes.GetSelectedRowCount() > 0 && gvItemTypes.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvItemTypes.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvItemTypes.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvItemTypes.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvItemTypes.cpFilteredRowCountWithoutPage + gvItemTypes.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItemTypes.SelectRows();
}

function UnselectAllRows() {
    gvItemTypes.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});