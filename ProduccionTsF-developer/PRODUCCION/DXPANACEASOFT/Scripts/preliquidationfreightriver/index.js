
function btnSearch_click(s, e) {
    var data = $("#formFilterPreLiquidationriver").serialize();

    if (data != null) {
        $.ajax({
            url: "PreLiquidationFreightRiver/RemissionGuideRiverTransportationResults",
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
function btnClear_click(s, e) {
}
function PrelimLiquidationRiver_BeginCallback(s, e) {
}
function OnBatchEditEndCallback(s, e) {
}