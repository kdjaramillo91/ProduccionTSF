using DevExpress.Web.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.IO;
using System.Configuration;

namespace DXPANACEASOFT.Controllers
{
    public class VehicleController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        #region VEHICLE GRIDVIEW

        [ValidateInput(false)]
        public ActionResult VehiclePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Vehicle.FirstOrDefault(b => b.id == keyToCopy);
            }
            List<Vehicle> model = new List<Vehicle>();
            string ruta = ConfigurationManager.AppSettings["rutalog"];
            try
            {
                model = db.Vehicle
                            .Where(o => o.id_company == this.ActiveCompanyId)
                            .ToList()
                            .Select(s => new Vehicle
                            {
                                id = s.id,
                                carRegistration = s.carRegistration,
                                mark = s.mark,
                                model = s.model,
                                isOwn = s.isOwn,
                                description = s.description,
                                isActive = s.isActive,
                                id_company = s.id_company,
                                id_userCreate = s.id_userCreate,
                                dateCreate = s.dateCreate,
                                id_userUpdate = s.id_userUpdate,
                                dateUpdate = s.dateUpdate,
                                id_VehicleType = s.id_VehicleType,
                                id_itemColor = s.id_itemColor,
                                id_personOwner = s.id_personOwner,
                                ItemColor = DataProviderItemColor.ItemColorById(s.id_itemColor),
                                VehicleType = DataProviderVehicleType.VehicleType(s.id_VehicleType),
                                id_providerT = DataProviderVehicle.VehicleProviderTransportistId(s.id),
								rucNameProvider = DataProviderVehicle.VehicleRucProviderTransportis(s.id),
								nameProvider = DataProviderVehicle.VehicleProviderTransportis(s.id),
                                id_providerTBilling = DataProviderVehicle.VehicleProviderTransportistBillingId(s.id),
								rucNameProviderBilling = DataProviderVehicle.VehicleRucProviderTransportistBilling(s.id),
								nameProviderBilling = DataProviderVehicle.VehicleProviderTransportistBilling(s.id),
                                hunterLockText = s.hunterLockText
                            }).ToList();
            }
            catch (Exception ex)
            {
                Utilitarios.Logs.MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "vehiclePartial", "PROD");
            }
            
            return PartialView("_VehiclePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VehiclePartialAddNew(DXPANACEASOFT.Models.Vehicle item)
        {
            //int id_itemColor = 0;

            Vehicle vNew = new Vehicle();
            VeicleProviderTransport vptTmpNew = null;
            VehicleProviderTransportBilling vptbTmpNew = null;

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                   try
                    {
                        if (item.id_providerT != null && item.id_providerT != 0)
                        {
                            vptTmpNew = new VeicleProviderTransport();
                            vptTmpNew.id_vehicle = item.id;
                            vptTmpNew.id_Provider = item.id_providerT;
                            
                            vptTmpNew.dateinit = DateTime.Now;
                            vptTmpNew.Estado = true;

                            db.VeicleProviderTransport.Attach(vptTmpNew);
                            db.Entry(vptTmpNew).State = EntityState.Added;
                        }

                        if (item.id_providerTBilling != null && item.id_providerTBilling != 0)
                        {
                            vptbTmpNew = new VehicleProviderTransportBilling();
                            vptbTmpNew.id_vehicle = item.id;
                            vptbTmpNew.id_provider = (int)item.id_providerTBilling;
                            vptbTmpNew.dateinit = DateTime.Now;

                            vptbTmpNew.state = true;
                            db.VehicleProviderTransportBilling.Attach(vptbTmpNew);
                            db.Entry(vptbTmpNew).State = EntityState.Added;
                        }

                        vNew.carRegistration = item.carRegistration;
                        vNew.mark = item.mark;
                        vNew.model = item.model;
                        vNew.isOwn = item.isOwn;
                        vNew.description = item.description;
                        vNew.isActive = item.isActive;
                        vNew.id_personOwner = item.id_personOwner;
                        vNew.id_VehicleType = item.id_VehicleType;
                        vNew.id_itemColor = item.id_itemColor;
                        vNew.hunterLockText = item.hunterLockText;
                        vNew.hasHunterDevice = item.hasHunterDevice;

                        vNew.id_company = this.ActiveCompanyId;
                        vNew.id_userCreate = ActiveUser.id;
                        vNew.dateCreate = DateTime.Now;
                        vNew.id_userUpdate = ActiveUser.id;
                        vNew.dateUpdate = DateTime.Now;
                        
                        db.Vehicle.Add(vNew);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Vehículo: " + item.mark + " guardado exitosamente");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        ViewData["EditMessage"] = ErrorMessage(ex.Message);
                    }
                }

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            List<Vehicle> model = new List<Vehicle>();
            string ruta = ConfigurationManager.AppSettings["rutalog"];
            try
            {
                model = db.Vehicle
                            .Where(o => o.id_company == this.ActiveCompanyId)
                            .ToList()
                            .Select(s => new Vehicle
                            {
                                id = s.id,
                                carRegistration = s.carRegistration,
                                mark = s.mark,
                                model = s.model,
                                isOwn = s.isOwn,
                                description = s.description,
                                isActive = s.isActive,
                                id_company = s.id_company,
                                id_userCreate = s.id_userCreate,
                                dateCreate = s.dateCreate,
                                id_userUpdate = s.id_userUpdate,
                                dateUpdate = s.dateUpdate,
                                id_VehicleType = s.id_VehicleType,
                                id_itemColor = s.id_itemColor,
                                id_personOwner = s.id_personOwner,
                                ItemColor = DataProviderItemColor.ItemColorById(s.id_itemColor),
                                VehicleType = DataProviderVehicleType.VehicleType(s.id_VehicleType),
                                id_providerT = DataProviderVehicle.VehicleProviderTransportistId(s.id),
								rucNameProvider = DataProviderVehicle.VehicleRucProviderTransportis(s.id),
								nameProvider = DataProviderVehicle.VehicleProviderTransportis(s.id),
                                id_providerTBilling = DataProviderVehicle.VehicleProviderTransportistBillingId(s.id),
                                rucNameProviderBilling = DataProviderVehicle.VehicleRucProviderTransportistBilling(s.id),
								nameProviderBilling = DataProviderVehicle.VehicleProviderTransportistBilling(s.id),
								hunterLockText = s.hunterLockText
                            }).ToList();
            }
            catch (Exception ex)
            {
                Utilitarios.Logs.MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "vehiclePartial", "PROD");
            }
            return PartialView("_VehiclePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VehiclePartialUpdate(DXPANACEASOFT.Models.Vehicle item)
        {
            VeicleProviderTransport vptTmp = null;
            VeicleProviderTransport vptTmpNew = null;

            VehicleProviderTransportBilling vptbTmp = null;
            VehicleProviderTransportBilling vptbTmpNew = null;

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Vehicle.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            vptTmp = db.VeicleProviderTransport.FirstOrDefault(fod => fod.id_vehicle == item.id && fod.datefin == null && fod.Estado == true);
                            if (vptTmp == null)
                            {
                                vptTmpNew = new VeicleProviderTransport();
                                vptTmpNew.id_vehicle = item.id;
                                vptTmpNew.id_Provider = item.id_providerT;
                                vptTmpNew.dateinit = DateTime.Now;
                                vptTmpNew.Estado = true;

                                db.VeicleProviderTransport.Attach(vptTmpNew);
                                db.Entry(vptTmpNew).State = EntityState.Added;
                            }
                            else
                            {
                                if (vptTmp.id_Provider != item.id_providerT)
                                {
                                    vptTmp.datefin = DateTime.Now;
                                    vptTmp.Estado = false;

                                    db.VeicleProviderTransport.Attach(vptTmp);
                                    db.Entry(vptTmp).State = EntityState.Modified;

                                    vptTmpNew = new VeicleProviderTransport();
                                    vptTmpNew.id_vehicle = item.id;
                                    vptTmpNew.id_Provider = item.id_providerT;
                                    vptTmpNew.dateinit = DateTime.Now;
                                    vptTmpNew.Estado = true;

                                    db.VeicleProviderTransport.Attach(vptTmpNew);
                                    db.Entry(vptTmpNew).State = EntityState.Added;
                                }
                            }
                            vptbTmp = db.VehicleProviderTransportBilling.FirstOrDefault(fod => fod.id_vehicle == item.id && fod.datefin == null && fod.state == true);
                            if (vptbTmp == null)
                            {
                                vptbTmpNew = new VehicleProviderTransportBilling();
                                vptbTmpNew.id_vehicle = item.id;
                                vptbTmpNew.id_provider = (int)item.id_providerTBilling;
                                vptbTmpNew.dateinit = DateTime.Now;
                                vptbTmpNew.state = true;

                                db.VehicleProviderTransportBilling.Attach(vptbTmpNew);
                                db.Entry(vptbTmpNew).State = EntityState.Added;
                            }
                            else
                            {
                                if (vptbTmp.id_provider != item.id_providerTBilling)
                                {
                                    vptbTmp.datefin = DateTime.Now;
                                    vptbTmp.state = false;

                                    db.VehicleProviderTransportBilling.Attach(vptbTmp);
                                    db.Entry(vptbTmp).State = EntityState.Modified;

                                    vptbTmpNew = new VehicleProviderTransportBilling();
                                    vptbTmpNew.id_vehicle = item.id;
                                    vptbTmpNew.id_provider = (int)item.id_providerTBilling;
                                    vptbTmpNew.dateinit = DateTime.Now;
                                    vptbTmpNew.state = true;

                                    db.VehicleProviderTransportBilling.Attach(vptbTmpNew);
                                    db.Entry(vptbTmpNew).State = EntityState.Added;
                                }
                            }


                            modelItem.carRegistration = item.carRegistration;
                            modelItem.mark = item.mark;
                            modelItem.model = item.model;
                            modelItem.isOwn = item.isOwn;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;
                            modelItem.id_personOwner = item.id_personOwner;
                            modelItem.id_VehicleType = item.id_VehicleType;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;
                            modelItem.id_itemColor = item.id_itemColor;
                            modelItem.hunterLockText = item.hunterLockText;
                            modelItem.hasHunterDevice = item.hasHunterDevice;                                

                            db.Vehicle.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Vehículo: " + item.mark + " guardado exitosamente");
                        }
                            
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(ex.Message);
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }
            List<Vehicle> model = new List<Vehicle>();
            string ruta = ConfigurationManager.AppSettings["rutalog"];
            try
            {
                model = db.Vehicle
                            .Where(o => o.id_company == this.ActiveCompanyId)
                            .ToList()
                            .Select(s => new Vehicle
                            {
                                id = s.id,
                                carRegistration = s.carRegistration,
                                mark = s.mark,
                                model = s.model,
                                isOwn = s.isOwn,
                                description = s.description,
                                isActive = s.isActive,
                                id_company = s.id_company,
                                id_userCreate = s.id_userCreate,
                                dateCreate = s.dateCreate,
                                id_userUpdate = s.id_userUpdate,
                                dateUpdate = s.dateUpdate,
                                id_VehicleType = s.id_VehicleType,
                                id_itemColor = s.id_itemColor,
                                id_personOwner = s.id_personOwner,
                                ItemColor = DataProviderItemColor.ItemColorById(s.id_itemColor),
                                VehicleType = DataProviderVehicleType.VehicleType(s.id_VehicleType),
                                id_providerT = DataProviderVehicle.VehicleProviderTransportistId(s.id),
                                rucNameProvider = DataProviderVehicle.VehicleRucProviderTransportis(s.id),
								nameProvider = DataProviderVehicle.VehicleProviderTransportis(s.id),
								id_providerTBilling = DataProviderVehicle.VehicleProviderTransportistBillingId(s.id),
                                rucNameProviderBilling = DataProviderVehicle.VehicleRucProviderTransportistBilling(s.id),
								nameProviderBilling = DataProviderVehicle.VehicleProviderTransportistBilling(s.id),
								hunterLockText = s.hunterLockText
                            }).ToList();
            }
            catch (Exception ex)
            {
                Utilitarios.Logs.MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "vehiclePartial", "PROD");
            }
            //var model = db.Vehicle.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_VehiclePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VehiclePartialDelete(System.Int32 id)
        {
        
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                {
                    var item = db.Vehicle.FirstOrDefault(it => it.id == id);
                    if (item != null)
                    {
                        item.isActive = false;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                        
                        
                    }
                        db.Vehicle.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Vehículo: " + (item?.mark ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }
            List<Vehicle> model = new List<Vehicle>();
            string ruta = ConfigurationManager.AppSettings["rutalog"];
            try
            {
                model = db.Vehicle
                            .Where(o => o.id_company == this.ActiveCompanyId)
                            .ToList()
                            .Select(s => new Vehicle
                            {
                                id = s.id,
                                carRegistration = s.carRegistration,
                                mark = s.mark,
                                model = s.model,
                                isOwn = s.isOwn,
                                description = s.description,
                                isActive = s.isActive,
                                id_company = s.id_company,
                                id_userCreate = s.id_userCreate,
                                dateCreate = s.dateCreate,
                                id_userUpdate = s.id_userUpdate,
                                dateUpdate = s.dateUpdate,
                                id_VehicleType = s.id_VehicleType,
                                id_itemColor = s.id_itemColor,
                                id_personOwner = s.id_personOwner,
                                ItemColor = DataProviderItemColor.ItemColorById(s.id_itemColor),
                                VehicleType = DataProviderVehicleType.VehicleType(s.id_VehicleType),
                                id_providerT = DataProviderVehicle.VehicleProviderTransportistId(s.id),
                                rucNameProvider = DataProviderVehicle.VehicleRucProviderTransportis(s.id),
								nameProvider = DataProviderVehicle.VehicleProviderTransportis(s.id),
								id_providerTBilling = DataProviderVehicle.VehicleProviderTransportistBillingId(s.id),
                                rucNameProviderBilling = DataProviderVehicle.VehicleRucProviderTransportistBilling(s.id),
								nameProviderBilling = DataProviderVehicle.VehicleProviderTransportistBilling(s.id),
								hunterLockText = s.hunterLockText
                            }).ToList();
            }
            catch (Exception ex)
            {
                Utilitarios.Logs.MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "vehiclePartial", "PROD");
            }
            //var model = db.Vehicle.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_VehiclePartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedVehicle(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var car = db.Vehicle.Where(i => ids.Contains(i.id));
                            foreach (var vehicle in car)
                            {
                                vehicle.isActive = false;

                                vehicle.id_userUpdate = ActiveUser.id;
                                vehicle.dateUpdate = DateTime.Now;

                                db.Vehicle.Attach(vehicle);
                                db.Entry(vehicle).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Vehículos desactivados exitosamente");
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            ViewData["EditMessage"] = ErrorMessage();
                        }
                    }
                }
                else
                {
                    ViewData["EditMessage"] = ErrorMessage();
                }
                List<Vehicle> model = new List<Vehicle>();
                string ruta = ConfigurationManager.AppSettings["rutalog"];
                try
                {
                model = db.Vehicle
                            .Where(o => o.id_company == this.ActiveCompanyId)
                            .ToList()
                            .Select(s => new Vehicle
                            {
                                id = s.id,
                                carRegistration = s.carRegistration,
                                mark = s.mark,
                                model = s.model,
                                isOwn = s.isOwn,
                                description = s.description,
                                isActive = s.isActive,
                                id_company = s.id_company,
                                id_userCreate = s.id_userCreate,
                                dateCreate = s.dateCreate,
                                id_userUpdate = s.id_userUpdate,
                                dateUpdate = s.dateUpdate,
                                id_VehicleType = s.id_VehicleType,
                                id_itemColor = s.id_itemColor,
                                id_personOwner = s.id_personOwner,
                                ItemColor = DataProviderItemColor.ItemColorById(s.id_itemColor),
                                VehicleType = DataProviderVehicleType.VehicleType(s.id_VehicleType),
                                id_providerT = DataProviderVehicle.VehicleProviderTransportistId(s.id),
                                rucNameProvider = DataProviderVehicle.VehicleRucProviderTransportis(s.id),
								nameProvider = DataProviderVehicle.VehicleProviderTransportis(s.id),
								id_providerTBilling = DataProviderVehicle.VehicleProviderTransportistBillingId(s.id),
                                rucNameProviderBilling = DataProviderVehicle.VehicleRucProviderTransportistBilling(s.id),
								nameProviderBilling = DataProviderVehicle.VehicleProviderTransportistBilling(s.id),
								hunterLockText = s.hunterLockText
                            }).ToList();
                }
                catch (Exception ex)
                {
                    Utilitarios.Logs.MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "vehiclePartial", "PROD");
                }

            //var model = db.Vehicle.Where(o => o.id_company == this.ActiveCompanyId);
                return PartialView("_VehiclePartial", model.ToList());
            }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileVehicle()
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                List<string> errorMessages = new List<string>();

                if (file != null)
                {
                    string filename = Server.MapPath("~/App_Data/Temp/" + file.FileName);

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    file.SaveAs(filename);

                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(filename);

                    if (workbook.Sheets.Count > 0)
                    {
                        Excel.Worksheet worksheet = workbook.ActiveSheet;
                        Excel.Range table = worksheet.UsedRange;

                        string mark = string.Empty;
                        string model = string.Empty;
                        string carRegistration = string.Empty;               
                        string description = string.Empty;

                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 2; i < table.Rows.Count; i++)
                                {
                                    Excel.Range row = table.Rows[i]; // FILA i
                                    try
                                    {
                                        mark = row.Cells[1].Text;       // COLUMNA 1
                                        model = row.Cells[2].Text;
                                        carRegistration = row.Cells[3].Text;                                      
                                        description = row.Cells[4].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    Vehicle vehicleImport = db.Vehicle.FirstOrDefault(l => l.carRegistration.Equals(carRegistration));

                                    if (vehicleImport == null)
                                    {
                                        vehicleImport = new Vehicle
                                        {
                                            mark = mark,
                                            model = model,
                                            carRegistration = carRegistration,                                    
                                            description = description,
                                            isOwn = true,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };
                                        db.Vehicle.Add(vehicleImport);
                                    }
                                    else
                                    {
                                        vehicleImport.mark = mark;
                                        vehicleImport.model = model;
                                        vehicleImport.carRegistration = carRegistration;                                  
                                        vehicleImport.description = description;

                                        vehicleImport.id_userUpdate = ActiveUser.id;
                                        vehicleImport.dateUpdate = DateTime.Now;

                                        db.Vehicle.Attach(vehicleImport);
                                        db.Entry(vehicleImport).State = EntityState.Modified;
                                    }
                                }

                                db.SaveChanges();
                                trans.Commit();
                            }
                            catch (Exception)
                            {
                                trans.Rollback();
                            }
                        }
                    }

                    application.Workbooks.Close();

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }

                    return Json(file?.FileName, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateCarRegistrationVehicle(int id_vehicle, string carRegistration)
        {
            Vehicle vechile = db.Vehicle.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.carRegistration == carRegistration
                                                                            && b.id != id_vehicle);

            if (vechile == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Matrícula en uso por otro Vehículo" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult VehicleReport()
        {
            VehicleReport report = new VehicleReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_VehicleReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VehicleDetailReport(int id)
        {
            VehicleDetailReport report = new VehicleDetailReport();
            report.Parameters["id_vehicle"].Value = id;
            return PartialView("_VehicleReport", report);
        }

        #endregion
    }
}
