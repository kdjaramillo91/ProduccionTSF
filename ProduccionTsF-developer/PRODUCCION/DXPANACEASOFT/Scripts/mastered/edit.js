var id_salesInit = null;
var id_salesCurrent = null;
var id_productLotMPInit = null;
var id_productLotMPCurrent = null;
var id_productPTInit = null;
var id_productPTCurrent = null;
var id_customerInit = null;
var id_customerCurrent = null;

function OnItemDetailSalesInit(s, e) {
	id_salesInit = s.GetValue();
	id_salesCurrent = id_salesInit;
	id_productLotMPInit = id_productLotMP.GetValue();
	id_productLotMPCurrent = id_productLotMPInit;
	id_productPTInit = id_productPT.GetValue();
	id_productPTCurrent = id_productPTInit;
	id_customerInit = id_customer.GetValue();

	s.PerformCallback();
}

function DetailSales_BeginCallback(s, e) {
	e.customArgs["id_sales"] = id_salesCurrent;
	e.customArgs["id_customer"] = id_customerInit;
}

function DetailSales_EndCallback(s, e) {
	s.SetValue(id_salesInit);
	id_productLotMP.PerformCallback();
}

function DetailSales_SelectedIndexChanged(s, e) {
	id_salesCurrent = s.GetValue();
	id_productLotMPCurrent = null;
	id_productPTInit = null;
	id_productPTCurrent = null;
	id_customerInit = null;

	codProductMP.SetText("");
	id_productLotMP.SetValue(null);
	loteMP.SetText("");
	saldoMP.SetValue(0.00);
	quantityMP.SetValue(0.00);

	codProductPT.SetText("");
	id_productPT.SetValue(null);
	quantityPT.SetValue(0.00);
	loteBoxes.SetText("");
	quantityBoxes.SetValue(0.00);

	id_productLotMP.PerformCallback();
}

function DetailItemMP_BeginCallback(s, e) {
	e.customArgs["id_productLotMP"] = id_productLotMPCurrent;
	e.customArgs["id_sales"] = id_sales.GetValue() === undefined ? null : id_sales.GetValue();
	e.customArgs["id_itemType"] = ComboBoxItemType.GetValue() === undefined ? null : ComboBoxItemType.GetValue();
	e.customArgs["id_size"] = ComboBoxSize.GetValue() === undefined ? null : ComboBoxSize.GetValue();
	e.customArgs["id_trademark"] = ComboBoxTrademark.GetValue() === undefined ? null : ComboBoxTrademark.GetValue();
	e.customArgs["id_presentationMP"] = ComboBoxPresentationMP.GetValue() === undefined ? null : ComboBoxPresentationMP.GetValue();
	e.customArgs["decProducto"] = TextBoxDecProducto.GetText() === undefined ? null : TextBoxDecProducto.GetText();
	e.customArgs["codigoProducto"] = TextBoxCodigoProducto.GetText() === undefined ? null : TextBoxCodigoProducto.GetText();
	e.customArgs["id_tokenBoxLotes"] = TokenBoxLotes.GetTokenValuesCollection() === undefined ? [] : TokenBoxLotes.GetTokenValuesCollection();
	e.customArgs["id_boxedWarehouse"] = ComboBoxBoxedWarehouse.GetValue() === undefined ? null : ComboBoxBoxedWarehouse.GetValue();
	e.customArgs["id_boxedWarehouseLocation"] = ComboBoxBoxedWarehouseLocation.GetValue() === undefined ? null : ComboBoxBoxedWarehouseLocation.GetValue();
	var emissionDt = DateTimeEmision.GetDate();
	e.customArgs["emissionDate"] = emissionDt != null
		? emissionDt.toISOString().substring(0, 10)
		: null;

	
}

function DetailItemMP_EndCallback(s, e) {
	if (id_productLotMPCurrent !== null) {
		s.SetValue(id_productLotMPCurrent);
	}
	id_productPT.PerformCallback();
}

function OnItemDetailItemMPValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo obligatorio";
	}
}

