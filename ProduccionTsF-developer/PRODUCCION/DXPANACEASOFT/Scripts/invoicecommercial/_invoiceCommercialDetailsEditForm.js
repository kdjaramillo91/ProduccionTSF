function InvoiceCommercialsDetail_OnBeginCallback(s, e) {
	var resHasGlaze = hasGlaze.GetValue();
	e.customArgs["hasGlazeValue"] = (resHasGlaze === null || resHasGlaze === 0) ? false : resHasGlaze;
}
// EDITOR'S EVENTS

function UpdateTitlePanelDetail() {
	var gv = gvInvoiceCommercialEditFormDetail;

	var selectedFilteredRowCount = GetSelectedFilteredRowCount();

	var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";

	$("#lblInfoDetails").html(text);

	if ($("#selectAllMode").val() !== "AllPages") {
		SetElementVisibility("lnkSelectAllRowsDetails", gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
		SetElementVisibility("lnkClearSelectionDetails", gv.GetSelectedRowCount() > 0);
	}
}

function GetSelectedFilteredRowCount() {
	return gvInvoiceCommercialEditFormDetail.cpFilteredRowCountWithoutPage + gvInvoiceCommercialEditFormDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewInitDetail(s, e) {
	UpdateTitlePanelDetail();
}

function OnGridViewSelectionChangedDetail(s, e) {
	UpdateTitlePanelDetail();

	gvInvoiceCommercialEditFormDetail.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackDetail);
}

function GetSelectedFieldValuesCallbackDetail(values) {
	var selectedRows = [];
	for (var i = 0; i < values.length; i++) {
		selectedRows.push(values[i]);
	}
}

var customCommand = "";

function OnGridViewBeginCallbackDetail(s, e) {
	customCommand = e.command;
}

function OnGridViewEndCallbackDetail(s, e) {
	UpdateTitlePanelDetail();
}

function gvEditClearSelectionDetail() {
	gvInvoiceCommercialEditFormDetail.UnselectRows();
}

function gvEditSelectAllRowsDetail() {
	gvInvoiceCommercialEditFormDetail.SelectRows();
}

//gvInvoiceCommercialEditFormDetail

var itemCurrentAux = null;

var id_itemInit = null;
var id_itemCurrent = null;
function ItemComboBox_Init(s, e) {
	id_itemInit = s.GetValue();
	id_itemCurrent = s.GetValue();
	if (gvInvoiceCommercialEditFormDetail.cpDocumentOrigen !== null && gvInvoiceCommercialEditFormDetail.cpDocumentOrigen !== "") {
		id_item.PerformCallback();
	}
}
function ItemMarkedComboBox_Init(s, e) {
	itemCurrentAux = s.GetValue();
}

function ItemComboBox_BeginCallback(s, e) {
	e.customArgs["id_itemCurrent"] = id_itemCurrent;
}

function ItemMarkedComboBox_BeginCallback(s, e) {
	e.customArgs["id_itemCurrent"] = itemCurrentAux;
}

function ItemRealComboBox_BeginCallback(s, e) {
	e.customArgs["id_itemCurrent"] = itemCurrentAux;
}

function ItemComboBox_EndCallback(s, e) {
	if (id_itemInit !== null) {
		id_item.SetValue(id_itemInit);
	}
}

function OnItemComboBoxValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnItemMarkedComboBoxValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function ItemComboBox_SelectedIndexChanged(s, e) {
	itemInvoiceCommercialAuxCode.SetText("");
	itemInvoiceCommercialMasterCode.SetText("");
	amountInvoice.SetValue(null);
	invoiceCommercialTotal.SetValue(null);

	var unitPriceAux = unitPrice.GetValue();
	var strUnitPrice = ((unitPriceAux === null) ? "0" : unitPriceAux.toString());
	var resUnitPrice = strUnitPrice.replace(".", ",");

	var numBoxesAux = numBoxes.GetValue();
	var strNumBoxes = ((numBoxesAux === null) ? "0" : numBoxesAux.toString());
	var resNumBoxes = strNumBoxes.replace(".", ",");

	var hasGlazeValue = hasGlaze.GetValue();
	var resHasGlazeValue = (hasGlazeValue === null || hasGlazeValue === 0) ? false : hasGlazeValue;

	UpdateInvoiceCommercialDetail(s.GetValue(), resNumBoxes, resUnitPrice, resHasGlazeValue);
}

function ItemMarkedComboBox_SelectedIndexChanged(s, e) {
}

function UpdateInvoiceCommercialDetail(id_itemCurrent, numBoxesCurrent, unitPriceCurrent, hasGlazeValue) {
	var _id_metricUnitInvoice = id_metricUnitInvoice.GetValue();

	var data =
	{
		id_itemCurrent: id_itemCurrent,
		numBoxesCurrent: numBoxesCurrent,
		unitPriceCurrent: unitPriceCurrent,
		id_MetricUnitInvoice: _id_metricUnitInvoice,
		hasGlazeValue: hasGlazeValue
	};

	$.ajax({
		url: "InvoiceCommercial/UpdateInvoiceCommercialDetail",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			if (result !== null && result !== undefined) {
				if (result.codeReturn === -1) {
					console.log(result.message);
					return;
				}

				var _itemInvoiceCommercialAuxCode = result.ValueDataList[0].valueObject;
				var _itemInvoiceCommercialMasterCode = result.ValueDataList[1].valueObject;
				var _itemInvoiceCommercialForeignName = result.ValueDataList[2].valueObject;
				var _amountInvoice = parseFloat(result.ValueDataList[3].valueObject);
				var _itemInvoiceCommercialUM = result.ValueDataList[4].valueObject;
				var _invoiceCommercialTotal = parseFloat(result.ValueDataList[5].valueObject);
				var _itemInvoiceCommercialweightBox = parseFloat(result.ValueDataList[6].valueObject);
				var _itemInvoiceCommercialweightBoxUM = result.ValueDataList[7].valueObject;
				var _unitPrice = result.ValueDataList[8].valueObject;
				var _numBoxes = result.ValueDataList[9].valueObject;
				var _idMarked = result.ValueDataList[10].valueObject;
				var _codigoItemMarked = result.ValueDataList[11].valueObject;
				var _nombreItemMarked = result.ValueDataList[12].valueObject;

				itemInvoiceCommercialAuxCode.SetText(_itemInvoiceCommercialAuxCode);
				itemInvoiceCommercialMasterCode.SetText(_itemInvoiceCommercialMasterCode);
				itemInvoiceCommercialForeignName.SetText(_itemInvoiceCommercialForeignName);
				amountInvoice.SetValue(_amountInvoice);
				itemInvoiceCommercialUM.SetText(_itemInvoiceCommercialUM);
				invoiceCommercialTotal.SetValue(_invoiceCommercialTotal);

				weightBox.SetValue(_itemInvoiceCommercialweightBox);
				weightBoxUM.SetValue(_itemInvoiceCommercialweightBoxUM);
				unitPrice.SetValue(_unitPrice);
				numBoxes.SetValue(_numBoxes);

				id_itemMarked.ClearItems();
				id_itemMarked.AddItem([_codigoItemMarked, _nombreItemMarked], _idMarked)
				id_itemMarked.SetValue(_idMarked);
			}
		},
		complete: function () {
		}
	});
}

