//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabRemissionGuide", true);

    var valuePriceTmp = valuePrice.GetText();
    if (valuePriceTmp == null) { valuePriceTmp = 0; }

    var valueAdvanceTmp = advancePrice.GetText();
    if (valueAdvanceTmp == null) { valueAdvanceTmp = 0; }
    if (parseFloat(valueAdvanceTmp) > parseFloat(valuePriceTmp)) {
        advancePrice.Validate();
        return;
    }
    if (parseFloat(valuePriceTmp) == 0) {
        valuePrice.Validate();
        return;
    }
    if (!valid) {
         UpdateTabImage({ isValid: false }, "tabRemissionGuide");
     }
     if (gvDetails.cpRowsCount === 0 || gvDetails.IsEditing()) {
         UpdateTabImage({ isValid: false }, "tabDetails");
         valid &= false;
     }
     valid &= ASPxClientEdit.ValidateEditorsInContainer(null, "tabTransportation", true);
     if (valid) {
         //var id = parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"));
         var id = parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"));
       var iddespachurehour = despachurehour.GetValue();
       var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditRemissionGuideRiver").serialize() + "&carRegistration=" + id_vehicle.GetText() + "&driverName=" + id_driver.GetText() + "&despachurehour=" + iddespachurehour + "&id_provider="+id_provider.GetValue();
       var url = (id === 0 || id === "0") ? "RemissionGuideRiver/RemissionGuideRiverPartialAddNew"
                               : "RemissionGuideRiver/RemissionGuideRiverPartialUpdate";
        showForm(url, data);
    }
}

function pricefreightrefresh() {
    // 

    var id_tttTmp = id_TransportTariffType.GetValue();
    var id_fsfgTmp = id_FishingSiteRGR.GetValue();

    if (id_tttTmp > 0 && id_fsfgTmp > 0) {
        var data = {
            id_FishingSite: id_fsfgTmp,
            id_TransportTariff: id_tttTmp
        };
        valuePrice.SetValue(0);

        $.ajax({

            url: "RemissionGuideRiver/pricefreightrefresh",
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
                valuePrice.SetValue(result.pricefreight);
            },
            complete: function () {

            }
        });
    }
    //var data = "id_fishingSiteRgrTmp=" & id_fishingSiteRgrTmp & "&id=" + id + "&" + $("#formEditRemissionGuideRiver").serialize() + "&carRegistration=" + id_vehicle.GetText() + "&driverName=" + id_driver.GetText() + "&despachurehour=" + iddespachurehour + "&id_provider=" + id_provider.GetValue();
    
}

function ButtonUpdate_Click(s, e) {
    Update(false);
}

function ButtonUpdateClose_Click(s, e) {
}

function ButtonCancel_Click(s, e) {
    showPage("RemissionGuideRiver/Index", null);
}
//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    //var data = {
    //    id: 0
    //};

    //showPage("RemissionGuideRiver/FormEditRemissionGuideRiver", data);

    // 
    showPage("RemissionGuideRiver/Index", null);
    AddNewGuideRemissionRiverFromRemissionGuide();
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
   // showPage("RemissionGuideRiver/RemissionGuideRiverCopy", { id: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")) });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar la guía de remisión?");
  
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"))
        };
        showForm("RemissionGuideRiver/Autorize", data);
    }, "¿Desea autorizar la guía de remisión?");
}

function CheckAutorizeRSIDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"))
        };
        showForm("RemissionGuideRiver/CheckAutorizeRSI", data);
    }, "¿Desea Verificar la Autorización en el SRI de la guía de remisión?");

}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"))
        };
        showForm("RemissionGuideRiver/Protect", data);
    }, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"))
        };
        showForm("RemissionGuideRiver/Cancel", data);
    }, "¿Desea anular la guía de remisión?");
} 

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"))
        };
        showForm("RemissionGuideRiver/Revert", data);
    }, "¿Desea reversar la guía de remisión?");
}

function ShowDocumentHistory(s, e) {

}


