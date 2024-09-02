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

function UpdateTabControlImage(e, tabName, tabControlCurrent) {
	var imageUrl = "/Content/image/noimage.png";
	if (!e.isValid) {
		imageUrl = "/Content/image/info-error.png";
	}
	var tab = tabControlCurrent.GetTabByName(tabName);
	tab.SetImageUrl(imageUrl);
	tab.SetActiveImageUrl(imageUrl);
}

function OnIdentificationNumberValidation(s, e) {
	e.isValid = true;
	e.errorText = "";
	var result = OnIdentificationNumberValidationGeneral(e.value);
	// 
	if (result !== undefined) {
		// 
		e.isValid = result.isValid;
		e.errorText = result.errorTextMessage;
	}

}

function OnIdentificationNumberValidationGeneral(parametroIdentificacion) {
	// 
	var item = id_identificationType.GetSelectedItem();
	var errorTextMessage = "";
	var isValidMessage = true;
	var isRepeatedTmp = "";

	if (parametroIdentificacion == null || parametroIdentificacion == undefined) {
		isValid = false;
		errorTextMessage = "Campo Obligatorio";
	} else {
		var validation = null;

		var id_personTmp = $("#id_person").val();
		$.ajax({
			url: "Person/VerifyIdentificationCode",
			type: "post",
			data: { id_code: parametroIdentificacion, id_person: id_personTmp },
			async: false,
			cache: false,
			error: function (error) {
			},
			beforedSend: function () {

			},
			success: function (result) {

				if (result.isRepeated == "SI") {
					isRepeatedTmp = "SI";
					isValidMessage = false;
					errorTextMessage = "Número de Identificación pertenece a la persona " + result.personName;
				}
			},
			complete: function () {

			}
		});

		if (isRepeatedTmp == "SI") {
			return { isValid: isValidMessage, errorTextMessage: errorTextMessage }
		}
		if (item != null) {
			// 
			if (item.value == 2) {
				validation = validarCI(parametroIdentificacion);
			} else if (item.value == 1) {
				validation = validarRUC(parametroIdentificacion);
			}
		}


		if (validation != null && validation != undefined && validation.isValid == false) {
			isValidMessage = validation.isValid;
			errorTextMessage = validation.errorText;
		}
		return { isValid: isValidMessage, errorTextMessage: errorTextMessage }
	}
}


function OnEmailValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var validation = validarEMAIL(e.value);

		if (!validation.isValid) {
			e.isValid = validation.isValid;
			e.errorText = validation.errorText;
		}
	}
}

// PROVIDER TYPE SHRIMP VALIDATOR
function OnMinisterialAgreementValidation(s, e) {
	var tab = tabControl.GetTabByName("tabProvider");
	var id_copackingTmp = isCopackingDetail.GetChecked();

	// 
	if (id_copackingTmp) {
		if (!tab.GetVisible()) {
			e.isValid = true;
			UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		} else if (e.value === null) {
			var id_providerType2 = id_providerType.GetValue();

			if (id_providerType2 !== 0 && id_providerType2 !== null && id_providerType2 !== undefined) {
				$.ajax({
					url: "Person/ProviderTypeWhatProviderIs",
					type: "post",
					data: { id_providerType: id_providerType2 },
					async: false,
					cache: false,
					error: function (error) {
						console.log(error);
					},
					beforedSend: function () {

					},
					success: function (result) {
						if (result.isShrimpProvider === "SI") {
							e.isValid = false;
							e.errorText = "Campo obligatorio";
							UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
							UpdateTabImage(e, "tabProvider");
						} else {
							e.isValid = true;
							UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
							UpdateTabImage(e, "tabProvider");
						}
					},
					complete: function () {

					}
				});
			}


		}
	}
}

//TRADENAME VALIDATOR
function OnTradeNameValidation(s, e) {
	e.isValid = true;
	var itemTmp = id_identificationType.GetSelectedItem();
	if (itemTmp !== null) {
		if (itemTmp.value === 1) {
			if (e.value === null) {
				e.isValid = false;
				e.errorText = "Campo Obligatorio cuando es RUC";
			}
		}
	}
}

// PROVIDER VALIDATOR

function OnPaymentMethodValidation(s, e) {
	var tab = tabControl.GetTabByName("tabProvider");
	if (!tab.GetVisible()) {
		e.isValid = true;
	} else if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}

	UpdateTabImage(e, "tabProvider");
}

