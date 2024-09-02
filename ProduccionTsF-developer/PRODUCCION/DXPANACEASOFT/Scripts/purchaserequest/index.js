
//Validation 


function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

//BOTONES PANTALLA FILTROS

function OnClickAddNewPurchaseRequest(s, e) {

    var data = {
        id: 0
    };

    showPage("PurchaseRequest/FormEditPurchaseRequest", data);
}

function OnClickSearchPurchaseRequest(s, e) {

    var data = $("#formFilterPurchaseRequest").serialize() + "&PurcharFilter=" + purchafil.GetValue();

    if (data != null) {
        $.ajax({
            url: "PurchaseRequest/PurchaseRequestResults",
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

function OnClickClearFiltersPurchaseRequest(s, e) {
    id_documentState.SetSelectedItem(null);
    number.SetText("");
    //reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    items.ClearTokenCollection();
    purchafil.SetSelectedItem(null);

    id_personRequesting.SetSelectedItem(null);
}

// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvPurchaseRequests.GetSelectedFieldValues("id", function (values) {

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
                gvPurchaseRequests.PerformCallback();
                //gvPurchaseRequests.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    OnClickAddNewPurchaseRequest(s, e);
}

function CopyDocument(s, e) {
    gvPurchaseRequests.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("PurchaseRequest/PurchaseRequestCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseRequest/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseRequest/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseRequest/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseRequest/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseRequest/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

} 

function Print(s, e) {
     
    


    gvPurchaseRequests.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var id =  selectedRows[0];

        var data = { ReportName: "PurchaseRequestReport", ReportDescription: "Requerimiento Compra", ListReportParameter: [] };


        $.ajax({
            url: 'PurchaseRequest/PurchaseRequestReport?id=' + id,
            data: data,
            async: true,
            cache: false,
            type: 'POST',
            beforeSend: function () {
                showLoading();
            },
            success: function (response) {
                 

                try {
                    if (response.isvalid) {
                        var reportModel = response.reportModel;
                        var url = 'Report/Index?reportModel=' + reportModel;
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


    });


    }
    

//function Print(s, e) {
//    gvPurchaseRequests.GetSelectedFieldValues("id", function (values) {

//        var selectedRows = [];
//        for (var i = 0; i < values.length; i++) {
//            selectedRows.push(values[i]);
//        }

//        $.ajax({
//            url: "PurchaseRequest/PurchaseRequestReport",
//            type: "post",
//            data: { id: selectedRows[0] },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                showLoading();
//            },
//            success: function (result) {
//                $("#maincontent").html(result);
//            },
//            complete: function () {
//                hideLoading();
//            }
//        });

//    });
    
//}

function OnClickUpdatePurchaseRequest(s, e) {

    var data = {
        id: gvPurchaseRequests.GetRowKey(e.visibleIndex)
    };

    showPage("PurchaseRequest/FormEditPurchaseRequest", data);
}

// SELECTION

function OnRowDoubleClick(s, e) {
    s.GetRowValues(e.visibleIndex, "id", function (value) {
        showPage("PurchaseRequest/FormEditPurchaseRequest", { id: value });
    });
}

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();
    
    var text = "Total de elementos seleccionados: <b>" + gvPurchaseRequests.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseRequests.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);
    
    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseRequests.GetSelectedRowCount() > 0 && gvPurchaseRequests.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseRequests.GetSelectedRowCount() > 0);
    //}

    btnApprove.SetEnabled(false);
    btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    btnHistory.SetEnabled(false);
    btnPrint.SetEnabled(false);

    btnCopy.SetEnabled(gvPurchaseRequests.GetSelectedRowCount() === 1);
    //btnApprove.SetEnabled(gvPurchaseRequests.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvPurchaseRequests.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvPurchaseRequests.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvPurchaseRequests.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvPurchaseRequests.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvPurchaseRequests.GetSelectedRowCount() === 1);
    //btnPrint.SetEnabled(gvPurchaseRequests.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvPurchaseRequests.cpFilteredRowCountWithoutPage + gvPurchaseRequests.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvPurchaseRequests.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvPurchaseRequests.SelectRows();
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

