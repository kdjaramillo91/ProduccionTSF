////Validation 
//function OnValidation(s, e) {
//    e.isValid = true;
//}

//function OnRangeEmissionDateValidation(s, e) {
//    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
//}

//function OnRangeAutorizationDateValidation(s, e) {
//    OnRangeDateValidation(e, startAuthorizationDate.GetValue(), endAuthorizationDate.GetValue(), "Rango de Fecha no válido");
//}


//// FILTERS FORM ACTIONS

//function btnSearch_click(s, e) {

//    var data = $("#formFilterSalesOrder").serialize();

//    if (data != null) {
//        $.ajax({
//            url: "SalesOrder/SalesOrderResults",
//            type: "post",
//            data: data,
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                showLoading();
//            },
//            success: function (result) {
//                $("#btnCollapse").click();
//                $("#results").html(result);
//            },
//            complete: function () {
//                hideLoading();
//            }
//        });
//    }
//    event.preventDefault();
//}

//function btnClear_click(s, e) {

//    id_documentState.SetSelectedItem(null);
//    number.SetText("");
//    reference.SetText("");
//    startEmissionDate.SetDate(null);
//    endEmissionDate.SetDate(null);
//    startAuthorizationDate.SetDate(null);
//    endAuthorizationDate.SetDate(null);
//    authorizationNumber.SetText("");
//    accessKey.SetText("");
//    items.ClearTokenCollection();

//    id_customer.SetSelectedItem(null);
//    id_employeeSeller.SetSelectedItem(null);
//    filterLogistic.SetChecked(true);
//   //id_priceList.SetSelectedItem(null);
//    //id_paymentTerm.SetSelectedItem(null);
//    //id_paymentMethod.SetSelectedItem(null);
//}

//function AddNewItemManual(s, e) {
//    var data = {
//        id: 0,
//        requestDetails: []
//    };

//    showPage("SalesOrder/FormEditSalesOrder", data);
//}

//function AddNewItemFromSalesRequest(s, e) {
//    $.ajax({
//        url: "SalesOrder/SalesRequestDetailsResults",
//        type: "post",
//        async: true,
//        cache: false,
//        error: function (error) {
//            console.log(error);
//        },
//        beforeSend: function () {
//            showLoading();
//        },
//        success: function (result) {
//            $("#btnCollapse").click();
//            $("#results").html(result);
//        },
//        complete: function () {
//            hideLoading();
//        }
//    });

//    event.preventDefault();
//}

//// COMMON GRIDVIEW AUXILIARS FUNCTIONS

//function GetGridViewSelectedRows(gv, key) {
//    var selectedRows = [];
//    gv.GetSelectedFieldValues(key, function (values) {
//        for (var i = 0; i < values.length; i++) {
//            selectedRows.push(values[i]);
//        }
//    });
//    return selectedRows;
//}

//function SetElementVisibility(id, visible) {
//    var $element = $("#" + id);
//    visible ? $element.show() : $element.hide();
//}

//// GRIDVIEW PURCHASE ORDERS RESULT ACTIONS BUTTONS

//function PerformDocumentAction(url) {
//    gvSalesOrders.GetSelectedFieldValues("id", function (values) {

//        var selectedRows = [];
//        for (var i = 0; i < values.length; i++) {
//            selectedRows.push(values[i]);
//        }

//        $.ajax({
//            url: url,
//            type: "post",
//            data: { ids: selectedRows },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                //showLoading();
//            },
//            success: function (result) {
//                //console.log(result);
//            },
//            complete: function () {
//                //hideLoading();
//                gvSalesOrders.PerformCallback();
//                // gvSalesOrders.UnselectRows();
//            }
//        });

//    });
//}

//function AddNewDocument(s, e) {
//    AddNewItemManual(s, e);
//}

//function CopyDocument(s, e) {

//}

//function ApproveDocuments(s, e) {
//    showConfirmationDialog(function () {
//        PerformDocumentAction("SalesOrder/ApproveDocuments");
//    }, "¿Desea aprobar los documentos seleccionados?");
//}

