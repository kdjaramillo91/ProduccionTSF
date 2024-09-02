function AddNewDocument(s, e) {
	AddNewSalesQuotationExteriorManual(s, e);
}

function SaveDocument(s, e) {
	ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
	ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
	var habilitarParam = $("#parmHabilita").val();
	if (habilitarParam == 'True') {
		DuplicateConfirmDialog.Show();
	}
	else {
		showPage("SalesQuotationExterior/InvoiceCopy", { id: $("#id_invoice").val() });
	}
}

function DevInvoiceDetail_foreignName_Init(s, e) {
	//debugger;
	var foreignName = s;
	foreignName.inputElement.tabIndex = -1;
	var _selectItemAux = id_item.GetSelectedItem();
	if (_selectItemAux !== undefined && _selectItemAux !== null) {
		foreignName.SetText(_selectItemAux.GetColumnText("foreignName"));
	}
}

function onConfirmDuplicateClick(s, e) {
	if (ASPxClientEdit.ValidateEditorsInContainerById("duplicateDialogForm", "", true)) {
		var data = {
			id: $("#id_invoice").val(),
			iteracion: SpinDuplicate.GetValue()
		};
		showPage("SalesQuotationExterior/InvoiceDuplicate", data);
	}
}

function onConfirmCancelClick(s, e) {
	SpinDuplicate.SetValue == null;
	DuplicateConfirmDialog.Hide();
}

function SpinValidate(s, e) {
	var value = s.GetValue();
	if (value == null || value <= 0) {
		e.isValid = false;
		e.errorText = "Valor inválido";
	}
}

function ApproveDocument(s, e) {
	showConfirmationDialog(function () {
		Update(true, false);
	}, "¿Desea aprobar la presente Proforma?");
}

function ProtectDocument(s, e) { }

function ReverseDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_invoice").val()
		};
		showForm("SalesQuotationExterior/Reverse", data);
	}, "¿Desea reversar la Proforma?");
}

function AnnulDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_invoice").val()
		};
		showForm("SalesQuotationExterior/Annul", data);
	}, "¿Desea anular la Proforma?");
}

function CloseDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_invoice").val()
		};
		showForm("SalesQuotationExterior/Close", data);
	}, "¿Desea cerrar la Proforma?");
}

function ButtonUpdate_Click(s, e) {
	Update(false, false);
}

function ButtonUpdateClose_Click(s, e) {
	Update(false, close);
}

function ButtonCancel_Click(s, e) {
	showPage("SalesQuotationExterior/Index", null);
}

function OnSalesQuotationExteriorRemissionGuideCodeValidation(s, e) {
	if (e.value !== null && e.value.length !== 17) {
		e.isValid = false;
		e.errorText = "Longitud de 17 digitos requerida";
	}
}

