function InitializePagination() {
    if (parseInt(parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"))) !== 0) {

        var current_page = 1;
        $.ajax({
            url: "RemissionGuideRiver/InitializePagination",
            type: "post",
            data: { id_remissionGuide: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")) },
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
                showForm("RemissionGuideRiver/Pagination", { page: page });
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