
//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabdetail", true);
    if (!valid) {
        UpdateTabImage({ isValid: false }, "tabdetail");
    }

    

    if (typeof (gvLiquidationFreightRiverDetail) != "undefined") {
        var _enEdicion = gvLiquidationFreightRiverDetail.batchEditApi.HasChanges();
        if (_enEdicion) {
            UpdateTabImage({ isValid: false }, "tabdetail");
            valid &= false;
        }
    }
     
    if (valid) {
          
         var id = $("#id_LiquidationFreightRiver").val();
         var id_providertransport = $("#id_providertransport").val();
         
      
         var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#FormEditLiquidationFreightRiver").serialize() + "&id_providertransport" + id_providertransport;
        var url = (id === "0") ? "LiquidationFreightRiver/LiquidationFreightRiverPartialAddNew"
                               : "LiquidationFreightRiver/LiquidationFreightRiverPartialUpdate";

        showForm(url, data);
    }
}

function ButtonUpdate_Click(s, e) {

    Update(false);
  
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    showPage("LiquidationFreightRiver/Index", null);
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0
    };

    showPage("LiquidationFreightRiver/RemissionGuideRiverResults", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    //showPage("LiquidationFreight/RemissionGuideCopy", { id: $("#id_remissionGuide").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar la Liquidación de Flete?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_LiquidationFreightRiver").val()
        };
        showForm("LiquidationFreightRiver/Autorize", data);
    }, "¿Desea autorizar la Liquidación de Flete?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_LiquidationFreightRiver").val()
        };
        showForm("LiquidationFreightRiver/Protect", data);
    }, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_LiquidationFreightRiver").val()
        };
        showForm("LiquidationFreightRiver/Cancel", data);
    }, "¿Desea anular la Liquidación de Flete?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_LiquidationFreightRiver").val()
        };
        showForm("LiquidationFreightRiver/Revert", data);
    }, "¿Desea reversar la Liquidación de Flete?");
}

function ShowDocumentHistory(s, e) {

}

//LIQUIDATION FREIGHT DOCUMENTS
// DETAILS ACTIONS ATTACHED DOCUMENTS
function AddNewAttachedDocumentDetail(s, e) {
    gvLiquidationRiverDocumentsAttachedDocuments.AddNewRow();
}

function RemoveAttachedDocumentDetail(s, e) {
}

function RefreshAttachedDocumentDetail(s, e) {
    gvLiquidationRiverDocumentsAttachedDocuments.UnselectRows();
    gvLiquidationRiverDocumentsAttachedDocuments.PerformCallback();
}

function AttachedUploadComplete(s, e) {
    var userData = JSON.parse(e.callbackData);
    $("#guid").val(userData.guid);
    $("#url").val(userData.url);
    TSattachmentName.SetText(userData.filename);
}

var attachmentNameIniAux = null;

function AttachedName_OnInit(s, e) {
    attachmentNameIniAux = s.GetText();
}

function AttachedNameValidate(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Archivo Obligatorio";
    } else {
        var guid = $("#guidAttachment").val();
        if (guid === null || guid.length === 0) {
            e.isValid = false;
            e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
        } else {
            var url = $("#urlAttachment").val();
            if (guid === null || guid.length === 0) {
                e.isValid = false;
                e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
            } else {
                var data = {
                    attachmentNameNew: e.value
                };
                if (data.attachmentNameNew != attachmentNameIniAux) {
                    $.ajax({
                        url: "LiquidationFreightRiver/ItsRepeatedAttachmentDetail",//ItsRepeatedLiquidation",
                        type: "post",
                        data: data,
                        async: false,
                        cache: false,
                        error: function (error) {
                            console.log(error);
                        },
                        beforeSend: function () {
                            //showLoading();
                        },
                        success: function (result) {
                            if (result !== null) {
                                if (result.itsRepeated == 1) {
                                    e.isValid = false;
                                    e.errorText = result.Error;
                                }
                            }
                        },
                        complete: function () {
                            //hideLoading();
                        }
                    });
                }
            }
        }
    }

}

