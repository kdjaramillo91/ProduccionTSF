using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models.IntegrationProcessDetailDTO;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessActionBillingOfMaterialsAndSupplies : IIntegrationProcessActionOutput
	{
		string IIntegrationProcessActionOutput.FindDocument(
			DBContext db, tbsysIntegrationDocumentConfig integrationConfig, ref IEnumerable<Document> preDocument)
		{
			var msgGeneral = "";

			try
			{
				// Filtramos los documentos en estado AUTORIZADO
				var idDocumentStateValidate = GetIdDocumentStateValidate(db, integrationConfig);

				preDocument = preDocument
					.Where(r => r.id_documentState == idDocumentStateValidate)
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

			var numero = document.number;
			var fechaEmision = document.emissionDate.ToString("dd/MM/yyyy");
			var proveedor = document.LiquidationMaterialSupplies.Provider.Person.fullname_businessName;

			return new Dictionary<string, string>
			{
				{ "No. Liquidación", numero },
				{ "Fecha de emisión", fechaEmision },
				{ "Proveedor", proveedor },
			};
		}

		decimal IIntegrationProcessActionOutput.GetTotalValue(Document document)
		{
			var liquidation = document.LiquidationMaterialSupplies;
			return liquidation.Total;
		}

		List<IntegrationProcessPrintGroup> IIntegrationProcessActionOutput.PrintGroup(
			DBContext db, List<IntegrationProcessDetail> integrationProcessDetailList)
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

			//TODO...
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

			//TODO...
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

			var process = db.IntegrationProcess.FirstOrDefault(p => p.id == id_IntegrationProcessLote);
			if (process == null)
				return msgGeneral;

			var details = process.IntegrationProcessDetail;
			IntegrationProcessDetailFmiDTO.GetListIntegrationProcessDetailDTO(details);

			var ListIntegrationProcessDetailFmiDTO = IntegrationProcessDetailFmiDTO.ListIntegrationProcessDetailFmiDTO;

			//TODO migrar informacion a tabla de contabilidad.
			using (var dbCI = new DBContextIntegration())
			{
				using (var trans = dbCI.Database.BeginTransaction())
				{
					try
					{
						//parametros parametrizables
						var typesMessageError = db.AdvanceParameters
							.FirstOrDefault(r => r.code.Equals(EnumIntegrationProcess.ParametersIntegration.SourceMessage))
							.AdvanceParametersDetail
							.ToList();

						if (typesMessageError == null)
						{
							throw new Exception("Parámetro de Integración \"Origen de Mensaje\" no ha sido definido.");
						}

						var _listaPrecio = db.Setting.FirstOrDefault(fod => fod.code.Equals("LISTPR"));
						var _cuenta = db.Setting.FirstOrDefault(fod => fod.code.Equals("CUENTA"));

						// Verificamos la existencia del parametro para lista de precio
						if (_listaPrecio == null)
						{
							var messageError = $"No se ha parametrizado lista de precio para el código: LISTPR.";

							throw new Exception(messageError);
						}

						// Verificamos la existencia del parametro para la cuenta contable
						if (_listaPrecio == null)
						{
							var messageError = $"No se ha parametrizado cuenta contable para el código: CUENTA.";

							throw new Exception(messageError);
						}

						foreach (var infoFact in ListIntegrationProcessDetailFmiDTO)
						{
							//validacion de Cliente
							var _cliente = dbCI.TblGeCliente
								.Where(r => r.CCiIdentificacion == infoFact.Proveedor.Person.identification_number)
								.Select(r => new { r.CCiCliente, r.CCiIdentificacion, r.CNuTelefono1, r.CCiCiudad, r.CDsDireccion, r.CCiFormaPago })
								.FirstOrDefault();

							if (_cliente == null)
							{
								var messageError = $"No se ha encontrado el cliente con el número de identificación: {infoFact.Proveedor.Person.identification_number}";

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

							foreach (var infoFactProduct in infoFact.DetailsInfoFactProducto)
							{
								// Verificamos la existencia del producto
								var _producto = dbCI.TblIvProducto
									.Select(r => new { r.CCiProducto })
									.FirstOrDefault(r => r.CCiProducto == infoFactProduct.Item.auxCode);

								if (_producto == null)
								{
									var messageError = $"No se ha encontrado el producto: {infoFactProduct.Item.name}";

									throw new Exception(messageError);
								}

								var infoFactMigration = new WRK_Proc_FaFacturaProduccion
								{
									NnuRegistro = dbCI.WRK_Proc_FaFacturaProduccion.Count() + 1,
									NnuControl = infoFact.Id,
									CdsEstacion = "",
									CciCliente = _cliente.CCiCliente.ToString(),
									DFxFechaFactura = infoFact.FechaEmision,
									CDxObservacion = "Fecha de emisión: " + infoFact.FechaEmision + " Cliente: " + infoFact.Proveedor.Person.fullname_businessName,
									CDsReferencia = infoFact.Proveedor.Person.address,
									CDsReferencia2 = "",
									CDsReferencia3 = "",
									CCiVendedor = "",
									CCiFormaPago = "01",
									CCiProducto = _producto.CCiProducto,
									NNuCantidad = infoFactProduct.Cantidad,
									NNuPreUnitario = infoFactProduct.PrecioUnitario,
									CsnGrabaIva = "N",//infoFactProduct.Item.auxCode, //revisar tabla graba iva
									NnuPorDescuento = 0,
									CdxObservacionPro = "",
									CCtFactura = "PRO",
									CCiDpto = "",
									CCiProyecto = "",
									CCiSubProyecto = "",
									CCtTipoDocumento = infoFact.TipoDocumento,
									CNuListaPrecio = _listaPrecio.value.ToString(),
									CCiCuenta = _cuenta.value.ToString(),

								};

								dbCI.WRK_Proc_FaFacturaProduccion.Add(infoFactMigration);
							}

						}

						dbCI.SaveChanges();
						trans.Commit();

					}
					catch (Exception)
					{
						trans.Rollback();
						throw;
					}
				}
			}

			return msgGeneral;
		}

		private int GetIdDocumentStateValidate(DBContext db, tbsysIntegrationDocumentConfig integrationConfig)
		{
			var codeDocumentStateValidate = integrationConfig
				.tbsysIntegrationDocumentValidationConfig
				.FirstOrDefault(r => r.code == "VFMI1")?
				.valueDirectValidate;

			if (codeDocumentStateValidate == null)
			{
				throw new Exception("Código de estado de liquidación para integrar NO definido");
			}

			var idDocumentStateValidate = db.DocumentState
				.FirstOrDefault(r => r.code == codeDocumentStateValidate)?
				.id ?? 0;

			if (idDocumentStateValidate <= 0)
			{
				throw new Exception("ID de estado de liquidación para integrar NO definido");
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
	}
}