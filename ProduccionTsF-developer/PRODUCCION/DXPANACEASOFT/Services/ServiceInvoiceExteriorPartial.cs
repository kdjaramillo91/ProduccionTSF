using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using DXPANACEASOFT.Models;
using System.IO;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace DXPANACEASOFT.Services
{

	public class ListAuxDividedDetail : List<AuxDividedDetail>
	{

	}
	public class AuxDividedDetail
	{

		public int Id_Item { get; set; }
		public int NumNormal { get; set; }
		public int Numcajas { get; set; }

		public int Residual { get; set; }


	}

	public class ServiceInvoiceExteriorPartial
	{
        const int LOGON_TYPE_NEW_CREDENTIALS = 9;
        const int LOGON32_PROVIDER_WINNT50 = 3;

        private static int previewNumberNotResidual(int numberToDivide, int topNumber)
		{
			int returnInt = 0;
			int ressidual = 0;
			returnInt = topNumber;

			while (ressidual != 0)
			{
				ressidual = numberToDivide / returnInt;
				returnInt--;
			}
			return returnInt;
		}

		public static string generateInvoicePartial(DBContext db, int id_Invoice)
		{

			string returnCallIdentity = null;

			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{

				try
				{

					Guid tGuid = Guid.NewGuid();
					returnCallIdentity = tGuid.ToString();
					Invoice invoiceToPartial = db.Invoice.FirstOrDefault(r => r.id == id_Invoice);
					// Numero de contenedores
					ListAuxDividedDetail auxDividedDetails = new ListAuxDividedDetail();
					int numeroContenedores = invoiceToPartial.InvoiceExterior.numeroContenedores;



					int residuo = 0;
					int entero = 0;
					//int residual = 0;
					int normal = 0;



					invoiceToPartial
							.InvoiceDetail
							.ToList()
							.ForEach(r =>
							{

								residuo = (int)r.numBoxes % numeroContenedores;
								normal = numeroContenedores - 1;
								entero = ((int)r.numBoxes / numeroContenedores);


								auxDividedDetails.Add(new AuxDividedDetail
								{
									Id_Item = r.id_item,
									NumNormal = normal,
									Numcajas = entero,
									Residual = residuo
								});

							});




					for (int i = 1; i <= numeroContenedores; i++)
					{

						Invoice invoice = new Invoice
						{

							Document = new Document
							{
								number = invoiceToPartial.Document.number /* + "/" + i.ToString()*/,
							},
						};

						invoice.InvoiceDetail = new List<InvoiceDetail>();

						List<InvoiceDetail> _invoiceDetail = invoiceToPartial
																	.InvoiceDetail
																	.Where(r => r.isActive)
																	.ToList();

						foreach (InvoiceDetail invoiceDetailPartial in _invoiceDetail)
						{

							int numBoxesLine = 0;
							AuxDividedDetail divDetail = auxDividedDetails.FirstOrDefault(r => r.Id_Item == invoiceDetailPartial.id_item);
							if (divDetail.NumNormal == 1)
							{

								numBoxesLine = divDetail.Numcajas + divDetail.Residual;
							}
							else
							{
								numBoxesLine = divDetail.Numcajas;
							}

							divDetail.NumNormal--;

							InvoiceDetail invoiceDetail = new InvoiceDetail();

							invoiceDetail.id = invoiceDetailPartial.id;
							invoiceDetail.id_item = invoiceDetailPartial.id_item;
							invoiceDetail.Item = invoiceDetailPartial.Item;
							invoiceDetail.id_metricUnit = invoiceDetailPartial.id_metricUnit;
							invoiceDetail.id_metricUnitInvoiceDetail = invoiceDetailPartial.id_metricUnitInvoiceDetail;
							invoiceDetail.id_amountInvoice = invoiceDetailPartial.id_amountInvoice;
							invoiceDetail.presentationMaximum = invoiceDetailPartial.presentationMaximum;
							invoiceDetail.presentationMinimum = invoiceDetailPartial.presentationMinimum;
							invoiceDetail.numBoxes = numBoxesLine; //(invoiceDetailPartial.numBoxes / numeroContenedores);
							invoiceDetail.unitPrice = invoiceDetailPartial.unitPrice;
							invoiceDetail.discount = invoiceDetailPartial.discount / numeroContenedores;
							invoiceDetail.isActive = true;
							invoiceDetail.Calculation();
							invoice.InvoiceDetail.Add(invoiceDetail);
						}


						invoice.calculateTotales();
						invoice.InvoiceExterior = new InvoiceExterior
						{
							id = invoiceToPartial.id,
							valueCustomsExpenditures = invoiceToPartial.InvoiceExterior.valueCustomsExpenditures / numeroContenedores,
							valueInternationalFreight = invoiceToPartial.InvoiceExterior.valueInternationalFreight / numeroContenedores,
							valueInternationalInsurance = invoiceToPartial.InvoiceExterior.valueInternationalInsurance / numeroContenedores,
							valueTransportationExpenses = invoiceToPartial.InvoiceExterior.valueTransportationExpenses / numeroContenedores,
						};
						invoice.calculateTotalesInvoiceExterior();
						invoice.calculateTotalBoxes();
						invoice.saveWeight(db);


						InvoiceExteriorDivided invoiceExteriorDivided
														= new InvoiceExteriorDivided
														{
															callIdentity = returnCallIdentity,
															secuencie = i,
															id_invoice = invoiceToPartial.id,
															numberDocumentDivided = invoice.Document.number,
															subtTotal0 = invoice.subTotalIVA0,
															subtTotalFob = invoice.InvoiceExterior.valueTotalFOB,
															subtTotalIncoTerm = invoice.InvoiceExterior.valuetotalCIF,
															subtTotalWithOutTax = invoice.subtotalNoTaxes,
															valueCustomsExpenditures = invoice.InvoiceExterior.valueCustomsExpenditures,
															valueInternationalFreight = invoice.InvoiceExterior.valueInternationalFreight,
															valueInternationalInsurance = invoice.InvoiceExterior.valueInternationalInsurance,
															valueTransportationExpenses = invoice.InvoiceExterior.valueTransportationExpenses,

														};

						invoiceExteriorDivided.InvoiceExteriorDetailDivided = new List<InvoiceExteriorDetailDivided>();

						foreach (InvoiceDetail invoiceDetail in invoice.InvoiceDetail.ToList())
						{

							InvoiceExteriorDetailDivided invoiceExteriorDetailDivided
															= new InvoiceExteriorDetailDivided
															{
																amount = (decimal)invoiceDetail.id_amountInvoice,
																discount = invoiceDetail.discount,
																id_InvoiceDetail = invoiceDetail.id,
																numBoxes = (int)invoiceDetail.numBoxes,
																total = invoiceDetail.total

															};

							invoiceExteriorDivided.InvoiceExteriorDetailDivided.Add(invoiceExteriorDetailDivided);

						}

						invoiceExteriorDivided.InvoiceExteriorWeightDivided = new List<InvoiceExteriorWeightDivided>();
						foreach (InvoiceExteriorWeight invoiceExteriorWeight in invoice.InvoiceExteriorWeight.ToList())
						{

							InvoiceExteriorWeightDivided invoiceExteriorWeightDivided
															= new InvoiceExteriorWeightDivided
															{
																id_metricUnit = invoiceExteriorWeight.id_metricUnit,
																id_WeightType = invoiceExteriorWeight.id_WeightType,
																peso = invoiceExteriorWeight.peso

															};
							invoiceExteriorDivided.InvoiceExteriorWeightDivided.Add(invoiceExteriorWeightDivided);
						}

						db.InvoiceExteriorDivided.Add(invoiceExteriorDivided);

					}

					db.SaveChanges();
					trans.Commit();

				}
				catch //(Exception e)
				{


				}

			}


			return returnCallIdentity;

		}


		public static async Task CallXML(DBContext db,
										  List<Invoice> invoiceExteriorList,
										  int idCompany,
										  string routePath,
										  int delayInSeconds)
		{

			await Task.Run(() => generateInvoiceXML(db,
													invoiceExteriorList,
													routePath,
													idCompany,
													delayInSeconds
													));

		}
		public static async Task generateInvoiceXML(DBContext db,
													 List<Invoice> invoiceExteriorList,
													 string routePath,
													 int idCompany,
													 int delayInSeconds
												   )
		{

			XmlDocument xmlFEX = new XmlDocument();
			await Task.Factory.StartNew(() =>
			{
				foreach (Invoice _invoiceExterior in invoiceExteriorList)
				{
					Task.Delay(delayInSeconds).Wait();
					xmlFEX = _invoiceExterior.GenerateXML(idCompany);
					string pathFileXmlFileName = routePath + "\\" + _invoiceExterior.Document.accessKey + ".xml";
					xmlFEX.Save(pathFileXmlFileName);

				}
			});


			Task.Delay(delayInSeconds).Wait();


			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{

					foreach (Invoice _invoiceExterior in invoiceExteriorList)
					{


						db.Invoice.Attach(_invoiceExterior);
						db.Entry(_invoiceExterior).State = EntityState.Modified;
					}
					db.SaveChanges();
					trans.Commit();

				}
				catch //(Exception e)
				{

					trans.Rollback();
				}


			}


		}


		public static async Task CallXML(DBContext db,
										 List<Invoice> invoiceExteriorList,
										 int idCompany,
										 string routePath,
										 string routePathA1Firmar,
										 int delayInSeconds)
		{

			await Task.Run(() => generateInvoiceXML(db,
													invoiceExteriorList,
													routePath,
													routePathA1Firmar,
													idCompany,
													delayInSeconds
													));

		}

        //Impersonation functionality
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        //Disconnection after file operations
        [DllImport("kernel32.dll")]
        private static extern Boolean CloseHandle(IntPtr hObject);

        public static async Task generateInvoiceXML(DBContext db,
														List<Invoice> invoiceExteriorList,
														string routePath,
														string routePathA1Firmar,
														int idCompany,
														int delayInSeconds
													  )
		{

			XmlDocument xmlFEX = new XmlDocument();
			await Task.Factory.StartNew(() =>
			{
				try
				{
					if (!string.IsNullOrEmpty(routePath))
					{
                        bool aExistsRoutePath = true;

                        IntPtr token = IntPtr.Zero;

                        string USER_rutaXmlFEX = ConfigurationManager.AppSettings["USER_rutaXmlFEX"];
                        USER_rutaXmlFEX = string.IsNullOrEmpty(USER_rutaXmlFEX) ? "admin" : USER_rutaXmlFEX;
                        string PASS_rutaXmlFEX = ConfigurationManager.AppSettings["PASS_rutaXmlFEX"];
                        PASS_rutaXmlFEX = string.IsNullOrEmpty(PASS_rutaXmlFEX) ? "admin" : PASS_rutaXmlFEX;
                        string DOMAIN_rutaXmlFEX = ConfigurationManager.AppSettings["DOMAIN_rutaXmlFEX"];
                        DOMAIN_rutaXmlFEX = string.IsNullOrEmpty(DOMAIN_rutaXmlFEX) ? "" : DOMAIN_rutaXmlFEX;

                        bool result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                        if (result == true)
                        {
                            //Use token to setup a WindowsImpersonationContext 
                            using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                            {
                                aExistsRoutePath = Directory.Exists(routePath);
                                if (!(aExistsRoutePath))
                                {
                                    Directory.CreateDirectory(routePath);
                                    LogController.WriteLog(routePath + ": Creado Exitosamente");
                                }

                                ctx.Undo();
                                CloseHandle(token);
                            }
                        }
                       
						if (aExistsRoutePath)
						{
							foreach (Invoice _invoiceExterior in invoiceExteriorList)
							{
                                result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                if (result == true)
                                {
                                    //Use token to setup a WindowsImpersonationContext 
                                    using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                    {
                                        Task.Delay(delayInSeconds).Wait();
                                        xmlFEX = _invoiceExterior.GenerateXML(idCompany);
                                        string pathFileXmlFileName = routePath + "\\" + _invoiceExterior.Document.accessKey + ".xml";
                                        xmlFEX.Save(pathFileXmlFileName);
                                        LogController.WriteLog(pathFileXmlFileName + ": Salvado Exitosamente");

                                        ctx.Undo();
                                        CloseHandle(token);
                                    }
                                }

                                bool aExistsRoutePathA1Firmar = true;

                                if (!string.IsNullOrEmpty(routePathA1Firmar))
								{
                                    string USER_rutaXmlA1Firmar = ConfigurationManager.AppSettings["USER_rutaXmlA1Firmar"];
                                    USER_rutaXmlA1Firmar = string.IsNullOrEmpty(USER_rutaXmlA1Firmar) ? "admin" : USER_rutaXmlA1Firmar;
                                    string PASS_rutaXmlA1Firmar = ConfigurationManager.AppSettings["PASS_rutaXmlA1Firmar"];
                                    PASS_rutaXmlA1Firmar = string.IsNullOrEmpty(PASS_rutaXmlA1Firmar) ? "admin" : PASS_rutaXmlA1Firmar;
                                    string DOMAIN_rutaXmlA1Firmar = ConfigurationManager.AppSettings["DOMAIN_rutaXmlA1Firmar"];
                                    DOMAIN_rutaXmlA1Firmar = string.IsNullOrEmpty(DOMAIN_rutaXmlA1Firmar) ? "" : DOMAIN_rutaXmlA1Firmar;

                                    result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                    if (result == true)
                                    {
                                        //Use token to setup a WindowsImpersonationContext 
                                        using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                        {
                                            aExistsRoutePathA1Firmar = Directory.Exists(routePathA1Firmar);

                                            if (!(aExistsRoutePathA1Firmar))
                                            {
                                                Directory.CreateDirectory(routePathA1Firmar);
                                                LogController.WriteLog(routePathA1Firmar + ": Creado Exitosamente");
                                            }

                                            ctx.Undo();
                                            CloseHandle(token);
                                        }
                                    }

                                    
									if (aExistsRoutePathA1Firmar)
									{
                                        result = LogonUser(USER_rutaXmlA1Firmar, DOMAIN_rutaXmlA1Firmar, PASS_rutaXmlA1Firmar, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                        if (result == true)
                                        {
                                            //Use token to setup a WindowsImpersonationContext 
                                            using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                            {
                                                //Task.Delay(delayInSeconds).Wait();
                                                //xmlFEX = _invoiceExterior.GenerateXML(idCompany);
                                                string pathFileXmlFileName2 = routePathA1Firmar + "\\" + _invoiceExterior.Document.accessKey + ".xml";
                                                xmlFEX.Save(pathFileXmlFileName2);
                                                LogController.WriteLog(pathFileXmlFileName2 + ": Salvado Exitosamente");

                                                ctx.Undo();
                                                CloseHandle(token);
                                            }
                                        }
                                        

										if (!string.IsNullOrEmpty(routePath))
										{
                                            result = LogonUser(USER_rutaXmlFEX, DOMAIN_rutaXmlFEX, PASS_rutaXmlFEX, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);

                                            if (result == true)
                                            {
                                                //Use token to setup a WindowsImpersonationContext 
                                                using (WindowsImpersonationContext ctx = new WindowsIdentity(token).Impersonate())
                                                {
                                                    if (Directory.Exists(routePath))
                                                    {
                                                        //Task.Delay(delayInSeconds).Wait();
                                                        string pathFileXmlFileName3 = routePath + "\\" + _invoiceExterior.Document.accessKey + ".xml";
                                                        System.IO.File.Delete(pathFileXmlFileName3);
                                                        LogController.WriteLog(pathFileXmlFileName3 + ": Eliminado Exitosamente");
                                                    }

                                                    ctx.Undo();
                                                    CloseHandle(token);
                                                }
                                            }
										}
									}
								}
								else
								{
									LogController.WriteLog("No existe la ruta de A1.Firmar.");
								}

							}
							//string pathFileXmlFileName = routePath + "\\" + invoice.Document.accessKey + ".xml";
							//xmlFEX.Save(pathFileXmlFileName);
						}
					}
					else
					{
						LogController.WriteLog("No existe la ruta de XML de FEX.");
					}

				}
				catch (Exception ex)
				{
					LogController.WriteLog(ex.Message);
				}

			});


			Task.Delay(delayInSeconds).Wait();


			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{

					foreach (Invoice _invoiceExterior in invoiceExteriorList)
					{


						db.Invoice.Attach(_invoiceExterior);
						db.Entry(_invoiceExterior).State = EntityState.Modified;
					}
					db.SaveChanges();
					trans.Commit();

				}
				catch //(Exception e)
				{

					trans.Rollback();
				}


			}


		}
	}



}