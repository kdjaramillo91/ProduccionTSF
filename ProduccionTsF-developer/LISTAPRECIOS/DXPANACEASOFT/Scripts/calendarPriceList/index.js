
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage(" Calendar/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterCalendarPriceList").serialize() + "&startDate=" + startDate.GetValue() + "&endDate=" + endDate.GetValue() + "&id_calendarPriceListType=" + id_calendarPriceListType.GetValue();
      if (data != null) {
        $.ajax({
            url: "Calendar/CalendarResults",
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
  
    startDate.SetDate(null);
    endDate.SetDate(null);
    id_calendarPriceListType.SetSelectedItem(null);
 
}

function AddNewCalendar() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("Calendar/FormEditCalendarPriceList", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvCalendar.GetSelectedFieldValues("id", function (values) {

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
                gvCalendar.PerformCallback();
                gvCalendar.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewCalendar(s, e);
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
    var codeReport = "CDLP";
    //var data = "codeReport=" + codeReport + "&" + $("#ProductionLotReceptionFilterForm").serialize();
    debugger;

    var strDtStart = GetStringFromDate(startDate.GetText(), "dd/MM/yyyy");
    var strDtEnd = GetStringFromDate(endDate.GetText(), "dd/MM/yyyy");

    var data = {
        codeReport: "CDLP"
        , str_emissionDateStart: strDtStart
        , str_emissionDateEnd: strDtEnd
    }

    if (data != null) {
        $.ajax({
            url: "Calendar/PrintRemissionGuideReportList",
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
                debugger;
                try {
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
}



function GridViewlgvCalendarCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvCalendar.GetRowKey(e.visibleIndex)
        };
        showPage("Calendar/FormEditCalendarPriceList", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvCalendar.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCalendar.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvCalendar.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvCalendar.GetSelectedRowCount() > 0 && gvCalendar.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCalendar.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvCalendar.GetSelectedRowCount() == 1);

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
    return gvCalendar.cpFilteredRowCountWithoutPage + gvCalendar.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvCalendar.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvCalendar.SelectRows();
}

function GetStringFromDate(dt_in, format_in) {
    debugger;
    var _str_res = "";
    if (format_in == "dd/MM/yyyy") {
        var tmp = dt_in.split("/");
        if (tmp.length == 3) {
            _str_res = tmp[2] + "-" + tmp[1] + "-" + tmp[0];
        }
    }
    return _str_res;
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