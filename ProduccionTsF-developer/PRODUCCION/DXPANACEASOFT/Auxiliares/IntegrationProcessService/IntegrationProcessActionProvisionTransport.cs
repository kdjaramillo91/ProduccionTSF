using DevExpress.Utils.Extensions;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessActionProvisionTransport : IIntegrationProcessActionOutput
	{
		const string SELECT_AccountingFreightDetails = "SELECT FD.id, id_accountingFreight, accountingAccountCode,isAuxiliar,code_auxiliar, idAuxContable,\r\n" +
			"id_costCenter, id_subCostCenter, accountType, FD.id_userCreate, FD.dateCreate, FD.id_userUpdate,\r\n" +
			"FD.dateUpdate, FD.isActive\r\n " +
			"FROM AccountingFreightDetails FD\r\n " +
			"INNER JOIN AccountingFreight F ON FD.id_accountingFreight = F.Id AND FD.isActive = 1\r\n " +
			"AND F.isActive = 1\r\n " +
			"WHERE F.id_processPlant = @id_processPlant\r\n " +
			"AND F.liquidation_type = @liquidation_type";



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
			var _document = db.Document
				.FirstOrDefault(r => r.id == id_document);

			var _integrationAux = this.GetInfoLiquidation(_document);

			var idsPerson = _integrationAux.ObjectChild
				.Select(r => r.idObjectChild1)
				.ToArray();

			var _propietarios = db.Person
				.Where(r => idsPerson.Contains(r.id))
				.Select(s => s.fullname_businessName)
				.AsEnumerable()
				.Aggregate((i, j) => i + "," + j);

            var result = (from d in db.Document
                          join lf in db.LiquidationFreightRiver on d.id equals lf.id
                          join lfd in db.LiquidationFreightRiverDetail on lf.id equals lfd.id_LiquidationFreightRiver
                          join rgr in db.RemissionGuideRiver on lfd.id_remisionGuideRiver equals rgr.id
                          where d.id == id_document
                          select new
                          {
                              id_personProcessPlant = rgr.id_personProcessPlant,
                              RemisionGuide = rgr.Document.sequential

                          }).ToList();

            if (result == null || result.Count == 0)
            {
                result = (from d in db.Document
                          join lf in db.LiquidationFreight on d.id equals lf.id
                          join lfd in db.LiquidationFreightDetail on lf.id equals lfd.id_LiquidationFreight
                          join rgr in db.RemissionGuide on lfd.id_remisionGuide equals rgr.id
                          where d.id == id_document
                          select new
                          {
                              id_personProcessPlant = rgr.id_personProcessPlant,
							  RemisionGuide = rgr.Document.sequential
                          }).ToList();
            }

            var person = db.Person
               .FirstOrDefault(r => r.id == _integrationAux.idObjectParent1);

            var plantasProceso = db.Person
                .Where(p => p.isActive && p.Rol.FirstOrDefault(r => r.name.Equals("Planta Proceso")) != null)
                .Select(p => new
                {
                    p.id,
                    planta = p.identification_number,
                    name = p.fullname_businessName,
                    processPlant = p.processPlant ?? p.fullname_businessName,
                })
                .ToList();

            var numeroLiquidacion = _document.number;
			var factura = _integrationAux.textoObjectParent1;
			var nombresPropietarios = _propietarios;
			var fechaLiquidacion = _document.emissionDate.ToString("dd/MM/yyyy") ?? "";
			var tipoLiquidacion = _integrationAux.textoObjectParent2;

			var returnData = new Dictionary<string, string>
			{
                { "Proveedor", person.fullname_businessName },
                { "Factura", factura },
                { "Número de Liquidación", numeroLiquidacion },
                { "Fecha Liquidación", fechaLiquidacion },
                { "Guía de Remisión", string.Join("-",result.Select(c => c.RemisionGuide)) },
                { "Tipo", tipoLiquidacion },
				{ "Proceso", plantasProceso.Where(p => p.id == result[0].id_personProcessPlant).Select( x => x.processPlant).FirstOrDefault() }

			};

			if (factura == null)
			{
				returnData.Remove("Factura");
			}

			return returnData;
		}

		decimal IIntegrationProcessActionOutput.GetTotalValue(Document document)
		{
			if (document.LiquidationFreight != null)
			{
				return document
					.LiquidationFreight
					.pricetotal ?? Decimal.Zero;
			}

			if (document.LiquidationFreightRiver != null)
			{
				return document
					.LiquidationFreightRiver
					.pricetotal ?? Decimal.Zero;
			}

			return Decimal.Zero;
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
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.CodeIntegrationProcess.ProvisionTransportista);

				if (integrationDocumentConfig == null)
				{
					throw new Exception("Configuración proceso de Integración para Provisión de Pago a Trasportistas, No definido");
				}

				var codeStatusValidate1 = integrationDocumentConfig.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault(r => r.code == "VPT")?
					.valueDirectValidate;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Provisión de Pago a Trasportistas, proceso de integración, No Definida");
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

					var _integrationAux = GetInfoLiquidation(_document);

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

						throw new Exception("Proveedor no definido");
					}

					var codeIdentificationType = _person.IdentificationType.code;
					var identification = _person.identification_number;

					_processOutput.code_division = _branchOffice.Division?.code ?? "";
					_processOutput.code_branchOffice = _branchOffice.code;
					_processOutput.id_emisionPoint = _emissionPoint.id;
					_processOutput.code_company = _emissionPoint.BranchOffice.Division.Company.code;

					_processOutput.code_documentType = documentType.code;
					_processOutput.documentNumber = _document.id;


					var infoGloss = GetProvisionTransportGloss(db, _document.id, documentType.code, _integrationAux);

					var descriptionInfoGloss = infoGloss
						.Select(r => r.Key + ": " + r.Value)
						.Aggregate((i, j) => i + "; " + j);

                    var stringGlossLength = descriptionInfoGloss?.Count() ?? 0;
                    var maxLengthGloss = (stringGlossLength == 0) ? 0 : ((stringGlossLength > 255) ? 255 : stringGlossLength);
                    _processOutput.descriptionGloss = descriptionInfoGloss.Substring(0, maxLengthGloss);

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


					// 0001-001-000000006
					var segDoocumentNumber = _document.number.Split('-');
					if (segDoocumentNumber.Length == 3)
					{
						_processOutput.auxChar1 = segDoocumentNumber[2];
					}

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
					const string tipoComprobante = "DPF";
					const string tipoCuentaContable = "CP";
					const string tipoAuxiliar = "VAR";
					const string auxiliar = "VARI0001";


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
					_tblCiCabMovBatch.DFxMateriaPrima = DateTime.Now;
					_tblCiCabMovBatch.CCiGrupoEconomico = "";

					// Glosa
					var stringLength = loteOut.descriptionGloss?.Count() ?? 0;
					var maxLength = (stringLength == 0) ? 0 : ((stringLength > 224) ? 224 : stringLength);
					var glosaExt = loteOut.descriptionGloss ?? "";

					_tblCiCabMovBatch.CdsGlosa = glosaExt;
					_tblCiCabMovBatch.CCiOrigen = "PP";
					_tblCiCabMovBatch.CCiIdentificacion = (loteOut.identification_number);

					_tblCiCabMovBatchList.Add(_tblCiCabMovBatch);


					// Informacion Cuenta y Auxiliar Proveedor
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
									&& r.CCtCuenta.Trim() == tipoCuentaContable);

					if (_cuentaProveedor == null)
					{
						_cuentaProveedor = dbIntegration.TblGeCuentaProveedor
							.FirstOrDefault(s => s.CCiProveedor == codeProveedor
											&& s.CCtCuenta.Trim() == tipoCuentaContable);

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


					var result = (from d in db.Document
								  join lf in db.LiquidationFreightRiver on d.id equals lf.id
								  join lfd in db.LiquidationFreightRiverDetail on lf.id equals lfd.id_LiquidationFreightRiver
								  join rgr in db.RemissionGuideRiver on lfd.id_remisionGuideRiver equals rgr.id
								  where d.id == loteOut.documentNumber
								  select new
								  {
									  d.id,
									  rgr.id_personProcessPlant,
									  Liquidation_type = "F"
								  }).FirstOrDefault();

					if(result == null)
					{
                        result = (from d in db.Document
                                      join lf in db.LiquidationFreight on d.id equals lf.id
                                      join lfd in db.LiquidationFreightDetail on lf.id equals lfd.id_LiquidationFreight
                                      join rgr in db.RemissionGuide on lfd.id_remisionGuide equals rgr.id
                                      where d.id == loteOut.documentNumber
                                      select new
                                      {
                                          d.id,
                                          rgr.id_personProcessPlant,
                                          Liquidation_type = "T"
                                      }).FirstOrDefault();
                    }

                    var accountingFreightDetails = DapperConnection.Execute<AccountingFreightDetails>(SELECT_AccountingFreightDetails, new
                    {
                        id_processPlant = result.id_personProcessPlant,
                        liquidation_type = result.Liquidation_type
                    }).ToList();

					if(accountingFreightDetails.Count() == 0)
					{
                        throw new Exception("No existe una plantilla configurara para la integracion del proceso" );
                    }

					var countRegistro = 0;
					foreach (var accountD in accountingFreightDetails.Where(x => x.accountType == "D"))
					{
						var totalDebit = (decimal)loteOut.valueTotal / accountingFreightDetails.Where(x => x.accountType == "D").Count();

						countRegistro++;

                        // Detalle -> Debe
                        var _tblCiDetMovBatch = new TblCiDetMovBatch();
                        _tblCiDetMovBatch.CCiCia = loteOut.code_company;
                        _tblCiDetMovBatch.CCiDivision = loteOut.code_division;
                        _tblCiDetMovBatch.CCiSucursal = loteOut.code_branchOffice;
                        _tblCiDetMovBatch.CCiTipoComprobante = tipoComprobante;
                        _tblCiDetMovBatch.NNuComprobanteBatch = numeroComprobante;
                        _tblCiDetMovBatch.NNuRegistro = countRegistro;
                        _tblCiDetMovBatch.NNuAnio = loteOut.emisionDate.Value.Year;
                        _tblCiDetMovBatch.NNuPeriodo = loteOut.emisionDate.Value.Month;
                        _tblCiDetMovBatch.CCiTipoDato = "L";
                        _tblCiDetMovBatch.NnuNroDocCta = 0;
                        _tblCiDetMovBatch.DFmFechaDocCta = null;

                        var _cuentaDebe = accountD.accountingAccountCode;
                        var _tipoCuentaDebe = "";
                        var _auxiliarCuentaDebe = "";

                        _tipoCuentaDebe = accountD.code_Auxiliar;
                        _auxiliarCuentaDebe = accountD.idAuxContable;


                        _tblCiDetMovBatch.CCiCuenta = accountD.accountingAccountCode;

                        if (accountD.isAuxiliar)
                        {
                            _tblCiDetMovBatch.CCiTipoAuxiliar = _tipoCuentaDebe ?? tipoAuxiliar;
                            _tblCiDetMovBatch.CCiAuxiliar = _auxiliarCuentaDebe ?? auxiliar;

                            //_tblCiDetMovBatch.CdsDetalle = _proveedor.CNoProveedor;
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
                        _tblCiDetMovBatch.NNuDebito = totalDebit;
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

						if(accountD.id_costCenter == null)
						{
                            _tblCiDetMovBatch.CCiCentroCosto = null;
						}
						else
						{
							_tblCiDetMovBatch.CCiCentroCosto = db.CostCenter.Where(x => x.id == (int)accountD.id_costCenter).Select(a => a.code).FirstOrDefault();

                        }

                        if (accountD.id_subCostCenter == null)
                        {
                            _tblCiDetMovBatch.CCiCentroCosto = null;
                        }
                        else
                        {
                            _tblCiDetMovBatch.CCiCentroCosto = db.CostCenter.Where(x => x.id == (int)accountD.id_costCenter).Select(a => a.code).FirstOrDefault();

                        }
                        _tblCiDetMovBatch.CCiSubCentroCosto = null;

                        _tblCiDetMovBatchList.Add(_tblCiDetMovBatch);

                    }


                    foreach (var accountC in accountingFreightDetails.Where(x => x.accountType == "C"))
					{
                        var totalCredit = (decimal)loteOut.valueTotal / accountingFreightDetails.Where(x => x.accountType == "C").Count();
                        countRegistro++;
                        // Detalle -> Haber
                        var _tblCiDetMovBatch = new TblCiDetMovBatch();
                        _tblCiDetMovBatch.CCiCia = loteOut.code_company;
                        _tblCiDetMovBatch.CCiDivision = loteOut.code_division;
                        _tblCiDetMovBatch.CCiSucursal = loteOut.code_branchOffice;
                        _tblCiDetMovBatch.CCiTipoComprobante = tipoComprobante;
                        _tblCiDetMovBatch.NNuComprobanteBatch = numeroComprobante;
                        _tblCiDetMovBatch.NNuRegistro = countRegistro++; ;
                        _tblCiDetMovBatch.NNuAnio = loteOut.emisionDate.Value.Year;
                        _tblCiDetMovBatch.NNuPeriodo = loteOut.emisionDate.Value.Month;
                        _tblCiDetMovBatch.CCiTipoDato = "L";
                        _tblCiDetMovBatch.NnuNroDocCta = 0;
                        _tblCiDetMovBatch.DFmFechaDocCta = null;


						var _cuentaHaber = accountC.accountingAccountCode;
						var _tipoCuentaHaber = "";
						var _auxiliarCuentaHaber = "";
						_tipoCuentaHaber = accountC.code_Auxiliar;
						_auxiliarCuentaHaber = accountC.idAuxContable;


						_tblCiDetMovBatch.CCiCuenta = accountC.accountingAccountCode;

                        if (accountC.isAuxiliar )
                        {
                            _tblCiDetMovBatch.CCiTipoAuxiliar = _tipoCuentaHaber ?? "PFT";
                            _tblCiDetMovBatch.CCiAuxiliar = _auxiliarCuentaHaber ?? "PFT00PVT";
                            //_tblCiDetMovBatch.CdsDetalle = _proveedor.CNoProveedor;
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
                        _tblCiDetMovBatch.NNuCredito = totalCredit; 
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
                        if (accountC.id_costCenter == null)
                        {
                            _tblCiDetMovBatch.CCiCentroCosto = null;
                        }
                        else
                        {
                            _tblCiDetMovBatch.CCiCentroCosto = db.CostCenter.Where(x => x.id == (int)accountC.id_costCenter).Select(a => a.code).FirstOrDefault();

                        }

                        if (accountC.id_subCostCenter == null)
                        {
                            _tblCiDetMovBatch.CCiCentroCosto = null;
                        }
                        else
                        {
                            _tblCiDetMovBatch.CCiCentroCosto = db.CostCenter.Where(x => x.id == (int)accountC.id_costCenter).Select(a => a.code).FirstOrDefault();

                        }

                        _tblCiDetMovBatchList.Add(_tblCiDetMovBatch);


                    }

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

		private IntegrationProcessOutputAux GetInfoLiquidation(Document document)
		{
			var _integrationAux = new IntegrationProcessOutputAux();

			var _liquidationFreight = document.LiquidationFreight;

			if (_liquidationFreight != null)
			{
				_integrationAux.idObjectParent1 = _liquidationFreight.id_providertransport;
				_integrationAux.textoObjectParent1 = _liquidationFreight.InvoiceNumber;
				_integrationAux.textoObjectParent2 = "Liq.Trans.Terrestre";

				_integrationAux.decimalObjectParent1 = (decimal)_liquidationFreight.pricetotal;

				_integrationAux.ObjectChild = new List<IntegrationProcessOutputAuxDetail>();

				_liquidationFreight.LiquidationFreightDetail.ToList().ForEach(r =>
				{
					_integrationAux.ObjectChild.Add(new IntegrationProcessOutputAuxDetail
					{
						idObjectChild1 = (int)r.idOwnerVehicle
					});
				});
			}
			else
			{
				var _liquidationFreightRiver = document.LiquidationFreightRiver;

				if (_liquidationFreightRiver != null)
				{
					_integrationAux.idObjectParent1 = _liquidationFreightRiver.id_providertransport;
					_integrationAux.textoObjectParent1 = _liquidationFreightRiver.InvoiceNumber;
					_integrationAux.textoObjectParent2 = "Liq.Trans.Fluvial";
					_integrationAux.decimalObjectParent1 = (decimal)_liquidationFreightRiver.pricetotal;

					_integrationAux.ObjectChild = new List<IntegrationProcessOutputAuxDetail>();

					_liquidationFreightRiver.LiquidationFreightRiverDetail.ToList().ForEach(r =>
					{
						_integrationAux.ObjectChild.Add(new IntegrationProcessOutputAuxDetail
						{
							idObjectChild1 = (int)r.idOwnerVehicle
						});
					});
				}
			}

			return _integrationAux;
		}

		private Dictionary<string, string> GetProvisionTransportGloss(
			DBContext db,
			int id_document,
			string code_documentType,
			IntegrationProcessOutputAux integrationAuxInfo)
		{
			var _document = db.Document.FirstOrDefault(r => r.id == id_document);
			var idsPerson = integrationAuxInfo.ObjectChild
				.Select(r => r.idObjectChild1)
				.ToArray();

			var _propietarios = db.Person
				.Where(r => idsPerson.Contains(r.id))
				.Select(s => s.fullname_businessName)
				.AsEnumerable()
				.Aggregate((i, j) => i + "," + j);

            var result = (from d in db.Document
                          join lf in db.LiquidationFreightRiver on d.id equals lf.id
                          join lfd in db.LiquidationFreightRiverDetail on lf.id equals lfd.id_LiquidationFreightRiver
                          join rgr in db.RemissionGuideRiver on lfd.id_remisionGuideRiver equals rgr.id
                          where d.id == id_document
                          select new
                          {
                              id_personProcessPlant = rgr.id_personProcessPlant,
                              RemisionGuide = rgr.Document.sequential
                          }).ToList();

            if (result == null || result.Count == 0)
            {
                result = (from d in db.Document
                          join lf in db.LiquidationFreight on d.id equals lf.id
                          join lfd in db.LiquidationFreightDetail on lf.id equals lfd.id_LiquidationFreight
                          join rgr in db.RemissionGuide on lfd.id_remisionGuide equals rgr.id
                          where d.id == id_document
                          select new
                          {
                              id_personProcessPlant = rgr.id_personProcessPlant,
                              RemisionGuide = rgr.Document.sequential
                          }).ToList();
            }


            var plantasProceso = db.Person
                .Where(p => p.isActive && p.Rol.FirstOrDefault(r => r.name.Equals("Planta Proceso")) != null)
                .Select(p => new
                {
                    p.id,
                    planta = p.identification_number,
                    name = p.fullname_businessName,
                    processPlant = p.processPlant ?? p.fullname_businessName,
                })
                .ToList();


            var numeroLiquidacion = _document.number;
			var factura = integrationAuxInfo.textoObjectParent1;
			var nombresPropietarios = _propietarios;
			var fechaLiquidacion = _document.emissionDate.ToString("dd/MM/yyyy") ?? "";
			var tipoLiquidacion = integrationAuxInfo.textoObjectParent2;


			var returnData = new Dictionary<string, string>();

            var person = db.Person
				.FirstOrDefault(r => r.id == integrationAuxInfo.idObjectParent1);

            if (person != null)
            {

                returnData.Add("Proveedor:", person.fullname_businessName);
            }


            if (factura != null)
            {
                returnData.Add("Factura", factura);
            }

            returnData.Add("Número de Liquidación", numeroLiquidacion);
            returnData.Add("Fecha Liquidación", fechaLiquidacion);
			returnData.Add("Guía de Remisión", string.Join("-", result.Select(c => c.RemisionGuide)));
            returnData.Add("Tipo", tipoLiquidacion);
            returnData.Add("Proceso", plantasProceso.Where(p => p.id == result[0].id_personProcessPlant).Select(x => x.processPlant).FirstOrDefault());







            return returnData;
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