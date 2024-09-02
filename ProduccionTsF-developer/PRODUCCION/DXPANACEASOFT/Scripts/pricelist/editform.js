// BUTTONS ACTIONS

function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

    if (!valid) {
        UpdateTabImage({ isValid: false }, "tabDocument");
    }

    if (gvPriceListDetails.cpRowsCount === 0 || gvPriceListDetails.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabDetails");
        return;
    }

    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var id = $("#id_priceList").val();

        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditPriceList").serialize();

        var url = (id === "0") ? "PriceList/PriceListPartialAddNew"
                               : "PriceList/PriceListPartialUpdate";

        showPage(url, data);
    }
}

function ButtonUpdate_Click(s, e) {
    Update(false);
}

function ButtonCancel_Click(s, e) {
    showPage("PriceList/Index", null);
}

//PRICELIS BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0
    };

    showPage("PriceList/PriceListEditForm", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    //ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    showPage("PriceList/PriceListCopy", { id: $("#id_priceList").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar la lista de Precio?");
}

function AutorizeDocument(s, e) {
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Autorize", data);
    //}, "¿Desea autorizar el documento?");
}

function ProtectDocument(s, e) {
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Protect", data);
    //}, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_priceList").val()
        };
        showForm("PriceList/Cancel", data);
    }, "¿Desea anular la Lista de Precio?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_priceList").val()
        };
        showForm("PriceList/Revert", data);
    }, "¿Desea reversar la Lista de Precio?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {
    var data = {
        id: $("#id_priceList").val()
    };

    showPage("PriceList/PriceListDetailReport", data);
}

// PRICELIST DETAILS ACTIONS BUTTONS

function AddNewDetail(s, e) {
    gvPriceListDetails.AddNewRow();
}

function RemoveDetail(s, e) {
}

function RefreshDetail(s, e) {
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvPriceListDetails.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            PriceListDetailsUpdateTitlePanel();
        });
    }
}

// COMBOBOX

function ItemCombo_OnInit(s, e) {
    //store actual filtering method and override
    var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
    s.filterStrategy.FilteringOnClient = function () {
        //create a new format string for all list box columns. you could skip this bit and just set
        //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
        //columns 1 and 3
        var lb = this.GetListBoxControl();
        var filterTextFormatStringItems = [];
        for (var i = 0; i < lb.columnFieldNames.length; i++) {
            filterTextFormatStringItems.push('{' + i + '}');
        }
        var filterTextFormatString = filterTextFormatStringItems.join(' ');

        //store actual format string and override with one for all columns
        var actualTextFormatString = lb.textFormatString;
        lb.textFormatString = filterTextFormatString;

        //call actual filtering method which will now work on our temporary format string
        actualFilteringOnClient.apply(this);

        //restore original format string
        lb.textFormatString = actualTextFormatString;
    };
}

function ItemCombo_SelectedIndexChanged(s, e) {
    UpdateItemInfo({
        id_item: s.GetValue()
    });
}

function ItemCombo_DropDown(s, e) {

    $.ajax({
        url: "PriceList/PriceListDetails",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            for (var i = 0; i < id_item.GetItemCount() ; i++) {
                var item = id_item.GetItem(i);
                if (result.indexOf(item.value) >= 0) {
                    id_item.RemoveItem(i);
                    i = -1;
                }
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function UpdateItemInfo(data) {

    if (data.id_item === null || data.quantityOrdered === null || data.price === null) {
        return;
    }

    masterCode.SetText("");
    purchasePrice.SetValue(0);
    salePrice.SetValue(0);

    if (id_item != null) {

        $.ajax({
            url: "PriceList/ItemDetailData",
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
                    console.log(result);
                    masterCode.SetText(result.masterCode);
                    purchasePrice.SetValue(result.purchasePrice);
                    salePrice.SetValue(result.salePrice);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

// PRICELIST DETAILS SELECTION

//var customCommand = "";

//function PriceListDetailsOnGridViewInit(s, e) {
//    PriceListDetailsUpdateTitlePanel();
//}

//function PriceListDetailsOnGridViewBeginCallback(s, e) {
//    customCommand = e.command;
//    PriceListDetailsUpdateTitlePanel();
//}

//function PriceListDetailsOnGridViewEndCallback(s, e) {
//    PriceListDetailsUpdateTitlePanel();
//    if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
//        s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
//    }
//}

//function PriceListDetailsOnGridViewSelectionChanged(s, e) {
//    PriceListDetailsUpdateTitlePanel();

//}

//function PriceListDetailsUpdateTitlePanel() {
//    var selectedFilteredRowCount = PriceListDetailsGetSelectedFilteredRowCount();

//    var text = "Total de elementos seleccionados: <b>" + gvPriceListDetails.GetSelectedRowCount() + "</b>";
//    var hiddenSelectedRowCount = gvPriceListDetails.GetSelectedRowCount() - PriceListDetailsGetSelectedFilteredRowCount();
//    if (hiddenSelectedRowCount > 0)
//        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
//    text += "<br />";
//    $("#lblInfo").html(text);

//    //if ($("#selectAllMode").val() != "AllPages") {
//    SetElementVisibility("lnkSelectAllRows", gvPriceListDetails.GetSelectedRowCount() > 0 && gvPriceListDetails.cpVisibleRowCount > selectedFilteredRowCount);
//    SetElementVisibility("lnkClearSelection", gvPriceListDetails.GetSelectedRowCount() > 0);
//    //}

//    btnRemoveDetail.SetEnabled(gvPriceListDetails.GetSelectedRowCount() > 0);
//}

//function PriceListDetailsGetSelectedFilteredRowCount() {
//    return gvPriceListDetails.cpFilteredRowCountWithoutPage + gvPriceListDetails.GetSelectedKeysOnPage().length;
//}

//function PriceListDetailsSelectAllRows() {
//    gvPriceListDetails.SelectRows();
//}

//function PriceListDetailsClearSelection() {
//    gvPriceListDetails.UnselectRows();
//}

//function SetElementVisibility(id, visible) {
//    var $element = $("#" + id);
//    visible ? $element.show() : $element.hide();
//}

//function AutoCloseAlert() {
//    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
//        setTimeout(function () {
//            $(".alert-success").alert('close');
//        }, 5000);
//    }
//}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "PriceList/InitializePagination",
        type: "post",
        data: { id_priceList: $("#id_priceList").val() },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            $("#pagination").attr("data-max-page", result.maximunPages);
            current_page = result.currentPage;
        },
        complete: function () {
        }
    });
    $('.pagination').current_page = current_page;
}

function UpdateViewEditForm() {
    var id = parseInt($("#id_priceList").val());
    //var id = parseInt($("#id_priceList").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);
    //btnCopy.SetEnabled(false);
    //
    btnAutorize.SetVisible(false);
    btnProtect.SetVisible(false);
    // STATES BUTTONS

    $.ajax({
        url: "PriceList/Actions",
        type: "post",
        data: { id: id },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            btnApprove.SetEnabled(result.btnApprove);
            //btnAutorize.SetEnabled(result.btnAutorize);
            //btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            //hideLoading();
        }
    });

    // HISTORY BUTTON
    btnHistory.SetEnabled(id !== 0);

    // PRINT BUTTON
    btnPrint.SetEnabled(id !== 0);
}

function init() {
    UpdatePagination();

    AutoCloseAlert();
}

$(function () {
    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateViewEditForm();
        }
    }, 100);

    init();
});