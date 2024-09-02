function btnAddPopup_Click(s, e) {
	var code_typeFiltersConfigurationAux = $("#code_typeFiltersConfiguration").val();

	var queryTextAux = query.GetText();
	var logicalTextAux = logical.GetText();
	logicalTextAux = (logicalTextAux == "Fin") ? "" : logicalTextAux;
	if (code_typeFiltersConfigurationAux == "Sel" || code_typeFiltersConfigurationAux == "Check") {
		queryTextAux += " [" + attribute1.GetText() + "] " + comparisonOperator.GetText() + " '" + conditionID.GetText() + "' " + logicalTextAux;
	} else if (code_typeFiltersConfigurationAux == "Date") {
		queryTextAux += " [" + attribute1.GetText() + "] " + comparisonOperator.GetText() + " '" + conditionDate.GetText() + "' " + logicalTextAux;
	} else if (code_typeFiltersConfigurationAux == "Text") {
		queryTextAux += " [" + attribute1.GetText() + "] " + comparisonOperator.GetText() + " '" + condition.GetText() + "' " + logicalTextAux;
	} else if (code_typeFiltersConfigurationAux == "Num") {
		var numWithPoint = conditionNum.GetText();
		numWithPoint = numWithPoint.replace(",", ".");
		queryTextAux += " [" + attribute1.GetText() + "] " + comparisonOperator.GetText() + " " + numWithPoint + " " + logicalTextAux;
	}

	query.SetText(queryTextAux);
}

function OnBtnSearchPopup_ClickAdvancedFilter(s, e) {
	$.ajax({
		url: "ProductionLot/ValidQuery",
		type: "post",
		data: { query: query.GetText() },
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
			if (result.isValidQuery) {
				$("#GridMessageErrorsDetail").hide();
				var url = "ProductionLot/GetResultsAdvancedFilter";
				var codeAdvancedFiltersConfigurationAux = $("#codeAdvancedFiltersConfiguration").val();
				if (codeAdvancedFiltersConfigurationAux == "OC") {
					url = "PurchaseOrder/GetResultsAdvancedFilter";
				} else
					if (codeAdvancedFiltersConfigurationAux == "RP") {
						url = "ProductionLotReception/GetResultsAdvancedFilter";
					}
				$.ajax({
					url: url,
					type: "post",
					data: null,
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
						popupAdvancedFilter.Hide();
						if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
							$("#btnCollapse").click();
						}
						$("#results").html(result);
					},
					complete: function () {
						hideLoading();
					}
				});
			} else {

				var msgErrorAux = ErrorMessage(result.message);
				gridMessageErrorsDetail.SetText(msgErrorAux);
				$("#GridMessageErrorsDetail").show();
				hideLoading();
			}
		},
		complete: function () {
		}
	});
}

function OnBtnClearPopup_ClickAdvancedFilter(s, e) {
	attribute1.SetValue(null);
	comparisonOperator.SetValue(null);
	conditionID.SetValue(null);
	logical.SetValue(null);
	conditionDate.SetDate(null);
	condition.SetText("");
	conditionNum.SetValue(null);

	query.SetText("");
}

function OnBtnClosePopup_ClickAdvancedFilter(s, e) {
	popupAdvancedFilter.Hide();
}

function ComparisonOperatorComboBox_BeginCallback(s, e) {
	e.customArgs["id_typeFiltersConfiguration"] = $("#id_typeFiltersConfiguration").val();
	// 
	//e.customArgs["editingEtapa"] = (e.command === "STARTEDIT");
	//if (e.command === "STARTEDIT") {
	//    e.customArgs['valueConditionSelectValueText'] = valueConditionSelect.GetValue();
	//} else {
	//    e.customArgs['valueConditionSelectValueText'] = "";
	//}

}

function ConditionIDComboBox_BeginCallback(s, e) {
	e.customArgs["datasource_advancedFiltersConfiguration"] = $("#datasource_advancedFiltersConfiguration").val();
}

function OnPopupAdvancedFilterBeginCallback(s, e) {
	e.customArgs["codeAdvancedFiltersConfiguration"] = $("#codeAdvancedFiltersConfiguration").val();

}

function RefreshVisibleCondition(code_typeFiltersConfiguration) {
	//SetElementVisibility("conditionDate", false);
	//conditionDate.SetVisible(false);
	var item = formLayoutEditAdvancedFilter.GetItemByName("conditionDate");
	item.SetVisible(false);
	item = formLayoutEditAdvancedFilter.GetItemByName("condition");
	item.SetVisible(false);

	item = formLayoutEditAdvancedFilter.GetItemByName("conditionNum");
	item.SetVisible(false);
	item = formLayoutEditAdvancedFilter.GetItemByName("conditionID");
	item.SetVisible(false);

	if (code_typeFiltersConfiguration == "Sel" || code_typeFiltersConfiguration == "Check") {

		item = formLayoutEditAdvancedFilter.GetItemByName("conditionID");
		item.SetVisible(true);

	} else if (code_typeFiltersConfiguration == "Date") {

		item = formLayoutEditAdvancedFilter.GetItemByName("conditionDate");
		item.SetVisible(true);

	} else if (code_typeFiltersConfiguration == "Text") {

		item = formLayoutEditAdvancedFilter.GetItemByName("condition");
		item.SetVisible(true);

	} else if (code_typeFiltersConfiguration == "Num") {

		item = formLayoutEditAdvancedFilter.GetItemByName("conditionNum");
		item.SetVisible(true);

	}
}

function Attribute1ComboBox_SelectedIndexChanged(s, e) {
	$.ajax({
		url: "ProductionLot/GetAdvancedFiltersConfiguration",
		type: "post",
		data: { id_advancedFiltersConfiguration: s.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();

		},
		success: function (result) {
			//resultFunction = result.enabledBtnGenerateLot;
			RefreshVisibleCondition(result.code_typeFiltersConfiguration);
			$("#code_typeFiltersConfiguration").val(result.code_typeFiltersConfiguration);
			$("#id_typeFiltersConfiguration").val(result.id_typeFiltersConfiguration);
			comparisonOperator.PerformCallback();

			if (result.code_typeFiltersConfiguration == "Sel") {
				//RefreshVisibleCondition(result.code_typeFiltersConfiguration);
				$("#datasource_advancedFiltersConfiguration").val(result.datasource_advancedFiltersConfiguration);
				conditionID.PerformCallback();
			} else if (result.code_typeFiltersConfiguration == "Check") {
				$("#datasource_advancedFiltersConfiguration").val("DataProviderAdvancedFilter.CheckDataSource");
				conditionID.PerformCallback();
				//$("#GridMessageErrorPurchaseRequest").show();
				//hideLoading();
			}
			//gvLeft.UnselectRows();
			//gvLeft.PerformCallback();
			////gvFilterBoxDateGridViewRight.UnselectRows();
			//gvRight.PerformCallback();
		},
		complete: function () {
			hideLoading();
			//gvProductionLotReceptions.PerformCallback();
			// gvPurchaseOrders.UnselectRows();
		}
	});


	//var id_logicalOperatorDateAux = id_logicalOperatorDate.GetValue();
	//if (id_logicalOperatorDateAux != "7" && id_logicalOperatorDateAux != 7) {
	//    valueConditionToDateTime.SetValue(null);
	//    valueConditionToDateTime.SetEnabled(false);
	//} else {
	//    var valueConditionFromDateTimeAux = valueConditionFromDateTime.GetValue();
	//    valueConditionToDateTime.SetValue(valueConditionFromDateTimeAux);
	//    valueConditionToDateTime.SetEnabled(true);
	//};
}


