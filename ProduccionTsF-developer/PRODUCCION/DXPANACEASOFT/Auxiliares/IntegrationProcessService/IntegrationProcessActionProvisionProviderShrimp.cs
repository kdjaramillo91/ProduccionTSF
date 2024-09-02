using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessActionProvisionProviderShrimp : IIntegrationProcessActionOutput
	{
		string IIntegrationProcessActionOutput.FindDocument(
			DBContext db,
			tbsysIntegrationDocumentConfig integrationConfig,
			ref IEnumerable<Document> preDocument)
		{
			var msgGeneral = "";

			try
			{
				var codeStatusValidate1 = integrationConfig
					.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault()?.valueDirectValidate;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Provisión pago proveedor camarón, Integración No Definida");
				}

				var idLotesCompraValidados = db.ProductionLot
					.Where(r => r.ProductionLotState.code == codeStatusValidate1
							&& r.totalToPay > 0)
					.Select(r => r.id)
					.ToList();

				preDocument = preDocument
					.Where(d => idLotesCompraValidados.Contains(d.id))
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
			var _document = db.Document
				.FirstOrDefault(r => r.id == id_document);
			var _integrationAux = this.GetInfoLiquidation(_document);

			var numeroLote = _integrationAux.textoObjectParent1;
			var numeroLiquidacionCamaron = _integrationAux.textoObjectParent2;
			var fechaLiquidacionCamaron = _integrationAux.textoObjectParent3;
			var person = db.Person
				.FirstOrDefault(r => r.id == _integrationAux.idObjectParent1);

            var proceso = db.Person
               .FirstOrDefault(r => r.id == _integrationAux.idObjectParent5);

            return new Dictionary<string, string>
			{
				{ "Proveedor",  person?.fullname_businessName ?? "" },
                { "Proceso", proceso?.processPlant ?? "" },
                { "Lote Producción", numeroLote },
				{ "Número de Liquidación", numeroLiquidacionCamaron },
				{ "Fecha Liquidación", fechaLiquidacionCamaron },
			};
		}

		decimal IIntegrationProcessActionOutput.GetTotalValue(Document document)
		{
			return document
				.Lot?
				.ProductionLot?
				.totalToPay ?? Decimal.Zero;
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
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.CodeIntegrationProcess.ProvisionProveedorCamaron);

				if (integrationDocumentConfig == null)
				{
					throw new Exception("Configuración proceso de Integración para Provisión de Pago a Proveedor de Camarón, No definido");
				}

				var codeStatusValidate1 = integrationDocumentConfig.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault(r => r.code == "VPC")?
					.valueDirectValidate;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Provisión de Pago a Proveedor, proceso de integración, No Definida");
				}

				integrationProcessDetailList.ForEach(r =>
				{
					var ierr = 0;

					var _productionLot = db.ProductionLot.FirstOrDefault(s => s.id == r.id_Document);

					if (_productionLot == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							r.id,
							"No existe relación Documento -> Lote de Producción.",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);
					}

					if (_productionLot.ProductionLotState.code != codeStatusValidate1)
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

						throw new Exception("Establecimiento  no Definido");
					}

					var _integrationAux = this.GetInfoLiquidation(_document);

					if (_integrationAux == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							"No existe información complementaria (Glosa)",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("No existe información complementaria (Glosa)");
					}

					var provisionTransportTotal = _integrationAux.decimalObjectParent1;
					var _person = db.Person
						.FirstOrDefault(r => r.id == _integrationAux.idObjectParent1);

					if (_person == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							processDetail.id,
							"Proveedor no  definido",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Proveedor no  definido");
					}

					var codeIdentificationType = _person.IdentificationType.code;
					var identification = _person.identification_number;

					_processOutput.code_division = _branchOffice.Division?.code ?? "";
					_processOutput.code_branchOffice = _branchOffice.code;
					_processOutput.id_emisionPoint = _emissionPoint.id;
					_processOutput.code_company = _emissionPoint.BranchOffice.Division.Company.code;
					_processOutput.code_documentType = documentType.code;
					_processOutput.documentNumber = _document.id;

					var infoGloss = GetProvisionProviderShrimpGloss(db, _document.id, documentType.code, _integrationAux);

					var descriptionInfoGloss = infoGloss
						.Select(r => r.Key + ": " + r.Value)
						.Aggregate((i, j) => i + "; " + j);

					_processOutput.descriptionGloss = descriptionInfoGloss;

					_processOutput.emisionDate = _document.emissionDate;
					_processOutput.realDate = _document.emissionDate;
					_processOutput.initDate = _document.emissionDate;
					_processOutput.documentNumberLegal = _document.number;
					_processOutput.valueTotal = provisionTransportTotal;
					_processOutput.valueToPay = provisionTransportTotal;


					_processOutput.valueZeroBase = provisionTransportTotal;
					_processOutput.code_identificationType = codeIdentificationType;
					_processOutput.identification_number = identification;
					_processOutput.dateCreate = DateTime.Now;
					_processOutput.id_userCreate = id_ActiveUser;
					_processOutput.dateUpdate = DateTime.Now;
					_processOutput.userUpdate = id_ActiveUser;
					_processOutput.idStatusOutput = _stateTransmit.id;


					var lengthInterNumber = _integrationAux.textoObjectParent1.Length;
					lengthInterNumber = (lengthInterNumber > 10) ? 10 : lengthInterNumber;
					_processOutput.auxChar1 = _integrationAux.textoObjectParent1.Substring(0, lengthInterNumber);
					_processOutput.auxInt1 = Convert.ToInt32(_integrationAux.textoObjectParent2);

					var _ProductionLot = db.ProductionLot
						.FirstOrDefault(r => r.id == _document.id);

					_processOutput.auxDate1 = _ProductionLot.receptionDate;
					_processOutput.auxChar2 = _ProductionLot.PriceList.GroupPersonByRol.id.ToString();

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



				var _tblCiCabMovBatchList = new List<TblCiCabMovBatch>();
				var _tblCiDetMovBatchList = new List<TblCiDetMovBatch>();

				var dbIntegration = new DBContextIntegration();
				var numeroComprobante = 0;
				var isFirstTime = true;

				foreach (var loteOut in _integrationProcessOutputs)
				{
                    var numeroRegistroHaber = 0;
                    const string tipoComprobante = "DPL";
					const string tipoAuxiliarContable = "CP";

					// Pending Obtener el numero de documento GEN_CALCULA_CONSECUTIVO_COMPROBANTE
					var rootMessage = " Documento de salida: " + loteOut.documentNumberLegal;

					if (isFirstTime)
					{
						numeroComprobante = GetExternalNumber(
							loteOut.code_company,
							loteOut.code_division,
							loteOut.code_branchOffice,
							tipoComprobante,
							loteOut.emisionDate.Value.Year,
							loteOut.emisionDate.Value.Month,
							"S",
							dbIntegration);

						isFirstTime = false;
					}
					else
					{
						numeroComprobante++;
					}


					var _tblCiCabMovBatch = new TblCiCabMovBatch();
					_tblCiCabMovBatch.CCiCia = loteOut.code_company;
					_tblCiCabMovBatch.CCiDivision = loteOut.code_division;
					_tblCiCabMovBatch.CCiSucursal = loteOut.code_branchOffice;
					_tblCiCabMovBatch.CCiTipoComprobante = tipoComprobante;
					_tblCiCabMovBatch.NNuComprobanteBatch = numeroComprobante;
					_tblCiCabMovBatch.NNuAnio = loteOut.emisionDate.Value.Year;
					_tblCiCabMovBatch.NNuPeriodo = loteOut.emisionDate.Value.Month;
					_tblCiCabMovBatch.CCiSistema = "PP";
					_tblCiCabMovBatch.DfmFechaContab = (DateTime)loteOut.emisionDate;
					_tblCiCabMovBatch.CTxReferencia = loteOut.auxChar1;

					_tblCiCabMovBatch.NNuTotDebito = (decimal)loteOut.valueTotal;
					_tblCiCabMovBatch.NNuTotCredito = (decimal)loteOut.valueTotal;
					_tblCiCabMovBatch.CCtEstadoComp = "I";
					_tblCiCabMovBatch.CCiUsuarioIngreso = "";
					_tblCiCabMovBatch.DFiFechaIngreso = (DateTime)loteOut.emisionDate;
					_tblCiCabMovBatch.CTxEstacionIng = "";

					_tblCiCabMovBatch.CCiUsuarioAprueba = null;
					_tblCiCabMovBatch.CTxEstacionAprueba = null;
					_tblCiCabMovBatch.DFcFechaAprueba = null;

					// Glosa
					var stringLength = loteOut.descriptionGloss?.Count() ?? 0;
					var maxLength = (stringLength == 0) ? 0 : ((stringLength > 224) ? 224 : stringLength);
					var glosaExt = loteOut.descriptionGloss ?? "";

					_tblCiCabMovBatch.CdsGlosa = glosaExt;
					_tblCiCabMovBatch.CCiOrigen = "PP";
					_tblCiCabMovBatch.CCiIdentificacion = (loteOut.identification_number);
					_tblCiCabMovBatch.NNuLiquidacionProduccion = loteOut.auxInt1;
					_tblCiCabMovBatch.DFxMateriaPrima = (DateTime)loteOut.auxDate1;
					_tblCiCabMovBatch.CCiGrupoEconomico = loteOut.auxChar2;

					_tblCiCabMovBatchList.Add(_tblCiCabMovBatch);


					// Informacion Cuenta y Auxiliar Proveedor
					string codeTipoDocumento = "";

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

                    var _contabPers = db.Setting.FirstOrDefault(fod => fod.code.Equals("CONTCAT"));
                    var _cuentaDebe = db.Setting.FirstOrDefault(fod => fod.code.Equals("CLIQCD"));
                    var _cuentaHaber = db.Setting.FirstOrDefault(fod => fod.code.Equals("CLIQCH"));

                    var _tblCiDetMovBatch = new TblCiDetMovBatch();
                    // Informacion de la cuenta
                    var _cuentaCab = dbIntegration.TblciCuenta
                        .Where(r => r.CCiCuenta == _cuentaDebe.value)
                        .Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
                        .FirstOrDefault();

                    if (_cuentaCab == null)
                    {
                        saveMessageError(
                            db,
                            id_ActiveUser,
                            id_IntegrationProcessLote,
                            "Cuenta " + _cuentaDebe.value + " No definida." + rootMessage,
                            EnumIntegrationProcess.SourceReference.LoteIntegracion,
                            EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
                            typesMessageError);

                        throw new Exception("Cuenta " + _cuentaDebe.value + " No definida." + rootMessage);
                    }
                    if (_contabPers.value == "SI")
                    {
                        var productionLotPayments = db.ProductionLotPayment
                                        .Where(fod => fod.id_productionLot == loteOut.documentNumber)
										.GroupBy(a => new { a.Item.id_itemType, a.Item.id_itemTypeCategory })
                                        .Select(r => new
                                        {
											idTipoProducto = r.Key.id_itemType,
                                            idCategoria = r.Key.id_itemTypeCategory,
                                            NNuDebito = r.Sum(t => t.totalToPay)
                                        })
                                        .ToList();

						var nnuRegistro = 0;

						foreach (var productionLotPayment in productionLotPayments)
						{
							nnuRegistro = nnuRegistro + 1;
							numeroRegistroHaber = nnuRegistro;
                            // Detalle -> Debe
                            _tblCiDetMovBatch = new TblCiDetMovBatch();
                            _tblCiDetMovBatch.CCiCia = loteOut.code_company;
							_tblCiDetMovBatch.CCiDivision = loteOut.code_division;
							_tblCiDetMovBatch.CCiSucursal = loteOut.code_branchOffice;
							_tblCiDetMovBatch.CCiTipoComprobante = tipoComprobante;
							_tblCiDetMovBatch.NNuComprobanteBatch = numeroComprobante;
							_tblCiDetMovBatch.NNuRegistro = nnuRegistro;
							_tblCiDetMovBatch.NNuAnio = loteOut.emisionDate.Value.Year;
							_tblCiDetMovBatch.NNuPeriodo = loteOut.emisionDate.Value.Month;
							_tblCiDetMovBatch.CCiTipoDato = "L";
							_tblCiDetMovBatch.NnuNroDocCta = 0;
							_tblCiDetMovBatch.DFmFechaDocCta = null;

							_tblCiDetMovBatch.CCiCuenta = _cuentaDebe.value;

							if (_cuentaCab.BSnAceptaAux == true)
							{
                                var itemTipoProducto = db.ItemType.FirstOrDefault(fod => fod.id == productionLotPayment.idTipoProducto).code;
                                var itemCategory = db.ItemTypeCategory.FirstOrDefault(fod => fod.id == productionLotPayment.idCategoria).code;
                                var _cuentaDebeDetalle = _cuentaDebe.SettingDetail.FirstOrDefault(fod => fod.Setting.code.Equals("CLIQCD") 
														&& fod.valueAux.Split('|')[0] == itemTipoProducto
                                                        && fod.valueAux.Split('|')[1] == itemCategory);

								_tblCiDetMovBatch.CCiTipoAuxiliar = "PPE";
								_tblCiDetMovBatch.CCiAuxiliar = _cuentaDebeDetalle != null ? _cuentaDebeDetalle.value : "";
								// _tblCiDetMovBatch.CdsDetalle = _proveedor.CNoProveedor;
								_tblCiDetMovBatch.CdsDetalle = glosaExt;
							}
							else
							{
								_tblCiDetMovBatch.CCiTipoAuxiliar = "";
								_tblCiDetMovBatch.CCiAuxiliar = "";
								_tblCiDetMovBatch.CdsDetalle = glosaExt;
							}

							_tblCiDetMovBatch.CciDpto = "";
							_tblCiDetMovBatch.CCiProyecto = "";
							_tblCiDetMovBatch.CCiSubProyecto = "";
							_tblCiDetMovBatch.NNuDebito = (Decimal)productionLotPayment.NNuDebito;
							_tblCiDetMovBatch.NNuCredito = 0;
							_tblCiDetMovBatch.NNuTercerImporte = 0;

							_tblCiDetMovBatch.CciUnidadMedida = "";

							_tblCiDetMovBatch.CTxReservadoUsuario = "";
							_tblCiDetMovBatch.DFcAuxiliar = null;
							_tblCiDetMovBatch.NnuNroDocAux = 0;
							_tblCiDetMovBatch.CCiTipoDocCta = "";
							_tblCiDetMovBatch.CCiTipoDocAux = "";
							_tblCiDetMovBatch.NIdPrPresupuesto = 0;
							_tblCiDetMovBatch.CCiCiaRRHH = null;
							_tblCiDetMovBatch.CCiDivisionRRHH = null;
							_tblCiDetMovBatch.CCiSucursalRRHH = null;
							_tblCiDetMovBatch.CCiArea = null;
							_tblCiDetMovBatch.CCiCentroCosto = null;
							_tblCiDetMovBatch.CCiSubCentroCosto = null;

							_tblCiDetMovBatchList.Add(_tblCiDetMovBatch);
						}
                    }
					else
					{
                        _tblCiDetMovBatch = new TblCiDetMovBatch();
                        _tblCiDetMovBatch.CCiCia = loteOut.code_company;
						_tblCiDetMovBatch.CCiDivision = loteOut.code_division;
						_tblCiDetMovBatch.CCiSucursal = loteOut.code_branchOffice;
						_tblCiDetMovBatch.CCiTipoComprobante = tipoComprobante;
						_tblCiDetMovBatch.NNuComprobanteBatch = numeroComprobante;
						_tblCiDetMovBatch.NNuRegistro = 1;
						_tblCiDetMovBatch.NNuAnio = loteOut.emisionDate.Value.Year;
						_tblCiDetMovBatch.NNuPeriodo = loteOut.emisionDate.Value.Month;
						_tblCiDetMovBatch.CCiTipoDato = "L";
						_tblCiDetMovBatch.NnuNroDocCta = 0;
						_tblCiDetMovBatch.DFmFechaDocCta = null;

						_tblCiDetMovBatch.CCiCuenta = _cuentaDebe.value;

						if (_cuentaCab.BSnAceptaAux == true)
						{
							_tblCiDetMovBatch.CCiTipoAuxiliar = _cuentaProveedor.CCiTipoAuxiliar;
							_tblCiDetMovBatch.CCiAuxiliar = _cuentaProveedor.CCiAuxiliar;
							// _tblCiDetMovBatch.CdsDetalle = _proveedor.CNoProveedor;
							_tblCiDetMovBatch.CdsDetalle = glosaExt;
						}
						else
						{
							_tblCiDetMovBatch.CCiTipoAuxiliar = "";
							_tblCiDetMovBatch.CCiAuxiliar = "";
							_tblCiDetMovBatch.CdsDetalle = glosaExt;
						}

						_tblCiDetMovBatch.CciDpto = "";
						_tblCiDetMovBatch.CCiProyecto = "";
						_tblCiDetMovBatch.CCiSubProyecto = "";
						_tblCiDetMovBatch.NNuDebito = (Decimal)loteOut.valueTotal;
						_tblCiDetMovBatch.NNuCredito = 0;
						_tblCiDetMovBatch.NNuTercerImporte = 0;

						_tblCiDetMovBatch.CciUnidadMedida = "";

						_tblCiDetMovBatch.CTxReservadoUsuario = "";
						_tblCiDetMovBatch.DFcAuxiliar = null;
						_tblCiDetMovBatch.NnuNroDocAux = 0;
						_tblCiDetMovBatch.CCiTipoDocCta = "";
						_tblCiDetMovBatch.CCiTipoDocAux = "";
						_tblCiDetMovBatch.NIdPrPresupuesto = 0;
						_tblCiDetMovBatch.CCiCiaRRHH = null;
						_tblCiDetMovBatch.CCiDivisionRRHH = null;
						_tblCiDetMovBatch.CCiSucursalRRHH = null;
						_tblCiDetMovBatch.CCiArea = null;
						_tblCiDetMovBatch.CCiCentroCosto = null;
						_tblCiDetMovBatch.CCiSubCentroCosto = null;

						_tblCiDetMovBatchList.Add(_tblCiDetMovBatch);
					}

                    // Detalle -> Haber      
                    _tblCiDetMovBatch = new TblCiDetMovBatch();
					_tblCiDetMovBatch.CCiCia = loteOut.code_company;
					_tblCiDetMovBatch.CCiDivision = loteOut.code_division;
					_tblCiDetMovBatch.CCiSucursal = loteOut.code_branchOffice;
					_tblCiDetMovBatch.CCiTipoComprobante = tipoComprobante;
					_tblCiDetMovBatch.NNuComprobanteBatch = numeroComprobante;
					_tblCiDetMovBatch.NNuRegistro = _contabPers.value == "SI" ? numeroRegistroHaber + 1 : 2;
                    _tblCiDetMovBatch.NNuAnio = loteOut.emisionDate.Value.Year;
					_tblCiDetMovBatch.NNuPeriodo = loteOut.emisionDate.Value.Month;
					_tblCiDetMovBatch.CCiTipoDato = "L";
					_tblCiDetMovBatch.NnuNroDocCta = 0;
					_tblCiDetMovBatch.DFmFechaDocCta = null;


					// Informacion de la Cuenta
					_cuentaCab = dbIntegration.TblciCuenta
						.Where(r => r.CCiCuenta == _cuentaHaber.value)
						.Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
						.FirstOrDefault();

					if (_cuentaCab == null)
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

					_tblCiDetMovBatch.CCiCuenta = _cuentaHaber.value;

					if (_cuentaCab.BSnAceptaAux == true)
					{
						if(_contabPers.value == "SI") 
						{
                            _tblCiDetMovBatch.CCiTipoAuxiliar = _cuentaProveedor.CCiTipoAuxiliar;
                            _tblCiDetMovBatch.CCiAuxiliar = _cuentaProveedor.CCiAuxiliar;
                        }
						else 
						{
							_tblCiDetMovBatch.CCiTipoAuxiliar = "PCC";
							_tblCiDetMovBatch.CCiAuxiliar = "PCC00PVC";
						}
						// _tblCiDetMovBatch.CdsDetalle = _proveedor.CNoProveedor;
						_tblCiDetMovBatch.CdsDetalle = glosaExt;
					}
					else
					{
						_tblCiDetMovBatch.CCiTipoAuxiliar = "";
						_tblCiDetMovBatch.CCiAuxiliar = "";
						_tblCiDetMovBatch.CdsDetalle = glosaExt;
					}

					_tblCiDetMovBatch.CciDpto = "";
					_tblCiDetMovBatch.CCiProyecto = "";
					_tblCiDetMovBatch.CCiSubProyecto = "";
					_tblCiDetMovBatch.NNuDebito = 0;
					_tblCiDetMovBatch.NNuCredito = (decimal)loteOut.valueTotal;
					_tblCiDetMovBatch.NNuTercerImporte = 0;

					_tblCiDetMovBatch.CciUnidadMedida = "";

					_tblCiDetMovBatch.CTxReservadoUsuario = "";
					_tblCiDetMovBatch.DFcAuxiliar = null;
					_tblCiDetMovBatch.NnuNroDocAux = 0;
					_tblCiDetMovBatch.CCiTipoDocCta = "";
					_tblCiDetMovBatch.CCiTipoDocAux = "";
					_tblCiDetMovBatch.NIdPrPresupuesto = 0;
					_tblCiDetMovBatch.CCiCiaRRHH = null;
					_tblCiDetMovBatch.CCiDivisionRRHH = null;
					_tblCiDetMovBatch.CCiSucursalRRHH = null;
					_tblCiDetMovBatch.CCiArea = null;
					_tblCiDetMovBatch.CCiCentroCosto = null;
					_tblCiDetMovBatch.CCiSubCentroCosto = null;

					_tblCiDetMovBatchList.Add(_tblCiDetMovBatch);


					// Pending TBCONTROL
					#region TblGeIntegracionControl

					var _tblGeIntegracionControl = new TblGeIntegracionControl
					{
						CCiCia = loteOut.code_company,
						CCiDivision = loteOut.code_division,
						CCiSucursal = loteOut.code_branchOffice,

						NNuAnio = (short)loteOut.emisionDate.Value.Year,
						NNuPeriodo = (short)loteOut.emisionDate.Value.Month,
						CodOrigen = "PP",
						TipoComprobante = tipoComprobante,
						ControlComprobante = numeroComprobante.ToString(),
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
						foreach (var _cabBatch in _tblCiCabMovBatchList)
						{
							dbIntegration.TblCiCabMovBatch.Add(_cabBatch);
						}

						foreach (var _detBatch in _tblCiDetMovBatchList)
						{
							dbIntegration.TblCiDetMovBatch.Add(_detBatch);
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

		private Dictionary<string, string> GetProvisionProviderShrimpGloss(
			DBContext db,
			int id_document,
			string code_documentType,
			IntegrationProcessOutputAux integrationAuxInfo)
		{
			var numeroLote = integrationAuxInfo.textoObjectParent1;
			var numeroLiquidacionCamaron = integrationAuxInfo.textoObjectParent2;
			var fechaLiquidacionCamaron = integrationAuxInfo.textoObjectParent3;
			var person = db.Person
				.FirstOrDefault(r => r.id == integrationAuxInfo.idObjectParent1);

			return new Dictionary<string, string>
			{
				{ "Proveedor",  person?.fullname_businessName ?? "" },
				{ "Lote Producción", numeroLote },
				{ "Número de Liquidación", numeroLiquidacionCamaron },
				{ "Fecha Liquidación", fechaLiquidacionCamaron },
			};
		}

		private IntegrationProcessOutputAux GetInfoLiquidation(Document document)
		{
			return new IntegrationProcessOutputAux
			{
				idObjectParent1 = document.Lot?.ProductionLot?.id_provider ?? 0,
				textoObjectParent1 = document.Lot?.ProductionLot?.internalNumber ?? "",
				textoObjectParent2 = document.Lot?.ProductionLot?.sequentialLiquidation?.ToString(),
				textoObjectParent3 = document.emissionDate.ToString("dd/MM/yyyy") ?? "",
                idObjectParent5 = document.Lot?.ProductionLot?.id_personProcessPlant ?? 0,
                decimalObjectParent1 = document.Lot?.ProductionLot?.totalToPay ?? 0m
			};
		}

		private int GetExternalNumber(
			string codeCompania,
			string codeDivision,
			string codeSucursal,
			string codeTipoComprobante,
			int anio,
			int periodo,
			string actualiza,
			DBContextIntegration dbIntegration)
		{
			var _numeroComprobante = new ObjectParameter("NNuComprobante", typeof(int));

			dbIntegration.GEN_CALCULA_CONSECUTIVO_COMPROBANTE(
				codeCompania,
				codeDivision,
				codeSucursal,
				codeTipoComprobante,
				anio,
				periodo,
				actualiza,
				_numeroComprobante);

			return Convert.ToInt32(_numeroComprobante.Value);
		}
	}
}