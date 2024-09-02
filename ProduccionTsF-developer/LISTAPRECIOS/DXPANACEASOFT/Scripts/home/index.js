
function dashboard() {
    $.ajax({
        url: "Home/DashboardViewerPartial",
        type: "post",
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            $("#maincontent").html(result);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

$(function () {
    dashboard();
});