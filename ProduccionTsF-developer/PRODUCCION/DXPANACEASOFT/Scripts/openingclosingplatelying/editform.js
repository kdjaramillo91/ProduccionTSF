function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

    if (!valid) {
        UpdateTabImage({ isValid: false }, "tabDocument");
    }

    //var codeState = $("#codeState").val();

    //if (codeState == "01") {
    if (gvOpeningClosingPlateLyingEditFormDetail.cpRowsCount === 0 ||
        gvOpeningClosingPlateLyingEditFormDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabOpeningClosingPlateLyingDetails");
        valid = false;
    }
    //}

    if (valid) {
        var id = $("#id_openingClosingPlateLying").val();

        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditOpeningClosingPlateLying").serialize();

        var url = (id === "0") ? "OpeningClosingPlateLying/OpeningClosingPlateLyingsAddNew"
                               : "OpeningClosingPlateLying/OpeningClosingPlateLyingsUpdate";

        showForm(url, data);
    }
}

// DIALOG BUTTONS ACTIONS
function ButtonUpdate_Click(s, e) {

    Update(false);
}

function ButtonUpdateClose_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var id = $("#id_openingClosingPlateLying").val();

        var data = "id=" + id + "&" + $("#formEditOpeningClosingPlateLying").serialize();

        var url = (id === "0") ? "OpeningClosingPlateLying/OpeningClosingPlateLyingsAddNew"
                               : "OpeningClosingPlateLying/OpeningClosingPlateLyingsUpdate";

        if (data != null) {
            $.ajax({
                url: url,
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
                    console.log(result);
                },
                complete: function () {
                    hideLoading();
                    showPage("ProductionLotDailyClose/Index", null);
                }
            });
        }
    }
}

function ButtonCancel_Click(s, e) {
    showPage("OpeningClosingPlateLying/Index", null);
}

// BUTTONS ACTION 
function AddNewDocument(s, e) {

    var data = {
        id: 0
    };

    showPage("OpeningClosingPlateLying/OpeningClosingPlateLyingFormEditPartial", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    //showPage("ProductionLotDailyClose/ProductionLotDailyCloseCopy", { id: $("#id_ProductionLotDailyClose").val() });
}

function ApproveDocument(s, e) {

    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar la tumbada?");

    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_ProductionLotDailyClose").val()
    //    };
    //    showForm("ProductionLotDailyClose/Approve", data);
    //}, "¿Desea aprobar la planificación?");
}

function AutorizeDocument(s, e) {
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_productionLotDailyClose").val()
    //    };
    //    showForm("ProductionLotDailyClose/Autorize", data);
    //}, "¿Desea autorizar la planificación?");
}

function ProtectDocument(s, e) {
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_productionLotDailyClose").val()
    //    };
    //    showForm("ProductionLotDailyClose/Protect", data);
    //}, "¿Desea cerrar la planificación?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_openingClosingPlateLying").val()
        };
        showForm("OpeningClosingPlateLying/Cancel", data);
    }, "¿Desea anular la tumbada?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_openingClosingPlateLying").val()
        };
        showForm("OpeningClosingPlateLying/Revert", data);
    }, "¿Desea reversar la tumbada?");
}

function ShowDocumentHistory(s, e) {
}

function PrintDocument(s, e) {
	showConfirmationDialog(function () {
		Update(true);
	}, "¿Desea aprobar la tumbada?");
}

// TABS CONTROL ACTIONS

//var gv = null;

//function TabControl_ActiveTabChanged(s, e) {
//    var activeTab = tabControl.GetActiveTab();
//    //console.log("Estoy en el TabControl_ActiveTabChanged del Detalle");
//    if (activeTab === null || activeTab === undefined) {
//        gv = null;
//        return;
//    }


//    if (activeTab.name === "tabItemsDetails") {
//        gv = gvProductionLotDailyCloseEditFormItemsDetail;
//    }
//    //console.log("El activeTab.name es tabItemsDetails");
//    //console.log("El gv active es: ");
//    //console.log(gv);
//    //else if (activeTab.name === "tabMaterialsDispatch") {
//    //    gv = gvProductionLotDailyCloseEditFormMaterialsDetail;
//    //} else if (activeTab.name === "tabProductionLotLiquidationsDetails") {
//    //    gv = gvProductionLotDailyCloseEditFormProductionLotLiquidationsDetail;
//    //} else if (activeTab.name === "tabProductionLotTrashsDetails") {
//    //    gv = gvProductionLotDailyCloseEditFormProductionLotTrashsDetail;
//    //} else if (activeTab.name === "tabProductionLotQualityAnalysisDetails") {
//    //    gv = gvProductionLotDailyCloseEditFormProductionLotQualityAnalysissDetail;
//    //}

//    UpdateTitlePanel();
//}

// COMMON DETAILS ACTIONS BUTTONS