function DetailItemMP_SelectedIndexChanged(s, e) {
	
	id_productLotMPCurrent = s.GetValue();

	id_productPTInit = null;
	id_productPTCurrent = null;
	codProductMP.SetText("");
	loteMP.SetText("");
	saldoMP.SetValue(0.00);
	quantityMP.SetValue(0.00);

	codProductPT.SetText("");
	quantityPT.SetValue(0.00);
	loteBoxes.SetText("");

	if (GridViewDetails.cpIsParLotMarked) {
		lotMarked.SetText("");
	}
	quantityBoxes.SetValue(0.00);

	

	//	id_productPT.PerformCallback();

	//if (id_productLotMPCurrent === null) {
	//	id_productPTInit = null;
	//	id_productPTCurrent = null;
	//	codProductMP.SetText("");
	//	loteMP.SetText("");
	//	saldoMP.SetValue(0.00);
	//	quantityMP.SetValue(0.00);
	//
	//	codProductPT.SetText("");
	//	quantityPT.SetValue(0.00);
	//	loteBoxes.SetText("");
	//
	//	if (GridViewDetails.cpIsParLotMarked) {
	//		lotMarked.SetText("");
	//	}
	//	quantityBoxes.SetValue(0.00);
	//
	//	id_productPT.PerformCallback();
	//} else {
		$.ajax({
			url: "Mastered/ProductLotMPChanged",
			type: "post",
			data: {
				id_productLotMP: s.GetValue(),
				id_boxedWarehouse: ComboBoxBoxedWarehouse.GetValue() === undefined ? null : ComboBoxBoxedWarehouse.GetValue(),
				id_boxedWarehouseLocation: ComboBoxBoxedWarehouseLocation.GetValue() === undefined ? null : ComboBoxBoxedWarehouseLocation.GetValue()
			},
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result !== null) {
					if (result.message !== null && result.message !== "") {
						NotifyError("Error. " + result.message);
					} else {
						codProductMP.SetText(result.codProductMP);
						loteMP.SetText(result.loteMP);
						saldoMP.SetValue(result.saldoMP);
						quantityMP.SetValue(result.quantityMP);
						loteBoxes.SetText(result.loteMP);
						if (GridViewDetails.cpIsParLotMarked) {
							lotMarked.SetText(result.loteMP);
						}
						id_productPT.PerformCallback();
					}
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	//}
}

function OnQuantityMPValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo obligatorio";
	} else {
		var aSaldoMP = saldoMP.GetValue() != null ? parseFloat(saldoMP.GetValue()) : 0.00;
		if (e.value > aSaldoMP || 0 >= e.value) {
			e.isValid = false;
			e.errorText = e.value === 0.00 ? "Cantidad MP debe ser mayor o igual que cero" : "Cantidad MP debe ser menor o igual al Saldo";
		}
	}
}

function OnQuantityPTValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo obligatorio";
	} else {
		var aSaldoMP = saldoMP.GetValue() != null ? parseFloat(saldoMP.GetValue()) : 0.00;
		if ((e.value > aSaldoMP || 0 >= e.value) && quantityBoxes.GetValue() <= 0) {
			e.isValid = false;
			e.errorText = e.value === 0.00 ? "Cantidad PT debe ser mayor que cero" : "Cantidad PT debe ser menor o igual al Saldo";
		}
	}
}

function OnQuantityBoxesValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo obligatorio";
	} else {
		var aQuantityPT = quantityPT.GetValue() != null ? parseFloat(quantityPT.GetValue()) : 0.00;
		if (e.value == 0 && aQuantityPT == 0) {
			e.isValid = false;
			e.errorText = "Cantidad de Cajas debe ser mayor que cero";
		}
	}
}

