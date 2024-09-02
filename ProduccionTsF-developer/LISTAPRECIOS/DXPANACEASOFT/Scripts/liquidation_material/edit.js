
function OnBeginCallback_GridViewDetail(s, e) {
    e.customArgs["visibleCantidadCero"] = CheckBoxVerCantcero.GetValue();
}

function OnCheckedChanged() {
    GridViewDetail.PerformCallback();
}

function OnBatchEditEndEditing(s, e) {
    var canAproved = $('#canAproved').val();
    var canAuthorize = $('#canAuthorize').val();
    var enable = (canAproved == true || canAproved == "true" || canAproved == "True" ||
        canAuthorize == true || canAuthorize == "true" || canAuthorize == "True");
    if (enable) {
        var aprovedLogistIndex = s.GetColumnByField("aprovedLogist").index;
        var aproved = e.rowValues[aprovedLogistIndex].value;
        var descriptionLogistIndex = s.GetColumnByField("descriptionLogist").index;
        var description = e.rowValues[descriptionLogistIndex].value;
        if (canAproved == false || canAproved == "false" || canAproved == "False") {
            var aprovedComertialIndex = s.GetColumnByField("aprovedComertial").index;
            aproved = e.rowValues[aprovedComertialIndex].value;
            var descriptionComertialIndex = s.GetColumnByField("descriptionComertial").index;
            description = e.rowValues[descriptionComertialIndex].value;
        }

        if (aproved) {

            //var unitCostIndex = s.GetColumnByField("unitCostOrigin").index;
            //var unitCostValue = e.rowValues[unitCostIndex].value;
            //s.batchEditApi.SetCellValue(e.visibleIndex, "unitCost", unitCostValue, null, true);

            //var subTotalIndex = s.GetColumnByField("subTotalOrigin").index;
            //var subTotalValue = e.rowValues[subTotalIndex].value;
            //s.batchEditApi.SetCellValue(e.visibleIndex, "subTotal", subTotalValue, null, true);

            //var subTotalIvaIndex = s.GetColumnByField("subTotalIvaOrigin").index;
            //var subTotalIvaValue = e.rowValues[subTotalIvaIndex].value;
            //s.batchEditApi.SetCellValue(e.visibleIndex, "subTotalIva", subTotalIvaValue, null, true);

            //var totalIndex = s.GetColumnByField("totalOrigin").index;
            //var totalValue = e.rowValues[totalIndex].value;
            //s.batchEditApi.SetCellValue(e.visibleIndex, "total", totalValue, null, true);

            var idIndex = s.GetColumnByField("id").index;
            var idValue = e.rowValues[idIndex].value;

            UpdateAndRefreshGridSummary(idValue, aproved, canAproved, description);

        }
        else {
            //s.batchEditApi.SetCellValue(e.visibleIndex, "unitCost", 0, null, true);
            //var unitCostCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, "unitCost").parentNode;
            ////unitCostCell.style.backgroundColor = "#FFC7CE";

            //s.batchEditApi.SetCellValue(e.visibleIndex, "subTotal", 0, null, true);
            //var subTotalCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, "subTotal").parentNode;
            ////subTotalCell.style.backgroundColor = "#FFC7CE";

            //s.batchEditApi.SetCellValue(e.visibleIndex, "subTotalIva", 0, null, true);
            //var subTotalIvaCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, "subTotalIva").parentNode;
            ////subTotalIvaCell.style.backgroundColor = "#FFC7CE";

            //s.batchEditApi.SetCellValue(e.visibleIndex, "total", 0, null, true);
            //var totalCell = s.batchEditApi.GetCellTextContainer(e.visibleIndex, "total").parentNode;
            ////totalCell.style.backgroundColor = "#FFC7CE";

            var idIndex = s.GetColumnByField("id").index;
            var idValue = e.rowValues[idIndex].value;

            UpdateAndRefreshGridSummary(idValue, aproved, canAproved, description);
        }
    }
}