//function AutorizeDocuments(s, e) {
//    showConfirmationDialog(function () {
//        PerformDocumentAction("SalesOrder/AutorizeDocuments");
//    }, "¿Desea autorizar los documentos seleccionados?");
//}

//function ProtectDocuments(s, e) {
//    showConfirmationDialog(function () {
//        PerformDocumentAction("SalesOrder/ProtectDocuments");
//    }, "¿Desea cerrar los documentos seleccionados?");
//}

//function CancelDocuments(s, e) {
//    showConfirmationDialog(function () {
//        PerformDocumentAction("SalesOrder/CancelDocuments");
//    }, "¿Desea anular los documentos seleccionados?");
//}

//function RevertDocuments(s, e) {
//    showConfirmationDialog(function () {
//        PerformDocumentAction("SalesOrder/RevertDocuments");
//    }, "¿Desea reversar los documentos seleccionados?");
//}

//function ShowHistory(s, e) {

//}

//function Print(s, e) {

//    gvSalesOrders.GetSelectedFieldValues("id", function (values) {

//        var selectedRows = [];

//        for (var i = 0; i < values.length; i++) {
//            selectedRows.push(values[i]);
//        }

//        $.ajax({
//            url: "SalesOrder/SalesOrdersReport",
//            type: "post",
//            data: { ids: selectedRows },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                showLoading();
//            },
//            success: function (result) {
//                $("#maincontent").html(result);
//            },
//            complete: function () {
//                hideLoading();
//            }
//        });

//    });

//}

//function SalesOrdersGridViewCustomCommandButton_Click(s, e) {
//    if (e.buttonID === "btnEditRow") {
//        var data = {
//            id: gvSalesOrders.GetRowKey(e.visibleIndex)
//        };
//        showPage("SalesOrder/FormEditSalesOrder", data);
//    }
//}

//// GRIDVIEW PURCHASE ORDERS SELECTION

//function SalesOrdersOnGridViewInit(s, e) {
//    SalesOrdersUpdateTitlePanel();
//}

//function SalesOrdersOnGridViewSelectionChanged(s, e) {
//    SalesOrdersUpdateTitlePanel();
//}

//function SalesOrdersOnGridViewEndCallback() {
//    SalesOrdersUpdateTitlePanel();
//}

//function SalesOrdersUpdateTitlePanel() {
//    var selectedFilteredRowCount = SalesOrdersGetSelectedFilteredRowCount();

//    var text = "Total de elementos seleccionados: <b>" + gvSalesOrders.GetSelectedRowCount() + "</b>";
//    var hiddenSelectedRowCount = gvSalesOrders.GetSelectedRowCount() - SalesOrdersGetSelectedFilteredRowCount();

//    if (hiddenSelectedRowCount > 0)
//        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
//    text += "<br />";
//    $("#lblInfo").html(text);

//    //if ($("#selectAllMode").val() != "AllPages") {
//    SetElementVisibility("lnkSelectAllRows", gvSalesOrders.GetSelectedRowCount() > 0 && gvSalesOrders.cpVisibleRowCount > selectedFilteredRowCount);
//    SetElementVisibility("lnkClearSelection", gvSalesOrders.GetSelectedRowCount() > 0);
//    //}

//    btnCopy.SetEnabled(gvSalesOrders.GetSelectedRowCount() === 1);
//    btnApprove.SetEnabled(gvSalesOrders.GetSelectedRowCount() > 0);
//    btnAutorize.SetEnabled(gvSalesOrders.GetSelectedRowCount() > 0);
//    btnProtect.SetEnabled(gvSalesOrders.GetSelectedRowCount() > 0);
//    btnCancel.SetEnabled(gvSalesOrders.GetSelectedRowCount() > 0);
//    btnRevert.SetEnabled(gvSalesOrders.GetSelectedRowCount() > 0);
//    btnHistory.SetEnabled(gvSalesOrders.GetSelectedRowCount() === 1);
//    btnPrint.SetEnabled(gvSalesOrders.GetSelectedRowCount() === 1);