function gvLiquidationDocumentsAttachedDocumentsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnUpdate") {
        console.log("e.buttonID: " + e.buttonID);
        var valid = true;
        var validAttachmentFormUpLoad = ASPxClientEdit.ValidateEditorsInContainerById("attachment-form-upLoad", null, true);
        console.log("validAttachmentFormUpLoad: " + validAttachmentFormUpLoad);
        if (validAttachmentFormUpLoad) {
            UpdateTabImage({ isValid: true }, "tabAttachedDocument");
        } else {
            UpdateTabImage({ isValid: false }, "tabAttachedDocument");
            valid = false;
        }
        console.log("valid: " + valid);

        if (valid) {
            gvLiquidationRiverDocumentsAttachedDocuments.UpdateEdit();
        }
    }
}


function PrintDocument(s, e) {
    var data = { id_lf: $("#id_LiquidationFreightRiver").val() };

    $.ajax({
        url: 'LiquidationFreightRiver/PrintLiquidationFreightRiverReport',
        data: data,
        async: true,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                if (result != undefined) {
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
                    newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                    newWindow.focus();
                    hideLoading();
                }
            }
            catch (err) {
                hideLoading();
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

// DETAILS BUTTONS ACTIONS

function RefreshDetail(s, e) {
    Refresh(s, e);
}

// SELECTION

var customCommand = "";

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {

    if (tabControl.GetActiveTab().name === "tabdetail") {
        activeGridView = gvLiquidationFreightRiverDetail;
    } 

    if (activeGridView === null)
        return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + activeGridView.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = activeGridView.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    var codeDocumentState = $("#codeDocumentState").val();

    if (codeDocumentState == "01") {
        var lblInfo = null;
        var lnkSelectAllRows = "";
        var lnkClearSelection = "";

        if (activeGridView === gvLiquidationFreightRiverDetail) {
            lblInfo = $("#lblInfoDetails");
            lnkSelectAllRows = "lnkSelectAllRowsDetails";
            lnkClearSelection = "lnkClearSelectionDetails";
        
        }

        if (lblInfo !== null && lblInfo !== undefined) {
            lblInfo.html(text);
        }

        SetElementVisibility(lnkSelectAllRows, activeGridView.GetSelectedRowCount() > 0 && activeGridView.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility(lnkClearSelection, activeGridView.GetSelectedRowCount() > 0);
    }
    
}

function GetSelectedFilteredRowCount() {
    return activeGridView.cpFilteredRowCountWithoutPage + activeGridView.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

// TABS FUNCTIONS

var activeGridView = null;

function TabControl_Init(s, e) {
    activeGridView = null;
    if (tabControl.GetActiveTab().name === "tabdetail") {
        activeGridView = gvLiquidationFreightDetail;
    } 
}

function TabControl_ActiveTabChanged(s, e) {

    var enabeled = false;
    var codeDocumentState = $("#codeDocumentState").val();

    if (codeDocumentState == "01") {

        if (tabControl.GetActiveTab().name === "tabdetail") {
            activeGridView = gvLiquidationFreightRiverDetail;
            enabeled = true;
        } 
    }

    if (enabeled === true) {
        UpdateTitlePanel();
    }
}

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
    var id = parseInt($("#id_LiquidationFreightRiver").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "LiquidationFreightRiver/Actions",
        type: "post",
        data: { id: id },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            btnApprove.SetEnabled(result.btnApprove);
            btnAutorize.SetEnabled(result.btnAutorize);
            btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            hideLoading();
        }
    });

    // HISTORY BUTTON
    btnHistory.SetEnabled(id !== 0);

    // PRINT BUTTON
    btnPrint.SetEnabled(id !== 0);
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "LiquidationFreightRiver/InitializePagination",
        type: "post",
        data: { id_LiquidationFreightRiver: $("#id_LiquidationFreightRiver").val() },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            $("#pagination").attr("data-max-page", result.maximunPages);
            current_page = result.currentPage;
        },
        complete: function () {
        }
    });
    $('.pagination').current_page = current_page;
}

