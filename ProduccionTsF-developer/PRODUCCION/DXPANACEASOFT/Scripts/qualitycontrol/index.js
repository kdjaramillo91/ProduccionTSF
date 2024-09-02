// DETAILS VIEW CALLBACKS


function QualityControlDetailView_OnBeginCallback(s, e) {
    e.customArgs["id_qualityControl"] = gvQualityControlDetails.cpIdQualityControl;

}

function onInitTotal(s, e) {
    if (s.name === "1gvQualityControlDetailsDetail2") {
        var valorTotal = s.cpValorTotal;
        totalWeightSampleNonEditable.SetValue(0);
        if (valorTotal !== null && valorTotal > 0) {
            totalWeightSampleNonEditable.SetValue(valorTotal);
        }
    }
}

//Validation 



function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

function OnBeginGridCallback(s, e) {
     
    e.customArgs = MVCxClientUtils.GetSerializedEditorValuesInContainer("options");
}


function OnRangeStartDateValidation(s, e) {
    OnRangeDateValidation(e, startStartDate.GetValue(), endStartDate.GetValue(), "Rango de Fecha no válido");
}

function OnRangeEndDateValidation(s, e) {
    OnRangeDateValidation(e, startEndDate.GetValue(), endEndDate.GetValue(), "Rango de Fecha no válido");
}


// FILTER FORM BUTTONS

function btnSearch_click(s, e) {
    var data = $("#formFilterQualityControl").serialize();

    if (data !== null) {
        $.ajax({
            url: "QualityControl/QualityControlResults",
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

function btnClear_click(s, e) {
    id_qualityControlConfiguration.SetValue(null);
    id_documentState.SetValue(null);
    qualityControlNumber.SetText("");
    documentReference.SetText("");

    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    id_analyst.SetValue(null);
    isConforms.SetChecked(true);
    contactPerson.SetText("");
    id_executivePerson.SetValue(null);
    id_executivePerson.SetText("");
    id_businessOportunityState.SetValue(null);
    id_businessOportunityState.SetText("");
}

function AddNewQualityControl(s, e) {
    var data = {
        id: 0
    };
    showPage("QualityControl/FormEditQualityControl", data);
}

function AddNewQualityControlFromRMP(s, e) {
    $.ajax({
        url: "QualityControl/ProductionLotDetailsResults",
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

// GRIDVIEW BUSINESS OPORTUNITIES RESULTS ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvBusinessOportunities.GetSelectedFieldValues("id", function (values) {

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
                gvBusinessOportunities.PerformCallback();
                // gvBusinessOportunities.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    AddNewQualityControl(s, e);
}

function CopyDocument(s, e) {
}

function ApproveDocuments(s, e) {
}

function AutorizeDocuments(s, e) {
}

function ProtectDocuments(s, e) {
}

function CancelDocuments(s, e) {
}

function RevertDocuments(s, e) {
}

function ShowHistory(s, e) {

}

function Print(s, e) {

}

function QualityControlGridViewCustomCommandButton_Click(s, e) {
    // 
    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvQualityControls.GetRowKey(e.visibleIndex)
        };
        showPage("QualityControl/FormEditQualityControl", data);
        
    }
}

function BusinessOportunity_OnBeginCallback(s, e) {
    e.customArgs['id_businessOportunity'] = $("#id_businessOportunity").val();
}

// GRIDVIEW QUALITY CONTROL SELECTION

function QualityControlOnRowDoubleClick(s, e) {
    //s.GetRowValues(e.visibleIndex, "id", function (value) {
    //    showPage("PurchaseOrder/FormEditPurchaseOrder", { id: value });
    //});
}

function QualityControlOnGridViewInit(s, e) {
    UpdateTitlePanelQualityControl();
}

function QualityControlOnGridViewSelectionChanged(s, e) {
    UpdateTitlePanelQualityControl();
}

function QualityControlOnGridViewEndCallback() {
    UpdateTitlePanelQualityControl();
}

function UpdateTitlePanelQualityControl() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCountQualityControl();

    var text = "Total de elementos seleccionados: <b>" + gvQualityControls.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvQualityControls.GetSelectedRowCount() - GetSelectedFilteredRowCountQualityControl();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRowsQualityControl", gvQualityControls.GetSelectedRowCount() > 0 && gvQualityControls.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelectionQualityControl", gvQualityControls.GetSelectedRowCount() > 0);
    //}

    btnCopy.SetEnabled(false);
    btnApprove.SetEnabled(false);
    btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    //btnHistory.SetEnabled(gvQualityControls.GetSelectedRowCount() === 1);
    btnPrint.SetVisible(false);

}

function GetSelectedFilteredRowCountQualityControl() {
    return gvQualityControls.cpFilteredRowCountWithoutPage + gvQualityControls.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function QualityControlSelectAllRows() {
    gvQualityControls.SelectRows();
}

function QualityControlClearSelection() {
    gvQualityControls.UnselectRows();
}

//NEW FUNCTIONS
function InsertQualityRMPAnalPorLoteMultiplesGuias() {
    showLoading();
    gvProductionLotDetails.GetSelectedFieldValues("id", function (values) {
        hideLoading();
        if (values == undefined || values == null) {
            NotifyError("Error. Debe seleccionar al menos 1 registro.");
            return;
        }
        if (values.length == 0) {
            NotifyError("Error. Debe seleccionar al menos 1 registro.");
            return;
        }
        
        $.ajax({
            url: "QualityControl/ValidateParamAnalisisPorLote",
            type: "post",
            async: true,
            data: { arr_id_productionLotDetail: values},
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result == undefined) {
                    NotifyError("Error. Se produjo un error en el aplicativo.");
                    return;
                }
                if (result.tieneError == "NO") {
                    if (result.tieneAdvertencia == "SI") {
                        showConfirmationDialog(function () {
                            InsertQualityRMP();
                        }, result.mensaje);
                    } else {
                        InsertQualityRMP();
                    }
                } else if (result.tieneError == "SI") {
                    NotifyError("Error. " + result.mensaje);
                    return;
                } else {
                    NotifyError("Error. Se produjo un error en el aplicativo.");
                    return;
                }
            },
            complete: function () {
                hideLoading();
            }
        });

    });
}


function InsertQualityRMP() {
    showLoading();
    gvProductionLotDetails.GetSelectedFieldValues("id", function (values) {
        hideLoading();
        var paramAnalisisXLote = $("#PARAMANALXLOT").val();
        // 
        if (values[0] != 0 && values[0] != null && values[0] != undefined) {
            var data2 = {
                id: 0,
                id_productionLotDetail: parseInt(values[0]),
                paramAnalisisPorLote: paramAnalisisXLote,
                arr_id_productionLotDetail: values
            };
            showPage("QualityControl/FormEditQualityControl", data2);            
        }
    });
}
//GRIDEVIEW 
function ProductionLotDetailsOnGridViewInit(s, e) {
}
function ProductionLotDetailsOnGridViewSelectionChanged(s, e) {
}
function ProductionLotDetailsOnGridViewEndCallback(s, e) {
}

// MAIN FUNCTIONS

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

//EDIT BATCH