function QuantityMPValueChanged(s, e) {
	$.ajax({
		url: "Mastered/QuantityMPValueChanged",
		type: "post",
		data: {
			id_sales: id_sales.GetValue(),
			id_productLotMP: id_productLotMP.GetValue(),
			quantityMP: s.GetValue(),
			id_productPT: id_productPT.GetValue()
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				} else {
					quantityPT.SetValue(result.quantityPT);
					quantityBoxes.SetValue(result.quantityBoxes);
				}
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function DetailItemPT_BeginCallback(s, e) {
	e.customArgs["id_productPT"] = id_productPTCurrent;
	e.customArgs["id_sales"] = id_sales.GetValue() === undefined ? null : id_sales.GetValue();
	e.customArgs["id_productLotMP"] = id_productLotMPCurrent;
	e.customArgs["id_presentationPT"] = ComboBoxPresentationPT.GetValue() === undefined ? null : ComboBoxPresentationPT.GetValue();
}

function DetailItemPT_EndCallback(s, e) {
	if (id_productPTInit !== null) {
		s.SetValue(id_productPTInit);
	}
	id_customer.PerformCallback();
	id_masteredWarehouseLocation.PerformCallback();
	id_warehouseLocationBoxes.PerformCallback();
}

function OnItemDetailItemPTValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo obligatorio";
	}
}

function DetailItemPT_SelectedIndexChanged(s, e) {
	var id_productPTCurrent = s.GetValue();
	if (id_productPTCurrent === null) {
		codProductPT.SetText("");
		quantityPT.SetValue(0.00);
		quantityBoxes.SetValue(quantityMP.GetValue());
	} else {
		$.ajax({
			url: "Mastered/ProductPTChanged",
			type: "post",
			data: {
				id_productPT: s.GetValue()
			},
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result !== null) {
					if (result.message !== null && result.message !== "") {
						NotifyError("Error. " + result.message);
					} else {
						codProductPT.SetText(result.codProductPT);
						QuantityMPValueChanged(quantityMP, quantityMP);
					}
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function DetailCustomer_BeginCallback(s, e) {
	var id_customerCurrent = s.GetValue() === undefined ? null : s.GetValue();
	e.customArgs["id_customerCurrent"] = id_customerCurrent === null ? id_customerInit : s.GetValue();
	e.customArgs["id_sales"] = id_sales.GetValue() === undefined ? null : id_sales.GetValue();
	e.customArgs["id_customer"] = ComboBoxCustomer.GetValue() === undefined ? null : ComboBoxCustomer.GetValue();
}

function DetailCustomer_EndCallback(s, e) {
	var id_salesAux = (id_sales.GetValue() === undefined ? null : id_sales.GetValue());
	if (id_customerInit !== null) {
		s.SetValue(id_customerInit);
		if (id_salesAux !== null) {
			s.SetEnabled(false);
		} else {
			s.SetEnabled(true);
		}
	} else {
		if (id_salesAux !== null) {
			$.ajax({
				url: "Mastered/GetCustomerSales",
				type: "post",
				data: {
					id_sales: id_salesAux
				},
				async: true,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					showLoading();
				},
				success: function (result) {
					if (result !== null) {
						if (result.message !== null && result.message !== "") {
							NotifyError("Error. " + result.message);
						} else {
							s.SetValue(result.id_customerSale);
							s.SetEnabled(false);
						}
					}
				},
				complete: function () {
					hideLoading();
				}
			});
		} else {
			s.SetEnabled(true);
			s.SetValue(ComboBoxCustomer.GetValue() === undefined ? null : ComboBoxCustomer.GetValue());
		}
	}
}

function DetailWareHouseLocation_BeginCallback(s, e) {
	e.customArgs["id_responsable"] = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
	e.customArgs["id_masteredWarehouse"] = ComboBoxMasteredWarehouse.GetValue() === undefined ? null : ComboBoxMasteredWarehouse.GetValue();
}
function DetailWareHouseLocation_EndCallback(s, e) {
	s.SetValue(ComboBoxMasteredWarehouseLocation.GetValue() === undefined ? null : ComboBoxMasteredWarehouseLocation.GetValue());
}
function DetailWarehouseLocationBoxes_BeginCallback(s, e) {
	e.customArgs["id_responsable"] = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
	e.customArgs["id_warehouseBoxes"] = ComboBoxWarehouseBoxes.GetValue() === undefined ? null : ComboBoxWarehouseBoxes.GetValue();
}
function DetailWarehouseLocationBoxes_EndCallback(s, e) {
	s.SetValue(ComboBoxWarehouseLocationBoxes.GetValue() === undefined ? null : ComboBoxWarehouseLocationBoxes.GetValue());
}

function OnGridViewDetailEndCallback(s, e) {
	$.ajax({
		url: "Mastered/GetTotales",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function (error) {
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			$('#labelCantidadMP').text(result.amountMPStr);
			$('#labelLbsMP').text(result.lbsMPStr);
			$('#labelKgMP').text(result.kgMPStr);

			$('#labelCantidadPT').text(result.amountPTStr);
			$('#labelLbsPT').text(result.lbsPTStr);
			$('#labelKgPT').text(result.kgPTStr);

			$('#labelCantidadCajas').text(result.amountBoxesStr);
			$('#labelLbsCajas').text(result.lbsBoxesStr);
			$('#labelKgCajas').text(result.kgBoxesStr);
		},
		complete: function () {
			if (GridViewDetails.cpError !== null && GridViewDetails.cpError !== "") {
				NotifyError(GridViewDetails.cpError);
			}
			hideLoading();
		}
	});
}

function ComboBoxTurn_SelectedIndexChanged(s, e) {
	$.ajax({
		url: "Mastered/TurnChanged",
		type: "post",
		data: { id_turn: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				}
				$('#timeInitTurn').val(result.timeInitTurn);
				$('#timeEndTurn').val(result.timeEndTurn);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function ComboBoxResponsable_SelectedIndexChanged(s, e) {
	ComboBoxBoxedWarehouse.SetValue(null);
	ComboBoxBoxedWarehouseLocation.SetValue(null);
	ComboBoxMasteredWarehouse.SetValue(null);
	ComboBoxMasteredWarehouseLocation.SetValue(null);
	ComboBoxWarehouseBoxes.SetValue(null);
	ComboBoxWarehouseLocationBoxes.SetValue(null);

	ComboBoxBoxedWarehouse.PerformCallback();
	ComboBoxMasteredWarehouse.PerformCallback();
	ComboBoxWarehouseBoxes.PerformCallback();
}

function TokenBoxLotes_BeginCallback(s, e) {
	e.customArgs["id_boxedWarehouse"] = ComboBoxBoxedWarehouse.GetValue() === undefined ? null : ComboBoxBoxedWarehouse.GetValue();
	e.customArgs["id_boxedWarehouseLocation"] = ComboBoxBoxedWarehouseLocation.GetValue() === undefined ? null : ComboBoxBoxedWarehouseLocation.GetValue();
	var emissionDt = DateTimeEmision.GetDate();
	e.customArgs["emissionDate"] = emissionDt != null
		? emissionDt.toISOString().substring(0, 10)
		: null;
}

function ComboBoxBoxedWarehouse_BeginCallback(s, e) {
	e.customArgs["id_responsable"] = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
}

function ComboBoxBoxedWarehouse_EndCallback(s, e) {
	$.ajax({
		url: "Mastered/GetValueBoxedWarehouse",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function (error) {
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				} else {
					s.SetValue(result.id_boxedWarehouse);
					var aResponsable = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
					if ((result.id_boxedWarehouse == null || result.id_boxedWarehouse == "") && aResponsable != null) {
						NotifyError("No esta definida la Bodega de INGRESO DE ENCARTONADO en el Stakehokder. Por favor, configúrela y vuelve a intentarlo");
					}
				}
			}
		},
		complete: function () {
			hideLoading();
			ComboBoxBoxedWarehouseLocation.PerformCallback();
		}
	});
}

function ComboBoxBoxedWarehouse_SelectedIndexChanged(s, e) {
	ComboBoxBoxedWarehouseLocation.PerformCallback();
}
function ComboBoxBoxedWarehouseLocation_BeginCallback(s, e) {
	e.customArgs["id_responsable"] = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
	e.customArgs["id_boxedWarehouse"] = ComboBoxBoxedWarehouse.GetValue() === undefined ? null : ComboBoxBoxedWarehouse.GetValue();
}

function ComboBoxBoxedWarehouseLocation_EndCallback(s, e) {
	$.ajax({
		url: "Mastered/GetValueBoxedWarehouseLocation",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function (error) {
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				} else {
					s.SetValue(result.id_boxedWarehouseLocation);
				}
			}
		},
		complete: function () {
			hideLoading();
			TokenBoxLotes.PerformCallback();
			DeleteDetail(s, e);
		}
	});
}

function ComboBoxBoxedWarehouseLocation_SelectedIndexChanged(s, e) {
	DeleteDetail(s, e);
}

function ComboBoxMasteredWarehouse_BeginCallback(s, e) {
	e.customArgs["id_responsable"] = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
}

function ComboBoxMasteredWarehouse_EndCallback(s, e) {
	$.ajax({
		url: "Mastered/GetValueMasteredWarehouse",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function (error) {
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				} else {
					s.SetValue(result.id_masteredWarehouse);
				}
			}
		},
		complete: function () {
			hideLoading();
			ComboBoxMasteredWarehouseLocation.PerformCallback();
		}
	});
}

function ComboBoxMasteredWarehouse_SelectedIndexChanged(s, e) {
	ComboBoxMasteredWarehouseLocation.PerformCallback();
}
function ComboBoxMasteredWarehouseLocation_BeginCallback(s, e) {
	e.customArgs["id_responsable"] = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
	e.customArgs["id_masteredWarehouse"] = ComboBoxMasteredWarehouse.GetValue() === undefined ? null : ComboBoxMasteredWarehouse.GetValue();
}

function ComboBoxMasteredWarehouseLocation_EndCallback(s, e) {
	$.ajax({
		url: "Mastered/GetValueMasteredWarehouseLocation",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function (error) {
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				} else {
					s.SetValue(result.id_masteredWarehouseLocation);
				}
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function ComboBoxWarehouseBoxes_BeginCallback(s, e) {
	e.customArgs["id_responsable"] = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
}

function ComboBoxWarehouseBoxes_EndCallback(s, e) {
	$.ajax({
		url: "Mastered/GetValueWarehouseBoxes",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function (error) {
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				} else {
					s.SetValue(result.id_warehouseBoxes);
				}
			}
		},
		complete: function () {
			hideLoading();
			ComboBoxWarehouseLocationBoxes.PerformCallback();
		}
	});
}

function ComboBoxWarehouseBoxes_SelectedIndexChanged(s, e) {
	ComboBoxWarehouseLocationBoxes.PerformCallback();
}
function ComboBoxWarehouseLocationBoxes_BeginCallback(s, e) {
	e.customArgs["id_responsable"] = ComboBoxResponsable.GetValue() === undefined ? null : ComboBoxResponsable.GetValue();
	e.customArgs["id_warehouseBoxes"] = ComboBoxWarehouseBoxes.GetValue() === undefined ? null : ComboBoxWarehouseBoxes.GetValue();
}

function ComboBoxWarehouseLocationBoxes_EndCallback(s, e) {
	$.ajax({
		url: "Mastered/GetValueWarehouseLocationBoxes",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function (error) {
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				} else {
					s.SetValue(result.id_warehouseLocationBoxes);
				}
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function OnGridViewDetailBeginCallback(s, e) {
}

function DeleteDetail(s, e) {
	$.ajax({
		url: "Mastered/DeleteDetail",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function (error) {
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			GridViewDetails.PerformCallback();
		},
		complete: function () {
			if (GridViewDetails.cpError !== null && GridViewDetails.cpError !== "") {
				NotifyError(GridViewDetails.cpError);
			}
			hideLoading();
		}
	});
}

function AddNewDetail(s, e) {
	GridViewDetails.AddNewRow();
}

function RefreshDetail(s, e) {
	GridViewDetails.PerformCallback();
}

function ShowCurrentItem(enabled) {
	var data = {
		id: $('#id_mastered').val(),
		enabled: enabled
	};

	showPage("Mastered/Edit", data);
}

function AddNewItem() {
	var data = {
		id: 0,
		enabled: true
	};
	showPage("Mastered/Edit", data);
}

function EditCurrentItem() {
	showLoading();
	ShowCurrentItem(true);
}

function SaveCurrentItem() {
	SaveItem(false);
}

const DOCUMENT_CODE_MASTERIZADO = "145";

function AprovedItem() {
	showLoading();
	$.ajax({
		url: 'Mastered/Approve',
		type: 'post',
		data: { id: $('#id_mastered').val() },
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result)
		{
			if (result.Code == CODE_FOR_SCHEDULE_TRANSAC) {
				
				hideLoading();
				callbackProcessControlState(false);
				observerNotification(DOCUMENT_CODE_TUMBADA_PLACA, 5000, callbackProcess);
				NotifySuccess(TRANSAC_FOR_QUEUE_MSG);
			}
			else if (result.Code !== 0 && result.Code != CODE_FOR_SCHEDULE_TRANSAC) {
				hideLoading();
				NotifyError("Error." + result.Message);

			}
			else {
				hideLoading();
				NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
				ShowCurrentItem(false);

			}
			//if (result.Code !== 0) {
			//	hideLoading();
			//	NotifyError("Error al Aprobar. " + result.Message);
			//	return;
			//}
			//
			//ShowCurrentItem(false);
			//NotifySuccess("El Masterizado Aprobado Satisfactoriamente. " + "Estado: " + result.Data);
		}
	});
}

function callbackProcessControlState(isEnabled) {

	btnEdit.SetEnabled(isEnabled);
	btnAproved.SetEnabled(isEnabled);
	btnAnnul.SetEnabled(isEnabled);
	btnExit.SetEnabled(isEnabled);
}

function callbackProcess(status) {

	debugger;

	if (status == "APROBADA") {
		NotifySuccess("Proceso realizado Satisfactoriamente, estado: APROBADA");
		ShowCurrentItem(false);
	}
	else if (status == "PENDIENTE") {
		NotifyError("Ha ocurrido un error, revise las notificaciones");

		// habilitar botones
	}

	callbackProcessControlState(true);
}


function AutoriceItem() {
	showLoading();
	$.ajax({
		url: 'Mastered/Autorice',
		type: 'post',
		data: { id: $('#id_mastered').val() },
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error al Autorizar. " + result.Message);
				return;
			}

			ShowCurrentItem(false);
			NotifySuccess("El Masterizado Autorizado Satisfactoriamente. " + "Estado: " + result.Data);
		}
	});
}

function AprovedCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Masterizado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			if ($("#enabled").val() === "true") {
				SaveItem(true);
			} else {
				AprovedItem();
			}
		}
	});
}

function AutoriceCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Autorizar el Masterizado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			if ($("#enabled").val() === "true") {
				SaveItem(true);
			} else {
				AutoriceItem();
			}
		}
	});
}

function ReverseCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Reversar el Masterizado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'Mastered/Reverse',
				type: 'post',
				data: { id: $('#id_mastered').val() },
				async: true,
				cache: false,
				error: function (result) {
					hideLoading();
					NotifyError("Error. " + result.Message);
				},
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error al Reversar. " + result.Message);
						return;
					}

					ShowCurrentItem(false);
					NotifySuccess("El Masterizado Reversado Satisfactoriamente. " + "Estado: " + result.Data);
				}
			});
		}
	});
}

function AnnulCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Anular el Masterizado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'Mastered/Annul',
				type: 'post',
				data: { id: $('#id_mastered').val() },
				async: true,
				cache: false,
				error: function (result) {
					hideLoading();
					NotifyError("Error. " + result.Message);
				},
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error al Anular. " + result.Message);
						return;
					}

					ShowCurrentItem(false);
					NotifySuccess("El Masterizado Anulado Satisfactoriamente. " + "Estado: " + result.Data);
				}
			});
		}
	});
}

function ConciliateCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Conciliar el Masterizado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			ConciliateItem();
		}
	});
}

