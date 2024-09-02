
function OnGridViewPersonRolsBeginCallback(s, e) {

	var values = id_roles.GetValue();

	var roles = values.split(",");

	e.customArgs['rols'] = roles;
	e.customArgs['id_person'] = $("#id_person").val();
	//
}

function tabControlProviderInit(s, e) {
	var rol = $("#rolExistente").val();
	var copacking = isCopacking.GetValue();
	var rolExistent = false;
	if (rol === "True" || copacking) {
		rolExistent = true;
	}
	tabControlProvider.GetTabByName("tabCopacking").SetVisible(rolExistent);
}

function OnClickUpdatePersonRol(s, e) {
	gvPersonRols.GetRowValues(e.visibleIndex, "name", OnGetRowValue);
	//console.log(nameAux); 

	//var data = {
	//    id: gvProductionLotReceptions.GetRowKey(e.visibleIndex)
	//};

	//showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
}

function OnGetRowValue(value) {
	//var nameAux = 
	//console.log(values[0]);
	if (value == "Cliente Exterior") {
		//showThickBox("ForeignCustomer/PopupControlRolForeignCustomer", { id_person: $("#id_person").val() });
        $.ajax({
            url: "ForeignCustomer/PopupControlRolForeignCustomer",
            type: "post",
            data: { id_person: $("#id_person").val() },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                //$.fancybox.center(true);
                $.fancybox(result
                    , {
                        //scrolling: false,
                        //padding: 0 auto,
                        //top:'25px',
                        width: '700px',
                        //'autoScale': false,
                        // width: '',
                        height: '1200px',
                        //overflow: scroll,
                        'scrolling': 'no',
                        //'autoScale': false,
                        //'transitionIn': 'none',
                        //'transitionOut': 'none',
                        //'type': 'html',
                        ////scrolling: true,
                        ////width: widthAux,
                        ////height: heightAux,
                        //padding:'10px',
                        modal: true,
                        //autoDimensions: false,

                    }
                );
                
                //popupControlRolForeignCustomer.SetContentHTML(result);
                //$("#div_popupControlRolForeignCustomer").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
        //popupControlRolForeignCustomer.Show();
	}
}

//FrameworkContract

function FrameworkContractCompanyCombo_SelectedIndexChanged(s, e) {

	id_typeContractFramework.ClearItems();
	$("#id_companyFrameworkContract").val(s.GetValue());

	$.ajax({
		url: "Person/PersonFrameworkContractCompanyDetailData",
		type: "post",
		data: {
			id_company: s.GetValue(),
			id_frameworkContract: $("#id_frameworkContract").val()
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_typeContractFramework, result.typeContractFrameworks, arrayFieldStr);
				PersonFrameworkContractItems.PerformCallback();
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function FrameworkContractTypeContractFrameworkCombo_SelectedIndexChanged(s, e) {


	$.ajax({
		url: "Person/PersonFrameworkContractTypeContractFrameworkDetailData",
		type: "post",
		data: {
			id_typeContractFramework: s.GetValue(),
			id_person: $("#id_person").val(),
			id_frameworkContract: $("#id_frameworkContract").val()
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				$("#code_typeContractFramework").val(result.code_typeContractFramework);

				if (PersonFrameworkContractItems.IsEditing()) {
					PersonFrameworkContractItems.CancelEdit();
				} else {
					PersonFrameworkContractItems.PerformCallback();
				}
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

var rolesFrameworkContractAux = [];

function FrameworkContractRolCombo_Init(s, e) {

	var values = id_roles.GetValue();

	var roles = values.split(",");

	rolesFrameworkContractAux = [];

	var isProvider = (roles.indexOf("1") !== -1);

	if (isProvider) {
		rolesFrameworkContractAux.push({ id: 1, name: "Proveedor" });
	}

	var isCustomerLocal = (roles.indexOf("3") !== -1);

	if (isCustomerLocal) {
		rolesFrameworkContractAux.push({ id: 3, name: "Cliente Local" });
	}

	var isCustomerExterior = (roles.indexOf("6") !== -1);
	if (isCustomerExterior) {
		rolesFrameworkContractAux.push({ id: 6, name: "Cliente Exterior" });
	}

	var into = false;
	for (var j = 0; j < rolesFrameworkContractAux.length; j++) {
		if (s.GetValue() == rolesFrameworkContractAux[j].id || s.GetValue() == null) {
			into = true;
			break;
		}
	}
	if (!into) {
		rolesFrameworkContractAux.push({ id: s.GetValue(), name: s.GetText() });
	}

	var arrayFieldStr = [];
	arrayFieldStr.push("name");
	UpdateDetailObjects(id_rol, rolesFrameworkContractAux, arrayFieldStr);
}

function FrameworkContractRolCombo_SelectedIndexChanged(s, e) {

	$.ajax({
		url: "Person/PersonFrameworkContractRolDetailData",
		type: "post",
		data: {
			id_person: $("#id_person").val(),
			id_frameworkContract: $("#id_frameworkContract").val()
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				//if (PersonFrameworkContractItems.IsEditing()) {

				//}
				PersonFrameworkContractItems.PerformCallback();
				$("#id_rolFrameworkContract").val(s.GetValue());
			}
		},
		complete: function () {
			hideLoading();
		}
	});



}

function InitCopackingDetail(s, e) {
	isCopackingDetail.GetChecked();
}

function OnItem_Init(s, e) {

	//$("#id_companyFrameworkContract").val();
	//$("#providerCustomer").val();

	$.ajax({
		url: "Person/PersonFrameworkContractItemDetailData",
		type: "post",
		data: {
			id_company: $("#id_companyFrameworkContract").val(),
			id_rol: $("#id_rolFrameworkContract").val(),
			id_item: s.GetValue(),
			id_metricUnit: id_metricUnitFrameworkContractItem.GetValue()
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("masterCode");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_itemFrameworkContractItem, result.items, arrayFieldStr);
				OnValueAmout_Init();

				arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_metricUnitFrameworkContractItem, result.metricUnits, arrayFieldStr);
				//if(id_metricUnitFrameworkContractItem.GetValue() == null)
				//{
				//    id_metricUnitFrameworkContractItem.SetValue(result.id_metricUnit);
				//}

			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function FrameworkContractItemCombo_SelectedIndexChanged(s, e) {

	id_metricUnitFrameworkContractItem.ClearItems();
	//console.log("id_item: " + s.GetValue());
	//console.log("id_rol: " + $("#id_rolFrameworkContract").val());
	$.ajax({
		url: "Person/PersonFrameworkContractItemChangedDetailData",
		type: "post",
		data: {
			id_item: s.GetValue(),
			id_rol: $("#id_rolFrameworkContract").val()
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_metricUnitFrameworkContractItem, result.metricUnits, arrayFieldStr);
				//id_metricUnitFrameworkContractItem.SetValue(result.id_metricUnit);
			}
		},
		complete: function () {
			hideLoading();
		}
	});

}

function OnValueAmout_Init() {

	var code_typeContractFramework = $("#code_typeContractFramework").val();
	if (code_typeContractFramework == "VA31") {
		SetElementVisibility("valueLabelFrameworkContractItem", true);
		SetElementVisibility("valueFrameworkContractItem", true);

		SetElementVisibility("amoutLabelFrameworkContractItem", true);
		SetElementVisibility("amoutFrameworkContractItem", true);
		SetElementVisibility("metricUnitFrameworkContractItem", true);
		SetElementVisibility("id_metricUnitFrameworkContractItem", true);

		//frameworkContractDeliveryPlanTable
		SetElementVisibility("frameworkContractDeliveryPlanTable", true);
		SetElementVisibility("valueFrameworkContractExtensionTable", false);
		SetElementVisibility("amoutFrameworkContractExtensionTable", false);
		SetElementVisibility("btnAddFrameworkContract", false);
	}

	if (code_typeContractFramework == "VA41V") {
		SetElementVisibility("valueLabelFrameworkContractItem", true);
		SetElementVisibility("valueFrameworkContractItem", true);

		SetElementVisibility("amoutLabelFrameworkContractItem", false);
		SetElementVisibility("amoutFrameworkContractItem", false);
		SetElementVisibility("metricUnitFrameworkContractItem", false);
		SetElementVisibility("id_metricUnitFrameworkContractItem", false);

		var createdFrameworkContractExtension = $("#createdFrameworkContractExtension").val();
		if (createdFrameworkContractExtension == true || createdFrameworkContractExtension == "True" || createdFrameworkContractExtension == "true") {
			SetElementVisibility("valueFrameworkContractExtensionTable", true);
			//SetElementVisibility("btnAddFrameworkContract", false);
		} else {
			SetElementVisibility("valueFrameworkContractExtensionTable", false);
			//SetElementVisibility("btnAddFrameworkContract", true);
		}
		SetElementVisibility("amoutFrameworkContractExtensionTable", false);
		SetElementVisibility("frameworkContractDeliveryPlanTable", false);
	}

	if (code_typeContractFramework == "VA41C") {
		SetElementVisibility("amoutLabelFrameworkContractItem", true);
		SetElementVisibility("amoutFrameworkContractItem", true);
		SetElementVisibility("metricUnitFrameworkContractItem", true);
		SetElementVisibility("id_metricUnitFrameworkContractItem", true);

		SetElementVisibility("valueLabelFrameworkContractItem", false);
		SetElementVisibility("valueFrameworkContractItem", false);
		if (createdFrameworkContractExtension == true || createdFrameworkContractExtension == "True" || createdFrameworkContractExtension == "true") {
			SetElementVisibility("amoutFrameworkContractExtensionTable", true);
			//SetElementVisibility("btnAddFrameworkContract", false);
		} else {
			SetElementVisibility("amoutFrameworkContractExtensionTable", false);
			//SetElementVisibility("btnAddFrameworkContract", true);
		}
		SetElementVisibility("valueFrameworkContractExtensionTable", false);
		SetElementVisibility("frameworkContractDeliveryPlanTable", false);

	}

}

function ButtonAddFrameworkContract_Click(s, e) {

	//id_itemFrameworkContractItem.SetEnabled(false);
	//startDateFrameworkContractItem.SetEnabled(false);
	//endDateFrameworkContractItem.SetEnabled(false);
	var code_typeContractFramework = $("#code_typeContractFramework").val();
	if (code_typeContractFramework == "VA41V") {
		//valueFrameworkContractItem.SetEnabled(false);
		SetElementVisibility("valueFrameworkContractExtensionTable", true);
	}
	if (code_typeContractFramework == "VA41C") {
		//amoutFrameworkContractItem.SetEnabled(false);
		SetElementVisibility("amoutFrameworkContractExtensionTable", true);
	}
	btnAddFrameworkContract.SetEnabled(false);
	$("#createdFrameworkContractExtension").val(true);
}


function ButtonUpdateFrameworkContract_Click(s, e) {
	var valid = true;

	var validFrameworkContractTable = ASPxClientEdit.ValidateEditorsInContainerById("frameworkContractTable", null, true);

	if (validFrameworkContractTable) {
		UpdateTabImage({ isValid: true }, "tabFrameworkContract");
	} else {
		UpdateTabImage({ isValid: false }, "tabFrameworkContract");
		valid = false;
	}

	if (PersonFrameworkContractItems.cpRowsCount === 0 || PersonFrameworkContractItems.IsEditing()) {
		UpdateTabImage({ isValid: false }, "tabFrameworkContract");
		valid = false;
	} else {
		if (valid) {
			UpdateTabImage({ isValid: true }, "tabFrameworkContract");
		}
	}

	if (valid) {
		PersonFrameworkContracts.UpdateEdit();

	}
}

function BtnCancelFrameworkContract_Click(s, e) {
	PersonFrameworkContracts.CancelEdit();
}


function ButtonUpdateFrameworkContractItem_Click(s, e) {
	var valid = true;
	var validFrameworkContractItemTable = ASPxClientEdit.ValidateEditorsInContainerById("frameworkContractItemTable", null, true);

	if (validFrameworkContractItemTable) {
		UpdateTabImage({ isValid: true }, "tabFrameworkContract");
	} else {
		UpdateTabImage({ isValid: false }, "tabFrameworkContract");
		valid = false;
	}

	var createdFrameworkContractExtension = $("#createdFrameworkContractExtension").val();
	if (createdFrameworkContractExtension == true || createdFrameworkContractExtension == "True" || createdFrameworkContractExtension == "true") {

		var code_typeContractFramework = $("#code_typeContractFramework").val();
		if (code_typeContractFramework == "VA41V") {
			var validValueFrameworkContractExtensionTable = ASPxClientEdit.ValidateEditorsInContainerById("valueFrameworkContractExtensionTable", null, true);

			if (validValueFrameworkContractExtensionTable && valid) {
				UpdateTabImage({ isValid: true }, "tabFrameworkContract");
			} else {
				UpdateTabImage({ isValid: false }, "tabFrameworkContract");
				valid = false;
			}
		} else {
			if (code_typeContractFramework == "VA41C") {
				var validAmoutFrameworkContractExtensionTable = ASPxClientEdit.ValidateEditorsInContainerById("amoutFrameworkContractExtensionTable", null, true);

				if (validAmoutFrameworkContractExtensionTable && valid) {
					UpdateTabImage({ isValid: true }, "tabFrameworkContract");
				} else {
					UpdateTabImage({ isValid: false }, "tabFrameworkContract");
					valid = false;
				}
			}
		}

	}

	var code_typeContractFramework = $("#code_typeContractFramework").val();
	if (code_typeContractFramework == "VA31") {

		if (PersonFrameworkContractDeliveryPlans.cpRowsCount === 0 || PersonFrameworkContractDeliveryPlans.IsEditing()) {
			UpdateTabImage({ isValid: false }, "tabFrameworkContract");
			valid = false;
		} else {
			if (valid) {
				UpdateTabImage({ isValid: true }, "tabFrameworkContract");
			}
		}
	}

	if (valid) {
		PersonFrameworkContractItems.UpdateEdit();

	}
}

function BtnCancelFrameworkContractItem_Click(s, e) {
	PersonFrameworkContractItems.CancelEdit();
}



// ProviderAll
var id_companyIniAux = 0;
var id_divisionIniAux = 0;
var id_branchOfficeIniAux = 0;


//ProviderAccountingAccounts

function ProviderAccountingAccountsCompanyCombo_Init(s, e) {

	//id_companyIniAux = s.GetValue();
	//id_divisionIniAux = id_division.GetValue();
	//id_branchOfficeIniAux = id_branchOffice.GetValue();

	var data = {
		id_company: s.GetValue(),
		id_division: id_division.GetValue(),
		id_branchOffice: id_branchOffice.GetValue(),
		id_accountFor: id_accountFor.GetValue(),
		id_accountPlan: id_accountPlan.GetValue(),
		id_account: id_account.GetValue(),
		id_accountingAssistantDetailType: id_accountingAssistantDetailType.GetValue()
	};

	$.ajax({
		url: "Person/InitProviderAccountingAccountsCompanyCombo",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_company
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_company, result.companies, arrayFieldStr);

			//id_division
			UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);

			//id_branchOffice
			UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);

			//id_accountPlan
			UpdateDetailObjects(id_accountPlan, result.accountPlans, arrayFieldStr);

			//id_accountFor
			arrayFieldStr = [];
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_accountFor, result.accountFors, arrayFieldStr);

			//id_account
			var arrayFieldStr = [];
			arrayFieldStr.push("number");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_account, result.accounts, arrayFieldStr);

			//id_accountingAssistantDetailType
			var arrayFieldStr = [];
			arrayFieldStr.push("assistantTypeCode");
			arrayFieldStr.push("accountingAssistantCode");
			arrayFieldStr.push("accountingAssistantName");
			UpdateDetailObjects(id_accountingAssistantDetailType, result.accountingAssistantDetailTypes, arrayFieldStr);

		},
		complete: function () {
			//hideLoading();
		}
	});
}

function ProviderAccountingAccountsAccountPlanCombo_SelectedIndexChanged(s, e) {

	id_account.ClearItems();
	id_accountingAssistantDetailType.ClearItems();

	$.ajax({
		url: "Person/ProviderAccountingAccountsAccountPlanDetailData",
		type: "post",
		data: { id_accountPlan: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("number");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_account, result.accounts, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function ProviderAccountingAccountsAccountCombo_SelectedIndexChanged(s, e) {

	id_accountingAssistantDetailType.ClearItems();

	$.ajax({
		url: "Person/ProviderAccountingAccountsAccountDetailData",
		type: "post",
		data: { id_account: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("assistantTypeCode");
				arrayFieldStr.push("accountingAssistantCode");
				arrayFieldStr.push("accountingAssistantName");
				UpdateDetailObjects(id_accountingAssistantDetailType, result.accountingAssistantDetailTypes, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

//ProviderMailByComDivBra

function OnProviderMailByComDivBraEmailValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else
		if (e.value !== null) {
			var validation = null;
			validation = validarEMAIL(e.value);
			if (validation !== null && validation !== undefined && !validation.isValid) {
				e.isValid = validation.isValid;
				e.errorText = validation.errorText;
				UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
				UpdateTabImage(e, "tabProvider");
			}
		}
}

//ProviderItem

var id_itemIniAux = 0;

function OnProviderItemItemComboValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var data = {
			id_itemNew: id_item.GetValue()
		};
		if (data.id_itemNew != id_itemIniAux) {
			$.ajax({
				url: "Person/ItsRepeatedItem",
				type: "post",
				data: data,
				async: false,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					//showLoading();
				},
				success: function (result) {
					if (result !== null) {
						if (result.itsRepeated == 1) {
							e.isValid = false;
							e.errorText = result.Error;
							UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
							UpdateTabImage(e, "tabProvider");
						} else {
							id_itemIniAux = 0;
						}
					}
				},
				complete: function () {
					//hideLoading();
				}
			});
		}

	}
}

function ProviderItemItemCombo_Init(s, e) {

	id_itemIniAux = s.GetValue();

	var data = {
		id_item: s.GetValue()
	};

	$.ajax({
		url: "Person/InitProviderItemItemCombo",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_company
			var arrayFieldStr = [];
			arrayFieldStr.push("masterCode");
			arrayFieldStr.push("barCode");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_item, result.items, arrayFieldStr);

		},
		complete: function () {
			//hideLoading();
		}
	});
}

//ProviderRelatedCompany

function ProviderRelatedCompanyCompanyCombo_Init(s, e) {

	id_companyIniAux = s.GetValue();
	id_divisionIniAux = id_division.GetValue();
	id_branchOfficeIniAux = id_branchOffice.GetValue();

	var data = {
		id_company: s.GetValue(),
		id_division: id_division.GetValue(),
		id_branchOffice: id_branchOffice.GetValue()
	};

	$.ajax({
		url: "Person/InitProviderPaymentTermCompanyCombo",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_company
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_company, result.companies, arrayFieldStr);

			//id_division
			//arrayFieldStr = [];
			//arrayFieldStr.push("code");
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);

			//id_branchOffice
			//arrayFieldStr = [];
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);

		},
		complete: function () {
			//hideLoading();
		}
	});
}

function ProviderRelatedCompanyCompanyCombo_SelectedIndexChanged(s, e) {

	id_division.ClearItems();
	id_branchOffice.ClearItems();

	$.ajax({
		url: "Person/ProviderPaymentTermCompanyDetailData",
		type: "post",
		data: { id_company: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function ProviderRelatedCompanyDivisionCombo_SelectedIndexChanged(s, e) {

	id_branchOffice.ClearItems();

	$.ajax({
		url: "Person/ProviderPaymentTermDivisionDetailData",
		type: "post",
		data: { id_company: id_company.GetValue(), id_division: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function OnProviderRelatedCompanyBranchOfficeComboValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var data = {
			id_companyNew: id_company.GetValue(),
			id_divisionNew: id_division.GetValue(),
			id_branchOfficeNew: id_branchOffice.GetValue()
		};
		if (data.id_companyNew != id_companyIniAux || data.id_divisionNew != id_divisionIniAux ||
			data.id_branchOfficeNew != id_branchOfficeIniAux) {
			$.ajax({
				url: "Person/ItsRepeatedRelatedCompany",
				type: "post",
				data: data,
				async: false,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					//showLoading();
				},
				success: function (result) {
					if (result !== null) {
						if (result.itsRepeated == 1) {
							e.isValid = false;
							e.errorText = result.Error;
							UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
							UpdateTabImage(e, "tabProvider");
						} else {
							id_companyIniAux = 0;
							id_divisionIniAux = 0;
							id_branchOfficeIniAux = 0;
						}
					}
				},
				complete: function () {
					//hideLoading();
				}
			});
		}

	}
}

//ProviderPersonAuthorizedToPayTheBill

function OnProviderPersonAuthorizedToPayTheBillIdentificationNumberComboValidation(s, e) {

	var item = id_identificationTypeProviderPersonAuthorizedToPayTheBill.GetSelectedItem();

	if (item === null) {
		e.isValid = false;
		e.errorText = "Seleccione el ID.";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var validation = null;

		if (item.value === 2) {
			validation = validarCI(e.value);
		} else if (item.value === 1) {
			validation = validarRUC(e.value);
		}

		if (validation !== null && validation !== undefined && !validation.isValid) {
			e.isValid = validation.isValid;
			e.errorText = validation.errorText;
			if (validation.isValid == false) {
				UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
				UpdateTabImage(e, "tabProvider");
			}
		}
	}

}

function OnProviderPersonAuthorizedToPayTheBillPhoneNumber1Validation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else
		if (e.value !== null) {
			var validation = null;
			validation = validarPhoneNumber(e.value);
			if (validation !== null && validation !== undefined && !validation.isValid) {
				e.isValid = validation.isValid;
				e.errorText = validation.errorText;
				UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
				UpdateTabImage(e, "tabProvider");
			}
		}
}

function OnProviderPersonAuthorizedToPayTheBillPhoneNumber2Validation(s, e) {
	if (e.value !== null) {
		var validation = null;
		validation = validarPhoneNumber(e.value);
		if (validation !== null && validation !== undefined && !validation.isValid) {
			e.isValid = validation.isValid;
			e.errorText = validation.errorText;
			UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
	}
}

function OnProviderPersonAuthorizedToPayTheBillCodeValidation(s, e) {
	if (e.value !== null && e.value.toString().length > 5) {
		e.isValid = false;
		e.errorText = "Máximo 5 caracteres";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

function ProviderPersonAuthorizedToPayTheBillIdentificationTypeCombo_Init(s, e) {

	//id_retentionIniAux = id_retention.GetValue();

	var data = {
		id_identificationType: id_identificationTypeProviderPersonAuthorizedToPayTheBill.GetValue(),
		id_country: id_country.GetValue(),
		id_bank: id_bank.GetValue(),
		id_accountType: id_accountType.GetValue()
	};

	$.ajax({
		url: "Person/InitProviderPersonAuthorizedToPayTheBillIdentificationTypeCombo",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_company
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_identificationTypeProviderPersonAuthorizedToPayTheBill, result.identificationTypes, arrayFieldStr);

			//id_division
			//arrayFieldStr = [];
			//arrayFieldStr.push("code");
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_country, result.countries, arrayFieldStr);

			//id_branchOffice
			//arrayFieldStr = [];
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_bank, result.banks, arrayFieldStr);


			UpdateDetailObjects(id_accountType, result.accountTypes, arrayFieldStr);


		},
		complete: function () {
			//hideLoading();
		}
	});
}

function OnProviderPersonAuthorizedToPayTheBillAmountValidation(s, e) {
	if (e.value !== null) {
		if (parseFloat(e.value) < 0) {
			e.isValid = false;
			e.errorText = "Monto debe ser mayor e igual a cero o nulo";
			UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
	}
}

function OnProviderPersonAuthorizedToPayTheBillNoPaymentsValidation(s, e) {
	if (e.value !== null) {
		if (parseFloat(e.value) < 0) {
			e.isValid = false;
			e.errorText = "Número de Pagos debe ser mayor e igual a cero o nulo";
			UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
	}
}

//ProviderRetention
var id_retentionIniAux = false;

function OnProviderRetentionComboValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var data = {
			id_retentionNew: id_retention.GetValue()
		};
		if (data.id_retentionNew != id_retentionIniAux) {

			$.ajax({
				url: "Person/ItsRepeatedRetention",
				type: "post",
				data: data,
				async: false,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					//showLoading();
				},
				success: function (result) {
					if (result !== null) {
						if (result.itsRepeated == 1) {
							e.isValid = false;
							e.errorText = result.Error;
							UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
							UpdateTabImage(e, "tabProvider");
						} else {
							id_retentionIniAux = 0;
						}
					}
				},
				complete: function () {
					//hideLoading();
				}
			});
		}

	}
}

function ProviderRetentionRetentionTypeCombo_Init(s, e) {

	id_retentionIniAux = id_retention.GetValue();

	var data = {
		id_retentionType: s.GetValue(),
		id_retentionGroup: id_retentionGroup.GetValue(),
		id_retention: id_retention.GetValue()
	};

	$.ajax({
		url: "Person/InitProviderRetentionRetentionTypeCombo",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_company
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_retentionType, result.retentionTypes, arrayFieldStr);

			//id_division
			//arrayFieldStr = [];
			//arrayFieldStr.push("code");
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_retentionGroup, result.retentionGroups, arrayFieldStr);

			//id_branchOffice
			//arrayFieldStr = [];
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_retention, result.retentions, arrayFieldStr);

		},
		complete: function () {
			//hideLoading();
		}
	});
}

