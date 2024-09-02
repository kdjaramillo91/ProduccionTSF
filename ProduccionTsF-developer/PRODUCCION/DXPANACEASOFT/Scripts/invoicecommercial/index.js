function PRInvoiceComercial(s, e) {
	var codeReport = "RFCLV1";
	var data = "codeReport=" + codeReport + "&" + $("#InvoiceCommercialFilterForm").serialize();

	if (data != null) {
		$.ajax({
			url: "InvoiceCommercial/PRInvoiceComercial",
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
					if (result == undefined) {
						console.log("Función No Retorna Valores");
						return;
					}

					if (result.codeReturn == -1) {
						console.log(result.message);
						return;
					}

					if (result.ValueDataList.length <= 0) {
						console.log("No existe Resultado");
						return;
					}

					var oResult = result.ValueDataList[0].valueObject;
					var reportTdr = oResult.nameQS;
					var url = 'ReportProd/Index?trepd=' + reportTdr;
					newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
					newWindow.focus();
					hideLoading();
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

function AddNewInvoiceComercialManual(s, e) {
	var data = {
		id: 0,
		requestDetails: []
	};

	showPage("InvoiceCommercial/InvoiceCommercialFormEditPartial", data);
}

function AddNewInvoiceComercialFromInviceExterior() {
	$.ajax({
		url: "InvoiceCommercial/InvoiceExteriorDetailsResults",
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

function AddNewInvoiceExteriorFromSalesQuotation(s, e) {
	$.ajax({
		url: "InvoiceCommercial/SalesQuotationList",
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

//Validation
function portDestination_SelectedIndexChanged(s, e) {
	var idPortDestination = s.GetValue();
	if (idPortDestination == null) return;

	id_portDischarge.SetValue(idPortDestination);

	var idCity = s.GetSelectedItem().GetColumnText('idCity');
	id_CityDelivery.SetValue(idCity);
}

function OnChangeBuyer(s, e) {
	var route = "Person/getBuyerData";

	if (s.GetSelectedItem() == null) {
		businessNameForeignCustomer.SetText(null);
		//identification_numberForeignCustomer.SetText(null);
		//addressForeignCustomer.SetText(null);
		emailForeignCustomer.SetText(null);
		return;
	}
	//
	var id_personBuyer = s.GetSelectedItem().value;
	var data = "id_person=" + id_personBuyer;

	if (id_Consignee.GetValue() == null) {
		id_Consignee.SetValue(id_personBuyer);
		person2_Info("id_Consignee");
	}

	if (id_Notifier.GetValue() == null) {
		id_Notifier.SetValue(id_personBuyer);
		person2_Info("id_Notifier");
	}

	if (id_Notifier2.GetValue() == null) {
		id_Notifier2.SetValue(id_personBuyer);
	}
	InvoiceExteriorAddressCustomerUpdate(id_personBuyer)
	genericAjaxCall(route, true, data, null, null, function (result) {
		if (result === undefined) return;
		if (result == null) return;
		try {
			businessNameForeignCustomer.SetText(result.fullname_businessName);
			identification_numberForeignCustomer.SetText(result.identification_number);
			//addressForeignCustomer.SetText(result.address);
			emailForeignCustomer.SetText(result.email);
		}
		catch (err) {
			console.log(err);
		}
	}, null)
}

function InvoiceExteriorAddressCustomerUpdate(id_personBuyerAux) {
	id_addressCustomer.ClearItems();
	id_addressCustomer.SetValue(null);


	$.ajax({
		url: "InvoiceCommercial/SetAddressCustomer",
		type: "post",
		data: {
			id_consignee: id_personBuyerAux
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {

			if (result !== null && result.length > 0) {
				for (var i = 0; i < result.length; i++) {
					id_addressCustomer.AddItem([result[i].tipoDireccion, result[i].name, result[i].emailInterno, result[i].emailInternoCC, result[i].phone1FC, result[i].fax1FC], result[i].id);
				}

				var value = result.length === 1 ? result[0].id : null;
				id_addressCustomer.SetValue(value);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function person2_Info(nameParentControl) {
	var oParentControl = ASPxClientControl.GetControlCollection().GetByName(nameParentControl);
	if (oParentControl === undefined || oParentControl === null) return;

	var _phone = oParentControl.GetSelectedItem().GetColumnText('phone');
	var _fax = oParentControl.GetSelectedItem().GetColumnText('fax');

	var _subfijo = oParentControl.cpSubFijo;
	var _phoneControlName = "telefono_" + _subfijo;
	var _faxControlName = "Fax_" + _subfijo;

	var objPhone = ASPxClientControl.GetControlCollection().GetByName(_phoneControlName);
	var objFax = ASPxClientControl.GetControlCollection().GetByName(_faxControlName);

	if (typeof objPhone !== undefined && objPhone !== null) {
		objPhone.SetValue(_phone);
	}

	if (typeof objFax !== undefined && objFax !== null) {
		objFax.SetValue(_fax);
	}
}

function person2_SelectedIndexChanged(s, e) {
	person2_Info(s.name);
}

function OnValueChanged(s, e) {
	//
	var valorDae = s.GetValue();
	if (typeof valorDae == undefined || valorDae == null) {
		return;
	}

	var lenValorDae = (valorDae.length) + 1;
	if (lenValorDae >= s.cpMaxLength) {
		var obj = ASPxClientControl.GetControlCollection().GetByName(s.cpNextControl);
		if (typeof obj == undefined || obj == null) {
			return;
		}
		obj.Focus();
	}
}

function OnSelectText(s, e) {
	s.SelectAll();
}

function OnValidation(s, e) {
	e.isValid = true;
}

function OnRangeEmissionDateValidation(s, e) {
	OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

function OnRangeDateShipmentValidation(s, e) {
	OnRangeDateValidation(e, startDateShipment.GetValue(), endDateShipment.GetValue(), "Rango de Fecha no válido");
}

// Filter Action Buttons
function OnClickSearchInvoiceCommercial() {
	var data = $("#InvoiceCommercialFilterForm").serialize();

	if (data != null) {
		$.ajax({
			url: "InvoiceCommercial/InvoiceCommercialResultsPartial",
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
}

function OnClickClearFiltersInvoiceCommercial() {
	//Customer
	customers.ClearTokenCollection();
	identification.SetText("");
	consignees.ClearTokenCollection();
	notifiers.ClearTokenCollection();

	//Document
	DocumentStateCombo_Init();
	startEmissionDate.SetDate(null);
	endEmissionDate.SetDate(null);
	numberInvoiceFiscal.SetText("");

	//Shipment
	startDateShipment.SetDate(null);
	endDateShipment.SetDate(null);
	id_shippingAgencys.ClearTokenCollection();
	id_portDischarges.ClearTokenCollection();
	id_portDestinations.ClearTokenCollection();
	BLNumber.SetText("");
}

function ButtonManualAddNewInvoiceCommercial_Click() {
	var data = {
		id: 0
	};
	showPage("InvoiceCommercial/InvoiceCommercialFormEditPartial", data);
}

function OnClickAddNewOpeningClosingPlateLying(trx) {
	ButtonManualAddNewOpeningClosingPlateLying_Click();
}

// Filter ComboBox
function DocumentStateCombo_Init() {
	id_documentState.SetValue(null);
}

function ResponsableCombo_Init() {
	id_responsable.SetValue(null);
	id_responsable.SetText("");
}

function FreezerWarehouseCombo_Init() {
	id_freezerWarehouse.SetValue(null);
	id_freezerWarehouse.SetText("");
}

function MaintenanceWarehouseCombo_Init() {
	id_maintenanceWarehouse.SetValue(null);
	id_maintenanceWarehouse.SetText("");
}

function MaintenanceWarehouseLocationCombo_Init() {
	id_maintenanceWarehouseLocation.SetValue(null);
	id_maintenanceWarehouseLocation.SetText("");
}

// Results GridView Selection
function OnGridViewInit(s, e) {
	UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
	UpdateTitlePanel();
	s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
	var selectedRows = [];
	for (var i = 0; i < values.length; i++) {
		selectedRows.push(values[i]);
	}
}

function OnGridViewEndCallback() {
	UpdateTitlePanel();
}

function UpdateTitlePanel() {
	//console.log("Estoy en el UpdateTitlePanel del Index");
	var selectedFilteredRowCount = GetSelectedFilteredRowCount();

	var text = "Total de elementos seleccionados: <b>" + gvInvoiceCommercials.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvInvoiceCommercials.GetSelectedRowCount() - GetSelectedFilteredRowCount();

	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";
	$("#lblInfo").html(text);

	//if ($("#selectAllMode").val() != "AllPages") {
	SetElementVisibility("lnkSelectAllRows", gvInvoiceCommercials.GetSelectedRowCount() > 0 && gvInvoiceCommercials.cpVisibleRowCount > selectedFilteredRowCount);
	SetElementVisibility("lnkClearSelection", gvInvoiceCommercials.GetSelectedRowCount() > 0);
	//}
	btnNew.SetEnabled(true);
	btnCopy.SetEnabled(false);
	btnApprove.SetEnabled(false);
	btnAutorize.SetEnabled(false);
	btnProtect.SetEnabled(false);
	btnCancel.SetEnabled(false);
	btnRevert.SetEnabled(false);
	btnHistory.SetEnabled(false);
	btnPrint.SetEnabled(false);
}

function GetSelectedFilteredRowCount() {
	return gvInvoiceCommercials.cpFilteredRowCountWithoutPage + gvInvoiceCommercials.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
	gvInvoiceCommercials.UnselectRows();
}

function gvResultsSelectAllRows() {
	gvInvoiceCommercials.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {
	gvInvoiceCommercials.GetSelectedFieldValues("id", function (values) {
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
				console.log(result);
			},
			complete: function () {
				gvInvoiceCommercials.PerformCallback();
			}
		});
	});
}

//btnNew
function AddNewDocument(s, e) {
	AddNewInvoiceComercialManual(s, e);
}

//btnCopy
function CopyDocument(s, e) {
}

//btnApprove
function ApproveDocuments(s, e) {
}

//btnAutorize
function AutorizeDocuments(s, e) {
}

//btnProtect
function ProtectDocuments(s, e) {
}

//btnCancel
function CancelDocuments(s, e) {
}

//btnRevert
function RevertDocuments(s, e) {
}

//btnHistory
function ShowHistory(s, e) {
}

//btnPrint
function Print(s, e) {
}
function gvSalesQuotationBeginCallback(s, e) {
	e.customArgs["isCallback"] = true;
}

function OnClickUpdateInvoiceCommercial(s, e) {
	var data = {
		id: gvInvoiceCommercials.GetRowKey(e.visibleIndex)
	};

	showPage("InvoiceCommercial/InvoiceCommercialFormEditPartial", data);
}

function ChangeState(trx) {
}

// DETAILS VIEW CALLBACKS

function InvoiceCommercialDetails_BeginCallback(s, e) {
	e.customArgs["id_invoiceCommercial"] = s.cpIdInvoiceCommercial;
}

function UpdateTotalValues() {
	var ovalueDiscount = ASPxClientControl.GetControlCollection().GetByName("valueDiscount");
	var oValueTotalFreight = ASPxClientControl.GetControlCollection().GetByName("valueTotalFreight");
	var oTotalValue = ASPxClientControl.GetControlCollection().GetByName("totalValue");
	var totalDetail = gvInvoiceCommercialEditFormDetail.cpTotalDetalleFacturaComercial;

	totalDetail = (totalDetail == null) ? 0 : totalDetail;
	var totalFreight = (oValueTotalFreight == null) ? 0 : oValueTotalFreight.GetValue();

	var subtotal = totalDetail;
	if (ovalueDiscount != null) {
		subtotal = (totalDetail - ovalueDiscount.GetValue());
	}

	if (oTotalValue != null) {
		var _total = (subtotal + totalFreight);
		oTotalValue.SetValue(_total);
	}
}

function InvoiceCommercialsDetail_OnEndCallback(s, e) {
	if (s.editMode == 1 && s.editState == 0) {
		UpdateTotalValues();
	}

	/* Modo Edicion, Estado Modificado */
	if (s.editMode == 1 && s.editState == 1) {
	}

	if (s.editMode == 1 && s.editState == 2) {
	}
}
function InvoiceCommercialsDetail_OnBeginCallback(s, e) {
	//
	e.customArgs["id_invoiceCommercial"] = s.cpIdInvoiceCommercial;
}

// Init
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

$("#id_invoiceCommercial").val();

function BankTransfer_SelectedIndexChanged(s, e) {
	////
	var idBankTransfer = s.GetValue();
	var data = { id_BankTransfer: idBankTransfer };
	var route = 'InvoiceCommercial/GetInfoBank';
	genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {
		////
		var oInfoBankTransfer = ASPxClientControl.GetControlCollection().GetByName("infoBankTransfer");
		if (result != null && result.codeReturn == -1) {
			console.log(result.msgerr);
			oInfoBankTransfer.GetValue("");
			$("[name=infoBankTransfer]").attr('rows', 1);
			return;
		}

		oInfoBankTransfer.SetValue(result.ValueDataList[0].valueObject);
		$("[name=infoBankTransfer]").attr('rows', 10);
	}, null)
}
function DateEditdateShipment_Validation(s, e) {
	if (s.GetValue() == null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var editdateShipmentAux = s.GetDate();
		var yeardateEditDate = editdateShipmentAux.getFullYear();
		var monthdateEditDate = editdateShipmentAux.getMonth();
		var daydateEditDate = editdateShipmentAux.getDate();
		var datedateEditDateAux = new Date(yeardateEditDate, monthdateEditDate, daydateEditDate);

		var _etaDate = etaDate.GetValue();
		if (_etaDate != null) {
			var yearEtaDate = _etaDate.getFullYear();
			var monthEtaDate = _etaDate.getMonth();
			var dayEtaDate = _etaDate.getDate();
			var EtaDateAux = new Date(yearEtaDate, monthEtaDate, dayEtaDate);

			if (EtaDateAux < datedateEditDateAux) {
				e.isValid = false;
				e.errorText = "La Fecha de Embarque debe ser menor a la fecha de Arribo";
			}
		} else {
			etaDate.Validate();
		}
	}
}

function DateEtaDate_Validation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var etaDateAux = s.GetDate();
		var yeardateEditDate = etaDateAux.getFullYear();
		var monthdateEditDate = etaDateAux.getMonth();
		var daydateEditDate = etaDateAux.getDate();
		var datedateEditDateAux = new Date(yeardateEditDate, monthdateEditDate, daydateEditDate);

		var _dateShipment = dateShipment.GetValue();
		if (_dateShipment != null) {
			var yearDateShipment = _dateShipment.getFullYear();
			var monthDateShipment = _dateShipment.getMonth();
			var dayDateShipment = _dateShipment.getDate();
			var DateShipmentAux = new Date(yearDateShipment, monthDateShipment, dayDateShipment);

			if (DateShipmentAux > datedateEditDateAux) {
				e.isValid = false;
				e.errorText = "La Fecha de Embarque debe ser menor a la fecha de Arribo";
			}
		} else {
			_dateShipment.Validate();
		}
	}
}

function TermsNegotiation_SelectedIndexChanged(s, e) {
	////
	var enabledTotalFreight = false;
	var oTermsNegotiation = s; //ASPxClientControl.GetControlCollection().GetByName("id_PaymentMethod");
	var oTotalFreight = ASPxClientControl.GetControlCollection().GetByName("valueTotalFreight");
	if (typeof oTermsNegotiation != 'undefined' && oTermsNegotiation != null && oTermsNegotiation.GetSelectedItem() != null) {
		var codeoTermsNegotiation = oTermsNegotiation.GetSelectedItem().GetColumnText("code");
		if (typeof codeoTermsNegotiation != 'undefined' && codeoTermsNegotiation != null) {
			if (codeoTermsNegotiation == 'FOBFLET') {
				enabledTotalFreight = true;
			}
		}
	}
	if (enabledTotalFreight == false) {
		oTotalFreight.SetValue(0.00);
	}
	oTotalFreight.SetEnabled(enabledTotalFreight);
}

function InvoiceCommercialPaymentMethod_SelectedIndexChanged(s, e) {
	//
	// Habilitar/Deshabilitar Combobox
	var enabledBankTransfer = false;
	var oPaymentMethod = s; //ASPxClientControl.GetControlCollection().GetByName("id_PaymentMethod");
	var oBankTransfer = ASPxClientControl.GetControlCollection().GetByName("id_BankTransfer");
	var oInfoBankTransfer = ASPxClientControl.GetControlCollection().GetByName("infoBankTransfer");
	if (typeof oPaymentMethod != 'undefined' && oPaymentMethod != null && oPaymentMethod.GetSelectedItem() != null) {
		var codePaymentMethod = oPaymentMethod.GetSelectedItem().GetColumnText("code");
		if (typeof codePaymentMethod != 'undefined' && codePaymentMethod != null) {
			if (codePaymentMethod == 'TR' || codePaymentMethod == 'CB') {
				enabledBankTransfer = true;
			}
		}
	}

	if (enabledBankTransfer == false) {
		oBankTransfer.SetValue(null);
		oInfoBankTransfer.SetValue("");
		$("[name=infoBankTransfer]").attr('rows', 1);
	}
	oBankTransfer.SetEnabled(enabledBankTransfer);

	id_PaymentTerm.ClearItems();
	id_PaymentTerm.SetValue(null);
	var route = 'InvoiceCommercial/SetPaymentMethod'
	var data =
	{
		"id_invoice": $("#id_invoiceCommercial").val(),
		"id_paymentMethod": ((s.GetSelectedItem() == null) ? null : s.GetSelectedItem().value)
	};

	genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {
		if (result != null && result.codeReturn == -1) {
			console.log(result.msgerr);
			return;
		}

		id_PaymentTerm.PerformCallback();
	}, null)
}
function OnshippingLine_EndCallback(s, e) {
	if (s.GetItemCount() == 0) return

	s.SetSelectedIndex(0);
}

function InvoiceCommercialShippingAgency_SelectedIndexChanged(s, e) {
	id_shippingLine.ClearItems();
	id_shippingLine.SetValue(null);
	var route = 'InvoiceCommercial/SetShippingAgency'
	var data = { "id_invoice": $("#id_invoice").val(), "id_shippingAgency": ((s.GetSelectedItem() == null) ? null : s.GetSelectedItem().value) };
	genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {
		if (result != null && result.error) {
			console.log(result.msgerr);
			return;
		}
		id_shippingLine.PerformCallback();
	}, null)
}

var selectedInvoiceExteriorRows = [];

function GenerateInvoiceCommercial(s, e) {
	gridMessageErrorInvoiceExterior.SetText("");
	$("#GridMessageErrorInvoiceExterior").hide();

	showLoading();

	gvInvoiceExterior.GetSelectedFieldValues("id", function (values) {
		var selectedInvoiceExteriorRows = [];

		for (var i = 0; i < values.length; i++) {
			selectedInvoiceExteriorRows.push(values[i]);
		}
		var data = {
			id: 0,
			id_InvoiceExterior: selectedInvoiceExteriorRows[0]
		};

		showPage("InvoiceCommercial/InvoiceCommercialFormEditPartial", data);
	});
}

function GenerateNewFromSalesQuotation(s, e) {
	gvSalesQuotation.GetSelectedFieldValues("id", function (values) {
		if (values.length > 0) {
			var data = {
				id: values[0]
			};
			showPage("InvoiceCommercial/FormEditInvoiceExteriorFromSalesQuotation", data);
		}
		else {
		}
	});
}

function GetSelectedFilteredOrderDetailsRowCount() {
	return gvInvoiceExterior.cpFilteredRowCountWithoutPage + gvInvoiceExterior.GetSelectedKeysOnPage().length;
}

function GetSelectedFieldDetailValuesCallback(values) {
	selectedInvoiceExteriorRows = [];
	for (var i = 0; i < values.length; i++) {
		selectedInvoiceExteriorRows.push(values[i]);
	}
}

function OnGridViewInvoiceCommercialDetailsInit() {
	UpdateTitlePanelOrderDetails();
}

function OnGridViewInvoiceCommercialDetailsSelectionChanged(s, e) {
	UpdateTitlePanelOrderDetails();
	s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function OnGridViewInvoiceCommercialDetailsEndCallback() {
	UpdateTitlePanelOrderDetails();
}

function UpdateTitlePanelOrderDetails() {
	var selectedFilteredRowCount = GetSelectedFilteredOrderDetailsRowCount();

	var text = "Total de elementos seleccionados: <b>" + gvInvoiceExterior.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvInvoiceExterior.GetSelectedRowCount() - GetSelectedFilteredOrderDetailsRowCount();
	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";
	$("#lblInfo").html(text);

	SetElementVisibility("lnkSelectAllRows", gvInvoiceExterior.GetSelectedRowCount() > 0 && gvInvoiceExterior.cpVisibleRowCount > selectedFilteredRowCount);
	SetElementVisibility("lnkClearSelection", gvInvoiceExterior.GetSelectedRowCount() > 0);

	btnGenerateInvoiceCommercial.SetEnabled(gvInvoiceExterior.GetSelectedRowCount() > 0);
}

var globalIdMetricUnitInvoice = 0;
var globalDinamycIdMetricUnitInvoice = 0;

function validationInvoiceCommercialMetricUnitInvoice(s, e) {
	if (s.GetValue() == null) {
		s.SetValue(globalIdMetricUnitInvoice);
	}
	else {
		globalIdMetricUnitInvoice = s.GetValue();
	}
}

function onInitInvoiceCommercialMetricUnitInvoice(s, e) {
	globalIdMetricUnitInvoice = s.GetValue();
	globalDinamycIdMetricUnitInvoice = s.GetValue();
}

function onChangeInvoiceCommercialMetricUnitInvoice(s, e) {
	if (globalDinamycIdMetricUnitInvoice != s.GetValue()) {
		afectarTotal();
		globalDinamycIdMetricUnitInvoice = s.GetValue();
	}
}

function afectarTotal() {
	var _data = { id_MetricUnitInvoice: id_metricUnitInvoice.GetValue() };
	genericPerformDocumentActionWithData("gvInvoiceCommercialEditFormDetail", "InvoiceCommercial/ChangeMetricUnitInvoiceMaster", _data);
}