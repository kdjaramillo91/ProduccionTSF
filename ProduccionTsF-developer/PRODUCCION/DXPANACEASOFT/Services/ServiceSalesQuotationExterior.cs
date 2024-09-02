using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Services
{
	public static class ServiceSalesQuotationExterior
	{
		public static void UpdateValuesFromInvoiceExterior(Invoice invoice, DBContext db)
		{
			var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == invoice.Document.id_documentOrigen);
			if (proforma == null)
				return;

			var noExistDetails = 0;
			var state = "TOTAL";

			foreach (var proformaDetail in proforma.Invoice.InvoiceDetail)
			{
				var invoiceDetail = invoice.InvoiceDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
				if (invoiceDetail != null)
				{
					proformaDetail.proformaUsedNumBoxes += invoiceDetail.numBoxes;
					proformaDetail.proformaPendingNumBoxes = proformaDetail.numBoxes + proformaDetail.proformaNumBoxesPlusMinus - proformaDetail.proformaUsedNumBoxes;

					proforma.usedBoxes += invoiceDetail.numBoxes;
					proforma.pendingBoxes = proforma.totalBoxes + proforma.numBoxesPlusMinus - proforma.usedBoxes;

                    //Actualizar Descuento
                    proformaDetail.proformaUsedDiscount += invoiceDetail.discount;
                    proformaDetail.proformaPendingDiscount = proformaDetail.discount - proformaDetail.proformaUsedDiscount;

                    //if (proformaDetail.proformaPendingNumBoxes > 0)
                    //{
                    //	state = "PARCIAL";
                    //}
                }
				else
				{
					noExistDetails++;
				}
			}

			if (proforma.pendingBoxes > 0)
			{
				state = "PARCIAL";
			}

			if (proforma.Invoice.InvoiceDetail.Count == noExistDetails || proforma.usedBoxes == 0)
				state = "NINGUNO";

			switch (state)
			{
				case "PARCIAL":
					{
						var transaccionStatePartial =
							db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("01"));
						if (transaccionStatePartial == null)
							throw new Exception(
								"Estado Trnaccional con codigo 01 (PARCIAL) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStatePartial.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
				case "TOTAL":
					{
						var transaccionStateTotal = db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("02"));
						if (transaccionStateTotal == null)
							throw new Exception(
								"Estado Trnaccional con codigo 02 (TOTAL) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStateTotal.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
				default:
					{

						var transaccionStateNinguno =
							db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("00"));
						if (transaccionStateNinguno == null)
							throw new Exception(
								"Estado Trnaccional con codigo 00 (NINGUNO) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStateNinguno.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
			}
		}

		public static void UpdateValuesFromInvoiceComercial(InvoiceCommercial invoice, DBContext db)
		{
			var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == invoice.Document.id_documentOrigen);
			if (proforma == null)
				return;

			var noExistDetails = 0;
			var state = "TOTAL";

			foreach (var proformaDetail in proforma.Invoice.InvoiceDetail)
			{
				var invoiceDetail = invoice.InvoiceCommercialDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
				if (invoiceDetail != null)
				{
					proformaDetail.proformaUsedNumBoxes += invoiceDetail.numBoxes;
					proformaDetail.proformaPendingNumBoxes = proformaDetail.numBoxes + proformaDetail.proformaNumBoxesPlusMinus - proformaDetail.proformaUsedNumBoxes;

					proforma.usedBoxes += invoiceDetail.numBoxes;
					proforma.pendingBoxes = proforma.totalBoxes + proforma.numBoxesPlusMinus - proforma.usedBoxes;

                    //if (proformaDetail.proformaPendingNumBoxes > 0)
                    //{
                    //	state = "PARCIAL";
                    //}
                }
				else
				{
					noExistDetails++;
				}
			}

			if (proforma.pendingBoxes > 0)
			{
				state = "PARCIAL";
			}

			if (proforma.Invoice.InvoiceDetail.Count == noExistDetails || proforma.usedBoxes == 0)
				state = "NINGUNO";

			switch (state)
			{
				case "PARCIAL":
					{
						var transaccionStatePartial = db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("01"));
						if (transaccionStatePartial == null)
							throw new Exception("Estado Trnaccional con codigo 01 (PARCIAL) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStatePartial.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
				case "TOTAL":
					{
						var transaccionStateTotal = db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("02"));
						if (transaccionStateTotal == null)
							throw new Exception("Estado Trnaccional con codigo 02 (TOTAL) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStateTotal.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
				default:
					{

						var transaccionStateNinguno = db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("00"));
						if (transaccionStateNinguno == null)
							throw new Exception("Estado Trnaccional con codigo 00 (NINGUNO) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStateNinguno.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
			}
		}

		public static void UpdateValuesFromInvoiceExteriorAnul(Invoice invoice, DBContext db)
		{
			var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == invoice.Document.id_documentOrigen);
			if (proforma == null)
				return;

			var noExistDetails = 0;
			var state = "TOTAL";

			foreach (var proformaDetail in proforma.Invoice.InvoiceDetail)
			{
				var invoiceDetail = invoice.InvoiceDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
				if (invoiceDetail != null)
				{
					proformaDetail.proformaUsedNumBoxes -= invoiceDetail.numBoxes;
					proformaDetail.proformaPendingNumBoxes = proformaDetail.numBoxes + proformaDetail.proformaNumBoxesPlusMinus - proformaDetail.proformaUsedNumBoxes;

					proforma.usedBoxes -= invoiceDetail.numBoxes;
					proforma.pendingBoxes = proforma.totalBoxes + proforma.numBoxesPlusMinus - proforma.usedBoxes;

                    //Actualizar Descuento
                    proformaDetail.proformaUsedDiscount -= invoiceDetail.discount;
                    proformaDetail.proformaPendingDiscount = proformaDetail.discount - proformaDetail.proformaUsedDiscount;

                    //if (proformaDetail.proformaUsedNumBoxes > 0)
                    //{
                    //	state = "PARCIAL";
                    //}
                }
                else
				{
					noExistDetails++;
				}
			}

			if (proforma.pendingBoxes > 0)
			{
				state = "PARCIAL";
			}
			if (proforma.Invoice.InvoiceDetail.Count == noExistDetails || proforma.usedBoxes == 0)
				state = "NINGUNO";

			switch (state)
			{
				case "PARCIAL":
					{
						var transaccionStatePartial =
							db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("01"));
						if (transaccionStatePartial == null)
							throw new Exception(
								"Estado Trnaccional con codigo 01 (PARCIAL) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStatePartial.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
				case "TOTAL":
					{
						var transaccionStateTotal = db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("02"));
						if (transaccionStateTotal == null)
							throw new Exception(
								"Estado Trnaccional con codigo 02 (TOTAL) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStateTotal.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
				default:
					{

						var transaccionStateNinguno =
							db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("00"));
						if (transaccionStateNinguno == null)
							throw new Exception(
								"Estado Trnaccional con codigo 00 (NINGUNO) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStateNinguno.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
			}
		}

		public static void UpdateValuesFromInvoiceComercialAnul(InvoiceCommercial invoice, DBContext db)
		{
			var proforma = db.SalesQuotationExterior.FirstOrDefault(s => s.id == invoice.Document.id_documentOrigen);
			if (proforma == null)
				return;

			var noExistDetails = 0;
			var state = "TOTAL";

			foreach (var proformaDetail in proforma.Invoice.InvoiceDetail)
			{
				var invoiceDetail = invoice.InvoiceCommercialDetail.FirstOrDefault(d => d.isActive && d.id_item == proformaDetail.id_item);
				if (invoiceDetail != null)
				{
					proformaDetail.proformaUsedNumBoxes -= invoiceDetail.numBoxes;
					proformaDetail.proformaPendingNumBoxes = proformaDetail.numBoxes + proformaDetail.proformaNumBoxesPlusMinus - proformaDetail.proformaUsedNumBoxes;

					proforma.usedBoxes -= invoiceDetail.numBoxes;
					proforma.pendingBoxes = proforma.totalBoxes + proforma.numBoxesPlusMinus - proforma.usedBoxes;

					//if (proformaDetail.proformaUsedNumBoxes > 0)
					//{
					//	state = "PARCIAL";
					//}
				}
				else
				{
					noExistDetails++;
				}
			}

			if (proforma.pendingBoxes > 0)
			{
				state = "PARCIAL";
			}

			if (proforma.Invoice.InvoiceDetail.Count == noExistDetails || proforma.usedBoxes == 0)
				state = "NINGUNO";

			switch (state)
			{
				case "PARCIAL":
					{
						var transaccionStatePartial =
							db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("01"));
						if (transaccionStatePartial == null)
							throw new Exception(
								"Estado Trnaccional con codigo 01 (PARCIAL) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStatePartial.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
				case "TOTAL":
					{
						var transaccionStateTotal = db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("02"));
						if (transaccionStateTotal == null)
							throw new Exception(
								"Estado Trnaccional con codigo 02 (TOTAL) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStateTotal.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
				default:
					{

						var transaccionStateNinguno =
							db.DocumentTransactionState.FirstOrDefault(dc => dc.code.Equals("00"));
						if (transaccionStateNinguno == null)
							throw new Exception(
								"Estado Trnaccional con codigo 00 (NINGUNO) no existe en la base de datos. Tabla DocumentTransactionState");

						proforma.Invoice.Document.id_documentTransactionState = transaccionStateNinguno.id;

						db.SalesQuotationExterior.Attach(proforma);
						db.Entry(proforma).State = EntityState.Modified;

						break;
					}
			}
		}
	}
}