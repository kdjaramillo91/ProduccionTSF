function GridViewInventoryLineItemTypesDetails_BeginCallback(s, e) {
    e.customArgs["id_inventoryLine"] = $("#id_inventoryLine").val();
}

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvProductionSchedulePeriods.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvProductionSchedulePeriods !== null && gvProductionSchedulePeriods !== undefined) {
        gvProductionSchedulePeriods.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}

function PrintDocument(s, e) {
    var data = {
        id: $("#id_inventoryLine").val()};

    showPage("InventoryLine/InventoryLineDetailReport", data);
}

function UpdateName(s, e) {
    var dateStarAux = dateStar.GetDate();
    var paramDateStar = null;
    if (dateStarAux != null) {
        var strDate = String(dateStarAux);
        var strDateDiv2Points = strDate.split(":");
        var strDateDiv2PointsWithSpace = strDateDiv2Points[2].split(" ");
        paramDateStar = JSON.stringify(strDateDiv2Points[0] + ":" + strDateDiv2Points[1] + ":" + strDateDiv2PointsWithSpace[0]);
    }
    var dateEndAux = dateEnd.GetDate();
    var paramDateEnd = null;
    if (dateEndAux != null) {
        var strDate = String(dateEndAux);
        var strDateDiv2Points = strDate.split(":");
        var strDateDiv2PointsWithSpace = strDateDiv2Points[2].split(" ");
        paramDateEnd = JSON.stringify(strDateDiv2Points[0] + ":" + strDateDiv2Points[1] + ":" + strDateDiv2PointsWithSpace[0]);
    }

    var data = {
        dateStar: paramDateStar,
        dateEnd: paramDateEnd
    };
    $.ajax({
        url: "ProductionSchedulePeriod/UpdateName",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                productionSchedulePeriodName.SetText(result.name);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
    
}