//Filter function Auxiliar 

function ValueConditionToDateTime_Init(s, e) {
	var id_logicalOperatorDateAux = id_logicalOperatorDate.GetValue();
	if (id_logicalOperatorDateAux != "7" && id_logicalOperatorDateAux != 7) {
		s.SetEnabled(false);
	};
}

function LogicalOperatorDate_SelectedIndexChanged(s, e) {
	var id_logicalOperatorDateAux = id_logicalOperatorDate.GetValue();
	if (id_logicalOperatorDateAux != "7" && id_logicalOperatorDateAux != 7) {
		valueConditionToDateTime.SetValue(null);
		valueConditionToDateTime.SetEnabled(false);
	} else {
		var valueConditionFromDateTimeAux = valueConditionFromDateTime.GetValue();
		valueConditionToDateTime.SetValue(valueConditionFromDateTimeAux);
		valueConditionToDateTime.SetEnabled(true);
	};
}

function ValueConditionToDecimal_Init(s, e) {
	var id_logicalOperatorNumberAux = id_logicalOperatorNumber.GetValue();
	if (id_logicalOperatorNumberAux != "7" && id_logicalOperatorNumberAux != 7) {
		s.SetEnabled(false);
	};
}

function LogicalOperatorNumber_SelectedIndexChanged(s, e) {
	var id_logicalOperatorNumberAux = id_logicalOperatorNumber.GetValue();
	if (id_logicalOperatorNumberAux != "7" && id_logicalOperatorNumberAux != 7) {
		valueConditionToDecimal.SetValue(null);
		valueConditionToDecimal.SetEnabled(false);
	} else {
		var valueConditionFromDecimalAux = valueConditionFromDecimal.GetValue();
		valueConditionToDecimal.SetValue(valueConditionFromDecimalAux);
		valueConditionToDecimal.SetEnabled(true);
	};
}

function GridViewFilterBoxDateLeft_OnGridViewEndCallback(s, e) {
	// 
	//gvFilterBoxDateGridViewRight.UnselectRows();
	//gvFilterBoxDateGridViewRight.PerformCallback();
}

function btnToRight_click(s, e, gvLeft, gvRight) {

	//event.preventDefault();
	gvLeft.GetSelectedFieldValues("id", function (values) {

		var lengthAux = values.length;
		if (lengthAux > 0) {
			// 
			var selectedRows = [];

			for (var i = 0; i < lengthAux; i++) {
				selectedRows.push(values[i]);
			}

			var data = {
				ids: selectedRows
			};

			$.ajax({
				url: "ProductionLot/MoveIdsLeftToRight",
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
					//resultFunction = result.enabledBtnGenerateLot;
					//if (result.Message == "OK") {
					//    showPage("PurchaseOrder/FormEditPurchaseOrder", data);
					//} else {
					//    gridMessageErrorPurchaseRequest.SetText(result.Message);
					//    $("#GridMessageErrorPurchaseRequest").show();
					//    hideLoading();
					//}
					gvLeft.UnselectRows();
					gvLeft.PerformCallback();
					//gvFilterBoxDateGridViewRight.UnselectRows();
					gvRight.PerformCallback();
				},
				complete: function () {
					hideLoading();
					//gvProductionLotReceptions.PerformCallback();
					// gvPurchaseOrders.UnselectRows();
				}
			});
		}

	});

}

function btnToRightDate_click(s, e) {
	btnToRight_click(s, e, gFilterBoxDateGridViewLeft, gvFilterBoxDateGridViewRight);
	//event.preventDefault();
	//gFilterBoxDateGridViewLeft.GetSelectedFieldValues("id", function (values) {

	//    var lengthAux = values.length;
	//    if (lengthAux > 0) {
	//        // 
	//        var selectedRows = [];

	//        for (var i = 0; i < lengthAux; i++) {
	//            selectedRows.push(values[i]);
	//        }

	//        var data = {
	//            ids: selectedRows
	//        };

	//        $.ajax({
	//            url: "ProductionLot/MoveIdsLeftToRight",
	//            type: "post",
	//            data: { ids: selectedRows },
	//            async: true,
	//            cache: false,
	//            error: function (error) {
	//                console.log(error);
	//            },
	//            beforeSend: function () {
	//                showLoading();

	//            },
	//            success: function (result) {
	//                //resultFunction = result.enabledBtnGenerateLot;
	//                //if (result.Message == "OK") {
	//                //    showPage("PurchaseOrder/FormEditPurchaseOrder", data);
	//                //} else {
	//                //    gridMessageErrorPurchaseRequest.SetText(result.Message);
	//                //    $("#GridMessageErrorPurchaseRequest").show();
	//                //    hideLoading();
	//                //}
	//                gFilterBoxDateGridViewLeft.UnselectRows();
	//                gFilterBoxDateGridViewLeft.PerformCallback();
	//                //gvFilterBoxDateGridViewRight.UnselectRows();
	//                gvFilterBoxDateGridViewRight.PerformCallback();
	//            },
	//            complete: function () {
	//                hideLoading();
	//                //gvProductionLotReceptions.PerformCallback();
	//                // gvPurchaseOrders.UnselectRows();
	//            }
	//        });
	//    }

	//});

}

function btnToRightText_click(s, e) {
	btnToRight_click(s, e, gFilterBoxTextGridViewLeft, gvFilterBoxTextGridViewRight);
}

function btnToRightNumber_click(s, e) {
	btnToRight_click(s, e, gFilterBoxNumberGridViewLeft, gvFilterBoxNumberGridViewRight);
}

function btnToRightSelect_click(s, e) {
	btnToRight_click(s, e, gFilterBoxSelectGridViewLeft, gvFilterBoxSelectGridViewRight);
}

function btnToRightCheck_click(s, e) {
	btnToRight_click(s, e, gFilterBoxCheckGridViewLeft, gvFilterBoxCheckGridViewRight);
}

function btnToLeft_click(s, e, gvLeft, gvRight) {

	//event.preventDefault();
	gvRight.GetSelectedFieldValues("id", function (values) {
		var lengthAux = values.length;
		if (lengthAux > 0) {
			// 
			var selectedRows = [];

			for (var i = 0; i < lengthAux; i++) {
				selectedRows.push(values[i]);
			}

			var data = {
				ids: selectedRows
			};

			$.ajax({
				url: "ProductionLot/MoveIdsRightToLeft",
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
					//resultFunction = result.enabledBtnGenerateLot;
					//if (result.Message == "OK") {
					//    showPage("PurchaseOrder/FormEditPurchaseOrder", data);
					//} else {
					//    gridMessageErrorPurchaseRequest.SetText(result.Message);
					//    $("#GridMessageErrorPurchaseRequest").show();
					//    hideLoading();
					//}
					//gFilterBoxDateGridViewLeft.UnselectRows();
					gvLeft.PerformCallback();
					gvRight.UnselectRows();
					gvRight.PerformCallback();
				},
				complete: function () {
					hideLoading();
					//gvProductionLotReceptions.PerformCallback();
					// gvPurchaseOrders.UnselectRows();
				}
			});
		}

	});

}

