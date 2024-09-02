//#region Modo Edicion
function OnGridViewCostAllocationResumeInit(s, e) {
    UpdateTitlePanelEditResume();
}


function UpdateTitlePanelEditResume() {

   // var selectedFilteredRowCount = GetSelectedFilteredRowCountLiquidationDetails();
   //
   // var text = "Total de elementos seleccionados: <b>" + gvCostAllocationEditResumido.GetSelectedRowCount() + "</b>";
   // var hiddenSelectedRowCount = gvCostAllocationEditResumido.GetSelectedRowCount() - GetSelectedFilteredRowCountLiquidationDetails();
   // if (hiddenSelectedRowCount > 0)
   //     text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
   // text += "<br />";


    //$("#lblInfoLiquidations").html(text);
    
    //if ($("#selectAllMode").val() !== "AllPages") {
    //    SetElementVisibility("lnkSelectAllRowsLiquidations", gvCostAllocationEditResumido.GetSelectedRowCount() > 0 && gvCostAllocationEditResumido.cpVisibleRowCount > selectedFilteredRowCount);
    //    SetElementVisibility("lnkClearSelectionLiquidations", gvCostAllocationEditResumido.GetSelectedRowCount() > 0);
    //}

    //btnRemoveLiquidation.SetEnabled(gvCostAllocationEditResumido.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountLiquidationDetails() {
    return gvCostAllocationEditResumido.cpFilteredRowCountWithoutPage +
            gvCostAllocationEditResumido.GetSelectedKeysOnPage().length;
}

function OnGridViewCostAllocationResumeSelectionChanged(s, e) {
    UpdateTitlePanelEditResume();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackEditResumen);

}

function GetSelectedFieldValuesCallbackEditResumen(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}
var customCommand = "";
function OnGridViewCostAllocationResumeBeginCallback(s, e) {
    customCommand = e.command;
    ///var indexDetail = 0;
    //e.customArgs["idCostAllocation"] = s.cpIdProductionLot;
     
}

function OnGridViewCostAllocationResumeEndCallback(s, e) {
    UpdateTitlePanelEditResume();
    //gvCostAllocationEditResumido.PerformCallback();
}

//#endregion

//#region Controles
function OnNumberChange_unitCostPounds(s, e) {
    
    showLoading();
    let costoLibras = s.GetValue();
    costoLibras = (typeof costoLibras == undefined || costoLibras == null) ? 0 : costoLibras;
    calculateCost(costoLibras);
}

function OnValidation_unitCostPounds(s, e) {
    //// 
    let valor = s.GetValue();

    if (valor < 0 )
    {
        e.errorText = "Debe ingresar valor superior a 0";
        e.isValid = false;
        s.SetValue(0);
    }
}

//#endregion

//#region Operaciones
function calculateCost(costoUnitarioLibras)
{
    //let cantidadCajas = amountBox.GetValue();
    //cantidadCajas = (typeof cantidadCajas == undefined || cantidadCajas == null) ? 0 : cantidadCajas;
    //
    //let cantidadLibras = amountPound.GetValue();
    //cantidadLibras = (typeof cantidadLibras == undefined || cantidadLibras == null) ? 0 : cantidadLibras;
    //
    //let cantidadKilos = amountKg.GetValue();
    //cantidadKilos = (typeof cantidadKilos == undefined || cantidadKilos == null) ? 0 : cantidadKilos;
     
    let sep = wdecSep();
    let tl = costoUnitarioLibras.toLocaleString(undefined, { style: 'decimal', minimumFractionDigits: '5' });
    let unitAr = tl.split(sep);
    let _id_InventoryLine = id_InventoryLineRes.GetValue();
    let _id_ItemType = id_ItemTypeRes.GetValue();
    let _id_ItemTypeCategory = id_ItemTypeCategoryRes.GetValue();
    let _id_ItemSize = id_ItemSizeRes.GetValue();
    let data = {
        id_InventoryLine:_id_InventoryLine,
        id_ItemType:_id_ItemType,
        id_ItemTypeCategory:_id_ItemTypeCategory,
        id_ItemSize: _id_ItemSize,
        unitCostPoundsEnt: unitAr[0],
        unitCostPoundsDec: unitAr[1],

    }
    

    procesarFuncion("CostAllocation/CalculateResumido", data, function (result) {
        
        if (result.estado == "ERR") {
            NotifyError(result.err);
            return;
        }
        unitCostKg.SetValue(result.unitCostKg);
        averageCostUnit.SetValue(result.averageCostUnit);
        totalCostPounds.SetValue(result.totalCostPounds);
        totalCostKg.SetValue(result.totalCostKg);
        totalCostUnit.SetValue(result.totalCostUnit);

        
    }, true);

    //let costoUnitarioKg = (costoUnitarioLibras * 2.2046);
    //let costoTotalLb = (costoUnitarioLibras * cantidadLibras);
    //let costoTotalKg = ((costoUnitarioLibras * 2.2046) * cantidadKilos);
    //let costoPromedioUnidades = (costoTotalLb / cantidadCajas);
    //
    //unitCostKg.SetValue(costoUnitarioKg);
    //averageCostUnit.SetValue(costoPromedioUnidades);
    //totalCostPounds.SetValue(costoTotalLb);
    //totalCostKg.SetValue(costoTotalKg);
    //totalCostUnit.SetValue(costoTotalLb);

}

//#endregion




