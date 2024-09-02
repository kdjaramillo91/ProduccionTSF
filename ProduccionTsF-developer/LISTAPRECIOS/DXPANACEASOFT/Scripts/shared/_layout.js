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
    var message = "<div id='warningMessage' class='alert alert-danger alert-dismissible fade in' style='margin-top: 10px; text-align: center; padding: 10px 15px;'>"
                  + "<button type='button' class='close' data-dismiss='alert' aria-label='close' title='close' style='top: 0px; right: 0px;'><span aria-hidden='true'>&times;</span></button>"
                  + text
                  + "</div>";
    return message;
}

function ErrorMessage(text)
{
    var message = "<div id='errorMessage' class='alert alert-danger alert-dismissible fade in' style='margin-top: 10px; text-align: center; padding: 10px 15px;'>"
                  + "<button type='button' class='close' data-dismiss='alert' aria-label='close' title='close' style='top: 0px; right: 0px;'><span aria-hidden='true'>&times;</span></button>"
                  + text
                  + "</div>";
return message;
}

function UpdateDetailObjects(id_object, objects, fields) {
    debugger;
    for (var i = 0; i < id_object.GetItemCount() ; i++) {
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


function UpdateDepartament(id_employee, employeeDepartament, tempDataKeep)
{
    $.ajax({
        url: "ProductionLot/UpdateDepartament",
        type: "post",
        data: { 
            id_employee: id_employee,
            tempDataKeep : tempDataKeep
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
    return window.open(_url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
}

function showThickBox(_url, data/*, width, height*/) {
    //var widthAux = isNaN(width) ? '100%' : width;
    //var heightAux = isNaN(height) ? '100%' : height;
    //UpdateViewCallback = null;
    //pagesShown = [];
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
                    scrolling: false,
                    padding: 0,
                    //'width': '622px',
                    //'autoScale': false,
                    // width: '',
                    //height: 'auto',
                    //'autoScale': false,
                    //'transitionIn': 'none',
                    //'transitionOut': 'none',
                    //'type': 'html',
                    ////scrolling: true,
                    ////width: widthAux,
                    ////height: heightAux,
                    modal: true
                    ////autoDimensions: true,

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

function showPage(_url, data){
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
            $("#maincontent").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function showForm(_url, data) {
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
            $("#mainform").html(result);
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
          +    "<table style=\"width: 100%;\">"
          +         "<tr>"
          +             "<td style=\"width: 15%;\">"
          +                  "<span class=\"glyphicon glyphicon-warning-sign warning\" style=\"color: #ff8800; font-size: 30px;\"></span>"
          +             "</td>"
          +             "<td style=\"text-align: left; vertical-align: middle;\">"
          +                  text
          +             "</td>"
          +         "</tr>"
          +     "</table>"
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
    debugger;
    var regExp = new RegExp("[^0-9]", "i");

    if (ruc.length != 13) {
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
    if (provCode < 1 || provCode > 22) {
        return { isValid: false, errorText: "Código de provincia emisora errado" };
    }

    if (ruc.substr(10, 13) === "000") {
        return { isValid: false, errorText: "Los tres últimos dígitos no pueden ser 000" };
    }

    var digitoAutoverificador = 0;
    var coeficientes = [];

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
            return { isValid: false, errorText: "Número de RUC incorrecto" };
        }
    }
    else {
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
    if (tipoPersona > 5) {
        return { isValid: false, errorText: "El 3er dígito del ruc debe ser menor que 6" };
    }

    var provCode = parseInt(cedula.substr(0, 2));
    if (provCode < 1 || provCode > 22) {
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
    if (sum >= 10){
        if (sum % 10 != 0){
            digitoVerificadorObtenido = (10 - (sum % 10));
        } else {
            digitoVerificadorObtenido = (sum % 10)
        }
    }else {
        digitoVerificadorObtenido = sum;
    }
    if(digitoVerificadorObtenido != digitoAutoverificador){
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

// MENU

function custom_menu_click(s, e) {

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
        error: function(error) {
            console.log(error);
        },
        beforeSend: function() {
            showLoading();
        },
        success: function (result) {
            hideLoading();
            window.location.href = "/";
        },
        complete: function() {
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

    if (submenues.length === 0)
    {
        html += "<a href=\"javascript:;\" data-url=\"" + controller + "/" + action + "\" class=\"custom-menu-item\">";
    }
    else
    {
        html += "<a href=\"javascript:;\" data-url=\"" + controller + "/" + action + "\" class=\"dropdown-toggle\">";
    }

    html += "<i class=\"menu-icon fa fa-caret-right\"></i>";
    html += " " + subMenu.title + " ";

    if(submenues.length === 0)
    {
        html += "<b class=\"arrow\"></b>";
    }
    else
    {
        html += "<b class=\"arrow fa fa-angle-down\"></b>";
    }

    html += "</a>";

    if (submenues.length > 0)
    {
        html += "<b class=\"arrow\"></b>";
        html += "<ul class=\"submenu\">";

        for (var i = 0; i < submenues.length; i++)
        {
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

    if (menuItem.id_parent === null)
    {
        html += "<li class=\"\">";

        if (submenues.length === 0)
        {
            html += "<a href=\"javascript:;\" data-url=\"" + controller + "/" + action + "\" class=\"custom-menu-item\">";
        }
        else
        {
            html += "<a href=\"javascript:;\" data-url=\"" + controller + "/" + action + "\" class=\"dropdown-toggle\">";
        }


        html += "<i class=\"menu-icon fa fa-list\"></i>";
        html += "<span class=\"menu-text\"> " + menuItem.title + " </span>";

        if (submenues.length === 0)
        {
            html += "<b class=\"arrow\"></b>";
        }
        else
        {
            html += "<b class=\"arrow fa fa-angle-down\"></b>";
        }

        html += "</a>";

        if (submenues.length > 0){
            html += "<b class=\"arrow\"></b>";
            html += "<ul class=\"submenu\">";

            for (var i = 0; i < submenues.length; i++)
            {
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
    for(var i = 0; i < treeMenu.length; i++) {
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

// MAIN

function init() {
    loadMenu();
    $("#logout").click(logout);
    $("#btn_go_notifications").click(GoToNotifications);

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
//SCRIPTS STANDAR
function OnUpdateImagenWhenRequiredField(s, e) {
    debugger;
    var messageErrorControl = "";
    if(s.cpHasTab != undefined){
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

    if (tab.GetVisible() === false) {
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

    event.preventDefault();
}

function GoToNotifications() {
    debugger;
    showPage("Notification/Index");
}

$(function () {
    init();

});