function btnToLeftDate_click(s, e) {

	btnToLeft_click(s, e, gFilterBoxDateGridViewLeft, gvFilterBoxDateGridViewRight);
	//event.preventDefault();
	//gvFilterBoxDateGridViewRight.GetSelectedFieldValues("id", function (values) {
	//    var lengthAux = values.length;
	//    if (lengthAux > 0) {
	//        // 
	//        var selectedRows = [];

	//        for (var i = 0; i < lengthAux; i++) {
	//            selectedRows.push(values[i]);
	//        }

	//        var data = {
	//            ids: selectedRows
	//        };

	//        $.ajax({
	//            url: "ProductionLot/MoveIdsRightToLeft",
	//            type: "post",
	//            data: { ids: selectedRows },
	//            async: true,
	//            cache: false,
	//            error: function (error) {
	//                console.log(error);
	//            },
	//            beforeSend: function () {
	//                showLoading();

	//            },
	//            success: function (result) {
	//                //resultFunction = result.enabledBtnGenerateLot;
	//                //if (result.Message == "OK") {
	//                //    showPage("PurchaseOrder/FormEditPurchaseOrder", data);
	//                //} else {
	//                //    gridMessageErrorPurchaseRequest.SetText(result.Message);
	//                //    $("#GridMessageErrorPurchaseRequest").show();
	//                //    hideLoading();
	//                //}
	//                //gFilterBoxDateGridViewLeft.UnselectRows();
	//                gFilterBoxDateGridViewLeft.PerformCallback();
	//                gvFilterBoxDateGridViewRight.UnselectRows();
	//                gvFilterBoxDateGridViewRight.PerformCallback();
	//            },
	//            complete: function () {
	//                hideLoading();
	//                //gvProductionLotReceptions.PerformCallback();
	//                // gvPurchaseOrders.UnselectRows();
	//            }
	//        });
	//    }

	//});

}

function btnToLeftText_click(s, e) {

	btnToLeft_click(s, e, gFilterBoxTextGridViewLeft, gvFilterBoxTextGridViewRight);
}

function btnToLeftNumber_click(s, e) {

	btnToLeft_click(s, e, gFilterBoxNumberGridViewLeft, gvFilterBoxNumberGridViewRight);
}

function btnToLeftSelect_click(s, e) {

	btnToLeft_click(s, e, gFilterBoxSelectGridViewLeft, gvFilterBoxSelectGridViewRight);
}

function btnToLeftCheck_click(s, e) {

	btnToLeft_click(s, e, gFilterBoxCheckGridViewLeft, gvFilterBoxCheckGridViewRight);
}

function ValueConditionSelect_BeginCallback(s, e) {
	e.customArgs["dataSource"] = gvFilterBoxSelectGridViewRight.cpEditingRowSelectDataSource;
	// 
	//e.customArgs["editingEtapa"] = (e.command === "STARTEDIT");
	//if (e.command === "STARTEDIT") {
	//    e.customArgs['valueConditionSelectValueText'] = valueConditionSelect.GetValue();
	//} else {
	//    e.customArgs['valueConditionSelectValueText'] = "";
	//}

}

function ValueConditionSelect_EndCallback(s, e) {
	$.ajax({
		url: "ProductionLot/GetSelectInit",
		type: "post",
		data: { key: gvFilterBoxSelectGridViewRight.cpEditingRowKey },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			s.SetValue(result.items);

		},
		complete: function () {
			//hideLoading();
			//gvProductionLotReceptions.PerformCallback();
			// gvPurchaseOrders.UnselectRows();
		}
	});

}

function ValueConditionSelect_Init(s, e) {
	s.PerformCallback();

}

function ValueConditionSelect_ValueChanged(s, e) {
	var valueAux = s.GetValue();
	$.ajax({
		url: "ProductionLot/UpdateFilterSelect",
		type: "post",
		data: { key: gvFilterBoxSelectGridViewRight.cpEditingRowKey, valueConditionSelectValue: valueAux.split(","), valueConditionSelectValueText: s.GetText() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();

		},
		success: function (result) {

		},
		complete: function () {
			//hideLoading();
			//gvProductionLotReceptions.PerformCallback();
			// gvPurchaseOrders.UnselectRows();
		}
	});
	//valueConditionSelectValueText.SetValue(s.GetValue());
}


function btnClearGlobal_click(s, e) {

	//event.preventDefault();
	$.ajax({
		url: "ProductionLot/FilterClearGlobal",
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
			//Date
			try {
				gFilterBoxDateGridViewLeft.UnselectRows();
				gFilterBoxDateGridViewLeft.PerformCallback();
				gvFilterBoxDateGridViewRight.UnselectRows();
				gvFilterBoxDateGridViewRight.PerformCallback();
			} catch (e) {

			}

			//Text
			try {
				gFilterBoxTextGridViewLeft.UnselectRows();
				gFilterBoxTextGridViewLeft.PerformCallback();
				gvFilterBoxTextGridViewRight.UnselectRows();
				gvFilterBoxTextGridViewRight.PerformCallback();
			} catch (e) {

			}

			//Number
			try {
				gFilterBoxNumberGridViewLeft.UnselectRows();
				gFilterBoxNumberGridViewLeft.PerformCallback();
				gvFilterBoxNumberGridViewRight.UnselectRows();
				gvFilterBoxNumberGridViewRight.PerformCallback();
			} catch (e) {

			}

			//Select
			try {
				gFilterBoxSelectGridViewLeft.UnselectRows();
				gFilterBoxSelectGridViewLeft.PerformCallback();
				gvFilterBoxSelectGridViewRight.UnselectRows();
				gvFilterBoxSelectGridViewRight.PerformCallback();
			} catch (e) {

			}

			//Check
			try {
				gFilterBoxCheckGridViewLeft.UnselectRows();
				gFilterBoxCheckGridViewLeft.PerformCallback();
				gvFilterBoxCheckGridViewRight.UnselectRows();
				gvFilterBoxCheckGridViewRight.PerformCallback();
			} catch (e) {

			}
		},
		complete: function () {
			hideLoading();
			//gvProductionLotReceptions.PerformCallback();
			// gvPurchaseOrders.UnselectRows();
		}
	});

}

//Global function Auxiliar 
function WarningMessage(text) {
	var message = "<div id='warningMessage' class='alert alert-warning alert-dismissible fade in' style='margin-top: 10px; text-align: center; padding: 10px 15px;'>"
		+ "<button type='button' class='close' data-dismiss='alert' aria-label='close' title='close' style='top: 0px; right: 0px;'><span aria-hidden='true'>&times;</span></button>"
		+ text
		+ "</div>";
	return message;
}

function ErrorMessage(text) {
	var message = "<div id='errorMessage' class='alert alert-danger alert-dismissible fade in' style='margin-top: 10px; text-align: center; padding: 10px 15px;'>"
		+ "<button type='button' class='close' data-dismiss='alert' aria-label='close' title='close' style='top: 0px; right: 0px;'><span aria-hidden='true'>&times;</span></button>"
		+ text
		+ "</div>";
	return message;
}

