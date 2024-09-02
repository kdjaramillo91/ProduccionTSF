
// DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);
    var errorMessage = "";
     
    $("#GridMessageErrorBO").hide();


    //}
    if (!valid) {
        UpdateTabImage({ isValid: false }, "tabOportunity");
        UpdateTabImage({ isValid: false }, "tabPlanification");
    }
    
    if (gvBusinessOportunityNotes.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabOportunity");
        valid = false;
        errorMessage += "Se está editando detalle de notas en la pestaña de oportunidad";
    }

    if (gvBusinessOportunityPlanningDetails.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabPlanification");
        valid = false;
        errorMessage += "</br>- Se está editando detalle de la planificación";
        
    }

    if (gvBusinessOportunityPhases.cpRowsCount == 0 || gvBusinessOportunityPhases.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabPhases");
        valid = false;

        if (gvBusinessOportunityPhases.cpRowsCount == 0) {
            errorMessage += "</br>- No hay detalle en las Etapas de la Oportunidades"
        }
        if (gvBusinessOportunityPhases.IsEditing()) {
            errorMessage += "</br>- Se está editando detalle de las Etapas de la Oportunidad";
        }
    }

    if (gvBusinessPartners.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabPartner");
        valid = false;
        errorMessage += "</br>- Se está editando detalle de los Socios de Negocios";
    }

    if (gvBusinessOportunityCompetitions.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabCompetition");
        valid = false;
        errorMessage += "</br>- Se está editando detalle de los Competidores";
    }

    if (gvAttachedDocuments.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabAttachedDocument");
        valid = false;
        errorMessage += "</br>- Se está editando detalle de los documentos adjuntos";
    }

    //console.log("id_documentType.GetValue(): " + id_documentType.GetValue());

    if (valid) {
        //var id = $("#id_businessOportunity").val();
        var id = parseInt(document.getElementById("id_businessOportunity").getAttribute("idBusinessOportunity"));
        var id_documentTypeCurrent = id_documentType.GetValue();
        //var endDateCurrent = JSON.stringify($("#endDateCurrent").val());

        var data = "id=" + id + "&" +
                   "approve=" + approve + "&" +
                   "id_documentTypeCurrent=" + id_documentTypeCurrent + "&" +
                   //"endDateCurrent=" + endDateCurrent + "&" +
                   $("#formEditBusinessOportunity").serialize();

        var url = (id === 0) ? "BusinessOportunity/BussinesOportunityPartialAddNew"
                               : "BusinessOportunity/BussinesOportunityPartialUpdate";
        showPage(url, data);
    }
    else {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorBO.SetText(msgErrorAux);
        $("#GridMessageErrorBO").show();
    }

}

function ButtonUpdate_Click(s, e) {

    Update(false);
    UpdateView();
}

function ButtonUpdateClose_Click(s, e) {
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    //if (valid) {
    //    var id = $("#id_order").val();

    //    var data = "id=" + id + "&" + $("#formEditPurchaseOrder").serialize();

    //    var url = (id === "0") ? "PurchaseOrder/PurchaseOrderPartialAddNew"
    //                           : "PurchaseOrder/PurchaseOrderPartialUpdate";

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
    //                showPage("PurchaseOrder/Index", null);
    //            }
    //        });
    //    }
    //}
}

function ButtonCancel_Click(s, e) {
    $("#GridMessageErrorBO").hide();
    showPage("BusinessOportunity/Index", null);
}

//PURCHASE ORDER BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0
        //type: type
    };
    showPage("BusinessOportunity/FormEditBusinessOportunity", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    //ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    //showPage("PurchaseOrder/PurchaseOrderCopy", { id: $("#id_order").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar la oportunidad?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Approve", data);
    //}, "¿Desea aprobar el documento?");
}

function AutorizeDocument(s, e) {
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Autorize", data);
    //}, "¿Desea autorizar el documento?");
}

