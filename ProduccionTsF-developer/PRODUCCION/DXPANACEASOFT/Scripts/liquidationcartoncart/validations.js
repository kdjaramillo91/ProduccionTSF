
function OnLiquidationCartOnCartEmissionDateValidation(s, e) {
	OnLiquidationDateReceptionValidation(s, emissionDate);
	OnEmissionDateDocumentValidation(e, emissionDate, "liquidationCartOnCart");
	
	UpdateTabImage(e, "tabDocument");
}

function OnRangeLiquidationDateValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		OnRangeDateValidation(e, dateInitLiquidation.GetValue(), dateEndLiquidation.GetValue(), "Rango de Fecha no válido");
	}
}

function OnLiquidationDateReceptionValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		// 
		var emissionDateTmp = emissionDate.GetText();
		var recepcionDatetmp = receptionDate.GetText();

		var tmp = emissionDateTmp.split("/");
		if (tmp.length == 3) {
			_emissionDate = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
		}
		var tmp1 = recepcionDatetmp.split("/");
		if (tmp1.length == 3) {
			_recepcionDate = tmp1[1] + "/" + tmp1[0] + "/" + tmp1[2];
		}
		if (emissionDateTmp != null) {
			if ((new Date(_recepcionDate).getTime() > new Date(_emissionDate).getTime())) {
				e.isValid = false;
				// 
				e.errorText = "Fecha de Emisión de la Liquidación no puede ser menor que la fecha de Recepción.";
				//UpdateTabImage(e, "tabDocument");
				//return;
			}
		}
	}

}

function OnIdProcessTypeValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}
// TABIMAGE

function UpdateTabImage(e, tabName) {
	var imageUrl = "/Content/image/noimage.png";
	if (!e.isValid) {
		imageUrl = "/Content/image/info-error.png";
	}
	var tab = tabControl.GetTabByName(tabName);
	tab.SetImageUrl(imageUrl);
	tab.SetActiveImageUrl(imageUrl);
}


function MachineForProd_Validation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnLiquidatorValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function TimeInitLiquidation_Validation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var timeInitMachineProdOpeningDetailAux = new Date($("#timeInitMachineProdOpeningDetail").val());
		var strTimeInitMachineProdOpeningDetailAux = timeInitMachineProdOpeningDetailAux.getHours() + ":" + timeInitMachineProdOpeningDetailAux.getMinutes() + ":" + timeInitMachineProdOpeningDetailAux.getSeconds();
		var timeEndTurnAux = new Date($("#timeEndTurn").val());
		var strTimeEndTurnAux = timeEndTurnAux.getHours() + ":" + timeEndTurnAux.getMinutes() + ":" + timeEndTurnAux.getSeconds();
		e.isValid = IsValidateTimeInRange(e, timeInitMachineProdOpeningDetailAux, timeEndTurnAux, timeInitLiquidation.GetValue(), "Hora Inicio de la Liquidación debe estar dentro de la hora inicial(" + strTimeInitMachineProdOpeningDetailAux + ") de la apertura y la hora final(" + strTimeEndTurnAux + ") del turno.", true, false)
		if (!e.isValid) {
			e.errorText = "Hora de Inicio de la Liquidación debe estar dentro de la hora inicial(" + strTimeInitMachineProdOpeningDetailAux + ") de la apertura y el hora final(" + strTimeEndTurnAux + ") del turno.";
		}
	}
}

function TimeEndLiquidation_Validation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var timeInitLiquidationAux = timeInitLiquidation.GetValue();
		var strTimeInitLiquidationAux = timeInitLiquidationAux.getHours() + ":" + timeInitLiquidationAux.getMinutes() + ":" + timeInitLiquidationAux.getSeconds();
		var timeEndTurnAux = new Date($("#timeEndTurn").val());
		var strTimeEndTurnAux = timeEndTurnAux.getHours() + ":" + timeEndTurnAux.getMinutes() + ":" + timeEndTurnAux.getSeconds();
		e.isValid = IsValidateTimeInRange(e, timeInitLiquidationAux, timeEndTurnAux, timeEndLiquidation.GetValue(), "Hora Fin de la Liquidación debe estar dentro de la hora inicial(" + strTimeInitLiquidationAux + ") de la liquidación y la hora final(" + strTimeEndTurnAux + ") del turno.", false, true)
		if (!e.isValid) {
			e.errorText = "Hora de Fin de la Liquidación debe estar dentro de la hora inicial(" + strTimeInitLiquidationAux + ") de la apertura y el hora final(" + strTimeEndTurnAux + ") del turno.";
		}
	}
}