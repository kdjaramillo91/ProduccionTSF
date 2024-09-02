using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading;

namespace DXPANACEASOFT.Extensions.Functions
{
	public static class DBNotification
	{
		public static void Execute(DBContext db)
		{
			foreach (var entry in db.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified))
			{
				var tableName = ObjectContext.GetObjectType(entry.Entity.GetType()).Name;
				if (!tableName.Equals("Document"))
					continue;

				var id_documentType = entry.CurrentValues.GetValue<int>("id_documentType");
				var id_documentStateCurrent = entry.CurrentValues.GetValue<int>("id_documentState");
				var id_documentStateOrigen = entry.OriginalValues.GetValue<int>("id_documentState");

				if (id_documentStateCurrent == id_documentStateOrigen)
					continue;

				var id = entry.CurrentValues.GetValue<int>("id");
				var document = db.Document.Include(d => d.DocumentState)
										  .Include(d => d.DocumentType)
										  .FirstOrDefault(c => c.id == id);
				if (document?.DocumentType == null)
					continue;

				var users = GetUsers(db, document);

				var settingNotificationList = db.SettingNotification.Where(n => n.id_documentType == id_documentType &&
																				n.id_documentState == id_documentStateCurrent).ToList();
				foreach (var sNotification in settingNotificationList)
				{
					var th1 = new Thread(() => CreateNotificarion(document, sNotification, users));
					th1.Start();

					if (!sNotification.sendByMail)
						continue;

					var th2 = new Thread(() => SendByMail(document, sNotification, users));
					th2.Start();
				}
			}
		}

		private static IEnumerable<User> GetUsers(DBContext db, Document document)
		{
			var users = new List<User>();
			if (document.DocumentType.code.Equals("18")) //Lista de Precio
			{
				var priceList = db.PriceList.FirstOrDefault(p => p.id == document.id);
				if (priceList != null)
					users.Add(priceList.User);
			}
			//else if(){
			//TODO: todos los demas tipos de comprobantes
			//}

			return users;
		}

		private static void SendByMail(Document document,
									   SettingNotification settingNotification,
									   IEnumerable<User> users)
		{
			var emailSubject = VariableToString(settingNotification.title, document);
			var emailBody = VariableToString(settingNotification.description, document);
			var listEmailAdress = !string.IsNullOrEmpty(settingNotification.addressesMails)
								  ? settingNotification.addressesMails.Split(';').ToList()
								  : new List<string>();

			using (var db = new DBContext())
			{
				foreach (var user in users)
				{
					var employee = db.Employee.Include(e => e.Person).FirstOrDefault(e => e.id == user.id_employee);
					listEmailAdress.Add(employee.Person.email);
				}
			}

			Services.ServiceSendMail.SendEmail(emailSubject, emailBody, listEmailAdress);
		}

		private static void CreateNotificarion(Document document,
											   SettingNotification settingNotification,
											   IEnumerable<User> users)
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
							noDocument = GetDocumentName(document) ?? document.number,
							id_documentType = document.id_documentType,
							documentType = document.DocumentType.name,
							id_documentState = document.id_documentState,
							documentState = document.DocumentState.name,
							title = VariableToString(settingNotification.title, document),
							description = VariableToString(settingNotification.description, document),
							dateTime = DateTime.Now,
							reading = false
						}).ToList();

						db.Notification.AddRange(notifications);
						db.SaveChanges(false);
						trans.Commit();
					}
					catch (Exception)
					{
						trans.Rollback();
					}
				}
			}
		}

		private static string GetDocumentName(Document document)
		{
			using (var db = new DBContext())
			{
				if (document.DocumentType.code.Equals("18")) //Lista de Precio
				{
					return db.PriceList.FirstOrDefault(p => p.id == document.id)?.name ?? "";
				}
				//else if(){
				//TODO: todos los demas tipos de comprobantes
				//}

				return null;
			}
		}

		private static string VariableToString(string text, Document document)
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