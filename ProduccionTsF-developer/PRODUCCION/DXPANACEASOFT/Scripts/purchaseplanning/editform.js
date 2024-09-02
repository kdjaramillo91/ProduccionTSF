
// DIALOG BUTTONS ACTIONS
function ButtonUpdate_Click(s, e) {

    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);
    
    if(!valid) {
        UpdateTabImage({ isValid: false }, "tabDocument");
    }

    if (gvPurchasePlanningEditFormItemsDetail.cpRowsCount === 0 ||
        gvPurchasePlanningEditFormItemsDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabItemsDetails");
        valid = false;
    }

    //if (gvPurchasePlanningEditFormMaterialsDetail.cpRowsCount === 0 ||
    //    gvPurchasePlanningEditFormMaterialsDetail.IsEditing()) {
    //    UpdateTabImage({ isValid: false }, "tabMaterialsDispatch");
    //    valid = false;
    //}

    //if (gvPurchasePlanningEditFormProductionLotLiquidationsDetail.cpRowsCount === 0 ||
    //    gvPurchasePlanningEditFormProductionLotLiquidationsDetail.IsEditing()) {
    //    UpdateTabImage({ isValid: false }, "tabProductionLotLiquidationsDetails");
    //    valid = false;
    //}

    if (valid) {
        var id = $("#id_purchasePlanning").val();

        var data = "id=" + id + "&" + $("#formEditPurchasePlanning").serialize();

        var url = (id === "0") ? "PurchasePlanning/PurchasePlanningsAddNew"
                               : "PurchasePlanning/PurchasePlanningsUpdate";

        showForm(url, data);
    }
}

function ButtonUpdateClose_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var id = $("#id_purchasePlanning").val();

        var data = "id=" + id + "&" + $("#formEditPurchasePlanning").serialize();

        var url = (id === "0") ? "PurchasePlanning/PurchasePlanningsAddNew"
                               : "PurchasePlanning/PurchasePlanningsUpdate";

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
                    showPage("PurchasePlanning/Index", null);
                }
            });
        }
    }
}

function ButtonCancel_Click(s, e) {
    showPage("PurchasePlanning/Index", null);
}

// BUTTONS ACTION 
function AddNewDocument(s, e) {

    var data = {
        id: 0
    };

    showPage("PurchasePlanning/PurchasePlanningFormEditPartial", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    showPage("PurchasePlanning/PurchasePlanningCopy", { id: $("#id_purchasePlanning").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchasePlanning").val()
        };
        showForm("PurchasePlanning/Approve", data);
    }, "¿Desea aprobar la planificación?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchasePlanning").val()
        };
        showForm("PurchasePlanning/Autorize", data);
    }, "¿Desea autorizar la planificación?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchasePlanning").val()
        };
        showForm("PurchasePlanning/Protect", data);
    }, "¿Desea cerrar la planificación?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchasePlanning").val()
        };
        showForm("PurchasePlanning/Cancel", data);
    }, "¿Desea anular la planificación?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchasePlanning").val()
        };
        showForm("PurchasePlanning/Revert", data);
    }, "¿Desea reversar la planificación?");
}

function ShowDocumentHistory(s, e) {
}