//}

//function SalesOrdersGetSelectedFilteredRowCount() {
//    return gvSalesOrders.cpFilteredRowCountWithoutPage + gvSalesOrders.GetSelectedKeysOnPage().length;
//}

//function SalesOrdersSelectAllRows() {
//    gvSalesOrders.SelectRows();
//}

//function SalesOrdersClearSelection() {
//    gvSalesOrders.UnselectRows();
//}

//// GRIDVIEW PURCHASE REQUEST RESULTS ACTIONS

//function GenerateOrder(s, e) {

//    gridMessageErrorSalesRequest.SetText("");
//    $("#GridMessageErrorSalesRequest").hide();

//    gvSalesRequestDetails.GetSelectedFieldValues("id", function (values) {
//        var selectedRows = [];

//        for (var i = 0; i < values.length; i++) {
//            selectedRows.push(values[i]);
//        }

//        var data = {
//            requestDetails: selectedRows,
//            id: 0
//        };

//        $.ajax({
//            url: "SalesOrder/ValidateSelectedRowsSalesRequest",//ValidateSelectedRowsSalesQuotation",//ValidateSelectedRowsPurchaseRequest",
//            type: "post",
//            data: { ids: selectedRows },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                showLoading();

//            },
//            success: function (result) {
//                if (result.Message == "OK") {
//                    showPage("SalesOrder/FormEditSalesOrder", data);
//                } else {
//                    gridMessageErrorSalesRequest.SetText(result.Message);
//                    $("#GridMessageErrorSalesRequest").show();
//                    hideLoading();
//                }
//            },
//            complete: function () {
//                //hideLoading();
//                //gvProductionLotReceptions.PerformCallback();
//                // gvPurchaseOrders.UnselectRows();
//            }
//        });


//    });

//    //gvSalesRequestDetails.GetSelectedFieldValues("id_salesRequest;id_item", function (values) {

//    //    var selectedRows = [];

//    //    for (var i = 0; i < values.length; i++) {
//    //        selectedRows.push(values[i]);
//    //    }

//    //    var data = {
//    //        id: 0,
//    //        requestDetails: []
//    //    };

//    //    for (var k = 0; k < selectedRows.length; k++) {
//    //        data.requestDetails.push({
//    //            id_salesRequest: selectedRows[k][0],
//    //            id_item: selectedRows[k][1]
//    //        });
//    //    }

//    //    showPage("SalesOrder/FormEditSalesOrder", data);
//    //});
//}

//// GRIDVIEW PURCHASE REQUEST RESULTS SELECTION

//function SalesRequestDetailsOnGridViewInit(s, e) {
//    SalesRequestDetailsUpdateTitlePanel();
//}

//function SalesRequestDetailsOnGridViewSelectionChanged(s, e) {
//    SalesRequestDetailsUpdateTitlePanel();
//}

//function SalesRequestDetailsOnGridViewEndCallback() {
//    SalesRequestDetailsUpdateTitlePanel();
//}

//function SalesRequestDetailsUpdateTitlePanel() {
//    var selectedFilteredRowCount = SalesRequestDetailsGetSelectedFilteredRowCount();

//    var text = "Total de elementos seleccionados: <b>" + gvSalesRequestDetails.GetSelectedRowCount() + "</b>";
//    var hiddenSelectedRowCount = gvSalesRequestDetails.GetSelectedRowCount() - SalesRequestDetailsGetSelectedFilteredRowCount();
//    if (hiddenSelectedRowCount > 0)
//        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
//    text += "<br />";
//    $("#lblInfo").html(text);

//    //console.log(gvSalesRequestDetails.GetSelectedRowCount());

