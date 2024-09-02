
// FILTER FORM BUTTONS ACTIONS

function btnSearch_click() {
    // 

    var controls = ASPxClientControl.GetControlCollection();
    var id_DocumentTypeControl = controls.GetByName("id_DocumentTypeP");
    var id_DocumentTypeTmp = id_DocumentTypeControl.GetValue();
    var data = {
        id_documentType: id_DocumentTypeTmp
        , emissionDateStart: emissionDateStartP.GetText()
        , emissionDateEnd: emissionDateEndP.GetText()
    }
    // 
    if (data != null) {
        $.ajax({
            url: "OpenningCloseDocuments/OpenningCloseDocumentsResults",
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
    id_providerP.SetSelectedItem(null);

    liquidationDateStartP.SetDate(null);
    liquidationDateEndP.SetDate(null);
}

function AddNewDocument(s, e) {

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
    var controls = ASPxClientControl.GetControlCollection();
    var id_providerPControl = controls.GetByName("id_providerP");
    var id_providerTmp = id_providerPControl.GetValue();
    var data = {
        codeReport: "LCCPL"
    , id_provider: id_providerTmp
    , liquidationDateStart: liquidationDateStartP.GetDate()
    , liquidationDateEnd: liquidationDateEndP.GetDate()
    }
    if (data != null) {
        $.ajax({
            url: "OpeningCloseDocuments/PrintLiquidationPayProviderQuery",
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
                // 
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


// RESULT GRIDVIEW SELECTION
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

    var text = "Total de elementos seleccionados: <b>" + gvOpeningCloseDocument.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvOpeningCloseDocument.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvOpeningCloseDocument.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvOpeningCloseDocument.GetSelectedRowCount() > 0 && gvOpeningCloseDocument.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvOpeningCloseDocument.GetSelectedRowCount() > 0);
    //}


    btnCopy.SetEnabled(false);
    btnNew.SetEnabled(false);
    btnApprove.SetEnabled(false);
    btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    btnHistory.SetEnabled(false);
    btnPrint.SetEnabled(true);


}

function GetSelectedFilteredRowCount() {
    return gvOpeningCloseDocument.cpFilteredRowCountWithoutPage + gvOpeningCloseDocument.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvOpeningCloseDocument.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvOpeningCloseDocument.SelectRows();
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function OnRowDoubleClick(s, e) {
    s.GetRowValues(e.visibleIndex, "id", OnGetRowValue);
}

function OnGetRowValue(value) {
    // 
    if (value > 0) {
        showThickBox("DocumentOpenClose/PopupControlDocumentOpenCloseId", { id_doc: value });
    }
}
// MAINS FUNCTIONS
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
