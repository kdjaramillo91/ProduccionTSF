function PrintItem(data, url) {
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
            try {
                if (result !== undefined) {
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

function PrintLot(data, url) {
    
    var data = {
        id: $("#imovExit").val(),
        codeReport: "INEGEG",
    };

    var url = "ProductionLotProcess/PrintEgreso";

    PrintItem(data, url);
}

function printIngresoBodega(data, url) {
    var data = {
        id: $("#imoveEntry").val(),
        codeReport: "INEGIN",
    };

    var url = "ProductionLotProcess/PrintIngreso";

    PrintItem(data, url);
}

function printProcesoInternoMerma(data, url) {
    var data = {
        id: $("#id_productionLot").val(),
        codeReport: "RPMER",
    };

    var url = "ProductionLotProcess/PrintIngreso";

    PrintItem(data, url);
}

function printLiquidacionProcesoInterno(data, url) {

     var data = {
        id: $("#id_productionLot").val(),
        codeReport: "PROCIN",
    };

    var url = "ProductionLotProcess/PrintIngreso";

    PrintItem(data, url);
}


function MachineForProd_SelectedIndexChanged(s, e) {
    
    var idMachProdOpeningDetail = 0;
    var idMachineProdOpeningTmp = 0;
    
    if (s.GetValue() !== null && s.GetValue !== undefined) {
        var objItemSelected2 = id_MachineProdOpeningDetail.GetSelectedItem();
        if (objItemSelected2 != null) {
            idMachineProdOpeningTmp = objItemSelected2.GetColumnText(2);
            idMachProdOpeningDetail = objItemSelected2.GetColumnText(3);            
        }
    }

    if (idMachProdOpeningDetail == 0 && idMachineProdOpeningTmp == 0) return;

    $.ajax({
        url: "ProductionLotProcess/GetDataMachineForProd",
        type: "post",
        data: { id_MachineForProd: idMachProdOpeningDetail, id_MachineProdOpening: idMachineProdOpeningTmp },
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
                $("#id_MachineForProd").val(result.id_MachineForProd);
                $("#id_MachineProdOpening").val(result.id_MachineProdOpening);
               // $("#timeInitMachineProdOpeningDetail").val(new Date(2011, 1, 1, result.timeInitMachineProdOpeningDetail.Hours, result.timeInitMachineProdOpeningDetail.Minutes, result.timeInitMachineProdOpeningDetail.Seconds));
            }
        },
        complete: function () {
        }
    });
}

function MachineForProd_BeginCallback(s, e) {
    var _receptionDate = receptionDate.GetDate();

    var yearInicio = _receptionDate.getFullYear();
    var monthInicio = _receptionDate.getMonth() + 1;
    var dayInicio = _receptionDate.getDate();

    e.customArgs["id_MachineForProd"] = s.cpMachineForProd;
    e.customArgs["id_MachineProdOpening"] = s.cpMachineProdOpening;
    e.customArgs["documentStateCode"] = s.cpDocumentStateCode;
    e.customArgs["emissionDate"] = dayInicio + "/" + monthInicio + "/" + yearInicio;
    e.customArgs["id_PersonProcessPlant"] = s.cpPersonProcessPlant;
    e.customArgs["id_Turn"] = id_Turn.GetValue();
}
//Maquina
function MachineForProd_Init(s, e) {
    var idMachProdOpeningDetail = $("#id_MachineProdOpeningT2").val();

    if (idMachProdOpeningDetail === null || idMachProdOpeningDetail === undefined || idMachProdOpeningDetail === 0) {
        id_MachineProdOpeningDetail.SetValue(null);
    } else {
        id_MachineProdOpeningDetail.SetValue(idMachProdOpeningDetail);
        //MachineForProd_SelectedIndexChanged(s, e);

    }
}

function Update(approve) {
    receptionDate.SetEnabled(true);
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
     
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

    if (!valid) {
        UpdateTabImage({ isValid: false }, "tabDocument");
    }


    var codeState = $("#codeState").val();

    if (codeState == "01") {
        if (gvProductionLotProcessEditFormItemsDetail.cpRowsCount === 0 ||
            gvProductionLotProcessEditFormItemsDetail.IsEditing()) {
            UpdateTabImage({ isValid: false }, "tabItemsDetails");
            valid = false;
        }
    }

    if (codeState == "02" || codeState == "03") {
        if (gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowsCount === 0 ||
            gvProductionLotEditFormProductionLotLiquidationsDetail.IsEditing()) {
            UpdateTabImage({ isValid: false }, "tabProductionLotLiquidationsDetails");
            valid = false;
        }
        if (gvProductionLotEditFormProductionLotPackingMaterialsDetail.IsEditing()) {
            UpdateTabImage({ isValid: false }, "tabProductionLotPackingMaterialDetails");
            valid = false;
        }
        if (gvProductionLotEditFormProductionLotTrashsDetail.IsEditing()) {
            UpdateTabImage({ isValid: false }, "tabProductionLotTrashsDetails");
            valid = false;
        }
        if (gvProductionLotEditFormProductionLotLossDetail.IsEditing()) {
            UpdateTabImage({ isValid: false }, "tabProductionLotLossDetails");
            valid = false;
        }
    }

    if (valid) {
        var id = $("#id_productionLot").val();
        let dataLiqNoVal = "";
        var isRequestCarMachine = $("#isRequestCarMachine").val(); 
        if (isRequestCarMachine == 'True')
        {
            dataLiqNoVal = getDataLiqNoVal();
        }
        var data = `id=${id}&approve=${approve}&${$("#formEditProductionLotProcess").serialize()}&${dataLiqNoVal}`;
        //"id=" + id + "&" + "approve=" + approve + "&" + $("#formEditProductionLotProcess").serialize();
        var url = (id === "0") ? "ProductionLotProcess/ProductionLotProcessAddNew"
            : "ProductionLotProcess/ProductionLotProcessUpdate";

        showForm(url, data);
    } else {
        receptionDate.SetEnabled(false);
	}
}

function getDataLiqNoVal()
{
    let data = "";
    if (typeof id_MachineProdOpeningDetailLiqNoVal != 'undefined') {
        if (id_MachineProdOpeningDetailLiqNoVal.GetValue() !== null) {
            var objItemSelected2 = id_MachineProdOpeningDetailLiqNoVal.GetSelectedItem();
            if (objItemSelected2 != null) {

                let vidMachineProdOpeningDetail = objItemSelected2.GetColumnText(5);
                let vidMachineProdOpening = objItemSelected2.GetColumnText(2);
                let vidTurn = objItemSelected2.GetColumnText(4);
                let vidMachineForProd = objItemSelected2.GetColumnText(3);

                let vtimeInit = timeInitLiqNoVal.GetText();
                let vtimeEnd = timeEndLiqNoVal.GetText();

                data = `idMachineProdOpeningDetail=${vidMachineProdOpeningDetail}&idMachineProdOpening=${vidMachineProdOpening}&idTurn=${vidTurn}
            &idMachineForProd=${vidMachineForProd}&timeInit=${vtimeInit}&timeEnd=${vtimeEnd}`;
            }

            return data;

        }
    }
    
}

function tabControl_init(s, e) {
    receptionDate.SetEnabled(!tabControl.cpExistenRegistros);
}

// DIALOG BUTTONS ACTIONS
function ButtonUpdate_Click(s, e) {
    Update(false);
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    var isBtnToReturn = $("#isBtnToReturn").val();
    if (isBtnToReturn == "True" || isBtnToReturn == "true" || isBtnToReturn == true) {
        btnToReturn_click(s, e);
    } else {
        showPage("ProductionLotProcess/Index", null);
    }
    
}

// Edit Form Action Buttons

//btnNew
function AddNewLot(s, e) {
    var data = {
        id: 0
    };

    showPage("ProductionLotProcess/ProductionLotProcessFormEditPartial", data);
}

//btnSave
function SaveLot(s, e) {
    ButtonUpdate_Click(s, e);
}

//btnSaveClose
function SaveCloseLot(s, e) {
    ButtonUpdateClose_Click(s, e);
}

//btnCopy
function CopyLot(s, e) {
    showPage("ProductionLotReception/ProductionLotProcessCopy", { id: $("#id_productionLot").val() });
}

//btnApprove
function ApproveLot(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar el lote?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_productionLot").val()
    //    };
    //    showForm("ProductionLotProcess/Approve", data);
    //}, "¿Desea aprobar el lote?");
}

//btnTransfer
function TransferirLot(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea transferir el lote?");

}

//btnAutorize
function AutorizeLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotProcess/Autorize", data);
    }, "¿Desea autorizar el lote?");
}

