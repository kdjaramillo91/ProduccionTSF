using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class DataTypeController : DefaultController
    {
       [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }


        #region DataType GridView

        [ValidateInput(false)]
        public ActionResult DataTypesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.DataType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.DataType.Where(d => d.id_company == this.ActiveCompanyId);
            return PartialView("_DataTypesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DataTypesPartialAddNew(DataType item)
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

                        db.DataType.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tipo de Dato: " + item.name + " guardado exitosamente");
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

            var model = db.DataType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DataTypesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DataTypesPartialUpdate(DataType item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.DataType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.purchase_decimalPlace = item.purchase_decimalPlace;
                            modelItem.sale_decimalPlace = item.sale_decimalPlace;
                            modelItem.inventory_decimalPlace = item.inventory_decimalPlace;

                            modelItem.name = item.name;
                            modelItem.description = item.description;

                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.DataType.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Tipo de Dato: " + item.name + " guardado exitosamente");
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

            var model = db.DataType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DataTypesPartial", model.ToList());

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult DataTypesPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {

                        var item = db.DataType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                        db.DataType.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tipo de Dato: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.DataType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DataTypesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedDataTypes(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.DataType.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.DataType.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipo de Datos desactivados exitosamente");
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

            var model = db.DataType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DataTypesPartial", model.ToList());
        }



        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileDataType()
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

                        string code = string.Empty;
                        string name = string.Empty;
                        int purchase_decimalPlace = 0;
                        int sale_decimalPlace = 0;
                        int inventory_decimalPlace = 0;
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
                                        code = row.Cells[1].Text;        // COLUMNA 1
                                        name = row.Cells[2].Text;
                                        purchase_decimalPlace = int.Parse(row.Cells[3].Text);
                                        sale_decimalPlace = int.Parse(row.Cells[3].Text);
                                        inventory_decimalPlace = int.Parse(row.Cells[3].Text);
                                        description = row.Cells[6].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    DataType dataTypeImport = db.DataType.FirstOrDefault(l => l.code.Equals(code));

                                    if (dataTypeImport == null)
                                    {
                                        dataTypeImport = new DataType
                                        {
                                            code = code,
                                            name = name,
                                            purchase_decimalPlace = purchase_decimalPlace,
                                            sale_decimalPlace = sale_decimalPlace,
                                            inventory_decimalPlace = inventory_decimalPlace,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.DataType.Add(dataTypeImport);
                                    }
                                    else
                                    {
                                        dataTypeImport.code = code;
                                        dataTypeImport.name = name;
                                        dataTypeImport.purchase_decimalPlace = purchase_decimalPlace;
                                        dataTypeImport.sale_decimalPlace = sale_decimalPlace;
                                        dataTypeImport.inventory_decimalPlace = inventory_decimalPlace;
                                        dataTypeImport.description = description;

                                        dataTypeImport.id_userUpdate = ActiveUser.id;
                                        dataTypeImport.dateUpdate = DateTime.Now;

                                        db.DataType.Attach(dataTypeImport);
                                        db.Entry(dataTypeImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeDataType(int id_dataType, string code)
        {
            DataType dataType = db.DataType.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_dataType);

            if (dataType == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tipo de Dato" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region REPORTS

        [HttpPost]
        public ActionResult DataTypeReport()
        {
            DataTypeReport report = new DataTypeReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_DataTypeReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DataTypeDetailReport(int id)
        {
            DataTypeDetailReport report = new DataTypeDetailReport();
            report.Parameters["id_dataType"].Value = id;
            return PartialView("_DataTypeReport", report);
        }

        #endregion
    }
}



