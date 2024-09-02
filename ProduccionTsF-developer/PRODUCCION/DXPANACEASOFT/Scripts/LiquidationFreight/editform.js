//POP UP FOR REMISSION GUIDE SELECTION
function OnGridViewForPopUpRemissionGuideInit(s, e) {
    UpdateTitlePanelForPopUp();
}

var selectedRemissionGuideRowspopup = [];
var selectedRemissionGuideRowsPopUpFinal = [];

function ClosePopUp_BtnClick() {
    popupRemissionGuide.Hide();
}

function AddRemissionGuideFromPopUp(s, e) {
    // 
    var key = "id";
    // 
    var selectedRows = [];

    var url = "LiquidationFreight/AddSelectedRemissionGuides";
    $.ajax({
        url: url,
        type: "post",
        data: { ids: selectedRemissionGuideRowsPopUpFinal },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function () {
            // TODO: 
        },
        complete: function () {
            selectedRemissionGuideRowsPopUpFinal = [];
            gvLiquidationFreightDetail.PerformCallback();
            popupRemissionGuide.Hide();
        }
    });

}

function OnGridViewSelectionChangedForPopUpRemissionGuide(s, e) {
    // 
    var index = e.visibleIndex;
    var key = s.GetRowKey(e.visibleIndex);

    if (e.isSelected) {
        selectedRemissionGuideRowsPopUpFinal.push(key);
    }
    else {
        for (var i = 0; i < selectedRemissionGuideRowsPopUpFinal.length; i++) {
            if (selectedRemissionGuideRowsPopUpFinal[i] == key) {
                selectedRemissionGuideRowsPopUpFinal.splice(i, 1);
            }
        }
    }
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallbackpopup);
    UpdateTitlePanelForPopUp();
}

function GetSelectedFieldDetailValuesCallbackpopup(values) {
    // 
    selectedRemissionGuideRowspopup = [];

    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideRowspopup.push(values[i]);
    }
}

function OnGridViewEndCallbackForPopUpRemissionGuide(s, e) {
    UpdateTitlePanelForPopUp();
    var codeDocumentState = $("#codeDocumentState").val();
}

