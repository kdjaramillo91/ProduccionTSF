
//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvSetting.UpdateEdit();
    }

}

function TabControl_ActiveTabChanged(s, e) {
}

function ButtonUpdate_Click(s, e) {

    Update(true);
  
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    //showPage("Setting/Index", null);
    if (gvSetting !== null && gvSetting !== undefined) {
        gvSetting.CancelEdit();
    }
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("Setting/FormEditSetting", data);
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
            id: $("#id_Setting").val()
        };
        showForm("Setting/Cancel", data);
    }, "¿Desea anular el Estado?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_Setting").val()
        };
        showForm("Setting/Revert", data);
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

