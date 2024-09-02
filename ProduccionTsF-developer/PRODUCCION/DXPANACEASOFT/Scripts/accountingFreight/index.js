// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage("AccountingFreight/Index", null);
}

function btnSearch_click() {

    var data = $("#formFilterAccountingFreight").serialize();
    + "&id_processPlant=" + id_processPlant.GetValue() + "&liquidation_type=" + liquidation_type.GetValue();
    if (data != null) {
        $.ajax({
            url: "AccountingFreight/AccountingFreightResults",
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
    event.preventDefault();
}

function btnClear_click() {

}

function AddNewDocument(s, e) {
    AddNewAccountingFreight(s, e);
}

function AddNewAccountingFreight() {
    var data = {
        id: 0
    };

    showPage("AccountingFreight/FormEditAccountingFreight", data);
}

function OnGridViewInit(s, e) {
    //UpdateTitlePanel();
}

function OnGridViewEndCallback() {
    //UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GridViewlgvAccountingFreightCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvAccountingFreight.GetRowKey(e.visibleIndex)
        };
        showPage("AccountingFreight/FormEditAccountingFreight", data);
    }
}

function OnProcessPlantSelectedIndexChanged(s, e) {
    var id_processPlant = s.GetValue();
}

function OnLiquidationTypeSelectedIndexChanged(s, e) {
    var liquidation_type = s.GetValue();
}
