// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideRiverInternControlViatic").serialize();
     
    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiverInternControlViatic/RemissionGuideRiverInternControlViaticResults",
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
    event.preventDefault();
}

function btnClear_click() {
    id_documentState.SetSelectedItem(null);
    number.SetText("");
    reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    startAuthorizationDate.SetDate(null);
    endAuthorizationDate.SetDate(null);
    authorizationNumber.SetText("");
    accessKey.SetText("");

    startDespachureDate.SetDate(null);
    endDespachureDate.SetDate(null);
    startArrivalDate.SetDate(null);
    endArrivalDate.SetDate(null);
    startReturnDate.SetDate(null);
    endReturnDate.SetDate(null);
}

//// GRIDVIEW RESULT ACTIONS BUTTONS

function AddNewDocument(s, e) {
    //AddNewGuideRemissionManual(s, e);
}

function CopyDocument(s, e) {
    gvRemisssionGuideRiverInternControlViatic.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideRiverInternControlViatic/RemissionGuideRiverControlVehicleCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControlViatic/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControlViatic/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControlViatic/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControlViatic/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControlViatic/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
   
    gvRemisssionGuideRiverInternControlViatic.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideRiverInternControlViatic/RemissionGuideRiverReport",
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

//// REMISSION GUIDES RESULT GRIDVIEW EDIT ACTION

function GridViewRemissionGuideRiverInternControlViaticCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideRiverInternControlViatic.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideRiverInternControlViatic/FormEditRemissionGuideRiverInternControlViatic", data);
    }
}

function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
}

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuideRiverInternControlViatic.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuideRiverInternControlViatic.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuideRiverInternControlViatic.GetSelectedRowCount() > 0 && gvRemisssionGuideRiverInternControlViatic.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuideRiverInternControlViatic.GetSelectedRowCount() > 0);


    //btnCopy.SetEnabled(false);
    //btnApprove.SetEnabled(false);
    //btnAutorize.SetEnabled(false);
    //btnProtect.SetEnabled(false);
    //btnCancel.SetEnabled(false);
    //btnRevert.SetEnabled(false);
    //btnHistory.SetEnabled(false);
    //btnPrint.SetEnabled(false);

}

function GetSelectedFilteredRowCount() {
    return gvRemisssionGuideRiverInternControlViatic.cpFilteredRowCountWithoutPage + gvRemisssionGuideRiverInternControlViatic.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideRiverInternControlViatic.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideRiverInternControlViatic.SelectRows();
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function AssignNewStateRGP() {
     
    $.ajax({
        url: "RemissionGuideRiverInternControlViatic/RemissionGuideRiverResults",
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

    event.preventDefault();
}

////ASIGNAR ESTADO DE PAGO -buscar inf
function UpdateTitlePanelRemissionGuideRiverDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredRemissionGuideRiverRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideRiverInternControlViatic2.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideRiverInternControlViatic2.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRiverRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideRiverInternControlViatic2.GetSelectedRowCount() > 0 && gvRemissionGuideRiverInternControlViatic2.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideRiverInternControlViatic2.GetSelectedRowCount() > 0);

    //btnCopy.SetEnabled(false);
    //btnApprove.SetEnabled(false);
    //btnAutorize.SetEnabled(false);
    //btnProtect.SetEnabled(false);
    //btnCancel.SetEnabled(false);
    //btnRevert.SetEnabled(false);
    //btnHistory.SetEnabled(false);
    //btnPrint.SetEnabled(false);
    //btnnew.SetEnabled(gvRemissionGuideRiverInternControlViatic2.GetSelectedRowCount() == 1);
    
    //btnAssignStatePaymentViatic.SetEnabled(gvRemissionGuideRiverInternControlViatic2.GetSelectedRowCount() == 1);
    
}

function OnGridViewRemissionGuideRiverControlVehicleInit(s, e) {
    UpdateTitlePanelRemissionGuideRiverDetails();
}

function OnGridViewRemissionGuideRiverControlVehicleSelectionChanged(s, e) {
    UpdateTitlePanelRemissionGuideRiverDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function GetSelectedFieldDetailValuesCallback(values) {
    selectedRemissionGuideRiverRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideRiverRows.push(values[i]);

    }
}

function OnGridViewRemissionGuideRiverControlVehicleEndCallback() {
    UpdateTitlePanelRemissionGuideRiverDetails();
}

function GetSelectedFilteredRemissionGuideRiverRowCount() {
    return gvRemissionGuideRiverInternControlViatic2.cpFilteredRowCountWithoutPage + gvRemissionGuideRiverInternControlViatic2.GetSelectedKeysOnPage().length;
}

function AssingStatePaymentViatic(s, e) {
    //showLoading();
    // 
    gvRemissionGuideRiverInternControlViatic2.GetSelectedFieldValues("id", function (values) {
        // 
        if (values != null) {
            if (values.length > 0){
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideRiverInternControlViatic/FormEditRemissionGuideRiverInternControlViatic", data);
                }
            }
        }
    });
}

function PRViaticFluvial(s, e) {
    var codeReport = "LGRVF";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideRiverInternControlViatic").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiverInternControlViatic/PRViaticFluvial",
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

function PRAdvanceFluvial(s, e) {
    var codeReport = "RAGRFL";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideRiverInternControlViatic").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiverInternControlViatic/PRViaticFluvial",
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

function RemissionGuideRiverControlVehicleDetailViewAssignedStaff_BeginCallback(s, e) {
    // 
    e.customArgs["id_remissionGuideRiver"] = $("#id_remissionGuideRiver").val();
}