function ProviderRetentionRetentionTypeCombo_SelectedIndexChanged(s, e) {
	UpdateProviderRetentionRetentionCombo({ id_retentionType: s.GetValue(), id_retentionGroup: id_retentionGroup.GetValue() });
}

function ProviderRetentionRetentionGroupCombo_SelectedIndexChanged(s, e) {
	UpdateProviderRetentionRetentionCombo({ id_retentionType: id_retentionType.GetValue(), id_retentionGroup: s.GetValue() });
}

function UpdateProviderRetentionRetentionCombo(data) {

	id_retention.ClearItems();
	percentRetencion.SetValue(0);

	$.ajax({
		url: "Person/UpdateProviderRetentionRetentionCombo",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_retention, result.retentions, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function ProviderRetentionRetentionCombo_SelectedIndexChanged(s, e) {

	$.ajax({
		url: "Person/ProviderRetentionRetentionDetailData",
		type: "post",
		data: { id_retention: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				percentRetencion.SetValue(result.percentRetencion);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

//ProviderSeriesForDocuments

function OnProviderInitialNumberValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else if (parseFloat(e.value) < 0) {
		e.isValid = false;
		e.errorText = "Número Inicial debe ser mayor e igual a cero";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var finalNumberAux = finalNumber.GetValue();
		finalNumberAux = finalNumberAux == null ? "0" : finalNumberAux;
		if (parseFloat(e.value) > parseFloat(finalNumberAux)) {
			e.isValid = false;
			e.errorText = "Número Inicial no puede ser mayor que el Número Final";
			UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		} else {
			var currentNumberAux = currentNumber.GetValue();
			currentNumberAux = currentNumberAux == null ? "0" : currentNumberAux;
			if (parseFloat(e.value) > parseFloat(currentNumberAux)) {
				e.isValid = false;
				e.errorText = "Número Inicial no puede ser mayor que el Número Actual";
				UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
				UpdateTabImage(e, "tabProvider");
			}
		}
	}
}

function OnProviderFinalNumberValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else if (parseFloat(e.value) <= 0) {
		e.isValid = false;
		e.errorText = "Número Final debe ser mayor que cero";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var initialNumberAux = initialNumber.GetValue();
		initialNumberAux = initialNumberAux == null ? "0" : initialNumberAux;
		if (parseFloat(e.value) <= parseFloat(initialNumberAux)) {
			e.isValid = false;
			e.errorText = "Número Final no puede ser menor o igual que el Número Inicial";
			UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		} else {
			var currentNumberAux = currentNumber.GetValue();
			currentNumberAux = currentNumberAux == null ? "0" : currentNumberAux;
			if (parseFloat(e.value) < parseFloat(currentNumberAux)) {
				e.isValid = false;
				e.errorText = "Número Final no puede ser menor que el Número Actual";
				UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
				UpdateTabImage(e, "tabProvider");
			}
		}
	}
}

function OnProviderCurrentNumberValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else if (parseFloat(e.value) < 0) {
		e.isValid = false;
		e.errorText = "Número Actual debe ser mayor e igual a cero";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var initialNumberAux = initialNumber.GetValue();
		initialNumberAux = initialNumberAux == null ? "0" : initialNumberAux;
		if (parseFloat(e.value) < parseFloat(initialNumberAux)) {
			e.isValid = false;
			e.errorText = "Número Actual no puede ser menor que el Número Inicial";
			UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		} else {
			var finalNumberAux = finalNumber.GetValue();
			finalNumberAux = finalNumberAux == null ? "0" : finalNumberAux;
			if (parseFloat(e.value) > parseFloat(finalNumberAux)) {
				e.isValid = false;
				e.errorText = "Número Actual no puede ser mayor que el Número Final";
				UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
				UpdateTabImage(e, "tabProvider");
			}
		}
	}
}

function ProviderSeriesForDocumentsDocumentTypeCombo_Init(s, e) {

	var data = {
		id_documentType: s.GetValue(),
		id_retentionSeriesForDocumentsType: id_retentionSeriesForDocumentsType.GetValue()
	};

	$.ajax({
		url: "Person/InitProviderSeriesForDocumentsDocumentTypeCombo",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_documentType
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_documentType, result.documentTypes, arrayFieldStr);

			//id_retentionSeriesForDocumentsType
			//arrayFieldStr = [];
			//arrayFieldStr.push("code");
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_retentionSeriesForDocumentsType, result.retentionSeriesForDocumentsTypes, arrayFieldStr);

		},
		complete: function () {
			//hideLoading();
		}
	});
}

if (!String.prototype.padStart) {
	String.prototype.padStart = function padStart(targetLength, padString) {
		targetLength = targetLength >> 0; //floor if number or convert non-number to 0;
		padString = String(padString || ' ');
		if (this.length > targetLength) {
			return String(this);
		}
		else {
			targetLength = targetLength - this.length;
			if (targetLength > padString.length) {
				padString += padString.repeat(targetLength / padString.length); //append to original to ensure we are longer than needed
			}
			return padString.slice(0, targetLength) + String(this);
		}
	};
}

function OnDateOfExpiryValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var ahora = new Date();
		var dateOfExpiryAux = s.GetValue();

		var dateOfExpiryMonth = (dateOfExpiryAux.getMonth() + 1).toString();
		var dateOfExpiryDay = dateOfExpiryAux.getDate().toString();
		var dateOfExpiryAux2 = dateOfExpiryAux.getFullYear().toString() + dateOfExpiryMonth.padStart(2, "0") + dateOfExpiryDay.padStart(2, "0");
		//console.log("dateOfExpiry: " + dateOfExpiryAux2);
		var ahoraDateMonth = (ahora.getMonth() + 1).toString();
		var ahoraDateDay = ahora.getDate().toString();
		var ahoraDate = ahora.getFullYear().toString() + ahoraDateMonth.padStart(2, "0") + ahoraDateDay.padStart(2, "0");
		//console.log("ahoraDate: " + ahoraDate);

		if (dateOfExpiryAux2 <= ahoraDate) {
			e.isValid = false;
			e.errorText = "La Fecha de Caducidad debe ser mayor que la Fecha Actual";
			UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");

		}
	}



}


