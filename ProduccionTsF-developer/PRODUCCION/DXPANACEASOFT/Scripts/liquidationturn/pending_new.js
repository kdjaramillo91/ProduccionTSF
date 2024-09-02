
function GridViewGenerate_Click(sender, e) {
	sender.GetRowValues(e.visibleIndex, 'id_turn;emissionDateStr;id_personProcessPlant', CreateNewItem);
}

function CreateNewItem(values) {
    var data = {
        id: 0,
        id_turn: values[0],
		emissionDate: values[1],
		id_personProcessPlant: values[2],
        enabled: true
    };
    showPage("LiquidationTurn/Edit", data);
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("LiquidationTurn/Index");
    });
}

$(function () {
    Init();
});