//DIALOG BUTTONS ACTIONS

function Update(approve) {
    // 
    var valid = ASPxClientEdit.ValidateEditorsInContainer("tabRemissionGuide",null, true);
   // var validT = ASPxClientEdit.ValidateEditorsInContainer("RGCVContainer", null, true);
    var valid2 = ASPxClientEdit.AreEditorsValid();
    //var exitTimePlantProductionTmp = exitTimeProductionBuilding.GetText();
    if (valid  && valid2) {
        // 
        //var id = $("#id_remissionGuide").val();

        var id = document.getElementById("id_remissionGuide").getAttribute("idremissionGuide");

        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditRemissionGuideInternControl").serialize();
        var url = "RemissionGuideInternControl/RemissionGuideInternControlPartialUpdate";

        showForm(url, data);
    }
}

function ButtonUpdate_Click(s, e) {
    Update(false);
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
     
    showPage("RemissionGuideInternControl/Index", null);
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

        var url = "RemissionGuideInternControl/DeleteSelectedRemissionGuideControlVehicleDetails";

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

//ON INIT
function exitDateProductionBuildingOnInit(s, e) {
    
    var eTmp = document.getElementById("edpb").value;
    if (eTmp == null || eTmp == undefined || eTmp == "") {
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
    //var id = parseInt($("#id_remissionGuide").val());

    //// EDITING BUTTONS
    //btnNew.SetEnabled(true);
    //btnSave.SetEnabled(false);
    //btnCopy.SetEnabled(id !== 0);

    //// STATES BUTTONS

    //$.ajax({
    //    url: "RemissionGuideControlVehicle/Actions",
    //    type: "post",
    //    data: { id: id },
    //    async: true,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //    },
    //    beforeSend: function () {
    //        showLoading();
    //    },
    //    success: function (result) {
    //        btnApprove.SetEnabled(result.btnApprove);
    //        btnAutorize.SetEnabled(result.btnAutorize);
    //        btnProtect.SetEnabled(result.btnProtect);
    //        btnCancel.SetEnabled(result.btnCancel);
    //        btnRevert.SetEnabled(result.btnRevert);
    //    },
    //    complete: function (result) {
    //        hideLoading();
    //    }
    //});

    //// HISTORY BUTTON
    //btnHistory.SetEnabled(id !== 0);

    //// PRINT BUTTON
    //btnPrint.SetEnabled(id !== 0);
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "RemissionGuideInternControl/InitializePagination",
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