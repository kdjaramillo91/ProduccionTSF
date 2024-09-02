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
    public class RecordSecurityController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region RecordSecurity GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult RecordSecurityPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.tbsysUserRecordSecurity.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.tbsysUserRecordSecurity
				.Where(e => e.User.isActive)
				.OrderByDescending(e => e.id).ToList();
            return PartialView("_RecordSecurityPartial", model);
        } 

        [HttpPost, ValidateInput(false)]
        public ActionResult RecordSecurityPartialAddNew(tbsysUserRecordSecurity item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
						if(ValidateRegisterExits(item.id_user, item.id_SecurityRecord, false, false))
						{
							throw new Exception("Registro de seguridad ya asociado a este usuario.");
						}

						db.tbsysUserRecordSecurity.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Registro de seguridad de usuario guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                    }
                }

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.tbsysUserRecordSecurity
				.Where(e => e.User.isActive)
				.OrderByDescending(e => e.id).ToList();
			return PartialView("_RecordSecurityPartial", model);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult RecordSecurityPartialUpdate(tbsysUserRecordSecurity item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.tbsysUserRecordSecurity.FirstOrDefault(it => it.id == item.id);

						if(modelItem.id_user != item.id_user || modelItem.id_SecurityRecord != item.id_SecurityRecord || modelItem.isActive != item.isActive)
						{
							if (ValidateRegisterExits(item.id_user, item.id_SecurityRecord, true, item.isActive))
							{
								throw new Exception("Registro de seguridad ya asociado a este usuario.");
							}
						}

						if (modelItem != null)
                        {

							modelItem.id_SecurityRecord = item.id_SecurityRecord;
							modelItem.id_user = item.id_user;
							modelItem.isActive = item.isActive;
 
                            db.tbsysUserRecordSecurity.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Registro de seguridad de usuario guardado exitosamente");
                        }
                    }
					catch (Exception e)
					{
						trans.Rollback();
						ViewData["EditMessage"] = ErrorMessage(e.Message);
					}
				}
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.tbsysUserRecordSecurity
				.Where(e => e.User.isActive)
				.OrderByDescending(e => e.id).ToList();
			return PartialView("_RecordSecurityPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RecordSecurityPartialDelete(int id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.tbsysUserRecordSecurity.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            db.tbsysUserRecordSecurity.Remove(item);
                            //db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();


                            ViewData["EditMessage"] = SuccessMessage("Registro de seguridad de usuario eliminado exitosamente");
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

            var model = db.tbsysUserRecordSecurity
				.Where(e => e.User.isActive)
				.OrderByDescending(e => e.id).ToList();
			return PartialView("_RecordSecurityPartial", model);
        }

        [HttpPost]
        public ActionResult DeleteSelectedRecordSecurity(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.tbsysUserRecordSecurity.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            db.tbsysUserRecordSecurity.Remove(item);
                            //db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Registros de seguridad de usuarios desactivados exitosamente");
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

			var model = db.tbsysUserRecordSecurity
				.Where(e => e.User.isActive)
				.OrderByDescending(e => e.id).ToList();

			return PartialView("_RecordSecurityPartial", model);
        }
		#endregion

		#region Funciones Auxiliares
		[HttpPost]
		public JsonResult ImportFileWarehouses()
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

						int id_user = 0;
						int id_SecurityRecord = 0;
						bool isActive = true;


						using (DbContextTransaction trans = db.Database.BeginTransaction())
						{
							try
							{
								for (int i = 2; i < table.Rows.Count; i++)
								{
									Excel.Range row = table.Rows[i]; // FILA i
									try
									{
										id_user = int.Parse(row.Cells[1].Text);        // COLUMNA 1
										id_SecurityRecord = int.Parse(row.Cells[2].Text);
										isActive = bool.Parse(row.Cells[5].Text);

									}
									catch (Exception)
									{
										errorMessages.Add($"Error en formato de datos fila: {i}.");
									}

									tbsysUserRecordSecurity warehouseImport = db.tbsysUserRecordSecurity.FirstOrDefault();

									if (warehouseImport == null)
									{
										warehouseImport = new tbsysUserRecordSecurity
										{
											id_user = id_user,
											id_SecurityRecord = id_SecurityRecord,
											isActive = true
										};

										db.tbsysUserRecordSecurity.Add(warehouseImport);
									}
									else
									{
										warehouseImport.id_user = id_user;
										warehouseImport.id_SecurityRecord = id_SecurityRecord;
										warehouseImport.isActive = isActive;
										

										db.tbsysUserRecordSecurity.Attach(warehouseImport);
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

		[HttpPost]
		public bool ValidateRegisterExits(int? id_user, int? id_SecurityRecord, bool update, bool isActive)
		{
			bool registro = false;

			if (update)
			{
				registro = db.tbsysUserRecordSecurity.
					Where(b => b.id_user == id_user && b.id_SecurityRecord == id_SecurityRecord && isActive).Any();
			}
			else
			{
				registro = db.tbsysUserRecordSecurity.
					Where(b => b.id_user == id_user && b.id_SecurityRecord == id_SecurityRecord).Any();
			}
			

			return registro;

		}
	}
}



