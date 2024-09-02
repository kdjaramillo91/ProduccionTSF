using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessActionOutputAdvanceProviderShrimp : IIntegrationProcessActionOutput
	{
		string IIntegrationProcessActionOutput.FindDocument(
			DBContext db,
			tbsysIntegrationDocumentConfig integrationConfig,
			ref IEnumerable<Document> preDocument)
		{
			var msgGeneral = "";

			try
			{
				var codeStatus = integrationConfig.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault()?.valueDirectValidate;

				if (codeStatus == null)
				{
					throw new Exception("Validacion de Integracion No Definida");
				}

				preDocument = preDocument
					.Where(r => r.DocumentState.code == codeStatus)
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
			var _advanceProvider = db.AdvanceProvider
				.FirstOrDefault(r => r.id == id_document);

			if (_advanceProvider == null)
			{
				return new Dictionary<string, string>();
			}

			var id_provider = _advanceProvider.id_provider ?? 0;

			var nameProvider = db.Person
				.FirstOrDefault(r => r.id == id_provider)?
				.fullname_businessName ?? "";

			var _productionLot = _advanceProvider
				.Lot?
				.ProductionLot;

			var libraRecibidas = String.Format("{0:0,0}", _advanceProvider.QuantityPoundReceived.Value);
			var precioPromedio = String.Format("{0:0.00}", _advanceProvider.valueAverage.Value);
			var dateFechaRecepcion = "";
			var secTransaccion = "";
			var internalNumber = "";
            var proceso = "";

            if (_productionLot != null)
			{
				dateFechaRecepcion = _productionLot.receptionDate.ToString("dd/MM/yyyy") ?? "";
				secTransaccion = _productionLot.number;
				internalNumber = _productionLot.internalNumber;
                proceso = _productionLot.Person1.processPlant;
            }

			return new Dictionary<string, string>
			{
				{ "Lote", internalNumber },
				{ "Proveedor", nameProvider },
                { "Proceso", proceso },
                { "Fecha Recepción", dateFechaRecepcion },
				{ "Sec.Tran.", secTransaccion },
				{ "Libras Recibidas", libraRecibidas },
				{ "Precio Promedio", precioPromedio },
			};
		}

		decimal IIntegrationProcessActionOutput.GetTotalValue(Document document)
		{
			return document
				.AdvanceProvider
				.valueAdvanceTotalRounded ?? Decimal.Zero;
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
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.CodeIntegrationProcess.AnticipoProveedorCamaron);

				if (integrationDocumentConfig == null)
				{
					throw new Exception("Configuración proceso de Integración para Anticipo de Compra de Camarón, No definido");
				}

				var codeStatusValidate1 = integrationDocumentConfig.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault(r => r.code == "VAC1")?
					.valueDirectValidate;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Anticipo de Compra de Camarón, proceso de integración, No Definida");
				}

				integrationProcessDetailList.ForEach(r =>
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
			var msgGeneral = "";

			try
			{
				var _stateTransmit = db.IntegrationState
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Create);

				foreach (IntegrationProcessDetail ProcessDetail in integrationProcessDetailList)
				{
					var internalNumber = "";

					var _processOutput = new IntegrationProcessOutput();
					_processOutput.IntegrationProcessOutputDocument = new List<IntegrationProcessOutputDocument>();

					var _document = ProcessDetail.Document;
					var _emissionPoint = _document.EmissionPoint;
					var _advanceProvider = _document.AdvanceProvider;
					var _provider = _advanceProvider.Provider?.Person;
					var _productionLot = _advanceProvider.Lot?.ProductionLot ?? null;
					if (_productionLot != null)
					{
						internalNumber = _productionLot.internalNumber;
					}

					if (_provider == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							ProcessDetail.id,
							"Proveedor no Definido",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Proveedor no Definido");
					}


					var nameProvider = _provider.fullname_businessName ?? "";
					var codigoTipoIdentificacion = _provider.IdentificationType?.code ?? "";
					var identificacion = _provider.identification_number ?? "";
					var libraRecibidas = _advanceProvider.QuantityPoundReceived.ToString();
					var precioPromedio = _advanceProvider.valueAverage.ToString();
					var valorAnticipo = _advanceProvider.valueAdvanceTotalRounded ?? 0m;

					var _branchOffice = _emissionPoint.BranchOffice;
					if (_branchOffice == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							ProcessDetail.id,
							"Establecimiento no definido",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Establecimiento  no Definido");
					}

					_processOutput.code_division = _branchOffice.Division?.code ?? "";
					_processOutput.code_branchOffice = _branchOffice.code;
					_processOutput.id_emisionPoint = _emissionPoint.id;
					_processOutput.code_company = _emissionPoint.BranchOffice.Division.Company.code;

					_processOutput.code_documentType = documentType.code;
					_processOutput.documentNumber = _document.id;
					//_processOutput.documentNumber = _document.sequential;
					_processOutput.auxInt1 = _advanceProvider.diasPlazo ?? 0;

					_processOutput.descriptionGloss =
						$"Ant.Compra Camarón.:{_document.number}, Lote Produc.:{internalNumber}, " +
						$"Lb Recibidas: {libraRecibidas}, Precio Promed.:{precioPromedio}";

					_processOutput.emisionDate = integrationProcessLote.dateAccounting;//  _document.emissionDate;
					_processOutput.realDate = integrationProcessLote.dateAccounting;  //_document.emissionDate;
					_processOutput.initDate = integrationProcessLote.dateAccounting;  //_document.emissionDate;
					_processOutput.documentNumberLegal = _document.number;
					_processOutput.valueTotal = valorAnticipo;
					_processOutput.valueToPay = valorAnticipo;

					// Null Default 0
					/*
					valueIceBase	decimal(16,6),
					valueIceTariff	decimal(5,2),
					valueIce		decimal(16,6),
					valueIvaBase	decimal(16,6),
					valueIvaPorc	decimal(5,2),
					valueIva		decimal(16,6),
					valueIvaAssets  decimal(16,6),	
					valueIvaService decimal(16,6),
					valueZeroBaseAssets decimal(16,6),
					valueZeroBaseService decimal(16,6), 
					dateProcess  datetime,
					*/
					_processOutput.valueZeroBase = valorAnticipo;
					_processOutput.code_identificationType = codigoTipoIdentificacion;
					_processOutput.identification_number = identificacion;
					_processOutput.dateCreate = DateTime.Now;
					_processOutput.id_userCreate = id_ActiveUser;
					_processOutput.dateUpdate = DateTime.Now;
					_processOutput.userUpdate = id_ActiveUser;
					_processOutput.idStatusOutput = _stateTransmit.id;
					//_processOutput.isProcess = false;

					// 
					var maxLength = (internalNumber.Length > 10) ? 10 : (internalNumber.Length);

					_processOutput.auxChar1 = internalNumber.Substring(0, maxLength);

					_processOutput.auxDate1 = _productionLot.receptionDate;
					_processOutput.auxChar2 = _advanceProvider.PriceList.GroupPersonByRol.id.ToString();


					_processOutput.IntegrationProcessOutputDocument.Add(
						new IntegrationProcessOutputDocument
						{
							id_IntegrationProcessDetail = ProcessDetail.id,
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
				var dbIntegration = new DBContextIntegration();


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


				// Tipo Cta Contrapartida
				var paramCtaPredet = dbIntegration
												.TblCiParametro
												.FirstOrDefault(r => r.CCiParametro == "CTAANTPROV")?.CTxTexto;

				// "210102011";
				string cuentaContraPartida = null;
				var ctaCtraPartidaIsGeneral = false;

				if (paramCtaPredet != null && paramCtaPredet == "N")
				{
					cuentaContraPartida = dbIntegration
						.TblCpCtasPreDefinidas
						.FirstOrDefault(r => r.CCiTipoCuenta == "AP")?.CCiCuenta.Trim();

					if (cuentaContraPartida == null)
					{
						throw new Exception("Cuenta de Contrapartida Predefinida No configurada.");
					}

					ctaCtraPartidaIsGeneral = true;
				}



				var _cabMovimientoBatchList = new List<TblCpCabMovimientoBatch>();
				var _detMovimientoBatchList = new List<TblCpDetMovimientoBatch>();

				foreach (var loteOut in _integrationProcessOutputs)
				{
					const string tipoAuxiliarContable = "CP";

					var rootMessage = " Documento de salida: " + loteOut.documentNumberLegal;
					var codeTipoDocumento = "";

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

					var codeProveedor = _proveedor.CCiProveedor.TrimStart(' ');

					var _cuentaProveedor = dbIntegration.TblGeCuentaProveedor
						.FirstOrDefault(r => r.CCiCia.Trim() == loteOut.code_company.Trim()
									&& r.CCiDivision.Trim() == loteOut.code_division.Trim()
									&& r.CCiSucursal.Trim() == loteOut.code_branchOffice.Trim()
									&& r.CCiProveedor.Trim() == codeProveedor
									&& r.CCtCuenta.Trim() == tipoAuxiliarContable);

					if (_cuentaProveedor == null)
					{
						_cuentaProveedor = dbIntegration.TblGeCuentaProveedor
							.FirstOrDefault(s => s.CCiProveedor == codeProveedor
										&& s.CCtCuenta.Trim() == tipoAuxiliarContable);

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

					// Obtener Cuenta Contrapartida
					if (!ctaCtraPartidaIsGeneral)
					{
						cuentaContraPartida = _cuentaProveedor.CCiCuenta;
					}

					var _cabMovimiento = new TblCpCabMovimientoBatch();
					_cabMovimiento.CCiCia = loteOut.code_company;
					_cabMovimiento.CCiDivision = loteOut.code_division;
					_cabMovimiento.CCiSucursal = loteOut.code_branchOffice;
					_cabMovimiento.NNUAnio = loteOut.emisionDate.Value.Year;
					_cabMovimiento.NNuPeriodo = loteOut.emisionDate.Value.Month;
					_cabMovimiento.CCiTipoComprobante = "AP";
					//_cabMovimiento.NNuComprobante = (int)LoteOut.documentNumber;
					_cabMovimiento.NNuComprobante = loteOut.id;
					_cabMovimiento.CCtTipoDoc = "AP";

					var stringLength = loteOut.descriptionGloss?.Count() ?? 0;
					var maxLength = (stringLength == 0) ? 0 : ((stringLength > 224) ? 224 : stringLength);
					var glosaExt = loteOut.descriptionGloss ?? "";

					_cabMovimiento.CDsConcepto = glosaExt.Substring(0, maxLength);
					_cabMovimiento.DFmFechaFactura = (DateTime)loteOut.emisionDate;
					_cabMovimiento.DFmFechaRecepcion = (DateTime)loteOut.emisionDate;
					_cabMovimiento.DFmFechaVcto = (DateTime)loteOut.emisionDate;
					_cabMovimiento.DFxContabiliza = loteOut.realDate;
					_cabMovimiento.DFxFechaVigencia = (DateTime)loteOut.emisionDate;
					_cabMovimiento.CTxAutorizacion = "";
					_cabMovimiento.CTxNumSerie = "";
					_cabMovimiento.CTxNumDocumento = "";

					_cabMovimiento.CTxDrawback = "N";
					_cabMovimiento.CtxInterComp = "N";
					//_cabMovimiento.CCiCodInter = null;
					//_cabMovimiento.CCiTipoComprobanteInter = null;
					//_cabMovimiento.NNuComprobanteInter = null;
					//_cabMovimiento.NNuAnioComprInter = null;
					//_cabMovimiento.NNuPeriodoComprInter = null;
					_cabMovimiento.NNuValor = (decimal)loteOut.valueTotal;
					_cabMovimiento.NNuValorAPagar = (decimal)loteOut.valueToPay;
					_cabMovimiento.NNuBaseICE = (decimal)((loteOut.valueIceBase == null) ? 0 : loteOut.valueIceBase);
					_cabMovimiento.NCiTarifaICE = (decimal)((loteOut.valueIceTariff == null) ? 0 : loteOut.valueIceTariff);
					_cabMovimiento.NNuValorICE = (decimal)((loteOut.valueIce == null) ? 0 : loteOut.valueIce);
					_cabMovimiento.NNuBaseIVA = (decimal)((loteOut.valueIvaBase == null) ? 0 : loteOut.valueIvaBase);
					_cabMovimiento.NNuPorcIva = (decimal)((loteOut.valueIvaPorc == null) ? 0 : loteOut.valueIvaPorc);
					_cabMovimiento.NNuValorIVA = (decimal)((loteOut.valueIva == null) ? 0 : loteOut.valueIva);
					_cabMovimiento.NNuBase0 = (decimal)((loteOut.valueZeroBase == null) ? 0 : loteOut.valueZeroBase);
					_cabMovimiento.NNuIvaBienes = (decimal)((loteOut.valueIvaAssets == null) ? 0 : loteOut.valueIvaAssets);
					_cabMovimiento.NNuIvaServicios = (decimal)((loteOut.valueIvaService == null) ? 0 : loteOut.valueIvaService);
					_cabMovimiento.NNuBaseCeroBienes = (decimal)((loteOut.valueZeroBaseAssets == null) ? 0 : loteOut.valueZeroBaseAssets);
					_cabMovimiento.NNuBaseCeroServicios = (decimal)((loteOut.valueZeroBaseService == null) ? 0 : loteOut.valueZeroBaseService);
					_cabMovimiento.CCtEstadoComp = "I";
					_cabMovimiento.CSNDctoInterno = "S";
					_cabMovimiento.CCtTipoCompra = "";
					_cabMovimiento.NNuDiasPlazoPago = loteOut.auxInt1;

					var _cuentaCab = dbIntegration.TblciCuenta
						.Where(r => r.CCiCuenta == cuentaContraPartida)
						.Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
						.FirstOrDefault();

					if (_cuentaCab == null)
					{
						var errMessage = "Cuenta " + cuentaContraPartida + " No definida.";

						saveMessageError(
							db,
							id_ActiveUser,
							id_IntegrationProcessLote,
							errMessage + rootMessage,
							EnumIntegrationProcess.SourceReference.LoteIntegracion,
							EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
							typesMessageError);

						throw new Exception(errMessage + rootMessage);
					}

					_cabMovimiento.CCiCuenta = cuentaContraPartida;
					_cabMovimiento.CCiProveedor = codeProveedor;

					if (_cuentaCab.BSnAceptaAux == true)
					{
						_cabMovimiento.CCiTipoAuxiliar = _cuentaProveedor.CCiTipoAuxiliar;
						_cabMovimiento.CCiAuxiliar = _cuentaProveedor.CCiAuxiliar;
					}
					else
					{
						_cabMovimiento.CCiTipoAuxiliar = "";
						_cabMovimiento.CCiAuxiliar = "";
					}

					//_cabMovimiento.CCiTipoAuxiliar = "PVC";
					//_cabMovimiento.CCiAuxiliar = _cuentaProveedor.CCiAuxiliar;

					// _cabMovimiento.CCiAuxiliar == // obtener el id del proveedor
					// tabla : tblGeProveedor lookup : CCiTipoIdentificacion + CCiIdentificacion
					/*
					   <mapfix sourcer="01" destiny="R" />	
					   <mapfix sourcer="02" destiny="C" />	
					   <mapfix sourcer="03" destiny="P" />	                     
					 */

					_cabMovimiento.CCiDpto = "";
					_cabMovimiento.CCiProyecto = "";
					_cabMovimiento.CCiSubProyecto = "";
					_cabMovimiento.CCtCaja = "";
					_cabMovimiento.CCiBanco = "";
					_cabMovimiento.CCiCtaCte = "";
					// 2018-08-03: Cambio 
					/*string referencia = "";
					string[] segmentDocuLegal = LoteOut.documentNumberLegal?.Split('-');
					if(segmentDocuLegal != null && segmentDocuLegal.Count()  > 0 )
					{
						int posicionMaxima = segmentDocuLegal.Count();
						int posicionDocumento = posicionMaxima - 1;
						referencia = segmentDocuLegal[posicionDocumento];
					}
					_cabMovimiento.CDsReferencia = referencia;*/
					_cabMovimiento.CDsReferencia = loteOut.auxChar1;
					_cabMovimiento.CCiSustento = "";
					//_cabMovimiento.CTxNumeroContrato = null;
					//_cabMovimiento.NNuTituloOneroso = null;
					//_cabMovimiento.NNuTituloGratuito = null;
					_cabMovimiento.NNuValorAdicional = 0;
					//_cabMovimiento.CCiTipoDctoModi = null;
					//_cabMovimiento.CNuSerieModi = null;
					//_cabMovimiento.CNuSecuenciaModi = null;
					//_cabMovimiento.CNuAutorizacionModi = null;
					//_cabMovimiento.DFxEmisionModi = null;
					_cabMovimiento.ccisistema = "PP";
					_cabMovimiento.CSnProvision = "S";
					_cabMovimiento.CciTipoAnticipo = "00";
					_cabMovimiento.DFxMateriaPrima = (DateTime)loteOut.auxDate1;
					_cabMovimiento.CCiGrupoEconomico = loteOut.auxChar2;


					//_cabMovimiento.NNuValor =  null;
					//_cabMovimiento.NNuValorAPagar = null;
					_cabMovimientoBatchList.Add(_cabMovimiento);

                    var _cuentaHaber = db.Setting.FirstOrDefault(fod => fod.code.Equals("CUANTPC"));

                    // Informacion de la Cuenta
                    var _cuenta = dbIntegration.TblciCuenta
                        .Where(r => r.CCiCuenta == _cuentaHaber.value)
                        .Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
                        .FirstOrDefault();

                    if (_cuenta == null)
                    {
                        saveMessageError(
                            db,
                            id_ActiveUser,
                            id_IntegrationProcessLote,
                            "Cuenta " + _cuentaHaber.value + " No definida." + rootMessage,
                            EnumIntegrationProcess.SourceReference.LoteIntegracion,
                            EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
                            typesMessageError);

                        throw new Exception("Cuenta " + _cuentaHaber.value + " No definida." + rootMessage);
                    }

                    var _detMovimiento = new TblCpDetMovimientoBatch();
					_detMovimiento.CCiCia = loteOut.code_company;
					_detMovimiento.CCiDivision = loteOut.code_division;
					_detMovimiento.CCiSucursal = loteOut.code_branchOffice;
					_detMovimiento.NNUAnio = loteOut.emisionDate.Value.Year;
					_detMovimiento.NNuPeriodo = loteOut.emisionDate.Value.Month;
					_detMovimiento.CCiTipoComprobante = "AP";
					_detMovimiento.NNuComprobante = loteOut.id; //(int)LoteOut.documentNumber;
					_detMovimiento.NNuRegistro = 1;
					_detMovimiento.CCtCuenta = "AC";
					_detMovimiento.CCiCuenta = _cuentaHaber.value;

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
					_detMovimiento.NNuDebito = (decimal)loteOut.valueTotal;
					_detMovimiento.NNuCredito = 0;
					_detMovimiento.NNuTercerImporte = 0;
					_detMovimiento.CCiUnidadMedida = "";
					_detMovimiento.CCiProyecto = "";
					_detMovimiento.CCiSubProyecto = "";
					_detMovimiento.CTxCodiRete = "";
					_detMovimiento.NNuBaseRete = 0;
					_detMovimiento.NNuPorcRete = 0;
					stringLength = loteOut.descriptionGloss?.Count() ?? 0;
					maxLength = (stringLength == 0) ? 0 : ((stringLength > 224) ? 224 : stringLength);
					_detMovimiento.CDsDetalle = loteOut.descriptionGloss?.Substring(0, maxLength) ?? "";
					//_detMovimiento.CSNIvaDistribuido = null;
					//_detMovimiento.NNuValorIvaDistribuido = null;
					_detMovimiento.CTxReservadoUsuario = "";
					_detMovimientoBatchList.Add(_detMovimiento);


					_detMovimiento = new TblCpDetMovimientoBatch();
					_detMovimiento.CCiCia = loteOut.code_company;
					_detMovimiento.CCiDivision = loteOut.code_division;
					_detMovimiento.CCiSucursal = loteOut.code_branchOffice;
					_detMovimiento.NNUAnio = loteOut.emisionDate.Value.Year;
					_detMovimiento.NNuPeriodo = loteOut.emisionDate.Value.Month;
					_detMovimiento.CCiTipoComprobante = "AP";
					_detMovimiento.NNuComprobante = loteOut.id; //(int)LoteOut.documentNumber;
					_detMovimiento.NNuRegistro = 2;
					_detMovimiento.CCtCuenta = "P";

					/* 2018-11-22
					 * Descripcion: Obtener la cuenta Contrapartida predefinida 
					 *              considerando  Parametro
					 */


					//else
					//{
					//	cuentaContraPartida = _cuentaProveedor.CCiCuenta;
					//}

					_detMovimiento.CCiCuenta = cuentaContraPartida;
					_cuenta = dbIntegration.TblciCuenta
						.Where(r => r.CCiCuenta == cuentaContraPartida)
						.Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
						.FirstOrDefault();
					if (_cuenta == null)
					{
						saveMessageError(
						   db,
						   id_ActiveUser,
						   id_IntegrationProcessLote,
						   "Cuenta " + cuentaContraPartida + " No definida" + rootMessage,
						   EnumIntegrationProcess.SourceReference.LoteIntegracion,
						   EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
						   typesMessageError);
						throw new Exception("Cuenta " + cuentaContraPartida + " No definida." + rootMessage);
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
					_detMovimiento.NNuDebito = 0;
					_detMovimiento.NNuCredito = (decimal)loteOut.valueTotal;
					_detMovimiento.NNuTercerImporte = 0;
					_detMovimiento.CCiUnidadMedida = "";
					_detMovimiento.CCiProyecto = "";
					_detMovimiento.CCiSubProyecto = "";
					_detMovimiento.CTxCodiRete = "";
					_detMovimiento.NNuBaseRete = 0;
					_detMovimiento.NNuPorcRete = 0;
					stringLength = loteOut.descriptionGloss?.Count() ?? 0;
					maxLength = (stringLength == 0) ? 0 : ((stringLength > 224) ? 224 : stringLength);
					_detMovimiento.CDsDetalle = loteOut.descriptionGloss?.Substring(0, maxLength) ?? "";
					//_detMovimiento.CSNIvaDistribuido = null;
					//_detMovimiento.NNuValorIvaDistribuido = null;
					_detMovimiento.CTxReservadoUsuario = "";
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
						TipoComprobante = "AP",
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
						foreach (var _cabBatch in _cabMovimientoBatchList)
						{
							dbIntegration.TblCpCabMovimientoBatch.Add(_cabBatch);
						}

						foreach (var _detBatch in _detMovimientoBatchList)
						{
							dbIntegration.TblCpDetMovimientoBatch.Add(_detBatch);
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
	}
}