// ProviderPaymentTermMethod
var isPredeterminedIniAux = false;
var isActiveIniAux = false;

// ProviderPaymentMethod

var id_paymentMethodIniAux = 0;

function OnProviderPaymentMethodPaymentMethodComboValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var data = {
			id_companyNew: id_companyPM.GetValue(),
			id_divisionNew: id_divisionPM.GetValue(),
			id_branchOfficeNew: id_branchOfficePM.GetValue(),
			id_paymentMethodNew: id_paymentMethod.GetValue(),
			isPredeterminedNew: isPredeterminedPM.GetValue(),
			onlyBecauseIsPredetermined: false
		};
		if (data.id_companyNew != id_companyIniAux || data.id_divisionNew != id_divisionIniAux ||
			data.id_branchOfficeNew != id_branchOfficeIniAux || data.id_paymentMethodNew != id_paymentMethodIniAux ||
			data.isPredeterminedNew != isPredeterminedIniAux) {
			data.onlyBecauseIsPredetermined = (data.id_companyNew == id_companyIniAux && data.id_divisionNew == id_divisionIniAux &&
				data.id_branchOfficeNew == id_branchOfficeIniAux && data.id_paymentMethodNew == id_paymentMethodIniAux);
			$.ajax({
				url: "Person/ItsRepeatedPaymentMethod",
				type: "post",
				data: data,
				async: false,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					//showLoading();
				},
				success: function (result) {
					if (result !== null) {
						if (result.itsRepeated == 1) {
							e.isValid = false;
							e.errorText = result.Error;
							UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
							UpdateTabImage(e, "tabProvider");
						} else {
							id_companyIniAux = 0;
							id_divisionIniAux = 0;
							id_branchOfficeIniAux = 0;
							id_paymentMethodIniAux = 0;
							isPredeterminedIniAux = false;
							isActiveIniAux = false;
						}
					}
				},
				complete: function () {
					//hideLoading();
				}
			});
		}

	}
}