function OnPaymentMeanValidation(s, e) {
	var tab = tabControl.GetTabByName("tabProvider");
	if (!tab.GetVisible()) {
		e.isValid = true;
	} else if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}

	UpdateTabImage(e, "tabProvider");
}


// PROVIDER GENERAL DATA EP VALIDATOR

function OnIdentificationTypeEPValidation(s, e) {
	if (!electronicPayment.GetChecked()) {
		e.isValid = true;
	} else if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

function OnPaymentMethodEPValidation(s, e) {
	if (!electronicPayment.GetChecked()) {
		e.isValid = true;
	} else if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

function OnNoAccountEPValidation(s, e) {
	if (!electronicPayment.GetChecked()) {
		e.isValid = true;
	} else
		if (e.value !== null && e.value.toString().length > 20) {
			e.isValid = false;
			e.errorText = "Máximo 20 caracteres";
			UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
}

function OnNoRefTransferValidation(s, e) {
	if (!electronicPayment.GetChecked()) {
		e.isValid = true;
	} else
		if (e.value !== null && e.value.toString().length > 20) {
			e.isValid = false;
			e.errorText = "Máximo 20 caracteres";
			UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
}

function OnNoRouteValidation(s, e) {
	if (!electronicPayment.GetChecked()) {
		e.isValid = true;
	} else
		if (e.value !== null && e.value.toString().length > 20) {
			e.isValid = false;
			e.errorText = "Máximo 20 caracteres";
			UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
}

function OnEmailEPValidation(s, e) {
	if (!electronicPayment.GetChecked()) {
		e.isValid = true;
	} else
		if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo obligatorio";
			UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		} else
			if (e.value !== null) {
				var validation = null;
				validation = validarEMAIL(e.value);
				if (validation !== null && validation !== undefined && !validation.isValid) {
					e.isValid = validation.isValid;
					e.errorText = validation.errorText;
					UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
					UpdateTabImage(e, "tabProvider");
				}
			}
}

// PROVIDER GENERAL DATA RISE VALIDATOR

function OnCategoryRiseValidation(s, e) {
	if (!rise.GetChecked()) {
		e.isValid = true;
	} else if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

function OnActivityRiseValidation(s, e) {
	if (!rise.GetChecked()) {
		e.isValid = true;
	} else if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

// EMPLOYEE VALIDATOR

function OnEmployeeDepartmentValidation(s, e) {
	var tab = tabControl.GetTabByName("tabEmployee");
	if (!tab.GetVisible()) {
		e.isValid = true;
	} else if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}

	UpdateTabImage(e, "tabEmployee");
}

// FRAMEWORK CONTRACT

function OnCompanyValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnTypeContractFrameworkValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnRolValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnItemValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnStartDateValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var isValidAux = OnRangeDateValidation(e, e.value, endDateFrameworkContractItem.GetValue(), "Fecha de Inicio debe ser menor o igual a la Fecha Fin.");

	}
}

function OnEndDateValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var isValidAux = OnRangeDateValidation(e, startDateFrameworkContractItem.GetValue(), e.value, "Fecha de Fin debe ser mayor o igual a la Fecha Inicio.");
	}
}

function buyerLabelValidate(s, e) {
	var roles = id_roles.GetValue();
	var isBuyer = (roles.indexOf("7") !== -1);

	var value = s.GetValue();
	if (isBuyer && value == null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		e.isValid = true;
		e.errorText = "";
	}
}

function OnValueValidation(s, e) {
	var code_typeContractFramework = $("#code_typeContractFramework").val();
	if (code_typeContractFramework == "VA31" || code_typeContractFramework == "VA41V") {
		if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo Obligatorio";
		} else {
			var valueMin = 0.01;
			var valueMax = 9999999999999.99;
			var msgErrorAux = "Valor debe estar entre $0.01 y $9,999,999,999,999.99";
			OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
		}
	}

}

