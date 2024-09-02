
function InitializePagination() {
    if (parseInt($("#id_priceList").val()) !== 0) {

        var current_page = 1;
        $.ajax({
            url: "PriceList/InitializePagination",
            type: "post",
            data: { id_priceList: $("#id_priceList").val() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                $("#pagination").attr("data-max-page", result.maximunPages);
                current_page = result.currentPage;
            },
            complete: function () {
            }
        });

        $('.pagination').jqPagination({
            current_page: current_page,
            page_string: "{current_page} de {max_page}",
            paged: function (page) {
                showForm("PriceList/Pagination", { page: page });
            }
        });
    }
}

function init() {
    InitializePagination();
}

function UpdateView() {
    //btnApprove.SetClientVisible(false);
    btnAutorize.SetClientVisible(false);
    btnProtect.SetClientVisible(false);
    //btnCancel.SetClientVisible(false);
    //btnRevert.SetClientVisible(false);
    //btnHistory.SetClientVisible(false);
}

$(function () {
    init();

    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);
});