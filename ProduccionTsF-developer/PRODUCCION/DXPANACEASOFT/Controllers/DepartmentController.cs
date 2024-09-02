using DevExpress.Web.Mvc;
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
    [Authorize]
    public class DepartmentController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }


        #region DEPARTMENT GRIDVIEW

        [ValidateInput(false)]
        public ActionResult DepartmentsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.Department.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.Department.Where(d => d.id_company == this.ActiveCompanyId);
            return PartialView("_DepartmentPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentsPartialAddNew(Department item)
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

                        db.Department.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Departamento: " + item.name + " guardado exitosamente");
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

            var model = db.Department.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DepartmentPartial", model.ToList());

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentsPartialUpdate(Department item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Department.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.Department.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] =
                                SuccessMessage("Departamento: " + item.name + " guardado exitosamente");
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

            var model = db.Department.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DepartmentPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentsPartialDelete(System.Int32 id)   
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Department.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                        db.Department.Remove(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Departamento: " + (item?.name ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.Department.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DepartmentPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedDepartments(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Department.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.Department.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Departamentos desactivados exitosamente");
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

            var model = db.Department.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_DepartmentPartial", model.ToList());
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileDepartment()
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

                                    Department departmentImport = db.Department.FirstOrDefault(l => l.code.Equals(code));

                                    if (departmentImport == null)
                                    {
                                        departmentImport = new Department
                                        {
                                            code = code,
                                            name = name,
                                            description = description,
                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.Department.Add(departmentImport);
                                    }
                                    else
                                    {
                                        departmentImport.code = code;
                                        departmentImport.name = name;
                                        departmentImport.description = description;

                                        departmentImport.id_userUpdate = ActiveUser.id;
                                        departmentImport.dateUpdate = DateTime.Now;

                                        db.Department.Attach(departmentImport);
                                        db.Entry(departmentImport).State = EntityState.Modified;
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
        public JsonResult ValidateCodeDepartment(int id_department, string code)
        {
            Department department = db.Department.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_department);

            if (department == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Departamento" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region REPORT

        [HttpPost]
        public ActionResult DepartmentReport()
        {
            DepartmentReport report = new DepartmentReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_DepartmentReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentDetailReport(int id)
        {
            DepartmentDetailReport report = new DepartmentDetailReport();
            report.Parameters["id_department"].Value = id;
            return PartialView("_DepartmentReport", report);
        }

        #endregion
    }
}


    
