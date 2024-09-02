var OnCodeProductionCostValidation = function(s, e) {
	if (e.value === null || e.value.length === 0) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (e.value.length > 20) {
		e.isValid = false;
		e.errorText = "Máximo 20 caracteres";
	} else {
		$.ajax({
			url: "ProductionCost/ValidateCodeProductionCost",
			type: "post",
			async: false,
			cache: false, data: {
				id_productionCost: $("#id_productionCost").val(),
				code: e.value
			},
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {
				//showLoading();
			},
			success: function(result) {
				e.isValid = result.isValid;
				e.errorText = result.errorText;
			},
			complete: function() {
				//hideLoading();
			}
		});
	}
};

var OnNameProductionCostValidation = function(s, e) {
	if (e.value === null || e.value.length === 0) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (e.value.length > 50) {
		e.isValid = false;
		e.errorText = "Máximo 50 caracteres";
	}
};

var OnExecutionTypeProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnOrderProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (e.value < 0) {
		e.isValid = false;
		e.errorText = "Valor debe ser mayor o igual a cero";
	}
};


var OnCodeProductionCostDetailValidation = function(s, e) {
	if (e.value === null || e.value.length === 0) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (e.value.length > 20) {
		e.isValid = false;
		e.errorText = "Máximo 20 caracteres";
	}
};

var OnNameProductionCostDetailValidation = function(s, e) {
	if (e.value === null || e.value.length === 0) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (e.value.length > 50) {
		e.isValid = false;
		e.errorText = "Máximo 50 caracteres";
	}
};

var OnOrderProductionCostDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (e.value < 0) {
		e.isValid = false;
		e.errorText = "Valor debe ser mayor o igual a cero";
	}
};