function UpdateAndRefreshGridSummary(id, aproved, canAproved, description) {
    var data = {
        id: id,
        aproved: aproved,
        canAproved: canAproved,
        description: description
    }

    $.ajax({
        url: "LiquidationMaterial/UpdateAndRefreshGridSummary",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            NotifyError("Error. " + error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            GridViewDetail.PerformCallback();
            GridViewSummary.PerformCallback();
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_liquidationMaterial').val(),
        ids: null,
        enabled: enabled
    }

    showPage("LiquidationMaterial/Edit", data);
}

function AddNewItem() {
    $.ajax({
        url: "LiquidationMaterial/ReceptionMaterialApproved",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            debugger;
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

function ObtenerDecimalParaGrabar(paramFloat) {
    var strParamFloat = paramFloat.toString();
    strParamFloat = strParamFloat.replace(".", ",");
    return strParamFloat;
}

function SaveDataUser(approved) {
    var emissionDateDocumentAux = emissionDateDocument.GetDate().getFullYear() + "-" +
        (emissionDateDocument.GetDate().getMonth() + 1) + "-" +
        emissionDateDocument.GetDate().getDate();
    var userData = {
        id: $('#id_liquidationMaterial').val(),
        emissionDateDocument: emissionDateDocumentAux,
        documentDescription: documentDescription.GetText(),
        approved: approved,

        LiquidationMaterialDetailDTO: []
    }

    for (let rowDetail = 0; rowDetail < GridViewDetail.cpDetailCount; rowDetail++) {
        var emisionGuia = GridViewDetail.batchEditApi.GetCellValue(rowDetail, 6);
        var emisionGuiaAux = emisionGuia.getFullYear() + "-" +
            (emisionGuia.getMonth() + 1) + "-" +
            emisionGuia.getDate();
        userData.LiquidationMaterialDetailDTO.push({
            id: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 0),
            id_guia: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 1),
            aprovedComertial: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 2) == null ? true : GridViewDetail.batchEditApi.GetCellValue(rowDetail, 2),
            aprovedLogist: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 3),
            id_guiaDetail: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 4),
            numberGuia: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 5),
            emisionGuia: emisionGuiaAux,
            id_item: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 7),
            id_metricUnit: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 10),
            quantityOrigin: ObtenerDecimalParaGrabar(GridViewDetail.batchEditApi.GetCellValue(rowDetail, 12)),
            unitCostOrigin: ObtenerDecimalParaGrabar(GridViewDetail.batchEditApi.GetCellValue(rowDetail, 13)),
            subTotalOrigin: ObtenerDecimalParaGrabar(GridViewDetail.batchEditApi.GetCellValue(rowDetail, 15)),
            iva: ObtenerDecimalParaGrabar(GridViewDetail.batchEditApi.GetCellValue(rowDetail, 17)),
            subTotalIvaOrigin: ObtenerDecimalParaGrabar(GridViewDetail.batchEditApi.GetCellValue(rowDetail, 18)),
            totalOrigin: ObtenerDecimalParaGrabar(GridViewDetail.batchEditApi.GetCellValue(rowDetail, 20)),
            descriptionLogist: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 22) == null ? "" : GridViewDetail.batchEditApi.GetCellValue(rowDetail, 22),
            descriptionComertial: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 23) == null ? "" : GridViewDetail.batchEditApi.GetCellValue(rowDetail, 23)
        });
    }

    //var drainingTest = {
    //    jsonDrainingTest: JSON.stringify(userData)
    //};

    return userData
}

function SaveItemAux(approved) {
    showLoading();

    //if (!Validate()) {
    //    hideLoading();
    //    return;
    //}

    $.ajax({
        url: 'LiquidationMaterial/Save',
        type: 'post',
        data: SaveDataUser(approved),
        async: true,
        cache: false,
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error. " + result.Message);
                return;
            }

            var id = result.Data;
            $('#id_liquidationMaterial').val(id);

            //if (aproved)
            //    AprovedItem();
            //else
            ShowCurrentItem(true);

            hideLoading();
            NotifySuccess("Elemento Guardado Satisfactoriamente.");

            //RedirecBack();
        },
        error: function (result) {
            hideLoading();
        },
    });
}

