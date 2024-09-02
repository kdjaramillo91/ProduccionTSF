
function OnDateStarsValidation(s, e) {
    OnADateValidation(s, e, dateStar);
}

function OnDateEndsValidation(s, e) {
    OnADateValidation(s, e, dateEnd);
}

function OnADateValidation(s, e, aDate) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var strDate = String(aDate.GetDate());
        var strDateDiv2Points = strDate.split(":");
        var strDateDiv2PointsWithSpace = strDateDiv2Points[2].split(" ");
        var data = {
            aDate: JSON.stringify(strDateDiv2Points[0] + ":" + strDateDiv2Points[1] + ":" + strDateDiv2PointsWithSpace[0]),//"Mon Jul 10 2017 20:07:05"),//_emissionDate.GetDate()),
            id_productionSchedulePeriod: gvProductionSchedulePeriods.cpEditingRowKey
        };
        $.ajax({
            url: "ProductionSchedulePeriod/OnDateIncludedInPeriodsValidation",
            type: "post",
            data: data,
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
                    if (result.itsIncluded == 1) {
                        e.isValid = false;
                        e.errorText = result.Error;
                    } else {
                        OnRangeDateValidation(e, dateStar.GetValue(), dateEnd.GetValue(), "Rango de Fecha no válido");
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}


