
// COMMON TABS FUNCTIOS VALIDATIONS

function UpdateTabImage(e, tabName) {
	var imageUrl = "/Content/image/noimage.png";
	if (!e.isValid) {
		imageUrl = "/Content/image/info-error.png";
	}

	var tab = tabControl.GetTabByName(tabName);
	tab.SetImageUrl(imageUrl);
	tab.SetActiveImageUrl(imageUrl);
}


function OnCodeValidation(s, e) {
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


			var id = $("#id_MachineForProd").val();

			$.ajax({
				url: "MachineForProd/ReptCodigo",
				type: "post",
				data: {
					id_MachineForProd: $("#id_MachineForProd").val(),
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
					if (result != null && result.rept) {
						e.errorText = "Ya existe una Máquina con el mismo Codigo ";
						e.isValid = false;
					}
				},
				complete: function () {
				}
			});
		}

	}
}
function OnNameValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnCapacityMachineValidation(s, e) {
	if (e.value === 0) {
		e.isValid = false;
		e.errorText = "Ingrese valor";
	}
}

function OnTypeMachineValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnWareouseTypeValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnMaterialWarehouseLocationValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnWarehouseValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnMaterialCenterCostValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnMaterialSubCenterCostValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnPlantProcesoValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}



















