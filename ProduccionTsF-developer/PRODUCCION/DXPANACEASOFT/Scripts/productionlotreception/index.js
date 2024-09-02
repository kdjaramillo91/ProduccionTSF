//PRINT REPORT OPTIONS
function PRReception(s, e) {
    var codeReport = "RRCP1";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRReception",
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

function PRPoundsLiquidation(s, e) {
    var codeReport = "RLLQ1";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRPoundsLiquidation",
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
                // 
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



//-------------------------

function PRMargenporTallas(s, e) {
    var codeReport = "RTCOM";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();
    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRMargenporTallas",
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
function PRMargenporTallasExcel(s, e) {
    var codeReport = "RTCOM";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();
    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRMargenporTallas",
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
                        var url = 'ReportProd/DownloadExcelSizeBuy';
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

//---------------------------------//////


//-------------------------PRResumenCompraPeriodo

function PRResumenProveedorCompras(s, e) {
    var codeReport = "RPCOM";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRResumenProveedorCompras",
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
function PRResumenProveedorComprasExcel(s, e) {
    var codeReport = "RPCOM";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRResumenProveedorCompras",
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
                        var url = 'ReportProd/DownloadExcelSupplierLiquidationResumen';
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

//---------------------------------//////

//-------------------------

function PRResumenCompraPeriodo(s, e) {
    var codeReport = "RCOMP";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRResumenCompraPeriodo",
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


//---------------------------------//////

//-------------------------

function PRResumenCompraPeriodoG(s, e) {
    var codeReport = "RCOMPG";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRResumenCompraPeriodoG",
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

function PRShrimpEntranceStatus(s, e) {
    var codeReport = "RICS1";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRShrimpIncome",
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
                // 
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

function PRPendingShrimp(s, e) {
    var codeReport = "LRPPL";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRShrimpIncome",
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
                // 
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

function PRVitacora() {
    var codeReport = "RMPV";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PrintReportVitacora",
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
                // 
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


function PRLiquidacionSequencial() {
    var codeReport = "RMRL";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRLiquidacionSequencial",
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

function PRLiquidacionValorizadaLiquid() {
    var codeReport = "RMPP";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRLiquidacionValorizadaLiquid",
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

function PRLiquidacionValorizadaLiquidPAproved() {
    var codeReport = "RMPPA";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRLiquidacionValorizadaLiquidPAproved",
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
                // 
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

function PRLiquidacionValorizadaLiquidProv() {
    var codeReport = "RPVA";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRLiquidacionValorizadaLiquidProv",
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
                // 
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

function PRLiquidacionValorizadaLiquidProvPAproved() {
    var codeReport = "RPVPA";
    var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/PRLiquidacionValorizadaLiquidProvPAproved",
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
                // 
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


// FILTERS FORM ACTIONS
function AddNewItemFromPurchaseOrder(s, e) {
    $.ajax({
        url: "ProductionLotReception/PurchaseOrderDetailsResults",
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
            $("#btnCollapse").click();
            $("#results").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function AddNewItemFromRemisionGuide(s, e) {
    $.ajax({
        url: "ProductionLotReception/RemissionGuideResults",
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
            $("#btnCollapse").click();
            $("#results").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

// GRIDVIEW PURCHASE ORDER RESULTS SELECTION

function PurchaseOrderDetailsOnGridViewInit(s, e) {
    //btnGenerateLot.SetEnabled(false);
    PurchaseOrderDetailsUpdateTitlePanel();
}

function PurchaseOrderDetailsOnGridViewSelectionChanged(s, e) {
    PurchaseOrderDetailsUpdateTitlePanel();
    //PurchaseOrderDetailsGetEnabledBtnGenerateLot();
}

function PurchaseOrderDetailsOnGridViewEndCallback() {
    PurchaseOrderDetailsUpdateTitlePanel();
}

function PurchaseOrderDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = PurchaseOrderDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrderDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrderDetails.GetSelectedRowCount() - PurchaseOrderDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvPurchaseOrderDetails.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrderDetails.GetSelectedRowCount() > 0 && gvPurchaseOrderDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
    //}

    //btnGenerateLot.SetEnabled(gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
    //btnGenerateLot.SetEnabled(gvPurchaseOrderDetails.cpEnabledBtnGenerateLot);
}

function PurchaseOrderDetailsGetEnabledBtnGenerateLot() {

    gvPurchaseOrderDetails.GetSelectedFieldValues("id_provider;id_buyer;withPrice", function (values) {
        var id_providerAux = 0;
        var id_buyerAux = 0;
        var withPriceAux = null;
        var enabledbtnGenerateLot = false;
        for (var i = 0; i < values.length; i++) {
            if (id_providerAux == 0) {
                id_providerAux = values[i][0];
                id_buyerAux = values[i][1];
                withPriceAux = values[i][2];
                enabledbtnGenerateLot = true;
            } else {
                if (id_providerAux != values[i][0] || id_buyerAux != values[i][1] || withPriceAux != values[i][2]) {
                    enabledbtnGenerateLot = false;
                    break;
                }
            }
        }
        btnGenerateLot.SetEnabled(enabledbtnGenerateLot);
    });
}

function PurchaseOrderDetailsGetSelectedFilteredRowCount() {
    return gvPurchaseOrderDetails.cpFilteredRowCountWithoutPage + gvPurchaseOrderDetails.GetSelectedKeysOnPage().length;
}

function PurchaseOrderDetailsSelectAllRow() {
    gvPurchaseOrderDetails.SelectRows();
}

function PurchaseOrderDetailsClearSelection() {
    gvPurchaseOrderDetails.UnselectRows();
}


// REMISSION GUIDES RESULT GRIDVIEW SELECTION

function RemissionGuideOnGridViewInit(s, e) {
    RemissionGuideUpdateTitlePanel();
}

var RemissionGuideselectedRows = [];

function RemissionGuideOnGridViewSelectionChanged(s, e) {
    RemissionGuideUpdateTitlePanel();
    //  s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function RemissionGuideGetSelectedFieldValuesCallback(values) {
    RemissionGuideselectedRows = [];
    for (var i = 0; i < values.length; i++) {
        RemissionGuideselectedRows.push(values[i]);
    }
}

function RemissionGuideOnGridViewEndCallback() {
    RemissionGuideUpdateTitlePanel();
}

function RemissionGuideUpdateTitlePanel() {
    var selectedFilteredRowCount = RemissionGuideGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuide.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuide.GetSelectedRowCount() - RemissionGuideGetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvRemisssionGuide.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuide.GetSelectedRowCount() > 0 && gvRemisssionGuide.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuide.GetSelectedRowCount() > 0);
    //}





}

function RemissionGuideGetSelectedFilteredRowCount() {
    return gvRemisssionGuide.cpFilteredRowCountWithoutPage + gvRemisssionGuide.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function RemissionGuidegvResultsClearSelection() {
    gvRemisssionGuide.UnselectRows();
}

function RemissionGuidegvResultsSelectAllRows() {
    gvRemisssionGuide.SelectRows();
}

// GRIDVIEW PURCHASE ORDER RESULTS ACTIONS

function GenerateLot(s, e) {

    gridMessageErrorPurchaseOrdersAndRemissionGuides.SetText("");
    $("#GridMessageErrorPurchaseOrdersAndRemissionGuides").hide();
    showLoading();

    gvPurchaseOrderDetails.GetSelectedFieldValues("id_provider;id_buyer;withPrice;id", function (values) {
        var selectedRows = [];
        var id_providerAux = 0;
        var id_buyerAux = 0;
        var withPriceAux = null;
        var enabledbtnGenerateLot = false;
        var errorMessage = "";
        for (var i = 0; i < values.length; i++) {
            if (id_providerAux == 0) {
                id_providerAux = values[i][0];
                id_buyerAux = values[i][1];
                withPriceAux = values[i][2];
                enabledbtnGenerateLot = true;
            } else {
                if (id_providerAux != values[i][0]) {
                    errorMessage = "Los Proveedores deben ser iguales para cada detalle, por favor verifíquelo";
                    enabledbtnGenerateLot = false;
                    break;
                } else {
                    if (id_buyerAux != values[i][1]) {
                        errorMessage = "Los Compradores deben ser iguales para cada detalle, por favor verifíquelo";
                        enabledbtnGenerateLot = false;
                        break;
                    } else {
                        if (withPriceAux != values[i][2]) {
                            errorMessage = "El origen del precio(Desde Lista de Precio o Fijado en la Orden) con que se gestiona el detalle de la orden de compra debe ser iguales para cada detalle, por favor verifíquelo";
                            enabledbtnGenerateLot = false;
                            break;
                        }
                    }
                }
            }
            selectedRows.push(values[i][3]);
        }
        //btnGenerateLot.SetEnabled(enabledbtnGenerateLot);
        if (enabledbtnGenerateLot) {
            var data = {
                id: 0,
                loteManual: false,
                ids: selectedRows
            };

            showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
        } else {
            var msgErrorAux = ErrorMessage(errorMessage);
            gridMessageErrorPurchaseOrdersAndRemissionGuides.SetText(msgErrorAux);
            $("#GridMessageErrorPurchaseOrdersAndRemissionGuides").show();
            hideLoading();
        }
    });

    //gvPurchaseOrderDetails.GetSelectedFieldValues("id", function (values) {

    //    var selectedRows = [];

    //    for (var i = 0; i < values.length; i++) {
    //        selectedRows.push(values[i]);
    //    }
    //console.log("selectedRows: ");
    //console.log(selectedRows);


    //$.ajax({
    //    url: "ProductionLotReception/GetEnabledBtnGenerateLot",
    //    type: "post",
    //    data: { ids: selectedRows },
    //    async: true,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //    },
    //    beforeSend: function () {
    //        //showLoading();
    //    },
    //    success: function (result) {
    //        //resultFunction = result.enabledBtnGenerateLot;
    //        btnGenerateLot.SetEnabled(result.enabledBtnGenerateLot);
    //        if (result.enabledBtnGenerateLot == true) {
    //            var data = {
    //                id: 0,
    //                ids_purchaseOrdersDetails: selectedRows
    //            };

    //            showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);

    //        } else {
    //            GetDialogMessage("No se puede generar un Lote con detalle de Diferente Proveedor");
    //        }

    //    },
    //    complete: function () {
    //        //hideLoading();
    //        //gvProductionLotReceptions.PerformCallback();
    //        // gvPurchaseOrders.UnselectRows();
    //    }
    //});

    //});
}

// GRIDVIEW GUIDE REMISION ORDER RESULTS ACTIONS

function GenerateLotGuiaRemision(s, e) {
    // 
    gridMessageErrorPurchaseOrdersAndRemissionGuides.SetText("");
    $("#GridMessageErrorPurchaseOrdersAndRemissionGuides").hide();
    showLoading();

    gvRemisssionGuide.GetSelectedFieldValues("id;id_certification", function (values) {

        var selectedRows = [];
        var id_providerAux = 0;
        var enabledbtnGenerateLot = false;
        var errorMessage = "";

        var certificationAux = null;
        for (var i = 0; i < values.length; i++) {
            if (i === 0) {
                certificationAux = values[i][1];
            } else {
                if (certificationAux !== values[i][1]) {
                    hideLoading();
                    var aError = "Los detalles seleccionado deben de tener igual Certificado en su Guía correspondiente";
                    gridMessageErrorPurchaseOrdersAndRemissionGuides.SetText(ErrorMessage(aError));
                    $("#GridMessageErrorPurchaseOrdersAndRemissionGuides").show();
                    return;
                }
            }
            selectedRows.push(values[i][0]);
        }
        var data = {
            ids: selectedRows,
            id: 0
        };

        $.ajax({
            url: "ProductionLotReception/ValidateSelectedRowsRemissionGuide",
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
                // 
                if (result.Message == "OK") {
                    showPage("ProductionLotReception/ProductionLotReceptionFormEditPartialGuiadRemision", data);
                } else {
                    gridMessageErrorPurchaseOrdersAndRemissionGuides.SetText(ErrorMessage(result.Message));
                    $("#GridMessageErrorPurchaseOrdersAndRemissionGuides").show();
                    hideLoading();
                }
            },
            complete: function () {
            }
        });
    });
}

// Filter Action Buttons
function OnClickSearchProductionLotReception() {
    var data = "liquidationNumber=" + liqNumber.GetText() + "&" + $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/ProductionLotReceptionResultsPartial",
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

function OnClickClearFiltersProductionLotReception(s, e) {
    /////OJO
    ProductionLotStateCombo_Init();
    number.SetText("");
    internalNumber.SetText("");
    reference.SetText("");
    ProductionUnitCombo_Init();

    startReceptionDate.SetDate(null);
    endReceptionDate.SetDate(null);

    PersonReceivingCombo_Init();

    id_provider.SetValue(-1);
    id_provider.SetText("");

    id_buyer.SetValue(-1);
    id_buyer.SetText("");

    WarehouseCombo_Init();
    WarehouseLocationCombo_Init();
    items.ClearTokenCollection();
}

function ButtonManualAddNewProductionLotReception_Click() {
    var data = {
        id: 0,
        loteManual: false
    };
    showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
}

function ButtonManualAddNewProductionLotReceptionManual_Click() {
    var data = {
        id: 0,
        loteManual: true
    };
    showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
}

function OnClickAddNewProductionLotReception(trx) {


    if (trx === "M")
        showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
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

function WarehouseCombo_Init() {
    filterWarehouse.SetValue(-1);
    filterWarehouse.SetText("");
}

function WarehouseLocationCombo_Init() {
    filterWarehouseLocation.SetValue(-1);
    filterWarehouseLocation.SetText("");
}

function PersonReceivingCombo_Init() {
    id_personReceiving.SetValue(-1);
    id_personReceiving.SetText("");
}

// Results GridView Selection
function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    s.GetSelectedFieldValues("idProductionLot", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotReceptions.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotReceptions.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionLotReceptions.GetSelectedRowCount() > 0 && gvProductionLotReceptions.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionLotReceptions.GetSelectedRowCount() > 0);
    //}

    btnCopy.SetEnabled(false);
    btnApprove.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() > 0);
    btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    btnHistory.SetEnabled(false);
    //btnPrint.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvProductionLotReceptions.cpFilteredRowCountWithoutPage + gvProductionLotReceptions.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvProductionLotReceptions.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvProductionLotReceptions.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {
    gvProductionLotReceptions.GetSelectedFieldValues("id", function (values) {

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
                gvProductionLotReceptions.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}

//btnNew
function AddNewLot(s, e) {
    OnClickAddNewProductionLotReception(s, e);
}

//btnCopy
function CopyLot(s, e) {
    gvProductionLotReceptions.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("ProductionLotReception/ProductionLotReceptionCopy", { id: values[0] });
        }
    });
}

//btnApprove
function ApproveLots(s, e) {
    showConfirmationDialog(function () {
        // 
        GridMessageProductionLots.SetText("");
        $("#GridMessageProductionLots").hide();
        gvProductionLotReceptions.GetSelectedFieldValues("idProductionLot", function (values) {
            // 
            var selectedRows = [];
            var id_providerAux = 0;
            var enabledbtnGenerateLot = false;
            var errorMessage = "";

            for (var i = 0; i < values.length; i++) {
                selectedRows.push(values[i]);
            }
            var data = {
                ids: selectedRows,
                id: 0
            };

            $.ajax({
                url: "ProductionLotReception/ApproveProductionLotsPendingsMassive",
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
                    // 
                    GridMessageProductionLots.SetText(result.message);
                    $("#GridMessageProductionLots").show();
                    gvProductionLotReceptions.PerformCallback();
                    hideLoading();

                },
                complete: function () {
                    hideLoading();
                }
            });
        });
    }, "¿Desea aprobar los lotes seleccionados?");

}

function ApproveProductionLotsPending() {
    // 
    GridMessageProductionLots.SetText("");
    $("#GridMessageProductionLots").hide();
    gvProductionLotReceptions.GetSelectedFieldValues("idProductionLot", function (values) {
        var selectedRows = [];
        var id_providerAux = 0;
        var enabledbtnGenerateLot = false;
        var errorMessage = "";

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        var data = {
            ids: selectedRows,
            id: 0
        };

        $.ajax({
            url: "ProductionLotReception/ApproveProductionLotsPendingsMassive",
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
                // 
                GridMessageProductionLots.SetText(result.message);
                $("#GridMessageProductionLots").show();
                gvProductionLotReceptions.PerformCallback();
                hideLoading();

            },
            complete: function () {
                hideLoading();
            }
        });
    });
}
//btnAutorize
function AutorizeLots(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/AutorizeLots");
    }, "¿Desea autorizar los lotes seleccionados?");
}

//btnProtect
function ProtectLots(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/ProtectLots");
    }, "¿Desea cerrar los lotes seleccionados?");
}

//btnCancel
function CancelLots(s, e) {
    //var c = confirm("¿Desea anular los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Canc");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/CancelLots");
    }, "¿Desea anular los lotes seleccionados?");
}

//btnRevert
function RevertLots(s, e) {
    //var c = confirm("¿Desea reversar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Rev");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/RevertLots");
    }, "¿Desea reversar los lotes seleccionados?");
}

//btnHistory
function ShowHistory(s, e) {

}

//btnPrint
function Print(s, e) {
    gvProductionLotReceptions.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "ProductionLotReception/ProductionLotReceptionReport",
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

function OnClickUpdateProductionLotReception(s, e) {

    var data = {
        id: gvProductionLotReceptions.GetRowKey(e.visibleIndex),
        loteManual: false
    };

    showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
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
    if (s.cpIdProductionLot == null || s.cpIdProductionLot == undefined) {
        e.customArgs["id_productionLot"] = $("#id_productionLot").val();
    }
}

function ProductionLotReceptionDetail_OnBeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailItems_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailDispatchMaterials_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailProductionLotLiquidations_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailProductionLotTrashs_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailProductionLotQualityAnalysiss_BeginCallback(s, e) {
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

    $("#btnAdvancedFilter").click(function (event) {
        popupAdvancedFilter.PerformCallback();
        popupAdvancedFilter.Show();
    });
}

function ComboProviderproductionUnitProviderPool_SelectedIndexChanged(s, e) {
    var data = {
        id_provider: s.GetValue()
    };


    $.ajax({
        url: "PurchaseOrder/UpdatePurchaseOrderprotectiveProvider",
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

            if (result !== null && result !== undefined) {

                if (result.id_protectiveProvider != null) UpdatePurchaseOrderProtectiveProvider(result.id_protectiveProvider);

            }

        },
        complete: function () {
            //hideLoading();
            id_productionUnitProvider.PerformCallback();

        }
    });

}

function UpdatePurchaseOrderProtectiveProvider(protectiveProvider) {

    for (var i = 0; i < id_providerapparent.GetItemCount(); i++) {
        var providerapparent = id_providerapparent.GetItem(i);
        var into = false;

        if (protectiveProvider == providerapparent.value) {
            id_providerapparent.selectedValue = protectiveProvider;
            id_providerapparent.SetSelectedIndex(providerapparent.index);



            break;
        }


    }
}

function onGridViewgvDistributedCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnDistributedRow") {
        var data = {
            id: gvProductionLotReceptionEditFormPaymentsDetail.GetRowKey(e.visibleIndex),
            id_listPrice: id_priceList.GetValue()
        };
        showPage("ProductionLotReception/FormShowDistributed", data);
	}
}

function OnGetRowValues(value) {

    // 
    var data = {
        id: gvProductionLotReceptionEditFormPaymentsDetail.GetRowKey(e.visibleIndex)
    };

    $.ajax({
        url: "ProductionLotReception/FormShowDistributed",
        type: "post",
        data: { id: data },
        sync: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $.fancybox(result
            ,{
                width: '700px',
                height: '1200px',
                'scrolling': 'no',
                modal: true,
            });
        },
        complete: function () {
            hideLoading();
        }
    });
}

//Prouction Lot Distributed
//Boton Cerrar

function ButtonCancel_Click(s, e) {
    
    var data = {
        id: $("#id_productionLot").val(),
        tabSelected: 8,
        loteManual: false
    };

    showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
}

var errorMessage = "";
var id_codigoProAux = null;

//Validation
function OnCodeDetailValidation(s, e) {

	errorMessage = "";

	$("#GridMessageErrorsDetail").hide();

	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Código: Es obligatorio.";
		} else {
			errorMessage += "</br>- Código: Es obligatorio.";
		}
	} else {
		var data = {
            id_codigoItem: s.GetValue(),
            id_Lot: $("#id_productionLotPayment").val()
        };
        // 
        if (data.id_codigoItem != id_codigoProAux) {
			$.ajax({
				url: "ProductionLotReception/ItsRepeatedDetail",
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
							if (errorMessage == null || errorMessage == "") {
								errorMessage = "- Código: " + result.Error;
							} else {
								errorMessage += "</br>- Código: " + result.Error;
							}
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

//Init de combo detalles Producto
function onCodeDetailLotReceptionInit(s, e) {

    id_codigoProAux = codigo.GetValue();
    var id_priceList = $("#id_codigoListaPrecio").val();

    if (id_codigoProAux != null && id_codigoProAux !== undefined) {
		

        $("#codigo").prop("readonly", true);

		$.ajax({
			url: "ProductionLotReception/LoadDatosGridViewReception",
			type: "post",
            data: { idCode: id_codigoProAux, id_price: id_priceList },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {

                if (result !== null && result !== undefined) {
                    codeProduct.SetValue(result.codigoProducto);
					nameProduct.SetValue(result.nombreProducto);
					nameCategory.SetValue(result.nombreCategoria);
					nameSize.SetValue(result.nombreTalla);
					nameProceso.SetValue(result.nombreProceso);
					nameUMProceso.SetValue(result.nombreUMProceso);
					nameUMPresentacion.SetValue(result.nombreUMPresentacion);
				}
			},
			complete: function () {

			}
		});
	}
}

//Cargar datos del GridView
function ComboCode_SelectedIndexChanged(s, e) {
    codeProduct.SetText("");
	nameProduct.SetText("");
	nameCategory.SetText("");
	nameSize.SetText("");
	nameProceso.SetText("");
	nameUMProceso.SetText("");
	nameUMPresentacion.SetText("");

    var data = codigo.GetValue();

    var id_priceList = $("#id_codigoListaPrecio").val();

	if (data != null && data !== undefined) {

		$.ajax({
			url: "ProductionLotReception/LoadDatosGridViewReception",
            type: "post",
            data: { idCode: data, id_price: id_priceList },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {

                if (result !== null && result !== undefined) {
                    codeProduct.SetValue(result.codigoProducto);
					nameProduct.SetValue(result.nombreProducto);
					nameCategory.SetValue(result.nombreCategoria);
					nameSize.SetValue(result.nombreTalla);
					nameProceso.SetValue(result.nombreProceso);
					nameUMProceso.SetValue(result.nombreUMProceso);
                    nameUMPresentacion.SetValue(result.nombreUMPresentacion);
                    priceLp.SetValue(result.preiceListItem);
				}
			},
			complete: function () {

			}
		});
	}
}

function onValueChangedNumberBox(s, e) {
	totalKg.SetValue(0);
	totalLb.SetValue(0);
    rendimiento.SetValue(0);
    totalPriceLp.SetValue(0);

	var data = codigo.GetValue();
	var numberBox = s.GetValue();
	var nombrePresentacionProducto = nameUMPresentacion.GetValue();
    var nombreProcesoUM = nameUMProceso.GetValue();
    var precioList = priceLp.GetValue();

	if (data != null && data !== undefined && numberBox > 0) {

		$.ajax({
			url: "ProductionLotReception/LoadDatosGridViewReceptionKg",
			type: "post",
            data: { idCode: data, numberB: numberBox, nombreP: nombrePresentacionProducto, nombrePro: nombreProcesoUM},
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
            success: function (result) {

				if (result !== null && result !== undefined) {

					if (result.rendimientoTotal > 0) {
						totalKg.SetValue(result.totalKilo);
                        rendimiento.SetValue(result.totalKilo);
                        totalPriceLp.SetValue(rendimiento.GetValue() * precioList);
					}
					else {
						totalKg.SetValue(result.totalKilo);
					}
					
				}
			},
			complete: function () {

			}
		});

		$.ajax({
			url: "ProductionLotReception/LoadDatosGridViewReceptionLbs",
			type: "post",
            data: { idCode: data, numberB: numberBox, nombreP: nombrePresentacionProducto, nombrePro: nombreProcesoUM},
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				if (result !== null && result !== undefined) {

					if (result.rendimientoTotal > 0) {
						totalLb.SetValue(result.totalLibras);
                        rendimiento.SetValue(result.totalLibras);
                        totalPriceLp.SetValue(rendimiento.GetValue() * precioList);
					}
					else {
						totalLb.SetValue(result.totalLibras);
					}
					
				}
			},
			complete: function () {

			}
        });

	}
}

function ProductionLotReceptionDistributedDetail_OnBeginCallback(s, e) {

	var objproductionLotReceptionCostType = ASPxClientControl.GetControlCollection().GetByName("id_productionLotPayment");
	e.customArgs['id_productionLotPayment'] = $("#id_productionLotPayment").val();
	e.customArgs['id_productionLotPaymentType'] = (objproductionLotReceptionCostType != undefined) ? ((objproductionLotReceptionCostType != null) ? objproductionLotReceptionCostType.GetValue() : 0) : 0;
}

function ProductionLotReceptionDistributedDetailGridProduct_OnBeginCallback(s, e) {

    e.customArgs['id_productionLotPayment'] = $("#id_productionLotPayment").val();
}

function OnGridViewPaymentReceptionDetailDistributedEndCallback(s, e) {
	gvProductionLotReceptionEditFormClosesDetailDistributed.PerformCallback();
}

function OnGridViewPaymentReceptionDetailDistributedInit(s, e) {
    gvProductionLotReceptionEditFormClosesDetailDistributed.PerformCallback();
}

function OnGridViewInitProductionDetailDistributedNew(s, e) {
    gvProductionLotReceptionEditFormClosesDetailDistributed.PerformCallback();
}

//Acción del botón actualizar
function ButtonUpdate_Click(s, e) {

	Update(false);

}

function Update(approve) {
	var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabdetail", true);

	if (!valid) {
		UpdateTabImage({ isValid: false }, "tabdetail");
    }

	if (valid) {

		var id = $("#id_productionLotPayment").val();

        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#FormEditDistributed").serialize();
		var url = "ProductionLotReception/ProductionLotReceptionPartialUpdate";

		showForm(url, data);

	}
}

$(function () {
    init();
});
