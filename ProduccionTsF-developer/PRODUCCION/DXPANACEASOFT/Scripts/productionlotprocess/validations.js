
// TABIMAGE

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function OnInternalNumberValidation(s, e) {
    if (e.value !== null && e.value.length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
    }
}

function OnProductionProcessValidation(s, e)
{
     
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnLiquidatorValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function MachineForProd_Validation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
} 

function OnProductionUnitValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnTotalQuantityRecivedValidation(s, e) {
    var totalQuantityRecivedAux = e.value === null ? 0 : e.value;
    console.log("totalQuantityRecivedAux:" + totalQuantityRecivedAux);
    var totalQuantityLiquidationAux = totalQuantityLiquidation.GetValue();
    totalQuantityLiquidationAux = totalQuantityLiquidationAux === null ? 0 : totalQuantityLiquidationAux;
    console.log("totalQuantityLiquidationAux:" + totalQuantityLiquidationAux);
    var totalQuantityTrashAux = totalQuantityTrash.GetValue();
    totalQuantityTrashAux = totalQuantityTrashAux === null ? 0 : totalQuantityTrashAux;
    console.log("totalQuantityTrashAux:" + totalQuantityTrashAux);
    if (totalQuantityRecivedAux < (totalQuantityLiquidationAux + totalQuantityTrashAux)) {
        e.isValid = false;
        e.errorText = "Libras Recibidas debe ser mayor o igual a la suma de las Libras Liquidadas mas las Libras de Desperdicio";
    }

   

}

function OnInternalNumberTextChanged(s, e) {
    var jngt = julianoNumber.GetText();
    var ingt = internalNumber.GetText();
    internalNumberConcatenated.SetText(String(jngt) + String(ingt));
}


//#region Liquidacion No Valorizada
function MachineForProdLiqNoVal_Validation(s, e) {
    //if (e.value === null) {
    //    e.isValid = false;
    //    e.errorText = "Campo Obligatorio";
    //}
} 
//#endregion