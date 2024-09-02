


function btnAsignarCustodio_click()
{
    $.ajax({
        url: "CustodianIncome/RemissionGuideDetailsResults",
        type: "post",
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
            $("#btnCollapse").click();
            $("#results").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });

    event.preventDefault();  

}

function btnGenerateCustodianIncome_Click(s,e)
{
    gridMessageErrorRemissionGuide.SetText("");
    $("#GridMessageErrorRemissionGuide").hide();

    showLoading();

    gvRemissionGuide.GetSelectedFieldValues("id", function (values) {

        if (values.length > 1)
        {
            hideLoading();
            var aError = "Solo se puede procesar una Guia de Remision";
            gridMessageErrorRemissionGuide.SetText(ErrorMessage(aError));
            $("#GridMessageErrorRemissionGuide").show();
            return;
        }
        let remissionId = values[0];

        var data = {
            id: 0,
            remissionid: remissionId
        };
        showPage("CustodianIncome/FormEditCustodianIncome", data);
         

    });
}

function OnGridViewRemissionGuideDetailsInit(s, e) {
    UpdateTitlePanelOrderDetails();
}

function UpdateTitlePanelOrderDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredOrderDetailsRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuide.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuide.GetSelectedRowCount() - GetSelectedFilteredOrderDetailsRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuide.GetSelectedRowCount() > 0 && gvRemissionGuide.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuide.GetSelectedRowCount() > 0);
    //}

    btnGenerateCustodianIncome.SetEnabled(gvRemissionGuide.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredOrderDetailsRowCount() {
    return gvRemissionGuide.cpFilteredRowCountWithoutPage + gvRemissionGuide.GetSelectedKeysOnPage().length;
}

function OnGridViewRemissionGuideDetailsSelectionChanged(s, e) {
    UpdateTitlePanelOrderDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}
function GetSelectedFieldDetailValuesCallback(values) {
    // 
    selectedRemissionGuideDetailsRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideDetailsRows.push(values[i]);
    }
}
function OnGridViewRemissionGuideDetailsEndCallback() {
    UpdateTitlePanelOrderDetails();
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

/* 
  Result Partial 
*/
function OnGridViewInit(s, e)
{

}
function OnGridViewSelectionChanged(s, e) {

}
function OnGridViewEndCallback(s, e) {

}

function GridViewCustodiaIncomeCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        
        var aId = gvCustodianIncome.GetRowKey(e.visibleIndex)
        let data = {
            id: aId
        };
        showPage("CustodianIncome/FormEditCustodianIncome", data );
    }
}


// #region Filter Actions Clear
function btnClear_click() {
    id_documentState_init();
    number.SetText("");
    reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    startAuthorizationDate.SetDate(null);
    endAuthorizationDate.SetDate(null);
    authorizationNumber.SetText("");
    accessKey.SetText("");

    startDespachureDate.SetDate(null);
    endDespachureDate.SetDate(null);

    startexitDateProductionBuilding.SetDate(null);
    endexitDateProductionBuilding.SetDate(null);

    startentranceDateProductionBuilding.SetDate(null);
    endentranceDateProductionBuilding.SetDate(null);
    id_ProviderRemisionGuide_init();
    id_driver_init();
    carRegistrationFilter.SetText("");
}
function id_driver_init()
{
    id_driver.SetValue(null);
    id_driver.SetText("");
}
function id_ProviderRemisionGuide_init()
{
    id_ProviderRemisionGuide.SetValue(null);
    id_ProviderRemisionGuide.SetText("");
}
function id_documentState_init()
{
    id_documentState.SetValue(null);
    id_documentState.SetText("");
}

// #endregion

// #region Filter Actions Search
function btnSearch_click() {

    var data = $("#formFilterCustodianIncome").serialize();

    if (data !== null) {
        $.ajax({
            url: "CustodianIncome/CustodianIncomeResults",
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

// #endregion

// #region Init Form js
var init = function () {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
};

$(function () {
    init();
});
// #endregion