function ProviderPaymentMethodCompanyCombo_Init(s, e) {

	id_companyIniAux = s.GetValue();
	id_divisionIniAux = id_divisionPM.GetValue();
	id_branchOfficeIniAux = id_branchOfficePM.GetValue();
	id_paymentMethodIniAux = id_paymentMethod.GetValue();
	isPredeterminedIniAux = isPredeterminedPM.GetValue();
	isActiveIniAux = isActive.GetValue();

	var data = {
		id_company: s.GetValue(),
		id_division: id_divisionPM.GetValue(),
		id_branchOffice: id_branchOfficePM.GetValue(),
		id_paymentMethod: id_paymentMethod.GetValue()
	};

	$.ajax({
		url: "Person/InitProviderPaymentMethodCompanyCombo",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_company
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_companyPM, result.companies, arrayFieldStr);

			UpdateDetailObjects(id_divisionPM, result.divisions, arrayFieldStr);

			UpdateDetailObjects(id_branchOfficePM, result.branchOffices, arrayFieldStr);

			//id_paymentMethod
			UpdateDetailObjects(id_paymentMethod, result.paymentMethods, arrayFieldStr);
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function ProviderPaymentMethodCompanyCombo_SelectedIndexChanged(s, e) {

	id_divisionPM.ClearItems();
	id_branchOfficePM.ClearItems();
	id_paymentMethod.ClearItems();

	$.ajax({
		url: "Person/ProviderPaymentMethodCompanyDetailData",
		type: "post",
		data: { id_company: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_divisionPM, result.divisions, arrayFieldStr);
				UpdateDetailObjects(id_paymentMethod, result.paymentMethods, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

// Production Unit Provider
function OnCodeProductionUnitProvider(s, e) {
	var noVale = false;
	var texto = "";
	// 
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var indice = ProductionShrimpUnitProvider.cpEditingRowVisibleIndex;
		var isNew = true;
		if (indice >= 0) {
			isNew = false;
		}
		var data = {
			codeProductionUnitProvider: e.value,
			isNew: isNew,
			id: ProductionShrimpUnitProvider.GetRowKey(e.visibleIndex)
		};
		$.ajax({
			url: "Person/ItsRepeatedCodeProduction",
			type: "post",
			data: data,
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				// 
				if (result !== null) {
					if (result.itsRepeated == 1) {
						noVale = true;
						texto = result.Error;
					}
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
		// 
		if (noVale == true) {
			e.isValid = false;
			e.errorText = texto;
			UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}

	}
}

function OnGetRowVal(values) {
	// 
}

function OnNameProductionUnitProvider(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

function OnAddressProductionUnitProvider(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

function OnPoolNumberProductionUnitProviderValidation(s, e) {
	if (e.value !== null) {
		if (parseInt(e.value) < 0) {
			e.isValid = false;
			e.errorText = "Número de piscinas debe ser mayor e igual a cero o nulo";
			UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
	}
}

function OnINPnumberProductionUnitProviderValidation(s, e) {
	var vIsCopackingD = isCopackingDetail.GetChecked();
	if (!vIsCopackingD) {
		if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo obligatorio"
			UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
	}
}

function OnMinisterialAgreementProductionUnitProviderTextChanged(s, e) {
	// 
	if (tramitNumberProductionUnitProvider != undefined) {
		tramitNumberProductionUnitProvider.Validate();
	}

}

function OnMinisterialAgreementProductionUnitProviderValidation(s, e) {
	var vIsCopackingD = isCopackingDetail.GetChecked();
	// 
	if (!vIsCopackingD) {
		var tramitNumberProductionUnitProviderTmp = tramitNumberProductionUnitProvider.GetText();
		if ((e.value === "" || e.value === null) && (tramitNumberProductionUnitProviderTmp === null || tramitNumberProductionUnitProviderTmp === "")) {
			e.isValid = false;
			e.errorText = "La Información de Acuerdo Ministerial y Trámite no pueden estar vacíos al mismo tiempo";
			UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
		if (e.value !== null && e.value !== "") {
			if (tramitNumberProductionUnitProviderTmp !== null && tramitNumberProductionUnitProviderTmp !== "") {
				e.isValid = false;
				e.errorText = "La información de Acuerdo Ministerial y Número de Trámite no pueden existir al mismo tiempo";
				UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
				UpdateTabImage(e, "tabProvider");
			}
		}
	}
}

function OnTramitNumberProductionUnitProviderTextChanged(s, e) {
	// 
	if (ministerialAgreementProductionUnitProvider != undefined) {
		ministerialAgreementProductionUnitProvider.Validate();
	}

}


function OnTramitNumberProductionUnitProviderValidation(s, e) {
	//var vIsCopacking = isCopacking.GetChecked();
	var vIsCopackingD = isCopackingDetail.GetChecked();
	// 
	if (!vIsCopackingD) {
		var ministerialAgreementProductionUnitProviderTmp = ministerialAgreementProductionUnitProvider.GetText();
		if ((e.value === "" || e.value === null) && (ministerialAgreementProductionUnitProviderTmp === null || ministerialAgreementProductionUnitProviderTmp === "")) {
			e.isValid = false;
			e.errorText = "La Información de Acuerdo Ministerial y Trámite no pueden estar vacíos al mismo tiempo";
			UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
			UpdateTabImage(e, "tabProvider");
		}
		if (e.value !== null && e.value !== "") {
			if (ministerialAgreementProductionUnitProviderTmp !== null && ministerialAgreementProductionUnitProviderTmp !== "") {
				e.isValid = false;
				e.errorText = "La información de Acuerdo Ministerial y Número de Trámite no pueden existir al mismo tiempo";
				UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
				UpdateTabImage(e, "tabProvider");
			}

		}
	}
}



function OnshippingTypeValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

function OnFishingSiteValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}

function OnFishingZoneValidation(s, e) {


}


function FishingZone_Init(s, e) {
	FishingZone_SelectedIndexChanged(s, e);
}



function FishingZone_SelectedIndexChanged(s, e) {
	var _id_FishingSite = ASPxClientControl.GetControlCollection().GetByName("id_FishingSite").GetValue();
	id_FishingSite.ClearItems();

	var id = s.GetValue();

	$.ajax({
		url: "FishingZone/FishingZoneSiteData",
		type: "post",
		data: { id_fishingZone: id },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {

			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("id");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_FishingSite, result._fishingSite, arrayFieldStr);

				var isSameValue = findJsonData(result._fishingSite, "id", _id_FishingSite);

				if (isSameValue) {
					ASPxClientControl.GetControlCollection().GetByName("id_FishingSite").SetValue(_id_FishingSite);
				}

			}
		},
		complete: function () {
			hideLoading();
		}
	});

}



// ProviderPaymentTerm
var id_paymentTermIniAux = 0;


function OnProviderControlComboValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	}
}


function OnProviderPaymentTermPaymentTermComboValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
		UpdateTabImage(e, "tabProvider");
	} else {
		var data = {
			id_companyNew: id_company.GetValue(),
			id_divisionNew: id_division.GetValue(),
			id_branchOfficeNew: id_branchOffice.GetValue(),
			id_paymentTermNew: id_paymentTerm.GetValue(),
			isPredeterminedNew: isPredetermined.GetValue(),
			onlyBecauseIsPredetermined: false
		};
		if (data.id_companyNew != id_companyIniAux || data.id_divisionNew != id_divisionIniAux ||
			data.id_branchOfficeNew != id_branchOfficeIniAux || data.id_paymentTermNew != id_paymentTermIniAux ||
			data.isPredeterminedNew != isPredeterminedIniAux) {
			data.onlyBecauseIsPredetermined = (data.id_companyNew == id_companyIniAux && data.id_divisionNew == id_divisionIniAux &&
				data.id_branchOfficeNew == id_branchOfficeIniAux && data.id_paymentTermNew == id_paymentTermIniAux);
			$.ajax({
				url: "Person/ItsRepeatedPaymentTerm",
				type: "post",
				data: data,
				async: false,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					//showLoading();
				},
				success: function (result) {
					if (result !== null) {
						if (result.itsRepeated == 1) {
							e.isValid = false;
							e.errorText = result.Error;
							UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
							UpdateTabImage(e, "tabProvider");
						} else {
							id_companyIniAux = 0;
							id_divisionIniAux = 0;
							id_branchOfficeIniAux = 0;
							id_paymentTermIniAux = 0;
							isPredeterminedIniAux = false;
							isActiveIniAux = false;
						}
					}
				},
				complete: function () {
					//hideLoading();
				}
			});
		}

	}
}


function ProviderPaymentTerm_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
}

function ProductionShrimpUnitProvider_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
}

function FrameworkContract_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
	e.customArgs['providerCustomer'] = $("#providerCustomer").val();
}

function FrameworkContractItemView_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
	e.customArgs["id_frameworkContract"] = PersonFrameworkContractItemsView.cpIdFrameworkContract;
	e.customArgs["code_typeContractFramework"] = PersonFrameworkContractItemsView.cpCodeTypeFrameworkContract; //$("#code_typeContractFramework").val();
}

