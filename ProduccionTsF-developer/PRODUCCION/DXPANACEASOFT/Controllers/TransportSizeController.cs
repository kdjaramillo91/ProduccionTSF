using DevExpress.CodeParser;
using DXPANACEASOFT.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using DXPANACEASOFT.Models.Dto;
using System.Data;
using DXPANACEASOFT.Dapper;
using DevExpress.Utils;
using DXPANACEASOFT.Models.PLDTO;
using DXPANACEASOFT.Extensions;
using System.Windows.Media.TextFormatting;
using DXPANACEASOFT.Models.DTOModel;

namespace DXPANACEASOFT.Controllers
{

    [Authorize]
    public class TransportSizeController : DefaultController
    {
        const string INSERT_TRANSPORTSIZE = "INSERT INTO TransportSize (code,name,description,id_poundsRange,id_iceBagRange,isActive, " +
                                            "id_company,id_userCreate,dateCreate,id_userUpdate,dateUpdate,id_transportTariffType, " +
                                            "binRangeMinimum,binRangeMaximun ) values " +
                                            "(@code,@name,@description,@id_poundsRange,@id_iceBagRange,@isActive, " +
                                            "@id_company, @id_userCreate, @dateCreate, @id_userUpdate, @dateUpdate, @id_transportTariffType, " +
                                            "@binRangeMinimum,@binRangeMaximun ); SELECT SCOPE_IDENTITY()";

        const string SELECT_TRANSPORTSIZE = "SELECT id, code,name,description,id_poundsRange,id_iceBagRange,isActive, " +
                                            " id_company,id_userCreate,dateCreate,id_userUpdate,dateUpdate,id_transportTariffType, " +
                                            " binRangeMinimum,binRangeMaximun " +
                                            " FROM TransportSize " ;

        const string UPDATE_TRANSPORTSIZE = "UPDATE TransportSize " +
                                            "SET code = @code, name=@name, " +
                                            "id_poundsRange=@id_poundsRange,id_iceBagRange=@id_iceBagRange,isActive=@isActive, " +
                                            "id_userUpdate=@id_userUpdate,dateUpdate=@dateUpdate,id_transportTariffType=@id_transportTariffType, "+
                                            "binRangeMinimum=@binRangeMinimum,binRangeMaximun=@binRangeMaximun " +
                                            "WHERE id=@id";
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region TransportSize GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportSizePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                //ViewData["rowToCopy"] =  db.TransportSize.FirstOrDefault(b => b.id == keyToCopy);
                ViewData["rowToCopy"] = getTransportSize(transportSizeId: keyToCopy)?.FirstOrDefault().toDTO(db);
            }
            //var model = db.TransportSize.Where(whl => whl.id_company == this.ActiveCompanyId);
            var model = getTransportSize(companyId: this.ActiveCompanyId).Select(r=> r.toDTO(db)).ToArray();
            return PartialView("_TransportSizePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportSizePartialAddNew(TransportSize item)
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


                    //db.TransportSize.Add(item);
                    //db.SaveChanges();
                    //trans.Commit();

                    saveDb<TransportSize>(item, null, insertTransportSize);

                    ViewData["EditMessage"] = SuccessMessage("Tamano de Transporte: " + item.name + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    //trans.Rollback();
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                }
                //}

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage("Modelo No es Válido");
            }

