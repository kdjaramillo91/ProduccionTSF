
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage("ItemPresentation/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterPresentation").serialize() + "&id_itemPackingMinimum=" + id_itemPackingMinimum.GetValue() + "&id_itemPackingMaximum=" + id_itemPackingMaximum.GetValue() + "&id_metricUnit=" + id_metricUnit.GetValue();
      if (data != null) {
        $.ajax({
            url: "ItemPresentation/PresentationResults",
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
                $("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
    event.preventDefault();
}

function btnClear_click() {
    id_itemPackingMaximum.SetSelectedItem(null);
    id_itemPackingMinimum.SetSelectedItem(null);
    id_metricUnit.SetSelectedItem(null);
 
}

function AddNewCalendar() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("ItemPresentation/FormEditPresentation", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvPresentation.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
         
        $.ajax({
            url: url,
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvPresentation.PerformCallback();
                gvPresentation.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewCalendar(s, e);
}
function CopyDocument(s, e) {
   
}
function ApproveDocuments(s, e) {

}
function AutorizeDocuments(s, e) {
 
}
function ProtectDocuments(s, e) {

}
function CancelDocuments(s, e) {

}
function RevertDocuments(s, e) {

}
function ShowHistory(s, e) {

}
function Print(s, e) {
    
}

function importFile(s, e) {
    showPage('itemPresentation/FormEditItemPresentations');
}

function DownloadTemplateImportItemPresentations(s, e) {
    $('#download-area').empty();
    $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='itemPresentation/DownloadTemplateImportItemPresentations'></iframe>");
}

function onItemPresentationImportFileValidate(s, e) {
    var fileID = $("#GuidArchivoItemPresentation").val();
    if (fileID === null || fileID.length === 0) {
        e.isValid = false;
        e.errorText = "Archivo de datos es obligatorio.";
        ImportarItemPresentationButton.SetEnabled(false);
    } else {
        ImportarItemPresentationButton.SetEnabled(true);
    }
}

function onItemPresentationImportFileUploadComplete(s, e) {
    if (e.isValid) {
        var userData = JSON.parse(e.callbackData);
        $("#GuidArchivoItemPresentation").val(userData.id);
        ItemPresentationArchivoEditText.SetText(userData.filename);
    }
    ItemPresentationArchivoEditText.Validate();
}

function createNewFromFileUploaded() {
    if (ASPxClientEdit.ValidateEditorsInContainerById("itemPresentationUploadFileForm", "", true)) {
        const obj = JSON.parse($("#GuidArchivoItemPresentation").val());
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
                    var url = "ItemPresentation/DownloadDocumentosFallidosImportacion?" + $.param(downloadArgs);
                }
                else {
                    var url = "ItemPresentation/DownloadDocumentosImportadosImportacion?" + $.param(downloadArgs);
                }
                ImportarItemPresentationButton.SetEnabled(false);
                ItemPresentationArchivoEditText.SetText(null);
                $('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
            }
        }

        $.ajax({
            url: "ItemPresentation/ImportDatosCargaMasiva",
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

function GridViewlgvPresentationCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvPresentation.GetRowKey(e.visibleIndex)
        };
        showPage("ItemPresentation/FormEditPresentation", data);
    }
}



function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

var selectedRows = [];

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPresentation.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPresentation.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvPresentation.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPresentation.GetSelectedRowCount() > 0 && gvPresentation.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPresentation.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvPresentation.GetSelectedRowCount() == 1);

//    btnCopy.SetEnabled(false);
//    btnApprove.SetEnabled(false);
//    btnAutorize.SetEnabled(false);
//    btnProtect.SetEnabled(false);
//    btnCancel.SetEnabled(false);
//    btnRevert.SetEnabled(false);
//    btnHistory.SetEnabled(false);
//    btnPrint.SetEnabled(false);
}

function GetSelectedFilteredRowCount() {
    return gvPresentation.cpFilteredRowCountWithoutPage + gvPresentation.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvPresentation.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvPresentation.SelectRows();
}

function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
}
$(function () {
    init();
});