function FrameworkContractItem_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
	//e.customArgs["id_frameworkContract"] = PersonFrameworkContractItems.cpIdFrameworkContract;
	e.customArgs["id_frameworkContract"] = $("#id_frameworkContract").val();
	e.customArgs["code_typeContractFramework"] = $("#code_typeContractFramework").val();
	e.customArgs["code_documentState"] = $("#code_documentState").val();
}

function FrameworkContractDeliveryPlanView_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
	e.customArgs["id_frameworkContract"] = PersonFrameworkContractDeliveryPlansView.cpIdFrameworkContract;
	e.customArgs["id_frameworkContractItem"] = PersonFrameworkContractDeliveryPlansView.cpIdFrameworkContractItem;
}

function FrameworkContractDeliveryPlan_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
	e.customArgs["id_frameworkContract"] = $("#id_frameworkContract").val();
	e.customArgs["id_frameworkContractItem"] = $("#id_frameworkContractItem").val();
	e.customArgs["code_documentState"] = $("#code_documentState").val();
}

function ProviderPaymentTermCompanyCombo_Init(s, e) {

	id_companyIniAux = s.GetValue();
	id_divisionIniAux = id_division.GetValue();
	id_branchOfficeIniAux = id_branchOffice.GetValue();
	id_paymentTermIniAux = id_paymentTerm.GetValue();
	isPredeterminedIniAux = isPredetermined.GetValue();
	isActiveIniAux = isActive.GetValue();

	var data = {
		id_company: s.GetValue(),
		id_division: id_division.GetValue(),
		id_branchOffice: id_branchOffice.GetValue(),
		id_paymentTerm: id_paymentTerm.GetValue()
	};

	$.ajax({
		url: "Person/InitProviderPaymentTermCompanyCombo",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_company
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_company, result.companies, arrayFieldStr);

			//id_division
			//arrayFieldStr = [];
			//arrayFieldStr.push("code");
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);

			//id_branchOffice
			//arrayFieldStr = [];
			//arrayFieldStr.push("name");
			UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);

			//id_paymentTerm
			UpdateDetailObjects(id_paymentTerm, result.paymentTerms, arrayFieldStr);
		},
		complete: function () {
			//hideLoading();
		}
	});


}


