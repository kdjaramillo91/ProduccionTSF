//#region Controles
function onObtener_CostAllocation(s, e) {
    // Validar que los parametros tengan Valores
     
    s.SetEnabled(false);
    let _anio = anio.GetValue();
    let _mes = mes.GetValue();
    let _id_Warehouses = warehouses.GetValue();

    if (_anio == 0 || _mes == 0 || _id_Warehouses == null || _id_Warehouses.length == 0)
    {
        NotifyError("Debe ingresar año, mes y bodega(s)");
        s.SetEnabled(true);
        return;
    }

    var data = $("#formEditCostAllocation").serialize() + "&isApproved=false";
     
    let objCtl = s;
    showFormFunction("CostAllocation/CalculateInfo", data, function (result)
    {
        
        if (typeof result.status !== undefined && result.status !== null && result.status == "error") {
            NotifyError(result.message);
        }
        else
        {
            $("#mainform").html(result);
            NotifySuccess("Se han obtenido los movimientos para el Periodo y Bodega(s) indicados");
        }
        objCtl.SetEnabled(true);
        
    });
}
function OnSelectedIndexChanged_Warehouse(s, e) {

     
    //if (s.isValid)
    //{
    //    var data = $("#formEditCostAllocation").serialize();
    //    procesarFuncion("CostAllocation/GetEstateWareHousePeriod", data, function (result) {
    //         
    //        if (result.err.length === 0)
    //        {
    //            estadoPeriodoBodega.SetValue(result.estadoBodega);
    //        }
    //        else
    //        {
    //            estadoPeriodoBodega.SetValue("");
    //            NotifyError(result.err);
    //        }
    //
    //    });
    //}

        
    

}
function OnValueChange_Anio(s, e) {
    preWarehousessexCallback();
}
function OnValueChange_Mes(s, e)
{
    preWarehousessexCallback();
}
function preWarehousessexCallback()
{
    //let _anio = anio.GetValue();
    //let _mes = mes.GetValue();
    //if (typeof _anio == undefined || _anio == null || _anio == 0) return;
    //if (typeof _mes == undefined || _mes == null || _mes == 0) return;
    warehouses.PerformCallback();
}

var warehousex = "";
function id_Warehousessex_TokensChanged(s, e)
{
    warehousex = warehouses.GetValue();
    //id_Warehousessex.PerformCallback();
}
function OnCostAllocationBodegaTokenCallBack(s, e)
{
    //// 
    e.customArgs["anio"] = anio.GetValue();
    e.customArgs["mes"] = mes.GetValue();
    e.customArgs["id_Warehousessex"] = warehousex;
}
//#endregion

//#region Opcion
function ButtonCancel_Click() {
    RedirecBack();
}

function RedirecBack() {
    showPage("CostAllocation/Index");
}

function messageValida(input)
{
    let valida = true;
    if (input.length > 0)
    {
        valida = false;
        NotifyError("Error. " + input);
    }

    return valida;

}
function Validate() {

    var validate = true;
    var errors = "";

    if (emissionDate.GetValue() == null) {
        errors = "Fecha de emisión es un campo Obligatorio. \n\r";
        return messageValida(errors);
        //validate = false;
    }
    if (anio.GetValue() == 0) {
        errors = "El Año del Periodo es un campo Obligatorio. \n\r";
        return messageValida(errors);
        //validate = false;
    }
    if (mes.GetValue() == 0) {
        errors = "El Mes del Periodo es un campo Obligatorio. \n\r";
        return messageValida(errors);
        //validate = false;
    }
    
    let _id_Warehouses = warehouses.GetValue();
    if (_id_Warehouses == null || _id_Warehouses.length == 0) {
        errors = "Bodega(s) es un campo Obligatorio. \n\r";
        return messageValida(errors);
        //validate = false;
    }
    if (gvCostAllocationEditResumido.cpRowsCount === 0) {
        UpdateTabImage({ isValid: false }, "tabResumen");
        //validate = false;
        errors = "No se han obtenido los movimientos. \n\r";
        return messageValida(errors);
    } else
    {
        if (gvCostAllocationEditResumido.IsEditing())
        {
            UpdateTabImage({ isValid: false }, "tabResumen");
            //validate = false;
            errors = "Existen datos no guardados en el Resúmen de Costos. \n\r";
            return messageValida(errors);
        }
    }
        
    if (gvCostAllocationEditDetalle.cpRowsCount === 0 ||
        gvCostAllocationEditDetalle.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabDetalle");
        //validate = false;
        errors = "No se han calculados los costos asignados. \n\r";
        return messageValida(errors);
    }

    

    return validate;
}