function UpdateDetailObjects(id_object, objects, fields) {
	////// 
	for (var i = 0; i < id_object.GetItemCount(); i++) {
		var object = id_object.GetItem(i);
		var into = false;
		for (var j = 0; j < objects.length; j++) {

			if (object.value == objects[j].id) {
				into = true;
				break;
			}
		}
		if (!into) {
			id_object.RemoveItem(i);
			i -= 1;
		}
	}


	for (var i = 0; i < objects.length; i++) {
		var object = id_object.FindItemByValue(objects[i].id);
		var arrayStr = [];
		for (var j = 0; j < fields.length; j++) {
			arrayStr.push(objects[i][fields[j]]);
		}
		//arrayStr.push(objects[i].name);
		//arrayStr.push(objects[i].clase);
		//arrayStr.push(objects[i].size);
		if (object == null) id_object.AddItem(arrayStr, objects[i].id);
	}

}

function UpdateDepartament(id_employee, employeeDepartament, tempDataKeep) {
	$.ajax({
		url: "ProductionLot/UpdateDepartament",
		type: "post",
		data: {
			id_employee: id_employee,
			tempDataKeep: tempDataKeep
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null && result != undefined) {
				employeeDepartament.SetText(result.employeeDepartament);
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
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

function GetStringFromDate(dt_in, format_in) {
	//// 
	var _str_res = "";
	if (format_in == "dd/MM/yyyy") {
		var tmp = dt_in.split("/");
		if (tmp.length == 3) {
			_str_res = tmp[2] + "-" + tmp[1] + "-" + tmp[0];
		}
	}
	return _str_res;
}

// GLOBAL CALLBACK FUNCTION

var CallBackFunction = null;

var UpdateViewCallback = null;

// SHOW AJAX PAGE
var pagesShown = [];

function showPagefromLink(_url, data) {

	if (data.toReturn != "True" && data.toReturn != "true" && data.toReturn != true) {
		var maincontentCurrent = {
			id: pagesShown.length,
			urlToReturn: data.urlToReturn,
			tabSelected: data.tabSelected,
			arrayTempDataKeep: pagesShown.length == 0 ? data.arrayTempDataKeep : pagesShown[pagesShown.length - 1].arrayTempDataKeep.push(data.arrayTempDataKeep[0])
		}
		data.arrayTempDataKeep = maincontentCurrent.arrayTempDataKeep;
		pagesShown.push(maincontentCurrent);
	}
	showLoading();
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			
		},
		success: function (result) {
			$("#maincontent").html(result);
		},
		complete: function () {
			hideLoading();
		}
	});

}

//ToReturn(Link)

function btnToReturn_click(s, e) {
	var maincontentCurrent = pagesShown.pop();

	if (pagesShown.length > 0) {
		var data = {
			id: 0,
			toReturn: true,
			tabSelected: maincontentCurrent.tabSelected,
			arrayTempDataKeep: pagesShown[pagesShown.length - 1].arrayTempDataKeep
		};
		ViewData["arrayTempDataKeep"] = pagesShown[pagesShown.length - 1].arrayTempDataKeep;
		showPagefromLink(maincontentCurrent.urlToReturn, data);
	} else {
		var data = {
			id: 0,
			toReturn: true,
			tabSelected: maincontentCurrent.tabSelected
		};
		$.ajax({
			url: maincontentCurrent.urlToReturn,
			type: "post",
			data: data,
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
			},
			complete: function () {
				hideLoading();
			}
		});
	}






}

function openWindow(_url) {
	return window.open(_url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0, scrollbars=yes');
}

function showThickBox(_url, data/*, width, height*/) {
	//var widthAux = isNaN(width) ? '100%' : width;
	//var heightAux = isNaN(height) ? '100%' : height;
	//UpdateViewCallback = null;
	//pagesShown = [];
	//// 
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {

			$.fancybox(result
				, {
					scrolling: true,
					//padding: 0,
					//top:'25px',
					width: '800px',
					//'autoScale': false,
					// width: '',
					height: '700px',
					overflow: scroll,
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
					autoDimensions: false,

				}
			);
			//$("#maincontent").html(result);
		},
		complete: function () {
			hideLoading();
		}
	});

	//event.preventDefault();
}

function showPartialPage(divObject, _url, data) {
	pagesShown = [];
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			divObject.html(result);
		},
		complete: function () {
			hideLoading();
		}
	});
}

function showErrorTitle(error) {
    var e1 = error.responseText.split("<title>");
    if (e1.length > 1) {
        var e2 = e1[1].split("</title>");
        if (e2.length > 1) {
            NotifyError(e2[0]);
        }
    }
}

function showPage(_url, data) {
	pagesShown = [];
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
            console.log(error);
            showErrorTitle(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			$("#maincontent").html(result);

		},
		complete: function () {
			hideLoading();
		}
	});
}

function showPageCallBack(_url, data, precall, callback) {
	// 
	pagesShown = [];
	precall();
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			showErrorTitle(error);
			callback();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			callback();
			$("#maincontent").html(result);
			

		},
		complete: function () {
			hideLoading();
		}
	});
}
function showPageCallBackInit(_url, data, callback) {
	
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			showErrorTitle(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			
			$("#maincontent").html(result);

			$(function () {
				callback();
			});


		},
		complete: function () {
			hideLoading();
		}
	});
}

function showForm(_url, data, callback) {
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			$("#mainform").html(result);

			if (typeof callback !== 'undefined') {
				if (callback !== null) {
					callback();
				}
			}
		},
		complete: function () {
			hideLoading();
		}
	});

	event.preventDefault();
}

function showFormFunction(_url, data, callback) {
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			// 
			

		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {

			if (typeof callback !== undefined && callback != null && (callback instanceof Function)) {
				callback(result);
			}
			
		},
		complete: function () {
			hideLoading();
		}
	});

	event.preventDefault();
}
// DOCUMENT DIALOG

var id = 0;
var data = null;
function PopupBeginCallback(s, e) {
	e.customArgs = {
		id: id,
		data: JSON.stringify(data)
	};
}

function showDocumentDialog(title, documentId, documentData) {
	id = documentId;
	data = documentData;
	popupDocument.PerformCallback();
	popupDocument.SetHeaderText(title);
	popupDocument.Show();
}

function hideDocumentDialog() {
	popupDocument.Hide();
}

// LOADING

function showLoading() {
	LoadingPanel.Show();
}

function hideLoading(parameters) {
	LoadingPanel.Hide();
}

// CONFIRMATION DIALOG

function GetDialogMessage(text) {
	return "<div style=\"padding-left: 5px;\">"
		+ "<table style=\"width: 100%;\">"
		+ "<tr>"
		+ "<td style=\"width: 15%;\">"
		+ "<span class=\"glyphicon glyphicon-warning-sign warning\" style=\"color: #ff8800; font-size: 30px;\"></span>"
		+ "</td>"
		+ "<td style=\"text-align: left; vertical-align: middle;\">"
		+ text
		+ "</td>"
		+ "</tr>"
		+ "</table>"
		+ "</div>";
}

function showConfirmationDialog(confirmation_function, message) {

	var text = "<p>Se dispone a borrar el/los registro(s) seleccionado(s).</p><p>¿Está seguro?</p>";
	if (message !== undefined && message !== null) {
		text = GetDialogMessage(message);
	}
	else {
		text = GetDialogMessage(text);
	}
	confirmDialog.SetContentHtml(text);
	confirmDialog.Show();

	$("#" + btnConfirmOk.name).unbind("click");
	$("#" + btnConfirmOk.name).bind("click", function () {
		confirmation_function();
		confirmDialog.Hide();
	});
}

