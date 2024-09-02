function ButtonUpdate_Click(s, e) {

	Update();

}

function OnProcessPlantSelectedIndexChanged(s, e) {
	var id_processPlant = s.GetValue();
}

function OnLiquidationTypeSelectedIndexChanged(s, e) {
	var liquidation_type = s.GetValue();
}

function Update() {
	var id_accountingFreight = parseInt($("#id_accountingFreight").val());
	var actionUrl = (id_accountingFreight === 0) ? "AccountingFreight/Create" : "AccountingFreight/Update";

	var operationData = {
		 id_processPlant : id_processPlant.GetValue(),
		 liquidation_type : liquidation_type.GetValue()
	};

	$.ajax({
		url: actionUrl,
		type: "post",
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		data: JSON.stringify(operationData),
		async: true,
		cache: false,
		error: function (error) {
			console.error(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result.isValid) {
				var successData = {
					id: result.id,
					successMessage: result.message
				};
				NotifySuccess("Plantilla Guardada Satisfactoriamente.")
				showPage("AccountingFreight/FormEditAccountingFreight", successData);
			} else {
				ShowEditMessage(result.message);
			}
		},
		complete: function () {
			hideLoading();
		}
	});

}


var ShowEditMessage = function (message) {
	if (message !== null && message.length > 0) {
		$("#messageAlert").html(message);

		$(".close").click(function () {
			$(".alert").alert('close');
			$("#messageAlert").empty();
		});
	}
}

function ButtonClose_Click(s, e) {
	showPage("AccountingFreight/Index", null);
}

function OnMessageInit(s, e) {
	var message = $('#Message').val();
	if (!(message == undefined || message == "")) {
		if (gridMessage != undefined) {
			gridMessage.SetText(message);
			$("#GridMessage").show();
		}
	} else {
		if ($("#GridMessage") != undefined) {
			$("#GridMessage").hide();
		}

	}
}

function RemoveDetail(s, e) {
	gridViewMoveDetails.UnselectRows();
	gridViewMoveDetails.PerformCallback();
}

function RefreshDetail(s, e) {
	gridViewMoveDetails.UnselectRows();
	gridViewMoveDetails.PerformCallback();
}