function ConciliateItem() {
	showLoading();
	$.ajax({
		url: 'Mastered/Conciliate',
		type: 'post',
		data: { id: $('#id_mastered').val() },
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error al Conciliar. " + result.Message);
				return;
			}

			ShowCurrentItem(false);
			//hideLoading();
			NotifySuccess("El Masterizado Conciliado Satisfactoriamente. " + "Estado: " + result.Data);
		}
	});
}


var selectedRows = [];

function GetSelectedFieldDetailValuesCallback(values) {
	selectedRows = values;
}

function GetIdsSelected(s, e) {
	GridViewDetails.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function SaveDataUser() {
	var emisionDate = DateTimeEmision.GetValue();
	var yearEmisionDate = emisionDate.getFullYear();
	var monthEmisionDate = emisionDate.getMonth();
	var dayEmisionDate = emisionDate.getDate();

	var dateTimeStartMasteredAux = dateTimeStartMastered.GetDate();
	var yearDateTimeStartMastered = dateTimeStartMasteredAux.getFullYear();
	var monthDateTimeStartMastered = dateTimeStartMasteredAux.getMonth();
	var dayDateTimeStartMastered = dateTimeStartMasteredAux.getDate();

	var dateTimeEndMasteredAux = dateTimeEndMastered.GetDate();
	var yearDateTimeEndMastered = dateTimeEndMasteredAux.getFullYear();
	var monthDateTimeEndMastered = dateTimeEndMasteredAux.getMonth();
	var dayDateTimeEndMastered = dateTimeEndMasteredAux.getDate();
	var userData = {
		id: $('#id_mastered').val(),
		id_boxedWarehouse: ComboBoxBoxedWarehouse.GetValue(),
		boxedWarehouse: ComboBoxBoxedWarehouse.GetText(),
		id_boxedWarehouseLocation: ComboBoxBoxedWarehouseLocation.GetValue(),
		boxedWarehouseLocation: ComboBoxBoxedWarehouseLocation.GetText(),

		id_masteredWarehouse: ComboBoxMasteredWarehouse.GetValue(),
		masteredWarehouse: ComboBoxMasteredWarehouse.GetText(),
		id_masteredWarehouseLocation: ComboBoxMasteredWarehouseLocation.GetValue(),
		masteredWarehouseLocation: ComboBoxMasteredWarehouseLocation.GetText(),

		id_warehouseBoxes: ComboBoxWarehouseBoxes.GetValue(),
		warehouseBoxes: ComboBoxWarehouseBoxes.GetText(),
		id_warehouseLocationBoxes: ComboBoxWarehouseLocationBoxes.GetValue(),
		warehouseLocationBoxes: ComboBoxWarehouseLocationBoxes.GetText(),

		dateTimeEmision: yearEmisionDate + "-" +
			(++monthEmisionDate).toString().padStart(2, 0) + "-" +
			dayEmisionDate.toString().padStart(2, 0) + "T00:00:00",
		description: MemoDescription.GetText(),
		dateTimeStartMastered: yearDateTimeStartMastered + "-" +
			(++monthDateTimeStartMastered).toString().padStart(2, 0) + "-" +
			dayDateTimeStartMastered.toString().padStart(2, 0) + "T" +
			dateTimeStartMasteredAux.getHours().toString().padStart(2, 0) + ":" +
			dateTimeStartMasteredAux.getMinutes().toString().padStart(2, 0) + ":00",
		dateTimeEndMastered: yearDateTimeEndMastered + "-" +
			(++monthDateTimeEndMastered).toString().padStart(2, 0) + "-" +
			dayDateTimeEndMastered.toString().padStart(2, 0) + "T" +
			dateTimeEndMasteredAux.getHours().toString().padStart(2, 0) + ":" +
			dateTimeEndMasteredAux.getMinutes().toString().padStart(2, 0) + ":00",
		id_responsable: ComboBoxResponsable.GetValue(),
		id_turn: ComboBoxTurn.GetValue()
	};

	var Mastered = {
		jsonMastered: JSON.stringify(userData)
	};

	return Mastered;
}

var aprovedGlobal = false;
function SaveItem_GetSelectedFieldDetailValuesCallback(values) {
	selectedRows = values;
	if (selectedRows.length < 1) {
		NotifyError("Debe seleccionar al menos un detalle. ");
		hideLoading();
		return;
	}
	$.ajax({
		url: 'Mastered/Save',
		type: 'post',
		data: SaveDataUser(),
		async: true,
		cache: false,
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error. " + result.Message);
				return;
			}

			var id = result.Data;
			$('#id_mastered').val(id);

			if (aprovedGlobal)
				AprovedItem();
			else
				ShowCurrentItem(true);

			NotifySuccess("El Masterizado Guardado Satisfactoriamente.");
		},
		error: function (result) {
			hideLoading();
		}
	});
}

