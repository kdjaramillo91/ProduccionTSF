// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    var data = $("#formFilterRemissionGuideControlVehicleEntranceThird").serialize();
     
    if (data != null) {
        $.ajax({
            url: "RemissionGuideControlVehicleEntranceThird/RemissionGuideControlVehicleEntranceThirdResults",
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

// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvRemissionGuideControlVehicleEntranceThird.GetSelectedFieldValues("id", function (values) {

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
                gvRemissionGuideControlVehicleEntranceThird.PerformCallback();
                gvRemissionGuideControlVehicleEntranceThird.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    //AddNewGuideRemissionManual(s, e);
}

function CopyDocument(s, e) {
    gvRemissionGuideControlVehicleEntranceThird.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("RemissionGuideControlVehicleEntranceThird/RemissionGuideControlVehicleEntranceThirdCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntranceThird/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntranceThird/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntranceThird/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntranceThird/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("RemissionGuideControlVehicleEntranceThird/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
    gvRemissionGuideControlVehicleEntranceThird.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "RemissionGuideControlVehicleEntranceThird/RemissionGuideReport",
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

function GridViewRemissionGuideControlVehicleEntranceThirdCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvRemissionGuideControlVehicleEntranceThird.GetRowKey(e.visibleIndex)
        };
        showPage("RemissionGuideControlVehicleEntranceThird/FormEditRemissionGuideControlVehicleEntranceThird", data);
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
    // 
    //var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    //var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() + "</b>";
    //var hiddenSelectedRowCount = gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    //if (hiddenSelectedRowCount > 0)
    //    text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    //text += "<br />";
    //$("#lblInfo").html(text);

    //SetElementVisibility("lnkSelectAllRows", gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() > 0 && gvRemissionGuideControlVehicleEntranceThird.cpVisibleRowCount > selectedFilteredRowCount);
    //SetElementVisibility("lnkClearSelection", gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() > 0);


}

function GetSelectedFilteredRowCount() {
    // 
    return gvRemissionGuideControlVehicleEntranceThird.cpFilteredRowCountWithoutPage + gvRemissionGuideControlVehicleEntranceThird.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvRemissionGuideControlVehicleEntranceThird.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvRemissionGuideControlVehicleEntranceThird.SelectRows();
}

function OnGridViewEndCallback() {
    // 
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    // 
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
        url: "RemissionGuideControlVehicleEntranceThird/RemissionGuideResults",
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

function AssignModifyEntranceDatetimePlanct() {
     
    $.ajax({
        url: "RemissionGuideControlVehicleEntranceThird/PurchaseOrderDetailsResults",
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
    //var selectedFilteredRowCount = GetSelectedFilteredRemissionGuideRowCount();

    //var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() + "</b>";
    //var hiddenSelectedRowCount = gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() - GetSelectedFilteredRemissionGuideRowCount();
    //if (hiddenSelectedRowCount > 0)
    //    text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    //text += "<br />";
    //$("#lblInfo").html(text);
    //SetElementVisibility("lnkSelectAllRows", gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() > 0 && gvRemissionGuideControlVehicleEntranceThird.cpVisibleRowCount > selectedFilteredRowCount);
    //SetElementVisibility("lnkClearSelection", gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() > 0);


    btnAssignDateTimeRemissionGuideControl.SetEnabled(gvRemissionGuideControlVehicleEntranceThird.GetSelectedRowCount() == 1);
}

function OnGridViewRemissionGuideControlVehicleEntranceThirdInit(s, e) {
    UpdateTitlePanelRemissionGuideDetails();
}

function OnGridViewRemissionGuideControlVehicleEntranceThirdSelectionChanged(s, e) {
    // 
    UpdateTitlePanelRemissionGuideDetails();
    //s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function GetSelectedFieldDetailValuesCallback(values) {
    selectedRemissionGuideRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideRows.push(values[i]);

    }
}

function OnGridViewRemissionGuideControlVehicleEntranceThirdEndCallback() {
    UpdateTitlePanelRemissionGuideDetails();
}

function GetSelectedFilteredRemissionGuideRowCount() {
    return gvRemissionGuideControlVehicleEntranceThird.cpFilteredRowCountWithoutPage + gvRemissionGuideControlVehicleEntranceThird.GetSelectedKeysOnPage().length;
}

function GenerateRemissionGuideControlVehicleEntrance(s, e) {
    // 
    gvRemissionGuideControlVehicleEntranceThird.GetSelectedFieldValues("id", function (values) {
        // 
        if (values != null) {
            if (values.length > 0){
                var data = {
                    id: values[0]
                };
                if (values[0] > 0) {
                    showPage("RemissionGuideControlVehicleEntranceThird/FormEditRemissionGuideControlVehicleEntranceThird", data);
                }
            }
        }
    });
}

function RemissionGuideControlVehicleEntranceThirdSecuritySeals_BeginCallback(s, e) {
    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}

function RemissionGuideControlVehicleDetailViewAssignedStaff_BeginCallback(s, e) {

    e.customArgs["id_remissionGuide"] = $("#id_remissionGuide").val();
}