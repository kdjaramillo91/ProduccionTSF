
function ProductionLotReceptionSearch_Click(s, e) {
    var data = $("#formProductionLotLiquidationFilter").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotLiquidation/ProductionLotLiquidationResults",
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
}

function ButtonNewLiquidation_Click() {
    showPage("ProductionLotLiquidation/GenerateLotLiquidation", null);
}

function ButtonLiquidateLot_Click(s, e) {
    var data = null;
    showPage("ProductionLotLiquidation/GenerateLotLiquidation", data);
}

function init() {
    $("#btnCollapse").click(function (event) {
        $("#results").html("");
    });
}

$(function () {
    init();
});