
function GridViewGenerate_Click(sender, e) {
    showLoading();
    sender.GetRowValues(e.visibleIndex, 'id', CreateNewItem);
}

function CreateNewItem(values) {
    var data = {
        id: 0,
        ids: [],
        id_proforma: values,
        enabled: true
    };
    showPage("SalesOrder/Edit", data);
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("SalesOrder/Index");
    });
}

$(function () {
    Init();
});