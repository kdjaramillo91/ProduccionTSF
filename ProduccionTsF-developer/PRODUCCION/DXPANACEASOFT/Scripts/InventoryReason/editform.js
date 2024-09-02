
//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvInventoryReason.UpdateEdit();
    }

    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabdetail", true);

    // if (!valid) {
    //     UpdateTabImage({ isValid: false }, "tabdetail");
    // }






    // if (valid) {
          
    //     var id = $("#id_InventoryReason").val();
   
         
      
    //     var data = "id=" + id + "&" +  $("#FormEditInventoryReason").serialize();
    //     var url = (id === "0") ? "InventoryReason/InventoryReasonPartialAddNew"
    //                           : "InventoryReason/InventoryReasonPartialUpdate";

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
    //showPage("InventoryReason/Index", null);
    if (gvInventoryReason !== null && gvInventoryReason !== undefined) {
        gvInventoryReason.CancelEdit();
    }
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("InventoryReason/FormEditInventoryReason", data);
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
            id: $("#id_InventoryReason").val()
        };
        showForm("InventoryReason/Cancel", data);
    }, "¿Desea anular el Estado?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_InventoryReason").val()
        };
        showForm("InventoryReason/Revert", data);
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
    //var id = parseInt($("#id_InventoryReason").val());

    //// EDITING BUTTONS
    //btnNew.SetEnabled(true);
    //btnSave.SetEnabled(false);
    //btnCopy.SetEnabled(id !== 0);

    //// STATES BUTTONS

    //$.ajax({
    //    url: "InventoryReason/Actions",
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



// COMPONENT FUNCTIONS

function OnChangeNatureMoveReason(s, e) {

    if (s.GetText() === "INGRESO") {
        valorization.SetValue("Manual");
        valorization.SetEnabled(true);
        typeOfCalculation.SetValue("Promedio");
        typeOfCalculation.SetEnabled(true);
        id_inventoryReasonRelated.SetValue(null);
        id_inventoryReasonRelated.SetEnabled(false);
        motivoCosto.SetValue("Ninguno");
        motivoCosto.SetEnabled(true);
        idMotivoEgreso.SetValue(null);
    }
    else {
        if (s.GetText() === "EGRESO") {
            valorization.SetValue("Automático");
            valorization.SetEnabled(false);
            typeOfCalculation.SetValue("Promedio");
            typeOfCalculation.SetEnabled(false);
            id_inventoryReasonRelated.SetValue(null);
            id_inventoryReasonRelated.SetEnabled(false);
        }
        else {
            valorization.SetValue(null);
            valorization.SetEnabled(false);
            typeOfCalculation.SetValue(null);
            typeOfCalculation.SetEnabled(false);
            id_inventoryReasonRelated.SetValue(null);
            id_inventoryReasonRelated.SetEnabled(false);
        }
        motivoCosto.SetValue(null);
        motivoCosto.SetEnabled(false);
        idMotivoEgreso.SetValue(null);
        idMotivoEgreso.SetEnabled(false);
    }
    
}


function OnChangeTypeOfCalculationReason(s, e) {

    if (s.GetText() === "Heredado") {
        id_inventoryReasonRelated.SetEnabled(true);
    }
    else {
        id_inventoryReasonRelated.SetValue(null);
        id_inventoryReasonRelated.SetEnabled(false);
    }
}


function OnChangeValorizationReason(s, e) {

    if (s.GetText() === "Manual") {
        id_inventoryReasonRelated.SetValue(null);
        id_inventoryReasonRelated.SetEnabled(false);
        typeOfCalculation.SetValue(null);
        typeOfCalculation.SetEnabled(true);
    }
    else {
        id_inventoryReasonRelated.SetValue(null);
        id_inventoryReasonRelated.SetEnabled(true);
        typeOfCalculation.SetValue("Heredado");
        typeOfCalculation.SetEnabled(false);
    }
}

function OnGridViewInventoryReasonBeginCallback(s, e) {

    //e.customArgs["txtItemsFilter"] = txtItemsFilter.GetText();
    // 
    customCommand = e.command;
    if (e.command == "UPDATEEDIT" /*|| e.command == "ADDNEWROW"|| e.command == "UPDATEROW"*/) {
        e.customArgs["valorization"] = valorization.GetValue();
        e.customArgs["typeOfCalculation"] = typeOfCalculation.GetValue();
        e.customArgs["id_inventoryReasonRelated"] = id_inventoryReasonRelated.GetValue();
    }
}

function OnCategoriaCostoValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
	}
}

function OnMotivoCostoInit(s, e) {
    motivoCosto.SetEnabled($('#codeNatureMove').val() === 'I');
}

function OnMotivoCostoValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnMotivoCostoSelectedIndexChanged(s, e) {
    var motivoCostoValue = s.GetText();
    if (motivoCostoValue === null || motivoCostoValue === "Ninguno" || motivoCostoValue === "") {
        
        idMotivoEgreso.SetValue(null);
        idMotivoEgreso.SetEnabled(false);
    }
    else {
        idMotivoEgreso.SetEnabled(true);
	}
};

function OnMotivoEgresoInit(s, e) {
    var motivoCostoValue =  motivoCosto.GetValue();
    if (motivoCostoValue === null || motivoCostoValue === 'Ninguno') {
        idMotivoEgreso.SetValue(null);
        idMotivoEgreso.SetEnabled(false);
    }
    else {
        idMotivoEgreso.SetEnabled(true);
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