function OnAmoutValidation(s, e) {
	var code_typeContractFramework = $("#code_typeContractFramework").val();
	if (code_typeContractFramework == "VA31" || code_typeContractFramework == "VA41C") {
		if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo Obligatorio";
		} else {
			var valueMin = 0.01;
			var valueMax = 9999999999999.99;
			var msgErrorAux = "Cantidad debe estar entre 0.01 y 9,999,999,999,999.99";
			var valid = OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
			if (valid && code_typeContractFramework == "VA31") {

				var resAmoutFrameworkContractDeliveryPlan = "0";
				var id_frameworkContractDeliveryPlanAux = 0;
				if (PersonFrameworkContractDeliveryPlans.IsEditing()) {
					var amoutFrameworkContractDeliveryPlanAux = amoutFrameworkContractDeliveryPlan.GetValue();
					var strAmoutFrameworkContractDeliveryPlanAux = amoutFrameworkContractDeliveryPlanAux == null ? "0" : amoutFrameworkContractDeliveryPlanAux.toString();
					resAmoutFrameworkContractDeliveryPlan = strAmoutFrameworkContractDeliveryPlanAux.replace(".", ",");
					id_frameworkContractDeliveryPlanAux = PersonFrameworkContractDeliveryPlans.cpEditingRowID;
				}

				var amoutFrameworkContractItemAux = amoutFrameworkContractItem.GetValue();
				var strAmoutFrameworkContractItemAux = amoutFrameworkContractItemAux == null ? "0" : amoutFrameworkContractItemAux.toString();
				var resAmoutFrameworkContractItem = strAmoutFrameworkContractItemAux.replace(".", ",");

				$.ajax({
					url: "Person/PersonFrameworkContractItemValidateAmount",
					type: "post",
					data: {
						id_person: $("#id_person").val(),
						id_frameworkContract: $("#id_frameworkContract").val(),
						id_frameworkContractItem: $("#id_frameworkContractItem").val(),

						//id_item: id_itemFrameworkContractItem.GetValue(),
						amoutEdit: resAmoutFrameworkContractDeliveryPlan,
						amoutTotal: resAmoutFrameworkContractItem,
						id_frameworkContractDeliveryPlan: id_frameworkContractDeliveryPlanAux
					},
					async: false,
					cache: false,
					error: function (error) {
						console.log(error);
						hideLoading();
					},
					beforeSend: function () {
						showLoading();
					},
					success: function (result) {
						if (result !== null && result.itsValid == 0) {
							e.isValid = false;
							e.errorText = result.Error;
						}
					},
					complete: function () {
						hideLoading();
					}
				});
			}
		}

	}

}

function OnMetricUnitValidation(s, e) {
	var code_typeContractFramework = $("#code_typeContractFramework").val();
	if (code_typeContractFramework == "VA31" || code_typeContractFramework == "VA41C") {
		if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo Obligatorio";
		}
	}

}


function OnValueFrameworkContractExtensionValidation(s, e) {
	var code_typeContractFramework = $("#code_typeContractFramework").val();
	var createdFrameworkContractExtension = $("#createdFrameworkContractExtension").val();
	if (code_typeContractFramework == "VA41V" &&
		(createdFrameworkContractExtension == true || createdFrameworkContractExtension == "True" || createdFrameworkContractExtension == "true")) {
		if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo Obligatorio";
		} else {
			var valueMin = 0.01;
			var valueMax = 9999999999999.99;
			var msgErrorAux = "Valor debe estar entre $0.01 y $9,999,999,999,999.99";
			OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
		}
	}
}

function OnAmoutFrameworkContractExtensionValidation(s, e) {
	var code_typeContractFramework = $("#code_typeContractFramework").val();
	var createdFrameworkContractExtension = $("#createdFrameworkContractExtension").val();
	if (code_typeContractFramework == "VA41C" &&
		(createdFrameworkContractExtension == true || createdFrameworkContractExtension == "True" || createdFrameworkContractExtension == "true")) {
		if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo Obligatorio";
		} else {
			var valueMin = 0.01;
			var valueMax = 9999999999999.99;
			var msgErrorAux = "Cantidad debe estar entre 0.01 y 9,999,999,999,999.99";
			OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
		}
	}

}

function OnDeliveryPlanDateValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		if (startDateFrameworkContractItem.GetValue() === null || endDateFrameworkContractItem.GetValue() === null) {
			e.isValid = false;
			e.errorText = "Fecha de Inicio y Fecha Fin deben estar definidos antes del plan de entrega.";
		} else {
			var isValidAux = OnRangeDateValidation(e, startDateFrameworkContractItem.GetValue(), e.value, "Fecha del Plan de Entrega debe ser mayor o igual a la Fecha de Inicio.");
			isValidAux = OnRangeDateValidation(e, e.value, endDateFrameworkContractItem.GetValue(), "Fecha del Plan de Entrega debe ser menor o igual a la Fecha Fin.");
		}
	}
}

function OnAmoutFrameworkContractDeliveryPlanValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var valueMin = 0.01;
		var valueMax = 9999999999999.99;
		var msgErrorAux = "Cantidad debe estar entre 0.01 y 9,999,999,999,999.99";
		var valid = OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
		if (valid) {

			var amoutAux = e.value;
			var strAmout = amoutAux == null ? "0" : amoutAux.toString();
			var resAmout = strAmout.replace(".", ",");

			var amoutFrameworkContractItemAux = amoutFrameworkContractItem.GetValue();
			var strAmoutFrameworkContractItemAux = amoutFrameworkContractItemAux == null ? "0" : amoutFrameworkContractItemAux.toString();
			var resAmoutFrameworkContractItem = strAmoutFrameworkContractItemAux.replace(".", ",");

			$.ajax({
				url: "Person/PersonFrameworkContractDeliveryPlanValidateAmount",
				type: "post",
				data: {
					id_person: $("#id_person").val(),
					id_frameworkContract: $("#id_frameworkContract").val(),
					id_frameworkContractItem: $("#id_frameworkContractItem").val(),
					amout: resAmout,
					amoutTotal: resAmoutFrameworkContractItem,
					id_frameworkContractDeliveryPlan: PersonFrameworkContractDeliveryPlans.cpEditingRowID
				},
				async: false,
				cache: false,
				error: function (error) {
					console.log(error);
					hideLoading();
				},
				beforeSend: function () {
					showLoading();
				},
				success: function (result) {
					if (result !== null && result.itsValid == 0) {
						e.isValid = false;
						e.errorText = result.Error;
					}
				},
				complete: function () {
					hideLoading();
				}
			});
		}
	}
}

//PROVIDER TYPE SHRIMP
function OnGrammageUpToValidation_selectedIndexChanged(s, e) {
	grammageFrom.Validate();
}

function OnGrammageFromToValidation_selectedIndexChanged(s, e) {
	grammageUpto.Validate();
}

function OnGrammageUpToValidation(s, e) {
	e.isValid = true;
	e.errorText = "";
	var controls = ASPxClientControl.GetControlCollection();

	var objectControlUpto = controls.GetByName("grammageUpto");
	var objectControlUpto = controls.GetByName("grammageUpto");
	var grammaUpToTmp = objectControlUpto.GetValue();

	var objectControl = controls.GetByName("grammageFrom");
	var grammageFromTmp = objectControl.GetValue();

	if (grammaUpToTmp != null) {
		if (!(grammageFromTmp == undefined || grammageFromTmp == "")) {
			if (s.GetSelectedItem() != undefined && objectControl.GetSelectedItem() != undefined) {
				if (parseInt(s.GetSelectedItem().texts[1]) < parseInt(objectControl.GetSelectedItem().texts[1])) {
					e.isValid = false;
					e.errorText = "Gramaje Hasta debe ser mayor que gramaje Desde"
				}
			}
		}
	}

}

function OnGrammageFromValidation(s, e) {
	e.isValid = true;
	e.errorText = "";
	var errorMessageTmp = "";
	var controls = ASPxClientControl.GetControlCollection();

	var objectControlFrom = controls.GetByName("grammageFrom");
	var grammageFromTmp = objectControlFrom.GetValue();

	var objectControl = controls.GetByName("grammageUpto");
	var grammaUpToTmp = objectControl.GetValue();

	if (grammageFromTmp != null) {
		if (!(grammaUpToTmp == undefined || grammaUpToTmp == "")) {
			if (s.GetSelectedItem() != undefined && objectControl.GetSelectedItem() != undefined) {
				if (parseInt(s.GetSelectedItem().texts[1]) > parseInt(objectControl.GetSelectedItem(grammaUpToTmp).texts[1])) {
					e.isValid = false;
					e.errorText = "Gramaje Desde debe ser menor que gramaje Hasta"
				}
			}
		}
	}
}

function OnANTEspecification(s, e) {
	// 
	var isProviderShrimp = document.getElementById("isProviderShrimpBit").value;
	var isProviderTransportist = document.getElementById("isProviderTransportistBit").value;
	if (isProviderTransportist == "SI") {
		if (e.value == null) {
			e.isValid = false;
			e.errorText = "Campo Obligatorio"
			UpdateTabControlImage(e, "tabTransportist", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
	}
}