function showCopyConfirmationDialog(confirmation_function, message) {

	var text = "<p>Se dispone a copiar el registro seleccionado.</p><p>¿Está seguro?</p>";
	if (message !== undefined && message !== null) {
		text = GetDialogMessage(message);
	}
	else {
		text = GetDialogMessage(text);
	}
	confirmDialog.SetContentHtml(text);
	confirmDialog.Show();

	$("#" + btnConfirmOk.name).unbind("click");
	$("#" + btnConfirmOk.name).bind("click", function () {
		confirmation_function();
		confirmDialog.Hide();
	});
}

function showCopyFormulationConfirmationDialog(confirmation_function, message) {

	var text = "<p>Se dispone a copiar las fórmulas del Producto Origen a los registros seleccionados.</p><p>¿Está seguro?</p>";
	if (message !== undefined && message !== null) {
		text = GetDialogMessage(message);
	}
	else {
		text = GetDialogMessage(text);
	}
	confirmDialog.SetContentHtml(text);
	confirmDialog.Show();

	$("#" + btnConfirmOk.name).unbind("click");
	$("#" + btnConfirmOk.name).bind("click", function () {
		confirmation_function();
		confirmDialog.Hide();
	});
}

function hideConfirmationDialog() {
	confirmDialog.Hide();
}

function ConfirmationDialogButtonCancel_Click(s, e) {
	hideConfirmationDialog();
}

// GLOBAL VALIDATIONS
function OnValidation(s, e) {
	e.isValid = true;
}

function validarRUC(ruc) {

	var regExp = new RegExp("[^0-9]", "i")
	if (ruc.length != 13) {;

		return { isValid: false, errorText: "El ruc debe tener 13 dígitos" };
	}

	if (regExp.test(ruc)) {
		return { isValid: false, errorText: "Solo se admiten dígitos" };
	}

	var tipoPersona = parseInt(ruc[2]);
	if (tipoPersona > 6 && tipoPersona != 9) {
		return { isValid: false, errorText: "El 3er dígito del ruc debe ser menor que 6 ó 9" };
	}

	var provCode = parseInt(ruc.substr(0, 2));
	if (provCode < 1 || provCode > 50) {
		return { isValid: false, errorText: "Código de provincia emisora errado" };
	}

	if (ruc.substr(10, 13) === "000") {
		return { isValid: false, errorText: "Los tres últimos dígitos no pueden ser 000" };
	}

	var digitoAutoverificador = 0;
	var coeficientes = [];
	// 
	if (tipoPersona < 6) {
		digitoAutoverificador = ruc[9];
		coeficientes = [2, 1, 2, 1, 2, 1, 2, 1, 2];

		var sum = 0;
		for (var i = 0; i < coeficientes.length; i++) {
			var value = parseInt(coeficientes[i]) * parseInt(ruc[i]);
			if (value > 9) {
				sum += value % 10 + 1;
			} else {
				sum += value;
			}
		}

		var residuo = sum % 10;
		if (residuo == 0 && digitoAutoverificador == 0) {
			return { isValid: true, errorText: null };
		}

		if (10 - sum % 10 != digitoAutoverificador) {
			return { isValid: false, errorText: "Número de RUC incorrecto" };
		}

	} else if (tipoPersona == 6) {
		var error = false;
		digitoAutoverificador = ruc[8];
		coeficientes = [3, 2, 7, 6, 5, 4, 3, 2];

		var sum = 0;
		for (var i = 0; i < coeficientes.length; i++) {
			var value = parseInt(coeficientes[i]) * parseInt(ruc[i]);
			sum += value;
		}

		var residuo = sum % 11;

		if (residuo == 0 && digitoAutoverificador == 0) {
			return { isValid: true, errorText: null };
		}

		if (11 - sum % 11 != digitoAutoverificador) {
			error = true;
			//return { isValid: false, errorText: "Número de RUC incorrecto" };
		}

		if (error)
		{
			digitoAutoverificador = ruc[9];
			coeficientes = [2, 1, 2, 1, 2, 1, 2, 1, 2];

			var sum = 0;
			for (var i = 0; i < coeficientes.length; i++) {
				var value = parseInt(coeficientes[i]) * parseInt(ruc[i]);
				if (value > 9) {
					sum += value % 10 + 1;
				} else {
					sum += value;
				}
			}

			var residuo = sum % 10;
			if (residuo == 0 && digitoAutoverificador == 0) {
				return { isValid: true, errorText: null };
			}

			if (10 - sum % 10 != digitoAutoverificador) {
				return { isValid: false, errorText: "Número de RUC incorrecto" };
			}
				
		}
	}
	else {
		if (tipoPersona != 9) {
		digitoAutoverificador = ruc[9];
		coeficientes = [4, 3, 2, 7, 6, 5, 4, 3, 2];

		var sum = 0;
		for (var i = 0; i < coeficientes.length; i++) {
			var value = parseInt(coeficientes[i]) * parseInt(ruc[i]);
			sum += value;
		}

		var residuo = sum % 11;

		if (residuo == 0 && digitoAutoverificador == 0) {
			return { isValid: true, errorText: null };
		}

		if (11 - sum % 11 != digitoAutoverificador) {
			return { isValid: false, errorText: "Número de RUC incorrecto" };
			}
		}
	}

	return { isValid: true, errorText: null };
}

function validarCI(cedula) {

	var regExp = new RegExp("[^0-9]", "i");

	if (cedula.length != 10) {
		return { isValid: false, errorText: "La cédula debe tener 10 dígitos" };
	}

	if (regExp.test(cedula)) {
		return { isValid: false, message: "Solo se admiten dígitos" };
	}

	var tipoPersona = parseInt(cedula[2]);
	if (tipoPersona > 6) {
		return { isValid: false, errorText: "El 3er dígito del ruc debe ser menor que 7" };
	}

	var provCode = parseInt(cedula.substr(0, 2));
	if (provCode < 1 || provCode > 50) {
		return { isValid: false, errorText: "Código de provincia emisora errado" };
	}

	var digitoAutoverificador = cedula[9];
	var coeficientes = [2, 1, 2, 1, 2, 1, 2, 1, 2];

	var sum = 0;
	for (var i = 0; i < coeficientes.length; i++) {
		var value = parseInt(coeficientes[i]) * parseInt(cedula[i]);
		if (value > 9) {
			//sum += value % 10 + 1;
			sum += (value - 9);
		} else {
			sum += value;
		}
	}
	var digitoVerificadorObtenido = 0;
	if (sum >= 10) {
		if (sum % 10 != 0) {
			digitoVerificadorObtenido = (10 - (sum % 10));
		} else {
			digitoVerificadorObtenido = (sum % 10)
		}
	} else {
		digitoVerificadorObtenido = sum;
	}
	if (digitoVerificadorObtenido != digitoAutoverificador) {
		return { isValid: false, errorText: "Número de cédula incorrecto" };
	}
	return { isValid: true, errorText: null };
}