//    //if ($("#selectAllMode").val() != "AllPages") {
//    SetElementVisibility("lnkSelectAllRows", gvSalesRequestDetails.GetSelectedRowCount() > 0 && gvSalesRequestDetails.cpVisibleRowCount > selectedFilteredRowCount);
//    SetElementVisibility("lnkClearSelection", gvSalesRequestDetails.GetSelectedRowCount() > 0);
//    //}

//    btnGenerateOrder.SetEnabled(gvSalesRequestDetails.GetSelectedRowCount() > 0);
//}

//function SalesRequestDetailsGetSelectedFilteredRowCount() {
//    return gvSalesRequestDetails.cpFilteredRowCountWithoutPage + gvSalesRequestDetails.GetSelectedKeysOnPage().length;
//}

//function SalesRequestDetailsSelectAllRow() {
//    gvSalesRequestDetails.SelectRows();
//}

//function SalesRequestDetailsClearSelection() {
//    gvSalesRequestDetails.UnselectRows();
//}

//// MASTER DETAILS FUNCTIONS 

//function SalesOrderResultsDetailViewDetails_BeginCallback(s, e) {
//    e.customArgs["id_salesOrder"] = $("#id_salesOrder").val();
//}

//// MAIN FUNCTIONS

//function init() {
//    $("#btnCollapse").click(function (event) {
//        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
//            $("#results").css("display", "");
//        } else {
//            $("#results").css("display", "none");
//        }
//    });
//}

//$(function () {
//    init();
//});



function SearchClick() {

	$("#btnCollapse").click();

	var dateInicio = DateEditInit.GetDate();
	var yearInicio = dateInicio.getFullYear();
	var monthInicio = dateInicio.getMonth() + 1;
	var dayInicio = dateInicio.getDate();

	var dateFin = DateEditEnd.GetDate();
	var yearFin = dateFin.getFullYear();
	var monthFin = dateFin.getMonth() + 1;
	var dayFin = dateFin.getDate();

	var data = {
		initDate: dayInicio + "/" + monthInicio + "/" + yearInicio,
		endtDate: dayFin + "/" + monthFin + "/" + yearFin,
		id_state: ComboBoxState.GetValue(),
		number: TextBoxNumber.GetText(),
		reference: TextBoxReference.GetText(),

		id_documentType: ComboBoxDocumentTypeIndex.GetValue(),
		id_customer: ComboBoxCustomerIndex.GetValue(),
		id_seller: ComboBoxSellerIndex.GetValue(),
		items: TokenBoxItemsIndex.GetTokenValuesCollection(),
		id_Logistics: ComboBoxLogisticsIndex.GetValue()
	};

	showPartialPage($("#grid"), 'SalesOrder/SearchResult', data);
}

function ClearClick() {
	var dayNow = new Date();
	DateEditInit.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
	DateEditEnd.SetDate(dayNow);
	ComboBoxState.SetValue(null);
	TextBoxNumber.SetText(null);
	TextBoxReference.SetText(null);
	ComboBoxDocumentTypeIndex.SetValue(null);
	ComboBoxCustomerIndex.SetValue(null);
	ComboBoxSellerIndex.SetValue(null);
	TokenBoxItemsIndex.ClearTokenCollection();
	ComboBoxLogisticsIndex.SetValue(null);
}

function NewClickFromOrderStock() {
	var data = {
		id: 0,
		ids: [],
		id_proforma: null,
		enabled: true
	};
	showPage("SalesOrder/Edit", data);
}

function NewClickFromOrderLocalClient() {
	$.ajax({
		url: "SalesOrder/PendingNewOrderLocalClient",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			//// 
			NotifyError("Error. " + error);
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
}

function NewClickFromOrderForeignCustomers() {
	$.ajax({
		url: "SalesOrder/PendingNewOrderForeignCustomers",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			//// 
			NotifyError("Error. " + error);
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
}

function Init() {
	$("#btnCollapse").click(function (event) {
		if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
			$("#grid").css("display", "");
		} else {
			$("#grid").css("display", "none");
		}
	});
}

$(function () {
	Init();
});

