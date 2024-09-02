using DXPANACEASOFT.Models;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Utilitarios.CorreoElectronico;
using Utilitarios.Encriptacion;

namespace DXPANACEASOFT.Services
{
	public class ServiceRemissionGuideControlVehicle
	{
		public static void Envio_Correos_Logistica_Salida(DBContext db, int id_remissionGuide, int remissionGuideForControlVehicleId)
		{
			try
			{

				string respuestaCorreo = Envio_Correos_Logistica_Salida_Vehiculos(db, id_remissionGuide);

				if (respuestaCorreo == "OK")
				{
					DBContext db3 = new DBContext();
					RemissionGuideControlVehicle rgcvSETmp = null;
					using (DbContextTransaction trans = db3.Database.BeginTransaction())
					{
						try
						{
							rgcvSETmp = db3.RemissionGuideControlVehicle.FirstOrDefault(fod => fod.id_remissionGuide == remissionGuideForControlVehicleId);
							if (rgcvSETmp != null)
							{
								rgcvSETmp.hasSentEmail = true;
								db3.RemissionGuideControlVehicle.Attach(rgcvSETmp);
								db3.Entry(rgcvSETmp).State = EntityState.Modified;
								db3.SaveChanges();
								trans.Commit();
							}
						}
						catch
						{
							// Log
						}
					}
					db3.Dispose();
				}
			}
			catch
			{
				// Log
			}
		}


