// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvItemGroupCategories.AddNewRow();
}

function RemoveItems(s, e) {
    gvItemGroupCategories.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvItemGroupCategories.PerformCallback({ ids: selectedRows });
            gvItemGroupCategories.UnselectRows();
        });
    });
}
var keyToCopy = null;
function CopyItems(s, e) {
    gvItemGroupCategories.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvItemGroupCategories.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function RefreshGrid(s, e) {
    gvItemGroupCategories.Refresh();
}


function Print(s, e) {
    gvItemGroupCategories.GetSelectedFieldValues("id", function (values) {

        var _url = "ItemGroupCategory/ItemGroupCategoryReport";

        var data = null;
        if (values.length === 1) {
            _url = "ItemGroupCategory/ItemGroupCategoryDetailReport";
            data = { id: values[0] };
        }
        $.ajax({
            url: _url,
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {console.log(error);
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

function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemGroupCategoryUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItemGroupCategory").val());
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
                    var url = "ItemGroupCategory/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "ItemGroupCategory/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemGroupCategorysButton.SetEnabled(false);
                ItemGroupCategoryArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "ItemGroupCategory/ImportDatosCargaMasiva",
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

/*function ImportFile(data) {
    uploadFile("ItemGroupCategory/ImportFileItemGroupCategory", data, function (result) {
        gvItemGroupCategories.Refresh();
    });
}*/

function onItemGroupCategoryImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemGroupCategory").val(userData.id);
        ItemGroupCategoryArchivoEditText.SetText(userData.filename);
    }
    ItemGroupCategoryArchivoEditText.Validate();
}

function onItemGroupCategoryImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItemGroupCategory").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemGroupCategorysButton.SetEnabled(false);
    } else {
        ImportarItemGroupCategorysButton.SetEnabled(true);
    }
}
function DownloadTemplateImportItemGroupCategory(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='ItemGroupCategory/DownloadTemplateImportItemGroupCategoria'></iframe>");
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

    var text = "Total de elementos seleccionados: <b>" + gvItemGroupCategories.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvItemGroupCategories.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvItemGroupCategories.GetSelectedRowCount() > 0 && gvItemGroupCategories.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvItemGroupCategories.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvItemGroupCategories.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvItemGroupCategories.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvItemGroupCategories.cpFilteredRowCountWithoutPage + gvItemGroupCategories.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvItemGroupCategories.SelectRows();
}

function UnselectAllRows() {
    gvItemGroupCategories.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});