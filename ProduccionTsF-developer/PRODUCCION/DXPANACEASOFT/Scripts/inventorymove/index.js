// Warehouse && Warehouse Location

function OnWarehouseExit_SelectedIndexChanged(s, e) {
    id_warehouseLocationExit.PerformCallback();
}

function OnWarehouseLocationExitInit(s, e) {
    s.PerformCallback();
}

function OnWarehouseEntry_SelectedIndexChanged(s, e) {
    id_warehouseLocationEntry.PerformCallback();
}

function OnWarehouseLocationEntryInit(s, e) {
    s.PerformCallback();
}

//Validation 

function OnValidation(s, e) {
    e.isValid = true;
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

// FORM FILTER BUTTONS

function OnClickSearchInventoryMoves(s, e) {


    var _natureMoveTmp = $("#natureMoveFilter").val();
    var _inventoryReasonTmp = $("#inventoryReasonFilter").val();
    var data = "natureMove=" + _natureMoveTmp
        + "&" + $("#formFilterInventoryMove").serialize()
        + "&inventoryReason=" + _inventoryReasonTmp;

    //var itemDocumentType = documentType.GetSelectedItem();
    //if(itemDocumentType != null && itemDocumentType != undefined) {
    //    data = "id_documentType=" + itemDocumentType.value + "&" + data;
    //}

    $.ajax({
        url: "InventoryMove/InventoryResults",
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

function OnClickClearFiltersInventoryMoves(s, e) {

    //documentType.SetSelectedItem(null);
    id_documentState.SetSelectedItem(null);

    number.SetText("");
    reference.SetText("");

    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);

    id_inventoryReason.SetSelectedItem(null);

    var codeDocumentType = $("#codeDocumentTypefilter").val();

    if (codeDocumentType === "05" || codeDocumentType === "32") {
        id_warehouseExit.SetSelectedItem(null);
        id_warehouseLocationExit.SetSelectedItem(null);
        id_dispatcher.SetSelectedItem(null);
    }

    if (codeDocumentType === "03" || codeDocumentType === "04" || codeDocumentType === "34") {
        id_warehouseEntry.SetSelectedItem(null);
        id_warehouseLocationEntry.SetSelectedItem(null);
        id_receiver.SetSelectedItem(null);
    }

}


function OnClickKardexInventoryMoves(s, e) {
    $.ajax({
        url: "InventoryMove/InventoryKardexResults",
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
}

function AddNewInventoryMoveGeneral(s, e, code, natureMove) {
    //var codeDocumentType = $("#codeDocumentTypefilter").val();
    //// 
    if (natureMove == "I") {
        AddNewEntryManual(s, e, natureMove, code);
    } else if (natureMove == "E") {
        AddNewExitManual(s, e, natureMove, code);
    }
}

function AddNewEntryManual(s, e, codeNature, code) {
    //// 
    var natureMoveTmp = $("#natureMoveFilter").val();
    if (codeNature == "I" || codeNature == "E") {
        natureMoveTmp = codeNature;
    }

    var customParamOPTmp = $("#customParamOP").val();
    AddNewInventoryMove((code != null && code != undefined && code != "") ? code : $("#codeDocumentTypefilter").val(), natureMoveTmp, customParamOPTmp);
    //var data = {
    //    id: 0,
    //    code: (code != null && code != undefined && code != "") ?code: $("#codeDocumentTypefilter").val(),
    //    natureMoveType: natureMoveTmp
    //};
    //showPage("InventoryMove/InventoryMoveEditFormPartial", data);
}

function AddNewInventoryMove(code, natureMoveTmp, customParamOPTmp) {

    if (code === "04") {//Ingreso x Orden de Compra
        $.ajax({
            url: "InventoryMove/PurchaseOrdersResult",
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
    }
    else {
        if (code === "34") {//Ingreso Por Transferencia
            $.ajax({
                url: "InventoryMove/InventoryMoveDetailTransferExitsResult",
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
        }
        else {
            // 
            var data =
            {
                id: 0,
                code: code,
                natureMoveType: natureMoveTmp,
                customParamOP: customParamOPTmp
            };
            showPage("InventoryMove/InventoryMoveEditFormPartial", data);
        }
    }
}

function AddNewEntryPurchaseOrder() {
    var natureMoveTmp = $("#natureMoveFilter").val();
    AddNewInventoryMove("04", natureMoveTmp);//Ingreso x Orden de Compra
}

function AddNewExitManualPackanging() {
    var natureMoveTmp = $("#natureMoveFilter").val();
    AddNewInventoryMove("50", natureMoveTmp);//Egreso
}

function AddNewExitManual(s, e, codeNature, code) {
    var natureMoveTmp = $("#natureMoveFilter").val();
    if (codeNature == "I" || codeNature == "E") {
        natureMoveTmp = codeNature;
    }

    //// 
    var customParamOPTmp = $("#customParamOP").val();
    AddNewInventoryMove((code != null && code != undefined && code != "") ? code : $("#codeDocumentTypefilter").val(), natureMoveTmp, customParamOPTmp);
}

function AddNewTransferExit() {
    var natureMoveTmp = $("#natureMoveFilter").val();
    AddNewInventoryMove("32", natureMoveTmp);//Egreso Por Transferencia
}

function AddNewTransferEntry() {
    var natureMoveTmp = $("#natureMoveFilter").val();
    AddNewInventoryMove("34", natureMoveTmp);//Ingreso Por Transferencia
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