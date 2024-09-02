// Filter Action Buttons
function OnClickSearchProductionLotProcess() {
    var data = $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/ProductionLotProcessResultsPartial",
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
                $("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function OnClickClearFiltersProductionLotProcess() {
    //PersonRequestingCombo_Init(s, e);
    //DocumentStateCombo_Init(s, e);
    //ProviderCombo_Init(s, e);
    //filterNumber.SetText("");
    //filterReference.SetText("");
    //filterStartEmissionDate.SetText("");
    //filterEndEmissionDate.SetText("");
    //filterStartAuthorizationDate.SetText("");
    //filterEndAuthorizationDate.SetText("");
    //filterAuthorizationNumber.SetText("");
    //filterAccessKey.SetText("");
    //filterItem.ClearTokenCollection();
}

function ButtonAddNewProcess_Process() {
    var data = {
        id: 0
    };

    showPage("ProductionLotProcess/ProductionLotProcessFormEditPartial", data);
}

function ButtonAddNewProcess_AddedValue() {
    var data = {
        id: 0
    };

    showPage("ProductionLotProcess/ProductionLotProcessAddedValueFormEditPartial", data);
}

// Filter ComboBox
function ProductionLotStateCombo_Init() {
    id_ProductionLotState.SetValue(-1);
    id_ProductionLotState.SetText("");
}

function ProductionUnitCombo_Init() {
    id_productionUnit.SetValue(-1);
    id_productionUnit.SetText("");
}

function ProductionProcessCombo_Init() {
    id_productionProcess.SetValue(-1);
    id_productionProcess.SetText("");
}

//function PersonRequestingCombo_Init(s, e) {
//    filterPersonRequesting.SetValue(-1);
//    filterPersonRequesting.SetText("");
//}

function WarehouseCombo_Init() {
    filterWarehouse.SetValue(-1);
    filterWarehouse.SetText("");
}

function WarehouseLocationCombo_Init() {
    filterWarehouseLocation.SetValue(-1);
    filterWarehouseLocation.SetText("");
}

//function PersonReceivingCombo_Init() {
//    filterPersonReceiving.SetValue(-1);
//    filterPersonReceiving.SetText("");
//} 

//function ItemTypeCombo_Init() {
//    filterItemType.SetValue(-1);
//    filterItemType.SetText("");
//}

//function ItemTypeCategoryCombo_Init() {
//    filterItemTypeCategory.SetValue(-1);
//    filterItemTypeCategory.SetText("");
//}


// Results GridView Selection
function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}



function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewBeginCallback(s, e) {

}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotProcess.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotProcess.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionLotProcess.GetSelectedRowCount() > 0 && gvProductionLotProcess.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionLotProcess.GetSelectedRowCount() > 0);
    //}

    //if(btnReprocess !== undefined && btnReprocess !== null) {
    //    btnReprocess.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() > 0);
    //}

    //if (btnAddValue !== undefined && btnAddValue !== null) {
    //    btnAddValue.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() > 0);
    //}

    btnCopy.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() == 1);
    btnApprove.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() > 0);
    btnAutorize.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() > 0);
    btnProtect.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() > 0);
    btnCancel.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() > 0);
    btnRevert.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() > 0);
    btnHistory.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvProductionLotProcess.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvProductionLotProcess.cpFilteredRowCountWithoutPage + gvProductionLotProcess.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvProductionLotProcess.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvProductionLotProcess.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {
    gvProductionLotProcess.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: url,
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvProductionLotProcess.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}


// Results GridView Acction Buttons
//btnNew
function AddNewLot(s, e) {
    //OnClickAddNewProductionLotProcess(s, e);
}

//btnCopy
function CopyLot(s, e) {
    gvProductionLotProcess.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("ProductionLotProcess/ProductionLotProcessCopy", { id: values[0] });
        }
    });
}

//btnApprove
function ApproveLots(s, e) {
    //var c = confirm("¿Desea aprobar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Appr");
    //}

    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotProcess/ApproveLots");
    }, "¿Desea aprobar los lotes seleccionados?");
}

//btnAutorize
function AutorizeLots(s, e) {
    //var c = confirm("¿Desea autorizar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Auth");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotProcess/AutorizeLots");
    }, "¿Desea autorizar los lotes seleccionados?");
}

