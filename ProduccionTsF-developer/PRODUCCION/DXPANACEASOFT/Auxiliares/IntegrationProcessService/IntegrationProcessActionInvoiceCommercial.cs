using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessActionInvoiceCommercial : IIntegrationProcessActionOutput
	{
		string IIntegrationProcessActionOutput.FindDocument(
			DBContext db,
			tbsysIntegrationDocumentConfig integrationConfig,
			ref IEnumerable<Document> preDocument)
		{
			var msgGeneral = "";

			try
			{
				// TODO: Elaborar la extracción de datos...
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
			// TODO: Generar la glosa del documento...
			return new Dictionary<string, string>();
		}

		decimal IIntegrationProcessActionOutput.GetTotalValue(Document document)
		{
			return document
				.InvoiceCommercial
				.totalValue;
		}

		List<IntegrationProcessPrintGroup> IIntegrationProcessActionOutput.PrintGroup(
			DBContext db,
			List<IntegrationProcessDetail> integrationProcessDetailList)
		{
			return new List<IntegrationProcessPrintGroup>();
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
				var _integrationDocumentConfig = db.tbsysIntegrationDocumentConfig
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.CodeIntegrationProcess.FacturaComercial);

				if (_integrationDocumentConfig == null)
				{
					throw new Exception("Configuración proceso de Integración para Factura Comercial, No definido");
				}

				var codeStatusValidate1 = _integrationDocumentConfig
					.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault(r => r.code == "VFC")?.valueDirectValidate ?? null;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Factura Comercial, proceso de integración, No Definida");
				}

				integrationProcessDetailList
					.ForEach(r =>
					{
						var ierr = 0;

						if (r.Document.DocumentState.code != codeStatusValidate1)
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
			string msgGeneral = "";

			try
			{
				string codeCompany = null;
				string codeDivision = null;
				string codeSucursal = null;
				string codeSistemaDestino = null;

				var _stateTransmit = db.IntegrationState
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Create);

				var idsInvoiceCommercial = integrationProcessDetailList
					.Select(r => r.id_Document)
					.ToArray();

				var invoiceCommercialParam = db.AdvanceParameters
					.FirstOrDefault(r => r.code == EnumAdvanceParameter.Codes.IntegrationProcessInvoiceCommercialParams)?
					.AdvanceParametersDetail
					.ToList();

				if (invoiceCommercialParam != null)
				{
					codeCompany = invoiceCommercialParam?
						.FirstOrDefault(r => r.valueCode == EnumAdvanceParameter.IntegrationProcessInvoiceCommercialParamsCodes.CodigoCompania)?
						.valueVarchar;

					codeDivision = invoiceCommercialParam?
						.FirstOrDefault(r => r.valueCode == EnumAdvanceParameter.IntegrationProcessInvoiceCommercialParamsCodes.CodigoDivision)?
						.valueVarchar;

					codeSucursal = invoiceCommercialParam?
						.FirstOrDefault(r => r.valueCode == EnumAdvanceParameter.IntegrationProcessInvoiceCommercialParamsCodes.CodigoSucursal)?
						.valueVarchar;

					codeSistemaDestino = invoiceCommercialParam?
						.FirstOrDefault(r => r.valueCode == EnumAdvanceParameter.IntegrationProcessInvoiceCommercialParamsCodes.SistemaDestino)?
						.valueVarchar;
				}

				if (invoiceCommercialParam == null
					 || String.IsNullOrEmpty(codeCompany)
					 || String.IsNullOrEmpty(codeDivision)
					 || String.IsNullOrEmpty(codeSucursal)
					 || String.IsNullOrEmpty(codeSistemaDestino)
					 )
				{
					var errMessage = "Parametros de Integración de Factura Comercial no Definidos";
					saveMessageError(
						db,
						id_ActiveUser,
						integrationProcessLote.id,
						errMessage,
						EnumIntegrationProcess.SourceReference.LoteIntegracion,
						EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
						_parametersDetail);

					throw new Exception(errMessage);
				}


				var allGloss = this.BuildAllInvoiceCommercialGloss(idsInvoiceCommercial);

				var integracionBancos = db.IntegrationProcessForeignCodeTable
					.FirstOrDefault(r => r.codeTable == EnumIntegrationProcess.ForeingTables.Bancos)
					.IntegrationProcessForeignCodeValue
					.ToList();

				var integracionFormasPago = db.IntegrationProcessForeignCodeTable
					.FirstOrDefault(r => r.codeTable == EnumIntegrationProcess.ForeingTables.FormasPago)
					.IntegrationProcessForeignCodeValue
					.ToList();

				var integracionPlazosPago = db.IntegrationProcessForeignCodeTable
					.FirstOrDefault(r => r.codeTable == EnumIntegrationProcess.ForeingTables.PlazosPago)
					.IntegrationProcessForeignCodeValue
					.ToList();

				var integracionVendedor = db.IntegrationProcessForeignCodeTable
					.FirstOrDefault(r => r.codeTable == EnumIntegrationProcess.ForeingTables.Vendedores)
					.IntegrationProcessForeignCodeValue
					.ToList();

				if (integracionBancos == null || integracionFormasPago == null
					|| integracionPlazosPago == null || integracionVendedor == null)
				{
					var errMessage = "Una o varias tablas de integración de Factura Comercial no han sido definidas";

					saveMessageError(
						db,
						id_ActiveUser,
						integrationProcessLote.id,
						errMessage,
						EnumIntegrationProcess.SourceReference.LoteIntegracion,
						EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
						_parametersDetail);

					throw new Exception(errMessage);
				}

				foreach (var processDetail in integrationProcessDetailList)
				{
					var _processOutput = new IntegrationProcessOutput
					{
						IntegrationProcessOutputDocument = new List<IntegrationProcessOutputDocument>()
					};

					var _document = processDetail.Document;
					var _emissionPoint = _document.EmissionPoint;


					var _branchOffice = _emissionPoint.BranchOffice;
					if (_branchOffice == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							"Establecimiento no definido",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Establecimiento  no Definido");
					}


					var _invoiceCommercial = db.InvoiceCommercial
						.FirstOrDefault(r => r.id == processDetail.id_Document);
					if (_invoiceCommercial == null)
					{
						var errMessage = "Factura Comercial no encontrada";
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							errMessage,
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception(errMessage);
					}


					var _person = db.Person
						.FirstOrDefault(r => r.id == _invoiceCommercial.id_ForeignCustomer);
					if (_person == null)
					{
						var errMessage = "Cliente del exterior no Definido";
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							errMessage,
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception(errMessage);
					}


					var codeIdentificationType = _person.IdentificationType.code;
					var identification = _person.identification_number;
					var direccion = _person.address;
					var invoiceCommercialTotal = _invoiceCommercial.totalValue;


					_processOutput.code_company = codeCompany;
					_processOutput.code_division = codeDivision;
					_processOutput.code_branchOffice = codeSucursal;
					_processOutput.id_emisionPoint = _emissionPoint.id;

					_processOutput.code_documentType = documentType.code;
					_processOutput.documentNumber = _document.id;

					int maxLenGloss = processDetail.glossData.Length;
					_processOutput.descriptionGloss = processDetail.glossData.Substring(0, (maxLenGloss >= 255 ? 255 : maxLenGloss));

					_processOutput.emisionDate = _document.emissionDate;   //  FECHA DE EMISION
					_processOutput.realDate = _document.emissionDate;      //  FECHA DE AUTORIZACION
																		   //_processOutput.initDate = _document.authorizationDate; //  FECHA PRIMER VENCIMIENTO

					_processOutput.autorization = _document.authorizationNumber;
					_processOutput.documentNumberLegal = _document.number;

					_processOutput.valueTotal = invoiceCommercialTotal;
					_processOutput.valueToPay = invoiceCommercialTotal;
					_processOutput.valueZeroBase = invoiceCommercialTotal;

					_processOutput.code_identificationType = codeIdentificationType;
					_processOutput.identification_number = identification;

					_processOutput.dateCreate = DateTime.Now;
					_processOutput.id_userCreate = id_ActiveUser;
					_processOutput.dateUpdate = DateTime.Now;
					_processOutput.userUpdate = id_ActiveUser;
					_processOutput.idStatusOutput = _stateTransmit.id;


					//int lenInterNumber = _integrationAux.textoObjectParent1.Length;
					//lenInterNumber = (lenInterNumber > 10) ? 10 : lenInterNumber;
					//_processOutput.auxChar1 = _integrationAux.textoObjectParent1.Substring(0, lenInterNumber);
					// Vendedor 
					/*
					 * Obtener el Id de person y buscar en la tabla respectiva                                          
					 */
					//_processOutput.auxChar1 =
					var idVendor = _invoiceCommercial.idVendor;
					if (idVendor == null)
					{
						var errMessage = "Vendedor no Definido";
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							errMessage,
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception(errMessage);
					}

					var codeVendedorF = integracionVendedor
						.FirstOrDefault(r => r.codeForeignSystem == codeSistemaDestino
									&& r.codeSource == idVendor.ToString()
									&& (bool)r.isActive)?
						.codeForeign;

					if (codeVendedorF == null)
					{
						var errMessage = "Relacion Vendedor con Sistema Destino no definido";
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							errMessage,
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception(errMessage);
					}

					_processOutput.auxChar1 = codeVendedorF;


					var codeFormaPago = db.PaymentMethod
						.FirstOrDefault(r => r.id == _invoiceCommercial.id_PaymentMethod)?
						.code;

					if (codeFormaPago == null)
					{
						var errMessage = "Forma de Pago  no Definido";
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							errMessage,
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception(errMessage);
					}

					var codeFormaPagoF = integracionFormasPago
						.FirstOrDefault(r => r.codeForeignSystem == codeSistemaDestino
											&& r.codeSource == codeFormaPago
											&& (bool)r.isActive)?
						.codeForeign;

					if (codeFormaPagoF == null)
					{
						var errMessage = "Relacion Forma de Pago con Sistema Destino no definido";
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							errMessage,
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception(errMessage);
					}

					_processOutput.auxChar2 = codeFormaPagoF;

					// 
					var codePlazoPago = db.PaymentTerm
						.FirstOrDefault(r => r.id == _invoiceCommercial.id_PaymentTerm)?
						.code;

					if (codePlazoPago == null)
					{
						var errMessage = "Plazo de Pago no Definido";
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							errMessage,
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception(errMessage);
					}

					var codePlazoPagoF = integracionFormasPago
						.FirstOrDefault(r => r.codeForeignSystem == codeSistemaDestino
									&& r.codeSource == codePlazoPago
									&& (bool)r.isActive)?
						.codeForeign;

					if (codeFormaPagoF == null)
					{
						var errMessage = "Relacion Plazo de Pago con Sistema Destino no definido";
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							errMessage,
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception(errMessage);
					}
					_processOutput.auxChar3 = codeFormaPagoF;


					_processOutput.IntegrationProcessOutputDocument.Add(
						new IntegrationProcessOutputDocument
						{
							id_IntegrationProcessDetail = processDetail.id,
							dateLastUpdateDocument = _document.dateUpdate.Date
						});

					integrationProcessLote.IntegrationProcessOutput.Add(_processOutput);
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
			}
			catch (Exception e)
			{
				msgGeneral = e.Message;
			}

			return msgGeneral;
		}

		private List<IntegrationProcessGlossDataAuxHead> BuildAllInvoiceCommercialGloss(int[] idsDocuments)
		{
			var _infoGloss = new List<IntegrationProcessGlossDataAuxHead>();

			try
			{
				var db = new DBContext();

				// secuencial, Cliente del Exterior ,descripcion
				var _documentList = db.Document
					.Where(r => idsDocuments.Contains(r.id))
					.ToList();

				var _facturasCliente = db.InvoiceCommercial
					.Where(r => idsDocuments.Contains(r.id))
					.Select(s => new
					{
						idClienteExterior = s.id_ForeignCustomer,
						idFactura = s.id
					})
					.ToList();

				var idsForeingCustomer = _facturasCliente
					.Select(r => r.idClienteExterior)
					.ToArray();

				var _foreingCustomerForDocuments = db.Person
					.Where(r => idsForeingCustomer.Contains(r.id))
					.Select(r => new
					{
						idClienteExterior = r.id,
						nombreCliente = r.fullname_businessName
					});

				var _preInfoGloss = (
					from _document in _documentList
					join _facturaCliente in _facturasCliente
							on _document.id equals _facturaCliente.idFactura
					join _foreingCustomerForDocument in _foreingCustomerForDocuments
							on _facturaCliente.idClienteExterior equals _foreingCustomerForDocument.idClienteExterior
					select new
					{
						idDocument = _document.id,
						secuencial = _document.number,
						observacion = _document.description,
						clienteExterior = _foreingCustomerForDocument.nombreCliente
					})
					.ToList();

				_preInfoGloss.ForEach(r =>
				{
					var _dataGloss = new IntegrationProcessGlossDataAuxHead();

					_dataGloss.idDocument = r.idDocument;
					_dataGloss.infoGloss = new List<IntegrationProcessGlossDataAux>();

					_dataGloss.infoGloss.Add(new IntegrationProcessGlossDataAux
					{
						field = "Secuencial",
						valueGloss = r.secuencial
					});

					_dataGloss.infoGloss.Add(new IntegrationProcessGlossDataAux
					{
						field = "Cliente del Exterior",
						valueGloss = r.clienteExterior
					});

					_dataGloss.infoGloss.Add(new IntegrationProcessGlossDataAux
					{
						field = "Observación",
						valueGloss = r.observacion
					});

					_infoGloss.Add(_dataGloss);
				});
			}
			catch //(Exception e)
			{
			}

			return _infoGloss;
		}




		private List<IntegrationProcessGlossDataAux> GetGlossXXXXXXXXXXXXXXXXXXX(int id_document)
		{
			DBContext db = new DBContext();
			List<IntegrationProcessGlossDataAux> _infoGloss = new List<IntegrationProcessGlossDataAux>();

			try
			{

				Document _document = db.Document
										.FirstOrDefault(r => r.id == id_document);


				string secuencial = _document?.number;
				string observacion = _document?.description;

				string nombreCliente = db.InvoiceCommercial.FirstOrDefault(r => r.id == id_document)?.Person?.fullname_businessName;


				_infoGloss.Add(new IntegrationProcessGlossDataAux
				{
					field = "Secuencial",
					valueGloss = secuencial
				});

				_infoGloss.Add(new IntegrationProcessGlossDataAux
				{
					field = "Cliente del Exterior",
					valueGloss = nombreCliente
				});

				_infoGloss.Add(new IntegrationProcessGlossDataAux
				{
					field = "Observación",
					valueGloss = observacion
				});



			}
			catch //(Exception e)
			{


			}

			return _infoGloss;
		}
		private List<IntegrationProcessDetailTotal> GetAllTotalValueXXXXXXXXXXXXX(int[] idsDocument)
		{

			DBContext db = new DBContext();
			List<IntegrationProcessDetailTotal> getIntegrationProcessDetailTotal = new List<IntegrationProcessDetailTotal>();

			try
			{

				getIntegrationProcessDetailTotal = db.InvoiceCommercial
														.Where(r => idsDocument.Contains(r.id))
														.Select(r => new IntegrationProcessDetailTotal
														{
															idDocument = r.id,
															valueTotal = r.totalValue
														})
														.ToList();

			}
			catch (Exception e)
			{

				throw e;
			}


			return getIntegrationProcessDetailTotal;

		}
	}
}