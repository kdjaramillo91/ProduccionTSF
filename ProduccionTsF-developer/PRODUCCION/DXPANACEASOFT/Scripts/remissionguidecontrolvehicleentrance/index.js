// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideControlVehicleEntrance").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideControlVehicleEntrance/RemissionGuideControlVehicleEntranceResults",
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
    gvRemisssionGuideControlVehicleEntrance.GetSelectedFieldValues("id", function (values) {

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
                gvRemisssionGuideControlVehicleEntrance.PerformCallback();
                gvRemisssionGuideControlVehicleEntrance.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    //AddNewGuideRemissionManual(s, e);
}

function CopyDocument(s, e) {
    gvRemisssionGuideControlVehicleEntrance.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideControlVehicleEntrance/RemissionGuideControlVehicleEntranceCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntrance/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntrance/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntrance/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntrance/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntrance/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
    gvRemisssionGuideControlVehicleEntrance.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideControlVehicleEntrance/RemissionGuideReport",
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

function GridViewRemissionGuideControlVehicleEntranceCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideControlVehicleEntrance.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideControlVehicleEntrance/FormEditRemissionGuideControlVehicleEntrance", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() > 0 && gvRemisssionGuideControlVehicleEntrance.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() > 0);


    //btnCopy.SetEnabled(gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvRemisssionGuideControlVehicleEntrance.GetSelectedRowCount() == 1);

}

function GetSelectedFilteredRowCount() {
    return gvRemisssionGuideControlVehicleEntrance.cpFilteredRowCountWithoutPage + gvRemisssionGuideControlVehicleEntrance.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideControlVehicleEntrance.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideControlVehicleEntrance.SelectRows();
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
        url: "RemissionGuideControlVehicleEntrance/RemissionGuideResults",
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
        url: "RemissionGuideControlVehicleEntrance/PurchaseOrderDetailsResults",
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

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideControlVehicleEntrance.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideControlVehicleEntrance.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideControlVehicleEntrance.GetSelectedRowCount() > 0 && gvRemissionGuideControlVehicleEntrance.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideControlVehicleEntrance.GetSelectedRowCount() > 0);

    // 
    btnAssignDateTimeRemissionGuideControlEntrance.SetEnabled(gvRemissionGuideControlVehicleEntrance.GetSelectedKeysOnPage() > 0);
}

function OnGridViewRemissionGuideControlVehicleEntranceInit(s, e) {
    UpdateTitlePanelRemissionGuideDetails();
}

function OnGridViewRemissionGuideControlVehicleEntranceSelectionChanged(s, e) {
    UpdateTitlePanelRemissionGuideDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

var selectedItemsRGCtmp = [];

function GetSelectedFieldDetailValuesCallback(values) {
     
    selectedRemissionGuideRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideRows.push(values[i]);
    }
}

function OnGridViewRemissionGuideControlVehicleEntranceEndCallback() {
    UpdateTitlePanelRemissionGuideDetails();
}

function GetSelectedFilteredRemissionGuideRowCount() {
    return gvRemissionGuideControlVehicleEntrance.cpFilteredRowCountWithoutPage + gvRemissionGuideControlVehicleEntrance.GetSelectedKeysOnPage().length;
}

function GenerateRemissionGuideControlVehicleEntrance(s, e) {
    console.log(selectedItemsRGCtmp);

    gvRemissionGuideControlVehicleEntrance.GetSelectedFieldValues("id", function (values) {
        
        if (values != null) {
            if (values.length > 0) {
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideControlVehicleEntrance/FormEditRemissionGuideControlVehicleEntrance", data);
                }
            }
        }
    });
}

function RemissionGuideControlVehicleEntranceSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}