//btnProtect
function ProtectLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotProcess/Protect", data);
    }, "¿Desea cerrar el lote?");
}

//btnCancel
function CancelLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotProcess/Cancel", data);
    }, "¿Desea anular el lote?");
}

//btnRevert
function RevertLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotProcess/Revert", data);
    }, "¿Desea reversar el lote?");
}

//btnHistory
function ShowLotHistory(s, e) {

}

//btnConciliation
function ConciliationLot(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionLot").val()
        };
        showForm("ProductionLotProcess/Conciliation", data);
    }, "¿Desea conciliar el lote?");
}

//btnPrint
//function PrintLot(s, e) {
//    //var _url = "PurchaseRequest/PurchaseRequestsReport";
//    //var id = $("#id_purchaseRequest").val();
//
//    //if (!(id == 0) && !(id == null)) {
//    //    var ids = [id];
//    //    $.ajax({
//    //        url: _url,
//    //        type: "post",
//    //        data: { ids: ids },
//    //        async: true,
//    //        cache: false,
//    //        error: function (error) {
//    //            console.log(error);
//    //        },
//    //        beforeSend: function () {
//    //            showLoading();
//    //        },
//    //        success: function (result) {
//    //            $("#maincontent").html(result);
//    //        },
//    //        complete: function () {
//    //            hideLoading();
//    //        }
//    //    });
//    //}
//    $.ajax({
//        url: "ProductionLotProcess/ProductionLotProcessReport",
//        type: "post",
//        data: { id: $("#id_productionLot").val() },
//        async: true,
//        cache: false,
//        error: function (error) {
//           console.log(error);
//       },
//        beforeSend: function () {
//           showLoading();
//       },
//        success: function (result) {
//            $("#maincontent").html(result);
//        },
//        complete: function () {
//           hideLoading();
//       }
//    });
//}

