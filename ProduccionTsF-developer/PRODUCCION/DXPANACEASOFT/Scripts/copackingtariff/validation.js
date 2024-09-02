
/// TransporTariff-Partial
function OnTTNameValidation(s, e) {
	isRequired(s, e);
}

function OnTTTransportTypeValidation(s, e) {

	var _id_transportTariff = $("#id_copackingTariff").val();
	isRequired(s, e);
}

function OnDateInitTransportTariffValidation(s, e) {
	isRequired(s, e);
}

function OnDateEndTransportTariffValidation(s, e) {
	isRequired(s, e);
}

function OnTTDFishingSiteValidation(s, e) {


	var _id_transportTariff = $("#id_transportTariff").val();

	var _id_transportTariffType = ASPxClientControl.GetControlCollection().GetByName("id_TransportTariffType").GetValue();

}

function OnTTDFishingSiteValidation2(s, e) {

	var _isTerrestriel = $("#hd-transporttariff-isterrestriel").val();
	var _isInternal = $("#hd-transporttariff-isinternal").val();
	var idControl = "";
	if (!isRequired(s, e)) return;



	if (_isInternal) {
		idControl = "id_IceBagRange";
	}

	if (_isTerrestriel) {
		idControl = "id_TransportSize";
	}

	ASPxClientControl.GetControlCollection().GetByName(idControl).Validate();


}

function OnTTDTransportSizeValidation(s, e) {

	// accion: validar si el tipo de tarifario es tipo terrestre
	var _isTerrestriel = boolInputHidden("hd-transporttariff-isterrestriel");
	if (_isTerrestriel) {
		isRequired(s, e);
	}

}

function OnTTDIceBagRangeValidation(s, e) {

	// accion: validar si el tipo de tarifario es tipo terrestre
	var _isInternal = boolInputHidden("hd-transporttariff-isinternal");
	if (_isInternal) {
		isRequired(s, e)
	}

}


function OnTariffControlValidation(s, e) {
	//isRequired(s, e);
	if (!TariffDateisValidate()) return;
	OnTransportTariffChangeDatesRange(s, e);


}

function OnTTChangeDatesRangeInit(s, e) {
	ASPxClientControl.GetControlCollection().GetByName("dateEnd").Validate();
}



function OnTTValidateDatesRangeInit(s, e) {
	if (!TariffDateisValidate()) return;
	OnTransportTariffChangeDatesRange(s, e);
}


function OnTTChangeDatesRangeEnd(s, e) {
	// ASPxClientControl.GetControlCollection().GetByName("dateInit").Validate();
	ASPxClientControl.GetControlCollection().GetByName("dateInit").Validate();

}

function OnTTValidateDatesRangeEnd(s, e) {
	if (!TariffDateisValidate()) return;
	OnTransportTariffChangeDatesRange(s, e);
}


function TariffDateisValidate() {
	var validate = true;
	if ($("[name='dateInit']").val() === undefined || $("[name='dateInit']").val() === undefined) return false;
	if ($("[name='dateInit']").val() === null || $("[name='dateInit']").val() === null) return false;

	return validate;
}


/* accion: valida la consistencuia de las fechas */
/* accion: valida que rango de fechas no se repita para otro tarifario del mismo tipo  */
function OnTransportTariffChangeDatesRange(s, e) {
	var _id_copackingTariff = $("#id_CopackingTariff").val();
	var fechaIni = $("[name='dateInit']").val();
	var fechaFin = $("[name='dateEnd']").val();

	var id_provider = ASPxClientControl.GetControlCollection().GetByName("id_provider").GetValue();


	if (fechaIni === undefined || fechaFin === undefined || id_provider === undefined) return;
	if (fechaIni === null || fechaFin === null || id_provider === null) return;
	if (fechaIni.length === 0 || fechaFin.length === 0 || id_provider.length === 0) return;

	$.ajax({
		url: "CopackingTariff/CopackingTarrifValidateConsistentDate",
		type: "post",
		data: {
			id_CopackingTariff: _id_copackingTariff,
			FechaIni: fechaIni,
			FechaFin: fechaFin,
			id_proveedor: id_provider
		},
		async: false,
		cache: false,
		error: function (error) {

		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {

			if (!(result === null)) {

				e.isValid = result.isValid;
				e.errorText = result.errorText;

				var _dateInit = ASPxClientControl.GetControlCollection().GetByName("dateInit");
				var _dateEnd = ASPxClientControl.GetControlCollection().GetByName("dateEnd");
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function OnTTCodeValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var noValido = / /;
		if (noValido.test(e.value)) { // se chequea el regex de que el string no tenga espacio
			e.errorText = " no puede contener espacios en blanco";
			e.isValid = false;
		}
		else {

			var id = $("#id_CopackingTariff").val();

			$.ajax({
				url: "CopackingTariff/ReptCodigo",
				type: "post",
				data: {
					id_CopackingTariff: $("#id_CopackingTariff").val(),
					codigo: e.value
				},
				async: false,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
				},
				success: function (result) {
					if (result !== null && result.rept) {
						e.errorText = "Ya existe un Tarifario con el mismo Codigo ";
						e.isValid = false;
					}
				},
				complete: function () {
				}
			});
		}
	}
}



// accion: valida que un tipo de tarifario solo este una vez
function OnTransportTariffSingleton(s, e, _id_transportTariff) {

}

function OnFishingSiteSingleton(s, e) {

}


function OnFishingSiteTransportSizeSingleton(s, e) {

}


function OnFishingSiteIceBagRangeSingleton(s, e) {

}
