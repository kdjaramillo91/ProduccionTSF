function PrintItem() {

}

function AddNewItem() {

    showLoading();

    var data = {
        id: 0,
        enabled: true
    }

    showPage("PenalityClassShrimp/Edit", data);
}

function EditCurrentItem() {
    showLoading();

    var data = {
        id: $('#id').val(),
        enabled: true
    }

    showPage("PenalityClassShrimp/Edit", data);
}

function SaveCurrentItem() {
    SaveItem();
}

function Validate() {

    var validate = true;
    var errors = "";
    if ((IsValid(rbParaProveedor) && !rbParaProveedor.GetValue()) && (IsValid(rbParaGrupo) && !rbParaGrupo.GetValue())){
        errors += "Seleccione Proveedor o Grupo. \n\r";
        validate = false;
    }
    if ((!IsValid(rbParaProveedor) || rbParaProveedor.GetValue()) &&
        (!IsValid(ComboBoxPenalityProveedores) || ComboBoxPenalityProveedores.GetValue() == null)) {
        errors += "Seleccione un Proveedor. \n\r";
        validate = false;
    }
    if ((!IsValid(rbParaGrupo) || rbParaGrupo.GetValue()) &&
        (!IsValid(ComboBoxPenalityGrupos) || ComboBoxPenalityGrupos.GetValue() == null)) {
        errors += "Seleccione un Grupo. \n\r";
        validate = false;
    }

    if (validate == false) {
        NotifyError("Error. " + errors);
    }

    return validate;
}

function SaveItem() {
    //debugger;

    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'PenalityClassShrimp/Save',
        type: 'post',
        data: SaveDataUser(),
        async: true,
        cache: false,
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error. " + result.Message);
                return;
            }

            hideLoading();
            NotifySuccess("Elemento Guardado Satisfactoriamente.");
            var id = result.Data;
            $('#id').val(id);
        },
        error: function (result) {
            hideLoading();
        },
    });
} 

function IsValid(object) {
    return (object != null && object != undefined && object != "undefined");
}

function SaveDataUser() {
    //debugger;
    var userData = {
        id: $('#id').val(),
        byProvider: IsValid(rbParaProveedor) ? (rbParaProveedor.GetValue() == null ? false : rbParaProveedor.GetValue()) : false,
        id_provider: IsValid(ComboBoxPenalityProveedores) ? ComboBoxPenalityProveedores.GetValue() : 0,
        id_groupPersonByRol: IsValid(ComboBoxPenalityGrupos) ? ComboBoxPenalityGrupos.GetValue() : 0,

        priceListPenalty: []
    }

    for (let rowPenalty = 0; rowPenalty < GridViewPenaltyDetails.pageRowCount; rowPenalty++) {
        userData.priceListPenalty.push({
            id_classShrimp: GridViewPenaltyDetails.batchEditApi.GetCellValue(rowPenalty, 0),
            value: parseFloat(GridViewPenaltyDetails.batchEditApi.GetCellValue(rowPenalty, 2)).toFixed(4),
        });
    }

    var penality = {
        jsonPenality: JSON.stringify(userData)
    };

    return penality;
}

function EditItem() {

}

function RemoveItems() {
}

function RefreshGrid() {

}

function Print() {

}

function ExitItem() {
    showPage("PenalityClassShrimp/Index");
}

function RadioProveedoresGrupo_Click() {

    if (rbParaProveedor.GetValue()) {
        ComboBoxPenalityGrupos.SetValue(null);
        document.getElementById("divComboBoxProveedores").style.display = "block";
        document.getElementById("divComboBoxGrupos").style.display = "none";  
    }
    else {
        ComboBoxPenalityProveedores.SetValue(null);
        document.getElementById("divComboBoxProveedores").style.display = "none";
        document.getElementById("divComboBoxGrupos").style.display = "block";
    }
} 

function init() {
}

$(function () {
    init();
    RadioProveedoresGrupo_Click();
});