function ProviderPaymentTermDivisionCombo_SelectedIndexChanged(s, e) {

	id_branchOffice.ClearItems();

	$.ajax({
		url: "Person/ProviderPaymentTermDivisionDetailData",
		type: "post",
		data: { id_company: id_company.GetValue(), id_division: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function ProviderPaymentMethodDivisionCombo_SelectedIndexChanged(s, e) {
	id_branchOfficePM.ClearItems();

	$.ajax({
		url: "Person/ProviderPaymentMethodDivisionDetailData",
		type: "post",
		data: { id_company: id_companyPM.GetValue(), id_division: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_branchOfficePM, result.branchOffices, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function ProviderPaymentTermCompanyCombo_SelectedIndexChanged(s, e) {

	id_division.ClearItems();
	id_branchOffice.ClearItems();
	id_paymentTerm.ClearItems();

	$.ajax({
		url: "Person/ProviderPaymentTermCompanyDetailData",
		type: "post",
		data: { id_company: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);
				UpdateDetailObjects(id_paymentTerm, result.paymentTerms, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

//function IsActive_CheckedChanged(s, e) {
//    console.log("!s.GetChecked():");
//    console.log(!s.GetChecked());
//    if (!s.GetChecked()) {
//        isPredetermined.SetChecked(false);
//    } 
//}

//ProviderGeneralData

function ComboOrigin_Init(s, e) {

	var data = {
		id_origin: s.GetValue(),
		id_country: id_country.GetValue(),
		id_city: id_city.GetValue(),
		id_stateOfContry: id_stateOfContry.GetValue()
	};

	$.ajax({
		url: "Person/InitComboOrigin",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_country
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_country, result.countries, arrayFieldStr);

			//id_city
			arrayFieldStr = [];
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_city, result.cities, arrayFieldStr);

			//id_stateOfContry
			arrayFieldStr = [];
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_stateOfContry, result.stateOfContries, arrayFieldStr);
		},
		complete: function () {
			//hideLoading();
		}
	});

}

function ComboOrigin_SelectedIndexChanged(s, e) {

	id_country.ClearItems();
	id_city.ClearItems();
	id_stateOfContry.ClearItems();

	$.ajax({
		url: "Person/OriginDetailData",
		type: "post",
		data: { id_origin: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result !== null) {
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				// 
				UpdateDetailObjects(id_country, result.countries, arrayFieldStr);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function ComboContry_SelectedIndexChanged(s, e) {

	id_city.ClearItems();
	id_stateOfContry.ClearItems();

	$.ajax({
		url: "Person/CountryDetailData",
		type: "post",
		data: { id_country: s.GetValue() },
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			if (result !== null) {
				//id_city
				arrayFieldStr = [];
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_city, result.cities, arrayFieldStr);

				//id_stateOfContry
				arrayFieldStr = [];
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_stateOfContry, result.stateOfContries, arrayFieldStr);


				//codeCountry
				if (result.codeCountry == codeCountrySystem.GetValue()) {
					id_stateOfContry.SetVisible(false);
					id_stateOfContryLabel.SetVisible(false);
				} else {
					id_stateOfContry.SetVisible(true);
					id_stateOfContryLabel.SetVisible(true);
				}
			}
		},
		complete: function () {
		}
	});
}

//Rise
function ComboCategoryRise_SelectedIndexChanged(s, e) {
	GetInvoiceAmountRise({
		id_categoryRise: s.GetValue(),
		id_activityRise: id_activityRise.GetValue()
	});
}

function ComboActivityRise_SelectedIndexChanged(s, e) {
	GetInvoiceAmountRise({
		id_categoryRise: id_categoryRise.GetValue(),
		id_activityRise: s.GetValue()
	});
}

function GetInvoiceAmountRise(data) {

	$.ajax({
		url: "Person/GetInvoiceAmountRise",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			invoiceAmountRise.SetValue(0);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null) {
				invoiceAmountRise.SetValue(result.invoiceAmountRise);
			} else {
				invoiceAmountRise.SetValue(0);
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

//Additional Data

function ComboTypeBoxCardAndBankAD_Init(s, e) {

	var data = {
		id_typeBoxCardAndBankAD: s.GetValue(),
		id_boxCardAndBankAD: id_boxCardAndBankAD.GetValue()
	};

	$.ajax({
		url: "Person/InitComboTypeBoxCardAndBankAD",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_metricUnit.SetValue(null);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_boxCardAndBankAD
			var arrayFieldStr = [];
			arrayFieldStr.push("code");
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_boxCardAndBankAD, result.boxCardAndBankADs, arrayFieldStr);

		},
		complete: function () {
			//hideLoading();
		}
	});

}

function ComboTypeBoxCardAndBankAD_SelectedIndexChanged(s, e) {

	id_boxCardAndBankAD.ClearItems();

	$.ajax({
		url: "Person/TypeBoxCardAndBankADDetailData",
		type: "post",
		data: { id_typeBoxCardAndBankAD: s.GetValue() },
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null) {
				//id_boxCardAndBankAD
				var arrayFieldStr = [];
				arrayFieldStr.push("code");
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_boxCardAndBankAD, result.boxCardAndBankADs, arrayFieldStr);
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}
function OnFdaValidation(s, e) {
	var values = id_roles.GetValue();
	var roles = values.split(",");

	var isRol = (roles.indexOf("25") !== -1);
	if (isRol) {
		var tab = tabControl.GetTabByName("tabProvider");
		if (!tab.GetVisible()) {
			e.isValid = true;
		} else if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo obligatorio";
		}

		UpdateTabImage(e, "tabProvider");
	}
}

function OnPlantCodeValidation(s, e) {
	var values = id_roles.GetValue();
	var roles = values.split(",");

	var isRol = (roles.indexOf("25") !== -1);
	if (isRol) {
	var tab = tabControl.GetTabByName("tabProvider");
		if (!tab.GetVisible()) {
			e.isValid = true;
		} else if (e.value === null) {
			e.isValid = false;
			e.errorText = "Campo obligatorio";
		}

		UpdateTabImage(e, "tabProvider");
	}
}

// Provider Type changed
function ComboProviderType_SelectedIndexChanged(s, e) {
	var id_providerType = s.GetValue();
	console.log(id_providerType);
	if (id_providerType != 0 && id_providerType != null && id_providerType !== undefined) {
		$.ajax({
			url: "Person/ProviderTypeWhatProviderIs",
			type: "post",
			data: { id_providerType: s.GetValue() },
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforedSend: function () {
				showLoading();
			},
			success: function (result) {
				// 
				if (result.isShrimpProvider == "SI") {
					//$("#detailShrimpProvider").show();
					document.getElementById("isProviderShrimpBit").value = "SI";
					tabControlProvider.GetTabByName("tabShrimpProvider").SetVisible(true);
				} else {
					//$("#detailShrimpProvider").hide();
					document.getElementById("isProviderShrimpBit").value = "NO";
					tabControlProvider.GetTabByName("tabShrimpProvider").SetVisible(false);
				}
				if (result.isTransportist == "SI") {
					document.getElementById("isProviderTransportistBit").value = "SI";
					tabControlProvider.GetTabByName("tabTransportist").SetVisible(true);
				} else {
					document.getElementById("isProviderTransportistBit").value = "NO";
					tabControlProvider.GetTabByName("tabTransportist").SetVisible(false);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

// Checked Changed

function ShowDetailElectronicPayment(s, e) {
	if (s.GetChecked()) {
		$("#detailElectronicPayment").show();
	} else {
		$("#detailElectronicPayment").hide();
	}
}

function ShowDetailRise(s, e) {
	if (s.GetChecked()) {
		$("#detailRise").show();
	} else {
		$("#detailRise").hide();
	}
}

//
function ComboIdentificationType_SelectedIndexChanged(s, e) {
	//OnIdentificationNumberValidationGeneral()
	identification_number.Validate();
	//var resultTmp = OnIdentificationNumberValidationGeneral(identification_number.GetText());
	//if (resultTmp != undefined){
	//    identification_number.isValid = resultTmp.isValid;
	//    identification_number.errorText = resultTmp.errorTextMessage;

	//}



	//OnIdentificationNumberValidation(identification_number,identification_number.)
	var id_provider = $("#id_provider").val();
	if (id_provider != 0 && id_provider != null && id_provider !== undefined) {
		$.ajax({
			url: "Person/PersonsGetCodSRIIdentificationType",
			type: "post",
			data: { id_identificationType: s.GetValue() },
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result.codSRI_IdentificationType == "06") { // Es el codigo del SRI del tipo de identificación Pasaporte
					$("#detailPassportImportData").show();
				} else {
					$("#detailPassportImportData").hide();
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}

}



function AddValidation(grid, name) {
	if (grid.editorIDList.indexOf(name) < 0)
		grid.editorIDList.push(name);
}

function RemoveValidation(grid, name) {
	var index = grid.editorIDList.indexOf(name);
	if (index > 0) {
		grid.editorIDList.splice(index, 1);
	}
}

function UpdateTabImage(e, tabName) {
	var imageUrl = "/Content/image/noimage.png";
	if (!e.isValid) {
		imageUrl = "/Content/image/info-error.png";
	}

	var tab = tabControl.GetTabByName(tabName);
	tab.SetImageUrl(imageUrl);
	tab.SetActiveImageUrl(imageUrl);
}

function ClearValidators() {
	RemoveValidation(gvPerson, "Rol");

	// PROVAIDER
	RemoveValidation(gvPerson, "id_pymentMethod");
	RemoveValidation(gvPerson, "id_pymentMean");

	// EMEPLOYEE
	RemoveValidation(gvPerson, "id_department");
}

// ROL CHANGE
function RolTokenBox_TokensChanged(s, e) {
	var values = s.GetValue();

	var roles = values.split(",");


	var isProvider = (roles.indexOf("1") !== -1);


	var isEmployee = (roles.indexOf("2") !== -1);

	var isCustomerLocal = (roles.indexOf("3") !== -1);


	var isCustomerExterior = (roles.indexOf("6") !== -1);

	var isProcessPlant = (roles.indexOf("21") !== -1);

	var isexportPlant = (roles.indexOf("25") !== -1);

	var isProviderShrimp = false;
	var isProviderTransportist = false;

	gvPersonRols.PerformCallback();
	var id_providerType = s.GetValue();
	console.log(id_providerType);
	if (id_providerType != 0 && id_providerType != null && id_providerType !== undefined) {
		$.ajax({
			url: "Person/ProviderTypeWhatProviderIs",
			type: "post",
			data: { id_providerType: s.GetValue() },
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforedSend: function () {
				//showLoading();
			},
			success: function (result) {
				if (result.isShrimpProvider == "SI") {
					isProviderShrimp = true;
				} else {
					isProviderShrimp = false;
				}
				if (result.isTransportist == "SI") {
					isProviderTransportist = true;
				} else {
					isProviderTransportist = false;
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
	if (isexportPlant) { 
		$("#detailExportPlant").show();
	} else {
		$("#detailExportPlant").hide();
	}
	// 
	tabControl.GetTabByName("tabProvider").SetVisible(isProvider);
	tabControl.GetTabByName("tabEmployee").SetVisible(isEmployee);
	tabControl.GetTabByName("tabCustomer").SetVisible(isCustomerLocal);

	tabControlProvider.GetTabByName("tabGeneralDataProvider").SetVisible(isProvider);
	tabControlProvider.GetTabByName("tabSpecificDatesProvider").SetVisible(isProvider);
	tabControlProvider.GetTabByName("tabRelatedInformationProvider").SetVisible(isProvider);
	tabControlProvider.GetTabByName("tabShrimpProvider").SetVisible(isProvider && isProviderShrimp);
	tabControlProvider.GetTabByName("tabTransportist").SetVisible(isProvider && isProviderTransportist);

	tabControlCustomer.GetTabByName("tabGeneralDataCustomer").SetVisible(isCustomerLocal);
	tabControlCustomer.GetTabByName("tabContactDataCustomer").SetVisible(isCustomerLocal);
	tabControlCustomer.GetTabByName("tabDebtsDataCustomer").SetVisible(isCustomerLocal);
	tabControl.GetTabByName("tabFrameworkContract").SetVisible(isProvider || isCustomerLocal || isCustomerExterior);

	tabControlProvider.GetTabByName("tabCopacking").SetVisible(isProcessPlant);
}

function AddNewItem(s, e) {
	//isCopackingDetail.SetValue(isCopacking.GetChecked());
	gvPerson.AddNewRow();
}

function RemoveItems(s, e) {
	gvPerson.GetSelectedFieldValues("id", function (values) {

		var selectedRows = [];

		for (var i = 0; i < values.length; i++) {
			selectedRows.push(values[i]);
		}

		showConfirmationDialog(function () {
			$.ajax({
				url: "Person/DeleteSelectedPersons",
				type: "post",
				data: { ids: selectedRows },
				async: true,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					showLoading();
				},
				success: function (result) {
					//$("#maincontent").html(result);
				},
				complete: function () {
					gvPerson.PerformCallback();
					gvPerson.UnselectRows();
					hideLoading();
				}
			});
		});
	});
}

function GridViewItemsCustomCommandButton_Click(s, e) {
	if (e.buttonID === "btnDeleteRow") {
		showConfirmationDialog(function () {
			s.DeleteRow(e.visibleIndex);
		});
	}
}

function RefreshGrid(s, e) {
	gvPerson.PerformCallback();
}

function Print(s, e) {

}

function importFile(s, e) {
	console.log('Funcionalidad no implementada');
}


//SELECTION

function OnGridViewInit(s, e) {
	UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
	UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
	UpdateTitlePanel();
}

function UpdateTitlePanel() {
	var selectedFilteredRowCount = GetSelectedFilteredRowCount();

	var text = "Total de elementos seleccionados: <b>" + gvPerson.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvPerson.GetSelectedRowCount() - GetSelectedFilteredRowCount();
	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";
	$("#lblInfo").html(text);

	//if ($("#selectAllMode").val() != "AllPages") {
	SetElementVisibility("lnkSelectAllRows", gvPerson.GetSelectedRowCount() > 0 && gvPerson.cpVisibleRowCount > selectedFilteredRowCount);
	SetElementVisibility("lnkClearSelection", gvPerson.GetSelectedRowCount() > 0);
	//}

	btnRemove.SetEnabled(gvPerson.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
	return gvPerson.cpFilteredRowCountWithoutPage + gvPerson.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
	gvPerson.SelectRows();
}

function UnselectAllRows() {
	gvPerson.UnselectRows();
}

function ValidateTabDetailsProvider(gv, validDetailsProvider, validProvider, tabDetailsProvider) {
	if (gv.IsEditing()) {//gv.cpRowsCount === 0 ||
		UpdateTabControlImage({ isValid: false }, tabDetailsProvider, tabControlProvider);
		UpdateTabImage({ isValid: false }, "tabProvider");
		return false;
	} else {
		if (validDetailsProvider) {
			UpdateTabControlImage({ isValid: true }, tabDetailsProvider, tabControlProvider);
		}
		if (validProvider) {
			UpdateTabImage({ isValid: true }, "tabProvider");
		}
		return true;
	}
}

function ValidateTabSpecificDatesProvider(gv, validSpecificDatesProvider, validProvider) {
	return ValidateTabDetailsProvider(gv, validSpecificDatesProvider, validProvider, "tabSpecificDatesProvider");
}

function ValidateTabRelatedInformationProvider(gv, validRelatedInformationProvider, validProvider) {
	return ValidateTabDetailsProvider(gv, validRelatedInformationProvider, validProvider, "tabRelatedInformationProvider");
}

function ButtonUpdate_Click(s, e) {


	//var r = confirm("Press a button!");


	var valid = true;
	var id_PersonTmp = 0;

	var validGeneralDataPersonTable = ASPxClientEdit.ValidateEditorsInContainerById("generalDataPersonTable", null, true);

	if (!validGeneralDataPersonTable) {
		valid = false;
	}

	var validProvider = true;
	var validSpecificDatesProvider = true;
	var validRelatedInformationProvider = true;

	var values = id_roles.GetValue();

	var roles = values.split(",");

	var isProvider = (roles.indexOf("1") !== -1);

	if (isProvider) {
		var validTabGeneralDataProviderTable = ASPxClientEdit.ValidateEditorsInContainerById("tabGeneralDataProviderTable", null, true);

		if (validTabGeneralDataProviderTable) {
			UpdateTabControlImage({ isValid: true }, "tabGeneralDataProvider", tabControlProvider);
			UpdateTabImage({ isValid: true }, "tabProvider");
		} else {
			UpdateTabControlImage({ isValid: false }, "tabGeneralDataProvider", tabControlProvider);
			UpdateTabImage({ isValid: false }, "tabProvider");
			valid = false;
			validProvider = false;
		}
		//ValidateSpecificDatesProvider
		//ProviderPaymentTerms
		validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderPaymentTerms, validSpecificDatesProvider, validProvider));
		validProvider = (validProvider && validSpecificDatesProvider);
		valid = (valid && validProvider);

		//ProviderPaymentMethods
		validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderPaymentMethods, validSpecificDatesProvider, validProvider));
		validProvider = (validProvider && validSpecificDatesProvider);
		valid = (valid && validProvider);

		//ProviderSeriesForDocumentss
		validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderSeriesForDocumentss, validSpecificDatesProvider, validProvider));
		validProvider = (validProvider && validSpecificDatesProvider);
		valid = (valid && validProvider);

		//ProviderRetentions
		validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderRetentions, validSpecificDatesProvider, validProvider));
		validProvider = (validProvider && validSpecificDatesProvider);
		valid = (valid && validProvider);

		//ProviderPersonAuthorizedToPayTheBills
		validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderPersonAuthorizedToPayTheBills, validSpecificDatesProvider, validProvider));
		validProvider = (validProvider && validSpecificDatesProvider);
		valid = (valid && validProvider);

		//ValidateRelatedInformationProvider
		//ProviderRelatedCompanies
		validRelatedInformationProvider = (validRelatedInformationProvider && ValidateTabRelatedInformationProvider(ProviderRelatedCompanies, validRelatedInformationProvider, validProvider));
		validProvider = (validProvider && validRelatedInformationProvider);
		valid = (valid && validProvider);

		//ProviderAccountingAccounts
		validRelatedInformationProvider = (validRelatedInformationProvider && ValidateTabRelatedInformationProvider(ProviderAccountingAccounts, validRelatedInformationProvider, validProvider));
		validProvider = (validProvider && validRelatedInformationProvider);
		valid = (valid && validProvider);

		//ProviderItems
		validRelatedInformationProvider = (validRelatedInformationProvider && ValidateTabRelatedInformationProvider(ProviderItems, validRelatedInformationProvider, validProvider));
		validProvider = (validProvider && validRelatedInformationProvider);
		valid = (valid && validProvider);

		//ProviderMailByComDivBras
		validRelatedInformationProvider = (validRelatedInformationProvider && ValidateTabRelatedInformationProvider(ProviderMailByComDivBras, validRelatedInformationProvider, validProvider));
		validProvider = (validProvider && validRelatedInformationProvider);
		valid = (valid && validProvider);

	}

	var isCustomerLocal = (roles.indexOf("3") !== -1);
	var isCustomerExterior = (roles.indexOf("6") !== -1);

	if (isCustomerLocal || isCustomerExterior || isProvider) {
		if (PersonFrameworkContracts.IsEditing()) {
			UpdateTabImage({ isValid: false }, "tabFrameworkContract");
			//UpdateTabImage({ isValid: false }, "tabProvider");
			valid = false;
		} else {
			UpdateTabImage({ isValid: true }, "tabFrameworkContract");
		}
	}

	var isEmployee = (roles.indexOf("2") !== -1);

	if (isEmployee) {
		var validTabEmployeeTable = ASPxClientEdit.ValidateEditorsInContainerById("tabEmployeeTable", null, true);

		if (validTabEmployeeTable) {
			UpdateTabImage({ isValid: true }, "tabEmployee");
		} else {
			UpdateTabImage({ isValid: false }, "tabEmployee");
			valid = false;
		}
	}

	if (valid) {
		if (isProvider) {
			if (forcedToKeepAccounts.GetChecked() != null || specialTaxPayer.GetChecked() != null) {
				var message1 = "";
				var message2 = "";
				var messageFinal = "";
				var answer = false;
				if (forcedToKeepAccounts.GetChecked() === false || specialTaxPayer.GetChecked() === false) {
					if (forcedToKeepAccounts.GetChecked() === false) {
						message1 = "El campo Obligado a llevar Contabilidad"
					}
					if (specialTaxPayer.GetChecked() === false) {
						message2 = "El campo Contribuyente especial"
					}
					if (message1 != "" && message2 != "") {
						messageFinal = message1 + " y " + message2 + " están en falso ¿Desea Continuar?"
					} else if (message1 != "") {
						messageFinal = message1 + " está en falso ¿Desea Continuar?"
					} else if (message2 != "") {
						messageFinal = message2 + " está en falso ¿Desea Continuar?"
					}
					if (messageFinal != "") {
						answer = confirm(messageFinal);
						if (answer == false) {
							return;
						}
					}

				}
			}
		}

		var person = "id=" + $("#id_person").val() + "&" + $("#formEditPerson").serialize();

		var url = ($("#id_person").val() === "0") ? "Person/PersonsPartialAddNew"
			: "Person/PersonsPartialUpdate";

		$.ajax({
			url: url,
			type: "post",
			data: person,
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {

				if (result != null) {
					id_PersonTmp = result.idPerson;
					if (id_PersonTmp != 0 && id_PersonTmp != undefined && id_PersonTmp != null) {
						if (isCustomerLocal || isProvider || isCustomerExterior) {
							$.ajax({
								url: "Person/MigrarIndividual",
								type: "post",
								data: "id=" + id_PersonTmp,
								async: false,
								cache: false,
								error: function (error) {
									console.log(error);
								},
								beforedSend: function () {
								},
								success: function (result) {
									if (result != null) {
										if (result.Data != null) {
											if (result.Data.respuestaProveedor != null) {
												console.log(result.Data.respuestaProveedor);
											}
											if (result.Data.respuestaCliente != null) {
												console.log(result.Data.respuestaCliente);
											}
										}
									}
								},
								complete: function () {
								}
							});
						}
					}
				}
				gvPerson.CancelEdit();
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function BtnCancel_Click(s, e) {
	gvPerson.CancelEdit();
}

function init() {

}

$(function () {
	init();
});


// {RA} CUSTOMER
//General Data
function OnSelectedIndexChanged_ClientCategory(s, e) {
    id_customerType.PerformCallback();
}

function CustomerType_BeginCallback(s, e) {
    e.customArgs["id_customerType"] = s.GetValue() === undefined ? null : s.GetValue();
    e.customArgs["id_clientCategory"] = id_clientCategory.GetValue() === undefined ? null : id_clientCategory.GetValue();
}

function CustomerType_EndCallback(s, e) {
    id_businessLine.PerformCallback();
}

function OnSelectedIndexChanged_CustomerType(s, e) {
    id_businessLine.PerformCallback();
}

function BusinessLine_BeginCallback(s, e) {
    e.customArgs["id_businessLine"] = s.GetValue() === undefined ? null : s.GetValue();
    e.customArgs["id_customerType"] = id_customerType.GetValue() === undefined ? null : id_customerType.GetValue();
    e.customArgs["id_clientCategory"] = id_clientCategory.GetValue() === undefined ? null : id_clientCategory.GetValue();
}

// CustomerPriceList
function CustomerPriceList_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
}

function CustomerPriceList_SelectedIndexChanged(s, e) {

	var objpriceListgTable = s.GetSelectedItem();

	var fechaInicio = objpriceListgTable.texts[1];   // GetColumnText("startDate").Text();
	var fechaFin = objpriceListgTable.texts[2];      //objpriceListgTable.GetColumnText("endDate");


	ASPxClientControl.GetControlCollection().GetByName("startDateCums").SetValue(fechaInicio);
	ASPxClientControl.GetControlCollection().GetByName("endDateCums").SetValue(fechaFin);


}
// CustomerAdress
function CustomerAddress_OnBeginCallback(s, e) {
	e.customArgs['id_person'] = $("#id_person").val();
}

function CambiarCopackingCheck(s, e) {
	var copack = s.GetValue();
	var rol = $("#rolExistente").val();
	var rolExistente = false;
	if (rol === "True") {
		rolExistente = true;
	}
	// 
	if (copack || rolExistente) {
		tabControlProvider.GetTabByName("tabCopacking").SetVisible(true);
	} else {
		tabControlProvider.GetTabByName("tabCopacking").SetVisible(false);
	}
}

function WarehouseCombo_SelectedIndexChanged(s, e) {
	id_WarehouseLocation.SetValue(null);
	id_WarehouseLocation.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "Person/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_WarehouseLocation, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}
function WarehouseCartonCombo_SelectedIndexChanged(s, e) {
	id_warehouseLocationCarton.SetValue(null);
	id_warehouseLocationCarton.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "Person/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_warehouseLocationCarton, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}
function WarehouseFreezeCombo_SelectedIndexChanged(s, e) {
	id_warehouseLocationFreeze.SetValue(null);
	id_warehouseLocationFreeze.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "Person/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_warehouseLocationFreeze, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}
function WarehouseLooseCartonCombo_SelectedIndexChanged(s, e) {
	id_WarehouseLocationLooseCarton.SetValue(null);
	id_WarehouseLocationLooseCarton.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "Person/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_WarehouseLocationLooseCarton, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}


function OnWarehouseLocationEntryInit(s, e) {
	var idwarehouse = id_Warehouse.GetValue();
	if (idwarehouse === null) {
		id_WarehouseLocation.ClearItems();
	} else {
		$.ajax({
			url: "Person/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: id_Warehouse.GetValue() },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_WarehouseLocation, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnWarehouseLocationCartonEntryInit(s, e) {
	var idwarehouse = id_WarehouseCarton.GetValue();
	if (idwarehouse === null) {
		id_warehouseLocationCarton.ClearItems();
	} else {
		$.ajax({
			url: "Person/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: id_WarehouseCarton.GetValue() },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_warehouseLocationCarton, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}
function OnWarehouseLocationFreezeEntryInit(s, e) {
	var idwarehouse = id_WarehouseFreeze.GetValue();
	if (idwarehouse === null) {
		id_warehouseLocationFreeze.ClearItems();
	} else {
		$.ajax({
			url: "Person/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: id_WarehouseFreeze.GetValue() },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_warehouseLocationFreeze, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}
function OnWarehouseLocationLooseCartonInit(s, e) {
	var idwarehouse = id_WarehouseLooseCarton.GetValue();
	if (idwarehouse === null) {
		id_WarehouseLocationLooseCarton.ClearItems();
	} else {
		$.ajax({
			url: "Person/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: id_WarehouseLooseCarton.GetValue() },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_WarehouseLocationLooseCarton, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnWarehouseLocationValidation(s, e) {
	var idwarehouse = id_Warehouse.GetValue();
	var idWarehouseLocation = id_WarehouseLocation.GetValue();
	if (idwarehouse !== null && idWarehouseLocation === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}


function OnWarehouseLocationCartonValidation(s, e) {
	var idwarehouse = id_WarehouseCarton.GetValue();
	var idWarehouseLocation = id_warehouseLocationCarton.GetValue();
	if (idwarehouse !== null && idWarehouseLocation === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}


function OnWarehouseLocationFreezeValidation(s, e) {
	var idwarehouse = id_WarehouseFreeze.GetValue();
	var idWarehouseLocation = id_warehouseLocationFreeze.GetValue();
	if (idwarehouse !== null && idWarehouseLocation === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnWarehouseLocationLooseCartonValidation(s, e) {
	var idwarehouse = id_WarehouseLooseCarton.GetValue();
	var idWarehouseLocation = id_WarehouseLocationLooseCarton.GetValue();
	if (idwarehouse !== null && idWarehouseLocation === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function SubCostCenterCombo_SelectedIndexChanged(s, e) {
	id_SubCostCenter.SetValue(null);
	id_SubCostCenter.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "Person/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: data },
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
					var subCostCenter = result[i];
					id_SubCostCenter.AddItem(subCostCenter.name, subCostCenter.id);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function SubCostCenterCartonCombo_SelectedIndexChanged(s, e) {
	id_SubCostCenterCarton.SetValue(null);
	id_SubCostCenterCarton.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}
	if (data !== null) {

		$.ajax({
			url: "Person/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: data },
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
					var subCostCenter = result[i];
					id_SubCostCenterCarton.AddItem(subCostCenter.name, subCostCenter.id);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function SubCostCenterFreezeCombo_SelectedIndexChanged(s, e) {
	id_SubCostCenterFreeze.SetValue(null);
	id_SubCostCenterFreeze.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}
	// 
	if (data !== null) {

		$.ajax({
			url: "Person/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: data },
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
					var subCostCenter = result[i];
					id_SubCostCenterFreeze.AddItem(subCostCenter.name, subCostCenter.id);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function SubCostCenterLooseCartonCombo_SelectedIndexChanged(s, e) {
	id_SubCostCenterLooseCarton.SetValue(null);
	id_SubCostCenterLooseCarton.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}
	// 
	if (data !== null) {

		$.ajax({
			url: "Person/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: data },
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
					var subCostCenter = result[i];
					id_SubCostCenterLooseCarton.AddItem(subCostCenter.name, subCostCenter.id);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}


function OnSubCostCenterEntryInit(s, e) {
	var idmaterialCostCenter = id_CostCenter.GetValue();
	if (idmaterialCostCenter === null) {
		id_SubCostCenter.ClearItems();
		id_SubCostCenter.SetValue(null);
	} else {
		var id = $("#idWarehouse").val();
		$.ajax({
			url: "Person/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: idmaterialCostCenter },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				var indexMaterial;
				id_SubCostCenter.ClearItems();
				if (result !== null && result !== undefined) {
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						if (id === warehouse.id.toString()) {
							indexMaterial = i;
						}
						id_SubCostCenter.AddItem(warehouse.name, warehouse.id);
					}
					id_SubCostCenter.SetSelectedIndex(indexMaterial);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnSubCostCenterCartonEntryInit(s, e) {
	var idmaterialCostCenter = id_CostCenterCarton.GetValue();
	if (idmaterialCostCenter === null) {
		id_SubCostCenterCarton.ClearItems();
		id_SubCostCenterCarton.SetValue(null);
	} else {
		var id = $("#idCostCenterCarton").val();
		$.ajax({
			url: "Person/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: idmaterialCostCenter },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				var indexMaterial;
				id_SubCostCenterCarton.ClearItems();
				if (result !== null && result !== undefined) {
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						if (id === warehouse.id.toString()) {
							indexMaterial = i;
						}
						id_SubCostCenterCarton.AddItem(warehouse.name, warehouse.id);
					}
					id_SubCostCenterCarton.SetSelectedIndex(indexMaterial);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnSubCostCenterFreezeEntryInit(s, e) {
	var idmaterialCostCenter = id_CostCenterFreeze.GetValue();
	if (idmaterialCostCenter === null) {
		id_SubCostCenterFreeze.ClearItems();
		id_SubCostCenterFreeze.SetValue(null);
	} else {
		var id = $("#idCostCenterFreeze").val();
		$.ajax({
			url: "Person/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: idmaterialCostCenter },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				var indexMaterial;
				id_SubCostCenterFreeze.ClearItems();
				if (result !== null && result !== undefined) {
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						if (id === warehouse.id.toString()) {
							indexMaterial = i;
						}
						id_SubCostCenterFreeze.AddItem(warehouse.name, warehouse.id);
					}
					id_SubCostCenterFreeze.SetSelectedIndex(indexMaterial);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}
function OnSubCostCenterLooseCartonEntryInit(s, e) {
	var idmaterialCostCenter = id_CostCenterLooseCarton.GetValue();
	if (idmaterialCostCenter === null) {
		id_SubCostCenterLooseCarton.ClearItems();
		id_SubCostCenterLooseCarton.SetValue(null);
	} else {
		var id = $("#idCostCenterFreeze").val();
		$.ajax({
			url: "Person/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: idmaterialCostCenter },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				var indexMaterial;
				id_SubCostCenterLooseCarton.ClearItems();
				if (result !== null && result !== undefined) {
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						if (id === warehouse.id.toString()) {
							indexMaterial = i;
						}
						id_SubCostCenterLooseCarton.AddItem(warehouse.name, warehouse.id);
					}
					id_SubCostCenterLooseCarton.SetSelectedIndex(indexMaterial);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnSubCostCenterValidation(s, e) {
	var idCostCenter = id_CostCenter.GetValue();
	var idSubCostCenter = id_SubCostCenter.GetValue();
	if (idCostCenter !== null && idSubCostCenter === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnSubCostCenterCartonValidation(s, e) {
	var idCostCenter = id_CostCenterCarton.GetValue();
	var idSubCostCenter = id_SubCostCenterCarton.GetValue();
	if (idCostCenter !== null && idSubCostCenter === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnSubCostCenterFreezeValidation(s, e) {
	var idCostCenter = id_CostCenterFreeze.GetValue();
	var idSubCostCenter = id_SubCostCenterFreeze.GetValue();
	if (idCostCenter !== null && idSubCostCenter === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnSubCostCenterLooseCartonValidation(s, e) {
	var idCostCenter = id_CostCenterLooseCarton.GetValue();
	var idSubCostCenter = id_SubCostCenterLooseCarton.GetValue();
	if (idCostCenter !== null && idSubCostCenter === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function ShowCopakingDetail(s, e) {
	// 
	if (s.GetChecked()) {
		$("#isCopackingDetail").show();
	} else {
		$("#isCopackingDetail").hide();
	}
}


