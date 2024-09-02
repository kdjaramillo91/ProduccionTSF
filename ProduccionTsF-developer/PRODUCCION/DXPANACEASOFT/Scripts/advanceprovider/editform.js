function CalculateAdvanceProvider() {
	var data = $("#formEditAdvanceProviderPL").serialize();
	showForm("AdvanceProvider/CalculateAdvanceProvider", data);
}

var CalculatePercentageAdvanceProvider = function() {
	var data = $("#formEditAdvanceProviderPL").serialize();

	$.ajax({
		url: 'AdvanceProvider/CalculatePercentageAdvanceProvider',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function() {
			showLoading();
		},
		success: function(result) {
			try {
				if (result) {
					AdvanceValuePercentageUsed.SetValue(result.valuePercentageAdvance);
				}
				hideLoading();
			}
			catch (err) {
				hideLoading();
			}
		},
		complete: function() {
			hideLoading();
		}
	});
};

function PrintDocument(s, e) {
	var id = parseInt(document.getElementById("id_ap").getAttribute("idAdvanceProvider"));
	var data = { id_adpr: id };

	$.ajax({
		url: 'AdvanceProvider/PrintAdvanceProviderReport',
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

function SetActiveTab(s, e) {
	tabControl.SetActiveTab(tabControl.GetTab(1));
}

//FORM EDIT
function OnPriceListBeginCallback(s, e) {
	e.customArgs['id_provider'] = $("#id_provider").val();
	e.customArgs['purchaseOrderDate'] = purchaseOrderDate.GetDate();
}

var OnPriceListSelectedIndexChanged = function(s, e) {
	if (s.GetValue() === null) {
		AdvanceValuePercentageUsed.SetValue(0);
	} else {
		CalculatePercentageAdvanceProvider();
	}
};

function OnPurchaseOrderDateValueChanged(s, e) {
	id_priceList.PerformCallback();
}

function OnPriceListDetailClick(s, e) {
	var id_Tmp = id_priceList.GetValue();
	var idProvider_Tmp = $("#id_provider").val();

	// 
	if (id_Tmp > 0) {
		showThickBox("PriceListDetail/PopupControlPriceListDetail", { id_priceList: id_Tmp, idProvider: idProvider_Tmp, idOc: null });
	}
}

function ApproveDocument(s, e) {

	//Validar Listas de Precios

	var data = { id_priList: id_priceList.GetValue(), id_procType: $("#id_procType").val() };
	$.ajax({
		url: "AdvanceProvider/ValidateProcessType",
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
				if (result.isListApproved == "NO") { /*result.areDiferentProc == "YES" || */
					id_priceList.Validate();
					return;
				} else {
					showConfirmationDialog(function () {
						var data2 = {
							id: $("#id_ap").val()
						};
						showForm("AdvanceProvider/Approve", data2);
					}, "¿Desea aprobar el anticipo a Proveedor?");
				}
			}
		},
		complete: function () {
		}
	});

}
// UPDATE VIEW

function AutoCloseAlert() {
	if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
		$(".alert-success").fadeTo(3000, 0.45, function () {
			$(".alert-success").alert('close');
		});
		//setTimeout(function () {
		//    $(".alert-success").alert('close');
		//}, 2000);
	}

}

function UpdateView() {
	var id = parseInt($("#id_ap").val());

	//// EDITING BUTTONS
	//btnNew.SetEnabled(true);
	//btnSave.SetEnabled(false);
	//btnCopy.SetEnabled(id !== 0);

	// STATES BUTTONS

	$.ajax({
		url: "AdvanceProvider/Actions",
		type: "post",
		data: { id: id },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			btnApprove.SetEnabled(result.btnApprove);
			btnAutorize.SetEnabled(result.btnAutorize);
			btnProtect.SetEnabled(result.btnProtect);
			btnCancel.SetEnabled(result.btnCancel);
			btnRevert.SetEnabled(result.btnRevert);
		},
		complete: function (result) {
			hideLoading();
		}
	});

	// PRINT BUTTON
	btnPrint.SetEnabled(id !== 0);
}

function UpdatePagination() {
	var current_page = 1;
	$.ajax({
		url: "AdvanceProvider/InitializePagination",
		type: "post",
		data: { id_advanceProvider: $("#id_ap").val() },
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			$("#pagination").attr("data-max-page", result.maximunPages);
			current_page = result.currentPage;
		},
		complete: function () {
		}
	});
	$('.pagination').current_page = current_page;
}


//QuantityAdvanceProvider
function Quantity_NumberChange(s, e) {
    var quantity = s.GetValue();
    if (quantity === null) { quantity = 0.0; }

    var valueAdvancetmp = valueAdvance.GetValue();

    if (quantity === 0) {
        NotifyWarning("El % del anticipo no puede ser 0");
        return;
    }

    if (valueAdvancetmp < quantity) {
        NotifyWarning("El % del anticipo no puede ser mayor al valor total.");
        return;
    }

    var valueAdvanceProviderAux = Math.round((parseFloat(quantity) / parseFloat(valueAdvance.GetValue())) * 100,0);
    AdvanceValuePercentageUsed.SetValue(valueAdvanceProviderAux);

}


// MAIN FUNCTIONS

function init() {
	UpdatePagination();
	AutoCloseAlert();
}

$(function () {

	var chkReadyState = setInterval(function () {
		if (document.readyState === "complete") {
			clearInterval(chkReadyState);
			UpdateView();
		}
	}, 100);

	init();
});