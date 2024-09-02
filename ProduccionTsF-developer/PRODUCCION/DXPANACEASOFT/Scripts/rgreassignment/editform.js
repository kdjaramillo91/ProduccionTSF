
//DIALOG BUTTONS ACTIONS
function Update(approve) {
     
     var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabRemissionGuide", true);

     if (!valid) {
         UpdateTabImage({ isValid: false }, "tabRemissionGuide");
     }

     if (!isOwn.GetChecked()) {
         var valuePriceTmp = valuePrice.GetText();
         if (valuePriceTmp == null) { valuePriceTmp = 0; }
         var valueAdvanceTmp = advancePrice.GetText();
         if (valueAdvanceTmp == null) { valueAdvanceTmp = 0; }
         if (parseFloat(valueAdvanceTmp) > parseFloat(valuePriceTmp)) {
             advancePrice.Validate();
             return;
        }

    }

     //if (isExport.GetChecked()) {
     //    valid &= ASPxClientEdit.ValidateEditorsInContainer(null, "tabExport", true);
     //}

     if (gvDetails.cpRowsCount === 0 || gvDetails.IsEditing()) {
         UpdateTabImage({ isValid: false }, "tabDetails");
         valid &= false;
     }

     valid &= ASPxClientEdit.ValidateEditorsInContainer(null, "tabTransportation", true);



     if (/*gvDispatchMaterials.cpRowsCount === 0 || */gvDispatchMaterials.IsEditing()) {
         UpdateTabImage({ isValid: false }, "tabDispatchMaterials");
         valid &= false;
     }

     if (/*gvSecuritySeals.cpRowsCount === 0 || */gvSecuritySeals.IsEditing()) {
         UpdateTabImage({ isValid: false }, "tabSecuritySeals");
         valid &= false;
     }

     if (/*gvAssignedStaff.cpRowsCount === 0 || */gvAssignedStaff.IsEditing()) {
         UpdateTabImage({ isValid: false }, "tabAssignedStaff");
         valid &= false;
     }

     if (valid) {
          
         //var id = $("#id_remissionGuide").val();
         var id = parseInt(document.getElementById("id_remissionGuide").getAttribute("idremissionGuide"));
         var iddespachurehour = despachurehour.GetValue();
        var id_RemisionGuideReassignment = $("#id_RemisionGuideReassignment").val();
        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditRemissionGuide").serialize() + "&carRegistration=" + id_vehicle.GetText() + "&driverName=" + id_driver.GetText() + "&id_RemisionGuideReassignment=" + id_RemisionGuideReassignment + "&despachurehour=" + iddespachurehour + "&id_provider="+id_provider.GetValue();
        var url = (id === 0 || id === "0") ? "Logistics/RemissionGuidePartialAddNew"
                               : "Logistics/RemissionGuidePartialUpdate";

        showForm(url, data);
    }
}

function pricefreightrefresh() {
    var id = $("#id_remissionGuide").val();
    var iddespachurehour = despachurehour.GetValue();
    var id_RemisionGuideReassignment = $("#id_RemisionGuideReassignment").val();
    var data = "id=" + id + "&" + $("#formEditRemissionGuide").serialize() + "&carRegistration=" + id_vehicle.GetText() + "&driverName=" + id_driver.GetText() + "&id_RemisionGuideReassignment=" + id_RemisionGuideReassignment + "&despachurehour=" + iddespachurehour + "&id_provider=" + id_provider.GetValue();

    valuePrice.SetValue(0);

    $.ajax({
        url: "Logistics/pricefreightrefresh",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            // 
            valuePrice.SetValue(result.pricefreight);
        },
        complete: function () {
        }
    });
}

