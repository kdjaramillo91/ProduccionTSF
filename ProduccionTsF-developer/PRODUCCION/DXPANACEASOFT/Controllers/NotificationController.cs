using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.CodeParser;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    public class NotificationController : DefaultController
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View(db.Notification.ToList());
        }

        [ValidateInput(false)]
        public ActionResult GridViewNotification()
        {
            return PartialView("_GridView", db.Notification.ToList());
        }

        [HttpPost]
        public ActionResult UpdateNotification(int id, bool reading)
        {
            var notification = db.Notification.FirstOrDefault(s => s.id == id);
            if (notification == null)
            {
                ViewData["EditError"] = "Please, correct all errors.";
            }
            else
            {
                notification.reading = reading;

                db.Notification.Attach(notification);
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
            }

            return GridViewNotification();
        }

        private void DeleteNotificationDb(int id)
        {
            var notification = db.Notification.FirstOrDefault(s => s.id == id);
            if (notification == null)
            {
                ViewData["EditError"] = "Please, correct all errors.";
            }
            else
            {
                db.Notification.Remove(notification);
                db.Entry(notification).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RefreshNotification(int id)
        {
            return PartialView("Notifications/_LayoutNotificationPartial");
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateNotificationUnRead(string documentTypeCode)
        {
            int documentTypeId = db.DocumentType.FirstOrDefault(r => r.code == documentTypeCode).id;
            var notificactions = db.Notification.Where(r => r.id_documentType == documentTypeId && r.id_user == this.ActiveUserId && !r.reading).ToList();
            int countNotification = (notificactions?.Count()??0);
            var lastNotification = notificactions?.OrderByDescending(r => r.dateTime)?.FirstOrDefault();
            string stateLastNotification = (lastNotification?.documentState??"");
            string nameLastProcess = (lastNotification?.title ?? "");

            var result = new
            {
                countNot = countNotification,
                stateNotification = stateLastNotification,
                nameProcess = nameLastProcess
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateNotificationAsRead(string documentTypeCode)
        {
            bool isOK = false;
            try
            {
                int documentTypeId = db.DocumentType.FirstOrDefault(r => r.code == documentTypeCode).id;
                var notificationList = db.Notification.Where(s => s.id_documentType == documentTypeId && s.id_user == this.ActiveUserId).ToArray();
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var notification in notificationList)
                        {
                            notification.reading = true;

                            db.Notification.Attach(notification);
                            db.Entry(notification).State = EntityState.Modified;

                        }
                        db.SaveChanges();
                        trans.Commit();
                        isOK = true;

                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        isOK = false;
                    }
                }
            }
            catch (Exception ex)
            {

                isOK = false;
            }

            var result = new
            {
                ok = isOK
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteNotification(int id)
        {
            if (id <= 0)
            {
                ViewData["EditError"] = "Please, correct all errors.";
                return GridViewNotification();
            }

            DeleteNotificationDb(id);

            return GridViewNotification();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteNotifications(int[] ids)
        {
            foreach (var id in ids)
            {
                DeleteNotificationDb(id);
            }

            return GridViewNotification();
        }
    }
}