function PrintDocument(s, e) {

    $.ajax({
        url: "PurchasePlanning/PurchasePlanningReport",
        type: "post",
        data: { id: $("#id_purchasePlanning").val() },
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

// TABS CONTROL ACTIONS

var gv = null;

function TabControl_ActiveTabChanged(s, e) {
    var activeTab = tabControl.GetActiveTab();
    //console.log("Estoy en el TabControl_ActiveTabChanged del Detalle");
    if (activeTab === null || activeTab === undefined) {
        gv = null;
        return;
    }
    

    if (activeTab.name === "tabItemsDetails") {
        gv = gvPurchasePlanningEditFormItemsDetail;
    }
    //console.log("El activeTab.name es tabItemsDetails");
    //console.log("El gv active es: ");
    //console.log(gv);
    //else if (activeTab.name === "tabMaterialsDispatch") {
    //    gv = gvPurchasePlanningEditFormMaterialsDetail;
    //} else if (activeTab.name === "tabProductionLotLiquidationsDetails") {
    //    gv = gvPurchasePlanningEditFormProductionLotLiquidationsDetail;
    //} else if (activeTab.name === "tabProductionLotTrashsDetails") {
    //    gv = gvPurchasePlanningEditFormProductionLotTrashsDetail;
    //} else if (activeTab.name === "tabProductionLotQualityAnalysisDetails") {
    //    gv = gvPurchasePlanningEditFormProductionLotQualityAnalysissDetail;
    //}

    UpdateTitlePanel();
}

// COMMON DETAILS ACTIONS BUTTONS

function AddNew(s, e) {
    if (gv !== null && gv !== undefined) {
        gv.AddNewRow();
    }
}

function Remove(s, e) {
}

function Refresh(s, e) {
    if (gv !== null && gv !== undefined) {
        gv.UnselectRows();
        gv.PerformCallback();
    }
}

// DETAILS ACTIONS

function AddNewDetail(s, e) {
    AddNew(s, e);
}

function RemoveDetail(s, e) {
    Remove(s, e);
}

function RefreshDetail(s, e) {
    Refresh(s, e);
}

// DISPATCH MATERIALS ACTIONS

function AddNewDispatchMaterials(s, e) {
    AddNew(s, e);
}

function RemoveDispatchMaterials(s, e) {
    Remove(s, e);
}

function RefreshDispatchMaterials(s, e) {
    Refresh(s, e);
}

// LIQUIDATONS ACTIONS

function AddNewLiquidation(s, e) {
    AddNew(s, e);
}

function RemoveLiquidation(s, e) {
    Remove(s, e);
}

function RefreshLiquidation(s, e) {
    Refresh(s, e);
}

// TRASH ACTIONS

function AddNewTrash(s, e) {
    AddNew(s, e);
}

function RemoveTrash(s, e) {
    Remove(s, e);
}

function RefreshTrash(s, e) {
    Refresh(s, e);
}

// TRASH ACTIONS

function AddNewQualityAnalysis(s, e) {
    AddNew(s, e);
}

function RemoveQualityAnalysis(s, e) {
    Remove(s, e);
}

function RefreshQualityAnalysis(s, e) {
    Refresh(s, e);
}

// DETAILS AND DISPATCH MATERIALS SELECTION

function OnGridViewInitDetail(s, e) {
    TabControl_ActiveTabChanged(s, e);
}



function OnGridViewSelectionChangedDetail(s, e) {
    //console.log("OnGridViewSelectionChangedDetail s y e:");
    //console.log(s);
    //console.log(e);
    UpdateTitlePanel();

    //if (gv !== gvPurchasePlanningEditFormProductionLotQualityAnalysissDetail) {
    gv.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackDetail);
    //console.log("Sali de OnGridViewSelectionChangedDetail");
    //} else {
    /// s.GetSelectedFieldValues("id_qualityAnalysis", GetSelectedFieldValuesCallbackDetail);
    //}
    
}

function GetSelectedFieldValuesCallbackDetail(values) {
    //console.log("GetSelectedFieldValuesCallbackDetail values:");
    //console.log(values);
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewBeginCallbackDetail(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallbackDetail(s, e) {
    UpdateTitlePanel();

    //if (gv !== gvPurchasePlanningEditFormProductionLotQualityAnalysissDetail) {
        if (gv.GetEditor("id") !== null && gv.GetEditor("id") !== undefined) {
            gv.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
        }
    //}
    //else {
    //    if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
    //        s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
    //    }
    //}


    //if (gv !== gvPurchasePlanningEditFormMaterialsDetail && gv !== gvPurchasePlanningEditFormProductionLotQualityAnalysissDetail) {
    //    UpdateProductionLotTotals();
    //}
}

function UpdateTitlePanel() {
    //console.log("Estoy en el UpdateTitlePanel del Detalle");
    if (gv === null || gv === undefined)
        return;
    //console.log("El gv active al inicio del UpdateTitlePanel  del Detalle es: ");
    //console.log(gv);

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    var element = (gv === gvPurchasePlanningEditFormItemsDetail) ? "Items" : "";
                  //(gv === gvPurchasePlanningEditFormMaterialsDetail) ? "Materials" :
                  //(gv === gvPurchasePlanningEditFormProductionLotLiquidationsDetail) ? "Liquidations" :
                  //(gv === gvPurchasePlanningEditFormProductionLotTrashsDetail) ? "Trashs" : "QualityAnalysiss";

    $("#lblInfo" + element).html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRows" + element, gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelection" + element, gv.GetSelectedRowCount() > 0);
    }

    btnRemoveDetail.SetEnabled(gv.GetSelectedRowCount() > 0);

    //console.log("El gv active al final del UpdateTitlePanel del Detalle es: ");
    //console.log(gv);
}

function GetSelectedFilteredRowCount() {
    return gv.cpFilteredRowCountWithoutPage + gv.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvEditClearSelection() {
    gv.UnselectRows();
}

function gvEditSelectAllRows() {
    gv.SelectRows();
}

// UPDATE PRODUCTION LOT TOTALS

function UpdateProductionLotTotals() {
    $.ajax({
        url: "PurchasePlanning/ProductionLotTotals",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                totalQuantityOrdered.SetValue(result.totalQuantityOrdered);
                totalQuantityRemitted.SetValue(result.totalQuantityRemitted);
                totalQuantityRecived.SetValue(result.totalQuantityRecived);
                totalQuantityLiquidation.SetValue(result.totalQuantityLiquidation);
                totalQuantityTrash.SetValue(result.totalQuantityTrash);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

//DETALLE DE SOLICITUD COMBOS

function itemCombo_OnInit(s, e) {
    //store actual filtering method and override
    var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
    s.filterStrategy.FilteringOnClient = function () {
        //create a new format string for all list box columns. you could skip this bit and just set
        //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
        //columns 1 and 3
        var lb = this.GetListBoxControl();
        var filterTextFormatStringItems = [];
        for (var i = 0; i < lb.columnFieldNames.length; i++) {
            filterTextFormatStringItems.push('{' + i + '}');
        }
        var filterTextFormatString = filterTextFormatStringItems.join(' ');

        //store actual format string and override with one for all columns
        var actualTextFormatString = lb.textFormatString;
        lb.textFormatString = filterTextFormatString;

        //call actual filtering method which will now work on our temporary format string
        actualFilteringOnClient.apply(this);

        //restore original format string
        lb.textFormatString = actualTextFormatString;
    };
}

// UPDATE VIEW

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
    var id = parseInt($("#id_purchasePlanning").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "PurchasePlanning/Actions",
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
            btnAutorize.SetEnabled(result.btnAutorize);
            btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            //hideLoading();
        }
    });

    // HISTORY BUTTON
    btnHistory.SetEnabled(id !== 0);

    // PRINT BUTTON
    btnPrint.SetEnabled(id !== 0);
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "PurchasePlanning/InitializePagination",
        type: "post",
        data: { id_purchasePlanning: $("#id_purchasePlanning").val() },
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