// TABS CONTROL ACTIONS

//var gv = null;

//function TabControl_ActiveTabChanged(s, e) {
//    var activeTab = tabControl.GetActiveTab();

//    if (activeTab === null || activeTab === undefined) {
//        gv = null;
//        return;
//    }

//    if (activeTab.name === "tabItemsDetails") {
//        gv = gvProductionLotProcessEditFormItemsDetail;
//    } else if (activeTab.name === "tabProductionLotLiquidationsDetails") {
//        gv = gvProductionLotProcessEditFormProductionLotLiquidationsDetail;
//    } else if (activeTab.name === "tabProductionLotTrashsDetails") {
//        gv = gvProductionLotProcessEditFormProductionLotTrashsDetail;
//    }

//    UpdateTitlePanel();
//}

// COMMON ACTIONS BUTTONS

function AddNew(s, e) {
    //if (gv !== null && gv !== undefined) {
    //    gv.AddNewRow();
    //}
}

function Remove(s, e) {
}

function Refresh(s, e) {
    ////if (gv !== null && gv !== undefined) {
    ////    gv.UnselectRows();
    ////    gv.PerformCallback();
    ////}
}

// DETAILS ACTIONS

function AddNewDetail(s, e) {
    if (id_productionProcess.GetValue() == null) return;
    // TODO: Mensaje para indicar que debe seleccionar proceso
    gvProductionLotProcessEditFormItemsDetail.AddNewRow();
}

function RemoveDetail(s, e) {
    Remove(s, e);
}

function RefreshDetail(s, e) {
     
    if (gvProductionLotProcessEditFormItemsDetail !== null && gvProductionLotProcessEditFormItemsDetail !== undefined) {
        gvProductionLotProcessEditFormItemsDetail.UnselectRows();
        gvProductionLotProcessEditFormItemsDetail.PerformCallback();
    }
}

// LIQUIDATION ACTIONS

function AddNewLiquidation(s, e) {
    gvProductionLotEditFormProductionLotLiquidationsDetail.AddNewRow();
    //gvProductionLotProcessEditFormProductionLotLiquidationsDetail.AddNewRow();
}

function RemoveLiquidation(s, e) {
    Remove(s, e);
}

