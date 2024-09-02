using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class VehicleTypeController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region VehicleType GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult VehicleTypePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.VehicleType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.VehicleType.Where(whl => whl.id_company == this.ActiveCompanyId);
            return PartialView("_VehicleTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VehicleTypePartialAddNew(VehicleType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.VehicleType.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipo de Vehículo: " + item.name + " guardado exitosamente");
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

            var model = db.VehicleType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_VehicleTypePartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult VehicleTypePartialUpdate(VehicleType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.VehicleType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.axisNumber = item.axisNumber;
                            modelItem.codeInen = item.codeInen;
                            modelItem.subCodeInen = item.subCodeInen;
                            modelItem.high = item.high;
                            modelItem.width = item.width;
                            modelItem.@long = item.@long;
                            modelItem.id_metricUnitMaxLoad = item.id_metricUnitMaxLoad;
                            modelItem.maxLoad = item.maxLoad;
                            modelItem.id_metricUnitmaxCubicCapacity = item.id_metricUnitmaxCubicCapacity;
                            modelItem.maxCubicCapacity = item.maxCubicCapacity;
                            modelItem.id_shippingType = item.id_shippingType;
                            modelItem.isActive = item.isActive;

                            modelItem.id_company = this.ActiveCompanyId;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.VehicleType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Tipo de Vehículo: " + item.name + " guardado exitosamente");
                        }
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

            var model = db.VehicleType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_VehicleTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VehicleTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.VehicleType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.VehicleType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();


                            ViewData["EditMessage"] = SuccessMessage("Tipo de Vehículo: " + (item?.name ?? "") + " desactivado exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.VehicleType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_VehicleTypePartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedVehicleType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.VehicleType.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.VehicleType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tipos de Vehículos desactivados exitosamente");
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

            var model = db.VehicleType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_VehicleTypePartial", model.ToList());
        }
        #endregion

        #region REPORTS

        #endregion

        #region AUXILIAR FUNCTIONS
        [HttpPost]
        public JsonResult ValidateCodeVehicleType(int id_VehicleType, string code)
        {
            VehicleType warehouse = db.VehicleType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.name == code
                                                                            && b.id != id_VehicleType);

            if (warehouse == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tipo de Vehículo" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileVehicleType()
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

                        
                        string name = string.Empty;                
                        string description = string.Empty;
                        int axisNumber = 0;
                        string codeInen = string.Empty;
                        string subCodeInen = string.Empty;
                        int high = 0;
                        int width = 0;
                        int Long = 0;
                        int id_metricUnitMaxLoad = 0;
                        int maxLoad = 0;
                        int id_metricUnitmaxCubicCapacity = 0;
                        int maxCubicCapacity = 0;
                        int id_shippingType = 0;

                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 2; i < table.Rows.Count; i++)
                                {
                                    Excel.Range row = table.Rows[i]; // FILA i
                                    try
                                    {
                                        name = row.Cells[1].Text;        // COLUMNA 1
                                        description = row.Cells[2].Text;
                                        axisNumber = int.Parse(row.Cells[3].Text);
                                        codeInen = row.Cells[4].Text;
                                        subCodeInen = row.Cells[5].Text;
                                        high = int.Parse(row.Cells[6].Text);
                                        width = int.Parse(row.Cells[7].Text);
                                        Long = int.Parse(row.Cells[8].Text);
                                        id_metricUnitMaxLoad = int.Parse(row.Cells[9].Text);
                                        maxLoad = int.Parse(row.Cells[10].Text);
                                        id_metricUnitmaxCubicCapacity = int.Parse(row.Cells[10].Text);
                                        maxCubicCapacity = int.Parse(row.Cells[10].Text);
                                        id_shippingType = int.Parse(row.Cells[10].Text);
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    VehicleType warehouseImport = db.VehicleType.FirstOrDefault(l => l.name.Equals(name));

                                    if (warehouseImport == null)
                                    {
                                        warehouseImport = new VehicleType
                                        {
                                            name = name,       // COLUMNA 1
                                            description = description,
                                            axisNumber = axisNumber,
                                            codeInen = codeInen,
                                            subCodeInen = subCodeInen,
                                            high = high,
                                            width = width,
                                            @long = Long,
                                            id_metricUnitMaxLoad = id_metricUnitMaxLoad,
                                            maxLoad = maxLoad,
                                            id_metricUnitmaxCubicCapacity = id_metricUnitmaxCubicCapacity,
                                            maxCubicCapacity = maxCubicCapacity,
                                            id_shippingType = id_shippingType,
                                            
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.VehicleType.Add(warehouseImport);
                                    }
                                    else
                                    {
                                        warehouseImport.name = name;
                                        warehouseImport.description = description;
                                        warehouseImport.axisNumber = axisNumber;
                                        warehouseImport.codeInen = codeInen;
                                        warehouseImport.subCodeInen = subCodeInen;
                                        warehouseImport.high = high;
                                        warehouseImport.width = width;
                                        warehouseImport.@long = Long;
                                        warehouseImport.id_metricUnitMaxLoad = id_metricUnitMaxLoad;
                                        warehouseImport.maxLoad = maxLoad;
                                        warehouseImport.id_metricUnitmaxCubicCapacity = id_metricUnitmaxCubicCapacity;
                                        warehouseImport.maxCubicCapacity = maxCubicCapacity;
                                        warehouseImport.id_shippingType = id_shippingType;

                                        warehouseImport.id_userUpdate = ActiveUser.id;
                                        warehouseImport.dateUpdate = DateTime.Now;

                                        db.VehicleType.Attach(warehouseImport);
                                        db.Entry(warehouseImport).State = EntityState.Modified;
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
        #endregion
    }
}



