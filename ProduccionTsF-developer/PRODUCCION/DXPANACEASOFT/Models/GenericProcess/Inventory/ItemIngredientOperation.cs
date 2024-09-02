using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DXPANACEASOFT.Models.ModelExtension;

namespace DXPANACEASOFT.Models.GenericProcess
{
    internal static class ItemIngredientOperation
    {

        internal static ItemIngredientCalculate CalculateDosage(this DbSet<ItemHeadIngredient> thisObj, int idItem, decimal quantity, DBContext db )
        {

            Item _item = db.Item.FirstOrDefault(r => r.id == idItem);

            ItemIngredientCalculate _itemIngredientCalculate = new ItemIngredientCalculate();
            ItemHeadIngredient _itemHeadIngredient = thisObj
                                                        .FirstOrDefault(r => r.id_Item == idItem);

            if (_itemHeadIngredient == null) throw new Exception("No existe información de dosificación para Item:" + _item.masterCode);
            if (_itemHeadIngredient.amount == 0) throw new Exception("Información de dosificación incorrecta para Item:" + _item.masterCode+ " .Cantidad cero.");
            if (_itemHeadIngredient.id_metricUnit == null) throw new Exception("Información de dosificación incorrecta para Item:" + _item.masterCode + " .Unidad de Medida no definida.");

            _itemIngredientCalculate.idItem = idItem;
            _itemIngredientCalculate.amount = _itemHeadIngredient.amount ;
            _itemIngredientCalculate.idMetricUnit = _itemHeadIngredient.id_metricUnit?? 0;

            _itemIngredientCalculate.ItemIngredientDetail = db.ItemIngredient
                                                                .Where(r => r.id_compoundItem == idItem)
                                                                .Select(
                                                                         s => new ItemIngredientDetailCalculate
                                                                         {
                                                                             amountDetail = (s.amount ?? 0),
                                                                             idItemDetail = s.id_ingredientItem,
                                                                             idMetricUnitDetail = (s.id_metricUnit??0)

                                                                         })
                                                                        .ToList();



            decimal factorAmount = quantity / _itemIngredientCalculate.amount;

            _itemIngredientCalculate.ItemIngredientFinal = _itemIngredientCalculate
                                                                .ItemIngredientDetail
                                                                .Select(s => new ItemIngredientCalculateFinal
                                                                 {
                                                                     idItemCalculate = s.idItemDetail,
                                                                     idMetricUnitCalculate = s.idMetricUnitDetail,
                                                                     amountCalculate = s.amountDetail * factorAmount
                                                                })
                                                                .ToList();



            return _itemIngredientCalculate;

        }





    }



}