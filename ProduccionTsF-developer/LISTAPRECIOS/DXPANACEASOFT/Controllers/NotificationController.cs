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