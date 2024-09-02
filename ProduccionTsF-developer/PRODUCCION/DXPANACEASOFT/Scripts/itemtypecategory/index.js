// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvItemTypeCategories.AddNewRow();
}

function RemoveItems(s, e) {
    gvItemTypeCategories.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            gvItemTypeCategories.PerformCallback({ ids: selectedRows });
            gvItemTypeCategories.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
 gvItemTypeCategories.Refresh();

}

var keyToCopy = null;
function CopyItems(s, e) {
    gvItemTypeCategories.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvItemTypeCategories.AddNewRow();
            keyToCopy = 0;

        }
    });
}

 function Print(s, e) {
    gvItemTypeCategories.GetSelectedFieldValues("id", function (values) {

        var _url = "ItemTypeCategory/ItemTypeCategoryReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemTypeCategory/ItemTypeCategoryDetailReport";
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
    showPage('ItemTypeCategory/FormEditItemTypeCategory');
}

function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemTypeCategoryUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItemTypeCategory").val());
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
                    var url = "ItemTypeCategory/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "ItemTypeCategory/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemTypeCategoryButton.SetEnabled(false);
                ItemTypeCategoryArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "ItemTypeCategory/ImportDatosCargaMasiva",
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

function onItemTypeCategoryImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemTypeCategory").val(userData.id);
        ItemTypeCategoryArchivoEditText.SetText(userData.filename);
    }
    ItemTypeCategoryArchivoEditText.Validate();
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

function onItemTypeCategoryImportFileValidate(s, e) {   
    var fileID = $("#GuidArchivoItemTypeCategory").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemTypeCategoryButton.SetEnabled(false);
    } else {
        ImportarItemTypeCategoryButton.SetEnabled(true);
    }
}

function DownloadTemplateImportItemTypeCategory(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='ItemTypeCategory/DownloadTemplateImportItemTypeCategoria'></iframe>");
}

// GRIDVIEW CLIENT SIDE EVENTES

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
// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvItemTypeCategories.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItemTypeCategories.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvItemTypeCategories.GetSelectedRowCount() > 0 && gvItemTypeCategories.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvItemTypeCategories.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvItemTypeCategories.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvItemTypeCategories.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvItemTypeCategories.cpFilteredRowCountWithoutPage + gvItemTypeCategories.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItemTypeCategories.SelectRows();
}

function UnselectAllRows() {
    gvItemTypeCategories.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});
