using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderProductionUnit
    {
        private static DBContext db = null;

        public static IEnumerable ProductionUnits()
        {
            db = new DBContext();
            return db.ProductionUnit.ToList();
        }

        public static ProductionUnit ProductionUnitById(int? id)
        {
            db = new DBContext();
            return db.ProductionUnit.FirstOrDefault(t => t.id == id);
        }

        public static string ProductionUnitNameByProcessId(int? productProcessid)
        {
            db = new DBContext();
            ProductionUnit model = new ProductionUnit();
            ProductionProcess proceso = db.ProductionProcess.FirstOrDefault(r => r.id == productProcessid);
            if (proceso != null)
            {
                model = db.ProductionUnit
                                .FirstOrDefault(r => r.id == proceso.id_ProductionUnit);
            }
            return model?.name;
        }
        public static IEnumerable ProductionUnitsByProcessReq(int? id_company, int? id_pruductionProcess, int? id_ProductionUnitCurrent)
        {
            db = new DBContext();
            IList<ProductionUnit> productionUnits = new List<ProductionUnit>();
            ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.id == id_pruductionProcess && p.id_company == id_company);
            if (process?.ProductionUnit != null  )
            {
                var productionUnitCurrent = db.ProductionUnit.FirstOrDefault(p => p.id == id_ProductionUnitCurrent);
                productionUnits.Add(productionUnitCurrent);
                //if (id_ProductionUnitCurrent != null && process.ProductionUnit.id == id_ProductionUnitCurrent)
                //{
                //    
                //}
                //List<ProductionUnit> _ProductionUnit = new List<ProductionUnit>();
                //_ProductionUnit.Add(process.ProductionUnit);
                //productionUnits = _ProductionUnit; //process.ProductionUnit.Where(w => w.id_company == id_company).OrderBy(u => u.id).ToList();
                //if (id_ProductionUnitCurrent != null && !productionUnits.Any(a => a.id == id_ProductionUnitCurrent))
                //{
                //    var productionUnitCurrent = db.ProductionUnit.FirstOrDefault(p => p.id == id_ProductionUnitCurrent);
                //    productionUnits.Add(productionUnitCurrent);
                //}
                //return productionUnits;
            }

            return productionUnits.ToList();
        }

        public static IEnumerable ProductionUnitsByProcess(int? id_company, int? id_pruductionProcess, int? id_ProductionUnitCurrent)
        {
            db = new DBContext();

            ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.id == id_pruductionProcess && p.id_company == id_company);
            if (process != null)
            {
                var productionUnits = process.ProductionUnit1.Where(w => w.id_company == id_company).OrderBy(u => u.id).ToList();
                if (id_ProductionUnitCurrent != null && !productionUnits.Any(a => a.id == id_ProductionUnitCurrent))
                {
                    var productionUnitCurrent = db.ProductionUnit.FirstOrDefault(p => p.id == id_ProductionUnitCurrent);
                    productionUnits.Add(productionUnitCurrent);
                }
                return productionUnits;
            }

            return db.ProductionUnit.Where(w => w.id_company == id_company).OrderBy(u => u.id).ToList();
        }

        public static IEnumerable ProductionUnitsByProcessAndBranchoffice(int? id_productionProcess, int? id_branchoffice)
        {
            db = new DBContext();

            ProductionProcess process = db.ProductionProcess.FirstOrDefault(p => p.id == id_productionProcess);
            List<ProductionUnit> _ProductionUnit = new List<ProductionUnit>();
            if (process?.ProductionUnit != null)
            {
                _ProductionUnit.Add(process.ProductionUnit);
            }
            
            return _ProductionUnit;
            //process?.ProductionUnit.Where(pu=> pu.id_branchOffice == id_branchoffice && pu.isActive).OrderBy(u => u.id).ToList() ?? db.ProductionUnit.OrderBy(u => u.id).ToList();

        }


        public static IEnumerable ProductionUnitsItems(int? id_productionUnit)
        {
            db = new DBContext();

            ProductionUnit productionUnit = db.ProductionUnit.FirstOrDefault(p => p.id == id_productionUnit);
            if (productionUnit != null)
            {
                return productionUnit.Item.OrderBy(u => u.id).ToList();
            }

            return new List<Item>();
        }
        public static IEnumerable AllProductionUnitRECsByCompany(int? id_company)
        {
            db = new DBContext();

            return db.ProductionUnit.Where(s => s.id_company == id_company && s.ProductionProcess != null && s.ProductionProcess.FirstOrDefault(fod => fod.code == "REC") != null).Select(s => new { id = s.id, name = s.name }).ToList();

        }

        public static IEnumerable ProductionUnitByCompany(int? id_company)
        {
            db = new DBContext();
            return db.ProductionUnit.Where(g => (g.isActive && g.id_company == id_company)).Select(p => new { p.id, name = p.name }).ToList();
        }
    }
}