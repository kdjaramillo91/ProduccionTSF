using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Utilitarios.Logs;

namespace DXPANACEASOFT.Controllers
{
	public class GroupPersonByRolController : DefaultController
	{
		// GET: GroupPersonByRol
		public ActionResult Index()
		{
			return View(GetGroupPersonByRolListDTO());
		}

		private GroupPersonByRolDTO GetGroupPersonByRolDTO()
		{
			if (!(Session["GroupPersonByRolDTO"] is GroupPersonByRolDTO groupPersonByRolDTO))
				groupPersonByRolDTO = new GroupPersonByRolDTO();
			return groupPersonByRolDTO;
		}

		private void SetGroupPersonByRolDTO(GroupPersonByRolDTO groupPersonByRolDTO)
		{
			Session["GroupPersonByRolDTO"] = groupPersonByRolDTO;
		}

		private List<GroupPersonByRolDTO> GetGroupPersonByRolListDTO()
		{
			using (var db = new DBContext())
			{
				var query = db.GroupPersonByRol.Select(p => new GroupPersonByRolDTO
				{
					id = p.id,
					name = p.name,
					rol = p.Rol.name,
					id_rol = p.id_rol,
					isActive = p.isActive
				}).ToList();

				return query;
			}
		}

		[ValidateInput(false)]
		public ActionResult GridViewGroupPersonaByRolList()
		{
			return PartialView("_GridViewGroupPersonByRol", GetGroupPersonByRolListDTO());
		}

		private void BuildGroupPersonByRolDetails(GroupPersonByRolDTO groupPersonByRolDTO)
		{
			using (var db = new DBContext())
			{
				groupPersonByRolDTO.ListGroupPersonByRolDetailDTO = new List<GroupPersonByRolDetailDTO>();
				if (groupPersonByRolDTO.id != 0)
				{
					groupPersonByRolDTO.ListGroupPersonByRolDetailDTO = db.GroupPersonByRolDetail
					.Where(d => d.id_groupPersonByRol == groupPersonByRolDTO.id)
					.Select(i => new GroupPersonByRolDetailDTO
					{
						id = i.id,
						id_groupPersonByRol = i.id_groupPersonByRol,
						id_person = i.id_person,
					}).ToList();
				}
			}
		}

		private void BuildViewData(GroupPersonByRolDTO _groupPersonByRolDTO = null, bool enabled = true)
		{
			BuildComboBoxCompany();
			BuildComboBoxRol();
			ViewBag.id_company = ActiveUser.id_company;
			ViewBag.enabled = enabled;
		}

