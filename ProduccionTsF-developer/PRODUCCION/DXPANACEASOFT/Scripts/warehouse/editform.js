
//COMBOS

function ComboBoxCompanies_SelectedIndexChanged(s, e) {
	id_warehouseType.ClearItems();
	id_inventoryLine.ClearItems();

	var item = id_company.GetSelectedItem();

	if (item !== null && item !== undefined) {
		$.ajax({
			url: "Warehouse/WarehouseTypeByCompany",
			type: "post",
			data: { id_company: item.value },
			async: true,
			cache: false,
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {
				//showLoading();
			},
			success: function(result) {
				for (var i = 0; i < result.length; i++) {
					id_warehouseType.AddItem(result[i].name, result[i].id);
				}
			},
			complete: function() {
				//hideLoading();
			}
		});

		$.ajax({
			url: "Warehouse/InventoryLineByCompany",
			type: "post",
			data: { id_company: item.value },
			async: true,
			cache: false,
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {
				//showLoading();
			},
			success: function(result) {
				for (var i = 0; i < result.length; i++) {
					id_inventoryLine.AddItem(result[i].name, result[i].id);
				}
			},
			complete: function() {
				//hideLoading();
			}
		});
	}

}


function AccountingTemplateCombo_SelectedIndexChanged(s, e) {

	processPlant.SetText("");

	var idAccounting = s.GetValue();
	if (idAccounting === null) {
		return;
	}

	if (idAccounting !== null) {

		$.ajax({
			url: "Warehouse/GetProcessPlantbyIdAccountingtemplate",
			type: "post",
			data: { id_accountingTemplate: idAccounting },
			async: true,
			cache: false,
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {

			},
			success: function(result) {

				if (result !== null && result !== undefined) {
					processPlant.SetText(result.namePlantaProceso);
				}
			},
			complete: function() {

			}
		});
	}

}

function AccountingTemplateCombo_Init(s, e) {
	processPlant.SetText("");

	var idAccounting = s.GetValue();
	if (idAccounting === null) {
		return;
	}

	if (idAccounting !== null) {

		$.ajax({
			url: "Warehouse/GetProcessPlantbyIdAccountingtemplate",
			type: "post",
			data: { id_accountingTemplate: idAccounting },
			async: true,
			cache: false,
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {

			},
			success: function(result) {

				if (result !== null && result !== undefined) {
					processPlant.SetText(result.namePlantaProceso);
				}
			},
			complete: function() {

			}
		});
	}
}

function CostProductionCombo_SelectedIndexChanged(s, e) {

	id_productionExpense.SetValue(null);
	id_accountingTemplate.SetValue(null);
	processPlant.SetText("");
	id_productionExpense.PerformCallback();
	//id_expenseProduction.SetValue(null);
	//id_expenseProduction.ClearItems();

	//var data = s.GetValue();
	//if (data === null) {
	//    return;
	//}

	//if (data !== null) {

	//    $.ajax({
	//        url: "Warehouse/CostProductionChangeData",
	//        type: "post",
	//        data: { id_costPoduction: data },
	//        async: true,
	//        cache: false,
	//        error: function (error) {
	//            console.log(error);
	//        },
	//        beforeSend: function () {

	//        },
	//        success: function (result) {
	//            for (var i = 0; i < result.length; i++) {
	//                var expenseProduction = result[i];
	//                id_expenseProduction.AddItem(expenseProduction.name, expenseProduction.id);
	//            }
	//        },
	//        complete: function () {

	//        }
	//    });
	//}
}

function PeriodNumberCombo_SelectedIndexChanged(s, e) {

	numberPeriod.SetValue(null);
	numberPeriod.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "Warehouse/NumberPeriodChangeData",
			type: "post",
			data: { yearPeriod: data },
			async: true,
			cache: false,
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {

			},
			success: function(result) {
				for (var i = 0; i < result.length; i++) {
					var period = result[i];
					numberPeriod.AddItem([period.period, period.id]);
				}
			},
			complete: function() {

			}
		});
	}
}



function DateInitCombo_SelectedIndexChanged(s, e) {

	dateInitPeriod.SetValue(null);
	dateEndPeriod.SetValue(null);

	var data = numberPeriod.GetValue();
	var year = yearPeriod.GetValue();

	if (data !== null && data !== undefined) {

		$.ajax({
			url: "Warehouse/DateInitChangeData",
			type: "post",
			data: { id: data, yearPeriod: year },
			async: true,
			cache: false,
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {

			},
			success: function(result) {
				if (result !== null && result !== undefined) {

					var _dateInit = new Date(result.dateInitYear, result.dateInitMonth - 1, result.dateInitDate);
					var _dateEnd = new Date(result.dateEndYear, result.dateEndMonth - 1, result.dateEndDay);

					dateInitPeriod.SetValue(_dateInit);
					dateEndPeriod.SetValue(_dateEnd);
				}
			},
			complete: function() {

			}
		});
	}
}

