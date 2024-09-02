function TabControl_Init(s, e) {

}

function TabControl_ActiveTabChanged(s, e) {

}

function bs() { }


function ButtonUpdate_Click(s, e)
{
    Save(false);
}

function ButtonCancel_Click(s, e) {
    showPage("CustodianIncome/Index");
}


function AddNewDocument(s,e)
{
    showPageCallBackInit("CustodianIncome/Index", null, btnAsignarCustodio_click);
    //showPage("CustodianIncome/Index", null);
    //btnAsignarCustodio_click();

}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_custodianIncome").val()
        };
        showFormFunction("CustodianIncome/Cancel", data, function (result) {
            $("#mainform").html(result);
            UpdateView();

        });
    }, "¿Desea anular el ingreso de custodio?");
}

function SaveDocument(s, e) {
    Save(false);
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Save(true);
    }, "¿Desea autorizar el ingreso de custodio?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_custodianIncome").val()
        };
        showFormFunction("CustodianIncome/Revert", data, function (result) {
            $("#mainform").html(result);
            UpdateView();

        });
    }, "¿Desea reversar el ingreso de custodio?");
}

function OnEmissionDateValidation(s, e) {

}

function Save(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    let data = getData(aproved);
    showFormFunction("CustodianIncome/Save", data, function (result) {
        $("#mainform").html(result);
        UpdateView();

    });
}

function UpdateView() {
    
    var id = parseInt(document.getElementById("id_custodianIncome").getAttribute("idcustodianIncome"));

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    
    // STATES BUTTONS
    $.ajax({
        url: "CustodianIncome/Actions",
        type: "post",
        data: {
            id: id
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
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
             
        },
        complete: function (result) {
            hideLoading();
        }
    });

}

function getData(aproved) {
    
    let id = $("#id_custodianIncome").val();
    let id_RemissionGuide = $("#id_RemissionGuide").val();
    let val_FishingCustodianfield1 = fishingCustodianField2.GetValue();
    let val_FishingCustodianfield2 = fishingCustodianField2.GetValue();
    var data = "id=" + id + "&" + $("#formEditCustodianIncome").serialize()
        + "&approve=" + aproved
        + "&fishingCustodianfield1="+ val_FishingCustodianfield1
        + "&fishingCustodianfield2=" + val_FishingCustodianfield2
        + "&id_RemissionGuide=" + id_RemissionGuide;
    return data;
}

function Validate() {

    var validate = true;
    var errors = "";

    if (id_PersonCompanyCustodian1.GetValue() == null && id_PersonCompanyCustodian2.GetValue() == null) {
        errors = "Elegir empresa de custodia, es un campo Obligatorio. \n\r";
        UpdateTabImage({ isValid: false }, "tabCustodianControl");
        return messageValida(errors);
        //validate = false;
    }
    if (id_FishingCustodian1.GetValue() == null && id_FishingCustodian2.GetValue() == null) {
        errors = "Elegir sitio de pesca, es un campo Obligatorio. \n\r";
        UpdateTabImage({ isValid: false }, "tabCustodianControl");
        return messageValida(errors);
        //validate = false;
    }
    if (fishingCustodianField1.GetValue() == null && fishingCustodianField2.GetValue()) {
        errors = "El Valor de Custodio es un campo Obligatorio. \n\r";
        UpdateTabImage({ isValid: false }, "tabCustodianControl");
        return messageValida(errors);
        //validate = false;
    }
 
    return validate;
}


function messageValida(input) {
    let valida = true;
    if (input.length > 0) {
        valida = false;
        NotifyError("Error. " + input);
    }

    return valida;

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

//#region ComboCascada Fishing custodian Value
function id_FishingCustodian1_SelectedIndexChanged(s, e)
{
    
    var data = {
        id_FishingCustodian: s.GetValue(),
        controlName: "fishingCustodianField1",
        controlDependName: "id_FishingCustodian1"
    };
    $.ajax({
        url: "CustodianIncome/FishingCustodianValues",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            //da.SetValue(result.Provider_address)
        },
        complete: function () {
            fishingCustodianField1.PerformCallback();
            hideLoading();
        }
    });


}
function id_FishingCustodian2_SelectedIndexChanged(s, e) {
    
    var data = {
        id_FishingCustodian: s.GetValue(),
        controlName: "fishingCustodianField2",
        controlDependName: "id_FishingCustodian2"
    };
    $.ajax({
        url: "CustodianIncome/FishingCustodianValues",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            //da.SetValue(result.Provider_address)
        },
        complete: function () {
            fishingCustodianField2.PerformCallback();
            hideLoading();
        }
    });
}
//#endregion



$(function () {
    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    //init();
});