function getData(aproved)
{
    var id = $("#id_costAllocation").val();
    var data = "id=" + id + "&" + $("#formEditCostAllocation").serialize() + "&isApproved=" + aproved;
    return data;
}

function Save(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    let data = getData(aproved);
    showFormFunction("CostAllocation/Save", data, function (result)
    {
        $("#mainform").html(result);
        UpdateView();

    });
}
//#endregion

//#region Toolbar
function AddNewCostAllocation(s, e) {
    var data = {
        id: 0
    };
    showPage("CostAllocation/Edit", data);
}
function SaveCostAllocation(s, e) {
    Save(false);

}
function CopyCostAllocation(s, e) {

}
function ApproveCostAllocation(s, e) {

     
    that = this;
    // validar 
    procesarFuncion("CostAllocation/ValidateWarehouseOpen", data, function (result) {
         
        if (result.err.length !== 0) {
            NotifyError(result.err);
        }
        else {
             
            that.Save(true);
        }

    },false);

    
    //var _id = $("#id_costAllocation").val();
    //var data = {
    //    id: _id
    //};
    //showFormFunction("CostAllocation/Aprovee", data, function (result)
    //{
    //    if (typeof result.status !== undefined && result.status !== null && result.status == "error") {
    //        NotifyError(result.message);
    //    }
    //    else {
    //        $("#mainform").html(result);
    //        UpdateView();
    //    }
    //});
}
function CancelCostAllocation(s, e) {

    var _id = $("#id_costAllocation").val();
    var data = {
        id: _id
    };
    showFormFunction("CostAllocation/Cancel", data, function (result)
    {
        if (typeof result.status !== undefined && result.status !== null && result.status == "error") {
            NotifyError(result.message);
        }
        else
        {
             
            $("#mainform").html(result);
            UpdateView();
        }

    });

}
function RevertCostAllocation(s, e) {
    var _id = $("#id_costAllocation").val();
    var data = {
        id: _id
    };
    showFormFunction("CostAllocation/Revert", data, function (result)
    {
        if (typeof result.status !== undefined && result.status !== null && result.status == "error")
        {
            NotifyError(result.message);
        }
        else {
            
            $("#mainform").html(result);
            UpdateView();
        }
    });
}
function PrintCostAllocation(s, e) {

}


function UpdateView() {
     
    var id = parseInt($("#id_costAllocation").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnUpdateMain.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnPrint.SetEnabled(false);
    btnObtenerCostAllocation.SetEnabled(false);
    btnDistribuirCosto.SetEnabled(false);

    // STATES BUTTONS
    $.ajax({
        url: "CostAllocation/Actions",
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
             
            var id = parseInt($("#id_costAllocation").val());
            if (id == 0) {
                btnApprove.SetEnabled(false);
            }
            else
            {
                btnApprove.SetEnabled(result.btnApprove);
            }
            
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
            btnPrint.SetEnabled(result.btnPrint);
            btnSave.SetEnabled(result.btnSave);
            btnUpdateMain.SetEnabled(result.btnSave);

             
            let enabled = (result.btnApprove | result.btnCancel);
            btnObtenerCostAllocation.SetEnabled(enabled );
            btnDistribuirCosto.SetEnabled(enabled );
        },
        complete: function (result) {
            //hideLoading();
        }
    });

}
// #endregion


//#region Toolbar Detalle
function RefreshDetail(s, e) {

}

function onDistribuir_CostAllocation(s, e) {
    
    s.SetEnabled(false);
    procesarFuncion("CostAllocation/CalculateDetalle", null, function (result) {
        s.SetEnabled(true);
        if (result.estado == "ERR") {
            NotifyError(result.err);
            return;
        }
        else
        {
            NotifySuccess("Se han distribuido los costo para los movimientos seleccionados");
        }
        gvCostAllocationEditDetalle.PerformCallback();
    },false);
}

//#endregion


$(function () {

    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});