function SaveItem() {
    SaveItemAux(false);
}

function EditCurrentItem() {
    showLoading();
    ShowCurrentItem(true);
}

function ApproveDataUser() {
    var listApprove = [];
    var enabled = $('#enabled').val();
    if (enabled == true || enabled == "true" || enabled == "True") {
        for (let rowDetail = 0; rowDetail < GridViewDetail.cpDetailCount; rowDetail++) {
            listApprove.push({
                id: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 0),
                id_guia: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 1),
                aprovedLogist: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 2),
                descriptionLogist: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 22)
            });
        }
    }

    return listApprove
}

function AuthorizeDataUser() {
    var listAuthorize = [];
    //var enabled = $('#enabled').val();
    //if (enabled == true || enabled == "true" || enabled == "True") {
    for (let rowDetail = 0; rowDetail < GridViewDetail.cpDetailCount; rowDetail++) {
        listAuthorize.push({
            id: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 0),
            id_guia: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 1),
            aprovedComertial: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 2),
            descriptionComertial: GridViewDetail.batchEditApi.GetCellValue(rowDetail, 23) == null ? "" : GridViewDetail.batchEditApi.GetCellValue(rowDetail, 23)
        });
    }
    //}

    return listAuthorize
}

function AprovedItem() {

    if ($('#enabled').val() == true || $('#enabled').val() == "true" || $('#enabled').val() == "True") {
        SaveItemAux(true);
    } else {
        showLoading();
        $.ajax({
            url: 'LiquidationMaterial/Approve',
            type: 'post',
            data: { id: $('#id_liquidationMaterial').val() },
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
                NotifySuccess("Elemento Aprobado Satisfactoriamente. " + "Estado: " + result.Data);
            },
        });
    }
    
   
}

function AuthorizeItem() {
    showLoading();
    $.ajax({
        url: 'LiquidationMaterial/Authorize',
        type: 'post',
        data: { id: $('#id_liquidationMaterial').val(), listAuthorize: AuthorizeDataUser() },
        async: true,
        cache: false,
        error: function (result) {
            hideLoading();
            NotifyError("Error. " + result.Message);
        },
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error al Autorizar. " + result.Message);
                return;
            }

            ShowCurrentItem(false);
            hideLoading();
            NotifySuccess("Elemento Autorizado Satisfactoriamente. " + "Estado: " + result.Data);
        },
    });
}

function AnnulItem() {
    showLoading();
    $.ajax({
        url: 'LiquidationMaterial/Annul',
        type: 'post',
        data: { id: $('#id_liquidationMaterial').val() },
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
            NotifySuccess("Elemento Anulado Satisfactoriamente. " + "Estado: " + result.Data);
        },
    });
}

function ReverseItem() {
    showLoading();
    $.ajax({
        url: 'LiquidationMaterial/Reverse',
        type: 'post',
        data: { id: $('#id_liquidationMaterial').val() },
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
            NotifySuccess("Elemento Reversado Satisfactoriamente. " + "Estado: " + result.Data);
        },
    });
}

function PrintItem() {
    showLoading();
    hideLoading();
    NotifyError("Error al Imprimir. ");
    return;
}

function ButtonCancel_Click() {
    RedirecBack();
}

function RedirecBack() {
    showPage("LiquidationMaterial/Index");
}

function InitializePagination() {

    if ($("#id_liquidationMaterial").val() !== 0) {

        var current_page = 1;
        var max_page = 1;
        $.ajax({
            url: "LiquidationMaterial/InitializePagination",
            type: "post",
            data: { id: $("#id_liquidationMaterial").val() },
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
                showPage("LiquidationMaterial/Pagination", { page: page });
            }
        });
    }
}

$(function () {
    InitializePagination();
});