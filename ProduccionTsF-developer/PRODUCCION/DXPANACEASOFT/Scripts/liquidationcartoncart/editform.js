
//Maquina
function MachineForProd_Init(s, e) {
	var idMachProdOpeningDetail = $("#id_MachineProdOpeningT2").val();
	
	if (idMachProdOpeningDetail === null || idMachProdOpeningDetail === undefined || idMachProdOpeningDetail === 0) {
		id_MachineProdOpeningDetail.SetValue(null);
	} else {
		id_MachineProdOpeningDetail.SetValue(idMachProdOpeningDetail);
		MachineForProd_SelectedIndexChanged(s, e);
		
	}
}

function MachineForProd_SelectedIndexChanged(s, e) {

	var idMachProdOpeningDetail = 0;
	var idMachineProdOpeningTmp = 0;
	if (s.GetValue() !== null && s.GetValue !== undefined)
	{
		var objItemSelected2 = id_MachineProdOpeningDetail.GetSelectedItem();
		if (objItemSelected2 != null)
		{
			idMachineProdOpeningTmp = objItemSelected2.GetColumnText(2);
			idMachProdOpeningDetail = objItemSelected2.GetColumnText(3);
		}
		
	}

	if (idMachProdOpeningDetail == 0 && idMachineProdOpeningTmp == 0) return;

	$.ajax({
		url: "LiquidationCartOnCart/GetDataMachineForProd",
		type: "post",
		data: { id_MachineForProd: idMachProdOpeningDetail, id_MachineProdOpening: idMachineProdOpeningTmp },
		async: false,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
			//showLoading();
		},
		success: function (result) {
			// 
			if (result !== null) {
				nameTurno.SetValue(result.nameTurno);
				$("#id_MachineForProd").val(result.id_MachineForProd);
				$("#id_MachineProdOpening").val(result.id_MachineProdOpening);
				$("#timeInitMachineProdOpeningDetail").val(new Date(2011, 1, 1, result.timeInitMachineProdOpeningDetail.Hours, result.timeInitMachineProdOpeningDetail.Minutes, result.timeInitMachineProdOpeningDetail.Seconds));

				dateInitLiquidation.SetDate(new Date(result.dateEmissionYear, result.dateEmissionMonth - 1, result.dateEmissionDay));
				dateEndLiquidation.SetDate(new Date(result.dateEmissionYear, result.dateEmissionMonth - 1, result.dateEmissionDay));
				//$("dateInitLiquidation").val(new Date(result.dateEmissionYear, result.dateEmissionMonth, result.dateEmissionDay));
				$("#timeInitTurn").val(new Date(2011, 1, 1, result.timeInitTurn.Hours, result.timeInitTurn.Minutes, result.timeInitTurn.Seconds));
				$("#timeEndTurn").val(new Date(2011, 1, 1, result.timeEndTurn.Hours, result.timeEndTurn.Minutes, result.timeEndTurn.Seconds));
			}
		},
		complete: function() {
		}
	});
}

function MachineForProd_BeginCallback(s, e) {
	e.customArgs["id_MachineForProd"] = s.cpMachineForProd;
	e.customArgs["id_MachineProdOpening"] = s.cpMachineProdOpening;
	e.customArgs["documentStateCode"] = s.cpDocumentStateCode;
	e.customArgs["emissionDate"] = emissionDate.GetDate();
	e.customArgs["id_PersonProcessPlant"] = s.cpPersonProcessPlant;
}

function OnLiquidationCartOnCartEmissionDateChanged(s, e) {
	id_MachineProdOpeningDetail.PerformCallback();
}

function OnIdProcessType_SelectedIndexChanged(s, e) {
	var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
    var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
    if (window["ItemLiquidation" + key] !== null && window["ItemLiquidation" + key] !== undefined) {
		window["ItemLiquidation" + key].PerformCallback();
	}
}