//IMPRESIONES
function PrintDocumentAllManual(s, e) {

    // 
    var hasPersonalAssigned = "N";
    var hasAdvance = "N";

    var data3 = {
        id_remissionGuideRiver: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"))
    };
    $.ajax({
        url: 'RemissionGuideRiver/GetPrintInformation',
        data: data3,
        async: false,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            // 
            if (result != null && result != undefined) {
                hasAdvance = result.hasAdvanceP;
                hasPersonalAssigned = result.hasPersAss;
            }
        },
        complete: function () {
            hideLoading();
        }
    });

    var data1 = { id_rg: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")), codeReport: "GRVSRF", typePrint: "N" };
    if (hasPersonalAssigned == "Y") {
        $.ajax({
            url: 'RemissionGuideRiver/PrintRemissionGuideRiverReports',
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
                        if (result.printType == "D") {
                            var reportTdr = result.nameQS;
                            $.ajax({
                                url: "ReportProd/PrintDirect",
                                type: "post",
                                data: { _strq: reportTdr },
                                async: false,
                                cache: false,
                                error: function (error) {
                                },
                                beforeSend: function () {
                                },
                                success: function (result) {
                                },
                                complete: function () {
                                }
                            });
                            hideLoading();
                        } else {
                            var reportTdr = result.nameQS;
                            var url = 'ReportProd/Index?trepd=' + reportTdr;
                            newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                            newWindow.focus();
                            hideLoading();
                        }
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

    var data2 = { id_rg: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")), codeReport: "GRVCRF", typePrint: "N" };

    if (hasAdvance == "Y") {
        $.ajax({
            url: 'RemissionGuideRiver/PrintRemissionGuideRiverReports',
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
                        if (result.printType == "D") {
                            var reportTdr = result.nameQS;
                            $.ajax({
                                url: "ReportProd/PrintDirect",
                                type: "post",
                                data: { _strq: reportTdr },
                                async: false,
                                cache: false,
                                error: function (error) {
                                },
                                beforeSend: function () {
                                },
                                success: function (result) {
                                },
                                complete: function () {
                                }
                            });
                            hideLoading();
                        } else {
                            var reportTdr = result.nameQS;
                            var url = 'ReportProd/Index?trepd=' + reportTdr;
                            newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                            newWindow.focus();
                            hideLoading();
                        }
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

function PrintDocumentManual(s, e) {
    var data = { id_rg: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")), typePrint: "N" };

    $.ajax({
        url: 'RemissionGuideRiver/PrintRemissionGuideRiverReport',
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
                    if (result.printType == "D") {
                        var reportTdr = result.nameQS;
                        $.ajax({
                            url: "ReportProd/PrintDirect",
                            type: "post",
                            data: { _strq: reportTdr },
                            async: false,
                            cache: false,
                            error: function (error) {
                            },
                            beforeSend: function () {
                            },
                            success: function (result) {
                            },
                            complete: function () {
                            }
                        });
                        hideLoading();
                    } else {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
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

function PrintDocumentAll(s, e) {

    // 
    var hasPersonalAssigned = "N";
    var hasAdvance = "N";

    var data3 = {
        id_remissionGuideRiver: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide"))
    };
    $.ajax({
        url: 'RemissionGuideRiver/GetPrintInformation',
        data: data3,
        async: false,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            // 
            if (result != null && result != undefined) {
                hasAdvance = result.hasAdvanceP;
                hasPersonalAssigned = result.hasPersAss;
            }
        },
        complete: function () {
            hideLoading();
        }
    });

    var data1 = { id_rg: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")), codeReport: "GRVSRF", typePrint: "D" };
    if (hasPersonalAssigned == "Y") {
        $.ajax({
            url: 'RemissionGuideRiver/PrintRemissionGuideRiverReports',
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
                        if (result.printType == "D") {
                            var reportTdr = result.nameQS;
                            $.ajax({
                                url: "ReportProd/PrintDirect",
                                type: "post",
                                data: { _strq: reportTdr },
                                async: false,
                                cache: false,
                                error: function (error) {
                                },
                                beforeSend: function () {
                                },
                                success: function (result) {
                                },
                                complete: function () {
                                }
                            });
                            hideLoading();
                        } else {
                            var reportTdr = result.nameQS;
                            var url = 'ReportProd/Index?trepd=' + reportTdr;
                            newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                            newWindow.focus();
                            hideLoading();
                        }
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

    var data2 = { id_rg: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")), codeReport: "GRVCRF", typePrint: "D" };

    if (hasAdvance == "Y") {
        $.ajax({
            url: 'RemissionGuideRiver/PrintRemissionGuideRiverReports',
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
                        if (result.printType == "D") {
                            var reportTdr = result.nameQS;
                            $.ajax({
                                url: "ReportProd/PrintDirect",
                                type: "post",
                                data: { _strq: reportTdr },
                                async: false,
                                cache: false,
                                error: function (error) {
                                },
                                beforeSend: function () {
                                },
                                success: function (result) {
                                },
                                complete: function () {
                                }
                            });
                            hideLoading();
                        } else {
                            var reportTdr = result.nameQS;
                            var url = 'ReportProd/Index?trepd=' + reportTdr;
                            newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                            newWindow.focus();
                            hideLoading();
                        }
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
    var data = { id_rg: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")), typePrint: "D" };

    $.ajax({
        url: 'RemissionGuideRiver/PrintRemissionGuideRiverReport',
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
                    if (result.printType == "D") {
                        var reportTdr = result.nameQS;
                        $.ajax({
                            url: "ReportProd/PrintDirect",
                            type: "post",
                            data: { _strq: reportTdr },
                            async: false,
                            cache: false,
                            error: function (error) {
                            },
                            beforeSend: function () {
                            },
                            success: function (result) {
                            },
                            complete: function () {
                            }
                        });
                        hideLoading();
                    } else {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
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


function PrintRide(s, e) {
    var data = { id_rg: parseInt(document.getElementById("id_remissionGuide").getAttribute("idremissionGuide")), typePrint: "D" };

    $.ajax({
        url: 'RemissionGuideRiver/PrintRemissionGuideRiverReport',
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


function ShowReasignacion(s, e) {
}
// COMMOM BUTTONS ACTIONS
function AddNew(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabRemissionGuide", true);
    if (!valid) {
        UpdateTabImage({ isValid: false }, "tabRemissionGuide");
    }
    if (valid) {
        // 
        var iddespachurehour = despachurehour.GetValue();
        var data = $("#formEditRemissionGuideRiver").serialize() + "&carRegistration=" + id_vehicle.GetText() + "&driverName=" + id_driver.GetText() + "&despachurehour=" + iddespachurehour + "&id_provider=" + id_provider.GetValue();
        
        var url = "RemissionGuideRiver/RemissionGuideGeneredDetailspopupPartial";

        $.ajax({
            url: url,
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
            success: function () {
                // 
                popupRemissionGuide.Show();
                popupRemissionGuide.PerformCallback();
            },
            complete: function () {
                    
            }
        });
    }
}

function Remove(s, e) {
    // 
    var key = "id_RemissionGuide";
    activeGridView.GetSelectedFieldValues(key, function (values) {
        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
       var url = "RemissionGuideRiver/DeleteSelectedRemissionGuideDetails";
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

// COMMOM BUTTONS ACTIONS

function AddNewCommon(s, e) {
    if (activeGridView !== null && activeGridView !== undefined) {
        activeGridView.AddNewRow();
    }
}

function RemoveCommon(s, e) {
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

        var url = "RemissionGuideRiver/DeleteSelectedRemissionGuideRiverDetails";
        if (activeGridView === gvDispatchMaterials) {
            url = "RemissionGuideRiver/DeleteSelectedRemissionGuideRiverDispatchMaterials";
        } else if (activeGridView === gvSecuritySeals) {
            url = "RemissionGuideRiver/DeleteSelectedRemissionGuideRiverSecuritySeals";
        } else if (activeGridView === gvAssignedStaff) {
            url = "RemissionGuideRiver/DeleteSelectedRemissionGuideRiverAssignedStaff";
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

function RefreshCommon(s, e) {
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

    var codeDocumentState = $("#codeDocumentState").val();
    gvMaterialDispatchRiverEditFormDetails.PerformCallback();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    if (tabControl.GetActiveTab().name === "tabDetails") {
        activeGridView = gvDetails;
        pricefreightrefresh();
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
        } 
        if (lblInfo !== null && lblInfo !== undefined) {
            lblInfo.html(text);
        }
        SetElementVisibility(lnkSelectAllRows, activeGridView.GetSelectedRowCount() > 0 && activeGridView.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility(lnkClearSelection, activeGridView.GetSelectedRowCount() > 0);
     //   btnRemoveDetail.SetEnabled(activeGridView.GetSelectedRowCount() > 0);
    }
   
}

function GetSelectedFilteredRowCount() {
    return activeGridView.cpFilteredRowCountWithoutPage + activeGridView.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function OnGridViewInitpopup(s, e) {
    UpdateTitlePanelpopup();
}

var selectedRemissionGuideDetailsRowspopup = [];
var selectedRemissionGuideDetailsRowspopupfinal = [];
function OnGridViewSelectionChangedpopup(s, e) {
    // 
    var index = e.visibleIndex;
    var key = s.GetRowKey(e.visibleIndex);

    if (e.isSelected) {
     
        selectedRemissionGuideDetailsRowspopupfinal.push(key);
       
    }
    else {
    

        for (var i = 0; i < selectedRemissionGuideDetailsRowspopupfinal.length; i++) {
            if (selectedRemissionGuideDetailsRowspopupfinal[i] == key) {
               
              selectedRemissionGuideDetailsRowspopupfinal.splice(i, 1);

            }
        }
      
    }
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallbackpopup);

    UpdateTitlePanelpopup();
   
   
}

function GetSelectedFieldDetailValuesCallbackpopup(values) {
    // 
    selectedRemissionGuideDetailsRowspopup = [];

    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideDetailsRowspopup.push(values[i]);
    }
}

function OnGridViewBeginCallbackpopup(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallbackpopup(s, e) {
    UpdateTitlePanelpopup();
    var codeDocumentState = $("#codeDocumentState").val();
    gvMaterialDispatchRiverEditFormDetails.PerformCallback();
}

function GetSelectedFilteredRowCountPopup() {
    return gvDetailsRemissionGuide.cpFilteredRowCountWithoutPage + gvDetailsRemissionGuide.GetSelectedKeysOnPage().length;
}

function UpdateTitlePanelpopup() {
    // 
    if (gvDetailsRemissionGuide === null)
        return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountPopup();
    var text = "Total de elementos seleccionados: <b>" + gvDetailsRemissionGuide.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvDetailsRemissionGuide.GetSelectedRowCount() - GetSelectedFilteredRowCountPopup();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    var codeDocumentState = $("#codeDocumentState").val();
   // if (codeDocumentState == "01") {
        var lblInfo = null;
        var lnkSelectAllRows = "";
        var lnkClearSelection = "";

   
            lblInfo = $("#lblInfoDetailsRemision");
            lnkSelectAllRows = "lnkSelectAllRowsDetailsRemission";
            lnkClearSelection = "lnkClearSelectionDetailsRemission";
        
        if (lblInfo !== null && lblInfo !== undefined) {
            lblInfo.html(text);
        }
        SetElementVisibility(lnkSelectAllRows, gvDetailsRemissionGuide.GetSelectedRowCount() > 0 && gvDetailsRemissionGuide.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility(lnkClearSelection, gvDetailsRemissionGuide.GetSelectedRowCount() > 0);
      
   // }

}

function OnBtnClosePopup_ClickRemissionFilter() {
    popupRemissionGuide.Hide();
}

function AddREmisiondetailpopup(s, e) {
    // 
    var key = "id";
    // 
    var selectedRows = [];

    var url = "RemissionGuideRiver/AddSelectedRemissionGuideDetailsPopup";
    $.ajax({
        url: url,
        type: "post",
        data: { ids: selectedRemissionGuideDetailsRowspopupfinal },
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
            selectedRemissionGuideDetailsRowspopupfinal = [];
            gvDetails.PerformCallback();
            popupRemissionGuide.Hide();
        }
    });
}

function CancelRG(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_remissionGuide").val()
        };
        showForm("Logistics/CancelRG", data);
    }, "¿Desea Cancelar la guía de remisión?");
}

// SECURITY SEALS BUTTONS ACTIONS

function AddNewSecuritySeal(s, e) {
    AddNewCommon(s, e);
}

function RemoveSecuritySeal(s, e) {
    RemoveCommon(s, e);
}

function RefreshSecuritySeal(s, e) {
    RefreshCommon(s, e);
}

// ASSIGNED STAFF BUTTONS ACTIONS

function AddNewAssignedStaff(s, e) {
    AddNewCommon(s, e);
}

function RemoveAssignedStaff(s, e) {
    RemoveCommon(s, e);
}

function RefreshAssignedStaff(s, e) {
    RefreshCommon(s, e);
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

// REMISSION GUIDE TRANSPORTATION FUNCTIONS
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
  if (s.GetValue() != null) {
     $.ajax({
            url: "RemissionGuideRiver/VehicleData",
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
                    if (result.id_VeicleProvider !== null) {
               
                        UpdateProviderVehicle(result.id_VeicleProvider);
                    } else {
                      id_provider.SetValue(null);
                    }

                }
            },
            complete: function () {
               // id_provider.PerformCallback();
                //hideLoading();
            }
        });
  };
}

function OnFishingZoneRGRBeginCallback(s, e) {
    e.customArgs["id_FishingZoneRGRNew"] = s.GetValue();
}

function FishingZoneRGR_SelectedIndexChanged(s, e) {
    id_FishingSiteRGR.ClearItems();
    id_FishingSiteRGR.SetValue(null);

    id_FishingSiteRGR.PerformCallback();
}

function FishingSiteRGR_SelectedIndexChanged(s, e) {
    //CalculatePriceFreightAndRefresh();
    pricefreightrefresh();
}
// TABS FUNCTIONS
var activeGridView = null;
function TabControl_Init(s, e) {

    activeGridView = null;
    if (tabControl.GetActiveTab().name === "tabDetails") {
        activeGridView = gvDetails;
    } else if (tabControl.GetActiveTab().name === "tabDispatchMaterials") {
        activeGridView = gvMaterialDispatchRiverEditFormDetails;
    } else if (tabControl.GetActiveTab().name === "tabSecuritySeals") {
        activeGridView = gvSecuritySealsRiver;
    } else if (tabControl.GetActiveTab().name === "tabAssignedStaff") {
        activeGridView = gvAssignedStaffRiver;
    }
}

function TabControl_ActiveTabChanged(s, e) {

    var enabeled = false;
    var codeDocumentState = $("#codeDocumentState").val();

    if (codeDocumentState == "01") {

        if (tabControl.GetActiveTab().name === "tabDetails") {
            activeGridView = gvDetails;
            enabeled = true;
        } else if (tabControl.GetActiveTab().name === "tabDispatchMaterials") {
            activeGridView = gvMaterialDispatchRiverEditFormDetails;
            enabeled = true;
        } else if (tabControl.GetActiveTab().name === "tabSecuritySeals") {
            activeGridView = gvSecuritySealsRiver;
            enabeled = true;
        } else if (tabControl.GetActiveTab().name === "tabAssignedStaff") {
            activeGridView = gvAssignedStaffRiver;
            enabeled = true;
        }

   
        btnNewDetail.SetEnabled(enabeled);
      //  btnRemoveDetail.SetEnabled(enabeled);
       // btnRefreshDetails.SetEnabled(enabeled);
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
    var id = parseInt(parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")));

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    //btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "RemissionGuideRiver/Actions",
        type: "post",
        data: { id: id },
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
            btnCheckAutorizeRSI.SetEnabled(result.btnCheckAutorizeRSI);
            btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
            btnPrint.SetEnabled(result.btnPrint);
            btnPrintAlldoc.SetEnabled(result.btnPrintAlldoc);
            btnPrintManual.SetEnabled(result.btnPrintManual);
            btnPrintAlldocManual.SetEnabled(result.btnPrintAlldocManual);
        },
        complete: function (result) {
            hideLoading();
        }
    });

    // HISTORY BUTTON
    //btnHistory.SetEnabled(id !== 0);

    // PRINT BUTTON
}

function UpdatePagination() {
    var current_page = 1;
    // 
    $.ajax({
        url: "RemissionGuideRiver/InitializePagination",
        type: "post",
        data: { id_remissionGuide: parseInt(document.getElementById("id_remissionriverGuide").getAttribute("idremissionriverGuide")) },
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


function UpdateFishingZone() {


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