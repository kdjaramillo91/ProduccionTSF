
function GridViewBtnShow_Click(s, e) {
    s.GetRowValues(e.visibleIndex, 'id', EditItem);
}

function EditItem(values) {
    var data = {
        id: values,
        id_MachineProdOpeningDetail: 0,
        enabled: true
    };
    showPage("MachineAvailability/Edit", data);
}


function Init() {

}

$(function () {
	Init();
});
