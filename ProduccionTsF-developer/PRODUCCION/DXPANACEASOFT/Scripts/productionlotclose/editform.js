
// DIALOG BUTTONS ACTIONS
function ButtonUpdate_Click(s, e) {

    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);
    
    if(!valid) {
        UpdateTabImage({ isValid: false }, "tabDocument");
    }

    if (gvProductionLotReceptionEditFormItemsDetail.cpRowsCount === 0 ||
        gvProductionLotReceptionEditFormItemsDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabItemsDetails");
        valid = false;
    }

    if (gvProductionLotReceptionEditFormMaterialsDetail.cpRowsCount === 0 ||
        gvProductionLotReceptionEditFormMaterialsDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabMaterialsDispatch");
        valid = false;
    }

    if (gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.cpRowsCount === 0 ||
        gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabProductionLotLiquidationsDetails");
        valid = false;
    }

    if (valid) {
        var id = $("#id_productionLot").val();

        var data = "id=" + id + "&" + $("#formEditProductionLotReception").serialize();

        var url = (id === "0") ? "ProductionLotReception/ProductionLotReceptionsAddNew"
                               : "ProductionLotReception/ProductionLotReceptionsUpdate";

        showForm(url, data);
    }
}

function ButtonUpdateClose_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var id = $("#id_productionLot").val();

        var data = "id=" + id + "&" + $("#formEditProductionLotReception").serialize();

        var url = (id === "0") ? "ProductionLotReception/ProductionLotReceptionsAddNew"
                               : "ProductionLotReception/ProductionLotReceptionsUpdate";

        if (data !== null) {
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
                    showPage("ProductionLotReception/Index", null);
                }
            });
        }
    }
}

function ButtonCancel_Click(s, e) {
    showPage("ProductionLotReception/Index", null);
}

// BUTTONS ACTION 
function AddNewLot(s, e) {

    var data = {
        id: 0,
        loteManual: false
    };

    showForm("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
}

function SaveLot(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseLot(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyLot(s, e) {
    showPage("ProductionLotReception/ProductionLotReceptionCopy", { id: $("#id_productionLot").val() });
}

function ApproveLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotReception/Approve", data);
    }, "¿Desea aprobar el lote?");
}

function AutorizeLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotReception/Autorize", data);
    }, "¿Desea autorizar el lote?");
}

function ProtectLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotReception/Protect", data);
    }, "¿Desea cerrar el lote?");
}

function CancelLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotReception/Cancel", data);
    }, "¿Desea anular el lote?");
}

function RevertLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotReception/Revert", data);
    }, "¿Desea reversar el lote?");
}

function ShowLotHistory(s, e) {
}

function PrintLot(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionReport",
        type: "post",
        data: { id: $("#id_productionLot").val() },
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

    if (activeTab === null || activeTab === undefined) {
        gv = null;
        return;
    }

    if (activeTab.name === "tabItemsDetails") {
        gv = gvProductionLotReceptionEditFormItemsDetail;
    } else if (activeTab.name === "tabMaterialsDispatch") {
        gv = gvProductionLotReceptionEditFormMaterialsDetail;
    } else if (activeTab.name === "tabProductionLotLiquidationsDetails") {
        gv = gvProductionLotReceptionEditFormProductionLotLiquidationsDetail;
    } else if (activeTab.name === "tabProductionLotTrashsDetails") {
        gv = gvProductionLotReceptionEditFormProductionLotTrashsDetail;
    } else if (activeTab.name === "tabProductionLotQualityAnalysisDetails") {
        gv = gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail;
    }

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

function OnGridViewInit(s, e) {
    TabControl_ActiveTabChanged(s, e);
}



function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    if (gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
        s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallback);
    } else {
        s.GetSelectedFieldValues("id_qualityAnalysis", GetSelectedFieldValuesCallback);
    }
    
}

function GetSelectedFieldValuesCallback(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();

    if (gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
        if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
            s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
        }
    } else {
        if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
            s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
        }
    }


    if (gv !== gvProductionLotReceptionEditFormMaterialsDetail && gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
        UpdateProductionLotTotals();
    }
}

function UpdateTitlePanel() {

    if (gv === null || gv === undefined)
        return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    var element = (gv === gvProductionLotReceptionEditFormItemsDetail) ? "Items" : 
                  (gv === gvProductionLotReceptionEditFormMaterialsDetail) ? "Materials" :
                  (gv === gvProductionLotReceptionEditFormProductionLotLiquidationsDetail) ? "Liquidations" :
                  (gv === gvProductionLotReceptionEditFormProductionLotTrashsDetail) ? "Trashs" : "QualityAnalysiss";

    $("#lblInfo" + element).html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRows" + element, gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelection" + element, gv.GetSelectedRowCount() > 0);
    }

    btnRemoveDetail.SetEnabled(gv.GetSelectedRowCount() > 0);
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
        url: "ProductionLotReception/ProductionLotTotals",
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
    var id = parseInt($("#id_productionLot").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "ProductionLotReception/Actions",
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
        url: "ProductionLotReception/InitializePagination",
        type: "post",
        data: { id_productionLot: $("#id_productionLot").val() },
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