function RefreshLiquidation(s, e) {
    gvProductionLotEditFormProductionLotLiquidationsDetail.UnselectRows();
    gvProductionLotEditFormProductionLotLiquidationsDetail.PerformCallback();
    //if (gvProductionLotProcessEditFormProductionLotLiquidationsDetail !== null && gvProductionLotProcessEditFormProductionLotLiquidationsDetail !== undefined) {
    //    gvProductionLotProcessEditFormProductionLotLiquidationsDetail.UnselectRows();
    //    gvProductionLotProcessEditFormProductionLotLiquidationsDetail.PerformCallback();
    //}
}

// PACKING MATERIAL ACTIONS

function AddNewPackingMaterial(s, e) {
    gvProductionLotEditFormProductionLotPackingMaterialsDetail.AddNewRow();
    //AddNew(s, e);
}

function RemovePackingMaterial(s, e) {
    Remove(s, e);
}

function RefreshPackingMaterial(s, e) {
    //Refresh(s, e);
    gvProductionLotEditFormProductionLotPackingMaterialsDetail.UnselectRows();
    gvProductionLotEditFormProductionLotPackingMaterialsDetail.PerformCallback();
}

// LIQUIDATION ACTIONS

function AddNewTrash(s, e) {
    gvProductionLotEditFormProductionLotTrashsDetail.AddNewRow();
    //gvProductionLotProcessEditFormProductionLotTrashsDetail.AddNewRow();
}

function RemoveTrash(s, e) {
    Remove(s, e);
}

function RefreshTrash(s, e) {
    gvProductionLotEditFormProductionLotTrashsDetail.UnselectRows();
    gvProductionLotEditFormProductionLotTrashsDetail.PerformCallback();
    //if (gvProductionLotProcessEditFormProductionLotTrashsDetail !== null && gvProductionLotProcessEditFormProductionLotTrashsDetail !== undefined) {
    //    gvProductionLotProcessEditFormProductionLotTrashsDetail.UnselectRows();
    //    gvProductionLotProcessEditFormProductionLotTrashsDetail.PerformCallback();
    //}
}
function RefreshLoss(s, e) {
    gvProductionLotEditFormProductionLotLossDetail.UnselectRows();
    gvProductionLotEditFormProductionLotLossDetail.PerformCallback();
}
function AddNewLoss(s, e) {
    gvProductionLotEditFormProductionLotLossDetail.AddNewRow();
}
function RemoveLoss(s, e) {
    RefreshLoss(s, e);
}
// DETAILS AND DISPATCH MATERIALS SELECTION

//function OnGridViewDetailsInit(s, e) {
//    TabControl_ActiveTabChanged(s, e);
//}

//function OnGridViewDetailsSelectionChanged(s, e) {
//    UpdateTitlePanel();
//    s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallback);
//}

// DETAILS VIEW CALLBACKS

function ProductionLotProcessDetailItems_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

// ---------------
// Unit Production
// ---------------
var _id_ProductionUnit = null;
function ComboProductionUnit_BeginCallback(s, e)
{
    if (s !== undefined)
    {
        if(s.GetValue() != null)
            //e.customArgs["id_ProductionUnitCurrent"] = _id_ProductionUnit;
            e.customArgs["id_ProductionUnitCurrent"] = s.GetValue();
    }
    
}

function ComboProductionUnit_EndCallback(s, e)
{
    s.SetValue(_id_ProductionUnit);
    //if (s.initializeInputValue !== null && s.initializeInputValue !== 0) {
    //
    //    id_productionUnit.SetValue(s.initializeInputValue);
    //}
}

function ComboProductionUnit_SelectedIndexChanged(s, e) {
    _id_ProductionUnit = s.GetValue();

}

var currentWarehouse = 0;
var currentWarehouseLocation = 0;
function OnProductionProcess_SelectedIndexChanged(s, e)
{
    var idProductionProcess = s.GetValue();

    var id = $("#id_productionLot").val();
    var _data = "id=" + id + "&" + "idProductionProcess=" + idProductionProcess + "&" + $("#formEditProductionLotProcess").serialize();
    // data: { productionProcessId: idProductionProcess },
    $.ajax({
        url: "ProductionLotProcess/SetProductionProcessId",
        type: "post",
        data: _data,
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
                $("#maincontent").html(result);
            }
             
        },
        complete: function () {
            hideLoading();
        }
    });

    //id_productionUnit.PerformCallback();
}


//function GetSelectedFieldValuesCallback(values) {
//    selectedRows = [];
//    for (var i = 0; i < values.length; i++) {
//        selectedRows.push(values[i]);
//    }
//}

