
//Validations

function OnQualityAnalysisProductionLotQualityAnalysisDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnResultProductionLotQualityAnalysisDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function QualityAnalysisProductionLotQualityAnalysisDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionQualityAnalysisDetails",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            for (var i = 0; i < id_qualityAnalysis.GetItemCount() ; i++) {
                var qualityAnalysis = id_qualityAnalysis.GetItem(i);
                if (result.indexOf(qualityAnalysis.value) >= 0) {
                    id_qualityAnalysis.RemoveItem(i);
                    i = -1;
                }
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// EDITOR'S EVENTS

function OnGridViewQualityAnalysisDetailsInit(s, e) {
    UpdateTitlePanelQualityAnalysisDetails();
}

function UpdateTitlePanelQualityAnalysisDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountQualityAnalysisDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountQualityAnalysisDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoQualityAnalysiss").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsQualityAnalysiss", gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetSelectedRowCount() > 0 && gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionQualityAnalysiss", gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveQualityAnalysis.SetEnabled(gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountQualityAnalysisDetails() {
    return gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewQualityAnalysissDetailSelectionChanged(s, e) {
    UpdateTitlePanelQualityAnalysisDetails();
    s.GetSelectedFieldValues("id_qualityAnalysis", GetSelectedFieldValuesCallbackQualityAnalysissDetail);

}

function GetSelectedFieldValuesCallbackQualityAnalysissDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewQualityAnalysisDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewQualityAnalysisDetailsEndCallback(s, e) {
    UpdateTitlePanelQualityAnalysisDetails();
    if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
        s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
    }
    //if (gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
    //    if (gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
    //        if (s.GetEditor("id_QualityAnalysis") !== null && s.GetEditor("id_QualityAnalysis") !== undefined) {
    //            s.GetEditor("id_QualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
    //        }
    //    } else {
    //        if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
    //            s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
    //        }
    //    }

    //} else {
    //    if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
    //        s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
    //    }
    //}

    //UpdateProductionLotTotals();

}

function gvEditQualityAnalysisDetailsClearSelection() {
    gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.UnselectRows();
}

function gvEditQualityAnalysisDetailsSelectAllRows() {
    gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.SelectRows();
}