function Update(approve, autorize, close) {
	var valid = true;
	valid = ASPxClientEdit.ValidateEditorsInContainerById("mainTabSalesQuotationExterior", null, true);

	if (valid) {
		var serializePaymentTermDetail = JSON.stringify(gvSalesQuotationExteriorPaymentTermDetailView.cpCurrentPaymentTermDetails);
		console.log("serializacion: " + serializePaymentTermDetail);

		var id = $("#id_invoice").val();
		var data = "id=" + id + "&approve=" + approve + "&autorize=" + autorize +
			"&jsonPaymentTermDetails=" + encodeURIComponent(serializePaymentTermDetail) +
			"&" + $("#formEditSalesQuotationExterior").serialize();
		var url = (id === "0")
			? "SalesQuotationExterior/SalesQuotationExteriorPartialAddNew"
			: "SalesQuotationExterior/SalesQuotationExteriorPartialUpdate";
		if (close !== undefined) {
			if (close != null && close) {
				genericAjaxCall(url, true, data, null, showLoading(), null, function () {
					hideLoading();
					showPage("SalesQuotationExterior/Index", null);
				})
				return;
			}
		}
		var id = $("#id_invoice").val();
		var data = "id=" + id + "&approve=" + approve + "&autorize=" + autorize +
			"&jsonPaymentTermDetails=" + encodeURIComponent(serializePaymentTermDetail) +
			"&" + $("#formEditSalesQuotationExterior").serialize();

		var url = (id === "0")
			? "SalesQuotationExterior/SalesQuotationExteriorPartialAddNew"
			: "SalesQuotationExterior/SalesQuotationExteriorPartialUpdate";
		if (close !== undefined) {
			if (close != null && close) {
				genericAjaxCall(url, true, data, null, showLoading(), null, function () {
					hideLoading();
					showPage("SalesQuotationExterior/Index", null);
				})
				return;
			}
		}

		var callBackAction = function () {
			btnNew.SetEnabled(true);
			setTimeout(function () { $("#successMessage").hide() }, 3000);
		};

		showForm(url, data, callBackAction);
	}
}
var id_consigneeCurrent = null;
function OnChangeBuyer(s, e) {
	var route = "Person/getBuyerData";

	if (s.GetSelectedItem() == null) {
		fullname_businessName.SetText(null);
		identification_number.SetText(null);
		id_addressCustomer.SetValue(null);
		email.SetText(null);
		emailInterno.SetText(null);
		phone1FC.SetText(null);
		fax1FC.SetText(null);
		return;
	}

	var id_personBuyer = s.GetSelectedItem().value;
	var data = "id_person=" + id_personBuyer;
	id_consigneeCurrent = id_personBuyer;

	id_buyer.SetValue(id_personBuyer);
	id_notifier.SetValue(id_personBuyer);
	SalesQuotationExteriorAddressCustomerUpdate(id_personBuyer)

	genericAjaxCall(route,
		true,
		data,
		null,
		null,
		function (result) {
			if (result === undefined) return;
			if (result == null) return;
			try {
				fullname_businessName.SetText(result.fullname_businessName);
				identification_number.SetText(result.identification_number);
				email.SetText(result.email);
				emailInterno.SetText(result.emailInterno);
				phone1FC.SetText(result.phone1FC);
				fax1FC.SetText(result.fax1FC);
			} catch (err) {
				console.log(err);
			}
		}, null);
}

function OnChangeCustomerExterior(s, e) {
}

function SalesQuotationExteriorPaymentMethod_SelectedIndexChanged(s, e) {
	id_PaymentTerm.ClearItems();
	id_PaymentTerm.SetValue(null);
	var route = 'SalesQuotationExterior/SetPaymentMethod'
	var data = { "id_invoice": $("#id_invoice").val(), "id_paymentMethod": ((s.GetSelectedItem() == null) ? null : s.GetSelectedItem().value) };
	genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {
		if (result != null && result.error) {
			console.log(result.msgerr);
			return;
		}
		id_PaymentTerm.PerformCallback();
	}, null)
}

