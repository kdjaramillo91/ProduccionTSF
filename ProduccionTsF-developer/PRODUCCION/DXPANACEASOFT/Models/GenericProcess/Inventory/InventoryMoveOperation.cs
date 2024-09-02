using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DXPANACEASOFT.Models.GenericProcess
{
    internal static class InventoryMoveOperationExtension
    {

        internal static IList<InventoryMove> FindCodeReason(this DbSet<InventoryMove> thisObj, string codeReason)
        {

            IList<InventoryMove> resultInventoryMove = thisObj            
                                                        //.AsParallel()
                                                        .Where(r => r.InventoryReason.code == codeReason)
                                                        .ToList();

            return resultInventoryMove;

        }

        internal static IList<InventoryMove> FindCodeReason(this DbSet<InventoryMove> thisObj, string codeReason, int[] idsDocument)
        {

            IList<InventoryMove> resultInventoryMove = thisObj
                                                        //.AsParallel()
                                                        .Where(r => idsDocument.Contains(r.id)  && r.InventoryReason.code == codeReason)
                                                        .ToList();

            return resultInventoryMove;

        }
    }
}