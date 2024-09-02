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
    public class ItemGroupController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region ItemGroup GRIDVIEW
        [ValidateInput(false)]
        public ActionResult ItemGroupsPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.ItemGroup.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.ItemGroup.Where(ig => ig.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemGroupsPartialAddNew(ItemGroup item)
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

                        db.ItemGroup.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage(item.name + "  " + "Guardado Exitosamente");
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

            var model = db.ItemGroup.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupsPartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult ItemGroupsPartialUpdate(ItemGroup item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemGroup.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.id_parentGroup = item.id_parentGroup;
                            modelItem.name = item.name;
                            modelItem.description = item.description;

                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.ItemGroup.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] = SuccessMessage(item.name +"  "+ "Guardado Exitosamente");
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

            var model = db.ItemGroup.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemGroupsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ItemGroup.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemGroup.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] =  SuccessMessage((item?.name ?? "") + " desactivada exitosamente");
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

            var model = db.ItemGroup.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupsPartial", model.ToList());
        }



        [HttpPost]
        public ActionResult DeleteSelectedItemGroups(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.ItemGroup.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.ItemGroup.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Desactivadas exitosamente");
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

            var model = db.ItemGroup.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_ItemGroupsPartial", model.ToList());
        }

        #endregion

        #region MASTER DETAILS VIEW
        [HttpPost]
        public ActionResult ItemGroupDetailItemSubGroupsPartial(int? id_itemGroup)
        {
            ItemGroup itemGroup = db.ItemGroup.FirstOrDefault(d => d.id == id_itemGroup);
            var model = itemGroup?.ItemGroup1?.ToList() ?? new List<ItemGroup>();

            return PartialView("_ItemGroupDetailItemSubGroupsPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult ItemGroupDetailItemGroupCategoriesPartial(int? id_itemGroup)
        {
            ItemGroup itemGroup = db.ItemGroup.FirstOrDefault(d => d.id == id_itemGroup);
            var model = db.ItemGroupCategory.Where(w=> w.ItemGroupItemGroupCategory.Any(a=> a.id_itemGroup == id_itemGroup)).ToList() ?? new List<ItemGroupCategory>();
            //var model = itemGroup?.ItemGroupCategory?.ToList() ?? new List<ItemGroupCategory>();

            return PartialView("_ItemGroupDetailItemGroupCategoriesPartial", model);//.ToList());
        }

        #endregion

        #region REPORTS

        [HttpPost]
        public ActionResult ItemGroupReport()
        {
            ItemGroupReport report = new ItemGroupReport();
            report.Parameters["id_company"].Value = this.ActiveCompanyId;
            return PartialView("_ItemGroupReport", report);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemGroupDetailReport(int id)
        {
            ItemGroupDetailReport report = new ItemGroupDetailReport();
            report.Parameters["id_itemGroup"].Value = id;
            return PartialView("_ItemGroupReport", report);
        }


        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ItemGroupByCompany(int id_company, int? id_itemGroup)
        {
            var model = db.ItemGroup.Where(d => d.id_company == id_company && d.isActive && d.id != id_itemGroup).ToList();

            var result = model.Select(d => new { d.id, d.name });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

       

       [HttpPost]
        public JsonResult ValidateCodeItemGroup(int id_itemGroup, string code)
        {
            ItemGroup itemGroup = db.ItemGroup.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_itemGroup);

            if (itemGroup == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Grupo o Subgrupo de Productos " }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportFileItemGroup()
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
                        int id_parentGroup = 0;
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
                                        id_parentGroup = int.Parse(row.Cells[3].Text);
                                        description = row.Cells[4].Text;
                                        
                                 
                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    ItemGroup itemGroup = db.ItemGroup.FirstOrDefault(l => l.code.Equals(code));

                                    if (itemGroup == null)
                                    {
                                        itemGroup = new ItemGroup
                                        {
                                            code = code,
                                            name = name,
                                            id_parentGroup = id_parentGroup,
                                            description = description,

                                            isActive = true,

                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };

                                        db.ItemGroup.Add(itemGroup);
                                    }
                                    else
                                    {
                                        itemGroup.code = code;
                                        itemGroup.name = name;
                                        itemGroup.id_parentGroup = id_parentGroup;
                                        itemGroup.description = description;

                                        itemGroup.id_userUpdate = ActiveUser.id;
                                        itemGroup.dateUpdate = DateTime.Now;

                                        db.ItemGroup.Attach(itemGroup);
                                        db.Entry(itemGroup).State = EntityState.Modified;
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

