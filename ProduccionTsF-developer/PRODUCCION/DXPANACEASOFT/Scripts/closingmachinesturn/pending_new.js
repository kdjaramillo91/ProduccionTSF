
function GridViewGenerate_Click(sender, e) {
    sender.GetRowValues(e.visibleIndex, 'id_MachineProdOpeningDetail', CreateNewItem);
}

function CreateNewItem(values) {
    var data = {
        id: 0,
        id_MachineProdOpeningDetail: values,
        enabled: true
    };
    showPage("ClosingMachinesTurn/Edit", data);
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("ClosingMachinesTurn/Index");
    });
}

$(function () {
    Init();
});