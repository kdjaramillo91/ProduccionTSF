// DETAILS VIEW CALLBACKS

function BusinessOportunity_OnBeginCallback(s, e) {
    e.customArgs["id_businessOportunity"] = s.cpIdBusinessOportunity;
}

//Validation 

function OnValidation(s, e) {
    e.isValid = true;
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
    //var validated = ValidateRangeDate(startEmissionDate.GetValue(), endEmissionDate.GetValue());
    //if (!validated) {
    //    e.isValid = validated;
    //    e.errorText = "Rango de Fecha no válido";
    //}
}

function OnRangeStartDateValidation(s, e) {
    OnRangeDateValidation(e, startStartDate.GetValue(), endStartDate.GetValue(), "Rango de Fecha no válido");
}

function OnRangeEndDateValidation(s, e) {
    OnRangeDateValidation(e, startEndDate.GetValue(), endEndDate.GetValue(), "Rango de Fecha no válido");
}
//function OnRangeDateValidation(e, startDate, endDate, msgError) {
//    var validated = ValidateRangeDate(startDate, endDate);
//    if (!validated) {
//        e.isValid = validated;
//        e.errorText = msgError;
//    }
//}

//DOCUMENT TYPE

function DocumentType_SelectedIndexChanged(s, e) {
    id_documentState.ClearItems();
    id_documentState.SetValue(null);

    id_businessPartner.ClearItems();
    id_businessPartner.SetValue(null);

        $.ajax({
            url: "BusinessOportunity/DocumentTypeDetailsData",//ItemPlaningDetailsData",
            type: "post",
            data: {id_documentType : s.GetValue()},
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null) {

                    var arrayFieldStr = [];
                    arrayFieldStr.push("name");
                    UpdateDetailObjects(id_documentState, result.documentStates, arrayFieldStr);

                    var arrayFieldStr = [];
                    arrayFieldStr.push("fullname_businessName");
                    UpdateDetailObjects(id_businessPartner, result.businessPartners, arrayFieldStr);

                }
            },
            complete: function () {
                //hideLoading();
            }
        });
}


// FILTER FORM BUTTONS

function btnSearch_click(s, e) {
    var data = $("#formFilterBusinessOportunity").serialize();

    if (data !== null) {
        $.ajax({
            url: "BusinessOportunity/BusinessOportunityResults",
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
    id_documentType.SetValue(null);
    id_documentType.SetText("");
    id_documentState.SetValue(null);
    id_documentState.SetText("");
    number.SetText("");

    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    startStartDate.SetDate(null);
    endStartDate.SetDate(null);
    startEndDate.SetDate(null);
    endEndDate.SetDate(null);

    nameOportunity.SetText("");
    id_businessPartner.SetValue(null);
    id_businessPartner.SetText("");
    contactPerson.SetText("");
    id_executivePerson.SetValue(null);
    id_executivePerson.SetText("");
    id_businessOportunityState.SetValue(null);
    id_businessOportunityState.SetText("");
}

function AddNewOportunity(s,e) {
    var data = {
        id: 0
        //type: type
    };
    showPage("BusinessOportunity/FormEditBusinessOportunity", data);
}

// GRIDVIEW BUSINESS OPORTUNITIES RESULTS ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvBusinessOportunities.GetSelectedFieldValues("id", function (values) {

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
                gvBusinessOportunities.PerformCallback();
                // gvBusinessOportunities.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    //AddNewItemManual(s, e);
    AddNewOportunity(s, e);
}

function CopyDocument(s, e) {
    //gvBusinessOportunities.GetSelectedFieldValues("id", function (values) {
    //    if (values.length > 0) {
    //        showPage("PurchaseOrder/PurchaseOrderCopy", { id: values[0] });
    //    }
    //});
}

function ApproveDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/ApproveDocuments");
    //}, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/AutorizeDocuments");
    //}, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/ProtectDocuments");
    //}, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/CancelDocuments");
    //}, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/RevertDocuments");
    //}, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {

    //gvBusinessOportunities.GetSelectedFieldValues("id", function (values) {

    //    var selectedRows = [];

    //    for (var i = 0; i < values.length; i++) {
    //        selectedRows.push(values[i]);
    //    }

    //    $.ajax({
    //        url: "PurchaseOrder/PurchaseOrdersReport",
    //        type: "post",
    //        data: { ids: selectedRows },
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            showLoading();
    //        },
    //        success: function (result) {
    //            $("#maincontent").html(result);
    //        },
    //        complete: function () {
    //            hideLoading();
    //        }
    //    });

    //});

}

function BusinessOportunityGridViewCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        //var keys = gvBusinessOportunities.GetSelectedKeysOnPage();
        gvBusinessOportunities.GetPageRowValues("id", function (values) {

            //var selectedRows = [];
            //for (var i = 0; i < values.length; i++) {
            //    selectedRows.push(values[i]);
            //}
            console.log("values: " + values);
            console.log("e.visibleIndex: " + (e.visibleIndex));
            console.log("values[e.visibleIndex]: " + values[e.visibleIndex]);

            var data = {
                id: values[e.visibleIndex]
                //id: gvBusinessOportunities.GetRowKey(e.visibleIndex)
            };
            showPage("BusinessOportunity/FormEditBusinessOportunity", data);

            //$.ajax({
            //    url: url,
            //    type: "post",
            //    data: { ids: selectedRows },
            //    async: true,
            //    cache: false,
            //    error: function (error) {
            //        console.log(error);
            //    },
            //    beforeSend: function () {
            //        //showLoading();
            //    },
            //    success: function (result) {
            //        //console.log(result);
            //    },
            //    complete: function () {
            //        //hideLoading();
            //        gvProductionLotReceptions.PerformCallback();
            //        // gvPurchaseOrders.UnselectRows();
            //    }
            //});

        });
        //console.log("keys: " + keys);
        //console.log("keys[e.visibleIndex]: " + keys[e.visibleIndex]);
        //console.log("e.visibleIndex: " +e.visibleIndex);
        //var data = {
        //    id: keys[e.visibleIndex]
        //    //id: gvBusinessOportunities.GetRowKey(e.visibleIndex)
        //};
        //showPage("BusinessOportunity/FormEditBusinessOportunity", data);
    }
}

function BusinessOportunity_OnBeginCallback(s, e) {
    e.customArgs['id_businessOportunity'] = $("#id_businessOportunity").val();
}

// GRIDVIEW BUSINESS OPORTUNITY SELECTION

function OnRowDoubleClick(s, e) {
    //s.GetRowValues(e.visibleIndex, "id", function (value) {
    //    showPage("PurchaseOrder/FormEditPurchaseOrder", { id: value });
    //});
}

function BusinessOportunityOnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function BusinessOportunityOnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function BusinessOportunityOnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvBusinessOportunities.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvBusinessOportunities.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvBusinessOportunities.GetSelectedRowCount() > 0 && gvBusinessOportunities.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvBusinessOportunities.GetSelectedRowCount() > 0);
    //}

    btnCopy.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() === 1);
    btnApprove.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnAutorize.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnProtect.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnCancel.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnRevert.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnHistory.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() === 1);
    btnPrint.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() === 1);

}

function GetSelectedFilteredRowCount() {
    return gvBusinessOportunities.cpFilteredRowCountWithoutPage + gvBusinessOportunities.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function BusinessOportunitySelectAllRows() {
    gvBusinessOportunities.SelectRows();
}

function BusinessOportunityClearSelection() {
    gvBusinessOportunities.UnselectRows();
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
});