//function AddNew(s, e) {
//    if (gv !== null && gv !== undefined) {
//        gv.AddNewRow();
//    }
//}

//function Remove(s, e) {
//}

//function Refresh(s, e) {
//    if (gv !== null && gv !== undefined) {
//        gv.UnselectRows();
//        gv.PerformCallback();
//    }
//}

// DETAILS ACTIONS

function AddNewDetail(s, e) {
    //AddNew(s, e);
}

function RemoveDetail(s, e) {
    //Remove(s, e);
}

function RefreshDetail(s, e) {
    //Refresh(s, e);
    $.ajax({
        url: "OpeningClosingPlateLying/UpdateDetail",
        type: "post",
        data: { openingClosingPlateLying: null, refreshAll : false },
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
            gvOpeningClosingPlateLyingEditFormDetail.UnselectRows();
            gvOpeningClosingPlateLyingEditFormDetail.PerformCallback();
        },
        complete: function () {
            hideLoading();
        }
    });
}

function RefreshDetailAll(s, e) {
    //Refresh(s, e);
    $.ajax({
        url: "OpeningClosingPlateLying/UpdateDetail",
        type: "post",
        data: { openingClosingPlateLying: null, refreshAll: true },
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
            gvOpeningClosingPlateLyingEditFormDetail.UnselectRows();
            gvOpeningClosingPlateLyingEditFormDetail.PerformCallback();
        },
        complete: function () {
            hideLoading();
        }
    });
}
// DISPATCH MATERIALS ACTIONS

//function AddNewDispatchMaterials(s, e) {
//    AddNew(s, e);
//}

//function RemoveDispatchMaterials(s, e) {
//    Remove(s, e);
//}

//function RefreshDispatchMaterials(s, e) {
//    Refresh(s, e);
//}

// LIQUIDATONS ACTIONS

//function AddNewLiquidation(s, e) {
//    AddNew(s, e);
//}

//function RemoveLiquidation(s, e) {
//    Remove(s, e);
//}

//function RefreshLiquidation(s, e) {
//    Refresh(s, e);
//}

// TRASH ACTIONS

//function AddNewTrash(s, e) {
//    AddNew(s, e);
//}

//function RemoveTrash(s, e) {
//    Remove(s, e);
//}

//function RefreshTrash(s, e) {
//    Refresh(s, e);
//}

// TRASH ACTIONS

//function AddNewQualityAnalysis(s, e) {
//    AddNew(s, e);
//}

//function RemoveQualityAnalysis(s, e) {
//    Remove(s, e);
//}

//function RefreshQualityAnalysis(s, e) {
//    Refresh(s, e);
//}

// DETAILS AND DISPATCH MATERIALS SELECTION

//function OnGridViewInitDetail(s, e) {
//    TabControl_ActiveTabChanged(s, e);
//}

//function OnGridViewSelectionChangedDetail(s, e) {
//    //console.log("OnGridViewSelectionChangedDetail s y e:");
//    //console.log(s);
//    //console.log(e);
//    UpdateTitlePanel();

//    //if (gv !== gvProductionLotDailyCloseEditFormProductionLotQualityAnalysissDetail) {
//    gv.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackDetail);
//    //console.log("Sali de OnGridViewSelectionChangedDetail");
//    //} else {
//    /// s.GetSelectedFieldValues("id_qualityAnalysis", GetSelectedFieldValuesCallbackDetail);
//    //}

//}

//function GetSelectedFieldValuesCallbackDetail(values) {
//    //console.log("GetSelectedFieldValuesCallbackDetail values:");
//    //console.log(values);
//    var selectedRows = [];
//    for (var i = 0; i < values.length; i++) {
//        selectedRows.push(values[i]);
//    }
//}

//var customCommand = "";

//function OnGridViewBeginCallbackDetail(s, e) {
//    customCommand = e.command;
//}

//function OnGridViewEndCallbackDetail(s, e) {
//    UpdateTitlePanel();

//    //if (gv !== gvProductionLotDailyCloseEditFormProductionLotQualityAnalysissDetail) {
//    if (gv.GetEditor("id") !== null && gv.GetEditor("id") !== undefined) {
//        gv.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
//    }
//    //}
//    //else {
//    //    if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
//    //        s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
//    //    }
//    //}


//    //if (gv !== gvProductionLotDailyCloseEditFormMaterialsDetail && gv !== gvProductionLotDailyCloseEditFormProductionLotQualityAnalysissDetail) {
//    //    UpdateProductionLotTotals();
//    //}
//}

//function UpdateTitlePanel() {
//    //console.log("Estoy en el UpdateTitlePanel del Detalle");
//    if (gv === null || gv === undefined)
//        return;
//    //console.log("El gv active al inicio del UpdateTitlePanel  del Detalle es: ");
//    //console.log(gv);

//    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

