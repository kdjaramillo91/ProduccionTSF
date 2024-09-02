
function btnSearch_click(s, e) {

    var data = $("#formFilterElectronicDocument").serialize();

    if (data != null) {
        $.ajax({
            url: "ElectronicDocument/ElectronicDocumentResults",
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
}

function btnClear_click(s, e) {

    //id_documentState.SetSelectedItem(null);
    //number.SetText("");
    //reference.SetText("");
    //startEmissionDate.SetDate(null);
    //endEmissionDate.SetDate(null);
    //startAuthorizationDate.SetDate(null);
    //endAuthorizationDate.SetDate(null);
    //authorizationNumber.SetText("");
    //accessKey.SetText("");
    //items.ClearTokenCollection();

    //id_provider.SetSelectedItem(null);
    //id_buyer.SetSelectedItem(null);
    //id_priceList.SetSelectedItem(null);
    //id_paymentTerm.SetSelectedItem(null);
    //id_paymentMethod.SetSelectedItem(null);
}

// ACTIONS BUTTONS

function SendToSri(s, e) {
    $.ajax({
        url: "ElectronicDocument/SendXmlToSri",
        type: "post",
        data: { id: s.cpId },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            
            var color = "red";
            if (result.isValid) {
                color = "green";
            }

            $.toast().reset('all');
            $.toast({
                heading: (result.isValid) ? 'Información' : 'Error',
                icon: (result.isValid) ? 'success' : 'error',
                text: result.errorText,
                showHideTransition: 'slide',  // It can be plain, fade or slide
                bgColor: color,              // Background color for toast
                textColor: 'white',            // text color
                allowToastClose: true,       // Show the close button or not
                hideAfter: 5000,              // `false` to make it sticky or time in miliseconds to hide after
                textAlign: 'left',            // Alignment of text i.e. left, right, center
                position: 'bottom-right',       // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values to position the toast on page
                afterHidden: function () {
                    gvElectronicDocuments.Refresh();
                }
            });
        },
        complete: function () {
            hideLoading();
        }
    });
}

function CkechSriAutorization(s, e) {
    $.ajax({
        url: "ElectronicDocument/CheckXmlAuthorization",
        type: "post",
        data: { id: s.cpId },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {

            var color = "red";
            if (result.isValid) {
                color = "green";
            }

            $.toast().reset('all');
            $.toast({
                heading: (result.isValid) ? 'Información' : 'Error',
                icon: (result.isValid) ? 'success' : 'error',
                text: result.errorText,
                showHideTransition: 'slide',  // It can be plain, fade or slide
                bgColor: color,               // Background color for toast
                textColor: 'white',           // text color
                allowToastClose: true,        // Show the close button or not
                hideAfter: 5000,              // `false` to make it sticky or time in miliseconds to hide after
                textAlign: 'left',            // Alignment of text i.e. left, right, center
                position: 'bottom-right',     // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values to position the toast on page
                afterHidden: function () {
                    gvElectronicDocuments.Refresh();
                }
            });
        },
        complete: function () {
            hideLoading();
        }
    });
}

function SendMail(s, e) {
    
}

function ShowXml(s, e) {

    var win = window.open("ElectronicDocument/ViewXml/" + s.cpId);
    if (win) {
        win.focus();
    }
}

function Print(s, e) {
    
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    //ShowEditMessage(s, e);
}

function OnGridViewBeginCallback(s, e) {
}

function OnGridViewSelectionChanged(s, e) {
   UpdateTitlePanel();
}

// SELECTIONS

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvElectronicDocuments.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvElectronicDocuments.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    ////if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvElectronicDocuments.GetSelectedRowCount() > 0 && gvElectronicDocuments.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvElectronicDocuments.GetSelectedRowCount() > 0);
    ////}

    ////btnRemove.SetEnabled(gvElectronicDocuments.GetSelectedRowCount() > 0);
    ////btnCopy.SetEnabled(gvElectronicDocuments.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvElectronicDocuments.cpFilteredRowCountWithoutPage + gvElectronicDocuments.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvElectronicDocuments.SelectRows();
}

function ClearSelection() {
    gvElectronicDocuments.UnselectRows();
}

// MAIN FUNCTIONS

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
})