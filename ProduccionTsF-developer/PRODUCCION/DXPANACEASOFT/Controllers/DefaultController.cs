using DevExpress.Web.ASPxHtmlEditor.Internal;
using DocumentFormat.OpenXml.Presentation;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models.General;
using DXPANACEASOFT.Models.Helpers;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Utilitarios.General;
using Utilitarios.Logs;

namespace DXPANACEASOFT.Controllers
{
	public class DefaultController : Controller
	{
		#region ATTRIBUTES FOR

		protected string _codeStateDocument { get; set; }

		protected string _messageAnswer { get; set; }

		protected string _nameStateDocument { get; set; }

		protected string _strNumberDocAnswer { get; set; }

		protected ButtonsOnEditForm _btnOnEditForm { get; set; }

		protected AnswerForAction _Answerfa { get; set; }

		public DBContext db { get; } = new DBContext();

		public DefaultController()
		{
			this.db.Database.CommandTimeout = 90;
            //ViewData["ID"] = Guid.NewGuid().ToString();
            //ViewData["ID"] = DateTime.Now.ToFileTimeUtc().ToString();
        }

		#endregion

		private User m_activeUser;
		public int ActiveUserId
		{
			get
			{
				if (this.m_activeUser == null)
				{
					return ((LogedUser)User).User?.id ?? 0;
				}
				else
				{
					return this.m_activeUser.id;
				}
			}
		}
		public User ActiveUser
		{
			get
			{
				if (this.m_activeUser == null)
				{
					if (User is LogedUser)
					{
						var appUser = ((LogedUser)User).User;
						this.m_activeUser = db.User.FirstOrDefault(u => u.id == appUser.id);
					}
				}
				return this.m_activeUser;
			}
		}

		protected void SetCommonViewBagData()
		{
			var user = this.ActiveUser;

			if (user == null)
			{
				throw new Exception("Usuario no encontrado en el sistema");
			}

			var puedeAdicionar = this.GetUserPermission(user, "Adicionar");
			var puedeAprobar = this.GetUserPermission(user, "Aprobar");
			var puedeAutorizar = this.GetUserPermission(user, "Autorizar");
			var puedeAnular = this.GetUserPermission(user, "Anular");
			var puedeCerrar = this.GetUserPermission(user, "Cerrar");
			var puedeCopiar = this.GetUserPermission(user, "Copiar");
			var puedeReversar = this.GetUserPermission(user, "Reversar");
			var puedeImprimir = this.GetUserPermission(user, "Imprimir");
			var puedeEliminar = this.GetUserPermission(user, "Eliminar");
			var puedeImportar = this.GetUserPermission(user, "Importar");
			var puedeEditar = this.GetUserPermission(user, "Editar");
			var puedeActualizar = this.GetUserPermission(user, "Actualizar");
			var puedeVisualizar = this.GetUserPermission(user, "Visualizar");
			var puedeReportesLiquidacion = this.GetUserPermission(user, "ReportesLiquidacion");
			var puedeAprobacionComercial = this.GetUserPermission(user, "AprobacionComercial");
			var puedeAprobacionLogistica = this.GetUserPermission(user, "AprobacionLogistica");
			var puedeVerCosto = this.GetUserPermission(user, "Visualizar");

			this.ViewBag.PuedeAdicionar = puedeAdicionar;
			this.ViewBag.PuedeAprobar = puedeAprobar;
			this.ViewBag.PuedeAutorizar = puedeAutorizar;
			this.ViewBag.PuedeAnular = puedeAnular;
			this.ViewBag.PuedeCerrar = puedeCerrar;
			this.ViewBag.PuedeCopiar = puedeCopiar;
			this.ViewBag.PuedeReversar = puedeReversar;
			this.ViewBag.PuedeImprimir = puedeImprimir;
			this.ViewBag.PuedeEliminar = puedeEliminar;
			this.ViewBag.PuedeImportar = puedeImportar;
			this.ViewBag.PuedeEditar = puedeEditar;
			this.ViewBag.PuedeActualizar = puedeActualizar;
			this.ViewBag.PuedeVisualizar = puedeVisualizar;
			this.ViewBag.PuedeReportesLiquidacion = puedeReportesLiquidacion;
			this.ViewBag.PuedeAprobacionComercial = puedeAprobacionComercial;
			this.ViewBag.PuedeAprobacionLogistica = puedeAprobacionLogistica;
			this.ViewBag.PuedeVerCosto = puedeVerCosto;
		}

