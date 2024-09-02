
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage(" City/Index", null);
}
function Combocountry_SelectedIndexChanged(s, e) {
    var data = {
        id_country: s.GetValue()
      
    };
    // 
   

    $.ajax({
        url: "City/StateOfContryall",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            //da.SetValue(result.Provider_address)
        },
        complete: function () {
            id_StateOfContry.PerformCallback();
            hideLoading();
        }
    });

}
function btnSearch_click() {
   
    var data = $("#formFilterCity").serialize() + "&scode=" + scode.GetValue() + "&id_country=" + id_country.GetValue() + "&id_StateOfContry=" + id_StateOfContry.GetValue();
      if (data != null) {
        $.ajax({
            url: "City/CityResults",
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
  
    scode.SetValue(null);
    id_country.SetSelectedItem(null);
    id_StateOfContry.SetSelectedItem(null);
}

function AddNewCity() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("City/FormEditCity", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvCity.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
         
        $.ajax({
            url: url,
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvCity.PerformCallback();
                gvCity.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewCity(s, e);
}
function CopyDocument(s, e) {
   
}
function ApproveDocuments(s, e) {

}
function AutorizeDocuments(s, e) {
 
}
function ProtectDocuments(s, e) {

}
function CancelDocuments(s, e) {

}
function RevertDocuments(s, e) {

}
function ShowHistory(s, e) {

}
function Print(s, e) {
    
}



function GridViewlgvCityCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvCity.GetRowKey(e.visibleIndex)
        };
        showPage("City/FormEditCity", data);
    }
}



function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

var selectedRows = [];

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvCity.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCity.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvCity.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvCity.GetSelectedRowCount() > 0 && gvCity.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCity.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvCity.GetSelectedRowCount() == 1);

//    btnCopy.SetEnabled(false);
//    btnApprove.SetEnabled(false);
//    btnAutorize.SetEnabled(false);
//    btnProtect.SetEnabled(false);
//    btnCancel.SetEnabled(false);
//    btnRevert.SetEnabled(false);
//    btnHistory.SetEnabled(false);
//    btnPrint.SetEnabled(false);
}

function GetSelectedFilteredRowCount() {
    return gvCity.cpFilteredRowCountWithoutPage + gvCity.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvCity.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvCity.SelectRows();
}

function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
}
$(function () {
    init();
});