function ProtectDocument(s, e) {
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Protect", data);
    //}, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_businessOportunity").val()
        };
        showForm("BusinessOportunity/Cancel", data);
    }, "¿Desea anular la oportunidad?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Cancel", data);
    //}, "¿Desea anular el documento?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_businessOportunity").val()
        };
        showForm("BusinessOportunity/Revert", data);
    }, "¿Desea reversar la oportunidad?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Revert", data);
    //}, "¿Desea reversar el documento?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {
    //var _url = "PurchaseOrder/PurchaseOrdersReport";
    //var id = $("#id_purchaseOrder").val();

    //if (!(id == 0) && !(id == null)) {
    //    var ids = [id];
    //    $.ajax({
    //        url: _url,
    //        type: "post",
    //        data: { ids: ids },
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
    //}

}

// TABCONTROL TABCAHNCGED

var gv = null;
var removeMethodAction = "";

function TabControl_ActiveTabChanged(s, e) {
    
    var activeTab = tabControl.GetActiveTab();

    if (activeTab === undefined || activeTab === null) {
        gv = null;
        return;
    }

    if(activeTab.name === "tabOportunity") {
        gv = gvBusinessOportunityNotes;
        removeMethodAction = "DeleteSelectedBusinessOportunityNotes";
    } else if(activeTab.name === "tabPlanification") {
        gv = gvBusinessOportunityPlanningDetails;
        removeMethodAction = "DeleteSelectedPlanningDetails";
    } else if(activeTab.name === "tabPhases") {
        gv = gvBusinessOportunityPhases;
        removeMethodAction = "DeleteSelectedOportunityPhases";
    }

}

// DETAILS ACTIONS BUTTONS

function AddNewDetail(s, e) {
    if(gv !== null) {
        gv.AddNewRow();
    }
}