		private bool GetUserPermission(User activeUser, string namePermission)
        {
			int id_menu = (int)ViewData["id_menu"];

			if (activeUser == null)
				return false;

			var userMenu = activeUser.UserMenu.FirstOrDefault(e => e.Menu.id == id_menu);

			if(userMenu == null)
            {
				return false;
            }

			var permission = userMenu.Permission.FirstOrDefault(e => e.name == namePermission);

			return permission != null;
		}

		#region ACTIVE STRUCTURE

		private Company m_activeCompany;
		protected int ActiveCompanyId
		{
			get
			{
				if (this.m_activeCompany == null)
				{
					return ActiveUser.id_company;
				}
				else
				{
					return this.m_activeCompany.id;
				}
			}
		}
		protected Company ActiveCompany
		{
			get
			{
				if (this.m_activeCompany == null)
				{
					this.m_activeCompany = db.Company.FirstOrDefault(c => c.id == ActiveUser.id_company);
				}

				return this.m_activeCompany;
			}
		}

		private Division m_activeDivision;
		protected Division ActiveDivision
		{
			get
			{
				if (this.m_activeDivision == null)
				{
					this.m_activeDivision = this.ActiveCompany.Division.FirstOrDefault();
				}

				return this.m_activeDivision;
			}
		}

		private BranchOffice m_activeSucursal;
		protected BranchOffice ActiveSucursal
		{
			get
			{
				if (this.m_activeSucursal == null)
				{
					this.m_activeSucursal = this.ActiveDivision.BranchOffice.FirstOrDefault();
				}

				return this.m_activeSucursal;
			}
		}

		private EmissionPoint m_activeEmissionPoint;
		protected EmissionPoint ActiveEmissionPoint
		{
			get
			{
				if (this.m_activeEmissionPoint == null)
				{
					this.m_activeEmissionPoint = this.ActiveUser.EmissionPoint.FirstOrDefault();
				}

				return this.m_activeEmissionPoint;
			}
		}

		#endregion

		#region COMMUN TOOLBOX FUNCTIONS 

		protected string FormatDate(string date, string[] langs)
		{
			string formatedDate = DateTime.Now.ToString("dd/MM/yyyy");
			try
			{
				formatedDate = DateTime.Parse(date, new CultureInfo(langs.First())).ToString("dd/MM/yyyy");
				return formatedDate;
			}
			catch (Exception)
			{
				formatedDate = DateTime.Now.ToString("dd/MM/yyyy");
				return formatedDate;
			}
		}

		protected int GetDocumentSequential(int id_documentType)
		{
			DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.id == id_documentType && d.id_company == this.ActiveCompanyId);
			return documentType?.currentNumber ?? 0;
		}

