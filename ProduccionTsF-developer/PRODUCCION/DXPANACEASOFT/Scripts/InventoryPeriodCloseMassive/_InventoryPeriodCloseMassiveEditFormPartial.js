function InitializePagination() {
    if (parseInt($("#id_InventoryPeriod").val()) !== 0) {

        var current_page = 1;
        $.ajax({
            url: "InventoryPeriodCloseMassive/InitializePagination",
            type: "post",
            data: { id_InventoryPeriod: $("#id_InventoryPeriod").val() },
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
                showForm("InventoryPeriodCloseMassive/Pagination", { page: page });
            }
        });
    }
}


function init() {
    InitializePagination();
}

$(function () {
    init();
});