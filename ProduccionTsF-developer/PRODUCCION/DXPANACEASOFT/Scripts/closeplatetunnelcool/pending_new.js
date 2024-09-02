
function GridViewGenerate_Click(sender, e) {
    sender.GetRowValues(e.visibleIndex, 'id_MachineProdOpeningDetail', CreateNewItem);
}

function CreateNewItem(values) {
    var data = {
        id: 0,
        id_MachineProdOpeningDetail: values,
        enabled: true
    };
    showPage("ClosePlateTunnelCool/Edit", data);
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("ClosePlateTunnelCool/Index");
    });
}

$(function () {
    Init();
});