		private static string Envio_Correos_Logistica_Salida_Vehiculos(DBContext db, int id_remissionGuide)
		{
			string mensajeRespuesta = string.Empty;
			DBContext db2 = new DBContext();
			try
			{
				string cuerpoMensaje = string.Empty;
				string asuntoMensaje = string.Empty;
				var remissionGuide = db2.RemissionGuide.FirstOrDefault(fod => fod.id == id_remissionGuide);
				var remissionGuideControlVehicle = db2.RemissionGuideControlVehicle.FirstOrDefault(fod => fod.id_remissionGuide == id_remissionGuide);

				string mailFrom = ConfigurationManager.AppSettings["correoDefaultDesde"];
				string passwordMailFrom = ConfigurationManager.AppSettings["contrasenaCorreoDefault"];
				string passwordMailFromUncrypted = clsEncriptacion1.LeadnjirSimple.Desencriptar(passwordMailFrom);
				string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
				string puertoHost = ConfigurationManager.AppSettings["puertoHost"];
				string mensajePrueba = ConfigurationManager.AppSettings["Pruebas"];

				int puertoHostInt = int.Parse(puertoHost);

				if (remissionGuide != null)
				{
					//string TextoPrueba = "PRUEBAS";
					int id_buyer = (int)remissionGuide.id_buyer;
					int id_provider = (int)remissionGuide.id_providerRemisionGuide;
					int id_receiver = (int)remissionGuide.id_reciver;
					int id_driver = (int)remissionGuide?.RemissionGuideTransportation?.id_driver;

					string destanationMails = string.Empty;

					string mailBuyer = db2.Person.FirstOrDefault(fod => fod.id == id_buyer)?.email;
					string mailProvider = db2.Person.FirstOrDefault(fod => fod.id == id_provider)?.email;
					string nameProvider = db2.Person.FirstOrDefault(fod => fod.id == id_provider)?.fullname_businessName;
					string recieverName = db2.Person.FirstOrDefault(fod => fod.id == id_receiver)?.fullname_businessName ?? "SIN RECIBIDOR";

					//Inserto mail Comprador
					if (!(string.IsNullOrEmpty(mailProvider)))
					{
						destanationMails = destanationMails + ";" + mailBuyer;
					}
					//Listo de correos de prueba
					var mailListLogisticsExit = db.SettingDetail.Where(w => w.Setting.code == "CPESVGR").ToList();

					if (mailListLogisticsExit != null && mailListLogisticsExit.Count > 0)
					{
						foreach (var i in mailListLogisticsExit)
						{
							if (i != null)
							{
								destanationMails = destanationMails + ";" + i.value;
							}
						}
					}

					if (!(string.IsNullOrEmpty(destanationMails) && destanationMails.Length > 0))
					{
						destanationMails = destanationMails.Substring(1, (destanationMails.Length - 1));
					}

					string carRegistration = remissionGuide?.RemissionGuideTransportation?.carRegistration ?? "SIN PLACA";
					string carMark = remissionGuide?.RemissionGuideTransportation?.Vehicle?.mark ?? "SIN MARCA";
					string carModel = remissionGuide?.RemissionGuideTransportation?.Vehicle?.model ?? "SIN MODELO";

					string dateExit = remissionGuideControlVehicle.exitDateProductionBuilding.Value.ToString("dd/MM/yyyy");
					string hoursTimeExit = remissionGuideControlVehicle.exitTimeProductionBuilding.Value.Hours.ToString();
					string minutesTimeExit = remissionGuideControlVehicle.exitTimeProductionBuilding.Value.Minutes.ToString();

					string codeProductionUnitProvider = remissionGuide?.ProductionUnitProvider?.code ?? "SIN CODIGO";
					string nameProducctionUnitProvider = remissionGuide?.ProductionUnitProvider?.name ?? "SIN NOMBRE";
					string addressProductionUnitProvider = remissionGuide?.ProductionUnitProvider?.address ?? "SIN DIRECCION";

					string site = remissionGuide?.ProductionUnitProvider?.FishingSite?.name ?? "SIN SITIO";
					string zone = remissionGuide?.ProductionUnitProvider?.FishingSite?.FishingZone?.name ?? "SIN ZONA";

					string driverName = db2.Person.FirstOrDefault(fod => fod.id == id_driver)?.fullname_businessName ?? "SIN CONDUCTOR";
					string cellPhoneDriver = db2.Person.FirstOrDefault(fod => fod.id == id_driver)?.cellPhoneNumberPerson ?? "";
					string description = remissionGuide?.Document?.description ?? "";

					cuerpoMensaje = string.Empty;
					cuerpoMensaje = string.Concat("<b>Transporte: Placa: </b>", carRegistration, " <b>Marca: </b>", carMark, " <b>Modelo: </b>", carModel);
					cuerpoMensaje = cuerpoMensaje + "<br />";
					cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Fecha Salida: </b>", dateExit, " <b> Hora Salida: </b>", hoursTimeExit, ":", minutesTimeExit);
					cuerpoMensaje = cuerpoMensaje + "<br />";
					cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Nombre Proveedor: </b> ", nameProvider, "<b> Camaronera: </b>", nameProducctionUnitProvider);
					cuerpoMensaje = cuerpoMensaje + "<br />";
					cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Zona: </b>", zone, "<b> Sitio: </b>", site);
					cuerpoMensaje = cuerpoMensaje + "<br />";
					cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Recibe: </b>", recieverName);
					cuerpoMensaje = cuerpoMensaje + "<br />";
					cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Chofer: </b>", driverName, "<b> Celular: </b>", cellPhoneDriver);
					cuerpoMensaje = cuerpoMensaje + "<br />";
					cuerpoMensaje = cuerpoMensaje + string.Concat("<b>Descripción: </b>", description);

					if (mensajePrueba == "SI")
					{
						asuntoMensaje = "PRUEBAS ";
					}
					asuntoMensaje += string.Concat("Salida de Vehículo, Guia de Remisión: ", remissionGuide.Document.number);

					mensajeRespuesta = clsCorreoElectronico
											.EnviarCorreoElectronico(destanationMails,
																		mailFrom,
																		asuntoMensaje,
																		smtpHost,
																		puertoHostInt,
																		passwordMailFromUncrypted,
																		cuerpoMensaje, ';');

				}

			}
			catch (Exception ex)
			{
				Console.Write("Error: " + ex.Message);
			}
			db2.Dispose();
			return mensajeRespuesta;
		}
	}
}