function validarEMAIL(email) {
	//var regexp = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
	var regexp = /^(([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+)(([\s]*[;]+[\s]*(([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+))*)$/;

	if (!regexp.test(email)) {
		return { isValid: false, errorText: "Correo electrónico incorrecto" };
	}

	return { isValid: true, errorText: null };
}

function OnRangeDateValidation(e, startDate, endDate, msgError) {
	var validated = ValidateRangeDate(startDate, endDate);
	if (!validated) {
		e.isValid = validated;
		e.errorText = msgError;
	}
	return validated;
}

function ValidateRangeDate(startDate, endDate) {
	return (startDate == null || endDate == null || startDate <= endDate);
}

function OnRangeTimeValidation(e, startTime, endTime, msgError, includeEquality) {
	var validated = ValidateRangeTime(startTime, endTime, includeEquality);
	if (!validated) {
		e.isValid = validated;
		e.errorText = msgError;
	}
	return validated;
}

function ValidateRangeTime(startTime, endTime, includeEquality) {
	var startTimeAux = startTime != null ? new Date(2011, 1, 1, startTime.getHours(), startTime.getMinutes(), startTime.getSeconds()) : null;
	var endTimeAux = endTime != null ? new Date(2011, 1, 1, endTime.getHours(), endTime.getMinutes(), endTime.getSeconds()) : null;
	return (startTime == null || endTime == null || (includeEquality == true || includeEquality == null || includeEquality == undefined ? startTimeAux <= endTimeAux : startTimeAux < endTimeAux));

}

function OnRangeNumberValidation(e, value, valueMin, valueMax, msgError) {
	var validated = ValidateRangeNumber(value, valueMin, valueMax);
	if (!validated) {
		e.isValid = validated;
		e.errorText = msgError;
	}
	return validated;
}

function ValidateRangeNumber(value, valueMin, valueMax) {
	return (value >= valueMin && value <= valueMax);
}

function OnEmissionDateDocumentValidation(e, _emissionDate, tempDataKeep) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo obligatorio";
	} else {
		var strEmissionDate = String(_emissionDate.GetDate());
		var strEmissionDateDiv2Points = strEmissionDate.split(":");
		var strEmissionDateDiv2PointsWithSpace = strEmissionDateDiv2Points[2].split(" ");
		var data = {
			emissionDate: JSON.stringify(strEmissionDateDiv2Points[0] + ":" + strEmissionDateDiv2Points[1] + ":" + strEmissionDateDiv2PointsWithSpace[0]),//"Mon Jul 10 2017 20:07:05"),//_emissionDate.GetDate()),
			tempDataKeep: tempDataKeep
		};
		$.ajax({
			url: "ProductionLot/OnEmissionDateDocumentValidation",
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
					if (result.itsValided == 0) {
						e.isValid = false;
						e.errorText = result.Error;
					}
				}
			},
			complete: function () {
				//hideLoading();
			}
		});

	}
}

function IsValidateTimeInRange(e, timeInitRange, timeEndRange, timeCurrent, msg, equalTimeInitRange, equalTimeEndRange) {
	//var timeInitDetailAux = new Date("2011-01-01T" + value);
	var time00Aux = new Date("2011-01-01T00:00:00");
	var time11Aux = new Date("2011-01-01T11:59:59");
	//var time12Aux = new Date("2011-01-01T12:00:00");
	//var time23Aux = new Date("2011-01-01T23:59:59");
	var diurnoTimeInitRange = false;
	var diurnoTimeEndRange = false;
	var diurnoTimeCurrent = false;

	var mayorTimeCurrentTimeInitRange = false;
	var menorTimeCurrentTimeEndRange = false;
	var valido = false;
	//console.log("timeCurrent: " + timeCurrent);
	valido = OnRangeTimeValidation(e, time00Aux, timeInitRange, msg);
	if (valido) {
		valido = OnRangeTimeValidation(e, timeInitRange, time11Aux, msg);
		if (valido) {
			diurnoTimeInitRange = valido;
		}
	}

	valido = OnRangeTimeValidation(e, time00Aux, timeEndRange, msg);
	if (valido) {
		valido = OnRangeTimeValidation(e, timeEndRange, time11Aux, msg);
		if (valido) {
			diurnoTimeEndRange = valido;
		}
	}

	valido = OnRangeTimeValidation(e, time00Aux, timeCurrent, msg);
	if (valido) {
		valido = OnRangeTimeValidation(e, timeCurrent, time11Aux, msg);
		if (valido) {
			diurnoTimeCurrent = valido;
		}
	}

	valido = OnRangeTimeValidation(e, timeInitRange, timeCurrent, msg, (equalTimeInitRange == true));
	mayorTimeCurrentTimeInitRange = valido;

	valido = OnRangeTimeValidation(e, timeCurrent, timeEndRange, msg, (equalTimeEndRange == true));
	menorTimeCurrentTimeEndRange = valido;

	valido = ((diurnoTimeInitRange == diurnoTimeCurrent && mayorTimeCurrentTimeInitRange) || diurnoTimeInitRange != diurnoTimeCurrent) &&
		((diurnoTimeEndRange == diurnoTimeCurrent && menorTimeCurrentTimeEndRange) || diurnoTimeEndRange != diurnoTimeCurrent);
	//console.log("valido3: " + valido);
	return valido;
}

// MENU