function CalculatePriceFreightAndRefresh() {
    valuePrice.SetValue(0);
    
    // 
    var id_tttTmp = id_TransportTariffType.GetValue();
    var id_fsfgTmp = id_FishingSiteRG.GetValue();

    if (id_tttTmp > 0 && id_fsfgTmp > 0) {
        var data = {
            id_FishingSite: id_fsfgTmp,
            id_TransportTariff: id_tttTmp
        };
        $.ajax({
            url: "Logistics/CalculatePriceFreightRefresh",
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                // 
                if (result != undefined) {
                    if ($("#errorMessage") != undefined) {
                        if (parseFloat(result.priceFreight) > 0) {
                            $("#errorMessage").hide();
                        }
                    }

                    valuePrice.SetValue(result.priceFreight);
                    advancePrice.Validate();
                }
            },
            complete: function () {
            }
        });
    }
    


}

function ButtonUpdate_Click(s, e) {
    Update(false);
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    showPage("Logistics/Index", null);
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    //var data = {
    //    id: 0
    //};

    //showPage("Logistics/FormEditRemissionGuide", data);
    // 
    showPage("Logistics/Index", null);
    AddNewGuideRemissionFromPurchaseOrder();
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    showPage("Logistics/RemissionGuideCopy", { id: $("#id_remissionGuide").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar la guía de remisión?");

    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_remissionGuide").val()
    //    };
    //    showForm("Logistics/Approve", data);
    //}, "¿Desea aprobar el documento?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_remissionGuide").val()
        };
        showForm("Logistics/Autorize", data);
    }, "¿Desea autorizar la guía de remisión?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_remissionGuide").val()
        };
        showForm("Logistics/Protect", data);
    }, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_remissionGuide").val()
        };
        showForm("Logistics/Cancel", data);
    }, "¿Desea anular la guía de remisión?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_remissionGuide").val()
        };
        showForm("Logistics/Revert", data);
    }, "¿Desea reversar la guía de remisión?");
}

function ShowDocumentHistory(s, e) {

}