            var model = getTransportSize(companyId: this.ActiveCompanyId).Select(r => r.toDTO(db)).ToArray();
            return PartialView("_TransportSizePartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult TransportSizePartialUpdate(TransportSize item)
        {
            if (ModelState.IsValid)
            {
                //using (DbContextTransaction trans = db.Database.BeginTransaction())
                //{
                    try
                    {
                        //var modelItem =db.TransportSize.FirstOrDefault(it => it.id == item.id);
                        var modelItem = getTransportSize(transportSizeId: item.id)?.FirstOrDefault();
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
                            modelItem.id_iceBagRange = item.id_iceBagRange;
                            modelItem.id_poundsRange = item.id_poundsRange;
                            modelItem.id_transportTariffType = item.id_transportTariffType;
                            modelItem.binRangeMinimum = item.binRangeMinimum;
                            modelItem.binRangeMaximun = item.binRangeMaximun;
                            modelItem.isActive = item.isActive;
                            modelItem.id_company = ActiveCompany.id;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                        saveDb<TransportSize>(modelItem, actionDB: updateTransportSize);
                        //UPDATE_TRANSPORTSIZE
                        //db.TransportSize.Attach(modelItem);
                        //db.Entry(modelItem).State = EntityState.Modified;

                        //db.SaveChanges();
                        //trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tamano de Transporte: " + item.name + " guardada exitosamente");
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

            //var model = db.TransportSize.Where(o => o.id_company == this.ActiveCompanyId);
            var model = getTransportSize(companyId: this.ActiveCompanyId).Select(r => r.toDTO(db)).ToArray();
            return PartialView("_TransportSizePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransportSizePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                //using (DbContextTransaction trans = db.Database.BeginTransaction())
                //{
                    try
                    {
                        //var item = db.TransportSize.FirstOrDefault(it => it.id == id);
                        var item = getTransportSize(transportSizeId: id)?.FirstOrDefault();
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            //db.TransportSize.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;

                            //db.SaveChanges();
                            //trans.Commit();
                            saveDb<TransportSize>(item, updateTransportSize);
                            ViewData["EditMessage"] = SuccessMessage("Tamano de Transporte: " + (item?.name ?? "") + " desactivada exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        //trans.Rollback();
                    }
                //}
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = getTransportSize(companyId: this.ActiveCompanyId).Select(r => r.toDTO(db)).ToArray();
            return PartialView("_TransportSizePartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedTransportSize(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                //using (DbContextTransaction trans = db.Database.BeginTransaction())
                //{
                try
                {
                    //var modelItem = db.TransportSize.Where(i => ids.Contains(i.id));
                    var modelItem = getTransportSize(ids: ids);
                    foreach (var item in modelItem)
                    {
                        item.isActive = false;

                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        //db.TransportSize.Attach(item);
                        //db.Entry(item).State = EntityState.Modified;
                    }
                    saveDb<TransportSize[]>(modelItem, actionDB: updateTransportSizes);
                    //db.SaveChanges();
                    //trans.Commit();
                    ViewData["EditMessage"] = SuccessMessage("Tamanos de Transporte desactivadas exitosamente");
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

            //var model = db.TransportSize.Where(o => o.id_company == this.ActiveCompanyId);
            var model = getTransportSize(companyId: this.ActiveCompanyId).Select(r => r.toDTO(db)).ToArray();
            return PartialView("_TransportSizePartial", model.ToList());
        }
        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public JsonResult ImportFileTransportSize()
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
                        int id_poundsRange = 0;
                        int id_iceBagRange = 0;
                        int id_transportTariffType = 0;
                        int? binRangeMinimum = null;
                        int? binRangeMaximun = null;

                        //using (DbContextTransaction trans = db.Database.BeginTransaction())
                        //{
                        //try
                        //{
                        for (int i = 2; i < table.Rows.Count; i++)
                                {
                                    Excel.Range row = table.Rows[i]; // FILA i
                                    try
                                    {
                                        code = row.Cells[1].Text;        // COLUMNA 1
                                        name = row.Cells[2].Text;
                                        id_poundsRange = row.Cells[3].Text;
                                        id_iceBagRange = row.Cells[4].Text;
                                        id_transportTariffType = row.Cells[5].Text;
                                        binRangeMinimum = row.Cells[6].Text;
                                        binRangeMaximun = row.Cells[7].Text;

                                    }
                                    catch (Exception)
                                    {
                                        errorMessages.Add($"Error en formato de datos fila: {i}.");
                                    }

                                    TransportSize transportSizeImport = getTransportSize(code: code)?.FirstOrDefault();
                                    //db.TransportSize.FirstOrDefault(l => l.code.Equals(code));

                                    if (transportSizeImport == null)
                                    {
                                        transportSizeImport = new TransportSize
                                        {
                                            code = code,
                                            name = name,
                                            description = name,
                                            id_iceBagRange = id_iceBagRange,
                                            id_poundsRange = id_poundsRange,
                                            id_transportTariffType = id_transportTariffType,
                                            binRangeMaximun = binRangeMaximun,
                                            binRangeMinimum=binRangeMinimum,
                                            isActive = true,
                                            id_company = this.ActiveCompanyId,
                                            id_userCreate = ActiveUser.id,
                                            dateCreate = DateTime.Now,
                                            id_userUpdate = ActiveUser.id,
                                            dateUpdate = DateTime.Now
                                        };
                                        transportSizeImport.id = saveDb<TransportSize>(transportSizeImport,functionDB: insertTransportSize);
                                        //db.TransportSize.Add(transportSizeImport);
                                    }
                                    else
                                    {
                                        transportSizeImport.code = code;
                                        transportSizeImport.name = name;
                                        transportSizeImport.description = name;
                                        transportSizeImport.id_iceBagRange = id_iceBagRange;
                                        transportSizeImport.id_poundsRange = id_poundsRange;
                                        transportSizeImport.binRangeMaximun= binRangeMaximun;
                                        transportSizeImport.binRangeMinimum= binRangeMinimum;
                                        transportSizeImport.id_transportTariffType= id_transportTariffType;
                                        transportSizeImport.id_userUpdate = ActiveUser.id;
                                        transportSizeImport.dateUpdate = DateTime.Now;

                                        saveDb<TransportSize>(transportSizeImport, updateTransportSize);
                                         //db.TransportSize.Attach(transportSizeImport);
                                         //        db.Entry(transportSizeImport).State = EntityState.Modified;
                                    }
                                }

                               //db.SaveChanges();
                               //trans.Commit();
                           //}
                           //catch (Exception)
                           //{
                           //    trans.Rollback();
                           //}
                        //}
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
        public JsonResult ValidateCodeTransportSize(int id_TransportSize, string code)
        {
            var transportSize = getTransportSize(companyId: this.ActiveCompanyId)?.FirstOrDefault(r => r.code == code
                                                                                                        && r.id != id_TransportSize);
            //TransportSize transportSize = db.TransportSize.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
            //                                                                && b.code == code
            //                                                                && b.id != id_TransportSize);

            if (transportSize == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tamano de Transporte" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region ADO Method
        private int insertTransportSize(TransportSize transportSize, SqlConnection db, SqlTransaction tr)
        {
            int currentUserId = this.ActiveUserId;
            int currentCompanyId = this.ActiveCompanyId;
            int newJobId = 0;
            using (var command = new SqlCommand())
            {
                var parameters = new DynamicParameters();

                command.Parameters.Add("@code", SqlDbType.VarChar).Value = transportSize.code;
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = transportSize.name;
                command.Parameters.Add("@description", SqlDbType.VarChar).Value = transportSize.name;
                command.Parameters.Add("@id_poundsRange", SqlDbType.Int).Value = transportSize.id_poundsRange;
                command.Parameters.Add("@id_iceBagRange", SqlDbType.Int).Value = transportSize.id_iceBagRange;
                command.Parameters.Add("@isActive", SqlDbType.Bit).Value = 1;
                command.Parameters.Add("@id_company", SqlDbType.Int).Value = currentCompanyId;
                command.Parameters.Add("@id_userCreate", SqlDbType.Int).Value = transportSize.id_userCreate;
                command.Parameters.Add("@dateCreate", SqlDbType.DateTime).Value = transportSize.dateCreate;
                command.Parameters.Add("@id_userUpdate", SqlDbType.Int).Value = transportSize.id_userUpdate;
                command.Parameters.Add("@dateUpdate", SqlDbType.DateTime).Value = transportSize.dateUpdate;
                command.Parameters.Add("@id_transportTariffType", SqlDbType.Int).Value = transportSize.id_transportTariffType;
                command.Parameters.Add("@binRangeMinimum", SqlDbType.Int) .Value = (transportSize.binRangeMinimum?? 0);
                command.Parameters.Add("@binRangeMaximun", SqlDbType.Int).Value = (transportSize.binRangeMaximun ?? 0);

                string textCommand = INSERT_TRANSPORTSIZE;

                command.CommandText = textCommand;
                command.CommandType = CommandType.Text;
                command.Connection = db;
                command.Transaction = tr;

                object returnObj = command.ExecuteScalar();
                if (returnObj != null)
                {
                    int.TryParse(returnObj.ToString(), out newJobId);
                }

            }
            return newJobId;
        }
        private void updateTransportSize(TransportSize transportSize, SqlConnection db, SqlTransaction tr)
        {
            using (var command = new SqlCommand())
            {
                var parameters = new DynamicParameters();

                command.Parameters.Add("@code", SqlDbType.VarChar).Value = transportSize.code;
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = transportSize.name;
                command.Parameters.Add("@description", SqlDbType.VarChar).Value = transportSize.name;
                command.Parameters.Add("@id_poundsRange", SqlDbType.Int).Value = transportSize.id_poundsRange;
                command.Parameters.Add("@id_iceBagRange", SqlDbType.Int).Value = transportSize.id_iceBagRange;
                command.Parameters.Add("@isActive", SqlDbType.Bit).Value = transportSize.isActive;
                command.Parameters.Add("@id_userUpdate", SqlDbType.Int).Value = transportSize.id_userUpdate;
                command.Parameters.Add("@dateUpdate", SqlDbType.DateTime).Value = transportSize.dateUpdate;
                command.Parameters.Add("@id_transportTariffType", SqlDbType.Int).Value = transportSize.id_transportTariffType;
                command.Parameters.Add("@binRangeMinimum", SqlDbType.Int).Value = (transportSize.binRangeMinimum ?? 0);
                command.Parameters.Add("@binRangeMaximun", SqlDbType.Int).Value = (transportSize.binRangeMaximun ?? 0);

                command.Parameters.Add("@id", SqlDbType.Int).Value = transportSize.id;
                string textCommand = UPDATE_TRANSPORTSIZE;

                command.CommandText = textCommand;
                command.CommandType = CommandType.Text;
                command.Connection = db;
                command.Transaction = tr;

                command.ExecuteNonQuery();

            }
        }

        private void updateTransportSizes(TransportSize[] transportSizes, SqlConnection db, SqlTransaction tr)
        {
            foreach (var transportSize in transportSizes)
            {
                updateTransportSize(transportSize, db, tr);
            }
        }

        private TransportSize[] getTransportSize(int? transportSizeId = null, int? companyId = null, int[] ids= null,string code= null)
        {
            TransportSize[] transportSizeList = null;
            string predicado = "";
            if (transportSizeId.HasValue)
            {
                predicado = $" WHERE ID={transportSizeId.Value}";
            }
            else if (companyId.HasValue)
            {
                predicado = $" WHERE id_company={companyId.Value}";
            }
            else if ((ids?.Length??0)>0)
            {
                predicado = $" WHERE id in({ids.Select(r=> r.ToString()).Aggregate((i,j) => $"{i},{j}")}); ";
            }
            else if (!string.IsNullOrEmpty(code)  && !string.IsNullOrWhiteSpace(code))
            {
                predicado = $" WHERE code={code}";
            }

            using (var db = DapperConnection.Connection())
            {
                db.Open();

                string m_sql = $"{SELECT_TRANSPORTSIZE}{predicado}; ";
                try
                {
                    transportSizeList = db.Query<TransportSize>(m_sql).ToArray();
                }
                finally
                {
                    db.Close();
                }

            }
            return transportSizeList;
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

        #region ComboBox
        public ActionResult ComboBoxPoundsRangeEdit(int companyId, int id_poundsRange)
        {
            //BuildComboBoxReasonEdit("ComboBoxPoundRangeEdit");
            TransportSizeDto transportSize = new TransportSizeDto { id_company= companyId, id_poundsRange = id_poundsRange };

            return PartialView("CmdBoxesEdit/_ComboBoxPoundRangeEdit", transportSize);
        }
        #endregion
    }


}