function RemoveDetail(s, e) {
    gv.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            $.ajax({
                url: "BusinessOportunity/" + removeMethodAction,
                type: "post",
                data: { ids: selectedRows },
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    //$("#maincontent").html(result);
                },
                complete: function () {
                    gv.PerformCallback();
                    gv.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function RefreshDetail(s, e) {
    if (gv !== null) {
        gv.Refresh();
    }
}


//BUSINESS OPORTUNITY DOCUMENT TYPE

function BusinessOportunityDocumentType_SelectedIndexChanged(s, e) {
    //id_documentState.ClearItems();
    //id_documentState.SetValue(null);

    id_businessPartner.ClearItems();
    id_businessPartner.SetValue(null);

    $.ajax({
        url: "BusinessOportunity/DocumentTypeDetailsData",//ItemPlaningDetailsData",
        type: "post",
        data: { id_documentType: s.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if (result !== null) {

                //var arrayFieldStr = [];
                //arrayFieldStr.push("name");
                //UpdateDetailObjects(id_documentState, result.documentStates, arrayFieldStr);
                //$("#codeBusinessOportunityDocumentType").val(result.codeBusinessOportunityDocumentType);
                //var arrayFieldStr = [];
                //arrayFieldStr.push("fullname_businessName");
                //UpdateDetailObjects(id_businessPartner, result.businessPartners, arrayFieldStr);

                id_businessPartner.PerformCallback();

                UpdateBusinessOportunityAmount();
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function BusinessOportunityBusinessPartner_SelectedIndexChanged(s, e) {
   
    var data = { 
        id_businessPartner: s.GetValue(),
        id_documentType: id_documentType.GetValue(),
        contactPerson: contactPerson.GetText(),
        locationName: locationName.GetText()
    };

    $.ajax({
        url: "BusinessOportunity/BusinessPartnerDetailsData",//DocumentTypeDetailsData",
        type: "post",
        data: data,
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

                //var arrayFieldStr = [];
                //arrayFieldStr.push("name");
                //UpdateDetailObjects(id_documentState, result.documentStates, arrayFieldStr);

                contactPerson.SetText(result.contactPerson);
                total.SetValue(result.total);
                locationName.SetText(result.locationName);

                gvBusinessPartners.PerformCallback();

            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// DETAILS ACTIONS NOTES

function AddNewDetailNote(s, e) {
    gvBusinessOportunityNotes.AddNewRow();
    //AddNew(s, e);
}

function RemoveDetailNote(s, e) {
    //Remove(s, e);
}

function RefreshDetailNote(s, e) {
    //Refresh(s, e);
    gvBusinessOportunityNotes.UnselectRows();
    gvBusinessOportunityNotes.PerformCallback();
}


// ACTIONS PLANING

function Changed_EstimatedProfitOrAmount(s, e) {

    var estimatedProfitAux = parseFloat(estimatedProfit.GetValue());
    estimatedProfitAux = isNaN(estimatedProfitAux) ? 0 : estimatedProfitAux;

    var amountAux = parseFloat(amount.GetValue());
    amountAux = isNaN(amountAux) ? 0 : amountAux;

    var grossProfitAux = (amountAux * estimatedProfitAux)/100;

    grossProfit.SetValue(grossProfitAux);
    totalAmount.SetValue(amountAux);
}

// DETAILS ACTIONS PLANING DETAILS

var id_itemIniAux = null;
var id_personIniAux = null;
var id_priceListInitAux = null;
var id_documentIniAux = 0;

function OnItemValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var data = {
            id_itemNew: s.GetValue(),
            id_personNew: id_person.GetValue(),
            id_documentNew: id_documentSalesPurchase.GetValue()
        };
        if (data.id_itemNew != id_itemIniAux || data.id_personNew != id_personIniAux ||
            data.id_documentNew != id_documentIniAux) {
            $.ajax({
                url: "BusinessOportunity/ItsRepeatedPlaningDetail",//ItsRepeatedLiquidation",
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
                    if (result !== null) {
                        if (result.itsRepeated == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                        } else {
                            id_itemIniAux = 0
                            id_personIniAux = 0
                            id_documentIniAux = 0
                        }
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }

    }
}

function AddNewPlaningDetail(s, e) {
    gvBusinessOportunityPlanningDetails.AddNewRow();
    //AddNew(s, e);
}

function RemovePlaningDetail(s, e) {
    //Remove(s, e);
}

function RefreshPlaningDetail(s, e) {
    //Refresh(s, e);
    gvBusinessOportunityPlanningDetails.UnselectRows();
    gvBusinessOportunityPlanningDetails.PerformCallback();
}

function Changed_QuantityOrPriceDetail(s, e) {

    var quantityAux = parseFloat(quantity.GetValue());
    quantityAux = isNaN(quantityAux) ? 0 : quantityAux;

    var priceAux = parseFloat(price.GetValue());
    priceAux = isNaN(priceAux) ? 0 : priceAux;

    var totalItemDetailAux = (quantityAux * priceAux);

    totalItemDetail.SetValue(totalItemDetailAux);

}

function BusinessPlanningDetailPerson_BeginCallback(s, e) {
    e.customArgs["id_person"] = id_person.GetValue();
}

function BusinessPlanningDetailPerson_EndCallback(s, e) {
    id_person.SetValue(id_personIniAux);
}

function BusinessPlanningDetailPriceListBeginCallback(s, e) {
    e.customArgs["id_person"] = id_person.GetValue();
    e.customArgs["id_priceList"] = s.GetValue();
}

function BusinessPlanningDetailPriceList_EndCallback(s, e) {
    id_priceList.SetValue(id_priceListIniAux);
}

function BusinessPlanningDetailItem_BeginCallback(s, e) {
    e.customArgs["id_item"] = id_item.GetValue();
}

function BusinessPlanningDetailItem_EndCallback(s, e) {
    id_item.SetValue(id_itemIniAux);
}

function ItemCombo_OnInit(s, e) {

    id_itemIniAux = s.GetValue();
    id_personIniAux = id_person.GetValue();
    id_priceListIniAux = id_priceList.GetValue();
    id_documentIniAux = id_documentSalesPurchase.GetValue();

    id_item.PerformCallback();
    
    id_person.PerformCallback();

    id_priceList.PerformCallback();


    //var data = {
    //    id_itemCurrent: s.GetValue(),
    //    id_personCurrent: id_person.GetValue(),
    //    id_documentCurrent: id_documentSalesPurchase.GetValue(),
    //    id_documentType: id_documentType.GetValue()
    //};

    ////if (data.id_itemCurrent != null) s.SetEnabled(false);

    //$.ajax({
    //    url: "BusinessOportunity/PlaningDetails",//PackingMaterialDetails",
    //    type: "post",
    //    data: data,
    //    async: false,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //        //id_metricUnit.SetValue(null);
    //    },
    //    beforeSend: function () {
    //        //showLoading();
    //    },
    //    success: function (result) {
    //        //id_company
    //        //var arrayFieldStr = [];
    //        //arrayFieldStr.push("masterCode");
    //        //arrayFieldStr.push("name");
    //        //arrayFieldStr.push("MetricUnitCode");
    //        //UpdateDetailObjects(id_item, result.items, arrayFieldStr);
    //        //id_item.SetValue(id_itemIniAux);
    //        id_item.PerformCallback();
    //        //arrayFieldStr = [];
    //        //arrayFieldStr.push("fullname_businessName");
    //        //UpdateDetailObjects(id_person, result.persons, arrayFieldStr);
    //        id_person.PerformCallback();
    //        //var personAux = id_person.FindItemByValue(id_personIniAux);
    //        //if (personAux == null && result.salesOrder.id != null) s.AddItem(result.salesOrder.name, result.salesOrder.id);
    //        //s.SetValue(result.salesOrder.id);

    //        //id_person.SetValue(null);
    //        //id_person.SetValue(id_personIniAux);

    //        //arrayFieldStr = [];
    //        //arrayFieldStr.push("number");
    //        //UpdateDetailObjects(id_document, result.documents, arrayFieldStr);
    //        //id_document.SetValue(null);
    //        //id_document.SetValue(id_documentIniAux);

    //    },
    //    complete: function () {
    //        //hideLoading();
    //    }
    //});
}

function DetailsItemCombo_SelectedIndexChanged(s, e) {
    DetailsUpdateItemInfo({
        id_item: s.GetValue(),
        id_documentType: id_documentType.GetValue(),
        id_priceList: id_priceList.GetValue()
    });
}

function DetailsUpdateItemInfo(data) {

    metricUnit.SetText("");
    size.SetText("");
    itemTypeCategory.SetText("");
    //id_document.ClearItems();
    //id_document.SetValue(null);

    if (data.id_item === null) {
        //masterCode.SetText("");

        return;
    }

    //masterCode.SetText("");
    //metricUnit.SetText("");

    if (id_item.GetValue() != null) {
        {

            ItemPlaningDetailsData(data);
        }
        //else {
        //ValidateDetailPackingMaterial();
        //}
    }
}

function ItemPlaningDetailsData(data) {
    $.ajax({
            url: "BusinessOportunity/ItemPlaningDetailsData",//ItemPackingMaterialDetailsData",
            type: "post",
            data: data,
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
                    //masterCodePackingMaterial.SetText(result.ItemDetailData.masterCode);
                    metricUnit.SetText(result.metricUnit);
                    size.SetText(result.size);
                    itemTypeCategory.SetText(result.itemTypeCategory);

                    price.SetValue(result.price);
                    Changed_QuantityOrPriceDetail();

                    //var arrayFieldStr = [];
                    //arrayFieldStr.push("number");
                    //UpdateDetailObjects(id_document, result.documents, arrayFieldStr);
                }
            },
            complete: function () {
                //hideLoading();
                //ValidateDetailPackingMaterial();
            }
        });
}

function DetailsPersonCombo_SelectedIndexChanged(s, e) {
    id_priceList.SetValue(null);
    id_priceList.PerformCallback();
}

function DetailsPriceListCombo_SelectedIndexChanged(s, e) {
    ItemPlaningDetailsData({
        id_item: id_item.GetValue(),
        id_documentType: id_documentType.GetValue(),
        id_priceList: s.GetValue()
    });
    }


function UpdateBusinessOportunityAmount() {
    $.ajax({
        url: "BusinessOportunity/UpdateBusinessOportunityAmount",
        type: "post",
        data: null,
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
                if (result.amount > 0) {
                    amount.SetValue(result.amount);
                    amount.SetEnabled(false);
                    Changed_EstimatedProfitOrAmount(amount, amount);
                } else {
                    amount.SetEnabled(true);
                }

                gvBusinessPartners.PerformCallback();

            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function BusinessOportunityPlanningDetail_OnEndCallback(s, e) {
    UpdateBusinessOportunityAmount();
}

//function BusinessOportunityPlanningDetail_OnBeginCallback(s, e) {
//    e.customArgs["codeBusinessOportunityDocumentType"] = $("#codeBusinessOportunityDocumentType").val();
//}

// DETAILS ACTIONS PHASE DETAILS

function AddNewPhaseDetail(s, e) {
    gvBusinessOportunityPhases.AddNewRow();
    //AddNew(s, e);
}

function RemovePhaseDetail(s, e) {
    //Remove(s, e);
}

function RefreshPhaseDetail(s, e) {
    //Refresh(s, e);
    gvBusinessOportunityPhases.UnselectRows();
    gvBusinessOportunityPhases.PerformCallback();
}

function OnPhaseInit(s, e) {

    var data = {
        id_businessOportunityDocumentTypePhaseCurrent: s.GetValue(),
        idCurrent: gvBusinessOportunityPhases.cpEditingRowIDPhaseDetail,
        id_documentType: id_documentType.GetValue()
    };

    //if (data.id_itemCurrent != null) s.SetEnabled(false);

    $.ajax({
        url: "BusinessOportunity/PhaseDetails",//PlaningDetails",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //s.SetEnabled(result.enabled);
            startDatePhase.SetEnabled(result.enabled);
            endDatePhase.SetEnabled(result.enabled);
            //id_phase
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_businessOportunityDocumentTypePhase, result.businessOportunityDocumentTypePhases, arrayFieldStr);

            //potentialAmount.SetValue(result.potentialAmount);
            potentialAmount.SetValue(amount.GetValue());

        },
        complete: function () {
            //hideLoading();
        }
    });
}

function DetailsPhaseCombo_SelectedIndexChanged(s, e) {
    DetailsUpdatePhaseInfo({
        id_businessOportunityDocumentTypePhase: s.GetValue(),
        strPotentialAmount: amount.GetValue(),

    });
}

function DetailsUpdatePhaseInfo(data) {

    //metricUnit.SetText("");
    //id_document.ClearItems();
    weightedAmount.SetValue(null);
    advance.SetValue(null);

    if (data.strPotentialAmount === null) {
        data.strPotentialAmount = 0;
    }

    if (data.id_businessOportunityDocumentTypePhase === null) {
        //masterCode.SetText("");

        return;
    }

    //masterCode.SetText("");
    //metricUnit.SetText("");

    if (id_businessOportunityDocumentTypePhase.GetValue() != null) {

        $.ajax({
            url: "BusinessOportunity/PhaseDetailsData",//ItemPlaningDetailsData",
            type: "post",
            data: data,
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
                    weightedAmount.SetValue(result.amount);
                    advance.SetValue(result.advance);
                }
            },
            complete: function () {
                //hideLoading();
                //ValidateDetailPackingMaterial();
            }
        });
    } 
    //else {
        //ValidateDetailPackingMaterial();
    //}
}

function ButtonUpdatePhase_Click(s, e) {
    var valid = true;

    if (gvBusinessOportunityPhaseAttachments.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabPhases");
        valid = false;
    }

    if (gvBusinessOportunityPhaseActivities.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabPhases");
        valid = false;
    }

    if (valid) {
        gvBusinessOportunityPhases.UpdateEdit();
    }
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

    //if (!valid) {
    //    UpdateTabImage({ isValid: false }, "tabOportunity");
    //}

    

    //if (valid) {
    //    var id = $("#id_businessOportunity").val();
    //    var data = "id=" + id + "&" + $("#formEditBusinessOportunity").serialize();
    //    var url = (id === "0") ? "BusinessOportunity/BussinesOportunityPartialAddNew"
    //                           : "BusinessOportunity/BussinesOportunityPartialUpdate";
    //    showForm(url, data);
    //}


}

function BtnCancelPhase_Click(s, e) {
    $("#GridMessageErrorPhase").hide();
    gvBusinessOportunityPhases.CancelEdit();
    
    //showPage("BusinessOportunity/Index", null);
}

function BusinessOportunityPhase_OnBeginCallback(s, e) {
    e.customArgs["id_businessOportunityPhase"] = $("#id_businessOportunityPhase").val();
}

function BusinessOportunityPhaseView_OnBeginCallback(s, e) {
    e.customArgs["id_businessOportunityPhaseView"] = $("#id_businessOportunityPhaseView").val();
}

function UpdateBusinessOportunityClosingPercentage() {
    $.ajax({
        url: "BusinessOportunity/UpdateBusinessOportunityClosingPercentage",
        type: "post",
        data: null,
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
                closingPercentage.SetValue(result.closingPercentage);
                //$("#endDateCurrent").val(result.endDateCurrent);
                //if (result.amount > 0) {
                //    amount.SetEnabled(false);
                //    Changed_EstimatedProfitOrAmount(amount, amount);
                //} else {
                //    amount.SetEnabled(true);
                //}

                //gvBusinessPartners.PerformCallback();

            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function BusinessOportunityPhase_OnEndCallback(s, e) {
    UpdateBusinessOportunityClosingPercentage();
}

function AttachedPhaseUploadComplete(s, e) {
    var userData = JSON.parse(e.callbackData);
    //console.log("userData: " + userData);
    $("#guidPhase").val(userData.guid);
    //console.log("userData.guid: " + userData.guid);
    //console.log("$('#guid').val(): " + $("#guid").val());
    $("#urlPhase").val(userData.url);
    //console.log("userData.url: " + userData.url);
    //console.log("$('#url').val(): " + $("#url").val());
    attachmentPhaseName.SetText(userData.filename);
}

var attachmentPhaseNameIniAux = null;

function AttachedPhaseName_OnInit(s, e) {

    attachmentPhaseNameIniAux = s.GetText();

}

function AttachedPhaseNameValidate(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Archivo Obligatorio";
    } else {
        var guid = $("#guidAttachmentPhase").val();
        if (guid === null || guid.length === 0) {
            e.isValid = false;
            e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
        } else {
            var url = $("#urlAttachmentPhase").val();
            if (guid === null || guid.length === 0) {
                e.isValid = false;
                e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
            } else {
                var data = {
                    attachmentPhaseNameNew: e.value
                };
                if (data.attachmentPhaseNameNew != attachmentPhaseNameIniAux) {
                    $.ajax({
                        url: "BusinessOportunity/ItsRepeatedAttachmentPhaseDetail",//ItsRepeatedLiquidation",
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
                            if (result !== null) {
                                if (result.itsRepeated == 1) {
                                    e.isValid = false;
                                    e.errorText = result.Error;
                                }
                            }
                        },
                        complete: function () {
                            //hideLoading();
                        }
                    });
                }
            }
        }
    }

}

// DETAILS ACTIONS PARTNERS

function AddNewPartnerDetail(s, e) {
    gvBusinessPartners.AddNewRow();
    //AddNew(s, e);
}

function RemovePartnerDetail(s, e) {
    //Remove(s, e);
}

function RefreshPartnerDetail(s, e) {
    //Refresh(s, e);
    gvBusinessPartners.UnselectRows();
    gvBusinessPartners.PerformCallback();
}

var id_partnerIniAux = null;

function BusinessOportunityPartner_BeginCallback(s, e) {
    //console.log("id_partnerIniAux: " + id_partnerIniAux);
    e.customArgs["id_partner"] = id_partnerIniAux;
}

function BusinessOportunityPartner_EndCallback(s, e) {
    id_partner.SetValue(id_partnerIniAux);
    if (id_partnerIniAux != null) s.SetEnabled(false);
}

function PartnerCombo_OnInit(s, e) {

    id_partnerIniAux = s.GetValue();
    //var data = {
    //    id_partner: id_partnerIniAux,
    //    id_documentType: id_documentType.GetValue()
    //};

    

    id_partner.PerformCallback();

    
    //$.ajax({
    //    url: "BusinessOportunity/PartnerDetails",//PhaseDetails",
    //    type: "post",
    //    data: data,
    //    async: false,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //        //id_metricUnit.SetValue(null);
    //    },
    //    beforeSend: function () {
    //        //showLoading();
    //    },
    //    success: function (result) {
    //        //id_partner
    //        var arrayFieldStr = [];
    //        arrayFieldStr.push("fullname_businessName");
    //        UpdateDetailObjects(id_partner, result.partners, arrayFieldStr);

    //    },
    //    complete: function () {
    //        //hideLoading();
    //    }
    //});
}

// DETAILS ACTIONS COMPETITIONS

function AddNewCompetitionDetail(s, e) {
    gvBusinessOportunityCompetitions.AddNewRow();
    //AddNew(s, e);
}

function RemoveCompetitionDetail(s, e) {
    //Remove(s, e);
}

function RefreshCompetitionDetail(s, e) {
    //Refresh(s, e);
    gvBusinessOportunityCompetitions.UnselectRows();
    gvBusinessOportunityCompetitions.PerformCallback();
}

var id_competitorIniAux = null;

function BusinessOportunityCompetitor_BeginCallback(s, e) {
    //console.log("id_partnerIniAux: " + id_partnerIniAux);
    e.customArgs["id_competitor"] = id_competitorIniAux;
}

function BusinessOportunityCompetitor_EndCallback(s, e) {
    id_competitor.SetValue(id_competitorIniAux);
    if (id_competitorIniAux != null) s.SetEnabled(false);
}

function CompetitorCombo_OnInit(s, e) {

    id_competitorIniAux = s.GetValue();
    
    id_competitor.PerformCallback();
}

// DETAILS ACTIONS ATTACHED DOCUMENTS

function AddNewAttachedDocumentDetail(s, e) {
    gvAttachedDocuments.AddNewRow();
    
    //AddNew(s, e);
}

function RemoveAttachedDocumentDetail(s, e) {
    //Remove(s, e);
}

function RefreshAttachedDocumentDetail(s, e) {
    //Refresh(s, e);
    gvAttachedDocuments.UnselectRows();
    gvAttachedDocuments.PerformCallback();
}

function AttachedUploadComplete(s, e) {
    var userData = JSON.parse(e.callbackData);
    $("#guid").val(userData.guid);
    $("#url").val(userData.url);
    attachmentName.SetText(userData.filename);
}

var attachmentNameIniAux = null;

function AttachedName_OnInit(s, e) {

    attachmentNameIniAux = s.GetText();

}

function AttachedNameValidate(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Archivo Obligatorio";
    } else {
        var guid = $("#guidAttachment").val();
        if (guid === null || guid.length === 0) {
            e.isValid = false;
            e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
        } else {
            var url = $("#urlAttachment").val();
            if (guid === null || guid.length === 0) {
                e.isValid = false;
                e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
            } else {
                var data = {
                    attachmentNameNew: e.value
                };
                if (data.attachmentNameNew != attachmentNameIniAux) {
                    $.ajax({
                        url: "BusinessOportunity/ItsRepeatedAttachmentDetail",//ItsRepeatedLiquidation",
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
                            if (result !== null) {
                                if (result.itsRepeated == 1) {
                                    e.isValid = false;
                                    e.errorText = result.Error;
                                }
                            }
                        },
                        complete: function () {
                            //hideLoading();
                        }
                    });
                }
            }
        }
    }

}

function gvAttachedDocumentsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnUpdate") {
        console.log("e.buttonID: " + e.buttonID);
        var valid = true;
        var validAttachmentFormUpLoad = ASPxClientEdit.ValidateEditorsInContainerById("attachment-form-upLoad", null, true);
        console.log("validAttachmentFormUpLoad: " + validAttachmentFormUpLoad);
        if (validAttachmentFormUpLoad) {
            UpdateTabImage({ isValid: true }, "tabAttachedDocument");
        } else {
            UpdateTabImage({ isValid: false }, "tabAttachedDocument");
            valid = false;
        }
        console.log("valid: " + valid);

        if (valid) {
            gvAttachedDocuments.UpdateEdit();
        }
    }
}

// UPDATE VIEW

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
     
    //var id_businessOportunityTmp = parseInt($("#id_businessOportunity").val());

    var id_businessOportunityTmp = parseInt(document.getElementById("id_businessOportunity").getAttribute("idBusinessOportunity"));
    //var id = parseInt($("#id_businessOportunity").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "BusinessOportunity/Actions",
        type: "post",
        data: { id: id_businessOportunityTmp },
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
     
    var tmpIdBusinessOportunity = parseInt(document.getElementById("id_businessOportunity").getAttribute("idBusinessOportunity"));

    var current_page = 1;
    $.ajax({
        url: "BusinessOportunity/InitializePagination",
        type: "post",
        data: { id_businessOportunity: tmpIdBusinessOportunity },
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

// MAIN FUNCTIONS

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
