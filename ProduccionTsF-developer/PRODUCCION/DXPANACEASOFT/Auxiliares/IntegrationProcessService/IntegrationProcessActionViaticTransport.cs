using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessActionViaticTransport : IIntegrationProcessActionOutput
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
				var codeStatusValidate1 = integrationConfig.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault(r => r.code == "VAT1")?.valueDirectValidate;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Viatico Terrestre, Integracion No Definida");
				}

				var idViaticosValidados = db.RemissionGuideCustomizedViaticPersonalAssigned
					.Where(r => r.isActive
							&& r.id_PaymentState != null
							&& r.tbsysCatalogState.codeState == codeStatusValidate1
							&& r.hasPayment)
					.Select(r => r.id_ViaticPersonalAssigned)
					.ToList();

				preDocument = preDocument
					.Where(d => idViaticosValidados.Contains(d.id))
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
			var viaticPersonalAssigned = db.RemissionGuideCustomizedViaticPersonalAssigned
				.FirstOrDefault(r => r.id_ViaticPersonalAssigned == id_document);

			// Por cada Documento cada uno de ellos
			var idRemissionGuide = viaticPersonalAssigned.id_RemissionGuide;

			var glosa = db.RemissionGuideAssignedStaff
				.Where(r => r.id_remissionGuide == idRemissionGuide
						&& r.isActive
						&& r.viaticPrice > 0)
				.AsEnumerable()
				.Select(r => "Guia de Remisión: " + r.RemissionGuide.Document.number + "  ,  "
							+ "Empleado: " + r.Person.fullname_businessName + "  ,  "
							+ "Rol: " + r.RemissionGuideAssignedStaffRol.description + "  ,  "
							+ "Tipo Asignación: " + r.RemissionGuideTravelType.description + "  ,  "
							+ "Fecha Aprobación: " + (viaticPersonalAssigned.dateApproved.Value.ToString("dd/MM/yyyy") ?? "") + "  ,  "
							+ "Valor: " + String.Format("{0:0,0}", r.viaticPrice.Value)
				)
				.Aggregate((i, j) => i + Environment.NewLine + j);

			return new Dictionary<string, string>
			{
				{ "Asignación", glosa },
			};
		}

		decimal IIntegrationProcessActionOutput.GetTotalValue(Document document)
		{
			return document
				.RemissionGuideCustomizedViaticPersonalAssigned
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
				var _person = db.Person.FirstOrDefault(s => s.id == r.idObjectParent1);
				if (_person != null)
				{
					_descriptionGroup = _person.fullname_businessName + "(" + r.textoObjectParent1 + ")";

				}
				_valueTotal = r.ObjectChild.Sum(s => s.decimalObjectChild1);

				var _printGroup = new IntegrationProcessPrintGroup()
				{
					descriptionGroup = _descriptionGroup,
					valueTotal = _valueTotal,
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
					.FirstOrDefault(r => r.code == EnumIntegrationProcess.CodeIntegrationProcess.AnticipoViaticoTransporte);

				if (integrationDocumentConfig == null)
				{
					throw new Exception("Configuración proceso de Integración para Anticipo de Viático, No definido");
				}

				var codeStatusValidate1 = integrationDocumentConfig.tbsysIntegrationDocumentValidationConfig
					.FirstOrDefault(r => r.code == "VAT1")?
					.valueDirectValidate;

				if (codeStatusValidate1 == null)
				{
					throw new Exception("Validación Viático Terrestre, proceso de integración, No Definida");
				}

				integrationProcessDetailList.ForEach(r =>
				{
					var ierr = 0;

					var _viaticPersonalAssigned = db.RemissionGuideCustomizedViaticPersonalAssigned
						.FirstOrDefault(s => s.id_ViaticPersonalAssigned == r.id_Document
									&& s.isActive
									&& s.id_PaymentState != null
									&& s.valueTotal > 0);

					if (_viaticPersonalAssigned == null)
					{
						throw new Exception("Viático #" + r.id_Document + ". No activo, sin estado o sin valores.");
					}

					if (_viaticPersonalAssigned.tbsysCatalogState.codeState.Trim() != codeStatusValidate1)
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

				var _integrationProcessOutputAuxList = GroupData(db, integrationProcessDetailList);

				if (_integrationProcessOutputAuxList == null || _integrationProcessOutputAuxList.Count == 0)
				{

					throw new Exception("No existen Infomación relacionada a los Documentos a Integrar");
				}


				// Prepar la informacion de Output
				foreach (var outputAux in _integrationProcessOutputAuxList)
				{
					var _processOutput = new IntegrationProcessOutput();
					_processOutput.IntegrationProcessOutputDocument = new List<IntegrationProcessOutputDocument>();

					var _document = db.Document.FirstOrDefault(r => r.id == outputAux.idObjectParent2);
					var _emissionPoint = _document.EmissionPoint;

					var _employee = db.Person.FirstOrDefault(r => r.id == outputAux.idObjectParent1);
					var codigoTipoIdentificacion = _employee.IdentificationType?.code ?? "";
					var identificacion = _employee.identification_number ?? "";


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

					if (_employee == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							outputAux.idObjectParent3,
							"Empleado no Definido",
							EnumIntegrationProcess.SourceReference.DetalleLoteIntegracion,
							EnumIntegrationProcess.SourceMessage.CreacionRegistrosSalida,
							_parametersDetail);

						throw new Exception("Empleado no Definido");
					}

					var idsGuiasRemision = outputAux.ObjectChild
						.Select(r => r.idObjectChild2)
						.ToArray();

					var guiasRemision = db.Document
										.Where(r => idsGuiasRemision.Contains(r.id))
										.Select(r => r.number)
										.AsEnumerable()
										.Aggregate((i, j) => i + ", " + j);

					var valorAnticipo = outputAux.ObjectChild.Sum(r => r.decimalObjectChild1);
					_processOutput.code_division = _branchOffice.Division?.code ?? "";
					_processOutput.code_branchOffice = _branchOffice.code;
					_processOutput.id_emisionPoint = _emissionPoint.id;
					_processOutput.code_company = _emissionPoint.BranchOffice.Division.Company.code;

					_processOutput.code_documentType = documentType.code;
					_processOutput.documentNumber = _document.id;
					//_processOutput.documentNumber = _document.sequential;
					_processOutput.descriptionGloss =
						"Lote Int.:" + integrationProcessLote.codeLote + "," +
						"Empleado:" + _employee.fullname_businessName + "," +
						"Guias Remision:" + guiasRemision + "," +
						"Rol:" + outputAux.textoObjectParent1;

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

					// 
					var maxLength = (identificacion.Length > 10) ? 10 : (identificacion.Length);

					_processOutput.auxChar1 = identificacion.Substring(0, maxLength);

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



				var _cabMovimientoBatchList = new List<TblCpCabMovimientoBatch>();
				var _detMovimientoBatchList = new List<TblCpDetMovimientoBatch>();

				var dbIntegration = new DBContextIntegration();

				foreach (var loteOut in _integrationProcessOutputs)
				{
					var rootMessage = " Documento de salida: " + loteOut.documentNumberLegal;

					var _cabMovimiento = new TblCpCabMovimientoBatch();
					_cabMovimiento.CCiCia = loteOut.code_company;
					_cabMovimiento.CCiDivision = loteOut.code_division;
					_cabMovimiento.CCiSucursal = loteOut.code_branchOffice;
					_cabMovimiento.NNUAnio = loteOut.emisionDate.Value.Year;
					_cabMovimiento.NNuPeriodo = loteOut.emisionDate.Value.Month;
					_cabMovimiento.CCiTipoComprobante = "CJ";
					_cabMovimiento.NNuComprobante = loteOut.id;
					_cabMovimiento.CCtTipoDoc = "VA";

					var stringLength = loteOut.descriptionGloss?.Count() ?? 0;
					var maxLength = (stringLength == 0) ? 0 : ((stringLength > 224) ? 224 : stringLength);
					var glosaExt = loteOut.descriptionGloss ?? "";


					_cabMovimiento.CDsConcepto = glosaExt.Substring(0, maxLength) ?? "";
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


					var _cuentaCab = dbIntegration.TblciCuenta
						.Where(r => r.CCiCuenta == "11010102001")
						.Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
						.FirstOrDefault();

					if (_cuentaCab == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							id_IntegrationProcessLote,
                            "Cuenta 11010102001 No definida." + rootMessage,
							EnumIntegrationProcess.SourceReference.LoteIntegracion,
							EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
							typesMessageError);

						throw new Exception("Cuenta 11010102001 No definida." + rootMessage);
					}

					_cabMovimiento.CCiCuenta = "11010102001";


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

					_cabMovimiento.CCiProveedor = _proveedor.CCiProveedor;

					var _cuentaProveedor = dbIntegration.TblGeCuentaProveedor
						.FirstOrDefault(r => r.CCiCia == loteOut.code_company
										&& r.CCiDivision == loteOut.code_division
										&& r.CCiSucursal == loteOut.code_branchOffice
										&& r.CCiProveedor == _proveedor.CCiProveedor
										&& r.CCtCuenta.Trim() == "PD");

					if (_cuentaProveedor == null)
					{
						_cuentaProveedor = dbIntegration.TblGeCuentaProveedor
							.FirstOrDefault(r => r.CCiProveedor == _proveedor.CCiProveedor
												&& r.CCtCuenta.Trim() == "PD");

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

					_cabMovimiento.CCiTipoAuxiliar = "CJC";
					_cabMovimiento.CCiAuxiliar = "CJC00004";

					//if (_cuentaCab.BSnAceptaAux == true)
					//{
					//	_cabMovimiento.CCiTipoAuxiliar = _cuentaProveedor.CCiTipoAuxiliar;
					//	_cabMovimiento.CCiAuxiliar = _cuentaProveedor.CCiAuxiliar;
					//}
					//else
					//{
					//	_cabMovimiento.CCiTipoAuxiliar = "";
					//	_cabMovimiento.CCiAuxiliar = "";
					//}
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
					_cabMovimiento.CCtCaja = "C";
					_cabMovimiento.CCiBanco = "99";
					_cabMovimiento.CCiCtaCte = "04";
					_cabMovimiento.CDsReferencia = loteOut.auxChar1;
					_cabMovimiento.CCiSustento = "";
					_cabMovimiento.NNuValorAdicional = 0;
					_cabMovimiento.ccisistema = "PP";
					_cabMovimiento.CSnProvision = null;
					_cabMovimiento.CciTipoAnticipo = null;

					_cabMovimientoBatchList.Add(_cabMovimiento);



					// Detalle 1
					var _detMovimiento = new TblCpDetMovimientoBatch();

					_detMovimiento.CCiCia = loteOut.code_company;
					_detMovimiento.CCiDivision = loteOut.code_division;
					_detMovimiento.CCiSucursal = loteOut.code_branchOffice;
					_detMovimiento.NNUAnio = loteOut.emisionDate.Value.Year;
					_detMovimiento.NNuPeriodo = loteOut.emisionDate.Value.Month;
					_detMovimiento.CCiTipoComprobante = "CJ";
					_detMovimiento.NNuComprobante = loteOut.id; //(int)LoteOut.documentNumber;
					_detMovimiento.NNuRegistro = 1;
					_detMovimiento.CCtCuenta = "AC";
					_detMovimiento.CCiCuenta = "11020601004";

					var _cuenta = dbIntegration.TblciCuenta
						.Where(r => r.CCiCuenta == "11020601004")
						.Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
						.FirstOrDefault();
					if (_cuenta == null)
					{
						saveMessageError(
							db,
							id_ActiveUser,
							id_IntegrationProcessLote,
                            "Cuenta 11020101001 No definida" + rootMessage,
							EnumIntegrationProcess.SourceReference.LoteIntegracion,
							EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
							typesMessageError);

						throw new Exception("Cuenta 11020101001 No definida." + rootMessage);
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


					// Detalle 2
					_detMovimiento = new TblCpDetMovimientoBatch();
					_detMovimiento.CCiCia = loteOut.code_company;
					_detMovimiento.CCiDivision = loteOut.code_division;
					_detMovimiento.CCiSucursal = loteOut.code_branchOffice;
					_detMovimiento.NNUAnio = loteOut.emisionDate.Value.Year;
					_detMovimiento.NNuPeriodo = loteOut.emisionDate.Value.Month;
					_detMovimiento.CCiTipoComprobante = "CJ";
					_detMovimiento.NNuComprobante = loteOut.id; //(int)LoteOut.documentNumber;
					_detMovimiento.NNuRegistro = 2;
					_detMovimiento.CCtCuenta = "P";
					_detMovimiento.CCiCuenta = "11010102001";

					_cuenta = dbIntegration.TblciCuenta
						.Where(r => r.CCiCuenta == "11010102001")
						.Select(r => new { r.CCiCuenta, r.BSnAceptaAux })
						.FirstOrDefault();

					if (_cuenta == null)
					{
						saveMessageError(
						   db,
						   id_ActiveUser,
						   id_IntegrationProcessLote,
						   "Cuenta 210102011 No definida" + rootMessage,
						   EnumIntegrationProcess.SourceReference.LoteIntegracion,
						   EnumIntegrationProcess.SourceMessage.ValidacionTransferencia,
						   typesMessageError);

						throw new Exception("Cuenta 210102011 No definida." + rootMessage);
					}


					if (_cuenta.BSnAceptaAux == true)
					{
						_detMovimiento.CCiTipoAuxiliar = "CJC";
						_detMovimiento.CCiAuxiliar = "CJC00004";
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
						TipoComprobante = "CJ",
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

		private List<IntegrationProcessOutputAux> GroupData(
			DBContext db,
			List<IntegrationProcessDetail> integrationProcessDetailList)
		{
			var _outputAux = new List<IntegrationProcessOutputAux>();

			// Preparar la Agrupacion por Persona
			foreach (var processDetail in integrationProcessDetailList)
			{
				var document = db.Document
					.FirstOrDefault(r => r.id == processDetail.id_Document);
				if (document == null)
				{
					throw new Exception("Documento no Definido");
				}

				var idRemissionGuide = db.RemissionGuideCustomizedViaticPersonalAssigned
					.FirstOrDefault(r => r.id_ViaticPersonalAssigned == processDetail.id_Document)?
					.id_RemissionGuide;

				if (idRemissionGuide == null)
				{
					throw new Exception("Registro no Definido");
				}

				db.RemissionGuideAssignedStaff
					.Where(r => r.id_remissionGuide == idRemissionGuide
							&& r.isActive
							&& r.viaticPrice > 0)
					.ToList()
					.ForEach(r =>
					{
						var _integrationProcessOutputAux = _outputAux
							.FirstOrDefault(s => s.idObjectParent1 == r.id_person);

						if (_integrationProcessOutputAux == null)
						{
							_integrationProcessOutputAux = new IntegrationProcessOutputAux()
							{
								idObjectParent1 = r.id_person,
								idObjectParent2 = processDetail.id_Document,
								idObjectParent3 = processDetail.id,
								textoObjectParent1 = r.RemissionGuideAssignedStaffRol.name
							};
							_outputAux.Add(_integrationProcessOutputAux);
						}

						var _integrationProcessOutputAuxDetail = new IntegrationProcessOutputAuxDetail()
						{
							idObjectChild1 = processDetail.id,
							idObjectChild2 = idRemissionGuide.Value,
							fechaObjectChild1 = document.dateUpdate.Date,
							decimalObjectChild1 = r.viaticPrice.Value,
						};

						_integrationProcessOutputAux.ObjectChild.Add(_integrationProcessOutputAuxDetail);
					});
			}

			return _outputAux;
		}
	}
}