using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DXPANACEASOFT.Models.GenericProcess
{
    internal static class InventoryMoveDetailOperationExtension
    {
        internal static IList<InventoryMoveDetail> FindInventoryMoveIds(this DbSet<InventoryMoveDetail> thisObj, int[] idsDocument)
        {

            IList<InventoryMoveDetail> resultInventoryMoveDetail = thisObj
                                                                    //.AsParallel()
                                                                    .Where(r => idsDocument.Contains(r.id_inventoryMove) )
                                                                    .ToList();

            return resultInventoryMoveDetail;

        }


    }
}