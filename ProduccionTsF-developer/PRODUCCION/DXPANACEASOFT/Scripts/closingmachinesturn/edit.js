
function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_closingMachinesTurn').val(),
        id_MachineProdOpeningDetail: 0,
        enabled: enabled
    };

    showPage("ClosingMachinesTurn/Edit", data);
}

function AddNewItem() {
    $.ajax({
        url: "ClosingMachinesTurn/PendingNew",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            //// 
            NotifyError("Error. " + error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#maincontent").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function EditCurrentItem() {
    showLoading();
    ShowCurrentItem(true);
}

function SaveCurrentItem() {
    SaveItem(false);
}

function AprovedItem() {
    showLoading();
    $.ajax({
        url: 'ClosingMachinesTurn/Approve',
        type: 'post',
        data: { id: $('#id_closingMachinesTurn').val() },
        async: true,
        cache: false,
        error: function (result) {
            hideLoading();
            NotifyError("Error. " + result.Message);
        },
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error al Aprobar. " + result.Message);
                return;
            }

            ShowCurrentItem(false);
            hideLoading();
            NotifySuccess("Cierre de Apertura - Turno Aprobada Satisfactoriamente. " + "Estado: " + result.Data);
        }
    });
}

function AprovedCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Cierre de Apertura - Turno?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            if ($("#enabled").val() == "true") {
                SaveItem(true);
            } else {
                AprovedItem();
            }
        }
    });
}

function ReverseCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Cierre de Apertura - Turno?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'ClosingMachinesTurn/Reverse',
                type: 'post',
                data: { id: $('#id_closingMachinesTurn').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Reversar. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Cierre de Apertura - Turno Reversada Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function AnnulCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Anular el Cierre de Apertura - Turno?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'ClosingMachinesTurn/Annul',
                type: 'post',
                data: { id: $('#id_closingMachinesTurn').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Anular. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Cierre de Apertura - Turno Anulada Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function SaveDataUser() {

    var userData = {
        id: $('#id_closingMachinesTurn').val(),
        //id_machineProdOpening: $('#id_machineProdOpening').val(),
        dateTimeEmision: DateTimeEmision.GetValue(),
        description: MemoDescription.GetText()
        
    };

    var ClosingMachinesTurn = {
        jsonClosingMachinesTurn: JSON.stringify(userData)
    };

    return ClosingMachinesTurn;
}

function SaveItem(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'ClosingMachinesTurn/Save',
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

            var id = result.Data;
            $('#id_closingMachinesTurn').val(id);

            if (aproved)
                AprovedItem();
            else
                ShowCurrentItem(true);

            hideLoading();
            NotifySuccess("Cierre de Apertura - Turno Guardada Satisfactoriamente.");
        },
        error: function (result) {
            hideLoading();
        }
    });
}

function IsValid(object) {
    return (object != null && object != undefined && object != "undefined");
}

function Validate() {
    var validate = true;
    var errors = "";

    if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() == null) {
        errors += "Fecha Emisión es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (validate == false) {
        NotifyError("Error. " + errors);
    }

    return validate;
}

function ButtonUpdate_Click() {
    SaveItem(false);
}

function ButtonCancel_Click() {
    RedirecBack();
}

function RedirecBack() {
    showPage("ClosingMachinesTurn/Index");
}

function InitializePagination() {

    if ($("#id_closingMachinesTurn").val() !== 0) {

        var current_page = 1;
        var max_page = 1;
        $.ajax({
            url: "ClosingMachinesTurn/InitializePagination",
            type: "post",
            data: { id: $("#id_closingMachinesTurn").val() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                max_page = result.maximunPages;
                current_page = result.currentPage;
            },
            complete: function () {
            }
        });

        $('.pagination').jqPagination({
            current_page: current_page,
            max_page: max_page,
            page_string: "{current_page} de {max_page}",
            paged: function (page) {
                showPage("ClosingMachinesTurn/Pagination", { page: page });
            }
        });
    }
}

function Init() {
}

$(function () {
    InitializePagination();
    Init();
});