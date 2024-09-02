// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideInternControlViatic").serialize();
     
    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControlViatic/RemissionGuideInternControlViaticResults",
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
    gvRemisssionGuideInternControlViatic.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideInternControlViatic/RemissionGuideControlVehicleCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControlViatic/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControlViatic/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControlViatic/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControlViatic/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideInternControlViatic/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
   
    gvRemisssionGuideInternControlViatic.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideInternControlViatic/RemissionGuideReport",
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

function GridViewRemissionGuideInternControlViaticCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideInternControlViatic.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideInternControlViatic/FormEditRemissionGuideInternControlViatic", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuideInternControlViatic.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuideInternControlViatic.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuideInternControlViatic.GetSelectedRowCount() > 0 && gvRemisssionGuideInternControlViatic.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuideInternControlViatic.GetSelectedRowCount() > 0);


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
    return gvRemisssionGuideInternControlViatic.cpFilteredRowCountWithoutPage + gvRemisssionGuideInternControlViatic.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideInternControlViatic.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideInternControlViatic.SelectRows();
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
        url: "RemissionGuideInternControlViatic/RemissionGuideResults",
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

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideInternControlViatic2.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideInternControlViatic2.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideInternControlViatic2.GetSelectedRowCount() > 0 && gvRemissionGuideInternControlViatic2.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideInternControlViatic2.GetSelectedRowCount() > 0);

    //btnCopy.SetEnabled(false);
    //btnApprove.SetEnabled(false);
    //btnAutorize.SetEnabled(false);
    //btnProtect.SetEnabled(false);
    //btnCancel.SetEnabled(false);
    //btnRevert.SetEnabled(false);
    //btnHistory.SetEnabled(false);
    //btnPrint.SetEnabled(false);
    //btnnew.SetEnabled(gvRemissionGuideInternControlViatic2.GetSelectedRowCount() == 1);
    
    //btnAssignStatePaymentViatic.SetEnabled(gvRemissionGuideInternControlViatic2.GetSelectedRowCount() == 1);
    
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
    return gvRemissionGuideInternControlViatic2.cpFilteredRowCountWithoutPage + gvRemissionGuideInternControlViatic2.GetSelectedKeysOnPage().length;
}

function AssingStatePaymentViatic(s, e) {
    //showLoading();
    // 
    gvRemissionGuideInternControlViatic2.GetSelectedFieldValues("id", function (values) {
        // 
        if (values != null) {
            if (values.length > 0){
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideInternControlViatic/FormEditRemissionGuideInternControlViatic", data);
                }
            }
        }
    });
}

function PRViaticTerrestrel(s, e) {
    var codeReport = "LGRVT";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideInternControlViatic").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControlViatic/PRViaticTerrestrel",
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
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideInternControlViatic").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControlViatic/PRViaticTerrestrel",
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
                        var url = 'ReportProd/DownLoadExcelRemisionGuideViaticTerrestre';
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
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideInternControlViatic").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControlViatic/PRViaticTerrestrel",
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
    var data = "codeReport=" + codeReport + "&" + $("#formFilterRemissionGuideInternControlViatic").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideInternControlViatic/PRViaticTerrestrel",
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

function RemissionGuideControlVehicleDetailViewAssignedStaff_BeginCallback(s, e) {
    // 
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}