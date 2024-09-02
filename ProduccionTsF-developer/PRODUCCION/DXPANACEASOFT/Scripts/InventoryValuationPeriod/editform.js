
//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabdetail", true);

     if (!valid) {
         UpdateTabImage({ isValid: false }, "tabdetail");
     }

     if (valid) {
          
         var id = $("#id_InventoryValuationPeriod").val();
   
         
      
         var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#FormEditInventoryValuationPeriod").serialize();
         var url = (id === "0") ? "InventoryValuationPeriod/InventoryValuationPeriodPartialAddNew"
                               : "InventoryValuationPeriod/InventoryValuationPeriodPartialUpdate";

        showForm(url, data);
    }
}

function TabControl_ActiveTabChanged(s, e) {

   

    
}

function ButtonUpdate_Click(s, e) {

    Update(false);
  
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    showPage("InventoryValuationPeriod/Index", null);
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("InventoryValuationPeriod/FormEditInventoryValuationPeriod", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {

}

function ApproveDocument(s, e) {
  


}

function AutorizeDocument(s, e) {

}

function ProtectDocument(s, e) {

}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_InventoryValuationPeriod").val()
        };
        showForm("InventoryValuationPeriod/Cancel", data);
    }, "¿Desea anular el InventoryValuationPeriodio?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_InventoryValuationPeriod").val()
        };
        showForm("InventoryValuationPeriod/Revert", data);
    }, "¿Desea Activar?");
}

function ShowDocumentHistory(s, e) {

}



function PrintDocument(s, e) {
             
}

// DETAILS BUTTONS ACTIONS

function RefreshDetail(s, e) {
    Refresh(s, e);
}

// TABS FUNCTIONS

var activeGridView = null;

function TabControl_Init(s, e) {

    activeGridView = null;

    if (tabControl.GetActiveTab().name === "tabdetail") {
     
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

// MAIN FUNCTIONS

function init() {
  
    AutoCloseAlert();
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

}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {

   if (tabControl.GetActiveTab().name === "tabDeta") {
        activeGridView = gvDetail;
     
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

        if (activeGridView === gvDetail) {
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



$(function () {
    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});