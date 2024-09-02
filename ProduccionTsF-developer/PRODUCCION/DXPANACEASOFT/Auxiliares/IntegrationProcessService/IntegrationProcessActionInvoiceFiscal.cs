using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessActionInvoiceFiscal : IIntegrationProcessActionOutput
	{
		string IIntegrationProcessActionOutput.FindDocument(
			DBContext db,
			tbsysIntegrationDocumentConfig integrationConfig,
			ref IEnumerable<Document> preDocument)
		{
			var msgGeneral = "";

			try
			{
				// Filtramos los documentos en estado AUTORIZADO
				var idDocumentStateValidate = GetIdDocumentStateValidate(db, integrationConfig);

				preDocument = preDocument
					.Where(r => r.id_documentState == idDocumentStateValidate)
					.ToList();

				// Filtramos los documentos con estado electrónico AUTORIZADO
				var idFactElectStateValidate = GetIdFactElectStateValidate(db, integrationConfig);

				var idComprobantesAutorizados = db.ElectronicDocument
					.Where(r => r.id_electronicDocumentState == idFactElectStateValidate)
					.Select(r => r.id)
					.ToList();

				preDocument = preDocument
					.Where(d => idComprobantesAutorizados.Contains(d.id))
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
			var document = db.Document
				.FirstOrDefault(r => r.id == id_document);

			var numeroFactura = document.number;
			var fechaEmision = document.emissionDate.ToString("dd/MM/yyyy");
			var fechaAutorizacion = document.authorizationDate?.ToString("dd/MM/yyyy") ?? "ninguna";
			var cliente = document.Invoice.Person?.fullname_businessName;

			return new Dictionary<string, string>
			{
				{ "Número de Factura", numeroFactura },
				{ "Fecha de emisión", fechaEmision },
				{ "Fecha de autorización", fechaAutorizacion },
				{ "Cliente", cliente },
			};
		}

		decimal IIntegrationProcessActionOutput.GetTotalValue(Document document)
		{
			var invoice = document.Invoice;
			invoice.calculateTotales();
			invoice.calculateTotalBoxes();
			invoice.calculateTotalesInvoiceExterior();
			return invoice.valuetotalCIFTruncate;
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
				var integrationDocumentConfig = db.tbsysIntegrationDocumentConfig
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.CodeIntegrationProcess.FacturaFiscal);

				if (integrationDocumentConfig == null)
				{
					throw new Exception("Configuración proceso de Integración para Factura Fiscal no definido");
				}

				// Recuperamos los códigos de estados válidos
				var idDocumentStateValidate = GetIdDocumentStateValidate(db, integrationDocumentConfig);
				int idFactElectStateValidate = GetIdFactElectStateValidate(db, integrationDocumentConfig);

				integrationProcessDetailList.ForEach(r =>
				{
					var hasError = false;
					var ierr = 0;

					if (r.Document.id_documentState != idDocumentStateValidate)
					{
						hasError = true;
						ierr++;

						saveMessageError(
							db,
							id_ActiveUser,
							r.id,
							"Documento no tiene estado para ser Integrado",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);
					}

					var factElectronica = db.ElectronicDocument
						.Where(f => f.id == r.Document.id)
						.Select(f => new { f.id, f.id_electronicDocumentState })
						.FirstOrDefault();

					if (factElectronica == null)
					{
						hasError = true;
						ierr++;

						saveMessageError(
							db,
							id_ActiveUser,
							r.id,
							"Documento electrónico no existe",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);
					}
					else if (factElectronica.id_electronicDocumentState != idFactElectStateValidate)
					{
						hasError = true;
						ierr++;

						saveMessageError(
							db,
							id_ActiveUser,
							r.id,
							"Documento electrónico no ha sido autorizado para ser Integrado",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);
					}

					if (hasError)
					{
						msgGeneral += (String.IsNullOrEmpty(msgGeneral) ? "" : " ,") + r.Document.number + "(" + r.id_Document + ")";
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
					throw new Exception("Estado CREADO para el proceso de Integración no ha sido definido.");
				}

				foreach (var processDetail in integrationProcessDetailList)
				{
					var _processOutput = new IntegrationProcessOutput();
					_processOutput.IntegrationProcessOutputDocument = new List<IntegrationProcessOutputDocument>();


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

						throw new Exception("Establecimiento no definido");
					}


					var _invoice = _document.Invoice;
					if (_invoice == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							"Factura relacionada no ha sido encontrada",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Establecimiento no definido");
					}
					_invoice.calculateTotales();
					_invoice.calculateTotalBoxes();
					_invoice.calculateTotalesInvoiceExterior();

					var _cliente = db.Person
						.FirstOrDefault(r => r.id == _invoice.id_buyer);

					if (_cliente == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							"Cliente no definido",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Cliente no definido");
					}

					var codeIdentificationType = _cliente.IdentificationType.code;
					var identification = _cliente.identification_number;
					if (identification.Length > 13)
					{
						// Sistema de facturación de Escritorio solo soporta 13 caracteres
						identification = identification.Substring(0, 13);
					}

					_processOutput.code_division = _branchOffice.Division?.code ?? "";
					_processOutput.code_branchOffice = _branchOffice.code;
					_processOutput.id_emisionPoint = _emissionPoint.id;
					_processOutput.code_company = _emissionPoint.BranchOffice.Division.Company.code;
					_processOutput.code_documentType = documentType.code;
					_processOutput.documentNumber = _document.id;

					var infoGloss = this.BuildInvoiceFiscalGloss(db, _document, documentType.code);

					var descriptionInfoGloss = infoGloss
						.Select(r => r.Key + ": " + r.Value)
						.Aggregate((i, j) => i + "; " + j);

                    var stringGlossLength = descriptionInfoGloss?.Count() ?? 0;
                    var maxLengthGloss = (stringGlossLength == 0) ? 0 : ((stringGlossLength > 255) ? 255 : stringGlossLength);
                    _processOutput.descriptionGloss = descriptionInfoGloss.Substring(0, maxLengthGloss); ;

					_processOutput.emisionDate = _document.emissionDate;
					_processOutput.realDate = _document.emissionDate;
					_processOutput.initDate = _document.emissionDate;
					_processOutput.documentNumberLegal = _document.number;
					_processOutput.autorization = _document.authorizationNumber;
					_processOutput.auxDate1 = _document.authorizationDate;

					_processOutput.valueIvaBase = _invoice.subtotalIVA;
					_processOutput.valueZeroBase = _invoice.valuetotalCIFTruncate;
					_processOutput.valueTotal = _invoice.valuetotalCIFTruncate;
					_processOutput.valueToPay = _invoice.valuetotalCIFTruncate;

					_processOutput.valueIva = _invoice.IVA;
					_processOutput.valueIvaPorc = 0m;

					// Buscamos el porcentaje de IVA aplicado en alguno de los detalles.
					// Estructura de ESCRITORIO soporta solo un porcentaje de IVA por documento!
					foreach (var invoiceDetail in _invoice.InvoiceDetail)
					{
						var ratePercentaje = invoiceDetail
							.InvoiceDetailsTaxes
							.FirstOrDefault(t => t.Rate.percentage > 0m)?
							.Rate?
							.percentage;

						if (ratePercentaje.HasValue)
						{
							_processOutput.valueIvaPorc = ratePercentaje;
							break;
						}
					}

					_processOutput.auxDate2 = _invoice.InvoiceExterior?.etaDate;
					_processOutput.auxDate3 = _invoice.InvoiceExterior?.dateShipment;

					_processOutput.code_identificationType = codeIdentificationType;
					_processOutput.identification_number = identification;
					_processOutput.dateCreate = _document.dateCreate;
					_processOutput.id_userCreate = id_ActiveUser;
					_processOutput.dateUpdate = _document.dateUpdate;
					_processOutput.userUpdate = id_ActiveUser;
					_processOutput.idStatusOutput = _stateTransmit.id;

					// 001-001-000000006
					var documentNumberTokens = _document.number.Split('-');

					if (documentNumberTokens.Length >= 2)
					{
						_processOutput.auxChar1 = String.Join("", documentNumberTokens, 0, documentNumberTokens.Length - 1);
						_processOutput.auxChar2 = documentNumberTokens[documentNumberTokens.Length - 1];
					}
					else
					{
						_processOutput.auxChar1 = "";
						_processOutput.auxChar2 = _document.number;
					}

					_processOutput.auxChar3 = _invoice.tbsysInvoiceType.codigoExterno;
					_processOutput.auxChar4 = _invoice.InvoiceExterior.daeNumber;
					_processOutput.auxChar5 = _invoice.InvoiceExterior.daeNumber2;
					_processOutput.auxChar6 = _invoice.InvoiceExterior.daeNumber3;
					_processOutput.auxChar7 = _invoice.InvoiceExterior.daeNumber4;
					_processOutput.auxChar8 = _invoice.InvoiceExterior.shipNumberTrip;

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
					throw new Exception("Estado TRANSMITIDO para el proceso de Integración no ha sido definido.");
				}

				var typesMessageError = db.AdvanceParameters
					.FirstOrDefault(r => r.code.Equals(EnumIntegrationProcess.ParametersIntegration.SourceMessage))
					.AdvanceParametersDetail
					.ToList();

				if (typesMessageError == null)
				{
					throw new Exception("Parámetro de Integración \"Origen de Mensaje\" no ha sido definido.");
				}

				var typesMessageError2 = db.AdvanceParameters
					.FirstOrDefault(r => r.code.Equals(EnumIntegrationProcess.ParametersIntegration.SourceReference))
					.AdvanceParametersDetail
					.ToList();

				if (typesMessageError2 == null)
				{
					throw new Exception("Parámetro de Integración \"Origen de Referencia\" no ha sido definido.");
				}

				typesMessageError.AddRange(typesMessageError2);



				var _tblFaCabFacturaGralBatchList = new List<TblFaCabFacturaGralBatch>();
				var _tblFaDetFacturaGralBatchList = new List<TblFaDetFacturaGralBatch>();
				var _tblFaDetFacturaGralPlazoPagoBatchList = new List<TblFaDetFacturaGralPlazoPagoBatch>();

				var dbIntegration = new DBContextIntegration();

				var _bodegaUbicacionAux = db.SettingDetail.FirstOrDefault(fod => fod.Setting.code.Equals("TRANSBODFACT"));
				// Parámetro para tranmisión de cliente al Sistema contable
				var esConsignatario = db.Setting.Any(e => e.code == "TRCLISC" && e.value == "SI");
				//se mueve consulta por error de DataReader
				var librasPar = db.Setting.FirstOrDefault(fod => fod.code == "PTRACLIB")?.value ?? "NO";
				var getMetricUnit = db.MetricUnit.Select(m => new { m.id, m.code }).ToList();

				foreach (var loteOut in _integrationProcessOutputs)
				{
					var rootMessage = " Documento de salida: " + loteOut.documentNumberLegal;

					// Busco Cliente independiente del parámetro TRCLISC
					var invoiceCliente = db.InvoiceExterior.FirstOrDefault(e => e.id == loteOut.documentNumber);

					// Condición: Si 'esClienteExterior' es activo, la identificación es de CONSIGNATARIO
					string identificacionCliente = esConsignatario
						? db.Person.FirstOrDefault(e => e.id == invoiceCliente.id_consignee).identification_number
						: loteOut.identification_number;

					// Busco por la identificación asignada
					var _cliente = dbIntegration.TblGeCliente
						.Where(r => r.CCiIdentificacion == identificacionCliente)
						.Select(r => new { r.CCiCliente, r.CCiIdentificacion, r.CNuTelefono1, r.CCiCiudad, r.CDsDireccion, r.CCiFormaPago })
						.FirstOrDefault();

					if (_cliente == null)
					{
						var messageError = $"No se ha encontrado el cliente con el número de identificación: {loteOut.identification_number}. {rootMessage}";

						saveMessageError(
							db,
							id_ActiveUser,
							id_IntegrationProcessLote,
							messageError,
							EnumIntegrationProcess.SourceReference.LoteIntegracion,
							EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
							typesMessageError);

						throw new Exception(messageError);
					}

					var _clienteDireccion = dbIntegration.TblFaDireccionCliente
						.Where(r => r.CciCliente == _cliente.CCiCliente && r.CCiTipoDireccion == "P")
						.Select(r => new { r.CciCliente, r.CTxTelefono, r.CCiCiudad, r.CDsDireccion })
						.FirstOrDefault();

					var clienteCDsDireccion = !String.IsNullOrEmpty(_clienteDireccion?.CDsDireccion)
						? _clienteDireccion.CDsDireccion
						: _cliente.CDsDireccion;
					var clienteCCiCiudad = !String.IsNullOrEmpty(_clienteDireccion?.CCiCiudad)
						? _clienteDireccion.CCiCiudad
						: _cliente.CCiCiudad;
					var clienteCTxTelefono = !String.IsNullOrEmpty(_clienteDireccion?.CTxTelefono)
						? _clienteDireccion.CTxTelefono
						: _cliente.CNuTelefono1;

					var stringLength = loteOut.descriptionGloss?.Count() ?? 0;
					var maxLength = (stringLength == 0) ? 0 : ((stringLength > 224) ? 224 : stringLength);
					var glosaExt = loteOut.descriptionGloss ?? "";
					glosaExt = glosaExt.Substring(0, maxLength);

					var _cabFacturaFiscal = new TblFaCabFacturaGralBatch
					{
						CCiCompania = loteOut.code_company,
						CCiDivision = loteOut.code_division,
						CCiSucursal = loteOut.code_branchOffice,
						CNuSerie = loteOut.auxChar1,
						CNuSecuencia = loteOut.auxChar2,
						CNuAutorizacion = loteOut.autorization,
						CCiIdentificacion = loteOut.identification_number,
						DFxFechaFactura = (DateTime)loteOut.emisionDate,
						DFxVenceFactura = (DateTime)loteOut.emisionDate,
						CDsDireccion = clienteCDsDireccion,
						CDxObservacion = glosaExt,
						CDsReferencia = "",
						CDsReferencia2 = "",
						CDsReferencia3 = "",
						NVtBrutoBaseCero = loteOut.valueZeroBase ?? 0m,
						NNuValDsctoBaseCero = 0m,
						NVtBrutoBaseIva = loteOut.valueIvaBase ?? 0m,
						NNuValDsctoBaseIva = 0m,
						NNuSubTotalNeto = loteOut.valueTotal ?? 0m,
						NNuPorcIVA = loteOut.valueIvaPorc ?? 0m,
						NVtValorIVA = loteOut.valueIva ?? 0m,
						NVtTotalFactura = loteOut.valueToPay ?? 0m,
						DFxIngreso = loteOut.dateCreate,
						CCiUsuarioIngreso = "",
						CDsEstacionIngreso = "",
						CCiVendedor = null, // TODO: Definir modelo de homologación de códigos de vendedores
						CCiFormaPago = _cliente.CCiFormaPago,
						CCiCiudad = clienteCCiCiudad,
						CTxTelefono = clienteCTxTelefono,
						NNuControlFactura = 0,
						CCiClaveAcceso = loteOut.autorization,
						CCiPlazoPago = "", // TODO: Pendiente código de Plazo Pago
						CCtOrigen = "PR",
						CCiCuenta = null,
						CCiTipoAuxiliar = null,
						CCiAuxiliar = null,
						CCiBodega = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.value : "001",
						CCiUbicacion = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.valueAux : "01",
						DFxFechaAutorizacion = loteOut.auxDate1,
						DFxFechaETA = (DateTime)loteOut.emisionDate,
						DFxFechaBL = loteOut.auxDate3,
						CCiTipoFact = "FF", // FACTURA FISCAL
						CCiTipoDcto = loteOut.auxChar3,
						CCiBanco = "",
						CNuRefrendoDistrito = loteOut.auxChar4,
						CNuRefrendoAnio = loteOut.auxChar5,
						CNuRefrendoRegimen = loteOut.auxChar6,
						CNuRefrendoCorrelativo = loteOut.auxChar7,
						CNuTransporte = loteOut.auxChar8,
						TblFaDetFacturaGralBatch = new List<TblFaDetFacturaGralBatch>(),
					};

					_tblFaCabFacturaGralBatchList.Add(_cabFacturaFiscal);


					// Detalle -> TblFaDetFacturaGralBatch
					var invoiceDetails = db.InvoiceDetail
						.Include( x => x.InvoiceDetailsTaxes)
						.Where(d => d.id_invoice == loteOut.documentNumber && d.isActive)
						.OrderBy(d => d.id);
					short secuencia = 1;

					foreach (var invoiceDetail in invoiceDetails)
					{
						// Verificamos la existencia del producto
						var _producto = dbIntegration.TblIvProducto
							.Select(r => new { r.CCiProducto })
							.FirstOrDefault(r => r.CCiProducto == invoiceDetail.masterCode);

						if (_producto == null)
						{
							var messageError = $"No se ha encontrado el producto con el código: {invoiceDetail.masterCode}. {rootMessage}";

							saveMessageError(
								db,
								id_ActiveUser,
								id_IntegrationProcessLote,
								messageError,
								EnumIntegrationProcess.SourceReference.LoteIntegracion,
								EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
								typesMessageError);

							throw new Exception(messageError);
						}

						// Recuperamos los detalles de impuestos
						var gravaIVA = false;
						var porcentajeIVA = 0m;
						var montoIVA = 0m;

						var metricUnit = getMetricUnit.Where(m => m.id == invoiceDetail.id_metricUnit).FirstOrDefault().code; 

						var cantidad = (librasPar == "SI" && metricUnit == "Kg") 
							? Decimal.Round((invoiceDetail.amount * (decimal)2.2046), 2, MidpointRounding.AwayFromZero)
                            : Decimal.Round(invoiceDetail.amount, 2, MidpointRounding.AwayFromZero);

						var precioUnitario = (librasPar == "SI" && metricUnit == "Kg")
							? Decimal.Round((invoiceDetail.totalPriceWithoutTax / cantidad), 6, MidpointRounding.AwayFromZero)
                            : Decimal.Round(invoiceDetail.unitPrice, 6, MidpointRounding.AwayFromZero);

                        var detalleImpuestos = invoiceDetail
							.InvoiceDetailsTaxes?
							.Where(t => t.Rate.percentage > 0m);

						if (detalleImpuestos.Any())
						{
							gravaIVA = true;
							porcentajeIVA = detalleImpuestos.First()
								.Rate.percentage;
							montoIVA = detalleImpuestos.Sum(d => d.value);
						}

						var _detFacturaFiscal = new TblFaDetFacturaGralBatch
						{
							NNuControl = 0,
							TblFaCabFacturaGralBatch = _cabFacturaFiscal,
							NNuSecuencia = secuencia,
							CCiProducto = _producto.CCiProducto,
							CSnInventario = "N",
                            CCiBodega = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.value : "001",
                            CCiUbicacion = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.valueAux : "01",
                            CCiUnidadVenta = "",
							CTxRamo = "",
							NNuPoliza = "0",
							NNuFactura = "0",
							NNuEndoso = "0",
							NVtPrimaNeta = 0,
							NNuCantidad = cantidad,
							NNuPrecioUnitario_ = precioUnitario,
							NVtSubTotalBruto = Decimal.Round(invoiceDetail.totalPriceWithoutTax, 2, MidpointRounding.AwayFromZero),
							NNuPorcDscto = 0,
							NVtValorDscto = 0,
							CSnGrabaIva = gravaIVA ? "S" : "N",
							NNuPorcIva = Decimal.Round(porcentajeIVA, 2, MidpointRounding.AwayFromZero),
							NVtValorIva = Decimal.Round(montoIVA, 2, MidpointRounding.AwayFromZero),
							NVtTotalItem = Decimal.Round(invoiceDetail.total, 2, MidpointRounding.AwayFromZero),
							CDxObservacion = "",
							CCiDpto = "",
							CCiProyecto = "",
							CCiSubProyecto = "",
						};

						_cabFacturaFiscal.TblFaDetFacturaGralBatch.Add(_detFacturaFiscal);
						_tblFaDetFacturaGralBatchList.Add(_detFacturaFiscal);

						secuencia++;
					}

					//Buscamos información de la tabla de Invoice Exterior
					var invoiceExterior = db.InvoiceExterior
						.FirstOrDefault(r => r.id == loteOut.documentNumber);

					var _productoFleteInternacional = db.Setting.FirstOrDefault(fod => fod.code.Equals("CODFLEI"));
					var _productoSeguroInternacional = db.Setting.FirstOrDefault(fod => fod.code.Equals("CODSEGI"));
					var _productoGastosAduaneros = db.Setting.FirstOrDefault(fod => fod.code.Equals("CODGADU"));
					var _productoGastosTransporte = db.Setting.FirstOrDefault(fod => fod.code.Equals("CODGTRA"));

					int secuenciaProducto = secuencia;

					//verificamos si hay Flete Internacional y lo adicionamos como producto
					if (invoiceExterior != null && invoiceExterior.valueInternationalFreight != 0)
					{
						secuenciaProducto = _tblFaDetFacturaGralBatchList.Count() + 1;
						// Verificamos la existencia del producto
						if (_productoFleteInternacional == null)
						{
							var messageError = $"No se ha parametrizado el producto para Flete Internacional con el código: CODFLEI. {rootMessage}";

							throw new Exception(messageError);
						}
						var _productoFlete = dbIntegration.TblIvProducto
							.Select(r => new { r.CCiProducto })
							.FirstOrDefault(r => r.CCiProducto == _productoFleteInternacional.value);

						if (_productoFlete == null)
						{
							var messageError = $"No se ha encontrado el producto con el código: {_productoFleteInternacional.value}. {rootMessage}";

							throw new Exception(messageError);
						}
						var _detFacturaFiscalFlete = new TblFaDetFacturaGralBatch
						{
							NNuControl = 0,
							TblFaCabFacturaGralBatch = _cabFacturaFiscal,
							NNuSecuencia = secuenciaProducto,
							CCiProducto = _productoFlete.CCiProducto,
							CSnInventario = "N",
                            CCiBodega = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.value : "001",
                            CCiUbicacion = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.valueAux : "01",
                            CCiUnidadVenta = "",
							CTxRamo = "",
							NNuPoliza = "0",
							NNuFactura = "0",
							NNuEndoso = "0",
							NVtPrimaNeta = 0,
							NNuCantidad = 1,
							NNuPrecioUnitario_ = Decimal.Round(invoiceExterior.valueInternationalFreight, 6, MidpointRounding.AwayFromZero),
							NVtSubTotalBruto = Decimal.Round(invoiceExterior.valueInternationalFreight, 2, MidpointRounding.AwayFromZero),
							NNuPorcDscto = 0,
							NVtValorDscto = 0,
							CSnGrabaIva = "N",
							NNuPorcIva = 0,
							NVtValorIva = 0,
							NVtTotalItem = Decimal.Round(invoiceExterior.valueInternationalFreight, 2, MidpointRounding.AwayFromZero),
							CDxObservacion = "",
							CCiDpto = "",
							CCiProyecto = "",
							CCiSubProyecto = "",
						};

						_cabFacturaFiscal.TblFaDetFacturaGralBatch.Add(_detFacturaFiscalFlete);
						_tblFaDetFacturaGralBatchList.Add(_detFacturaFiscalFlete);
					}

					//verificamos si hay Seguro Internacional y lo adicionamos como producto
					if (invoiceExterior != null && invoiceExterior.valueInternationalInsurance != 0)
					{
						secuenciaProducto = _tblFaDetFacturaGralBatchList.Count() + 1;
						// Verificamos la existencia del producto
						if (_productoSeguroInternacional == null)
						{
							var messageError = $"No se ha parametrizado el producto para Seguro Internacional con el código: CODSEGI. {rootMessage}";

							throw new Exception(messageError);
						}
						var _productoSeguro = dbIntegration.TblIvProducto
							.Select(r => new { r.CCiProducto })
							.FirstOrDefault(r => r.CCiProducto == _productoSeguroInternacional.value);

						if (_productoSeguro == null)
						{
							var messageError = $"No se ha encontrado el producto con el código: {_productoSeguroInternacional.value}. {rootMessage}";

							throw new Exception(messageError);
						}
						var _detFacturaFiscalSeguro = new TblFaDetFacturaGralBatch
						{
							NNuControl = 0,
							TblFaCabFacturaGralBatch = _cabFacturaFiscal,
							NNuSecuencia = secuenciaProducto,
							CCiProducto = _productoSeguro.CCiProducto,
							CSnInventario = "N",
                            CCiBodega = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.value : "001",
                            CCiUbicacion = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.valueAux : "01",
                            CCiUnidadVenta = "",
							CTxRamo = "",
							NNuPoliza = "0",
							NNuFactura = "0",
							NNuEndoso = "0",
							NVtPrimaNeta = 0,
							NNuCantidad = 1,
							NNuPrecioUnitario_ = Decimal.Round(invoiceExterior.valueInternationalInsurance, 6, MidpointRounding.AwayFromZero),
							NVtSubTotalBruto = Decimal.Round(invoiceExterior.valueInternationalInsurance, 2, MidpointRounding.AwayFromZero),
							NNuPorcDscto = 0,
							NVtValorDscto = 0,
							CSnGrabaIva = "N",
							NNuPorcIva = 0,
							NVtValorIva = 0,
							NVtTotalItem = Decimal.Round(invoiceExterior.valueInternationalInsurance, 2, MidpointRounding.AwayFromZero),
							CDxObservacion = "",
							CCiDpto = "",
							CCiProyecto = "",
							CCiSubProyecto = "",
						};

						_cabFacturaFiscal.TblFaDetFacturaGralBatch.Add(_detFacturaFiscalSeguro);
						_tblFaDetFacturaGralBatchList.Add(_detFacturaFiscalSeguro);
					}

					//verificamos si hay Gastos Aduaneros y lo adicionamos como producto
					if (invoiceExterior != null && invoiceExterior.valueCustomsExpenditures != 0)
					{
						secuenciaProducto = _tblFaDetFacturaGralBatchList.Count() + 1;

						// Verificamos la existencia del producto
						if (_productoGastosAduaneros == null)
						{
							var messageError = $"No se ha parametrizado el producto para Gastos Aduaneros con el código: CODGADU. {rootMessage}";

							throw new Exception(messageError);
						}
						var _productoAduanero = dbIntegration.TblIvProducto
							.Select(r => new { r.CCiProducto })
							.FirstOrDefault(r => r.CCiProducto == _productoGastosAduaneros.value);

						if (_productoAduanero == null)
						{
							var messageError = $"No se ha encontrado el producto con el código: {_productoGastosAduaneros.value}. {rootMessage}";

							throw new Exception(messageError);
						}
						var _detFacturaFiscalAduaneros = new TblFaDetFacturaGralBatch
						{
							NNuControl = 0,
							TblFaCabFacturaGralBatch = _cabFacturaFiscal,
							NNuSecuencia = secuenciaProducto,
							CCiProducto = _productoAduanero.CCiProducto,
							CSnInventario = "N",
                            CCiBodega = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.value : "001",
                            CCiUbicacion = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.valueAux : "01",
                            CCiUnidadVenta = "",
							CTxRamo = "",
							NNuPoliza = "0",
							NNuFactura = "0",
							NNuEndoso = "0",
							NVtPrimaNeta = 0,
							NNuCantidad = 1,
							NNuPrecioUnitario_ = Decimal.Round(invoiceExterior.valueCustomsExpenditures, 6, MidpointRounding.AwayFromZero),
							NVtSubTotalBruto = Decimal.Round(invoiceExterior.valueCustomsExpenditures, 2, MidpointRounding.AwayFromZero),
							NNuPorcDscto = 0,
							NVtValorDscto = 0,
							CSnGrabaIva = "N",
							NNuPorcIva = 0,
							NVtValorIva = 0,
							NVtTotalItem = Decimal.Round(invoiceExterior.valueCustomsExpenditures, 2, MidpointRounding.AwayFromZero),
							CDxObservacion = "",
							CCiDpto = "",
							CCiProyecto = "",
							CCiSubProyecto = "",
						};

						_cabFacturaFiscal.TblFaDetFacturaGralBatch.Add(_detFacturaFiscalAduaneros);
						_tblFaDetFacturaGralBatchList.Add(_detFacturaFiscalAduaneros);
					}

					if (invoiceExterior != null && invoiceExterior.valueTransportationExpenses != 0)
					{
							secuenciaProducto = _tblFaDetFacturaGralBatchList.Count() + 1;

						// Verificamos la existencia del producto
						if (_productoGastosTransporte == null)
						{
							var messageError = $"No se ha parametrizado el producto para Gastos Transp./Otros con el código: CODGTRA. {rootMessage}";

							throw new Exception(messageError);
						}
						var _productoTransporte = dbIntegration.TblIvProducto
							.Select(r => new { r.CCiProducto })
							.FirstOrDefault(r => r.CCiProducto == _productoGastosTransporte.value);

						if (_productoTransporte == null)
						{
							var messageError = $"No se ha encontrado el producto con el código: {_productoGastosTransporte.value}. {rootMessage}";

							throw new Exception(messageError);
						}
						var _detFacturaFiscalTransporte = new TblFaDetFacturaGralBatch
						{
							NNuControl = 0,
							TblFaCabFacturaGralBatch = _cabFacturaFiscal,
							NNuSecuencia = secuenciaProducto,
							CCiProducto = _productoTransporte.CCiProducto,
							CSnInventario = "N",
                            CCiBodega = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.value : "001",
                            CCiUbicacion = _bodegaUbicacionAux != null ? _bodegaUbicacionAux.valueAux : "01",
                            CCiUnidadVenta = "",
							CTxRamo = "",
							NNuPoliza = "0",
							NNuFactura = "0",
							NNuEndoso = "0",
							NVtPrimaNeta = 0,
							NNuCantidad = 1,
							NNuPrecioUnitario_ = Decimal.Round(invoiceExterior.valueTransportationExpenses, 6, MidpointRounding.AwayFromZero),
							NVtSubTotalBruto = Decimal.Round(invoiceExterior.valueTransportationExpenses, 2, MidpointRounding.AwayFromZero),
							NNuPorcDscto = 0,
							NVtValorDscto = 0,
							CSnGrabaIva = "N",
							NNuPorcIva = 0,
							NVtValorIva = 0,
							NVtTotalItem = Decimal.Round(invoiceExterior.valueTransportationExpenses, 2, MidpointRounding.AwayFromZero),
							CDxObservacion = "",
							CCiDpto = "",
							CCiProyecto = "",
							CCiSubProyecto = "",
						};

						_cabFacturaFiscal.TblFaDetFacturaGralBatch.Add(_detFacturaFiscalTransporte);
						_tblFaDetFacturaGralBatchList.Add(_detFacturaFiscalTransporte);
					}

					// Plazo de Pago -> TblFaDetFacturaGralPlazoPagoBatch
					var invoicePaymentTerms = db.InvoiceExteriorPaymentTerm
						.Where(d => d.idInvoiceExterior == loteOut.documentNumber)
						.OrderBy(d => d.orderPayment);

					foreach (var invoicePaymentTerm in invoicePaymentTerms)
					{
						var _detFacturaPlazoFiscal = new TblFaDetFacturaGralPlazoPagoBatch
						{
							NNuControl = 0,
                            TblFaCabFacturaGralBatch = _cabFacturaFiscal,
							NNuSecuencia = secuencia,
							DFxVencimiento = invoicePaymentTerm.dueDate,
							NNuPorcentaje = Decimal.Round(invoicePaymentTerm.porcentaje ?? 0m, 2, MidpointRounding.AwayFromZero),
							NNuTotalxPagar = Decimal.Round(invoicePaymentTerm.valuePayment, 2, MidpointRounding.AwayFromZero),
						};

						_cabFacturaFiscal.TblFaDetFacturaGralPlazoPagoBatch.Add(_detFacturaPlazoFiscal);
						_tblFaDetFacturaGralPlazoPagoBatchList.Add(_detFacturaPlazoFiscal);

						secuencia++;
					}


					#region TblGeIntegracionControl

					var _tblGeIntegracionControl = new TblGeIntegracionControl
					{
						CCiCia = loteOut.code_company,
						CCiDivision = loteOut.code_division,
						CCiSucursal = loteOut.code_branchOffice,

						NNuAnio = (short)loteOut.emisionDate.Value.Year,
						NNuPeriodo = (short)loteOut.emisionDate.Value.Month,
						CodOrigen = "PR",
						TipoComprobante = "FF",
						CodTipoComprobante = loteOut.code_documentType,
						CodLoteOutput = loteOut.id,
						FechaIntegracion = DateTime.Now,
						CodEstado = EnumIntegrationProcess.States.Transmitted,
						Procesado = false
					};

					dbIntegration.TblGeIntegracionControl.Add(_tblGeIntegracionControl);

					#endregion
				}

				using (var transaction = dbIntegration.Database.BeginTransaction())
				{
					try
					{
						foreach (var _cabFaBatch in _tblFaCabFacturaGralBatchList)
						{
							dbIntegration.TblFaCabFacturaGralBatch.Add(_cabFaBatch);
						}

						foreach (var _detFaBatch in _tblFaDetFacturaGralBatchList)
						{
							dbIntegration.TblFaDetFacturaGralBatch.Add(_detFaBatch);
						}

						foreach (var _detFaPlazoPagoBatch in _tblFaDetFacturaGralPlazoPagoBatchList)
						{
							dbIntegration.TblFaDetFacturaGralPlazoPagoBatch.Add(_detFaPlazoPagoBatch);
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

				msgGeneral = (e.InnerException?.Message ?? e.Message);
			}

			return msgGeneral;
		}

		private int GetIdDocumentStateValidate(DBContext db, tbsysIntegrationDocumentConfig integrationConfig)
		{
			var codeDocumentStateValidate = integrationConfig
				.tbsysIntegrationDocumentValidationConfig
				.FirstOrDefault(r => r.code == "VFAF1")?
				.valueDirectValidate;

			if (codeDocumentStateValidate == null)
			{
				throw new Exception("Código de estado de factura para integrar NO definido");
			}

			var idDocumentStateValidate = db.DocumentState
				.FirstOrDefault(r => r.code == codeDocumentStateValidate)?
				.id ?? 0;

			if (idDocumentStateValidate <= 0)
			{
				throw new Exception("ID de estado de factura para integrar NO definido");
			}

			return idDocumentStateValidate;
		}

		private int GetIdFactElectStateValidate(DBContext db, tbsysIntegrationDocumentConfig integrationConfig)
		{
			var codeFactElectStateValidate = integrationConfig
				.tbsysIntegrationDocumentValidationConfig
				.FirstOrDefault(r => r.code == "VFAF2")?
				.valueDirectValidate;

			if (codeFactElectStateValidate == null)
			{
				throw new Exception("Código de estado de autorización electrónica de factura para integrar NO definido");
			}

			var idFactElectStateValidate = db.ElectronicDocumentState
				.FirstOrDefault(r => r.code == codeFactElectStateValidate)?
				.id ?? 0;

			if (idFactElectStateValidate <= 0)
			{
				throw new Exception("ID de estado de autorización electrónica de factura para integrar NO definido");
			}

			return idFactElectStateValidate;
		}

		private Dictionary<string, string> BuildInvoiceFiscalGloss(
			DBContext db,
			Document document,
			string code_documentType)
		{
			var tipoFactura = document.Invoice.tbsysInvoiceType.name;
			var numeroFactura = document.number;
			var fechaEmision = document.emissionDate.ToString("dd/MM/yyyy");
			var cliente = db.Person
				.FirstOrDefault(r => r.id == document.Invoice.id_buyer)
				.fullname_businessName;
			var fechaAutorizacion = document.authorizationDate?.ToString("dd/MM/yyyy") ?? "ninguna";
			var observacion = document.description;

			var docRelated = db.Document.Where(x => x.id == document.id_documentOrigen && x.id_documentType == 1147).Select(x => x.number).FirstOrDefault();
			
			return new Dictionary<string, string>
			{
				{ "Tipo", tipoFactura },
				{ "Número de Factura", numeroFactura },
				{ "Fecha de emisión", fechaEmision },
				{ "Cliente", cliente },
				{ "Fecha de autorización", fechaAutorizacion },
				{ "Observación", observacion },
				{ docRelated != null? "Proforma" : "", docRelated ?? ""},
			};
		}
	}
}