		[ValidateInput(false)]
		public ActionResult GridViewGroupPersonByRolDetails(bool enabled)
		{
			ViewBag.enabled = enabled;
			var groupPersonByRolDTO = GetGroupPersonByRolDTO();
			return PartialView("_GridViewGroupPersonByRolDetails", groupPersonByRolDTO.ListGroupPersonByRolDetailDTO);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EditFormLayoutAddNewPartial(GroupPersonByRolDetailDTO groupPersonByRolDetail, bool enabled = true)
		{
			var groupPersonByRolDTO = GetGroupPersonByRolDTO();
			if (string.IsNullOrEmpty(groupPersonByRolDetail.id_person.ToString()))
				ViewData["EditError"] = "Valor no es valido.";
			else
			{
				groupPersonByRolDTO.ListGroupPersonByRolDetailDTO.Add(new GroupPersonByRolDetailDTO
				{
					id = groupPersonByRolDTO.ListGroupPersonByRolDetailDTO.Count() + 1,
					id_person = groupPersonByRolDetail.id_person
				});
			}
			ViewBag.enabled = enabled;
			return PartialView("_GridViewGroupPersonByRolDetails", groupPersonByRolDTO.ListGroupPersonByRolDetailDTO);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EditFormLayoutUpdatePartial(GroupPersonByRolDetailDTO groupPersonByRolDetail, bool enabled = true)
		{
			var groupPersonByRolDTO = GetGroupPersonByRolDTO();

			if (string.IsNullOrEmpty(groupPersonByRolDetail.id_person.ToString()))
				ViewData["EditError"] = "Valor no es valido.";
			else
			{
				var gpr = groupPersonByRolDTO.ListGroupPersonByRolDetailDTO.FirstOrDefault(c => c.id == groupPersonByRolDetail.id);
				if (gpr == null)
					ViewData["EditError"] = "Valor no es valido.";
				else
				{
					gpr.id_person = groupPersonByRolDetail.id_person;
				}
			}
			ViewBag.enabled = enabled;
			return PartialView("_GridViewGroupPersonByRolDetails", groupPersonByRolDTO.ListGroupPersonByRolDetailDTO);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EditFormLayoutDeletePartial(int id, bool enabled = true)
		{
			var groupPersonByRolDTO = GetGroupPersonByRolDTO();
			var detail = groupPersonByRolDTO.ListGroupPersonByRolDetailDTO.FirstOrDefault(c => c.id == id);
			if (detail != null)
			{
				groupPersonByRolDTO.ListGroupPersonByRolDetailDTO.Remove(detail);
			}
			else
			{
				ViewData["EditError"] = "Elemento no es valido.";
			}
			ViewBag.enabled = enabled;
			return PartialView("_GridViewGroupPersonByRolDetails", groupPersonByRolDTO.ListGroupPersonByRolDetailDTO);
		}

		public ActionResult DataSourceComboBoxPersonsRol(int id_company, int id_personRol)
		{
			var model = db.Person.Select(p => new
			{
				name = p.identification_number + " - " + p.fullname_businessName,
				id = p.id
			}).ToList();

			return GridViewExtension.GetComboBoxCallbackResult(p =>
			{
				p.TextField = "name";
				p.ValueField = "id";
				p.ValueType = typeof(int);
				p.BindList(model);
			});
		}

		private void BuildComboBoxCompany()
		{
			using (DBContext db = new DBContext())
			{
				var companys = db.Company.Select(s => new SelectListItem
				{
					Text = s.businessName,
					Value = s.id.ToString()
				}).ToList();

				ViewData["Companys"] = companys;
			}
		}

		public ActionResult ComboBoxCompany(bool enabled = true)
		{
			BuildComboBoxCompany();
			ViewBag.enabled = enabled;
			return PartialView("_ComboBoxCompany");
		}

		private void BuildComboBoxRol()
		{
			using (DBContext db = new DBContext())
			{
				var rols = db.Rol.Select(s => new SelectListItem
				{
					Text = s.name,
					Value = s.id.ToString()
				}).ToList();

				ViewData["Rols"] = rols;
			}
		}

		public ActionResult ComboBoxRol(bool enabled = true)
		{
			BuildComboBoxRol();
			ViewBag.enabled = enabled;
			return PartialView("_ComboBoxRol");
		}

		[HttpPost]
		public ActionResult Edit(int id, bool enabled = true)
		{
			using (var db = new DBContext())
			{
				GroupPersonByRolDTO groupPersonByRolDTO;
				var group = db.GroupPersonByRol.FirstOrDefault(p => p.id == id);
				if (group == null)
				{
					groupPersonByRolDTO = new GroupPersonByRolDTO
					{
						id = 0,
						isActive = true,
						id_company = null,
						id_rol = null
					};
				}
				else
				{
					groupPersonByRolDTO = new GroupPersonByRolDTO
					{
						id = group.id,
						isActive = group.isActive,
						id_company = group.id_company,
						id_rol = group.id_rol,
						description = group.description,
						name = group.name,
						rol = group.Rol.name,
						company = "",
					};
				}

				BuildGroupPersonByRolDetails(groupPersonByRolDTO);
				BuildViewData(groupPersonByRolDTO, enabled);

				SetGroupPersonByRolDTO(groupPersonByRolDTO);

				return PartialView(groupPersonByRolDTO);
			}
		}

		private bool ExistInOtherGroup(int id_group, int id_person, int id_personRol)
		{
			using (var db = new DBContext())
			{
				return db.GroupPersonByRolDetail.Where(d => d.id_groupPersonByRol != id_group)
						 .FirstOrDefault(d => d.GroupPersonByRol.id_rol == id_personRol &&
											  d.id_person == id_person) != null;
			}
		}

		private bool ExistInThisGroup(int id_person, ICollection<GroupPersonByRolDetail> details)
		{
			return (details.FirstOrDefault(d => d.id_person == id_person) != null);
		}

		[HttpPost]
		public JsonResult Save(string json)
		{
			var result = new ApiResult();
			bool isSaved = false;
			using (var db = new DBContext())
			{
				using (var trans = db.Database.BeginTransaction())
				{
					try
					{
						JToken token = JsonConvert.DeserializeObject<JToken>(json);
						var newObject = false;
						var id = token.Value<int>("id");

						var group = db.GroupPersonByRol.FirstOrDefault(p => p.id == id);
						if (group == null)
						{
							newObject = true;
							group = new GroupPersonByRol
							{
								id_userCreate = ActiveUser.id,
								dateCreate = DateTime.Now
							};
						}

						group.name = token.Value<string>("name");
						group.description = token.Value<string>("description");
						group.id_rol = token.Value<int>("id_rol");
						group.isActive = token.Value<bool>("isActive");
						group.id_company = ActiveUser.id_company;
						group.id_userUpdate = ActiveUser.id;
						group.dateUpdate = DateTime.Now;

						//Detalle
						var details = db.GroupPersonByRolDetail.Where(i => i.id_groupPersonByRol == group.id);
						foreach (var d in details)
						{
							db.GroupPersonByRolDetail.Remove(d);
							db.GroupPersonByRolDetail.Attach(d);
							db.Entry(d).State = EntityState.Deleted;
						}

						var groupPersonByRolDTO = GetGroupPersonByRolDTO();
						foreach (var detail in groupPersonByRolDTO.ListGroupPersonByRolDetailDTO)
						{
							var persona = db.Person.FirstOrDefault(p => p.id == detail.id_person);

							if (ExistInThisGroup(detail.id_person, group.GroupPersonByRolDetail))
							{
								throw new Exception("La Persona: " + persona.identification_number + " - " + persona.fullname_businessName + " Se encuentra repetida");
							}

							if (ExistInOtherGroup(group.id, detail.id_person, group.id_rol))
							{
								throw new Exception("La Persona: " + persona.identification_number + " - " + persona.fullname_businessName + " Ya existe en otro grupo con el mismo tipo de Rol");
							}

							if (persona.Rol.FirstOrDefault(p => p.id == group.id_rol) == null)
							{
								throw new Exception("La Persona: " + persona.identification_number + " - " + persona.fullname_businessName + " No cumple con el rol necesario para estar en este grupo");
							}

							group.GroupPersonByRolDetail.Add(new GroupPersonByRolDetail
							{
								id_person = detail.id_person
							});
						}

						if (newObject)
						{
							db.GroupPersonByRol.Add(group);
							db.Entry(group).State = EntityState.Added;
						}
						else
						{
							db.GroupPersonByRol.Attach(group);
							db.Entry(group).State = EntityState.Modified;
						}

						db.SaveChanges();
						trans.Commit();
						isSaved = true;
						result.Data = group.id.ToString();
					}
					catch (Exception e)
					{
						result.Code = e.HResult;
						result.Message = e.Message;
						trans.Rollback();
					}
				}
			}
			string ruta = ConfigurationManager.AppSettings["rutaLog"];
			string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];
			#region Replicate Group to Production
			try
			{
				if (isSaved)
				{
					#region Replicate Group to Production
					if (isSaved)
					{
						#region 
						var startInfo = new ProcessStartInfo()
						{
							FileName = _pathProgramReplication,
							Arguments = result.Data + " RLPGP ReplicateInformation",
							UseShellExecute = false,
							CreateNoWindow = true,
						};

						Process.Start(startInfo);
						#endregion
					}
					#endregion
				}
			}
			catch (Exception ex)
			{
				MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "GroupUpdateReplication", "PROD");
			}
			#endregion
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult PrintGroupReportList()
		{
			#region Armo Parametros

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "ADLP";
			_repMod.conex = objConex;

			rep = GetTmpDataName(20);


			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

			#endregion
		}
	}
}