function SalesQuotationExteriorAddressCustomerUpdate(id_personBuyerAux) {
	id_addressCustomer.ClearItems();
	id_addressCustomer.SetValue(null);

	$.ajax({
		url: "SalesQuotationExterior/SetAddressCustomer",
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

				var value = result.length === 1 ? result[0].name : null;
				id_addressCustomer.SetText(value);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function SalesQuotationExteriorShippingAgency_SelectedIndexChanged(s, e) {
	id_ShippingLine.ClearItems();
	id_ShippingLine.SetValue(null);
	var route = 'SalesQuotationExterior/SetShippingAgency'
	var data = { "id_invoice": $("#id_invoice").val(), "id_shippingAgency": ((s.GetSelectedItem() == null) ? null : s.GetSelectedItem().value) };
	genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {
		if (result != null && result.error) {
			console.log(result.msgerr);
			return;
		}
		id_ShippingLine.PerformCallback();
	}, null)
}

function OnshippingLine_EndCallback(s, e) {
	if (s.GetItemCount() == 0) return
	s.SetSelectedIndex(0);
}
function AddressCustomer_BeginCallback(s, e) {
	e.customArgs['id_consignee'] = id_consignee.GetValue();
}
function AddressCustomer_EndCallback(s, e) {
	id_addressCustomer.SetValue(id_consigneeCurrent);
	SalesQuotationExteriorAddressCustomerUpdate(id_consigneeCurrent);
}

function SalesQuotationExteriorTermsNegotiation_SelectedIndexChanged(s, e) {
	if (s == null) s = id_termsNegotiation;
	if (s.GetSelectedItem() == null) {
		valueTransportationExpenses.SetValue(0);
		valueCustomsExpenditures.SetValue(0);
		valueInternationalInsurance.SetValue(0);
		valueInternationalFreight.SetValue(0);

		$("input[name='valueInternationalFreight']").prop("disabled", true);
		$("input[name='valueTransportationExpenses']").prop("disabled", true);
		$("input[name='valueCustomsExpenditures']").prop("disabled", true);
		$("input[name='valueInternationalInsurance']").prop("disabled", true);

		return;
	}

	var _TermsNegotiationCode = s.GetSelectedItem().GetColumnText("code");

	switch (_TermsNegotiationCode) {
		case "FOB":
			valueInternationalInsurance.SetValue(0);

			$("input[name='valueInternationalInsurance']").prop("disabled", true);
			$("input[name='valueInternationalFreight']").prop("disabled", true);
			break;
		case "CIF":
			$("input[name='valueInternationalInsurance']").prop("disabled", false);
			$("input[name='valueInternationalFreight']").prop("disabled", false);
			break;
		case "CFR":
			valueInternationalInsurance.SetValue(0);
			$("input[name='valueInternationalInsurance']").prop("disabled", true);
			$("input[name='valueInternationalFreight']").prop("disabled", false);
			break;
	}

	$("input[name='valueTransportationExpenses']").prop("disabled", false);
	$("input[name='valueCustomsExpenditures']").prop("disabled", false);
}

function SalesQuotationExteriordateShipmentInit(s, e) {
	if (dateShipment.GetValue() == null) dateShipment.SetValue(new Date());
}

function AddNewDetail(s, e) {
	gvInvoiceDetail.AddNewRow();
	btnNew.SetEnabled(false);
}

function RemoveDetail(s, e) {
	gvInvoiceDetail.GetSelectedFieldValues("id_item", function (values) {
		var selectedRows = [];

		for (var i = 0; i < values.length; i++) {
			selectedRows.push(values[i]);
		}

		genericAjaxCall('SalesQuotationExterior/SalesQuotationExteriorDetailsDeleteSeleted', true, { ids: selectedRows }, function (error) { console.log(error) }, null, null, function () {
			gvInvoiceDetail.PerformCallback();
			reloadInvoiceTotal();
			validateExistMetricUnitInvoice(null, null);
		});
	});
}

function RefreshDetail(s, e) {
	var serializePaymentTermDetail = JSON.stringify(gvSalesQuotationExteriorPaymentTermDetailView.cpCurrentPaymentTermDetails);
	console.log("serializacion: " + serializePaymentTermDetail);

	var id = $("#id_invoice").val();
	var data = "id=" + id +
		"&jsonPaymentTermDetails=" + encodeURIComponent(serializePaymentTermDetail) +
		"&" + $("#formEditSalesQuotationExterior").serialize();

	$.ajax({
		url: "SalesQuotationExterior/UpdateDivTabSalesQuotationExterior",
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
			$("#divTabSalesQuotationExterior").html(result);
		},
		complete: function () {
			gvInvoiceDetail.UnselectRows();
			gvInvoiceDetail.PerformCallback();
			gvSalesQuotation.UnselectRows();
			gvSalesQuotation.PerformCallback();
			reloadInvoiceTotal();
		}
	});
}

function numBoxesValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = 'Valor Obligatorio';
	} else {
		if (e.value <= 0) {
			e.isValid = false;
			e.errorText = 'Valor debe ser mayor a 0';
		} else {
			if (typeof proformaUsedNumBoxes === "undefined" || proformaUsedNumBoxes.GetValue() === null) return;
			if (e.value < proformaUsedNumBoxes.GetValue()) {
				e.isValid = false;
				e.errorText = 'El número de Cartones debe ser mayor e igual a la cantidad Facturada.';
			}
		}
	}
}

function proformaWeightValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = 'Valor Obligatorio';
	} else {
		if (e.value <= 0) {
			e.isValid = false;
			e.errorText = 'Valor debe ser mayor a 0';
		} else {
			if (typeof netWeight === "undefined" || netWeight.GetValue() === null) return;
			if (e.value < netWeight.GetValue()) {
				e.isValid = false;
				e.errorText = 'El peso proforma debe ser mayor e igual al peso neto.';
			}
		}
	}
}

function proformaPorcVariationPlusMinusValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = 'Valor Obligatorio';
	} else {
		if (e.value < 0.00 || e.value > 300) {
			e.isValid = false;
			e.errorText = 'Valor debe estar entre 0 y 300';
		}
	}
}

function proformaPorcVariationPlusMinusInit(s, e) {
	if (s.GetValue() === null || s.GetValue() === 0) {
		s.SetValue(porcVariationPlusMinus.GetValue());
		updateAmounts();
	}
	if (id_item.GetValue() !== null) {
		var data = { id_item: id_item.GetValue() };

		$.ajax({
			url: 'SalesQuotationExterior/ProductOnInvoice',
			data: data,
			async: true,
			cache: false,
			type: 'POST',
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				try {
					if (result !== undefined) {
						unitPrice.SetEnabled(result.itsProductOnInvoice);
						id_item.SetEnabled(result.itsProductOnInvoice);
						discount.SetEnabled(result.itsProductOnInvoice);
						proformaWeight.SetEnabled(result.itsProductOnInvoice);
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

function updateAmounts() {
	var proformaUsedNumBoxesAux = typeof proformaUsedNumBoxes !== "undefined" && proformaUsedNumBoxes.GetValue() !== null ? proformaUsedNumBoxes.GetValue() : 0.00;
	var proformaPorcVariationPlusMinusAux = typeof proformaPorcVariationPlusMinus !== "undefined" && proformaPorcVariationPlusMinus.GetValue() !== null ? proformaPorcVariationPlusMinus.GetValue() : 0.00;
	var numBoxesAux = typeof numBoxes !== "undefined" && numBoxes.GetValue() !== null ? numBoxes.GetValue() : 0;
	var proformaNumBoxesPlusMinusAuxFloat = numBoxesAux * proformaPorcVariationPlusMinusAux / 100;
	var proformaNumBoxesPlusMinusAuxInt = parseInt(proformaNumBoxesPlusMinusAuxFloat);
	var proformaNumBoxesPlusMinusAux = proformaNumBoxesPlusMinusAuxFloat > proformaNumBoxesPlusMinusAuxInt ? proformaNumBoxesPlusMinusAuxInt + 1 : proformaNumBoxesPlusMinusAuxInt;
	proformaPendingNumBoxes.SetValue(numBoxesAux + proformaNumBoxesPlusMinusAux - proformaUsedNumBoxesAux);
	proformaNumBoxesPlusMinus.SetValue(proformaNumBoxesPlusMinusAux);
	proformaUsedNumBoxes.SetValue(proformaUsedNumBoxesAux);
}

var id_itemIniAux = null;
var id_itemMarkedIniAux = null;

function ItemComboInit(s, e) {
	id_itemIniAux = s.GetValue();
}

function ItemMarkedComboInit(s, e) {
	id_itemMarkedIniAux = s.GetValue();
}

function ItemComboValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = 'Debe elegir un Item';
	} else {
		var data = {
			id_itemNew: s.GetValue()
		};
		if (data.id_itemNew !== id_itemIniAux) {
			$.ajax({
				url: "SalesQuotationExterior/ItsRepeatedItem",
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
					if (result !== null) {
						if (result.itsRepeated === 1) {
							e.isValid = false;
							e.errorText = "- Nombre del Producto: " + result.Error;
						}
					}
				},
				complete: function () {
				}
			});
		}
	}
}
function ItemMarkedComboValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = 'Debe elegir un Item';
	} else {
		var data = {
			id_itemNew: s.GetValue()
		};
		if (data.id_itemNew !== id_itemMarkedIniAux) {
			$.ajax({
				url: "SalesQuotationExterior/ItsRepeatedItemMarked",
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
					if (result !== null) {
						if (result.itsRepeated === 1) {
							e.isValid = false;
							e.errorText = "- Nombre del Producto: " + result.Error;
						}
					}
				},
				complete: function () {
				}
			});
		}
	}
}

