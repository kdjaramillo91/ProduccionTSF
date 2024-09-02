function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    var visibleProvider = id_provider.GetValue() !== null;
    var visibleUnitProvider = id_productionUnitProvider.GetValue() !== null;
    // 
    if (valid && visibleProvider && visibleUnitProvider) {
        gvProductionUnitProviderPools.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvProductionUnitProviderPools !== null && gvProductionUnitProviderPools !== undefined) {
        gvProductionUnitProviderPools.CancelEdit();
    }
}

function OnCertification_EndCallback(s, e) {
    if (id_priceList.GetValue() !== null) {
        $.ajax({
            url: "ProductionUnitProviderPool/UpdateCertification",
            type: "post",
            data: null,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result !== null) {
                    id_certification.SetValue(result.id_certificationUpdate);
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}