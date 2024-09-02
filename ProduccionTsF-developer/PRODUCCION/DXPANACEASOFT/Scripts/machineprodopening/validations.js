
function OnMachineProdOpeningEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "machineProdOpening");

    UpdateTabImage(e, "tabDocument");
}

// TABIMAGE

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function Turn_Validation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        $.ajax({
            url: "MachineProdOpening/IsValidScheduleTurn",
            type: "post",
            data: { id_turn: id_Turn.GetValue(), emissionDate: emissionDate.GetFormattedText() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null) {
                    if (result.isValid == 0) {
                        e.isValid = false;
                        e.errorText = result.Error;
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });

    }
}