function custom_menu_click(s, e) {
	//// 
	UpdateViewCallback = null;

	var _url = $(this).attr("data-url");

	if (typeof s.GetMainElement === "function") {
		_url = $(s.GetMainElement()).attr("data-url");
	}
	if (_url !== null && _url !== "" && _url !== "/") {
		$.ajax({
			url: _url,
			type: "post",
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
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

// LOGOUT
function logout() {
	$.ajax({
		url: "Login/Logout",
		type: "post",
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			hideLoading();
			window.location.href = "/";
		},
		complete: function () {
			hideLoading();
		}
	});
}

// MENU
function renderSubMenu(subMenu) {
	var html = "";

	var controller = subMenu.controller;
	var action = subMenu.action;

	var submenues = subMenu.children;

	html += "<li class=\"\">";

	if (submenues.length === 0) {
		html += "<a href=\"javascript:;\" data-url=\"" + controller + "/" + action + "\" class=\"custom-menu-item\">";
	}
	else {
		html += "<a href=\"javascript:;\" data-url=\"" + controller + "/" + action + "\" class=\"dropdown-toggle\">";
	}

	html += "<i class=\"menu-icon fa fa-caret-right\"></i>";
	html += " " + subMenu.title + " ";

	if (submenues.length === 0) {
		html += "<b class=\"arrow\"></b>";
	}
	else {
		html += "<b class=\"arrow fa fa-angle-down\"></b>";
	}

	html += "</a>";

	if (submenues.length > 0) {
		html += "<b class=\"arrow\"></b>";
		html += "<ul class=\"submenu\">";

		for (var i = 0; i < submenues.length; i++) {
			html += renderSubMenu(submenues[i]);
		}

		html += "</ul>";
	}

	html += "</li>";

	return html;
}

function renderMenuItem(menuItem) {

	var html = "";

	var controller = menuItem.controller;
	var action = menuItem.action;

	var submenues = menuItem.children;

	if (menuItem.id_parent === null) {
		html += "<li class=\"\">";

		if (submenues.length === 0) {
			html += "<a href=\"javascript:;\" data-url=\"" + controller + "/" + action + "\" class=\"custom-menu-item\">";
		}
		else {
			html += "<a href=\"javascript:;\" data-url=\"" + controller + "/" + action + "\" class=\"dropdown-toggle\">";
		}


		html += "<i class=\"menu-icon fa fa-list\"></i>";
		html += "<span class=\"menu-text\"> " + menuItem.title + " </span>";

		if (submenues.length === 0) {
			html += "<b class=\"arrow\"></b>";
		}
		else {
			html += "<b class=\"arrow fa fa-angle-down\"></b>";
		}

		html += "</a>";

		if (submenues.length > 0) {
			html += "<b class=\"arrow\"></b>";
			html += "<ul class=\"submenu\">";

			for (var i = 0; i < submenues.length; i++) {
				html += renderSubMenu(submenues[i]);
			}

			html += "</ul>";
		}

		html += "</li>";
	}

	return html;
}

function renderTreeMenu(treeMenu) {
	var html = "";
	for (var i = 0; i < treeMenu.length; i++) {
		html += renderMenuItem(treeMenu[i]);
	}
	$("#treemenu").append($(html));
}

function loadMenu() {
	$.ajax({
		url: "Home/SideBarMenu",
		type: "post",
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			renderTreeMenu(result);
			$(".custom-menu-item").click(custom_menu_click);
		},
		complete: function () {
			hideLoading();
		}
	});
}

// FILE UPLOAD

function uploadFile(url, data, success) {
	$.ajax({
		url: url,
		type: "post",
		data: data,
		processData: false,
		contentType: false,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: success,
		complete: function () {
			hideLoading();
		}
	});
}


//Emission Point Changed

// MAIN

function init() {
	loadMenu();
	$("#logout").click(logout);
	//	$("#btn_go_notifications").click(GoToNotifications);

	$("#search-box").autocomplete({
		type: 'POST',
		serviceUrl: 'Menu/SearchMenu',
		showNoSuggestionNotice: true,
		noSuggestionNotice: "Sin resultados",
		lookupFilter: function (suggestion, query, queryLowerCase) {

		},
		autoSelectFirst: true,
		onInvalidateSelection: function (e) {
		},
		onSelect: function (suggestion) {
			showPage(suggestion.data.controller + "/" + suggestion.data.action);
			$("#search-box").val(null);
		}
	});
}

ASPxClientGridView.prototype.ExportToXlsx = function () {
	performGridViewToolbarButtonClick(this, 'ExportToXlsx');
};
ASPxClientGridView.prototype.ExportToDocx = function () {
	performGridViewToolbarButtonClick(this, 'ExportToDocx');
};
ASPxClientGridView.prototype.ExportToPdf = function () {
	performGridViewToolbarButtonClick(this, 'ExportToPdf');
};

// Ejecutar acción en toolbar de GridView
var performGridViewToolbarButtonClick = function (gridView, buttonText) {
	if (gridView.GetToolbar) {
		var toolbar = gridView.GetToolbar(0);
		if (toolbar) {
			var count = toolbar.GetItemCount();
			for (var i = 0; i < count; i++) {
				var button = toolbar.GetItemByName(buttonText);
				if (button !== null) {
					toolbar.DoItemClick(button.indexPath, false, null);
					break;
				}
			}
		}
	}
};
//SCRIPTS STANDAR
function OnUpdateImagenWhenRequiredField(s, e) {
	//// 
	var messageErrorControl = "";
	if (s.cpHasTab != undefined) {
		if (s.cpHasTab == "false") {
			if (s.cpIsRequired == "true") {
				if (e.value == null) {
					e.errorText = s.cpMessageError;
					e.isValid = false;
					return;
				} else {
					if (s.cpInitialCondition != undefined) {
						if (e.value == s.cpInitialCondition) {
							e.errorText = s.cpMessageError;
							e.isValid = false;
							return;
						}
					}
				}
			}
			if (s.cpMinimunLength != undefined) {
				if (e.value != null) {
					if (e.value.length < s.cpMinimunLength) {
						e.errorText = "La longitud Mínimo es " + s.cpMinimunLength;
						e.isValid = false;
						return;
					}
				}
			}
			if (s.cpMaximunLength != undefined) {
				if (e.value != null) {
					if (e.value.length > s.cpMaximunLength) {
						imageUrl = "/Content/image/info-error.png";
						e.errorText = "La Longitud Máximo es " + s.cpMaximunLength;
						if (tab !== null) {
							tab.SetImageUrl(imageUrl);
							tab.SetActiveImageUrl(imageUrl);
						}
						return;
					}
				}
			}
			if (!e.isValid) {
				if (s.cpMessageErrorFormart != undefined) {
					e.errorText = s.cpMessageErrorFormart;
					e.isValid = false;
					return;
				}
			}
		}
	} else {
		if (s.cpTabContainer == undefined || s.cpTabControl == undefined) {
			return;
		}
	}

	if (s.cpTabContainer == undefined || s.cpTabControl == undefined) {
		return;
	}
	var controls = ASPxClientControl.GetControlCollection();
	var genericTabControl = controls.GetByName(s.cpTabControl);
	var tab = genericTabControl.GetTabByName(s.cpTabContainer);

	if (tab === null || tab.GetVisible() === false) {
		e.isValid = true;
		return;
	}
	var imageUrl = "/Content/image/noimage.png";
	tab.SetImageUrl(imageUrl);
	tab.SetActiveImageUrl(imageUrl);

	if (s.cpIsRequired == "true") {
		if (e.value == null) {
			imageUrl = "/Content/image/info-error.png";
			e.errorText = s.cpMessageError;
			e.isValid = false;
			if (tab !== null) {
				tab.SetImageUrl(imageUrl);
				tab.SetActiveImageUrl(imageUrl);
			}
			return;
		} else {
			if (s.cpInitialCondition != undefined) {
				if (e.value == s.cpInitialCondition) {
					imageUrl = "/Content/image/info-error.png";
					e.errorText = s.cpMessageError;
					e.isValid = false;
					if (tab !== null) {
						tab.SetImageUrl(imageUrl);
						tab.SetActiveImageUrl(imageUrl);
					}
					return;
				}
			}
		}
	}
	if (s.cpMinimunLength != undefined) {
		if (s.cpMinimunLength != 0) {
			if (e.value != null) {
				if (e.value.length < s.cpMinimunLength) {
					imageUrl = "/Content/image/info-error.png";
					e.errorText = "La longitud Mínimo es " + s.cpMinimunLength;
					e.isValid = false;
					if (tab !== null) {
						tab.SetImageUrl(imageUrl);
						tab.SetActiveImageUrl(imageUrl);
					}
					return;
				}
			}
		}

	}
	if (s.cpMaximunLength != undefined) {
		if (e.value != null) {
			if (e.value.length > s.cpMaximunLength) {
				imageUrl = "/Content/image/info-error.png";
				e.errorText = "La Longitud Máximo es " + s.cpMaximunLength;
				e.isValid = false;
				if (tab !== null) {
					tab.SetImageUrl(imageUrl);
					tab.SetActiveImageUrl(imageUrl);
				}
				return;
			}
		}

	}
	if (!e.isValid) {
		if (s.cpMessageErrorFormart != undefined) {
			imageUrl = "/Content/image/info-error.png";
			e.errorText = s.cpMessageErrorFormart;
			e.isValid = false;
			if (tab !== null) {
				tab.SetImageUrl(imageUrl);
				tab.SetActiveImageUrl(imageUrl);
			}
			return;
		}
	}
}

function showFormPostBack(_url, data, callBack) {
	//// 
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			//// 
			$("#mainform").html(result);
			if (typeof callBack != "undefined") return;
			if (typeof callBack === 'function') {
				callBack();
			}
		},
		complete: function () {
			hideLoading();
		}
	});

	//// 
	if (typeof event != "undefined") {
		event.preventDefault();
	}

}

