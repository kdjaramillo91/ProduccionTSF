
// FILTER FORM BUTTONS ACTIONS

   function btnSearch_click() {
    //var data = $("#formFilterPurchaseOrder").serialize();
    var data = $("#formFilteradvanceproviderquery").serialize();
    // 
    if (data != null) {
        $.ajax({
            url: "AdvanceProviderQuery/AdvanceProviderQueryResults",
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
    id_provider.SetSelectedItem(null);

    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
   


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
        
        var codeReport = "APCAP";
        var data = "codeReport=" + codeReport + "&" + $("#formFilteradvanceproviderquery").serialize();

        if (data != null) {
            $.ajax({
                url: "AdvanceProviderQuery/PrintAdvanceProviderQuery",
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

    // REMISSION GUIDES RESULT GRIDVIEW SELECTION
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

        var text = "Total de elementos seleccionados: <b>" + gvAdvanceProvider.GetSelectedRowCount() + "</b>";
        var hiddenSelectedRowCount = gvAdvanceProvider.GetSelectedRowCount() - GetSelectedFilteredRowCount();

        if (hiddenSelectedRowCount > 0)
            text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
        text += "<br />";
        $("#lblInfo").html(text);

        //console.log(gvAdvanceProvider.GetSelectedRowCount());

        //if ($("#selectAllMode").val() != "AllPages") {
        SetElementVisibility("lnkSelectAllRows", gvAdvanceProvider.GetSelectedRowCount() > 0 && gvAdvanceProvider.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelection", gvAdvanceProvider.GetSelectedRowCount() > 0);
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
        return gvAdvanceProvider.cpFilteredRowCountWithoutPage + gvAdvanceProvider.GetSelectedKeysOnPage().length;
    }
    function SetElementVisibility(id, visible) {
        var $element = $("#" + id);
        visible ? $element.show() : $element.hide();
    }
    function gvResultsClearSelection() {
        gvAdvanceProvider.UnselectRows();
    }
    function gvResultsSelectAllRows() {
        gvAdvanceProvider.SelectRows();
    }
    // GENERATE REMISSION GUIDE FROM PURCHASE ORDERS
    // PURCHASE ORDERS RESULT GRIDVIEW SELECTION
    function SetElementVisibility(id, visible) {
        var $element = $("#" + id);
        visible ? $element.show() : $element.hide();
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
