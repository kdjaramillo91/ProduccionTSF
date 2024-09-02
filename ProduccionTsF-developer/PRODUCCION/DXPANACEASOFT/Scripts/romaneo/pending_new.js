
function GridViewGenerate_Click(sender, e) {
    sender.GetRowValues(e.visibleIndex, 'idProductionLotReception', CreateNewItem);
}

function CreateNewItem(values) {
    var data = {
        id: 0,
        idProductionLotReception: values,
        enabled: true,
    }
    showPage("Romaneo/Edit", data);
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("Romaneo/Index");
    });
}

$(function () {
    Init();
});