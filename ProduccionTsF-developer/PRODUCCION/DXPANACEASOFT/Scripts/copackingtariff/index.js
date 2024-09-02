// BUTTONS ACTIONS

function AddNewItem(s, e) {
	gvTransportTariff.AddNewRow();
}

function RemoveItems(s, e) {
	gvTransportTariff.GetSelectedFieldValues("id", function (values) {

		var selectedRows = [];
		for (var i = 0; i < values.length; i++) {
			selectedRows.push(values[i]);
		}
		showConfirmationDialog(function () {
			gvTransportTariff.PerformCallback({ ids: selectedRows });
			gvTransportTariff.UnselectRows();
		});
	});
}

function RefreshGrid(s, e) {
	gvTransportTariff.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
	var id_copacking = $("#id_CopackingTariff").val();
	//// 
	if (id_copacking !== "0" && id_copacking !== undefined) {
		$.ajax({
			url: "CopackingTariff/CopackingTariffCopy",
			type: "post",
			data: {
				id: id_copacking
			},
			async: true,
			cache: false,
			error: function (error) {

			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				// 
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}


function ImportFile(data) {
    /* uploadFile("ItemType/ImportFileItemType", data, function (result) {
         gvItemTypes.Refresh();
     });*/
}

function Print(s, e) {
	var id_copacking = $("#id_CopackingTariff").val();
	var codeReport = "";

	if (id_copacking === "0")
		return;

	if (id_copacking === undefined) {
		id_copacking = 0;
		codeReport = "TRCKA";
	} else {
		codeReport = "TRCKI";
	}

	// 
	$.ajax({
		url: "CopackingTariff/PrintCopackingReport",
		type: "post",
		data: {
			id_copacking: id_copacking,
			codeReport: codeReport
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

function importFile(s, e) {
	console.log('Funcionalidad no implementada');
}


/// TransporTariff-Partial

function OnGridViewInit(s, e) {
	UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
	UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
	UpdateTitlePanel();
	ShowEditMessage(s, e);
}


function OnGridViewBeginCallback(s, e) {
	e.customArgs["keyToCopy"] = keyToCopy;
}


function UpdateTitlePanel() {
	var selectedFilteredRowCount = GetSelectedFilteredRowCount();

	var text = "Total de elementos seleccionados: <b>" + gvTransportTariff.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvTransportTariff.GetSelectedRowCount() - GetSelectedFilteredRowCount();
	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";
	$("#lblInfo").html(text);

	//if ($("#selectAllMode").val() != "AllPages") {
	SetElementVisibility("lnkSelectAllRows", gvTransportTariff.GetSelectedRowCount() > 0 && gvTransportTariff.cpVisibleRowCount > selectedFilteredRowCount);
	SetElementVisibility("lnkClearSelection", gvTransportTariff.GetSelectedRowCount() > 0);
	//}

	btnRemove.SetEnabled(gvTransportTariff.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
	return gvTransportTariff.cpFilteredRowCountWithoutPage + gvTransportTariff.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
	gvTransportTariff.SelectRows();
}

function UnselectAllRows() {
	gvTransportTariff.UnselectRows();
}

/// TransporTariffsDetail-Partial

function TransportTariffDetail_OnBeginCallback(s, e) {
	var objtransportTariffType = ASPxClientControl.GetControlCollection().GetByName("id_provider");
	e.customArgs['id_CopackingTariff'] = $("#id_CopackingTariff").val();
	e.customArgs['id_provider'] = (objtransportTariffType !== undefined) ? ((objtransportTariffType !== null) ? objtransportTariffType.GetValue() : 0) : 0;
}

function OnTTDFishingSiteValid(s, e) {

	if (typeof (gvTransportTariffDetail) === "undefined") {

		if (gvTransportTariffDetail !== null) {

			if (gvTransportTariffDetail.editState === 0) return false;

		}

	}

	return true;
}

function OnInventoryLineIngredientItemInit(s, e) {

	$.ajax({
		url: "Item/InventoryLineIngredientItemInit",
		type: "post",
		data: {
			id_inventoryLine: s.GetValue(),
			id_itemType: id_productType.GetValue()
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			initIngredientItem(result);
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function ComboInventoryLineIngredientItem_SelectedIndexChanged(s, e) {

	id_productType.ClearItems();
	// 
	$.ajax({
		url: "CopackingTariff/ItemTypesByInventoryLine",
		type: "post",
		data: { id_inventoryLine: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			for (var i = 0; i < result.length; i++) {
				var itemType = result[i];
				id_productType.AddItem(itemType.name, itemType.id);
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function CopackingTariffType_SelectedIndexChanged(s, e) {
	// 
	var _id_CopackingTariff = $("#id_CopackingTariff").val();

	ASPxClientControl.GetControlCollection().GetByName("dateInit").Validate();
	ASPxClientControl.GetControlCollection().GetByName("dateEnd").Validate();

	GetInfoProviderCopacking(s.GetValue());

	if (typeof (gvTransportTariffDetail) !== "undefined") {
		if (gvTransportTariffDetail !== null) {
			// if (gvTransportTariffDetail.editState == 0)
			gvTransportTariffDetail.PerformCallback({ id_transportTariffType: s.GetValue(), id_transportTariff: _id_transportTariff });
			return false;
		}
	}

	var _id_copackingTariff = $("#id_CopackingTariff").val();
	var _id_provider = ASPxClientControl.GetControlCollection().GetByName("id_provider").GetValue();

	if (typeof (_id_copackingTariff) === "undefined") return;
	if (_id_provider === null) return;

	$.ajax({
		url: "CopackingTariff/CopackingTariffDetail",
		type: "post",
		data: {
			id_CopackingTariff: _id_CopackingTariff,
			id_provider: _id_provider
		},
		async: false,
		cache: false,
		error: function (error) {

		},
		beforeSend: function () {
			// showLoading();
		},
		success: function (result) {

			if (!(result === null)) {
				$("#objTariffTransportDetail").empty();
				$("#objTariffTransportDetail").replaceWith(result);
			}
		},
		complete: function () {
			// hideLoading();
		}
	});
}

function GridViewTransportTariffCustomCommandButton_Click(s, e) {

	if (e.buttonID === "btnDeleteRow") {
		showConfirmationDialog(function () {
			s.DeleteRow(e.visibleIndex);
			s.UnselectRows();
		});
	}
}

// Aux. Funcions

function GetInfoProviderCopacking(id_provider) {

	$.ajax({
		url: "CopackingTariff/ObtainInfoProvider",
		type: "post",
		data: {
			id_provider: id_provider
		},
		async: true,
		cache: false,
		error: function (error) {

		},
		beforeSend: function () {
			// showLoading();
		},
		success: function (result) {
			true;
		},
		complete: function () {
			// hideLoading();
		}
	});


}

function OnTTDFishingSiteSelectedIndexChanged(s, e) {

}

// MAIN FUNCTIONS

function init() {
	$("#btnImport").hide();
}

$(function () {
	init();
});

function initIngredientItem(datosIniciales) {

	//inventoryLines

	for (var i = 0; i < id_inventoryLine.GetItemCount(); i++) {
		var inventoryLineIngredientItem = id_inventoryLine.GetItem(i);
		var into = false;
		for (var j = 0; j < datosIniciales.inventoryLines.length; j++) {
			if (inventoryLineIngredientItem.value === datosIniciales.inventoryLines[j].id) {
				into = true;
				break;
			}
		}
		if (!into) {
			id_inventoryLineIngredientItem.RemoveItem(i);
			i -= 1;
		}
	}

	for (var i = 0; i < datosIniciales.inventoryLines.length; i++) {
		var inventoryLineIngredientItem = id_inventoryLine.FindItemByValue(datosIniciales.inventoryLines[i].id);
		if (inventoryLineIngredientItem === null) id_inventoryLine.AddItem(datosIniciales.inventoryLines[i].name, datosIniciales.inventoryLines[i].id);
	}
	//itemTypes

	for (var i = 0; i < id_productType.GetItemCount(); i++) {
		var itemTypeIngredientItem = id_productType.GetItem(i);
		var into = false;
		for (var j = 0; j < datosIniciales.itemTypes.length; j++) {
			if (itemTypeIngredientItem.value === datosIniciales.itemTypes[j].id) {
				into = true;
				break;
			}
		}
		if (!into) {
			id_productType.RemoveItem(i);
			i -= 1;
		}
	}

	for (var i = 0; i < datosIniciales.itemTypes.length; i++) {
		var itemTypeIngredientItem = id_productType.FindItemByValue(datosIniciales.itemTypes[i].id);
		if (itemTypeIngredientItem === null) id_productType.AddItem(datosIniciales.itemTypes[i].name, datosIniciales.itemTypes[i].id);
	}
}
