//DIALOG BUTTONS ACTIONS

function Update(approve) {
    // 
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabRemissionGuideControlEntrance", true);
    var validT = ASPxClientEdit.ValidateEditorsInContainer(null, "detailRGCVEcont", true);
    var valid2 = ASPxClientEdit.AreEditorsValid();
        
    var entranceTimeProductionUnitProviderTmp = entranceTimeProductionUnitProviderBuilding.GetText();
    var exitTimeProductionUnitProviderTmp = exitTimeProductionUnitProviderBuilding.GetText();
    var entranceTimePlantProductionTmp = entranceTimeProductionBuilding.GetText();
    var poundsRemittedTmp = poundsRemitted.GetValue();
    var piscinasTmp = piscinas.GetText();
    var nGabetasTmp = noGabetas.GetValue();
    var nBinesTmp = noBines.GetValue();

    if (valid && validT && valid2) {
         
        var id = $("#id_remissionGuide").val();

        var data = "id=" + id + "&" + "approve=" + approve + "&" + "entranceTimePUP=" + entranceTimeProductionUnitProviderTmp + "&"
                            + "exitTimePUP=" + exitTimeProductionUnitProviderTmp + "&"
                            + "entranceTimeBuilding=" + entranceTimePlantProductionTmp + "&" 
                            + "poundsRemitted=" + poundsRemittedTmp + "&" 
                            + "piscinas=" + piscinasTmp + "&" 
                            + "nGabetas=" + nGabetasTmp + "&" 
                            + "nBines=" + nBinesTmp + "&" 
                            + $("#formEditRemissionGuideControlVehicleEntrance").serialize();
        var url = (id === "0" || id === 0) ? "RemissionGuideControlVehicleEntrance/RemissionGuideControlVehicleEntrancePartialAddNew"
                               : "RemissionGuideControlVehicleEntrance/RemissionGuideControlVehicleEntrancePartialUpdate";

        showForm(url, data);
    }
}

function ButtonUpdate_Click(s, e) {
    Update(false);
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
     
    showPage("RemissionGuideControlVehicleEntrance/Index", null);
}


// COMMOM BUTTONS ACTIONS

function AddNew(s, e) {
    if (activeGridView !== null && activeGridView !== undefined) {
        activeGridView.AddNewRow();
    }
}

function Remove(s, e) {
    var key = "id_item";

    activeGridView.GetSelectedFieldValues(key, function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var url = "RemissionGuideControlVehicleEntrance/DeleteSelectedRemissionGuideControlVehicleEntranceDetails";

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
                // CODIGO VA AQUI: 
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


// ON INIT
//ON INIT
function entranceDateProductionBuildingOnInit(s, e) {
    var entrancedTmp = document.getElementById("entrancedpb").value;
    if (entrancedTmp == null || entrancedTmp == undefined || entrancedTmp == "") {
        s.SetValue(new Date());
    }
}

function entranceDateProductionUnitProviderBuildingOnInit(s, e) {
    var entrancedTmp = document.getElementById("entrancedpupb").value;
    if (entrancedTmp == null || entrancedTmp == undefined || entrancedTmp == "") {
        s.SetValue(new Date());
    }
}

function exitDateProductionUnitProviderBuildingOnInit(s, e) {
    var exitedTmp = document.getElementById("exitdpupb").value;
    if (exitedTmp == null || exitedTmp == undefined || exitedTmp == "") {
        s.SetValue(new Date());
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

    if (s.GetEditor("number") !== null && s.GetEditor("number") !== undefined) {
        s.GetEditor("number").SetEnabled(customCommand === "ADDNEWROW");
    } else if (s.GetEditor("id_person") !== null && s.GetEditor("id_person") !== undefined) {
        s.GetEditor("id_person").SetEnabled(customCommand === "ADDNEWROW");
    }


    var codeDocumentState = $("#codeDocumentState").val();

    if (codeDocumentState == "01") {
    }
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {

    if (tabControl.GetActiveTab().name === "tabDetails") {
        activeGridView = gvDetails;
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

// TABS FUNCTIONS

var activeGridView = null;

function TabControl_Init(s, e) {

    activeGridView = null;

    if (tabControl.GetActiveTab().name === "tabDetails") {
        activeGridView = gvDetails;
    }

}

function TabControl_ActiveTabChanged(s, e) {

    var enabeled = false;
    var codeDocumentState = $("#codeDocumentState").val();

    if (codeDocumentState == "01") {

        if (tabControl.GetActiveTab().name === "tabDetails") {
            activeGridView = gvDetails;
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

}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "RemissionGuideControlVehicleEntrance/InitializePagination",
        type: "post",
        data: { id_remissionGuide: $("#id_remissionGuide").val() },
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

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
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