

// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    //var data = $("#formFilterPurchaseOrder").serialize();
    var data = "carRegistration=" + carRegistrationFilter.GetText() + "&" + $("#formFilterLogistics").serialize();

    if (data !== null) {
        $.ajax({
            url: "Logistics/RemissionGuideResults",
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
    startexitDateProductionBuilding.SetDate(null);
    endexitDateProductionBuilding.SetDate(null);
    startentranceDateProductionBuilding.SetDate(null);
    endentranceDateProductionBuilding.SetDate(null);
    carRegistrationFilter.SetText("");
}

function AddNewGuideRemissionManual() {
    var data = {
        id: 0,
        requestDetails: []
    };

    showPage("Logistics/FormEditRemissionGuide", data);
}

function AddNewGuideRemissionFromPurchaseOrder() {
    $.ajax({
        url: "Logistics/PurchaseOrderDetailsResults",
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

//Reassignment
function ReassignmentRemissionGuide() {
    $.ajax({
        url: "Logistics/PurchaseOrderDetailsResultsForReassignment",
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
            showPage("Logistics/RemissionGuideCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("Logistics/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {

    showConfirmationDialog(function () {
        showLoading();
        genericSelectedFieldActionCallBack("gvRemisssionGuide", "Logistics/AutorizeDocuments",
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


    gvRemisssionGuide.GetSelectedFieldValues("id", function (values) {

        debugger;
        if (values.length == 0) {
            viewMessageClient("msgInfoRemisssionGuide",prepareMessageClient('alert-warning', 'Debe seleccionar el/los guias(s) de remision'));
            return;
        }

        let valoresSeleccionados = values;
        showConfirmationDialog(function () {

            var selectedRows = [];
            for (var i = 0; i < valoresSeleccionados.length; i++) {
                selectedRows.push(valoresSeleccionados[i]);
            }

            genericAjaxCall("Logistics/CheckAutorizeRSIDocuments"
                , true
                , { ids: selectedRows }
                , function (error) { console.log(error); }
                , function () { showLoading(); }
                , function (result) {

                    if (result !== null) {

                        if (result.codeReturn == 1) {
                            gvRemisssionGuide.PerformCallback();
                            gvRemisssionGuide.UnselectRows();
                        }
                        if (result.message.length > 0) {

                            viewMessageClient("msgInfoRemisssionGuide", result.message);

                        }
                    }
                    else {
                        gvRemisssionGuide.PerformCallback();
                        gvRemisssionGuide.UnselectRows();
                    }

                }
                , function () { hideLoading(); }
            );


        }, "¿Desea Verificar la Autorización en el SRI de los documentos seleccionados?");


    });


    //showConfirmationDialog(function () {
    //    showLoading();
	//	genericSelectedFieldActionCallBack("gvRemisssionGuide", "Logistics/CheckAutorizeRSIDocuments",
    //        function (result) {
    //            hideLoading();
    //            if (result.codeReturn == 1) {
    //                gvRemisssionGuide.PerformCallback();
	//				gvRemisssionGuide.UnselectRows();
	//			}
	//			if (result.message.length > 0) {
	//				$("#msgInfoRemisssionGuide").empty();
    //
	//				$("#msgInfoRemisssionGuide").append(result.message)
	//					.show()
	//					.delay(5000)
	//					.hide(0);
	//			}
	//		});
	//}, "¿Desea Verificar la Autorización en el SRI de los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("Logistics/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("Logistics/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("Logistics/RevertDocuments");
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
            url: "Logistics/RemissionGuideReport",
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
        var aId = gvRemisssionGuide.GetRowKey(e.visibleIndex)
        $.ajax({
            url: 'Logistics/LockedDocument',
            data: {
                id_document: aId,
                nameDocument: "Orden de Compra",
                code_sourceLockedDocument: "GR",
                namesourceLockedDocument: "Guía de Remisión"
            },
            async: true,
            cache: false,
            type: 'POST',
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.Code !== 0) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                    return;
                }

                var data = {
                    id_rg: aId
                };

                $.ajax({
                    url: "Logistics/GetTypeFromRemissionGuide",
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
                        if (result != undefined) {
                            var data2 = { id: result.id_RemGuide }
                            if (result.RemGuideType == "TR") {
                                showPage("Logistics/FormEditRemissionGuideForReassignment", data2);
                            } else {
                                showPage("Logistics/FormEditRemissionGuide", data2);
                            }
                        }
                    },
                    complete: function () {
                    }
                });

            },
            complete: function () {
                //hideLoading();
            }
        });
        

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
                    gridMessageErrorPurchaseOrder.SetText(ErrorMessage(aError));
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

        $.ajax({
            url: "Logistics/ValidateSelectedRowsPurchaseOrder",
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
                    $.ajax({
                        url: 'Logistics/lockedDocumentOC',
                        data: {
                            ids: data.orderDetails,
                            nameDocument: "Orden de Compra",
                            code_sourceLockedDocument: "GR",
                            namesourceLockedDocument: "Guía de Remisión"
                        },
                        async: true,
                        cache: false,
                        type: 'POST',
                        beforeSend: function () {
                            showLoading();
                        },
                        success: function (result) {
                            if (result.Code !== 0) {
                                hideLoading();
                                NotifyError("Error. " + result.Message);
                                return;
                            }

                            showPage("Logistics/FormEditRemissionGuide", data);

                        },
                        complete: function () {
                            //hideLoading();
                        }
                    });
                    
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

function GeneratePurchaseDetailForReassignment(s, e) {
    gridMessageErrorPurchaseOrder.SetText("");
    $("#GridMessageErrorPurchaseOrder").hide();

    showLoading();

    gvPurchaseOrderDetails.GetSelectedFieldValues("id", function (values) {

        var selectedPurchaseOrderDetailsRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedPurchaseOrderDetailsRows.push(values[i]);
        }

        var data = {
            id: 0,
            orderDetails: selectedPurchaseOrderDetailsRows
        };

        $.ajax({
            url: "Logistics/ValidateSelectedRowsPurchaseOrder",
            type: "post",
            data: { ids: data.orderDetails },
            async: false,
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
                    //showPage("Logistics/FormEditRemissionGuide", data);
                    $.ajax({
                        url: "Logistics/RemissionGuideListForReassignmentResults",
                        type: "post",
                        async: false,
                        cache: false,
                        error: function (error) {
                            console.log(error);
                        },
                        beforeSend: function () {
                            showLoading();
                        },
                        success: function (result) {
                            // 
                            //$("#btnCollapse").click();
                            $("#results").html(result);
                        },
                        complete: function () {
                            hideLoading();
                        }
                    });

                    //event.preventDefault();
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

function GenerateRemissionGuideForReassignment(s, e) {

    gridMessageErrorRemissionGuideReassignment.SetText("");
    $("#GridMessageErrorRemissionGuideReassignment").hide();

    showLoading();

    gvRemissionGuideReassignment.GetSelectedFieldValues("id", function (values) {

        var selectedRGReassignmentRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRGReassignmentRows.push(values[i]);
        }

        var data = {
            id: 0,
            rgs: selectedRGReassignmentRows
        };
        showPage("Logistics/FormEditRemissionGuideForReassignment", data);
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


//PURCHASE ORDER REASSIGNMENT
function OnGridViewPurchaseOrderDetailsReassignmentInit(s, e) {
    UpdateTitlePanelOrderDetailsReassignment();
}

var selectedPurchaseOrderDetailsReassignmentRows = [];

function OnGridViewPurchaseOrderDetailsReassignmentSelectionChanged(s, e) {
    UpdateTitlePanelOrderDetailsReassignment();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailReassignmentValuesCallback);
}

function GetSelectedFieldDetailReassignmentValuesCallback(values) {
    selectedPurchaseOrderDetailsRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedPurchaseOrderDetailsReassignmentRows.push(values[i]);
    }
}

function OnGridViewPurchaseOrderDetailsReassignmentEndCallback() {
    UpdateTitlePanelOrderDetailsReassignment();
}

function UpdateTitlePanelOrderDetailsReassignment() {
    var selectedFilteredRowCount = GetSelectedFilteredOrderDetailsReassignmentRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrderDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrderDetails.GetSelectedRowCount() - GetSelectedFilteredOrderDetailsReassignmentRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrderDetails.GetSelectedRowCount() > 0 && gvPurchaseOrderDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
    //}

    btnGeneratePurchaseDetailForReassignment.SetEnabled(gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredOrderDetailsReassignmentRowCount() {
    return gvPurchaseOrderDetails.cpFilteredRowCountWithoutPage + gvPurchaseOrderDetails.GetSelectedKeysOnPage().length;
}

function GridViewPurchaseRequestDetailsReassignmentClearSelection() {
    gvPurchaseOrderDetails.UnselectRows();
}

function GridViewPurchaseRequestDetailsReassignmentSelectAllRow() {
    gvPurchaseOrderDetails.SelectRows();
}

//REASSIGNMENT
function OnGridViewRGReassignmentDetailsInit(s, e) {
    UpdateTitlePanelRGReassignment();
}

var selectedRGReassignmentRows = [];

function OnGridViewRGReassignmentDetailsSelectionChanged(s, e) {
    UpdateTitlePanelRGReassignment();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function OnGridViewRGReassignmentDetailsEndCallback() {
    UpdateTitlePanelRGReassignment();
}

function GetSelectedFieldDetailValuesCallback(values) {
    selectedRGReassignmentRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRGReassignmentRows.push(values[i]);
    }
}

function UpdateTitlePanelRGReassignment() {
    var selectedFilteredRowCount = GetSelectedFilteredRGReassignmentRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideReassignment.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideReassignment.GetSelectedRowCount() - GetSelectedFilteredRGReassignmentRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideReassignment.GetSelectedRowCount() > 0 && gvRemissionGuideReassignment.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideReassignment.GetSelectedRowCount() > 0);
    //}

    btnGenerateRemissionGuideForReasignment.SetEnabled(gvRemissionGuideReassignment.GetSelectedRowCount() == 1);
}

function GetSelectedFilteredRGReassignmentRowCount() {
    return gvRemissionGuideReassignment.cpFilteredRowCountWithoutPage + gvRemissionGuideReassignment.GetSelectedKeysOnPage().length;
}

function GridViewRGReassingmentClearSelection() {
    gvRemissionGuideReassignment.UnselectRows();
}

function GridViewRGReassingmentSelectAllRow() {
    gvRemissionGuideReassignment.SelectRows();
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

// REMISSION GUIDE REPORTS VERSION 
function LRGReport(s, e) {
    var codeReport = "GRLTF";
    //var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();
    // 

    var strDtStart = GetStringFromDate(startEmissionDate.GetText(), "dd/MM/yyyy");
    var strDtEnd = GetStringFromDate(endEmissionDate.GetText(), "dd/MM/yyyy");

    var data = {
        codeReport: "GRLTF"
        , str_emissionDateStart: strDtStart
        , str_emissionDateEnd: strDtEnd
    }

    if (data != null) {
        $.ajax({
            url: "Logistics/PrintRemissionGuideReportList",
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
                // 
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
}

function LRGReportExcel(s, e) {
    var codeReport = "GRLTF";
    // 

    var strDtStart = GetStringFromDate(startEmissionDate.GetText(), "dd/MM/yyyy");
    var strDtEnd = GetStringFromDate(endEmissionDate.GetText(), "dd/MM/yyyy");

    var data = {
        codeReport: "GRLTF"
        , str_emissionDateStart: strDtStart
        , str_emissionDateEnd: strDtEnd
    }

    if (data != null) {
        $.ajax({
            url: "Logistics/PrintRemissionGuideReportListExcel",
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
                // 
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/DownLoadExcelRemisionGuieT';
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
        id_provider: id_providerRemisionGuide.GetValue()
    };
    // 
    nameFishingSite.SetValue(null);
    nameFishingZone.SetValue(null);

    if (isCopackingRG.GetChecked()) {
        $.ajax({
            url: "PurchaseOrder/UpdatePurchaseOrderprotectiveProviderCopacking",
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
                route.SetValue(result.Provider_address);
                //}
            },
            complete: function () {
                $.ajax({
                    url: "PurchaseOrder/UpdatePurchaseOrderprotectiveProviderCopacking",
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

                            if (result.id_protectiveProvider !== null) UpdatePurchaseOrderProtectiveProvider(result.id_protectiveProvider);

                        }

                    },
                    complete: function () {
                        //hideLoading();
                        id_productionUnitProvider.PerformCallback();

                    }
                });

            }
        });
    } else {
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

                            if (result.id_protectiveProvider !== null) UpdatePurchaseOrderProtectiveProvider(result.id_protectiveProvider);

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
}

function UpdatePurchaseOrderProtectiveProvider(protectiveProvider) {

    for (var i = 0; i < id_protectiveProvider.GetItemCount(); i++) {
        var providerapparent = id_protectiveProvider.GetItem(i);
        var into = false;

        if (protectiveProvider === providerapparent.value) {
            id_protectiveProvider.selectedValue = protectiveProvider;
            id_protectiveProvider.SetSelectedIndex(providerapparent.index);



            break;
        }


    }

}

$(function () {
    init();
});


