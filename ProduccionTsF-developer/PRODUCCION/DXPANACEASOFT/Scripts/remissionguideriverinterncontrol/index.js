// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideRiverInternControl").serialize();
     
    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiverInternControl/RemissionGuideRiverInternControlResults",
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
    gvRemisssionGuideRiverInternControl.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideRiverInternControl/RemissionGuideRiverControlVehicleCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControl/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControl/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControl/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControl/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverInternControl/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
   
    gvRemisssionGuideRiverInternControl.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideRiverInternControl/RemissionGuideRiverReport",
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

function GridViewRemissionGuideRiverInternControlCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideRiverInternControl.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideRiverInternControl/FormEditRemissionGuideRiverInternControl", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuideRiverInternControl.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuideRiverInternControl.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuideRiverInternControl.GetSelectedRowCount() > 0 && gvRemisssionGuideRiverInternControl.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuideRiverInternControl.GetSelectedRowCount() > 0);


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
    return gvRemisssionGuideRiverInternControl.cpFilteredRowCountWithoutPage + gvRemisssionGuideRiverInternControl.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideRiverInternControl.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideRiverInternControl.SelectRows();
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
        url: "RemissionGuideRiverInternControl/RemissionGuideRiverResults",
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

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideRiverInternControl2.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideRiverInternControl2.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRiverRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideRiverInternControl2.GetSelectedRowCount() > 0 && gvRemissionGuideRiverInternControl2.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideRiverInternControl2.GetSelectedRowCount() > 0);

    //btnCopy.SetEnabled(false);
    //btnApprove.SetEnabled(false);
    //btnAutorize.SetEnabled(false);
    //btnProtect.SetEnabled(false);
    //btnCancel.SetEnabled(false);
    //btnRevert.SetEnabled(false);
    //btnHistory.SetEnabled(false);
    //btnPrint.SetEnabled(false);
    //btnnew.SetEnabled(gvRemissionGuideRiverInternControl2.GetSelectedRowCount() == 1);
    
    //btnAssignStatePaymentViatic.SetEnabled(gvRemissionGuideRiverInternControl2.GetSelectedRowCount() == 1);
    
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
    return gvRemissionGuideRiverInternControl2.cpFilteredRowCountWithoutPage + gvRemissionGuideRiverInternControl2.GetSelectedKeysOnPage().length;
}

function AssingStatePaymentViatic(s, e) {
    //showLoading();
    // 
    gvRemissionGuideRiverInternControl2.GetSelectedFieldValues("id", function (values) {
        // 
        if (values != null) {
            if (values.length > 0){
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideRiverInternControl/FormEditRemissionGuideRiverInternControl", data);
                }
            }
        }
    });
}

function PRViaticFluvial(s, e) {
    var codeReport = "LGRVF";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideRiverInternControl").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiverInternControl/PRViaticFluvial",
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
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideRiverInternControl").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiverInternControl/PRViaticFluvial",
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