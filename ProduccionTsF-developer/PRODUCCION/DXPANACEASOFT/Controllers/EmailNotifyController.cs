using DXPANACEASOFT.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System;
using System.Web.Mvc;
using Utilitarios.ProdException;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using EntidadesAuxiliares.General;
using System.Data.Common;
using Z.Dapper.Plus;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace DXPANACEASOFT.Controllers
{

    [Authorize]
    public class EmailNotifyController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region EmailNotify GRIDVIEW
        [ValidateInput(false)]
        public ActionResult EmailNotifyDocumentTypePartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = EmailNotifyDocumentType.GetOneById((keyToCopy??0)); 
                //db.EmailNotifyDocumentType.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = EmailNotifyDocumentType.GetAllByCompanyId(this.ActiveCompanyId);  
            //db.EmailNotifyDocumentType.Where(igc => igc.id_company == this.ActiveCompanyId);
            return PartialView("_EmailNotifyDocumentTypePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EmailNotifyDocumentTypePartialAddNew(EmailNotifyDocumentType item)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.id == item.id_DocumentType);
            DbConnection conexion = db.Database.Connection;
            conexion.Open();
            //if (ModelState.IsValid)
            //{
                using (var trans = conexion.BeginTransaction())
                {
                    try
                    {

                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                        item.id_DocumentType = item.id_DocumentType;
                        //
                        #region ItemGroupEmailNotifyDocumentType

                        List<int> persons = (TempData["persons"] as List<int>);
                        persons = persons ?? new List<int>();
                    
                        item.EmailNotifyDocumentTypePersons = new List<EmailNotifyDocumentTypePerson>();

                        foreach (var i in persons)
                        {
                            item.EmailNotifyDocumentTypePersons.Add(new EmailNotifyDocumentTypePerson
                            {
                                id_PersonReceiver = i,
                                //PersonReceiver = db.Person.FirstOrDefault(e => e.id == i),
                                id_EmailNotifyDocumentType = item.id,
                                //EmailNotifyDocumentType = item
                            });
                        }
                        #endregion
                        //trans.BulkInsert<EmailNotifyDocumentType>(/*new DapperPlusContext(conexion),*/  new EmailNotifyDocumentType[] { item }, r => r.EmailNotifyDocumentTypePersons);
                        trans.BulkInsert(new EmailNotifyDocumentType[] { item })
                            .ThenForEach(x => x.EmailNotifyDocumentTypePersons.ForEach(y => y.id_EmailNotifyDocumentType= x.id))
                            .ThenBulkInsert(x => x.EmailNotifyDocumentTypePersons);
                        

                        //db.EmailNotifyDocumentType.Add(item);
                        //db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage($"Notificación via Email para el Proceso: {documentType?.name} guardado exitosamente");
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
                        FullLog(e);
                    }
                    finally
                    {
                        conexion.Close();
                    }
                }

            //}
            //else
            //{
            //    ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
            //}

            var model = EmailNotifyDocumentType.GetAllByCompanyId(this.ActiveCompanyId);
            //db.EmailNotifyDocumentType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_EmailNotifyDocumentTypePartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EmailNotifyDocumentTypePartialUpdate(EmailNotifyDocumentType item)
        {

            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.id == item.id_DocumentType);
            DbConnection conexion = db.Database.Connection;
            conexion.Open();

            
            //if (ModelState.IsValid)
            //{
                using (var trans = conexion.BeginTransaction())
                {
                    try
                    {
                        var modelItem = EmailNotifyDocumentType.GetOneById(item.id);  
                        //db.EmailNotifyDocumentType.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.isActive = item.isActive;
                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            #region ItemGroupEmailNotifyDocumentType

                            modelItem.EmailNotifyDocumentTypePersons = EmailNotifyDocumentTypePerson.GetAllByEmailNotifyId(item.id)?.ToList();
                            trans.BulkDelete(modelItem.EmailNotifyDocumentTypePersons);

                            for (int i = modelItem.EmailNotifyDocumentTypePersons.Count - 1; i >= 0; i--)
                            {
                                var detail = modelItem.EmailNotifyDocumentTypePersons.ElementAt(i);

                                modelItem.EmailNotifyDocumentTypePersons.Remove(detail);
                                //db.Entry(detail).State = EntityState.Deleted;
                            }

                            List<int> persons = (TempData["persons"] as List<int>);
                            persons = persons ?? new List<int>();

                            foreach (var i in persons)
                            {
                                modelItem.EmailNotifyDocumentTypePersons.Add(new EmailNotifyDocumentTypePerson
                                {
                                    id_PersonReceiver = i,
                                    //PersonReceiver= db.Person.FirstOrDefault(e => e.id == i),
                                    id_EmailNotifyDocumentType = modelItem.id,
                                     
                                    //EmailNotifyDocumentType = modelItem
                                });
                            }
                        #endregion

                        trans.BulkUpdate(modelItem)
                                 //.ThenForEach(x => x.EmailNotifyDocumentTypePersons.ForEach(y => y.id_EmailNotifyDocumentType = x.id))
                                 .ThenBulkInsert(x => x.EmailNotifyDocumentTypePersons);
                            //db.EmailNotifyDocumentType.Attach(modelItem);
                            //db.Entry(modelItem).State = EntityState.Modified;
                            //db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage($"Notificación via Email para el Proceso: {documentType?.name} guardado exitosamente");
                        }
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
                        FullLog(e);
                    }
                }

           //}
           //else
           //{
           //    ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
           //}

            var model = EmailNotifyDocumentType.GetAllByCompanyId(this.ActiveCompanyId);  
            //db.EmailNotifyDocumentType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_EmailNotifyDocumentTypePartial", model.ToList());

        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult EmailNotifyDocumentTypePartialDelete(System.Int32 id)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.id == id);
            DbConnection conexion = db.Database.Connection;
            conexion.Open();

            if (id >= 0)
            {
                using (var trans = conexion.BeginTransaction())
                {
                    try
                    {
                        var item = EmailNotifyDocumentType.GetOneById(id); 
                        //db.EmailNotifyDocumentType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            //db.EmailNotifyDocumentType.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                            //db.SaveChanges();

                            trans.BulkUpdate(new EmailNotifyDocumentType[] { item });
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage($"Notificación via Email para el Proceso: {documentType?.name} desactivada exitosamente");
                        }
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
                        FullLog(e);
                    }
                }

            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
            }

            var model = EmailNotifyDocumentType.GetAllByCompanyId(this.ActiveCompanyId);  
            //db.EmailNotifyDocumentType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_EmailNotifyDocumentTypePartial", model.ToList());

        }

        [HttpPost]
        public ActionResult DeleteSelectedEmailNotifyDocumentType(int[] ids)
        {
            DbConnection conexion = db.Database.Connection;
            conexion.Open();
            if (ids != null && ids.Length > 0)
            {
                using (var trans = conexion.BeginTransaction())
                {
                    try
                    {
                        var modelItem = EmailNotifyDocumentType.GetAllByContaisIds(ids);  
                        //db.EmailNotifyDocumentType.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            //db.EmailNotifyDocumentType.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                        }
                        //db.SaveChanges();
                        trans.BulkUpdate(modelItem);
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage($"Notificaciones via Email desactivada exitosamente");
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
                        FullLog(e);
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage(GenericError.ErrorGeneral);
            }

            var model = EmailNotifyDocumentType.GetAllByCompanyId(this.ActiveCompanyId);  
            //db.EmailNotifyDocumentType.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_EmailNotifyDocumentTypePartial", model.ToList());
        }

        
        #endregion

        #region  Detalle
        [HttpPost, ValidateInput(false)]
        public JsonResult GetEmailNotifyDocumentTypePersons(int? id_emailNotifyDocumentType)
        {
            var emailNotifyPerson = EmailNotifyDocumentTypePerson.GetAllByEmailNotifyId((id_emailNotifyDocumentType ?? 0));
                                                //db.ItemGroupItemGroupCategory.Where(w => w.id_itemGroupCategory == id_itemGroupCategory);
            List<int> persons = new List<int>();
            string list_personsStr = "";
            foreach (var igigc in emailNotifyPerson)
            {

                if (list_personsStr == "") list_personsStr = igigc.id_PersonReceiver.ToString();
                else list_personsStr += "," + igigc.id_PersonReceiver.ToString();
                persons.Add(igigc.id_PersonReceiver);
            }
            var result = new
            {
                persons = list_personsStr

            };

            TempData["persons"] = persons;
            TempData.Keep("persons");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdatePersons(int[] personsCurrent)
        {
            List<int> persons = new List<int>();
            if ((personsCurrent?.Length ?? 0) > 0)
            {
                //itemGroups = itemGroups ?? int[];
                foreach (var i in personsCurrent)
                {
                    if (i != 0)
                    {
                        persons.Add(i);
                        //if (list_idInventaryLineFilterStr == "") list_idInventaryLineFilterStr = i.ToString();
                        //else list_idInventaryLineFilterStr += "," + i.ToString();
                    }
                }

            }

            var result = new
            {
                Message = "Ok"
            };

            TempData["persons"] = persons;

            TempData.Keep("persons");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Tipo Documento
        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateExistEmailDocumentType(int id_DocumentType, int emailNotifyDocumentTypeId)
        {
            int[] ooo = TempData["persons"] as int[];

            TempData.Keep("persons");

            var configEmailDocumentTypeList = EmailNotifyDocumentType.GetAllByCompanyId(this.ActiveCompanyId);
            int countExist = configEmailDocumentTypeList
                                .Where(r=> r.id_DocumentType == id_DocumentType && r.id != emailNotifyDocumentTypeId)
                                .Count();

            var result = new
            {
                exists = (countExist>0)?"S":"N" 
            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

    }

}