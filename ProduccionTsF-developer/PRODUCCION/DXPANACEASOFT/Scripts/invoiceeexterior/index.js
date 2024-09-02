//PRINT REPORT OPTIONS
function PRInvoiceFiscal(s, e) {
	var codeReport = "RFFLV1";
	var data = "codeReport=" + codeReport + "&" + $("#InvoiceExteriorFilterForm").serialize();

	if (data != null) {
		$.ajax({
			url: "InvoiceExterior/PRInvoiceFiscal",
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

// Buttons
function AddNewDocument(s, e) {
	AddNewInvoiceExteriorManual(s, e);
}

function CopyDocument(s, e) {

	gvInvoiceExterior.GetSelectedFieldValues("id", function (values) {
		if (values.length > 0) {
			showPage("InvoiceExterior/InvoiceCopy", { id: values[0] });
		}
	});


}

function ApprovePartialDocuments(s, e) {
	showConfirmationDialog(function () {
		genericSelectedFieldActionCallBack("gvInvoiceExterior", "InvoiceExterior/ApprovePartialDocuments",
			function (result) {

				if (result.codeReturn == 1) {
					gvInvoiceExterior.UnselectRows();
				}

				if (result.message.length > 0) {
					$("#msgInfoInvoiceExterior").empty();

					$("#msgInfoInvoiceExterior").append(result.message)
						.show()
						.delay(5000)
						.hide(0);
				}

			});
	}, "¿Desea aprobar parcialmente los documentos seleccionados?");
}

function ApproveDocuments(s, e) {
	showConfirmationDialog(function () {
		genericSelectedFieldActionCallBack("gvInvoiceExterior", "InvoiceExterior/ApproveDocuments",
			function (result) {

				if (result.codeReturn == 1) {
					gvInvoiceExterior.UnselectRows();
				}

				if (result.message.length > 0) {
					$("#msgInfoInvoiceExterior").empty();

					$("#msgInfoInvoiceExterior").append(result.message)
						.show()
						.delay(5000)
						.hide(0);
				}

			});
	}, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
	showConfirmationDialog(function () {
		genericSelectedFieldActionCallBack("gvInvoiceExterior", "InvoiceExterior/AutorizeDocuments",
			function (result) {
				if (result.codeReturn == 1) {
					gvInvoiceExterior.UnselectRows();
				}
				if (result.message.length > 0) {
					$("#msgInfoInvoiceExterior").empty();

					$("#msgInfoInvoiceExterior").append(result.message)
						.show()
						.delay(5000)
						.hide(0);
				}
			});
	}, "¿Desea Autorizar los documentos seleccionados?");
}

function CheckAutorizeRSIDocuments(s, e) {
	showConfirmationDialog(function () {
		genericSelectedFieldActionCallBack("gvInvoiceExterior", "InvoiceExterior/CheckAutorizeRSIDocuments",
			function (result) {
				if (result.codeReturn == 1) {
					gvInvoiceExterior.UnselectRows();
				}
				if (result.message.length > 0) {
					$("#msgInfoInvoiceExterior").empty();

					$("#msgInfoInvoiceExterior").append(result.message)
						.show()
						.delay(5000)
						.hide(0);
				}
			});
	}, "¿Desea Verificar la Autorización en el SRI de los documentos seleccionados?");
}

function CancelDocuments(s, e) {
	showConfirmationDialog(function () {
		genericPerformDocumentAction("gvInvoiceExterior", "InvoiceExterior/CancelDocuments");
	}, "¿Desea anular los documentos seleccionados?");
}

//function Print(s, e)
//{ }
function PrintDocuments(s, e) {
	showConfirmationDialog(function () {
		genericSelectedFieldActionCallBack("gvInvoiceExterior", null,
			function (rows) {
				PrintInvoiceGeneric(rows);
			});
	}, "¿Desea visualizar los documentos seleccionados?");

}




// Grid Master

function InvoiceExterior_OnSelectionChanged(s, e) {

	var _btnPrint = ASPxClientControl.GetControlCollection().GetByName("btnPrint");
	var _btnApprove = ASPxClientControl.GetControlCollection().GetByName("btnApprove");
	var _btnApprovePartial = ASPxClientControl.GetControlCollection().GetByName("btnApprovePartial");

	if (s.GetSelectedRowCount() == null || s.GetSelectedRowCount() == 0) {

		if (typeof _btnPrint != 'undefined' && _btnPrint != null) {
			_btnPrint.SetEnabled(false);
		}


		if (typeof _btnApprove != 'undefined' && _btnApprove != null) {
			_btnApprove.SetEnabled(false);
		}

		if (typeof _btnApprovePartial != 'undefined' && _btnApprovePartial != null) {
			_btnApprovePartial.SetEnabled(false);
		}

		return;
	}

	_btnPrint.SetEnabled(true);
	_btnApprove.SetEnabled(true);
	_btnApprovePartial.SetEnabled(true);








}
function InvoiceExterriorOnRowDoubleClick(s, e) {

	s.GetRowValues(e.visibleIndex, "id", function (value) {
		showPage("InvoiceExterior/FormEditInvoiceExterior", { id: value });
	});
}

function InvoiceExteriorSelectAllRows() {
	gvInvoiceExterior.SelectRows();

}

function InvoiceExteriorClearSelection() {
	gvInvoiceExterior.UnselectRows();

}
InvoiceExteriorsGridViewCustomCommandButton_Click

function InvoiceExteriorsGridViewCustomCommandButton_Click(s, e) {
	if (e.buttonID === "btnEditRow") {
		var data = {
			id: gvInvoiceExterior.GetRowKey(e.visibleIndex)
		};
		showPage("InvoiceExterior/FormEditInvoiceExterior", data);
	}
}

function UpdateButtons() {
	// $("#btnRevert").hide();
	genericSetElementVisibility("btnHistory", false);
	//genericSetElementVisibility("btnRevert", false);
	genericSetElementVisibility("btnProtect", false);
	//genericSetElementVisibility("btnCopy", false);

};

function InvoiceExteriorResultsDetailViewDetails_BeginCallback(s, e) {
	e.customArgs["id_invoiceExterior"] = $("#id_invoiceExterior").val();
}

// funciones Default

function btnSearch_click(s, e) {

	var data = $("#InvoiceExteriorFilterForm").serialize();

	if (data != null) {
		$.ajax({
			url: "invoiceExterior/InvoiceExteriorResults",
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

function btnClear_click(s, e) {

	fullname_businessName.SetText("");
	identity.SetText("");
	id_documentState.SetSelectedItem(null);


	fechaEmisionDesde.SetDate(null);
	fechaEmisionHasta.SetDate(null);

	number.SetText("");

	fechaEmbarqueDesde.SetDate(null);
	fechaEmbarqueHasta.SetDate(null);

	id_shippingAgency.SetSelectedItem(null);

	id_portDischarge.SetSelectedItem(null);
	id_portDestination.SetSelectedItem(null);

}

function OnRangeEmissionDateValidation(s, e) {
	OnRangeDateValidation(e, fechaEmisionDesde.GetValue(), fechaEmisionHasta.GetValue(), "Rango de Fecha no válido");
}

function OnRangeEmbarqueDateValidation(s, e) {
	OnRangeDateValidation(e, fechaEmbarqueDesde.GetValue(), fechaEmbarqueHasta.GetValue(), "Rango de Fecha no válido");
}

function AddNewInvoiceExteriorManual(s, e) {
	var data = {
		id: 0,
		requestDetails: []
	};

	showPage("InvoiceExterior/FormEditInvoiceExterior", data);

}

function AddNewInvoiceExteriorFromSalesQuotation(s, e) {

	$.ajax({
		url: "InvoiceExterior/SalesQuotationList",
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
			$("#btnCollapse").click();
			$("#results").html(result);
		},
		complete: function () {
			hideLoading();
		}
	});

	event.preventDefault();
}

function gvSalesQuotationBeginCallback(s, e) {
	e.customArgs["isCallback"] = true;
}

function AddNewInvoiceExteriorFromInvoiceCommercial(s, e) {

	$.ajax({
		url: "InvoiceExterior/InvoiceCommercialList",
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
			$("#btnCollapse").click();
			$("#results").html(result);
		},
		complete: function () {
			hideLoading();
		}
	});

	event.preventDefault();
}

function GenerateNewFromSalesQuotation(s, e) {
	gvSalesQuotation.GetSelectedFieldValues("id", function (values) {

		if (values.length > 0) {

			var data = {
				id: values[0],
				isReasignacion: false
			};
			showPage("InvoiceExterior/FormEditInvoiceExteriorFromSalesQuotation", data);
		}
		else {
			//Mostrar mensaje de error de seleccion.
		}
	});
}
function GeneratereasignNewFromSalesQuotation(s, e) {
	gvSalesQuotation.GetSelectedFieldValues("id", function (values) {

		if (values.length > 0) {

			var data = {
				id: values[0],
				isReasignacion: true
			};
			showPage("InvoiceExterior/FormEditInvoiceExteriorFromSalesQuotation", data);
		}
		else {
			//Mostrar mensaje de error de seleccion.
		}
	});
}


function gvInvoiceCommercialBeginCallback(s, e) {
	e.customArgs["callBack"] = true;
}
function GenerateNewFromInvoiceCommercial(s, e) {
	gvInvoiceCommercial.GetSelectedFieldValues("id", function (values) {

		if (values.length > 0) {

			var data = {
				id: values[0],
				isReasignacion: false
			};
			showPage("InvoiceExterior/FormEditInvoiceExteriorFromInvoiceCommercial", data);
		}
		else {
			//Mostrar mensaje de error de seleccion.
		}
	});
}
function GeneratereasignNewFromInvoiceCommercial(s, e) {
	gvInvoiceCommercial.GetSelectedFieldValues("id", function (values) {

		if (values.length > 0) {

			var data = {
				id: values[0],
				isReasignacion: true
			};
			showPage("InvoiceExterior/FormEditInvoiceExteriorFromInvoiceCommercial", data);
		}
		else {
			//Mostrar mensaje de error de seleccion.
		}
	});
}

var globalIdMetricUnitInvoice = 0;
var globalDinamycIdMetricUnitInvoice = 0;

function validationInvoiceExteriorMetricUnitInvoice(s, e) {
	if (s.GetValue() == null) {

        /*e.isValid = false;
        e.errorText = "Debe seleccionar unidad de medida";*/
		s.SetValue(globalIdMetricUnitInvoice);

	}
	else {

		globalIdMetricUnitInvoice = s.GetValue();
	}

}



function onInitInvoiceExteriorMetricUnitInvoice(s, e) {
	globalIdMetricUnitInvoice = s.GetValue();
	globalDinamycIdMetricUnitInvoice = s.GetValue();
	validateFirstExistMetricUnitInvoice(s, e);

}

function onChangeInvoiceExteriorMetricUnitInvoice(s, e) {

	if (globalDinamycIdMetricUnitInvoice != s.GetValue()) {
		validateFirstExistMetricUnitInvoice(s, e);
		PostChangeMetricUnitInvoice();
		globalDinamycIdMetricUnitInvoice = s.GetValue();
	}

}

// Edit:    2018-06-28
// Tkt:     20180628ix1
// Action:  Incluir en las opciones de Unidad de Medida: No Definido
// Task:    Permitir el uso de id 0
function validateFirstExistMetricUnitInvoice(s, e) {
	var objid_metricUnitInvoice = null;
	if (s != null) {
		objid_metricUnitInvoice = s.GetValue();
	}
	else {
		objid_metricUnitInvoice = (ASPxClientControl.GetControlCollection().GetByName("id_metricUnitInvoice") == null)
			? null : ASPxClientControl.GetControlCollection().GetByName("id_metricUnitInvoice").GetValue();
	}

	var _psv_metricUnitInvoiceDetail = (objid_metricUnitInvoice == undefined)
		? 0 : ((objid_metricUnitInvoice == null || objid_metricUnitInvoice == 0) ? 0 : objid_metricUnitInvoice);

	var _action = (objid_metricUnitInvoice == undefined) ? false : ((objid_metricUnitInvoice == null) ? false : true);

	$("#id_metricUnitInvoiceDetail").val(_psv_metricUnitInvoiceDetail);
}

function PostChangeMetricUnitInvoice() {
	if (gvInvoiceDetail.cpVisibleRowCount > 0 && !gvInvoiceDetail.IsEditing()) {
		GenericFreeStyleShowConfirmationDialogTwoOptionsWithActionRightNow("Al cambiar la unidad de medida se cambiará a todos los items ingresados?<br>Esto podría hacer que varie la cantidad del producto.<br>Si existe esta variación que desea que ocurra?", "Afectar al Precio", "Afectar al Total", function () { afectarPrecio() }, function () { afectarTotal() });
	}
}


function afectarPrecio() {

	var _data = { id_MetricUnitInvoice: id_metricUnitInvoice.GetValue(), accion: "PRICE" };
	//  var _data = {};
	genericPerformDocumentActionWithData("gvInvoiceDetail", "InvoiceExterior/ChangeMetricUnitInvoiceMaster", _data);
}


function afectarTotal() {

	var _data = { id_MetricUnitInvoice: id_metricUnitInvoice.GetValue(), accion: "TOTAL" };
	//var _data = {};
	genericPerformDocumentActionWithData("gvInvoiceDetail", "InvoiceExterior/ChangeMetricUnitInvoiceMaster", _data);
}

function tariffHeadingDetail_SelectedIndexChanged(s, e) { }


// validaciones
function OnValidation(s, e) {
	e.isValid = true;
}


// --------------------------
// Detalle
// --------------------------
function InvoiceExteriorDetail_OnGridViewInit(s, e) {

	// 
	updateEstatusButton();
}

function InvoiceExteriorDetail_OnSelectionChanged(s, e) {
	updateEstatusButton();
}

function updateEstatusButton() {
	//TODO: Revisar utilidad
	//btnRemoveDetail.SetEnabled(gvInvoiceDetail.GetSelectedRowCount() > 0);
}

function Invoice_ChangeInvoiceTotal(s, e) {


	reloadInvoiceTotal();
}

function reloadInvoiceTotal() {
	var _valueTransportationExpenses = (valueTransportationExpenses.GetValue() == null) ? 0 : valueTransportationExpenses.GetValue().toString().replace(".", ",");
	var _valueCustomsExpenditures = (valueCustomsExpenditures.GetValue() == null) ? 0 : valueCustomsExpenditures.GetValue().toString().replace(".", ",");
	var _valueInternationalInsurance = (valueInternationalInsurance.GetValue() == null) ? 0 : valueInternationalInsurance.GetValue().toString().replace(".", ",");
	var _valueInternationalFreight = (valueInternationalFreight.GetValue() == null) ? 0 : valueInternationalFreight.GetValue().toString().replace(".", ",");

	var data = {
		valueTransportationExpenses: _valueTransportationExpenses,
		valueCustomsExpenditures: _valueCustomsExpenditures,
		valueInternationalInsurance: _valueInternationalInsurance,
		valueInternationalFreight: _valueInternationalFreight
	};

	var errCallBack = function (error) { console.log(error) };
	var okCallBackTotales = function (result) {
		if (result != null) {
			$("#objTotalesPartial").empty();
			$("#objTotalesPartial").html(result);
            InvoiceExteriorTermsNegotiation_SelectedIndexChanged(null, null);
            OnInvoiceTotalValueReady();
            valueTotal.SetValue(valuetotalCIF.GetValue());
            var aValueSubscribed = valueSubscribed.GetValue() == null ? 0.00 : valueSubscribed.GetValue();
            balance.SetValue(valueTotal.GetValue() - aValueSubscribed); 
		}
	};

	var okCallBackInforAdicional = function (result) {
		if (result != null) {
			$("#objInfoAdicional").empty();
			$("#objInfoAdicional").html(result);
		}
	};

	genericAjaxCall('InvoiceExterior/InvoiceExternalTotales', false, data, errCallBack, showLoading(), okCallBackTotales, hideLoading());
	genericAjaxCall('InvoiceExterior/InvoiceExternalWeight', false, {}, errCallBack, showLoading(), okCallBackInforAdicional, hideLoading());

}


function setTariffHeadingDescription() {


	// (_url, _async, _data, actionError, actionBeforeSend, actionSuccess, actionComplete)
	genericAjaxCall('InvoiceExterior/tariffheadingdesc'
		, true
		, { id_invoice: $("#id_invoice").val() }
		, function (error) { console.log(error) }
		, null, function (result) {

			if (result != null) {
				if (result.codeReturn == -1) {
					console.log(result.message);
					return;
				}

				if (result.ValueDataList.length > 0) {
					var obj = ASPxClientControl.GetControlCollection().GetByName(result.ValueDataList[0].CodeObject);
					if (typeof obj != 'undefined' && obj != null) {
						obj.SetValue(result.ValueDataList[0].valueObject);

					}

				}

			}


		}, null)
}

function InvoiceExteriorDetail_OnStartEditing(s, e)
{
	// 
	if (e.cellInfo.column.name == 'masterCode')
	{
		e.cancel = true;
	}
}
// Inicializacion Grid
function InvoiceExteriorDetail_OnEndCallback(s, e) {
	/* edicion actualizacion*/
	if (s.editMode == 1 && s.editState == 1) {
		numBoxesValueChanged(null, null);
	}
    reloadInvoiceTotal();

	if (s.editMode == 1 && s.editState == 0) {
		//reloadInvoiceTotal();
		setTariffHeadingDescription();
        //OnInvoiceTotalValueReady();
    }
}


function InvoiceExteriorDetail_OnBeginCallback(s, e) {
	e.customArgs["id_invoice"] = $("#id_invoice").val();
	validateFirstExistMetricUnitInvoice(null, null);

	if (e.command == 'STARTEDIT') {
		id_metricUnitInvoice.SetEnabled(false);
		$("#amount").val(0);
		$("#id_amountInvoice").val(0);
	}

	else if (e.command == 'ADDNEWROW') {
		id_metricUnitInvoice.SetEnabled(false);
		$("#amount").val(0);
		$("#id_amountInvoice").val(0);
		$("#id_metricUnitInvoiceDetail").val(0);
	}

	else if (e.command == 'UPDATEEDIT') {
		id_metricUnitInvoice.SetEnabled(true);
		validateFirstExistMetricUnitInvoice(null, null);
		$("#amount").val(0);
		$("#id_amountInvoice").val(0);

		e.customArgs["id_metricUnitInvoiceDetail"] = $("#id_metricUnitInvoiceDetail").val();
	}

	else if (e.command == 'DELETEROW') {
		id_metricUnitInvoice.SetEnabled(true);
		validateFirstExistMetricUnitInvoice(null, null);
	}

	else if (e.command == 'CANCELEDIT') {
		id_metricUnitInvoice.SetEnabled(true);
		validateFirstExistMetricUnitInvoice(null, null);
	}
}

function DevInvoiceDetail_masterCode_Init(s, e)
{
	// 
	var masterCode = s;
	masterCode.inputElement.tabIndex = -1;

}
function DevInvoiceDetail_description2_Init(s, e)
{
	var description2 = s;
	description2.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_auxCode_Init(s, e)
{
	var auxCode = s;
	auxCode.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_amountDisplay_Init(s, e)
{
	var amountDisplay = s;
	amountDisplay.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_amountInvoiceDisplay_Init(s, e) {
	var amountInvoiceDisplay = s;
	amountInvoiceDisplay.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_proformaWeight_Init(s, e) {
	var proformaWeight = s;
	proformaWeight.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_amountProformaDisplay_Init(s, e) {
	var amountProformaDisplay = s;
	amountProformaDisplay.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_unitPriceProforma_Init(s, e) {
	var unitPriceProforma = s;
	unitPriceProforma.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_totalProforma_Init(s, e) {
	var totalProforma = s;
	totalProforma.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_unitPrice_Init(s, e) {

	// 
	if ( gvInvoiceDetail.cpDocumentOrigen !== null && gvInvoiceDetail.cpDocumentOrigen !== "" ) // Desde Proforma
	{
		var unitPrice = s;
		unitPrice.inputElement.tabIndex = -1;
	}
}
function DevInvoiceDetail_totalPriceWithoutTax_Init(s, e) {
	
	var totalPriceWithoutTax = s;
	totalPriceWithoutTax.inputElement.tabIndex = -1;
}
// Generic Print
function PrintInvoiceGeneric(idsInvoice) {


	if (typeof idsInvoice === 'undefined' || idsInvoice === null || idsInvoice.length == 0) {
		return;
	}


	for (var i = 0; i < idsInvoice.length; i++) {

		data = { id_inv: idsInvoice[i] };

		$.ajax({
			url: 'InvoiceExterior/PrintInvoiceExteriorReport',
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
						var _nameWindow = Math.round(Math.random() * 10000000000000) + "_prt";
						newWindow = window.open(url, _nameWindow, 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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

// Generic Print Propia
function PrintInvoiceGenericPropia(idsInvoice) {


	if (typeof idsInvoice === 'undefined' || idsInvoice === null || idsInvoice.length == 0) {
		return;
	}


	for (var i = 0; i < idsInvoice.length; i++) {

		data = { id_inv: idsInvoice[i] };

		$.ajax({
			url: 'InvoiceExterior/PrintInvoiceExteriorPropioReport',
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
						var _nameWindow = Math.round(Math.random() * 10000000000000) + "_prt";
						newWindow = window.open(url, _nameWindow, 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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

// Generic Print Certificada
function PrintInvoiceGenericCertificada(idsInvoice) {


	if (typeof idsInvoice === 'undefined' || idsInvoice === null || idsInvoice.length == 0) {
		return;
	}


	for (var i = 0; i < idsInvoice.length; i++) {

		data = { id_inv: idsInvoice[i] };

		$.ajax({
			url: 'InvoiceExterior/PrintInvoiceExteriorCertificadaReport',
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
						var _nameWindow = Math.round(Math.random() * 10000000000000) + "_prt";
						newWindow = window.open(url, _nameWindow, 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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


// Generic Print Propia
function PrintInvoiceHeight(idsInvoice) {


	if (typeof idsInvoice === 'undefined' || idsInvoice === null || idsInvoice.length == 0) {
		return;
	}


	for (var i = 0; i < idsInvoice.length; i++) {

		data = { id_inv: idsInvoice[i] };

		$.ajax({
			url: 'InvoiceExterior/PrintInvoiceExteriorHeight',
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
						var _nameWindow = Math.round(Math.random() * 10000000000000) + "_prt";
						newWindow = window.open(url, _nameWindow, 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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

// Generic Print Paking List
function PrintInvoiceGenericPakingList(idsInvoice) {


	if (typeof idsInvoice === 'undefined' || idsInvoice === null || idsInvoice.length == 0) {
		return;
	}


	for (var i = 0; i < idsInvoice.length; i++) {

		data = { id_inv: idsInvoice[i] };

		$.ajax({
			url: 'InvoiceExterior/PrintInvoiceExteriorPakinListReport',
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
						var _nameWindow = Math.round(Math.random() * 10000000000000) + "_prt";
						newWindow = window.open(url, _nameWindow, 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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

// Generic Print Non Wood
function PrintInvoiceGenericNonWood(idsInvoice) {


    if (typeof idsInvoice === 'undefined' || idsInvoice === null || idsInvoice.length == 0) {
        return;
    }


    for (var i = 0; i < idsInvoice.length; i++) {

        data = { id_inv: idsInvoice[i] };

        $.ajax({
            url: 'InvoiceExterior/PrintInvoiceExteriorNonWoodReport',
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
                        var _nameWindow = Math.round(Math.random() * 10000000000000) + "_prt";
                        newWindow = window.open(url, _nameWindow, 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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


function personValidation(s, e) {


	//var _id_buyer = ASPxClientControl.GetControlCollection().GetByName("id_buyer").GetValue();
	var _id_buyer = id_buyer.GetValue();
	if (e.value == null && id_buyer != null) {
		//e.errorText = "Debe seleccionar"
		//e.isValid = false;
		e.value = _id_buyer;
	}

	return e.value;

}


//  funciones Default

function init() {

}

$(function () {


	var chkReadyStateMain = setInterval(function () {
		if (document.readyState === "complete") {
			clearInterval(chkReadyStateMain);
			UpdateButtons();
			SetCollapseButton();
		}
	}, 100);


	init();
});

function SetCollapseButton() {

	//btnCollapse
}

