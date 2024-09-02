function OnCodeFishingCustodianValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length >= 20) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        }
        else
        {
            $.ajax({
                url: "FishingCustodian/ValidateCodeFishingCustodian",
                type: "post",
                async: false,
                cache: false, data: {
                    id_FishingCustodian: gvFishingCustodian.cpEditingRowKey,
                    code: e.value
                },
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    e.isValid = result.isValid;
                    e.errorText = result.errorText;
                },
                complete: function () {
                    hideLoading();
                }
            });
        }
    }
}


function OnFishingSiteValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

 


function patrolValidate(s, e) {

}

function semiCompleteValidate(s, e) {

}

function truckDriverValidate(s, e) {

}

function changeHGValidate(s, e) {

}

function cabinHRValidate(s, e) {

}
