// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideInternControl").serialize();
     
    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControl/RemissionGuideInternControlResults",
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
    gvRemisssionGuideInternControl.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideInternControl/RemissionGuideControlVehicleCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControl/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControl/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControl/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControl/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControl/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
   
    gvRemisssionGuideInternControl.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideInternControl/RemissionGuideReport",
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

function GridViewRemissionGuideInternControlCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideInternControl.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideInternControl/FormEditRemissionGuideInternControl", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuideInternControl.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuideInternControl.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuideInternControl.GetSelectedRowCount() > 0 && gvRemisssionGuideInternControl.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuideInternControl.GetSelectedRowCount() > 0);


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
    return gvRemisssionGuideInternControl.cpFilteredRowCountWithoutPage + gvRemisssionGuideInternControl.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideInternControl.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideInternControl.SelectRows();
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
        url: "RemissionGuideInternControl/RemissionGuideResults",
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
function UpdateTitlePanelRemissionGuideDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredRemissionGuideRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideInternControl2.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideInternControl2.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideInternControl2.GetSelectedRowCount() > 0 && gvRemissionGuideInternControl2.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideInternControl2.GetSelectedRowCount() > 0);

    //btnCopy.SetEnabled(false);
    //btnApprove.SetEnabled(false);
    //btnAutorize.SetEnabled(false);
    //btnProtect.SetEnabled(false);
    //btnCancel.SetEnabled(false);
    //btnRevert.SetEnabled(false);
    //btnHistory.SetEnabled(false);
    //btnPrint.SetEnabled(false);
    //btnnew.SetEnabled(gvRemissionGuideInternControl2.GetSelectedRowCount() == 1);
    
    //btnAssignStatePaymentViatic.SetEnabled(gvRemissionGuideInternControl2.GetSelectedRowCount() == 1);
    
}

function OnGridViewRemissionGuideControlVehicleInit(s, e) {
    UpdateTitlePanelRemissionGuideDetails();
}

function OnGridViewRemissionGuideControlVehicleSelectionChanged(s, e) {
    UpdateTitlePanelRemissionGuideDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function GetSelectedFieldDetailValuesCallback(values) {
    selectedRemissionGuideRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideRows.push(values[i]);

    }
}

function OnGridViewRemissionGuideControlVehicleEndCallback() {
    UpdateTitlePanelRemissionGuideDetails();
}

function GetSelectedFilteredRemissionGuideRowCount() {
    return gvRemissionGuideInternControl2.cpFilteredRowCountWithoutPage + gvRemissionGuideInternControl2.GetSelectedKeysOnPage().length;
}

function AssingStatePaymentViatic(s, e) {
    //showLoading();
    // 
    gvRemissionGuideInternControl2.GetSelectedFieldValues("id", function (values) {
        // 
        if (values != null) {
            if (values.length > 0){
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideInternControl/FormEditRemissionGuideInternControl", data);
                }
            }
        }
    });
}

function PRViaticTerrestrel(s, e) {
    var codeReport = "LGRVT";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideInternControl").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControl/PRViaticTerrestrel",
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

function PRViaticTerrestrelToExcel(s, e) {
    var codeReport = "LGRVT";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideInternControl").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControl/PRViaticTerrestrel",
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
                        var url = 'ReportProd/ToExcel?trepd=' + reportTdr;
                        newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0', false);
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

function PRAdvanceTerrestrel(s, e) {
    var codeReport = "RAGRTL";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideInternControl").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControl/PRViaticTerrestrel",
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

function PRAdvanceTerrestrelToExcel(s, e) {
    var codeReport = "RAGRTL";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideInternControl").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControl/PRViaticTerrestrel",
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
                        var url = 'ReportProd/DownLoadExcelRemisionGuideTerrestreViatic';
                        newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0', false);
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

function RemissionGuideControlVehicleDetailViewAssignedStaff_BeginCallback(s, e) {
    // 
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}