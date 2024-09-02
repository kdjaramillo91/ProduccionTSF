using Dapper;
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utilitarios.Logs;
using Utilitarios.ProdException;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class AccountingFreightController : DefaultController
    {
        const string SELECT_AccountingFreight = "SELECT acc.Id, acc.id_processPlant, pp.processPlant processPlantName, acc.liquidation_type, \r\n" +
            "CASE WHEN acc.liquidation_type = 'T' THEN 'TERRESTRE' ELSE 'FLUVIAL' END liquidationName, acc.isActive\r\n" +
            "FROM AccountingFreight acc\r\n" +
            "INNER JOIN (SELECT  p.id, p.processPlant FROM Person p \r\n" +
            "inner join PersonRol pr on p.id = pr.id_person\r\n" +
            "inner join Rol r  on pr.id_rol = r.id and r.name ='Planta Proceso'\r\n)" +
            " pp ON acc.id_processPlant = pp.id";


        const string SELECT_AccountingFreight_ID = "SELECT acc.Id, acc.id_processPlant, pp.processPlant processPlantName, acc.liquidation_type, \r\n" +
            "CASE WHEN acc.liquidation_type = 'T' THEN 'TERRESTRE' ELSE 'FLUVIAL' END liquidationName, acc.isActive\r\n" +
            "FROM AccountingFreight acc\r\n" +
            "INNER JOIN (SELECT  p.id, p.processPlant FROM Person p \r\n" +
            "inner join PersonRol pr on p.id = pr.id_person\r\n" +
            "inner join Rol r  on pr.id_rol = r.id and r.name ='Planta Proceso'\r\n) " +
            " pp ON acc.id_processPlant = pp.id " +
            " WHERE acc.id = @id ";
        
        const string SELECT_AccountingFreightDetail = "SELECT id, id_accountingFreight, accountingAccountCode,isAuxiliar,code_auxiliar, idAuxContable , id_costCenter, id_subCostCenter\r\n" +
            "FROM AccountingFreightDetails\r\n" +
            "WHERE isactive = 1 AND  Id = @id_accountingFreight";

        const string SELECT_AccountingFreightDetailEdit = "SELECT id, id_accountingFreight, accountingAccountCode,isAuxiliar,code_auxiliar, idAuxContable , id_costCenter, id_subCostCenter\r\n" +
        ",accountType,id_userCreate, dateCreate, id_userUpdate, dateUpdate, isActive\r\n" +
        "FROM AccountingFreightDetails\r\n" +
        "WHERE isactive = 1 AND  id_accountingFreight = @id_accountingFreight";

        const String Max_sequence = "SELECT ISNULL(MAX(id),0) FROM AccountingFreight";
        const String Max_sequence_Det = "SELECT ISNULL(MAX(id),0) FROM AccountingFreightDetails";

        // GET: AccountingFreight
        public ActionResult Index()
        {
            return PartialView();
        }

        #region FILTERS RESULTS
        [HttpPost]
        public ActionResult AccountingFreightResults(AccountingFreightResult accountingFreightResult,int? id_processPlant,
                                                  string liquidation_type)
        {
            
            var result = DapperConnection.Execute<AccountingFreightResult>(SELECT_AccountingFreight);

            if(result.Count() > 0)
            {
                result = result.ToArray();
            }
            else
            {
                result = Array.Empty<AccountingFreightResult>();
            }

            if (id_processPlant > 0)
            {
                result = result.Where(d => d.id_processPlant == (int)id_processPlant).ToArray();
            }

            if (liquidation_type != null)
            {
                result = result.Where(d => d.liquidation_type == liquidation_type).ToArray();
            }


            this.TempData["AccountingFreightResult"] = result;
            this.TempData.Keep("AccountingFreightResult");
            return PartialView("_AccountingFreightResultPartial", result.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingFreightPartial()
        {

            var model = (this.TempData["AccountingFreightResult"] as AccountingFreightResult[])?.ToList() ?? new List<AccountingFreightResult>();
            this.TempData.Keep("AccountingFreightResult");
            return PartialView("_AccountingFreightPartial", model);
        }

        #endregion

        #region Edit StateOfContry
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditAccountingFreight(int id)
        {
            var custodianIncomeList = DapperConnection.Execute<AccountingFreight>(SELECT_AccountingFreight_ID, new
            {
                id = id
            }).FirstOrDefault();

            if (custodianIncomeList == null)
            {

                custodianIncomeList = new AccountingFreight
                {

                    id_userCreate = ActiveUser.id,
                    dateCreate = DateTime.Now,
                    isActive = true,
                };
            }

            if (custodianIncomeList.AccountingFreightDetails == null)
            {
                var accountingFreightDetails = DapperConnection.Execute<AccountingFreightDetails>(SELECT_AccountingFreightDetailEdit, new
                {
                    id_accountingFreight = custodianIncomeList.id
                }).ToList();

                accountingFreightDetails.ForEach(detail =>
                {
                    detail.id_userUpdate = ActiveUser.id;
                    detail.dateUpdate = DateTime.Now;
                });

                custodianIncomeList.AccountingFreightDetails = accountingFreightDetails;
            }

            TempData["AccountingFreight"] = custodianIncomeList;
            TempData.Keep("AccountingFreight");

            return PartialView("_FormEditAccountingFreight", custodianIncomeList);
        }

        #endregion

        // GET: AccountingFreight/Details/5
        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingFreightDetailsEditFormPartial()
        {

            List<AccountingFreightDetails> model = new List<AccountingFreightDetails>();

            var accountingFreight = (TempData["AccountingFreight"] as AccountingFreight);

            int idaccountingFreight = (accountingFreight?.id ?? 0);

            if (accountingFreight == null)
            {
                accountingFreight = DapperConnection.Execute<AccountingFreight>(SELECT_AccountingFreight_ID, new
                {
                    id = idaccountingFreight
                }).FirstOrDefault();

            }

            accountingFreight = accountingFreight ?? new AccountingFreight();

            if (accountingFreight.AccountingFreightDetails == null)
            {
                accountingFreight.AccountingFreightDetails = new List<AccountingFreightDetails>();
            }

            if(accountingFreight.AccountingFreightDetails == null)
            {
                var accountingFreightDetails = DapperConnection.Execute<AccountingFreightDetails>(SELECT_AccountingFreightDetail, new
                {
                    id_accountingFreight = accountingFreight.id
                }).ToList();

                accountingFreight.AccountingFreightDetails = accountingFreightDetails;
            }

            model = accountingFreight?.AccountingFreightDetails.ToList() ?? new List<AccountingFreightDetails>();

            TempData["AccountingFreight"] = TempData["AccountingFreight"] ?? accountingFreight;
            TempData.Keep("AccountingFreight");


            return PartialView("_AccountingFreightDetailsEditFormPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingFreightDetailsEditFormPartialAddNew(int id_accountingFreight, AccountingFreightDetails accountingFreightDetails)
        {

            var accountingFreight = this.GetEditingAccountingFreight(
               id_accountingFreight);


            var currentDetail = accountingFreight.AccountingFreightDetails?
                    .FirstOrDefault(d => d.accountingAccountCode == accountingFreightDetails.accountingAccountCode
                                            && d.isAuxiliar ==  accountingFreightDetails.isAuxiliar
                                            && d.code_Auxiliar == accountingFreightDetails.code_Auxiliar
                                            && d.idAuxContable == accountingFreightDetails.idAuxContable
                                            && d.id_costCenter == accountingFreightDetails.id_costCenter
                                            && d.id_subCostCenter == accountingFreightDetails.id_subCostCenter);

            if (currentDetail != null)
            {
                // Si ya existe y está activo, no permitimos duplicados...
                this.ViewBag.EditError = $"Ya existe un elemento con el código indicado. Código: {accountingFreightDetails.accountingAccountCode}.";
            }

            if (accountingFreightDetails.accountType == null)
            {
                this.ViewBag.EditError = "Debe seleccionar tipo de cuenta Débito o Crédito";
            }

            if (accountingFreightDetails.code_Auxiliar == null)
            {
                this.ViewBag.EditError = "Debe seleccionar el código auxiliar de la cuenta";
            }

            if (accountingFreightDetails.idAuxContable == null)
            {
                this.ViewBag.EditError = "Debe seleccionar un tipo de auxiliar contable";
            }
            
            if (accountingFreightDetails.accountingAccountCode == null)
            {
                this.ViewBag.EditError = "Debe seleccionar una cuenta contable";
            }




            if(string.IsNullOrEmpty(this.ViewBag.EditError))
            {

                accountingFreight.AccountingFreightDetails
                    .Add(new AccountingFreightDetails()
                    {
                        id = this.CalculateNextSequenceDetail(),
                        id_accountingFreight = id_accountingFreight,
                        accountingAccountCode = accountingFreightDetails.accountingAccountCode,
                        isAuxiliar = accountingFreightDetails.isAuxiliar,
                        code_Auxiliar = accountingFreightDetails.code_Auxiliar,
                        idAuxContable = accountingFreightDetails.idAuxContable,
                        id_costCenter = accountingFreightDetails.id_costCenter,
                        id_subCostCenter = accountingFreightDetails.id_subCostCenter,
                        accountType = accountingFreightDetails.accountType,
                        isActive = true,
                        id_userCreate = ActiveUser.id,
                        dateCreate = DateTime.Now
                    });
            }

            this.TempData["AccountingFreight"] = accountingFreight;
            this.TempData.Keep("AccountingFreight");

            return PartialView("_AccountingFreightDetailsEditFormPartial", accountingFreight.AccountingFreightDetails);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingFreightDetailsEditFormPartialUpdate(int id_accountingFreight, AccountingFreightDetails accountingFreightDetails)
        {
            var accountingFreight = this.GetEditingAccountingFreight(
               id_accountingFreight);

            var currentDetail = accountingFreight.AccountingFreightDetails?
                   .FirstOrDefault(d => d.accountingAccountCode == accountingFreightDetails.accountingAccountCode
                                           && d.isAuxiliar == accountingFreightDetails.isAuxiliar
                                           && d.code_Auxiliar == accountingFreightDetails.code_Auxiliar
                                           && d.idAuxContable == accountingFreightDetails.idAuxContable
                                           && d.id_costCenter == accountingFreightDetails.id_costCenter
                                           && d.id_subCostCenter == accountingFreightDetails.id_subCostCenter
                                           && d.accountType == accountingFreightDetails.accountType
                                           && d.isActive);


            if (currentDetail != null)
            {
                // Si ya existe y está activo, no permitimos duplicados...
                this.ViewBag.EditError = $"Ya existe un elemento con el código indicado. Código: {accountingFreightDetails.accountingAccountCode}.";
            }

            if (accountingFreightDetails.accountType == null)
            {
                this.ViewBag.EditError = "Debe seleccionar tipo de cuenta Débito o Crédito";
            }

            if (accountingFreightDetails.code_Auxiliar == null)
            {
                this.ViewBag.EditError = "Debe seleccionar el código auxiliar de la cuenta";
            }

            if (accountingFreightDetails.idAuxContable == null)
            {
                this.ViewBag.EditError = "Debe seleccionar un tipo de auxiliar contable";
            }

            if (accountingFreightDetails.accountingAccountCode == null)
            {
                this.ViewBag.EditError = "Debe seleccionar una cuenta contable";
            }



            if (string.IsNullOrEmpty(this.ViewBag.EditError))
            {
                currentDetail = accountingFreight.AccountingFreightDetails?.Where(x => x.id == accountingFreightDetails.id).FirstOrDefault();

                currentDetail.id_accountingFreight = accountingFreightDetails.id_accountingFreight;
                currentDetail.accountingAccountCode = accountingFreightDetails.accountingAccountCode;
                currentDetail.isAuxiliar = accountingFreightDetails.isAuxiliar;
                currentDetail.code_Auxiliar = accountingFreightDetails.code_Auxiliar;
                currentDetail.idAuxContable = accountingFreightDetails.idAuxContable;
                currentDetail.id_costCenter = accountingFreightDetails.id_costCenter;
                currentDetail.id_subCostCenter = accountingFreightDetails.id_subCostCenter;
                currentDetail.accountType = accountingFreightDetails.accountType;
                currentDetail.isActive = true;
                currentDetail.id_userUpdate = ActiveUser.id;
                currentDetail.dateUpdate = DateTime.Now;

            }


            this.TempData["AccountingFreight"] = accountingFreight;
            this.TempData.Keep("AccountingFreight");

            return PartialView("_AccountingFreightDetailsEditFormPartial", accountingFreight.AccountingFreightDetails);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingFreightDetailsEditFormPartialDelete(int id_accountingFreight, AccountingFreightDetails accountingFreightDetails)
        {
            var accountingFreight = this.GetEditingAccountingFreight(
                          id_accountingFreight);

            var currentDetail = accountingFreight.AccountingFreightDetails?.Where(x => x.id == accountingFreightDetails.id).FirstOrDefault();

            var accountingFreightDetailsDeleted = new List<AccountingFreightDetails>();
            if (currentDetail != null)
            {
                // Si existe, lo inactivamos...
                accountingFreight.AccountingFreightDetails.Remove(currentDetail);
                currentDetail.isActive = false;
                accountingFreightDetailsDeleted.Add(currentDetail);
            }


            this.TempData["AccountingFreight"] = accountingFreight;
            this.TempData["AccountingFreightDeleted"] = accountingFreightDetailsDeleted;
            this.TempData.Keep("AccountingFreight");
            this.TempData.Keep("AccountingFreightDeleted");

            return PartialView("_AccountingFreightDetailsEditFormPartial", accountingFreight.AccountingFreightDetails);

        }
        

        private AccountingFreight GetEditingAccountingFreight(int? id_accountingFreight)
        {
            // Recuperamos el elemento del caché local
            var accountingFreight = (this.TempData["AccountingFreight"] as AccountingFreight);

            // Si no hay elemento en el caché, consultamos desde la base
            if ((accountingFreight == null) && id_accountingFreight.HasValue)
            {
                accountingFreight = DapperConnection.Execute<AccountingFreight>(SELECT_AccountingFreight_ID, new
                {
                    id = id_accountingFreight
                }).FirstOrDefault();
            }

            // Si no existe, creamos un nuevo elemento
            if (accountingFreight == null)
            {
                accountingFreight = new AccountingFreight
                {
                    id = this.CalculateNextSequence(),
                    id_userCreate = ActiveUser.id,
                    dateCreate = DateTime.Now,
                    isActive = true,
                };
            }

            if (accountingFreight.AccountingFreightDetails == null)
            {
                var accountingFreightDetails = DapperConnection.Execute<AccountingFreightDetails>(SELECT_AccountingFreightDetail, new
                {
                    id = accountingFreight.id
                }).ToList();

                accountingFreight.AccountingFreightDetails = accountingFreightDetails;
            }

            return accountingFreight;
        }

        private int CalculateNextSequence()
        {
            
            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;

            int currentSequence;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();

                currentSequence = cnn.Query<int>(Max_sequence).FirstOrDefault();

                cnn.Close();
            };
            return currentSequence + 1;
        }

        private int CalculateNextSequenceDetail()
        {

            var dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;

            int currentSequence;
            using (var cnn = new SqlConnection(dapperDBContext))
            {
                cnn.Open();

                currentSequence = cnn.Query<int>(Max_sequence_Det).FirstOrDefault();

                cnn.Close();
            };
            return currentSequence + 1;
        }

        [HttpPost]
        public ActionResult GetSubCostCenter(int? id_costCenterDetail, int? id_subCostCenterDetail, int? idWarehouse)
        {
            AccountingFreight inventoryMove = (TempData["AccountingFreight"] as AccountingFreight);
            List<CostCenter> items = new List<CostCenter>();
            if (id_costCenterDetail != null)
            {
                items = db.CostCenter.Where(w => (w.id_higherCostCenter == id_costCenterDetail && w.isActive) ||
                                                    (w.id == id_subCostCenterDetail)).ToList();
            }
            else
            {
                items = db.CostCenter.Where(w => w.id == id_subCostCenterDetail).ToList();
            }
            TempData.Keep("AccountingFreight");


            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "id_subCostCenterDetail";
                p.Width = Unit.Percentage(100);

                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.CallbackRouteValues = new { Controller = "AccountingFreight", Action = "GetSubCostCenter" };
                p.ClientSideEvents.BeginCallback = "InventoryMoveSubCostCenter_BeginCallback";
                p.ClientSideEvents.EndCallback = "InventoryMoveSubCostCenter_EndCallback";
                p.ClientSideEvents.Validation = "OnSubCostCenterDetailValidation";
                p.CallbackPageSize = 5;
                p.ValueField = "id";
                p.TextField = "name";
                p.ValueType = typeof(int);
                p.BindList(items);
            });
        }


        [HttpPost]
        public JsonResult QueryTiposAuxiliarContables(
            string idCuentaContable)
        {
            var items = DataProviderAccountingFreight
                .AccountingCountTypeAuxiliar(idCuentaContable)
                .Select(ta => new
                {
                    idTipoAuxContable = ta.CCiTipoAuxiliar,
                    tipoAuxContable = ta.CDsTipoAuxiliar,
                })
                .OrderBy(ta => ta.idTipoAuxContable)
                .ToArray();

            var result = new
            {
                isValid = true,
                items,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult QueryAuxiliaresContables(string idTipoAuxContable)
        {
            var items = DataProviderAccountingFreight
                .GetAuxiliaresContablesByCurrent(idTipoAuxContable, null);

            var result = new
            {
                isValid = true,
                items,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: AccountingFreight/Create
        [HttpPost]
        public JsonResult Create(int id_processPlant, string liquidation_type)
        {
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            string message = ""; // Inicializamos message
            bool isValid = true; // Inicializamos isValid
            int newId = 0;
            AccountingFreight tempAccountingFreight = (TempData["AccountingFreight"] as AccountingFreight);
            try
            {
                if (tempAccountingFreight == null)
                {
                    throw new ProdHandlerException("No existen datos para plantilla desde el cache");
                }

                if (id_processPlant == 0)
                {
                    throw new ProdHandlerException("Debe seleccionar una planta de proceso");
                }

                if (liquidation_type == "")
                {
                    throw new ProdHandlerException("Debe seleccionar un tipo de liquidacion");
                }


                if(tempAccountingFreight.AccountingFreightDetails.Where(x => x.accountType == "D").Count() == 0)
                {
                    throw new ProdHandlerException("Falta una cuenta de Débito");
                }

                if (tempAccountingFreight.AccountingFreightDetails.Where(x => x.accountType == "C").Count() == 0)
                {
                    throw new ProdHandlerException("Falta una cuenta de Crébito");
                }

                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar la cabecera
                        var insertHeaderCommand = db.Database.Connection.CreateCommand();
                        insertHeaderCommand.CommandText = "INSERT INTO AccountingFreight (id_processPlant, liquidation_type, id_userCreate, dateCreate, isActive )\r\n" +
                            " VALUES (@id_processPlant, @liquidation_type, @id_userCreate, @dateCreate, @isActive )\r\n; SELECT SCOPE_IDENTITY();";
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@id_processPlant", id_processPlant));
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@liquidation_type", liquidation_type));
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@id_userCreate", tempAccountingFreight.id_userCreate));
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@dateCreate", tempAccountingFreight.dateCreate));
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@isActive", tempAccountingFreight.isActive));
                        insertHeaderCommand.Transaction = trans.UnderlyingTransaction;

                        newId = Convert.ToInt32(insertHeaderCommand.ExecuteScalar());

                        // 2. Insertar los detalles
                        foreach (var detalle in tempAccountingFreight.AccountingFreightDetails)
                        {
                            var insertDetailCommand = db.Database.Connection.CreateCommand();
                            insertDetailCommand.CommandText = "INSERT INTO AccountingFreightDetails\r\n" +
                                "(id_accountingFreight,accountingAccountCode,isAuxiliar,code_Auxiliar,idAuxContable,id_costCenter,\r\n" +
                                "id_subCostCenter,accountType,id_userCreate,dateCreate,isActive)\r\n" +
                                "VALUES (@id_accountingFreight,@accountingAccountCode,@isAuxiliar,@code_Auxiliar,@idAuxContable,@id_costCenter,\r\n" +
                                "@id_subCostCenter,@accountType,@id_userCreate,@dateCreate,@isActive)";
                            insertDetailCommand.Parameters.Add(new SqlParameter("@id_accountingFreight", newId));
                            insertDetailCommand.Parameters.Add(new SqlParameter("@accountingAccountCode", detalle.accountingAccountCode));
                            insertDetailCommand.Parameters.Add(new SqlParameter("@isAuxiliar", detalle.isAuxiliar));
                            insertDetailCommand.Parameters.Add(new SqlParameter("@code_Auxiliar", detalle.code_Auxiliar));
                            insertDetailCommand.Parameters.Add(new SqlParameter("@idAuxContable", detalle.idAuxContable));
                            if (detalle.id_costCenter != null)
                            {insertDetailCommand.Parameters.Add(new SqlParameter("@id_costCenter", detalle.id_costCenter));
                            }
                            else
                            {insertDetailCommand.Parameters.Add(new SqlParameter("@id_costCenter", DBNull.Value));
                            }
                            if (detalle.id_subCostCenter != null)
                            {insertDetailCommand.Parameters.Add(new SqlParameter("@id_subCostCenter", detalle.id_subCostCenter));
                            }
                            else
                            {insertDetailCommand.Parameters.Add(new SqlParameter("@id_subCostCenter", DBNull.Value));
                            }
                            insertDetailCommand.Parameters.Add(new SqlParameter("@accountType", detalle.accountType));
                            insertDetailCommand.Parameters.Add(new SqlParameter("@id_userCreate", detalle.id_userCreate));
                            insertDetailCommand.Parameters.Add(new SqlParameter("@dateCreate", detalle.dateCreate));
                            insertDetailCommand.Parameters.Add(new SqlParameter("@isActive", detalle.isActive));
                            insertDetailCommand.Transaction = trans.UnderlyingTransaction;
                            insertDetailCommand.ExecuteNonQuery();
                        }

                        // Commit la transacción
                        trans.Commit();
                        isValid = true;
                        message = "Plantilla guardada con exito";

                    }
                    catch (Exception ex)
                    {
                        // En caso de error, deshacer la transacción
                        TempData.Keep("AccountingFreight");
                        trans.Rollback();
                        isValid = false;
                        message = ex.Message;
                        MetodosEscrituraLogs.EscribeExcepcionLogNest(ex, rutaLog, "AccountingFreight", "Produccion");
                        // Aquí maneja el error según tus necesidades (p. ej., log, mensaje al usuario, etc.)
                    }
                }


            }
            catch (ProdHandlerException e)
            {
                TempData.Keep("AccountingFreight");
                ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                isValid = false;
                message = e.Message;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "AccountingFreight", "Produccion");
            }
            catch (Exception e)
            {
                TempData.Keep("AccountingFreight");
                ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "AccountingFreight", "Produccion");
                message = "Error al crear elemento: " + e.Message;
                isValid = false;
            }

            var result = new
            {
                id = newId,
                message,
                isValid,
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Update(int id_processPlant, string liquidation_type) 
        {

            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            string message = ""; // Inicializamos message
            bool isValid = true; // Inicializamos isValid
            int newId = 0;
            AccountingFreight tempAccountingFreight = (TempData["AccountingFreight"] as AccountingFreight);
            var tempAccountingFreightDeleted = (TempData["AccountingFreightDeleted"] as List<AccountingFreightDetails>);
            try
            {
                if (tempAccountingFreight == null)
                {
                    throw new ProdHandlerException("No existen datos para plantilla desde el cache");
                }

                if (id_processPlant == 0)
                {
                    throw new ProdHandlerException("Debe seleccionar una planta de proceso");
                }

                if (liquidation_type == "")
                {
                    throw new ProdHandlerException("Debe seleccionar un tipo de liquidacion");
                }


                if (tempAccountingFreight.AccountingFreightDetails.Where(x => x.accountType == "D").Count() == 0)
                {
                    throw new ProdHandlerException("Falta una cuenta de Débito");
                }

                if (tempAccountingFreight.AccountingFreightDetails.Where(x => x.accountType == "C").Count() == 0)
                {
                    throw new ProdHandlerException("Falta una cuenta de Crébito");
                }

                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar la cabecera
                        var insertHeaderCommand = db.Database.Connection.CreateCommand();
                        insertHeaderCommand.CommandText = "UPDATE AccountingFreight SET id_processPlant = @id_processPlant, " +
                            "liquidation_type = @liquidation_type , id_userUpdate = @id_userUpdate, " +
                            "dateUpdate = @dateUpdate \r\n" +
                            "WHERE Id = @Id ";
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@id_processPlant", id_processPlant));
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@liquidation_type", liquidation_type));
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@id_userUpdate", ActiveUser.id));
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@dateUpdate", DateTime.Now));
                        insertHeaderCommand.Parameters.Add(new SqlParameter("@id", tempAccountingFreight.id));
                        insertHeaderCommand.Transaction = trans.UnderlyingTransaction;

                        // 2. Insertar los detalles
                        foreach (var detalle in tempAccountingFreight.AccountingFreightDetails)
                        {


                            var insertDetailCommand = db.Database.Connection.CreateCommand();

                            if (detalle.id_userUpdate != null)
                            {
                                var updateDetailCommand = db.Database.Connection.CreateCommand();
                                updateDetailCommand.CommandText = "UPDATE AccountingFreightDetails SET " +
                                    "accountingAccountCode = @accountingAccountCode, " +
                                    "isAuxiliar = @isAuxiliar, " +
                                    "code_Auxiliar = @code_Auxiliar, " +
                                    "idAuxContable = @idAuxContable, " +
                                    "id_costCenter = @id_costCenter, " +
                                    "id_subCostCenter = @id_subCostCenter, " +
                                    "accountType = @accountType, " +
                                    "id_userUpdate = @id_userUpdate, " +
                                    "dateUpdate = @dateUpdate, " +
                                    "isActive = @isActive " +
                                    "WHERE id = @id"; // Supongo que hay un campo llamado id que identifica de forma única cada detalle

                                // Parámetros para la actualización
                                updateDetailCommand.Parameters.Add(new SqlParameter("@id", detalle.id)); // Asegúrate de cambiar esto al nombre del campo id real
                                updateDetailCommand.Parameters.Add(new SqlParameter("@accountingAccountCode", detalle.accountingAccountCode));
                                updateDetailCommand.Parameters.Add(new SqlParameter("@isAuxiliar", detalle.isAuxiliar));
                                updateDetailCommand.Parameters.Add(new SqlParameter("@code_Auxiliar", detalle.code_Auxiliar));
                                updateDetailCommand.Parameters.Add(new SqlParameter("@idAuxContable", detalle.idAuxContable));
                                if (detalle.id_costCenter != null)
                                {
                                    updateDetailCommand.Parameters.Add(new SqlParameter("@id_costCenter", detalle.id_costCenter));
                                }
                                else
                                {
                                    updateDetailCommand.Parameters.Add(new SqlParameter("@id_costCenter", DBNull.Value));
                                }
                                if (detalle.id_subCostCenter != null)
                                {
                                    updateDetailCommand.Parameters.Add(new SqlParameter("@id_subCostCenter", detalle.id_subCostCenter));
                                }
                                else
                                {
                                    updateDetailCommand.Parameters.Add(new SqlParameter("@id_subCostCenter", DBNull.Value));
                                }
                                updateDetailCommand.Parameters.Add(new SqlParameter("@accountType", detalle.accountType));
                                updateDetailCommand.Parameters.Add(new SqlParameter("@id_userUpdate", ActiveUser.id));
                                updateDetailCommand.Parameters.Add(new SqlParameter("@dateUpdate", DateTime.Now));
                                updateDetailCommand.Parameters.Add(new SqlParameter("@isActive", detalle.isActive));

                                // Asociar la transacción
                                updateDetailCommand.Transaction = trans.UnderlyingTransaction;

                                // Ejecutar la actualización
                                updateDetailCommand.ExecuteNonQuery();
                            }
                            else
                            {
                                insertDetailCommand.CommandText = "INSERT INTO AccountingFreightDetails\r\n" +
                               "(id_accountingFreight,accountingAccountCode,isAuxiliar,code_Auxiliar,idAuxContable,id_costCenter,\r\n" +
                               "id_subCostCenter,accountType,id_userCreate,dateCreate,isActive)\r\n" +
                               "VALUES (@id_accountingFreight,@accountingAccountCode,@isAuxiliar,@code_Auxiliar,@idAuxContable,@id_costCenter,\r\n" +
                               "@id_subCostCenter,@accountType,@id_userCreate,@dateCreate,@isActive)";
                                insertDetailCommand.Parameters.Add(new SqlParameter("@id_accountingFreight", tempAccountingFreight.id));
                                insertDetailCommand.Parameters.Add(new SqlParameter("@accountingAccountCode", detalle.accountingAccountCode));
                                insertDetailCommand.Parameters.Add(new SqlParameter("@isAuxiliar", detalle.isAuxiliar));
                                insertDetailCommand.Parameters.Add(new SqlParameter("@code_Auxiliar", detalle.code_Auxiliar));
                                insertDetailCommand.Parameters.Add(new SqlParameter("@idAuxContable", detalle.idAuxContable));
                                if (detalle.id_costCenter != null)
                                {
                                    insertDetailCommand.Parameters.Add(new SqlParameter("@id_costCenter", detalle.id_costCenter));
                                }
                                else
                                {
                                    insertDetailCommand.Parameters.Add(new SqlParameter("@id_costCenter", DBNull.Value));
                                }
                                if (detalle.id_subCostCenter != null)
                                {
                                    insertDetailCommand.Parameters.Add(new SqlParameter("@id_subCostCenter", detalle.id_subCostCenter));
                                }
                                else
                                {
                                    insertDetailCommand.Parameters.Add(new SqlParameter("@id_subCostCenter", DBNull.Value));
                                }
                                insertDetailCommand.Parameters.Add(new SqlParameter("@accountType", detalle.accountType));
                                insertDetailCommand.Parameters.Add(new SqlParameter("@id_userCreate", detalle.id_userCreate));
                                insertDetailCommand.Parameters.Add(new SqlParameter("@dateCreate", detalle.dateCreate));
                                insertDetailCommand.Parameters.Add(new SqlParameter("@isActive", detalle.isActive));
                                insertDetailCommand.Transaction = trans.UnderlyingTransaction;
                                insertDetailCommand.ExecuteNonQuery();
                            }
                           
                            if(tempAccountingFreightDeleted != null)
                            {
                                foreach (var deteled in tempAccountingFreightDeleted)
                                {
                                    var deletedDetailCommand = db.Database.Connection.CreateCommand();
                                    deletedDetailCommand.CommandText = "UPDATE AccountingFreightDetails SET id_userUpdate = @id_userUpdate, \r\n" +
                                        "dateUpdate = @dateUpdate, isActive = @isActive  WHERE id = @id";
                                    deletedDetailCommand.Parameters.Add(new SqlParameter("@id_userUpdate", ActiveUser.id));
                                    deletedDetailCommand.Parameters.Add(new SqlParameter("@dateUpdate", DateTime.Now));
                                    deletedDetailCommand.Parameters.Add(new SqlParameter("@isActive", false));
                                    deletedDetailCommand.Parameters.Add(new SqlParameter("@id", deteled.id));
                                    deletedDetailCommand.Transaction = trans.UnderlyingTransaction;
                                    deletedDetailCommand.ExecuteNonQuery();
                                }
                            }
                            
                        }

                        // Commit la transacción
                        trans.Commit();
                        isValid = true;
                        message = "Plantilla guardada con exito";

                    }
                    catch (Exception ex)
                    {
                        // En caso de error, deshacer la transacción
                        TempData.Keep("AccountingFreight");
                        trans.Rollback();
                        isValid = false;
                        message = ex.Message;
                        MetodosEscrituraLogs.EscribeExcepcionLogNest(ex, rutaLog, "AccountingFreight", "Produccion");
                        // Aquí maneja el error según tus necesidades (p. ej., log, mensaje al usuario, etc.)
                    }
                }


            }
            catch (ProdHandlerException e)
            {
                TempData.Keep("AccountingFreight");
                ViewData["ErrorMessage"] = $"{(e.Message ?? "")}";
                isValid = false;
                message = e.Message;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "AccountingFreight", "Produccion");
            }
            catch (Exception e)
            {
                TempData.Keep("AccountingFreight");
                ViewData["ErrorMessage"] = GenericError.ErrorGeneral;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, rutaLog, "AccountingFreight", "Produccion");
                message = "Error al crear elemento: " + e.Message;
                isValid = false;
            }

            var result = new
            {
                id = tempAccountingFreight.id,
                message,
                isValid,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
