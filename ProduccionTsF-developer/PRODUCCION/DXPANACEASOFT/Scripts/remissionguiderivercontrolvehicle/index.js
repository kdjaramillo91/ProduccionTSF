// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideRiverControlVehicle").serialize();
     
    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiverControlVehicle/RemissionGuideRiverControlVehicleResults",
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

//    showPage("RemissionGuideRiverControlVehicle/FormEditRemissionGuideRiverControlVehicle", data);
//}

//function AssignModifyExitDatetimePlanct() {
//    $.ajax({
//        url: "RemissionGuideRiverControlVehicle/PurchaseOrderDetailsResults",
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
    gvRemisssionGuideRiverControlVehicle.GetSelectedFieldValues("id", function (values) {

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
                gvRemisssionGuideRiver.PerformCallback();
                gvRemisssionGuideRiver.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    //AddNewGuideRemissionManual(s, e);
}

function CopyDocument(s, e) {
    gvRemisssionGuideRiverControlVehicle.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideRiverControlVehicle/RemissionGuideRiverControlVehicleCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicle/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicle/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicle/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicle/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicle/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
    gvRemisssionGuideRiverControlVehicle.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideRiverControlVehicle/RemissionGuideRiverReport",
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

function GridViewRemissionGuideRiverControlVehicleCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideRiverControlVehicle.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideRiverControlVehicle/FormEditRemissionGuideRiverControlVehicle", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() > 0 && gvRemisssionGuideRiverControlVehicle.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() > 0);


    //btnCopy.SetEnabled(gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvRemisssionGuideRiverControlVehicle.GetSelectedRowCount() == 1);

}

function GetSelectedFilteredRowCount() {
    return gvRemisssionGuideRiverControlVehicle.cpFilteredRowCountWithoutPage + gvRemisssionGuideRiverControlVehicle.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideRiverControlVehicle.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideRiverControlVehicle.SelectRows();
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
        url: "RemissionGuideRiverControlVehicle/RemissionGuideRiverResults",
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
        url: "RemissionGuideRiverControlVehicle/PurchaseOrderDetailsResults",
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
function UpdateTitlePanelRemissionGuideRiverDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredRemissionGuideRiverRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideRiverControlVehicle.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideRiverControlVehicle.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRiverRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideRiverControlVehicle.GetSelectedRowCount() > 0 && gvRemissionGuideRiverControlVehicle.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideRiverControlVehicle.GetSelectedRowCount() > 0);


    btnAssignDateTimeRemissionGuideRiverControl.SetEnabled(gvRemissionGuideRiverControlVehicle.GetSelectedRowCount() == 1);
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
    return gvRemissionGuideRiverControlVehicle.cpFilteredRowCountWithoutPage + gvRemissionGuideRiverControlVehicle.GetSelectedKeysOnPage().length;
}

function GenerateRemissionGuideRiverControlVehicleExit(s, e) {
    //showLoading();

    gvRemissionGuideRiverControlVehicle.GetSelectedFieldValues("id", function (values) {
         
        if (values != null) {
            if (values.length > 0){
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideRiverControlVehicle/FormEditRemissionGuideRiverControlVehicle", data);
                }
            }
        }
    });
}

function RemissionGuideRiverControlVehicleSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_RemissionGuideRiver"] = $("#id_RemissionGuideRiver").val();
}

function RemissionGuideRiverControlVehicleDetailViewAssignedStaff_BeginCallback(s, e) {
    // 
    e.customArgs["id_RemissionGuideRiver"] = $("#id_RemissionGuideRiver").val();
}