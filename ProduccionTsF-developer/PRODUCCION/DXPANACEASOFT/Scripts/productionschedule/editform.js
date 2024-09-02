
function ProductionSchedulePeriodCombo_SelectedIndexChanged(s, e) {
    //if (data.id_productionSchedulePurchaseRequestDetail === null) {
    //    salesPurchaseRequest.SetText("");
    //    outstandingAmountRequired.SetValue(0);
    //    metricUnitPurchaseRequest.SetText("");
    //    metricUnitScheduleRequest.SetText("");
    //    ValidateScheduleDetail();
    //    return;
    //}

    $.ajax({
        url: "ProductionSchedule/ProductionSchedulePeriodDetailDate",
        type: "post",
        data: { id_productionSchedulePeriod : s.GetValue()},
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {

            $("#dateStarProductionSchedulePeriod").val(result.dateStarProductionSchedulePeriod);
            $("#dateEndProductionSchedulePeriod").val(result.dateEndProductionSchedulePeriod);
            //salesPurchaseRequest.SetText(result.salesPurchaseRequest);
            //outstandingAmountRequired.SetValue(result.outstandingAmountRequired);
            //metricUnitPurchaseRequest.SetText(result.metricUnitPurchaseRequest);
            //metricUnitScheduleRequest.SetText(result.metricUnitScheduleRequest);

        },
        complete: function () {
            //hideLoading();
            //ValidateScheduleDetail();
        }
    });
}

function ProductionScheduleDetail_OnBeginCallback(s, e) {
    e.customArgs["id_productionSchedule"] = $("#id_productionSchedule").val();
    s.cpIdProductionSchedule = $("#id_productionSchedule").val();
}

function ProductionSchedulePurchaseRequestDetail_OnEndCallback(s, e) {
    try {
        btnNewProductionScheduleScheduleDetail.SetEnabled(gvProductionSchedulePurchaseRequestDetail.cpRowsCount > 0);
    } catch (e) {

    }
}

// DIALOG BUTTONS ACTIONS

function Update(approve) {
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);
    var valid = true;
    var validProductionScheduleDocument = ASPxClientEdit.ValidateEditorsInContainerById("productionScheduleDocument", null, true);
    //var validMainTabPurchaseOrder = ASPxClientEdit.ValidateEditorsInContainerById("mainTabPurchaseOrder", null, true);

    if (validProductionScheduleDocument) {
        UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    if (gvProductionScheduleRequestDetail.cpRowsCount === 0 || gvProductionScheduleRequestDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabProductionScheduleRequestsDetails");
        valid = false;
    }

    if (gvProductionScheduleScheduleDetail.cpRowsCount === 0 || gvProductionScheduleScheduleDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabProductionScheduleSchedulesDetails");
        valid = false;
    }

    if (approve && !gvProductionScheduleScheduleDetail.IsEditing()) {
        $.ajax({
            url: "ProductionSchedule/ProductionSchedulesAllSchedules",
                type: "post",
                data: null,
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    if (result !== null) {
                        if (result.itsAllSchedules == 0) {
                            UpdateTabImage({ isValid: false }, "tabProductionScheduleSchedulesDetails");
                            valid = false;
                            //e.errorText = result.Error;
                            var msgErrorAux = ErrorMessage(result.Error);
                            gridMessageErrorProductionScheduleScheduleDetail.SetText(msgErrorAux);
                            $("#GridMessageErrorProductionScheduleScheduleDetail").show();
                        }
                        //else {
                        //    id_itemIniAux = 0
                        //    id_purchaseRequestDetailIniAux = 0
                        //}
                    }
                },
                complete: function () {
                    hideLoading();
                    //showPage("PurchasePlanning/Index", null);
                }
            });
    }

    if (valid) {
        var id = $("#id_productionSchedule").val();
        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditProductionSchedule").serialize();
        var url = (id === "0") ? "ProductionSchedule/ProductionSchedulesPartialAddNew"
                               : "ProductionSchedule/ProductionSchedulesPartialUpdate";

        showForm(url, data);
    }
}

function ButtonUpdate_Click(s, e) {

    Update(false);

}

function ButtonUpdateClose_Click(s, e) {
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    //if (valid) {
    //    var id = $("#id_purchasePlanning").val();

    //    var data = "id=" + id + "&" + $("#formEditPurchasePlanning").serialize();

    //    var url = (id === "0") ? "PurchasePlanning/PurchasePlanningsAddNew"
    //                           : "PurchasePlanning/PurchasePlanningsUpdate";

    //    if (data != null) {
    //        $.ajax({
    //            url: url,
    //            type: "post",
    //            data: data,
    //            async: true,
    //            cache: false,
    //            error: function (error) {
    //                console.log(error);
    //            },
    //            beforeSend: function () {
    //                showLoading();
    //            },
    //            success: function (result) {
    //                console.log(result);
    //            },
    //            complete: function () {
    //                hideLoading();
    //                showPage("PurchasePlanning/Index", null);
    //            }
    //        });
    //    }
    //}
}

function ButtonCancel_Click(s, e) {
    showPage("ProductionSchedule/Index", null);
}