// MAIN FUNCTIONS

function init() {
    UpdatePagination();
    AutoCloseAlert();
}

function pricedays_Init() {
    pricedays.SetValue(0);

}

function priceextension_Init() {
    priceextension.SetValue(0);
}

function priceadjustment_Init() {
    priceadjustment.SetValue(0);

}

function pricedays_ValueChanged(s, e) {

    var pricedaysAux = s.GetValue();
    var priceextensionAux = priceextension.GetValue();
    var priceadjustmentAux = priceadjustment.GetValue();

    var strpricedaysAux       = pricedaysAux       == null ? "0" : pricedaysAux.toString();
    var strpriceextensionAux  = priceextensionAux  == null ? "0" : priceextensionAux.toString();
    var strpriceadjustmentAux = priceadjustmentAux == null ? "0" : priceadjustmentAux.toString();

    
    var respricedays          = strpricedaysAux.replace(".", ",");
    var respriceextension     = strpriceextensionAux.replace(".", ",");
    var respriceadjustment    = strpriceadjustmentAux.replace(".", ",");


    UpdateItemInfo({
        pricedays: respricedays,
        priceextension: respriceextension,
        priceadjustment: respriceadjustment
    });

}


function OnBatchEditEndCallback(s, e) {

    $.ajax({
        url: "LiquidationFreightRiver/BachEdiTItemDetailData",
        type: "post",
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                pricetotal.SetValue(result.ItemData.pricetotal);
                pricedays.SetValue(result.ItemData.pricedays);
                priceextension.SetValue(result.ItemData.priceextension);
                priceadjustment.SetValue(result.ItemData.priceadjustment);
                pricesubtotal.SetValue(result.ItemData.pricesubtotal);
                priceavance.SetValue(result.ItemData.priceavance);
                price.SetValue(result.ItemData.price);
                PriceCancelledTotal.SetValue(result.ItemData.priceCancelled);
            }
        },
        complete: function () {
        }
    });
}

function priceextension_ValueChanged(s, e) {

    var pricedaysAux = pricedays.GetValue();
    var priceextensionAux = s.GetValue();
    var priceadjustmentAux = priceadjustment.GetValue();

    var strpricedaysAux = pricedaysAux == null ? "0" : pricedaysAux.toString();
    var strpriceextensionAux = priceextensionAux == null ? "0" : priceextensionAux.toString();
    var strpriceadjustmentAux = priceadjustmentAux == null ? "0" : priceadjustmentAux.toString();

    var respricedays = strpricedaysAux.replace(".", ",");
    var respriceextension = strpriceextensionAux.replace(".", ",");
    var respriceadjustment = strpriceadjustmentAux.replace(".", ",");

    UpdateItemInfo({
        pricedays: respricedays,
        priceextension: respriceextension,
        priceadjustment: respriceadjustment
    });
}

function priceadjustment_ValueChanged(s, e) {
     
    var pricedaysAux = pricedays.GetValue();
    var priceextensionAux = priceextension.GetValue();
    var priceadjustmentAux = s.GetValue();

    var strpricedaysAux = pricedaysAux == null ? "0" : pricedaysAux.toString();
    var strpriceextensionAux = priceextensionAux == null ? "0" : priceextensionAux.toString();
    var strpriceadjustmentAux = priceadjustmentAux == null ? "0" : priceadjustmentAux.toString();

    var respricedays = strpricedaysAux.replace(".", ",");
    var respriceextension = strpriceextensionAux.replace(".", ",");
    var respriceadjustment = strpriceadjustmentAux.replace(".", ",");


    UpdateItemInfo({
        pricedays: respricedays,
        priceextension: respriceextension,
        priceadjustment: respriceadjustment
    });
}


function UpdateItemInfo(data) {
    $.ajax({
        url: "LiquidationFreightRiver/ItemDetailData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                pricetotal.SetValue(result.ItemData.pricetotal);
            }
        },
        complete: function () {
        }
    });
}


$(function () {
    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});