function Update(approve) {
	var valid = true;
	var validDocumentLiquidationCartOnCart = ASPxClientEdit.ValidateEditorsInContainerById("documentLiquidationCartOnCart", null, true);
	//var validTableProccess = ASPxClientEdit.ValidateEditorsInContainerById("tableProccess", null, true);
	var validTableProccess = ASPxClientEdit.ValidateEditorsInContainerById("tableProccess", null, true);
	var validGroupItem1FormLayoutEditLiquidationCartOnCart = ASPxClientEdit.ValidateEditorsInContainerById("groupItem1FormLayoutEditLiquidationCartOnCart", null, true);
	var validGroupItem2FormLayoutEditLiquidationCartOnCartTime = ASPxClientEdit.ValidateEditorsInContainerById("groupItem2FormLayoutEditLiquidationCartOnCartTime", null, true);



	if (validDocumentLiquidationCartOnCart) {
		UpdateTabImage({ isValid: true }, "tabDocument");
	} else {
		UpdateTabImage({ isValid: false }, "tabDocument");
		valid = false;
	}

	if (validTableProccess) {
		UpdateTabImage({ isValid: true }, "tabProccess");
	} else {
		UpdateTabImage({ isValid: false }, "tabProccess");
		valid = false;
	}

	if (validGroupItem1FormLayoutEditLiquidationCartOnCart) {
		if (validGroupItem2FormLayoutEditLiquidationCartOnCartTime) {
			if ((gvLiquidationCartOnCartDetailEditForm.cpRowsCount === 0 && gvLiquidationCartOnCartReceivedDetailEditForm.cpRowsCount === 0) || gvLiquidationCartOnCartDetailEditForm.IsEditing()) {
				UpdateTabImage({ isValid: false }, "tabLiquidationCartOnCartDetails");
				valid = false;
			} else {
				UpdateTabImage({ isValid: true }, "tabLiquidationCartOnCartDetails");
			}
		} else {
			UpdateTabImage({ isValid: false }, "tabLiquidationCartOnCartDetails");
			valid = false;
		}
	} else {
		UpdateTabImage({ isValid: false }, "tabLiquidationCartOnCartDetails");
		valid = false;
	}

	if (valid) {
		var id = $("#id_liquidationCartOnCart").val();

		var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditLiquidationCartOnCart").serialize();

		var url = id === "0"
			? "LiquidationCartOnCart/LiquidationCartOnCartAddNew"
			: "LiquidationCartOnCart/LiquidationCartOnCartUpdate";

		showForm(url, data);
	}
}

// DIALOG BUTTONS ACTIONS
function ButtonUpdate_Click(s, e) {

	Update(false);
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
	showPage("LiquidationCartOnCart/Index", null);
}

// BUTTONS ACTION 
function AddNewDocument(s, e) {
	pagesShown = [];
	$.ajax({
		url: "LiquidationCartOnCart/Index",
		type: "post",
		data: null,
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
			$.ajax({
				url: "LiquidationCartOnCart/ProductionLotsResults",
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
					$("#btnCollapse").click();
					$("#results").html(result);
				},
				complete: function () {
					hideLoading();
				}
			});
		},
		complete: function () {
			hideLoading();
		}
	});
}

function SaveDocument(s, e) {
	ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
	ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
}

function ApproveDocument(s, e) {

	showConfirmationDialog(function() {
		Update(true);
	}, "¿Desea aprobar la Liquidación Carro por Carro?");
}

function AutorizeDocument(s, e) {
	showConfirmationDialog(function() {
		var data = {
			id: $("#id_invoiceCommercial").val()
		};
		showForm("InvoiceCommercial/Autorize", data);
	}, "¿Desea autorizar la Factura Comercial?");
}

function ProtectDocument(s, e) {
}

