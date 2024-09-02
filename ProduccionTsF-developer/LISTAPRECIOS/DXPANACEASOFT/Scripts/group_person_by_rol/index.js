
var rowKeySelected;

function PrintItem() {

    $.ajax({
        url: 'GroupPersonByRol/PrintGroupReportList',
        type: 'post',
        async: true,
        cache: false,
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                debugger;
                if (result != undefined) {
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
                    newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                    newWindow.focus();
                    hideLoading();
                }
            }
            catch (err) {
                hideLoading();
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function AddNewItem() {

    var data = {
        id: 0,
        enabled: true,
    }
    showPage("GroupPersonByRol/Edit", data);
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
    showPage("GroupPersonByRol/Edit", data);
}

function RefreshGrid() {
    $.ajax({
        url: "GroupPersonByRol/Index",
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
        showPage("GroupPersonByRol/Edit", data);
    });
}

function OnGridFocusedRowChanged(s, e) {
    GridViewGroupPersonaByRol.GetRowValues(e.visibleIndex, 'id', OnGetRowValues);
}

function OnGetRowValues(values) {
    rowKeySelected = values;
}

$(function () {

});