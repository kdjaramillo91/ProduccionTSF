using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    public class SettingNotificationController : DefaultController
    {
        // GET: SettingNotification
        public ActionResult Index()
        {
            return View(db.SettingNotification.ToList());
        }

        [ValidateInput(false)]
        public ActionResult GridViewSettingNotification()
        {
            return PartialView("_GridView", db.SettingNotification.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateSettingNotification(SettingNotification element)
        {
            if (ModelState.IsValid)
            {
                var settingNotification = db.SettingNotification.FirstOrDefault(s => s.id == element.id);
                if (settingNotification == null)
                {
                    settingNotification = new SettingNotification
                    {
                        id_documentType = element.id_documentType,
                        id_documentState = element.id_documentState,
                        title = element.title,
                        description = element.description,
                        sendByMail = element.sendByMail,
                        addressesMails = element.addressesMails
                    };

                    db.SettingNotification.Add(settingNotification);
                    db.Entry(settingNotification).State = EntityState.Added;
                }
                else
                {
                    settingNotification.id_documentType = element.id_documentType;
                    settingNotification.id_documentState = element.id_documentState;
                    settingNotification.title = element.title;
                    settingNotification.description = element.description;
                    settingNotification.sendByMail = element.sendByMail;
                    settingNotification.addressesMails = element.addressesMails;

                    db.SettingNotification.Attach(settingNotification);
                    db.Entry(settingNotification).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return GridViewSettingNotification();
        }

        private void DeleteSettingNotificationDb(int id)
        {
            var settingNotification = db.SettingNotification.FirstOrDefault(s => s.id == id);
            if (settingNotification == null)
            {
                ViewData["EditError"] = "Please, correct all errors.";
            }
            else
            {
                db.SettingNotification.Remove(settingNotification);
                db.Entry(settingNotification).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteSettingNotification(int id)
        {
            if (id <= 0)
            {
                ViewData["EditError"] = "Please, correct all errors.";
                return GridViewSettingNotification();
            }

            DeleteSettingNotificationDb(id);

            return GridViewSettingNotification();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteNotifications(int[] ids)
        {
            foreach (var id in ids)
            {
                DeleteSettingNotificationDb(id);
            }

            return GridViewSettingNotification();
        }
    }
}