// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideRiverControlVehicleEntrance").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideRiverControlVehicleEntrance/RemissionGuideRiverControlVehicleEntranceResults",
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


function PerformDocumentAction(url) {
    gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedFieldValues("id", function (values) {

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
                gvRemisssionGuideRiverControlVehicleEntrance.PerformCallback();
                gvRemisssionGuideRiverControlVehicleEntrance.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    //AddNewGuideRemissionManual(s, e);
}

function CopyDocument(s, e) {
    gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideRiverControlVehicleEntrance/RemissionGuideRiverControlVehicleEntranceCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicleEntrance/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicleEntrance/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicleEntrance/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicleEntrance/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideRiverControlVehicleEntrance/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
    gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideRiverControlVehicleEntrance/RemissionGuideRiverReport",
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

function GridViewRemissionGuideRiverControlVehicleEntranceCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideRiverControlVehicleEntrance.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideRiverControlVehicleEntrance/FormEditRemissionGuideRiverControlVehicleEntrance", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0 && gvRemisssionGuideRiverControlVehicleEntrance.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0);


    //btnCopy.SetEnabled(gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedRowCount() == 1);

}

function GetSelectedFilteredRowCount() {
    return gvRemisssionGuideRiverControlVehicleEntrance.cpFilteredRowCountWithoutPage + gvRemisssionGuideRiverControlVehicleEntrance.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideRiverControlVehicleEntrance.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideRiverControlVehicleEntrance.SelectRows();
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

function AssignNewEntranceDatetimePlanct() {
    $.ajax({
        url: "RemissionGuideRiverControlVehicleEntrance/RemissionGuideRiverResults",
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
        url: "RemissionGuideRiverControlVehicleEntrance/PurchaseOrderDetailsResults",
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

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideRiverControlVehicleEntrance.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideRiverControlVehicleEntrance.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRiverRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0 && gvRemissionGuideRiverControlVehicleEntrance.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideRiverControlVehicleEntrance.GetSelectedRowCount() > 0);


    btnAssignDateTimeRemissionGuideRiverControlEntrance.SetEnabled(gvRemissionGuideRiverControlVehicleEntrance.GetSelectedRowCount() == 1);
}

function OnGridViewRemissionGuideRiverControlVehicleEntranceInit(s, e) {
    UpdateTitlePanelRemissionGuideRiverDetails();
}

function OnGridViewRemissionGuideRiverControlVehicleEntranceSelectionChanged(s, e) {
    UpdateTitlePanelRemissionGuideRiverDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

var selectedItemsRGCtmp = [];

function GetSelectedFieldDetailValuesCallback(values) {
     
    selectedRemissionGuideRiverRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideRiverRows.push(values[i]);
    }
}

function OnGridViewRemissionGuideRiverControlVehicleEntranceEndCallback() {
    UpdateTitlePanelRemissionGuideRiverDetails();
}

function GetSelectedFilteredRemissionGuideRiverRowCount() {
    return gvRemissionGuideRiverControlVehicleEntrance.cpFilteredRowCountWithoutPage + gvRemissionGuideRiverControlVehicleEntrance.GetSelectedKeysOnPage().length;
}

function GenerateRemissionGuideRiverControlVehicleEntrance(s, e) {
    console.log(selectedItemsRGCtmp);

    gvRemissionGuideRiverControlVehicleEntrance.GetSelectedFieldValues("id", function (values) {
        
        if (values != null) {
            if (values.length > 0) {
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideRiverControlVehicleEntrance/FormEditRemissionGuideRiverControlVehicleEntrance", data);
                }
            }
        }
    });
}

function RemissionGuideRiverControlVehicleEntranceSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_remissionGuideRiver"] = $("#id_remissionGuideRiver").val();
}
