
//#region Filtro Busqueda

function DocumentStateCombo_Init() {
    id_documentState.SetValue(null);
    id_documentState.SetText("");
}

function WarehouseCombo_Init() {
    id_Warehouse.SetValue(null);
    id_Warehouse.SetText("");
}

function InventoryLineCombo_Init() {
    id_InventoryLine.SetValue(null);
    id_InventoryLine.SetText("");
}

function OnClickSearchCostAllocation() {
    var data = $("#CostAllocationFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "CostAllocation/CostAllocationResultsPartial",
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

function OnClickClearFiltersCostAllocation() {
    DocumentStateCombo_Init();
    WarehouseCombo_Init();
    InventoryLineCombo_Init();
    number.SetText("");
    reference.SetText("");
    startEmissionDate.SetText("");
    endEmissionDate.SetText("");
    
}

function OnClickNewCostAllocation(s, e) {

    var data = {
        idCostAllocation: gvCostAllocationResult.GetRowKey(e.visibleIndex)
    };

    showPage("CostAllocation/Edit", data);

    //var data = {
    //    id: gvProductionLotProcess.GetRowKey(e.visibleIndex)
    //};

    //showPage("ProductionLotProcess/ProductionLotProcessFormEditPartial", data);
}

function ButtonAddNewCostAllocation() {
    var data = {
        id: 0
    };

    showPage("CostAllocation/Edit", data);
}

//#endregion


//#region ToolBar Filtro 
function AddNewCost(s, e)
{
    OnClickNewCostAllocation(s, e)
}

function CopyCosts(s, e) {

}

function ApproveCosts(s, e) {

}

function CancelCosts(s, e) {
    showLoading();
    gvCostAllocationResult.GetSelectedFieldValues("id;Document.DocumentState.description", function (values)
    {
       
        let aprobados = values.find(r => r[1] == "APROBADA");
        if (typeof aprobados !== 'undefined')
        {
            hideLoading();
            NotifyError("Seleccionar solo Asignaciones en estado Pendiente.");
            return;
        }
        let pendientes = values.filter(r => r[1] == "PENDIENTE");
        if (pendientes.length == 0)
        {
            hideLoading();
            NotifyError("No ha seleccionado Asignaciones en estado Pendiente.");
            return;
        }
        let anulados = values.find(r => r[1] == "ANULADA");
        if (typeof anulados !== 'undefined') {
            hideLoading();
            NotifyWarning("Se procesarán solo las Asignaciones en estado Pendiente.");
        }
        let data = {
            idsCancel: pendientes.map(r=> r[0]),
        };
        hideLoading();
        procesarFuncion("CostAllocation/CancelSelected", data, function (result)
        {
            if (result.status == "error") {
                NotifyError(result.message);
            } else
            {
                NotifySuccess(result.message);
                gvCostAllocationResult.PerformCallback();
            }
        }, true);
    })
}

function RevertCosts(s, e) {

    showLoading();
    gvCostAllocationResult.GetSelectedFieldValues("id;Document.DocumentState.description", function (values) {
        
        let pendientes = values.find(r => r[1] == "PENDIENTE");
        if (typeof pendientes !== 'undefined' ) {
            hideLoading();
            NotifyError("Seleccionar solo Asignaciones en estado Aprobada.");
            return;
        }

        let aprobados = values.filter(r => r[1] == "APROBADA");
        if (aprobados.length == 0) {
            hideLoading();
            NotifyError("No ha seleccionado Asignaciones en estado Aprobada.");
            return;
        }
        let anulados = values.find(r => r[1] == "ANULADA");
        if (typeof anulados !== 'undefined') {
            hideLoading();
            NotifyWarning("Se procesarán solo las Asignaciones en estado Aprobada.");
        }
        let data = {
            idsRevert: aprobados.map(r => r[0]),
        };
        hideLoading();
        procesarFuncion("CostAllocation/RevertSelected", data, function (result) {
            if (result.status == "error") {
                NotifyError(result.message);
            } else {
                NotifySuccess(result.message);
                gvCostAllocationResult.PerformCallback();
            }
        }, true);
    })
}

function PrintCosts(s, e) {

}

function OnCostAllocationResultInit(s, e) {
    //UpdateTitlePanel();
}
function OnCostAllocationResultSelectionChanged(s, e) {
    UpdateTitlePanel();
}
function OnCostAllocationResultEndCallback(s, e) {
    UpdateTitlePanel();
}


function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvCostAllocationResult.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCostAllocationResult.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvCostAllocationResult.GetSelectedRowCount() > 0 && gvCostAllocationResult.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCostAllocationResult.GetSelectedRowCount() > 0);

    //btnCopy.SetEnabled(gvCostAllocationResult.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvCostAllocationResult.GetSelectedRowCount() > 0);
    btnCancel.SetEnabled(gvCostAllocationResult.GetSelectedRowCount() > 0);
    btnRevert.SetEnabled(gvCostAllocationResult.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvCostAllocationResult.cpFilteredRowCountWithoutPage + gvCostAllocationResult.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}
//#endregion



//#region Funciones Generales
function procesarFuncion(url, data, callback, viewLoading ) {
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
            
            if (viewLoading)
            {
                showLoading();
            }
        },
        success: function (result) {
            callback(result);
        },
        complete: function (result) {
            if (viewLoading) {
                hideLoading();
            }
        }
    });
}
function IsValid(object) {
    return (object != null && object != undefined && object != "undefined");
}

function wdecSep() {
    var n = 1.1;
    n = n.toLocaleString().substring(1, 2);
    return n;
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

//#endregion

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
