using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using System.Data;
using DXPANACEASOFT.Dapper;
//using DocumentFormat.OpenXml.Office2010.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class FishingCustodianController : DefaultController
    {

        const string INSERT_FISHINGCUSTODIAN = "INSERT INTO FishingCustodian (code,patrol,semiComplete,truckDriver,changeHG,cabinHR, " +
                                            "id_FishingSite ,isActive,id_company,id_userCreate,dateCreate,id_userUpdate, "+
                                            "dateUpdate) values " +
                                            "(@code,@patrol,@semiComplete,@truckDriver,@changeHG,@cabinHR, " +
                                            "@id_FishingSite ,@isActive,@id_company,@id_userCreate,@dateCreate,@id_userUpdate, " +
                                            "@dateUpdate ); SELECT SCOPE_IDENTITY()";

        const string SELECT_FISHINGCUSTODIAN = "SELECT id, code,patrol,semiComplete,truckDriver,changeHG,cabinHR,  " +
                                            " id_FishingSite ,isActive,id_company,id_userCreate,dateCreate,id_userUpdate,  " +
                                            " dateUpdate " +
                                            " FROM FishingCustodian ";

        const string UPDATE_FISHINGCUSTODIAN = "UPDATE FishingCustodian " +
                                            "SET code = @code, patrol=@patrol, " +
                                            "semiComplete=@semiComplete,truckDriver=@truckDriver,changeHG=@changeHG, " +
                                            "cabinHR=@cabinHR, id_FishingSite= @id_FishingSite, " +
                                            "id_userUpdate=@id_userUpdate, dateUpdate=@dateUpdate, isActive= @isActive " +
                                            "WHERE id=@id";


        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region FishingCustodian GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingCustodianPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = getFishingCustodian(fishingCustodianId: keyToCopy)?.FirstOrDefault();
            }
            var model = getFishingCustodian(companyId: this.ActiveCompanyId);
            return PartialView("_FishingCustodianPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingCustodianPartialAddNew(FishingCustodian item)
        {
            if (ModelState.IsValid)
            {
                //using (DbContextTransaction trans = db.Database.BeginTransaction())
                //{
                    try
                    {
                        item.id_company = this.ActiveCompanyId;

                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                    //db.FishingCustodian.Add(item);
                    //db.SaveChanges();
                    //trans.Commit();
                    saveDb<FishingCustodian>(item, null, insertFishingCustodian);
                    ViewData["EditMessage"] = SuccessMessage("Tarifario Custodio: guardado exitosamente");
                    }
                    catch (Exception)
                    {
                        //trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                //}

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = getFishingCustodian(companyId: this.ActiveCompanyId);
            return PartialView("_FishingCustodianPartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult FishingCustodianPartialUpdate(FishingCustodian item)
        {
            if (ModelState.IsValid)
            {
                //using (DbContextTransaction trans = db.Database.BeginTransaction())
                //{
                    try
                    {
                        var modelItem = getFishingCustodian(fishingCustodianId: item.id)?.FirstOrDefault();
                        if (modelItem != null)
                        {
                            modelItem.code = item.code;
                            modelItem.patrol = item.patrol;
                            modelItem.semiComplete = item.semiComplete;
                            modelItem.truckDriver = item.truckDriver;
                            modelItem.changeHG = item.changeHG;
                            modelItem.cabinHR = item.cabinHR;
                            modelItem.id_FishingSite= item.id_FishingSite;

                            modelItem.isActive = item.isActive;
                            modelItem.id_company = ActiveCompany.id;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            saveDb<FishingCustodian>(modelItem, actionDB: updateFishingCustodian);
                        //db.FishingCustodian.Attach(modelItem);
                        //db.Entry(modelItem).State = EntityState.Modified;
                        //db.SaveChanges();
                        //trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tarifario de Custodios: guardado exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        //trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                //}
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = getFishingCustodian(companyId: this.ActiveCompanyId);
            return PartialView("_FishingCustodianPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FishingCustodianPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                //using (DbContextTransaction trans = db.Database.BeginTransaction())
                //{
                    try
                    {
                        var item = getFishingCustodian(fishingCustodianId: id)?.FirstOrDefault();
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                            
                            saveDb<FishingCustodian>(item, updateFishingCustodian);

                            //db.FishingCustodian.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                            //
                            //db.SaveChanges();
                            //trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Tarifario Custodios: desactivado exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                       // trans.Rollback();
                    }
               // }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = getFishingCustodian(companyId: this.ActiveCompanyId);
            return PartialView("_FishingCustodianPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedFishingCustodian(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                //using (DbContextTransaction trans = db.Database.BeginTransaction())
                //{
                    try
                    {
                        var modelItem = getFishingCustodian(ids: ids);
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            //db.FishingCustodian.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                        }
                    saveDb<FishingCustodian[]>(modelItem, actionDB: updateFishingCustodians);
                    //db.SaveChanges();
                    //trans.Commit();
                    ViewData["EditMessage"] = SuccessMessage("Tarifario Custodios: desactivados exitosamente");
                    }
                    catch (Exception)
                    {
                        //trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                //}
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = getFishingCustodian(companyId: this.ActiveCompanyId);
            return PartialView("_FishingCustodianPartial", model.ToList());
        }
        #endregion

        #region REPORTS

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileFishingCustodian()
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

                        string code = "";
                        decimal patrol = 0;
                        decimal semiComplete = 0;
                        decimal truckDriver = 0;
                        decimal changeHG = 0;
                        decimal cabinHR = 0;
                        int id_FishingSite = 0;


                        using (DbContextTransaction trans = db.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 2; i < table.Rows.Count; i++)
                                {
                                    Excel.Range row = table.Rows[i]; // FILA i
                                    try
                                    {
                                        code = row.Cells[1].Text;
                                        patrol = decimal.Parse(row.Cells[2].Text);        // COLUMNA 1
                                        semiComplete = decimal.Parse(row.Cells[3].Text);
                                        truckDriver = decimal.Parse(row.Cells[4].Text);
                                        changeHG = decimal.Parse(row.Cells[5].Text);
                                        cabinHR = decimal.Parse(row.Cells[6].Text);
                                        id_FishingSite = int.Parse(row.Cells[7].Text);

                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    FishingCustodian fishingCustodian = getFishingCustodian(code: code).FirstOrDefault();

                                    if (fishingCustodian == null)
                                    {
                                        fishingCustodian = new FishingCustodian
                                        {
                                            code = code,
                                            patrol = patrol,
                                            semiComplete = semiComplete,
                                            truckDriver = truckDriver,
                                            changeHG = changeHG,
                                            cabinHR = cabinHR,
                                            id_FishingSite = id_FishingSite,
                                            isActive = true,
                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };
                                        fishingCustodian.id = saveDb<FishingCustodian>(fishingCustodian, functionDB: insertFishingCustodian);
                                        //db.FishingCustodian.Add(fishingCustodian);
                                    }
                                    else
                                    {
                                        fishingCustodian.code = code;                                        
                                        fishingCustodian.patrol = patrol;
                                        fishingCustodian.semiComplete = semiComplete;
                                        fishingCustodian.truckDriver = truckDriver;
                                        fishingCustodian.changeHG = changeHG;
                                        fishingCustodian.cabinHR = cabinHR;
                                        fishingCustodian.id_FishingSite = id_FishingSite;
                                        fishingCustodian.id_userUpdate = ActiveUser.id;
                                        fishingCustodian.dateUpdate = DateTime.Now;

                                        //db.FishingCustodian.Attach(FishingCustodian);
                                        //db.Entry(FishingCustodian).State = EntityState.Modified;
                                        saveDb<FishingCustodian>(fishingCustodian, updateFishingCustodian);
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
        public JsonResult ValidateCodeFishingCustodian(int id_FishingCustodian, string code)
        {
            FishingCustodian FishingCustodian = getFishingCustodian(companyId: this.ActiveCompanyId)?.FirstOrDefault(   b => b.code == code
                                                                                                                        && b.id != id_FishingCustodian);
    
            if (FishingCustodian == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tarifario Custodio" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region ADO Method
        private int insertFishingCustodian(FishingCustodian fishingCustodian, SqlConnection db, SqlTransaction tr)
        {
            int currentUserId = this.ActiveUserId;
            int currentCompanyId = this.ActiveCompanyId;
            int newJobId = db.ExecuteScalar<int>(INSERT_FISHINGCUSTODIAN, new 
            {
                code = fishingCustodian.code,
                patrol= fishingCustodian.patrol,
                semiComplete = fishingCustodian.semiComplete,
                truckDriver = fishingCustodian.truckDriver,
                changeHG = fishingCustodian.changeHG,
                cabinHR = fishingCustodian.cabinHR,
                id_FishingSite = fishingCustodian.id_FishingSite,
                isActive = fishingCustodian.isActive,
                id_company = currentCompanyId,
                id_userCreate = fishingCustodian.id_userCreate,
                dateCreate = fishingCustodian.dateCreate,
                id_userUpdate = fishingCustodian.id_userUpdate,
                dateUpdate = fishingCustodian.dateUpdate
            },tr);
            //using (var command = new SqlCommand())
            //{
            //    var parameters = new DynamicParameters();
            //
            //    command.Parameters.Add("@code", SqlDbType.VarChar).Value = fishingCustodian.code;
            //    command.Parameters.Add("@patrol", SqlDbType.Decimal).Value = fishingCustodian.patrol;
            //    command.Parameters.Add("@semiComplete", SqlDbType.Decimal).Value = fishingCustodian.semiComplete;
            //    command.Parameters.Add("@truckDriver", SqlDbType.Decimal).Value = fishingCustodian.truckDriver;
            //    command.Parameters.Add("@changeHG", SqlDbType.Decimal).Value = fishingCustodian.changeHG;
            //    command.Parameters.Add("@cabinHR", SqlDbType.Decimal).Value = fishingCustodian.cabinHR;
            //    command.Parameters.Add("@id_FishingSite", SqlDbType.Int).Value = fishingCustodian.id_FishingSite;
            //    command.Parameters.Add("@isActive", SqlDbType.Bit).Value = 1;
            //    command.Parameters.Add("@id_company", SqlDbType.Int).Value = currentCompanyId;
            //    command.Parameters.Add("@id_userCreate", SqlDbType.Int).Value = fishingCustodian.id_userCreate;
            //    command.Parameters.Add("@dateCreate", SqlDbType.DateTime).Value = fishingCustodian.dateCreate;
            //    command.Parameters.Add("@id_userUpdate", SqlDbType.Int).Value = fishingCustodian.id_userUpdate;
            //    command.Parameters.Add("@dateUpdate", SqlDbType.DateTime).Value = fishingCustodian.dateUpdate;
            //    
            //
            //    string textCommand = INSERT_FISHINGCUSTODIAN;
            //
            //    command.CommandText = textCommand;
            //    command.CommandType = CommandType.Text;
            //    command.Connection = db;
            //    command.Transaction = tr;
            //
            //    object returnObj = command.ExecuteScalar();
            //    if (returnObj != null)
            //    {
            //        int.TryParse(returnObj.ToString(), out newJobId);
            //    }
            //
            //}
            return newJobId;
        }
        private void updateFishingCustodian(FishingCustodian fishingCustodian, SqlConnection db, SqlTransaction tr)
        {
            db.Execute(UPDATE_FISHINGCUSTODIAN, new
            {
                code = fishingCustodian.code,
                patrol = fishingCustodian.patrol,
                semiComplete = fishingCustodian.semiComplete,
                truckDriver = fishingCustodian.truckDriver,
                changeHG = fishingCustodian.changeHG,
                cabinHR= fishingCustodian.cabinHR,
                id_FishingSite = fishingCustodian.id_FishingSite,
                isActive = fishingCustodian.isActive,
                id_userUpdate = fishingCustodian.id_userUpdate,
                dateUpdate = fishingCustodian.dateUpdate,
                id = fishingCustodian.id
            }, tr);
            //using (var command = new SqlCommand())
            //{
            //    var parameters = new DynamicParameters();
            //
            //    command.Parameters.Add("@code", SqlDbType.VarChar).Value = fishingCustodian.code;
            //    command.Parameters.Add("@patrol", SqlDbType.Decimal).Value = fishingCustodian.patrol;
            //    command.Parameters.Add("@semiComplete", SqlDbType.Decimal).Value = fishingCustodian.semiComplete;
            //    command.Parameters.Add("@truckDriver", SqlDbType.Decimal).Value = fishingCustodian.truckDriver;
            //    command.Parameters.Add("@changeHG", SqlDbType.Decimal).Value = fishingCustodian.changeHG;
            //    command.Parameters.Add("@cabinHR", SqlDbType.Decimal).Value = fishingCustodian.cabinHR;
            //    command.Parameters.Add("@id_FishingSite", SqlDbType.Int).Value = fishingCustodian.id_FishingSite;
            //    command.Parameters.Add("@isActive", SqlDbType.Bit).Value = fishingCustodian.isActive;
            //    command.Parameters.Add("@id_userUpdate", SqlDbType.Int).Value = fishingCustodian.id_userUpdate;
            //    command.Parameters.Add("@dateUpdate", SqlDbType.DateTime).Value = fishingCustodian.dateUpdate;
            //    
            //
            //    command.Parameters.Add("@id", SqlDbType.Int).Value = fishingCustodian.id;
            //    string textCommand = UPDATE_FISHINGCUSTODIAN;
            //
            //    command.CommandText = textCommand;
            //    command.CommandType = CommandType.Text;
            //    command.Connection = db;
            //    command.Transaction = tr;
            //
            //    command.ExecuteNonQuery();
            //
            //}
        }

        private void updateFishingCustodians(FishingCustodian[] fishingCustodians, SqlConnection db, SqlTransaction tr)
        {
            foreach (var fishingCustodian in fishingCustodians)
            {
                updateFishingCustodian(fishingCustodian, db, tr);
            }
        }

        private FishingCustodian[] getFishingCustodian(int? fishingCustodianId = null, int? companyId = null, int[] ids = null, string code = null)
        {
            FishingCustodian[] fishingCustodianList = null;
            string predicado = "";
            if (fishingCustodianId.HasValue)
            {
                predicado = $" WHERE ID={fishingCustodianId.Value}";
            }
            else if (companyId.HasValue)
            {
                predicado = $" WHERE id_company={companyId.Value}";
            }
            else if ((ids?.Length ?? 0) > 0)
            {
                predicado = $" WHERE id in({ids.Select(r => r.ToString()).Aggregate((i, j) => $"{i},{j}")}); ";
            }
            else if (!string.IsNullOrEmpty(code) && !string.IsNullOrWhiteSpace(code))
            {
                predicado = $" WHERE code={code}";
            }

            using (var db1 = DapperConnection.Connection())
            {
                db1.Open();

                string m_sql = $"{SELECT_FISHINGCUSTODIAN}{predicado}; ";
                try
                {
                    fishingCustodianList = db1.Query<FishingCustodian>(m_sql).ToArray();
                }
                finally
                {
                    db1.Close();
                }

                int[] fishingSiteIds = fishingCustodianList.Select(r=> r.id_FishingSite).ToArray();
                var fishingSite = db.FishingSite.Include("FishingZone").Where(r => fishingSiteIds.Contains(r.id)).ToArray();
                if ((fishingSite?.Length ?? 0) > 0)
                {
                    fishingCustodianList = (from custodian in fishingCustodianList
                                            join site in fishingSite
                                            on custodian.id_FishingSite equals site.id
                                            select new FishingCustodian
                                            {
                                                FishingSite = site,
                                                id = custodian.id,
                                                dateCreate = custodian.dateCreate,
                                                cabinHR = (custodian.cabinHR ?? 0),
                                                changeHG = (custodian.changeHG ?? 0),
                                                code = custodian.code,
                                                dateUpdate = custodian.dateUpdate,
                                                id_company = custodian.id_company,
                                                id_FishingSite = custodian.id_FishingSite,
                                                id_userCreate = custodian.id_userCreate,
                                                id_userUpdate = custodian.id_userUpdate,
                                                isActive = custodian.isActive,
                                                patrol = (custodian.patrol ?? 0),
                                                semiComplete = (custodian.semiComplete??0),
                                                truckDriver = (custodian.truckDriver??0)
                                            }).ToArray();
                }

            }
            return fishingCustodianList;
        }

        private int saveDb<T>(T dataToSave, Action<T, SqlConnection, SqlTransaction> actionDB = null, Func<T, SqlConnection, SqlTransaction, int> functionDB = null)
        {
            string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
            int newId = 0;
            using (var db = new System.Data.SqlClient.SqlConnection(dapperDBContext))
            {
                try
                {
                    db.Open();
                    using (var tr = db.BeginTransaction())
                    {
                        try
                        {
                            if (actionDB != null)
                            {
                                actionDB(dataToSave, db, tr);
                            }
                            if (functionDB != null)
                            {
                                newId = functionDB(dataToSave, db, tr);
                            }

                            tr.Commit();
                        }
                        catch (Exception e)
                        {
                            tr.Rollback();
                            throw;
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
            return newId;
        }
        #endregion

    }
}



