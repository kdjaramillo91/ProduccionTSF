using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using Dapper;

namespace DXPANACEASOFT.Dapper
{
    public static class SalesQuotationExteriorDapper
    {
        public static IEnumerable<SalesQuotationExterior> RecuperarProformasPorFacturar()
        {
            IEnumerable<SalesQuotationExterior> salesQuotations = null;
            using (var db = DapperConnection.Connection())
            {
                if(db.State == ConnectionState.Closed) { db.Open(); }

                var id_documentType = db.ExecuteScalar<int>("SELECT id FROM DocumentType WHERE code = '131'");
                var id_documentTransactionState = db.ExecuteScalar<int>("SELECT id FROM DocumentTransactionState WHERE code = '02'");
                var idsDocumentState = db.Query<DocumentState>(
                        @"SELECT * FROM DocumentState WHERE code in ('01','03')")
                    .Select(e => e.id);

                var idsDocument = db.Query<Document>(
                        @"  SELECT * FROM Document 
                            WHERE id_documentType = @id_documentType
                            AND id_documentTransactionState <> @id_documentTransactionState
                            AND id_documentState in @idsDocumentState
                        ", new { id_documentType, id_documentTransactionState, idsDocumentState })
                    .Select(e => e.id);

                var query =
                    @"
                        SELECT	    
                            sq.*, d.*, ds.*, dts.*, i.*, p.*
                        FROM	    SalesQuotationExterior sq
                        INNER JOIN  Document d on sq.id = d.id
                        INNER JOIN	DocumentState ds ON d.id_documentState = ds.id
                        INNER JOIN	DocumentTransactionState dts ON d.id_documentTransactionState = dts.id
                        INNER JOIN	Invoice i ON d.id = i.id
                        INNER JOIN	Person p ON p.id = i.id_buyer
                        WHERE       d.id in @idsDocument
                    ";

                salesQuotations = db.Query<SalesQuotationExterior, 
                    Document, DocumentState, DocumentTransactionState, Invoice, Person, SalesQuotationExterior>
                    (query, (salesQuotationExterior, document, documentState, documentTransactionState, invoice, person) =>
                    {
                        if (invoice != null) { 
                            salesQuotationExterior.Invoice = invoice; 
                        };
                        if (invoice != null && person != null) { 
                            salesQuotationExterior.Invoice.Person = person;
                        };
                        if (invoice != null && document != null) { 
                            salesQuotationExterior.Invoice.Document = document; 
                        }
                        if (document != null && documentState != null) { 
                            salesQuotationExterior.Invoice.Document.DocumentState = documentState; 
                        }
                        if (document != null && documentTransactionState != null) { 
                            salesQuotationExterior.Invoice.Document.DocumentTransactionState = documentTransactionState; 
                        }

                        return salesQuotationExterior;
                    }, new { idsDocument }, splitOn: "id");

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return salesQuotations;
        }
    }
}