// BUTTONS ACTION 
function AddNewDocument(s, e) {

    var data = {
        id: 0
    };

    showPage("ProductionSchedule/ProductionScheduleFormEditPartial", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    //showPage("PurchasePlanning/PurchasePlanningCopy", { id: $("#id_purchasePlanning").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionSchedule").val()
        };
        Update(true);
        //showForm("ProductionSchedule/Approve", data);
    }, "¿Desea aprobar la programación?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionSchedule").val()
        };
        showForm("ProductionSchedule/Autorize", data);
    }, "¿Desea autorizar la programación?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionSchedule").val()
        };
        showForm("ProductionSchedule/Protect", data);
    }, "¿Desea cerrar la programación?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionSchedule").val()
        };
        showForm("ProductionSchedule/Cancel", data);
    }, "¿Desea anular la programación?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_productionSchedule").val()
        };
        showForm("ProductionSchedule/Revert", data);
    }, "¿Desea reversar la programación?");
}

function ShowDocumentHistory(s, e) {
}

function PrintDocument(s, e) {

    $.ajax({
        url: "ProductionSchedule/ProductionScheduleReport",
        type: "post",
        data: { id: $("#id_productionSchedule").val() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#maincontent").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

// PRODUCTION SCHEDULE REQUESTS DETAILS

function AddNewProductionScheduleRequestDetail(s, e) {
    gvProductionScheduleRequestDetail.AddNewRow();
}

function RemoveProductionScheduleRequestDetail(s, e) {

    //gvPurchaseOrderEditFormDetails.GetSelectedFieldValues("id_item", function (values) {
    //    var selectedRows = [];

    //    for (var i = 0; i < values.length; i++) {
    //        selectedRows.push(values[i]);
    //    }

    //    $.ajax({
    //        url: "PurchaseOrder/PurchaseOrderDetailsDeleteSeleted",
    //        type: "post",
    //        data: { ids: selectedRows },
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            //showLoading();
    //        },
    //        success: function () {
    //            // TODO: 
    //        },
    //        complete: function () {
    //            gvPurchaseOrderEditFormDetails.PerformCallback();
    //            UpdateOrderTotals();
    //        }
    //    });
    //});

}

function RefreshProductionScheduleRequestDetail(s, e) {
    gvProductionScheduleRequestDetail.UnselectRows();
    gvProductionScheduleRequestDetail.PerformCallback();
}

// PRODUCTION SCHEDULE REQUESTS DETAILS

function AddNewProductionScheduleScheduleDetail(s, e) {
    gvProductionScheduleScheduleDetail.AddNewRow();
}

function RemoveProductionScheduleScheduleDetail(s, e) {

    //gvPurchaseOrderEditFormDetails.GetSelectedFieldValues("id_item", function (values) {
    //    var selectedRows = [];

    //    for (var i = 0; i < values.length; i++) {
    //        selectedRows.push(values[i]);
    //    }

    //    $.ajax({
    //        url: "PurchaseOrder/PurchaseOrderDetailsDeleteSeleted",
    //        type: "post",
    //        data: { ids: selectedRows },
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            //showLoading();
    //        },
    //        success: function () {
    //            // TODO: 
    //        },
    //        complete: function () {
    //            gvPurchaseOrderEditFormDetails.PerformCallback();
    //            UpdateOrderTotals();
    //        }
    //    });
    //});

}

function RefreshProductionScheduleScheduleDetail(s, e) {
    gvProductionScheduleScheduleDetail.UnselectRows();
    gvProductionScheduleScheduleDetail.PerformCallback();
}

// UPDATE VIEW

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        $(".alert-success").fadeTo(3000, 0.45, function () {
            $(".alert-success").alert('close');
        });
        //setTimeout(function () {
        //    $(".alert-success").alert('close');
        //}, 2000);
    }
    //if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
    //    setTimeout(function () {
    //        $(".alert-success").alert('close');
    //    }, 2000);
    //}
}

function UpdateView() {
    var id = parseInt($("#id_productionSchedule").val());
    var codeState = parseInt($("#codeState").val());
    //console.log("codeState: " + codeState);
    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(codeState == "01");//Pendiente
    btnCopy.SetEnabled(false);//id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "ProductionSchedule/Actions",
        type: "post",
        data: { id: id },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            btnApprove.SetEnabled(result.btnApprove);
            btnAutorize.SetEnabled(result.btnAutorize);
            btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            //hideLoading();
        }
    });

    // HISTORY BUTTON
    btnHistory.SetEnabled(id !== 0);

    // PRINT BUTTON
    btnPrint.SetEnabled(id !== 0);
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "ProductionSchedule/InitializePagination",
        type: "post",
        data: { id_purchasePlanning: $("#id_productionSchedule").val() },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            $("#pagination").attr("data-max-page", result.maximunPages);
            current_page = result.currentPage;
        },
        complete: function () {
        }
    });
    $('.pagination').current_page = current_page;
}

function init() {
    UpdatePagination();

    AutoCloseAlert();
}

$(function () {

    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});