//var customCommand = "";

//function OnGridViewDetailsBeginCallback(s, e) {
//    customCommand = e.command;
//}

//function OnGridViewDetailsEndCallback(s, e) {
//    UpdateTitlePanel();

//    if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
//        s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
//    }
//    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
//    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
//    //}

//    //if (s.GetEditor("id_originLot") !== null && s.GetEditor("id_originLot") !== undefined) {
//    //    s.GetEditor("id_originLot").SetEnabled(customCommand === "ADDNEWROW");
//    //}
//    //if (gv !== gvProductionLotReceptionEditFormMaterialsDetail && gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
//    //    UpdateProductionLotTotals();
//    //}
//    UpdateProductionLotTotals();
//}

//function UpdateTitlePanel() {

//    if (gv === null || gv === undefined)
//        return;

//    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

//    var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
//    var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
//    if (hiddenSelectedRowCount > 0)
//        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
//    text += "<br />";

//    //var element = (gv === gvProductionLotProcessEditFormItemsDetail) ? "Items" : "Materials";
//    var element = (gv === gvProductionLotProcessEditFormItemsDetail) ? "Items" : (gv === gvProductionLotProcessEditFormProductionLotLiquidationsDetail) ? "Liquidations" : "Trashs";

//    $("#lblInfo" + element).html(text);

//    if ($("#selectAllMode").val() !== "AllPages") {
//        SetElementVisibility("lnkSelectAllRows" + element, gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
//        SetElementVisibility("lnkClearSelection" + element, gv.GetSelectedRowCount() > 0);
//    }

//    btnRemoveDetail.SetEnabled(gv.GetSelectedRowCount() > 0);
//}

//function GetSelectedFilteredRowCount() {
//    return gv.cpFilteredRowCountWithoutPage + gv.GetSelectedKeysOnPage().length;
//}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

//function gvEditDetailsClearSelection() {
//    gv.UnselectRows();
//}

//function gvEditDetailsSelectAllRows() {
//    gv.SelectRows();
//}

//function OnGridViewItemsDetailSelectionChanged(s, e) {
//    UpdateTitlePanel();
//    //s.GetSelectedFieldValues("id_originLot;id_item", GetSelectedFieldValuesItemsDetailCallback);
//    s.GetSelectedFieldValues("id", GetSelectedFieldValuesItemsDetailCallback);
//}

//function GetSelectedFieldValuesItemsDetailCallback(values) {
//    var selectedRows = [];
//    for (var i = 0; i < values.length; i++) {
//        //selectedRows.push({ id_originLot: values[i].id_originLot, id_item: values[i].id_item });
//        selectedRows.push(values[i]);
//    }
//}

// UPDATE PRODUCTION LOT TOTALS

