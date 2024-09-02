

//Validation 

function OnValidation(s, e) {
    e.isValid = true;
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

// FORM FILTER BUTTONS

function OnClickSearchInventoryMoves(s, e) {

    var data = $("#formFilterInventoryMove").serialize();

    //var itemDocumentType = documentType.GetSelectedItem();
    //if(itemDocumentType != null && itemDocumentType != undefined) {
    //    data = "id_documentType=" + itemDocumentType.value + "&" + data;
    //}

    $.ajax({
        url: "InventoryMoveTransfer/InventoryResults",
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

    if (codeDocumentType == "05" || codeDocumentType == "32") {
        id_warehouseExit.SetSelectedItem(null);
        id_warehouseLocationExit.SetSelectedItem(null);
        id_dispatcher.SetSelectedItem(null);
    }

    if (codeDocumentType == "03" || codeDocumentType == "04" || codeDocumentType == "34") {
        id_warehouseEntry.SetSelectedItem(null);
        id_warehouseLocationEntry.SetSelectedItem(null);
        id_receiver.SetSelectedItem(null);
    }

}


function OnClickKardexInventoryMoves(s, e) {
    $.ajax({
        url: "InventoryMoveTransfer/InventoryKardexResults",
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

function AddNewInventoryMoveGeneral(code) {
    //var codeDocumentType = $("#codeDocumentTypefilter").val();
    if (code == "04")//Ingreso x Orden de Compra
    {
        $.ajax({
            url: "InventoryMoveTransfer/IndexEntryMovePurchaseOrder",
            type: "post",
            data: null,
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
                console.log("codeDocumentType: " + code);
                AddNewInventoryMove(code);
            }
        });
    }

    if (code == "03")//Ingreso
    {
        AddNewEntryManual();
    }

    if (code == "05")//Egreso
    {
        AddNewExitManual();
    }

    if (code == "32")//Egreso por Transferencia
    {
        AddNewTransferExit();
    }

    if (code == "34")//Ingreso por Transferencia
    {
        $.ajax({
            url: "InventoryMoveTransfer/IndexTransferEntryMove",
            type: "post",
            data: null,
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
                console.log("codeDocumentType: " + code);
                AddNewInventoryMove(code);
            }
        });
    }
}

function AddNewEntryManual() {
    var natureMoveTmp = $("#natureMoveFilter").val();
    var data = {
        id: 0,
        code: "03",
        natureMoveType: natureMoveTmp
    };
    showPage("InventoryMoveTransfer/InventoryMoveEditFormPartial", data);
}

function AddNewInventoryMove(code, natureMoveTmp) {
    // 
    if (code === "04") {//Ingreso x Orden de Compra
        $.ajax({
            url: "InventoryMoveTransfer/PurchaseOrdersResult",
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
                url: "InventoryMoveTransfer/InventoryMoveDetailTransferExitsResult",
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
            var data = {
                id: 0,
                code: code,
                natureMoveType: natureMoveTmp
            };
            showPage("InventoryMoveTransfer/InventoryMoveEditFormPartial", data);
        }
    }
}

function AddNewEntryPurchaseOrder() {
    var natureMoveTmp = $("#natureMoveFilter").val();
    AddNewInventoryMove("04", natureMoveTmp);//Ingreso x Orden de Compra
}

function AddNewExitManual() {
    var natureMoveTmp = $("#natureMoveFilter").val();
    AddNewInventoryMove("05", natureMoveTmp);//Egreso
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

$(function() {
    init();
});