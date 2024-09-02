using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessActionAdvanceTransport : IIntegrationProcessActionOutput
	{
		string IIntegrationProcessActionOutput.FindDocument(
			DBContext db,
			tbsysIntegrationDocumentConfig integrationConfig,
			ref IEnumerable<Document> preDocument)
		{
			var msgGeneral = "";

			try
			{
				// Estados Viatico Terrestre
				var codeStatusValidate1 = integrationConfig
					.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault(r => r.code == "VTT1")?
					.valueDirectValidate;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Anticipo Transporte, No Definida");
				}

				var idAnticiposTransporteValidos = db.RemissionGuideCustomizedAdvancedTransportist
					.Where(r => r.isActive
							&& r.id_PaymentState != null
							&& r.tbsysCatalogState.codeState == codeStatusValidate1
							&& r.hasPayment)
					.Select(r => r.id_AdvancedTransportist)
					.ToList();

				preDocument = preDocument
					.Where(d => idAnticiposTransporteValidos.Contains(d.id))
					.ToList();
			}
			catch (Exception e)
			{
				msgGeneral = e.Message;
			}

			return msgGeneral;
		}

		Dictionary<string, string> IIntegrationProcessActionOutput.GetGlossX(
			DBContext db,
			int id_document,
			string code_documentType)
		{
			var _advancedTransportist = db.RemissionGuideCustomizedAdvancedTransportist
				.FirstOrDefault(r => r.id_AdvancedTransportist == id_document);

			if (_advancedTransportist == null)
			{
				throw new Exception("Documento: " + id_document.ToString() + ", No definido.");
			}


			var idRemissionGuide = _advancedTransportist.id_RemissionGuide;
			var fechaAprobacionAnticipo = (_advancedTransportist.dateApproved.HasValue)
				? _advancedTransportist.dateApproved.Value.ToString("dd/MM/yyyy")
				: "";


			var _remissionGuide = db.RemissionGuide
				.FirstOrDefault(r => r.id == idRemissionGuide);
			if (_remissionGuide == null)
			{
				throw new Exception("Documento Guía de Remisión: " + idRemissionGuide.ToString() + ", No definido.");
			}


			var guiaRemisionNumero = _remissionGuide.Document.number;
			var _remissionGuideTransportation = _remissionGuide.RemissionGuideTransportation;
			if (_remissionGuideTransportation == null)
			{
				throw new Exception("Infoormación de Transporte para Guía de Remisión: " + idRemissionGuide.ToString() + ", No definido.");
			}


			var nombreCompaniaTransporte = _remissionGuideTransportation
				.VehicleProviderTransportBilling?
				.Person?
				.fullname_businessName ?? "";
			var _vehicle = _remissionGuideTransportation.Vehicle;
			if (_vehicle == null)
			{
				throw new Exception("Infoormación del Vehículo Transporte para Guía de Remisión: " + idRemissionGuide.ToString() + ", No definido.");
			}


			var nombrePropietario = _vehicle
				.Person?
				.fullname_businessName ?? "";

			var placaVehiculo = _vehicle
				.carRegistration;


			return new Dictionary<string, string>
				{
					{ "Guía Remisión", guiaRemisionNumero },
					{ "Fecha Aprobación", fechaAprobacionAnticipo },
					{ "Compañía Tansporte", nombreCompaniaTransporte },
					{ "Propietario", nombrePropietario },
					{ "Placa", placaVehiculo },
				};
		}

		decimal IIntegrationProcessActionOutput.GetTotalValue(Document document)
		{
			return document
				.RemissionGuideCustomizedAdvancedTransportist
				.FirstOrDefault()?
				.valueTotal ?? Decimal.Zero;
		}

		List<IntegrationProcessPrintGroup> IIntegrationProcessActionOutput.PrintGroup(
			DBContext db,
			List<IntegrationProcessDetail> integrationProcessDetailList)
		{
			var outputAux = this.GroupData(db, integrationProcessDetailList);
			if (outputAux == null)
			{
				return null;
			}

			var returnPrintGroupList = new List<IntegrationProcessPrintGroup>();

			outputAux.ForEach(r =>
			{
				var _descriptionGroup = "";
				var _valueTotal = 0m;
				var _personCompaniaTransporte = db.Person.FirstOrDefault(s => s.id == r.idObjectParent1);
				var _personPropietario = db.Person.FirstOrDefault(s => s.id == r.idObjectParent2);

				if (_personCompaniaTransporte != null)
				{
					_descriptionGroup = _personCompaniaTransporte.fullname_businessName;
				}

				if (_personPropietario != null)
				{
					_descriptionGroup += " (" + _personPropietario.fullname_businessName + ")";
				}

				_valueTotal = r.ObjectChild.Sum(s => s.decimalObjectChild1);

				var _printGroup = new IntegrationProcessPrintGroup
				{
					descriptionGroup = _descriptionGroup,
					valueTotal = _valueTotal
				};

				returnPrintGroupList.Add(_printGroup);
			});

			return returnPrintGroupList;
		}

		string IIntegrationProcessActionOutput.Validate(
			DBContext db,
			int id_ActiveUser,
			DocumentType documentType,
			IntegrationProcess integrationProcessLote,
			List<IntegrationProcessDetail> integrationProcessDetailList,
			List<AdvanceParametersDetail> _parametersDetail,
			Func<DBContext, int, int, string, string, string, List<AdvanceParametersDetail>, int> saveMessageError)
		{
			var msgGeneral = "";

			try
			{
				var integrationDocumentConfig = db.tbsysIntegrationDocumentConfig
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.CodeIntegrationProcess.AnticipoTransportista);

				if (integrationDocumentConfig == null)
				{
					throw new Exception("Configuración proceso de Integración para Anticipo Transportista, No definido");
				}

				var codeStatusValidate1 = integrationDocumentConfig
					.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault(r => r.code == "VTT1")?
					.valueDirectValidate;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Anticipo Transporte, No Definida");
				}

				integrationProcessDetailList.ForEach(r =>
				{
					var ierr = 0;

					var advancedTransportist = db.RemissionGuideCustomizedAdvancedTransportist
						.FirstOrDefault(s => s.id_AdvancedTransportist == r.id_Document
									&& s.isActive
									&& s.id_PaymentState != null
									&& s.valueTotal > 0);

					if (advancedTransportist == null)
					{
						throw new Exception("Anticipo #" + r.id_Document + ". No activo, sin estado o sin valores.");
					}

					if (advancedTransportist.tbsysCatalogState.codeState.Trim() != codeStatusValidate1)
					{
						ierr++;
						msgGeneral += (String.IsNullOrEmpty(msgGeneral) ? "" : " ,") + r.Document.number + "(" + r.id_Document + ")";

						saveMessageError(
							db,
							id_ActiveUser,
							r.id,
							"Documento no tiene estado para ser Integrado",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);
					}
				});
			}
			catch (Exception e)
			{
				msgGeneral = e.Message;
			}

			return msgGeneral;
		}

		string IIntegrationProcessActionOutput.SaveOutput(
			DBContext db,
			int id_ActiveUser,
			DocumentType documentType,
			ref IntegrationProcess integrationProcessLote,
			List<IntegrationProcessDetail> integrationProcessDetailList,
			List<AdvanceParametersDetail> _parametersDetail,
			Func<DBContext, int, int, string, string, string, List<AdvanceParametersDetail>, int> saveMessageError)
		{
			var msgGeneral = "";

			try
			{
				var _stateTransmit = db.IntegrationState
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Create);

				if (_stateTransmit == null)
				{
					throw new Exception("Estado Creado para el proceso de integración, No definido.");
				}

				// Preparar la Agrupacion por Compañia, Propietario
				var _integrationProcessOutputAuxList = this.GroupData(db, integrationProcessDetailList);

				if (_integrationProcessOutputAuxList == null || _integrationProcessOutputAuxList.Count == 0)
				{
					throw new Exception("No existen Infomación relacionada a los Documentos a Integrar");
				}

				// Prepar la informacion de Output
				foreach (IntegrationProcessOutputAux outputAux in _integrationProcessOutputAuxList)
				{
					var _document = db.Document
						.FirstOrDefault(r => r.id == outputAux.idObjectParent4);
					var _emissionPoint = _document.EmissionPoint;

					var _propietario = db.Person
						.FirstOrDefault(r => r.id == outputAux.idObjectParent1);

					var codigoTipoIdentificacion = _propietario
						.IdentificationType?
						.code ?? "";
					var identificacion = _propietario
						.identification_number ?? "";

					var _branchOffice = _emissionPoint.BranchOffice;

					if (_branchOffice == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							outputAux.idObjectParent3,
							"Establecimiento no definido",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Establecimiento no Definido");
					}

					if (_propietario == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							outputAux.idObjectParent3,
							"Empleado no Definido",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Socio de Negocio no Definido");
					}

					var idsGuiasRemision = outputAux.ObjectChild
						.Select(r => r.idObjectChild2)
						.ToArray();

					var guiasRemision = db.Document
						.Where(r => idsGuiasRemision.Contains(r.id))
						.Select(r => r.number)
						.AsEnumerable()
						.Aggregate((i, j) => i + ", " + j);

					var valorAnticipo = outputAux.ObjectChild
						.Sum(r => r.decimalObjectChild1);

					var _processOutput = new IntegrationProcessOutput()
					{
						IntegrationProcessOutputDocument = new List<IntegrationProcessOutputDocument>()
					};

					_processOutput.code_division = _branchOffice.Division?.code ?? "";
					_processOutput.code_branchOffice = _branchOffice.code;
					_processOutput.id_emisionPoint = _emissionPoint.id;
					_processOutput.code_company = _emissionPoint.BranchOffice.Division.Company.code;

					_processOutput.code_documentType = documentType.code;
					_processOutput.documentNumber = _document.id;

					//_processOutput.documentNumber = _document.sequential;
					_processOutput.descriptionGloss =
						"Lote Int.:" + integrationProcessLote.codeLote + "," +
						"Guias Remision:" + guiasRemision + "," +
						"Compañía Transporte:" + outputAux.textoObjectParent1 + "," +
						"Propietario:" + outputAux.textoObjectParent2 + "," +
						"Placa:" + outputAux.textoObjectParent3;

					_processOutput.emisionDate = integrationProcessLote.dateAccounting;//  _document.emissionDate;
					_processOutput.realDate = integrationProcessLote.dateAccounting;  //_document.emissionDate;
					_processOutput.initDate = integrationProcessLote.dateAccounting;  //_document.emissionDate;
					_processOutput.documentNumberLegal = _document.number;
					_processOutput.valueTotal = valorAnticipo;
					_processOutput.valueToPay = valorAnticipo;

					_processOutput.valueZeroBase = valorAnticipo;
					_processOutput.code_identificationType = codigoTipoIdentificacion;
					_processOutput.identification_number = identificacion;
					_processOutput.dateCreate = DateTime.Now;
					_processOutput.id_userCreate = id_ActiveUser;
					_processOutput.dateUpdate = DateTime.Now;
					_processOutput.userUpdate = id_ActiveUser;
					_processOutput.idStatusOutput = _stateTransmit.id;
					//_processOutput.isProcess = false;

					var maxLength = (identificacion.Length > 10) ? 10 : (identificacion.Length);

					_processOutput.auxChar1 = identificacion.Substring(0, maxLength);
					_processOutput.auxChar2 = _propietario.fullname_businessName;

					var maxLength2 = (integrationProcessLote.codeLote.Length > 10) ? 10 : (integrationProcessLote.codeLote.Length);

					_processOutput.auxChar3 = integrationProcessLote.codeLote.Substring(0, maxLength2);


					foreach (var outputAuxDetail in outputAux.ObjectChild)
					{
						_processOutput.IntegrationProcessOutputDocument.Add(
							new IntegrationProcessOutputDocument
							{
								id_IntegrationProcessDetail = outputAuxDetail.idObjectChild1,
								dateLastUpdateDocument = outputAuxDetail.fechaObjectChild1
							});
						integrationProcessLote.IntegrationProcessOutput.Add(_processOutput);
					}
				}
			}
			catch (Exception e)
			{

				msgGeneral = e.Message;
			}

			return msgGeneral;
		}

		string IIntegrationProcessActionOutput.TransferData(
			DBContext db,
			int id_ActiveUser,
			int id_IntegrationProcessLote,
			List<IntegrationProcessOutput> _integrationProcessOutputs,
			Func<DBContext, int, int, string, string, string, List<AdvanceParametersDetail>, int> saveMessageError)
		{
			var msgGeneral = "";

			try
			{
				var _stateTransmit = db.IntegrationState
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Transmitted);

				if (_stateTransmit == null)
				{
					throw new Exception("Estado de Integración Transmitido, no definido");
				}


				var typesMessageError = db.AdvanceParameters
					.FirstOrDefault(r => r.code.Equals(EnumIntegrationProcess.ParametersIntegration.SourceMessage))
					.AdvanceParametersDetail
					.ToList();

				if (typesMessageError == null)
				{
					throw new Exception("Parámetros de Integración: Origen de Mensaje, No definido.");
				}


				var typesMessageError2 = db.AdvanceParameters
					.FirstOrDefault(r => r.code.Equals(EnumIntegrationProcess.ParametersIntegration.SourceReference))
					.AdvanceParametersDetail
					.ToList();

				if (typesMessageError2 == null)
				{
					throw new Exception("Parámetros de Integración: Origen de Referencia, No definido.");
				}

				typesMessageError.AddRange(typesMessageError2);




				var _cabMovimientoBatchList = new List<TblCpCabProvisionOPBatch>();
				var _detMovimientoBatchList = new List<TblCpDetProvisionOPBatch>();
				var secuencialOutput = 1;

				var dbIntegration = new DBContextIntegration();

				foreach (var loteOut in _integrationProcessOutputs)
				{
					var rootMessage = " Documento de salida: " + loteOut.documentNumberLegal;
					var codeTipoDocumento = "";

					var _cabMovimiento = new TblCpCabProvisionOPBatch();
					_cabMovimiento.NIdCpProvisionOP = loteOut.id;

					_cabMovimiento.CCiCia = loteOut.code_company;
					_cabMovimiento.CCiDivision = loteOut.code_division;
					_cabMovimiento.CCiSucursal = loteOut.code_branchOffice;
					_cabMovimiento.NNuProvisionOP = loteOut.id;
					_cabMovimiento.FFxOrdenPago = loteOut.emisionDate.Value;

					var stringLength = loteOut.descriptionGloss?.Count() ?? 0;
					var maxLength = (stringLength == 0) ? 0 : ((stringLength > 224) ? 224 : stringLength);
					var glosaExt = (loteOut.descriptionGloss ?? "");

					_cabMovimiento.CDsDetalle = glosaExt.Substring(0, maxLength) ?? "";
					_cabMovimiento.CCiBanco = "99";
					_cabMovimiento.CCiCtaCte = "04";
					_cabMovimiento.CCiCajaBanco = "";
					_cabMovimiento.CCiCajaCtaCte = "";
					_cabMovimiento.CCeFormaPago = "E";

					switch (loteOut.code_identificationType)
					{
						case "01":
							codeTipoDocumento = "R";
							break;
						case "02":
							codeTipoDocumento = "C";
							break;
						case "03":
							codeTipoDocumento = "P";
							break;
					};

					var _proveedor = dbIntegration.TblGeProveedor
						.Where(r => r.CCiTipoIdentificacion == codeTipoDocumento
								&& r.CCiIdentificacion == loteOut.identification_number)
						.Select(r => new { r.CCiProveedor, r.CNoProveedor })
						.FirstOrDefault();

					if (_proveedor == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							id_IntegrationProcessLote,
							"No definido Proveedor con identificación: " + codeTipoDocumento + " " + loteOut.identification_number + rootMessage,
							EnumIntegrationProcess.SourceReference.LoteIntegracion,
							EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
							typesMessageError);

						throw new Exception("No definido Proveedor con identificación: " + codeTipoDocumento + " " + loteOut.identification_number + "." + rootMessage);
					}

					_cabMovimiento.CCiAuxiliar = _proveedor.CCiProveedor;
					_cabMovimiento.CCiProveedor = _proveedor.CCiProveedor;
					_cabMovimiento.NVtBruto = loteOut.valueTotal;
					_cabMovimiento.NNuDescuento = 0;
					_cabMovimiento.NVtTotal = loteOut.valueTotal.Value;

					_cabMovimiento.CCeProvisionOP = "I";
					_cabMovimiento.CCtCaja = "";
					_cabMovimiento.CCtTipoOrden = "G";
					_cabMovimiento.CDsBeneficiario = loteOut.auxChar2.Substring(0, ((loteOut.auxChar2.Length > 60) ? 60 : loteOut.auxChar2.Length));
					_cabMovimiento.CSNAnticipo = "S";
					_cabMovimiento.CDsReferencia = loteOut.auxChar3;
					_cabMovimiento.CCiMotivo = "";
					_cabMovimiento.NNuSolicitud = null;
					_cabMovimiento.NIdPrAnticipo = null;
					_cabMovimiento.CCiGrupoObra = "";
					_cabMovimiento.CDsOrigen = "PP";
					_cabMovimiento.CCiServicioPago = "";
					_cabMovimiento.CCeAnticipo = "";
					_cabMovimiento.NNuAbonadoAnticipo = 0;
					_cabMovimiento.CCtNaturaleza = "A";
					_cabMovimiento.DFxVencimiento = (DateTime)loteOut.emisionDate;
					_cabMovimiento.CCiDpto = "";
					_cabMovimiento.NIdPrPresupuesto = 0;
					_cabMovimiento.CCiUsuarioIngreso = "";
					_cabMovimiento.CCiEstacionIngreso = "";
					_cabMovimiento.DFxIngreso = DateTime.Now;
					_cabMovimiento.CCiUsuarioModifica = "";
					_cabMovimiento.CCiEstacionModifica = "";
					_cabMovimiento.DFxModifica = null;
					_cabMovimiento.CCiUsuarioAprueba = null;
					_cabMovimiento.DFxAprueba = null;
					_cabMovimiento.NNuProvisionRRHH = 0;
					_cabMovimiento.CCiCliente = "";
					_cabMovimiento.CCiUsuarioCajero = "";
					_cabMovimiento.CCiCanalVenta = "";
					_cabMovimiento.CCiVendedor = "";
					_cabMovimiento.CciTipoAnticipo = "01";
					_cabMovimiento.NIdCiComprobantePresConsumo = 0;
					_cabMovimiento.CSnReembolso = "S";

					var _cuentaProveedor = dbIntegration.TblGeCuentaProveedor
						.FirstOrDefault(r => r.CCiCia == loteOut.code_company
									&& r.CCiDivision == loteOut.code_division
									&& r.CCiSucursal == loteOut.code_branchOffice
									&& r.CCiProveedor == _proveedor.CCiProveedor
									&& r.CCtCuenta == "CP");

					if (_cuentaProveedor == null)
					{
						_cuentaProveedor = dbIntegration.TblGeCuentaProveedor
							.FirstOrDefault(r => r.CCiProveedor == _proveedor.CCiProveedor
											&& r.CCtCuenta == "CP");

						if (_cuentaProveedor == null)
						{
							saveMessageError(
								db,
								id_ActiveUser,
								id_IntegrationProcessLote,
								"Cuenta Cble. de Proveedor:" + _proveedor.CNoProveedor + " no definido." + rootMessage,
								EnumIntegrationProcess.SourceReference.LoteIntegracion,
								EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
								typesMessageError);

							throw new Exception("Cuenta Cble. de Proveedor:" + _proveedor.CNoProveedor + " no definido." + rootMessage);
						}
					}

					_cabMovimientoBatchList.Add(_cabMovimiento);


					// Detalle 1
					var _detMovimiento = new TblCpDetProvisionOPBatch();

					_detMovimiento.NIdCpProvisionOP = loteOut.id;
					_detMovimiento.NNuRegistro = 1;
					_detMovimiento.CCiCuenta = "11020201003";

					var _cuenta = dbIntegration.TblciCuenta
						.Where(r => r.CCiCuenta == "11020201003")
						.Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
						.FirstOrDefault();

					if (_cuenta == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							id_IntegrationProcessLote,
                            "Cuenta 11020201003 No definida" + rootMessage,
							EnumIntegrationProcess.SourceReference.LoteIntegracion,
							EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
							typesMessageError);

						throw new Exception("Cuenta 11020201003  No definida." + rootMessage);
					}

					if (_cuenta.BSnAceptaAux == true)
					{
						_detMovimiento.CCiTipoAuxiliar = _cuentaProveedor.CCiTipoAuxiliar;
						_detMovimiento.CCiAuxiliar = _cuentaProveedor.CCiAuxiliar;
					}
					else
					{
						_detMovimiento.CCiTipoAuxiliar = "";
						_detMovimiento.CCiAuxiliar = "";
					}

					_detMovimiento.CCiDpto = "";
					_detMovimiento.CCiProyecto = "";
					_detMovimiento.CCiSubProyecto = "";
					_detMovimiento.NNuDebito = (decimal)loteOut.valueTotal;
					_detMovimiento.NNuCredito = 0;
					_detMovimiento.CDsDetalle = glosaExt.Substring(0, maxLength) ?? "";
					_detMovimiento.CTxReservadoUsuario = "";
					_detMovimiento.NIdPrPresupuesto = 0;

					_detMovimientoBatchList.Add(_detMovimiento);



					loteOut.dateUpdate = DateTime.Now;
					loteOut.userUpdate = id_ActiveUser;
					loteOut.idStatusOutput = _stateTransmit.id;


					#region TblGeIntegracionControl

					var _tblGeIntegracionControl = new TblGeIntegracionControl
					{
						CCiCia = loteOut.code_company,
						CCiDivision = loteOut.code_division,
						CCiSucursal = loteOut.code_branchOffice,

						NNuAnio = (short)loteOut.emisionDate.Value.Year,
						NNuPeriodo = (short)loteOut.emisionDate.Value.Month,
						CodOrigen = "PP",
						TipoComprobante = "PV",
						CodTipoComprobante = loteOut.code_documentType,
						CodLoteOutput = loteOut.id,
						FechaIntegracion = DateTime.Now,
						CodEstado = EnumIntegrationProcess.States.Transmitted,
						Procesado = false

					};

					dbIntegration.TblGeIntegracionControl.Add(_tblGeIntegracionControl);

					#endregion

					secuencialOutput++;
				}

				using (var transaction = dbIntegration.Database.BeginTransaction())
				{
					try
					{
						foreach (var _cabBatch in _cabMovimientoBatchList)
						{
							dbIntegration.TblCpCabProvisionOPBatch.Add(_cabBatch);
						}

						foreach (var _detBatch in _detMovimientoBatchList)
						{
							dbIntegration.TblCpDetProvisionOPBatch.Add(_detBatch);
						}

						dbIntegration.SaveChanges();
						transaction.Commit();
					}
					catch
					{
						transaction.Rollback();
						transaction.Dispose();

						throw;
					}
				}

			}
			catch (Exception e)
			{
				msgGeneral = e.Message;
			}

			return msgGeneral;
		}

		private List<IntegrationProcessOutputAux> GroupData(
			DBContext db,
			List<IntegrationProcessDetail> integrationProcessDetailList)
		{
			var _outputAux = new List<IntegrationProcessOutputAux>();

			foreach (var processDetail in integrationProcessDetailList)
			{
				var document = db.Document
					.FirstOrDefault(r => r.id == processDetail.id_Document);
				if (document == null)
				{
					throw new Exception("Documento no Definido");
				}

				var _advancedTransportist = db.RemissionGuideCustomizedAdvancedTransportist
					.FirstOrDefault(r => r.id_AdvancedTransportist == processDetail.id_Document);
				if (_advancedTransportist == null)
				{
					throw new Exception("Información de Anticipo de Transportista, No definido.");
				}

				var _remissionGuideTransportation = db.RemissionGuideTransportation
					.FirstOrDefault(r => r.id_remionGuide == _advancedTransportist.id_RemissionGuide);
				if (_remissionGuideTransportation == null)
				{
					throw new Exception("Información de Anticipo de Transportista en Guía de Remisión, No definido.");
				}

				var personCompaniaTransporte = _remissionGuideTransportation
					.VehicleProviderTransportBilling?.Person;
				if (personCompaniaTransporte == null)
				{
					throw new Exception("Compañía de Transporte, No definido.");
				}


				var idCompaniaTransporte = personCompaniaTransporte.id;
				var nombreCompaniaTransporte = personCompaniaTransporte.fullname_businessName;


				var _vehiculo = _remissionGuideTransportation.Vehicle;
				if (_vehiculo == null)
				{
					throw new Exception("Vehículo, No definido.");
				}
				var placaVehículo = _vehiculo.carRegistration;


				var personPropietario = _remissionGuideTransportation.Vehicle?.Person;
				if (personPropietario == null)
				{
					throw new Exception("Propietario, No definido.");
				}
				var idPropietario = personPropietario.id;
				var nombrePropietario = personPropietario.fullname_businessName;


				var _integrationProcessOutputAux = _outputAux
					.FirstOrDefault(s => s.idObjectParent1 == idCompaniaTransporte
									&& s.idObjectParent2 == idPropietario);
				if (_integrationProcessOutputAux == null)
				{
					_integrationProcessOutputAux = new IntegrationProcessOutputAux()
					{
						idObjectParent1 = idCompaniaTransporte,
						idObjectParent2 = idPropietario,
						idObjectParent3 = processDetail.id,
						idObjectParent4 = processDetail.id_Document,
						textoObjectParent1 = nombreCompaniaTransporte,
						textoObjectParent2 = nombrePropietario,
						textoObjectParent3 = placaVehículo,
					};

					_outputAux.Add(_integrationProcessOutputAux);
				}

				var _integrationProcessOutputAuxDetail = new IntegrationProcessOutputAuxDetail()
				{
					idObjectChild1 = processDetail.id,
					idObjectChild2 = _advancedTransportist.id_RemissionGuide,
					fechaObjectChild1 = document.dateUpdate.Date,
					decimalObjectChild1 = _advancedTransportist.valueTotal,
				};

				_integrationProcessOutputAux.ObjectChild.Add(_integrationProcessOutputAuxDetail);
			}

			return _outputAux;
		}
	}
}