function UpdateProductionLotTotals() {
    $.ajax({
        url: "ProductionLotProcess/ProductionLotTotals",
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
                //totalQuantityOrdered.SetValue(result.totalQuantityOrdered);
                //totalQuantityRemitted.SetValue(result.totalQuantityRemitted);
                totalQuantityRecivedDoc.SetValue(result.totalQuantityRecived);
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
    //var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
    //s.filterStrategy.FilteringOnClient = function () {
    //    //create a new format string for all list box columns. you could skip this bit and just set
    //    //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
    //    //columns 1 and 3
    //    var lb = this.GetListBoxControl();
    //    var filterTextFormatStringItems = [];
    //    for (var i = 0; i < lb.columnFieldNames.length; i++) {
    //        filterTextFormatStringItems.push('{' + i + '}');
    //    }
    //    var filterTextFormatString = filterTextFormatStringItems.join(' ');

    //    //store actual format string and override with one for all columns
    //    var actualTextFormatString = lb.textFormatString;
    //    lb.textFormatString = filterTextFormatString;

    //    //call actual filtering method which will now work on our temporary format string
    //    actualFilteringOnClient.apply(this);

    //    //restore original format string
    //    lb.textFormatString = actualTextFormatString;
    //};
}

//function OnItemDetailBeginCallback(s, e) {
//    e.customArgs["id_item"] = $("#ItemDetail" + gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey).val();
//    e.customArgs["indice"] = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey;
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
    var id = parseInt($("#id_productionLot").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);
    btnPrint.SetEnabled(false);
    btnPrintIngresoBodega.SetEnabled(false);
    btnPrintLiquidacionProcesoInterno.SetEnabled(false);


    // STATES BUTTONS

    $.ajax({
        url: "ProductionLotProcess/Actions",
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
            btnTransfer.SetEnabled(result.btnTransfer);
            btnPrint.SetEnabled(result.btnPrint );
            btnPrintIngresoBodega.SetEnabled(result.btnPrintIngresoBodega);
            btnPrintLiquidacionProcesoInterno.SetEnabled(result.btnPrintLiquidacionProcesoInterno);
            btnConciliation.SetEnabled(result.btnConciliation);
        },
        complete: function (result) {
            //hideLoading();
        }
    });

    // HISTORY BUTTON
    btnHistory.SetEnabled(id !== 0);

    // PRINT BUTTON
    btnPrint.SetEnabled(id !== 0);

    var isBtnToReturn = $("#isBtnToReturn").val();
    if (isBtnToReturn == "True" || isBtnToReturn == "true" || isBtnToReturn == true) {
        //$("#pagination").show();
        $('.pagination').hide();
        btnNew.SetEnabled(false);
    } else {
        //$("#pagination").hide();
        $('.pagination').show();
        btnNew.SetEnabled(true);
    }

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


//#region Liuidacion No Valorizada
function MachineForProdLiqNoVal_Init(s, e) {
    var idMachProdOpeningDetail = $("#id_MachineProdOpeningT2LiqNoVal").val();

    if (idMachProdOpeningDetail === null || idMachProdOpeningDetail === undefined || idMachProdOpeningDetail === 0) {
        id_MachineProdOpeningDetail.SetValue(null);
    } else {
        id_MachineProdOpeningDetailLiqNoVal.SetValue(idMachProdOpeningDetail);
        //MachineForProd_SelectedIndexChanged(s, e);

    }
}

function MachineForProdLiqNoVal_SelectedIndexChanged(s, e) {

    var idMachProdOpeningDetail = 0;
    var idMachineProdOpeningTmp = 0;
    let turnName = "";
    if (s.GetValue() !== null && s.GetValue !== undefined) {
        var objItemSelected2 = id_MachineProdOpeningDetailLiqNoVal.GetSelectedItem();
        if (objItemSelected2 != null) {
            idMachineProdOpeningTmp = objItemSelected2.GetColumnText(2);
            idMachProdOpeningDetail = objItemSelected2.GetColumnText(3);
            turnName = objItemSelected2.GetColumnText(1);
        }

    }

    TurnNameLiqNoVal.SetValue(turnName);

    if (idMachProdOpeningDetail == 0 && idMachineProdOpeningTmp == 0) return;

    $.ajax({
        url: "ProductionLotProcess/GetDataMachineForProd",
        type: "post",
        data: { id_MachineForProd: idMachProdOpeningDetail, id_MachineProdOpening: idMachineProdOpeningTmp },
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
                $("#id_MachineForProdLiqNoVal").val(result.id_MachineForProd);
                $("#id_MachineProdOpeningLiqNoVal").val(result.id_MachineProdOpening);
                // $("#timeInitMachineProdOpeningDetail").val(new Date(2011, 1, 1, result.timeInitMachineProdOpeningDetail.Hours, result.timeInitMachineProdOpeningDetail.Minutes, result.timeInitMachineProdOpeningDetail.Seconds));
            }
        },
        complete: function () {
        }
    });
}

function MachineForProdLiqNoVal_BeginCallback(s, e) {
    var _receptionDate = receptionDate.GetDate();

    var yearInicio = _receptionDate.getFullYear();
    var monthInicio = _receptionDate.getMonth() + 1;
    var dayInicio = _receptionDate.getDate();

    e.customArgs["id_MachineForProd"] = s.cpMachineForProd;
    e.customArgs["id_MachineProdOpening"] = s.cpMachineProdOpening;
    e.customArgs["documentStateCode"] = s.cpDocumentStateCode;
    e.customArgs["emissionDate"] = dayInicio + "/" + monthInicio + "/" + yearInicio;
    e.customArgs["id_PersonProcessPlant"] = s.cpPersonProcessPlant;
    e.customArgs["id_Turn"] = id_Turn.GetValue();
}

function MachineForProdLiqNoVal_GetDate()
{
    

}
//#endregion
