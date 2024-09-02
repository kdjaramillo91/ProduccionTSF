using ProductionApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MigracionProduccionCIWebApi.Models
{
	public class ProviderRepository
	{
		private DBContext db = new DBContext();
		private DBContextCI dbCI = new DBContextCI();
		public ProviderRepository()
		{
			//ProviderRepository
		}

		public IEnumerable<string> GetIDNumerProviders()
		{
			var providerIDNumber = db.Provider.Where(w => w.Person.isActive && w.Person.IdentificationType.codeSRI.Equals("04")).Select(s => new
			{
				s.Person.identification_number
			}).ToList();

			List<string> strlist = new List<string>();
			foreach (var pin in providerIDNumber)
			{
				strlist.Add(pin.identification_number);
			}

			return strlist;
		}

		public IEnumerable<PersonProvider> GetAllProviders()
		{
			return db.Provider.Where(w => w.Person.isActive).Select(s => new PersonProvider
			{
				id = s.id,
				id_personType = s.Person.id_personType,
				id_identificationType = s.Person.id_identificationType,
				identification_number = s.Person.identification_number,
				fullname_businessName = s.Person.fullname_businessName,
				photo = s.Person.photo,
				address = s.Person.address,
				email = s.Person.email,
				id_company = s.Person.id_company
			}).ToList();
		}

		public PersonProvider GetProvider(int id)
		{
			return db.Provider
				.Where(w => w.id == id && w.Person.isActive)
				.Select(s => new PersonProvider
				{
					id = s.id,
					id_personType = s.Person.id_personType,
					id_identificationType = s.Person.id_identificationType,
					identification_number = s.Person.identification_number,
					fullname_businessName = s.Person.fullname_businessName,
					photo = s.Person.photo,
					address = s.Person.address,
					email = s.Person.email,
					id_company = s.Person.id_company
				})
				.FirstOrDefault();
		}

		public void UpdateMigrationProvider(MigrationPerson migrationPerson, string message, string modo = "manual")
		{
			var id_ur = migrationPerson.id_user_replicate;
			var fechaCreacion = migrationPerson.dateCreate;
			using (DbContextTransaction tran = db.Database.BeginTransaction())
			{
				try
				{
					var historyMigrationProvider = db.HistoryMigrationPerson.FirstOrDefault(fod => fod.id_migrationPerson == migrationPerson.id);

					if (historyMigrationProvider != null)
					{
						historyMigrationProvider.message = message;
						historyMigrationProvider.mode = modo;
						historyMigrationProvider.id_user_replicate = id_ur;
						historyMigrationProvider.dateCreate = DateTime.Now;
						db.Entry(migrationPerson).State = EntityState.Modified;
					}
					else
					{
						var userCreateAux = db.User.FirstOrDefault(fod => fod.username == "admin" /*|| fod.id == 1*/ || fod.username.Contains("admin"));// ?? db.User.FirstOrDefault();
						HistoryMigrationPerson historyMigrationPerson = new HistoryMigrationPerson
						{
							id_migrationPerson = migrationPerson.id,
							id_person = migrationPerson.id_person,
							id_rol = migrationPerson.id_rol,
							id_userCreateMigrationPerson = migrationPerson.id_userCreate,
							dateCreateMigrationPerson = migrationPerson.dateCreate,
							mode = modo,
							message = message,
							id_user_replicate = id_ur,
							id_userCreate = userCreateAux?.id,
							dateCreate = DateTime.Now
						};
						db.Entry(historyMigrationPerson).State = EntityState.Added;

					}
					db.SaveChanges();
					tran.Commit();
				}
				catch (Exception e)
				{
					tran.Rollback();
					Console.Write(e.Message);
				}
			}

		}

		public void DeleteMigrationInsertHistoryPerson(MigrationPerson migrationPerson, string message, string modo = "manual")
		{
			var id_ur = migrationPerson.id_user_replicate;
			var fechaCreacion = migrationPerson.dateCreate;
			using (DbContextTransaction tran = db.Database.BeginTransaction())
			{
				try
				{
					var userCreateAux = db.User.FirstOrDefault(fod => fod.username == "admin" /*|| fod.id == 1*/ || fod.username.Contains("admin"));// ?? db.User.FirstOrDefault();
					HistoryMigrationPerson historyMigrationPersonTmp = new HistoryMigrationPerson();
					historyMigrationPersonTmp.id_migrationPerson = migrationPerson.id;
					historyMigrationPersonTmp.id_person = migrationPerson.id_person;
					historyMigrationPersonTmp.id_rol = migrationPerson.id_rol;
					historyMigrationPersonTmp.id_userCreateMigrationPerson = migrationPerson.id_userCreate;
					historyMigrationPersonTmp.dateCreateMigrationPerson = migrationPerson.dateCreate;
					historyMigrationPersonTmp.message = message;
					historyMigrationPersonTmp.mode = modo;
					historyMigrationPersonTmp.id_user_replicate = id_ur;
					historyMigrationPersonTmp.id_userCreate = migrationPerson.id_userCreate;
					historyMigrationPersonTmp.dateCreate = DateTime.Now;

					db.HistoryMigrationPerson.Add(historyMigrationPersonTmp);
					db.Entry(historyMigrationPersonTmp).State = EntityState.Added;

					//Delete
					db.MigrationPerson.Remove(migrationPerson);
					db.Entry(migrationPerson).State = EntityState.Deleted;
					db.SaveChanges();
					tran.Commit();
				}
				catch (Exception e)
				{
					tran.Rollback();
					Console.Write(e.Message);
				}
			}


			// CAMPOS DE AUDITORIA 

		}

		public AnswerMigration AddProvider(MigrationPerson migrationPerson, string modo = "manual")
		{
			var messageCommon = "Persona: " + migrationPerson.Person.fullname_businessName +
								" con tipo de Identificación: " + migrationPerson.Person.IdentificationType.code +
								"(" + migrationPerson.Person.identification_number + ")";
			//identification_number
			var answerMigration = new AnswerMigration();


			var provider = db.Provider.FirstOrDefault(fod => fod.id == migrationPerson.id_person);

			if (provider == null)
			{
				answerMigration.message = messageCommon + ". No existe actualmente como Proveedor, se descarto su migración con este Rol";
				//answerPersonProvider.personProvider = null;
				DeleteMigrationInsertHistoryPerson(migrationPerson, answerMigration.message, modo);
				return answerMigration;
			}
			#region "Transaccion Produccion a Contable"

			using (DbContextTransaction trans = dbCI.Database.BeginTransaction())
			{
				try
				{
					var proveedorCI = dbCI.tblGeProveedor.FirstOrDefault(fod => fod.CCiProveedor == migrationPerson.Person.codeCI.ToString());
					var newValue = false;

					if (proveedorCI == null)
					{
						proveedorCI = new tblGeProveedor();
						newValue = true;
						proveedorCI.CCiProveedor = migrationPerson.Person.codeCI.ToString();
					}

					proveedorCI.CNoProveedor = migrationPerson.Person.fullname_businessName;
					proveedorCI.CCtOrigen = provider.ProviderGeneralData?.Origin?.code == "NAC" ? "N" :
											(provider.ProviderGeneralData?.Origin?.code == "EXT" ? "E" : "");
					proveedorCI.CCiTipoIdentificacion = migrationPerson.Person.IdentificationType.codeSRI == "04" ? "R" :
														(migrationPerson.Person.IdentificationType.codeSRI == "05" ? "C" :
														(migrationPerson.Person.IdentificationType.codeSRI == "06" ? "P" :
														(migrationPerson.Person.IdentificationType.codeSRI == "07" ? "CF" :
														(migrationPerson.Person.IdentificationType.codeSRI == "08" ? "IE" :
														(migrationPerson.Person.IdentificationType.codeSRI == "09" ? "PL" : "")))));
                    var maxLengthIdentification = (migrationPerson.Person.identification_number.Length > 13) ? 13 : (migrationPerson.Person.identification_number.Length);
                    proveedorCI.CCiIdentificacion = migrationPerson.Person.identification_number.Substring(0, maxLengthIdentification);
                    proveedorCI.CCiAlterno = provider.ProviderGeneralData?.cod_alternate ?? "";
                    var maxLength = (migrationPerson.Person.address.Length > 255) ? 255 : (migrationPerson.Person.address.Length);
                    proveedorCI.CDsDireccion = migrationPerson.Person.address.Substring(0, maxLength);
                    proveedorCI.CNoPais = provider.ProviderGeneralData?.Country?.name ?? "";
                    if (proveedorCI.CNoPais.Length > 30)
                    {
                        throw new Exception($"Longitud de caracteres por nombre de país de proveedor no permitido: {proveedorCI.CNoPais.Length}. Límite: 30");
                    }
                    proveedorCI.CNoEstado = provider.ProviderGeneralData?.StateOfContry?.name ?? "";
					proveedorCI.CNoCiudad = provider.ProviderGeneralData?.City?.name ?? "";
					proveedorCI.CNuTelefono1 = provider.ProviderGeneralData?.phoneNumber1 ?? "";
					proveedorCI.CNuTelefono2 = provider.ProviderGeneralData?.phoneNumber2 ?? "";
					proveedorCI.CNuFax = provider.ProviderGeneralData?.noFax ?? "";
					proveedorCI.CDsEmail = migrationPerson.Person.email;
					proveedorCI.CSNGrabaIva = (provider.ProviderGeneralData.isCRAFTSMAN == true) ? "S" : "N";

					if (provider.ProviderGeneralData != null && provider.ProviderGeneralData.id_economicGroup != null)
					{
						proveedorCI.CCiGrupoEconomico = provider.ProviderGeneralData.EconomicGroup.code;
						var grupoEconomico = dbCI.TblGeGrupoEconomico.FirstOrDefault(fod => fod.CCiGrupoEconomico == proveedorCI.CCiGrupoEconomico);
						var newGrupoEconomico = false;
						if (grupoEconomico == null)
						{
							grupoEconomico = new TblGeGrupoEconomico();
							grupoEconomico.CCiGrupoEconomico = provider.ProviderGeneralData.EconomicGroup.code;
							grupoEconomico.CNoGrupoEconomico = provider.ProviderGeneralData.EconomicGroup.name;
							grupoEconomico.CCiUsuario = "master";
							grupoEconomico.CCeGrupoEconomico = provider.ProviderGeneralData.EconomicGroup.isActive ? "S" : "N";
							grupoEconomico.CCiUsuarioIngreso = "master";
							grupoEconomico.CDsEstacionIngreso = "producción";
							grupoEconomico.DFxIngreso = DateTime.Now;
							newGrupoEconomico = true;
						}
						if (newGrupoEconomico)
						{
							dbCI.TblGeGrupoEconomico.Add(grupoEconomico);
							dbCI.Entry(grupoEconomico).State = EntityState.Added;
						}
					}


					proveedorCI.CSNContribEspecial = provider.ProviderGeneralData?.specialTaxPayer == true ? "S" : "N";

					var productionUnitProviderTmp = provider?.ProductionUnitProvider?.ToList();
					var codeProductionUnitProvider = string.Empty;
					var providerTypeShrimpTmp = provider?.ProviderTypeShrimp;
					var providerTransportistTmp = provider?.ProviderTransportist;

					if (providerTransportistTmp != null && !string.IsNullOrEmpty(providerTransportistTmp.ANTEspecification))
					{
						proveedorCI.CnuPermisoANT = providerTransportistTmp.ANTEspecification;
					}
					var newProductionUnitProvider = false;
					var nnusecuencia = 0;
					if (productionUnitProviderTmp != null && productionUnitProviderTmp.Count > 0)
					{
						foreach (var tmpProductionUnitProvider in productionUnitProviderTmp)
						{
							codeProductionUnitProvider = string.Empty;
							codeProductionUnitProvider = tmpProductionUnitProvider.code;
							var productionUnitProviderCamaronero = dbCI.TblGeProveedorCamaronero.FirstOrDefault(fod => fod.CCiProveedor == provider.Person.codeCI && fod.CCiUnidadProduccion == codeProductionUnitProvider);
							if (productionUnitProviderCamaronero == null)
							{
								productionUnitProviderCamaronero = new TblGeProveedorCamaronero();
								productionUnitProviderCamaronero.CCiProveedor = provider.Person.codeCI;
								productionUnitProviderCamaronero.CCiUnidadProduccion = tmpProductionUnitProvider.code;
								productionUnitProviderCamaronero.CNoUnidadProduccion = tmpProductionUnitProvider.name;
								productionUnitProviderCamaronero.CDsAcuerdoMinist = tmpProductionUnitProvider.ministerialAgreement;
								productionUnitProviderCamaronero.CNuTramite = tmpProductionUnitProvider.tramitNumber;
								productionUnitProviderCamaronero.CNuRegINP = tmpProductionUnitProvider.INPnumber;
								productionUnitProviderCamaronero.CDsGramaje = (providerTypeShrimpTmp?.Grammage1?.description ?? "") + " - " + (providerTypeShrimpTmp?.Grammage?.description ?? "");
								productionUnitProviderCamaronero.NnuHectareaPisc = tmpProductionUnitProvider?.poolNumber?.ToString();
								productionUnitProviderCamaronero.CciProveedorAmp = providerTypeShrimpTmp?.Provider1?.Person?.codeCI ?? "";
								var lstTmpProductionUnitProvider = dbCI.TblGeProveedorCamaronero.Where(fod => fod.CCiProveedor == provider.Person.codeCI).ToList();
								if (nnusecuencia == 0)
								{
									nnusecuencia = lstTmpProductionUnitProvider.Count > 0 && lstTmpProductionUnitProvider?.Max(m => m.NNuSecuencia) != null ? (int)(lstTmpProductionUnitProvider?.Max(m => m.NNuSecuencia)) + 1 : 1;
								}
								else
								{
									nnusecuencia = nnusecuencia + 1;
								}

								productionUnitProviderCamaronero.NNuSecuencia = nnusecuencia;
								newProductionUnitProvider = true;
							}
							else
							{
								productionUnitProviderCamaronero.CNoUnidadProduccion = tmpProductionUnitProvider.name;
								productionUnitProviderCamaronero.CDsAcuerdoMinist = tmpProductionUnitProvider.ministerialAgreement;
								productionUnitProviderCamaronero.CNuTramite = tmpProductionUnitProvider.tramitNumber;
								productionUnitProviderCamaronero.CNuRegINP = tmpProductionUnitProvider.INPnumber;
								productionUnitProviderCamaronero.CDsGramaje = (providerTypeShrimpTmp?.Grammage1?.description ?? "") + " - " + (providerTypeShrimpTmp?.Grammage?.description ?? "");
								productionUnitProviderCamaronero.NnuHectareaPisc = tmpProductionUnitProvider?.poolNumber?.ToString();
								productionUnitProviderCamaronero.CciProveedorAmp = providerTypeShrimpTmp?.Provider1?.Person?.codeCI ?? "";
								newProductionUnitProvider = false;
							}
							if (newProductionUnitProvider == true)
							{
								dbCI.TblGeProveedorCamaronero.Add(productionUnitProviderCamaronero);
								dbCI.Entry(productionUnitProviderCamaronero).State = EntityState.Added;
							}
							else
							{
								dbCI.Entry(productionUnitProviderCamaronero).State = EntityState.Modified;
							}
						}
					}


					proveedorCI.CCiBanco = provider.ProviderGeneralDataEP?.BoxCardAndBank?.code ?? "";

					if (proveedorCI.CCiBanco != "")
					{
						var BancoAcreditar = dbCI.TblCiBancos.FirstOrDefault(fod => fod.CCiBanco == proveedorCI.CCiBanco);
						var BoxCardAndBank = db.BoxCardAndBank.FirstOrDefault(fod => fod.code == provider.ProviderGeneralDataEP.BoxCardAndBank.code);
						var newBanco = false;
						if (BancoAcreditar == null)
						{
							BancoAcreditar = new TblCiBancos();
							BancoAcreditar.CCiBanco = BoxCardAndBank.code;
							BancoAcreditar.CDsNombreBanco = BoxCardAndBank.name;
							BancoAcreditar.CDsNombreCorto = (BoxCardAndBank.name.Length > 15 ? BoxCardAndBank.name.Substring(1, 15) : BoxCardAndBank.name);
							BancoAcreditar.CSnMuestraenVerfRecarga = "N";
							BancoAcreditar.CCtCajaBanco = BoxCardAndBank.TypeBoxCardAndBank.code == "CAJA" ? "C" :
															(BoxCardAndBank.TypeBoxCardAndBank.code == "TC" ? "T" :
															(BoxCardAndBank.TypeBoxCardAndBank.code == "BAN" ? "B" : ""));
							BancoAcreditar.CTxEtqInicialNombreArchivoTransf = "";
							BancoAcreditar.CTxExtArchivoTransf = "";
							BancoAcreditar.CCtAgrupacionArchivo = "";
							BancoAcreditar.CCeCajaBanco = (BoxCardAndBank.isActive == true) ? "A" : "I";
							BancoAcreditar.CCiBancoInternacional = "";
							BancoAcreditar.CciFormaPagoSRI = "";
							BancoAcreditar.CceBancoSRI = "S";
							BancoAcreditar.CCiFormatoTransfBco = "";
							BancoAcreditar.CCiSistemaTransf = "";
							BancoAcreditar.CCiTipoServicioTransf = "";
							BancoAcreditar.CCtNombreArchivoTransf = "";
							newBanco = true;
						}

						if (newBanco)
						{
							dbCI.TblCiBancos.Add(BancoAcreditar);
							dbCI.Entry(BancoAcreditar).State = EntityState.Added;
						}
					}

					proveedorCI.CNuCuenta = provider.ProviderGeneralDataEP?.noAccountEP ?? "";
					proveedorCI.CCtCuenta = provider.ProviderGeneralDataEP?.AccountTypeGeneral?.code == "AHO" ? "A" :
											(provider.ProviderGeneralDataEP?.AccountTypeGeneral?.code == "COR" ? "C" :
											(provider.ProviderGeneralDataEP?.AccountTypeGeneral?.code == "VIR" ? "V" : ""));
					proveedorCI.CCtAplicaDscto = "";
					proveedorCI.CCtDsctoGeneral = "";
					//proveedorCI.NVtDsctoCredito = provider.ProviderGeneralData?.creditDiscount ?? 0;
					//proveedorCI.NVtDsctoContado = provider.ProviderGeneralData?.discountedDiscount ?? 0;
					//proveedorCI.NVtCupoCredito = provider.ProviderGeneralData?.cupoCredit ?? 0;
					//proveedorCI.NVtCupoEmergente = provider.ProviderGeneralData?.cupoEmergent ?? 0;
					//proveedorCI.NVtCupoDisponible = provider.ProviderGeneralData?.cupoAvailable ?? 0;
					//proveedorCI.CCtAplicaDscto = provider.ProviderGeneralData?.DiscountToDetailApplyTo?.code == "PRE" ? "P":
					//                                (provider.ProviderGeneralData?.DiscountToDetailApplyTo?.code == "SUB" ? "S" : "");
					proveedorCI.CCiCategoria = "";//(No hace nada se guarda en blanco en Contable)

					var CCiTipoProveedorAux = provider.ProviderGeneralData?.ProviderType?.code ?? "";


					if (provider.ProviderAccountingAccounts != null)
					{
						//List<ProviderAccountingAccounts> paaTmpLst = provider.ProviderAccountingAccounts.ToList();
						//foreach (var paaTmp in paaTmpLst)
						//{
						//    TblGeCuentaProveedor tgecpTemporal = dbCI.TblGeCuentaProveedor.FirstOrDefault(fod => fod.CCiCia == paaTmp.Company.code &&
						//                                            fod.CCiDivision == paaTmp.Division.code &&
						//                                            fod.CCiSucursal == paaTmp.BranchOffice.code &&
						//                                            fod.CCiPlanCta == paaTmp.AccountPlan.code &&
						//                                            fod.CCiProveedor == provider.
						//                                            )
						//} 
					}

					if (CCiTipoProveedorAux != proveedorCI.CCiTipoProveedor)
					{
						proveedorCI.CCiTipoProveedor = CCiTipoProveedorAux;
						if (proveedorCI.CCiTipoProveedor != "")
						{
							var tipoProveedor = dbCI.TblGeTipoProveedor.FirstOrDefault(fod => fod.CCiTipoProveedor == proveedorCI.CCiTipoProveedor);
							var newtipoProveedor = false;
							if (tipoProveedor == null)
							{
								tipoProveedor = new TblGeTipoProveedor();
								tipoProveedor.CCiTipoProveedor = proveedorCI.CCiTipoProveedor;
								newtipoProveedor = true;
							}

							tipoProveedor.CNoTipoProveedor = provider.ProviderGeneralData?.ProviderType.name;
							tipoProveedor.CCeTipoProveedor = (bool)(provider.ProviderGeneralData?.ProviderType?.isActive) ? "A" : "I";

							if (newtipoProveedor)
							{
								dbCI.TblGeTipoProveedor.Add(tipoProveedor);
								dbCI.Entry(tipoProveedor).State = EntityState.Added;
							}
							else
							{
								dbCI.Entry(tipoProveedor).State = EntityState.Modified;
							}

						}
					}
					else
					{
						if (proveedorCI.CCiTipoProveedor != "")
						{
							var tipoProveedor = dbCI.TblGeTipoProveedor.FirstOrDefault(fod => fod.CCiTipoProveedor == proveedorCI.CCiTipoProveedor);

							tipoProveedor.CNoTipoProveedor = provider.ProviderGeneralData?.ProviderType.name;
							tipoProveedor.CCeTipoProveedor = provider.ProviderGeneralData.ProviderType.isActive ? "A" : "I";

							dbCI.Entry(tipoProveedor).State = EntityState.Modified;

						}
					}


					proveedorCI.CDsReferencia = provider.ProviderGeneralData?.reference ?? "";
					proveedorCI.CDsObservacion = provider.ProviderGeneralData?.observation ?? "";
					proveedorCI.CCeProveedor = migrationPerson.Person.isActive ? "A" : "I";



					if (migrationPerson.isNewPerson || newValue)
					{
						proveedorCI.CCiUsuarioIngreso = db.User.FirstOrDefault(fod => fod.id == migrationPerson.Person.id_userCreate).username ?? "";
						proveedorCI.CDsEstacionIngreso = "PC-Producción";//Hay que ver de donde sale estos campos por el momento fijo;
						proveedorCI.DFiIngreso = migrationPerson.dateCreate;
						proveedorCI.DFmModifica = null;
						proveedorCI.CCiUsuarioModifica = null;
						proveedorCI.CDsEstacionModifica = null;
					}
					else
					{
						proveedorCI.CCiUsuarioModifica = db.User.FirstOrDefault(fod => fod.id == migrationPerson.Person.id_userUpdate).username ?? "";
						proveedorCI.CDsEstacionModifica = "PC-Producción";//Hay que ver de donde sale estos campos por el momento fijo;
						proveedorCI.DFmModifica = migrationPerson.dateCreate;
					}

					proveedorCI.CNoContacto = provider.ProviderGeneralData?.contactName ?? "";
					proveedorCI.CNuTelefContacto = provider.ProviderGeneralData?.contactPhoneNumber ?? "";
					proveedorCI.CNuFaxContacto = provider.ProviderGeneralData?.contactNoFax ?? "";
					//proveedorCI.CCtDsctoGeneral = provider.ProviderGeneralData?.BasisForGeneralDiscounts?.code == "VAP" ? "P" :
					//                                (provider.ProviderGeneralData?.BasisForGeneralDiscounts?.code == "RAD" ? "R" : "");
					proveedorCI.CSNTransfCuenta = provider.ProviderGeneralData?.electronicPayment == true ? "S" : "N";
					proveedorCI.CCiRutaCitiBank = provider.ProviderGeneralDataEP?.RtInternational?.code == "ABA" ? "A" :
												  (provider.ProviderGeneralDataEP?.RtInternational?.code == "CHIP" ? "C" :
												  (provider.ProviderGeneralDataEP?.RtInternational?.code == "SWIFT" ? "S" : ""));
					proveedorCI.CNuRutaCitiBank = provider.ProviderGeneralDataEP?.noRoute ?? "";
					proveedorCI.CsnObligaContabilidad = provider.ProviderGeneralData?.forcedToKeepAccounts == true ? "S" : "N";
					//proveedorCI.CdsNombreComercial = provider.ProviderGeneralData?.tradeName ?? "";
					proveedorCI.CdsNombreComercial = provider.Person.tradeName ?? "";
					proveedorCI.CSnRise = provider.ProviderGeneralData?.rise == true ? "S" : "N";
					proveedorCI.CSnCajaChica = provider.ProviderGeneralData?.TypeBoxCardAndBank?.code == "CAJA" ? "S" :
												(provider.ProviderGeneralData?.TypeBoxCardAndBank?.code == "TAR" ? "A" : "N");

					var CCiBancoCajaAux = provider.ProviderGeneralData?.BoxCardAndBank?.code ?? "";
					if (CCiBancoCajaAux != proveedorCI.CCiBancoCaja)
					{
						proveedorCI.CCiBancoCaja = CCiBancoCajaAux;
						if (proveedorCI.CCiBancoCaja != "")
						{
							var ciBancos = dbCI.TblCiBancos.FirstOrDefault(fod => fod.CCiBanco == proveedorCI.CCiBancoCaja);
							var newCiBancos = false;
							if (ciBancos == null)
							{
								ciBancos = new TblCiBancos();
								ciBancos.CCiBanco = proveedorCI.CCiBancoCaja;
								newCiBancos = true;
							}

							ciBancos.CDsNombreBanco = provider.ProviderGeneralData.BoxCardAndBank?.name;
							ciBancos.CCeCajaBanco = (bool)(provider.ProviderGeneralData.BoxCardAndBank?.isActive) ? "A" : "I";

							if (newCiBancos)
							{
								ciBancos.CCiBancoInternacional = ""; //Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CciFormaPagoSRI = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CceBancoSRI = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CCtCajaBanco = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CDsNombreCorto = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CCiFormatoTransfBco = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CCiSistemaTransf = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CTxExtArchivoTransf = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CCiTipoServicioTransf = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CCtAgrupacionArchivo = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CCtNombreArchivoTransf = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CTxEtqInicialNombreArchivoTransf = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
								ciBancos.CSnMuestraenVerfRecarga = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción

								dbCI.TblCiBancos.Add(ciBancos);
								dbCI.Entry(ciBancos).State = EntityState.Added;
							}
							else
							{
								dbCI.Entry(ciBancos).State = EntityState.Modified;
							}

						}
						proveedorCI.CCiCtaCteCaja = ""; // Esto se llena en CI de Panacea no tengo las estructuras en Producción
					}
					else
					{
						if (proveedorCI.CCiBancoCaja != "")
						{
							var ciBancos = dbCI.TblCiBancos.FirstOrDefault(fod => fod.CCiBanco == proveedorCI.CCiBancoCaja);
							ciBancos.CDsNombreBanco = provider.ProviderGeneralData.BoxCardAndBank?.name;
							ciBancos.CCeCajaBanco = (bool)(provider.ProviderGeneralData.BoxCardAndBank?.isActive) ? "A" : "I";

							dbCI.Entry(ciBancos).State = EntityState.Modified;
						}
					}

					proveedorCI.CciIdentificacionTransF = provider.ProviderGeneralDataEP?.noRefTransfer ?? "";

					//proveedorCI.CCiPais
					var CCiPaisAux = provider.ProviderGeneralData?.Country?.code ?? "";
					if (CCiPaisAux != proveedorCI.CCiPais)
					{
						proveedorCI.CCiPais = CCiPaisAux;
						if (proveedorCI.CCiPais != "")
						{
							var pais = dbCI.TblGePais.FirstOrDefault(fod => fod.CCiPais == proveedorCI.CCiPais);
							var newpais = false;
							if (pais == null)
							{
								pais = new TblGePais();
								pais.CCiPais = proveedorCI.CCiPais;
								newpais = true;
							}

							pais.CNoPais = provider.ProviderGeneralData.Country.name;
							pais.CCePais = provider.ProviderGeneralData.Country.isActive ? "A" : "I";

							if (newpais)
							{
								pais.CCiCodigoSRI = ""; // Esto se llena en CI de Panacea no tengo las estructuras en Producción
								pais.CCiCodigoUAF = ""; // Esto se llena en CI de Panacea no tengo las estructuras en Producción

								dbCI.TblGePais.Add(pais);
								dbCI.Entry(pais).State = EntityState.Added;
							}
							else
							{
								dbCI.Entry(pais).State = EntityState.Modified;
							}

						}
					}
					else
					{
						if (proveedorCI.CCiPais != "")
						{
							var pais = dbCI.TblGePais.FirstOrDefault(fod => fod.CCiPais == proveedorCI.CCiPais);

							pais.CNoPais = provider.ProviderGeneralData.Country.name;
							pais.CCePais = provider.ProviderGeneralData.Country.isActive ? "A" : "I";

							dbCI.Entry(pais).State = EntityState.Modified;

						}
					}

					//proveedorCI.CCiServicioPago
					var CCiServicioPagoAux = provider.ProviderGeneralDataEP?.PaymentMethod?.code ?? "";
					if (CCiServicioPagoAux != proveedorCI.CCiServicioPago)
					{
						proveedorCI.CCiServicioPago = CCiServicioPagoAux;
						if (proveedorCI.CCiServicioPago != "")
						{
							var cServicioPagoElectronico = dbCI.TblBcServicioPagoElectronico.FirstOrDefault(fod => fod.CCiServicioPago == proveedorCI.CCiServicioPago);
							var newcServicioPagoElectronico = false;
							if (cServicioPagoElectronico == null)
							{
								cServicioPagoElectronico = new TblBcServicioPagoElectronico();
								cServicioPagoElectronico.CCiServicioPago = proveedorCI.CCiServicioPago;
								newcServicioPagoElectronico = true;
							}

							cServicioPagoElectronico.CNoServicioPago = provider.ProviderGeneralDataEP.PaymentMethod.name;
							cServicioPagoElectronico.CCeServicioPago = provider.ProviderGeneralDataEP.PaymentMethod.isActive ? "A" : "I";

							if (newcServicioPagoElectronico)
							{
								cServicioPagoElectronico.CSnCuenta = ""; // Esto se llena en CI de Panacea no tengo las estructuras en Producción

								dbCI.TblBcServicioPagoElectronico.Add(cServicioPagoElectronico);
								dbCI.Entry(cServicioPagoElectronico).State = EntityState.Added;
							}
							else
							{
								dbCI.Entry(cServicioPagoElectronico).State = EntityState.Modified;
							}

						}
					}
					else
					{
						if (proveedorCI.CCiServicioPago != "")
						{
							var cServicioPagoElectronico = dbCI.TblBcServicioPagoElectronico.FirstOrDefault(fod => fod.CCiServicioPago == proveedorCI.CCiServicioPago);

							cServicioPagoElectronico.CNoServicioPago = provider.ProviderGeneralDataEP.PaymentMethod.name;
							cServicioPagoElectronico.CCeServicioPago = provider.ProviderGeneralDataEP.PaymentMethod.isActive ? "A" : "I";

							dbCI.Entry(cServicioPagoElectronico).State = EntityState.Modified;

						}
					}

					proveedorCI.CCtEmisionDctoElectronico = provider.electronicDocumentIssuance ? "S" : "N";

					//RISE
					//proveedorCI.CCiCategoriaRISE
					var CCiCategoriaRISEAux = provider.ProviderGeneralDataRise?.CategoryRise.code ?? "";
					if (CCiCategoriaRISEAux != proveedorCI.CCiCategoriaRISE)
					{
						proveedorCI.CCiCategoriaRISE = CCiCategoriaRISEAux;
						if (proveedorCI.CCiCategoriaRISE != "")
						{
							var categoriaRISE = dbCI.TblCpCategoriaRISE.FirstOrDefault(fod => fod.CCiCategoria == proveedorCI.CCiCategoriaRISE);
							var newcategoriaRISE = false;
							if (categoriaRISE == null)
							{
								categoriaRISE = new TblCpCategoriaRISE();
								categoriaRISE.CCiCategoria = proveedorCI.CCiCategoriaRISE;
								newcategoriaRISE = true;
							}

							categoriaRISE.CDsCategoria = provider.ProviderGeneralDataRise.CategoryRise.name;
							categoriaRISE.CCeCategoria = provider.ProviderGeneralDataRise.CategoryRise.isActive ? "A" : "I";
							categoriaRISE.DFmModifica = DateTime.Now;//Hay que ver de donde sale estos campos por el momento fijo;
							categoriaRISE.CCiUsuarioModifica = db.User.FirstOrDefault(fod => fod.id == provider.ProviderGeneralDataRise.CategoryRise.id_userUpdate).username ?? "";
							categoriaRISE.CDsEstacionModifica = "PC-Producción";//Hay que ver de donde sale estos campos por el momento fijo;

							if (newcategoriaRISE)
							{
								categoriaRISE.CSnSegmento1 = "S";//Hay que ver de donde sale estos campos por el momento fijo;
								categoriaRISE.CSnSegmento2 = "N";//Hay que ver de donde sale estos campos por el momento fijo;
								categoriaRISE.CSnSegmento3 = "N";//Hay que ver de donde sale estos campos por el momento fijo;
								categoriaRISE.NNuValorInicial = 0;//Hay que ver de donde sale estos campos por el momento fijo;
								categoriaRISE.NNuValorFinal = 60000;//Hay que ver de donde sale estos campos por el momento fijo;
								categoriaRISE.DFiIngreso = provider.ProviderGeneralDataRise.CategoryRise.dateCreate;
								categoriaRISE.CCiUsuarioIngreso = db.User.FirstOrDefault(fod => fod.id == provider.ProviderGeneralDataRise.CategoryRise.id_userCreate).username ?? "";
								categoriaRISE.CDsEstacionIngreso = "PC-Producción";//Hay que ver de donde sale estos campos por el momento fijo;

								dbCI.TblCpCategoriaRISE.Add(categoriaRISE);
								dbCI.Entry(categoriaRISE).State = EntityState.Added;
							}
							else
							{
								dbCI.Entry(categoriaRISE).State = EntityState.Modified;
							}

						}
					}
					else
					{
						if (proveedorCI.CCiCategoriaRISE != "")
						{
							var categoriaRISE = dbCI.TblCpCategoriaRISE.FirstOrDefault(fod => fod.CCiCategoria == proveedorCI.CCiCategoriaRISE);

							categoriaRISE.CDsCategoria = provider.ProviderGeneralDataRise.CategoryRise.name;
							categoriaRISE.CCeCategoria = provider.ProviderGeneralDataRise.CategoryRise.isActive ? "A" : "I";

							dbCI.Entry(categoriaRISE).State = EntityState.Modified;

						}
					}

					var CCiActividadRISEAux = provider.ProviderGeneralDataRise?.ActivityRise.code ?? "";
					if (CCiActividadRISEAux != proveedorCI.CCiActividadRISE)
					{
						proveedorCI.CCiActividadRISE = CCiActividadRISEAux;
						if (proveedorCI.CCiActividadRISE != "")
						{
							var actividadRISE = dbCI.TblCpActividadRISE.FirstOrDefault(fod => fod.CCiActividad == proveedorCI.CCiActividadRISE);
							var newactividadRISE = false;
							if (actividadRISE == null)
							{
								actividadRISE = new TblCpActividadRISE();
								actividadRISE.CCiActividad = proveedorCI.CCiActividadRISE;
								newactividadRISE = true;
							}

							actividadRISE.CDsActividad = provider.ProviderGeneralDataRise.ActivityRise.name;
							actividadRISE.CCeActividad = provider.ProviderGeneralDataRise.ActivityRise.isActive ? "A" : "I";
							actividadRISE.DFmModifica = provider.ProviderGeneralDataRise.ActivityRise.dateUpdate;
							actividadRISE.CCiUsuarioModifica = db.User.FirstOrDefault(fod => fod.id == provider.ProviderGeneralDataRise.ActivityRise.id_userUpdate).username ?? "";
							actividadRISE.CDsEstacionModifica = "PC-Producción";//Hay que ver de donde sale estos campos por el momento fijo;

							if (newactividadRISE)
							{
								actividadRISE.DFiIngreso = provider.ProviderGeneralDataRise.ActivityRise.dateCreate;
								actividadRISE.CCiUsuarioIngreso = db.User.FirstOrDefault(fod => fod.id == provider.ProviderGeneralDataRise.ActivityRise.id_userCreate).username ?? "";
								actividadRISE.CDsEstacionIngreso = "PC-Producción";//Hay que ver de donde sale estos campos por el momento fijo;

								dbCI.TblCpActividadRISE.Add(actividadRISE);
								dbCI.Entry(actividadRISE).State = EntityState.Added;
							}
							else
							{
								dbCI.Entry(actividadRISE).State = EntityState.Modified;
							}

						}
					}
					else
					{
						if (proveedorCI.CCiActividadRISE != "")
						{
							var actividadRISE = dbCI.TblCpActividadRISE.FirstOrDefault(fod => fod.CCiActividad == proveedorCI.CCiActividadRISE);

							actividadRISE.CDsActividad = provider.ProviderGeneralDataRise.ActivityRise.name;
							actividadRISE.CCeActividad = provider.ProviderGeneralDataRise.ActivityRise.isActive ? "A" : "I";

							dbCI.Entry(actividadRISE).State = EntityState.Modified;

						}
					}

					//Falta ingresar si existe la combinación Tblcprelactividadcategoriarise


					proveedorCI.CCtIdentificacionArchivo = provider.ProviderGeneralDataEP?.IdentificationType?.codeSRI == "04" ? "R" :
														(provider.ProviderGeneralDataEP?.IdentificationType?.codeSRI == "05" ? "C" :
														(provider.ProviderGeneralDataEP?.IdentificationType?.codeSRI == "06" ? "P" :
														(provider.ProviderGeneralDataEP?.IdentificationType?.codeSRI == "07" ? "CF" :
														(provider.ProviderGeneralDataEP?.IdentificationType?.codeSRI == "08" ? "IE" :
														(provider.ProviderGeneralDataEP?.IdentificationType?.codeSRI == "09" ? "PL" : "")))));

					proveedorCI.CSnMantenerDatos = provider.ProviderGeneralData?.electronicPayment == true ? "S" : "N";

					//proveedorCI.CDsEmailHide = provider.ProviderGeneralData?.bCC ?? "";
					proveedorCI.CDsEmailHide = provider.Person.bCC ?? "";

					proveedorCI.CSnExportadorHabitual = provider.ProviderGeneralData?.habitualExporter == true ? "S" : "N";
					proveedorCI.CSnPagoParaisoFiscal = provider.ProviderGeneralData?.taxHavenExporter == true ? "S" : "N";
					proveedorCI.CDsEmailPagoElectronico = provider.ProviderGeneralDataEP?.emailEP ?? "";
					proveedorCI.CsnAplicaLeySolidaria = provider.ProviderGeneralData?.sponsoredLinks == true ? "S" : "N";



					if (newValue)
					{
						proveedorCI.CCiTipoProvSRI = "";//Hay que ver de donde sale estos campos por el momento fijo al parecer es de una base de Anexos Transsaccional;
						proveedorCI.CSnParteRelacionada = ""; // Esto se llena en CI de Panacea no tengo las estructuras en Producción
						proveedorCI.CSnAplicaDobleTributacion = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
						proveedorCI.CSnExtSujReyNorLeg = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
						proveedorCI.CSnEnviarCorreo = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
						proveedorCI.NNuResolucionSRI = null;//Esto se llena en CI de Panacea no tengo las estructuras en Producción

						proveedorCI.CCiCliente = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción
						proveedorCI.CCiIdentificacionClte = "";//Esto se llena en CI de Panacea no tengo las estructuras en Producción

						proveedorCI.CTxCampoAdicionalRTE1 = "";//Hay que ver de donde sale estos campos por el momento fijo;
						proveedorCI.CTxCampoAdicionalRTE2 = "";//Hay que ver de donde sale estos campos por el momento fijo;
						proveedorCI.CTxCampoAdicionalRTE3 = "";//Hay que ver de donde sale estos campos por el momento fijo;
						proveedorCI.CTxCampoAdicionalRTE4 = "";//Hay que ver de donde sale estos campos por el momento fijo;
						proveedorCI.CTxCampoAdicionalRTE5 = "";//Hay que ver de donde sale estos campos por el momento fijo;

						dbCI.tblGeProveedor.Add(proveedorCI);
						dbCI.Entry(proveedorCI).State = EntityState.Added;
					}
					else
					{
						dbCI.Entry(proveedorCI).State = EntityState.Modified;

					}

					//model.Add(modelPerson);
					dbCI.SaveChanges();
					trans.Commit();

				}
				catch (Exception e)
				{
					//ViewData["EditError"] = e.Message;

					trans.Rollback();
					answerMigration.message = messageCommon + ". Error no esperado: " + e.Message + (e.InnerException != null ? ("- Excepción Interna: " + e.InnerException.Message) : "");
					answerMigration.resultado = false;
					UpdateMigrationProvider(migrationPerson, answerMigration.message, modo);
					//answerPersonProvider.personProvider = null;
					return answerMigration;
				}
			}
			#endregion
			answerMigration.message = messageCommon + ". Migrado satisfactoriamente.";
			answerMigration.resultado = true;
			DeleteMigrationInsertHistoryPerson(migrationPerson, answerMigration.message, modo);
			return answerMigration;
		}

		public List<AnswerPersonProvider> AddProviders(List<PersonProvider> listProvider)
		{
			var listAnswerPersonProvider = new List<AnswerPersonProvider>();
			//foreach (var provider in listProvider)
			//{
			//    listAnswerPersonProvider.Add(AddProvider(provider));
			//}
			return listAnswerPersonProvider;
		}

		public List<AnswerMigration> AddProviders()
		{
			string sMensaje = string.Empty;

			List<MigrationPerson> listProviderUpdate = new List<MigrationPerson>();

			using (DbContextTransaction tran = db.Database.BeginTransaction())
			{
				try
				{
					listProviderUpdate = db.MigrationPerson.Where(w => w.Rol.name == "Proveedor").ToList();
					tran.Commit();
				}
				catch (Exception ex)
				{
					if (ex.InnerException.Message != null)
					{
						sMensaje = string.Concat("Excepcion: ", ex.Message, "; Excepción Interna: ", ex.InnerException.Message);
					}
					tran.Rollback();

				}
			}

			if (!string.IsNullOrEmpty(sMensaje))
			{
				return null;
			}

			var listAnswerPersonProvider = new List<AnswerMigration>();

			foreach (var provider in listProviderUpdate)
			{
				listAnswerPersonProvider.Add(AddProvider(provider));


			}

			return listAnswerPersonProvider;


		}

		public string AddModifyProvider(int id)
		{
			string resultado;
			string sMensaje = string.Empty;
			MigrationPerson mpTmp = null;
			AnswerMigration amTmp = null;
			using (DbContextTransaction tran = db.Database.BeginTransaction())
			{
				try
				{
					mpTmp = db.MigrationPerson.FirstOrDefault(fod => fod.Rol.name == "Proveedor" && fod.id_person == id);
					tran.Commit();
				}
				catch (Exception ex)
				{
					if (ex.InnerException.Message != null)
					{
						sMensaje = string.Concat("Excepcion: ", ex.Message, "; Excepción Interna: ", ex.InnerException.Message);
					}
					tran.Rollback();

				}
			}
			amTmp = (mpTmp != null ? AddProvider(mpTmp, "En línea") : null);

			resultado = amTmp != null ? (amTmp.resultado == true ? "OK" : amTmp.message) : "NO EXISTE PERSONA DISPONIBLE PARA MIGRACIÓN";

			return resultado;
		}
	}
}