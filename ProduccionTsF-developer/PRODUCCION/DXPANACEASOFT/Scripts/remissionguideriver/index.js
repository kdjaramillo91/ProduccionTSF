// FILTER FORM BUTTONS ACTIONS
function btnSearch_click() {
    var data = $("#formFilterremissionriver").serialize();
    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiver/RemissionGuideRiverResults",
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
}

function AddNewGuideRemissionRiver() {
    var data = {
        id: 0,
        requestDetails: []
    };

    showPage("RemissionGuideRiver/FormEditRemissionGuideRiver", data);
}

function AddNewGuideRemissionRiverFromRemissionGuide() {
    $.ajax({
        url: "RemissionGuideRiver/RemissionGuideForRiverResults",
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

    //event.preventDefault();
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
    AddNewGuideRemissionRiver(s, e);
}

function CopyDocument(s, e) {
}

function ApproveDocuments(s, e) {
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        showLoading();
        genericSelectedFieldActionCallBack("gvRemisssionGuide", "RemissionGuideRiver/AutorizeDocuments",
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
}

function CheckAutorizeRSIDocuments(s, e) {
	showConfirmationDialog(function () {
        showLoading();
		genericSelectedFieldActionCallBack("gvRemisssionGuide", "RemissionGuideRiver/CheckAutorizeRSIDocuments",
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
}

function CancelDocuments(s, e) {
}

function RevertDocuments(s, e) {
}

function ShowHistory(s, e) {
}

function Print(s, e) {
}

// REMISSION GUIDES RESULT GRIDVIEW EDIT ACTION
function GridViewRemissionGuideRiverCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuide.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideRiver/FormEditRemissionGuideRiver", data);
    }
}

// GENERATE REMISSION GUIDE RIVER FROM REMISSION GUIDE

function GenerateRemissionGuideRiver(s, e) {
    gridMessageErrorRemissionGuideForRiver.SetText("");
    $("#GridMessageErrorRemissionGuideForRiver").hide();

    showLoading();

    gvRemissionGuideForRiver.GetSelectedFieldValues("id;requiredLogistic", function (values) {
        // 
        var selectedRemissionGuideForRiverRows = [];
        var requiredLogisticAux = null;
        for (var i = 0; i < values.length; i++) {
            if (requiredLogisticAux === null) {
                requiredLogisticAux = values[i][1];
            } else {
                if (requiredLogisticAux !== values[i][1]) {
                    gridMessageErrorRemissionGuideForRiver.SetText(ErrorMessage("Sólo se puedan seleccionar guías con el mismo tipo de Logística: Propia o Terceros."));
                    $("#GridMessageErrorRemissionGuideForRiver").show();
                    hideLoading();
                    return;
                } 
            }
            selectedRemissionGuideForRiverRows.push(values[i][0]);
        }

        var data = {
            id: 0,
            remissionGuides: selectedRemissionGuideForRiverRows
        };

        $.ajax({
            url: "RemissionGuideRiver/ValidateSelectedRowsRemissionGuide",
            type: "post",
            data: { ids: data.remissionGuides },
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
                if (result.Message == "OK") {
                    showPage("RemissionGuideRiver/FormEditRemissionGuideRiver", data);
                } else {
                    gridMessageErrorRemissionGuideForRiver.SetText(result.Message);
                    $("#GridMessageErrorRemissionGuideForRiver").show();
                    hideLoading();
                }
            },
                complete: function () {
            }
        });
    });
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
    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuide.GetSelectedRowCount() > 0 && gvRemisssionGuide.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuide.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(false);
    btnApprove.SetEnabled(false);
    //btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    //btnHistory.SetEnabled(false);
    btnPrint.SetEnabled(false);
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

// REMISSION GUIDE MASTER DETAILS FUNCTIONS
function RemissionGuideRiverDetailViewDetails_BeginCallback(s, e) {
    e.customArgs["id_remissionriverGuide"] = parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"));
}

function RemissionGuideRiverDetailViewDispatchMaterials_BeginCallback(s, e) {
    e.customArgs["id_remissionriverGuide"] = parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"));
}

function RemissionGuideRiverDetailViewAssignedStaff_BeginCallback(s, e) {
    e.customArgs["id_remissionriverGuide"] = parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"));
}

function RemissionGuideRiverDetailViewSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_remissionriverGuide"] = parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"));
}

//REMISSION GUIDE RIVER FROM REMISSION GUIDE
var selectedRemissionGuidesForRiverRows = [];
function OnGridViewRemissionGuideForRiverInit(s, e) {
    UpdateTitlePanelRemissionGuides();
}

function OnGridViewRemissionGuideForRiverSelectionChanged(s, e) {
    UpdateTitlePanelRemissionGuides();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function OnGridViewRemissionGuideForRiverEndCallback(s, e) {
    UpdateTitlePanelRemissionGuides();
}

function UpdateTitlePanelRemissionGuides() {
    var selectedFilteredRowCount = GetSelectedFilteredRemissionGuidesRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideForRiver.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideForRiver.GetSelectedRowCount() - GetSelectedFilteredRemissionGuidesRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideForRiver.GetSelectedRowCount() > 0 && gvRemissionGuideForRiver.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideForRiver.GetSelectedRowCount() > 0);
    

    btnGenerateRemissionGuideRiver.SetEnabled(gvRemissionGuideForRiver.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRemissionGuidesRowCount() {
    return gvRemissionGuideForRiver.cpFilteredRowCountWithoutPage + gvRemissionGuideForRiver.GetSelectedKeysOnPage().length;
}

function GetSelectedFieldDetailValuesCallback(values) {
    selectedRemissionGuidesForRiverRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuidesForRiverRows.push(values[i]);
    }
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
        url: "RemissionGuideRiver/GetAddressPurchaseRemisionGuideProvider",
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
            // 
            var sdata;
            sdata = route.GetValue();

            route.SetValue(result.ProductionUnitProvider_address);
            nameFishingSite.SetValue(result.nameFishingSite);
            nameFishingZone.SetValue(result.nameFishingZone);

                var fishingZoneObject = id_FishingZoneRGRNew.FindItemByValue(result.idFishingZone);
                //if (fishingZoneObject != null) id_FishingZoneRGRNew.SelectedItem(fishingZoneObject);
                if (fishingZoneObject != null) id_FishingZoneRGRNew.SetSelectedIndex(fishingZoneObject.index);
                id_FishingSiteRGR.PerformCallback();

        },
        complete: function () {
          

        }
    });

}

function DespachureDate_ValueChanged(s, e) {
  

}

function ComboProviderRemisionGuideRiver_SelectedIndexChanged(s, e) {
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
                
            },
            success: function (result) {

                var sdata;
               sdata=route.GetValue();
               route.SetValue(result.Provider_address);

            },
            complete: function () {
                //$.ajax({
                //    url: "PurchaseOrder/UpdatePurchaseOrderprotectiveProvider",
                //    type: "post",
                //    data: data,
                //    async: true,
                //    cache: false,
                //    error: function (error) {
                //        console.log(error);
                   
                //    },
                //    beforeSend: function () {
                //        //showLoading();
                //    },
                //    success: function (result) {
                         
                //        if (result !== null && result !== undefined) {
                  
                //            if (result.id_protectiveProvider != null) UpdatePurchaseOrderProtectiveProvider(result.id_protectiveProvider);

                //        }
                  
                //    },
                //    complete: function () {
                //        //hideLoading();
                  id_productionUnitProvider.PerformCallback();
                      
                //    }
                //});

            }
        });
   
}

function RemissionGuideRiverDetailViewAssignedStaff_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}

function RemissionGuideRiverDetailViewSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}

$(function () {
    init();
});