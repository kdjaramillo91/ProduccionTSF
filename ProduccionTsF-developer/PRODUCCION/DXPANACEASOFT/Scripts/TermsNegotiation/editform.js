
//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        // 
        gvTermsNegotiation.UpdateEdit();
    }

    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabdetail", true);

    // if (!valid) {
    //     UpdateTabImage({ isValid: false }, "tabdetail");
    // }






    // if (valid) {
          
    //     var id = $("#id_TermsNegotiation").val();
   
         
      
    //     var data = "id=" + id + "&" +  $("#FormEditTermsNegotiation").serialize();
    //     var url = (id === "0") ? "TermsNegotiation/TermsNegotiationPartialAddNew"
    //                           : "TermsNegotiation/TermsNegotiationPartialUpdate";

    //    showForm(url, data);
    //}
}

function TabControl_ActiveTabChanged(s, e) {

   

    
}

function ButtonUpdate_Click(s, e) {

    Update(false);
  
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    //showPage("TermsNegotiation/Index", null);
    if (gvTermsNegotiation !== null && gvTermsNegotiation !== undefined) {
        gvTermsNegotiation.CancelEdit();
    }
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("TermsNegotiation/FormEditTermsNegotiation", data);
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
            id: $("#id_TermsNegotiation").val()
        };
        showForm("TermsNegotiation/Cancel", data);
    }, "¿Desea anular el Estado?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_TermsNegotiation").val()
        };
        showForm("TermsNegotiation/Revert", data);
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

   
    


}









function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
    //var id = parseInt($("#id_TermsNegotiation").val());

    //// EDITING BUTTONS
    //btnNew.SetEnabled(true);
    //btnSave.SetEnabled(false);
    //btnCopy.SetEnabled(id !== 0);

    //// STATES BUTTONS

    //$.ajax({
    //    url: "TermsNegotiation/Actions",
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









$(function () {
    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});