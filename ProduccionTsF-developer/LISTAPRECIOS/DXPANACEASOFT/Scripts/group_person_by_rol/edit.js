function PrintItem() {

}

function AddNewItem() {

    showLoading();

    var data = {
        id: 0,
        enabled: true
    }

    showPage("GroupPersonByRol/Edit", data);
}

function EditCurrentItem() {
    showLoading();

    var data = {
        id: $('#id').val(),
        enabled: true
    }

    showPage("GroupPersonByRol/Edit", data);
}

function SaveCurrentItem() {
    SaveItem();
}

function Validate() {

    var validate = true;
    var errors = "";

    if (!IsValid(TextBoxGroupName) || TextBoxGroupName.GetValue() == "" || TextBoxGroupName.GetValue() == null) {
        errors += "Nombre es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(TextBoxGroupDescription) || TextBoxGroupDescription.GetValue() == "" || TextBoxGroupDescription.GetValue() == null) {
        errors += "Descripción es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxRol) || ComboBoxRol.GetValue() == null) {
        errors += "Rol es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxCompany) || ComboBoxCompany.GetValue() == null) {
        errors += "Compañia es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (validate == false) {
        NotifyError("Error. " + errors);
    }

    return validate;
}
    
function SaveItem() {

    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'GroupPersonByRol/Save',
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
    var userData = {
        id: $('#id').val(),
        name: IsValid(TextBoxGroupName) ? TextBoxGroupName.GetText() : "",
        description: IsValid(TextBoxGroupDescription) ? TextBoxGroupDescription.GetText() : "",
        id_rol: IsValid(ComboBoxRol) ? ComboBoxRol.GetValue() : 0,
        isActive: IsValid(CheckBoxGroupActive) ? CheckBoxGroupActive.GetValue() : false,
    }

    var data = {
        json: JSON.stringify(userData)
    };

    return data;
}

function EditItem() {

}

function RemoveItems() {
}

function RefreshGrid() {

}

function Print() {
    //var data = { idGroup: $('#id').val() };
    //debugger;
    //$.ajax({
    //    url: 'GroupPersonByRol/PrintPriceListReport',
    //    type: 'post',
    //    data: data,
    //    async: true,
    //    cache: false,
    //    beforeSend: function () {
    //        showLoading();
    //    },
    //    success: function (result) {
    //        try {
    //            debugger;
    //            if (result != undefined) {
    //                var reportTdr = result.nameQS;
    //                var url = 'ReportProd/Index?trepd=' + reportTdr;
    //                newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
    //                newWindow.focus();
    //                hideLoading();
    //            }
    //        }
    //        catch (err) {
    //            hideLoading();
    //        }
    //    },
    //    complete: function () {
    //        hideLoading();
    //    }
    //});
}

function PrintItem() {
    var data = { idGroup: $('#id').val() };
    debugger;
    $.ajax({
        url: 'GroupPersonByRol/PrintGroupReport',
        type: 'post',
        data: data,
        async: true,
        cache: false,
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                debugger;
                if (result != undefined) {
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
                    newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                    newWindow.focus();
                    hideLoading();
                }
            }
            catch (err) {
                hideLoading();
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ExitItem() {
    showPage("GroupPersonByRol/Index");
}

function InitLoadPersonByRol() {

    if (ComboBoxCompany.GetValue() == null || ComboBoxRol.GetValue() == null) {
        NotifyDialog("Antes de Adicionar un elemento </br> Los campos <b>Compañia</b> y <b>Rol</b> son obligatorios. </br>" +
            "Cancele la acción adicionar, seleccione los valores y vuelva a intentarlo");
        return;
    }

    gvEditing.GetEditor("id_person").PerformCallback();
}

function LoadPersonByRol(s, e) {
    e.customArgs["id_company"] = ComboBoxCompany.GetValue();
    e.customArgs["id_personRol"] = ComboBoxRol.GetValue();
}

function init() {
}

$(function () {
    init();
});