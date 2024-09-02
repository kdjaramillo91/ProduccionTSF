
var rowKeySelected;

function PrintItem() {

}

function AddNewItem() {

    var data = {
        id: 0,
        enabled: true,
    }
    showPage("PenalityClassShrimp/Edit", data);
}

function EditCurrentItem() {

    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var data = {
        id: rowKeySelected,
        enabled: true
    }
    showPage("PenalityClassShrimp/Edit", data);
}

function RefreshGrid() {
    $.ajax({
        url: "PenalityClassShrimp/Index",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            debugger;
            NotifyError("Error. " + error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#maincontent").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function GridViewPenalityShow_Click(sender, e) {
    sender.GetRowValues(e.visibleIndex, 'id', function (value) {
        var data = {
            id: value,
            enabled: false
        }
        showPage("PenalityClassShrimp/Edit", data);
    });
}

function OnGridFocusedRowChanged(s, e) {
    GridViewPenality.GetRowValues(e.visibleIndex, 'id', OnGetRowValues);
}

function OnGetRowValues(values) {
    rowKeySelected = values;
}

$(function () {

});