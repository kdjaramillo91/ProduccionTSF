
function LocationFilter_OnSelectedIndexChanged(s, e)
{

     
    CallBackGrid();
     

}

function LocationFilter_InitCallBack(s, e)
{
    var _idWarehouse = (idWarehouse.GetValue() === null) ? 0 : idWarehouse.GetValue();
    e.customArgs["idWareHouse"] = _idWarehouse;


}

function CallBackGrid()
{


    if (gridViewMoveDetails.cpVisibleRowCount === 0) return;

    /* Refresh Detail */
    var _idWarehouse = (idWarehouse.GetValue() === null) ? 0 : idWarehouse.GetValue();
    var _id_costCenter = (id_costCenter.GetValue() === null) ? 0 : id_costCenter.GetValue();
    var _id_subCostCenter = (id_subCostCenter.GetValue() === null) ? 0 : id_subCostCenter.GetValue();

    var data =
        {
            code: $("#codeDocumentType").val(),
            idWarehouse: _idWarehouse,
            id_costCenter: _id_costCenter,
            id_subCostCenter: _id_subCostCenter,
            deletedAll: true
        }


    gridViewMoveDetails.PerformCallback(data);

 
}

function LoteFilter_OnSelectedIndexChanged(s, e)
{
     

    //idItemFilter.ClearItems();
    //idItemFilter.SetValue(null);
    nameProvider.SetValue(null);
    nameCamaronera.SetValue(null);
    //nameItem.SetValue(null);
    
    var recordFilter = ((s.GetSelectedItem() === null) ? null : s.GetSelectedItem());
    if (recordFilter  != null)
    {
        nameProvider.SetValue(recordFilter.GetColumnText('nameProviderProductionLotSingleModelP') );
        nameCamaronera.SetValue(recordFilter.GetColumnText('nameUnitProviderProductionLotSingleModelP')  );
    }
    
    

    var idProductionLoteFilter = s.GetValue();
   // idItemFilter.PerformCallback({ id_ProductionLot: idProductionLoteFilter });


    CallBackGrid();
    
}


function CostCenter_OnSelectedIndexChanged(s, e)
{

     
    idCostSubCenter.ClearItems();
    idCostSubCenter.SetValue(null);
     
    var idCostCenterFilter = s.GetValue();
    idCostSubCenter.PerformCallback({ id_CostCenter: idCostCenterFilter });


    CallBackGrid();

}


function SubCostCenter_OnSelectedIndexChanged(s, e)
{
     
    CallBackGrid();

}

function ItemFilter_OnSelectedIndexChanged(s, e)
{

     
    nameItem.SetValue(null);
    //amoutItem.SetValue(0.00);

    var recordFilter = ((s.GetSelectedItem() == null) ? null : s.GetSelectedItem());

    if (recordFilter != null)
    {
       
        var descriptionItem = recordFilter.GetColumnText('descriptionModelP');        
        descriptionItem = (descriptionItem == null) ? recordFilter.GetColumnText('nameModelP') : descriptionItem;

        nameItem.SetValue(descriptionItem);
       // amoutItem.SetValue(recordFilter.GetColumnText('quantity'));
    }

    CallBackGrid();

}


function ButtonCancelCalculateDosage_Click(s, e)
{
    // 
    var _idWarehouse = (idWarehouse.GetValue() === null) ? 0 : idWarehouse.GetValue();

    var recordWarehouse = ((idWarehouse.GetSelectedItem() === null) ? null : idWarehouse.GetSelectedItem());
    var _codeWarehouse = null;
    if (recordWarehouse != null) {
        _codeWarehouse = recordWarehouse.GetColumnText('code');
    }

    idLoteFilter.SetValue(null);
    //idItemFilter
    amoutItem.SetValue(0.00);
    var dataLocation =
        {            
            codeWareHouse: _codeWarehouse
        }
    id_LocationFilter.SetValue(null);
    id_LocationFilter.PerformCallback(dataLocation);


    idCostCenter.SetValue(null);
    idCostSubCenter.SetValue(null);
    idItemFilter.SetValue(null);
    idItemFilter.PerformCallback(null);
    nameItem.SetValue(null);

    nameProvider.SetValue(null);
    nameCamaronera.SetValue(null);
    

    CallBackGrid();
}

function ButtonCalculateDosage_Click(s, e) {

    
    var _code = $("#codeDocumentType").val();
    var _natureMoveType = $("#natureMoveIM").val();
    
    var mensajeError = "";
    
       
    var _idProdutionLot = (idLoteFilter.GetValue() === null) ? 0 : idLoteFilter.GetValue();
    if (_idProdutionLot === 0) mensajeError = "Lote ";

    var _idItemMaster = (idItemFilter.GetValue() === null) ? 0 : idItemFilter.GetValue();
    if (_idProdutionLot === 0) mensajeError += "Producto ";

    var _quantity = (amoutItem.GetValue() == null) ? "0" : amoutItem.GetValue().toString();
    _quantity = _quantity.replace(".", ",");
    if (_quantity === "0") mensajeError += "Cantidad ";


    var _idWarehouse = (idWarehouse.GetValue() === null) ? 0 : idWarehouse.GetValue();
    if (_idWarehouse === 0) mensajeError += "Bodega ";

    var _idLocation = (id_LocationFilter.GetValue() === null) ? 0 : id_LocationFilter.GetValue();
    if (_idLocation === 0) mensajeError += "Ubicación ";

    var _idCostCenter = (idCostCenter.GetValue() === null) ? 0 : idCostCenter.GetValue();
    if (_idCostCenter === 0) mensajeError += "Centro de Costo ";

    var _idSubCostCenter = (idCostSubCenter.GetValue() === null) ? 0 : idCostSubCenter.GetValue();
    if (_idSubCostCenter === 0) mensajeError += "Sub-Centro de Costo ";

    if (mensajeError.length  > 0 )
    {
        mensajeError = "Debe seleccionar o ingresar información en estos campo(s):" + mensajeError;
        var errMsg = ErrorMessage(mensajeError);
        $("#infoInventoryMove").html(errMsg);
        return;
    }

    var data =
        {
            code: _code,
            natureMoveType: _natureMoveType,            
            idProdutionLot: _idProdutionLot,
            idItemMaster: _idItemMaster,
            quantity: _quantity,
            idWarehouse: _idWarehouse,
            idLocation: _idLocation,
            idCostCenter: _idCostCenter,
            idSubCostCenter: _idSubCostCenter
        }

    var reloadDetail = function (returnData)
    {

        

        if (returnData.codeReturn < 0)
        {
            $("#infoInventoryMove").html(returnData.message);

            return;
        }
        $("#infoInventoryMove").html("");
        
        var dataGridDetail =
                        {
                            code: _code,
                            idWarehouse: _idWarehouse,
                            deletedAll:false,
                            natureMoveType: _natureMoveType,
                            customParamOP: "IPXM"
                             
                        }
         
        gridViewMoveDetails.PerformCallback(dataGridDetail);
       // $("#wrapper_InventoryMoveDetailsEditFormPartial").html(partialView);

    };

    var actionBeforeSendFunction  = function ()
    {
        showLoading();
    }


    var actionCompleteFunction = function () {
        hideLoading();
    }

    genericAjaxCall("InventoryMove/SaveInventoryItemPackagingMaterials", true, data, null, actionBeforeSendFunction, reloadDetail, actionCompleteFunction)
    


}