function ItemCombo_SelectedIndexChanged(s, e) {
	var _selectItem = s.GetSelectedItem();
	if (_selectItem == undefined || _selectItem == null) {
		masterCode.SetText(null);
		auxCode.SetText(null);
		foreignName.SetText(null);
		netWeight.SetText(null);
		proformaWeight.SetText(null);
		numBoxes.Focus();
		return;
	}

	var _masterCode = _selectItem.GetColumnText("masterCode");
	var _auxCode = _selectItem.GetColumnText("auxCode");
	var _name = _selectItem.GetColumnText("name");
	var _foreignName = _selectItem.GetColumnText("foreignName");
	var _netWeight = _selectItem.texts[5];
	var _proformaWeight = _selectItem.texts[6];

	masterCode.SetText(_masterCode);
	auxCode.SetText(_auxCode);
	foreignName.SetText(_foreignName);

	var id_itemMarkedTemp = _selectItem.value;

	id_itemMarked.ClearItems();
	id_itemMarked.AddItem([_masterCode, _name, _foreignName], id_itemMarkedTemp)
	id_itemMarked.SetValue(id_itemMarkedTemp);

	var aDescriptionCustomer = descriptionCustomer.GetText();
	if (aDescriptionCustomer === "" || aDescriptionCustomer === null) {
		descriptionCustomer.SetText(_foreignName);
	}

	netWeight.SetText(_netWeight);
	proformaWeight.SetText(_proformaWeight);
	numBoxesValueChanged(null, null);
	genericDetailCalculate();

	var netWeightAux = typeof netWeight !== "undefined" && netWeight.GetValue() !== null ? netWeight.GetValue() : 0.00;
	var proformaWeightAux = typeof proformaWeight !== "undefined" && proformaWeight.GetValue() !== null ? proformaWeight.GetValue() : 0.00;
	glaseo.SetText(Math.round((((proformaWeightAux - netWeightAux) / proformaWeightAux) * 100) * 100) / 100);

	numBoxes.Focus();
}

function numBoxesValueChanged(a, e) {
	s = numBoxes.GetValue();
	updateAmounts();
	if (id_item.GetValue() == undefined || id_item.GetValue() == null) return;
	if (id_metricUnitInvoice.GetValue() == undefined || id_metricUnitInvoice.GetValue() == null) return;

	var NumeroCajas = 0;
	var pesoNeto = netWeight.GetValue();
	pesoNeto = pesoNeto !== null ? pesoNeto.toString().replace(".", ",") : pesoNeto;
	var pesoProforma = proformaWeight.GetValue();
	pesoProforma = pesoProforma !== null ? pesoProforma.toString().replace(".", ",") : pesoProforma;
	if (s == undefined || s == null) {
		NumeroCajas = numBoxes.GetValue();
	}
	else {
		NumeroCajas = s;
	}

	if (NumeroCajas == undefined || NumeroCajas == null) return;

	if (NumeroCajas == 0 || id_item.GetValue() == 0 || id_metricUnitInvoice.GetValue() == 0) {
		amountDisplay.SetText(0);
		amountInvoiceDisplay.SetText(0);

		totalPriceWithoutTax.SetValue(0);

		$("#amount").val(0);
		$("#id_amountInvoice").val(0);
		discount.SetValue(0);
		return;
	}
	var data = { "id_item": id_item.GetValue(), "numCajas": NumeroCajas, "id_metricUnitInvoice": id_metricUnitInvoice.GetValue(), "pesoProforma": pesoProforma, "pesoNeto": pesoNeto };
	genericAjaxCall('SalesQuotationExterior/CalculaCantidadCartones', true, data, function (error) { console.log(error) }, null,
		function (result) {
			if (result != null && result.error) {
				console.log(result.msgerr);
				return;
			}

			amountDisplay.SetValue(result.cantidadDisplay);
			amountInvoiceDisplay.SetValue(result.cantidadInvoiceDisplay);
			$("#amount").val(result.cantidadItem);
			$("#id_amountInvoice").val(result.cantidadFactura);
			unitPriceValueChanged(null, null);
		}, null);

	var netWeightAux = typeof netWeight !== "undefined" && netWeight.GetValue() !== null ? netWeight.GetValue() : 0.00;
	var proformaWeightAux = typeof proformaWeight !== "undefined" && proformaWeight.GetValue() !== null ? proformaWeight.GetValue() : 0.00;
	glaseo.SetText(Math.round((((proformaWeightAux - netWeightAux) / proformaWeightAux) * 100) * 100) / 100);
}