function UpdateTitlePanelForPopUp() {
    if (gvRemissionGuidesForLiquidation === null)
        return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountPopup();
    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuidesForLiquidation.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuidesForLiquidation.GetSelectedRowCount() - GetSelectedFilteredRowCountPopup();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";

    text += "<br />";

    var codeDocumentState = $("#codeDocumentState").val();

    var lblInfo = null;
    var lnkSelectAllRows = "";
    var lnkClearSelection = "";

    lblInfo = $("#lblInfoDetailsRemision");
    lnkSelectAllRows = "lnkSelectAllRowsDetailsRemission";
    lnkClearSelection = "lnkClearSelectionDetailsRemission";

    if (lblInfo !== null && lblInfo !== undefined) {
        lblInfo.html(text);
    }

    SetElementVisibility(lnkSelectAllRows, gvRemissionGuidesForLiquidation.GetSelectedRowCount() > 0 && gvRemissionGuidesForLiquidation.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility(lnkClearSelection, gvRemissionGuidesForLiquidation.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountPopup() {
    return gvRemissionGuidesForLiquidation.cpFilteredRowCountWithoutPage + gvRemissionGuidesForLiquidation.GetSelectedKeysOnPage().length;
}

function AddRemissionGuide(s, e) {
    // 
    var data = { id_pt: $("#id_providertransport").val() };
    var url = "LiquidationFreight/GetRemissionGuidesPopUpPartial";

    $.ajax({
        url: url,
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
        success: function () {
            // 
            popupRemissionGuide.Show();
            popupRemissionGuide.PerformCallback();
            hideLoading();
        },
        complete: function () {

        }
    });
}

//LIQUIDATION FREIGHT DOCUMENTS
// DETAILS ACTIONS ATTACHED DOCUMENTS

function AddNewAttachedDocumentDetail(s, e) {
    gvLiquidationDocumentsAttachedDocuments.AddNewRow();
}

function RemoveAttachedDocumentDetail(s, e) {
    //Remove(s, e);
}

function RefreshAttachedDocumentDetail(s, e) {
    //Refresh(s, e);
    gvLiquidationDocumentsAttachedDocuments.UnselectRows();
    gvLiquidationDocumentsAttachedDocuments.PerformCallback();
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
                        url: "LiquidationFreight/ItsRepeatedAttachmentDetail",//ItsRepeatedLiquidation",
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
            gvLiquidationDocumentsAttachedDocuments.UpdateEdit();
        }
    }
}

function ButtonUpdateDetailDocument_Click() {
    var id = $("#id_LiquidationFreight").val();
    var data = "id=" + id  + "&" + $("#FormEditLiquidationFreight").serialize() + "&id_providertransport" + id_providertransport;
    var url = "LiquidationFreight/UpdateDetailDocumentLiquidation";
    showForm(url, data);
}

//DIALOG BUTTONS ACTIONS
function Update(approve) {
    // 
    //tabControl.SetActiveTabIndex = 0;
    tabControl.SetActiveTab(tabControl.GetTab(0))
    
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabdetail", true);

     if (!valid) {
         UpdateTabImage({ isValid: false }, "tabdetail");
     }

    if (typeof (gvLiquidationFreightDetail) != "undefined") {
        var _enEdicion = gvLiquidationFreightDetail.batchEditApi.HasChanges();
        if (_enEdicion) {
            UpdateTabImage({ isValid: false }, "tabdetail");
            valid &= false;
        }
    }
    debugger;
    if (typeof $("#id_providertransport").val() == undefined
        || $("#id_providertransport").val() == null
        || $("#id_providertransport").val() == ''
        || $("#id_providertransport").val() == 0
    )
    {
        UpdateTabImage({ isValid: false }, "tabdetail");
        valid &= false;
    }



     if (valid) {
          
         var id = $("#id_LiquidationFreight").val();
         var id_providertransport = $("#id_providertransport").val();
         
      
         var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#FormEditLiquidationFreight").serialize() + "&id_providertransport" + id_providertransport;
        var url = (id === "0") ? "LiquidationFreight/LiquidationFreightPartialAddNew"
                               : "LiquidationFreight/LiquidationFreightPartialUpdate";

        showForm(url, data);
    }
}


function ButtonUpdate_Click(s, e) {

    Update(false);
  
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    showPage("LiquidationFreight/Index", null);
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0
    };

    showPage("LiquidationFreight/RemissionGuideResults", data);
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

    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_remissionGuide").val()
    //    };
    //    showForm("Logistics/Approve", data);
    //}, "¿Desea aprobar el documento?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_LiquidationFreight").val()
        };
        showForm("LiquidationFreight/Autorize", data);
    }, "¿Desea autorizar la Liquidación de Flete?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_LiquidationFreight").val()
        };
        showForm("LiquidationFreight/Protect", data);
    }, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_LiquidationFreight").val()
        };
        showForm("LiquidationFreight/Cancel", data);
    }, "¿Desea anular la Liquidación de Flete?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_LiquidationFreight").val()
        };
        showForm("LiquidationFreight/Revert", data);
    }, "¿Desea reversar la Liquidación de Flete?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {
    var data = { id_lf: $("#id_LiquidationFreight").val() };

    $.ajax({
        url: 'LiquidationFreight/PrintLiquidationFreightReport',
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
        activeGridView = gvLiquidationFreightDetail;
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

        if (activeGridView === gvLiquidationFreightDetail) {
            lblInfo = $("#lblInfoDetails");
            lnkSelectAllRows = "lnkSelectAllRowsDetails";
            lnkClearSelection = "lnkClearSelectionDetails";
        
        }

        if (lblInfo !== null && lblInfo !== undefined) {
            lblInfo.html(text);
        }

        SetElementVisibility(lnkSelectAllRows, activeGridView.GetSelectedRowCount() > 0 && activeGridView.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility(lnkClearSelection, activeGridView.GetSelectedRowCount() > 0);

      //  btnRemoveDetail.SetEnabled(activeGridView.GetSelectedRowCount() > 0);

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
            activeGridView = gvLiquidationFreightDetail;
            enabeled = true;
        } 

   
        //btnNewDetail.SetEnabled(enabeled);
        //btnRemoveDetail.SetEnabled(enabeled);
        //btnRefreshDetails.SetEnabled(enabeled);
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
    var id = parseInt($("#id_LiquidationFreight").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "LiquidationFreight/Actions",
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
        url: "LiquidationFreight/InitializePagination",
        type: "post",
        data: { id_LiquidationFreight: $("#id_LiquidationFreight").val() },
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
    var PriceCancelledTotalAux = PriceCancelledTotal.GetValue();

    var strpricedaysAux       = pricedaysAux       == null ? "0" : pricedaysAux.toString();
    var strpriceextensionAux  = priceextensionAux  == null ? "0" : priceextensionAux.toString();
    var strpriceadjustmentAux = priceadjustmentAux == null ? "0" : priceadjustmentAux.toString();
    var strPriceCancelledTotalAux = PriceCancelledTotalAux == null ? "0" : PriceCancelledTotalAux.toString();
    
    var respricedays          = strpricedaysAux.replace(".", ",");
    var respriceextension     = strpriceextensionAux.replace(".", ",");
    var respriceadjustment    = strpriceadjustmentAux.replace(".", ",");
    var resPriceCancelledTotalAux = strPriceCancelledTotalAux.replace(".", ",");

    UpdateItemInfo({
        pricedays: respricedays,
        priceextension: respriceextension,
        priceadjustment: respriceadjustment
    });

}

function OnBatchEditEndCallback(s, e) {

    $.ajax({
        url: "LiquidationFreight/BachEdiTItemDetailData",
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
            url: "LiquidationFreight/ItemDetailData",
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