function CancelDocument(s, e) {
	showConfirmationDialog(function() {
		var data = {
			id: $("#id_liquidationCartOnCart").val()
		};
		showForm("LiquidationCartOnCart/Cancel", data);
	}, "¿Desea anular la Liquidación Carro por Carro?");
}

function RevertDocument(s, e) {
	showConfirmationDialog(function() {
		var data = {
			id: $("#id_liquidationCartOnCart").val()
		};
		showForm("LiquidationCartOnCart/Revert", data);
	}, "¿Desea reversar la Liquidación Carro por Carro?");
}

function ShowDocumentHistory(s, e) {
}

function PrintDocument(s, e) {
	var _codeEnteroRep = $("#codeEnteroReport").val();
	var _codeColaRep = $("#codeColaReport").val();

	var data = { id_liquidation: $("#id_liquidationCartOnCart").val(), codeProcessType: "ENT", codeReport: "LCXCP" };


	if (_codeEnteroRep === "Y") {
		$.ajax({
			url: 'LiquidationCartOnCart/PrintReportLiquidation',
			data: data,
			async: true,
			cache: false,
			type: 'POST',
			beforeSend: function() {
				showLoading();
			},
			success: function(result) {
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
			complete: function() {
				hideLoading();
			}
		});
	}

	data = { id_liquidation: $("#id_liquidationCartOnCart").val(), codeProcessType: "COL", codeReport: "LCXCP" };

	if (_codeColaRep === "Y") {
		$.ajax({
			url: 'LiquidationCartOnCart/PrintReportLiquidation',
			data: data,
			async: true,
			cache: false,
			type: 'POST',
			beforeSend: function() {
				showLoading();
			},
			success: function(result) {
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
			complete: function() {
				hideLoading();
			}
		});
	}

}


function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}


// UPDATE VIEW
function AutoCloseAlert() {
	if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
		setTimeout(function() {
			$(".alert-success").alert('close');
		}, 2000);
	}
}

function UpdateView() {
	var id = parseInt($("#id_liquidationCartOnCart").val());

	// EDITING BUTTONS
	btnNew.SetEnabled(false);
	btnSave.SetEnabled(false);
	btnCopy.SetVisible(false);
	btnAutorize.SetVisible(false);
	btnProtect.SetVisible(false);

	// STATES BUTTONS

	$.ajax({
		url: "LiquidationCartOnCart/Actions",
		type: "post",
		data: { id: id },
		async: true,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
			//showLoading();
		},
		success: function(result) {
			btnApprove.SetEnabled(result.btnApprove);
			//btnAutorize.SetEnabled(result.btnAutorize);
			//btnProtect.SetEnabled(result.btnProtect);
			btnCancel.SetEnabled(result.btnCancel);
			btnRevert.SetEnabled(result.btnRevert);
			btnSave.SetEnabled(result.btnSave);
		},
		complete: function(result) {
			//hideLoading();
		}
	});

	// NEW BUTTON
	btnNew.SetEnabled(id !== 0);
	// HISTORY BUTTON
	btnHistory.SetEnabled(id !== 0);

	// PRINT BUTTON
	btnPrint.SetEnabled(id !== 0);

	if (codeDocumentState === "01") {
		btnRemoveDetail.SetVisible(false);
	}
}

function UpdatePagination() {
	var current_page = 1;
	$.ajax({
		url: "LiquidationCartOnCart/InitializePagination",
		type: "post",
		data: { id_liquidationCartOnCart: $("#id_liquidationCartOnCart").val() },
		async: false,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
		},
		success: function(result) {
			$("#pagination").attr("data-max-page", result.maximunPages);
			current_page = result.currentPage;
		},
		complete: function() {
		}
	});
	$('.pagination').current_page = current_page;
}

function init() {
	UpdatePagination();

	AutoCloseAlert();
}

$(function() {

	var chkReadyState = setInterval(function() {
		if (document.readyState === "complete") {
			clearInterval(chkReadyState);
			UpdateView();
		}
	}, 100);

	init();
});