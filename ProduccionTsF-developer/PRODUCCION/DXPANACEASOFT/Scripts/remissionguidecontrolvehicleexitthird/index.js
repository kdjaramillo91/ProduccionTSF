// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideControlVehicleExitThird").serialize();

    if (data != null) {
        $.ajax({
            url: "RemissionGuideControlVehicleExitThird/RemissionGuideControlVehicleExitThirdResults",
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
    gvRemisssionGuideControlVehicleExitThird.GetSelectedFieldValues("id", function (values) {

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
    gvRemisssionGuideControlVehicleExitThird.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideControlVehicleExitThird/RemissionGuideControlVehicleExitThirdCopy", { id: values[0] });
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
    gvRemisssionGuideControlVehicleExitThird.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideControlVehicleExitThird/RemissionGuideReport",
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

function GridViewRemissionGuideControlVehicleExitThirdCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemisssionGuideControlVehicleExitThird.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideControlVehicleExitThird/FormEditRemissionGuideControlVehicleExitThird", data);
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

}

function GetSelectedFilteredRowCount() {
    return gvRemisssionGuideControlVehicleExitThird.cpFilteredRowCountWithoutPage + gvRemisssionGuideControlVehicleExitThird.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemisssionGuideControlVehicleExitThird.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemisssionGuideControlVehicleExitThird.SelectRows();
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
        url: "RemissionGuideControlVehicleExitThird/RemissionGuideResults",
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
        url: "RemissionGuideControlVehicleExitThird/PurchaseOrderDetailsResults",
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

    btnAssignDateTimeRemissionGuideControlExitThird.SetEnabled(gvRemissionGuideControlVehicleExitThird.GetSelectedRowCount() == 1);
}

function OnGridViewRemissionGuideControlVehicleExitThirdInit(s, e) {
    UpdateTitlePanelRemissionGuideDetails();
}

function OnGridViewRemissionGuideControlVehicleExitThirdSelectionChanged(s, e) {
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

function OnGridViewRemissionGuideControlVehicleExitThirdEndCallback() {
    UpdateTitlePanelRemissionGuideDetails();
}

function GetSelectedFilteredRemissionGuideRowCount() {
    return gvRemissionGuideControlVehicleExitThird.cpFilteredRowCountWithoutPage + gvRemissionGuideControlVehicleExitThird.GetSelectedKeysOnPage().length;
}

function GenerateRemissionGuideControlVehicleExitThird(s, e) {
    console.log(selectedItemsRGCtmp);

    gvRemissionGuideControlVehicleExitThird.GetSelectedFieldValues("id", function (values) {
        
        if (values != null) {
            if (values.length > 0) {
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideControlVehicleExitThird/FormEditRemissionGuideControlVehicleExitThird", data);
                }
            }
        }
    });
}

function RemissionGuideControlVehicleExitThirdSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}
