using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Extensions.Functions
{
    public static class DBNotification
    {
        static SettingNotification settingNotification;
        static Document document;
        static IEnumerable<User> users;

        public static void Excecute(DBContext _db)
        {
            foreach (var entry in _db.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified))
            {
                var tableName = ObjectContext.GetObjectType(entry.Entity.GetType()).Name;
                if (!tableName.Equals("Document"))
                    continue;

                var id_documentType = entry.CurrentValues.GetValue<int>("id_documentType");
                var id_documentState = entry.CurrentValues.GetValue<int>("id_documentState");

                settingNotification = _db.SettingNotification
                                            .FirstOrDefault(n => n.id_documentType == id_documentType &&
                                                                n.id_documentState == id_documentState);
                if (settingNotification == null)
                    return;

                var id = entry.CurrentValues.GetValue<int>("id");
                document = _db.Document.FirstOrDefault(c => c.id == id);
                users = GetUsers();

                if (document?.DocumentType == null)
                    return;

                var th1 = new Thread(new ThreadStart(CreateNotificarion));
                th1.Start();

                if (!settingNotification.sendByMail)
                    continue;

                var th2 = new Thread(new ThreadStart(SendByMail));
                th2.Start();
            }
        }

        private static void SendByMail()
        {
            var emailSubject = VariableToString(settingNotification.title);
            var emailBody = VariableToString(settingNotification.description);
            var listEmailAdress = settingNotification.addressesMails.Split(';').ToList();

            listEmailAdress.AddRange(users.Select(user => user.Employee.Person.email));

            Services.ServiceSendMail.SendEmail(emailSubject, emailBody, listEmailAdress);
        }

        private static void CreateNotificarion()
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var notifications = users.Select(user => new Notification
                        {
                            id_user = user.id,
                            id_document = document.id,
                            noDocument = GetDocumentName() ?? document.number,
                            id_documentType = document.id_documentType,
                            documentType = document.DocumentType.name,
                            id_documentState = document.id_documentState,
                            documentState = document.DocumentState.name,
                            title = settingNotification.title,
                            description = settingNotification.description,
                            dateTime = DateTime.Now,
                            reading = false
                        }).ToList();

                        db.Notification.AddRange(notifications);
                        //db.SaveChanges(false);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                    }
                }
            }
        }

        private static IEnumerable<User> GetUsers()
        {
            var users = new List<User>();
            if (document.DocumentType.code.Equals("18")) //Lista de Precio
            {
                users.Add(document.PriceList.User);
            }
            //else if(){
            //TODO: todos los demas tipos de comprobantes
            //}

            return users;
        }

        private static string GetDocumentName()
        {
            if (document.DocumentType.code.Equals("18")) //Lista de Precio
            {
                return document.PriceList.name;
            }
            //else if(){
            //TODO: todos los demas tipos de comprobantes
            //}

            return null;
        }

        private static string VariableToString(string text)
        {
            
            var _estado = "#estado";
            var _fecha = "#fecha";
            var _hora = "#hora";
            var _saltolinea = "#saltolinea";    

            text = text.Replace(_estado, document.DocumentState.name);
            text = text.Replace(_fecha, DateTime.Now.ToString("dd/MM/yyyy"));
            text = text.Replace(_hora, DateTime.Now.ToString("hh:mm:ss tt"));
            text = text.Replace(_saltolinea, "<br>");

            return text;
        }
    }
}