function unitPriceValueChanged(s, e) {
	totalPriceWithoutTax.SetValue(0);

	if (typeof $("#id_amountInvoice").val() === "undefined" || $("#id_amountInvoice").val() == null) return;
	if (typeof unitPrice.GetValue() === "undefined" || unitPrice.GetValue() == null) return;
	if (typeof discount.GetValue() === "undefined" || discount.GetValue() == null) return;

	var cantidad = $("#id_amountInvoice").val();
	var precioUnitario = unitPrice.GetValue();
	if (cantidad == 0 && precioUnitario == 0) {
		discount.SetValue(0);
	}
	else {
		var descuento = discount.GetValue();
	}

	var total = (cantidad == 0 || precioUnitario == 0) ? 0 : ((cantidad * precioUnitario) - descuento);

	total = parseFloat(total);

	totalPriceWithoutTax.SetValue(total);
}

function genericDetailCalculate() {
	numBoxesValueChanged(null, null);
	unitPriceValueChanged(null, null);
}

function discountValidation(s, e) {
	if (typeof $("#id_amountInvoice").val() === "undefined" || $("#id_amountInvoice").val() == null) return;
	if (typeof unitPrice.GetValue() === "undefined" || unitPrice.GetValue() == null) return;
	if (typeof discount.GetValue() === "undefined" || discount.GetValue() == null) return;

	var cantidad = $("#id_amountInvoice").val();
	var precioUnitario = unitPrice.GetValue();
	var _discount = discount.GetValue();

	if (_discount > (cantidad * precioUnitario)) {
		e.isValid = false;
		e.errorText = "Valor Descuento no puede ser mayor a Cantidad x Precio Unitario"
		e.value = 0;
	}
}

function OnValueSubscribed_Validate(s, e) {
	var originValue = s.inputValueBeforeFocus;
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Ingreso valor igual o superior a cero";
		e.value = originValue;
	}
};

function AddNewAttachedDocumentDetail(s, e) {
	gvAttachedDocuments.AddNewRow();
}

function RemoveAttachedDocumentDetail(s, e) {
}

function RefreshAttachedDocumentDetail(s, e) {
	gvAttachedDocuments.UnselectRows();
	gvAttachedDocuments.PerformCallback();
}

function AttachedUploadComplete(s, e) {
	var userData = JSON.parse(e.callbackData);
	$("#guid").val(userData.guid);
	$("#url").val(userData.url);
	attachmentName.SetText(userData.filename);
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
						url: "SalesQuotationExterior/ItsRepeatedAttachmentDetail",
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
							if (result !== null) {
								if (result.itsRepeated == 1) {
									e.isValid = false;
									e.errorText = result.Error;
								}
							}
						},
						complete: function () {
						}
					});
				}
			}
		}
	}
}

function SalesQuotationExterior_OnBeginCallback(s, e) {
	e.customArgs["id_invoice"] = s.cpIdSalesQuotationExterior;
}

function gvAttachedDocumentsCustomCommandButton_Click(s, e) {
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
			gvAttachedDocuments.UpdateEdit();
		}
	}
}

