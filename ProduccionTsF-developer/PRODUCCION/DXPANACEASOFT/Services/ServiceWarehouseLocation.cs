using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Services
{
    public class ServiceWarehouseLocation
    {
        public static WarehouseLocation CreateWarehouseLocationPerson(DBContext db, User activeUser, Company activeCompany, Warehouse warehouse, int id_person, string codeRol)
        {
            string result = "";
            WarehouseLocation warehouseLocation = new WarehouseLocation();
            try
            {

                #region WarehouseLocation

                var person = db.Person.FirstOrDefault(dt => dt.id == id_person);

                warehouseLocation.code = (codeRol + id_person);

                warehouseLocation.id_warehouse = warehouse.id;
                warehouseLocation.Warehouse = warehouse;

                warehouseLocation.name = "Ubicación de " + warehouse.name + " para el " + codeRol + ": " + person.identification_number;
                warehouseLocation.description = "Ubicación Creada Automáticamente en la Bodega: " + warehouse.name + " para el " + codeRol + ": " + person.fullname_businessName;
                warehouseLocation.id_person = id_person;
                warehouseLocation.Person = person;
                warehouseLocation.isRolling = false;

                warehouseLocation.id_company = activeCompany.id;

                warehouseLocation.isActive = true;
                warehouseLocation.id_userCreate = activeUser.id;
                warehouseLocation.dateCreate = DateTime.Now;
                warehouseLocation.id_userUpdate = activeUser.id;
                warehouseLocation.dateUpdate = DateTime.Now;

                db.WarehouseLocation.Add(warehouseLocation);

                #endregion
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            return warehouseLocation;
        }

        public static WarehouseLocation GetWarehouseLocationPerson(DBContext db, User ActiveUser, Company ActiveCompany, Warehouse warehouse, int id_person, string codeRol)
        {
            string result = "";
            WarehouseLocation warehouseLocation = db.WarehouseLocation.FirstOrDefault(fod=> fod.id_warehouse == warehouse.id && fod.code == (codeRol + id_person));
            try
            {
                if(warehouseLocation == null)
                {
                    warehouseLocation = CreateWarehouseLocationPerson(db, ActiveUser, ActiveCompany, warehouse, id_person, codeRol);
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            return warehouseLocation;
        }

    }
}