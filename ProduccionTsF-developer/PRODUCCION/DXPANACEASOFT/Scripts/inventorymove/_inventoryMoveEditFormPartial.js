
function InitializePagination() {
    if (parseInt($("#id_inventoryMove").val()) !== 0) {

        var current_page = 1;
        $.ajax({
            url: "InventoryMove/InitializePagination",
            type: "post",
            data: { id_inventoryMove: $("#id_inventoryMove").val(), codeDocumentType: $("#codeDocumentType").val() },
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
                showForm("InventoryMove/Pagination", { page: page, codeDocumentType : $("#codeDocumentType").val() });
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