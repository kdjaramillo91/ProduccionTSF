
//COMBOS
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvWarehousesTypes.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvWarehousesTypes !== null && gvWarehousesTypes !== undefined) {
        gvWarehousesTypes.CancelEdit();
    } 
}

function OnProductionCostInit(s, e) {
    SetEventFromCmbProductionCost(s, e);
}

function OnProductionCost_SelectedIndexChanged(s, e) {
    SetEventFromCmbProductionCost(s, e);
}

function SetEventFromCmbProductionCost(s, e) {
    let valSelected = s.GetValue();
    let _poundsType = ASPxClientControl.GetControlCollection().GetByName("poundsType");
    let _reasonCosts = ASPxClientControl.GetControlCollection().GetByName("reasonCosts");

    let _idProcessedPoundsSimpleFormula = ASPxClientControl.GetControlCollection().GetByName("idProcessedPoundsSimpleFormula");
    let _idFinishedPoundsSimpleFormula = ASPxClientControl.GetControlCollection().GetByName("idFinishedPoundsSimpleFormula");

    if (valSelected == "NO") {
        _poundsType.SetEnabled(false);
        _reasonCosts.SetEnabled(false);

        _idProcessedPoundsSimpleFormula.SetEnabled(false);
        _idFinishedPoundsSimpleFormula.SetEnabled(false);

    } else {
        _poundsType.SetEnabled(true);
        _reasonCosts.SetEnabled(true);

        // Verify Pounds Type
        let _valPoundsType = _poundsType.GetValue();

        if (_valPoundsType == "AMBAS" || _valPoundsType == "LIBPRO") {
            _idProcessedPoundsSimpleFormula.SetEnabled(true);
        } else {
            _idProcessedPoundsSimpleFormula.SetEnabled(false);
        }
        if (_valPoundsType == "AMBAS" || _valPoundsType == "LIBTERM") {
            _idFinishedPoundsSimpleFormula.SetEnabled(true);
        } else {
            _idFinishedPoundsSimpleFormula.SetEnabled(false);
        }
    }
}

function OnPoundType_Init(s, e) {
    SetEventFromCmbPoundType(s, e);
}

function OnPoundType_SelectedIndexChanged(s, e) {
    SetEventFromCmbPoundType(s, e);
}

function SetEventFromCmbPoundType(s, e) {
    let _valPoundsType = s.GetValue();
    let valProdCostSelected = ASPxClientControl.GetControlCollection().GetByName("productionCosting").GetValue();

    let _idProcessedPoundsSimpleFormula = ASPxClientControl.GetControlCollection().GetByName("idProcessedPoundsSimpleFormula");
    let _idFinishedPoundsSimpleFormula = ASPxClientControl.GetControlCollection().GetByName("idFinishedPoundsSimpleFormula");

    if (valProdCostSelected == "SI") {
        if (_valPoundsType == "AMBAS" || _valPoundsType == "LIBPRO") {
            _idProcessedPoundsSimpleFormula.SetEnabled(true);
        } else {
            _idProcessedPoundsSimpleFormula.SetEnabled(false);
        }
        if (_valPoundsType == "AMBAS" || _valPoundsType == "LIBTERM") {
            _idFinishedPoundsSimpleFormula.SetEnabled(true);
        } else {
            _idFinishedPoundsSimpleFormula.SetEnabled(false);
        }
    }
}


function OnFormulaPoundsProcessed_SelectedIndexChanged(s, e) {
}

function OnFormulaPoundsFinished_SelectedIndexChanged(s, e) {
}

function OnReasonCosts_SelectedIndexChanged(s, e) {

}