function UpdateView() {
	var id = parseInt($("#id_invoice").val());

	genericAjaxCall('SalesQuotationExterior/Actions',
		true,
		{ id: id },
		function (error) { console.log(error) },
		showLoading(),
		function (result) {
			if (result != null) {
				btnSave.SetEnabled(result.btnSave);
				btnApprove.SetEnabled(result.btnApprove);
				btnAnnul.SetEnabled(result.btnAnnul);
				btnRevert.SetEnabled(result.btnRevert);
				btnClose.SetEnabled(result.btnClose);
				btnPrint.SetEnabled(result.btnPrint);
				btnExportExcel.SetEnabled(result.btnExportExcel);
				btnCopy.SetEnabled(result.btnDuplicate);
			}
		},
		hideLoading());
}

function OValidationnIdTipoTemperatura(s, e) {
	if (e.value === null && temperatureInstruction.GetValue() != null) {
		e.isValid = false;
		e.errorText = 'Valor Obligatorio';
	}
}

function PrintDocument(s, e) {
	var idsInvoice = [];
	idsInvoice.push($("#id_invoice").val());
	PrintInvoiceGeneric(idsInvoice);
}

function PrintTemper(s, e) {
	var data = {
		id: $("#id_invoice").val(),
		codeReport: "PRTEM",
	};

	var url = "SalesQuotationExterior/PrintSalesQuotationExteriorISFTempReport";

	PrintItem(data, url);
}

function PrintSaleContract(s, e) {
	var data = {
		id: $("#id_invoice").val(),
		codeReport: "PRSACO",
	};

	var url = "SalesQuotationExterior/PrintSalesQuotationExteriorSaleContractReport";

	PrintItem(data, url);
}