//btnProtect
function ProtectLots(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotProcess/ProtectLots");
    }, "¿Desea cerrar los lotes seleccionados?");
}

//btnCancel
function CancelLots(s, e) {
    //var c = confirm("¿Desea anular los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Canc");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotProcess/CancelLots");
    }, "¿Desea anular los lotes seleccionados?");
}

//btnRevert
function RevertLots(s, e) {
    //var c = confirm("¿Desea reversar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Rev");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotProcess/RevertLots");
    }, "¿Desea reversar los lotes seleccionados?");
}

//btnHistory
function ShowHistory(s, e) {

}


//------------------------------------------------------------------------------------------------------------
function DetailLiquidation(s, e) {
    var codeReport = "PIDET";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/PrintResume",
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
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function DetailLiquidationExcel(s, e) {
    var codeReport = "PIDET";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/PrintResume",
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
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/DownloadExcelResumeProcessDetails'
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

//--------------------------------------------------------------------------------------------------


//--------------------------------------------------------------------------------------------------
function ResumeLiquidation(s, e) {
    var codeReport = "PIRES";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/PrintResume",
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
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function ResumeLiquidationExcel(s, e) {
    var codeReport = "PIRES";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/PrintResume",
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
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/DownloadExcelResumeProcessDetailsInternal';
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

//--------------------------------------------------------------------------------------------------
function ResumeReproceso(s, e) {
    var codeReport = "PIRRP";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/PrintResume2",
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
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}


//--------------------------------------------------------------------------------------------------
function DetalleReproceso(s, e) {
    var codeReport = "PIDRP";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/PrintResume2",
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
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}



function MovimientosCostos(s, e) {
    var codeReport = "PIICS";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/PrintMovimientoCosto",
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
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        //var url = 'ReportProd/ToExcel?trepd=' + reportTdr;
                        var url = 'ReportProd/DownloadExcelMovementCost';
                        newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function InternalProcessesMatrixExcel(s, e) {
    var codeReport = "MTPROI";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/PrintMatrixProcesosInternos",
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
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/ToExcel?trepd=' + reportTdr;
                        newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function InternalProcessesMatrix(s, e) {
    //var URLactual = jQuery(location).attr('href').split("?")
    //console.log(URLactual[0]);
    //$.ajax({
    //    url: URLactual[0]
   

    var data = /*"codeReport=" + codeReport + "&" +*/ $("#ProductionLotProcessFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotProcess/InternalProcessesMatrix",
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
                try {
                    if (result != undefined) {
                        var url = 'ProductionLotProcess/WriteDataToExcel';
                        //newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
					    newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();

                        //var reportTdr = result.nameQS;
                        //var url = 'ReportProd/ToExcel?trepd=' + reportTdr;
                        //newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        //newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}
//-------------------------------------------------------------------------------------------------




//btnPrint
function Print(s, e) {
    gvProductionLotProcess.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "ProductionLotProcess/ProductionLotProcessReport",
            type: "post",
            data: { id: selectedRows[0] },
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

    });
}

function OnClickUpdateProductionLotProcess(s, e) {

    var data = {
        id: gvProductionLotProcess.GetRowKey(e.visibleIndex)
    };

    showPage("ProductionLotProcess/ProductionLotProcessFormEditPartial", data);

    //var data = {
    //    id: gvProductionLotProcess.GetRowKey(e.visibleIndex)
    //};

    //showPage("ProductionLotProcess/ProductionLotProcessFormEditPartial", data);
}

function ChangeState(trx) {
    //$.ajax({
    //    url: "PurchaseRequest/ChangeStateSelectedDocuments",
    //    type: "post",
    //    data: { ids: selectedRows, trx: trx },
    //    async: true,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //    },
    //    beforeSend: function () {
    //        showLoading();
    //    },
    //    success: function (result) {
    //        console.log(result);
    //    },
    //    complete: function () {
    //        gvPurchaseRequests.UnselectRows();
    //        gvPurchaseRequests.PerformCallback();
    //        hideLoading();
    //    }
    //});
}

// DETAILS VIEW CALLBACKS

function ProductionLotDetail_OnBeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}


function ProductionLotProcessDetailItems_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotProcessDetailProductionLotLiquidations_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotProcessDetailProductionLotTrashs_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

// Init
function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
}

$(function () {
    init();
});