function ButtonUpdate_Click(s, e) {
	var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
	if (valid) {
		gvWarehouses.UpdateEdit();
	}
}

function ButtonCancel_Click(s, e) {
	if (gvWarehouses !== null && gvWarehouses !== undefined) {
		gvWarehouses.CancelEdit();
	} /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
		dialogAddDocumentType.Hide();
	}*/
}


var idExpenseProductionCombo = null;
function WarehouseExpenseAccountingTemplate_OnBeginCallback(s, e) {
	e.customArgs['id_warehouse'] = $("#id_warehouse").val();
}
function WarehouseExpenseAccountingTemplate_OnEndCallback(s, e) {

}

function ExpenseProductionCombo_OnInit(s, e) {
	if (!(s.GetValue() == null || s.GetValue() == undefined))
		idExpenseProductionCombo = s.GetValue();
	else
		idExpenseProductionCombo = null;
	id_expenseProduction.PerformCallback();
}

function ExpenseProductionCombo_BeginCallback(s, e) {
	e.customArgs['id_productionExpense'] = idExpenseProductionCombo;
	e.customArgs['id_productionCost'] = id_costProduction.GetValue();
}
function ExpenseProductionCombo_EndCallback(s, e) {
	// 
	//var _id_productionExpenseTmp = <%= ViewData["id_productionExpense"] %>;<%= new JavaScriptSerializer().Serialize(ViewData["Text"])) %>;
	if (idExpenseProductionCombo != null && idExpenseProductionCombo > 0)
		s.SetValue(idExpenseProductionCombo);
}

function ExpenseProductionCombo_SelectedIndexChanged(s, e) {
	id_accountingTemplate.PerformCallback();
	processPlant.SetText("");
}

function AccountingTemplateCombo_OnInit(s, e) {

	//id_accountingTemplate.PerformCallback();
}

function AccountingTemplateCombo_BeginCallback(s, e) {
	e.customArgs['id_accountingTemplate'] = s.GetValue();
	e.customArgs['id_productionCost'] = id_costProduction.GetValue();
	e.customArgs['id_productionExpense'] = id_expenseProduction.GetValue();
}

function AccountingTemplateCombo_EndCallback(s, e) {
	var _id_accountingTemplateTmp = '@ViewData["id_accountingTemplate"]';
	if (_id_accountingTemplateTmp != null && _id_accountingTemplateTmp > 0)
		s.SetValue(_id_accountingTemplateTmp);
	//console.log("AccountingTemplateCombo_OnInit " + s.GetValue());
}

function AccountingTemplateCombo_Validation(s, e) {
	//console.log("newrow " + gvWarehouseExpenseAccountingTemplate.cpIsNewRowEdit);
	//console.log("editrow " + gvWarehouseExpenseAccountingTemplate.cpEditingRowKey);
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		if (id_expenseProduction.GetValue() == null) {
			e.isValid = false;
			e.errorText = "Debe llenar la informacion de gasto de produccion.";
		} else {
			ValidationDetailExpenseAccountTemplate(id_expenseProduction.GetValue()
				, s.GetValue(), gvWarehouseExpenseAccountingTemplate.cpIsNewRowEdit
				, gvWarehouseExpenseAccountingTemplate.cpEditingRowKey, e);
		}
	}
}
function ExpenseProductionCombo_Validation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function ValidationDetailExpenseAccountTemplate(productionExpense, accountTemplate, isNewRow, editRowKey, e) {
	$.ajax({
		url: "Warehouse/ValidateRepeatedExpenses",
		type: "post",
		async: false,
		cache: false,
		data: {
			id_warehouse: $("#id_warehouse").val(),
			id_productionExpense: productionExpense,
			id_accountingTemplate: accountTemplate,
			isNewRow: isNewRow,
			editRowKey: editRowKey
		},
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
			//showLoading();
		},
		success: function(result) {
			if (!result.isValid) {
				e.isValid = result.isValid;
				e.errorText = result.errorText;
			}
		},
		complete: function() {
			//hideLoading();
		}
	});
}

function GridViewItemsWEACustomCommandButton_Click(s, e) {
	if (e.buttonID === "btnEditRow") {
		s.UpdateEdit();
	} else if (e.buttonID === "btnDeleteRow") {
		showConfirmationDialog(function() {
			s.DeleteRow(e.visibleIndex);
		});
	}
}


function OnEnableProductionCostType_Validation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}
function OnEnableProductionCostType_SelectedIndexChanged(s, e) {
	id_productionCostPoundType.SetEnabled(s.GetValue() === true);
}
function OnProductionCostPoundTypeCombo_Validation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}
function requierePerson_CheckedChanged(s, e) {

	if (s.GetChecked()) {
		ids_Roles.SetEnabled(true);
	} else {
		ids_Roles.SetValue(null);
		ids_Roles.SetEnabled(false);
	}

}