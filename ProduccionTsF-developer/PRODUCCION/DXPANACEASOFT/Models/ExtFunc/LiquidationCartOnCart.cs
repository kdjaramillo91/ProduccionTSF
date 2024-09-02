
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;
using DXPANACEASOFT.Models.FE;
using Utilitarios.Logs;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Models.ModelExtension;
using DXPANACEASOFT.Models.LiquidationCartOnCartP.LiquidationCartOnCartModel;
using System.Collections;

namespace DXPANACEASOFT.Models
{
    public partial class LiquidationCartOnCart
    {
        private DBContext db = new DBContext();
        public List<int> GetLiquidationsWithThisProductionLot()
        {
            List<int> lstLiquidations = db.LiquidationCartOnCart
                                            .Where(w => w.id_ProductionLot == this.id_ProductionLot
                                                        && w.Document.DocumentState.code.Equals("03"))
                                            .Select(s => s.id)
                                            .ToList();

            return lstLiquidations;
        }

        public bool ValidateProductionLotState()
        {
            string _statePL = db.ProductionLot
                                .FirstOrDefault(fod => fod.id == this.id_ProductionLot)?
                                .ProductionLotState?.code ?? "";

            return !(_statePL.Equals("01") || _statePL.Equals("02")) ? false : true;
        }

        public IEnumerable GenerateLiquidationCartOnCartDetail(List<int> idsLiquidation)
        {
            var lstLiqCartOnCart = db.LiquidationCartOnCartDetail.ToList();

            lstLiqCartOnCart = (from a in lstLiqCartOnCart
                                join b in idsLiquidation on a.id_LiquidationCartOnCart equals b
                                select a).ToList();

            return (from a in lstLiqCartOnCart
                    group a by a.id_ItemLiquidation into g
                    select new LiquidationCartOnCartDetailGroup
                    {
                        IdItemLiquidation = g.Key,
                        quantityBox = Convert.ToInt32(g.Sum(sq => sq.quatityBoxesIL)),
                        quantityKgsILSum = g.Sum(sqk => sqk.quantityKgsIL),
                        quantityPoundsILSum = g.Sum(sqp => sqp.quantityPoundsIL),
                    }).ToList();

        }
    }
}