function PrintDocumentAll(s, e) {
    var lstWarehouse = [];
    var isOwnTransTmp = $("#isOwnTransport").val();
    var data2 = {
        id_remissionGuide: $("#id_remissionGuide").val()
    };

    // 
    if (isOwnTransTmp == "NO")
    {
        // 
        $.ajax({
            url: 'Logistics/GetWarehouseListFromDispatchMaterial',
            data: data2,
            async: false,
            cache: false,
            type: 'POST',
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                // 
                if (result != null && result != undefined) {
                    lstWarehouse = result;
                }
            },
            complete: function () {
                hideLoading();
            }
        });
        var data = {
            ReportName: "",
            ReportDescription: "",
            ListReportParameter: []
        };
        if (lstWarehouse != null && lstWarehouse.length > 0) {
            for (var j = 0; j < lstWarehouse.length; j++) {
                var data3 = {
                    id_rg: $("#id_remissionGuide").val(),
                    id_warehouse: lstWarehouse[j].id,
                    codeReport: "GRDM1"
                };
                $.ajax({
                    url: 'Logistics/PrintRemissionGuideReports',
                    data: data3,
                    async: false,
                    cache: false,
                    type: 'POST',
                    beforeSend: function () {
                        showLoading();
                    },
                    success: function (result) {
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
    }


    if (!isOwn.GetChecked()) {

        var data1 = { id_rg: $("#id_remissionGuide").val(), codeReport: "GRVCR" };

        $.ajax({
            url: 'Logistics/PrintRemissionGuideReports',
            data: data1,
            async: true,
            cache: false,
            type: 'POST',
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
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
        var data2 = { id_rg: $("#id_remissionGuide").val(), codeReport: "GRVSR" };
        $.ajax({
            url: 'Logistics/PrintRemissionGuideReports',
            data: data2,
            async: true,
            cache: false,
            type: 'POST',
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
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
        //var data = { ReportName: "RemisionGuideReportViaticoCustomized", ReportDescription: "Guia Remision - Viaticos Caja Chica", ListReportParameter: [] }; //mia


        //$.ajax({
        //    url: 'Logistics/RemissionGuideReportFilter',
        //    data: data,
        //    async: true,
        //    cache: false,
        //    type: 'POST',
        //    beforeSend: function () {
        //        showLoading();
        //    },
        //    success: function (response) {


        //        try {
        //            if (response.isvalid) {
        //                var reportModel = response.reportModel;
        //                var url = 'Report/Index?reportModel=' + reportModel;
        //                newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
        //                newWindow.focus();
        //                hideLoading();
        //            }
        //        }
        //        catch (err) {
        //            hideLoading();
        //        }

        //    },
        //    complete: function () {
        //        hideLoading();
        //    }
        //});

        //data = { ReportName: "RemisionGuideReportViaticoPersona", ReportDescription: "Viaticos Personal", ListReportParameter: [] };

        //$.ajax({
        //    url: 'Logistics/RemissionGuideReportFilter',
        //    data: data,
        //    async: true,
        //    cache: false,
        //    type: 'POST',
        //    beforeSend: function () {
        //        showLoading();
        //    },
        //    success: function (response) {
        //        try {
        //            if (response.isvalid) {
        //                var reportModel = response.reportModel;
        //                var url = 'Report/Index?reportModel=' + reportModel;
        //                newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
        //                newWindow.focus();
        //                hideLoading();
        //            }
        //        }
        //        catch (err) {
        //            hideLoading();
        //        }
        //    },
        //    complete: function () {
        //        hideLoading();
        //    }
        //});
    }

    var tab = tabControl.GetTabByName("tabDispatchMaterials");

    if (printIceRep.GetChecked() && tab.GetVisible()) {
        data = { ReportName: "RemisionGuideReporthielo", ReportDescription: "Orden de Hielo", ListReportParameter: [] };

        $.ajax({
            url: 'Logistics/RemissionGuideReportFilter',
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
    }
}

function PrintDocument(s, e) {
    var data = { id_rg: $("#id_remissionGuide").val() };

    $.ajax({
        url: 'Logistics/PrintRemissionGuideReport',
        data: data,
        async: true,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
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

//Cancelación de Guía de Remisión
function CancelRG(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_remissionGuide").val()
        };
        showForm("Logistics/CancelRG", data);
    }, "¿Desea Cancelar la guía de remisión?");
}

function ShowReasignacion(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_remissionGuide").val()
        };
        showPage("Logistics/RemissionGuideResignacion", data);
    }, "¿Desea Reasignar la guía de remisión?");
}

// COMMOM BUTTONS ACTIONS

function AddNew(s, e) {
    if (activeGridView !== null && activeGridView !== undefined) {
        activeGridView.AddNewRow();
    }
}

function Remove(s, e) {
    var key = "id_item";

    if (activeGridView === gvSecuritySeals) {
        key = "number";
    } else if (activeGridView === gvAssignedStaff) {
        key = "id_person";
    }

    activeGridView.GetSelectedFieldValues(key, function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var url = "Logistics/DeleteSelectedRemissionGuideDetails";
        if (activeGridView === gvDispatchMaterials) {
            url = "Logistics/DeleteSelectedRemissionGuideDispatchMaterials";
        } else if (activeGridView === gvSecuritySeals) {
            url = "Logistics/DeleteSelectedRemissionGuideSecuritySeals";
        } else if (activeGridView === gvAssignedStaff) {
            url = "Logistics/DeleteSelectedRemissionGuideAssignedStaff";
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
            success: function () {
                // TODO: 
            },
            complete: function () {
                activeGridView.PerformCallback();
            }
        });

    });
}

function Refresh(s, e) {
    if (activeGridView !== null && activeGridView !== undefined) {
        activeGridView.PerformCallback();
    }
}

// DETAILS BUTTONS ACTIONS

function AddNewDetail(s, e) {
    AddNew(s, e);
}

function RemoveDetail(s, e) {
    Remove(s, e);
}

function RefreshDetail(s, e) {
    Refresh(s, e);
}

// DISPATCH MATERIALS BUTTONS ACTIONS

function AddNewDispatchMaterial(s, e) {
    AddNew(s, e);
}

function RemoveDispatchMaterial(s, e) {
    Remove(s, e);
}

function RefreshDispatchMaterial(s, e) {
    Refresh(s, e);
}


// SECURITY SEALS BUTTONS ACTIONS

function AddNewSecuritySeal(s, e) {
    AddNew(s, e);
}

function RemoveSecuritySeal(s, e) {
    Remove(s, e);
}

function RefreshSecuritySeal(s, e) {
    Refresh(s, e);
}

// ASSIGNED STAFF BUTTONS ACTIONS

function AddNewAssignedStaff(s, e) {
    AddNew(s, e);
}

function RemoveAssignedStaff(s, e) {
    Remove(s, e);
}

function RefreshAssignedStaff(s, e) {
    Refresh(s, e);
}

function InitAssignedPersonTravelType(s, e) {
    if (e.value == null) {
        s.SetSelectedIndex(0);
    }
}

function InitSecuritySealTravelType(s, e) {
    if (e.value == null) {
        s.SetSelectedIndex(0);
    }
}
// SELECTION

var customCommand = "";

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //} else
    // 
    var codeDocumentState = $("#codeDocumentState").val();
    if (codeDocumentState != "01" && codeDocumentState != "") {
        if (s.GetEditor("number") !== null && s.GetEditor("number") !== undefined) {
            s.GetEditor("number").SetEnabled(customCommand === "ADDNEWROW");
        } else if (s.GetEditor("id_person") !== null && s.GetEditor("id_person") !== undefined) {
            s.GetEditor("id_person").SetEnabled(customCommand === "ADDNEWROW");
        }
    }
    
    if (codeDocumentState == "01") {
        if (s == gvDetails || activeGridView == gvDetails)
        {
            gvDispatchMaterials.PerformCallback();
        }
        CalculatePriceFreightAndRefresh();
    }
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {

    if (tabControl.GetActiveTab().name === "tabDetails") {
        activeGridView = gvDetails;
        //pricefreightrefresh();
    } else if (tabControl.GetActiveTab().name === "tabDispatchMaterials") {
        activeGridView = gvDispatchMaterials;
        //pricefreightrefresh();
    }

    if (activeGridView === null)
        return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + activeGridView.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = activeGridView.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    var codeDocumentState = $("#codeDocumentState").val();

    if (codeDocumentState == "01") {
        var lblInfo = null;
        var lnkSelectAllRows = "";
        var lnkClearSelection = "";

        if (activeGridView === gvDetails) {
            lblInfo = $("#lblInfoDetails");
            lnkSelectAllRows = "lnkSelectAllRowsDetails";
            lnkClearSelection = "lnkClearSelectionDetails";
        } else if (activeGridView === gvDispatchMaterials) {
            lblInfo = $("#lblInfoDispatchMaterials");
            lnkSelectAllRows = "lnkSelectAllRowsDispatchMaterials";
            lnkClearSelection = "lnkClearSelectionDispatchMaterials";
        } else if (activeGridView === gvSecuritySeals) {
            lblInfo = $("#lblInfoSecurotySeals");
            lnkSelectAllRows = "lnkSelectAllRowsSecuritySeals";
            lnkClearSelection = "lnkClearSelectionSecuritySeals";
        } else if (activeGridView === gvAssignedStaff) {
            lblInfo = $("#lblInfoAssigendStaff");
            lnkSelectAllRows = "lnkSelectAllRowsAssigendStaff";
            lnkClearSelection = "lnkClearSelectionAssigendStaff";
        }

        if (lblInfo !== null && lblInfo !== undefined) {
            lblInfo.html(text);
        }

        SetElementVisibility(lnkSelectAllRows, activeGridView.GetSelectedRowCount() > 0 && activeGridView.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility(lnkClearSelection, activeGridView.GetSelectedRowCount() > 0);

        btnRemoveDetail.SetEnabled(activeGridView.GetSelectedRowCount() > 0);

    }

    

    
}

function GetSelectedFilteredRowCount() {
    return activeGridView.cpFilteredRowCountWithoutPage + activeGridView.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

// REMISSION GUIDE TRANSPORTATION FUNCTIONS
function OnTransportTariffTypeInit(s, e) {
    // 
    //id_vehicle.PerformCallback();
    //mark.SetValue(null);
    //model.SetValue(null);
    //id_provider.SetValue(null);
    //var idSTtmp = id_shippingType.GetValue();
    //if (idSTtmp != null && idSTtmp != undefined) {
    //    if (id_TransportTariffType.length > 0) {
    //        id_TransportTariffType.SetSelectedIndex(0);
    //        pricefreightrefresh();
    //    }
    //}
    CalculatePriceFreightAndRefresh();
    //pricefreightrefresh();
}

function OnTransportTariffEndCallBack(s, e) {
    // 
    id_vehicle.PerformCallback();
    mark.SetValue(null);
    model.SetValue(null);
    id_provider.SetValue(null);
    //var idSTtmp = id_shippingType.GetValue();
    //if (idSTtmp != null && idSTtmp != undefined) {
    //    if (id_TransportTariffType.length > 0) {
    //        id_TransportTariffType.SetSelectedIndex(0);
    //        pricefreightrefresh();
    //    }
    //}
    
}

function OnIsOwn_CheckedChanged(s, e) {

    if (isOwn.GetChecked()) {
        id_TransportTariffType.SetEnabled(false);
        id_provider.SetEnabled(false);
        id_vehicle.SetEnabled(false);
        id_driver.SetEnabled(false);
        advancePrice.SetEnabled(false);
        carRegistrationThird.SetEnabled(true);
        driverNameThird.SetEnabled(true);
        despachureDateLabel.SetText("Fecha Llegada:");
    }
    else {
        id_provider.SetEnabled(true);
        id_TransportTariffType.SetEnabled(true);
        id_vehicle.SetEnabled(true);
        id_driver.SetEnabled(true);
        advancePrice.SetEnabled(true);
        carRegistrationThird.SetEnabled(false);
        driverNameThird.SetEnabled(false);
        carRegistrationThird.SetText("");
        driverNameThird.SetText("");
        despachureDateLabel.SetText("Fecha Despacho:");
    }
}

function OnIsOwnInit(s, e) {
    if (isOwn.GetChecked()) {
        despachureDateLabel.SetText("Fecha Llegada:");
    } else {
        despachureDateLabel.SetText("Fecha Despacho:");
    }
}

function OnDriverNameThirdInit(s, e) {
    // 
    var answerControlsEnabledTmp = $("#answerControlsEnabled").val();
    if (answerControlsEnabledTmp == "NO") {
        driverNameThird.SetEnabled(false);
    }
}

function OnCarRegistrationThirdInit(s, e) {
    // 
    var answerControlsEnabledTmp = $("#answerControlsEnabled").val();
    if (answerControlsEnabledTmp == "NO") {
        carRegistrationThird.SetEnabled(false);
    }
}

function UpdateProviderVehicle(id_providervehicle) {
    // 
    var isprovider = false;
    for (var i = 0; i < id_provider.GetItemCount() ; i++) {
        var providertransport = id_provider.GetItem(i);

        if (id_providervehicle == providertransport.value) {
            //id_provider.selectedValue = id_providervehicle;
            id_provider.SetSelectedIndex(providertransport.index);
            isprovider = true;
            break;
        }
    }
    if (isprovider == false) {
        id_provider.SetValue(null);
    }

}

function VehicleCombo_SelectedIndexChanged(s, e) {
   
    mark.SetText("");
    model.SetText("");
    VehicleHunterLockText.SetText("");

    
    if (s.GetValue() != null) {

        $.ajax({
            url: "Logistics/VehicleData",
            type: "post",
            data: {id_vehicle : s.GetValue()},
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
                    // 
                    mark.SetText(result.mark);
                    model.SetText(result.model);
                    VehicleHunterLockText.SetText(result.hunterLock);
                    if (result.id_VeicleProvider !== null) {
                        UpdateProviderVehicle(result.id_VeicleProvider);
                    } else {
                      id_provider.SetValue(null);
                    }

                }
            },
            complete: function () {
            }
        });
    };
    
}


function OnFishingZoneRGBeginCallback(s, e) {
    e.customArgs["id_FishingZoneRGNew"] = s.GetValue();
}

function FishingZoneRG_SelectedIndexChanged(s, e) {
    id_FishingSiteRG.ClearItems();
    id_FishingSiteRG.SetValue(null);
    
    id_FishingSiteRG.PerformCallback();
    //if (s.GetValue() != null && s.GetValue() > 0)
    //{
    //    $.ajax({
    //            url: "Logistics/UpdateFishingSiteForRG",//ItemPlaningDetailsData",
    //            type : "post",
    //            data: { id_FishingZoneRGTmp: s.GetValue()},
    //            async : true,
    //            cache: false,
    //            error: function (error) {
    //                console.log(error);
    //            },
    //            beforeSend: function () {
    //            },
    //            success: function (result) {
    //                id_FishingSiteRG.PerformCallback();
    //            },
    //            complete: function () {
    //            }
    //    });
    //}

}

function FishingSiteRG_SelectedIndexChanged(s, e) {
    CalculatePriceFreightAndRefresh();
}
// TABS FUNCTIONS

var activeGridView = null;

function TabControl_Init(s, e) {

activeGridView = null;

if (tabControl.GetActiveTab().name === "tabDetails") {
activeGridView = gvDetails;
} else if (tabControl.GetActiveTab().name === "tabDispatchMaterials") {
activeGridView = gvDispatchMaterials;
} else if (tabControl.GetActiveTab().name === "tabSecuritySeals") {
activeGridView = gvSecuritySeals;
} else if (tabControl.GetActiveTab().name === "tabAssignedStaff") {
activeGridView = gvAssignedStaff;
}

//tabControl.GetTabByName("tabExport").SetVisible(isExport.GetChecked());
}

function TabControl_ActiveTabChanged(s, e) {

var enabeled = false;
var codeDocumentState = $("#codeDocumentState").val();

if (codeDocumentState == "01") {

if (tabControl.GetActiveTab().name === "tabDetails") {
    activeGridView = gvDetails;
    enabeled = true;
} else if (tabControl.GetActiveTab().name === "tabDispatchMaterials") {
    activeGridView = gvDispatchMaterials;
    enabeled = true;
} else if (tabControl.GetActiveTab().name === "tabSecuritySeals") {
    activeGridView = gvSecuritySeals;
    enabeled = true;
} else if (tabControl.GetActiveTab().name === "tabAssignedStaff") {
    activeGridView = gvAssignedStaff;
    enabeled = true;
}


btnNewDetail.SetEnabled(enabeled);
btnRemoveDetail.SetEnabled(enabeled);
btnRefreshDetails.SetEnabled(enabeled);
}


if (enabeled === true) {
UpdateTitlePanel();
}
}

function AutoCloseAlert() {
if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
setTimeout(function () {
    $(".alert-success").alert('close');
}, 2000);
}
}

function UpdateView() {
var id = parseInt($("#id_remissionGuide").val());

// EDITING BUTTONS
btnNew.SetEnabled(true);
btnSave.SetEnabled(false);
btnCopy.SetEnabled(id !== 0);

// STATES BUTTONS

$.ajax({
    url: "Logistics/Actions",
    type: "post",
    data: { id: id
},
    async: true,
    cache: false,
    error: function (error) {
    console.log(error);
},
    beforeSend: function () {
    showLoading();
},
    success: function (result) {
    btnApprove.SetEnabled(result.btnApprove);
    btnAutorize.SetEnabled(result.btnAutorize);
    btnProtect.SetEnabled(result.btnProtect);
    btnCancel.SetEnabled(result.btnCancel);
    btnRevert.SetEnabled(result.btnRevert);
    btnreassignment.SetEnabled(result.btnreassignment);
    btnCancelRG.SetEnabled(result.btnCancelRG);
},
    complete: function (result) {
    hideLoading();
}
});

// HISTORY BUTTON
//btnHistory.SetEnabled(id !== 0);

// PRINT BUTTON
btnPrint.SetEnabled(id !== 0);
}

function UpdatePagination() {
var current_page = 1;
$.ajax({
    url: "Logistics/InitializePagination",
    type: "post",
    data: { id_remissionGuide: $("#id_remissionGuide").val()
},
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