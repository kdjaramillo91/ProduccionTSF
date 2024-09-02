// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideControlVehicle").serialize();
     
    if (data != null) {
        $.ajax({
            url: "RemissionGuideControlVehicle/RemissionGuideControlVehicleResults",
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

//function AssignNewExitDatetimePlanct() {
//     
//    var data = {
//        id: 0,
//        requestDetails: []
//    };

//    showPage("RemissionGuideControlVehicle/FormEditRemissionGuideControlVehicle", data);
//}

//function AssignModifyExitDatetimePlanct() {
//    $.ajax({
//        url: "RemissionGuideControlVehicle/PurchaseOrderDetailsResults",
//        type: "post",
//        async: true,
//        cache: false,
//        error: function (error) {
//            console.log(error);
//        },
//        beforeSend: function () {
//            showLoading();
//        },
//        success: function (result) {
//            $("#btnCollapse").click();
//            $("#results").html(result);
//        },
//        complete: function () {
//            hideLoading();
//        }
//    });

//    event.preventDefault();
//}

// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvRemisssionGuideControlVehicle.GetSelectedFieldValues("id", function (values) {

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
                gvRemisssionGuide.PerformCallback();
                gvRemisssionGuide.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    //AddNewGuideRemissionManual(s, e);
}

function CopyDocument(s, e) {
    gvRemisssionGuideControlVehicle.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideControlVehicle/RemissionGuideControlVehicleCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicle/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicle/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicle/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicle/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicle/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
    gvRemisssionGuideControlVehicle.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideControlVehicle/RemissionGuideReport",
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
// REMISSION GUIDES RESULT GRIDVIEW EDIT ACTION

function GridViewRemissionGuideControlVehicleCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideControlVehicle.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideControlVehicle/FormEditRemissionGuideControlVehicle", data);
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

$(function () {
    init();
});

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuideControlVehicle.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuideControlVehicle.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuideControlVehicle.GetSelectedRowCount() > 0 && gvRemisssionGuideControlVehicle.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuideControlVehicle.GetSelectedRowCount() > 0);


    //btnCopy.SetEnabled(gvRemisssionGuideControlVehicle.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvRemisssionGuideControlVehicle.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvRemisssionGuideControlVehicle.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvRemisssionGuideControlVehicle.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvRemisssionGuideControlVehicle.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvRemisssionGuideControlVehicle.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvRemisssionGuideControlVehicle.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvRemisssionGuideControlVehicle.GetSelectedRowCount() == 1);

}

function GetSelectedFilteredRowCount() {
    return gvRemisssionGuideControlVehicle.cpFilteredRowCountWithoutPage + gvRemisssionGuideControlVehicle.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideControlVehicle.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideControlVehicle.SelectRows();
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

function AssignNewExitDatetimePlanct() {
     
    $.ajax({
        url: "RemissionGuideControlVehicle/RemissionGuideResults",
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

function AssignModifyExitDatetimePlanct() {
     
    $.ajax({
        url: "RemissionGuideControlVehicle/PurchaseOrderDetailsResults",
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

//ASIGNAR FECHA Y HORA GUIA DE REMISION
function UpdateTitlePanelRemissionGuideDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredRemissionGuideRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideControlVehicle.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideControlVehicle.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideControlVehicle.GetSelectedRowCount() > 0 && gvRemissionGuideControlVehicle.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideControlVehicle.GetSelectedRowCount() > 0);


    btnAssignDateTimeRemissionGuideControl.SetEnabled(gvRemissionGuideControlVehicle.GetSelectedRowCount() == 1);
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
    return gvRemissionGuideControlVehicle.cpFilteredRowCountWithoutPage + gvRemissionGuideControlVehicle.GetSelectedKeysOnPage().length;
}

function GenerateRemissionGuideControlVehicleExit(s, e) {
    //showLoading();

    gvRemissionGuideControlVehicle.GetSelectedFieldValues("id", function (values) {
         
        if (values != null) {
            if (values.length > 0){
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideControlVehicle/FormEditRemissionGuideControlVehicle", data);
                }
            }
        }
    });
}

function RemissionGuideControlVehicleSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}

function RemissionGuideControlVehicleDetailViewAssignedStaff_BeginCallback(s, e) {
    // 
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}