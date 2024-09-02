

// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    //var data = $("#formFilterPurchaseOrder").serialize();
    var data = $("#formFilterLogistics").serialize();

    if (data != null) {
        $.ajax({
            url: "RemGuideLandAditional/RemissionGuideResults",
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
    id_documentState.SetSelectedItem(null);
    number.SetText("");
    reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    startAuthorizationDate.SetDate(null);
    endAuthorizationDate.SetDate(null);
    authorizationNumber.SetText("");
    accessKey.SetText("");

    startDespachureDate.SetDate(null);
    endDespachureDate.SetDate(null);
    startArrivalDate.SetDate(null);
    endArrivalDate.SetDate(null);
    startReturnDate.SetDate(null);
    endReturnDate.SetDate(null);
}

function AddNewGuideRemissionManual() {
    var data = {
        id: 0,
        requestDetails: []
    };

    showPage("RemGuideLandAditional/FormEditRemissionGuide", data);
}

function AddNewGuideRemissionFromPurchaseOrder() {
    $.ajax({
        url: "RemGuideLandAditional/PurchaseOrderDetailsResults",
        type: "post",
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            // 
            $("#btnCollapse").click();
            $("#results").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });

    event.preventDefault();
}

// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvRemisssionGuide.GetSelectedFieldValues("id", function (values) {

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
                gvRemisssionGuide.PerformCallback();
                gvRemisssionGuide.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
     
    AddNewGuideRemissionManual(s, e);
}

function CopyDocument(s, e) {
    gvRemisssionGuide.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemGuideLandAditional/RemissionGuideCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemGuideLandAditional/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        showLoading();
        genericSelectedFieldActionCallBack("gvRemisssionGuide", "RemGuideLandAditional/AutorizeDocuments",
            function (result) {
                hideLoading();
                if (result.codeReturn == 1) {
                    gvRemisssionGuide.PerformCallback();
                    gvRemisssionGuide.UnselectRows();
                }
                if (result.message.length > 0) {
                    $("#msgInfoRemisssionGuide").empty();

                    $("#msgInfoRemisssionGuide").append(result.message)
                        .show()
                        .delay(5000)
                        .hide(0);
                }
            });
    }, "¿Desea Autorizar en el SRI de los documentos seleccionados?");
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("RemGuideLandAditional/AutorizeDocuments");
    //}, "¿Desea autorizar los documentos seleccionados?");
}

function CheckAutorizeRSIDocuments(s, e) {
	showConfirmationDialog(function () {
        showLoading();
		genericSelectedFieldActionCallBack("gvRemisssionGuide", "RemGuideLandAditional/CheckAutorizeRSIDocuments",
			function (result) {
                hideLoading();
                if (result.codeReturn == 1) {
                    gvRemisssionGuide.PerformCallback();
					gvRemisssionGuide.UnselectRows();
				}
				if (result.message.length > 0) {
					$("#msgInfoRemisssionGuide").empty();

					$("#msgInfoRemisssionGuide").append(result.message)
						.show()
						.delay(5000)
						.hide(0);
				}
			});
	}, "¿Desea Verificar la Autorización en el SRI de los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemGuideLandAditional/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemGuideLandAditional/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemGuideLandAditional/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
    gvRemisssionGuide.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemGuideLandAditional/RemissionGuideReport",
            type: "post",
            data: { id: selectedRows[0] },
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

// REMISSION GUIDES RESULT GRIDVIEW EDIT ACTION

function GridViewRemissionGuideCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuide.GetRowKey(e.visibleIndex)
        };
        showPage("RemGuideLandAditional/FormEditRemissionGuide", data);
    }
}

// REMISSION GUIDES RESULT GRIDVIEW SELECTION

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

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuide.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuide.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvRemisssionGuide.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuide.GetSelectedRowCount() > 0 && gvRemisssionGuide.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuide.GetSelectedRowCount() > 0);
    //}


    btnCopy.SetEnabled(false);
    btnApprove.SetEnabled(false);
    //btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    btnPrint.SetEnabled(false);

    //btnCopy.SetEnabled(gvRemisssionGuide.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvRemisssionGuide.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvRemisssionGuide.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvRemisssionGuide.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvRemisssionGuide.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvRemisssionGuide.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvRemisssionGuide.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvRemisssionGuide.GetSelectedRowCount() == 1);

}

function GetSelectedFilteredRowCount() {
    return gvRemisssionGuide.cpFilteredRowCountWithoutPage + gvRemisssionGuide.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuide.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuide.SelectRows();
}

// GENERATE REMISSION GUIDE FROM PURCHASE ORDERS