//    var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
//    var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
//    if (hiddenSelectedRowCount > 0)
//        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
//    text += "<br />";

//    var element = (gv === gvProductionLotDailyCloseEditFormItemsDetail) ? "Items" : "";
//    //(gv === gvProductionLotDailyCloseEditFormMaterialsDetail) ? "Materials" :
//    //(gv === gvProductionLotDailyCloseEditFormProductionLotLiquidationsDetail) ? "Liquidations" :
//    //(gv === gvProductionLotDailyCloseEditFormProductionLotTrashsDetail) ? "Trashs" : "QualityAnalysiss";

//    $("#lblInfo" + element).html(text);

//    if ($("#selectAllMode").val() !== "AllPages") {
//        SetElementVisibility("lnkSelectAllRows" + element, gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
//        SetElementVisibility("lnkClearSelection" + element, gv.GetSelectedRowCount() > 0);
//    }

//    btnRemoveDetail.SetEnabled(gv.GetSelectedRowCount() > 0);

//    //console.log("El gv active al final del UpdateTitlePanel del Detalle es: ");
//    //console.log(gv);
//}

//function GetSelectedFilteredRowCount() {
//    return gv.cpFilteredRowCountWithoutPage + gv.GetSelectedKeysOnPage().length;
//}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

//function gvEditClearSelection() {
//    gv.UnselectRows();
//}

//function gvEditSelectAllRows() {
//    gv.SelectRows();
//}

// UPDATE PRODUCTION LOT TOTALS

//function UpdateProductionLotTotals() {
//    $.ajax({
//        url: "ProductionLotDailyClose/ProductionLotTotals",
//        type: "post",
//        data: null,
//        async: true,
//        cache: false,
//        error: function (error) {
//            console.log(error);
//        },
//        beforeSend: function () {
//            //showLoading();
//        },
//        success: function (result) {
//            if (result !== null) {
//                totalQuantityOrdered.SetValue(result.totalQuantityOrdered);
//                totalQuantityRemitted.SetValue(result.totalQuantityRemitted);
//                totalQuantityRecived.SetValue(result.totalQuantityRecived);
//                totalQuantityLiquidation.SetValue(result.totalQuantityLiquidation);
//                totalQuantityTrash.SetValue(result.totalQuantityTrash);
//            }
//        },
//        complete: function () {
//            //hideLoading();
//        }
//    });
//}

//DETALLE DE SOLICITUD COMBOS

//function itemCombo_OnInit(s, e) {
//    //store actual filtering method and override
//    var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
//    s.filterStrategy.FilteringOnClient = function () {
//        //create a new format string for all list box columns. you could skip this bit and just set
//        //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
//        //columns 1 and 3
//        var lb = this.GetListBoxControl();
//        var filterTextFormatStringItems = [];
//        for (var i = 0; i < lb.columnFieldNames.length; i++) {
//            filterTextFormatStringItems.push('{' + i + '}');
//        }
//        var filterTextFormatString = filterTextFormatStringItems.join(' ');

//        //store actual format string and override with one for all columns
//        var actualTextFormatString = lb.textFormatString;
//        lb.textFormatString = filterTextFormatString;

//        //call actual filtering method which will now work on our temporary format string
//        actualFilteringOnClient.apply(this);

//        //restore original format string
//        lb.textFormatString = actualTextFormatString;
//    };
//}

// UPDATE VIEW

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
    var id = parseInt($("#id_openingClosingPlateLying").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    //btnCopy.SetEnabled(id !== 0);
    btnCopy.SetVisible(false);
    btnAutorize.SetVisible(false);
    btnProtect.SetVisible(false);
    btnPrint.SetVisible(false);

	// STATES BUTTONS
    $.ajax({
        url: "OpeningClosingPlateLying/Actions",
        type: "post",
        data: { id: id },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            btnApprove.SetEnabled(result.btnApprove);
            //btnAutorize.SetEnabled(result.btnAutorize);
            //btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            //hideLoading();
        }
    });

    // HISTORY BUTTON
	btnHistory.SetEnabled(id !== 0);
	// 
    // PRINT BUTTON
    btnPrint.SetEnabled(id !== 0);
    var codeDocumentState = $("#codeDocumentState").val();

    if (codeDocumentState == "01") {
        //UpdateViewDetailButton
        btnNewDetail.SetVisible(false);
        btnRemoveDetail.SetVisible(false);
    }
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "OpeningClosingPlateLying/InitializePagination",
        type: "post",
        data: { id_openingClosingPlateLying: $("#id_openingClosingPlateLying").val() },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            $("#pagination").attr("data-max-page", result.maximunPages);
            current_page = result.currentPage;
        },
        complete: function () {
        }
    });
    $('.pagination').current_page = current_page;
}

function init() {
    UpdatePagination();

    AutoCloseAlert();
}

$(function () {

    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});