function GoToNotifications() {
	showPage("Notification/Index");
}

// Generador de mensajes
var _clearMessageTimerId = null;
var resetClearMessageTimer = function () {
	if (_clearMessageTimerId !== null) {
		clearTimeout(_clearMessageTimerId);
		_clearMessageTimerId = null;
	}
};
var showMessage = function (id, cssClass, text) {
	resetClearMessageTimer();

	var $button = $(
		'<button type="button" class="close" data-dismiss="alert" aria-label="close" title="close" style="top:0px;right:0px;">'
		+ '<span aria-hidden="true">&times;</span></button>');

	var $message = $('<div class="alert alert-dismissible fade in" style="margin-top:10px;text-align:center;padding:10px 15px;">')
		.attr("id", id).addClass(cssClass).text(text).append($button);

	$("#notification").empty().append($message);

	$button.find(".close").click(function () {
		resetClearMessageTimer();

		$("#" + id).alert('close');
		$("#notification").empty();
	});

	$(window).scrollTop(0);

	_clearMessageTimerId = setTimeout(function () {
		_clearMessageTimerId = null;
		$("#" + id).alert('close');
		$("#notification").empty();
	}, 10000);
};

function showPartialPageHide(divObject, _url, data) {
	pagesShown = [];
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			divObject.html(result);
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function showPartialPageHideArrayCallBack(divObject, _url, data, arraycallback,status) {
	pagesShown = [];
	$.ajax({
		url: _url,
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			divObject.html(result);
			
			if (typeof arraycallback !== undefined && arraycallback != null) {
				for (let i = 0; i < arraycallback.length; i++) {
					let tFunction = arraycallback[i];
					if (typeof tFunction !== undefined && tFunction != null && (tFunction instanceof Function)) {
						tFunction(status);
					}
				}
			}


		},
		complete: function () {
			//hideLoading();
		}
	});
}
function showInfoMessage(text) {
	showMessage("successMessage", "alert-info", text);
};
function showSuccessMessage (text) {
	showMessage("successMessage", "alert-success", text);
};
function showWarningMessage (text) {
	showMessage("warningMessage", "alert-warning", text);
};
function showErrorMessage (text) {
	showMessage("errorMessage", "alert-danger", text);
};
function showFormMessage (type, message) {
	if (message) {
		if (type === "danger") {
			showErrorMessage(message);
		} else if (type === "success") {
			showSuccessMessage(message);
		} else if (type === "warning") {
			showWarningMessage(message);
		} else {
			showInfoMessage(message);
		}
	}
};

function prepareMessageClient( cssClass, text )
{
	
	var $message = $('<div class="alert alert-dismissible fade in" style="margin-top:10px;text-align:center;padding:10px 15px;">')
				.attr("id", id).addClass(cssClass).text(text);
	return $message;

} 
function viewMessageClient(tagview, message) {
	debugger;
	if (message.length > 0) {
		
		$(`#${tagview}`).empty();

		$(`#${tagview}`).append(message)
			.show()
			.delay(5000)
			.hide(0);
	}
}

var _rootPath = "../../../";
var getFullPath = function (relativeUrl) {
	if (relativeUrl !== null && typeof relativeUrl === 'string' && relativeUrl.length > 0) {
		if (relativeUrl.slice(0, 1) === "/") {
			return _rootPath + relativeUrl.slice(1);
		} else {
			return _rootPath + relativeUrl;
		}
	}
	return _rootPath;
};

$(function () {
	init();
});

function messagePostProcessNotification(status, name) {
	
	if ((typeof status === undefined) ||  (typeof name === undefined) ) {
		NotifySuccess("Ha finalizado con éxito el proceso");
	}
	else {
		if (status == "PROCESADO") {
			NotifySuccess(`Ha finalizado con éxito el proceso ${name}`);
		}
		else if (status == "ERROR")
		{
			NotifyError(`Ha finalizado con error el proceso ${name}`);
		}
	}
}

function refreshProcessNotification(postFunction, codeType, thisTimer, status, nameProcess) {

	// 1 PostFunction
	// 2 Update State Notificaction
	// 3 End Timer
	

	let funtionReadNotification = function ()
	{
		console.log("Iniciar marcar como leido Notificacion");
		$.ajax({
			url: "Notification/UpdateNotificationAsRead",
			type: "post",
			data: {
				documentTypeCode: codeType,
			},
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				
				if (result !== null && result.ok == true) {
					console.log("Deteniendo Observer");
					thisTimer.detenerInside();
					
					thisTimer.messagePostProcessNotification(status, nameProcess );
				}
			},
			complete: function () {
			}
		});

	}
	const array_functions = [postFunction, funtionReadNotification ];

	let data = {
		id: 0
	};
	
	showPartialPageHideArrayCallBack($("#layout_notification_wrap"), "Notification/RefreshNotification", data, array_functions, status);
}

const CODE_FOR_SCHEDULE_TRANSAC = -6666;
const TRANSAC_FOR_QUEUE_MSG = "La operación se encuentra en proceso, se notificará al finalizar";

observerNotification = function (codeType, time, functionPost) {
	let continuar = true;

	console.log('Iniciando temporizador con estado...');
	let that = this;
	const temporizador = () => {
		return new Promise((resolve) => {

			if (continuar) {
				setTimeout(() => {
					$.ajax({
						url: "Notification/ValidateNotificationUnRead",
						type: "post",
						data: {
							documentTypeCode: codeType,
						},
						async: false,
						cache: false,
						error: function (error) {
							console.log(error);
						},
						beforeSend: function () {
						},
						success: function (result) {
							if (result !== null && result.countNot > 0) {
								refreshProcessNotification(functionPost, codeType, that, result.stateNotification, result.nameProcess  );
								//functionPost();

							}
						},
						complete: function () {
						}
					});
					// buscar notificaciones no leidas del tipo codeType
					// Si no existe sale, se reinicia el temporizador
					// Si existe ejecutar refreshProcessNotification
					//    ejecutar con show con Function para pasar el functionPost
					// ejecutar action para actualizar la notification a reader
					// detener

					resolve('EJECUTADO.');
				}, time);
			} else {
				resolve('Temporizador detenido.');
			}
		}).then((mensaje) => {
			console.log("Resultado THEN");
			console.log(mensaje);
			if (continuar) {

				temporizador();
			}
		});
	};

	temporizador();

	detenerInside = function () {
		continuar = false;
	},
		reanudarInside = function () {
			if (!continuar) {
				continuar = true;
				temporizador();
			}
		}

	return {
		detener: function () {
			continuar = false;
		},
		reanudar: function () {
			if (!continuar) {
				continuar = true;
				temporizador();
			}
		}
	};
}

