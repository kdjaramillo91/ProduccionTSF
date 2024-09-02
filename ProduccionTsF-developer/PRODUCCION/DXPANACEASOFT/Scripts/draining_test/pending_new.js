
function GridViewGenerate_Click(sender, e) {
    sender.GetRowValues(e.visibleIndex, 'idProductionLotReceptionDetail', CreateNewItem);
}

function CreateNewItem(values) {
    var data = {
        id: 0,
        idProductionLotReceptionDetail: values,
        enabled: true,
    }
    showPage("DrainingTest/Edit", data);
}

$(function () {
});