function GenerateRemissionGuide(s, e) {
    gridMessageErrorPurchaseOrder.SetText("");
    $("#GridMessageErrorPurchaseOrder").hide();
    
    showLoading();

    gvPurchaseOrderDetails.GetSelectedFieldValues("id;PurchaseOrder.id_certification", function (values) {

        var selectedPurchaseOrderDetailsRows = [];

        var certificationAux = null;
        for (var i = 0; i < values.length; i++) {
            if (i === 0) {
                certificationAux = values[i][1];
            } else {
                if (certificationAux !== values[i][1]) {
                    hideLoading();
                    var aError = "Los detalles seleccionado deben de tener igual Certificado en su orden de compra correspondiente";
                    gridMessageErrorPurchaseOrder.SetText(aError);
                    $("#GridMessageErrorPurchaseOrder").show();
                    return;
                }
            }
            selectedPurchaseOrderDetailsRows.push(values[i][0]);
        }
      
        var data = {
            id: 0,
            orderDetails: selectedPurchaseOrderDetailsRows
        };
        // 
        $.ajax({
            url: "RemGuideLandAditional/ValidateSelectedRowsPurchaseOrder",
            type: "post",
            data: { ids: data.orderDetails },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();

            },
            success: function (result) {
                if (result.Message == "OK") {
                    showPage("RemGuideLandAditional/FormEditRemissionGuide", data);
                } else {
                    gridMessageErrorPurchaseOrder.SetText(result.Message);
                    $("#GridMessageErrorPurchaseOrder").show();
                    hideLoading();
                }
            },
            complete: function () {
            }
        });
    });
}

// PURCHASE ORDERS RESULT GRIDVIEW SELECTION

function OnGridViewPurchaseOrderDetailsInit(s, e) {
    UpdateTitlePanelOrderDetails();
}

var selectedPurchaseOrderDetailsRows = [];

function OnGridViewPurchaseOrderDetailsSelectionChanged(s, e) {
    UpdateTitlePanelOrderDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function GetSelectedFieldDetailValuesCallback(values) {
    selectedPurchaseOrderDetailsRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedPurchaseOrderDetailsRows.push(values[i]);
    }
}

function OnGridViewPurchaseOrderDetailsEndCallback() {
    UpdateTitlePanelOrderDetails();
}

function UpdateTitlePanelOrderDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredOrderDetailsRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrderDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrderDetails.GetSelectedRowCount() - GetSelectedFilteredOrderDetailsRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvPurchaseOrderDetails.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrderDetails.GetSelectedRowCount() > 0 && gvPurchaseOrderDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
    //}

    btnGenerateRemissionGuide.SetEnabled(gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredOrderDetailsRowCount() {
    return gvPurchaseOrderDetails.cpFilteredRowCountWithoutPage + gvPurchaseOrderDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function GridViewPurchaseRequestDetailsClearSelection() {
    gvPurchaseOrderDetails.UnselectRows();
}

function GridViewPurchaseRequestDetailsSelectAllRow() {
    gvPurchaseOrderDetails.SelectRows();
}

// REMISSION GUIDE MASTER DETAILS FUNCTIONS

function RemissionGuideDetailViewDetails_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}

function RemissionGuideDetailViewDispatchMaterials_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}

function RemissionGuideDetailViewAssignedStaff_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}

function RemissionGuideDetailViewSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}

// MAINS FUNCTIONS

function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
}

function ComboProductionUnitProvider_SelectedIndexChanged(s, e) {
    var data = {
        id_ProductionUnitProvider: s.GetValue()
    };
     
    valuePrice.SetValue(0);
    
 //   id_productionUnitProviderPool.PerformCallback();
    $.ajax({
        url: "PurchaseOrder/GetAddressPurchaseRemisionGuideProvider",
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

            var sdata;
            sdata = route.GetValue();



             
            // if (sdata.includes(result.Provider_address) )
            //{
            route.SetValue(result.ProductionUnitProvider_address);
            nameFishingSite.SetValue(result.nameFishingSite);
            nameFishingZone.SetValue(result.nameFishingZone);


          
            //}




        },
        complete: function () {
          

        }
    });

}

function DespachureDate_ValueChanged(s, e) {
    arrivalDate.SetDate(s.GetValue());
    returnDate.SetDate(s.GetValue());

}
function ComboProviderRemisionGuide_SelectedIndexChanged(s, e) {
    var data = {
        id_provider: s.GetValue()
    };
     
    nameFishingSite.SetValue(null);
    nameFishingZone.SetValue(null);

        $.ajax({
            url: "PurchaseOrder/GetAddressPurchaseRemisionGuideProvider",
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

                var sdata;
                sdata=route.GetValue();
  



               // if (sdata.includes(result.Provider_address) )
               //{
                route.SetValue(result.Provider_address)
                //}
               



            },
            complete: function () {
                $.ajax({
                    url: "PurchaseOrder/UpdatePurchaseOrderprotectiveProvider",
                    type: "post",
                    data: data,
                    async: true,
                    cache: false,
                    error: function (error) {
                        console.log(error);
                   
                    },
                    beforeSend: function () {
                        //showLoading();
                    },
                    success: function (result) {
                         
                        if (result !== null && result !== undefined) {
                  
                            if (result.id_protectiveProvider != null) UpdatePurchaseOrderProtectiveProvider(result.id_protectiveProvider);

                        }
                  
                    },
                    complete: function () {
                        //hideLoading();
                        id_productionUnitProvider.PerformCallback();
                      
                    }
                });

            }
        });
   
}

function UpdatePurchaseOrderProtectiveProvider(protectiveProvider) {
     
    for (var i = 0; i < id_protectiveProvider.GetItemCount() ; i++) {
        var providerapparent = id_protectiveProvider.GetItem(i);
        var into = false;

        if (protectiveProvider == providerapparent.value) {
            id_protectiveProvider.selectedValue = protectiveProvider;
            id_protectiveProvider.SetSelectedIndex(providerapparent.index);

         

            break;
        }


    }

}


$(function () {
    init();
});