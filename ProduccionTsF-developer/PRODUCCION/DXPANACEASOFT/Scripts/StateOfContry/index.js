
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage("StateOfContry/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterStateOfContry").serialize() + "&scode=" + scode.GetValue() + "&id_country=" + id_country.GetValue();
      if (data != null) {
        $.ajax({
            url: "StateOfContry/StateOfContryResults",
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
 
}

function AddNewStateOfContry() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("StateOfContry/FormEditStateOfContry", data);
}

function Combocountry_SelectedIndexChanged(s, e) {
    var data = {
        id_country: s.GetValue()
    };

    nameFishingSite.SetValue(null);
    nameFishingZone.SetValue(null);

    $.ajax({
        url: "StateOfContry/StateOfContryall",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //da.SetValue(result.Provider_address)
      },
        complete: function () {
            id_StateOfContry.PerformCallback();

        }
    });

}

// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvStateOfContry.GetSelectedFieldValues("id", function (values) {

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
                gvStateOfContry.PerformCallback();
                gvStateOfContry.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewStateOfContry(s, e);
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



function GridViewlgvStateOfContryCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvStateOfContry.GetRowKey(e.visibleIndex)
        };
        showPage("StateOfContry/FormEditStateOfContry", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvStateOfContry.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvStateOfContry.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvStateOfContry.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvStateOfContry.GetSelectedRowCount() > 0 && gvStateOfContry.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvStateOfContry.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvStateOfContry.GetSelectedRowCount() == 1);

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
    return gvStateOfContry.cpFilteredRowCountWithoutPage + gvStateOfContry.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvStateOfContry.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvStateOfContry.SelectRows();
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