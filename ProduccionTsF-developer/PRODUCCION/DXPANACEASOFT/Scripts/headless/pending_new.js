
function GridViewGenerate_Click(sender, e) {
    showLoading();
    sender.GetRowValues(e.visibleIndex, 'id_productionLot', CreateNewItem);
}

function CreateNewItem(values) {
    var data = {
        id: 0,
        id_productionLot: values,
        enabled: true
    };
    showPage("Headless/Edit", data);
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("Headless/Index");
    });
}

$(function () {
    Init();
});