function GlazePercentage_NumberChange(s, e) {
	var unitPriceAux = unitPrice.GetValue();
	var strUnitPrice = (unitPriceAux === null) ? "0" : unitPriceAux.toString();
	var resUnitPrice = strUnitPrice.replace(".", ",");

	var numBoxesAux = numBoxes.GetValue();
	var strNumBoxes = (numBoxesAux === null) ? "0" : numBoxesAux.toString();
	var resNumBoxes = strNumBoxes.replace(".", ",");

	var glassPercent = glazePercentageDetail.GetValue();
	var strGlassPercent = (glassPercent === null) ? "0" : glassPercent.toString();
	var hasGlazeValue = hasGlaze.GetValue();
	var resHasGlazeValue = (hasGlazeValue === null || hasGlazeValue === 0) ? false : hasGlazeValue;

	UpdateInvoiceCommercialDetail(id_item.GetValue(), resNumBoxes, resUnitPrice, resHasGlazeValue);
}

function NumBoxes_NumberChange(s, e) {
	var unitPriceAux = unitPrice.GetValue();
	var strUnitPrice = (unitPriceAux === null) ? "0" : unitPriceAux.toString();
	var resUnitPrice = strUnitPrice.replace(".", ",");

	var numBoxesAux = s.GetValue();
	var strNumBoxes = (numBoxesAux === null) ? "0" : numBoxesAux.toString();
	var resNumBoxes = strNumBoxes.replace(".", ",");

	var hasGlazeValue = hasGlaze.GetValue();
	var resHasGlazeValue = (hasGlazeValue === null || hasGlazeValue === 0) ? false : hasGlazeValue;

	UpdateInvoiceCommercialDetail(id_item.GetValue(), resNumBoxes, resUnitPrice, resHasGlazeValue);
}

function ValidateRequired(s, e) {
	if (s.GetValue() <= 0) {
		e.errorText = "Debe ingresar valor mayor a cero";
		e.isValid = false;
	}
}

function UnitPrice_NumberChange(s, e) {
	var unitPriceAux = s.GetValue();
	var strUnitPrice = (unitPriceAux === null) ? "0" : unitPriceAux.toString();
	var resUnitPrice = strUnitPrice.replace(".", ",");

	var numBoxesAux = numBoxes.GetValue();
	var strNumBoxes = (numBoxesAux === null) ? "0" : numBoxesAux.toString();
	var resNumBoxes = strNumBoxes.replace(".", ",");
	var hasGlazeValue = hasGlaze.GetValue();
	var resHasGlazeValue = (hasGlazeValue === null || hasGlazeValue === 0) ? false : hasGlazeValue;

	UpdateInvoiceCommercialDetail(id_item.GetValue(), resNumBoxes, resUnitPrice, resHasGlazeValue);
}

function InvoiceCommercialDiscount_Init(s, e) {
	var numCartonesTotal = gvInvoiceCommercialEditFormDetail.cpTotalBoxes;
	var numCartonesDetail = numBoxes.GetValue();
	var descuentoTotal = valueDiscount.GetValue();

	var factor = numCartonesTotal > 0
		? numCartonesDetail / numCartonesTotal
		: 0.00;

	var descuentoDetalle = descuentoTotal * factor;
	invoiceCommercialDiscount.SetValue(descuentoDetalle);
}

function ButtonUpdateItemDetail_Click(s, e) {
	var valid = true;
	var validFormLayoutEditInvoiceCommercialDetail = ASPxClientEdit.ValidateEditorsInContainerById("formLayoutEditInvoiceCommercialDetail", null, true);

	if (validFormLayoutEditInvoiceCommercialDetail) {
	} else {
		valid = false;
	}

	if (unitPrice.GetValue() <= 0 || numBoxes.GetValue() <= 0) valid = false;

	if (valid) {
		UpdateTabImage({ isValid: true }, "tabInvoiceCommercialDetails");

		gvInvoiceCommercialEditFormDetail.UpdateEdit();
	}
	else {
		UpdateTabImage({ isValid: false }, "tabInvoiceCommercialDetails");
	}
}

function BtnCancelItemDetail_Click(s, e) {
	gvInvoiceCommercialEditFormDetail.CancelEdit();
}