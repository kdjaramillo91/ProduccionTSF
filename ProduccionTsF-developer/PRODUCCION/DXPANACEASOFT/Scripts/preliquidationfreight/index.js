
function btnSearch_click(s, e) {
    var data = $("#formFilterPreLiquidation").serialize();

    if (data != null) {
        $.ajax({
            url: "PreLiquidationFreight/RemissionGuideTransportationResults",
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
function PrelimLiquidation_BeginCallback(s, e) {
}
function OnBatchEditEndCallback(s, e) {
}