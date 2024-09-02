
function ButtonConsultar_Click() {
    $.ajax({
        url: "AccountPlan/AccountsTreeListPartial",
        type: "get",
        data: { id_plan: $("#id").val() },
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

    event.preventDefault();
}

// ACCOUNTS METHODS

function TreeList_BeginCallback(s, e) {
    e.customArgs["id_plan"] = $("#id").val();
}

function ComboBoxParent_SelectedIndexChanged(s, e) {
    formattedNumberParent.SetValue(id_parentAccount.GetSelectedItem().GetColumnText("formatted_number"));
}


function AddNewItem(s, e) {
    alert("OK");
    tvAccount.StartEditNewNode();
}

function RemoveItems(s, e) {
    //console.log("RemoveItems");
    //var c = confirm("¿Desea eliminar las cuentas seleccionadas?");
    //if (c === true) {
    //    $.ajax({
    //        url: "Account/DeleteSelectedAccounts",
    //        type: "post",
    //        data: { ids: selectedIDs },
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            showLoading();
    //        },
    //        success: function (result) {
    //            //$("#maincontent").html(result);
    //        },
    //        complete: function () {
    //            tvAccount.PerformCallback();
    //            hideLoading();
    //        }
    //    });
    //}
}

function RefreshGrid(s, e) {
    //tvAccount.PerformCallback();
}

function Print(s, e) {

}

// SELECTION

function OnGridViewInit() {
    //UpdateTitlePanel();
}

var selectedIDs = [];

function OnGridViewSelectionChanged(s, e) {
    console.log("OnGridViewSelectionChanged");
    //UpdateTitlePanel();

    s.GetSelectedNodeValues("id", GetSelectedFieldValuesCallback);
    s.PerformCallback();
}

function GetSelectedFieldValuesCallback(values) {
    console.log("GetSelectedFieldValuesCallback");
    selectedIDs = [];
    for (var i = 0; i < values.length; i++) {
        selectedIDs.push(values[i]);
    }
}

function OnGridViewEndCallback() {
    //UpdateTitlePanel();
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {

});