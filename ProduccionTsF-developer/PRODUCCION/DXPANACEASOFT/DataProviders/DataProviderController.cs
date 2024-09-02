using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderController
    {
        private static DBContext db = null;

        public static TController Controller(int? id)
        {
            db = new DBContext();
            return db.TController.FirstOrDefault(i => i.id == id);
        }

        public static List<TController> Controllers()
        {
            db = new DBContext();
            return db.TController.ToList();
        }

        public static Menu GetControllerByName(int? id_company, int? id_Controller)
        {
            db = new DBContext();
            var menuControllers = db.Menu
                .FirstOrDefault(e => e.isActive && e.id_company == id_company && e.id_controller == id_Controller);           

            return menuControllers;
        }

        public static IEnumerable GetControllersByName(int? id_company)
        {
            db = new DBContext();

            var controllers = db.Menu.Where(e => e.isActive && e.id_company == id_company && e.id_controller != null)
                .Select(s => new { s.title, s.id_controller}).ToList();

            return controllers;
        }
    }
}