		protected string GetDocumentNumber(int id_documentType, int id_ep = 0)
		{

			EmissionPoint ep = null;
			if (id_ep > 0)
			{
				ep = db.EmissionPoint.FirstOrDefault(fod => fod.id == id_ep);
			}
			ep = ep ?? ActiveEmissionPoint;
			string number = GetDocumentSequential(id_documentType).ToString().PadLeft(9, '0');

			string documentNumber = string.Empty;
			documentNumber = $"{ep.BranchOffice.code.ToString().PadLeft(3, '0')}-{ep.code.ToString().PadLeft(3, '0')}-{number}";
			return documentNumber;
		}
        protected int GetDocumentSequentialEmissionPoint(int id_documentType, int id_ep = 0)
        {

            DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.id == id_documentType && d.id_company == this.ActiveCompanyId);
            EmissionPoint ep = null;
            if (id_ep > 0)
            {
                ep = db.EmissionPoint.FirstOrDefault(fod => fod.id == id_ep);
            }
            return ep?.secuenciaValor ?? (documentType?.currentNumber ?? 0);
        }
        protected string GetDocumentNumberEmissionPoint(int id_documentType, int id_ep = 0)
        {

            EmissionPoint ep = null;
            if (id_ep > 0)
            {
                ep = db.EmissionPoint.FirstOrDefault(fod => fod.id == id_ep);
            }
            ep = ep ?? ActiveEmissionPoint;
            string number = GetDocumentSequentialEmissionPoint(id_documentType, id_ep).ToString().PadLeft(9, '0');

            string documentNumber = string.Empty;
            documentNumber = $"{ep.BranchOffice.code.ToString().PadLeft(3, '0')}-{ep.code.ToString().PadLeft(3, '0')}-{number}";
            return documentNumber;
        }

        protected string SuccessMessage(string text = "Registro guardado exitosamente")
		{
			string message = @"<div id=""successMessage"" class=""alert alert-success alert-dismissible fade in"" style=""margin-top: 10px; text-align: center; padding: 10px 15px;"">"
						   + @"<button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""close"" title=""close"" style=""top: 0px; right: 0px;""><span aria-hidden=""true"">&times;</span></button>"
						   + text
						   + @"</div>";
			return message;
		}

		protected string ErrorMessage(string text = "Ha ocurrido un error")
		{
			string message = @"<div id=""errorMessage"" class=""alert alert-danger alert-dismissible fade in"" style=""margin-top: 10px; text-align: center; padding: 10px 15px;"">"
						   + @"<button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""close"" title=""close"" style=""top: 0px; right: 0px;""><span aria-hidden=""true"">&times;</span></button>"
						   + text
						   + @"</div>";
			return message;
		}

		protected string WarningMessage(string text = "Advertencia!, puede ocurrir un error")
		{
			string message = @"<div id=""warningMessage"" class=""alert alert-warning alert-dismissible fade in"" style=""margin-top: 10px; text-align: center; padding: 10px 15px;"">"
						   + @"<button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""close"" title=""close"" style=""top: 0px; right: 0px;""><span aria-hidden=""true"">&times;</span></button>"
						   + text
						   + @"</div>";
			return message;
		}

		protected void UpdateArrayTempDataKeep(string[] arrayTempDataKeep)
		{
			//ViewData["arrayTempDataKeep"] = arrayTempDataKeep;
			TempData["arrayTempDataKeep"] = arrayTempDataKeep;
			TempData.Keep("arrayTempDataKeep");
			if (arrayTempDataKeep != null && arrayTempDataKeep.Length > 0)
			{
				foreach (var str in arrayTempDataKeep)
				{
					TempData.Keep(str);
				}

			}
		}


		protected string GetFullLog(Exception e)
		{
			return MetodosEscrituraLogs.GetExcepcionNestMessage(e);

        }
		protected void FullLog(	Exception e,							    		                        
                                string seccion = null,
                                string extraInfo = null,
                                [CallerFilePath] string callFilePath = "",
								[CallerMemberName] string memberName = "",
								[CallerLineNumber] int lineNumber = 0)
		{
			string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            string origen = this.GetType().Name;
            string app = "Produccion";
            MetodosEscrituraLogs.EscribeExcepcionLogNest(	e, 
															rutaLog, 
															origen, 
															app, 
															seccion: seccion, 
															extraInfo: extraInfo, 
															callFilePath: callFilePath,
															memberName: memberName,
															lineNumber: lineNumber);
        }

		protected void LogWrite(Exception e, string mensaje, string xtraInfo)
		{
			string app = "Produccion";
			string origen = this.GetType().Name;
			string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");

			MetodosEscrituraLogs.LogWrite(e, mensaje, rutaLog, origen, app, xtraInfo);
        }
        protected void LogInfo(string mensaje, DateTime fechaHora )
		{
            string app = "Produccion";
            string origen = this.GetType().Name;
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            MetodosEscrituraLogs.EscribeMensajeLog($"{mensaje} - {fechaHora}", rutaLog, origen, app);
        }

		#endregion

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			try
			{
				_codeStateDocument = "";
				_messageAnswer = "";
				_nameStateDocument = "";
				_strNumberDocAnswer = "";
				_btnOnEditForm = new ButtonsOnEditForm();
				_Answerfa = new AnswerForAction();

                

                User user = this.ActiveUser;

				if (TempData["id_ep"] != null)
				{
					TempData.Keep("id_ep");
				}

				if (user != null)
				{
					ViewData["id_user"] = user.id;
					ViewData["username"] = user.username;


					var activeCompany = db.Company
						.Where(c => c.id == user.id_company)
						.Select(c => new { c.id, c.trademark })
						.FirstOrDefault();
					ViewData["id_company"] = activeCompany?.id;
					ViewData["company"] = activeCompany?.trademark;


					var activeDivision = (activeCompany != null)
						? db.Division
							.Where(d => d.id_company == activeCompany.id)
							.Select(d => new { d.id, d.name })
							.FirstOrDefault()
						: null;
					ViewData["id_division"] = activeDivision?.id;
					ViewData["division"] = activeDivision?.name;


					var activeSucursal = (activeDivision != null)
						? db.BranchOffice
							.Where(d => d.id_division == activeDivision.id)
							.Select(s => new { s.id, s.name })
							.FirstOrDefault()
						: null;
					ViewData["id_sucursal"] = activeSucursal?.id;
					ViewData["sucursal"] = activeSucursal?.name;


					var activePuntoEmision = this.ActiveEmissionPoint;
					ViewData["id_emissionPoint"] = activePuntoEmision?.id;
					ViewData["emissionPoint"] = activePuntoEmision?.name;

					ViewData["id_menu"] = 0;
					ViewData["permissions"] = null;
					ViewData["entityObjectPermissions"] = GetEntityObjectPermissions();

					UserMenu userMenu = null;

					if (TempData["menu"] != null)
					{
						userMenu = (TempData["menu"] as UserMenu);
						List<int> permissions = userMenu?.Permission.Where(x => x.isActive).Select(x => x.id).ToList();

						ViewData["id_menu"] = userMenu?.Menu.id ?? 0;
						ViewData["permissions"] = permissions;
					}

					string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
					string action = filterContext.ActionDescriptor.ActionName;

					Menu menu = db.Menu.FirstOrDefault(m => m.TController.name.Equals(controller) && m.TAction.name.Equals(action));

					if (menu != null)
					{
						userMenu = db.UserMenu.FirstOrDefault(m => m.id_user == user.id && m.Menu.id == menu.id);

						if (TempData["menu"] == null || userMenu != null)
							TempData["menu"] = userMenu;

						userMenu = (TempData["menu"] as UserMenu);
						List<int> permissions = userMenu?.Permission.Where(x => x.isActive).Select(x => x.id).ToList();

						ViewData["id_menu"] = userMenu?.Menu.id ?? 0;
						ViewData["permissions"] = permissions;
					}


				}
			}
			catch (Exception)
			{
                ViewData["id_user"] = 0;
				ViewData["username"] = "";
				ViewData["id_company"] = 0;
				ViewData["company"] = "";
				ViewData["id_division"] = 0;
				ViewData["division"] = "";
				ViewData["id_sucursal"] = 0;
				ViewData["sucursal"] = "";
				ViewData["id_emissionPoint"] = 0;
				ViewData["emissionPoint"] = "";
				ViewData["id_menu"] = 0;
				ViewData["permissions"] = null;
				ViewData["entityObjectPermissions"] = new EntityObjectPermissions();
			}

			TempData.Keep("menu");
		}

		protected EntityObjectPermissions GetEntityObjectPermissions()
		{
			var entityObjectPermissions = new EntityObjectPermissions();
			var userAux = this.ActiveUser;
			if (userAux != null)
			{
				foreach (var item in userAux.UserEntity)
				{
					var entityPermissions = new EntityPermissions
					{
						codeEntity = item.Entity.code,
						nameEntity = item.Entity.name,
						id_entity = item.Entity.id,
						listValue = new List<EntityValuePermissions>()
					};
					foreach (var item2 in item.UserEntityDetail)
					{
						entityPermissions.listValue.Add(new EntityValuePermissions
						{
							id_entityValue = item2.id_entityValue,
							listPermissions = item2.UserEntityDetailPermission.Select(s => s.Permission).ToList()
						});
					}
					entityObjectPermissions.listEntityPermissions.Add(entityPermissions);

				}

				foreach (var item in userAux.UserObject)
				{
					var objectPermissions = new ObjectPermissions
					{
						id_object = item.id_object,
						codeObject = item.Object.code,
						nameObject = item.Object.name
					};

					entityObjectPermissions.listObjectPermissions.Add(objectPermissions);

				}
			}
			return entityObjectPermissions;
		}

		#region Auxiliar

		public static decimal TruncateDecimal(decimal decimalParam)
		{
			var decimalTruncate = decimal.Truncate(decimalParam);
			if ((decimalParam - decimalTruncate) > 0)
			{
				decimalTruncate++;
			};
			return decimalTruncate;
		}

		public static decimal GetFactorConversion(MetricUnit metricOrigin, MetricUnit metricDestiny, string messageError, DBContext db)
		{
			decimal factorConversionAux;
			try
			{
				factorConversionAux = (metricOrigin.id != metricDestiny.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricOrigin.id &&
																															fod.id_metricDestiny == metricDestiny.id)?.factor ?? 0 : 1;
				if (factorConversionAux == 0)
				{
					throw new Exception(messageError);

				}
			}
			catch (Exception e)
			{
				throw e;
			}


			return factorConversionAux;
		}

		public static ReportParanNameModel GetTmpDataName(int strLength)
		{
			return new ReportParanNameModel
			{
				nameQS = GeneralStr.GetRandomStr(strLength),
				isEncripted = true,
				messError = "",
			};
		}

		public static Conexion GetObjectConnection(string strConex)
		{
			string strCon = ConfigurationManager.ConnectionStrings[strConex].ConnectionString;
			Conexion _conex = null;

			if (!string.IsNullOrWhiteSpace(strCon))
			{
				string[] lstConex = strCon.Split('=');
				_conex = new Conexion();
				if (lstConex.Length > 0)
				{
					_conex.SrvName = lstConex[1].Split(';')[0];
					_conex.DbName = lstConex[2].Split(';')[0];
					_conex.UsrName = lstConex[3].Split(';')[0];
					_conex.PswName = lstConex[4].Split(';')[0];
				}
			}
			return _conex;
		}

		[HttpPost, ValidateInput(false)]
		public void EmissionPointChanged(int id_ep)
		{
			if (id_ep > 0)
			{
				TempData["id_ep"] = id_ep;
				TempData.Keep("id_ep");
			}

		}

		public static ButtonsOnEditForm GetActionsOnButtons()
		{
			return null;
		}

		public static ButtonsOnEditFormRemissionGuide GetActionsOnButtonsRemissionGuide(int id, string state)
		{
			ButtonsOnEditFormRemissionGuide actions = new ButtonsOnEditFormRemissionGuide
			{
				btnApprove = false,
				btnAutorize = false,
				btnProtect = false,
				btnCancel = false,
				btnRevert = false,
				btnreassignment = false,
				btnCancelRG = false,
				btnPrint = false,
				btnPrintAlldoc = false,
				btnPrintManual = false,
				btnPrintAlldocManual = false,
				btnSave = true,
			};

			if (id == 0)
			{
				return actions;
			}

			if (state == "01") // PENDIENTE
			{
				actions = new ButtonsOnEditFormRemissionGuide
				{
					btnApprove = true,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = true,
					btnRevert = false,
					btnreassignment = false,
					btnCancelRG = false,
					btnPrint = false,
					btnPrintAlldoc = false,
					btnPrintManual = false,
					btnPrintAlldocManual = false,
					btnSave = true,
				};
			}
			else if (state == "03")//|| state == 3) // APROBADA
			{
				actions = new ButtonsOnEditFormRemissionGuide
				{
					btnApprove = false,
					btnAutorize = true,
					btnProtect = false,
					btnCancel = false,
					btnRevert = true,
					btnreassignment = false,
					btnCancelRG = false,
					btnPrint = true,
					btnPrintAlldoc = true,
					btnPrintManual = true,
					btnPrintAlldocManual = true,
					btnSave = false
				};
			}
			else if (state == "04" || state == "05") // CERRADA O ANULADA
			{
				actions = new ButtonsOnEditFormRemissionGuide
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = false,
					btnreassignment = false,
					btnCancelRG = false,
					btnPrint = false,
					btnPrintAlldoc = false,
					btnPrintManual = false,
					btnPrintAlldocManual = false,
					btnSave = false
				};
			}
			else if (state == "06") // AUTORIZADA
			{
				actions = new ButtonsOnEditFormRemissionGuide
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = true,
					btnreassignment = true,
					btnCancelRG = true,
					btnPrint = true,
					btnPrintAlldoc = true,
					btnPrintManual = true,
					btnPrintAlldocManual = true,
					btnSave = false
				};
			}
			else if (state == "08") //NO SE CUAL SERA ESTE ESTADO
			{
				actions = new ButtonsOnEditFormRemissionGuide
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = false,
					btnreassignment = false,
					btnCancelRG = false,
					btnPrint = false,
					btnPrintAlldoc = false,
					btnPrintManual = false,
					btnPrintAlldocManual = false,
					btnSave = false
				};
			}

			return actions;
		}

		#endregion

		protected bool validarDetalleInventoryPeriod(InventoryPeriodDetail inventoryPeriodDetail, ref string mensaje, ref int IdSiguiente, ref int IdEStadoA)
		{
			bool result = true;
			IdSiguiente = 0;
			int idEstadActivre = 0;
			int idEstadPendiente = 0;
			int idEstadCerrado = 0;
			InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);

			inventoryPeriod = inventoryPeriod ?? db.InventoryPeriod.Where(i => i.id == inventoryPeriod.id).FirstOrDefault();
			inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();

			var cantre = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.dateInit >= x.dateInit && inventoryPeriodDetail.dateInit <= x.dateEnd).ToList().Count();
			if (cantre > 0)
			{
				mensaje = ErrorMessage("Ya existe un Periodo con el mismo rango de fecha");
				TempData.Keep("InventoryPeriod");
				ViewData["EditMessage"] = mensaje;
				result = false;
				return result;
			}


			var cantrefin = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.dateEnd >= x.dateInit && inventoryPeriodDetail.dateEnd <= x.dateEnd).ToList().Count();
			if (cantrefin > 0)
			{
				mensaje = ErrorMessage("Y existe un Periodo con el mismo rango de fecha");
				TempData.Keep("InventoryPeriod");
				ViewData["EditMessage"] = mensaje;
				result = false;
				return result;
			}

			var asignacionCosto = db.CostAllocation
									 .Where(r => r.CostAllocationWarehouse.Where(t => t.id_Warehouse == inventoryPeriod.id_warehouse).Count() > 0
												 && r.anio == inventoryPeriod.year
												 && r.mes == inventoryPeriodDetail.periodNumber
												 && r.Document.DocumentState.code == "03"
												 ).Count();

			if (asignacionCosto > 0)
			{
				mensaje = ErrorMessage("Existe una Asignación de Costo activa en este Periodos/Bodegas. Reverse la Asignación de Costo");
				TempData.Keep("InventoryPeriod");
				ViewData["EditMessage"] = mensaje;
				result = false;
				return result;
			}

			var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
			if (vestado != null)
			{
				var idestado = (from es in vestado
								where es.valueCode.ToUpper().Trim() == "A"
								select es).FirstOrDefault();

				if (idestado != null)
				{

					idEstadActivre = idestado.id;

					var idPendiente = (from es in vestado
									   where es.valueCode.ToUpper().Trim() == "P"
									   select es).FirstOrDefault();
					idEstadPendiente = idPendiente.id;

					var idCerrado = (from es in vestado
									 where es.valueCode.ToUpper().Trim() == "C"
									 select es).FirstOrDefault();

					idEstadCerrado = idCerrado.id;

					if (inventoryPeriodDetail.id_PeriodState == idestado.id)
					{
						var cantestado = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.id_PeriodState == x.id_PeriodState).ToList().Count();

						if (cantestado > 0)
						{
							TempData.Keep("InventoryPeriod");
							ViewData["EditMessage"] = ErrorMessage("Y existe un Periodo Activo");
							mensaje = ErrorMessage("Y existe un Periodo Activo");
							result = false;
							return result;
						}

						var cantCerrado = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && (x.id_PeriodState == idEstadPendiente || x.id_PeriodState == idEstadActivre) && x.periodNumber < inventoryPeriodDetail.periodNumber).ToList().Count();

						if (cantCerrado > 0 && result == true)
						{
							TempData.Keep("InventoryPeriod");
							ViewData["EditMessage"] = ErrorMessage(" Todos los periodos anteriores  deben estar como CERRADO");
							mensaje = ErrorMessage("Todos los periodos anteriores  deben estar como CERRADO");
							result = false;
							return result;
						}


						var cantPendiente = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && (x.id_PeriodState == idEstadCerrado || x.id_PeriodState == idEstadActivre) && x.periodNumber > inventoryPeriodDetail.periodNumber).ToList().Count();

						if (cantPendiente > 0 && result == true)
						{
							TempData.Keep("InventoryPeriod");
							ViewData["EditMessage"] = ErrorMessage(" Todos los periodos anteriores  deben estar como PENDIENTE");
							mensaje = ErrorMessage("Todos los periodos anteriores  deben estar como PENDIENTE");
							result = false;
							return result;
						}
						 

					}
					else
					{
						IdEStadoA = idestado.id;
					}

				}


				idestado = (from es in vestado
							where es.valueCode.ToUpper().Trim() == "C"
							select es).FirstOrDefault();

				if (idestado != null)
				{
					if (inventoryPeriodDetail.id_PeriodState == idestado.id)
					{
						var idsi = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idestado.id != x.id_PeriodState && x.periodNumber > inventoryPeriodDetail.periodNumber).FirstOrDefault();

						if (idsi == null)
						{
							TempData.Keep("InventoryPeriod");
							ViewData["EditMessage"] = ErrorMessage("No existe otro periodo Pendiente");
							mensaje = ErrorMessage("No existe otro periodo Pendiente");
							result = false;
							return result;
						}
						else
						{

							var registroactivo = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idEstadActivre == x.id_PeriodState).ToList().Count();

							if (registroactivo <= 0)
							{
								IdSiguiente = idsi.id;
							}
						}
					}

				}

			}

			return result;

		}

        #region Permisos por objeto

		public bool EstaAutorizadoObjeto(string m_ConstController, string codigo)
        {
			// Busco el controllador para verificar permiso del usuario
			var idController = db.TController.FirstOrDefault(e => e.name == m_ConstController).id;

			// Busco El objeto para el permiso
			var idObjectPermission = db.ObjectPermission.FirstOrDefault(fod => fod.code == codigo).id;

			// Busco en la tabla de objeto usuario
			var objectPermission = this.ActiveUser.ObjectPermissionUser.Any(e => e.id_controller == idController 
											&& e.id_objectPemission == idObjectPermission && e.isActive);

			return objectPermission;
        }

		#endregion

		#region TransCtl

		private void prepareDataTemp(string[] dataTempKeys, string[] dataTempValues, string[] dataTempTypes)
        {
            if ((dataTempKeys.Length != dataTempValues.Length) && (dataTempKeys.Length != dataTempTypes.Length) )
            {
                throw new ArgumentException("Los arrays deben tener el mismo número de elementos.");
            }

            for (int i = 0; i < dataTempKeys.Length; i++)
            {
				TempData[dataTempKeys[i]] = ServiceTransCtl.ConvertirTextoATipo(dataTempValues[i], dataTempTypes[i]);
				TempData.Keep(dataTempKeys[i]);
            }
        }
 
        protected void SetInfoForTrans(ActiveUserDto activeUser, string dataTempKeys, string dataTempValues, string dataTempTypes)
        {			
            if (!string.IsNullOrEmpty(dataTempKeys) && !string.IsNullOrEmpty(dataTempValues))
			{
				string[] keyAr = JsonConvert.DeserializeObject<string[]>(dataTempKeys);
                string[] valueAr = JsonConvert.DeserializeObject<string[]>(dataTempValues);
                string[] typeAr = JsonConvert.DeserializeObject<string[]>(dataTempTypes);

				prepareDataTemp(keyAr, valueAr, typeAr);

            }

			if (activeUser.permisos != null)
			{
                ViewData["entityObjectPermissions"] = (EntityObjectPermissions)activeUser.permisos.ToModel();                
            }

			this.m_activeUser = db.User.FirstOrDefault(u => u.id == activeUser.id);

        }

        #endregion
    }
}