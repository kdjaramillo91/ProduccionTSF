using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using Utilitarios.Logs;
using Utilitarios.ProdException;

namespace DXPANACEASOFT.Services
{
    public class ServiceLot
    {
        private static string RUTALOG;

        private static string getRutaLog()
        {
            if (RUTALOG == null)
            {
                RUTALOG = ConfigurationManager.AppSettings.Get("rutalog");
            }
            return RUTALOG;
        }

        public static Lot CreateLot(DBContext db, User activeUser, Company activeCompany, EmissionPoint activeEmissionPoint)
        {
            string result = "";
            Lot lot = new Lot();
            try
            {
                #region Document

                Document document = new Document();
                document.id_userCreate = activeUser.id;
                document.dateCreate = DateTime.Now;
                document.id_userUpdate = activeUser.id;
                document.dateUpdate = DateTime.Now;

                DocumentType documentType = db.DocumentType.FirstOrDefault(dt => dt.code == "36");//Lote
                if (documentType == null)
                {
                    throw new Exception("No se puedo Crear el Documento Lote porque no existe el Tipo de Documento: Lote con Código: 36, configúrelo e inténtelo de nuevo");
                }
                document.id_documentType = documentType.id;
                document.DocumentType = documentType;
                document.sequential = GetDocumentSequential(document.id_documentType, db, activeCompany);
                document.number = GetDocumentNumber(document.id_documentType, db, activeCompany, activeEmissionPoint);


                DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE
                if (documentState == null)
                {
                    throw new Exception("No se puedo Crear el Documento Lote porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
                }
                document.id_documentState = documentState.id;
                document.DocumentState = documentState;

                document.EmissionPoint = db.EmissionPoint.FirstOrDefault(e => e.id == activeEmissionPoint.id);
                document.id_emissionPoint = activeEmissionPoint.id;

                document.emissionDate = DateTime.Now;
                document.description = null;

                //Actualiza Secuencial
                if (documentType != null)
                {
                    documentType.currentNumber = documentType.currentNumber + 1;
                    //db.DocumentType.Attach(documentType);
                    db.Entry(documentType).State = EntityState.Modified;
                }

                #endregion

                #region Lot
                
                lot.id = document.id;
                lot.Document = document;

                lot.number = document.number;
                lot.id_company = activeCompany.id;

                db.Lot.Add(lot);

                #endregion
            }
            catch (Exception e)
            {
                result = e.Message;
                MetodosEscrituraLogs.EscribeExcepcionLogNest(e, getRutaLog(), "ServiceLot", "Producion", seccion: result);
                throw;
            }

            return lot;
        }

         


        #region Auxiliar

        protected static int GetDocumentSequential(int id_documentType, DBContext db, Company activeCompany)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.id == id_documentType && d.id_company == activeCompany.id);
            return documentType?.currentNumber ?? 0;
        }

        protected static string GetDocumentNumber(int id_documentType, DBContext db, Company activeCompany, EmissionPoint activeEmissionPoint)
        {
            string number = GetDocumentSequential(id_documentType, db, activeCompany).ToString().PadLeft(9, '0');
            string documentNumber = string.Empty;
            documentNumber = $"{activeEmissionPoint.BranchOffice.code.ToString().PadLeft(3, '0')}-{activeEmissionPoint.code.ToString().PadLeft(3, '0')}-{number}";
            return documentNumber;
        }

        #endregion

    }
}