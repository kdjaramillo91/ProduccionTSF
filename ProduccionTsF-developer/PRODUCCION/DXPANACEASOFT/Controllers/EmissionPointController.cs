using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class EmissionPointController : DefaultController
	{
		[HttpPost]
		public ActionResult Index()
		{
			return PartialView();
		}

		#region EmissionPoint GRIDVIEW

		[ValidateInput(false)]
		public ActionResult EmissionPointsPartial(int? keyToCopy)
		{
			if (keyToCopy != null)
			{
				ViewData["rowToCopy"] = db.EmissionPoint.FirstOrDefault(b => b.id == keyToCopy);
			}
			var model = db.EmissionPoint.Where(e => e.id_company == this.ActiveCompanyId);
			return PartialView("_EmissionPointsPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EmissionPointsPartialAddNew(EmissionPoint item)
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


						db.EmissionPoint.Add(item);
						db.SaveChanges();
						trans.Commit();

						ViewData["EditMessage"] = SuccessMessage("Punto de Emisión: " + item.name + " guardado exitosamente");
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

			var model = db.EmissionPoint.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_EmissionPointsPartial", model.ToList());

		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EmissionPointsPartialUpdate(EmissionPoint item)
		{
			if (ModelState.IsValid)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var modelItem = db.EmissionPoint.FirstOrDefault(it => it.id == item.id);
						if (modelItem != null)
						{
							modelItem.id_branchOffice = item.id_branchOffice;
							modelItem.id_division = item.id_division;

							modelItem.name = item.name;
							modelItem.code = item.code;

							modelItem.email = item.email;
							modelItem.address = item.address;
							modelItem.phoneNumber = item.phoneNumber;
							modelItem.id_documentType = item.id_documentType;
							modelItem.secuenciaInicio = item.secuenciaInicio;
							modelItem.secuenciaFinal = item.secuenciaFinal;
							modelItem.secuenciaValor = item.secuenciaValor;

							modelItem.description = item.description;
							modelItem.isActive = item.isActive;

							modelItem.id_userUpdate = ActiveUser.id;
							modelItem.dateUpdate = DateTime.Now;

							db.EmissionPoint.Attach(modelItem);
							db.Entry(modelItem).State = EntityState.Modified;

							db.SaveChanges();
							trans.Commit();

							ViewData["EditMessage"] =
								SuccessMessage("Punto de Emisión: " + item.name + " guardado exitosamente");
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

			var model = db.EmissionPoint.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_EmissionPointsPartial", model.ToList());

		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EmissionPointsPartialDelete(System.Int32 id)
		{
			if (id >= 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var item = db.EmissionPoint.FirstOrDefault(it => it.id == id);
						if (item != null)
						{
							item.isActive = false;
							item.id_userUpdate = ActiveUser.id;
							item.dateUpdate = DateTime.Now;

						}
						// db.EmissionPoint.Remove(item);
						db.Entry(item).State = EntityState.Modified;
						item.id_documentType = item.id_documentType;
						db.SaveChanges();
						trans.Commit();
						ViewData["EditMessage"] = SuccessMessage("Punto de Emisión: " + (item?.name ?? "") + " desactivado exitosamente");
					}
					catch //(Exception ex)
					{
						ViewData["EditMessage"] = ErrorMessage();
						trans.Rollback();
					}
				}

			}

			var model = db.EmissionPoint.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_EmissionPointsPartial", model.ToList());
		}

		[HttpPost]
		public ActionResult DeleteSelectedEmissionPoints(int[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						var modelItem = db.EmissionPoint.Where(i => ids.Contains(i.id));
						foreach (var item in modelItem)
						{
							item.isActive = false;

							item.id_userUpdate = ActiveUser.id;
							item.dateUpdate = DateTime.Now;

							db.EmissionPoint.Attach(item);
							db.Entry(item).State = EntityState.Modified;
						}
						db.SaveChanges();
						trans.Commit();

						ViewData["EditMessage"] = SuccessMessage("Puntos de Emisiones desactivados exitosamente");
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

			var model = db.EmissionPoint.Where(o => o.id_company == this.ActiveCompanyId);
			return PartialView("_EmissionPointsPartial", model.ToList());
		}

		#endregion

		#region AUXILIAR FUNCTIONS

		//[HttpPost]
		//public JsonResult DivisionByCompany(int id_company)
		//{
		//    var model = db.Division.Where(d => d.id_company == id_company && d.isActive).ToList();

		//    var result = model.Select(d => new { d.id, d.name });

		//    return Json(result, JsonRequestBehavior.AllowGet);
		//}

		[HttpPost]
		public JsonResult BranchOfficeByDivision(int id_division)
		{
			var model = db.BranchOffice.Where(bo => bo.id_division == id_division && bo.isActive).ToList();

			var result = model.Select(d => new { d.id, d.name });

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult BranchOfficeData(int id_branchOffice)
		{
			var model = db.BranchOffice.FirstOrDefault(bo => bo.id == id_branchOffice);

			if (model == null)
			{
				return Json(null, JsonRequestBehavior.AllowGet);
			}

			var result = new
			{
				model.address,
				model.phoneNumber,
				model.email
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult ValidateCodeEmissionPoint(int id_emissionPoint, int code, int id_documentType)
		{
			EmissionPoint emissionPoint = db.EmissionPoint.FirstOrDefault(b =>
																			 b.code == code
																			&& b.id != id_emissionPoint
																			&& b.id_documentType == id_documentType);

			if (emissionPoint == null)
			{
				return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
			}

			return Json(new { isValid = false, errorText = "Código en uso por otro Puntos de Emisión" }, JsonRequestBehavior.AllowGet);
		}






		[HttpPost]
		public JsonResult ImportFileEmissionPoint()
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

						int code = 0;
						string name = string.Empty;
						int id_branchOffice = 0;
						int id_division = 0;
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
										code = int.Parse(row.Cells[1].Text);        // COLUMNA 1
										name = row.Cells[2].Text;
										id_branchOffice = int.Parse(row.Cells[3].Text);
										id_division = int.Parse(row.Cells[3].Text);
										description = row.Cells[5].Text;
									}
									catch (Exception)
									{
										errorMessages.Add($"Error en formato de datos fila: {i}.");
									}

									EmissionPoint emissionPointImport = db.EmissionPoint.FirstOrDefault(l => l.code == code);

									if (emissionPointImport == null)
									{
										emissionPointImport = new EmissionPoint
										{
											code = code,
											id_branchOffice = id_branchOffice,
											id_division = id_division,

											name = name,
											description = description,
											isActive = true,

											id_company = this.ActiveCompanyId,
											id_userCreate = ActiveUser.id,
											dateCreate = DateTime.Now,
											id_userUpdate = ActiveUser.id,
											dateUpdate = DateTime.Now
										};

										db.EmissionPoint.Add(emissionPointImport);
									}
									else
									{
										emissionPointImport.code = code;
										emissionPointImport.name = name;
										emissionPointImport.id_branchOffice = id_branchOffice;
										emissionPointImport.id_division = id_division;
										emissionPointImport.description = description;

										emissionPointImport.id_userUpdate = ActiveUser.id;
										emissionPointImport.dateUpdate = DateTime.Now;

										db.EmissionPoint.Attach(emissionPointImport);
										db.Entry(emissionPointImport).State = EntityState.Modified;
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

		#region REPORTS


		[HttpPost]
		public ActionResult EmissionPointReport()
		{
			EmissionPointReport report = new EmissionPointReport();
			report.Parameters["id_company"].Value = this.ActiveCompanyId;
			return PartialView("_EmissionPointReport", report);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EmissionPointDetailReport(int id)
		{
			EmissionPointDetailReport report = new EmissionPointDetailReport();
			report.Parameters["id_emissionPoint"].Value = id;
			return PartialView("_EmissionPointReport", report);
		}

		#endregion
	}
}



