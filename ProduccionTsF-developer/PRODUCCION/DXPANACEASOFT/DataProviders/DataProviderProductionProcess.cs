using System;
using System.Collections;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderProductionProcess
    {
        private static DBContext db = null;

        public static IEnumerable ProductionProcesses(int? id_company)
        {
            db = new DBContext();

            var model = db.ProductionProcess.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static IEnumerable ProductionProcessesWithoutRECWithCurrent(int id_company, int? id_ProductionProcessCurrent = null)
        {
            db = new DBContext();

            var model = db.ProductionProcess.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => (p.id_company == id_company && p.code != "REC"  && p.isActive) || p.id == id_ProductionProcessCurrent).ToList();
            }

            return model;
        }

        
        public static ProductionProcess ProductionProcessById(int? id)
        {
            //db = new DBContext();
            return ProductionProcess.GetOneById((id??0));
        }

        public static int? ProductionProcessByCode(string code, int? id_company)
        {
            db = new DBContext();
            return db.ProductionProcess.FirstOrDefault(t => t.code == code && t.id_company == id_company)?.id;
        }

        public static IEnumerable ProductionProcessByCodeSubProcessAndType(string codeSubProcess, string type)
        {
            db = new DBContext();
            return db.SubProcessIOProductionProcess.Where(t => t.SubProcess.code == codeSubProcess && t.type == type)
                                                   .Select(s => new { s.id, s.ProductionProcess.name }).ToList();
        }

        public static SubProcessIOProductionProcess SubProcessIOProductionProcessByCodeSubProcessTypeAndCodeProductionProcess(string codeSubProcess, string type, string codeProductionProcess)
        {
            db = new DBContext();
            return db.SubProcessIOProductionProcess.FirstOrDefault(t => t.SubProcess.code == codeSubProcess && t.type == type && t.ProductionProcess.code == codeProductionProcess);
        }

        public static SubProcessIOProductionProcess SubProcessIOProductionProcessById(int? id)
        {
            db = new DBContext();
            return db.SubProcessIOProductionProcess.FirstOrDefault(t => t.id == id);
        }

        public static Tuple<int?,string, int?, string> GetWarehouseAndLocationForInternProcess(int idProductionProcess)
        {
            db = new DBContext();
            int? idWarehouse = null;
            string warehouseName = null;
            int? idWarehouseLocation = null;
            string warehouseLocationName = null;
            var productionProcess = db.ProductionProcess
                                                .FirstOrDefault(r => r.id == idProductionProcess);

            if (productionProcess != null)
            {
                idWarehouse = productionProcess.id_warehouse;
                if (idWarehouse.HasValue)
                {
                    warehouseName = productionProcess.Warehouse.name;
                }
                idWarehouseLocation = productionProcess.id_WarehouseLocation;
                if (idWarehouseLocation.HasValue)
                {
                    warehouseLocationName = productionProcess.WarehouseLocation.name;
                }

            }

            return new Tuple<int?,string, int?,string>(idWarehouse, warehouseName, idWarehouseLocation, warehouseLocationName);
        }
    }
}