function SaveItem(aproved) {
	showLoading();

	if (!Validate()) {
		hideLoading();
		return;
	}
	aprovedGlobal = aproved;
	$.ajax({
		url: 'Mastered/Save',
		type: 'post',
		data: SaveDataUser(),
		async: true,
		cache: false,
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error. " + result.Message);
				return;
			}

			var id = result.Data;
			$('#id_mastered').val(id);

			if (aprovedGlobal)
				AprovedItem();
			else
				ShowCurrentItem(true);

			NotifySuccess("El Masterizado Guardado Satisfactoriamente.");
		},
		error: function (result) {
			hideLoading();
		}
	});
}

function IsValid(object) {
	return object !== null && object !== undefined && object !== "undefined";
}

function Validate() {
	var validate = true;
	var errors = "";

	if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() === null) {
		errors += "Fecha Emisión es un campo Obligatorio. \n\r";
		validate = false;
	} else {
		var aDateTimeEmision = DateTimeEmision.GetValue();
		var yearDateTimeEmision = aDateTimeEmision.getFullYear();
		var monthDateTimeEmision = aDateTimeEmision.getMonth() + 1;
		var dayDateTimeEmision = aDateTimeEmision.getDate();
		var dateTimeEmisionAux = new Date(yearDateTimeEmision, aDateTimeEmision.getMonth(), dayDateTimeEmision);

		var dateHoy = $('#dateHoy').val().split("-");
		var yearHoyDate = parseInt(dateHoy[2]);
		var monthHoyDate = parseInt(dateHoy[1]);
		var dayHoyDate = parseInt(dateHoy[0]);
		var hoyDateAux = new Date(yearHoyDate, monthHoyDate - 1, dayHoyDate);

		var dateHoyMin = $('#dateHoyMin').val().split("-");
		var yearHoyMinDate = parseInt(dateHoyMin[2]);
		var monthHoyMinDate = parseInt(dateHoyMin[1]);
		var dayHoyMinDate = parseInt(dateHoyMin[0]);
		var hoyMinDateAux = new Date(yearHoyMinDate, monthHoyMinDate - 1, dayHoyMinDate);

		if (dateTimeEmisionAux.getTime() !== hoyDateAux.getTime() && dateTimeEmisionAux.getTime() !== hoyMinDateAux.getTime() &&
			(dateTimeEmisionAux < hoyMinDateAux || dateTimeEmisionAux > hoyDateAux)) {
			errors += "Fecha de emisión debe ser mayor o igual a fecha mínima y menor o igual a la fecha de hoy. \n\r";
			validate = false;
		}
	}
	if (!IsValid(ComboBoxResponsable) || ComboBoxResponsable.GetValue() === null) {
		errors += "Responsable es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (!IsValid(dateTimeStartMastered) || dateTimeStartMastered.GetValue() === null) {
		errors += "Fecha Hora Inicio es un campo Obligatorio. \n\r";
		validate = false;
	} else {
		if (!IsValid(dateTimeEndMastered) || dateTimeEndMastered.GetValue() === null) {
			errors += "Fecha Hora Fin es un campo Obligatorio. \n\r";
			validate = false;
		} else {
			var aDateTimeStartMastered = dateTimeStartMastered.GetDate();
			var yearDateTimeStartMastered = aDateTimeStartMastered.getFullYear();
			var monthDateTimeStartMastered = aDateTimeStartMastered.getMonth();
			var dayDateTimeStartMastered = aDateTimeStartMastered.getDate();
			var dateTimeStartMasteredAux = new Date(yearDateTimeStartMastered, monthDateTimeStartMastered, dayDateTimeStartMastered, aDateTimeStartMastered.getHours(), aDateTimeStartMastered.getMinutes(), 0);

			var aDateTimeEndMastered = dateTimeEndMastered.GetDate();
			var yearDateTimeEndMastered = aDateTimeEndMastered.getFullYear();
			var monthDateTimeEndMastered = aDateTimeEndMastered.getMonth();
			var dayDateTimeEndMastered = aDateTimeEndMastered.getDate();
			var dateTimeEndMasteredAux = new Date(yearDateTimeEndMastered, monthDateTimeEndMastered, dayDateTimeEndMastered, aDateTimeEndMastered.getHours(), aDateTimeEndMastered.getMinutes(), 0);

			if (dateTimeStartMasteredAux > dateTimeEndMasteredAux || dateTimeStartMasteredAux.getTime() === dateTimeEndMasteredAux.getTime()) {
				errors += "La Fecha Hora Fin debe ser mayor a la Fecha Hora Inicio. \n\r";
				validate = false;
			} else {
				if ($('#timeInitTurn').val() !== null && $('#timeInitTurn').val() !== "" && $('#timeEndTurn').val() !== null && $('#timeEndTurn').val() !== "") {
					var timeInitTurnAux = $('#timeInitTurn').val();
					var timeInitTurnAuxArray = timeInitTurnAux.split(":");
					var dateInitTurnWhithTimeAux = new Date(yearDateTimeStartMastered, monthDateTimeStartMastered, dayDateTimeStartMastered, timeInitTurnAuxArray[0], timeInitTurnAuxArray[1], 0);

					var timeEndTurnAux = $('#timeEndTurn').val();
					var timeEndTurnAuxArray = timeEndTurnAux.split(":");
					var dateEndTurnWhithTimeAux = new Date(yearDateTimeStartMastered, monthDateTimeStartMastered, dayDateTimeStartMastered, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);

					var dateStartMasteredStr = dayDateTimeStartMastered.toString().padStart(2, 0) + "/" + (++monthDateTimeStartMastered).toString().padStart(2, 0) + "/" + yearDateTimeStartMastered;
					if (dateInitTurnWhithTimeAux > dateTimeStartMasteredAux || dateEndTurnWhithTimeAux < dateTimeStartMasteredAux) {
						errors += "La Fecha Hora Inicio debe estar dentro del horario del turno: " + ComboBoxTurn.GetText() + ", con la fecha de inicio(" + dateStartMasteredStr + "). \n\r";
						validate = false;
					}
					if (dateInitTurnWhithTimeAux > dateTimeEndMasteredAux || dateEndTurnWhithTimeAux < dateTimeEndMasteredAux) {
						errors += "La Fecha Hora Fin debe estar dentro del horario del turno: " + ComboBoxTurn.GetText() + ", con la fecha de inicio(" + dateStartMasteredStr + "). \n\r";
						validate = false;
					}
				}
			}
		}
	}

	if (dateTimeStartMasteredAux < dateTimeEmisionAux) {
		errors += "La Fecha Hora Inicio debe ser mayor o igual a la Fecha Emisión. \n\r";
		validate = false;
	}

	if (!IsValid(ComboBoxTurn) || ComboBoxTurn.GetValue() === null) {
		errors += "Turno es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (!IsValid(ComboBoxBoxedWarehouse) || ComboBoxBoxedWarehouse.GetValue() === null) {
		errors += "Bodega Mat. Prima es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (!IsValid(ComboBoxBoxedWarehouseLocation) || ComboBoxBoxedWarehouseLocation.GetValue() === null) {
		errors += "Ubicación en Bodega Mat. Prima es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (!IsValid(ComboBoxMasteredWarehouse) || ComboBoxMasteredWarehouse.GetValue() === null) {
		errors += "Bodega Prod. Terminado es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (!IsValid(ComboBoxMasteredWarehouseLocation) || ComboBoxMasteredWarehouseLocation.GetValue() === null) {
		errors += "Ubicación en Bodega Prod. Terminado es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (!IsValid(ComboBoxWarehouseBoxes) || ComboBoxWarehouseBoxes.GetValue() === null) {
		errors += "Bodega Cajas es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (!IsValid(ComboBoxWarehouseLocationBoxes) || ComboBoxWarehouseLocationBoxes.GetValue() === null) {
		errors += "Ubicación en Bodega Cajas es un campo Obligatorio. \n\r";
		validate = false;
	}
	if (validate === false) {
		NotifyError("Error. " + errors);
	}

	return validate;
}

