using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;


namespace DXPANACEASOFT.Controllers
{
 
    public class ProductionCartController : DefaultController
    {
        
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ProductionCart GRIDVIEW

        [ValidateInput(false)]
        public ActionResult ProductionCartPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ProductionCart.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ProductionCart.ToList();
            return PartialView("_ProductionCartPartial", model.ToList());
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionCartAddNew(DXPANACEASOFT.Models.ProductionCart item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                     
                        db.ProductionCart.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Carro de Producción: " + item.name + " guardado exitosamente");
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

            var model = db.ProductionCart.Where(o => o.isActive);
            return PartialView("_ProductionCartPartial", model.ToList());

        }

        public ActionResult ProductionCartUpdate(DXPANACEASOFT.Models.ProductionCart item)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ProductionCart.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                         

                            modelItem.isActive = item.isActive;
                      


                            db.ProductionCart.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage("Carro de Producción: " + item.name + " guardado exitosamente");
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

            var model = db.ProductionCart.Where(o => o.isActive);
            return PartialView("_ProductionCartPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ProductionCartDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ProductionCart.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                          
                        }
                        db.ProductionCart.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Carro de Producción: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.ProductionCart.Where(o => o.isActive);
            return PartialView("_ProductionCartPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedProductionCart(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var car = db.ProductionCart.Where(i => ids.Contains(i.id));
                        foreach (var country in car)
                        {
                            country.isActive = false;

                            

                            db.ProductionCart.Attach(country);
                            db.Entry(country).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Carros de Producción desactivados exitosamente");
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

            var model = db.ProductionCart.Where(o => o.isActive);
            return PartialView("_ProductionCartPartial", model.ToList());
        }

        [HttpPost]
        public JsonResult ImportFileProductionCart()
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
                                        description = row.Cells[3].Text;
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ProductionCart countryImport = db.ProductionCart.FirstOrDefault(l => l.code.Equals(code));

                                    if (countryImport == null)
                                    {
                                        countryImport = new ProductionCart
                                        {
                                            code = code,
                                            name = name,
                               
                                            isActive = true,

                                           
                                        };

                                        db.ProductionCart.Add(countryImport);
                                    }
                                    else
                                    {
                                        countryImport.code = code;
                                        countryImport.name = name;
                                        
                                        db.ProductionCart.Attach(countryImport);
                                        db.Entry(countryImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeProductionCart(int id_ProductionCart, string code)
        {
            ProductionCart country = db.ProductionCart.FirstOrDefault(b => b.code == code
                                                                            && b.id != id_ProductionCart);

            if (country == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Carro" }, JsonRequestBehavior.AllowGet);
        }
    }
}