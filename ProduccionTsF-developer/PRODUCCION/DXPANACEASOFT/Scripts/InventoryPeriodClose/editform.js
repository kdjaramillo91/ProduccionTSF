
//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabdetail", true);

     if (!valid) {
         UpdateTabImage({ isValid: false }, "tabdetail");
     }






     if (valid) {
          
         var id = $("#id_InventoryPeriod").val();
   
         
      
         var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#FormEditInventoryPeriodClose").serialize();
         var url = "InventoryPeriodClose/InventoryPerioClosedPartialUpdate";
         

        showForm(url, data);
    }
}


function OnClosed(sender) {

    var idAtt = $(sender).attr("data-id");
    var idObj = $(sender).data();
 
    var data = {
        id: idObj.id
    };
    //showForm("InventoryPeriodClose/InventoryPeriodDetailPartialUpdate", data);
        //var data = "id=" + idObj.id;
        //var url =  "InventoryPeriodClose/InventoryPeriodDetailPartialUpdate";

        //showForm(url, data);

        $.ajax({
            url: "InventoryPeriodClose/InventoryPeriodDetailPartialUpdate",
        type: "post",
        data: { id: idObj.id },
        async: true,
        cache: false,
        error: function (error) {
            //console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if(result.id ==1)
            {

                // 
                var msgErrorAux = ErrorMessage(result.mensaje);
                gridMessageErrora.SetText(msgErrorAux);
                $("#gridMessageErrora").show();
                

            } else {
                gridMessageErrora.SetText("");
                $("#gridMessageErrora").hide();;
            }
           

        },
        complete: function (result) {
          //  gvDetail.PerformCallback();
     
            tabControl.reloadContentOnCallback = true;
            gvDetail.PerformCallback();
            hideLoading();
        }
    });
    }



function TabControl_ActiveTabChanged(s, e) {

   

    
}

function ButtonUpdate_Click(s, e) {

    Update(false);
  
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    showPage("InventoryPeriodClose/Index", null);
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("InventoryPeriodClose/FormEditInventoryPeriodClose", data);
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
            id: $("#id_InventoryPeriod").val()
        };
        showForm("InventoryPeriodClose/Cancel", data);
    }, "¿Desea anular el InventoryPeriodio?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_InventoryPeriod").val()
        };
        showForm("InventoryPeriodClose/Revert", data);
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
    //var id = parseInt($("#id_InventoryPeriod").val());

    //// EDITING BUTTONS
    //btnNew.SetEnabled(true);
    //btnSave.SetEnabled(false);
    //btnCopy.SetEnabled(id !== 0);

    //// STATES BUTTONS

    //$.ajax({
    //    url: "InventoryPeriod/Actions",
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
   // tabControl.PerformCallback();



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