function ButtonUpdate_Click() {
	SaveItem(false);
}

function ButtonCancel_Click() {
	RedirecBack();
}

function RedirecBack() {
	showPage("Mastered/Index");
}

function InitializePagination() {
	if ($("#id_mastered").val() !== 0) {
		var current_page = 1;
		var max_page = 1;
		$.ajax({
			url: "Mastered/InitializePagination",
			type: "post",
			data: { id: $("#id_mastered").val() },
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				max_page = result.maximunPages;
				current_page = result.currentPage;
			},
			complete: function () {
			}
		});

		$('.pagination').jqPagination({
			current_page: current_page,
			max_page: max_page,
			page_string: "{current_page} de {max_page}",
			paged: function (page) {
				showPage("Mastered/Pagination", { page: page });
			}
		});
	}
}

function Init() {
	$("#btnCollapseFilterDetails").click(function (event) {
		if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
		} else {
		}
	});

	if ($('#id_mastered').val() !== 0 && $('#id_mastered').val() !== "0") {
		$("#btnCollapseFilterDetails").click();
	}
}

function PrintLot(data, url) {
	var data = {
		id: $("#imovExit").val(),
		codeReport: "INEGEG",
	};

	var url = "ProductionLotProcess/PrintEgreso";

	PrintItem(data, url);
}

$(function () {
	InitializePagination();
	Init();
});