function PrintItem(data, url) {
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
		success: function (result) {
			try {
				if (result !== undefined) {
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

function ExportExcel(s, e) {
	var data = { id_invoice: $("#id_invoice").val() };

	$.ajax({
		url: 'SalesQuotationExterior/SalesQuotationExteriorExporExcel',
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
					var url = 'ReportProd/DownloadExcelSalesQuotationExterior';
					newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0', false);

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

function validateItemContainer(s, e) {
	var numRows = gvInvoiceDetail.cpVisibleRowCount;

	if (numRows > 0 && (numeroContenedores.GetValue() == 0 || numeroContenedores.GetValue() == null)) {
		e.isValid = false;
	}
	else {
		e.isValid = true;
	}

	return e.isValid;
}

function validateCapacityContainer(s, e) {
	return !(numeroContenedores.GetValue() > 0 && s.GetSelectedItem() == null);
}

function validateShippingLine(s, e) {
	return !(id_shippingAgency.GetSelectedItem() != null && id_ShippingLine.GetSelectedItem() == null);
}

function validateShipNumberTrip(s, e) {
	return !(shipName.GetValue() != null && s.GetValue() == 0);
}

function OnDaeTextChanged(s, e) {
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

function PortDischarge_SelectedIndexChanged(s, e) {
	var obj = ASPxClientControl.GetControlCollection().GetByName("id_portDestination");
	if (typeof obj === 'undefined' || obj === null) {
		return;
	}

	if (s.GetValue() != null) {
		obj.SetValue(s.GetValue());
	}
}

var SalesQuotationExteriorPaymentTermDetailView_InitEdit = function (s, e) {
	emissionDate.DateChanged.AddHandler(OnInvoiceEmissionDateChanged);
};

var SalesQuotationExteriorPaymentTermDetailView_BeginCallback = function (s, e) {
	e.customArgs["idPaymentTerm"] = id_PaymentTerm.GetValue();
	e.customArgs["emissionDate"] = emissionDate.GetValue();
	e.customArgs["invoiceTotal"] = valuetotalCIF.GetValue();
	e.customArgs["canEditPaymentTerm"] = s.cpCanEditPaymentTerm;
	e.customArgs["cpCurrentPaymentTermDetails"] = gvSalesQuotationExteriorPaymentTermDetailView.cpCurrentPaymentTermDetails;
};

var OnFechaVencimientoDateChanged = function (s, e) {
	var details = gvSalesQuotationExteriorPaymentTermDetailView.cpCurrentPaymentTermDetails;
	var numDetails = details !== null ? details.length : 0;
	for (var i = 0; i < numDetails; i++) {
		var detail = details[i];
		if (detail.orderPayment === s.cpOrderPayment) {
			detail.dueDate = getISODateString(s.GetDate());
			break;
		}
	}
};

var OnFechaVencimientoValidate = function (s, e) {
	var value = s.GetValue();
	if (value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var getISODateString = function (date) {
	if (date !== null) {
		return date.getFullYear() + "-"
			+ (date.getMonth() < 9 ? "0" : "") + (date.getMonth() + 1) + "-"
			+ (date.getDate() < 10 ? "0" : "") + date.getDate();
	} else {
		return null;
	}
};

var OnIdPaymentTermSelectedIndexChanged = function (s, e) {
	gvSalesQuotationExteriorPaymentTermDetailView.PerformCallback();
};

function BankTransfer_SelectedIndexChanged(s, e) {
	infoBankTransfer.SetText(null);
	var idBankTransfer = s.GetValue();
	var data = { id_BankTransfer: idBankTransfer };
	var route = 'SalesQuotationExterior/GetInfoBank';
	genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {
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
var OnIdAddressCustomerSelectedIndexChanged = function (s, e) {
	var _selectItem = s.GetSelectedItem();
	var _emailInterno = _selectItem.GetColumnText("emailInterno");
	var _emailInternoCC = _selectItem.GetColumnText("emailInternoCC");
	var _phone1FC = _selectItem.GetColumnText("phone1FC");
	var _fax1FC = _selectItem.GetColumnText("fax1FC");

	email.SetText(_emailInterno);
	emailInterno.SetText(_emailInternoCC);
	phone1FC.SetText(_phone1FC);
	fax1FC.SetText(_fax1FC);
};

var OnInvoiceEmissionDateChanged = function (s, e) {
	gvSalesQuotationExteriorPaymentTermDetailView.PerformCallback();
};

var OnInvoiceTotalValueReady = function (s, e) {
	gvSalesQuotationExteriorPaymentTermDetailView.PerformCallback();
};

var OnInitCommissionAgent = function (s, e) {
};

function init() {
}

var InventoryLinesComboBox_SelectedIndexChanged = function (s, e) {
	id_itemType.ClearItems();
	id_itemTypeCategory.ClearItems();

	var idLinea = id_inventoryLine.GetValue();
	var numTipos = id_itemType.cpTipos ? id_itemType.cpTipos.length : 0;
	for (var i = 0; i < numTipos; i++) {
		var tipo = id_itemType.cpTipos[i];
		if (tipo.id_inventoryLine === idLinea) {
			id_itemType.AddItem(tipo.name, tipo.id);
		}
	}
};

var ItemTypesComboBox_SelectedIndexChanged = function (s, e) {
	id_itemTypeCategory.ClearItems();

	var idTipo = id_itemType.GetValue();
	var numCategorias = id_itemTypeCategory.cpCategorias ? id_itemTypeCategory.cpCategorias.length : 0;
	for (var i = 0; i < numCategorias; i++) {
		var categoria = id_itemTypeCategory.cpCategorias[i];
		if (categoria.id_itemType === idTipo) {
			id_itemTypeCategory.AddItem(categoria.name, categoria.id);
		}
	}
};

var ItemGroupsComboBox_SelectedIndexChanged = function (s, e) {
	id_subgroup.ClearItems();

	var idGrupo = id_group.GetValue();
	var numSubgrupos = id_subgroup.cpSubgrupos ? id_subgroup.cpSubgrupos.length : 0;
	for (var i = 0; i < numSubgrupos; i++) {
		var subgrupo = id_subgroup.cpSubgrupos[i];
		if (subgrupo.id_parentGroup === idGrupo) {
			id_subgroup.AddItem(subgrupo.name, subgrupo.id);
		}
	}
};

$(function () {
	var chkReadyState = setInterval(function () {
		if (document.readyState === "complete") {
			clearInterval(chkReadyState);
			UpdateView();
		}
	}, 100);

	init();
});